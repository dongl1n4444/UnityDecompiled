namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>Enumeration for SystemInfo.deviceType, denotes a coarse grouping of kinds of devices.
    /// 
    /// Windows Store Apps: tablets are treated as desktop machines, thus DeviceType.Handheld will only be returned for Windows Phones.</para>
    /// </summary>
    public enum DeviceType
    {
        Unknown,
        Handheld,
        Console,
        Desktop
    }
}

