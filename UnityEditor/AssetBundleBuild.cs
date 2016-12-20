namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>AssetBundle building map entry.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AssetBundleBuild
    {
        /// <summary>
        /// <para>AssetBundle name.</para>
        /// </summary>
        public string assetBundleName;
        /// <summary>
        /// <para>AssetBundle variant.</para>
        /// </summary>
        public string assetBundleVariant;
        /// <summary>
        /// <para>Asset names which belong to the given AssetBundle.</para>
        /// </summary>
        public string[] assetNames;
    }
}

