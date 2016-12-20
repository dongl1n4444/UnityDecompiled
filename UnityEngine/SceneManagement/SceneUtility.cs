namespace UnityEngine.SceneManagement
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Scene and Build Settings related utilities.</para>
    /// </summary>
    public static class SceneUtility
    {
        /// <summary>
        /// <para>Get the build index from a scene path.</para>
        /// </summary>
        /// <param name="scenePath">Scene path (e.g: "AssetsScenesScene1.unity").</param>
        /// <returns>
        /// <para>Build index.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int GetBuildIndexByScenePath(string scenePath);
        /// <summary>
        /// <para>Get the scene path from a build index.</para>
        /// </summary>
        /// <param name="buildIndex"></param>
        /// <returns>
        /// <para>Scene path (e.g "AssetsScenesScene1.unity").</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GetScenePathByBuildIndex(int buildIndex);
    }
}

