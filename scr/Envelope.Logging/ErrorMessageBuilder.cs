using Microsoft.Extensions.Logging;
using Envelope.Trace;

namespace Envelope.Logging;

public interface IErrorMessageBuilder<TBuilder, TObject, TIdentity> : ILogMessageBuilder<TBuilder, TObject, TIdentity>
	where TBuilder : IErrorMessageBuilder<TBuilder, TObject, TIdentity>
	where TObject : IErrorMessage<TIdentity>
	where TIdentity : struct
{
}

public abstract class ErrorMessageBuilderBase<TBuilder, TObject, TIdentity> : LogMessageBuilderBase<TBuilder, TObject, TIdentity>, IErrorMessageBuilder<TBuilder, TObject, TIdentity>
	where TBuilder : ErrorMessageBuilderBase<TBuilder, TObject, TIdentity>
	where TObject : IErrorMessage<TIdentity>
	where TIdentity : struct
{
	protected ErrorMessageBuilderBase(TObject errorMessage)
		:base(errorMessage)
	{
	}

	public override TBuilder LogLevel(LogLevel logLevel, bool force = false)
	{
		if (logLevel != Microsoft.Extensions.Logging.LogLevel.Error
			&& logLevel != Microsoft.Extensions.Logging.LogLevel.Critical)
			throw new InvalidOperationException($"Invalid {nameof(logLevel)} = {logLevel}");

		if (force || _logMessage.LogLevel == default)
			_logMessage.LogLevel = logLevel;

		return _builder;
	}
}

public class ErrorMessageBuilder<TIdentity> : ErrorMessageBuilderBase<ErrorMessageBuilder<TIdentity>, IErrorMessage<TIdentity>, TIdentity>
	where TIdentity : struct
{
	public ErrorMessageBuilder(MethodLogScope<TIdentity> methodLogScope)
		: this(methodLogScope?.TraceInfo!)
	{
	}

	public ErrorMessageBuilder(ITraceInfo<TIdentity> traceInfo)
		: this(new ErrorMessage<TIdentity>(traceInfo))
	{
	}

	public ErrorMessageBuilder(ErrorMessage<TIdentity> errorMessage)
		: base(errorMessage)
	{
	}

	public static implicit operator ErrorMessage<TIdentity>?(ErrorMessageBuilder<TIdentity> builder)
	{
		if (builder == null)
			return null;

		return builder._logMessage as ErrorMessage<TIdentity>;
	}

	public static implicit operator ErrorMessageBuilder<TIdentity>?(ErrorMessage<TIdentity> errorMessage)
	{
		if (errorMessage == null)
			return null;

		return new ErrorMessageBuilder<TIdentity>(errorMessage);
	}
}


public class ErrorMessageBuilder : ErrorMessageBuilder<Guid>
{
	public ErrorMessageBuilder(MethodLogScope<Guid> methodLogScope)
		: base(methodLogScope)
	{
	}

	public ErrorMessageBuilder(ITraceInfo<Guid> traceInfo)
		: base(traceInfo)
	{
	}

	public ErrorMessageBuilder(ErrorMessage<Guid> errorMessage)
		: base(errorMessage)
	{
	}
}