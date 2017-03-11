namespace UnityEditor.Tizen
{
    using System;
    using System.Collections;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEngine;

    internal class TizenSettingsEditorExtension : DefaultPlayerSettingsEditorExtension
    {
        private Vector2 capScrollViewPosition = Vector2.zero;
        private SerializedProperty m_ApplicationBundleVersion;
        private SerializedProperty m_IPhoneSplashScreen;
        private GUIContent m_LabelAll = EditorGUIUtility.TextContent("All");
        private GUIContent m_LabelCapabilities = EditorGUIUtility.TextContent("Capabilities");
        private GUIContent m_LabelDeploymentTarget = EditorGUIUtility.TextContent("Deployment Target");
        private GUIContent m_LabelDesc = EditorGUIUtility.TextContent("Product Description");
        private GUIContent m_LabelDiscover = EditorGUIUtility.TextContent("Discover");
        private GUIContent m_LabelDiscoverDevices = EditorGUIUtility.TextContent("Discover Tizen Devices");
        private GUIContent m_LabelEmulator = EditorGUIUtility.TextContent("Emulator");
        private GUIContent m_LabelMobile = EditorGUIUtility.TextContent("Mobile");
        private GUIContent m_LabelProfile = EditorGUIUtility.TextContent("Signing Profile Name");
        private GUIContent m_LabelURL = EditorGUIUtility.TextContent("Product URL");
        private PlayerSettingsEditor m_SettingsEditor;
        private SerializedProperty m_TizenMinOSVersion;
        private SerializedProperty m_TizenProductDescription;
        private SerializedProperty m_TizenProductURL;
        private SerializedProperty m_TizenProfileName;

        public override bool CanShowUnitySplashScreen() => 
            true;

        public override void ConfigurationSectionGUI()
        {
        }

        public override bool HasIdentificationGUI() => 
            true;

        public override void IdentificationSectionGUI()
        {
            PlayerSettingsEditor.ShowApplicationIdentifierUI(this.m_SettingsEditor.serializedObject, BuildTargetGroup.Tizen, "Package ID", "Changed Tizen Package ID");
            EditorGUILayout.PropertyField(this.m_ApplicationBundleVersion, EditorGUIUtility.TextContent("Version*"), new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_TizenMinOSVersion, EditorGUIUtility.TextContent("Minimum API Level"), new GUILayoutOption[0]);
        }

        public override void OnEnable(PlayerSettingsEditor settingsEditor)
        {
            this.m_TizenProductDescription = settingsEditor.FindPropertyAssert("tizenProductDescription");
            this.m_TizenProductURL = settingsEditor.FindPropertyAssert("tizenProductURL");
            this.m_TizenProfileName = settingsEditor.FindPropertyAssert("tizenSigningProfileName");
            this.m_TizenMinOSVersion = settingsEditor.FindPropertyAssert("tizenMinOSVersion");
            this.m_IPhoneSplashScreen = settingsEditor.FindPropertyAssert("iPhoneSplashScreen");
            this.m_ApplicationBundleVersion = settingsEditor.FindPropertyAssert("bundleVersion");
            this.m_SettingsEditor = settingsEditor;
        }

        public override void PublishSectionGUI(float h, float kLabelFloatMinW, float kLabelFloatMaxW)
        {
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUILayout.PrefixLabel(this.m_LabelDeploymentTarget, EditorStyles.miniButton);
            if (GUILayout.Button(this.m_LabelDiscover, EditorStyles.miniButton, new GUILayoutOption[0]))
            {
                DeploymentTargetBrowser window = (DeploymentTargetBrowser) EditorWindow.GetWindow(typeof(DeploymentTargetBrowser));
                window.titleContent = this.m_LabelDiscoverDevices;
                window.Show();
            }
            EditorGUILayout.EndHorizontal();
            if (PlayerSettings.Tizen.deploymentTargetType != -1)
            {
                string text = "";
                switch (PlayerSettings.Tizen.deploymentTargetType)
                {
                    case 0:
                        text = this.m_LabelMobile.text + ": " + PlayerSettings.Tizen.deploymentTarget;
                        break;

                    case 1:
                        text = this.m_LabelEmulator.text + ": " + PlayerSettings.Tizen.deploymentTarget;
                        break;

                    case 2:
                        text = this.m_LabelAll.text;
                        break;
                }
                GUILayout.Label(text, EditorStyles.boldLabel, new GUILayoutOption[0]);
            }
            EditorGUILayout.PropertyField(this.m_TizenProductDescription, this.m_LabelDesc, new GUILayoutOption[0]);
            string stringValue = this.m_TizenProductURL.stringValue;
            EditorGUILayout.PropertyField(this.m_TizenProductURL, this.m_LabelURL, new GUILayoutOption[0]);
            PlayerSettings.Tizen.productURL = stringValue;
            string signingProfileName = PlayerSettings.Tizen.signingProfileName;
            this.m_TizenProfileName.stringValue = EditorGUILayout.TextField(this.m_LabelProfile.text, signingProfileName, new GUILayoutOption[0]);
            PlayerSettings.Tizen.signingProfileName = signingProfileName;
            PlayerSettings.Tizen.productDescription = this.m_TizenProductDescription.stringValue;
            GUILayout.Label(this.m_LabelCapabilities, EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinHeight(200f) };
            this.capScrollViewPosition = GUILayout.BeginScrollView(this.capScrollViewPosition, EditorStyles.helpBox, options);
            IEnumerator enumerator = Enum.GetValues(typeof(PlayerSettings.TizenCapability)).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    PlayerSettings.TizenCapability current = (PlayerSettings.TizenCapability) enumerator.Current;
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    if (current == PlayerSettings.TizenCapability.PackageManagerInfo)
                    {
                        GUI.enabled = false;
                    }
                    bool capability = PlayerSettings.Tizen.GetCapability(current);
                    GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.MinWidth(150f) };
                    bool flag = GUILayout.Toggle(capability, current.ToString(), optionArray2);
                    if (current == PlayerSettings.TizenCapability.PackageManagerInfo)
                    {
                        flag = true;
                        GUI.enabled = true;
                    }
                    PlayerSettings.Tizen.SetCapability(current, flag);
                    GUILayout.EndHorizontal();
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
            GUILayout.EndScrollView();
        }

        public override void SplashSectionGUI()
        {
            this.m_IPhoneSplashScreen.objectReferenceValue = EditorGUILayout.ObjectField(EditorGUIUtility.TextContent("Mobile Splash Screen*"), (Texture2D) this.m_IPhoneSplashScreen.objectReferenceValue, typeof(Texture2D), false, new GUILayoutOption[0]);
        }

        public override bool SupportsOrientation() => 
            true;
    }
}

