namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEditor.BuildReporting;
    using UnityEditor.Connect;
    using UnityEditor.Modules;
    using UnityEditor.SceneManagement;
    using UnityEditor.VersionControl;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.SceneManagement;

    internal class BuildPlayerWindow : EditorWindow
    {
        private int initialSelectedLVItem = -1;
        private const string kAssetsFolder = "Assets/";
        private const string kEditorBuildSettingsPath = "ProjectSettings/EditorBuildSettings.asset";
        private ListViewState lv = new ListViewState();
        private static BuildPlatforms s_BuildPlatforms;
        private Vector2 scrollPosition = new Vector2(0f, 0f);
        private bool[] selectedBeforeDrag;
        private bool[] selectedLVItems = new bool[0];
        private static Styles styles = null;

        public BuildPlayerWindow()
        {
            base.position = new Rect(50f, 50f, 540f, 530f);
            base.minSize = new Vector2(630f, 580f);
            base.titleContent = new GUIContent("Build Settings");
        }

        private void ActiveBuildTargetsGUI()
        {
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(255f) };
            GUILayout.BeginVertical(options);
            GUILayout.Label(styles.platformTitle, styles.title, new GUILayoutOption[0]);
            this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, "OL Box");
            for (int i = 0; i < 2; i++)
            {
                bool flag = i == 0;
                bool flag2 = false;
                foreach (BuildPlatform platform in s_BuildPlatforms.buildPlatforms)
                {
                    if (((IsBuildTargetGroupSupported(platform.targetGroup, platform.DefaultTarget) == flag) && (IsBuildTargetGroupSupported(platform.targetGroup, platform.DefaultTarget) || platform.forceShowTarget)) && BuildPipeline.IsBuildTargetCompatibleWithOS(platform.DefaultTarget))
                    {
                        this.ShowOption(platform, platform.title, !flag2 ? styles.oddRow : styles.evenRow);
                        flag2 = !flag2;
                    }
                }
                GUI.contentColor = Color.white;
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.Space(10f);
            BuildTarget target = CalculateSelectedBuildTarget();
            BuildTargetGroup selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUI.enabled = BuildPipeline.IsBuildTargetSupported(selectedBuildTargetGroup, target) && (EditorUserBuildSettings.activeBuildTargetGroup != selectedBuildTargetGroup);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(110f) };
            if (GUILayout.Button(styles.switchPlatform, optionArray2))
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(selectedBuildTargetGroup, target);
                GUIUtility.ExitGUI();
            }
            GUI.enabled = BuildPipeline.IsBuildTargetSupported(selectedBuildTargetGroup, target);
            GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(110f) };
            if (GUILayout.Button(new GUIContent("Player Settings..."), optionArray3))
            {
                Selection.activeObject = Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings");
                EditorWindow.GetWindow<InspectorWindow>();
            }
            GUILayout.EndHorizontal();
            GUI.enabled = true;
            GUILayout.EndVertical();
        }

        private void ActiveScenesGUI()
        {
            int num;
            int num2;
            int num3 = 0;
            int row = this.lv.row;
            bool shift = Event.current.shift;
            bool actionKey = EditorGUI.actionKey;
            Event current = Event.current;
            Rect position = GUILayoutUtility.GetRect(styles.scenesInBuild, styles.title);
            List<EditorBuildSettingsScene> list = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            this.lv.totalRows = list.Count;
            if (this.selectedLVItems.Length != list.Count)
            {
                Array.Resize<bool>(ref this.selectedLVItems, list.Count);
            }
            int[] numArray = new int[list.Count];
            for (num = 0; num < numArray.Length; num++)
            {
                EditorBuildSettingsScene scene = list[num];
                numArray[num] = num3;
                if (scene.enabled)
                {
                    num3++;
                }
            }
            IEnumerator enumerator = ListViewGUILayout.ListView(this.lv, ListViewOptions.wantsExternalFiles | ListViewOptions.wantsReordering, styles.box, new GUILayoutOption[0]).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    ListViewElement element = (ListViewElement) enumerator.Current;
                    EditorBuildSettingsScene scene2 = list[element.row];
                    bool flag3 = File.Exists(scene2.path);
                    using (new EditorGUI.DisabledScope(!flag3))
                    {
                        bool on = this.selectedLVItems[element.row];
                        if (on && (current.type == EventType.Repaint))
                        {
                            styles.selected.Draw(element.position, false, false, false, false);
                        }
                        if (!flag3)
                        {
                            scene2.enabled = false;
                        }
                        Rect rect2 = new Rect(element.position.x + 4f, element.position.y, styles.toggleSize.x, styles.toggleSize.y);
                        EditorGUI.BeginChangeCheck();
                        scene2.enabled = GUI.Toggle(rect2, scene2.enabled, "");
                        if (EditorGUI.EndChangeCheck() && on)
                        {
                            for (int i = 0; i < list.Count; i++)
                            {
                                if (this.selectedLVItems[i])
                                {
                                    list[i].enabled = scene2.enabled;
                                }
                            }
                        }
                        GUILayout.Space(styles.toggleSize.x);
                        string path = scene2.path;
                        if (path.StartsWith("Assets/"))
                        {
                            path = path.Substring("Assets/".Length);
                        }
                        if (path.EndsWith(".unity", StringComparison.InvariantCultureIgnoreCase))
                        {
                            path = path.Substring(0, path.Length - ".unity".Length);
                        }
                        Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.TempContent(path), styles.levelString);
                        if (Event.current.type == EventType.Repaint)
                        {
                            styles.levelString.Draw(rect, EditorGUIUtility.TempContent(path), false, false, on, false);
                        }
                        GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MaxWidth(36f) };
                        GUILayout.Label(!scene2.enabled ? "" : numArray[element.row].ToString(), styles.levelStringCounter, options);
                    }
                    if ((ListViewGUILayout.HasMouseUp(element.position) && !shift) && !actionKey)
                    {
                        if (!shift && !actionKey)
                        {
                            ListViewGUILayout.MultiSelection(row, element.row, ref this.initialSelectedLVItem, ref this.selectedLVItems);
                        }
                    }
                    else if (ListViewGUILayout.HasMouseDown(element.position))
                    {
                        if ((!this.selectedLVItems[element.row] || shift) || actionKey)
                        {
                            ListViewGUILayout.MultiSelection(row, element.row, ref this.initialSelectedLVItem, ref this.selectedLVItems);
                        }
                        this.lv.row = element.row;
                        this.selectedBeforeDrag = new bool[this.selectedLVItems.Length];
                        this.selectedLVItems.CopyTo(this.selectedBeforeDrag, 0);
                        this.selectedBeforeDrag[this.lv.row] = true;
                    }
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
            GUI.Label(position, styles.scenesInBuild, styles.title);
            if (GUIUtility.keyboardControl == this.lv.ID)
            {
                if ((Event.current.type == EventType.ValidateCommand) && (Event.current.commandName == "SelectAll"))
                {
                    Event.current.Use();
                }
                else if ((Event.current.type == EventType.ExecuteCommand) && (Event.current.commandName == "SelectAll"))
                {
                    for (num = 0; num < this.selectedLVItems.Length; num++)
                    {
                        this.selectedLVItems[num] = true;
                    }
                    this.lv.selectionChanged = true;
                    Event.current.Use();
                    GUIUtility.ExitGUI();
                }
            }
            if (this.lv.selectionChanged)
            {
                ListViewGUILayout.MultiSelection(row, this.lv.row, ref this.initialSelectedLVItem, ref this.selectedLVItems);
            }
            if (this.lv.fileNames != null)
            {
                Array.Sort<string>(this.lv.fileNames);
                int num6 = 0;
                for (num = 0; num < this.lv.fileNames.Length; num++)
                {
                    if (this.lv.fileNames[num].EndsWith("unity", StringComparison.InvariantCultureIgnoreCase))
                    {
                        <ActiveScenesGUI>c__AnonStorey0 storey = new <ActiveScenesGUI>c__AnonStorey0 {
                            scenePath = FileUtil.GetProjectRelativePath(this.lv.fileNames[num])
                        };
                        if (storey.scenePath == string.Empty)
                        {
                            storey.scenePath = this.lv.fileNames[num];
                        }
                        if (!Enumerable.Any<EditorBuildSettingsScene>(list, new Func<EditorBuildSettingsScene, bool>(storey.<>m__0)))
                        {
                            EditorBuildSettingsScene item = new EditorBuildSettingsScene {
                                path = storey.scenePath,
                                enabled = true
                            };
                            list.Insert(this.lv.draggedTo + num6++, item);
                        }
                    }
                }
                if (num6 != 0)
                {
                    Array.Resize<bool>(ref this.selectedLVItems, list.Count);
                    for (num = 0; num < this.selectedLVItems.Length; num++)
                    {
                        this.selectedLVItems[num] = (num >= this.lv.draggedTo) && (num < (this.lv.draggedTo + num6));
                    }
                }
                this.lv.draggedTo = -1;
            }
            if (this.lv.draggedTo != -1)
            {
                List<EditorBuildSettingsScene> collection = new List<EditorBuildSettingsScene>();
                num2 = 0;
                num = 0;
                while (num < this.selectedLVItems.Length)
                {
                    if (this.selectedBeforeDrag[num])
                    {
                        collection.Add(list[num2]);
                        list.RemoveAt(num2);
                        num2--;
                        if (this.lv.draggedTo >= num)
                        {
                            this.lv.draggedTo--;
                        }
                    }
                    num++;
                    num2++;
                }
                this.lv.draggedTo = ((this.lv.draggedTo <= list.Count) && (this.lv.draggedTo >= 0)) ? this.lv.draggedTo : list.Count;
                list.InsertRange(this.lv.draggedTo, collection);
                for (num = 0; num < this.selectedLVItems.Length; num++)
                {
                    this.selectedLVItems[num] = (num >= this.lv.draggedTo) && (num < (this.lv.draggedTo + collection.Count));
                }
            }
            if (((current.type == EventType.KeyDown) && ((current.keyCode == KeyCode.Backspace) || (current.keyCode == KeyCode.Delete))) && (GUIUtility.keyboardControl == this.lv.ID))
            {
                num2 = 0;
                num = 0;
                while (num < this.selectedLVItems.Length)
                {
                    if (this.selectedLVItems[num])
                    {
                        list.RemoveAt(num2);
                        num2--;
                    }
                    this.selectedLVItems[num] = false;
                    num++;
                    num2++;
                }
                this.lv.row = 0;
                current.Use();
            }
            EditorBuildSettings.scenes = list.ToArray();
        }

        private void AddOpenScenes()
        {
            List<EditorBuildSettingsScene> list = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            bool flag = false;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                <AddOpenScenes>c__AnonStorey1 storey = new <AddOpenScenes>c__AnonStorey1 {
                    scene = SceneManager.GetSceneAt(i)
                };
                if (((storey.scene.path.Length != 0) || EditorSceneManager.SaveScene(storey.scene, "", false)) && !Enumerable.Any<EditorBuildSettingsScene>(list, new Func<EditorBuildSettingsScene, bool>(storey.<>m__0)))
                {
                    EditorBuildSettingsScene item = new EditorBuildSettingsScene {
                        path = storey.scene.path,
                        enabled = true
                    };
                    list.Add(item);
                    flag = true;
                }
            }
            if (flag)
            {
                EditorBuildSettings.scenes = list.ToArray();
                base.Repaint();
                GUIUtility.ExitGUI();
            }
        }

        private static void BuildPlayerAndRun()
        {
            if (!BuildPlayerWithDefaultSettings(false, BuildOptions.AutoRunPlayer))
            {
                ShowBuildPlayerWindow();
            }
        }

        private static void BuildPlayerAndSelect()
        {
            if (!BuildPlayerWithDefaultSettings(false, BuildOptions.ShowBuiltPlayer))
            {
                ShowBuildPlayerWindow();
            }
        }

        private static bool BuildPlayerWithDefaultSettings(bool askForBuildLocation, BuildOptions forceOptions) => 
            BuildPlayerWithDefaultSettings(askForBuildLocation, forceOptions, true);

        private static bool BuildPlayerWithDefaultSettings(bool askForBuildLocation, BuildOptions forceOptions, bool first)
        {
            bool updateExistingBuild = false;
            InitBuildPlatforms();
            if (!UnityConnect.instance.canBuildWithUPID && !EditorUtility.DisplayDialog("Missing Project ID", "Because you are not a member of this project this build will not access Unity services.\nDo you want to continue?", "Yes", "No"))
            {
                return false;
            }
            BuildTarget target = CalculateSelectedBuildTarget();
            BuildTargetGroup selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            if (!BuildPipeline.IsBuildTargetSupported(selectedBuildTargetGroup, target))
            {
                return false;
            }
            IBuildWindowExtension buildWindowExtension = ModuleManager.GetBuildWindowExtension(ModuleManager.GetTargetStringFrom(EditorUserBuildSettings.selectedBuildTargetGroup, target));
            if (((buildWindowExtension != null) && ((forceOptions & BuildOptions.AutoRunPlayer) != BuildOptions.CompressTextures)) && !buildWindowExtension.EnabledBuildAndRunButton())
            {
                return false;
            }
            if (Unsupported.IsBleedingEdgeBuild())
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("This version of Unity is a BleedingEdge build that has not seen any manual testing.");
                builder.AppendLine("You should consider this build unstable.");
                builder.AppendLine("We strongly recommend that you use a normal version of Unity instead.");
                if (EditorUtility.DisplayDialog("BleedingEdge Build", builder.ToString(), "Cancel", "OK"))
                {
                    return false;
                }
            }
            string location = "";
            bool flag3 = (EditorUserBuildSettings.installInBuildFolder && PostprocessBuildPlayer.SupportsInstallInBuildFolder(selectedBuildTargetGroup, target)) && (Unsupported.IsDeveloperBuild() || IsMetroPlayer(target));
            BuildOptions options = forceOptions;
            bool development = EditorUserBuildSettings.development;
            if (development)
            {
                options |= BuildOptions.Development;
            }
            if (EditorUserBuildSettings.allowDebugging && development)
            {
                options |= BuildOptions.AllowDebugging;
            }
            if (EditorUserBuildSettings.symlinkLibraries)
            {
                options |= BuildOptions.SymlinkLibraries;
            }
            if ((target == BuildTarget.Android) && EditorUserBuildSettings.exportAsGoogleAndroidProject)
            {
                options |= BuildOptions.AcceptExternalModificationsToPlayer;
            }
            if (EditorUserBuildSettings.enableHeadlessMode)
            {
                options |= BuildOptions.EnableHeadlessMode;
            }
            if (EditorUserBuildSettings.connectProfiler && (development || (target == BuildTarget.WSAPlayer)))
            {
                options |= BuildOptions.ConnectWithProfiler;
            }
            if (EditorUserBuildSettings.buildScriptsOnly)
            {
                options |= BuildOptions.BuildScriptsOnly;
            }
            if (EditorUserBuildSettings.forceOptimizeScriptCompilation)
            {
                options |= BuildOptions.ForceOptimizeScriptCompilation;
            }
            if (flag3)
            {
                options |= BuildOptions.InstallInBuildFolder;
            }
            if (!flag3)
            {
                if (askForBuildLocation && !PickBuildLocation(selectedBuildTargetGroup, target, options, out updateExistingBuild))
                {
                    return false;
                }
                location = EditorUserBuildSettings.GetBuildLocation(target);
                if (location.Length == 0)
                {
                    return false;
                }
                if (!askForBuildLocation)
                {
                    switch (InternalEditorUtility.BuildCanBeAppended(target, location))
                    {
                        case CanAppendBuild.Yes:
                            updateExistingBuild = true;
                            break;

                        case CanAppendBuild.No:
                            if (!PickBuildLocation(selectedBuildTargetGroup, target, options, out updateExistingBuild))
                            {
                                return false;
                            }
                            location = EditorUserBuildSettings.GetBuildLocation(target);
                            if ((location.Length == 0) || !Directory.Exists(FileUtil.DeleteLastPathNameComponent(location)))
                            {
                                return false;
                            }
                            break;
                    }
                }
            }
            if (updateExistingBuild)
            {
                options |= BuildOptions.AcceptExternalModificationsToPlayer;
            }
            ArrayList list = new ArrayList();
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            foreach (EditorBuildSettingsScene scene in scenes)
            {
                if (scene.enabled)
                {
                    list.Add(scene.path);
                }
            }
            string[] levels = list.ToArray(typeof(string)) as string[];
            bool delayToAfterScriptReload = false;
            if ((EditorUserBuildSettings.activeBuildTarget != target) || (EditorUserBuildSettings.activeBuildTargetGroup != selectedBuildTargetGroup))
            {
                if (!EditorUserBuildSettings.SwitchActiveBuildTarget(selectedBuildTargetGroup, target))
                {
                    object[] args = new object[] { BuildPipeline.GetBuildTargetGroupDisplayName(selectedBuildTargetGroup), s_BuildPlatforms.GetBuildTargetDisplayName(target) };
                    Debug.LogErrorFormat("Could not switch to build target '{0}', '{1}'.", args);
                    return false;
                }
                if (EditorApplication.isCompiling)
                {
                    delayToAfterScriptReload = true;
                }
            }
            BuildReport report = BuildPipeline.BuildPlayerInternalNoCheck(levels, location, null, selectedBuildTargetGroup, target, options, delayToAfterScriptReload);
            return ((report == null) || (report.totalErrors == 0));
        }

        private static BuildTarget CalculateSelectedBuildTarget()
        {
            BuildTargetGroup selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            if (selectedBuildTargetGroup == BuildTargetGroup.Standalone)
            {
                return EditorUserBuildSettings.selectedStandaloneTarget;
            }
            else if (selectedBuildTargetGroup == BuildTargetGroup.Facebook)
            {
                return EditorUserBuildSettings.selectedFacebookTarget;
            }
            if (s_BuildPlatforms == null)
            {
                throw new Exception("Build platforms are not initialized.");
            }
            BuildPlatform platform = s_BuildPlatforms.BuildPlatformFromTargetGroup(selectedBuildTargetGroup);
            return platform?.DefaultTarget;
        }

        private static bool FolderIsEmpty(string path) => 
            (!Directory.Exists(path) || ((Directory.GetDirectories(path).Length == 0) && (Directory.GetFiles(path).Length == 0)));

        public static string GetPlaybackEngineDownloadURL(string moduleName)
        {
            string unityVersionFull = InternalEditorUtility.GetUnityVersionFull();
            string str2 = "";
            string str3 = "";
            int length = unityVersionFull.LastIndexOf('_');
            if (length != -1)
            {
                str2 = unityVersionFull.Substring(length + 1);
                str3 = unityVersionFull.Substring(0, length);
            }
            if (moduleName == "XboxOne")
            {
                return "http://blogs.unity3d.com/2014/08/11/unity-for-xbox-one-is-here/";
            }
            Dictionary<string, string> dictionary = new Dictionary<string, string> {
                { 
                    "SamsungTV",
                    "Samsung-TV"
                },
                { 
                    "tvOS",
                    "AppleTV"
                },
                { 
                    "OSXStandalone",
                    "Mac"
                },
                { 
                    "WindowsStandalone",
                    "Windows"
                },
                { 
                    "LinuxStandalone",
                    "Linux"
                },
                { 
                    "Facebook",
                    "Facebook-Games"
                }
            };
            if (dictionary.ContainsKey(moduleName))
            {
                moduleName = dictionary[moduleName];
            }
            string str5 = "Unknown";
            string str6 = "Unknown";
            string str7 = "Unknown";
            if ((str3.IndexOf('a') != -1) || (str3.IndexOf('b') != -1))
            {
                str5 = "beta";
                str6 = "download";
            }
            else
            {
                str5 = "download";
                str6 = "download_unity";
            }
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                str7 = "TargetSupportInstaller";
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                str7 = "MacEditorTargetInstaller";
            }
            string[] textArray1 = new string[] { "http://", str5, ".unity3d.com/", str6, "/", str2, "/", str7, "/UnitySetup-", moduleName, "-Support-for-Editor-", str3 };
            string str8 = string.Concat(textArray1);
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                str8 = str8 + ".exe";
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                str8 = str8 + ".pkg";
            }
            return str8;
        }

        internal static List<BuildPlatform> GetValidPlatforms() => 
            GetValidPlatforms(false);

        internal static List<BuildPlatform> GetValidPlatforms(bool includeMetaPlatforms)
        {
            InitBuildPlatforms();
            List<BuildPlatform> list = new List<BuildPlatform>();
            foreach (BuildPlatform platform in s_BuildPlatforms.buildPlatforms)
            {
                if (((platform.targetGroup == BuildTargetGroup.Standalone) || BuildPipeline.IsBuildTargetSupported(platform.targetGroup, platform.DefaultTarget)) && ((platform.targetGroup != BuildTargetGroup.Facebook) || includeMetaPlatforms))
                {
                    list.Add(platform);
                }
            }
            return list;
        }

        private static void GUIBuildButtons(bool enableBuildButton, bool enableBuildAndRunButton, bool canInstallInBuildFolder, BuildPlatform platform)
        {
            GUIBuildButtons(null, enableBuildButton, enableBuildAndRunButton, canInstallInBuildFolder, platform);
        }

        private static void GUIBuildButtons(IBuildWindowExtension buildWindowExtension, bool enableBuildButton, bool enableBuildAndRunButton, bool canInstallInBuildFolder, BuildPlatform platform)
        {
            GUILayout.FlexibleSpace();
            if (canInstallInBuildFolder)
            {
                GUILayoutOption[] optionArray1 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                EditorUserBuildSettings.installInBuildFolder = GUILayout.Toggle(EditorUserBuildSettings.installInBuildFolder, "Install in Builds folder\n(for debugging with source code)", optionArray1);
            }
            else
            {
                EditorUserBuildSettings.installInBuildFolder = false;
            }
            if ((buildWindowExtension != null) && Unsupported.IsDeveloperBuild())
            {
                buildWindowExtension.ShowInternalPlatformBuildOptions();
            }
            if ((!IsColorSpaceValid(platform) && enableBuildButton) && enableBuildAndRunButton)
            {
                enableBuildAndRunButton = false;
                enableBuildButton = false;
                EditorGUILayout.HelpBox(Styles.invalidColorSpaceMessage.text, MessageType.Warning);
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            if (EditorGUILayout.LinkLabel(styles.learnAboutUnityCloudBuild, new GUILayoutOption[0]))
            {
                Application.OpenURL($"{WebURLs.cloudBuildPage}/from/editor/buildsettings?upid={PlayerSettings.cloudProjectId}&pid={PlayerSettings.productGUID}&currentplatform={EditorUserBuildSettings.activeBuildTarget}&selectedplatform={CalculateSelectedBuildTarget()}&unityversion={Application.unityVersion}");
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(6f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUIContent build = styles.build;
            if ((platform.targetGroup == BuildTargetGroup.Android) && EditorUserBuildSettings.exportAsGoogleAndroidProject)
            {
                build = styles.export;
            }
            if ((platform.targetGroup == BuildTargetGroup.iPhone) && (Application.platform != RuntimePlatform.OSXEditor))
            {
                enableBuildAndRunButton = false;
            }
            GUI.enabled = enableBuildButton;
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(110f) };
            if (GUILayout.Button(build, options))
            {
                BuildPlayerWithDefaultSettings(true, BuildOptions.ShowBuiltPlayer);
                GUIUtility.ExitGUI();
            }
            GUI.enabled = enableBuildAndRunButton;
            GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(110f) };
            if (GUILayout.Button(styles.buildAndRun, optionArray3))
            {
                BuildPlayerWithDefaultSettings(true, BuildOptions.AutoRunPlayer);
                GUIUtility.ExitGUI();
            }
            GUILayout.EndHorizontal();
        }

        private static void InitBuildPlatforms()
        {
            if (s_BuildPlatforms == null)
            {
                s_BuildPlatforms = new BuildPlatforms();
                RepairSelectedBuildTargetGroup();
            }
        }

        private static bool IsAnyStandaloneModuleLoaded() => 
            ((ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneLinux)) || ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneOSXIntel))) || ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneWindows)));

        internal static bool IsBuildTargetGroupSupported(BuildTargetGroup targetGroup, BuildTarget target) => 
            ((targetGroup == BuildTargetGroup.Standalone) || BuildPipeline.IsBuildTargetSupported(targetGroup, target));

        private static bool IsColorSpaceValid(BuildPlatform platform)
        {
            if (PlayerSettings.colorSpace == ColorSpace.Linear)
            {
                bool flag = true;
                bool flag2 = true;
                if (platform.targetGroup == BuildTargetGroup.iPhone)
                {
                    GraphicsDeviceType[] graphicsAPIs = PlayerSettings.GetGraphicsAPIs(BuildTarget.iOS);
                    flag = !graphicsAPIs.Contains<GraphicsDeviceType>(GraphicsDeviceType.OpenGLES3) && !graphicsAPIs.Contains<GraphicsDeviceType>(GraphicsDeviceType.OpenGLES2);
                    Version version = new Version(8, 0);
                    Version version2 = new Version(6, 0);
                    Version version3 = !string.IsNullOrEmpty(PlayerSettings.iOS.targetOSVersionString) ? new Version(PlayerSettings.iOS.targetOSVersionString) : version2;
                    flag2 = version3 >= version;
                }
                else if (platform.targetGroup == BuildTargetGroup.tvOS)
                {
                    GraphicsDeviceType[] source = PlayerSettings.GetGraphicsAPIs(BuildTarget.tvOS);
                    flag = !source.Contains<GraphicsDeviceType>(GraphicsDeviceType.OpenGLES3) && !source.Contains<GraphicsDeviceType>(GraphicsDeviceType.OpenGLES2);
                }
                else if (platform.targetGroup == BuildTargetGroup.Android)
                {
                    GraphicsDeviceType[] typeArray3 = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
                    flag = (typeArray3.Contains<GraphicsDeviceType>(GraphicsDeviceType.Vulkan) || typeArray3.Contains<GraphicsDeviceType>(GraphicsDeviceType.OpenGLES3)) && !typeArray3.Contains<GraphicsDeviceType>(GraphicsDeviceType.OpenGLES2);
                    flag2 = PlayerSettings.Android.minSdkVersion >= AndroidSdkVersions.AndroidApiLevel18;
                }
                return (flag && flag2);
            }
            return true;
        }

        private static bool IsMetroPlayer(BuildTarget target) => 
            (target == BuildTarget.WSAPlayer);

        private bool IsModuleInstalled(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget)
        {
            bool flag = BuildPipeline.LicenseCheck(buildTarget);
            string targetStringFrom = ModuleManager.GetTargetStringFrom(buildTargetGroup, buildTarget);
            return (((flag && !string.IsNullOrEmpty(targetStringFrom)) && (ModuleManager.GetBuildPostProcessor(targetStringFrom) == null)) && ((EditorUserBuildSettings.selectedBuildTargetGroup != BuildTargetGroup.Standalone) || !IsAnyStandaloneModuleLoaded()));
        }

        private void OnGUI()
        {
            InitBuildPlatforms();
            if (styles == null)
            {
                styles = new Styles();
                styles.toggleSize = styles.toggle.CalcSize(new GUIContent("X"));
                this.lv.rowHeight = (int) styles.levelString.CalcHeight(new GUIContent("X"), 100f);
            }
            if (!UnityConnect.instance.canBuildWithUPID)
            {
                this.ShowAlert();
            }
            GUILayout.Space(5f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(10f);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            string message = "";
            bool disabled = !AssetDatabase.IsOpenForEdit("ProjectSettings/EditorBuildSettings.asset", out message);
            using (new EditorGUI.DisabledScope(disabled))
            {
                this.ActiveScenesGUI();
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                if (disabled)
                {
                    GUI.enabled = true;
                    if (Provider.enabled && GUILayout.Button("Check out", new GUILayoutOption[0]))
                    {
                        Asset assetByPath = Provider.GetAssetByPath("ProjectSettings/EditorBuildSettings.asset");
                        AssetList assets = new AssetList();
                        assets.Add(assetByPath);
                        Provider.Checkout(assets, CheckoutMode.Asset);
                    }
                    GUILayout.Label(message, new GUILayoutOption[0]);
                    GUI.enabled = false;
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Add Open Scenes", new GUILayoutOption[0]))
                {
                    this.AddOpenScenes();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(10f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(301f) };
            GUILayout.BeginHorizontal(options);
            this.ActiveBuildTargetsGUI();
            GUILayout.Space(10f);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            this.ShowBuildTargetSettings();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(10f);
            GUILayout.EndVertical();
            GUILayout.Space(10f);
            GUILayout.EndHorizontal();
        }

        private static bool PickBuildLocation(BuildTargetGroup targetGroup, BuildTarget target, BuildOptions options, out bool updateExistingBuild)
        {
            updateExistingBuild = false;
            string buildLocation = EditorUserBuildSettings.GetBuildLocation(target);
            if ((target == BuildTarget.Android) && EditorUserBuildSettings.exportAsGoogleAndroidProject)
            {
                string str2 = "Export Google Android Project";
                string location = EditorUtility.SaveFolderPanel(str2, buildLocation, "");
                EditorUserBuildSettings.SetBuildLocation(target, location);
                return true;
            }
            string extension = PostprocessBuildPlayer.GetExtensionForBuildTarget(targetGroup, target, options);
            string directory = FileUtil.DeleteLastPathNameComponent(buildLocation);
            string lastPathNameComponent = FileUtil.GetLastPathNameComponent(buildLocation);
            string title = "Build " + s_BuildPlatforms.GetBuildTargetDisplayName(target);
            string path = EditorUtility.SaveBuildPanel(target, title, directory, lastPathNameComponent, extension, out updateExistingBuild);
            if (path == string.Empty)
            {
                return false;
            }
            if ((extension != string.Empty) && (FileUtil.GetPathExtension(path).ToLower() != extension))
            {
                path = path + '.' + extension;
            }
            if (FileUtil.GetLastPathNameComponent(path) == string.Empty)
            {
                return false;
            }
            string str10 = (extension == string.Empty) ? path : FileUtil.DeleteLastPathNameComponent(path);
            if (!Directory.Exists(str10))
            {
                Directory.CreateDirectory(str10);
            }
            if (((target == BuildTarget.iOS) && (Application.platform != RuntimePlatform.OSXEditor)) && (!FolderIsEmpty(path) && !UserWantsToDeleteFiles(path)))
            {
                return false;
            }
            EditorUserBuildSettings.SetBuildLocation(target, path);
            return true;
        }

        private static void RepairSelectedBuildTargetGroup()
        {
            BuildTargetGroup selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            if (((selectedBuildTargetGroup == BuildTargetGroup.Unknown) || (s_BuildPlatforms == null)) || (s_BuildPlatforms.BuildPlatformIndexFromTargetGroup(selectedBuildTargetGroup) < 0))
            {
                EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.Standalone;
            }
        }

        private void ShowAlert()
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(10f);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            EditorGUILayout.HelpBox("Because you are not a member of this project this build will not access Unity services.", MessageType.Warning);
            GUILayout.EndVertical();
            GUILayout.Space(5f);
            GUILayout.EndHorizontal();
        }

        private static void ShowBuildPlayerWindow()
        {
            EditorUserBuildSettings.selectedBuildTargetGroup = EditorUserBuildSettings.activeBuildTargetGroup;
            EditorWindow.GetWindow<BuildPlayerWindow>(true, "Build Settings");
        }

        private void ShowBuildTargetSettings()
        {
            EditorGUIUtility.labelWidth = Mathf.Min((float) 180f, (float) ((base.position.width - 265f) * 0.47f));
            BuildTarget target = CalculateSelectedBuildTarget();
            BuildTargetGroup selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            BuildPlatform platform = s_BuildPlatforms.BuildPlatformFromTargetGroup(selectedBuildTargetGroup);
            bool flag = BuildPipeline.LicenseCheck(target);
            GUILayout.Space(18f);
            Rect rect = GUILayoutUtility.GetRect((float) 50f, (float) 36f);
            rect.x++;
            GUI.Label(new Rect(rect.x + 3f, rect.y + 3f, 32f, 32f), platform.title.image, GUIStyle.none);
            GUI.Toggle(rect, false, platform.title.text, styles.platformSelector);
            GUILayout.Space(10f);
            if (((platform.targetGroup == BuildTargetGroup.WebGL) && !BuildPipeline.IsBuildTargetSupported(platform.targetGroup, target)) && (IntPtr.Size == 4))
            {
                GUILayout.Label("Building for WebGL requires a 64-bit Unity editor.", new GUILayoutOption[0]);
                GUIBuildButtons(false, false, false, platform);
            }
            else
            {
                string targetStringFrom = ModuleManager.GetTargetStringFrom(selectedBuildTargetGroup, target);
                if (this.IsModuleInstalled(selectedBuildTargetGroup, target))
                {
                    GUILayout.Label("No " + s_BuildPlatforms.GetModuleDisplayName(selectedBuildTargetGroup, target) + " module loaded.", new GUILayoutOption[0]);
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                    if (GUILayout.Button("Open Download Page", EditorStyles.miniButton, options))
                    {
                        Help.BrowseURL(GetPlaybackEngineDownloadURL(targetStringFrom));
                    }
                    GUIBuildButtons(false, false, false, platform);
                }
                else
                {
                    if (Application.HasProLicense() && !InternalEditorUtility.HasAdvancedLicenseOnBuildTarget(target))
                    {
                        string text = string.Format("{0} is not included in your Unity Pro license. Your {0} build will include a Unity Personal Edition splash screen.\n\nYou must be eligible to use Unity Personal Edition to use this build option. Please refer to our EULA for further information.", s_BuildPlatforms.GetBuildTargetDisplayName(target));
                        GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
                        GUILayout.Label(text, EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
                        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                        if (GUILayout.Button("EULA", EditorStyles.miniButton, new GUILayoutOption[0]))
                        {
                            Application.OpenURL("http://unity3d.com/legal/eula");
                        }
                        if (GUILayout.Button($"Add {s_BuildPlatforms.GetBuildTargetDisplayName(target)} to your Unity Pro license", EditorStyles.miniButton, new GUILayoutOption[0]))
                        {
                            Application.OpenURL("http://unity3d.com/get-unity");
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                    }
                    GUIContent downloadErrorForTarget = styles.GetDownloadErrorForTarget(target);
                    if (downloadErrorForTarget != null)
                    {
                        GUILayout.Label(downloadErrorForTarget, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
                        GUIBuildButtons(false, false, false, platform);
                    }
                    else if (!flag)
                    {
                        int num = s_BuildPlatforms.BuildPlatformIndexFromTargetGroup(platform.targetGroup);
                        GUILayout.Label(styles.notLicensedMessages[num, 0], EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
                        GUILayout.Space(5f);
                        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                        GUILayout.FlexibleSpace();
                        if ((styles.notLicensedMessages[num, 1].text.Length != 0) && GUILayout.Button(styles.notLicensedMessages[num, 1], new GUILayoutOption[0]))
                        {
                            Application.OpenURL(styles.notLicensedMessages[num, 2].text);
                        }
                        GUILayout.EndHorizontal();
                        GUIBuildButtons(false, false, false, platform);
                    }
                    else
                    {
                        IBuildWindowExtension buildWindowExtension = ModuleManager.GetBuildWindowExtension(ModuleManager.GetTargetStringFrom(platform.targetGroup, target));
                        if (buildWindowExtension != null)
                        {
                            buildWindowExtension.ShowPlatformBuildOptions();
                        }
                        GUI.changed = false;
                        BuildTargetGroup targetGroup = platform.targetGroup;
                        if (((targetGroup == BuildTargetGroup.iPhone) || (targetGroup == BuildTargetGroup.tvOS)) && (Application.platform == RuntimePlatform.OSXEditor))
                        {
                            EditorUserBuildSettings.symlinkLibraries = EditorGUILayout.Toggle(styles.symlinkiOSLibraries, EditorUserBuildSettings.symlinkLibraries, new GUILayoutOption[0]);
                        }
                        GUI.enabled = true;
                        bool enableBuildButton = (buildWindowExtension == null) || buildWindowExtension.EnabledBuildButton();
                        bool enableBuildAndRunButton = false;
                        bool flag4 = (buildWindowExtension == null) || buildWindowExtension.ShouldDrawScriptDebuggingCheckbox();
                        bool flag5 = (buildWindowExtension != null) && buildWindowExtension.ShouldDrawExplicitNullCheckbox();
                        bool flag6 = (buildWindowExtension != null) && buildWindowExtension.ShouldDrawExplicitDivideByZeroCheckbox();
                        bool flag7 = (buildWindowExtension == null) || buildWindowExtension.ShouldDrawDevelopmentPlayerCheckbox();
                        bool flag8 = ((target == BuildTarget.StandaloneLinux) || (target == BuildTarget.StandaloneLinux64)) || (target == BuildTarget.StandaloneLinuxUniversal);
                        IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(selectedBuildTargetGroup, target);
                        bool flag9 = (buildPostProcessor != null) && buildPostProcessor.SupportsScriptsOnlyBuild();
                        bool canInstallInBuildFolder = false;
                        if (BuildPipeline.IsBuildTargetSupported(selectedBuildTargetGroup, target))
                        {
                            bool flag11 = (buildWindowExtension == null) || buildWindowExtension.ShouldDrawProfilerCheckbox();
                            GUI.enabled = flag7;
                            if (flag7)
                            {
                                EditorUserBuildSettings.development = EditorGUILayout.Toggle(styles.debugBuild, EditorUserBuildSettings.development, new GUILayoutOption[0]);
                            }
                            bool development = EditorUserBuildSettings.development;
                            GUI.enabled = development;
                            if (flag11)
                            {
                                if (!GUI.enabled)
                                {
                                    if (!development)
                                    {
                                        styles.profileBuild.tooltip = "Profiling only enabled in Development Player";
                                    }
                                }
                                else
                                {
                                    styles.profileBuild.tooltip = "";
                                }
                                EditorUserBuildSettings.connectProfiler = EditorGUILayout.Toggle(styles.profileBuild, EditorUserBuildSettings.connectProfiler, new GUILayoutOption[0]);
                            }
                            GUI.enabled = development;
                            if (flag4)
                            {
                                EditorUserBuildSettings.allowDebugging = EditorGUILayout.Toggle(styles.allowDebugging, EditorUserBuildSettings.allowDebugging, new GUILayoutOption[0]);
                            }
                            bool flag13 = PlayerSettings.GetScriptingBackend(platform.targetGroup) == ScriptingImplementation.IL2CPP;
                            if ((((buildWindowExtension != null) && development) && flag13) && buildWindowExtension.ShouldDrawForceOptimizeScriptsCheckbox())
                            {
                                EditorUserBuildSettings.forceOptimizeScriptCompilation = EditorGUILayout.Toggle(styles.forceOptimizeScriptCompilation, EditorUserBuildSettings.forceOptimizeScriptCompilation, new GUILayoutOption[0]);
                            }
                            if (flag5)
                            {
                                GUI.enabled = !development;
                                if (!GUI.enabled)
                                {
                                    EditorUserBuildSettings.explicitNullChecks = true;
                                }
                                EditorUserBuildSettings.explicitNullChecks = EditorGUILayout.Toggle(styles.explicitNullChecks, EditorUserBuildSettings.explicitNullChecks, new GUILayoutOption[0]);
                                GUI.enabled = development;
                            }
                            if (flag6)
                            {
                                GUI.enabled = !development;
                                if (!GUI.enabled)
                                {
                                    EditorUserBuildSettings.explicitDivideByZeroChecks = true;
                                }
                                EditorUserBuildSettings.explicitDivideByZeroChecks = EditorGUILayout.Toggle(styles.explicitDivideByZeroChecks, EditorUserBuildSettings.explicitDivideByZeroChecks, new GUILayoutOption[0]);
                                GUI.enabled = development;
                            }
                            if (flag9)
                            {
                                EditorUserBuildSettings.buildScriptsOnly = EditorGUILayout.Toggle(styles.buildScriptsOnly, EditorUserBuildSettings.buildScriptsOnly, new GUILayoutOption[0]);
                            }
                            GUI.enabled = !development;
                            if (flag8)
                            {
                                EditorUserBuildSettings.enableHeadlessMode = EditorGUILayout.Toggle(styles.enableHeadlessMode, EditorUserBuildSettings.enableHeadlessMode && !development, new GUILayoutOption[0]);
                            }
                            GUI.enabled = true;
                            GUILayout.FlexibleSpace();
                            canInstallInBuildFolder = Unsupported.IsDeveloperBuild() && PostprocessBuildPlayer.SupportsInstallInBuildFolder(selectedBuildTargetGroup, target);
                            if (enableBuildButton)
                            {
                                enableBuildAndRunButton = (buildWindowExtension == null) ? !EditorUserBuildSettings.installInBuildFolder : (buildWindowExtension.EnabledBuildAndRunButton() && !EditorUserBuildSettings.installInBuildFolder);
                            }
                        }
                        else
                        {
                            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
                            GUILayout.BeginHorizontal(optionArray2);
                            GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
                            GUILayout.BeginVertical(optionArray3);
                            int index = s_BuildPlatforms.BuildPlatformIndexFromTargetGroup(platform.targetGroup);
                            GUILayout.Label(styles.GetTargetNotInstalled(index, 0), new GUILayoutOption[0]);
                            if ((styles.GetTargetNotInstalled(index, 1) != null) && GUILayout.Button(styles.GetTargetNotInstalled(index, 1), new GUILayoutOption[0]))
                            {
                                Application.OpenURL(styles.GetTargetNotInstalled(index, 2).text);
                            }
                            GUILayout.EndVertical();
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                        }
                        GUIBuildButtons(buildWindowExtension, enableBuildButton, enableBuildAndRunButton, canInstallInBuildFolder, platform);
                    }
                }
            }
        }

        private void ShowOption(BuildPlatform bp, GUIContent title, GUIStyle background)
        {
            Rect position = GUILayoutUtility.GetRect((float) 50f, (float) 36f);
            position.x++;
            position.y++;
            bool flag = BuildPipeline.LicenseCheck(bp.DefaultTarget);
            GUI.contentColor = new Color(1f, 1f, 1f, !flag ? 0.7f : 1f);
            bool on = EditorUserBuildSettings.selectedBuildTargetGroup == bp.targetGroup;
            if (Event.current.type == EventType.Repaint)
            {
                background.Draw(position, GUIContent.none, false, false, on, false);
                GUI.Label(new Rect(position.x + 3f, position.y + 3f, 32f, 32f), title.image, GUIStyle.none);
                if (EditorUserBuildSettings.activeBuildTargetGroup == bp.targetGroup)
                {
                    GUI.Label(new Rect((position.xMax - styles.activePlatformIcon.width) - 8f, (position.y + 3f) + ((0x20 - styles.activePlatformIcon.height) / 2), (float) styles.activePlatformIcon.width, (float) styles.activePlatformIcon.height), styles.activePlatformIcon, GUIStyle.none);
                }
            }
            if (GUI.Toggle(position, on, title.text, styles.platformSelector) && (EditorUserBuildSettings.selectedBuildTargetGroup != bp.targetGroup))
            {
                EditorUserBuildSettings.selectedBuildTargetGroup = bp.targetGroup;
                Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(InspectorWindow));
                for (int i = 0; i < objArray.Length; i++)
                {
                    InspectorWindow window = objArray[i] as InspectorWindow;
                    if (window != null)
                    {
                        window.Repaint();
                    }
                }
            }
        }

        private static bool UserWantsToDeleteFiles(string path)
        {
            string message = "WARNING: all files and folders located in target folder: '" + path + "' will be deleted by build process.";
            return EditorUtility.DisplayDialog("Deleting existing files", message, "OK", "Cancel");
        }

        public static BuildPlatforms AvailableBuildPlatforms =>
            s_BuildPlatforms;

        [CompilerGenerated]
        private sealed class <ActiveScenesGUI>c__AnonStorey0
        {
            internal string scenePath;

            internal bool <>m__0(EditorBuildSettingsScene s) => 
                (s.path == this.scenePath);
        }

        [CompilerGenerated]
        private sealed class <AddOpenScenes>c__AnonStorey1
        {
            internal Scene scene;

            internal bool <>m__0(EditorBuildSettingsScene s) => 
                (s.path == this.scene.path);
        }

        public class BuildPlatform
        {
            public bool forceShowTarget;
            public string name;
            public Texture2D smallIcon;
            public BuildTargetGroup targetGroup;
            public GUIContent title;
            public string tooltip;

            public BuildPlatform(string locTitle, string iconId, BuildTargetGroup targetGroup, bool forceShowTarget) : this(locTitle, "", iconId, targetGroup, forceShowTarget)
            {
            }

            public BuildPlatform(string locTitle, string tooltip, string iconId, BuildTargetGroup targetGroup, bool forceShowTarget)
            {
                this.targetGroup = targetGroup;
                this.name = (targetGroup == BuildTargetGroup.Unknown) ? "" : BuildPipeline.GetBuildTargetGroupName(this.DefaultTarget);
                this.title = EditorGUIUtility.TextContentWithIcon(locTitle, iconId);
                this.smallIcon = EditorGUIUtility.IconContent(iconId + ".Small").image as Texture2D;
                this.tooltip = tooltip;
                this.forceShowTarget = forceShowTarget;
            }

            public BuildTarget DefaultTarget
            {
                get
                {
                    switch (this.targetGroup)
                    {
                        case BuildTargetGroup.WebGL:
                            return BuildTarget.WebGL;

                        case BuildTargetGroup.WSA:
                            return BuildTarget.WSAPlayer;

                        case BuildTargetGroup.Tizen:
                            return BuildTarget.Tizen;

                        case BuildTargetGroup.PSP2:
                            return BuildTarget.PSP2;

                        case BuildTargetGroup.PS4:
                            return BuildTarget.PS4;

                        case BuildTargetGroup.XboxOne:
                            return BuildTarget.XboxOne;

                        case BuildTargetGroup.SamsungTV:
                            return BuildTarget.SamsungTV;

                        case BuildTargetGroup.N3DS:
                            return BuildTarget.N3DS;

                        case BuildTargetGroup.WiiU:
                            return BuildTarget.WiiU;

                        case BuildTargetGroup.tvOS:
                            return BuildTarget.tvOS;

                        case BuildTargetGroup.Facebook:
                            return BuildTarget.StandaloneWindows64;

                        case BuildTargetGroup.Switch:
                            return BuildTarget.Switch;

                        case BuildTargetGroup.Standalone:
                            return BuildTarget.StandaloneWindows;

                        case BuildTargetGroup.iPhone:
                            return BuildTarget.iOS;

                        case BuildTargetGroup.Android:
                            return BuildTarget.Android;
                    }
                    return BuildTarget.iPhone;
                }
            }
        }

        public class BuildPlatforms
        {
            public BuildPlayerWindow.BuildPlatform[] buildPlatforms;

            internal BuildPlatforms()
            {
                List<BuildPlayerWindow.BuildPlatform> list = new List<BuildPlayerWindow.BuildPlatform> {
                    new BuildPlayerWindow.BuildPlatform("PC, Mac & Linux Standalone", "BuildSettings.Standalone", BuildTargetGroup.Standalone, true),
                    new BuildPlayerWindow.BuildPlatform("iOS", "BuildSettings.iPhone", BuildTargetGroup.iPhone, true),
                    new BuildPlayerWindow.BuildPlatform("tvOS", "BuildSettings.tvOS", BuildTargetGroup.tvOS, true),
                    new BuildPlayerWindow.BuildPlatform("Android", "BuildSettings.Android", BuildTargetGroup.Android, true),
                    new BuildPlayerWindow.BuildPlatform("Tizen", "BuildSettings.Tizen", BuildTargetGroup.Tizen, true),
                    new BuildPlayerWindow.BuildPlatform("Xbox One", "BuildSettings.XboxOne", BuildTargetGroup.XboxOne, true),
                    new BuildPlayerWindow.BuildPlatform("PS Vita", "BuildSettings.PSP2", BuildTargetGroup.PSP2, true),
                    new BuildPlayerWindow.BuildPlatform("PS4", "BuildSettings.PS4", BuildTargetGroup.PS4, true),
                    new BuildPlayerWindow.BuildPlatform("Wii U", "BuildSettings.WiiU", BuildTargetGroup.WiiU, false),
                    new BuildPlayerWindow.BuildPlatform("Windows Store", "BuildSettings.Metro", BuildTargetGroup.WSA, true),
                    new BuildPlayerWindow.BuildPlatform("WebGL", "BuildSettings.WebGL", BuildTargetGroup.WebGL, true),
                    new BuildPlayerWindow.BuildPlatform("Samsung TV", "BuildSettings.SamsungTV", BuildTargetGroup.SamsungTV, true),
                    new BuildPlayerWindow.BuildPlatform("Nintendo 3DS", "BuildSettings.N3DS", BuildTargetGroup.N3DS, false),
                    new BuildPlayerWindow.BuildPlatform("Facebook", "BuildSettings.Facebook", BuildTargetGroup.Facebook, true),
                    new BuildPlayerWindow.BuildPlatform("Nintendo Switch", "BuildSettings.Switch", BuildTargetGroup.Switch, false)
                };
                foreach (BuildPlayerWindow.BuildPlatform platform in list)
                {
                    platform.tooltip = BuildPipeline.GetBuildTargetGroupDisplayName(platform.targetGroup) + " settings";
                }
                this.buildPlatforms = list.ToArray();
            }

            public BuildPlayerWindow.BuildPlatform BuildPlatformFromTargetGroup(BuildTargetGroup group)
            {
                int index = this.BuildPlatformIndexFromTargetGroup(group);
                return ((index == -1) ? null : this.buildPlatforms[index]);
            }

            public int BuildPlatformIndexFromTargetGroup(BuildTargetGroup group)
            {
                for (int i = 0; i < this.buildPlatforms.Length; i++)
                {
                    if (group == this.buildPlatforms[i].targetGroup)
                    {
                        return i;
                    }
                }
                return -1;
            }

            public string GetBuildTargetDisplayName(BuildTarget target)
            {
                foreach (BuildPlayerWindow.BuildPlatform platform in this.buildPlatforms)
                {
                    if (platform.DefaultTarget == target)
                    {
                        return platform.title.text;
                    }
                }
                switch (target)
                {
                    case BuildTarget.StandaloneOSXUniversal:
                    case BuildTarget.StandaloneOSXIntel:
                    case BuildTarget.StandaloneOSXIntel64:
                        return "Mac OS X";

                    case BuildTarget.StandaloneWindows:
                        break;

                    case BuildTarget.StandaloneLinux64:
                    case BuildTarget.StandaloneLinuxUniversal:
                        goto Label_00A1;

                    default:
                        switch (target)
                        {
                            case BuildTarget.StandaloneLinux:
                                goto Label_00A1;

                            case ((BuildTarget) 0x12):
                                goto Label_00AC;

                            case BuildTarget.StandaloneWindows64:
                                break;

                            default:
                                goto Label_00AC;
                        }
                        break;
                }
                return "Windows";
            Label_00A1:
                return "Linux";
            Label_00AC:
                return "Unsupported Target";
            }

            public string GetModuleDisplayName(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget)
            {
                if (buildTargetGroup == BuildTargetGroup.Facebook)
                {
                    return BuildPipeline.GetBuildTargetGroupDisplayName(buildTargetGroup);
                }
                return this.GetBuildTargetDisplayName(buildTarget);
            }
        }

        public class SceneSorter : IComparer
        {
            int IComparer.Compare(object x, object y) => 
                new CaseInsensitiveComparer().Compare(y, x);
        }

        private class Styles
        {
            public Texture2D activePlatformIcon = (EditorGUIUtility.IconContent("BuildSettings.SelectedIcon").image as Texture2D);
            public GUIContent allowDebugging;
            public GUIStyle box = "OL Box";
            public GUIContent build = EditorGUIUtility.TextContent("Build");
            public GUIContent buildAndRun = EditorGUIUtility.TextContent("Build And Run");
            public GUIContent buildScriptsOnly;
            private GUIContent[,] buildTargetNotInstalled;
            public GUIContent debugBuild;
            public GUIContent enableHeadlessMode;
            public GUIStyle evenRow = "CN EntryBackEven";
            public GUIContent explicitDivideByZeroChecks;
            public GUIContent explicitNullChecks;
            public GUIContent export = EditorGUIUtility.TextContent("Export");
            public GUIContent forceOptimizeScriptCompilation;
            public static readonly GUIContent invalidColorSpaceMessage = EditorGUIUtility.TextContent("In order to build a player go to 'Player Settings...' to resolve the incompatibility between the Color Space and the current settings.");
            public const float kButtonWidth = 110f;
            private const string kDownloadURL = "http://unity3d.com/unity/download/";
            private const string kMailURL = "http://unity3d.com/company/sales?type=sales";
            private const string kShopURL = "https://store.unity3d.com/shop/";
            public GUIContent learnAboutUnityCloudBuild;
            public GUIStyle levelString = "PlayerSettingsLevel";
            public GUIStyle levelStringCounter = new GUIStyle("Label");
            public GUIContent noSessionDialogText = EditorGUIUtility.TextContent("In order to publish your build to UDN, you need to sign in via the AssetStore and tick the 'Stay signed in' checkbox.");
            public GUIContent[,] notLicensedMessages;
            public GUIStyle oddRow = "CN EntryBackOdd";
            public GUIStyle platformSelector = "PlayerSettingsPlatform";
            public GUIContent platformTitle = EditorGUIUtility.TextContent("Platform|Which platform to build for");
            public GUIContent profileBuild;
            public GUIContent scenesInBuild = EditorGUIUtility.TextContent("Scenes In Build|Which scenes to include in the build");
            public GUIStyle selected = "ServerUpdateChangesetOn";
            public GUIContent switchPlatform = EditorGUIUtility.TextContent("Switch Platform");
            public GUIContent symlinkiOSLibraries;
            public GUIStyle title = EditorStyles.boldLabel;
            public GUIStyle toggle = "Toggle";
            public Vector2 toggleSize;

            public Styles()
            {
                GUIContent[] contentArray1 = new GUIContent[,] { { EditorGUIUtility.TextContent("Your license does not cover Standalone Publishing."), new GUIContent(""), new GUIContent("https://store.unity3d.com/shop/") }, { EditorGUIUtility.TextContent("Your license does not cover iOS Publishing."), EditorGUIUtility.TextContent("Go to Our Online Store"), new GUIContent("https://store.unity3d.com/shop/") }, { EditorGUIUtility.TextContent("Your license does not cover Apple TV Publishing."), EditorGUIUtility.TextContent("Go to Our Online Store"), new GUIContent("https://store.unity3d.com/shop/") }, { EditorGUIUtility.TextContent("Your license does not cover Android Publishing."), EditorGUIUtility.TextContent("Go to Our Online Store"), new GUIContent("https://store.unity3d.com/shop/") }, { EditorGUIUtility.TextContent("Your license does not cover Tizen Publishing."), EditorGUIUtility.TextContent("Go to Our Online Store"), new GUIContent("https://store.unity3d.com/shop/") }, { EditorGUIUtility.TextContent("Your license does not cover Xbox One Publishing."), EditorGUIUtility.TextContent("Contact sales"), new GUIContent("http://unity3d.com/company/sales?type=sales") }, { EditorGUIUtility.TextContent("Your license does not cover PS Vita Publishing."), EditorGUIUtility.TextContent("Contact sales"), new GUIContent("http://unity3d.com/company/sales?type=sales") }, { EditorGUIUtility.TextContent("Your license does not cover PS4 Publishing."), EditorGUIUtility.TextContent("Contact sales"), new GUIContent("http://unity3d.com/company/sales?type=sales") }, { EditorGUIUtility.TextContent("Your license does not cover Wii U Publishing."), EditorGUIUtility.TextContent("Contact sales"), new GUIContent("http://unity3d.com/company/sales?type=sales") }, { EditorGUIUtility.TextContent("Your license does not cover Windows Store Publishing."), EditorGUIUtility.TextContent("Go to Our Online Store"), new GUIContent("https://store.unity3d.com/shop/") }, { EditorGUIUtility.TextContent("Your license does not cover Windows Phone 8 Publishing."), EditorGUIUtility.TextContent("Go to Our Online Store"), new GUIContent("https://store.unity3d.com/shop/") }, { EditorGUIUtility.TextContent("Your license does not cover SamsungTV Publishing"), EditorGUIUtility.TextContent("Go to Our Online Store"), new GUIContent("https://store.unity3d.com/shop/") }, { EditorGUIUtility.TextContent("Your license does not cover Nintendo 3DS Publishing"), EditorGUIUtility.TextContent("Contact sales"), new GUIContent("http://unity3d.com/company/sales?type=sales") }, { EditorGUIUtility.TextContent("Your license does not cover Facebook Publishing"), EditorGUIUtility.TextContent("Go to Our Online Store"), new GUIContent("https://store.unity3d.com/shop/") }, { EditorGUIUtility.TextContent("Your license does not cover Nintendo Switch Publishing"), EditorGUIUtility.TextContent("Contact sales"), new GUIContent("http://unity3d.com/company/sales?type=sales") } };
                this.notLicensedMessages = contentArray1;
                GUIContent[] contentArray2 = new GUIContent[15, 3];
                contentArray2[0, 0] = EditorGUIUtility.TextContent("Standalone Player is not supported in this build.\nDownload a build that supports it.");
                contentArray2[0, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[1, 0] = EditorGUIUtility.TextContent("iOS Player is not supported in this build.\nDownload a build that supports it.");
                contentArray2[1, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[2, 0] = EditorGUIUtility.TextContent("Apple TV Player is not supported in this build.\nDownload a build that supports it.");
                contentArray2[2, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[3, 0] = EditorGUIUtility.TextContent("Android Player is not supported in this build.\nDownload a build that supports it.");
                contentArray2[3, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[4, 0] = EditorGUIUtility.TextContent("Tizen is not supported in this build.\nDownload a build that supports it.");
                contentArray2[4, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[5, 0] = EditorGUIUtility.TextContent("Xbox One Player is not supported in this build.\nDownload a build that supports it.");
                contentArray2[5, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[6, 0] = EditorGUIUtility.TextContent("PS Vita Player is not supported in this build.\nDownload a build that supports it.");
                contentArray2[6, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[7, 0] = EditorGUIUtility.TextContent("PS4 Player is not supported in this build.\nDownload a build that supports it.");
                contentArray2[7, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[8, 0] = EditorGUIUtility.TextContent("Wii U Player is not supported in this build.\nDownload a build that supports it.");
                contentArray2[8, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[9, 0] = EditorGUIUtility.TextContent("Windows Store Player is not supported in\nthis build.\n\nDownload a build that supports it.");
                contentArray2[9, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[10, 0] = EditorGUIUtility.TextContent("Windows Phone 8 Player is not supported\nin this build.\n\nDownload a build that supports it.");
                contentArray2[10, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[11, 0] = EditorGUIUtility.TextContent("SamsungTV Player is not supported in this build.\nDownload a build that supports it.");
                contentArray2[11, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[12, 0] = EditorGUIUtility.TextContent("Nintendo 3DS is not supported in this build.\nDownload a build that supports it.");
                contentArray2[12, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[13, 0] = EditorGUIUtility.TextContent("Facebook is not supported in this build.\nDownload a build that supports it.");
                contentArray2[13, 2] = new GUIContent("http://unity3d.com/unity/download/");
                contentArray2[14, 0] = EditorGUIUtility.TextContent("Nintendo Switch is not supported in this build.\nDownload a build that supports it.");
                contentArray2[14, 2] = new GUIContent("http://unity3d.com/unity/download/");
                this.buildTargetNotInstalled = contentArray2;
                this.debugBuild = EditorGUIUtility.TextContent("Development Build");
                this.profileBuild = EditorGUIUtility.TextContent("Autoconnect Profiler");
                this.allowDebugging = EditorGUIUtility.TextContent("Script Debugging");
                this.symlinkiOSLibraries = EditorGUIUtility.TextContent("Symlink Unity libraries");
                this.explicitNullChecks = EditorGUIUtility.TextContent("Explicit Null Checks");
                this.explicitDivideByZeroChecks = EditorGUIUtility.TextContent("Divide By Zero Checks");
                this.enableHeadlessMode = EditorGUIUtility.TextContent("Headless Mode");
                this.buildScriptsOnly = EditorGUIUtility.TextContent("Scripts Only Build");
                this.forceOptimizeScriptCompilation = EditorGUIUtility.TextContent("Build Optimized Scripts|Compile IL2CPP using full compiler optimizations. Note this will obfuscate callstack output.");
                this.learnAboutUnityCloudBuild = EditorGUIUtility.TextContent("Learn about Unity Cloud Build");
                this.levelStringCounter.alignment = TextAnchor.MiddleRight;
                if (Unsupported.IsDeveloperBuild() && ((this.buildTargetNotInstalled.GetLength(0) != this.notLicensedMessages.GetLength(0)) || (this.buildTargetNotInstalled.GetLength(0) != BuildPlayerWindow.s_BuildPlatforms.buildPlatforms.Length)))
                {
                    object[] args = new object[] { this.buildTargetNotInstalled.GetLength(0), this.notLicensedMessages.GetLength(0), BuildPlayerWindow.s_BuildPlatforms.buildPlatforms.Length };
                    Debug.LogErrorFormat("Build platforms and messages are desynced in BuildPlayerWindow! ({0} vs. {1} vs. {2}) DON'T SHIP THIS!", args);
                }
            }

            public GUIContent GetDownloadErrorForTarget(BuildTarget target) => 
                null;

            public GUIContent GetTargetNotInstalled(int index, int item)
            {
                if (index >= this.buildTargetNotInstalled.GetLength(0))
                {
                    index = 0;
                }
                return this.buildTargetNotInstalled[index, item];
            }
        }
    }
}

