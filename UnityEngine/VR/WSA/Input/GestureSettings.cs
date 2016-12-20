namespace UnityEngine.VR.WSA.Input
{
    using System;

    /// <summary>
    /// <para>This enumeration represents the set of gestures that may be recognized by GestureRecognizer.</para>
    /// </summary>
    public enum GestureSettings
    {
        /// <summary>
        /// <para>Enable support for the double-tap gesture.</para>
        /// </summary>
        DoubleTap = 2,
        /// <summary>
        /// <para>Enable support for the hold gesture.</para>
        /// </summary>
        Hold = 4,
        /// <summary>
        /// <para>Enable support for the manipulation gesture which tracks changes to the hand's position.  This gesture is relative to the start position of the gesture and measures an absolute movement through the world.</para>
        /// </summary>
        ManipulationTranslate = 8,
        /// <summary>
        /// <para>Enable support for the navigation gesture, in the horizontal axis using rails (guides).</para>
        /// </summary>
        NavigationRailsX = 0x80,
        /// <summary>
        /// <para>Enable support for the navigation gesture, in the vertical axis using rails (guides).</para>
        /// </summary>
        NavigationRailsY = 0x100,
        /// <summary>
        /// <para>Enable support for the navigation gesture, in the depth axis using rails (guides).</para>
        /// </summary>
        NavigationRailsZ = 0x200,
        /// <summary>
        /// <para>Enable support for the navigation gesture, in the horizontal axis.</para>
        /// </summary>
        NavigationX = 0x10,
        /// <summary>
        /// <para>Enable support for the navigation gesture, in the vertical axis.</para>
        /// </summary>
        NavigationY = 0x20,
        /// <summary>
        /// <para>Enable support for the navigation gesture, in the depth axis.</para>
        /// </summary>
        NavigationZ = 0x40,
        /// <summary>
        /// <para>Disable support for gestures.</para>
        /// </summary>
        None = 0,
        /// <summary>
        /// <para>Enable support for the tap gesture.</para>
        /// </summary>
        Tap = 1
    }
}

