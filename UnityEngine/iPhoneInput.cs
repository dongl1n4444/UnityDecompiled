namespace UnityEngine
{
    using System;

    [Obsolete("iPhoneInput class is deprecated. Please use Input instead (UnityUpgradable) -> Input", true)]
    public class iPhoneInput
    {
        public static iPhoneAccelerationEvent GetAccelerationEvent(int index) => 
            new iPhoneAccelerationEvent();

        public static iPhoneTouch GetTouch(int index) => 
            new iPhoneTouch();

        public static Vector3 acceleration =>
            new Vector3();

        public static int accelerationEventCount =>
            0;

        public static iPhoneAccelerationEvent[] accelerationEvents =>
            null;

        [Obsolete("lastLocation property is deprecated. Please use Input.location.lastData instead.", true)]
        public static LocationInfo lastLocation =>
            new LocationInfo();

        public static bool multiTouchEnabled
        {
            get => 
                false;
            set
            {
            }
        }

        [Obsolete("orientation property is deprecated. Please use Input.deviceOrientation instead (UnityUpgradable) -> Input.deviceOrientation", true)]
        public static iPhoneOrientation orientation =>
            iPhoneOrientation.Unknown;

        public static int touchCount =>
            0;

        public static iPhoneTouch[] touches =>
            null;
    }
}

