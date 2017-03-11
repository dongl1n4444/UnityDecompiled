namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Manifest for all the AssetBundles in the build.</para>
    /// </summary>
    public sealed class AssetBundleManifest : UnityEngine.Object
    {
        /// <summary>
        /// <para>Get all the AssetBundles in the manifest.</para>
        /// </summary>
        /// <returns>
        /// <para>An array of asset bundle names.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern string[] GetAllAssetBundles();
        /// <summary>
        /// <para>Get all the AssetBundles with variant in the manifest.</para>
        /// </summary>
        /// <returns>
        /// <para>An array of asset bundle names.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern string[] GetAllAssetBundlesWithVariant();
        /// <summary>
        /// <para>Get all the dependent AssetBundles for the given AssetBundle.</para>
        /// </summary>
        /// <param name="assetBundleName">Name of the asset bundle.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern string[] GetAllDependencies(string assetBundleName);
        /// <summary>
        /// <para>Get the hash for the given AssetBundle.</para>
        /// </summary>
        /// <param name="assetBundleName">Name of the asset bundle.</param>
        /// <returns>
        /// <para>The 128-bit hash for the asset bundle.</para>
        /// </returns>
        public Hash128 GetAssetBundleHash(string assetBundleName)
        {
            Hash128 hash;
            INTERNAL_CALL_GetAssetBundleHash(this, assetBundleName, out hash);
            return hash;
        }

        /// <summary>
        /// <para>Get the direct dependent AssetBundles for the given AssetBundle.</para>
        /// </summary>
        /// <param name="assetBundleName">Name of the asset bundle.</param>
        /// <returns>
        /// <para>Array of asset bundle names this asset bundle depends on.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern string[] GetDirectDependencies(string assetBundleName);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetAssetBundleHash(AssetBundleManifest self, string assetBundleName, out Hash128 value);
    }
}

