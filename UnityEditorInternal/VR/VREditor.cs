namespace UnityEditorInternal.VR
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;

    public sealed class VREditor
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern VRDeviceInfoEditor[] GetAllVRDeviceInfo(BuildTargetGroup targetGroup);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern VRDeviceInfoEditor[] GetAllVRDeviceInfoByTarget(BuildTarget target);
        public static VRDeviceInfoEditor[] GetEnabledVRDeviceInfo(BuildTarget target)
        {
            <GetEnabledVRDeviceInfo>c__AnonStorey1 storey = new <GetEnabledVRDeviceInfo>c__AnonStorey1 {
                enabledVRDevices = GetVREnabledDevicesOnTarget(target)
            };
            return Enumerable.ToArray<VRDeviceInfoEditor>(Enumerable.Where<VRDeviceInfoEditor>(GetAllVRDeviceInfoByTarget(target), new Func<VRDeviceInfoEditor, bool>(storey, (IntPtr) this.<>m__0)));
        }

        public static VRDeviceInfoEditor[] GetEnabledVRDeviceInfo(BuildTargetGroup targetGroup)
        {
            <GetEnabledVRDeviceInfo>c__AnonStorey0 storey = new <GetEnabledVRDeviceInfo>c__AnonStorey0 {
                enabledVRDevices = GetVREnabledDevicesOnTargetGroup(targetGroup)
            };
            return Enumerable.ToArray<VRDeviceInfoEditor>(Enumerable.Where<VRDeviceInfoEditor>(GetAllVRDeviceInfo(targetGroup), new Func<VRDeviceInfoEditor, bool>(storey, (IntPtr) this.<>m__0)));
        }

        [Obsolete("Use GetVREnabledOnTargetGroup instead.")]
        public static bool GetVREnabled(BuildTargetGroup targetGroup)
        {
            return GetVREnabledOnTargetGroup(targetGroup);
        }

        [Obsolete("Use GetVREnabledDevicesOnTargetGroup instead.")]
        public static string[] GetVREnabledDevices(BuildTargetGroup targetGroup)
        {
            return GetVREnabledDevicesOnTargetGroup(targetGroup);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string[] GetVREnabledDevicesOnTarget(BuildTarget target);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string[] GetVREnabledDevicesOnTargetGroup(BuildTargetGroup targetGroup);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool GetVREnabledOnTargetGroup(BuildTargetGroup targetGroup);
        [Obsolete("UseSetVREnabledOnTargetGroup instead.")]
        public static void SetVREnabled(BuildTargetGroup targetGroup, bool value)
        {
            SetVREnabledOnTargetGroup(targetGroup, value);
        }

        [Obsolete("Use SetVREnabledDevicesOnTargetGroup instead.")]
        public static void SetVREnabledDevices(BuildTargetGroup targetGroup, string[] devices)
        {
            SetVREnabledDevicesOnTargetGroup(targetGroup, devices);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetVREnabledDevicesOnTargetGroup(BuildTargetGroup targetGroup, string[] devices);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetVREnabledOnTargetGroup(BuildTargetGroup targetGroup, bool value);

        [CompilerGenerated]
        private sealed class <GetEnabledVRDeviceInfo>c__AnonStorey0
        {
            internal string[] enabledVRDevices;

            internal bool <>m__0(VRDeviceInfoEditor d)
            {
                return Enumerable.Contains<string>(this.enabledVRDevices, d.deviceNameKey);
            }
        }

        [CompilerGenerated]
        private sealed class <GetEnabledVRDeviceInfo>c__AnonStorey1
        {
            internal string[] enabledVRDevices;

            internal bool <>m__0(VRDeviceInfoEditor d)
            {
                return Enumerable.Contains<string>(this.enabledVRDevices, d.deviceNameKey);
            }
        }
    }
}

