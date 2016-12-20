namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Suspends the coroutine execution for the given amount of seconds using scaled time.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public sealed class WaitForSeconds : YieldInstruction
    {
        internal float m_Seconds;
        /// <summary>
        /// <para>Creates a yield instruction to wait for a given number of seconds using scaled time.</para>
        /// </summary>
        /// <param name="seconds"></param>
        public WaitForSeconds(float seconds)
        {
            this.m_Seconds = seconds;
        }
    }
}

