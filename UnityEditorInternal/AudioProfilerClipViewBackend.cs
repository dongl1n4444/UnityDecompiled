namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    internal class AudioProfilerClipViewBackend
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private List<AudioProfilerClipInfoWrapper> <items>k__BackingField;
        public AudioProfilerClipTreeViewState m_TreeViewState;
        public DataUpdateDelegate OnUpdate;

        public AudioProfilerClipViewBackend(AudioProfilerClipTreeViewState state)
        {
            this.m_TreeViewState = state;
            this.items = new List<AudioProfilerClipInfoWrapper>();
        }

        public void SetData(List<AudioProfilerClipInfoWrapper> data)
        {
            this.items = data;
            this.UpdateSorting();
        }

        public void UpdateSorting()
        {
            this.items.Sort(new AudioProfilerClipInfoHelper.AudioProfilerClipInfoComparer((AudioProfilerClipInfoHelper.ColumnIndices) this.m_TreeViewState.selectedColumn, (AudioProfilerClipInfoHelper.ColumnIndices) this.m_TreeViewState.prevSelectedColumn, this.m_TreeViewState.sortByDescendingOrder));
            if (this.OnUpdate != null)
            {
                this.OnUpdate();
            }
        }

        public List<AudioProfilerClipInfoWrapper> items { get; private set; }

        public delegate void DataUpdateDelegate();
    }
}

