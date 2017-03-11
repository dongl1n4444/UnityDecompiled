namespace UnityEditor.Web
{
    using System;
    using System.IO;
    using System.Text;
    using System.Timers;
    using UnityEditor;
    using UnityEngine;

    internal abstract class WebViewEditorWindow : EditorWindow, IHasCustomMenu
    {
        private const int k_RepaintTimerDelay = 30;
        [SerializeField]
        protected string m_GlobalObjectTypeName;
        protected bool m_HasDelayedRefresh = false;
        [SerializeField]
        protected string m_InitialOpenURL;
        private Timer m_PostLoadTimer;
        protected bool m_SyncingFocus;
        internal WebScriptObject scriptObject;

        protected WebViewEditorWindow()
        {
        }

        public void About()
        {
            if (this.webView != null)
            {
                this.webView.LoadURL("chrome://version");
            }
        }

        public virtual void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Reload"), false, new GenericMenu.MenuFunction(this.Reload));
            if (Unsupported.IsDeveloperBuild())
            {
                menu.AddItem(new GUIContent("About"), false, new GenericMenu.MenuFunction(this.About));
            }
        }

        public static T Create<T>(string title, string sourcesPath, int minWidth, int minHeight, int maxWidth, int maxHeight) where T: WebViewEditorWindow
        {
            T window = ScriptableObject.CreateInstance<T>();
            window.m_GlobalObjectTypeName = typeof(T).FullName;
            CreateWindowCommon<T>(window, title, sourcesPath, minWidth, minHeight, maxWidth, maxHeight);
            window.Show();
            return window;
        }

        public static T CreateBase<T>(string title, string sourcesPath, int minWidth, int minHeight, int maxWidth, int maxHeight) where T: WebViewEditorWindow
        {
            T window = EditorWindow.GetWindow<T>(title);
            CreateWindowCommon<T>(window, title, sourcesPath, minWidth, minHeight, maxWidth, maxHeight);
            window.Show();
            return window;
        }

        private void CreateScriptObject()
        {
            if (this.scriptObject == null)
            {
                this.scriptObject = ScriptableObject.CreateInstance<WebScriptObject>();
                this.scriptObject.hideFlags = HideFlags.HideAndDontSave;
                this.scriptObject.webView = this.webView;
            }
        }

        public static T CreateUtility<T>(string title, string sourcesPath, int minWidth, int minHeight, int maxWidth, int maxHeight) where T: WebViewEditorWindow
        {
            T window = ScriptableObject.CreateInstance<T>();
            window.m_GlobalObjectTypeName = typeof(T).FullName;
            CreateWindowCommon<T>(window, title, sourcesPath, minWidth, minHeight, maxWidth, maxHeight);
            window.ShowUtility();
            return window;
        }

        private static void CreateWindowCommon<T>(T window, string title, string sourcesPath, int minWidth, int minHeight, int maxWidth, int maxHeight) where T: WebViewEditorWindow
        {
            window.titleContent = new GUIContent(title);
            window.minSize = new Vector2((float) minWidth, (float) minHeight);
            window.maxSize = new Vector2((float) maxWidth, (float) maxHeight);
            window.m_InitialOpenURL = sourcesPath;
            window.Init();
        }

        private void DoPostLoadTask()
        {
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.DoPostLoadTask));
            base.RepaintImmediately();
        }

        public virtual void Init()
        {
        }

        protected virtual void InitWebView(Rect m_WebViewRect)
        {
            if (this.webView == null)
            {
                int x = (int) m_WebViewRect.x;
                int y = (int) m_WebViewRect.y;
                int width = (int) m_WebViewRect.width;
                int height = (int) m_WebViewRect.height;
                this.webView = ScriptableObject.CreateInstance<WebView>();
                this.webView.InitWebView(base.m_Parent, x, y, width, height, false);
                this.webView.hideFlags = HideFlags.HideAndDontSave;
                this.SetFocus(base.hasFocus);
            }
            this.webView.SetDelegateObject(this);
            this.LoadUri();
            this.SetFocus(true);
        }

        private void InvokeJSMethod(string objectName, string name, params object[] args)
        {
            if (this.webView != null)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(objectName);
                builder.Append('.');
                builder.Append(name);
                builder.Append('(');
                bool flag = true;
                foreach (object obj2 in args)
                {
                    if (!flag)
                    {
                        builder.Append(',');
                    }
                    bool flag2 = obj2 is string;
                    if (flag2)
                    {
                        builder.Append('"');
                    }
                    builder.Append(obj2);
                    if (flag2)
                    {
                        builder.Append('"');
                    }
                    flag = false;
                }
                builder.Append(");");
                this.webView.ExecuteJavascript(builder.ToString());
            }
        }

        protected virtual void LoadPage()
        {
            if (this.webView != null)
            {
                this.NotifyVisibility(false);
                this.LoadUri();
                this.webView.Show();
            }
        }

        protected void LoadUri()
        {
            if (this.m_InitialOpenURL.StartsWith("http"))
            {
                this.webView.LoadURL(this.m_InitialOpenURL);
                this.m_PostLoadTimer = new Timer(30.0);
                this.m_PostLoadTimer.Elapsed += new ElapsedEventHandler(this.RaisePostLoadCondition);
                this.m_PostLoadTimer.Enabled = true;
            }
            else if (this.m_InitialOpenURL.StartsWith("file"))
            {
                this.webView.LoadFile(this.m_InitialOpenURL);
            }
            else
            {
                string path = Path.Combine(Uri.EscapeUriString(Path.Combine(EditorApplication.applicationContentsPath, "Resources")), this.m_InitialOpenURL);
                this.webView.LoadFile(path);
            }
        }

        protected void NotifyVisibility(bool visible)
        {
            if (this.webView != null)
            {
                string scriptCode = "document.dispatchEvent(new CustomEvent('showWebView',{ detail: { visible:";
                scriptCode = scriptCode + (!visible ? "false" : "true") + "}, bubbles: true, cancelable: false }));";
                this.webView.ExecuteJavascript(scriptCode);
            }
        }

        public void OnBatchMode()
        {
            Rect rect = GUIClip.Unclip(new Rect(0f, 0f, base.position.width, base.position.height));
            if ((this.m_InitialOpenURL != null) && (this.webView == null))
            {
                this.InitWebView(rect);
            }
        }

        public void OnBecameInvisible()
        {
            if (this.webView != null)
            {
                this.webView.SetHostView(null);
            }
        }

        public virtual void OnDestroy()
        {
            if (this.webView != null)
            {
                UnityEngine.Object.DestroyImmediate(this.webView);
            }
        }

        public virtual void OnEnable()
        {
            this.Init();
        }

        public void OnFocus()
        {
            this.SetFocus(true);
        }

        public void OnGUI()
        {
            Rect screenRect = GUIClip.Unclip(new Rect(0f, 0f, base.position.width, base.position.height));
            GUILayout.BeginArea(screenRect);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUILayout.Label("Loading...", EditorStyles.label, new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            if ((Event.current.type == EventType.Repaint) && this.m_HasDelayedRefresh)
            {
                this.Refresh();
                this.m_HasDelayedRefresh = false;
            }
            if (this.m_InitialOpenURL != null)
            {
                if (this.webView == null)
                {
                    this.InitWebView(screenRect);
                }
                if (Event.current.type == EventType.Repaint)
                {
                    this.webView.SetSizeAndPosition((int) screenRect.x, (int) screenRect.y, (int) screenRect.width, (int) screenRect.height);
                }
            }
        }

        public virtual void OnInitScripting()
        {
            this.SetScriptObject();
        }

        public void OnLoadError(string url)
        {
            if (this.webView == null)
            {
            }
        }

        public void OnLostFocus()
        {
            this.SetFocus(false);
        }

        private void RaisePostLoadCondition(object obj, ElapsedEventArgs args)
        {
            this.m_PostLoadTimer.Stop();
            this.m_PostLoadTimer = null;
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.DoPostLoadTask));
        }

        public void Refresh()
        {
            if (this.webView != null)
            {
                this.webView.Hide();
                this.webView.Show();
            }
        }

        public void Reload()
        {
            if (this.webView != null)
            {
                this.webView.Reload();
            }
        }

        private void SetFocus(bool value)
        {
            if (!this.m_SyncingFocus)
            {
                this.m_SyncingFocus = true;
                if (this.webView != null)
                {
                    if (value)
                    {
                        this.webView.SetHostView(base.m_Parent);
                        if (Application.platform != RuntimePlatform.WindowsEditor)
                        {
                            this.m_HasDelayedRefresh = true;
                        }
                        else
                        {
                            this.webView.Show();
                        }
                    }
                    this.webView.SetApplicationFocus(((base.m_Parent != null) && base.m_Parent.hasFocus) && base.hasFocus);
                    this.webView.SetFocus(value);
                }
                this.m_SyncingFocus = false;
            }
        }

        protected void SetScriptObject()
        {
            if (this.webView != null)
            {
                this.CreateScriptObject();
                this.webView.DefineScriptObject("window.webScriptObject", this.scriptObject);
            }
        }

        public void ToggleMaximize()
        {
            base.maximized = !base.maximized;
            this.Refresh();
            this.SetFocus(true);
        }

        public string initialOpenUrl
        {
            get => 
                this.m_InitialOpenURL;
            set
            {
                this.m_InitialOpenURL = value;
            }
        }

        internal abstract WebView webView { get; set; }
    }
}

