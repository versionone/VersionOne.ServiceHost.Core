using System;
using System.Collections.Generic;
using System.Text;
using VersionOne.ServiceHost.HostedServices;
using System.Xml;
using VersionOne.ServiceHost.Eventing;
using VersionOne.Profile;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;

using Log4netLogger = log4net.Repository.Hierarchy.Logger;
using log4net.Core;
using log4net.Layout;

namespace VersionOne.ServiceHost.Core.Logging 
{
    public class LogService : IHostedService 
    {
        private const string LogPattern = "[%5level] %date{dd-MM-yyyy HH:mm:ss} %message (%logger{1}:%line)%n";
        
        private LogMessage.SeverityType severity = LogMessage.SeverityType.Info;

        private IEventManager eventManager;
        private ILog logger;

        private ILog Logger 
        {
            get { return LogManager.GetLogger(typeof(LogService)); }
        }

        public void Initialize(XmlElement config, IEventManager eventManager, IProfile profile) 
        {
            this.eventManager = eventManager;
            
            eventManager.Subscribe(typeof(LogMessage), HandleLogMessage);
			eventManager.Subscribe(typeof(ServiceHostState), HandleServiceHostStateMessage);
        }

        private void HandleLogMessage(object pubobj)
		{
			Log((LogMessage) pubobj);
		}

		private void HandleServiceHostStateMessage(object pubobj)
		{
			ServiceHostState state = (ServiceHostState) pubobj;
			
            switch (state)
			{
				case ServiceHostState.Startup:
                    Logger.Info("[Startup]");
					break;
				case ServiceHostState.Shutdown:
					Logger.Info("[Shutdown]");
					break;
			}
		}

        private void Log(LogMessage message) 
        {
            // TODO impl
        }

        private void ConfigureLogger(LogMessage.SeverityType level) 
        {
            Log4netLogger root = ((Hierarchy)LogManager.GetRepository()).Root;

            IAppender consoleAppender = CreateConsoleAppender();
            IAppender fileAppender = CreateRollingFileAppender("ServiceHost.log", "20MB");

            root.AddAppender(consoleAppender);
            root.AddAppender(fileAppender);
            root.Repository.Configured = true;
        }

        private IAppender CreateConsoleAppender() 
        {
            ConsoleAppender appender = new ConsoleAppender();
            appender.Layout = new PatternLayout(LogPattern);
            appender.Name = "Console";
            appender.Threshold = TranslateLevel(severity);
            appender.ActivateOptions();

            return appender;
        }

        private IAppender CreateRollingFileAppender(string filename, string maxFileSize) 
        {
            RollingFileAppender appender = new RollingFileAppender();
            appender.Layout = new PatternLayout(LogPattern);
            appender.Name = "File";
            appender.Threshold = TranslateLevel(severity);
            appender.AppendToFile = true;
            appender.File = filename;
            appender.MaximumFileSize = maxFileSize;
            appender.ActivateOptions();

            return appender;
        }

        protected Level TranslateLevel(LogMessage.SeverityType severity) 
        {
            switch (severity) 
            {
                case LogMessage.SeverityType.Debug:
                    return Level.Debug;
                case LogMessage.SeverityType.Info:
                    return Level.Info;
                case LogMessage.SeverityType.Error:
                    return Level.Error;
                default:
                    return Level.Info;
            }
        }
    }
}