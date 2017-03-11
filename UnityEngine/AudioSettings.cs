namespace UnityEngine
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Controls the global audio settings from script.</para>
    /// </summary>
    public sealed class AudioSettings
    {
        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public static  event AudioConfigurationChangeHandler OnAudioConfigurationChanged;

        /// <summary>
        /// <para>Returns the current configuration of the audio device and system. The values in the struct may then be modified and reapplied via AudioSettings.Reset.</para>
        /// </summary>
        /// <returns>
        /// <para>The new configuration to be applied.</para>
        /// </returns>
        public static AudioConfiguration GetConfiguration()
        {
            AudioConfiguration configuration;
            INTERNAL_CALL_GetConfiguration(out configuration);
            return configuration;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void GetDSPBufferSize(out int bufferLength, out int numBuffers);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetConfiguration(out AudioConfiguration value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_Reset(ref AudioConfiguration config);
        [RequiredByNativeCode]
        internal static void InvokeOnAudioConfigurationChanged(bool deviceWasChanged)
        {
            if (OnAudioConfigurationChanged != null)
            {
                OnAudioConfigurationChanged(deviceWasChanged);
            }
        }

        /// <summary>
        /// <para>Performs a change of the device configuration. In response to this the AudioSettings.OnAudioConfigurationChanged delegate is invoked with the argument deviceWasChanged=false. It cannot be guaranteed that the exact settings specified can be used, but the an attempt is made to use the closest match supported by the system.</para>
        /// </summary>
        /// <param name="config">The new configuration to be used.</param>
        /// <returns>
        /// <para>True if all settings could be successfully applied.</para>
        /// </returns>
        public static bool Reset(AudioConfiguration config) => 
            INTERNAL_CALL_Reset(ref config);

        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("AudioSettings.SetDSPBufferSize is deprecated and has been replaced by audio project settings and the AudioSettings.GetConfiguration/AudioSettings.Reset API."), GeneratedByOldBindingsGenerator]
        public static extern void SetDSPBufferSize(int bufferLength, int numBuffers);

        /// <summary>
        /// <para>Returns the speaker mode capability of the current audio driver. (Read Only)</para>
        /// </summary>
        public static AudioSpeakerMode driverCapabilities { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("AudioSettings.driverCaps is obsolete. Use driverCapabilities instead (UnityUpgradable) -> driverCapabilities", true)]
        public static AudioSpeakerMode driverCaps =>
            driverCapabilities;

        /// <summary>
        /// <para>Returns the current time of the audio system.</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public static double dspTime { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        internal static bool editingInPlaymode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Get the mixer's current output rate.</para>
        /// </summary>
        public static int outputSampleRate { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        internal static int profilerCaptureFlags { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Gets the current speaker mode. Default is 2 channel stereo.</para>
        /// </summary>
        public static AudioSpeakerMode speakerMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        internal static bool unityAudioDisabled { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>A delegate called whenever the global audio settings are changed, either by AudioSettings.Reset or by an external device change such as the OS control panel changing the sample rate or because the default output device was changed, for example when plugging in an HDMI monitor or a USB headset.</para>
        /// </summary>
        /// <param name="deviceWasChanged">True if the change was caused by an device change.</param>
        public delegate void AudioConfigurationChangeHandler(bool deviceWasChanged);
    }
}

