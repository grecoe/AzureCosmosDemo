namespace Logging.Local
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A class to enable passing key-value pairs to add as columns to be logged.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public class CustomLogProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomLogProperty"/> class.
        /// </summary>
        /// <param name="key">Value of Key to be logged.</param>
        /// <param name="value">Value of Value to be logged.</param>
        public CustomLogProperty(string key, object value)
        {
            this.Key = key;
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the Key of the Key-Value pair.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value of the Key-Value pair.
        /// </summary>
        public object Value { get; set; }
    }
}
