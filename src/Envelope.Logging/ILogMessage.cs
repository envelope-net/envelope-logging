using Envelope.Trace;
using Microsoft.Extensions.Logging;

namespace Envelope.Logging;

public interface ILogMessage : Serializer.IDictionaryObject
{
	Guid IdLogMessage { get; set; }

	LogLevel LogLevel { get; set; }

	int IdLogLevel { get; }

	DateTimeOffset CreatedUtc { get; set; }

	ITraceInfo TraceInfo { get; set; }

	string? LogCode { get; set; }

	string? ClientMessage { get; set; }

	string? InternalMessage { get; set; }

#if NETSTANDARD2_0 || NETSTANDARD2_1
	[Newtonsoft.Json.JsonIgnore]
#elif NET6_0_OR_GREATER
	[System.Text.Json.Serialization.JsonIgnore]
#endif
	Exception? Exception { get; set; }

	string? StackTrace { get; set; }

	string? Detail { get; set; }

	string ClientMessageWithId { get; }

	string ClientMessageWithIdAndPropName { get; }

	string FullMessage { get; }

	string? CommandQueryName { get; set; }

	Guid? IdCommandQuery { get; set; }

	bool IsLogged { get; set; }

	decimal? MethodCallElapsedMilliseconds { get; set; }

	string? PropertyName { get; set; }

	object? ValidationFailure { get; set; }

	string? DisplayPropertyName { get; set; }

	bool IsValidationError { get; set; }

	Dictionary<string, string>? CustomData { get; set; }

	List<string>? Tags { get; set; }

	string ToString(bool withId, bool withPropertyName, bool withDetail);

	Exception ToException();
}