namespace UnityEditor.iOS.Xcode.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEditor.iOS.Xcode;
    using UnityEditor.iOS.Xcode.PBX;

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
            list.Add("LD_RUNPATH_SEARCH_PATHS", "$(inherited)");
            list.Add("LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");
            list.Add("LD_RUNPATH_SEARCH_PATHS", "@executable_path/../../Frameworks");
            list.Add("PRODUCT_NAME", "$(TARGET_NAME)");
            list.Add("SKIP_INSTALL", "YES");
            appExtensionReleaseBuildFlags = list;
            list = new FlagList();
            list.Add("LD_RUNPATH_SEARCH_PATHS", "$(inherited)");
            list.Add("LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");
            list.Add("LD_RUNPATH_SEARCH_PATHS", "@executable_path/../../Frameworks");
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

        public static string AddAppExtension(this PBXProject proj, string mainTargetGuid, string name, string bundleId, string infoPlistPath)
        {
            string ext = ".appex";
            string targetGuid = proj.AddTarget(name, ext, "com.apple.product-type.app-extension");
            foreach (string str3 in proj.BuildConfigNames())
            {
                string configGuid = proj.BuildConfigByName(targetGuid, str3);
                if (str3.Contains("Debug"))
                {
                    proj.SetDefaultAppExtensionDebugBuildFlags(configGuid);
                }
                else
                {
                    proj.SetDefaultAppExtensionReleaseBuildFlags(configGuid);
                }
                proj.SetBuildPropertyForConfig(configGuid, "INFOPLIST_FILE", infoPlistPath);
                proj.SetBuildPropertyForConfig(configGuid, "PRODUCT_BUNDLE_IDENTIFIER", bundleId);
            }
            proj.AddSourcesBuildPhase(targetGuid);
            proj.AddResourcesBuildPhase(targetGuid);
            proj.AddFrameworksBuildPhase(targetGuid);
            string sectionGuid = proj.AddCopyFilesBuildPhase(mainTargetGuid, "Embed App Extensions", "", "13");
            proj.AddFileToBuildSection(mainTargetGuid, sectionGuid, proj.GetTargetProductFileRef(targetGuid));
            proj.AddTargetDependency(mainTargetGuid, targetGuid);
            return targetGuid;
        }

        internal static void AddExternalLibraryDependency(this PBXProject proj, string targetGuid, string filename, string remoteFileGuid, string projectPath, string remoteInfo)
        {
            PBXNativeTargetData target = proj.nativeTargets[targetGuid];
            filename = PBXPath.FixSlashes(filename);
            projectPath = PBXPath.FixSlashes(projectPath);
            string containerRef = proj.FindFileGuidByRealPath(projectPath);
            if (containerRef == null)
            {
                throw new Exception("No such project");
            }
            string guid = null;
            foreach (ProjectReference reference in proj.project.project.projectReferences)
            {
                if (reference.projectRef == containerRef)
                {
                    guid = reference.group;
                    break;
                }
            }
            if (guid == null)
            {
                throw new Exception("Malformed project: no project in project references");
            }
            PBXGroupData data2 = proj.GroupsGet(guid);
            string extension = Path.GetExtension(filename);
            if (!FileTypeUtils.IsBuildableFile(extension))
            {
                throw new Exception("Wrong file extension");
            }
            PBXContainerItemProxyData data3 = PBXContainerItemProxyData.Create(containerRef, "2", remoteFileGuid, remoteInfo);
            proj.containerItems.AddEntry(data3);
            string typeName = FileTypeUtils.GetTypeName(extension);
            PBXReferenceProxyData data4 = PBXReferenceProxyData.Create(filename, typeName, data3.guid, "BUILT_PRODUCTS_DIR");
            proj.references.AddEntry(data4);
            PBXBuildFileData buildFile = PBXBuildFileData.CreateFromFile(data4.guid, false, null);
            proj.BuildFilesAdd(targetGuid, buildFile);
            proj.BuildSectionAny(target, extension, false).files.AddGUID(buildFile.guid);
            data2.children.AddGUID(data4.guid);
        }

        internal static void AddExternalProjectDependency(this PBXProject proj, string path, string projectPath, PBXSourceTree sourceTree)
        {
            if (sourceTree == PBXSourceTree.Group)
            {
                throw new Exception("sourceTree must not be PBXSourceTree.Group");
            }
            path = PBXPath.FixSlashes(path);
            projectPath = PBXPath.FixSlashes(projectPath);
            PBXGroupData gr = PBXGroupData.CreateRelative("Products");
            proj.GroupsAddDuplicate(gr);
            PBXFileReferenceData fileRef = PBXFileReferenceData.CreateFromFile(path, Path.GetFileName(projectPath), sourceTree);
            proj.FileRefsAdd(path, projectPath, null, fileRef);
            proj.CreateSourceGroup(PBXPath.GetDirectory(projectPath)).children.AddGUID(fileRef.guid);
            proj.project.project.AddReference(gr.guid, fileRef.guid);
        }

        public static string AddWatchApp(this PBXProject proj, string mainTargetGuid, string watchExtensionTargetGuid, string name, string bundleId, string infoPlistPath)
        {
            string targetGuid = proj.AddTarget(name, ".app", "com.apple.product-type.application.watchapp2");
            string str2 = proj.nativeTargets[watchExtensionTargetGuid].name.Replace(" ", "_");
            foreach (string str3 in proj.BuildConfigNames())
            {
                string configGuid = proj.BuildConfigByName(targetGuid, str3);
                if (str3.Contains("Debug"))
                {
                    proj.SetDefaultWatchAppDebugBuildFlags(configGuid);
                }
                else
                {
                    proj.SetDefaultWatchAppReleaseBuildFlags(configGuid);
                }
                proj.SetBuildPropertyForConfig(configGuid, "PRODUCT_BUNDLE_IDENTIFIER", bundleId);
                proj.SetBuildPropertyForConfig(configGuid, "INFOPLIST_FILE", infoPlistPath);
                proj.SetBuildPropertyForConfig(configGuid, "IBSC_MODULE", str2);
            }
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
            foreach (string str2 in proj.BuildConfigNames())
            {
                string configGuid = proj.BuildConfigByName(targetGuid, str2);
                if (str2.Contains("Debug"))
                {
                    proj.SetDefaultWatchExtensionDebugBuildFlags(configGuid);
                }
                else
                {
                    proj.SetDefaultWatchExtensionReleaseBuildFlags(configGuid);
                }
                proj.SetBuildPropertyForConfig(configGuid, "PRODUCT_BUNDLE_IDENTIFIER", bundleId);
                proj.SetBuildPropertyForConfig(configGuid, "INFOPLIST_FILE", infoPlistPath);
            }
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

