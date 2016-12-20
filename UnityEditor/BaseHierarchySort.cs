namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// <para>The base class used to create new sorting.</para>
    /// </summary>
    [Obsolete("BaseHierarchySort is no longer supported because of performance reasons")]
    public abstract class BaseHierarchySort : IComparer<GameObject>
    {
        protected BaseHierarchySort()
        {
        }

        /// <summary>
        /// <para>The sorting method used to determine the order of GameObjects.</para>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public virtual int Compare(GameObject lhs, GameObject rhs)
        {
            return 0;
        }

        /// <summary>
        /// <para>The content to display to quickly identify the hierarchy's mode.</para>
        /// </summary>
        public virtual GUIContent content
        {
            get
            {
                return null;
            }
        }
    }
}

