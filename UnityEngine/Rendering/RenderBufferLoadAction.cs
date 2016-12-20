namespace UnityEngine.Rendering
{
    using System;

    /// <summary>
    /// <para>Handling of loading RenderBuffer contents on setting as active RenderTarget.</para>
    /// </summary>
    public enum RenderBufferLoadAction
    {
        /// <summary>
        /// <para>RenderBuffer will try to skip loading its contents on setting as Render Target.</para>
        /// </summary>
        DontCare = 2,
        /// <summary>
        /// <para>Make RenderBuffer to Load its contents when setting as RenderTarget.</para>
        /// </summary>
        Load = 0
    }
}

