namespace UnityEditor.iOS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEditorInternal;
    using UnityEngine;

    internal class iOSSettingsEditorExtension : DefaultPlayerSettingsEditorExtension
    {
        private LaunchScreenProperties iPadLaunchScreenProperties = new LaunchScreenProperties();
        private LaunchScreenProperties iPhoneLaunchScreenProperties = new LaunchScreenProperties();
        private static readonly iOSLaunchScreenType[] kLaunchScreenTypeOrder = new iOSLaunchScreenType[] { iOSLaunchScreenType.Default };
        private static readonly GUIContent[] kLaunchScreenUIDescriptions = new GUIContent[] { EditorGUIUtility.TextContent("Default"), EditorGUIUtility.TextContent("None"), EditorGUIUtility.TextContent("Image and background (relative size)"), EditorGUIUtility.TextContent("Image and background (constant size)"), EditorGUIUtility.TextContent("Custom XIB") };
        private SerializedProperty m_AppInBackgroundBehavior;
        private SerializedProperty m_AppleDeveloperTeamID;
        private SerializedProperty m_AppleEnableAutomaticSigning;
        private GUIContent m_AutomaticSigningGUIContent = EditorGUIUtility.TextContent("Automatically Sign|Check this to allow Xcode to Automatically sign your build.");
        private GUIContent m_iOSManualSigningGUIContent = EditorGUIUtility.TextContent("iOS Provisioning Profile");
        private SerializedProperty m_iOSManualSigningProvisioningProfileID;
        private ReorderableList m_RequirementList;
        private bool m_ResourceVariantsVisible = false;
        private GUIContent m_TeamIDGUIContent = EditorGUIUtility.TextContent("Automatic Signing Team ID|Developers can retrieve their Team ID by visiting the Apple Developer site under Account > Membership.");
        private SerializedProperty[] splashScreenProperties;

        public override bool CanShowUnitySplashScreen() => 
            true;

        public override void ConfigurationSectionGUI()
        {
            EditorGUILayout.PropertyField(this.m_AppInBackgroundBehavior, EditorGUIUtility.TextContent("Behavior in Background"), new GUILayoutOption[0]);
            if (this.m_AppInBackgroundBehavior.intValue == -1)
            {
                IOSBackgroundModesSection();
            }
            if (PlayerSettings.iOS.useOnDemandResources)
            {
                this.ShowDeviceRequirementsForVariantNamesGui();
            }
            SettingsUI.ShowArchitectureButton(BuildTargetGroup.iPhone);
        }

        private void CreateRequirementsForMissingTags()
        {
            List<string> allVariantNames = GetAllVariantNames();
            string[] assetBundleVariantsWithDeviceRequirements = PlayerSettings.iOS.GetAssetBundleVariantsWithDeviceRequirements();
            List<string> list2 = new List<string>();
            foreach (string str in allVariantNames)
            {
                if (!assetBundleVariantsWithDeviceRequirements.Contains<string>(str))
                {
                    list2.Add(str);
                }
            }
            if (list2.Count > 0)
            {
                foreach (string str2 in list2)
                {
                    PlayerSettings.iOS.AddDeviceRequirementsForAssetBundleVariant(str2).Add(new iOSDeviceRequirement());
                }
                this.m_RequirementList.list = GetRequirementItems();
            }
        }

        private void DrawLaunchScreenGUI(LaunchScreenProperties properties, UnityEditor.iOS.DeviceType device)
        {
            PlayerSettingsEditor.BuildEnumPopup<iOSLaunchScreenType>(properties.type, EditorGUIUtility.TextContent("Launch screen type"), kLaunchScreenTypeOrder, kLaunchScreenUIDescriptions);
            iOSLaunchScreenType intValue = (iOSLaunchScreenType) properties.type.intValue;
            switch (intValue)
            {
                case iOSLaunchScreenType.ImageAndBackgroundRelative:
                case iOSLaunchScreenType.ImageAndBackgroundConstant:
                    if (device == UnityEditor.iOS.DeviceType.iPhone)
                    {
                        SettingsUI.Texture2DField(properties.portraitImage, EditorGUIUtility.TextContent("Portrait Image | Use only advanced type textures"));
                        SettingsUI.Texture2DField(properties.landscapeImage, EditorGUIUtility.TextContent("Landscape Image | Use only advanced type textures"));
                    }
                    if (device == UnityEditor.iOS.DeviceType.iPad)
                    {
                        SettingsUI.Texture2DField(properties.portraitAndLandscapeImage, EditorGUIUtility.TextContent("Image | Use only advanced type textures"));
                    }
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(properties.backgroundColor, EditorGUIUtility.TextContent("Background Color"), new GUILayoutOption[0]);
                    if (intValue == iOSLaunchScreenType.ImageAndBackgroundConstant)
                    {
                        GUIContent label = null;
                        if (device == UnityEditor.iOS.DeviceType.iPad)
                        {
                            label = EditorGUIUtility.TextContent("Size in points | Vertical in both portrait and landscape");
                        }
                        else
                        {
                            label = EditorGUIUtility.TextContent("Size in points | Horizontal in portrait, vertical in landscape");
                        }
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(properties.size, label, new GUILayoutOption[0]);
                        if (EditorGUI.EndChangeCheck() && (properties.size.floatValue < 0f))
                        {
                            properties.size.floatValue = 0f;
                        }
                    }
                    else
                    {
                        GUIContent content2 = null;
                        if (device == UnityEditor.iOS.DeviceType.iPad)
                        {
                            content2 = EditorGUIUtility.TextContent("Fill percentage | Vertical in both portrait and landscape");
                        }
                        else
                        {
                            content2 = EditorGUIUtility.TextContent("Fill percentage | Horizontal in portrait, vertical in landscape");
                        }
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(properties.fillPercentage, content2, new GUILayoutOption[0]);
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (properties.fillPercentage.floatValue < 1f)
                            {
                                properties.fillPercentage.floatValue = 1f;
                            }
                            else if (properties.fillPercentage.floatValue > 100f)
                            {
                                properties.fillPercentage.floatValue = 100f;
                            }
                        }
                    }
                    break;

                default:
                    if (intValue == iOSLaunchScreenType.CustomXib)
                    {
                        PlayerSettingsEditor.BuildFileBoxButton(properties.customXibPath, LocalizationDatabase.GetLocalizedString("Custom Xib"), Application.dataPath, "xib", null);
                    }
                    break;
            }
        }

        private static int FindNewlyAddedItem(string tag, List<RequirementItem> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                RequirementItem item = items[i];
                if (item.name == tag)
                {
                    RequirementItem item2 = items[i];
                    return ((i + PlayerSettings.iOS.GetDeviceRequirementsForAssetBundleVariant(item2.name).count) - 1);
                }
            }
            return 0;
        }

        private static List<string> GetAllVariantNames()
        {
            HashSet<string> source = new HashSet<string>();
            List<Resource> list = UnityEditor.iOS.BuildPipeline.CollectResources();
            foreach (Resource resource in list)
            {
                if (resource.variants != null)
                {
                    foreach (Resource.Variant variant in resource.variants)
                    {
                        if (variant.variantName != null)
                        {
                            source.Add(variant.variantName);
                        }
                    }
                }
            }
            return source.ToList<string>();
        }

        private static List<RequirementItem> GetRequirementItems()
        {
            List<RequirementItem> list = new List<RequirementItem>();
            string[] assetBundleVariantsWithDeviceRequirements = PlayerSettings.iOS.GetAssetBundleVariantsWithDeviceRequirements();
            foreach (string str in assetBundleVariantsWithDeviceRequirements)
            {
                int count = PlayerSettings.iOS.GetDeviceRequirementsForAssetBundleVariant(str).count;
                for (int i = 0; i < count; i++)
                {
                    RequirementItem item = new RequirementItem {
                        name = str,
                        requirementId = i
                    };
                    list.Add(item);
                }
            }
            return list;
        }

        public override bool HasIdentificationGUI() => 
            true;

        public override bool HasPublishSection() => 
            false;

        public override void IdentificationSectionGUI()
        {
            ProvisioningProfileGUI.ShowUIWithDefaults(iOSEditorPrefKeys.kDefaultiOSProvisioningProfileUUID, this.m_AppleEnableAutomaticSigning, this.m_AutomaticSigningGUIContent, this.m_iOSManualSigningProvisioningProfileID, this.m_iOSManualSigningGUIContent, this.m_AppleDeveloperTeamID, this.m_TeamIDGUIContent);
        }

        private void InitRequirements()
        {
            this.m_RequirementList = new ReorderableList(GetRequirementItems(), typeof(RequirementItem), false, true, true, true);
            this.m_RequirementList.onAddDropdownCallback = new ReorderableList.AddDropdownCallbackDelegate(this.OnDeviceRequirementAdd);
            this.m_RequirementList.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(this.OnDeviceRequirementRemove);
            this.m_RequirementList.onSelectCallback = new ReorderableList.SelectCallbackDelegate(this.OnDeviceRequirementSelect);
            this.m_RequirementList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.OnDeviceRequirementDraw);
            this.m_RequirementList.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(this.OnDeviceRequirementHeaderDraw);
            this.CreateRequirementsForMissingTags();
        }

        private static void IOSBackgroundModesSection()
        {
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            iOSBackgroundMode backgroundModes = PlayerSettings.iOS.backgroundModes;
            iOSBackgroundMode none = iOSBackgroundMode.None;
            none |= IOSBackgroundModeToggle(backgroundModes, iOSBackgroundMode.Audio, "Audio, AirPlay, PiP");
            none |= IOSBackgroundModeToggle(backgroundModes, iOSBackgroundMode.Location, "Location updates");
            none |= IOSBackgroundModeToggle(backgroundModes, iOSBackgroundMode.None | iOSBackgroundMode.VOIP, "Voice over IP");
            none |= IOSBackgroundModeToggle(backgroundModes, iOSBackgroundMode.NewsstandContent, "Newsstand downloads");
            none |= IOSBackgroundModeToggle(backgroundModes, iOSBackgroundMode.ExternalAccessory, "External accessory communication");
            none |= IOSBackgroundModeToggle(backgroundModes, iOSBackgroundMode.BluetoothCentral, "Uses Bluetooth LE accessories");
            none |= IOSBackgroundModeToggle(backgroundModes, iOSBackgroundMode.BluetoothPeripheral, "Acts as a Bluetooth LE accessory");
            none |= IOSBackgroundModeToggle(backgroundModes, iOSBackgroundMode.Fetch, "Background fetch");
            none |= IOSBackgroundModeToggle(backgroundModes, iOSBackgroundMode.None | iOSBackgroundMode.RemoteNotification, "Remote notifications");
            if (EditorGUI.EndChangeCheck())
            {
                PlayerSettings.iOS.backgroundModes = none;
            }
            EditorGUI.indentLevel--;
        }

        private static iOSBackgroundMode IOSBackgroundModeToggle(iOSBackgroundMode currentModes, iOSBackgroundMode toggleMode, string title) => 
            (!EditorGUILayout.Toggle(title, (currentModes & toggleMode) != iOSBackgroundMode.None, new GUILayoutOption[0]) ? iOSBackgroundMode.None : toggleMode);

        private void OnDeviceRequirementAdd(Rect buttonRect, ReorderableList list)
        {
            string[] options = GetAllVariantNames().ToArray();
            EditorUtility.DisplayCustomMenu(buttonRect, options, new int[0], new EditorUtility.SelectMenuItemFunction(this.OnDeviceRequirementAddMenuSelected), null);
        }

        private void OnDeviceRequirementAddMenuSelected(object userData, string[] options, int selected)
        {
            string name = options[selected];
            PlayerSettings.iOS.GetDeviceRequirementsForAssetBundleVariant(name).Add(new iOSDeviceRequirement());
            List<RequirementItem> requirementItems = GetRequirementItems();
            this.m_RequirementList.list = requirementItems;
            this.m_RequirementList.index = FindNewlyAddedItem(name, requirementItems);
        }

        private void OnDeviceRequirementDraw(Rect rect, int index, bool isActive, bool isFocused)
        {
            RequirementItem item = (RequirementItem) this.m_RequirementList.list[index];
            iOSDeviceRequirementGroup deviceRequirementsForAssetBundleVariant = PlayerSettings.iOS.GetDeviceRequirementsForAssetBundleVariant(item.name);
            iOSDeviceRequirement requirement = deviceRequirementsForAssetBundleVariant[item.requirementId];
            GUI.Label(rect, item.name + " : " + DeviceRequirementUtils.RequirementToReadableString(requirement), EditorStyles.label);
        }

        private void OnDeviceRequirementHeaderDraw(Rect rect)
        {
            GUI.Label(rect, LocalizationDatabase.GetLocalizedString("Configured variant names"), EditorStyles.label);
        }

        private void OnDeviceRequirementRemove(ReorderableList list)
        {
            RequirementItem item = (RequirementItem) list.list[list.index];
            iOSDeviceRequirementGroup deviceRequirementsForAssetBundleVariant = PlayerSettings.iOS.GetDeviceRequirementsForAssetBundleVariant(item.name);
            deviceRequirementsForAssetBundleVariant.RemoveAt(item.requirementId);
            list.list = GetRequirementItems();
            list.index--;
        }

        private void OnDeviceRequirementSelect(ReorderableList list)
        {
            this.CreateRequirementsForMissingTags();
        }

        public override void OnEnable(PlayerSettingsEditor editor)
        {
            this.splashScreenProperties = new SerializedProperty[UnityEditor.iOS.SplashScreen.iOSTypes.Length];
            for (int i = 0; i < UnityEditor.iOS.SplashScreen.iOSTypes.Length; i++)
            {
                this.splashScreenProperties[i] = editor.FindPropertyAssert(UnityEditor.iOS.SplashScreen.iOSTypes[i].serializationName);
            }
            this.iPhoneLaunchScreenProperties.type = editor.FindPropertyAssert("iOSLaunchScreenType");
            this.iPhoneLaunchScreenProperties.portraitImage = editor.FindPropertyAssert("iOSLaunchScreenPortrait");
            this.iPhoneLaunchScreenProperties.landscapeImage = editor.FindPropertyAssert("iOSLaunchScreenLandscape");
            this.iPhoneLaunchScreenProperties.backgroundColor = editor.FindPropertyAssert("iOSLaunchScreenBackgroundColor");
            this.iPhoneLaunchScreenProperties.fillPercentage = editor.FindPropertyAssert("iOSLaunchScreenFillPct");
            this.iPhoneLaunchScreenProperties.size = editor.FindPropertyAssert("iOSLaunchScreenSize");
            this.iPhoneLaunchScreenProperties.customXibPath = editor.FindPropertyAssert("iOSLaunchScreenCustomXibPath");
            this.iPadLaunchScreenProperties.type = editor.FindPropertyAssert("iOSLaunchScreeniPadType");
            this.iPadLaunchScreenProperties.portraitAndLandscapeImage = editor.FindPropertyAssert("iOSLaunchScreeniPadImage");
            this.iPadLaunchScreenProperties.backgroundColor = editor.FindPropertyAssert("iOSLaunchScreeniPadBackgroundColor");
            this.iPadLaunchScreenProperties.fillPercentage = editor.FindPropertyAssert("iOSLaunchScreeniPadFillPct");
            this.iPadLaunchScreenProperties.size = editor.FindPropertyAssert("iOSLaunchScreeniPadSize");
            this.iPadLaunchScreenProperties.customXibPath = editor.FindPropertyAssert("iOSLaunchScreeniPadCustomXibPath");
            this.m_AppleDeveloperTeamID = editor.FindPropertyAssert("appleDeveloperTeamID");
            this.m_iOSManualSigningProvisioningProfileID = editor.FindPropertyAssert("iOSManualSigningProvisioningProfileID");
            this.m_AppleEnableAutomaticSigning = editor.FindPropertyAssert("appleEnableAutomaticSigning");
            this.InitRequirements();
            this.m_AppInBackgroundBehavior = editor.FindPropertyAssert("iosAppInBackgroundBehavior");
        }

        private static string PopupWithOptionalStringEntry(string label, string selected, string[] values, string[] displayedValues)
        {
            <PopupWithOptionalStringEntry>c__AnonStorey1 storey = new <PopupWithOptionalStringEntry>c__AnonStorey1 {
                selected = selected
            };
            List<string> list = new List<string>(displayedValues);
            int count = list.Count;
            list.Add(LocalizationDatabase.GetLocalizedString("Custom value"));
            int selectedIndex = Array.FindIndex<string>(values, new Predicate<string>(storey.<>m__0));
            if (selectedIndex >= 0)
            {
                int num3 = EditorGUILayout.Popup(label, selectedIndex, list.ToArray(), new GUILayoutOption[0]);
                if (num3 == count)
                {
                    return null;
                }
                return values[num3];
            }
            int index = EditorGUILayout.Popup(label, count, list.ToArray(), new GUILayoutOption[0]);
            EditorGUI.indentLevel++;
            string str2 = EditorGUILayout.TextField(storey.selected, new GUILayoutOption[0]);
            EditorGUI.indentLevel--;
            if (index != count)
            {
                return values[index];
            }
            return str2;
        }

        private void ShowDeviceRequirementSettingsGui(string variantName, int requirementId)
        {
            iOSDeviceRequirementGroup deviceRequirementsForAssetBundleVariant = PlayerSettings.iOS.GetDeviceRequirementsForAssetBundleVariant(variantName);
            iOSDeviceRequirement requirement = deviceRequirementsForAssetBundleVariant[requirementId];
            GUILayout.Label(EditorGUIUtility.TextContent("Variant settings"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.LabelField("Variant name", variantName, new GUILayoutOption[0]);
            string[] knownKeys = DeviceRequirementUtils.GetKnownKeys();
            EditorGUI.BeginChangeCheck();
            foreach (string str in knownKeys)
            {
                string defaultValueForKey = DeviceRequirementUtils.GetDefaultValueForKey(str);
                if (requirement.values.ContainsKey(str))
                {
                    defaultValueForKey = requirement.values[str];
                }
                string keyDescription = DeviceRequirementUtils.GetKeyDescription(str);
                string[] knownValuesForKey = DeviceRequirementUtils.GetKnownValuesForKey(str);
                string[] displayedValues = new string[knownValuesForKey.Length];
                for (int i = 0; i < knownValuesForKey.Length; i++)
                {
                    displayedValues[i] = DeviceRequirementUtils.GetValueDescription(str, knownValuesForKey[i]);
                }
                string str4 = PopupWithOptionalStringEntry(keyDescription, defaultValueForKey, knownValuesForKey, displayedValues);
                if (str4 == null)
                {
                    str4 = "<...>";
                }
                if (str4 == "")
                {
                    requirement.values.Remove(str);
                }
                else
                {
                    requirement.values[str] = str4;
                }
            }
            List<string> list = new List<string>(requirement.values.Keys);
            using (List<string>.Enumerator enumerator = list.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    <ShowDeviceRequirementSettingsGui>c__AnonStorey0 storey = new <ShowDeviceRequirementSettingsGui>c__AnonStorey0 {
                        key = enumerator.Current
                    };
                    if (!Array.Exists<string>(knownKeys, new Predicate<string>(storey.<>m__0)))
                    {
                        string text = requirement.values[storey.key];
                        Rect controlRect = EditorGUILayout.GetControlRect(true, new GUILayoutOption[0]);
                        float width = controlRect.width / 2f;
                        float num4 = controlRect.width - width;
                        Rect position = new Rect(controlRect.x, controlRect.y, width, controlRect.height);
                        Rect rect3 = new Rect(controlRect.x + width, controlRect.y, num4, controlRect.height);
                        string key = EditorGUI.TextField(position, storey.key);
                        string str7 = EditorGUI.TextField(rect3, text);
                        if (key != storey.key)
                        {
                            requirement.values.Remove(storey.key);
                            if ((key != "") && (str7 != ""))
                            {
                                requirement.values.Add(key, str7);
                            }
                        }
                        else if (str7 != text)
                        {
                            if (str7 == "")
                            {
                                requirement.values.Remove(storey.key);
                            }
                            else
                            {
                                requirement.values[storey.key] = str7;
                            }
                        }
                    }
                }
            }
            if (GUILayout.Button(EditorGUIUtility.TextContent("Add custom entry"), EditorStyles.miniButtonRight, new GUILayoutOption[0]))
            {
                requirement.values["<key>"] = "<value>";
            }
            if (EditorGUI.EndChangeCheck())
            {
                deviceRequirementsForAssetBundleVariant[requirementId] = requirement;
            }
        }

        private void ShowDeviceRequirementsForVariantNamesGui()
        {
            this.m_ResourceVariantsVisible = EditorGUILayout.Foldout(this.m_ResourceVariantsVisible, LocalizationDatabase.GetLocalizedString("Variant map for app slicing"), true);
            if (this.m_ResourceVariantsVisible)
            {
                EditorGUI.indentLevel++;
                this.m_RequirementList.DoLayoutList();
                if (this.m_RequirementList.index >= 0)
                {
                    RequirementItem item = (RequirementItem) this.m_RequirementList.list[this.m_RequirementList.index];
                    EditorGUI.indentLevel++;
                    this.ShowDeviceRequirementSettingsGui(item.name, item.requirementId);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
        }

        public override void SplashSectionGUI()
        {
            GUILayout.Label(EditorGUIUtility.TextContent("iPhone Launch Screen"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUI.indentLevel++;
            this.DrawLaunchScreenGUI(this.iPhoneLaunchScreenProperties, UnityEditor.iOS.DeviceType.iPhone);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            GUILayout.Label(EditorGUIUtility.TextContent("iPad Launch Screen"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUI.indentLevel++;
            this.DrawLaunchScreenGUI(this.iPadLaunchScreenProperties, UnityEditor.iOS.DeviceType.iPad);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            GUILayout.Label(EditorGUIUtility.TextContent("Legacy Launch Images"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUI.indentLevel++;
            for (int i = 0; i < UnityEditor.iOS.SplashScreen.iOSTypes.Length; i++)
            {
                SettingsUI.Texture2DField(this.splashScreenProperties[i], EditorGUIUtility.TextContent(UnityEditor.iOS.SplashScreen.iOSTypes[i].localizedNameAndTooltip));
                EditorGUILayout.Space();
            }
            EditorGUI.indentLevel--;
        }

        public override bool SupportsOrientation() => 
            true;

        [CompilerGenerated]
        private sealed class <PopupWithOptionalStringEntry>c__AnonStorey1
        {
            internal string selected;

            internal bool <>m__0(string v) => 
                (v == this.selected);
        }

        [CompilerGenerated]
        private sealed class <ShowDeviceRequirementSettingsGui>c__AnonStorey0
        {
            internal string key;

            internal bool <>m__0(string s) => 
                (s == this.key);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LaunchScreenProperties
        {
            public SerializedProperty type;
            public SerializedProperty portraitAndLandscapeImage;
            public SerializedProperty portraitImage;
            public SerializedProperty landscapeImage;
            public SerializedProperty backgroundColor;
            public SerializedProperty fillPercentage;
            public SerializedProperty size;
            public SerializedProperty customXibPath;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RequirementItem
        {
            public string name;
            public int requirementId;
        }
    }
}

