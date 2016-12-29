namespace UnityEditor.IMGUI.Controls
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    internal class TreeViewState
    {
        [SerializeField]
        private float[] m_ColumnWidths = null;
        [SerializeField]
        private CreateAssetUtility m_CreateAssetUtility = new CreateAssetUtility();
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
        public Vector2 scrollPos;

        internal void OnAwake()
        {
            this.m_RenameOverlay.Clear();
            this.m_CreateAssetUtility = new CreateAssetUtility();
        }

        internal float[] columnWidths
        {
            get => 
                this.m_ColumnWidths;
            set
            {
                this.m_ColumnWidths = value;
            }
        }

        internal CreateAssetUtility createAssetUtility
        {
            get => 
                this.m_CreateAssetUtility;
            set
            {
                this.m_CreateAssetUtility = value;
            }
        }

        public List<int> expandedIDs
        {
            get => 
                this.m_ExpandedIDs;
            set
            {
                this.m_ExpandedIDs = value;
            }
        }

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

        public string searchString
        {
            get => 
                this.m_SearchString;
            set
            {
                this.m_SearchString = value;
            }
        }

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

