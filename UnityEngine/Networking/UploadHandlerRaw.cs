namespace UnityEngine.Networking
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>A general-purpose UploadHandler subclass, using a native-code memory buffer.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class UploadHandlerRaw : UploadHandler
    {
        /// <summary>
        /// <para>General constructor. Contents of the input argument are copied into a native buffer.</para>
        /// </summary>
        /// <param name="data">Raw data to transmit to the remote server.</param>
        public UploadHandlerRaw(byte[] data)
        {
            base.InternalCreateRaw(data);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern string InternalGetContentType();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void InternalSetContentType(string newContentType);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern byte[] InternalGetData();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern float InternalGetProgress();
        internal override string GetContentType() => 
            this.InternalGetContentType();

        internal override void SetContentType(string newContentType)
        {
            this.InternalSetContentType(newContentType);
        }

        internal override byte[] GetData() => 
            this.InternalGetData();

        internal override float GetProgress() => 
            this.InternalGetProgress();
    }
}

