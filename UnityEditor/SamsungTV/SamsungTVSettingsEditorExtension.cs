namespace UnityEditor.SamsungTV
{
    using System;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEngine;

    internal class SamsungTVSettingsEditorExtension : DefaultPlayerSettingsEditorExtension
    {
        private SerializedProperty m_IgnoreAlphaClear;
        private SerializedProperty m_IPhoneSplashScreen;
        private SerializedProperty m_SamsungTVDeviceAddress;
        private SerializedProperty m_SamsungTVProductAuthor;
        private SerializedProperty m_SamsungTVProductAuthorEmail;
        private SerializedProperty m_SamsungTVProductCategory;
        private SerializedProperty m_SamsungTVProductDescription;
        private SerializedProperty m_SamsungTVProductLink;
        private PlayerSettingsEditor m_SettingsEditor;

        public override bool CanShowUnitySplashScreen() => 
            true;

        public override bool HasBundleIdentifier() => 
            false;

        public override bool HasIdentificationGUI() => 
            true;

        public override void OnEnable(PlayerSettingsEditor settingsEditor)
        {
            this.m_SamsungTVDeviceAddress = settingsEditor.FindPropertyAssert("stvDeviceAddress");
            this.m_SamsungTVProductDescription = settingsEditor.FindPropertyAssert("stvProductDescription");
            this.m_SamsungTVProductAuthor = settingsEditor.FindPropertyAssert("stvProductAuthor");
            this.m_SamsungTVProductAuthorEmail = settingsEditor.FindPropertyAssert("stvProductAuthorEmail");
            this.m_SamsungTVProductLink = settingsEditor.FindPropertyAssert("stvProductLink");
            this.m_SamsungTVProductCategory = settingsEditor.FindPropertyAssert("stvProductCategory");
            this.m_IPhoneSplashScreen = settingsEditor.FindPropertyAssert("iPhoneSplashScreen");
            this.m_IgnoreAlphaClear = settingsEditor.FindPropertyAssert("ignoreAlphaClear");
            this.m_SettingsEditor = settingsEditor;
        }

        public override void PublishSectionGUI(float h, float kLabelFloatMinW, float kLabelFloatMaxW)
        {
            EditorGUILayout.PropertyField(this.m_SamsungTVDeviceAddress, EditorGUIUtility.TextContent("Device Address"), new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_SamsungTVProductDescription, EditorGUIUtility.TextContent("Description"), new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_SamsungTVProductAuthor, EditorGUIUtility.TextContent("Author"), new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_SamsungTVProductAuthorEmail, EditorGUIUtility.TextContent("E-Mail"), new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_SamsungTVProductLink, EditorGUIUtility.TextContent("Link"), new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_SamsungTVProductCategory, EditorGUIUtility.TextContent("Category"), new GUILayoutOption[0]);
        }

        public override void ResolutionSectionGUI(float h, float midWidth, float maxWidth)
        {
            this.m_IgnoreAlphaClear.boolValue = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Ignore BG Alpha Clear"), this.m_IgnoreAlphaClear.boolValue, new GUILayoutOption[0]);
        }

        public void SetTVDeviceAddressString(string tvAddress)
        {
            if (!this.m_SamsungTVDeviceAddress.stringValue.Contains(tvAddress))
            {
                this.m_SamsungTVDeviceAddress.stringValue = this.m_SamsungTVDeviceAddress.stringValue + "/" + tvAddress;
                this.m_SettingsEditor.serializedObject.ApplyModifiedProperties();
            }
        }

        public override void SplashSectionGUI()
        {
            this.m_IPhoneSplashScreen.objectReferenceValue = EditorGUILayout.ObjectField(EditorGUIUtility.TextContent("Mobile Splash Screen*"), (Texture2D) this.m_IPhoneSplashScreen.objectReferenceValue, typeof(Texture2D), false, new GUILayoutOption[0]);
        }
    }
}

