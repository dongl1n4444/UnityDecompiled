namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SceneManagement;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Main Application class.</para>
    /// </summary>
    public sealed class EditorApplication
    {
        [CompilerGenerated]
        private static CallbackFunction <>f__mg$cache0;
        [CompilerGenerated]
        private static CallbackFunction <>f__mg$cache1;
        internal static CallbackFunction assetBundleNameChanged;
        internal static CallbackFunction assetLabelsChanged;
        /// <summary>
        /// <para>Callback raised whenever the user contex-clicks on a property in an Inspector.</para>
        /// </summary>
        public static SerializedPropertyCallbackFunction contextualPropertyMenu;
        /// <summary>
        /// <para>Delegate which is called once after all inspectors update.</para>
        /// </summary>
        public static CallbackFunction delayCall;
        private static CallbackFunction delayedCallback;
        internal static UnityAction editorApplicationQuit;
        internal static CallbackFunction globalEventHandler;
        /// <summary>
        /// <para>A callback to be raised when an object in the hierarchy changes.
        /// 
        /// Each time an object is (or a group of objects are) created, renamed, parented, unparented or destroyed this callback is raised.
        /// </para>
        /// </summary>
        public static CallbackFunction hierarchyWindowChanged;
        /// <summary>
        /// <para>Delegate for OnGUI events for every visible list item in the HierarchyWindow.</para>
        /// </summary>
        public static HierarchyWindowItemCallback hierarchyWindowItemOnGUI;
        /// <summary>
        /// <para>Delegate for changed keyboard modifier keys.</para>
        /// </summary>
        public static CallbackFunction modifierKeysChanged;
        /// <summary>
        /// <para>Delegate for play mode state changes.</para>
        /// </summary>
        public static CallbackFunction playmodeStateChanged;
        internal static UnityAction projectWasLoaded;
        /// <summary>
        /// <para>Callback raised whenever the state of the Project window changes.</para>
        /// </summary>
        public static CallbackFunction projectWindowChanged;
        /// <summary>
        /// <para>Delegate for OnGUI events for every visible list item in the ProjectWindow.</para>
        /// </summary>
        public static ProjectWindowItemCallback projectWindowItemOnGUI;
        private static float s_DelayedCallbackTime = 0f;
        /// <summary>
        /// <para>Callback raised whenever the contents of a window's search box are changed.</para>
        /// </summary>
        public static CallbackFunction searchChanged;
        /// <summary>
        /// <para>Delegate for generic updates.</para>
        /// </summary>
        public static CallbackFunction update;
        internal static CallbackFunction windowsReordered;

        /// <summary>
        /// <para>Plays system beep sound.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void Beep();
        internal static void CallDelayed(CallbackFunction function, float timeFromNow)
        {
            delayedCallback = function;
            s_DelayedCallbackTime = Time.realtimeSinceStartup + timeFromNow;
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new CallbackFunction(EditorApplication.CheckCallDelayed);
            }
            update = (CallbackFunction) Delegate.Combine(update, <>f__mg$cache0);
        }

        private static void CheckCallDelayed()
        {
            if (Time.realtimeSinceStartup > s_DelayedCallbackTime)
            {
                if (<>f__mg$cache1 == null)
                {
                    <>f__mg$cache1 = new CallbackFunction(EditorApplication.CheckCallDelayed);
                }
                update = (CallbackFunction) Delegate.Remove(update, <>f__mg$cache1);
                delayedCallback();
            }
        }

        /// <summary>
        /// <para>Set the hierarchy sorting method as dirty.</para>
        /// </summary>
        public static void DirtyHierarchyWindowSorting()
        {
            foreach (SceneHierarchyWindow window in UnityEngine.Resources.FindObjectsOfTypeAll(typeof(SceneHierarchyWindow)))
            {
                window.DirtySortingMethods();
            }
        }

        /// <summary>
        /// <para>Invokes the menu item in the specified path.</para>
        /// </summary>
        /// <param name="menuItemPath"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool ExecuteMenuItem(string menuItemPath);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool ExecuteMenuItemOnGameObjects(string menuItemPath, GameObject[] objects);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool ExecuteMenuItemWithTemporaryContext(string menuItemPath, UnityEngine.Object[] objects);
        /// <summary>
        /// <para>Exit the Unity editor application.</para>
        /// </summary>
        /// <param name="returnValue"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void Exit(int returnValue);
        internal static void Internal_CallAssetBundleNameChanged()
        {
            if (assetBundleNameChanged != null)
            {
                assetBundleNameChanged();
            }
        }

        internal static void Internal_CallAssetLabelsHaveChanged()
        {
            if (assetLabelsChanged != null)
            {
                assetLabelsChanged();
            }
        }

        private static void Internal_CallDelayFunctions()
        {
            CallbackFunction delayCall = EditorApplication.delayCall;
            EditorApplication.delayCall = null;
            if (delayCall != null)
            {
                delayCall();
            }
        }

        [RequiredByNativeCode]
        private static void Internal_CallGlobalEventHandler()
        {
            if (globalEventHandler != null)
            {
                globalEventHandler();
            }
            WindowLayout.MaximizeKeyHandler();
            Event.current = null;
        }

        private static void Internal_CallHierarchyWindowHasChanged()
        {
            if (hierarchyWindowChanged != null)
            {
                hierarchyWindowChanged();
            }
        }

        private static void Internal_CallKeyboardModifiersChanged()
        {
            if (modifierKeysChanged != null)
            {
                modifierKeysChanged();
            }
        }

        private static void Internal_CallProjectWindowHasChanged()
        {
            if (projectWindowChanged != null)
            {
                projectWindowChanged();
            }
        }

        internal static void Internal_CallSearchHasChanged()
        {
            if (searchChanged != null)
            {
                searchChanged();
            }
        }

        private static void Internal_CallUpdateFunctions()
        {
            if (update != null)
            {
                update();
            }
        }

        private static void Internal_CallWindowsReordered()
        {
            if (windowsReordered != null)
            {
                windowsReordered();
            }
        }

        private static void Internal_EditorApplicationQuit()
        {
            if (editorApplicationQuit != null)
            {
                editorApplicationQuit();
            }
        }

        private static void Internal_PlaymodeStateChanged()
        {
            if (playmodeStateChanged != null)
            {
                playmodeStateChanged();
            }
        }

        private static void Internal_ProjectWasLoaded()
        {
            if (projectWasLoaded != null)
            {
                projectWasLoaded();
            }
        }

        private static void Internal_SwitchSkin()
        {
            EditorGUIUtility.Internal_SwitchSkin();
        }

        /// <summary>
        /// <para>Load the given level additively in play mode asynchronously</para>
        /// </summary>
        /// <param name="path"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern AsyncOperation LoadLevelAdditiveAsyncInPlayMode(string path);
        /// <summary>
        /// <para>Load the given level additively in play mode.</para>
        /// </summary>
        /// <param name="path"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void LoadLevelAdditiveInPlayMode(string path);
        /// <summary>
        /// <para>Load the given level in play mode asynchronously.</para>
        /// </summary>
        /// <param name="path"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern AsyncOperation LoadLevelAsyncInPlayMode(string path);
        /// <summary>
        /// <para>Load the given level in play mode.</para>
        /// </summary>
        /// <param name="path"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void LoadLevelInPlayMode(string path);
        /// <summary>
        /// <para>Prevents loading of assemblies when it is inconvenient.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void LockReloadAssemblies();
        /// <summary>
        /// <para>Explicitly mark the current opened scene as modified.</para>
        /// </summary>
        [Obsolete("Use EditorSceneManager.MarkSceneDirty or EditorSceneManager.MarkAllScenesDirty")]
        public static void MarkSceneDirty()
        {
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        /// <summary>
        /// <para>Create a new absolutely empty scene.</para>
        /// </summary>
        [Obsolete("Use EditorSceneManager.NewScene (NewSceneSetup.EmptyScene)")]
        public static void NewEmptyScene()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
        }

        /// <summary>
        /// <para>Create a new scene.</para>
        /// </summary>
        [Obsolete("Use EditorSceneManager.NewScene (NewSceneSetup.DefaultGameObjects)")]
        public static void NewScene()
        {
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
        }

        /// <summary>
        /// <para>Open another project.</para>
        /// </summary>
        /// <param name="projectPath">The path of a project to open.</param>
        /// <param name="args">Arguments to pass to command line.</param>
        public static void OpenProject(string projectPath, params string[] args)
        {
            OpenProjectInternal(projectPath, args);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void OpenProjectInternal(string projectPath, string[] args);
        /// <summary>
        /// <para>Opens the scene at path.</para>
        /// </summary>
        /// <param name="path"></param>
        [Obsolete("Use EditorSceneManager.OpenScene")]
        public static bool OpenScene(string path)
        {
            if (isPlaying)
            {
                throw new InvalidOperationException("EditorApplication.OpenScene() cannot be called when in the Unity Editor is in play mode.");
            }
            return EditorSceneManager.OpenScene(path).IsValid();
        }

        /// <summary>
        /// <para>Opens the scene at path additively.</para>
        /// </summary>
        /// <param name="path"></param>
        [Obsolete("Use EditorSceneManager.OpenScene")]
        public static void OpenSceneAdditive(string path)
        {
            if (Application.isPlaying)
            {
                Debug.LogWarning("Exiting playmode.\nOpenSceneAdditive was called at a point where there was no active scene.\nThis usually means it was called in a PostprocessScene function during scene loading or it was called during playmode.\nThis is no longer allowed. Use SceneManager.LoadScene to load scenes at runtime or in playmode.");
            }
            Scene sourceScene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            Scene activeScene = SceneManager.GetActiveScene();
            SceneManager.MergeScenes(sourceScene, activeScene);
        }

        public static void RepaintAnimationWindow()
        {
            foreach (AnimEditor editor in AnimEditor.GetAllAnimationWindows())
            {
                editor.Repaint();
            }
        }

        /// <summary>
        /// <para>Can be used to ensure repaint of the HierarchyWindow.</para>
        /// </summary>
        public static void RepaintHierarchyWindow()
        {
            foreach (SceneHierarchyWindow window in UnityEngine.Resources.FindObjectsOfTypeAll(typeof(SceneHierarchyWindow)))
            {
                window.Repaint();
            }
        }

        /// <summary>
        /// <para>Can be used to ensure repaint of the ProjectWindow.</para>
        /// </summary>
        public static void RepaintProjectWindow()
        {
            foreach (ProjectBrowser browser in ProjectBrowser.GetAllProjectBrowsers())
            {
                browser.Repaint();
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void ReportUNetWeaver(string filename, string msg, bool isError);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void RequestRepaintAllViews();
        /// <summary>
        /// <para>Saves all serializable assets that have not yet been written to disk (eg. Materials).</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator, Obsolete("Use AssetDatabase.SaveAssets instead (UnityUpgradable) -> AssetDatabase.SaveAssets()", true)]
        public static extern void SaveAssets();
        /// <summary>
        /// <para>Ask the user if he wants to save the open scene.</para>
        /// </summary>
        [Obsolete("Use EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo")]
        public static bool SaveCurrentSceneIfUserWantsTo() => 
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        [Obsolete("This function is internal and no longer supported")]
        internal static bool SaveCurrentSceneIfUserWantsToForce() => 
            false;

        /// <summary>
        /// <para>Save the open scene.</para>
        /// </summary>
        /// <param name="path">The file path to save at. If empty, the current open scene will be overwritten, or if never saved before, a save dialog is shown.</param>
        /// <param name="saveAsCopy">If set to true, the scene will be saved without changing the currentScene and without clearing the unsaved changes marker.</param>
        /// <returns>
        /// <para>True if the save succeeded, otherwise false.</para>
        /// </returns>
        [Obsolete("Use EditorSceneManager.SaveScene")]
        public static bool SaveScene() => 
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "", false);

        /// <summary>
        /// <para>Save the open scene.</para>
        /// </summary>
        /// <param name="path">The file path to save at. If empty, the current open scene will be overwritten, or if never saved before, a save dialog is shown.</param>
        /// <param name="saveAsCopy">If set to true, the scene will be saved without changing the currentScene and without clearing the unsaved changes marker.</param>
        /// <returns>
        /// <para>True if the save succeeded, otherwise false.</para>
        /// </returns>
        [Obsolete("Use EditorSceneManager.SaveScene")]
        public static bool SaveScene(string path) => 
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), path, false);

        /// <summary>
        /// <para>Save the open scene.</para>
        /// </summary>
        /// <param name="path">The file path to save at. If empty, the current open scene will be overwritten, or if never saved before, a save dialog is shown.</param>
        /// <param name="saveAsCopy">If set to true, the scene will be saved without changing the currentScene and without clearing the unsaved changes marker.</param>
        /// <returns>
        /// <para>True if the save succeeded, otherwise false.</para>
        /// </returns>
        [Obsolete("Use EditorSceneManager.SaveScene")]
        public static bool SaveScene(string path, bool saveAsCopy) => 
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), path, saveAsCopy);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void SetSceneRepaintDirty();
        /// <summary>
        /// <para>Sets the path that Unity should store the current temporary project at, when the project is closed.</para>
        /// </summary>
        /// <param name="path">The path that the current temporary project should be relocated to when closing it.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetTemporaryProjectKeepPath(string path);
        /// <summary>
        /// <para>Perform a single frame step.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void Step();
        /// <summary>
        /// <para>Must be called after LockReloadAssemblies, to reenable loading of assemblies.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void UnlockReloadAssemblies();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void UpdateSceneIfNeeded();

        /// <summary>
        /// <para>Path to the Unity editor contents folder. (Read Only)</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public static string applicationContentsPath { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Returns the path to the Unity editor application. (Read Only)</para>
        /// </summary>
        public static string applicationPath { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The path of the scene that the user has currently open (Will be an empty string if no scene is currently open). (Read Only)</para>
        /// </summary>
        [Obsolete("Use EditorSceneManager to see which scenes are currently loaded")]
        public static string currentScene
        {
            get
            {
                Scene activeScene = SceneManager.GetActiveScene();
                if (activeScene.IsValid())
                {
                    return activeScene.path;
                }
                return "";
            }
            set
            {
            }
        }

        /// <summary>
        /// <para>Is editor currently compiling scripts? (Read Only)</para>
        /// </summary>
        public static bool isCompiling { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Is editor currently paused?</para>
        /// </summary>
        public static bool isPaused { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Is editor currently in play mode?</para>
        /// </summary>
        public static bool isPlaying { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Is editor either currently in play mode, or about to switch to it? (Read Only)</para>
        /// </summary>
        public static bool isPlayingOrWillChangePlaymode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Is editor currently connected to Unity Remote 4 client app.</para>
        /// </summary>
        public static bool isRemoteConnected { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Is true if the currently open scene in the editor contains unsaved modifications.</para>
        /// </summary>
        [Obsolete("Use Scene.isDirty instead. Use EditorSceneManager.GetScene API to get each open scene")]
        public static bool isSceneDirty =>
            SceneManager.GetActiveScene().isDirty;

        /// <summary>
        /// <para>Returns true if the current project was created as a temporary project.</para>
        /// </summary>
        public static bool isTemporaryProject { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Is editor currently updating? (Read Only)</para>
        /// </summary>
        public static bool isUpdating { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        internal static UnityEngine.Object renderSettings { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        internal static bool supportsHiDPI =>
            (Application.platform == RuntimePlatform.OSXEditor);

        internal static UnityEngine.Object tagManager { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The time since the editor was started. (Read Only)</para>
        /// </summary>
        public static double timeSinceStartup { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        internal static string userJavascriptPackagesPath { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Delegate to be called from EditorApplication callbacks.</para>
        /// </summary>
        public delegate void CallbackFunction();

        /// <summary>
        /// <para>Delegate to be called for every visible list item in the HierarchyWindow on every OnGUI event.</para>
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="selectionRect"></param>
        public delegate void HierarchyWindowItemCallback(int instanceID, Rect selectionRect);

        /// <summary>
        /// <para>Delegate to be called for every visible list item in the ProjectWindow on every OnGUI event.</para>
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="selectionRect"></param>
        public delegate void ProjectWindowItemCallback(string guid, Rect selectionRect);

        /// <summary>
        /// <para>Delegate to be called from EditorApplication contextual inspector callbacks.</para>
        /// </summary>
        /// <param name="menu">The contextual menu which is about to be shown to the user.</param>
        /// <param name="property">The property for which the contextual menu is shown.</param>
        public delegate void SerializedPropertyCallbackFunction(GenericMenu menu, SerializedProperty property);
    }
}

