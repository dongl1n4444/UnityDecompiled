namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Script interface for a.</para>
    /// </summary>
    public sealed class LensFlare : Behaviour
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_color(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_color(ref Color value);

        /// <summary>
        /// <para>The strength of the flare.</para>
        /// </summary>
        public float brightness { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The color of the flare.</para>
        /// </summary>
        public Color color
        {
            get
            {
                Color color;
                this.INTERNAL_get_color(out color);
                return color;
            }
            set
            {
                this.INTERNAL_set_color(ref value);
            }
        }

        /// <summary>
        /// <para>The fade speed of the flare.</para>
        /// </summary>
        public float fadeSpeed { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The to use.</para>
        /// </summary>
        public Flare flare { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

