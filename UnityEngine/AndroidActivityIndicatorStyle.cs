namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>ActivityIndicator Style (Android Specific).</para>
    /// </summary>
    public enum AndroidActivityIndicatorStyle
    {
        /// <summary>
        /// <para>Do not show ActivityIndicator.</para>
        /// </summary>
        DontShow = -1,
        /// <summary>
        /// <para>Large Inversed (android.R.attr.progressBarStyleLargeInverse).</para>
        /// </summary>
        InversedLarge = 1,
        /// <summary>
        /// <para>Small Inversed (android.R.attr.progressBarStyleSmallInverse).</para>
        /// </summary>
        InversedSmall = 3,
        /// <summary>
        /// <para>Large (android.R.attr.progressBarStyleLarge).</para>
        /// </summary>
        Large = 0,
        /// <summary>
        /// <para>Small (android.R.attr.progressBarStyleSmall).</para>
        /// </summary>
        Small = 2
    }
}

