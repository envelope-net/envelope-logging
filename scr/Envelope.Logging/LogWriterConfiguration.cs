using Envelope.Data;

namespace Envelope.Logging;

public class LogWriterConfiguration
{
	protected readonly Dictionary<Type, IBatchWriter> _batchWriters = new();

	public LogWriterConfiguration SetBatchWriter<T>(IBatchWriter<T> batchWriter)
	{
		_batchWriters.Add(typeof(T), batchWriter);
		return this;
	}

	public LogWriter? CreateLogWriter()
	{
		if (_batchWriters.Count == 0)
			return null;

		return new LogWriter(_batchWriters);
	}
}
