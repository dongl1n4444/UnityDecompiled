namespace UnityEditor.Android
{
    using System;
    using UnityEditor;

    public class AndroidTargetDeviceARMv7 : AndroidTargetDeviceType
    {
        public override string ABI
        {
            get
            {
                return "armeabi-v7a";
            }
        }

        public override string Architecture
        {
            get
            {
                return "ARMv7";
            }
        }

        public override AndroidTargetDevice TargetDevice
        {
            get
            {
                return AndroidTargetDevice.ARMv7;
            }
        }
    }
}

