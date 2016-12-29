namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Provides access to a display / screen for rendering operations.</para>
    /// </summary>
    [UsedByNativeCode]
    public sealed class Display
    {
        private static Display _mainDisplay = displays[0];
        /// <summary>
        /// <para>The list of currently connected Displays. Contains at least one (main) display.</para>
        /// </summary>
        public static Display[] displays = new Display[] { new Display() };
        internal IntPtr nativeDisplay;

        public static  event DisplaysUpdatedDelegate onDisplaysUpdated;

        internal Display()
        {
            this.nativeDisplay = new IntPtr(0);
        }

        internal Display(IntPtr nativeDisplay)
        {
            this.nativeDisplay = nativeDisplay;
        }

        /// <summary>
        /// <para>Activate an external display. Eg. Secondary Monitors connected to the System.</para>
        /// </summary>
        public void Activate()
        {
            ActivateDisplayImpl(this.nativeDisplay, 0, 0, 60);
        }

        /// <summary>
        /// <para>This overloaded function available for Windows allows specifying desired Window Width, Height and Refresh Rate.</para>
        /// </summary>
        /// <param name="width">Desired Width of the Window (for Windows only. On Linux and Mac uses Screen Width).</param>
        /// <param name="height">Desired Height of the Window (for Windows only. On Linux and Mac uses Screen Height).</param>
        /// <param name="refreshRate">Desired Refresh Rate.</param>
        public void Activate(int width, int height, int refreshRate)
        {
            ActivateDisplayImpl(this.nativeDisplay, width, height, refreshRate);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void ActivateDisplayImpl(IntPtr nativeDisplay, int width, int height, int refreshRate);
        [RequiredByNativeCode]
        private static void FireDisplaysUpdated()
        {
            if (onDisplaysUpdated != null)
            {
                onDisplaysUpdated();
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void GetRenderingBuffersImpl(IntPtr nativeDisplay, out RenderBuffer color, out RenderBuffer depth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void GetRenderingExtImpl(IntPtr nativeDisplay, out int w, out int h);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void GetSystemExtImpl(IntPtr nativeDisplay, out int w, out int h);
        [Obsolete("MultiDisplayLicense has been deprecated.", false)]
        public static bool MultiDisplayLicense() => 
            true;

        [RequiredByNativeCode]
        private static void RecreateDisplayList(IntPtr[] nativeDisplay)
        {
            displays = new Display[nativeDisplay.Length];
            for (int i = 0; i < nativeDisplay.Length; i++)
            {
                displays[i] = new Display(nativeDisplay[i]);
            }
            _mainDisplay = displays[0];
        }

        /// <summary>
        /// <para>Query relative mouse coordinates.</para>
        /// </summary>
        /// <param name="inputMouseCoordinates">Mouse Input Position as Coordinates.</param>
        public static Vector3 RelativeMouseAt(Vector3 inputMouseCoordinates)
        {
            Vector3 vector;
            int rx = 0;
            int ry = 0;
            int x = (int) inputMouseCoordinates.x;
            int y = (int) inputMouseCoordinates.y;
            vector.z = RelativeMouseAtImpl(x, y, out rx, out ry);
            vector.x = rx;
            vector.y = ry;
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int RelativeMouseAtImpl(int x, int y, out int rx, out int ry);
        /// <summary>
        /// <para>Set rendering size and position on screen (Windows only).</para>
        /// </summary>
        /// <param name="width">Change Window Width (Windows Only).</param>
        /// <param name="height">Change Window Height (Windows Only).</param>
        /// <param name="x">Change Window Position X (Windows Only).</param>
        /// <param name="y">Change Window Position Y (Windows Only).</param>
        public void SetParams(int width, int height, int x, int y)
        {
            SetParamsImpl(this.nativeDisplay, width, height, x, y);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetParamsImpl(IntPtr nativeDisplay, int width, int height, int x, int y);
        /// <summary>
        /// <para>Sets rendering resolution for the display.</para>
        /// </summary>
        /// <param name="w">Rendering width in pixels.</param>
        /// <param name="h">Rendering height in pixels.</param>
        public void SetRenderingResolution(int w, int h)
        {
            SetRenderingResolutionImpl(this.nativeDisplay, w, h);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetRenderingResolutionImpl(IntPtr nativeDisplay, int w, int h);

        /// <summary>
        /// <para>Color RenderBuffer.</para>
        /// </summary>
        public RenderBuffer colorBuffer
        {
            get
            {
                RenderBuffer buffer;
                RenderBuffer buffer2;
                GetRenderingBuffersImpl(this.nativeDisplay, out buffer, out buffer2);
                return buffer;
            }
        }

        /// <summary>
        /// <para>Depth RenderBuffer.</para>
        /// </summary>
        public RenderBuffer depthBuffer
        {
            get
            {
                RenderBuffer buffer;
                RenderBuffer buffer2;
                GetRenderingBuffersImpl(this.nativeDisplay, out buffer, out buffer2);
                return buffer2;
            }
        }

        /// <summary>
        /// <para>Main Display.</para>
        /// </summary>
        public static Display main =>
            _mainDisplay;

        /// <summary>
        /// <para>Vertical resolution that the display is rendering at.</para>
        /// </summary>
        public int renderingHeight
        {
            get
            {
                int w = 0;
                int h = 0;
                GetRenderingExtImpl(this.nativeDisplay, out w, out h);
                return h;
            }
        }

        /// <summary>
        /// <para>Horizontal resolution that the display is rendering at.</para>
        /// </summary>
        public int renderingWidth
        {
            get
            {
                int w = 0;
                int h = 0;
                GetRenderingExtImpl(this.nativeDisplay, out w, out h);
                return w;
            }
        }

        /// <summary>
        /// <para>Vertical native display resolution.</para>
        /// </summary>
        public int systemHeight
        {
            get
            {
                int w = 0;
                int h = 0;
                GetSystemExtImpl(this.nativeDisplay, out w, out h);
                return h;
            }
        }

        /// <summary>
        /// <para>Horizontal native display resolution.</para>
        /// </summary>
        public int systemWidth
        {
            get
            {
                int w = 0;
                int h = 0;
                GetSystemExtImpl(this.nativeDisplay, out w, out h);
                return w;
            }
        }

        public delegate void DisplaysUpdatedDelegate();
    }
}

