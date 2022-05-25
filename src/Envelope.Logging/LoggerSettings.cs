using Serilog.Core;

namespace Envelope.Logging;

public static class LoggerSettings
{
	internal const string LogMessage = "Envelope_LogMessage";
	internal const string EnvironmentInfo = "Envelope_EnvironmentInfo";
	internal const string HardwareInfo = "Envelope_HardwareInfo";

	internal const string LogMessage_Template = "{@Envelope_LogMessage}";
	internal const string EnvironmentInfo_Template = "{@Envelope_EnvironmentInfo}";
	internal const string HardwareInfo_Template = "{@Envelope_HardwareInfo}";

	public static readonly LoggingLevelSwitch LevelSwitch = new(Serilog.Events.LogEventLevel.Information);
}
