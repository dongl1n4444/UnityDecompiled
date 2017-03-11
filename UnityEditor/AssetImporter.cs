namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// <para>Base class from which asset importers for specific asset types derive.</para>
    /// </summary>
    public class AssetImporter : UnityEngine.Object
    {
        /// <summary>
        /// <para>Retrieves the asset importer for the asset at path.</para>
        /// </summary>
        /// <param name="path"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern AssetImporter GetAtPath(string path);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int LocalFileIDToClassID(long fileId);
        /// <summary>
        /// <para>Save asset importer settings if asset importer is dirty.</para>
        /// </summary>
        public void SaveAndReimport()
        {
            AssetDatabase.ImportAsset(this.assetPath);
        }

        /// <summary>
        /// <para>Set the AssetBundle name and variant.</para>
        /// </summary>
        /// <param name="assetBundleName">AssetBundle name.</param>
        /// <param name="assetBundleVariant">AssetBundle variant.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetAssetBundleNameAndVariant(string assetBundleName, string assetBundleVariant);

        /// <summary>
        /// <para>Get or set the AssetBundle name.</para>
        /// </summary>
        public string assetBundleName { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Get or set the AssetBundle variant.</para>
        /// </summary>
        public string assetBundleVariant { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The path name of the asset for this importer. (Read Only)</para>
        /// </summary>
        public string assetPath { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public ulong assetTimeStamp { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Get or set any user data.</para>
        /// </summary>
        public string userData { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

