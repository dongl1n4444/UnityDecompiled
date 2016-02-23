using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>How particles are aligned when rendered.</para>
	/// </summary>
	public enum ParticleSystemRenderSpace
	{
		/// <summary>
		///   <para>Particles face the camera.</para>
		/// </summary>
		View,
		/// <summary>
		///   <para>Particles align with the world.</para>
		/// </summary>
		World,
		/// <summary>
		///   <para>Particles align with their local transform.</para>
		/// </summary>
		Local
	}
}
