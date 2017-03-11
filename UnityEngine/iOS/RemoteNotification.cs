namespace UnityEngine.iOS
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>RemoteNotification is only available on iPhoneiPadiPod Touch.</para>
    /// </summary>
    [RequiredByNativeCode]
    public sealed class RemoteNotification
    {
        private IntPtr notificationWrapper;

        private RemoteNotification()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        private extern void Destroy();
        ~RemoteNotification()
        {
            this.Destroy();
        }

        /// <summary>
        /// <para>The message displayed in the notification alert. (Read Only)</para>
        /// </summary>
        public string alertBody { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The number to display as the application's icon badge. (Read Only)</para>
        /// </summary>
        public int applicationIconBadgeNumber { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>A boolean value that controls whether the alert action is visible or not. (Read Only)</para>
        /// </summary>
        public bool hasAction { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The name of the sound file to play when an alert is displayed. (Read Only)</para>
        /// </summary>
        public string soundName { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>A dictionary for passing custom information to the notified application. (Read Only)</para>
        /// </summary>
        public IDictionary userInfo { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

