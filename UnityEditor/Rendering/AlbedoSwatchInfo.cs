namespace UnityEditor.Rendering
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>Contains the custom albedo swatch data.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AlbedoSwatchInfo
    {
        /// <summary>
        /// <para>Name of the albedo swatch to show in the physically based renderer validator user interface.</para>
        /// </summary>
        public string name;
        /// <summary>
        /// <para>Color of the albedo swatch that is shown in the physically based rendering validator user interface.</para>
        /// </summary>
        public Color color;
        /// <summary>
        /// <para>The minimum luminance value used to validate the albedo for the physically based rendering albedo validator.</para>
        /// </summary>
        public float minLuminance;
        /// <summary>
        /// <para>The maximum luminance value used to validate the albedo for the physically based rendering albedo validator.</para>
        /// </summary>
        public float maxLuminance;
    }
}

