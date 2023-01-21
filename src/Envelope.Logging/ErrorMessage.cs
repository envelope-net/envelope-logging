using Envelope.Trace;

namespace Envelope.Logging;

public class ErrorMessage : LogMessage, IErrorMessage
{
	internal ErrorMessage(ITraceInfo traceInfo)
		: base(traceInfo)
	{
	}

	public new ErrorMessageDto ToDto(params string[] ignoredPropterties)
	{
		ignoredPropterties ??= Array.Empty<string>();
		var dto = new ErrorMessageDto();

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

	public new ErrorMessageDto ToClientDto()
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
