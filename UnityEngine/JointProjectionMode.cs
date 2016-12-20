namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>Determines how to snap physics joints back to its constrained position when it drifts off too much.</para>
    /// </summary>
    public enum JointProjectionMode
    {
        /// <summary>
        /// <para>Don't snap at all.</para>
        /// </summary>
        None = 0,
        /// <summary>
        /// <para>Snap both position and rotation.</para>
        /// </summary>
        PositionAndRotation = 1,
        /// <summary>
        /// <para>Snap Position only.</para>
        /// </summary>
        [Obsolete("JointProjectionMode.PositionOnly is no longer supported", true)]
        PositionOnly = 2
    }
}

