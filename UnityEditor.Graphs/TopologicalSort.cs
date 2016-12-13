using System;
using System.Collections.Generic;

namespace UnityEditor.Graphs
{
	public sealed class TopologicalSort
	{
		private enum NodeState
		{
			Visited
		}

		private static List<Node> s_SortedNodes;

		private static Graph s_Graph;

		private static Dictionary<Node, TopologicalSort.NodeState> s_NodeStates;

		public static List<Node> SortedNodes
		{
			get
			{
				return TopologicalSort.s_SortedNodes;
			}
		}

		public static IEnumerable<Node> deadNodes
		{
			get
			{
				TopologicalSort.<>c__Iterator0 <>c__Iterator = new TopologicalSort.<>c__Iterator0();
				TopologicalSort.<>c__Iterator0 expr_07 = <>c__Iterator;
				expr_07.$PC = -2;
				return expr_07;
			}
		}

		private TopologicalSort()
		{
		}

		private static void Visit(Node n)
		{
			if (!TopologicalSort.s_NodeStates.ContainsKey(n))
			{
				TopologicalSort.s_NodeStates[n] = TopologicalSort.NodeState.Visited;
				foreach (Edge current in n.outputEdges)
				{
					TopologicalSort.Visit(current.toSlot.node);
				}
				TopologicalSort.s_SortedNodes.Add(n);
			}
		}

		public static void Sort(Graph g)
		{
			TopologicalSort.s_Graph = g;
			TopologicalSort.s_SortedNodes = new List<Node>();
			TopologicalSort.s_NodeStates = new Dictionary<Node, TopologicalSort.NodeState>();
			foreach (Node current in g.nodes)
			{
				TopologicalSort.Visit(current);
			}
		}
	}
}
