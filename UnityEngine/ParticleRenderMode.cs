namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>The rendering mode for legacy particles.</para>
    /// </summary>
    [Obsolete("This is part of the legacy particle system, which is deprecated and will be removed in a future release. Use the ParticleSystem component instead.", false)]
    public enum ParticleRenderMode
    {
        /// <summary>
        /// <para>Render the particles as billboards facing the player. (Default)</para>
        /// </summary>
        Billboard = 0,
        /// <summary>
        /// <para>Render the particles as billboards always facing up along the y-Axis.</para>
        /// </summary>
        HorizontalBillboard = 4,
        /// <summary>
        /// <para>Sort the particles back-to-front and render as billboards.</para>
        /// </summary>
        SortedBillboard = 2,
        /// <summary>
        /// <para>Stretch particles in the direction of motion.</para>
        /// </summary>
        Stretch = 3,
        /// <summary>
        /// <para>Render the particles as billboards always facing the player, but not pitching along the x-Axis.</para>
        /// </summary>
        VerticalBillboard = 5
    }
}

