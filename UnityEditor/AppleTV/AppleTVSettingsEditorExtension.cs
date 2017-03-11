namespace UnityEditor.AppleTV
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.iOS;
    using UnityEditor.Modules;
    using UnityEditorInternal;
    using UnityEngine;

    internal class AppleTVSettingsEditorExtension : DefaultPlayerSettingsEditorExtension
    {
        [CompilerGenerated]
        private static ReorderableIconLayerList.ChangedCallbackDelegate <>f__am$cache0;
        [CompilerGenerated]
        private static ReorderableIconLayerList.ChangedCallbackDelegate <>f__am$cache1;
        [CompilerGenerated]
        private static ReorderableIconLayerList.ChangedCallbackDelegate <>f__am$cache2;
        [CompilerGenerated]
        private static ReorderableIconLayerList.ChangedCallbackDelegate <>f__am$cache3;
        private SerializedProperty m_AppleDeveloperTeamID;
        private SerializedProperty m_AppleEnableAutomaticSigning;
        private SerializedProperty m_ApplicationBundleVersion;
        private GUIContent m_AutomaticSigningGUIContent = EditorGUIUtility.TextContent("Automatically Sign|Check this to allow Xcode to Automatically sign your build.");
        private ReorderableIconLayerList m_LargeIconsLayers;
        private PlayerSettingsEditor m_SettingsEditor;
        private ReorderableIconLayerList m_SmallIconsLayers;
        private GUIContent m_TeamIDGUIContent = EditorGUIUtility.TextContent("Automatic Signing Team ID|Developers can retrieve their Team ID by visiting the Apple Developer site under Account > Membership.");
        private ReorderableIconLayerList m_TopShelfImageLayers;
        private ReorderableIconLayerList m_TopShelfImageWideLayers;
        private GUIContent m_tvosManualSigningGUIContent = EditorGUIUtility.TextContent("tvOS Provisioning Profile");
        private SerializedProperty m_tvOSManualSigningProvisioningProfileID;
        private SerializedProperty m_tvOSRequireExtendedGameController;
        private SerializedProperty[] splashScreenProperties;
        private static readonly UnityEditor.iOS.SplashScreen[] splashScreenTypes = UnityEditor.iOS.SplashScreen.tvOSTypes;

        public override bool CanShowUnitySplashScreen() => 
            true;

        public override void ConfigurationSectionGUI()
        {
            SettingsUI.ShowArchitectureButton(BuildTargetGroup.tvOS);
            EditorGUILayout.PropertyField(this.m_tvOSRequireExtendedGameController, EditorGUIUtility.TextContent("Require Extended Game Controller"), new GUILayoutOption[0]);
        }

        private void EnsureMinimumNumberOfTextures(ReorderableIconLayerList list)
        {
            while (list.textures.Count < list.minItems)
            {
                list.textures.Add(null);
            }
        }

        public override bool HasIdentificationGUI() => 
            true;

        public override bool HasPublishSection() => 
            false;

        public override void IconSectionGUI()
        {
            this.m_SmallIconsLayers.textures = PlayerSettings.tvOS.GetSmallIconLayers().ToList<Texture2D>();
            this.m_LargeIconsLayers.textures = PlayerSettings.tvOS.GetLargeIconLayers().ToList<Texture2D>();
            this.m_TopShelfImageLayers.textures = PlayerSettings.tvOS.GetTopShelfImageLayers().ToList<Texture2D>();
            this.m_TopShelfImageWideLayers.textures = PlayerSettings.tvOS.GetTopShelfImageWideLayers().ToList<Texture2D>();
            this.EnsureMinimumNumberOfTextures(this.m_SmallIconsLayers);
            this.EnsureMinimumNumberOfTextures(this.m_LargeIconsLayers);
            this.EnsureMinimumNumberOfTextures(this.m_TopShelfImageLayers);
            this.EnsureMinimumNumberOfTextures(this.m_TopShelfImageWideLayers);
            this.m_SmallIconsLayers.DoLayoutList();
            this.m_LargeIconsLayers.DoLayoutList();
            this.m_TopShelfImageLayers.DoLayoutList();
            this.m_TopShelfImageWideLayers.DoLayoutList();
        }

        public override void IdentificationSectionGUI()
        {
            PlayerSettingsEditor.ShowApplicationIdentifierUI(this.m_SettingsEditor.serializedObject, BuildTargetGroup.tvOS, "Bundle Identifier", "Changed tvOS bundleIdentifier");
            EditorGUILayout.PropertyField(this.m_ApplicationBundleVersion, EditorGUIUtility.TextContent("Version*"), new GUILayoutOption[0]);
            PlayerSettingsEditor.ShowBuildNumberUI(this.m_SettingsEditor.serializedObject, BuildTargetGroup.tvOS, "Build", "Changed tvOS build number");
            ProvisioningProfileGUI.ShowUIWithDefaults(iOSEditorPrefKeys.kDefaulttvOSProvisioningProfileUUID, this.m_AppleEnableAutomaticSigning, this.m_AutomaticSigningGUIContent, this.m_tvOSManualSigningProvisioningProfileID, this.m_tvosManualSigningGUIContent, this.m_AppleDeveloperTeamID, this.m_TeamIDGUIContent);
        }

        public override void OnEnable(PlayerSettingsEditor editor)
        {
            this.m_ApplicationBundleVersion = editor.FindPropertyAssert("bundleVersion");
            this.splashScreenProperties = new SerializedProperty[splashScreenTypes.Length];
            for (int i = 0; i < splashScreenTypes.Length; i++)
            {
                this.splashScreenProperties[i] = editor.FindPropertyAssert(splashScreenTypes[i].serializationName);
            }
            this.m_SmallIconsLayers = new ReorderableIconLayerList();
            this.m_SmallIconsLayers.headerString = LocalizationDatabase.GetLocalizedString("Small icon layers");
            this.m_SmallIconsLayers.minItems = 2;
            this.m_SmallIconsLayers.SetImageSize(400, 240);
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = list => PlayerSettings.tvOS.SetSmallIconLayers(list.textures.ToArray());
            }
            this.m_SmallIconsLayers.onChangedCallback = <>f__am$cache0;
            this.m_LargeIconsLayers = new ReorderableIconLayerList();
            this.m_LargeIconsLayers.headerString = LocalizationDatabase.GetLocalizedString("Large icon layers");
            this.m_LargeIconsLayers.minItems = 2;
            this.m_LargeIconsLayers.SetImageSize(0x500, 0x300);
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = list => PlayerSettings.tvOS.SetLargeIconLayers(list.textures.ToArray());
            }
            this.m_LargeIconsLayers.onChangedCallback = <>f__am$cache1;
            this.m_TopShelfImageLayers = new ReorderableIconLayerList();
            this.m_TopShelfImageLayers.headerString = LocalizationDatabase.GetLocalizedString("Top shelf image layers");
            this.m_TopShelfImageLayers.minItems = 1;
            this.m_TopShelfImageLayers.SetImageSize(0x780, 720);
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = list => PlayerSettings.tvOS.SetTopShelfImageLayers(list.textures.ToArray());
            }
            this.m_TopShelfImageLayers.onChangedCallback = <>f__am$cache2;
            this.m_TopShelfImageWideLayers = new ReorderableIconLayerList();
            this.m_TopShelfImageWideLayers.headerString = LocalizationDatabase.GetLocalizedString("Top shelf image wide layers");
            this.m_TopShelfImageWideLayers.minItems = 1;
            this.m_TopShelfImageWideLayers.SetImageSize(0x910, 720);
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = list => PlayerSettings.tvOS.SetTopShelfImageWideLayers(list.textures.ToArray());
            }
            this.m_TopShelfImageWideLayers.onChangedCallback = <>f__am$cache3;
            this.m_tvOSRequireExtendedGameController = editor.serializedObject.FindProperty("tvOSRequireExtendedGameController");
            this.m_tvOSManualSigningProvisioningProfileID = editor.FindPropertyAssert("tvOSManualSigningProvisioningProfileID");
            this.m_AppleEnableAutomaticSigning = editor.FindPropertyAssert("appleEnableAutomaticSigning");
            this.m_AppleDeveloperTeamID = editor.FindPropertyAssert("appleDeveloperTeamID");
            this.m_SettingsEditor = editor;
        }

        public override void SplashSectionGUI()
        {
            for (int i = 0; i < splashScreenTypes.Length; i++)
            {
                SettingsUI.Texture2DField(this.splashScreenProperties[i], EditorGUIUtility.TextContent(splashScreenTypes[i].localizedNameAndTooltip));
                EditorGUILayout.Space();
            }
        }

        public override bool UsesStandardIcons() => 
            false;
    }
}

