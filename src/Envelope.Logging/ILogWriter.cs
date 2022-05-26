namespace Envelope.Logging;

public interface ILogWriter : IDisposable
{
	void Write<T>(T obj);
}
