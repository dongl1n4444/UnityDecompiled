namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Asynchronous create request for an AssetBundle.</para>
    /// </summary>
    [RequiredByNativeCode]
    public sealed class AssetBundleCreateRequest : AsyncOperation
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void DisableCompatibilityChecks();

        /// <summary>
        /// <para>Asset object being loaded (Read Only).</para>
        /// </summary>
        public AssetBundle assetBundle { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

