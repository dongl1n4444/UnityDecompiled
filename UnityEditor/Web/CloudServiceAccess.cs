namespace UnityEditor.Web
{
    using System;
    using UnityEditor;
    using UnityEditor.Connect;

    internal abstract class CloudServiceAccess
    {
        public virtual void EnableService(bool enabled)
        {
            PlayerSettings.SetCloudServiceEnabled(this.GetServiceName(), enabled);
        }

        protected string GetSafeServiceName()
        {
            return this.GetServiceName().Replace(' ', '_');
        }

        public virtual string GetServiceDisplayName()
        {
            return this.GetServiceName();
        }

        public abstract string GetServiceName();
        protected WebView GetWebView()
        {
            return UnityConnectServiceCollection.instance.GetWebViewFromServiceName(this.GetServiceName());
        }

        public void GoBackToHub()
        {
            UnityConnectServiceCollection.instance.ShowService("Hub", true);
        }

        public virtual bool IsServiceEnabled()
        {
            return PlayerSettings.GetCloudServiceEnabled(this.GetServiceName());
        }

        public virtual void OnProjectUnbound()
        {
        }

        public void ShowServicePage()
        {
            UnityConnectServiceCollection.instance.ShowService(this.GetServiceName(), true);
        }
    }
}

