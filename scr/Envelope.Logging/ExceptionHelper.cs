using Envelope.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Envelope.Logging;

public static class ExceptionHelper
{
	[return: NotNullIfNotNull("logMessage")]
	public static Exception? ToException<TIdentity>(ILogMessage<TIdentity> logMessage)
		where TIdentity : struct
	{
		if (logMessage == null)
			return null;

		if (logMessage.Exception != null)
			return logMessage.Exception;

		var exception = new Exception(logMessage.ToString(true, true, true));
		exception.SetLogMessage(logMessage);
		return exception;
	}

	[return: NotNullIfNotNull("logMessage")]
	public static TException? ToException<TIdentity, TException>(ILogMessage<TIdentity> logMessage, Func<string, TException> exceptionFactory)
		where TIdentity : struct
		where TException : Exception
	{
		if (logMessage == null)
			return null;

		if (logMessage.Exception != null && logMessage.Exception is TException tException)
			return tException;

		var exception = exceptionFactory(logMessage.ToString(true, true, true));
		exception.SetLogMessage(logMessage);
		return exception;
	}
}
