namespace Logging.Local
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Interface for the Logging framework.
    /// </summary>
    public interface ILogger : Microsoft.Extensions.Logging.ILogger
    {
        public String CorrelationId { get; set; }

        /// <summary>
        /// Copies over the logger to put in a correlation id for different services
        /// <summary></summary>
        ILogger Copy();

        /// <summary>
        /// Log Operation Stats Events.
        /// </summary>
        /// <param name="logLevel">The <see cref="LogLevel"/> for the OpStats log.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="metricsTracker">A <see cref="MetricsTracker"/> instance to track OpStats as metrics.</param>
        /// <param name="metricValue">An optional value to be passed as metric value, default to 1.</param>
        /// <param name="customLogProperties">Addtional props to be logged as columns.</param>
        void LogOpStats(Microsoft.Extensions.Logging.LogLevel logLevel, string message, IMetricsTracker metricsTracker, ulong metricValue = 1, params CustomLogProperty[] customLogProperties);

        /// <summary>
        /// Log Critical or Fatal Events.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="customLogProperties">Addtional props to be logged as columns.</param>
        void LogCritical(string message, params CustomLogProperty[] customLogProperties);

        /// <summary>
        /// Log Exception Events.
        /// </summary>
        /// <param name="exception">The exception to be logged.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="customLogProperties">Addtional props to be logged as columns.</param>
        void LogException(Exception exception, string message, params CustomLogProperty[] customLogProperties);

        /// <summary>
        /// Log Error Events.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="customLogProperties">Addtional props to be logged as columns.</param>
        void LogError(string message, params CustomLogProperty[] customLogProperties);

        /// <summary>
        /// Log Warning Events.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="customLogProperties">Addtional props to be logged as columns.</param>
        void LogWarning(string message, params CustomLogProperty[] customLogProperties);

        /// <summary>
        /// Log Info Events.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="customLogProperties">Addtional props to be logged as columns.</param>
        void LogInformation(string message, params CustomLogProperty[] customLogProperties);

        /// <summary>
        /// Log Debug Events.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="customLogProperties">Addtional props to be logged as columns.</param>
        void LogDebug(string message, params CustomLogProperty[] customLogProperties);

        /// <summary>
        /// Log Trace Events.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="customLogProperties">Addtional props to be logged as columns.</param>
        void LogTrace(string message, params CustomLogProperty[] customLogProperties);

        /// <summary>
        /// Gets the event source name.
        /// </summary>
        /// <returns> returns event source.</returns>
        string GetEventSourceName();

        /// <summary>
        /// Set Properties Dict at a thread global level
        /// to be translated to log table columns.
        /// </summary>
        /// <param name="props">The thread global props dict.</param>
        /// <param name="overrideProps">Over-write existing props dict.</param>
        void SetThreadGlobalProperties(List<CustomLogProperty> props, bool overrideProps = false);
    }
}
