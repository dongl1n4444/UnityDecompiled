namespace UnityEngine
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Asynchronous load request from an AssetBundle.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public sealed class AssetBundleRequest : AsyncOperation
    {
        /// <summary>
        /// <para>Asset object being loaded (Read Only).</para>
        /// </summary>
        public Object asset { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
        /// <summary>
        /// <para>Asset objects with sub assets being loaded. (Read Only)</para>
        /// </summary>
        public Object[] allAssets { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

