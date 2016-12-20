namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>A Takeinfo object contains all the information needed to describe a take.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct TakeInfo
    {
        /// <summary>
        /// <para>Take name as define from imported file.</para>
        /// </summary>
        public string name;
        /// <summary>
        /// <para>This is the default clip name for the clip generated for this take.</para>
        /// </summary>
        public string defaultClipName;
        /// <summary>
        /// <para>Start time in second.</para>
        /// </summary>
        public float startTime;
        /// <summary>
        /// <para>Stop time in second.</para>
        /// </summary>
        public float stopTime;
        /// <summary>
        /// <para>Start time in second.</para>
        /// </summary>
        public float bakeStartTime;
        /// <summary>
        /// <para>Stop time in second.</para>
        /// </summary>
        public float bakeStopTime;
        /// <summary>
        /// <para>Sample rate of the take.</para>
        /// </summary>
        public float sampleRate;
    }
}

