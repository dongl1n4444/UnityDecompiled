namespace UnityEditor.iOS
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEditor.BuildReporting;
    using UnityEditor.Connect;
    using UnityEditor.CrashReporting;
    using UnityEditor.iOS.Il2Cpp;
    using UnityEditor.iOS.Xcode;
    using UnityEditor.iOS.Xcode.Extensions;
    using UnityEditor.Modules;
    using UnityEditor.Utils;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Rendering;

    internal class PostProcessiPhonePlayer
    {
        [CompilerGenerated]
        private static Func<string, string, bool> <>f__mg$cache0;
        private const string iPhonePlugins = "Assets/Plugins/iOS";
        private const string kImagesAssetCatalogPath = "Images.xcassets";
        private static string[] kiOSBundlePluginExtensions = new string[] { ".framework", ".bundle" };
        private static string[] kiOSEditablePluginFileExtensions = new string[] { ".m", ".mm", ".cpp", ".cc", ".c", ".h", ".xib", ".swift" };
        private static string[] kiOSPluginFileExtensions = new string[] { ".a", ".m", ".mm", ".cpp", ".cc", ".c", ".h", ".xib", ".png", ".swift" };
        private const string kUnityAssetCatalogPath = "UnityData.xcassets";
        private const string kUnityImagesAssetCatalogPath = "UnityImages.xcassets";
        private static IXcodeController s_xcodeController;

        private static void AddCollectedResources(ProjectPaths paths, IncludedFileList includedFiles, bool useOnDemandResources, string companyName)
        {
            List<Resource> list = UnityEditor.iOS.BuildPipeline.CollectResources();
            if (list.Count > 0)
            {
                includedFiles.Add(null, "UnityData.xcassets");
            }
            AssetCatalog catalog = new AssetCatalog(paths.UnityAssetCatalogStaging(), companyName);
            foreach (Resource resource in list)
            {
                string directory = PBXPath.GetDirectory(resource.name);
                catalog.OpenNamespacedFolder("", directory);
                AssetDataSet dataset = catalog.OpenDataSet(resource.name);
                if (resource.path != null)
                {
                    dataset.AddVariant(new DeviceRequirement(), resource.path, null);
                }
                else
                {
                    foreach (Resource.Variant variant in resource.variants)
                    {
                        if (variant.requirement != null)
                        {
                            AddVariantToDataset(dataset, variant.path, variant.requirement, resource.name);
                        }
                        else
                        {
                            string variantNameFromPath = Utils.GetVariantNameFromPath(variant.path);
                            if (variantNameFromPath == null)
                            {
                                throw new Exception($"Resource variants must have extensions: {variant.path}");
                            }
                            iOSDeviceRequirementGroup deviceRequirementsForAssetBundleVariant = PlayerSettings.iOS.GetDeviceRequirementsForAssetBundleVariant(variantNameFromPath);
                            if (deviceRequirementsForAssetBundleVariant == null)
                            {
                                throw new Exception($"Please configure resource variant '{variantNameFromPath}' in player settings");
                            }
                            if (deviceRequirementsForAssetBundleVariant.count > 0)
                            {
                                for (int i = 0; i < deviceRequirementsForAssetBundleVariant.count; i++)
                                {
                                    AddVariantToDataset(dataset, variant.path, deviceRequirementsForAssetBundleVariant[i], resource.name);
                                }
                            }
                            else
                            {
                                object[] args = new object[] { variantNameFromPath, resource.name, resource.path };
                                UnityEngine.Debug.LogWarningFormat("The device requirements have not been set for variant name '{0}'. Absence of device requirements has been assumed. [Required for resource '{1}', path '{2}']", args);
                                dataset.AddVariant(new DeviceRequirement(), resource.path, null);
                            }
                        }
                    }
                }
                if (useOnDemandResources)
                {
                    foreach (string str3 in resource.tags)
                    {
                        dataset.AddOnDemandResourceTag(str3);
                    }
                }
            }
            List<string> warnings = new List<string>();
            catalog.Write(warnings);
            foreach (string str4 in warnings)
            {
                UnityEngine.Debug.LogWarning(str4);
            }
        }

        private static void AddIl2CppOutputFiles(IncludedFileList includedFiles, string il2cppOutputDir)
        {
            if (Directory.Exists(il2cppOutputDir))
            {
                foreach (string str in Directory.GetFiles(il2cppOutputDir))
                {
                    IncludedFile item = new IncludedFile {
                        sourcePath = null,
                        destinationPath = "Classes/Native/" + FileUtil.RemovePathPrefix(str, il2cppOutputDir),
                        symlinkType = SymlinkType.None,
                        shouldAddToProject = !IsHeaderFile(str) && !IsIl2CppEmbeddedResourcesFile(str)
                    };
                    includedFiles.Add(item);
                }
            }
        }

        private static void AddIncludeDirForFile(List<string> list, string relative, string path)
        {
            bool flag = Path.IsPathRooted(path);
            path = PBXPath.FixSlashes(path);
            string directory = PBXPath.GetDirectory(path);
            if (!flag)
            {
                directory = PBXPath.FixSlashes(relative) + "/" + directory;
            }
            if (!list.Contains(directory))
            {
                list.Add(directory);
            }
        }

        private static void AddiPhoneAssemblyFiles(IncludedFileList includedFiles, string[] filesToAdd)
        {
            foreach (string str in filesToAdd)
            {
                if ((str != null) && str.EndsWith(".dll"))
                {
                    includedFiles.Add(null, "Libraries/" + str + ".s");
                }
            }
        }

        private static void AddLaunchScreenFilesIOS(ProjectPaths paths, BuildSettings bs, IncludedFileList includedFiles)
        {
            if (bs.IsIPhoneEnabled())
            {
                iOSLaunchScreenType intValue = (iOSLaunchScreenType) PlayerSettings.FindProperty("iOSLaunchScreenType").intValue;
                switch (intValue)
                {
                    case iOSLaunchScreenType.Default:
                        AddLaunchScreenFilesiPhoneDefault(paths, includedFiles);
                        break;

                    case iOSLaunchScreenType.ImageAndBackgroundRelative:
                    case iOSLaunchScreenType.ImageAndBackgroundConstant:
                        AddLaunchScreenFilesiPhoneWithBackground(paths, bs, includedFiles, intValue);
                        break;

                    case iOSLaunchScreenType.CustomXib:
                        AddLaunchScreenFilesiPhoneCustom(paths, includedFiles);
                        break;
                }
            }
            if (bs.IsIPadEnabled())
            {
                iOSLaunchScreenType type = (iOSLaunchScreenType) PlayerSettings.FindProperty("iOSLaunchScreeniPadType").intValue;
                switch (type)
                {
                    case iOSLaunchScreenType.Default:
                        AddLaunchScreenFilesiPadDefault(paths, includedFiles);
                        break;

                    case iOSLaunchScreenType.ImageAndBackgroundRelative:
                    case iOSLaunchScreenType.ImageAndBackgroundConstant:
                        AddLaunchScreenFilesiPadWithBackground(paths, bs, includedFiles, type);
                        break;

                    case iOSLaunchScreenType.CustomXib:
                        AddLaunchScreenFilesiPadCustom(paths, includedFiles);
                        break;
                }
            }
        }

        private static void AddLaunchScreenFilesiPadCustom(ProjectPaths paths, IncludedFileList includedFiles)
        {
            string stringValue = PlayerSettings.FindProperty("iOSLaunchScreeniPadCustomXibPath").stringValue;
            if (!System.IO.File.Exists(stringValue))
            {
                UnityEngine.Debug.LogWarning("Custom xib file for iPad does not exist");
            }
            else
            {
                FileUtil.ReplaceFile(stringValue, paths.LaunchScreeniPadXibStaging());
                includedFiles.Add(null, "LaunchScreen-iPad.xib");
            }
        }

        private static void AddLaunchScreenFilesiPadDefault(ProjectPaths paths, IncludedFileList includedFiles)
        {
            string[] srcFiles = new string[] { paths.LaunchScreeniPadOutput(), paths.SplashScreenSharedOutput(), paths.LaunchScreenFallbackSource() };
            Utils.InstallFileWithFallbacks(srcFiles, paths.LaunchScreeniPadStaging());
            FileUtil.CopyFileOrDirectory(paths.LaunchScreeniPadXibDefaultSource(), paths.LaunchScreeniPadXibStaging());
            includedFiles.Add(null, "LaunchScreen-iPad.xib");
            includedFiles.Add(null, "LaunchScreen-iPad.png");
        }

        private static void AddLaunchScreenFilesiPadWithBackground(ProjectPaths paths, BuildSettings bs, IncludedFileList includedFiles, iOSLaunchScreenType type)
        {
            string[] srcFiles = new string[] { paths.LaunchScreeniPadOutput(), paths.SplashScreenSharedOutput(), paths.LaunchScreenFallbackSource() };
            bool flag = Utils.InstallFileWithFallbacks(srcFiles, paths.LaunchScreeniPadStaging()) == 0;
            if (bs.IsIPadEnabled() && !flag)
            {
                UnityEngine.Debug.Log("iPad launch screen image not provided");
            }
            LaunchScreenUpdater.XibWithBackgroundData data = new LaunchScreenUpdater.XibWithBackgroundData();
            if (flag)
            {
                Texture2D objectReferenceValue = (Texture2D) PlayerSettings.FindProperty("iOSLaunchScreeniPadImage").objectReferenceValue;
                data.iPadHeight = objectReferenceValue.height;
                data.iPadWidth = objectReferenceValue.width;
            }
            data.background = PlayerSettings.FindProperty("iOSLaunchScreeniPadBackgroundColor").colorValue;
            if (type == iOSLaunchScreenType.ImageAndBackgroundRelative)
            {
                data.iPadRelativeVerticalSize = PlayerSettings.FindProperty("iOSLaunchScreeniPadFillPct").floatValue / 100f;
                string contents = LaunchScreenUpdater.UpdateStringForBackgroundRelative(System.IO.File.ReadAllText(paths.LaunchScreeniPadXibRelativeSizeSource()), data, UnityEditor.iOS.DeviceType.iPad);
                System.IO.File.WriteAllText(paths.LaunchScreeniPadXibStaging(), contents);
            }
            else
            {
                data.iPadVerticalSize = PlayerSettings.FindProperty("iOSLaunchScreeniPadSize").floatValue;
                string str2 = LaunchScreenUpdater.UpdateStringForBackgroundConstant(System.IO.File.ReadAllText(paths.LaunchScreeniPadXibConstantSizeSource()), data, UnityEditor.iOS.DeviceType.iPad);
                System.IO.File.WriteAllText(paths.LaunchScreeniPadXibStaging(), str2);
            }
            includedFiles.Add(null, "LaunchScreen-iPad.xib");
            includedFiles.Add(null, "LaunchScreen-iPad.png");
        }

        private static void AddLaunchScreenFilesiPhoneCustom(ProjectPaths paths, IncludedFileList includedFiles)
        {
            string stringValue = PlayerSettings.FindProperty("iOSLaunchScreenCustomXibPath").stringValue;
            if (!System.IO.File.Exists(stringValue))
            {
                UnityEngine.Debug.LogWarning("Custom xib file for iPhone does not exist");
            }
            else
            {
                FileUtil.ReplaceFile(stringValue, paths.LaunchScreeniPhoneXibStaging());
                includedFiles.Add(null, "LaunchScreen-iPhone.xib");
            }
        }

        private static void AddLaunchScreenFilesiPhoneDefault(ProjectPaths paths, IncludedFileList includedFiles)
        {
            string[] srcFiles = new string[] { paths.SplashScreenSharedOutput(), paths.LaunchScreenFallbackSource() };
            Utils.InstallFileWithFallbacks(srcFiles, paths.LaunchScreeniPhonePortraitStaging());
            Utils.InstallFileWithFallbacks(srcFiles, paths.LaunchScreeniPhoneLandscapeStaging());
            FileUtil.CopyFileOrDirectory(paths.LaunchScreeniPhoneXibDefaultSource(), paths.LaunchScreeniPhoneXibStaging());
            includedFiles.Add(null, "LaunchScreen-iPhone.xib");
            includedFiles.Add(null, "LaunchScreen-iPhonePortrait.png");
            includedFiles.Add(null, "LaunchScreen-iPhoneLandscape.png");
        }

        private static void AddLaunchScreenFilesiPhoneWithBackground(ProjectPaths paths, BuildSettings bs, IncludedFileList includedFiles, iOSLaunchScreenType type)
        {
            string[] srcFiles = new string[] { paths.LaunchScreeniPhonePortraitOutput(), paths.SplashScreenSharedOutput(), paths.LaunchScreenTransparentSource() };
            bool flag = Utils.InstallFileWithFallbacks(srcFiles, paths.LaunchScreeniPhonePortraitStaging()) == 0;
            string[] strArray2 = new string[] { paths.LaunchScreeniPhoneLandscapeOutput(), paths.SplashScreenSharedOutput(), paths.LaunchScreenTransparentSource() };
            bool flag2 = Utils.InstallFileWithFallbacks(strArray2, paths.LaunchScreeniPhoneLandscapeStaging()) == 0;
            AvailableOrientations availableOrientations = GetAvailableOrientations();
            if (bs.IsIPhoneEnabled())
            {
                if (!flag2 && (availableOrientations.landscapeLeft || availableOrientations.landscapeRight))
                {
                    UnityEngine.Debug.Log("Landscape iPhone launch screen image not provided");
                }
                if (!flag && (availableOrientations.portrait || availableOrientations.portraitUpsideDown))
                {
                    UnityEngine.Debug.Log("Portrait iPhone launch screen image not provided");
                }
            }
            LaunchScreenUpdater.XibWithBackgroundData data = new LaunchScreenUpdater.XibWithBackgroundData();
            if (flag2)
            {
                Texture2D objectReferenceValue = (Texture2D) PlayerSettings.FindProperty("iOSLaunchScreenLandscape").objectReferenceValue;
                data.iPhoneLandscapeHeight = objectReferenceValue.height;
                data.iPhoneLandscapeWidth = objectReferenceValue.width;
            }
            if (flag)
            {
                Texture2D textured2 = (Texture2D) PlayerSettings.FindProperty("iOSLaunchScreenPortrait").objectReferenceValue;
                data.iPhonePortraitHeight = textured2.height;
                data.iPhonePortraitWidth = textured2.width;
            }
            data.background = PlayerSettings.FindProperty("iOSLaunchScreenBackgroundColor").colorValue;
            if (type == iOSLaunchScreenType.ImageAndBackgroundRelative)
            {
                float num = PlayerSettings.FindProperty("iOSLaunchScreenFillPct").floatValue / 100f;
                data.iPhonePortraitRelativeHorizontalSize = data.iPhoneLandscapeRelativeVerticalSize = num;
                string contents = LaunchScreenUpdater.UpdateStringForBackgroundRelative(System.IO.File.ReadAllText(paths.LaunchScreeniPhoneXibRelativeSizeSource()), data, UnityEditor.iOS.DeviceType.iPhone);
                if (!availableOrientations.portrait && !availableOrientations.portraitUpsideDown)
                {
                    contents = LaunchScreenUpdater.RemoveiPhonePortraitViewInRelative(contents);
                }
                System.IO.File.WriteAllText(paths.LaunchScreeniPhoneXibStaging(), contents);
            }
            else
            {
                float floatValue = PlayerSettings.FindProperty("iOSLaunchScreenSize").floatValue;
                data.iPhonePortraitHorizontalSize = data.iPhoneLandscapeVerticalSize = floatValue;
                string str2 = LaunchScreenUpdater.UpdateStringForBackgroundConstant(System.IO.File.ReadAllText(paths.LaunchScreeniPhoneXibConstantSizeSource()), data, UnityEditor.iOS.DeviceType.iPhone);
                if (!availableOrientations.portrait && !availableOrientations.portraitUpsideDown)
                {
                    str2 = LaunchScreenUpdater.RemoveiPhonePortraitViewInConstant(str2);
                }
                System.IO.File.WriteAllText(paths.LaunchScreeniPhoneXibStaging(), str2);
            }
            includedFiles.Add(null, "LaunchScreen-iPhone.xib");
            includedFiles.Add(null, "LaunchScreen-iPhonePortrait.png");
            includedFiles.Add(null, "LaunchScreen-iPhoneLandscape.png");
        }

        private static void AddPluginFiles(BuildTarget target, IncludedFileList includedFiles, List<string> frameworks)
        {
            <AddPluginFiles>c__AnonStorey4 storey = new <AddPluginFiles>c__AnonStorey4 {
                includedFiles = includedFiles
            };
            string platformName = BuildTargetToTargetName(target);
            foreach (PluginImporter importer in PluginImporter.GetImporters(platformName))
            {
                <AddPluginFiles>c__AnonStorey3 storey2 = new <AddPluginFiles>c__AnonStorey3 {
                    <>f__ref$4 = storey,
                    compileFlags = importer.GetPlatformData(platformName, Plugin.compileFlagsKey)
                };
                if (Directory.Exists(importer.assetPath))
                {
                    Action<string> fileCallback = new Action<string>(storey2.<>m__0);
                    Func<string, bool> directoryCallback = new Func<string, bool>(storey2.<>m__1);
                    if (directoryCallback(importer.assetPath))
                    {
                        FileUtil.WalkFilesystemRecursively(importer.assetPath, fileCallback, directoryCallback);
                    }
                }
                else
                {
                    if (!ShouldCopyPluginFile(importer.assetPath))
                    {
                        continue;
                    }
                    IncludedFile item = new IncludedFile {
                        sourcePath = importer.assetPath,
                        destinationPath = GetPluginDestinationPath("Libraries", importer.assetPath),
                        shouldAddToProject = ShouldAddPluginToProject(importer.assetPath),
                        symlinkType = !ShouldUseXcodeReferenceForPlugin(importer.assetPath) ? SymlinkType.Symlink : SymlinkType.XcodeReference,
                        compileFlags = storey2.compileFlags
                    };
                    storey.includedFiles.Add(item);
                }
                string platformData = importer.GetPlatformData(platformName, Plugin.frameworkDependenciesKey);
                if (platformData != null)
                {
                    char[] separator = new char[] { ';' };
                    string[] strArray = platformData.Split(separator);
                    foreach (string str3 in strArray)
                    {
                        if ((str3 != "") && !frameworks.Contains(str3))
                        {
                            frameworks.Add(str3);
                        }
                    }
                }
            }
        }

        private static void AddPluginPNGFiles(IncludedFileList includedFiles)
        {
            if (Directory.Exists("Assets/Plugins/iOS"))
            {
                foreach (string str in Directory.GetFiles("Assets/Plugins/iOS"))
                {
                    if (Path.GetExtension(str).ToLower() == ".png")
                    {
                        includedFiles.Add(str, "Libraries/" + Path.GetFileName(str));
                    }
                }
            }
        }

        private static void AddVariantToDataset(AssetDataSet dataset, string path, iOSDeviceRequirement requirement, string resourceName)
        {
            DeviceRequirement requirement2 = DeviceRequirementUtils.ToXcodeRequirement(requirement);
            if (!dataset.HasVariant(requirement2))
            {
                dataset.AddVariant(requirement2, path, null);
            }
            else
            {
                object[] args = new object[] { path, resourceName };
                UnityEngine.Debug.LogWarningFormat("Variant '{0}' has been ignored for resource '{1}'. A variant with the same device requirements has been already added", args);
            }
        }

        private static void AddXcodeDevProjectAdditionalLibs(ProjectPaths paths, IncludedFileList includedFiles)
        {
            string str = "Frameworks/";
            foreach (string str2 in System.IO.File.ReadAllLines(paths.DevProjectLibList()))
            {
                string relativePath = Utils.GetRelativePath(paths.UnityTree(), str2);
                if (!System.IO.File.Exists(str2))
                {
                    UnityEngine.Debug.LogWarning("XCode development project: library does not exist! " + str2);
                }
                else
                {
                    IncludedFile item = new IncludedFile {
                        sourcePath = null,
                        destinationPath = str2,
                        projectPath = Path.Combine(str, relativePath),
                        shouldAddToProject = true
                    };
                    includedFiles.Add(item);
                }
            }
        }

        private static void AddXcodeDevProjectBuildFiles(ProjectPaths paths, IncludedFileList includedFiles)
        {
            string str = "Classes/";
            foreach (string str2 in System.IO.File.ReadAllLines(paths.DevProjectFileList()))
            {
                string str3;
                string str4;
                string str6;
                string relativePath;
                int index = str2.IndexOf(' ');
                if (index == -1)
                {
                    str3 = str2;
                    str4 = null;
                }
                else
                {
                    str3 = str2.Substring(0, index);
                    str4 = str2.Substring(index + 1);
                }
                List<string> list = new List<string> { 
                    "<iPhonePlayer-armv7>",
                    "<iOSSimulatorPlayer>",
                    "<iOSSimulatorPlayerUnityLib>"
                };
                foreach (string str5 in list)
                {
                    if (str3.IndexOf(str5) == 0)
                    {
                        str3 = str3.Substring(str5.Length);
                    }
                }
                if (Path.IsPathRooted(str3))
                {
                    relativePath = Utils.GetRelativePath(paths.UnityTree(), str3);
                    str6 = str3;
                }
                else
                {
                    relativePath = str3;
                    str6 = Path.Combine(paths.UnityTree(), str3);
                }
                if (!System.IO.File.Exists(str6))
                {
                    UnityEngine.Debug.LogWarning("XCode development project: file does not exist! " + str6);
                }
                else if (!str6.EndsWith(".txt"))
                {
                    IncludedFile item = new IncludedFile {
                        sourcePath = null,
                        destinationPath = str6,
                        projectPath = Path.Combine(str, relativePath),
                        shouldAddToProject = true,
                        compileFlags = str4
                    };
                    includedFiles.Add(item);
                }
            }
        }

        internal static void BuildPlayerLib()
        {
            string config = !EditorUserBuildSettings.development ? "release" : "debug";
            string devPlayer = !EditorUserBuildSettings.development ? "0" : "1";
            string str3 = (PlayerSettings.GetScriptingBackend(BuildTargetGroup.iPhone) != ScriptingImplementation.IL2CPP) ? "mono" : "il2cpp";
            string platform = (PlayerSettings.iOS.sdkVersion != iOSSdkVersion.SimulatorSDK) ? "iphone" : "iphonesimulator";
            string str5 = (PlayerSettings.iOS.sdkVersion != iOSSdkVersion.SimulatorSDK) ? "iOSPlayer" : "iOSSimulatorPlayer";
            ProjectPaths paths = new ProjectPaths {
                playerPackage = UnityEditor.BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.iOS, BuildOptions.CompressTextures)
            };
            string progressMessage = $"Building {!EditorUserBuildSettings.development ? "release" : "development"} {str3} {str5} library";
            if (!RunJam(paths, config, devPlayer, str3, platform, str5, "", "Building runtime static library", progressMessage))
            {
                EditorUtility.DisplayDialog("Error", "Failed building static library. Check Editor.log for details.", "OK");
            }
        }

        private static string BuildTargetToTargetName(BuildTarget target)
        {
            if (target != BuildTarget.iOS)
            {
                if (target == BuildTarget.tvOS)
                {
                    return "tvOS";
                }
            }
            else
            {
                return "iOS";
            }
            return null;
        }

        private static void BuildXCodeProject(ProjectPaths paths, BuildSettings bs, UsedFeatures usedFeatures, IncludedFileList includedFiles, List<string> frameworks, List<string> initialInstallTags)
        {
            string path = paths.PBXProjectStaging();
            PBXProject proj = new PBXProject();
            proj.ReadFromFile(path);
            string targetGuid = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
            string str3 = proj.TargetGuidByName(PBXProject.GetUnityTestTargetName());
            string str4 = proj.ProjectGuid();
            if ((targetGuid == null) || (str3 == null))
            {
                throw new Exception($"Deletion of either of the "{PBXProject.GetUnityTargetName()}" or "{PBXProject.GetUnityTestTargetName()}" targets is not supported");
            }
            string[] targetGuids = new string[] { targetGuid, str3 };
            string[] strArray2 = new string[] { targetGuid, str3, str4 };
            HashSet<string> groupChildrenFilesRefs = proj.GetGroupChildrenFilesRefs("Classes/Native");
            string[] strArray3 = new string[] { "LaunchScreen-iPad.xib", "LaunchScreen-iPhone.xib", "LaunchScreen-iPhonePortrait.png", "LaunchScreen-iPhoneLandscape.png", "LaunchScreen-iPad.png" };
            HashSet<string> first = new HashSet<string>(groupChildrenFilesRefs.Union<string>(proj.GetFileRefsByProjectPaths(strArray3)));
            HashSet<string> second = new HashSet<string>(includedFiles.GetProjectFilesInPath("Classes/Native").Union<string>(includedFiles.GetProjectFilesByFullPath(new HashSet<string>(strArray3))));
            HashSet<string> set4 = new HashSet<string>(first.Except<string>(second));
            HashSet<string> set5 = new HashSet<string>(first.Intersect<string>(second));
            foreach (string str6 in set4)
            {
                proj.RemoveFile(proj.FindFileGuidByProjectPath(str6));
            }
            List<string> groupChildrenFiles = proj.GetGroupChildrenFiles("Libraries");
            foreach (string str7 in groupChildrenFiles)
            {
                if (str7.EndsWith(".dll.s"))
                {
                    proj.RemoveFile(proj.FindFileGuidByProjectPath("Libraries/" + str7));
                }
            }
            List<string> list = new List<string>();
            List<string> list3 = new List<string>();
            foreach (IncludedFile file in includedFiles)
            {
                string ext = Path.GetExtension(file.destinationPath).ToLower();
                if (!PBXProject.IsKnownExtension(ext))
                {
                    UnityEngine.Debug.LogWarning("Unknown file extension: " + ext);
                }
                if (GetProjectPath(file) == "Libraries/libiPhone-lib.a")
                {
                    if (file.symlinkType == SymlinkType.XcodeReference)
                    {
                        throw new Exception("SymlinkType.XcodeReference is not supported for libiPhone-lib.a");
                    }
                    continue;
                }
                string destinationPath = file.destinationPath;
                if (file.shouldAddToProject && (!second.Contains(destinationPath) || !set5.Contains(destinationPath)))
                {
                    if ((bs.symlinkLibraries && (file.symlinkType == SymlinkType.XcodeReference)) && (file.sourcePath != null))
                    {
                        if (ShouldUseRelativeSymlinkForFile(file.sourcePath, paths.installPath))
                        {
                            destinationPath = PBXPath.FixSlashes(Utils.GetRelativePath(paths.installPath, file.sourcePath));
                        }
                        else
                        {
                            destinationPath = PBXPath.GetFullPath(file.sourcePath);
                        }
                    }
                    switch (ext)
                    {
                        case ".so":
                        case ".a":
                            AddIncludeDirForFile(list, "$(SRCROOT)", destinationPath);
                            break;

                        default:
                            if (ext == ".framework")
                            {
                                AddIncludeDirForFile(list3, "$(PROJECT_DIR)", destinationPath);
                            }
                            break;
                    }
                    string fileGuid = proj.AddFile(destinationPath, GetProjectPath(file), GetSourceTreeForPath(destinationPath));
                    proj.AddFileToBuildWithFlags(targetGuid, fileGuid, file.compileFlags);
                }
            }
            foreach (string str11 in frameworks)
            {
                proj.AddFrameworkToProject(targetGuid, str11 + ".framework", false);
            }
            if (!frameworks.Contains("GameKit"))
            {
                if (usedFeatures.gameCenter)
                {
                    proj.AddFrameworkToProject(targetGuid, "GameKit.framework", false);
                }
                else
                {
                    proj.RemoveFrameworkFromProject(targetGuid, "GameKit.framework");
                }
            }
            if (usedFeatures.replayKit)
            {
                proj.AddFrameworkToProject(targetGuid, "ReplayKit.framework", true);
            }
            else
            {
                proj.RemoveFrameworkFromProject(targetGuid, "ReplayKit.framework");
            }
            if (bs.IsAppleTVEnabled() && !frameworks.Contains("GameController"))
            {
                proj.AddFrameworkToProject(targetGuid, "GameController.framework", false);
            }
            if (!frameworks.Contains("GameController") && usedFeatures.controller)
            {
                proj.AddFrameworkToProject(targetGuid, "GameController.framework", true);
            }
            if (!frameworks.Contains("Metal") && IsMetalUsed(bs))
            {
                proj.AddFrameworkToProject(targetGuid, "Metal.framework", true);
            }
            if (!IsMetalUsed(bs))
            {
                proj.RemoveFrameworkFromProject(targetGuid, "Metal.framework");
            }
            list.Remove("$(SRCROOT)/Libraries");
            foreach (string str12 in list)
            {
                proj.AddBuildProperty(targetGuids, "LIBRARY_SEARCH_PATHS", str12);
            }
            proj.SetBuildProperty(targetGuids, "FRAMEWORK_SEARCH_PATHS", "$(inherited)");
            foreach (string str13 in list3)
            {
                proj.AddBuildProperty(targetGuids, "FRAMEWORK_SEARCH_PATHS", str13);
            }
            proj.SetBuildProperty(targetGuids, "ARCHS", GetTargetArchitecture(bs));
            if (bs.IsAppleTVEnabled())
            {
                proj.RemoveBuildProperty(targetGuids, "CODE_SIGN_IDENTITY[sdk=iphoneos*]");
                proj.SetBuildProperty(targetGuids, "CODE_SIGN_IDENTITY[sdk=appletvos*]", "iPhone Developer");
                proj.SetBuildProperty(strArray2, "SDKROOT", "appletvos");
                proj.RemoveBuildProperty(strArray2, "SUPPORTED_PLATFORMS");
                proj.AddBuildProperty(strArray2, "SUPPORTED_PLATFORMS", "appletvos");
                if (bs.sdkType == SdkType.Simulator)
                {
                    proj.AddBuildProperty(strArray2, "SUPPORTED_PLATFORMS", "appletvsimulator");
                }
                proj.RemoveBuildProperty(targetGuids, "IPHONEOS_DEPLOYMENT_TARGET");
                proj.SetBuildProperty(targetGuids, "TVOS_DEPLOYMENT_TARGET", bs.targetOsVersion.ToString());
                proj.RemoveFrameworkFromProject(targetGuid, "CoreMotion.framework");
                proj.RemoveFrameworkFromProject(str3, "CoreMotion.framework");
                string[] valueList = new string[] { "-weak_framework", "CoreMotion" };
                proj.RemoveBuildPropertyValueList(targetGuids, "OTHER_LDFLAGS", valueList);
            }
            else
            {
                proj.SetBuildProperty(strArray2, "SDKROOT", (bs.sdkType != SdkType.Device) ? "iphonesimulator" : "iphoneos");
                proj.RemoveBuildProperty(strArray2, "SUPPORTED_PLATFORMS");
                proj.AddBuildProperty(strArray2, "SUPPORTED_PLATFORMS", "iphoneos");
                if (bs.sdkType == SdkType.Simulator)
                {
                    proj.AddBuildProperty(strArray2, "SUPPORTED_PLATFORMS", "iphonesimulator");
                }
                proj.SetBuildProperty(targetGuids, "IPHONEOS_DEPLOYMENT_TARGET", bs.targetOsVersion.ToString());
            }
            proj.SetBuildProperty(targetGuids, "PRODUCT_NAME", bs.productName);
            proj.SetBuildProperty(targetGuids, "TARGETED_DEVICE_FAMILY", GetTargetDeviceFamily(bs));
            proj.SetBuildProperty(targetGuids, "UNITY_RUNTIME_VERSION", Application.unityVersion);
            proj.SetBuildProperty(targetGuids, "UNITY_SCRIPTING_BACKEND", !bs.UseIl2Cpp() ? "mono2x" : "il2cpp");
            if (!bs.automaticallySignBuild)
            {
                if (bs.IsAppleTVEnabled())
                {
                    proj.SetBuildProperty(targetGuids, "PROVISIONING_PROFILE", bs.tvOSManualProvisioningProfileUUID);
                    proj.SetTargetAttributes("ProvisioningStyle", "Manual");
                }
                else
                {
                    proj.SetBuildProperty(targetGuids, "PROVISIONING_PROFILE", bs.iOSManualProvisioningProfileUUID);
                    proj.SetTargetAttributes("ProvisioningStyle", "Manual");
                }
                proj.SetBuildProperty(targetGuids, "DEVELOPMENT_TEAM", "");
            }
            else
            {
                proj.SetBuildProperty(targetGuids, "DEVELOPMENT_TEAM", bs.appleDeveloperTeamID);
                proj.SetBuildProperty(targetGuids, "PROVISIONING_PROFILE", "Automatic");
                proj.SetTargetAttributes("ProvisioningStyle", "Automatic");
                proj.SetBuildProperty(targetGuids, "PROVISIONING_PROFILE", "");
            }
            if (bs.UseIl2Cpp())
            {
                proj.SetBuildProperty(targetGuids, "LD_GENERATE_MAP_FILE", "YES");
            }
            else
            {
                proj.SetBuildProperty(targetGuids, "ENABLE_BITCODE", "NO");
            }
            if (bs.useOnDemandResources)
            {
                proj.SetBuildProperty(targetGuid, "ENABLE_ON_DEMAND_RESOURCES", "YES");
            }
            proj.RemoveBuildProperty(targetGuid, "ON_DEMAND_RESOURCES_INITIAL_INSTALL_TAGS");
            foreach (string str14 in initialInstallTags)
            {
                proj.AddBuildProperty(targetGuid, "ON_DEMAND_RESOURCES_INITIAL_INSTALL_TAGS", str14);
            }
            if (bs.installInBuildFolder)
            {
                List<string> list4 = new List<string> { 
                    "External/FMOD/builds/iphone/lib",
                    "External/Mono/builds/embedruntimes/iphone",
                    "External/RakNet/builds",
                    "External/PLCrashReporter/builds/ios",
                    "External/Allegorithmic/builds/Engines/lib/ios",
                    "External/PhysX3/builds/Lib/ios",
                    "PlatformDependent/iPhonePlayer/External/Enlighten/builds/Lib/IOS_LIBSTDCXX-Release"
                };
                foreach (string str15 in list4)
                {
                    proj.AddBuildProperty(targetGuids, "LIBRARY_SEARCH_PATHS", Path.Combine(paths.UnityTree(), str15));
                }
                string str16 = Path.Combine(paths.DevProjectInstallPath(), "UnityDevProject.xcodeproj");
                proj.AddExternalProjectDependency(str16, "UnityDevProject.xcodeproj", PBXSourceTree.Absolute);
                proj.AddExternalLibraryDependency(targetGuid, "libUnityDevProject.a", "AA88A7D019101316001E7AB7", str16, "UnityDevProject");
            }
            List<string> list6 = new List<string>();
            List<string> list7 = new List<string>();
            List<string> list8 = new List<string>();
            List<string> list9 = new List<string>();
            if (bs.sdkType == SdkType.Simulator)
            {
                list6.Add("-DTARGET_IPHONE_SIMULATOR=1");
                list8.Add("-Wl,-undefined,dynamic_lookup");
            }
            else
            {
                list7.Add("-DTARGET_IPHONE_SIMULATOR=1");
                list9.Add("-Wl,-undefined,dynamic_lookup");
            }
            if (bs.UseIl2Cpp())
            {
                list6.Add("-DINIT_SCRIPTING_BACKEND=1");
                list6.Add("-fno-strict-overflow");
                list7.Add("-mno-thumb");
                list6.Add("-DRUNTIME_IL2CPP=1");
            }
            else
            {
                list6.Add("-mno-thumb");
                list7.Add("-DINIT_SCRIPTING_BACKEND=1");
            }
            proj.UpdateBuildProperty(targetGuid, "OTHER_CFLAGS", list6.ToArray(), list7.ToArray());
            proj.UpdateBuildProperty(targetGuid, "OTHER_LDFLAGS", list8.ToArray(), list9.ToArray());
            SetupFeatureDefines(proj, targetGuid, bs);
            string[] addValues = new string[] { "$(SRCROOT)/Classes/Native", "$(SRCROOT)/Libraries/bdwgc/include", "$(SRCROOT)/Libraries/libil2cpp/include" };
            if (bs.UseIl2Cpp())
            {
                proj.SetBuildProperty(targetGuid, "GCC_ENABLE_CPP_EXCEPTIONS", "YES");
                proj.UpdateBuildProperty(targetGuid, "HEADER_SEARCH_PATHS", addValues, null);
            }
            else
            {
                proj.SetBuildProperty(targetGuid, "GCC_ENABLE_CPP_EXCEPTIONS", "NO");
                proj.UpdateBuildProperty(targetGuid, "HEADER_SEARCH_PATHS", null, addValues);
            }
            if (bs.nativeCrashReportingEnabled && (bs.symbolsServiceBaseUri != null))
            {
                proj.AppendShellScriptBuildPhase(targetGuid, "Process symbols", "/bin/sh", "\"$PROJECT_DIR/process_symbols.sh\"");
                proj.SetBuildProperty(strArray2, "UNITY_CLOUD_PROJECT_ID", bs.cloudProjectId);
                proj.SetBuildProperty(strArray2, "USYM_UPLOAD_URL_SOURCE", new Uri(bs.symbolsServiceBaseUri, "url").ToString());
                proj.SetBuildProperty(strArray2, "DEBUG_INFORMATION_FORMAT", "dwarf-with-dsym");
                proj.SetBuildProperty(strArray2, "USYM_UPLOAD_AUTH_TOKEN", GetUsymUploadAuthToken(bs));
            }
            proj.WriteToFile(path);
        }

        private static void BuildXCodeUnityDevProject(ProjectPaths paths, IncludedFileList includedFiles)
        {
            string path = paths.DevProjectPBXProjectStaging();
            PBXProject project = new PBXProject();
            project.ReadFromFile(path);
            string targetGuid = project.TargetGuidByName("UnityDevProject");
            project.SetBuildProperty(targetGuid, "GCC_PREFIX_HEADER", Path.Combine(paths.UnityTree(), "Projects/PrecompiledHeaders/UnityPrefix.h"));
            foreach (IncludedFile file in includedFiles)
            {
                if (file.shouldAddToProject)
                {
                    string fileGuid = project.AddFile(file.destinationPath, GetProjectPath(file), GetSourceTreeForPath(file.destinationPath));
                    project.AddFileToBuildWithFlags(targetGuid, fileGuid, file.compileFlags);
                }
            }
            project.WriteToFile(path);
        }

        internal static bool CheckIfPlayerLibIsBuilt()
        {
            BuildSettings bs = new BuildSettings {
                target = BuildTarget.iOS,
                sdkType = (PlayerSettings.iOS.sdkVersion != iOSSdkVersion.DeviceSDK) ? SdkType.Simulator : SdkType.Device,
                backend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.iPhone),
                isDevelopmentPlayer = EditorUserBuildSettings.development
            };
            ProjectPaths paths = new ProjectPaths {
                playerPackage = UnityEditor.BuildPipeline.GetPlaybackEngineDirectory(bs.target, BuildOptions.CompressTextures)
            };
            return System.IO.File.Exists(GetPlayerLibLocation(paths, bs));
        }

        private static void CheckIOSBundleIdentifier(string identifier, BuildSettings bs)
        {
            if (!IsBundleIdAcceptableByXcode(identifier))
            {
                string message = "Please set up the iPhoneBundleIdentifier in the Player Settings.";
                message = (message + " The value should follow the convention 'com.YourCompanyName.YourProductName'" + " and can contain alphanumeric characters (a-z, A-Z, 0-9) and hyphen (-).") + " You also have to create a Provisioning Profile with the same identifier" + " and install it in the Xcode Organizer Provisioning Profiles.";
                EditorUtility.DisplayDialog("iPhoneBundleIdentifier has not been set up correctly", message, "Ok");
                Selection.activeObject = Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings");
                throw new Exception("iPhoneBundleIdentifier has not been set up.\n" + message);
            }
            if (!IsBundleIdInReverseDnsFormat(identifier))
            {
                UnityEngine.Debug.LogWarning("Bundle ID doesn't follow reverse DNS 'com.MyCompany.MyProductName' format suggested by Apple. Your app may get rejected when submitting to the App Store.");
            }
            if (!bs.isDevelopmentPlayer && (identifier == "com.Company.ProductName"))
            {
                UnityEngine.Debug.LogWarning("Don't forget to change iPhoneBundleIdentifier in the Player Settings before submitting to the App Store.");
            }
        }

        private static void CleanupMDBFiles(string managedFolder)
        {
            foreach (string str in Directory.GetFiles(managedFolder))
            {
                if (str.EndsWith(".mdb"))
                {
                    if (!System.IO.File.Exists(str.Substring(0, str.Length - 4)))
                    {
                        System.IO.File.Delete(str);
                    }
                    else if (!Path.GetFileName(str).StartsWith("Assembly-CSharp") && !Path.GetFileName(str).StartsWith("Assembly-UnityScript"))
                    {
                        System.IO.File.Delete(str);
                    }
                }
            }
        }

        private static string CreateTagNameFromFileName(string filename) => 
            filename;

        private static IncludedFileList CrossCompileManagedDlls(BuildSettings bs, ProjectPaths paths, AssemblyReferenceChecker checker, RuntimeClassRegistry usedClassRegistry, BuildReport buildReport)
        {
            <CrossCompileManagedDlls>c__AnonStorey0 storey = new <CrossCompileManagedDlls>c__AnonStorey0 {
                paths = paths,
                usedClassRegistry = usedClassRegistry
            };
            IncludedFileList includedFiles = new IncludedFileList();
            CrossCompileOptions crossCompileOptions = GetCrossCompileOptions(bs);
            if (!bs.UseIl2Cpp())
            {
                CleanupMDBFiles(storey.paths.stagingAreaDataManaged);
                if (!MonoCrossCompile.CrossCompileAOTDirectoryParallel(GetCrossCompilerPath(storey.paths.playerPackage), bs.target, crossCompileOptions, storey.paths.stagingAreaDataManaged, storey.paths.LibrariesStaging(), PlayerSettings.aotOptions))
                {
                    throw new UnityException("Cross compilation failed.");
                }
                bool stripping = (PlayerSettings.strippingLevel > StrippingLevel.Disabled) && (bs.sdkType == SdkType.Device);
                string file = Path.Combine(storey.paths.LibrariesStaging(), "RegisterMonoModules.cpp");
                iOSIl2CppPlatformProvider platformProvider = new iOSIl2CppPlatformProvider(BuildTarget.iOS, bs.isDevelopmentPlayer, storey.paths.stagingAreaData, buildReport);
                MonoAOTRegistration.WriteCPlusPlusFileForStaticAOTModuleRegistration(bs.target, file, crossCompileOptions, true, PlayerSettings.iOS.targetDevice.ToString(), stripping, storey.usedClassRegistry, checker, storey.paths.stagingAreaDataManaged, platformProvider);
                return includedFiles;
            }
            <CrossCompileManagedDlls>c__AnonStorey1 storey2 = new <CrossCompileManagedDlls>c__AnonStorey1 {
                <>f__ref$0 = storey
            };
            FileUtil.CopyFileOrDirectory(storey.paths.playerPackage + "/il2cpp/ios_il2cpp_link.xml", storey.paths.stagingAreaData + "/platform_native_link.xml");
            storey2.platformProvider = new iOSIl2CppPlatformProvider(BuildTarget.iOS, bs.isDevelopmentPlayer, storey.paths.stagingAreaData, bs.buildReport);
            Action<string> modifyOutputBeforeCompile = new Action<string>(storey2.<>m__0);
            storey.paths.SetInternalIl2cppOutputPath(IL2CPPUtils.RunIl2Cpp("Temp/il2cppOutput", storey.paths.stagingAreaData, storey2.platformProvider, modifyOutputBeforeCompile, storey.usedClassRegistry, false).GetCppOutputDirectoryInStagingArea());
            AddIl2CppOutputFiles(includedFiles, storey.paths.InternalIl2cppOutputPath());
            string dir = storey.paths.stagingAreaDataManaged + "/Resources";
            FileUtil.CreateOrCleanDirectory(dir);
            IL2CPPUtils.CopyEmbeddedResourceFiles("Temp/il2cppOutput", dir);
            string str4 = storey.paths.stagingAreaDataManaged + "/Metadata";
            FileUtil.CreateOrCleanDirectory(str4);
            IL2CPPUtils.CopyMetadataFiles("Temp/il2cppOutput", str4);
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                FileUtil.CopyFileOrDirectory(Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), "Tools/MapFileParser/MapFileParser"), Path.Combine(storey.paths.StagingTrampoline(), "MapFileParser"));
            }
            bool flag2 = (crossCompileOptions & CrossCompileOptions.FastICall) != CrossCompileOptions.Dynamic;
            WriteIl2CppOptionsFile(Path.Combine(storey.paths.LibrariesStaging(), "Il2CppOptions.cpp"), flag2);
            return includedFiles;
        }

        private static UsedFeatures DetermineUsedFeatures(AssemblyReferenceChecker checker)
        {
            bool flag = checker.HasReferenceToMethod("ReplayKit::StartBroadcasting(UnityEngine.Apple.ReplayKit.ReplayKit/BroadcastStatusCallback,System.Boolean", true) || checker.HasReferenceToMethod("ReplayKit::StartRecording(System.Boolean", true);
            return new UsedFeatures { 
                camera = checker.HasReferenceToType("UnityEngine.WebCamTexture") || flag,
                gameCenter = checker.HasReferenceToType("UnityEngine.Social"),
                gyroscope = checker.HasReferenceToType("UnityEngine.Gyroscope"),
                location = checker.HasReferenceToType("UnityEngine.LocationService"),
                microphone = checker.HasReferenceToType("UnityEngine.Microphone") || flag,
                remoteNotifications = checker.HasReferenceToType("UnityEngine.iOS.RemoteNotification") || checker.HasReferenceToMethod("UnityEngine.iOS.NotificationServices::RegisterForNotifications"),
                replayKit = checker.HasReferenceToType("UnityEngine.Apple.ReplayKit"),
                controller = checker.HasReferenceToMethod("UnityEngine.Input::GetJoystickNames"),
                stylus = (checker.HasReferenceToMethod("UnityEngine.Touch::get_azimuthAngle") || checker.HasReferenceToMethod("UnityEngine.Touch::get_altitudeAngle")) || checker.HasReferenceToMethod("UnityEngine.Touch::get_orientation")
            };
        }

        private static UnityType FindTypeByNameChecked(string name)
        {
            UnityType type = UnityType.FindTypeByName(name);
            if (type == null)
            {
                throw new ArgumentException($"Could not map typename '{name}' to type info (IPhonePlayer class registration skipped classes)");
            }
            return type;
        }

        private static List<string> GetAvailableIconsList(BuildSettings bs)
        {
            List<string> list = new List<string>();
            if (bs.IsIPhoneEnabled())
            {
                list.Add("AppIcon57x57.png");
                list.Add("AppIcon57x57@2x.png");
                list.Add("AppIcon60x60@2x.png");
            }
            if (bs.IsIPadEnabled())
            {
                list.Add("AppIcon72x72@2x~ipad.png");
                list.Add("AppIcon72x72~ipad.png");
                list.Add("AppIcon76x76@2x~ipad.png");
                list.Add("AppIcon76x76~ipad.png");
            }
            return list;
        }

        private static AvailableOrientations GetAvailableOrientations()
        {
            AvailableOrientations orientations = new AvailableOrientations();
            UIOrientation defaultInterfaceOrientation = PlayerSettings.defaultInterfaceOrientation;
            if (PlayerSettings.virtualRealitySupported)
            {
                defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            }
            bool flag = defaultInterfaceOrientation == UIOrientation.AutoRotation;
            if ((defaultInterfaceOrientation == UIOrientation.Portrait) || (flag && PlayerSettings.allowedAutorotateToPortrait))
            {
                orientations.portrait = true;
            }
            if ((defaultInterfaceOrientation == UIOrientation.PortraitUpsideDown) || (flag && PlayerSettings.allowedAutorotateToPortraitUpsideDown))
            {
                orientations.portraitUpsideDown = true;
            }
            if ((defaultInterfaceOrientation == UIOrientation.LandscapeLeft) || (flag && PlayerSettings.allowedAutorotateToLandscapeLeft))
            {
                orientations.landscapeLeft = true;
            }
            if ((defaultInterfaceOrientation == UIOrientation.LandscapeRight) || (flag && PlayerSettings.allowedAutorotateToLandscapeRight))
            {
                orientations.landscapeRight = true;
            }
            return orientations;
        }

        private static BuildTargetGroup GetBuildTargetGroup(BuildTarget target)
        {
            if (target != BuildTarget.iOS)
            {
                if (target != BuildTarget.tvOS)
                {
                    throw new Exception("Invalid build target");
                }
            }
            else
            {
                return BuildTargetGroup.iPhone;
            }
            return BuildTargetGroup.tvOS;
        }

        private static string GetCrashSubmissionUrl(Uri symbolsServiceBaseUri)
        {
            if (symbolsServiceBaseUri != null)
            {
                return new Uri(symbolsServiceBaseUri, "symbolicate").ToString();
            }
            return string.Empty;
        }

        private static CrossCompileOptions GetCrossCompileOptions(BuildSettings bs)
        {
            CrossCompileOptions @static = CrossCompileOptions.Static;
            if (PlayerSettings.iOS.scriptCallOptimization >= ScriptCallOptimizationLevel.FastButNoExceptions)
            {
                @static |= CrossCompileOptions.FastICall;
            }
            if (bs.allowDebugging && bs.isDevelopmentPlayer)
            {
                @static |= CrossCompileOptions.Debugging;
                @static |= CrossCompileOptions.ExplicitNullChecks;
            }
            return @static;
        }

        private static string GetCrossCompilerPath(string playerPackage)
        {
            string str;
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                str = "/mono-xcompiler-wrapper.sh";
            }
            else
            {
                str = "/mono-xcompiler" + Utils.GetOSExeExtension();
            }
            return Path.Combine(playerPackage, "Tools/" + Utils.GetOSPathPart() + str);
        }

        private static Func<string, string, bool> GetFileComparer(BuildTargetGroup targetGroup)
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new Func<string, string, bool>(FileMirroring.CanSkipCopy);
            }
            return <>f__mg$cache0;
        }

        private static string GetPlayerLibLocation(ProjectPaths paths, BuildSettings bs)
        {
            string str = (bs.target != BuildTarget.tvOS) ? "libiPhone-lib" : "libunity-appletv";
            string str2 = "";
            if (bs.sdkType == SdkType.Simulator)
            {
                if (bs.backend == ScriptingImplementation.IL2CPP)
                {
                    str2 = str2 + "-amd64-il2cpp";
                }
                else
                {
                    str2 = str2 + "-i386";
                }
            }
            else
            {
                if (bs.backend == ScriptingImplementation.IL2CPP)
                {
                    str2 = str2 + "-il2cpp";
                }
                if (bs.isDevelopmentPlayer)
                {
                    str2 = str2 + "-dev";
                }
            }
            return Path.Combine(paths.EditorTrampoline(), $"Libraries/{str}{str2}.a");
        }

        private static string GetPluginDestinationPath(string destDir, string assetPath)
        {
            if (assetPath.StartsWith("Assets"))
            {
                return Path.Combine(destDir, Utils.GetRelativePath("Assets", assetPath));
            }
            return Path.Combine(destDir, Path.GetFileName(assetPath));
        }

        internal static string GetProjectPath(IncludedFile file)
        {
            if (file.projectPath != null)
            {
                return file.projectPath;
            }
            return file.destinationPath;
        }

        private static PBXSourceTree GetSourceTreeForPath(string path)
        {
            if (Path.IsPathRooted(path))
            {
                return PBXSourceTree.Absolute;
            }
            return PBXSourceTree.Source;
        }

        private static string GetStatusBarStyle(iOSStatusBarStyle style)
        {
            if (style != iOSStatusBarStyle.Default)
            {
                if (style == iOSStatusBarStyle.BlackTranslucent)
                {
                    return "UIStatusBarStyleBlackTranslucent";
                }
                if (style == iOSStatusBarStyle.BlackOpaque)
                {
                    return "UIStatusBarStyleBlackOpaque";
                }
            }
            else
            {
                return "UIStatusBarStyleDefault";
            }
            return "";
        }

        private static string GetTargetArchitecture(BuildSettings bs)
        {
            if (bs.sdkType == SdkType.Simulator)
            {
                if (bs.UseIl2Cpp())
                {
                    return "x86_64";
                }
                return "i386";
            }
            if (!bs.UseIl2Cpp())
            {
                return "armv7";
            }
            switch (PlayerSettings.GetArchitecture(bs.targetGroup))
            {
                case 0:
                    return "armv7";

                case 1:
                    return "arm64";

                case 2:
                    return "armv7 arm64";
            }
            throw new Exception("Architecture not supported");
        }

        private static string GetTargetDeviceFamily(BuildSettings bs)
        {
            if (bs.IsIPhoneEnabled() && bs.IsIPadEnabled())
            {
                return "1,2";
            }
            if (bs.IsIPhoneEnabled())
            {
                return "1";
            }
            if (bs.IsIPadEnabled())
            {
                return "2";
            }
            if (!bs.IsAppleTVEnabled())
            {
                throw new Exception("Wrong target device family");
            }
            return "3";
        }

        private static double GetTotalAssembliesSizeMB(string directoryPath, string[] fileNames)
        {
            double num = 0.0;
            foreach (string str in fileNames)
            {
                FileInfo info = new FileInfo(Path.Combine(directoryPath, str));
                num += info.Length;
            }
            return (num / 1048576.0);
        }

        private static string GetUsymUploadAuthToken(BuildSettings bs)
        {
            string str = string.Empty;
            try
            {
                string usymUploadAuthToken = bs.usymUploadAuthToken;
                if (!string.IsNullOrEmpty(usymUploadAuthToken))
                {
                    return usymUploadAuthToken;
                }
                string unityConnectAccessToken = bs.unityConnectAccessToken;
                if (string.IsNullOrEmpty(unityConnectAccessToken))
                {
                    return string.Empty;
                }
                Uri requestUri = new Uri(bs.symbolsServiceBaseUri, $"token/{bs.cloudProjectId}");
                WebRequest request = WebRequest.Create(requestUri);
                request.Headers.Add("Authorization", $"Bearer {unityConnectAccessToken}");
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                string jsondata = string.Empty;
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    jsondata = reader.ReadToEnd();
                }
                JSONValue value2 = JSONParser.SimpleParse(jsondata);
                if (value2.ContainsKey("AuthToken"))
                {
                    str = value2["AuthToken"].AsString();
                }
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogException(exception);
            }
            return str;
        }

        private static void InstallIcons(ProjectPaths paths, BuildSettings bs)
        {
            if (bs.target == BuildTarget.tvOS)
            {
                InstallIconsTVOS(paths, bs.companyName);
            }
            else
            {
                InstallIconsIOS(paths, bs);
            }
        }

        private static void InstallIconsIOS(ProjectPaths paths, BuildSettings bs)
        {
            List<Icon> icons = new List<Icon>();
            foreach (Icon icon in Icon.types)
            {
                string path = Path.Combine(paths.IconPathInstall(), icon.xcodefile);
                FileUtil.DeleteFileOrDirectory(path);
                if (bs.IsDeviceTypeEnabled(icon.deviceType))
                {
                    string[] srcFiles = new string[] { Path.Combine(paths.IconOutputFolder(), icon.xcodefile), Path.Combine(paths.IconSourceFolder(), icon.xcodefile) };
                    if (Utils.InstallFileWithFallbacks(srcFiles, path) != -1)
                    {
                        icons.Add(icon);
                    }
                }
            }
            System.IO.File.WriteAllText(Path.Combine(paths.IconPathInstall(), "Contents.json"), IconJsonUpdater.CreateJsonString(icons, PlayerSettings.iOS.prerenderedIcon));
        }

        private static void InstallIconsTVOS(ProjectPaths paths, string iPhoneCompanyName)
        {
            AssetCatalog catalog = new AssetCatalog(paths.UnityImagesAssetCatalogInstall(), iPhoneCompanyName);
            AssetBrandAssetGroup assetGroup = catalog.OpenBrandAssetGroup("AppIcon");
            PopulateLayerImagesToAssetCatalog(assetGroup, "App Icon - Small", "primary-app-icon", 400, 240, paths.tvOSSmallIconOutputBase(), IntClamp(PlayerSettings.tvOS.GetSmallIconLayers().Length, 2, 5));
            PopulateLayerImagesToAssetCatalog(assetGroup, "App Icon - Large", "primary-app-icon", 0x500, 0x300, paths.tvOSLargeIconOutputBase(), IntClamp(PlayerSettings.tvOS.GetLargeIconLayers().Length, 2, 5));
            PopulateLayerImagesToAssetCatalog(assetGroup, "Top Shelf Image", "top-shelf-image", 0x780, 720, paths.tvOSTopShelfImageOutputBase(), IntClamp(PlayerSettings.tvOS.GetTopShelfImageLayers().Length, 1, 5));
            PopulateLayerImagesToAssetCatalog(assetGroup, "Top Shelf Image Wide", "top-shelf-image-wide", 0x910, 720, paths.tvOSTopShelfImageWideOutputBase(), IntClamp(PlayerSettings.tvOS.GetTopShelfImageWideLayers().Length, 1, 5));
            List<string> warnings = new List<string>();
            catalog.Write(warnings);
            foreach (string str in warnings)
            {
                UnityEngine.Debug.LogWarning(str);
            }
            if (Directory.Exists(paths.IconPathInstall()))
            {
                Directory.Delete(paths.IconPathInstall(), true);
            }
        }

        private static void InstallIncludedFiles(IncludedFileList includedFiles, string installPath, BuildSettings bs)
        {
            foreach (IncludedFile file in includedFiles)
            {
                if (file.sourcePath != null)
                {
                    string sourcePath = file.sourcePath;
                    string destinationPath = file.destinationPath;
                    destinationPath = Path.Combine(installPath, destinationPath);
                    FileUtil.DeleteFileOrDirectory(destinationPath);
                    if (Directory.Exists(sourcePath))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                        FileUtil.CopyDirectoryRecursive(sourcePath, destinationPath);
                    }
                    else if ((file.symlinkType == SymlinkType.None) || !bs.symlinkLibraries)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                        if (!System.IO.File.Exists(sourcePath))
                        {
                            throw new Exception($"The required file: '{sourcePath}' does not exist");
                        }
                        FileMirroring.MirrorFile(sourcePath, destinationPath, GetFileComparer(bs.targetGroup));
                    }
                    else if (bs.symlinkLibraries && (file.symlinkType == SymlinkType.Symlink))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                        System.IO.File.Delete(destinationPath);
                        if (ShouldUseRelativeSymlinkForFile(sourcePath, destinationPath))
                        {
                            Utils.SymlinkFileRelative(sourcePath, destinationPath);
                        }
                        else
                        {
                            Utils.SymlinkFileAbsolute(sourcePath, destinationPath);
                        }
                    }
                }
            }
        }

        private static void InstallSplashScreens(ProjectPaths paths, BuildSettings bs)
        {
            List<UnityEditor.iOS.SplashScreen> splashes = new List<UnityEditor.iOS.SplashScreen>();
            UnityEditor.iOS.SplashScreen[] screenArray = (bs.target != BuildTarget.tvOS) ? UnityEditor.iOS.SplashScreen.iOSTypes : UnityEditor.iOS.SplashScreen.tvOSTypes;
            foreach (UnityEditor.iOS.SplashScreen screen in screenArray)
            {
                string path = Path.Combine(paths.SplashPathInstall(), screen.xcodeFile);
                FileUtil.DeleteFileOrDirectory(path);
                if (bs.IsDeviceTypeEnabled(screen.deviceType))
                {
                    string[] srcFiles = new string[] { Path.Combine(paths.SplashScreenOutputFolder(), screen.xcodeFile), Path.Combine(paths.SplashScreenSourceFolder(), screen.defaultSource) };
                    if (Utils.InstallFileWithFallbacks(srcFiles, path) != -1)
                    {
                        splashes.Add(screen);
                    }
                    else if (screen.deviceType == UnityEditor.iOS.DeviceType.AppleTV)
                    {
                        splashes.Add(screen);
                    }
                }
            }
            System.IO.File.WriteAllText(Path.Combine(paths.SplashPathInstall(), "Contents.json"), SplashJsonUpdater.CreateJsonString(splashes));
        }

        private static int IntClamp(int count, int min, int max)
        {
            if (count < min)
            {
                count = min;
            }
            if (count > max)
            {
                count = max;
            }
            return count;
        }

        internal static bool IsBundleIdAcceptableByXcode(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            return new Regex("^[a-zA-Z0-9-.]+$").IsMatch(id);
        }

        internal static bool IsBundleIdInReverseDnsFormat(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            return new Regex(@"^[a-zA-Z]+(\.[a-zA-Z0-9-]+){2,}$").IsMatch(id);
        }

        private static bool IsHeaderFile(string fileName) => 
            (Path.GetExtension(fileName) == ".h");

        private static bool IsIl2CppEmbeddedResourcesFile(string fileName) => 
            fileName.EndsWith("-resources.dat");

        internal static bool IsiOSBundlePlugin(string path) => 
            Utils.IsPathExtensionOneOf(path, kiOSBundlePluginExtensions);

        private static bool IsMetalUsed(BuildSettings bs)
        {
            if (!PlayerSettings.GetUseDefaultGraphicsAPIs(bs.target))
            {
                GraphicsDeviceType[] graphicsAPIs = PlayerSettings.GetGraphicsAPIs(bs.target);
                return ((graphicsAPIs.Length > 0) && (graphicsAPIs[0] == GraphicsDeviceType.Metal));
            }
            return true;
        }

        public static void Launch(BuildLaunchPlayerArgs args)
        {
            string str2;
            BuildTarget target = args.target;
            string installPath = args.installPath;
            bool flag = (args.options & BuildOptions.AcceptExternalModificationsToPlayer) != BuildOptions.CompressTextures;
            if (installPath.StartsWith("/") || installPath.StartsWith("~"))
            {
                str2 = installPath;
            }
            else
            {
                str2 = Path.Combine(Directory.GetCurrentDirectory(), installPath);
            }
            LaunchInXcode(UnityEditor.BuildPipeline.GetBuildToolsDirectory(target), str2, !flag);
        }

        public static void LaunchInXcode(string buildToolsDir, string buildPath, bool cleanProject)
        {
            IXcodeController xcodeController = XcodeController;
            try
            {
                if (xcodeController.InitializeXcodeApplication(buildToolsDir))
                {
                    string projectPath = Path.Combine(buildPath, "Unity-iPhone.xcodeproj");
                    xcodeController.CloseAllOpenUnityProjects(buildToolsDir, projectPath);
                    EditorUtility.DisplayProgressBar("Building Player", "Build & Install iPhone player", 1f);
                    xcodeController.OpenProject(projectPath);
                    if (PlayerSettings.iOS.sdkVersion == iOSSdkVersion.DeviceSDK)
                    {
                        xcodeController.SelectDeviceScheme(projectPath);
                    }
                    else
                    {
                        xcodeController.SelectSimulatorScheme("iossimulator Simulator");
                    }
                    if (cleanProject)
                    {
                        xcodeController.CleanProject(projectPath);
                    }
                    xcodeController.RunProject(projectPath);
                }
            }
            catch (Exception exception)
            {
                object[] param = new object[] { exception };
                xcodeController.Log("{0}", param);
                throw new UnityException("Launching iOS project via Xcode failed. Check editor log for details.");
            }
        }

        private static void ModifyIl2CppOutputDirBeforeCompile(ProjectPaths paths, string outputDir, RuntimeClassRegistry classRegistry, string engineDll, IIl2CppPlatformProvider platformProvider)
        {
            string directoryName = Path.GetDirectoryName(Path.GetFullPath(engineDll));
            System.IO.File.Copy(Path.Combine(directoryName, "UnityICallRegistration.cpp"), Path.Combine(outputDir, "UnityICallRegistration.cpp"));
            UnityType[] classesToSkip = new UnityType[] { FindTypeByNameChecked("ClusterInputManager") };
            CodeStrippingUtils.WriteModuleAndClassRegistrationFile(directoryName, Path.Combine(directoryName, "ICallSummary.txt"), outputDir, classRegistry, classesToSkip, platformProvider);
        }

        private static void PopulateLayerImagesToAssetCatalog(AssetBrandAssetGroup assetGroup, string name, string role, int width, int height, string basePath, int count)
        {
            if (count >= 1)
            {
                if (count == 1)
                {
                    string path = string.Format(basePath, "0");
                    AssetImageSet set = assetGroup.OpenImageSet(name, "tv", role, width, height);
                    DeviceRequirement requirement = new DeviceRequirement().AddDevice("tv").AddScale("1x");
                    set.AddVariant(requirement, path);
                }
                else
                {
                    AssetImageStack stack = assetGroup.OpenImageStack(name, "tv", role, width, height);
                    for (int i = 0; i < count; i++)
                    {
                        string str2 = string.Format(basePath, i);
                        if (!System.IO.File.Exists(str2))
                        {
                            for (int j = 0; j < count; j++)
                            {
                                string str3 = string.Format(basePath, j);
                                if (System.IO.File.Exists(str3))
                                {
                                    str2 = str3;
                                    UnityEngine.Debug.LogWarning($"Not all image layers are defined for '{name}'. Duplicating another layer");
                                    break;
                                }
                            }
                        }
                        AssetImageStackLayer layer = stack.AddLayer($"Layer-{i}");
                        DeviceRequirement requirement2 = new DeviceRequirement().AddDevice("tv").AddScale("1x");
                        layer.GetImageSet().AddVariant(requirement2, str2);
                    }
                }
            }
        }

        internal static void PostProcess(PostProcessorSettings postProcessorSettings, BuildPostProcessArgs args)
        {
            BuildSettings bs = SetupBuildSettings(postProcessorSettings, args);
            VerifyBuildSettings(bs);
            ProjectPaths paths = SetupProjectPaths(args, bs.installInBuildFolder);
            if (bs.appendMode && !System.IO.File.Exists(paths.PBXProjectInstall()))
            {
                UnityEngine.Debug.LogWarning("iOS project doesn't seem to exist, can't append, using replace mode.");
                bs.appendMode = false;
            }
            PostProcess(bs, paths, args.usedClassRegistry, args.report);
        }

        private static void PostProcess(BuildSettings bs, ProjectPaths paths, RuntimeClassRegistry usedClassRegistry, BuildReport buildReport)
        {
            Regex[] ignoreList = new Regex[] { new Regex(@"lib.*\.a"), new Regex(@"\.DS_Store") };
            Utils.CopyRecursiveWithIgnoreList(paths.EditorTrampoline(), paths.StagingTrampoline(), ignoreList);
            FileUtil.CopyFileOrDirectory(paths.DataStaging() + "/unity default resources", paths.stagingAreaData + "/unity default resources");
            IncludedFileList includedFiles = new IncludedFileList();
            if (!bs.installInBuildFolder)
            {
                includedFiles.Add(GetPlayerLibLocation(paths, bs), "Libraries/libiPhone-lib.a");
            }
            string etcDirectory = MonoInstallationFinder.GetEtcDirectory("Mono");
            string dir = paths.stagingAreaData + "/Managed/mono/2.0";
            FileUtil.CreateOrCleanDirectory(dir);
            FileUtil.CopyFileOrDirectory(etcDirectory + "/2.0/machine.config", dir + "/machine.config");
            if (bs.nativeCrashReportingEnabled)
            {
                FileUtil.CopyFileOrDirectory(Path.Combine(EditorApplication.applicationContentsPath, "Tools/usymtool"), Path.Combine(paths.StagingTrampoline(), "usymtool"));
                FileUtil.CopyFileOrDirectory(Path.Combine(EditorApplication.applicationContentsPath, "Tools/lzma"), Path.Combine(paths.StagingTrampoline(), "lzma"));
            }
            AssemblyReferenceChecker checker = new AssemblyReferenceChecker();
            bool collectMethods = true;
            bool ignoreSystemDlls = false;
            checker.CollectReferences(paths.stagingAreaDataManaged, collectMethods, 0f, ignoreSystemDlls);
            IncludedFileList collection = CrossCompileManagedDlls(bs, paths, checker, usedClassRegistry, buildReport);
            includedFiles.AddRange(collection);
            if (ShouldStripByteCodeInManagedDlls(bs))
            {
                StripByteCodeInManagedDlls(bs.target, paths, checker);
            }
            UsedFeatures usedFeatures = DetermineUsedFeatures(checker);
            WriteFeaturesRegistrationFile(paths.FeatureRegistrationStaging(), usedFeatures);
            if (bs.appendMode && System.IO.File.Exists(paths.PBXProjectInstall()))
            {
                Utils.ReplaceFileOrDirectoryCopy(paths.PBXProjectInstall(), paths.PBXProjectStaging());
            }
            if (bs.installInBuildFolder)
            {
                EditorUtility.DisplayProgressBar("Preparing development project", "Preparing development project", 0.96f);
                if (!PrepareDevProject(paths, bs))
                {
                    string str3 = "Failed to prepare development project. ";
                    throw new SystemException(str3 + "Add -sUNITY_IOS_DEV_PROJECT=1 when running jam to find out why.");
                }
                FileUtil.CopyFileOrDirectory(paths.DevProjectEditorTrampoline(), paths.DevProjectStagingTrampoline());
            }
            if (Directory.Exists("Assets/StreamingAssets"))
            {
                FileUtil.CopyDirectoryRecursiveForPostprocess("Assets/StreamingAssets", paths.stagingAreaData + "/Raw", true);
            }
            Utils.ReplaceFileOrDirectoryCopy(paths.stagingAreaData, paths.DataStaging());
            if (bs.UseIl2Cpp())
            {
                Regex[] matchList = new Regex[] { new Regex(@"\.dll$"), new Regex(@"\.mdb$") };
                Utils.DeleteRecursiveWithMatchList(Path.Combine(paths.DataStaging(), "Managed"), matchList);
                string[] strArray = new string[] { "methods_pointedto_by_uievents.xml", "platform_native_link.xml", "preserved_derived_types.xml", "Managed/ICallSummary.txt", "Managed/UnityICallRegistration.cpp", "Managed/tempUnstripped" };
                foreach (string str4 in strArray)
                {
                    string path = Path.Combine(paths.DataStaging(), str4);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    else if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                    }
                }
            }
            if (!bs.UseIl2Cpp())
            {
                AddiPhoneAssemblyFiles(includedFiles, checker.GetAssemblyFileNames());
            }
            List<string> frameworks = new List<string>();
            AddPluginFiles(bs.target, includedFiles, frameworks);
            AddPluginPNGFiles(includedFiles);
            if (bs.target == BuildTarget.tvOS)
            {
                includedFiles.Add(null, "UnityImages.xcassets");
            }
            else
            {
                AddLaunchScreenFilesIOS(paths, bs, includedFiles);
            }
            AddCollectedResources(paths, includedFiles, PlayerSettings.iOS.useOnDemandResources, bs.companyName);
            if (bs.installInBuildFolder)
            {
                AddXcodeDevProjectAdditionalLibs(paths, includedFiles);
            }
            UpdateXcScheme(paths, bs.buildType);
            includedFiles.Add(null, "Classes/Unity/IUnityInterface.h");
            includedFiles.Add(null, "Classes/Unity/IUnityGraphics.h");
            includedFiles.Add(null, "Classes/Unity/IUnityGraphicsMetal.h");
            List<string> initialInstallTags = UnityEditor.iOS.BuildPipeline.CollectInitialInstallTags();
            BuildXCodeProject(paths, bs, usedFeatures, includedFiles, frameworks, initialInstallTags);
            if (bs.installInBuildFolder)
            {
                IncludedFileList list5 = new IncludedFileList();
                AddXcodeDevProjectBuildFiles(paths, list5);
                BuildXCodeUnityDevProject(paths, list5);
            }
            if (bs.appendMode && System.IO.File.Exists(paths.PlistInstall()))
            {
                Utils.ReplaceFileOrDirectoryCopy(paths.PlistInstall(), paths.PlistStaging());
            }
            string iPhoneLaunchStoryboardName = !System.IO.File.Exists(paths.LaunchScreeniPhoneXibStaging()) ? null : "LaunchScreen-iPhone";
            string iPadLaunchStoryboardName = !System.IO.File.Exists(paths.LaunchScreeniPadXibStaging()) ? null : "LaunchScreen-iPad";
            UpdateInfoPlist(paths.PlistStaging(), bs, iPhoneLaunchStoryboardName, iPadLaunchStoryboardName);
            if (usedFeatures.camera && string.IsNullOrEmpty(PlayerSettings.iOS.cameraUsageDescription))
            {
                UnityEngine.Debug.LogWarning("WebCamTexture class is used but Camera Usage Description is empty. App will not work on iOS 10+.");
            }
            if (usedFeatures.location && string.IsNullOrEmpty(PlayerSettings.iOS.locationUsageDescription))
            {
                UnityEngine.Debug.LogWarning("LocationService class is used but Locations Usage Description is empty. App will not work on iOS 10+.");
            }
            if (usedFeatures.microphone && string.IsNullOrEmpty(PlayerSettings.iOS.microphoneUsageDescription))
            {
                UnityEngine.Debug.LogWarning("Microphone class is used but Microphone Usage Description is empty. App will not work on iOS 10+.");
            }
            if (!bs.appendMode && bs.symlinkTrampoline)
            {
                SymlinkTrampoline(Path.Combine(paths.UnityTree(), "PlatformDependent/iPhonePlayer/Trampoline"), paths.StagingTrampoline());
            }
            UpdateInstallLocation(paths, bs, includedFiles);
            if (!bs.symlinkTrampoline)
            {
                UpdateCppDefines(paths.installPath, bs, usedFeatures);
            }
        }

        private static bool PrepareDevProject(ProjectPaths paths, BuildSettings bs)
        {
            string str = !bs.isDevelopmentPlayer ? "release" : "debug";
            string str2 = !bs.isDevelopmentPlayer ? "0" : "1";
            string str3 = (bs.backend != ScriptingImplementation.IL2CPP) ? "mono" : "il2cpp";
            string str4 = (bs.sdkType != SdkType.Simulator) ? "iOSPlayer" : "iOSSimulatorPlayer";
            ProcessStartInfo startInfo = new ProcessStartInfo {
                FileName = Path.Combine(paths.UnityTree(), "jam"),
                Arguments = string.Format("-g -sPLATFORM=iphone -sCONFIG={0} -sDEVELOPMENT_PLAYER={1} -sSCRIPTING_BACKEND={2} -sTARGETNAME={3} -sUNITY_IOS_DEV_PROJECT=1 {3}", new object[] { 
                    str,
                    str2,
                    str3,
                    str4
                }),
                WorkingDirectory = paths.UnityTree(),
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process process = Process.Start(startInfo);
            process.WaitForExit();
            return (process.ExitCode == 0);
        }

        internal static void ReplaceCppMacro(string[] lines, string name, string newValue)
        {
            Regex regex = new Regex(@"^.*#\s*define\s+" + name);
            Regex regex2 = new Regex(@"^.*#\s*define\s+" + name + @"(:?|\s|\s.*[^\\])$");
            for (int i = 0; i < lines.Count<string>(); i++)
            {
                if (regex.Match(lines[i]).Success)
                {
                    lines[i] = regex2.Replace(lines[i], "#define " + name + " " + newValue);
                }
            }
        }

        private static bool RunJam(ProjectPaths paths, string config, string devPlayer, string backend, string platform, string target, string extraArgs, string progressTitle, string progressMessage)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo {
                    FileName = Path.Combine(paths.UnityTree(), "jam"),
                    Arguments = string.Format("-sPLATFORM={0} -sCONFIG={1} -sDEVELOPMENT_PLAYER={2} -sSCRIPTING_BACKEND={3} -sTARGETNAME={4} {5} {4}", new object[] { 
                        platform,
                        config,
                        devPlayer,
                        backend,
                        target,
                        extraArgs
                    }),
                    WorkingDirectory = paths.UnityTree(),
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                if (progressTitle != null)
                {
                    EditorUtility.DisplayProgressBar(progressTitle, (progressMessage != null) ? progressMessage : progressTitle, 0f);
                    startInfo.RedirectStandardOutput = true;
                }
                Process process = Process.Start(startInfo);
                if (progressTitle != null)
                {
                    if (progressMessage == null)
                    {
                        progressMessage = progressTitle;
                    }
                    for (string str = process.StandardOutput.ReadLine(); str != null; str = process.StandardOutput.ReadLine())
                    {
                        Console.WriteLine(str);
                        if (str.StartsWith("*** completed "))
                        {
                            int length = str.IndexOf('%', "*** completed ".Length) - "*** completed ".Length;
                            float result = 0f;
                            if ((length > 0) && float.TryParse(str.Substring("*** completed ".Length, length), out result))
                            {
                                int index = str.IndexOf('(', ("*** completed ".Length + length) + 1);
                                string str2 = "";
                                if (index > 0)
                                {
                                    length = str.IndexOf(')', index) - index;
                                    if (length > 0)
                                    {
                                        str2 = " " + str.Substring(index, length + 1);
                                    }
                                }
                                EditorUtility.DisplayProgressBar(progressTitle, progressMessage + str2, result / 100f);
                            }
                        }
                    }
                }
                process.WaitForExit();
                if (progressTitle != null)
                {
                    EditorUtility.ClearProgressBar();
                }
                return (process.ExitCode == 0);
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogException(exception);
                if (progressTitle != null)
                {
                    EditorUtility.ClearProgressBar();
                }
                return false;
            }
        }

        private static BuildSettings SetupBuildSettings(PostProcessorSettings postProcessorSettings, BuildPostProcessArgs args)
        {
            string targetOSVersionString;
            BuildSettings bs = new BuildSettings {
                useOnDemandResources = PlayerSettings.iOS.useOnDemandResources,
                target = args.target,
                targetGroup = GetBuildTargetGroup(args.target),
                targetDevice = PlayerSettings.iOS.targetDevice
            };
            bs.backend = PlayerSettings.GetScriptingBackend(bs.targetGroup);
            bs.buildReport = args.report;
            bs.symlinkTrampoline = EditorUserBuildSettings.symlinkTrampoline;
            bs.buildType = EditorUserBuildSettings.iOSBuildConfigType;
            if (args.target == BuildTarget.iOS)
            {
                bs.sdkType = (PlayerSettings.iOS.sdkVersion != iOSSdkVersion.DeviceSDK) ? SdkType.Simulator : SdkType.Device;
                targetOSVersionString = PlayerSettings.iOS.targetOSVersionString;
            }
            else
            {
                bs.sdkType = (PlayerSettings.tvOS.sdkVersion != tvOSSdkVersion.Device) ? SdkType.Simulator : SdkType.Device;
                targetOSVersionString = PlayerSettings.tvOS.targetOSVersionString;
            }
            if (Utils.ParseVersion(targetOSVersionString, out bs.targetOsVersion))
            {
                if (bs.targetOsVersion < postProcessorSettings.MinimumOsVersion)
                {
                    UnityEngine.Debug.LogWarning($"{postProcessorSettings.OsName} deployment target version is set to {bs.targetOsVersion}, but Unity supports only versions starting from {postProcessorSettings.MinimumOsVersion}.");
                }
            }
            else
            {
                bs.targetOsVersion = postProcessorSettings.MinimumOsVersion;
            }
            bs.appendMode = (args.options & BuildOptions.AcceptExternalModificationsToPlayer) != BuildOptions.CompressTextures;
            bs.isDevelopmentPlayer = (args.options & BuildOptions.Development) != BuildOptions.CompressTextures;
            bs.symlinkLibraries = (args.options & BuildOptions.SymlinkLibraries) != BuildOptions.CompressTextures;
            bs.installInBuildFolder = (args.options & BuildOptions.InstallInBuildFolder) != BuildOptions.CompressTextures;
            bs.allowDebugging = (args.options & BuildOptions.AllowDebugging) != BuildOptions.CompressTextures;
            bs.autoRunPlayer = (args.options & BuildOptions.AutoRunPlayer) != BuildOptions.CompressTextures;
            if ((bs.appendMode && (Application.platform != RuntimePlatform.OSXEditor)) && (Application.platform != RuntimePlatform.WindowsEditor))
            {
                UnityEngine.Debug.LogWarning("iOS project appending is only supported on Windows and OSX, falling back to full build.");
                bs.appendMode = false;
            }
            if (bs.installInBuildFolder)
            {
                bs.appendMode = false;
            }
            BuildTargetGroup targetGroup = (args.target != BuildTarget.tvOS) ? BuildTargetGroup.iPhone : BuildTargetGroup.tvOS;
            string applicationIdentifier = PlayerSettings.GetApplicationIdentifier(targetGroup);
            CheckIOSBundleIdentifier(applicationIdentifier, bs);
            bs.productName = applicationIdentifier.Substring(applicationIdentifier.LastIndexOf(".") + 1);
            bs.companyName = applicationIdentifier.Substring(0, applicationIdentifier.LastIndexOf("."));
            bs.appleDeveloperTeamID = PlayerSettings.iOS.appleDeveloperTeamID;
            bs.automaticallySignBuild = PlayerSettings.iOS.appleEnableAutomaticSigning;
            bs.iOSManualProvisioningProfileUUID = PlayerSettings.iOS.iOSManualProvisioningProfileID;
            bs.tvOSManualProvisioningProfileUUID = PlayerSettings.iOS.tvOSManualProvisioningProfileID;
            bs.cloudProjectId = PlayerSettings.cloudProjectId;
            if (args.target == BuildTarget.iOS)
            {
                bs.buildNumber = PlayerSettings.iOS.buildNumber;
            }
            else if (args.target == BuildTarget.tvOS)
            {
                bs.buildNumber = PlayerSettings.tvOS.buildNumber;
            }
            bs.nativeCrashReportingEnabled = CrashReportingSettings.enabled;
            string configurationURL = UnityConnect.instance.GetConfigurationURL(CloudConfigUrl.CloudPerfEvents);
            if (!string.IsNullOrEmpty(configurationURL))
            {
                bs.symbolsServiceBaseUri = new Uri(configurationURL);
            }
            bs.usymUploadAuthToken = Environment.GetEnvironmentVariable("USYM_UPLOAD_AUTH_TOKEN");
            bs.unityConnectAccessToken = UnityConnect.instance.GetAccessToken();
            return bs;
        }

        private static void SetupFeatureDefines(PBXProject project, string target, BuildSettings buildSettings)
        {
            List<string> list = new List<string>();
            List<string> list2 = new List<string>();
            if (buildSettings.useOnDemandResources)
            {
                list.Add("-DENABLE_IOS_ON_DEMAND_RESOURCES");
            }
            else
            {
                list2.Add("-DENABLE_IOS_ON_DEMAND_RESOURCES");
            }
            project.UpdateBuildProperty(target, "OTHER_CFLAGS", list.ToArray(), list2.ToArray());
        }

        private static ProjectPaths SetupProjectPaths(BuildPostProcessArgs args, bool installInBuildFolder)
        {
            ProjectPaths paths = new ProjectPaths {
                buildToolsDir = UnityEditor.BuildPipeline.GetBuildToolsDirectory(args.target),
                installPath = args.installPath,
                playerPackage = args.playerPackage,
                stagingArea = args.stagingArea,
                stagingAreaData = args.stagingAreaData,
                stagingAreaDataManaged = args.stagingAreaDataManaged
            };
            if (installInBuildFolder)
            {
                paths.installPath = Path.Combine(paths.playerPackage, "Workspace-XCodeDevProject");
            }
            return paths;
        }

        internal static bool ShouldAddPluginToProject(string path) => 
            Utils.IsPathExtensionOneOf(path, kiOSPluginFileExtensions);

        internal static bool ShouldCopyPluginFile(string path) => 
            (Path.GetExtension(path) != ".dll");

        private static bool ShouldStripByteCodeInManagedDlls(BuildSettings bs) => 
            (((!bs.UseIl2Cpp() && (PlayerSettings.strippingLevel >= StrippingLevel.StripByteCode)) && (bs.sdkType == SdkType.Device)) && (!bs.allowDebugging || !bs.isDevelopmentPlayer));

        private static bool ShouldUseRelativeSymlinkForFile(string srcPath, string dstPath)
        {
            string relativePath = Utils.GetRelativePath("", srcPath);
            string str2 = Utils.GetRelativePath("", dstPath);
            if (relativePath.StartsWith("../"))
            {
                return false;
            }
            if (str2.StartsWith("../../"))
            {
                return false;
            }
            return true;
        }

        internal static bool ShouldUseXcodeReferenceForPlugin(string path) => 
            Utils.IsPathExtensionOneOf(path, kiOSEditablePluginFileExtensions);

        private static void StripByteCodeInManagedDlls(BuildTarget target, ProjectPaths paths, AssemblyReferenceChecker checker)
        {
            EditorUtility.DisplayProgressBar("Stripping bytecode from assemblies", "Stripping bytecode from assemblies", 0.95f);
            MonoAssemblyStripping.MonoCilStrip(target, paths.stagingAreaDataManaged, checker.GetAssemblyFileNames());
            Console.Write("\nIncluded DLLs (after bytecode stripping) {0} mb\n\n", GetTotalAssembliesSizeMB(paths.stagingAreaDataManaged, checker.GetAssemblyFileNames()).ToString("0.0"));
        }

        private static void SymlinkTrampoline(string source, string destination)
        {
            <SymlinkTrampoline>c__AnonStorey2 storey = new <SymlinkTrampoline>c__AnonStorey2();
            if (Utils.PlatformSupportsSymlinks())
            {
                source = Path.Combine(source, "Classes");
                destination = Path.Combine(destination, "Classes");
                Regex[] ignoreList = new Regex[] { new Regex("Native"), new Regex("CrashReporter.h"), new Regex("InternalProfiler.h"), new Regex("Preprocessor.h") };
                storey.filesToSymlink = new List<string>();
                Utils.EnumerateCallback fileCallback = new Utils.EnumerateCallback(storey.<>m__0);
                Utils.EnumerateFilesRecursivelyWithIgnoreList(source, fileCallback, null, ignoreList);
                foreach (string str in storey.filesToSymlink)
                {
                    string path = Path.Combine(destination, Utils.GetRelativePath(source, str));
                    System.IO.File.Delete(path);
                    Utils.SymlinkFileAbsolute(str, path);
                }
            }
        }

        private static void UpdateCppDefines(string installPath, BuildSettings bs, UsedFeatures usedFeatures)
        {
            Dictionary<string, bool> valuesToUpdate = new Dictionary<string, bool> {
                { 
                    "ENABLE_IOS_CRASH_REPORTING",
                    PlayerSettings.actionOnDotNetUnhandledException == ActionOnDotNetUnhandledException.Crash
                },
                { 
                    "ENABLE_OBJC_UNCAUGHT_EXCEPTION_HANDLER",
                    PlayerSettings.logObjCUncaughtExceptions
                },
                { 
                    "ENABLE_CUSTOM_CRASH_REPORTER",
                    PlayerSettings.enableCrashReportAPI || bs.nativeCrashReportingEnabled
                },
                { 
                    "ENABLE_CRASH_REPORT_SUBMISSION",
                    bs.nativeCrashReportingEnabled
                }
            };
            UpdateDefinesInFile(installPath + "/Classes/CrashReporter.h", valuesToUpdate);
            valuesToUpdate = new Dictionary<string, bool> {
                { 
                    "ENABLE_INTERNAL_PROFILER",
                    PlayerSettings.enableInternalProfiler
                }
            };
            UpdateDefinesInFile(installPath + "/Classes/Unity/InternalProfiler.h", valuesToUpdate);
            valuesToUpdate = new Dictionary<string, bool> {
                { 
                    "PLATFORM_TVOS",
                    bs.IsAppleTVEnabled()
                },
                { 
                    "PLATFORM_IOS",
                    !bs.IsAppleTVEnabled()
                },
                { 
                    "UNITY_USES_REMOTE_NOTIFICATIONS",
                    usedFeatures.remoteNotifications
                },
                { 
                    "UNITY_USES_WEBCAM",
                    usedFeatures.camera
                },
                { 
                    "UNITY_USES_REPLAY_KIT",
                    usedFeatures.replayKit
                },
                { 
                    "UNITY_DEVELOPER_BUILD",
                    bs.isDevelopmentPlayer
                }
            };
            UpdateDefinesInFile(installPath + "/Classes/Preprocessor.h", valuesToUpdate);
        }

        private static void UpdateDefinesInFile(string file, Dictionary<string, bool> valuesToUpdate)
        {
            string[] second = System.IO.File.ReadAllLines(file);
            string[] lines = (string[]) second.Clone();
            foreach (KeyValuePair<string, bool> pair in valuesToUpdate)
            {
                ReplaceCppMacro(lines, pair.Key, !pair.Value ? "0" : "1");
            }
            if (!lines.SequenceEqual<string>(second))
            {
                System.IO.File.WriteAllLines(file, lines);
            }
        }

        private static void UpdateInfoPlist(string filename, BuildSettings bs, string iPhoneLaunchStoryboardName, string iPadLaunchStoryboardName)
        {
            string text = System.IO.File.ReadAllText(filename);
            string applicationDisplayName = PlayerSettings.iOS.applicationDisplayName;
            PlistUpdater.CustomData data = new PlistUpdater.CustomData {
                bundleIdentifier = bs.companyName + ".${PRODUCT_NAME}",
                bundleVersion = PlayerSettings.bundleVersion,
                buildNumber = bs.buildNumber,
                bundleDisplayName = !string.IsNullOrEmpty(applicationDisplayName) ? applicationDisplayName : "${PRODUCT_NAME}",
                availableIconsList = GetAvailableIconsList(bs),
                availableOrientationSet = GetAvailableOrientations(),
                iPhoneLaunchStoryboardName = iPhoneLaunchStoryboardName,
                iPadLaunchStoryboardName = iPadLaunchStoryboardName,
                isIconPrerendered = PlayerSettings.iOS.prerenderedIcon,
                isPersistenWifiRequired = PlayerSettings.iOS.requiresPersistentWiFi,
                requiresFullScreen = PlayerSettings.iOS.requiresFullScreen,
                isStatusBarHidden = PlayerSettings.statusBarHidden,
                statusBarStyle = GetStatusBarStyle(PlayerSettings.iOS.statusBarStyle),
                exitOnSuspend = PlayerSettings.iOS.appInBackgroundBehavior == iOSAppInBackgroundBehavior.Exit,
                backgroundModes = (PlayerSettings.iOS.appInBackgroundBehavior != iOSAppInBackgroundBehavior.Custom) ? iOSBackgroundMode.None : PlayerSettings.iOS.backgroundModes,
                loadingActivityIndicatorStyle = (int) PlayerSettings.iOS.showActivityIndicatorOnLoading,
                cameraUsageDescription = PlayerSettings.iOS.cameraUsageDescription,
                locationUsageDescription = PlayerSettings.iOS.locationUsageDescription,
                microphoneUsageDescription = PlayerSettings.iOS.microphoneUsageDescription,
                allowHTTP = PlayerSettings.iOS.allowHTTPDownload,
                supportedURLSchemes = PlayerSettings.iOS.GetURLSchemes().ToList<string>(),
                tvOSRequireExtendedGameController = PlayerSettings.tvOS.requireExtendedGameController,
                isAppleTV = bs.IsAppleTVEnabled(),
                cloudProjectId = bs.cloudProjectId,
                crashSubmissionUrl = GetCrashSubmissionUrl(bs.symbolsServiceBaseUri)
            };
            if (!PlayerSettings.GetUseDefaultGraphicsAPIs(bs.target))
            {
                GraphicsDeviceType[] graphicsAPIs = PlayerSettings.GetGraphicsAPIs(bs.target);
                data.requireES3 = graphicsAPIs.Contains<GraphicsDeviceType>(GraphicsDeviceType.OpenGLES3) && !graphicsAPIs.Contains<GraphicsDeviceType>(GraphicsDeviceType.OpenGLES2);
                data.requireMetal = (graphicsAPIs.Length == 1) && (graphicsAPIs[0] == GraphicsDeviceType.Metal);
            }
            try
            {
                text = PlistUpdater.UpdateString(text, data);
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogException(exception);
                return;
            }
            System.IO.File.WriteAllText(filename, text);
        }

        private static void UpdateInstallLocation(ProjectPaths paths, BuildSettings bs, IncludedFileList includedFiles)
        {
            if (bs.autoRunPlayer)
            {
                EditorUtility.DisplayProgressBar("Closing open Unity projects in Xcode", "Closing open Unity projects in Xcode", 0.97f);
                string projectPath = Path.Combine(paths.installPath, "Unity-iPhone.xcodeproj");
                XcodeController.CloseAllOpenUnityProjects(paths.buildToolsDir, projectPath);
            }
            Func<string, string, bool> fileComparer = GetFileComparer(bs.targetGroup);
            if (bs.UseIl2Cpp())
            {
                FileMirroring.MirrorFolder(paths.playerPackage + "/il2cpp/libil2cpp", paths.LibrariesStaging() + "/libil2cpp", fileComparer);
            }
            else
            {
                FileUtil.DeleteFileOrDirectory(paths.LibrariesStaging() + "/libil2cpp");
            }
            if (!bs.appendMode)
            {
                if (!Directory.Exists(paths.installPath))
                {
                    Directory.CreateDirectory(paths.installPath);
                }
                Utils.ReplaceFileOrDirectoryCopy(paths.StagingTrampoline(), paths.installPath);
                if (bs.UseIl2Cpp())
                {
                    Utils.ReplaceFileOrDirectoryCopy(paths.InternalIl2cppOutputPath(), paths.installPath + "/Classes/Native");
                }
            }
            else
            {
                FileMirroring.MirrorFile(paths.PBXProjectStaging(), paths.PBXProjectInstall(), fileComparer);
                FileMirroring.MirrorFolder(paths.DataStaging(), paths.DataInstall(), fileComparer);
                FileMirroring.MirrorFolder(paths.LibrariesStaging(), paths.LibrariesInstall(), fileComparer);
                if (bs.UseIl2Cpp())
                {
                    FileMirroring.MirrorFolder(paths.InternalIl2cppOutputPath(), paths.installPath + "/Classes/Native", fileComparer);
                }
                FileMirroring.MirrorFolder(paths.UnityAssetCatalogStaging(), paths.UnityAssetCatalogInstall(), fileComparer);
                FileMirroring.MirrorFolder(paths.TempStaging(), paths.TempInstall(), fileComparer);
                FileMirroring.MirrorFile(paths.PlistStaging(), paths.PlistInstall(), fileComparer);
                FileMirroring.MirrorFile(paths.LaunchScreeniPhoneXibStaging(), paths.LaunchScreeniPhoneXibInstall(), fileComparer);
                FileMirroring.MirrorFile(paths.LaunchScreeniPhonePortraitStaging(), paths.LaunchScreeniPhonePortraitInstall(), fileComparer);
                FileMirroring.MirrorFile(paths.LaunchScreeniPhoneLandscapeStaging(), paths.LaunchScreeniPhoneLandscapeInstall(), fileComparer);
                FileMirroring.MirrorFile(paths.LaunchScreeniPadStaging(), paths.LaunchScreeniPadInstall(), fileComparer);
                FileMirroring.MirrorFile(paths.LaunchScreeniPadXibStaging(), paths.LaunchScreeniPadXibInstall(), fileComparer);
            }
            if (bs.UseIl2Cpp())
            {
                FileUtil.DeleteFileOrDirectory(paths.installPath + "/Classes/Native/Data");
            }
            if (bs.installInBuildFolder)
            {
                Utils.ReplaceFileOrDirectoryMove(paths.DevProjectStagingTrampoline(), paths.DevProjectInstallPath());
            }
            InstallIncludedFiles(includedFiles, paths.installPath, bs);
            if (!bs.isDevelopmentPlayer)
            {
                WarnAboutMissingSplashscreens(paths, bs);
            }
            InstallIcons(paths, bs);
            InstallSplashScreens(paths, bs);
        }

        private static void UpdateXcScheme(ProjectPaths paths, iOSBuildType buildType)
        {
            string path = paths.XcSchemeStaging();
            if (System.IO.File.Exists(path))
            {
                string contents = XcSchemeUpdater.UpdateString(System.IO.File.ReadAllText(path), buildType);
                System.IO.File.WriteAllText(path, contents);
            }
        }

        private static void VerifyBuildSettings(BuildSettings bs)
        {
            if (bs.IsAppleTVEnabled() && !bs.UseIl2Cpp())
            {
                throw new Exception("Unsupported scripting backend for tvOS; only IL2CPP is supported. Please check your player settings");
            }
        }

        private static void WarnAboutMissingSplashscreens(ProjectPaths paths, BuildSettings bs)
        {
            AvailableOrientations availableOrientations = GetAvailableOrientations();
            List<string> list = new List<string>();
            foreach (UnityEditor.iOS.SplashScreen screen in UnityEditor.iOS.SplashScreen.iOSTypes)
            {
                if (bs.IsDeviceTypeEnabled(screen.deviceType))
                {
                    bool matchOrientation = screen.matchOrientation;
                    bool flag2 = (screen.isPortrait && (availableOrientations.portrait || availableOrientations.portraitUpsideDown)) || (!screen.isPortrait && (availableOrientations.landscapeLeft || availableOrientations.landscapeRight));
                    if ((matchOrientation || flag2) && !System.IO.File.Exists(Path.Combine(paths.SplashScreenOutputFolder(), screen.xcodeFile)))
                    {
                        list.Add(screen.localizedName);
                    }
                }
            }
            if (list.Count > 0)
            {
                UnityEngine.Debug.LogWarning("Splash screen images not provided: " + string.Join(", ", list.ToArray()));
            }
        }

        private static void WriteFeaturesRegistrationFile(string path, UsedFeatures usedFeatures)
        {
            using (TextWriter writer = new StreamWriter(path))
            {
                writer.WriteLine("");
                writer.WriteLine("#include \"RegisterFeatures.h\"");
                writer.WriteLine("");
                writer.WriteLine("extern bool gEnableGyroscope;");
                writer.WriteLine("extern bool gEnableStylusTouch;");
                writer.WriteLine("");
                writer.WriteLine("void RegisterFeatures()");
                writer.WriteLine("{");
                writer.WriteLine("    gEnableGyroscope = {0};", !usedFeatures.gyroscope ? "false" : "true");
                writer.WriteLine("    gEnableStylusTouch = {0};", !usedFeatures.stylus ? "false" : "true");
                writer.WriteLine("}");
                writer.WriteLine("");
                writer.Close();
            }
        }

        private static void WriteIl2CppOptionsFile(string path, bool il2CppUseExceptions)
        {
            using (TextWriter writer = new StreamWriter(path))
            {
                writer.WriteLine("bool il2cpp_no_exceptions = {0};", !il2CppUseExceptions ? "false" : "true");
            }
        }

        private static IXcodeController XcodeController =>
            XcodeControllerFactory.CreateXcodeController();

        [CompilerGenerated]
        private sealed class <AddPluginFiles>c__AnonStorey3
        {
            internal PostProcessiPhonePlayer.<AddPluginFiles>c__AnonStorey4 <>f__ref$4;
            internal string compileFlags;

            internal void <>m__0(string path)
            {
                if (PostProcessiPhonePlayer.ShouldCopyPluginFile(path))
                {
                    PostProcessiPhonePlayer.IncludedFile item = new PostProcessiPhonePlayer.IncludedFile {
                        sourcePath = path,
                        destinationPath = PostProcessiPhonePlayer.GetPluginDestinationPath("Libraries", path),
                        shouldAddToProject = PostProcessiPhonePlayer.ShouldAddPluginToProject(path),
                        symlinkType = !PostProcessiPhonePlayer.ShouldUseXcodeReferenceForPlugin(path) ? PostProcessiPhonePlayer.SymlinkType.Symlink : PostProcessiPhonePlayer.SymlinkType.XcodeReference,
                        compileFlags = this.compileFlags
                    };
                    this.<>f__ref$4.includedFiles.Add(item);
                }
            }

            internal bool <>m__1(string path)
            {
                if (PostProcessiPhonePlayer.IsiOSBundlePlugin(path))
                {
                    this.<>f__ref$4.includedFiles.Add(path, PostProcessiPhonePlayer.GetPluginDestinationPath("Frameworks", path));
                    return false;
                }
                return true;
            }
        }

        [CompilerGenerated]
        private sealed class <AddPluginFiles>c__AnonStorey4
        {
            internal PostProcessiPhonePlayer.IncludedFileList includedFiles;
        }

        [CompilerGenerated]
        private sealed class <CrossCompileManagedDlls>c__AnonStorey0
        {
            internal PostProcessiPhonePlayer.ProjectPaths paths;
            internal RuntimeClassRegistry usedClassRegistry;
        }

        [CompilerGenerated]
        private sealed class <CrossCompileManagedDlls>c__AnonStorey1
        {
            internal PostProcessiPhonePlayer.<CrossCompileManagedDlls>c__AnonStorey0 <>f__ref$0;
            internal iOSIl2CppPlatformProvider platformProvider;

            internal void <>m__0(string outputDir)
            {
                PostProcessiPhonePlayer.ModifyIl2CppOutputDirBeforeCompile(this.<>f__ref$0.paths, outputDir, this.<>f__ref$0.usedClassRegistry, Path.Combine(this.<>f__ref$0.paths.stagingAreaData, "Managed/UnityEngine.dll"), this.platformProvider);
            }
        }

        [CompilerGenerated]
        private sealed class <SymlinkTrampoline>c__AnonStorey2
        {
            internal List<string> filesToSymlink;

            internal void <>m__0(string path, string curLevel)
            {
                this.filesToSymlink.Add(path);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BuildSettings
        {
            public bool useOnDemandResources;
            public BuildTarget target;
            public BuildTargetGroup targetGroup;
            public ScriptingImplementation backend;
            public bool symlinkTrampoline;
            public iOSBuildType buildType;
            public SdkType sdkType;
            public Version targetOsVersion;
            public string productName;
            public string companyName;
            public string buildNumber;
            public iOSTargetDevice targetDevice;
            public string appleDeveloperTeamID;
            public bool automaticallySignBuild;
            public string iOSManualProvisioningProfileUUID;
            public string tvOSManualProvisioningProfileUUID;
            public string cloudProjectId;
            public bool nativeCrashReportingEnabled;
            public Uri symbolsServiceBaseUri;
            public string usymUploadAuthToken;
            public string unityConnectAccessToken;
            public BuildReport buildReport;
            public bool appendMode;
            public bool isDevelopmentPlayer;
            public bool symlinkLibraries;
            public bool installInBuildFolder;
            public bool allowDebugging;
            public bool autoRunPlayer;
            public bool UseIl2Cpp() => 
                (this.backend == ScriptingImplementation.IL2CPP);

            public bool IsIPadEnabled() => 
                ((this.target == BuildTarget.iOS) && ((this.targetDevice == iOSTargetDevice.iPadOnly) || (this.targetDevice == iOSTargetDevice.iPhoneAndiPad)));

            public bool IsIPhoneEnabled() => 
                ((this.target == BuildTarget.iOS) && ((this.targetDevice == iOSTargetDevice.iPhoneOnly) || (this.targetDevice == iOSTargetDevice.iPhoneAndiPad)));

            public bool IsAppleTVEnabled() => 
                (this.target == BuildTarget.tvOS);

            public bool IsDeviceTypeEnabled(UnityEditor.iOS.DeviceType type)
            {
                if (type == UnityEditor.iOS.DeviceType.iPhone)
                {
                    return this.IsIPhoneEnabled();
                }
                if (type == UnityEditor.iOS.DeviceType.iPad)
                {
                    return this.IsIPadEnabled();
                }
                return ((type == UnityEditor.iOS.DeviceType.AppleTV) && this.IsAppleTVEnabled());
            }
        }

        internal class IncludedFile
        {
            public string compileFlags = null;
            public string destinationPath = null;
            public string projectPath = null;
            public bool shouldAddToProject = true;
            public string sourcePath = null;
            public PostProcessiPhonePlayer.SymlinkType symlinkType = PostProcessiPhonePlayer.SymlinkType.Symlink;
        }

        private class IncludedFileList : List<PostProcessiPhonePlayer.IncludedFile>
        {
            [CompilerGenerated]
            private static Func<PostProcessiPhonePlayer.IncludedFile, bool> <>f__am$cache0;
            [CompilerGenerated]
            private static Func<PostProcessiPhonePlayer.IncludedFile, bool> <>f__am$cache1;

            public void Add(string srcPath, string dstPath)
            {
                PostProcessiPhonePlayer.IncludedFile item = new PostProcessiPhonePlayer.IncludedFile {
                    sourcePath = srcPath,
                    destinationPath = dstPath
                };
                base.Add(item);
            }

            public HashSet<string> GetProjectFilesByFullPath(HashSet<string> fullPaths)
            {
                HashSet<string> set = new HashSet<string>();
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = x => x.shouldAddToProject;
                }
                List<PostProcessiPhonePlayer.IncludedFile> list = Enumerable.Where<PostProcessiPhonePlayer.IncludedFile>(this, <>f__am$cache1).ToList<PostProcessiPhonePlayer.IncludedFile>();
                foreach (PostProcessiPhonePlayer.IncludedFile file in list)
                {
                    if (fullPaths.Contains(file.destinationPath))
                    {
                        set.Add(file.destinationPath);
                    }
                }
                return set;
            }

            public HashSet<string> GetProjectFilesInPath(string projectPath)
            {
                HashSet<string> set = new HashSet<string>();
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = x => x.shouldAddToProject;
                }
                List<PostProcessiPhonePlayer.IncludedFile> list = Enumerable.Where<PostProcessiPhonePlayer.IncludedFile>(this, <>f__am$cache0).ToList<PostProcessiPhonePlayer.IncludedFile>();
                foreach (PostProcessiPhonePlayer.IncludedFile file in list)
                {
                    if (file.destinationPath.Contains(PBXPath.FixSlashes(projectPath)))
                    {
                        set.Add(file.destinationPath);
                    }
                }
                return set;
            }
        }

        private class ProjectPaths
        {
            public string buildToolsDir;
            public string installPath;
            private string m_InternalIl2cppOutputStaging = null;
            public string playerPackage;
            public string stagingArea;
            public string stagingAreaData;
            public string stagingAreaDataManaged;

            public string DataInstall() => 
                Path.Combine(this.installPath, "Data");

            public string DataStaging() => 
                Path.Combine(this.StagingTrampoline(), "Data");

            public string DevProjectEditorTrampoline() => 
                Path.Combine(this.playerPackage, "Trampoline-UnityDevProject");

            public string DevProjectFileList() => 
                Path.Combine(this.playerPackage, "xcode_file_list.txt");

            public string DevProjectInstallPath() => 
                Path.Combine(this.playerPackage, "Workspace-UnityDevProject");

            public string DevProjectLibList() => 
                Path.Combine(this.playerPackage, "xcode_lib_list.txt");

            public string DevProjectPBXProjectStaging() => 
                Path.Combine(this.DevProjectStagingTrampoline(), "UnityDevProject.xcodeproj/project.pbxproj");

            public string DevProjectStagingTrampoline() => 
                Path.Combine(this.stagingArea, "Trampoline-UnityDevProject");

            public string EditorProTrampoline() => 
                Path.Combine(this.playerPackage, "Trampoline-ProExtras");

            public string EditorTrampoline() => 
                Path.Combine(this.playerPackage, "Trampoline");

            public string FeatureRegistrationStaging() => 
                Path.Combine(this.LibrariesStaging(), "RegisterFeatures.cpp");

            public string IconOutputFolder() => 
                this.stagingArea;

            public string IconPathInstall() => 
                Path.Combine(this.ImagesAssetCatalogInstall(), "AppIcon.appiconset");

            public string IconSourceFolder() => 
                Path.Combine(this.playerPackage, "Icons");

            public string ImagesAssetCatalogInstall() => 
                Path.Combine(this.installPath, "Unity-iPhone/Images.xcassets");

            public string InternalIl2cppOutputPath() => 
                this.m_InternalIl2cppOutputStaging;

            public string LaunchScreenFallbackSource() => 
                Path.Combine(this.SplashScreenSourceFolder(), "LaunchScreenImage-default.png");

            public string LaunchScreeniPadInstall() => 
                Path.Combine(this.installPath, "LaunchScreen-iPad.png");

            public string LaunchScreeniPadOutput() => 
                Path.Combine(this.SplashScreenOutputFolder(), "LaunchScreen-iPad.png");

            public string LaunchScreeniPadStaging() => 
                Path.Combine(this.StagingTrampoline(), "LaunchScreen-iPad.png");

            public string LaunchScreeniPadXibConstantSizeSource() => 
                Path.Combine(this.SplashScreenSourceFolder(), "LaunchScreeniPadConstantSize.xib");

            public string LaunchScreeniPadXibDefaultSource() => 
                Path.Combine(this.SplashScreenSourceFolder(), "LaunchScreeniPadDefault.xib");

            public string LaunchScreeniPadXibInstall() => 
                Path.Combine(this.installPath, "LaunchScreen-iPad.xib");

            public string LaunchScreeniPadXibRelativeSizeSource() => 
                Path.Combine(this.SplashScreenSourceFolder(), "LaunchScreeniPadRelativeSize.xib");

            public string LaunchScreeniPadXibStaging() => 
                Path.Combine(this.StagingTrampoline(), "LaunchScreen-iPad.xib");

            public string LaunchScreeniPhoneLandscapeInstall() => 
                Path.Combine(this.installPath, "LaunchScreen-iPhoneLandscape.png");

            public string LaunchScreeniPhoneLandscapeOutput() => 
                Path.Combine(this.SplashScreenOutputFolder(), "LaunchScreen-iPhoneLandscape.png");

            public string LaunchScreeniPhoneLandscapeStaging() => 
                Path.Combine(this.StagingTrampoline(), "LaunchScreen-iPhoneLandscape.png");

            public string LaunchScreeniPhonePortraitInstall() => 
                Path.Combine(this.installPath, "LaunchScreen-iPhonePortrait.png");

            public string LaunchScreeniPhonePortraitOutput() => 
                Path.Combine(this.SplashScreenOutputFolder(), "LaunchScreen-iPhonePortrait.png");

            public string LaunchScreeniPhonePortraitStaging() => 
                Path.Combine(this.StagingTrampoline(), "LaunchScreen-iPhonePortrait.png");

            public string LaunchScreeniPhoneXibConstantSizeSource() => 
                Path.Combine(this.SplashScreenSourceFolder(), "LaunchScreeniPhoneConstantSize.xib");

            public string LaunchScreeniPhoneXibDefaultSource() => 
                Path.Combine(this.SplashScreenSourceFolder(), "LaunchScreeniPhoneDefault.xib");

            public string LaunchScreeniPhoneXibInstall() => 
                Path.Combine(this.installPath, "LaunchScreen-iPhone.xib");

            public string LaunchScreeniPhoneXibRelativeSizeSource() => 
                Path.Combine(this.SplashScreenSourceFolder(), "LaunchScreeniPhoneRelativeSize.xib");

            public string LaunchScreeniPhoneXibStaging() => 
                Path.Combine(this.StagingTrampoline(), "LaunchScreen-iPhone.xib");

            public string LaunchScreenTransparentSource() => 
                Path.Combine(this.SplashScreenSourceFolder(), "LaunchScreenImage-transparent.png");

            public string LibrariesInstall() => 
                Path.Combine(this.installPath, "Libraries");

            public string LibrariesStaging() => 
                Path.Combine(this.StagingTrampoline(), "Libraries");

            public string PBXProjectInstall() => 
                Path.Combine(this.installPath, "Unity-iPhone.xcodeproj/project.pbxproj");

            public string PBXProjectStaging() => 
                Path.Combine(this.StagingTrampoline(), "Unity-iPhone.xcodeproj/project.pbxproj");

            public string PlistInstall() => 
                Path.Combine(this.installPath, "Info.plist");

            public string PlistStaging() => 
                Path.Combine(this.StagingTrampoline(), "Info.plist");

            public void SetInternalIl2cppOutputPath(string path)
            {
                this.m_InternalIl2cppOutputStaging = path;
            }

            public string SplashPathInstall() => 
                Path.Combine(this.ImagesAssetCatalogInstall(), "LaunchImage.launchimage");

            public string SplashScreenOutputFolder() => 
                this.stagingArea;

            public string SplashScreenSharedOutput() => 
                Path.Combine(this.SplashScreenOutputFolder(), "Shared.png");

            public string SplashScreenSourceFolder() => 
                Path.Combine(this.playerPackage, "SplashScreens");

            public string StagingTrampoline() => 
                Path.Combine(this.stagingArea, "Trampoline");

            public string TempInstall() => 
                Path.Combine(this.installPath, "Temp");

            public string TempStaging() => 
                Path.Combine(this.StagingTrampoline(), "Temp");

            public string tvOSLargeIconOutputBase() => 
                Path.Combine(this.IconOutputFolder(), "TVOS-LargeIconLayer-{0}.png");

            public string tvOSSmallIconOutputBase() => 
                Path.Combine(this.IconOutputFolder(), "TVOS-SmallIconLayer-{0}.png");

            public string tvOSTopShelfImageOutputBase() => 
                Path.Combine(this.IconOutputFolder(), "TVOS-TopShelfImageLayer-{0}.png");

            public string tvOSTopShelfImageWideOutputBase() => 
                Path.Combine(this.IconOutputFolder(), "TVOS-TopShelfImageWideLayer-{0}.png");

            public string UnityAssetCatalogInstall() => 
                Path.Combine(this.installPath, "UnityData.xcassets");

            public string UnityAssetCatalogStaging() => 
                Path.Combine(this.StagingTrampoline(), "UnityData.xcassets");

            public string UnityImagesAssetCatalogInstall() => 
                Path.Combine(this.installPath, "UnityImages.xcassets");

            public string UnityTree() => 
                FileUtil.DeleteLastPathNameComponent(FileUtil.DeleteLastPathNameComponent(this.playerPackage));

            public string XcSchemeStaging() => 
                Path.Combine(this.StagingTrampoline(), "Unity-iPhone.xcodeproj/xcshareddata/xcschemes/Unity-iPhone.xcscheme");
        }

        internal enum SymlinkType
        {
            None,
            Symlink,
            XcodeReference
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct UsedFeatures
        {
            public bool camera;
            public bool controller;
            public bool gameCenter;
            public bool gyroscope;
            public bool location;
            public bool microphone;
            public bool remoteNotifications;
            public bool replayKit;
            public bool stylus;
        }
    }
}

