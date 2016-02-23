using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>The emission shape (Shuriken).</para>
	/// </summary>
	public enum ParticleSystemShapeType
	{
		/// <summary>
		///   <para>Emit from the volume of a sphere.</para>
		/// </summary>
		Sphere,
		/// <summary>
		///   <para>Emit from the surface of a sphere.</para>
		/// </summary>
		SphereShell,
		/// <summary>
		///   <para>Emit from the volume of a half-sphere.</para>
		/// </summary>
		Hemisphere,
		/// <summary>
		///   <para>Emit from the surface of a half-sphere.</para>
		/// </summary>
		HemisphereShell,
		/// <summary>
		///   <para>Emit from the base surface of a cone.</para>
		/// </summary>
		Cone,
		/// <summary>
		///   <para>Emit from the volume of a box.</para>
		/// </summary>
		Box,
		/// <summary>
		///   <para>Emit from a mesh.</para>
		/// </summary>
		Mesh,
		/// <summary>
		///   <para>Emit from the base surface of a cone.</para>
		/// </summary>
		ConeShell,
		/// <summary>
		///   <para>Emit from the volume of a cone.</para>
		/// </summary>
		ConeVolume,
		/// <summary>
		///   <para>Emit from the surface of a cone.</para>
		/// </summary>
		ConeVolumeShell,
		/// <summary>
		///   <para>Emit from a circle.</para>
		/// </summary>
		Circle,
		/// <summary>
		///   <para>Emit from the edge of a circle.</para>
		/// </summary>
		CircleEdge,
		/// <summary>
		///   <para>Emit from an edge.</para>
		/// </summary>
		SingleSidedEdge,
		/// <summary>
		///   <para>Emit from a mesh renderer.</para>
		/// </summary>
		MeshRenderer,
		/// <summary>
		///   <para>Emit from a skinned mesh renderer.</para>
		/// </summary>
		SkinnedMeshRenderer
	}
}
