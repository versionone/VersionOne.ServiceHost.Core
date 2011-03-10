using System;
using System.Collections.Generic;
using System.Text;
using VersionOne.ServiceHost.Eventing;

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
    }
}