//using System;
//using System.Xml;
//using VersionOne.Profile;
//using VersionOne.ServiceHost.Eventing;
//using VersionOne.ServiceHost.Core.Logging;
//using log4net;
//using log4net.Appender;
//using log4net.Layout;
//using log4net.Repository.Hierarchy;
//using log4net.Core;

//namespace VersionOne.ServiceHost.Logging
//{
//    public class ConsoleLogService : BaseLogService
//    {
//        private LogMessage.SeverityType severity = LogMessage.SeverityType.Info;

//        public override void Initialize(XmlElement config, IEventManager eventManager, IProfile profile)
//        {
//            base.Initialize(config, eventManager, profile);

//            if (config["LogLevel"] != null && ! string.IsNullOrEmpty(config["LogLevel"].InnerText))
//            {
//                string logLevel = config["LogLevel"].InnerText;

//                bool severityParsed = false;

//                try
//                {
//                    severity = (LogMessage.SeverityType) Enum.Parse(typeof(LogMessage.SeverityType), logLevel, true);
//                    severityParsed = true;    
//                }
//                catch (Exception)
//                {
//                    // Do nothing, severity defaults to Info
//                }

//                ConfigureLogger(severity);

//                if(!severityParsed) 
//                {
//                    Logger.Info("Failed to parse Console logger severity, try Debug, Info or Error. Setting default value [Info].");
//                }
//            }
//        }

//        protected override void Startup()
//        {
//            Logger.Info("[Startup]");
//        }		
		
//        protected override void Shutdown()
//        {
//            Logger.Info("[Shutdown]");
//        }

//        private static void OutputStringAndFlush(string data) 
//        {
//            Console.WriteLine(data);
//            Console.Out.Flush();
//        }
//    }
//}