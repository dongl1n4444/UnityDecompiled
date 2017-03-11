namespace UnityEditor.OSXStandalone
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEditor.Utils;
    using UnityEditorInternal;
    using UnityEngine;

    internal class OSXDesktopStandalonePostProcessor : DesktopStandalonePostProcessor, IBuildPostprocessor
    {
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache3;

        protected override void CopyDataForBuildsFolder()
        {
        }

        protected override void DeleteDestination()
        {
            FileUtil.DeleteFileOrDirectory(this.m_PostProcessArgs.installPath);
        }

        public string GetExtension(BuildTarget target, BuildOptions options) => 
            "app";

        protected override IIl2CppPlatformProvider GetPlatformProvider(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.StandaloneOSXUniversal:
                case BuildTarget.StandaloneOSXIntel:
                    break;

                default:
                    if (target != BuildTarget.StandaloneOSXIntel64)
                    {
                        throw new Exception("Build target not supported.");
                    }
                    break;
            }
            return new OSXStandaloneIl2CppPlatformProvider(target, this.StagingAreaContents + "/Data", base.Development);
        }

        public string GetScriptLayoutFileFromBuild(BuildOptions options, string installPath, string fileName) => 
            string.Empty;

        protected override string GetVariationName()
        {
            string str = "mono";
            if (base.UseIl2Cpp)
            {
                str = "il2cpp";
            }
            return $"{base.GetVariationName()}_{str}";
        }

        public void LaunchPlayer(BuildLaunchPlayerArgs args)
        {
        }

        protected override string PlatformStringFor(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.StandaloneOSXUniversal:
                    return "universal";

                case BuildTarget.StandaloneOSXIntel:
                    return "macosx32";
            }
            if (target != BuildTarget.StandaloneOSXIntel64)
            {
                throw new ArgumentException("Unexpected target: " + target);
            }
            return "macosx64";
        }

        public void PostProcess(BuildPostProcessArgs args)
        {
            base.m_PostProcessArgs = args;
            AssemblyReferenceChecker checker = new AssemblyReferenceChecker();
            checker.CollectReferences(args.stagingAreaDataManaged, false, 0f, true);
            bool flag = !string.IsNullOrEmpty(checker.WhoReferencesClass("UnityEngine.SocialPlatforms.GameCenter.GameCenterPlatform", true));
            this.SaveEditorOnlyPlayerSettingsToPlist();
            base.PostProcess();
            if (flag)
            {
                if (Application.platform != RuntimePlatform.OSXEditor)
                {
                    UnityEngine.Debug.LogWarning("OS X Standalone players with GameCenter support need to be built on an OS X machine in order to pass Mac App Store validation.");
                }
                else
                {
                    Console.WriteLine("Adding GameKit linkage to OS X binary.");
                    ProcessStartInfo info2 = new ProcessStartInfo {
                        FileName = Path.Combine(BuildPipeline.GetPlaybackEngineDirectory(args.target, args.options), "optool")
                    };
                    string[] textArray1 = new string[] { "install -c weak -p /System/Library/Frameworks/GameKit.framework/Versions/A/GameKit -t \"", this.m_PostProcessArgs.installPath, "/Contents/MacOS/", this.InstallNameWithoutExtension, "\"" };
                    info2.Arguments = string.Concat(textArray1);
                    info2.CreateNoWindow = true;
                    ProcessStartInfo si = info2;
                    Program program = new Program(si);
                    program.Start();
                    while (!program.WaitForExit(100))
                    {
                    }
                    if (program.ExitCode != 0)
                    {
                        UnityEngine.Debug.LogError("Running optool to link GameKit failed\n" + si.FileName + " " + si.Arguments + "\n" + program.GetAllOutput());
                    }
                    program.Dispose();
                }
            }
        }

        public void PostProcessScriptsOnly(BuildPostProcessArgs args)
        {
            throw new NotImplementedException();
        }

        public string PrepareForBuild(BuildOptions options, BuildTarget target) => 
            null;

        protected override void RenameFilesInStagingArea()
        {
            File.Move(this.StagingAreaContents + "/MacOS/UnityPlayer", this.StagingAreaContents + "/MacOS/" + this.InstallNameWithoutExtension);
            this.m_PostProcessArgs.report.AddFile(this.StagingAreaContents + "/MacOS/" + this.InstallNameWithoutExtension, "Executable");
            string path = base.StagingArea + "/Data/Managed/Resources";
            if (Directory.Exists(path))
            {
                string dir = this.StagingAreaContents + "/Data/Managed/Resources";
                FileUtil.CreateOrCleanDirectory(dir);
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = f => f.EndsWith("-resources.dat");
                }
                FileUtil.CopyDirectoryFiltered(path, dir, true, <>f__am$cache0, true);
            }
            string str3 = base.StagingArea + "/Data/Managed/Metadata";
            if (Directory.Exists(str3))
            {
                string str4 = this.StagingAreaContents + "/Data/Managed/Metadata";
                FileUtil.CreateOrCleanDirectory(str4);
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = f => f.EndsWith("-metadata.dat");
                }
                FileUtil.CopyDirectoryFiltered(str3, str4, true, <>f__am$cache1, true);
            }
            string source = base.StagingArea + "/Data/Managed";
            string target = this.StagingAreaContents + "/Data/Managed";
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = f => f.EndsWith("SymbolMap");
            }
            FileUtil.CopyDirectoryFiltered(source, target, true, <>f__am$cache2, true);
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = f => true;
            }
            FileUtil.CopyDirectoryFiltered(base.StagingArea + "/Data", this.StagingAreaContents + "/Resources/Data", true, <>f__am$cache3, true);
            FileUtil.DeleteFileOrDirectory(base.StagingArea + "/Data");
            this.m_PostProcessArgs.report.RelocateFiles(base.StagingArea + "/Data", this.StagingAreaContents + "/Resources/Data");
            if (base.UseIl2Cpp)
            {
                FileUtil.DeleteFileOrDirectory(this.StagingAreaContents + "/Resources/Data/Managed");
                FileUtil.DeleteFileOrDirectory(this.StagingAreaContents + "/Resources/Data/il2cppOutput");
            }
            string str7 = FileUtil.NiceWinPath(this.StagingAreaContents + "/Info.plist");
            string[] input = new string[] { "CFBundleShortVersionString", PlayerSettings.bundleVersion, "CFBundleVersion", PlayerSettings.macOS.buildNumber, "CFBundleIdentifier", PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Standalone), "CFBundleExecutable", this.InstallNameWithoutExtension, "CFBundleName", this.m_PostProcessArgs.productName };
            this.UpdateInfoPlist(str7, input);
            FileUtil.MoveFileOrDirectory(base.StagingArea + "/UnityPlayer.app", base.StagingArea + "/" + this.InstallNameWithoutExtension + ".app");
            this.m_PostProcessArgs.report.RelocateFiles(base.StagingArea + "/UnityPlayer.app", base.StagingArea + "/" + this.InstallNameWithoutExtension + ".app");
        }

        private void SaveEditorOnlyPlayerSettingsToPlist()
        {
            FileUtil.CreateOrCleanDirectory(this.StagingAreaContents + "/Resources");
            TextWriter writer = new StreamWriter(this.StagingAreaContents + "/Resources/DefaultPreferences.plist");
            writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            writer.WriteLine("<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">");
            writer.WriteLine("<plist version=\"1.0\">");
            writer.WriteLine("<dict>");
            writer.WriteLine("<key>NSQuitAlwaysKeepsWindows</key>");
            writer.WriteLine("<false/>");
            writer.WriteLine("<key>Screenmanager Is Fullscreen mode</key>");
            writer.WriteLine("<string>" + PlayerSettings.defaultIsFullScreen + "</string>");
            writer.WriteLine("</dict>");
            writer.WriteLine("</plist>");
            writer.Close();
        }

        public bool SupportsInstallInBuildFolder() => 
            true;

        public bool SupportsScriptsOnlyBuild() => 
            false;

        private void UpdateInfoPlist(string path, params string[] input)
        {
            string[] contents = File.ReadAllLines(path);
            for (int i = 0; i < (contents.Length - 1); i++)
            {
                for (int j = 0; j < input.Length; j += 2)
                {
                    if (contents[i].Contains(input[j]))
                    {
                        i++;
                        contents[i] = Regex.Replace(contents[i], "<string>.+</string>", $"<string>{input[j + 1]}</string>");
                    }
                }
            }
            File.WriteAllLines(path, contents);
        }

        protected override string DestinationFolderForInstallingIntoBuildsFolder =>
            ("build/MacStandaloneSupport/Variations/" + this.GetVariationName() + "/UnityPlayer.app/Contents/Resources/Data");

        private string InstallNameWithoutExtension =>
            FileUtil.UnityGetFileNameWithoutExtension(this.m_PostProcessArgs.installPath);

        private string StagingAreaContents =>
            (base.StagingArea + "/UnityPlayer.app/Contents");

        protected override string StagingAreaPluginsFolder =>
            (this.StagingAreaContents + "/Plugins");
    }
}

