using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>How to apply emitter velocity to particles.</para>
	/// </summary>
	public enum ParticleSystemInheritVelocityMode
	{
		/// <summary>
		///   <para>Apply the emitter velocity at the time the particle was born, on every frame during its lifetime.</para>
		/// </summary>
		Initial,
		/// <summary>
		///   <para>Apply the current emitter velocity to a particle, on every frame during its lifetime.</para>
		/// </summary>
		Current
	}
}
