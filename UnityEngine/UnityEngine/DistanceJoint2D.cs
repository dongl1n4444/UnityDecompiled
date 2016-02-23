using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	/// <summary>
	///   <para>Joint that keeps two Rigidbody2D objects a fixed distance apart.</para>
	/// </summary>
	public sealed class DistanceJoint2D : AnchoredJoint2D
	{
		/// <summary>
		///   <para>Should the distance be calculated automatically?</para>
		/// </summary>
		public extern bool autoConfigureDistance
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The distance separating the two ends of the joint.</para>
		/// </summary>
		public extern float distance
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Whether to maintain a maximum distance only or not.  If not then the absolute distance will be maintained instead.</para>
		/// </summary>
		public extern bool maxDistanceOnly
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
