namespace UnityEditor.Android
{
    using System;
    using UnityEditor;

    public class AndroidTargetDeviceFAT : AndroidTargetDeviceType
    {
        public override string ABI
        {
            get
            {
                throw new NotSupportedException("Getting ABI is not supported for FAT target device");
            }
        }

        public override string Architecture
        {
            get
            {
                return "FAT";
            }
        }

        public override AndroidTargetDevice TargetDevice
        {
            get
            {
                return AndroidTargetDevice.FAT;
            }
        }
    }
}

