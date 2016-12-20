namespace UnityEditor.WSA
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor;

    [Extension]
    internal static class MetroPluginImporterExtensions
    {
        [Extension]
        internal static bool IsAnySDK(PluginImporter imp)
        {
            return (IsSDK(imp, "") || IsSDK(imp, "AnySDK"));
        }

        [Extension]
        private static bool IsCPUArchitecture(PluginImporter imp, string cpu)
        {
            return (string.Compare(imp.GetPlatformData(platformName, Plugin.cpuTag), cpu) == 0);
        }

        [Extension]
        internal static bool IsCPUArchitectureAnyCPU(PluginImporter imp)
        {
            return (IsCPUArchitecture(imp, "") || IsCPUArchitecture(imp, "AnyCPU"));
        }

        [Extension]
        internal static bool IsCPUArchitectureARM(PluginImporter imp)
        {
            return IsCPUArchitecture(imp, "ARM");
        }

        [Extension]
        internal static bool IsCPUArchitectureX64(PluginImporter imp)
        {
            return IsCPUArchitecture(imp, "X64");
        }

        [Extension]
        internal static bool IsCPUArchitectureX86(PluginImporter imp)
        {
            return IsCPUArchitecture(imp, "X86");
        }

        [Extension]
        internal static bool IsPhoneSDK81(PluginImporter imp)
        {
            return IsSDK(imp, "PhoneSDK81");
        }

        [Extension]
        private static bool IsSDK(PluginImporter imp, string sdk)
        {
            return (string.Compare(imp.GetPlatformData(platformName, Plugin.sdkTag), sdk) == 0);
        }

        [Extension]
        internal static bool IsSDK80(PluginImporter imp)
        {
            return IsSDK(imp, "SDK80");
        }

        [Extension]
        internal static bool IsSDK81(PluginImporter imp)
        {
            return IsSDK(imp, "SDK81");
        }

        [Extension]
        internal static bool IsUniversalSDK81(PluginImporter imp)
        {
            return IsSDK(imp, "UniversalSDK81");
        }

        [Extension]
        internal static bool IsUWP(PluginImporter imp)
        {
            return IsSDK(imp, "UWP");
        }

        internal static string platformName
        {
            get
            {
                return BuildPipeline.GetBuildTargetName(BuildTarget.WSAPlayer);
            }
        }
    }
}

