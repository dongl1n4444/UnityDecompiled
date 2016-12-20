namespace UnityEditor.Web
{
    using System;
    using UnityEditor;
    using UnityEditor.Connect;

    [InitializeOnLoad]
    internal class HubAccess : CloudServiceAccess
    {
        private const string kServiceDisplayName = "Services";
        public const string kServiceName = "Hub";
        private const string kServiceUrl = "https://public-cdn.cloud.unity3d.com/editor/production/cloud/hub";
        private static HubAccess s_Instance = new HubAccess();

        static HubAccess()
        {
            UnityConnectServiceData cloudService = new UnityConnectServiceData("Hub", "https://public-cdn.cloud.unity3d.com/editor/production/cloud/hub", s_Instance, "unity/project/cloud/hub");
            UnityConnectServiceCollection.instance.AddService(cloudService);
        }

        public void EnableCloudService(string name, bool enabled)
        {
            UnityConnectServiceCollection.instance.EnableService(name, enabled);
        }

        public override string GetServiceDisplayName()
        {
            return "Services";
        }

        public override string GetServiceName()
        {
            return "Hub";
        }

        public UnityConnectServiceCollection.ServiceInfo[] GetServices()
        {
            return UnityConnectServiceCollection.instance.GetAllServiceInfos();
        }

        [MenuItem("Window/Services %0", false, 0x7cf)]
        private static void ShowMyWindow()
        {
            UnityConnectServiceCollection.instance.ShowService("Hub", true);
        }

        public void ShowService(string name)
        {
            UnityConnectServiceCollection.instance.ShowService(name, true);
        }

        public static HubAccess instance
        {
            get
            {
                return s_Instance;
            }
        }
    }
}

