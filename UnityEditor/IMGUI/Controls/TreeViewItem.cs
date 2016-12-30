namespace UnityEditor.IMGUI.Controls
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// <para>The TreeViewItem is used to build the tree representation of a tree data structure.</para>
    /// </summary>
    public class TreeViewItem : IComparable<TreeViewItem>
    {
        private List<TreeViewItem> m_Children;
        private int m_Depth;
        private string m_DisplayName;
        private Texture2D m_Icon;
        private int m_ID;
        private TreeViewItem m_Parent;

        public TreeViewItem()
        {
            this.m_Children = null;
        }

        /// <summary>
        /// <para>TreeViewItem constructor.</para>
        /// </summary>
        /// <param name="id">Unique ID to identify this TreeViewItem with among all TreeViewItems of the TreeView. See Also id.</param>
        /// <param name="depth">Depth of this TreeViewItem. See Also depth.</param>
        /// <param name="displayName">Rendered name of this TreeViewItem. See Also displayName.</param>
        public TreeViewItem(int id)
        {
            this.m_Children = null;
            this.m_ID = id;
        }

        /// <summary>
        /// <para>TreeViewItem constructor.</para>
        /// </summary>
        /// <param name="id">Unique ID to identify this TreeViewItem with among all TreeViewItems of the TreeView. See Also id.</param>
        /// <param name="depth">Depth of this TreeViewItem. See Also depth.</param>
        /// <param name="displayName">Rendered name of this TreeViewItem. See Also displayName.</param>
        public TreeViewItem(int id, int depth)
        {
            this.m_Children = null;
            this.m_ID = id;
            this.m_Depth = depth;
        }

        /// <summary>
        /// <para>TreeViewItem constructor.</para>
        /// </summary>
        /// <param name="id">Unique ID to identify this TreeViewItem with among all TreeViewItems of the TreeView. See Also id.</param>
        /// <param name="depth">Depth of this TreeViewItem. See Also depth.</param>
        /// <param name="displayName">Rendered name of this TreeViewItem. See Also displayName.</param>
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

        /// <summary>
        /// <para>Helper method that adds the child TreeViewItem to the children list and sets the parent property on the child.</para>
        /// </summary>
        /// <param name="child">TreeViewItem to be added to the children list.</param>
        public void AddChild(TreeViewItem child)
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

        /// <summary>
        /// <para>The list of child items of this TreeViewItem.</para>
        /// </summary>
        public virtual List<TreeViewItem> children
        {
            get => 
                this.m_Children;
            set
            {
                this.m_Children = value;
            }
        }

        /// <summary>
        /// <para>The depth refers to how many ancestors this item has, and corresponds to the number of horizontal ‘indents’ this item has.</para>
        /// </summary>
        public virtual int depth
        {
            get => 
                this.m_Depth;
            set
            {
                this.m_Depth = value;
            }
        }

        /// <summary>
        /// <para>Name shown for this item when rendered.</para>
        /// </summary>
        public virtual string displayName
        {
            get => 
                this.m_DisplayName;
            set
            {
                this.m_DisplayName = value;
            }
        }

        /// <summary>
        /// <para>Returns true if children has any items.</para>
        /// </summary>
        public virtual bool hasChildren =>
            ((this.m_Children != null) && (this.m_Children.Count > 0));

        /// <summary>
        /// <para>If set, this icon will be rendered to the left of the displayName. The icon is rendered at 16x16 points by default.</para>
        /// </summary>
        public virtual Texture2D icon
        {
            get => 
                this.m_Icon;
            set
            {
                this.m_Icon = value;
            }
        }

        /// <summary>
        /// <para>Unique ID for an item.</para>
        /// </summary>
        public virtual int id
        {
            get => 
                this.m_ID;
            set
            {
                this.m_ID = value;
            }
        }

        /// <summary>
        /// <para>The parent of this TreeViewItem. If it is null then it is considered the root of the TreeViewItem tree.</para>
        /// </summary>
        public virtual TreeViewItem parent
        {
            get => 
                this.m_Parent;
            set
            {
                this.m_Parent = value;
            }
        }
    }
}

