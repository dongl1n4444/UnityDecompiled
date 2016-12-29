namespace UnityEditor.iOS.Xcode.PBX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal class GUIDList : IEnumerable<string>, IEnumerable
    {
        private List<string> m_List;

        public GUIDList()
        {
            this.m_List = new List<string>();
        }

        public GUIDList(List<string> data)
        {
            this.m_List = new List<string>();
            this.m_List = data;
        }

        public void AddGUID(string guid)
        {
            this.m_List.Add(guid);
        }

        public void Clear()
        {
            this.m_List.Clear();
        }

        public bool Contains(string guid) => 
            this.m_List.Contains(guid);

        public static implicit operator GUIDList(List<string> data) => 
            new GUIDList(data);

        public static implicit operator List<string>(GUIDList list) => 
            list.m_List;

        public void RemoveGUID(string guid)
        {
            <RemoveGUID>c__AnonStorey0 storey = new <RemoveGUID>c__AnonStorey0 {
                guid = guid
            };
            this.m_List.RemoveAll(new Predicate<string>(storey.<>m__0));
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator() => 
            this.m_List.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            this.m_List.GetEnumerator();

        public int Count =>
            this.m_List.Count;

        [CompilerGenerated]
        private sealed class <RemoveGUID>c__AnonStorey0
        {
            internal string guid;

            internal bool <>m__0(string x) => 
                (x == this.guid);
        }
    }
}

