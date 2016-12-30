namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>The trail renderer is used to make trails behind objects in the scene as they move about.</para>
    /// </summary>
    public sealed class TrailRenderer : Renderer
    {
        /// <summary>
        /// <para>Removes all points from the TrailRenderer.
        /// Useful for restarting a trail from a new position.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void Clear();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_endColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_startColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_endColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_startColor(ref Color value);

        /// <summary>
        /// <para>Does the GameObject of this trail renderer auto destructs?</para>
        /// </summary>
        public bool autodestruct { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Set the color gradient describing the color of the trail at various points along its length.</para>
        /// </summary>
        public Gradient colorGradient { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Set the color at the end of the trail.</para>
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
        /// <para>The width of the trail at the end of the trail.</para>
        /// </summary>
        public float endWidth { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Set the minimum distance the trail can travel before a new vertex is added to it.</para>
        /// </summary>
        public float minVertexDistance { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Set this to a value greater than 0, to get rounded corners on each end of the trail.</para>
        /// </summary>
        public int numCapVertices { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Set this to a value greater than 0, to get rounded corners between each segment of the trail.</para>
        /// </summary>
        public int numCornerVertices { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Set the color at the start of the trail.</para>
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
        /// <para>The width of the trail at the spawning point.</para>
        /// </summary>
        public float startWidth { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Choose whether the U coordinate of the trail texture is tiled or stretched.</para>
        /// </summary>
        public LineTextureMode textureMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>How long does the trail take to fade out.</para>
        /// </summary>
        public float time { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Set the curve describing the width of the trail at various points along its length.</para>
        /// </summary>
        public AnimationCurve widthCurve { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Set an overall multiplier that is applied to the TrailRenderer.widthCurve to get the final width of the trail.</para>
        /// </summary>
        public float widthMultiplier { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

