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
    /// <para>Captures a photo from the web camera and stores it in memory or on disk.</para>
    /// </summary>
    public sealed class PhotoCapture : IDisposable
    {
        private static readonly long HR_SUCCESS = 0L;
        private IntPtr m_NativePtr;
        private static Resolution[] s_SupportedResolutions;

        private PhotoCapture(IntPtr nativeCaptureObject)
        {
            this.m_NativePtr = nativeCaptureObject;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void CapturePhotoToDisk_Internal(IntPtr photoCaptureObj, string filename, int fileOutputFormat, OnCapturedToDiskCallback onCapturedPhotoToDiskCallback);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void CapturePhotoToMemory_Internal(IntPtr photoCaptureObj, OnCapturedToMemoryCallback onCapturedPhotoToMemoryCallback);
        public static void CreateAsync(bool showHolograms, OnCaptureResourceCreatedCallback onCreatedCallback)
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

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Dispose_Internal(IntPtr photoCaptureObj);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private static extern void DisposeThreaded_Internal(IntPtr photoCaptureObj);
        ~PhotoCapture()
        {
            if (this.m_NativePtr != IntPtr.Zero)
            {
                DisposeThreaded_Internal(this.m_NativePtr);
                this.m_NativePtr = IntPtr.Zero;
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
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
        private static IntPtr GetUnsafePointerToVideoDeviceController_Internal(IntPtr photoCaptureObj)
        {
            IntPtr ptr;
            INTERNAL_CALL_GetUnsafePointerToVideoDeviceController_Internal(photoCaptureObj, out ptr);
            return ptr;
        }

        private static IntPtr Instantiate_Internal(bool showHolograms, OnCaptureResourceCreatedCallback onCreatedCallback)
        {
            IntPtr ptr;
            INTERNAL_CALL_Instantiate_Internal(showHolograms, onCreatedCallback, out ptr);
            return ptr;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetUnsafePointerToVideoDeviceController_Internal(IntPtr photoCaptureObj, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Instantiate_Internal(bool showHolograms, OnCaptureResourceCreatedCallback onCreatedCallback, out IntPtr value);
        [RequiredByNativeCode]
        private static void InvokeOnCapturedPhotoToDiskDelegate(OnCapturedToDiskCallback callback, long hResult)
        {
            callback(MakeCaptureResult(hResult));
        }

        [RequiredByNativeCode]
        private static void InvokeOnCapturedPhotoToMemoryDelegate(OnCapturedToMemoryCallback callback, long hResult, IntPtr photoCaptureFramePtr)
        {
            PhotoCaptureFrame photoCaptureFrame = null;
            if (photoCaptureFramePtr != IntPtr.Zero)
            {
                photoCaptureFrame = new PhotoCaptureFrame(photoCaptureFramePtr);
            }
            callback(MakeCaptureResult(hResult), photoCaptureFrame);
        }

        [RequiredByNativeCode]
        private static void InvokeOnCreatedResourceDelegate(OnCaptureResourceCreatedCallback callback, IntPtr nativePtr)
        {
            if (nativePtr == IntPtr.Zero)
            {
                callback(null);
            }
            else
            {
                callback(new PhotoCapture(nativePtr));
            }
        }

        [RequiredByNativeCode]
        private static void InvokeOnPhotoModeStartedDelegate(OnPhotoModeStartedCallback callback, long hResult)
        {
            callback(MakeCaptureResult(hResult));
        }

        [RequiredByNativeCode]
        private static void InvokeOnPhotoModeStoppedDelegate(OnPhotoModeStoppedCallback callback, long hResult)
        {
            callback(MakeCaptureResult(hResult));
        }

        private static PhotoCaptureResult MakeCaptureResult(long hResult)
        {
            CaptureResultType success;
            PhotoCaptureResult result = new PhotoCaptureResult();
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

        private static PhotoCaptureResult MakeCaptureResult(CaptureResultType resultType, long hResult) => 
            new PhotoCaptureResult { 
                resultType = resultType,
                hResult = hResult
            };

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void StartPhotoMode_Internal(IntPtr photoCaptureObj, OnPhotoModeStartedCallback onPhotoModeStartedCallback, float hologramOpacity, float frameRate, int cameraResolutionWidth, int cameraResolutionHeight, int pixelFormat);
        public void StartPhotoModeAsync(CameraParameters setupParams, OnPhotoModeStartedCallback onPhotoModeStartedCallback)
        {
            if (this.m_NativePtr == IntPtr.Zero)
            {
                throw new InvalidOperationException("You must create a Photo Capture Object before starting its photo mode.");
            }
            if (onPhotoModeStartedCallback == null)
            {
                throw new ArgumentException("onPhotoModeStartedCallback");
            }
            if ((setupParams.cameraResolutionWidth == 0) || (setupParams.cameraResolutionHeight == 0))
            {
                throw new ArgumentOutOfRangeException("setupParams", "The camera resolution must be set to a supported resolution.");
            }
            this.StartPhotoMode_Internal(this.m_NativePtr, onPhotoModeStartedCallback, setupParams.hologramOpacity, setupParams.frameRate, setupParams.cameraResolutionWidth, setupParams.cameraResolutionHeight, (int) setupParams.pixelFormat);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void StopPhotoMode_Internal(IntPtr photoCaptureObj, OnPhotoModeStoppedCallback onPhotoModeStoppedCallback);
        public void StopPhotoModeAsync(OnPhotoModeStoppedCallback onPhotoModeStoppedCallback)
        {
            if (this.m_NativePtr == IntPtr.Zero)
            {
                throw new InvalidOperationException("You must create a Photo Capture Object before stopping its photo mode.");
            }
            if (onPhotoModeStoppedCallback == null)
            {
                throw new ArgumentException("onPhotoModeStoppedCallback");
            }
            this.StopPhotoMode_Internal(this.m_NativePtr, onPhotoModeStoppedCallback);
        }

        public void TakePhotoAsync(OnCapturedToMemoryCallback onCapturedPhotoToMemoryCallback)
        {
            if (this.m_NativePtr == IntPtr.Zero)
            {
                throw new InvalidOperationException("You must create a Photo Capture Object before taking a photo.");
            }
            if (onCapturedPhotoToMemoryCallback == null)
            {
                throw new ArgumentNullException("onCapturedPhotoToMemoryCallback");
            }
            this.CapturePhotoToMemory_Internal(this.m_NativePtr, onCapturedPhotoToMemoryCallback);
        }

        public void TakePhotoAsync(string filename, PhotoCaptureFileOutputFormat fileOutputFormat, OnCapturedToDiskCallback onCapturedPhotoToDiskCallback)
        {
            if (this.m_NativePtr == IntPtr.Zero)
            {
                throw new InvalidOperationException("You must create a Photo Capture Object before taking a photo.");
            }
            if (onCapturedPhotoToDiskCallback == null)
            {
                throw new ArgumentNullException("onCapturedPhotoToDiskCallback");
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
            this.CapturePhotoToDisk_Internal(this.m_NativePtr, filename, (int) fileOutputFormat, onCapturedPhotoToDiskCallback);
        }

        /// <summary>
        /// <para>A list of all the supported device resolutions for taking pictures.</para>
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
        /// <para>Contains the result of the capture request.</para>
        /// </summary>
        public enum CaptureResultType
        {
            Success,
            UnknownError
        }

        /// <summary>
        /// <para>Called when a photo has been saved to the file system.</para>
        /// </summary>
        /// <param name="result">Indicates whether or not the photo was successfully saved to the file system.</param>
        public delegate void OnCapturedToDiskCallback(PhotoCapture.PhotoCaptureResult result);

        /// <summary>
        /// <para>Called when a photo has been captured to memory.</para>
        /// </summary>
        /// <param name="result">Indicates whether or not the photo was successfully captured to memory.</param>
        /// <param name="photoCaptureFrame">Contains the target texture.  If available, the spatial information will be accessible through this structure as well.</param>
        public delegate void OnCapturedToMemoryCallback(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame);

        /// <summary>
        /// <para>Called when a PhotoCapture resource has been created.</para>
        /// </summary>
        /// <param name="captureObject">The PhotoCapture instance.</param>
        public delegate void OnCaptureResourceCreatedCallback(PhotoCapture captureObject);

        /// <summary>
        /// <para>Called when photo mode has been started.</para>
        /// </summary>
        /// <param name="result">Indicates whether or not photo mode was successfully activated.</param>
        public delegate void OnPhotoModeStartedCallback(PhotoCapture.PhotoCaptureResult result);

        /// <summary>
        /// <para>Called when photo mode has been stopped.</para>
        /// </summary>
        /// <param name="result">Indicates whether or not photo mode was successfully deactivated.</param>
        public delegate void OnPhotoModeStoppedCallback(PhotoCapture.PhotoCaptureResult result);

        /// <summary>
        /// <para>A data container that contains the result information of a photo capture operation.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct PhotoCaptureResult
        {
            /// <summary>
            /// <para>A generic result that indicates whether or not the PhotoCapture operation succeeded.</para>
            /// </summary>
            public PhotoCapture.CaptureResultType resultType;
            /// <summary>
            /// <para>The specific HResult value.</para>
            /// </summary>
            public long hResult;
            /// <summary>
            /// <para>Indicates whether or not the operation was successful.</para>
            /// </summary>
            public bool success =>
                (this.resultType == PhotoCapture.CaptureResultType.Success);
        }
    }
}

