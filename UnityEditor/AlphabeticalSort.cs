namespace UnityEditor
{
    using System;
    using UnityEngine;

    /// <summary>
    /// <para>Hierarchy sort method to allow for items and their children to be sorted alphabetically.</para>
    /// </summary>
    [Obsolete("BaseHierarchySort is no longer supported because of performance reasons")]
    public class AlphabeticalSort : BaseHierarchySort
    {
        private readonly GUIContent m_Content = new GUIContent(EditorGUIUtility.FindTexture("AlphabeticalSorting"), "Alphabetical Order");

        /// <summary>
        /// <para>Content to visualize the alphabetical sorting method.</para>
        /// </summary>
        public override GUIContent content
        {
            get
            {
                return this.m_Content;
            }
        }
    }
}

