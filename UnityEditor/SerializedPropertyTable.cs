namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;
    using UnityEngine.Profiling;

    internal class SerializedPropertyTable
    {
        private float m_ColumnHeaderHeight;
        private SerializedPropertyDataStore m_DataStore;
        private bool m_DragHandleEnabled = false;
        private readonly float m_DragHeight = 20f;
        private readonly float m_FilterHeight = 20f;
        private SerializedPropertyDataStore.GatherDelegate m_GatherDelegate;
        private HeaderDelegate m_HeaderDelegate;
        private bool m_Initialized;
        private MultiColumnHeaderState m_MultiColumnHeaderState;
        private string m_SerializationUID;
        private float m_TableHeight = 200f;
        private SerializedPropertyTreeView m_TreeView;
        private TreeViewState m_TreeViewState;
        private static readonly string s_TableHeight = "_TableHeight";

        public SerializedPropertyTable(string serializationUID, SerializedPropertyDataStore.GatherDelegate gatherDelegate, HeaderDelegate headerDelegate)
        {
            this.m_SerializationUID = serializationUID;
            this.m_GatherDelegate = gatherDelegate;
            this.m_HeaderDelegate = headerDelegate;
            this.OnEnable();
        }

        private float GetMinHeight()
        {
            float num = ((this.m_FilterHeight + this.m_ColumnHeaderHeight) + 16f) + this.m_DragHeight;
            return (num + 48f);
        }

        private void InitIfNeeded()
        {
            if (!this.m_Initialized)
            {
                if (this.m_TreeViewState == null)
                {
                    this.m_TreeViewState = new TreeViewState();
                }
                if (this.m_MultiColumnHeaderState == null)
                {
                    string[] strArray;
                    this.m_MultiColumnHeaderState = new MultiColumnHeaderState(this.m_HeaderDelegate(out strArray));
                    this.m_DataStore = new SerializedPropertyDataStore(strArray, this.m_GatherDelegate);
                }
                MultiColumnHeader multicolumnHeader = new MultiColumnHeader(this.m_MultiColumnHeaderState);
                this.m_ColumnHeaderHeight = multicolumnHeader.height;
                this.m_TreeView = new SerializedPropertyTreeView(this.m_TreeViewState, multicolumnHeader, this.m_DataStore);
                this.m_TreeView.DeserializeState(this.m_SerializationUID);
                this.m_TreeView.Reload();
                this.m_Initialized = true;
            }
        }

        public void OnDisable()
        {
            if (this.m_TreeView != null)
            {
                this.m_TreeView.SerializeState(this.m_SerializationUID);
            }
            SessionState.SetFloat(this.m_SerializationUID + s_TableHeight, this.m_TableHeight);
        }

        public void OnEnable()
        {
            this.m_TableHeight = SessionState.GetFloat(this.m_SerializationUID + s_TableHeight, 200f);
        }

        public void OnGUI()
        {
            Rect rect;
            Profiler.BeginSample("SerializedPropertyTable.OnGUI");
            this.InitIfNeeded();
            if (this.dragHandleEnabled)
            {
                rect = GUILayoutUtility.GetRect(0f, 10000f, this.m_TableHeight, this.m_TableHeight);
            }
            else
            {
                rect = GUILayoutUtility.GetRect(0f, float.MaxValue, 0f, float.MaxValue);
            }
            if (Event.current.type != EventType.Layout)
            {
                float width = rect.width;
                float num2 = 32f;
                float num3 = (rect.height - this.m_FilterHeight) - (!this.dragHandleEnabled ? 0f : this.m_DragHeight);
                float height = rect.height;
                rect.height = this.m_FilterHeight;
                Rect r = rect;
                rect.height = num3;
                rect.y += this.m_FilterHeight;
                Rect rect3 = rect;
                Profiler.BeginSample("TreeView.OnGUI");
                this.m_TreeView.OnGUI(rect3);
                Profiler.EndSample();
                if (this.dragHandleEnabled)
                {
                    rect.y += num3 + 1f;
                    rect.height = 1f;
                    Rect position = rect;
                    rect.height = 10f;
                    rect.y += 10f;
                    rect.x += (rect.width - num2) * 0.5f;
                    rect.width = num2;
                    this.m_TableHeight = EditorGUI.HeightResizer(rect, this.m_TableHeight, this.GetMinHeight(), float.MaxValue);
                    if (this.m_MultiColumnHeaderState.widthOfAllVisibleColumns <= width)
                    {
                        Rect texCoords = new Rect(0f, 1f, 1f, 1f - (1f / ((float) EditorStyles.inspectorTitlebar.normal.background.height)));
                        GUI.DrawTextureWithTexCoords(position, EditorStyles.inspectorTitlebar.normal.background, texCoords);
                    }
                    if (Event.current.type == EventType.Repaint)
                    {
                        Styles.DragHandle.Draw(rect, false, false, false, false);
                    }
                }
                this.m_TreeView.OnFilterGUI(r);
                if (this.m_TreeView.IsFilteredDirty())
                {
                    this.m_TreeView.Reload();
                }
                Profiler.EndSample();
            }
        }

        public void OnHierarchyChange()
        {
            if (((this.m_DataStore != null) && this.m_DataStore.Repopulate()) && (this.m_TreeView != null))
            {
                this.m_TreeView.FullReload();
            }
        }

        public void OnInspectorUpdate()
        {
            if (((this.m_DataStore != null) && this.m_DataStore.Repopulate()) && (this.m_TreeView != null))
            {
                this.m_TreeView.FullReload();
            }
            else if ((this.m_TreeView != null) && this.m_TreeView.Update())
            {
                this.m_TreeView.Repaint();
            }
        }

        public void OnSelectionChange()
        {
            this.OnSelectionChange(Selection.instanceIDs);
        }

        public void OnSelectionChange(int[] instanceIDs)
        {
            if (this.m_TreeView != null)
            {
                this.m_TreeView.SetSelection(instanceIDs);
            }
        }

        public bool dragHandleEnabled
        {
            get => 
                this.m_DragHandleEnabled;
            set
            {
                this.m_DragHandleEnabled = value;
            }
        }

        internal delegate SerializedPropertyTreeView.Column[] HeaderDelegate(out string[] propNames);

        private static class Styles
        {
            public static readonly GUIStyle DragHandle = "RL DragHandle";
        }
    }
}

