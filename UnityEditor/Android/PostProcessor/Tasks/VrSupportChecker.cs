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
        private const int kCardboardTargetSdkVersion = 0x15;
        private const int kDaydreamMinSdkVersion = 0x18;
        private const int kDaydreamTargetSdkVersion = 0x18;
        private const int kOculusGearVRMinSdkVersion = 0x13;
        private const int kOculusGearVRTargetSdkVersion = 0x13;
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

        private void CheckAllMinimumSdkVersions(PostProcessorContext context)
        {
            if (this.isCardboardEnabled)
            {
                this.CheckMinimumSdkVersion(context, "Cardboard", 0x13);
            }
            if (this.isDaydreamOnly)
            {
                this.CheckMinimumSdkVersion(context, "Daydream", 0x18);
            }
            if (this.isOculusEnabled)
            {
                this.CheckMinimumSdkVersion(context, "Oculus", 0x13);
            }
        }

        private void CheckAllTargetSdkVersions(PostProcessorContext context)
        {
            if (this.isCardboardEnabled)
            {
                this.CheckTargetSdkVersion(context, "Cardboard", 0x15);
            }
            if (this.isDaydreamEnabled)
            {
                this.CheckTargetSdkVersion(context, "Daydream", 0x18);
            }
            if (this.isOculusEnabled)
            {
                this.CheckTargetSdkVersion(context, "Oculus", 0x13);
            }
        }

        private void CheckMinimumSdkVersion(PostProcessorContext context, string vrName, int minSdkRequired)
        {
            int num = context.Get<int>("MinSDKVersion");
            if (num < minSdkRequired)
            {
                string message = $"{vrName} Requires a Minimum API Level of {minSdkRequired}.
You have selected {num}";
                CancelPostProcess.AbortBuild("Minimum API Level Not Supported on Requested VR Device", message, null);
            }
        }

        private void CheckTargetSdkVersion(PostProcessorContext context, string vrName, int targetSdkRequired)
        {
            int num = context.Get<int>("TargetSDKVersion");
            if (num < targetSdkRequired)
            {
                string message = $"{vrName} Requires a Target API Level of {targetSdkRequired}.
You have selected {num}";
                CancelPostProcess.AbortBuild("Target API Level Not Supported on Requested VR Device", message, null);
            }
        }

        public static void CheckVrSdkVersions(PostProcessorContext context)
        {
            VrSupportChecker checker = new VrSupportChecker();
            checker.CheckAllMinimumSdkVersions(context);
            checker.CheckAllTargetSdkVersions(context);
        }

        public bool isCardboardEnabled =>
            this.m_IsCardboardEnabled;

        public bool isDaydreamEnabled =>
            this.m_IsDaydreamEnabled;

        public bool isDaydreamOnly =>
            ((this.m_IsDaydreamEnabled && !this.m_IsCardboardEnabled) && !this.m_IsOculusEnabled);

        public bool isDaydreamPrimary =>
            this.m_IsDaydreamPrimary;

        public bool isOculusEnabled =>
            this.m_IsOculusEnabled;
    }
}

