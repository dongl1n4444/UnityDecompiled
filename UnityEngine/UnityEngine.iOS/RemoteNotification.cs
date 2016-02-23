using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.iOS
{
	/// <summary>
	///   <para>RemoteNotification is only available on iPhoneiPadiPod Touch.</para>
	/// </summary>
	[RequiredByNativeCode]
	public sealed class RemoteNotification
	{
		private IntPtr notificationWrapper;

		/// <summary>
		///   <para>The message displayed in the notification alert. (Read Only)</para>
		/// </summary>
		public extern string alertBody
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>A boolean value that controls whether the alert action is visible or not. (Read Only)</para>
		/// </summary>
		public extern bool hasAction
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The number to display as the application's icon badge. (Read Only)</para>
		/// </summary>
		public extern int applicationIconBadgeNumber
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The name of the sound file to play when an alert is displayed. (Read Only)</para>
		/// </summary>
		public extern string soundName
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>A dictionary for passing custom information to the notified application. (Read Only)</para>
		/// </summary>
		public extern IDictionary userInfo
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		private RemoteNotification()
		{
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Destroy();

		~RemoteNotification()
		{
			this.Destroy();
		}
	}
}
