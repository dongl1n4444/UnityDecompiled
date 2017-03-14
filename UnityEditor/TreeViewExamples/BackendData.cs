namespace UnityEditor.TreeViewExamples
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class BackendData
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <IDCounter>k__BackingField;
        private const int k_MaxChildren = 15;
        private const int k_MaxDepth = 12;
        private const int k_MinChildren = 3;
        private const float k_ProbOfLastDescendent = 0.5f;
        private int m_MaxItems = 0x2710;
        public bool m_RecursiveFindParentsBelow = true;
        private Foo m_Root;

        private void AddChildrenRecursive(Foo foo, int numChildren, bool force)
        {
            if (((this.IDCounter <= this.m_MaxItems) && (foo.depth < 12)) && (force || (UnityEngine.Random.value >= 0.5f)))
            {
                if (foo.children == null)
                {
                    foo.children = new List<Foo>(numChildren);
                }
                for (int i = 0; i < numChildren; i++)
                {
                    int num2;
                    this.IDCounter = num2 = this.IDCounter + 1;
                    Foo item = new Foo("Tud" + this.IDCounter, foo.depth + 1, num2) {
                        parent = foo
                    };
                    foo.children.Add(item);
                }
                if (this.IDCounter <= this.m_MaxItems)
                {
                    foreach (Foo foo3 in foo.children)
                    {
                        this.AddChildrenRecursive(foo3, UnityEngine.Random.Range(3, 15), false);
                    }
                }
            }
        }

        public Foo Find(int id) => 
            this.FindRecursive(id, this.m_Root);

        public static Foo FindItemRecursive(Foo item, int id)
        {
            if (item != null)
            {
                if (item.id == id)
                {
                    return item;
                }
                if (item.children == null)
                {
                    return null;
                }
                foreach (Foo foo2 in item.children)
                {
                    Foo foo3 = FindItemRecursive(foo2, id);
                    if (foo3 != null)
                    {
                        return foo3;
                    }
                }
            }
            return null;
        }

        public Foo FindRecursive(int id, Foo parent)
        {
            if (parent.hasChildren)
            {
                foreach (Foo foo2 in parent.children)
                {
                    if (foo2.id == id)
                    {
                        return foo2;
                    }
                    Foo foo3 = this.FindRecursive(id, foo2);
                    if (foo3 != null)
                    {
                        return foo3;
                    }
                }
            }
            return null;
        }

        public void GenerateData(int maxNumItems)
        {
            this.m_MaxItems = maxNumItems;
            this.IDCounter = 1;
            this.m_Root = new Foo("Root", 0, 0);
            for (int i = 0; i < 10; i++)
            {
                this.AddChildrenRecursive(this.m_Root, UnityEngine.Random.Range(3, 15), true);
            }
        }

        public HashSet<int> GetParentsBelow(int id)
        {
            Foo searchFromThis = FindItemRecursive(this.root, id);
            if (searchFromThis != null)
            {
                if (this.m_RecursiveFindParentsBelow)
                {
                    return this.GetParentsBelowRecursive(searchFromThis);
                }
                return this.GetParentsBelowStackBased(searchFromThis);
            }
            return new HashSet<int>();
        }

        private HashSet<int> GetParentsBelowRecursive(Foo searchFromThis)
        {
            HashSet<int> parentIDs = new HashSet<int>();
            GetParentsBelowRecursive(searchFromThis, parentIDs);
            return parentIDs;
        }

        private static void GetParentsBelowRecursive(Foo item, HashSet<int> parentIDs)
        {
            if (item.hasChildren)
            {
                parentIDs.Add(item.id);
                foreach (Foo foo in item.children)
                {
                    GetParentsBelowRecursive(foo, parentIDs);
                }
            }
        }

        private HashSet<int> GetParentsBelowStackBased(Foo searchFromThis)
        {
            Stack<Foo> stack = new Stack<Foo>();
            stack.Push(searchFromThis);
            HashSet<int> set = new HashSet<int>();
            while (stack.Count > 0)
            {
                Foo foo = stack.Pop();
                if (foo.hasChildren)
                {
                    set.Add(foo.id);
                    foreach (Foo foo2 in foo.children)
                    {
                        stack.Push(foo2);
                    }
                }
            }
            return set;
        }

        public void ReparentSelection(Foo parentItem, int insertionIndex, List<Foo> draggedItems)
        {
            if (parentItem != null)
            {
                if (insertionIndex > 0)
                {
                    insertionIndex -= Enumerable.Count<Foo>(parentItem.children.GetRange(0, insertionIndex), new Func<Foo, bool>(draggedItems.Contains));
                }
                foreach (Foo foo in draggedItems)
                {
                    foo.parent.children.Remove(foo);
                    foo.parent = parentItem;
                }
                if (!parentItem.hasChildren)
                {
                    parentItem.children = new List<Foo>();
                }
                List<Foo> list = new List<Foo>(parentItem.children);
                if (insertionIndex == -1)
                {
                    insertionIndex = 0;
                }
                list.InsertRange(insertionIndex, draggedItems);
                parentItem.children = list;
            }
        }

        public int IDCounter { get; private set; }

        public Foo root =>
            this.m_Root;

        public class Foo
        {
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private List<BackendData.Foo> <children>k__BackingField;
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private int <depth>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private int <id>k__BackingField;
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private string <name>k__BackingField;
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private BackendData.Foo <parent>k__BackingField;

            public Foo(string name, int depth, int id)
            {
                this.name = name;
                this.depth = depth;
                this.id = id;
            }

            public List<BackendData.Foo> children { get; set; }

            public int depth { get; set; }

            public bool hasChildren =>
                ((this.children != null) && (this.children.Count > 0));

            public int id { get; set; }

            public string name { get; set; }

            public BackendData.Foo parent { get; set; }
        }
    }
}

