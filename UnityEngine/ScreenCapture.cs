namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Functionality to take Screenshots.</para>
    /// </summary>
    public static class ScreenCapture
    {
        [ExcludeFromDocs]
        public static void CaptureScreenshot(string filename)
        {
            int superSize = 0;
            CaptureScreenshot(filename, superSize);
        }

        /// <summary>
        /// <para>Captures a screenshot at path filename as a PNG file.</para>
        /// </summary>
        /// <param name="filename">Pathname to save the screenshot file to.</param>
        /// <param name="superSize">Factor by which to increase resolution.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void CaptureScreenshot(string filename, [DefaultValue("0")] int superSize);
    }
}

