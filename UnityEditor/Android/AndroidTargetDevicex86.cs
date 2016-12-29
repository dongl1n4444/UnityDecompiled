namespace UnityEditor.Android
{
    using System;
    using UnityEditor;

    public class AndroidTargetDevicex86 : AndroidTargetDeviceType
    {
        public override string ABI =>
            "x86";

        public override string Architecture =>
            "x86";

        public override AndroidTargetDevice TargetDevice =>
            AndroidTargetDevice.x86;
    }
}

