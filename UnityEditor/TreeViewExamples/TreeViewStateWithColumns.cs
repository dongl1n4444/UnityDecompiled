namespace UnityEditor.TreeViewExamples
{
    using System;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    internal class TreeViewStateWithColumns : TreeViewState
    {
        [SerializeField]
        public float[] columnWidths;
    }
}

