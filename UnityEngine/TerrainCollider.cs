namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>A heightmap based collider.</para>
    /// </summary>
    public sealed class TerrainCollider : Collider
    {
        /// <summary>
        /// <para>The terrain that stores the heightmap.</para>
        /// </summary>
        public TerrainData terrainData { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

