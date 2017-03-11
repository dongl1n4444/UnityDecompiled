namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEditor.Connect;
    using UnityEditor.Modules;
    using UnityEditor.VisualStudioIntegration;
    using UnityEditor.Web;
    using UnityEditorInternal;
    using UnityEngine;

    internal class PreferencesWindow : EditorWindow
    {
        private static Constants constants = null;
        private int currentPage;
        private const int k_LangListMenuOffset = 2;
        private static int kMaxSpriteCacheSizeInGigabytes = 200;
        private static int kMinSpriteCacheSizeInGigabytes = 1;
        private const int kRecentAppsCount = 10;
        private const string kRecentImageAppsKey = "RecentlyUsedImageApp";
        private const string kRecentScriptAppsKey = "RecentlyUsedScriptApp";
        private bool m_AllowAlphaNumericHierarchy = false;
        private bool m_AllowAttachedDebuggingOfEditor;
        private bool m_AllowAttachedDebuggingOfEditorStateChangedThisSession;
        private bool m_AutoRefresh;
        private bool m_CompressAssetsOnImport;
        private int m_DiffToolIndex;
        private string[] m_DiffTools;
        private string[] m_EditorLanguageNames;
        private bool m_EnableEditorAnalytics;
        private const string m_ExpressNotSupportedMessage = "Unfortunately Visual Studio Express does not allow itself to be controlled by external applications. You can still use it by manually opening the Visual Studio project file, but Unity cannot automatically open files for you when you doubleclick them. \n(This does work with Visual Studio Pro)";
        private bool m_ExternalEditorSupportsUnityProj;
        private GICacheSettings m_GICacheSettings;
        private string[] m_ImageAppDisplayNames;
        private RefString m_ImageAppPath = new RefString("");
        private string[] m_ImageApps;
        private string m_InvalidKeyMessage = string.Empty;
        private Vector2 m_KeyScrollPos;
        private string m_noDiffToolsMessage = string.Empty;
        private bool m_RefreshCustomPreferences;
        private bool m_ReopenLastUsedProjectOnStartup;
        private string[] m_ScriptAppDisplayNames;
        private string[] m_ScriptApps;
        private string[] m_ScriptAppsEditions;
        private string m_ScriptEditorArgs = "";
        private RefString m_ScriptEditorPath = new RefString("");
        private List<Section> m_Sections;
        private Vector2 m_SectionScrollPos;
        private PrefKey m_SelectedKey = null;
        private SystemLanguage m_SelectedLanguage = SystemLanguage.English;
        private int m_SelectedSectionIndex;
        private bool m_ShowAssetStoreSearchHits;
        private int m_SpriteAtlasCacheSize;
        private bool m_UseOSColorPicker;
        private bool m_ValidKeyChange = true;
        private bool m_VerifySavingAssets;
        private List<IPreferenceWindowExtension> prefWinExtensions;
        private SortedDictionary<string, List<KeyValuePair<string, PrefColor>>> s_CachedColors = null;
        private static int s_KeysControlHash = "KeysControlHash".GetHashCode();
        private static Vector2 s_ScrollPosition = Vector2.zero;

        private void AddCustomSections()
        {
            foreach (Assembly assembly in EditorAssemblies.loadedAssemblies)
            {
                System.Type[] typesFromAssembly = AssemblyHelper.GetTypesFromAssembly(assembly);
                foreach (System.Type type in typesFromAssembly)
                {
                    foreach (MethodInfo info in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
                    {
                        PreferenceItem customAttribute = Attribute.GetCustomAttribute(info, typeof(PreferenceItem)) as PreferenceItem;
                        if (customAttribute != null)
                        {
                            OnGUIDelegate guiFunc = Delegate.CreateDelegate(typeof(OnGUIDelegate), info) as OnGUIDelegate;
                            if (guiFunc != null)
                            {
                                this.m_Sections.Add(new Section(customAttribute.name, guiFunc));
                            }
                        }
                    }
                }
            }
        }

        private void ApplyChangesToPrefs()
        {
            if (GUI.changed)
            {
                this.WritePreferences();
                this.ReadPreferences();
                base.Repaint();
            }
        }

        private void AppsListClick(object userData, string[] options, int selected)
        {
            AppsListUserData data = (AppsListUserData) userData;
            if (options[selected] == "Browse...")
            {
                string str = EditorUtility.OpenFilePanel("Browse for application", "", InternalEditorUtility.GetApplicationExtensionForRuntimePlatform(Application.platform));
                if (str.Length != 0)
                {
                    data.str.str = str;
                    if (data.onChanged != null)
                    {
                        data.onChanged();
                    }
                }
            }
            else
            {
                data.str.str = data.paths[selected];
                if (data.onChanged != null)
                {
                    data.onChanged();
                }
            }
            this.WritePreferences();
            this.ReadPreferences();
        }

        private string[] BuildAppPathList(string userAppPath, string recentAppsKey, string stringForInternalEditor)
        {
            string[] array = new string[] { stringForInternalEditor };
            if (((userAppPath != null) && (userAppPath.Length != 0)) && (Array.IndexOf<string>(array, userAppPath) == -1))
            {
                ArrayUtility.Add<string>(ref array, userAppPath);
            }
            for (int i = 0; i < 10; i++)
            {
                string path = EditorPrefs.GetString(recentAppsKey + i);
                if (!File.Exists(path))
                {
                    path = "";
                    EditorPrefs.SetString(recentAppsKey + i, path);
                }
                if ((path.Length != 0) && (Array.IndexOf<string>(array, path) == -1))
                {
                    ArrayUtility.Add<string>(ref array, path);
                }
            }
            return array;
        }

        private string[] BuildFriendlyAppNameList(string[] appPathList, string[] appEditionList, string defaultBuiltIn)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < appPathList.Length; i++)
            {
                string app = appPathList[i];
                switch (app)
                {
                    case "internal":
                    case "":
                        list.Add(defaultBuiltIn);
                        break;

                    default:
                    {
                        string str2 = this.StripMicrosoftFromVisualStudioName(OSUtil.GetAppFriendlyName(app));
                        if ((appEditionList != null) && !string.IsNullOrEmpty(appEditionList[i]))
                        {
                            str2 = $"{str2} ({appEditionList[i]})";
                        }
                        list.Add(str2);
                        break;
                    }
                }
            }
            return list.ToArray();
        }

        private void DoUnityProjCheckbox()
        {
            bool flag = false;
            bool externalEditorSupportsUnityProj = false;
            switch (this.GetSelectedScriptEditor())
            {
                case InternalEditorUtility.ScriptEditor.Internal:
                    externalEditorSupportsUnityProj = true;
                    break;

                case InternalEditorUtility.ScriptEditor.MonoDevelop:
                    flag = true;
                    externalEditorSupportsUnityProj = this.m_ExternalEditorSupportsUnityProj;
                    break;
            }
            using (new EditorGUI.DisabledScope(!flag))
            {
                externalEditorSupportsUnityProj = EditorGUILayout.Toggle("Add .unityproj's to .sln", externalEditorSupportsUnityProj, new GUILayoutOption[0]);
            }
            if (flag)
            {
                this.m_ExternalEditorSupportsUnityProj = externalEditorSupportsUnityProj;
            }
        }

        private void FilePopup(string label, string selectedString, ref string[] names, ref string[] paths, RefString outString, string defaultString, Action onChanged)
        {
            GUIStyle popup = EditorStyles.popup;
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUILayout.PrefixLabel(label, popup);
            int[] selected = new int[0];
            if (paths.Contains<string>(selectedString))
            {
                selected = new int[] { Array.IndexOf<string>(paths, selectedString) };
            }
            GUIContent content = new GUIContent((selected.Length != 0) ? names[selected[0]] : defaultString);
            Rect position = GUILayoutUtility.GetRect(GUIContent.none, popup);
            AppsListUserData userData = new AppsListUserData(paths, outString, onChanged);
            if (EditorGUI.DropdownButton(position, content, FocusType.Passive, popup))
            {
                ArrayUtility.Add<string>(ref names, "Browse...");
                EditorUtility.DisplayCustomMenu(position, names, selected, new EditorUtility.SelectMenuItemFunction(this.AppsListClick), userData);
            }
            GUILayout.EndHorizontal();
        }

        private static string GetProgramFilesFolder()
        {
            string environmentVariable = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            if (environmentVariable != null)
            {
                return environmentVariable;
            }
            return Environment.GetEnvironmentVariable("ProgramFiles");
        }

        private InternalEditorUtility.ScriptEditor GetSelectedScriptEditor() => 
            InternalEditorUtility.GetScriptEditorFromPath(this.m_ScriptEditorPath.str);

        private void HandleKeys()
        {
            if ((Event.current.type == EventType.KeyDown) && (GUIUtility.keyboardControl == 0))
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.UpArrow:
                        this.selectedSectionIndex--;
                        Event.current.Use();
                        break;

                    case KeyCode.DownArrow:
                        this.selectedSectionIndex++;
                        Event.current.Use();
                        break;
                }
            }
        }

        private bool IsSelectedScriptEditorSpecial() => 
            InternalEditorUtility.IsScriptEditorSpecial(this.m_ScriptEditorPath.str);

        private void OnEnable()
        {
            this.prefWinExtensions = ModuleManager.GetPreferenceWindowExtensions();
            this.ReadPreferences();
            this.m_Sections = new List<Section>();
            this.m_Sections.Add(new Section("General", new OnGUIDelegate(this.ShowGeneral)));
            this.m_Sections.Add(new Section("External Tools", new OnGUIDelegate(this.ShowExternalApplications)));
            this.m_Sections.Add(new Section("Colors", new OnGUIDelegate(this.ShowColors)));
            this.m_Sections.Add(new Section("Keys", new OnGUIDelegate(this.ShowKeys)));
            this.m_Sections.Add(new Section("GI Cache", new OnGUIDelegate(this.ShowGICache)));
            this.m_Sections.Add(new Section("2D", new OnGUIDelegate(this.Show2D)));
            if (Unsupported.IsDeveloperBuild() || UnityConnect.preferencesEnabled)
            {
                this.m_Sections.Add(new Section("Unity Services", new OnGUIDelegate(this.ShowUnityConnectPrefs)));
            }
            this.m_RefreshCustomPreferences = true;
        }

        private void OnGUI()
        {
            if (this.m_RefreshCustomPreferences)
            {
                this.AddCustomSections();
                this.m_RefreshCustomPreferences = false;
            }
            EditorGUIUtility.labelWidth = 200f;
            if (constants == null)
            {
                constants = new Constants();
            }
            this.HandleKeys();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(120f) };
            this.m_SectionScrollPos = GUILayout.BeginScrollView(this.m_SectionScrollPos, constants.sectionScrollView, options);
            GUILayout.Space(40f);
            for (int i = 0; i < this.m_Sections.Count; i++)
            {
                Section section = this.m_Sections[i];
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
                Rect position = GUILayoutUtility.GetRect(section.content, constants.sectionElement, optionArray2);
                if ((section == this.selectedSection) && (Event.current.type == EventType.Repaint))
                {
                    constants.selected.Draw(position, false, false, false, false);
                }
                EditorGUI.BeginChangeCheck();
                if (GUI.Toggle(position, this.selectedSectionIndex == i, section.content, constants.sectionElement))
                {
                    this.selectedSectionIndex = i;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    GUIUtility.keyboardControl = 0;
                }
            }
            GUILayout.EndScrollView();
            GUILayout.Space(10f);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.Label(this.selectedSection.content, constants.sectionHeader, new GUILayoutOption[0]);
            GUILayout.Space(10f);
            s_ScrollPosition = EditorGUILayout.BeginScrollView(s_ScrollPosition, new GUILayoutOption[0]);
            this.selectedSection.guiFunc();
            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void OnScriptEditorArgsChanged()
        {
            InternalEditorUtility.SetExternalScriptEditorArgs(this.m_ScriptEditorArgs);
        }

        private void OnScriptEditorChanged()
        {
            InternalEditorUtility.SetExternalScriptEditor((string) this.m_ScriptEditorPath);
            this.m_ScriptEditorArgs = InternalEditorUtility.GetExternalScriptEditorArgs();
            UnityVSSupport.ScriptEditorChanged(this.m_ScriptEditorPath.str);
        }

        private SortedDictionary<string, List<KeyValuePair<string, T>>> OrderPrefs<T>(IEnumerable<KeyValuePair<string, T>> input) where T: IPrefType
        {
            SortedDictionary<string, List<KeyValuePair<string, T>>> dictionary = new SortedDictionary<string, List<KeyValuePair<string, T>>>();
            foreach (KeyValuePair<string, T> pair in input)
            {
                string str;
                string key;
                int index = pair.Key.IndexOf('/');
                if (index == -1)
                {
                    str = "General";
                    key = pair.Key;
                }
                else
                {
                    str = pair.Key.Substring(0, index);
                    key = pair.Key.Substring(index + 1);
                }
                if (!dictionary.ContainsKey(str))
                {
                    List<KeyValuePair<string, T>> collection = new List<KeyValuePair<string, T>> {
                        new KeyValuePair<string, T>(key, pair.Value)
                    };
                    dictionary.Add(str, new List<KeyValuePair<string, T>>(collection));
                }
                else
                {
                    dictionary[str].Add(new KeyValuePair<string, T>(key, pair.Value));
                }
            }
            return dictionary;
        }

        private void ReadPreferences()
        {
            this.m_ScriptEditorPath.str = InternalEditorUtility.GetExternalScriptEditor();
            this.m_ScriptEditorArgs = InternalEditorUtility.GetExternalScriptEditorArgs();
            this.m_ExternalEditorSupportsUnityProj = EditorPrefs.GetBool("kExternalEditorSupportsUnityProj", false);
            this.m_ImageAppPath.str = EditorPrefs.GetString("kImagesDefaultApp");
            this.m_ScriptApps = this.BuildAppPathList((string) this.m_ScriptEditorPath, "RecentlyUsedScriptApp", "internal");
            this.m_ScriptAppsEditions = new string[this.m_ScriptApps.Length];
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                foreach (VisualStudioPath[] pathArray in SyncVS.InstalledVisualStudios.Values)
                {
                    foreach (VisualStudioPath path in pathArray)
                    {
                        int index = Array.IndexOf<string>(this.m_ScriptApps, path.Path);
                        if (index == -1)
                        {
                            ArrayUtility.Add<string>(ref this.m_ScriptApps, path.Path);
                            ArrayUtility.Add<string>(ref this.m_ScriptAppsEditions, path.Edition);
                        }
                        else
                        {
                            this.m_ScriptAppsEditions[index] = path.Edition;
                        }
                    }
                }
            }
            this.m_ImageApps = this.BuildAppPathList((string) this.m_ImageAppPath, "RecentlyUsedImageApp", "");
            this.m_ScriptAppDisplayNames = this.BuildFriendlyAppNameList(this.m_ScriptApps, this.m_ScriptAppsEditions, "MonoDevelop (built-in)");
            this.m_ImageAppDisplayNames = this.BuildFriendlyAppNameList(this.m_ImageApps, null, "Open by file extension");
            this.m_DiffTools = InternalEditorUtility.GetAvailableDiffTools();
            if (((this.m_DiffTools == null) || (this.m_DiffTools.Length == 0)) && InternalEditorUtility.HasTeamLicense())
            {
                this.m_noDiffToolsMessage = InternalEditorUtility.GetNoDiffToolsDetectedMessage();
            }
            string str = EditorPrefs.GetString("kDiffsDefaultApp");
            this.m_DiffToolIndex = ArrayUtility.IndexOf<string>(this.m_DiffTools, str);
            if (this.m_DiffToolIndex == -1)
            {
                this.m_DiffToolIndex = 0;
            }
            this.m_AutoRefresh = EditorPrefs.GetBool("kAutoRefresh");
            this.m_ReopenLastUsedProjectOnStartup = EditorPrefs.GetBool("ReopenLastUsedProjectOnStartup");
            this.m_UseOSColorPicker = EditorPrefs.GetBool("UseOSColorPicker");
            this.m_EnableEditorAnalytics = EditorPrefs.GetBool("EnableEditorAnalytics", true);
            this.m_ShowAssetStoreSearchHits = EditorPrefs.GetBool("ShowAssetStoreSearchHits", true);
            this.m_VerifySavingAssets = EditorPrefs.GetBool("VerifySavingAssets", false);
            this.m_GICacheSettings.m_EnableCustomPath = EditorPrefs.GetBool("GICacheEnableCustomPath");
            this.m_GICacheSettings.m_CachePath = EditorPrefs.GetString("GICacheFolder");
            this.m_GICacheSettings.m_MaximumSize = EditorPrefs.GetInt("GICacheMaximumSizeGB");
            this.m_GICacheSettings.m_CompressionLevel = EditorPrefs.GetInt("GICacheCompressionLevel");
            this.m_SpriteAtlasCacheSize = EditorPrefs.GetInt("SpritePackerCacheMaximumSizeGB");
            this.m_AllowAttachedDebuggingOfEditor = EditorPrefs.GetBool("AllowAttachedDebuggingOfEditor", true);
            this.m_SelectedLanguage = LocalizationDatabase.GetCurrentEditorLanguage();
            this.m_AllowAlphaNumericHierarchy = EditorPrefs.GetBool("AllowAlphaNumericHierarchy", false);
            this.m_CompressAssetsOnImport = Unsupported.GetApplicationSettingCompressAssetsOnImport();
            foreach (IPreferenceWindowExtension extension in this.prefWinExtensions)
            {
                extension.ReadPreferences();
            }
        }

        private void RevertColors()
        {
            foreach (KeyValuePair<string, PrefColor> pair in Settings.Prefs<PrefColor>())
            {
                pair.Value.ResetToDefault();
                EditorPrefs.SetString(pair.Value.Name, pair.Value.ToUniqueString());
            }
        }

        private void RevertKeys()
        {
            foreach (KeyValuePair<string, PrefKey> pair in Settings.Prefs<PrefKey>())
            {
                pair.Value.ResetToDefault();
                EditorPrefs.SetString(pair.Value.Name, pair.Value.ToUniqueString());
            }
        }

        private static void SetupDefaultPreferences()
        {
        }

        private void Show2D()
        {
            EditorGUI.BeginChangeCheck();
            this.m_SpriteAtlasCacheSize = EditorGUILayout.IntSlider(Styles.spriteMaxCacheSize, this.m_SpriteAtlasCacheSize, kMinSpriteCacheSizeInGigabytes, kMaxSpriteCacheSizeInGigabytes, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                this.WritePreferences();
            }
        }

        private void ShowColors()
        {
            if (this.s_CachedColors == null)
            {
                this.s_CachedColors = this.OrderPrefs<PrefColor>(Settings.Prefs<PrefColor>());
            }
            bool flag = false;
            PrefColor color = null;
            foreach (KeyValuePair<string, List<KeyValuePair<string, PrefColor>>> pair in this.s_CachedColors)
            {
                GUILayout.Label(pair.Key, EditorStyles.boldLabel, new GUILayoutOption[0]);
                foreach (KeyValuePair<string, PrefColor> pair2 in pair.Value)
                {
                    EditorGUI.BeginChangeCheck();
                    Color color2 = EditorGUILayout.ColorField(pair2.Key, pair2.Value.Color, new GUILayoutOption[0]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        color = pair2.Value;
                        color.Color = color2;
                        flag = true;
                    }
                }
                if (color != null)
                {
                    Settings.Set<PrefColor>(color.Name, color);
                }
            }
            GUILayout.Space(5f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(120f) };
            if (GUILayout.Button("Use Defaults", options))
            {
                this.RevertColors();
                flag = true;
            }
            if (flag)
            {
                EditorApplication.RequestRepaintAllViews();
            }
        }

        private void ShowExternalApplications()
        {
            this.FilePopup("External Script Editor", (string) this.m_ScriptEditorPath, ref this.m_ScriptAppDisplayNames, ref this.m_ScriptApps, this.m_ScriptEditorPath, "internal", new Action(this.OnScriptEditorChanged));
            if (!this.IsSelectedScriptEditorSpecial())
            {
                string scriptEditorArgs = this.m_ScriptEditorArgs;
                this.m_ScriptEditorArgs = EditorGUILayout.TextField("External Script Editor Args", this.m_ScriptEditorArgs, new GUILayoutOption[0]);
                if (scriptEditorArgs != this.m_ScriptEditorArgs)
                {
                    this.OnScriptEditorArgsChanged();
                }
            }
            this.DoUnityProjCheckbox();
            bool allowAttachedDebuggingOfEditor = this.m_AllowAttachedDebuggingOfEditor;
            this.m_AllowAttachedDebuggingOfEditor = EditorGUILayout.Toggle("Editor Attaching", this.m_AllowAttachedDebuggingOfEditor, new GUILayoutOption[0]);
            if (allowAttachedDebuggingOfEditor != this.m_AllowAttachedDebuggingOfEditor)
            {
                this.m_AllowAttachedDebuggingOfEditorStateChangedThisSession = true;
            }
            if (this.m_AllowAttachedDebuggingOfEditorStateChangedThisSession)
            {
                GUILayout.Label("Changing this setting requires a restart to take effect.", EditorStyles.helpBox, new GUILayoutOption[0]);
            }
            if (this.GetSelectedScriptEditor() == InternalEditorUtility.ScriptEditor.VisualStudioExpress)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox, new GUILayoutOption[0]);
                GUILayout.Label("", constants.warningIcon, new GUILayoutOption[0]);
                GUILayout.Label("Unfortunately Visual Studio Express does not allow itself to be controlled by external applications. You can still use it by manually opening the Visual Studio project file, but Unity cannot automatically open files for you when you doubleclick them. \n(This does work with Visual Studio Pro)", constants.errorLabel, new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(10f);
            this.FilePopup("Image application", (string) this.m_ImageAppPath, ref this.m_ImageAppDisplayNames, ref this.m_ImageApps, this.m_ImageAppPath, "internal", null);
            GUILayout.Space(10f);
            using (new EditorGUI.DisabledScope(!InternalEditorUtility.HasTeamLicense()))
            {
                this.m_DiffToolIndex = EditorGUILayout.Popup("Revision Control Diff/Merge", this.m_DiffToolIndex, this.m_DiffTools, new GUILayoutOption[0]);
            }
            if (this.m_noDiffToolsMessage != string.Empty)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox, new GUILayoutOption[0]);
                GUILayout.Label("", constants.warningIcon, new GUILayoutOption[0]);
                GUILayout.Label(this.m_noDiffToolsMessage, constants.errorLabel, new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(10f);
            foreach (IPreferenceWindowExtension extension in this.prefWinExtensions)
            {
                if (extension.HasExternalApplications())
                {
                    GUILayout.Space(10f);
                    extension.ShowExternalApplications();
                }
            }
            this.ApplyChangesToPrefs();
        }

        private void ShowGeneral()
        {
            bool disabled = CollabAccess.Instance.IsServiceEnabled();
            using (new EditorGUI.DisabledScope(disabled))
            {
                if (disabled)
                {
                    this.m_AutoRefresh = EditorGUILayout.Toggle("Auto Refresh", true, new GUILayoutOption[0]);
                }
                else
                {
                    this.m_AutoRefresh = EditorGUILayout.Toggle("Auto Refresh", this.m_AutoRefresh, new GUILayoutOption[0]);
                }
            }
            if (disabled)
            {
                EditorGUILayout.HelpBox("Auto Refresh must be set when using Collaboration feature.", MessageType.Warning);
            }
            this.m_ReopenLastUsedProjectOnStartup = EditorGUILayout.Toggle("Load Previous Project on Startup", this.m_ReopenLastUsedProjectOnStartup, new GUILayoutOption[0]);
            bool compressAssetsOnImport = this.m_CompressAssetsOnImport;
            this.m_CompressAssetsOnImport = EditorGUILayout.Toggle("Compress Assets on Import", compressAssetsOnImport, new GUILayoutOption[0]);
            if (GUI.changed && (this.m_CompressAssetsOnImport != compressAssetsOnImport))
            {
                Unsupported.SetApplicationSettingCompressAssetsOnImport(this.m_CompressAssetsOnImport);
            }
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                this.m_UseOSColorPicker = EditorGUILayout.Toggle("OS X Color Picker", this.m_UseOSColorPicker, new GUILayoutOption[0]);
            }
            bool flag3 = Application.HasProLicense();
            using (new EditorGUI.DisabledScope(!flag3))
            {
                this.m_EnableEditorAnalytics = !EditorGUILayout.Toggle("Disable Editor Analytics (Pro Only)", !this.m_EnableEditorAnalytics, new GUILayoutOption[0]);
                if (!flag3 && !this.m_EnableEditorAnalytics)
                {
                    this.m_EnableEditorAnalytics = true;
                }
            }
            bool flag4 = false;
            EditorGUI.BeginChangeCheck();
            this.m_ShowAssetStoreSearchHits = EditorGUILayout.Toggle("Show Asset Store search hits", this.m_ShowAssetStoreSearchHits, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                flag4 = true;
            }
            this.m_VerifySavingAssets = EditorGUILayout.Toggle("Verify Saving Assets", this.m_VerifySavingAssets, new GUILayoutOption[0]);
            using (new EditorGUI.DisabledScope(!flag3))
            {
                string[] displayedOptions = new string[] { "Personal", "Professional" };
                int num = EditorGUILayout.Popup("Editor Skin", EditorGUIUtility.isProSkin ? 1 : 0, displayedOptions, new GUILayoutOption[0]);
                if ((EditorGUIUtility.isProSkin ? 1 : 0) != num)
                {
                    InternalEditorUtility.SwitchSkinAndRepaintAllViews();
                }
            }
            bool allowAlphaNumericHierarchy = this.m_AllowAlphaNumericHierarchy;
            this.m_AllowAlphaNumericHierarchy = EditorGUILayout.Toggle("Enable Alpha Numeric Sorting", this.m_AllowAlphaNumericHierarchy, new GUILayoutOption[0]);
            bool flag6 = false;
            SystemLanguage selectedLanguage = this.m_SelectedLanguage;
            SystemLanguage[] availableEditorLanguages = LocalizationDatabase.GetAvailableEditorLanguages();
            if (availableEditorLanguages.Length > 1)
            {
                if (this.m_EditorLanguageNames == null)
                {
                    this.m_EditorLanguageNames = new string[availableEditorLanguages.Length];
                    for (int i = 0; i < availableEditorLanguages.Length; i++)
                    {
                        this.m_EditorLanguageNames[i] = availableEditorLanguages[i].ToString();
                    }
                    string item = $"Default ( {LocalizationDatabase.GetDefaultEditorLanguage().ToString()} )";
                    ArrayUtility.Insert<string>(ref this.m_EditorLanguageNames, 0, "");
                    ArrayUtility.Insert<string>(ref this.m_EditorLanguageNames, 0, item);
                }
                EditorGUI.BeginChangeCheck();
                selectedLanguage = this.m_SelectedLanguage;
                int selectedIndex = 2 + Array.IndexOf<SystemLanguage>(availableEditorLanguages, this.m_SelectedLanguage);
                int num4 = EditorGUILayout.Popup("Language", selectedIndex, this.m_EditorLanguageNames, new GUILayoutOption[0]);
                this.m_SelectedLanguage = (num4 != 0) ? availableEditorLanguages[num4 - 2] : LocalizationDatabase.GetDefaultEditorLanguage();
                if (EditorGUI.EndChangeCheck() && (selectedLanguage != this.m_SelectedLanguage))
                {
                    flag6 = true;
                }
            }
            this.ApplyChangesToPrefs();
            if (flag6)
            {
                EditorGUIUtility.NotifyLanguageChanged(this.m_SelectedLanguage);
                InternalEditorUtility.RequestScriptReload();
            }
            if (allowAlphaNumericHierarchy != this.m_AllowAlphaNumericHierarchy)
            {
                EditorApplication.DirtyHierarchyWindowSorting();
            }
            if (flag4)
            {
                ProjectBrowser.ShowAssetStoreHitsWhileSearchingLocalAssetsChanged();
            }
        }

        private void ShowGICache()
        {
            this.m_GICacheSettings.m_MaximumSize = EditorGUILayout.IntSlider(Styles.maxCacheSize, this.m_GICacheSettings.m_MaximumSize, 5, 200, new GUILayoutOption[0]);
            this.WritePreferences();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (Lightmapping.isRunning)
            {
                EditorGUILayout.HelpBox(EditorGUIUtility.TextContent(Styles.cantChangeCacheSettings.text).text, MessageType.Warning, true);
            }
            GUILayout.EndHorizontal();
            using (new EditorGUI.DisabledScope(Lightmapping.isRunning))
            {
                this.m_GICacheSettings.m_EnableCustomPath = EditorGUILayout.Toggle(Styles.customCacheLocation, this.m_GICacheSettings.m_EnableCustomPath, new GUILayoutOption[0]);
                if (this.m_GICacheSettings.m_EnableCustomPath)
                {
                    GUIStyle miniButton = EditorStyles.miniButton;
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    EditorGUILayout.PrefixLabel(Styles.cacheFolderLocation, miniButton);
                    Rect position = GUILayoutUtility.GetRect(GUIContent.none, miniButton);
                    GUIContent content = !string.IsNullOrEmpty(this.m_GICacheSettings.m_CachePath) ? new GUIContent(this.m_GICacheSettings.m_CachePath) : Styles.browse;
                    if (EditorGUI.DropdownButton(position, content, FocusType.Passive, miniButton))
                    {
                        string cachePath = this.m_GICacheSettings.m_CachePath;
                        string str2 = EditorUtility.OpenFolderPanel(Styles.browseGICacheLocation.text, cachePath, "");
                        if (!string.IsNullOrEmpty(str2))
                        {
                            this.m_GICacheSettings.m_CachePath = str2;
                            this.WritePreferences();
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                else
                {
                    this.m_GICacheSettings.m_CachePath = "";
                }
                this.m_GICacheSettings.m_CompressionLevel = !EditorGUILayout.Toggle(Styles.cacheCompression, this.m_GICacheSettings.m_CompressionLevel == 1, new GUILayoutOption[0]) ? 0 : 1;
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(120f) };
                if (GUILayout.Button(Styles.cleanCache, options))
                {
                    EditorUtility.DisplayProgressBar(Styles.cleanCache.text, Styles.pleaseWait.text, 0f);
                    Lightmapping.Clear();
                    EditorUtility.DisplayProgressBar(Styles.cleanCache.text, Styles.pleaseWait.text, 0.5f);
                    Lightmapping.ClearDiskCache();
                    EditorUtility.ClearProgressBar();
                }
                if (Lightmapping.diskCacheSize >= 0L)
                {
                    GUILayout.Label(Styles.cacheSizeIs.text + " " + EditorUtility.FormatBytes(Lightmapping.diskCacheSize), new GUILayoutOption[0]);
                }
                else
                {
                    GUILayout.Label(Styles.cacheSizeIs.text + " is being calculated...", new GUILayoutOption[0]);
                }
                GUILayout.Label(Styles.cacheFolderLocation.text + ":", new GUILayoutOption[0]);
                GUILayout.Label(Lightmapping.diskCachePath, constants.cacheFolderLocation, new GUILayoutOption[0]);
            }
        }

        private void ShowKeys()
        {
            int controlID = GUIUtility.GetControlID(s_KeysControlHash, FocusType.Keyboard);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(185f) };
            GUILayout.BeginVertical(options);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
            GUILayout.Label("Actions", constants.settingsBoxTitle, optionArray2);
            this.m_KeyScrollPos = GUILayout.BeginScrollView(this.m_KeyScrollPos, constants.settingsBox);
            PrefKey key = null;
            PrefKey key2 = null;
            bool flag = false;
            foreach (KeyValuePair<string, PrefKey> pair in Settings.Prefs<PrefKey>())
            {
                if (!flag)
                {
                    if (pair.Value == this.m_SelectedKey)
                    {
                        flag = true;
                    }
                    else
                    {
                        key = pair.Value;
                    }
                }
                else if (key2 == null)
                {
                    key2 = pair.Value;
                }
                EditorGUI.BeginChangeCheck();
                if (GUILayout.Toggle(pair.Value == this.m_SelectedKey, pair.Key, constants.keysElement, new GUILayoutOption[0]))
                {
                    if (this.m_SelectedKey != pair.Value)
                    {
                        this.m_ValidKeyChange = true;
                    }
                    this.m_SelectedKey = pair.Value;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    GUIUtility.keyboardControl = controlID;
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.Space(10f);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            if (this.m_SelectedKey != null)
            {
                Event keyboardEvent = this.m_SelectedKey.KeyboardEvent;
                GUI.changed = false;
                char[] separator = new char[] { '/' };
                string[] strArray = this.m_SelectedKey.Name.Split(separator);
                GUILayout.Label(strArray[0], "boldLabel", new GUILayoutOption[0]);
                GUILayout.Label(strArray[1], "boldLabel", new GUILayoutOption[0]);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.Label("Key:", new GUILayoutOption[0]);
                keyboardEvent = EditorGUILayout.KeyEventField(keyboardEvent, new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.Label("Modifiers:", new GUILayoutOption[0]);
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                if (Application.platform == RuntimePlatform.OSXEditor)
                {
                    keyboardEvent.command = GUILayout.Toggle(keyboardEvent.command, "Command", new GUILayoutOption[0]);
                }
                keyboardEvent.control = GUILayout.Toggle(keyboardEvent.control, "Control", new GUILayoutOption[0]);
                keyboardEvent.shift = GUILayout.Toggle(keyboardEvent.shift, "Shift", new GUILayoutOption[0]);
                keyboardEvent.alt = GUILayout.Toggle(keyboardEvent.alt, "Alt", new GUILayoutOption[0]);
                if (!GUI.changed)
                {
                    if ((GUIUtility.keyboardControl == controlID) && (Event.current.type == EventType.KeyDown))
                    {
                        switch (Event.current.keyCode)
                        {
                            case KeyCode.UpArrow:
                                if ((key != null) && (key != this.m_SelectedKey))
                                {
                                    this.m_SelectedKey = key;
                                    this.m_ValidKeyChange = true;
                                }
                                Event.current.Use();
                                break;

                            case KeyCode.DownArrow:
                                if ((key2 != null) && (key2 != this.m_SelectedKey))
                                {
                                    this.m_SelectedKey = key2;
                                    this.m_ValidKeyChange = true;
                                }
                                Event.current.Use();
                                break;
                        }
                    }
                }
                else
                {
                    this.m_ValidKeyChange = true;
                    char[] chArray2 = new char[] { '/' };
                    string str = this.m_SelectedKey.Name.Split(chArray2)[0];
                    foreach (KeyValuePair<string, PrefKey> pair2 in Settings.Prefs<PrefKey>())
                    {
                        char[] chArray3 = new char[] { '/' };
                        string str2 = pair2.Key.Split(chArray3)[0];
                        if ((pair2.Value.KeyboardEvent.Equals(keyboardEvent) && (str2 == str)) && (pair2.Key != this.m_SelectedKey.Name))
                        {
                            this.m_ValidKeyChange = false;
                            StringBuilder builder = new StringBuilder();
                            if ((Application.platform == RuntimePlatform.OSXEditor) && keyboardEvent.command)
                            {
                                builder.Append("Command+");
                            }
                            if (keyboardEvent.control)
                            {
                                builder.Append("Ctrl+");
                            }
                            if (keyboardEvent.shift)
                            {
                                builder.Append("Shift+");
                            }
                            if (keyboardEvent.alt)
                            {
                                builder.Append("Alt+");
                            }
                            builder.Append(keyboardEvent.keyCode);
                            this.m_InvalidKeyMessage = $"Key {builder} can't be used for action "{this.m_SelectedKey.Name}" because it's already used for action "{pair2.Key}"";
                            break;
                        }
                    }
                    if (this.m_ValidKeyChange)
                    {
                        this.m_SelectedKey.KeyboardEvent = keyboardEvent;
                        Settings.Set<PrefKey>(this.m_SelectedKey.Name, this.m_SelectedKey);
                    }
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                if (!this.m_ValidKeyChange)
                {
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayout.Label("", constants.warningIcon, new GUILayoutOption[0]);
                    GUILayout.Label(this.m_InvalidKeyMessage, constants.errorLabel, new GUILayoutOption[0]);
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
            GUILayout.Space(10f);
            GUILayout.EndHorizontal();
            GUILayout.Space(5f);
            GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(120f) };
            if (GUILayout.Button("Use Defaults", optionArray3))
            {
                this.m_ValidKeyChange = true;
                this.RevertKeys();
            }
        }

        private static void ShowPreferencesWindow()
        {
            EditorWindow window = EditorWindow.GetWindow<PreferencesWindow>(true, "Unity Preferences");
            window.minSize = new Vector2(500f, 400f);
            window.maxSize = new Vector2(window.minSize.x, window.maxSize.y);
            window.position = new Rect(new Vector2(100f, 100f), window.minSize);
            window.m_Parent.window.m_DontSaveToLayout = true;
        }

        private void ShowUnityConnectPrefs()
        {
            UnityConnectPrefs.ShowPanelPrefUI();
            this.ApplyChangesToPrefs();
        }

        private string StripMicrosoftFromVisualStudioName(string arg)
        {
            if (!arg.Contains("Visual Studio"))
            {
                return arg;
            }
            if (!arg.StartsWith("Microsoft"))
            {
                return arg;
            }
            return arg.Substring("Microsoft ".Length);
        }

        private void WritePreferences()
        {
            InternalEditorUtility.SetExternalScriptEditor((string) this.m_ScriptEditorPath);
            InternalEditorUtility.SetExternalScriptEditorArgs(this.m_ScriptEditorArgs);
            EditorPrefs.SetBool("kExternalEditorSupportsUnityProj", this.m_ExternalEditorSupportsUnityProj);
            EditorPrefs.SetString("kImagesDefaultApp", (string) this.m_ImageAppPath);
            EditorPrefs.SetString("kDiffsDefaultApp", (this.m_DiffTools.Length != 0) ? this.m_DiffTools[this.m_DiffToolIndex] : "");
            this.WriteRecentAppsList(this.m_ScriptApps, (string) this.m_ScriptEditorPath, "RecentlyUsedScriptApp");
            this.WriteRecentAppsList(this.m_ImageApps, (string) this.m_ImageAppPath, "RecentlyUsedImageApp");
            EditorPrefs.SetBool("kAutoRefresh", this.m_AutoRefresh);
            if (Unsupported.IsDeveloperBuild() || UnityConnect.preferencesEnabled)
            {
                UnityConnectPrefs.StorePanelPrefs();
            }
            EditorPrefs.SetBool("ReopenLastUsedProjectOnStartup", this.m_ReopenLastUsedProjectOnStartup);
            EditorPrefs.SetBool("UseOSColorPicker", this.m_UseOSColorPicker);
            EditorPrefs.SetBool("EnableEditorAnalytics", this.m_EnableEditorAnalytics);
            EditorPrefs.SetBool("ShowAssetStoreSearchHits", this.m_ShowAssetStoreSearchHits);
            EditorPrefs.SetBool("VerifySavingAssets", this.m_VerifySavingAssets);
            EditorPrefs.SetBool("AllowAttachedDebuggingOfEditor", this.m_AllowAttachedDebuggingOfEditor);
            LocalizationDatabase.SetCurrentEditorLanguage(this.m_SelectedLanguage);
            EditorPrefs.SetBool("AllowAlphaNumericHierarchy", this.m_AllowAlphaNumericHierarchy);
            EditorPrefs.SetBool("GICacheEnableCustomPath", this.m_GICacheSettings.m_EnableCustomPath);
            EditorPrefs.SetInt("GICacheMaximumSizeGB", this.m_GICacheSettings.m_MaximumSize);
            EditorPrefs.SetString("GICacheFolder", this.m_GICacheSettings.m_CachePath);
            EditorPrefs.SetInt("GICacheCompressionLevel", this.m_GICacheSettings.m_CompressionLevel);
            EditorPrefs.SetInt("SpritePackerCacheMaximumSizeGB", this.m_SpriteAtlasCacheSize);
            foreach (IPreferenceWindowExtension extension in this.prefWinExtensions)
            {
                extension.WritePreferences();
            }
            Lightmapping.UpdateCachePath();
        }

        private void WriteRecentAppsList(string[] paths, string path, string prefsKey)
        {
            int num = 0;
            if (path.Length != 0)
            {
                EditorPrefs.SetString(prefsKey + num, path);
                num++;
            }
            for (int i = 0; i < paths.Length; i++)
            {
                if (num >= 10)
                {
                    break;
                }
                if ((paths[i].Length != 0) && (paths[i] != path))
                {
                    EditorPrefs.SetString(prefsKey + num, paths[i]);
                    num++;
                }
            }
        }

        private Section selectedSection =>
            this.m_Sections[this.m_SelectedSectionIndex];

        private int selectedSectionIndex
        {
            get => 
                this.m_SelectedSectionIndex;
            set
            {
                if (this.m_SelectedSectionIndex != value)
                {
                    this.m_ValidKeyChange = true;
                }
                this.m_SelectedSectionIndex = value;
                if (this.m_SelectedSectionIndex >= this.m_Sections.Count)
                {
                    this.m_SelectedSectionIndex = 0;
                }
                else if (this.m_SelectedSectionIndex < 0)
                {
                    this.m_SelectedSectionIndex = this.m_Sections.Count - 1;
                }
            }
        }

        private class AppsListUserData
        {
            public Action onChanged;
            public string[] paths;
            public PreferencesWindow.RefString str;

            public AppsListUserData(string[] paths, PreferencesWindow.RefString str, Action onChanged)
            {
                this.paths = paths;
                this.str = str;
                this.onChanged = onChanged;
            }
        }

        internal class Constants
        {
            public GUIStyle cacheFolderLocation = new GUIStyle(GUI.skin.label);
            public GUIStyle errorLabel = "WordWrappedLabel";
            public GUIStyle evenRow = "CN EntryBackEven";
            public GUIStyle keysElement = "PreferencesKeysElement";
            public GUIStyle oddRow = "CN EntryBackOdd";
            public GUIStyle sectionElement = "PreferencesSection";
            public GUIStyle sectionHeader = new GUIStyle(EditorStyles.largeLabel);
            public GUIStyle sectionScrollView = "PreferencesSectionBox";
            public GUIStyle selected = "OL SelectedRow";
            public GUIStyle settingsBox = "OL Box";
            public GUIStyle settingsBoxTitle = "OL Title";
            public GUIStyle warningIcon = "CN EntryWarn";

            public Constants()
            {
                this.sectionScrollView = new GUIStyle(this.sectionScrollView);
                RectOffset overflow = this.sectionScrollView.overflow;
                overflow.bottom++;
                this.sectionHeader.fontStyle = FontStyle.Bold;
                this.sectionHeader.fontSize = 0x12;
                this.sectionHeader.margin.top = 10;
                RectOffset margin = this.sectionHeader.margin;
                margin.left++;
                if (!EditorGUIUtility.isProSkin)
                {
                    this.sectionHeader.normal.textColor = new Color(0.4f, 0.4f, 0.4f, 1f);
                }
                else
                {
                    this.sectionHeader.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1f);
                }
                this.cacheFolderLocation.wordWrap = true;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct GICacheSettings
        {
            public bool m_EnableCustomPath;
            public int m_MaximumSize;
            public string m_CachePath;
            public int m_CompressionLevel;
        }

        private delegate void OnGUIDelegate();

        private class RefString
        {
            public string str;

            public RefString(string s)
            {
                this.str = s;
            }

            public static implicit operator string(PreferencesWindow.RefString s) => 
                s.str;

            public override string ToString() => 
                this.str;
        }

        private class Section
        {
            public GUIContent content;
            public PreferencesWindow.OnGUIDelegate guiFunc;

            public Section(string name, PreferencesWindow.OnGUIDelegate guiFunc)
            {
                this.content = new GUIContent(name);
                this.guiFunc = guiFunc;
            }

            public Section(GUIContent content, PreferencesWindow.OnGUIDelegate guiFunc)
            {
                this.content = content;
                this.guiFunc = guiFunc;
            }

            public Section(string name, Texture2D icon, PreferencesWindow.OnGUIDelegate guiFunc)
            {
                this.content = new GUIContent(name, icon);
                this.guiFunc = guiFunc;
            }
        }

        internal class Styles
        {
            public static readonly GUIContent browse = EditorGUIUtility.TextContent("Browse...");
            public static readonly GUIContent browseGICacheLocation = EditorGUIUtility.TextContent("Browse for GI Cache location");
            public static readonly GUIContent cacheCompression = EditorGUIUtility.TextContent("Cache compression|Use fast realtime compression for the GI cache files to reduce the size of generated data. Disable it and clean the cache if you need access to the raw data generated by Enlighten.");
            public static readonly GUIContent cacheFolderLocation = EditorGUIUtility.TextContent("Cache Folder Location|The GI Cache folder is shared between all projects.");
            public static readonly GUIContent cacheSizeIs = EditorGUIUtility.TextContent("Cache size is");
            public static readonly GUIContent cantChangeCacheSettings = EditorGUIUtility.TextContent("Cache settings can't be changed while lightmapping is being computed.");
            public static readonly GUIContent cleanCache = EditorGUIUtility.TextContent("Clean Cache");
            public static readonly GUIContent customCacheLocation = EditorGUIUtility.TextContent("Custom cache location|Specify the GI Cache folder location.");
            public static readonly GUIContent maxCacheSize = EditorGUIUtility.TextContent("Maximum Cache Size (GB)|The size of the GI Cache folder will be kept below this maximum value when possible. A background job will periodically clean up the oldest unused files.");
            public static readonly GUIContent pleaseWait = EditorGUIUtility.TextContent("Please wait...");
            public static readonly GUIContent spriteMaxCacheSize = EditorGUIUtility.TextContent("Max Sprite Atlas Cache Size (GB)|The size of the Sprite Atlas Cache folder will be kept below this maximum value when possible. Change requires Editor restart");
        }
    }
}

