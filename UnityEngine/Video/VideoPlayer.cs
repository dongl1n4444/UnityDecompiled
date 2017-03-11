namespace UnityEngine.Video
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Plays video content onto a target.</para>
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public sealed class VideoPlayer : Behaviour
    {
        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event ErrorEventHandler errorReceived;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event EventHandler frameDropped;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event FrameReadyEventHandler frameReady;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event EventHandler loopPointReached;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event EventHandler prepareCompleted;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event EventHandler seekCompleted;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event EventHandler started;

        /// <summary>
        /// <para>Enable/disable audio track decoding. Only effective when the VideoPlayer is not currently playing.</para>
        /// </summary>
        /// <param name="trackIndex">Index of the audio track to enable/disable.</param>
        /// <param name="enabled">True for enabling the track. False for disabling the track.</param>
        public void EnableAudioTrack(ushort trackIndex, bool enabled)
        {
            INTERNAL_CALL_EnableAudioTrack(this, trackIndex, enabled);
        }

        /// <summary>
        /// <para>The number of audio channels in the specified audio track.</para>
        /// </summary>
        /// <param name="trackIndex">Index for the audio track being queried.</param>
        /// <returns>
        /// <para>Number of audio channels.</para>
        /// </returns>
        public ushort GetAudioChannelCount(ushort trackIndex) => 
            INTERNAL_CALL_GetAudioChannelCount(this, trackIndex);

        /// <summary>
        /// <para>Returns the language code, if any, for the specified track.</para>
        /// </summary>
        /// <param name="trackIndex">Index of the audio track to query.</param>
        /// <returns>
        /// <para>Language code.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern string GetAudioLanguageCode(ushort trackIndex);
        /// <summary>
        /// <para>Get the direct-output audio mute status for the specified track.</para>
        /// </summary>
        /// <param name="trackIndex"></param>
        public bool GetDirectAudioMute(ushort trackIndex) => 
            INTERNAL_CALL_GetDirectAudioMute(this, trackIndex);

        /// <summary>
        /// <para>Return the direct-output volume for specified track.</para>
        /// </summary>
        /// <param name="trackIndex">Track index for which the volume is queried.</param>
        /// <returns>
        /// <para>Volume, between 0 and 1.</para>
        /// </returns>
        public float GetDirectAudioVolume(ushort trackIndex) => 
            INTERNAL_CALL_GetDirectAudioVolume(this, trackIndex);

        /// <summary>
        /// <para>Gets the AudioSource that will receive audio samples for the specified track if Video.VideoPlayer.audioOutputMode is set to Video.VideoAudioOutputMode.AudioSource.</para>
        /// </summary>
        /// <param name="trackIndex">Index of the audio track for which the AudioSource is wanted.</param>
        /// <returns>
        /// <para>The source associated with the audio track.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern AudioSource GetTargetAudioSource(ushort trackIndex);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_EnableAudioTrack(VideoPlayer self, ushort trackIndex, bool enabled);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern ushort INTERNAL_CALL_GetAudioChannelCount(VideoPlayer self, ushort trackIndex);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_GetDirectAudioMute(VideoPlayer self, ushort trackIndex);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern float INTERNAL_CALL_GetDirectAudioVolume(VideoPlayer self, ushort trackIndex);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_IsAudioTrackEnabled(VideoPlayer self, ushort trackIndex);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Pause(VideoPlayer self);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Play(VideoPlayer self);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Prepare(VideoPlayer self);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetDirectAudioMute(VideoPlayer self, ushort trackIndex, bool mute);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetDirectAudioVolume(VideoPlayer self, ushort trackIndex, float volume);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetTargetAudioSource(VideoPlayer self, ushort trackIndex, AudioSource source);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Stop(VideoPlayer self);
        [RequiredByNativeCode]
        private static void InvokeErrorReceivedCallback_Internal(VideoPlayer source, string errorStr)
        {
            if (source.errorReceived != null)
            {
                source.errorReceived(source, errorStr);
            }
        }

        [RequiredByNativeCode]
        private static void InvokeFrameDroppedCallback_Internal(VideoPlayer source)
        {
            if (source.frameDropped != null)
            {
                source.frameDropped(source);
            }
        }

        [RequiredByNativeCode]
        private static void InvokeFrameReadyCallback_Internal(VideoPlayer source, long frameIdx)
        {
            if (source.frameReady != null)
            {
                source.frameReady(source, frameIdx);
            }
        }

        [RequiredByNativeCode]
        private static void InvokeLoopPointReachedCallback_Internal(VideoPlayer source)
        {
            if (source.loopPointReached != null)
            {
                source.loopPointReached(source);
            }
        }

        [RequiredByNativeCode]
        private static void InvokePrepareCompletedCallback_Internal(VideoPlayer source)
        {
            if (source.prepareCompleted != null)
            {
                source.prepareCompleted(source);
            }
        }

        [RequiredByNativeCode]
        private static void InvokeSeekCompletedCallback_Internal(VideoPlayer source)
        {
            if (source.seekCompleted != null)
            {
                source.seekCompleted(source);
            }
        }

        [RequiredByNativeCode]
        private static void InvokeStartedCallback_Internal(VideoPlayer source)
        {
            if (source.started != null)
            {
                source.started(source);
            }
        }

        /// <summary>
        /// <para>Returns whether decoding for the specified audio track is enabled. See Video.VideoPlayer.EnableAudioTrack for distinction with mute.</para>
        /// </summary>
        /// <param name="trackIndex">Index of the audio track being queried.</param>
        /// <returns>
        /// <para>True if decoding for the specified audio track is enabled.</para>
        /// </returns>
        public bool IsAudioTrackEnabled(ushort trackIndex) => 
            INTERNAL_CALL_IsAudioTrackEnabled(this, trackIndex);

        /// <summary>
        /// <para>Pauses the playback and leaves the current time intact.</para>
        /// </summary>
        public void Pause()
        {
            INTERNAL_CALL_Pause(this);
        }

        /// <summary>
        /// <para>Starts playback.</para>
        /// </summary>
        public void Play()
        {
            INTERNAL_CALL_Play(this);
        }

        /// <summary>
        /// <para>Initiates playback engine prepration.</para>
        /// </summary>
        public void Prepare()
        {
            INTERNAL_CALL_Prepare(this);
        }

        /// <summary>
        /// <para>Set the direct-output audio mute status for the specified track.</para>
        /// </summary>
        /// <param name="trackIndex">Track index for which the mute is set.</param>
        /// <param name="mute">Mute on/off.</param>
        public void SetDirectAudioMute(ushort trackIndex, bool mute)
        {
            INTERNAL_CALL_SetDirectAudioMute(this, trackIndex, mute);
        }

        /// <summary>
        /// <para>Set the direct-output audio volume for the specified track.</para>
        /// </summary>
        /// <param name="trackIndex">Track index for which the volume is set.</param>
        /// <param name="volume">New volume, between 0 and 1.</param>
        public void SetDirectAudioVolume(ushort trackIndex, float volume)
        {
            INTERNAL_CALL_SetDirectAudioVolume(this, trackIndex, volume);
        }

        /// <summary>
        /// <para>Sets the AudioSource that will receive audio samples for the specified track if this audio target is selected with Video.VideoPlayer.audioOutputMode.</para>
        /// </summary>
        /// <param name="trackIndex">Index of the audio track to associate with the specified AudioSource.</param>
        /// <param name="source">AudioSource to associate with the audio track.</param>
        public void SetTargetAudioSource(ushort trackIndex, AudioSource source)
        {
            INTERNAL_CALL_SetTargetAudioSource(this, trackIndex, source);
        }

        /// <summary>
        /// <para>Advances the current time by one frame immediately.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void StepForward();
        /// <summary>
        /// <para>Pauses the playback and sets the current time to 0.</para>
        /// </summary>
        public void Stop()
        {
            INTERNAL_CALL_Stop(this);
        }

        /// <summary>
        /// <para>Defines how the video content will be stretched to fill the target area.</para>
        /// </summary>
        public VideoAspectRatio aspectRatio { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Destination for the audio embedded in the video.</para>
        /// </summary>
        public VideoAudioOutputMode audioOutputMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Number of audio tracks found in the data source currently configured.</para>
        /// </summary>
        public ushort audioTrackCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Whether direct-output volume controls are supported for the current platform and video format. (Read Only)</para>
        /// </summary>
        public bool canSetDirectAudioVolume { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Whether the playback speed can be changed. (Read Only)</para>
        /// </summary>
        public bool canSetPlaybackSpeed { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Determines whether the player skips frames to catch up with current time. (Read Only)</para>
        /// </summary>
        public bool canSetSkipOnDrop { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Whether current time can be changed using the time or timeFrames property. (Read Only)</para>
        /// </summary>
        public bool canSetTime { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Whether the time source followed by the video player can be changed. (Read Only)</para>
        /// </summary>
        public bool canSetTimeSource { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Returns true if the player can step forwards into the video content. (Read Only)</para>
        /// </summary>
        public bool canStep { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The clip being played by the player.</para>
        /// </summary>
        public VideoClip clip { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Number of audio tracks that this VideoPlayer will take control of. The other ones will be silenced. A maximum of 64 tracks are allowed.
        /// The actual number of audio tracks cannot be known in advance when playing URLs, which is why this value is independent of the Video.VideoPlayer.audioTrackCount property.</para>
        /// </summary>
        public ushort controlledAudioTrackCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Maximum number of audio tracks that can be controlled.</para>
        /// </summary>
        public static ushort controlledAudioTrackMaxCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The frame index currently being displayed by the player.</para>
        /// </summary>
        public long frame { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Number of frames in the current video content.</para>
        /// </summary>
        public ulong frameCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The frame rate of the clip or URL in frames/second. (Read Only).</para>
        /// </summary>
        public float frameRate { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Deterimes whether the player restarts from the beginning without when it reaches the end of the clip.</para>
        /// </summary>
        public bool isLooping { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Whether content is being played. (Read Only)</para>
        /// </summary>
        public bool isPlaying { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Whether the player has successfully prepared the content to be played. (Read Only)</para>
        /// </summary>
        public bool isPrepared { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Factor by which the basic playback rate will be multiplied.</para>
        /// </summary>
        public float playbackSpeed { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Whether the content will start playing back as soon as the component awakes.</para>
        /// </summary>
        public bool playOnAwake { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Where the video content will be drawn.</para>
        /// </summary>
        public VideoRenderMode renderMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Enables the frameReady events.</para>
        /// </summary>
        public bool sendFrameReadyEvents { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Whether the player is allowed to skip frames to catch up with current time.</para>
        /// </summary>
        public bool skipOnDrop { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The source (Video.VideoClip or URL) that the player uses for playback.</para>
        /// </summary>
        public VideoSource source { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Camera component to draw to when Video.VideoPlayer.renderMode is set to either Video.VideoTarget.CameraBackPlane or Video.VideoTarget.CameraFrontPlane.</para>
        /// </summary>
        public Camera targetCamera { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Overall transparency level of the target camera plane video.</para>
        /// </summary>
        public float targetCameraAlpha { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Material which is targeted in the Renderer when Video.VideoPlayer.renderMode is set to Video.VideoTarget.MaterialOverride. An empty string indicates that the renderer's first material should be used.</para>
        /// </summary>
        public string targetMaterialName { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Material texture property which is targeted when Video.VideoPlayer.renderMode is set to Video.VideoTarget.MaterialOverride. An empty string indicates that the material's first texture property should be used.</para>
        /// </summary>
        public string targetMaterialProperty { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Renderer which is targeted when Video.VideoPlayer.renderMode is set to Video.VideoTarget.MaterialOverride</para>
        /// </summary>
        public Renderer targetMaterialRenderer { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>RenderTexture to draw to when Video.VideoPlayer.renderMode is set to Video.VideoTarget.RenderTexture.</para>
        /// </summary>
        public RenderTexture targetTexture { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Internal texture in which video content is placed.</para>
        /// </summary>
        public Texture texture { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The player current time in seconds.</para>
        /// </summary>
        public double time { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The clock that the player follows to derive its current time.</para>
        /// </summary>
        public VideoTimeSource timeSource { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The file or HTTP URL that the player will read content from.</para>
        /// </summary>
        public string url { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Determines whether the player will wait for the first frame to be loaded into the texture before starting playback when Video.VideoPlayer.playOnAwake is on.</para>
        /// </summary>
        public bool waitForFirstFrame { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Delegate type for VideoPlayer events that contain an error message.</para>
        /// </summary>
        /// <param name="source">The player that is emitting the event.</param>
        /// <param name="message">Message describing the error just encountered.</param>
        public delegate void ErrorEventHandler(VideoPlayer source, string message);

        /// <summary>
        /// <para>Delegate type for all parameter-less events emitted by VideoPlayers.</para>
        /// </summary>
        /// <param name="source">The player that is emitting the event.</param>
        public delegate void EventHandler(VideoPlayer source);

        /// <summary>
        /// <para>Delegate type for VideoPlayer events that carry a frame number.</para>
        /// </summary>
        /// <param name="source">The player that is emitting the event.</param>
        /// <param name="frameNum">The frame the player is now at.</param>
        /// <param name="frameIdx"></param>
        public delegate void FrameReadyEventHandler(VideoPlayer source, long frameIdx);
    }
}

