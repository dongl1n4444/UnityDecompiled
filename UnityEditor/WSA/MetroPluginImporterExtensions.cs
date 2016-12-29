namespace UnityEditor.WSA
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor;

    internal static class MetroPluginImporterExtensions
    {
        internal static bool IsAnySDK(this PluginImporter imp) => 
            (imp.IsSDK("") || imp.IsSDK("AnySDK"));

        private static bool IsCPUArchitecture(this PluginImporter imp, string cpu) => 
            (string.Compare(imp.GetPlatformData(platformName, Plugin.cpuTag), cpu) == 0);

        internal static bool IsCPUArchitectureAnyCPU(this PluginImporter imp) => 
            (imp.IsCPUArchitecture("") || imp.IsCPUArchitecture("AnyCPU"));

        internal static bool IsCPUArchitectureARM(this PluginImporter imp) => 
            imp.IsCPUArchitecture("ARM");

        internal static bool IsCPUArchitectureX64(this PluginImporter imp) => 
            imp.IsCPUArchitecture("X64");

        internal static bool IsCPUArchitectureX86(this PluginImporter imp) => 
            imp.IsCPUArchitecture("X86");

        internal static bool IsPhoneSDK81(this PluginImporter imp) => 
            imp.IsSDK("PhoneSDK81");

        private static bool IsSDK(this PluginImporter imp, string sdk) => 
            (string.Compare(imp.GetPlatformData(platformName, Plugin.sdkTag), sdk) == 0);

        internal static bool IsSDK80(this PluginImporter imp) => 
            imp.IsSDK("SDK80");

        internal static bool IsSDK81(this PluginImporter imp) => 
            imp.IsSDK("SDK81");

        internal static bool IsUniversalSDK81(this PluginImporter imp) => 
            imp.IsSDK("UniversalSDK81");

        internal static bool IsUWP(this PluginImporter imp) => 
            imp.IsSDK("UWP");

        internal static string platformName =>
            BuildPipeline.GetBuildTargetName(BuildTarget.WSAPlayer);
    }
}

