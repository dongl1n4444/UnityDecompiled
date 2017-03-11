namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable]
    internal class CurveEditor : TimeArea, CurveUpdater
    {
        [CompilerGenerated]
        private static Func<CurveWrapper, int> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<CurveWrapper, int> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<ChangedCurve, int> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache3;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private bool <editingPoints>k__BackingField;
        public CallbackFunction curvesUpdated;
        private string focusedPointField;
        public float invSnap;
        private const float kCurveTimeEpsilon = 1E-05f;
        private const float kExactPickDistSqr = 16f;
        private const float kMaxPickDistSqr = 100f;
        private const string kPointTimeFieldName = "pointTimeField";
        private const string kPointValueFieldName = "pointValueField";
        private CurveWrapper[] m_AnimationCurves;
        private string m_AxisLabelFormat;
        private AxisLock m_AxisLock;
        private bool m_BoundsAreDirty;
        private List<SavedCurve> m_CurveBackups;
        private Bounds m_CurveBounds;
        internal Bounds m_DefaultBounds;
        private Vector2 m_DraggedCoord;
        private CurveWrapper[] m_DraggingCurveOrRegion;
        private CurveWrapper m_DraggingKey;
        private Bounds m_DrawingBounds;
        private List<int> m_DrawOrder;
        private CurveMenuManager m_MenuManager;
        private Vector2 m_MoveCoord;
        private CurveControlPointRenderer m_PointRenderer;
        private Vector2 m_PreviousDrawPointCenter;
        private CurveEditorRectangleTool m_RectangleTool;
        private CurveSelection m_SelectedTangentPoint;
        [SerializeField]
        private CurveEditorSelection m_Selection;
        private Bounds m_SelectionBounds;
        private CurveEditorSettings m_Settings;
        private Color m_TangentColor;
        internal Styles ms_Styles;
        private Vector2 pointEditingFieldPosition;
        private List<CurveSelection> preCurveDragSelection;
        private Vector2 s_EndMouseDragPosition;
        private PickMode s_PickMode;
        private List<CurveSelection> s_SelectionBackup;
        private static int s_SelectKeyHash = "SelectKeys".GetHashCode();
        private float s_StartClickedTime;
        private Vector2 s_StartKeyDragPosition;
        private Vector2 s_StartMouseDragPosition;
        private static int s_TangentControlIDHash = "s_TangentControlIDHash".GetHashCode();
        private bool s_TimeRangeSelectionActive;
        private float s_TimeRangeSelectionEnd;
        private float s_TimeRangeSelectionStart;
        public ICurveEditorState state;
        private bool timeWasEdited;
        private bool valueWasEdited;

        public CurveEditor(Rect rect, CurveWrapper[] curves, bool minimalGUI) : base(minimalGUI)
        {
            this.m_DrawOrder = new List<int>();
            this.m_DefaultBounds = new Bounds(new Vector3(0.5f, 0.5f, 0f), new Vector3(1f, 1f, 0f));
            this.m_Settings = new CurveEditorSettings();
            this.m_TangentColor = new Color(1f, 1f, 1f, 0.5f);
            this.invSnap = 0f;
            this.preCurveDragSelection = null;
            this.s_TimeRangeSelectionActive = false;
            this.m_BoundsAreDirty = true;
            this.m_SelectionBounds = new Bounds(Vector3.zero, Vector3.zero);
            this.m_CurveBounds = new Bounds(Vector3.zero, Vector3.zero);
            this.m_DrawingBounds = new Bounds(Vector3.zero, Vector3.zero);
            this.m_DraggingKey = null;
            this.m_AxisLabelFormat = "n1";
            this.focusedPointField = null;
            this.m_DraggingCurveOrRegion = null;
            base.rect = rect;
            this.animationCurves = curves;
            float[] tickModulos = new float[] { 
                1E-07f, 5E-07f, 1E-06f, 5E-06f, 1E-05f, 5E-05f, 0.0001f, 0.0005f, 0.001f, 0.005f, 0.01f, 0.05f, 0.1f, 0.5f, 1f, 5f,
                10f, 50f, 100f, 500f, 1000f, 5000f, 10000f, 50000f, 100000f, 500000f, 1000000f, 5000000f, 1E+07f
            };
            base.hTicks = new TickHandler();
            base.hTicks.SetTickModulos(tickModulos);
            base.vTicks = new TickHandler();
            base.vTicks.SetTickModulos(tickModulos);
            base.margin = 40f;
            this.OnEnable();
        }

        public void AddKey(CurveWrapper cw, Keyframe key)
        {
            CurveSelection curveSelection = this.AddKeyframeAndSelect(key, cw);
            if (curveSelection != null)
            {
                this.ClearSelection();
                this.AddSelection(curveSelection);
                this.RecalcSecondarySelection();
            }
            else
            {
                this.SelectNone();
            }
        }

        private CurveSelection AddKeyAtPosition(CurveWrapper cw, Vector2 position)
        {
            float num;
            position.x = this.SnapTime(position.x);
            if (this.invSnap != 0f)
            {
                num = 0.5f / this.invSnap;
            }
            else
            {
                num = 0.0001f;
            }
            if (CurveUtility.HaveKeysInRange(cw.curve, position.x - num, position.x + num))
            {
                return null;
            }
            float inTangent = 0f;
            Keyframe key = new Keyframe(position.x, this.SnapValue(position.y), inTangent, inTangent);
            return this.AddKeyframeAndSelect(key, cw);
        }

        private CurveSelection AddKeyAtTime(CurveWrapper cw, float time)
        {
            float num;
            time = this.SnapTime(time);
            if (this.invSnap != 0f)
            {
                num = 0.5f / this.invSnap;
            }
            else
            {
                num = 0.0001f;
            }
            if (CurveUtility.HaveKeysInRange(cw.curve, time - num, time + num))
            {
                return null;
            }
            float inTangent = cw.renderer.EvaluateCurveDeltaSlow(time);
            float num3 = this.ClampVerticalValue(this.SnapValue(cw.renderer.EvaluateCurveSlow(time)), cw.id);
            Keyframe key = new Keyframe(time, num3, inTangent, inTangent);
            return this.AddKeyframeAndSelect(key, cw);
        }

        private CurveSelection AddKeyframeAndSelect(Keyframe key, CurveWrapper cw)
        {
            if (!cw.animationIsEditable)
            {
                return null;
            }
            int keyIndex = cw.curve.AddKey(key);
            CurveUtility.SetKeyModeFromContext(cw.curve, keyIndex);
            AnimationUtility.UpdateTangentsFromModeSurrounding(cw.curve, keyIndex);
            CurveSelection selection2 = new CurveSelection(cw.id, keyIndex);
            cw.selected = CurveWrapper.SelectionMode.Selected;
            cw.changed = true;
            if (this.syncTimeDuringDrag)
            {
                this.activeTime = key.time + cw.timeOffset;
            }
            return selection2;
        }

        internal void AddSelection(CurveSelection curveSelection)
        {
            this.selectedCurves.Add(curveSelection);
        }

        protected void ApplySettings()
        {
            base.hRangeLocked = this.settings.hRangeLocked;
            base.vRangeLocked = this.settings.vRangeLocked;
            base.hRangeMin = this.settings.hRangeMin;
            base.hRangeMax = this.settings.hRangeMax;
            base.vRangeMin = this.settings.vRangeMin;
            base.vRangeMax = this.settings.vRangeMax;
            base.scaleWithWindow = this.settings.scaleWithWindow;
            base.hSlider = this.settings.hSlider;
            base.vSlider = this.settings.vSlider;
            this.RecalculateBounds();
        }

        public void BeginTimeRangeSelection(float time, bool addToSelection)
        {
            if (this.s_TimeRangeSelectionActive)
            {
                UnityEngine.Debug.LogError("BeginTimeRangeSelection can only be called once");
            }
            else
            {
                this.s_TimeRangeSelectionActive = true;
                this.s_TimeRangeSelectionStart = this.s_TimeRangeSelectionEnd = time;
                if (addToSelection)
                {
                    this.s_SelectionBackup = new List<CurveSelection>(this.selectedCurves);
                }
                else
                {
                    this.s_SelectionBackup = new List<CurveSelection>();
                }
            }
        }

        public void CancelTimeRangeSelection()
        {
            if (!this.s_TimeRangeSelectionActive)
            {
                UnityEngine.Debug.LogError("CancelTimeRangeSelection can only be called after BeginTimeRangeSelection");
            }
            else
            {
                this.selectedCurves = this.s_SelectionBackup;
                this.s_TimeRangeSelectionActive = false;
            }
        }

        private float ClampVerticalValue(float value, int curveID)
        {
            value = Mathf.Clamp(value, base.vRangeMin, base.vRangeMax);
            CurveWrapper curveWrapperFromID = this.GetCurveWrapperFromID(curveID);
            if (curveWrapperFromID != null)
            {
                value = Mathf.Clamp(value, curveWrapperFromID.vRangeMin, curveWrapperFromID.vRangeMax);
            }
            return value;
        }

        internal void ClearSelection()
        {
            this.selectedCurves.Clear();
        }

        private void CreateKeyFromClick(object obj)
        {
            string undoLabel = "Add Key";
            this.SaveKeySelection(undoLabel);
            List<int> curveIds = this.CreateKeyFromClick((Vector2) obj);
            if (curveIds.Count > 0)
            {
                this.UpdateCurves(curveIds, undoLabel);
            }
        }

        private List<int> CreateKeyFromClick(Vector2 viewPos)
        {
            Vector2 vector2;
            List<int> list = new List<int>();
            int index = this.OnlyOneEditableCurve();
            if (index >= 0)
            {
                CurveWrapper cw = this.m_AnimationCurves[index];
                Vector2 localPos = this.OffsetViewToDrawingTransformPoint(cw, viewPos);
                float num2 = localPos.x - cw.timeOffset;
                if (((cw.curve.keys.Length == 0) || (num2 < cw.curve.keys[0].time)) || (num2 > cw.curve.keys[cw.curve.keys.Length - 1].time))
                {
                    if (this.CreateKeyFromClick(index, localPos))
                    {
                        list.Add(cw.id);
                    }
                    return list;
                }
            }
            int curveAtPosition = this.GetCurveAtPosition(viewPos, out vector2);
            if (this.CreateKeyFromClick(curveAtPosition, vector2.x) && (curveAtPosition >= 0))
            {
                list.Add(this.m_AnimationCurves[curveAtPosition].id);
            }
            return list;
        }

        private bool CreateKeyFromClick(int curveIndex, float time)
        {
            time = Mathf.Clamp(time, this.settings.hRangeMin, this.settings.hRangeMax);
            if (curveIndex >= 0)
            {
                CurveSelection curveSelection = null;
                CurveWrapper cw = this.m_AnimationCurves[curveIndex];
                if (cw.animationIsEditable)
                {
                    if (cw.groupId == -1)
                    {
                        curveSelection = this.AddKeyAtTime(cw, time);
                    }
                    else
                    {
                        foreach (CurveWrapper wrapper2 in this.m_AnimationCurves)
                        {
                            if (wrapper2.groupId == cw.groupId)
                            {
                                CurveSelection selection2 = this.AddKeyAtTime(wrapper2, time);
                                if (wrapper2.id == cw.id)
                                {
                                    curveSelection = selection2;
                                }
                            }
                        }
                    }
                    if (curveSelection != null)
                    {
                        this.ClearSelection();
                        this.AddSelection(curveSelection);
                        this.RecalcSecondarySelection();
                    }
                    else
                    {
                        this.SelectNone();
                    }
                    return true;
                }
            }
            return false;
        }

        private bool CreateKeyFromClick(int curveIndex, Vector2 localPos)
        {
            localPos.x = Mathf.Clamp(localPos.x, this.settings.hRangeMin, this.settings.hRangeMax);
            if (curveIndex >= 0)
            {
                CurveSelection curveSelection = null;
                CurveWrapper cw = this.m_AnimationCurves[curveIndex];
                if (cw.animationIsEditable)
                {
                    if (cw.groupId == -1)
                    {
                        curveSelection = this.AddKeyAtPosition(cw, localPos);
                    }
                    else
                    {
                        foreach (CurveWrapper wrapper2 in this.m_AnimationCurves)
                        {
                            if (wrapper2.groupId == cw.groupId)
                            {
                                if (wrapper2.id == cw.id)
                                {
                                    curveSelection = this.AddKeyAtPosition(wrapper2, localPos);
                                }
                                else
                                {
                                    this.AddKeyAtTime(wrapper2, localPos.x);
                                }
                            }
                        }
                    }
                    if (curveSelection != null)
                    {
                        this.ClearSelection();
                        this.AddSelection(curveSelection);
                        this.RecalcSecondarySelection();
                    }
                    else
                    {
                        this.SelectNone();
                    }
                    return true;
                }
            }
            return false;
        }

        private List<Vector3> CreateRegion(CurveWrapper minCurve, CurveWrapper maxCurve, float deltaTime)
        {
            List<Vector3> list = new List<Vector3>();
            List<float> list2 = new List<float>();
            float item = deltaTime;
            while (item <= 1f)
            {
                list2.Add(item);
                item += deltaTime;
            }
            if (item != 1f)
            {
                list2.Add(1f);
            }
            foreach (Keyframe keyframe in maxCurve.curve.keys)
            {
                if ((keyframe.time > 0f) && (keyframe.time < 1f))
                {
                    list2.Add(keyframe.time - 0.0001f);
                    list2.Add(keyframe.time);
                    list2.Add(keyframe.time + 0.0001f);
                }
            }
            foreach (Keyframe keyframe2 in minCurve.curve.keys)
            {
                if ((keyframe2.time > 0f) && (keyframe2.time < 1f))
                {
                    list2.Add(keyframe2.time - 0.0001f);
                    list2.Add(keyframe2.time);
                    list2.Add(keyframe2.time + 0.0001f);
                }
            }
            list2.Sort();
            Vector3 v = new Vector3(0f, maxCurve.renderer.EvaluateCurveSlow(0f), 0f);
            Vector3 vector2 = new Vector3(0f, minCurve.renderer.EvaluateCurveSlow(0f), 0f);
            Vector3 vector3 = this.DrawingToOffsetViewMatrix(maxCurve).MultiplyPoint(v);
            Vector3 vector4 = this.DrawingToOffsetViewMatrix(minCurve).MultiplyPoint(vector2);
            for (int i = 0; i < list2.Count; i++)
            {
                float x = list2[i];
                Vector3 vector5 = new Vector3(x, maxCurve.renderer.EvaluateCurveSlow(x), 0f);
                Vector3 vector6 = new Vector3(x, minCurve.renderer.EvaluateCurveSlow(x), 0f);
                Vector3 vector7 = this.DrawingToOffsetViewMatrix(maxCurve).MultiplyPoint(vector5);
                Vector3 vector8 = this.DrawingToOffsetViewMatrix(minCurve).MultiplyPoint(vector6);
                if ((v.y >= vector2.y) && (vector5.y >= vector6.y))
                {
                    list.Add(vector3);
                    list.Add(vector8);
                    list.Add(vector4);
                    list.Add(vector3);
                    list.Add(vector7);
                    list.Add(vector8);
                }
                else if ((v.y <= vector2.y) && (vector5.y <= vector6.y))
                {
                    list.Add(vector4);
                    list.Add(vector7);
                    list.Add(vector3);
                    list.Add(vector4);
                    list.Add(vector8);
                    list.Add(vector7);
                }
                else
                {
                    Vector2 zero = Vector2.zero;
                    if (Mathf.LineIntersection(vector3, vector7, vector4, vector8, ref zero))
                    {
                        list.Add(vector3);
                        list.Add((Vector3) zero);
                        list.Add(vector4);
                        list.Add(vector7);
                        list.Add((Vector3) zero);
                        list.Add(vector8);
                    }
                    else
                    {
                        UnityEngine.Debug.Log("Error: No intersection found! There should be one...");
                    }
                }
                v = vector5;
                vector2 = vector6;
                vector3 = vector7;
                vector4 = vector8;
            }
            return list;
        }

        public void CurveGUI()
        {
            if (this.m_PointRenderer == null)
            {
                this.m_PointRenderer = new CurveControlPointRenderer(this.styles);
            }
            if (this.m_RectangleTool == null)
            {
                this.m_RectangleTool = new CurveEditorRectangleTool();
                this.m_RectangleTool.Initialize(this);
            }
            GUI.BeginGroup(base.drawRect);
            this.Init();
            GUIUtility.GetControlID(s_SelectKeyHash, FocusType.Passive);
            Color white = Color.white;
            GUI.backgroundColor = white;
            GUI.contentColor = white;
            Color color = GUI.color;
            Event current = Event.current;
            if (current.type != EventType.Repaint)
            {
                this.EditSelectedPoints();
            }
            EventType type = current.type;
            switch (type)
            {
                case EventType.ValidateCommand:
                case EventType.ExecuteCommand:
                {
                    bool flag = current.type == EventType.ExecuteCommand;
                    switch (current.commandName)
                    {
                        case "Delete":
                            if (this.hasSelection)
                            {
                                if (flag)
                                {
                                    this.DeleteSelectedKeys();
                                }
                                current.Use();
                            }
                            break;

                        case "FrameSelected":
                            if (flag)
                            {
                                this.FrameSelected(true, true);
                            }
                            current.Use();
                            break;

                        case "SelectAll":
                            if (flag)
                            {
                                this.SelectAll();
                            }
                            current.Use();
                            break;
                    }
                    goto Label_03C0;
                }
                case EventType.ContextClick:
                {
                    CurveSelection curveSelection = this.FindNearest();
                    if (curveSelection != null)
                    {
                        List<KeyIdentifier> userData = new List<KeyIdentifier>();
                        bool flag2 = false;
                        foreach (CurveSelection selection2 in this.selectedCurves)
                        {
                            userData.Add(new KeyIdentifier(this.GetCurveFromSelection(selection2), selection2.curveID, selection2.key));
                            if ((selection2.curveID == curveSelection.curveID) && (selection2.key == curveSelection.key))
                            {
                                flag2 = true;
                            }
                        }
                        if (!flag2)
                        {
                            userData.Clear();
                            userData.Add(new KeyIdentifier(this.GetCurveFromSelection(curveSelection), curveSelection.curveID, curveSelection.key));
                            this.ClearSelection();
                            this.AddSelection(curveSelection);
                        }
                        bool flag3 = !this.selectedCurves.Exists(sel => !this.GetCurveWrapperFromSelection(sel).animationIsEditable);
                        this.m_MenuManager = new CurveMenuManager(this);
                        GenericMenu menu = new GenericMenu();
                        string text = (userData.Count <= 1) ? "Delete Key" : "Delete Keys";
                        string str3 = (userData.Count <= 1) ? "Edit Key..." : "Edit Keys...";
                        if (flag3)
                        {
                            menu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(this.DeleteKeys), userData);
                            menu.AddItem(new GUIContent(str3), false, new GenericMenu.MenuFunction2(this.StartEditingSelectedPointsContext), this.OffsetMousePositionInDrawing(this.GetCurveWrapperFromSelection(curveSelection)));
                        }
                        else
                        {
                            menu.AddDisabledItem(new GUIContent(text));
                            menu.AddDisabledItem(new GUIContent(str3));
                        }
                        if (flag3)
                        {
                            menu.AddSeparator("");
                            this.m_MenuManager.AddTangentMenuItems(menu, userData);
                        }
                        menu.ShowAsContext();
                        Event.current.Use();
                    }
                    goto Label_03C0;
                }
            }
            if (type == EventType.KeyDown)
            {
                if (((current.keyCode == KeyCode.Backspace) || (current.keyCode == KeyCode.Delete)) && this.hasSelection)
                {
                    this.DeleteSelectedKeys();
                    current.Use();
                }
                if (current.keyCode == KeyCode.A)
                {
                    this.FrameClip(true, true);
                    current.Use();
                }
            }
        Label_03C0:
            EditorGUI.BeginChangeCheck();
            GUI.color = color;
            this.m_RectangleTool.HandleOverlayEvents();
            this.DragTangents();
            this.m_RectangleTool.HandleEvents();
            this.EditAxisLabels();
            this.SelectPoints();
            if (EditorGUI.EndChangeCheck())
            {
                this.RecalcSecondarySelection();
                this.RecalcCurveSelection();
            }
            EditorGUI.BeginChangeCheck();
            Vector2 vector = this.MovePoints();
            if (EditorGUI.EndChangeCheck() && (this.m_DraggingKey != null))
            {
                if (this.syncTimeDuringDrag)
                {
                    this.activeTime = (vector.x + this.s_StartClickedTime) + this.m_DraggingKey.timeOffset;
                }
                this.m_MoveCoord = vector;
            }
            if (current.type == EventType.Repaint)
            {
                this.DrawCurves();
                this.m_RectangleTool.OnGUI();
                this.DrawCurvesTangents();
                this.DrawCurvesOverlay();
                this.m_RectangleTool.OverlayOnGUI();
                this.EditSelectedPoints();
            }
            GUI.color = color;
            GUI.EndGroup();
        }

        private void DeleteKeys(object obj)
        {
            string str;
            List<KeyIdentifier> list = (List<KeyIdentifier>) obj;
            if (list.Count > 1)
            {
                str = "Delete Keys";
            }
            else
            {
                str = "Delete Key";
            }
            this.SaveKeySelection(str);
            List<int> curveIds = new List<int>();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if ((this.settings.allowDeleteLastKeyInCurve || (list[i].curve.keys.Length != 1)) && this.GetCurveWrapperFromID(list[i].curveId).animationIsEditable)
                {
                    list[i].curve.RemoveKey(list[i].key);
                    AnimationUtility.UpdateTangentsFromMode(list[i].curve);
                    curveIds.Add(list[i].curveId);
                }
            }
            this.UpdateCurves(curveIds, str);
            this.SelectNone();
        }

        internal void DeleteSelectedKeys()
        {
            string str;
            if (this.selectedCurves.Count > 1)
            {
                str = "Delete Keys";
            }
            else
            {
                str = "Delete Key";
            }
            this.SaveKeySelection(str);
            for (int i = this.selectedCurves.Count - 1; i >= 0; i--)
            {
                CurveSelection curveSelection = this.selectedCurves[i];
                CurveWrapper curveWrapperFromSelection = this.GetCurveWrapperFromSelection(curveSelection);
                if (((curveWrapperFromSelection != null) && curveWrapperFromSelection.animationIsEditable) && (this.settings.allowDeleteLastKeyInCurve || (curveWrapperFromSelection.curve.keys.Length != 1)))
                {
                    curveWrapperFromSelection.curve.RemoveKey(curveSelection.key);
                    AnimationUtility.UpdateTangentsFromMode(curveWrapperFromSelection.curve);
                    curveWrapperFromSelection.changed = true;
                    GUI.changed = true;
                }
            }
            this.SelectNone();
        }

        private void DragTangents()
        {
            CurveSelection selectedTangentPoint;
            CurveWrapper wrapper2;
            Keyframe keyframeFromSelection;
            Event current = Event.current;
            int controlID = GUIUtility.GetControlID(s_TangentControlIDHash, FocusType.Passive);
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if ((current.button == 0) && !current.alt)
                    {
                        this.m_SelectedTangentPoint = null;
                        float num2 = 100f;
                        Vector2 mousePosition = Event.current.mousePosition;
                        foreach (CurveSelection selection in this.selectedCurves)
                        {
                            CurveWrapper curveWrapperFromSelection = this.GetCurveWrapperFromSelection(selection);
                            if (curveWrapperFromSelection != null)
                            {
                                if (this.IsLeftTangentEditable(selection))
                                {
                                    CurveSelection selection2 = new CurveSelection(selection.curveID, selection.key, CurveSelection.SelectionType.InTangent);
                                    Vector2 vector2 = this.DrawingToOffsetViewTransformPoint(curveWrapperFromSelection, this.GetPosition(selection2)) - mousePosition;
                                    float sqrMagnitude = vector2.sqrMagnitude;
                                    if (sqrMagnitude <= num2)
                                    {
                                        this.m_SelectedTangentPoint = selection2;
                                        num2 = sqrMagnitude;
                                    }
                                }
                                if (this.IsRightTangentEditable(selection))
                                {
                                    CurveSelection selection3 = new CurveSelection(selection.curveID, selection.key, CurveSelection.SelectionType.OutTangent);
                                    Vector2 vector3 = this.DrawingToOffsetViewTransformPoint(curveWrapperFromSelection, this.GetPosition(selection3)) - mousePosition;
                                    float num4 = vector3.sqrMagnitude;
                                    if (num4 <= num2)
                                    {
                                        this.m_SelectedTangentPoint = selection3;
                                        num2 = num4;
                                    }
                                }
                            }
                        }
                        if (this.m_SelectedTangentPoint != null)
                        {
                            this.SaveKeySelection("Edit Curve");
                            GUIUtility.hotControl = controlID;
                            current.Use();
                        }
                    }
                    return;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                        current.Use();
                    }
                    return;

                case EventType.MouseMove:
                case EventType.KeyDown:
                case EventType.KeyUp:
                case EventType.ScrollWheel:
                    return;

                case EventType.MouseDrag:
                {
                    if (GUIUtility.hotControl != controlID)
                    {
                        return;
                    }
                    selectedTangentPoint = this.m_SelectedTangentPoint;
                    wrapper2 = this.GetCurveWrapperFromSelection(selectedTangentPoint);
                    if ((wrapper2 == null) || !wrapper2.animationIsEditable)
                    {
                        goto Label_0352;
                    }
                    Vector2 vector4 = this.OffsetMousePositionInDrawing(wrapper2);
                    keyframeFromSelection = this.GetKeyframeFromSelection(selectedTangentPoint);
                    if (selectedTangentPoint.type != CurveSelection.SelectionType.InTangent)
                    {
                        if (selectedTangentPoint.type == CurveSelection.SelectionType.OutTangent)
                        {
                            Vector2 vector6 = vector4 - new Vector2(keyframeFromSelection.time, keyframeFromSelection.value);
                            if (vector6.x > 0.0001f)
                            {
                                keyframeFromSelection.outTangent = vector6.y / vector6.x;
                            }
                            else
                            {
                                keyframeFromSelection.outTangent = float.PositiveInfinity;
                            }
                            AnimationUtility.SetKeyRightTangentMode(ref keyframeFromSelection, AnimationUtility.TangentMode.Free);
                            if (!AnimationUtility.GetKeyBroken(keyframeFromSelection))
                            {
                                keyframeFromSelection.inTangent = keyframeFromSelection.outTangent;
                                AnimationUtility.SetKeyLeftTangentMode(ref keyframeFromSelection, AnimationUtility.TangentMode.Free);
                            }
                        }
                        goto Label_0314;
                    }
                    Vector2 vector5 = vector4 - new Vector2(keyframeFromSelection.time, keyframeFromSelection.value);
                    if (vector5.x >= -0.0001f)
                    {
                        keyframeFromSelection.inTangent = float.PositiveInfinity;
                        break;
                    }
                    keyframeFromSelection.inTangent = vector5.y / vector5.x;
                    break;
                }
                case EventType.Repaint:
                    if (GUIUtility.hotControl == controlID)
                    {
                        Rect position = new Rect(current.mousePosition.x - 10f, current.mousePosition.y - 10f, 20f, 20f);
                        EditorGUIUtility.AddCursorRect(position, MouseCursor.MoveArrow);
                    }
                    return;

                default:
                    return;
            }
            AnimationUtility.SetKeyLeftTangentMode(ref keyframeFromSelection, AnimationUtility.TangentMode.Free);
            if (!AnimationUtility.GetKeyBroken(keyframeFromSelection))
            {
                keyframeFromSelection.outTangent = keyframeFromSelection.inTangent;
                AnimationUtility.SetKeyRightTangentMode(ref keyframeFromSelection, AnimationUtility.TangentMode.Free);
            }
        Label_0314:
            selectedTangentPoint.key = wrapper2.curve.MoveKey(selectedTangentPoint.key, keyframeFromSelection);
            AnimationUtility.UpdateTangentsFromModeSurrounding(wrapper2.curve, selectedTangentPoint.key);
            wrapper2.changed = true;
            GUI.changed = true;
        Label_0352:
            Event.current.Use();
        }

        private void DrawCurve(CurveWrapper cw, bool hasFocus)
        {
            CurveRenderer renderer = cw.renderer;
            Color a = cw.color;
            if (this.IsDraggingCurve(cw) || (cw.selected == CurveWrapper.SelectionMode.Selected))
            {
                a = Color.Lerp(a, Color.white, 0.3f);
            }
            else if (this.settings.useFocusColors && !hasFocus)
            {
                a = (Color) (a * 0.5f);
                a.a = 0.8f;
            }
            Rect shownArea = base.shownArea;
            renderer.DrawCurve(shownArea.xMin - cw.timeOffset, shownArea.xMax, a, this.DrawingToOffsetViewMatrix(cw), (Color) (this.settings.wrapColor * cw.wrapColorMultiplier));
        }

        private void DrawCurveAndPoints(CurveWrapper cw, List<CurveSelection> selection, bool hasFocus)
        {
            this.DrawCurve(cw, hasFocus);
            this.DrawPointsOnCurve(cw, selection, hasFocus);
        }

        private void DrawCurveLine(CurveWrapper cw, Vector2 lhs, Vector2 rhs)
        {
            GL.Vertex(this.DrawingToOffsetViewTransformPoint(cw, new Vector3(lhs.x, lhs.y, 0f)));
            GL.Vertex(this.DrawingToOffsetViewTransformPoint(cw, new Vector3(rhs.x, rhs.y, 0f)));
        }

        private void DrawCurves()
        {
            if (Event.current.type == EventType.Repaint)
            {
                this.m_PointRenderer.Clear();
                for (int i = 0; i < this.m_DrawOrder.Count; i++)
                {
                    CurveWrapper curveWrapperFromID = this.GetCurveWrapperFromID(this.m_DrawOrder[i]);
                    if (((curveWrapperFromID != null) && !curveWrapperFromID.hidden) && (curveWrapperFromID.curve.length != 0))
                    {
                        CurveWrapper wrapper2 = null;
                        if (i < (this.m_DrawOrder.Count - 1))
                        {
                            wrapper2 = this.GetCurveWrapperFromID(this.m_DrawOrder[i + 1]);
                        }
                        if (this.IsRegion(curveWrapperFromID, wrapper2))
                        {
                            i++;
                            bool hasFocus = this.ShouldCurveHaveFocus(i, curveWrapperFromID, wrapper2);
                            this.DrawCurvesAndRegion(curveWrapperFromID, wrapper2, !this.IsRegionCurveSelected(curveWrapperFromID, wrapper2) ? null : this.selectedCurves, hasFocus);
                        }
                        else
                        {
                            bool flag2 = this.ShouldCurveHaveFocus(i, curveWrapperFromID, null);
                            this.DrawCurveAndPoints(curveWrapperFromID, !this.IsCurveSelected(curveWrapperFromID) ? null : this.selectedCurves, flag2);
                        }
                    }
                }
                this.m_PointRenderer.Render();
            }
        }

        private void DrawCurvesAndRegion(CurveWrapper cw1, CurveWrapper cw2, List<CurveSelection> selection, bool hasFocus)
        {
            this.DrawRegion(cw1, cw2, hasFocus);
            this.DrawCurveAndPoints(cw1, !this.IsCurveSelected(cw1) ? null : selection, hasFocus);
            this.DrawCurveAndPoints(cw2, !this.IsCurveSelected(cw2) ? null : selection, hasFocus);
        }

        private void DrawCurvesOverlay()
        {
            if ((this.m_DraggingCurveOrRegion == null) && ((this.m_DraggingKey != null) && (this.settings.rectangleToolFlags == CurveEditorSettings.RectangleToolFlags.NoRectangleTool)))
            {
                GUI.color = Color.white;
                float vRangeMin = base.vRangeMin;
                float vRangeMax = base.vRangeMax;
                vRangeMin = Mathf.Max(vRangeMin, this.m_DraggingKey.vRangeMin);
                vRangeMax = Mathf.Min(vRangeMax, this.m_DraggingKey.vRangeMax);
                Vector2 lhs = this.m_DraggedCoord + this.m_MoveCoord;
                lhs.x = Mathf.Clamp(lhs.x, base.hRangeMin, base.hRangeMax);
                lhs.y = Mathf.Clamp(lhs.y, vRangeMin, vRangeMax);
                Vector2 vector2 = this.DrawingToOffsetViewTransformPoint(this.m_DraggingKey, lhs);
                Vector2 vector3 = (this.m_DraggingKey.getAxisUiScalarsCallback == null) ? Vector2.one : this.m_DraggingKey.getAxisUiScalarsCallback();
                if (vector3.x >= 0f)
                {
                    lhs.x *= vector3.x;
                }
                if (vector3.y >= 0f)
                {
                    lhs.y *= vector3.y;
                }
                GUIContent content = new GUIContent($"{base.FormatTime(lhs.x, this.invSnap, this.timeFormat)}, {base.FormatValue(lhs.y)}");
                Vector2 vector4 = this.styles.dragLabel.CalcSize(content);
                EditorGUI.DoDropShadowLabel(new Rect(vector2.x, vector2.y - vector4.y, vector4.x, vector4.y), content, this.styles.dragLabel, 0.3f);
            }
        }

        private void DrawCurvesTangents()
        {
            if (this.m_DraggingCurveOrRegion == null)
            {
                HandleUtility.ApplyWireMaterial();
                GL.Begin(1);
                GL.Color(this.m_TangentColor * new Color(1f, 1f, 1f, 0.75f));
                for (int i = 0; i < this.selectedCurves.Count; i++)
                {
                    CurveSelection selection = this.selectedCurves[i];
                    if (!selection.semiSelected)
                    {
                        Vector2 position = this.GetPosition(selection);
                        CurveWrapper curveWrapperFromSelection = this.GetCurveWrapperFromSelection(selection);
                        if (curveWrapperFromSelection != null)
                        {
                            AnimationCurve curve = curveWrapperFromSelection.curve;
                            if (this.IsLeftTangentEditable(selection) && (this.GetKeyframeFromSelection(selection).time != curve.keys[0].time))
                            {
                                Vector2 lhs = this.GetPosition(new CurveSelection(selection.curveID, selection.key, CurveSelection.SelectionType.InTangent));
                                this.DrawCurveLine(curveWrapperFromSelection, lhs, position);
                            }
                            if (this.IsRightTangentEditable(selection) && (this.GetKeyframeFromSelection(selection).time != curve.keys[curve.keys.Length - 1].time))
                            {
                                Vector2 rhs = this.GetPosition(new CurveSelection(selection.curveID, selection.key, CurveSelection.SelectionType.OutTangent));
                                this.DrawCurveLine(curveWrapperFromSelection, position, rhs);
                            }
                        }
                    }
                }
                GL.End();
                this.m_PointRenderer.Clear();
                GUI.color = this.m_TangentColor;
                for (int j = 0; j < this.selectedCurves.Count; j++)
                {
                    CurveSelection curveSelection = this.selectedCurves[j];
                    if (!curveSelection.semiSelected)
                    {
                        CurveWrapper cw = this.GetCurveWrapperFromSelection(curveSelection);
                        if (cw != null)
                        {
                            AnimationCurve curve2 = cw.curve;
                            if (this.IsLeftTangentEditable(curveSelection) && (this.GetKeyframeFromSelection(curveSelection).time != curve2.keys[0].time))
                            {
                                Vector2 viewPos = this.DrawingToOffsetViewTransformPoint(cw, this.GetPosition(new CurveSelection(curveSelection.curveID, curveSelection.key, CurveSelection.SelectionType.InTangent)));
                                this.DrawPoint(viewPos, CurveWrapper.SelectionMode.None);
                            }
                            if (this.IsRightTangentEditable(curveSelection) && (this.GetKeyframeFromSelection(curveSelection).time != curve2.keys[curve2.keys.Length - 1].time))
                            {
                                Vector2 vector5 = this.DrawingToOffsetViewTransformPoint(cw, this.GetPosition(new CurveSelection(curveSelection.curveID, curveSelection.key, CurveSelection.SelectionType.OutTangent)));
                                this.DrawPoint(vector5, CurveWrapper.SelectionMode.None);
                            }
                        }
                    }
                }
                this.m_PointRenderer.Render();
            }
        }

        private Matrix4x4 DrawingToOffsetViewMatrix(CurveWrapper cw) => 
            (this.TimeOffsetMatrix(cw) * base.drawingToViewMatrix);

        private Vector2 DrawingToOffsetViewTransformPoint(CurveWrapper cw, Vector2 lhs) => 
            new Vector2(((lhs.x * this.m_Scale.x) + this.m_Translation.x) + (cw.timeOffset * this.m_Scale.x), (lhs.y * this.m_Scale.y) + this.m_Translation.y);

        private Vector3 DrawingToOffsetViewTransformPoint(CurveWrapper cw, Vector3 lhs) => 
            new Vector3(((lhs.x * this.m_Scale.x) + this.m_Translation.x) + (cw.timeOffset * this.m_Scale.x), (lhs.y * this.m_Scale.y) + this.m_Translation.y, 0f);

        private void DrawLine(Vector2 lhs, Vector2 rhs)
        {
            GL.Vertex(base.DrawingToViewTransformPoint(new Vector3(lhs.x, lhs.y, 0f)));
            GL.Vertex(base.DrawingToViewTransformPoint(new Vector3(rhs.x, rhs.y, 0f)));
        }

        private void DrawPoint(Vector2 viewPos, CurveWrapper.SelectionMode selected)
        {
            this.DrawPoint(viewPos, selected, MouseCursor.Arrow);
        }

        private void DrawPoint(Vector2 viewPos, CurveWrapper.SelectionMode selected, MouseCursor mouseCursor)
        {
            Rect rect = new Rect(Mathf.Floor(viewPos.x) - 4f, Mathf.Floor(viewPos.y) - 4f, (float) this.styles.pointIcon.width, (float) this.styles.pointIcon.height);
            Vector2 center = rect.center;
            Vector2 vector2 = center - this.m_PreviousDrawPointCenter;
            if (vector2.magnitude > 8f)
            {
                if (selected == CurveWrapper.SelectionMode.None)
                {
                    this.m_PointRenderer.AddPoint(rect, GUI.color);
                }
                else if (selected == CurveWrapper.SelectionMode.Selected)
                {
                    this.m_PointRenderer.AddSelectedPoint(rect, GUI.color);
                }
                else
                {
                    this.m_PointRenderer.AddSemiSelectedPoint(rect, GUI.color);
                }
                if ((mouseCursor != MouseCursor.Arrow) && (GUIUtility.hotControl == 0))
                {
                    EditorGUIUtility.AddCursorRect(rect, mouseCursor);
                }
                this.m_PreviousDrawPointCenter = center;
            }
        }

        private void DrawPointsOnCurve(CurveWrapper cw, List<CurveSelection> selected, bool hasFocus)
        {
            this.m_PreviousDrawPointCenter = new Vector2(float.MinValue, float.MinValue);
            if (selected == null)
            {
                Color color = cw.color;
                if (this.settings.useFocusColors && !hasFocus)
                {
                    color = (Color) (color * 0.5f);
                }
                GUI.color = color;
                foreach (Keyframe keyframe in cw.curve.keys)
                {
                    this.DrawPoint(this.DrawingToOffsetViewTransformPoint(cw, new Vector2(keyframe.time, keyframe.value)), CurveWrapper.SelectionMode.None);
                }
            }
            else
            {
                GUI.color = Color.Lerp(cw.color, Color.white, 0.2f);
                int num2 = 0;
                while ((num2 < selected.Count) && (selected[num2].curveID != cw.id))
                {
                    num2++;
                }
                int num3 = 0;
                foreach (Keyframe keyframe2 in cw.curve.keys)
                {
                    if (((num2 < selected.Count) && (selected[num2].key == num3)) && (selected[num2].curveID == cw.id))
                    {
                        if (selected[num2].semiSelected)
                        {
                            this.DrawPoint(this.DrawingToOffsetViewTransformPoint(cw, new Vector2(keyframe2.time, keyframe2.value)), CurveWrapper.SelectionMode.SemiSelected);
                        }
                        else
                        {
                            this.DrawPoint(this.DrawingToOffsetViewTransformPoint(cw, new Vector2(keyframe2.time, keyframe2.value)), CurveWrapper.SelectionMode.Selected, (this.settings.rectangleToolFlags != CurveEditorSettings.RectangleToolFlags.NoRectangleTool) ? MouseCursor.Arrow : MouseCursor.MoveArrow);
                        }
                        num2++;
                    }
                    else
                    {
                        this.DrawPoint(this.DrawingToOffsetViewTransformPoint(cw, new Vector2(keyframe2.time, keyframe2.value)), CurveWrapper.SelectionMode.None);
                    }
                    num3++;
                }
                GUI.color = Color.white;
            }
        }

        public void DrawRegion(CurveWrapper curve1, CurveWrapper curve2, bool hasFocus)
        {
            if (Event.current.type == EventType.Repaint)
            {
                float deltaTime = 1f / (base.rect.width / 10f);
                List<Vector3> list = this.CreateRegion(curve1, curve2, deltaTime);
                Color a = curve1.color;
                if (this.IsDraggingRegion(curve1, curve2))
                {
                    a = Color.Lerp(a, Color.black, 0.1f);
                    a.a = 0.4f;
                }
                else if (this.settings.useFocusColors && !hasFocus)
                {
                    a = (Color) (a * 0.4f);
                    a.a = 0.1f;
                }
                else
                {
                    a = (Color) (a * 1f);
                    a.a = 0.4f;
                }
                Shader.SetGlobalColor("_HandleColor", a);
                HandleUtility.ApplyWireMaterial();
                GL.Begin(4);
                int num2 = list.Count / 3;
                for (int i = 0; i < num2; i++)
                {
                    GL.Color(a);
                    GL.Vertex(list[i * 3]);
                    GL.Vertex(list[(i * 3) + 1]);
                    GL.Vertex(list[(i * 3) + 2]);
                }
                GL.End();
            }
        }

        private void DrawWrapperPopups()
        {
            if (this.settings.showWrapperPopups)
            {
                int num;
                this.GetTopMostCurveID(out num);
                if (num != -1)
                {
                    CurveWrapper curveWrapperFromID = this.GetCurveWrapperFromID(num);
                    AnimationCurve curve = curveWrapperFromID.curve;
                    if (((curve != null) && (curve.length >= 2)) && (curve.preWrapMode != WrapMode.Default))
                    {
                        Color contentColor = GUI.contentColor;
                        Keyframe key = curve.keys[0];
                        WrapMode oldWrap = (curve == null) ? WrapMode.Default : curve.preWrapMode;
                        oldWrap = this.WrapModeIconPopup(key, oldWrap, -1.5f);
                        if ((curve != null) && (oldWrap != curve.preWrapMode))
                        {
                            curve.preWrapMode = oldWrap;
                            curveWrapperFromID.changed = true;
                        }
                        Keyframe keyframe2 = curve.keys[curve.length - 1];
                        WrapMode mode2 = (curve == null) ? WrapMode.Default : curve.postWrapMode;
                        mode2 = this.WrapModeIconPopup(keyframe2, mode2, 0.5f);
                        if ((curve != null) && (mode2 != curve.postWrapMode))
                        {
                            curve.postWrapMode = mode2;
                            curveWrapperFromID.changed = true;
                        }
                        if (curveWrapperFromID.changed)
                        {
                            curveWrapperFromID.renderer.SetWrap(curve.preWrapMode, curve.postWrapMode);
                            if (this.curvesUpdated != null)
                            {
                                this.curvesUpdated();
                            }
                        }
                        GUI.contentColor = contentColor;
                    }
                }
            }
        }

        private void EditAxisLabels()
        {
            int controlID = GUIUtility.GetControlID(0x1218b72, FocusType.Keyboard);
            List<CurveWrapper> curvesWithSameParameterSpace = new List<CurveWrapper>();
            Vector2 axisUiScalars = this.GetAxisUiScalars(curvesWithSameParameterSpace);
            if (((axisUiScalars.y >= 0f) && (curvesWithSameParameterSpace.Count > 0)) && (curvesWithSameParameterSpace[0].setAxisUiScalarsCallback != null))
            {
                Rect position = new Rect(0f, base.topmargin - 8f, base.leftmargin - 4f, 16f);
                Rect rect2 = position;
                rect2.y -= position.height;
                Event current = Event.current;
                switch (current.GetTypeForControl(controlID))
                {
                    case EventType.MouseDown:
                        if (current.button == 0)
                        {
                            if (rect2.Contains(Event.current.mousePosition) && (GUIUtility.hotControl == 0))
                            {
                                GUIUtility.keyboardControl = 0;
                                GUIUtility.hotControl = controlID;
                                GUI.changed = true;
                                current.Use();
                            }
                            if (!position.Contains(Event.current.mousePosition))
                            {
                                GUIUtility.keyboardControl = 0;
                            }
                        }
                        break;

                    case EventType.MouseUp:
                        if (GUIUtility.hotControl == controlID)
                        {
                            GUIUtility.hotControl = 0;
                        }
                        break;

                    case EventType.MouseDrag:
                        if (GUIUtility.hotControl == controlID)
                        {
                            float num2 = Mathf.Clamp01(Mathf.Max(axisUiScalars.y, Mathf.Pow(Mathf.Abs(axisUiScalars.y), 0.5f)) * 0.01f);
                            axisUiScalars.y += HandleUtility.niceMouseDelta * num2;
                            if (axisUiScalars.y < 0.001f)
                            {
                                axisUiScalars.y = 0.001f;
                            }
                            this.SetAxisUiScalars(axisUiScalars, curvesWithSameParameterSpace);
                            current.Use();
                        }
                        break;

                    case EventType.Repaint:
                        if (GUIUtility.hotControl == 0)
                        {
                            EditorGUIUtility.AddCursorRect(rect2, MouseCursor.SlideArrow);
                        }
                        break;
                }
                string kFloatFieldFormatString = EditorGUI.kFloatFieldFormatString;
                EditorGUI.kFloatFieldFormatString = this.m_AxisLabelFormat;
                float y = EditorGUI.FloatField(position, axisUiScalars.y, this.styles.axisLabelNumberField);
                if (axisUiScalars.y != y)
                {
                    this.SetAxisUiScalars(new Vector2(axisUiScalars.x, y), curvesWithSameParameterSpace);
                }
                EditorGUI.kFloatFieldFormatString = kFloatFieldFormatString;
            }
        }

        private void EditSelectedPoints()
        {
            Event current = Event.current;
            if (this.editingPoints && !this.hasSelection)
            {
                this.editingPoints = false;
            }
            bool flag = false;
            if (current.type == EventType.KeyDown)
            {
                if ((current.keyCode == KeyCode.KeypadEnter) || (current.keyCode == KeyCode.Return))
                {
                    if (this.hasSelection && !this.editingPoints)
                    {
                        this.StartEditingSelectedPoints();
                        current.Use();
                    }
                    else if (this.editingPoints)
                    {
                        this.FinishEditingPoints();
                        current.Use();
                    }
                }
                else if (current.keyCode == KeyCode.Escape)
                {
                    flag = true;
                }
            }
            if (this.editingPoints)
            {
                Vector2 vector = base.DrawingToViewTransformPoint(this.pointEditingFieldPosition);
                Rect rect = Rect.MinMaxRect(base.leftmargin, base.topmargin, base.rect.width - base.rightmargin, base.rect.height - base.bottommargin);
                vector.x = Mathf.Clamp(vector.x, rect.xMin, rect.xMax - 80f);
                vector.y = Mathf.Clamp(vector.y, rect.yMin, rect.yMax - 36f);
                EditorGUI.BeginChangeCheck();
                GUI.SetNextControlName("pointTimeField");
                float num = this.PointFieldForSelection(new Rect(vector.x, vector.y, 80f, 18f), 1, x => this.GetKeyframeFromSelection(x).time, (r, id, time) => base.TimeField(r, id, time, this.invSnap, this.timeFormat), "time");
                if (EditorGUI.EndChangeCheck())
                {
                    this.timeWasEdited = true;
                }
                EditorGUI.BeginChangeCheck();
                GUI.SetNextControlName("pointValueField");
                float y = this.PointFieldForSelection(new Rect(vector.x, vector.y + 18f, 80f, 18f), 2, x => this.GetKeyframeFromSelection(x).value, (r, id, value) => base.ValueField(r, id, value), "value");
                if (EditorGUI.EndChangeCheck())
                {
                    this.valueWasEdited = true;
                }
                if (this.timeWasEdited || this.valueWasEdited)
                {
                    this.SetSelectedKeyPositions(new Vector2(num, y), this.timeWasEdited, this.valueWasEdited);
                }
                if (flag)
                {
                    this.FinishEditingPoints();
                }
                if (this.focusedPointField != null)
                {
                    EditorGUI.FocusTextInControl(this.focusedPointField);
                    if (current.type == EventType.Repaint)
                    {
                        this.focusedPointField = null;
                    }
                }
                if ((current.type == EventType.KeyDown) && (current.character == '\t'))
                {
                    this.focusedPointField = (GUI.GetNameOfFocusedControl() != "pointValueField") ? "pointValueField" : "pointTimeField";
                    current.Use();
                }
                if (current.type == EventType.MouseDown)
                {
                    this.FinishEditingPoints();
                }
            }
        }

        public void EndLiveEdit()
        {
            this.m_CurveBackups = null;
        }

        public void EndTimeRangeSelection()
        {
            if (!this.s_TimeRangeSelectionActive)
            {
                UnityEngine.Debug.LogError("EndTimeRangeSelection can only be called after BeginTimeRangeSelection");
            }
            else
            {
                this.s_TimeRangeSelectionStart = this.s_TimeRangeSelectionEnd = 0f;
                this.s_TimeRangeSelectionActive = false;
            }
        }

        private CurveSelection FindNearest()
        {
            Vector2 mousePosition = Event.current.mousePosition;
            bool flag = false;
            int curveID = -1;
            int key = -1;
            float num3 = 100f;
            for (int i = this.m_DrawOrder.Count - 1; i >= 0; i--)
            {
                CurveWrapper curveWrapperFromID = this.GetCurveWrapperFromID(this.m_DrawOrder[i]);
                if (!curveWrapperFromID.readOnly && !curveWrapperFromID.hidden)
                {
                    for (int j = 0; j < curveWrapperFromID.curve.keys.Length; j++)
                    {
                        Keyframe keyframe = curveWrapperFromID.curve.keys[j];
                        Vector2 vector2 = this.GetGUIPoint(curveWrapperFromID, (Vector3) new Vector2(keyframe.time, keyframe.value)) - mousePosition;
                        float sqrMagnitude = vector2.sqrMagnitude;
                        if (sqrMagnitude <= 16f)
                        {
                            return new CurveSelection(curveWrapperFromID.id, j);
                        }
                        if (sqrMagnitude < num3)
                        {
                            flag = true;
                            curveID = curveWrapperFromID.id;
                            key = j;
                            num3 = sqrMagnitude;
                        }
                    }
                    if ((i == (this.m_DrawOrder.Count - 1)) && (curveID >= 0))
                    {
                        num3 = 16f;
                    }
                }
            }
            if (flag)
            {
                return new CurveSelection(curveID, key);
            }
            return null;
        }

        private void FinishEditingPoints()
        {
            this.editingPoints = false;
            this.EndLiveEdit();
        }

        public void FrameClip(bool horizontally, bool vertically)
        {
            Bounds curveBounds = this.curveBounds;
            if (curveBounds.size != Vector3.zero)
            {
                if (horizontally)
                {
                    base.SetShownHRangeInsideMargins(curveBounds.min.x, curveBounds.max.x);
                }
                if (vertically)
                {
                    base.SetShownVRangeInsideMargins(curveBounds.min.y, curveBounds.max.y);
                }
            }
        }

        public void FrameSelected(bool horizontally, bool vertically)
        {
            if (!this.hasSelection)
            {
                this.FrameClip(horizontally, vertically);
            }
            else
            {
                Bounds selectionBounds = this.selectionBounds;
                float x = Mathf.Max(selectionBounds.size.x, 0.1f);
                selectionBounds.size = new Vector3(x, Mathf.Max(selectionBounds.size.y, 0.1f), 0f);
                if (horizontally)
                {
                    base.SetShownHRangeInsideMargins(selectionBounds.min.x, selectionBounds.max.x);
                }
                if (vertically)
                {
                    base.SetShownVRangeInsideMargins(selectionBounds.min.y, selectionBounds.max.y);
                }
            }
        }

        private Vector2 GetAxisUiScalars(List<CurveWrapper> curvesWithSameParameterSpace)
        {
            if (this.selectedCurves.Count <= 1)
            {
                if (this.m_DrawOrder.Count > 0)
                {
                    CurveWrapper curveWrapperFromID = this.GetCurveWrapperFromID(this.m_DrawOrder[this.m_DrawOrder.Count - 1]);
                    if ((curveWrapperFromID != null) && (curveWrapperFromID.getAxisUiScalarsCallback != null))
                    {
                        if (curvesWithSameParameterSpace != null)
                        {
                            curvesWithSameParameterSpace.Add(curveWrapperFromID);
                        }
                        return curveWrapperFromID.getAxisUiScalarsCallback();
                    }
                }
                return Vector2.one;
            }
            Vector2 vector2 = new Vector2(-1f, -1f);
            if (this.selectedCurves.Count > 1)
            {
                bool flag = true;
                bool flag2 = true;
                Vector2 one = Vector2.one;
                for (int i = 0; i < this.selectedCurves.Count; i++)
                {
                    CurveWrapper curveWrapperFromSelection = this.GetCurveWrapperFromSelection(this.selectedCurves[i]);
                    if ((curveWrapperFromSelection != null) && (curveWrapperFromSelection.getAxisUiScalarsCallback != null))
                    {
                        Vector2 vector4 = curveWrapperFromSelection.getAxisUiScalarsCallback();
                        if (i == 0)
                        {
                            one = vector4;
                        }
                        else
                        {
                            if (Mathf.Abs((float) (vector4.x - one.x)) > 1E-05f)
                            {
                                flag = false;
                            }
                            if (Mathf.Abs((float) (vector4.y - one.y)) > 1E-05f)
                            {
                                flag2 = false;
                            }
                            one = vector4;
                        }
                        if (curvesWithSameParameterSpace != null)
                        {
                            curvesWithSameParameterSpace.Add(curveWrapperFromSelection);
                        }
                    }
                }
                if (flag)
                {
                    vector2.x = one.x;
                }
                if (flag2)
                {
                    vector2.y = one.y;
                }
            }
            return vector2;
        }

        private int GetCurveAtPosition(Vector2 viewPos, out Vector2 closestPointOnCurve)
        {
            int num = (int) Mathf.Sqrt(100f);
            float num2 = 100f;
            int index = -1;
            closestPointOnCurve = Vector3.zero;
            for (int i = this.m_DrawOrder.Count - 1; i >= 0; i--)
            {
                CurveWrapper curveWrapperFromID = this.GetCurveWrapperFromID(this.m_DrawOrder[i]);
                if (!curveWrapperFromID.hidden && !curveWrapperFromID.readOnly)
                {
                    Vector2 vector2;
                    Vector2 vector = this.OffsetViewToDrawingTransformPoint(curveWrapperFromID, viewPos);
                    vector2.x = vector.x - (((float) num) / base.scale.x);
                    vector2.y = curveWrapperFromID.renderer.EvaluateCurveSlow(vector2.x);
                    vector2 = this.DrawingToOffsetViewTransformPoint(curveWrapperFromID, vector2);
                    for (int j = -num; j < num; j++)
                    {
                        Vector2 vector4;
                        vector4.x = vector.x + (((float) (j + 1)) / base.scale.x);
                        vector4.y = curveWrapperFromID.renderer.EvaluateCurveSlow(vector4.x);
                        vector4 = this.DrawingToOffsetViewTransformPoint(curveWrapperFromID, vector4);
                        float num6 = HandleUtility.DistancePointLine((Vector3) viewPos, (Vector3) vector2, (Vector3) vector4);
                        num6 *= num6;
                        if (num6 < num2)
                        {
                            num2 = num6;
                            index = curveWrapperFromID.listIndex;
                            closestPointOnCurve = HandleUtility.ProjectPointLine((Vector3) viewPos, (Vector3) vector2, (Vector3) vector4);
                        }
                        vector2 = vector4;
                    }
                }
            }
            if (index >= 0)
            {
                closestPointOnCurve = this.OffsetViewToDrawingTransformPoint(this.m_AnimationCurves[index], closestPointOnCurve);
            }
            return index;
        }

        internal CurveWrapper GetCurveFromID(int curveID)
        {
            if (this.m_AnimationCurves != null)
            {
                foreach (CurveWrapper wrapper2 in this.m_AnimationCurves)
                {
                    if (wrapper2.id == curveID)
                    {
                        return wrapper2;
                    }
                }
            }
            return null;
        }

        internal AnimationCurve GetCurveFromSelection(CurveSelection curveSelection)
        {
            CurveWrapper curveWrapperFromSelection = this.GetCurveWrapperFromSelection(curveSelection);
            return curveWrapperFromSelection?.curve;
        }

        internal CurveWrapper GetCurveWrapperFromID(int curveID)
        {
            if (this.m_AnimationCurves != null)
            {
                foreach (CurveWrapper wrapper2 in this.m_AnimationCurves)
                {
                    if (wrapper2.id == curveID)
                    {
                        return wrapper2;
                    }
                }
            }
            return null;
        }

        internal CurveWrapper GetCurveWrapperFromSelection(CurveSelection curveSelection) => 
            this.GetCurveWrapperFromID(curveSelection.curveID);

        private Vector2 GetGUIPoint(CurveWrapper cw, Vector3 point) => 
            HandleUtility.WorldToGUIPoint(this.DrawingToOffsetViewTransformPoint(cw, point));

        internal Keyframe GetKeyframeFromSelection(CurveSelection curveSelection)
        {
            AnimationCurve curveFromSelection = this.GetCurveFromSelection(curveSelection);
            if ((curveFromSelection != null) && ((curveSelection.key >= 0) && (curveSelection.key < curveFromSelection.length)))
            {
                return curveFromSelection[curveSelection.key];
            }
            return new Keyframe();
        }

        private Vector2 GetPosition(CurveSelection selection)
        {
            Keyframe keyframeFromSelection = this.GetKeyframeFromSelection(selection);
            Vector2 vector = new Vector2(keyframeFromSelection.time, keyframeFromSelection.value);
            float num = 50f;
            if (selection.type == CurveSelection.SelectionType.InTangent)
            {
                Vector2 vec = new Vector2(1f, keyframeFromSelection.inTangent);
                if (keyframeFromSelection.inTangent == float.PositiveInfinity)
                {
                    vec = new Vector2(0f, -1f);
                }
                vec = base.NormalizeInViewSpace(vec);
                return (vector - ((Vector2) (vec * num)));
            }
            if (selection.type == CurveSelection.SelectionType.OutTangent)
            {
                Vector2 vector4 = new Vector2(1f, keyframeFromSelection.outTangent);
                if (keyframeFromSelection.outTangent == float.PositiveInfinity)
                {
                    vector4 = new Vector2(0f, -1f);
                }
                vector4 = base.NormalizeInViewSpace(vector4);
                return (vector + ((Vector2) (vector4 * num)));
            }
            return vector;
        }

        public bool GetTopMostCurveID(out int curveID)
        {
            if (this.m_DrawOrder.Count > 0)
            {
                curveID = this.m_DrawOrder[this.m_DrawOrder.Count - 1];
                return true;
            }
            curveID = -1;
            return false;
        }

        public void GridGUI()
        {
            if (Event.current.type == EventType.Repaint)
            {
                float yMin;
                float num2;
                GUI.BeginClip(base.drawRect);
                Color color = GUI.color;
                Vector2 axisUiScalars = this.GetAxisUiScalars(null);
                Rect shownArea = base.shownArea;
                base.hTicks.SetRanges(shownArea.xMin * axisUiScalars.x, shownArea.xMax * axisUiScalars.x, base.drawRect.xMin, base.drawRect.xMax);
                base.vTicks.SetRanges(shownArea.yMin * axisUiScalars.y, shownArea.yMax * axisUiScalars.y, base.drawRect.yMin, base.drawRect.yMax);
                HandleUtility.ApplyWireMaterial();
                GL.Begin(1);
                base.hTicks.SetTickStrengths((float) this.settings.hTickStyle.distMin, (float) this.settings.hTickStyle.distFull, false);
                if (this.settings.hTickStyle.stubs)
                {
                    yMin = shownArea.yMin;
                    num2 = shownArea.yMin - (40f / base.scale.y);
                }
                else
                {
                    yMin = Mathf.Max(shownArea.yMin, base.vRangeMin);
                    num2 = Mathf.Min(shownArea.yMax, base.vRangeMax);
                }
                for (int i = 0; i < base.hTicks.tickLevels; i++)
                {
                    float strengthOfLevel = base.hTicks.GetStrengthOfLevel(i);
                    if (strengthOfLevel > 0f)
                    {
                        GL.Color((Color) ((this.settings.hTickStyle.tickColor * new Color(1f, 1f, 1f, strengthOfLevel)) * new Color(1f, 1f, 1f, 0.75f)));
                        float[] ticksAtLevel = base.hTicks.GetTicksAtLevel(i, true);
                        for (int k = 0; k < ticksAtLevel.Length; k++)
                        {
                            ticksAtLevel[k] /= axisUiScalars.x;
                            if ((ticksAtLevel[k] > base.hRangeMin) && (ticksAtLevel[k] < base.hRangeMax))
                            {
                                this.DrawLine(new Vector2(ticksAtLevel[k], yMin), new Vector2(ticksAtLevel[k], num2));
                            }
                        }
                    }
                }
                GL.Color((Color) ((this.settings.hTickStyle.tickColor * new Color(1f, 1f, 1f, 1f)) * new Color(1f, 1f, 1f, 0.75f)));
                if (base.hRangeMin != float.NegativeInfinity)
                {
                    this.DrawLine(new Vector2(base.hRangeMin, yMin), new Vector2(base.hRangeMin, num2));
                }
                if (base.hRangeMax != float.PositiveInfinity)
                {
                    this.DrawLine(new Vector2(base.hRangeMax, yMin), new Vector2(base.hRangeMax, num2));
                }
                base.vTicks.SetTickStrengths((float) this.settings.vTickStyle.distMin, (float) this.settings.vTickStyle.distFull, false);
                if (this.settings.vTickStyle.stubs)
                {
                    yMin = shownArea.xMin;
                    num2 = shownArea.xMin + (40f / base.scale.x);
                }
                else
                {
                    yMin = Mathf.Max(shownArea.xMin, base.hRangeMin);
                    num2 = Mathf.Min(shownArea.xMax, base.hRangeMax);
                }
                for (int j = 0; j < base.vTicks.tickLevels; j++)
                {
                    float a = base.vTicks.GetStrengthOfLevel(j);
                    if (a > 0f)
                    {
                        GL.Color((Color) ((this.settings.vTickStyle.tickColor * new Color(1f, 1f, 1f, a)) * new Color(1f, 1f, 1f, 0.75f)));
                        float[] numArray2 = base.vTicks.GetTicksAtLevel(j, true);
                        for (int m = 0; m < numArray2.Length; m++)
                        {
                            numArray2[m] /= axisUiScalars.y;
                            if ((numArray2[m] > base.vRangeMin) && (numArray2[m] < base.vRangeMax))
                            {
                                this.DrawLine(new Vector2(yMin, numArray2[m]), new Vector2(num2, numArray2[m]));
                            }
                        }
                    }
                }
                GL.Color((Color) ((this.settings.vTickStyle.tickColor * new Color(1f, 1f, 1f, 1f)) * new Color(1f, 1f, 1f, 0.75f)));
                if (base.vRangeMin != float.NegativeInfinity)
                {
                    this.DrawLine(new Vector2(yMin, base.vRangeMin), new Vector2(num2, base.vRangeMin));
                }
                if (base.vRangeMax != float.PositiveInfinity)
                {
                    this.DrawLine(new Vector2(yMin, base.vRangeMax), new Vector2(num2, base.vRangeMax));
                }
                GL.End();
                if (this.settings.showAxisLabels)
                {
                    if ((this.settings.hTickStyle.distLabel > 0) && (axisUiScalars.x > 0f))
                    {
                        GUI.color = (Color) this.settings.hTickStyle.labelColor;
                        int levelWithMinSeparation = base.hTicks.GetLevelWithMinSeparation((float) this.settings.hTickStyle.distLabel);
                        int numberOfDecimalsForMinimumDifference = MathUtils.GetNumberOfDecimalsForMinimumDifference(base.hTicks.GetPeriodOfLevel(levelWithMinSeparation));
                        float[] numArray3 = base.hTicks.GetTicksAtLevel(levelWithMinSeparation, false);
                        float[] numArray4 = (float[]) numArray3.Clone();
                        float y = Mathf.Floor(base.drawRect.height);
                        for (int n = 0; n < numArray3.Length; n++)
                        {
                            numArray4[n] /= axisUiScalars.x;
                            if ((numArray4[n] >= base.hRangeMin) && (numArray4[n] <= base.hRangeMax))
                            {
                                Rect rect7;
                                TextAnchor upperCenter;
                                Vector2 vector4 = base.DrawingToViewTransformPoint(new Vector2(numArray4[n], 0f));
                                vector4 = new Vector2(Mathf.Floor(vector4.x), y);
                                float num13 = numArray3[n];
                                if (this.settings.hTickStyle.centerLabel)
                                {
                                    upperCenter = TextAnchor.UpperCenter;
                                    rect7 = new Rect(vector4.x, (vector4.y - 16f) - this.settings.hTickLabelOffset, 1f, 16f);
                                }
                                else
                                {
                                    upperCenter = TextAnchor.UpperLeft;
                                    rect7 = new Rect(vector4.x, (vector4.y - 16f) - this.settings.hTickLabelOffset, 50f, 16f);
                                }
                                if (this.styles.labelTickMarksX.alignment != upperCenter)
                                {
                                    this.styles.labelTickMarksX.alignment = upperCenter;
                                }
                                GUI.Label(rect7, num13.ToString("n" + numberOfDecimalsForMinimumDifference) + this.settings.hTickStyle.unit, this.styles.labelTickMarksX);
                            }
                        }
                    }
                    if ((this.settings.vTickStyle.distLabel > 0) && (axisUiScalars.y > 0f))
                    {
                        GUI.color = (Color) this.settings.vTickStyle.labelColor;
                        int level = base.vTicks.GetLevelWithMinSeparation((float) this.settings.vTickStyle.distLabel);
                        float[] numArray5 = base.vTicks.GetTicksAtLevel(level, false);
                        float[] numArray6 = (float[]) numArray5.Clone();
                        int num15 = MathUtils.GetNumberOfDecimalsForMinimumDifference(base.vTicks.GetPeriodOfLevel(level));
                        string format = "n" + num15;
                        this.m_AxisLabelFormat = format;
                        float width = 35f;
                        if (!this.settings.vTickStyle.stubs && (numArray5.Length > 1))
                        {
                            float num17 = numArray5[1];
                            float num18 = numArray5[numArray5.Length - 1];
                            string text = num17.ToString(format) + this.settings.vTickStyle.unit;
                            string str3 = num18.ToString(format) + this.settings.vTickStyle.unit;
                            width = Mathf.Max(this.styles.labelTickMarksY.CalcSize(new GUIContent(text)).x, this.styles.labelTickMarksY.CalcSize(new GUIContent(str3)).x) + 6f;
                        }
                        for (int num19 = 0; num19 < numArray5.Length; num19++)
                        {
                            numArray6[num19] /= axisUiScalars.y;
                            if ((numArray6[num19] >= base.vRangeMin) && (numArray6[num19] <= base.vRangeMax))
                            {
                                Rect rect8;
                                Vector2 vector7 = base.DrawingToViewTransformPoint(new Vector2(0f, numArray6[num19]));
                                vector7 = new Vector2(vector7.x, Mathf.Floor(vector7.y));
                                float num20 = numArray5[num19];
                                if (this.settings.vTickStyle.centerLabel)
                                {
                                    rect8 = new Rect(0f, vector7.y - 8f, base.leftmargin - 4f, 16f);
                                }
                                else
                                {
                                    rect8 = new Rect(0f, vector7.y - 13f, width, 16f);
                                }
                                GUI.Label(rect8, num20.ToString(format) + this.settings.vTickStyle.unit, this.styles.labelTickMarksY);
                            }
                        }
                    }
                }
                GUI.color = color;
                GUI.EndClip();
            }
        }

        private bool HandleCurveAndRegionMoveToFrontOnMouseDown(ref Vector2 timeValue, ref CurveWrapper[] curves)
        {
            Vector2 vector;
            int curveAtPosition = this.GetCurveAtPosition(Event.current.mousePosition, out vector);
            if (curveAtPosition >= 0)
            {
                this.MoveCurveToFront(this.m_AnimationCurves[curveAtPosition].id);
                timeValue = this.OffsetMousePositionInDrawing(this.m_AnimationCurves[curveAtPosition]);
                CurveWrapper[] wrapperArray1 = new CurveWrapper[] { this.m_AnimationCurves[curveAtPosition] };
                curves = wrapperArray1;
                return true;
            }
            for (int i = this.m_DrawOrder.Count - 1; i >= 0; i--)
            {
                CurveWrapper curveWrapperFromID = this.GetCurveWrapperFromID(this.m_DrawOrder[i]);
                if (((curveWrapperFromID != null) && !curveWrapperFromID.hidden) && (curveWrapperFromID.curve.length != 0))
                {
                    CurveWrapper wrapper2 = null;
                    if (i > 0)
                    {
                        wrapper2 = this.GetCurveWrapperFromID(this.m_DrawOrder[i - 1]);
                    }
                    if (this.IsRegion(curveWrapperFromID, wrapper2))
                    {
                        Vector2 vector2 = this.OffsetMousePositionInDrawing(curveWrapperFromID);
                        Vector2 vector3 = this.OffsetMousePositionInDrawing(wrapper2);
                        float num3 = curveWrapperFromID.renderer.EvaluateCurveSlow(vector2.x);
                        float num4 = wrapper2.renderer.EvaluateCurveSlow(vector3.x);
                        if (num3 > num4)
                        {
                            float num5 = num3;
                            num3 = num4;
                            num4 = num5;
                        }
                        if ((vector2.y >= num3) && (vector2.y <= num4))
                        {
                            timeValue = vector2;
                            CurveWrapper[] wrapperArray2 = new CurveWrapper[] { curveWrapperFromID, wrapper2 };
                            curves = wrapperArray2;
                            this.MoveCurveToFront(curveWrapperFromID.id);
                            return true;
                        }
                        i--;
                    }
                }
            }
            return false;
        }

        private void Init()
        {
        }

        public bool InLiveEdit() => 
            (this.m_CurveBackups != null);

        public void InvalidateBounds()
        {
            this.m_BoundsAreDirty = true;
        }

        private bool IsCurveSelected(CurveWrapper cw) => 
            ((cw != null) && (cw.selected != CurveWrapper.SelectionMode.None));

        public bool IsDraggingCurve(CurveWrapper cw) => 
            (((this.m_DraggingCurveOrRegion != null) && (this.m_DraggingCurveOrRegion.Length == 1)) && (this.m_DraggingCurveOrRegion[0] == cw));

        public bool IsDraggingCurveOrRegion() => 
            (this.m_DraggingCurveOrRegion != null);

        public bool IsDraggingKey() => 
            (this.m_DraggingKey != null);

        public bool IsDraggingRegion(CurveWrapper cw1, CurveWrapper cw2) => 
            (((this.m_DraggingCurveOrRegion != null) && (this.m_DraggingCurveOrRegion.Length == 2)) && ((this.m_DraggingCurveOrRegion[0] == cw1) || (this.m_DraggingCurveOrRegion[0] == cw2)));

        private bool IsLeftTangentEditable(CurveSelection selection)
        {
            switch (AnimationUtility.GetKeyLeftTangentMode(this.GetKeyframeFromSelection(selection)))
            {
                case AnimationUtility.TangentMode.Free:
                    return true;

                case AnimationUtility.TangentMode.ClampedAuto:
                case AnimationUtility.TangentMode.Auto:
                    return true;
            }
            return false;
        }

        private bool IsRegion(CurveWrapper cw1, CurveWrapper cw2) => 
            ((((cw1 != null) && (cw2 != null)) && (cw1.regionId >= 0)) && (cw1.regionId == cw2.regionId));

        private bool IsRegionCurveSelected(CurveWrapper cw1, CurveWrapper cw2) => 
            (this.IsCurveSelected(cw1) || this.IsCurveSelected(cw2));

        private bool IsRightTangentEditable(CurveSelection selection)
        {
            switch (AnimationUtility.GetKeyRightTangentMode(this.GetKeyframeFromSelection(selection)))
            {
                case AnimationUtility.TangentMode.Free:
                    return true;

                case AnimationUtility.TangentMode.ClampedAuto:
                case AnimationUtility.TangentMode.Auto:
                    return true;
            }
            return false;
        }

        private void MakeCurveBackups()
        {
            this.SaveKeySelection("Edit Curve");
            this.m_CurveBackups = new List<SavedCurve>();
            int num = -1;
            SavedCurve item = null;
            for (int i = 0; i < this.selectedCurves.Count; i++)
            {
                CurveSelection curveSelection = this.selectedCurves[i];
                if (curveSelection.curveID != num)
                {
                    AnimationCurve curveFromSelection = this.GetCurveFromSelection(curveSelection);
                    if (curveFromSelection != null)
                    {
                        item = new SavedCurve();
                        num = item.curveId = curveSelection.curveID;
                        Keyframe[] keys = curveFromSelection.keys;
                        item.keys = new List<SavedCurve.SavedKeyFrame>(keys.Length);
                        foreach (Keyframe keyframe in keys)
                        {
                            item.keys.Add(new SavedCurve.SavedKeyFrame(keyframe, CurveWrapper.SelectionMode.None));
                        }
                        this.m_CurveBackups.Add(item);
                    }
                }
                item.keys[curveSelection.key].selected = !curveSelection.semiSelected ? CurveWrapper.SelectionMode.Selected : CurveWrapper.SelectionMode.SemiSelected;
            }
        }

        private void MoveCurveToFront(int curveID)
        {
            int count = this.m_DrawOrder.Count;
            for (int i = 0; i < count; i++)
            {
                if (this.m_DrawOrder[i] == curveID)
                {
                    int regionId = this.GetCurveWrapperFromID(curveID).regionId;
                    if (regionId >= 0)
                    {
                        int num4 = 0;
                        int item = -1;
                        if ((i - 1) >= 0)
                        {
                            int num6 = this.m_DrawOrder[i - 1];
                            if (regionId == this.GetCurveWrapperFromID(num6).regionId)
                            {
                                item = num6;
                                num4 = -1;
                            }
                        }
                        if (((i + 1) < count) && (item < 0))
                        {
                            int num7 = this.m_DrawOrder[i + 1];
                            if (regionId == this.GetCurveWrapperFromID(num7).regionId)
                            {
                                item = num7;
                                num4 = 0;
                            }
                        }
                        if (item >= 0)
                        {
                            this.m_DrawOrder.RemoveRange(i + num4, 2);
                            this.m_DrawOrder.Add(item);
                            this.m_DrawOrder.Add(curveID);
                            this.ValidateCurveList();
                            break;
                        }
                        UnityEngine.Debug.LogError("Unhandled region");
                    }
                    else
                    {
                        if (i != (count - 1))
                        {
                            this.m_DrawOrder.RemoveAt(i);
                            this.m_DrawOrder.Add(curveID);
                            this.ValidateCurveList();
                        }
                        break;
                    }
                }
            }
        }

        public Vector2 MovePoints()
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            if (!this.hasSelection && !this.settings.allowDraggingCurvesAndRegions)
            {
                return Vector2.zero;
            }
            Event current = Event.current;
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (current.button != 0)
                    {
                        goto Label_051F;
                    }
                    foreach (CurveSelection selection in this.selectedCurves)
                    {
                        CurveWrapper curveWrapperFromSelection = this.GetCurveWrapperFromSelection(selection);
                        if ((curveWrapperFromSelection != null) && !curveWrapperFromSelection.hidden)
                        {
                            Vector2 vector2 = this.DrawingToOffsetViewTransformPoint(curveWrapperFromSelection, this.GetPosition(selection)) - current.mousePosition;
                            if (vector2.sqrMagnitude <= 100f)
                            {
                                Keyframe keyframeFromSelection = this.GetKeyframeFromSelection(selection);
                                this.SetupKeyOrCurveDragging(new Vector2(keyframeFromSelection.time, keyframeFromSelection.value), curveWrapperFromSelection, controlID, current.mousePosition);
                                current.Use();
                                break;
                            }
                        }
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        if (this.InLiveEdit())
                        {
                            this.EndLiveEdit();
                        }
                        this.ResetDragging();
                        GUI.changed = true;
                        current.Use();
                    }
                    goto Label_051F;

                case EventType.MouseDrag:
                {
                    if (GUIUtility.hotControl != controlID)
                    {
                        goto Label_051F;
                    }
                    Vector2 lhs = current.mousePosition - this.s_StartMouseDragPosition;
                    Vector3 zero = Vector3.zero;
                    if (current.shift && (this.m_AxisLock == AxisLock.None))
                    {
                        float introduced22 = Mathf.Abs(lhs.x);
                        this.m_AxisLock = (introduced22 <= Mathf.Abs(lhs.y)) ? AxisLock.Y : AxisLock.X;
                    }
                    if (this.m_DraggingCurveOrRegion != null)
                    {
                        lhs.x = 0f;
                        zero = (Vector3) base.ViewToDrawingTransformVector(lhs);
                        float introduced23 = this.SnapValue(zero.y + this.s_StartKeyDragPosition.y);
                        zero.y = introduced23 - this.s_StartKeyDragPosition.y;
                    }
                    else
                    {
                        switch (this.m_AxisLock)
                        {
                            case AxisLock.None:
                            {
                                zero = (Vector3) base.ViewToDrawingTransformVector(lhs);
                                float introduced24 = this.SnapTime(zero.x + this.s_StartKeyDragPosition.x);
                                zero.x = introduced24 - this.s_StartKeyDragPosition.x;
                                float introduced25 = this.SnapValue(zero.y + this.s_StartKeyDragPosition.y);
                                zero.y = introduced25 - this.s_StartKeyDragPosition.y;
                                break;
                            }
                            case AxisLock.X:
                            {
                                lhs.y = 0f;
                                zero = (Vector3) base.ViewToDrawingTransformVector(lhs);
                                float introduced26 = this.SnapTime(zero.x + this.s_StartKeyDragPosition.x);
                                zero.x = introduced26 - this.s_StartKeyDragPosition.x;
                                break;
                            }
                            case AxisLock.Y:
                            {
                                lhs.x = 0f;
                                zero = (Vector3) base.ViewToDrawingTransformVector(lhs);
                                float introduced27 = this.SnapValue(zero.y + this.s_StartKeyDragPosition.y);
                                zero.y = introduced27 - this.s_StartKeyDragPosition.y;
                                break;
                            }
                        }
                    }
                    if (!this.InLiveEdit())
                    {
                        this.StartLiveEdit();
                    }
                    this.TranslateSelectedKeys(zero);
                    GUI.changed = true;
                    current.Use();
                    return zero;
                }
                case EventType.KeyDown:
                    if ((GUIUtility.hotControl == controlID) && (current.keyCode == KeyCode.Escape))
                    {
                        this.TranslateSelectedKeys(Vector2.zero);
                        this.ResetDragging();
                        GUI.changed = true;
                        current.Use();
                    }
                    goto Label_051F;

                case EventType.Repaint:
                {
                    Rect position = new Rect(current.mousePosition.x - 10f, current.mousePosition.y - 10f, 20f, 20f);
                    if (this.m_DraggingCurveOrRegion == null)
                    {
                        if (this.m_DraggingKey != null)
                        {
                            EditorGUIUtility.AddCursorRect(position, MouseCursor.MoveArrow);
                        }
                    }
                    else
                    {
                        EditorGUIUtility.AddCursorRect(position, MouseCursor.ResizeVertical);
                    }
                    goto Label_051F;
                }
                default:
                    goto Label_051F;
            }
            if (this.settings.allowDraggingCurvesAndRegions && (this.m_DraggingKey == null))
            {
                Vector2 timeValue = Vector2.zero;
                CurveWrapper[] curves = null;
                if (this.HandleCurveAndRegionMoveToFrontOnMouseDown(ref timeValue, ref curves))
                {
                    List<CurveSelection> list = new List<CurveSelection>();
                    foreach (CurveWrapper wrapper2 in curves)
                    {
                        for (int i = 0; i < wrapper2.curve.keys.Length; i++)
                        {
                            list.Add(new CurveSelection(wrapper2.id, i));
                        }
                        this.MoveCurveToFront(wrapper2.id);
                    }
                    this.preCurveDragSelection = this.selectedCurves;
                    this.selectedCurves = list;
                    this.SetupKeyOrCurveDragging(timeValue, curves[0], controlID, current.mousePosition);
                    this.m_DraggingCurveOrRegion = curves;
                    current.Use();
                }
            }
        Label_051F:
            return Vector2.zero;
        }

        private Vector2 OffsetMousePositionInDrawing(CurveWrapper cw) => 
            this.OffsetViewToDrawingTransformPoint(cw, Event.current.mousePosition);

        private Vector2 OffsetViewToDrawingTransformPoint(CurveWrapper cw, Vector2 lhs) => 
            new Vector2(((lhs.x - this.m_Translation.x) - (cw.timeOffset * this.m_Scale.x)) / this.m_Scale.x, (lhs.y - this.m_Translation.y) / this.m_Scale.y);

        private Vector3 OffsetViewToDrawingTransformPoint(CurveWrapper cw, Vector3 lhs) => 
            new Vector3(((lhs.x - this.m_Translation.x) - (cw.timeOffset * this.m_Scale.x)) / this.m_Scale.x, (lhs.y - this.m_Translation.y) / this.m_Scale.y, 0f);

        public void OnDestroy()
        {
            if (this.m_Selection != null)
            {
                UnityEngine.Object.DestroyImmediate(this.m_Selection);
            }
        }

        public void OnDisable()
        {
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
            if (this.m_PointRenderer != null)
            {
                this.m_PointRenderer.FlushCache();
            }
        }

        public void OnEnable()
        {
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
        }

        public void OnGUI()
        {
            base.BeginViewGUI();
            this.GridGUI();
            this.DrawWrapperPopups();
            this.CurveGUI();
            base.EndViewGUI();
        }

        private int OnlyOneEditableCurve()
        {
            int num = -1;
            int num2 = 0;
            for (int i = 0; i < this.m_AnimationCurves.Length; i++)
            {
                CurveWrapper wrapper = this.m_AnimationCurves[i];
                if (!wrapper.hidden && !wrapper.readOnly)
                {
                    num2++;
                    num = i;
                }
            }
            if (num2 == 1)
            {
                return num;
            }
            return -1;
        }

        private float PointFieldForSelection(Rect rect, int customID, Func<CurveSelection, float> memberGetter, Func<Rect, int, float, float> memberSetter, string label)
        {
            <PointFieldForSelection>c__AnonStoreyA ya = new <PointFieldForSelection>c__AnonStoreyA {
                memberGetter = memberGetter,
                $this = this
            };
            float num = 0f;
            if (Enumerable.All<CurveSelection>(this.selectedCurves, new Func<CurveSelection, bool>(ya.<>m__0)))
            {
                num = ya.memberGetter(this.selectedCurves[0]);
            }
            else
            {
                EditorGUI.showMixedValue = true;
            }
            Rect position = rect;
            position.x -= position.width;
            GUIStyle style = GUI.skin.label;
            style.alignment = TextAnchor.UpperRight;
            int num2 = GUIUtility.GetControlID(customID, FocusType.Keyboard, rect);
            Color color = GUI.color;
            GUI.color = Color.white;
            GUI.Label(position, label, style);
            float num3 = memberSetter(rect, num2, num);
            GUI.color = color;
            EditorGUI.showMixedValue = false;
            return num3;
        }

        private void RecalcCurveSelection()
        {
            foreach (CurveWrapper wrapper in this.m_AnimationCurves)
            {
                wrapper.selected = CurveWrapper.SelectionMode.None;
            }
            foreach (CurveSelection selection in this.selectedCurves)
            {
                CurveWrapper curveWrapperFromSelection = this.GetCurveWrapperFromSelection(selection);
                if (curveWrapperFromSelection != null)
                {
                    curveWrapperFromSelection.selected = !selection.semiSelected ? CurveWrapper.SelectionMode.Selected : CurveWrapper.SelectionMode.SemiSelected;
                }
            }
        }

        private void RecalcSecondarySelection()
        {
            List<CurveSelection> list = new List<CurveSelection>(this.selectedCurves.Count);
            foreach (CurveSelection selection in this.selectedCurves)
            {
                CurveWrapper curveWrapperFromSelection = this.GetCurveWrapperFromSelection(selection);
                if (curveWrapperFromSelection != null)
                {
                    int groupId = curveWrapperFromSelection.groupId;
                    if ((groupId != -1) && !selection.semiSelected)
                    {
                        list.Add(selection);
                        foreach (CurveWrapper wrapper2 in this.m_AnimationCurves)
                        {
                            if ((wrapper2.groupId == groupId) && (wrapper2 != curveWrapperFromSelection))
                            {
                                CurveSelection item = new CurveSelection(wrapper2.id, selection.key) {
                                    semiSelected = true
                                };
                                list.Add(item);
                            }
                        }
                    }
                    else
                    {
                        list.Add(selection);
                    }
                }
            }
            list.Sort();
            int num3 = 0;
            while (num3 < (list.Count - 1))
            {
                CurveSelection selection3 = list[num3];
                CurveSelection selection4 = list[num3 + 1];
                if ((selection3.curveID == selection4.curveID) && (selection3.key == selection4.key))
                {
                    if (!selection3.semiSelected || !selection4.semiSelected)
                    {
                        selection3.semiSelected = false;
                    }
                    list.RemoveAt(num3 + 1);
                }
                else
                {
                    num3++;
                }
            }
            this.selectedCurves = list;
        }

        private void RecalculateBounds()
        {
            if (this.m_BoundsAreDirty)
            {
                this.m_DrawingBounds = this.m_DefaultBounds;
                this.m_CurveBounds = this.m_DefaultBounds;
                if (this.animationCurves != null)
                {
                    bool flag = false;
                    for (int i = 0; i < this.animationCurves.Length; i++)
                    {
                        CurveWrapper wrapper = this.animationCurves[i];
                        if (!wrapper.hidden && (wrapper.curve.length != 0))
                        {
                            if (!flag)
                            {
                                this.m_CurveBounds = wrapper.renderer.GetBounds();
                                flag = true;
                            }
                            else
                            {
                                this.m_CurveBounds.Encapsulate(wrapper.renderer.GetBounds());
                            }
                        }
                    }
                }
                float x = (base.hRangeMin == float.NegativeInfinity) ? this.m_CurveBounds.min.x : base.hRangeMin;
                float y = (base.vRangeMin == float.NegativeInfinity) ? this.m_CurveBounds.min.y : base.vRangeMin;
                float num4 = (base.hRangeMax == float.PositiveInfinity) ? this.m_CurveBounds.max.x : base.hRangeMax;
                float num5 = (base.vRangeMax == float.PositiveInfinity) ? this.m_CurveBounds.max.y : base.vRangeMax;
                this.m_DrawingBounds.SetMinMax(new Vector3(x, y, this.m_CurveBounds.min.z), new Vector3(num4, num5, this.m_CurveBounds.max.z));
                float introduced17 = Mathf.Max(this.m_DrawingBounds.size.x, 0.1f);
                this.m_DrawingBounds.size = new Vector3(introduced17, Mathf.Max(this.m_DrawingBounds.size.y, 0.1f), 0f);
                float introduced18 = Mathf.Max(this.m_CurveBounds.size.x, 0.1f);
                this.m_CurveBounds.size = new Vector3(introduced18, Mathf.Max(this.m_CurveBounds.size.y, 0.1f), 0f);
                this.m_BoundsAreDirty = false;
            }
        }

        private void RecalculateSelectionBounds()
        {
            if (this.hasSelection)
            {
                CurveWrapper curveWrapperFromSelection = this.GetCurveWrapperFromSelection(this.selectedCurves[0]);
                float num = (curveWrapperFromSelection == null) ? 0f : curveWrapperFromSelection.timeOffset;
                Keyframe keyframeFromSelection = this.GetKeyframeFromSelection(this.selectedCurves[0]);
                this.m_SelectionBounds = new Bounds((Vector3) new Vector2(keyframeFromSelection.time + num, keyframeFromSelection.value), (Vector3) Vector2.zero);
                for (int i = 1; i < this.selectedCurves.Count; i++)
                {
                    Keyframe keyframe2 = this.GetKeyframeFromSelection(this.selectedCurves[i]);
                    this.m_SelectionBounds.Encapsulate((Vector3) new Vector2(keyframe2.time + num, keyframe2.value));
                }
            }
            else
            {
                this.m_SelectionBounds = new Bounds(Vector3.zero, Vector3.zero);
            }
        }

        internal void RemoveSelection(CurveSelection curveSelection)
        {
            this.selectedCurves.Remove(curveSelection);
        }

        private void ResetDragging()
        {
            if (this.m_DraggingCurveOrRegion != null)
            {
                this.selectedCurves = this.preCurveDragSelection;
                this.preCurveDragSelection = null;
            }
            GUIUtility.hotControl = 0;
            this.m_DraggingKey = null;
            this.m_DraggingCurveOrRegion = null;
            this.m_MoveCoord = Vector2.zero;
            this.m_AxisLock = AxisLock.None;
        }

        public void SaveKeySelection(string undoLabel)
        {
            if (this.settings.undoRedoSelection)
            {
                Undo.RegisterCompleteObjectUndo(this.selection, undoLabel);
            }
        }

        public void SelectAll()
        {
            int capacity = 0;
            foreach (CurveWrapper wrapper in this.m_AnimationCurves)
            {
                if (!wrapper.hidden)
                {
                    capacity += wrapper.curve.length;
                }
            }
            List<CurveSelection> list = new List<CurveSelection>(capacity);
            foreach (CurveWrapper wrapper2 in this.m_AnimationCurves)
            {
                wrapper2.selected = CurveWrapper.SelectionMode.Selected;
                for (int i = 0; i < wrapper2.curve.length; i++)
                {
                    list.Add(new CurveSelection(wrapper2.id, i));
                }
            }
            this.selectedCurves = list;
        }

        public void SelectNone()
        {
            this.ClearSelection();
            foreach (CurveWrapper wrapper in this.m_AnimationCurves)
            {
                wrapper.selected = CurveWrapper.SelectionMode.None;
            }
        }

        private void SelectPoints()
        {
            int controlID = GUIUtility.GetControlID(0xdb218, FocusType.Passive);
            Event current = Event.current;
            bool shift = current.shift;
            bool actionKey = EditorGUI.actionKey;
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                {
                    if ((current.clickCount != 2) || (current.button != 0))
                    {
                        if (current.button == 0)
                        {
                            <SelectPoints>c__AnonStorey8 storey3 = new <SelectPoints>c__AnonStorey8 {
                                selectedPoint = this.FindNearest()
                            };
                            if ((storey3.selectedPoint == null) || storey3.selectedPoint.semiSelected)
                            {
                                Vector2 zero = Vector2.zero;
                                CurveWrapper[] curves = null;
                                bool flag3 = this.HandleCurveAndRegionMoveToFrontOnMouseDown(ref zero, ref curves);
                                if ((!shift && !actionKey) && !flag3)
                                {
                                    this.SelectNone();
                                }
                                GUIUtility.hotControl = controlID;
                                this.s_EndMouseDragPosition = this.s_StartMouseDragPosition = current.mousePosition;
                                this.s_PickMode = PickMode.Click;
                            }
                            else
                            {
                                this.MoveCurveToFront(storey3.selectedPoint.curveID);
                                if (this.syncTimeDuringDrag)
                                {
                                    CurveWrapper curveWrapperFromSelection = this.GetCurveWrapperFromSelection(storey3.selectedPoint);
                                    if (curveWrapperFromSelection != null)
                                    {
                                        this.activeTime = this.GetKeyframeFromSelection(storey3.selectedPoint).time + curveWrapperFromSelection.timeOffset;
                                    }
                                }
                                Keyframe keyframeFromSelection = this.GetKeyframeFromSelection(storey3.selectedPoint);
                                this.s_StartKeyDragPosition = new Vector2(keyframeFromSelection.time, keyframeFromSelection.value);
                                if (shift)
                                {
                                    bool flag4 = false;
                                    int key = storey3.selectedPoint.key;
                                    int a = storey3.selectedPoint.key;
                                    for (int i = 0; i < this.selectedCurves.Count; i++)
                                    {
                                        CurveSelection selection2 = this.selectedCurves[i];
                                        if (selection2.curveID == storey3.selectedPoint.curveID)
                                        {
                                            flag4 = true;
                                            key = Mathf.Min(key, selection2.key);
                                            a = Mathf.Max(a, selection2.key);
                                        }
                                    }
                                    if (!flag4)
                                    {
                                        if (!this.selectedCurves.Contains(storey3.selectedPoint))
                                        {
                                            this.AddSelection(storey3.selectedPoint);
                                        }
                                    }
                                    else
                                    {
                                        <SelectPoints>c__AnonStorey9 storey4 = new <SelectPoints>c__AnonStorey9 {
                                            <>f__ref$8 = storey3,
                                            keyIndex = key
                                        };
                                        while (storey4.keyIndex <= a)
                                        {
                                            if (!Enumerable.Any<CurveSelection>(this.selectedCurves, new Func<CurveSelection, bool>(storey4.<>m__0)))
                                            {
                                                CurveSelection curveSelection = new CurveSelection(storey3.selectedPoint.curveID, storey4.keyIndex);
                                                this.AddSelection(curveSelection);
                                            }
                                            storey4.keyIndex++;
                                        }
                                    }
                                    Event.current.Use();
                                }
                                else if (actionKey)
                                {
                                    if (!this.selectedCurves.Contains(storey3.selectedPoint))
                                    {
                                        this.AddSelection(storey3.selectedPoint);
                                    }
                                    else
                                    {
                                        this.RemoveSelection(storey3.selectedPoint);
                                    }
                                    Event.current.Use();
                                }
                                else if (!this.selectedCurves.Contains(storey3.selectedPoint))
                                {
                                    this.ClearSelection();
                                    this.AddSelection(storey3.selectedPoint);
                                }
                            }
                            GUI.changed = true;
                            HandleUtility.Repaint();
                        }
                        break;
                    }
                    <SelectPoints>c__AnonStorey6 storey = new <SelectPoints>c__AnonStorey6 {
                        selectedPoint = this.FindNearest()
                    };
                    if (storey.selectedPoint == null)
                    {
                        this.SaveKeySelection("Add Key");
                        List<int> list = this.CreateKeyFromClick(Event.current.mousePosition);
                        if (list.Count > 0)
                        {
                            foreach (int num4 in list)
                            {
                                this.GetCurveFromID(num4).changed = true;
                            }
                            GUI.changed = true;
                        }
                    }
                    else
                    {
                        if (!shift)
                        {
                            this.ClearSelection();
                        }
                        AnimationCurve curveFromSelection = this.GetCurveFromSelection(storey.selectedPoint);
                        if (curveFromSelection != null)
                        {
                            <SelectPoints>c__AnonStorey7 storey2 = new <SelectPoints>c__AnonStorey7 {
                                <>f__ref$6 = storey,
                                keyIndex = 0
                            };
                            while (storey2.keyIndex < curveFromSelection.keys.Length)
                            {
                                if (!Enumerable.Any<CurveSelection>(this.selectedCurves, new Func<CurveSelection, bool>(storey2.<>m__0)))
                                {
                                    CurveSelection selection = new CurveSelection(storey.selectedPoint.curveID, storey2.keyIndex);
                                    this.AddSelection(selection);
                                }
                                storey2.keyIndex++;
                            }
                        }
                    }
                    current.Use();
                    break;
                }
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                        this.s_PickMode = PickMode.None;
                        Event.current.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        this.s_EndMouseDragPosition = current.mousePosition;
                        if (this.s_PickMode != PickMode.Click)
                        {
                            Rect rect2 = EditorGUIExt.FromToRect(this.s_StartMouseDragPosition, current.mousePosition);
                            List<CurveSelection> list2 = new List<CurveSelection>(this.s_SelectionBackup);
                            foreach (CurveWrapper wrapper3 in this.m_AnimationCurves)
                            {
                                if (!wrapper3.readOnly && !wrapper3.hidden)
                                {
                                    int num9 = 0;
                                    foreach (Keyframe keyframe3 in wrapper3.curve.keys)
                                    {
                                        if (rect2.Contains(this.GetGUIPoint(wrapper3, (Vector3) new Vector2(keyframe3.time, keyframe3.value))))
                                        {
                                            list2.Add(new CurveSelection(wrapper3.id, num9));
                                            this.MoveCurveToFront(wrapper3.id);
                                        }
                                        num9++;
                                    }
                                }
                            }
                            this.selectedCurves = list2;
                            GUI.changed = true;
                        }
                        else
                        {
                            this.s_PickMode = PickMode.Marquee;
                            if (!shift && !actionKey)
                            {
                                this.s_SelectionBackup = new List<CurveSelection>();
                            }
                            else
                            {
                                this.s_SelectionBackup = new List<CurveSelection>(this.selectedCurves);
                            }
                        }
                        current.Use();
                    }
                    break;

                case EventType.Layout:
                    HandleUtility.AddDefaultControl(controlID);
                    break;

                case EventType.ContextClick:
                {
                    Rect drawRect = base.drawRect;
                    float num2 = 0f;
                    drawRect.y = num2;
                    drawRect.x = num2;
                    if (drawRect.Contains(Event.current.mousePosition))
                    {
                        Vector2 vector;
                        int curveAtPosition = this.GetCurveAtPosition(Event.current.mousePosition, out vector);
                        if (curveAtPosition >= 0)
                        {
                            GenericMenu menu = new GenericMenu();
                            if (this.m_AnimationCurves[curveAtPosition].animationIsEditable)
                            {
                                menu.AddItem(new GUIContent("Add Key"), false, new GenericMenu.MenuFunction2(this.CreateKeyFromClick), Event.current.mousePosition);
                            }
                            else
                            {
                                menu.AddDisabledItem(new GUIContent("Add Key"));
                            }
                            menu.ShowAsContext();
                            Event.current.Use();
                        }
                    }
                    break;
                }
            }
            if (this.s_PickMode == PickMode.Marquee)
            {
                GUI.Label(EditorGUIExt.FromToRect(this.s_StartMouseDragPosition, this.s_EndMouseDragPosition), GUIContent.none, this.styles.selectionRect);
            }
        }

        private void SetAxisUiScalars(Vector2 newScalars, List<CurveWrapper> curvesInSameSpace)
        {
            foreach (CurveWrapper wrapper in curvesInSameSpace)
            {
                Vector2 newAxisScalars = wrapper.getAxisUiScalarsCallback();
                if (newScalars.x >= 0f)
                {
                    newAxisScalars.x = newScalars.x;
                }
                if (newScalars.y >= 0f)
                {
                    newAxisScalars.y = newScalars.y;
                }
                if (wrapper.setAxisUiScalarsCallback != null)
                {
                    wrapper.setAxisUiScalarsCallback(newAxisScalars);
                }
            }
        }

        internal void SetSelectedKeyPositions(Vector2 newPosition, bool updateTime, bool updateValue)
        {
            <SetSelectedKeyPositions>c__AnonStorey1 storey = new <SetSelectedKeyPositions>c__AnonStorey1 {
                updateTime = updateTime,
                newPosition = newPosition,
                updateValue = updateValue,
                $this = this
            };
            bool flag = this.InLiveEdit();
            if (!flag)
            {
                this.StartLiveEdit();
            }
            this.UpdateCurvesFromPoints(new SavedCurve.KeyFrameOperation(storey.<>m__0));
            if (!flag)
            {
                this.EndLiveEdit();
            }
        }

        private void SetupKeyOrCurveDragging(Vector2 timeValue, CurveWrapper cw, int id, Vector2 mousePos)
        {
            this.m_DraggedCoord = timeValue;
            this.m_DraggingKey = cw;
            GUIUtility.hotControl = id;
            if (this.syncTimeDuringDrag)
            {
                this.activeTime = timeValue.x + cw.timeOffset;
            }
            this.s_StartMouseDragPosition = mousePos;
            this.s_StartClickedTime = timeValue.x;
        }

        private bool ShouldCurveHaveFocus(int indexIntoDrawOrder, CurveWrapper cw1, CurveWrapper cw2)
        {
            bool flag = false;
            if (indexIntoDrawOrder == (this.m_DrawOrder.Count - 1))
            {
                return true;
            }
            if (this.hasSelection)
            {
                flag = this.IsCurveSelected(cw1) || this.IsCurveSelected(cw2);
            }
            return flag;
        }

        private float SnapTime(float t)
        {
            if (EditorGUI.actionKey)
            {
                int levelWithMinSeparation = base.hTicks.GetLevelWithMinSeparation(5f);
                float periodOfLevel = base.hTicks.GetPeriodOfLevel(levelWithMinSeparation);
                t = Mathf.Round(t / periodOfLevel) * periodOfLevel;
                return t;
            }
            if (this.invSnap != 0f)
            {
                t = Mathf.Round(t * this.invSnap) / this.invSnap;
            }
            return t;
        }

        private float SnapValue(float v)
        {
            if (EditorGUI.actionKey)
            {
                int levelWithMinSeparation = base.vTicks.GetLevelWithMinSeparation(5f);
                float periodOfLevel = base.vTicks.GetPeriodOfLevel(levelWithMinSeparation);
                v = Mathf.Round(v / periodOfLevel) * periodOfLevel;
            }
            return v;
        }

        private void StartEditingSelectedPoints()
        {
            float num = Enumerable.Min<CurveSelection>(this.selectedCurves, (Func<CurveSelection, float>) (x => this.GetKeyframeFromSelection(x).time));
            float num2 = Enumerable.Max<CurveSelection>(this.selectedCurves, (Func<CurveSelection, float>) (x => this.GetKeyframeFromSelection(x).time));
            float num3 = Enumerable.Min<CurveSelection>(this.selectedCurves, (Func<CurveSelection, float>) (x => this.GetKeyframeFromSelection(x).value));
            float num4 = Enumerable.Max<CurveSelection>(this.selectedCurves, (Func<CurveSelection, float>) (x => this.GetKeyframeFromSelection(x).value));
            Vector2 fieldPosition = (Vector2) (new Vector2(num + num2, num3 + num4) * 0.5f);
            this.StartEditingSelectedPoints(fieldPosition);
        }

        private void StartEditingSelectedPoints(Vector2 fieldPosition)
        {
            this.pointEditingFieldPosition = fieldPosition;
            this.focusedPointField = "pointValueField";
            this.timeWasEdited = this.valueWasEdited = false;
            this.editingPoints = true;
            this.StartLiveEdit();
        }

        private void StartEditingSelectedPointsContext(object fieldPosition)
        {
            this.StartEditingSelectedPoints((Vector2) fieldPosition);
        }

        public void StartLiveEdit()
        {
            this.MakeCurveBackups();
        }

        private void SyncDrawOrder()
        {
            if (this.m_DrawOrder.Count == 0)
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = cw => cw.id;
                }
                this.m_DrawOrder = Enumerable.Select<CurveWrapper, int>(this.m_AnimationCurves, <>f__am$cache0).ToList<int>();
            }
            else
            {
                List<int> list = new List<int>(this.m_AnimationCurves.Length);
                for (int i = 0; i < this.m_DrawOrder.Count; i++)
                {
                    int item = this.m_DrawOrder[i];
                    for (int j = 0; j < this.m_AnimationCurves.Length; j++)
                    {
                        if (this.m_AnimationCurves[j].id == item)
                        {
                            list.Add(item);
                            break;
                        }
                    }
                }
                this.m_DrawOrder = list;
                if (this.m_DrawOrder.Count != this.m_AnimationCurves.Length)
                {
                    for (int k = 0; k < this.m_AnimationCurves.Length; k++)
                    {
                        int id = this.m_AnimationCurves[k].id;
                        bool flag = false;
                        for (int m = 0; m < this.m_DrawOrder.Count; m++)
                        {
                            if (this.m_DrawOrder[m] == id)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            this.m_DrawOrder.Add(id);
                        }
                    }
                    if (this.m_DrawOrder.Count != this.m_AnimationCurves.Length)
                    {
                        if (<>f__am$cache1 == null)
                        {
                            <>f__am$cache1 = cw => cw.id;
                        }
                        this.m_DrawOrder = Enumerable.Select<CurveWrapper, int>(this.m_AnimationCurves, <>f__am$cache1).ToList<int>();
                    }
                }
            }
        }

        private void SyncSelection()
        {
            this.Init();
            List<CurveSelection> list = new List<CurveSelection>(this.selectedCurves.Count);
            foreach (CurveSelection selection in this.selectedCurves)
            {
                CurveWrapper curveWrapperFromSelection = this.GetCurveWrapperFromSelection(selection);
                if ((curveWrapperFromSelection != null) && (!curveWrapperFromSelection.hidden || (curveWrapperFromSelection.groupId != -1)))
                {
                    curveWrapperFromSelection.selected = CurveWrapper.SelectionMode.Selected;
                    list.Add(selection);
                }
            }
            this.selectedCurves = list;
            this.InvalidateBounds();
        }

        private Matrix4x4 TimeOffsetMatrix(CurveWrapper cw) => 
            Matrix4x4.TRS(new Vector3(cw.timeOffset * this.m_Scale.x, 0f, 0f), Quaternion.identity, Vector3.one);

        public void TimeRangeSelectTo(float time)
        {
            if (!this.s_TimeRangeSelectionActive)
            {
                UnityEngine.Debug.LogError("TimeRangeSelectTo can only be called after BeginTimeRangeSelection");
            }
            else
            {
                this.s_TimeRangeSelectionEnd = time;
                List<CurveSelection> list = new List<CurveSelection>(this.s_SelectionBackup);
                float num = Mathf.Min(this.s_TimeRangeSelectionStart, this.s_TimeRangeSelectionEnd);
                float num2 = Mathf.Max(this.s_TimeRangeSelectionStart, this.s_TimeRangeSelectionEnd);
                foreach (CurveWrapper wrapper in this.m_AnimationCurves)
                {
                    if (!wrapper.readOnly && !wrapper.hidden)
                    {
                        int key = 0;
                        foreach (Keyframe keyframe in wrapper.curve.keys)
                        {
                            if ((keyframe.time >= num) && (keyframe.time < num2))
                            {
                                list.Add(new CurveSelection(wrapper.id, key));
                            }
                            key++;
                        }
                    }
                }
                this.selectedCurves = list;
                this.RecalcSecondarySelection();
                this.RecalcCurveSelection();
            }
        }

        internal void TransformRippleKeys(Matrix4x4 matrix, float t1, float t2, bool flipX)
        {
            <TransformRippleKeys>c__AnonStorey3 storey = new <TransformRippleKeys>c__AnonStorey3 {
                matrix = matrix,
                flipX = flipX,
                t2 = t2,
                t1 = t1,
                $this = this
            };
            bool flag = this.InLiveEdit();
            if (!flag)
            {
                this.StartLiveEdit();
            }
            this.UpdateCurvesFromPoints(new SavedCurve.KeyFrameOperation(storey.<>m__0));
            if (!flag)
            {
                this.EndLiveEdit();
            }
        }

        internal void TransformSelectedKeys(Matrix4x4 matrix, bool flipX, bool flipY)
        {
            <TransformSelectedKeys>c__AnonStorey2 storey = new <TransformSelectedKeys>c__AnonStorey2 {
                matrix = matrix,
                flipX = flipX,
                flipY = flipY,
                $this = this
            };
            bool flag = this.InLiveEdit();
            if (!flag)
            {
                this.StartLiveEdit();
            }
            this.UpdateCurvesFromPoints(new SavedCurve.KeyFrameOperation(storey.<>m__0));
            if (!flag)
            {
                this.EndLiveEdit();
            }
        }

        internal void TranslateSelectedKeys(Vector2 movement)
        {
            <TranslateSelectedKeys>c__AnonStorey0 storey = new <TranslateSelectedKeys>c__AnonStorey0 {
                movement = movement,
                $this = this
            };
            bool flag = this.InLiveEdit();
            if (!flag)
            {
                this.StartLiveEdit();
            }
            this.UpdateCurvesFromPoints(new SavedCurve.KeyFrameOperation(storey.<>m__0));
            if (!flag)
            {
                this.EndLiveEdit();
            }
        }

        private void UndoRedoPerformed()
        {
            if (!this.settings.undoRedoSelection)
            {
                this.SelectNone();
            }
        }

        public void UpdateCurves(List<int> curveIds, string undoText)
        {
            foreach (int num in curveIds)
            {
                this.GetCurveWrapperFromID(num).changed = true;
            }
            if (this.curvesUpdated != null)
            {
                this.curvesUpdated();
            }
        }

        public void UpdateCurves(List<ChangedCurve> changedCurves, string undoText)
        {
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = curve => curve.curveId;
            }
            this.UpdateCurves(new List<int>(Enumerable.Select<ChangedCurve, int>(changedCurves, <>f__am$cache2)), undoText);
        }

        private void UpdateCurvesFromPoints(SavedCurve.KeyFrameOperation action)
        {
            if (this.m_CurveBackups != null)
            {
                List<CurveSelection> list = new List<CurveSelection>();
                foreach (SavedCurve curve in this.m_CurveBackups)
                {
                    CurveWrapper curveWrapperFromID = this.GetCurveWrapperFromID(curve.curveId);
                    if (curveWrapperFromID.animationIsEditable)
                    {
                        List<SavedCurve.SavedKeyFrame> list2 = new List<SavedCurve.SavedKeyFrame>(curve.keys.Count);
                        foreach (SavedCurve.SavedKeyFrame frame in curve.keys)
                        {
                            if (frame.selected == CurveWrapper.SelectionMode.None)
                            {
                                <UpdateCurvesFromPoints>c__AnonStorey4 storey = new <UpdateCurvesFromPoints>c__AnonStorey4 {
                                    newKeyframe = action(frame, curve)
                                };
                                list2.RemoveAll(new Predicate<SavedCurve.SavedKeyFrame>(storey.<>m__0));
                                list2.Add(storey.newKeyframe);
                            }
                        }
                        foreach (SavedCurve.SavedKeyFrame frame2 in curve.keys)
                        {
                            if (frame2.selected != CurveWrapper.SelectionMode.None)
                            {
                                <UpdateCurvesFromPoints>c__AnonStorey5 storey2 = new <UpdateCurvesFromPoints>c__AnonStorey5 {
                                    newKeyframe = action(frame2, curve)
                                };
                                list2.RemoveAll(new Predicate<SavedCurve.SavedKeyFrame>(storey2.<>m__0));
                                list2.Add(storey2.newKeyframe);
                            }
                        }
                        list2.Sort();
                        Keyframe[] keyframeArray = new Keyframe[list2.Count];
                        for (int i = 0; i < list2.Count; i++)
                        {
                            SavedCurve.SavedKeyFrame frame3 = list2[i];
                            keyframeArray[i] = frame3.key;
                            if (frame3.selected != CurveWrapper.SelectionMode.None)
                            {
                                CurveSelection item = new CurveSelection(curve.curveId, i);
                                if (frame3.selected == CurveWrapper.SelectionMode.SemiSelected)
                                {
                                    item.semiSelected = true;
                                }
                                list.Add(item);
                            }
                        }
                        this.selectedCurves = list;
                        curveWrapperFromID.curve.keys = keyframeArray;
                        curveWrapperFromID.changed = true;
                    }
                }
                this.UpdateTangentsFromSelection();
            }
        }

        private void UpdateTangentsFromSelection()
        {
            foreach (CurveSelection selection in this.selectedCurves)
            {
                AnimationCurve curveFromSelection = this.GetCurveFromSelection(selection);
                if (curveFromSelection != null)
                {
                    AnimationUtility.UpdateTangentsFromModeSurrounding(curveFromSelection, selection.key);
                }
            }
        }

        private void ValidateCurveList()
        {
            for (int i = 0; i < this.m_AnimationCurves.Length; i++)
            {
                CurveWrapper wrapper = this.m_AnimationCurves[i];
                int regionId = wrapper.regionId;
                if (regionId >= 0)
                {
                    if (i == (this.m_AnimationCurves.Length - 1))
                    {
                        UnityEngine.Debug.LogError("Region has only one curve last! Regions should be added as two curves after each other with same regionId");
                        return;
                    }
                    CurveWrapper wrapper2 = this.m_AnimationCurves[++i];
                    int num3 = wrapper2.regionId;
                    if (regionId != num3)
                    {
                        UnityEngine.Debug.LogError(string.Concat(new object[] { "Regions should be added as two curves after each other with same regionId: ", regionId, " != ", num3 }));
                        return;
                    }
                }
            }
            if (this.m_DrawOrder.Count != this.m_AnimationCurves.Length)
            {
                UnityEngine.Debug.LogError(string.Concat(new object[] { "DrawOrder and AnimationCurves mismatch: DrawOrder ", this.m_DrawOrder.Count, ", AnimationCurves: ", this.m_AnimationCurves.Length }));
            }
            else
            {
                int count = this.m_DrawOrder.Count;
                for (int j = 0; j < count; j++)
                {
                    int curveID = this.m_DrawOrder[j];
                    int num7 = this.GetCurveWrapperFromID(curveID).regionId;
                    if (num7 >= 0)
                    {
                        if (j == (count - 1))
                        {
                            UnityEngine.Debug.LogError("Region has only one curve last! Regions should be added as two curves after each other with same regionId");
                            break;
                        }
                        int num8 = this.m_DrawOrder[++j];
                        int num9 = this.GetCurveWrapperFromID(num8).regionId;
                        if (num7 != num9)
                        {
                            UnityEngine.Debug.LogError(string.Concat(new object[] { "DrawOrder: Regions not added correctly after each other. RegionIds: ", num7, " , ", num9 }));
                            break;
                        }
                    }
                }
            }
        }

        private WrapMode WrapModeIconPopup(Keyframe key, WrapMode oldWrap, float hOffset)
        {
            float width = this.styles.wrapModeMenuIcon.image.width;
            Vector3 lhs = new Vector3(key.time, key.value);
            lhs = base.DrawingToViewTransformPoint(lhs);
            Rect position = new Rect(lhs.x + (width * hOffset), lhs.y + base.drawRect.y, width, width);
            WrapModeFixedCurve curve = (WrapModeFixedCurve) oldWrap;
            Enum[] array = Enum.GetValues(typeof(WrapModeFixedCurve)).Cast<Enum>().ToArray<Enum>();
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = x => ObjectNames.NicifyVariableName(x);
            }
            string[] texts = Enumerable.Select<string, string>(Enum.GetNames(typeof(WrapModeFixedCurve)), <>f__am$cache3).ToArray<string>();
            int index = Array.IndexOf<Enum>(array, curve);
            int controlID = GUIUtility.GetControlID("WrapModeIconPopup".GetHashCode(), FocusType.Keyboard, position);
            int selectedValueForControl = EditorGUI.PopupCallbackInfo.GetSelectedValueForControl(controlID, index);
            GUIContent[] options = EditorGUIUtility.TempContent(texts);
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.Repaint:
                    GUIStyle.none.Draw(position, this.styles.wrapModeMenuIcon, controlID, false);
                    break;

                case EventType.MouseDown:
                    if ((current.button == 0) && position.Contains(current.mousePosition))
                    {
                        if (Application.platform == RuntimePlatform.OSXEditor)
                        {
                            position.y = (position.y - (selectedValueForControl * 0x10)) - 19f;
                        }
                        EditorGUI.PopupCallbackInfo.instance = new EditorGUI.PopupCallbackInfo(controlID);
                        EditorUtility.DisplayCustomMenu(position, options, selectedValueForControl, new EditorUtility.SelectMenuItemFunction(EditorGUI.PopupCallbackInfo.instance.SetEnumValueDelegate), null);
                        GUIUtility.keyboardControl = controlID;
                        current.Use();
                    }
                    break;

                case EventType.KeyDown:
                    if (current.MainActionKeyForControl(controlID))
                    {
                        if (Application.platform == RuntimePlatform.OSXEditor)
                        {
                            position.y = (position.y - (selectedValueForControl * 0x10)) - 19f;
                        }
                        EditorGUI.PopupCallbackInfo.instance = new EditorGUI.PopupCallbackInfo(controlID);
                        EditorUtility.DisplayCustomMenu(position, options, selectedValueForControl, new EditorUtility.SelectMenuItemFunction(EditorGUI.PopupCallbackInfo.instance.SetEnumValueDelegate), null);
                        current.Use();
                    }
                    break;
            }
            return (WrapMode) array[selectedValueForControl];
        }

        public float activeTime
        {
            set
            {
                if (this.state != null)
                {
                    this.state.currentTime = value;
                }
            }
        }

        public CurveWrapper[] animationCurves
        {
            get
            {
                if (this.m_AnimationCurves == null)
                {
                    this.m_AnimationCurves = new CurveWrapper[0];
                }
                return this.m_AnimationCurves;
            }
            set
            {
                if (this.m_AnimationCurves == null)
                {
                    this.m_AnimationCurves = new CurveWrapper[0];
                }
                this.m_AnimationCurves = value;
                for (int i = 0; i < this.m_AnimationCurves.Length; i++)
                {
                    this.m_AnimationCurves[i].listIndex = i;
                }
                this.SyncDrawOrder();
                this.SyncSelection();
                this.ValidateCurveList();
            }
        }

        public Bounds curveBounds
        {
            get
            {
                this.RecalculateBounds();
                return this.m_CurveBounds;
            }
        }

        public override Bounds drawingBounds
        {
            get
            {
                this.RecalculateBounds();
                return this.m_DrawingBounds;
            }
        }

        private bool editingPoints { get; set; }

        public bool hasSelection =>
            (this.selectedCurves.Count != 0);

        internal List<CurveSelection> selectedCurves
        {
            get => 
                this.selection.selectedCurves;
            set
            {
                this.selection.selectedCurves = value;
            }
        }

        internal CurveEditorSelection selection
        {
            get
            {
                if (this.m_Selection == null)
                {
                    this.m_Selection = ScriptableObject.CreateInstance<CurveEditorSelection>();
                    this.m_Selection.hideFlags = HideFlags.HideAndDontSave;
                }
                return this.m_Selection;
            }
        }

        public Bounds selectionBounds
        {
            get
            {
                this.RecalculateSelectionBounds();
                return this.m_SelectionBounds;
            }
        }

        public CurveEditorSettings settings
        {
            get => 
                this.m_Settings;
            set
            {
                if (value != null)
                {
                    this.m_Settings = value;
                    this.ApplySettings();
                }
            }
        }

        internal Styles styles
        {
            get
            {
                if (this.ms_Styles == null)
                {
                    this.ms_Styles = new Styles();
                }
                return this.ms_Styles;
            }
        }

        public bool syncTimeDuringDrag
        {
            get
            {
                if (this.state != null)
                {
                    return this.state.syncTimeDuringDrag;
                }
                return true;
            }
        }

        public Color tangentColor
        {
            get => 
                this.m_TangentColor;
            set
            {
                this.m_TangentColor = value;
            }
        }

        public TimeArea.TimeFormat timeFormat
        {
            get
            {
                if (this.state != null)
                {
                    return this.state.timeFormat;
                }
                return TimeArea.TimeFormat.None;
            }
        }

        [CompilerGenerated]
        private sealed class <PointFieldForSelection>c__AnonStoreyA
        {
            internal CurveEditor $this;
            internal Func<CurveSelection, float> memberGetter;

            internal bool <>m__0(CurveSelection x) => 
                (this.memberGetter(x) == this.memberGetter(this.$this.selectedCurves[0]));
        }

        [CompilerGenerated]
        private sealed class <SelectPoints>c__AnonStorey6
        {
            internal CurveSelection selectedPoint;
        }

        [CompilerGenerated]
        private sealed class <SelectPoints>c__AnonStorey7
        {
            internal CurveEditor.<SelectPoints>c__AnonStorey6 <>f__ref$6;
            internal int keyIndex;

            internal bool <>m__0(CurveSelection x) => 
                ((x.curveID == this.<>f__ref$6.selectedPoint.curveID) && (x.key == this.keyIndex));
        }

        [CompilerGenerated]
        private sealed class <SelectPoints>c__AnonStorey8
        {
            internal CurveSelection selectedPoint;
        }

        [CompilerGenerated]
        private sealed class <SelectPoints>c__AnonStorey9
        {
            internal CurveEditor.<SelectPoints>c__AnonStorey8 <>f__ref$8;
            internal int keyIndex;

            internal bool <>m__0(CurveSelection x) => 
                ((x.curveID == this.<>f__ref$8.selectedPoint.curveID) && (x.key == this.keyIndex));
        }

        [CompilerGenerated]
        private sealed class <SetSelectedKeyPositions>c__AnonStorey1
        {
            internal CurveEditor $this;
            internal Vector2 newPosition;
            internal bool updateTime;
            internal bool updateValue;

            internal CurveEditor.SavedCurve.SavedKeyFrame <>m__0(CurveEditor.SavedCurve.SavedKeyFrame keyframe, CurveEditor.SavedCurve curve)
            {
                if (keyframe.selected != CurveWrapper.SelectionMode.None)
                {
                    CurveEditor.SavedCurve.SavedKeyFrame frame = keyframe.Clone();
                    if (this.updateTime)
                    {
                        frame.key.time = Mathf.Clamp(this.newPosition.x, this.$this.hRangeMin, this.$this.hRangeMax);
                    }
                    if (this.updateValue)
                    {
                        frame.key.value = this.$this.ClampVerticalValue(this.newPosition.y, curve.curveId);
                    }
                    return frame;
                }
                return keyframe;
            }
        }

        [CompilerGenerated]
        private sealed class <TransformRippleKeys>c__AnonStorey3
        {
            internal CurveEditor $this;
            internal bool flipX;
            internal Matrix4x4 matrix;
            internal float t1;
            internal float t2;

            internal CurveEditor.SavedCurve.SavedKeyFrame <>m__0(CurveEditor.SavedCurve.SavedKeyFrame keyframe, CurveEditor.SavedCurve curve)
            {
                float time = keyframe.key.time;
                if (keyframe.selected != CurveWrapper.SelectionMode.None)
                {
                    Vector3 v = new Vector3(keyframe.key.time, 0f, 0f);
                    time = this.matrix.MultiplyPoint3x4(v).x;
                    CurveEditor.SavedCurve.SavedKeyFrame frame = keyframe.Clone();
                    frame.key.time = this.$this.SnapTime(Mathf.Clamp(time, this.$this.hRangeMin, this.$this.hRangeMax));
                    if (this.flipX)
                    {
                        frame.key.inTangent = (keyframe.key.outTangent == float.PositiveInfinity) ? float.PositiveInfinity : -keyframe.key.outTangent;
                        frame.key.outTangent = (keyframe.key.inTangent == float.PositiveInfinity) ? float.PositiveInfinity : -keyframe.key.inTangent;
                    }
                    return frame;
                }
                if (keyframe.key.time > this.t2)
                {
                    Vector3 vector2 = new Vector3(!this.flipX ? this.t2 : this.t1, 0f, 0f);
                    float num2 = this.matrix.MultiplyPoint3x4(vector2).x - this.t2;
                    if (num2 > 0f)
                    {
                        time = keyframe.key.time + num2;
                    }
                }
                else if (keyframe.key.time < this.t1)
                {
                    Vector3 vector3 = new Vector3(!this.flipX ? this.t1 : this.t2, 0f, 0f);
                    float num3 = this.matrix.MultiplyPoint3x4(vector3).x - this.t1;
                    if (num3 < 0f)
                    {
                        time = keyframe.key.time + num3;
                    }
                }
                if (!Mathf.Approximately(time, keyframe.key.time))
                {
                    CurveEditor.SavedCurve.SavedKeyFrame frame3 = keyframe.Clone();
                    frame3.key.time = this.$this.SnapTime(Mathf.Clamp(time, this.$this.hRangeMin, this.$this.hRangeMax));
                    return frame3;
                }
                return keyframe;
            }
        }

        [CompilerGenerated]
        private sealed class <TransformSelectedKeys>c__AnonStorey2
        {
            internal CurveEditor $this;
            internal bool flipX;
            internal bool flipY;
            internal Matrix4x4 matrix;

            internal CurveEditor.SavedCurve.SavedKeyFrame <>m__0(CurveEditor.SavedCurve.SavedKeyFrame keyframe, CurveEditor.SavedCurve curve)
            {
                if (keyframe.selected == CurveWrapper.SelectionMode.None)
                {
                    return keyframe;
                }
                CurveEditor.SavedCurve.SavedKeyFrame frame = keyframe.Clone();
                Vector3 v = new Vector3(frame.key.time, frame.key.value, 0f);
                v = this.matrix.MultiplyPoint3x4(v);
                v.x = this.$this.SnapTime(v.x);
                frame.key.time = Mathf.Clamp(v.x, this.$this.hRangeMin, this.$this.hRangeMax);
                if (this.flipX)
                {
                    frame.key.inTangent = (keyframe.key.outTangent == float.PositiveInfinity) ? float.PositiveInfinity : -keyframe.key.outTangent;
                    frame.key.outTangent = (keyframe.key.inTangent == float.PositiveInfinity) ? float.PositiveInfinity : -keyframe.key.inTangent;
                }
                if (frame.selected == CurveWrapper.SelectionMode.Selected)
                {
                    frame.key.value = this.$this.ClampVerticalValue(v.y, curve.curveId);
                    if (this.flipY)
                    {
                        frame.key.inTangent = (frame.key.inTangent == float.PositiveInfinity) ? float.PositiveInfinity : -frame.key.inTangent;
                        frame.key.outTangent = (frame.key.outTangent == float.PositiveInfinity) ? float.PositiveInfinity : -frame.key.outTangent;
                    }
                }
                return frame;
            }
        }

        [CompilerGenerated]
        private sealed class <TranslateSelectedKeys>c__AnonStorey0
        {
            internal CurveEditor $this;
            internal Vector2 movement;

            internal CurveEditor.SavedCurve.SavedKeyFrame <>m__0(CurveEditor.SavedCurve.SavedKeyFrame keyframe, CurveEditor.SavedCurve curve)
            {
                if (keyframe.selected != CurveWrapper.SelectionMode.None)
                {
                    CurveEditor.SavedCurve.SavedKeyFrame frame = keyframe.Clone();
                    frame.key.time = Mathf.Clamp(frame.key.time + this.movement.x, this.$this.hRangeMin, this.$this.hRangeMax);
                    if (frame.selected == CurveWrapper.SelectionMode.Selected)
                    {
                        frame.key.value = this.$this.ClampVerticalValue(frame.key.value + this.movement.y, curve.curveId);
                    }
                    return frame;
                }
                return keyframe;
            }
        }

        [CompilerGenerated]
        private sealed class <UpdateCurvesFromPoints>c__AnonStorey4
        {
            internal CurveEditor.SavedCurve.SavedKeyFrame newKeyframe;

            internal bool <>m__0(CurveEditor.SavedCurve.SavedKeyFrame workingKeyframe) => 
                (Mathf.Abs((float) (workingKeyframe.key.time - this.newKeyframe.key.time)) < 1E-05f);
        }

        [CompilerGenerated]
        private sealed class <UpdateCurvesFromPoints>c__AnonStorey5
        {
            internal CurveEditor.SavedCurve.SavedKeyFrame newKeyframe;

            internal bool <>m__0(CurveEditor.SavedCurve.SavedKeyFrame workingKeyframe) => 
                (Mathf.Abs((float) (workingKeyframe.key.time - this.newKeyframe.key.time)) < 1E-05f);
        }

        private enum AxisLock
        {
            None,
            X,
            Y
        }

        public delegate void CallbackFunction();

        [StructLayout(LayoutKind.Sequential)]
        private struct KeyFrameCopy
        {
            public float time;
            public float value;
            public float inTangent;
            public float outTangent;
            public int idx;
            public int selectionIdx;
            public KeyFrameCopy(int idx, int selectionIdx, Keyframe source)
            {
                this.idx = idx;
                this.selectionIdx = selectionIdx;
                this.time = source.time;
                this.value = source.value;
                this.inTangent = source.inTangent;
                this.outTangent = source.outTangent;
            }
        }

        internal enum PickMode
        {
            None,
            Click,
            Marquee
        }

        private class SavedCurve
        {
            public int curveId;
            public List<SavedKeyFrame> keys;

            public delegate CurveEditor.SavedCurve.SavedKeyFrame KeyFrameOperation(CurveEditor.SavedCurve.SavedKeyFrame keyframe, CurveEditor.SavedCurve curve);

            public class SavedKeyFrame : IComparable
            {
                public Keyframe key;
                public CurveWrapper.SelectionMode selected;

                public SavedKeyFrame(Keyframe key, CurveWrapper.SelectionMode selected)
                {
                    this.key = key;
                    this.selected = selected;
                }

                public CurveEditor.SavedCurve.SavedKeyFrame Clone() => 
                    new CurveEditor.SavedCurve.SavedKeyFrame(this.key, this.selected);

                public int CompareTo(object _other)
                {
                    CurveEditor.SavedCurve.SavedKeyFrame frame = (CurveEditor.SavedCurve.SavedKeyFrame) _other;
                    float num = this.key.time - frame.key.time;
                    return ((num >= 0f) ? ((num <= 0f) ? 0 : 1) : -1);
                }
            }
        }

        internal class Styles
        {
            public GUIStyle axisLabelNumberField = new GUIStyle(EditorStyles.miniTextField);
            public GUIStyle dragLabel = "ProfilerBadge";
            public GUIStyle labelTickMarksX;
            public GUIStyle labelTickMarksY = "CurveEditorLabelTickMarks";
            public GUIStyle none = new GUIStyle();
            public Texture2D pointIcon = EditorGUIUtility.LoadIcon("curvekeyframe");
            public Texture2D pointIconSelected = EditorGUIUtility.LoadIcon("curvekeyframeselected");
            public Texture2D pointIconSelectedOverlay = EditorGUIUtility.LoadIcon("curvekeyframeselectedoverlay");
            public Texture2D pointIconSemiSelectedOverlay = EditorGUIUtility.LoadIcon("curvekeyframesemiselectedoverlay");
            public GUIStyle selectionRect = "SelectionRect";
            public GUIContent wrapModeMenuIcon = EditorGUIUtility.IconContent("AnimationWrapModeMenu");

            public Styles()
            {
                this.axisLabelNumberField.alignment = TextAnchor.UpperRight;
                this.labelTickMarksY.contentOffset = Vector2.zero;
                this.labelTickMarksX = new GUIStyle(this.labelTickMarksY);
                this.labelTickMarksX.clipping = TextClipping.Overflow;
            }
        }
    }
}

