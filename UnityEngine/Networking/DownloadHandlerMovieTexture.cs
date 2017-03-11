namespace UnityEngine.Networking
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>A specialized DownloadHandler for creating MovieTexture out of downloaded bytes.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class DownloadHandlerMovieTexture : DownloadHandler
    {
        /// <summary>
        /// <para>Create new DownloadHandlerMovieTexture.</para>
        /// </summary>
        public DownloadHandlerMovieTexture()
        {
            this.InternalCreateDHMovieTexture();
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void InternalCreateDHMovieTexture();
        /// <summary>
        /// <para>Raw downloaded data.</para>
        /// </summary>
        /// <returns>
        /// <para>Raw downloaded bytes.</para>
        /// </returns>
        protected override byte[] GetData() => 
            this.InternalGetData();

        protected override string GetText()
        {
            throw new NotSupportedException("String access is not supported for movies");
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern byte[] InternalGetData();
        /// <summary>
        /// <para>A MovieTexture created out of downloaded bytes.</para>
        /// </summary>
        public MovieTexture movieTexture { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
        /// <summary>
        /// <para>A convenience (helper) method for casting DownloadHandler to DownloadHandlerMovieTexture and accessing its movieTexture property.</para>
        /// </summary>
        /// <param name="uwr">A UnityWebRequest with attached DownloadHandlerMovieTexture.</param>
        /// <returns>
        /// <para>A MovieTexture created out of downloaded bytes.</para>
        /// </returns>
        public static MovieTexture GetContent(UnityWebRequest uwr) => 
            DownloadHandler.GetCheckedDownloader<DownloadHandlerMovieTexture>(uwr).movieTexture;
    }
}

