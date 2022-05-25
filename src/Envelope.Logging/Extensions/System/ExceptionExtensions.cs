using Envelope.Logging;

namespace Envelope.Extensions;

public static class ExceptionExtensions
{
	private const string ENVELOPE_LOG_MESSAGE = nameof(ENVELOPE_LOG_MESSAGE);

	public static T AppendLogMessage<T, TIdentity>(this T exception, ILogMessage<TIdentity> logMessage)
		where T : Exception
		where TIdentity : struct
	{
		if (exception == null)
			throw new ArgumentNullException(nameof(exception));

		if (logMessage != null)
		{
			if (exception.Data.Contains(ENVELOPE_LOG_MESSAGE))
			{
				var value = exception.Data[ENVELOPE_LOG_MESSAGE];
				if (value is ErrorMessage<TIdentity> msg)
				{
					msg.Detail = string.IsNullOrWhiteSpace(msg.Detail)
						? $"---NEXT LOG MESSAGE---{Environment.NewLine}{logMessage.FullMessage}"
						: $"{msg.Detail}{Environment.NewLine}---NEXT LOG MESSAGE---{Environment.NewLine}{logMessage.FullMessage}";
				}
			}
			else
			{
				exception.Data[ENVELOPE_LOG_MESSAGE] = logMessage;
			}
		}

		return exception;
	}

	public static T SetLogMessage<T, TIdentity>(this T exception, ILogMessage<TIdentity>? logMessage)
		where T : Exception
		where TIdentity : struct
	{
		if (exception == null)
			throw new ArgumentNullException(nameof(exception));

		exception.Data[ENVELOPE_LOG_MESSAGE] = logMessage;

		return exception;
	}

	public static ILogMessage<TIdentity>? GetLogMessage<TIdentity>(this Exception exception)
		where TIdentity : struct
	{
		if (exception == null)
			throw new ArgumentNullException(nameof(exception));

		return exception.Data.Contains(ENVELOPE_LOG_MESSAGE)
			? exception.Data[ENVELOPE_LOG_MESSAGE] as ILogMessage<TIdentity>
			: null;
	}
}
