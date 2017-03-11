namespace UnityEngine.Advertisements.Android
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEngine;
    using UnityEngine.Advertisements;

    internal sealed class Platform : AndroidJavaProxy, IPlatform
    {
        private readonly CallbackExecutor m_CallbackExecutor;
        private readonly AndroidJavaObject m_CurrentActivity;
        private readonly AndroidJavaClass m_UnityAds;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event EventHandler<ErrorEventArgs> OnError;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event EventHandler<FinishEventArgs> OnFinish;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event EventHandler<ReadyEventArgs> OnReady;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event EventHandler<StartEventArgs> OnStart;

        public Platform() : base("com.unity3d.ads.IUnityAdsListener")
        {
            this.m_CurrentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            this.m_UnityAds = new AndroidJavaClass("com.unity3d.ads.UnityAds");
            GameObject target = new GameObject("UnityAdsCallbackExecutorObject") {
                hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector
            };
            this.m_CallbackExecutor = target.AddComponent<CallbackExecutor>();
            UnityEngine.Object.DontDestroyOnLoad(target);
        }

        public PlacementState GetPlacementState(string placementId)
        {
            AndroidJavaObject obj2;
            if (placementId == null)
            {
                obj2 = this.m_UnityAds.CallStatic<AndroidJavaObject>("getPlacementState", new object[0]);
            }
            else
            {
                object[] args = new object[] { placementId };
                obj2 = this.m_UnityAds.CallStatic<AndroidJavaObject>("getPlacementState", args);
            }
            return obj2.Call<int>("ordinal", new object[0]);
        }

        public void Initialize(string gameId, bool testMode)
        {
            object[] args = new object[] { this.m_CurrentActivity, gameId, this, testMode };
            this.m_UnityAds.CallStatic("initialize", args);
        }

        public bool IsReady(string placementId)
        {
            if (placementId == null)
            {
                return this.m_UnityAds.CallStatic<bool>("isReady", new object[0]);
            }
            object[] args = new object[] { placementId };
            return this.m_UnityAds.CallStatic<bool>("isReady", args);
        }

        private void onUnityAdsError(AndroidJavaObject rawError, string message)
        {
            <onUnityAdsError>c__AnonStorey4 storey = new <onUnityAdsError>c__AnonStorey4 {
                message = message,
                $this = this,
                handler = this.OnError
            };
            if (storey.handler != null)
            {
                <onUnityAdsError>c__AnonStorey5 storey2 = new <onUnityAdsError>c__AnonStorey5 {
                    <>f__ref$4 = storey,
                    error = (long) rawError.Call<int>("ordinal", new object[0])
                };
                this.m_CallbackExecutor.Post(new Action<CallbackExecutor>(storey2.<>m__0));
            }
        }

        private void onUnityAdsFinish(string placementId, AndroidJavaObject rawShowResult)
        {
            <onUnityAdsFinish>c__AnonStorey2 storey = new <onUnityAdsFinish>c__AnonStorey2 {
                placementId = placementId,
                $this = this,
                handler = this.OnFinish
            };
            if (storey.handler != null)
            {
                <onUnityAdsFinish>c__AnonStorey3 storey2 = new <onUnityAdsFinish>c__AnonStorey3 {
                    <>f__ref$2 = storey,
                    showResult = rawShowResult.Call<int>("ordinal", new object[0])
                };
                this.m_CallbackExecutor.Post(new Action<CallbackExecutor>(storey2.<>m__0));
            }
        }

        private void onUnityAdsReady(string placementId)
        {
            <onUnityAdsReady>c__AnonStorey0 storey = new <onUnityAdsReady>c__AnonStorey0 {
                placementId = placementId,
                $this = this,
                handler = this.OnReady
            };
            if (storey.handler != null)
            {
                this.m_CallbackExecutor.Post(new Action<CallbackExecutor>(storey.<>m__0));
            }
        }

        private void onUnityAdsStart(string placementId)
        {
            <onUnityAdsStart>c__AnonStorey1 storey = new <onUnityAdsStart>c__AnonStorey1 {
                placementId = placementId,
                $this = this,
                handler = this.OnStart
            };
            if (storey.handler != null)
            {
                this.m_CallbackExecutor.Post(new Action<CallbackExecutor>(storey.<>m__0));
            }
        }

        public void SetMetaData(MetaData metaData)
        {
            object[] args = new object[] { this.m_CurrentActivity };
            AndroidJavaObject obj2 = new AndroidJavaObject("com.unity3d.ads.metadata.MetaData", args);
            object[] objArray2 = new object[] { metaData.category };
            obj2.Call("setCategory", objArray2);
            foreach (KeyValuePair<string, object> pair in metaData.Values)
            {
                object[] objArray3 = new object[] { pair.Key, pair.Value };
                obj2.Call("set", objArray3);
            }
            obj2.Call("commit", new object[0]);
        }

        public void Show(string placementId)
        {
            if (placementId == null)
            {
                object[] args = new object[] { this.m_CurrentActivity };
                this.m_UnityAds.CallStatic("show", args);
            }
            else
            {
                object[] objArray2 = new object[] { this.m_CurrentActivity, placementId };
                this.m_UnityAds.CallStatic("show", objArray2);
            }
        }

        public bool debugMode
        {
            get => 
                this.m_UnityAds.CallStatic<bool>("getDebugMode", new object[0]);
            set
            {
                object[] args = new object[] { value };
                this.m_UnityAds.CallStatic("setDebugMode", args);
            }
        }

        public bool isInitialized =>
            this.m_UnityAds.CallStatic<bool>("isInitialized", new object[0]);

        public bool isSupported =>
            this.m_UnityAds.CallStatic<bool>("isSupported", new object[0]);

        public string version =>
            this.m_UnityAds.CallStatic<string>("getVersion", new object[0]);

        [CompilerGenerated]
        private sealed class <onUnityAdsError>c__AnonStorey4
        {
            internal Platform $this;
            internal EventHandler<ErrorEventArgs> handler;
            internal string message;
        }

        [CompilerGenerated]
        private sealed class <onUnityAdsError>c__AnonStorey5
        {
            internal Platform.<onUnityAdsError>c__AnonStorey4 <>f__ref$4;
            internal long error;

            internal void <>m__0(CallbackExecutor executor)
            {
                this.<>f__ref$4.handler(this.<>f__ref$4.$this, new ErrorEventArgs(this.error, this.<>f__ref$4.message));
            }
        }

        [CompilerGenerated]
        private sealed class <onUnityAdsFinish>c__AnonStorey2
        {
            internal Platform $this;
            internal EventHandler<FinishEventArgs> handler;
            internal string placementId;
        }

        [CompilerGenerated]
        private sealed class <onUnityAdsFinish>c__AnonStorey3
        {
            internal Platform.<onUnityAdsFinish>c__AnonStorey2 <>f__ref$2;
            internal ShowResult showResult;

            internal void <>m__0(CallbackExecutor executor)
            {
                this.<>f__ref$2.handler(this.<>f__ref$2.$this, new FinishEventArgs(this.<>f__ref$2.placementId, this.showResult));
            }
        }

        [CompilerGenerated]
        private sealed class <onUnityAdsReady>c__AnonStorey0
        {
            internal Platform $this;
            internal EventHandler<ReadyEventArgs> handler;
            internal string placementId;

            internal void <>m__0(CallbackExecutor executor)
            {
                this.handler(this.$this, new ReadyEventArgs(this.placementId));
            }
        }

        [CompilerGenerated]
        private sealed class <onUnityAdsStart>c__AnonStorey1
        {
            internal Platform $this;
            internal EventHandler<StartEventArgs> handler;
            internal string placementId;

            internal void <>m__0(CallbackExecutor executor)
            {
                this.handler(this.$this, new StartEventArgs(this.placementId));
            }
        }
    }
}

