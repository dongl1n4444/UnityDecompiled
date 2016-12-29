namespace UnityEditor.Web
{
    using System;
    using UnityEditor;
    using UnityEditor.Connect;
    using UnityEditor.CrashReporting;

    [InitializeOnLoad]
    internal class CrashReportingAccess : CloudServiceAccess
    {
        private const string kServiceDisplayName = "Game Performance";
        private const string kServiceName = "Game Performance";
        private const string kServiceUrl = "https://public-cdn.cloud.unity3d.com/editor/production/cloud/crash";

        static CrashReportingAccess()
        {
            UnityConnectServiceData cloudService = new UnityConnectServiceData("Game Performance", "https://public-cdn.cloud.unity3d.com/editor/production/cloud/crash", new CrashReportingAccess(), "unity/project/cloud/crashreporting");
            UnityConnectServiceCollection.instance.AddService(cloudService);
        }

        public override void EnableService(bool enabled)
        {
            CrashReportingSettings.enabled = enabled;
        }

        public bool GetCaptureEditorExceptions() => 
            CrashReportingSettings.captureEditorExceptions;

        public override string GetServiceDisplayName() => 
            "Game Performance";

        public override string GetServiceName() => 
            "Game Performance";

        public override bool IsServiceEnabled() => 
            CrashReportingSettings.enabled;

        public void SetCaptureEditorExceptions(bool captureEditorExceptions)
        {
            CrashReportingSettings.captureEditorExceptions = captureEditorExceptions;
        }
    }
}

