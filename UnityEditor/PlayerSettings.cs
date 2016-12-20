namespace UnityEditor
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using UnityEditor.Rendering;
    using UnityEngine;
    using UnityEngine.Internal;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Player Settings is where you define various parameters for the final game that you will build in Unity. Some of these values are used in the Resolution Dialog that launches when you open a standalone game.</para>
    /// </summary>
    public sealed class PlayerSettings : Object
    {
        private static SerializedObject _serializedObject;
        internal static readonly char[] defineSplits = new char[] { ';', ',', ' ', '\0' };

        internal static SerializedProperty FindProperty(string name)
        {
            SerializedProperty property = GetSerializedObject().FindProperty(name);
            if (property == null)
            {
                Debug.LogError("Failed to find:" + name);
            }
            return property;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GetAdditionalIl2CppArgs();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int GetArchitecture(BuildTargetGroup targetGroup);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void GetBatchingForPlatform(BuildTarget platform, out int staticBatching, out int dynamicBatching);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool GetCloudServiceEnabled(string serviceKey);
        /// <summary>
        /// <para>Get graphics APIs to be used on a build platform.</para>
        /// </summary>
        /// <param name="platform">Platform to get APIs for.</param>
        /// <returns>
        /// <para>Array of graphics APIs.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern GraphicsDeviceType[] GetGraphicsAPIs(BuildTarget platform);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern Texture2D GetIconForPlatformAtSize(string platform, int width, int height);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int[] GetIconHeightsForPlatform(string platform);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern Texture2D[] GetIconsForPlatform(string platform);
        /// <summary>
        /// <para>Returns the list of assigned icons for the specified platform.</para>
        /// </summary>
        /// <param name="platform"></param>
        public static Texture2D[] GetIconsForTargetGroup(BuildTargetGroup platform)
        {
            Texture2D[] iconsForPlatform = GetIconsForPlatform(GetPlatformName(platform));
            if (iconsForPlatform.Length == 0)
            {
                return new Texture2D[GetIconSizesForTargetGroup(platform).Length];
            }
            return iconsForPlatform;
        }

        /// <summary>
        /// <para>Returns a list of icon sizes for the specified platform.</para>
        /// </summary>
        /// <param name="platform"></param>
        public static int[] GetIconSizesForTargetGroup(BuildTargetGroup platform)
        {
            return GetIconWidthsForPlatform(GetPlatformName(platform));
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int[] GetIconWidthsForPlatform(string platform);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool GetIncrementalIl2CppBuild(BuildTargetGroup targetGroup);
        internal static string GetPlatformName(BuildTargetGroup targetGroup)
        {
            <GetPlatformName>c__AnonStorey0 storey = new <GetPlatformName>c__AnonStorey0 {
                targetGroup = targetGroup
            };
            BuildPlayerWindow.BuildPlatform platform = BuildPlayerWindow.GetValidPlatforms().Find(new Predicate<BuildPlayerWindow.BuildPlatform>(storey.<>m__0));
            return ((platform != null) ? platform.name : string.Empty);
        }

        [ExcludeFromDocs, Obsolete("Use explicit API instead.")]
        public static bool GetPropertyBool(string name)
        {
            BuildTargetGroup unknown = BuildTargetGroup.Unknown;
            return GetPropertyBool(name, unknown);
        }

        /// <summary>
        /// <para>Returns a PlayerSettings named bool property (with an optional build target it should apply to).</para>
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="target">BuildTarget for which the property should apply (use default value BuildTargetGroup.Unknown to apply to all targets).</param>
        /// <returns>
        /// <para>The current value of the property.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("Use explicit API instead.")]
        public static extern bool GetPropertyBool(string name, [DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target);
        [ExcludeFromDocs, Obsolete("Use explicit API instead.")]
        public static int GetPropertyInt(string name)
        {
            BuildTargetGroup unknown = BuildTargetGroup.Unknown;
            return GetPropertyInt(name, unknown);
        }

        /// <summary>
        /// <para>Returns a PlayerSettings named int property (with an optional build target it should apply to).</para>
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="target">BuildTarget for which the property should apply (use default value BuildTargetGroup.Unknown to apply to all targets).</param>
        /// <returns>
        /// <para>The current value of the property.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("Use explicit API instead.")]
        public static extern int GetPropertyInt(string name, [DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target);
        [ExcludeFromDocs, Obsolete("Use explicit API instead.")]
        public static bool GetPropertyOptionalBool(string name, ref bool value)
        {
            BuildTargetGroup unknown = BuildTargetGroup.Unknown;
            return GetPropertyOptionalBool(name, ref value, unknown);
        }

        [Obsolete("Use explicit API instead.")]
        public static bool GetPropertyOptionalBool(string name, ref bool value, [DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
        {
            value = GetPropertyBool(name, target);
            return true;
        }

        [ExcludeFromDocs, Obsolete("Use explicit API instead.")]
        public static bool GetPropertyOptionalInt(string name, ref int value)
        {
            BuildTargetGroup unknown = BuildTargetGroup.Unknown;
            return GetPropertyOptionalInt(name, ref value, unknown);
        }

        [Obsolete("Use explicit API instead.")]
        public static bool GetPropertyOptionalInt(string name, ref int value, [DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
        {
            value = GetPropertyInt(name, target);
            return true;
        }

        [ExcludeFromDocs, Obsolete("Use explicit API instead.")]
        public static bool GetPropertyOptionalString(string name, ref string value)
        {
            BuildTargetGroup unknown = BuildTargetGroup.Unknown;
            return GetPropertyOptionalString(name, ref value, unknown);
        }

        [Obsolete("Use explicit API instead.")]
        public static bool GetPropertyOptionalString(string name, ref string value, [DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
        {
            value = GetPropertyString(name, target);
            return true;
        }

        [ExcludeFromDocs, Obsolete("Use explicit API instead.")]
        public static string GetPropertyString(string name)
        {
            BuildTargetGroup unknown = BuildTargetGroup.Unknown;
            return GetPropertyString(name, unknown);
        }

        /// <summary>
        /// <para>Returns a PlayerSettings named string property (with an optional build target it should apply to).</para>
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="target">BuildTarget for which the property should apply (use default value BuildTargetGroup.Unknown to apply to all targets).</param>
        /// <returns>
        /// <para>The current value of the property.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("Use explicit API instead.")]
        public static extern string GetPropertyString(string name, [DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern ScriptingImplementation GetScriptingBackend(BuildTargetGroup targetGroup);
        /// <summary>
        /// <para>Get user-specified symbols for script compilation for the given build target group.</para>
        /// </summary>
        /// <param name="targetGroup"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GetScriptingDefineSymbolsForGroup(BuildTargetGroup targetGroup);
        internal static SerializedObject GetSerializedObject()
        {
            if (_serializedObject == null)
            {
                _serializedObject = new SerializedObject(InternalGetPlayerSettingsObject());
            }
            return _serializedObject;
        }

        /// <summary>
        /// <para>Get stack trace logging options.</para>
        /// </summary>
        /// <param name="logType"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern StackTraceLogType GetStackTraceLogType(LogType logType);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern GraphicsDeviceType[] GetSupportedGraphicsAPIs(BuildTarget platform);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string GetTemplateCustomValue(string key);
        /// <summary>
        /// <para>Is a build platform using automatic graphics API choice?</para>
        /// </summary>
        /// <param name="platform">Platform to get the flag for.</param>
        /// <returns>
        /// <para>Should best available graphics API be used.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool GetUseDefaultGraphicsAPIs(BuildTarget platform);
        /// <summary>
        /// <para>Returns whether or not the specified aspect ratio is enabled.</para>
        /// </summary>
        /// <param name="aspectRatio"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool HasAspectRatio(AspectRatio aspectRatio);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_get_cursorHotspot(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_set_cursorHotspot(ref Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Object InternalGetPlayerSettingsObject();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetAdditionalIl2CppArgs(string additionalArgs);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetApiCompatibilityInternal(int value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetArchitecture(BuildTargetGroup targetGroup, int architecture);
        /// <summary>
        /// <para>Enables the specified aspect ratio.</para>
        /// </summary>
        /// <param name="aspectRatio"></param>
        /// <param name="enable"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetAspectRatio(AspectRatio aspectRatio, bool enable);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetBatchingForPlatform(BuildTarget platform, int staticBatching, int dynamicBatching);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetCloudServiceEnabled(string serviceKey, bool enabled);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetDirty();
        /// <summary>
        /// <para>Set graphics APIs to be used on a build platform.</para>
        /// </summary>
        /// <param name="platform">Platform to set APIs for.</param>
        /// <param name="apis">Array of graphics APIs.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetGraphicsAPIs(BuildTarget platform, GraphicsDeviceType[] apis);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetIconsForPlatform(string platform, Texture2D[] icons);
        /// <summary>
        /// <para>Assign a list of icons for the specified platform.</para>
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="icons"></param>
        public static void SetIconsForTargetGroup(BuildTargetGroup platform, Texture2D[] icons)
        {
            SetIconsForPlatform(GetPlatformName(platform), icons);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetIncrementalIl2CppBuild(BuildTargetGroup targetGroup, bool enabled);
        [Obsolete("Use explicit API instead."), ExcludeFromDocs]
        public static void SetPropertyBool(string name, bool value)
        {
            BuildTargetGroup unknown = BuildTargetGroup.Unknown;
            SetPropertyBool(name, value, unknown);
        }

        [Obsolete("Use explicit API instead.")]
        public static void SetPropertyBool(string name, bool value, BuildTarget target)
        {
            SetPropertyBool(name, value, BuildPipeline.GetBuildTargetGroup(target));
        }

        /// <summary>
        /// <para>Sets a PlayerSettings named bool property.</para>
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="value">Value of the property (bool).</param>
        /// <param name="target">BuildTarget for which the property should apply (use default value BuildTargetGroup.Unknown to apply to all targets).</param>
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("Use explicit API instead.")]
        public static extern void SetPropertyBool(string name, bool value, [DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target);
        [Obsolete("Use explicit API instead."), ExcludeFromDocs]
        public static void SetPropertyInt(string name, int value)
        {
            BuildTargetGroup unknown = BuildTargetGroup.Unknown;
            SetPropertyInt(name, value, unknown);
        }

        [Obsolete("Use explicit API instead.")]
        public static void SetPropertyInt(string name, int value, BuildTarget target)
        {
            SetPropertyInt(name, value, BuildPipeline.GetBuildTargetGroup(target));
        }

        /// <summary>
        /// <para>Sets a PlayerSettings named int property.</para>
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="value">Value of the property (int).</param>
        /// <param name="target">BuildTarget for which the property should apply (use default value BuildTargetGroup.Unknown to apply to all targets).</param>
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("Use explicit API instead.")]
        public static extern void SetPropertyInt(string name, int value, [DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target);
        [ExcludeFromDocs, Obsolete("Use explicit API instead.")]
        public static void SetPropertyString(string name, string value)
        {
            BuildTargetGroup unknown = BuildTargetGroup.Unknown;
            SetPropertyString(name, value, unknown);
        }

        [Obsolete("Use explicit API instead.")]
        public static void SetPropertyString(string name, string value, BuildTarget target)
        {
            SetPropertyString(name, value, BuildPipeline.GetBuildTargetGroup(target));
        }

        /// <summary>
        /// <para>Sets a PlayerSettings named string property.</para>
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="value">Value of the property (string).</param>
        /// <param name="target">BuildTarget for which the property should apply (use default value BuildTargetGroup.Unknown to apply to all targets).</param>
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("Use explicit API instead.")]
        public static extern void SetPropertyString(string name, string value, [DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetScriptingBackend(BuildTargetGroup targetGroup, ScriptingImplementation backend);
        /// <summary>
        /// <para>Set user-specified symbols for script compilation for the given build target group.</para>
        /// </summary>
        /// <param name="targetGroup"></param>
        /// <param name="defines"></param>
        public static void SetScriptingDefineSymbolsForGroup(BuildTargetGroup targetGroup, string defines)
        {
            if (!string.IsNullOrEmpty(defines))
            {
                defines = string.Join(";", defines.Split(defineSplits, StringSplitOptions.RemoveEmptyEntries));
            }
            SetScriptingDefineSymbolsForGroupInternal(targetGroup, defines);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetScriptingDefineSymbolsForGroupInternal(BuildTargetGroup targetGroup, string defines);
        /// <summary>
        /// <para>Set stack trace logging options. 
        /// Note: calling this function will implicitly call Application.SetStackTraceLogType.</para>
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="stackTraceType"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetStackTraceLogType(LogType logType, StackTraceLogType stackTraceType);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetTemplateCustomValue(string key, string value);
        /// <summary>
        /// <para>Should a build platform use automatic graphics API choice.</para>
        /// </summary>
        /// <param name="platform">Platform to set the flag for.</param>
        /// <param name="automatic">Should best available graphics API be used?</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetUseDefaultGraphicsAPIs(BuildTarget platform, bool automatic);

        /// <summary>
        /// <para>Accelerometer update frequency.</para>
        /// </summary>
        public static int accelerometerFrequency { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Sets the crash behavior on .NET unhandled exception.</para>
        /// </summary>
        public static ActionOnDotNetUnhandledException actionOnDotNetUnhandledException { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Is the advanced version being used?</para>
        /// </summary>
        public static bool advancedLicense { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Is auto-rotation to landscape left supported?</para>
        /// </summary>
        public static bool allowedAutorotateToLandscapeLeft { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Is auto-rotation to landscape right supported?</para>
        /// </summary>
        public static bool allowedAutorotateToLandscapeRight { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Is auto-rotation to portrait supported?</para>
        /// </summary>
        public static bool allowedAutorotateToPortrait { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Is auto-rotation to portrait upside-down supported?</para>
        /// </summary>
        public static bool allowedAutorotateToPortraitUpsideDown { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>If enabled, allows the user to switch between full screen and windowed mode using OS specific keyboard short cuts.</para>
        /// </summary>
        public static bool allowFullscreenSwitch { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [Obsolete("The option alwaysDisplayWatermark is deprecated and is always false", true)]
        public static bool alwaysDisplayWatermark
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        /// <summary>
        /// <para>Additional AOT compilation options. Shared by AOT platforms.</para>
        /// </summary>
        public static string aotOptions { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>.NET API compatibility level.</para>
        /// </summary>
        public static ApiCompatibilityLevel apiCompatibilityLevel { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Pre bake collision meshes on player build.</para>
        /// </summary>
        public static bool bakeCollisionMeshes { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Application bundle identifier shared between iOS &amp; Android platforms.</para>
        /// </summary>
        public static string bundleIdentifier { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Application bundle version shared between iOS &amp; Android platforms.</para>
        /// </summary>
        public static string bundleVersion { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Defines if fullscreen games should darken secondary displays.</para>
        /// </summary>
        public static bool captureSingleScreen { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>A unique cloud project identifier. It is unique for every project (Read Only).</para>
        /// </summary>
        public static string cloudProjectId
        {
            get
            {
                return cloudProjectIdRaw;
            }
            internal set
            {
                cloudProjectIdRaw = value;
            }
        }

        private static string cloudProjectIdRaw { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Set the rendering color space for the current project.</para>
        /// </summary>
        public static ColorSpace colorSpace { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The name of your company.</para>
        /// </summary>
        public static string companyName { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Default cursor's click position in pixels from the top left corner of the cursor image.</para>
        /// </summary>
        public static Vector2 cursorHotspot
        {
            get
            {
                Vector2 vector;
                INTERNAL_get_cursorHotspot(out vector);
                return vector;
            }
            set
            {
                INTERNAL_set_cursorHotspot(ref value);
            }
        }

        /// <summary>
        /// <para>Define how to handle fullscreen mode in Windows standalones (Direct3D 11 mode).</para>
        /// </summary>
        public static D3D11FullscreenMode d3d11FullscreenMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Define how to handle fullscreen mode in Windows standalones (Direct3D 9 mode).</para>
        /// </summary>
        public static D3D9FullscreenMode d3d9FullscreenMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The default cursor for your application.</para>
        /// </summary>
        public static Texture2D defaultCursor { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Default screen orientation for mobiles.</para>
        /// </summary>
        public static UIOrientation defaultInterfaceOrientation { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>If enabled, the game will default to fullscreen mode.</para>
        /// </summary>
        public static bool defaultIsFullScreen { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        public static bool defaultIsNativeResolution { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Default vertical dimension of stand-alone player window.</para>
        /// </summary>
        public static int defaultScreenHeight { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Default horizontal dimension of stand-alone player window.</para>
        /// </summary>
        public static int defaultScreenWidth { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Default vertical dimension of web player window.</para>
        /// </summary>
        public static int defaultWebScreenHeight { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Default horizontal dimension of web player window.</para>
        /// </summary>
        public static int defaultWebScreenWidth { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Defines the behaviour of the Resolution Dialog on product launch.</para>
        /// </summary>
        public static ResolutionDialogSetting displayResolutionDialog { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Enables CrashReport API.</para>
        /// </summary>
        public static bool enableCrashReportAPI { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Enables internal profiler.</para>
        /// </summary>
        public static bool enableInternalProfiler { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>First level to have access to all Resources.Load assets in Streamed Web Players.</para>
        /// </summary>
        [Obsolete("Use AssetBundles instead for streaming data", true)]
        public static int firstStreamedLevelWithResources
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        /// <summary>
        /// <para>Restrict standalone players to a single concurrent running instance.</para>
        /// </summary>
        public static bool forceSingleInstance { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Enable GPU skinning on capable platforms.</para>
        /// </summary>
        public static bool gpuSkinning { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Enable graphics jobs (multi threaded rendering).</para>
        /// </summary>
        public static bool graphicsJobs { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The bundle identifier of the iPhone application.</para>
        /// </summary>
        public static string iPhoneBundleIdentifier { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Password for the key used for signing an Android application.</para>
        /// </summary>
        public static string keyaliasPass { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Password used for interacting with the Android Keystore.</para>
        /// </summary>
        public static string keystorePass { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Describes the reason for access to the user's location data.</para>
        /// </summary>
        [Obsolete("Use PlayerSettings.iOS.locationUsageDescription instead (UnityUpgradable) -> UnityEditor.PlayerSettings/iOS.locationUsageDescription", false)]
        public static string locationUsageDescription
        {
            get
            {
                return iOS.locationUsageDescription;
            }
            set
            {
                iOS.locationUsageDescription = value;
            }
        }

        /// <summary>
        /// <para>Are ObjC uncaught exceptions logged?</para>
        /// </summary>
        public static bool logObjCUncaughtExceptions { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Define how to handle fullscreen mode in macOS standalones.</para>
        /// </summary>
        public static MacFullscreenMode macFullscreenMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        public static bool mobileMTRendering { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [Obsolete("mobileRenderingPath is ignored, use UnityEditor.Rendering.TierSettings with UnityEditor.Rendering.SetTierSettings/GetTierSettings instead", false)]
        public static RenderingPath mobileRenderingPath
        {
            get
            {
                return EditorGraphicsSettings.GetCurrentTierSettings().renderingPath;
            }
            set
            {
            }
        }

        /// <summary>
        /// <para>Is multi-threaded rendering enabled?</para>
        /// </summary>
        public static bool MTRendering { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Mute or allow audio from other applications to play in the background while the Unity application is running.</para>
        /// </summary>
        public static bool muteOtherAudioSources { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        public static bool openGLRequireES31 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        public static bool openGLRequireES31AEP { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        public static Guid productGUID
        {
            get
            {
                return new Guid(productGUIDRaw);
            }
        }

        private static byte[] productGUIDRaw { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The name of your product.</para>
        /// </summary>
        public static string productName { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Protect graphics memory.</para>
        /// </summary>
        public static bool protectGraphicsMemory { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Which rendering path is enabled?</para>
        /// </summary>
        [Obsolete("renderingPath is ignored, use UnityEditor.Rendering.TierSettings with UnityEditor.Rendering.SetTierSettings/GetTierSettings instead", false)]
        public static RenderingPath renderingPath
        {
            get
            {
                return EditorGraphicsSettings.GetCurrentTierSettings().renderingPath;
            }
            set
            {
            }
        }

        /// <summary>
        /// <para>Use resizable window in standalone player builds.</para>
        /// </summary>
        public static bool resizableWindow { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The image to display in the Resolution Dialog window.</para>
        /// </summary>
        public static Texture2D resolutionDialogBanner { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>If enabled, your game will continue to run after lost focus.</para>
        /// </summary>
        public static bool runInBackground { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should the builtin Unity splash screen be shown?</para>
        /// </summary>
        [Obsolete("Use PlayerSettings.SplashScreen.show instead")]
        public static bool showUnitySplashScreen { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should Unity support single-pass stereo rendering?</para>
        /// </summary>
        [Obsolete("singlePassStereoRendering will be deprecated. Use stereoRenderingPath instead.")]
        public static bool singlePassStereoRendering { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The style to use for the builtin Unity splash screen.</para>
        /// </summary>
        [Obsolete("Use PlayerSettings.SplashScreen.unityLogoStyle instead")]
        public static SplashScreenStyle splashScreenStyle { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal static string spritePackerPolicy { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should status bar be hidden. Shared between iOS &amp; Android platforms.</para>
        /// </summary>
        public static bool statusBarHidden { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Active stereo rendering path</para>
        /// </summary>
        public static StereoRenderingPath stereoRenderingPath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should player render in stereoscopic 3d on supported hardware?</para>
        /// </summary>
        [Obsolete("Use VREditor.GetStereoDeviceEnabled instead")]
        public static bool stereoscopic3D { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Remove unused Engine code from your build (IL2CPP-only).</para>
        /// </summary>
        public static bool stripEngineCode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Managed code stripping level.</para>
        /// </summary>
        public static StrippingLevel strippingLevel { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should unused Mesh components be excluded from game build?</para>
        /// </summary>
        public static bool stripUnusedMeshComponents { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal static bool submitAnalytics { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [Obsolete("targetGlesGraphics is ignored, use SetGraphicsAPIs/GetGraphicsAPIs APIs", false)]
        public static TargetGlesGraphics targetGlesGraphics
        {
            get
            {
                return TargetGlesGraphics.Automatic;
            }
            set
            {
            }
        }

        [Obsolete("targetIOSGraphics is ignored, use SetGraphicsAPIs/GetGraphicsAPIs APIs", false)]
        public static TargetIOSGraphics targetIOSGraphics
        {
            get
            {
                return TargetIOSGraphics.Automatic;
            }
            set
            {
            }
        }

        internal static string[] templateCustomKeys { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>32-bit Display Buffer is used.</para>
        /// </summary>
        public static bool use32BitDisplayBuffer { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Let the OS autorotate the screen as the device orientation changes.</para>
        /// </summary>
        public static bool useAnimatedAutorotation { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should Direct3D 11 be used when available?</para>
        /// </summary>
        [Obsolete("Use UnityEditor.PlayerSettings.SetGraphicsAPIs/GetGraphicsAPIs instead")]
        public static bool useDirect3D11 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Enable receipt validation for the Mac App Store.</para>
        /// </summary>
        public static bool useMacAppStoreValidation { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Write a log file with debugging information.</para>
        /// </summary>
        public static bool usePlayerLog { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Virtual Reality specific splash screen.</para>
        /// </summary>
        public static Texture2D virtualRealitySplashScreen { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Enable virtual reality support.</para>
        /// </summary>
        public static bool virtualRealitySupported { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>On Windows, show the application in the background if Fullscreen Windowed mode is used.</para>
        /// </summary>
        public static bool visibleInBackground { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal static string webPlayerTemplate { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        public static int xboxAdditionalTitleMemorySize { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Xbox 360 Kinect Head Orientation file deployment.</para>
        /// </summary>
        public static bool xboxDeployKinectHeadOrientation { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Xbox 360 Kinect Head Position file deployment.</para>
        /// </summary>
        public static bool xboxDeployKinectHeadPosition { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Xbox 360 Kinect resource file deployment.</para>
        /// </summary>
        public static bool xboxDeployKinectResources { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Xbox 360 Avatars.</para>
        /// </summary>
        public static bool xboxEnableAvatar { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public static bool xboxEnableGuest { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Xbox 360 Kinect title flag - if false, the Kinect APIs are inactive.</para>
        /// </summary>
        public static bool xboxEnableKinect { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Xbox 360 Kinect automatic skeleton tracking.</para>
        /// </summary>
        public static bool xboxEnableKinectAutoTracking { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Xbox 360 Kinect Enable Speech Engine.</para>
        /// </summary>
        public static bool xboxEnableSpeech { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Xbox 360 auto-generation of _SPAConfig.cs.</para>
        /// </summary>
        public static bool xboxGenerateSpa { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Xbox 360 ImageXex override configuration file path.</para>
        /// </summary>
        public static string xboxImageXexFilePath { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public static int xboxOneResolution { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public static bool xboxPIXTextureCapture { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Xbox 360 SPA file path.</para>
        /// </summary>
        public static string xboxSpaFilePath { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Xbox 360 Kinect Speech DB.</para>
        /// </summary>
        public static uint xboxSpeechDB { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Xbox 360 splash screen.</para>
        /// </summary>
        public static Texture2D xboxSplashScreen { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Xbox 360 title id.</para>
        /// </summary>
        public static string xboxTitleId { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [CompilerGenerated]
        private sealed class <GetPlatformName>c__AnonStorey0
        {
            internal BuildTargetGroup targetGroup;

            internal bool <>m__0(BuildPlayerWindow.BuildPlatform p)
            {
                return (p.targetGroup == this.targetGroup);
            }
        }

        /// <summary>
        /// <para>Android specific player settings.</para>
        /// </summary>
        public sealed class Android
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            internal static extern Texture2D GetAndroidBannerForHeight(int height);
            [MethodImpl(MethodImplOptions.InternalCall)]
            internal static extern AndroidBanner[] GetAndroidBanners();
            [MethodImpl(MethodImplOptions.InternalCall)]
            internal static extern void SetAndroidBanners(Texture2D[] banners);

            internal static bool androidBannerEnabled { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            internal static AndroidGamepadSupportLevel androidGamepadSupportLevel { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Publish the build as a game rather than a regular application. This option affects devices running Android 5.0 Lollipop and later</para>
            /// </summary>
            public static bool androidIsGame { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Provide a build that is Android TV compatible.</para>
            /// </summary>
            public static bool androidTVCompatibility { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Android bundle version code.</para>
            /// </summary>
            public static int bundleVersionCode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            internal static bool createWallpaper { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Disable Depth and Stencil Buffers.</para>
            /// </summary>
            public static bool disableDepthAndStencilBuffers { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Force internet permission flag.</para>
            /// </summary>
            public static bool forceInternetPermission { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Force SD card permission.</para>
            /// </summary>
            public static bool forceSDCardPermission { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Android key alias name.</para>
            /// </summary>
            public static string keyaliasName { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Android key alias password.</para>
            /// </summary>
            public static string keyaliasPass { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Android keystore name.</para>
            /// </summary>
            public static string keystoreName { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Android keystore password.</para>
            /// </summary>
            public static string keystorePass { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>License verification flag.</para>
            /// </summary>
            public static bool licenseVerification { [MethodImpl(MethodImplOptions.InternalCall)] get; }

            /// <summary>
            /// <para>Minimal Android SDK version.</para>
            /// </summary>
            public static AndroidSdkVersions minSdkVersion { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Preferred application install location.</para>
            /// </summary>
            public static AndroidPreferredInstallLocation preferredInstallLocation { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Application should show ActivityIndicator when loading.</para>
            /// </summary>
            public static AndroidShowActivityIndicatorOnLoading showActivityIndicatorOnLoading { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Android splash screen scale mode.</para>
            /// </summary>
            public static AndroidSplashScreenScale splashScreenScale { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Android target device.</para>
            /// </summary>
            public static AndroidTargetDevice targetDevice { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>24-bit Depth Buffer is used.</para>
            /// </summary>
            [Obsolete("This has been replaced by disableDepthAndStencilBuffers")]
            public static bool use24BitDepthBuffer
            {
                get
                {
                    return !disableDepthAndStencilBuffers;
                }
                set
                {
                }
            }

            /// <summary>
            /// <para>Use APK Expansion Files.</para>
            /// </summary>
            public static bool useAPKExpansionFiles { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        }

        /// <summary>
        /// <para>iOS specific player settings.</para>
        /// </summary>
        public sealed class iOS
        {
            internal static iOSDeviceRequirementGroup AddDeviceRequirementsForAssetBundleVariant(string name)
            {
                return new iOSDeviceRequirementGroup(name);
            }

            [MethodImpl(MethodImplOptions.InternalCall)]
            private static extern bool CheckAssetBundleVariantHasDeviceRequirements(string name);
            [MethodImpl(MethodImplOptions.InternalCall)]
            internal static extern string[] GetAssetBundleVariantsWithDeviceRequirements();
            internal static iOSDeviceRequirementGroup GetDeviceRequirementsForAssetBundleVariant(string name)
            {
                if (!CheckAssetBundleVariantHasDeviceRequirements(name))
                {
                    return null;
                }
                return new iOSDeviceRequirementGroup(name);
            }

            [MethodImpl(MethodImplOptions.InternalCall)]
            internal static extern string[] GetURLSchemes();
            internal static void RemoveDeviceRequirementsForAssetBundleVariant(string name)
            {
                iOSDeviceRequirementGroup deviceRequirementsForAssetBundleVariant = GetDeviceRequirementsForAssetBundleVariant(name);
                for (int i = 0; i < deviceRequirementsForAssetBundleVariant.count; i++)
                {
                    deviceRequirementsForAssetBundleVariant.RemoveAt(0);
                }
            }

            /// <summary>
            /// <para>Should insecure HTTP downloads be allowed?</para>
            /// </summary>
            public static bool allowHTTPDownload { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Application behavior when entering background.</para>
            /// </summary>
            public static iOSAppInBackgroundBehavior appInBackgroundBehavior { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Set this property with your Apple Developer Team ID. You can find this on the Apple Developer website under &lt;a href="https:developer.apple.comaccount#membership"&gt; Account &gt; Membership &lt;/a&gt; . This sets the Team ID for the generated Xcode project, allowing developers to use the Build and Run functionality. An Apple Developer Team ID must be set here for automatic signing of your app.</para>
            /// </summary>
            public static string appleDeveloperTeamID { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Set this property to true for Xcode to attempt to automatically sign your app based on your appleDeveloperTeamID.</para>
            /// </summary>
            public static bool appleEnableAutomaticSigning { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>iOS application display name.</para>
            /// </summary>
            public static string applicationDisplayName { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Supported background execution modes (when appInBackgroundBehavior is set to iOSAppInBackgroundBehavior.Custom).</para>
            /// </summary>
            public static iOSBackgroundMode backgroundModes { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>The build number of the bundle.</para>
            /// </summary>
            public static string buildNumber { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Describes the reason for access to the user's camera.</para>
            /// </summary>
            public static string cameraUsageDescription { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Application should exit when suspended to background.</para>
            /// </summary>
            [Obsolete("exitOnSuspend is deprecated, use appInBackgroundBehavior", false)]
            public static bool exitOnSuspend
            {
                get
                {
                    return (appInBackgroundBehavior == iOSAppInBackgroundBehavior.Exit);
                }
                set
                {
                    appInBackgroundBehavior = iOSAppInBackgroundBehavior.Exit;
                }
            }

            /// <summary>
            /// <para>Should hard shadows be enforced when running on (mobile) Metal.</para>
            /// </summary>
            public static bool forceHardShadowsOnMetal { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>A provisioning profile Universally Unique Identifier (UUID) that Xcode will use to build your iOS app in Manual Signing mode.</para>
            /// </summary>
            public static string iOSManualProvisioningProfileID { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Describes the reason for access to the user's location data.</para>
            /// </summary>
            public static string locationUsageDescription { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Describes the reason for access to the user's microphone.</para>
            /// </summary>
            public static string microphoneUsageDescription { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Determines iPod playing behavior.</para>
            /// </summary>
            [Obsolete("Use PlayerSettings.muteOtherAudioSources instead (UnityUpgradable) -> UnityEditor.PlayerSettings.muteOtherAudioSources", false)]
            public static bool overrideIPodMusic
            {
                get
                {
                    return PlayerSettings.muteOtherAudioSources;
                }
                set
                {
                    PlayerSettings.muteOtherAudioSources = value;
                }
            }

            /// <summary>
            /// <para>Icon is prerendered.</para>
            /// </summary>
            public static bool prerenderedIcon { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>RequiresFullScreen maps to Apple's plist build setting UIRequiresFullScreen, which is used to opt out of being eligible to participate in Slide Over and Split View for iOS 9.0 multitasking.</para>
            /// </summary>
            public static bool requiresFullScreen { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Application requires persistent WiFi.</para>
            /// </summary>
            public static bool requiresPersistentWiFi { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Script calling optimization.</para>
            /// </summary>
            public static ScriptCallOptimizationLevel scriptCallOptimization { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Active iOS SDK version used for build.</para>
            /// </summary>
            public static iOSSdkVersion sdkVersion { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Application should show ActivityIndicator when loading.</para>
            /// </summary>
            public static iOSShowActivityIndicatorOnLoading showActivityIndicatorOnLoading { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Status bar style.</para>
            /// </summary>
            public static iOSStatusBarStyle statusBarStyle { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Targeted device.</para>
            /// </summary>
            public static iOSTargetDevice targetDevice { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Deployment minimal version of iOS.</para>
            /// </summary>
            [Obsolete("targetOSVersion is obsolete, use targetOSVersionString")]
            public static iOSTargetOSVersion targetOSVersion { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Deployment minimal version of iOS.</para>
            /// </summary>
            public static string targetOSVersionString { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use Screen.SetResolution at runtime", true)]
            public static iOSTargetResolution targetResolution
            {
                get
                {
                    return iOSTargetResolution.Native;
                }
                set
                {
                }
            }

            /// <summary>
            /// <para>A provisioning profile Universally Unique Identifier (UUID) that Xcode will use to build your tvOS app in Manual Signing mode.</para>
            /// </summary>
            public static string tvOSManualProvisioningProfileID { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Indicates whether application will use On Demand Resources (ODR) API.</para>
            /// </summary>
            public static bool useOnDemandResources { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        }

        /// <summary>
        /// <para>Nintendo 3DS player settings.</para>
        /// </summary>
        public sealed class N3DS
        {
            /// <summary>
            /// <para>The unique ID of the application, issued by Nintendo.  (0x00300 -&gt; 0xf7fff)</para>
            /// </summary>
            public static string applicationId { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Specify true to enable static memory compression or false to disable it.</para>
            /// </summary>
            public static bool compressStaticMem { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Disable depth/stencil buffers, to free up memory.</para>
            /// </summary>
            public static bool disableDepthAndStencilBuffers { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Disable sterescopic (3D) view on the upper screen.</para>
            /// </summary>
            public static bool disableStereoscopicView { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Enable shared L/R command list, for increased performance with stereoscopic rendering.</para>
            /// </summary>
            public static bool enableSharedListOpt { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Enable vsync.</para>
            /// </summary>
            public static bool enableVSync { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Specify the expanded save data number using 20 bits.</para>
            /// </summary>
            public static string extSaveDataNumber { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Application Logo Style.</para>
            /// </summary>
            public static LogoStyle logoStyle { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Distribution media size.</para>
            /// </summary>
            public static MediaSize mediaSize { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Specifies the product code, or the add-on content code.</para>
            /// </summary>
            public static string productCode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Specifies the title region settings.</para>
            /// </summary>
            public static Region region { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Specify the stack size of the main thread, in bytes.</para>
            /// </summary>
            public static int stackSize { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>The 3DS target platform.</para>
            /// </summary>
            public static TargetPlatform targetPlatform { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>The title of the application.</para>
            /// </summary>
            public static string title { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Specify true when using expanded save data.</para>
            /// </summary>
            public static bool useExtSaveData { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Nintendo 3DS logo style specification.</para>
            /// </summary>
            public enum LogoStyle
            {
                Nintendo,
                Distributed,
                iQue,
                Licensed
            }

            /// <summary>
            /// <para>Nintendo 3DS distribution media size.</para>
            /// </summary>
            public enum MediaSize
            {
                _128MB,
                _256MB,
                _512MB,
                _1GB,
                _2GB
            }

            /// <summary>
            /// <para>Nintendo 3DS Title region.</para>
            /// </summary>
            public enum Region
            {
                /// <summary>
                /// <para>For all regions.</para>
                /// </summary>
                All = 7,
                /// <summary>
                /// <para>For the American region.</para>
                /// </summary>
                America = 2,
                /// <summary>
                /// <para>For the Chinese region.</para>
                /// </summary>
                China = 4,
                /// <summary>
                /// <para>For the European region.</para>
                /// </summary>
                Europe = 3,
                /// <summary>
                /// <para>For the Japanese region.</para>
                /// </summary>
                Japan = 1,
                /// <summary>
                /// <para>For the Korean region.</para>
                /// </summary>
                Korea = 5,
                /// <summary>
                /// <para>For the Taiwanese region.</para>
                /// </summary>
                Taiwan = 6
            }

            /// <summary>
            /// <para>Nintendo 3DS target platform.</para>
            /// </summary>
            public enum TargetPlatform
            {
                /// <summary>
                /// <para>Target the New Nintendo 3DS platform.</para>
                /// </summary>
                NewNintendo3DS = 2,
                /// <summary>
                /// <para>Target the Nintendo 3DS platform.</para>
                /// </summary>
                Nintendo3DS = 1
            }
        }

        public sealed class PS4
        {
            public static int applicationParameter1 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static int applicationParameter2 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static int applicationParameter3 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static int applicationParameter4 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static int appType { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string appVersion { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool attrib3DSupport { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static int attribCpuUsage { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool attribExclusiveVR { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static PlayStationVREyeToEyeDistanceSettings attribEyeToEyeDistanceSettingVR { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool attribMoveSupport { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool attribShareSupport { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool attribUserManagement { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static int audio3dVirtualSpeakerCount { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string BackgroundImagePath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string BGMPath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static PS4AppCategory category { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string contentID { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool contentSearchFeaturesUsed { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool disableAutoHideSplash { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static int downloadDataSize { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static PS4EnterButtonAssignment enterButtonAssignment { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static int garlicHeapSize { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string[] includedModules { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string masterVersion { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string monoEnv { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static int npAgeRating { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string NPtitleDatPath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string npTitleSecret { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string npTrophyPackPath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string paramSfxPath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static int parentalLevel { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string passcode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string PatchChangeinfoPath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool patchDayOne { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string PatchLatestPkgPath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string PatchPkgPath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool playerPrefsSupport { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static int playTogetherPlayerCount { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool pnFriends { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool pnGameCustomData { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool pnPresence { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool pnSessions { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string PrivacyGuardImagePath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static int proGarlicHeapSize { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string PronunciationSIGPath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string PronunciationXMLPath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static PS4RemotePlayKeyAssignment remotePlayKeyAssignment { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string remotePlayKeyMappingDir { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool reprojectionSupport { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool restrictedAudioUsageRights { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string SaveDataImagePath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static int scriptOptimizationLevel { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string SdkOverride { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string ShareFilePath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string ShareOverlayImagePath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static int socialScreenEnabled { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string StartupImagePath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool useAudio3dBackend { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            internal static bool useDebugIl2cppLibs { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool useResolutionFallback { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static int videoOutBaseModeInitialWidth { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static int videoOutInitialWidth { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static int videoOutPixelFormat { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static int videoOutReprojectionRate { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("videoOutResolution is deprecated. Use PlayerSettings.PS4.videoOutInitialWidth and PlayerSettings.PS4.videoOutReprojectionRate to control initial display resolution and reprojection rate.")]
            public static int videoOutResolution { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool videoRecordingFeaturesUsed { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public enum PlayStationVREyeToEyeDistanceSettings
            {
                PerUser,
                ForceDefault,
                DynamicModeAtRuntime
            }

            /// <summary>
            /// <para>PS4 application category.</para>
            /// </summary>
            public enum PS4AppCategory
            {
                Application,
                Patch
            }

            /// <summary>
            /// <para>PS4 enter button assignment.</para>
            /// </summary>
            public enum PS4EnterButtonAssignment
            {
                CircleButton,
                CrossButton
            }

            /// <summary>
            /// <para>Remote Play key assignment.</para>
            /// </summary>
            public enum PS4RemotePlayKeyAssignment
            {
                /// <summary>
                /// <para>No Remote play key assignment.</para>
                /// </summary>
                None = -1,
                /// <summary>
                /// <para>Remote Play key layout configuration A.</para>
                /// </summary>
                PatternA = 0,
                /// <summary>
                /// <para>Remote Play key layout configuration B.</para>
                /// </summary>
                PatternB = 1,
                /// <summary>
                /// <para>Remote Play key layout configuration C.</para>
                /// </summary>
                PatternC = 2,
                /// <summary>
                /// <para>Remote Play key layout configuration D.</para>
                /// </summary>
                PatternD = 3,
                /// <summary>
                /// <para>Remote Play key layout configuration E.</para>
                /// </summary>
                PatternE = 4,
                /// <summary>
                /// <para>Remote Play key layout configuration F.</para>
                /// </summary>
                PatternF = 5,
                /// <summary>
                /// <para>Remote Play key layout configuration G.</para>
                /// </summary>
                PatternG = 6,
                /// <summary>
                /// <para>Remote Play key layout configuration H.</para>
                /// </summary>
                PatternH = 7
            }
        }

        public sealed class PSM
        {
        }

        /// <summary>
        /// <para>PS Vita specific player settings.</para>
        /// </summary>
        public sealed class PSVita
        {
            /// <summary>
            /// <para>Aquire PS Vita background music.</para>
            /// </summary>
            public static bool acquireBGM { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Support for the PS Vita twitter dialog was removed by SCE in SDK 3.570.</para>
            /// </summary>
            [Obsolete("AllowTwitterDialog has no effect as of SDK 3.570")]
            public static bool AllowTwitterDialog { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>The PS Vita application version.</para>
            /// </summary>
            public static string appVersion { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>The package build category.</para>
            /// </summary>
            public static PSVitaAppCategory category { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>The applications content ID.</para>
            /// </summary>
            public static string contentID { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>PS Vita DRM Type.</para>
            /// </summary>
            public static PSVitaDRMType drmType { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Specifies whether circle or cross will be used as the default enter button.</para>
            /// </summary>
            public static PSVitaEnterButtonAssignment enterButtonAssignment { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Specifies whether or not a health warning will be added to the software manual.</para>
            /// </summary>
            public static bool healthWarning { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Specifies the color of the PS Vita information bar, true = white, false = black.</para>
            /// </summary>
            public static bool infoBarColor { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Specifies whether or not to show the PS Vita information bar when the application starts.</para>
            /// </summary>
            public static bool infoBarOnStartup { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Keystone file.</para>
            /// </summary>
            public static string keystoneFile { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>PS Vita Live area background image.</para>
            /// </summary>
            public static string liveAreaBackroundPath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>PS Vita Live area gate image.</para>
            /// </summary>
            public static string liveAreaGatePath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>PS Vita Live area path.</para>
            /// </summary>
            public static string liveAreaPath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>PS Vita Live area trial path.</para>
            /// </summary>
            public static string liveAreaTrialPath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>PS Vita sofware manual.</para>
            /// </summary>
            public static string manualPath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>PS Vita content master version.</para>
            /// </summary>
            public static string masterVersion { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Should always = 01.00.</para>
            /// </summary>
            public static int mediaCapacity { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>PS Vita memory expansion mode.</para>
            /// </summary>
            public static PSVitaMemoryExpansionMode memoryExpansionMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>PSN Age rating.</para>
            /// </summary>
            public static int npAgeRating { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>PS Vita NP Passphrase.</para>
            /// </summary>
            public static string npCommsPassphrase { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>PS Vita NP Signature.</para>
            /// </summary>
            public static string npCommsSig { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>PS Vita NP Communications ID.</para>
            /// </summary>
            public static string npCommunicationsID { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Support Game Boot Message or Game Joining Presence.</para>
            /// </summary>
            public static bool npSupportGBMorGJP { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>PS Vita NP Title Data File.</para>
            /// </summary>
            public static string npTitleDatPath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Path specifying wher to copy a trophy pack from.</para>
            /// </summary>
            public static string npTrophyPackPath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>32 character password for use if you want to access the contents of a package.</para>
            /// </summary>
            public static string packagePassword { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Path specifying where to copy the package parameter file (param.sfx) from.</para>
            /// </summary>
            public static string paramSfxPath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>PS Vita parental level.</para>
            /// </summary>
            public static int parentalLevel { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>For cumlative patch packages.</para>
            /// </summary>
            public static string patchChangeInfoPath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>For building cumulative patch packages.</para>
            /// </summary>
            public static string patchOriginalPackage { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>PS Vita power mode.</para>
            /// </summary>
            public static PSVitaPowerMode powerMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Save data quota.</para>
            /// </summary>
            public static int saveDataQuota { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>The applications short title.</para>
            /// </summary>
            public static string shortTitle { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>PS Vita media type.</para>
            /// </summary>
            public static int storageType { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>PS Vita TV boot mode.</para>
            /// </summary>
            public static PSVitaTvBootMode tvBootMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>PS Vita TV Disable Emu flag.</para>
            /// </summary>
            public static bool tvDisableEmu { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Indicates that this is an upgradable (trial) type application which can be converted to a full application by purchasing an upgrade.</para>
            /// </summary>
            public static bool upgradable { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            internal static bool useDebugIl2cppLibs { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Support for the PS Vita location library was removed by SCE in SDK 3.570.</para>
            /// </summary>
            [Obsolete("useLibLocation has no effect as of SDK 3.570")]
            public static bool useLibLocation { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Application package category enum.</para>
            /// </summary>
            public enum PSVitaAppCategory
            {
                Application,
                ApplicationPatch
            }

            /// <summary>
            /// <para>DRM type enum.</para>
            /// </summary>
            public enum PSVitaDRMType
            {
                PaidFor,
                Free
            }

            /// <summary>
            /// <para>Enter button assignment enum.</para>
            /// </summary>
            public enum PSVitaEnterButtonAssignment
            {
                Default,
                CircleButton,
                CrossButton
            }

            /// <summary>
            /// <para>Memory expansion mode enum.</para>
            /// </summary>
            public enum PSVitaMemoryExpansionMode
            {
                None,
                ExpandBy29MB,
                ExpandBy77MB,
                ExpandBy109MB
            }

            /// <summary>
            /// <para>Power mode enum.</para>
            /// </summary>
            public enum PSVitaPowerMode
            {
                ModeA,
                ModeB,
                ModeC
            }

            /// <summary>
            /// <para>PS Vita TV boot mode enum.</para>
            /// </summary>
            public enum PSVitaTvBootMode
            {
                Default,
                PSVitaBootablePSVitaTvBootable,
                PSVitaBootablePSVitaTvNotBootable
            }
        }

        /// <summary>
        /// <para>Samsung Smart TV specific Player Settings.</para>
        /// </summary>
        public sealed class SamsungTV
        {
            /// <summary>
            /// <para>The address used when accessing the device.</para>
            /// </summary>
            public static string deviceAddress { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Author of the created product.</para>
            /// </summary>
            public static string productAuthor { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Product author's e-mail.</para>
            /// </summary>
            public static string productAuthorEmail { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>The category of the created product.</para>
            /// </summary>
            public static SamsungTVProductCategories productCategory { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>The description of the created product.</para>
            /// </summary>
            public static string productDescription { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>The author's website link.</para>
            /// </summary>
            public static string productLink { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Types of available product categories.</para>
            /// </summary>
            public enum SamsungTVProductCategories
            {
                Games,
                Videos,
                Sports,
                Lifestyle,
                Information,
                Education,
                Kids
            }
        }

        /// <summary>
        /// <para>Interface to splash screen player settings.</para>
        /// </summary>
        public sealed class SplashScreen
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            private static extern void INTERNAL_get_backgroundColor(out Color value);
            [MethodImpl(MethodImplOptions.InternalCall)]
            private static extern void INTERNAL_set_backgroundColor(ref Color value);

            /// <summary>
            /// <para>The target zoom (from 0 to 1) for the background when it reaches the end of the SplashScreen animation's total duration. Only used when animationMode is PlayerSettings.SplashScreen.AnimationMode.Custom|AnimationMode.Custom.</para>
            /// </summary>
            public static float animationBackgroundZoom { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>The target zoom (from 0 to 1) for the logo when it reaches the end of the logo animation's total duration. Only used when animationMode is PlayerSettings.SplashScreen.AnimationMode.Custom|AnimationMode.Custom.</para>
            /// </summary>
            public static float animationLogoZoom { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>The type of animation applied during the splash screen.</para>
            /// </summary>
            public static AnimationMode animationMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>The background Sprite that is shown in landscape mode. Also shown in portrait mode if backgroundPortrait is null.</para>
            /// </summary>
            public static Sprite background { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>The background color shown if no background Sprite is assigned. Default is a dark blue RGB(34.44,54).</para>
            /// </summary>
            public static Color backgroundColor
            {
                get
                {
                    Color color;
                    INTERNAL_get_backgroundColor(out color);
                    return color;
                }
                set
                {
                    INTERNAL_set_backgroundColor(ref value);
                }
            }

            /// <summary>
            /// <para>The background Sprite that is shown in portrait mode.</para>
            /// </summary>
            public static Sprite backgroundPortrait { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Determines how the Unity logo should be drawn, if it is enabled. If no Unity logo exists in [logos] then the draw mode defaults to PlayerSettings.SplashScreen.DrawMode.UnityLogoBelow|DrawMode.UnityLogoBelow.</para>
            /// </summary>
            public static DrawMode drawMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>The collection of logos that is shown during the splash screen. Logos are drawn in ascending order, starting from index 0, followed by 1, etc etc.</para>
            /// </summary>
            public static PlayerSettings.SplashScreenLogo[] logos { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>In order to increase contrast between the background and the logos, an overlay color modifier is applied. The overlay opacity is the strength of this effect. Note: Reducing the value below 0.5 requires a Plus/Pro license.</para>
            /// </summary>
            public static float overlayOpacity { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Set this to true to display the Splash Screen be shown when the application is launched. Set it to false to disable the Splash Screen. Note: Disabling the Splash Screen requires a Plus/Pro license.</para>
            /// </summary>
            public static bool show { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Set this to true to show the Unity logo during the Splash Screen. Set it to false to disable the Unity logo. Note: Disabling the Unity logo requires a Plus/Pro license.</para>
            /// </summary>
            public static bool showUnityLogo { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>The style to use for the Unity logo during the Splash Screen.</para>
            /// </summary>
            public static UnityLogoStyle unityLogoStyle { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>The type of animation applied during the Splash Screen.</para>
            /// </summary>
            public enum AnimationMode
            {
                Static,
                Dolly,
                Custom
            }

            /// <summary>
            /// <para>Determines how the Unity logo should be drawn, if it is enabled.</para>
            /// </summary>
            public enum DrawMode
            {
                UnityLogoBelow,
                AllSequential
            }

            /// <summary>
            /// <para>The style to use for the Unity logo during the Splash Screen.</para>
            /// </summary>
            public enum UnityLogoStyle
            {
                DarkOnLight,
                LightOnDark
            }
        }

        /// <summary>
        /// <para>A single logo that is shown during the Splash Screen. Controls the Sprite that is displayed and its duration.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SplashScreenLogo
        {
            private const float k_MinLogoTime = 2f;
            private static Sprite s_UnityLogo;
            private Sprite m_Logo;
            private float m_Duration;
            static SplashScreenLogo()
            {
                s_UnityLogo = Resources.GetBuiltinResource<Sprite>("UnitySplash-cube.png");
            }

            [ExcludeFromDocs]
            public static PlayerSettings.SplashScreenLogo Create(float duration)
            {
                Sprite logo = null;
                return Create(duration, logo);
            }

            [ExcludeFromDocs]
            public static PlayerSettings.SplashScreenLogo Create()
            {
                Sprite logo = null;
                float duration = 2f;
                return Create(duration, logo);
            }

            /// <summary>
            /// <para>Creates a new Splash Screen logo with the provided duration and logo Sprite.</para>
            /// </summary>
            /// <param name="duration">The total time in seconds that the logo will be shown. Note minimum time is 2 seconds.</param>
            /// <param name="logo">The logo Sprite to display.</param>
            /// <returns>
            /// <para>The new logo.</para>
            /// </returns>
            public static PlayerSettings.SplashScreenLogo Create([DefaultValue("k_MinLogoTime")] float duration, [DefaultValue("null")] Sprite logo)
            {
                return new PlayerSettings.SplashScreenLogo { 
                    m_Duration = duration,
                    m_Logo = logo
                };
            }

            [ExcludeFromDocs]
            public static PlayerSettings.SplashScreenLogo CreateWithUnityLogo()
            {
                float duration = 2f;
                return CreateWithUnityLogo(duration);
            }

            /// <summary>
            /// <para>Creates a new Splash Screen logo with the provided duration and the unity logo.</para>
            /// </summary>
            /// <param name="duration">The total time in seconds that the logo will be shown. Note minimum time is 2 seconds.</param>
            /// <returns>
            /// <para>The new logo.</para>
            /// </returns>
            public static PlayerSettings.SplashScreenLogo CreateWithUnityLogo([DefaultValue("k_MinLogoTime")] float duration)
            {
                return new PlayerSettings.SplashScreenLogo { 
                    m_Duration = duration,
                    m_Logo = s_UnityLogo
                };
            }

            /// <summary>
            /// <para>The Sprite that is shown during this logo. If this is null, then no logo will be displayed for the duration.</para>
            /// </summary>
            public Sprite logo
            {
                get
                {
                    return this.m_Logo;
                }
                set
                {
                    this.m_Logo = value;
                }
            }
            /// <summary>
            /// <para>The Unity logo Sprite.</para>
            /// </summary>
            public static Sprite unityLogo
            {
                get
                {
                    return s_UnityLogo;
                }
            }
            /// <summary>
            /// <para>The total time in seconds for which the logo is shown. The minimum duration is 2 seconds.</para>
            /// </summary>
            public float duration
            {
                get
                {
                    return Mathf.Max(this.m_Duration, 2f);
                }
                set
                {
                    this.m_Duration = Mathf.Max(value, 2f);
                }
            }
        }

        /// <summary>
        /// <para>Tizen specific player settings.</para>
        /// </summary>
        public sealed class Tizen
        {
            public static bool GetCapability(PlayerSettings.TizenCapability capability)
            {
                string str = InternalGetCapability(capability.ToString());
                if (string.IsNullOrEmpty(str))
                {
                    return false;
                }
                try
                {
                    return (bool) TypeDescriptor.GetConverter(typeof(bool)).ConvertFromString(str);
                }
                catch
                {
                    Debug.LogError("Failed to parse value  ('" + capability.ToString() + "," + str + "') to bool type.");
                    return false;
                }
            }

            [MethodImpl(MethodImplOptions.InternalCall)]
            private static extern string InternalGetCapability(string name);
            [MethodImpl(MethodImplOptions.InternalCall)]
            private static extern void InternalSetCapability(string name, string value);
            public static void SetCapability(PlayerSettings.TizenCapability capability, bool value)
            {
                InternalSetCapability(capability.ToString(), value.ToString());
            }

            /// <summary>
            /// <para>Currently chosen Tizen deployment target.</para>
            /// </summary>
            public static string deploymentTarget { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Choose a type of Tizen target to deploy to. Options are Device or Emulator.</para>
            /// </summary>
            public static int deploymentTargetType { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Minimum Tizen OS version that this application is compatible with. 
            /// IMPORTANT: For example: if you choose Tizen 2.4 your application will only run on devices with Tizen 2.4 or later.</para>
            /// </summary>
            public static TizenOSVersion minOSVersion { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Description of your project to be displayed in the Tizen Store.</para>
            /// </summary>
            public static string productDescription { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>URL of your project to be displayed in the Tizen Store.</para>
            /// </summary>
            public static string productURL { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Sets or gets the game loading indicator style.For available styles see TizenShowActivityIndicatorOnLoading.</para>
            /// </summary>
            public static TizenShowActivityIndicatorOnLoading showActivityIndicatorOnLoading { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Name of the security profile to code sign Tizen applications with.</para>
            /// </summary>
            public static string signingProfileName { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        }

        /// <summary>
        /// <para>Tizen application capabilities.</para>
        /// </summary>
        public enum TizenCapability
        {
            Location,
            DataSharing,
            NetworkGet,
            WifiDirect,
            CallHistoryRead,
            Power,
            ContactWrite,
            MessageWrite,
            ContentWrite,
            Push,
            AccountRead,
            ExternalStorage,
            Recorder,
            PackageManagerInfo,
            NFCCardEmulation,
            CalendarWrite,
            WindowPrioritySet,
            VolumeSet,
            CallHistoryWrite,
            AlarmSet,
            Call,
            Email,
            ContactRead,
            Shortcut,
            KeyManager,
            LED,
            NetworkProfile,
            AlarmGet,
            Display,
            CalendarRead,
            NFC,
            AccountWrite,
            Bluetooth,
            Notification,
            NetworkSet,
            ExternalStorageAppData,
            Download,
            Telephony,
            MessageRead,
            MediaStorage,
            Internet,
            Camera,
            Haptic,
            AppManagerLaunch,
            SystemSettings
        }

        /// <summary>
        /// <para>tvOS specific player settings.</para>
        /// </summary>
        public sealed class tvOS
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            internal static extern Texture2D[] GetLargeIconLayers();
            [MethodImpl(MethodImplOptions.InternalCall)]
            internal static extern Texture2D[] GetSmallIconLayers();
            [MethodImpl(MethodImplOptions.InternalCall)]
            internal static extern Texture2D[] GetTopShelfImageLayers();
            [MethodImpl(MethodImplOptions.InternalCall)]
            internal static extern Texture2D[] GetTopShelfImageWideLayers();
            [MethodImpl(MethodImplOptions.InternalCall)]
            internal static extern void SetLargeIconLayers(Texture2D[] layers);
            [MethodImpl(MethodImplOptions.InternalCall)]
            internal static extern void SetSmallIconLayers(Texture2D[] layers);
            [MethodImpl(MethodImplOptions.InternalCall)]
            internal static extern void SetTopShelfImageLayers(Texture2D[] layers);
            [MethodImpl(MethodImplOptions.InternalCall)]
            internal static extern void SetTopShelfImageWideLayers(Texture2D[] layers);

            /// <summary>
            /// <para>Application requires extended game controller.</para>
            /// </summary>
            public static bool requireExtendedGameController { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Active tvOS SDK version used for build.</para>
            /// </summary>
            public static tvOSSdkVersion sdkVersion { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Deployment minimal version of tvOS.</para>
            /// </summary>
            public static tvOSTargetOSVersion targetOSVersion { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Deployment minimal version of tvOS.</para>
            /// </summary>
            public static string targetOSVersionString { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        }

        /// <summary>
        /// <para>WebGL specific player settings.</para>
        /// </summary>
        public sealed class WebGL
        {
            public static bool analyzeBuildSize { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>CompressionFormat defines the compression type that the WebGL resources are encoded to.</para>
            /// </summary>
            public static WebGLCompressionFormat compressionFormat { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Enables automatic caching of asset data.</para>
            /// </summary>
            public static bool dataCaching { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Enables writting out of debug symbols to the build output directory in a *.debugSymbols.js file.</para>
            /// </summary>
            public static bool debugSymbols { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string emscriptenArgs { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Exception support for WebGL builds.</para>
            /// </summary>
            public static WebGLExceptionSupport exceptionSupport { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Memory size for WebGL builds in MB.</para>
            /// </summary>
            public static int memorySize { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string modulesDirectory { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Path to the WebGL template asset.</para>
            /// </summary>
            public static string template { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool useEmbeddedResources { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool useWasm { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        }

        public sealed class WiiU
        {
            public static int accountBossSize { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static int accountSaveSize { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string[] addOnUniqueIDs { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool allowScreenCapture { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static int commonBossSize { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static int commonSaveSize { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static int controllerCount { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool drcBufferDisabled { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static int gamePadMSAA { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static Texture2D gamePadStartupScreen { [MethodImpl(MethodImplOptions.InternalCall)] get; }

            public static string groupID { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string joinGameId { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string joinGameModeMask { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static int loaderThreadStackSize { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static int mainThreadStackSize { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string olvAccessKey { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string profilerLibraryPath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool supportsBalanceBoard { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool supportsClassicController { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool supportsMotionPlus { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool supportsNunchuk { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool supportsProController { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static int systemHeapSize { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string tinCode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string titleID { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static WiiUTVResolution tvResolution { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static Texture2D tvStartupScreen { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        }

        /// <summary>
        /// <para>Windows Store Apps specific player settings.</para>
        /// </summary>
        public sealed class WSA
        {
            public static bool GetCapability(PlayerSettings.WSACapability capability)
            {
                string str = InternalGetCapability(capability.ToString());
                if (string.IsNullOrEmpty(str))
                {
                    return false;
                }
                try
                {
                    return (bool) TypeDescriptor.GetConverter(typeof(bool)).ConvertFromString(str);
                }
                catch
                {
                    Debug.LogError("Failed to parse value  ('" + capability.ToString() + "," + str + "') to bool type.");
                    return false;
                }
            }

            public static string GetVisualAssetsImage(PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale scale)
            {
                ValidateWSAImageType(type);
                ValidateWSAImageScale(scale);
                return GetWSAImage(type, scale);
            }

            [MethodImpl(MethodImplOptions.InternalCall)]
            private static extern string GetWSAImage(PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale scale);
            [MethodImpl(MethodImplOptions.InternalCall)]
            private static extern void INTERNAL_get_internalFileTypeAssociations(out PlayerSettings.WSAFileTypeAssociations value);
            [MethodImpl(MethodImplOptions.InternalCall)]
            private static extern void INTERNAL_get_splashScreenBackgroundColorRaw(out Color value);
            [MethodImpl(MethodImplOptions.InternalCall)]
            private static extern void INTERNAL_get_tileBackgroundColor(out Color value);
            [MethodImpl(MethodImplOptions.InternalCall)]
            private static extern void INTERNAL_set_internalFileTypeAssociations(ref PlayerSettings.WSAFileTypeAssociations value);
            [MethodImpl(MethodImplOptions.InternalCall)]
            private static extern void INTERNAL_set_splashScreenBackgroundColorRaw(ref Color value);
            [MethodImpl(MethodImplOptions.InternalCall)]
            private static extern void INTERNAL_set_tileBackgroundColor(ref Color value);
            [MethodImpl(MethodImplOptions.InternalCall)]
            private static extern string InternalGetCapability(string name);
            [MethodImpl(MethodImplOptions.InternalCall)]
            private static extern void InternalSetCapability(string name, string value);
            public static void SetCapability(PlayerSettings.WSACapability capability, bool value)
            {
                InternalSetCapability(capability.ToString(), value.ToString());
            }

            [MethodImpl(MethodImplOptions.InternalCall)]
            public static extern bool SetCertificate(string path, string password);
            public static void SetVisualAssetsImage(string image, PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale scale)
            {
                ValidateWSAImageType(type);
                ValidateWSAImageScale(scale);
                SetWSAImage(image, type, scale);
            }

            [MethodImpl(MethodImplOptions.InternalCall)]
            private static extern void SetWSAImage(string image, PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale scale);
            internal static string ValidatePackageVersion(string value)
            {
                Regex regex = new Regex(@"^(\d+)\.(\d+)\.(\d+)\.(\d+)$", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                if (regex.IsMatch(value))
                {
                    return value;
                }
                return "1.0.0.0";
            }

            private static void ValidateWSAImageScale(PlayerSettings.WSAImageScale scale)
            {
                if (((((scale != PlayerSettings.WSAImageScale.Target16) && (scale != PlayerSettings.WSAImageScale.Target24)) && ((scale != PlayerSettings.WSAImageScale.Target32) && (scale != PlayerSettings.WSAImageScale.Target48))) && (((scale != PlayerSettings.WSAImageScale._80) && (scale != PlayerSettings.WSAImageScale._100)) && ((scale != PlayerSettings.WSAImageScale._125) && (scale != PlayerSettings.WSAImageScale._140)))) && ((((scale != PlayerSettings.WSAImageScale._150) && (scale != PlayerSettings.WSAImageScale._180)) && ((scale != PlayerSettings.WSAImageScale._200) && (scale != PlayerSettings.WSAImageScale._240))) && ((scale != PlayerSettings.WSAImageScale.Target256) && (scale != PlayerSettings.WSAImageScale._400))))
                {
                    throw new Exception("Unknown image scale: " + scale);
                }
            }

            private static void ValidateWSAImageType(PlayerSettings.WSAImageType type)
            {
                switch (type)
                {
                    case PlayerSettings.WSAImageType.StoreTileLogo:
                    case PlayerSettings.WSAImageType.StoreTileWideLogo:
                    case PlayerSettings.WSAImageType.StoreTileSmallLogo:
                    case PlayerSettings.WSAImageType.StoreSmallTile:
                    case PlayerSettings.WSAImageType.StoreLargeTile:
                    case PlayerSettings.WSAImageType.PhoneAppIcon:
                    case PlayerSettings.WSAImageType.PhoneSmallTile:
                    case PlayerSettings.WSAImageType.PhoneMediumTile:
                    case PlayerSettings.WSAImageType.PhoneWideTile:
                    case PlayerSettings.WSAImageType.PhoneSplashScreen:
                    case PlayerSettings.WSAImageType.UWPSquare44x44Logo:
                    case PlayerSettings.WSAImageType.UWPSquare71x71Logo:
                    case PlayerSettings.WSAImageType.UWPSquare150x150Logo:
                    case PlayerSettings.WSAImageType.UWPSquare310x310Logo:
                    case PlayerSettings.WSAImageType.UWPWide310x150Logo:
                        return;
                }
                if ((type != PlayerSettings.WSAImageType.PackageLogo) && (type != PlayerSettings.WSAImageType.SplashScreenImage))
                {
                    throw new Exception("Unknown WSA image type: " + type);
                }
            }

            public static string applicationDescription { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string certificateIssuer { [MethodImpl(MethodImplOptions.InternalCall)] get; }

            public static DateTime? certificateNotAfter
            {
                get
                {
                    long certificateNotAfterRaw = PlayerSettings.WSA.certificateNotAfterRaw;
                    if (certificateNotAfterRaw != 0L)
                    {
                        return new DateTime?(DateTime.FromFileTime(certificateNotAfterRaw));
                    }
                    return null;
                }
            }

            private static long certificateNotAfterRaw { [MethodImpl(MethodImplOptions.InternalCall)] get; }

            internal static string certificatePassword { [MethodImpl(MethodImplOptions.InternalCall)] get; }

            public static string certificatePath { [MethodImpl(MethodImplOptions.InternalCall)] get; }

            public static string certificateSubject { [MethodImpl(MethodImplOptions.InternalCall)] get; }

            public static string commandLineArgsFile { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Specify how to compile C# files when building to Windows Store Apps.</para>
            /// </summary>
            public static PlayerSettings.WSACompilationOverrides compilationOverrides { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static PlayerSettings.WSADefaultTileSize defaultTileSize { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Enable/Disable independent input source feature.</para>
            /// </summary>
            [Obsolete("PlayerSettings.WSA.enableIndependentInputSource is deprecated. Use PlayerSettings.WSA.inputSource.", false)]
            public static bool enableIndependentInputSource
            {
                get
                {
                    return (inputSource == PlayerSettings.WSAInputSource.IndependentInputSource);
                }
                set
                {
                    inputSource = !value ? PlayerSettings.WSAInputSource.CoreWindow : PlayerSettings.WSAInputSource.IndependentInputSource;
                }
            }

            /// <summary>
            /// <para>Enable/Disable low latency presentation API.</para>
            /// </summary>
            [Obsolete("PlayerSettings.enableLowLatencyPresentationAPI is deprecated. It is now always enabled.", false)]
            public static bool enableLowLatencyPresentationAPI { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Where Unity gets input from.</para>
            /// </summary>
            public static PlayerSettings.WSAInputSource inputSource { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            internal static PlayerSettings.WSAFileTypeAssociations internalFileTypeAssociations
            {
                get
                {
                    PlayerSettings.WSAFileTypeAssociations associations;
                    INTERNAL_get_internalFileTypeAssociations(out associations);
                    return associations;
                }
                set
                {
                    INTERNAL_set_internalFileTypeAssociations(ref value);
                }
            }

            internal static string internalProtocolName { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool largeTileShowName { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool mediumTileShowName { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string packageLogo { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string packageLogo140 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string packageLogo180 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string packageLogo240 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string packageName { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static Version packageVersion
            {
                get
                {
                    Version version;
                    try
                    {
                        version = new Version(ValidatePackageVersion(packageVersionRaw));
                    }
                    catch (Exception exception)
                    {
                        throw new Exception(string.Format("{0}, the raw string was {1}", exception.Message, packageVersionRaw));
                    }
                    return version;
                }
                set
                {
                    packageVersionRaw = value.ToString();
                }
            }

            private static string packageVersionRaw { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneAppIcon { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneAppIcon140 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneAppIcon240 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneMediumTile { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneMediumTile140 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneMediumTile240 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneSmallTile { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneSmallTile140 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneSmallTile240 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneSplashScreenImage { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneSplashScreenImageScale140 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneSplashScreenImageScale240 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneWideTile { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneWideTile140 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string phoneWideTile240 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static Color? splashScreenBackgroundColor
            {
                get
                {
                    if (splashScreenUseBackgroundColor)
                    {
                        return new Color?(splashScreenBackgroundColorRaw);
                    }
                    return null;
                }
                set
                {
                    splashScreenUseBackgroundColor = value.HasValue;
                    if (value.HasValue)
                    {
                        splashScreenBackgroundColorRaw = value.Value;
                    }
                }
            }

            private static Color splashScreenBackgroundColorRaw
            {
                get
                {
                    Color color;
                    INTERNAL_get_splashScreenBackgroundColorRaw(out color);
                    return color;
                }
                set
                {
                    INTERNAL_set_splashScreenBackgroundColorRaw(ref value);
                }
            }

            private static bool splashScreenUseBackgroundColor { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeLargeTile { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeLargeTile140 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeLargeTile180 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeLargeTile80 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeSmallTile { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeSmallTile140 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeSmallTile180 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeSmallTile80 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeSplashScreenImage { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeSplashScreenImageScale140 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeSplashScreenImageScale180 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeTileLogo { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeTileLogo140 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeTileLogo180 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeTileLogo80 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeTileSmallLogo { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeTileSmallLogo140 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeTileSmallLogo180 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeTileSmallLogo80 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeTileWideLogo { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeTileWideLogo140 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeTileWideLogo180 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
            public static string storeTileWideLogo80 { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static Color tileBackgroundColor
            {
                get
                {
                    Color color;
                    INTERNAL_get_tileBackgroundColor(out color);
                    return color;
                }
                set
                {
                    INTERNAL_set_tileBackgroundColor(ref value);
                }
            }

            public static PlayerSettings.WSAApplicationForegroundText tileForegroundText { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string tileShortName { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static PlayerSettings.WSAApplicationShowName tileShowName { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool wideTileShowName { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            /// <summary>
            /// <para>Windows Store Apps declarations.</para>
            /// </summary>
            public static class Declarations
            {
                /// <summary>
                /// <para>Set information for file type associations.
                /// 
                /// For more information - https:msdn.microsoft.comlibrarywindowsappshh779671https:msdn.microsoft.comlibrarywindowsappshh779671.</para>
                /// </summary>
                public static PlayerSettings.WSAFileTypeAssociations fileTypeAssociations
                {
                    get
                    {
                        return PlayerSettings.WSA.internalFileTypeAssociations;
                    }
                    set
                    {
                        PlayerSettings.WSA.internalFileTypeAssociations = value;
                    }
                }

                /// <summary>
                /// <para>
                /// Registers this application to be a default handler for specified URI scheme name.
                /// 
                /// For example: if you specify myunitygame, your application can be run from other applications via the URI scheme myunitygame:. You can also test this using the Windows "Run" dialog box (invoked with Windows + R key).
                /// 
                /// For more information https:msdn.microsoft.comlibrarywindowsappshh779670https:msdn.microsoft.comlibrarywindowsappshh779670.</para>
                /// </summary>
                public static string protocolName
                {
                    get
                    {
                        return PlayerSettings.WSA.internalProtocolName;
                    }
                    set
                    {
                        PlayerSettings.WSA.internalProtocolName = value;
                    }
                }
            }
        }

        public enum WSAApplicationForegroundText
        {
            Dark = 2,
            Light = 1
        }

        public enum WSAApplicationShowName
        {
            NotSet,
            AllLogos,
            NoLogos,
            StandardLogoOnly,
            WideLogoOnly
        }

        public enum WSACapability
        {
            EnterpriseAuthentication,
            InternetClient,
            InternetClientServer,
            MusicLibrary,
            PicturesLibrary,
            PrivateNetworkClientServer,
            RemovableStorage,
            SharedUserCertificates,
            VideosLibrary,
            WebCam,
            Proximity,
            Microphone,
            Location,
            HumanInterfaceDevice,
            AllJoyn,
            BlockedChatMessages,
            Chat,
            CodeGeneration,
            Objects3D,
            PhoneCall,
            UserAccountInformation,
            VoipCall,
            Bluetooth,
            SpatialPerception,
            InputInjectionBrokered
        }

        /// <summary>
        /// <para>Compilation overrides for C# files.</para>
        /// </summary>
        public enum WSACompilationOverrides
        {
            None,
            UseNetCore,
            UseNetCorePartially
        }

        public enum WSADefaultTileSize
        {
            NotSet,
            Medium,
            Wide
        }

        /// <summary>
        /// <para>Describes File Type Association declaration.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
        public struct WSAFileTypeAssociations
        {
            /// <summary>
            /// <para>Localizable string that will be displayed to the user as associated file handler.</para>
            /// </summary>
            public string name;
            /// <summary>
            /// <para>Supported file types for this association.</para>
            /// </summary>
            public PlayerSettings.WSASupportedFileType[] supportedFileTypes;
        }

        /// <summary>
        /// <para>Various image scales, supported by Windows Store Apps.</para>
        /// </summary>
        public enum WSAImageScale
        {
            _100 = 100,
            _125 = 0x7d,
            _140 = 140,
            _150 = 150,
            _180 = 180,
            _200 = 200,
            _240 = 240,
            _400 = 400,
            _80 = 80,
            Target16 = 0x10,
            Target24 = 0x18,
            Target256 = 0x100,
            Target32 = 0x20,
            Target48 = 0x30
        }

        /// <summary>
        /// <para>Image types, supported by Windows Store Apps.</para>
        /// </summary>
        public enum WSAImageType
        {
            PackageLogo = 1,
            PhoneAppIcon = 0x15,
            PhoneMediumTile = 0x17,
            PhoneSmallTile = 0x16,
            PhoneSplashScreen = 0x19,
            PhoneWideTile = 0x18,
            SplashScreenImage = 2,
            StoreLargeTile = 15,
            StoreSmallTile = 14,
            StoreTileLogo = 11,
            StoreTileSmallLogo = 13,
            StoreTileWideLogo = 12,
            UWPSquare150x150Logo = 0x21,
            UWPSquare310x310Logo = 0x22,
            UWPSquare44x44Logo = 0x1f,
            UWPSquare71x71Logo = 0x20,
            UWPWide310x150Logo = 0x23
        }

        /// <summary>
        /// <para>Where Unity takes input from (subscripbes to events).</para>
        /// </summary>
        public enum WSAInputSource
        {
            CoreWindow,
            IndependentInputSource,
            SwapChainPanel
        }

        /// <summary>
        /// <para>Describes supported file type for File Type Association declaration.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
        public struct WSASupportedFileType
        {
            /// <summary>
            /// <para>The 'Content Type' value for the file type's MIME content type. For example: 'image/jpeg'. Can also be left blank.</para>
            /// </summary>
            public string contentType;
            /// <summary>
            /// <para>File type extension. For ex., .myUnityGame</para>
            /// </summary>
            public string fileType;
        }

        public sealed class XboxOne
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            public static extern bool AddAllowedProductId(string id);
            [MethodImpl(MethodImplOptions.InternalCall)]
            public static extern bool GetCapability(string capability);
            [MethodImpl(MethodImplOptions.InternalCall)]
            public static extern int GetGameRating(string name);
            [MethodImpl(MethodImplOptions.InternalCall)]
            public static extern void GetSocketDefinition(string name, out string port, out int protocol, out int[] usages, out string templateName, out int sessionRequirment, out int[] deviceUsages);
            [MethodImpl(MethodImplOptions.InternalCall)]
            public static extern bool GetSupportedLanguage(string language);
            [MethodImpl(MethodImplOptions.InternalCall)]
            public static extern void RemoveAllowedProductId(string id);
            [MethodImpl(MethodImplOptions.InternalCall)]
            public static extern void RemoveSocketDefinition(string name);
            [MethodImpl(MethodImplOptions.InternalCall)]
            public static extern void SetCapability(string capability, bool value);
            [MethodImpl(MethodImplOptions.InternalCall)]
            public static extern void SetGameRating(string name, int value);
            [MethodImpl(MethodImplOptions.InternalCall)]
            public static extern void SetSocketDefinition(string name, string port, int protocol, int[] usages, string templateName, int sessionRequirment, int[] deviceUsages);
            [MethodImpl(MethodImplOptions.InternalCall)]
            public static extern void SetSupportedLanguage(string language, bool enabled);
            [MethodImpl(MethodImplOptions.InternalCall)]
            public static extern void UpdateAllowedProductId(int idx, string id);

            public static string[] AllowedProductIds { [MethodImpl(MethodImplOptions.InternalCall)] get; }

            public static string AppManifestOverridePath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string ContentId { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static XboxOneLoggingLevel defaultLoggingLevel { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string Description { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool DisableKinectGpuReservation { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool EnablePIXSampling { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool EnableVariableGPU { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string GameOsOverridePath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static bool IsContentPackage { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static int monoLoggingLevel { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static XboxOnePackageUpdateGranularity PackageUpdateGranularity { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static XboxOneEncryptionLevel PackagingEncryption { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string PackagingOverridePath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static uint PersistentLocalStorageSize { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string ProductId { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string SandboxId { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string SCID { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string[] SocketNames { [MethodImpl(MethodImplOptions.InternalCall)] get; }

            public static string TitleId { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string UpdateKey { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            public static string Version { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        }
    }
}

