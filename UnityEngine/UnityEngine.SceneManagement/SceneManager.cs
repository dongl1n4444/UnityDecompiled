using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine.SceneManagement
{
	/// <summary>
	///   <para>Scene management at run-time.</para>
	/// </summary>
	public class SceneManager
	{
		/// <summary>
		///   <para>The total number of scenes.</para>
		/// </summary>
		public static extern int sceneCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Number of scenes in Build Settings.</para>
		/// </summary>
		public static extern int sceneCountInBuildSettings
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Get the active scene.</para>
		/// </summary>
		public static Scene GetActiveScene()
		{
			Scene result;
			SceneManager.INTERNAL_CALL_GetActiveScene(out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetActiveScene(out Scene value);

		/// <summary>
		///   <para>Set the scene to be active.</para>
		/// </summary>
		/// <param name="scene">The scene to be set.</param>
		/// <returns>
		///   <para>Returns false if the scene is not loaded yet.</para>
		/// </returns>
		public static bool SetActiveScene(Scene scene)
		{
			return SceneManager.INTERNAL_CALL_SetActiveScene(ref scene);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_SetActiveScene(ref Scene scene);

		/// <summary>
		///   <para>Searches all scenes added to the SceneManager for a scene that has the given asset path.</para>
		/// </summary>
		/// <param name="scenePath">Path of the scene. Should be relative to the project folder. Like: "AssetsMyScenesMyScene.unity".</param>
		public static Scene GetSceneByPath(string scenePath)
		{
			Scene result;
			SceneManager.INTERNAL_CALL_GetSceneByPath(scenePath, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetSceneByPath(string scenePath, out Scene value);

		/// <summary>
		///   <para>Searches through the scenes added to the SceneManager for a scene with the given name.</para>
		/// </summary>
		/// <param name="name">Name of scene to find.</param>
		/// <returns>
		///   <para>The scene if found or an invalid scene if not.</para>
		/// </returns>
		public static Scene GetSceneByName(string name)
		{
			Scene result;
			SceneManager.INTERNAL_CALL_GetSceneByName(name, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetSceneByName(string name, out Scene value);

		/// <summary>
		///   <para>Get the scene at index in the SceneManager's list of added scenes.</para>
		/// </summary>
		/// <param name="index">Index of the scene to get. Index must be greater than or equal to 0 and less than SceneManager.sceneCount.</param>
		public static Scene GetSceneAt(int index)
		{
			Scene result;
			SceneManager.INTERNAL_CALL_GetSceneAt(index, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetSceneAt(int index, out Scene value);

		[Obsolete("Use SceneManager.sceneCount and SceneManager.GetSceneAt(int index) to loop the all scenes instead.")]
		public static Scene[] GetAllScenes()
		{
			Scene[] array = new Scene[SceneManager.sceneCount];
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				array[i] = SceneManager.GetSceneAt(i);
			}
			return array;
		}

		[ExcludeFromDocs]
		public static void LoadScene(string sceneName)
		{
			LoadSceneMode mode = LoadSceneMode.Single;
			SceneManager.LoadScene(sceneName, mode);
		}

		/// <summary>
		///   <para>Loads the scene by its name or index in Build Settings.</para>
		/// </summary>
		/// <param name="sceneName">Name of the scene to load.</param>
		/// <param name="sceneBuildIndex">Index of the scene in the Build Settings to load.</param>
		/// <param name="mode">Allows you to specify whether or not to load the scene additively. See SceneManagement.LoadSceneMode for more information about the options.</param>
		public static void LoadScene(string sceneName, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
		{
			SceneManager.LoadSceneAsyncNameIndexInternal(sceneName, -1, mode == LoadSceneMode.Additive, true);
		}

		[ExcludeFromDocs]
		public static void LoadScene(int sceneBuildIndex)
		{
			LoadSceneMode mode = LoadSceneMode.Single;
			SceneManager.LoadScene(sceneBuildIndex, mode);
		}

		/// <summary>
		///   <para>Loads the scene by its name or index in Build Settings.</para>
		/// </summary>
		/// <param name="sceneName">Name of the scene to load.</param>
		/// <param name="sceneBuildIndex">Index of the scene in the Build Settings to load.</param>
		/// <param name="mode">Allows you to specify whether or not to load the scene additively. See SceneManagement.LoadSceneMode for more information about the options.</param>
		public static void LoadScene(int sceneBuildIndex, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
		{
			SceneManager.LoadSceneAsyncNameIndexInternal(null, sceneBuildIndex, mode == LoadSceneMode.Additive, true);
		}

		[ExcludeFromDocs]
		public static AsyncOperation LoadSceneAsync(string sceneName)
		{
			LoadSceneMode mode = LoadSceneMode.Single;
			return SceneManager.LoadSceneAsync(sceneName, mode);
		}

		/// <summary>
		///   <para>Loads the scene asynchronously in the background.</para>
		/// </summary>
		/// <param name="sceneName">Name of the scene to load.</param>
		/// <param name="sceneBuildIndex">Index of the scene in the Build Settings to load.</param>
		/// <param name="mode">If LoadSceneMode.Single then all current scenes will be unloaded before loading.</param>
		public static AsyncOperation LoadSceneAsync(string sceneName, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
		{
			return SceneManager.LoadSceneAsyncNameIndexInternal(sceneName, -1, mode == LoadSceneMode.Additive, false);
		}

		[ExcludeFromDocs]
		public static AsyncOperation LoadSceneAsync(int sceneBuildIndex)
		{
			LoadSceneMode mode = LoadSceneMode.Single;
			return SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
		}

		/// <summary>
		///   <para>Loads the scene asynchronously in the background.</para>
		/// </summary>
		/// <param name="sceneName">Name of the scene to load.</param>
		/// <param name="sceneBuildIndex">Index of the scene in the Build Settings to load.</param>
		/// <param name="mode">If LoadSceneMode.Single then all current scenes will be unloaded before loading.</param>
		public static AsyncOperation LoadSceneAsync(int sceneBuildIndex, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
		{
			return SceneManager.LoadSceneAsyncNameIndexInternal(null, sceneBuildIndex, mode == LoadSceneMode.Additive, false);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AsyncOperation LoadSceneAsyncNameIndexInternal(string sceneName, int sceneBuildIndex, bool isAdditive, bool mustCompleteNextFrame);

		/// <summary>
		///   <para>Create an empty new scene with the given name additively.</para>
		/// </summary>
		/// <param name="sceneName">The name of the new scene. It cannot be empty or null, or same as the name of the existing scenes.</param>
		public static Scene CreateScene(string sceneName)
		{
			Scene result;
			SceneManager.INTERNAL_CALL_CreateScene(sceneName, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_CreateScene(string sceneName, out Scene value);

		/// <summary>
		///   <para>Unloads all GameObjects associated with the given scene.</para>
		/// </summary>
		/// <param name="sceneBuildIndex">Index of the scene in the Build Settings to unload.</param>
		/// <param name="sceneName">Name of the scene to unload.</param>
		/// <param name="scene">Scene to unload.</param>
		/// <returns>
		///   <para>Returns true if the scene is unloaded.</para>
		/// </returns>
		public static bool UnloadScene(int sceneBuildIndex)
		{
			return SceneManager.UnloadSceneNameIndexInternal(string.Empty, sceneBuildIndex);
		}

		/// <summary>
		///   <para>Unloads all GameObjects associated with the given scene.</para>
		/// </summary>
		/// <param name="sceneBuildIndex">Index of the scene in the Build Settings to unload.</param>
		/// <param name="sceneName">Name of the scene to unload.</param>
		/// <param name="scene">Scene to unload.</param>
		/// <returns>
		///   <para>Returns true if the scene is unloaded.</para>
		/// </returns>
		public static bool UnloadScene(string sceneName)
		{
			return SceneManager.UnloadSceneNameIndexInternal(sceneName, -1);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool UnloadSceneNameIndexInternal(string sceneName, int sceneBuildIndex);

		/// <summary>
		///   <para>This will merge the source scene into the destinationScene.
		/// This function merges the contents of the source scene into the destination scene, and deletes the source scene. All GameObjects at the root of the source scene are moved to the root of the destination scene.
		/// NOTE: This function is destructive: The source scene will be destroyed once the merge has been completed.</para>
		/// </summary>
		/// <param name="sourceScene">The scene that will be merged into the destination scene.</param>
		/// <param name="destinationScene">Existing scene to merge the source scene into.</param>
		public static void MergeScenes(Scene sourceScene, Scene destinationScene)
		{
			SceneManager.INTERNAL_CALL_MergeScenes(ref sourceScene, ref destinationScene);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_MergeScenes(ref Scene sourceScene, ref Scene destinationScene);

		/// <summary>
		///   <para>Move a GameObject from its current scene to a new scene.
		/// It is required that the GameObject is at the root of its current scene.</para>
		/// </summary>
		/// <param name="go">GameObject to move.</param>
		/// <param name="scene">Scene to move into.</param>
		public static void MoveGameObjectToScene(GameObject go, Scene scene)
		{
			SceneManager.INTERNAL_CALL_MoveGameObjectToScene(go, ref scene);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_MoveGameObjectToScene(GameObject go, ref Scene scene);
	}
}
