namespace UnityEditor.Graphs
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class GroupNode : Node
    {
        [CompilerGenerated]
        private static Predicate<Node> <>f__am$cache0;
        [CompilerGenerated]
        private static Predicate<UnityEditor.Graphs.Edge> <>f__am$cache1;
        [CompilerGenerated]
        private static Predicate<Node> <>f__am$cache2;
        [CompilerGenerated]
        private static Predicate<Node> <>f__am$cache3;
        [NonSerialized]
        private ProxyNode m_ProxyInNode;
        [NonSerialized]
        private ProxyNode m_ProxyOutNode;
        [SerializeField]
        private Graph m_SubGraph;

        public GroupNode()
        {
        }

        private GroupNode(string name, Graph parentGraph, System.Type graphType)
        {
            this.m_SubGraph = (Graph) Activator.CreateInstance(graphType);
            base.name = name;
            this.m_SubGraph.name = "SubGraph";
        }

        public void AddChildNode(Node node)
        {
            this.m_SubGraph.AddNode(node, false);
        }

        public void DestroyChildNode(Node node)
        {
            this.m_SubGraph.DestroyNode(node);
        }

        public static GroupNode FromNodes(string name, List<Node> nodes, System.Type graphType)
        {
            if (nodes.Count == 0)
            {
                throw new ArgumentException("No nodes to group");
            }
            Graph parentGraph = nodes[0].graph;
            if (parentGraph == null)
            {
                throw new ArgumentException("Nodes needs to be attached to a graph");
            }
            GroupNode node = new GroupNode(name, parentGraph, graphType);
            parentGraph.AddNode(node);
            node.m_ProxyInNode = ProxyNode.Instance(true);
            node.subGraph.AddNode(node.m_ProxyInNode);
            node.m_ProxyOutNode = ProxyNode.Instance(false);
            node.subGraph.AddNode(node.m_ProxyOutNode);
            List<UnityEditor.Graphs.Edge> list = new List<UnityEditor.Graphs.Edge>();
            foreach (Node node2 in nodes)
            {
                list.AddRange(node2.outputEdges);
                list.AddRange(node2.inputEdges);
                node.AddChildNode(node2);
                parentGraph.nodes.Remove(node2);
            }
            foreach (UnityEditor.Graphs.Edge edge in list)
            {
                if ((edge.fromSlot.node.graph == node.subGraph) && (edge.toSlot.node.graph == node.subGraph))
                {
                    if (!node.subGraph.Connected(edge.fromSlot, edge.toSlot))
                    {
                        node.subGraph.Connect(edge.fromSlot, edge.toSlot);
                    }
                }
                else if ((edge.fromSlot.node.graph == node.subGraph) && (edge.toSlot.node.graph != node.subGraph))
                {
                    string str = edge.fromSlot.name;
                    int num = 0;
                    while (node.m_ProxyInNode[str] != null)
                    {
                        str = edge.fromSlot.name + "_" + num++;
                    }
                    Slot s = new Slot(SlotType.InputSlot, str);
                    node.m_ProxyInNode.AddSlot(s);
                    node.subGraph.Connect(edge.fromSlot, s);
                    Slot slot2 = new Slot(SlotType.OutputSlot, str);
                    node.AddSlot(slot2);
                    node.graph.Connect(slot2, edge.toSlot);
                }
                else if ((edge.fromSlot.node.graph != node.subGraph) && (edge.toSlot.node.graph == node.subGraph))
                {
                    string str2 = edge.toSlot.name;
                    int num2 = 0;
                    while (node.m_ProxyOutNode[str2] != null)
                    {
                        str2 = edge.toSlot.name + "_" + num2++;
                    }
                    Slot slot3 = new Slot(SlotType.OutputSlot, str2);
                    node.m_ProxyOutNode.AddSlot(slot3);
                    node.subGraph.Connect(slot3, edge.toSlot);
                    Slot slot4 = new Slot(SlotType.InputSlot, str2);
                    node.AddSlot(slot4);
                    node.graph.Connect(edge.fromSlot, slot4);
                }
                node.graph.RemoveEdge(edge);
            }
            return node;
        }

        public override void NodeUI(GraphGUI host)
        {
            GUILayout.Label("Internal Nodes:" + this.m_SubGraph.nodes.Count, new GUILayoutOption[0]);
            if (GUILayout.Button("Ungroup", new GUILayoutOption[0]))
            {
                this.UnGroup();
            }
            if (GUILayout.Button("Edit", new GUILayoutOption[0]))
            {
                host.ZoomToGraph(this.m_SubGraph);
            }
        }

        public override string ToString()
        {
            object[] objArray1 = new object[] { base.ToString(), " \"", this.m_SubGraph, "\" " };
            return string.Concat(objArray1);
        }

        public void UnGroup()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = n => !(n is ProxyNode);
            }
            List<Node> list = this.m_SubGraph.nodes.FindAll(<>f__am$cache0);
            foreach (Node node in list)
            {
                base.graph.AddNode(node, false);
                this.subGraph.nodes.Remove(node);
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = e => !(e.toSlot.node is ProxyNode) && !(e.fromSlot.node is ProxyNode);
            }
            List<UnityEditor.Graphs.Edge> list2 = this.m_SubGraph.edges.FindAll(<>f__am$cache1);
            foreach (UnityEditor.Graphs.Edge edge in list2)
            {
                base.graph.edges.Add(edge);
                this.subGraph.edges.Remove(edge);
            }
            List<UnityEditor.Graphs.Edge> list3 = new List<UnityEditor.Graphs.Edge>();
            List<UnityEditor.Graphs.Edge> list4 = new List<UnityEditor.Graphs.Edge>();
            foreach (Slot slot in base.inputSlots)
            {
                int num = 0;
                foreach (UnityEditor.Graphs.Edge edge2 in slot.edges)
                {
                    Slot slot2 = this.ProxyOutNode[slot.name];
                    UnityEditor.Graphs.Edge item = slot2.edges[num];
                    base.graph.Connect(edge2.fromSlot, item.toSlot);
                    list3.Add(edge2);
                    list4.Add(item);
                    num++;
                }
            }
            foreach (Slot slot3 in base.outputSlots)
            {
                int num2 = 0;
                foreach (UnityEditor.Graphs.Edge edge4 in slot3.edges)
                {
                    Slot slot4 = this.ProxyInNode[slot3.name];
                    UnityEditor.Graphs.Edge edge5 = slot4.edges[num2];
                    base.graph.Connect(edge5.fromSlot, edge4.toSlot);
                    list3.Add(edge4);
                    list4.Add(edge5);
                    num2++;
                }
            }
            foreach (UnityEditor.Graphs.Edge edge6 in list4)
            {
                this.subGraph.edges.Remove(edge6);
            }
            foreach (UnityEditor.Graphs.Edge edge7 in list3)
            {
                base.graph.edges.Remove(edge7);
            }
            this.subGraph.RemoveNode(this.ProxyInNode, false);
            this.subGraph.RemoveNode(this.ProxyOutNode, false);
            base.graph.RemoveNode(this, false);
        }

        internal override void WakeUp(Graph owner)
        {
            if (base.m_Slots == null)
            {
                base.m_Slots = new List<Slot>();
            }
            if (this.m_SubGraph != null)
            {
                this.m_SubGraph.WakeUp();
            }
            else
            {
                Debug.LogError("Subgraph is null????");
            }
            if (this.m_SubGraph != null)
            {
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = n => (n is ProxyNode) && ((ProxyNode) n).isIn;
                }
                this.m_ProxyInNode = (ProxyNode) this.m_SubGraph.nodes.Find(<>f__am$cache2);
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = n => (n is ProxyNode) && !((ProxyNode) n).isIn;
                }
                this.m_ProxyOutNode = (ProxyNode) this.m_SubGraph.nodes.Find(<>f__am$cache3);
            }
            base.WakeUp(owner);
        }

        internal ProxyNode ProxyInNode =>
            this.m_ProxyInNode;

        internal ProxyNode ProxyOutNode =>
            this.m_ProxyOutNode;

        public Graph subGraph =>
            this.m_SubGraph;
    }
}

