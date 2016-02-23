using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	/// <summary>
	///   <para>Keeps two Rigidbody2D at their relative orientations.</para>
	/// </summary>
	public sealed class RelativeJoint2D : Joint2D
	{
		/// <summary>
		///   <para>The maximum force that can be generated when trying to maintain the relative joint constraint.</para>
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
		///   <para>The maximum torque that can be generated when trying to maintain the relative joint constraint.</para>
		/// </summary>
		public extern float maxTorque
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Scales both the linear and angular forces used to correct the required relative orientation.</para>
		/// </summary>
		public extern float correctionScale
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Should both the linearOffset and angularOffset be calculated automatically?</para>
		/// </summary>
		public extern bool autoConfigureOffset
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The current linear offset between the Rigidbody2D that the joint connects.</para>
		/// </summary>
		public Vector2 linearOffset
		{
			get
			{
				Vector2 result;
				this.INTERNAL_get_linearOffset(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_linearOffset(ref value);
			}
		}

		/// <summary>
		///   <para>The current angular offset between the Rigidbody2D that the joint connects.</para>
		/// </summary>
		public extern float angularOffset
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The world-space position that is currently trying to be maintained.</para>
		/// </summary>
		public Vector2 target
		{
			get
			{
				Vector2 result;
				this.INTERNAL_get_target(out result);
				return result;
			}
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_linearOffset(out Vector2 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_linearOffset(ref Vector2 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_target(out Vector2 value);
	}
}
