namespace UnityEditorInternal.VR
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    internal class PlayerSettingsEditorVR
    {
        [CompilerGenerated]
        private static Func<VRDeviceInfoEditor, string> <>f__am$cache0;
        [CompilerGenerated]
        private static ReorderableList.HeaderCallbackDelegate <>f__am$cache1;
        private Dictionary<BuildTargetGroup, VRDeviceInfoEditor[]> m_AllVRDevicesForBuildTarget = new Dictionary<BuildTargetGroup, VRDeviceInfoEditor[]>();
        private Dictionary<string, VRCustomOptions> m_CustomOptions = new Dictionary<string, VRCustomOptions>();
        private Dictionary<string, string> m_MapVRDeviceKeyToUIString = new Dictionary<string, string>();
        private Dictionary<string, string> m_MapVRUIStringToDeviceKey = new Dictionary<string, string>();
        private Dictionary<BuildTargetGroup, ReorderableList> m_VRDeviceActiveUI = new Dictionary<BuildTargetGroup, ReorderableList>();
        private SerializedProperty m_VREditorSettings;

        public PlayerSettingsEditorVR(SerializedProperty settingsEditor)
        {
            this.m_VREditorSettings = settingsEditor;
        }

        private void AddVRDeviceElement(BuildTargetGroup target, Rect rect, ReorderableList list)
        {
            <AddVRDeviceElement>c__AnonStorey0 storey = new <AddVRDeviceElement>c__AnonStorey0();
            VRDeviceInfoEditor[] editorArray = this.m_AllVRDevicesForBuildTarget[target];
            storey.enabledDevices = VREditor.GetVREnabledDevicesOnTargetGroup(target).ToList<string>();
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = d => d.deviceNameUI;
            }
            string[] options = Enumerable.Select<VRDeviceInfoEditor, string>(editorArray, <>f__am$cache0).ToArray<string>();
            bool[] enabled = Enumerable.Select<VRDeviceInfoEditor, bool>(editorArray, new Func<VRDeviceInfoEditor, bool>(storey.<>m__0)).ToArray<bool>();
            EditorUtility.DisplayCustomMenu(rect, options, enabled, null, new EditorUtility.SelectMenuItemFunction(this.AddVRDeviceMenuSelected), target);
        }

        private void AddVRDeviceMenuSelected(object userData, string[] options, int selected)
        {
            string str;
            BuildTargetGroup targetGroup = (BuildTargetGroup) userData;
            List<string> list = VREditor.GetVREnabledDevicesOnTargetGroup(targetGroup).ToList<string>();
            if (!this.m_MapVRUIStringToDeviceKey.TryGetValue(options[selected], out str))
            {
                str = options[selected];
            }
            list.Add(str);
            this.ApplyChangedVRDeviceList(targetGroup, list.ToArray());
        }

        private void ApplyChangedVRDeviceList(BuildTargetGroup target, string[] devices)
        {
            if (this.m_VRDeviceActiveUI.ContainsKey(target))
            {
                VREditor.SetVREnabledDevicesOnTargetGroup(target, devices);
                this.m_VRDeviceActiveUI[target].list = devices;
            }
        }

        internal void DevicesGUI(BuildTargetGroup targetGroup)
        {
            if (this.TargetGroupSupportsVirtualReality(targetGroup))
            {
                bool vREnabledOnTargetGroup = VREditor.GetVREnabledOnTargetGroup(targetGroup);
                EditorGUI.BeginChangeCheck();
                vREnabledOnTargetGroup = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Virtual Reality Supported"), vREnabledOnTargetGroup, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    VREditor.SetVREnabledOnTargetGroup(targetGroup, vREnabledOnTargetGroup);
                }
                if (vREnabledOnTargetGroup)
                {
                    this.VRDevicesGUIOneBuildTarget(targetGroup);
                }
            }
        }

        private void DrawVRDeviceElement(BuildTargetGroup target, Rect rect, int index, bool selected, bool focused)
        {
            string str2;
            VRCustomOptions options;
            string key = (string) this.m_VRDeviceActiveUI[target].list[index];
            if (!this.m_MapVRDeviceKeyToUIString.TryGetValue(key, out str2))
            {
                str2 = key + " (missing from build)";
            }
            if (this.m_CustomOptions.TryGetValue(key, out options) && !(options is VRCustomOptionsNone))
            {
                Rect position = new Rect(rect) {
                    width = EditorStyles.foldout.border.left,
                    height = EditorStyles.foldout.border.top
                };
                bool hierarchyMode = EditorGUIUtility.hierarchyMode;
                EditorGUIUtility.hierarchyMode = false;
                options.IsExpanded = EditorGUI.Foldout(position, options.IsExpanded, "", false, EditorStyles.foldout);
                EditorGUIUtility.hierarchyMode = hierarchyMode;
            }
            rect.xMin += EditorStyles.foldout.border.left;
            GUI.Label(rect, str2, EditorStyles.label);
            rect.y += EditorGUIUtility.singleLineHeight + 2f;
            if ((options != null) && options.IsExpanded)
            {
                options.Draw(rect);
            }
        }

        private float GetVRDeviceElementHeight(BuildTargetGroup target, int index)
        {
            VRCustomOptions options;
            ReorderableList list = this.m_VRDeviceActiveUI[target];
            string key = (string) list.list[index];
            float num = 0f;
            if (this.m_CustomOptions.TryGetValue(key, out options))
            {
                num = !options.IsExpanded ? 0f : (options.GetHeight() + 2f);
            }
            return (list.elementHeight + num);
        }

        private void RefreshVRDeviceList(BuildTargetGroup targetGroup)
        {
            VRDeviceInfoEditor[] allVRDeviceInfo = VREditor.GetAllVRDeviceInfo(targetGroup);
            this.m_AllVRDevicesForBuildTarget[targetGroup] = allVRDeviceInfo;
            for (int i = 0; i < allVRDeviceInfo.Length; i++)
            {
                VRCustomOptions options;
                VRDeviceInfoEditor editor = allVRDeviceInfo[i];
                this.m_MapVRDeviceKeyToUIString[editor.deviceNameKey] = editor.deviceNameUI;
                this.m_MapVRUIStringToDeviceKey[editor.deviceNameUI] = editor.deviceNameKey;
                if (!this.m_CustomOptions.TryGetValue(editor.deviceNameKey, out options))
                {
                    System.Type type = System.Type.GetType("UnityEditorInternal.VR.VRCustomOptions" + editor.deviceNameKey, false, true);
                    if (type != null)
                    {
                        options = (VRCustomOptions) Activator.CreateInstance(type);
                    }
                    else
                    {
                        options = new VRCustomOptionsNone();
                    }
                    options.Initialize(this.m_VREditorSettings);
                    this.m_CustomOptions.Add(editor.deviceNameKey, options);
                }
            }
        }

        private void RemoveVRDeviceElement(BuildTargetGroup target, ReorderableList list)
        {
            List<string> list2 = VREditor.GetVREnabledDevicesOnTargetGroup(target).ToList<string>();
            list2.RemoveAt(list.index);
            this.ApplyChangedVRDeviceList(target, list2.ToArray());
        }

        private void ReorderVRDeviceElement(BuildTargetGroup target, ReorderableList list)
        {
            string[] devices = list.list.Cast<string>().ToArray<string>();
            this.ApplyChangedVRDeviceList(target, devices);
        }

        private void SelectVRDeviceElement(BuildTargetGroup target, ReorderableList list)
        {
            VRCustomOptions options;
            string key = (string) this.m_VRDeviceActiveUI[target].list[list.index];
            if (this.m_CustomOptions.TryGetValue(key, out options))
            {
                options.IsExpanded = false;
            }
        }

        internal bool TargetGroupSupportsVirtualReality(BuildTargetGroup targetGroup)
        {
            if (!this.m_AllVRDevicesForBuildTarget.ContainsKey(targetGroup))
            {
                this.RefreshVRDeviceList(targetGroup);
            }
            VRDeviceInfoEditor[] editorArray = this.m_AllVRDevicesForBuildTarget[targetGroup];
            return (editorArray.Length > 0);
        }

        private void VRDevicesGUIOneBuildTarget(BuildTargetGroup targetGroup)
        {
            <VRDevicesGUIOneBuildTarget>c__AnonStorey2 storey = new <VRDevicesGUIOneBuildTarget>c__AnonStorey2 {
                targetGroup = targetGroup,
                $this = this
            };
            if (!this.m_VRDeviceActiveUI.ContainsKey(storey.targetGroup))
            {
                ReorderableList list = new ReorderableList(VREditor.GetVREnabledDevicesOnTargetGroup(storey.targetGroup), typeof(VRDeviceInfoEditor), true, true, true, true) {
                    onAddDropdownCallback = new ReorderableList.AddDropdownCallbackDelegate(storey.<>m__0),
                    onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(storey.<>m__1),
                    onReorderCallback = new ReorderableList.ReorderCallbackDelegate(storey.<>m__2),
                    drawElementCallback = new ReorderableList.ElementCallbackDelegate(storey.<>m__3)
                };
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = rect => GUI.Label(rect, "Virtual Reality SDKs", EditorStyles.label);
                }
                list.drawHeaderCallback = <>f__am$cache1;
                list.elementHeightCallback = new ReorderableList.ElementHeightCallbackDelegate(storey.<>m__4);
                list.onSelectCallback = new ReorderableList.SelectCallbackDelegate(storey.<>m__5);
                this.m_VRDeviceActiveUI.Add(storey.targetGroup, list);
            }
            this.m_VRDeviceActiveUI[storey.targetGroup].DoLayoutList();
            if (this.m_VRDeviceActiveUI[storey.targetGroup].list.Count == 0)
            {
                EditorGUILayout.HelpBox("Must add at least one Virtual Reality SDK.", MessageType.Warning);
            }
        }

        [CompilerGenerated]
        private sealed class <AddVRDeviceElement>c__AnonStorey0
        {
            internal List<string> enabledDevices;

            internal bool <>m__0(VRDeviceInfoEditor d)
            {
                <AddVRDeviceElement>c__AnonStorey1 storey = new <AddVRDeviceElement>c__AnonStorey1 {
                    <>f__ref$0 = this,
                    d = d
                };
                return !Enumerable.Any<string>(this.enabledDevices, new Func<string, bool>(storey.<>m__0));
            }

            private sealed class <AddVRDeviceElement>c__AnonStorey1
            {
                internal PlayerSettingsEditorVR.<AddVRDeviceElement>c__AnonStorey0 <>f__ref$0;
                internal VRDeviceInfoEditor d;

                internal bool <>m__0(string enabledDeviceName) => 
                    (this.d.deviceNameKey == enabledDeviceName);
            }
        }

        [CompilerGenerated]
        private sealed class <VRDevicesGUIOneBuildTarget>c__AnonStorey2
        {
            internal PlayerSettingsEditorVR $this;
            internal BuildTargetGroup targetGroup;

            internal void <>m__0(Rect rect, ReorderableList list)
            {
                this.$this.AddVRDeviceElement(this.targetGroup, rect, list);
            }

            internal void <>m__1(ReorderableList list)
            {
                this.$this.RemoveVRDeviceElement(this.targetGroup, list);
            }

            internal void <>m__2(ReorderableList list)
            {
                this.$this.ReorderVRDeviceElement(this.targetGroup, list);
            }

            internal void <>m__3(Rect rect, int index, bool isActive, bool isFocused)
            {
                this.$this.DrawVRDeviceElement(this.targetGroup, rect, index, isActive, isFocused);
            }

            internal float <>m__4(int index) => 
                this.$this.GetVRDeviceElementHeight(this.targetGroup, index);

            internal void <>m__5(ReorderableList list)
            {
                this.$this.SelectVRDeviceElement(this.targetGroup, list);
            }
        }
    }
}

