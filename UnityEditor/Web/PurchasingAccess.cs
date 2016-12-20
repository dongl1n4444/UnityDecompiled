namespace UnityEditor.Web
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Net;
    using System.Net.Security;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography.X509Certificates;
    using UnityEditor;
    using UnityEditor.Connect;
    using UnityEditor.Purchasing;
    using UnityEngine;

    [InitializeOnLoad]
    internal class PurchasingAccess : CloudServiceAccess
    {
        [CompilerGenerated]
        private static RemoteCertificateValidationCallback <>f__am$cache0;
        private const string kETagPath = "Assets/Plugins/UnityPurchasing/ETag";
        private static readonly Uri kPackageUri = new Uri("https://public-cdn.cloud.unity3d.com/UnityEngine.Cloud.Purchasing.unitypackage");
        private const string kServiceDisplayName = "In App Purchasing";
        private const string kServiceName = "Purchasing";
        private const string kServiceUrl = "https://public-cdn.cloud.unity3d.com/editor/production/cloud/purchasing";
        private const string kUnknownPackageETag = "unknown";
        private bool m_InstallInProgress;

        static PurchasingAccess()
        {
            UnityConnectServiceData cloudService = new UnityConnectServiceData("Purchasing", "https://public-cdn.cloud.unity3d.com/editor/production/cloud/purchasing", new PurchasingAccess(), "unity/project/cloud/purchasing");
            UnityConnectServiceCollection.instance.AddService(cloudService);
        }

        public override void EnableService(bool enabled)
        {
            PurchasingSettings.enabled = enabled;
        }

        public string GetInstalledETag()
        {
            if (File.Exists("Assets/Plugins/UnityPurchasing/ETag"))
            {
                return File.ReadAllText("Assets/Plugins/UnityPurchasing/ETag");
            }
            if (Directory.Exists(Path.GetDirectoryName("Assets/Plugins/UnityPurchasing/ETag")))
            {
                return "unknown";
            }
            return null;
        }

        public override string GetServiceDisplayName()
        {
            return "In App Purchasing";
        }

        public override string GetServiceName()
        {
            return "Purchasing";
        }

        public void InstallUnityPackage()
        {
            <InstallUnityPackage>c__AnonStorey0 storey = new <InstallUnityPackage>c__AnonStorey0 {
                $this = this
            };
            if (!this.m_InstallInProgress)
            {
                storey.originalCallback = ServicePointManager.ServerCertificateValidationCallback;
                if (Application.platform != RuntimePlatform.OSXEditor)
                {
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = (a, b, c, d) => true;
                    }
                    ServicePointManager.ServerCertificateValidationCallback = <>f__am$cache0;
                }
                this.m_InstallInProgress = true;
                storey.location = FileUtil.GetUniqueTempPathInProject();
                storey.location = Path.ChangeExtension(storey.location, ".unitypackage");
                storey.client = new WebClient();
                storey.client.DownloadFileCompleted += new AsyncCompletedEventHandler(storey.<>m__0);
                storey.client.DownloadFileAsync(kPackageUri, storey.location);
            }
        }

        public override bool IsServiceEnabled()
        {
            return PurchasingSettings.enabled;
        }

        private void SaveETag(WebClient client)
        {
            string contents = client.ResponseHeaders.Get("ETag");
            if (contents != null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName("Assets/Plugins/UnityPurchasing/ETag"));
                File.WriteAllText("Assets/Plugins/UnityPurchasing/ETag", contents);
            }
        }

        [CompilerGenerated]
        private sealed class <InstallUnityPackage>c__AnonStorey0
        {
            internal PurchasingAccess $this;
            internal WebClient client;
            internal string location;
            internal RemoteCertificateValidationCallback originalCallback;

            internal void <>m__0(object sender, AsyncCompletedEventArgs args)
            {
                <InstallUnityPackage>c__AnonStorey1 storey = new <InstallUnityPackage>c__AnonStorey1 {
                    <>f__ref$0 = this,
                    args = args,
                    handler = null
                };
                storey.handler = new EditorApplication.CallbackFunction(storey.<>m__0);
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, storey.handler);
            }

            private sealed class <InstallUnityPackage>c__AnonStorey1
            {
                internal PurchasingAccess.<InstallUnityPackage>c__AnonStorey0 <>f__ref$0;
                internal AsyncCompletedEventArgs args;
                internal EditorApplication.CallbackFunction handler;

                internal void <>m__0()
                {
                    ServicePointManager.ServerCertificateValidationCallback = this.<>f__ref$0.originalCallback;
                    EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, this.handler);
                    this.<>f__ref$0.$this.m_InstallInProgress = false;
                    if (this.args.Error == null)
                    {
                        this.<>f__ref$0.$this.SaveETag(this.<>f__ref$0.client);
                        AssetDatabase.ImportPackage(this.<>f__ref$0.location, false);
                    }
                    else
                    {
                        Debug.LogWarning("Failed to download IAP package. Please check connectivity and retry.");
                        Debug.LogException(this.args.Error);
                    }
                }
            }
        }
    }
}

