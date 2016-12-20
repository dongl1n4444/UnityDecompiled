namespace UnityEditor.Animations
{
    using System;

    /// <summary>
    /// <para>The mode of the condition.</para>
    /// </summary>
    public enum AnimatorConditionMode
    {
        /// <summary>
        /// <para>The condition is true when parameter value is equal to the threshold.</para>
        /// </summary>
        Equals = 6,
        /// <summary>
        /// <para>The condition is true when parameter value is greater than the threshold.</para>
        /// </summary>
        Greater = 3,
        /// <summary>
        /// <para>The condition is true when the parameter value is true.</para>
        /// </summary>
        If = 1,
        /// <summary>
        /// <para>The condition is true when the parameter value is false.</para>
        /// </summary>
        IfNot = 2,
        /// <summary>
        /// <para>The condition is true when the parameter value is less than the threshold.</para>
        /// </summary>
        Less = 4,
        /// <summary>
        /// <para>The condition is true when the parameter value is not equal to the threshold.</para>
        /// </summary>
        NotEqual = 7
    }
}

