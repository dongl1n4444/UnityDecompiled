namespace UnityEngine.SocialPlatforms
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>The score range a leaderboard query should include.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Range
    {
        /// <summary>
        /// <para>The rank of the first score which is returned.</para>
        /// </summary>
        public int from;
        /// <summary>
        /// <para>The total amount of scores retreived.</para>
        /// </summary>
        public int count;
        /// <summary>
        /// <para>Constructor for a score range, the range starts from a specific value and contains a maxium score count.</para>
        /// </summary>
        /// <param name="fromValue">The minimum allowed value.</param>
        /// <param name="valueCount">The number of possible values.</param>
        public Range(int fromValue, int valueCount)
        {
            this.from = fromValue;
            this.count = valueCount;
        }
    }
}

