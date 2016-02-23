using System;

namespace UnityEngine.iOS
{
	/// <summary>
	///   <para>Specify calendrical units.</para>
	/// </summary>
	public enum CalendarUnit
	{
		/// <summary>
		///   <para>Specifies the era unit.</para>
		/// </summary>
		Era = 2,
		/// <summary>
		///   <para>Specifies the year unit.</para>
		/// </summary>
		Year = 4,
		/// <summary>
		///   <para>Specifies the month unit.</para>
		/// </summary>
		Month = 8,
		/// <summary>
		///   <para>Specifies the day unit.</para>
		/// </summary>
		Day = 16,
		/// <summary>
		///   <para>Specifies the hour unit.</para>
		/// </summary>
		Hour = 32,
		/// <summary>
		///   <para>Specifies the minute unit.</para>
		/// </summary>
		Minute = 64,
		/// <summary>
		///   <para>Specifies the second unit.</para>
		/// </summary>
		Second = 128,
		/// <summary>
		///   <para>Specifies the week unit.</para>
		/// </summary>
		Week = 256,
		/// <summary>
		///   <para>Specifies the weekday unit.</para>
		/// </summary>
		Weekday = 512,
		/// <summary>
		///   <para>Specifies the ordinal weekday unit.</para>
		/// </summary>
		WeekdayOrdinal = 1024,
		/// <summary>
		///   <para>Specifies the quarter of the calendar.</para>
		/// </summary>
		Quarter = 2048
	}
}
