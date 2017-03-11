namespace UnityEditor.IMGUI.Controls
{
    using System;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// <para>A compound handle to edit a box-shaped bounding volume in the Scene view.</para>
    /// </summary>
    public class BoxBoundsHandle : PrimitiveBoundsHandle
    {
        /// <summary>
        /// <para>Create a new instance of the BoxBoundsHandle class.</para>
        /// </summary>
        /// <param name="controlIDHint">An integer value used to generate consistent control IDs for each control handle on this instance. You may use any value you like, but should avoid using the same value for all of your PrimitiveBoundsHandle instances.</param>
        public BoxBoundsHandle(int controlIDHint) : base(controlIDHint)
        {
        }

        /// <summary>
        /// <para>Draw a wireframe box for this instance.</para>
        /// </summary>
        protected override void DrawWireframe()
        {
            Handles.DrawWireCube(base.center, this.size);
        }

        /// <summary>
        /// <para>Gets or sets the size of the bounding box.</para>
        /// </summary>
        public Vector3 size
        {
            get => 
                base.GetSize();
            set
            {
                base.SetSize(value);
            }
        }
    }
}

