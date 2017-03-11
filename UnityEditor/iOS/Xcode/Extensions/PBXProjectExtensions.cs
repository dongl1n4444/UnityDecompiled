namespace UnityEditor.iOS.Xcode.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditor.iOS.Xcode;

    public static class PBXProjectExtensions
    {
        internal static FlagList appExtensionDebugBuildFlags;
        internal static FlagList appExtensionReleaseBuildFlags;
        internal static FlagList watchAppDebugBuildFlags;
        internal static FlagList watchAppReleaseBuildFlags;
        internal static FlagList watchExtensionDebugBuildFlags;
        internal static FlagList watchExtensionReleaseBuildFlags;

        static PBXProjectExtensions()
        {
            FlagList list = new FlagList();
            list.Add("ALWAYS_SEARCH_USER_PATHS", "NO");
            list.Add("CLANG_CXX_LANGUAGE_STANDARD", "gnu++0x");
            list.Add("CLANG_CXX_LIBRARY", "libc++");
            list.Add("CLANG_ENABLE_MODULES", "YES");
            list.Add("CLANG_ENABLE_OBJC_ARC", "YES");
            list.Add("CLANG_WARN_BOOL_CONVERSION", "YES");
            list.Add("CLANG_WARN_CONSTANT_CONVERSION", "YES");
            list.Add("CLANG_WARN_DIRECT_OBJC_ISA_USAGE", "YES_ERROR");
            list.Add("CLANG_WARN_EMPTY_BODY", "YES");
            list.Add("CLANG_WARN_ENUM_CONVERSION", "YES");
            list.Add("CLANG_WARN_INT_CONVERSION", "YES");
            list.Add("CLANG_WARN_OBJC_ROOT_CLASS", "YES_ERROR");
            list.Add("CLANG_WARN_UNREACHABLE_CODE", "YES");
            list.Add("CLANG_WARN__DUPLICATE_METHOD_MATCH", "YES");
            list.Add("COPY_PHASE_STRIP", "YES");
            list.Add("ENABLE_NS_ASSERTIONS", "NO");
            list.Add("ENABLE_STRICT_OBJC_MSGSEND", "YES");
            list.Add("GCC_C_LANGUAGE_STANDARD", "gnu99");
            list.Add("GCC_WARN_64_TO_32_BIT_CONVERSION", "YES");
            list.Add("GCC_WARN_ABOUT_RETURN_TYPE", "YES_ERROR");
            list.Add("GCC_WARN_UNDECLARED_SELECTOR", "YES");
            list.Add("GCC_WARN_UNINITIALIZED_AUTOS", "YES_AGGRESSIVE");
            list.Add("GCC_WARN_UNUSED_FUNCTION", "YES");
            list.Add("IPHONEOS_DEPLOYMENT_TARGET", "8.0");
            list.Add("LD_RUNPATH_SEARCH_PATHS", "$(inherited)");
            list.Add("LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");
            list.Add("LD_RUNPATH_SEARCH_PATHS", "@executable_path/../../Frameworks");
            list.Add("MTL_ENABLE_DEBUG_INFO", "NO");
            list.Add("PRODUCT_NAME", "$(TARGET_NAME)");
            list.Add("SKIP_INSTALL", "YES");
            list.Add("VALIDATE_PRODUCT", "YES");
            appExtensionReleaseBuildFlags = list;
            list = new FlagList();
            list.Add("ALWAYS_SEARCH_USER_PATHS", "NO");
            list.Add("CLANG_CXX_LANGUAGE_STANDARD", "gnu++0x");
            list.Add("CLANG_CXX_LIBRARY", "libc++");
            list.Add("CLANG_ENABLE_MODULES", "YES");
            list.Add("CLANG_ENABLE_OBJC_ARC", "YES");
            list.Add("CLANG_WARN_BOOL_CONVERSION", "YES");
            list.Add("CLANG_WARN_CONSTANT_CONVERSION", "YES");
            list.Add("CLANG_WARN_DIRECT_OBJC_ISA_USAGE", "YES_ERROR");
            list.Add("CLANG_WARN_EMPTY_BODY", "YES");
            list.Add("CLANG_WARN_ENUM_CONVERSION", "YES");
            list.Add("CLANG_WARN_INT_CONVERSION", "YES");
            list.Add("CLANG_WARN_OBJC_ROOT_CLASS", "YES_ERROR");
            list.Add("CLANG_WARN_UNREACHABLE_CODE", "YES");
            list.Add("CLANG_WARN__DUPLICATE_METHOD_MATCH", "YES");
            list.Add("COPY_PHASE_STRIP", "NO");
            list.Add("ENABLE_STRICT_OBJC_MSGSEND", "YES");
            list.Add("GCC_C_LANGUAGE_STANDARD", "gnu99");
            list.Add("GCC_DYNAMIC_NO_PIC", "NO");
            list.Add("GCC_OPTIMIZATION_LEVEL", "0");
            list.Add("GCC_PREPROCESSOR_DEFINITIONS", "DEBUG=1");
            list.Add("GCC_PREPROCESSOR_DEFINITIONS", "$(inherited)");
            list.Add("GCC_SYMBOLS_PRIVATE_EXTERN", "NO");
            list.Add("GCC_WARN_64_TO_32_BIT_CONVERSION", "YES");
            list.Add("GCC_WARN_ABOUT_RETURN_TYPE", "YES_ERROR");
            list.Add("GCC_WARN_UNDECLARED_SELECTOR", "YES");
            list.Add("GCC_WARN_UNINITIALIZED_AUTOS", "YES_AGGRESSIVE");
            list.Add("GCC_WARN_UNUSED_FUNCTION", "YES");
            list.Add("IPHONEOS_DEPLOYMENT_TARGET", "8.0");
            list.Add("LD_RUNPATH_SEARCH_PATHS", "$(inherited)");
            list.Add("LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");
            list.Add("LD_RUNPATH_SEARCH_PATHS", "@executable_path/../../Frameworks");
            list.Add("MTL_ENABLE_DEBUG_INFO", "YES");
            list.Add("ONLY_ACTIVE_ARCH", "YES");
            list.Add("PRODUCT_NAME", "$(TARGET_NAME)");
            list.Add("SKIP_INSTALL", "YES");
            appExtensionDebugBuildFlags = list;
            list = new FlagList();
            list.Add("ASSETCATALOG_COMPILER_COMPLICATION_NAME", "Complication");
            list.Add("CLANG_ANALYZER_NONNULL", "YES");
            list.Add("CLANG_WARN_DOCUMENTATION_COMMENTS", "YES");
            list.Add("CLANG_WARN_INFINITE_RECURSION", "YES");
            list.Add("CLANG_WARN_SUSPICIOUS_MOVE", "YES");
            list.Add("DEBUG_INFORMATION_FORMAT", "dwarf-with-dsym");
            list.Add("GCC_NO_COMMON_BLOCKS", "YES");
            list.Add("LD_RUNPATH_SEARCH_PATHS", "$(inherited)");
            list.Add("LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");
            list.Add("LD_RUNPATH_SEARCH_PATHS", "@executable_path/../../Frameworks");
            list.Add("PRODUCT_NAME", "${TARGET_NAME}");
            list.Add("SDKROOT", "watchos");
            list.Add("SKIP_INSTALL", "YES");
            list.Add("TARGETED_DEVICE_FAMILY", "4");
            list.Add("WATCHOS_DEPLOYMENT_TARGET", "3.1");
            list.Add("ARCHS", "$(ARCHS_STANDARD)");
            list.Add("SUPPORTED_PLATFORMS", "watchos");
            list.Add("SUPPORTED_PLATFORMS", "watchsimulator");
            watchExtensionReleaseBuildFlags = list;
            list = new FlagList();
            list.Add("ASSETCATALOG_COMPILER_COMPLICATION_NAME", "Complication");
            list.Add("CLANG_ANALYZER_NONNULL", "YES");
            list.Add("CLANG_WARN_DOCUMENTATION_COMMENTS", "YES");
            list.Add("CLANG_WARN_INFINITE_RECURSION", "YES");
            list.Add("CLANG_WARN_SUSPICIOUS_MOVE", "YES");
            list.Add("DEBUG_INFORMATION_FORMAT", "dwarf");
            list.Add("ENABLE_TESTABILITY", "YES");
            list.Add("GCC_NO_COMMON_BLOCKS", "YES");
            list.Add("LD_RUNPATH_SEARCH_PATHS", "$(inherited)");
            list.Add("LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");
            list.Add("LD_RUNPATH_SEARCH_PATHS", "@executable_path/../../Frameworks");
            list.Add("PRODUCT_NAME", "${TARGET_NAME}");
            list.Add("SDKROOT", "watchos");
            list.Add("SKIP_INSTALL", "YES");
            list.Add("TARGETED_DEVICE_FAMILY", "4");
            list.Add("WATCHOS_DEPLOYMENT_TARGET", "3.1");
            list.Add("ARCHS", "$(ARCHS_STANDARD)");
            list.Add("SUPPORTED_PLATFORMS", "watchos");
            list.Add("SUPPORTED_PLATFORMS", "watchsimulator");
            watchExtensionDebugBuildFlags = list;
            list = new FlagList();
            list.Add("ASSETCATALOG_COMPILER_APPICON_NAME", "AppIcon");
            list.Add("CLANG_ANALYZER_NONNULL", "YES");
            list.Add("CLANG_WARN_DOCUMENTATION_COMMENTS", "YES");
            list.Add("CLANG_WARN_INFINITE_RECURSION", "YES");
            list.Add("CLANG_WARN_SUSPICIOUS_MOVE", "YES");
            list.Add("DEBUG_INFORMATION_FORMAT", "dwarf-with-dsym");
            list.Add("GCC_NO_COMMON_BLOCKS", "YES");
            list.Add("PRODUCT_NAME", "$(TARGET_NAME)");
            list.Add("SDKROOT", "watchos");
            list.Add("SKIP_INSTALL", "YES");
            list.Add("TARGETED_DEVICE_FAMILY", "4");
            list.Add("WATCHOS_DEPLOYMENT_TARGET", "3.1");
            list.Add("ARCHS", "$(ARCHS_STANDARD)");
            list.Add("SUPPORTED_PLATFORMS", "watchos");
            list.Add("SUPPORTED_PLATFORMS", "watchsimulator");
            watchAppReleaseBuildFlags = list;
            list = new FlagList();
            list.Add("ASSETCATALOG_COMPILER_APPICON_NAME", "AppIcon");
            list.Add("CLANG_ANALYZER_NONNULL", "YES");
            list.Add("CLANG_WARN_DOCUMENTATION_COMMENTS", "YES");
            list.Add("CLANG_WARN_INFINITE_RECURSION", "YES");
            list.Add("CLANG_WARN_SUSPICIOUS_MOVE", "YES");
            list.Add("DEBUG_INFORMATION_FORMAT", "dwarf");
            list.Add("ENABLE_TESTABILITY", "YES");
            list.Add("GCC_NO_COMMON_BLOCKS", "YES");
            list.Add("PRODUCT_NAME", "$(TARGET_NAME)");
            list.Add("SDKROOT", "watchos");
            list.Add("SKIP_INSTALL", "YES");
            list.Add("TARGETED_DEVICE_FAMILY", "4");
            list.Add("WATCHOS_DEPLOYMENT_TARGET", "3.1");
            list.Add("ARCHS", "$(ARCHS_STANDARD)");
            list.Add("SUPPORTED_PLATFORMS", "watchos");
            list.Add("SUPPORTED_PLATFORMS", "watchsimulator");
            watchAppDebugBuildFlags = list;
        }

        internal static string AddAppExtension(this PBXProject proj, string mainTarget, string name, string infoPlistPath)
        {
            string ext = ".appex";
            string targetGuid = proj.AddTarget(name, ext, "com.apple.product-type.app-extension");
            string configGuid = proj.AddBuildConfigForTarget(targetGuid, "Debug");
            string str4 = proj.AddBuildConfigForTarget(targetGuid, "Release");
            proj.SetDefaultAppExtensionDebugBuildFlags(configGuid);
            proj.SetDefaultAppExtensionReleaseBuildFlags(str4);
            string[] configGuids = new string[] { configGuid, str4 };
            proj.SetBuildPropertyForConfig(configGuids, "INFOPLIST_FILE", infoPlistPath);
            proj.AddSourcesBuildPhase(targetGuid);
            proj.AddResourcesBuildPhase(targetGuid);
            proj.AddFrameworksBuildPhase(targetGuid);
            string sectionGuid = proj.AddCopyFilesBuildPhase(mainTarget, "Embed App Extensions", "", "13");
            proj.AddFileToBuildSection(mainTarget, sectionGuid, proj.GetTargetProductFileRef(targetGuid));
            proj.AddTargetDependency(mainTarget, targetGuid);
            return targetGuid;
        }

        public static string AddWatchApp(this PBXProject proj, string mainTargetGuid, string watchExtensionTargetGuid, string name, string bundleId, string infoPlistPath)
        {
            string targetGuid = proj.AddTarget(name, ".app", "com.apple.product-type.application.watchapp2");
            string configGuid = proj.AddBuildConfigForTarget(targetGuid, "Debug");
            string str3 = proj.AddBuildConfigForTarget(targetGuid, "Release");
            proj.SetDefaultWatchAppDebugBuildFlags(configGuid);
            proj.SetDefaultWatchAppReleaseBuildFlags(str3);
            string str4 = proj.nativeTargets[watchExtensionTargetGuid].name.Replace(" ", "_");
            string[] configGuids = new string[] { configGuid, str3 };
            proj.SetBuildPropertyForConfig(configGuids, "PRODUCT_BUNDLE_IDENTIFIER", bundleId);
            proj.SetBuildPropertyForConfig(configGuids, "INFOPLIST_FILE", infoPlistPath);
            proj.SetBuildPropertyForConfig(configGuids, "IBSC_MODULE", str4);
            proj.AddResourcesBuildPhase(targetGuid);
            string sectionGuid = proj.AddCopyFilesBuildPhase(targetGuid, "Embed App Extensions", "", "13");
            proj.AddFileToBuildSection(targetGuid, sectionGuid, proj.GetTargetProductFileRef(watchExtensionTargetGuid));
            string str6 = proj.AddCopyFilesBuildPhase(mainTargetGuid, "Embed Watch Content", "$(CONTENTS_FOLDER_PATH)/Watch", "16");
            proj.AddFileToBuildSection(mainTargetGuid, str6, proj.GetTargetProductFileRef(targetGuid));
            proj.AddTargetDependency(targetGuid, watchExtensionTargetGuid);
            proj.AddTargetDependency(mainTargetGuid, targetGuid);
            return targetGuid;
        }

        public static string AddWatchExtension(this PBXProject proj, string mainTarget, string name, string bundleId, string infoPlistPath)
        {
            string targetGuid = proj.AddTarget(name, ".appex", "com.apple.product-type.watchkit2-extension");
            string configGuid = proj.AddBuildConfigForTarget(targetGuid, "Debug");
            string str3 = proj.AddBuildConfigForTarget(targetGuid, "Release");
            proj.SetDefaultWatchExtensionDebugBuildFlags(configGuid);
            proj.SetDefaultWatchExtensionReleaseBuildFlags(str3);
            string[] configGuids = new string[] { configGuid, str3 };
            proj.SetBuildPropertyForConfig(configGuids, "PRODUCT_BUNDLE_IDENTIFIER", bundleId);
            proj.SetBuildPropertyForConfig(configGuids, "INFOPLIST_FILE", infoPlistPath);
            proj.AddSourcesBuildPhase(targetGuid);
            proj.AddResourcesBuildPhase(targetGuid);
            proj.AddFrameworksBuildPhase(targetGuid);
            return targetGuid;
        }

        private static void SetBuildFlagsFromDict(this PBXProject proj, string configGuid, IEnumerable<KeyValuePair<string, string>> data)
        {
            foreach (KeyValuePair<string, string> pair in data)
            {
                proj.AddBuildPropertyForConfig(configGuid, pair.Key, pair.Value);
            }
        }

        internal static void SetDefaultAppExtensionDebugBuildFlags(this PBXProject proj, string configGuid)
        {
            proj.SetBuildFlagsFromDict(configGuid, appExtensionDebugBuildFlags);
        }

        internal static void SetDefaultAppExtensionReleaseBuildFlags(this PBXProject proj, string configGuid)
        {
            proj.SetBuildFlagsFromDict(configGuid, appExtensionReleaseBuildFlags);
        }

        internal static void SetDefaultWatchAppDebugBuildFlags(this PBXProject proj, string configGuid)
        {
            proj.SetBuildFlagsFromDict(configGuid, watchAppDebugBuildFlags);
        }

        internal static void SetDefaultWatchAppReleaseBuildFlags(this PBXProject proj, string configGuid)
        {
            proj.SetBuildFlagsFromDict(configGuid, watchAppReleaseBuildFlags);
        }

        internal static void SetDefaultWatchExtensionDebugBuildFlags(this PBXProject proj, string configGuid)
        {
            proj.SetBuildFlagsFromDict(configGuid, watchExtensionDebugBuildFlags);
        }

        internal static void SetDefaultWatchExtensionReleaseBuildFlags(this PBXProject proj, string configGuid)
        {
            proj.SetBuildFlagsFromDict(configGuid, watchExtensionReleaseBuildFlags);
        }

        internal class FlagList : List<KeyValuePair<string, string>>
        {
            public void Add(string flag, string value)
            {
                base.Add(new KeyValuePair<string, string>(flag, value));
            }
        }
    }
}

