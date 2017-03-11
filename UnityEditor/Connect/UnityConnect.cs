namespace UnityEditor.Connect
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.Web;
    using UnityEngine.Scripting;

    [InitializeOnLoad]
    internal sealed class UnityConnect
    {
        private static readonly UnityConnect s_Instance = new UnityConnect();

        [field: DebuggerBrowsable(0), CompilerGenerated]
        public event ProjectStateChangedDelegate ProjectStateChanged;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event StateChangedDelegate StateChanged;

        [field: DebuggerBrowsable(0), CompilerGenerated]
        public event UserStateChangedDelegate UserStateChanged;

        static UnityConnect()
        {
            JSProxyMgr.GetInstance().AddGlobalObject("unity/connect", s_Instance);
        }

        private UnityConnect()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void BindProject(string projectGUID, string projectName, string organizationId);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void ClearAccessToken();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void ClearCache();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void ClearError(int errorCode);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void ClearErrors();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void ComputerDidWakeUp();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void ComputerGoesToSleep();
        public bool DisplayDialog(string title, string message, string okBtn, string cancelBtn) => 
            EditorUtility.DisplayDialog(title, message, okBtn, cancelBtn);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern string GetAccessToken();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern string GetAPIVersion();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern string GetConfigurationURL(CloudConfigUrl config);
        public string GetConfigurationUrlByIndex(int index)
        {
            if (index == 0)
            {
                return this.GetConfigurationURL(CloudConfigUrl.CloudCore);
            }
            if (index == 1)
            {
                return this.GetConfigurationURL(CloudConfigUrl.CloudCollab);
            }
            if (index == 2)
            {
                return this.GetConfigurationURL(CloudConfigUrl.CloudWebauth);
            }
            if (index == 3)
            {
                return this.GetConfigurationURL(CloudConfigUrl.CloudLogin);
            }
            if (index == 6)
            {
                return this.GetConfigurationURL(CloudConfigUrl.CloudIdentity);
            }
            if (index == 7)
            {
                return this.GetConfigurationURL(CloudConfigUrl.CloudPortal);
            }
            return "";
        }

        public ConnectInfo GetConnectInfo() => 
            this.connectInfo;

        public string GetCoreConfigurationUrl() => 
            this.GetConfigurationURL(CloudConfigUrl.CloudCore);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern string GetEnvironment();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern string GetOrganizationForeignKey();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern string GetOrganizationId();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern string GetOrganizationName();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern string GetProjectGUID();
        public ProjectInfo GetProjectInfo() => 
            this.projectInfo;

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern string GetProjectName();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern string GetUserId();
        public UserInfo GetUserInfo() => 
            this.userInfo;

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern string GetUserName();
        public void GoToHub(string page)
        {
            UnityConnectServiceCollection.instance.ShowService("Hub", page, true);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void Logout();
        private static void OnProjectStateChanged()
        {
            ProjectStateChangedDelegate projectStateChanged = instance.ProjectStateChanged;
            if (projectStateChanged != null)
            {
                projectStateChanged(instance.projectInfo);
            }
        }

        private static void OnStateChanged()
        {
            StateChangedDelegate stateChanged = instance.StateChanged;
            if (stateChanged != null)
            {
                stateChanged(instance.connectInfo);
            }
        }

        private static void OnUserStateChanged()
        {
            UserStateChangedDelegate userStateChanged = instance.UserStateChanged;
            if (userStateChanged != null)
            {
                userStateChanged(instance.userInfo);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void OpenAuthorizedURLInWebBrowser(string url);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void RefreshProject();
        public bool SetCOPPACompliance(int compliance) => 
            this.SetCOPPACompliance((COPPACompliance) compliance);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool SetCOPPACompliance(COPPACompliance compliance);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetError(int errorCode);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void ShowLogin();
        [UnityEditor.MenuItem("Window/Unity Connect/Reset AccessToken", false, 0x3e8, true)]
        public static void TestClearAccessToken()
        {
            instance.ClearAccessToken();
        }

        [UnityEditor.MenuItem("Window/Unity Connect/Computer DidWakeUp", false, 0x3e8, true)]
        public static void TestComputerDidWakeUp()
        {
            instance.ComputerDidWakeUp();
        }

        [UnityEditor.MenuItem("Window/Unity Connect/Computer GoesToSleep", false, 0x3e8, true)]
        public static void TestComputerGoesToSleep()
        {
            instance.ComputerGoesToSleep();
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void UnbindCloudProject();
        public void UnbindProject()
        {
            this.UnbindCloudProject();
            UnityConnectServiceCollection.instance.UnbindAllServices();
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void UnhandledError(string request, int responseCode, string response);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void WorkOffline(bool rememberDecision);

        public bool canBuildWithUPID { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public string configuration { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public ConnectInfo connectInfo { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public static UnityConnect instance =>
            s_Instance;

        public int lastErrorCode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public string lastErrorMessage { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public bool loggedIn { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public bool online { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public static bool preferencesEnabled { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public ProjectInfo projectInfo { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public bool projectValid { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public bool shouldShowServicesWindow { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public static bool skipMissingUPID { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public UserInfo userInfo { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public bool workingOffline { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        [Flags]
        internal enum UnityErrorBehaviour
        {
            Alert,
            Automatic,
            Hidden,
            ConsoleOnly,
            Reconnect
        }

        [Flags]
        internal enum UnityErrorFilter
        {
            All = 7,
            ByChild = 4,
            ByContext = 1,
            ByParent = 2
        }

        [Flags]
        internal enum UnityErrorPriority
        {
            Critical,
            Error,
            Warning,
            Info,
            None
        }
    }
}

