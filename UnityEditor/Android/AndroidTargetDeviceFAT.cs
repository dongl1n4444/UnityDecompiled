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
                throw new NotSupportedException("Getting ABI is not supported for FAT target device.");
            }
        }

        public override string Architecture =>
            "FAT";

        public override string GradleProductFlavor
        {
            get
            {
                throw new NotSupportedException("Getting Gradle product flavor is not supported for FAT target device.");
            }
        }

        public override AndroidTargetDevice TargetDevice =>
            AndroidTargetDevice.FAT;

        public override string VisualStudioPlatform
        {
            get
            {
                throw new NotSupportedException("Getting Visual Studio platform is not supported for FAT target device.");
            }
        }
    }
}

