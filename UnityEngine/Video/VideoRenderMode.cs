namespace UnityEngine.Video
{
    using System;

    /// <summary>
    /// <para>Type of destination for the images read by a VideoPlayer.</para>
    /// </summary>
    public enum VideoRenderMode
    {
        CameraFarPlane,
        CameraNearPlane,
        RenderTexture,
        MaterialOverride,
        APIOnly
    }
}

