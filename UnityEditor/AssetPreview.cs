namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Utility for fetching asset previews by instance ID of assets, See AssetPreview.GetAssetPreview. Since previews are loaded asynchronously methods are provided for requesting if all previews have been fully loaded, see AssetPreview.IsLoadingAssetPreviews. Loaded previews are stored in a cache, the size of the cache can be controlled by calling [AssetPreview.SetPreviewTextureCacheSize].</para>
    /// </summary>
    public sealed class AssetPreview
    {
        private const int kSharedClientID = 0;

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void ClearTemporaryAssetPreviews();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void DeletePreviewTextureManagerByID(int clientID);
        internal static Texture2D GetAssetPreview(int instanceID) => 
            GetAssetPreview(instanceID, 0);

        /// <summary>
        /// <para>Returns a preview texture for an asset.</para>
        /// </summary>
        /// <param name="asset"></param>
        public static Texture2D GetAssetPreview(Object asset)
        {
            if (asset != null)
            {
                return GetAssetPreview(asset.GetInstanceID());
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern Texture2D GetAssetPreview(int instanceID, int clientID);
        /// <summary>
        /// <para>Returns the thumbnail for an object (like the ones you see in the project view).</para>
        /// </summary>
        /// <param name="obj"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern Texture2D GetMiniThumbnail(Object obj);
        /// <summary>
        /// <para>Returns the thumbnail for the type.</para>
        /// </summary>
        /// <param name="type"></param>
        public static Texture2D GetMiniTypeThumbnail(Type type)
        {
            if (typeof(MonoBehaviour).IsAssignableFrom(type))
            {
                return EditorGUIUtility.LoadIcon(type.FullName.Replace('.', '/') + " Icon");
            }
            return INTERNAL_GetMiniTypeThumbnailFromType(type);
        }

        internal static Texture2D GetMiniTypeThumbnail(Object obj) => 
            INTERNAL_GetMiniTypeThumbnailFromObject(obj);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern Texture2D GetMiniTypeThumbnailFromClassID(int classID);
        internal static bool HasAnyNewPreviewTexturesAvailable() => 
            HasAnyNewPreviewTexturesAvailable(0);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool HasAnyNewPreviewTexturesAvailable(int clientID);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern Texture2D INTERNAL_GetMiniTypeThumbnailFromObject(Object monoObj);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern Texture2D INTERNAL_GetMiniTypeThumbnailFromType(Type managedType);
        /// <summary>
        /// <para>Loading previews is asynchronous so it is useful to know if a specific asset preview is in the process of being loaded so client code e.g can repaint while waiting for the loading to finish.</para>
        /// </summary>
        /// <param name="instanceID">InstanceID of the assset that a preview has been requested for by: AssetPreview.GetAssetPreview().</param>
        public static bool IsLoadingAssetPreview(int instanceID) => 
            IsLoadingAssetPreview(instanceID, 0);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool IsLoadingAssetPreview(int instanceID, int clientID);
        /// <summary>
        /// <para>Loading previews is asynchronous so it is useful to know if any requested previews are in the process of being loaded so client code e.g can repaint while waiting.</para>
        /// </summary>
        public static bool IsLoadingAssetPreviews() => 
            IsLoadingAssetPreviews(0);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool IsLoadingAssetPreviews(int clientID);
        /// <summary>
        /// <para>Set the asset preview cache to a size that can hold all visible previews on the screen at once.</para>
        /// </summary>
        /// <param name="size">The number of previews that can be loaded into the cache before the least used previews are being unloaded.</param>
        public static void SetPreviewTextureCacheSize(int size)
        {
            SetPreviewTextureCacheSize(size, 0);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void SetPreviewTextureCacheSize(int size, int clientID);
    }
}

