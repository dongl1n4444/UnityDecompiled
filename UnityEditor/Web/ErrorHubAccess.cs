namespace UnityEditor.Web
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Connect;

    [InitializeOnLoad]
    internal class ErrorHubAccess : CloudServiceAccess
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <errorMessage>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static ErrorHubAccess <instance>k__BackingField;
        public const string kServiceName = "ErrorHub";
        private static string kServiceUrl = ("file://" + EditorApplication.userJavascriptPackagesPath + "unityeditor-cloud-hub/dist/index.html?failure=unity_connect");

        static ErrorHubAccess()
        {
            instance = new ErrorHubAccess();
            UnityConnectServiceData cloudService = new UnityConnectServiceData("ErrorHub", kServiceUrl, instance, "unity/project/cloud/errorhub");
            UnityConnectServiceCollection.instance.AddService(cloudService);
        }

        public override string GetServiceName() => 
            "ErrorHub";

        public string errorMessage { get; set; }

        public static ErrorHubAccess instance
        {
            [CompilerGenerated]
            get => 
                <instance>k__BackingField;
            [CompilerGenerated]
            private set
            {
                <instance>k__BackingField = value;
            }
        }
    }
}

