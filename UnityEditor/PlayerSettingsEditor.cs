namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.AnimatedValues;
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
        public static readonly GUIContent defaultIsFullScreen;
        private const int kIconSpacing = 6;
        private const int kMaxPreviewSize = 0x60;
        private static BuildTargetGroup[] kSinglePassStereoRenderingTargetGroups;
        private const int kSlotSize = 0x40;
        private static BuildTargetGroup[] kStereoInstancingRenderingTargetGroups;
        private static int[] kStereoRenderingMethodValues;
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
        private SerializedProperty m_ApiCompatibilityLevel;
        private SerializedProperty m_ApplicationBundleIdentifier;
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
        private Dictionary<BuildTarget, ReorderableList> m_GraphicsDeviceLists = new Dictionary<BuildTarget, ReorderableList>();
        private SerializedProperty m_GraphicsJobs;
        private SerializedProperty m_IOSAllowHTTPDownload;
        private SerializedProperty m_iosShowActivityIndicatorOnLoading;
        private SerializedProperty m_IOSURLSchemes;
        private SerializedProperty m_IPhoneApplicationDisplayName;
        private SerializedProperty m_IPhoneBuildNumber;
        private SerializedProperty m_IPhoneScriptCallOptimization;
        private SerializedProperty m_IPhoneSdkVersion;
        private SerializedProperty m_IPhoneStrippingLevel;
        private SerializedProperty m_IPhoneTargetOSVersion;
        private SerializedProperty m_LocationUsageDescription;
        private SerializedProperty m_LogObjCUncaughtExceptions;
        private SerializedProperty m_MacFullscreenMode;
        private SerializedProperty m_MetalForceHardShadows;
        private SerializedProperty m_MicrophoneUsageDescription;
        private SerializedProperty m_MobileMTRendering;
        private SerializedProperty m_MTRendering;
        private SerializedProperty m_MuteOtherAudioSources;
        private static Dictionary<ScriptingImplementation, GUIContent> m_NiceScriptingBackendNames;
        private Dictionary<string, Texture2D[]> m_PlatformIcons = new Dictionary<string, Texture2D[]>();
        private SerializedProperty m_PreloadedAssets;
        private SerializedProperty m_PreloadShaders;
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
        private SerializedProperty m_VREditorSettings;
        public PlayerSettingsEditorVR m_VRSettings;
        private SerializedProperty m_XboxDeployHeadOrientation;
        private SerializedProperty m_XboxDeployKinectHeadPosition;
        private SerializedProperty m_XboxDeployKinectResources;
        private SerializedProperty m_XboxEnableFitness;
        private SerializedProperty m_XboxEnableHeadOrientation;
        private SerializedProperty m_XboxEnableKinect;
        private SerializedProperty m_XboxEnableKinectAutoTracking;
        private SerializedProperty m_XboxEnableSpeech;
        private SerializedProperty m_XboxSpeechDB;
        private static Texture2D s_WarningIcon;
        private int scriptingDefinesControlID = 0;
        private int selectedPlatform = 0;
        private BuildPlayerWindow.BuildPlatform[] validPlatforms;

        static PlayerSettingsEditor()
        {
            int[] numArray1 = new int[3];
            numArray1[1] = 1;
            numArray1[2] = 2;
            kStereoRenderingMethodValues = numArray1;
            kSinglePassStereoRenderingTargetGroups = new BuildTargetGroup[] { BuildTargetGroup.Standalone, BuildTargetGroup.PS4 };
            kStereoInstancingRenderingTargetGroups = new BuildTargetGroup[] { BuildTargetGroup.WSA };
            defaultIsFullScreen = EditorGUIUtility.TextContent("Default Is Full Screen*");
            GraphicsJobMode[] modeArray1 = new GraphicsJobMode[2];
            modeArray1[1] = GraphicsJobMode.Legacy;
            m_GfxJobModeValues = modeArray1;
            m_GfxJobModeNames = new GUIContent[] { new GUIContent("Native"), new GUIContent("Legacy") };
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
                this.SyncPlatformAPIsList(platform);
            }
        }

        private void ApplyChangedGraphicsAPIList(BuildTarget target, GraphicsDeviceType[] apis, bool firstEntryChanged)
        {
            bool flag = true;
            bool flag2 = false;
            if (firstEntryChanged && WillEditorUseFirstGraphicsAPI(target))
            {
                flag = false;
                if (EditorUtility.DisplayDialog("Changing editor graphics device", "Changing active graphics API requires reloading all graphics objects, it might take a while", "Apply", "Cancel") && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    flag = true;
                    flag2 = true;
                }
            }
            if (flag)
            {
                PlayerSettings.SetGraphicsAPIs(target, apis);
                this.SyncPlatformAPIsList(target);
            }
            else
            {
                this.m_GraphicsDeviceLists.Remove(target);
            }
            if (flag2)
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
                string text = EditorGUIUtility.TextContent(browsePanelTitle).text;
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
            int intValue = prop.intValue;
            int num2 = BuildEnumPopup<T>(uiString, intValue, options, optionNames);
            if (num2 != intValue)
            {
                prop.intValue = num2;
                prop.serializedObject.ApplyModifiedProperties();
            }
        }

        public static int BuildEnumPopup<T>(GUIContent uiString, int selected, T[] options, GUIContent[] optionNames)
        {
            T local = (T) selected;
            int selectedIndex = 0;
            for (int i = 1; i < options.Length; i++)
            {
                if (local.Equals(options[i]))
                {
                    selectedIndex = i;
                    break;
                }
            }
            int index = EditorGUILayout.Popup(uiString, selectedIndex, optionNames, new GUILayoutOption[0]);
            return (int) options[index];
        }

        internal static void BuildFileBoxButton(SerializedProperty prop, string uiString, string directory, string ext)
        {
            BuildFileBoxButton(prop, uiString, directory, ext, null);
        }

        internal static void BuildFileBoxButton(SerializedProperty prop, string uiString, string directory, string ext, System.Action onSelect)
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

        private void CommonSettings()
        {
            Texture2D[] texturedArray;
            EditorGUILayout.PropertyField(this.m_CompanyName, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_ProductName, new GUILayoutOption[0]);
            EditorGUILayout.Space();
            GUI.changed = false;
            string key = "";
            if (!this.m_PlatformIcons.TryGetValue(key, out texturedArray))
            {
                this.m_PlatformIcons[key] = texturedArray = PlayerSettings.GetIconsForPlatform(key);
            }
            int[] iconWidthsForPlatform = PlayerSettings.GetIconWidthsForPlatform(key);
            if (texturedArray.Length != iconWidthsForPlatform.Length)
            {
                texturedArray = new Texture2D[iconWidthsForPlatform.Length];
                this.m_PlatformIcons[key] = texturedArray;
            }
            texturedArray[0] = (Texture2D) EditorGUILayout.ObjectField(Styles.defaultIcon, texturedArray[0], typeof(Texture2D), false, new GUILayoutOption[0]);
            if (GUI.changed)
            {
                PlayerSettings.SetIconsForPlatform(key, texturedArray);
            }
            GUILayout.Space(3f);
            this.m_DefaultCursor.objectReferenceValue = EditorGUILayout.ObjectField(Styles.defaultCursor, this.m_DefaultCursor.objectReferenceValue, typeof(Texture2D), false, new GUILayoutOption[0]);
            EditorGUI.PropertyField(EditorGUI.PrefixLabel(EditorGUILayout.GetControlRect(new GUILayoutOption[0]), 0, Styles.cursorHotspot), this.m_CursorHotspot, GUIContent.none);
        }

        public void DebugAndCrashReportingGUI(BuildPlayerWindow.BuildPlatform platform, BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
        {
            if ((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS))
            {
                if (this.BeginSettingsBox(3, EditorGUIUtility.TextContent("Debugging and crash reporting")))
                {
                    GUILayout.Label(EditorGUIUtility.TextContent("Debugging"), EditorStyles.boldLabel, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_EnableInternalProfiler, EditorGUIUtility.TextContent("Enable Internal Profiler*"), new GUILayoutOption[0]);
                    EditorGUILayout.Space();
                    GUILayout.Label(EditorGUIUtility.TextContent("Crash Reporting"), EditorStyles.boldLabel, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_ActionOnDotNetUnhandledException, EditorGUIUtility.TextContent("On .Net UnhandledException*"), new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_LogObjCUncaughtExceptions, EditorGUIUtility.TextContent("Log Obj-C Uncaught Exceptions*"), new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_EnableCrashReportAPI, EditorGUIUtility.TextContent("Enable CrashReport API*"), new GUILayoutOption[0]);
                    EditorGUILayout.Space();
                }
                this.EndSettingsBox();
            }
        }

        private void DrawGraphicsDeviceElement(BuildTarget target, Rect rect, int index, bool selected, bool focused)
        {
            string text = this.m_GraphicsDeviceLists[target].list[index].ToString();
            switch (text)
            {
                case "Direct3D12":
                    text = "Direct3D12 (Experimental)";
                    break;

                case "Vulkan":
                    text = "Vulkan (Experimental)";
                    break;
            }
            if ((target == BuildTarget.StandaloneOSXUniversal) && (text == "Metal"))
            {
                text = "Metal (Experimental, Player Only)";
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

        private static GUIContent[] GetNiceScriptingBackendNames(ScriptingImplementation[] scriptingBackends)
        {
            if (m_NiceScriptingBackendNames == null)
            {
                Dictionary<ScriptingImplementation, GUIContent> dictionary = new Dictionary<ScriptingImplementation, GUIContent> {
                    { 
                        ScriptingImplementation.Mono2x,
                        EditorGUIUtility.TextContent("Mono2x")
                    },
                    { 
                        ScriptingImplementation.WinRTDotNET,
                        EditorGUIUtility.TextContent(".NET")
                    },
                    { 
                        ScriptingImplementation.IL2CPP,
                        EditorGUIUtility.TextContent("IL2CPP")
                    }
                };
                m_NiceScriptingBackendNames = dictionary;
            }
            GUIContent[] contentArray = new GUIContent[scriptingBackends.Length];
            for (int i = 0; i < scriptingBackends.Length; i++)
            {
                if (!m_NiceScriptingBackendNames.ContainsKey(scriptingBackends[i]))
                {
                    throw new NotImplementedException("Missing nice scripting implementation name");
                }
                contentArray[i] = m_NiceScriptingBackendNames[scriptingBackends[i]];
            }
            return contentArray;
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
                    Undo.RecordObject(base.target, "Changed Graphics API Settings");
                    PlayerSettings.SetUseDefaultGraphicsAPIs(storey.targetPlatform, useDefaultGraphicsAPIs);
                }
                if (!useDefaultGraphicsAPIs)
                {
                    <GraphicsAPIsGUIOnePlatform>c__AnonStorey1 storey2 = new <GraphicsAPIsGUIOnePlatform>c__AnonStorey1 {
                        <>f__ref$0 = storey
                    };
                    if (WillEditorUseFirstGraphicsAPI(storey.targetPlatform))
                    {
                        EditorGUILayout.HelpBox("Reordering the list will switch editor to the first available platform", MessageType.Info, true);
                    }
                    storey2.displayTitle = "Graphics APIs";
                    if (platformTitle != null)
                    {
                        storey2.displayTitle = storey2.displayTitle + platformTitle;
                    }
                    if (!this.m_GraphicsDeviceLists.ContainsKey(storey.targetPlatform))
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
                        this.m_GraphicsDeviceLists.Add(storey.targetPlatform, list2);
                    }
                    this.m_GraphicsDeviceLists[storey.targetPlatform].DoLayoutList();
                    this.OpenGLES31OptionsGUI(targetGroup, storey.targetPlatform);
                }
            }
        }

        private void IconSectionGUI(BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
        {
            if (this.BeginSettingsBox(1, EditorGUIUtility.TextContent("Icon")))
            {
                bool flag = true;
                if (settingsExtension != null)
                {
                    flag = settingsExtension.UsesStandardIcons();
                }
                if (flag)
                {
                    bool flag2 = this.selectedPlatform < 0;
                    BuildPlayerWindow.BuildPlatform platform = null;
                    targetGroup = BuildTargetGroup.Standalone;
                    string key = "";
                    if (!flag2)
                    {
                        platform = this.validPlatforms[this.selectedPlatform];
                        targetGroup = platform.targetGroup;
                        key = platform.name;
                    }
                    bool enabled = GUI.enabled;
                    if ((targetGroup == BuildTargetGroup.SamsungTV) || (targetGroup == BuildTargetGroup.WebGL))
                    {
                        this.ShowNoSettings();
                        EditorGUILayout.Space();
                    }
                    else if (targetGroup != BuildTargetGroup.WSA)
                    {
                        Texture2D[] texturedArray;
                        if (!this.m_PlatformIcons.TryGetValue(key, out texturedArray))
                        {
                            this.m_PlatformIcons[key] = texturedArray = PlayerSettings.GetIconsForPlatform(key);
                        }
                        int[] iconWidthsForPlatform = PlayerSettings.GetIconWidthsForPlatform(key);
                        int[] iconHeightsForPlatform = PlayerSettings.GetIconHeightsForPlatform(key);
                        bool flag4 = true;
                        if (!flag2)
                        {
                            GUI.changed = false;
                            flag4 = texturedArray.Length == iconWidthsForPlatform.Length;
                            flag4 = GUILayout.Toggle(flag4, "Override for " + platform.title.text, new GUILayoutOption[0]);
                            GUI.enabled = enabled && flag4;
                            if (GUI.changed || (!flag4 && (texturedArray.Length > 0)))
                            {
                                if (flag4)
                                {
                                    texturedArray = new Texture2D[iconWidthsForPlatform.Length];
                                }
                                else
                                {
                                    texturedArray = new Texture2D[0];
                                }
                                this.m_PlatformIcons[key] = texturedArray;
                                if (GUI.changed)
                                {
                                    PlayerSettings.SetIconsForPlatform(key, texturedArray);
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
                                texturedArray[i] = (Texture2D) EditorGUI.ObjectField(new Rect((((rect3.x + num4) - 96f) - 64f) - 6f, rect3.y, (float) num5, (float) num6), texturedArray[i], typeof(Texture2D), false);
                            }
                            Rect position = new Rect((rect3.x + num4) - 96f, rect3.y, (float) num2, (float) b);
                            Texture2D image = PlayerSettings.GetIconForPlatformAtSize(key, iconWidthsForPlatform[i], iconHeightsForPlatform[i]);
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
                            PlayerSettings.SetIconsForPlatform(key, texturedArray);
                        }
                        GUI.enabled = enabled;
                        if ((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS))
                        {
                            EditorGUILayout.PropertyField(this.m_UIPrerenderedIcon, EditorGUIUtility.TextContent("Prerendered Icon"), new GUILayoutOption[0]);
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

        private void KinectGUI()
        {
            GUILayout.Label(EditorGUIUtility.TextContent("Kinect"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            this.m_XboxEnableKinect.boolValue = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Enable Kinect"), this.m_XboxEnableKinect.boolValue, new GUILayoutOption[0]);
            if (this.m_XboxEnableKinect.boolValue)
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.Space(10f);
                this.m_XboxEnableHeadOrientation.boolValue = GUILayout.Toggle(this.m_XboxEnableHeadOrientation.boolValue, new GUIContent("Head Orientation", "Head orientation support"), new GUILayoutOption[0]);
                this.m_XboxEnableKinectAutoTracking.boolValue = GUILayout.Toggle(this.m_XboxEnableKinectAutoTracking.boolValue, new GUIContent("Auto Tracking", "Automatic player tracking"), new GUILayoutOption[0]);
                this.m_XboxEnableFitness.boolValue = GUILayout.Toggle(this.m_XboxEnableFitness.boolValue, new GUIContent("Fitness", "Fitness support"), new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.Space(10f);
                this.m_XboxEnableSpeech.boolValue = GUILayout.Toggle(this.m_XboxEnableSpeech.boolValue, new GUIContent("Speech", "Speech Recognition Support"), new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
                this.m_XboxDeployKinectResources.boolValue = true;
                if (this.m_XboxEnableHeadOrientation.boolValue)
                {
                    this.m_XboxDeployHeadOrientation.boolValue = true;
                }
            }
            GUILayout.Label(EditorGUIUtility.TextContent("Deploy Kinect resources"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(10f);
            GUI.enabled = !this.m_XboxEnableKinect.boolValue;
            this.m_XboxDeployKinectResources.boolValue = GUILayout.Toggle(this.m_XboxDeployKinectResources.boolValue, new GUIContent("Base", "Identity and Skeleton Database files"), new GUILayoutOption[0]);
            GUI.enabled = !(this.m_XboxEnableHeadOrientation.boolValue && this.m_XboxEnableKinect.boolValue);
            this.m_XboxDeployHeadOrientation.boolValue = GUILayout.Toggle(this.m_XboxDeployHeadOrientation.boolValue, new GUIContent("Head Orientation", "Head orientation database"), new GUILayoutOption[0]);
            GUI.enabled = true;
            this.m_XboxDeployKinectHeadPosition.boolValue = GUILayout.Toggle(this.m_XboxDeployKinectHeadPosition.boolValue, new GUIContent("Head Position", "Head position database"), new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            GUILayout.Label(EditorGUIUtility.TextContent("Speech"), new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(10f);
            this.m_XboxSpeechDB.intValue ^= (GUILayout.Toggle((this.m_XboxSpeechDB.intValue & 1) != 0, new GUIContent("en-US", "Speech database: English - US, Canada"), new GUILayoutOption[0]) == ((this.m_XboxSpeechDB.intValue & 1) != 0)) ? 0 : 1;
            this.m_XboxSpeechDB.intValue ^= (GUILayout.Toggle((this.m_XboxSpeechDB.intValue & 2) != 0, new GUIContent("fr-CA", "Speech database: French - Canada"), new GUILayoutOption[0]) == ((this.m_XboxSpeechDB.intValue & 2) != 0)) ? 0 : 2;
            this.m_XboxSpeechDB.intValue ^= (GUILayout.Toggle((this.m_XboxSpeechDB.intValue & 4) != 0, new GUIContent("en-GB", "Speech database: English - United Kingdom, Ireland"), new GUILayoutOption[0]) == ((this.m_XboxSpeechDB.intValue & 4) != 0)) ? 0 : 4;
            this.m_XboxSpeechDB.intValue ^= (GUILayout.Toggle((this.m_XboxSpeechDB.intValue & 8) != 0, new GUIContent("es-MX", "Speech database: Spanish - Mexico"), new GUILayoutOption[0]) == ((this.m_XboxSpeechDB.intValue & 8) != 0)) ? 0 : 8;
            this.m_XboxSpeechDB.intValue ^= (GUILayout.Toggle((this.m_XboxSpeechDB.intValue & 0x10) != 0, new GUIContent("ja-JP", "Speech database: Japanese - Japan"), new GUILayoutOption[0]) == ((this.m_XboxSpeechDB.intValue & 0x10) != 0)) ? 0 : 0x10;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(10f);
            this.m_XboxSpeechDB.intValue ^= (GUILayout.Toggle((this.m_XboxSpeechDB.intValue & 0x20) != 0, new GUIContent("fr-FR", "Speech database: French - France, Switzerland"), new GUILayoutOption[0]) == ((this.m_XboxSpeechDB.intValue & 0x20) != 0)) ? 0 : 0x20;
            this.m_XboxSpeechDB.intValue ^= (GUILayout.Toggle((this.m_XboxSpeechDB.intValue & 0x40) != 0, new GUIContent("es-ES", "Speech database: Spanish - Spain"), new GUILayoutOption[0]) == ((this.m_XboxSpeechDB.intValue & 0x40) != 0)) ? 0 : 0x40;
            this.m_XboxSpeechDB.intValue ^= (GUILayout.Toggle((this.m_XboxSpeechDB.intValue & 0x80) != 0, new GUIContent("de-DE", "Speech database: German - Germany, Austria, Switzerland"), new GUILayoutOption[0]) == ((this.m_XboxSpeechDB.intValue & 0x80) != 0)) ? 0 : 0x80;
            this.m_XboxSpeechDB.intValue ^= (GUILayout.Toggle((this.m_XboxSpeechDB.intValue & 0x100) != 0, new GUIContent("it-IT", "Speech database: Italian - Italy"), new GUILayoutOption[0]) == ((this.m_XboxSpeechDB.intValue & 0x100) != 0)) ? 0 : 0x100;
            this.m_XboxSpeechDB.intValue ^= (GUILayout.Toggle((this.m_XboxSpeechDB.intValue & 0x200) != 0, new GUIContent("en-AU", "Speech database: English - Australia, New Zealand"), new GUILayoutOption[0]) == ((this.m_XboxSpeechDB.intValue & 0x200) != 0)) ? 0 : 0x200;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(10f);
            this.m_XboxSpeechDB.intValue ^= (GUILayout.Toggle((this.m_XboxSpeechDB.intValue & 0x400) != 0, new GUIContent("pt-BR", "Speech database: Portuguese - Brazil"), new GUILayoutOption[0]) == ((this.m_XboxSpeechDB.intValue & 0x400) != 0)) ? 0 : 0x400;
            GUILayout.EndHorizontal();
        }

        private void OnDisable()
        {
        }

        private void OnEnable()
        {
            this.validPlatforms = BuildPlayerWindow.GetValidPlatforms().ToArray();
            this.m_IPhoneSdkVersion = this.FindPropertyAssert("iPhoneSdkVersion");
            this.m_IPhoneTargetOSVersion = this.FindPropertyAssert("iOSTargetOSVersionString");
            this.m_IPhoneStrippingLevel = this.FindPropertyAssert("iPhoneStrippingLevel");
            this.m_IPhoneBuildNumber = this.FindPropertyAssert("iPhoneBuildNumber");
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
            this.m_ApplicationBundleIdentifier = base.serializedObject.FindProperty("bundleIdentifier");
            if (this.m_ApplicationBundleIdentifier == null)
            {
                this.m_ApplicationBundleIdentifier = this.FindPropertyAssert("iPhoneBundleIdentifier");
            }
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
            this.m_ApiCompatibilityLevel = this.FindPropertyAssert("apiCompatibilityLevel");
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
            this.m_PreloadShaders = this.FindPropertyAssert("preloadShaders");
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
            this.m_XboxDeployKinectResources = this.FindPropertyAssert("XboxDeployKinectResources");
            this.m_XboxEnableKinect = this.FindPropertyAssert("xboxEnableKinect");
            this.m_XboxEnableKinectAutoTracking = this.FindPropertyAssert("xboxEnableKinectAutoTracking");
            this.m_XboxEnableSpeech = this.FindPropertyAssert("xboxEnableSpeech");
            this.m_XboxSpeechDB = this.FindPropertyAssert("xboxSpeechDB");
            this.m_XboxEnableFitness = this.FindPropertyAssert("xboxEnableFitness");
            this.m_XboxEnableHeadOrientation = this.FindPropertyAssert("xboxEnableHeadOrientation");
            this.m_XboxDeployHeadOrientation = this.FindPropertyAssert("xboxDeployKinectHeadOrientation");
            this.m_XboxDeployKinectHeadPosition = this.FindPropertyAssert("xboxDeployKinectHeadPosition");
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
            this.m_VREditorSettings = this.FindPropertyAssert("vrEditorSettings");
            this.m_VRSettings = new PlayerSettingsEditorVR(this.m_VREditorSettings);
            this.splashScreenEditor.OnEnable();
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
            BuildPlayerWindow.BuildPlatform platform = this.validPlatforms[this.selectedPlatform];
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
            GUILayout.Label(EditorGUIUtility.TextContent("Configuration"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            IScriptingImplementations scriptingImplementations = ModuleManager.GetScriptingImplementations(targetGroup);
            if (scriptingImplementations == null)
            {
                BuildDisabledEnumPopup(EditorGUIUtility.TextContent("Default"), EditorGUIUtility.TextContent("Scripting Backend"));
            }
            else
            {
                int num2;
                ScriptingImplementation[] options = scriptingImplementations.Enabled();
                int scriptingBackend = (int) PlayerSettings.GetScriptingBackend(targetGroup);
                if (targetGroup == BuildTargetGroup.tvOS)
                {
                    num2 = 1;
                    BuildDisabledEnumPopup(new GUIContent("IL2CPP"), EditorGUIUtility.TextContent("Scripting Backend"));
                }
                else
                {
                    num2 = BuildEnumPopup<ScriptingImplementation>(EditorGUIUtility.TextContent("Scripting Backend"), scriptingBackend, options, GetNiceScriptingBackendNames(options));
                }
                if (num2 != scriptingBackend)
                {
                    PlayerSettings.SetScriptingBackend(targetGroup, (ScriptingImplementation) num2);
                }
            }
            if ((((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS)) || (targetGroup == BuildTargetGroup.Android)) || (targetGroup == BuildTargetGroup.WSA))
            {
                if (targetGroup == BuildTargetGroup.iPhone)
                {
                    EditorGUILayout.PropertyField(this.m_TargetDevice, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_IPhoneSdkVersion, EditorGUIUtility.TextContent("Target SDK"), new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_IPhoneTargetOSVersion, EditorGUIUtility.TextContent("Target minimum iOS Version"), new GUILayoutOption[0]);
                    if (this.ParseIosVersion(this.m_IPhoneTargetOSVersion.stringValue).Major == 0)
                    {
                        this.m_IPhoneTargetOSVersion.stringValue = "7.0";
                    }
                }
                if (targetGroup == BuildTargetGroup.tvOS)
                {
                    EditorGUILayout.PropertyField(this.m_tvOSSdkVersion, EditorGUIUtility.TextContent("Target SDK"), new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_tvOSTargetOSVersion, EditorGUIUtility.TextContent("Target minimum tvOS Version"), new GUILayoutOption[0]);
                    if (this.ParseIosVersion(this.m_tvOSTargetOSVersion.stringValue).Major == 0)
                    {
                        this.m_tvOSTargetOSVersion.stringValue = "9.0";
                    }
                }
                if ((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS))
                {
                    EditorGUILayout.PropertyField(this.m_useOnDemandResources, EditorGUIUtility.TextContent("Use on demand resources*"), new GUILayoutOption[0]);
                    if (this.m_useOnDemandResources.boolValue && (this.ParseIosVersion(this.m_IPhoneTargetOSVersion.stringValue).Major < 9))
                    {
                        this.m_IPhoneTargetOSVersion.stringValue = "9.0";
                    }
                }
                if (((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS)) || (targetGroup == BuildTargetGroup.WSA))
                {
                    EditorGUILayout.PropertyField(this.m_AccelerometerFrequency, EditorGUIUtility.TextContent("Accelerometer Frequency*"), new GUILayoutOption[0]);
                }
                if ((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS))
                {
                    EditorGUILayout.PropertyField(this.m_CameraUsageDescription, EditorGUIUtility.TextContent("Camera Usage Description*"), new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_LocationUsageDescription, EditorGUIUtility.TextContent("Location Usage Description*"), new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_MicrophoneUsageDescription, EditorGUIUtility.TextContent("Microphone Usage Description*"), new GUILayoutOption[0]);
                }
                if (((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS)) || (targetGroup == BuildTargetGroup.Android))
                {
                    EditorGUILayout.PropertyField(this.m_MuteOtherAudioSources, EditorGUIUtility.TextContent("Mute Other Audio Sources*"), new GUILayoutOption[0]);
                }
                if ((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS))
                {
                    if (targetGroup == BuildTargetGroup.iPhone)
                    {
                        EditorGUILayout.PropertyField(this.m_PrepareIOSForRecording, EditorGUIUtility.TextContent("Prepare iOS for Recording"), new GUILayoutOption[0]);
                    }
                    EditorGUILayout.PropertyField(this.m_UIRequiresPersistentWiFi, EditorGUIUtility.TextContent("Requires Persistent WiFi*"), new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_IOSAllowHTTPDownload, EditorGUIUtility.TextContent("Allow downloads over HTTP (nonsecure)*"), new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_IOSURLSchemes, EditorGUIUtility.TextContent("Supported URL schemes*"), true, new GUILayoutOption[0]);
                }
            }
            using (new EditorGUI.DisabledScope(!Application.HasProLicense()))
            {
                bool flag3 = !this.m_SubmitAnalytics.boolValue;
                bool flag4 = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Disable HW Statistics*|Disables HW Statistics (Pro Only)"), flag3, new GUILayoutOption[0]);
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
            EditorGUILayout.LabelField(EditorGUIUtility.TextContent("Scripting Define Symbols*"), new GUILayoutOption[0]);
            EditorGUI.BeginChangeCheck();
            string defines = EditorGUILayout.DelayedTextField(PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup), EditorStyles.textField, new GUILayoutOption[0]);
            this.scriptingDefinesControlID = EditorGUIUtility.s_LastControlID;
            if (EditorGUI.EndChangeCheck())
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
            }
            EditorGUILayout.Space();
        }

        public void OtherSectionGUI(BuildPlayerWindow.BuildPlatform platform, BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
        {
            if (this.BeginSettingsBox(4, EditorGUIUtility.TextContent("Other Settings")))
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
                GUILayout.Label(EditorGUIUtility.TextContent("Identification"), EditorStyles.boldLabel, new GUILayoutOption[0]);
                if (settingsExtension.HasBundleIdentifier())
                {
                    EditorGUILayout.PropertyField(this.m_ApplicationBundleIdentifier, EditorGUIUtility.TextContent("Bundle Identifier"), new GUILayoutOption[0]);
                }
                EditorGUILayout.PropertyField(this.m_ApplicationBundleVersion, EditorGUIUtility.TextContent("Version*"), new GUILayoutOption[0]);
                if ((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS))
                {
                    EditorGUILayout.PropertyField(this.m_IPhoneBuildNumber, EditorGUIUtility.TextContent("Build"), new GUILayoutOption[0]);
                }
                if (settingsExtension != null)
                {
                    settingsExtension.IdentificationSectionGUI();
                }
                EditorGUILayout.Space();
            }
        }

        private void OtherSectionLoggingGUI()
        {
            GUILayout.Label(EditorGUIUtility.TextContent("Logging*"), EditorStyles.boldLabel, new GUILayoutOption[0]);
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
            GUILayout.Label(EditorGUIUtility.TextContent("Optimization"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            if (targetGroup == BuildTargetGroup.WiiU)
            {
                this.ShowDisabledFakeEnumPopup(FakeEnum.WiiUSubset);
            }
            else if (targetGroup == BuildTargetGroup.WSA)
            {
                this.ShowDisabledFakeEnumPopup(FakeEnum.WSASubset);
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(this.m_ApiCompatibilityLevel, EditorGUIUtility.TextContent("API Compatibility level*"), new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    PlayerSettings.SetApiCompatibilityInternal(this.m_ApiCompatibilityLevel.intValue);
                }
            }
            EditorGUILayout.PropertyField(this.m_BakeCollisionMeshes, EditorGUIUtility.TextContent("Prebake Collision Meshes*|Bake collision data into the meshes on build time"), new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_PreloadShaders, EditorGUIUtility.TextContent("Preload Shaders"), new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_PreloadedAssets, EditorGUIUtility.TextContent("Preloaded Assets|Assets to load at start up in the player"), true, new GUILayoutOption[0]);
            if (((((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS)) || ((targetGroup == BuildTargetGroup.XboxOne) || (targetGroup == BuildTargetGroup.WiiU))) || (targetGroup == BuildTargetGroup.PS4)) || (targetGroup == BuildTargetGroup.PSP2))
            {
                EditorGUILayout.PropertyField(this.m_AotOptions, EditorGUIUtility.TextContent("AOT Compilation Options*"), new GUILayoutOption[0]);
            }
            if ((((((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS)) || ((targetGroup == BuildTargetGroup.Android) || (targetGroup == BuildTargetGroup.Tizen))) || (((targetGroup == BuildTargetGroup.WebGL) || (targetGroup == BuildTargetGroup.WiiU)) || ((targetGroup == BuildTargetGroup.PSP2) || (targetGroup == BuildTargetGroup.PS4)))) || (targetGroup == BuildTargetGroup.XboxOne)) || (targetGroup == BuildTargetGroup.WSA))
            {
                ScriptingImplementation scriptingBackend = PlayerSettings.GetScriptingBackend(targetGroup);
                if ((targetGroup == BuildTargetGroup.WebGL) || (scriptingBackend == ScriptingImplementation.IL2CPP))
                {
                    EditorGUILayout.PropertyField(this.m_StripEngineCode, EditorGUIUtility.TextContent("Strip Engine Code*|Strip Unused Engine Code - Note that byte code stripping of managed assemblies is always enabled for the IL2CPP scripting backend."), new GUILayoutOption[0]);
                }
                else if (scriptingBackend != ScriptingImplementation.WinRTDotNET)
                {
                    EditorGUILayout.PropertyField(this.m_IPhoneStrippingLevel, EditorGUIUtility.TextContent("Stripping Level*"), new GUILayoutOption[0]);
                }
            }
            if ((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS))
            {
                EditorGUILayout.PropertyField(this.m_IPhoneScriptCallOptimization, EditorGUIUtility.TextContent("Script Call Optimization*"), new GUILayoutOption[0]);
            }
            if (targetGroup == BuildTargetGroup.Android)
            {
                EditorGUILayout.PropertyField(this.m_AndroidProfiler, EditorGUIUtility.TextContent("Enable Internal Profiler"), new GUILayoutOption[0]);
            }
            EditorGUILayout.Space();
            VertexChannelCompressionFlags intValue = (VertexChannelCompressionFlags) this.m_VertexChannelCompressionMask.intValue;
            intValue = (VertexChannelCompressionFlags) EditorGUILayout.EnumMaskPopup(Styles.vertexChannelCompressionMask, intValue, new GUILayoutOption[0]);
            this.m_VertexChannelCompressionMask.intValue = (int) intValue;
            if (targetGroup != BuildTargetGroup.PSM)
            {
                EditorGUILayout.PropertyField(this.m_StripUnusedMeshComponents, EditorGUIUtility.TextContent("Optimize Mesh Data*|Remove unused mesh components"), new GUILayoutOption[0]);
            }
            if ((targetGroup == BuildTargetGroup.PSP2) || (targetGroup == BuildTargetGroup.PSM))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(this.m_VideoMemoryForVertexBuffers, EditorGUIUtility.TextContent("Mesh Video Mem*|How many megabytes of video memory to use for mesh data before we use main memory"), new GUILayoutOption[0]);
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

        private void OtherSectionRenderingGUI(BuildPlayerWindow.BuildPlatform platform, BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
        {
            int num;
            int num2;
            GUILayout.Label(EditorGUIUtility.TextContent("Rendering"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            if ((((targetGroup == BuildTargetGroup.Standalone) || (targetGroup == BuildTargetGroup.iPhone)) || ((targetGroup == BuildTargetGroup.tvOS) || (targetGroup == BuildTargetGroup.Android))) || (((targetGroup == BuildTargetGroup.PS4) || (targetGroup == BuildTargetGroup.WSA)) || ((targetGroup == BuildTargetGroup.WiiU) || (targetGroup == BuildTargetGroup.WebGL))))
            {
                using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(this.m_ActiveColorSpace, EditorGUIUtility.TextContent("Color Space*"), new GUILayoutOption[0]);
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
                        if (!(typeArray3.Contains<GraphicsDeviceType>(GraphicsDeviceType.OpenGLES3) && !typeArray3.Contains<GraphicsDeviceType>(GraphicsDeviceType.OpenGLES2)) || (PlayerSettings.Android.minSdkVersion < AndroidSdkVersions.AndroidApiLevel18))
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
            this.GraphicsAPIsGUI(targetGroup, platform.DefaultTarget);
            if ((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS))
            {
                this.m_MetalForceHardShadows.boolValue = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Force hard shadows on Metal*"), this.m_MetalForceHardShadows.boolValue, new GUILayoutOption[0]);
            }
            if (((targetGroup == BuildTargetGroup.PSP2) || (targetGroup == BuildTargetGroup.PSM)) || ((targetGroup == BuildTargetGroup.Android) || (targetGroup == BuildTargetGroup.SamsungTV)))
            {
                if (this.IsMobileTarget(targetGroup))
                {
                    this.m_MobileMTRendering.boolValue = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Multithreaded Rendering*"), this.m_MobileMTRendering.boolValue, new GUILayoutOption[0]);
                }
                else
                {
                    this.m_MTRendering.boolValue = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Multithreaded Rendering*"), this.m_MTRendering.boolValue, new GUILayoutOption[0]);
                }
            }
            else if ((targetGroup == BuildTargetGroup.PSP2) || (targetGroup == BuildTargetGroup.PSM))
            {
                if (Unsupported.IsDeveloperBuild())
                {
                    this.m_MTRendering.boolValue = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Multithreaded Rendering*"), this.m_MTRendering.boolValue, new GUILayoutOption[0]);
                }
                else
                {
                    this.m_MTRendering.boolValue = true;
                }
            }
            bool flag4 = true;
            bool flag5 = true;
            if (settingsExtension != null)
            {
                flag4 = settingsExtension.SupportsStaticBatching();
                flag5 = settingsExtension.SupportsDynamicBatching();
            }
            PlayerSettings.GetBatchingForPlatform(platform.DefaultTarget, out num, out num2);
            bool flag6 = false;
            if (!flag4 && (num == 1))
            {
                num = 0;
                flag6 = true;
            }
            if (!flag5 && (num2 == 1))
            {
                num2 = 0;
                flag6 = true;
            }
            if (flag6)
            {
                PlayerSettings.SetBatchingForPlatform(platform.DefaultTarget, num, num2);
            }
            EditorGUI.BeginChangeCheck();
            using (new EditorGUI.DisabledScope(!flag4))
            {
                if (GUI.enabled)
                {
                    num = !EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Static Batching"), num != 0, new GUILayoutOption[0]) ? 0 : 1;
                }
                else
                {
                    EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Static Batching"), false, new GUILayoutOption[0]);
                }
            }
            using (new EditorGUI.DisabledScope(!flag5))
            {
                num2 = !EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Dynamic Batching"), num2 != 0, new GUILayoutOption[0]) ? 0 : 1;
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(base.target, "Changed Batching Settings");
                PlayerSettings.SetBatchingForPlatform(platform.DefaultTarget, num, num2);
            }
            if ((((targetGroup == BuildTargetGroup.WiiU) || (targetGroup == BuildTargetGroup.WiiU)) || ((targetGroup == BuildTargetGroup.Standalone) || (targetGroup == BuildTargetGroup.iPhone))) || ((((targetGroup == BuildTargetGroup.tvOS) || (targetGroup == BuildTargetGroup.Android)) || ((targetGroup == BuildTargetGroup.PSP2) || (targetGroup == BuildTargetGroup.PS4))) || ((targetGroup == BuildTargetGroup.PSM) || (targetGroup == BuildTargetGroup.WSA))))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(this.m_SkinOnGPU, (targetGroup == BuildTargetGroup.PS4) ? EditorGUIUtility.TextContent("Compute Skinning*|Use Compute pipeline for Skinning") : EditorGUIUtility.TextContent("GPU Skinning*|Use DX11/ES3 GPU Skinning"), new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    ShaderUtil.RecreateSkinnedMeshResources();
                }
            }
            EditorGUILayout.PropertyField(this.m_GraphicsJobs, EditorGUIUtility.TextContent("Graphics Jobs (Experimental)*"), new GUILayoutOption[0]);
            if (this.PlatformSupportsGfxJobModes(targetGroup))
            {
                using (new EditorGUI.DisabledScope(!this.m_GraphicsJobs.boolValue))
                {
                    int graphicsJobMode = (int) PlayerSettings.graphicsJobMode;
                    int num4 = BuildEnumPopup<GraphicsJobMode>(Styles.graphicsJobsMode, graphicsJobMode, m_GfxJobModeValues, m_GfxJobModeNames);
                    if (num4 != graphicsJobMode)
                    {
                        PlayerSettings.graphicsJobMode = (GraphicsJobMode) num4;
                    }
                }
            }
            if (this.m_VRSettings.TargetGroupSupportsVirtualReality(targetGroup))
            {
                this.m_VRSettings.DevicesGUI(targetGroup);
                using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
                {
                    bool flag7 = TargetSupportsSinglePassStereoRendering(targetGroup);
                    bool flag8 = TargetSupportsStereoInstancingRendering(targetGroup);
                    if (PlayerSettings.virtualRealitySupported)
                    {
                        int num5 = (1 + (!flag7 ? 0 : 1)) + (!flag8 ? 0 : 1);
                        GUIContent[] displayedOptions = new GUIContent[num5];
                        int[] optionValues = new int[num5];
                        int index = 0;
                        displayedOptions[index] = Styles.kStereoRenderingMethodsAll[0];
                        optionValues[index++] = kStereoRenderingMethodValues[0];
                        if (flag7)
                        {
                            displayedOptions[index] = Styles.kStereoRenderingMethodsAll[1];
                            optionValues[index++] = kStereoRenderingMethodValues[1];
                        }
                        if (flag8)
                        {
                            displayedOptions[index] = Styles.kStereoRenderingMethodsAll[2];
                            optionValues[index++] = kStereoRenderingMethodValues[2];
                        }
                        if (!flag8 && (this.m_StereoRenderingPath.intValue == 2))
                        {
                            this.m_StereoRenderingPath.intValue = 1;
                        }
                        if (!flag7 && (this.m_StereoRenderingPath.intValue == 1))
                        {
                            this.m_StereoRenderingPath.intValue = 0;
                        }
                        EditorGUILayout.IntPopup(this.m_StereoRenderingPath, displayedOptions, optionValues, EditorGUIUtility.TextContent("Stereo Rendering Method*"), new GUILayoutOption[0]);
                    }
                }
            }
            if (TargetSupportsProtectedGraphicsMem(targetGroup))
            {
                PlayerSettings.protectGraphicsMemory = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Protect Graphics Memory|Protect GPU memory from being read (on supported devices). Will prevent user from taking screenshots"), PlayerSettings.protectGraphicsMemory, new GUILayoutOption[0]);
            }
            EditorGUILayout.Space();
        }

        private Version ParseIosVersion(string text)
        {
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
                if (this.BeginSettingsBox(5, EditorGUIUtility.TextContent("Publishing Settings")))
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
            if (this.BeginSettingsBox(0, EditorGUIUtility.TextContent("Resolution and Presentation")))
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
                    GUILayout.Label(EditorGUIUtility.TextContent("Resolution"), EditorStyles.boldLabel, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_DefaultIsFullScreen, defaultIsFullScreen, new GUILayoutOption[0]);
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
                        EditorGUILayout.PropertyField(this.m_DefaultScreenWidth, EditorGUIUtility.TextContent("Default Screen Width"), new GUILayoutOption[0]);
                        if (EditorGUI.EndChangeCheck() && (this.m_DefaultScreenWidth.intValue < 1))
                        {
                            this.m_DefaultScreenWidth.intValue = 1;
                        }
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(this.m_DefaultScreenHeight, EditorGUIUtility.TextContent("Default Screen Height"), new GUILayoutOption[0]);
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
                    EditorGUILayout.PropertyField(this.m_RunInBackground, EditorGUIUtility.TextContent("Run In Background*"), new GUILayoutOption[0]);
                }
                if ((settingsExtension != null) && settingsExtension.SupportsOrientation())
                {
                    GUILayout.Label(EditorGUIUtility.TextContent("Orientation"), EditorStyles.boldLabel, new GUILayoutOption[0]);
                    using (new EditorGUI.DisabledScope(PlayerSettings.virtualRealitySupported))
                    {
                        EditorGUILayout.PropertyField(this.m_DefaultScreenOrientation, EditorGUIUtility.TextContent("Default Orientation*"), new GUILayoutOption[0]);
                        if (PlayerSettings.virtualRealitySupported)
                        {
                            EditorGUILayout.HelpBox(EditorGUIUtility.TextContent("This setting is overridden by Virtual Reality Support.").text, MessageType.Info);
                        }
                        if (this.m_DefaultScreenOrientation.enumValueIndex == 4)
                        {
                            if ((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.Tizen))
                            {
                                EditorGUILayout.PropertyField(this.m_UseOSAutoRotation, EditorGUIUtility.TextContent("Use Animated Autorotation|If set OS native animated autorotation method will be used. Otherwise orientation will be changed immediately."), new GUILayoutOption[0]);
                            }
                            EditorGUI.indentLevel++;
                            GUILayout.Label(EditorGUIUtility.TextContent("Allowed Orientations for Auto Rotation"), EditorStyles.boldLabel, new GUILayoutOption[0]);
                            if (!(((this.m_AllowedAutoRotateToPortrait.boolValue || this.m_AllowedAutoRotateToPortraitUpsideDown.boolValue) || this.m_AllowedAutoRotateToLandscapeRight.boolValue) || this.m_AllowedAutoRotateToLandscapeLeft.boolValue))
                            {
                                this.m_AllowedAutoRotateToPortrait.boolValue = true;
                                Debug.LogError("All orientations are disabled. Allowing portrait");
                            }
                            EditorGUILayout.PropertyField(this.m_AllowedAutoRotateToPortrait, EditorGUIUtility.TextContent("Portrait"), new GUILayoutOption[0]);
                            if ((targetGroup != BuildTargetGroup.WSA) || (EditorUserBuildSettings.wsaSDK != WSASDK.PhoneSDK81))
                            {
                                EditorGUILayout.PropertyField(this.m_AllowedAutoRotateToPortraitUpsideDown, EditorGUIUtility.TextContent("Portrait Upside Down"), new GUILayoutOption[0]);
                            }
                            EditorGUILayout.PropertyField(this.m_AllowedAutoRotateToLandscapeRight, EditorGUIUtility.TextContent("Landscape Right"), new GUILayoutOption[0]);
                            EditorGUILayout.PropertyField(this.m_AllowedAutoRotateToLandscapeLeft, EditorGUIUtility.TextContent("Landscape Left"), new GUILayoutOption[0]);
                            EditorGUI.indentLevel--;
                        }
                    }
                }
                if (targetGroup == BuildTargetGroup.iPhone)
                {
                    GUILayout.Label(EditorGUIUtility.TextContent("Multitasking Support"), EditorStyles.boldLabel, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_UIRequiresFullScreen, EditorGUIUtility.TextContent("Requires Fullscreen"), new GUILayoutOption[0]);
                    EditorGUILayout.Space();
                    GUILayout.Label(EditorGUIUtility.TextContent("Status Bar"), EditorStyles.boldLabel, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_UIStatusBarHidden, EditorGUIUtility.TextContent("Status Bar Hidden"), new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_UIStatusBarStyle, EditorGUIUtility.TextContent("Status Bar Style"), new GUILayoutOption[0]);
                    EditorGUILayout.Space();
                }
                EditorGUILayout.Space();
                if (targetGroup == BuildTargetGroup.Standalone)
                {
                    GUILayout.Label(EditorGUIUtility.TextContent("Standalone Player Options"), EditorStyles.boldLabel, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_CaptureSingleScreen, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_DisplayResolutionDialog, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_UsePlayerLog, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_ResizableWindow, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_UseMacAppStoreValidation, EditorGUIUtility.TempContent("Mac App Store Validation"), new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_MacFullscreenMode, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_D3D9FullscreenMode, EditorGUIUtility.TempContent("D3D9 Fullscreen Mode"), new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_D3D11FullscreenMode, EditorGUIUtility.TempContent("D3D11 Fullscreen Mode"), new GUILayoutOption[0]);
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
                        EditorGUILayout.PropertyField(this.m_VisibleInBackground, EditorGUIUtility.TempContent("Visible In Background"), new GUILayoutOption[0]);
                    }
                    EditorGUILayout.PropertyField(this.m_AllowFullscreenSwitch, EditorGUIUtility.TempContent("Allow Fullscreen Switch"), new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_ForceSingleInstance, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_SupportedAspectRatios, true, new GUILayoutOption[0]);
                    EditorGUILayout.Space();
                }
                if (this.IsMobileTarget(targetGroup))
                {
                    if (((targetGroup != BuildTargetGroup.Tizen) && (targetGroup != BuildTargetGroup.iPhone)) && (targetGroup != BuildTargetGroup.tvOS))
                    {
                        EditorGUILayout.PropertyField(this.m_Use32BitDisplayBuffer, EditorGUIUtility.TextContent("Use 32-bit Display Buffer*|If set Display Buffer will be created to hold 32-bit color values. Use it only if you see banding, as it has performance implications."), new GUILayoutOption[0]);
                    }
                    EditorGUILayout.PropertyField(this.m_DisableDepthAndStencilBuffers, EditorGUIUtility.TextContent("Disable Depth and Stencil*"), new GUILayoutOption[0]);
                }
                if (targetGroup == BuildTargetGroup.iPhone)
                {
                    EditorGUILayout.PropertyField(this.m_iosShowActivityIndicatorOnLoading, EditorGUIUtility.TextContent("Show Loading Indicator"), new GUILayoutOption[0]);
                }
                if (targetGroup == BuildTargetGroup.Android)
                {
                    EditorGUILayout.PropertyField(this.m_androidShowActivityIndicatorOnLoading, EditorGUIUtility.TextContent("Show Loading Indicator"), new GUILayoutOption[0]);
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

        private void ShowDisabledFakeEnumPopup(FakeEnum enumValue)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUILayout.PrefixLabel(this.m_ApiCompatibilityLevel.displayName);
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.EnumPopup(enumValue, new GUILayoutOption[0]);
            }
            GUILayout.EndHorizontal();
        }

        private void ShowNoSettings()
        {
            GUILayout.Label(EditorGUIUtility.TextContent("Not applicable for this platform."), EditorStyles.miniLabel, new GUILayoutOption[0]);
        }

        public void ShowSharedNote()
        {
            GUILayout.Label(EditorGUIUtility.TextContent("* Shared setting between multiple platforms."), EditorStyles.miniLabel, new GUILayoutOption[0]);
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

        private void SyncPlatformAPIsList(BuildTarget target)
        {
            if (this.m_GraphicsDeviceLists.ContainsKey(target))
            {
                GraphicsDeviceType[] graphicsAPIs = PlayerSettings.GetGraphicsAPIs(target);
                List<GraphicsDeviceType> list = (graphicsAPIs == null) ? new List<GraphicsDeviceType>() : graphicsAPIs.ToList<GraphicsDeviceType>();
                this.m_GraphicsDeviceLists[target].list = list;
            }
        }

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

        private static bool TargetSupportsSinglePassStereoRendering(BuildTargetGroup targetGroup) => 
            kSinglePassStereoRenderingTargetGroups.Contains<BuildTargetGroup>(targetGroup);

        private static bool TargetSupportsStereoInstancingRendering(BuildTargetGroup targetGroup) => 
            kStereoInstancingRenderingTargetGroups.Contains<BuildTargetGroup>(targetGroup);

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

        private enum FakeEnum
        {
            WiiUSubset,
            WSASubset
        }

        private static class Styles
        {
            public static readonly GUIStyle categoryBox = new GUIStyle(EditorStyles.helpBox);
            public static readonly GUIContent colorSpaceAndroidWarning = EditorGUIUtility.TextContent("On Android, linear colorspace requires OpenGL ES 3.0 only, uncheck 'Automatic Graphics API' to remove OpenGL ES 2 API and 'Minimum API Level' must be at least Android 4.3");
            public static readonly GUIContent colorSpaceIOSWarning = EditorGUIUtility.TextContent("Linear colorspace requires Metal API only. Uncheck 'Automatic Graphics API' and remove OpenGL ES 2 API. Additionally, 'minimum iOS version' set to 8.0 at least");
            public static readonly GUIContent colorSpaceTVOSWarning = EditorGUIUtility.TextContent("Linear colorspace requires Metal API only. Uncheck 'Automatic Graphics API' and remove OpenGL ES 2 API.");
            public static readonly GUIContent colorSpaceWebGLWarning = EditorGUIUtility.TextContent("On WebGL, linear colorspace is not supported");
            public static readonly GUIContent cursorHotspot = EditorGUIUtility.TextContent("Cursor Hotspot");
            public static readonly GUIContent defaultCursor = EditorGUIUtility.TextContent("Default Cursor");
            public static readonly GUIContent defaultIcon = EditorGUIUtility.TextContent("Default Icon");
            public static readonly GUIContent graphicsJobsMode = EditorGUIUtility.TextContent("Graphics Jobs Mode*");
            public static readonly GUIContent[] kStereoRenderingMethodsAll = new GUIContent[] { new GUIContent("Multi pass (Slow)"), new GUIContent("Single Pass (Fast)"), new GUIContent("Single Pass Instanced (Fastest)") };
            public static readonly GUIContent require31 = EditorGUIUtility.TextContent("Require ES3.1");
            public static readonly GUIContent requireAEP = EditorGUIUtility.TextContent("Require ES3.1+AEP");
            public static readonly GUIContent vertexChannelCompressionMask = EditorGUIUtility.TextContent("Vertex Compression*|Select which vertex channels should be compressed. Compression can save memory and bandwidth but precision will be lower.");

            static Styles()
            {
                categoryBox.padding.left = 14;
            }
        }
    }
}

