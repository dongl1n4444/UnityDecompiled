namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor.Collaboration;
    using UnityEditor.Connect;
    using UnityEditor.Web;
    using UnityEngine;

    internal class CollabToolbarWindow : WebViewEditorStaticWindow, IHasCustomMenu
    {
        [CompilerGenerated]
        private static EditorApplication.CallbackFunction <>f__mg$cache0;
        private const int kWindowHeight = 350;
        private const string kWindowName = "Unity Collab Toolbar";
        private const int kWindowWidth = 320;
        private static CollabToolbarWindow s_CollabToolbarWindow;
        private static long s_LastClosedTime;
        public static bool s_ToolbarIsVisible = false;

        public static void CloseToolbarWindows()
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new EditorApplication.CallbackFunction(CollabToolbarWindow.CloseToolbarWindowsImmediately);
            }
            EditorApplication.CallDelayed(<>f__mg$cache0, 1f);
        }

        public static void CloseToolbarWindowsImmediately()
        {
            foreach (CollabToolbarWindow window in UnityEngine.Resources.FindObjectsOfTypeAll<CollabToolbarWindow>())
            {
                window.Close();
            }
        }

        public void OnDestroy()
        {
            base.OnDestroy();
        }

        internal void OnDisable()
        {
            s_LastClosedTime = DateTime.Now.Ticks / 0x2710L;
            s_CollabToolbarWindow = null;
        }

        public override void OnEnable()
        {
            base.minSize = new Vector2(320f, 350f);
            base.maxSize = new Vector2(320f, 350f);
            base.initialOpenUrl = "file:///" + EditorApplication.userJavascriptPackagesPath + "unityeditor-collab-toolbar/dist/index.html";
            base.OnEnable();
        }

        public void OnFocus()
        {
            base.OnFocus();
            EditorApplication.LockReloadAssemblies();
            s_ToolbarIsVisible = true;
        }

        public void OnInitScripting()
        {
            base.OnInitScripting();
        }

        public void OnLostFocus()
        {
            base.OnLostFocus();
            EditorApplication.UnlockReloadAssemblies();
            s_ToolbarIsVisible = false;
        }

        public void OnReceiveTitle(string title)
        {
            base.titleContent.text = title;
        }

        internal static bool ShowCenteredAtPosition(Rect buttonRect)
        {
            buttonRect.x -= 160f;
            long num = DateTime.Now.Ticks / 0x2710L;
            if (num >= (s_LastClosedTime + 50L))
            {
                if (Event.current.type != EventType.Layout)
                {
                    Event.current.Use();
                }
                if (s_CollabToolbarWindow == null)
                {
                    s_CollabToolbarWindow = ScriptableObject.CreateInstance<CollabToolbarWindow>();
                }
                buttonRect = GUIUtility.GUIToScreenRect(buttonRect);
                Vector2 windowSize = new Vector2(320f, 350f);
                s_CollabToolbarWindow.initialOpenUrl = "file:///" + EditorApplication.userJavascriptPackagesPath + "unityeditor-collab-toolbar/dist/index.html";
                s_CollabToolbarWindow.Init();
                s_CollabToolbarWindow.ShowAsDropDown(buttonRect, windowSize);
                return true;
            }
            return false;
        }

        [UnityEditor.MenuItem("Window/Collab Toolbar", false, 0x7db, true)]
        public static CollabToolbarWindow ShowToolbarWindow() => 
            EditorWindow.GetWindow<CollabToolbarWindow>(false, "Unity Collab Toolbar");

        [UnityEditor.MenuItem("Window/Collab Toolbar", true)]
        public static bool ValidateShowToolbarWindow() => 
            (UnityConnect.instance.userInfo.whitelisted && Collab.instance.collabInfo.whitelisted);

        internal override WebView webView
        {
            get => 
                WebViewStatic.GetWebView();
            set
            {
                WebViewStatic.SetWebView(value);
            }
        }
    }
}

