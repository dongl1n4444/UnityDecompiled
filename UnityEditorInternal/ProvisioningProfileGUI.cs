namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;

    internal class ProvisioningProfileGUI
    {
        internal static ProvisioningProfile Browse(string path)
        {
            string title = "Select the Provising Profile used for Manual Signing";
            string directory = path;
            if (InternalEditorUtility.inBatchMode)
            {
                return null;
            }
            ProvisioningProfile provisioningProfile = null;
            do
            {
                path = EditorUtility.OpenFilePanel(title, directory, "mobileprovision");
                if (path.Length == 0)
                {
                    return null;
                }
            }
            while (!GetProvisioningProfileId(path, out provisioningProfile));
            return provisioningProfile;
        }

        private static bool GetBoolForAutomaticSigningValue(int signingValue) => 
            (signingValue == 1);

        private static int GetDefaultAutomaticSigningValue(SerializedProperty prop, string editorPropKey)
        {
            int intValue = prop.intValue;
            if (intValue == 0)
            {
                intValue = !EditorPrefs.GetBool(editorPropKey, true) ? 2 : 1;
            }
            return intValue;
        }

        private static string GetDefaultStringValue(SerializedProperty prop, string editorPrefKey)
        {
            string stringValue = prop.stringValue;
            if (string.IsNullOrEmpty(stringValue))
            {
                stringValue = EditorPrefs.GetString(editorPrefKey, "");
            }
            return stringValue;
        }

        private static int GetIntValueForAutomaticSigningBool(bool automaticallySign) => 
            (!automaticallySign ? 2 : 1);

        internal static bool GetProvisioningProfileId(string filePath, out ProvisioningProfile provisioningProfile)
        {
            ProvisioningProfile profile = ProvisioningProfile.ParseProvisioningProfileAtPath(filePath);
            provisioningProfile = profile;
            return (profile.UUID != null);
        }

        internal static void ShowProvisioningProfileUIWithCallback(GUIContent titleWithToolTip, ProvisioningProfile profile, ProvisioningProfileChangedDelegate callback)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label(titleWithToolTip, EditorStyles.label, new GUILayoutOption[0]);
            if (GUILayout.Button("Browse", EditorStyles.miniButton, new GUILayoutOption[0]))
            {
                ProvisioningProfile profile2 = Browse("");
                if ((profile2 != null) && !string.IsNullOrEmpty(profile2.UUID))
                {
                    profile = profile2;
                    callback(profile);
                    GUI.FocusControl("");
                }
            }
            GUILayout.EndHorizontal();
            EditorGUI.BeginChangeCheck();
            EditorGUI.indentLevel++;
            GUIContent label = EditorGUIUtility.TextContent("Profile ID:");
            profile.UUID = EditorGUILayout.TextField(label, profile.UUID, new GUILayoutOption[0]);
            EditorGUI.indentLevel--;
            if (EditorGUI.EndChangeCheck())
            {
                callback(profile);
            }
        }

        private static void ShowProvisioningProfileUIWithDefaults(string defaultPreferenceKey, SerializedProperty uuidProp, GUIContent title)
        {
            string stringValue = uuidProp.stringValue;
            if (string.IsNullOrEmpty(stringValue))
            {
                stringValue = EditorPrefs.GetString(defaultPreferenceKey);
            }
            ShowProvisioningProfileUIWithProperty(title, new ProvisioningProfile(stringValue), uuidProp);
        }

        internal static void ShowProvisioningProfileUIWithProperty(GUIContent titleWithToolTip, ProvisioningProfile profile, SerializedProperty prop)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label(titleWithToolTip, EditorStyles.label, new GUILayoutOption[0]);
            Rect totalPosition = EditorGUILayout.GetControlRect(false, 0f, new GUILayoutOption[0]);
            GUIContent label = EditorGUIUtility.TextContent("Profile ID:");
            EditorGUI.BeginProperty(totalPosition, label, prop);
            if (GUILayout.Button("Browse", EditorStyles.miniButton, new GUILayoutOption[0]))
            {
                ProvisioningProfile profile2 = Browse("");
                if ((profile2 != null) && !string.IsNullOrEmpty(profile2.UUID))
                {
                    profile = profile2;
                    prop.stringValue = profile.UUID;
                    GUI.FocusControl("");
                }
            }
            GUILayout.EndHorizontal();
            EditorGUI.EndProperty();
            EditorGUI.BeginChangeCheck();
            EditorGUI.indentLevel++;
            totalPosition = EditorGUILayout.GetControlRect(true, 0f, new GUILayoutOption[0]);
            label = EditorGUIUtility.TextContent("Profile ID:");
            EditorGUI.BeginProperty(totalPosition, label, prop);
            profile.UUID = EditorGUILayout.TextField(label, profile.UUID, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                prop.stringValue = profile.UUID;
            }
            EditorGUI.EndProperty();
            EditorGUI.indentLevel--;
        }

        internal static void ShowUIWithDefaults(string provisioningPrefKey, SerializedProperty enableAutomaticSigningProp, GUIContent automaticSigningGUI, SerializedProperty manualSigningIDProp, GUIContent manualSigningProfileGUI, SerializedProperty appleDevIDProp, GUIContent teamIDGUIContent)
        {
            bool boolForAutomaticSigningValue = GetBoolForAutomaticSigningValue(GetDefaultAutomaticSigningValue(enableAutomaticSigningProp, iOSEditorPrefKeys.kDefaultiOSAutomaticallySignBuild));
            EditorGUI.BeginProperty(EditorGUILayout.GetControlRect(true, 0f, new GUILayoutOption[0]), automaticSigningGUI, enableAutomaticSigningProp);
            bool automaticallySign = EditorGUILayout.Toggle(automaticSigningGUI, boolForAutomaticSigningValue, new GUILayoutOption[0]);
            if (automaticallySign != boolForAutomaticSigningValue)
            {
                enableAutomaticSigningProp.intValue = GetIntValueForAutomaticSigningBool(automaticallySign);
            }
            EditorGUI.EndProperty();
            if (!automaticallySign)
            {
                ShowProvisioningProfileUIWithDefaults(provisioningPrefKey, manualSigningIDProp, manualSigningProfileGUI);
            }
            else
            {
                string defaultStringValue = GetDefaultStringValue(appleDevIDProp, iOSEditorPrefKeys.kDefaultiOSAutomaticSignTeamId);
                string str2 = null;
                EditorGUI.BeginProperty(EditorGUILayout.GetControlRect(true, 0f, new GUILayoutOption[0]), teamIDGUIContent, appleDevIDProp);
                EditorGUI.BeginChangeCheck();
                str2 = EditorGUILayout.TextField(teamIDGUIContent, defaultStringValue, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    appleDevIDProp.stringValue = str2;
                }
                EditorGUI.EndProperty();
            }
        }

        internal delegate void ProvisioningProfileChangedDelegate(ProvisioningProfile profile);
    }
}

