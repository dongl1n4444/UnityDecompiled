namespace UnityEngine
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Text;
    using UnityEngine.Internal;
    using UnityEngine.SceneManagement;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Access to application run-time data.</para>
    /// </summary>
    public sealed class Application
    {
        internal static AdvertisingIdentifierCallback OnAdvertisingIdentifierCallback;
        private static LogCallback s_LogCallbackHandler;
        private static LogCallback s_LogCallbackHandlerThreaded;
        private static volatile LogCallback s_RegisterLogCallbackDeprecated;

        public static  event LogCallback logMessageReceived
        {
            add
            {
                s_LogCallbackHandler = (LogCallback) Delegate.Combine(s_LogCallbackHandler, value);
                SetLogCallbackDefined(true);
            }
            remove
            {
                s_LogCallbackHandler = (LogCallback) Delegate.Remove(s_LogCallbackHandler, value);
            }
        }

        public static  event LogCallback logMessageReceivedThreaded
        {
            add
            {
                s_LogCallbackHandlerThreaded = (LogCallback) Delegate.Combine(s_LogCallbackHandlerThreaded, value);
                SetLogCallbackDefined(true);
            }
            remove
            {
                s_LogCallbackHandlerThreaded = (LogCallback) Delegate.Remove(s_LogCallbackHandlerThreaded, value);
            }
        }

        private static string BuildInvocationForArguments(string functionName, params object[] args)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(functionName);
            builder.Append('(');
            int length = args.Length;
            for (int i = 0; i < length; i++)
            {
                if (i != 0)
                {
                    builder.Append(", ");
                }
                builder.Append(ObjectToJSString(args[i]));
            }
            builder.Append(')');
            builder.Append(';');
            return builder.ToString();
        }

        [RequiredByNativeCode]
        private static void CallLogCallback(string logString, string stackTrace, LogType type, bool invokedOnMainThread)
        {
            if (invokedOnMainThread)
            {
                LogCallback callback = s_LogCallbackHandler;
                if (callback != null)
                {
                    callback(logString, stackTrace, type);
                }
            }
            LogCallback callback2 = s_LogCallbackHandlerThreaded;
            if (callback2 != null)
            {
                callback2(logString, stackTrace, type);
            }
        }

        /// <summary>
        /// <para>Cancels quitting the application. This is useful for showing a splash screen at the end of a game.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CancelQuit();
        /// <summary>
        /// <para>Can the streamed level be loaded?</para>
        /// </summary>
        /// <param name="levelIndex"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool CanStreamedLevelBeLoaded(int levelIndex);
        /// <summary>
        /// <para>Can the streamed level be loaded?</para>
        /// </summary>
        /// <param name="levelName"></param>
        public static bool CanStreamedLevelBeLoaded(string levelName) => 
            CanStreamedLevelBeLoadedByName(levelName);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool CanStreamedLevelBeLoadedByName(string levelName);
        /// <summary>
        /// <para>Captures a screenshot at path filename as a PNG file.</para>
        /// </summary>
        /// <param name="filename">Pathname to save the screenshot file to.</param>
        /// <param name="superSize">Factor by which to increase resolution.</param>
        [ExcludeFromDocs]
        public static void CaptureScreenshot(string filename)
        {
            int superSize = 0;
            CaptureScreenshot(filename, superSize);
        }

        /// <summary>
        /// <para>Captures a screenshot at path filename as a PNG file.</para>
        /// </summary>
        /// <param name="filename">Pathname to save the screenshot file to.</param>
        /// <param name="superSize">Factor by which to increase resolution.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CaptureScreenshot(string filename, [DefaultValue("0")] int superSize);
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("Use Object.DontDestroyOnLoad instead")]
        public static extern void DontDestroyOnLoad(UnityEngine.Object mono);
        /// <summary>
        /// <para>Calls a function in the web page that contains the WebGL Player.</para>
        /// </summary>
        /// <param name="functionName">Name of the function to call.</param>
        /// <param name="args">Array of arguments passed in the call.</param>
        public static void ExternalCall(string functionName, params object[] args)
        {
            Internal_ExternalCall(BuildInvocationForArguments(functionName, args));
        }

        /// <summary>
        /// <para>Evaluates script function in the containing web page.</para>
        /// </summary>
        /// <param name="script">The Javascript function to call.</param>
        public static void ExternalEval(string script)
        {
            if ((script.Length > 0) && (script[script.Length - 1] != ';'))
            {
                script = script + ';';
            }
            Internal_ExternalCall(script);
        }

        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("For internal use only")]
        public static extern void ForceCrash(int mode);
        /// <summary>
        /// <para>Get stack trace logging options. The default value is StackTraceLogType.ScriptOnly.</para>
        /// </summary>
        /// <param name="logType"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern StackTraceLogType GetStackTraceLogType(LogType logType);
        /// <summary>
        /// <para>How far has the download progressed? [0...1].</para>
        /// </summary>
        /// <param name="levelIndex"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float GetStreamProgressForLevel(int levelIndex);
        /// <summary>
        /// <para>How far has the download progressed? [0...1].</para>
        /// </summary>
        /// <param name="levelName"></param>
        public static float GetStreamProgressForLevel(string levelName) => 
            GetStreamProgressForLevelByName(levelName);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern float GetStreamProgressForLevelByName(string levelName);
        internal static UserAuthorization GetUserAuthorizationRequestMode() => 
            ((UserAuthorization) GetUserAuthorizationRequestMode_Internal());

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int GetUserAuthorizationRequestMode_Internal();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string GetValueForARGV(string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool HasAdvancedLicense();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool HasARGV(string name);
        /// <summary>
        /// <para>Is Unity activated with the Pro license?</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool HasProLicense();
        /// <summary>
        /// <para>Check if the user has authorized use of the webcam or microphone in the Web Player.</para>
        /// </summary>
        /// <param name="mode"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool HasUserAuthorization(UserAuthorization mode);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_ExternalCall(string script);
        internal static void InvokeOnAdvertisingIdentifierCallback(string advertisingId, bool trackingEnabled)
        {
            if (OnAdvertisingIdentifierCallback != null)
            {
                OnAdvertisingIdentifierCallback(advertisingId, trackingEnabled, string.Empty);
            }
        }

        /// <summary>
        /// <para>Loads the level by its name or index.</para>
        /// </summary>
        /// <param name="index">The level to load.</param>
        /// <param name="name">The name of the level to load.</param>
        [Obsolete("Use SceneManager.LoadScene")]
        public static void LoadLevel(int index)
        {
            SceneManager.LoadScene(index, LoadSceneMode.Single);
        }

        /// <summary>
        /// <para>Loads the level by its name or index.</para>
        /// </summary>
        /// <param name="index">The level to load.</param>
        /// <param name="name">The name of the level to load.</param>
        [Obsolete("Use SceneManager.LoadScene")]
        public static void LoadLevel(string name)
        {
            SceneManager.LoadScene(name, LoadSceneMode.Single);
        }

        /// <summary>
        /// <para>Loads a level additively.</para>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="name"></param>
        [Obsolete("Use SceneManager.LoadScene")]
        public static void LoadLevelAdditive(int index)
        {
            SceneManager.LoadScene(index, LoadSceneMode.Additive);
        }

        /// <summary>
        /// <para>Loads a level additively.</para>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="name"></param>
        [Obsolete("Use SceneManager.LoadScene")]
        public static void LoadLevelAdditive(string name)
        {
            SceneManager.LoadScene(name, LoadSceneMode.Additive);
        }

        /// <summary>
        /// <para>Loads the level additively and asynchronously in the background.</para>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="levelName"></param>
        [Obsolete("Use SceneManager.LoadSceneAsync")]
        public static AsyncOperation LoadLevelAdditiveAsync(int index) => 
            SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);

        /// <summary>
        /// <para>Loads the level additively and asynchronously in the background.</para>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="levelName"></param>
        [Obsolete("Use SceneManager.LoadSceneAsync")]
        public static AsyncOperation LoadLevelAdditiveAsync(string levelName) => 
            SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);

        /// <summary>
        /// <para>Loads the level asynchronously in the background.</para>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="levelName"></param>
        [Obsolete("Use SceneManager.LoadSceneAsync")]
        public static AsyncOperation LoadLevelAsync(int index) => 
            SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);

        /// <summary>
        /// <para>Loads the level asynchronously in the background.</para>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="levelName"></param>
        [Obsolete("Use SceneManager.LoadSceneAsync")]
        public static AsyncOperation LoadLevelAsync(string levelName) => 
            SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);

        private static string ObjectToJSString(object o)
        {
            if (o == null)
            {
                return "null";
            }
            if (o is string)
            {
                string str2 = o.ToString().Replace(@"\", @"\\").Replace("\"", "\\\"").Replace("\n", @"\n").Replace("\r", @"\r").Replace("\0", "").Replace("\u2028", "").Replace("\u2029", "");
                return ('"' + str2 + '"');
            }
            if (((o is int) || (o is short)) || (((o is uint) || (o is ushort)) || (o is byte)))
            {
                return o.ToString();
            }
            if (o is float)
            {
                NumberFormatInfo numberFormat = CultureInfo.InvariantCulture.NumberFormat;
                float num = (float) o;
                return num.ToString(numberFormat);
            }
            if (o is double)
            {
                NumberFormatInfo provider = CultureInfo.InvariantCulture.NumberFormat;
                double num2 = (double) o;
                return num2.ToString(provider);
            }
            if (o is char)
            {
                if (((char) o) == '"')
                {
                    return "\"\\\"\"";
                }
                return ('"' + o.ToString() + '"');
            }
            if (o is IList)
            {
                IList list = (IList) o;
                StringBuilder builder = new StringBuilder();
                builder.Append("new Array(");
                int count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    if (i != 0)
                    {
                        builder.Append(", ");
                    }
                    builder.Append(ObjectToJSString(list[i]));
                }
                builder.Append(")");
                return builder.ToString();
            }
            return ObjectToJSString(o.ToString());
        }

        /// <summary>
        /// <para>Opens the url in a browser.</para>
        /// </summary>
        /// <param name="url"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void OpenURL(string url);
        /// <summary>
        /// <para>Quits the player application.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void Quit();
        [Obsolete("Application.RegisterLogCallback is deprecated. Use Application.logMessageReceived instead.")]
        public static void RegisterLogCallback(LogCallback handler)
        {
            RegisterLogCallback(handler, false);
        }

        private static void RegisterLogCallback(LogCallback handler, bool threaded)
        {
            if (s_RegisterLogCallbackDeprecated != null)
            {
                logMessageReceived -= s_RegisterLogCallbackDeprecated;
                logMessageReceivedThreaded -= s_RegisterLogCallbackDeprecated;
            }
            s_RegisterLogCallbackDeprecated = handler;
            if (handler != null)
            {
                if (threaded)
                {
                    logMessageReceivedThreaded += handler;
                }
                else
                {
                    logMessageReceived += handler;
                }
            }
        }

        [Obsolete("Application.RegisterLogCallbackThreaded is deprecated. Use Application.logMessageReceivedThreaded instead.")]
        public static void RegisterLogCallbackThreaded(LogCallback handler)
        {
            RegisterLogCallback(handler, true);
        }

        [ExcludeFromDocs]
        internal static void ReplyToUserAuthorizationRequest(bool reply)
        {
            bool remember = false;
            ReplyToUserAuthorizationRequest(reply, remember);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void ReplyToUserAuthorizationRequest(bool reply, [DefaultValue("false")] bool remember);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool RequestAdvertisingIdentifierAsync(AdvertisingIdentifierCallback delegateMethod);
        /// <summary>
        /// <para>Request authorization to use the webcam or microphone in the Web Player.</para>
        /// </summary>
        /// <param name="mode"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern AsyncOperation RequestUserAuthorization(UserAuthorization mode);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetLogCallbackDefined(bool defined);
        /// <summary>
        /// <para>Set stack trace logging options. The default value is StackTraceLogType.ScriptOnly.</para>
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="stackTraceType"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetStackTraceLogType(LogType logType, StackTraceLogType stackTraceType);
        /// <summary>
        /// <para>Unloads the Unity runtime.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void Unload();
        /// <summary>
        /// <para>Unloads all GameObject associated with the given scene. Note that assets are currently not unloaded, in order to free up asset memory call Resources.UnloadAllUnusedAssets.</para>
        /// </summary>
        /// <param name="index">Index of the scene in the PlayerSettings to unload.</param>
        /// <param name="scenePath">Name of the scene to Unload.</param>
        /// <returns>
        /// <para>Return true if the scene is unloaded.</para>
        /// </returns>
        [Obsolete("Use SceneManager.UnloadScene")]
        public static bool UnloadLevel(int index) => 
            SceneManager.UnloadScene(index);

        /// <summary>
        /// <para>Unloads all GameObject associated with the given scene. Note that assets are currently not unloaded, in order to free up asset memory call Resources.UnloadAllUnusedAssets.</para>
        /// </summary>
        /// <param name="index">Index of the scene in the PlayerSettings to unload.</param>
        /// <param name="scenePath">Name of the scene to Unload.</param>
        /// <returns>
        /// <para>Return true if the scene is unloaded.</para>
        /// </returns>
        [Obsolete("Use SceneManager.UnloadScene")]
        public static bool UnloadLevel(string scenePath) => 
            SceneManager.UnloadScene(scenePath);

        [Obsolete("absoluteUrl is deprecated. Please use absoluteURL instead (UnityUpgradable) -> absoluteURL", true)]
        public static string absoluteUrl =>
            absoluteURL;

        /// <summary>
        /// <para>The absolute path to the web player data file (Read Only).</para>
        /// </summary>
        public static string absoluteURL { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Priority of background loading thread.</para>
        /// </summary>
        public static ThreadPriority backgroundLoadingPriority { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Returns application bundle identifier at runtime.</para>
        /// </summary>
        public static string bundleIdentifier { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>A unique cloud project identifier. It is unique for every project (Read Only).</para>
        /// </summary>
        public static string cloudProjectId { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Return application company name (Read Only).</para>
        /// </summary>
        public static string companyName { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Contains the path to the game data folder (Read Only).</para>
        /// </summary>
        public static string dataPath { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns false if application is altered in any way after it was built.</para>
        /// </summary>
        public static bool genuine { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns true if application integrity can be confirmed.</para>
        /// </summary>
        public static bool genuineCheckAvailable { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns the name of the store or package that installed the application (Read Only).</para>
        /// </summary>
        public static string installerName { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns application install mode (Read Only).</para>
        /// </summary>
        public static ApplicationInstallMode installMode { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns the type of Internet reachability currently possible on the device.</para>
        /// </summary>
        public static NetworkReachability internetReachability { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        internal static bool isBatchmode { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Is the current Runtime platform a known console platform.</para>
        /// </summary>
        public static bool isConsolePlatform
        {
            get
            {
                RuntimePlatform platform = Application.platform;
                return ((platform == RuntimePlatform.PS4) || (platform == RuntimePlatform.XboxOne));
            }
        }

        /// <summary>
        /// <para>Are we running inside the Unity editor? (Read Only)</para>
        /// </summary>
        public static bool isEditor { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        internal static bool isHumanControllingUs { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Is some level being loaded? (Read Only)</para>
        /// </summary>
        [Obsolete("This property is deprecated, please use LoadLevelAsync to detect if a specific scene is currently loading.")]
        public static bool isLoadingLevel { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Is the current Runtime platform a known mobile platform.</para>
        /// </summary>
        public static bool isMobilePlatform
        {
            get
            {
                RuntimePlatform platform = Application.platform;
                switch (platform)
                {
                    case RuntimePlatform.MetroPlayerX86:
                    case RuntimePlatform.MetroPlayerX64:
                    case RuntimePlatform.MetroPlayerARM:
                    case RuntimePlatform.TizenPlayer:
                        break;

                    default:
                        switch (platform)
                        {
                            case RuntimePlatform.IPhonePlayer:
                            case RuntimePlatform.Android:
                                break;

                            case RuntimePlatform.PS3:
                            case RuntimePlatform.XBOX360:
                                goto Label_004C;

                            default:
                                goto Label_004C;
                        }
                        break;
                }
                return true;
            Label_004C:
                return false;
            }
        }

        [Obsolete("use Application.isEditor instead")]
        public static bool isPlayer =>
            !isEditor;

        /// <summary>
        /// <para>Returns true when in any kind of player (Read Only).</para>
        /// </summary>
        public static bool isPlaying { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Checks whether splash screen is being shown.</para>
        /// </summary>
        public static bool isShowingSplashScreen { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        internal static bool isTestRun { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Are we running inside a web player? (Read Only)</para>
        /// </summary>
        public static bool isWebPlayer { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The total number of levels available (Read Only).</para>
        /// </summary>
        [Obsolete("Use SceneManager.sceneCountInBuildSettings")]
        public static int levelCount =>
            SceneManager.sceneCountInBuildSettings;

        /// <summary>
        /// <para>The level index that was last loaded (Read Only).</para>
        /// </summary>
        [Obsolete("Use SceneManager to determine what scenes have been loaded")]
        public static int loadedLevel =>
            SceneManager.GetActiveScene().buildIndex;

        /// <summary>
        /// <para>The name of the level that was last loaded (Read Only).</para>
        /// </summary>
        [Obsolete("Use SceneManager to determine what scenes have been loaded")]
        public static string loadedLevelName =>
            SceneManager.GetActiveScene().name;

        /// <summary>
        /// <para>Contains the path to a persistent data directory (Read Only).</para>
        /// </summary>
        [SecurityCritical]
        public static string persistentDataPath { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns the platform the game is running on (Read Only).</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public static RuntimePlatform platform { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns application product name (Read Only).</para>
        /// </summary>
        public static string productName { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Should the player be running when the application is in the background?</para>
        /// </summary>
        public static bool runInBackground { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Returns application running in sandbox (Read Only).</para>
        /// </summary>
        public static ApplicationSandboxType sandboxType { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The path to the web player data file relative to the html file (Read Only).</para>
        /// </summary>
        public static string srcValue { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Stack trace logging options. The default value is StackTraceLogType.ScriptOnly.</para>
        /// </summary>
        [Obsolete("Use SetStackTraceLogType/GetStackTraceLogType instead")]
        public static StackTraceLogType stackTraceLogType { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>How many bytes have we downloaded from the main unity web stream (Read Only).</para>
        /// </summary>
        public static int streamedBytes { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Contains the path to the StreamingAssets folder (Read Only).</para>
        /// </summary>
        public static string streamingAssetsPath { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        internal static bool submitAnalytics { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The language the user's operating system is running in.</para>
        /// </summary>
        public static SystemLanguage systemLanguage { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Instructs game to try to render at a specified frame rate.</para>
        /// </summary>
        public static int targetFrameRate { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Contains the path to a temporary data / cache directory (Read Only).</para>
        /// </summary>
        public static string temporaryCachePath { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The version of the Unity runtime used to play the content.</para>
        /// </summary>
        public static string unityVersion { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns application version number  (Read Only).</para>
        /// </summary>
        public static string version { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Indicates whether Unity's webplayer security model is enabled.</para>
        /// </summary>
        [Obsolete("Application.webSecurityEnabled is no longer supported, since the Unity Web Player is no longer supported by Unity."), ThreadAndSerializationSafe]
        public static bool webSecurityEnabled { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        [Obsolete("Application.webSecurityHostUrl is no longer supported, since the Unity Web Player is no longer supported by Unity."), ThreadAndSerializationSafe]
        public static string webSecurityHostUrl { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Delegate method for fetching advertising ID.</para>
        /// </summary>
        /// <param name="advertisingId">Advertising ID.</param>
        /// <param name="trackingEnabled">Indicates whether user has chosen to limit ad tracking.</param>
        /// <param name="errorMsg">Error message.</param>
        public delegate void AdvertisingIdentifierCallback(string advertisingId, bool trackingEnabled, string errorMsg);

        /// <summary>
        /// <para>Use this delegate type with Application.logMessageReceived or Application.logMessageReceivedThreaded to monitor what gets logged.</para>
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        public delegate void LogCallback(string condition, string stackTrace, LogType type);
    }
}

