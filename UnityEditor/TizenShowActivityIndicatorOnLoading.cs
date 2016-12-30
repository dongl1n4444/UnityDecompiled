namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Enumerator list of different activity indicators your game can show when loading.</para>
    /// </summary>
    public enum TizenShowActivityIndicatorOnLoading
    {
        /// <summary>
        /// <para>Sets your game not to show any indicator while loading.</para>
        /// </summary>
        DontShow = -1,
        /// <summary>
        /// <para>The loading indicator size is large and rotates counterclockwise.</para>
        /// </summary>
        InversedLarge = 1,
        /// <summary>
        /// <para>The loading indicator size is small and rotates counterclockwise.</para>
        /// </summary>
        InversedSmall = 3,
        /// <summary>
        /// <para>The loading indicator size is large and rotates clockwise.</para>
        /// </summary>
        Large = 0,
        /// <summary>
        /// <para>The loading indicator size is small and rotates clockwise.</para>
        /// </summary>
        Small = 2
    }
}

