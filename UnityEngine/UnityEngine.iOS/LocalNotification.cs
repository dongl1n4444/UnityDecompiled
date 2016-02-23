using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.iOS
{
	/// <summary>
	///   <para>iOS.LocalNotification is a wrapper around the UILocalNotification class found in the Apple UIKit framework and is only available on iPhoneiPadiPod Touch.</para>
	/// </summary>
	[RequiredByNativeCode]
	public sealed class LocalNotification
	{
		private IntPtr notificationWrapper;

		private static long m_NSReferenceDateTicks;

		/// <summary>
		///   <para>The date and time when the system should deliver the notification.</para>
		/// </summary>
		public DateTime fireDate
		{
			get
			{
				return new DateTime((long)(this.GetFireDate() * 10000000.0) + LocalNotification.m_NSReferenceDateTicks);
			}
			set
			{
				this.SetFireDate((double)(value.ToUniversalTime().Ticks - LocalNotification.m_NSReferenceDateTicks) / 10000000.0);
			}
		}

		/// <summary>
		///   <para>The time zone of the notification's fire date.</para>
		/// </summary>
		public extern string timeZone
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The calendar interval at which to reschedule the notification.</para>
		/// </summary>
		public extern CalendarUnit repeatInterval
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The calendar type (Gregorian, Chinese, etc) to use for rescheduling the notification.</para>
		/// </summary>
		public extern CalendarIdentifier repeatCalendar
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The message displayed in the notification alert.</para>
		/// </summary>
		public extern string alertBody
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The title of the action button or slider.</para>
		/// </summary>
		public extern string alertAction
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>A boolean value that controls whether the alert action is visible or not.</para>
		/// </summary>
		public extern bool hasAction
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Identifies the image used as the launch image when the user taps the action button.</para>
		/// </summary>
		public extern string alertLaunchImage
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The number to display as the application's icon badge.</para>
		/// </summary>
		public extern int applicationIconBadgeNumber
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The name of the sound file to play when an alert is displayed.</para>
		/// </summary>
		public extern string soundName
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The default system sound. (Read Only)</para>
		/// </summary>
		public static extern string defaultSoundName
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>A dictionary for passing custom information to the notified application.</para>
		/// </summary>
		public extern IDictionary userInfo
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Creates a new local notification.</para>
		/// </summary>
		public LocalNotification()
		{
			this.InitWrapper();
		}

		static LocalNotification()
		{
			// Note: this type is marked as 'beforefieldinit'.
			DateTime dateTime = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			LocalNotification.m_NSReferenceDateTicks = dateTime.Ticks;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern double GetFireDate();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetFireDate(double dt);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Destroy();

		~LocalNotification()
		{
			this.Destroy();
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InitWrapper();
	}
}
