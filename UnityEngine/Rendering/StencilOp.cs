namespace UnityEngine.Rendering
{
    using System;

    /// <summary>
    /// <para>Specifies the operation that's performed on the stencil buffer when rendering.</para>
    /// </summary>
    public enum StencilOp
    {
        Keep,
        Zero,
        Replace,
        IncrementSaturate,
        DecrementSaturate,
        Invert,
        IncrementWrap,
        DecrementWrap
    }
}

