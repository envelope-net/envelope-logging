using Microsoft.Extensions.Logging;
using Envelope.Trace;
using System.Runtime.CompilerServices;

namespace Envelope.Logging;

public class MethodLogScope<TIdentity> : IDisposable
	where TIdentity : struct
{
	private readonly IDisposable _logScope;

	public ITraceInfo<TIdentity> TraceInfo { get; }

	public MethodLogScope(ITraceInfo<TIdentity> traceInfo, IDisposable logScope)
	{
		TraceInfo = traceInfo ?? throw new ArgumentNullException(nameof(traceInfo));
		_logScope = logScope;
	}

	private bool _disposed;
	protected virtual void Dispose(bool disposing)
	{
		if (_disposed)
			return;

		_disposed = true;

		if (disposing)
			_logScope?.Dispose();
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	public static MethodLogScope<TIdentity> Create(
		string sourceSystemName,
		ILogger logger,
		IEnumerable<MethodParameter>? methodParameters = null,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> Create(sourceSystemName, logger, (ITraceInfo<TIdentity>?)null, methodParameters, memberName, sourceFilePath, sourceLineNumber);

	public static MethodLogScope<TIdentity> Create(
		string sourceSystemName,
		ILogger logger,
		MethodLogScope<TIdentity>? methodLogScope,
		IEnumerable<MethodParameter>? methodParameters = null,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> Create(sourceSystemName, logger, methodLogScope?.TraceInfo, methodParameters, memberName, sourceFilePath, sourceLineNumber);

	public static MethodLogScope<TIdentity> Create(
		string sourceSystemName,
		ILogger logger,
		ITraceInfo<TIdentity>? previousTraceInfo,
		IEnumerable<MethodParameter>? methodParameters = null,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
	{
		if (logger == null)
			throw new ArgumentNullException(nameof(logger));

		var traceInfo =
			new TraceInfoBuilder<TIdentity>(
				sourceSystemName,
				new TraceFrameBuilder()
					.CallerMemberName(memberName)
					.CallerFilePath(sourceFilePath)
					.CallerLineNumber(sourceLineNumber == 0 ? (int?)null : sourceLineNumber)
					.MethodParameters(methodParameters)
					.Build(),
				previousTraceInfo)
				.Build();

		var disposable = logger.BeginScope(new Dictionary<string, Guid?>
		{
			[nameof(ILogMessage<TIdentity>.TraceInfo.TraceFrame.MethodCallId)] = traceInfo.TraceFrame.MethodCallId,
			[nameof(ILogMessage<TIdentity>.TraceInfo.CorrelationId)] = traceInfo.CorrelationId
		});

		var scope = new MethodLogScope<TIdentity>(traceInfo, disposable);
		return scope;
	}
}
