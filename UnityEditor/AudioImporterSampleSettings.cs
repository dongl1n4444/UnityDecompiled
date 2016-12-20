namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>This structure contains a collection of settings used to define how an AudioClip should be imported.
    /// 
    /// This  structure is used with the AudioImporter to define how the AudioClip should be imported and treated during loading within the scene.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AudioImporterSampleSettings
    {
        /// <summary>
        /// <para>LoadType defines how the imported AudioClip data should be loaded.</para>
        /// </summary>
        public AudioClipLoadType loadType;
        /// <summary>
        /// <para>Defines how the sample rate is modified (if at all) of the importer audio file.</para>
        /// </summary>
        public AudioSampleRateSetting sampleRateSetting;
        /// <summary>
        /// <para>Target sample rate to convert to when samplerateSetting is set to OverrideSampleRate.</para>
        /// </summary>
        public uint sampleRateOverride;
        /// <summary>
        /// <para>CompressionFormat defines the compression type that the audio file is encoded to. Different compression types have different performance and audio artifact characteristics.</para>
        /// </summary>
        public AudioCompressionFormat compressionFormat;
        /// <summary>
        /// <para>Audio compression quality (0-1)
        /// 
        /// Amount of compression. The value roughly corresponds to the ratio between the resulting and the source file sizes.</para>
        /// </summary>
        public float quality;
        public int conversionMode;
    }
}

