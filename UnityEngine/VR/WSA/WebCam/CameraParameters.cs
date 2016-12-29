namespace UnityEngine.VR.WSA.WebCam
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>When calling PhotoCapture.StartPhotoModeAsync, you must pass in a CameraParameters object that contains the various settings that the web camera will use.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CameraParameters
    {
        private float m_HologramOpacity;
        private float m_FrameRate;
        private int m_CameraResolutionWidth;
        private int m_CameraResolutionHeight;
        private CapturePixelFormat m_PixelFormat;
        [CompilerGenerated]
        private static Func<Resolution, int> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<Resolution, int> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<float, float> <>f__am$cache2;
        public CameraParameters(WebCamMode webCamMode)
        {
            this.m_HologramOpacity = 1f;
            this.m_PixelFormat = CapturePixelFormat.BGRA32;
            this.m_FrameRate = 0f;
            this.m_CameraResolutionWidth = 0;
            this.m_CameraResolutionHeight = 0;
            if (webCamMode == WebCamMode.PhotoMode)
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = new Func<Resolution, int>(null, (IntPtr) <CameraParameters>m__0);
                }
                Resolution resolution = Enumerable.OrderByDescending<Resolution, int>(PhotoCapture.SupportedResolutions, <>f__am$cache0).First<Resolution>();
                this.m_CameraResolutionWidth = resolution.width;
                this.m_CameraResolutionHeight = resolution.height;
            }
            else if (webCamMode == WebCamMode.VideoMode)
            {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = new Func<Resolution, int>(null, (IntPtr) <CameraParameters>m__1);
                }
                Resolution resolution2 = Enumerable.OrderByDescending<Resolution, int>(VideoCapture.SupportedResolutions, <>f__am$cache1).First<Resolution>();
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = new Func<float, float>(null, (IntPtr) <CameraParameters>m__2);
                }
                float num = Enumerable.OrderByDescending<float, float>(VideoCapture.GetSupportedFrameRatesForResolution(resolution2), <>f__am$cache2).First<float>();
                this.m_CameraResolutionWidth = resolution2.width;
                this.m_CameraResolutionHeight = resolution2.height;
                this.m_FrameRate = num;
            }
        }

        /// <summary>
        /// <para>The opacity of captured holograms.</para>
        /// </summary>
        public float hologramOpacity
        {
            get => 
                this.m_HologramOpacity;
            set
            {
                this.m_HologramOpacity = value;
            }
        }
        /// <summary>
        /// <para>The framerate at which to capture video.  This is only for use with VideoCapture.</para>
        /// </summary>
        public float frameRate
        {
            get => 
                this.m_FrameRate;
            set
            {
                this.m_FrameRate = value;
            }
        }
        /// <summary>
        /// <para>A valid width resolution for use with the web camera.</para>
        /// </summary>
        public int cameraResolutionWidth
        {
            get => 
                this.m_CameraResolutionWidth;
            set
            {
                this.m_CameraResolutionWidth = value;
            }
        }
        /// <summary>
        /// <para>A valid height resolution for use with the web camera.</para>
        /// </summary>
        public int cameraResolutionHeight
        {
            get => 
                this.m_CameraResolutionHeight;
            set
            {
                this.m_CameraResolutionHeight = value;
            }
        }
        /// <summary>
        /// <para>The pixel format used to capture and record your image data.</para>
        /// </summary>
        public CapturePixelFormat pixelFormat
        {
            get => 
                this.m_PixelFormat;
            set
            {
                this.m_PixelFormat = value;
            }
        }
        [CompilerGenerated]
        private static int <CameraParameters>m__0(Resolution res) => 
            (res.width * res.height);

        [CompilerGenerated]
        private static int <CameraParameters>m__1(Resolution res) => 
            (res.width * res.height);

        [CompilerGenerated]
        private static float <CameraParameters>m__2(float fps) => 
            fps;
    }
}

