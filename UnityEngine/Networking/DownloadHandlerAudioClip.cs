namespace UnityEngine.Networking
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>A DownloadHandler subclass specialized for downloading audio data for use as AudioClip objects.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class DownloadHandlerAudioClip : DownloadHandler
    {
        /// <summary>
        /// <para>Constructor, specifies what kind of audio data is going to be downloaded.</para>
        /// </summary>
        /// <param name="url">The nominal (pre-redirect) URL at which the audio clip is located.</param>
        /// <param name="audioType">Value to set for AudioClip type.</param>
        [RequiredByNativeCode]
        public DownloadHandlerAudioClip(string url, AudioType audioType)
        {
            this.InternalCreateAudioClip(url, audioType);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void InternalCreateAudioClip(string url, AudioType audioType);
        /// <summary>
        /// <para>Called by DownloadHandler.data. Returns a copy of the downloaded clip data as raw bytes.</para>
        /// </summary>
        /// <returns>
        /// <para>A copy of the downloaded data.</para>
        /// </returns>
        protected override byte[] GetData() => 
            this.InternalGetData();

        protected override string GetText()
        {
            throw new NotSupportedException("String access is not supported for audio clips");
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern byte[] InternalGetData();
        /// <summary>
        /// <para>Returns the downloaded AudioClip, or null. (Read Only)</para>
        /// </summary>
        public AudioClip audioClip { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
        /// <summary>
        /// <para>Returns the downloaded AudioClip, or null.</para>
        /// </summary>
        /// <param name="www">A finished UnityWebRequest object with DownloadHandlerAudioClip attached.</param>
        /// <returns>
        /// <para>The same as DownloadHandlerAudioClip.audioClip</para>
        /// </returns>
        public static AudioClip GetContent(UnityWebRequest www) => 
            DownloadHandler.GetCheckedDownloader<DownloadHandlerAudioClip>(www).audioClip;
    }
}

