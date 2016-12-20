namespace UnityEditor
{
    using UnityEngine;

    internal class AlphabeticalSorting : HierarchySorting
    {
        private readonly GUIContent m_Content = new GUIContent(EditorGUIUtility.FindTexture("AlphabeticalSorting"), "Alphabetical Order");

        public override GUIContent content
        {
            get
            {
                return this.m_Content;
            }
        }
    }
}

