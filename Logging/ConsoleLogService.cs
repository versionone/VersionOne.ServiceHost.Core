using System;
using System.Xml;
using VersionOne.Profile;
using VersionOne.ServiceHost.Eventing;
using VersionOne.ServiceHost.Core.Logging;

namespace VersionOne.ServiceHost.Logging
{
	public class ConsoleLogService : BaseLogService
	{
		private LogMessage.SeverityType _severity = LogMessage.SeverityType.Info;

		protected override void Log(LogMessage msg)
		{
			if (msg.Severity >= _severity)
			{
				OutputStringAndFlush(string.Format("[{0}] {1}", msg.Severity, msg.Message));
				Exception ex = msg.Exception;
				while (ex != null)
				{
					OutputStringAndFlush(string.Format("[{0}] Exception: {1}", msg.Severity, ex.Message));
					ex = ex.InnerException;
				}
			}
		}

		public override void Initialize(XmlElement config, IEventManager eventManager, IProfile profile)
		{
			base.Initialize(config, eventManager, profile);

			if (config["LogLevel"] != null && ! string.IsNullOrEmpty(config["LogLevel"].InnerText))
			{
				string logLevel = config["LogLevel"].InnerText;

				try
				{
					_severity = (LogMessage.SeverityType) Enum.Parse(typeof(LogMessage.SeverityType), logLevel, true);
				}
				catch (Exception)
				{
					OutputStringAndFlush( "Couldn't parse LogLevel '{0}'. Try Debug, Info, or Error.");
				}

			}
		}

		protected override void Startup()
		{
			OutputStringAndFlush("[Startup]");
		}		
		
		protected override void Shutdown()
		{
			OutputStringAndFlush("[Shutdown]");
		}

        private static void OutputStringAndFlush(string data) 
        {
            Console.WriteLine(data);
            Console.Out.Flush();
        }
	}
}