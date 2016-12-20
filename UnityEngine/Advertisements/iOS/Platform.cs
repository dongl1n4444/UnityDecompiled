namespace UnityEngine.Advertisements.iOS
{
    using AOT;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine;
    using UnityEngine.Advertisements;

    internal sealed class Platform : IPlatform
    {
        [CompilerGenerated]
        private static unityAdsReady <>f__mg$cache0;
        [CompilerGenerated]
        private static unityAdsDidError <>f__mg$cache1;
        [CompilerGenerated]
        private static unityAdsDidStart <>f__mg$cache2;
        [CompilerGenerated]
        private static unityAdsDidFinish <>f__mg$cache3;
        private static CallbackExecutor s_CallbackExecutor;
        private static Platform s_Instance;

        public event EventHandler<ErrorEventArgs> OnError;

        public event EventHandler<FinishEventArgs> OnFinish;

        public event EventHandler<ReadyEventArgs> OnReady;

        public event EventHandler<StartEventArgs> OnStart;

        public Platform()
        {
            s_Instance = this;
            GameObject obj3 = new GameObject("UnityAdsCallbackExecutorObject") {
                hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector
            };
            s_CallbackExecutor = obj3.AddComponent<CallbackExecutor>();
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new unityAdsReady(Platform.UnityAdsReady);
            }
            UnityAdsEngineSetReadyCallback(<>f__mg$cache0);
            if (<>f__mg$cache1 == null)
            {
                <>f__mg$cache1 = new unityAdsDidError(Platform.UnityAdsDidError);
            }
            UnityAdsEngineSetDidErrorCallback(<>f__mg$cache1);
            if (<>f__mg$cache2 == null)
            {
                <>f__mg$cache2 = new unityAdsDidStart(Platform.UnityAdsDidStart);
            }
            UnityAdsEngineSetDidStartCallback(<>f__mg$cache2);
            if (<>f__mg$cache3 == null)
            {
                <>f__mg$cache3 = new unityAdsDidFinish(Platform.UnityAdsDidFinish);
            }
            UnityAdsEngineSetDidFinishCallback(<>f__mg$cache3);
        }

        public PlacementState GetPlacementState(string placementId)
        {
            return (PlacementState) ((int) UnityAdsEngineGetPlacementState(placementId));
        }

        public void Initialize(string gameId, bool testMode)
        {
            UnityAdsEngineInitialize(gameId, testMode);
        }

        public bool IsReady(string placementId)
        {
            return UnityAdsEngineIsReady(placementId);
        }

        public void SetMetaData(MetaData metaData)
        {
            UnityAdsEngineSetMetaData(metaData.category, metaData.ToJSON());
        }

        public void Show(string placementId)
        {
            UnityAdsEngineShow(placementId);
        }

        [MonoPInvokeCallback(typeof(unityAdsDidError))]
        private static void UnityAdsDidError(long rawError, string message)
        {
            <UnityAdsDidError>c__AnonStorey1 storey = new <UnityAdsDidError>c__AnonStorey1 {
                rawError = rawError,
                message = message,
                handler = s_Instance.OnError
            };
            if (storey.handler != null)
            {
                s_CallbackExecutor.Post(new Action<CallbackExecutor>(storey.<>m__0));
            }
        }

        [MonoPInvokeCallback(typeof(unityAdsDidFinish))]
        private static void UnityAdsDidFinish(string placementId, long rawShowResult)
        {
            <UnityAdsDidFinish>c__AnonStorey3 storey = new <UnityAdsDidFinish>c__AnonStorey3 {
                placementId = placementId,
                handler = s_Instance.OnFinish
            };
            if (storey.handler != null)
            {
                <UnityAdsDidFinish>c__AnonStorey4 storey2 = new <UnityAdsDidFinish>c__AnonStorey4 {
                    <>f__ref$3 = storey,
                    showResult = (ShowResult) ((int) rawShowResult)
                };
                s_CallbackExecutor.Post(new Action<CallbackExecutor>(storey2.<>m__0));
            }
        }

        [MonoPInvokeCallback(typeof(unityAdsDidStart))]
        private static void UnityAdsDidStart(string placementId)
        {
            <UnityAdsDidStart>c__AnonStorey2 storey = new <UnityAdsDidStart>c__AnonStorey2 {
                placementId = placementId,
                handler = s_Instance.OnStart
            };
            if (storey.handler != null)
            {
                s_CallbackExecutor.Post(new Action<CallbackExecutor>(storey.<>m__0));
            }
        }

        [DllImport("__Internal")]
        private static extern bool UnityAdsEngineGetDebugMode();
        [DllImport("__Internal")]
        private static extern long UnityAdsEngineGetPlacementState(string placementId);
        [DllImport("__Internal")]
        private static extern string UnityAdsEngineGetVersion();
        [DllImport("__Internal")]
        private static extern void UnityAdsEngineInitialize(string gameId, bool testMode);
        [DllImport("__Internal")]
        private static extern bool UnityAdsEngineIsInitialized();
        [DllImport("__Internal")]
        private static extern bool UnityAdsEngineIsReady(string placementId);
        [DllImport("__Internal")]
        private static extern bool UnityAdsEngineIsSupported();
        [DllImport("__Internal")]
        private static extern void UnityAdsEngineSetDebugMode(bool debugMode);
        [DllImport("__Internal")]
        private static extern void UnityAdsEngineSetDidErrorCallback(unityAdsDidError callback);
        [DllImport("__Internal")]
        private static extern void UnityAdsEngineSetDidFinishCallback(unityAdsDidFinish callback);
        [DllImport("__Internal")]
        private static extern void UnityAdsEngineSetDidStartCallback(unityAdsDidStart callback);
        [DllImport("__Internal")]
        private static extern void UnityAdsEngineSetMetaData(string category, string data);
        [DllImport("__Internal")]
        private static extern void UnityAdsEngineSetReadyCallback(unityAdsReady callback);
        [DllImport("__Internal")]
        private static extern void UnityAdsEngineShow(string placementId);
        [MonoPInvokeCallback(typeof(unityAdsReady))]
        private static void UnityAdsReady(string placementId)
        {
            <UnityAdsReady>c__AnonStorey0 storey = new <UnityAdsReady>c__AnonStorey0 {
                placementId = placementId,
                handler = s_Instance.OnReady
            };
            if (storey.handler != null)
            {
                s_CallbackExecutor.Post(new Action<CallbackExecutor>(storey.<>m__0));
            }
        }

        public bool debugMode
        {
            get
            {
                return UnityAdsEngineGetDebugMode();
            }
            set
            {
                UnityAdsEngineSetDebugMode(value);
            }
        }

        public bool isInitialized
        {
            get
            {
                return UnityAdsEngineIsInitialized();
            }
        }

        public bool isSupported
        {
            get
            {
                return UnityAdsEngineIsSupported();
            }
        }

        public string version
        {
            get
            {
                return UnityAdsEngineGetVersion();
            }
        }

        [CompilerGenerated]
        private sealed class <UnityAdsDidError>c__AnonStorey1
        {
            internal EventHandler<ErrorEventArgs> handler;
            internal string message;
            internal long rawError;

            internal void <>m__0(CallbackExecutor executor)
            {
                this.handler(Platform.s_Instance, new ErrorEventArgs(this.rawError, this.message));
            }
        }

        [CompilerGenerated]
        private sealed class <UnityAdsDidFinish>c__AnonStorey3
        {
            internal EventHandler<FinishEventArgs> handler;
            internal string placementId;
        }

        [CompilerGenerated]
        private sealed class <UnityAdsDidFinish>c__AnonStorey4
        {
            internal Platform.<UnityAdsDidFinish>c__AnonStorey3 <>f__ref$3;
            internal ShowResult showResult;

            internal void <>m__0(CallbackExecutor executor)
            {
                this.<>f__ref$3.handler(Platform.s_Instance, new FinishEventArgs(this.<>f__ref$3.placementId, this.showResult));
            }
        }

        [CompilerGenerated]
        private sealed class <UnityAdsDidStart>c__AnonStorey2
        {
            internal EventHandler<StartEventArgs> handler;
            internal string placementId;

            internal void <>m__0(CallbackExecutor executor)
            {
                this.handler(Platform.s_Instance, new StartEventArgs(this.placementId));
            }
        }

        [CompilerGenerated]
        private sealed class <UnityAdsReady>c__AnonStorey0
        {
            internal EventHandler<ReadyEventArgs> handler;
            internal string placementId;

            internal void <>m__0(CallbackExecutor executor)
            {
                this.handler(Platform.s_Instance, new ReadyEventArgs(this.placementId));
            }
        }

        private delegate void unityAdsDidError(long rawError, string message);

        private delegate void unityAdsDidFinish(string placementId, long rawFinishState);

        private delegate void unityAdsDidStart(string placementId);

        private delegate void unityAdsReady(string placementId);
    }
}

