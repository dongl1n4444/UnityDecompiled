namespace UnityEditor.Android
{
    using System;
    using UnityEditor;

    public abstract class AndroidTargetDeviceType
    {
        protected AndroidTargetDeviceType()
        {
        }

        public abstract string ABI { get; }

        public abstract string Architecture { get; }

        public abstract AndroidTargetDevice TargetDevice { get; }
    }
}

