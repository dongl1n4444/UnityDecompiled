namespace UnityEngine.Rendering
{
    using System;

    /// <summary>
    /// <para>How much CPU usage to assign to the final lighting calculations at runtime.</para>
    /// </summary>
    public enum RealtimeGICPUUsage
    {
        /// <summary>
        /// <para>75% of the allowed CPU threads are used as worker threads.</para>
        /// </summary>
        High = 0x4b,
        /// <summary>
        /// <para>25% of the allowed CPU threads are used as worker threads.</para>
        /// </summary>
        Low = 0x19,
        /// <summary>
        /// <para>50% of the allowed CPU threads are used as worker threads.</para>
        /// </summary>
        Medium = 50,
        /// <summary>
        /// <para>100% of the allowed CPU threads are used as worker threads.</para>
        /// </summary>
        Unlimited = 100
    }
}

