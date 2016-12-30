namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Methods to compensate for aspect ratio discrepancies between the source resolution and the wanted encoding size.</para>
    /// </summary>
    public enum VideoEncodeAspectRatio
    {
        /// <summary>
        /// <para>Perform no operation.</para>
        /// </summary>
        NoScaling = 0,
        /// <summary>
        /// <para>Stretch the source to fill the target resolution without preserving the aspect ratio.</para>
        /// </summary>
        Stretch = 4
    }
}

