namespace UnityEditor.TestTools.TestRunner
{
    using System;
    using UnityEditor;

    internal class ApplePlatformSetup : IPlatformSetup
    {
        private readonly BuildTarget m_BuildTarget;

        public ApplePlatformSetup(BuildTarget buildTarget)
        {
            this.m_BuildTarget = buildTarget;
        }

        public void CleanUp()
        {
            IDeviceUtils.StopPlayerConnectionSupport(this.GetDeviceId());
        }

        private string GetDeviceId()
        {
            string environmentVariable = Environment.GetEnvironmentVariable((this.m_BuildTarget != BuildTarget.iOS) ? "TVOS_DEVICE_ID" : "IOS_DEVICE_ID");
            if (string.IsNullOrEmpty(environmentVariable))
            {
                throw new ArgumentException("Need to set IOS_DEVICE_ID or TVOS_DEVICE_ID env variable");
            }
            return environmentVariable;
        }

        public void Setup()
        {
            IDeviceUtils.StartPlayerConnectionSupport(this.GetDeviceId());
        }
    }
}

