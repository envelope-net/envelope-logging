using Envelope.Data;
using Envelope.Logging.SerilogEx.Sink;
using Serilog.Configuration;
using Serilog.Events;

namespace Serilog;

public static class SerilogExtensions
{
	//public static LoggerConfiguration WithLogMessage(this LoggerEnrichmentConfiguration enrich)
	//{
	//	if (enrich == null)
	//		throw new ArgumentNullException(nameof(enrich));

	//	return enrich.With<LogMessageEnricher>();
	//}

	public static LoggerConfiguration EnvelopeBatchSink(
		this LoggerSinkConfiguration loggerConfiguration,
		Func<IEnumerable<LogEvent>, CancellationToken, Task<ulong>> writeBatchCallback,
		LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
		=> EnvelopeBatchSink(
			loggerConfiguration,
			writeBatchCallback,
			null,
			restrictedToMinimumLevel);

	public static LoggerConfiguration EnvelopeBatchSink(
		this LoggerSinkConfiguration loggerConfiguration,
		Func<IEnumerable<LogEvent>, CancellationToken, Task<ulong>> writeBatchCallback,
		BatchWriterOptions? options,
		LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
		=> EnvelopeBatchSink(
			loggerConfiguration,
			e => true,
			writeBatchCallback,
			options,
			restrictedToMinimumLevel);

	public static LoggerConfiguration EnvelopeBatchSink(
		this LoggerSinkConfiguration loggerConfiguration,
		Func<LogEvent, bool> includeCallBack,
		Func<IEnumerable<LogEvent>, CancellationToken, Task<ulong>> writeBatchCallback,
		BatchWriterOptions? options,
		LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
	{
		if (loggerConfiguration == null)
			throw new ArgumentNullException(nameof(loggerConfiguration));

		var sink = new EnvelopeBatchSink(
			includeCallBack,
			writeBatchCallback,
			options);

		return loggerConfiguration.Sink(sink, restrictedToMinimumLevel);
	}
}
