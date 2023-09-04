namespace Logging.Local
{
    using System.Diagnostics;

    public class OperationLocalLogger : ILogger, IDisposable
    {
        public String CorrelationId { get; set; }

        private readonly Stopwatch stopwatch;

        private string operationName;

        private ILogger logger;

        private IMetricsTracker metricsTracker;

        private bool emitOpStats;

        private bool emitMetrics;

        private CustomLogProperty[] operationLogContext;

        private bool disposed = false;

        public OperationLocalLogger(
            string operationName,
            ILogger logger,
#pragma warning disable CS8625
            IMetricsTracker metricsTracker = null,
#pragma warning restore CS8625
            bool emitOpStats = true,
            bool emitMetrics = true,
            params CustomLogProperty[] operationLogContext)
        {
            this.operationName = operationName;
            this.logger = logger;
            this.metricsTracker = metricsTracker;

            // Unused
            this.operationLogContext = operationLogContext;
            this.emitOpStats = emitOpStats;
            this.emitMetrics = emitMetrics;

            this.stopwatch = new Stopwatch();
            this.stopwatch.Restart();

            this.EmitOperationStartLog();
        }

        public ILogger Copy()
        {
            return new OperationLocalLogger(
                    this.operationName,
                    this.logger,
                    this.metricsTracker,
                    this.emitOpStats,
                    this.emitMetrics,
                    this.operationLogContext);
        }

        public void LogCritical(string message, params CustomLogProperty[] customLogProperties)
        {
            this.logger.LogCritical(message, customLogProperties);
        }

        public void LogDebug(string message, params CustomLogProperty[] customLogProperties)
        {
            this.logger.LogDebug(message, customLogProperties);
        }

        public void LogError(string message, params CustomLogProperty[] customLogProperties)
        {
            this.logger.LogError(message, customLogProperties);
        }

        public void LogException(Exception exception, string message, params CustomLogProperty[] customLogProperties)
        {
            this.logger.LogException(exception, message, customLogProperties);
        }

        public void LogInformation(string message, params CustomLogProperty[] customLogProperties)
        {
            this.logger.LogInformation(message, customLogProperties);
        }


        public void LogTrace(string message, params CustomLogProperty[] customLogProperties)
        {
            this.logger.LogTrace(message, customLogProperties);
        }

        public void LogWarning(string message, params CustomLogProperty[] customLogProperties)
        {
            this.logger.LogWarning(message, customLogProperties);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Implementing IDisposable interface.
        /// </summary>
        /// <param name="disposing">Release both managed and unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.stopwatch.Stop();
                this.EmitOperationEndLog();
            }

            this.disposed = true;
        }

        private void EmitOperationStartLog()
        {
            this.logger.LogDebug(String.Format("{0} started", this.operationName), operationLogContext);
        }

        private void EmitOperationEndLog()
        {
            this.logger.LogDebug(String.Format("{0} completed : {1} seconds",
                this.operationName,
                this.stopwatch.Elapsed.TotalSeconds));
        }



        #region Not Implemented
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
            throw new NotImplementedException();
        }

        public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, Microsoft.Extensions.Logging.EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            throw new NotImplementedException();
        }
        public void LogOpStats(Microsoft.Extensions.Logging.LogLevel logLevel, string message, IMetricsTracker metricsTracker, ulong metricValue = 1, params CustomLogProperty[] customLogProperties)
        {
            throw new NotImplementedException();
        }

        public void SetThreadGlobalProperties(List<CustomLogProperty> props, bool overrideProps = false)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
