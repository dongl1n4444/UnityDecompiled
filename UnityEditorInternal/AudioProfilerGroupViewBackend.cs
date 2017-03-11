namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    internal class AudioProfilerGroupViewBackend
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private List<AudioProfilerGroupInfoWrapper> <items>k__BackingField;
        public AudioProfilerGroupTreeViewState m_TreeViewState;
        public DataUpdateDelegate OnUpdate;

        public AudioProfilerGroupViewBackend(AudioProfilerGroupTreeViewState state)
        {
            this.m_TreeViewState = state;
            this.items = new List<AudioProfilerGroupInfoWrapper>();
        }

        public void SetData(List<AudioProfilerGroupInfoWrapper> data)
        {
            this.items = data;
            this.UpdateSorting();
        }

        public void UpdateSorting()
        {
            this.items.Sort(new AudioProfilerGroupInfoHelper.AudioProfilerGroupInfoComparer((AudioProfilerGroupInfoHelper.ColumnIndices) this.m_TreeViewState.selectedColumn, (AudioProfilerGroupInfoHelper.ColumnIndices) this.m_TreeViewState.prevSelectedColumn, this.m_TreeViewState.sortByDescendingOrder));
            if (this.OnUpdate != null)
            {
                this.OnUpdate();
            }
        }

        public List<AudioProfilerGroupInfoWrapper> items { get; private set; }

        public delegate void DataUpdateDelegate();
    }
}

