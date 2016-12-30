namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>Enumerator list of different activity indicators your game can show when loading.</para>
    /// </summary>
    public enum TizenActivityIndicatorStyle
    {
        /// <summary>
        /// <para>Sets your game not to show any indicator while loading.</para>
        /// </summary>
        DontShow = -1,
        /// <summary>
        /// <para>The loading indicator size is large and rotates counterclockwise (progress_large and inverted).</para>
        /// </summary>
        InversedLarge = 1,
        /// <summary>
        /// <para>The loading indicator size is small and rotates counterclockwise (process_small and inverted).</para>
        /// </summary>
        InversedSmall = 3,
        /// <summary>
        /// <para>The loading indicator size is large and rotates clockwise (progress_large).</para>
        /// </summary>
        Large = 0,
        /// <summary>
        /// <para>The loading indicator size is small and rotates clockwise (process_small).</para>
        /// </summary>
        Small = 2
    }
}

