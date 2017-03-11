namespace UnityEditor.Purchasing
{
    using System;
    using System.Net;
    using System.Net.Security;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using UnityEditor.Connect;
    using UnityEngine;

    public static class CloudCatalogUploader
    {
        [CompilerGenerated]
        private static RemoteCertificateValidationCallback <>f__am$cache0;

        public static void Upload(string catalogJson)
        {
            Upload(catalogJson, null, null);
        }

        public static void Upload(string catalogJson, Action<UploadDataCompletedEventArgs> onComplete)
        {
            Upload(catalogJson, onComplete, null);
        }

        public static void Upload(string catalogJson, Action<UploadDataCompletedEventArgs> onComplete, Action<UploadProgressChangedEventArgs> onProgressChanged)
        {
            Upload(catalogJson, onComplete, onProgressChanged, UnityConnect.instance.GetCoreConfigurationUrl());
        }

        public static void Upload(string catalogJson, Action<UploadDataCompletedEventArgs> onComplete, Action<UploadProgressChangedEventArgs> onProgressChanged, string baseURL)
        {
            <Upload>c__AnonStorey0 storey = new <Upload>c__AnonStorey0 {
                onComplete = onComplete,
                onProgressChanged = onProgressChanged
            };
            string str = baseURL;
            Uri address = new Uri((str + "/api/v2/projects/") + UnityConnect.instance.GetProjectGUID() + "/iap_catalog");
            byte[] bytes = Encoding.UTF8.GetBytes(catalogJson);
            storey.originalCallback = ServicePointManager.ServerCertificateValidationCallback;
            if (Application.platform != RuntimePlatform.OSXEditor)
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = (a, b, c, d) => true;
                }
                ServicePointManager.ServerCertificateValidationCallback = <>f__am$cache0;
            }
            WebClient client = new WebClient {
                Encoding = Encoding.UTF8
            };
            client.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {UnityConnect.instance.GetAccessToken()}");
            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            if (storey.onComplete != null)
            {
                client.UploadDataCompleted += new UploadDataCompletedEventHandler(storey.<>m__0);
            }
            if (storey.onProgressChanged != null)
            {
                client.UploadProgressChanged += new UploadProgressChangedEventHandler(storey.<>m__1);
            }
            client.UploadDataAsync(address, "POST", bytes);
        }

        [CompilerGenerated]
        private sealed class <Upload>c__AnonStorey0
        {
            internal Action<UploadDataCompletedEventArgs> onComplete;
            internal Action<UploadProgressChangedEventArgs> onProgressChanged;
            internal RemoteCertificateValidationCallback originalCallback;

            internal void <>m__0(object o, UploadDataCompletedEventArgs eventArgs)
            {
                ServicePointManager.ServerCertificateValidationCallback = this.originalCallback;
                this.onComplete(eventArgs);
            }

            internal void <>m__1(object o, UploadProgressChangedEventArgs eventArgs)
            {
                this.onProgressChanged(eventArgs);
            }
        }
    }
}

