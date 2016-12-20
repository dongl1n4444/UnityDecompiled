namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>(Legacy Particles) Renders particles on to the screen.</para>
    /// </summary>
    [Obsolete("This component is part of the legacy particle system, which is deprecated and will be removed in a future release. Use the ParticleSystem component instead.", false)]
    public sealed class ParticleRenderer : Renderer
    {
        [Obsolete("animatedTextureCount has been replaced by uvAnimationXTile and uvAnimationYTile.")]
        public int animatedTextureCount
        {
            get
            {
                return this.uvAnimationXTile;
            }
            set
            {
                this.uvAnimationXTile = value;
            }
        }

        /// <summary>
        /// <para>How much are the particles strected depending on the Camera's speed.</para>
        /// </summary>
        public float cameraVelocityScale { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [Obsolete("This function has been removed.", true)]
        public AnimationCurve heightCurve
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        /// <summary>
        /// <para>How much are the particles stretched in their direction of motion.</para>
        /// </summary>
        public float lengthScale { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Clamp the maximum particle size.</para>
        /// </summary>
        public float maxParticleSize { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        public float maxPartileSize
        {
            get
            {
                return this.maxParticleSize;
            }
            set
            {
                this.maxParticleSize = value;
            }
        }

        /// <summary>
        /// <para>How particles are drawn.</para>
        /// </summary>
        public ParticleRenderMode particleRenderMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [Obsolete("This function has been removed.", true)]
        public AnimationCurve rotationCurve
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        /// <summary>
        /// <para>Set uv animation cycles.</para>
        /// </summary>
        public float uvAnimationCycles { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Set horizontal tiling count.</para>
        /// </summary>
        public int uvAnimationXTile { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Set vertical tiling count.</para>
        /// </summary>
        public int uvAnimationYTile { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        public Rect[] uvTiles { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>How much are the particles strectched depending on "how fast they move".</para>
        /// </summary>
        public float velocityScale { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [Obsolete("This function has been removed.", true)]
        public AnimationCurve widthCurve
        {
            get
            {
                return null;
            }
            set
            {
            }
        }
    }
}

