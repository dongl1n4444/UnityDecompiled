namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.AnimatedValues;
    using UnityEditor.Build;
    using UnityEditor.CrashReporting;
    using UnityEditor.Modules;
    using UnityEditor.SceneManagement;
    using UnityEditorInternal;
    using UnityEditorInternal.VR;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Rendering;

    [CustomEditor(typeof(PlayerSettings))]
    internal class PlayerSettingsEditor : Editor
    {
        private static ApiCompatibilityLevel[] allProfiles;
        private const int kIconSpacing = 6;
        private const int kMaxPreviewSize = 0x60;
        private const int kSlotSize = 0x40;
        private SerializedProperty m_AccelerometerFrequency;
        private SerializedProperty m_ActionOnDotNetUnhandledException;
        private SerializedProperty m_ActiveColorSpace;
        private SerializedProperty m_AllowedAutoRotateToLandscapeLeft;
        private SerializedProperty m_AllowedAutoRotateToLandscapeRight;
        private SerializedProperty m_AllowedAutoRotateToPortrait;
        private SerializedProperty m_AllowedAutoRotateToPortraitUpsideDown;
        private SerializedProperty m_AllowFullscreenSwitch;
        private SerializedProperty m_AndroidProfiler;
        private SerializedProperty m_androidShowActivityIndicatorOnLoading;
        private SerializedProperty m_AotOptions;
        private SerializedProperty m_ApplicationBundleVersion;
        private SerializedProperty m_BakeCollisionMeshes;
        private SerializedProperty m_CameraUsageDescription;
        private SerializedProperty m_CaptureSingleScreen;
        private SerializedProperty m_CompanyName;
        private SerializedProperty m_CursorHotspot;
        private SerializedProperty m_D3D11FullscreenMode;
        private SerializedProperty m_D3D9FullscreenMode;
        private SerializedProperty m_DefaultCursor;
        private SerializedProperty m_DefaultIsFullScreen;
        private SerializedProperty m_DefaultIsNativeResolution;
        private SerializedProperty m_DefaultScreenHeight;
        private SerializedProperty m_DefaultScreenOrientation;
        private SerializedProperty m_DefaultScreenWidth;
        private SerializedProperty m_DisableDepthAndStencilBuffers;
        private SerializedProperty m_DisplayResolutionDialog;
        private SerializedProperty m_EnableCrashReportAPI;
        private SerializedProperty m_EnableInternalProfiler;
        private SerializedProperty m_ForceSingleInstance;
        private static GUIContent[] m_GfxJobModeNames;
        private static GraphicsJobMode[] m_GfxJobModeValues;
        private SerializedProperty m_GraphicsJobs;
        private SerializedProperty m_IOSAllowHTTPDownload;
        private SerializedProperty m_iosShowActivityIndicatorOnLoading;
        private SerializedProperty m_IOSURLSchemes;
        private SerializedProperty m_IPhoneApplicationDisplayName;
        private SerializedProperty m_IPhoneScriptCallOptimization;
        private SerializedProperty m_IPhoneSdkVersion;
        private SerializedProperty m_IPhoneStrippingLevel;
        private SerializedProperty m_IPhoneTargetOSVersion;
        private SerializedProperty m_KeepLoadedShadersAlive;
        private SerializedProperty m_LocationUsageDescription;
        private SerializedProperty m_LogObjCUncaughtExceptions;
        private SerializedProperty m_MacFullscreenMode;
        private SerializedProperty m_MetalAPIValidation;
        private SerializedProperty m_MetalEditorSupport;
        private SerializedProperty m_MetalForceHardShadows;
        private SerializedProperty m_MicrophoneUsageDescription;
        private SerializedProperty m_MobileMTRendering;
        private SerializedProperty m_MTRendering;
        private SerializedProperty m_MuteOtherAudioSources;
        private static Dictionary<ApiCompatibilityLevel, GUIContent> m_NiceApiCompatibilityLevelNames;
        private static Dictionary<ScriptingImplementation, GUIContent> m_NiceScriptingBackendNames;
        private SerializedProperty m_PreloadedAssets;
        private SerializedProperty m_PrepareIOSForRecording;
        private SerializedProperty m_ProductName;
        private SerializedProperty m_RequireES31;
        private SerializedProperty m_RequireES31AEP;
        private SerializedProperty m_ResizableWindow;
        private SerializedProperty m_RunInBackground;
        private AnimBool[] m_SectionAnimators = new AnimBool[6];
        private SavedInt m_SelectedSection = new SavedInt("PlayerSettings.ShownSection", -1);
        private ISettingEditorExtension[] m_SettingsExtensions;
        private readonly AnimBool m_ShowDefaultIsNativeResolution = new AnimBool();
        private readonly AnimBool m_ShowResolution = new AnimBool();
        private SerializedProperty m_SkinOnGPU;
        private PlayerSettingsSplashScreenEditor m_SplashScreenEditor;
        private SerializedProperty m_StereoRenderingPath;
        private SerializedProperty m_StripEngineCode;
        private SerializedProperty m_StripUnusedMeshComponents;
        private SerializedProperty m_SubmitAnalytics;
        private SerializedProperty m_SupportedAspectRatios;
        private SerializedProperty m_TargetDevice;
        private SerializedProperty m_tizenShowActivityIndicatorOnLoading;
        private SerializedProperty m_tvOSSdkVersion;
        private SerializedProperty m_tvOSTargetOSVersion;
        private SerializedProperty m_UIPrerenderedIcon;
        private SerializedProperty m_UIRequiresFullScreen;
        private SerializedProperty m_UIRequiresPersistentWiFi;
        private SerializedProperty m_UIStatusBarHidden;
        private SerializedProperty m_UIStatusBarStyle;
        private SerializedProperty m_Use32BitDisplayBuffer;
        private SerializedProperty m_UseMacAppStoreValidation;
        private SerializedProperty m_useOnDemandResources;
        private SerializedProperty m_UseOSAutoRotation;
        private SerializedProperty m_UsePlayerLog;
        private SerializedProperty m_VertexChannelCompressionMask;
        private SerializedProperty m_VideoMemoryForVertexBuffers;
        private SerializedProperty m_VisibleInBackground;
        public PlayerSettingsEditorVR m_VRSettings;
        private static ApiCompatibilityLevel[] only_2_0_profiles;
        private static Dictionary<BuildTarget, ReorderableList> s_GraphicsDeviceLists;
        private static Texture2D s_WarningIcon;
        private int scriptingDefinesControlID = 0;
        private int selectedPlatform = 0;
        private BuildPlatform[] validPlatforms;

        static PlayerSettingsEditor()
        {
            GraphicsJobMode[] modeArray1 = new GraphicsJobMode[2];
            modeArray1[1] = GraphicsJobMode.Legacy;
            m_GfxJobModeValues = modeArray1;
            m_GfxJobModeNames = new GUIContent[] { new GUIContent("Native"), new GUIContent("Legacy") };
            s_GraphicsDeviceLists = new Dictionary<BuildTarget, ReorderableList>();
            only_2_0_profiles = new ApiCompatibilityLevel[] { ApiCompatibilityLevel.NET_2_0, ApiCompatibilityLevel.NET_2_0_Subset };
            allProfiles = new ApiCompatibilityLevel[] { ApiCompatibilityLevel.NET_2_0 };
        }

        private void AddGraphicsDeviceElement(BuildTarget target, Rect rect, ReorderableList list)
        {
            GraphicsDeviceType[] supportedGraphicsAPIs = PlayerSettings.GetSupportedGraphicsAPIs(target);
            if ((supportedGraphicsAPIs != null) && (supportedGraphicsAPIs.Length != 0))
            {
                string[] options = new string[supportedGraphicsAPIs.Length];
                bool[] enabled = new bool[supportedGraphicsAPIs.Length];
                for (int i = 0; i < supportedGraphicsAPIs.Length; i++)
                {
                    options[i] = supportedGraphicsAPIs[i].ToString();
                    enabled[i] = !list.list.Contains(supportedGraphicsAPIs[i]);
                }
                EditorUtility.DisplayCustomMenu(rect, options, enabled, null, new EditorUtility.SelectMenuItemFunction(this.AddGraphicsDeviceMenuSelected), target);
            }
        }

        private void AddGraphicsDeviceMenuSelected(object userData, string[] options, int selected)
        {
            BuildTarget platform = (BuildTarget) userData;
            GraphicsDeviceType[] graphicsAPIs = PlayerSettings.GetGraphicsAPIs(platform);
            if (graphicsAPIs != null)
            {
                GraphicsDeviceType item = (GraphicsDeviceType) Enum.Parse(typeof(GraphicsDeviceType), options[selected], true);
                List<GraphicsDeviceType> list = graphicsAPIs.ToList<GraphicsDeviceType>();
                list.Add(item);
                graphicsAPIs = list.ToArray();
                PlayerSettings.SetGraphicsAPIs(platform, graphicsAPIs);
            }
        }

        private void ApplyChangedGraphicsAPIList(BuildTarget target, GraphicsDeviceType[] apis, bool firstEntryChanged)
        {
            ChangeGraphicsApiAction action = this.CheckApplyGraphicsAPIList(target, firstEntryChanged);
            this.ApplyChangeGraphicsApiAction(target, apis, action);
        }

        private void ApplyChangeGraphicsApiAction(BuildTarget target, GraphicsDeviceType[] apis, ChangeGraphicsApiAction action)
        {
            if (action.changeList)
            {
                PlayerSettings.SetGraphicsAPIs(target, apis);
            }
            else
            {
                s_GraphicsDeviceLists.Remove(target);
            }
            if (action.reloadGfx)
            {
                ShaderUtil.RecreateGfxDevice();
                GUIUtility.ExitGUI();
            }
        }

        private void AutoAssignProperty(SerializedProperty property, string packageDir, string fileName)
        {
            if (((property.stringValue.Length == 0) || !File.Exists(Path.Combine(packageDir, property.stringValue))) && File.Exists(Path.Combine(packageDir, fileName)))
            {
                property.stringValue = fileName;
            }
        }

        public bool BeginSettingsBox(int nr, GUIContent header)
        {
            bool enabled = GUI.enabled;
            GUI.enabled = true;
            EditorGUILayout.BeginVertical(Styles.categoryBox, new GUILayoutOption[0]);
            Rect position = GUILayoutUtility.GetRect((float) 20f, (float) 18f);
            position.x += 3f;
            position.width += 6f;
            EditorGUI.BeginChangeCheck();
            bool flag2 = GUI.Toggle(position, this.m_SelectedSection.value == nr, header, EditorStyles.inspectorTitlebarText);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_SelectedSection.value = !flag2 ? -1 : nr;
                GUIUtility.keyboardControl = 0;
            }
            this.m_SectionAnimators[nr].target = flag2;
            GUI.enabled = enabled;
            return EditorGUILayout.BeginFadeGroup(this.m_SectionAnimators[nr].faded);
        }

        public void BrowseablePathProperty(string propertyLabel, SerializedProperty property, string browsePanelTitle, string extension, string dir)
        {
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUILayout.PrefixLabel(EditorGUIUtility.TextContent(propertyLabel));
            GUIContent content = new GUIContent("...");
            Vector2 vector = GUI.skin.GetStyle("Button").CalcSize(content);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MaxWidth(vector.x) };
            if (GUILayout.Button(content, EditorStyles.miniButton, options))
            {
                GUI.FocusControl("");
                string text = EditorGUIUtility.TempContent(browsePanelTitle).text;
                string folder = !string.IsNullOrEmpty(dir) ? (dir.Replace('\\', '/') + "/") : (Directory.GetCurrentDirectory().Replace('\\', '/') + "/");
                string str3 = "";
                if (string.IsNullOrEmpty(extension))
                {
                    str3 = EditorUtility.OpenFolderPanel(text, folder, "");
                }
                else
                {
                    str3 = EditorUtility.OpenFilePanel(text, folder, extension);
                }
                if (str3.StartsWith(folder))
                {
                    str3 = str3.Substring(folder.Length);
                }
                if (!string.IsNullOrEmpty(str3))
                {
                    property.stringValue = str3;
                    base.serializedObject.ApplyModifiedProperties();
                    GUIUtility.ExitGUI();
                }
            }
            GUIContent content2 = null;
            bool disabled = string.IsNullOrEmpty(property.stringValue);
            using (new EditorGUI.DisabledScope(disabled))
            {
                if (disabled)
                {
                    content2 = EditorGUIUtility.TextContent("Not selected.");
                }
                else
                {
                    content2 = EditorGUIUtility.TempContent(property.stringValue);
                }
                EditorGUI.BeginChangeCheck();
                GUILayoutOption[] optionArray = new GUILayoutOption[] { GUILayout.Width(32f), GUILayout.ExpandWidth(true) };
                string str4 = EditorGUILayout.TextArea(content2.text, optionArray);
                if (EditorGUI.EndChangeCheck() && string.IsNullOrEmpty(str4))
                {
                    property.stringValue = "";
                    base.serializedObject.ApplyModifiedProperties();
                    GUI.FocusControl("");
                    GUIUtility.ExitGUI();
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        public static void BuildDisabledEnumPopup(GUIContent selected, GUIContent uiString)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                GUIContent[] displayedOptions = new GUIContent[] { selected };
                EditorGUI.Popup(EditorGUILayout.GetControlRect(true, new GUILayoutOption[0]), uiString, 0, displayedOptions);
            }
        }

        public static void BuildEnumPopup<T>(SerializedProperty prop, GUIContent uiString, T[] options, GUIContent[] optionNames)
        {
            T intValue = (T) prop.intValue;
            T local2 = BuildEnumPopup<T>(uiString, intValue, options, optionNames);
            if (!local2.Equals(intValue))
            {
                prop.intValue = (int) local2;
                prop.serializedObject.ApplyModifiedProperties();
            }
        }

        public static T BuildEnumPopup<T>(GUIContent uiString, T selected, T[] options, GUIContent[] optionNames)
        {
            int selectedIndex = 0;
            for (int i = 1; i < options.Length; i++)
            {
                if (selected.Equals(options[i]))
                {
                    selectedIndex = i;
                    break;
                }
            }
            int index = EditorGUILayout.Popup(uiString, selectedIndex, optionNames, new GUILayoutOption[0]);
            return options[index];
        }

        internal static void BuildFileBoxButton(SerializedProperty prop, string uiString, string directory, string ext)
        {
            BuildFileBoxButton(prop, uiString, directory, ext, null);
        }

        internal static void BuildFileBoxButton(SerializedProperty prop, string uiString, string directory, string ext, Action onSelect)
        {
            float minHeight = 16f;
            float minWidth = (80f + EditorGUIUtility.fieldWidth) + 5f;
            float maxWidth = (80f + EditorGUIUtility.fieldWidth) + 5f;
            Rect rect = GUILayoutUtility.GetRect(minWidth, maxWidth, minHeight, minHeight, EditorStyles.layerMaskField, null);
            float labelWidth = EditorGUIUtility.labelWidth;
            Rect position = new Rect(rect.x + EditorGUI.indent, rect.y, labelWidth - EditorGUI.indent, rect.height);
            Rect rect3 = new Rect(rect.x + labelWidth, rect.y, rect.width - labelWidth, rect.height);
            string text = (prop.stringValue.Length != 0) ? prop.stringValue : "Not selected.";
            EditorGUI.TextArea(rect3, text, EditorStyles.label);
            if (GUI.Button(position, EditorGUIUtility.TextContent(uiString)))
            {
                string path = EditorUtility.OpenFilePanel(EditorGUIUtility.TextContent(uiString).text, directory, ext);
                string projectRelativePath = FileUtil.GetProjectRelativePath(path);
                prop.stringValue = (projectRelativePath == string.Empty) ? path : projectRelativePath;
                if (onSelect != null)
                {
                    onSelect();
                }
                prop.serializedObject.ApplyModifiedProperties();
                GUIUtility.ExitGUI();
            }
        }

        private bool CanRemoveGraphicsDeviceElement(ReorderableList list) => 
            (list.list.Count >= 2);

        private ChangeGraphicsApiAction CheckApplyGraphicsAPIList(BuildTarget target, bool firstEntryChanged)
        {
            bool doChange = true;
            bool doReload = false;
            if (firstEntryChanged && WillEditorUseFirstGraphicsAPI(target))
            {
                doChange = false;
                if (EditorUtility.DisplayDialog("Changing editor graphics device", "Changing active graphics API requires reloading all graphics objects, it might take a while", "Apply", "Cancel") && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    doChange = doReload = true;
                }
            }
            return new ChangeGraphicsApiAction(doChange, doReload);
        }

        private void CommonSettings()
        {
            EditorGUILayout.PropertyField(this.m_CompanyName, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_ProductName, new GUILayoutOption[0]);
            EditorGUILayout.Space();
            GUI.changed = false;
            string platform = "";
            Texture2D[] iconsForPlatform = PlayerSettings.GetIconsForPlatform(platform);
            int[] iconWidthsForPlatform = PlayerSettings.GetIconWidthsForPlatform(platform);
            if (iconsForPlatform.Length != iconWidthsForPlatform.Length)
            {
                iconsForPlatform = new Texture2D[iconWidthsForPlatform.Length];
            }
            iconsForPlatform[0] = (Texture2D) EditorGUILayout.ObjectField(Styles.defaultIcon, iconsForPlatform[0], typeof(Texture2D), false, new GUILayoutOption[0]);
            if (GUI.changed)
            {
                Undo.RecordObject(base.target, Styles.undoChangedIconString);
                PlayerSettings.SetIconsForPlatform(platform, iconsForPlatform);
            }
            GUILayout.Space(3f);
            Rect totalPosition = EditorGUILayout.GetControlRect(true, 64f, new GUILayoutOption[0]);
            EditorGUI.BeginProperty(totalPosition, Styles.defaultCursor, this.m_DefaultCursor);
            this.m_DefaultCursor.objectReferenceValue = EditorGUI.ObjectField(totalPosition, Styles.defaultCursor, this.m_DefaultCursor.objectReferenceValue, typeof(Texture2D), false);
            EditorGUI.EndProperty();
            EditorGUI.PropertyField(EditorGUI.PrefixLabel(EditorGUILayout.GetControlRect(new GUILayoutOption[0]), 0, Styles.cursorHotspot), this.m_CursorHotspot, GUIContent.none);
        }

        public void DebugAndCrashReportingGUI(BuildPlatform platform, BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
        {
            if ((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS))
            {
                if (this.BeginSettingsBox(3, Styles.debuggingCrashReportingTitle))
                {
                    GUILayout.Label(Styles.debuggingTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_EnableInternalProfiler, Styles.enableInternalProfiler, new GUILayoutOption[0]);
                    EditorGUILayout.Space();
                    GUILayout.Label(Styles.crashReportingTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_ActionOnDotNetUnhandledException, Styles.actionOnDotNetUnhandledException, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_LogObjCUncaughtExceptions, Styles.logObjCUncaughtExceptions, new GUILayoutOption[0]);
                    GUIContent enableCrashReportAPI = Styles.enableCrashReportAPI;
                    bool disabled = false;
                    if (CrashReportingSettings.enabled)
                    {
                        enableCrashReportAPI = new GUIContent(enableCrashReportAPI);
                        disabled = true;
                        enableCrashReportAPI.tooltip = "CrashReport API must be enabled for Performance Reporting service.";
                        this.m_EnableCrashReportAPI.boolValue = true;
                    }
                    EditorGUI.BeginDisabledGroup(disabled);
                    EditorGUILayout.PropertyField(this.m_EnableCrashReportAPI, enableCrashReportAPI, new GUILayoutOption[0]);
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.Space();
                }
                this.EndSettingsBox();
            }
        }

        private void DrawGraphicsDeviceElement(BuildTarget target, Rect rect, int index, bool selected, bool focused)
        {
            string text = s_GraphicsDeviceLists[target].list[index].ToString();
            if (text == "Direct3D12")
            {
                text = "Direct3D12 (Experimental)";
            }
            if ((text == "Vulkan") && (target != BuildTarget.Android))
            {
                text = "Vulkan (Experimental)";
            }
            if (target == BuildTarget.WebGL)
            {
                if (text == "OpenGLES3")
                {
                    text = "WebGL 2.0";
                }
                else if (text == "OpenGLES2")
                {
                    text = "WebGL 1.0";
                }
            }
            GUI.Label(rect, text, EditorStyles.label);
        }

        public void EndSettingsBox()
        {
            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.EndVertical();
        }

        public SerializedProperty FindPropertyAssert(string name)
        {
            SerializedProperty property = base.serializedObject.FindProperty(name);
            if (property == null)
            {
                Debug.LogError("Failed to find:" + name);
            }
            return property;
        }

        private ApiCompatibilityLevel[] GetAvailableApiCompatibilityLevels(BuildTargetGroup activeBuildTargetGroup)
        {
            if ((activeBuildTargetGroup == BuildTargetGroup.WSA) || (activeBuildTargetGroup == BuildTargetGroup.XboxOne))
            {
                return allProfiles;
            }
            return only_2_0_profiles;
        }

        private static GUIContent[] GetGUIContentsForValues<T>(Dictionary<T, GUIContent> contents, T[] values)
        {
            GUIContent[] contentArray = new GUIContent[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                if (!contents.ContainsKey(values[i]))
                {
                    throw new NotImplementedException($"Missing name for {values[i]}");
                }
                contentArray[i] = contents[values[i]];
            }
            return contentArray;
        }

        private static GUIContent[] GetNiceApiCompatibilityLevelNames(ApiCompatibilityLevel[] apiCompatibilityLevels)
        {
            if (m_NiceApiCompatibilityLevelNames == null)
            {
                Dictionary<ApiCompatibilityLevel, GUIContent> dictionary = new Dictionary<ApiCompatibilityLevel, GUIContent> {
                    { 
                        ApiCompatibilityLevel.NET_2_0,
                        Styles.apiCompatibilityLevel_NET_2_0
                    },
                    { 
                        ApiCompatibilityLevel.NET_2_0_Subset,
                        Styles.apiCompatibilityLevel_NET_2_0_Subset
                    },
                    { 
                        ApiCompatibilityLevel.NET_4_6,
                        Styles.apiCompatibilityLevel_NET_4_6
                    }
                };
                m_NiceApiCompatibilityLevelNames = dictionary;
            }
            return GetGUIContentsForValues<ApiCompatibilityLevel>(m_NiceApiCompatibilityLevelNames, apiCompatibilityLevels);
        }

        private static GUIContent[] GetNiceScriptingBackendNames(ScriptingImplementation[] scriptingBackends)
        {
            if (m_NiceScriptingBackendNames == null)
            {
                Dictionary<ScriptingImplementation, GUIContent> dictionary = new Dictionary<ScriptingImplementation, GUIContent> {
                    { 
                        ScriptingImplementation.Mono2x,
                        Styles.scriptingMono2x
                    },
                    { 
                        ScriptingImplementation.WinRTDotNET,
                        Styles.scriptingWinRTDotNET
                    },
                    { 
                        ScriptingImplementation.IL2CPP,
                        Styles.scriptingIL2CPP
                    }
                };
                m_NiceScriptingBackendNames = dictionary;
            }
            return GetGUIContentsForValues<ScriptingImplementation>(m_NiceScriptingBackendNames, scriptingBackends);
        }

        private void GraphicsAPIsGUI(BuildTargetGroup targetGroup, BuildTarget target)
        {
            if (targetGroup == BuildTargetGroup.Standalone)
            {
                this.GraphicsAPIsGUIOnePlatform(targetGroup, BuildTarget.StandaloneWindows, " for Windows");
                this.GraphicsAPIsGUIOnePlatform(targetGroup, BuildTarget.StandaloneOSXUniversal, " for Mac");
                this.GraphicsAPIsGUIOnePlatform(targetGroup, BuildTarget.StandaloneLinuxUniversal, " for Linux");
            }
            else
            {
                this.GraphicsAPIsGUIOnePlatform(targetGroup, target, null);
            }
        }

        private void GraphicsAPIsGUIOnePlatform(BuildTargetGroup targetGroup, BuildTarget targetPlatform, string platformTitle)
        {
            <GraphicsAPIsGUIOnePlatform>c__AnonStorey0 storey = new <GraphicsAPIsGUIOnePlatform>c__AnonStorey0 {
                targetPlatform = targetPlatform,
                $this = this
            };
            GraphicsDeviceType[] supportedGraphicsAPIs = PlayerSettings.GetSupportedGraphicsAPIs(storey.targetPlatform);
            if ((supportedGraphicsAPIs != null) && (supportedGraphicsAPIs.Length >= 2))
            {
                EditorGUI.BeginChangeCheck();
                bool useDefaultGraphicsAPIs = PlayerSettings.GetUseDefaultGraphicsAPIs(storey.targetPlatform);
                if (platformTitle == null)
                {
                }
                useDefaultGraphicsAPIs = EditorGUILayout.Toggle("Auto Graphics API" + string.Empty, useDefaultGraphicsAPIs, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(base.target, Styles.undoChangedGraphicsAPIString);
                    PlayerSettings.SetUseDefaultGraphicsAPIs(storey.targetPlatform, useDefaultGraphicsAPIs);
                }
                if (!useDefaultGraphicsAPIs)
                {
                    <GraphicsAPIsGUIOnePlatform>c__AnonStorey1 storey2 = new <GraphicsAPIsGUIOnePlatform>c__AnonStorey1 {
                        <>f__ref$0 = storey
                    };
                    if (WillEditorUseFirstGraphicsAPI(storey.targetPlatform))
                    {
                        EditorGUILayout.HelpBox(Styles.recordingInfo.text, MessageType.Info, true);
                    }
                    storey2.displayTitle = "Graphics APIs";
                    if (platformTitle != null)
                    {
                        storey2.displayTitle = storey2.displayTitle + platformTitle;
                    }
                    if (!s_GraphicsDeviceLists.ContainsKey(storey.targetPlatform))
                    {
                        GraphicsDeviceType[] graphicsAPIs = PlayerSettings.GetGraphicsAPIs(storey.targetPlatform);
                        List<GraphicsDeviceType> elements = (graphicsAPIs == null) ? new List<GraphicsDeviceType>() : graphicsAPIs.ToList<GraphicsDeviceType>();
                        ReorderableList list2 = new ReorderableList(elements, typeof(GraphicsDeviceType), true, true, true, true) {
                            onAddDropdownCallback = new ReorderableList.AddDropdownCallbackDelegate(storey2.<>m__0),
                            onCanRemoveCallback = new ReorderableList.CanRemoveCallbackDelegate(this.CanRemoveGraphicsDeviceElement),
                            onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(storey2.<>m__1),
                            onReorderCallback = new ReorderableList.ReorderCallbackDelegate(storey2.<>m__2),
                            drawElementCallback = new ReorderableList.ElementCallbackDelegate(storey2.<>m__3),
                            drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(storey2.<>m__4),
                            elementHeight = 16f
                        };
                        s_GraphicsDeviceLists.Add(storey.targetPlatform, list2);
                    }
                    s_GraphicsDeviceLists[storey.targetPlatform].DoLayoutList();
                    this.OpenGLES31OptionsGUI(targetGroup, storey.targetPlatform);
                }
            }
        }

        private void IconSectionGUI(BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
        {
            if (this.BeginSettingsBox(1, Styles.iconTitle))
            {
                bool flag = true;
                if (settingsExtension != null)
                {
                    flag = settingsExtension.UsesStandardIcons();
                }
                if (flag)
                {
                    bool flag2 = this.selectedPlatform < 0;
                    BuildPlatform platform = null;
                    targetGroup = BuildTargetGroup.Standalone;
                    string name = "";
                    if (!flag2)
                    {
                        platform = this.validPlatforms[this.selectedPlatform];
                        targetGroup = platform.targetGroup;
                        name = platform.name;
                    }
                    bool enabled = GUI.enabled;
                    if ((targetGroup == BuildTargetGroup.SamsungTV) || (targetGroup == BuildTargetGroup.WebGL))
                    {
                        this.ShowNoSettings();
                        EditorGUILayout.Space();
                    }
                    else if (targetGroup != BuildTargetGroup.WSA)
                    {
                        Texture2D[] iconsForPlatform = PlayerSettings.GetIconsForPlatform(name);
                        int[] iconWidthsForPlatform = PlayerSettings.GetIconWidthsForPlatform(name);
                        int[] iconHeightsForPlatform = PlayerSettings.GetIconHeightsForPlatform(name);
                        bool flag4 = true;
                        if (!flag2)
                        {
                            GUI.changed = false;
                            flag4 = iconsForPlatform.Length == iconWidthsForPlatform.Length;
                            flag4 = GUILayout.Toggle(flag4, "Override for " + platform.title.text, new GUILayoutOption[0]);
                            GUI.enabled = enabled && flag4;
                            if (GUI.changed || (!flag4 && (iconsForPlatform.Length > 0)))
                            {
                                if (flag4)
                                {
                                    iconsForPlatform = new Texture2D[iconWidthsForPlatform.Length];
                                }
                                else
                                {
                                    iconsForPlatform = new Texture2D[0];
                                }
                                if (GUI.changed)
                                {
                                    PlayerSettings.SetIconsForPlatform(name, iconsForPlatform);
                                }
                            }
                        }
                        GUI.changed = false;
                        for (int i = 0; i < iconWidthsForPlatform.Length; i++)
                        {
                            int num2 = Mathf.Min(0x60, iconWidthsForPlatform[i]);
                            int b = (int) ((iconHeightsForPlatform[i] * num2) / ((float) iconWidthsForPlatform[i]));
                            if (targetGroup == BuildTargetGroup.iPhone)
                            {
                                if (((i + 1) < iconWidthsForPlatform.Length) && (iconWidthsForPlatform[i + 1] == 80))
                                {
                                    Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.labelWidth, 20f);
                                    GUI.Label(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, 20f), "Spotlight icons", EditorStyles.boldLabel);
                                }
                                if (iconWidthsForPlatform[i] == 0x57)
                                {
                                    Rect rect2 = GUILayoutUtility.GetRect(EditorGUIUtility.labelWidth, 20f);
                                    GUI.Label(new Rect(rect2.x, rect2.y, EditorGUIUtility.labelWidth, 20f), "Settings icons", EditorStyles.boldLabel);
                                }
                            }
                            Rect rect3 = GUILayoutUtility.GetRect(64f, (float) (Mathf.Max(0x40, b) + 6));
                            float num4 = Mathf.Min(rect3.width, (((EditorGUIUtility.labelWidth + 4f) + 64f) + 6f) + 96f);
                            string text = iconWidthsForPlatform[i] + "x" + iconHeightsForPlatform[i];
                            GUI.Label(new Rect(rect3.x, rect3.y, ((num4 - 96f) - 64f) - 12f, 20f), text);
                            if (flag4)
                            {
                                int num5 = 0x40;
                                int num6 = (int) ((((float) iconHeightsForPlatform[i]) / ((float) iconWidthsForPlatform[i])) * 64f);
                                iconsForPlatform[i] = (Texture2D) EditorGUI.ObjectField(new Rect((((rect3.x + num4) - 96f) - 64f) - 6f, rect3.y, (float) num5, (float) num6), iconsForPlatform[i], typeof(Texture2D), false);
                            }
                            Rect position = new Rect((rect3.x + num4) - 96f, rect3.y, (float) num2, (float) b);
                            Texture2D image = PlayerSettings.GetIconForPlatformAtSize(name, iconWidthsForPlatform[i], iconHeightsForPlatform[i]);
                            if (image != null)
                            {
                                GUI.DrawTexture(position, image);
                            }
                            else
                            {
                                GUI.Box(position, "");
                            }
                        }
                        if (GUI.changed)
                        {
                            Undo.RecordObject(base.target, Styles.undoChangedIconString);
                            PlayerSettings.SetIconsForPlatform(name, iconsForPlatform);
                        }
                        GUI.enabled = enabled;
                        if ((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS))
                        {
                            EditorGUILayout.PropertyField(this.m_UIPrerenderedIcon, Styles.UIPrerenderedIcon, new GUILayoutOption[0]);
                            EditorGUILayout.Space();
                        }
                    }
                }
                if (settingsExtension != null)
                {
                    settingsExtension.IconSectionGUI();
                }
            }
            this.EndSettingsBox();
        }

        private bool IsMobileTarget(BuildTargetGroup targetGroup) => 
            ((((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS)) || ((targetGroup == BuildTargetGroup.Android) || (targetGroup == BuildTargetGroup.Tizen))) || (targetGroup == BuildTargetGroup.SamsungTV));

        private void OnEnable()
        {
            this.validPlatforms = BuildPlatforms.instance.GetValidPlatforms(true).ToArray();
            this.m_IPhoneSdkVersion = this.FindPropertyAssert("iPhoneSdkVersion");
            this.m_IPhoneTargetOSVersion = this.FindPropertyAssert("iOSTargetOSVersionString");
            this.m_IPhoneStrippingLevel = this.FindPropertyAssert("iPhoneStrippingLevel");
            this.m_StripEngineCode = this.FindPropertyAssert("stripEngineCode");
            this.m_tvOSSdkVersion = this.FindPropertyAssert("tvOSSdkVersion");
            this.m_tvOSTargetOSVersion = this.FindPropertyAssert("tvOSTargetOSVersionString");
            this.m_IPhoneScriptCallOptimization = this.FindPropertyAssert("iPhoneScriptCallOptimization");
            this.m_AndroidProfiler = this.FindPropertyAssert("AndroidProfiler");
            this.m_CompanyName = this.FindPropertyAssert("companyName");
            this.m_ProductName = this.FindPropertyAssert("productName");
            this.m_DefaultCursor = this.FindPropertyAssert("defaultCursor");
            this.m_CursorHotspot = this.FindPropertyAssert("cursorHotspot");
            this.m_UIPrerenderedIcon = this.FindPropertyAssert("uIPrerenderedIcon");
            this.m_UIRequiresFullScreen = this.FindPropertyAssert("uIRequiresFullScreen");
            this.m_UIStatusBarHidden = this.FindPropertyAssert("uIStatusBarHidden");
            this.m_UIStatusBarStyle = this.FindPropertyAssert("uIStatusBarStyle");
            this.m_StereoRenderingPath = this.FindPropertyAssert("m_StereoRenderingPath");
            this.m_ActiveColorSpace = this.FindPropertyAssert("m_ActiveColorSpace");
            this.m_MTRendering = this.FindPropertyAssert("m_MTRendering");
            this.m_MobileMTRendering = this.FindPropertyAssert("m_MobileMTRendering");
            this.m_StripUnusedMeshComponents = this.FindPropertyAssert("StripUnusedMeshComponents");
            this.m_VertexChannelCompressionMask = this.FindPropertyAssert("VertexChannelCompressionMask");
            this.m_MetalForceHardShadows = this.FindPropertyAssert("iOSMetalForceHardShadows");
            this.m_MetalEditorSupport = this.FindPropertyAssert("metalEditorSupport");
            this.m_MetalAPIValidation = this.FindPropertyAssert("metalAPIValidation");
            this.m_ApplicationBundleVersion = base.serializedObject.FindProperty("bundleVersion");
            if (this.m_ApplicationBundleVersion == null)
            {
                this.m_ApplicationBundleVersion = this.FindPropertyAssert("iPhoneBundleVersion");
            }
            this.m_useOnDemandResources = this.FindPropertyAssert("useOnDemandResources");
            this.m_AccelerometerFrequency = this.FindPropertyAssert("accelerometerFrequency");
            this.m_MuteOtherAudioSources = this.FindPropertyAssert("muteOtherAudioSources");
            this.m_PrepareIOSForRecording = this.FindPropertyAssert("Prepare IOS For Recording");
            this.m_UIRequiresPersistentWiFi = this.FindPropertyAssert("uIRequiresPersistentWiFi");
            this.m_IOSAllowHTTPDownload = this.FindPropertyAssert("iosAllowHTTPDownload");
            this.m_SubmitAnalytics = this.FindPropertyAssert("submitAnalytics");
            this.m_IOSURLSchemes = this.FindPropertyAssert("iOSURLSchemes");
            this.m_AotOptions = this.FindPropertyAssert("aotOptions");
            this.m_CameraUsageDescription = this.FindPropertyAssert("cameraUsageDescription");
            this.m_LocationUsageDescription = this.FindPropertyAssert("locationUsageDescription");
            this.m_MicrophoneUsageDescription = this.FindPropertyAssert("microphoneUsageDescription");
            this.m_EnableInternalProfiler = this.FindPropertyAssert("enableInternalProfiler");
            this.m_ActionOnDotNetUnhandledException = this.FindPropertyAssert("actionOnDotNetUnhandledException");
            this.m_LogObjCUncaughtExceptions = this.FindPropertyAssert("logObjCUncaughtExceptions");
            this.m_EnableCrashReportAPI = this.FindPropertyAssert("enableCrashReportAPI");
            this.m_DefaultScreenWidth = this.FindPropertyAssert("defaultScreenWidth");
            this.m_DefaultScreenHeight = this.FindPropertyAssert("defaultScreenHeight");
            this.m_RunInBackground = this.FindPropertyAssert("runInBackground");
            this.m_DefaultScreenOrientation = this.FindPropertyAssert("defaultScreenOrientation");
            this.m_AllowedAutoRotateToPortrait = this.FindPropertyAssert("allowedAutorotateToPortrait");
            this.m_AllowedAutoRotateToPortraitUpsideDown = this.FindPropertyAssert("allowedAutorotateToPortraitUpsideDown");
            this.m_AllowedAutoRotateToLandscapeRight = this.FindPropertyAssert("allowedAutorotateToLandscapeRight");
            this.m_AllowedAutoRotateToLandscapeLeft = this.FindPropertyAssert("allowedAutorotateToLandscapeLeft");
            this.m_UseOSAutoRotation = this.FindPropertyAssert("useOSAutorotation");
            this.m_Use32BitDisplayBuffer = this.FindPropertyAssert("use32BitDisplayBuffer");
            this.m_DisableDepthAndStencilBuffers = this.FindPropertyAssert("disableDepthAndStencilBuffers");
            this.m_iosShowActivityIndicatorOnLoading = this.FindPropertyAssert("iosShowActivityIndicatorOnLoading");
            this.m_androidShowActivityIndicatorOnLoading = this.FindPropertyAssert("androidShowActivityIndicatorOnLoading");
            this.m_tizenShowActivityIndicatorOnLoading = this.FindPropertyAssert("tizenShowActivityIndicatorOnLoading");
            this.m_DefaultIsFullScreen = this.FindPropertyAssert("defaultIsFullScreen");
            this.m_DefaultIsNativeResolution = this.FindPropertyAssert("defaultIsNativeResolution");
            this.m_CaptureSingleScreen = this.FindPropertyAssert("captureSingleScreen");
            this.m_DisplayResolutionDialog = this.FindPropertyAssert("displayResolutionDialog");
            this.m_SupportedAspectRatios = this.FindPropertyAssert("m_SupportedAspectRatios");
            this.m_TargetDevice = this.FindPropertyAssert("targetDevice");
            this.m_UsePlayerLog = this.FindPropertyAssert("usePlayerLog");
            this.m_KeepLoadedShadersAlive = this.FindPropertyAssert("keepLoadedShadersAlive");
            this.m_PreloadedAssets = this.FindPropertyAssert("preloadedAssets");
            this.m_BakeCollisionMeshes = this.FindPropertyAssert("bakeCollisionMeshes");
            this.m_ResizableWindow = this.FindPropertyAssert("resizableWindow");
            this.m_UseMacAppStoreValidation = this.FindPropertyAssert("useMacAppStoreValidation");
            this.m_D3D9FullscreenMode = this.FindPropertyAssert("d3d9FullscreenMode");
            this.m_D3D11FullscreenMode = this.FindPropertyAssert("d3d11FullscreenMode");
            this.m_VisibleInBackground = this.FindPropertyAssert("visibleInBackground");
            this.m_AllowFullscreenSwitch = this.FindPropertyAssert("allowFullscreenSwitch");
            this.m_MacFullscreenMode = this.FindPropertyAssert("macFullscreenMode");
            this.m_SkinOnGPU = this.FindPropertyAssert("gpuSkinning");
            this.m_GraphicsJobs = this.FindPropertyAssert("graphicsJobs");
            this.m_ForceSingleInstance = this.FindPropertyAssert("forceSingleInstance");
            this.m_RequireES31 = this.FindPropertyAssert("openGLRequireES31");
            this.m_RequireES31AEP = this.FindPropertyAssert("openGLRequireES31AEP");
            this.m_VideoMemoryForVertexBuffers = this.FindPropertyAssert("videoMemoryForVertexBuffers");
            this.m_SettingsExtensions = new ISettingEditorExtension[this.validPlatforms.Length];
            for (int i = 0; i < this.validPlatforms.Length; i++)
            {
                string targetStringFromBuildTargetGroup = ModuleManager.GetTargetStringFromBuildTargetGroup(this.validPlatforms[i].targetGroup);
                this.m_SettingsExtensions[i] = ModuleManager.GetEditorSettingsExtension(targetStringFromBuildTargetGroup);
                if (this.m_SettingsExtensions[i] != null)
                {
                    this.m_SettingsExtensions[i].OnEnable(this);
                }
            }
            for (int j = 0; j < this.m_SectionAnimators.Length; j++)
            {
                this.m_SectionAnimators[j] = new AnimBool(this.m_SelectedSection.value == j, new UnityAction(this.Repaint));
            }
            this.m_ShowDefaultIsNativeResolution.value = this.m_DefaultIsFullScreen.boolValue;
            this.m_ShowResolution.value = !(this.m_DefaultIsFullScreen.boolValue && this.m_DefaultIsNativeResolution.boolValue);
            this.m_ShowDefaultIsNativeResolution.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowResolution.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_VRSettings = new PlayerSettingsEditorVR(base.serializedObject);
            this.splashScreenEditor.OnEnable();
            s_GraphicsDeviceLists.Clear();
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins, new GUILayoutOption[0]);
            this.CommonSettings();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            int selectedPlatform = this.selectedPlatform;
            this.selectedPlatform = EditorGUILayout.BeginPlatformGrouping(this.validPlatforms, null);
            if (EditorGUI.EndChangeCheck())
            {
                if (EditorGUI.s_DelayedTextEditor.IsEditingControl(this.scriptingDefinesControlID))
                {
                    EditorGUI.EndEditingActiveTextField();
                    GUIUtility.keyboardControl = 0;
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(this.validPlatforms[selectedPlatform].targetGroup, EditorGUI.s_DelayedTextEditor.text);
                }
                GUI.FocusControl("");
            }
            GUILayout.Label("Settings for " + this.validPlatforms[this.selectedPlatform].title.text, new GUILayoutOption[0]);
            EditorGUIUtility.labelWidth = Mathf.Max((float) 150f, (float) (EditorGUIUtility.labelWidth - 8f));
            BuildPlatform platform = this.validPlatforms[this.selectedPlatform];
            BuildTargetGroup targetGroup = platform.targetGroup;
            this.ResolutionSectionGUI(targetGroup, this.m_SettingsExtensions[this.selectedPlatform]);
            this.IconSectionGUI(targetGroup, this.m_SettingsExtensions[this.selectedPlatform]);
            this.m_SplashScreenEditor.SplashSectionGUI(platform, targetGroup, this.m_SettingsExtensions[this.selectedPlatform]);
            this.DebugAndCrashReportingGUI(platform, targetGroup, this.m_SettingsExtensions[this.selectedPlatform]);
            this.OtherSectionGUI(platform, targetGroup, this.m_SettingsExtensions[this.selectedPlatform]);
            this.PublishSectionGUI(targetGroup, this.m_SettingsExtensions[this.selectedPlatform]);
            EditorGUILayout.EndPlatformGrouping();
            base.serializedObject.ApplyModifiedProperties();
        }

        private void OpenGLES31OptionsGUI(BuildTargetGroup targetGroup, BuildTarget targetPlatform)
        {
            if (targetGroup == BuildTargetGroup.Android)
            {
                GraphicsDeviceType[] graphicsAPIs = PlayerSettings.GetGraphicsAPIs(targetPlatform);
                if (graphicsAPIs.Contains<GraphicsDeviceType>(GraphicsDeviceType.OpenGLES3) && !graphicsAPIs.Contains<GraphicsDeviceType>(GraphicsDeviceType.OpenGLES2))
                {
                    EditorGUILayout.PropertyField(this.m_RequireES31, Styles.require31, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_RequireES31AEP, Styles.requireAEP, new GUILayoutOption[0]);
                }
            }
        }

        private void OtherSectionConfigurationGUI(BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
        {
            GUILayout.Label(Styles.configurationTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
            IScriptingImplementations scriptingImplementations = ModuleManager.GetScriptingImplementations(targetGroup);
            if (scriptingImplementations == null)
            {
                BuildDisabledEnumPopup(Styles.scriptingDefault, Styles.scriptingBackend);
            }
            else
            {
                ScriptingImplementation implementation2;
                ScriptingImplementation[] options = scriptingImplementations.Enabled();
                ScriptingImplementation scriptingBackend = PlayerSettings.GetScriptingBackend(targetGroup);
                if (targetGroup == BuildTargetGroup.tvOS)
                {
                    implementation2 = ScriptingImplementation.IL2CPP;
                    BuildDisabledEnumPopup(Styles.scriptingIL2CPP, Styles.scriptingBackend);
                }
                else
                {
                    implementation2 = BuildEnumPopup<ScriptingImplementation>(Styles.scriptingBackend, scriptingBackend, options, GetNiceScriptingBackendNames(options));
                }
                if (implementation2 != scriptingBackend)
                {
                    PlayerSettings.SetScriptingBackend(targetGroup, implementation2);
                }
            }
            if (targetGroup == BuildTargetGroup.WiiU)
            {
                BuildDisabledEnumPopup(Styles.apiCompatibilityLevel_WiiUSubset, Styles.apiCompatibilityLevel);
            }
            else
            {
                ApiCompatibilityLevel apiCompatibilityLevel = PlayerSettings.GetApiCompatibilityLevel(targetGroup);
                ApiCompatibilityLevel[] availableApiCompatibilityLevels = this.GetAvailableApiCompatibilityLevels(targetGroup);
                ApiCompatibilityLevel level2 = BuildEnumPopup<ApiCompatibilityLevel>(Styles.apiCompatibilityLevel, apiCompatibilityLevel, availableApiCompatibilityLevels, GetNiceApiCompatibilityLevelNames(availableApiCompatibilityLevels));
                if (apiCompatibilityLevel != level2)
                {
                    PlayerSettings.SetApiCompatibilityLevel(targetGroup, level2);
                }
            }
            if ((((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS)) || (targetGroup == BuildTargetGroup.Android)) || (targetGroup == BuildTargetGroup.WSA))
            {
                if (targetGroup == BuildTargetGroup.iPhone)
                {
                    EditorGUILayout.PropertyField(this.m_TargetDevice, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_IPhoneSdkVersion, Styles.targetSdkVersion, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_IPhoneTargetOSVersion, Styles.iPhoneTargetOSVersion, new GUILayoutOption[0]);
                    Version version = this.ParseIosVersion(this.m_IPhoneTargetOSVersion.stringValue);
                    this.m_IPhoneTargetOSVersion.stringValue = (version.Major != 0) ? version.ToString() : "7.0";
                }
                if (targetGroup == BuildTargetGroup.tvOS)
                {
                    EditorGUILayout.PropertyField(this.m_tvOSSdkVersion, Styles.targetSdkVersion, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_tvOSTargetOSVersion, Styles.tvOSTargetOSVersion, new GUILayoutOption[0]);
                    Version version2 = this.ParseIosVersion(this.m_IPhoneTargetOSVersion.stringValue);
                    this.m_IPhoneTargetOSVersion.stringValue = (version2.Major != 0) ? version2.ToString() : "7.0";
                }
                if ((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS))
                {
                    EditorGUILayout.PropertyField(this.m_useOnDemandResources, Styles.useOnDemandResources, new GUILayoutOption[0]);
                    Version version3 = this.ParseIosVersion(this.m_IPhoneTargetOSVersion.stringValue);
                    this.m_IPhoneTargetOSVersion.stringValue = (!this.m_useOnDemandResources.boolValue || (version3.Major >= 9)) ? version3.ToString() : "9.0";
                }
                if (((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS)) || (targetGroup == BuildTargetGroup.WSA))
                {
                    EditorGUILayout.PropertyField(this.m_AccelerometerFrequency, Styles.accelerometerFrequency, new GUILayoutOption[0]);
                }
                if ((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS))
                {
                    EditorGUILayout.PropertyField(this.m_CameraUsageDescription, Styles.cameraUsageDescription, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_LocationUsageDescription, Styles.locationUsageDescription, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_MicrophoneUsageDescription, Styles.microphoneUsageDescription, new GUILayoutOption[0]);
                }
                if (((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS)) || (targetGroup == BuildTargetGroup.Android))
                {
                    EditorGUILayout.PropertyField(this.m_MuteOtherAudioSources, Styles.muteOtherAudioSources, new GUILayoutOption[0]);
                }
                if ((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS))
                {
                    if (targetGroup == BuildTargetGroup.iPhone)
                    {
                        EditorGUILayout.PropertyField(this.m_PrepareIOSForRecording, Styles.prepareIOSForRecording, new GUILayoutOption[0]);
                    }
                    EditorGUILayout.PropertyField(this.m_UIRequiresPersistentWiFi, Styles.UIRequiresPersistentWiFi, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_IOSAllowHTTPDownload, Styles.iOSAllowHTTPDownload, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_IOSURLSchemes, Styles.iOSURLSchemes, true, new GUILayoutOption[0]);
                }
            }
            using (new EditorGUI.DisabledScope(!Application.HasProLicense()))
            {
                bool flag3 = !this.m_SubmitAnalytics.boolValue;
                bool flag4 = EditorGUILayout.Toggle(Styles.disableStatistics, flag3, new GUILayoutOption[0]);
                if (flag3 != flag4)
                {
                    this.m_SubmitAnalytics.boolValue = !flag4;
                }
                if (!Application.HasProLicense())
                {
                    this.m_SubmitAnalytics.boolValue = true;
                }
            }
            if (settingsExtension != null)
            {
                settingsExtension.ConfigurationSectionGUI();
            }
            EditorGUILayout.LabelField(Styles.scriptingDefineSymbols, new GUILayoutOption[0]);
            EditorGUI.BeginChangeCheck();
            string defines = EditorGUILayout.DelayedTextField(PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup), EditorStyles.textField, new GUILayoutOption[0]);
            this.scriptingDefinesControlID = EditorGUIUtility.s_LastControlID;
            if (EditorGUI.EndChangeCheck())
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
            }
            EditorGUILayout.Space();
        }

        public void OtherSectionGUI(BuildPlatform platform, BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
        {
            if (this.BeginSettingsBox(4, Styles.otherSettingsTitle))
            {
                this.OtherSectionRenderingGUI(platform, targetGroup, settingsExtension);
                this.OtherSectionIdentificationGUI(targetGroup, settingsExtension);
                this.OtherSectionConfigurationGUI(targetGroup, settingsExtension);
                this.OtherSectionOptimizationGUI(targetGroup);
                this.OtherSectionLoggingGUI();
                this.ShowSharedNote();
            }
            this.EndSettingsBox();
        }

        private void OtherSectionIdentificationGUI(BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
        {
            if ((settingsExtension != null) && settingsExtension.HasIdentificationGUI())
            {
                GUILayout.Label(Styles.identificationTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
                settingsExtension.IdentificationSectionGUI();
                EditorGUILayout.Space();
            }
            else if (targetGroup == BuildTargetGroup.Standalone)
            {
                GUILayout.Label(Styles.macAppStoreTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
                ShowApplicationIdentifierUI(base.serializedObject, BuildTargetGroup.Standalone, "Bundle Identifier", Styles.undoChangedBundleIdentifierString);
                EditorGUILayout.PropertyField(this.m_ApplicationBundleVersion, EditorGUIUtility.TextContent("Version*"), new GUILayoutOption[0]);
                ShowBuildNumberUI(base.serializedObject, BuildTargetGroup.Standalone, "Build", Styles.undoChangedBuildNumberString);
                EditorGUILayout.PropertyField(this.m_UseMacAppStoreValidation, Styles.useMacAppStoreValidation, new GUILayoutOption[0]);
                EditorGUILayout.Space();
            }
        }

        private void OtherSectionLoggingGUI()
        {
            GUILayout.Label(Styles.loggingTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label("Log Type", new GUILayoutOption[0]);
            IEnumerator enumerator = Enum.GetValues(typeof(StackTraceLogType)).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    StackTraceLogType current = (StackTraceLogType) enumerator.Current;
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(70f) };
                    GUILayout.Label(current.ToString(), options);
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
            GUILayout.EndHorizontal();
            IEnumerator enumerator2 = Enum.GetValues(typeof(LogType)).GetEnumerator();
            try
            {
                while (enumerator2.MoveNext())
                {
                    LogType logType = (LogType) enumerator2.Current;
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayout.Label(logType.ToString(), new GUILayoutOption[0]);
                    IEnumerator enumerator3 = Enum.GetValues(typeof(StackTraceLogType)).GetEnumerator();
                    try
                    {
                        while (enumerator3.MoveNext())
                        {
                            StackTraceLogType stackTraceType = (StackTraceLogType) enumerator3.Current;
                            StackTraceLogType stackTraceLogType = PlayerSettings.GetStackTraceLogType(logType);
                            EditorGUI.BeginChangeCheck();
                            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(70f) };
                            bool flag = EditorGUILayout.ToggleLeft(" ", stackTraceLogType == stackTraceType, optionArray2);
                            if (EditorGUI.EndChangeCheck() && flag)
                            {
                                PlayerSettings.SetStackTraceLogType(logType, stackTraceType);
                            }
                        }
                    }
                    finally
                    {
                        IDisposable disposable2 = enumerator3 as IDisposable;
                        if (disposable2 != null)
                        {
                            disposable2.Dispose();
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            finally
            {
                IDisposable disposable3 = enumerator2 as IDisposable;
                if (disposable3 != null)
                {
                    disposable3.Dispose();
                }
            }
            GUILayout.EndVertical();
        }

        private void OtherSectionOptimizationGUI(BuildTargetGroup targetGroup)
        {
            GUILayout.Label(Styles.optimizationTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_BakeCollisionMeshes, Styles.bakeCollisionMeshes, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_KeepLoadedShadersAlive, Styles.keepLoadedShadersAlive, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_PreloadedAssets, Styles.preloadedAssets, true, new GUILayoutOption[0]);
            if (((((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS)) || ((targetGroup == BuildTargetGroup.XboxOne) || (targetGroup == BuildTargetGroup.WiiU))) || (targetGroup == BuildTargetGroup.PS4)) || (targetGroup == BuildTargetGroup.PSP2))
            {
                EditorGUILayout.PropertyField(this.m_AotOptions, Styles.aotOptions, new GUILayoutOption[0]);
            }
            if ((((((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS)) || ((targetGroup == BuildTargetGroup.Android) || (targetGroup == BuildTargetGroup.Tizen))) || (((targetGroup == BuildTargetGroup.WebGL) || (targetGroup == BuildTargetGroup.WiiU)) || ((targetGroup == BuildTargetGroup.PSP2) || (targetGroup == BuildTargetGroup.PS4)))) || (targetGroup == BuildTargetGroup.XboxOne)) || (targetGroup == BuildTargetGroup.WSA))
            {
                ScriptingImplementation scriptingBackend = PlayerSettings.GetScriptingBackend(targetGroup);
                if ((targetGroup == BuildTargetGroup.WebGL) || (scriptingBackend == ScriptingImplementation.IL2CPP))
                {
                    EditorGUILayout.PropertyField(this.m_StripEngineCode, Styles.stripEngineCode, new GUILayoutOption[0]);
                }
                else if (scriptingBackend != ScriptingImplementation.WinRTDotNET)
                {
                    EditorGUILayout.PropertyField(this.m_IPhoneStrippingLevel, Styles.iPhoneStrippingLevel, new GUILayoutOption[0]);
                }
            }
            if ((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS))
            {
                EditorGUILayout.PropertyField(this.m_IPhoneScriptCallOptimization, Styles.iPhoneScriptCallOptimization, new GUILayoutOption[0]);
            }
            if (targetGroup == BuildTargetGroup.Android)
            {
                EditorGUILayout.PropertyField(this.m_AndroidProfiler, Styles.enableInternalProfiler, new GUILayoutOption[0]);
            }
            EditorGUILayout.Space();
            VertexChannelCompressionFlags intValue = (VertexChannelCompressionFlags) this.m_VertexChannelCompressionMask.intValue;
            intValue = (VertexChannelCompressionFlags) EditorGUILayout.EnumMaskPopup(Styles.vertexChannelCompressionMask, intValue, new GUILayoutOption[0]);
            this.m_VertexChannelCompressionMask.intValue = (int) intValue;
            if (targetGroup != BuildTargetGroup.PSM)
            {
                EditorGUILayout.PropertyField(this.m_StripUnusedMeshComponents, Styles.stripUnusedMeshComponents, new GUILayoutOption[0]);
            }
            if ((targetGroup == BuildTargetGroup.PSP2) || (targetGroup == BuildTargetGroup.PSM))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(this.m_VideoMemoryForVertexBuffers, Styles.videoMemoryForVertexBuffers, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    if (this.m_VideoMemoryForVertexBuffers.intValue < 0)
                    {
                        this.m_VideoMemoryForVertexBuffers.intValue = 0;
                    }
                    else if (this.m_VideoMemoryForVertexBuffers.intValue > 0xc0)
                    {
                        this.m_VideoMemoryForVertexBuffers.intValue = 0xc0;
                    }
                }
            }
            EditorGUILayout.Space();
        }

        private void OtherSectionRenderingGUI(BuildPlatform platform, BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
        {
            int num;
            int num2;
            GUILayout.Label(Styles.renderingTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
            if ((((targetGroup == BuildTargetGroup.Standalone) || (targetGroup == BuildTargetGroup.iPhone)) || ((targetGroup == BuildTargetGroup.tvOS) || (targetGroup == BuildTargetGroup.Android))) || (((targetGroup == BuildTargetGroup.PS4) || (targetGroup == BuildTargetGroup.XboxOne)) || (((targetGroup == BuildTargetGroup.WSA) || (targetGroup == BuildTargetGroup.WiiU)) || (targetGroup == BuildTargetGroup.WebGL))))
            {
                using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(this.m_ActiveColorSpace, Styles.activeColorSpace, new GUILayoutOption[0]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        base.serializedObject.ApplyModifiedProperties();
                        GUIUtility.ExitGUI();
                    }
                }
                if (PlayerSettings.colorSpace == ColorSpace.Linear)
                {
                    if (targetGroup == BuildTargetGroup.iPhone)
                    {
                        GraphicsDeviceType[] graphicsAPIs = PlayerSettings.GetGraphicsAPIs(BuildTarget.iOS);
                        bool flag = !graphicsAPIs.Contains<GraphicsDeviceType>(GraphicsDeviceType.OpenGLES3) && !graphicsAPIs.Contains<GraphicsDeviceType>(GraphicsDeviceType.OpenGLES2);
                        Version version = new Version(8, 0);
                        Version version2 = new Version(6, 0);
                        Version version3 = !string.IsNullOrEmpty(PlayerSettings.iOS.targetOSVersionString) ? new Version(PlayerSettings.iOS.targetOSVersionString) : version2;
                        if (!flag || (version3 < version))
                        {
                            EditorGUILayout.HelpBox(Styles.colorSpaceIOSWarning.text, MessageType.Warning);
                        }
                    }
                    if (targetGroup == BuildTargetGroup.tvOS)
                    {
                        GraphicsDeviceType[] source = PlayerSettings.GetGraphicsAPIs(BuildTarget.tvOS);
                        if (source.Contains<GraphicsDeviceType>(GraphicsDeviceType.OpenGLES3) || source.Contains<GraphicsDeviceType>(GraphicsDeviceType.OpenGLES2))
                        {
                            EditorGUILayout.HelpBox(Styles.colorSpaceTVOSWarning.text, MessageType.Warning);
                        }
                    }
                    if (targetGroup == BuildTargetGroup.Android)
                    {
                        GraphicsDeviceType[] typeArray3 = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
                        if (((!typeArray3.Contains<GraphicsDeviceType>(GraphicsDeviceType.Vulkan) && !typeArray3.Contains<GraphicsDeviceType>(GraphicsDeviceType.OpenGLES3)) || typeArray3.Contains<GraphicsDeviceType>(GraphicsDeviceType.OpenGLES2)) || (PlayerSettings.Android.minSdkVersion < AndroidSdkVersions.AndroidApiLevel18))
                        {
                            EditorGUILayout.HelpBox(Styles.colorSpaceAndroidWarning.text, MessageType.Warning);
                        }
                    }
                    if (targetGroup == BuildTargetGroup.WebGL)
                    {
                        EditorGUILayout.HelpBox(Styles.colorSpaceWebGLWarning.text, MessageType.Error);
                    }
                }
            }
            this.GraphicsAPIsGUI(targetGroup, platform.defaultTarget);
            if ((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS))
            {
                this.m_MetalForceHardShadows.boolValue = EditorGUILayout.Toggle(Styles.metalForceHardShadows, this.m_MetalForceHardShadows.boolValue, new GUILayoutOption[0]);
            }
            if ((Application.platform == RuntimePlatform.OSXEditor) && (((targetGroup == BuildTargetGroup.Standalone) || (targetGroup == BuildTargetGroup.iPhone)) || (targetGroup == BuildTargetGroup.tvOS)))
            {
                bool flag4 = this.m_MetalEditorSupport.boolValue || (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal);
                bool flag5 = EditorGUILayout.Toggle(Styles.metalEditorSupport, flag4, new GUILayoutOption[0]);
                if (flag5 != flag4)
                {
                    if (Application.platform == RuntimePlatform.OSXEditor)
                    {
                        GraphicsDeviceType[] apis = PlayerSettings.GetGraphicsAPIs(BuildTarget.StandaloneOSXUniversal);
                        bool firstEntryChanged = apis[0] != SystemInfo.graphicsDeviceType;
                        if (!flag5 && (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal))
                        {
                            firstEntryChanged = true;
                        }
                        if (flag5 && (apis[0] == GraphicsDeviceType.Metal))
                        {
                            firstEntryChanged = true;
                        }
                        ChangeGraphicsApiAction action = this.CheckApplyGraphicsAPIList(BuildTarget.StandaloneOSXUniversal, firstEntryChanged);
                        if (action.changeList)
                        {
                            this.m_MetalEditorSupport.boolValue = flag5;
                            base.serializedObject.ApplyModifiedProperties();
                        }
                        this.ApplyChangeGraphicsApiAction(BuildTarget.StandaloneOSXUniversal, apis, action);
                    }
                    else
                    {
                        this.m_MetalEditorSupport.boolValue = flag5;
                        base.serializedObject.ApplyModifiedProperties();
                    }
                }
                if (this.m_MetalEditorSupport.boolValue)
                {
                    this.m_MetalAPIValidation.boolValue = EditorGUILayout.Toggle(Styles.metalAPIValidation, this.m_MetalAPIValidation.boolValue, new GUILayoutOption[0]);
                }
            }
            if (((targetGroup == BuildTargetGroup.PSP2) || (targetGroup == BuildTargetGroup.PSM)) || ((targetGroup == BuildTargetGroup.Android) || (targetGroup == BuildTargetGroup.SamsungTV)))
            {
                if (this.IsMobileTarget(targetGroup))
                {
                    this.m_MobileMTRendering.boolValue = EditorGUILayout.Toggle(Styles.mTRendering, this.m_MobileMTRendering.boolValue, new GUILayoutOption[0]);
                }
                else
                {
                    this.m_MTRendering.boolValue = EditorGUILayout.Toggle(Styles.mTRendering, this.m_MTRendering.boolValue, new GUILayoutOption[0]);
                }
            }
            else if ((targetGroup == BuildTargetGroup.PSP2) || (targetGroup == BuildTargetGroup.PSM))
            {
                if (Unsupported.IsDeveloperBuild())
                {
                    this.m_MTRendering.boolValue = EditorGUILayout.Toggle(Styles.mTRendering, this.m_MTRendering.boolValue, new GUILayoutOption[0]);
                }
                else
                {
                    this.m_MTRendering.boolValue = true;
                }
            }
            bool flag7 = true;
            bool flag8 = true;
            if (settingsExtension != null)
            {
                flag7 = settingsExtension.SupportsStaticBatching();
                flag8 = settingsExtension.SupportsDynamicBatching();
            }
            PlayerSettings.GetBatchingForPlatform(platform.defaultTarget, out num, out num2);
            bool flag9 = false;
            if (!flag7 && (num == 1))
            {
                num = 0;
                flag9 = true;
            }
            if (!flag8 && (num2 == 1))
            {
                num2 = 0;
                flag9 = true;
            }
            if (flag9)
            {
                PlayerSettings.SetBatchingForPlatform(platform.defaultTarget, num, num2);
            }
            EditorGUI.BeginChangeCheck();
            using (new EditorGUI.DisabledScope(!flag7))
            {
                if (GUI.enabled)
                {
                    num = !EditorGUILayout.Toggle(Styles.staticBatching, num != 0, new GUILayoutOption[0]) ? 0 : 1;
                }
                else
                {
                    EditorGUILayout.Toggle(Styles.staticBatching, false, new GUILayoutOption[0]);
                }
            }
            using (new EditorGUI.DisabledScope(!flag8))
            {
                num2 = !EditorGUILayout.Toggle(Styles.dynamicBatching, num2 != 0, new GUILayoutOption[0]) ? 0 : 1;
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(base.target, Styles.undoChangedBatchingString);
                PlayerSettings.SetBatchingForPlatform(platform.defaultTarget, num, num2);
            }
            if ((((targetGroup == BuildTargetGroup.WiiU) || (targetGroup == BuildTargetGroup.Standalone)) || ((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS))) || (((targetGroup == BuildTargetGroup.Android) || (targetGroup == BuildTargetGroup.PSP2)) || (((targetGroup == BuildTargetGroup.PS4) || (targetGroup == BuildTargetGroup.PSM)) || (targetGroup == BuildTargetGroup.WSA))))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(this.m_SkinOnGPU, (targetGroup == BuildTargetGroup.PS4) ? Styles.skinOnGPUPS4 : Styles.skinOnGPU, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    ShaderUtil.RecreateSkinnedMeshResources();
                }
            }
            EditorGUILayout.PropertyField(this.m_GraphicsJobs, Styles.graphicsJobs, new GUILayoutOption[0]);
            if (this.PlatformSupportsGfxJobModes(targetGroup))
            {
                using (new EditorGUI.DisabledScope(!this.m_GraphicsJobs.boolValue))
                {
                    GraphicsJobMode graphicsJobMode = PlayerSettings.graphicsJobMode;
                    GraphicsJobMode mode2 = BuildEnumPopup<GraphicsJobMode>(Styles.graphicsJobsMode, graphicsJobMode, m_GfxJobModeValues, m_GfxJobModeNames);
                    if (mode2 != graphicsJobMode)
                    {
                        PlayerSettings.graphicsJobMode = mode2;
                    }
                }
            }
            if (this.m_VRSettings.TargetGroupSupportsVirtualReality(targetGroup))
            {
                this.m_VRSettings.DevicesGUI(targetGroup);
                using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
                {
                    this.m_VRSettings.SinglePassStereoGUI(targetGroup, this.m_StereoRenderingPath);
                }
            }
            if (TargetSupportsProtectedGraphicsMem(targetGroup))
            {
                PlayerSettings.protectGraphicsMemory = EditorGUILayout.Toggle(Styles.protectGraphicsMemory, PlayerSettings.protectGraphicsMemory, new GUILayoutOption[0]);
            }
            if (TargetSupportsHighDynamicRangeDisplays(targetGroup))
            {
                PlayerSettings.useHDRDisplay = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Use display in HDR mode|Automatically switch the display to HDR output (on supported displays) at start of application."), PlayerSettings.useHDRDisplay, new GUILayoutOption[0]);
            }
            EditorGUILayout.Space();
        }

        private Version ParseIosVersion(string text)
        {
            if (text.IndexOf('.') < 0)
            {
                text = text + ".0";
            }
            try
            {
                return new Version(text);
            }
            catch
            {
                return new Version();
            }
        }

        private bool PlatformSupportsGfxJobModes(BuildTargetGroup targetGroup) => 
            (targetGroup == BuildTargetGroup.PS4);

        public void PublishSectionGUI(BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
        {
            if ((((targetGroup == BuildTargetGroup.WSA) || (targetGroup == BuildTargetGroup.PSP2)) || (targetGroup == BuildTargetGroup.PSM)) || ((settingsExtension != null) && settingsExtension.HasPublishSection()))
            {
                if (this.BeginSettingsBox(5, Styles.publishingSettingsTitle))
                {
                    float h = 16f;
                    float midWidth = (80f + EditorGUIUtility.fieldWidth) + 5f;
                    float maxWidth = (80f + EditorGUIUtility.fieldWidth) + 5f;
                    if (settingsExtension != null)
                    {
                        settingsExtension.PublishSectionGUI(h, midWidth, maxWidth);
                    }
                    if (targetGroup == BuildTargetGroup.PSM)
                    {
                    }
                }
                this.EndSettingsBox();
            }
        }

        private void RemoveGraphicsDeviceElement(BuildTarget target, ReorderableList list)
        {
            GraphicsDeviceType[] graphicsAPIs = PlayerSettings.GetGraphicsAPIs(target);
            if (graphicsAPIs != null)
            {
                if (graphicsAPIs.Length < 2)
                {
                    EditorApplication.Beep();
                }
                else
                {
                    List<GraphicsDeviceType> list2 = graphicsAPIs.ToList<GraphicsDeviceType>();
                    list2.RemoveAt(list.index);
                    graphicsAPIs = list2.ToArray();
                    this.ApplyChangedGraphicsAPIList(target, graphicsAPIs, list.index == 0);
                }
            }
        }

        private void ReorderGraphicsDeviceElement(BuildTarget target, ReorderableList list)
        {
            GraphicsDeviceType[] graphicsAPIs = PlayerSettings.GetGraphicsAPIs(target);
            GraphicsDeviceType[] apis = ((List<GraphicsDeviceType>) list.list).ToArray();
            bool firstEntryChanged = graphicsAPIs[0] != apis[0];
            this.ApplyChangedGraphicsAPIList(target, apis, firstEntryChanged);
        }

        public void ResolutionSectionGUI(BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
        {
            if (this.BeginSettingsBox(0, Styles.resolutionPresentationTitle))
            {
                if (settingsExtension != null)
                {
                    float h = 16f;
                    float midWidth = (80f + EditorGUIUtility.fieldWidth) + 5f;
                    float maxWidth = (80f + EditorGUIUtility.fieldWidth) + 5f;
                    settingsExtension.ResolutionSectionGUI(h, midWidth, maxWidth);
                }
                if (targetGroup == BuildTargetGroup.Standalone)
                {
                    GUILayout.Label(Styles.resolutionTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_DefaultIsFullScreen, Styles.defaultIsFullScreen, new GUILayoutOption[0]);
                    this.m_ShowDefaultIsNativeResolution.target = this.m_DefaultIsFullScreen.boolValue;
                    if (EditorGUILayout.BeginFadeGroup(this.m_ShowDefaultIsNativeResolution.faded))
                    {
                        EditorGUILayout.PropertyField(this.m_DefaultIsNativeResolution, new GUILayoutOption[0]);
                    }
                    if ((this.m_ShowDefaultIsNativeResolution.faded != 0f) && (this.m_ShowDefaultIsNativeResolution.faded != 1f))
                    {
                        EditorGUILayout.EndFadeGroup();
                    }
                    this.m_ShowResolution.target = !(this.m_DefaultIsFullScreen.boolValue && this.m_DefaultIsNativeResolution.boolValue);
                    if (EditorGUILayout.BeginFadeGroup(this.m_ShowResolution.faded))
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(this.m_DefaultScreenWidth, Styles.defaultScreenWidth, new GUILayoutOption[0]);
                        if (EditorGUI.EndChangeCheck() && (this.m_DefaultScreenWidth.intValue < 1))
                        {
                            this.m_DefaultScreenWidth.intValue = 1;
                        }
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(this.m_DefaultScreenHeight, Styles.defaultScreenHeight, new GUILayoutOption[0]);
                        if (EditorGUI.EndChangeCheck() && (this.m_DefaultScreenHeight.intValue < 1))
                        {
                            this.m_DefaultScreenHeight.intValue = 1;
                        }
                    }
                    if ((this.m_ShowResolution.faded != 0f) && (this.m_ShowResolution.faded != 1f))
                    {
                        EditorGUILayout.EndFadeGroup();
                    }
                }
                if (targetGroup == BuildTargetGroup.Standalone)
                {
                    EditorGUILayout.PropertyField(this.m_RunInBackground, Styles.runInBackground, new GUILayoutOption[0]);
                }
                if ((settingsExtension != null) && settingsExtension.SupportsOrientation())
                {
                    GUILayout.Label(Styles.orientationTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
                    using (new EditorGUI.DisabledScope(PlayerSettings.virtualRealitySupported))
                    {
                        EditorGUILayout.PropertyField(this.m_DefaultScreenOrientation, Styles.defaultScreenOrientation, new GUILayoutOption[0]);
                        if (PlayerSettings.virtualRealitySupported)
                        {
                            EditorGUILayout.HelpBox(Styles.VRSupportOverridenInfo.text, MessageType.Info);
                        }
                        if (this.m_DefaultScreenOrientation.enumValueIndex == 4)
                        {
                            if ((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.Tizen))
                            {
                                EditorGUILayout.PropertyField(this.m_UseOSAutoRotation, Styles.useOSAutoRotation, new GUILayoutOption[0]);
                            }
                            EditorGUI.indentLevel++;
                            GUILayout.Label(Styles.allowedOrientationTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
                            if (!(((this.m_AllowedAutoRotateToPortrait.boolValue || this.m_AllowedAutoRotateToPortraitUpsideDown.boolValue) || this.m_AllowedAutoRotateToLandscapeRight.boolValue) || this.m_AllowedAutoRotateToLandscapeLeft.boolValue))
                            {
                                this.m_AllowedAutoRotateToPortrait.boolValue = true;
                                Debug.LogError("All orientations are disabled. Allowing portrait");
                            }
                            EditorGUILayout.PropertyField(this.m_AllowedAutoRotateToPortrait, Styles.allowedAutoRotateToPortrait, new GUILayoutOption[0]);
                            if ((targetGroup != BuildTargetGroup.WSA) || (EditorUserBuildSettings.wsaSDK != WSASDK.PhoneSDK81))
                            {
                                EditorGUILayout.PropertyField(this.m_AllowedAutoRotateToPortraitUpsideDown, Styles.allowedAutoRotateToPortraitUpsideDown, new GUILayoutOption[0]);
                            }
                            EditorGUILayout.PropertyField(this.m_AllowedAutoRotateToLandscapeRight, Styles.allowedAutoRotateToLandscapeRight, new GUILayoutOption[0]);
                            EditorGUILayout.PropertyField(this.m_AllowedAutoRotateToLandscapeLeft, Styles.allowedAutoRotateToLandscapeLeft, new GUILayoutOption[0]);
                            EditorGUI.indentLevel--;
                        }
                    }
                }
                if (targetGroup == BuildTargetGroup.iPhone)
                {
                    GUILayout.Label(Styles.multitaskingSupportTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_UIRequiresFullScreen, Styles.UIRequiresFullScreen, new GUILayoutOption[0]);
                    EditorGUILayout.Space();
                    GUILayout.Label(Styles.statusBarTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_UIStatusBarHidden, Styles.UIStatusBarHidden, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_UIStatusBarStyle, Styles.UIStatusBarStyle, new GUILayoutOption[0]);
                    EditorGUILayout.Space();
                }
                EditorGUILayout.Space();
                if (targetGroup == BuildTargetGroup.Standalone)
                {
                    GUILayout.Label(Styles.standalonePlayerOptionsTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_CaptureSingleScreen, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_DisplayResolutionDialog, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_UsePlayerLog, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_ResizableWindow, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_MacFullscreenMode, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_D3D9FullscreenMode, Styles.D3D9FullscreenMode, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_D3D11FullscreenMode, Styles.D3D11FullscreenMode, new GUILayoutOption[0]);
                    GraphicsDeviceType[] graphicsAPIs = PlayerSettings.GetGraphicsAPIs(BuildTarget.StandaloneWindows);
                    bool flag2 = (graphicsAPIs.Length >= 1) && (graphicsAPIs[0] == GraphicsDeviceType.Direct3D9);
                    bool flag3 = this.m_D3D9FullscreenMode.intValue == 0;
                    bool disabled = flag2 && flag3;
                    if (disabled)
                    {
                        this.m_VisibleInBackground.boolValue = false;
                    }
                    using (new EditorGUI.DisabledScope(disabled))
                    {
                        EditorGUILayout.PropertyField(this.m_VisibleInBackground, Styles.visibleInBackground, new GUILayoutOption[0]);
                    }
                    EditorGUILayout.PropertyField(this.m_AllowFullscreenSwitch, Styles.allowFullscreenSwitch, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_ForceSingleInstance, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_SupportedAspectRatios, true, new GUILayoutOption[0]);
                    EditorGUILayout.Space();
                }
                if (this.IsMobileTarget(targetGroup))
                {
                    if (((targetGroup != BuildTargetGroup.Tizen) && (targetGroup != BuildTargetGroup.iPhone)) && (targetGroup != BuildTargetGroup.tvOS))
                    {
                        EditorGUILayout.PropertyField(this.m_Use32BitDisplayBuffer, Styles.use32BitDisplayBuffer, new GUILayoutOption[0]);
                    }
                    EditorGUILayout.PropertyField(this.m_DisableDepthAndStencilBuffers, Styles.disableDepthAndStencilBuffers, new GUILayoutOption[0]);
                }
                if (targetGroup == BuildTargetGroup.iPhone)
                {
                    EditorGUILayout.PropertyField(this.m_iosShowActivityIndicatorOnLoading, Styles.iosShowActivityIndicatorOnLoading, new GUILayoutOption[0]);
                }
                if (targetGroup == BuildTargetGroup.Android)
                {
                    EditorGUILayout.PropertyField(this.m_androidShowActivityIndicatorOnLoading, Styles.androidShowActivityIndicatorOnLoading, new GUILayoutOption[0]);
                }
                if (targetGroup == BuildTargetGroup.Tizen)
                {
                    EditorGUILayout.PropertyField(this.m_tizenShowActivityIndicatorOnLoading, EditorGUIUtility.TextContent("Show Loading Indicator"), new GUILayoutOption[0]);
                }
                if (((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.Android)) || (targetGroup == BuildTargetGroup.Tizen))
                {
                    EditorGUILayout.Space();
                }
                this.ShowSharedNote();
            }
            this.EndSettingsBox();
        }

        internal static void ShowApplicationIdentifierUI(SerializedObject serializedObject, BuildTargetGroup targetGroup, string label, string undoText)
        {
            EditorGUI.BeginChangeCheck();
            string identifier = EditorGUILayout.DelayedTextField(EditorGUIUtility.TextContent(label), PlayerSettings.GetApplicationIdentifier(targetGroup), new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(serializedObject.targetObject, undoText);
                PlayerSettings.SetApplicationIdentifier(targetGroup, identifier);
            }
        }

        internal static void ShowBuildNumberUI(SerializedObject serializedObject, BuildTargetGroup targetGroup, string label, string undoText)
        {
            EditorGUI.BeginChangeCheck();
            string buildNumber = EditorGUILayout.DelayedTextField(EditorGUIUtility.TextContent(label), PlayerSettings.GetBuildNumber(targetGroup), new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(serializedObject.targetObject, undoText);
                PlayerSettings.SetBuildNumber(targetGroup, buildNumber);
            }
        }

        private void ShowNoSettings()
        {
            GUILayout.Label(Styles.notApplicableInfo, EditorStyles.miniLabel, new GUILayoutOption[0]);
        }

        public void ShowSharedNote()
        {
            GUILayout.Label(Styles.sharedBetweenPlatformsInfo, EditorStyles.miniLabel, new GUILayoutOption[0]);
        }

        private static void ShowWarning(GUIContent warningMessage)
        {
            if (s_WarningIcon == null)
            {
                s_WarningIcon = EditorGUIUtility.LoadIcon("console.warnicon");
            }
            warningMessage.image = s_WarningIcon;
            GUILayout.Space(5f);
            GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
            GUILayout.Label(warningMessage, EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
            GUILayout.EndVertical();
        }

        public static void SyncPlatformAPIsList(BuildTarget target)
        {
            if (s_GraphicsDeviceLists.ContainsKey(target))
            {
                s_GraphicsDeviceLists[target].list = PlayerSettings.GetGraphicsAPIs(target).ToList<GraphicsDeviceType>();
            }
        }

        private static bool TargetSupportsHighDynamicRangeDisplays(BuildTargetGroup targetGroup) => 
            (targetGroup == BuildTargetGroup.XboxOne);

        private static bool TargetSupportsOptionalBuiltinSplashScreen(BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
        {
            if (settingsExtension != null)
            {
                return settingsExtension.CanShowUnitySplashScreen();
            }
            return (targetGroup == BuildTargetGroup.Standalone);
        }

        private static bool TargetSupportsProtectedGraphicsMem(BuildTargetGroup targetGroup) => 
            (targetGroup == BuildTargetGroup.Android);

        public override bool UseDefaultMargins() => 
            false;

        private static bool WillEditorUseFirstGraphicsAPI(BuildTarget targetPlatform) => 
            (((Application.platform == RuntimePlatform.WindowsEditor) && (targetPlatform == BuildTarget.StandaloneWindows)) || ((Application.platform == RuntimePlatform.OSXEditor) && (targetPlatform == BuildTarget.StandaloneOSXUniversal)));

        private PlayerSettingsSplashScreenEditor splashScreenEditor
        {
            get
            {
                if (this.m_SplashScreenEditor == null)
                {
                    this.m_SplashScreenEditor = new PlayerSettingsSplashScreenEditor(this);
                }
                return this.m_SplashScreenEditor;
            }
        }

        [CompilerGenerated]
        private sealed class <GraphicsAPIsGUIOnePlatform>c__AnonStorey0
        {
            internal PlayerSettingsEditor $this;
            internal BuildTarget targetPlatform;
        }

        [CompilerGenerated]
        private sealed class <GraphicsAPIsGUIOnePlatform>c__AnonStorey1
        {
            internal PlayerSettingsEditor.<GraphicsAPIsGUIOnePlatform>c__AnonStorey0 <>f__ref$0;
            internal string displayTitle;

            internal void <>m__0(Rect rect, ReorderableList list)
            {
                this.<>f__ref$0.$this.AddGraphicsDeviceElement(this.<>f__ref$0.targetPlatform, rect, list);
            }

            internal void <>m__1(ReorderableList list)
            {
                this.<>f__ref$0.$this.RemoveGraphicsDeviceElement(this.<>f__ref$0.targetPlatform, list);
            }

            internal void <>m__2(ReorderableList list)
            {
                this.<>f__ref$0.$this.ReorderGraphicsDeviceElement(this.<>f__ref$0.targetPlatform, list);
            }

            internal void <>m__3(Rect rect, int index, bool isActive, bool isFocused)
            {
                this.<>f__ref$0.$this.DrawGraphicsDeviceElement(this.<>f__ref$0.targetPlatform, rect, index, isActive, isFocused);
            }

            internal void <>m__4(Rect rect)
            {
                GUI.Label(rect, this.displayTitle, EditorStyles.label);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ChangeGraphicsApiAction
        {
            public bool changeList;
            public bool reloadGfx;
            public ChangeGraphicsApiAction(bool doChange, bool doReload)
            {
                this.changeList = doChange;
                this.reloadGfx = doReload;
            }
        }

        private static class Styles
        {
            public static readonly GUIContent accelerometerFrequency = EditorGUIUtility.TextContent("Accelerometer Frequency*");
            public static readonly GUIContent actionOnDotNetUnhandledException = EditorGUIUtility.TextContent("On .Net UnhandledException*");
            public static readonly GUIContent activeColorSpace = EditorGUIUtility.TextContent("Color Space*");
            public static readonly GUIContent allowedAutoRotateToLandscapeLeft = EditorGUIUtility.TextContent("Landscape Left");
            public static readonly GUIContent allowedAutoRotateToLandscapeRight = EditorGUIUtility.TextContent("Landscape Right");
            public static readonly GUIContent allowedAutoRotateToPortrait = EditorGUIUtility.TextContent("Portrait");
            public static readonly GUIContent allowedAutoRotateToPortraitUpsideDown = EditorGUIUtility.TextContent("Portrait Upside Down");
            public static readonly GUIContent allowedOrientationTitle = EditorGUIUtility.TextContent("Allowed Orientations for Auto Rotation");
            public static readonly GUIContent allowFullscreenSwitch = EditorGUIUtility.TextContent("Allow Fullscreen Switch");
            public static readonly GUIContent androidShowActivityIndicatorOnLoading = EditorGUIUtility.TextContent("Show Loading Indicator");
            public static readonly GUIContent aotOptions = EditorGUIUtility.TextContent("AOT Compilation Options*");
            public static readonly GUIContent apiCompatibilityLevel = EditorGUIUtility.TextContent("Api Compatibility Level*");
            public static readonly GUIContent apiCompatibilityLevel_NET_2_0 = EditorGUIUtility.TextContent(".NET 2.0");
            public static readonly GUIContent apiCompatibilityLevel_NET_2_0_Subset = EditorGUIUtility.TextContent(".NET 2.0 Subset");
            public static readonly GUIContent apiCompatibilityLevel_NET_4_6 = EditorGUIUtility.TextContent(".NET 4.6");
            public static readonly GUIContent apiCompatibilityLevel_WiiUSubset = EditorGUIUtility.TextContent("WiiU Subset");
            public static readonly GUIContent appleDeveloperTeamID = EditorGUIUtility.TextContent("iOS Developer Team ID|Developers can retrieve their Team ID by visiting the Apple Developer site under Account > Membership.");
            public static readonly GUIContent applicationBuildNumber = EditorGUIUtility.TextContent("Build");
            public static readonly GUIContent bakeCollisionMeshes = EditorGUIUtility.TextContent("Prebake Collision Meshes*|Bake collision data into the meshes on build time");
            public static readonly GUIContent cameraUsageDescription = EditorGUIUtility.TextContent("Camera Usage Description*");
            public static readonly GUIStyle categoryBox = new GUIStyle(EditorStyles.helpBox);
            public static readonly GUIContent colorSpaceAndroidWarning = EditorGUIUtility.TextContent("On Android, linear colorspace requires OpenGL ES 3.0 or Vulkan, uncheck 'Automatic Graphics API' to remove OpenGL ES 2 API and 'Minimum API Level' must be at least Android 4.3");
            public static readonly GUIContent colorSpaceIOSWarning = EditorGUIUtility.TextContent("Linear colorspace requires Metal API only. Uncheck 'Automatic Graphics API' and remove OpenGL ES 2 API. Additionally, 'minimum iOS version' set to 8.0 at least");
            public static readonly GUIContent colorSpaceTVOSWarning = EditorGUIUtility.TextContent("Linear colorspace requires Metal API only. Uncheck 'Automatic Graphics API' and remove OpenGL ES 2 API.");
            public static readonly GUIContent colorSpaceWebGLWarning = EditorGUIUtility.TextContent("On WebGL, linear colorspace is not supported");
            public static readonly GUIContent configurationTitle = EditorGUIUtility.TextContent("Configuration");
            public static readonly GUIContent crashReportingTitle = EditorGUIUtility.TextContent("Crash Reporting");
            public static readonly GUIContent cursorHotspot = EditorGUIUtility.TextContent("Cursor Hotspot");
            public static readonly GUIContent D3D11FullscreenMode = EditorGUIUtility.TextContent("D3D11 Fullscreen Mode");
            public static readonly GUIContent D3D9FullscreenMode = EditorGUIUtility.TextContent("D3D9 Fullscreen Mode");
            public static readonly GUIContent debuggingCrashReportingTitle = EditorGUIUtility.TextContent("Debugging and crash reporting");
            public static readonly GUIContent debuggingTitle = EditorGUIUtility.TextContent("Debugging");
            public static readonly GUIContent defaultCursor = EditorGUIUtility.TextContent("Default Cursor");
            public static readonly GUIContent defaultIcon = EditorGUIUtility.TextContent("Default Icon");
            public static readonly GUIContent defaultIsFullScreen = EditorGUIUtility.TextContent("Default Is Full Screen*");
            public static readonly GUIContent defaultScreenHeight = EditorGUIUtility.TextContent("Default Screen Height");
            public static readonly GUIContent defaultScreenOrientation = EditorGUIUtility.TextContent("Default Orientation*");
            public static readonly GUIContent defaultScreenWidth = EditorGUIUtility.TextContent("Default Screen Width");
            public static readonly GUIContent disableDepthAndStencilBuffers = EditorGUIUtility.TextContent("Disable Depth and Stencil*");
            public static readonly GUIContent disableStatistics = EditorGUIUtility.TextContent("Disable HW Statistics*|Disables HW Statistics (Pro Only)");
            public static readonly GUIContent dynamicBatching = EditorGUIUtility.TextContent("Dynamic Batching");
            public static readonly GUIContent enableCrashReportAPI = EditorGUIUtility.TextContent("Enable CrashReport API*");
            public static readonly GUIContent enableInternalProfiler = EditorGUIUtility.TextContent("Enable Internal Profiler* (Deprecated)|Internal profiler counters should be accessed by scripts using UnityEngine.Profiling::Profiler API.");
            public static readonly GUIContent graphicsJobs = EditorGUIUtility.TextContent("Graphics Jobs (Experimental)*");
            public static readonly GUIContent graphicsJobsMode = EditorGUIUtility.TextContent("Graphics Jobs Mode*");
            public static readonly GUIContent iconTitle = EditorGUIUtility.TextContent("Icon");
            public static readonly GUIContent identificationTitle = EditorGUIUtility.TextContent("Identification");
            public static readonly GUIContent IL2CPPAndroidExperimentalInfo = EditorGUIUtility.TextContent("IL2CPP on Android is experimental and unsupported");
            public static readonly GUIContent iOSAllowHTTPDownload = EditorGUIUtility.TextContent("Allow downloads over HTTP (nonsecure)*");
            public static readonly GUIContent iosShowActivityIndicatorOnLoading = EditorGUIUtility.TextContent("Show Loading Indicator");
            public static readonly GUIContent iOSURLSchemes = EditorGUIUtility.TextContent("Supported URL schemes*");
            public static readonly GUIContent iPhoneScriptCallOptimization = EditorGUIUtility.TextContent("Script Call Optimization*");
            public static readonly GUIContent iPhoneStrippingLevel = EditorGUIUtility.TextContent("Stripping Level*");
            public static readonly GUIContent iPhoneTargetOSVersion = EditorGUIUtility.TextContent("Target minimum iOS Version");
            public static readonly GUIContent keepLoadedShadersAlive = EditorGUIUtility.TextContent("Keep Loaded Shaders Alive*|Prevents shaders from being unloaded");
            public static readonly GUIContent locationUsageDescription = EditorGUIUtility.TextContent("Location Usage Description*");
            public static readonly GUIContent loggingTitle = EditorGUIUtility.TextContent("Logging*");
            public static readonly GUIContent logObjCUncaughtExceptions = EditorGUIUtility.TextContent("Log Obj-C Uncaught Exceptions*");
            public static readonly GUIContent macAppStoreTitle = EditorGUIUtility.TextContent("Mac App Store Options");
            public static readonly GUIContent metalAPIValidation = EditorGUIUtility.TextContent("Metal API Validation*");
            public static readonly GUIContent metalEditorSupport = EditorGUIUtility.TextContent("Metal Editor Support* (Experimental)");
            public static readonly GUIContent metalForceHardShadows = EditorGUIUtility.TextContent("Force hard shadows on Metal*");
            public static readonly GUIContent microphoneUsageDescription = EditorGUIUtility.TextContent("Microphone Usage Description*");
            public static readonly GUIContent mTRendering = EditorGUIUtility.TextContent("Multithreaded Rendering*");
            public static readonly GUIContent multitaskingSupportTitle = EditorGUIUtility.TextContent("Multitasking Support");
            public static readonly GUIContent muteOtherAudioSources = EditorGUIUtility.TextContent("Mute Other Audio Sources*");
            public static readonly GUIContent notApplicableInfo = EditorGUIUtility.TextContent("Not applicable for this platform.");
            public static readonly GUIContent optimizationTitle = EditorGUIUtility.TextContent("Optimization");
            public static readonly GUIContent orientationTitle = EditorGUIUtility.TextContent("Orientation");
            public static readonly GUIContent otherSettingsTitle = EditorGUIUtility.TextContent("Other Settings");
            public static readonly GUIContent preloadedAssets = EditorGUIUtility.TextContent("Preloaded Assets*|Assets to load at start up in the player and kept alive until the player terminates");
            public static readonly GUIContent prepareIOSForRecording = EditorGUIUtility.TextContent("Prepare iOS for Recording");
            public static readonly GUIContent protectGraphicsMemory = EditorGUIUtility.TextContent("Protect Graphics Memory|Protect GPU memory from being read (on supported devices). Will prevent user from taking screenshots");
            public static readonly GUIContent publishingSettingsTitle = EditorGUIUtility.TextContent("Publishing Settings");
            public static readonly GUIContent recordingInfo = EditorGUIUtility.TextContent("Reordering the list will switch editor to the first available platform");
            public static readonly GUIContent renderingTitle = EditorGUIUtility.TextContent("Rendering");
            public static readonly GUIContent require31 = EditorGUIUtility.TextContent("Require ES3.1");
            public static readonly GUIContent requireAEP = EditorGUIUtility.TextContent("Require ES3.1+AEP");
            public static readonly GUIContent resolutionPresentationTitle = EditorGUIUtility.TextContent("Resolution and Presentation");
            public static readonly GUIContent resolutionTitle = EditorGUIUtility.TextContent("Resolution");
            public static readonly GUIContent runInBackground = EditorGUIUtility.TextContent("Run In Background*");
            public static readonly GUIContent scriptingBackend = EditorGUIUtility.TextContent("Scripting Backend");
            public static readonly GUIContent scriptingDefault = EditorGUIUtility.TextContent("Default");
            public static readonly GUIContent scriptingDefineSymbols = EditorGUIUtility.TextContent("Scripting Define Symbols*");
            public static readonly GUIContent scriptingIL2CPP = EditorGUIUtility.TextContent("IL2CPP");
            public static readonly GUIContent scriptingMono2x = EditorGUIUtility.TextContent("Mono2x");
            public static readonly GUIContent scriptingWinRTDotNET = EditorGUIUtility.TextContent(".NET");
            public static readonly GUIContent sharedBetweenPlatformsInfo = EditorGUIUtility.TextContent("* Shared setting between multiple platforms.");
            public static readonly GUIContent skinOnGPU = EditorGUIUtility.TextContent("GPU Skinning*|Use DX11/ES3 GPU Skinning");
            public static readonly GUIContent skinOnGPUPS4 = EditorGUIUtility.TextContent("Compute Skinning*|Use Compute pipeline for Skinning");
            public static readonly GUIContent standalonePlayerOptionsTitle = EditorGUIUtility.TextContent("Standalone Player Options");
            public static readonly GUIContent staticBatching = EditorGUIUtility.TextContent("Static Batching");
            public static readonly GUIContent statusBarTitle = EditorGUIUtility.TextContent("Status Bar");
            public static readonly GUIContent stripEngineCode = EditorGUIUtility.TextContent("Strip Engine Code*|Strip Unused Engine Code - Note that byte code stripping of managed assemblies is always enabled for the IL2CPP scripting backend.");
            public static readonly GUIContent stripUnusedMeshComponents = EditorGUIUtility.TextContent("Optimize Mesh Data*|Remove unused mesh components");
            public static readonly GUIContent targetSdkVersion = EditorGUIUtility.TextContent("Target SDK");
            public static readonly GUIContent tvOSTargetOSVersion = EditorGUIUtility.TextContent("Target minimum tvOS Version");
            public static readonly GUIContent UIPrerenderedIcon = EditorGUIUtility.TextContent("Prerendered Icon");
            public static readonly GUIContent UIRequiresFullScreen = EditorGUIUtility.TextContent("Requires Fullscreen");
            public static readonly GUIContent UIRequiresPersistentWiFi = EditorGUIUtility.TextContent("Requires Persistent WiFi*");
            public static readonly GUIContent UIStatusBarHidden = EditorGUIUtility.TextContent("Status Bar Hidden");
            public static readonly GUIContent UIStatusBarStyle = EditorGUIUtility.TextContent("Status Bar Style");
            public static readonly GUIContent use32BitDisplayBuffer = EditorGUIUtility.TextContent("Use 32-bit Display Buffer*|If set Display Buffer will be created to hold 32-bit color values. Use it only if you see banding, as it has performance implications.");
            public static readonly GUIContent useMacAppStoreValidation = EditorGUIUtility.TextContent("Mac App Store Validation");
            public static readonly GUIContent useOnDemandResources = EditorGUIUtility.TextContent("Use on demand resources*");
            public static readonly GUIContent useOSAutoRotation = EditorGUIUtility.TextContent("Use Animated Autorotation|If set OS native animated autorotation method will be used. Otherwise orientation will be changed immediately.");
            public static readonly GUIContent vertexChannelCompressionMask = EditorGUIUtility.TextContent("Vertex Compression*|Select which vertex channels should be compressed. Compression can save memory and bandwidth but precision will be lower.");
            public static readonly GUIContent videoMemoryForVertexBuffers = EditorGUIUtility.TextContent("Mesh Video Mem*|How many megabytes of video memory to use for mesh data before we use main memory");
            public static readonly GUIContent visibleInBackground = EditorGUIUtility.TextContent("Visible In Background");
            public static readonly GUIContent VRSupportOverridenInfo = EditorGUIUtility.TextContent("This setting is overridden by Virtual Reality Support.");

            static Styles()
            {
                categoryBox.padding.left = 14;
            }

            public static string undoChangedBatchingString =>
                LocalizationDatabase.GetLocalizedString("Changed Batching Settings");

            public static string undoChangedBuildNumberString =>
                LocalizationDatabase.GetLocalizedString("Changed macOS build number");

            public static string undoChangedBundleIdentifierString =>
                LocalizationDatabase.GetLocalizedString("Changed macOS bundleIdentifier");

            public static string undoChangedGraphicsAPIString =>
                LocalizationDatabase.GetLocalizedString("Changed Graphics API Settings");

            public static string undoChangedIconString =>
                LocalizationDatabase.GetLocalizedString("Changed Icon");
        }
    }
}

