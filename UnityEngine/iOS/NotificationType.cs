namespace UnityEngine.iOS
{
    using System;

    /// <summary>
    /// <para>Specifies local and remote notification types.</para>
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// <para>Notification is an alert message.</para>
        /// </summary>
        Alert = 4,
        /// <summary>
        /// <para>Notification is a badge shown above the application's icon.</para>
        /// </summary>
        Badge = 1,
        /// <summary>
        /// <para>No notification types specified.</para>
        /// </summary>
        None = 0,
        /// <summary>
        /// <para>Notification is an alert sound.</para>
        /// </summary>
        Sound = 2
    }
}

