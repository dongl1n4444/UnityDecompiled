namespace UnityEditor.Web
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Connect;
    using UnityEditor.SceneManagement;

    [InitializeOnLoad]
    internal sealed class EditorProjectAccess
    {
        private const string kCloudEnabled = "CloudEnabled";
        private const string kCloudServiceKey = "CloudServices";

        static EditorProjectAccess()
        {
            JSProxyMgr.GetInstance().AddGlobalObject("unity/project", new EditorProjectAccess());
        }

        public void CloseToolbarWindow()
        {
            CollabToolbarWindow.CloseToolbarWindows();
        }

        public void CloseToolbarWindowImmediately()
        {
            CollabToolbarWindow.CloseToolbarWindowsImmediately();
        }

        public void EnableCloud(bool enable)
        {
            EditorUserSettings.SetConfigValue("CloudServices/CloudEnabled", enable.ToString());
        }

        public void EnterPlayMode()
        {
            EditorApplication.isPlaying = true;
        }

        public string GetBuildTarget() => 
            EditorUserBuildSettings.activeBuildTarget.ToString();

        public int GetEditorSkinIndex() => 
            EditorGUIUtility.skinIndex;

        public string GetEnvironment() => 
            UnityConnect.instance.GetEnvironment();

        public string GetOrganizationID() => 
            UnityConnect.instance.projectInfo.organizationId;

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern string GetProjectEditorVersion();
        public string GetProjectGUID() => 
            UnityConnect.instance.projectInfo.projectGUID;

        public string GetProjectIcon() => 
            null;

        public string GetProjectName()
        {
            string projectName = UnityConnect.instance.projectInfo.projectName;
            if (projectName != "")
            {
                return projectName;
            }
            return PlayerSettings.productName;
        }

        public string GetProjectPath() => 
            Directory.GetCurrentDirectory();

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern string GetRESTServiceURI();
        public string GetUserAccessToken() => 
            UnityConnect.instance.GetAccessToken();

        public string GetUserDisplayName() => 
            UnityConnect.instance.userInfo.displayName;

        public string GetUserName() => 
            UnityConnect.instance.userInfo.userName;

        public string GetUserPrimaryOrganizationId() => 
            UnityConnect.instance.userInfo.primaryOrg;

        public void GoToHistory()
        {
            CollabHistoryWindow.ShowHistoryWindow().Focus();
        }

        public bool IsLoggedIn() => 
            UnityConnect.instance.loggedIn;

        public bool IsOnline() => 
            UnityConnect.instance.online;

        public bool IsPlayMode() => 
            EditorApplication.isPlaying;

        public bool IsProjectBound() => 
            UnityConnect.instance.projectInfo.projectBound;

        public void OpenLink(string link)
        {
            Help.BrowseURL(link);
        }

        public bool SaveCurrentModifiedScenesIfUserWantsTo() => 
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        public void ShowToolbarDropdown()
        {
            Toolbar.requestShowCollabToolbar = true;
            if (Toolbar.get != null)
            {
                Toolbar.get.Repaint();
            }
        }
    }
}

