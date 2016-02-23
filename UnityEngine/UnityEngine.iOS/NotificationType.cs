using System;

namespace UnityEngine.iOS
{
	/// <summary>
	///   <para>Specifies local and remote notification types.</para>
	/// </summary>
	public enum NotificationType
	{
		/// <summary>
		///   <para>No notification types specified.</para>
		/// </summary>
		None,
		/// <summary>
		///   <para>Notification is a badge shown above the application's icon.</para>
		/// </summary>
		Badge,
		/// <summary>
		///   <para>Notification is an alert sound.</para>
		/// </summary>
		Sound,
		/// <summary>
		///   <para>Notification is an alert message.</para>
		/// </summary>
		Alert = 4
	}
}
