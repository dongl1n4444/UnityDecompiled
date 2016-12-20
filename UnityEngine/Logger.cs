namespace UnityEngine
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Initializes a new instance of the Logger.</para>
    /// </summary>
    public class Logger : ILogger, ILogHandler
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private LogType <filterLogType>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private bool <logEnabled>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private ILogHandler <logHandler>k__BackingField;
        private const string kNoTagFormat = "{0}";
        private const string kTagFormat = "{0}: {1}";

        private Logger()
        {
        }

        /// <summary>
        /// <para>Create a custom Logger.</para>
        /// </summary>
        /// <param name="logHandler">Pass in default log handler or custom log handler.</param>
        public Logger(ILogHandler logHandler)
        {
            this.logHandler = logHandler;
            this.logEnabled = true;
            this.filterLogType = LogType.Log;
        }

        private static string GetString(object message)
        {
            return ((message == null) ? "Null" : message.ToString());
        }

        /// <summary>
        /// <para>Check logging is enabled based on the LogType.</para>
        /// </summary>
        /// <param name="logType">The type of the log message.</param>
        /// <returns>
        /// <para>Retrun true in case logs of LogType will be logged otherwise returns false.</para>
        /// </returns>
        public bool IsLogTypeAllowed(LogType logType)
        {
            return (this.logEnabled && ((logType <= this.filterLogType) || (logType == LogType.Exception)));
        }

        /// <summary>
        /// <para>Logs message to the Unity Console using default logger.</para>
        /// </summary>
        /// <param name="logType">The type of the log message.</param>
        /// <param name="tag">Used to identify the source of a log message. It usually identifies the class where the log call occurs.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public void Log(object message)
        {
            if (this.IsLogTypeAllowed(LogType.Log))
            {
                object[] args = new object[] { GetString(message) };
                this.logHandler.LogFormat(LogType.Log, null, "{0}", args);
            }
        }

        /// <summary>
        /// <para>Logs message to the Unity Console using default logger.</para>
        /// </summary>
        /// <param name="logType">The type of the log message.</param>
        /// <param name="tag">Used to identify the source of a log message. It usually identifies the class where the log call occurs.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public void Log(string tag, object message)
        {
            if (this.IsLogTypeAllowed(LogType.Log))
            {
                object[] args = new object[] { tag, GetString(message) };
                this.logHandler.LogFormat(LogType.Log, null, "{0}: {1}", args);
            }
        }

        /// <summary>
        /// <para>Logs message to the Unity Console using default logger.</para>
        /// </summary>
        /// <param name="logType">The type of the log message.</param>
        /// <param name="tag">Used to identify the source of a log message. It usually identifies the class where the log call occurs.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public void Log(LogType logType, object message)
        {
            if (this.IsLogTypeAllowed(logType))
            {
                object[] args = new object[] { GetString(message) };
                this.logHandler.LogFormat(logType, null, "{0}", args);
            }
        }

        /// <summary>
        /// <para>Logs message to the Unity Console using default logger.</para>
        /// </summary>
        /// <param name="logType">The type of the log message.</param>
        /// <param name="tag">Used to identify the source of a log message. It usually identifies the class where the log call occurs.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public void Log(string tag, object message, UnityEngine.Object context)
        {
            if (this.IsLogTypeAllowed(LogType.Log))
            {
                object[] args = new object[] { tag, GetString(message) };
                this.logHandler.LogFormat(LogType.Log, context, "{0}: {1}", args);
            }
        }

        /// <summary>
        /// <para>Logs message to the Unity Console using default logger.</para>
        /// </summary>
        /// <param name="logType">The type of the log message.</param>
        /// <param name="tag">Used to identify the source of a log message. It usually identifies the class where the log call occurs.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public void Log(LogType logType, object message, UnityEngine.Object context)
        {
            if (this.IsLogTypeAllowed(logType))
            {
                object[] args = new object[] { GetString(message) };
                this.logHandler.LogFormat(logType, context, "{0}", args);
            }
        }

        /// <summary>
        /// <para>Logs message to the Unity Console using default logger.</para>
        /// </summary>
        /// <param name="logType">The type of the log message.</param>
        /// <param name="tag">Used to identify the source of a log message. It usually identifies the class where the log call occurs.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public void Log(LogType logType, string tag, object message)
        {
            if (this.IsLogTypeAllowed(logType))
            {
                object[] args = new object[] { tag, GetString(message) };
                this.logHandler.LogFormat(logType, null, "{0}: {1}", args);
            }
        }

        /// <summary>
        /// <para>Logs message to the Unity Console using default logger.</para>
        /// </summary>
        /// <param name="logType">The type of the log message.</param>
        /// <param name="tag">Used to identify the source of a log message. It usually identifies the class where the log call occurs.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public void Log(LogType logType, string tag, object message, UnityEngine.Object context)
        {
            if (this.IsLogTypeAllowed(logType))
            {
                object[] args = new object[] { tag, GetString(message) };
                this.logHandler.LogFormat(logType, context, "{0}: {1}", args);
            }
        }

        /// <summary>
        /// <para>A variant of Logger.Log that logs an error message.</para>
        /// </summary>
        /// <param name="tag">Used to identify the source of a log message. It usually identifies the class where the log call occurs.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public void LogError(string tag, object message)
        {
            if (this.IsLogTypeAllowed(LogType.Error))
            {
                object[] args = new object[] { tag, GetString(message) };
                this.logHandler.LogFormat(LogType.Error, null, "{0}: {1}", args);
            }
        }

        /// <summary>
        /// <para>A variant of Logger.Log that logs an error message.</para>
        /// </summary>
        /// <param name="tag">Used to identify the source of a log message. It usually identifies the class where the log call occurs.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public void LogError(string tag, object message, UnityEngine.Object context)
        {
            if (this.IsLogTypeAllowed(LogType.Error))
            {
                object[] args = new object[] { tag, GetString(message) };
                this.logHandler.LogFormat(LogType.Error, context, "{0}: {1}", args);
            }
        }

        /// <summary>
        /// <para>A variant of Logger.Log that logs an exception message.</para>
        /// </summary>
        /// <param name="exception">Runtime Exception.</param>
        /// <param name="context">Object to which the message applies.</param>
        public void LogException(Exception exception)
        {
            if (this.logEnabled)
            {
                this.logHandler.LogException(exception, null);
            }
        }

        /// <summary>
        /// <para>A variant of Logger.Log that logs an exception message.</para>
        /// </summary>
        /// <param name="exception">Runtime Exception.</param>
        /// <param name="context">Object to which the message applies.</param>
        public void LogException(Exception exception, UnityEngine.Object context)
        {
            if (this.logEnabled)
            {
                this.logHandler.LogException(exception, context);
            }
        }

        /// <summary>
        /// <para>Logs a formatted message.</para>
        /// </summary>
        /// <param name="logType">The type of the log message.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        public void LogFormat(LogType logType, string format, params object[] args)
        {
            if (this.IsLogTypeAllowed(logType))
            {
                this.logHandler.LogFormat(logType, null, format, args);
            }
        }

        /// <summary>
        /// <para>Logs a formatted message.</para>
        /// </summary>
        /// <param name="logType">The type of the log message.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            if (this.IsLogTypeAllowed(logType))
            {
                this.logHandler.LogFormat(logType, context, format, args);
            }
        }

        /// <summary>
        /// <para>A variant of Logger.Log that logs an warning message.</para>
        /// </summary>
        /// <param name="tag">Used to identify the source of a log message. It usually identifies the class where the log call occurs.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public void LogWarning(string tag, object message)
        {
            if (this.IsLogTypeAllowed(LogType.Warning))
            {
                object[] args = new object[] { tag, GetString(message) };
                this.logHandler.LogFormat(LogType.Warning, null, "{0}: {1}", args);
            }
        }

        /// <summary>
        /// <para>A variant of Logger.Log that logs an warning message.</para>
        /// </summary>
        /// <param name="tag">Used to identify the source of a log message. It usually identifies the class where the log call occurs.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public void LogWarning(string tag, object message, UnityEngine.Object context)
        {
            if (this.IsLogTypeAllowed(LogType.Warning))
            {
                object[] args = new object[] { tag, GetString(message) };
                this.logHandler.LogFormat(LogType.Warning, context, "{0}: {1}", args);
            }
        }

        /// <summary>
        /// <para>To selective enable debug log message.</para>
        /// </summary>
        public LogType filterLogType { get; set; }

        /// <summary>
        /// <para>To runtime toggle debug logging [ON/OFF].</para>
        /// </summary>
        public bool logEnabled { get; set; }

        /// <summary>
        /// <para>Set  Logger.ILogHandler.</para>
        /// </summary>
        public ILogHandler logHandler { get; set; }
    }
}

