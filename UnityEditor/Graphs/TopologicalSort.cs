namespace UnityEditor.Graphs
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public sealed class TopologicalSort
    {
        private static Graph s_Graph;
        private static Dictionary<Node, NodeState> s_NodeStates;
        private static List<Node> s_SortedNodes;

        private TopologicalSort()
        {
        }

        public static void Sort(Graph g)
        {
            s_Graph = g;
            s_SortedNodes = new List<Node>();
            s_NodeStates = new Dictionary<Node, NodeState>();
            foreach (Node node in g.nodes)
            {
                Visit(node);
            }
        }

        private static void Visit(Node n)
        {
            if (!s_NodeStates.ContainsKey(n))
            {
                s_NodeStates[n] = NodeState.Visited;
                foreach (Edge edge in n.outputEdges)
                {
                    Visit(edge.toSlot.node);
                }
                s_SortedNodes.Add(n);
            }
        }

        public static IEnumerable<Node> deadNodes =>
            new <>c__Iterator0 { $PC=-2 };

        public static List<Node> SortedNodes =>
            s_SortedNodes;

        [CompilerGenerated]
        private sealed class <>c__Iterator0 : IEnumerable, IEnumerable<Node>, IEnumerator, IDisposable, IEnumerator<Node>
        {
            internal Node $current;
            internal bool $disposing;
            internal List<Node>.Enumerator $locvar0;
            internal int $PC;
            internal Node <n>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 1:
                        try
                        {
                        }
                        finally
                        {
                            this.$locvar0.Dispose();
                        }
                        break;
                }
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                        if (TopologicalSort.s_Graph != null)
                        {
                            this.$locvar0 = TopologicalSort.s_Graph.nodes.GetEnumerator();
                            num = 0xfffffffd;
                            break;
                        }
                        goto Label_00DF;

                    case 1:
                        break;

                    default:
                        goto Label_00DF;
                }
                try
                {
                    switch (num)
                    {
                        case 1:
                            goto Label_00AC;
                    }
                    while (this.$locvar0.MoveNext())
                    {
                        this.<n>__0 = this.$locvar0.Current;
                        if (!TopologicalSort.s_SortedNodes.Contains(this.<n>__0))
                        {
                            this.$current = this.<n>__0;
                            if (!this.$disposing)
                            {
                                this.$PC = 1;
                            }
                            flag = true;
                            return true;
                        }
                    Label_00AC:;
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    this.$locvar0.Dispose();
                }
                this.$PC = -1;
            Label_00DF:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<Node> IEnumerable<Node>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new TopologicalSort.<>c__Iterator0();
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<UnityEditor.Graphs.Node>.GetEnumerator();

            Node IEnumerator<Node>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        private enum NodeState
        {
            Visited
        }
    }
}

