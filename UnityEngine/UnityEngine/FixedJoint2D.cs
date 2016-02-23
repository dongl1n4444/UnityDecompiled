using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	/// <summary>
	///   <para>Connects two Rigidbody2D together at their anchor points using a configurable spring.</para>
	/// </summary>
	public sealed class FixedJoint2D : AnchoredJoint2D
	{
		/// <summary>
		///   <para>The amount by which the spring force is reduced in proportion to the movement speed.</para>
		/// </summary>
		public extern float dampingRatio
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The frequency at which the spring oscillates around the distance between the objects.</para>
		/// </summary>
		public extern float frequency
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The angle referenced between the two bodies used as the constraint for the joint.</para>
		/// </summary>
		public extern float referenceAngle
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
