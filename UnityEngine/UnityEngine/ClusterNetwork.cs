using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	/// <summary>
	///   <para>A helper class that contains static method to inquire status of Unity Cluster.</para>
	/// </summary>
	public sealed class ClusterNetwork
	{
		/// <summary>
		///   <para>Check whether the current instance is a master node in the cluster network.</para>
		/// </summary>
		public static extern bool isMasterOfCluster
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Check whether the current instance is disconnected from the cluster network.</para>
		/// </summary>
		public static extern bool isDisconnected
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>To acquire or set the node index of the current machine from the cluster network.</para>
		/// </summary>
		public static extern int nodeIndex
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
