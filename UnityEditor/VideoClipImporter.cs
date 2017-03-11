namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>VideoClipImporter lets you modify Video.VideoClip import settings from Editor scripts.</para>
    /// </summary>
    public sealed class VideoClipImporter : AssetImporter
    {
        /// <summary>
        /// <para>Clear the platform-specific import settings for the specified platform, causing them to go back to the default settings.</para>
        /// </summary>
        /// <param name="platform">Platform name.</param>
        public void ClearTargetSettings(string platform)
        {
            if (platform.Equals(defaultTargetName, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Cannot clear the Default VideoClipTargetSettings.");
            }
            BuildTargetGroup buildTargetGroupByName = BuildPipeline.GetBuildTargetGroupByName(platform);
            if (buildTargetGroupByName == BuildTargetGroup.Unknown)
            {
                throw new ArgumentException("Unknown platform passed to AudioImporter.GetOverrideSampleSettings (" + platform + "), please use one of 'Web', 'Standalone', 'iOS', 'Android', 'WebGL', 'PS4', 'PSP2', 'PSM', 'XBox360', 'XboxOne', 'WP8', or 'WSA'");
            }
            this.Internal_ClearTargetSettings(buildTargetGroupByName);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern bool EqualsDefaultTargetSettings(VideoImporterTargetSettings settings);
        /// <summary>
        /// <para>Returns a texture with the transcoded clip's current frame.
        /// Returns frame 0 when not playing, and frame at current time when playing.</para>
        /// </summary>
        /// <returns>
        /// <para>Texture containing the current frame.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern Texture GetPreviewTexture();
        /// <summary>
        /// <para>Get the resulting height of the resize operation for the specified resize mode.</para>
        /// </summary>
        /// <param name="mode">Mode for which the height is queried.</param>
        /// <returns>
        /// <para>Height for the specified resize mode.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern int GetResizeHeight(VideoResizeMode mode);
        /// <summary>
        /// <para>Get the full name of the resize operation for the specified resize mode.</para>
        /// </summary>
        /// <param name="mode">Mode for which the width is queried.</param>
        /// <returns>
        /// <para>Name for the specified resize mode.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern string GetResizeModeName(VideoResizeMode mode);
        /// <summary>
        /// <para>Get the resulting width of the resize operation for the specified resize mode.</para>
        /// </summary>
        /// <param name="mode">Mode for which the width is queried.</param>
        /// <returns>
        /// <para>Width for the specified resize mode.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern int GetResizeWidth(VideoResizeMode mode);
        /// <summary>
        /// <para>Number of audio channels in the specified source track.</para>
        /// </summary>
        /// <param name="audioTrackIdx">Index of the audio track to query.</param>
        /// <returns>
        /// <para>Number of channels.</para>
        /// </returns>
        public ushort GetSourceAudioChannelCount(ushort audioTrackIdx) => 
            INTERNAL_CALL_GetSourceAudioChannelCount(this, audioTrackIdx);

        /// <summary>
        /// <para>Sample rate of the specified audio track.</para>
        /// </summary>
        /// <param name="audioTrackIdx">Index of the audio track to query.</param>
        /// <returns>
        /// <para>Sample rate in Hertz.</para>
        /// </returns>
        public uint GetSourceAudioSampleRate(ushort audioTrackIdx) => 
            INTERNAL_CALL_GetSourceAudioSampleRate(this, audioTrackIdx);

        /// <summary>
        /// <para>Returns the platform-specific import settings for the specified platform.</para>
        /// </summary>
        /// <param name="platform">Platform name.</param>
        /// <returns>
        /// <para>The platform-specific import settings.  Throws an exception if the platform is unknown.</para>
        /// </returns>
        public VideoImporterTargetSettings GetTargetSettings(string platform)
        {
            BuildTargetGroup buildTargetGroupByName = BuildPipeline.GetBuildTargetGroupByName(platform);
            if (!platform.Equals(defaultTargetName, StringComparison.OrdinalIgnoreCase) && (buildTargetGroupByName == BuildTargetGroup.Unknown))
            {
                throw new ArgumentException("Unknown platform passed to AudioImporter.GetOverrideSampleSettings (" + platform + "), please use one of 'Default', 'Web', 'Standalone', 'iOS', 'Android', 'WebGL', 'PS4', 'PSP2', 'PSM', 'XBox360', 'XboxOne', 'WP8', or 'WSA'");
            }
            return this.Internal_GetTargetSettings(buildTargetGroupByName);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern ushort INTERNAL_CALL_GetSourceAudioChannelCount(VideoClipImporter self, ushort audioTrackIdx);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern uint INTERNAL_CALL_GetSourceAudioSampleRate(VideoClipImporter self, ushort audioTrackIdx);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void Internal_ClearTargetSettings(BuildTargetGroup group);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern VideoImporterTargetSettings Internal_GetTargetSettings(BuildTargetGroup group);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void Internal_SetTargetSettings(BuildTargetGroup group, VideoImporterTargetSettings settings);
        /// <summary>
        /// <para>Starts preview playback.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void PlayPreview();
        /// <summary>
        /// <para>Sets the platform-specific import settings for the specified platform.</para>
        /// </summary>
        /// <param name="platform">Platform name.</param>
        /// <param name="settings">The new platform-specific import settings.</param>
        public void SetTargetSettings(string platform, VideoImporterTargetSettings settings)
        {
            BuildTargetGroup buildTargetGroupByName = BuildPipeline.GetBuildTargetGroupByName(platform);
            if (!platform.Equals(defaultTargetName, StringComparison.OrdinalIgnoreCase) && (buildTargetGroupByName == BuildTargetGroup.Unknown))
            {
                throw new ArgumentException("Unknown platform passed to AudioImporter.GetOverrideSampleSettings (" + platform + "), please use one of 'Default', 'Web', 'Standalone', 'iOS', 'Android', 'WebGL', 'PS4', 'PSP2', 'PSM', 'XBox360', 'XboxOne', 'WP8', or 'WSA'");
            }
            this.Internal_SetTargetSettings(buildTargetGroupByName, settings);
        }

        /// <summary>
        /// <para>Stops preview playback.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void StopPreview();

        internal static string defaultTargetName { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Default values for the platform-specific import settings.</para>
        /// </summary>
        public VideoImporterTargetSettings defaultTargetSettings
        {
            get => 
                this.GetTargetSettings(defaultTargetName);
            set
            {
                this.SetTargetSettings(defaultTargetName, value);
            }
        }

        /// <summary>
        /// <para>Images are deinterlaced during transcode.  This tells the importer how to interpret fields in the source, if any.</para>
        /// </summary>
        public VideoDeinterlaceMode deinterlaceMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Apply a horizontal flip during import.</para>
        /// </summary>
        public bool flipHorizontal { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Apply a vertical flip during import.</para>
        /// </summary>
        public bool flipVertical { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Number of frames in the clip.</para>
        /// </summary>
        public int frameCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Frame rate of the clip.</para>
        /// </summary>
        public double frameRate { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Import audio tracks from source file.</para>
        /// </summary>
        public bool importAudio { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Whether the preview is currently playing.</para>
        /// </summary>
        public bool isPlayingPreview { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Whether to keep the alpha from the source into the transcoded clip.</para>
        /// </summary>
        public bool keepAlpha { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Used in legacy import mode.  Same as MovieImport.linearTexture.</para>
        /// </summary>
        public bool linearColor { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Size in bytes of the file once imported.</para>
        /// </summary>
        public ulong outputFileSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Used in legacy import mode.  Same as MovieImport.quality.</para>
        /// </summary>
        public float quality { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Number of audio tracks in the source file.</para>
        /// </summary>
        public ushort sourceAudioTrackCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Size in bytes of the file before importing.</para>
        /// </summary>
        public ulong sourceFileSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>True if the source file has a channel for per-pixel transparency.</para>
        /// </summary>
        public bool sourceHasAlpha { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Whether to import a MovieTexture (legacy) or a VideoClip.</para>
        /// </summary>
        public bool useLegacyImporter { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

