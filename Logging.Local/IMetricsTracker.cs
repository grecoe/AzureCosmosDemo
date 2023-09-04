namespace Logging.Local
{
    /// <summary>
    /// Interface for the Metrics Tracking framework.
    /// </summary>
    public interface IMetricsTracker
    {
        /// <summary>
        /// Set Properties Dict at a thread global level
        /// to be translated to metrics dimensions.
        /// </summary>
        /// <param name="props">The thread golbal props dict.</param>
        /// <param name="overrideProps">Over-write existing props dict.</param>
        void SetThreadGlobalProperties(Dictionary<string, string> props, bool overrideProps = false);

        /// <summary>
        /// Getter for Common Metrics Context.
        /// </summary>
        /// <returns>The <see cref="CommonMetricsContext"/> object for the Metrics Tracker.</returns>
        public CommonMetricsContext GetCommonMetricsContext();

        /// <summary>
        /// Track Metrics.
        /// </summary>
        /// <param name="name">The metrics name.</param>
        /// <param name="value">The metric value.</param>
        /// <param name="trackProps">Additional dimensions that must be tracked.</param>
        void TrackMetric(string name, ulong value, Dictionary<string, string>? trackProps = null);
    }
}
