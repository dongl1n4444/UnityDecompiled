using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.iOS
{
	/// <summary>
	///   <para>Represents a request for On Demand Resources (ODR). It's an AsyncOperation and can be yielded in a coroutine.</para>
	/// </summary>
	[UsedByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class OnDemandResourcesRequest : AsyncOperation, IDisposable
	{
		/// <summary>
		///   <para>Returns an error after operation is complete.</para>
		/// </summary>
		public extern string error
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Sets the priority for request.</para>
		/// </summary>
		public extern float loadingPriority
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal OnDemandResourcesRequest()
		{
		}

		/// <summary>
		///   <para>Gets file system's path to the resource available in On Demand Resources (ODR) request.</para>
		/// </summary>
		/// <param name="resourceName">Resource name.</param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetResourcePath(string resourceName);

		/// <summary>
		///   <para>Release all resources kept alive by On Demand Resources (ODR) request.</para>
		/// </summary>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Dispose();

		~OnDemandResourcesRequest()
		{
			this.Dispose();
		}
	}
}
