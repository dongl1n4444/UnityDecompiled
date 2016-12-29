namespace UnityEditor
{
    using System;
    using UnityEngine;

    [InitializeOnLoad]
    internal class WebViewStatic : ScriptableSingleton<WebViewStatic>
    {
        [SerializeField]
        private WebView m_WebView;

        public static WebView GetWebView() => 
            ScriptableSingleton<WebViewStatic>.instance.m_WebView;

        public static void SetWebView(WebView webView)
        {
            ScriptableSingleton<WebViewStatic>.instance.m_WebView = webView;
        }
    }
}

