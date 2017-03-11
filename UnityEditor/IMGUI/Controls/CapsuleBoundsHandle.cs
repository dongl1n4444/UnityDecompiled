namespace UnityEditor.IMGUI.Controls
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// <para>A compound handle to edit a capsule-shaped bounding volume in the Scene view.</para>
    /// </summary>
    public class CapsuleBoundsHandle : PrimitiveBoundsHandle
    {
        private const int k_DirectionX = 0;
        private const int k_DirectionY = 1;
        private const int k_DirectionZ = 2;
        private int m_HeightAxis;
        private static readonly Vector3[] s_HeightAxes = new Vector3[] { Vector3.right, Vector3.up, Vector3.forward };
        private static readonly int[] s_NextAxis = new int[] { 1, 2, 0 };

        /// <summary>
        /// <para>Create a new instance of the CapsuleBoundsHandle class.</para>
        /// </summary>
        /// <param name="controlIDHint">An integer value used to generate consistent control IDs for each control handle on this instance. You may use any value you like, but should avoid using the same value for all of your PrimitiveBoundsHandle instances.</param>
        public CapsuleBoundsHandle(int controlIDHint) : base(controlIDHint)
        {
            this.m_HeightAxis = 1;
        }

        /// <summary>
        /// <para>Draw a wireframe capsule for this instance.</para>
        /// </summary>
        protected override void DrawWireframe()
        {
            HeightAxis y = HeightAxis.Y;
            HeightAxis z = HeightAxis.Z;
            switch (this.heightAxis)
            {
                case HeightAxis.Y:
                    y = HeightAxis.Z;
                    z = HeightAxis.X;
                    break;

                case HeightAxis.Z:
                    y = HeightAxis.X;
                    z = HeightAxis.Y;
                    break;
            }
            bool flag = base.IsAxisEnabled((int) this.heightAxis);
            bool flag2 = base.IsAxisEnabled((int) y);
            bool flag3 = base.IsAxisEnabled((int) z);
            Vector3 normal = s_HeightAxes[this.m_HeightAxis];
            Vector3 vector2 = s_HeightAxes[s_NextAxis[this.m_HeightAxis]];
            Vector3 from = s_HeightAxes[s_NextAxis[s_NextAxis[this.m_HeightAxis]]];
            float radius = this.radius;
            float height = this.height;
            Vector3 center = base.center + ((Vector3) (normal * ((height * 0.5f) - radius)));
            Vector3 vector5 = base.center - ((Vector3) (normal * ((height * 0.5f) - radius)));
            if (flag)
            {
                if (flag3)
                {
                    Handles.DrawWireArc(center, vector2, from, 180f, radius);
                    Handles.DrawWireArc(vector5, vector2, from, -180f, radius);
                    Handles.DrawLine(center + ((Vector3) (from * radius)), vector5 + ((Vector3) (from * radius)));
                    Handles.DrawLine(center - ((Vector3) (from * radius)), vector5 - ((Vector3) (from * radius)));
                }
                if (flag2)
                {
                    Handles.DrawWireArc(center, from, vector2, -180f, radius);
                    Handles.DrawWireArc(vector5, from, vector2, 180f, radius);
                    Handles.DrawLine(center + ((Vector3) (vector2 * radius)), vector5 + ((Vector3) (vector2 * radius)));
                    Handles.DrawLine(center - ((Vector3) (vector2 * radius)), vector5 - ((Vector3) (vector2 * radius)));
                }
            }
            if (flag2 && flag3)
            {
                Handles.DrawWireArc(center, normal, vector2, 360f, radius);
                Handles.DrawWireArc(vector5, normal, vector2, -360f, radius);
            }
        }

        private bool GetRadiusAxis(out int radiusAxis)
        {
            radiusAxis = s_NextAxis[this.m_HeightAxis];
            if (!base.IsAxisEnabled(radiusAxis))
            {
                radiusAxis = s_NextAxis[radiusAxis];
                return false;
            }
            return base.IsAxisEnabled(s_NextAxis[radiusAxis]);
        }

        protected override Bounds OnHandleChanged(PrimitiveBoundsHandle.HandleDirection handle, Bounds boundsOnClick, Bounds newBounds)
        {
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
            Vector3 max = newBounds.max;
            Vector3 min = newBounds.min;
            if (num == this.m_HeightAxis)
            {
                int num2;
                this.GetRadiusAxis(out num2);
                float num3 = max[num2] - min[num2];
                float num4 = max[this.m_HeightAxis] - min[this.m_HeightAxis];
                if (num4 < num3)
                {
                    if (((handle == PrimitiveBoundsHandle.HandleDirection.PositiveX) || (handle == PrimitiveBoundsHandle.HandleDirection.PositiveY)) || (handle == PrimitiveBoundsHandle.HandleDirection.PositiveZ))
                    {
                        max[this.m_HeightAxis] = min[this.m_HeightAxis] + num3;
                    }
                    else
                    {
                        min[this.m_HeightAxis] = max[this.m_HeightAxis] - num3;
                    }
                }
            }
            else
            {
                max[this.m_HeightAxis] = boundsOnClick.center[this.m_HeightAxis] + (0.5f * boundsOnClick.size[this.m_HeightAxis]);
                min[this.m_HeightAxis] = boundsOnClick.center[this.m_HeightAxis] - (0.5f * boundsOnClick.size[this.m_HeightAxis]);
                float b = 0.5f * (max[num] - min[num]);
                float a = 0.5f * (max[this.m_HeightAxis] - min[this.m_HeightAxis]);
                for (int i = 0; i < 3; i++)
                {
                    if (i != num)
                    {
                        float num8 = (i != this.m_HeightAxis) ? b : Mathf.Max(a, b);
                        min[i] = base.center[i] - num8;
                        max[i] = base.center[i] + num8;
                    }
                }
            }
            return new Bounds((Vector3) ((max + min) * 0.5f), max - min);
        }

        /// <summary>
        /// <para>Gets or sets the height of the capsule bounding volume.</para>
        /// </summary>
        public float height
        {
            get => 
                (base.IsAxisEnabled(this.m_HeightAxis) ? Mathf.Max(base.GetSize()[this.m_HeightAxis], 2f * this.radius) : 0f);
            set
            {
                value = Mathf.Max(Mathf.Abs(value), 2f * this.radius);
                if (this.height != value)
                {
                    Vector3 size = base.GetSize();
                    size[this.m_HeightAxis] = value;
                    base.SetSize(size);
                }
            }
        }

        /// <summary>
        /// <para>Gets or sets the axis in the handle's space to which height maps. The radius maps to the remaining axes.</para>
        /// </summary>
        public HeightAxis heightAxis
        {
            get => 
                ((HeightAxis) this.m_HeightAxis);
            set
            {
                int num = (int) value;
                if (this.m_HeightAxis != num)
                {
                    Vector3 size = (Vector3) ((Vector3.one * this.radius) * 2f);
                    size[num] = base.GetSize()[this.m_HeightAxis];
                    this.m_HeightAxis = num;
                    base.SetSize(size);
                }
            }
        }

        /// <summary>
        /// <para>Gets or sets the radius of the capsule bounding volume.</para>
        /// </summary>
        public float radius
        {
            get
            {
                int num;
                if (this.GetRadiusAxis(out num) || base.IsAxisEnabled(this.m_HeightAxis))
                {
                    return (0.5f * base.GetSize()[num]);
                }
                return 0f;
            }
            set
            {
                Vector3 size = base.GetSize();
                float b = 2f * value;
                for (int i = 0; i < 3; i++)
                {
                    size[i] = (i != this.m_HeightAxis) ? b : Mathf.Max(size[i], b);
                }
                base.SetSize(size);
            }
        }

        /// <summary>
        /// <para>An enumeration for specifying which axis on a CapsuleBoundsHandle object maps to the CapsuleBoundsHandle.height parameter.</para>
        /// </summary>
        public enum HeightAxis
        {
            X,
            Y,
            Z
        }
    }
}

