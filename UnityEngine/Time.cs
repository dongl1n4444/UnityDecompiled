namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>The interface to get time information from Unity.</para>
    /// </summary>
    public sealed class Time
    {
        /// <summary>
        /// <para>Slows game playback time to allow screenshots to be saved between frames.</para>
        /// </summary>
        public static int captureFramerate { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The time in seconds it took to complete the last frame (Read Only).</para>
        /// </summary>
        public static float deltaTime { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The interval in seconds at which physics and other fixed frame rate updates (like MonoBehaviour's MonoBehaviour.FixedUpdate) are performed.</para>
        /// </summary>
        public static float fixedDeltaTime { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The time the latest MonoBehaviour.FixedUpdate has started (Read Only). This is the time in seconds since the start of the game.</para>
        /// </summary>
        public static float fixedTime { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The total number of frames that have passed (Read Only).</para>
        /// </summary>
        public static int frameCount { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The maximum time a frame can take. Physics and other fixed frame rate updates (like MonoBehaviour's MonoBehaviour.FixedUpdate).</para>
        /// </summary>
        public static float maximumDeltaTime { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The maximum time a frame can spend on particle updates. If the frame takes longer than this, then updates are split into multiple smaller updates.</para>
        /// </summary>
        public static float maximumParticleDeltaTime { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The real time in seconds since the game started (Read Only).</para>
        /// </summary>
        public static float realtimeSinceStartup { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public static int renderedFrameCount { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>A smoothed out Time.deltaTime (Read Only).</para>
        /// </summary>
        public static float smoothDeltaTime { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The time at the beginning of this frame (Read Only). This is the time in seconds since the start of the game.</para>
        /// </summary>
        public static float time { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The scale at which the time is passing. This can be used for slow motion effects.</para>
        /// </summary>
        public static float timeScale { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The time this frame has started (Read Only). This is the time in seconds since the last level has been loaded.</para>
        /// </summary>
        public static float timeSinceLevelLoad { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The timeScale-independent time in seconds it took to complete the last frame (Read Only).</para>
        /// </summary>
        public static float unscaledDeltaTime { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The timeScale-independant time at the beginning of this frame (Read Only). This is the time in seconds since the start of the game.</para>
        /// </summary>
        public static float unscaledTime { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

