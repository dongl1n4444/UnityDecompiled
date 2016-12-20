namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Mac fullscreen mode.</para>
    /// </summary>
    public enum MacFullscreenMode
    {
        [Obsolete("Capture Display mode is deprecated, Use FullscreenWindow instead")]
        CaptureDisplay = 0,
        /// <summary>
        /// <para>Fullscreen window.</para>
        /// </summary>
        FullscreenWindow = 1,
        /// <summary>
        /// <para>Fullscreen window with Dock and Menu bar.</para>
        /// </summary>
        FullscreenWindowWithDockAndMenuBar = 2
    }
}

