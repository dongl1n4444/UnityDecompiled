namespace UnityEngine.SceneManagement
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Scene management at run-time.</para>
    /// </summary>
    [RequiredByNativeCode]
    public class SceneManager
    {
        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public static  event UnityAction<Scene, Scene> activeSceneChanged;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public static  event UnityAction<Scene, LoadSceneMode> sceneLoaded;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public static  event UnityAction<Scene> sceneUnloaded;

        /// <summary>
        /// <para>Create an empty new scene at runtime with the given name.</para>
        /// </summary>
        /// <param name="sceneName">The name of the new scene. It cannot be empty or null, or same as the name of the existing scenes.</param>
        /// <returns>
        /// <para>A reference to the new scene that was created, or an invalid scene if creation failed.</para>
        /// </returns>
        public static Scene CreateScene(string sceneName)
        {
            Scene scene;
            INTERNAL_CALL_CreateScene(sceneName, out scene);
            return scene;
        }

        /// <summary>
        /// <para>Gets the currently active scene.</para>
        /// </summary>
        /// <returns>
        /// <para>The active scene.</para>
        /// </returns>
        public static Scene GetActiveScene()
        {
            Scene scene;
            INTERNAL_CALL_GetActiveScene(out scene);
            return scene;
        }

        /// <summary>
        /// <para>Returns an array of all the scenes currently open in the hierarchy.</para>
        /// </summary>
        /// <returns>
        /// <para>Array of Scenes in the Hierarchy.</para>
        /// </returns>
        [Obsolete("Use SceneManager.sceneCount and SceneManager.GetSceneAt(int index) to loop the all scenes instead.")]
        public static Scene[] GetAllScenes()
        {
            Scene[] sceneArray = new Scene[sceneCount];
            for (int i = 0; i < sceneCount; i++)
            {
                sceneArray[i] = GetSceneAt(i);
            }
            return sceneArray;
        }

        /// <summary>
        /// <para>Get the scene at index in the SceneManager's list of added scenes.</para>
        /// </summary>
        /// <param name="index">Index of the scene to get. Index must be greater than or equal to 0 and less than SceneManager.sceneCount.</param>
        /// <returns>
        /// <para>A reference to the scene at the index specified.</para>
        /// </returns>
        public static Scene GetSceneAt(int index)
        {
            Scene scene;
            INTERNAL_CALL_GetSceneAt(index, out scene);
            return scene;
        }

        /// <summary>
        /// <para>Get a scene struct from a build index.</para>
        /// </summary>
        /// <param name="buildIndex">Build index as shown in the Build Settings window.</param>
        /// <returns>
        /// <para>A reference to the scene, if valid. If not, an invalid scene is returned.</para>
        /// </returns>
        public static Scene GetSceneByBuildIndex(int buildIndex)
        {
            Scene scene;
            INTERNAL_CALL_GetSceneByBuildIndex(buildIndex, out scene);
            return scene;
        }

        /// <summary>
        /// <para>Searches through the scenes added to the SceneManager for a scene with the given name.</para>
        /// </summary>
        /// <param name="name">Name of scene to find.</param>
        /// <returns>
        /// <para>A reference to the scene, if valid. If not, an invalid scene is returned.</para>
        /// </returns>
        public static Scene GetSceneByName(string name)
        {
            Scene scene;
            INTERNAL_CALL_GetSceneByName(name, out scene);
            return scene;
        }

        /// <summary>
        /// <para>Searches all scenes added to the SceneManager for a scene that has the given asset path.</para>
        /// </summary>
        /// <param name="scenePath">Path of the scene. Should be relative to the project folder. Like: "AssetsMyScenesMyScene.unity".</param>
        /// <returns>
        /// <para>A reference to the scene, if valid. If not, an invalid scene is returned.</para>
        /// </returns>
        public static Scene GetSceneByPath(string scenePath)
        {
            Scene scene;
            INTERNAL_CALL_GetSceneByPath(scenePath, out scene);
            return scene;
        }

        [RequiredByNativeCode]
        private static void Internal_ActiveSceneChanged(Scene previousActiveScene, Scene newActiveScene)
        {
            if (activeSceneChanged != null)
            {
                activeSceneChanged(previousActiveScene, newActiveScene);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_CreateScene(string sceneName, out Scene value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetActiveScene(out Scene value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetSceneAt(int index, out Scene value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetSceneByBuildIndex(int buildIndex, out Scene value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetSceneByName(string name, out Scene value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetSceneByPath(string scenePath, out Scene value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_MergeScenes(ref Scene sourceScene, ref Scene destinationScene);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_MoveGameObjectToScene(GameObject go, ref Scene scene);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_SetActiveScene(ref Scene scene);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern AsyncOperation INTERNAL_CALL_UnloadSceneAsyncInternal(ref Scene scene);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_UnloadSceneInternal(ref Scene scene);
        [RequiredByNativeCode]
        private static void Internal_SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (sceneLoaded != null)
            {
                sceneLoaded(scene, mode);
            }
        }

        [RequiredByNativeCode]
        private static void Internal_SceneUnloaded(Scene scene)
        {
            if (sceneUnloaded != null)
            {
                sceneUnloaded(scene);
            }
        }

        [ExcludeFromDocs]
        public static void LoadScene(int sceneBuildIndex)
        {
            LoadSceneMode single = LoadSceneMode.Single;
            LoadScene(sceneBuildIndex, single);
        }

        [ExcludeFromDocs]
        public static void LoadScene(string sceneName)
        {
            LoadSceneMode single = LoadSceneMode.Single;
            LoadScene(sceneName, single);
        }

        /// <summary>
        /// <para>Loads the scene by its name or index in Build Settings.</para>
        /// </summary>
        /// <param name="sceneName">Name or path of the scene to load.</param>
        /// <param name="sceneBuildIndex">Index of the scene in the Build Settings to load.</param>
        /// <param name="mode">Allows you to specify whether or not to load the scene additively.
        /// See SceneManagement.LoadSceneMode for more information about the options.</param>
        public static void LoadScene(int sceneBuildIndex, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
        {
            LoadSceneAsyncNameIndexInternal(null, sceneBuildIndex, mode == LoadSceneMode.Additive, true);
        }

        /// <summary>
        /// <para>Loads the scene by its name or index in Build Settings.</para>
        /// </summary>
        /// <param name="sceneName">Name or path of the scene to load.</param>
        /// <param name="sceneBuildIndex">Index of the scene in the Build Settings to load.</param>
        /// <param name="mode">Allows you to specify whether or not to load the scene additively.
        /// See SceneManagement.LoadSceneMode for more information about the options.</param>
        public static void LoadScene(string sceneName, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
        {
            LoadSceneAsyncNameIndexInternal(sceneName, -1, mode == LoadSceneMode.Additive, true);
        }

        [ExcludeFromDocs]
        public static AsyncOperation LoadSceneAsync(int sceneBuildIndex)
        {
            LoadSceneMode single = LoadSceneMode.Single;
            return LoadSceneAsync(sceneBuildIndex, single);
        }

        [ExcludeFromDocs]
        public static AsyncOperation LoadSceneAsync(string sceneName)
        {
            LoadSceneMode single = LoadSceneMode.Single;
            return LoadSceneAsync(sceneName, single);
        }

        /// <summary>
        /// <para>Loads the scene asynchronously in the background.</para>
        /// </summary>
        /// <param name="sceneName">Name or path of the scene to load.</param>
        /// <param name="sceneBuildIndex">Index of the scene in the Build Settings to load.</param>
        /// <param name="mode">If LoadSceneMode.Single then all current scenes will be unloaded before loading.</param>
        /// <returns>
        /// <para>Use the AsyncOperation to determine if the operation has completed.</para>
        /// </returns>
        public static AsyncOperation LoadSceneAsync(int sceneBuildIndex, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode) => 
            LoadSceneAsyncNameIndexInternal(null, sceneBuildIndex, mode == LoadSceneMode.Additive, false);

        /// <summary>
        /// <para>Loads the scene asynchronously in the background.</para>
        /// </summary>
        /// <param name="sceneName">Name or path of the scene to load.</param>
        /// <param name="sceneBuildIndex">Index of the scene in the Build Settings to load.</param>
        /// <param name="mode">If LoadSceneMode.Single then all current scenes will be unloaded before loading.</param>
        /// <returns>
        /// <para>Use the AsyncOperation to determine if the operation has completed.</para>
        /// </returns>
        public static AsyncOperation LoadSceneAsync(string sceneName, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode) => 
            LoadSceneAsyncNameIndexInternal(sceneName, -1, mode == LoadSceneMode.Additive, false);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern AsyncOperation LoadSceneAsyncNameIndexInternal(string sceneName, int sceneBuildIndex, bool isAdditive, bool mustCompleteNextFrame);
        /// <summary>
        /// <para>This will merge the source scene into the destinationScene.</para>
        /// </summary>
        /// <param name="sourceScene">The scene that will be merged into the destination scene.</param>
        /// <param name="destinationScene">Existing scene to merge the source scene into.</param>
        public static void MergeScenes(Scene sourceScene, Scene destinationScene)
        {
            INTERNAL_CALL_MergeScenes(ref sourceScene, ref destinationScene);
        }

        /// <summary>
        /// <para>Move a GameObject from its current scene to a new Scene.</para>
        /// </summary>
        /// <param name="go">GameObject to move.</param>
        /// <param name="scene">Scene to move into.</param>
        public static void MoveGameObjectToScene(GameObject go, Scene scene)
        {
            INTERNAL_CALL_MoveGameObjectToScene(go, ref scene);
        }

        /// <summary>
        /// <para>Set the scene to be active.</para>
        /// </summary>
        /// <param name="scene">The scene to be set.</param>
        /// <returns>
        /// <para>Returns false if the scene is not loaded yet.</para>
        /// </returns>
        public static bool SetActiveScene(Scene scene) => 
            INTERNAL_CALL_SetActiveScene(ref scene);

        /// <summary>
        /// <para>Destroys all GameObjects associated with the given scene and removes the scene from the SceneManager.</para>
        /// </summary>
        /// <param name="sceneBuildIndex">Index of the scene in the Build Settings to unload.</param>
        /// <param name="sceneName">Name or path of the scene to unload.</param>
        /// <param name="scene">Scene to unload.</param>
        /// <returns>
        /// <para>Returns true if the scene is unloaded.</para>
        /// </returns>
        [Obsolete("Use SceneManager.UnloadSceneAsync. This function is not safe to use during triggers and under other circumstances. See Scripting reference for more details.")]
        public static bool UnloadScene(int sceneBuildIndex)
        {
            bool flag;
            UnloadSceneNameIndexInternal("", sceneBuildIndex, true, out flag);
            return flag;
        }

        /// <summary>
        /// <para>Destroys all GameObjects associated with the given scene and removes the scene from the SceneManager.</para>
        /// </summary>
        /// <param name="sceneBuildIndex">Index of the scene in the Build Settings to unload.</param>
        /// <param name="sceneName">Name or path of the scene to unload.</param>
        /// <param name="scene">Scene to unload.</param>
        /// <returns>
        /// <para>Returns true if the scene is unloaded.</para>
        /// </returns>
        [Obsolete("Use SceneManager.UnloadSceneAsync. This function is not safe to use during triggers and under other circumstances. See Scripting reference for more details.")]
        public static bool UnloadScene(string sceneName)
        {
            bool flag;
            UnloadSceneNameIndexInternal(sceneName, -1, true, out flag);
            return flag;
        }

        /// <summary>
        /// <para>Destroys all GameObjects associated with the given scene and removes the scene from the SceneManager.</para>
        /// </summary>
        /// <param name="sceneBuildIndex">Index of the scene in the Build Settings to unload.</param>
        /// <param name="sceneName">Name or path of the scene to unload.</param>
        /// <param name="scene">Scene to unload.</param>
        /// <returns>
        /// <para>Returns true if the scene is unloaded.</para>
        /// </returns>
        [Obsolete("Use SceneManager.UnloadSceneAsync. This function is not safe to use during triggers and under other circumstances. See Scripting reference for more details.")]
        public static bool UnloadScene(Scene scene) => 
            UnloadSceneInternal(scene);

        /// <summary>
        /// <para>Destroys all GameObjects associated with the given scene and removes the scene from the SceneManager.</para>
        /// </summary>
        /// <param name="sceneBuildIndex">Index of the scene in BuildSettings.</param>
        /// <param name="sceneName">Name or path of the scene to unload.</param>
        /// <param name="scene">Scene to unload.</param>
        /// <returns>
        /// <para>Use the AsyncOperation to determine if the operation has completed.</para>
        /// </returns>
        public static AsyncOperation UnloadSceneAsync(int sceneBuildIndex)
        {
            bool flag;
            return UnloadSceneNameIndexInternal("", sceneBuildIndex, false, out flag);
        }

        /// <summary>
        /// <para>Destroys all GameObjects associated with the given scene and removes the scene from the SceneManager.</para>
        /// </summary>
        /// <param name="sceneBuildIndex">Index of the scene in BuildSettings.</param>
        /// <param name="sceneName">Name or path of the scene to unload.</param>
        /// <param name="scene">Scene to unload.</param>
        /// <returns>
        /// <para>Use the AsyncOperation to determine if the operation has completed.</para>
        /// </returns>
        public static AsyncOperation UnloadSceneAsync(string sceneName)
        {
            bool flag;
            return UnloadSceneNameIndexInternal(sceneName, -1, false, out flag);
        }

        /// <summary>
        /// <para>Destroys all GameObjects associated with the given scene and removes the scene from the SceneManager.</para>
        /// </summary>
        /// <param name="sceneBuildIndex">Index of the scene in BuildSettings.</param>
        /// <param name="sceneName">Name or path of the scene to unload.</param>
        /// <param name="scene">Scene to unload.</param>
        /// <returns>
        /// <para>Use the AsyncOperation to determine if the operation has completed.</para>
        /// </returns>
        public static AsyncOperation UnloadSceneAsync(Scene scene) => 
            UnloadSceneAsyncInternal(scene);

        private static AsyncOperation UnloadSceneAsyncInternal(Scene scene) => 
            INTERNAL_CALL_UnloadSceneAsyncInternal(ref scene);

        private static bool UnloadSceneInternal(Scene scene) => 
            INTERNAL_CALL_UnloadSceneInternal(ref scene);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern AsyncOperation UnloadSceneNameIndexInternal(string sceneName, int sceneBuildIndex, bool immediately, out bool outSuccess);

        /// <summary>
        /// <para>The total number of currently loaded scenes.</para>
        /// </summary>
        public static int sceneCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Number of scenes in Build Settings.</para>
        /// </summary>
        public static int sceneCountInBuildSettings { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

