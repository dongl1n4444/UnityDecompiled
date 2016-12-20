namespace UnityEditor.Graphs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;

    public class EdgeGUI : IEdgeGUI
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private UnityEditor.Graphs.Edge <dontDrawEdge>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<int> <edgeSelection>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private GraphGUI <host>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private UnityEditor.Graphs.Edge <moveEdge>k__BackingField;
        public EdgeStyle edgeStyle = EdgeStyle.Curvy;
        private const float kEdgeCurveRadius = 5f;
        private const float kEdgeCurveTangentRatio = 0.6f;
        private const float kEdgeSlotYOffset = 9f;
        private const float kEdgeWidth = 3f;
        public static readonly UnityEngine.Color kFunctionEdgeColor = new UnityEngine.Color(1f, 1f, 1f);
        public static readonly UnityEngine.Color kObjectTypeEdgeColor = new UnityEngine.Color(0.65f, 1f, 0.65f);
        public static readonly UnityEngine.Color kSimpleTypeEdgeColor = new UnityEngine.Color(0.6f, 0.75f, 1f);
        private static Slot s_DragSourceSlot;
        private static Slot s_DropTarget;

        public EdgeGUI()
        {
            this.edgeSelection = new List<int>();
        }

        public void BeginSlotDragging(Slot slot, bool allowStartDrag, bool allowEndDrag)
        {
            if (allowStartDrag)
            {
                s_DragSourceSlot = slot;
                Event.current.Use();
            }
            if (allowEndDrag && (slot.edges.Count > 0))
            {
                this.moveEdge = slot.edges[slot.edges.Count - 1];
                s_DragSourceSlot = this.moveEdge.fromSlot;
                s_DropTarget = slot;
                Event.current.Use();
            }
        }

        public void DoDraggedEdge()
        {
            if (s_DragSourceSlot != null)
            {
                switch (Event.current.GetTypeForControl(0))
                {
                    case EventType.Repaint:
                    {
                        Rect position = s_DragSourceSlot.m_Position;
                        Vector2 mousePosition = Event.current.mousePosition;
                        if (s_DropTarget != null)
                        {
                            Rect rect2 = s_DropTarget.m_Position;
                            mousePosition = GUIClip.Clip(new Vector2(rect2.x, rect2.y + 9f));
                        }
                        DrawEdge(GUIClip.Clip(new Vector2(position.xMax, position.y + 9f)), mousePosition, (Texture2D) UnityEditor.Graphs.Styles.selectedConnectionTexture.image, UnityEngine.Color.white, this.edgeStyle);
                        break;
                    }
                    case EventType.MouseDrag:
                        s_DropTarget = null;
                        this.dontDrawEdge = null;
                        Event.current.Use();
                        break;
                }
            }
        }

        public void DoEdges()
        {
            int num = 0;
            int num2 = 0;
            if (Event.current.type == EventType.Repaint)
            {
                foreach (UnityEditor.Graphs.Edge edge in this.host.graph.edges)
                {
                    if ((edge != this.dontDrawEdge) && (edge != this.moveEdge))
                    {
                        Texture2D image = (Texture2D) UnityEditor.Graphs.Styles.connectionTexture.image;
                        if ((num < this.edgeSelection.Count) && (this.edgeSelection[num] == num2))
                        {
                            num++;
                            image = (Texture2D) UnityEditor.Graphs.Styles.selectedConnectionTexture.image;
                        }
                        UnityEngine.Color kObjectTypeEdgeColor = !edge.toSlot.isFlowSlot ? kSimpleTypeEdgeColor : kFunctionEdgeColor;
                        if (!edge.toSlot.isFlowSlot && edge.toSlot.dataType.IsSubclassOf(typeof(UnityEngine.Object)))
                        {
                            kObjectTypeEdgeColor = EdgeGUI.kObjectTypeEdgeColor;
                        }
                        kObjectTypeEdgeColor *= edge.color;
                        DrawEdge(edge, image, kObjectTypeEdgeColor, this.edgeStyle);
                        num2++;
                    }
                }
            }
            if ((s_DragSourceSlot != null) && (Event.current.type == EventType.MouseUp))
            {
                if (this.moveEdge != null)
                {
                    this.host.graph.RemoveEdge(this.moveEdge);
                    this.moveEdge = null;
                }
                if (s_DropTarget == null)
                {
                    this.EndDragging();
                    Event.current.Use();
                }
            }
        }

        private static void DrawEdge(UnityEditor.Graphs.Edge e, Texture2D tex, UnityEngine.Color color, EdgeStyle style)
        {
            Vector2 vector;
            Vector2 vector2;
            GetEdgeEndPoints(e, out vector, out vector2);
            DrawEdge(vector, vector2, tex, color, style);
        }

        private static void DrawEdge(Vector2 start, Vector2 end, Texture2D tex, UnityEngine.Color color, EdgeStyle style)
        {
            Vector3[] vectorArray;
            Vector3[] vectorArray2;
            if (style != EdgeStyle.Angular)
            {
                if (style == EdgeStyle.Curvy)
                {
                    GetCurvyConnectorValues(start, end, out vectorArray, out vectorArray2);
                    Handles.DrawBezier(vectorArray[0], vectorArray[1], vectorArray2[0], vectorArray2[1], color, tex, 3f);
                }
            }
            else
            {
                GetAngularConnectorValues(start, end, out vectorArray, out vectorArray2);
                DrawRoundedPolyLine(vectorArray, vectorArray2, tex, color);
            }
        }

        private static void DrawRoundedPolyLine(Vector3[] points, Vector3[] tangets, Texture2D tex, UnityEngine.Color color)
        {
            Handles.color = color;
            for (int i = 0; i < points.Length; i += 2)
            {
                Vector3[] vectorArray1 = new Vector3[] { points[i], points[i + 1] };
                Handles.DrawAAPolyLine(tex, 3f, vectorArray1);
            }
            for (int j = 0; j < tangets.Length; j += 2)
            {
                Handles.DrawBezier(points[j + 1], points[j + 2], tangets[j], tangets[j + 1], color, tex, 3f);
            }
        }

        public void EndDragging()
        {
            s_DragSourceSlot = (Slot) (s_DropTarget = null);
            UnityEditor.Graphs.Edge edge = null;
            this.moveEdge = edge;
            this.dontDrawEdge = edge;
        }

        public void EndSlotDragging(Slot slot, bool allowMultiple)
        {
            if (s_DropTarget == slot)
            {
                if (this.moveEdge != null)
                {
                    slot.node.graph.RemoveEdge(this.moveEdge);
                }
                while ((s_DropTarget.edges.Count > 0) && !allowMultiple)
                {
                    slot.node.graph.RemoveEdge(s_DropTarget.edges[0]);
                }
                try
                {
                    slot.node.graph.Connect(s_DragSourceSlot, slot);
                }
                finally
                {
                    this.EndDragging();
                    slot.node.graph.Dirty();
                    Event.current.Use();
                }
                GUIUtility.ExitGUI();
            }
        }

        public UnityEditor.Graphs.Edge FindClosestEdge()
        {
            Vector2 mousePosition = Event.current.mousePosition;
            float positiveInfinity = float.PositiveInfinity;
            UnityEditor.Graphs.Edge edge = null;
            foreach (UnityEditor.Graphs.Edge edge2 in this.host.graph.edges)
            {
                Vector2 vector2;
                Vector2 vector3;
                Vector3[] vectorArray;
                Vector3[] vectorArray2;
                GetEdgeEndPoints(edge2, out vector2, out vector3);
                if (this.edgeStyle == EdgeStyle.Angular)
                {
                    GetAngularConnectorValues(vector2, vector3, out vectorArray, out vectorArray2);
                }
                else
                {
                    GetCurvyConnectorValues(vector2, vector3, out vectorArray, out vectorArray2);
                }
                for (int i = 0; i < vectorArray.Length; i += 2)
                {
                    float num3 = HandleUtility.DistancePointLine((Vector3) mousePosition, vectorArray[i], vectorArray[i + 1]);
                    if (num3 < positiveInfinity)
                    {
                        positiveInfinity = num3;
                        edge = edge2;
                    }
                }
            }
            if (positiveInfinity > 10f)
            {
                edge = null;
            }
            return edge;
        }

        private static void GetAngularConnectorValues(Vector2 start, Vector2 end, out Vector3[] points, out Vector3[] tangents)
        {
            Vector2 vector = start - end;
            Vector2 vector2 = ((Vector2) (vector / 2f)) + end;
            float x = Mathf.Sign(vector.x);
            Vector2 vector3 = new Vector2(x, Mathf.Sign(vector.y));
            float introduced9 = Mathf.Min(Mathf.Abs((float) (vector.x / 2f)), 5f);
            Vector2 vector4 = new Vector2(introduced9, Mathf.Min(Mathf.Abs((float) (vector.y / 2f)), 5f));
            Vector3[] vectorArray1 = new Vector3[] { start, new Vector3(vector2.x + (vector4.x * vector3.x), start.y), new Vector3(vector2.x, start.y - (vector4.y * vector3.y)), new Vector3(vector2.x, end.y + (vector4.y * vector3.y)), new Vector3(vector2.x - (vector4.x * vector3.x), end.y), end };
            points = vectorArray1;
            Vector3[] vectorArray2 = new Vector3[4];
            Vector3 vector5 = points[1] - points[0];
            vectorArray2[0] = ((Vector3) ((vector5.normalized * vector4.x) * 0.6f)) + points[1];
            Vector3 vector6 = points[2] - points[3];
            vectorArray2[1] = ((Vector3) ((vector6.normalized * vector4.y) * 0.6f)) + points[2];
            Vector3 vector7 = points[3] - points[2];
            vectorArray2[2] = ((Vector3) ((vector7.normalized * vector4.y) * 0.6f)) + points[3];
            Vector3 vector8 = points[4] - points[5];
            vectorArray2[3] = ((Vector3) ((vector8.normalized * vector4.x) * 0.6f)) + points[4];
            tangents = vectorArray2;
        }

        private static void GetCurvyConnectorValues(Vector2 start, Vector2 end, out Vector3[] points, out Vector3[] tangents)
        {
            Vector3[] vectorArray1 = new Vector3[] { start, end };
            points = vectorArray1;
            tangents = new Vector3[2];
            float num = (start.y >= end.y) ? 0.7f : 0.3f;
            num = 0.5f;
            float num2 = 1f - num;
            float y = 0f;
            if (start.x > end.x)
            {
                num2 = num = -0.25f;
                float f = (start.x - end.x) / (start.y - end.y);
                if (Mathf.Abs(f) > 0.5f)
                {
                    float num5 = (Mathf.Abs(f) - 0.5f) / 8f;
                    y = Mathf.Min((float) (Mathf.Sqrt(num5) * 80f), (float) 80f);
                    if (start.y > end.y)
                    {
                        y = -y;
                    }
                }
            }
            Vector2 vector = start - end;
            float num6 = Mathf.Clamp01((vector.magnitude - 10f) / 50f);
            tangents[0] = (Vector3) (start + (new Vector2(((end.x - start.x) * num) + 30f, y) * num6));
            tangents[1] = (Vector3) (end + (new Vector2(((end.x - start.x) * -num2) - 30f, -y) * num6));
        }

        private static void GetEdgeEndPoints(UnityEditor.Graphs.Edge e, out Vector2 start, out Vector2 end)
        {
            start = GUIClip.Clip(new Vector2(e.fromSlot.m_Position.xMax, e.fromSlot.m_Position.y + 9f));
            end = GUIClip.Clip(new Vector2(e.toSlot.m_Position.x, e.toSlot.m_Position.y + 9f));
        }

        public void SlotDragging(Slot slot, bool allowEndDrag, bool allowMultiple)
        {
            if ((allowEndDrag && (s_DragSourceSlot != null)) && (s_DragSourceSlot != slot))
            {
                if (((s_DropTarget != slot) && slot.node.graph.CanConnect(s_DragSourceSlot, slot)) && !slot.node.graph.Connected(s_DragSourceSlot, slot))
                {
                    s_DropTarget = slot;
                    if ((slot.edges.Count > 0) && !allowMultiple)
                    {
                        this.dontDrawEdge = slot.edges[slot.edges.Count - 1];
                    }
                }
                Event.current.Use();
            }
        }

        private UnityEditor.Graphs.Edge dontDrawEdge { get; set; }

        public List<int> edgeSelection { get; set; }

        public GraphGUI host { get; set; }

        private UnityEditor.Graphs.Edge moveEdge { get; set; }

        public enum EdgeStyle
        {
            Angular,
            Curvy
        }
    }
}

