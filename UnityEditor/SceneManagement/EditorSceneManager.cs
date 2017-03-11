namespace UnityEditor.SceneManagement
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine.Events;
    using UnityEngine.Internal;
    using UnityEngine.SceneManagement;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Scene management in the Editor.</para>
    /// </summary>
    public sealed class EditorSceneManager : SceneManager
    {
        internal static UnityAction<Scene, NewSceneMode> sceneWasCreated;
        internal static UnityAction<Scene, OpenSceneMode> sceneWasOpened;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public static  event NewSceneCreatedCallback newSceneCreated;

        [field: DebuggerBrowsable(0), CompilerGenerated]
        public static  event SceneClosedCallback sceneClosed;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public static  event SceneClosingCallback sceneClosing;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public static  event SceneOpenedCallback sceneOpened;

        [field: DebuggerBrowsable(0), CompilerGenerated]
        public static  event SceneOpeningCallback sceneOpening;

        [field: DebuggerBrowsable(0), CompilerGenerated]
        public static  event SceneSavedCallback sceneSaved;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public static  event SceneSavingCallback sceneSaving;

        /// <summary>
        /// <para>Close the Scene. If removeScene flag is true, the closed Scene will also be removed from EditorSceneManager.</para>
        /// </summary>
        /// <param name="scene">The Scene to be closed/removed.</param>
        /// <param name="removeScene">Bool flag to indicate if the Scene should be removed after closing.</param>
        /// <returns>
        /// <para>Returns true if the Scene is closed/removed.</para>
        /// </returns>
        public static bool CloseScene(Scene scene, bool removeScene) => 
            INTERNAL_CALL_CloseScene(ref scene, removeScene);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool CreateSceneAsset(string scenePath, bool createDefaultGameObjects);
        /// <summary>
        /// <para>Detects cross-scene references in a Scene.</para>
        /// </summary>
        /// <param name="scene">Scene to check for cross-scene references.</param>
        /// <returns>
        /// <para>Was any cross-scene references found.</para>
        /// </returns>
        public static bool DetectCrossSceneReferences(Scene scene) => 
            INTERNAL_CALL_DetectCrossSceneReferences(ref scene);

        /// <summary>
        /// <para>Shows a save dialog if an Untitled scene exists in the current scene manager setup.</para>
        /// </summary>
        /// <param name="dialogContent">Text shown in the save dialog.</param>
        /// <returns>
        /// <para>True if the scene is saved or if there is no Untitled scene.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool EnsureUntitledSceneHasBeenSaved(string dialogContent);
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
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern SceneSetup[] GetSceneManagerSetup();
        internal static Scene GetTargetSceneForNewGameObjects()
        {
            Scene scene;
            INTERNAL_CALL_GetTargetSceneForNewGameObjects(out scene);
            return scene;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_CloseScene(ref Scene scene, bool removeScene);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_DetectCrossSceneReferences(ref Scene scene);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetSceneByHandle(int handle, out Scene value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetTargetSceneForNewGameObjects(out Scene value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_MarkSceneDirty(ref Scene scene);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_MoveSceneAfter(ref Scene src, ref Scene dst);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_MoveSceneBefore(ref Scene src, ref Scene dst);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_NewScene(NewSceneSetup setup, NewSceneMode mode, out Scene value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_OpenScene(string scenePath, OpenSceneMode mode, out Scene value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_ReloadScene(ref Scene scene);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_SaveScene(ref Scene scene, string dstScenePath, bool saveAsCopy);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_SaveSceneAs(ref Scene scene);
        [RequiredByNativeCode]
        private static void Internal_NewSceneCreated(Scene scene, NewSceneSetup setup, NewSceneMode mode)
        {
            if (newSceneCreated != null)
            {
                newSceneCreated(scene, setup, mode);
            }
        }

        private static void Internal_NewSceneWasCreated(Scene scene, NewSceneMode mode)
        {
            if (sceneWasCreated != null)
            {
                sceneWasCreated(scene, mode);
            }
        }

        [RequiredByNativeCode]
        private static void Internal_SceneClosed(Scene scene)
        {
            if (sceneClosed != null)
            {
                sceneClosed(scene);
            }
        }

        [RequiredByNativeCode]
        private static void Internal_SceneClosing(Scene scene, bool removingScene)
        {
            if (sceneClosing != null)
            {
                sceneClosing(scene, removingScene);
            }
        }

        [RequiredByNativeCode]
        private static void Internal_SceneOpened(Scene scene, OpenSceneMode mode)
        {
            if (sceneOpened != null)
            {
                sceneOpened(scene, mode);
            }
        }

        [RequiredByNativeCode]
        private static void Internal_SceneOpening(string path, OpenSceneMode mode)
        {
            if (sceneOpening != null)
            {
                sceneOpening(path, mode);
            }
        }

        [RequiredByNativeCode]
        private static void Internal_SceneSaved(Scene scene)
        {
            if (sceneSaved != null)
            {
                sceneSaved(scene);
            }
        }

        [RequiredByNativeCode]
        private static void Internal_SceneSaving(Scene scene, string path)
        {
            if (sceneSaving != null)
            {
                sceneSaving(scene, path);
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
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
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
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void RestoreSceneManagerSetup(SceneSetup[] value);
        /// <summary>
        /// <para>Asks you if you want to save the modified Scene or Scenes.</para>
        /// </summary>
        /// <returns>
        /// <para>This returns true if you chose to save the Scene or Scenes, and returns false if you pressed Cancel.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool SaveCurrentModifiedScenesIfUserWantsTo();
        /// <summary>
        /// <para>Asks whether the modfied input Scenes should be saved.</para>
        /// </summary>
        /// <param name="scenes">Scenes that should be saved if they are modified.</param>
        /// <returns>
        /// <para>Your choice of whether to save or not save the Scenes.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool SaveModifiedScenesIfUserWantsTo(Scene[] scenes);
        /// <summary>
        /// <para>Save all open Scenes.</para>
        /// </summary>
        /// <returns>
        /// <para>Returns true if all open Scenes are successfully saved.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
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
        /// <param name="dstScenePath">The file path to save the Scene to. If the path is empty, the current open Scene is overwritten. If it has not yet been saved at all, a save dialog is shown.</param>
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
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool SaveScenes(Scene[] scenes);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void SetTargetSceneForNewGameObjects(int sceneHandle);

        /// <summary>
        /// <para>The number of loaded Scenes.</para>
        /// </summary>
        public static int loadedSceneCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Controls whether cross-Scene references are allowed in the Editor.</para>
        /// </summary>
        public static bool preventCrossSceneReferences { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Callbacks of this type which have been added to the newSceneCreated event are called after a new Scene has been created.</para>
        /// </summary>
        /// <param name="scene">The Scene that was created.</param>
        /// <param name="setup">The setup mode used when creating the Scene.</param>
        /// <param name="mode">The mode used for creating the Scene.</param>
        public delegate void NewSceneCreatedCallback(Scene scene, NewSceneSetup setup, NewSceneMode mode);

        /// <summary>
        /// <para>Callbacks of this type which have been added to the sceneClosed event are called immediately after the Scene has been closed.</para>
        /// </summary>
        /// <param name="scene">The Scene that was closed.</param>
        public delegate void SceneClosedCallback(Scene scene);

        /// <summary>
        /// <para>Callbacks of this type which have been added to the sceneClosing event are called just before a Scene is closed.</para>
        /// </summary>
        /// <param name="scene">The Scene that is going to be closed.</param>
        /// <param name="removingScene">Whether or not the Scene is also going to be removed from the Scene Manager after closing. If true the Scene is removed after closing.</param>
        public delegate void SceneClosingCallback(Scene scene, bool removingScene);

        /// <summary>
        /// <para>Callbacks of this type which have been added to the sceneOpened event are called after a Scene has been opened.</para>
        /// </summary>
        /// <param name="scene">The Scene that was opened.</param>
        /// <param name="mode">The mode used to open the Scene.</param>
        public delegate void SceneOpenedCallback(Scene scene, OpenSceneMode mode);

        /// <summary>
        /// <para>Callbacks of this type which have been added to the sceneOpening event are called just before opening a Scene.</para>
        /// </summary>
        /// <param name="path">Path of the Scene to be opened. This is relative to the Project path.</param>
        /// <param name="mode">Mode that is used when opening the Scene.</param>
        public delegate void SceneOpeningCallback(string path, OpenSceneMode mode);

        /// <summary>
        /// <para>Callbacks of this type which have been added to the sceneSaved event are called after a Scene has been saved.</para>
        /// </summary>
        /// <param name="scene">The Scene that was saved.</param>
        public delegate void SceneSavedCallback(Scene scene);

        /// <summary>
        /// <para>Callbacks of this type which have been added to the sceneSaving event are called just before the Scene is saved.</para>
        /// </summary>
        /// <param name="scene">The Scene to be saved.</param>
        /// <param name="path">The path to which the Scene is saved.</param>
        public delegate void SceneSavingCallback(Scene scene, string path);
    }
}

