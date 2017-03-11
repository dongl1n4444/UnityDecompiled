namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>The lighting data asset used by the active scene.</para>
    /// </summary>
    public sealed class LightingDataAsset : UnityEngine.Object
    {
        internal bool isValid { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        internal string validityErrorMessage { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

