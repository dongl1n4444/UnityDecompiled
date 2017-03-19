namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.Scripting.Compilers;
    using UnityEngine;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Editor utility functions.</para>
    /// </summary>
    public sealed class EditorUtility
    {
        [Obsolete("Use BuildPipeline.BuildAssetBundle instead")]
        public static bool BuildResourceFile(UnityEngine.Object[] selection, string pathName) => 
            BuildPipeline.BuildAssetBundle(null, selection, pathName, BuildAssetBundleOptions.CompleteAssets);

        /// <summary>
        /// <para>Removes progress bar.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void ClearProgressBar();
        /// <summary>
        /// <para>Collect all objects in the hierarchy rooted at each of the given objects.</para>
        /// </summary>
        /// <param name="roots">Array of objects where the search will start.</param>
        /// <returns>
        /// <para>Array of objects heirarchically attached to the search array.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern UnityEngine.Object[] CollectDeepHierarchy(UnityEngine.Object[] roots);
        /// <summary>
        /// <para>Calculates and returns a list of all assets the assets listed in roots depend on.</para>
        /// </summary>
        /// <param name="roots"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern UnityEngine.Object[] CollectDependencies(UnityEngine.Object[] roots);
        public static string[] CompileCSharp(string[] sources, string[] references, string[] defines, string outputFile) => 
            MonoCSharpCompiler.Compile(sources, references, defines, outputFile);

        private static void CompressTexture(Texture2D texture, TextureFormat format)
        {
            CompressTexture(texture, format, TextureCompressionQuality.Normal);
        }

        /// <summary>
        /// <para>Compress a texture.</para>
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="format"></param>
        /// <param name="quality"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void CompressTexture(Texture2D texture, TextureFormat format, int quality);
        /// <summary>
        /// <para>Compress a texture.</para>
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="format"></param>
        /// <param name="quality"></param>
        public static void CompressTexture(Texture2D texture, TextureFormat format, TextureCompressionQuality quality)
        {
            CompressTexture(texture, format, (int) quality);
        }

        /// <summary>
        /// <para>Copy all settings of a Unity Object.</para>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void CopySerialized(UnityEngine.Object source, UnityEngine.Object dest);
        /// <summary>
        /// <para>Copy all settings of a Unity Object to a second Object if they differ.</para>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        public static void CopySerializedIfDifferent(UnityEngine.Object source, UnityEngine.Object dest)
        {
            if (source == null)
            {
                throw new ArgumentNullException("Argument 'source' is null");
            }
            if (dest == null)
            {
                throw new ArgumentNullException("Argument 'dest' is null");
            }
            InternalCopySerializedIfDifferent(source, dest);
        }

        [Obsolete("Use PrefabUtility.CreateEmptyPrefab")]
        public static UnityEngine.Object CreateEmptyPrefab(string path) => 
            PrefabUtility.CreateEmptyPrefab(path);

        /// <summary>
        /// <para>Creates a game object with HideFlags and specified components.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="flags"></param>
        /// <param name="components"></param>
        public static GameObject CreateGameObjectWithHideFlags(string name, HideFlags flags, params System.Type[] components)
        {
            GameObject obj2 = Internal_CreateGameObjectWithHideFlags(name, flags);
            obj2.AddComponent(typeof(Transform));
            foreach (System.Type type in components)
            {
                obj2.AddComponent(type);
            }
            return obj2;
        }

        /// <summary>
        /// <para>Displays or updates a progress bar that has a cancel button.</para>
        /// </summary>
        /// <param name="title"></param>
        /// <param name="info"></param>
        /// <param name="progress"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool DisplayCancelableProgressBar(string title, string info, float progress);
        internal static void DisplayCustomMenu(Rect position, string[] options, int[] selected, SelectMenuItemFunction callback, object userData)
        {
            DisplayCustomMenu(position, options, selected, callback, userData, false);
        }

        public static void DisplayCustomMenu(Rect position, GUIContent[] options, int selected, SelectMenuItemFunction callback, object userData)
        {
            DisplayCustomMenu(position, options, selected, callback, userData, false);
        }

        internal static void DisplayCustomMenu(Rect position, string[] options, bool[] enabled, int[] selected, SelectMenuItemFunction callback, object userData)
        {
            DisplayCustomMenu(position, options, enabled, selected, callback, userData, false);
        }

        internal static void DisplayCustomMenu(Rect position, string[] options, int[] selected, SelectMenuItemFunction callback, object userData, bool showHotkey)
        {
            bool[] separator = new bool[options.Length];
            DisplayCustomMenuWithSeparators(position, options, separator, selected, callback, userData, showHotkey);
        }

        public static void DisplayCustomMenu(Rect position, GUIContent[] options, int selected, SelectMenuItemFunction callback, object userData, bool showHotkey)
        {
            int[] numArray = new int[] { selected };
            string[] strArray = new string[options.Length];
            for (int i = 0; i < options.Length; i++)
            {
                strArray[i] = options[i].text;
            }
            DisplayCustomMenu(position, strArray, numArray, callback, userData, showHotkey);
        }

        internal static void DisplayCustomMenu(Rect position, string[] options, bool[] enabled, int[] selected, SelectMenuItemFunction callback, object userData, bool showHotkey)
        {
            bool[] separator = new bool[options.Length];
            DisplayCustomMenuWithSeparators(position, options, enabled, separator, selected, callback, userData, showHotkey);
        }

        internal static void DisplayCustomMenuWithSeparators(Rect position, string[] options, bool[] separator, int[] selected, SelectMenuItemFunction callback, object userData)
        {
            DisplayCustomMenuWithSeparators(position, options, separator, selected, callback, userData, false);
        }

        internal static void DisplayCustomMenuWithSeparators(Rect position, string[] options, bool[] enabled, bool[] separator, int[] selected, SelectMenuItemFunction callback, object userData)
        {
            DisplayCustomMenuWithSeparators(position, options, enabled, separator, selected, callback, userData, false);
        }

        internal static void DisplayCustomMenuWithSeparators(Rect position, string[] options, bool[] separator, int[] selected, SelectMenuItemFunction callback, object userData, bool showHotkey)
        {
            Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(position.x, position.y));
            position.x = vector.x;
            position.y = vector.y;
            int[] enabled = new int[options.Length];
            int[] numArray2 = new int[options.Length];
            for (int i = 0; i < options.Length; i++)
            {
                enabled[i] = 1;
                numArray2[i] = 0;
            }
            Internal_DisplayCustomMenu(position, options, enabled, numArray2, selected, callback, userData, showHotkey);
            ResetMouseDown();
        }

        internal static void DisplayCustomMenuWithSeparators(Rect position, string[] options, bool[] enabled, bool[] separator, int[] selected, SelectMenuItemFunction callback, object userData, bool showHotkey)
        {
            Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(position.x, position.y));
            position.x = vector.x;
            position.y = vector.y;
            int[] numArray = new int[options.Length];
            int[] numArray2 = new int[options.Length];
            for (int i = 0; i < options.Length; i++)
            {
                numArray[i] = !enabled[i] ? 0 : 1;
                numArray2[i] = !separator[i] ? 0 : 1;
            }
            Internal_DisplayCustomMenu(position, options, numArray, numArray2, selected, callback, userData, showHotkey);
            ResetMouseDown();
        }

        /// <summary>
        /// <para>Displays a modal dialog.</para>
        /// </summary>
        /// <param name="title">The title of the message box.</param>
        /// <param name="message">The text of the message.</param>
        /// <param name="ok">Label displayed on the OK dialog button.</param>
        /// <param name="cancel">Label displayed on the Cancel dialog button.</param>
        [ExcludeFromDocs]
        public static bool DisplayDialog(string title, string message, string ok)
        {
            string cancel = "";
            return DisplayDialog(title, message, ok, cancel);
        }

        /// <summary>
        /// <para>Displays a modal dialog.</para>
        /// </summary>
        /// <param name="title">The title of the message box.</param>
        /// <param name="message">The text of the message.</param>
        /// <param name="ok">Label displayed on the OK dialog button.</param>
        /// <param name="cancel">Label displayed on the Cancel dialog button.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool DisplayDialog(string title, string message, string ok, [DefaultValue("\"\"")] string cancel);
        /// <summary>
        /// <para>Displays a modal dialog with three buttons.</para>
        /// </summary>
        /// <param name="title">Title for dialog.</param>
        /// <param name="message">Purpose for the dialog.</param>
        /// <param name="ok">Dialog function chosen.</param>
        /// <param name="alt">Choose alternative dialog purpose.</param>
        /// <param name="cancel">Close dialog with no operation.</param>
        /// <returns>
        /// <para>The id of the chosen button.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern int DisplayDialogComplex(string title, string message, string ok, string cancel, string alt);
        internal static void DisplayObjectContextMenu(Rect position, UnityEngine.Object context, int contextUserData)
        {
            UnityEngine.Object[] objArray1 = new UnityEngine.Object[] { context };
            DisplayObjectContextMenu(position, objArray1, contextUserData);
        }

        internal static void DisplayObjectContextMenu(Rect position, UnityEngine.Object[] context, int contextUserData)
        {
            Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(position.x, position.y));
            position.x = vector.x;
            position.y = vector.y;
            Internal_DisplayObjectContextMenu(position, context, contextUserData);
            ResetMouseDown();
        }

        /// <summary>
        /// <para>Displays a popup menu.</para>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="menuItemPath"></param>
        /// <param name="command"></param>
        public static void DisplayPopupMenu(Rect position, string menuItemPath, MenuCommand command)
        {
            if (((menuItemPath == "CONTEXT") || (menuItemPath == "CONTEXT/")) || (menuItemPath == @"CONTEXT\"))
            {
                bool flag = false;
                if (command == null)
                {
                    flag = true;
                }
                if ((command != null) && (command.context == null))
                {
                    flag = true;
                }
                if (flag)
                {
                    Debug.LogError("DisplayPopupMenu: invalid arguments: using CONTEXT requires a valid MenuCommand object. If you want a custom context menu then try using the GenericMenu.");
                    return;
                }
            }
            Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(position.x, position.y));
            position.x = vector.x;
            position.y = vector.y;
            Internal_DisplayPopupMenu(position, menuItemPath, (command != null) ? command.context : null, (command != null) ? command.userData : 0);
            ResetMouseDown();
        }

        /// <summary>
        /// <para>Displays or updates a progress bar.</para>
        /// </summary>
        /// <param name="title"></param>
        /// <param name="info"></param>
        /// <param name="progress"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void DisplayProgressBar(string title, string info, float progress);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool EventHasDragCopyModifierPressed(Event evt);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool EventHasDragMoveModifierPressed(Event evt);
        /// <summary>
        /// <para>Saves an AudioClip or MovieTexture to a file.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool ExtractOggFile(UnityEngine.Object obj, string path);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator, Obsolete("use AssetDatabase.LoadAssetAtPath")]
        public static extern UnityEngine.Object FindAsset(string path, System.Type type);
        [Obsolete("Use PrefabUtility.FindPrefabRoot")]
        public static GameObject FindPrefabRoot(GameObject source) => 
            PrefabUtility.FindPrefabRoot(source);

        /// <summary>
        /// <para>Brings the project window to the front and focuses it.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void FocusProjectWindow();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void ForceRebuildInspectors();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void ForceReloadInspectors();
        /// <summary>
        /// <para>Returns a text for a number of bytes.</para>
        /// </summary>
        /// <param name="bytes"></param>
        public static string FormatBytes(int bytes) => 
            FormatBytes((long) bytes);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string FormatBytes(long bytes);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern string GetActiveNativePlatformSupportModuleName();
        [Obsolete("Use AssetDatabase.GetAssetPath")]
        public static string GetAssetPath(UnityEngine.Object asset) => 
            AssetDatabase.GetAssetPath(asset);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern int GetDirtyIndex(int instanceID);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern string GetInvalidFilenameChars();
        /// <summary>
        /// <para>Is the object enabled (0 disabled, 1 enabled, -1 has no enabled button).</para>
        /// </summary>
        /// <param name="target"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern int GetObjectEnabled(UnityEngine.Object target);
        [Obsolete("Use PrefabUtility.GetPrefabParent")]
        public static UnityEngine.Object GetPrefabParent(UnityEngine.Object source) => 
            PrefabUtility.GetPrefabParent(source);

        [Obsolete("Use PrefabUtility.GetPrefabType")]
        public static PrefabType GetPrefabType(UnityEngine.Object target) => 
            PrefabUtility.GetPrefabType(target);

        internal static void InitInstantiatedPreviewRecursive(GameObject go)
        {
            go.hideFlags = HideFlags.HideAndDontSave;
            go.layer = Camera.PreviewCullingLayer;
            IEnumerator enumerator = go.transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    InitInstantiatedPreviewRecursive(current.gameObject);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        /// <summary>
        /// <para>Translates an instance ID to a reference to an object.</para>
        /// </summary>
        /// <param name="instanceID"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern UnityEngine.Object InstanceIDToObject(int instanceID);
        internal static GameObject InstantiateForAnimatorPreview(UnityEngine.Object original)
        {
            if (original == null)
            {
                throw new ArgumentException("The prefab you want to instantiate is null.");
            }
            GameObject go = InstantiateRemoveAllNonAnimationComponents(original, Vector3.zero, Quaternion.identity) as GameObject;
            go.name = go.name + "AnimatorPreview";
            go.tag = "Untagged";
            InitInstantiatedPreviewRecursive(go);
            Animator[] componentsInChildren = go.GetComponentsInChildren<Animator>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                Animator animator = componentsInChildren[i];
                animator.enabled = false;
                animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                animator.logWarnings = false;
                animator.fireEvents = false;
            }
            if (componentsInChildren.Length == 0)
            {
                Animator animator2 = go.AddComponent<Animator>();
                animator2.enabled = false;
                animator2.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                animator2.logWarnings = false;
                animator2.fireEvents = false;
            }
            return go;
        }

        [Obsolete("Use PrefabUtility.InstantiatePrefab")]
        public static UnityEngine.Object InstantiatePrefab(UnityEngine.Object target) => 
            PrefabUtility.InstantiatePrefab(target);

        internal static UnityEngine.Object InstantiateRemoveAllNonAnimationComponents(UnityEngine.Object original, Vector3 position, Quaternion rotation)
        {
            if (original == null)
            {
                throw new ArgumentException("The prefab you want to instantiate is null.");
            }
            return Internal_InstantiateRemoveAllNonAnimationComponentsSingle(original, position, rotation);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_DisplayObjectContextMenu(ref Rect position, UnityEngine.Object[] context, int contextUserData);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern UnityEngine.Object INTERNAL_CALL_Internal_InstantiateRemoveAllNonAnimationComponentsSingle(UnityEngine.Object data, ref Vector3 pos, ref Quaternion rot);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Private_DisplayCustomMenu(ref Rect screenPosition, string[] options, int[] enabled, int[] separator, int[] selected, SelectMenuItemFunction callback, object userData, bool showHotkey);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Private_DisplayPopupMenu(ref Rect position, string menuItemPath, UnityEngine.Object context, int contextUserData);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern GameObject Internal_CreateGameObjectWithHideFlags(string name, HideFlags flags);
        private static void Internal_DisplayCustomMenu(Rect screenPosition, string[] options, int[] enabled, int[] separator, int[] selected, SelectMenuItemFunction callback, object userData, bool showHotkey)
        {
            Private_DisplayCustomMenu(screenPosition, options, enabled, separator, selected, callback, userData, showHotkey);
        }

        internal static void Internal_DisplayObjectContextMenu(Rect position, UnityEngine.Object[] context, int contextUserData)
        {
            INTERNAL_CALL_Internal_DisplayObjectContextMenu(ref position, context, contextUserData);
        }

        internal static void Internal_DisplayPopupMenu(Rect position, string menuItemPath, UnityEngine.Object context, int contextUserData)
        {
            Private_DisplayPopupMenu(position, menuItemPath, context, contextUserData);
        }

        private static UnityEngine.Object Internal_InstantiateRemoveAllNonAnimationComponentsSingle(UnityEngine.Object data, Vector3 pos, Quaternion rot) => 
            INTERNAL_CALL_Internal_InstantiateRemoveAllNonAnimationComponentsSingle(data, ref pos, ref rot);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern string Internal_SaveFilePanelInProject(string title, string defaultName, string extension, string message, string path);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void Internal_UpdateAllMenus();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void Internal_UpdateMenuTitleForLanguage(SystemLanguage newloc);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void InternalCopySerializedIfDifferent(UnityEngine.Object source, UnityEngine.Object dest);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string InvokeDiffTool(string leftTitle, string leftFile, string rightTitle, string rightFile, string ancestorTitle, string ancestorFile);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool IsDirty(int instanceID);
        /// <summary>
        /// <para>Determines if an object is stored on disk.</para>
        /// </summary>
        /// <param name="target"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool IsPersistent(UnityEngine.Object target);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool IsWindows10OrGreater();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void LoadPlatformSupportModuleNativeDllInternal(string target);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void LoadPlatformSupportNativeLibrary(string nativeLibrary);
        public static bool LoadWindowLayout(string path)
        {
            bool newProjectLayoutWasCreated = false;
            return WindowLayout.LoadWindowLayout(path, newProjectLayoutWasCreated);
        }

        /// <summary>
        /// <para>Human-like sorting.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern int NaturalCompare(string a, string b);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern int NaturalCompareObjectNames(UnityEngine.Object a, UnityEngine.Object b);
        /// <summary>
        /// <para>Displays the "open file" dialog and returns the selected path name.</para>
        /// </summary>
        /// <param name="title"></param>
        /// <param name="directory"></param>
        /// <param name="extension"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string OpenFilePanel(string title, string directory, string extension);
        /// <summary>
        /// <para>Displays the "open file" dialog and returns the selected path name.</para>
        /// </summary>
        /// <param name="title">Title for dialog.</param>
        /// <param name="directory">Default directory.</param>
        /// <param name="filters">File extensions in form { "Image files", "png,jpg,jpeg", "All files", "*" }.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string OpenFilePanelWithFilters(string title, string directory, string[] filters);
        /// <summary>
        /// <para>Displays the "open folder" dialog and returns the selected path name.</para>
        /// </summary>
        /// <param name="title"></param>
        /// <param name="folder"></param>
        /// <param name="defaultName"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string OpenFolderPanel(string title, string folder, string defaultName);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void OpenWithDefaultApp(string fileName);
        private static void Private_DisplayCustomMenu(Rect screenPosition, string[] options, int[] enabled, int[] separator, int[] selected, SelectMenuItemFunction callback, object userData, bool showHotkey)
        {
            INTERNAL_CALL_Private_DisplayCustomMenu(ref screenPosition, options, enabled, separator, selected, callback, userData, showHotkey);
        }

        private static void Private_DisplayPopupMenu(Rect position, string menuItemPath, UnityEngine.Object context, int contextUserData)
        {
            INTERNAL_CALL_Private_DisplayPopupMenu(ref position, menuItemPath, context, contextUserData);
        }

        [Obsolete("Use PrefabUtility.CreateEmptyPrefab")]
        public static bool ReconnectToLastPrefab(GameObject go) => 
            PrefabUtility.ReconnectToLastPrefab(go);

        [Obsolete("Use PrefabUtility.ReplacePrefab")]
        public static GameObject ReplacePrefab(GameObject go, UnityEngine.Object targetPrefab) => 
            PrefabUtility.ReplacePrefab(go, targetPrefab, ReplacePrefabOptions.Default);

        [Obsolete("Use PrefabUtility.ReplacePrefab")]
        public static GameObject ReplacePrefab(GameObject go, UnityEngine.Object targetPrefab, ReplacePrefabOptions options) => 
            PrefabUtility.ReplacePrefab(go, targetPrefab, options);

        internal static void ResetMouseDown()
        {
            Tools.s_ButtonDown = -1;
            GUIUtility.hotControl = 0;
        }

        [Obsolete("Use PrefabUtility.ResetToPrefabState")]
        public static bool ResetToPrefabState(UnityEngine.Object source) => 
            PrefabUtility.ResetToPrefabState(source);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void RevealInFinder(string path);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern string SaveBuildPanel(BuildTarget target, string title, string directory, string defaultName, string extension, out bool updateExistingBuild);
        /// <summary>
        /// <para>Displays the "save file" dialog and returns the selected path name.</para>
        /// </summary>
        /// <param name="title"></param>
        /// <param name="directory"></param>
        /// <param name="defaultName"></param>
        /// <param name="extension"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string SaveFilePanel(string title, string directory, string defaultName, string extension);
        /// <summary>
        /// <para>Displays the "save file" dialog in the Assets folder of the project and returns the selected path name.</para>
        /// </summary>
        /// <param name="title"></param>
        /// <param name="defaultName"></param>
        /// <param name="extension"></param>
        /// <param name="message"></param>
        public static string SaveFilePanelInProject(string title, string defaultName, string extension, string message) => 
            Internal_SaveFilePanelInProject(title, defaultName, extension, message, "Assets");

        public static string SaveFilePanelInProject(string title, string defaultName, string extension, string message, string path) => 
            Internal_SaveFilePanelInProject(title, defaultName, extension, message, path);

        /// <summary>
        /// <para>Displays the "save folder" dialog and returns the selected path name.</para>
        /// </summary>
        /// <param name="title"></param>
        /// <param name="folder"></param>
        /// <param name="defaultName"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string SaveFolderPanel(string title, string folder, string defaultName);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void SetCameraAnimateMaterials(Camera camera, bool animate);
        /// <summary>
        /// <para>Marks target object as dirty. (Only suitable for non-scene objects).</para>
        /// </summary>
        /// <param name="target">The object to mark as dirty.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetDirty(UnityEngine.Object target);
        /// <summary>
        /// <para>Set the enabled state of the object.</para>
        /// </summary>
        /// <param name="target"></param>
        /// <param name="enabled"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetObjectEnabled(UnityEngine.Object target, bool enabled);
        /// <summary>
        /// <para>Set the Scene View selected display mode for this Renderer.</para>
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="renderState"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetSelectedRenderState(Renderer renderer, EditorSelectedRenderState renderState);
        /// <summary>
        /// <para>Sets whether the selected Renderer's wireframe will be hidden when the GameObject it is attached to is selected.</para>
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="enabled"></param>
        [Obsolete("Use EditorUtility.SetSelectedRenderState")]
        public static void SetSelectedWireframeHidden(Renderer renderer, bool enabled)
        {
            SetSelectedRenderState(renderer, !enabled ? (EditorSelectedRenderState.Highlight | EditorSelectedRenderState.Wireframe) : EditorSelectedRenderState.Hidden);
        }

        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("Use EditorUtility.UnloadUnusedAssetsImmediate instead"), GeneratedByOldBindingsGenerator]
        public static extern void UnloadUnusedAssets();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator, Obsolete("Use EditorUtility.UnloadUnusedAssetsImmediate instead")]
        public static extern void UnloadUnusedAssetsIgnoreManagedReferences();
        /// <summary>
        /// <para>Unloads assets that are not used.</para>
        /// </summary>
        /// <param name="ignoreReferencesFromScript">When true delete assets even if linked in scripts.</param>
        public static void UnloadUnusedAssetsImmediate()
        {
            UnloadUnusedAssetsImmediateInternal(true);
        }

        public static void UnloadUnusedAssetsImmediate(bool includeMonoReferencesAsRoots)
        {
            UnloadUnusedAssetsImmediateInternal(includeMonoReferencesAsRoots);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void UnloadUnusedAssetsImmediateInternal(bool includeMonoReferencesAsRoots);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool WarnPrefab(UnityEngine.Object target, string title, string warning, string okButton);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool WSACreateTestCertificate(string path, string publisher, string password, bool overwrite);

        public static bool audioMasterMute { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        internal static bool audioProfilingEnabled { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>True if there are any compilation error messages in the log.</para>
        /// </summary>
        public static bool scriptCompilationFailed { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public delegate void SelectMenuItemFunction(object userData, string[] options, int selected);
    }
}

