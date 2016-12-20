namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Target Android device architecture.</para>
    /// </summary>
    public enum AndroidTargetDevice
    {
        /// <summary>
        /// <para>ARMv7 only.</para>
        /// </summary>
        ARMv7 = 3,
        /// <summary>
        /// <para>All supported architectures.</para>
        /// </summary>
        FAT = 0,
        /// <summary>
        /// <para>Intel only.</para>
        /// </summary>
        x86 = 4
    }
}

