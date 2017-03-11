namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Audio importer lets you modify AudioClip import settings from editor scripts.</para>
    /// </summary>
    public sealed class AudioImporter : AssetImporter
    {
        /// <summary>
        /// <para>Clears the sample settings override for the given platform.</para>
        /// </summary>
        /// <param name="platform">The platform to clear the overrides for.</param>
        /// <returns>
        /// <para>Returns true if any overrides were actually cleared.</para>
        /// </returns>
        public bool ClearSampleSettingOverride(string platform)
        {
            BuildTargetGroup buildTargetGroupByName = BuildPipeline.GetBuildTargetGroupByName(platform);
            if (buildTargetGroupByName == BuildTargetGroup.Unknown)
            {
                Debug.LogError("Unknown platform passed to AudioImporter.ClearSampleSettingOverride (" + platform + "), please use one of 'Web', 'Standalone', 'iOS', 'Android', 'WebGL', 'PS4', 'PSP2', 'PSM', 'XboxOne' or 'WSA'");
                return false;
            }
            return this.Internal_ClearSampleSettingOverride(buildTargetGroupByName);
        }

        /// <summary>
        /// <para>Returns whether a given build target has its sample settings currently overridden.</para>
        /// </summary>
        /// <param name="platform">The platform to query if this AudioImporter has an override for.</param>
        /// <returns>
        /// <para>Returns true if the platform is currently overriden in this AudioImporter.</para>
        /// </returns>
        public bool ContainsSampleSettingsOverride(string platform)
        {
            BuildTargetGroup buildTargetGroupByName = BuildPipeline.GetBuildTargetGroupByName(platform);
            if (buildTargetGroupByName == BuildTargetGroup.Unknown)
            {
                Debug.LogError("Unknown platform passed to AudioImporter.ContainsSampleSettingsOverride (" + platform + "), please use one of 'Web', 'Standalone', 'iOS', 'Android', 'WebGL', 'PS4', 'PSP2', 'PSM', 'XboxOne' or 'WSA'");
                return false;
            }
            return this.Internal_ContainsSampleSettingsOverride(buildTargetGroupByName);
        }

        /// <summary>
        /// <para>Return the current override settings for the given platform.</para>
        /// </summary>
        /// <param name="platform">The platform to get the override settings for.</param>
        /// <returns>
        /// <para>The override sample settings for the given platform.</para>
        /// </returns>
        public AudioImporterSampleSettings GetOverrideSampleSettings(string platform)
        {
            BuildTargetGroup buildTargetGroupByName = BuildPipeline.GetBuildTargetGroupByName(platform);
            if (buildTargetGroupByName == BuildTargetGroup.Unknown)
            {
                Debug.LogError("Unknown platform passed to AudioImporter.GetOverrideSampleSettings (" + platform + "), please use one of 'Web', 'Standalone', 'iOS', 'Android', 'WebGL', 'PS4', 'PSP2', 'PSM', 'XboxOne' or 'WSA'");
                return this.defaultSampleSettings;
            }
            return this.Internal_GetOverrideSampleSettings(buildTargetGroupByName);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern bool Internal_ClearSampleSettingOverride(BuildTargetGroup platform);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern bool Internal_ContainsSampleSettingsOverride(BuildTargetGroup platformGroup);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern AudioImporterSampleSettings Internal_GetOverrideSampleSettings(BuildTargetGroup platformGroup);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern bool Internal_GetPreloadAudioData();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern bool Internal_SetOverrideSampleSettings(BuildTargetGroup platformGroup, AudioImporterSampleSettings settings);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_SetPreloadAudioData(bool flag);
        [Obsolete("AudioImporter.maxBitrate is deprecated.", true)]
        internal int maxBitrate(AudioType type) => 
            0;

        [Obsolete("AudioImporter.minBitrate is deprecated.", true)]
        internal int minBitrate(AudioType type) => 
            0;

        /// <summary>
        /// <para>Sets the override sample settings for the given platform.</para>
        /// </summary>
        /// <param name="platform">The platform which will have the sample settings overridden.</param>
        /// <param name="settings">The override settings for the given platform.</param>
        /// <returns>
        /// <para>Returns true if the settings were successfully overriden. Some setting overrides are not possible for the given platform, in which case false is returned and the settings are not registered.</para>
        /// </returns>
        public bool SetOverrideSampleSettings(string platform, AudioImporterSampleSettings settings)
        {
            BuildTargetGroup buildTargetGroupByName = BuildPipeline.GetBuildTargetGroupByName(platform);
            if (buildTargetGroupByName == BuildTargetGroup.Unknown)
            {
                Debug.LogError("Unknown platform passed to AudioImporter.SetOverrideSampleSettings (" + platform + "), please use one of 'Web', 'Standalone', 'iOS', 'Android', 'WebGL', 'PS4', 'PSP2', 'PSM', 'XboxOne' or 'WSA'");
                return false;
            }
            return this.Internal_SetOverrideSampleSettings(buildTargetGroupByName, settings);
        }

        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("AudioImporter.updateOrigData is deprecated.", true), GeneratedByOldBindingsGenerator]
        internal extern void updateOrigData();

        [Obsolete("Setting and getting import channels is not used anymore (use forceToMono instead)", true)]
        public AudioImporterChannels channels
        {
            get => 
                AudioImporterChannels.Automatic;
            set
            {
            }
        }

        /// <summary>
        /// <para>Compression bitrate.</para>
        /// </summary>
        [Obsolete("AudioImporter.compressionBitrate is no longer supported", true)]
        public int compressionBitrate { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        internal int compSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        [Obsolete("Setting/Getting decompressOnLoad is deprecated. Use AudioImporterSampleSettings.loadType instead.")]
        private bool decompressOnLoad
        {
            get => 
                (this.defaultSampleSettings.loadType == AudioClipLoadType.DecompressOnLoad);
            set
            {
                AudioImporterSampleSettings defaultSampleSettings = this.defaultSampleSettings;
                defaultSampleSettings.loadType = !value ? AudioClipLoadType.CompressedInMemory : AudioClipLoadType.DecompressOnLoad;
                this.defaultSampleSettings = defaultSampleSettings;
            }
        }

        [Obsolete("AudioImporter.defaultBitrate is deprecated.", true)]
        internal int defaultBitrate { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The default sample settings for the AudioClip importer.</para>
        /// </summary>
        public AudioImporterSampleSettings defaultSampleSettings { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [Obsolete("AudioImporter.durationMS is deprecated.", true)]
        internal int durationMS { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Force this clip to mono?</para>
        /// </summary>
        public bool forceToMono { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [Obsolete("Setting and getting the compression format is not used anymore (use compressionFormat in defaultSampleSettings instead). Source audio file is assumed to be PCM Wav.")]
        private AudioImporterFormat format
        {
            get => 
                ((this.defaultSampleSettings.compressionFormat != AudioCompressionFormat.PCM) ? AudioImporterFormat.Compressed : AudioImporterFormat.Native);
            set
            {
                AudioImporterSampleSettings defaultSampleSettings = this.defaultSampleSettings;
                defaultSampleSettings.compressionFormat = (value != AudioImporterFormat.Native) ? AudioCompressionFormat.Vorbis : AudioCompressionFormat.PCM;
                this.defaultSampleSettings = defaultSampleSettings;
            }
        }

        [Obsolete("AudioImporter.frequency is deprecated.", true)]
        internal int frequency { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        [Obsolete("AudioImporter.hardware is no longer supported. All mixing of audio is done by software and only some platforms use hardware acceleration to perform decoding.")]
        public bool hardware { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Corresponding to the "Load In Background" flag in the AudioClip inspector, when this flag is set, the loading of the clip will happen delayed without blocking the main thread.</para>
        /// </summary>
        [Obsolete("loadInBackground is not used anymore. AudioClips will now always be loaded in separate threads.")]
        public bool loadInBackground
        {
            get => 
                true;
            set
            {
            }
        }

        [Obsolete("AudioImporter.loopable is no longer supported. All audio assets encoded by Unity are by default loopable.")]
        public bool loopable { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [Obsolete("AudioImporter.origChannelCount is deprecated.", true)]
        internal int origChannelCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        [Obsolete("AudioImporter.origFileSize is deprecated.", true)]
        internal int origFileSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        [Obsolete("AudioImporter.origIsCompressible is deprecated.", true)]
        internal bool origIsCompressible { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        [Obsolete("AudioImporter.origIsMonoForcable is deprecated.", true)]
        internal bool origIsMonoForcable { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        internal int origSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        [Obsolete("AudioImporter.origType is deprecated.", true)]
        internal AudioType origType =>
            AudioType.UNKNOWN;

        /// <summary>
        /// <para>Preloads audio data of the clip when the clip asset is loaded. When this flag is off, scripts have to call AudioClip.LoadAudioData() to load the data before the clip can be played. Properties like length, channels and format are available before the audio data has been loaded.</para>
        /// </summary>
        public bool preloadAudioData
        {
            get => 
                this.Internal_GetPreloadAudioData();
            set
            {
                this.Internal_SetPreloadAudioData(value);
            }
        }

        [Obsolete("AudioImporter.quality is no longer supported. Use AudioImporterSampleSettings.")]
        private float quality
        {
            get => 
                this.defaultSampleSettings.quality;
            set
            {
                AudioImporterSampleSettings defaultSampleSettings = this.defaultSampleSettings;
                defaultSampleSettings.quality = value;
                this.defaultSampleSettings = defaultSampleSettings;
            }
        }

        [Obsolete("AudioImporter.threeD is no longer supported")]
        public bool threeD { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

