namespace UnityEngine.SceneManagement
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>Run-time data structure for *.unity file.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Scene
    {
        private int m_Handle;
        internal int handle =>
            this.m_Handle;
        internal LoadingState loadingState =>
            GetLoadingStateInternal(this.handle);
        /// <summary>
        /// <para>Whether this is a valid scene.
        /// A scene may be invalid if, for example, you tried to open a scene that does not exist. In this case, the scene returned from EditorSceneManager.OpenScene would return False for IsValid.</para>
        /// </summary>
        /// <returns>
        /// <para>Whether this is a valid scene.</para>
        /// </returns>
        public bool IsValid() => 
            IsValidInternal(this.handle);

        /// <summary>
        /// <para>Returns the relative path of the scene. Like: "AssetsMyScenesMyScene.unity".</para>
        /// </summary>
        public string path =>
            GetPathInternal(this.handle);
        /// <summary>
        /// <para>Returns the name of the scene.</para>
        /// </summary>
        public string name
        {
            get => 
                GetNameInternal(this.handle);
            internal set
            {
                SetNameInternal(this.handle, value);
            }
        }
        /// <summary>
        /// <para>Returns true if the scene is loaded.</para>
        /// </summary>
        public bool isLoaded =>
            GetIsLoadedInternal(this.handle);
        /// <summary>
        /// <para>Returns the index of the scene in the Build Settings. Always returns -1 if the scene was loaded through an AssetBundle.</para>
        /// </summary>
        public int buildIndex =>
            GetBuildIndexInternal(this.handle);
        /// <summary>
        /// <para>Returns true if the scene is modifed.</para>
        /// </summary>
        public bool isDirty =>
            GetIsDirtyInternal(this.handle);
        /// <summary>
        /// <para>The number of root transforms of this scene.</para>
        /// </summary>
        public int rootCount =>
            GetRootCountInternal(this.handle);
        /// <summary>
        /// <para>Returns all the root game objects in the scene.</para>
        /// </summary>
        /// <returns>
        /// <para>An array of game objects.</para>
        /// </returns>
        public GameObject[] GetRootGameObjects()
        {
            List<GameObject> rootGameObjects = new List<GameObject>(this.rootCount);
            this.GetRootGameObjects(rootGameObjects);
            return rootGameObjects.ToArray();
        }

        public void GetRootGameObjects(List<GameObject> rootGameObjects)
        {
            if (rootGameObjects.Capacity < this.rootCount)
            {
                rootGameObjects.Capacity = this.rootCount;
            }
            rootGameObjects.Clear();
            if (!this.IsValid())
            {
                throw new ArgumentException("The scene is invalid.");
            }
            if (!this.isLoaded)
            {
                throw new ArgumentException("The scene is not loaded.");
            }
            if (this.rootCount != 0)
            {
                GetRootGameObjectsInternal(this.handle, rootGameObjects);
            }
        }

        public static bool operator ==(Scene lhs, Scene rhs) => 
            (lhs.handle == rhs.handle);

        public static bool operator !=(Scene lhs, Scene rhs) => 
            (lhs.handle != rhs.handle);

        public override int GetHashCode() => 
            this.m_Handle;

        public override bool Equals(object other)
        {
            if (!(other is Scene))
            {
                return false;
            }
            Scene scene = (Scene) other;
            return (this.handle == scene.handle);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool IsValidInternal(int sceneHandle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string GetPathInternal(int sceneHandle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string GetNameInternal(int sceneHandle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetNameInternal(int sceneHandle, string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool GetIsLoadedInternal(int sceneHandle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern LoadingState GetLoadingStateInternal(int sceneHandle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool GetIsDirtyInternal(int sceneHandle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int GetBuildIndexInternal(int sceneHandle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int GetRootCountInternal(int sceneHandle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void GetRootGameObjectsInternal(int sceneHandle, object resultRootList);
        internal enum LoadingState
        {
            NotLoaded,
            Loading,
            Loaded
        }
    }
}

