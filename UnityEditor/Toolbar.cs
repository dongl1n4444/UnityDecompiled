namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor.Collaboration;
    using UnityEditor.Connect;
    using UnityEditor.Web;
    using UnityEditorInternal;
    using UnityEngine;

    internal class Toolbar : GUIView
    {
        [CompilerGenerated]
        private static Func<float> <>f__am$cache0;
        [CompilerGenerated]
        private static GenericMenu.MenuFunction <>f__am$cache1;
        [CompilerGenerated]
        private static GenericMenu.MenuFunction <>f__am$cache2;
        [CompilerGenerated]
        private static GenericMenu.MenuFunction <>f__am$cache3;
        public static Toolbar get = null;
        private const float kCollabButtonWidth = 78f;
        private ButtonWithAnimatedIconRotation m_CollabButton;
        private CollabToolbarState m_CollabToolbarState = CollabToolbarState.UpToDate;
        private string m_DynamicTooltip;
        [SerializeField]
        private string m_LastLoadedLayoutName;
        private static bool m_ShowCollabTooltip = false;
        public static bool requestShowCollabToolbar = false;
        private static GUIContent s_AccountContent;
        private static GUIContent s_CloudIcon;
        private static GUIContent[] s_CollabIcons;
        private static GUIContent s_LayerContent;
        private static GUIContent[] s_PivotIcons;
        private static GUIContent[] s_PivotRotation;
        private static GUIContent[] s_PlayIcons;
        private static GUIContent[] s_ShownToolIcons = new GUIContent[5];
        private static GUIContent[] s_ToolIcons;
        private static GUIContent[] s_ViewToolIcons;

        public float CalcHeight() => 
            30f;

        private void DoCollabDropDown(Rect rect)
        {
            this.UpdateCollabToolbarState();
            bool requestShowCollabToolbar = Toolbar.requestShowCollabToolbar;
            Toolbar.requestShowCollabToolbar = false;
            bool flag2 = !EditorApplication.isPlaying;
            using (new EditorGUI.DisabledScope(!flag2))
            {
                bool animate = this.m_CollabToolbarState == CollabToolbarState.InProgress;
                EditorGUIUtility.SetIconSize(new Vector2(12f, 12f));
                if (this.m_CollabButton.OnGUI(rect, this.currentCollabContent, animate, Styles.collabButtonStyle))
                {
                    requestShowCollabToolbar = true;
                }
                EditorGUIUtility.SetIconSize(Vector2.zero);
            }
            if (requestShowCollabToolbar)
            {
                this.ShowPopup(rect);
            }
        }

        private void DoLayersDropDown(Rect rect)
        {
            GUIStyle style = "DropDown";
            if (EditorGUI.ButtonMouseDown(rect, s_LayerContent, FocusType.Passive, style) && LayerVisibilityWindow.ShowAtPosition(rect))
            {
                GUIUtility.ExitGUI();
            }
        }

        private void DoLayoutDropDown(Rect rect)
        {
            if (EditorGUI.ButtonMouseDown(rect, GUIContent.Temp(lastLoadedLayoutName), FocusType.Passive, "DropDown"))
            {
                Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(rect.x, rect.y));
                rect.x = vector.x;
                rect.y = vector.y;
                EditorUtility.Internal_DisplayPopupMenu(rect, "Window/Layouts", this, 0);
            }
        }

        private void DoPivotButtons(Rect rect)
        {
            Tools.pivotMode = (PivotMode) EditorGUI.CycleButton(new Rect(rect.x, rect.y, rect.width / 2f, rect.height), (int) Tools.pivotMode, s_PivotIcons, "ButtonLeft");
            if ((Tools.current == Tool.Scale) && (Selection.transforms.Length < 2))
            {
                GUI.enabled = false;
            }
            PivotRotation rotation = (PivotRotation) EditorGUI.CycleButton(new Rect(rect.x + (rect.width / 2f), rect.y, rect.width / 2f, rect.height), (int) Tools.pivotRotation, s_PivotRotation, "ButtonRight");
            if (Tools.pivotRotation != rotation)
            {
                Tools.pivotRotation = rotation;
                if (rotation == PivotRotation.Global)
                {
                    Tools.ResetGlobalHandleRotation();
                }
            }
            if (Tools.current == Tool.Scale)
            {
                GUI.enabled = true;
            }
            if (GUI.changed)
            {
                Tools.RepaintAllToolViews();
            }
        }

        private void DoPlayButtons(bool isOrWillEnterPlaymode)
        {
            bool isPlaying = EditorApplication.isPlaying;
            GUI.changed = false;
            int index = !isPlaying ? 0 : 4;
            if (AnimationMode.InAnimationMode())
            {
                index = 8;
            }
            Color color = GUI.color + new Color(0.01f, 0.01f, 0.01f, 0.01f);
            GUI.contentColor = new Color(1f / color.r, 1f / color.g, 1f / color.g, 1f / color.a);
            GUILayout.Toggle(isOrWillEnterPlaymode, s_PlayIcons[index], "CommandLeft", new GUILayoutOption[0]);
            GUI.backgroundColor = Color.white;
            if (GUI.changed)
            {
                TogglePlaying();
                GUIUtility.ExitGUI();
            }
            GUI.changed = false;
            bool flag2 = GUILayout.Toggle(EditorApplication.isPaused, s_PlayIcons[index + 1], "CommandMid", new GUILayoutOption[0]);
            if (GUI.changed)
            {
                EditorApplication.isPaused = flag2;
                GUIUtility.ExitGUI();
            }
            if (GUILayout.Button(s_PlayIcons[index + 2], "CommandRight", new GUILayoutOption[0]))
            {
                EditorApplication.Step();
                GUIUtility.ExitGUI();
            }
        }

        private void DoToolButtons(Rect rect)
        {
            GUI.changed = false;
            int selected = !Tools.viewToolActive ? ((int) Tools.current) : 0;
            for (int i = 1; i < 5; i++)
            {
                s_ShownToolIcons[i] = s_ToolIcons[(i - 1) + ((i != selected) ? 0 : 4)];
                s_ShownToolIcons[i].tooltip = s_ToolIcons[i - 1].tooltip;
            }
            s_ShownToolIcons[0] = s_ViewToolIcons[((int) Tools.viewTool) + ((selected != 0) ? 0 : 4)];
            selected = GUI.Toolbar(rect, selected, s_ShownToolIcons, "Command");
            if (GUI.changed)
            {
                Tools.current = (Tool) selected;
            }
        }

        private Rect GetThickArea(Rect pos) => 
            new Rect(pos.x, 5f, pos.width, 24f);

        private Rect GetThinArea(Rect pos) => 
            new Rect(pos.x, 7f, pos.width, 18f);

        private void InitializeToolIcons()
        {
            if (s_ToolIcons == null)
            {
                s_ToolIcons = new GUIContent[] { EditorGUIUtility.IconContent("MoveTool", "|Move the selected objects."), EditorGUIUtility.IconContent("RotateTool", "|Rotate the selected objects."), EditorGUIUtility.IconContent("ScaleTool", "|Scale the selected objects."), EditorGUIUtility.IconContent("RectTool"), EditorGUIUtility.IconContent("MoveTool On"), EditorGUIUtility.IconContent("RotateTool On"), EditorGUIUtility.IconContent("ScaleTool On"), EditorGUIUtility.IconContent("RectTool On") };
                s_ViewToolIcons = new GUIContent[] { EditorGUIUtility.IconContent("ViewToolOrbit", "|Orbit the Scene view."), EditorGUIUtility.IconContent("ViewToolMove"), EditorGUIUtility.IconContent("ViewToolZoom"), EditorGUIUtility.IconContent("ViewToolOrbit", "|Orbit the Scene view."), EditorGUIUtility.IconContent("ViewToolOrbit On"), EditorGUIUtility.IconContent("ViewToolMove On"), EditorGUIUtility.IconContent("ViewToolZoom On"), EditorGUIUtility.IconContent("ViewToolOrbit On") };
                s_PivotIcons = new GUIContent[] { EditorGUIUtility.TextContentWithIcon("Center|The tool handle is placed at the center of the selection.", "ToolHandleCenter"), EditorGUIUtility.TextContentWithIcon("Pivot|The tool handle is placed at the active object's pivot point.", "ToolHandlePivot") };
                s_PivotRotation = new GUIContent[] { EditorGUIUtility.TextContentWithIcon("Local|Tool handles are in active object's rotation.", "ToolHandleLocal"), EditorGUIUtility.TextContentWithIcon("Global|Tool handles are in global rotation.", "ToolHandleGlobal") };
                s_LayerContent = EditorGUIUtility.TextContent("Layers|Which layers are visible in the Scene views.");
                s_PlayIcons = new GUIContent[] { EditorGUIUtility.IconContent("PlayButton"), EditorGUIUtility.IconContent("PauseButton"), EditorGUIUtility.IconContent("StepButton"), EditorGUIUtility.IconContent("PlayButtonProfile"), EditorGUIUtility.IconContent("PlayButton On"), EditorGUIUtility.IconContent("PauseButton On"), EditorGUIUtility.IconContent("StepButton On"), EditorGUIUtility.IconContent("PlayButtonProfile On"), EditorGUIUtility.IconContent("PlayButton Anim"), EditorGUIUtility.IconContent("PauseButton Anim"), EditorGUIUtility.IconContent("StepButton Anim"), EditorGUIUtility.IconContent("PlayButtonProfile Anim") };
                s_CloudIcon = EditorGUIUtility.IconContent("CloudConnect");
                s_AccountContent = new GUIContent("Account");
                s_CollabIcons = new GUIContent[] { EditorGUIUtility.TextContentWithIcon("Collab| You need to enable collab.", "CollabNew"), EditorGUIUtility.TextContentWithIcon("Collab| You are up to date.", "Collab"), EditorGUIUtility.TextContentWithIcon("Collab| Please fix your conflicts prior to publishing.", "CollabConflict"), EditorGUIUtility.TextContentWithIcon("Collab| Last operation failed. Please retry later.", "CollabError"), EditorGUIUtility.TextContentWithIcon("Collab| Please update, there are server changes.", "CollabPull"), EditorGUIUtility.TextContentWithIcon("Collab| You have files to publish.", "CollabPush"), EditorGUIUtility.TextContentWithIcon("Collab| Operation in progress.", "CollabProgress"), EditorGUIUtility.TextContentWithIcon("Collab| Collab is disabled.", "CollabNew"), EditorGUIUtility.TextContentWithIcon("Collab| Please check your network connection.", "CollabOffline") };
            }
        }

        private static void InternalWillTogglePlaymode()
        {
            InternalEditorUtility.RepaintAllViews();
        }

        public void OnCollabStateChanged(CollabInfo info)
        {
            this.UpdateCollabToolbarState();
        }

        public void OnDisable()
        {
            EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(this.Repaint));
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.OnSelectionChange));
            UnityConnect.instance.StateChanged -= new StateChangedDelegate(this.OnUnityConnectStateChanged);
            UnityConnect.instance.UserStateChanged -= new UserStateChangedDelegate(this.OnUnityConnectUserStateChanged);
            Collab.instance.StateChanged -= new StateChangedDelegate(this.OnCollabStateChanged);
            if (this.m_CollabButton != null)
            {
                this.m_CollabButton.Clear();
            }
        }

        public void OnEnable()
        {
            EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(this.Repaint));
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.OnSelectionChange));
            UnityConnect.instance.StateChanged += new StateChangedDelegate(this.OnUnityConnectStateChanged);
            UnityConnect.instance.UserStateChanged += new UserStateChangedDelegate(this.OnUnityConnectUserStateChanged);
            get = this;
            Collab.instance.StateChanged += new StateChangedDelegate(this.OnCollabStateChanged);
            if (this.m_CollabButton == null)
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = () => ((float) EditorApplication.timeSinceStartup) * 500f;
                }
                this.m_CollabButton = new ButtonWithAnimatedIconRotation(<>f__am$cache0, new Action(this.Repaint), 20f, true);
            }
        }

        protected override bool OnFocus() => 
            false;

        private void OnGUI()
        {
            float width = 10f;
            float num2 = 20f;
            float num3 = 32f;
            float num4 = 64f;
            float num5 = 80f;
            this.InitializeToolIcons();
            bool isPlayingOrWillChangePlaymode = EditorApplication.isPlayingOrWillChangePlaymode;
            if (isPlayingOrWillChangePlaymode)
            {
                GUI.color = (Color) HostView.kPlayModeDarken;
            }
            if (Event.current.type == EventType.Repaint)
            {
                Styles.appToolbar.Draw(new Rect(0f, 0f, base.position.width, base.position.height), false, false, false, false);
            }
            Rect pos = new Rect(0f, 0f, 0f, 0f);
            this.ReserveWidthRight(width, ref pos);
            this.ReserveWidthRight(num3 * 5f, ref pos);
            this.DoToolButtons(this.GetThickArea(pos));
            this.ReserveWidthRight(num2, ref pos);
            this.ReserveWidthRight(num4 * 2f, ref pos);
            this.DoPivotButtons(this.GetThinArea(pos));
            float num6 = 100f;
            pos = new Rect((float) Mathf.RoundToInt((base.position.width - num6) / 2f), 0f, 140f, 0f);
            GUILayout.BeginArea(this.GetThickArea(pos));
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            this.DoPlayButtons(isPlayingOrWillChangePlaymode);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            pos = new Rect(base.position.width, 0f, 0f, 0f);
            this.ReserveWidthLeft(width, ref pos);
            this.ReserveWidthLeft(num5, ref pos);
            this.DoLayoutDropDown(this.GetThinArea(pos));
            this.ReserveWidthLeft(width, ref pos);
            this.ReserveWidthLeft(num5, ref pos);
            this.DoLayersDropDown(this.GetThinArea(pos));
            this.ReserveWidthLeft(num2, ref pos);
            this.ReserveWidthLeft(num5, ref pos);
            if (EditorGUI.ButtonMouseDown(this.GetThinArea(pos), s_AccountContent, FocusType.Passive, Styles.dropdown))
            {
                this.ShowUserMenu(this.GetThinArea(pos));
            }
            this.ReserveWidthLeft(width, ref pos);
            this.ReserveWidthLeft(32f, ref pos);
            if (GUI.Button(this.GetThinArea(pos), s_CloudIcon))
            {
                UnityConnectServiceCollection.instance.ShowService("Hub", true);
            }
            this.ReserveWidthLeft(width, ref pos);
            this.ReserveWidthLeft(78f, ref pos);
            this.DoCollabDropDown(this.GetThinArea(pos));
            EditorGUI.ShowRepaints();
            Highlighter.ControlHighlightGUI(this);
        }

        private void OnSelectionChange()
        {
            Tools.OnSelectionChange();
            base.Repaint();
        }

        protected void OnUnityConnectStateChanged(ConnectInfo state)
        {
            this.UpdateCollabToolbarState();
            RepaintToolbar();
        }

        protected void OnUnityConnectUserStateChanged(UserInfo state)
        {
            this.UpdateCollabToolbarState();
        }

        internal static void RepaintToolbar()
        {
            if (get != null)
            {
                get.Repaint();
            }
        }

        private void ReserveBottom(float height, ref Rect pos)
        {
            pos.y += height;
        }

        private void ReserveRight(float width, ref Rect pos)
        {
            pos.x += width;
        }

        private void ReserveWidthLeft(float width, ref Rect pos)
        {
            pos.x -= width;
            pos.width = width;
        }

        private void ReserveWidthRight(float width, ref Rect pos)
        {
            pos.x += pos.width;
            pos.width = width;
        }

        private void ShowPopup(Rect rect)
        {
            AssetDatabase.SaveAssets();
            this.ReserveRight(39f, ref rect);
            this.ReserveBottom(5f, ref rect);
            if (CollabToolbarWindow.ShowCenteredAtPosition(rect))
            {
                GUIUtility.ExitGUI();
            }
        }

        private void ShowUserMenu(Rect dropDownRect)
        {
            GenericMenu menu = new GenericMenu();
            if (!UnityConnect.instance.online)
            {
                menu.AddDisabledItem(new GUIContent("Go to account"));
                menu.AddDisabledItem(new GUIContent("Sign in..."));
                if (!Application.HasProLicense())
                {
                    menu.AddSeparator("");
                    menu.AddDisabledItem(new GUIContent("Upgrade to Pro"));
                }
            }
            else
            {
                <ShowUserMenu>c__AnonStorey0 storey = new <ShowUserMenu>c__AnonStorey0 {
                    accountUrl = UnityConnect.instance.GetConfigurationURL(CloudConfigUrl.CloudPortal)
                };
                if (UnityConnect.instance.loggedIn)
                {
                    menu.AddItem(new GUIContent("Go to account"), false, new GenericMenu.MenuFunction(storey.<>m__0));
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Go to account"));
                }
                if (UnityConnect.instance.loggedIn)
                {
                    string text = "Sign out " + UnityConnect.instance.userInfo.displayName;
                    if (<>f__am$cache1 == null)
                    {
                        <>f__am$cache1 = () => UnityConnect.instance.Logout();
                    }
                    menu.AddItem(new GUIContent(text), false, <>f__am$cache1);
                }
                else
                {
                    if (<>f__am$cache2 == null)
                    {
                        <>f__am$cache2 = () => UnityConnect.instance.ShowLogin();
                    }
                    menu.AddItem(new GUIContent("Sign in..."), false, <>f__am$cache2);
                }
                if (!Application.HasProLicense())
                {
                    menu.AddSeparator("");
                    if (<>f__am$cache3 == null)
                    {
                        <>f__am$cache3 = () => Application.OpenURL("https://store.unity3d.com/");
                    }
                    menu.AddItem(new GUIContent("Upgrade to Pro"), false, <>f__am$cache3);
                }
            }
            menu.DropDown(dropDownRect);
        }

        private static void TogglePlaying()
        {
            bool flag = !EditorApplication.isPlaying;
            EditorApplication.isPlaying = flag;
            InternalWillTogglePlaymode();
        }

        public void UpdateCollabToolbarState()
        {
            CollabToolbarState upToDate = CollabToolbarState.UpToDate;
            bool flag = UnityConnect.instance.connectInfo.online && UnityConnect.instance.connectInfo.loggedIn;
            this.m_DynamicTooltip = "";
            if (flag)
            {
                Collab instance = Collab.instance;
                bool flag2 = instance.JobRunning(0);
                CollabInfo collabInfo = instance.collabInfo;
                if (!collabInfo.ready)
                {
                    upToDate = CollabToolbarState.InProgress;
                }
                else if (collabInfo.error)
                {
                    upToDate = CollabToolbarState.OperationError;
                    this.m_DynamicTooltip = "Last operation failed. " + collabInfo.lastErrorMsg;
                }
                else if (flag2)
                {
                    upToDate = CollabToolbarState.InProgress;
                }
                else
                {
                    bool flag3 = CollabAccess.Instance.IsServiceEnabled();
                    if (!UnityConnect.instance.projectInfo.projectBound || !flag3)
                    {
                        upToDate = CollabToolbarState.NeedToEnableCollab;
                    }
                    else if (collabInfo.update)
                    {
                        upToDate = CollabToolbarState.ServerHasChanges;
                    }
                    else if (collabInfo.conflict)
                    {
                        upToDate = CollabToolbarState.Conflict;
                    }
                    else if (collabInfo.publish)
                    {
                        upToDate = CollabToolbarState.FilesToPush;
                    }
                }
            }
            else
            {
                upToDate = CollabToolbarState.Offline;
            }
            if ((upToDate != this.m_CollabToolbarState) || (CollabToolbarWindow.s_ToolbarIsVisible == m_ShowCollabTooltip))
            {
                this.m_CollabToolbarState = upToDate;
                m_ShowCollabTooltip = !CollabToolbarWindow.s_ToolbarIsVisible;
                RepaintToolbar();
            }
        }

        private GUIContent currentCollabContent
        {
            get
            {
                GUIContent content = new GUIContent(s_CollabIcons[(int) this.m_CollabToolbarState]);
                if (!m_ShowCollabTooltip)
                {
                    content.tooltip = null;
                    return content;
                }
                if (this.m_DynamicTooltip != "")
                {
                    content.tooltip = this.m_DynamicTooltip;
                }
                return content;
            }
        }

        internal static string lastLoadedLayoutName
        {
            get => 
                (!string.IsNullOrEmpty(get.m_LastLoadedLayoutName) ? get.m_LastLoadedLayoutName : "Layout");
            set
            {
                get.m_LastLoadedLayoutName = value;
                get.Repaint();
            }
        }

        [CompilerGenerated]
        private sealed class <ShowUserMenu>c__AnonStorey0
        {
            internal string accountUrl;

            internal void <>m__0()
            {
                UnityConnect.instance.OpenAuthorizedURLInWebBrowser(this.accountUrl);
            }
        }

        private enum CollabToolbarState
        {
            NeedToEnableCollab,
            UpToDate,
            Conflict,
            OperationError,
            ServerHasChanges,
            FilesToPush,
            InProgress,
            Disabled,
            Offline
        }

        private static class Styles
        {
            public static readonly GUIStyle appToolbar;
            public static readonly GUIStyle collabButtonStyle;
            public static readonly GUIStyle dropdown;

            static Styles()
            {
                GUIStyle style = new GUIStyle("Dropdown") {
                    padding = { left = 0x18 }
                };
                collabButtonStyle = style;
                dropdown = "Dropdown";
                appToolbar = "AppToolbar";
            }
        }
    }
}

