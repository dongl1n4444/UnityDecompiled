namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>Priority of a thread.</para>
    /// </summary>
    public enum ThreadPriority
    {
        /// <summary>
        /// <para>Below normal thread priority.</para>
        /// </summary>
        BelowNormal = 1,
        /// <summary>
        /// <para>Highest thread priority.</para>
        /// </summary>
        High = 4,
        /// <summary>
        /// <para>Lowest thread priority.</para>
        /// </summary>
        Low = 0,
        /// <summary>
        /// <para>Normal thread priority.</para>
        /// </summary>
        Normal = 2
    }
}

