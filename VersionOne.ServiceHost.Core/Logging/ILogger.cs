using System;
using System.Xml;

namespace VersionOne.ServiceHost.Core.Logging 
{
    /// <summary>
    /// Allows classes that don't need to know about the EventManager to log errors and debug info in an abstract manner
    /// </summary>
    public interface ILogger 
    {
        void Log(string message);
        void Log(string message, Exception exception);
        void Log(LogMessage.SeverityType severity, string message);
        void Log(LogMessage.SeverityType severity, string message, Exception exception);

        void LogVersionOneConfiguration(LogMessage.SeverityType severity, XmlElement config);
        void LogVersionOneConnectionInformation(LogMessage.SeverityType severity, string metaVersion, string memberOid, string defaultMemberRole);
    }
}