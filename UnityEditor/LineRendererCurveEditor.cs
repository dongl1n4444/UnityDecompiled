namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class LineRendererCurveEditor
    {
        private CurveEditor m_Editor = null;
        private bool m_Refresh = false;
        private CurveEditorSettings m_Settings = new CurveEditorSettings();
        private SerializedProperty m_WidthCurve;
        private SerializedProperty m_WidthMultiplier;

        public void CheckCurveChangedExternally()
        {
            CurveWrapper curveWrapperFromID = this.m_Editor.GetCurveWrapperFromID(0);
            if (this.m_WidthCurve != null)
            {
                AnimationCurve animationCurveValue = this.m_WidthCurve.animationCurveValue;
                if ((curveWrapperFromID == null) != this.m_WidthCurve.hasMultipleDifferentValues)
                {
                    this.m_Refresh = true;
                }
                else if (curveWrapperFromID != null)
                {
                    if (curveWrapperFromID.curve.length == 0)
                    {
                        this.m_Refresh = true;
                    }
                    else if ((animationCurveValue.length >= 1) && (animationCurveValue.keys[0].value != curveWrapperFromID.curve.keys[0].value))
                    {
                        this.m_Refresh = true;
                    }
                }
            }
            else if (curveWrapperFromID != null)
            {
                this.m_Refresh = true;
            }
        }

        public Vector2 GetAxisScalars()
        {
            return new Vector2(1f, this.m_WidthMultiplier.floatValue);
        }

        private CurveWrapper GetCurveWrapper(AnimationCurve curve)
        {
            float r = EditorGUIUtility.isProSkin ? 1f : 0.9f;
            Color color = new Color(r, r, r, 1f);
            CurveWrapper wrapper = new CurveWrapper {
                id = 0,
                groupId = -1,
                color = new Color(1f, 0f, 0f, 1f) * color,
                hidden = false,
                readOnly = false,
                renderer = new NormalCurveRenderer(curve)
            };
            wrapper.renderer.SetCustomRange(0f, 1f);
            wrapper.getAxisUiScalarsCallback = new CurveWrapper.GetAxisScalarsCallback(this.GetAxisScalars);
            return wrapper;
        }

        public void OnDisable()
        {
            this.m_Editor.OnDisable();
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
        }

        public void OnEnable(SerializedObject serializedObject)
        {
            this.m_WidthMultiplier = serializedObject.FindProperty("m_Parameters.widthMultiplier");
            this.m_WidthCurve = serializedObject.FindProperty("m_Parameters.widthCurve");
            this.m_Settings.hRangeMin = 0f;
            this.m_Settings.vRangeMin = 0f;
            this.m_Settings.vRangeMax = 1f;
            this.m_Settings.hRangeMax = 1f;
            this.m_Settings.vSlider = false;
            this.m_Settings.hSlider = false;
            TickStyle style = new TickStyle {
                tickColor = { color = new Color(0f, 0f, 0f, 0.15f) },
                distLabel = 30
            };
            this.m_Settings.hTickStyle = style;
            TickStyle style2 = new TickStyle {
                tickColor = { color = new Color(0f, 0f, 0f, 0.15f) },
                distLabel = 20
            };
            this.m_Settings.vTickStyle = style2;
            this.m_Settings.undoRedoSelection = true;
            this.m_Editor = new CurveEditor(new Rect(0f, 0f, 1000f, 100f), new CurveWrapper[0], false);
            this.m_Editor.settings = this.m_Settings;
            this.m_Editor.margin = 25f;
            this.m_Editor.SetShownHRangeInsideMargins(0f, 1f);
            this.m_Editor.SetShownVRangeInsideMargins(0f, 1f);
            this.m_Editor.ignoreScrollWheelUntilClicked = true;
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
        }

        public void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this.m_WidthMultiplier, Styles.widthMultiplier, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_Refresh = true;
            }
            Rect aspectRect = GUILayoutUtility.GetAspectRect(2.5f, GUI.skin.textField);
            aspectRect.xMin += EditorGUI.indent;
            if ((Event.current.type != EventType.Layout) && (Event.current.type != EventType.Used))
            {
                this.m_Editor.rect = new Rect(aspectRect.x, aspectRect.y, aspectRect.width, aspectRect.height);
            }
            if (this.m_Refresh)
            {
                this.m_Editor.animationCurves = new CurveWrapper[] { this.GetCurveWrapper(this.m_WidthCurve.animationCurveValue) };
                this.m_Refresh = false;
            }
            GUI.Label(this.m_Editor.drawRect, GUIContent.none, "TextField");
            this.m_Editor.hRangeLocked = Event.current.shift;
            this.m_Editor.vRangeLocked = EditorGUI.actionKey;
            this.m_Editor.OnGUI();
            if ((this.m_Editor.GetCurveWrapperFromID(0) != null) && this.m_Editor.GetCurveWrapperFromID(0).changed)
            {
                AnimationCurve curve = this.m_Editor.GetCurveWrapperFromID(0).curve;
                if (curve.length > 0)
                {
                    this.m_WidthCurve.animationCurveValue = curve;
                    this.m_Editor.GetCurveWrapperFromID(0).changed = false;
                }
            }
        }

        private void UndoRedoPerformed()
        {
            this.m_Refresh = true;
        }

        private class Styles
        {
            public static GUIContent widthMultiplier = EditorGUIUtility.TextContent("Width|The multiplier applied to the curve, describing the width (in world space) along the line.");
        }
    }
}

