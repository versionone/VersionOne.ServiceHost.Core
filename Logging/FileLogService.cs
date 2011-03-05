//using System;
//using System.IO;
//using System.Reflection;
//using System.Xml;
//using VersionOne.Profile;
//using VersionOne.ServiceHost.Eventing;
//using VersionOne.ServiceHost.Core.Logging;
//using log4net.Appender;
//using log4net.Repository.Hierarchy;

//namespace VersionOne.ServiceHost.Logging
//{
//    public class FileLogService : BaseLogService
//    {
//        private const string _majorsep = "================================================================";
//        private const string _minorsep = "----------------------------------------------------------------";
//        private string filename;
//        private StreamWriter _writer = null;

//        private LogMessage.SeverityType severity = LogMessage.SeverityType.Info;
		
//        protected override void Log(LogMessage message)
//        {
//#if !DEBUG
//            if (message.Severity == LogMessage.SeverityType.Debug)
//                return;
//#endif
//            if (_writer != null)
//            {
//                _writer.WriteLine(string.Format("[{0}] {2} {1}",message.Severity, message.Message, message.Stamp));
//                string prefix = string.Format("[Exception] {0} ", message.Stamp);
//                Exception ex = message.Exception;
//                while (ex != null)
//                {
//                    _writer.WriteLine(prefix + ex.ToString());
//                    string extracontent = AdditionalExceptionContent(ex);
//                    if (!string.IsNullOrEmpty(extracontent))
//                    {
//                        _writer.WriteLine(_minorsep);
//                        _writer.WriteLine(extracontent);
//                        _writer.WriteLine(_minorsep);
//                    }
//                    _writer.WriteLine(_minorsep);

//                    ex = ex.InnerException;
//                }
//            }
//        }

//        public override void Initialize(XmlElement config, IEventManager eventManager, IProfile profile)
//        {
//            filename = config["LogFile"].InnerText;
			
//            base.Initialize(config,eventManager,profile);

//            string folder = Path.GetDirectoryName(filename);
            
//            if (!Directory.Exists(folder)) 
//            {
//                Directory.CreateDirectory(folder);
//            }

//            ConfigureLogger(severity);
			
//            Logger.InfoFormat("[Startup] Log opened {0}", DateTime.Now);
//            Logger.InfoFormat("[Startup] By {0}", Assembly.GetEntryAssembly().Location);			
//        }

//        private void ConfigureLogger(LogMessage.SeverityType level) 
//        {
//            RollingFileAppender appender = new RollingFileAppender();
//            appender.Layout = new PatternLayout(LogPattern);
//            appender.Name = "File";
//            appender.Threshold = TranslateLevel(severity);
//            appender.AppendToFile = true;
//            appender.File = filename;
//            appender.MaximumFileSize = "10MB";
//            appender.ActivateOptions();

//            Logger root = ((Hierarchy)LogManager.GetRepository(RepositoryName)).Root;
//            root.AddAppender(appender);
//            root.Repository.Configured = true;
//        }

//        protected override void Shutdown()
//        {
//            Logger.InfoFormat("[Shutdown] Log closed {0}", DateTime.Now);
//            Logger.Info(_majorsep);
			
//            base.Shutdown();
//        }
//    }
//}