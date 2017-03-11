namespace UnityEditor.Graphs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    public class Node : ScriptableObject
    {
        [CompilerGenerated]
        private static Func<Slot, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<Slot, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<string, Slot, string> <>f__am$cache10;
        [CompilerGenerated]
        private static Func<string, Slot, string> <>f__am$cache11;
        [CompilerGenerated]
        private static Func<Slot, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<Slot, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<Slot, bool> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<Slot, bool> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<Slot, bool> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<Slot, IEnumerable<UnityEditor.Graphs.Edge>> <>f__am$cache7;
        [CompilerGenerated]
        private static Func<Slot, IEnumerable<UnityEditor.Graphs.Edge>> <>f__am$cache8;
        [CompilerGenerated]
        private static Func<Slot, IEnumerable<UnityEditor.Graphs.Edge>> <>f__am$cache9;
        [CompilerGenerated]
        private static Func<Slot, IEnumerable<UnityEditor.Graphs.Edge>> <>f__am$cacheA;
        [CompilerGenerated]
        private static Func<Slot, IEnumerable<UnityEditor.Graphs.Edge>> <>f__am$cacheB;
        [CompilerGenerated]
        private static Func<Slot, IEnumerable<UnityEditor.Graphs.Edge>> <>f__am$cacheC;
        [CompilerGenerated]
        private static Func<Slot, bool> <>f__am$cacheD;
        [CompilerGenerated]
        private static Func<Slot, bool> <>f__am$cacheE;
        [CompilerGenerated]
        private static Func<Slot, bool> <>f__am$cacheF;
        [SerializeField]
        public UnityEditor.Graphs.Styles.Color color;
        [SerializeField]
        protected string m_GenericTypeString = string.Empty;
        [NonSerialized]
        internal Graph m_Graph;
        [NonSerialized]
        internal List<int> m_HiddenProperties = new List<int>();
        private bool m_IsDragging;
        private string m_NodeInvalidError = string.Empty;
        [SerializeField]
        internal List<Property> m_Properties = new List<Property>();
        [NonSerialized]
        internal List<int> m_SettingProperties = new List<int>();
        [NonSerialized]
        internal List<string> m_SettingPropertyTitles = new List<string>();
        [SerializeField]
        protected List<Slot> m_Slots = new List<Slot>();
        [NonSerialized]
        private string m_Title = string.Empty;
        [SerializeField]
        public Rect position;
        [SerializeField]
        internal bool showEmptySlots = true;
        [SerializeField]
        public string style = "node";

        public virtual void AddedToGraph()
        {
        }

        public Slot AddInputSlot(string name) => 
            this.AddInputSlot(name, null);

        public Slot AddInputSlot(string name, System.Type type)
        {
            Slot s = new Slot(SlotType.InputSlot, name, type);
            this.AddSlot(s);
            return s;
        }

        public Property AddOrModifyProperty(System.Type dataType, string name)
        {
            Property property = this.TryGetProperty(name);
            if (property != null)
            {
                if (!property.isGeneric && (property.type != dataType))
                {
                    property.ChangeDataType(dataType);
                }
                return property;
            }
            return this.ConstructAndAddProperty(dataType, name);
        }

        public Property AddOrModifyPropertyForSlot(Slot s)
        {
            Property property = this.TryGetProperty(s.name);
            if (property != null)
            {
                if (!s.isGeneric && (property.type != s.dataType))
                {
                    property.ChangeDataType(s.dataType);
                }
                return property;
            }
            return this.ConstructAndAddProperty(s.dataTypeString, s.name);
        }

        public Slot AddOutputSlot(string name) => 
            this.AddOutputSlot(name, null);

        public Slot AddOutputSlot(string name, System.Type type)
        {
            Slot s = new Slot(SlotType.OutputSlot, name, type);
            this.AddSlot(s);
            return s;
        }

        public void AddProperty(Property p)
        {
            this.AssertNotDuplicateName(p.name);
            this.m_Properties.Add(p);
            this.Dirty();
        }

        public void AddSlot(Slot s)
        {
            this.AddSlot(s, -1);
        }

        public virtual void AddSlot(Slot s, int index)
        {
            if (index != -1)
            {
                this.m_Slots.Insert(index, s);
            }
            else
            {
                this.m_Slots.Add(s);
            }
            if ((s.type == SlotType.InputSlot) && !s.isFlowSlot)
            {
                this.AddOrModifyPropertyForSlot(s);
            }
            if (s.node != null)
            {
                throw new ArgumentException("Slot already attached to another node");
            }
            s.node = this;
            this.Dirty();
        }

        private void AssertNotDuplicateName(string name)
        {
            if (this.TryGetProperty(name) != null)
            {
                throw new ArgumentException("Property '" + name + "' already exists.");
            }
        }

        internal virtual void Awake()
        {
        }

        public virtual void BeginDrag()
        {
        }

        public virtual void ChangeSlotType(Slot s, System.Type toType)
        {
            if (s.dataType != toType)
            {
                s.dataType = toType;
                if (s.isInputDataSlot)
                {
                    this.GetProperty(s.name).ChangeDataType(toType);
                }
                if ((this.graph != null) && s.isDataSlot)
                {
                    if (s.type == SlotType.InputSlot)
                    {
                        this.graph.RevalidateInputDataEdges(s);
                    }
                    else
                    {
                        this.graph.RevalidateOutputDataEdges(s);
                    }
                }
                this.Dirty();
            }
        }

        public Property ConstructAndAddProperty(string serializedTypeString, string name)
        {
            Property p = new Property(serializedTypeString, name);
            this.AddProperty(p);
            return p;
        }

        public Property ConstructAndAddProperty(System.Type type, string name)
        {
            Property p = new Property(type, name);
            this.AddProperty(p);
            return p;
        }

        public virtual void Dirty()
        {
            EditorUtility.SetDirty(this);
        }

        public virtual void EndDrag()
        {
            this.m_IsDragging = false;
        }

        public Property GetOrCreateAndAddProperty(System.Type type, string name)
        {
            Property property = this.TryGetProperty(name);
            if (property == null)
            {
                return this.ConstructAndAddProperty(type, name);
            }
            property.ChangeDataType(type);
            return property;
        }

        public Property GetProperty(string name)
        {
            Property property = this.TryGetProperty(name);
            if (property == null)
            {
                throw new ArgumentException("Property '" + name + "' not found.");
            }
            return property;
        }

        public object GetPropertyValue(string name) => 
            this.GetProperty(name).value;

        public string GetSettingPropertyTitle(Property property)
        {
            int index = this.m_Properties.IndexOf(property);
            if (index == -1)
            {
                return string.Empty;
            }
            index = this.m_SettingProperties.IndexOf(index);
            if (index == -1)
            {
                return string.Empty;
            }
            return this.m_SettingPropertyTitles[index];
        }

        public object GetSlotValue(string slotName) => 
            this.GetPropertyValue(slotName);

        internal void HideProperty(Property p)
        {
            int index = this.properties.IndexOf(p);
            if (index == -1)
            {
                throw new ArgumentException("Could not find property to hide.");
            }
            this.m_HiddenProperties.Add(index);
            this.Dirty();
        }

        public virtual void InputEdgeChanged(UnityEditor.Graphs.Edge e)
        {
        }

        public static Node Instance() => 
            Instance<Node>();

        public static T Instance<T>() where T: Node, new() => 
            ScriptableObject.CreateInstance<T>();

        internal bool IsPropertyHidden(Property p)
        {
            int index = this.properties.IndexOf(p);
            if (index == -1)
            {
                return false;
            }
            return this.m_HiddenProperties.Contains(index);
        }

        internal void MakeSettingProperty(Property p, string title)
        {
            int index = this.properties.IndexOf(p);
            if (index == -1)
            {
                throw new ArgumentException("Failed to find property to turn into a setting property.");
            }
            this.m_SettingProperties.Add(index);
            this.m_SettingPropertyTitles.Add(title);
            this.Dirty();
        }

        public virtual void NodeUI(GraphGUI host)
        {
        }

        public virtual void OnDrag()
        {
            this.m_IsDragging = true;
        }

        public void RemoveProperty(string name)
        {
            Property p = this.TryGetProperty(name);
            if (p == null)
            {
                Debug.LogError("Trying to remove non-existant property " + name);
            }
            else
            {
                this.RemoveProperty(p);
            }
        }

        public void RemoveProperty(Property p)
        {
            if (!this.m_Properties.Contains(p))
            {
                Debug.LogError("Trying to remove non-existant property " + base.name);
            }
            else
            {
                int index = this.m_Properties.IndexOf(p);
                this.m_Properties.RemoveAt(index);
                int num2 = this.m_SettingProperties.IndexOf(index);
                if (num2 != -1)
                {
                    this.m_SettingProperties.RemoveAt(num2);
                    this.m_SettingPropertyTitles.RemoveAt(num2);
                }
                int num3 = this.m_HiddenProperties.IndexOf(index);
                if (num3 != -1)
                {
                    this.m_HiddenProperties.RemoveAt(num3);
                }
                this.Dirty();
            }
        }

        public virtual void RemoveSlot(Slot s)
        {
            this.m_Slots.Remove(s);
            foreach (UnityEditor.Graphs.Edge edge in s.edges)
            {
                if (edge.fromSlot != s)
                {
                    edge.fromSlot.RemoveEdge(edge);
                }
                if (edge.toSlot != s)
                {
                    edge.toSlot.RemoveEdge(edge);
                }
                this.graph.edges.Remove(edge);
                this.graph.Dirty();
            }
            s.edges.Clear();
            if (this.graph != null)
            {
                this.graph.RemoveInvalidEdgesForSlot(s);
            }
            this.Dirty();
        }

        public virtual void RemovingFromGraph()
        {
        }

        public virtual void RenameProperty(string oldName, string newName, System.Type newType)
        {
            Property property = this.GetProperty(oldName);
            property.name = newName;
            property.ChangeDataType(newType);
            this.Dirty();
        }

        public virtual void ResetGenericPropertyArgumentType()
        {
            this.m_GenericTypeString = string.Empty;
            foreach (Slot slot in this.slots)
            {
                if (slot.isGeneric && slot.isInputDataSlot)
                {
                    this.GetProperty(slot.name).ResetGenericArgumentType();
                    slot.ResetGenericArgumentType();
                    while (slot.edges.Any<UnityEditor.Graphs.Edge>())
                    {
                        this.graph.RemoveEdge(slot.edges.First<UnityEditor.Graphs.Edge>());
                    }
                }
            }
            if (<>f__am$cacheF == null)
            {
                <>f__am$cacheF = s => s.isGeneric && s.isOutputDataSlot;
            }
            foreach (Slot slot2 in Enumerable.Where<Slot>(this.slots, <>f__am$cacheF))
            {
                slot2.ResetGenericArgumentType();
                while (slot2.edges.Any<UnityEditor.Graphs.Edge>())
                {
                    this.graph.RemoveEdge(slot2.edges.First<UnityEditor.Graphs.Edge>());
                }
            }
            this.Dirty();
        }

        public virtual void SetGenericPropertyArgumentType(System.Type type)
        {
            if (<>f__am$cacheD == null)
            {
                <>f__am$cacheD = s => s.isGeneric;
            }
            foreach (Slot slot in Enumerable.Where<Slot>(this.inputDataSlots, <>f__am$cacheD))
            {
                Property property = this.GetProperty(slot.name);
                slot.SetGenericArgumentType(type);
                property.SetGenericArgumentType(type);
            }
            if (<>f__am$cacheE == null)
            {
                <>f__am$cacheE = s => s.isGeneric;
            }
            foreach (Slot slot2 in Enumerable.Where<Slot>(this.outputDataSlots, <>f__am$cacheE))
            {
                slot2.SetGenericArgumentType(type);
            }
            this.genericType = type;
            this.Dirty();
        }

        public void SetPropertyValue(string name, object value)
        {
            this.GetProperty(name).value = value;
            this.Dirty();
        }

        public void SetPropertyValueOrCreateAndAddProperty(string name, System.Type type, object value)
        {
            Property p = this.TryGetProperty(name);
            if (p == null)
            {
                p = new Property(type, name);
                this.AddProperty(p);
            }
            else
            {
                p.ChangeDataType(type);
            }
            p.value = value;
            this.Dirty();
        }

        public override string ToString()
        {
            string str = ("[" + base.GetType().Name + "   " + this.title) + "| ";
            if (<>f__am$cache10 == null)
            {
                <>f__am$cache10 = (current, slot) => current + "o[" + slot.name + "]";
            }
            str = Enumerable.Aggregate<Slot, string>(this.outputSlots, str, <>f__am$cache10);
            if (<>f__am$cache11 == null)
            {
                <>f__am$cache11 = (current, slot) => current + "i[" + slot.name + "]";
            }
            return (Enumerable.Aggregate<Slot, string>(this.outputSlots, str, <>f__am$cache11) + "]");
        }

        public Property TryGetProperty(string name)
        {
            <TryGetProperty>c__AnonStorey1 storey = new <TryGetProperty>c__AnonStorey1 {
                name = name
            };
            return Enumerable.FirstOrDefault<Property>(this.m_Properties, new Func<Property, bool>(storey.<>m__0));
        }

        public object TryGetSlotPropertyValue(Slot slot)
        {
            Property property = this.TryGetProperty(slot.name);
            return property?.value;
        }

        internal virtual void WakeUp(Graph owner)
        {
            this.m_Graph = owner;
            if (this.m_Slots == null)
            {
                Debug.LogError("Slots are null - should not happen");
                this.m_Slots = new List<Slot>();
            }
            foreach (Slot slot in this.slots)
            {
                if (slot != null)
                {
                    slot.WakeUp(this);
                }
                else
                {
                    Debug.LogError("NULL SLOT");
                }
            }
        }

        public System.Type genericType
        {
            get => 
                SerializedType.FromString(this.m_GenericTypeString);
            set
            {
                this.m_GenericTypeString = SerializedType.ToString(value);
            }
        }

        public Graph graph
        {
            get => 
                this.m_Graph;
            set
            {
                this.m_Graph = value;
            }
        }

        public bool hasTitle =>
            !string.IsNullOrEmpty(this.m_Title);

        public IEnumerable<UnityEditor.Graphs.Edge> inputDataEdges
        {
            get
            {
                if (<>f__am$cacheC == null)
                {
                    <>f__am$cacheC = s => s.edges;
                }
                return Enumerable.SelectMany<Slot, UnityEditor.Graphs.Edge>(this.inputDataSlots, <>f__am$cacheC);
            }
        }

        public IEnumerable<Slot> inputDataSlots
        {
            get
            {
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = s => ((s.type == SlotType.InputSlot) && s.isDataSlot) && (s.name != "$Target");
                }
                return Enumerable.Where<Slot>(this.m_Slots, <>f__am$cache3);
            }
        }

        public IEnumerable<UnityEditor.Graphs.Edge> inputEdges
        {
            get
            {
                if (<>f__am$cache8 == null)
                {
                    <>f__am$cache8 = s => s.edges;
                }
                return Enumerable.SelectMany<Slot, UnityEditor.Graphs.Edge>(this.inputSlots, <>f__am$cache8);
            }
        }

        public IEnumerable<UnityEditor.Graphs.Edge> inputFlowEdges
        {
            get
            {
                if (<>f__am$cacheA == null)
                {
                    <>f__am$cacheA = s => s.edges;
                }
                return Enumerable.SelectMany<Slot, UnityEditor.Graphs.Edge>(this.inputFlowSlots, <>f__am$cacheA);
            }
        }

        public IEnumerable<Slot> inputFlowSlots
        {
            get
            {
                if (<>f__am$cache5 == null)
                {
                    <>f__am$cache5 = s => (s.type == SlotType.InputSlot) && s.isFlowSlot;
                }
                return Enumerable.Where<Slot>(this.m_Slots, <>f__am$cache5);
            }
        }

        public IEnumerable<Slot> inputSlots
        {
            get
            {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = s => s.type == SlotType.InputSlot;
                }
                return Enumerable.Where<Slot>(this.m_Slots, <>f__am$cache1);
            }
        }

        public bool isDragging =>
            this.m_IsDragging;

        public bool isGeneric
        {
            get
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = s => s.isGeneric;
                }
                return Enumerable.Any<Slot>(this.slots, <>f__am$cache0);
            }
        }

        public Slot this[string name]
        {
            get
            {
                <>c__AnonStorey0 storey = new <>c__AnonStorey0 {
                    name = name
                };
                return Enumerable.FirstOrDefault<Slot>(this.m_Slots, new Func<Slot, bool>(storey.<>m__0));
            }
        }

        public Slot this[int index] =>
            this.m_Slots[index];

        public string nodeInvalidError
        {
            get => 
                this.m_NodeInvalidError;
            set
            {
                this.m_NodeInvalidError = value;
            }
        }

        public bool nodeIsInvalid =>
            (this.m_NodeInvalidError != string.Empty);

        public IEnumerable<UnityEditor.Graphs.Edge> outputDataEdges
        {
            get
            {
                if (<>f__am$cacheB == null)
                {
                    <>f__am$cacheB = s => s.edges;
                }
                return Enumerable.SelectMany<Slot, UnityEditor.Graphs.Edge>(this.outputDataSlots, <>f__am$cacheB);
            }
        }

        public IEnumerable<Slot> outputDataSlots
        {
            get
            {
                if (<>f__am$cache4 == null)
                {
                    <>f__am$cache4 = s => (s.type == SlotType.OutputSlot) && s.isDataSlot;
                }
                return Enumerable.Where<Slot>(this.m_Slots, <>f__am$cache4);
            }
        }

        public IEnumerable<UnityEditor.Graphs.Edge> outputEdges
        {
            get
            {
                if (<>f__am$cache7 == null)
                {
                    <>f__am$cache7 = s => s.edges;
                }
                return Enumerable.SelectMany<Slot, UnityEditor.Graphs.Edge>(this.outputSlots, <>f__am$cache7);
            }
        }

        public IEnumerable<UnityEditor.Graphs.Edge> outputFlowEdges
        {
            get
            {
                if (<>f__am$cache9 == null)
                {
                    <>f__am$cache9 = s => s.edges;
                }
                return Enumerable.SelectMany<Slot, UnityEditor.Graphs.Edge>(this.outputFlowSlots, <>f__am$cache9);
            }
        }

        public IEnumerable<Slot> outputFlowSlots
        {
            get
            {
                if (<>f__am$cache6 == null)
                {
                    <>f__am$cache6 = s => (s.type == SlotType.OutputSlot) && s.isFlowSlot;
                }
                return Enumerable.Where<Slot>(this.m_Slots, <>f__am$cache6);
            }
        }

        public IEnumerable<Slot> outputSlots
        {
            get
            {
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = s => s.type == SlotType.OutputSlot;
                }
                return Enumerable.Where<Slot>(this.m_Slots, <>f__am$cache2);
            }
        }

        public List<Property> properties =>
            this.m_Properties;

        public IEnumerable<Property> settingProperties =>
            (from i in this.m_SettingProperties select this.m_Properties[i]);

        public List<Slot> slots =>
            this.m_Slots;

        public virtual string title
        {
            get => 
                ((this.m_Title != string.Empty) ? this.m_Title : base.name);
            set
            {
                this.m_Title = value;
                this.Dirty();
            }
        }

        public virtual string windowTitle =>
            base.GetType().Name;

        [CompilerGenerated]
        private sealed class <>c__AnonStorey0
        {
            internal string name;

            internal bool <>m__0(Slot s) => 
                (s.name == this.name);
        }

        [CompilerGenerated]
        private sealed class <TryGetProperty>c__AnonStorey1
        {
            internal string name;

            internal bool <>m__0(Property p) => 
                (p.name == this.name);
        }
    }
}

