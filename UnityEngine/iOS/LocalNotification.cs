namespace UnityEngine.iOS
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>iOS.LocalNotification is a wrapper around the UILocalNotification class found in the Apple UIKit framework and is only available on iPhoneiPadiPod Touch.</para>
    /// </summary>
    [RequiredByNativeCode]
    public sealed class LocalNotification
    {
        private static long m_NSReferenceDateTicks;
        private IntPtr notificationWrapper;

        static LocalNotification()
        {
            DateTime time = new DateTime(0x7d1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            m_NSReferenceDateTicks = time.Ticks;
        }

        /// <summary>
        /// <para>Creates a new local notification.</para>
        /// </summary>
        public LocalNotification()
        {
            this.InitWrapper();
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private extern void Destroy();
        ~LocalNotification()
        {
            this.Destroy();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern double GetFireDate();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void InitWrapper();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetFireDate(double dt);

        /// <summary>
        /// <para>The title of the action button or slider.</para>
        /// </summary>
        public string alertAction { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The message displayed in the notification alert.</para>
        /// </summary>
        public string alertBody { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Identifies the image used as the launch image when the user taps the action button.</para>
        /// </summary>
        public string alertLaunchImage { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The number to display as the application's icon badge.</para>
        /// </summary>
        public int applicationIconBadgeNumber { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The default system sound. (Read Only)</para>
        /// </summary>
        public static string defaultSoundName { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The date and time when the system should deliver the notification.</para>
        /// </summary>
        public DateTime fireDate
        {
            get
            {
                return new DateTime(((long) (this.GetFireDate() * 10000000.0)) + m_NSReferenceDateTicks);
            }
            set
            {
                this.SetFireDate(((double) (value.ToUniversalTime().Ticks - m_NSReferenceDateTicks)) / 10000000.0);
            }
        }

        /// <summary>
        /// <para>A boolean value that controls whether the alert action is visible or not.</para>
        /// </summary>
        public bool hasAction { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The calendar type (Gregorian, Chinese, etc) to use for rescheduling the notification.</para>
        /// </summary>
        public UnityEngine.iOS.CalendarIdentifier repeatCalendar { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The calendar interval at which to reschedule the notification.</para>
        /// </summary>
        public UnityEngine.iOS.CalendarUnit repeatInterval { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The name of the sound file to play when an alert is displayed.</para>
        /// </summary>
        public string soundName { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The time zone of the notification's fire date.</para>
        /// </summary>
        public string timeZone { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>A dictionary for passing custom information to the notified application.</para>
        /// </summary>
        public IDictionary userInfo { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

