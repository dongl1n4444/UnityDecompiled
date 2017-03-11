namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.Hardware;
    using UnityEditor.VersionControl;
    using UnityEditor.Web;
    using UnityEditorInternal;
    using UnityEngine;

    [CustomEditor(typeof(EditorSettings))]
    internal class EditorSettingsInspector : Editor
    {
        private PopupElement[] behaviorPopupList = new PopupElement[] { new PopupElement("3D"), new PopupElement("2D") };
        private string[] logLevelPopupList = new string[] { "Verbose", "Info", "Notice", "Fatal" };
        private PopupElement[] remoteCompressionList = new PopupElement[] { new PopupElement("JPEG"), new PopupElement("PNG") };
        private DevDevice[] remoteDeviceList;
        private PopupElement[] remoteDevicePopupList;
        private PopupElement[] remoteJoystickSourceList = new PopupElement[] { new PopupElement("Remote"), new PopupElement("Local") };
        private PopupElement[] remoteResolutionList = new PopupElement[] { new PopupElement("Downsize"), new PopupElement("Normal") };
        private string[] semanticMergePopupList = new string[] { "Off", "Premerge", "Ask" };
        private PopupElement[] serializationPopupList = new PopupElement[] { new PopupElement("Mixed"), new PopupElement("Force Binary"), new PopupElement("Force Text") };
        private PopupElement[] spritePackerPaddingPowerPopupList = new PopupElement[] { new PopupElement("1"), new PopupElement("2"), new PopupElement("3") };
        private PopupElement[] spritePackerPopupList = new PopupElement[] { new PopupElement("Disabled"), new PopupElement("Enabled For Builds"), new PopupElement("Always Enabled") };
        private PopupElement[] vcDefaultPopupList = new PopupElement[] { new PopupElement(ExternalVersionControl.Disabled), new PopupElement(ExternalVersionControl.Generic) };
        private PopupElement[] vcPopupList = null;

        private void BuildRemoteDeviceList()
        {
            List<DevDevice> list = new List<DevDevice>();
            List<PopupElement> list2 = new List<PopupElement>();
            list.Add(DevDevice.none);
            list2.Add(new PopupElement("None"));
            list.Add(new DevDevice("Any Android Device", "Any Android Device", "Android", "Android", DevDeviceState.Connected, DevDeviceFeatures.RemoteConnection));
            list2.Add(new PopupElement("Any Android Device"));
            foreach (DevDevice device in DevDeviceList.GetDevices())
            {
                bool flag = (device.features & DevDeviceFeatures.RemoteConnection) != DevDeviceFeatures.None;
                if (device.isConnected && flag)
                {
                    list.Add(device);
                    list2.Add(new PopupElement(device.name));
                }
            }
            this.remoteDeviceList = list.ToArray();
            this.remoteDevicePopupList = list2.ToArray();
        }

        private void CreatePopupMenu(string title, PopupElement[] elements, int selectedIndex, GenericMenu.MenuFunction2 func)
        {
            this.CreatePopupMenu(title, elements[selectedIndex].content, elements, selectedIndex, func);
        }

        private void CreatePopupMenu(string title, GUIContent content, PopupElement[] elements, int selectedIndex, GenericMenu.MenuFunction2 func)
        {
            Rect position = EditorGUI.PrefixLabel(GUILayoutUtility.GetRect(content, EditorStyles.popup), 0, new GUIContent(title));
            if (EditorGUI.DropdownButton(position, content, FocusType.Passive, EditorStyles.popup))
            {
                this.DoPopup(position, elements, selectedIndex, func);
            }
        }

        private void CreatePopupMenuVersionControl(string title, PopupElement[] elements, string selectedValue, GenericMenu.MenuFunction2 func)
        {
            <CreatePopupMenuVersionControl>c__AnonStorey1 storey = new <CreatePopupMenuVersionControl>c__AnonStorey1 {
                selectedValue = selectedValue
            };
            int index = Array.FindIndex<PopupElement>(elements, new Predicate<PopupElement>(storey.<>m__0));
            GUIContent content = new GUIContent(elements[index].content);
            this.CreatePopupMenu(title, content, elements, index, func);
        }

        private void DoInternalSettings()
        {
            if (EditorPrefs.GetBool("InternalMode", false))
            {
                GUILayout.Space(10f);
                GUILayout.Label("Internal settings", EditorStyles.boldLabel, new GUILayoutOption[0]);
                string text = EditorSettings.Internal_UserGeneratedProjectSuffix;
                string str2 = EditorGUILayout.DelayedTextField("Assembly suffix", text, new GUILayoutOption[0]);
                if (str2 != text)
                {
                    EditorSettings.Internal_UserGeneratedProjectSuffix = str2;
                    EditorApplication.ExecuteMenuItem("Assets/Reimport All");
                }
            }
        }

        private void DoPopup(Rect popupRect, PopupElement[] elements, int selectedIndex, GenericMenu.MenuFunction2 func)
        {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < elements.Length; i++)
            {
                PopupElement element = elements[i];
                if (element.Enabled)
                {
                    menu.AddItem(element.content, i == selectedIndex, func, i);
                }
                else
                {
                    menu.AddDisabledItem(element.content);
                }
            }
            menu.DropDown(popupRect);
        }

        private void DoProjectGenerationSettings()
        {
            GUILayout.Space(10f);
            GUILayout.Label("C# Project Generation", EditorStyles.boldLabel, new GUILayoutOption[0]);
            string text = EditorSettings.Internal_ProjectGenerationUserExtensions;
            string str2 = EditorGUILayout.TextField("Additional extensions to include", text, new GUILayoutOption[0]);
            if (str2 != text)
            {
                EditorSettings.Internal_ProjectGenerationUserExtensions = str2;
            }
            text = EditorSettings.projectGenerationRootNamespace;
            str2 = EditorGUILayout.TextField("Root namespace", text, new GUILayoutOption[0]);
            if (str2 != text)
            {
                EditorSettings.projectGenerationRootNamespace = str2;
            }
        }

        private void DrawOverlayDescription(Asset.States state)
        {
            Rect atlasRectForState = Provider.GetAtlasRectForState((int) state);
            if (atlasRectForState.width != 0f)
            {
                Texture2D overlayAtlas = Provider.overlayAtlas;
                if (overlayAtlas != null)
                {
                    GUILayout.Label("    " + Asset.StateToString(state), EditorStyles.miniLabel, new GUILayoutOption[0]);
                    Rect lastRect = GUILayoutUtility.GetLastRect();
                    lastRect.width = 16f;
                    GUI.DrawTextureWithTexCoords(lastRect, overlayAtlas, atlasRectForState);
                }
            }
        }

        private void DrawOverlayDescriptions()
        {
            if (Provider.overlayAtlas != null)
            {
                GUILayout.Space(10f);
                GUILayout.Label("Overlay legends", EditorStyles.boldLabel, new GUILayoutOption[0]);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                this.DrawOverlayDescription(Asset.States.Local);
                this.DrawOverlayDescription(Asset.States.OutOfSync);
                this.DrawOverlayDescription(Asset.States.CheckedOutLocal);
                this.DrawOverlayDescription(Asset.States.CheckedOutRemote);
                this.DrawOverlayDescription(Asset.States.DeletedLocal);
                this.DrawOverlayDescription(Asset.States.DeletedRemote);
                GUILayout.EndVertical();
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                this.DrawOverlayDescription(Asset.States.AddedLocal);
                this.DrawOverlayDescription(Asset.States.AddedRemote);
                this.DrawOverlayDescription(Asset.States.Conflicted);
                this.DrawOverlayDescription(Asset.States.LockedLocal);
                this.DrawOverlayDescription(Asset.States.LockedRemote);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        }

        private static int GetIndexById(PopupElement[] elements, string id, int defaultIndex)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i].id == id)
                {
                    return i;
                }
            }
            return defaultIndex;
        }

        private static int GetIndexById(DevDevice[] elements, string id, int defaultIndex)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i].id == id)
                {
                    return i;
                }
            }
            return defaultIndex;
        }

        private void OnDeviceListChanged()
        {
            this.BuildRemoteDeviceList();
        }

        public void OnDisable()
        {
            DevDeviceList.Changed -= new DevDeviceList.OnChangedHandler(this.OnDeviceListChanged);
        }

        public void OnEnable()
        {
            Plugin[] availablePlugins = Plugin.availablePlugins;
            List<PopupElement> list = new List<PopupElement>(this.vcDefaultPopupList);
            foreach (Plugin plugin in availablePlugins)
            {
                list.Add(new PopupElement(plugin.name, true));
            }
            this.vcPopupList = list.ToArray();
            DevDeviceList.Changed += new DevDeviceList.OnChangedHandler(this.OnDeviceListChanged);
            this.BuildRemoteDeviceList();
        }

        public override void OnInspectorGUI()
        {
            bool enabled = GUI.enabled;
            this.ShowUnityRemoteGUI(enabled);
            GUILayout.Space(10f);
            bool flag2 = CollabAccess.Instance.IsServiceEnabled();
            using (new EditorGUI.DisabledScope(!flag2))
            {
                GUI.enabled = !flag2;
                GUILayout.Label("Version Control", EditorStyles.boldLabel, new GUILayoutOption[0]);
                GUI.enabled = enabled && !flag2;
                ExternalVersionControl externalVersionControl = EditorSettings.externalVersionControl;
                this.CreatePopupMenuVersionControl("Mode", this.vcPopupList, (string) externalVersionControl, new GenericMenu.MenuFunction2(this.SetVersionControlSystem));
            }
            if (flag2)
            {
                EditorGUILayout.HelpBox("Version Control not available when using Collaboration feature.", MessageType.Warning);
            }
            if (this.VersionControlSystemHasGUI())
            {
                <OnInspectorGUI>c__AnonStorey0 storey = new <OnInspectorGUI>c__AnonStorey0();
                GUI.enabled = true;
                bool flag3 = false;
                if ((EditorSettings.externalVersionControl != ExternalVersionControl.Generic) && (EditorSettings.externalVersionControl != ExternalVersionControl.Disabled))
                {
                    ConfigField[] activeConfigFields = Provider.GetActiveConfigFields();
                    flag3 = true;
                    foreach (ConfigField field in activeConfigFields)
                    {
                        string str;
                        string configValue = EditorUserSettings.GetConfigValue(field.name);
                        if (field.isPassword)
                        {
                            str = EditorGUILayout.PasswordField(field.label, configValue, new GUILayoutOption[0]);
                            if (str != configValue)
                            {
                                EditorUserSettings.SetPrivateConfigValue(field.name, str);
                            }
                        }
                        else
                        {
                            str = EditorGUILayout.TextField(field.label, configValue, new GUILayoutOption[0]);
                            if (str != configValue)
                            {
                                EditorUserSettings.SetConfigValue(field.name, str);
                            }
                        }
                        if (field.isRequired && string.IsNullOrEmpty(str))
                        {
                            flag3 = false;
                        }
                    }
                }
                storey.logLevel = EditorUserSettings.GetConfigValue("vcSharedLogLevel");
                int num2 = Array.FindIndex<string>(this.logLevelPopupList, new Predicate<string>(storey.<>m__0));
                if (num2 == -1)
                {
                    storey.logLevel = "info";
                }
                int index = EditorGUILayout.Popup("Log Level", Math.Abs(num2), this.logLevelPopupList, new GUILayoutOption[0]);
                if (index != num2)
                {
                    EditorUserSettings.SetConfigValue("vcSharedLogLevel", this.logLevelPopupList[index].ToLower());
                }
                GUI.enabled = enabled;
                string str3 = "Connected";
                if (Provider.onlineState == OnlineState.Updating)
                {
                    str3 = "Connecting...";
                }
                else if (Provider.onlineState == OnlineState.Offline)
                {
                    str3 = "Disconnected";
                }
                EditorGUILayout.LabelField("Status", str3, new GUILayoutOption[0]);
                if ((Provider.onlineState != OnlineState.Online) && !string.IsNullOrEmpty(Provider.offlineReason))
                {
                    GUI.enabled = false;
                    GUILayout.TextArea(Provider.offlineReason, new GUILayoutOption[0]);
                    GUI.enabled = enabled;
                }
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                GUI.enabled = flag3 && (Provider.onlineState != OnlineState.Updating);
                if (GUILayout.Button("Connect", EditorStyles.miniButton, new GUILayoutOption[0]))
                {
                    Provider.UpdateSettings();
                }
                GUILayout.EndHorizontal();
                EditorUserSettings.AutomaticAdd = EditorGUILayout.Toggle("Automatic add", EditorUserSettings.AutomaticAdd, new GUILayoutOption[0]);
                if (Provider.requiresNetwork)
                {
                    bool flag4 = EditorGUILayout.Toggle("Work Offline", EditorUserSettings.WorkOffline, new GUILayoutOption[0]);
                    if (flag4 != EditorUserSettings.WorkOffline)
                    {
                        if (flag4 && !EditorUtility.DisplayDialog("Confirm working offline", "Working offline and making changes to your assets means that you will have to manually integrate changes back into version control using your standard version control client before you stop working offline in Unity. Make sure you know what you are doing.", "Work offline", "Cancel"))
                        {
                            flag4 = false;
                        }
                        EditorUserSettings.WorkOffline = flag4;
                        EditorApplication.RequestRepaintAllViews();
                    }
                }
                if (Provider.hasCheckoutSupport)
                {
                    EditorUserSettings.showFailedCheckout = EditorGUILayout.Toggle("Show failed checkouts", EditorUserSettings.showFailedCheckout, new GUILayoutOption[0]);
                }
                GUI.enabled = enabled;
                EditorUserSettings.semanticMergeMode = (SemanticMergeMode) EditorGUILayout.Popup("Smart merge", (int) EditorUserSettings.semanticMergeMode, this.semanticMergePopupList, new GUILayoutOption[0]);
                this.DrawOverlayDescriptions();
            }
            GUILayout.Space(10f);
            int serializationMode = (int) EditorSettings.serializationMode;
            using (new EditorGUI.DisabledScope(!flag2))
            {
                GUI.enabled = !flag2;
                GUILayout.Label("Asset Serialization", EditorStyles.boldLabel, new GUILayoutOption[0]);
                GUI.enabled = enabled && !flag2;
                this.CreatePopupMenu("Mode", this.serializationPopupList, serializationMode, new GenericMenu.MenuFunction2(this.SetAssetSerializationMode));
            }
            if (flag2)
            {
                EditorGUILayout.HelpBox("Asset Serialization is forced to Text when using Collaboration feature.", MessageType.Warning);
            }
            GUILayout.Space(10f);
            GUI.enabled = true;
            GUILayout.Label("Default Behavior Mode", EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUI.enabled = enabled;
            serializationMode = Mathf.Clamp((int) EditorSettings.defaultBehaviorMode, 0, this.behaviorPopupList.Length - 1);
            this.CreatePopupMenu("Mode", this.behaviorPopupList, serializationMode, new GenericMenu.MenuFunction2(this.SetDefaultBehaviorMode));
            GUILayout.Space(10f);
            GUI.enabled = true;
            GUILayout.Label("Sprite Packer", EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUI.enabled = enabled;
            serializationMode = Mathf.Clamp((int) EditorSettings.spritePackerMode, 0, this.spritePackerPopupList.Length - 1);
            this.CreatePopupMenu("Mode", this.spritePackerPopupList, serializationMode, new GenericMenu.MenuFunction2(this.SetSpritePackerMode));
            serializationMode = Mathf.Clamp(EditorSettings.spritePackerPaddingPower - 1, 0, 2);
            this.CreatePopupMenu("Padding Power", this.spritePackerPaddingPowerPopupList, serializationMode, new GenericMenu.MenuFunction2(this.SetSpritePackerPaddingPower));
            this.DoProjectGenerationSettings();
            this.DoInternalSettings();
        }

        private void SetAssetSerializationMode(object data)
        {
            int num = (int) data;
            EditorSettings.serializationMode = (SerializationMode) num;
        }

        private void SetDefaultBehaviorMode(object data)
        {
            int num = (int) data;
            EditorSettings.defaultBehaviorMode = (EditorBehaviorMode) num;
        }

        private void SetSpritePackerMode(object data)
        {
            int num = (int) data;
            EditorSettings.spritePackerMode = (SpritePackerMode) num;
        }

        private void SetSpritePackerPaddingPower(object data)
        {
            int num = (int) data;
            EditorSettings.spritePackerPaddingPower = num + 1;
        }

        private void SetUnityRemoteCompression(object data)
        {
            EditorSettings.unityRemoteCompression = this.remoteCompressionList[(int) data].id;
        }

        private void SetUnityRemoteDevice(object data)
        {
            EditorSettings.unityRemoteDevice = this.remoteDeviceList[(int) data].id;
        }

        private void SetUnityRemoteJoystickSource(object data)
        {
            EditorSettings.unityRemoteJoystickSource = this.remoteJoystickSourceList[(int) data].id;
        }

        private void SetUnityRemoteResolution(object data)
        {
            EditorSettings.unityRemoteResolution = this.remoteResolutionList[(int) data].id;
        }

        private void SetVersionControlSystem(object data)
        {
            int index = (int) data;
            if ((index >= 0) || (index < this.vcPopupList.Length))
            {
                PopupElement element = this.vcPopupList[index];
                string externalVersionControl = EditorSettings.externalVersionControl;
                EditorSettings.externalVersionControl = element.id;
                Provider.UpdateSettings();
                AssetDatabase.Refresh();
                if ((externalVersionControl != element.id) && ((element.content.text == ExternalVersionControl.Disabled) || (element.content.text == ExternalVersionControl.Generic)))
                {
                    WindowPending.CloseAllWindows();
                }
            }
        }

        private void ShowUnityRemoteGUI(bool editorEnabled)
        {
            GUI.enabled = true;
            GUILayout.Label("Unity Remote", EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUI.enabled = editorEnabled;
            string unityRemoteDevice = EditorSettings.unityRemoteDevice;
            int index = GetIndexById(this.remoteDeviceList, unityRemoteDevice, 0);
            GUIContent content = new GUIContent(this.remoteDevicePopupList[index].content);
            Rect position = EditorGUI.PrefixLabel(GUILayoutUtility.GetRect(content, EditorStyles.popup), 0, new GUIContent("Device"));
            if (EditorGUI.DropdownButton(position, content, FocusType.Passive, EditorStyles.popup))
            {
                this.DoPopup(position, this.remoteDevicePopupList, index, new GenericMenu.MenuFunction2(this.SetUnityRemoteDevice));
            }
            int num2 = GetIndexById(this.remoteCompressionList, EditorSettings.unityRemoteCompression, 0);
            content = new GUIContent(this.remoteCompressionList[num2].content);
            position = EditorGUI.PrefixLabel(GUILayoutUtility.GetRect(content, EditorStyles.popup), 0, new GUIContent("Compression"));
            if (EditorGUI.DropdownButton(position, content, FocusType.Passive, EditorStyles.popup))
            {
                this.DoPopup(position, this.remoteCompressionList, num2, new GenericMenu.MenuFunction2(this.SetUnityRemoteCompression));
            }
            int num3 = GetIndexById(this.remoteResolutionList, EditorSettings.unityRemoteResolution, 0);
            content = new GUIContent(this.remoteResolutionList[num3].content);
            position = EditorGUI.PrefixLabel(GUILayoutUtility.GetRect(content, EditorStyles.popup), 0, new GUIContent("Resolution"));
            if (EditorGUI.DropdownButton(position, content, FocusType.Passive, EditorStyles.popup))
            {
                this.DoPopup(position, this.remoteResolutionList, num3, new GenericMenu.MenuFunction2(this.SetUnityRemoteResolution));
            }
            int num4 = GetIndexById(this.remoteJoystickSourceList, EditorSettings.unityRemoteJoystickSource, 0);
            content = new GUIContent(this.remoteJoystickSourceList[num4].content);
            position = EditorGUI.PrefixLabel(GUILayoutUtility.GetRect(content, EditorStyles.popup), 0, new GUIContent("Joystick Source"));
            if (EditorGUI.DropdownButton(position, content, FocusType.Passive, EditorStyles.popup))
            {
                this.DoPopup(position, this.remoteJoystickSourceList, num4, new GenericMenu.MenuFunction2(this.SetUnityRemoteJoystickSource));
            }
        }

        private bool VersionControlSystemHasGUI()
        {
            if (!CollabAccess.Instance.IsServiceEnabled())
            {
                ExternalVersionControl externalVersionControl = EditorSettings.externalVersionControl;
                return (((externalVersionControl != ExternalVersionControl.Disabled) && (externalVersionControl != ExternalVersionControl.AutoDetect)) && (externalVersionControl != ExternalVersionControl.Generic));
            }
            return false;
        }

        [CompilerGenerated]
        private sealed class <CreatePopupMenuVersionControl>c__AnonStorey1
        {
            internal string selectedValue;

            internal bool <>m__0(EditorSettingsInspector.PopupElement typeElem) => 
                (typeElem.id == this.selectedValue);
        }

        [CompilerGenerated]
        private sealed class <OnInspectorGUI>c__AnonStorey0
        {
            internal string logLevel;

            internal bool <>m__0(string item) => 
                (item.ToLower() == this.logLevel);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PopupElement
        {
            public readonly string id;
            public readonly bool requiresTeamLicense;
            public readonly GUIContent content;
            public PopupElement(string content) : this(content, false)
            {
            }

            public PopupElement(string content, bool requiresTeamLicense)
            {
                this.id = content;
                this.content = new GUIContent(content);
                this.requiresTeamLicense = requiresTeamLicense;
            }

            public bool Enabled =>
                (!this.requiresTeamLicense || InternalEditorUtility.HasTeamLicense());
        }
    }
}

