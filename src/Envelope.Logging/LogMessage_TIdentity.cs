using Microsoft.Extensions.Logging;
using Envelope.Identity;
using Envelope.Trace;
using System.Runtime.CompilerServices;
using System.Text;
#if NETSTANDARD2_0 || NETSTANDARD2_1
using Newtonsoft.Json;
#elif NET6_0_OR_GREATER
using System.Text.Json;
#endif

namespace Envelope.Logging;

public class LogMessage<TIdentity> : ILogMessage<TIdentity>
	where TIdentity : struct
{
	public Guid IdLogMessage { get; set; }
	public LogLevel LogLevel { get; set; }
	public int IdLogLevel => (int)LogLevel;
	public DateTimeOffset CreatedUtc { get; set; }
	public ITraceInfo<TIdentity> TraceInfo { get; set; }
	public string? LogCode { get; set; }
	public string? ClientMessage { get; set; }
	public string? InternalMessage { get; set; }

#if NETSTANDARD2_0 || NETSTANDARD2_1
	[Newtonsoft.Json.JsonIgnore]
#elif NET6_0_OR_GREATER
	[System.Text.Json.Serialization.JsonIgnore]
#endif
	public Exception? Exception { get; internal set; }
	Exception? ILogMessage<TIdentity>.Exception
	{
		get => Exception;
		set => Exception = value;
	}

	public string? StackTrace { get; set; }
	public string? Detail { get; set; }
	public string ClientMessageWithId => ToString(true, false, false);
	public string ClientMessageWithIdAndPropName => ToString(true, true, false);
	public string FullMessage => ToString(true, true, true);
	public bool IsLogged { get; set; }
	public string? CommandQueryName { get; set; }
	public Guid? IdCommandQuery { get; set; }
	public decimal? MethodCallElapsedMilliseconds { get; set; }
	public string? PropertyName { get; set; }
	public object? ValidationFailure { get; set; }
	public string? DisplayPropertyName { get; set; }
	public bool IsValidationError { get; set; }
	public Dictionary<string, string>? CustomData { get; set; }
	public List<string>? Tags { get; set; }

	internal LogMessage(ITraceInfo<TIdentity> traceInfo)
	{
		IdLogMessage = Guid.NewGuid();
		CreatedUtc = DateTimeOffset.UtcNow;
		TraceInfo = traceInfo ?? throw new ArgumentNullException(nameof(traceInfo));
	}

	private static ILogMessage<TIdentity> CreateLogMessage(
		string sourceSystemName,
		LogLevel logLevel,
		Action<LogMessageBuilder<TIdentity>> messageBuilder,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
	{
		if (messageBuilder == null)
			throw new ArgumentNullException(nameof(messageBuilder));

		var builder = new LogMessageBuilder<TIdentity>(Trace.TraceInfo<TIdentity>.Create(sourceSystemName, (EnvelopePrincipal<TIdentity>?)null, null, null, memberName, sourceFilePath, sourceLineNumber))
			.LogLevel(logLevel);

		messageBuilder.Invoke(builder);
		var message = builder.Build();

		return message;
	}

	private static ILogMessage<TIdentity> CreateLogMessage(
		ITraceInfo<TIdentity> traceInfo,
		LogLevel logLevel,
		Action<LogMessageBuilder<TIdentity>> messageBuilder)
	{
		if (traceInfo == null)
			throw new ArgumentNullException(nameof(traceInfo));

		if (messageBuilder == null)
			throw new ArgumentNullException(nameof(messageBuilder));

		var builder = new LogMessageBuilder<TIdentity>(traceInfo)
			.LogLevel(logLevel);

		messageBuilder.Invoke(builder);
		var message = builder.Build();

		return message;
	}

	private static IErrorMessage<TIdentity> CreateErrorMessage(
		string sourceSystemName,
		LogLevel logLevel,
		Action<ErrorMessageBuilder<TIdentity>> messageBuilder,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
	{
		if (messageBuilder == null)
			throw new ArgumentNullException(nameof(messageBuilder));

		var builder = new ErrorMessageBuilder<TIdentity>(Trace.TraceInfo<TIdentity>.Create(sourceSystemName, (EnvelopePrincipal<TIdentity>?)null, null, null, memberName, sourceFilePath, sourceLineNumber))
			.LogLevel(logLevel);

		messageBuilder.Invoke(builder);
		var message = builder.Build();

		return message;
	}

	private static IErrorMessage<TIdentity> CreateErrorMessage(
		ITraceInfo<TIdentity> traceInfo,
		LogLevel logLevel,
		Action<ErrorMessageBuilder<TIdentity>> messageBuilder)
	{
		if (traceInfo == null)
			throw new ArgumentNullException(nameof(traceInfo));

		if (messageBuilder == null)
			throw new ArgumentNullException(nameof(messageBuilder));

		var builder = new ErrorMessageBuilder<TIdentity>(traceInfo)
			.LogLevel(logLevel);

		messageBuilder.Invoke(builder);
		var message = builder.Build();

		return message;
	}

	public static ILogMessage<TIdentity> CreateTraceMessage(
		string sourceSystemName,
		Action<LogMessageBuilder<TIdentity>> messageBuilder,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> CreateLogMessage(sourceSystemName, LogLevel.Trace, messageBuilder, memberName, sourceFilePath, sourceLineNumber);

	public static ILogMessage<TIdentity> CreateTraceMessage(
		ITraceInfo<TIdentity> traceInfo,
		Action<LogMessageBuilder<TIdentity>> messageBuilder)
		=> CreateLogMessage(traceInfo, LogLevel.Trace, messageBuilder);

	public static ILogMessage<TIdentity> CreateDebugMessage(
		string sourceSystemName,
		Action<LogMessageBuilder<TIdentity>> messageBuilder,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> CreateLogMessage(sourceSystemName, LogLevel.Debug, messageBuilder, memberName, sourceFilePath, sourceLineNumber);

	public static ILogMessage<TIdentity> CreateDebugMessage(
		ITraceInfo<TIdentity> traceInfo,
		Action<LogMessageBuilder<TIdentity>> messageBuilder)
		=> CreateLogMessage(traceInfo, LogLevel.Debug, messageBuilder);

	public static ILogMessage<TIdentity> CreateInformationMessage(
		string sourceSystemName,
		Action<LogMessageBuilder<TIdentity>> messageBuilder,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> CreateLogMessage(sourceSystemName, LogLevel.Information, messageBuilder, memberName, sourceFilePath, sourceLineNumber);

	public static ILogMessage<TIdentity> CreateInformationMessage(
		ITraceInfo<TIdentity> traceInfo,
		Action<LogMessageBuilder<TIdentity>> messageBuilder)
		=> CreateLogMessage(traceInfo, LogLevel.Information, messageBuilder);

	public static ILogMessage<TIdentity> CreateWarningMessage(
		string sourceSystemName,
		Action<LogMessageBuilder<TIdentity>> messageBuilder,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> CreateLogMessage(sourceSystemName, LogLevel.Warning, messageBuilder, memberName, sourceFilePath, sourceLineNumber);

	public static ILogMessage<TIdentity> CreateWarningMessage(
		ITraceInfo<TIdentity> traceInfo,
		Action<LogMessageBuilder<TIdentity>> messageBuilder)
		=> CreateLogMessage(traceInfo, LogLevel.Warning, messageBuilder);

	public static IErrorMessage<TIdentity> CreateErrorMessage(
		string sourceSystemName,
		Action<ErrorMessageBuilder<TIdentity>> messageBuilder,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> CreateErrorMessage(sourceSystemName, LogLevel.Error, messageBuilder, memberName, sourceFilePath, sourceLineNumber);

	public static IErrorMessage<TIdentity> CreateErrorMessage(
		ITraceInfo<TIdentity> traceInfo,
		Action<ErrorMessageBuilder<TIdentity>> messageBuilder)
		=> CreateErrorMessage(traceInfo, LogLevel.Error, messageBuilder);

	public static IErrorMessage<TIdentity> CreateCriticalMessage(
		string sourceSystemName,
		Action<ErrorMessageBuilder<TIdentity>> messageBuilder,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> CreateErrorMessage(sourceSystemName, LogLevel.Critical, messageBuilder, memberName, sourceFilePath, sourceLineNumber);

	public static IErrorMessage<TIdentity> CreateCriticalMessage(
		ITraceInfo<TIdentity> traceInfo,
		Action<ErrorMessageBuilder<TIdentity>> messageBuilder)
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

		if (CustomData != null && 0 < CustomData.Count)
#if NETSTANDARD2_0 || NETSTANDARD2_1
			dict.Add(nameof(CustomData), JsonConvert.SerializeObject(CustomData));
#elif NET6_0_OR_GREATER
			dict.Add(nameof(CustomData), JsonSerializer.Serialize(CustomData));
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
		}

		return sb.ToString();
	}
}
