namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>Attribute used to make a float or int variable in a script be restricted to a specific range.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited=true, AllowMultiple=false)]
    public sealed class RangeAttribute : PropertyAttribute
    {
        public readonly float max;
        public readonly float min;

        /// <summary>
        /// <para>Attribute used to make a float or int variable in a script be restricted to a specific range.</para>
        /// </summary>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        public RangeAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}

