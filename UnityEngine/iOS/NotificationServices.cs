﻿namespace UnityEngine.iOS
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>NotificationServices is only available on iPhoneiPadiPod Touch.</para>
    /// </summary>
    public sealed class NotificationServices
    {
        /// <summary>
        /// <para>Cancels the delivery of all scheduled local notifications.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CancelAllLocalNotifications();
        /// <summary>
        /// <para>Cancels the delivery of the specified scheduled local notification.</para>
        /// </summary>
        /// <param name="notification"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CancelLocalNotification(LocalNotification notification);
        /// <summary>
        /// <para>Discards of all received local notifications.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void ClearLocalNotifications();
        /// <summary>
        /// <para>Discards of all received remote notifications.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void ClearRemoteNotifications();
        /// <summary>
        /// <para>Returns an object representing a specific local notification. (Read Only)</para>
        /// </summary>
        /// <param name="index"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern LocalNotification GetLocalNotification(int index);
        /// <summary>
        /// <para>Returns an object representing a specific remote notification. (Read Only)</para>
        /// </summary>
        /// <param name="index"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern RemoteNotification GetRemoteNotification(int index);
        /// <summary>
        /// <para>Presents a local notification immediately.</para>
        /// </summary>
        /// <param name="notification"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void PresentLocalNotificationNow(LocalNotification notification);
        /// <summary>
        /// <para>Register to receive local and remote notifications of the specified types from a provider via Apple Push Service.</para>
        /// </summary>
        /// <param name="notificationTypes">Notification types to register for.</param>
        /// <param name="registerForRemote">Specify true to also register for remote notifications.</param>
        public static void RegisterForNotifications(NotificationType notificationTypes)
        {
            RegisterForNotifications(notificationTypes, true);
        }

        /// <summary>
        /// <para>Register to receive local and remote notifications of the specified types from a provider via Apple Push Service.</para>
        /// </summary>
        /// <param name="notificationTypes">Notification types to register for.</param>
        /// <param name="registerForRemote">Specify true to also register for remote notifications.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void RegisterForNotifications(NotificationType notificationTypes, bool registerForRemote);
        /// <summary>
        /// <para>Schedules a local notification.</para>
        /// </summary>
        /// <param name="notification"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void ScheduleLocalNotification(LocalNotification notification);
        /// <summary>
        /// <para>Unregister for remote notifications.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void UnregisterForRemoteNotifications();

        /// <summary>
        /// <para>Device token received from Apple Push Service after calling NotificationServices.RegisterForRemoteNotificationTypes. (Read Only)</para>
        /// </summary>
        public static byte[] deviceToken { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Enabled local and remote notification types.</para>
        /// </summary>
        public static NotificationType enabledNotificationTypes { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The number of received local notifications. (Read Only)</para>
        /// </summary>
        public static int localNotificationCount { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The list of objects representing received local notifications. (Read Only)</para>
        /// </summary>
        public static LocalNotification[] localNotifications
        {
            get
            {
                int localNotificationCount = NotificationServices.localNotificationCount;
                LocalNotification[] notificationArray = new LocalNotification[localNotificationCount];
                for (int i = 0; i < localNotificationCount; i++)
                {
                    notificationArray[i] = GetLocalNotification(i);
                }
                return notificationArray;
            }
        }

        /// <summary>
        /// <para>Returns an error that might occur on registration for remote notifications via NotificationServices.RegisterForRemoteNotificationTypes. (Read Only)</para>
        /// </summary>
        public static string registrationError { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The number of received remote notifications. (Read Only)</para>
        /// </summary>
        public static int remoteNotificationCount { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The list of objects representing received remote notifications. (Read Only)</para>
        /// </summary>
        public static RemoteNotification[] remoteNotifications
        {
            get
            {
                int remoteNotificationCount = NotificationServices.remoteNotificationCount;
                RemoteNotification[] notificationArray = new RemoteNotification[remoteNotificationCount];
                for (int i = 0; i < remoteNotificationCount; i++)
                {
                    notificationArray[i] = GetRemoteNotification(i);
                }
                return notificationArray;
            }
        }

        /// <summary>
        /// <para>All currently scheduled local notifications.</para>
        /// </summary>
        public static LocalNotification[] scheduledLocalNotifications { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

