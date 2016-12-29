namespace UnityEditor
{
    using System;
    using UnityEngine;

    /// <summary>
    /// <para>Is the default sorting method used by the hierarchy.</para>
    /// </summary>
    [Obsolete("BaseHierarchySort is no longer supported because of performance reasons")]
    public class TransformSort : BaseHierarchySort
    {
        private readonly GUIContent m_Content = new GUIContent(EditorGUIUtility.FindTexture("DefaultSorting"), "Transform Child Order");

        /// <summary>
        /// <para>Content to visualize the transform sorting method.</para>
        /// </summary>
        public override GUIContent content =>
            this.m_Content;
    }
}

