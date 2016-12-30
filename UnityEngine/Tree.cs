namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Tree Component for the tree creator.</para>
    /// </summary>
    public sealed class Tree : Component
    {
        /// <summary>
        /// <para>Data asociated to the Tree.</para>
        /// </summary>
        public ScriptableObject data { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Tells if there is wind data exported from SpeedTree are saved on this component.</para>
        /// </summary>
        public bool hasSpeedTreeWind { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

