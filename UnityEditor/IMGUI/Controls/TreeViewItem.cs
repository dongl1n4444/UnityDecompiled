namespace UnityEditor.IMGUI.Controls
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class TreeViewItem : IComparable<TreeViewItem>
    {
        private List<TreeViewItem> m_Children;
        private int m_Depth;
        private string m_DisplayName;
        private Texture2D m_Icon;
        private int m_ID;
        private TreeViewItem m_Parent;
        private object m_UserData;

        public TreeViewItem(int id)
        {
            this.m_Children = null;
            this.m_ID = id;
        }

        public TreeViewItem(int id, int depth)
        {
            this.m_Children = null;
            this.m_ID = id;
            this.m_Depth = depth;
        }

        public TreeViewItem(int id, int depth, string displayName)
        {
            this.m_Children = null;
            this.m_Depth = depth;
            this.m_ID = id;
            this.m_DisplayName = displayName;
        }

        internal TreeViewItem(int id, int depth, TreeViewItem parent, string displayName)
        {
            this.m_Children = null;
            this.m_Depth = depth;
            this.m_Parent = parent;
            this.m_ID = id;
            this.m_DisplayName = displayName;
        }

        internal void AddChild(TreeViewItem child)
        {
            if (this.m_Children == null)
            {
                this.m_Children = new List<TreeViewItem>();
            }
            this.m_Children.Add(child);
            if (child != null)
            {
                child.parent = this;
            }
        }

        public virtual int CompareTo(TreeViewItem other) => 
            this.displayName.CompareTo(other.displayName);

        public override string ToString() => 
            $"Item: '{this.displayName}' ({this.id}), has {(!this.hasChildren ? 0 : this.children.Count)} children, depth {this.depth}, parent id {((this.parent == null) ? -1 : this.parent.id)}";

        public virtual List<TreeViewItem> children
        {
            get => 
                this.m_Children;
            set
            {
                this.m_Children = value;
            }
        }

        public virtual int depth
        {
            get => 
                this.m_Depth;
            set
            {
                this.m_Depth = value;
            }
        }

        public virtual string displayName
        {
            get => 
                this.m_DisplayName;
            set
            {
                this.m_DisplayName = value;
            }
        }

        public virtual bool hasChildren =>
            ((this.m_Children != null) && (this.m_Children.Count > 0));

        public virtual Texture2D icon
        {
            get => 
                this.m_Icon;
            set
            {
                this.m_Icon = value;
            }
        }

        public virtual int id
        {
            get => 
                this.m_ID;
            set
            {
                this.m_ID = value;
            }
        }

        public virtual TreeViewItem parent
        {
            get => 
                this.m_Parent;
            set
            {
                this.m_Parent = value;
            }
        }

        internal virtual object userData
        {
            get => 
                this.m_UserData;
            set
            {
                this.m_UserData = value;
            }
        }
    }
}

