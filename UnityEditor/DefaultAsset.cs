namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// <para>DefaultAsset is used for assets that does not have a specific type (yet).</para>
    /// </summary>
    public sealed class DefaultAsset : UnityEngine.Object
    {
        internal bool isWarning { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        internal string message { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

