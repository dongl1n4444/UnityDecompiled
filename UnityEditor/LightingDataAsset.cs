namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// <para>The lighting data asset used by the active scene.</para>
    /// </summary>
    public sealed class LightingDataAsset : Object
    {
        internal bool isValid { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        internal string validityErrorMessage { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

