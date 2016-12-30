namespace UnityEditor
{
    using System;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;
    using UnityEngine.Profiling;

    internal class EmissionTable
    {
        private EmissionTreeDataStore m_DataStore;
        private MultiColumnHeader m_Header;
        private string m_SerializationUID = "EmissionTable_";
        private TreeViewState m_State;
        private EmissionTreeView m_TreeView;

        public EmissionTable()
        {
            this.OnEnable();
        }

        private void InitIfNeeded()
        {
            if (this.m_TreeView == null)
            {
                MultiColumnHeaderState.Column[] columnArray1 = new MultiColumnHeaderState.Column[3];
                MultiColumnHeaderState.Column column = new MultiColumnHeaderState.Column {
                    headerContent = new GUIContent("Name"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 150f,
                    minWidth = 30f,
                    maxWidth = float.MaxValue,
                    autoResize = false,
                    allowToggleVisibility = false
                };
                columnArray1[0] = column;
                column = new MultiColumnHeaderState.Column {
                    headerContent = new GUIContent("Material"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 150f,
                    minWidth = 80f,
                    maxWidth = float.MaxValue,
                    autoResize = false,
                    allowToggleVisibility = false
                };
                columnArray1[1] = column;
                column = new MultiColumnHeaderState.Column {
                    headerContent = new GUIContent("GI mode"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 150f,
                    minWidth = 30f,
                    maxWidth = float.MaxValue,
                    autoResize = false,
                    allowToggleVisibility = true
                };
                columnArray1[2] = column;
                MultiColumnHeaderState.Column[] columns = columnArray1;
                this.m_DataStore = new EmissionTreeDataStore();
                this.m_State = new TreeViewState();
                this.m_Header = new MultiColumnHeader(new MultiColumnHeaderState(columns));
                this.m_TreeView = new EmissionTreeView(this.m_State, this.m_Header, this.m_DataStore);
                this.m_TreeView.multiColumnHeader.state.sortedColumnIndex = 0;
                this.m_TreeView.DeserializeState(this.m_SerializationUID);
                this.m_TreeView.Reload();
            }
        }

        public void OnDisable()
        {
            if (this.m_TreeView != null)
            {
                this.m_TreeView.SerializeState(this.m_SerializationUID);
            }
        }

        public void OnEnable()
        {
        }

        public void OnGUI()
        {
            Profiler.BeginSample("EmissionTable.OnGUI");
            this.InitIfNeeded();
            Rect rect = GUILayoutUtility.GetRect(0f, float.MaxValue, 0f, float.MaxValue);
            if (Event.current.type != EventType.Layout)
            {
                float width = rect.width;
                float num2 = 20f;
                float num3 = rect.height - num2;
                float height = rect.height;
                rect.height = num2;
                Rect r = rect;
                rect.height = num3;
                rect.y += num2;
                Rect rect3 = rect;
                this.m_TreeView.OnGUI(rect3);
                this.m_TreeView.OnFilterGUI(r);
                Profiler.EndSample();
            }
        }

        public void OnHierarchyChange()
        {
            if ((this.m_DataStore != null) && (this.m_TreeView != null))
            {
                this.m_DataStore.Repopulate();
                this.m_TreeView.FullReload();
            }
        }

        public void OnInspectorUpdate()
        {
            if ((this.m_DataStore != null) && (this.m_TreeView != null))
            {
                this.m_DataStore.Repopulate();
                this.m_TreeView.FullReload();
            }
        }

        public void OnSelectionChange()
        {
            if (this.m_TreeView != null)
            {
                this.m_TreeView.SyncSelection();
            }
        }
    }
}

