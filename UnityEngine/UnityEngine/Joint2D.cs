using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	/// <summary>
	///   <para>Parent class for joints to connect Rigidbody2D objects.</para>
	/// </summary>
	public class Joint2D : Behaviour
	{
		/// <summary>
		///   <para>The Rigidbody2D object to which the other end of the joint is attached (ie, the object without the joint component).</para>
		/// </summary>
		public extern Rigidbody2D connectedBody
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Should the two rigid bodies connected with this joint collide with each other?</para>
		/// </summary>
		public extern bool enableCollision
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The force that needs to be applied for this joint to break.</para>
		/// </summary>
		public extern float breakForce
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The torque that needs to be applied for this joint to break.</para>
		/// </summary>
		public extern float breakTorque
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Gets the reaction force of the joint.</para>
		/// </summary>
		public Vector2 reactionForce
		{
			get
			{
				return this.GetReactionForce(Time.fixedDeltaTime);
			}
		}

		/// <summary>
		///   <para>Gets the reaction torque of the joint.</para>
		/// </summary>
		public float reactionTorque
		{
			get
			{
				return this.GetReactionTorque(Time.fixedDeltaTime);
			}
		}

		/// <summary>
		///   <para>Can the joint collide with the other Rigidbody2D object to which it is attached?</para>
		/// </summary>
		[Obsolete("Joint2D.collideConnected has been deprecated. Use Joint2D.enableCollision instead (UnityUpgradable) -> enableCollision", true)]
		public bool collideConnected
		{
			get
			{
				return this.enableCollision;
			}
			set
			{
				this.enableCollision = value;
			}
		}

		/// <summary>
		///   <para>Gets the reaction force of the joint given the specified timeStep.</para>
		/// </summary>
		/// <param name="timeStep">The time to calculate the reaction force for.</param>
		/// <returns>
		///   <para>The reaction force of the joint in the specified timeStep.</para>
		/// </returns>
		public Vector2 GetReactionForce(float timeStep)
		{
			Vector2 result;
			Joint2D.Joint2D_CUSTOM_INTERNAL_GetReactionForce(this, timeStep, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Joint2D_CUSTOM_INTERNAL_GetReactionForce(Joint2D joint, float timeStep, out Vector2 value);

		/// <summary>
		///   <para>Gets the reaction torque of the joint given the specified timeStep.</para>
		/// </summary>
		/// <param name="timeStep">The time to calculate the reaction torque for.</param>
		/// <returns>
		///   <para>The reaction torque of the joint in the specified timeStep.</para>
		/// </returns>
		public float GetReactionTorque(float timeStep)
		{
			return Joint2D.INTERNAL_CALL_GetReactionTorque(this, timeStep);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float INTERNAL_CALL_GetReactionTorque(Joint2D self, float timeStep);
	}
}
