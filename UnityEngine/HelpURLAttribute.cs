namespace UnityEngine
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Provide a custom documentation URL for a class.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public sealed class HelpURLAttribute : Attribute
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string <URL>k__BackingField;

        /// <summary>
        /// <para>Initialize the HelpURL attribute with a documentation url.</para>
        /// </summary>
        /// <param name="url">The custom documentation URL for this class.</param>
        public HelpURLAttribute(string url)
        {
            this.URL = url;
        }

        /// <summary>
        /// <para>The documentation URL specified for this class.</para>
        /// </summary>
        public string URL { get; private set; }
    }
}

