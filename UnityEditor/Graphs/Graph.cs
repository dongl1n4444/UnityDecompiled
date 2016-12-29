namespace UnityEditor.Graphs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;

    public class Graph : ScriptableObject
    {
        [CompilerGenerated]
        private static Func<Node, int> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<Node, IEnumerable<Slot>> <>f__am$cache1;
        [SerializeField]
        public List<UnityEditor.Graphs.Edge> edges = new List<UnityEditor.Graphs.Edge>();
        [NonSerialized]
        internal Rect graphExtents;
        [NonSerialized]
        private List<Slot> m_changingOutputSlotTypesCycleSlots = new List<Slot>();
        [NonSerialized]
        private bool m_ImAwake;
        [SerializeField]
        internal List<UnityEditor.Graphs.Edge> m_InvalidEdges = new List<UnityEditor.Graphs.Edge>();
        [SerializeField]
        public List<Node> nodes = new List<Node>();

        public virtual void AddNode(Node node)
        {
            this.AddNode(node, true);
            this.Dirty();
        }

        internal virtual void AddNode(Node node, bool serialize)
        {
            this.nodes.Add(node);
            node.graph = this;
            this.Dirty();
            node.AddedToGraph();
        }

        public virtual void AddNodes(params Node[] nodes)
        {
            foreach (Node node in nodes)
            {
                this.AddNode(node);
            }
        }

        public virtual bool CanConnect(Slot fromSlot, Slot toSlot) => 
            true;

        public virtual void Clear(bool destroyNodes = false)
        {
            foreach (Node node in this.nodes)
            {
                node.RemovingFromGraph();
                this.RemoveEdgesFromNode(node);
                if (destroyNodes)
                {
                    UnityEngine.Object.DestroyImmediate(node, true);
                }
            }
            this.nodes.Clear();
            this.Dirty();
        }

        public virtual UnityEditor.Graphs.Edge Connect(Slot fromSlot, Slot toSlot)
        {
            <Connect>c__AnonStorey1 storey = new <Connect>c__AnonStorey1 {
                fromSlot = fromSlot,
                toSlot = toSlot
            };
            bool flag = this.m_changingOutputSlotTypesCycleSlots.Count == 0;
            if (this.m_changingOutputSlotTypesCycleSlots.Contains(storey.toSlot))
            {
                storey.toSlot.edges.RemoveAll(new Predicate<UnityEditor.Graphs.Edge>(storey.<>m__0));
                storey.fromSlot.edges.RemoveAll(new Predicate<UnityEditor.Graphs.Edge>(storey.<>m__1));
                this.edges.RemoveAll(new Predicate<UnityEditor.Graphs.Edge>(storey.<>m__2));
                throw new ArgumentException("Connecting node data slots this way creates infinite cycle of changing node types");
            }
            this.m_changingOutputSlotTypesCycleSlots.Add(storey.toSlot);
            if ((storey.fromSlot == null) || (storey.toSlot == null))
            {
                throw new ArgumentException("to/from slot can't be null");
            }
            if (this.Connected(storey.fromSlot, storey.toSlot))
            {
                throw new ArgumentException("Already connected");
            }
            UnityEditor.Graphs.Edge item = new UnityEditor.Graphs.Edge(storey.fromSlot, storey.toSlot);
            this.edges.Add(item);
            SetGenericPropertyArgumentType(storey.toSlot, storey.fromSlot.dataType);
            this.Dirty();
            storey.toSlot.node.InputEdgeChanged(item);
            if (flag)
            {
                this.m_changingOutputSlotTypesCycleSlots.Clear();
            }
            return item;
        }

        public virtual bool Connected(Slot fromSlot, Slot toSlot)
        {
            <Connected>c__AnonStorey0 storey = new <Connected>c__AnonStorey0 {
                fromSlot = fromSlot,
                toSlot = toSlot
            };
            return this.edges.Exists(new Predicate<UnityEditor.Graphs.Edge>(storey.<>m__0));
        }

        public virtual void DestroyNode(Node node)
        {
            this.RemoveNode(node, true);
        }

        public virtual void Dirty()
        {
            EditorUtility.SetDirty(this);
        }

        private void DoWakeUpEdges(List<UnityEditor.Graphs.Edge> inEdges, List<UnityEditor.Graphs.Edge> ok, List<UnityEditor.Graphs.Edge> error, bool inEdgesUsedToBeValid)
        {
            foreach (UnityEditor.Graphs.Edge edge in inEdges)
            {
                if (edge != null)
                {
                    if (edge.NodesNotNull())
                    {
                        if (edge.WakeUp())
                        {
                            ok.Add(edge);
                            if (!inEdgesUsedToBeValid)
                            {
                                this.Dirty();
                            }
                        }
                        else
                        {
                            error.Add(edge);
                            if (inEdgesUsedToBeValid)
                            {
                                this.Dirty();
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogError("Edge is null?");
                }
            }
        }

        public static Graph FlattenedCopy(Graph source)
        {
            Dictionary<Node, Node> dictionary = new Dictionary<Node, Node>();
            Graph graph = (Graph) Activator.CreateInstance(source.GetType());
            foreach (Node node in source.nodes)
            {
                Node dest = (Node) Activator.CreateInstance(node.GetType());
                EditorUtility.CopySerialized(node, dest);
                dictionary.Add(node, dest);
                graph.AddNode(dest);
            }
            graph.OnEnable();
            foreach (UnityEditor.Graphs.Edge edge in source.edges)
            {
                Node node3 = edge.fromSlot.node;
                Node node4 = edge.toSlot.node;
                node3 = dictionary[node3];
                node4 = dictionary[node4];
                Slot fromSlot = node3[edge.fromSlot.name];
                Slot toSlot = node4[edge.toSlot.name];
                graph.Connect(fromSlot, toSlot);
            }
            return graph;
        }

        internal virtual GraphGUI GetEditor()
        {
            GraphGUI hgui = ScriptableObject.CreateInstance<GraphGUI>();
            hgui.graph = this;
            return hgui;
        }

        public Node GetNodeByName(string name)
        {
            foreach (Node node in this.nodes)
            {
                if (node.name == name)
                {
                    return node;
                }
            }
            return null;
        }

        internal static int[] GetNodeIdsForSerialization(Graph graph)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<Node, int>(null, (IntPtr) <GetNodeIdsForSerialization>m__0);
            }
            return Enumerable.Select<Node, int>(graph.nodes, <>f__am$cache0).ToArray<int>();
        }

        public virtual void OnEnable()
        {
            this.WakeUp();
        }

        public void RedirectSlotEdges(Node node, string oldSlotName, string newSlotName)
        {
            foreach (UnityEditor.Graphs.Edge edge in this.edges)
            {
                if (edge.fromSlotName == oldSlotName)
                {
                    edge.fromSlotName = newSlotName;
                }
                if (edge.toSlotName == oldSlotName)
                {
                    edge.toSlotName = newSlotName;
                }
            }
        }

        public virtual void RemoveEdge(UnityEditor.Graphs.Edge e)
        {
            this.edges.Remove(e);
            if (e.fromSlot != null)
            {
                e.fromSlot.RemoveEdge(e);
            }
            if (e.toSlot != null)
            {
                e.toSlot.RemoveEdge(e);
                e.toSlot.node.InputEdgeChanged(e);
            }
            ResetGenericPropertyArgumentType(e.toSlot);
            this.Dirty();
        }

        private void RemoveEdgesFromNode(Node node)
        {
            List<UnityEditor.Graphs.Edge> list = new List<UnityEditor.Graphs.Edge>();
            foreach (UnityEditor.Graphs.Edge edge in node.inputEdges)
            {
                list.Add(edge);
            }
            foreach (UnityEditor.Graphs.Edge edge2 in node.outputEdges)
            {
                list.Add(edge2);
            }
            foreach (UnityEditor.Graphs.Edge edge3 in list)
            {
                this.RemoveEdge(edge3);
            }
        }

        public void RemoveInvalidEdgesForSlot(Slot slot)
        {
            <RemoveInvalidEdgesForSlot>c__AnonStorey4 storey = new <RemoveInvalidEdgesForSlot>c__AnonStorey4 {
                slot = slot
            };
            this.m_InvalidEdges.RemoveAll(new Predicate<UnityEditor.Graphs.Edge>(storey.<>m__0));
        }

        public virtual void RemoveNode(Node node, bool destroyNode = false)
        {
            if (node == null)
            {
                throw new ArgumentNullException("Node is null");
            }
            node.RemovingFromGraph();
            this.RemoveEdgesFromNode(node);
            this.nodes.Remove(node);
            if (destroyNode)
            {
                UnityEngine.Object.DestroyImmediate(node, true);
            }
            this.Dirty();
        }

        public virtual void RemoveNodes(List<Node> nodesToRemove, bool destroyNodes = false)
        {
            foreach (Node node in nodesToRemove)
            {
                this.RemoveNode(node, destroyNodes);
            }
        }

        private static void ResetGenericPropertyArgumentType(Slot toSlot)
        {
            if ((toSlot != null) && ((toSlot.isInputDataSlot && (toSlot.node.genericType != null)) && (toSlot.node.isGeneric && (toSlot.node.inputDataSlots.First<Slot>() == toSlot))))
            {
                toSlot.node.ResetGenericPropertyArgumentType();
            }
        }

        public void RevalidateInputDataEdges(Slot s)
        {
            <RevalidateInputDataEdges>c__AnonStorey2 storey = new <RevalidateInputDataEdges>c__AnonStorey2 {
                s = s
            };
            if (!storey.s.isDataSlot || (storey.s.type != SlotType.InputSlot))
            {
                throw new ArgumentException("Expected an input data slot");
            }
            if (storey.s.edges.Count<UnityEditor.Graphs.Edge>() > 1)
            {
                throw new ArgumentException("Got input data slot with multiple input Edges. This should never happen.");
            }
            if (storey.s.edges.Count<UnityEditor.Graphs.Edge>() == 1)
            {
                UnityEditor.Graphs.Edge item = storey.s.edges.First<UnityEditor.Graphs.Edge>();
                item.fromSlot.edges.Remove(item);
                storey.s.edges.Clear();
                this.edges.Remove(item);
                if (this.CanConnect(item.fromSlot, item.toSlot))
                {
                    if (this.m_changingOutputSlotTypesCycleSlots.Contains(storey.s))
                    {
                        this.m_changingOutputSlotTypesCycleSlots.Remove(storey.s);
                    }
                    this.Connect(item.fromSlot, storey.s);
                }
                else
                {
                    this.m_InvalidEdges.Add(item);
                    item.toSlot.node.InputEdgeChanged(item);
                }
            }
            else
            {
                foreach (UnityEditor.Graphs.Edge edge2 in Enumerable.Where<UnityEditor.Graphs.Edge>(this.m_InvalidEdges, new Func<UnityEditor.Graphs.Edge, bool>(storey, (IntPtr) this.<>m__0)))
                {
                    if (this.CanConnect(edge2.fromSlot, edge2.toSlot))
                    {
                        this.Connect(edge2.fromSlot, edge2.toSlot);
                        this.m_InvalidEdges.Remove(edge2);
                        break;
                    }
                }
            }
        }

        public void RevalidateOutputDataEdges(Slot s)
        {
            <RevalidateOutputDataEdges>c__AnonStorey3 storey = new <RevalidateOutputDataEdges>c__AnonStorey3 {
                s = s
            };
            if (!storey.s.isDataSlot || (storey.s.type != SlotType.OutputSlot))
            {
                throw new ArgumentException("Expected an output data slot");
            }
            List<UnityEditor.Graphs.Edge> list = storey.s.edges.ToList<UnityEditor.Graphs.Edge>();
            List<UnityEditor.Graphs.Edge> list2 = Enumerable.Where<UnityEditor.Graphs.Edge>(this.m_InvalidEdges, new Func<UnityEditor.Graphs.Edge, bool>(storey, (IntPtr) this.<>m__0)).ToList<UnityEditor.Graphs.Edge>();
            foreach (UnityEditor.Graphs.Edge edge in storey.s.edges)
            {
                edge.toSlot.edges.RemoveAll(new Predicate<UnityEditor.Graphs.Edge>(storey.<>m__1));
            }
            storey.s.edges.Clear();
            this.edges.RemoveAll(new Predicate<UnityEditor.Graphs.Edge>(storey.<>m__2));
            this.m_InvalidEdges.RemoveAll(new Predicate<UnityEditor.Graphs.Edge>(storey.<>m__3));
            foreach (UnityEditor.Graphs.Edge edge2 in list)
            {
                if (this.CanConnect(edge2.fromSlot, edge2.toSlot))
                {
                    this.Connect(edge2.fromSlot, edge2.toSlot);
                }
                else
                {
                    this.m_InvalidEdges.Add(edge2);
                    edge2.toSlot.node.InputEdgeChanged(edge2);
                }
            }
            foreach (UnityEditor.Graphs.Edge edge3 in list2)
            {
                if (this.CanConnect(edge3.fromSlot, edge3.toSlot))
                {
                    this.Connect(edge3.fromSlot, edge3.toSlot);
                }
                else
                {
                    this.m_InvalidEdges.Add(edge3);
                }
            }
        }

        private static void SetGenericPropertyArgumentType(Slot toSlot, System.Type fromSlotType)
        {
            if ((toSlot.isInputDataSlot && (toSlot.node.genericType == null)) && (toSlot.node.isGeneric && (toSlot.node.inputDataSlots.First<Slot>() == toSlot)))
            {
                toSlot.node.SetGenericPropertyArgumentType(SerializedType.GenericType(fromSlotType));
            }
        }

        public override string ToString()
        {
            string str = "[";
            foreach (Node node in this.nodes)
            {
                str = str + node + "|";
            }
            str = str + "];[";
            foreach (UnityEditor.Graphs.Edge edge in this.edges)
            {
                if (edge != null)
                {
                    str = str + edge + "|";
                }
            }
            return (str + "]");
        }

        public void WakeUp()
        {
            this.WakeUp(false);
        }

        public virtual void WakeUp(bool force)
        {
            if (force || !this.m_ImAwake)
            {
                for (int i = this.nodes.Count - 1; i >= 0; i--)
                {
                    if (this.nodes[i] == null)
                    {
                        Debug.LogError("Removing null node");
                        this.nodes.RemoveAt(i);
                    }
                }
                foreach (Node node in this.nodes)
                {
                    node.Awake();
                }
                this.WakeUpNodes();
                if (this.edges != null)
                {
                    this.WakeUpEdges(false);
                }
                else
                {
                    Debug.LogError("Edges are null?");
                }
                this.m_ImAwake = true;
            }
        }

        public void WakeUpEdges(bool clearSlotEdges)
        {
            if (clearSlotEdges)
            {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = new Func<Node, IEnumerable<Slot>>(null, (IntPtr) <WakeUpEdges>m__1);
                }
                foreach (Slot slot in Enumerable.SelectMany<Node, Slot>(this.nodes, <>f__am$cache1))
                {
                    slot.edges.Clear();
                }
            }
            List<UnityEditor.Graphs.Edge> ok = new List<UnityEditor.Graphs.Edge>();
            List<UnityEditor.Graphs.Edge> error = new List<UnityEditor.Graphs.Edge>();
            this.DoWakeUpEdges(this.edges, ok, error, true);
            this.DoWakeUpEdges(this.m_InvalidEdges, ok, error, false);
            this.edges = ok;
            this.m_InvalidEdges = error;
        }

        protected virtual void WakeUpNodes()
        {
            foreach (Node node in this.nodes)
            {
                node.WakeUp(this);
            }
        }

        protected bool isAwake =>
            this.m_ImAwake;

        public Node this[string name] =>
            this.GetNodeByName(name);

        [CompilerGenerated]
        private sealed class <Connect>c__AnonStorey1
        {
            internal Slot fromSlot;
            internal Slot toSlot;

            internal bool <>m__0(UnityEditor.Graphs.Edge edg) => 
                (edg.fromSlot == this.fromSlot);

            internal bool <>m__1(UnityEditor.Graphs.Edge edg) => 
                (edg.toSlot == this.toSlot);

            internal bool <>m__2(UnityEditor.Graphs.Edge edg) => 
                ((edg.fromSlot == this.fromSlot) && (edg.toSlot == this.toSlot));
        }

        [CompilerGenerated]
        private sealed class <Connected>c__AnonStorey0
        {
            internal Slot fromSlot;
            internal Slot toSlot;

            internal bool <>m__0(UnityEditor.Graphs.Edge e) => 
                ((e.fromSlot == this.fromSlot) && (e.toSlot == this.toSlot));
        }

        [CompilerGenerated]
        private sealed class <RemoveInvalidEdgesForSlot>c__AnonStorey4
        {
            internal Slot slot;

            internal bool <>m__0(UnityEditor.Graphs.Edge e) => 
                ((e.fromSlot == this.slot) || (e.toSlot == this.slot));
        }

        [CompilerGenerated]
        private sealed class <RevalidateInputDataEdges>c__AnonStorey2
        {
            internal Slot s;

            internal bool <>m__0(UnityEditor.Graphs.Edge e) => 
                (e.toSlot == this.s);
        }

        [CompilerGenerated]
        private sealed class <RevalidateOutputDataEdges>c__AnonStorey3
        {
            internal Slot s;

            internal bool <>m__0(UnityEditor.Graphs.Edge e) => 
                (e.fromSlot == this.s);

            internal bool <>m__1(UnityEditor.Graphs.Edge edg) => 
                (edg.fromSlot == this.s);

            internal bool <>m__2(UnityEditor.Graphs.Edge e) => 
                (e.fromSlot == this.s);

            internal bool <>m__3(UnityEditor.Graphs.Edge e) => 
                (e.fromSlot == this.s);
        }
    }
}

