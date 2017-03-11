namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>The line renderer is used to draw free-floating lines in 3D space.</para>
    /// </summary>
    public sealed class LineRenderer : Renderer
    {
        /// <summary>
        /// <para>Get the position of a vertex in the line.</para>
        /// </summary>
        /// <param name="index">The index of the position to retrieve.</param>
        /// <returns>
        /// <para>The position at the specified index in the array.</para>
        /// </returns>
        public Vector3 GetPosition(int index)
        {
            Vector3 vector;
            INTERNAL_CALL_GetPosition(this, index, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Get the positions of all vertices in the line.</para>
        /// </summary>
        /// <param name="positions">The array of positions to retrieve. The array passed should be of at least numPositions in size.</param>
        /// <returns>
        /// <para>How many positions were actually stored in the output array.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern int GetPositions(Vector3[] positions);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetPosition(LineRenderer self, int index, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetPosition(LineRenderer self, int index, ref Vector3 position);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_endColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_startColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_endColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_startColor(ref Color value);
        /// <summary>
        /// <para>Set the line color at the start and at the end.</para>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        [Obsolete("Use startColor, endColor or colorGradient instead.", false)]
        public void SetColors(Color start, Color end)
        {
            this.startColor = start;
            this.endColor = end;
        }

        /// <summary>
        /// <para>Set the position of a vertex in the line.</para>
        /// </summary>
        /// <param name="index">Which position to set.</param>
        /// <param name="position">The new position.</param>
        public void SetPosition(int index, Vector3 position)
        {
            INTERNAL_CALL_SetPosition(this, index, ref position);
        }

        /// <summary>
        /// <para>Set the positions of all vertices in the line.</para>
        /// </summary>
        /// <param name="positions">The array of positions to set.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetPositions(Vector3[] positions);
        /// <summary>
        /// <para>Set the number of line segments.</para>
        /// </summary>
        /// <param name="count"></param>
        [Obsolete("Use numPositions instead.", false)]
        public void SetVertexCount(int count)
        {
            this.numPositions = count;
        }

        /// <summary>
        /// <para>Set the line width at the start and at the end.</para>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        [Obsolete("Use startWidth, endWidth or widthCurve instead.", false)]
        public void SetWidth(float start, float end)
        {
            this.startWidth = start;
            this.endWidth = end;
        }

        /// <summary>
        /// <para>Select whether the line will face the camera, or the orientation of the Transform Component.</para>
        /// </summary>
        public LineAlignment alignment { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Set the color gradient describing the color of the line at various points along its length.</para>
        /// </summary>
        public Gradient colorGradient { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Set the color at the end of the line.</para>
        /// </summary>
        public Color endColor
        {
            get
            {
                Color color;
                this.INTERNAL_get_endColor(out color);
                return color;
            }
            set
            {
                this.INTERNAL_set_endColor(ref value);
            }
        }

        /// <summary>
        /// <para>Set the width at the end of the line.</para>
        /// </summary>
        public float endWidth { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Connect the start and end positions of the line together to form a continuous loop.</para>
        /// </summary>
        public bool loop { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Set this to a value greater than 0, to get rounded corners on each end of the line.</para>
        /// </summary>
        public int numCapVertices { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Set this to a value greater than 0, to get rounded corners between each segment of the line.</para>
        /// </summary>
        public int numCornerVertices { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Set the number of line segments.</para>
        /// </summary>
        [Obsolete("Use positionCount property (UnityUpgradable) -> positionCount")]
        public int numPositions { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Set the number of line segments.</para>
        /// </summary>
        public int positionCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Set the color at the start of the line.</para>
        /// </summary>
        public Color startColor
        {
            get
            {
                Color color;
                this.INTERNAL_get_startColor(out color);
                return color;
            }
            set
            {
                this.INTERNAL_set_startColor(ref value);
            }
        }

        /// <summary>
        /// <para>Set the width at the start of the line.</para>
        /// </summary>
        public float startWidth { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Choose whether the U coordinate of the line texture is tiled or stretched.</para>
        /// </summary>
        public LineTextureMode textureMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>If enabled, the lines are defined in world space.</para>
        /// </summary>
        public bool useWorldSpace { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Set the curve describing the width of the line at various points along its length.</para>
        /// </summary>
        public AnimationCurve widthCurve { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Set an overall multiplier that is applied to the LineRenderer.widthCurve to get the final width of the line.</para>
        /// </summary>
        public float widthMultiplier { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

