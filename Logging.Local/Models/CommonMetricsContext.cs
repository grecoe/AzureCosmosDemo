namespace Logging.Local
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// This class holds the Common Metrics Context
    /// that must be added to the Metrics as Dimensions
    /// and is common for an instantiation.
    /// </summary>
#pragma warning disable CS8618
    [ExcludeFromCodeCoverageAttribute]
    public class CommonMetricsContext
    {
        /// <summary>
        /// Gets or sets the version of the component tracking metrics.
        /// </summary>
        public string ComponentVersion { get; set; }

        /// <summary>
        /// Gets or sets the name of the component tracking metrics.
        /// </summary>
        public string ComponentName { get; set; }

        /// <summary>
        /// Gets or sets the version of the oep version.
        /// </summary>
        public string OEPVersion { get; set; }

        /// <summary>
        /// Gets or sets the environment: prod, dev, test, dogfood.
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        public string BCDRMode { get; set; }
    }
#pragma warning restore CS8618

}
