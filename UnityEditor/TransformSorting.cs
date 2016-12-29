namespace UnityEditor
{
    using UnityEngine;

    internal class TransformSorting : HierarchySorting
    {
        private readonly GUIContent m_Content = new GUIContent(EditorGUIUtility.FindTexture("DefaultSorting"), "Transform Child Order");

        public override GUIContent content =>
            this.m_Content;
    }
}

