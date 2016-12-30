namespace UnityEngine.VR.WSA.WebCam
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Records a video from the web camera directly to disk.</para>
    /// </summary>
    public sealed class VideoCapture : IDisposable
    {
        private static readonly long HR_SUCCESS = 0L;
        private IntPtr m_NativePtr;
        private static Resolution[] s_SupportedResolutions;

        private VideoCapture(IntPtr nativeCaptureObject)
        {
            this.m_NativePtr = nativeCaptureObject;
        }

        public static void CreateAsync(bool showHolograms, OnVideoCaptureResourceCreatedCallback onCreatedCallback)
        {
            if (onCreatedCallback == null)
            {
                throw new ArgumentNullException("onCreatedCallback");
            }
            Instantiate_Internal(showHolograms, onCreatedCallback);
        }

        /// <summary>
        /// <para>Dispose must be called to shutdown the PhotoCapture instance.</para>
        /// </summary>
        public void Dispose()
        {
            if (this.m_NativePtr != IntPtr.Zero)
            {
                Dispose_Internal(this.m_NativePtr);
                this.m_NativePtr = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Dispose_Internal(IntPtr videoCaptureObj);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        private static extern void DisposeThreaded_Internal(IntPtr videoCaptureObj);
        ~VideoCapture()
        {
            if (this.m_NativePtr != IntPtr.Zero)
            {
                DisposeThreaded_Internal(this.m_NativePtr);
                this.m_NativePtr = IntPtr.Zero;
            }
        }

        /// <summary>
        /// <para>Returns the supported frame rates at which a video can be recorded given a resolution.</para>
        /// </summary>
        /// <param name="resolution">A recording resolution.</param>
        /// <returns>
        /// <para>The frame rates at which the video can be recorded.</para>
        /// </returns>
        public static IEnumerable<float> GetSupportedFrameRatesForResolution(Resolution resolution) => 
            GetSupportedFrameRatesForResolution_Internal(resolution.width, resolution.height);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern float[] GetSupportedFrameRatesForResolution_Internal(int resolutionWidth, int resolutionHeight);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern Resolution[] GetSupportedResolutions_Internal();
        /// <summary>
        /// <para>Provides a COM pointer to the native IVideoDeviceController.</para>
        /// </summary>
        /// <returns>
        /// <para>A native COM pointer to the IVideoDeviceController.</para>
        /// </returns>
        public IntPtr GetUnsafePointerToVideoDeviceController() => 
            GetUnsafePointerToVideoDeviceController_Internal(this.m_NativePtr);

        [ThreadAndSerializationSafe]
        private static IntPtr GetUnsafePointerToVideoDeviceController_Internal(IntPtr videoCaptureObj)
        {
            IntPtr ptr;
            INTERNAL_CALL_GetUnsafePointerToVideoDeviceController_Internal(videoCaptureObj, out ptr);
            return ptr;
        }

        private static IntPtr Instantiate_Internal(bool showHolograms, OnVideoCaptureResourceCreatedCallback onCreatedCallback)
        {
            IntPtr ptr;
            INTERNAL_CALL_Instantiate_Internal(showHolograms, onCreatedCallback, out ptr);
            return ptr;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetUnsafePointerToVideoDeviceController_Internal(IntPtr videoCaptureObj, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Instantiate_Internal(bool showHolograms, OnVideoCaptureResourceCreatedCallback onCreatedCallback, out IntPtr value);
        [RequiredByNativeCode]
        private static void InvokeOnCreatedVideoCaptureResourceDelegate(OnVideoCaptureResourceCreatedCallback callback, IntPtr nativePtr)
        {
            if (nativePtr == IntPtr.Zero)
            {
                callback(null);
            }
            else
            {
                callback(new VideoCapture(nativePtr));
            }
        }

        [RequiredByNativeCode]
        private static void InvokeOnStartedRecordingVideoToDiskDelegate(OnStartedRecordingVideoCallback callback, long hResult)
        {
            callback(MakeCaptureResult(hResult));
        }

        [RequiredByNativeCode]
        private static void InvokeOnStoppedRecordingVideoToDiskDelegate(OnStoppedRecordingVideoCallback callback, long hResult)
        {
            callback(MakeCaptureResult(hResult));
        }

        [RequiredByNativeCode]
        private static void InvokeOnVideoModeStartedDelegate(OnVideoModeStartedCallback callback, long hResult)
        {
            callback(MakeCaptureResult(hResult));
        }

        [RequiredByNativeCode]
        private static void InvokeOnVideoModeStoppedDelegate(OnVideoModeStoppedCallback callback, long hResult)
        {
            callback(MakeCaptureResult(hResult));
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern bool IsRecording_Internal(IntPtr videoCaptureObj);
        private static VideoCaptureResult MakeCaptureResult(long hResult)
        {
            CaptureResultType success;
            VideoCaptureResult result = new VideoCaptureResult();
            if (hResult == HR_SUCCESS)
            {
                success = CaptureResultType.Success;
            }
            else
            {
                success = CaptureResultType.UnknownError;
            }
            result.resultType = success;
            result.hResult = hResult;
            return result;
        }

        private static VideoCaptureResult MakeCaptureResult(CaptureResultType resultType, long hResult) => 
            new VideoCaptureResult { 
                resultType = resultType,
                hResult = hResult
            };

        public void StartRecordingAsync(string filename, OnStartedRecordingVideoCallback onStartedRecordingVideoCallback)
        {
            if (this.m_NativePtr == IntPtr.Zero)
            {
                throw new InvalidOperationException("You must create a Video Capture Object before recording video.");
            }
            if (onStartedRecordingVideoCallback == null)
            {
                throw new ArgumentNullException("onStartedRecordingVideoCallback");
            }
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException("filename");
            }
            filename = filename.Replace("/", @"\");
            string directoryName = Path.GetDirectoryName(filename);
            if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
            {
                throw new ArgumentException("The specified directory does not exist.", "filename");
            }
            this.StartRecordingVideoToDisk_Internal(this.m_NativePtr, filename, onStartedRecordingVideoCallback);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void StartRecordingVideoToDisk_Internal(IntPtr videoCaptureObj, string filename, OnStartedRecordingVideoCallback onStartedRecordingVideoCallback);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void StartVideoMode_Internal(IntPtr videoCaptureObj, int audioState, OnVideoModeStartedCallback onVideoModeStartedCallback, float hologramOpacity, float frameRate, int cameraResolutionWidth, int cameraResolutionHeight, int pixelFormat);
        public void StartVideoModeAsync(CameraParameters setupParams, AudioState audioState, OnVideoModeStartedCallback onVideoModeStartedCallback)
        {
            if (this.m_NativePtr == IntPtr.Zero)
            {
                throw new InvalidOperationException("You must create a Video Capture Object before starting its video mode.");
            }
            if (onVideoModeStartedCallback == null)
            {
                throw new ArgumentNullException("onVideoModeStartedCallback");
            }
            if ((setupParams.cameraResolutionWidth == 0) || (setupParams.cameraResolutionHeight == 0))
            {
                throw new ArgumentOutOfRangeException("setupParams", "The camera resolution must be set to a supported resolution.");
            }
            if (setupParams.frameRate == 0f)
            {
                throw new ArgumentOutOfRangeException("setupParams", "The camera frame rate must be set to a supported recording frame rate.");
            }
            this.StartVideoMode_Internal(this.m_NativePtr, (int) audioState, onVideoModeStartedCallback, setupParams.hologramOpacity, setupParams.frameRate, setupParams.cameraResolutionWidth, setupParams.cameraResolutionHeight, (int) setupParams.pixelFormat);
        }

        public void StopRecordingAsync(OnStoppedRecordingVideoCallback onStoppedRecordingVideoCallback)
        {
            if (this.m_NativePtr == IntPtr.Zero)
            {
                throw new InvalidOperationException("You must create a Video Capture Object before recording video.");
            }
            if (onStoppedRecordingVideoCallback == null)
            {
                throw new ArgumentNullException("onStoppedRecordingVideoCallback");
            }
            this.StopRecordingVideoToDisk_Internal(this.m_NativePtr, onStoppedRecordingVideoCallback);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void StopRecordingVideoToDisk_Internal(IntPtr videoCaptureObj, OnStoppedRecordingVideoCallback onStoppedRecordingVideoCallback);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void StopVideoMode_Internal(IntPtr videoCaptureObj, OnVideoModeStoppedCallback onVideoModeStoppedCallback);
        public void StopVideoModeAsync(OnVideoModeStoppedCallback onVideoModeStoppedCallback)
        {
            if (this.m_NativePtr == IntPtr.Zero)
            {
                throw new InvalidOperationException("You must create a Video Capture Object before stopping its video mode.");
            }
            if (onVideoModeStoppedCallback == null)
            {
                throw new ArgumentNullException("onVideoModeStoppedCallback");
            }
            this.StopVideoMode_Internal(this.m_NativePtr, onVideoModeStoppedCallback);
        }

        /// <summary>
        /// <para>Indicates whether or not the VideoCapture instance is currently recording video.</para>
        /// </summary>
        public bool IsRecording
        {
            get
            {
                if (this.m_NativePtr == IntPtr.Zero)
                {
                    throw new InvalidOperationException("You must create a Video Capture Object before using it.");
                }
                return this.IsRecording_Internal(this.m_NativePtr);
            }
        }

        /// <summary>
        /// <para>A list of all the supported device resolutions for recording videos.</para>
        /// </summary>
        public static IEnumerable<Resolution> SupportedResolutions
        {
            get
            {
                if (s_SupportedResolutions == null)
                {
                    s_SupportedResolutions = GetSupportedResolutions_Internal();
                }
                return s_SupportedResolutions;
            }
        }

        /// <summary>
        /// <para>Specifies what audio sources should be recorded while recording the video.</para>
        /// </summary>
        public enum AudioState
        {
            MicAudio,
            ApplicationAudio,
            ApplicationAndMicAudio,
            None
        }

        /// <summary>
        /// <para>Contains the result of the capture request.</para>
        /// </summary>
        public enum CaptureResultType
        {
            Success,
            UnknownError
        }

        /// <summary>
        /// <para>Called when the web camera begins recording the video.</para>
        /// </summary>
        /// <param name="result">Indicates whether or not video recording started successfully.</param>
        public delegate void OnStartedRecordingVideoCallback(VideoCapture.VideoCaptureResult result);

        /// <summary>
        /// <para>Called when the video recording has been saved to the file system.</para>
        /// </summary>
        /// <param name="result">Indicates whether or not video recording was saved successfully to the file system.</param>
        public delegate void OnStoppedRecordingVideoCallback(VideoCapture.VideoCaptureResult result);

        /// <summary>
        /// <para>Called when a VideoCapture resource has been created.</para>
        /// </summary>
        /// <param name="captureObject">The VideoCapture instance.</param>
        public delegate void OnVideoCaptureResourceCreatedCallback(VideoCapture captureObject);

        /// <summary>
        /// <para>Called when video mode has been started.</para>
        /// </summary>
        /// <param name="result">Indicates whether or not video mode was successfully activated.</param>
        public delegate void OnVideoModeStartedCallback(VideoCapture.VideoCaptureResult result);

        /// <summary>
        /// <para>Called when video mode has been stopped.</para>
        /// </summary>
        /// <param name="result">Indicates whether or not video mode was successfully deactivated.</param>
        public delegate void OnVideoModeStoppedCallback(VideoCapture.VideoCaptureResult result);

        /// <summary>
        /// <para>A data container that contains the result information of a video recording operation.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct VideoCaptureResult
        {
            /// <summary>
            /// <para>A generic result that indicates whether or not the VideoCapture operation succeeded.</para>
            /// </summary>
            public VideoCapture.CaptureResultType resultType;
            /// <summary>
            /// <para>The specific HResult value.</para>
            /// </summary>
            public long hResult;
            /// <summary>
            /// <para>Indicates whether or not the operation was successful.</para>
            /// </summary>
            public bool success =>
                (this.resultType == VideoCapture.CaptureResultType.Success);
        }
    }
}

