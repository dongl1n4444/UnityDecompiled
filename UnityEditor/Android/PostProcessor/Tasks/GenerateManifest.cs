namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Xml;
    using UnityEditor;
    using UnityEditor.Analytics;
    using UnityEditor.Android;
    using UnityEditor.Android.PostProcessor;
    using UnityEditor.CrashReporting;
    using UnityEditor.Utils;
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.Rendering;

    internal class GenerateManifest : IPostProcessorTask
    {
        private bool _developmentPlayer;
        private string _stagingArea;
        private static readonly string[] ReservedJavaKeywords = new string[] { 
            "abstract", "continue", "for", "new", "switch", "assert", "default", "if", "package", "synchronized", "boolean", "do", "goto", "private", "this", "break",
            "double", "implements", "protected", "throw", "byte", "else", "import", "public", "throws", "case", "enum", "instanceof", "return", "transient", "catch", "extends",
            "int", "short", "try", "char", "final", "interface", "static", "void", "class", "finally", "long", "strictfp", "volatile", "const", "float", "native",
            "super", "while", "null", "false", "true"
        };

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event ProgressHandler OnProgress;

        private void AddVRRelatedManifestEntries(PostProcessorContext context, AndroidManifest manifestXML, HashSet<string> activities)
        {
            VrSupportChecker checker = new VrSupportChecker();
            if (checker.isCardboardEnabled || checker.isDaydreamEnabled)
            {
                manifestXML.OverrideTheme("@style/VrActivityTheme");
            }
            if (checker.isCardboardEnabled)
            {
                manifestXML.AddIntentFilterCategory("com.google.intent.category.CARDBOARD");
            }
            if (checker.isDaydreamEnabled)
            {
                manifestXML.AddUsesFeature("android.hardware.vr.high_performance", checker.isDaydreamOnly);
                manifestXML.AddIntentFilterCategory("com.google.intent.category.DAYDREAM");
                manifestXML.AddResourceToLaunchActivity("com.google.android.vr.icon", "@drawable/vr_icon_front");
                manifestXML.AddResourceToLaunchActivity("com.google.android.vr.icon_background", "@drawable/vr_icon_back");
                foreach (string str in activities)
                {
                    if (checker.isDaydreamPrimary)
                    {
                        manifestXML.EnableVrMode(str);
                    }
                    manifestXML.SetResizableActivity(str, false);
                }
            }
            if (checker.isCardboardEnabled || checker.isDaydreamEnabled)
            {
                manifestXML.AddApplicationMetaDataAttribute("unityplayer.SkipPermissionsDialog", "true");
            }
            if (checker.isCardboardEnabled)
            {
                manifestXML.AddUsesPermission("android.permission.READ_EXTERNAL_STORAGE");
            }
        }

        private string CopyMainManifest(PostProcessorContext context, string target)
        {
            string str = context.Get<string>("AndroidPluginsPath");
            string str2 = context.Get<string>("PlayerPackage");
            string path = Path.Combine(str, "AndroidManifest.xml");
            if (!File.Exists(path))
            {
                string[] components = new string[] { str2, "Apk", "AndroidManifest.xml" };
                path = Paths.Combine(components);
            }
            return new AndroidManifest(path).SaveAs(target);
        }

        private void CreateSupportsTextureElem(AndroidManifest manifestXML, MobileTextureSubtarget subTarget)
        {
            if ((PlayerSettings.colorSpace == ColorSpace.Linear) && (subTarget != MobileTextureSubtarget.DXT))
            {
                UnityEngine.Debug.LogWarning("Linear rendering works only on new Tegra devices");
            }
            switch (subTarget)
            {
                case MobileTextureSubtarget.Generic:
                case MobileTextureSubtarget.ETC:
                    manifestXML.AddSupportsGLTexture("GL_OES_compressed_ETC1_RGB8_texture");
                    break;

                case MobileTextureSubtarget.DXT:
                    manifestXML.AddSupportsGLTexture("GL_EXT_texture_compression_dxt1");
                    manifestXML.AddSupportsGLTexture("GL_EXT_texture_compression_dxt5");
                    manifestXML.AddSupportsGLTexture("GL_EXT_texture_compression_s3tc");
                    break;

                case MobileTextureSubtarget.PVRTC:
                    manifestXML.AddSupportsGLTexture("GL_IMG_texture_compression_pvrtc");
                    break;

                case MobileTextureSubtarget.ATC:
                    manifestXML.AddSupportsGLTexture("GL_AMD_compressed_ATC_texture");
                    manifestXML.AddSupportsGLTexture("GL_ATI_texture_compression_atitc");
                    break;

                case MobileTextureSubtarget.ETC2:
                    break;

                case MobileTextureSubtarget.ASTC:
                    manifestXML.AddSupportsGLTexture("GL_KHR_texture_compression_astc_ldr");
                    break;

                default:
                    UnityEngine.Debug.LogWarning("SubTarget not recognized : " + subTarget);
                    break;
            }
        }

        private bool doesReferenceNetworkClasses(AssemblyReferenceChecker checker) => 
            (((((checker.HasReferenceToType("UnityEngine.Networking") || checker.HasReferenceToType("System.Net.Sockets")) || (checker.HasReferenceToType("UnityEngine.Network") || checker.HasReferenceToType("UnityEngine.RPC"))) || ((checker.HasReferenceToType("UnityEngine.WWW") || checker.HasReferenceToType(typeof(Ping).FullName)) || (checker.HasReferenceToType(typeof(UnityWebRequest).FullName) || PlayerSettings.submitAnalytics))) || (AnalyticsSettings.enabled || PerformanceReportingSettings.enabled)) || CrashReportingSettings.enabled);

        public void Execute(PostProcessorContext context)
        {
            this._developmentPlayer = context.Get<bool>("DevelopmentPlayer");
            this._stagingArea = context.Get<string>("StagingArea");
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Merging AndroidManifest.xml files");
            }
            string manifest = this.CopyMainManifest(context, Path.Combine(this._stagingArea, "AndroidManifest-main.xml"));
            this.InjectBundleAndSDKVersion(context, manifest);
            string str2 = this.MergeManifests(context, Path.Combine(this._stagingArea, "AndroidManifest.xml"), manifest);
            context.Set<string>("ManifestName", str2);
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Patching manifest");
            }
            string str3 = this.PatchManifest(context, str2);
            context.Set<string>("PackageName", str3);
            if (new AndroidManifest(str2).GetActivityWithLaunchIntent().Length == 0)
            {
                UnityEngine.Debug.LogWarning("No activity found in the manifest with action MAIN and category LAUNCHER.\nYour application may not start correctly.");
            }
            this.ThrowIfInvalid(str3);
        }

        private HashSet<string> GetActivitiesWithMetadata(AndroidManifest doc, string name, string val)
        {
            HashSet<string> set = new HashSet<string>();
            IEnumerator enumerator = doc.GetElementsByTagName("meta-data").GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    XmlElement current = (XmlElement) enumerator.Current;
                    XmlElement parentNode = (XmlElement) current.ParentNode;
                    if (((parentNode != null) && (parentNode.LocalName == "activity")) && ((current.GetAttribute("android:name") == name) && (current.GetAttribute("android:value") == val)))
                    {
                        set.Add(parentNode.GetAttribute("android:name"));
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            return set;
        }

        private string GetOrientationAttr()
        {
            UIOrientation defaultInterfaceOrientation = PlayerSettings.defaultInterfaceOrientation;
            string str = null;
            bool flag = PlayerSettings.allowedAutorotateToPortrait || PlayerSettings.allowedAutorotateToPortraitUpsideDown;
            bool flag2 = PlayerSettings.allowedAutorotateToLandscapeLeft || PlayerSettings.allowedAutorotateToLandscapeRight;
            switch (defaultInterfaceOrientation)
            {
                case UIOrientation.Portrait:
                    str = "portrait";
                    break;

                case UIOrientation.PortraitUpsideDown:
                    str = "reversePortrait";
                    break;

                case UIOrientation.LandscapeRight:
                    str = "reverseLandscape";
                    break;

                case UIOrientation.LandscapeLeft:
                    str = "landscape";
                    break;

                default:
                    if (flag && flag2)
                    {
                        str = "fullSensor";
                    }
                    else if (flag)
                    {
                        str = "sensorPortrait";
                    }
                    else if (flag2)
                    {
                        str = "sensorLandscape";
                    }
                    else
                    {
                        str = "unspecified";
                    }
                    break;
            }
            if (PlayerSettings.virtualRealitySupported)
            {
                str = "landscape";
            }
            return str;
        }

        private void InjectBundleAndSDKVersion(PostProcessorContext context, string manifest)
        {
            int targetSdkVersion = context.Get<int>("TargetSDKVersion");
            AndroidManifest manifest2 = new AndroidManifest(manifest);
            string bundleVersion = PlayerSettings.bundleVersion;
            int bundleVersionCode = PlayerSettings.Android.bundleVersionCode;
            manifest2.SetVersion(bundleVersion, bundleVersionCode);
            int minSdkVersion = (int) PlayerSettings.Android.minSdkVersion;
            manifest2.AddUsesSDK(minSdkVersion, targetSdkVersion);
            string applicationIdentifier = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
            if (this.IsValidAndroidBundleIdentifier(applicationIdentifier))
            {
                manifest2.packageName = applicationIdentifier;
            }
            manifest2.Save();
        }

        private bool IsValidAndroidBundleIdentifier(string bundleIdentifier)
        {
            string extraValidChars = "_";
            bool flag = false;
            char[] separator = new char[] { '.' };
            foreach (string str2 in bundleIdentifier.Split(separator))
            {
                if (((str2.Length < 1) || char.IsDigit(str2[0])) || (extraValidChars.IndexOf(str2[0]) != -1))
                {
                    flag = true;
                }
            }
            return (!flag && this.IsValidBundleIdentifier(bundleIdentifier, extraValidChars));
        }

        private bool IsValidBundleIdentifier(string domainish, string extraValidChars)
        {
            if (string.IsNullOrEmpty(domainish))
            {
                return false;
            }
            if ((domainish == "com.Company.ProductName") || (domainish == "com.unity3d.player"))
            {
                return false;
            }
            string str = "a-zA-Z0-9" + extraValidChars;
            Regex regex = new Regex(string.Format("^([{0}]+[.])*[{0}]+[.][{0}]+$", str));
            return regex.IsMatch(domainish);
        }

        private bool IsValidJavaPackageName(string packageName)
        {
            if (!this.IsValidAndroidBundleIdentifier(packageName))
            {
                return false;
            }
            char[] separator = new char[] { '.' };
            foreach (string str in packageName.Split(separator))
            {
                if (ReservedJavaKeywords.Contains<string>(str))
                {
                    return false;
                }
            }
            return true;
        }

        private string MergeManifests(PostProcessorContext context, string targetManifest, string mainManifest)
        {
            AndroidSDKTools tools = context.Get<AndroidSDKTools>("SDKTools");
            AndroidLibraries libraries = context.Get<AndroidLibraries>("AndroidLibraries");
            bool flag = context.Get<bool>("ExportAndroidProject");
            string[] manifestFiles = libraries.GetManifestFiles();
            if (!flag && (manifestFiles.Length > 0))
            {
                tools.MergeManifests(targetManifest, mainManifest, manifestFiles, null);
                return targetManifest;
            }
            FileUtil.CopyFileOrDirectory(mainManifest, targetManifest);
            return targetManifest;
        }

        private string PatchManifest(PostProcessorContext context, string manifest)
        {
            BuildTarget platform = context.Get<BuildTarget>("BuildTarget");
            string[] components = new string[] { this._stagingArea, "assets", "bin" };
            string str = Paths.Combine(components);
            AndroidManifest manifestXML = new AndroidManifest(manifest);
            string location = this.PreferredInstallLocationAsString();
            manifestXML.SetInstallLocation(location);
            manifestXML.SetDebuggable(this._developmentPlayer || Unsupported.IsDeveloperBuild());
            int minSdkVersion = (int) PlayerSettings.Android.minSdkVersion;
            string glEsVersion = "0x00020000";
            GraphicsDeviceType[] graphicsAPIs = PlayerSettings.GetGraphicsAPIs(platform);
            if (graphicsAPIs.Contains<GraphicsDeviceType>(GraphicsDeviceType.OpenGLES3) && (minSdkVersion >= 0x12))
            {
                glEsVersion = "0x00030000";
            }
            if (graphicsAPIs.Contains<GraphicsDeviceType>(GraphicsDeviceType.OpenGLES2))
            {
                glEsVersion = "0x00020000";
            }
            if ((glEsVersion == "0x00030000") && (PlayerSettings.openGLRequireES31 || PlayerSettings.openGLRequireES31AEP))
            {
                glEsVersion = "0x00030001";
            }
            manifestXML.AddGLESVersion(glEsVersion);
            if ((glEsVersion == "0x00030001") && PlayerSettings.openGLRequireES31AEP)
            {
                manifestXML.AddUsesFeature("android.hardware.opengles.aep", true);
            }
            if (graphicsAPIs.Contains<GraphicsDeviceType>(GraphicsDeviceType.Vulkan))
            {
                manifestXML.AddUsesFeature("android.hardware.vulkan", graphicsAPIs.Length == 1);
            }
            if (EditorUserBuildSettings.androidBuildSubtarget != MobileTextureSubtarget.Generic)
            {
                this.CreateSupportsTextureElem(manifestXML, EditorUserBuildSettings.androidBuildSubtarget);
            }
            HashSet<string> activities = new HashSet<string>(this.GetActivitiesWithMetadata(manifestXML, "unityplayer.UnityActivity", "true"));
            string[] other = new string[] { "com.unity3d.player.UnityPlayerNativeActivity", "com.unity3d.player.UnityPlayerActivity", "com.unity3d.player.UnityPlayerProxyActivity" };
            activities.UnionWith(other);
            string orientationAttr = this.GetOrientationAttr();
            bool flag = false;
            foreach (string str5 in activities)
            {
                flag = manifestXML.SetOrientation(str5, orientationAttr) || flag;
                flag = manifestXML.SetLaunchMode(str5, "singleTask") || flag;
                flag = manifestXML.SetConfigChanges(str5, AndroidManifest.AndroidConfigChanges) || flag;
            }
            if (!flag)
            {
                UnityEngine.Debug.LogWarning($"Unable to find unity activity in manifest. You need to make sure orientation attribute is set to {orientationAttr} manually.");
            }
            manifestXML.SetApplicationFlag("isGame", PlayerSettings.Android.androidIsGame);
            if (PlayerSettings.Android.androidBannerEnabled)
            {
                manifestXML.SetApplicationBanner("@drawable/app_banner");
            }
            if ((PlayerSettings.Android.androidTVCompatibility && !manifestXML.HasLeanbackLauncherActivity()) && !manifestXML.AddLeanbackLauncherActivity())
            {
                UnityEngine.Debug.LogWarning("No activity with LEANBACK_LAUNCHER or LAUNCHER categories found.\nThe build may not be compatible with Android TV. Specify an activity with LEANBACK_LAUNCHER or LAUNCHER category in the manifest, or disable Android TV compatibility in Player Settings.");
            }
            switch (PlayerSettings.Android.androidGamepadSupportLevel)
            {
                case AndroidGamepadSupportLevel.SupportsGamepad:
                    manifestXML.AddUsesFeature("android.hardware.gamepad", false);
                    break;

                case AndroidGamepadSupportLevel.RequiresGamepad:
                    manifestXML.AddUsesFeature("android.hardware.gamepad", true);
                    break;
            }
            if (PlayerSettings.virtualRealitySupported)
            {
                this.AddVRRelatedManifestEntries(context, manifestXML, activities);
            }
            AssemblyReferenceChecker checker = new AssemblyReferenceChecker();
            bool collectMethods = true;
            bool ignoreSystemDlls = true;
            string[] textArray3 = new string[] { str, "Data", "Managed" };
            string path = Paths.Combine(textArray3);
            checker.CollectReferences(path, collectMethods, 0f, ignoreSystemDlls);
            this.SetPermissionAttributes(context, manifestXML, checker);
            manifestXML.StripUnityLibEntryForNativeActitivy();
            manifestXML.SaveAs(manifest);
            return manifestXML.packageName;
        }

        private string PreferredInstallLocationAsString()
        {
            AndroidPreferredInstallLocation preferredInstallLocation = PlayerSettings.Android.preferredInstallLocation;
            if (preferredInstallLocation != AndroidPreferredInstallLocation.Auto)
            {
                if (preferredInstallLocation == AndroidPreferredInstallLocation.PreferExternal)
                {
                    return "preferExternal";
                }
                if (preferredInstallLocation == AndroidPreferredInstallLocation.ForceInternal)
                {
                    return "internalOnly";
                }
            }
            else
            {
                return "auto";
            }
            return "preferExternal";
        }

        private void SetPermissionAttributes(PostProcessorContext context, AndroidManifest manifestXML, AssemblyReferenceChecker checker)
        {
            if ((this._developmentPlayer || PlayerSettings.Android.forceInternetPermission) || this.doesReferenceNetworkClasses(checker))
            {
                manifestXML.AddUsesPermission("android.permission.INTERNET");
            }
            if (checker.HasReferenceToMethod("UnityEngine.Handheld::Vibrate"))
            {
                manifestXML.AddUsesPermission("android.permission.VIBRATE");
            }
            if (checker.HasReferenceToMethod("UnityEngine.iPhoneSettings::get_internetReachability") || checker.HasReferenceToMethod("UnityEngine.Application::get_internetReachability"))
            {
                manifestXML.AddUsesPermission("android.permission.ACCESS_NETWORK_STATE");
            }
            if (((checker.HasReferenceToMethod("UnityEngine.Input::get_location") || checker.HasReferenceToMethod("UnityEngine.iPhoneInput::get_lastLocation")) || (checker.HasReferenceToMethod("UnityEngine.iPhoneSettings::get_locationServiceStatus") || checker.HasReferenceToMethod("UnityEngine.iPhoneSettings::get_locationServiceEnabledByUser"))) || (checker.HasReferenceToMethod("UnityEngine.iPhoneSettings::StartLocationServiceUpdates") || checker.HasReferenceToMethod("UnityEngine.iPhoneSettings::StopLocationServiceUpdates")))
            {
                manifestXML.AddUsesPermission("android.permission.ACCESS_FINE_LOCATION");
                manifestXML.AddUsesFeature("android.hardware.location.gps", false);
                manifestXML.AddUsesFeature("android.hardware.location", false);
            }
            if (checker.HasReferenceToType("UnityEngine.WebCamTexture"))
            {
                manifestXML.AddUsesPermission("android.permission.CAMERA");
                manifestXML.AddUsesFeature("android.hardware.camera", false);
                manifestXML.AddUsesFeature("android.hardware.camera.autofocus", false);
                manifestXML.AddUsesFeature("android.hardware.camera.front", false);
            }
            if (checker.HasReferenceToType("UnityEngine.Microphone"))
            {
                manifestXML.AddUsesPermission("android.permission.RECORD_AUDIO");
                manifestXML.AddUsesFeature("android.hardware.microphone", false);
            }
            if (PlayerSettings.Android.forceSDCardPermission)
            {
                manifestXML.AddUsesPermission("android.permission.WRITE_EXTERNAL_STORAGE");
            }
            else if (this._developmentPlayer)
            {
                manifestXML.AddUsesPermission("android.permission.WRITE_EXTERNAL_STORAGE", 0x12);
                manifestXML.AddUsesPermission("android.permission.READ_EXTERNAL_STORAGE", 0x12);
            }
            if ((checker.HasReferenceToMethod("UnityEngine.Input::get_acceleration") || checker.HasReferenceToMethod("UnityEngine.Input::GetAccelerationEvent")) || (checker.HasReferenceToMethod("UnityEngine.Input::get_accelerationEvents") || checker.HasReferenceToMethod("UnityEngine.Input::get_accelerationEventCount")))
            {
                manifestXML.AddUsesFeature("android.hardware.sensor.accelerometer", false);
            }
            manifestXML.AddUsesFeature("android.hardware.touchscreen", false);
            if (((checker.HasReferenceToMethod("UnityEngine.Input::get_touches") || checker.HasReferenceToMethod("UnityEngine.Input::GetTouch")) || (checker.HasReferenceToMethod("UnityEngine.Input::get_touchCount") || checker.HasReferenceToMethod("UnityEngine.Input::get_multiTouchEnabled"))) || (((checker.HasReferenceToMethod("UnityEngine.Input::set_multiTouchEnabled") || checker.HasReferenceToMethod("UnityEngine.iPhoneInput::get_touches")) || (checker.HasReferenceToMethod("UnityEngine.iPhoneInput::GetTouch") || checker.HasReferenceToMethod("UnityEngine.iPhoneInput::get_touchCount"))) || (checker.HasReferenceToMethod("UnityEngine.iPhoneInput::get_multiTouchEnabled") || checker.HasReferenceToMethod("UnityEngine.iPhoneInput::set_multiTouchEnabled"))))
            {
                manifestXML.AddUsesFeature("android.hardware.touchscreen.multitouch", false);
                manifestXML.AddUsesFeature("android.hardware.touchscreen.multitouch.distinct", false);
            }
            if ((checker.HasReferenceToMethod("UnityEngine.iPhoneInput::get_acceleration") || checker.HasReferenceToMethod("UnityEngine.iPhoneInput::GetAccelerationEvent")) || (checker.HasReferenceToMethod("UnityEngine.iPhoneInput::get_accelerationEvents") || checker.HasReferenceToMethod("UnityEngine.iPhoneInput::get_accelerationEventCount")))
            {
                manifestXML.AddUsesFeature("android.hardware.sensor.accelerometer", false);
            }
            if (checker.HasReferenceToMethod("UnityEngine.iPhoneUtils::Vibrate"))
            {
                manifestXML.AddUsesPermission("android.permission.VIBRATE");
            }
        }

        private void ThrowIfInvalid(string packageName)
        {
            if (!this.IsValidAndroidBundleIdentifier(packageName))
            {
                string message = "Please set the Bundle Identifier in the Player Settings.";
                message = (message + " The value must follow the convention 'com.YourCompanyName.YourProductName'") + " and can contain alphanumeric characters and underscore." + "\nEach segment must not start with a numeric character or underscore.";
                Selection.activeObject = Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings");
                CancelPostProcess.AbortBuild("Bundle Identifier has not been set up correctly", message, null);
            }
            if (!this.IsValidJavaPackageName(packageName))
            {
                UnityEngine.Debug.LogWarning((("As of Unity 4.2 the restrictions on the Bundle Identifier has been updated " + " to include those for java package names. Specifically the" + " restrictions have been updated regarding reserved java keywords.") + "\n" + "\nhttp://docs.oracle.com/javase/tutorial/java/package/namingpkgs.html") + "\nhttp://docs.oracle.com/javase/specs/jls/se7/html/jls-3.html#jls-3.9" + "\n");
            }
        }

        public string Name =>
            "Creating Android manifest";
    }
}

