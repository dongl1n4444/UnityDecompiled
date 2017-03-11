namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class PolygonEditorUtility
    {
        [CompilerGenerated]
        private static Handles.CapFunction <>f__mg$cache0;
        [CompilerGenerated]
        private static Handles.CapFunction <>f__mg$cache1;
        private const float k_HandlePickDistance = 50f;
        private const float k_HandlePointSnap = 10f;
        private Collider2D m_ActiveCollider;
        private bool m_DeleteMode = false;
        private bool m_FirstOnSceneGUIAfterReset;
        private bool m_HandleEdge = false;
        private bool m_HandlePoint = false;
        private bool m_LeftIntersect = false;
        private bool m_LoopingCollider = false;
        private int m_MinPathPoints = 3;
        private bool m_RightIntersect = false;
        private int m_SelectedEdgePath = -1;
        private int m_SelectedEdgeVertex0 = -1;
        private int m_SelectedEdgeVertex1 = -1;
        private int m_SelectedPath = -1;
        private int m_SelectedVertex = -1;

        private bool DeleteCommandEvent(Event evt) => 
            (((evt.type == EventType.ExecuteCommand) || (evt.type == EventType.ValidateCommand)) && ((evt.commandName == "Delete") || (evt.commandName == "SoftDelete")));

        private void DrawEdgesForSelectedPoint(Vector3 worldPos, Transform transform, bool leftIntersect, bool rightIntersect, bool loop)
        {
            Vector2 vector2;
            Vector2 vector3;
            bool flag = true;
            bool flag2 = true;
            int pointCount = PolygonEditor.GetPointCount(this.m_SelectedPath);
            int pointIndex = this.m_SelectedVertex - 1;
            if (pointIndex == -1)
            {
                pointIndex = pointCount - 1;
                flag = loop;
            }
            int num3 = this.m_SelectedVertex + 1;
            if (num3 == pointCount)
            {
                num3 = 0;
                flag2 = loop;
            }
            Vector2 offset = this.m_ActiveCollider.offset;
            PolygonEditor.GetPoint(this.m_SelectedPath, pointIndex, out vector2);
            PolygonEditor.GetPoint(this.m_SelectedPath, num3, out vector3);
            vector2 += offset;
            vector3 += offset;
            Vector3 vector4 = transform.TransformPoint((Vector3) vector2);
            Vector3 vector5 = transform.TransformPoint((Vector3) vector3);
            vector4.z = vector5.z = worldPos.z;
            float width = 4f;
            if (flag)
            {
                Handles.color = (!leftIntersect && !this.m_DeleteMode) ? Color.green : Color.red;
                Vector3[] points = new Vector3[] { worldPos, vector4 };
                Handles.DrawAAPolyLine(width, points);
            }
            if (flag2)
            {
                Handles.color = (!rightIntersect && !this.m_DeleteMode) ? Color.green : Color.red;
                Vector3[] vectorArray2 = new Vector3[] { worldPos, vector5 };
                Handles.DrawAAPolyLine(width, vectorArray2);
            }
            Handles.color = Color.white;
        }

        private Vector2 GetNearestPointOnEdge(Vector2 point, Vector2 start, Vector2 end)
        {
            Vector2 rhs = point - start;
            Vector2 vector3 = end - start;
            Vector2 normalized = vector3.normalized;
            float num = Vector2.Dot(normalized, rhs);
            if (num <= 0f)
            {
                return start;
            }
            if (num >= Vector2.Distance(start, end))
            {
                return end;
            }
            Vector2 vector5 = (Vector2) (normalized * num);
            return (start + vector5);
        }

        public void OnSceneGUI()
        {
            if ((this.m_ActiveCollider != null) && !Tools.viewToolActive)
            {
                float num;
                Vector2 offset = this.m_ActiveCollider.offset;
                Event current = Event.current;
                this.m_DeleteMode = current.command || current.control;
                Transform transform = this.m_ActiveCollider.transform;
                GUIUtility.keyboardControl = 0;
                HandleUtility.s_CustomPickDistance = 50f;
                Plane plane = new Plane(-transform.forward, Vector3.zero);
                Ray ray = HandleUtility.GUIPointToWorldRay(current.mousePosition);
                plane.Raycast(ray, out num);
                Vector3 point = ray.GetPoint(num);
                Vector2 vector3 = transform.InverseTransformPoint(point);
                if ((current.type == EventType.MouseMove) || this.m_FirstOnSceneGUIAfterReset)
                {
                    int num2;
                    int num3;
                    int num4;
                    float num5;
                    if (PolygonEditor.GetNearestPoint(vector3 - offset, out num2, out num3, out num5))
                    {
                        this.m_SelectedPath = num2;
                        this.m_SelectedVertex = num3;
                    }
                    else
                    {
                        this.m_SelectedPath = -1;
                    }
                    if (PolygonEditor.GetNearestEdge(vector3 - offset, out num2, out num3, out num4, out num5, this.m_LoopingCollider))
                    {
                        this.m_SelectedEdgePath = num2;
                        this.m_SelectedEdgeVertex0 = num3;
                        this.m_SelectedEdgeVertex1 = num4;
                    }
                    else
                    {
                        this.m_SelectedEdgePath = -1;
                    }
                    if (current.type == EventType.MouseMove)
                    {
                        current.Use();
                    }
                }
                else if (current.type == EventType.MouseUp)
                {
                    this.m_LeftIntersect = false;
                    this.m_RightIntersect = false;
                }
                if (GUIUtility.hotControl == 0)
                {
                    if ((this.m_SelectedPath != -1) && (this.m_SelectedEdgePath != -1))
                    {
                        Vector2 vector4;
                        PolygonEditor.GetPoint(this.m_SelectedPath, this.m_SelectedVertex, out vector4);
                        vector4 += offset;
                        Vector3 world = transform.TransformPoint((Vector3) vector4);
                        Vector2 vector6 = HandleUtility.WorldToGUIPoint(world) - Event.current.mousePosition;
                        this.m_HandleEdge = vector6.sqrMagnitude > 100f;
                        this.m_HandlePoint = !this.m_HandleEdge;
                    }
                    else if (this.m_SelectedPath != -1)
                    {
                        this.m_HandlePoint = true;
                    }
                    else if (this.m_SelectedEdgePath != -1)
                    {
                        this.m_HandleEdge = true;
                    }
                    if (this.m_DeleteMode && this.m_HandleEdge)
                    {
                        this.m_HandleEdge = false;
                        this.m_HandlePoint = true;
                    }
                }
                bool flag = false;
                if (this.m_HandleEdge && !this.m_DeleteMode)
                {
                    Vector2 vector7;
                    Vector2 vector8;
                    PolygonEditor.GetPoint(this.m_SelectedEdgePath, this.m_SelectedEdgeVertex0, out vector7);
                    PolygonEditor.GetPoint(this.m_SelectedEdgePath, this.m_SelectedEdgeVertex1, out vector8);
                    vector7 += offset;
                    vector8 += offset;
                    Vector3 start = transform.TransformPoint((Vector3) vector7);
                    Vector3 end = transform.TransformPoint((Vector3) vector8);
                    start.z = end.z = 0f;
                    Handles.color = Color.green;
                    Vector3[] points = new Vector3[] { start, end };
                    Handles.DrawAAPolyLine((float) 4f, points);
                    Handles.color = Color.white;
                    Vector2 vector11 = this.GetNearestPointOnEdge(transform.TransformPoint((Vector3) vector3), start, end);
                    EditorGUI.BeginChangeCheck();
                    float handleSize = HandleUtility.GetHandleSize((Vector3) vector11) * 0.04f;
                    Handles.color = Color.green;
                    if (<>f__mg$cache0 == null)
                    {
                        <>f__mg$cache0 = new Handles.CapFunction(Handles.DotHandleCap);
                    }
                    vector11 = Handles.Slider2D((Vector3) vector11, new Vector3(0f, 0f, 1f), new Vector3(1f, 0f, 0f), new Vector3(0f, 1f, 0f), handleSize, <>f__mg$cache0, Vector3.zero);
                    Handles.color = Color.white;
                    if (EditorGUI.EndChangeCheck())
                    {
                        PolygonEditor.InsertPoint(this.m_SelectedEdgePath, this.m_SelectedEdgeVertex1, ((Vector2) ((vector7 + vector8) / 2f)) - offset);
                        this.m_SelectedPath = this.m_SelectedEdgePath;
                        this.m_SelectedVertex = this.m_SelectedEdgeVertex1;
                        this.m_HandleEdge = false;
                        this.m_HandlePoint = true;
                        flag = true;
                    }
                }
                if (this.m_HandlePoint)
                {
                    Vector2 vector12;
                    PolygonEditor.GetPoint(this.m_SelectedPath, this.m_SelectedVertex, out vector12);
                    vector12 += offset;
                    Vector3 vector13 = transform.TransformPoint((Vector3) vector12);
                    vector13.z = 0f;
                    Vector2 a = HandleUtility.WorldToGUIPoint(vector13);
                    float num8 = HandleUtility.GetHandleSize(vector13) * 0.04f;
                    if (((this.m_DeleteMode && (current.type == EventType.MouseDown)) && (Vector2.Distance(a, Event.current.mousePosition) < 50f)) || this.DeleteCommandEvent(current))
                    {
                        if ((current.type != EventType.ValidateCommand) && (PolygonEditor.GetPointCount(this.m_SelectedPath) > this.m_MinPathPoints))
                        {
                            PolygonEditor.RemovePoint(this.m_SelectedPath, this.m_SelectedVertex);
                            this.Reset();
                            flag = true;
                        }
                        current.Use();
                    }
                    EditorGUI.BeginChangeCheck();
                    Handles.color = !this.m_DeleteMode ? Color.green : Color.red;
                    if (<>f__mg$cache1 == null)
                    {
                        <>f__mg$cache1 = new Handles.CapFunction(Handles.DotHandleCap);
                    }
                    Vector3 position = Handles.Slider2D(vector13, new Vector3(0f, 0f, 1f), new Vector3(1f, 0f, 0f), new Vector3(0f, 1f, 0f), num8, <>f__mg$cache1, Vector3.zero);
                    Handles.color = Color.white;
                    if (EditorGUI.EndChangeCheck() && !this.m_DeleteMode)
                    {
                        vector12 = transform.InverseTransformPoint(position) - offset;
                        PolygonEditor.TestPointMove(this.m_SelectedPath, this.m_SelectedVertex, vector12, out this.m_LeftIntersect, out this.m_RightIntersect, this.m_LoopingCollider);
                        PolygonEditor.SetPoint(this.m_SelectedPath, this.m_SelectedVertex, vector12);
                        flag = true;
                    }
                    if (!flag)
                    {
                        this.DrawEdgesForSelectedPoint(position, transform, this.m_LeftIntersect, this.m_RightIntersect, this.m_LoopingCollider);
                    }
                }
                if (flag)
                {
                    Undo.RecordObject(this.m_ActiveCollider, "Edit Collider");
                    PolygonEditor.ApplyEditing(this.m_ActiveCollider);
                }
                if (this.DeleteCommandEvent(current))
                {
                    Event.current.Use();
                }
                this.m_FirstOnSceneGUIAfterReset = false;
            }
        }

        public void Reset()
        {
            this.m_SelectedPath = -1;
            this.m_SelectedVertex = -1;
            this.m_SelectedEdgePath = -1;
            this.m_SelectedEdgeVertex0 = -1;
            this.m_SelectedEdgeVertex1 = -1;
            this.m_LeftIntersect = false;
            this.m_RightIntersect = false;
            this.m_FirstOnSceneGUIAfterReset = true;
            this.m_HandlePoint = false;
            this.m_HandleEdge = false;
        }

        public void StartEditing(Collider2D collider)
        {
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
            this.Reset();
            PolygonCollider2D colliderd = collider as PolygonCollider2D;
            if (colliderd != null)
            {
                this.m_ActiveCollider = collider;
                this.m_LoopingCollider = true;
                this.m_MinPathPoints = 3;
                PolygonEditor.StartEditing(colliderd);
            }
            else
            {
                EdgeCollider2D colliderd2 = collider as EdgeCollider2D;
                if (colliderd2 == null)
                {
                    throw new NotImplementedException($"PolygonEditorUtility does not support {collider}");
                }
                this.m_ActiveCollider = collider;
                this.m_LoopingCollider = false;
                this.m_MinPathPoints = 2;
                PolygonEditor.StartEditing(colliderd2);
            }
        }

        public void StopEditing()
        {
            PolygonEditor.StopEditing();
            this.m_ActiveCollider = null;
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
        }

        private void UndoRedoPerformed()
        {
            if (this.m_ActiveCollider != null)
            {
                Collider2D activeCollider = this.m_ActiveCollider;
                this.StopEditing();
                this.StartEditing(activeCollider);
            }
        }
    }
}

