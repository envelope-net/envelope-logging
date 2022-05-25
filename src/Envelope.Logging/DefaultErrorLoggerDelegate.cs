using Envelope.Extensions;

namespace Envelope.Logging;

public static class DefaultErrorLoggerDelegate
{
#pragma warning disable IDE0060 // Remove unused parameter
	public static void Log(string message, object? batchWriter, object? exception, object? @null)
#pragma warning restore IDE0060 // Remove unused parameter
	{
		string msg;
		if (exception is Exception ex)
		{
			msg = string.Format(message, batchWriter, ex.ToStringTrace());
			Serilog.Log.Logger.Error(ex, msg);
		}
		else
		{
			msg = string.Format(message, batchWriter, exception);
			Serilog.Log.Logger.Error(msg);
		}
	}
}
