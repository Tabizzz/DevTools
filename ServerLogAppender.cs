using log4net.Appender;
using log4net.Core;
using log4net.Layout;

namespace DevTools;

internal class ServerLogAppender : AppenderSkeleton
{

	public ServerLogAppender()
	{
		Layout = new PatternLayout
		{
			ConversionPattern = "[%d{HH:mm:ss.fff}] [%t/%level] [%logger]: %m"
		};
		Name = "ServerLogDevTool";
	}
	protected override void Append(LoggingEvent loggingEvent)
	{
		if (!ImGUI.ImGUI.CanGui)
			ServerAppLog.Logs.Add(RenderLoggingEvent(loggingEvent));
	}
}