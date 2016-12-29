namespace UnityEditor
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// <para>Class used to implement content for a popup window.</para>
    /// </summary>
    public abstract class PopupWindowContent
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private EditorWindow <editorWindow>k__BackingField;

        protected PopupWindowContent()
        {
        }

        /// <summary>
        /// <para>The size of the popup window.</para>
        /// </summary>
        /// <returns>
        /// <para>The size of the Popup window.</para>
        /// </returns>
        public virtual Vector2 GetWindowSize() => 
            new Vector2(200f, 200f);

        /// <summary>
        /// <para>Callback when the popup window is closed.</para>
        /// </summary>
        public virtual void OnClose()
        {
        }

        /// <summary>
        /// <para>Callback for drawing GUI controls for the popup window.</para>
        /// </summary>
        /// <param name="rect">The rectangle to draw the GUI inside.</param>
        public abstract void OnGUI(Rect rect);
        /// <summary>
        /// <para>Callback when the popup window is opened.</para>
        /// </summary>
        public virtual void OnOpen()
        {
        }

        /// <summary>
        /// <para>The EditorWindow that contains the popup content.</para>
        /// </summary>
        public EditorWindow editorWindow { get; internal set; }
    }
}

