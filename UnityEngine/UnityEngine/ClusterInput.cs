using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	/// <summary>
	///   <para>Interface for reading and writing inputs in a Unity Cluster.</para>
	/// </summary>
	public sealed class ClusterInput
	{
		/// <summary>
		///   <para>Returns the axis value as a continous float.</para>
		/// </summary>
		/// <param name="name">Name of input to poll.c.</param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetAxis(string name);

		/// <summary>
		///   <para>Returns the binary value of a button.</para>
		/// </summary>
		/// <param name="name">Name of input to poll.</param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetButton(string name);

		/// <summary>
		///   <para>Return the position of a tracker as a Vector3.</para>
		/// </summary>
		/// <param name="name">Name of input to poll.</param>
		public static Vector3 GetTrackerPosition(string name)
		{
			Vector3 result;
			ClusterInput.INTERNAL_CALL_GetTrackerPosition(name, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetTrackerPosition(string name, out Vector3 value);

		/// <summary>
		///   <para>Returns the rotation of a tracker as a Quaternion.</para>
		/// </summary>
		/// <param name="name">Name of input to poll.</param>
		public static Quaternion GetTrackerRotation(string name)
		{
			Quaternion result;
			ClusterInput.INTERNAL_CALL_GetTrackerRotation(name, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetTrackerRotation(string name, out Quaternion value);

		/// <summary>
		///   <para>Sets the axis value for this input. Only works for input typed Custom.</para>
		/// </summary>
		/// <param name="name">Name of input to modify.</param>
		/// <param name="value">Value to set.</param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAxis(string name, float value);

		/// <summary>
		///   <para>Sets the button value for this input. Only works for input typed Custom.</para>
		/// </summary>
		/// <param name="name">Name of input to modify.</param>
		/// <param name="value">Value to set.</param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetButton(string name, bool value);

		/// <summary>
		///   <para>Sets the tracker position for this input. Only works for input typed Custom.</para>
		/// </summary>
		/// <param name="name">Name of input to modify.</param>
		/// <param name="value">Value to set.</param>
		public static void SetTrackerPosition(string name, Vector3 value)
		{
			ClusterInput.INTERNAL_CALL_SetTrackerPosition(name, ref value);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetTrackerPosition(string name, ref Vector3 value);

		/// <summary>
		///   <para>Sets the tracker rotation for this input. Only works for input typed Custom.</para>
		/// </summary>
		/// <param name="name">Name of input to modify.</param>
		/// <param name="value">Value to set.</param>
		public static void SetTrackerRotation(string name, Quaternion value)
		{
			ClusterInput.INTERNAL_CALL_SetTrackerRotation(name, ref value);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetTrackerRotation(string name, ref Quaternion value);

		/// <summary>
		///   <para>Add a new VRPN input entry.</para>
		/// </summary>
		/// <param name="name">Name of the input entry. This has to be unique.</param>
		/// <param name="deviceName">Device name registered to VRPN server.</param>
		/// <param name="serverUrl">URL to the vrpn server.</param>
		/// <param name="index">Index of the Input entry, refer to vrpn.cfg if unsure.</param>
		/// <param name="type">Type of the input.</param>
		/// <returns>
		///   <para>True if the operation succeed.</para>
		/// </returns>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool AddInput(string name, string deviceName, string serverUrl, int index, ClusterInputType type);

		/// <summary>
		///   <para>Edit an input entry which added via ClusterInput.AddInput.</para>
		/// </summary>
		/// <param name="name">Name of the input entry. This has to be unique.</param>
		/// <param name="deviceName">Device name registered to VRPN server.</param>
		/// <param name="serverUrl">URL to the vrpn server.</param>
		/// <param name="index">Index of the Input entry, refer to vrpn.cfg if unsure.</param>
		/// <param name="type">Type of the ClusterInputType as follow.</param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool EditInput(string name, string deviceName, string serverUrl, int index, ClusterInputType type);

		/// <summary>
		///   <para>Check the connection status of the device to the VRPN server it connected to.</para>
		/// </summary>
		/// <param name="name">Name of the input entry.</param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool CheckConnectionToServer(string name);
	}
}
