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
        public string error { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        /// <summary>
        /// <para>Sets the priority for request.</para>
        /// </summary>
        public float loadingPriority { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>Gets file system's path to the resource available in On Demand Resources (ODR) request.</para>
        /// </summary>
        /// <param name="resourceName">Resource name.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern string GetResourcePath(string resourceName);
        /// <summary>
        /// <para>Release all resources kept alive by On Demand Resources (ODR) request.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public extern void Dispose();
        ~OnDemandResourcesRequest()
        {
            this.Dispose();
        }
    }
}

