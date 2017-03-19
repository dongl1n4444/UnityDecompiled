namespace UnityEditor
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// <para>Tells an Editor class which run-time type it's an editor for.</para>
    /// </summary>
    public sealed class CustomEditor : Attribute
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <isFallback>k__BackingField;
        internal bool m_EditorForChildClasses;
        internal System.Type m_InspectedType;

        /// <summary>
        /// <para>Defines which object type the custom editor class can edit.</para>
        /// </summary>
        /// <param name="inspectedType">Type that this editor can edit.</param>
        /// <param name="editorForChildClasses">If true, child classes of inspectedType will also show this editor. Defaults to false.</param>
        public CustomEditor(System.Type inspectedType)
        {
            if (inspectedType == null)
            {
                UnityEngine.Debug.LogError("Failed to load CustomEditor inspected type");
            }
            this.m_InspectedType = inspectedType;
            this.m_EditorForChildClasses = false;
        }

        /// <summary>
        /// <para>Defines which object type the custom editor class can edit.</para>
        /// </summary>
        /// <param name="inspectedType">Type that this editor can edit.</param>
        /// <param name="editorForChildClasses">If true, child classes of inspectedType will also show this editor. Defaults to false.</param>
        public CustomEditor(System.Type inspectedType, bool editorForChildClasses)
        {
            if (inspectedType == null)
            {
                UnityEngine.Debug.LogError("Failed to load CustomEditor inspected type");
            }
            this.m_InspectedType = inspectedType;
            this.m_EditorForChildClasses = editorForChildClasses;
        }

        /// <summary>
        /// <para>If true, match this editor only if all non-fallback editors do not match. Defaults to false.</para>
        /// </summary>
        public bool isFallback { get; set; }
    }
}

