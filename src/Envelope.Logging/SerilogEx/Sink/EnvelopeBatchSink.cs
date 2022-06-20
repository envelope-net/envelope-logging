using Envelope.Data;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;

namespace Envelope.Logging.SerilogEx.Sink;

/*
 USAGE:
	Serilog.LoggerConfiguration
		.MinimumLevel.Verbose()
		//.Enrich.WithLogMessage()
		.WriteTo.EnvelopeBatchSink(events => Task.CompletedTask \/* TODO write to output  *\/, new EnvelopeBatchSinkOptions { EagerlyEmitFirstEvent = true })
		.WriteTo.Console())
 */

public class EnvelopeBatchSink : BatchWriter<LogEvent>, ILogEventSink, IDisposable
{
	public EnvelopeBatchSink(
		Func<LogEvent, bool> includeCallBack,
		Func<IEnumerable<LogEvent>, CancellationToken, Task<ulong>> writeBatchCallback,
		IBatchWriterOptions? options,
		Action<string, object?, object?, object?>? errorLogger = null)
		: base(includeCallBack, writeBatchCallback, options, errorLogger ?? SelfLog.WriteLine)
	{
	}

	public void Emit(LogEvent logEvent)
		=> Write(logEvent);
}
