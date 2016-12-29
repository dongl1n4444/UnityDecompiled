namespace UnityEngine.Advertisements
{
    using SimpleJson;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Class for sending various metadata to UnityAds.</para>
    /// </summary>
    public sealed class MetaData
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <category>k__BackingField;
        private readonly IDictionary<string, object> m_MetaData = new Dictionary<string, object>();

        public MetaData(string category)
        {
            this.category = category;
        }

        /// <summary>
        /// <para></para>
        /// </summary>
        /// <param name="key">Metadata key.</param>
        /// <returns>
        /// <para>Stored metadata.</para>
        /// </returns>
        public object Get(string key) => 
            this.m_MetaData[key];

        /// <summary>
        /// <para>Sets new metadata fields.</para>
        /// </summary>
        /// <param name="key">Metadata key.</param>
        /// <param name="value">Metadata value, needs to be JSON serializable.</param>
        public void Set(string key, object value)
        {
            this.m_MetaData[key] = value;
        }

        internal string ToJSON() => 
            SimpleJson.SimpleJson.SerializeObject(this.m_MetaData);

        /// <summary>
        /// <para>Metadata category.</para>
        /// </summary>
        public string category { get; private set; }

        /// <summary>
        /// <para></para>
        /// </summary>
        /// <returns>
        /// <para>Stored metadata dictionary.</para>
        /// </returns>
        public IDictionary<string, object> Values =>
            this.m_MetaData;
    }
}

