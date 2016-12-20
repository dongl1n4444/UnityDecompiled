namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Serialization;

    [StructLayout(LayoutKind.Sequential)]
    internal sealed class ContainerWindow : ScriptableObject
    {
        [SerializeField]
        private MonoReloadableIntPtr m_WindowPtr;
        [SerializeField]
        private Rect m_PixelRect = new Rect(0f, 0f, 400f, 300f);
        [SerializeField]
        private int m_ShowMode;
        [SerializeField]
        private string m_Title = "";
        [FormerlySerializedAs("m_MainView"), SerializeField]
        private View m_RootView;
        [SerializeField]
        private Vector2 m_MinSize = new Vector2(120f, 80f);
        [SerializeField]
        private Vector2 m_MaxSize = new Vector2(4000f, 4000f);
        internal bool m_DontSaveToLayout = false;
        private const float kBorderSize = 4f;
        private const float kTitleHeight = 24f;
        private int m_ButtonCount;
        private float m_TitleBarWidth;
        private const float kButtonWidth = 13f;
        private const float kButtonHeight = 13f;
        private const float kButtonSpacing = 3f;
        private const float kButtonTop = 0f;
        private static List<ContainerWindow> s_AllWindows = new List<ContainerWindow>();
        private static Vector2 s_LastDragMousePos;
        private static Rect dragPosition;
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetAlpha(float alpha);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetInvisible();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool IsZoomed();
        private void Internal_SetMinMaxSizes(Vector2 minSize, Vector2 maxSize)
        {
            INTERNAL_CALL_Internal_SetMinMaxSizes(this, ref minSize, ref maxSize);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Internal_SetMinMaxSizes(ContainerWindow self, ref Vector2 minSize, ref Vector2 maxSize);
        private void Internal_Show(Rect r, int showMode, Vector2 minSize, Vector2 maxSize)
        {
            INTERNAL_CALL_Internal_Show(this, ref r, showMode, ref minSize, ref maxSize);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Internal_Show(ContainerWindow self, ref Rect r, int showMode, ref Vector2 minSize, ref Vector2 maxSize);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void Internal_BringLiveAfterCreation(bool displayImmediately, bool setFocus);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetFreezeDisplay(bool freeze);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void DisplayAllViews();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void Minimize();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void ToggleMaximize();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void MoveInFrontOf(ContainerWindow other);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void MoveBehindOf(ContainerWindow other);
        public bool maximized { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void InternalClose();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void OnDestroy();
        public Rect position
        {
            get
            {
                Rect rect;
                this.INTERNAL_get_position(out rect);
                return rect;
            }
            set
            {
                this.INTERNAL_set_position(ref value);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_position(out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_position(ref Rect value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void Internal_SetTitle(string title);
        private void SetBackgroundColor(Color color)
        {
            INTERNAL_CALL_SetBackgroundColor(this, ref color);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetBackgroundColor(ContainerWindow self, ref Color color);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void GetOrderedWindowList();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void Internal_GetTopleftScreenPosition(out Vector2 pos);
        internal Rect FitWindowRectToScreen(Rect r, bool forceCompletelyVisible, bool useMouseScreen)
        {
            Rect rect;
            INTERNAL_CALL_FitWindowRectToScreen(this, ref r, forceCompletelyVisible, useMouseScreen, out rect);
            return rect;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_FitWindowRectToScreen(ContainerWindow self, ref Rect r, bool forceCompletelyVisible, bool useMouseScreen, out Rect value);
        internal static Rect FitRectToScreen(Rect defaultRect, bool forceCompletelyVisible, bool useMouseScreen)
        {
            Rect rect;
            INTERNAL_CALL_FitRectToScreen(ref defaultRect, forceCompletelyVisible, useMouseScreen, out rect);
            return rect;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_FitRectToScreen(ref Rect defaultRect, bool forceCompletelyVisible, bool useMouseScreen, out Rect value);
        internal static bool macEditor
        {
            get
            {
                return (Application.platform == RuntimePlatform.OSXEditor);
            }
        }
        private void __internalAwake()
        {
            base.hideFlags = HideFlags.DontSave;
        }

        internal ShowMode showMode
        {
            get
            {
                return (ShowMode) this.m_ShowMode;
            }
        }
        internal static bool IsPopup(ShowMode mode)
        {
            return ((mode == ShowMode.PopupMenu) || (ShowMode.PopupMenuWithKeyboardFocus == mode));
        }

        internal bool isPopup
        {
            get
            {
                return IsPopup((ShowMode) this.m_ShowMode);
            }
        }
        internal void ShowPopup()
        {
            this.m_ShowMode = 1;
            this.Internal_Show(this.m_PixelRect, this.m_ShowMode, this.m_MinSize, this.m_MaxSize);
            if (this.m_RootView != null)
            {
                this.m_RootView.SetWindowRecurse(this);
            }
            this.Internal_SetTitle(this.m_Title);
            this.Save();
            this.Internal_BringLiveAfterCreation(false, false);
        }

        private static Color skinBackgroundColor
        {
            get
            {
                return (!EditorGUIUtility.isProSkin ? Color.gray.AlphaMultiplied(0.32f) : Color.gray.RGBMultiplied((float) 0.3f).AlphaMultiplied(0.5f));
            }
        }
        public void Show(ShowMode showMode, bool loadPosition, bool displayImmediately)
        {
            if (showMode == ShowMode.AuxWindow)
            {
                showMode = ShowMode.Utility;
            }
            if ((showMode == ShowMode.Utility) || IsPopup(showMode))
            {
                this.m_DontSaveToLayout = true;
            }
            this.m_ShowMode = (int) showMode;
            if (!this.isPopup)
            {
                this.Load(loadPosition);
            }
            this.Internal_Show(this.m_PixelRect, this.m_ShowMode, this.m_MinSize, this.m_MaxSize);
            if (this.m_RootView != null)
            {
                this.m_RootView.SetWindowRecurse(this);
            }
            this.Internal_SetTitle(this.m_Title);
            this.SetBackgroundColor(skinBackgroundColor);
            this.Internal_BringLiveAfterCreation(displayImmediately, true);
            if (this != null)
            {
                this.position = this.FitWindowRectToScreen(this.m_PixelRect, true, false);
                this.rootView.position = new Rect(0f, 0f, this.m_PixelRect.width, this.m_PixelRect.height);
                this.rootView.Reflow();
                this.Save();
            }
        }

        public void OnEnable()
        {
            if (this.m_RootView != null)
            {
                this.m_RootView.Initialize(this);
            }
            this.SetBackgroundColor(skinBackgroundColor);
        }

        public void SetMinMaxSizes(Vector2 min, Vector2 max)
        {
            this.m_MinSize = min;
            this.m_MaxSize = max;
            Rect position = this.position;
            Rect rect2 = position;
            rect2.width = Mathf.Clamp(position.width, min.x, max.x);
            rect2.height = Mathf.Clamp(position.height, min.y, max.y);
            if ((rect2.width != position.width) || (rect2.height != position.height))
            {
                this.position = rect2;
            }
            this.Internal_SetMinMaxSizes(min, max);
        }

        internal void InternalCloseWindow()
        {
            this.Save();
            if (this.m_RootView != null)
            {
                if (this.m_RootView is GUIView)
                {
                    ((GUIView) this.m_RootView).RemoveFromAuxWindowList();
                }
                Object.DestroyImmediate(this.m_RootView, true);
                this.m_RootView = null;
            }
            Object.DestroyImmediate(this, true);
        }

        public void Close()
        {
            this.Save();
            this.InternalClose();
            Object.DestroyImmediate(this, true);
        }

        internal bool IsNotDocked()
        {
            return (((this.m_ShowMode == 2) || (this.m_ShowMode == 5)) || ((((this.rootView is SplitView) && (this.rootView.children.Length == 1)) && ((this.rootView.children.Length == 1) && (this.rootView.children[0] is DockArea))) && (((DockArea) this.rootView.children[0]).m_Panes.Count == 1)));
        }

        private string NotDockedWindowID()
        {
            if (this.IsNotDocked())
            {
                HostView rootView = this.rootView as HostView;
                if (rootView == null)
                {
                    if (!(this.rootView is SplitView))
                    {
                        return this.rootView.GetType().ToString();
                    }
                    rootView = (HostView) this.rootView.children[0];
                }
                return (((this.m_ShowMode != 2) && (this.m_ShowMode != 5)) ? ((DockArea) this.rootView.children[0]).m_Panes[0].GetType().ToString() : rootView.actualView.GetType().ToString());
            }
            return null;
        }

        public void Save()
        {
            if (((this.m_ShowMode != 4) && this.IsNotDocked()) && !this.IsZoomed())
            {
                string str = this.NotDockedWindowID();
                EditorPrefs.SetFloat(str + "x", this.m_PixelRect.x);
                EditorPrefs.SetFloat(str + "y", this.m_PixelRect.y);
                EditorPrefs.SetFloat(str + "w", this.m_PixelRect.width);
                EditorPrefs.SetFloat(str + "h", this.m_PixelRect.height);
            }
        }

        private void Load(bool loadPosition)
        {
            if ((this.m_ShowMode != 4) && this.IsNotDocked())
            {
                string str = this.NotDockedWindowID();
                Rect pixelRect = this.m_PixelRect;
                if (loadPosition)
                {
                    pixelRect.x = EditorPrefs.GetFloat(str + "x", this.m_PixelRect.x);
                    pixelRect.y = EditorPrefs.GetFloat(str + "y", this.m_PixelRect.y);
                }
                pixelRect.width = Mathf.Max(EditorPrefs.GetFloat(str + "w", this.m_PixelRect.width), this.m_MinSize.x);
                pixelRect.width = Mathf.Min(pixelRect.width, this.m_MaxSize.x);
                pixelRect.height = Mathf.Max(EditorPrefs.GetFloat(str + "h", this.m_PixelRect.height), this.m_MinSize.y);
                pixelRect.height = Mathf.Min(pixelRect.height, this.m_MaxSize.y);
                this.m_PixelRect = pixelRect;
            }
        }

        internal void OnResize()
        {
            if (this.rootView != null)
            {
                this.rootView.position = new Rect(0f, 0f, this.position.width, this.position.height);
                this.Save();
            }
        }

        public string title
        {
            get
            {
                return this.m_Title;
            }
            set
            {
                this.m_Title = value;
                this.Internal_SetTitle(value);
            }
        }
        public static ContainerWindow[] windows
        {
            get
            {
                s_AllWindows.Clear();
                GetOrderedWindowList();
                return s_AllWindows.ToArray();
            }
        }
        internal void AddToWindowList()
        {
            s_AllWindows.Add(this);
        }

        public Vector2 WindowToScreenPoint(Vector2 windowPoint)
        {
            Vector2 vector;
            this.Internal_GetTopleftScreenPosition(out vector);
            return (windowPoint + vector);
        }

        public View rootView
        {
            get
            {
                return this.m_RootView;
            }
            set
            {
                this.m_RootView = value;
                this.m_RootView.SetWindowRecurse(this);
                this.m_RootView.position = new Rect(0f, 0f, this.position.width, this.position.height);
                this.m_MinSize = value.minSize;
                this.m_MaxSize = value.maxSize;
            }
        }
        public SplitView rootSplitView
        {
            get
            {
                if (((this.m_ShowMode == 4) && (this.rootView != null)) && (this.rootView.children.Length == 3))
                {
                    return (this.rootView.children[1] as SplitView);
                }
                return (this.rootView as SplitView);
            }
        }
        internal string DebugHierarchy()
        {
            return this.rootView.DebugHierarchy(0);
        }

        internal Rect GetDropDownRect(Rect buttonRect, Vector2 minSize, Vector2 maxSize, PopupLocationHelper.PopupLocation[] locationPriorityOrder)
        {
            return PopupLocationHelper.GetDropDownRect(buttonRect, minSize, maxSize, this, locationPriorityOrder);
        }

        internal Rect GetDropDownRect(Rect buttonRect, Vector2 minSize, Vector2 maxSize)
        {
            return PopupLocationHelper.GetDropDownRect(buttonRect, minSize, maxSize, this);
        }

        internal Rect FitPopupWindowRectToScreen(Rect rect, float minimumHeight)
        {
            float num = 0f;
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                num = 10f;
            }
            float b = minimumHeight + num;
            Rect r = rect;
            r.height = Mathf.Min(r.height, 900f);
            r.height += num;
            r = this.FitWindowRectToScreen(r, true, true);
            float num3 = Mathf.Max(r.yMax - rect.y, b);
            r.y = r.yMax - num3;
            r.height = num3 - num;
            return r;
        }

        public void HandleWindowDecorationEnd(Rect windowPosition)
        {
        }

        public void HandleWindowDecorationStart(Rect windowPosition)
        {
            if (((windowPosition.y == 0f) && (this.showMode != ShowMode.Utility)) && !this.isPopup)
            {
                if (Mathf.Abs((float) (windowPosition.xMax - this.position.width)) < 2f)
                {
                    GUIStyle buttonClose = Styles.buttonClose;
                    GUIStyle buttonMin = Styles.buttonMin;
                    GUIStyle buttonMax = Styles.buttonMax;
                    if (macEditor && ((GUIView.focusedView == null) || (GUIView.focusedView.window != this)))
                    {
                        buttonClose = buttonMin = buttonMax = Styles.buttonInactive;
                    }
                    this.BeginTitleBarButtons(windowPosition);
                    if (this.TitleBarButton(buttonClose))
                    {
                        this.Close();
                    }
                    if (macEditor && this.TitleBarButton(buttonMin))
                    {
                        this.Minimize();
                        GUIUtility.ExitGUI();
                    }
                    if (this.TitleBarButton(buttonMax))
                    {
                        this.ToggleMaximize();
                    }
                }
                this.DragTitleBar(new Rect(0f, 0f, this.position.width, 24f));
            }
        }

        private void BeginTitleBarButtons(Rect windowPosition)
        {
            this.m_ButtonCount = 0;
            this.m_TitleBarWidth = windowPosition.width;
        }

        private bool TitleBarButton(GUIStyle style)
        {
            Rect position = new Rect((this.m_TitleBarWidth - (13f * ++this.m_ButtonCount)) - 4f, 0f, 13f, 13f);
            return GUI.Button(position, GUIContent.none, style);
        }

        private void DragTitleBar(Rect titleBarRect)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            Event current = Event.current;
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if ((titleBarRect.Contains(current.mousePosition) && (GUIUtility.hotControl == 0)) && (current.button == 0))
                    {
                        GUIUtility.hotControl = controlID;
                        Event.current.Use();
                        s_LastDragMousePos = GUIUtility.GUIToScreenPoint(current.mousePosition);
                        dragPosition = this.position;
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                        Event.current.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        Vector2 vector = GUIUtility.GUIToScreenPoint(current.mousePosition);
                        Vector2 vector2 = vector - s_LastDragMousePos;
                        s_LastDragMousePos = vector;
                        dragPosition.x += vector2.x;
                        dragPosition.y += vector2.y;
                        this.position = dragPosition;
                        GUI.changed = true;
                    }
                    break;

                case EventType.Repaint:
                    EditorGUIUtility.AddCursorRect(titleBarRect, MouseCursor.Arrow);
                    break;
            }
        }
        private static class Styles
        {
            public static GUIStyle buttonClose = (!ContainerWindow.macEditor ? "WinBtnClose" : "WinBtnCloseMac");
            public static GUIStyle buttonInactive = "WinBtnInactiveMac";
            public static GUIStyle buttonMax = (!ContainerWindow.macEditor ? "WinBtnMax" : "WinBtnMaxMac");
            public static GUIStyle buttonMin = (!ContainerWindow.macEditor ? "WinBtnClose" : "WinBtnMinMac");
        }
    }
}

