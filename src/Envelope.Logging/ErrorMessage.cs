using Envelope.Trace;

namespace Envelope.Logging;

public class ErrorMessage<TIdentity> : LogMessage<TIdentity>, IErrorMessage<TIdentity>
	where TIdentity : struct
{
	internal ErrorMessage(ITraceInfo<TIdentity> traceInfo)
		: base(traceInfo)
	{
	}
}
