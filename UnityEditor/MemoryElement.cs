namespace UnityEditor
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    internal class MemoryElement
    {
        public List<MemoryElement> children;
        public string description;
        public bool expanded;
        public ObjectInfo memoryInfo;
        public string name;
        public MemoryElement parent;
        public int totalChildCount;
        public long totalMemory;

        public MemoryElement()
        {
            this.children = null;
            this.parent = null;
            this.children = new List<MemoryElement>();
        }

        public MemoryElement(string n)
        {
            this.children = null;
            this.parent = null;
            this.expanded = false;
            this.name = n;
            this.children = new List<MemoryElement>();
            this.description = "";
        }

        public MemoryElement(string n, List<MemoryElement> groups)
        {
            this.children = null;
            this.parent = null;
            this.name = n;
            this.expanded = false;
            this.description = "";
            this.totalMemory = 0L;
            this.totalChildCount = 0;
            this.children = new List<MemoryElement>();
            foreach (MemoryElement element in groups)
            {
                this.AddChild(element);
            }
        }

        public MemoryElement(ObjectInfo memInfo, bool finalize)
        {
            this.children = null;
            this.parent = null;
            this.expanded = false;
            this.memoryInfo = memInfo;
            this.name = this.memoryInfo.name;
            this.totalMemory = (memInfo == null) ? 0L : memInfo.memorySize;
            this.totalChildCount = 1;
            if (finalize)
            {
                this.children = new List<MemoryElement>();
            }
        }

        public int AccumulatedChildCount() => 
            this.totalChildCount;

        public void AddChild(MemoryElement node)
        {
            if (node == this)
            {
                throw new Exception("Should not AddChild to itself");
            }
            this.children.Add(node);
            node.parent = this;
            this.totalMemory += node.totalMemory;
            this.totalChildCount += node.totalChildCount;
        }

        public int ChildCount()
        {
            if (this.children != null)
            {
                return this.children.Count;
            }
            return this.ReferenceCount();
        }

        public void ExpandChildren()
        {
            if (this.children == null)
            {
                this.children = new List<MemoryElement>();
                for (int i = 0; i < this.ReferenceCount(); i++)
                {
                    this.AddChild(new MemoryElement(this.memoryInfo.referencedBy[i], false));
                }
            }
        }

        public MemoryElement FirstChild() => 
            this.children[0];

        public int GetChildIndexInList()
        {
            for (int i = 0; i < this.parent.children.Count; i++)
            {
                if (this.parent.children[i] == this)
                {
                    return i;
                }
            }
            return this.parent.children.Count;
        }

        public MemoryElement GetNextNode()
        {
            if (this.expanded && (this.children.Count > 0))
            {
                return this.children[0];
            }
            int num = this.GetChildIndexInList() + 1;
            if (num < this.parent.children.Count)
            {
                return this.parent.children[num];
            }
            for (MemoryElement element2 = this.parent; element2.parent != null; element2 = element2.parent)
            {
                int num2 = element2.GetChildIndexInList() + 1;
                if (num2 < element2.parent.children.Count)
                {
                    return element2.parent.children[num2];
                }
            }
            return null;
        }

        public MemoryElement GetPrevNode()
        {
            int num = this.GetChildIndexInList() - 1;
            if (num >= 0)
            {
                MemoryElement element = this.parent.children[num];
                while (element.expanded)
                {
                    element = element.children[element.children.Count - 1];
                }
                return element;
            }
            return this.parent;
        }

        public MemoryElement GetRoot()
        {
            if (this.parent != null)
            {
                return this.parent.GetRoot();
            }
            return this;
        }

        public MemoryElement LastChild()
        {
            if (!this.expanded)
            {
                return this;
            }
            return this.children[this.children.Count - 1].LastChild();
        }

        public int ReferenceCount() => 
            (((this.memoryInfo == null) || (this.memoryInfo.referencedBy == null)) ? 0 : this.memoryInfo.referencedBy.Count);
    }
}

