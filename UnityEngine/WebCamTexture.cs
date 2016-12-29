namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    /// <summary>
    /// <para>WebCam Textures are textures onto which the live video input is rendered.</para>
    /// </summary>
    public sealed class WebCamTexture : Texture
    {
        /// <summary>
        /// <para>Create a WebCamTexture.</para>
        /// </summary>
        /// <param name="deviceName">The name of the video input device to be used.</param>
        /// <param name="requestedWidth">The requested width of the texture.</param>
        /// <param name="requestedHeight">The requested height of the texture.</param>
        /// <param name="requestedFPS">The requested frame rate of the texture.</param>
        public WebCamTexture()
        {
            Internal_CreateWebCamTexture(this, "", 0, 0, 0);
        }

        /// <summary>
        /// <para>Create a WebCamTexture.</para>
        /// </summary>
        /// <param name="deviceName">The name of the video input device to be used.</param>
        /// <param name="requestedWidth">The requested width of the texture.</param>
        /// <param name="requestedHeight">The requested height of the texture.</param>
        /// <param name="requestedFPS">The requested frame rate of the texture.</param>
        public WebCamTexture(string deviceName)
        {
            Internal_CreateWebCamTexture(this, deviceName, 0, 0, 0);
        }

        /// <summary>
        /// <para>Create a WebCamTexture.</para>
        /// </summary>
        /// <param name="deviceName">The name of the video input device to be used.</param>
        /// <param name="requestedWidth">The requested width of the texture.</param>
        /// <param name="requestedHeight">The requested height of the texture.</param>
        /// <param name="requestedFPS">The requested frame rate of the texture.</param>
        public WebCamTexture(int requestedWidth, int requestedHeight)
        {
            Internal_CreateWebCamTexture(this, "", requestedWidth, requestedHeight, 0);
        }

        /// <summary>
        /// <para>Create a WebCamTexture.</para>
        /// </summary>
        /// <param name="deviceName">The name of the video input device to be used.</param>
        /// <param name="requestedWidth">The requested width of the texture.</param>
        /// <param name="requestedHeight">The requested height of the texture.</param>
        /// <param name="requestedFPS">The requested frame rate of the texture.</param>
        public WebCamTexture(int requestedWidth, int requestedHeight, int requestedFPS)
        {
            Internal_CreateWebCamTexture(this, "", requestedWidth, requestedHeight, requestedFPS);
        }

        /// <summary>
        /// <para>Create a WebCamTexture.</para>
        /// </summary>
        /// <param name="deviceName">The name of the video input device to be used.</param>
        /// <param name="requestedWidth">The requested width of the texture.</param>
        /// <param name="requestedHeight">The requested height of the texture.</param>
        /// <param name="requestedFPS">The requested frame rate of the texture.</param>
        public WebCamTexture(string deviceName, int requestedWidth, int requestedHeight)
        {
            Internal_CreateWebCamTexture(this, deviceName, requestedWidth, requestedHeight, 0);
        }

        /// <summary>
        /// <para>Create a WebCamTexture.</para>
        /// </summary>
        /// <param name="deviceName">The name of the video input device to be used.</param>
        /// <param name="requestedWidth">The requested width of the texture.</param>
        /// <param name="requestedHeight">The requested height of the texture.</param>
        /// <param name="requestedFPS">The requested frame rate of the texture.</param>
        public WebCamTexture(string deviceName, int requestedWidth, int requestedHeight, int requestedFPS)
        {
            Internal_CreateWebCamTexture(this, deviceName, requestedWidth, requestedHeight, requestedFPS);
        }

        /// <summary>
        /// <para>Returns pixel color at coordinates (x, y).</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Color GetPixel(int x, int y)
        {
            Color color;
            INTERNAL_CALL_GetPixel(this, x, y, out color);
            return color;
        }

        /// <summary>
        /// <para>Get a block of pixel colors.</para>
        /// </summary>
        public Color[] GetPixels() => 
            this.GetPixels(0, 0, this.width, this.height);

        /// <summary>
        /// <para>Get a block of pixel colors.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="blockWidth"></param>
        /// <param name="blockHeight"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern Color[] GetPixels(int x, int y, int blockWidth, int blockHeight);
        [ExcludeFromDocs]
        public Color32[] GetPixels32()
        {
            Color32[] colors = null;
            return this.GetPixels32(colors);
        }

        /// <summary>
        /// <para>Returns the pixels data in raw format.</para>
        /// </summary>
        /// <param name="colors">Optional array to receive pixel data.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern Color32[] GetPixels32([DefaultValue("null")] Color32[] colors);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetPixel(WebCamTexture self, int x, int y, out Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Pause(WebCamTexture self);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Play(WebCamTexture self);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Stop(WebCamTexture self);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_CreateWebCamTexture([Writable] WebCamTexture self, string scriptingDevice, int requestedWidth, int requestedHeight, int maxFramerate);
        /// <summary>
        /// <para>Marks WebCamTexture as unreadable (no GetPixel* functions will be available (iOS only)).</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("since Unity 5.0 iOS WebCamTexture always goes through CVTextureCache and is read to memory on-demand")]
        public extern void MarkNonReadable();
        /// <summary>
        /// <para>Pauses the camera.</para>
        /// </summary>
        public void Pause()
        {
            INTERNAL_CALL_Pause(this);
        }

        /// <summary>
        /// <para>Starts the camera.</para>
        /// </summary>
        public void Play()
        {
            INTERNAL_CALL_Play(this);
        }

        /// <summary>
        /// <para>Stops the camera.</para>
        /// </summary>
        public void Stop()
        {
            INTERNAL_CALL_Stop(this);
        }

        /// <summary>
        /// <para>Set this to specify the name of the device to use.</para>
        /// </summary>
        public string deviceName { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Return a list of available devices.</para>
        /// </summary>
        public static WebCamDevice[] devices { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Did the video buffer update this frame?</para>
        /// </summary>
        public bool didUpdateThisFrame { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns if the camera is currently playing.</para>
        /// </summary>
        public bool isPlaying { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns if the WebCamTexture is non-readable. (iOS only).</para>
        /// </summary>
        [Obsolete("since Unity 5.0 iOS WebCamTexture always goes through CVTextureCache and is read to memory on-demand")]
        public bool isReadable { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Set the requested frame rate of the camera device (in frames per second).</para>
        /// </summary>
        public float requestedFPS { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Set the requested height of the camera device.</para>
        /// </summary>
        public int requestedHeight { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Set the requested width of the camera device.</para>
        /// </summary>
        public int requestedWidth { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Returns an clockwise angle (in degrees), which can be used to rotate a polygon so camera contents are shown in correct orientation.</para>
        /// </summary>
        public int videoRotationAngle { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns if the texture image is vertically flipped.</para>
        /// </summary>
        public bool videoVerticallyMirrored { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

