namespace UnityEngine.Advertisements.Editor
{
    using System;
    using System.IO;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEngine;
    using UnityEngine.Advertisements;

    internal sealed class Platform : IPlatform
    {
        private Configuration m_Configuration;
        private bool m_DebugMode;
        private Placeholder m_Placeholder;
        private static string s_BaseUrl = "http://adserver.unityads.unity3d.com/games";

        public event EventHandler<UnityEngine.Advertisements.ErrorEventArgs> OnError
        {
            add
            {
            }
            remove
            {
            }
        }

        public event EventHandler<FinishEventArgs> OnFinish;

        public event EventHandler<ReadyEventArgs> OnReady
        {
            add
            {
            }
            remove
            {
            }
        }

        public event EventHandler<StartEventArgs> OnStart;

        public Platform(string extensionPath)
        {
            GameObject obj3 = new GameObject("UnityAdsEditorPlaceHolderObject") {
                hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector
            };
            this.m_Placeholder = obj3.AddComponent<Placeholder>();
            this.m_Placeholder.OnFinish += new EventHandler<FinishEventArgs>(this.<Platform>m__0);
            this.m_Placeholder.Load(extensionPath);
        }

        [CompilerGenerated]
        private void <Platform>m__0(object sender, FinishEventArgs e)
        {
            EventHandler<FinishEventArgs> onFinish = this.OnFinish;
            if (onFinish != null)
            {
                onFinish(sender, new FinishEventArgs(e.placementId, e.showResult));
            }
        }

        public PlacementState GetPlacementState(string placementId)
        {
            if (this.IsReady(placementId))
            {
                return PlacementState.Ready;
            }
            return PlacementState.NotAvailable;
        }

        public void Initialize(string gameId, bool testMode)
        {
            <Initialize>c__AnonStorey0 storey = new <Initialize>c__AnonStorey0 {
                gameId = gameId,
                $this = this
            };
            Debug.Log(string.Concat(new object[] { "UnityAdsEditor: Initialize(", storey.gameId, ", ", testMode, ");" }));
            string[] textArray1 = new string[3];
            textArray1[0] = s_BaseUrl;
            textArray1[1] = storey.gameId;
            string[] textArray2 = new string[] { "configuration?platform=editor", "unityVersion=" + Uri.EscapeDataString(Application.unityVersion), "sdkVersionName=" + Uri.EscapeDataString(this.version) };
            textArray1[2] = string.Join("&", textArray2);
            string requestUriString = string.Join("/", textArray1);
            storey.request = WebRequest.Create(requestUriString);
            storey.request.BeginGetResponse(new AsyncCallback(storey.<>m__0), null);
        }

        public bool IsReady(string placementId)
        {
            if (placementId == null)
            {
                return this.isInitialized;
            }
            return (this.isInitialized && this.m_Configuration.placements.ContainsKey(placementId));
        }

        public void SetMetaData(MetaData metaData)
        {
        }

        public void Show(string placementId)
        {
            if (this.isInitialized && (placementId == null))
            {
                placementId = this.m_Configuration.defaultPlacement;
            }
            if (this.IsReady(placementId))
            {
                EventHandler<StartEventArgs> onStart = this.OnStart;
                if (onStart != null)
                {
                    onStart(this, new StartEventArgs(placementId));
                }
                this.m_Placeholder.Show(placementId, this.m_Configuration.placements[placementId]);
            }
            else
            {
                EventHandler<FinishEventArgs> onFinish = this.OnFinish;
                if (onFinish != null)
                {
                    onFinish(this, new FinishEventArgs(placementId, ShowResult.Failed));
                }
            }
        }

        public bool debugMode
        {
            get => 
                this.m_DebugMode;
            set
            {
                this.m_DebugMode = value;
            }
        }

        public bool isInitialized =>
            (this.m_Configuration != null);

        public bool isSupported =>
            Application.isEditor;

        public string version =>
            "2.0.6";

        [CompilerGenerated]
        private sealed class <Initialize>c__AnonStorey0
        {
            internal Platform $this;
            internal string gameId;
            internal WebRequest request;

            internal void <>m__0(IAsyncResult result)
            {
                WebResponse response = this.request.EndGetResponse(result);
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string configurationResponse = reader.ReadToEnd();
                try
                {
                    this.$this.m_Configuration = new Configuration(configurationResponse);
                    if (!this.$this.m_Configuration.enabled)
                    {
                        Debug.LogWarning("gameId " + this.gameId + " is not enabled");
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogError("Failed to parse configuration for gameId: " + this.gameId);
                    Debug.Log(configurationResponse);
                    Debug.LogException(exception);
                }
                reader.Close();
                response.Close();
            }
        }
    }
}

