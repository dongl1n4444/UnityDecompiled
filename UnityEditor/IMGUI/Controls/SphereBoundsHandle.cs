namespace UnityEditor.IMGUI.Controls
{
    using System;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// <para>A compound handle to edit a sphere-shaped bounding volume in the Scene view.</para>
    /// </summary>
    public class SphereBoundsHandle : PrimitiveBoundsHandle
    {
        /// <summary>
        /// <para>Create a new instance of the SphereBoundsHandle class.</para>
        /// </summary>
        /// <param name="controlIDHint">An integer value used to generate consistent control IDs for each control handle on this instance. You may use any value you like, but should avoid using the same value for all of your PrimitiveBoundsHandle instances.</param>
        public SphereBoundsHandle(int controlIDHint) : base(controlIDHint)
        {
        }

        /// <summary>
        /// <para>Draw a wireframe sphere for this instance.</para>
        /// </summary>
        protected override void DrawWireframe()
        {
            bool flag = base.IsAxisEnabled(PrimitiveBoundsHandle.Axes.X);
            bool flag2 = base.IsAxisEnabled(PrimitiveBoundsHandle.Axes.Y);
            bool flag3 = base.IsAxisEnabled(PrimitiveBoundsHandle.Axes.Z);
            if (flag && flag2)
            {
                Handles.DrawWireArc(base.center, Vector3.forward, Vector3.up, 360f, this.radius);
            }
            if (flag && flag3)
            {
                Handles.DrawWireArc(base.center, Vector3.up, Vector3.right, 360f, this.radius);
            }
            if (flag2 && flag3)
            {
                Handles.DrawWireArc(base.center, Vector3.right, Vector3.forward, 360f, this.radius);
            }
        }

        protected override Bounds OnHandleChanged(PrimitiveBoundsHandle.HandleDirection handle, Bounds boundsOnClick, Bounds newBounds)
        {
            Vector3 max = newBounds.max;
            Vector3 min = newBounds.min;
            int num = 0;
            switch (handle)
            {
                case PrimitiveBoundsHandle.HandleDirection.PositiveY:
                case PrimitiveBoundsHandle.HandleDirection.NegativeY:
                    num = 1;
                    break;

                case PrimitiveBoundsHandle.HandleDirection.PositiveZ:
                case PrimitiveBoundsHandle.HandleDirection.NegativeZ:
                    num = 2;
                    break;
            }
            float num2 = 0.5f * (max[num] - min[num]);
            for (int i = 0; i < 3; i++)
            {
                if (i != num)
                {
                    min[i] = base.center[i] - num2;
                    max[i] = base.center[i] + num2;
                }
            }
            return new Bounds((Vector3) ((max + min) * 0.5f), max - min);
        }

        /// <summary>
        /// <para>Gets or sets the radius of the capsule bounding volume.</para>
        /// </summary>
        public float radius
        {
            get
            {
                Vector3 size = base.GetSize();
                float a = 0f;
                for (int i = 0; i < 3; i++)
                {
                    if (base.IsAxisEnabled(i))
                    {
                        a = Mathf.Max(a, Mathf.Abs(size[i]));
                    }
                }
                return (a * 0.5f);
            }
            set
            {
                base.SetSize((Vector3) ((2f * value) * Vector3.one));
            }
        }
    }
}

