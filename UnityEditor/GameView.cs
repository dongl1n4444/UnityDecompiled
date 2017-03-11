namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEditor.Modules;
    using UnityEditor.SceneManagement;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Scripting;

    [EditorWindowTitle(title="Game", useTypeNameAsIconName=true)]
    internal class GameView : EditorWindow, IHasCustomMenu, IGameViewSizeMenuUser
    {
        private const int kBorderSize = 5;
        private readonly Color kClearBlack = new Color(0f, 0f, 0f, 0f);
        private const float kMaxScale = 5f;
        private const float kMinScale = 1f;
        private const int kScaleLabelWidth = 30;
        private const int kScaleSliderMaxWidth = 150;
        private const int kScaleSliderMinWidth = 30;
        private const int kScaleSliderSnapThreshold = 4;
        private const float kScrollZoomSnapDelay = 0.2f;
        private readonly Vector2 kWarningSize = new Vector2(400f, 140f);
        [SerializeField]
        private bool m_ClearInEditMode = true;
        [SerializeField]
        private ColorSpace m_CurrentColorSpace = ColorSpace.Uninitialized;
        [SerializeField]
        private float m_defaultScale = -1f;
        [SerializeField]
        private bool m_Gizmos;
        [SerializeField]
        private Vector2 m_LastWindowPixelSize;
        [SerializeField]
        private bool[] m_LowResolutionForAspectRatios = new bool[0];
        [SerializeField]
        private bool m_MaximizeOnPlay;
        [SerializeField]
        private bool m_NoCameraWarning = true;
        [SerializeField]
        private int[] m_SelectedSizes = new int[0];
        private int m_SizeChangeID = -2147483648;
        [SerializeField]
        private bool m_Stats;
        private bool m_TargetClamped;
        [SerializeField]
        private int m_TargetDisplay;
        [SerializeField]
        private RenderTexture m_TargetTexture;
        [SerializeField]
        private ZoomableArea m_ZoomArea;
        private static List<GameView> s_GameViews = new List<GameView>();
        private static GameView s_LastFocusedGameView = null;
        private static double s_LastScrollTime;
        private static GUIStyle s_ResolutionWarningStyle;

        public GameView()
        {
            base.autoRepaintOnSceneChange = true;
            this.m_TargetDisplay = 0;
            this.InitializeZoomArea();
        }

        public virtual void AddItemsToMenu(GenericMenu menu)
        {
            if (RenderDoc.IsInstalled() && !RenderDoc.IsLoaded())
            {
                menu.AddItem(Styles.loadRenderDocContent, false, new GenericMenu.MenuFunction(this.LoadRenderDoc));
            }
            menu.AddItem(Styles.noCameraWarningContextMenuContent, this.m_NoCameraWarning, new GenericMenu.MenuFunction(this.ToggleNoCameraWarning));
            menu.AddItem(Styles.clearEveryFrameContextMenuContent, this.m_ClearInEditMode, new GenericMenu.MenuFunction(this.ToggleClearInEditMode));
        }

        private void AllowCursorLockAndHide(bool enable)
        {
            Unsupported.SetAllowCursorLock(enable);
            Unsupported.SetAllowCursorHide(enable);
        }

        private void ClearTargetTexture()
        {
            if (this.m_TargetTexture.IsCreated())
            {
                RenderTexture active = RenderTexture.active;
                RenderTexture.active = this.m_TargetTexture;
                GL.Clear(true, true, this.kClearBlack);
                RenderTexture.active = active;
            }
        }

        private void ConfigureTargetTexture(int width, int height)
        {
            bool flag = false;
            if ((this.m_TargetTexture != null) && (this.m_CurrentColorSpace != QualitySettings.activeColorSpace))
            {
                UnityEngine.Object.DestroyImmediate(this.m_TargetTexture);
            }
            if (this.m_TargetTexture == null)
            {
                this.m_CurrentColorSpace = QualitySettings.activeColorSpace;
                this.m_TargetTexture = new RenderTexture(0, 0, 0x18, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
                this.m_TargetTexture.name = "GameView RT";
                this.m_TargetTexture.filterMode = UnityEngine.FilterMode.Point;
                this.m_TargetTexture.hideFlags = HideFlags.HideAndDontSave;
                EditorGUIUtility.SetGUITextureBlitColorspaceSettings(EditorGUIUtility.GUITextureBlitColorspaceMaterial);
            }
            if ((this.m_TargetTexture.width != width) || (this.m_TargetTexture.height != height))
            {
                this.m_TargetTexture.Release();
                this.m_TargetTexture.width = width;
                this.m_TargetTexture.height = height;
                this.m_TargetTexture.antiAliasing = 1;
                flag = true;
                if (this.m_TargetClamped)
                {
                    object[] args = new object[] { width, height };
                    Debug.LogWarningFormat("GameView reduced to a reasonable size for this system ({0}x{1})", args);
                }
            }
            this.m_TargetTexture.Create();
            if (flag)
            {
                this.ClearTargetTexture();
            }
        }

        private void ConfigureZoomArea()
        {
            this.m_ZoomArea.rect = this.viewInWindow;
            this.m_ZoomArea.hBaseRangeMin = this.targetInContent.xMin;
            this.m_ZoomArea.vBaseRangeMin = this.targetInContent.yMin;
            this.m_ZoomArea.hBaseRangeMax = this.targetInContent.xMax;
            this.m_ZoomArea.vBaseRangeMax = this.targetInContent.yMax;
            float minScale = this.minScale;
            this.m_ZoomArea.vScaleMin = minScale;
            this.m_ZoomArea.hScaleMin = minScale;
            minScale = this.maxScale;
            this.m_ZoomArea.vScaleMax = minScale;
            this.m_ZoomArea.hScaleMax = minScale;
        }

        internal void CopyDimensionsToParentView()
        {
            if (base.m_Parent != null)
            {
                base.SetParentGameViewDimensions(this.targetInParent, this.clippedTargetInParent, this.targetSize);
            }
        }

        private float DefaultScaleForTargetInView(Vector2 targetToFit, Vector2 viewSize)
        {
            float f = this.ScaleThatFitsTargetInView(targetToFit, viewSize);
            if (f > 1f)
            {
                f = Mathf.Min(this.maxScale * EditorGUIUtility.pixelsPerPoint, (float) Mathf.FloorToInt(f));
            }
            return f;
        }

        private void DoToolbarGUI()
        {
            ScriptableSingleton<GameViewSizes>.instance.RefreshStandaloneAndRemoteDefaultSizes();
            if (ScriptableSingleton<GameViewSizes>.instance.GetChangeID() != this.m_SizeChangeID)
            {
                this.EnsureSelectedSizeAreValid();
                this.m_SizeChangeID = ScriptableSingleton<GameViewSizes>.instance.GetChangeID();
            }
            GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
            if (this.ShouldShowMultiDisplayOption())
            {
                GUILayoutOption[] optionArray1 = new GUILayoutOption[] { GUILayout.Width(80f) };
                int num = EditorGUILayout.Popup(this.m_TargetDisplay, DisplayUtility.GetDisplayNames(), EditorStyles.toolbarPopup, optionArray1);
                EditorGUILayout.Space();
                if (num != this.m_TargetDisplay)
                {
                    this.m_TargetDisplay = num;
                    this.UpdateZoomAreaAndParent();
                }
            }
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(160f) };
            EditorGUILayout.GameViewSizePopup(currentSizeGroupType, this.selectedSizeIndex, this, EditorStyles.toolbarPopup, options);
            this.DoZoomSlider();
            if (FrameDebuggerUtility.IsLocalEnabled())
            {
                GUILayout.FlexibleSpace();
                Color color = GUI.color;
                GUI.color *= UnityEditor.AnimationMode.animatedPropertyColor;
                GUILayout.Label(Styles.frameDebuggerOnContent, EditorStyles.miniLabel, new GUILayoutOption[0]);
                GUI.color = color;
                if (Event.current.type == EventType.Repaint)
                {
                    FrameDebuggerWindow.RepaintAll();
                }
            }
            GUILayout.FlexibleSpace();
            if (RenderDoc.IsLoaded())
            {
                using (new EditorGUI.DisabledScope(!RenderDoc.IsSupported()))
                {
                    if (GUILayout.Button(Styles.renderdocContent, EditorStyles.toolbarButton, new GUILayoutOption[0]))
                    {
                        base.m_Parent.CaptureRenderDoc();
                        GUIUtility.ExitGUI();
                    }
                }
            }
            this.m_MaximizeOnPlay = GUILayout.Toggle(this.m_MaximizeOnPlay, Styles.maximizeOnPlayContent, EditorStyles.toolbarButton, new GUILayoutOption[0]);
            EditorUtility.audioMasterMute = GUILayout.Toggle(EditorUtility.audioMasterMute, Styles.muteContent, EditorStyles.toolbarButton, new GUILayoutOption[0]);
            this.m_Stats = GUILayout.Toggle(this.m_Stats, Styles.statsContent, EditorStyles.toolbarButton, new GUILayoutOption[0]);
            Rect position = GUILayoutUtility.GetRect(Styles.gizmosContent, Styles.gizmoButtonStyle);
            Rect rect2 = new Rect(position.xMax - Styles.gizmoButtonStyle.border.right, position.y, (float) Styles.gizmoButtonStyle.border.right, position.height);
            if (EditorGUI.DropdownButton(rect2, GUIContent.none, FocusType.Passive, GUIStyle.none) && AnnotationWindow.ShowAtPosition(GUILayoutUtility.topLevel.GetLast(), true))
            {
                GUIUtility.ExitGUI();
            }
            this.m_Gizmos = GUI.Toggle(position, this.m_Gizmos, Styles.gizmosContent, Styles.gizmoButtonStyle);
            GUILayout.EndHorizontal();
        }

        private void DoZoomSlider()
        {
            GUILayout.Label(Styles.zoomSliderContent, EditorStyles.miniLabel, new GUILayoutOption[0]);
            EditorGUI.BeginChangeCheck();
            float num = Mathf.Log10(this.m_ZoomArea.scale.y);
            float leftValue = Mathf.Log10(this.minScale);
            float rightValue = Mathf.Log10(this.maxScale);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MaxWidth(150f), GUILayout.MinWidth(30f) };
            num = GUILayout.HorizontalSlider(num, leftValue, rightValue, options);
            if (EditorGUI.EndChangeCheck())
            {
                float newZoom = Mathf.Pow(10f, num);
                this.SnapZoom(newZoom);
            }
            GUIContent content = EditorGUIUtility.TempContent($"{this.m_ZoomArea.scale.y.ToString("G2")}x");
            content.tooltip = Styles.zoomSliderContent.tooltip;
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(30f) };
            GUILayout.Label(content, EditorStyles.miniLabel, optionArray2);
            content.tooltip = string.Empty;
        }

        private void EnforceZoomAreaConstraints()
        {
            Rect shownArea = this.m_ZoomArea.shownArea;
            if (shownArea.width > this.targetInContent.width)
            {
                shownArea.x = -0.5f * shownArea.width;
            }
            else
            {
                shownArea.x = Mathf.Clamp(shownArea.x, this.targetInContent.xMin, this.targetInContent.xMax - shownArea.width);
            }
            if (shownArea.height > this.targetInContent.height)
            {
                shownArea.y = -0.5f * shownArea.height;
            }
            else
            {
                shownArea.y = Mathf.Clamp(shownArea.y, this.targetInContent.yMin, this.targetInContent.yMax - shownArea.height);
            }
            this.m_ZoomArea.shownArea = shownArea;
        }

        private void EnsureSelectedSizeAreValid()
        {
            Array values = Enum.GetValues(typeof(GameViewSizeGroupType));
            if (this.m_SelectedSizes.Length != values.Length)
            {
                Array.Resize<int>(ref this.m_SelectedSizes, values.Length);
            }
            IEnumerator enumerator = values.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    GameViewSizeGroupType current = (GameViewSizeGroupType) enumerator.Current;
                    GameViewSizeGroup group = ScriptableSingleton<GameViewSizes>.instance.GetGroup(current);
                    int index = (int) current;
                    this.m_SelectedSizes[index] = Mathf.Clamp(this.m_SelectedSizes[index], 0, group.GetTotalCount() - 1);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            int length = this.m_LowResolutionForAspectRatios.Length;
            if (this.m_LowResolutionForAspectRatios.Length != values.Length)
            {
                Array.Resize<bool>(ref this.m_LowResolutionForAspectRatios, values.Length);
            }
            for (int i = length; i < values.Length; i++)
            {
                this.m_LowResolutionForAspectRatios[i] = GameViewSizes.DefaultLowResolutionSettingForSizeGroupType((GameViewSizeGroupType) values.GetValue(i));
            }
        }

        internal static GameView GetMainGameView()
        {
            if (((s_LastFocusedGameView == null) && (s_GameViews != null)) && (s_GameViews.Count > 0))
            {
                s_LastFocusedGameView = s_GameViews[0];
            }
            return s_LastFocusedGameView;
        }

        internal static Vector2 GetMainGameViewTargetSize()
        {
            GameView mainGameView = GetMainGameView();
            if ((mainGameView != null) && (mainGameView.m_Parent != null))
            {
                return mainGameView.targetSize;
            }
            return new Vector2(640f, 480f);
        }

        [RequiredByNativeCode]
        private static void GetMainGameViewTargetSizeNoBox(out Vector2 result)
        {
            result = GetMainGameViewTargetSize();
        }

        internal static Vector2 GetSizeOfMainGameView() => 
            GetMainGameViewTargetSize();

        private void InitializeZoomArea()
        {
            this.m_ZoomArea = new ZoomableArea(true, false);
            this.m_ZoomArea.uniformScale = true;
            this.m_ZoomArea.upDirection = ZoomableArea.YDirection.Negative;
        }

        public bool IsShowingGizmos() => 
            this.m_Gizmos;

        private void LoadRenderDoc()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                RenderDoc.Load();
                ShaderUtil.RecreateGfxDevice();
            }
        }

        public void OnDisable()
        {
            s_GameViews.Remove(this);
            if (this.m_TargetTexture != null)
            {
                UnityEngine.Object.DestroyImmediate(this.m_TargetTexture);
            }
        }

        public void OnEnable()
        {
            base.titleContent = base.GetLocalizedTitleContent();
            this.EnsureSelectedSizeAreValid();
            this.UpdateZoomAreaAndParent();
            base.dontClearBackground = true;
            s_GameViews.Add(this);
        }

        private void OnFocus()
        {
            this.AllowCursorLockAndHide(true);
            s_LastFocusedGameView = this;
            InternalEditorUtility.OnGameViewFocus(true);
        }

        private void OnGUI()
        {
            if ((base.position.size * EditorGUIUtility.pixelsPerPoint) != this.m_LastWindowPixelSize)
            {
                this.UpdateZoomAreaAndParent();
            }
            this.DoToolbarGUI();
            this.CopyDimensionsToParentView();
            EditorGUIUtility.AddCursorRect(this.viewInWindow, MouseCursor.CustomCursor);
            EventType type = Event.current.type;
            if ((type == EventType.MouseDown) && this.viewInWindow.Contains(Event.current.mousePosition))
            {
                this.AllowCursorLockAndHide(true);
            }
            else if ((type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Escape))
            {
                Unsupported.SetAllowCursorLock(false);
            }
            bool flag = EditorApplication.isPlaying && !EditorApplication.isPaused;
            this.m_ZoomArea.hSlider = !flag && (this.m_ZoomArea.shownArea.width < this.targetInContent.width);
            this.m_ZoomArea.vSlider = !flag && (this.m_ZoomArea.shownArea.height < this.targetInContent.height);
            this.m_ZoomArea.enableMouseInput = !flag;
            this.ConfigureZoomArea();
            if (flag)
            {
                GUIUtility.keyboardControl = 0;
            }
            Vector2 mousePosition = Event.current.mousePosition;
            Vector2 vector2 = this.WindowToGameMousePosition(mousePosition);
            GUI.color = Color.white;
            EventType type2 = Event.current.type;
            this.m_ZoomArea.BeginViewGUI();
            switch (type)
            {
                case EventType.Layout:
                case EventType.Used:
                    break;

                case EventType.Repaint:
                {
                    GUI.Box(this.m_ZoomArea.drawRect, GUIContent.none, Styles.gameViewBackgroundStyle);
                    Vector2 vector3 = GUIUtility.s_EditorScreenPointOffset;
                    GUIUtility.s_EditorScreenPointOffset = Vector2.zero;
                    SavedGUIState state = SavedGUIState.Create();
                    this.ConfigureTargetTexture((int) this.targetSize.x, (int) this.targetSize.y);
                    if (this.m_ClearInEditMode && !EditorApplication.isPlaying)
                    {
                        this.ClearTargetTexture();
                    }
                    int targetDisplay = 0;
                    if (this.ShouldShowMultiDisplayOption())
                    {
                        targetDisplay = this.m_TargetDisplay;
                    }
                    if (this.m_TargetTexture.IsCreated())
                    {
                        EditorGUIUtility.RenderGameViewCamerasInternal(this.m_TargetTexture, targetDisplay, GUIClip.Unclip(this.viewInWindow), vector2, this.m_Gizmos);
                        state.ApplyAndForget();
                        GUIUtility.s_EditorScreenPointOffset = vector3;
                        GUI.BeginGroup(this.m_ZoomArea.drawRect);
                        GL.sRGBWrite = this.m_CurrentColorSpace == ColorSpace.Linear;
                        Graphics.DrawTexture(this.deviceFlippedTargetInView, this.m_TargetTexture, new Rect(0f, 0f, 1f, 1f), 0, 0, 0, 0, GUI.color, EditorGUIUtility.GUITextureBlitColorspaceMaterial);
                        GL.sRGBWrite = false;
                        GUI.EndGroup();
                    }
                    break;
                }
                default:
                {
                    if (WindowLayout.s_MaximizeKey.activated && (!EditorApplication.isPlaying || EditorApplication.isPaused))
                    {
                        return;
                    }
                    bool flag2 = this.viewInWindow.Contains(Event.current.mousePosition);
                    if ((Event.current.rawType == EventType.MouseDown) && !flag2)
                    {
                        return;
                    }
                    Event.current.mousePosition = vector2;
                    Event.current.displayIndex = this.m_TargetDisplay;
                    EditorGUIUtility.QueueGameViewInputEvent(Event.current);
                    bool flag3 = true;
                    if ((Event.current.rawType == EventType.MouseUp) && !flag2)
                    {
                        flag3 = false;
                    }
                    switch (type)
                    {
                        case EventType.ExecuteCommand:
                        case EventType.ValidateCommand:
                            flag3 = false;
                            break;
                    }
                    if (flag3)
                    {
                        Event.current.Use();
                    }
                    else
                    {
                        Event.current.mousePosition = mousePosition;
                    }
                    break;
                }
            }
            this.m_ZoomArea.EndViewGUI();
            if ((type2 == EventType.ScrollWheel) && (Event.current.type == EventType.Used))
            {
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.SnapZoomDelayed));
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.SnapZoomDelayed));
                s_LastScrollTime = EditorApplication.timeSinceStartup;
            }
            this.EnforceZoomAreaConstraints();
            if (this.m_TargetTexture != null)
            {
                if (this.m_ZoomArea.scale.y < 1f)
                {
                    this.m_TargetTexture.filterMode = UnityEngine.FilterMode.Bilinear;
                }
                else
                {
                    this.m_TargetTexture.filterMode = UnityEngine.FilterMode.Point;
                }
            }
            if (this.m_NoCameraWarning && !EditorGUIUtility.IsDisplayReferencedByCameras(this.m_TargetDisplay))
            {
                GUI.Label(this.warningPosition, GUIContent.none, EditorStyles.notificationBackground);
                string str = !this.ShouldShowMultiDisplayOption() ? string.Empty : DisplayUtility.GetDisplayNames()[this.m_TargetDisplay].text;
                string t = $"{str}
No cameras rendering";
                EditorGUI.DoDropShadowLabel(this.warningPosition, EditorGUIUtility.TempContent(t), EditorStyles.notificationText, 0.3f);
            }
            if (this.m_Stats)
            {
                GameViewGUI.GameViewStatsGUI();
            }
        }

        private void OnLostFocus()
        {
            if (!EditorApplicationLayout.IsInitializingPlaymodeLayout())
            {
                this.AllowCursorLockAndHide(false);
            }
            InternalEditorUtility.OnGameViewFocus(false);
        }

        private void OnSelectionChange()
        {
            if (this.m_Gizmos)
            {
                base.Repaint();
            }
        }

        public void OnValidate()
        {
            this.EnsureSelectedSizeAreValid();
        }

        public static void RepaintAll()
        {
            if (s_GameViews != null)
            {
                foreach (GameView view in s_GameViews)
                {
                    view.Repaint();
                }
            }
        }

        private float ScaleThatFitsTargetInView(Vector2 targetInPixels, Vector2 viewInPoints)
        {
            Vector2 vector = EditorGUIUtility.PixelsToPoints(targetInPixels);
            Vector2 vector2 = new Vector2(viewInPoints.x / vector.x, viewInPoints.y / vector.y);
            return Mathf.Min(vector2.x, vector2.y);
        }

        private bool ShouldShowMultiDisplayOption()
        {
            GUIContent[] displayNames = ModuleManager.GetDisplayNames(EditorUserBuildSettings.activeBuildTarget.ToString());
            return ((BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget) == BuildTargetGroup.Standalone) || (displayNames != null));
        }

        public void SizeSelectionCallback(int indexClicked, object objectSelected)
        {
            if (indexClicked != this.selectedSizeIndex)
            {
                this.selectedSizeIndex = indexClicked;
                base.dontClearBackground = true;
                this.UpdateZoomAreaAndParent();
            }
        }

        private void SnapZoom(float newZoom)
        {
            float num = Mathf.Log10(newZoom);
            float num2 = Mathf.Log10(this.minScale);
            float num3 = Mathf.Log10(this.maxScale);
            float maxValue = float.MaxValue;
            if ((num > num2) && (num < num3))
            {
                for (int i = 1; i <= this.maxScale; i++)
                {
                    float num6 = (150f * Mathf.Abs((float) (num - Mathf.Log10((float) i)))) / (num3 - num2);
                    if ((num6 < 4f) && (num6 < maxValue))
                    {
                        newZoom = i;
                        maxValue = num6;
                    }
                }
            }
            Rect shownAreaInsideMargins = this.m_ZoomArea.shownAreaInsideMargins;
            Vector2 focalPoint = shownAreaInsideMargins.position + ((Vector2) (shownAreaInsideMargins.size * 0.5f));
            this.m_ZoomArea.SetScaleFocused(focalPoint, (Vector2) (Vector2.one * newZoom));
        }

        private void SnapZoomDelayed()
        {
            if (EditorApplication.timeSinceStartup > (s_LastScrollTime + 0.20000000298023224))
            {
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.SnapZoomDelayed));
                this.SnapZoom(this.m_ZoomArea.scale.y);
                base.Repaint();
            }
        }

        private void ToggleClearInEditMode()
        {
            this.m_ClearInEditMode = !this.m_ClearInEditMode;
        }

        private void ToggleNoCameraWarning()
        {
            this.m_NoCameraWarning = !this.m_NoCameraWarning;
        }

        private void UpdateZoomAreaAndParent()
        {
            bool flag = Mathf.Approximately(this.m_ZoomArea.scale.y, this.m_defaultScale);
            this.ConfigureZoomArea();
            this.m_defaultScale = this.DefaultScaleForTargetInView(this.targetSize, this.viewInWindow.size);
            if (flag)
            {
                this.m_ZoomArea.SetTransform(Vector2.zero, (Vector2) (Vector2.one * this.m_defaultScale));
                this.EnforceZoomAreaConstraints();
            }
            this.CopyDimensionsToParentView();
            this.m_LastWindowPixelSize = (Vector2) (base.position.size * EditorGUIUtility.pixelsPerPoint);
            EditorApplication.SetSceneRepaintDirty();
        }

        private Vector2 WindowToGameMousePosition(Vector2 windowMousePosition) => 
            ((Vector2) ((windowMousePosition + this.gameMouseOffset) * this.gameMouseScale));

        private Rect clippedTargetInParent =>
            Rect.MinMaxRect(Mathf.Max(this.targetInParent.xMin, this.viewInParent.xMin), Mathf.Max(this.targetInParent.yMin, this.viewInParent.yMin), Mathf.Min(this.targetInParent.xMax, this.viewInParent.xMax), Mathf.Min(this.targetInParent.yMax, this.viewInParent.yMax));

        private GameViewSize currentGameViewSize =>
            ScriptableSingleton<GameViewSizes>.instance.currentGroup.GetGameViewSize(this.selectedSizeIndex);

        private static GameViewSizeGroupType currentSizeGroupType =>
            ScriptableSingleton<GameViewSizes>.instance.currentGroupType;

        private Rect deviceFlippedTargetInView
        {
            get
            {
                if (!SystemInfo.graphicsUVStartsAtTop)
                {
                    return this.targetInView;
                }
                Rect targetInView = this.targetInView;
                targetInView.y += targetInView.height;
                targetInView.height = -targetInView.height;
                return targetInView;
            }
        }

        public bool forceLowResolutionAspectRatios =>
            (EditorGUIUtility.pixelsPerPoint == 1f);

        private Vector2 gameMouseOffset =>
            (-this.viewInWindow.position - this.targetInView.position);

        private float gameMouseScale =>
            (EditorGUIUtility.pixelsPerPoint / this.m_ZoomArea.scale.y);

        public bool lowResolutionForAspectRatios
        {
            get => 
                this.m_LowResolutionForAspectRatios[(int) currentSizeGroupType];
            set
            {
                if (value != this.m_LowResolutionForAspectRatios[(int) currentSizeGroupType])
                {
                    this.m_LowResolutionForAspectRatios[(int) currentSizeGroupType] = value;
                    this.UpdateZoomAreaAndParent();
                }
            }
        }

        public bool maximizeOnPlay
        {
            get => 
                this.m_MaximizeOnPlay;
            set
            {
                this.m_MaximizeOnPlay = value;
            }
        }

        private float maxScale =>
            Mathf.Max(5f * EditorGUIUtility.pixelsPerPoint, this.ScaleThatFitsTargetInView(this.targetSize, this.viewInWindow.size));

        private float minScale
        {
            get
            {
                float a = Mathf.Min(1f, this.ScaleThatFitsTargetInView(this.targetSize, this.viewInWindow.size));
                if (this.m_LowResolutionForAspectRatios[(int) currentSizeGroupType] && (this.currentGameViewSize.sizeType == GameViewSizeType.AspectRatio))
                {
                    a = Mathf.Max(a, EditorGUIUtility.pixelsPerPoint);
                }
                return a;
            }
        }

        private int selectedSizeIndex
        {
            get => 
                this.m_SelectedSizes[(int) currentSizeGroupType];
            set
            {
                this.m_SelectedSizes[(int) currentSizeGroupType] = value;
            }
        }

        public bool showLowResolutionToggle =>
            EditorApplication.supportsHiDPI;

        private Rect targetInContent =>
            EditorGUIUtility.PixelsToPoints(new Rect((Vector2) (-0.5f * this.targetSize), this.targetSize));

        private Rect targetInParent =>
            new Rect(this.targetInView.position + this.viewInParent.position, this.targetInView.size);

        private Rect targetInView =>
            new Rect(this.m_ZoomArea.DrawingToViewTransformPoint(this.targetInContent.position), this.m_ZoomArea.DrawingToViewTransformVector(this.targetInContent.size));

        internal Vector2 targetSize
        {
            get
            {
                Rect startRect = !this.m_LowResolutionForAspectRatios[(int) currentSizeGroupType] ? EditorGUIUtility.PointsToPixels(this.viewInWindow) : this.viewInWindow;
                return GameViewSizes.GetRenderTargetSize(startRect, currentSizeGroupType, this.selectedSizeIndex, out this.m_TargetClamped);
            }
        }

        private Rect viewInParent
        {
            get
            {
                Rect viewInWindow = this.viewInWindow;
                RectOffset borderSize = base.m_Parent.borderSize;
                viewInWindow.x += borderSize.left;
                viewInWindow.y += borderSize.top + borderSize.bottom;
                return viewInWindow;
            }
        }

        private Rect viewInWindow =>
            new Rect(0f, 17f, base.position.width, base.position.height - 17f);

        private Rect warningPosition =>
            new Rect((Vector2) ((this.viewInWindow.size - this.kWarningSize) * 0.5f), this.kWarningSize);

        internal static class Styles
        {
            public static GUIContent clearEveryFrameContextMenuContent = EditorGUIUtility.TextContent("Clear Every Frame in Edit Mode");
            public static GUIContent frameDebuggerOnContent = EditorGUIUtility.TextContent("Frame Debugger On");
            public static GUIStyle gameViewBackgroundStyle = "GameViewBackground";
            public static GUIStyle gizmoButtonStyle = "GV Gizmo DropDown";
            public static GUIContent gizmosContent = EditorGUIUtility.TextContent("Gizmos");
            public static GUIContent loadRenderDocContent = EditorGUIUtility.TextContent("Load RenderDoc");
            public static GUIContent lowResAspectRatiosContextMenuContent = EditorGUIUtility.TextContent("Low Resolution Aspect Ratios");
            public static GUIContent maximizeOnPlayContent = EditorGUIUtility.TextContent("Maximize On Play");
            public static GUIContent muteContent = EditorGUIUtility.TextContent("Mute Audio");
            public static GUIContent noCameraWarningContextMenuContent = EditorGUIUtility.TextContent("Warn if No Cameras Rendering");
            public static GUIContent renderdocContent = EditorGUIUtility.IconContent("renderdoc", "Capture|Capture the current view and open in RenderDoc.");
            public static GUIContent statsContent = EditorGUIUtility.TextContent("Stats");
            public static GUIContent zoomSliderContent = EditorGUIUtility.TextContent("Scale|Size of the game view on the screen.");
        }
    }
}

