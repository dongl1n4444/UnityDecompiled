namespace UnityEditor.SceneManagement
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Events;
    using UnityEngine.Internal;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// <para>Scene management in the Editor.</para>
    /// </summary>
    public sealed class EditorSceneManager : SceneManager
    {
        internal static UnityAction<Scene, NewSceneMode> sceneWasCreated;
        internal static UnityAction<Scene, OpenSceneMode> sceneWasOpened;

        /// <summary>
        /// <para>Close the Scene. If removeScene flag is true, the closed Scene is also removed from EditorSceneManager.</para>
        /// </summary>
        /// <param name="scene">The Scene to be closed/removed.</param>
        /// <param name="removeScene">Bool flag to indicate if the Scene should be removed after closing.</param>
        /// <returns>
        /// <para>Returns true if the Scene is closed/removed.</para>
        /// </returns>
        public static bool CloseScene(Scene scene, bool removeScene) => 
            INTERNAL_CALL_CloseScene(ref scene, removeScene);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool CreateSceneAsset(string scenePath, bool createDefaultGameObjects);
        /// <summary>
        /// <para>Checks for cross-Scene references in the specified Scene.</para>
        /// </summary>
        /// <param name="scene">Scene to check for cross-Scene references.</param>
        /// <returns>
        /// <para>Whether any cross-Scene references were found.</para>
        /// </returns>
        public static bool DetectCrossSceneReferences(Scene scene) => 
            INTERNAL_CALL_DetectCrossSceneReferences(ref scene);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool EnsureUntitledSceneHasBeenSaved(string operation);
        internal static Scene GetSceneByHandle(int handle)
        {
            Scene scene;
            INTERNAL_CALL_GetSceneByHandle(handle, out scene);
            return scene;
        }

        /// <summary>
        /// <para>Returns the current setup of the SceneManager.</para>
        /// </summary>
        /// <returns>
        /// <para>An array of SceneSetup classes - one item for each Scene.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern SceneSetup[] GetSceneManagerSetup();
        internal static Scene GetTargetSceneForNewGameObjects()
        {
            Scene scene;
            INTERNAL_CALL_GetTargetSceneForNewGameObjects(out scene);
            return scene;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_CloseScene(ref Scene scene, bool removeScene);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_DetectCrossSceneReferences(ref Scene scene);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetSceneByHandle(int handle, out Scene value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetTargetSceneForNewGameObjects(out Scene value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_MarkSceneDirty(ref Scene scene);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_MoveSceneAfter(ref Scene src, ref Scene dst);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_MoveSceneBefore(ref Scene src, ref Scene dst);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_NewScene(NewSceneSetup setup, NewSceneMode mode, out Scene value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_OpenScene(string scenePath, OpenSceneMode mode, out Scene value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_ReloadScene(ref Scene scene);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_SaveScene(ref Scene scene, string dstScenePath, bool saveAsCopy);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_SaveSceneAs(ref Scene scene);
        private static void Internal_NewSceneWasCreated(Scene scene, NewSceneMode mode)
        {
            if (sceneWasCreated != null)
            {
                sceneWasCreated(scene, mode);
            }
        }

        private static void Internal_SceneWasOpened(Scene scene, OpenSceneMode mode)
        {
            if (sceneWasOpened != null)
            {
                sceneWasOpened(scene, mode);
            }
        }

        /// <summary>
        /// <para>Mark all the loaded Scenes as modified.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void MarkAllScenesDirty();
        /// <summary>
        /// <para>Mark the specified Scene as modified.</para>
        /// </summary>
        /// <param name="scene">The Scene to be marked as modified.</param>
        /// <returns>
        /// <para>Whether the Scene was successfully marked as dirty.</para>
        /// </returns>
        public static bool MarkSceneDirty(Scene scene) => 
            INTERNAL_CALL_MarkSceneDirty(ref scene);

        /// <summary>
        /// <para>Allows you to reorder the Scenes currently open in the Hierarchy window. Moves the source Scene so it comes after the destination Scene.</para>
        /// </summary>
        /// <param name="src">The Scene to move.</param>
        /// <param name="dst">The Scene which should come directly before the source Scene in the hierarchy.</param>
        public static void MoveSceneAfter(Scene src, Scene dst)
        {
            INTERNAL_CALL_MoveSceneAfter(ref src, ref dst);
        }

        /// <summary>
        /// <para>Allows you to reorder the Scenes currently open in the Hierarchy window. Moves the source Scene so it comes before the destination Scene.</para>
        /// </summary>
        /// <param name="src">The Scene to move.</param>
        /// <param name="dst">The Scene which should come directly after the source Scene in the hierarchy.</param>
        public static void MoveSceneBefore(Scene src, Scene dst)
        {
            INTERNAL_CALL_MoveSceneBefore(ref src, ref dst);
        }

        [ExcludeFromDocs]
        public static Scene NewScene(NewSceneSetup setup)
        {
            Scene scene;
            NewSceneMode single = NewSceneMode.Single;
            INTERNAL_CALL_NewScene(setup, single, out scene);
            return scene;
        }

        /// <summary>
        /// <para>Create a new Scene.</para>
        /// </summary>
        /// <param name="setup">Whether the new Scene should use the default set of GameObjects.</param>
        /// <param name="mode">Whether to keep existing Scenes open.</param>
        /// <returns>
        /// <para>A reference to the new Scene.</para>
        /// </returns>
        public static Scene NewScene(NewSceneSetup setup, [DefaultValue("NewSceneMode.Single")] NewSceneMode mode)
        {
            Scene scene;
            INTERNAL_CALL_NewScene(setup, mode, out scene);
            return scene;
        }

        [ExcludeFromDocs]
        public static Scene OpenScene(string scenePath)
        {
            Scene scene;
            OpenSceneMode single = OpenSceneMode.Single;
            INTERNAL_CALL_OpenScene(scenePath, single, out scene);
            return scene;
        }

        /// <summary>
        /// <para>Open a Scene in the Editor.</para>
        /// </summary>
        /// <param name="scenePath">The path of the Scene. This should be relative to the Project folder; for example, "AssetsMyScenesMyScene.unity".</param>
        /// <param name="mode">Allows you to select how to open the specified Scene, and whether to keep existing Scenes in the Hierarchy. See SceneManagement.OpenSceneMode for more information about the options.</param>
        /// <returns>
        /// <para>A reference to the opened Scene.</para>
        /// </returns>
        public static Scene OpenScene(string scenePath, [DefaultValue("OpenSceneMode.Single")] OpenSceneMode mode)
        {
            Scene scene;
            INTERNAL_CALL_OpenScene(scenePath, mode, out scene);
            return scene;
        }

        internal static bool ReloadScene(Scene scene) => 
            INTERNAL_CALL_ReloadScene(ref scene);

        /// <summary>
        /// <para>Restore the setup of the SceneManager.</para>
        /// </summary>
        /// <param name="value">In this array, at least one Scene should be loaded, and there must be one active Scene.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void RestoreSceneManagerSetup(SceneSetup[] value);
        /// <summary>
        /// <para>Asks you if you want to save the modified Scene or Scenes.</para>
        /// </summary>
        /// <returns>
        /// <para>This returns true if you chose to save the Scene or Scenes, and returns false if you pressed Cancel.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool SaveCurrentModifiedScenesIfUserWantsTo();
        /// <summary>
        /// <para>Asks whether the modfied input Scenes should be saved.</para>
        /// </summary>
        /// <param name="scenes">Scenes that should be saved if they are modified.</param>
        /// <returns>
        /// <para>Your choice of whether to save or not save the Scenes.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool SaveModifiedScenesIfUserWantsTo(Scene[] scenes);
        /// <summary>
        /// <para>Save all open Scenes.</para>
        /// </summary>
        /// <returns>
        /// <para>Returns true if all open Scenes are successfully saved.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool SaveOpenScenes();
        [ExcludeFromDocs]
        public static bool SaveScene(Scene scene)
        {
            bool saveAsCopy = false;
            string dstScenePath = "";
            return INTERNAL_CALL_SaveScene(ref scene, dstScenePath, saveAsCopy);
        }

        [ExcludeFromDocs]
        public static bool SaveScene(Scene scene, string dstScenePath)
        {
            bool saveAsCopy = false;
            return INTERNAL_CALL_SaveScene(ref scene, dstScenePath, saveAsCopy);
        }

        /// <summary>
        /// <para>Save a Scene.</para>
        /// </summary>
        /// <param name="scene">The Scene to be saved.</param>
        /// <param name="dstScenePath">The file path to save the Scene to. If the path is not empty, the current open Scene is overwritten. If it has not yet been saved at all, a save dialog is shown.</param>
        /// <param name="saveAsCopy">If set to true, the Scene is saved without changing the current Scene, and without clearing the unsaved changes marker.</param>
        /// <returns>
        /// <para>True if the save succeeded, otherwise false.</para>
        /// </returns>
        public static bool SaveScene(Scene scene, [DefaultValue("\"\"")] string dstScenePath, [DefaultValue("false")] bool saveAsCopy) => 
            INTERNAL_CALL_SaveScene(ref scene, dstScenePath, saveAsCopy);

        internal static bool SaveSceneAs(Scene scene) => 
            INTERNAL_CALL_SaveSceneAs(ref scene);

        /// <summary>
        /// <para>Save a list of Scenes.</para>
        /// </summary>
        /// <param name="scenes">List of Scenes that should be saved.</param>
        /// <returns>
        /// <para>True if the save succeeded. Otherwise false.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool SaveScenes(Scene[] scenes);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetTargetSceneForNewGameObjects(int sceneHandle);

        /// <summary>
        /// <para>The number of loaded Scenes.</para>
        /// </summary>
        public static int loadedSceneCount { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Controls whether cross-Scene references are allowed in the Editor.</para>
        /// </summary>
        public static bool preventCrossSceneReferences { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

