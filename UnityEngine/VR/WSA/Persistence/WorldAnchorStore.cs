namespace UnityEngine.VR.WSA.Persistence
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngine.VR.WSA;

    /// <summary>
    /// <para>The storage object for persisted WorldAnchors.</para>
    /// </summary>
    public sealed class WorldAnchorStore
    {
        private IntPtr m_NativePtr = IntPtr.Zero;
        private static WorldAnchorStore s_Instance = null;

        private WorldAnchorStore(IntPtr nativePtr)
        {
            this.m_NativePtr = nativePtr;
        }

        /// <summary>
        /// <para>Clears all persisted WorldAnchors.</para>
        /// </summary>
        public void Clear()
        {
            Clear_Internal(this.m_NativePtr);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Clear_Internal(IntPtr context);
        /// <summary>
        /// <para>Deletes a persisted WorldAnchor from the store.</para>
        /// </summary>
        /// <param name="id">The identifier of the WorldAnchor to delete.</param>
        /// <returns>
        /// <para>Whether or not the WorldAnchor was found and deleted.</para>
        /// </returns>
        public bool Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id must not be null or empty", "id");
            }
            return Delete_Internal(this.m_NativePtr, id);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool Delete_Internal(IntPtr context, string id);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Destruct_Internal(IntPtr context);
        /// <summary>
        /// <para>Gets all of the identifiers of the currently persisted WorldAnchors.</para>
        /// </summary>
        /// <returns>
        /// <para>An array of string identifiers.</para>
        /// </returns>
        public string[] GetAllIds()
        {
            string[] ids = new string[this.anchorCount];
            this.GetAllIds(ids);
            return ids;
        }

        /// <summary>
        /// <para>Gets all of the identifiers of the currently persisted WorldAnchors.</para>
        /// </summary>
        /// <param name="ids">A target array to receive the identifiers of the currently persisted world anchors.</param>
        /// <returns>
        /// <para>The number of identifiers stored in the target array.</para>
        /// </returns>
        public int GetAllIds(string[] ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException("ids");
            }
            if (ids.Length > 0)
            {
                return GetAllIds_Internal(this.m_NativePtr, ids);
            }
            return 0;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int GetAllIds_Internal(IntPtr context, string[] ids);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int GetAnchorCount_Internal(IntPtr context);
        public static void GetAsync(GetAsyncDelegate onCompleted)
        {
            if (s_Instance != null)
            {
                onCompleted(s_Instance);
            }
            else
            {
                GetAsync_Internal(onCompleted);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void GetAsync_Internal(GetAsyncDelegate onCompleted);
        [RequiredByNativeCode]
        private static void InvokeGetAsyncDelegate(GetAsyncDelegate handler, IntPtr nativePtr)
        {
            s_Instance = new WorldAnchorStore(nativePtr);
            handler(s_Instance);
        }

        /// <summary>
        /// <para>Loads a WorldAnchor from disk for given identifier and attaches it to the GameObject. If the GameObject has a WorldAnchor, that WorldAnchor will be updated. If the anchor is not found, null will be returned and the GameObject and any existing WorldAnchor attached to it will not be modified.</para>
        /// </summary>
        /// <param name="id">The identifier of the WorldAnchor to load.</param>
        /// <param name="go">The object to attach the WorldAnchor to if found.</param>
        /// <returns>
        /// <para>The WorldAnchor loaded by the identifier or null if not found.</para>
        /// </returns>
        public WorldAnchor Load(string id, GameObject go)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id must not be null or empty", "id");
            }
            if (go == null)
            {
                throw new ArgumentNullException("anchor");
            }
            WorldAnchor component = go.GetComponent<WorldAnchor>();
            bool flag = component != null;
            if (component == null)
            {
                component = go.AddComponent<WorldAnchor>();
            }
            if (Load_Internal(this.m_NativePtr, id, component))
            {
                return go.GetComponent<WorldAnchor>();
            }
            if (!flag)
            {
                UnityEngine.Object.DestroyImmediate(component);
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool Load_Internal(IntPtr context, string id, WorldAnchor anchor);
        /// <summary>
        /// <para>Saves the provided WorldAnchor with the provided identifier. If the identifier is already in use, the method will return false.</para>
        /// </summary>
        /// <param name="id">The identifier to save the anchor with. This needs to be unique for your app.</param>
        /// <param name="anchor">The anchor to save.</param>
        /// <returns>
        /// <para>Whether or not the save was successful. Will return false if the id conflicts with another already saved anchor's id.</para>
        /// </returns>
        public bool Save(string id, WorldAnchor anchor)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id must not be null or empty", "id");
            }
            if (anchor == null)
            {
                throw new ArgumentNullException("anchor");
            }
            return Save_Internal(this.m_NativePtr, id, anchor);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool Save_Internal(IntPtr context, string id, WorldAnchor anchor);

        /// <summary>
        /// <para>(Read Only) Gets the number of persisted world anchors in this WorldAnchorStore.</para>
        /// </summary>
        public int anchorCount =>
            GetAnchorCount_Internal(this.m_NativePtr);

        /// <summary>
        /// <para>The handler for when getting the WorldAnchorStore from GetAsync.</para>
        /// </summary>
        /// <param name="store">The instance of the WorldAnchorStore once loaded.</param>
        public delegate void GetAsyncDelegate(WorldAnchorStore store);
    }
}

