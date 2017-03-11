namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Access system and hardware information.</para>
    /// </summary>
    public sealed class SystemInfo
    {
        /// <summary>
        /// <para>Value returned by SystemInfo string properties which are not supported on the current platform.</para>
        /// </summary>
        public const string unsupportedIdentifier = "n/a";

        /// <summary>
        /// <para>Is render texture format supported?</para>
        /// </summary>
        /// <param name="format">The format to look up.</param>
        /// <returns>
        /// <para>True if the format is supported.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool SupportsRenderTextureFormat(RenderTextureFormat format);
        /// <summary>
        /// <para>Is texture format supported on this device?</para>
        /// </summary>
        /// <param name="format">The TextureFormat format to look up.</param>
        /// <returns>
        /// <para>True if the format is supported.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool SupportsTextureFormat(TextureFormat format);

        /// <summary>
        /// <para>The current battery level (Read Only).</para>
        /// </summary>
        public static float batteryLevel { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Returns the current status of the device's battery (Read Only).</para>
        /// </summary>
        public static BatteryStatus batteryStatus { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Support for various Graphics.CopyTexture cases (Read Only).</para>
        /// </summary>
        public static CopyTextureSupport copyTextureSupport { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The model of the device (Read Only).</para>
        /// </summary>
        public static string deviceModel { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The user defined name of the device (Read Only).</para>
        /// </summary>
        public static string deviceName { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Returns the kind of device the application is running on (Read Only).</para>
        /// </summary>
        public static DeviceType deviceType { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>A unique device identifier. It is guaranteed to be unique for every device (Read Only).</para>
        /// </summary>
        public static string deviceUniqueIdentifier { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The identifier code of the graphics device (Read Only).</para>
        /// </summary>
        public static int graphicsDeviceID { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The name of the graphics device (Read Only).</para>
        /// </summary>
        public static string graphicsDeviceName { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The graphics API type used by the graphics device (Read Only).</para>
        /// </summary>
        public static GraphicsDeviceType graphicsDeviceType { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The vendor of the graphics device (Read Only).</para>
        /// </summary>
        public static string graphicsDeviceVendor { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The identifier code of the graphics device vendor (Read Only).</para>
        /// </summary>
        public static int graphicsDeviceVendorID { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The graphics API type and driver version used by the graphics device (Read Only).</para>
        /// </summary>
        public static string graphicsDeviceVersion { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Amount of video memory present (Read Only).</para>
        /// </summary>
        public static int graphicsMemorySize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Is graphics device using multi-threaded rendering (Read Only)?</para>
        /// </summary>
        public static bool graphicsMultiThreaded { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        [Obsolete("graphicsPixelFillrate is no longer supported in Unity 5.0+.")]
        public static int graphicsPixelFillrate =>
            -1;

        /// <summary>
        /// <para>Graphics device shader capability level (Read Only).</para>
        /// </summary>
        public static int graphicsShaderLevel { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Returns true if the texture UV coordinate convention for this platform has Y starting at the top of the image.</para>
        /// </summary>
        public static bool graphicsUVStartsAtTop { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Maximum Cubemap texture size (Read Only).</para>
        /// </summary>
        public static int maxCubemapSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        internal static int maxRenderTextureSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Maximum texture size (Read Only).</para>
        /// </summary>
        public static int maxTextureSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>What NPOT (non-power of two size) texture support does the GPU provide? (Read Only)</para>
        /// </summary>
        public static NPOTSupport npotSupport { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Operating system name with version (Read Only).</para>
        /// </summary>
        public static string operatingSystem { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Returns the operating system family the game is running on (Read Only).</para>
        /// </summary>
        public static OperatingSystemFamily operatingSystemFamily { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Number of processors present (Read Only).</para>
        /// </summary>
        public static int processorCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Processor frequency in MHz (Read Only).</para>
        /// </summary>
        public static int processorFrequency { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Processor name (Read Only).</para>
        /// </summary>
        public static string processorType { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>How many simultaneous render targets (MRTs) are supported? (Read Only)</para>
        /// </summary>
        public static int supportedRenderTargetCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Are 2D Array textures supported? (Read Only)</para>
        /// </summary>
        public static bool supports2DArrayTextures { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Are 3D (volume) RenderTextures supported? (Read Only)</para>
        /// </summary>
        public static bool supports3DRenderTextures { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Are 3D (volume) textures supported? (Read Only)</para>
        /// </summary>
        public static bool supports3DTextures { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Is an accelerometer available on the device?</para>
        /// </summary>
        public static bool supportsAccelerometer { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Is there an Audio device available for playback?</para>
        /// </summary>
        public static bool supportsAudio { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Are compute shaders supported? (Read Only)</para>
        /// </summary>
        public static bool supportsComputeShaders { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Are Cubemap Array textures supported? (Read Only)</para>
        /// </summary>
        public static bool supportsCubemapArrayTextures { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Is a gyroscope available on the device?</para>
        /// </summary>
        public static bool supportsGyroscope { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Are image effects supported? (Read Only)</para>
        /// </summary>
        public static bool supportsImageEffects { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Is GPU draw call instancing supported? (Read Only)</para>
        /// </summary>
        public static bool supportsInstancing { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Is the device capable of reporting its location?</para>
        /// </summary>
        public static bool supportsLocationService { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Whether motion vectors are supported on this platform.</para>
        /// </summary>
        public static bool supportsMotionVectors { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Is sampling raw depth from shadowmaps supported? (Read Only)</para>
        /// </summary>
        public static bool supportsRawShadowDepthSampling { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Are render textures supported? (Read Only)</para>
        /// </summary>
        [Obsolete("supportsRenderTextures always returns true, no need to call it")]
        public static bool supportsRenderTextures { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Are cubemap render textures supported? (Read Only)</para>
        /// </summary>
        public static bool supportsRenderToCubemap { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Are built-in shadows supported? (Read Only)</para>
        /// </summary>
        public static bool supportsShadows { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Are sparse textures supported? (Read Only)</para>
        /// </summary>
        public static bool supportsSparseTextures { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Is the stencil buffer supported? (Read Only)</para>
        /// </summary>
        [Obsolete("supportsStencil always returns true, no need to call it")]
        public static int supportsStencil { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        [Obsolete("Vertex program support is required in Unity 5.0+")]
        public static bool supportsVertexPrograms =>
            true;

        /// <summary>
        /// <para>Is the device capable of providing the user haptic feedback by vibration?</para>
        /// </summary>
        public static bool supportsVibration { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Amount of system memory present (Read Only).</para>
        /// </summary>
        public static int systemMemorySize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>This property is true if the current platform uses a reversed depth buffer (where values range from 1 at the near plane and 0 at far plane), and false if the depth buffer is normal (0 is near, 1 is far). (Read Only)</para>
        /// </summary>
        public static bool usesReversedZBuffer { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

