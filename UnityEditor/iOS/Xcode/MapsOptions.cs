namespace UnityEditor.iOS.Xcode
{
    using System;

    [Serializable, Flags]
    public enum MapsOptions
    {
        Airplane = 1,
        Bike = 2,
        Bus = 4,
        Car = 8,
        Ferry = 0x10,
        None = 0,
        Other = 0x800,
        Pedestrian = 0x20,
        RideSharing = 0x40,
        StreetCar = 0x80,
        Subway = 0x100,
        Taxi = 0x200,
        Train = 0x400
    }
}

