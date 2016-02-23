using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	/// <summary>
	///   <para>The joint attempts to move a Rigidbody2D to a specific target position.</para>
	/// </summary>
	public sealed class TargetJoint2D : Joint2D
	{
		/// <summary>
		///   <para>The local-space anchor on the rigid-body the joint is attached to.</para>
		/// </summary>
		public Vector2 anchor
		{
			get
			{
				Vector2 result;
				this.INTERNAL_get_anchor(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_anchor(ref value);
			}
		}

		/// <summary>
		///   <para>The world-space position that the joint will attempt to move the body to.</para>
		/// </summary>
		public Vector2 target
		{
			get
			{
				Vector2 result;
				this.INTERNAL_get_target(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_target(ref value);
			}
		}

		/// <summary>
		///   <para>Should the target be calculated automatically?</para>
		/// </summary>
		public extern bool autoConfigureTarget
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The maximum force that can be generated when trying to maintain the target joint constraint.</para>
		/// </summary>
		public extern float maxForce
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The amount by which the target spring force is reduced in proportion to the movement speed.</para>
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
		///   <para>The frequency at which the target spring oscillates around the target position.</para>
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

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_anchor(out Vector2 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_anchor(ref Vector2 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_target(out Vector2 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_target(ref Vector2 value);
	}
}
