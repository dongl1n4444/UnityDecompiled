namespace UnityEditor.Graphs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Profiling;

    [Serializable]
    public abstract class GraphGUI : ScriptableObject
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map1;
        private static readonly int kDragGraphControlID = "DragGraph".GetHashCode();
        private static readonly int kDragNodesControlID = "DragNodes".GetHashCode();
        private static readonly int kDragSelectionControlID = "DragSelection".GetHashCode();
        protected const float kGraphPaddingMultiplier = 0.6f;
        private static readonly UnityEngine.Color kGridMajorColorDark = new UnityEngine.Color(0f, 0f, 0f, 0.28f);
        private static readonly UnityEngine.Color kGridMajorColorLight = new UnityEngine.Color(0f, 0f, 0f, 0.15f);
        private static readonly UnityEngine.Color kGridMinorColorDark = new UnityEngine.Color(0f, 0f, 0f, 0.18f);
        private static readonly UnityEngine.Color kGridMinorColorLight = new UnityEngine.Color(0f, 0f, 0f, 0.1f);
        private const float kMajorGridSize = 120f;
        protected const float kNodeGridSize = 12f;
        private const int kNodePositionIncrement = 40;
        private const int kNodePositionXMax = 700;
        private const int kNodePositionYMax = 250;
        private static readonly GUIContent kTempContent = new GUIContent();
        protected bool m_CenterGraph;
        protected Vector2? m_contextMenuMouseDownPosition;
        private Vector2 m_DragNodeDistance;
        private Vector2 m_DragStartPoint;
        protected IEdgeGUI m_EdgeGUI;
        [SerializeField]
        protected Graph m_Graph;
        protected Rect m_GraphClientArea;
        protected EditorWindow m_Host;
        private readonly Dictionary<Node, Rect> m_InitialDragNodePositions = new Dictionary<Node, Rect>();
        private SelectionDragMode m_IsDraggingSelection = SelectionDragMode.None;
        private Rect m_LastGraphExtents;
        private Vector2 m_LastMousePosition;
        private Vector2 m_LastNodeAddedPosition = new Vector2(40f, 0f);
        private List<Node> m_OldSelection;
        [SerializeField]
        protected Vector2 m_ScrollPosition;
        protected List<NodeTool> m_Tools = new List<NodeTool>();
        public List<Node> selection = new List<Node>();

        protected GraphGUI()
        {
        }

        protected virtual void AddNode(Node node)
        {
            this.m_Graph.AddNode(node);
            if ((node.position.x == 0f) && (node.position.y == 0f))
            {
                this.m_LastNodeAddedPosition.y += 40f;
                this.m_LastNodeAddedPosition.x += 40f;
                if (this.m_LastNodeAddedPosition.y >= 250f)
                {
                    this.m_LastNodeAddedPosition.y = 40f;
                }
                if (this.m_LastNodeAddedPosition.x >= 700f)
                {
                    this.m_LastNodeAddedPosition.x = 40f;
                }
                node.position = new Rect(this.m_LastNodeAddedPosition.x, this.m_LastNodeAddedPosition.y, 0f, 0f);
            }
        }

        public virtual void AddTools()
        {
        }

        public void BeginGraphGUI(EditorWindow host, Rect position)
        {
            this.m_GraphClientArea = position;
            this.m_Host = host;
            if (Event.current.type == EventType.Repaint)
            {
                UnityEditor.Graphs.Styles.graphBackground.Draw(position, false, false, false, false);
            }
            this.m_ScrollPosition = GUI.BeginScrollView(position, this.m_ScrollPosition, this.m_Graph.graphExtents, GUIStyle.none, GUIStyle.none);
            this.DrawGrid();
        }

        public void BeginToolbarGUI(Rect position)
        {
            GUI.BeginGroup(position);
            GUILayout.BeginHorizontal(GUIContent.none, EditorStyles.toolbar, new GUILayoutOption[0]);
        }

        protected static float CeilValueToGrid(float value) => 
            (Mathf.Ceil(value / 12f) * 12f);

        public void CenterGraph()
        {
            this.m_CenterGraph = true;
        }

        public virtual void ClearSelection()
        {
            this.selection.Clear();
            this.edgeGUI.edgeSelection.Clear();
        }

        private void ContextMenuClick(object userData, string[] options, int selected)
        {
            if (selected >= 0)
            {
                ContextMenuData data = (ContextMenuData) userData;
                string text = data.items[selected].text;
                switch (text)
                {
                    case "Cut":
                    case "Copy":
                    case "Duplicate":
                    case "Delete":
                        this.m_Host.SendEvent(EditorGUIUtility.CommandEvent(text));
                        break;

                    case "Paste":
                        this.m_contextMenuMouseDownPosition = new Vector2?(data.mousePosition);
                        this.m_Host.SendEvent(EditorGUIUtility.CommandEvent(text));
                        break;
                }
            }
        }

        protected Graph CopyNodesPasteboardData(out int[] ids)
        {
            Graph graph = ScriptableObject.CreateInstance(this.m_Graph.GetType()) as Graph;
            graph.nodes.AddRange(this.selection.ToArray());
            foreach (UnityEditor.Graphs.Edge edge in this.m_Graph.edges)
            {
                if (this.selection.Contains(edge.fromSlot.node) && this.selection.Contains(edge.toSlot.node))
                {
                    graph.Connect(edge.fromSlot, edge.toSlot);
                }
            }
            List<int> list = new List<int> {
                graph.GetInstanceID()
            };
            list.AddRange(Graph.GetNodeIdsForSerialization(graph));
            ids = list.ToArray();
            return graph;
        }

        protected virtual void CopyNodesToPasteboard()
        {
        }

        private void DeleteNodesAndEdges(List<Node> nodes, List<UnityEditor.Graphs.Edge> edges)
        {
            foreach (UnityEditor.Graphs.Edge edge in edges)
            {
                this.m_Graph.RemoveEdge(edge);
            }
            this.edgeGUI.edgeSelection.Clear();
            foreach (UnityEditor.Graphs.Edge edge2 in FindEdgesOfNodes(nodes, false))
            {
                this.m_Graph.RemoveEdge(edge2);
            }
            foreach (Node node in nodes)
            {
                this.m_Graph.DestroyNode(node);
            }
        }

        public virtual void DoBackgroundClickAction()
        {
        }

        private void DoSlot(int id, Rect position, string title, UnityEditor.Graphs.Slot slot, bool allowStartDrag, bool allowEndDrag, bool allowMultiple, GUIStyle style)
        {
            slot.m_Position = GUIClip.Unclip(position);
            Event current = Event.current;
            switch (current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (position.Contains(Event.current.mousePosition) && (current.button == 0))
                    {
                        this.edgeGUI.BeginSlotDragging(slot, allowStartDrag, allowEndDrag);
                    }
                    break;

                case EventType.MouseUp:
                    if (position.Contains(current.mousePosition) && (current.button == 0))
                    {
                        this.edgeGUI.EndSlotDragging(slot, allowMultiple);
                    }
                    break;

                case EventType.MouseDrag:
                    if (position.Contains(current.mousePosition) && (current.button == 0))
                    {
                        this.edgeGUI.SlotDragging(slot, allowEndDrag, allowMultiple);
                    }
                    break;

                case EventType.Repaint:
                    style.Draw(position, new GUIContent(title), id, IsSlotActive(slot));
                    break;
            }
        }

        private void DragGraph()
        {
            int controlID = GUIUtility.GetControlID(kDragGraphControlID, FocusType.Passive);
            Event current = Event.current;
            if ((current.button == 2) || ((current.button == 0) && current.alt))
            {
                switch (current.GetTypeForControl(controlID))
                {
                    case EventType.MouseDown:
                        GUIUtility.hotControl = controlID;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                        break;

                    case EventType.MouseUp:
                        if (GUIUtility.hotControl == controlID)
                        {
                            GUIUtility.hotControl = 0;
                            current.Use();
                            EditorGUIUtility.SetWantsMouseJumping(0);
                            break;
                        }
                        break;

                    case EventType.MouseMove:
                    case EventType.MouseDrag:
                        if (GUIUtility.hotControl == controlID)
                        {
                            this.m_ScrollPosition -= current.delta;
                            current.Use();
                            break;
                        }
                        break;
                }
            }
        }

        protected void DragNodes()
        {
            Event current = Event.current;
            int controlID = GUIUtility.GetControlID(kDragNodesControlID, FocusType.Passive);
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (current.button == 0)
                    {
                        this.m_LastMousePosition = GUIClip.Unclip(current.mousePosition);
                        this.m_DragNodeDistance = Vector2.zero;
                        foreach (Node node in this.selection)
                        {
                            this.m_InitialDragNodePositions[node] = node.position;
                            node.BeginDrag();
                        }
                        GUIUtility.hotControl = controlID;
                        current.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        foreach (Node node2 in this.selection)
                        {
                            node2.EndDrag();
                        }
                        this.m_InitialDragNodePositions.Clear();
                        GUIUtility.hotControl = 0;
                        current.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        this.m_DragNodeDistance += GUIClip.Unclip(current.mousePosition) - this.m_LastMousePosition;
                        this.m_LastMousePosition = GUIClip.Unclip(current.mousePosition);
                        foreach (Node node4 in this.selection)
                        {
                            Rect position = node4.position;
                            Rect rect2 = this.m_InitialDragNodePositions[node4];
                            position.x = rect2.x + this.m_DragNodeDistance.x;
                            position.y = rect2.y + this.m_DragNodeDistance.y;
                            node4.position = SnapPositionToGrid(position);
                            node4.OnDrag();
                            node4.Dirty();
                        }
                        current.Use();
                    }
                    break;

                case EventType.KeyDown:
                    if ((GUIUtility.hotControl == controlID) && (current.keyCode == KeyCode.Escape))
                    {
                        foreach (Node node3 in this.selection)
                        {
                            node3.position = SnapPositionToGrid(this.m_InitialDragNodePositions[node3]);
                            node3.Dirty();
                        }
                        GUIUtility.hotControl = 0;
                        current.Use();
                    }
                    break;
            }
        }

        protected void DragSelection(Rect position)
        {
            int controlID = GUIUtility.GetControlID(kDragSelectionControlID, FocusType.Passive);
            Event current = Event.current;
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if ((position.Contains(current.mousePosition) && (current.button == 0)) && ((current.clickCount != 2) && !current.alt))
                    {
                        GUIUtility.hotControl = controlID;
                        this.m_DragStartPoint = current.mousePosition;
                        this.m_OldSelection = new List<Node>(this.selection);
                        UnityEditor.Graphs.Edge item = this.edgeGUI.FindClosestEdge();
                        if (item != null)
                        {
                            if (!EditorGUI.actionKey && !current.shift)
                            {
                                this.ClearSelection();
                            }
                            int index = this.m_Graph.edges.IndexOf(item);
                            if (this.edgeGUI.edgeSelection.Contains(index))
                            {
                                this.edgeGUI.edgeSelection.Remove(index);
                            }
                            else
                            {
                                this.edgeGUI.edgeSelection.Add(index);
                            }
                            current.Use();
                        }
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                        this.m_OldSelection.Clear();
                        this.UpdateUnitySelection();
                        this.m_IsDraggingSelection = SelectionDragMode.None;
                        current.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        if ((!EditorGUI.actionKey && !current.shift) && (this.m_IsDraggingSelection == SelectionDragMode.Pick))
                        {
                            this.ClearSelection();
                        }
                        this.m_IsDraggingSelection = SelectionDragMode.Rect;
                        this.SelectNodesInRect(FromToRect(this.m_DragStartPoint, current.mousePosition));
                        current.Use();
                    }
                    break;

                case EventType.KeyDown:
                    if ((this.m_IsDraggingSelection != SelectionDragMode.None) && (current.keyCode == KeyCode.Escape))
                    {
                        this.selection = this.m_OldSelection;
                        GUIUtility.hotControl = 0;
                        this.m_IsDraggingSelection = SelectionDragMode.None;
                        current.Use();
                    }
                    break;

                case EventType.Repaint:
                    if (this.m_IsDraggingSelection == SelectionDragMode.Rect)
                    {
                        UnityEditor.Graphs.Styles.selectionRect.Draw(FromToRect(this.m_DragStartPoint, current.mousePosition), false, false, false, false);
                    }
                    break;
            }
        }

        private void DrawGrid()
        {
            if (Event.current.type == EventType.Repaint)
            {
                Profiler.BeginSample("DrawGrid");
                HandleUtility.ApplyWireMaterial();
                GL.PushMatrix();
                GL.Begin(1);
                this.DrawGridLines(12f, gridMinorColor);
                this.DrawGridLines(120f, gridMajorColor);
                GL.End();
                GL.PopMatrix();
                Profiler.EndSample();
            }
        }

        private void DrawGridLines(float gridSize, UnityEngine.Color gridColor)
        {
            GL.Color(gridColor);
            for (float i = this.m_Graph.graphExtents.xMin - (this.m_Graph.graphExtents.xMin % gridSize); i < this.m_Graph.graphExtents.xMax; i += gridSize)
            {
                this.DrawLine(new Vector2(i, this.m_Graph.graphExtents.yMin), new Vector2(i, this.m_Graph.graphExtents.yMax));
            }
            GL.Color(gridColor);
            for (float j = this.m_Graph.graphExtents.yMin - (this.m_Graph.graphExtents.yMin % gridSize); j < this.m_Graph.graphExtents.yMax; j += gridSize)
            {
                this.DrawLine(new Vector2(this.m_Graph.graphExtents.xMin, j), new Vector2(this.m_Graph.graphExtents.xMax, j));
            }
        }

        private void DrawLine(Vector2 p1, Vector2 p2)
        {
            GL.Vertex((Vector3) p1);
            GL.Vertex((Vector3) p2);
        }

        protected virtual void DuplicateNodesThroughPasteboard()
        {
        }

        public void EndGraphGUI()
        {
            this.UpdateGraphExtents();
            this.UpdateScrollPosition();
            this.DragGraph();
            GUI.EndScrollView();
        }

        public void EndToolbarGUI()
        {
            GUILayout.EndHorizontal();
            GUI.EndGroup();
        }

        private static List<UnityEditor.Graphs.Edge> FindEdgesOfNodes(List<Node> nodes, bool requireBoth)
        {
            Dictionary<UnityEditor.Graphs.Edge, int> dictionary = new Dictionary<UnityEditor.Graphs.Edge, int>();
            foreach (Node node in nodes)
            {
                foreach (UnityEditor.Graphs.Slot slot in node.slots)
                {
                    foreach (UnityEditor.Graphs.Edge edge in slot.edges)
                    {
                        if (!dictionary.ContainsKey(edge))
                        {
                            dictionary.Add(edge, 1);
                        }
                        else
                        {
                            Dictionary<UnityEditor.Graphs.Edge, int> dictionary2;
                            UnityEditor.Graphs.Edge edge2;
                            (dictionary2 = dictionary)[edge2 = edge] = dictionary2[edge2] + 1;
                        }
                    }
                }
            }
            List<UnityEditor.Graphs.Edge> list = new List<UnityEditor.Graphs.Edge>();
            int num = !requireBoth ? 1 : 2;
            foreach (KeyValuePair<UnityEditor.Graphs.Edge, int> pair in dictionary)
            {
                if (pair.Value >= num)
                {
                    list.Add(pair.Key);
                }
            }
            return list;
        }

        internal static Rect FromToRect(Vector2 start, Vector2 end)
        {
            Rect rect = new Rect(start.x, start.y, end.x - start.x, end.y - start.y);
            if (rect.width < 0f)
            {
                rect.x += rect.width;
                rect.width = -rect.width;
            }
            if (rect.height < 0f)
            {
                rect.y += rect.height;
                rect.height = -rect.height;
            }
            return rect;
        }

        private static Vector2 GetMidPoint(List<Node> nodes)
        {
            float maxValue = float.MaxValue;
            float num2 = float.MaxValue;
            float minValue = float.MinValue;
            float num4 = float.MinValue;
            foreach (Node node in nodes)
            {
                maxValue = Math.Min(node.position.x, maxValue);
                num2 = Math.Min(node.position.y, num2);
                minValue = Math.Max(node.position.x, minValue);
                num4 = Math.Max(node.position.y, num4);
            }
            return new Vector2((minValue + maxValue) / 2f, (num4 + num2) / 2f);
        }

        protected void HandleMenuEvents()
        {
            string[] source = new string[] { "SoftDelete", "Delete", "Cut", "Copy", "Paste", "Duplicate", "SelectAll" };
            Event current = Event.current;
            if (((current.type == EventType.ValidateCommand) || (current.type == EventType.ExecuteCommand)) && source.Contains<string>(current.commandName))
            {
                if (current.type == EventType.ValidateCommand)
                {
                    current.Use();
                }
                else
                {
                    string commandName = current.commandName;
                    if (commandName != null)
                    {
                        int num;
                        if (<>f__switch$map1 == null)
                        {
                            Dictionary<string, int> dictionary = new Dictionary<string, int>(7) {
                                { 
                                    "SoftDelete",
                                    0
                                },
                                { 
                                    "Delete",
                                    0
                                },
                                { 
                                    "Cut",
                                    1
                                },
                                { 
                                    "Copy",
                                    2
                                },
                                { 
                                    "Paste",
                                    3
                                },
                                { 
                                    "Duplicate",
                                    4
                                },
                                { 
                                    "SelectAll",
                                    5
                                }
                            };
                            <>f__switch$map1 = dictionary;
                        }
                        if (<>f__switch$map1.TryGetValue(commandName, out num))
                        {
                            switch (num)
                            {
                                case 0:
                                    if ((this.selection.Count != 0) || (this.edgeGUI.edgeSelection.Count != 0))
                                    {
                                        if (current.type == EventType.ExecuteCommand)
                                        {
                                            this.DeleteNodesAndEdges(this.selection, Enumerable.Select<int, UnityEditor.Graphs.Edge>(this.edgeGUI.edgeSelection, new Func<int, UnityEditor.Graphs.Edge>(this, (IntPtr) this.<HandleMenuEvents>m__0)).ToList<UnityEditor.Graphs.Edge>());
                                        }
                                        current.Use();
                                        break;
                                    }
                                    break;

                                case 1:
                                    this.CopyNodesToPasteboard();
                                    this.DeleteNodesAndEdges(this.selection, Enumerable.Select<int, UnityEditor.Graphs.Edge>(this.edgeGUI.edgeSelection, new Func<int, UnityEditor.Graphs.Edge>(this, (IntPtr) this.<HandleMenuEvents>m__1)).ToList<UnityEditor.Graphs.Edge>());
                                    current.Use();
                                    break;

                                case 2:
                                    this.CopyNodesToPasteboard();
                                    current.Use();
                                    break;

                                case 3:
                                    this.PasteNodesFromPasteboard();
                                    current.Use();
                                    break;

                                case 4:
                                    this.DuplicateNodesThroughPasteboard();
                                    current.Use();
                                    break;

                                case 5:
                                    this.ClearSelection();
                                    this.selection.AddRange(this.m_Graph.nodes);
                                    current.Use();
                                    break;
                            }
                        }
                    }
                }
            }
        }

        internal void InternalOnSelectionChange()
        {
            if (!(Selection.activeObject is Node))
            {
                this.m_Tools.Clear();
                this.AddTools();
            }
        }

        private static bool IsSlotActive(UnityEditor.Graphs.Slot slot)
        {
            if (slot.edges.Count > 0)
            {
                return true;
            }
            if (slot.isFlowSlot || slot.isOutputDataSlot)
            {
                return false;
            }
            if (slot.dataType == null)
            {
                return false;
            }
            return (slot.GetProperty().value != null);
        }

        public void LayoutSlot(UnityEditor.Graphs.Slot s, string title, bool allowStartDrag, bool allowEndDrag, bool allowMultiple, GUIStyle style)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            kTempContent.text = title;
            Rect position = GUILayoutUtility.GetRect(kTempContent, style);
            if ((Event.current.type != EventType.Layout) && (Event.current.type != EventType.Used))
            {
                UnityEngine.Color backgroundColor = GUI.backgroundColor;
                if (s.edges.Count > 0)
                {
                    GUI.backgroundColor = s.edges[0].color;
                }
                this.DoSlot(controlID, position, title, s, allowStartDrag, allowEndDrag, allowMultiple, style);
                GUI.backgroundColor = backgroundColor;
            }
        }

        public virtual void NodeGUI(Node n)
        {
            this.SelectNode(n);
            foreach (UnityEditor.Graphs.Slot slot in n.inputSlots)
            {
                this.LayoutSlot(slot, slot.title, false, true, false, UnityEditor.Graphs.Styles.varPinIn);
            }
            n.NodeUI(this);
            foreach (UnityEditor.Graphs.Slot slot2 in n.outputSlots)
            {
                this.LayoutSlot(slot2, slot2.title, true, false, true, UnityEditor.Graphs.Styles.varPinOut);
            }
            this.DragNodes();
        }

        protected static void OffsetPastedNodePositions(IEnumerable<Node> nodes, Vector2? pastePosition)
        {
            float num = 15f;
            float num2 = 15f;
            if (pastePosition.HasValue)
            {
                float maxValue = float.MaxValue;
                float a = float.MaxValue;
                foreach (Node node in nodes)
                {
                    maxValue = Mathf.Min(maxValue, node.position.x);
                    a = Mathf.Min(a, node.position.y);
                }
                num = pastePosition.Value.x - maxValue;
                num2 = pastePosition.Value.y - a;
            }
            foreach (Node node2 in nodes)
            {
                node2.position = new Rect(node2.position.x + num, node2.position.y + num2, node2.position.width, node2.position.height);
            }
        }

        public virtual void OnEnable()
        {
            this.AddTools();
        }

        public virtual void OnGraphGUI()
        {
            this.m_Host.BeginWindows();
            foreach (Node node in this.m_Graph.nodes)
            {
                <OnGraphGUI>c__AnonStorey0 storey = new <OnGraphGUI>c__AnonStorey0 {
                    $this = this,
                    n2 = node
                };
                bool on = this.selection.Contains(node);
                UnityEditor.Graphs.Styles.Color color = !node.nodeIsInvalid ? node.color : UnityEditor.Graphs.Styles.Color.Red;
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(0f), GUILayout.Height(0f) };
                node.position = GUILayout.Window(node.GetInstanceID(), node.position, new GUI.WindowFunction(storey.<>m__0), node.title, UnityEditor.Graphs.Styles.GetNodeStyle(node.style, color, on), options);
            }
            this.m_Host.EndWindows();
            this.edgeGUI.DoEdges();
            this.edgeGUI.DoDraggedEdge();
            this.DragSelection(new Rect(-5000f, -5000f, 10000f, 10000f));
            this.ShowContextMenu();
            this.HandleMenuEvents();
        }

        public virtual void OnNodeLibraryGUI(EditorWindow host, Rect position)
        {
        }

        public virtual void OnToolbarGUI()
        {
            GUI.enabled = this.selection.Count > 0;
            if (GUILayout.Button("Group " + this.selection.Count, EditorStyles.toolbarButton, new GUILayoutOption[0]))
            {
                Vector2 midPoint = GetMidPoint(this.selection);
                Debug.Log("Grouping" + this.selection.Count + " nodes");
                Node node = GroupNode.FromNodes("GroupNode", this.selection, this.m_Graph.GetType());
                node.position = new Rect(midPoint.x - (node.position.x / 2f), midPoint.y - (node.position.y / 2f), node.position.width, node.position.height);
            }
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUI.enabled = EditorApplication.isPlaying;
            if (GUILayout.Button("Reload", EditorStyles.toolbarButton, new GUILayoutOption[0]))
            {
                InternalEditorUtility.RequestScriptReload();
            }
            GUI.enabled = true;
            GUILayout.Space(5f);
        }

        protected virtual void PasteNodesFromPasteboard()
        {
        }

        protected virtual void PasteNodesPasteboardData(Graph dummyGraph)
        {
        }

        protected void SelectNode(Node node)
        {
            Event current = Event.current;
            if (((current.type == EventType.MouseDown) && ((current.button == 0) || (current.button == 1))) && (current.clickCount == 1))
            {
                if (EditorGUI.actionKey || current.shift)
                {
                    if (this.selection.Contains(node))
                    {
                        this.selection.Remove(node);
                    }
                    else
                    {
                        this.selection.Add(node);
                    }
                    current.Use();
                }
                else
                {
                    if (!this.selection.Contains(node))
                    {
                        this.ClearSelection();
                        this.selection.Add(node);
                    }
                    HandleUtility.Repaint();
                }
                this.UpdateUnitySelection();
                GUIUtility.keyboardControl = 0;
                EditorGUI.EndEditingActiveTextField();
            }
        }

        private void SelectNodesInRect(Rect r)
        {
            this.selection.Clear();
            foreach (Node node in this.m_Graph.nodes)
            {
                Rect position = node.position;
                if (((position.xMax >= r.x) && (position.x <= r.xMax)) && ((position.yMax >= r.y) && (position.y <= r.yMax)))
                {
                    this.selection.Add(node);
                }
            }
            foreach (UnityEditor.Graphs.Edge edge in this.m_Graph.edges)
            {
                if (this.selection.Contains(edge.fromSlot.node) && this.selection.Contains(edge.toSlot.node))
                {
                    this.edgeGUI.edgeSelection.Add(this.m_Graph.edges.IndexOf(edge));
                }
            }
        }

        protected void ShowContextMenu()
        {
            if (((Event.current.type == EventType.MouseDown) && (Event.current.button == 1)) && (Event.current.clickCount == 1))
            {
                Event.current.Use();
                Vector2 mousePosition = Event.current.mousePosition;
                Rect position = new Rect(mousePosition.x, mousePosition.y, 0f, 0f);
                List<GUIContent> list = new List<GUIContent>();
                if (this.selection.Count != 0)
                {
                    list.Add(new GUIContent("Cut"));
                    list.Add(new GUIContent("Copy"));
                    list.Add(new GUIContent("Duplicate"));
                    list.Add(new GUIContent(string.Empty));
                    list.Add(new GUIContent("Delete"));
                }
                else
                {
                    list.Add((this.edgeGUI.edgeSelection.Count != 0) ? new GUIContent("Delete") : new GUIContent("Paste"));
                }
                GUIContent[] options = list.ToArray();
                ContextMenuData userData = new ContextMenuData {
                    items = list.ToArray(),
                    mousePosition = mousePosition
                };
                this.m_contextMenuMouseDownPosition = null;
                EditorUtility.DisplayCustomMenu(position, options, -1, new EditorUtility.SelectMenuItemFunction(this.ContextMenuClick), userData);
            }
        }

        public void Slot(Rect position, string title, UnityEditor.Graphs.Slot s, bool allowStartDrag, bool allowEndDrag, bool allowMultiple, GUIStyle style)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            this.DoSlot(controlID, position, title, s, allowStartDrag, allowEndDrag, allowMultiple, style);
        }

        protected static Rect SnapPositionToGrid(Rect position)
        {
            int num = Mathf.RoundToInt(position.x / 12f);
            int num2 = Mathf.RoundToInt(position.y / 12f);
            position.x = num * 12f;
            position.y = num2 * 12f;
            return position;
        }

        public virtual void SyncGraphToUnitySelection()
        {
        }

        private void UpdateGraphExtents()
        {
            this.graph.graphExtents = GUILayoutUtility.GetWindowsBounds();
            this.graph.graphExtents.xMin -= this.m_Host.position.width * 0.6f;
            this.graph.graphExtents.xMax += this.m_Host.position.width * 0.6f;
            this.graph.graphExtents.yMin -= this.m_Host.position.height * 0.6f;
            this.graph.graphExtents.yMax += this.m_Host.position.height * 0.6f;
        }

        private void UpdateScrollPosition()
        {
            this.m_ScrollPosition.x += this.m_LastGraphExtents.x - this.graph.graphExtents.x;
            this.m_ScrollPosition.y += this.m_LastGraphExtents.y - this.graph.graphExtents.y;
            this.m_LastGraphExtents = this.graph.graphExtents;
            if (this.m_CenterGraph && (Event.current.type == EventType.Layout))
            {
                this.m_ScrollPosition = new Vector2((this.graph.graphExtents.width / 2f) - (this.m_Host.position.width / 2f), (this.graph.graphExtents.height / 2f) - (this.m_Host.position.height / 2f));
                this.m_CenterGraph = false;
            }
        }

        protected virtual void UpdateUnitySelection()
        {
            Selection.objects = new List<UnityEngine.Object>(this.selection.ToArray()).ToArray();
        }

        public void ZoomToGraph(Graph g)
        {
            throw new NotImplementedException();
        }

        public virtual IEdgeGUI edgeGUI
        {
            get
            {
                if (this.m_EdgeGUI == null)
                {
                    EdgeGUI egui = new EdgeGUI {
                        host = this
                    };
                    this.m_EdgeGUI = egui;
                }
                return this.m_EdgeGUI;
            }
        }

        public Graph graph
        {
            get => 
                this.m_Graph;
            set
            {
                this.m_Graph = value;
            }
        }

        private static UnityEngine.Color gridMajorColor
        {
            get
            {
                if (EditorGUIUtility.isProSkin)
                {
                    return kGridMajorColorDark;
                }
                return kGridMajorColorLight;
            }
        }

        private static UnityEngine.Color gridMinorColor
        {
            get
            {
                if (EditorGUIUtility.isProSkin)
                {
                    return kGridMinorColorDark;
                }
                return kGridMinorColorLight;
            }
        }

        [CompilerGenerated]
        private sealed class <OnGraphGUI>c__AnonStorey0
        {
            internal GraphGUI $this;
            internal Node n2;

            internal void <>m__0(int)
            {
                this.$this.NodeGUI(this.n2);
            }
        }

        private class ContextMenuData
        {
            public GUIContent[] items;
            public Vector2 mousePosition;
        }

        public class NodeTool
        {
            public string category;
            public GUIContent content;
            public CreateNodeFuncDelegate createNodeFunc;
            public bool visible;

            public NodeTool(string category, string title, CreateNodeFuncDelegate createNodeFunc)
            {
                this.createNodeFunc = createNodeFunc;
                this.category = category;
                this.content = new GUIContent(title);
                this.visible = false;
            }

            public delegate Node CreateNodeFuncDelegate();
        }

        private enum SelectionDragMode
        {
            None,
            Rect,
            Pick
        }
    }
}

