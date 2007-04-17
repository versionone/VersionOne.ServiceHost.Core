using System;

namespace VersionOne.ServiceHost.Logging
{
	public class ConsoleLogService : BaseLogService
	{
		protected override void Log(LogMessage msg)
		{
#if !DEBUG
			if (msg.Severity == LogMessage.SeverityType.Debug)
				return;
#endif
			Console.WriteLine(string.Format("[{0}] {1}", msg.Severity, msg.Message));
			if (msg.Exception != null)
				Console.WriteLine(string.Format("[{0}] Exception: {1}", msg.Severity, msg.Exception.Message));
		}

		protected override void Startup()
		{
			Console.WriteLine("[Startup]");
		}		
		
		protected override void Shutdown()
		{
			Console.WriteLine("[Shutdown]");
		}
	}
}