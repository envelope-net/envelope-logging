using Microsoft.Extensions.Logging;
using Envelope.Trace;
using System.Runtime.CompilerServices;
using System.Text;
#if NETSTANDARD2_0 || NETSTANDARD2_1
using Newtonsoft.Json;
#elif NET6_0_OR_GREATER
using System.Text.Json;
#endif

namespace Envelope.Logging;

public class LogMessage : ILogMessage
{
	public Guid IdLogMessage { get; set; }
	public LogLevel LogLevel { get; set; }
	public int IdLogLevel => (int)LogLevel;
	public DateTimeOffset CreatedUtc { get; set; }
	public ITraceInfo TraceInfo { get; set; }
	public string? LogCode { get; set; }
	public string? ClientMessage { get; set; }
	public string? InternalMessage { get; set; }

#if NETSTANDARD2_0 || NETSTANDARD2_1
	[Newtonsoft.Json.JsonIgnore]
#elif NET6_0_OR_GREATER
	[System.Text.Json.Serialization.JsonIgnore]
#endif
	public Exception? Exception { get; internal set; }
	public bool ShouldSerializeException() => false;
	Exception? ILogMessage.Exception
	{
		get => Exception;
		set => Exception = value;
	}

	public string? StackTrace { get; set; }
	public string? Detail { get; set; }
#if NETSTANDARD2_0 || NETSTANDARD2_1
	[Newtonsoft.Json.JsonIgnore]
#elif NET6_0_OR_GREATER
	[System.Text.Json.Serialization.JsonIgnore]
#endif
	public string ClientMessageWithId => ToString(true, false, false);
	public bool ShouldSerializeClientMessageWithId() => false;

#if NETSTANDARD2_0 || NETSTANDARD2_1
	[Newtonsoft.Json.JsonIgnore]
#elif NET6_0_OR_GREATER
	[System.Text.Json.Serialization.JsonIgnore]
#endif
	public string ClientMessageWithIdAndPropName => ToString(true, true, false);
	public bool ShouldSerializeClientMessageWithIdAndPropName() => false;

#if NETSTANDARD2_0 || NETSTANDARD2_1
	[Newtonsoft.Json.JsonIgnore]
#elif NET6_0_OR_GREATER
	[System.Text.Json.Serialization.JsonIgnore]
#endif
	public string FullMessage => ToString(true, true, true);
	public bool ShouldSerializeFullMessage() => false;

	public bool IsLogged { get; set; }
	public string? CommandQueryName { get; set; }
	public Guid? IdCommandQuery { get; set; }
	public decimal? MethodCallElapsedMilliseconds { get; set; }
	public string? PropertyName { get; set; }

#if NETSTANDARD2_0 || NETSTANDARD2_1
	[Newtonsoft.Json.JsonIgnore]
#elif NET6_0_OR_GREATER
	[System.Text.Json.Serialization.JsonIgnore]
#endif
	public object? ValidationFailure { get; set; }
	public bool ShouldSerializeValidationFailure() => false;
	public string? DisplayPropertyName { get; set; }
	public bool IsValidationError { get; set; }
	public List<string>? Tags { get; set; }

	public bool DisableTransactionRollback { get; set; }

	internal LogMessage(ITraceInfo traceInfo)
	{
		IdLogMessage = Guid.NewGuid();
		CreatedUtc = DateTimeOffset.UtcNow;
		TraceInfo = traceInfo ?? throw new ArgumentNullException(nameof(traceInfo));
	}

	private static ILogMessage CreateLogMessage(
		IApplicationContext applicationContext,
		LogLevel logLevel,
		Action<LogMessageBuilder> messageBuilder,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
	{
		if (messageBuilder == null)
			throw new ArgumentNullException(nameof(messageBuilder));

		var builder = new LogMessageBuilder(Trace.TraceInfo.Create(applicationContext, null, memberName, sourceFilePath, sourceLineNumber))
			.LogLevel(logLevel);

		messageBuilder.Invoke(builder);
		var message = builder.Build();

		return message;
	}

	private static ILogMessage CreateLogMessage(
		ITraceInfo traceInfo,
		LogLevel logLevel,
		Action<LogMessageBuilder> messageBuilder)
	{
		if (traceInfo == null)
			throw new ArgumentNullException(nameof(traceInfo));

		if (messageBuilder == null)
			throw new ArgumentNullException(nameof(messageBuilder));

		var builder = new LogMessageBuilder(traceInfo)
			.LogLevel(logLevel);

		messageBuilder.Invoke(builder);
		var message = builder.Build();

		return message;
	}

	private static IErrorMessage CreateErrorMessage(
		IApplicationContext applicationContext,
		LogLevel logLevel,
		Action<ErrorMessageBuilder> messageBuilder,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
	{
		if (messageBuilder == null)
			throw new ArgumentNullException(nameof(messageBuilder));

		var builder = new ErrorMessageBuilder(Trace.TraceInfo.Create(applicationContext, null, memberName, sourceFilePath, sourceLineNumber))
			.LogLevel(logLevel);

		messageBuilder.Invoke(builder);
		var message = builder.Build();

		return message;
	}

	private static IErrorMessage CreateErrorMessage(
		ITraceInfo traceInfo,
		LogLevel logLevel,
		Action<ErrorMessageBuilder> messageBuilder)
	{
		if (traceInfo == null)
			throw new ArgumentNullException(nameof(traceInfo));

		if (messageBuilder == null)
			throw new ArgumentNullException(nameof(messageBuilder));

		var builder = new ErrorMessageBuilder(traceInfo)
			.LogLevel(logLevel);

		messageBuilder.Invoke(builder);
		var message = builder.Build();

		return message;
	}

	public static ILogMessage CreateTraceMessage(
		IApplicationContext applicationContext,
		Action<LogMessageBuilder> messageBuilder,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> CreateLogMessage(applicationContext, LogLevel.Trace, messageBuilder, memberName, sourceFilePath, sourceLineNumber);

	public static ILogMessage CreateTraceMessage(
		ITraceInfo traceInfo,
		Action<LogMessageBuilder> messageBuilder)
		=> CreateLogMessage(traceInfo, LogLevel.Trace, messageBuilder);

	public static ILogMessage CreateDebugMessage(
		IApplicationContext applicationContext,
		Action<LogMessageBuilder> messageBuilder,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> CreateLogMessage(applicationContext, LogLevel.Debug, messageBuilder, memberName, sourceFilePath, sourceLineNumber);

	public static ILogMessage CreateDebugMessage(
		ITraceInfo traceInfo,
		Action<LogMessageBuilder> messageBuilder)
		=> CreateLogMessage(traceInfo, LogLevel.Debug, messageBuilder);

	public static ILogMessage CreateInformationMessage(
		IApplicationContext applicationContext,
		Action<LogMessageBuilder> messageBuilder,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> CreateLogMessage(applicationContext, LogLevel.Information, messageBuilder, memberName, sourceFilePath, sourceLineNumber);

	public static ILogMessage CreateInformationMessage(
		ITraceInfo traceInfo,
		Action<LogMessageBuilder> messageBuilder)
		=> CreateLogMessage(traceInfo, LogLevel.Information, messageBuilder);

	public static ILogMessage CreateWarningMessage(
		IApplicationContext applicationContext,
		Action<LogMessageBuilder> messageBuilder,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> CreateLogMessage(applicationContext, LogLevel.Warning, messageBuilder, memberName, sourceFilePath, sourceLineNumber);

	public static ILogMessage CreateWarningMessage(
		ITraceInfo traceInfo,
		Action<LogMessageBuilder> messageBuilder)
		=> CreateLogMessage(traceInfo, LogLevel.Warning, messageBuilder);

	public static IErrorMessage CreateErrorMessage(
		IApplicationContext applicationContext,
		Action<ErrorMessageBuilder> messageBuilder,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> CreateErrorMessage(applicationContext, LogLevel.Error, messageBuilder, memberName, sourceFilePath, sourceLineNumber);

	public static IErrorMessage CreateErrorMessage(
		ITraceInfo traceInfo,
		Action<ErrorMessageBuilder> messageBuilder)
		=> CreateErrorMessage(traceInfo, LogLevel.Error, messageBuilder);

	public static IErrorMessage CreateCriticalMessage(
		IApplicationContext applicationContext,
		Action<ErrorMessageBuilder> messageBuilder,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> CreateErrorMessage(applicationContext, LogLevel.Critical, messageBuilder, memberName, sourceFilePath, sourceLineNumber);

	public static IErrorMessage CreateCriticalMessage(
		ITraceInfo traceInfo,
		Action<ErrorMessageBuilder> messageBuilder)
		=> CreateErrorMessage(traceInfo, LogLevel.Critical, messageBuilder);

	public IDictionary<string, object?> ToDictionary(Serializer.ISerializer? serializer = null)
	{
		var dict = new Dictionary<string, object?>
		{
			{ nameof(IdLogMessage), IdLogMessage },
			//{ nameof(LogLevel), LogLevel },
			{ nameof(IdLogLevel), IdLogLevel },
			{ nameof(CreatedUtc), CreatedUtc },
			{ nameof(TraceInfo.RuntimeUniqueKey), TraceInfo.RuntimeUniqueKey },
			{ nameof(IsValidationError), IsValidationError }
		};

		if (!string.IsNullOrWhiteSpace(LogCode))
			dict.Add(nameof(LogCode), LogCode);

		if (!string.IsNullOrWhiteSpace(ClientMessage))
			dict.Add(nameof(ClientMessage), ClientMessage);

		if (!string.IsNullOrWhiteSpace(InternalMessage))
			dict.Add(nameof(InternalMessage), InternalMessage);

		if (!string.IsNullOrWhiteSpace(TraceInfo.SourceSystemName))
			dict.Add(nameof(TraceInfo.SourceSystemName), TraceInfo.SourceSystemName);

		if (TraceInfo.TraceFrame != null)
		{
			dict.Add(nameof(TraceInfo.TraceFrame.MethodCallId), TraceInfo.TraceFrame.MethodCallId);
			dict.Add(nameof(TraceInfo.TraceFrame), $"{TraceInfo.TraceFrame}");
		}

		if (!string.IsNullOrWhiteSpace(StackTrace))
			dict.Add(nameof(StackTrace), StackTrace);

		if (!string.IsNullOrWhiteSpace(Detail))
			dict.Add(nameof(Detail), Detail);

		if (TraceInfo.IdUser.HasValue)
			dict.Add(nameof(TraceInfo.IdUser), TraceInfo.IdUser);

		if (!string.IsNullOrWhiteSpace(CommandQueryName))
			dict.Add(nameof(CommandQueryName), CommandQueryName);

		if (IdCommandQuery.HasValue)
			dict.Add(nameof(IdCommandQuery), IdCommandQuery);

		if (MethodCallElapsedMilliseconds.HasValue)
			dict.Add(nameof(MethodCallElapsedMilliseconds), MethodCallElapsedMilliseconds);

		if (!string.IsNullOrWhiteSpace(PropertyName))
			dict.Add(nameof(PropertyName), PropertyName);

		if (!string.IsNullOrWhiteSpace(DisplayPropertyName))
			dict.Add(nameof(DisplayPropertyName), DisplayPropertyName);

		if (ValidationFailure != null)
			dict.Add(nameof(ValidationFailure), ValidationFailure.ToString());

		if (TraceInfo.CorrelationId.HasValue)
			dict.Add(nameof(TraceInfo.CorrelationId), TraceInfo.CorrelationId.Value);

		if (TraceInfo.ContextProperties != null && 0 < TraceInfo.ContextProperties.Count)
#if NETSTANDARD2_0 || NETSTANDARD2_1
			dict.Add(nameof(TraceInfo.ContextProperties), JsonConvert.SerializeObject(TraceInfo.ContextProperties));
#elif NET6_0_OR_GREATER
			dict.Add(nameof(TraceInfo.ContextProperties), JsonSerializer.Serialize(TraceInfo.ContextProperties));
#endif

		if (Tags != null && 0 < Tags.Count)
#if NETSTANDARD2_0 || NETSTANDARD2_1
			dict.Add(nameof(Tags), JsonConvert.SerializeObject(Tags));
#elif NET6_0_OR_GREATER
			dict.Add(nameof(Tags), JsonSerializer.Serialize(Tags));
#endif

		return dict;
	}

	public Exception ToException()
		=> ExceptionHelper.ToException(this);

	public override string ToString()
	{
		return FullMessage;
	}

	public string ToString(bool withId, bool withPropertyName, bool withDetail)
	{
		var sb = new StringBuilder();

		bool empty = string.IsNullOrWhiteSpace(ClientMessage);
		if (!empty)
			sb.Append(ClientMessage);

		if (withPropertyName)
		{
			if (!string.IsNullOrWhiteSpace(DisplayPropertyName))
			{
				if (empty)
					sb.Append(DisplayPropertyName);
				else
					sb.Append($" - {DisplayPropertyName}");

				empty = false;
			}
		}

		if (withId)
		{
			if (empty)
				sb.Append($"ID: {IdLogMessage}");
			else
				sb.Append($" (ID: {IdLogMessage})");

			empty = false;
		}

		if (withDetail && !string.IsNullOrWhiteSpace(InternalMessage))
		{
			if (empty)
				sb.Append(InternalMessage);
			else
				sb.Append($" | {InternalMessage}");
		}

		if (withDetail && !string.IsNullOrWhiteSpace(StackTrace))
		{
			if (empty)
				sb.Append(StackTrace);
			else
				sb.Append($" | {StackTrace}");

			empty = false;
		}

		if (withDetail && !string.IsNullOrWhiteSpace(Detail))
		{
			if (empty)
				sb.Append(Detail);
			else
				sb.Append($" | {Detail}");

			if (0 < TraceInfo.ContextProperties?.Count)
			{
				if (empty)
					sb.Append(string.Join("|", TraceInfo.ContextProperties.Select(x => $"{x.Key} = {x.Value}")));
				else
					sb.Append($" | {string.Join("|", TraceInfo.ContextProperties.Select(x => $"{x.Key} = {x.Value}"))}");
			}
		}

		return sb.ToString();
	}

	public LogMessageDto ToDto(params string[] ignoredPropterties)
	{
		ignoredPropterties ??= Array.Empty<string>();
		var dto = new LogMessageDto();

		if (!ignoredPropterties.Contains(nameof(IdLogMessage)))
			dto.IdLogMessage = IdLogMessage;

		if (!ignoredPropterties.Contains(nameof(LogLevel)))
			dto.LogLevel = LogLevel;

		if (!ignoredPropterties.Contains(nameof(CreatedUtc)))
			dto.CreatedUtc = CreatedUtc;

		if (!ignoredPropterties.Contains(nameof(IsLogged)))
			dto.IsLogged = IsLogged;

		if (!ignoredPropterties.Contains(nameof(IsValidationError)))
			dto.IsValidationError = IsValidationError;

		if (!ignoredPropterties.Contains(nameof(TraceInfo)))
			dto.TraceInfo = TraceInfo;

		if (!ignoredPropterties.Contains(nameof(LogCode)))
			dto.LogCode = LogCode;

		if (!ignoredPropterties.Contains(nameof(ClientMessage)))
			dto.ClientMessage = ClientMessage;

		if (!ignoredPropterties.Contains(nameof(InternalMessage)))
			dto.InternalMessage = InternalMessage;

		if (!ignoredPropterties.Contains(nameof(Exception)))
			dto.Exception = Exception;

		if (!ignoredPropterties.Contains(nameof(StackTrace)))
			dto.StackTrace = StackTrace;

		if (!ignoredPropterties.Contains(nameof(Detail)))
			dto.Detail = Detail;

		if (!ignoredPropterties.Contains(nameof(CommandQueryName)))
			dto.CommandQueryName = CommandQueryName;

		if (!ignoredPropterties.Contains(nameof(IdCommandQuery)))
			dto.IdCommandQuery = IdCommandQuery;

		if (!ignoredPropterties.Contains(nameof(MethodCallElapsedMilliseconds)))
			dto.MethodCallElapsedMilliseconds = MethodCallElapsedMilliseconds;

		if (!ignoredPropterties.Contains(nameof(PropertyName)))
			dto.PropertyName = PropertyName;

		if (!ignoredPropterties.Contains(nameof(ValidationFailure)))
			dto.ValidationFailure = ValidationFailure;

		if (!ignoredPropterties.Contains(nameof(DisplayPropertyName)))
			dto.DisplayPropertyName = DisplayPropertyName;

		if (!ignoredPropterties.Contains(nameof(Tags)))
			dto.Tags = Tags;

		return dto;
	}

	public LogMessageDto ToClientDto()
		=> new()
			{
				IdLogMessage = IdLogMessage,
				LogLevel = LogLevel,
				CreatedUtc = CreatedUtc,
				IsLogged = IsLogged,
				IsValidationError = IsValidationError,
				LogCode = LogCode,
				ClientMessage = ClientMessage,
				PropertyName = PropertyName
			};
}
