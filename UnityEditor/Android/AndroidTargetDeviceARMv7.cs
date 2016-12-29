namespace UnityEditor.Android
{
    using System;
    using UnityEditor;

    public class AndroidTargetDeviceARMv7 : AndroidTargetDeviceType
    {
        public override string ABI =>
            "armeabi-v7a";

        public override string Architecture =>
            "ARMv7";

        public override AndroidTargetDevice TargetDevice =>
            AndroidTargetDevice.ARMv7;
    }
}

