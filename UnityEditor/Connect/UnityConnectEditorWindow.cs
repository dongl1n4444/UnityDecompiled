namespace UnityEditor.Connect
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Web;
    using UnityEngine;

    [Serializable]
    internal class UnityConnectEditorWindow : WebViewEditorWindowTabs
    {
        [CompilerGenerated]
        private static Func<UnityConnectEditorWindow, bool> <>f__am$cache0;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string <ErrorUrl>k__BackingField;
        private bool m_ClearInitialOpenURL = true;
        private List<string> m_ServiceUrls = new List<string>();

        protected UnityConnectEditorWindow()
        {
        }

        public static UnityConnectEditorWindow Create(string title, List<string> serviceUrls)
        {
            UnityConnectEditorWindow[] windowArray = Resources.FindObjectsOfTypeAll(typeof(UnityConnectEditorWindow)) as UnityConnectEditorWindow[];
            if (windowArray != null)
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = new Func<UnityConnectEditorWindow, bool>(null, (IntPtr) <Create>m__0);
                }
                foreach (UnityConnectEditorWindow window in Enumerable.Where<UnityConnectEditorWindow>(windowArray, <>f__am$cache0))
                {
                    window.titleContent = new GUIContent(title);
                    return window;
                }
            }
            Type[] desiredDockNextTo = new Type[] { typeof(InspectorWindow) };
            UnityConnectEditorWindow window3 = EditorWindow.GetWindow<UnityConnectEditorWindow>(title, desiredDockNextTo);
            window3.m_ClearInitialOpenURL = false;
            window3.initialOpenUrl = serviceUrls[0];
            window3.Init();
            return window3;
        }

        public void OnEnable()
        {
            this.m_ServiceUrls = UnityConnectServiceCollection.instance.GetAllServiceUrls();
            base.OnEnable();
        }

        public void OnGUI()
        {
            if (this.m_ClearInitialOpenURL)
            {
                this.m_ClearInitialOpenURL = false;
                base.m_InitialOpenURL = (this.m_ServiceUrls.Count <= 0) ? null : UnityConnectServiceCollection.instance.GetUrlForService("Hub");
            }
            base.OnGUI();
        }

        public void OnInitScripting()
        {
            base.OnInitScripting();
        }

        public void OnLoadError(string url)
        {
            if (this.webView != null)
            {
                this.webView.LoadFile(EditorApplication.userJavascriptPackagesPath + "unityeditor-cloud-hub/dist/index.html?failure=load_error&reload_url=" + WWW.EscapeURL(url));
                if (url.StartsWith("http://") || url.StartsWith("https://"))
                {
                    base.UnregisterWebviewUrl(url);
                }
            }
        }

        public void ToggleMaximize()
        {
            base.ToggleMaximize();
        }

        public bool UrlsMatch(List<string> referenceUrls)
        {
            <UrlsMatch>c__AnonStorey0 storey = new <UrlsMatch>c__AnonStorey0 {
                referenceUrls = referenceUrls
            };
            if (this.m_ServiceUrls.Count != storey.referenceUrls.Count)
            {
                return false;
            }
            return !Enumerable.Where<string>(this.m_ServiceUrls, new Func<string, int, bool>(storey, (IntPtr) this.<>m__0)).Any<string>();
        }

        public string currentUrl
        {
            get => 
                base.m_InitialOpenURL;
            set
            {
                base.m_InitialOpenURL = value;
                this.LoadPage();
            }
        }

        public string ErrorUrl { get; set; }

        [CompilerGenerated]
        private sealed class <UrlsMatch>c__AnonStorey0
        {
            internal List<string> referenceUrls;

            internal bool <>m__0(string t, int idx) => 
                (t != this.referenceUrls[idx]);
        }
    }
}

