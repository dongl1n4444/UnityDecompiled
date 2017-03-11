namespace UnityEditor
{
    using System;
    using UnityEngine;

    /// <summary>
    /// <para>Base class to derive custom decorator drawers from.</para>
    /// </summary>
    public abstract class DecoratorDrawer : GUIDrawer
    {
        internal PropertyAttribute m_Attribute;

        protected DecoratorDrawer()
        {
        }

        /// <summary>
        /// <para>Override this method to specify how tall the GUI for this decorator is in pixels.</para>
        /// </summary>
        public virtual float GetHeight() => 
            16f;

        /// <summary>
        /// <para>Override this method to make your own GUI for the decorator.
        /// See DecoratorDrawer for an example of how to use this.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the decorator GUI.</param>
        public virtual void OnGUI(Rect position)
        {
        }

        /// <summary>
        /// <para>The PropertyAttribute for the decorator. (Read Only)</para>
        /// </summary>
        public PropertyAttribute attribute =>
            this.m_Attribute;
    }
}

