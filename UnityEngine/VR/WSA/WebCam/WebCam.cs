namespace UnityEngine.VR.WSA.WebCam
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Contains general information about the current state of the web camera.</para>
    /// </summary>
    public static class WebCam
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int GetWebCamModeState_Internal();

        /// <summary>
        /// <para>Specifies what mode the Web Camera is currently in.</para>
        /// </summary>
        public static WebCamMode Mode
        {
            get
            {
                return (WebCamMode) GetWebCamModeState_Internal();
            }
        }
    }
}

