using Microsoft.Extensions.Logging;
using Envelope.Identity;
using Envelope.Trace;
using System.Runtime.CompilerServices;

namespace Envelope.Logging.Extensions;

public static class LoggerExtensions
{
	public static IDisposable BeginMethodCallScope<TIdentity>(this ILogger logger, ITraceInfo<TIdentity> traceInfo)
		where TIdentity : struct
		=> traceInfo?.TraceFrame == null
			? throw new ArgumentNullException(nameof(traceInfo))
			: logger.BeginScope(new Dictionary<string, Guid?>
			{
				[nameof(ILogMessage<TIdentity>.TraceInfo.TraceFrame.MethodCallId)] = traceInfo.TraceFrame.MethodCallId,
				[nameof(ILogMessage<TIdentity>.TraceInfo.CorrelationId)] = traceInfo.CorrelationId
			});

	public static IDisposable BeginMethodCallScope<TIdentity>(this ILogger logger, ITraceFrame traceFrame)
		where TIdentity : struct
		=> traceFrame == null
			? throw new ArgumentNullException(nameof(traceFrame))
			: logger.BeginScope(new Dictionary<string, Guid>
				{
					[nameof(ILogMessage<TIdentity>.TraceInfo.TraceFrame.MethodCallId)] = traceFrame.MethodCallId
				});

	public static IDisposable BeginMethodCallScope<TIdentity>(this ILogger logger, Guid methodCallId)
		where TIdentity : struct
		=> logger.BeginScope(new Dictionary<string, Guid>
		{
			[nameof(ILogMessage<TIdentity>.TraceInfo.TraceFrame.MethodCallId)] = methodCallId
		});

	public static void LogTraceMessage<TIdentity>(this ILogger logger, ILogMessage<TIdentity> message, bool skipIfAlreadyLogged)
		where TIdentity : struct
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

	public static ILogMessage<TIdentity>? LogTraceMessage<TIdentity>(this ILogger logger, ITraceInfo<TIdentity> traceInfo, Action<LogMessageBuilder<TIdentity>> messageBuilder, bool skipIfAlreadyLogged)
		where TIdentity : struct
	{
		if (messageBuilder == null)
			throw new ArgumentNullException(nameof(messageBuilder));

		if (!logger.IsEnabled(LogLevel.Trace))
			return null;

		var builder = new LogMessageBuilder<TIdentity>(traceInfo)
			.LogLevel(LogLevel.Trace);

		messageBuilder.Invoke(builder);
		var message = builder.Build();

		if (!skipIfAlreadyLogged || !message.IsLogged)
			logger.LogTrace($"{LoggerSettings.LogMessage_Template}", message.ToDictionary());

		message.IsLogged = true;
		return message;
	}

	public static ILogMessage<TIdentity>? LogTraceMessage<TIdentity>(
		this ILogger logger,
		string sourceSystemName,
		Action<LogMessageBuilder<TIdentity>> messageBuilder,
		bool skipIfAlreadyLogged,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		where TIdentity : struct
		=> LogTraceMessage(logger, TraceInfo<TIdentity>.Create(sourceSystemName, (EnvelopePrincipal<TIdentity>?)null, null, null, memberName, sourceFilePath, sourceLineNumber), messageBuilder, skipIfAlreadyLogged);

	public static ILogMessage<TIdentity>? LogTraceMessage<TIdentity>(this ILogger logger, MethodLogScope<TIdentity> scope, Action<LogMessageBuilder<TIdentity>> messageBuilder, bool skipIfAlreadyLogged)
		where TIdentity : struct
	{
		if (scope?.TraceInfo == null)
			throw new ArgumentException($"{nameof(scope)}.{nameof(scope.TraceInfo)} == null", nameof(scope));

		return LogTraceMessage(logger, scope.TraceInfo, messageBuilder, skipIfAlreadyLogged);
	}

	public static void LogDebugMessage<TIdentity>(this ILogger logger, ILogMessage<TIdentity> message, bool skipIfAlreadyLogged)
		where TIdentity : struct
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

	public static ILogMessage<TIdentity>? LogDebugMessage<TIdentity>(this ILogger logger, ITraceInfo<TIdentity> traceInfo, Action<LogMessageBuilder<TIdentity>> messageBuilder, bool skipIfAlreadyLogged)
		where TIdentity : struct
	{
		if (messageBuilder == null)
			throw new ArgumentNullException(nameof(messageBuilder));

		if (!logger.IsEnabled(LogLevel.Debug))
			return null;

		var builder = new LogMessageBuilder<TIdentity>(traceInfo)
			.LogLevel(LogLevel.Debug);

		messageBuilder.Invoke(builder);
		var message = builder.Build();

		if (!skipIfAlreadyLogged || !message.IsLogged)
			logger.LogDebug($"{LoggerSettings.LogMessage_Template}", message.ToDictionary());

		message.IsLogged = true;
		return message;
	}

	public static ILogMessage<TIdentity>? LogDebugMessage<TIdentity>(
		this ILogger logger,
		string sourceSystemName,
		Action<LogMessageBuilder<TIdentity>> messageBuilder,
		bool skipIfAlreadyLogged,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		where TIdentity : struct
		=> LogDebugMessage(logger, TraceInfo<TIdentity>.Create(sourceSystemName, (EnvelopePrincipal<TIdentity>?)null, null, null, memberName, sourceFilePath, sourceLineNumber), messageBuilder, skipIfAlreadyLogged);

	public static ILogMessage<TIdentity>? LogDebugMessage<TIdentity>(this ILogger logger, MethodLogScope<TIdentity> scope, Action<LogMessageBuilder<TIdentity>> messageBuilder, bool skipIfAlreadyLogged)
		where TIdentity : struct
	{
		if (scope?.TraceInfo == null)
			throw new ArgumentException($"{nameof(scope)}.{nameof(scope.TraceInfo)} == null", nameof(scope));

		return LogDebugMessage(logger, scope.TraceInfo, messageBuilder, skipIfAlreadyLogged);
	}

	public static void LogInformationMessage<TIdentity>(this ILogger logger, ILogMessage<TIdentity> message, bool skipIfAlreadyLogged)
		where TIdentity : struct
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

	public static ILogMessage<TIdentity>? LogInformationMessage<TIdentity>(this ILogger logger, ITraceInfo<TIdentity> traceInfo, Action<LogMessageBuilder<TIdentity>> messageBuilder, bool skipIfAlreadyLogged)
		where TIdentity : struct
	{
		if (messageBuilder == null)
			throw new ArgumentNullException(nameof(messageBuilder));

		if (!logger.IsEnabled(LogLevel.Information))
			return null;

		var builder = new LogMessageBuilder<TIdentity>(traceInfo)
			.LogLevel(LogLevel.Information);

		messageBuilder.Invoke(builder);
		var message = builder.Build();

		if (!skipIfAlreadyLogged || !message.IsLogged)
			logger.LogInformation($"{LoggerSettings.LogMessage_Template}", message.ToDictionary());

		message.IsLogged = true;
		return message;
	}

	public static ILogMessage<TIdentity>? LogInformationMessage<TIdentity>(
		this ILogger logger,
		string sourceSystemName,
		Action<LogMessageBuilder<TIdentity>> messageBuilder,
		bool skipIfAlreadyLogged,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		where TIdentity : struct
		=> LogInformationMessage(logger, TraceInfo<TIdentity>.Create(sourceSystemName, (EnvelopePrincipal<TIdentity>?)null, null, null, memberName, sourceFilePath, sourceLineNumber), messageBuilder, skipIfAlreadyLogged);

	public static ILogMessage<TIdentity>? LogInformationMessage<TIdentity>(this ILogger logger, MethodLogScope<TIdentity> scope, Action<LogMessageBuilder<TIdentity>> messageBuilder, bool skipIfAlreadyLogged)
		where TIdentity : struct
	{
		if (scope?.TraceInfo == null)
			throw new ArgumentException($"{nameof(scope)}.{nameof(scope.TraceInfo)} == null", nameof(scope));

		return LogInformationMessage(logger, scope.TraceInfo, messageBuilder, skipIfAlreadyLogged);
	}

	public static void LogWarningMessage<TIdentity>(this ILogger logger, ILogMessage<TIdentity> message, bool skipIfAlreadyLogged)
		where TIdentity : struct
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

	public static ILogMessage<TIdentity>? LogWarningMessage<TIdentity>(this ILogger logger, ITraceInfo<TIdentity> traceInfo, Action<LogMessageBuilder<TIdentity>> messageBuilder, bool skipIfAlreadyLogged)
		where TIdentity : struct
	{
		if (messageBuilder == null)
			throw new ArgumentNullException(nameof(messageBuilder));

		if (!logger.IsEnabled(LogLevel.Warning))
			return null;

		var builder = new LogMessageBuilder<TIdentity>(traceInfo)
			.LogLevel(LogLevel.Warning);

		messageBuilder.Invoke(builder);
		var message = builder.Build();

		if (!skipIfAlreadyLogged || !message.IsLogged)
			logger.LogWarning($"{LoggerSettings.LogMessage_Template}", message.ToDictionary());

		message.IsLogged = true;
		return message;
	}

	public static ILogMessage<TIdentity>? LogWarningMessage<TIdentity>(
		this ILogger logger,
		string sourceSystemName,
		Action<LogMessageBuilder<TIdentity>> messageBuilder,
		bool skipIfAlreadyLogged,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		where TIdentity : struct
		=> LogWarningMessage(logger, TraceInfo<TIdentity>.Create(sourceSystemName, (EnvelopePrincipal<TIdentity>?)null, null, null, memberName, sourceFilePath, sourceLineNumber), messageBuilder, skipIfAlreadyLogged);

	public static ILogMessage<TIdentity>? LogWarningMessage<TIdentity>(this ILogger logger, MethodLogScope<TIdentity> scope, Action<LogMessageBuilder<TIdentity>> messageBuilder, bool skipIfAlreadyLogged)
		where TIdentity : struct
	{
		if (scope?.TraceInfo == null)
			throw new ArgumentException($"{nameof(scope)}.{nameof(scope.TraceInfo)} == null", nameof(scope));

		return LogWarningMessage(logger, scope.TraceInfo, messageBuilder, skipIfAlreadyLogged);
	}

	public static void LogErrorMessage<TIdentity>(this ILogger logger, IErrorMessage<TIdentity> message, bool skipIfAlreadyLogged)
		where TIdentity : struct
	{
		if (message == null)
			throw new ArgumentNullException(nameof(message));

		message.LogLevel = LogLevel.Error;

		if (!skipIfAlreadyLogged || !message.IsLogged)
			logger.LogError($"{LoggerSettings.LogMessage_Template}", message.ToDictionary());

		message.IsLogged = true;
	}

	public static IErrorMessage<TIdentity> LogErrorMessage<TIdentity>(this ILogger logger, ITraceInfo<TIdentity> traceInfo, Action<ErrorMessageBuilder<TIdentity>> messageBuilder, bool skipIfAlreadyLogged)
		where TIdentity : struct
	{
		if (messageBuilder == null)
			throw new ArgumentNullException(nameof(messageBuilder));

		var builder = new ErrorMessageBuilder<TIdentity>(traceInfo)
			.LogLevel(LogLevel.Error);

		messageBuilder.Invoke(builder);
		var message = builder.Build();

		if (!skipIfAlreadyLogged || !message.IsLogged)
			logger.LogError($"{LoggerSettings.LogMessage_Template}", message.ToDictionary());

		message.IsLogged = true;
		return message;
	}

	public static IErrorMessage<TIdentity> LogErrorMessage<TIdentity>(
		this ILogger logger,
		string sourceSystemName,
		Action<ErrorMessageBuilder<TIdentity>> messageBuilder,
		bool skipIfAlreadyLogged,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		where TIdentity : struct
		=> LogErrorMessage(logger, TraceInfo<TIdentity>.Create(sourceSystemName, (EnvelopePrincipal<TIdentity>?)null, null, null, memberName, sourceFilePath, sourceLineNumber), messageBuilder, skipIfAlreadyLogged);

	public static IErrorMessage<TIdentity> LogErrorMessage<TIdentity>(this ILogger logger, MethodLogScope<TIdentity> scope, Action<ErrorMessageBuilder<TIdentity>> messageBuilder, bool skipIfAlreadyLogged)
		where TIdentity : struct
	{
		if (scope?.TraceInfo == null)
			throw new ArgumentException($"{nameof(scope)}.{nameof(scope.TraceInfo)} == null", nameof(scope));

		return LogErrorMessage(logger, scope.TraceInfo, messageBuilder, skipIfAlreadyLogged);
	}

	public static void LogCriticalMessage<TIdentity>(this ILogger logger, IErrorMessage<TIdentity> message, bool skipIfAlreadyLogged)
		where TIdentity : struct
	{
		if (message == null)
			throw new ArgumentNullException(nameof(message));

		message.LogLevel = LogLevel.Critical;

		if (!skipIfAlreadyLogged || !message.IsLogged)
			logger.LogCritical($"{LoggerSettings.LogMessage_Template}", message.ToDictionary());

		message.IsLogged = true;
	}

	public static IErrorMessage<TIdentity> LogCriticalMessage<TIdentity>(this ILogger logger, ITraceInfo<TIdentity> traceInfo, Action<ErrorMessageBuilder<TIdentity>> messageBuilder, bool skipIfAlreadyLogged)
		where TIdentity : struct
	{
		if (messageBuilder == null)
			throw new ArgumentNullException(nameof(messageBuilder));

		var builder = new ErrorMessageBuilder<TIdentity>(traceInfo)
			.LogLevel(LogLevel.Critical);

		messageBuilder.Invoke(builder);
		var message = builder.Build();

		if (!skipIfAlreadyLogged || !message.IsLogged)
			logger.LogCritical($"{LoggerSettings.LogMessage_Template}", message.ToDictionary());

		message.IsLogged = true;
		return message;
	}

	public static IErrorMessage<TIdentity> LogCriticalMessage<TIdentity>(
		this ILogger logger,
		string sourceSystemName,
		Action<ErrorMessageBuilder<TIdentity>> messageBuilder,
		bool skipIfAlreadyLogged,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
			where TIdentity : struct
		=> LogCriticalMessage(logger, TraceInfo<TIdentity>.Create(sourceSystemName, (EnvelopePrincipal<TIdentity>?)null, null, null, memberName, sourceFilePath, sourceLineNumber), messageBuilder, skipIfAlreadyLogged);

	public static IErrorMessage<TIdentity> LogCriticalMessage<TIdentity>(this ILogger logger, MethodLogScope<TIdentity> scope, Action<ErrorMessageBuilder<TIdentity>> messageBuilder, bool skipIfAlreadyLogged)
		where TIdentity : struct
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






	public static IDisposable BeginMethodCallScope(this ILogger logger, ITraceInfo<Guid> traceInfo)
		=> BeginMethodCallScope<Guid>(logger, traceInfo);

	public static IDisposable BeginMethodCallScope(this ILogger logger, ITraceFrame traceFrame)
		=> BeginMethodCallScope<Guid>(logger, traceFrame);

	public static IDisposable BeginMethodCallScope(this ILogger logger, Guid methodCallId)
		=> BeginMethodCallScope<Guid>(logger, methodCallId);

	public static void LogTraceMessage(this ILogger logger, ILogMessage<Guid> message, bool skipIfAlreadyLogged)
		=> LogTraceMessage<Guid>(logger, message, skipIfAlreadyLogged);

	public static ILogMessage<Guid>? LogTraceMessage(this ILogger logger, ITraceInfo<Guid> traceInfo, Action<LogMessageBuilder<Guid>> messageBuilder, bool skipIfAlreadyLogged)
		=> LogTraceMessage<Guid>(logger, traceInfo, messageBuilder, skipIfAlreadyLogged);

	public static ILogMessage<Guid>? LogTraceMessage(
		this ILogger logger,
		string sourceSystemName,
		Action<LogMessageBuilder<Guid>> messageBuilder,
		bool skipIfAlreadyLogged,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> LogTraceMessage<Guid>(logger, sourceSystemName, messageBuilder, skipIfAlreadyLogged, memberName, sourceFilePath, sourceLineNumber);

	public static ILogMessage<Guid>? LogTraceMessage(this ILogger logger, MethodLogScope<Guid> scope, Action<LogMessageBuilder<Guid>> messageBuilder, bool skipIfAlreadyLogged)
		=> LogTraceMessage<Guid>(logger, scope, messageBuilder, skipIfAlreadyLogged);

	public static void LogDebugMessage(this ILogger logger, ILogMessage<Guid> message, bool skipIfAlreadyLogged)
		=> LogDebugMessage<Guid>(logger, message, skipIfAlreadyLogged);

	public static ILogMessage<Guid>? LogDebugMessage(this ILogger logger, ITraceInfo<Guid> traceInfo, Action<LogMessageBuilder<Guid>> messageBuilder, bool skipIfAlreadyLogged)
		=> LogDebugMessage<Guid>(logger, traceInfo, messageBuilder, skipIfAlreadyLogged);

	public static ILogMessage<Guid>? LogDebugMessage(
		this ILogger logger,
		string sourceSystemName,
		Action<LogMessageBuilder<Guid>> messageBuilder,
		bool skipIfAlreadyLogged,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> LogDebugMessage<Guid>(logger, sourceSystemName, messageBuilder, skipIfAlreadyLogged, memberName, sourceFilePath, sourceLineNumber);

	public static ILogMessage<Guid>? LogDebugMessage(this ILogger logger, MethodLogScope<Guid> scope, Action<LogMessageBuilder<Guid>> messageBuilder, bool skipIfAlreadyLogged)
		=> LogDebugMessage<Guid>(logger, scope, messageBuilder, skipIfAlreadyLogged);

	public static void LogInformationMessage(this ILogger logger, ILogMessage<Guid> message, bool skipIfAlreadyLogged)
		=> LogInformationMessage<Guid>(logger, message, skipIfAlreadyLogged);

	public static ILogMessage<Guid>? LogInformationMessage(this ILogger logger, ITraceInfo<Guid> traceInfo, Action<LogMessageBuilder<Guid>> messageBuilder, bool skipIfAlreadyLogged)
		=> LogInformationMessage<Guid>(logger, traceInfo, messageBuilder, skipIfAlreadyLogged);

	public static ILogMessage<Guid>? LogInformationMessage(
		this ILogger logger,
		string sourceSystemName,
		Action<LogMessageBuilder<Guid>> messageBuilder,
		bool skipIfAlreadyLogged,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> LogInformationMessage<Guid>(logger, sourceSystemName, messageBuilder, skipIfAlreadyLogged, memberName, sourceFilePath, sourceLineNumber);

	public static ILogMessage<Guid>? LogInformationMessage(this ILogger logger, MethodLogScope<Guid> scope, Action<LogMessageBuilder<Guid>> messageBuilder, bool skipIfAlreadyLogged)
		=> LogInformationMessage<Guid>(logger, scope, messageBuilder, skipIfAlreadyLogged);

	public static void LogWarningMessage(this ILogger logger, ILogMessage<Guid> message, bool skipIfAlreadyLogged)
		=> LogWarningMessage<Guid>(logger, message, skipIfAlreadyLogged);

	public static ILogMessage<Guid>? LogWarningMessage(this ILogger logger, ITraceInfo<Guid> traceInfo, Action<LogMessageBuilder<Guid>> messageBuilder, bool skipIfAlreadyLogged)
		=> LogWarningMessage<Guid>(logger, traceInfo, messageBuilder, skipIfAlreadyLogged);

	public static ILogMessage<Guid>? LogWarningMessage(
		this ILogger logger,
		string sourceSystemName,
		Action<LogMessageBuilder<Guid>> messageBuilder,
		bool skipIfAlreadyLogged,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> LogWarningMessage<Guid>(logger, sourceSystemName, messageBuilder, skipIfAlreadyLogged, memberName, sourceFilePath, sourceLineNumber);

	public static ILogMessage<Guid>? LogWarningMessage(this ILogger logger, MethodLogScope<Guid> scope, Action<LogMessageBuilder<Guid>> messageBuilder, bool skipIfAlreadyLogged)
		=> LogWarningMessage<Guid>(logger, scope, messageBuilder, skipIfAlreadyLogged);

	public static void LogErrorMessage(this ILogger logger, IErrorMessage<Guid> message, bool skipIfAlreadyLogged)
		=> LogErrorMessage<Guid>(logger, message, skipIfAlreadyLogged);

	public static IErrorMessage<Guid> LogErrorMessage(this ILogger logger, ITraceInfo<Guid> traceInfo, Action<ErrorMessageBuilder<Guid>> messageBuilder, bool skipIfAlreadyLogged)
		=> LogErrorMessage<Guid>(logger, traceInfo, messageBuilder, skipIfAlreadyLogged);

	public static IErrorMessage<Guid> LogErrorMessage(
		this ILogger logger,
		string sourceSystemName,
		Action<ErrorMessageBuilder<Guid>> messageBuilder,
		bool skipIfAlreadyLogged,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> LogErrorMessage<Guid>(logger, sourceSystemName, messageBuilder, skipIfAlreadyLogged, memberName, sourceFilePath, sourceLineNumber);

	public static IErrorMessage<Guid> LogErrorMessage(this ILogger logger, MethodLogScope<Guid> scope, Action<ErrorMessageBuilder<Guid>> messageBuilder, bool skipIfAlreadyLogged)
		=> LogErrorMessage<Guid>(logger, scope, messageBuilder, skipIfAlreadyLogged);

	public static void LogCriticalMessage(this ILogger logger, IErrorMessage<Guid> message, bool skipIfAlreadyLogged)
		=> LogCriticalMessage<Guid>(logger, message, skipIfAlreadyLogged);

	public static IErrorMessage<Guid> LogCriticalMessage(this ILogger logger, ITraceInfo<Guid> traceInfo, Action<ErrorMessageBuilder<Guid>> messageBuilder, bool skipIfAlreadyLogged)
		=> LogCriticalMessage<Guid>(logger, traceInfo, messageBuilder, skipIfAlreadyLogged);

	public static IErrorMessage<Guid> LogCriticalMessage(
		this ILogger logger,
		string sourceSystemName,
		Action<ErrorMessageBuilder<Guid>> messageBuilder,
		bool skipIfAlreadyLogged,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> LogCriticalMessage<Guid>(logger, sourceSystemName, messageBuilder, skipIfAlreadyLogged, memberName, sourceFilePath, sourceLineNumber);

	public static IErrorMessage<Guid> LogCriticalMessage(this ILogger logger, MethodLogScope<Guid> scope, Action<ErrorMessageBuilder<Guid>> messageBuilder, bool skipIfAlreadyLogged)
		=> LogCriticalMessage<Guid>(logger, scope, messageBuilder, skipIfAlreadyLogged);
}
