﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Envelope.Logging.Extensions;

public static class ServiceCollectionExtensions
{
#if NET6_0_OR_GREATER
	public static IServiceCollection ConfigureJsonPolymorphicConverters(this IServiceCollection services)
	{
		services.AddTransient<System.Text.Json.Serialization.JsonConverter, Envelope.Serializer.JsonPolymorphicConverterFactory>();
		return services;
	}
#endif

	public static IServiceCollection AddEnvelopeLogger(
		this IServiceCollection services,
		Action<LoggerConfiguration>? configurator,
		Action<LogWriterConfiguration>? logWriterConfigurator = null,
		Action<ILoggingBuilder>? configureLoggingBuilder = null)
		=> AddEnvelopeLogger(services, LogLevel.None, configurator, logWriterConfigurator, configureLoggingBuilder);

	public static IServiceCollection AddEnvelopeLogger(
		this IServiceCollection services,
		LogLevel logEventMinimumLevel,
		Action<LoggerConfiguration>? configurator,
		Action<LogWriterConfiguration>? logWriterConfigurator = null,
		Action<ILoggingBuilder>? configureLoggingBuilder = null)
	{
		switch (logEventMinimumLevel)
		{
			case LogLevel.Trace:
				LoggerSettings.LevelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Verbose;
				break;
			case LogLevel.Debug:
				LoggerSettings.LevelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Debug;
				break;
			case LogLevel.Information:
				LoggerSettings.LevelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Information;
				break;
			case LogLevel.Warning:
				LoggerSettings.LevelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Warning;
				break;
			case LogLevel.Error:
				LoggerSettings.LevelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Error;
				break;
			case LogLevel.Critical:
				LoggerSettings.LevelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Fatal;
				break;
			case LogLevel.None:
				LoggerSettings.LevelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Information;
				break;
			default:
				break;
		}

		var loggerConfiguration =
			new LoggerConfiguration()
				.MinimumLevel.ControlledBy(LoggerSettings.LevelSwitch)
				.Enrich.FromLogContext();

		configurator?.Invoke(loggerConfiguration);

		Log.Logger = loggerConfiguration.CreateLogger();
		Logger.SerilogLogger = Log.Logger;

		if (configureLoggingBuilder == null)
			configureLoggingBuilder = x => x.AddSerilog();

		Logger.MicrosoftLoggingBuilder = configureLoggingBuilder;

		services.AddLogging(configureLoggingBuilder);

		if (logWriterConfigurator != null)
		{
			var logWriterConfig = new LogWriterConfiguration();
			logWriterConfigurator.Invoke(logWriterConfig);
			var writer = logWriterConfig.CreateLogWriter();
			if (writer != null)
				LogWriter.Instance = writer;
		}

		return services;
	}
}
