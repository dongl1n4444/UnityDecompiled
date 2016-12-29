namespace UnityEditor.Web
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal class WebScriptObject : ScriptableObject
    {
        private WebView m_WebView = null;

        private WebScriptObject()
        {
        }

        public bool processMessage(string jsonRequest, WebViewV8CallbackCSharp callback) => 
            this.ProcessMessage(jsonRequest, callback);

        public bool ProcessMessage(string jsonRequest, WebViewV8CallbackCSharp callback)
        {
            <ProcessMessage>c__AnonStorey0 storey = new <ProcessMessage>c__AnonStorey0 {
                callback = callback
            };
            return ((this.m_WebView != null) && JSProxyMgr.GetInstance().DoMessage(jsonRequest, new JSProxyMgr.ExecCallback(storey.<>m__0), this.m_WebView));
        }

        public WebView webView
        {
            get => 
                this.m_WebView;
            set
            {
                this.m_WebView = value;
            }
        }

        [CompilerGenerated]
        private sealed class <ProcessMessage>c__AnonStorey0
        {
            internal WebViewV8CallbackCSharp callback;

            internal void <>m__0(object result)
            {
                string str = JSProxyMgr.GetInstance().Stringify(result);
                this.callback.Callback(str);
            }
        }
    }
}

