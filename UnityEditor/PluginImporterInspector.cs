namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.Modules;
    using UnityEditorInternal;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(PluginImporter))]
    internal class PluginImporterInspector : AssetImporterInspector
    {
        [CompilerGenerated]
        private static GetComptability <>f__am$cache0;
        [CompilerGenerated]
        private static GetComptability <>f__am$cache1;
        [CompilerGenerated]
        private static GetComptability <>f__am$cache2;
        [CompilerGenerated]
        private static GetComptability <>f__am$cache3;
        private Compatibility m_CompatibleWithAnyPlatform;
        private Compatibility m_CompatibleWithEditor;
        private Compatibility[] m_CompatibleWithPlatform = new Compatibility[GetPlatformGroupArraySize()];
        private DesktopPluginImporterExtension m_DesktopExtension = null;
        private EditorPluginImporterExtension m_EditorExtension = null;
        private bool m_HasModified;
        private Vector2 m_InformationScrollPosition = Vector2.zero;
        private Dictionary<string, string> m_PluginInformation;
        private static readonly BuildTarget[] m_StandaloneTargets = new BuildTarget[] { BuildTarget.StandaloneOSXIntel };

        internal override void Apply()
        {
            base.Apply();
            foreach (PluginImporter importer in this.importers)
            {
                if (this.m_CompatibleWithAnyPlatform > Compatibility.Mixed)
                {
                    importer.SetCompatibleWithAnyPlatform(this.m_CompatibleWithAnyPlatform == Compatibility.Compatible);
                }
                if (this.m_CompatibleWithEditor > Compatibility.Mixed)
                {
                    importer.SetCompatibleWithEditor(this.m_CompatibleWithEditor == Compatibility.Compatible);
                }
                foreach (BuildTarget target in GetValidBuildTargets())
                {
                    if (this.m_CompatibleWithPlatform[(int) target] > Compatibility.Mixed)
                    {
                        importer.SetCompatibleWithPlatform(target, this.m_CompatibleWithPlatform[(int) target] == Compatibility.Compatible);
                    }
                }
                if (this.m_CompatibleWithEditor > Compatibility.Mixed)
                {
                    importer.SetExcludeEditorFromAnyPlatform(this.m_CompatibleWithEditor == Compatibility.NotCompatible);
                }
                foreach (BuildTarget target2 in GetValidBuildTargets())
                {
                    if (this.m_CompatibleWithPlatform[(int) target2] > Compatibility.Mixed)
                    {
                        importer.SetExcludeFromAnyPlatform(target2, this.m_CompatibleWithPlatform[(int) target2] == Compatibility.NotCompatible);
                    }
                }
            }
            if (this.IsEditingPlatformSettingsSupported())
            {
                foreach (IPluginImporterExtension extension in this.additionalExtensions)
                {
                    extension.Apply(this);
                }
                foreach (BuildTarget target3 in GetValidBuildTargets())
                {
                    IPluginImporterExtension pluginImporterExtension = ModuleManager.GetPluginImporterExtension(target3);
                    if (pluginImporterExtension != null)
                    {
                        pluginImporterExtension.Apply(this);
                    }
                }
            }
        }

        internal override void Awake()
        {
            this.m_EditorExtension = new EditorPluginImporterExtension();
            this.m_DesktopExtension = new DesktopPluginImporterExtension();
            base.Awake();
        }

        private BuildPlayerWindow.BuildPlatform[] GetBuildPlayerValidPlatforms()
        {
            List<BuildPlayerWindow.BuildPlatform> validPlatforms = BuildPlayerWindow.GetValidPlatforms();
            List<BuildPlayerWindow.BuildPlatform> list2 = new List<BuildPlayerWindow.BuildPlatform>();
            if (this.m_CompatibleWithEditor > Compatibility.NotCompatible)
            {
                BuildPlayerWindow.BuildPlatform item = new BuildPlayerWindow.BuildPlatform("Editor settings", "Editor Settings", "BuildSettings.Editor", BuildTargetGroup.Unknown, true) {
                    name = BuildPipeline.GetEditorTargetName()
                };
                list2.Add(item);
            }
            foreach (BuildPlayerWindow.BuildPlatform platform2 in validPlatforms)
            {
                if (!IgnorePlatform(platform2.DefaultTarget))
                {
                    if (platform2.targetGroup == BuildTargetGroup.Standalone)
                    {
                        if (this.compatibleWithStandalone < Compatibility.Compatible)
                        {
                            continue;
                        }
                    }
                    else if ((this.m_CompatibleWithPlatform[(int) platform2.DefaultTarget] < Compatibility.Compatible) || (ModuleManager.GetPluginImporterExtension(platform2.targetGroup) == null))
                    {
                        continue;
                    }
                    list2.Add(platform2);
                }
            }
            return list2.ToArray();
        }

        internal Compatibility GetPlatformCompatibility(string platformName) => 
            this.m_CompatibleWithPlatform[(int) BuildPipeline.GetBuildTargetByName(platformName)];

        private static int GetPlatformGroupArraySize()
        {
            int num = 0;
            foreach (BuildTarget target in typeof(BuildTarget).EnumGetNonObsoleteValues())
            {
                if (num < (target + 1))
                {
                    num = ((int) target) + 1;
                }
            }
            return num;
        }

        private static List<BuildTarget> GetValidBuildTargets()
        {
            List<BuildTarget> list = new List<BuildTarget>();
            foreach (BuildTarget target in typeof(BuildTarget).EnumGetNonObsoleteValues())
            {
                if (((target > ~BuildTarget.iPhone) && !IgnorePlatform(target)) && ((!ModuleManager.IsPlatformSupported(target) || ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(target))) || IsStandaloneTarget(target)))
                {
                    list.Add(target);
                }
            }
            return list;
        }

        internal override bool HasModified()
        {
            bool flag = this.m_HasModified || base.HasModified();
            if (this.IsEditingPlatformSettingsSupported())
            {
                foreach (IPluginImporterExtension extension in this.additionalExtensions)
                {
                    flag |= extension.HasModified(this);
                }
                foreach (BuildTarget target in GetValidBuildTargets())
                {
                    IPluginImporterExtension pluginImporterExtension = ModuleManager.GetPluginImporterExtension(target);
                    if (pluginImporterExtension != null)
                    {
                        flag |= pluginImporterExtension.HasModified(this);
                    }
                }
            }
            return flag;
        }

        private static bool IgnorePlatform(BuildTarget platform) => 
            false;

        private bool IsEditingPlatformSettingsSupported() => 
            (base.targets.Length == 1);

        private static bool IsStandaloneTarget(BuildTarget buildTarget) => 
            m_StandaloneTargets.Contains<BuildTarget>(buildTarget);

        private void OnDisable()
        {
            base.OnDisable();
            if (this.IsEditingPlatformSettingsSupported())
            {
                foreach (IPluginImporterExtension extension in this.additionalExtensions)
                {
                    extension.OnDisable(this);
                }
                foreach (BuildTarget target in GetValidBuildTargets())
                {
                    IPluginImporterExtension pluginImporterExtension = ModuleManager.GetPluginImporterExtension(target);
                    if (pluginImporterExtension != null)
                    {
                        pluginImporterExtension.OnDisable(this);
                    }
                }
            }
        }

        private void OnEnable()
        {
            if (this.IsEditingPlatformSettingsSupported())
            {
                foreach (IPluginImporterExtension extension in this.additionalExtensions)
                {
                    extension.OnEnable(this);
                }
                foreach (BuildTarget target in GetValidBuildTargets())
                {
                    IPluginImporterExtension pluginImporterExtension = ModuleManager.GetPluginImporterExtension(target);
                    if (pluginImporterExtension != null)
                    {
                        pluginImporterExtension.OnEnable(this);
                        pluginImporterExtension.ResetValues(this);
                    }
                }
                this.m_PluginInformation = new Dictionary<string, string>();
                this.m_PluginInformation["Path"] = this.importer.assetPath;
                this.m_PluginInformation["Type"] = !this.importer.isNativePlugin ? "Managed" : "Native";
                if (!this.importer.isNativePlugin)
                {
                    string str;
                    switch (this.importer.dllType)
                    {
                        case DllType.UnknownManaged:
                            str = "Targets Unknown .NET";
                            break;

                        case DllType.ManagedNET35:
                            str = "Targets .NET 3.5";
                            break;

                        case DllType.ManagedNET40:
                            str = "Targets .NET 4.x";
                            break;

                        case DllType.WinMDNative:
                            str = "Native WinMD";
                            break;

                        case DllType.WinMDNET40:
                            str = "Managed WinMD";
                            break;

                        default:
                            throw new Exception("Unknown managed dll type: " + this.importer.dllType.ToString());
                    }
                    this.m_PluginInformation["Assembly Info"] = str;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            using (new EditorGUI.DisabledScope(false))
            {
                GUILayout.Label("Select platforms for plugin", EditorStyles.boldLabel, new GUILayoutOption[0]);
                EditorGUILayout.BeginVertical(GUI.skin.box, new GUILayoutOption[0]);
                this.ShowGeneralOptions();
                EditorGUILayout.EndVertical();
                GUILayout.Space(10f);
                if (this.IsEditingPlatformSettingsSupported())
                {
                    this.ShowPlatformSettings();
                }
            }
            base.ApplyRevertGUI();
            if (base.targets.Length <= 1)
            {
                GUILayout.Label("Information", EditorStyles.boldLabel, new GUILayoutOption[0]);
                this.m_InformationScrollPosition = EditorGUILayout.BeginVerticalScrollView(this.m_InformationScrollPosition, new GUILayoutOption[0]);
                foreach (KeyValuePair<string, string> pair in this.m_PluginInformation)
                {
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(85f) };
                    GUILayout.Label(pair.Key, options);
                    GUILayout.TextField(pair.Value, new GUILayoutOption[0]);
                    GUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
                GUILayout.FlexibleSpace();
                if (this.importer.isNativePlugin)
                {
                    EditorGUILayout.HelpBox("Once a native plugin is loaded from script, it's never unloaded. If you deselect a native plugin and it's already loaded, please restart Unity.", MessageType.Warning);
                }
                if ((this.importer.dllType == DllType.ManagedNET40) && (this.m_CompatibleWithEditor == Compatibility.Compatible))
                {
                    EditorGUILayout.HelpBox("Plugin targets .NET 4.x and is marked as compatible with Editor, Editor can only use assemblies targeting .NET 3.5 or lower, please unselect Editor as compatible platform.", MessageType.Error);
                }
            }
        }

        private void ResetCompatability(ref Compatibility value, GetComptability getComptability)
        {
            value = !getComptability(this.importer) ? Compatibility.NotCompatible : Compatibility.Compatible;
            foreach (PluginImporter importer in this.importers)
            {
                if (value != (!getComptability(importer) ? Compatibility.NotCompatible : Compatibility.Compatible))
                {
                    value = Compatibility.Mixed;
                    break;
                }
            }
        }

        internal override void ResetValues()
        {
            base.ResetValues();
            this.m_HasModified = false;
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = imp => imp.GetCompatibleWithAnyPlatform();
            }
            this.ResetCompatability(ref this.m_CompatibleWithAnyPlatform, <>f__am$cache0);
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = imp => imp.GetCompatibleWithEditor();
            }
            this.ResetCompatability(ref this.m_CompatibleWithEditor, <>f__am$cache1);
            if (this.m_CompatibleWithAnyPlatform < Compatibility.Compatible)
            {
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = imp => imp.GetCompatibleWithEditor("", "");
                }
                this.ResetCompatability(ref this.m_CompatibleWithEditor, <>f__am$cache2);
                using (List<BuildTarget>.Enumerator enumerator = GetValidBuildTargets().GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        <ResetValues>c__AnonStorey0 storey = new <ResetValues>c__AnonStorey0 {
                            platform = enumerator.Current
                        };
                        this.ResetCompatability(ref this.m_CompatibleWithPlatform[(int) storey.platform], new GetComptability(storey.<>m__0));
                    }
                }
            }
            else
            {
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = imp => !imp.GetExcludeEditorFromAnyPlatform();
                }
                this.ResetCompatability(ref this.m_CompatibleWithEditor, <>f__am$cache3);
                using (List<BuildTarget>.Enumerator enumerator2 = GetValidBuildTargets().GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        <ResetValues>c__AnonStorey1 storey2 = new <ResetValues>c__AnonStorey1 {
                            platform = enumerator2.Current
                        };
                        this.ResetCompatability(ref this.m_CompatibleWithPlatform[(int) storey2.platform], new GetComptability(storey2.<>m__0));
                    }
                }
            }
            if (this.IsEditingPlatformSettingsSupported())
            {
                foreach (IPluginImporterExtension extension in this.additionalExtensions)
                {
                    extension.ResetValues(this);
                }
                foreach (BuildTarget target in GetValidBuildTargets())
                {
                    IPluginImporterExtension pluginImporterExtension = ModuleManager.GetPluginImporterExtension(target);
                    if (pluginImporterExtension != null)
                    {
                        pluginImporterExtension.ResetValues(this);
                    }
                }
            }
        }

        internal void SetPlatformCompatibility(string platformName, bool compatible)
        {
            this.SetPlatformCompatibility(platformName, !compatible ? Compatibility.NotCompatible : Compatibility.Compatible);
        }

        internal void SetPlatformCompatibility(string platformName, Compatibility compatibility)
        {
            if (compatibility == Compatibility.Mixed)
            {
                throw new ArgumentException("compatibility value cannot be Mixed");
            }
            int buildTargetByName = (int) BuildPipeline.GetBuildTargetByName(platformName);
            if (this.m_CompatibleWithPlatform[buildTargetByName] != compatibility)
            {
                this.m_CompatibleWithPlatform[buildTargetByName] = compatibility;
                this.m_HasModified = true;
            }
        }

        private void ShowEditorSettings()
        {
            this.editorExtension.OnPlatformSettingsGUI(this);
        }

        private void ShowGeneralOptions()
        {
            EditorGUI.BeginChangeCheck();
            this.m_CompatibleWithAnyPlatform = this.ToggleWithMixedValue(this.m_CompatibleWithAnyPlatform, "Any Platform");
            if (this.m_CompatibleWithAnyPlatform == Compatibility.Compatible)
            {
                GUILayout.Label("Exclude Platforms", EditorStyles.boldLabel, new GUILayoutOption[0]);
                this.ShowPlatforms(new ValueSwitcher(this.SwitchToExclude));
            }
            else if (this.m_CompatibleWithAnyPlatform == Compatibility.NotCompatible)
            {
                GUILayout.Label("Include Platforms", EditorStyles.boldLabel, new GUILayoutOption[0]);
                this.ShowPlatforms(new ValueSwitcher(this.SwitchToInclude));
            }
            if (EditorGUI.EndChangeCheck())
            {
                this.m_HasModified = true;
            }
        }

        private void ShowPlatforms(ValueSwitcher switcher)
        {
            this.m_CompatibleWithEditor = switcher(this.ToggleWithMixedValue(switcher(this.m_CompatibleWithEditor), "Editor"));
            EditorGUI.BeginChangeCheck();
            Compatibility compatibility = this.ToggleWithMixedValue(switcher(this.compatibleWithStandalone), "Standalone");
            if (EditorGUI.EndChangeCheck())
            {
                this.compatibleWithStandalone = switcher(compatibility);
                if (this.compatibleWithStandalone != Compatibility.Mixed)
                {
                    this.desktopExtension.ValidateSingleCPUTargets(this);
                }
            }
            foreach (BuildTarget target in GetValidBuildTargets())
            {
                if (!IsStandaloneTarget(target))
                {
                    Compatibility introduced3 = switcher(this.m_CompatibleWithPlatform[(int) target]);
                    this.m_CompatibleWithPlatform[(int) target] = switcher(this.ToggleWithMixedValue(introduced3, target.ToString()));
                }
            }
        }

        private void ShowPlatformSettings()
        {
            BuildPlayerWindow.BuildPlatform[] buildPlayerValidPlatforms = this.GetBuildPlayerValidPlatforms();
            if (buildPlayerValidPlatforms.Length > 0)
            {
                GUILayout.Label("Platform settings", EditorStyles.boldLabel, new GUILayoutOption[0]);
                int index = EditorGUILayout.BeginPlatformGrouping(buildPlayerValidPlatforms, null);
                if (buildPlayerValidPlatforms[index].name == BuildPipeline.GetEditorTargetName())
                {
                    this.ShowEditorSettings();
                }
                else
                {
                    BuildTargetGroup targetGroup = buildPlayerValidPlatforms[index].targetGroup;
                    if (targetGroup == BuildTargetGroup.Standalone)
                    {
                        this.desktopExtension.OnPlatformSettingsGUI(this);
                    }
                    else
                    {
                        IPluginImporterExtension pluginImporterExtension = ModuleManager.GetPluginImporterExtension(targetGroup);
                        if (pluginImporterExtension != null)
                        {
                            pluginImporterExtension.OnPlatformSettingsGUI(this);
                        }
                    }
                }
                EditorGUILayout.EndPlatformGrouping();
            }
        }

        private Compatibility SwitchToExclude(Compatibility value)
        {
            switch ((value + 1))
            {
                case Compatibility.NotCompatible:
                    return Compatibility.Mixed;

                case Compatibility.Compatible:
                    return Compatibility.Compatible;

                case ((Compatibility) 2):
                    return Compatibility.NotCompatible;
            }
            throw new InvalidEnumArgumentException("Invalid value: " + value.ToString());
        }

        private Compatibility SwitchToInclude(Compatibility value) => 
            value;

        private Compatibility ToggleWithMixedValue(Compatibility value, string title)
        {
            EditorGUI.showMixedValue = value == Compatibility.Mixed;
            EditorGUI.BeginChangeCheck();
            bool flag = EditorGUILayout.Toggle(title, value == Compatibility.Compatible, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                return (!flag ? Compatibility.NotCompatible : Compatibility.Compatible);
            }
            EditorGUI.showMixedValue = false;
            return value;
        }

        internal IPluginImporterExtension[] additionalExtensions =>
            new IPluginImporterExtension[] { this.editorExtension, this.desktopExtension };

        private Compatibility compatibleWithStandalone
        {
            get
            {
                bool flag = false;
                foreach (BuildTarget target in m_StandaloneTargets)
                {
                    if (this.m_CompatibleWithPlatform[(int) target] == Compatibility.Mixed)
                    {
                        return Compatibility.Mixed;
                    }
                    flag |= this.m_CompatibleWithPlatform[(int) target] > Compatibility.NotCompatible;
                }
                return (!flag ? Compatibility.NotCompatible : Compatibility.Compatible);
            }
            set
            {
                foreach (BuildTarget target in m_StandaloneTargets)
                {
                    this.m_CompatibleWithPlatform[(int) target] = value;
                }
            }
        }

        internal DesktopPluginImporterExtension desktopExtension
        {
            get
            {
                if (this.m_DesktopExtension == null)
                {
                    this.m_DesktopExtension = new DesktopPluginImporterExtension();
                }
                return this.m_DesktopExtension;
            }
        }

        internal EditorPluginImporterExtension editorExtension
        {
            get
            {
                if (this.m_EditorExtension == null)
                {
                    this.m_EditorExtension = new EditorPluginImporterExtension();
                }
                return this.m_EditorExtension;
            }
        }

        internal PluginImporter importer =>
            (base.target as PluginImporter);

        internal PluginImporter[] importers =>
            base.targets.Cast<PluginImporter>().ToArray<PluginImporter>();

        internal override bool showImportedObject =>
            false;

        [CompilerGenerated]
        private sealed class <ResetValues>c__AnonStorey0
        {
            internal BuildTarget platform;

            internal bool <>m__0(PluginImporter imp) => 
                imp.GetCompatibleWithPlatform(this.platform);
        }

        [CompilerGenerated]
        private sealed class <ResetValues>c__AnonStorey1
        {
            internal BuildTarget platform;

            internal bool <>m__0(PluginImporter imp) => 
                !imp.GetExcludeFromAnyPlatform(this.platform);
        }

        internal enum Compatibility
        {
            Compatible = 1,
            Mixed = -1,
            NotCompatible = 0
        }

        private delegate bool GetComptability(PluginImporter imp);

        private delegate PluginImporterInspector.Compatibility ValueSwitcher(PluginImporterInspector.Compatibility value);
    }
}

