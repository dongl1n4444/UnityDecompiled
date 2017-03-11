namespace UnityEditor.Graphs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    public sealed class Explore
    {
        private static Dictionary<Node, NodeState> m_NodeStates = new Dictionary<Node, NodeState>();

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public static  event SearchHandler OnDiscoverEdge;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public static  event SearchHandler OnDiscoverNode;

        private Explore()
        {
        }

        public static void Traverse(Node v, SearchDirection direction)
        {
            m_NodeStates.Clear();
            Queue<Node> queue = new Queue<Node>();
            queue.Enqueue(v);
            m_NodeStates[v] = NodeState.Grey;
            while (queue.Count != 0)
            {
                Node n = queue.Dequeue();
                foreach (Edge edge in (direction != SearchDirection.Forward) ? n.inputEdges : n.outputEdges)
                {
                    if (OnDiscoverEdge != null)
                    {
                        OnDiscoverEdge(new SearchEvent(n, edge));
                    }
                    Node key = (direction != SearchDirection.Forward) ? edge.fromSlot.node : edge.toSlot.node;
                    if (!m_NodeStates.ContainsKey(key))
                    {
                        if (OnDiscoverNode != null)
                        {
                            OnDiscoverNode(new SearchEvent(key, edge));
                        }
                        queue.Enqueue(key);
                        m_NodeStates[key] = NodeState.Grey;
                    }
                }
                m_NodeStates[n] = NodeState.Black;
            }
        }

        private enum NodeState
        {
            White,
            Grey,
            Black
        }

        public enum SearchDirection
        {
            Forward,
            Backward
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SearchEvent
        {
            public Node node;
            public Edge edge;
            public SearchEvent(Node n, Edge e)
            {
                this.node = n;
                this.edge = e;
            }
        }

        public delegate void SearchHandler(Explore.SearchEvent e);
    }
}

