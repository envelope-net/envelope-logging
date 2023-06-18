using Envelope.Logging;
using Envelope.Trace;
using System.Xml.Schema;

namespace Envelope.Extensions;

public static  class ValidationEventArgsExtensions
{
	public static IErrorMessage ToErrorMessage(this ValidationEventArgs arg, ITraceInfo traceInfo)
		=> LogMessage.CreateErrorMessage(traceInfo, x => x.ExceptionInfo(arg.Exception).Detail(arg.Message));

	public static ILogMessage ToLogMessage(this ValidationEventArgs arg, ITraceInfo traceInfo)
		=> arg.Severity == XmlSeverityType.Error
		? LogMessage.CreateErrorMessage(traceInfo, x => x.ExceptionInfo(arg.Exception).Detail(arg.Message))
		: LogMessage.CreateWarningMessage(traceInfo, x => x.ExceptionInfo(arg.Exception).Detail(arg.Message));
}
