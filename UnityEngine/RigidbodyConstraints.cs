namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>Use these flags to constrain motion of Rigidbodies.</para>
    /// </summary>
    public enum RigidbodyConstraints
    {
        /// <summary>
        /// <para>Freeze rotation and motion along all axes.</para>
        /// </summary>
        FreezeAll = 0x7e,
        /// <summary>
        /// <para>Freeze motion along all axes.</para>
        /// </summary>
        FreezePosition = 14,
        /// <summary>
        /// <para>Freeze motion along the X-axis.</para>
        /// </summary>
        FreezePositionX = 2,
        /// <summary>
        /// <para>Freeze motion along the Y-axis.</para>
        /// </summary>
        FreezePositionY = 4,
        /// <summary>
        /// <para>Freeze motion along the Z-axis.</para>
        /// </summary>
        FreezePositionZ = 8,
        /// <summary>
        /// <para>Freeze rotation along all axes.</para>
        /// </summary>
        FreezeRotation = 0x70,
        /// <summary>
        /// <para>Freeze rotation along the X-axis.</para>
        /// </summary>
        FreezeRotationX = 0x10,
        /// <summary>
        /// <para>Freeze rotation along the Y-axis.</para>
        /// </summary>
        FreezeRotationY = 0x20,
        /// <summary>
        /// <para>Freeze rotation along the Z-axis.</para>
        /// </summary>
        FreezeRotationZ = 0x40,
        /// <summary>
        /// <para>No constraints.</para>
        /// </summary>
        None = 0
    }
}

