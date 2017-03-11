namespace UnityEngine.Experimental.Rendering
{
    using System;

    /// <summary>
    /// <para>Flags controlling RenderLoop.DrawRenderers.</para>
    /// </summary>
    [Flags]
    public enum DrawRendererFlags
    {
        None,
        EnableDynamicBatching,
        EnableInstancing
    }
}

