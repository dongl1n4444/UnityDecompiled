namespace UnityEngine.iOS
{
    using System;

    /// <summary>
    /// <para>Specify calendrical units.</para>
    /// </summary>
    public enum CalendarUnit
    {
        /// <summary>
        /// <para>Specifies the day unit.</para>
        /// </summary>
        Day = 0x10,
        /// <summary>
        /// <para>Specifies the era unit.</para>
        /// </summary>
        Era = 2,
        /// <summary>
        /// <para>Specifies the hour unit.</para>
        /// </summary>
        Hour = 0x20,
        /// <summary>
        /// <para>Specifies the minute unit.</para>
        /// </summary>
        Minute = 0x40,
        /// <summary>
        /// <para>Specifies the month unit.</para>
        /// </summary>
        Month = 8,
        /// <summary>
        /// <para>Specifies the quarter of the calendar.</para>
        /// </summary>
        Quarter = 0x800,
        /// <summary>
        /// <para>Specifies the second unit.</para>
        /// </summary>
        Second = 0x80,
        /// <summary>
        /// <para>Specifies the week unit.</para>
        /// </summary>
        Week = 0x100,
        /// <summary>
        /// <para>Specifies the weekday unit.</para>
        /// </summary>
        Weekday = 0x200,
        /// <summary>
        /// <para>Specifies the ordinal weekday unit.</para>
        /// </summary>
        WeekdayOrdinal = 0x400,
        /// <summary>
        /// <para>Specifies the year unit.</para>
        /// </summary>
        Year = 4
    }
}

