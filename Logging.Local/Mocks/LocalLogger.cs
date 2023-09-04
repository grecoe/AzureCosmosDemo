
namespace Logging.Local
{
    using Newtonsoft.Json;

    public class LocalLogger : ILogger
    {
 
        public String CorrelationId { get; set; }
        public string? LogFile { get; private set; }

        private object LogLock = new object();

        private bool LogAsObject { get; set; }  

        private bool LogToFile
        {
            get
            {
                return !String.IsNullOrEmpty(LogFile);
            }
        }

        public LocalLogger(string? logFile = null, bool logAsObject = false)
        {
            LogFile = logFile;
            LogAsObject = logAsObject;
        }

        public ILogger Copy()
        {
            return new LocalLogger(LogFile, LogAsObject);
        }

        public void LogCritical(string message, params CustomLogProperty[] customLogProperties)
        {
            WriteLog("CRITICAL", message, null, customLogProperties);
        }

        public void LogDebug(string message, params CustomLogProperty[] customLogProperties)
        {
            WriteLog("DEBUG", message, null, customLogProperties);
        }

        public void LogError(string message, params CustomLogProperty[] customLogProperties)
        {
            WriteLog("ERROR", message, null, customLogProperties);
        }

        public void LogException(Exception exception, string message, params CustomLogProperty[] customLogProperties)
        {
            WriteLog("EXCEPTION", message,  exception, customLogProperties);
        }

        public void LogInformation(string message, params CustomLogProperty[] customLogProperties)
        {
            WriteLog("INFO", message, null, customLogProperties);
        }


        public void LogTrace(string message, params CustomLogProperty[] customLogProperties)
        {
            WriteLog("TRACE", message, null, customLogProperties);
        }

        public void LogWarning(string message, params CustomLogProperty[] customLogProperties)
        {
            WriteLog("WARN", message, null, customLogProperties);
        }

        private void WriteLog(string level, string message, Exception? ex, params CustomLogProperty[] customLogProperties)
        {
            try
            {
                string output_mesage = PrepareMessage(level, message, ex, customLogProperties);

                if (LogToFile)
                {
                    lock (LogLock)
                    {
                        using (StreamWriter w = File.AppendText(this.LogFile))
                        {
                            w.WriteLine(output_mesage);
                        }
                    }
                }

                Console.WriteLine(output_mesage);
            }
            catch(Exception exception)
            {
                Console.WriteLine($"EXCEPTION WRITING LOG - {exception.Message}");
            }
        }

        private string PrepareMessage(string level, string message, Exception? ex, params CustomLogProperty[] customLogProperties)
        {
            DateTime time = DateTime.UtcNow;
            string output_message = String.Empty;
            string time_entry = String.Format("{0:yyyy-MM-dd HH:mm:ss}", time);

            if (LogAsObject)
            {
                var msg = new Dictionary<string, string>()
                {
                    { "time" ,time_entry },
                    {"level" , level },
                    { "message" , message }
                };

                if(ex != null)
                {
                    msg.Add("Exception", ex.ToString());
                }

                if(!String.IsNullOrEmpty(CorrelationId))
                {
                    msg.Add("CorrelationId", CorrelationId);
                }

                foreach (CustomLogProperty logProp in customLogProperties)
                {
                    msg.Add(logProp.Key, logProp.Value.ToString());
                }

                output_message = JsonConvert.SerializeObject(msg, Formatting.Indented);
            }
            else
            {
                if (!String.IsNullOrEmpty(CorrelationId))
                {
                    String output_format = "{0} : [{1}] : [{2}] : {3}";

                    output_message = String.Format(
                        output_format,
                        time_entry,
                        level,
                        CorrelationId,
                        message);
                }
                else
                {
                    String output_format = "{0} : [{1}] : {2}";

                    output_message = String.Format(
                        output_format,
                        time_entry,
                        level,
                        message);
                }

                if(ex != null)
                {
                    output_message += $"{Environment.NewLine}Exception: {ex.ToString()}";
                }

                foreach (CustomLogProperty logProp in customLogProperties)
                {
                    output_message += $"{Environment.NewLine}{logProp.Key} : {logProp.Value.ToString()}";
                }
            }

            return output_message;
        }

        #region Not Immplemented interface parts
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            throw new NotImplementedException();
        }

        public string GetEventSourceName()
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, Microsoft.Extensions.Logging.EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            throw new NotImplementedException();
        }

        public void SetThreadGlobalProperties(List<CustomLogProperty> props, bool overrideProps = false)
        {
            throw new NotImplementedException();
        }
        public void LogOpStats(Microsoft.Extensions.Logging.LogLevel logLevel, string message, IMetricsTracker metricsTracker, ulong metricValue = 1, params CustomLogProperty[] customLogProperties)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
