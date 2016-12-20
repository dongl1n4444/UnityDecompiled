namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.Android;
    using UnityEditor.Android.PostProcessor;
    using UnityEngine;
    using UnityEngine.Rendering;

    internal class CheckDevice : IPostProcessorTask
    {
        public event ProgressHandler OnProgress;

        public void Execute(PostProcessorContext context)
        {
            if (context.Get<bool>("AutoRunPlayer"))
            {
                AndroidDevice device = null;
                if (this.OnProgress != null)
                {
                    this.OnProgress(this, "Getting list of attached devices");
                }
                device = this.FindDevice(context);
                context.Set<AndroidDevice>("AndroidDevice", device);
            }
        }

        private AndroidDevice FindDevice(PostProcessorContext context)
        {
            BuildTarget platform = context.Get<BuildTarget>("BuildTarget");
            AndroidTargetDevice targetDevice = PlayerSettings.Android.targetDevice;
            List<string> list = null;
            do
            {
                list = ADB.Devices(null);
            }
            while ((list.Count == 0) && EditorUtility.DisplayDialog("No Android device found!", " * Make sure USB debugging has been enabled\n * Check your device, in most cases there should be a small icon in the status bar telling you if the USB connection is up.\n * If you are sure that device is attached then it might be USB driver problem, for details please check Android SDK Setup section in Unity manual.", "Retry", "Cancel"));
            if (list.Count < 1)
            {
                string message = string.Format("No Android devices found.{0}\n", (Application.platform != RuntimePlatform.WindowsEditor) ? "" : " If you are sure that device is attached then it might be USB driver problem, for details please check Android SDK Setup section in Unity manual.");
                CancelPostProcess.AbortBuild("Couldn't find Android device", message);
            }
            AndroidDevice device2 = new AndroidDevice(list[0]);
            int num = Convert.ToInt32(device2.Properties["ro.build.version.sdk"]);
            if (num < 9)
            {
                string str2 = (("Device: " + device2.Describe() + "\n") + "The connected device is not running Android OS 2.3 or later.") + " Unity Android does not support earlier versions of the Android OS;" + " please upgrade your device to a later OS version.";
                Selection.activeObject = Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings");
                CancelPostProcess.AbortBuild("Device software is not supported", str2);
            }
            int num2 = 0;
            try
            {
                num2 = Convert.ToInt32(device2.Properties["ro.opengles.version"]);
            }
            catch (FormatException)
            {
                num2 = -1;
            }
            int num3 = 0xf0000;
            GraphicsDeviceType[] graphicsAPIs = PlayerSettings.GetGraphicsAPIs(platform);
            if (Enumerable.Contains<GraphicsDeviceType>(graphicsAPIs, GraphicsDeviceType.OpenGLES3))
            {
                num3 = 0x30000;
            }
            if (Enumerable.Contains<GraphicsDeviceType>(graphicsAPIs, GraphicsDeviceType.OpenGLES2))
            {
                num3 = 0x20000;
            }
            bool flag = device2.Features.Contains("android.hardware.opengles.aep");
            if ((num3 == 0x30000) && (PlayerSettings.openGLRequireES31 || PlayerSettings.openGLRequireES31AEP))
            {
                num3 = 0x30001;
            }
            bool flag2 = true;
            bool flag3 = (graphicsAPIs.Length == 1) && (graphicsAPIs[0] == GraphicsDeviceType.Vulkan);
            if ("Amazon" != device2.Properties["ro.product.brand"])
            {
                string str3 = null;
                if (flag3 && !flag2)
                {
                    str3 = "The connected device does not support Vulkan.";
                    str3 = str3 + " Please select OpenGLES under Player Settings instead.";
                }
                if (((num2 >= 0) && (num2 < num3)) || (PlayerSettings.openGLRequireES31AEP && !flag))
                {
                    str3 = "The connected device is not compatible with the selected OpenGLES version.";
                    str3 = str3 + " Please select a lower OpenGLES version under Player Settings instead.";
                }
                if (str3 != null)
                {
                    Selection.activeObject = Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings");
                    CancelPostProcess.AbortBuild("Device hardware is not supported", str3);
                }
            }
            if ((targetDevice == AndroidTargetDevice.x86) && device2.Properties["ro.product.cpu.abi"].Equals("armeabi-v7a"))
            {
                string str4 = "You are trying to install x86 APK to ARM device. ";
                str4 = str4 + "Please select FAT or ARM as device filter under Player Settings, or connect a x86 device.";
                Selection.activeObject = Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings");
                CancelPostProcess.AbortBuild("Device hardware is not supported", str4);
            }
            string str5 = device2.Properties["ro.product.manufacturer"];
            string str6 = device2.Properties["ro.product.model"];
            string action = device2.Properties["ro.product.cpu.abi"];
            bool flag4 = device2.Properties["ro.secure"].Equals("1");
            string str8 = string.Format("{0}{1} {2}", char.ToUpper(str5[0]), str5.Substring(1), str6);
            string label = string.Format("Android API-{0}", num);
            UsabilityAnalytics.Event("Android Device", str8, label, !flag4 ? 0 : 1);
            string str10 = string.Format("gles {0}.{1}{2}", num2 >> 0x10, num2 & 0xffff, !flag ? "" : " AEP");
            if (num2 < 0)
            {
                str10 = "gles 2.0";
            }
            UsabilityAnalytics.Event("Android Architecture", action, str10, 1);
            string str11 = device2.Properties["ro.board.platform"];
            ulong i = device2.MemInfo["MemTotal"];
            i = UpperboundPowerOf2(i) / ((ulong) 0x100000L);
            UsabilityAnalytics.Event("Android Chipset", str11, string.Format("{0}MB", i), 1);
            return device2;
        }

        private static ulong UpperboundPowerOf2(ulong i)
        {
            i -= (ulong) 1L;
            i |= i >> 1;
            i |= i >> 2;
            i |= i >> 4;
            i |= i >> 8;
            i |= i >> 0x10;
            i |= i >> 0x20;
            return (i + ((ulong) 1L));
        }

        public string Name
        {
            get
            {
                return "Trying to find a suitable Android device";
            }
        }
    }
}

