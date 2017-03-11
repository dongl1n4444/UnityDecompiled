namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.Android.PostProcessor;
    using UnityEditorInternal.VR;

    internal class VrSupportChecker
    {
        private const int kCardboardMinSdkVersion = 0x13;
        private const int kCardboardTargetSdkVersion = 0x18;
        private const int kDaydreamMinSdkVersion = 0x18;
        private const int kDaydreamTargetSdkVersion = 0x18;
        private const int kOculusGearVRMinSdk = 0x13;
        private const int kOculusGearVRTargetSdk = 0x13;
        private bool m_IsCardboardEnabled = false;
        private bool m_IsDaydreamEnabled = false;
        private bool m_IsDaydreamPrimary = false;
        private bool m_IsOculusEnabled = false;

        public VrSupportChecker()
        {
            string[] vREnabledDevicesOnTargetGroup = VREditor.GetVREnabledDevicesOnTargetGroup(BuildTargetGroup.Android);
            this.m_IsOculusEnabled = vREnabledDevicesOnTargetGroup.Contains<string>("Oculus");
            this.m_IsCardboardEnabled = vREnabledDevicesOnTargetGroup.Contains<string>("cardboard");
            this.m_IsDaydreamEnabled = vREnabledDevicesOnTargetGroup.Contains<string>("daydream");
            this.m_IsDaydreamPrimary = this.m_IsDaydreamEnabled && (Array.IndexOf<string>(vREnabledDevicesOnTargetGroup, "daydream") == 0);
        }

        public void CheckAllMinimumSdkVersions()
        {
            if (this.isCardboardEnabled)
            {
                this.CheckMinimumSdkVersion("Cardboard", 0x13);
            }
            if (this.isDaydreamOnly)
            {
                this.CheckMinimumSdkVersion("Daydream", 0x18);
            }
            if (this.isOculusEnabled)
            {
                this.CheckMinimumSdkVersion("Oculus", 0x13);
            }
        }

        private void CheckMinimumSdkVersion(string vrName, int minSdkRequired)
        {
            int minSdkVersion = (int) PlayerSettings.Android.minSdkVersion;
            if (minSdkVersion < minSdkRequired)
            {
                string message = $"{vrName} Requires a Minimum API Level of {minSdkRequired}.
You have selected {minSdkVersion}";
                CancelPostProcess.AbortBuild("Minimum API Level Not Supported on Requested VR Device", message, null);
            }
        }

        public static void CheckVrMinimumSdkVersions()
        {
            new VrSupportChecker().CheckAllMinimumSdkVersions();
        }

        public int GetHighestRequiredSdk()
        {
            int num = 0;
            if (this.isOculusEnabled)
            {
                num = Math.Max(num, Math.Max(0x13, 0x13));
            }
            if (this.isCardboardEnabled)
            {
                num = Math.Max(num, Math.Max(0x13, 0x18));
            }
            if (this.isDaydreamEnabled)
            {
                num = Math.Max(num, Math.Max(0x18, 0x18));
            }
            return num;
        }

        public bool isCardboardEnabled =>
            this.m_IsCardboardEnabled;

        public bool isDaydreamEnabled =>
            this.m_IsDaydreamEnabled;

        public bool isDaydreamOnly =>
            (this.m_IsDaydreamEnabled && !this.m_IsCardboardEnabled);

        public bool isDaydreamPrimary =>
            this.m_IsDaydreamPrimary;

        public bool isOculusEnabled =>
            this.m_IsOculusEnabled;
    }
}

