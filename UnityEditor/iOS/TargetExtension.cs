namespace UnityEditor.iOS
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Hardware;
    using UnityEditor.Modules;
    using UnityEditorInternal;
    using UnityEngine;

    internal class TargetExtension : DefaultPlatformSupportModule
    {
        [CompilerGenerated]
        private static Usb.OnDevicesChangedHandler <>f__mg$cache0;
        [CompilerGenerated]
        private static Usb.OnDevicesChangedHandler <>f__mg$cache1;
        internal const string AppleTVTargetName = "tvOS";
        private string[] assemblyReferencesForUserScripts = new string[] { Path.Combine(UnityEditor.BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.iOS, BuildOptions.CompressTextures), "UnityEditor.iOS.Extensions.Xcode.dll"), Path.Combine(UnityEditor.BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.iOS, BuildOptions.CompressTextures), "UnityEditor.iOS.Extensions.Common.dll") };
        private iOSBuildWindowExtension buildWindow;
        internal static bool debugUsbDevices = false;
        private const DevDeviceFeatures deviceFeatures = (DevDeviceFeatures.RemoteConnection | DevDeviceFeatures.PlayerConnection);
        internal const string iOSTargetName = "iOS";
        private iOSPluginImporterExtension m_pluginImporterExtension;
        private iOSPreferenceWindowExtension m_preferenceWindowExtension;
        internal static Version MinimumOsVersion = new Version("6.0");
        internal const string nativeDll = "__Internal";
        private string[] nativeLibraries;
        private iOSSettingsEditorExtension settingsEditor;

        public TargetExtension()
        {
            string playbackEngineDirectory = UnityEditor.BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.iOS, BuildOptions.CompressTextures);
            string str2 = "UnityEditor.iOS.Native";
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                str2 = Path.Combine((IntPtr.Size != 4) ? "x86_64" : "x86", str2);
            }
            this.nativeLibraries = new string[] { Path.Combine(playbackEngineDirectory, str2) };
        }

        public override IBuildPostprocessor CreateBuildPostprocessor()
        {
            PostProcessorSettings settings = new PostProcessorSettings {
                OsName = "iOS",
                MinimumOsVersion = MinimumOsVersion
            };
            return new iOSBuildPostprocessor(settings);
        }

        public override IBuildWindowExtension CreateBuildWindowExtension()
        {
            if (this.buildWindow == null)
            {
                this.buildWindow = new iOSBuildWindowExtension();
            }
            return this.buildWindow;
        }

        public override IDevice CreateDevice(string deviceId)
        {
            foreach (iOSDevice device in ExtensionData.Devices)
            {
                if (device.Id == deviceId)
                {
                    return device;
                }
            }
            iOSDevice item = new iOSDevice(deviceId);
            ExtensionData.Devices.Add(item);
            return item;
        }

        public override IPluginImporterExtension CreatePluginImporterExtension()
        {
            if (this.m_pluginImporterExtension == null)
            {
                this.m_pluginImporterExtension = new iOSPluginImporterExtension();
            }
            return this.m_pluginImporterExtension;
        }

        public override IPreferenceWindowExtension CreatePreferenceWindowExtension()
        {
            if (this.m_preferenceWindowExtension == null)
            {
                this.m_preferenceWindowExtension = new iOSPreferenceWindowExtension();
            }
            return this.m_preferenceWindowExtension;
        }

        public override IScriptingImplementations CreateScriptingImplementations() => 
            new iOSScriptingImplementations();

        public override ISettingEditorExtension CreateSettingsEditorExtension()
        {
            if (this.settingsEditor == null)
            {
                this.settingsEditor = new iOSSettingsEditorExtension();
            }
            return this.settingsEditor;
        }

        public override void OnLoad()
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new Usb.OnDevicesChangedHandler(TargetExtension.OnUsbDevicesChanged);
            }
            Usb.DevicesChanged += <>f__mg$cache0;
        }

        public override void OnUnload()
        {
            if (<>f__mg$cache1 == null)
            {
                <>f__mg$cache1 = new Usb.OnDevicesChangedHandler(TargetExtension.OnUsbDevicesChanged);
            }
            Usb.DevicesChanged -= <>f__mg$cache1;
        }

        public static void OnUsbDevicesChanged(UsbDevice[] usbDevices)
        {
            List<DevDevice> list = new List<DevDevice>();
            foreach (UsbDevice device in usbDevices)
            {
                if (debugUsbDevices)
                {
                    Debug.Log(device);
                }
                string deviceType = Utils.GetDeviceType(device.name);
                if (deviceType != null)
                {
                    iOSDeviceDescription description;
                    if (Utils.FindiOSDevice(device.vendorId, device.productId, device.revision, deviceType, out description))
                    {
                        string[] textArray1 = new string[] { description.type, " ", description.model, " (", Utils.MakeShortUdid(device.udid), ")" };
                        string name = string.Concat(textArray1);
                        DevDevice item = new DevDevice(device.udid, name, "iOS", "iOS", DevDeviceState.Connected, DevDeviceFeatures.RemoteConnection | DevDeviceFeatures.PlayerConnection);
                        list.Add(item);
                        if (debugUsbDevices)
                        {
                            Debug.Log(item);
                        }
                    }
                    else if ((device.name.Contains("iPhone") || device.name.Contains("iPad")) || device.name.Contains("iPod"))
                    {
                        string str3 = device.productId.ToString("X");
                        string str4 = Utils.MakeShortUdid(device.udid);
                        string str5 = device.revision.ToString("X4");
                        string[] textArray2 = new string[] { "Unknown: ", device.name, " (pid: ", str3, ", rev: ", str5, ") (", str4, ")" };
                        string str6 = string.Concat(textArray2);
                        DevDevice device3 = new DevDevice(device.udid, str6, device.name, "iOS", DevDeviceState.Connected, DevDeviceFeatures.RemoteConnection | DevDeviceFeatures.PlayerConnection);
                        list.Add(device3);
                        if (debugUsbDevices)
                        {
                            Debug.Log(device3);
                        }
                    }
                }
            }
            DevDeviceList.Update("iOS", list.ToArray());
        }

        public override string[] AssemblyReferencesForEditorCsharpProject =>
            this.assemblyReferencesForUserScripts;

        public override string[] AssemblyReferencesForUserScripts =>
            this.assemblyReferencesForUserScripts;

        public override string JamTarget =>
            "iOSEditorExtensions";

        public override string[] NativeLibraries =>
            this.nativeLibraries;

        public override string TargetName =>
            "iOS";
    }
}

