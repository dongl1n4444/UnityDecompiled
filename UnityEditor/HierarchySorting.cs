namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal abstract class HierarchySorting
    {
        protected HierarchySorting()
        {
        }

        public virtual GUIContent content
        {
            get
            {
                return null;
            }
        }
    }
}

