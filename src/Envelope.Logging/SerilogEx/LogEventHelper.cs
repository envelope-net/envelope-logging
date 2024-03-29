﻿using Envelope.Infrastructure;
using Serilog.Events;
using System.Diagnostics;

namespace Envelope.Logging.SerilogEx;

public static class LogEventHelper
{
	public const string IS_DB_LOG = "IsDbLog";
	public const string SCOPE = "Scope";
	private static readonly Lazy<ScalarValue> _methodCallId = new (() => new(nameof(ILogMessage.TraceInfo.TraceFrame.MethodCallId)));
	private static readonly Lazy<ScalarValue> _correlationId = new(() => new(nameof(ILogMessage.TraceInfo.CorrelationId)));
	private static readonly Lazy<ScalarValue> _isDbLog = new(() => new(IS_DB_LOG));

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static bool IsLogType(string logType, LogEvent logEvent)
	=> logEvent != null
		&& logEvent.Properties.TryGetValue(logType, out LogEventPropertyValue? value)
		&& value is DictionaryValue;

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static IDictionary<string, object?>? ConvertToDictionary(string logType, LogEvent logEvent)
	{
		if (logEvent == null || logEvent.Properties == null)
			return null;

		IDictionary<string, object?>? result = null;

		if (logEvent.Properties.TryGetValue(logType, out LogEventPropertyValue? value))
		{
			if (value is DictionaryValue dict && dict.Elements != null)
				result = dict.Elements.ToDictionary(x => (string)x.Key.Value, x => (x.Value as ScalarValue)?.Value);
		}

		if (result == null)
			return null;

		if (logEvent.Properties.TryGetValue(Serilog.Core.Constants.SourceContextPropertyName, out LogEventPropertyValue? sourceContextValue))
		{
			if (sourceContextValue is ScalarValue scalarValue)
			{
				if (scalarValue.Value is string sourceContext)
				{
					result.Add(Serilog.Core.Constants.SourceContextPropertyName, sourceContext);
				}
			}
		}

		var list = new List<string>();
		if (logEvent.Properties.TryGetValue(SCOPE, out LogEventPropertyValue? scopeValue))
		{
			if (scopeValue is SequenceValue sequenceValue)
			{
				var elements = sequenceValue.Elements.Reverse();
				foreach (var element in elements)
				{
					if (element is DictionaryValue dict && dict.Elements != null)
					{
						if (dict.Elements != null)
						{
							list.AddRange(dict.Elements.Where(x => !x.Key.Equals(_methodCallId) && !x.Key.Equals(_correlationId) && !x.Key.Equals(_isDbLog)).Select(x => x.ToString()));
						}
					}
					else
					{
						list.Add(scopeValue.ToString());
					}
				}
			}
			else
			{
				list.Add(scopeValue.ToString());
			}
		}

		return result;
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static bool IsLogMessage(LogEvent logEvent)
		=> IsLogType(LoggerSettings.LogMessage, logEvent);

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static IDictionary<string, object?>? ConvertLogMessageToDictionary(LogEvent logEvent)
		=> ConvertToDictionary(LoggerSettings.LogMessage, logEvent);

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static bool IsEnvironmentInfo(LogEvent logEvent)
		=> IsLogType(LoggerSettings.EnvironmentInfo, logEvent);

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static IDictionary<string, object?>? ConvertEnvironmentInfoToDictionary(LogEvent logEvent)
		=> ConvertToDictionary(LoggerSettings.EnvironmentInfo, logEvent);

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static bool IsHardwareInfo(LogEvent logEvent)
		=> IsLogType(LoggerSettings.HardwareInfo, logEvent);

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static IDictionary<string, object?>? ConvertHardwareInfoToDictionary(LogEvent logEvent)
		=> ConvertToDictionary(LoggerSettings.HardwareInfo, logEvent);

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static IDictionary<string, object?>? ConvertLogToDictionary(LogEvent logEvent)
	{
		if (logEvent == null)
			return null;

		var result = new Dictionary<string, object?>
		{
			{ nameof(logEvent.Level), logEvent.Level },
			{ nameof(logEvent.Timestamp), logEvent.Timestamp.UtcDateTime },
			{ nameof(logEvent.MessageTemplate), logEvent.RenderMessage(null) },
			{ nameof(logEvent.Properties), logEvent.Properties },
			{ nameof(logEvent.Exception), logEvent.Exception },
			{ nameof(ILogMessage.TraceInfo.RuntimeUniqueKey), EnvironmentInfo.RUNTIME_UNIQUE_KEY },
			{ IS_DB_LOG, false }
		};

		var isDBLogIsSet = false;
		if (logEvent.Properties.TryGetValue(IS_DB_LOG, out LogEventPropertyValue? isDBLogValue))
		{
			if (isDBLogValue is ScalarValue scalarValue)
			{
				if (scalarValue.Value is bool isDBLog && isDBLog)
				{
					result[IS_DB_LOG] = isDBLog;
					isDBLogIsSet = true;
				}
			}
		}
		if (logEvent.Properties.TryGetValue(SCOPE, out LogEventPropertyValue? scopeValue))
		{
			if (scopeValue is SequenceValue sequenceValue)
			{
				//var last = sequenceValue.Elements.LastOrDefault();
				//if (last is DictionaryValue lastDict && lastDict.Elements != null)
				//{
				//	if (lastDict.Elements.TryGetValue(new ScalarValue(IS_DB_LOG), out LogEventPropertyValue? scopeIsDBLogValue))
				//	{
				//		if (scopeIsDBLogValue is ScalarValue scalarValue)
				//		{
				//			if (scalarValue.Value is bool isDBLog && isDBLog)
				//			{
				//				result[IS_DB_LOG] = isDBLog;
				//			}
				//		}
				//	}
				//}

				var elements = sequenceValue.Elements.Reverse();
				var methodCallIdIsSet = false;
				var correlationIdIsSet = false;
				foreach (var element in elements)
				{
					if (element is DictionaryValue dict && dict.Elements != null)
					{
						if (!methodCallIdIsSet && dict.Elements.TryGetValue(_methodCallId.Value, out LogEventPropertyValue? scopeMethodCallIdValue))
						{
							if (scopeMethodCallIdValue is ScalarValue scalarValue)
							{
								if (scalarValue.Value is Guid methodCallId)
								{
									result[nameof(ILogMessage.TraceInfo.TraceFrame.MethodCallId)] = methodCallId;
									methodCallIdIsSet = true;
								}
							}
						}

						if (!correlationIdIsSet && dict.Elements.TryGetValue(_correlationId.Value, out LogEventPropertyValue? scopeCorrelationIdValue))
						{
							if (scopeCorrelationIdValue is ScalarValue scalarValue)
							{
								if (scalarValue.Value is Guid correlationId)
								{
									result[nameof(ILogMessage.TraceInfo.CorrelationId)] = correlationId;
									correlationIdIsSet = true;
								}
							}
						}

						if (!isDBLogIsSet && dict.Elements.TryGetValue(_isDbLog.Value, out LogEventPropertyValue? scopeIsDBLogValue))
						{
							if (scopeIsDBLogValue is ScalarValue scalarValue)
							{
								if (scalarValue.Value is bool isDBLog && isDBLog)
								{
									result[IS_DB_LOG] = isDBLog;
									isDBLogIsSet = true;
								}
							}
						}
					}

					if (methodCallIdIsSet && correlationIdIsSet && isDBLogIsSet)
						break;
				}
			}
		}

		if (logEvent.Properties.TryGetValue(nameof(ILogMessage.TraceInfo.TraceFrame.MethodCallId), out LogEventPropertyValue? methodCallIdValue))
		{
			if (methodCallIdValue is ScalarValue scalarValue)
			{
				if (scalarValue.Value is Guid methodCallId)
				{
					result[nameof(ILogMessage.TraceInfo.TraceFrame.MethodCallId)] = methodCallId;
				}
			}
		}

		if (logEvent.Properties.TryGetValue(nameof(ILogMessage.TraceInfo.TraceFrame.MethodCallId), out LogEventPropertyValue? correlationIdValue))
		{
			if (correlationIdValue is ScalarValue scalarValue)
			{
				if (scalarValue.Value is Guid correlationId)
				{
					result[nameof(ILogMessage.TraceInfo.CorrelationId)] = correlationId;
				}
			}
		}

		if (logEvent.Properties.TryGetValue(Serilog.Core.Constants.SourceContextPropertyName, out LogEventPropertyValue? sourceContextValue))
		{
			if (sourceContextValue is ScalarValue scalarValue)
			{
				if (scalarValue.Value is string sourceContext)
				{
					result.Add(Serilog.Core.Constants.SourceContextPropertyName, sourceContext);
				}
			}
		}

		return result;
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static IDictionary<string, object?>? ConvertLogToDictionary<TIdentity>(LogEvent logEvent)
		where TIdentity : struct
	{
		if (logEvent == null)
			return null;

		var result = new Dictionary<string, object?>
		{
			{ nameof(logEvent.Level), logEvent.Level },
			{ nameof(logEvent.Timestamp), logEvent.Timestamp.UtcDateTime },
			{ nameof(logEvent.MessageTemplate), logEvent.RenderMessage(null) },
			{ nameof(logEvent.Properties), logEvent.Properties },
			{ nameof(logEvent.Exception), logEvent.Exception },
			{ nameof(ILogMessage<TIdentity>.TraceInfo.RuntimeUniqueKey), EnvironmentInfo.RUNTIME_UNIQUE_KEY },
			{ IS_DB_LOG, false }
		};

		var isDBLogIsSet = false;
		if (logEvent.Properties.TryGetValue(IS_DB_LOG, out LogEventPropertyValue? isDBLogValue))
		{
			if (isDBLogValue is ScalarValue scalarValue)
			{
				if (scalarValue.Value is bool isDBLog && isDBLog)
				{
					result[IS_DB_LOG] = isDBLog;
					isDBLogIsSet = true;
				}
			}
		}
		if (logEvent.Properties.TryGetValue(SCOPE, out LogEventPropertyValue? scopeValue))
		{
			if (scopeValue is SequenceValue sequenceValue)
			{
				//var last = sequenceValue.Elements.LastOrDefault();
				//if (last is DictionaryValue lastDict && lastDict.Elements != null)
				//{
				//	if (lastDict.Elements.TryGetValue(new ScalarValue(IS_DB_LOG), out LogEventPropertyValue? scopeIsDBLogValue))
				//	{
				//		if (scopeIsDBLogValue is ScalarValue scalarValue)
				//		{
				//			if (scalarValue.Value is bool isDBLog && isDBLog)
				//			{
				//				result[IS_DB_LOG] = isDBLog;
				//			}
				//		}
				//	}
				//}

				var elements = sequenceValue.Elements.Reverse();
				var methodCallIdIsSet = false;
				var correlationIdIsSet = false;
				foreach (var element in elements)
				{
					if (element is DictionaryValue dict && dict.Elements != null)
					{
						if (!methodCallIdIsSet && dict.Elements.TryGetValue(_methodCallId.Value, out LogEventPropertyValue? scopeMethodCallIdValue))
						{
							if (scopeMethodCallIdValue is ScalarValue scalarValue)
							{
								if (scalarValue.Value is Guid methodCallId)
								{
									result[nameof(ILogMessage<TIdentity>.TraceInfo.TraceFrame.MethodCallId)] = methodCallId;
									methodCallIdIsSet = true;
								}
							}
						}

						if (!correlationIdIsSet && dict.Elements.TryGetValue(_correlationId.Value, out LogEventPropertyValue? scopeCorrelationIdValue))
						{
							if (scopeCorrelationIdValue is ScalarValue scalarValue)
							{
								if (scalarValue.Value is Guid correlationId)
								{
									result[nameof(ILogMessage<TIdentity>.TraceInfo.CorrelationId)] = correlationId;
									correlationIdIsSet = true;
								}
							}
						}

						if (!isDBLogIsSet && dict.Elements.TryGetValue(_isDbLog.Value, out LogEventPropertyValue? scopeIsDBLogValue))
						{
							if (scopeIsDBLogValue is ScalarValue scalarValue)
							{
								if (scalarValue.Value is bool isDBLog && isDBLog)
								{
									result[IS_DB_LOG] = isDBLog;
									isDBLogIsSet = true;
								}
							}
						}
					}

					if (methodCallIdIsSet && correlationIdIsSet && isDBLogIsSet)
						break;
				}
			}
		}

		if (logEvent.Properties.TryGetValue(nameof(ILogMessage<TIdentity>.TraceInfo.TraceFrame.MethodCallId), out LogEventPropertyValue? methodCallIdValue))
		{
			if (methodCallIdValue is ScalarValue scalarValue)
			{
				if (scalarValue.Value is Guid methodCallId)
				{
					result[nameof(ILogMessage<TIdentity>.TraceInfo.TraceFrame.MethodCallId)] = methodCallId;
				}
			}
		}

		if (logEvent.Properties.TryGetValue(nameof(ILogMessage<TIdentity>.TraceInfo.TraceFrame.MethodCallId), out LogEventPropertyValue? correlationIdValue))
		{
			if (correlationIdValue is ScalarValue scalarValue)
			{
				if (scalarValue.Value is Guid correlationId)
				{
					result[nameof(ILogMessage<TIdentity>.TraceInfo.CorrelationId)] = correlationId;
				}
			}
		}

		if (logEvent.Properties.TryGetValue(Serilog.Core.Constants.SourceContextPropertyName, out LogEventPropertyValue? sourceContextValue))
		{
			if (sourceContextValue is ScalarValue scalarValue)
			{
				if (scalarValue.Value is string sourceContext)
				{
					result.Add(Serilog.Core.Constants.SourceContextPropertyName, sourceContext);
				}
			}
		}

		return result;
	}
}
