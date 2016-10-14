using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.IL2CPP.StackAnalysis
{
	public class StackState
	{
		private Stack<Entry> _entries = new Stack<Entry>();

		public Stack<Entry> Entries
		{
			get
			{
				return this._entries;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this._entries.Count == 0;
			}
		}

		public void Merge(StackState other)
		{
			List<Entry> list = new List<Entry>(this._entries);
			List<Entry> list2 = new List<Entry>(other.Entries);
			while (list.Count < list2.Count)
			{
				list.Add(new Entry());
			}
			for (int i = 0; i < list2.Count; i++)
			{
				Entry entry = list2[i];
				Entry entry2 = list[i];
				entry2.NullValue |= entry.NullValue;
				foreach (TypeReference current in entry.Types)
				{
					entry2.Types.Add(current);
				}
			}
			list.Reverse();
			this._entries = new Stack<Entry>(list);
		}

		public StackState Clone()
		{
			StackState stackState = new StackState();
			foreach (Entry current in this._entries.Reverse<Entry>())
			{
				stackState.Entries.Push(current.Clone());
			}
			return stackState;
		}
	}
}
