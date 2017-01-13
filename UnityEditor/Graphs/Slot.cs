namespace UnityEditor.Graphs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    public class Slot
    {
        [NonSerialized]
        public List<UnityEditor.Graphs.Edge> edges;
        [SerializeField]
        private string m_DataTypeString;
        [SerializeField]
        private string m_Name;
        [NonSerialized]
        private Node m_Node;
        [NonSerialized]
        internal Rect m_Position;
        [SerializeField]
        private string m_Title;
        public SlotType type;

        public Slot()
        {
            this.m_Title = string.Empty;
            this.m_Name = string.Empty;
            this.m_DataTypeString = string.Empty;
            this.Init();
        }

        public Slot(SlotType type)
        {
            this.m_Title = string.Empty;
            this.m_Name = string.Empty;
            this.m_DataTypeString = string.Empty;
            this.Init();
            this.type = type;
        }

        public Slot(SlotType type, string name)
        {
            this.m_Title = string.Empty;
            this.m_Name = string.Empty;
            this.m_DataTypeString = string.Empty;
            this.Init();
            this.name = name;
            this.type = type;
        }

        public Slot(SlotType type, string name, string title)
        {
            this.m_Title = string.Empty;
            this.m_Name = string.Empty;
            this.m_DataTypeString = string.Empty;
            this.Init();
            this.name = name;
            this.type = type;
            this.title = title;
        }

        public Slot(SlotType type, string name, System.Type dataType)
        {
            this.m_Title = string.Empty;
            this.m_Name = string.Empty;
            this.m_DataTypeString = string.Empty;
            this.Init();
            this.name = name;
            this.type = type;
            this.dataType = dataType;
        }

        public Slot(SlotType type, string name, string title, System.Type dataType)
        {
            this.m_Title = string.Empty;
            this.m_Name = string.Empty;
            this.m_DataTypeString = string.Empty;
            this.Init();
            this.name = name;
            this.type = type;
            this.title = title;
            this.dataType = dataType;
        }

        public void AddEdge(UnityEditor.Graphs.Edge e)
        {
            if (this.edges == null)
            {
                throw new NullReferenceException("Error - edges are null?");
            }
            this.edges.Add(e);
        }

        private static string GetNiceTitle(string name)
        {
            if (name != null)
            {
                if (name == "$Target")
                {
                    return "";
                }
                if (name == "$FnIn")
                {
                    return "In";
                }
                if (name == "$FnOut")
                {
                    return "Out";
                }
                if (name == "$VarIn")
                {
                    return "Param";
                }
                if (name == "$VarOut")
                {
                    return "Value";
                }
            }
            return ObjectNames.NicifyVariableName(name);
        }

        public Property GetProperty() => 
            this.node.GetProperty(this.name);

        internal bool HasConnectionTo(Slot toSlot)
        {
            <HasConnectionTo>c__AnonStorey0 storey = new <HasConnectionTo>c__AnonStorey0 {
                toSlot = toSlot
            };
            return Enumerable.Any<UnityEditor.Graphs.Edge>(this.edges, new Func<UnityEditor.Graphs.Edge, bool>(storey.<>m__0));
        }

        private void Init()
        {
            this.edges = new List<UnityEditor.Graphs.Edge>();
        }

        public void RemoveEdge(UnityEditor.Graphs.Edge e)
        {
            this.edges.Remove(e);
        }

        public void ResetGenericArgumentType()
        {
            this.m_DataTypeString = SerializedType.ResetGenericArgumentType(this.m_DataTypeString);
        }

        public void SetGenericArgumentType(System.Type type)
        {
            this.m_DataTypeString = SerializedType.SetGenericArgumentType(this.m_DataTypeString, type);
        }

        public override string ToString() => 
            this.name;

        internal void WakeUp(Node owner)
        {
            if (this.edges == null)
            {
                this.edges = new List<UnityEditor.Graphs.Edge>();
            }
            this.m_Node = owner;
        }

        public System.Type dataType
        {
            get => 
                SerializedType.FromString(this.m_DataTypeString);
            set
            {
                this.m_DataTypeString = SerializedType.ToString(value);
            }
        }

        public string dataTypeString =>
            this.m_DataTypeString;

        public bool isDataSlot =>
            !string.IsNullOrEmpty(this.m_DataTypeString);

        public bool isFlowSlot =>
            (!this.isDataSlot && !this.isTarget);

        public bool isGeneric =>
            SerializedType.IsBaseTypeGeneric(this.m_DataTypeString);

        public bool isInputDataSlot =>
            (this.isDataSlot && (this.type == SlotType.InputSlot));

        public bool isInputSlot =>
            (this.type == SlotType.InputSlot);

        public bool isOutputDataSlot =>
            (this.isDataSlot && (this.type == SlotType.OutputSlot));

        public bool isOutputSlot =>
            (this.type == SlotType.OutputSlot);

        public bool isTarget =>
            (this.name == "$Target");

        public string name
        {
            get => 
                this.m_Name;
            set
            {
                this.m_Name = value;
                this.m_Title = GetNiceTitle(this.m_Name);
            }
        }

        public Node node
        {
            get => 
                this.m_Node;
            set
            {
                this.m_Node = value;
            }
        }

        public string title
        {
            get => 
                this.m_Title;
            set
            {
                this.m_Title = value;
            }
        }

        [CompilerGenerated]
        private sealed class <HasConnectionTo>c__AnonStorey0
        {
            internal Slot toSlot;

            internal bool <>m__0(UnityEditor.Graphs.Edge e) => 
                (e.toSlot == this.toSlot);
        }
    }
}

