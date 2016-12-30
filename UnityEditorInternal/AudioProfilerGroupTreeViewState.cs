namespace UnityEditorInternal
{
    using System;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    internal class AudioProfilerGroupTreeViewState : TreeViewState
    {
        [SerializeField]
        public float[] columnWidths;
        [SerializeField]
        public int prevSelectedColumn = 5;
        [SerializeField]
        public int selectedColumn = 3;
        [SerializeField]
        public bool sortByDescendingOrder = true;

        public void SetSelectedColumn(int index)
        {
            if (index != this.selectedColumn)
            {
                this.prevSelectedColumn = this.selectedColumn;
            }
            else
            {
                this.sortByDescendingOrder = !this.sortByDescendingOrder;
            }
            this.selectedColumn = index;
        }
    }
}

