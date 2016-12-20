namespace UnityEditor.Web
{
    using System;
    using UnityEditor;
    using UnityEditor.Collaboration;
    using UnityEditor.Connect;

    [InitializeOnLoad]
    internal class CollabAccess : CloudServiceAccess
    {
        private const string kServiceDisplayName = "Unity Collab";
        private const string kServiceName = "Collab";
        private const string kServiceUrl = "https://public-cdn.cloud.unity3d.com/editor/production/cloud/collab";
        private static CollabAccess s_instance = new CollabAccess();

        static CollabAccess()
        {
            UnityConnectServiceData cloudService = new UnityConnectServiceData("Collab", "https://public-cdn.cloud.unity3d.com/editor/production/cloud/collab", s_instance, "unity/project/cloud/collab");
            UnityConnectServiceCollection.instance.AddService(cloudService);
        }

        public override void EnableService(bool enabled)
        {
            if (!Collab.instance.collabInfo.whitelisted)
            {
                base.EnableService(false);
                Collab.instance.SendNotification();
            }
            else
            {
                base.EnableService(enabled);
                Collab.instance.SendNotification();
                Collab.instance.SetCollabEnabledForCurrentProject(enabled);
            }
        }

        public override string GetServiceDisplayName()
        {
            return "Unity Collab";
        }

        public override string GetServiceName()
        {
            return "Collab";
        }

        public bool IsCollabUIAccessible()
        {
            return (UnityConnect.instance.userInfo.whitelisted && Collab.instance.collabInfo.whitelisted);
        }

        public static CollabAccess Instance
        {
            get
            {
                return s_instance;
            }
        }
    }
}

