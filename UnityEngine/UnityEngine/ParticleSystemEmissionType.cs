using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>The mode in which particles are emitted.</para>
	/// </summary>
	public enum ParticleSystemEmissionType
	{
		/// <summary>
		///   <para>Emit over time.</para>
		/// </summary>
		Time,
		/// <summary>
		///   <para>Emit when emitter moves.</para>
		/// </summary>
		Distance
	}
}
