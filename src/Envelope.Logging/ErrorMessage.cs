using Envelope.Trace;

namespace Envelope.Logging;

public class ErrorMessage : LogMessage, IErrorMessage
{
	internal ErrorMessage(ITraceInfo traceInfo)
		: base(traceInfo)
	{
	}
}
