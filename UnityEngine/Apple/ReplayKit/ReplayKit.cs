namespace UnityEngine.Apple.ReplayKit
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Internal;

    /// <summary>
    /// <para>ReplayKit is only available on certain iPhone, iPad and iPod Touch devices running iOS 9.0 or later.</para>
    /// </summary>
    public static class ReplayKit
    {
        /// <summary>
        /// <para>Discard the current recording.</para>
        /// </summary>
        /// <returns>
        /// <para>A boolean value of True if the recording was discarded successfully or False if an error occurred.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool Discard();
        /// <summary>
        /// <para>Hide the camera preview view.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void HideCameraPreview();
        /// <summary>
        /// <para>Preview the current recording</para>
        /// </summary>
        /// <returns>
        /// <para>A boolean value of True if the video preview window opened successfully or False if an error occurred.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool Preview();
        /// <summary>
        /// <para>Shows camera preview at coordinates posX and posY.</para>
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool ShowCameraPreviewAt(float posX, float posY);
        [ExcludeFromDocs]
        public static void StartBroadcasting(BroadcastStatusCallback callback)
        {
            bool enableCamera = false;
            bool enableMicrophone = false;
            StartBroadcasting(callback, enableMicrophone, enableCamera);
        }

        [ExcludeFromDocs]
        public static void StartBroadcasting(BroadcastStatusCallback callback, bool enableMicrophone)
        {
            bool enableCamera = false;
            StartBroadcasting(callback, enableMicrophone, enableCamera);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void StartBroadcasting(BroadcastStatusCallback callback, [DefaultValue("false")] bool enableMicrophone, [DefaultValue("false")] bool enableCamera);
        [ExcludeFromDocs]
        public static bool StartRecording()
        {
            bool enableCamera = false;
            bool enableMicrophone = false;
            return StartRecording(enableMicrophone, enableCamera);
        }

        [ExcludeFromDocs]
        public static bool StartRecording(bool enableMicrophone)
        {
            bool enableCamera = false;
            return StartRecording(enableMicrophone, enableCamera);
        }

        /// <summary>
        /// <para>Start a new recording.</para>
        /// </summary>
        /// <param name="enableMicrophone">Enable or disable the microphone while making a recording. Enabling the microphone allows you to include user commentary while recording. The default value is false.</param>
        /// <param name="enableCamera">Enable or disable the camera while making a recording. Enabling camera allows you to include user camera footage while recording. The default value is false. To actually include camera footage in your recording, you also have to call ShowCameraPreviewAt as well to position the preview view.</param>
        /// <returns>
        /// <para>A boolean value of True if recording started successfully or False if an error occurred.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool StartRecording([DefaultValue("false")] bool enableMicrophone, [DefaultValue("false")] bool enableCamera);
        /// <summary>
        /// <para>Stops current broadcast. 
        /// Will terminate currently on-going broadcast. If no broadcast is in progress, does nothing.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void StopBroadcasting();
        /// <summary>
        /// <para>Stop the current recording.</para>
        /// </summary>
        /// <returns>
        /// <para>A boolean value of True if recording stopped successfully or False if an error occurred.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool StopRecording();

        /// <summary>
        /// <para>A boolean that indicates whether the ReplayKit API is available (where True means available). (Read Only)</para>
        /// </summary>
        public static bool APIAvailable { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>A Boolean that indicates whether ReplayKit broadcasting API is available (true means available) (Read Only).
        /// Check the value of this property before making ReplayKit broadcasting API calls. On iOS versions prior to iOS 10, this property will have a value of false.</para>
        /// </summary>
        public static bool broadcastingAPIAvailable { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>A string property that contains an URL used to redirect the user to an on-going or completed broadcast (Read Only).</para>
        /// </summary>
        public static string broadcastURL { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Camera enabled status, true, if camera enabled, false otherwise.</para>
        /// </summary>
        public static bool cameraEnabled { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Boolean property that indicates whether a broadcast is currently in progress (Read Only).</para>
        /// </summary>
        public static bool isBroadcasting { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>A boolean that indicates whether ReplayKit is making a recording (where True means a recording is in progress). (Read Only)</para>
        /// </summary>
        public static bool isRecording { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>A string value of the last error incurred by the ReplayKit: Either 'Failed to get Screen Recorder' or 'No recording available'. (Read Only)</para>
        /// </summary>
        public static string lastError { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Microphone enabled status, true, if microhone enabled, false otherwise.</para>
        /// </summary>
        public static bool microphoneEnabled { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>A boolean value that indicates that a new recording is available for preview (where True means available). (Read Only)</para>
        /// </summary>
        public static bool recordingAvailable { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Function called at the completion of broadcast startup.</para>
        /// </summary>
        /// <param name="hasStarted">This parameter will be true if the broadcast started successfully and false in the event of an error.</param>
        /// <param name="errorMessage">In the event of failure to start a broadcast, this parameter contains the associated error message.</param>
        public delegate void BroadcastStatusCallback(bool hasStarted, string errorMessage);
    }
}

