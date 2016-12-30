namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>How the video clip's images will be resized during transcoding.</para>
    /// </summary>
    public enum VideoResizeMode
    {
        OriginalSize,
        ThreeQuarterRes,
        HalfRes,
        QuarterRes,
        Square1024,
        Square512,
        Square256,
        CustomSize
    }
}

