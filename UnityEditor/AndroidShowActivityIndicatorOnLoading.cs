namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Application should show ActivityIndicator when loading.</para>
    /// </summary>
    public enum AndroidShowActivityIndicatorOnLoading
    {
        /// <summary>
        /// <para>Don't Show.</para>
        /// </summary>
        DontShow = -1,
        /// <summary>
        /// <para>Inversed Large.</para>
        /// </summary>
        InversedLarge = 1,
        /// <summary>
        /// <para>Inversed Small.</para>
        /// </summary>
        InversedSmall = 3,
        /// <summary>
        /// <para>Large.</para>
        /// </summary>
        Large = 0,
        /// <summary>
        /// <para>Small.</para>
        /// </summary>
        Small = 2
    }
}

