namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>Describes screen orientation.</para>
    /// </summary>
    public enum ScreenOrientation
    {
        /// <summary>
        /// <para>Auto-rotates the screen as necessary toward any of the enabled orientations.</para>
        /// </summary>
        AutoRotation = 5,
        Landscape = 3,
        /// <summary>
        /// <para>Landscape orientation, counter-clockwise from the portrait orientation.</para>
        /// </summary>
        LandscapeLeft = 3,
        /// <summary>
        /// <para>Landscape orientation, clockwise from the portrait orientation.</para>
        /// </summary>
        LandscapeRight = 4,
        /// <summary>
        /// <para>Portrait orientation.</para>
        /// </summary>
        Portrait = 1,
        /// <summary>
        /// <para>Portrait orientation, upside down.</para>
        /// </summary>
        PortraitUpsideDown = 2,
        Unknown = 0
    }
}

