namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class DockArea : HostView, IDropArea
    {
        [CompilerGenerated]
        private static Func<ContainerWindow, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static EditorApplication.CallbackFunction <>f__mg$cache0;
        [CompilerGenerated]
        private static EditorApplication.CallbackFunction <>f__mg$cache1;
        private const float kBottomBorders = 2f;
        internal const float kDockHeight = 39f;
        private const float kSideBorders = 2f;
        internal const float kTabHeight = 17f;
        private const float kWindowButtonsWidth = 40f;
        private bool m_IsBeingDestroyed;
        [SerializeField]
        internal int m_LastSelected;
        [SerializeField]
        internal List<EditorWindow> m_Panes = new List<EditorWindow>();
        [SerializeField]
        internal int m_Selected;
        private static int s_DragMode;
        private static EditorWindow s_DragPane;
        private static DropInfo s_DropInfo = null;
        internal static View s_IgnoreDockingForView = null;
        internal static DockArea s_OriginalDragSource;
        private static int s_PlaceholderPos;
        private static Vector2 s_StartDragPosition;
        [NonSerialized]
        internal GUIStyle tabStyle = null;

        public DockArea()
        {
            if ((this.m_Panes != null) && (this.m_Panes.Count != 0))
            {
                Debug.LogError("m_Panes is filled in DockArea constructor.");
            }
        }

        protected override void AddDefaultItemsToMenu(GenericMenu menu, EditorWindow view)
        {
            if (menu.GetItemCount() != 0)
            {
                menu.AddSeparator("");
            }
            if (base.parent.window.showMode == ShowMode.MainWindow)
            {
                menu.AddItem(EditorGUIUtility.TextContent("Maximize"), !(base.parent is SplitView), new GenericMenu.MenuFunction2(this.Maximize), view);
            }
            else
            {
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Maximize"));
            }
            if ((base.window.showMode != ShowMode.MainWindow) || (this.GetMainWindowPaneCount() > 1))
            {
                menu.AddItem(EditorGUIUtility.TextContent("Close Tab"), false, new GenericMenu.MenuFunction2(this.Close), view);
            }
            else
            {
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Close Tab"));
            }
            menu.AddSeparator("");
            Type[] paneTypes = base.GetPaneTypes();
            GUIContent content = EditorGUIUtility.TextContent("Add Tab");
            foreach (Type type in paneTypes)
            {
                if (type != null)
                {
                    GUIContent content2 = new GUIContent(EditorWindow.GetLocalizedTitleContentFromType(type));
                    content2.text = content.text + "/" + content2.text;
                    menu.AddItem(content2, false, new GenericMenu.MenuFunction2(this.AddTabToHere), type);
                }
            }
        }

        public void AddTab(EditorWindow pane)
        {
            this.AddTab(this.m_Panes.Count, pane);
        }

        public void AddTab(int idx, EditorWindow pane)
        {
            base.DeregisterSelectedPane(true);
            this.m_Panes.Insert(idx, pane);
            base.m_ActualView = pane;
            this.m_Panes[idx] = pane;
            this.selected = idx;
            base.RegisterSelectedPane();
            SplitView parent = base.parent as SplitView;
            if (parent != null)
            {
                parent.Reflow();
            }
            base.Repaint();
        }

        private void AddTabToHere(object userData)
        {
            EditorWindow pane = (EditorWindow) ScriptableObject.CreateInstance((Type) userData);
            this.AddTab(pane);
        }

        private static void CheckDragWindowExists()
        {
            if ((s_DragMode == 1) && (PaneDragTab.get.m_Window == null))
            {
                s_OriginalDragSource.RemoveTab(s_DragPane);
                Object.DestroyImmediate(s_DragPane);
                PaneDragTab.get.Close();
                GUIUtility.hotControl = 0;
                ResetDragVars();
            }
        }

        internal void Close(object userData)
        {
            EditorWindow window = userData as EditorWindow;
            if (window != null)
            {
                window.Close();
            }
            else
            {
                this.RemoveTab(null, false);
                this.KillIfEmpty();
            }
        }

        public DropInfo DragOver(EditorWindow window, Vector2 mouseScreenPosition)
        {
            Rect screenPosition = base.screenPosition;
            screenPosition.height = 39f;
            if (screenPosition.Contains(mouseScreenPosition))
            {
                if (base.background == null)
                {
                    base.background = "hostview";
                }
                Rect rect2 = base.background.margin.Remove(base.screenPosition);
                Vector2 mousePos = mouseScreenPosition - new Vector2(rect2.x, rect2.y);
                Rect tabRect = this.tabRect;
                int tabAtMousePos = this.GetTabAtMousePos(mousePos, tabRect);
                float tabWidth = this.GetTabWidth(tabRect.width);
                if (s_PlaceholderPos != tabAtMousePos)
                {
                    base.Repaint();
                    s_PlaceholderPos = tabAtMousePos;
                }
                return new DropInfo(this) { 
                    type = DropInfo.Type.Tab,
                    rect = new Rect((mousePos.x - (tabWidth * 0.25f)) + rect2.x, tabRect.y + rect2.y, tabWidth, tabRect.height)
                };
            }
            return null;
        }

        private void DragTab(Rect pos, GUIStyle tabStyle)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            float tabWidth = this.GetTabWidth(pos.width);
            Event current = Event.current;
            if ((s_DragMode != 0) && (GUIUtility.hotControl == 0))
            {
                PaneDragTab.get.Close();
                ResetDragVars();
            }
            EventType typeForControl = current.GetTypeForControl(controlID);
            switch (typeForControl)
            {
                case EventType.MouseDown:
                    if (pos.Contains(current.mousePosition) && (GUIUtility.hotControl == 0))
                    {
                        int tabAtMousePos = this.GetTabAtMousePos(current.mousePosition, pos);
                        if (tabAtMousePos < this.m_Panes.Count)
                        {
                            switch (current.button)
                            {
                                case 0:
                                    if (tabAtMousePos != this.selected)
                                    {
                                        this.selected = tabAtMousePos;
                                    }
                                    GUIUtility.hotControl = controlID;
                                    s_StartDragPosition = current.mousePosition;
                                    s_DragMode = 0;
                                    current.Use();
                                    break;

                                case 2:
                                    this.m_Panes[tabAtMousePos].Close();
                                    current.Use();
                                    break;
                            }
                        }
                    }
                    goto Label_0744;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        Vector2 screenPos = GUIUtility.GUIToScreenPoint(current.mousePosition);
                        if (s_DragMode != 0)
                        {
                            s_DragMode = 0;
                            PaneDragTab.get.Close();
                            if (<>f__mg$cache1 == null)
                            {
                                <>f__mg$cache1 = new EditorApplication.CallbackFunction(DockArea.CheckDragWindowExists);
                            }
                            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, <>f__mg$cache1);
                            if ((s_DropInfo != null) && (s_DropInfo.dropArea != null))
                            {
                                s_DropInfo.dropArea.PerformDrop(s_DragPane, s_DropInfo, screenPos);
                            }
                            else
                            {
                                EditorWindow pane = s_DragPane;
                                ResetDragVars();
                                this.RemoveTab(pane);
                                Rect position = pane.position;
                                position.x = screenPos.x - (position.width * 0.5f);
                                position.y = screenPos.y - (position.height * 0.5f);
                                if (Application.platform == RuntimePlatform.WindowsEditor)
                                {
                                    position.y = Mathf.Max(InternalEditorUtility.GetBoundsOfDesktopAtPoint(screenPos).y, position.y);
                                }
                                EditorWindow.CreateNewWindowForEditorWindow(pane, false, false);
                                pane.position = pane.m_Parent.window.FitWindowRectToScreen(position, true, true);
                                GUIUtility.hotControl = 0;
                                GUIUtility.ExitGUI();
                            }
                            ResetDragVars();
                        }
                        GUIUtility.hotControl = 0;
                        current.Use();
                    }
                    goto Label_0744;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        Vector2 vector3 = current.mousePosition - s_StartDragPosition;
                        current.Use();
                        Rect screenPosition = base.screenPosition;
                        bool flag = (base.window.showMode != ShowMode.MainWindow) || (this.GetMainWindowPaneCount() > 1);
                        if (((s_DragMode == 0) && (vector3.sqrMagnitude > 99f)) && flag)
                        {
                            s_DragMode = 1;
                            s_PlaceholderPos = this.selected;
                            s_DragPane = this.m_Panes[this.selected];
                            if (this.m_Panes.Count != 1)
                            {
                                s_IgnoreDockingForView = null;
                            }
                            else
                            {
                                s_IgnoreDockingForView = this;
                            }
                            s_OriginalDragSource = this;
                            PaneDragTab.get.Show(new Rect((pos.x + screenPosition.x) + (tabWidth * this.selected), pos.y + screenPosition.y, tabWidth, pos.height), s_DragPane.titleContent, base.position.size, GUIUtility.GUIToScreenPoint(current.mousePosition));
                            if (<>f__mg$cache0 == null)
                            {
                                <>f__mg$cache0 = new EditorApplication.CallbackFunction(DockArea.CheckDragWindowExists);
                            }
                            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, <>f__mg$cache0);
                            GUIUtility.ExitGUI();
                        }
                        if (s_DragMode == 1)
                        {
                            DropInfo di = null;
                            ContainerWindow[] windows = ContainerWindow.windows;
                            Vector2 mouseScreenPosition = GUIUtility.GUIToScreenPoint(current.mousePosition);
                            ContainerWindow inFrontOf = null;
                            foreach (ContainerWindow window2 in windows)
                            {
                                SplitView rootSplitView = window2.rootSplitView;
                                if (rootSplitView != null)
                                {
                                    di = rootSplitView.DragOverRootView(mouseScreenPosition);
                                }
                                if (di == null)
                                {
                                    foreach (View view2 in window2.rootView.allChildren)
                                    {
                                        IDropArea area = view2 as IDropArea;
                                        if (area != null)
                                        {
                                            di = area.DragOver(s_DragPane, mouseScreenPosition);
                                        }
                                        if (di != null)
                                        {
                                            break;
                                        }
                                    }
                                }
                                if (di != null)
                                {
                                    inFrontOf = window2;
                                    break;
                                }
                            }
                            if (di == null)
                            {
                                di = new DropInfo(null);
                            }
                            if (di.type != DropInfo.Type.Tab)
                            {
                                s_PlaceholderPos = -1;
                            }
                            s_DropInfo = di;
                            if (PaneDragTab.get.m_Window != null)
                            {
                                PaneDragTab.get.SetDropInfo(di, mouseScreenPosition, inFrontOf);
                            }
                        }
                    }
                    goto Label_0744;

                case EventType.Repaint:
                {
                    float xMin = pos.xMin;
                    int num9 = 0;
                    if (base.actualView == null)
                    {
                        Rect rect7 = new Rect(xMin, pos.yMin, tabWidth, pos.height);
                        float x = Mathf.Round(rect7.x);
                        Rect rect8 = new Rect(x, rect7.y, Mathf.Round(rect7.x + rect7.width) - x, rect7.height);
                        tabStyle.Draw(rect8, "Failed to load", false, false, true, false);
                    }
                    else
                    {
                        for (int i = 0; i < this.m_Panes.Count; i++)
                        {
                            if (s_DragPane != this.m_Panes[i])
                            {
                                if (((s_DropInfo != null) && object.ReferenceEquals(s_DropInfo.dropArea, this)) && (s_PlaceholderPos == num9))
                                {
                                    xMin += tabWidth;
                                }
                                Rect rect5 = new Rect(xMin, pos.yMin, tabWidth, pos.height);
                                float num11 = Mathf.Round(rect5.x);
                                Rect rect6 = new Rect(num11, rect5.y, Mathf.Round(rect5.x + rect5.width) - num11, rect5.height);
                                tabStyle.Draw(rect6, this.m_Panes[i].titleContent, false, false, i == this.selected, base.hasFocus);
                                xMin += tabWidth;
                                num9++;
                            }
                        }
                    }
                    goto Label_0744;
                }
            }
            if ((typeForControl == EventType.ContextClick) && (pos.Contains(current.mousePosition) && (GUIUtility.hotControl == 0)))
            {
                int num5 = this.GetTabAtMousePos(current.mousePosition, pos);
                if (num5 < this.m_Panes.Count)
                {
                    base.PopupGenericMenu(this.m_Panes[num5], new Rect(current.mousePosition.x, current.mousePosition.y, 0f, 0f));
                }
            }
        Label_0744:
            this.selected = Mathf.Clamp(this.selected, 0, this.m_Panes.Count - 1);
        }

        protected override RectOffset GetBorderSize()
        {
            if (base.window != null)
            {
                int num = 0;
                base.m_BorderSize.bottom = num;
                base.m_BorderSize.top = num;
                base.m_BorderSize.right = num;
                base.m_BorderSize.left = num;
                Rect windowPosition = base.windowPosition;
                if (windowPosition.xMin != 0f)
                {
                    base.m_BorderSize.left += 2;
                }
                if (windowPosition.xMax != base.window.position.width)
                {
                    base.m_BorderSize.right += 2;
                }
                base.m_BorderSize.top = 0x11;
                bool flag = base.windowPosition.y == 0f;
                bool flag2 = windowPosition.yMax == base.window.position.height;
                base.m_BorderSize.bottom = 4;
                if (flag2)
                {
                    base.m_BorderSize.bottom -= 2;
                }
                if (flag)
                {
                    base.m_BorderSize.bottom += 3;
                }
            }
            return base.m_BorderSize;
        }

        private int GetMainWindowPaneCount()
        {
            int num = 0;
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<ContainerWindow, bool>(null, (IntPtr) <GetMainWindowPaneCount>m__0);
            }
            ContainerWindow window = Enumerable.First<ContainerWindow>(ContainerWindow.windows, <>f__am$cache0);
            if (window != null)
            {
                foreach (View view in window.rootView.allChildren)
                {
                    DockArea area = view as DockArea;
                    if (area != null)
                    {
                        num += area.m_Panes.Count;
                    }
                }
            }
            return num;
        }

        private int GetTabAtMousePos(Vector2 mousePos, Rect position) => 
            ((int) Mathf.Min((float) ((mousePos.x - position.xMin) / this.GetTabWidth(position.width)), (float) 100f));

        private float GetTabWidth(float width)
        {
            int count = this.m_Panes.Count;
            if ((s_DropInfo != null) && object.ReferenceEquals(s_DropInfo.dropArea, this))
            {
                count++;
            }
            if (this.m_Panes.IndexOf(s_DragPane) != -1)
            {
                count--;
            }
            return Mathf.Min((float) (width / ((float) count)), (float) 100f);
        }

        internal override void Initialize(ContainerWindow win)
        {
            base.Initialize(win);
            this.RemoveNullWindows();
            foreach (EditorWindow window in this.m_Panes)
            {
                window.m_Parent = this;
            }
        }

        private void KillIfEmpty()
        {
            if (this.m_Panes.Count == 0)
            {
                if (base.parent == null)
                {
                    base.window.InternalCloseWindow();
                }
                else
                {
                    SplitView parent = base.parent as SplitView;
                    ICleanuppable cleanuppable = base.parent as ICleanuppable;
                    parent.RemoveChildNice(this);
                    if (!this.m_IsBeingDestroyed)
                    {
                        Object.DestroyImmediate(this, true);
                    }
                    if (cleanuppable != null)
                    {
                        cleanuppable.Cleanup();
                    }
                }
            }
        }

        private void Maximize(object userData)
        {
            EditorWindow win = userData as EditorWindow;
            if (win != null)
            {
                WindowLayout.Maximize(win);
            }
        }

        public void OnDestroy()
        {
            this.m_IsBeingDestroyed = true;
            if (base.hasFocus)
            {
                base.Invoke("OnLostFocus");
            }
            base.actualView = null;
            foreach (EditorWindow window in this.m_Panes)
            {
                Object.DestroyImmediate(window, true);
            }
            base.OnDestroy();
        }

        public void OnEnable()
        {
            if (this.m_Panes != null)
            {
                if (this.m_Panes.Count == 0)
                {
                    this.m_Selected = 0;
                }
                else
                {
                    this.m_Selected = Math.Min(this.m_Selected, this.m_Panes.Count - 1);
                    base.actualView = this.m_Panes[this.m_Selected];
                }
            }
            base.OnEnable();
        }

        public void OnGUI()
        {
            base.ClearBackground();
            EditorGUIUtility.ResetGUIState();
            SplitView parent = base.parent as SplitView;
            if ((Event.current.type == EventType.Repaint) && (parent != null))
            {
                View child = this;
                while (parent != null)
                {
                    int controlID = parent.controlID;
                    if ((controlID == GUIUtility.hotControl) || (GUIUtility.hotControl == 0))
                    {
                        int num2 = parent.IndexOfChild(child);
                        if (parent.vertical)
                        {
                            if (num2 != 0)
                            {
                                EditorGUIUtility.AddCursorRect(new Rect(0f, 0f, base.position.width, 5f), MouseCursor.SplitResizeUpDown, controlID);
                            }
                            if (num2 != (parent.children.Length - 1))
                            {
                                EditorGUIUtility.AddCursorRect(new Rect(0f, base.position.height - 5f, base.position.width, 5f), MouseCursor.SplitResizeUpDown, controlID);
                            }
                        }
                        else
                        {
                            if (num2 != 0)
                            {
                                EditorGUIUtility.AddCursorRect(new Rect(0f, 0f, 5f, base.position.height), MouseCursor.SplitResizeLeftRight, controlID);
                            }
                            if (num2 != (parent.children.Length - 1))
                            {
                                EditorGUIUtility.AddCursorRect(new Rect(base.position.width - 5f, 0f, 5f, base.position.height), MouseCursor.SplitResizeLeftRight, controlID);
                            }
                        }
                    }
                    child = parent;
                    parent = parent.parent as SplitView;
                }
                parent = base.parent as SplitView;
            }
            bool flag = false;
            if (base.window.rootView.GetType() != typeof(MainView))
            {
                flag = true;
                if (base.windowPosition.y == 0f)
                {
                    base.background = "dockareaStandalone";
                }
                else
                {
                    base.background = "dockarea";
                }
            }
            else
            {
                base.background = "dockarea";
            }
            if (parent != null)
            {
                Event evt = new Event(Event.current);
                evt.mousePosition += new Vector2(base.position.x, base.position.y);
                parent.SplitGUI(evt);
                if (evt.type == EventType.Used)
                {
                    Event.current.Use();
                }
            }
            Rect position = base.background.margin.Remove(new Rect(0f, 0f, base.position.width, base.position.height));
            position.x = base.background.margin.left;
            position.y = base.background.margin.top;
            Rect windowPosition = base.windowPosition;
            float num3 = 2f;
            if (windowPosition.x == 0f)
            {
                position.x -= num3;
                position.width += num3;
            }
            if (windowPosition.xMax == base.window.position.width)
            {
                position.width += num3;
            }
            if (windowPosition.yMax == base.window.position.height)
            {
                position.height += !flag ? 2f : 2f;
            }
            GUI.Box(position, GUIContent.none, base.background);
            if (this.tabStyle == null)
            {
                this.tabStyle = "dragtab";
            }
            if (this.m_Panes.Count > 0)
            {
                HostView.BeginOffsetArea(new Rect(position.x + 2f, position.y + 17f, position.width - 4f, (position.height - 17f) - 2f), GUIContent.none, "TabWindowBackground");
                Vector2 vector = GUIUtility.GUIToScreenPoint(Vector2.zero);
                Rect rect16 = base.borderSize.Remove(base.position);
                rect16.x = vector.x;
                rect16.y = vector.y;
                this.m_Panes[this.selected].m_Pos = rect16;
                HostView.EndOffsetArea();
            }
            this.DragTab(new Rect(position.x + 1f, position.y, position.width - 40f, 17f), this.tabStyle);
            this.tabStyle = "dragtab";
            base.ShowGenericMenu();
            if (this.m_Panes.Count > 0)
            {
                base.InvokeOnGUI(position);
            }
            EditorGUI.ShowRepaints();
            Highlighter.ControlHighlightGUI(this);
        }

        public bool PerformDrop(EditorWindow w, DropInfo info, Vector2 screenPos)
        {
            s_OriginalDragSource.RemoveTab(w, s_OriginalDragSource != this);
            int idx = (s_PlaceholderPos <= this.m_Panes.Count) ? s_PlaceholderPos : this.m_Panes.Count;
            this.AddTab(idx, w);
            this.selected = idx;
            return true;
        }

        private void RemoveNullWindows()
        {
            List<EditorWindow> list = new List<EditorWindow>();
            foreach (EditorWindow window in this.m_Panes)
            {
                if (window != null)
                {
                    list.Add(window);
                }
            }
            this.m_Panes = list;
        }

        public void RemoveTab(EditorWindow pane)
        {
            this.RemoveTab(pane, true);
        }

        public void RemoveTab(EditorWindow pane, bool killIfEmpty)
        {
            if (base.m_ActualView == pane)
            {
                base.DeregisterSelectedPane(true);
            }
            int index = this.m_Panes.IndexOf(pane);
            if (index != -1)
            {
                this.m_Panes.Remove(pane);
                if (index == this.m_LastSelected)
                {
                    this.m_LastSelected = this.m_Panes.Count - 1;
                }
                else if ((index < this.m_LastSelected) || (this.m_LastSelected == this.m_Panes.Count))
                {
                    this.m_LastSelected--;
                }
                this.m_LastSelected = Mathf.Clamp(this.m_LastSelected, 0, this.m_Panes.Count - 1);
                if (index == this.m_Selected)
                {
                    this.m_Selected = this.m_LastSelected;
                }
                else
                {
                    this.m_Selected = this.m_Panes.IndexOf(base.m_ActualView);
                }
                if ((this.m_Selected >= 0) && (this.m_Selected < this.m_Panes.Count))
                {
                    base.actualView = this.m_Panes[this.m_Selected];
                }
                base.Repaint();
                pane.m_Parent = null;
                if (killIfEmpty)
                {
                    this.KillIfEmpty();
                }
                base.RegisterSelectedPane();
            }
        }

        private static void ResetDragVars()
        {
            s_DragPane = null;
            s_DropInfo = null;
            s_PlaceholderPos = -1;
            s_DragMode = 0;
            s_OriginalDragSource = null;
        }

        public int selected
        {
            get => 
                this.m_Selected;
            set
            {
                if (this.m_Selected != value)
                {
                    this.m_LastSelected = this.m_Selected;
                }
                this.m_Selected = value;
                if ((this.m_Selected >= 0) && (this.m_Selected < this.m_Panes.Count))
                {
                    base.actualView = this.m_Panes[this.m_Selected];
                }
            }
        }

        private Rect tabRect =>
            new Rect(0f, 0f, base.position.width, 17f);
    }
}

