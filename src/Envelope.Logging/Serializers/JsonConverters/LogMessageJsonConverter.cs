#if NET6_0_OR_GREATER
using Envelope.Enums;
using Envelope.Logging.Serializers.JsonConverters.Model;
using Envelope.Trace;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Envelope.Logging.Serializers.JsonConverters;

public class LogMessageJsonConverter : JsonConverter<ILogMessage>
{
	private static readonly Type _traceInfoType = typeof(ITraceInfo);
	private static readonly Type _dateTimeOffset = typeof(DateTimeOffset);

	public override void Write(Utf8JsonWriter writer, ILogMessage value, JsonSerializerOptions options)
	{
		throw new NotImplementedException("Read only converter");
	}

	public override ILogMessage? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.Null)
		{
			return null;
		}

		if (reader.TokenType != JsonTokenType.StartObject)
		{
			throw new JsonException();
		}
		else
		{
			var stringComparison = options.PropertyNameCaseInsensitive
				? StringComparison.OrdinalIgnoreCase
				: StringComparison.Ordinal;

			var logMessage = new DeserializedLogMessage
			{
			};

			while (reader.Read())
			{
				if (reader.TokenType == JsonTokenType.EndObject)
				{
					return logMessage;
				}

				if (reader.TokenType == JsonTokenType.PropertyName)
				{
					string? value;
					var propertyName = reader.GetString();
					reader.Read();
					switch (propertyName)
					{
						case var name when string.Equals(name, nameof(ILogMessage.IdLogMessage), stringComparison):
							value = reader.GetString();
							logMessage.IdLogMessage = Guid.TryParse(value, out var idLogMessage) ? idLogMessage : idLogMessage;
							break;
						case var name when string.Equals(name, nameof(ILogMessage.IdLogLevel), stringComparison):
							if (reader.TokenType != JsonTokenType.Null && reader.TryGetInt32(out var idLogLevel))
								logMessage.LogLevel = EnumHelper.ConvertIntToEnum<LogLevel>(idLogLevel);
							break;
						case var name when string.Equals(name, nameof(ILogMessage.CreatedUtc), stringComparison):
							logMessage.CreatedUtc = reader.TokenType == JsonTokenType.Null ? default : ((JsonConverter<DateTimeOffset>)options.GetConverter(_dateTimeOffset)).Read(ref reader, _dateTimeOffset, options);
							break;
						case var name when string.Equals(name, nameof(ILogMessage.TraceInfo), stringComparison):
							logMessage.TraceInfo = reader.TokenType == JsonTokenType.Null
								? null!
								: ((JsonConverter<ITraceInfo>)options.GetConverter(_traceInfoType)).Read(ref reader, _traceInfoType, options)!;
							break;
						case var name when string.Equals(name, nameof(ILogMessage.LogCode), stringComparison):
							logMessage.LogCode = reader.GetString()!;
							break;
						case var name when string.Equals(name, nameof(ILogMessage.ClientMessage), stringComparison):
							logMessage.ClientMessage = reader.GetString()!;
							break;
						case var name when string.Equals(name, nameof(ILogMessage.InternalMessage), stringComparison):
							logMessage.InternalMessage = reader.GetString()!;
							break;
						case var name when string.Equals(name, nameof(ILogMessage.StackTrace), stringComparison):
							logMessage.StackTrace = reader.GetString()!;
							break;
						case var name when string.Equals(name, nameof(ILogMessage.Detail), stringComparison):
							logMessage.Detail = reader.GetString()!;
							break;
						case var name when string.Equals(name, nameof(ILogMessage.CommandQueryName), stringComparison):
							logMessage.CommandQueryName = reader.GetString()!;
							break;
						case var name when string.Equals(name, nameof(ILogMessage.IdCommandQuery), stringComparison):
							value = reader.GetString();
							logMessage.IdCommandQuery = Guid.TryParse(value, out var idCommandQuery) ? idCommandQuery : idCommandQuery;
							break;
						case var name when string.Equals(name, nameof(ILogMessage.MethodCallElapsedMilliseconds), stringComparison):
							if (reader.TokenType != JsonTokenType.Null && reader.TryGetDecimal(out var methodCallElapsedMilliseconds))
								logMessage.MethodCallElapsedMilliseconds = methodCallElapsedMilliseconds;
							break;
						case var name when string.Equals(name, nameof(ILogMessage.PropertyName), stringComparison):
							logMessage.PropertyName = reader.GetString()!;
							break;
						case var name when string.Equals(name, nameof(ILogMessage.DisplayPropertyName), stringComparison):
							logMessage.DisplayPropertyName = reader.GetString()!;
							break;
						case var name when string.Equals(name, nameof(ILogMessage.IsValidationError), stringComparison):
							logMessage.IsValidationError = reader.TokenType != JsonTokenType.Null && reader.GetBoolean();
							break;
					}
				}
			}

			return default;
		}
	}
}
#endif
