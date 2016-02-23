using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.VR
{
	/// <summary>
	///   <para>VR Input tracking data.</para>
	/// </summary>
	public sealed class InputTracking
	{
		/// <summary>
		///   <para>The current position of the requested VRNode.</para>
		/// </summary>
		/// <param name="node">Node index.</param>
		/// <returns>
		///   <para>Position of node local to its tracking space.</para>
		/// </returns>
		public static Vector3 GetLocalPosition(VRNode node)
		{
			Vector3 result;
			InputTracking.INTERNAL_CALL_GetLocalPosition(node, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetLocalPosition(VRNode node, out Vector3 value);

		/// <summary>
		///   <para>The current rotation of the requested VRNode.</para>
		/// </summary>
		/// <param name="node">Node index.</param>
		/// <returns>
		///   <para>Rotation of node local to its tracking space.</para>
		/// </returns>
		public static Quaternion GetLocalRotation(VRNode node)
		{
			Quaternion result;
			InputTracking.INTERNAL_CALL_GetLocalRotation(node, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetLocalRotation(VRNode node, out Quaternion value);

		/// <summary>
		///   <para>Center tracking on the current pose.</para>
		/// </summary>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Recenter();
	}
}
