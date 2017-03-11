namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Renders a billboard from a BillboardAsset.</para>
    /// </summary>
    public sealed class BillboardRenderer : Renderer
    {
        /// <summary>
        /// <para>The BillboardAsset to render.</para>
        /// </summary>
        public BillboardAsset billboard { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

