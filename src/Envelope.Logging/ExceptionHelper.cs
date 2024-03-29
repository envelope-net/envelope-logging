﻿using Envelope.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Envelope.Logging;

public static partial class ExceptionHelper
{
	[return: NotNullIfNotNull("logMessage")]
	public static Exception? ToException(ILogMessage logMessage)
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
	public static TException? ToException<TException>(ILogMessage logMessage, Func<string, TException> exceptionFactory)
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
