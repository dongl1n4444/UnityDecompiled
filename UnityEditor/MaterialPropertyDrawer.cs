namespace UnityEditor
{
    using System;
    using UnityEngine;

    /// <summary>
    /// <para>Base class to derive custom material property drawers from.</para>
    /// </summary>
    public abstract class MaterialPropertyDrawer
    {
        protected MaterialPropertyDrawer()
        {
        }

        /// <summary>
        /// <para>Apply extra initial values to the material.</para>
        /// </summary>
        /// <param name="prop">The MaterialProperty to apply values for.</param>
        public virtual void Apply(MaterialProperty prop)
        {
        }

        /// <summary>
        /// <para>Override this method to specify how tall the GUI for this property is in pixels.</para>
        /// </summary>
        /// <param name="prop">The MaterialProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        /// <param name="editor">Current material editor.</param>
        public virtual float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return 16f;
        }

        /// <summary>
        /// <para>Override this method to make your own GUI for the property.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
        /// <param name="prop">The MaterialProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        /// <param name="editor">Current material editor.</param>
        public virtual void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            EditorGUI.LabelField(position, new GUIContent(label), EditorGUIUtility.TempContent("No GUI Implemented"));
        }

        public virtual void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            this.OnGUI(position, prop, label.text, editor);
        }
    }
}

