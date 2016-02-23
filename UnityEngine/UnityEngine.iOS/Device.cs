using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.iOS
{
	/// <summary>
	///   <para>Interface into iOS specific functionality.</para>
	/// </summary>
	public sealed class Device
	{
		/// <summary>
		///   <para>The generation of the device. (Read Only)</para>
		/// </summary>
		public static extern DeviceGeneration generation
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>iOS version.</para>
		/// </summary>
		public static extern string systemVersion
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Vendor ID.</para>
		/// </summary>
		public static extern string vendorIdentifier
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Advertising ID.</para>
		/// </summary>
		public static string advertisingIdentifier
		{
			get
			{
				string advertisingIdentifier = Device.GetAdvertisingIdentifier();
				Application.InvokeOnAdvertisingIdentifierCallback(advertisingIdentifier, Device.advertisingTrackingEnabled);
				return advertisingIdentifier;
			}
		}

		/// <summary>
		///   <para>Is advertising tracking enabled.</para>
		/// </summary>
		public static extern bool advertisingTrackingEnabled
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Set file flag to be excluded from iCloud/iTunes backup.</para>
		/// </summary>
		/// <param name="path"></param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetNoBackupFlag(string path);

		/// <summary>
		///   <para>Reset "no backup" file flag: file will be synced with iCloud/iTunes backup and can be deleted by OS in low storage situations.</para>
		/// </summary>
		/// <param name="path"></param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ResetNoBackupFlag(string path);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetAdvertisingIdentifier();
	}
}
