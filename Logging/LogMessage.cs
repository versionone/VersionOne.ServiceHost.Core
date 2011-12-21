using System;

namespace VersionOne.ServiceHost.Core.Logging {
    public class LogMessage {
        public enum SeverityType {
            Debug,
            Info,
            Error
        }

        public readonly SeverityType Severity;
        public readonly string Message;
        public readonly Exception Exception;
        public readonly DateTime Stamp;

        public LogMessage(SeverityType severity, string message, Exception exception) {
            Severity = severity;
            Message = message;
            Exception = exception;
            Stamp = DateTime.Now;
        }
    }
}