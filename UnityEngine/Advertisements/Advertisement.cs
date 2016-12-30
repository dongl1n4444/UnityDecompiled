namespace UnityEngine.Advertisements
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Advertisements.Android;
    using UnityEngine.Advertisements.Editor;
    using UnityEngine.Advertisements.iOS;
    using UnityEngine.Connect;

    /// <summary>
    /// <para>Unity Ads.</para>
    /// </summary>
    public static class Advertisement
    {
        [CompilerGenerated]
        private static EventHandler<StartEventArgs> <>f__am$cache0;
        [CompilerGenerated]
        private static EventHandler<FinishEventArgs> <>f__am$cache1;
        private static DebugLevelInternal s_DebugLevel = (!Debug.isDebugBuild ? (DebugLevelInternal.Info | DebugLevelInternal.Warning | DebugLevelInternal.Error) : (DebugLevelInternal.Debug | DebugLevelInternal.Info | DebugLevelInternal.Warning | DebugLevelInternal.Error));
        private static bool s_EditorSupportedPlatform;
        private static bool s_Initialized;
        private static IPlatform s_Platform;
        private static bool s_Showing;

        /// <summary>
        /// <para>Returns the placement state.</para>
        /// </summary>
        /// <param name="placementId">Placement identifier.</param>
        /// <returns>
        /// <para>Placement state.</para>
        /// </returns>
        public static PlacementState GetPlacementState() => 
            GetPlacementState(null);

        /// <summary>
        /// <para>Returns the placement state.</para>
        /// </summary>
        /// <param name="placementId">Placement identifier.</param>
        /// <returns>
        /// <para>Placement state.</para>
        /// </returns>
        public static PlacementState GetPlacementState(string placementId) => 
            s_Platform.GetPlacementState(!string.IsNullOrEmpty(placementId) ? placementId : null);

        /// <summary>
        /// <para>Manually initializes the advertisement system. Normally this is done from editor, and you should only call this method if you are using UnityAds with automatic initialization disabled.</para>
        /// </summary>
        /// <param name="gameId">Your game id. You can see a list of your registered games at the UnityAds admin site.</param>
        /// <param name="testMode">In test mode, you will see test advertisement. Can be overruled by settings in the admin site for game.</param>
        public static void Initialize(string gameId)
        {
            Initialize(gameId, false);
        }

        /// <summary>
        /// <para>Manually initializes the advertisement system. Normally this is done from editor, and you should only call this method if you are using UnityAds with automatic initialization disabled.</para>
        /// </summary>
        /// <param name="gameId">Your game id. You can see a list of your registered games at the UnityAds admin site.</param>
        /// <param name="testMode">In test mode, you will see test advertisement. Can be overruled by settings in the admin site for game.</param>
        public static void Initialize(string gameId, bool testMode)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = (EventHandler<StartEventArgs>) ((sender, e) => (isShowing = true));
                }
                s_Platform.OnStart += <>f__am$cache0;
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = (EventHandler<FinishEventArgs>) ((sender, e) => (isShowing = false));
                }
                s_Platform.OnFinish += <>f__am$cache1;
                MetaData metaData = new MetaData("framework");
                metaData.Set("name", "Unity");
                metaData.Set("version", Application.unityVersion);
                SetMetaData(metaData);
                MetaData data2 = new MetaData("adapter");
                data2.Set("name", "Engine");
                data2.Set("version", version);
                SetMetaData(data2);
                s_Platform.Initialize(gameId, testMode);
            }
        }

        private static bool IsEnabled() => 
            (UnityAdsSettings.enabled && UnityAdsSettings.IsPlatformEnabled(Application.platform));

        /// <summary>
        /// <para>Returns whether an advertisement is ready to be shown. Placements are configured per game in the UnityAds admin site, where you can also set your default placement.</para>
        /// </summary>
        /// <param name="placementId">Optional placement identifier. If not specified, your default placement specified in UnityAds server-side admin settings will be used.</param>
        /// <returns>
        /// <para>If the placement is ready.</para>
        /// </returns>
        public static bool IsReady() => 
            IsReady(null);

        /// <summary>
        /// <para>Returns whether an advertisement is ready to be shown. Placements are configured per game in the UnityAds admin site, where you can also set your default placement.</para>
        /// </summary>
        /// <param name="placementId">Optional placement identifier. If not specified, your default placement specified in UnityAds server-side admin settings will be used.</param>
        /// <returns>
        /// <para>If the placement is ready.</para>
        /// </returns>
        public static bool IsReady(string placementId) => 
            s_Platform.IsReady(!string.IsNullOrEmpty(placementId) ? placementId : null);

        private static void Load()
        {
            if (((s_Platform != null) && isSupported) && initializeOnStartup)
            {
                Initialize(gameId, testMode);
            }
        }

        private static void LoadEditor(string extensionPath, bool supportedPlatform)
        {
            if (s_Platform == null)
            {
                if (supportedPlatform)
                {
                    s_Platform = new UnityEngine.Advertisements.Editor.Platform(extensionPath);
                    s_EditorSupportedPlatform = true;
                }
                else
                {
                    s_Platform = new UnsupportedPlatform();
                }
                Load();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LoadRuntime()
        {
            if (((s_Platform != null) || Application.isEditor) || !isSupported)
            {
                return;
            }
            try
            {
                RuntimePlatform platform = Application.platform;
                if (platform != RuntimePlatform.Android)
                {
                    if (platform == RuntimePlatform.IPhonePlayer)
                    {
                        goto Label_004F;
                    }
                    goto Label_005E;
                }
                s_Platform = new UnityEngine.Advertisements.Android.Platform();
                goto Label_0095;
            Label_004F:
                s_Platform = new UnityEngine.Advertisements.iOS.Platform();
                goto Label_0095;
            Label_005E:
                s_Platform = new UnsupportedPlatform();
            }
            catch (Exception exception)
            {
                Debug.LogError("Initializing Unity Ads.");
                Debug.LogException(exception);
                s_Platform = new UnsupportedPlatform();
            }
        Label_0095:
            Load();
        }

        /// <summary>
        /// <para>Sets various metadata for Unity Ads.</para>
        /// </summary>
        /// <param name="metaData">MetaData to be set.</param>
        public static void SetMetaData(MetaData metaData)
        {
            s_Platform.SetMetaData(metaData);
        }

        /// <summary>
        /// <para>Show an advertisement in your project.</para>
        /// </summary>
        /// <param name="placementId">Optional placement identifier. If not specified, your default placement specified in the admin settings will be used.</param>
        /// <param name="showOptions">Specify e.g. callback handler to be called when video has finished.</param>
        public static void Show()
        {
            Show(null, null);
        }

        /// <summary>
        /// <para>Show an advertisement in your project.</para>
        /// </summary>
        /// <param name="placementId">Optional placement identifier. If not specified, your default placement specified in the admin settings will be used.</param>
        /// <param name="showOptions">Specify e.g. callback handler to be called when video has finished.</param>
        public static void Show(string placementId)
        {
            Show(placementId, null);
        }

        /// <summary>
        /// <para>Show an advertisement in your project.</para>
        /// </summary>
        /// <param name="placementId">Optional placement identifier. If not specified, your default placement specified in the admin settings will be used.</param>
        /// <param name="showOptions">Specify e.g. callback handler to be called when video has finished.</param>
        public static void Show(ShowOptions showOptions)
        {
            Show(null, showOptions);
        }

        /// <summary>
        /// <para>Show an advertisement in your project.</para>
        /// </summary>
        /// <param name="placementId">Optional placement identifier. If not specified, your default placement specified in the admin settings will be used.</param>
        /// <param name="showOptions">Specify e.g. callback handler to be called when video has finished.</param>
        public static void Show(string placementId, ShowOptions showOptions)
        {
            <Show>c__AnonStorey0 storey = new <Show>c__AnonStorey0 {
                showOptions = showOptions
            };
            if (storey.showOptions != null)
            {
                if (storey.showOptions.resultCallback != null)
                {
                    <Show>c__AnonStorey1 storey2 = new <Show>c__AnonStorey1 {
                        <>f__ref$0 = storey,
                        finishHandler = null
                    };
                    storey2.finishHandler = new EventHandler<FinishEventArgs>(storey2.<>m__0);
                    s_Platform.OnFinish += storey2.finishHandler;
                }
                if (!string.IsNullOrEmpty(storey.showOptions.gamerSid))
                {
                    MetaData metaData = new MetaData("player");
                    metaData.Set("server_id", storey.showOptions.gamerSid);
                    SetMetaData(metaData);
                }
            }
            s_Platform.Show(!string.IsNullOrEmpty(placementId) ? placementId : null);
        }

        /// <summary>
        /// <para>Controls the amount of logging output from the advertisement system.</para>
        /// </summary>
        [Obsolete("Use Advertisement.debugMode instead.")]
        public static DebugLevel debugLevel
        {
            get => 
                ((DebugLevel) s_DebugLevel);
            set
            {
                s_DebugLevel = (DebugLevelInternal) value;
            }
        }

        /// <summary>
        /// <para>Controls the amount of logging output from the advertisement system.</para>
        /// </summary>
        public static bool debugMode
        {
            get => 
                s_Platform.debugMode;
            set
            {
                s_Platform.debugMode = value;
            }
        }

        /// <summary>
        /// <para>Returns the game identifier for the current platform.</para>
        /// </summary>
        public static string gameId =>
            UnityAdsSettings.GetGameId(Application.platform);

        private static bool initializeOnStartup =>
            UnityAdsSettings.initializeOnStartup;

        /// <summary>
        /// <para>Returns whether the advertisement system is initialized successfully.</para>
        /// </summary>
        public static bool isInitialized
        {
            get => 
                s_Initialized;
            private set
            {
                s_Initialized = value;
            }
        }

        /// <summary>
        /// <para>Returns whether an advertisement is currently being shown.</para>
        /// </summary>
        public static bool isShowing
        {
            get => 
                s_Showing;
            private set
            {
                s_Showing = value;
            }
        }

        /// <summary>
        /// <para>Returns if the current platform is supported by the advertisement system.</para>
        /// </summary>
        public static bool isSupported =>
            (((Application.isEditor && s_EditorSupportedPlatform) || ((Application.platform == RuntimePlatform.IPhonePlayer) || (Application.platform == RuntimePlatform.Android))) && IsEnabled());

        /// <summary>
        /// <para>Returns whether the testing mode is enabled.</para>
        /// </summary>
        public static bool testMode =>
            UnityAdsSettings.testMode;

        /// <summary>
        /// <para>Returns the current Unity Ads version.</para>
        /// </summary>
        public static string version =>
            s_Platform.version;

        [CompilerGenerated]
        private sealed class <Show>c__AnonStorey0
        {
            internal ShowOptions showOptions;
        }

        [CompilerGenerated]
        private sealed class <Show>c__AnonStorey1
        {
            internal Advertisement.<Show>c__AnonStorey0 <>f__ref$0;
            internal EventHandler<FinishEventArgs> finishHandler;

            internal void <>m__0(object sender, FinishEventArgs e)
            {
                this.<>f__ref$0.showOptions.resultCallback(e.showResult);
                Advertisement.s_Platform.OnFinish -= this.finishHandler;
            }
        }

        /// <summary>
        /// <para>Player debug message level.</para>
        /// </summary>
        [Obsolete("Use Advertisement.debugMode instead."), Flags]
        public enum DebugLevel
        {
            /// <summary>
            /// <para>Prints all debugging messages.</para>
            /// </summary>
            Debug = 8,
            /// <summary>
            /// <para>Prints all error messages.</para>
            /// </summary>
            Error = 1,
            /// <summary>
            /// <para>Prints all informational messages.</para>
            /// </summary>
            Info = 4,
            /// <summary>
            /// <para>Prints out no debugging output.</para>
            /// </summary>
            None = 0,
            /// <summary>
            /// <para>Prints out warnings.</para>
            /// </summary>
            Warning = 2
        }

        [Flags]
        private enum DebugLevelInternal
        {
            Debug = 8,
            Error = 1,
            Info = 4,
            None = 0,
            Warning = 2
        }
    }
}

