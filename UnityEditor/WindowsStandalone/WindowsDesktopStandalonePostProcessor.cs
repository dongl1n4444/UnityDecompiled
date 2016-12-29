namespace UnityEditor.WindowsStandalone
{
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEditor.Utils;
    using UnityEditorInternal;

    internal class WindowsDesktopStandalonePostProcessor : DesktopStandalonePostProcessor, IBuildPostprocessor
    {
        protected override void CopyDataForBuildsFolder()
        {
            string[] components = new string[] { Unsupported.GetBaseUnityDeveloperFolder(), "build/WindowsStandaloneSupport/Variations", this.GetVariationName(), "Data" };
            string source = Paths.Combine(components);
            string[] textArray2 = new string[] { Unsupported.GetBaseUnityDeveloperFolder(), this.DestinationFolderForInstallingIntoBuildsFolder };
            string target = Paths.Combine(textArray2);
            FileUtil.CopyDirectoryRecursiveForPostprocess(source, target, true);
        }

        protected override bool CopyFilter(string path)
        {
            bool flag = base.CopyFilter(path);
            if (!UserBuildSettings.copyPDBFiles)
            {
                flag &= Path.GetExtension(path).ToLower() != ".pdb";
            }
            return flag;
        }

        protected override void DeleteDestination()
        {
            bool flag = true;
            if (File.Exists(this.FullPathToExe) || Directory.Exists(this.FullPathToExe))
            {
                flag = FileUtil.DeleteFileOrDirectory(this.FullPathToExe);
            }
            if (flag && (File.Exists(this.FullDataFolderPath) || Directory.Exists(this.FullDataFolderPath)))
            {
                flag = FileUtil.DeleteFileOrDirectory(this.FullDataFolderPath);
            }
            if (flag)
            {
                if (Directory.Exists(this.DestinationFolder))
                {
                    foreach (string str in Directory.GetFiles(this.DestinationFolder, "*.pdb"))
                    {
                        if (!flag)
                        {
                            break;
                        }
                        flag = FileUtil.DeleteFileOrDirectory(str);
                    }
                }
                else if (File.Exists(this.DestinationFolder))
                {
                    flag = FileUtil.DeleteFileOrDirectory(this.DestinationFolder);
                }
            }
            if (!flag)
            {
                throw new OperationCanceledException("Failed to prepare target build directory. Is a built game instance running?");
            }
        }

        public string GetExtension(BuildTarget target, BuildOptions options) => 
            "exe";

        protected override IIl2CppPlatformProvider GetPlatformProvider(BuildTarget target)
        {
            if ((target != BuildTarget.StandaloneWindows) && (target != BuildTarget.StandaloneWindows64))
            {
                throw new Exception("Build target not supported.");
            }
            return new WindowsStandaloneIl2CppPlatformProvider(target, base.DataFolder, base.Development);
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
            if (target != BuildTarget.StandaloneWindows)
            {
                if (target != BuildTarget.StandaloneWindows64)
                {
                    throw new ArgumentException("Unexpected target: " + target);
                }
            }
            else
            {
                return "win32";
            }
            return "win64";
        }

        public void PostProcess(BuildPostProcessArgs args)
        {
            base.m_PostProcessArgs = args;
            base.PostProcess();
        }

        public void PostProcessScriptsOnly(BuildPostProcessArgs args)
        {
            throw new NotImplementedException();
        }

        public string PrepareForBuild(BuildOptions options, BuildTarget target) => 
            null;

        protected override void RenameFilesInStagingArea()
        {
            string fileName = Path.GetFileName(this.m_PostProcessArgs.installPath);
            FileUtil.MoveFileOrDirectory(base.StagingArea + "/player_win.exe", base.StagingArea + "/" + fileName);
            this.m_PostProcessArgs.report.RelocateFiles(base.StagingArea + "/player_win.exe", base.StagingArea + "/" + fileName);
            FileUtil.MoveFileOrDirectory(base.StagingArea + "/Data", base.StagingArea + "/" + Path.GetFileNameWithoutExtension(fileName) + "_Data");
            this.m_PostProcessArgs.report.RelocateFiles(base.StagingArea + "/Data", base.StagingArea + "/" + Path.GetFileNameWithoutExtension(fileName) + "_Data");
        }

        public bool SupportsInstallInBuildFolder() => 
            true;

        public bool SupportsScriptsOnlyBuild() => 
            false;

        private string ToWindowsPath(string path)
        {
            if (((path.Length > 1) && (path[0] == '/')) && (path[1] == '/'))
            {
                path = @"\\" + path.Substring(2);
            }
            return path;
        }

        private string DestinationFileWithoutExtension =>
            this.ToWindowsPath(FileUtil.UnityGetFileNameWithoutExtension(this.FullPathToExe));

        protected override string DestinationFolder =>
            this.ToWindowsPath(base.DestinationFolder);

        protected override string DestinationFolderForInstallingIntoBuildsFolder =>
            ("build/WindowsStandaloneSupport/Variations/" + this.GetVariationName() + "/DataSource");

        private string FullDataFolderPath =>
            Path.Combine(this.DestinationFolder, this.DestinationFileWithoutExtension + "_Data");

        private string FullPathToExe =>
            this.m_PostProcessArgs.installPath;

        protected override string StagingAreaPluginsFolder =>
            (base.StagingArea + "/Data/Plugins");
    }
}

