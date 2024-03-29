﻿using Envelope.Data;
using Envelope.Extensions;
using Envelope.Logging.Internal;

namespace Envelope.Logging;

public class LogWriter : ILogWriter, IDisposable
{
	private static ILogWriter _instance = SilentLogWriter.Instance;

	public static ILogWriter Instance
	{
		get => _instance;
		set => _instance = value ?? throw new ArgumentNullException(nameof(value));
	}

	private readonly Dictionary<Type, IBatchWriter> _batchWriters;

	internal LogWriter(Dictionary<Type, IBatchWriter> batchWriters)
	{
		_batchWriters = batchWriters ?? throw new ArgumentNullException(nameof(batchWriters));
	}

	public void Write<T>(T obj)
	{
		if (obj == null)
			return;

		var batchWriterType = typeof(T);
		if (_batchWriters.TryGetValue(batchWriterType, out IBatchWriter? writer))
		{
			if (writer is IBatchWriter<T> batchWriter)
			{
				batchWriter.Write(obj);
			}

			throw new InvalidOperationException($"Cannot get {nameof(IBatchWriter<T>)} for type {batchWriterType.FullName}");
		}

		throw new NotSupportedException($"No writer configured for type {batchWriterType.FullName}");
	}

	public static void CloseAndFlush()
	{
		var logWriter = Interlocked.Exchange(ref _instance, SilentLogWriter.Instance);
		logWriter?.Dispose();
	}

	private bool _disposed;
	protected virtual void Dispose(bool disposing)
	{
		if (_disposed)
			return;

		_disposed = true;

		if (disposing)
		{
			foreach (var batchWriter in _batchWriters.Values)
			{
				try
				{
					batchWriter?.Dispose();
				}
				catch (Exception ex)
				{
					var msg = string.Format($"{nameof(LogWriter)}: Disposing {nameof(batchWriter)} '{batchWriter?.GetType().FullName ?? "null"}': {ex.ToStringTrace()}");
					Serilog.Log.Logger.Error(ex, msg);
				}
			}
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}
