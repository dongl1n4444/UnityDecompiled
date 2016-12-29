namespace UnityEditorInternal
{
    using System;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEngine;

    public class iOSPreferenceWindowExtension : IPreferenceWindowExtension
    {
        private bool m_DefaultAutomaticallySignBuild = true;
        private string m_DefaultAutomaticSignTeamId = null;
        private ProvisioningProfile m_DefaultiOSProvisioningProfile = null;
        private ProvisioningProfile m_DefaulttvOSProvisioningProfile = null;

        public bool HasExternalApplications() => 
            true;

        public void ReadPreferences()
        {
            this.m_DefaultAutomaticallySignBuild = EditorPrefs.GetBool(iOSEditorPrefKeys.kDefaultiOSAutomaticallySignBuild, true);
            this.m_DefaultAutomaticSignTeamId = EditorPrefs.GetString(iOSEditorPrefKeys.kDefaultiOSAutomaticSignTeamId);
            string uUID = EditorPrefs.GetString(iOSEditorPrefKeys.kDefaultiOSProvisioningProfileUUID);
            this.m_DefaultiOSProvisioningProfile = new ProvisioningProfile(uUID);
            uUID = EditorPrefs.GetString(iOSEditorPrefKeys.kDefaulttvOSProvisioningProfileUUID);
            this.m_DefaulttvOSProvisioningProfile = new ProvisioningProfile(uUID);
        }

        public void ShowExternalApplications()
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinHeight(25f) };
            EditorGUILayout.LabelField(EditorGUIUtility.TextContent("Xcode Default Settings|All of these settings can be overridden in the Player Settings panel"), EditorStyles.boldLabel, options);
            this.m_DefaultAutomaticallySignBuild = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Automatically Sign|Can be overridden in Player Settings"), this.m_DefaultAutomaticallySignBuild, new GUILayoutOption[0]);
            this.m_DefaultAutomaticSignTeamId = EditorGUILayout.TextField(EditorGUIUtility.TextContent("Automatic Signing Team Id:|Can be overriden in Player Settings"), this.m_DefaultAutomaticSignTeamId, new GUILayoutOption[0]);
            GUILayout.Space(10f);
            ProvisioningProfileGUI.ShowProvisioningProfileUIWithCallback(EditorGUIUtility.TextContent("iOS Manual Provisioning Profile|Can be overridden in Player Settings"), this.m_DefaultiOSProvisioningProfile, delegate (ProvisioningProfile profile) {
                this.m_DefaultiOSProvisioningProfile = profile;
                this.WritePreferences();
                this.ReadPreferences();
            });
            GUILayout.Space(10f);
            ProvisioningProfileGUI.ShowProvisioningProfileUIWithCallback(EditorGUIUtility.TextContent("tvOS Manual Provisioning Profile|Can be overridden in Player Settings"), this.m_DefaulttvOSProvisioningProfile, delegate (ProvisioningProfile profile) {
                this.m_DefaulttvOSProvisioningProfile = profile;
                this.WritePreferences();
                this.ReadPreferences();
            });
        }

        public void WritePreferences()
        {
            EditorPrefs.SetBool(iOSEditorPrefKeys.kDefaultiOSAutomaticallySignBuild, this.m_DefaultAutomaticallySignBuild);
            EditorPrefs.SetString(iOSEditorPrefKeys.kDefaultiOSAutomaticSignTeamId, this.m_DefaultAutomaticSignTeamId);
            if (this.m_DefaultiOSProvisioningProfile != null)
            {
                EditorPrefs.SetString(iOSEditorPrefKeys.kDefaultiOSProvisioningProfileUUID, this.m_DefaultiOSProvisioningProfile.UUID);
            }
            if (this.m_DefaulttvOSProvisioningProfile != null)
            {
                EditorPrefs.SetString(iOSEditorPrefKeys.kDefaulttvOSProvisioningProfileUUID, this.m_DefaulttvOSProvisioningProfile.UUID);
            }
        }
    }
}

