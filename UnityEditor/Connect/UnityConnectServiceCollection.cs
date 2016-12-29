namespace UnityEditor.Connect
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Web;
    using UnityEditorInternal;
    using UnityEngine;

    internal class UnityConnectServiceCollection
    {
        [CompilerGenerated]
        private static Func<UnityConnectEditorWindow, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<UnityConnectServiceData, string> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<KeyValuePair<string, UnityConnectServiceData>, ServiceInfo> <>f__am$cache2;
        private const string kDrawerContainerTitle = "Services";
        private string m_CurrentPageName = "";
        private string m_CurrentServiceName = "";
        private readonly Dictionary<string, UnityConnectServiceData> m_Services = new Dictionary<string, UnityConnectServiceData>();
        private static UnityConnectServiceCollection s_UnityConnectEditor;
        private static UnityConnectEditorWindow s_UnityConnectEditorWindow;

        private UnityConnectServiceCollection()
        {
            UnityConnect.instance.StateChanged += new StateChangedDelegate(this.InstanceStateChanged);
        }

        public bool AddService(UnityConnectServiceData cloudService)
        {
            if (this.m_Services.ContainsKey(cloudService.serviceName))
            {
                return false;
            }
            this.m_Services[cloudService.serviceName] = cloudService;
            return true;
        }

        public void CloseServices()
        {
            if (s_UnityConnectEditorWindow != null)
            {
                s_UnityConnectEditorWindow.Close();
                s_UnityConnectEditorWindow = null;
            }
            UnityConnect.instance.ClearCache();
        }

        public void EnableService(string name, bool enabled)
        {
            if (this.m_Services.ContainsKey(name))
            {
                this.m_Services[name].EnableService(enabled);
            }
        }

        private void EnsureDrawerIsVisible(bool forceFocus)
        {
            if ((s_UnityConnectEditorWindow == null) || !s_UnityConnectEditorWindow.UrlsMatch(this.GetAllServiceUrls()))
            {
                string title = "Services";
                int serviceEnv = UnityConnectPrefs.GetServiceEnv(this.m_CurrentServiceName);
                if (serviceEnv != 0)
                {
                    title = title + " [" + UnityConnectPrefs.kEnvironmentFamilies[serviceEnv] + "]";
                }
                s_UnityConnectEditorWindow = UnityConnectEditorWindow.Create(title, this.GetAllServiceUrls());
                s_UnityConnectEditorWindow.ErrorUrl = this.m_Services["ErrorHub"].serviceUrl;
                s_UnityConnectEditorWindow.minSize = new Vector2(275f, 50f);
            }
            string serviceUrl = this.m_Services[this.m_CurrentServiceName].serviceUrl;
            if (this.m_CurrentPageName.Length > 0)
            {
                serviceUrl = serviceUrl + "/#/" + this.m_CurrentPageName;
            }
            s_UnityConnectEditorWindow.currentUrl = serviceUrl;
            s_UnityConnectEditorWindow.ShowTab();
            if (InternalEditorUtility.isApplicationActive && forceFocus)
            {
                s_UnityConnectEditorWindow.Focus();
            }
        }

        private string GetActualServiceName(string desiredServiceName, ConnectInfo state)
        {
            if (!state.online)
            {
                return "ErrorHub";
            }
            if (!state.ready)
            {
                return "Hub";
            }
            if (state.maintenance)
            {
                return "ErrorHub";
            }
            if (((desiredServiceName != "Hub") && state.online) && !state.loggedIn)
            {
                return "Hub";
            }
            if ((desiredServiceName == "ErrorHub") && state.online)
            {
                return "Hub";
            }
            if (string.IsNullOrEmpty(desiredServiceName))
            {
                return "Hub";
            }
            return desiredServiceName;
        }

        public ServiceInfo[] GetAllServiceInfos()
        {
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<KeyValuePair<string, UnityConnectServiceData>, ServiceInfo>(null, (IntPtr) <GetAllServiceInfos>m__2);
            }
            return Enumerable.Select<KeyValuePair<string, UnityConnectServiceData>, ServiceInfo>(this.m_Services, <>f__am$cache2).ToArray<ServiceInfo>();
        }

        public List<string> GetAllServiceNames() => 
            this.m_Services.Keys.ToList<string>();

        public List<string> GetAllServiceUrls()
        {
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<UnityConnectServiceData, string>(null, (IntPtr) <GetAllServiceUrls>m__1);
            }
            return Enumerable.Select<UnityConnectServiceData, string>(this.m_Services.Values, <>f__am$cache1).ToList<string>();
        }

        public UnityConnectServiceData GetServiceFromUrl(string searchUrl)
        {
            <GetServiceFromUrl>c__AnonStorey0 storey = new <GetServiceFromUrl>c__AnonStorey0 {
                searchUrl = searchUrl
            };
            return Enumerable.FirstOrDefault<KeyValuePair<string, UnityConnectServiceData>>(this.m_Services, new Func<KeyValuePair<string, UnityConnectServiceData>, bool>(storey, (IntPtr) this.<>m__0)).Value;
        }

        public string GetUrlForService(string serviceName) => 
            (!this.m_Services.ContainsKey(serviceName) ? string.Empty : this.m_Services[serviceName].serviceUrl);

        public WebView GetWebViewFromServiceName(string serviceName)
        {
            if ((s_UnityConnectEditorWindow == null) || !s_UnityConnectEditorWindow.UrlsMatch(this.GetAllServiceUrls()))
            {
                return null;
            }
            if (!this.m_Services.ContainsKey(serviceName))
            {
                return null;
            }
            ConnectInfo connectInfo = UnityConnect.instance.connectInfo;
            string actualServiceName = this.GetActualServiceName(serviceName, connectInfo);
            string serviceUrl = this.m_Services[actualServiceName].serviceUrl;
            return s_UnityConnectEditorWindow.GetWebViewFromURL(serviceUrl);
        }

        private void Init()
        {
            JSProxyMgr.GetInstance().AddGlobalObject("UnityConnectEditor", this);
        }

        protected void InstanceStateChanged(ConnectInfo state)
        {
            if (this.isDrawerOpen && state.ready)
            {
                string actualServiceName = this.GetActualServiceName(this.m_CurrentServiceName, state);
                if ((actualServiceName != this.m_CurrentServiceName) || ((s_UnityConnectEditorWindow != null) && (this.m_Services[actualServiceName].serviceUrl != s_UnityConnectEditorWindow.currentUrl)))
                {
                    bool forceFocus = ((s_UnityConnectEditorWindow != null) && (s_UnityConnectEditorWindow.webView != null)) && s_UnityConnectEditorWindow.webView.HasApplicationFocus();
                    this.ShowService(actualServiceName, forceFocus);
                }
            }
        }

        public void ReloadServices()
        {
            if (s_UnityConnectEditorWindow != null)
            {
                s_UnityConnectEditorWindow.Reload();
            }
        }

        public bool RemoveService(string serviceName)
        {
            if (!this.m_Services.ContainsKey(serviceName))
            {
                return false;
            }
            return this.m_Services.Remove(serviceName);
        }

        public bool ServiceExist(string serviceName) => 
            this.m_Services.ContainsKey(serviceName);

        public bool ShowService(string serviceName, bool forceFocus) => 
            this.ShowService(serviceName, "", forceFocus);

        public bool ShowService(string serviceName, string atPage, bool forceFocus)
        {
            if (!this.m_Services.ContainsKey(serviceName))
            {
                return false;
            }
            ConnectInfo connectInfo = UnityConnect.instance.connectInfo;
            this.m_CurrentServiceName = this.GetActualServiceName(serviceName, connectInfo);
            this.m_CurrentPageName = atPage;
            this.EnsureDrawerIsVisible(forceFocus);
            return true;
        }

        public static void StaticEnableService(string serviceName, bool enabled)
        {
            instance.EnableService(serviceName, enabled);
        }

        public void UnbindAllServices()
        {
            foreach (UnityConnectServiceData data in this.m_Services.Values)
            {
                data.OnProjectUnbound();
            }
        }

        public static UnityConnectServiceCollection instance
        {
            get
            {
                if (s_UnityConnectEditor == null)
                {
                    s_UnityConnectEditor = new UnityConnectServiceCollection();
                    s_UnityConnectEditor.Init();
                }
                return s_UnityConnectEditor;
            }
        }

        public bool isDrawerOpen
        {
            get
            {
                UnityConnectEditorWindow[] windowArray = Resources.FindObjectsOfTypeAll(typeof(UnityConnectEditorWindow)) as UnityConnectEditorWindow[];
                if (windowArray == null)
                {
                }
                return ((<>f__am$cache0 == null) && Enumerable.Any<UnityConnectEditorWindow>(windowArray, <>f__am$cache0));
            }
        }

        [CompilerGenerated]
        private sealed class <GetServiceFromUrl>c__AnonStorey0
        {
            internal string searchUrl;

            internal bool <>m__0(KeyValuePair<string, UnityConnectServiceData> kvp) => 
                (kvp.Value.serviceUrl == this.searchUrl);
        }

        public class ServiceInfo
        {
            public bool enabled;
            public string name;
            public string unityPath;
            public string url;

            public ServiceInfo(string name, string url, string unityPath, bool enabled)
            {
                this.name = name;
                this.url = url;
                this.unityPath = unityPath;
                this.enabled = enabled;
            }
        }
    }
}

