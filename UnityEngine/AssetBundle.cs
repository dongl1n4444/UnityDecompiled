namespace UnityEngine
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using UnityEngine.Internal;
    using UnityEngineInternal;

    /// <summary>
    /// <para>AssetBundles let you stream additional assets via the WWW class and instantiate them at runtime. AssetBundles are created via BuildPipeline.BuildAssetBundle.</para>
    /// </summary>
    public sealed class AssetBundle : UnityEngine.Object
    {
        [Obsolete("This method is deprecated. Use GetAllAssetNames() instead.")]
        public string[] AllAssetNames() => 
            this.GetAllAssetNames();

        /// <summary>
        /// <para>Check if an AssetBundle contains a specific object.</para>
        /// </summary>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool Contains(string name);
        /// <summary>
        /// <para>Loads an asset bundle from a disk.</para>
        /// </summary>
        /// <param name="path">Path of the file on disk
        /// 
        /// See Also: WWW.assetBundle, WWW.LoadFromCacheOrDownload.</param>
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Method CreateFromFile has been renamed to LoadFromFile (UnityUpgradable) -> LoadFromFile(*)", true)]
        public static AssetBundle CreateFromFile(string path) => 
            null;

        /// <summary>
        /// <para>Asynchronously create an AssetBundle from a memory region.</para>
        /// </summary>
        /// <param name="binary"></param>
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Method CreateFromMemory has been renamed to LoadFromMemoryAsync (UnityUpgradable) -> LoadFromMemoryAsync(*)", true)]
        public static AssetBundleCreateRequest CreateFromMemory(byte[] binary) => 
            null;

        /// <summary>
        /// <para>Synchronously create an AssetBundle from a memory region.</para>
        /// </summary>
        /// <param name="binary">Array of bytes with the AssetBundle data.</param>
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Method CreateFromMemoryImmediate has been renamed to LoadFromMemory (UnityUpgradable) -> LoadFromMemory(*)", true)]
        public static AssetBundle CreateFromMemoryImmediate(byte[] binary) => 
            null;

        /// <summary>
        /// <para>Return all asset names in the AssetBundle.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern string[] GetAllAssetNames();
        /// <summary>
        /// <para>Return all the scene asset paths (paths to *.unity assets) in the AssetBundle.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern string[] GetAllScenePaths();
        [Obsolete("Method Load has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAsset instead and check the documentation for details.", true)]
        public UnityEngine.Object Load(string name) => 
            null;

        [Obsolete("Method Load has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAsset instead and check the documentation for details.", true)]
        public T Load<T>(string name) where T: UnityEngine.Object => 
            null;

        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("Method Load has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAsset instead and check the documentation for details.", true), TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
        public extern UnityEngine.Object Load(string name, System.Type type);
        [Obsolete("Method LoadAll has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAllAssets instead and check the documentation for details.", true)]
        public UnityEngine.Object[] LoadAll() => 
            null;

        [Obsolete("Method LoadAll has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAllAssets instead and check the documentation for details.", true)]
        public T[] LoadAll<T>() where T: UnityEngine.Object => 
            null;

        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("Method LoadAll has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAllAssets instead and check the documentation for details.", true)]
        public extern UnityEngine.Object[] LoadAll(System.Type type);
        /// <summary>
        /// <para>Loads all assets contained in the asset bundle.</para>
        /// </summary>
        public UnityEngine.Object[] LoadAllAssets() => 
            this.LoadAllAssets(typeof(UnityEngine.Object));

        public T[] LoadAllAssets<T>() where T: UnityEngine.Object => 
            Resources.ConvertObjects<T>(this.LoadAllAssets(typeof(T)));

        /// <summary>
        /// <para>Loads all assets contained in the asset bundle that inherit from type.</para>
        /// </summary>
        /// <param name="type"></param>
        public UnityEngine.Object[] LoadAllAssets(System.Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException("The input type cannot be null.");
            }
            return this.LoadAssetWithSubAssets_Internal("", type);
        }

        /// <summary>
        /// <para>Loads all assets contained in the asset bundle asynchronously.</para>
        /// </summary>
        public AssetBundleRequest LoadAllAssetsAsync() => 
            this.LoadAllAssetsAsync(typeof(UnityEngine.Object));

        public AssetBundleRequest LoadAllAssetsAsync<T>() => 
            this.LoadAllAssetsAsync(typeof(T));

        /// <summary>
        /// <para>Loads all assets contained in the asset bundle that inherit from type asynchronously.</para>
        /// </summary>
        /// <param name="type"></param>
        public AssetBundleRequest LoadAllAssetsAsync(System.Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException("The input type cannot be null.");
            }
            return this.LoadAssetWithSubAssetsAsync_Internal("", type);
        }

        /// <summary>
        /// <para>Loads asset with name from the bundle.</para>
        /// </summary>
        /// <param name="name"></param>
        public UnityEngine.Object LoadAsset(string name) => 
            this.LoadAsset(name, typeof(UnityEngine.Object));

        public T LoadAsset<T>(string name) where T: UnityEngine.Object => 
            ((T) this.LoadAsset(name, typeof(T)));

        /// <summary>
        /// <para>Loads asset with name of a given type from the bundle.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        [TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
        public UnityEngine.Object LoadAsset(string name, System.Type type)
        {
            if (name == null)
            {
                throw new NullReferenceException("The input asset name cannot be null.");
            }
            if (name.Length == 0)
            {
                throw new ArgumentException("The input asset name cannot be empty.");
            }
            if (type == null)
            {
                throw new NullReferenceException("The input type cannot be null.");
            }
            return this.LoadAsset_Internal(name, type);
        }

        [MethodImpl(MethodImplOptions.InternalCall), TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
        private extern UnityEngine.Object LoadAsset_Internal(string name, System.Type type);
        /// <summary>
        /// <para>Asynchronously loads asset with name from the bundle.</para>
        /// </summary>
        /// <param name="name"></param>
        public AssetBundleRequest LoadAssetAsync(string name) => 
            this.LoadAssetAsync(name, typeof(UnityEngine.Object));

        public AssetBundleRequest LoadAssetAsync<T>(string name) => 
            this.LoadAssetAsync(name, typeof(T));

        /// <summary>
        /// <para>Asynchronously loads asset with name of a given type from the bundle.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public AssetBundleRequest LoadAssetAsync(string name, System.Type type)
        {
            if (name == null)
            {
                throw new NullReferenceException("The input asset name cannot be null.");
            }
            if (name.Length == 0)
            {
                throw new ArgumentException("The input asset name cannot be empty.");
            }
            if (type == null)
            {
                throw new NullReferenceException("The input type cannot be null.");
            }
            return this.LoadAssetAsync_Internal(name, type);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern AssetBundleRequest LoadAssetAsync_Internal(string name, System.Type type);
        /// <summary>
        /// <para>Loads asset and sub assets with name from the bundle.</para>
        /// </summary>
        /// <param name="name"></param>
        public UnityEngine.Object[] LoadAssetWithSubAssets(string name) => 
            this.LoadAssetWithSubAssets(name, typeof(UnityEngine.Object));

        public T[] LoadAssetWithSubAssets<T>(string name) where T: UnityEngine.Object => 
            Resources.ConvertObjects<T>(this.LoadAssetWithSubAssets(name, typeof(T)));

        /// <summary>
        /// <para>Loads asset and sub assets with name of a given type from the bundle.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public UnityEngine.Object[] LoadAssetWithSubAssets(string name, System.Type type)
        {
            if (name == null)
            {
                throw new NullReferenceException("The input asset name cannot be null.");
            }
            if (name.Length == 0)
            {
                throw new ArgumentException("The input asset name cannot be empty.");
            }
            if (type == null)
            {
                throw new NullReferenceException("The input type cannot be null.");
            }
            return this.LoadAssetWithSubAssets_Internal(name, type);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern UnityEngine.Object[] LoadAssetWithSubAssets_Internal(string name, System.Type type);
        /// <summary>
        /// <para>Loads asset with sub assets with name from the bundle asynchronously.</para>
        /// </summary>
        /// <param name="name"></param>
        public AssetBundleRequest LoadAssetWithSubAssetsAsync(string name) => 
            this.LoadAssetWithSubAssetsAsync(name, typeof(UnityEngine.Object));

        public AssetBundleRequest LoadAssetWithSubAssetsAsync<T>(string name) => 
            this.LoadAssetWithSubAssetsAsync(name, typeof(T));

        /// <summary>
        /// <para>Loads asset with sub assets with name of a given type from the bundle asynchronously.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public AssetBundleRequest LoadAssetWithSubAssetsAsync(string name, System.Type type)
        {
            if (name == null)
            {
                throw new NullReferenceException("The input asset name cannot be null.");
            }
            if (name.Length == 0)
            {
                throw new ArgumentException("The input asset name cannot be empty.");
            }
            if (type == null)
            {
                throw new NullReferenceException("The input type cannot be null.");
            }
            return this.LoadAssetWithSubAssetsAsync_Internal(name, type);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern AssetBundleRequest LoadAssetWithSubAssetsAsync_Internal(string name, System.Type type);
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("Method LoadAsync has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAssetAsync instead and check the documentation for details.", true)]
        public extern AssetBundleRequest LoadAsync(string name, System.Type type);
        [ExcludeFromDocs]
        public static AssetBundle LoadFromFile(string path)
        {
            ulong offset = 0L;
            uint crc = 0;
            return LoadFromFile(path, crc, offset);
        }

        [ExcludeFromDocs]
        public static AssetBundle LoadFromFile(string path, uint crc)
        {
            ulong offset = 0L;
            return LoadFromFile(path, crc, offset);
        }

        /// <summary>
        /// <para>Synchronously loads an AssetBundle from a file on disk.</para>
        /// </summary>
        /// <param name="path">Path of the file on disk.</param>
        /// <param name="crc">An optional CRC-32 checksum of the uncompressed content. If this is non-zero, then the content will be compared against the checksum before loading it, and give an error if it does not match.</param>
        /// <param name="offset">An optional byte offset. This value specifies where to start reading the AssetBundle from.</param>
        /// <returns>
        /// <para>Loaded AssetBundle object or null if failed.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern AssetBundle LoadFromFile(string path, [UnityEngine.Internal.DefaultValue("0")] uint crc, [UnityEngine.Internal.DefaultValue("0")] ulong offset);
        [ExcludeFromDocs]
        public static AssetBundleCreateRequest LoadFromFileAsync(string path)
        {
            ulong offset = 0L;
            uint crc = 0;
            return LoadFromFileAsync(path, crc, offset);
        }

        [ExcludeFromDocs]
        public static AssetBundleCreateRequest LoadFromFileAsync(string path, uint crc)
        {
            ulong offset = 0L;
            return LoadFromFileAsync(path, crc, offset);
        }

        /// <summary>
        /// <para>Asynchronously loads an AssetBundle from a file on disk.</para>
        /// </summary>
        /// <param name="path">Path of the file on disk.</param>
        /// <param name="crc">An optional CRC-32 checksum of the uncompressed content. If this is non-zero, then the content will be compared against the checksum before loading it, and give an error if it does not match.</param>
        /// <param name="offset">An optional byte offset. This value specifies where to start reading the AssetBundle from.</param>
        /// <returns>
        /// <para>Asynchronous create request for an AssetBundle. Use AssetBundleCreateRequest.assetBundle property to get an AssetBundle once it is loaded.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern AssetBundleCreateRequest LoadFromFileAsync(string path, [UnityEngine.Internal.DefaultValue("0")] uint crc, [UnityEngine.Internal.DefaultValue("0")] ulong offset);
        [ExcludeFromDocs]
        public static AssetBundle LoadFromMemory(byte[] binary)
        {
            uint crc = 0;
            return LoadFromMemory(binary, crc);
        }

        /// <summary>
        /// <para>Synchronously create an AssetBundle from a memory region.</para>
        /// </summary>
        /// <param name="binary">Array of bytes with the AssetBundle data.</param>
        /// <param name="crc">An optional CRC-32 checksum of the uncompressed content. If this is non-zero, then the content will be compared against the checksum before loading it, and give an error if it does not match.</param>
        /// <returns>
        /// <para>Loaded AssetBundle object or null if failed.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern AssetBundle LoadFromMemory(byte[] binary, [UnityEngine.Internal.DefaultValue("0")] uint crc);
        [ExcludeFromDocs]
        public static AssetBundleCreateRequest LoadFromMemoryAsync(byte[] binary)
        {
            uint crc = 0;
            return LoadFromMemoryAsync(binary, crc);
        }

        /// <summary>
        /// <para>Asynchronously create an AssetBundle from a memory region.</para>
        /// </summary>
        /// <param name="binary">Array of bytes with the AssetBundle data.</param>
        /// <param name="crc">An optional CRC-32 checksum of the uncompressed content. If this is non-zero, then the content will be compared against the checksum before loading it, and give an error if it does not match.</param>
        /// <returns>
        /// <para>Asynchronous create request for an AssetBundle. Use AssetBundleCreateRequest.assetBundle property to get an AssetBundle once it is loaded.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern AssetBundleCreateRequest LoadFromMemoryAsync(byte[] binary, [UnityEngine.Internal.DefaultValue("0")] uint crc);
        /// <summary>
        /// <para>Unloads all assets in the bundle.</para>
        /// </summary>
        /// <param name="unloadAllLoadedObjects"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void Unload(bool unloadAllLoadedObjects);

        /// <summary>
        /// <para>Return true if the AssetBundle is a streamed scene AssetBundle.</para>
        /// </summary>
        public bool isStreamedSceneAssetBundle { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Main asset that was supplied when building the asset bundle (Read Only).</para>
        /// </summary>
        public UnityEngine.Object mainAsset { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

