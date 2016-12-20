namespace Unity.IL2CPP.StackAnalysis
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class StackState
    {
        private Stack<Entry> _entries = new Stack<Entry>();

        public StackState Clone()
        {
            StackState state = new StackState();
            foreach (Entry entry in Enumerable.Reverse<Entry>(this._entries))
            {
                state.Entries.Push(entry.Clone());
            }
            return state;
        }

        public void Merge(StackState other)
        {
            List<Entry> collection = new List<Entry>(this._entries);
            List<Entry> list2 = new List<Entry>(other.Entries);
            while (collection.Count < list2.Count)
            {
                collection.Add(new Entry());
            }
            for (int i = 0; i < list2.Count; i++)
            {
                Entry entry = list2[i];
                Entry entry2 = collection[i];
                entry2.NullValue |= entry.NullValue;
                foreach (TypeReference reference in entry.Types)
                {
                    entry2.Types.Add(reference);
                }
            }
            collection.Reverse();
            this._entries = new Stack<Entry>(collection);
        }

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
                return (this._entries.Count == 0);
            }
        }
    }
}

