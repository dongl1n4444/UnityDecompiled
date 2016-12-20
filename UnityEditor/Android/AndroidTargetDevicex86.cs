namespace UnityEditor.Android
{
    using System;
    using UnityEditor;

    public class AndroidTargetDevicex86 : AndroidTargetDeviceType
    {
        public override string ABI
        {
            get
            {
                return "x86";
            }
        }

        public override string Architecture
        {
            get
            {
                return "x86";
            }
        }

        public override AndroidTargetDevice TargetDevice
        {
            get
            {
                return AndroidTargetDevice.x86;
            }
        }
    }
}

