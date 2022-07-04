using Microsoft.Extensions.Logging;
using Envelope.Identity;
using Envelope.Trace;
using System.Runtime.CompilerServices;

namespace Envelope.Logging.Extensions;

public static partial class LoggerExtensions
{
	public static IDisposable BeginMethodCallScope(this ILogger logger, ITraceInfo traceInfo)
		=> traceInfo?.TraceFrame == null
			? throw new ArgumentNullException(nameof(traceInfo))
			: logger.BeginScope(new Dictionary<string, Guid?>
			{
				[nameof(ILogMessage.TraceInfo.TraceFrame.MethodCallId)] = traceInfo.TraceFrame.MethodCallId,
				[nameof(ILogMessage.TraceInfo.CorrelationId)] = traceInfo.CorrelationId
			});

	public static IDisposable BeginMethodCallScope(this ILogger logger, ITraceFrame traceFrame)
		=> traceFrame == null
			? throw new ArgumentNullException(nameof(traceFrame))
			: logger.BeginScope(new Dictionary<string, Guid>
				{
					[nameof(ILogMessage.TraceInfo.TraceFrame.MethodCallId)] = traceFrame.MethodCallId
				});

	public static IDisposable BeginMethodCallScope(this ILogger logger, Guid methodCallId)
		=> logger.BeginScope(new Dictionary<string, Guid>
		{
			[nameof(ILogMessage.TraceInfo.TraceFrame.MethodCallId)] = methodCallId
		});

	public static void LogTraceMessage(this ILogger logger, ILogMessage message, bool skipIfAlreadyLogged = true)
	{
		if (message == null)
			throw new ArgumentNullException(nameof(message));

		if (!logger.IsEnabled(LogLevel.Trace))
			return;

		message.LogLevel = LogLevel.Trace;

		if (!skipIfAlreadyLogged || !message.IsLogged)
			logger.LogTrace($"{LoggerSettings.LogMessage_Template}", message.ToDictionary());

		message.IsLogged = true;
	}

	public static ILogMessage? PrepareTraceMessage(this ILogger logger, ITraceInfo traceInfo, Action<LogMessageBuilder>? messageBuilder, bool onlyIfEnabled = false)
	{
		if (onlyIfEnabled && !logger.IsEnabled(LogLevel.Trace))
			return null;

		var builder = new LogMessageBuilder(traceInfo)
			.LogLevel(LogLevel.Trace);

		messageBuilder?.Invoke(builder);
		var message = builder.Build();

		return message;
	}

	public static ILogMessage? LogTraceMessage(this ILogger logger, ITraceInfo traceInfo, Action<LogMessageBuilder> messageBuilder, bool skipIfAlreadyLogged = true)
	{
		if (messageBuilder == null)
			throw new ArgumentNullException(nameof(messageBuilder));

		if (!logger.IsEnabled(LogLevel.Trace))
			return null;

		var builder = new LogMessageBuilder(traceInfo)
			.LogLevel(LogLevel.Trace);

		messageBuilder.Invoke(builder);
		var message = builder.Build();

		if (!skipIfAlreadyLogged || !message.IsLogged)
			logger.LogTrace($"{LoggerSettings.LogMessage_Template}", message.ToDictionary());

		message.IsLogged = true;
		return message;
	}

	public static ILogMessage? LogTraceMessage(
		this ILogger logger,
		string sourceSystemName,
		Action<LogMessageBuilder> messageBuilder,
		bool skipIfAlreadyLogged = true,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> LogTraceMessage(logger, TraceInfo.Create(sourceSystemName, (EnvelopePrincipal?)null, null, null, memberName, sourceFilePath, sourceLineNumber), messageBuilder, skipIfAlreadyLogged);

	public static ILogMessage? LogTraceMessage(this ILogger logger, MethodLogScope scope, Action<LogMessageBuilder> messageBuilder, bool skipIfAlreadyLogged = true)
	{
		if (scope?.TraceInfo == null)
			throw new ArgumentException($"{nameof(scope)}.{nameof(scope.TraceInfo)} == null", nameof(scope));

		return LogTraceMessage(logger, scope.TraceInfo, messageBuilder, skipIfAlreadyLogged);
	}

	public static void LogDebugMessage(this ILogger logger, ILogMessage message, bool skipIfAlreadyLogged = true)
	{
		if (message == null)
			throw new ArgumentNullException(nameof(message));

		if (!logger.IsEnabled(LogLevel.Debug))
			return;

		message.LogLevel = LogLevel.Debug;

		if (!skipIfAlreadyLogged || !message.IsLogged)
			logger.LogDebug($"{LoggerSettings.LogMessage_Template}", message.ToDictionary());

		message.IsLogged = true;
	}

	public static ILogMessage? PrepareDebugMessage(this ILogger logger, ITraceInfo traceInfo, Action<LogMessageBuilder> messageBuilder, bool onlyIfEnabled = false)
	{
		if (onlyIfEnabled && !logger.IsEnabled(LogLevel.Debug))
			return null;

		var builder = new LogMessageBuilder(traceInfo)
			.LogLevel(LogLevel.Debug);

		messageBuilder?.Invoke(builder);
		var message = builder.Build();

		return message;
	}

	public static ILogMessage? LogDebugMessage(this ILogger logger, ITraceInfo traceInfo, Action<LogMessageBuilder> messageBuilder, bool skipIfAlreadyLogged = true)
	{
		if (messageBuilder == null)
			throw new ArgumentNullException(nameof(messageBuilder));

		if (!logger.IsEnabled(LogLevel.Debug))
			return null;

		var builder = new LogMessageBuilder(traceInfo)
			.LogLevel(LogLevel.Debug);

		messageBuilder.Invoke(builder);
		var message = builder.Build();

		if (!skipIfAlreadyLogged || !message.IsLogged)
			logger.LogDebug($"{LoggerSettings.LogMessage_Template}", message.ToDictionary());

		message.IsLogged = true;
		return message;
	}

	public static ILogMessage? LogDebugMessage(
		this ILogger logger,
		string sourceSystemName,
		Action<LogMessageBuilder> messageBuilder,
		bool skipIfAlreadyLogged = true,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> LogDebugMessage(logger, TraceInfo.Create(sourceSystemName, (EnvelopePrincipal?)null, null, null, memberName, sourceFilePath, sourceLineNumber), messageBuilder, skipIfAlreadyLogged);

	public static ILogMessage? LogDebugMessage(this ILogger logger, MethodLogScope scope, Action<LogMessageBuilder> messageBuilder, bool skipIfAlreadyLogged = true)
	{
		if (scope?.TraceInfo == null)
			throw new ArgumentException($"{nameof(scope)}.{nameof(scope.TraceInfo)} == null", nameof(scope));

		return LogDebugMessage(logger, scope.TraceInfo, messageBuilder, skipIfAlreadyLogged);
	}

	public static void LogInformationMessage(this ILogger logger, ILogMessage message, bool skipIfAlreadyLogged = true)
	{
		if (message == null)
			throw new ArgumentNullException(nameof(message));

		if (!logger.IsEnabled(LogLevel.Information))
			return;

		message.LogLevel = LogLevel.Information;

		if (!skipIfAlreadyLogged || !message.IsLogged)
			logger.LogInformation($"{LoggerSettings.LogMessage_Template}", message.ToDictionary());

		message.IsLogged = true;
	}

	public static ILogMessage? PrepareInformationMessage(this ILogger logger, ITraceInfo traceInfo, Action<LogMessageBuilder> messageBuilder, bool onlyIfEnabled = false)
	{
		if (onlyIfEnabled && !logger.IsEnabled(LogLevel.Information))
			return null;

		var builder = new LogMessageBuilder(traceInfo)
			.LogLevel(LogLevel.Information);

		messageBuilder?.Invoke(builder);
		var message = builder.Build();

		return message;
	}

	public static ILogMessage? LogInformationMessage(this ILogger logger, ITraceInfo traceInfo, Action<LogMessageBuilder> messageBuilder, bool skipIfAlreadyLogged = true)
	{
		if (messageBuilder == null)
			throw new ArgumentNullException(nameof(messageBuilder));

		if (!logger.IsEnabled(LogLevel.Information))
			return null;

		var builder = new LogMessageBuilder(traceInfo)
			.LogLevel(LogLevel.Information);

		messageBuilder.Invoke(builder);
		var message = builder.Build();

		if (!skipIfAlreadyLogged || !message.IsLogged)
			logger.LogInformation($"{LoggerSettings.LogMessage_Template}", message.ToDictionary());

		message.IsLogged = true;
		return message;
	}

	public static ILogMessage? LogInformationMessage(
		this ILogger logger,
		string sourceSystemName,
		Action<LogMessageBuilder> messageBuilder,
		bool skipIfAlreadyLogged = true,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> LogInformationMessage(logger, TraceInfo.Create(sourceSystemName, (EnvelopePrincipal?)null, null, null, memberName, sourceFilePath, sourceLineNumber), messageBuilder, skipIfAlreadyLogged);

	public static ILogMessage? LogInformationMessage(this ILogger logger, MethodLogScope scope, Action<LogMessageBuilder> messageBuilder, bool skipIfAlreadyLogged = true)
	{
		if (scope?.TraceInfo == null)
			throw new ArgumentException($"{nameof(scope)}.{nameof(scope.TraceInfo)} == null", nameof(scope));

		return LogInformationMessage(logger, scope.TraceInfo, messageBuilder, skipIfAlreadyLogged);
	}

	public static void LogWarningMessage(this ILogger logger, ILogMessage message, bool skipIfAlreadyLogged = true)
	{
		if (message == null)
			throw new ArgumentNullException(nameof(message));

		if (!logger.IsEnabled(LogLevel.Warning))
			return;

		message.LogLevel = LogLevel.Warning;

		if (!skipIfAlreadyLogged || !message.IsLogged)
			logger.LogWarning($"{LoggerSettings.LogMessage_Template}", message.ToDictionary());

		message.IsLogged = true;
	}

	public static ILogMessage? PrepareWarningMessage(this ILogger logger, ITraceInfo traceInfo, Action<LogMessageBuilder> messageBuilder, bool onlyIfEnabled = false)
	{
		if (onlyIfEnabled && !logger.IsEnabled(LogLevel.Warning))
			return null;

		var builder = new LogMessageBuilder(traceInfo)
			.LogLevel(LogLevel.Warning);

		messageBuilder?.Invoke(builder);
		var message = builder.Build();

		return message;
	}

	public static ILogMessage? LogWarningMessage(this ILogger logger, ITraceInfo traceInfo, Action<LogMessageBuilder> messageBuilder, bool skipIfAlreadyLogged = true)
	{
		if (messageBuilder == null)
			throw new ArgumentNullException(nameof(messageBuilder));

		if (!logger.IsEnabled(LogLevel.Warning))
			return null;

		var builder = new LogMessageBuilder(traceInfo)
			.LogLevel(LogLevel.Warning);

		messageBuilder.Invoke(builder);
		var message = builder.Build();

		if (!skipIfAlreadyLogged || !message.IsLogged)
			logger.LogWarning($"{LoggerSettings.LogMessage_Template}", message.ToDictionary());

		message.IsLogged = true;
		return message;
	}

	public static ILogMessage? LogWarningMessage(
		this ILogger logger,
		string sourceSystemName,
		Action<LogMessageBuilder> messageBuilder,
		bool skipIfAlreadyLogged = true,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> LogWarningMessage(logger, TraceInfo.Create(sourceSystemName, (EnvelopePrincipal?)null, null, null, memberName, sourceFilePath, sourceLineNumber), messageBuilder, skipIfAlreadyLogged);

	public static ILogMessage? LogWarningMessage(this ILogger logger, MethodLogScope scope, Action<LogMessageBuilder> messageBuilder, bool skipIfAlreadyLogged = true)
	{
		if (scope?.TraceInfo == null)
			throw new ArgumentException($"{nameof(scope)}.{nameof(scope.TraceInfo)} == null", nameof(scope));

		return LogWarningMessage(logger, scope.TraceInfo, messageBuilder, skipIfAlreadyLogged);
	}

	public static void LogErrorMessage(this ILogger logger, IErrorMessage message, bool skipIfAlreadyLogged = true)
	{
		if (message == null)
			throw new ArgumentNullException(nameof(message));

		message.LogLevel = LogLevel.Error;

		if (!skipIfAlreadyLogged || !message.IsLogged)
			logger.LogError($"{LoggerSettings.LogMessage_Template}", message.ToDictionary());

		message.IsLogged = true;
	}

	public static IErrorMessage? PrepareErrorMessage(this ILogger logger, ITraceInfo traceInfo, Action<ErrorMessageBuilder> messageBuilder, bool onlyIfEnabled = false)
	{
		if (onlyIfEnabled && !logger.IsEnabled(LogLevel.Error))
			return null;

		var builder = new ErrorMessageBuilder(traceInfo)
			.LogLevel(LogLevel.Error);

		messageBuilder?.Invoke(builder);
		var message = builder.Build();

		return message;
	}

	public static IErrorMessage LogErrorMessage(this ILogger logger, ITraceInfo traceInfo, Action<ErrorMessageBuilder> messageBuilder, bool skipIfAlreadyLogged = true)
	{
		if (messageBuilder == null)
			throw new ArgumentNullException(nameof(messageBuilder));

		var builder = new ErrorMessageBuilder(traceInfo)
			.LogLevel(LogLevel.Error);

		messageBuilder.Invoke(builder);
		var message = builder.Build();

		if (!skipIfAlreadyLogged || !message.IsLogged)
			logger.LogError($"{LoggerSettings.LogMessage_Template}", message.ToDictionary());

		message.IsLogged = true;
		return message;
	}

	public static IErrorMessage LogErrorMessage(
		this ILogger logger,
		string sourceSystemName,
		Action<ErrorMessageBuilder> messageBuilder,
		bool skipIfAlreadyLogged = true,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> LogErrorMessage(logger, TraceInfo.Create(sourceSystemName, (EnvelopePrincipal?)null, null, null, memberName, sourceFilePath, sourceLineNumber), messageBuilder, skipIfAlreadyLogged);

	public static IErrorMessage LogErrorMessage(this ILogger logger, MethodLogScope scope, Action<ErrorMessageBuilder> messageBuilder, bool skipIfAlreadyLogged = true)
	{
		if (scope?.TraceInfo == null)
			throw new ArgumentException($"{nameof(scope)}.{nameof(scope.TraceInfo)} == null", nameof(scope));

		return LogErrorMessage(logger, scope.TraceInfo, messageBuilder, skipIfAlreadyLogged);
	}

	public static void LogCriticalMessage(this ILogger logger, IErrorMessage message, bool skipIfAlreadyLogged = true)
	{
		if (message == null)
			throw new ArgumentNullException(nameof(message));

		message.LogLevel = LogLevel.Critical;

		if (!skipIfAlreadyLogged || !message.IsLogged)
			logger.LogCritical($"{LoggerSettings.LogMessage_Template}", message.ToDictionary());

		message.IsLogged = true;
	}

	public static IErrorMessage? PrepareCriticalMessage(this ILogger logger, ITraceInfo traceInfo, Action<ErrorMessageBuilder> messageBuilder, bool onlyIfEnabled = false)
	{
		if (onlyIfEnabled && !logger.IsEnabled(LogLevel.Critical))
			return null;

		var builder = new ErrorMessageBuilder(traceInfo)
			.LogLevel(LogLevel.Critical);

		messageBuilder?.Invoke(builder);
		var message = builder.Build();

		return message;
	}

	public static IErrorMessage LogCriticalMessage(this ILogger logger, ITraceInfo traceInfo, Action<ErrorMessageBuilder> messageBuilder, bool skipIfAlreadyLogged = true)
	{
		if (messageBuilder == null)
			throw new ArgumentNullException(nameof(messageBuilder));

		var builder = new ErrorMessageBuilder(traceInfo)
			.LogLevel(LogLevel.Critical);

		messageBuilder.Invoke(builder);
		var message = builder.Build();

		if (!skipIfAlreadyLogged || !message.IsLogged)
			logger.LogCritical($"{LoggerSettings.LogMessage_Template}", message.ToDictionary());

		message.IsLogged = true;
		return message;
	}

	public static IErrorMessage LogCriticalMessage(
		this ILogger logger,
		string sourceSystemName,
		Action<ErrorMessageBuilder> messageBuilder,
		bool skipIfAlreadyLogged = true,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)

		=> LogCriticalMessage(logger, TraceInfo.Create(sourceSystemName, (EnvelopePrincipal?)null, null, null, memberName, sourceFilePath, sourceLineNumber), messageBuilder, skipIfAlreadyLogged);

	public static IErrorMessage LogCriticalMessage(this ILogger logger, MethodLogScope scope, Action<ErrorMessageBuilder> messageBuilder, bool skipIfAlreadyLogged = true)
	{
		if (scope?.TraceInfo == null)
			throw new ArgumentException($"{nameof(scope)}.{nameof(scope.TraceInfo)} == null", nameof(scope));

		return LogCriticalMessage(logger, scope.TraceInfo, messageBuilder, skipIfAlreadyLogged);
	}

	//public static void LogEnvironmentInfo(this ILogger logger)
	//	=> LogEnvironmentInfo(logger, EnvironmentInfoProvider.GetEnvironmentInfo());

	//public static void LogEnvironmentInfo(this ILogger logger, EnvironmentInfo environmentInfo)
	//{
	//	logger.LogInformation($"{LoggerSettings.EnvironmentInfo_Template}", environmentInfo.ToDictionary());
	//}
}
