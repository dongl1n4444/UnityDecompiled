namespace UnityEditor.Graphs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [Serializable]
    public sealed class Edge
    {
        [SerializeField]
        public UnityEngine.Color color = UnityEngine.Color.white;
        [SerializeField]
        private Node m_FromNode;
        [NonSerialized]
        private Slot m_FromSlot;
        [SerializeField]
        private string m_FromSlotName;
        [SerializeField]
        private Node m_ToNode;
        [NonSerialized]
        private Slot m_ToSlot;
        [SerializeField]
        private string m_ToSlotName;

        public Edge(Slot fromSlot, Slot toSlot)
        {
            this.fromSlot = fromSlot;
            this.toSlot = toSlot;
        }

        private static Slot FindSlotByName(IEnumerable<Slot> slots, string name)
        {
            <FindSlotByName>c__AnonStorey0 storey = new <FindSlotByName>c__AnonStorey0 {
                name = name
            };
            return Enumerable.FirstOrDefault<Slot>(slots, new Func<Slot, bool>(storey.<>m__0));
        }

        internal bool NodesNotNull()
        {
            if (this.m_FromNode == null)
            {
                Debug.LogError("Edge.fromNode is null");
                return false;
            }
            if (this.m_ToNode == null)
            {
                Debug.LogError("Edge.toNode is null");
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            object[] objArray1 = new object[] { this.fromSlot.node.title, "[", this.fromSlot, "]-->", this.toSlot.node.title, "[", this.toSlot, "]" };
            return string.Concat(objArray1);
        }

        internal bool WakeUp()
        {
            this.m_FromSlot = FindSlotByName(this.m_FromNode.outputSlots, this.m_FromSlotName);
            if (this.m_FromSlot == null)
            {
                return false;
            }
            this.m_ToSlot = FindSlotByName(this.m_ToNode.inputSlots, this.m_ToSlotName);
            if (this.m_ToSlot == null)
            {
                return false;
            }
            if (!this.m_FromNode.graph.CanConnect(this.m_FromSlot, this.m_ToSlot))
            {
                return false;
            }
            this.m_ToSlot.AddEdge(this);
            this.m_FromSlot.AddEdge(this);
            return true;
        }

        public Slot fromSlot
        {
            get => 
                this.m_FromSlot;
            set
            {
                if (this.m_FromSlot != null)
                {
                    this.m_FromSlot.RemoveEdge(this);
                }
                this.m_FromSlot = value;
                if (value != null)
                {
                    this.m_FromNode = value.node;
                    this.m_FromSlotName = value.name;
                    value.AddEdge(this);
                }
            }
        }

        public string fromSlotName
        {
            get => 
                this.m_FromSlotName;
            set
            {
                this.m_FromSlotName = value;
            }
        }

        public Slot toSlot
        {
            get => 
                this.m_ToSlot;
            set
            {
                if (this.m_ToSlot != null)
                {
                    this.m_ToSlot.RemoveEdge(this);
                }
                this.m_ToSlot = value;
                if (value != null)
                {
                    this.m_ToNode = value.node;
                    this.m_ToSlotName = value.name;
                    value.AddEdge(this);
                }
            }
        }

        public string toSlotName
        {
            get => 
                this.m_ToSlotName;
            set
            {
                this.m_ToSlotName = value;
            }
        }

        [CompilerGenerated]
        private sealed class <FindSlotByName>c__AnonStorey0
        {
            internal string name;

            internal bool <>m__0(Slot s) => 
                (s.name == this.name);
        }
    }
}

