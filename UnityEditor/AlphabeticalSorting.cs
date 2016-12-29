namespace UnityEditor
{
    using UnityEngine;

    internal class AlphabeticalSorting : HierarchySorting
    {
        private readonly GUIContent m_Content = new GUIContent(EditorGUIUtility.FindTexture("AlphabeticalSorting"), "Alphabetical Order");

        public override GUIContent content =>
            this.m_Content;
    }
}

