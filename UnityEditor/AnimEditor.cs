namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class AnimEditor : ScriptableObject
    {
        [CompilerGenerated]
        private static Predicate<AnimationWindowSelectionItem> <>f__am$cache0;
        internal static PrefKey kAnimationFirstKey = new PrefKey("Animation/First Keyframe", "#,");
        internal static PrefKey kAnimationLastKey = new PrefKey("Animation/Last Keyframe", "#.");
        internal static PrefKey kAnimationNextFrame = new PrefKey("Animation/Next Frame", ".");
        internal static PrefKey kAnimationNextKeyframe = new PrefKey("Animation/Next Keyframe", "&.");
        internal static PrefKey kAnimationPlayToggle = new PrefKey("Animation/Play Animation", " ");
        internal static PrefKey kAnimationPrevFrame = new PrefKey("Animation/Previous Frame", ",");
        internal static PrefKey kAnimationPrevKeyframe = new PrefKey("Animation/Previous Keyframe", "&,");
        internal static PrefKey kAnimationRecordKeyframe = new PrefKey("Animation/Record Keyframe", "k");
        internal static PrefKey kAnimationShowCurvesToggle = new PrefKey("Animation/Show Curves", "c");
        internal const float kDisabledRulerAlpha = 0.12f;
        internal static PrefColor kEulerXColor = new PrefColor("Testing/EulerX", 1f, 0f, 1f, 1f);
        internal static PrefColor kEulerYColor = new PrefColor("Testing/EulerY", 1f, 1f, 0f, 1f);
        internal static PrefColor kEulerZColor = new PrefColor("Testing/EulerZ", 0f, 1f, 1f, 1f);
        internal const int kHierarchyMinWidth = 300;
        internal const int kIntFieldWidth = 0x23;
        internal const int kLayoutRowHeight = 0x12;
        internal const int kSliderThickness = 15;
        internal const int kToggleButtonWidth = 80;
        [SerializeField]
        private AnimationWindowClipPopup m_ClipPopup;
        [SerializeField]
        private CurveEditor m_CurveEditor;
        [SerializeField]
        private DopeSheetEditor m_DopeSheet;
        [SerializeField]
        private AnimationEventTimeLine m_Events;
        [SerializeField]
        private AnimationWindowHierarchy m_Hierarchy;
        [SerializeField]
        private SplitterState m_HorizontalSplitter;
        [NonSerialized]
        private bool m_Initialized;
        [SerializeField]
        private AnimEditorOverlay m_Overlay;
        [SerializeField]
        private EditorWindow m_OwnerWindow;
        [NonSerialized]
        private Rect m_Position;
        [NonSerialized]
        private float m_PreviousUpdateTime;
        [SerializeField]
        private AnimationWindowState m_State;
        [NonSerialized]
        private bool m_StylesInitialized;
        [NonSerialized]
        private bool m_TriggerFraming;
        private static List<AnimEditor> s_AnimationWindows = new List<AnimEditor>();

        private void AddEventButtonOnGUI()
        {
            AnimationWindowSelectionItem selectedItem = this.m_State.selectedItem;
            if (selectedItem != null)
            {
                using (new EditorGUI.DisabledScope(!selectedItem.animationIsEditable))
                {
                    if (GUILayout.Button(AnimationWindowStyles.addEventContent, EditorStyles.toolbarButton, new GUILayoutOption[0]))
                    {
                        this.m_Events.AddEvent(this.m_State.currentTime - selectedItem.timeOffset, selectedItem.rootGameObject, selectedItem.animationClip);
                    }
                }
            }
        }

        private void AddKeyframeButtonOnGUI()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = selectedItem => selectedItem.animationIsEditable;
            }
            bool flag = (bool) this.m_State.selection.Find(<>f__am$cache0);
            using (new EditorGUI.DisabledScope(!flag))
            {
                if (GUILayout.Button(AnimationWindowStyles.addKeyframeContent, EditorStyles.toolbarButton, new GUILayoutOption[0]))
                {
                    this.SaveCurveEditorKeySelection();
                    AnimationKeyTime time = AnimationKeyTime.Time(this.m_State.currentTime, this.m_State.frameRate);
                    AnimationWindowUtility.AddSelectedKeyframes(this.m_State, time);
                    this.UpdateSelectedKeysToCurveEditor();
                }
            }
        }

        private void ClipSelectionDropDownOnGUI()
        {
            this.m_ClipPopup.OnGUI();
        }

        private void CurveEditorOnGUI(Rect position)
        {
            if (Event.current.type == EventType.Repaint)
            {
                this.m_CurveEditor.rect = position;
                this.m_CurveEditor.SetTickMarkerRanges();
            }
            if (this.m_TriggerFraming && (Event.current.type == EventType.Repaint))
            {
                this.m_CurveEditor.FrameClip(true, true);
                this.m_TriggerFraming = false;
            }
            Rect rect = new Rect(position.xMin, position.yMin, position.width - 15f, position.height - 15f);
            this.m_CurveEditor.vSlider = this.m_State.showCurveEditor;
            this.m_CurveEditor.hSlider = this.m_State.showCurveEditor;
            if (Event.current.type == EventType.Layout)
            {
                this.UpdateCurveEditorData();
            }
            this.m_CurveEditor.BeginViewGUI();
            if (!this.m_State.disabled)
            {
                GUI.Box(rect, GUIContent.none, AnimationWindowStyles.curveEditorBackground);
                this.m_CurveEditor.GridGUI();
            }
            EditorGUI.BeginChangeCheck();
            this.m_CurveEditor.CurveGUI();
            if (EditorGUI.EndChangeCheck())
            {
                this.SaveChangedCurvesFromCurveEditor();
            }
            this.m_CurveEditor.EndViewGUI();
        }

        private void DopeSheetOnGUI(Rect position)
        {
            Rect rect = new Rect(position.xMin, position.yMin, position.width - 15f, position.height);
            if (Event.current.type == EventType.Repaint)
            {
                this.m_DopeSheet.rect = rect;
                this.m_DopeSheet.SetTickMarkerRanges();
                this.m_DopeSheet.RecalculateBounds();
            }
            if (!this.m_State.showCurveEditor)
            {
                if (this.triggerFraming && (Event.current.type == EventType.Repaint))
                {
                    this.m_DopeSheet.FrameClip();
                    this.triggerFraming = false;
                }
                Rect rect2 = new Rect(position.xMin, position.yMin, position.width - 15f, position.height - 15f);
                Rect rect3 = new Rect(rect2.xMin, rect2.yMin, rect2.width, 16f);
                this.m_DopeSheet.BeginViewGUI();
                GUI.Label(position, GUIContent.none, AnimationWindowStyles.dopeSheetBackground);
                if (!this.m_State.disabled)
                {
                    this.m_DopeSheet.TimeRuler(rect2, this.m_State.frameRate, false, true, 0.12f, this.m_State.timeFormat);
                    this.m_DopeSheet.DrawMasterDopelineBackground(rect3);
                }
                this.m_DopeSheet.OnGUI(rect2, (Vector2) (this.m_State.hierarchyState.scrollPos * -1f));
                this.m_DopeSheet.EndViewGUI();
                Rect rect4 = new Rect(rect.xMax, rect.yMin, 15f, rect2.height);
                float height = this.m_Hierarchy.GetTotalRect().height;
                float bottomValue = Mathf.Max(height, this.m_Hierarchy.GetContentSize().y);
                this.m_State.hierarchyState.scrollPos.y = GUI.VerticalScrollbar(rect4, this.m_State.hierarchyState.scrollPos.y, height, 0f, bottomValue);
            }
        }

        private void EventLineOnGUI(Rect eventsRect)
        {
            eventsRect.width -= 15f;
            GUI.Label(eventsRect, GUIContent.none, AnimationWindowStyles.eventBackground);
            using (new EditorGUI.DisabledScope((this.m_State.selectedItem == null) || !this.m_State.selectedItem.animationIsEditable))
            {
                this.m_Events.EventLineGUI(eventsRect, this.m_State);
            }
        }

        private void FrameRateInputFieldOnGUI()
        {
            GUILayout.Label(AnimationWindowStyles.samples, AnimationWindowStyles.toolbarLabel, new GUILayoutOption[0]);
            EditorGUI.BeginChangeCheck();
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(35f) };
            int num = EditorGUILayout.IntField((int) this.m_State.clipFrameRate, EditorStyles.toolbarTextField, options);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_State.clipFrameRate = num;
            }
        }

        public static List<AnimEditor> GetAllAnimationWindows()
        {
            return s_AnimationWindows;
        }

        private void HandleCopyPaste()
        {
            if ((Event.current.type == EventType.ValidateCommand) || (Event.current.type == EventType.ExecuteCommand))
            {
                switch (Event.current.commandName)
                {
                    case "Copy":
                        if (Event.current.type == EventType.ExecuteCommand)
                        {
                            this.m_State.CopyKeys();
                        }
                        Event.current.Use();
                        break;

                    case "Paste":
                        if (Event.current.type == EventType.ExecuteCommand)
                        {
                            this.SaveCurveEditorKeySelection();
                            this.m_State.PasteKeys();
                            this.UpdateSelectedKeysToCurveEditor();
                        }
                        Event.current.Use();
                        break;
                }
            }
        }

        private void HandleHotKeys()
        {
            if (GUI.enabled && !this.m_State.disabled)
            {
                bool flag = false;
                if (kAnimationPrevKeyframe.activated)
                {
                    this.MoveToPreviousKeyframe();
                    flag = true;
                }
                if (kAnimationNextKeyframe.activated)
                {
                    this.MoveToNextKeyframe();
                    flag = true;
                }
                if (kAnimationNextFrame.activated)
                {
                    this.m_State.frame++;
                    flag = true;
                }
                if (kAnimationPrevFrame.activated)
                {
                    this.m_State.frame--;
                    flag = true;
                }
                if (kAnimationFirstKey.activated)
                {
                    this.MoveToFirstKeyframe();
                    flag = true;
                }
                if (kAnimationLastKey.activated)
                {
                    this.MoveToLastKeyframe();
                    flag = true;
                }
                if (flag)
                {
                    Event.current.Use();
                    this.Repaint();
                }
                if (kAnimationPlayToggle.activated)
                {
                    this.m_State.playing = !this.m_State.playing;
                    this.m_PreviousUpdateTime = Time.realtimeSinceStartup;
                    Event.current.Use();
                }
                if (kAnimationPlayToggle.activated)
                {
                    this.m_State.playing = !this.m_State.playing;
                    this.m_PreviousUpdateTime = Time.realtimeSinceStartup;
                    Event.current.Use();
                }
                if (kAnimationRecordKeyframe.activated)
                {
                    this.SaveCurveEditorKeySelection();
                    AnimationKeyTime time = AnimationKeyTime.Time(this.m_State.currentTime, this.m_State.frameRate);
                    AnimationWindowUtility.AddSelectedKeyframes(this.m_State, time);
                    this.UpdateSelectedKeysToCurveEditor();
                    Event.current.Use();
                }
            }
        }

        private void HierarchyOnGUI()
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandHeight(true) };
            Rect position = GUILayoutUtility.GetRect(this.hierarchyWidth, this.hierarchyWidth, 0f, float.MaxValue, options);
            if (!this.m_State.disabled)
            {
                this.m_Hierarchy.OnGUI(position);
            }
        }

        private void Initialize()
        {
            AnimationWindowStyles.Initialize();
            this.InitializeHierarchy();
            this.m_CurveEditor.state = this.m_State;
            this.m_HorizontalSplitter.realSizes[0] = 300;
            this.m_HorizontalSplitter.realSizes[1] = (int) Mathf.Max((float) (this.m_Position.width - 300f), (float) 300f);
            this.m_DopeSheet.rect = new Rect(0f, 0f, this.contentWidth, 100f);
            this.m_Initialized = true;
        }

        private void InitializeClipSelection()
        {
            this.m_ClipPopup = new AnimationWindowClipPopup();
        }

        private void InitializeCurveEditor()
        {
            this.m_CurveEditor = new CurveEditor(new Rect(0f, 0f, this.contentWidth, 100f), new CurveWrapper[0], false);
            CurveEditorSettings settings = new CurveEditorSettings {
                hTickStyle = { 
                    distMin = 30,
                    distFull = 80,
                    distLabel = 0
                }
            };
            if (EditorGUIUtility.isProSkin)
            {
                settings.vTickStyle.tickColor.color = new Color(1f, 1f, 1f, settings.vTickStyle.tickColor.color.a);
                settings.vTickStyle.labelColor.color = new Color(1f, 1f, 1f, settings.vTickStyle.labelColor.color.a);
            }
            settings.vTickStyle.distMin = 15;
            settings.vTickStyle.distFull = 40;
            settings.vTickStyle.distLabel = 30;
            settings.vTickStyle.stubs = true;
            settings.hRangeMin = 0f;
            settings.hRangeLocked = false;
            settings.vRangeLocked = false;
            settings.hSlider = true;
            settings.vSlider = true;
            settings.allowDeleteLastKeyInCurve = true;
            settings.rectangleToolFlags = CurveEditorSettings.RectangleToolFlags.FullRectangleTool;
            settings.undoRedoSelection = true;
            this.m_CurveEditor.shownArea = new Rect(1f, 1f, 1f, 1f);
            this.m_CurveEditor.settings = settings;
            this.m_CurveEditor.state = this.m_State;
        }

        private void InitializeDopeSheet()
        {
            this.m_DopeSheet = new DopeSheetEditor(this.m_OwnerWindow);
            this.m_DopeSheet.SetTickMarkerRanges();
            this.m_DopeSheet.hSlider = true;
            this.m_DopeSheet.shownArea = new Rect(1f, 1f, 1f, 1f);
            this.m_DopeSheet.rect = new Rect(0f, 0f, this.contentWidth, 100f);
            this.m_DopeSheet.hTicks.SetTickModulosForFrameRate(this.m_State.frameRate);
        }

        private void InitializeEvents()
        {
            this.m_Events = new AnimationEventTimeLine(this.m_OwnerWindow);
        }

        private void InitializeHierarchy()
        {
            this.m_Hierarchy = new AnimationWindowHierarchy(this.m_State, this.m_OwnerWindow, new Rect(0f, 0f, this.hierarchyWidth, 100f));
        }

        private void InitializeHorizontalSplitter()
        {
            float[] relativeSizes = new float[] { 300f, 900f };
            int[] minSizes = new int[] { 300, 300 };
            this.m_HorizontalSplitter = new SplitterState(relativeSizes, minSizes, null);
            this.m_HorizontalSplitter.realSizes[0] = 300;
            this.m_HorizontalSplitter.realSizes[1] = 300;
        }

        private void InitializeNonserializedValues()
        {
            this.m_State.onFrameRateChange = (Action<float>) Delegate.Combine(this.m_State.onFrameRateChange, delegate (float newFrameRate) {
                this.m_CurveEditor.invSnap = newFrameRate;
                this.m_CurveEditor.hTicks.SetTickModulosForFrameRate(newFrameRate);
            });
        }

        private void InitializeOverlay()
        {
            this.m_Overlay = new AnimEditorOverlay();
        }

        private void MainContentOnGUI(Rect contentLayoutRect)
        {
            if (this.m_State.animatorIsOptimized)
            {
                Vector2 vector = GUI.skin.label.CalcSize(AnimationWindowStyles.animatorOptimizedText);
                Rect position = new Rect((contentLayoutRect.x + (contentLayoutRect.width * 0.5f)) - (vector.x * 0.5f), (contentLayoutRect.y + (contentLayoutRect.height * 0.5f)) - (vector.y * 0.5f), vector.x, vector.y);
                GUI.Label(position, AnimationWindowStyles.animatorOptimizedText);
            }
            else
            {
                if (this.m_State.disabled)
                {
                    this.SetupWizardOnGUI(contentLayoutRect);
                }
                else if (this.m_State.showCurveEditor)
                {
                    this.CurveEditorOnGUI(contentLayoutRect);
                }
                else
                {
                    this.DopeSheetOnGUI(contentLayoutRect);
                }
                this.HandleCopyPaste();
            }
        }

        private void MoveToFirstKeyframe()
        {
            if (this.m_State.activeAnimationClip != null)
            {
                this.m_State.currentTime = this.m_State.activeAnimationClip.startTime;
            }
        }

        private void MoveToLastKeyframe()
        {
            if (this.m_State.activeAnimationClip != null)
            {
                this.m_State.currentTime = this.m_State.activeAnimationClip.stopTime;
            }
        }

        private void MoveToNextKeyframe()
        {
            float time = AnimationWindowUtility.GetNextKeyframeTime(((!this.m_State.showCurveEditor || (this.m_State.activeCurves.Count <= 0)) ? this.m_State.allCurves : this.m_State.activeCurves).ToArray(), this.m_State.currentTime, this.m_State.clipFrameRate);
            this.m_State.currentTime = this.m_State.SnapToFrame(time, AnimationWindowState.SnapMode.SnapToClipFrame);
        }

        private void MoveToPreviousKeyframe()
        {
            float time = AnimationWindowUtility.GetPreviousKeyframeTime(((!this.m_State.showCurveEditor || (this.m_State.activeCurves.Count <= 0)) ? this.m_State.allCurves : this.m_State.activeCurves).ToArray(), this.m_State.currentTime, this.m_State.clipFrameRate);
            this.m_State.currentTime = this.m_State.SnapToFrame(time, AnimationWindowState.SnapMode.SnapToClipFrame);
        }

        public void OnAnimEditorGUI(EditorWindow parent, Rect position)
        {
            this.m_DopeSheet.m_Owner = parent;
            this.m_OwnerWindow = parent;
            this.m_Position = position;
            if (!this.m_Initialized)
            {
                this.Initialize();
            }
            this.m_State.OnGUI();
            this.SynchronizeLayout();
            using (new EditorGUI.DisabledScope(this.m_State.disabled || this.m_State.animatorIsOptimized))
            {
                this.OverlayEventOnGUI();
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                SplitterGUILayout.BeginHorizontalSplit(this.m_HorizontalSplitter, new GUILayoutOption[0]);
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                GUILayout.BeginHorizontal(EditorStyles.toolbarButton, new GUILayoutOption[0]);
                this.PlayControlsOnGUI();
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.toolbarButton, new GUILayoutOption[0]);
                this.ClipSelectionDropDownOnGUI();
                GUILayout.FlexibleSpace();
                this.FrameRateInputFieldOnGUI();
                this.AddKeyframeButtonOnGUI();
                this.AddEventButtonOnGUI();
                GUILayout.EndHorizontal();
                this.HierarchyOnGUI();
                GUILayout.BeginHorizontal(AnimationWindowStyles.miniToolbar, new GUILayoutOption[0]);
                this.TabSelectionOnGUI();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                Rect timeRulerRect = GUILayoutUtility.GetRect(this.contentWidth, 18f);
                Rect rect = GUILayoutUtility.GetRect(this.contentWidth, 18f);
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandHeight(true) };
                Rect contentLayoutRect = GUILayoutUtility.GetRect(this.contentWidth, this.contentWidth, 0f, float.MaxValue, options);
                this.MainContentOnGUI(contentLayoutRect);
                this.TimeRulerOnGUI(timeRulerRect);
                this.EventLineOnGUI(rect);
                GUILayout.EndVertical();
                SplitterGUILayout.EndHorizontalSplit();
                GUILayout.EndHorizontal();
                this.OverlayOnGUI(contentLayoutRect);
                this.RenderEventTooltip();
            }
            this.HandleHotKeys();
            this.PostLayoutChanges();
        }

        public void OnDestroy()
        {
            if (this.m_CurveEditor != null)
            {
                this.m_CurveEditor.OnDestroy();
            }
            Object.DestroyImmediate(this.m_State);
        }

        public void OnDisable()
        {
            s_AnimationWindows.Remove(this);
            if (this.m_CurveEditor != null)
            {
                this.m_CurveEditor.curvesUpdated = (CurveEditor.CallbackFunction) Delegate.Remove(this.m_CurveEditor.curvesUpdated, new CurveEditor.CallbackFunction(this.SaveChangedCurvesFromCurveEditor));
                this.m_CurveEditor.OnDisable();
            }
            if (this.m_DopeSheet != null)
            {
                this.m_DopeSheet.OnDisable();
            }
            AnimationWindowSelection selection = this.m_State.selection;
            selection.onSelectionChanged = (Action) Delegate.Remove(selection.onSelectionChanged, new Action(this, (IntPtr) this.OnSelectionChanged));
            this.m_State.onStartLiveEdit = (Action) Delegate.Remove(this.m_State.onStartLiveEdit, new Action(this, (IntPtr) this.OnStartLiveEdit));
            this.m_State.onEndLiveEdit = (Action) Delegate.Remove(this.m_State.onEndLiveEdit, new Action(this, (IntPtr) this.OnEndLiveEdit));
            this.m_State.OnDisable();
        }

        public void OnEnable()
        {
            base.hideFlags = HideFlags.HideAndDontSave;
            s_AnimationWindows.Add(this);
            if (this.m_State == null)
            {
                this.m_State = ScriptableObject.CreateInstance(typeof(AnimationWindowState)) as AnimationWindowState;
                this.m_State.hideFlags = HideFlags.HideAndDontSave;
                this.m_State.animEditor = this;
                this.InitializeHorizontalSplitter();
                this.InitializeClipSelection();
                this.InitializeDopeSheet();
                this.InitializeEvents();
                this.InitializeCurveEditor();
                this.InitializeOverlay();
            }
            this.InitializeNonserializedValues();
            this.m_State.timeArea = !this.m_State.showCurveEditor ? ((TimeArea) this.m_DopeSheet) : ((TimeArea) this.m_CurveEditor);
            this.m_DopeSheet.state = this.m_State;
            this.m_ClipPopup.state = this.m_State;
            this.m_Overlay.state = this.m_State;
            this.m_CurveEditor.curvesUpdated = (CurveEditor.CallbackFunction) Delegate.Combine(this.m_CurveEditor.curvesUpdated, new CurveEditor.CallbackFunction(this.SaveChangedCurvesFromCurveEditor));
            AnimationWindowSelection selection = this.m_State.selection;
            selection.onSelectionChanged = (Action) Delegate.Combine(selection.onSelectionChanged, new Action(this, (IntPtr) this.OnSelectionChanged));
            this.m_State.onStartLiveEdit = (Action) Delegate.Combine(this.m_State.onStartLiveEdit, new Action(this, (IntPtr) this.OnStartLiveEdit));
            this.m_State.onEndLiveEdit = (Action) Delegate.Combine(this.m_State.onEndLiveEdit, new Action(this, (IntPtr) this.OnEndLiveEdit));
        }

        public void OnEndLiveEdit()
        {
            this.UpdateSelectedKeysToCurveEditor();
        }

        public void OnLostFocus()
        {
            if (this.m_Hierarchy != null)
            {
                this.m_Hierarchy.EndNameEditing(true);
            }
            EditorGUI.EndEditingActiveTextField();
        }

        public void OnSelectionChanged()
        {
            this.m_State.OnSelectionChanged();
            this.triggerFraming = true;
            this.Repaint();
        }

        public void OnStartLiveEdit()
        {
            this.SaveCurveEditorKeySelection();
        }

        private void OverlayEventOnGUI()
        {
            if (!this.m_State.animatorIsOptimized && !this.m_State.disabled)
            {
                Rect position = new Rect(this.hierarchyWidth - 1f, 0f, this.contentWidth - 15f, this.m_Position.height - 15f);
                GUI.BeginGroup(position);
                EditorGUI.BeginChangeCheck();
                this.m_Overlay.HandleEvents();
                GUI.EndGroup();
            }
        }

        private void OverlayOnGUI(Rect contentRect)
        {
            if ((!this.m_State.animatorIsOptimized && !this.m_State.disabled) && (Event.current.type == EventType.Repaint))
            {
                Rect rect = new Rect(contentRect.xMin, contentRect.yMin, contentRect.width - 15f, contentRect.height - 15f);
                Rect position = new Rect(this.hierarchyWidth - 1f, 0f, this.contentWidth - 15f, this.m_Position.height - 15f);
                GUI.BeginGroup(position);
                Rect rect3 = new Rect(0f, 0f, position.width, position.height);
                Rect rect4 = rect;
                rect4.position -= position.min;
                this.m_Overlay.OnGUI(rect3, rect4);
                GUI.EndGroup();
            }
        }

        private void PlaybackUpdate()
        {
            if (this.m_State.disabled && this.m_State.playing)
            {
                this.m_State.playing = false;
            }
            if (this.m_State.playing)
            {
                float num = Time.realtimeSinceStartup - this.m_PreviousUpdateTime;
                this.m_PreviousUpdateTime = Time.realtimeSinceStartup;
                this.m_State.currentTime += num;
                if (this.m_State.currentTime > this.m_State.maxTime)
                {
                    this.m_State.currentTime = this.m_State.minTime;
                }
                this.m_State.currentTime = Mathf.Clamp(this.m_State.currentTime, this.m_State.minTime, this.m_State.maxTime);
                this.m_State.ResampleAnimation();
                this.Repaint();
            }
        }

        private void PlayButtonOnGUI()
        {
            EditorGUI.BeginChangeCheck();
            this.m_State.playing = GUILayout.Toggle(this.m_State.playing, AnimationWindowStyles.playContent, EditorStyles.toolbarButton, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                EditorGUI.EndEditingActiveTextField();
                this.m_PreviousUpdateTime = Time.realtimeSinceStartup;
            }
        }

        private void PlayControlsOnGUI()
        {
            using (new EditorGUI.DisabledScope(!this.m_State.canRecord))
            {
                this.RecordButtonOnGUI();
            }
            if (GUILayout.Button(AnimationWindowStyles.firstKeyContent, EditorStyles.toolbarButton, new GUILayoutOption[0]))
            {
                this.MoveToFirstKeyframe();
                EditorGUI.EndEditingActiveTextField();
            }
            if (GUILayout.Button(AnimationWindowStyles.prevKeyContent, EditorStyles.toolbarButton, new GUILayoutOption[0]))
            {
                this.MoveToPreviousKeyframe();
                EditorGUI.EndEditingActiveTextField();
            }
            using (new EditorGUI.DisabledScope(!this.m_State.canRecord))
            {
                this.PlayButtonOnGUI();
            }
            if (GUILayout.Button(AnimationWindowStyles.nextKeyContent, EditorStyles.toolbarButton, new GUILayoutOption[0]))
            {
                this.MoveToNextKeyframe();
                EditorGUI.EndEditingActiveTextField();
            }
            if (GUILayout.Button(AnimationWindowStyles.lastKeyContent, EditorStyles.toolbarButton, new GUILayoutOption[0]))
            {
                this.MoveToLastKeyframe();
                EditorGUI.EndEditingActiveTextField();
            }
            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(35f) };
            int num = EditorGUILayout.IntField(this.m_State.frame, EditorStyles.toolbarTextField, options);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_State.frame = num;
            }
        }

        private void PostLayoutChanges()
        {
            if (this.policy != null)
            {
                if (GUIUtility.hotControl == this.m_HorizontalSplitter.ID)
                {
                    this.policy.OnGeometryChange(this.m_HorizontalSplitter.realSizes);
                }
                if (!this.m_State.disabled)
                {
                    float time = 0f;
                    if (this.policy.SynchronizeCurrentTime(ref time) && (time != this.m_State.currentTime))
                    {
                        this.policy.OnCurrentTimeChange(this.m_State.currentTime);
                    }
                    float horizontalScale = 1f;
                    float horizontalTranslation = 0f;
                    if (this.policy.SynchronizeZoomableArea(ref horizontalScale, ref horizontalTranslation) && (!Mathf.Approximately(horizontalScale, this.m_State.timeArea.m_Scale.x) || !Mathf.Approximately(horizontalTranslation, this.m_State.timeArea.m_Translation.x)))
                    {
                        this.policy.OnZoomableAreaChange(this.m_State.timeArea.m_Scale.x, this.m_State.timeArea.m_Translation.x);
                    }
                }
            }
        }

        private void RecordButtonOnGUI()
        {
            EditorGUI.BeginChangeCheck();
            this.m_State.recording = GUILayout.Toggle(this.m_State.recording, AnimationWindowStyles.recordContent, EditorStyles.toolbarButton, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck() && this.m_State.recording)
            {
                this.m_State.ResampleAnimation();
            }
        }

        private void RenderClipOverlay(Rect rect)
        {
            Vector2 timeRange = this.m_State.timeRange;
            AnimationWindowUtility.DrawRangeOfClip(rect, this.m_State.TimeToPixel(timeRange.x) + rect.xMin, this.m_State.TimeToPixel(timeRange.y) + rect.xMin);
        }

        private void RenderEventTooltip()
        {
            this.m_Events.DrawInstantTooltip(this.m_Position);
        }

        private void RenderSelectionOverlay(Rect rect)
        {
            if ((!this.m_State.showCurveEditor || this.m_CurveEditor.hasSelection) && (this.m_State.showCurveEditor || (this.m_State.selectedKeys.Count != 0)))
            {
                Bounds bounds = !this.m_State.showCurveEditor ? this.m_DopeSheet.selectionBounds : this.m_CurveEditor.selectionBounds;
                float startPixel = this.m_State.TimeToPixel(bounds.min.x) + rect.xMin;
                float endPixel = this.m_State.TimeToPixel(bounds.max.x) + rect.xMin;
                if ((endPixel - startPixel) < 14f)
                {
                    float num3 = (startPixel + endPixel) * 0.5f;
                    startPixel = num3 - 7f;
                    endPixel = num3 + 7f;
                }
                AnimationWindowUtility.DrawRangeOfSelection(rect, startPixel, endPixel);
            }
        }

        public void Repaint()
        {
            if (this.m_OwnerWindow != null)
            {
                this.m_OwnerWindow.Repaint();
            }
        }

        private void SaveChangedCurvesFromCurveEditor()
        {
            this.m_State.SaveKeySelection("Edit Curve");
            HashSet<AnimationClip> set = new HashSet<AnimationClip>();
            List<CurveWrapper> list = new List<CurveWrapper>();
            for (int i = 0; i < this.m_CurveEditor.animationCurves.Length; i++)
            {
                CurveWrapper item = this.m_CurveEditor.animationCurves[i];
                if (item.changed)
                {
                    if (!item.animationIsEditable)
                    {
                        Debug.LogError("Curve is not editable and shouldn't be saved.");
                    }
                    if (item.animationClip != null)
                    {
                        list.Add(item);
                        set.Add(item.animationClip);
                    }
                    item.changed = false;
                }
            }
            if (set.Count > 0)
            {
                foreach (AnimationClip clip in set)
                {
                    Undo.RegisterCompleteObjectUndo(clip, "Edit Curve");
                }
            }
            if (list.Count > 0)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    CurveWrapper wrapper2 = list[j];
                    AnimationUtility.SetEditorCurve(wrapper2.animationClip, wrapper2.binding, wrapper2.curve);
                }
                this.m_State.ResampleAnimation();
            }
            this.UpdateSelectedKeysFromCurveEditor();
        }

        private void SaveCurveEditorKeySelection()
        {
            this.UpdateSelectedKeysToCurveEditor();
            this.m_CurveEditor.SaveKeySelection("Edit Curve");
        }

        private void SetupWizardOnGUI(Rect position)
        {
            GUI.Label(position, GUIContent.none, AnimationWindowStyles.dopeSheetBackground);
            Rect rect = new Rect(position.x, position.y, position.width - 15f, position.height - 15f);
            GUI.BeginClip(rect);
            GUI.enabled = true;
            this.m_State.showCurveEditor = false;
            this.m_State.timeArea = this.m_DopeSheet;
            this.m_State.timeArea.SetShownHRangeInsideMargins(0f, 1f);
            if ((this.m_State.activeGameObject != null) && !EditorUtility.IsPersistent(this.m_State.activeGameObject))
            {
                string str = ((this.m_State.activeRootGameObject != null) || (this.m_State.activeAnimationClip != null)) ? AnimationWindowStyles.animationClip.text : AnimationWindowStyles.animatorAndAnimationClip.text;
                GUIContent content = GUIContent.Temp(string.Format(AnimationWindowStyles.formatIsMissing.text, this.m_State.activeGameObject.name, str));
                Vector2 vector = GUI.skin.label.CalcSize(content);
                Rect rect2 = new Rect((rect.width * 0.5f) - (vector.x * 0.5f), (rect.height * 0.5f) - (vector.y * 0.5f), vector.x, vector.y);
                GUI.Label(rect2, content);
                Rect rect3 = new Rect((rect.width * 0.5f) - 35f, rect2.yMax + 3f, 70f, 20f);
                if (GUI.Button(rect3, AnimationWindowStyles.create) && AnimationWindowUtility.InitializeGameobjectForAnimation(this.m_State.activeGameObject))
                {
                    Component closestAnimationPlayerComponentInParents = AnimationWindowUtility.GetClosestAnimationPlayerComponentInParents(this.m_State.activeGameObject.transform);
                    this.m_State.selection.UpdateClip(this.m_State.selectedItem, AnimationUtility.GetAnimationClips(closestAnimationPlayerComponentInParents.gameObject)[0]);
                    this.m_State.recording = true;
                    this.m_State.currentTime = 0f;
                    this.m_State.ResampleAnimation();
                    GUIUtility.ExitGUI();
                }
            }
            else
            {
                Color color = GUI.color;
                GUI.color = Color.gray;
                Vector2 vector2 = GUI.skin.label.CalcSize(AnimationWindowStyles.noAnimatableObjectSelectedText);
                Rect rect4 = new Rect((rect.width * 0.5f) - (vector2.x * 0.5f), (rect.height * 0.5f) - (vector2.y * 0.5f), vector2.x, vector2.y);
                GUI.Label(rect4, AnimationWindowStyles.noAnimatableObjectSelectedText);
                GUI.color = color;
            }
            GUI.EndClip();
            GUI.enabled = false;
        }

        private void SwitchBetweenCurvesAndDopesheet()
        {
            if (!this.m_State.showCurveEditor)
            {
                this.SwitchToCurveEditor();
            }
            else
            {
                this.SwitchToDopeSheetEditor();
            }
        }

        internal void SwitchToCurveEditor()
        {
            this.m_State.showCurveEditor = true;
            this.UpdateSelectedKeysToCurveEditor();
            AnimationWindowUtility.SyncTimeArea(this.m_DopeSheet, this.m_CurveEditor);
            this.m_State.timeArea = this.m_CurveEditor;
            this.m_CurveEditor.FrameSelected(false, true);
        }

        internal void SwitchToDopeSheetEditor()
        {
            this.m_State.showCurveEditor = false;
            this.UpdateSelectedKeysFromCurveEditor();
            AnimationWindowUtility.SyncTimeArea(this.m_CurveEditor, this.m_DopeSheet);
            this.m_State.timeArea = this.m_DopeSheet;
        }

        private void SynchronizeLayout()
        {
            this.m_HorizontalSplitter.realSizes[1] = (int) Mathf.Min(this.m_Position.width - this.m_HorizontalSplitter.realSizes[0], (float) this.m_HorizontalSplitter.realSizes[1]);
            if (this.policy != null)
            {
                this.policy.SynchronizeGeometry(ref this.m_HorizontalSplitter.realSizes, ref this.m_HorizontalSplitter.minSizes);
                float frameRate = 0f;
                if (this.policy.SynchronizeFrameRate(ref frameRate))
                {
                    this.m_State.frameRate = frameRate;
                }
                float time = 0f;
                if (this.policy.SynchronizeCurrentTime(ref time))
                {
                    this.m_State.currentTime = time;
                }
                float horizontalScale = 1f;
                float horizontalTranslation = 0f;
                if (this.policy.SynchronizeZoomableArea(ref horizontalScale, ref horizontalTranslation) && (this.m_State.timeArea != null))
                {
                    this.m_State.timeArea.m_Scale = new Vector2(horizontalScale, this.m_State.timeArea.m_Scale.y);
                    this.m_State.timeArea.m_Translation = new Vector2(horizontalTranslation, this.m_State.timeArea.m_Translation.y);
                    this.m_State.timeArea.EnforceScaleAndRange();
                }
            }
        }

        private void TabSelectionOnGUI()
        {
            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(80f) };
            GUILayout.Toggle(!this.m_State.showCurveEditor, AnimationWindowStyles.dopesheet, AnimationWindowStyles.miniToolbarButton, options);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(80f) };
            GUILayout.Toggle(this.m_State.showCurveEditor, AnimationWindowStyles.curves, AnimationWindowStyles.miniToolbarButton, optionArray2);
            if (EditorGUI.EndChangeCheck())
            {
                this.SwitchBetweenCurvesAndDopesheet();
            }
            else if (kAnimationShowCurvesToggle.activated)
            {
                this.SwitchBetweenCurvesAndDopesheet();
                Event.current.Use();
            }
        }

        private void TimeRulerOnGUI(Rect timeRulerRect)
        {
            Rect position = new Rect(timeRulerRect.xMin, timeRulerRect.yMin, timeRulerRect.width - 15f, timeRulerRect.height);
            GUI.Box(timeRulerRect, GUIContent.none, EditorStyles.toolbarButton);
            this.m_State.timeArea.TimeRuler(position, this.m_State.frameRate, true, false, 1f, this.m_State.timeFormat);
            if (!this.m_State.disabled)
            {
                GUI.BeginGroup(position);
                Rect rect = new Rect(0f, 0f, position.width, position.height);
                this.RenderClipOverlay(rect);
                this.RenderSelectionOverlay(rect);
                GUI.EndGroup();
            }
        }

        public void Update()
        {
            if (this.m_State != null)
            {
                this.PlaybackUpdate();
            }
        }

        internal void UpdateCurveEditorData()
        {
            this.m_CurveEditor.animationCurves = this.m_State.activeCurveWrappers.ToArray();
        }

        private void UpdateSelectedKeysFromCurveEditor()
        {
            this.m_State.ClearKeySelections();
            foreach (CurveSelection selection in this.m_CurveEditor.selectedCurves)
            {
                AnimationWindowKeyframe keyframe = AnimationWindowUtility.CurveSelectionToAnimationWindowKeyframe(selection, this.m_State.allCurves);
                if (keyframe != null)
                {
                    this.m_State.SelectKey(keyframe);
                }
            }
        }

        private void UpdateSelectedKeysToCurveEditor()
        {
            this.UpdateCurveEditorData();
            this.m_CurveEditor.ClearSelection();
            foreach (AnimationWindowKeyframe keyframe in this.m_State.selectedKeys)
            {
                CurveSelection curveSelection = AnimationWindowUtility.AnimationWindowKeyframeToCurveSelection(keyframe, this.m_CurveEditor);
                if (curveSelection != null)
                {
                    this.m_CurveEditor.AddSelection(curveSelection);
                }
            }
        }

        private float contentWidth
        {
            get
            {
                return (float) this.m_HorizontalSplitter.realSizes[1];
            }
        }

        internal CurveEditor curveEditor
        {
            get
            {
                return this.m_CurveEditor;
            }
        }

        internal DopeSheetEditor dopeSheetEditor
        {
            get
            {
                return this.m_DopeSheet;
            }
        }

        private float hierarchyWidth
        {
            get
            {
                return (float) this.m_HorizontalSplitter.realSizes[0];
            }
        }

        public bool locked
        {
            get
            {
                return this.m_State.locked;
            }
            set
            {
                this.m_State.locked = value;
            }
        }

        public AnimationWindowPolicy policy
        {
            get
            {
                return this.m_State.policy;
            }
            set
            {
                this.m_State.policy = value;
            }
        }

        public AnimationWindowSelectionItem selectedItem
        {
            get
            {
                return this.m_State.selectedItem;
            }
            set
            {
                this.m_State.selectedItem = value;
            }
        }

        public AnimationWindowSelection selection
        {
            get
            {
                return this.m_State.selection;
            }
        }

        public AnimationWindowState state
        {
            get
            {
                return this.m_State;
            }
        }

        public bool stateDisabled
        {
            get
            {
                return this.m_State.disabled;
            }
        }

        private bool triggerFraming
        {
            get
            {
                return (((this.policy == null) || this.policy.triggerFramingOnSelection) && this.m_TriggerFraming);
            }
            set
            {
                this.m_TriggerFraming = value;
            }
        }
    }
}

