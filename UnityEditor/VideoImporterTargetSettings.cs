namespace UnityEditor
{
    using System;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Importer settings that can have platform-specific values.</para>
    /// </summary>
    [RequiredByNativeCode]
    public sealed class VideoImporterTargetSettings
    {
        /// <summary>
        /// <para>How the aspect ratio discrepancies, if any, will be handled if the chosen import resolution has a different ratio than the source.</para>
        /// </summary>
        public VideoEncodeAspectRatio aspectRatio;
        /// <summary>
        /// <para>Bit rate type for the transcoded clip.</para>
        /// </summary>
        public VideoBitrateMode bitrateMode;
        /// <summary>
        /// <para>Codec that the resulting VideoClip will use.</para>
        /// </summary>
        public VideoCodec codec;
        /// <summary>
        /// <para>Height of the transcoded clip when the resizeMode is set to custom.</para>
        /// </summary>
        public int customHeight;
        /// <summary>
        /// <para>Width of the transcoded clip when the resizeMode is set to custom.</para>
        /// </summary>
        public int customWidth;
        /// <summary>
        /// <para>Controls whether the movie file will be transcoded during import. When transcoding is not enabled, the file will be imported in its original format.</para>
        /// </summary>
        public bool enableTranscoding;
        /// <summary>
        /// <para>How to resize the images when going into the imported clip.</para>
        /// </summary>
        public VideoResizeMode resizeMode;
        /// <summary>
        /// <para>Controls an internal image resize, resulting in blurrier images but smaller image dimensions and file size.</para>
        /// </summary>
        public VideoSpatialQuality spatialQuality;
    }
}

