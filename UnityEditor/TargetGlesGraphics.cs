namespace UnityEditor
{
    using System;

    [Obsolete("TargetGlesGraphics is ignored, use SetGraphicsAPIs/GetGraphicsAPIs APIs", false)]
    public enum TargetGlesGraphics
    {
        Automatic = -1,
        OpenGLES_1_x = 0,
        OpenGLES_2_0 = 1,
        OpenGLES_3_0 = 2
    }
}

