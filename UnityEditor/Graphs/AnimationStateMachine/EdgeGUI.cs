namespace UnityEditor.Graphs.AnimationStateMachine
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEditor.Graphs;
    using UnityEngine;

    internal class EdgeGUI : IEdgeGUI
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<int> <edgeSelection>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private UnityEditor.Graphs.GraphGUI <host>k__BackingField;
        private const float kArrowEdgeWidth = 2f;
        private const float kEdgeClickWidth = 10f;
        private const float kEdgeToSelfOffset = 30f;
        private const float kEdgeWidth = 5f;
        private UnityEditor.Graphs.Edge m_DraggingEdge;
        private static Slot s_TargetDraggingSlot;

        public EdgeGUI()
        {
            this.edgeSelection = new List<int>();
        }

        public void BeginSlotDragging(Slot slot, bool allowStartDrag, bool allowEndDrag)
        {
            this.EndDragging();
            UnityEditor.Graphs.Edge item = new UnityEditor.Graphs.Edge(slot, null);
            this.host.graph.edges.Add(item);
            this.m_DraggingEdge = item;
            this.smHost.tool.wantsMouseMove = true;
        }

        public void DoDraggedEdge()
        {
        }

        public void DoEdges()
        {
            if (Event.current.type == EventType.Repaint)
            {
                int num = 0;
                foreach (UnityEditor.Graphs.Edge edge in this.host.graph.edges)
                {
                    Texture2D image = (Texture2D) UnityEditor.Graphs.Styles.connectionTexture.image;
                    UnityEngine.Color defaultTransitionColor = edge.color;
                    EdgeInfo edgeInfo = this.smHost.stateMachineGraph.GetEdgeInfo(edge);
                    if (edgeInfo != null)
                    {
                        if (edgeInfo.hasDefaultState)
                        {
                            defaultTransitionColor = UnityEditor.Graphs.AnimationStateMachine.EdgeGUI.defaultTransitionColor;
                        }
                        else if (edgeInfo.edgeType == EdgeType.Transition)
                        {
                            defaultTransitionColor = selectorTransitionColor;
                        }
                    }
                    bool flag = false;
                    for (int i = 0; (i < this.edgeSelection.Count) && !flag; i++)
                    {
                        if (this.edgeSelection[i] == num)
                        {
                            defaultTransitionColor = selectedEdgeColor;
                            flag = true;
                        }
                    }
                    this.DrawEdge(edge, image, defaultTransitionColor, edgeInfo);
                    num++;
                }
            }
            if (this.IsDragging())
            {
                s_TargetDraggingSlot = null;
                Event.current.Use();
            }
            if (this.ShouldStopDragging())
            {
                this.EndDragging();
                Event.current.Use();
            }
        }

        private static void DrawArrow(UnityEngine.Color color, Vector3 cross, Vector3 direction, Vector3 center)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Vector3[] vectorArray;
                vectorArray = new Vector3[] { center + (direction * 5f), (center - (direction * 5f)) + (cross * 5f), (center - (direction * 5f)) - (cross * 5f), vectorArray[0] };
                UnityEngine.Color color2 = color;
                Shader.SetGlobalColor("_HandleColor", color2);
                HandleUtility.ApplyWireMaterial();
                GL.Begin(4);
                GL.Color(color2);
                GL.Vertex(vectorArray[0]);
                GL.Vertex(vectorArray[1]);
                GL.Vertex(vectorArray[2]);
                GL.End();
                Handles.color = color;
                Handles.DrawAAPolyLine((Texture2D) UnityEditor.Graphs.Styles.connectionTexture.image, 2f, vectorArray);
            }
        }

        private static void DrawArrows(UnityEngine.Color color, Vector3 cross, Vector3[] edgePoints, EdgeInfo info, bool isSelf)
        {
            Vector3 vector = edgePoints[1] - edgePoints[0];
            Vector3 normalized = vector.normalized;
            Vector3 vector3 = ((Vector3) (vector * 0.5f)) + edgePoints[0];
            vector3 -= (Vector3) (cross * 0.5f);
            int num = 1;
            if ((info != null) && info.hasMultipleTransitions)
            {
                num = 3;
            }
            for (int i = 0; i < num; i++)
            {
                UnityEngine.Color red = color;
                if (info != null)
                {
                    if (info.debugState == EdgeDebugState.MuteAll)
                    {
                        red = UnityEngine.Color.red;
                    }
                    else if (info.debugState == EdgeDebugState.SoloAll)
                    {
                        red = UnityEngine.Color.green;
                    }
                    else if (i == 0)
                    {
                        if ((info.debugState == EdgeDebugState.MuteSome) || (info.debugState == EdgeDebugState.MuteAndSolo))
                        {
                            red = UnityEngine.Color.red;
                        }
                        if (info.debugState == EdgeDebugState.SoloSome)
                        {
                            red = UnityEngine.Color.green;
                        }
                    }
                    else if ((i == 2) && (info.debugState == EdgeDebugState.MuteAndSolo))
                    {
                        red = UnityEngine.Color.green;
                    }
                    if ((i == 1) && (info.edgeType == EdgeType.MixedTransition))
                    {
                        red = selectorTransitionColor;
                    }
                }
                Vector3 center = vector3 + ((Vector3) ((((num != 1) ? ((float) (i - 1)) : ((float) i)) * 13f) * (!isSelf ? normalized : cross)));
                DrawArrow(red, cross, normalized, center);
            }
        }

        private void DrawEdge(UnityEditor.Graphs.Edge edge, Texture2D tex, UnityEngine.Color color, EdgeInfo info)
        {
            Vector3 vector;
            Vector3[] edgePoints = GetEdgePoints(edge, out vector);
            Handles.color = color;
            if (edgePoints[0] == edgePoints[1])
            {
                Vector3[] vectorArray1 = new Vector3[] { edgePoints[0] + new Vector3(0f, 31f, 0f), edgePoints[0] + new Vector3(0f, 30f, 0f) };
                DrawArrows(color, -Vector3.right, vectorArray1, info, true);
            }
            else
            {
                Vector3[] points = new Vector3[] { edgePoints[0], edgePoints[1] };
                Handles.DrawAAPolyLine(tex, 5f, points);
                DrawArrows(color, vector, edgePoints, info, false);
                if (info != null)
                {
                    bool flag = this.smHost.liveLinkInfo.srcNode == edge.fromSlot.node;
                    bool flag2 = this.smHost.liveLinkInfo.dstNode == edge.toSlot.node;
                    if (((flag && flag2) || ((flag2 && this.smHost.liveLinkInfo.transitionInfo.entry) && (edge.fromSlot.node is EntryNode))) || (((flag && this.smHost.liveLinkInfo.transitionInfo.exit) && (edge.toSlot.node is ExitNode)) || ((flag2 && this.smHost.liveLinkInfo.transitionInfo.anyState) && (edge.fromSlot.node is AnyStateNode))))
                    {
                        float normalizedTime = this.smHost.liveLinkInfo.transitionInfo.normalizedTime;
                        if (this.smHost.liveLinkInfo.currentStateMachine != this.smHost.liveLinkInfo.nextStateMachine)
                        {
                            normalizedTime = (normalizedTime % 0.5f) / 0.5f;
                        }
                        Handles.color = selectedEdgeColor;
                        Vector3[] vectorArray3 = new Vector3[] { edgePoints[0], (Vector3) ((edgePoints[1] * normalizedTime) + (edgePoints[0] * (1f - normalizedTime))) };
                        Handles.DrawAAPolyLine((float) 10f, vectorArray3);
                    }
                }
            }
        }

        public void EndDragging()
        {
            if (this.m_DraggingEdge != null)
            {
                this.host.graph.RemoveEdge(this.m_DraggingEdge);
                this.m_DraggingEdge = null;
                this.smHost.tool.Repaint();
            }
        }

        public void EndSlotDragging(Slot slot, bool allowMultiple)
        {
            if ((this.m_DraggingEdge != null) && !(slot.node is AnyStateNode))
            {
                UnityEditor.Graphs.AnimationStateMachine.Node item = this.m_DraggingEdge.fromSlot.node as UnityEditor.Graphs.AnimationStateMachine.Node;
                UnityEditor.Graphs.AnimationStateMachine.Node node = slot.node as UnityEditor.Graphs.AnimationStateMachine.Node;
                if (slot == this.m_DraggingEdge.fromSlot)
                {
                    this.host.graph.RemoveEdge(this.m_DraggingEdge);
                }
                else
                {
                    this.m_DraggingEdge.toSlot = slot;
                    this.host.selection.Clear();
                    this.host.selection.Add(item);
                    Selection.activeObject = item.selectionObject;
                    item.Connect(node, this.m_DraggingEdge);
                }
                this.m_DraggingEdge = null;
                s_TargetDraggingSlot = null;
                Event.current.Use();
                this.smHost.tool.wantsMouseMove = false;
                AnimatorControllerTool.tool.RebuildGraph();
            }
        }

        public UnityEditor.Graphs.Edge FindClosestEdge()
        {
            UnityEditor.Graphs.Edge edge = null;
            float positiveInfinity = float.PositiveInfinity;
            Vector3 mousePosition = (Vector3) Event.current.mousePosition;
            foreach (UnityEditor.Graphs.Edge edge2 in this.host.graph.edges)
            {
                Vector3[] edgePoints = GetEdgePoints(edge2);
                float num2 = float.PositiveInfinity;
                if (edgePoints[0] == edgePoints[1])
                {
                    num2 = Vector3.Distance(edgeToSelfOffsetVector + edgePoints[0], mousePosition);
                }
                else
                {
                    num2 = HandleUtility.DistancePointLine(mousePosition, edgePoints[0], edgePoints[1]);
                }
                if ((num2 < positiveInfinity) && (num2 < 10f))
                {
                    positiveInfinity = num2;
                    edge = edge2;
                }
            }
            return edge;
        }

        private static Vector3 GetEdgeEndPosition(UnityEditor.Graphs.Edge edge)
        {
            if (IsEdgeBeingDragged(edge))
            {
                if (s_TargetDraggingSlot != null)
                {
                    return GetNodeCenterFromSlot(s_TargetDraggingSlot);
                }
                return (Vector3) Event.current.mousePosition;
            }
            return GetNodeCenterFromSlot(edge.toSlot);
        }

        private static Vector3[] GetEdgePoints(UnityEditor.Graphs.Edge edge)
        {
            Vector3 vector;
            return GetEdgePoints(edge, out vector);
        }

        private static Vector3[] GetEdgePoints(UnityEditor.Graphs.Edge edge, out Vector3 cross)
        {
            Vector3[] vectorArray = new Vector3[] { GetEdgeStartPosition(edge), GetEdgeEndPosition(edge) };
            Vector3 vector = vectorArray[0] - vectorArray[1];
            cross = Vector3.Cross(vector.normalized, Vector3.forward);
            vectorArray[0] += (Vector3) (cross * 5f);
            if (!IsEdgeBeingDragged(edge))
            {
                vectorArray[1] += (Vector3) (cross * 5f);
            }
            return vectorArray;
        }

        private static Vector3 GetEdgeStartPosition(UnityEditor.Graphs.Edge edge) => 
            GetNodeCenterFromSlot(edge.fromSlot);

        private static Vector3 GetNodeCenterFromSlot(Slot slot) => 
            ((Vector3) slot.node.position.center);

        private bool IsDragging() => 
            ((Event.current.type == EventType.MouseMove) && (this.m_DraggingEdge != null));

        private static bool IsEdgeBeingDragged(UnityEditor.Graphs.Edge edge) => 
            (edge.toSlot == null);

        private bool ShouldStopDragging() => 
            (((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Escape)) && (this.m_DraggingEdge != null));

        public void SlotDragging(Slot slot, bool allowEndDrag, bool allowMultiple)
        {
            if ((this.m_DraggingEdge != null) && !(slot.node is AnyStateNode))
            {
                s_TargetDraggingSlot = slot;
                Event.current.Use();
            }
        }

        private static UnityEngine.Color defaultTransitionColor =>
            new UnityEngine.Color(0.6f, 0.4f, 0f, 1f);

        public List<int> edgeSelection { get; set; }

        private static Vector3 edgeToSelfOffsetVector =>
            new Vector3(0f, 30f, 0f);

        public UnityEditor.Graphs.GraphGUI host { get; set; }

        private static UnityEngine.Color selectedEdgeColor =>
            new UnityEngine.Color(0.42f, 0.7f, 1f, 1f);

        private static UnityEngine.Color selectorTransitionColor =>
            new UnityEngine.Color(0.5f, 0.5f, 0.5f, 1f);

        private UnityEditor.Graphs.AnimationStateMachine.GraphGUI smHost =>
            (this.host as UnityEditor.Graphs.AnimationStateMachine.GraphGUI);
    }
}

