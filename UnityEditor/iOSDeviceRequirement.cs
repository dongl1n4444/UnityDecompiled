namespace UnityEditor
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// <para>A device requirement description used for configuration of App Slicing.</para>
    /// </summary>
    public sealed class iOSDeviceRequirement
    {
        private SortedDictionary<string, string> m_Values = new SortedDictionary<string, string>();

        /// <summary>
        /// <para>The values of the device requirement description.</para>
        /// </summary>
        public IDictionary<string, string> values =>
            this.m_Values;
    }
}

