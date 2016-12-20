namespace UnityEditor.Web
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;

    internal class WebViewEditorWindowTabs : WebViewEditorWindow, IHasCustomMenu, ISerializationCallbackReceiver
    {
        protected object m_GlobalObject = null;
        [SerializeField]
        private List<WebView> m_RegisteredViewInstances = new List<WebView>();
        private Dictionary<string, WebView> m_RegisteredViews = new Dictionary<string, WebView>();
        [SerializeField]
        private List<string> m_RegisteredViewURLs = new List<string>();
        internal WebView m_WebView;

        protected WebViewEditorWindowTabs()
        {
            this.m_GlobalObject = null;
        }

        private bool FindWebView(string webViewUrl, out WebView webView)
        {
            webView = null;
            string key = MakeUrlKey(webViewUrl);
            return this.m_RegisteredViews.TryGetValue(key, out webView);
        }

        public WebView GetWebViewFromURL(string url)
        {
            string str = MakeUrlKey(url);
            return this.m_RegisteredViews[str];
        }

        public override void Init()
        {
            if ((this.m_GlobalObject == null) && !string.IsNullOrEmpty(base.m_GlobalObjectTypeName))
            {
                Type type = Type.GetType(base.m_GlobalObjectTypeName);
                if (type != null)
                {
                    this.m_GlobalObject = ScriptableObject.CreateInstance(type);
                    JSProxyMgr.GetInstance().AddGlobalObject(this.m_GlobalObject.GetType().Name, this.m_GlobalObject);
                }
            }
        }

        protected override void InitWebView(Rect webViewRect)
        {
            base.InitWebView(webViewRect);
            if ((base.m_InitialOpenURL != null) && (this.webView != null))
            {
                this.RegisterWebviewUrl(base.m_InitialOpenURL, this.webView);
            }
        }

        protected override void LoadPage()
        {
            if (this.webView != null)
            {
                WebView view;
                if (!this.FindWebView(base.m_InitialOpenURL, out view) || (view == null))
                {
                    base.NotifyVisibility(false);
                    this.webView.SetHostView(null);
                    this.webView = null;
                    Rect rect = GUIClip.Unclip(new Rect(0f, 0f, base.position.width, base.position.height));
                    this.InitWebView(rect);
                    this.RegisterWebviewUrl(base.m_InitialOpenURL, this.webView);
                    base.NotifyVisibility(true);
                }
                else
                {
                    if (view != this.webView)
                    {
                        base.NotifyVisibility(false);
                        view.SetHostView(base.m_Parent);
                        this.webView.SetHostView(null);
                        this.webView = view;
                        base.NotifyVisibility(true);
                        this.webView.Show();
                    }
                    base.LoadUri();
                }
            }
        }

        private static string MakeUrlKey(string webViewUrl)
        {
            string str;
            int index = webViewUrl.IndexOf("#");
            if (index != -1)
            {
                str = webViewUrl.Substring(0, index);
            }
            else
            {
                str = webViewUrl;
            }
            index = str.LastIndexOf("/");
            if (index == (str.Length - 1))
            {
                return str.Substring(0, index);
            }
            return str;
        }

        public void OnAfterDeserialize()
        {
            this.m_RegisteredViews = new Dictionary<string, WebView>();
            for (int i = 0; i != Math.Min(this.m_RegisteredViewURLs.Count, this.m_RegisteredViewInstances.Count); i++)
            {
                this.m_RegisteredViews.Add(this.m_RegisteredViewURLs[i], this.m_RegisteredViewInstances[i]);
            }
        }

        public void OnBeforeSerialize()
        {
            this.m_RegisteredViewURLs = new List<string>();
            this.m_RegisteredViewInstances = new List<WebView>();
            foreach (KeyValuePair<string, WebView> pair in this.m_RegisteredViews)
            {
                this.m_RegisteredViewURLs.Add(pair.Key);
                this.m_RegisteredViewInstances.Add(pair.Value);
            }
        }

        public override void OnDestroy()
        {
            if (this.webView != null)
            {
                Object.DestroyImmediate(this.webView);
            }
            this.m_GlobalObject = null;
            foreach (WebView view in this.m_RegisteredViews.Values)
            {
                if (view != null)
                {
                    Object.DestroyImmediate(view);
                }
            }
            this.m_RegisteredViews.Clear();
            this.m_RegisteredViewURLs.Clear();
            this.m_RegisteredViewInstances.Clear();
        }

        public override void OnInitScripting()
        {
            base.SetScriptObject();
        }

        private void RegisterWebviewUrl(string webViewUrl, WebView view)
        {
            string str = MakeUrlKey(webViewUrl);
            this.m_RegisteredViews[str] = view;
        }

        protected void UnregisterWebviewUrl(string webViewUrl)
        {
            string str = MakeUrlKey(webViewUrl);
            this.m_RegisteredViews[str] = null;
        }

        internal override WebView webView
        {
            get
            {
                return this.m_WebView;
            }
            set
            {
                this.m_WebView = value;
            }
        }
    }
}

