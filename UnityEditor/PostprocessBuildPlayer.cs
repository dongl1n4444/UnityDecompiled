namespace UnityEditor
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using UnityEditor.BuildReporting;
    using UnityEditor.Modules;
    using UnityEditor.Utils;
    using UnityEngine;

    internal class PostprocessBuildPlayer
    {
        internal const string StreamingAssets = "Assets/StreamingAssets";

        internal static string ExecuteSystemProcess(string command, string args, string workingdir)
        {
            ProcessStartInfo si = new ProcessStartInfo {
                FileName = command,
                Arguments = args,
                WorkingDirectory = workingdir,
                CreateNoWindow = true
            };
            Program program = new Program(si);
            program.Start();
            while (!program.WaitForExit(100))
            {
            }
            string standardOutputAsString = program.GetStandardOutputAsString();
            program.Dispose();
            return standardOutputAsString;
        }

        internal static string GenerateBundleIdentifier(string companyName, string productName) => 
            ("unity." + companyName + "." + productName);

        internal static string GetArchitectureForTarget(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneOSXIntel:
                    goto Label_0044;

                case BuildTarget.StandaloneWindows64:
                    break;

                default:
                    if (target != BuildTarget.StandaloneLinux64)
                    {
                        if (target != BuildTarget.StandaloneLinuxUniversal)
                        {
                            return string.Empty;
                        }
                        goto Label_0044;
                    }
                    break;
            }
            return "x86_64";
        Label_0044:
            return "x86";
        }

        public static string GetExtensionForBuildTarget(BuildTargetGroup targetGroup, BuildTarget target, BuildOptions options) => 
            ModuleManager.GetBuildPostProcessor(targetGroup, target)?.GetExtension(target, options);

        public static string GetScriptLayoutFileFromBuild(BuildOptions options, BuildTargetGroup targetGroup, BuildTarget target, string installPath, string fileName)
        {
            IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(targetGroup, target);
            if (buildPostProcessor != null)
            {
                return buildPostProcessor.GetScriptLayoutFileFromBuild(options, installPath, fileName);
            }
            return "";
        }

        internal static bool InstallPluginsByExtension(string pluginSourceFolder, string extension, string debugExtension, string destPluginFolder, bool copyDirectories)
        {
            bool flag = false;
            if (Directory.Exists(pluginSourceFolder))
            {
                string[] fileSystemEntries = Directory.GetFileSystemEntries(pluginSourceFolder);
                foreach (string str in fileSystemEntries)
                {
                    string fileName = Path.GetFileName(str);
                    string str3 = Path.GetExtension(str);
                    bool flag3 = str3.Equals(extension, StringComparison.OrdinalIgnoreCase) || fileName.Equals(extension, StringComparison.OrdinalIgnoreCase);
                    bool flag4 = ((debugExtension != null) && (debugExtension.Length != 0)) && (str3.Equals(debugExtension, StringComparison.OrdinalIgnoreCase) || fileName.Equals(debugExtension, StringComparison.OrdinalIgnoreCase));
                    if (flag3 || flag4)
                    {
                        if (!Directory.Exists(destPluginFolder))
                        {
                            Directory.CreateDirectory(destPluginFolder);
                        }
                        string target = Path.Combine(destPluginFolder, fileName);
                        if (copyDirectories)
                        {
                            FileUtil.CopyDirectoryRecursive(str, target);
                        }
                        else if (!Directory.Exists(str))
                        {
                            FileUtil.UnityFileCopy(str, target);
                        }
                        flag = true;
                    }
                }
            }
            return flag;
        }

        internal static void InstallStreamingAssets(string stagingAreaDataPath)
        {
            InstallStreamingAssets(stagingAreaDataPath, null);
        }

        internal static void InstallStreamingAssets(string stagingAreaDataPath, BuildReport report)
        {
            if (Directory.Exists("Assets/StreamingAssets"))
            {
                string target = Path.Combine(stagingAreaDataPath, "StreamingAssets");
                FileUtil.CopyDirectoryRecursiveForPostprocess("Assets/StreamingAssets", target, true);
                if (report != null)
                {
                    report.AddFilesRecursive(target, "Streaming Assets");
                }
            }
        }

        public static void Launch(BuildTargetGroup targetGroup, BuildTarget target, string path, string productName, BuildOptions options)
        {
            BuildLaunchPlayerArgs args;
            IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(targetGroup, target);
            if (buildPostProcessor == null)
            {
                throw new UnityException($"Launching {target} build target via mono is not supported");
            }
            args.target = target;
            args.playerPackage = BuildPipeline.GetPlaybackEngineDirectory(target, options);
            args.installPath = path;
            args.productName = productName;
            args.options = options;
            buildPostProcessor.LaunchPlayer(args);
        }

        public static void Postprocess(BuildTargetGroup targetGroup, BuildTarget target, string installPath, string companyName, string productName, int width, int height, string downloadWebplayerUrl, string manualDownloadWebplayerUrl, BuildOptions options, RuntimeClassRegistry usedClassRegistry, BuildReport report)
        {
            BuildPostProcessArgs args;
            string str = "Temp/StagingArea";
            string str2 = "Temp/StagingArea/Data";
            string str3 = "Temp/StagingArea/Data/Managed";
            string playbackEngineDirectory = BuildPipeline.GetPlaybackEngineDirectory(target, options);
            bool flag = ((options & BuildOptions.InstallInBuildFolder) != BuildOptions.CompressTextures) && SupportsInstallInBuildFolder(targetGroup, target);
            if ((installPath == string.Empty) && !flag)
            {
                throw new Exception(installPath + " must not be an empty string");
            }
            IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(targetGroup, target);
            if (buildPostProcessor == null)
            {
                throw new UnityException($"Build target '{target}' not supported");
            }
            args.target = target;
            args.stagingAreaData = str2;
            args.stagingArea = str;
            args.stagingAreaDataManaged = str3;
            args.playerPackage = playbackEngineDirectory;
            args.installPath = installPath;
            args.companyName = companyName;
            args.productName = productName;
            args.productGUID = PlayerSettings.productGUID;
            args.options = options;
            args.usedClassRegistry = usedClassRegistry;
            args.report = report;
            buildPostProcessor.PostProcess(args);
        }

        public static string PrepareForBuild(BuildOptions options, BuildTargetGroup targetGroup, BuildTarget target) => 
            ModuleManager.GetBuildPostProcessor(targetGroup, target)?.PrepareForBuild(options, target);

        public static bool SupportsInstallInBuildFolder(BuildTargetGroup targetGroup, BuildTarget target)
        {
            IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(targetGroup, target);
            if (buildPostProcessor != null)
            {
                return buildPostProcessor.SupportsInstallInBuildFolder();
            }
            switch (target)
            {
                case BuildTarget.PSP2:
                case BuildTarget.PSM:
                    break;

                default:
                    if ((target != BuildTarget.Android) && (target != BuildTarget.WSAPlayer))
                    {
                        return false;
                    }
                    break;
            }
            return true;
        }

        public static bool SupportsScriptsOnlyBuild(BuildTargetGroup targetGroup, BuildTarget target)
        {
            IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(targetGroup, target);
            return ((buildPostProcessor != null) && buildPostProcessor.SupportsScriptsOnlyBuild());
        }

        public static string subDir32Bit =>
            "x86";

        public static string subDir64Bit =>
            "x86_64";
    }
}

