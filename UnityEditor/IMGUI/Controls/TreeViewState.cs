namespace UnityEditor.IMGUI.Controls
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// <para>The TreeViewState contains serializable state information for the TreeView.</para>
    /// </summary>
    [Serializable]
    public class TreeViewState
    {
        [SerializeField]
        private List<int> m_ExpandedIDs = new List<int>();
        [SerializeField]
        private int m_LastClickedID;
        [SerializeField]
        private RenameOverlay m_RenameOverlay = new RenameOverlay();
        [SerializeField]
        private string m_SearchString;
        [SerializeField]
        private List<int> m_SelectedIDs = new List<int>();
        /// <summary>
        /// <para>The current scroll values of the TreeView's scroll view.</para>
        /// </summary>
        public Vector2 scrollPos;

        internal virtual void OnAwake()
        {
            this.m_RenameOverlay.Clear();
        }

        /// <summary>
        /// <para>This is the list of currently expanded TreeViewItem IDs.</para>
        /// </summary>
        public List<int> expandedIDs
        {
            get => 
                this.m_ExpandedIDs;
            set
            {
                this.m_ExpandedIDs = value;
            }
        }

        /// <summary>
        /// <para>The ID for the TreeViewItem that currently is being used for multi selection and key navigation.</para>
        /// </summary>
        public int lastClickedID
        {
            get => 
                this.m_LastClickedID;
            set
            {
                this.m_LastClickedID = value;
            }
        }

        internal RenameOverlay renameOverlay
        {
            get => 
                this.m_RenameOverlay;
            set
            {
                this.m_RenameOverlay = value;
            }
        }

        /// <summary>
        /// <para>Search string state that can be used in the TreeView to filter the tree data when creating the TreeViewItems.</para>
        /// </summary>
        public string searchString
        {
            get => 
                this.m_SearchString;
            set
            {
                this.m_SearchString = value;
            }
        }

        /// <summary>
        /// <para>Selected TreeViewItem IDs. Use of the SetSelection and IsSelected API will access this state.</para>
        /// </summary>
        public List<int> selectedIDs
        {
            get => 
                this.m_SelectedIDs;
            set
            {
                this.m_SelectedIDs = value;
            }
        }
    }
}

