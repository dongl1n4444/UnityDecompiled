namespace UnityEngine.VR.WSA.WebCam
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>Contains information captured from the web camera.</para>
    /// </summary>
    public sealed class PhotoCaptureFrame : IDisposable
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <dataLength>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <hasLocationData>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private CapturePixelFormat <pixelFormat>k__BackingField;
        private IntPtr m_NativePtr;

        internal PhotoCaptureFrame(IntPtr nativePtr)
        {
            this.m_NativePtr = nativePtr;
            this.dataLength = GetDataLength(nativePtr);
            this.hasLocationData = GetHasLocationData(nativePtr);
            this.pixelFormat = GetCapturePixelFormat(nativePtr);
            GC.AddMemoryPressure((long) this.dataLength);
        }

        private void Cleanup()
        {
            if (this.m_NativePtr != IntPtr.Zero)
            {
                GC.RemoveMemoryPressure((long) this.dataLength);
                Dispose_Internal(this.m_NativePtr);
                this.m_NativePtr = IntPtr.Zero;
            }
        }

        public void CopyRawImageDataIntoBuffer(List<byte> byteBuffer)
        {
            if (byteBuffer == null)
            {
                throw new ArgumentNullException("byteBuffer");
            }
            byte[] collection = CopyRawImageDataIntoBuffer_Internal(this.m_NativePtr);
            int length = collection.Length;
            if (byteBuffer.Capacity < length)
            {
                byteBuffer.Capacity = length;
            }
            byteBuffer.Clear();
            byteBuffer.AddRange(collection);
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        internal static extern byte[] CopyRawImageDataIntoBuffer_Internal(IntPtr photoCaptureFrame);
        /// <summary>
        /// <para>Disposes the PhotoCaptureFrame and any resources it uses.</para>
        /// </summary>
        public void Dispose()
        {
            this.Cleanup();
            GC.SuppressFinalize(this);
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private static extern void Dispose_Internal(IntPtr photoCaptureFrame);
        ~PhotoCaptureFrame()
        {
            this.Cleanup();
        }

        [ThreadAndSerializationSafe]
        private static Matrix4x4 GetCameraToWorldMatrix(IntPtr photoCaptureFrame)
        {
            Matrix4x4 matrixx;
            INTERNAL_CALL_GetCameraToWorldMatrix(photoCaptureFrame, out matrixx);
            return matrixx;
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private static extern CapturePixelFormat GetCapturePixelFormat(IntPtr photoCaptureFrame);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private static extern void GetData_Internal(IntPtr photoCaptureFrame, IntPtr targetBuffer);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private static extern int GetDataLength(IntPtr photoCaptureFrame);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private static extern bool GetHasLocationData(IntPtr photoCaptureFrame);
        [ThreadAndSerializationSafe]
        private static Matrix4x4 GetProjection(IntPtr photoCaptureFrame)
        {
            Matrix4x4 matrixx;
            INTERNAL_CALL_GetProjection(photoCaptureFrame, out matrixx);
            return matrixx;
        }

        /// <summary>
        /// <para>Provides a COM pointer to the native IMFMediaBuffer that contains the image data.</para>
        /// </summary>
        /// <returns>
        /// <para>A native COM pointer to the IMFMediaBuffer which contains the image data.</para>
        /// </returns>
        public IntPtr GetUnsafePointerToBuffer() => 
            GetUnsafePointerToBuffer(this.m_NativePtr);

        [ThreadAndSerializationSafe]
        private static IntPtr GetUnsafePointerToBuffer(IntPtr photoCaptureFrame)
        {
            IntPtr ptr;
            INTERNAL_CALL_GetUnsafePointerToBuffer(photoCaptureFrame, out ptr);
            return ptr;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetCameraToWorldMatrix(IntPtr photoCaptureFrame, out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetProjection(IntPtr photoCaptureFrame, out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetUnsafePointerToBuffer(IntPtr photoCaptureFrame, out IntPtr value);
        public bool TryGetCameraToWorldMatrix(out Matrix4x4 cameraToWorldMatrix)
        {
            cameraToWorldMatrix = Matrix4x4.identity;
            if (this.hasLocationData)
            {
                cameraToWorldMatrix = GetCameraToWorldMatrix(this.m_NativePtr);
                return true;
            }
            return false;
        }

        public bool TryGetProjectionMatrix(out Matrix4x4 projectionMatrix)
        {
            if (this.hasLocationData)
            {
                projectionMatrix = GetProjection(this.m_NativePtr);
                return true;
            }
            projectionMatrix = Matrix4x4.identity;
            return false;
        }

        public bool TryGetProjectionMatrix(float nearClipPlane, float farClipPlane, out Matrix4x4 projectionMatrix)
        {
            if (this.hasLocationData)
            {
                float num = 0.01f;
                if (nearClipPlane < num)
                {
                    nearClipPlane = num;
                }
                if (farClipPlane < (nearClipPlane + num))
                {
                    farClipPlane = nearClipPlane + num;
                }
                projectionMatrix = GetProjection(this.m_NativePtr);
                float num2 = 1f / (farClipPlane - nearClipPlane);
                float num3 = -(farClipPlane + nearClipPlane) * num2;
                float num4 = -((2f * farClipPlane) * nearClipPlane) * num2;
                projectionMatrix.m22 = num3;
                projectionMatrix.m23 = num4;
                return true;
            }
            projectionMatrix = Matrix4x4.identity;
            return false;
        }

        /// <summary>
        /// <para>This method will copy the captured image data into a user supplied texture for use in Unity.</para>
        /// </summary>
        /// <param name="targetTexture">The target texture that the captured image data will be copied to.</param>
        public void UploadImageDataToTexture(Texture2D targetTexture)
        {
            if (targetTexture == null)
            {
                throw new ArgumentNullException("targetTexture");
            }
            if (this.pixelFormat != CapturePixelFormat.BGRA32)
            {
                throw new ArgumentException("Uploading PhotoCaptureFrame to a texture is only supported with BGRA32 CameraFrameFormat!");
            }
            UploadImageDataToTexture_Internal(this.m_NativePtr, targetTexture);
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private static extern void UploadImageDataToTexture_Internal(IntPtr photoCaptureFrame, Texture2D targetTexture);

        /// <summary>
        /// <para>The length of the raw IMFMediaBuffer which contains the image captured.</para>
        /// </summary>
        public int dataLength { get; private set; }

        /// <summary>
        /// <para>Specifies whether or not spatial data was captured.</para>
        /// </summary>
        public bool hasLocationData { get; private set; }

        /// <summary>
        /// <para>The raw image data pixel format.</para>
        /// </summary>
        public CapturePixelFormat pixelFormat { get; private set; }
    }
}

