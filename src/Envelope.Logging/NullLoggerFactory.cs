using Microsoft.Extensions.Logging;

namespace Envelope.Logging;

public class NullLoggerFactory : ILoggerFactory
{
	public static readonly NullLoggerFactory Instance = new();

	public ILogger CreateLogger(string name)
	{
		return NullLogger.Instance;
	}

	public void AddProvider(ILoggerProvider provider)
	{
	}

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
	public void Dispose()
	{
	}
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
}
