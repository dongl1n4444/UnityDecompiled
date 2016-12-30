namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>This structure contains the frame information a Playable receives in Playable.PrepareFrame.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FrameData
    {
        internal ulong m_FrameID;
        internal double m_DeltaTime;
        internal float m_Weight;
        internal float m_EffectiveWeight;
        internal float m_EffectiveSpeed;
        internal Flags m_Flags;
        /// <summary>
        /// <para>The current frame identifier.</para>
        /// </summary>
        public ulong frameId =>
            this.m_FrameID;
        /// <summary>
        /// <para>Time difference between this frame and the preceding frame.</para>
        /// </summary>
        public float deltaTime =>
            ((float) this.m_DeltaTime);
        /// <summary>
        /// <para>The weight of the current Playable.</para>
        /// </summary>
        public float weight =>
            this.m_Weight;
        /// <summary>
        /// <para>The accumulated weight of the Playable during the PlayableGraph traversal.</para>
        /// </summary>
        public float effectiveWeight =>
            this.m_EffectiveWeight;
        /// <summary>
        /// <para>The accumulated speed of the Playable during the PlayableGraph traversal.</para>
        /// </summary>
        public float effectiveSpeed =>
            this.m_EffectiveSpeed;
        /// <summary>
        /// <para>Indicates the type of evaluation that caused PlayableGraph.PrepareFrame to be called.</para>
        /// </summary>
        public EvaluationType evaluationType =>
            (((this.m_Flags & Flags.Evaluate) == 0) ? EvaluationType.Playback : EvaluationType.Evaluate);
        /// <summary>
        /// <para>Indicates that the local time was explicitly set.</para>
        /// </summary>
        public bool seekOccurred =>
            ((this.m_Flags & Flags.SeekOccured) != 0);
        /// <summary>
        /// <para>Describes the cause for the evaluation of a PlayableGraph.</para>
        /// </summary>
        public enum EvaluationType
        {
            Evaluate,
            Playback
        }

        [Flags]
        internal enum Flags
        {
            Evaluate = 1,
            SeekOccured = 2
        }
    }
}

