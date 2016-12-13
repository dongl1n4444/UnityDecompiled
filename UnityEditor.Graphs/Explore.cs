using System;
using System.Collections.Generic;
using System.Threading;

namespace UnityEditor.Graphs
{
	public sealed class Explore
	{
		private enum NodeState
		{
			White,
			Grey,
			Black
		}

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

		public enum SearchDirection
		{
			Forward,
			Backward
		}

		private static Dictionary<Node, Explore.NodeState> m_NodeStates = new Dictionary<Node, Explore.NodeState>();

		public static event Explore.SearchHandler OnDiscoverNode
		{
			add
			{
				Explore.SearchHandler searchHandler = Explore.OnDiscoverNode;
				Explore.SearchHandler searchHandler2;
				do
				{
					searchHandler2 = searchHandler;
					searchHandler = Interlocked.CompareExchange<Explore.SearchHandler>(ref Explore.OnDiscoverNode, (Explore.SearchHandler)Delegate.Combine(searchHandler2, value), searchHandler);
				}
				while (searchHandler != searchHandler2);
			}
			remove
			{
				Explore.SearchHandler searchHandler = Explore.OnDiscoverNode;
				Explore.SearchHandler searchHandler2;
				do
				{
					searchHandler2 = searchHandler;
					searchHandler = Interlocked.CompareExchange<Explore.SearchHandler>(ref Explore.OnDiscoverNode, (Explore.SearchHandler)Delegate.Remove(searchHandler2, value), searchHandler);
				}
				while (searchHandler != searchHandler2);
			}
		}

		public static event Explore.SearchHandler OnDiscoverEdge
		{
			add
			{
				Explore.SearchHandler searchHandler = Explore.OnDiscoverEdge;
				Explore.SearchHandler searchHandler2;
				do
				{
					searchHandler2 = searchHandler;
					searchHandler = Interlocked.CompareExchange<Explore.SearchHandler>(ref Explore.OnDiscoverEdge, (Explore.SearchHandler)Delegate.Combine(searchHandler2, value), searchHandler);
				}
				while (searchHandler != searchHandler2);
			}
			remove
			{
				Explore.SearchHandler searchHandler = Explore.OnDiscoverEdge;
				Explore.SearchHandler searchHandler2;
				do
				{
					searchHandler2 = searchHandler;
					searchHandler = Interlocked.CompareExchange<Explore.SearchHandler>(ref Explore.OnDiscoverEdge, (Explore.SearchHandler)Delegate.Remove(searchHandler2, value), searchHandler);
				}
				while (searchHandler != searchHandler2);
			}
		}

		private Explore()
		{
		}

		public static void Traverse(Node v, Explore.SearchDirection direction)
		{
			Explore.m_NodeStates.Clear();
			Queue<Node> queue = new Queue<Node>();
			queue.Enqueue(v);
			Explore.m_NodeStates[v] = Explore.NodeState.Grey;
			while (queue.Count != 0)
			{
				Node node = queue.Dequeue();
				foreach (Edge current in ((direction != Explore.SearchDirection.Forward) ? node.inputEdges : node.outputEdges))
				{
					if (Explore.OnDiscoverEdge != null)
					{
						Explore.OnDiscoverEdge(new Explore.SearchEvent(node, current));
					}
					Node node2 = (direction != Explore.SearchDirection.Forward) ? current.fromSlot.node : current.toSlot.node;
					if (!Explore.m_NodeStates.ContainsKey(node2))
					{
						if (Explore.OnDiscoverNode != null)
						{
							Explore.OnDiscoverNode(new Explore.SearchEvent(node2, current));
						}
						queue.Enqueue(node2);
						Explore.m_NodeStates[node2] = Explore.NodeState.Grey;
					}
				}
				Explore.m_NodeStates[node] = Explore.NodeState.Black;
			}
		}
	}
}
