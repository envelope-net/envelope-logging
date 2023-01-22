namespace Envelope.Logging;

public class ErrorMessageDto : LogMessageDto, IErrorMessage
{
	internal ErrorMessageDto()
		: base()
	{
	}

	ErrorMessageDto IErrorMessage.ToDto(params string[] ignoredPropterties)
	{
		throw new NotImplementedException();
	}

	ErrorMessageDto IErrorMessage.ToClientDto()
	{
		throw new NotImplementedException();
	}
	public ErrorMessage ToErrorMessage(params string[] ignoredPropterties)
	{
		ignoredPropterties ??= Array.Empty<string>();
		var errorMessage = new ErrorMessage(TraceInfo);

		if (!ignoredPropterties.Contains(nameof(IdLogMessage)))
			errorMessage.IdLogMessage = IdLogMessage;

		if (!ignoredPropterties.Contains(nameof(LogLevel)))
			errorMessage.LogLevel = LogLevel;

		if (!ignoredPropterties.Contains(nameof(CreatedUtc)))
			errorMessage.CreatedUtc = CreatedUtc;

		if (!ignoredPropterties.Contains(nameof(IsLogged)))
			errorMessage.IsLogged = IsLogged;

		if (!ignoredPropterties.Contains(nameof(IsValidationError)))
			errorMessage.IsValidationError = IsValidationError;

		if (!ignoredPropterties.Contains(nameof(TraceInfo)))
			errorMessage.TraceInfo = TraceInfo;

		if (!ignoredPropterties.Contains(nameof(LogCode)))
			errorMessage.LogCode = LogCode;

		if (!ignoredPropterties.Contains(nameof(ClientMessage)))
			errorMessage.ClientMessage = ClientMessage;

		if (!ignoredPropterties.Contains(nameof(InternalMessage)))
			errorMessage.InternalMessage = InternalMessage;

		if (!ignoredPropterties.Contains(nameof(Exception)))
			errorMessage.Exception = Exception;

		if (!ignoredPropterties.Contains(nameof(StackTrace)))
			errorMessage.StackTrace = StackTrace;

		if (!ignoredPropterties.Contains(nameof(Detail)))
			errorMessage.Detail = Detail;

		if (!ignoredPropterties.Contains(nameof(CommandQueryName)))
			errorMessage.CommandQueryName = CommandQueryName;

		if (!ignoredPropterties.Contains(nameof(IdCommandQuery)))
			errorMessage.IdCommandQuery = IdCommandQuery;

		if (!ignoredPropterties.Contains(nameof(MethodCallElapsedMilliseconds)))
			errorMessage.MethodCallElapsedMilliseconds = MethodCallElapsedMilliseconds;

		if (!ignoredPropterties.Contains(nameof(PropertyName)))
			errorMessage.PropertyName = PropertyName;

		if (!ignoredPropterties.Contains(nameof(ValidationFailure)))
			errorMessage.ValidationFailure = ValidationFailure;

		if (!ignoredPropterties.Contains(nameof(DisplayPropertyName)))
			errorMessage.DisplayPropertyName = DisplayPropertyName;

		if (!ignoredPropterties.Contains(nameof(Tags)))
			errorMessage.Tags = Tags;

		return errorMessage;
	}
}
