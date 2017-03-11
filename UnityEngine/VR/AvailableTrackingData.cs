namespace UnityEngine.VR
{
    using System;

    [Flags]
    internal enum AvailableTrackingData
    {
        AccelerationAvailable = 0x10,
        AngularAccelerationAvailable = 0x20,
        AngularVelocityAvailable = 8,
        None = 0,
        PositionAvailable = 1,
        RotationAvailable = 2,
        VelocityAvailable = 4
    }
}

