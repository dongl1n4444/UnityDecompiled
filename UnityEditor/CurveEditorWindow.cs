namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable]
    internal class CurveEditorWindow : EditorWindow
    {
        [SerializeField]
        private GUIView delegateView;
        private const int kPresetsHeight = 0x2e;
        private Color m_Color;
        private AnimationCurve m_Curve;
        private CurveEditor m_CurveEditor;
        private CurvePresetsContentsForPopupWindow m_CurvePresets;
        private GUIContent m_GUIContent = new GUIContent();
        internal static Styles ms_Styles;
        private static CurveEditorWindow s_SharedCurveEditor;

        private static Keyframe[] CopyAndScaleCurveKeys(Keyframe[] orgKeys, Rect rect, NormalizationMode normalization)
        {
            Keyframe[] array = new Keyframe[orgKeys.Length];
            orgKeys.CopyTo(array, 0);
            if (normalization != NormalizationMode.None)
            {
                if (((rect.width == 0f) || (rect.height == 0f)) || (float.IsInfinity(rect.width) || float.IsInfinity(rect.height)))
                {
                    Debug.LogError("CopyAndScaleCurve: Invalid scale: " + rect);
                    return array;
                }
                float num = rect.height / rect.width;
                if (normalization != NormalizationMode.Normalize)
                {
                    if (normalization == NormalizationMode.Denormalize)
                    {
                        for (int i = 0; i < array.Length; i++)
                        {
                            array[i].time = (orgKeys[i].time * rect.width) + rect.xMin;
                            array[i].value = (orgKeys[i].value * rect.height) + rect.yMin;
                            if (!float.IsInfinity(orgKeys[i].inTangent))
                            {
                                array[i].inTangent = orgKeys[i].inTangent * num;
                            }
                            if (!float.IsInfinity(orgKeys[i].outTangent))
                            {
                                array[i].outTangent = orgKeys[i].outTangent * num;
                            }
                        }
                        return array;
                    }
                }
                else
                {
                    for (int j = 0; j < array.Length; j++)
                    {
                        array[j].time = (orgKeys[j].time - rect.xMin) / rect.width;
                        array[j].value = (orgKeys[j].value - rect.yMin) / rect.height;
                        if (!float.IsInfinity(orgKeys[j].inTangent))
                        {
                            array[j].inTangent = orgKeys[j].inTangent / num;
                        }
                        if (!float.IsInfinity(orgKeys[j].outTangent))
                        {
                            array[j].outTangent = orgKeys[j].outTangent / num;
                        }
                    }
                }
            }
            return array;
        }

        private void DoUpdateCurve(bool exitGUI)
        {
            if (((this.m_CurveEditor.animationCurves.Length > 0) && (this.m_CurveEditor.animationCurves[0] != null)) && this.m_CurveEditor.animationCurves[0].changed)
            {
                this.m_CurveEditor.animationCurves[0].changed = false;
                this.RefreshShownCurves();
                this.SendEvent("CurveChanged", exitGUI);
            }
        }

        internal static Keyframe[] GetConstantKeys(float value)
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, value, 0f, 0f), new Keyframe(1f, value, 0f, 0f) };
            SetSmoothEditable(ref keys);
            return keys;
        }

        private Rect GetCurveEditorRect() => 
            new Rect(0f, 0f, base.position.width, base.position.height - 46f);

        private CurveWrapper[] GetCurveWrapperArray()
        {
            if (this.m_Curve == null)
            {
                return new CurveWrapper[0];
            }
            CurveWrapper wrapper = new CurveWrapper {
                id = "Curve".GetHashCode(),
                groupId = -1,
                color = this.m_Color,
                hidden = false,
                readOnly = false,
                renderer = new NormalCurveRenderer(this.m_Curve)
            };
            wrapper.renderer.SetWrap(this.m_Curve.preWrapMode, this.m_Curve.postWrapMode);
            return new CurveWrapper[] { wrapper };
        }

        private Keyframe[] GetDenormalizedKeys(Keyframe[] sourceKeys) => 
            this.NormalizeKeys(sourceKeys, NormalizationMode.Denormalize);

        internal static Keyframe[] GetEaseInKeys()
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 0f, 0f, 0f), new Keyframe(1f, 1f, 2f, 2f) };
            SetSmoothEditable(ref keys);
            return keys;
        }

        internal static Keyframe[] GetEaseInMirrorKeys()
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 1f, -2f, -2f), new Keyframe(1f, 0f, 0f, 0f) };
            SetSmoothEditable(ref keys);
            return keys;
        }

        internal static Keyframe[] GetEaseInOutKeys()
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 0f, 0f, 0f), new Keyframe(1f, 1f, 0f, 0f) };
            SetSmoothEditable(ref keys);
            return keys;
        }

        internal static Keyframe[] GetEaseInOutMirrorKeys()
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 1f, 0f, 0f), new Keyframe(1f, 0f, 0f, 0f) };
            SetSmoothEditable(ref keys);
            return keys;
        }

        internal static Keyframe[] GetEaseOutKeys()
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 0f, 2f, 2f), new Keyframe(1f, 1f, 0f, 0f) };
            SetSmoothEditable(ref keys);
            return keys;
        }

        internal static Keyframe[] GetEaseOutMirrorKeys()
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 1f, 0f, 0f), new Keyframe(1f, 0f, -2f, -2f) };
            SetSmoothEditable(ref keys);
            return keys;
        }

        internal static Keyframe[] GetLinearKeys()
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 0f, 1f, 1f), new Keyframe(1f, 1f, 1f, 1f) };
            SetSmoothEditable(ref keys);
            return keys;
        }

        internal static Keyframe[] GetLinearMirrorKeys()
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 1f, -1f, -1f), new Keyframe(1f, 0f, -1f, -1f) };
            SetSmoothEditable(ref keys);
            return keys;
        }

        private bool GetNormalizationRect(out Rect normalizationRect)
        {
            normalizationRect = new Rect();
            if (this.m_CurveEditor.settings.hasUnboundedRanges)
            {
                return false;
            }
            normalizationRect = new Rect(this.m_CurveEditor.settings.hRangeMin, this.m_CurveEditor.settings.vRangeMin, this.m_CurveEditor.settings.hRangeMax - this.m_CurveEditor.settings.hRangeMin, this.m_CurveEditor.settings.vRangeMax - this.m_CurveEditor.settings.vRangeMin);
            return true;
        }

        private Keyframe[] GetNormalizedKeys(Keyframe[] sourceKeys) => 
            this.NormalizeKeys(sourceKeys, NormalizationMode.Normalize);

        private void Init(CurveEditorSettings settings)
        {
            this.m_CurveEditor = new CurveEditor(this.GetCurveEditorRect(), this.GetCurveWrapperArray(), true);
            this.m_CurveEditor.curvesUpdated = new CurveEditor.CallbackFunction(this.UpdateCurve);
            this.m_CurveEditor.scaleWithWindow = true;
            this.m_CurveEditor.margin = 40f;
            if (settings != null)
            {
                this.m_CurveEditor.settings = settings;
            }
            this.m_CurveEditor.settings.hTickLabelOffset = 10f;
            this.m_CurveEditor.settings.rectangleToolFlags = CurveEditorSettings.RectangleToolFlags.MiniRectangleTool;
            this.m_CurveEditor.settings.undoRedoSelection = true;
            this.m_CurveEditor.settings.showWrapperPopups = true;
            bool horizontally = true;
            bool vertically = true;
            if ((this.m_CurveEditor.settings.hRangeMin != float.NegativeInfinity) && (this.m_CurveEditor.settings.hRangeMax != float.PositiveInfinity))
            {
                this.m_CurveEditor.SetShownHRangeInsideMargins(this.m_CurveEditor.settings.hRangeMin, this.m_CurveEditor.settings.hRangeMax);
                horizontally = false;
            }
            if ((this.m_CurveEditor.settings.vRangeMin != float.NegativeInfinity) && (this.m_CurveEditor.settings.vRangeMax != float.PositiveInfinity))
            {
                this.m_CurveEditor.SetShownVRangeInsideMargins(this.m_CurveEditor.settings.vRangeMin, this.m_CurveEditor.settings.vRangeMax);
                vertically = false;
            }
            this.m_CurveEditor.FrameSelected(horizontally, vertically);
        }

        private void InitCurvePresets()
        {
            if (this.m_CurvePresets == null)
            {
                Action<AnimationCurve> presetSelectedCallback = delegate (AnimationCurve presetCurve) {
                    this.ValidateCurveLibraryTypeAndScale();
                    this.m_Curve.keys = this.GetDenormalizedKeys(presetCurve.keys);
                    this.m_Curve.postWrapMode = presetCurve.postWrapMode;
                    this.m_Curve.preWrapMode = presetCurve.preWrapMode;
                    this.m_CurveEditor.SelectNone();
                    this.RefreshShownCurves();
                    this.SendEvent("CurveChanged", true);
                };
                AnimationCurve animCurve = null;
                this.m_CurvePresets = new CurvePresetsContentsForPopupWindow(animCurve, this.curveLibraryType, presetSelectedCallback);
                this.m_CurvePresets.InitIfNeeded();
            }
        }

        private Keyframe[] NormalizeKeys(Keyframe[] sourceKeys, NormalizationMode normalization)
        {
            Rect rect;
            if (!this.GetNormalizationRect(out rect))
            {
                normalization = NormalizationMode.None;
            }
            return CopyAndScaleCurveKeys(sourceKeys, rect, normalization);
        }

        private void OnDestroy()
        {
            this.m_CurvePresets.GetPresetLibraryEditor().UnloadUsedLibraries();
        }

        private void OnDisable()
        {
            this.m_CurveEditor.OnDisable();
            if (s_SharedCurveEditor == this)
            {
                s_SharedCurveEditor = null;
            }
            else if (!this.Equals(s_SharedCurveEditor))
            {
                throw new ApplicationException("s_SharedCurveEditor does not equal this");
            }
        }

        private void OnEnable()
        {
            s_SharedCurveEditor = this;
            this.Init(null);
        }

        private void OnGUI()
        {
            bool flag = Event.current.type == EventType.MouseUp;
            if (this.delegateView == null)
            {
                this.m_Curve = null;
            }
            if (ms_Styles == null)
            {
                ms_Styles = new Styles();
            }
            this.m_CurveEditor.rect = this.GetCurveEditorRect();
            this.m_CurveEditor.hRangeLocked = Event.current.shift;
            this.m_CurveEditor.vRangeLocked = EditorGUI.actionKey;
            GUI.changed = false;
            GUI.Label(this.m_CurveEditor.drawRect, GUIContent.none, ms_Styles.curveEditorBackground);
            this.m_CurveEditor.OnGUI();
            GUI.Box(new Rect(0f, base.position.height - 46f, base.position.width, 46f), "", ms_Styles.curveSwatchArea);
            this.m_Color.a *= 0.6f;
            float y = (base.position.height - 46f) + 10.5f;
            this.InitCurvePresets();
            CurvePresetLibrary currentLib = this.m_CurvePresets.GetPresetLibraryEditor().GetCurrentLib();
            if (currentLib != null)
            {
                for (int i = 0; i < currentLib.Count(); i++)
                {
                    Rect position = new Rect(45f + (45f * i), y, 40f, 25f);
                    this.m_GUIContent.tooltip = currentLib.GetName(i);
                    if (GUI.Button(position, this.m_GUIContent, ms_Styles.curveSwatch))
                    {
                        AnimationCurve preset = currentLib.GetPreset(i) as AnimationCurve;
                        this.m_Curve.keys = this.GetDenormalizedKeys(preset.keys);
                        this.m_Curve.postWrapMode = preset.postWrapMode;
                        this.m_Curve.preWrapMode = preset.preWrapMode;
                        this.m_CurveEditor.SelectNone();
                        this.SendEvent("CurveChanged", true);
                    }
                    if (Event.current.type == EventType.Repaint)
                    {
                        currentLib.Draw(position, i);
                    }
                    if (position.xMax > (base.position.width - 90f))
                    {
                        break;
                    }
                }
            }
            Rect rect = new Rect(25f, y + 5f, 20f, 20f);
            this.PresetDropDown(rect);
            if ((Event.current.type == EventType.Used) && flag)
            {
                this.DoUpdateCurve(false);
                this.SendEvent("CurveChangeCompleted", true);
            }
            else if ((Event.current.type != EventType.Layout) && (Event.current.type != EventType.Repaint))
            {
                this.DoUpdateCurve(true);
            }
        }

        private void PresetDropDown(Rect rect)
        {
            if (EditorGUI.DropdownButton(rect, EditorGUI.GUIContents.titleSettingsIcon, FocusType.Passive, EditorStyles.inspectorTitlebarText) && (this.m_Curve != null))
            {
                if (this.m_CurvePresets == null)
                {
                    Debug.LogError("Curve presets error");
                }
                else
                {
                    this.ValidateCurveLibraryTypeAndScale();
                    AnimationCurve curve = new AnimationCurve(this.GetNormalizedKeys(this.m_Curve.keys)) {
                        postWrapMode = this.m_Curve.postWrapMode,
                        preWrapMode = this.m_Curve.preWrapMode
                    };
                    this.m_CurvePresets.curveToSaveAsPreset = curve;
                    PopupWindow.Show(rect, this.m_CurvePresets);
                }
            }
        }

        private void RefreshShownCurves()
        {
            this.m_CurveEditor.animationCurves = this.GetCurveWrapperArray();
        }

        private void SendEvent(string eventName, bool exitGUI)
        {
            if (this.delegateView != null)
            {
                Event e = EditorGUIUtility.CommandEvent(eventName);
                base.Repaint();
                this.delegateView.SendEvent(e);
                if (exitGUI)
                {
                    GUIUtility.ExitGUI();
                }
            }
            GUI.changed = true;
        }

        internal static void SetSmoothEditable(ref Keyframe[] keys)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                AnimationUtility.SetKeyBroken(ref keys[i], false);
                AnimationUtility.SetKeyLeftTangentMode(ref keys[i], AnimationUtility.TangentMode.Free);
                AnimationUtility.SetKeyRightTangentMode(ref keys[i], AnimationUtility.TangentMode.Free);
            }
        }

        public void Show(GUIView viewToUpdate, CurveEditorSettings settings)
        {
            this.delegateView = viewToUpdate;
            this.Init(settings);
            base.ShowAuxWindow();
            base.titleContent = new GUIContent("Curve");
            base.minSize = new Vector2(240f, 286f);
            base.maxSize = new Vector2(10000f, 10000f);
        }

        public void UpdateCurve()
        {
            this.DoUpdateCurve(false);
        }

        private void ValidateCurveLibraryTypeAndScale()
        {
            Rect rect;
            if (this.GetNormalizationRect(out rect))
            {
                if (this.curveLibraryType != CurveLibraryType.NormalizedZeroToOne)
                {
                    Debug.LogError("When having a normalize rect we should be using curve library type: NormalizedZeroToOne (normalizationRect: " + rect + ")");
                }
            }
            else if (this.curveLibraryType != CurveLibraryType.Unbounded)
            {
                Debug.LogError("When NOT having a normalize rect we should be using library type: Unbounded");
            }
        }

        public static Color color
        {
            get => 
                instance.m_Color;
            set
            {
                instance.m_Color = value;
                instance.RefreshShownCurves();
            }
        }

        public string currentPresetLibrary
        {
            get
            {
                this.InitCurvePresets();
                return this.m_CurvePresets.currentPresetLibrary;
            }
            set
            {
                this.InitCurvePresets();
                this.m_CurvePresets.currentPresetLibrary = value;
            }
        }

        public static AnimationCurve curve
        {
            get => 
                instance.m_Curve;
            set
            {
                if (value == null)
                {
                    instance.m_Curve = null;
                }
                else
                {
                    instance.m_Curve = value;
                    instance.RefreshShownCurves();
                }
            }
        }

        private CurveLibraryType curveLibraryType
        {
            get
            {
                if (this.m_CurveEditor.settings.hasUnboundedRanges)
                {
                    return CurveLibraryType.Unbounded;
                }
                return CurveLibraryType.NormalizedZeroToOne;
            }
        }

        public static CurveEditorWindow instance
        {
            get
            {
                if (s_SharedCurveEditor == null)
                {
                    s_SharedCurveEditor = ScriptableObject.CreateInstance<CurveEditorWindow>();
                }
                return s_SharedCurveEditor;
            }
        }

        public static bool visible =>
            (s_SharedCurveEditor != null);

        private enum NormalizationMode
        {
            None,
            Normalize,
            Denormalize
        }

        internal class Styles
        {
            public GUIStyle curveEditorBackground = "PopupCurveEditorBackground";
            public GUIStyle curveSwatch = "PopupCurveEditorSwatch";
            public GUIStyle curveSwatchArea = "PopupCurveSwatchBackground";
            public GUIStyle curveWrapPopup = "PopupCurveDropdown";
            public GUIStyle miniToolbarButton = "MiniToolbarButtonLeft";
            public GUIStyle miniToolbarPopup = "MiniToolbarPopup";
        }
    }
}

