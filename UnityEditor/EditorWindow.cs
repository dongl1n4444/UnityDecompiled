namespace UnityEditor
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Derive from this class to create an editor window.</para>
    /// </summary>
    [UsedByNativeCode]
    public class EditorWindow : ScriptableObject
    {
        private const double kWarningFadeoutTime = 1.0;
        private const double kWarningFadeoutWait = 4.0;
        [HideInInspector, SerializeField]
        private bool m_AutoRepaintOnSceneChange;
        [HideInInspector, SerializeField]
        private int m_DepthBufferBits = 0;
        private bool m_DontClearBackground;
        internal float m_FadeoutTime = 0f;
        private Rect m_GameViewClippedRect;
        private Rect m_GameViewRect;
        private Vector2 m_GameViewTargetSize;
        [SerializeField, HideInInspector]
        private Vector2 m_MaxSize = new Vector2(4000f, 4000f);
        [SerializeField, HideInInspector]
        private Vector2 m_MinSize = new Vector2(100f, 100f);
        internal GUIContent m_Notification = null;
        private Vector2 m_NotificationSize;
        [NonSerialized]
        internal HostView m_Parent;
        [HideInInspector, SerializeField]
        internal Rect m_Pos = new Rect(0f, 0f, 320f, 240f);
        [SerializeField, HideInInspector]
        internal GUIContent m_TitleContent;
        private bool m_WantsMouseEnterLeaveWindow;
        private bool m_WantsMouseMove;

        public EditorWindow()
        {
            this.titleContent.text = base.GetType().ToString();
        }

        private void __internalAwake()
        {
            base.hideFlags = HideFlags.DontSave;
        }

        [ContextMenu("Add Game")]
        internal void AddGameTab()
        {
        }

        [ContextMenu("Add Scene")]
        internal void AddSceneTab()
        {
        }

        /// <summary>
        /// <para>Mark the beginning area of all popup windows.</para>
        /// </summary>
        public void BeginWindows()
        {
            EditorGUIInternal.BeginWindowsForward(1, base.GetInstanceID());
        }

        internal void CheckForWindowRepaint()
        {
            double timeSinceStartup = EditorApplication.timeSinceStartup;
            if (timeSinceStartup >= this.m_FadeoutTime)
            {
                if (timeSinceStartup > (this.m_FadeoutTime + 1.0))
                {
                    this.RemoveNotification();
                }
                else
                {
                    this.Repaint();
                }
            }
        }

        /// <summary>
        /// <para>Close the editor window.</para>
        /// </summary>
        public void Close()
        {
            if (WindowLayout.IsMaximized(this))
            {
                WindowLayout.Unmaximize(this);
            }
            DockArea parent = this.m_Parent as DockArea;
            if (parent != null)
            {
                parent.RemoveTab(this, true);
            }
            else
            {
                this.m_Parent.window.Close();
            }
            UnityEngine.Object.DestroyImmediate(this, true);
        }

        internal static void CreateNewWindowForEditorWindow(EditorWindow window, bool loadPosition, bool showImmediately)
        {
            CreateNewWindowForEditorWindow(window, new Vector2(window.position.x, window.position.y), loadPosition, showImmediately);
        }

        internal static void CreateNewWindowForEditorWindow(EditorWindow window, Vector2 screenPosition, bool loadPosition, bool showImmediately)
        {
            ContainerWindow window2 = ScriptableObject.CreateInstance<ContainerWindow>();
            SplitView view = ScriptableObject.CreateInstance<SplitView>();
            window2.rootView = view;
            DockArea child = ScriptableObject.CreateInstance<DockArea>();
            view.AddChild(child);
            child.AddTab(window);
            Rect rect = window.m_Parent.borderSize.Add(new Rect(screenPosition.x, screenPosition.y, window.position.width, window.position.height));
            window2.position = rect;
            view.position = new Rect(0f, 0f, rect.width, rect.height);
            window.MakeParentsSettingsMatchMe();
            window2.Show(ShowMode.NormalWindow, loadPosition, showImmediately);
            window2.OnResize();
        }

        internal void DrawNotification()
        {
            EditorStyles.notificationText.CalcMinMaxWidth(this.m_Notification, out this.m_NotificationSize.y, out this.m_NotificationSize.x);
            this.m_NotificationSize.y = EditorStyles.notificationText.CalcHeight(this.m_Notification, this.m_NotificationSize.x);
            Vector2 notificationSize = this.m_NotificationSize;
            float num = this.position.width - EditorStyles.notificationText.margin.horizontal;
            float num2 = (this.position.height - EditorStyles.notificationText.margin.vertical) - 20f;
            if (num < this.m_NotificationSize.x)
            {
                float num3 = num / this.m_NotificationSize.x;
                notificationSize.x *= num3;
                notificationSize.y = EditorStyles.notificationText.CalcHeight(this.m_Notification, notificationSize.x);
            }
            if (notificationSize.y > num2)
            {
                notificationSize.y = num2;
            }
            Rect position = new Rect((this.position.width - notificationSize.x) * 0.5f, 20f + (((this.position.height - 20f) - notificationSize.y) * 0.7f), notificationSize.x, notificationSize.y);
            double timeSinceStartup = EditorApplication.timeSinceStartup;
            if (timeSinceStartup > this.m_FadeoutTime)
            {
                GUI.color = new Color(1f, 1f, 1f, 1f - ((float) ((timeSinceStartup - this.m_FadeoutTime) / 1.0)));
            }
            GUI.Label(position, GUIContent.none, EditorStyles.notificationBackground);
            EditorGUI.DoDropShadowLabel(position, this.m_Notification, EditorStyles.notificationText, 0.3f);
        }

        /// <summary>
        /// <para>Close a window group started with EditorWindow.BeginWindows.</para>
        /// </summary>
        public void EndWindows()
        {
            GUI.EndWindows();
        }

        /// <summary>
        /// <para>Moves keyboard focus to this EditorWindow.</para>
        /// </summary>
        public void Focus()
        {
            if (this.m_Parent != null)
            {
                this.ShowTab();
                this.m_Parent.Focus();
            }
        }

        public static void FocusWindowIfItsOpen<T>() where T: EditorWindow
        {
            FocusWindowIfItsOpen(typeof(T));
        }

        /// <summary>
        /// <para>Focuses the first found EditorWindow of specified type if it is open.</para>
        /// </summary>
        /// <param name="t">The type of the window. Must derive from EditorWindow.</param>
        public static void FocusWindowIfItsOpen(System.Type t)
        {
            UnityEngine.Object[] objArray = UnityEngine.Resources.FindObjectsOfTypeAll(t);
            EditorWindow window = (objArray.Length <= 0) ? null : (objArray[0] as EditorWindow);
            if (window != null)
            {
                window.Focus();
            }
        }

        private static EditorWindowTitleAttribute GetEditorWindowTitleAttribute(System.Type t)
        {
            object[] customAttributes = t.GetCustomAttributes(true);
            foreach (object obj2 in customAttributes)
            {
                Attribute attribute = (Attribute) obj2;
                if (attribute.TypeId == typeof(EditorWindowTitleAttribute))
                {
                    return (EditorWindowTitleAttribute) obj2;
                }
            }
            return null;
        }

        internal GUIContent GetLocalizedTitleContent() => 
            GetLocalizedTitleContentFromType(base.GetType());

        internal static GUIContent GetLocalizedTitleContentFromType(System.Type t)
        {
            EditorWindowTitleAttribute editorWindowTitleAttribute = GetEditorWindowTitleAttribute(t);
            if (editorWindowTitleAttribute != null)
            {
                string icon = "";
                if (!string.IsNullOrEmpty(editorWindowTitleAttribute.icon))
                {
                    icon = editorWindowTitleAttribute.icon;
                }
                else if (editorWindowTitleAttribute.useTypeNameAsIconName)
                {
                    icon = t.ToString();
                }
                if (!string.IsNullOrEmpty(icon))
                {
                    return EditorGUIUtility.TextContentWithIcon(editorWindowTitleAttribute.title, icon);
                }
                return EditorGUIUtility.TextContent(editorWindowTitleAttribute.title);
            }
            return new GUIContent(t.ToString());
        }

        internal int GetNumTabs()
        {
            DockArea parent = this.m_Parent as DockArea;
            if (parent != null)
            {
                return parent.m_Panes.Count;
            }
            return 0;
        }

        public static T GetWindow<T>() where T: EditorWindow => 
            GetWindow<T>(false, null, true);

        public static T GetWindow<T>(bool utility) where T: EditorWindow => 
            GetWindow<T>(utility, null, true);

        public static T GetWindow<T>(string title) where T: EditorWindow => 
            GetWindow<T>(title, true);

        /// <summary>
        /// <para>Returns the first EditorWindow of type t which is currently on the screen.</para>
        /// </summary>
        /// <param name="t">The type of the window. Must derive from EditorWindow.</param>
        /// <param name="utility">Set this to true, to create a floating utility window, false to create a normal window.</param>
        /// <param name="title">If GetWindow creates a new window, it will get this title. If this value is null, use the class name as title.</param>
        /// <param name="focus">Whether to give the window focus, if it already exists. (If GetWindow creates a new window, it will always get focus).</param>
        [ExcludeFromDocs]
        public static EditorWindow GetWindow(System.Type t)
        {
            bool focus = true;
            string title = null;
            bool utility = false;
            return GetWindow(t, utility, title, focus);
        }

        public static T GetWindow<T>(params System.Type[] desiredDockNextTo) where T: EditorWindow => 
            GetWindow<T>(null, true, desiredDockNextTo);

        public static T GetWindow<T>(bool utility, string title) where T: EditorWindow => 
            GetWindow<T>(utility, title, true);

        public static T GetWindow<T>(string title, bool focus) where T: EditorWindow => 
            GetWindow<T>(false, title, focus);

        public static T GetWindow<T>(string title, params System.Type[] desiredDockNextTo) where T: EditorWindow => 
            GetWindow<T>(title, true, desiredDockNextTo);

        /// <summary>
        /// <para>Returns the first EditorWindow of type t which is currently on the screen.</para>
        /// </summary>
        /// <param name="t">The type of the window. Must derive from EditorWindow.</param>
        /// <param name="utility">Set this to true, to create a floating utility window, false to create a normal window.</param>
        /// <param name="title">If GetWindow creates a new window, it will get this title. If this value is null, use the class name as title.</param>
        /// <param name="focus">Whether to give the window focus, if it already exists. (If GetWindow creates a new window, it will always get focus).</param>
        [ExcludeFromDocs]
        public static EditorWindow GetWindow(System.Type t, bool utility)
        {
            bool focus = true;
            string title = null;
            return GetWindow(t, utility, title, focus);
        }

        public static T GetWindow<T>(bool utility, string title, bool focus) where T: EditorWindow => 
            (GetWindow(typeof(T), utility, title, focus) as T);

        public static T GetWindow<T>(string title, bool focus, params System.Type[] desiredDockNextTo) where T: EditorWindow
        {
            T[] localArray = UnityEngine.Resources.FindObjectsOfTypeAll(typeof(T)) as T[];
            T pane = (localArray.Length <= 0) ? null : localArray[0];
            if (pane != null)
            {
                if (focus)
                {
                    pane.Focus();
                }
                return pane;
            }
            pane = ScriptableObject.CreateInstance<T>();
            if (title != null)
            {
                pane.titleContent = new GUIContent(title);
            }
            System.Type[] typeArray = desiredDockNextTo;
            for (int i = 0; i < typeArray.Length; i++)
            {
                <GetWindow>c__AnonStorey0<T> storey = new <GetWindow>c__AnonStorey0<T> {
                    desired = typeArray[i]
                };
                ContainerWindow[] windows = ContainerWindow.windows;
                foreach (ContainerWindow window in windows)
                {
                    foreach (View view in window.rootView.allChildren)
                    {
                        DockArea area = view as DockArea;
                        if ((area != null) && Enumerable.Any<EditorWindow>(area.m_Panes, new Func<EditorWindow, bool>(storey.<>m__0)))
                        {
                            area.AddTab(pane);
                            return pane;
                        }
                    }
                }
            }
            pane.Show();
            return pane;
        }

        /// <summary>
        /// <para>Returns the first EditorWindow of type t which is currently on the screen.</para>
        /// </summary>
        /// <param name="t">The type of the window. Must derive from EditorWindow.</param>
        /// <param name="utility">Set this to true, to create a floating utility window, false to create a normal window.</param>
        /// <param name="title">If GetWindow creates a new window, it will get this title. If this value is null, use the class name as title.</param>
        /// <param name="focus">Whether to give the window focus, if it already exists. (If GetWindow creates a new window, it will always get focus).</param>
        [ExcludeFromDocs]
        public static EditorWindow GetWindow(System.Type t, bool utility, string title)
        {
            bool focus = true;
            return GetWindow(t, utility, title, focus);
        }

        /// <summary>
        /// <para>Returns the first EditorWindow of type t which is currently on the screen.</para>
        /// </summary>
        /// <param name="t">The type of the window. Must derive from EditorWindow.</param>
        /// <param name="utility">Set this to true, to create a floating utility window, false to create a normal window.</param>
        /// <param name="title">If GetWindow creates a new window, it will get this title. If this value is null, use the class name as title.</param>
        /// <param name="focus">Whether to give the window focus, if it already exists. (If GetWindow creates a new window, it will always get focus).</param>
        public static EditorWindow GetWindow(System.Type t, [DefaultValue("false")] bool utility, [DefaultValue("null")] string title, [DefaultValue("true")] bool focus) => 
            GetWindowPrivate(t, utility, title, focus);

        internal static T GetWindowDontShow<T>() where T: EditorWindow
        {
            UnityEngine.Object[] objArray = UnityEngine.Resources.FindObjectsOfTypeAll(typeof(T));
            return ((objArray.Length <= 0) ? ScriptableObject.CreateInstance<T>() : ((T) objArray[0]));
        }

        private static EditorWindow GetWindowPrivate(System.Type t, bool utility, string title, bool focus)
        {
            UnityEngine.Object[] objArray = UnityEngine.Resources.FindObjectsOfTypeAll(t);
            EditorWindow window = (objArray.Length <= 0) ? null : ((EditorWindow) objArray[0]);
            if (window == null)
            {
                window = ScriptableObject.CreateInstance(t) as EditorWindow;
                if (title != null)
                {
                    window.titleContent = new GUIContent(title);
                }
                if (utility)
                {
                    window.ShowUtility();
                    return window;
                }
                window.Show();
                return window;
            }
            if (focus)
            {
                window.Show();
                window.Focus();
            }
            return window;
        }

        public static T GetWindowWithRect<T>(Rect rect) where T: EditorWindow => 
            GetWindowWithRect<T>(rect, false, null, true);

        /// <summary>
        /// <para>Returns the first EditorWindow of type t which is currently on the screen.</para>
        /// </summary>
        /// <param name="t">The type of the window. Must derive from EditorWindow.</param>
        /// <param name="rect">The position on the screen where a newly created window will show.</param>
        /// <param name="utility">Set this to true, to create a floating utility window, false to create a normal window.</param>
        /// <param name="title">If GetWindow creates a new window, it will get this title. If this value is null, use the class name as title.</param>
        [ExcludeFromDocs]
        public static EditorWindow GetWindowWithRect(System.Type t, Rect rect)
        {
            string title = null;
            bool utility = false;
            return GetWindowWithRect(t, rect, utility, title);
        }

        public static T GetWindowWithRect<T>(Rect rect, bool utility) where T: EditorWindow => 
            GetWindowWithRect<T>(rect, utility, null, true);

        /// <summary>
        /// <para>Returns the first EditorWindow of type t which is currently on the screen.</para>
        /// </summary>
        /// <param name="t">The type of the window. Must derive from EditorWindow.</param>
        /// <param name="rect">The position on the screen where a newly created window will show.</param>
        /// <param name="utility">Set this to true, to create a floating utility window, false to create a normal window.</param>
        /// <param name="title">If GetWindow creates a new window, it will get this title. If this value is null, use the class name as title.</param>
        [ExcludeFromDocs]
        public static EditorWindow GetWindowWithRect(System.Type t, Rect rect, bool utility)
        {
            string title = null;
            return GetWindowWithRect(t, rect, utility, title);
        }

        public static T GetWindowWithRect<T>(Rect rect, bool utility, string title) where T: EditorWindow => 
            GetWindowWithRect<T>(rect, utility, title, true);

        /// <summary>
        /// <para>Returns the first EditorWindow of type t which is currently on the screen.</para>
        /// </summary>
        /// <param name="t">The type of the window. Must derive from EditorWindow.</param>
        /// <param name="rect">The position on the screen where a newly created window will show.</param>
        /// <param name="utility">Set this to true, to create a floating utility window, false to create a normal window.</param>
        /// <param name="title">If GetWindow creates a new window, it will get this title. If this value is null, use the class name as title.</param>
        public static EditorWindow GetWindowWithRect(System.Type t, Rect rect, [DefaultValue("false")] bool utility, [DefaultValue("null")] string title) => 
            GetWindowWithRectPrivate(t, rect, utility, title);

        public static T GetWindowWithRect<T>(Rect rect, bool utility, string title, bool focus) where T: EditorWindow
        {
            T local;
            UnityEngine.Object[] objArray = UnityEngine.Resources.FindObjectsOfTypeAll(typeof(T));
            if (objArray.Length > 0)
            {
                local = (T) objArray[0];
                if (focus)
                {
                    local.Focus();
                }
                return local;
            }
            local = ScriptableObject.CreateInstance<T>();
            local.minSize = new Vector2(rect.width, rect.height);
            local.maxSize = new Vector2(rect.width, rect.height);
            local.position = rect;
            if (title != null)
            {
                local.titleContent = new GUIContent(title);
            }
            if (utility)
            {
                local.ShowUtility();
                return local;
            }
            local.Show();
            return local;
        }

        private static EditorWindow GetWindowWithRectPrivate(System.Type t, Rect rect, bool utility, string title)
        {
            UnityEngine.Object[] objArray = UnityEngine.Resources.FindObjectsOfTypeAll(t);
            EditorWindow window = (objArray.Length <= 0) ? null : ((EditorWindow) objArray[0]);
            if (window == null)
            {
                window = ScriptableObject.CreateInstance(t) as EditorWindow;
                window.minSize = new Vector2(rect.width, rect.height);
                window.maxSize = new Vector2(rect.width, rect.height);
                window.position = rect;
                if (title != null)
                {
                    window.titleContent = new GUIContent(title);
                }
                if (utility)
                {
                    window.ShowUtility();
                    return window;
                }
                window.Show();
                return window;
            }
            window.Focus();
            return window;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void MakeModal(ContainerWindow win);
        internal void MakeParentsSettingsMatchMe()
        {
            if ((this.m_Parent != null) && (this.m_Parent.actualView == this))
            {
                this.m_Parent.SetTitle(base.GetType().FullName);
                this.m_Parent.autoRepaintOnSceneChange = this.m_AutoRepaintOnSceneChange;
                bool flag = this.m_Parent.depthBufferBits != this.m_DepthBufferBits;
                this.m_Parent.depthBufferBits = this.m_DepthBufferBits;
                this.m_Parent.SetInternalGameViewDimensions(this.m_GameViewRect, this.m_GameViewClippedRect, this.m_GameViewTargetSize);
                this.m_Parent.wantsMouseMove = this.m_WantsMouseMove;
                this.m_Parent.wantsMouseEnterLeaveWindow = this.m_WantsMouseEnterLeaveWindow;
                Vector2 vector = new Vector2((float) (this.m_Parent.borderSize.left + this.m_Parent.borderSize.right), (float) (this.m_Parent.borderSize.top + this.m_Parent.borderSize.bottom));
                this.m_Parent.SetMinMaxSizes(this.minSize + vector, this.maxSize + vector);
                if (flag)
                {
                    this.m_Parent.RecreateContext();
                }
            }
        }

        internal virtual void OnResized()
        {
        }

        internal void RemoveFromDockArea()
        {
            DockArea parent = this.m_Parent as DockArea;
            if (parent != null)
            {
                parent.RemoveTab(this, true);
            }
        }

        /// <summary>
        /// <para>Stop showing notification message.</para>
        /// </summary>
        public void RemoveNotification()
        {
            if (this.m_FadeoutTime != 0f)
            {
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.CheckForWindowRepaint));
                this.m_Notification = null;
                this.m_FadeoutTime = 0f;
            }
        }

        /// <summary>
        /// <para>Make the window repaint.</para>
        /// </summary>
        public void Repaint()
        {
            if ((this.m_Parent != null) && (this.m_Parent.actualView == this))
            {
                this.m_Parent.Repaint();
            }
        }

        internal void RepaintImmediately()
        {
            if ((this.m_Parent != null) && (this.m_Parent.actualView == this))
            {
                this.m_Parent.RepaintImmediately();
            }
        }

        /// <summary>
        /// <para>Sends an Event to a window.</para>
        /// </summary>
        /// <param name="e"></param>
        public bool SendEvent(Event e) => 
            this.m_Parent.SendEvent(e);

        internal void SetParentGameViewDimensions(Rect rect, Rect clippedRect, Vector2 targetSize)
        {
            this.m_GameViewRect = rect;
            this.m_GameViewClippedRect = clippedRect;
            this.m_GameViewTargetSize = targetSize;
            this.m_Parent.SetInternalGameViewDimensions(this.m_GameViewRect, this.m_GameViewClippedRect, this.m_GameViewTargetSize);
        }

        /// <summary>
        /// <para>Show the EditorWindow.</para>
        /// </summary>
        /// <param name="immediateDisplay"></param>
        public void Show()
        {
            this.Show(false);
        }

        /// <summary>
        /// <para>Show the EditorWindow.</para>
        /// </summary>
        /// <param name="immediateDisplay"></param>
        public void Show(bool immediateDisplay)
        {
            if (this.m_Parent == null)
            {
                CreateNewWindowForEditorWindow(this, true, immediateDisplay);
            }
        }

        /// <summary>
        /// <para>Shows a window with dropdown behaviour and styling.</para>
        /// </summary>
        /// <param name="buttonRect">The button from which the position of the window will be determined (see description).</param>
        /// <param name="windowSize">The initial size of the window.</param>
        public void ShowAsDropDown(Rect buttonRect, Vector2 windowSize)
        {
            this.ShowAsDropDown(buttonRect, windowSize, null);
        }

        internal void ShowAsDropDown(Rect buttonRect, Vector2 windowSize, PopupLocationHelper.PopupLocation[] locationPriorityOrder)
        {
            this.ShowAsDropDown(buttonRect, windowSize, locationPriorityOrder, ShowMode.PopupMenu);
        }

        internal void ShowAsDropDown(Rect buttonRect, Vector2 windowSize, PopupLocationHelper.PopupLocation[] locationPriorityOrder, ShowMode mode)
        {
            this.position = this.ShowAsDropDownFitToScreen(buttonRect, windowSize, locationPriorityOrder);
            this.ShowWithMode(mode);
            this.position = this.ShowAsDropDownFitToScreen(buttonRect, windowSize, locationPriorityOrder);
            this.minSize = new Vector2(this.position.width, this.position.height);
            this.maxSize = new Vector2(this.position.width, this.position.height);
            if (focusedWindow != this)
            {
                this.Focus();
            }
            this.m_Parent.AddToAuxWindowList();
            this.m_Parent.window.m_DontSaveToLayout = true;
        }

        internal Rect ShowAsDropDownFitToScreen(Rect buttonRect, Vector2 windowSize, PopupLocationHelper.PopupLocation[] locationPriorityOrder) => 
            this.m_Parent?.window.GetDropDownRect(buttonRect, windowSize, windowSize, locationPriorityOrder);

        /// <summary>
        /// <para>Show the editor window in the auxiliary window.</para>
        /// </summary>
        public void ShowAuxWindow()
        {
            this.ShowWithMode(ShowMode.AuxWindow);
            this.Focus();
            this.m_Parent.AddToAuxWindowList();
        }

        internal void ShowModal()
        {
            this.ShowWithMode(ShowMode.AuxWindow);
            SavedGUIState state = SavedGUIState.Create();
            this.MakeModal(this.m_Parent.window);
            state.ApplyAndForget();
        }

        internal bool ShowNextTabIfPossible()
        {
            DockArea parent = this.m_Parent as DockArea;
            if (parent != null)
            {
                int num = (parent.m_Panes.IndexOf(this) + 1) % parent.m_Panes.Count;
                if (parent.selected != num)
                {
                    parent.selected = num;
                    parent.Repaint();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// <para>Show a notification message.</para>
        /// </summary>
        /// <param name="notification"></param>
        public void ShowNotification(GUIContent notification)
        {
            this.m_Notification = new GUIContent(notification);
            if (this.m_FadeoutTime == 0f)
            {
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.CheckForWindowRepaint));
            }
            this.m_FadeoutTime = (float) (EditorApplication.timeSinceStartup + 4.0);
        }

        /// <summary>
        /// <para>Shows an Editor window using popup-style framing.</para>
        /// </summary>
        public void ShowPopup()
        {
            if (this.m_Parent == null)
            {
                ContainerWindow window = ScriptableObject.CreateInstance<ContainerWindow>();
                window.title = this.titleContent.text;
                HostView view = ScriptableObject.CreateInstance<HostView>();
                view.actualView = this;
                Rect rect = this.m_Parent.borderSize.Add(new Rect(this.position.x, this.position.y, this.position.width, this.position.height));
                window.position = rect;
                window.rootView = view;
                this.MakeParentsSettingsMatchMe();
                window.ShowPopup();
            }
        }

        public void ShowTab()
        {
            DockArea parent = this.m_Parent as DockArea;
            if (parent != null)
            {
                int index = parent.m_Panes.IndexOf(this);
                if (parent.selected != index)
                {
                    parent.selected = index;
                }
            }
            this.Repaint();
        }

        /// <summary>
        /// <para>Show the EditorWindow as a floating utility window.</para>
        /// </summary>
        public void ShowUtility()
        {
            this.ShowWithMode(ShowMode.Utility);
        }

        internal void ShowWithMode(ShowMode mode)
        {
            if (this.m_Parent == null)
            {
                SavedGUIState state = SavedGUIState.Create();
                ContainerWindow window = ScriptableObject.CreateInstance<ContainerWindow>();
                window.title = this.titleContent.text;
                HostView view = ScriptableObject.CreateInstance<HostView>();
                view.actualView = this;
                Rect rect = this.m_Parent.borderSize.Add(new Rect(this.position.x, this.position.y, this.position.width, this.position.height));
                window.position = rect;
                window.rootView = view;
                this.MakeParentsSettingsMatchMe();
                window.Show(mode, true, false);
                state.ApplyAndForget();
            }
        }

        [Obsolete("AA is not supported on EditorWindows", false)]
        public int antiAlias
        {
            get => 
                1;
            set
            {
            }
        }

        /// <summary>
        /// <para>Does the window automatically repaint whenever the scene has changed?</para>
        /// </summary>
        public bool autoRepaintOnSceneChange
        {
            get => 
                this.m_AutoRepaintOnSceneChange;
            set
            {
                this.m_AutoRepaintOnSceneChange = value;
                this.MakeParentsSettingsMatchMe();
            }
        }

        public int depthBufferBits
        {
            get => 
                this.m_DepthBufferBits;
            set
            {
                this.m_DepthBufferBits = value;
            }
        }

        internal bool docked =>
            (((this.m_Parent != null) && (this.m_Parent.window != null)) && !this.m_Parent.window.IsNotDocked());

        internal bool dontClearBackground
        {
            get => 
                this.m_DontClearBackground;
            set
            {
                this.m_DontClearBackground = value;
                if ((this.m_Parent != null) && (this.m_Parent.actualView == this))
                {
                    this.m_Parent.backgroundValid = false;
                }
            }
        }

        /// <summary>
        /// <para>The EditorWindow which currently has keyboard focus. (Read Only)</para>
        /// </summary>
        public static EditorWindow focusedWindow
        {
            get
            {
                HostView focusedView = GUIView.focusedView as HostView;
                if (focusedView != null)
                {
                    return focusedView.actualView;
                }
                return null;
            }
        }

        internal bool hasFocus =>
            ((this.m_Parent != null) && (this.m_Parent.actualView == this));

        /// <summary>
        /// <para>Is this window maximized.</para>
        /// </summary>
        public bool maximized
        {
            get => 
                WindowLayout.IsMaximized(this);
            set
            {
                bool flag = WindowLayout.IsMaximized(this);
                if (value != flag)
                {
                    if (value)
                    {
                        WindowLayout.Maximize(this);
                    }
                    else
                    {
                        WindowLayout.Unmaximize(this);
                    }
                }
            }
        }

        /// <summary>
        /// <para>The maximum size of this window.</para>
        /// </summary>
        public Vector2 maxSize
        {
            get => 
                this.m_MaxSize;
            set
            {
                this.m_MaxSize = value;
                this.MakeParentsSettingsMatchMe();
            }
        }

        /// <summary>
        /// <para>The minimum size of this window.</para>
        /// </summary>
        public Vector2 minSize
        {
            get => 
                this.m_MinSize;
            set
            {
                this.m_MinSize = value;
                this.MakeParentsSettingsMatchMe();
            }
        }

        /// <summary>
        /// <para>The EditorWindow currently under the mouse cursor. (Read Only)</para>
        /// </summary>
        public static EditorWindow mouseOverWindow
        {
            get
            {
                HostView mouseOverView = GUIView.mouseOverView as HostView;
                if (mouseOverView != null)
                {
                    return mouseOverView.actualView;
                }
                return null;
            }
        }

        /// <summary>
        /// <para>The position of the window in screen space.</para>
        /// </summary>
        public Rect position
        {
            get => 
                this.m_Pos;
            set
            {
                this.m_Pos = value;
                if (this.m_Parent != null)
                {
                    DockArea parent = this.m_Parent as DockArea;
                    if (parent == null)
                    {
                        this.m_Parent.window.position = value;
                    }
                    else if (((parent.parent != null) && (parent.m_Panes.Count == 1)) && (parent.parent.parent == null))
                    {
                        Rect rect = parent.borderSize.Add(value);
                        if (parent.background != null)
                        {
                            rect.y -= parent.background.margin.top;
                        }
                        parent.window.position = rect;
                    }
                    else
                    {
                        parent.RemoveTab(this);
                        CreateNewWindowForEditorWindow(this, true, true);
                    }
                }
            }
        }

        /// <summary>
        /// <para>The title of this window.</para>
        /// </summary>
        [Obsolete("Use titleContent instead (it supports setting a title icon as well).")]
        public string title
        {
            get => 
                this.titleContent.text;
            set
            {
                this.titleContent = EditorGUIUtility.TextContent(value);
            }
        }

        /// <summary>
        /// <para>The GUIContent used for drawing the title of EditorWindows.</para>
        /// </summary>
        public GUIContent titleContent
        {
            get
            {
                GUIContent titleContent;
                if (this.m_TitleContent != null)
                {
                    titleContent = this.m_TitleContent;
                }
                else
                {
                    titleContent = this.m_TitleContent = new GUIContent();
                }
                return titleContent;
            }
            set
            {
                this.m_TitleContent = value;
                if (((this.m_TitleContent != null) && (this.m_Parent != null)) && ((this.m_Parent.window != null) && (this.m_Parent.window.rootView == this.m_Parent)))
                {
                    this.m_Parent.window.title = this.m_TitleContent.text;
                }
            }
        }

        /// <summary>
        /// <para>Checks whether MouseEnterWindow and MouseLeaveWindow events are received in the GUI in this Editor window.</para>
        /// </summary>
        public bool wantsMouseEnterLeaveWindow
        {
            get => 
                this.m_WantsMouseEnterLeaveWindow;
            set
            {
                this.m_WantsMouseEnterLeaveWindow = value;
                this.MakeParentsSettingsMatchMe();
            }
        }

        /// <summary>
        /// <para>Checks whether MouseMove events are received in the GUI in this Editor window.</para>
        /// </summary>
        public bool wantsMouseMove
        {
            get => 
                this.m_WantsMouseMove;
            set
            {
                this.m_WantsMouseMove = value;
                this.MakeParentsSettingsMatchMe();
            }
        }

        [CompilerGenerated]
        private sealed class <GetWindow>c__AnonStorey0<T> where T: EditorWindow
        {
            internal System.Type desired;

            internal bool <>m__0(EditorWindow pane) => 
                (pane.GetType() == this.desired);
        }
    }
}

