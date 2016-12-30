namespace UnityEngine.Networking
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>A general-purpose DownloadHandler implementation which stores received data in a native byte buffer.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class DownloadHandlerBuffer : DownloadHandler
    {
        /// <summary>
        /// <para>Default constructor.</para>
        /// </summary>
        public DownloadHandlerBuffer()
        {
            base.InternalCreateBuffer();
        }

        /// <summary>
        /// <para>Returns a copy of the contents of the native-memory data buffer as a byte array.</para>
        /// </summary>
        /// <returns>
        /// <para>A copy of the data which has been downloaded.</para>
        /// </returns>
        protected override byte[] GetData() => 
            this.InternalGetData();

        /// <summary>
        /// <para>Returns a copy of the native-memory buffer interpreted as a UTF8 string.</para>
        /// </summary>
        /// <returns>
        /// <para>A string representing the data in the native-memory buffer.</para>
        /// </returns>
        protected override string GetText() => 
            this.InternalGetText();

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern byte[] InternalGetData();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern string InternalGetText();
        /// <summary>
        /// <para>Returns a copy of the native-memory buffer interpreted as a UTF8 string.</para>
        /// </summary>
        /// <param name="www">A finished UnityWebRequest object with DownloadHandlerBuffer attached.</param>
        /// <returns>
        /// <para>The same as DownloadHandlerBuffer.text</para>
        /// </returns>
        public static string GetContent(UnityWebRequest www) => 
            DownloadHandler.GetCheckedDownloader<DownloadHandlerBuffer>(www).text;
    }
}

