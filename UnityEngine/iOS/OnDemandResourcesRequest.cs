namespace UnityEngine.iOS
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Represents a request for On Demand Resources (ODR). It's an AsyncOperation and can be yielded in a coroutine.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public sealed class OnDemandResourcesRequest : AsyncOperation, IDisposable
    {
        internal OnDemandResourcesRequest()
        {
        }

        /// <summary>
        /// <para>Returns an error after operation is complete.</para>
        /// </summary>
        public string error { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
        /// <summary>
        /// <para>Sets the priority for request.</para>
        /// </summary>
        public float loadingPriority { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
        /// <summary>
        /// <para>Gets file system's path to the resource available in On Demand Resources (ODR) request.</para>
        /// </summary>
        /// <param name="resourceName">Resource name.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern string GetResourcePath(string resourceName);
        /// <summary>
        /// <para>Release all resources kept alive by On Demand Resources (ODR) request.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        public extern void Dispose();
        ~OnDemandResourcesRequest()
        {
            this.Dispose();
        }
    }
}

