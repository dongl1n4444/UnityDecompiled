namespace UnityEditor.TreeViewExamples
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal class TreeViewColumnHeader
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Action<int, Rect> <columnRenderer>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float[] <columnWidths>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float <dragWidth>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private float <minColumnWidth>k__BackingField;

        public TreeViewColumnHeader()
        {
            this.minColumnWidth = 10f;
            this.dragWidth = 6f;
        }

        public void OnGUI(Rect rect)
        {
            float x = rect.x;
            for (int i = 0; i < this.columnWidths.Length; i++)
            {
                Rect rect2 = new Rect(x, rect.y, this.columnWidths[i], rect.height);
                x += this.columnWidths[i];
                Rect position = new Rect(x - (this.dragWidth / 2f), rect.y, 3f, rect.height);
                float num3 = EditorGUI.MouseDeltaReader(position, true).x;
                if (num3 != 0f)
                {
                    this.columnWidths[i] += num3;
                    this.columnWidths[i] = Mathf.Max(this.columnWidths[i], this.minColumnWidth);
                }
                if (this.columnRenderer != null)
                {
                    this.columnRenderer.Invoke(i, rect2);
                }
                if (Event.current.type == EventType.Repaint)
                {
                    EditorGUIUtility.AddCursorRect(position, MouseCursor.SplitResizeLeftRight);
                }
            }
        }

        public Action<int, Rect> columnRenderer { get; set; }

        public float[] columnWidths { get; set; }

        public float dragWidth { get; set; }

        public float minColumnWidth { get; set; }
    }
}

