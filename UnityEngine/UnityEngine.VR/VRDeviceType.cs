using System;

namespace UnityEngine.VR
{
	/// <summary>
	///   <para>Supported VR devices.</para>
	/// </summary>
	public enum VRDeviceType
	{
		/// <summary>
		///   <para>No VR Device.</para>
		/// </summary>
		None,
		/// <summary>
		///   <para>Stereo 3D via D3D11 or OpenGL.</para>
		/// </summary>
		Stereo,
		/// <summary>
		///   <para>Split screen stereo 3D (the left and right cameras are rendered side by side).</para>
		/// </summary>
		Split,
		/// <summary>
		///   <para>Oculus family of VR devices.</para>
		/// </summary>
		Oculus,
		/// <summary>
		///   <para>Sony's Project Morpheus VR device for Playstation 4. (Obsolete please use VRDeviceType.PlayStationVR instead).</para>
		/// </summary>
		[Obsolete("Enum member VRDeviceType.Morpheus has been deprecated. Use VRDeviceType.PlayStationVR instead (UnityUpgradable) -> PlayStationVR", true)]
		Morpheus,
		/// <summary>
		///   <para>Sony's PlayStation VR device for Playstation 4 (formerly called Project Morpheus VR).</para>
		/// </summary>
		PlayStationVR = 4
	}
}
