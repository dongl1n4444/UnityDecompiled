namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>A heightmap based collider.</para>
    /// </summary>
    public sealed class TerrainCollider : Collider
    {
        /// <summary>
        /// <para>The terrain that stores the heightmap.</para>
        /// </summary>
        public TerrainData terrainData { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

