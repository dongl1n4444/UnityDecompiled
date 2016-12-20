namespace UnityEditor.LinuxStandalone
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEditorInternal;

    internal class LinuxDesktopStandalonePostProcessor : DesktopStandalonePostProcessor, IBuildPostprocessor
    {
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<BuildTarget, string> <>f__mg$cache0;
        private static readonly string kScriptsOnlyError = "Linux standalone doesn't support scripts-only build";

        private static string ArchitecturePostFixFor(BuildTarget buildTarget)
        {
            return ((buildTarget != BuildTarget.StandaloneLinux64) ? "x86" : "x86_64");
        }

        protected override void CopyDataForBuildsFolder()
        {
            throw new NotSupportedException();
        }

        protected override void CopyVariationFolderIntoStagingArea()
        {
            if (base.Target != BuildTarget.StandaloneLinuxUniversal)
            {
                base.CopyVariationFolderIntoStagingArea();
            }
            else
            {
                BuildTarget[] targetArray1 = new BuildTarget[] { BuildTarget.StandaloneLinux, BuildTarget.StandaloneLinux64 };
                foreach (BuildTarget target in targetArray1)
                {
                    string[] paths = new string[] { this.m_PostProcessArgs.playerPackage, "Variations", this.VariationNameFor(target) };
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = new Func<string, bool>(null, (IntPtr) <CopyVariationFolderIntoStagingArea>m__0);
                    }
                    FileUtil.CopyDirectoryFiltered(FileUtil.CombinePaths(paths), base.StagingArea, true, <>f__am$cache0, true);
                    this.RenameStagingAreaFile("LinuxPlayer", Path.ChangeExtension("LinuxPlayer", ArchitecturePostFixFor(target)));
                }
            }
        }

        protected override void DeleteDestination()
        {
            FileUtil.DeleteFileOrDirectory(Path.ChangeExtension(base.InstallPath, ArchitecturePostFixFor(BuildTarget.StandaloneLinux)));
            FileUtil.DeleteFileOrDirectory(Path.ChangeExtension(base.InstallPath, ArchitecturePostFixFor(BuildTarget.StandaloneLinux64)));
            FileUtil.DeleteFileOrDirectory(this.FullDataFolderPath);
        }

        public string GetExtension(BuildTarget target, BuildOptions options)
        {
            return ArchitecturePostFixFor(target);
        }

        protected override IIl2CppPlatformProvider GetPlatformProvider(BuildTarget target)
        {
            return new LinuxStandaloneIl2CppPlatformProvider(target, base.DataFolder, base.Development);
        }

        public string GetScriptLayoutFileFromBuild(BuildOptions options, string installPath, string fileName)
        {
            throw new NotImplementedException(kScriptsOnlyError);
        }

        protected override string GetVariationName()
        {
            return this.VariationNameFor(base.Target);
        }

        public void LaunchPlayer(BuildLaunchPlayerArgs args)
        {
            throw new NotImplementedException();
        }

        protected override string PlatformStringFor(BuildTarget target)
        {
            if (target != BuildTarget.StandaloneLinux64)
            {
                if (target != BuildTarget.StandaloneLinuxUniversal)
                {
                    if (target != BuildTarget.StandaloneLinux)
                    {
                        throw new ArgumentException("Unexpected target: " + target);
                    }
                    return "linux32";
                }
            }
            else
            {
                return "linux64";
            }
            return "universal";
        }

        public void PostProcess(BuildPostProcessArgs args)
        {
            base.m_PostProcessArgs = args;
            base.PostProcess();
        }

        public void PostProcessScriptsOnly(BuildPostProcessArgs args)
        {
            throw new NotImplementedException(kScriptsOnlyError);
        }

        public string PrepareForBuild(BuildOptions options, BuildTarget target)
        {
            return null;
        }

        protected override void RenameFilesInStagingArea()
        {
            this.RenameStagingAreaFile("Data", FileUtil.UnityGetFileNameWithoutExtension(base.InstallPath) + "_Data");
            if (base.UseIl2Cpp)
            {
                string[] paths = new string[] { base.StagingArea, "Data", "Managed" };
                FileUtil.DeleteFileOrDirectory(FileUtil.CombinePaths(paths));
                string[] textArray2 = new string[] { base.StagingArea, "Data", "il2cppOutput" };
                FileUtil.DeleteFileOrDirectory(FileUtil.CombinePaths(textArray2));
            }
            if (base.Target != BuildTarget.StandaloneLinuxUniversal)
            {
                this.RenameStagingAreaFile("LinuxPlayer", FileUtil.UnityGetFileName(base.InstallPath));
            }
            else
            {
                BuildTarget[] targetArray1 = new BuildTarget[] { BuildTarget.StandaloneLinux, BuildTarget.StandaloneLinux64 };
                if (<>f__mg$cache0 == null)
                {
                    <>f__mg$cache0 = new Func<BuildTarget, string>(null, (IntPtr) ArchitecturePostFixFor);
                }
                foreach (string str in Enumerable.Select<BuildTarget, string>(targetArray1, <>f__mg$cache0))
                {
                    this.RenameStagingAreaFile(Path.ChangeExtension("LinuxPlayer", str), Path.ChangeExtension(FileUtil.UnityGetFileName(base.InstallPath), str));
                }
            }
        }

        private void RenameStagingAreaFile(string from, string to)
        {
            FileUtil.MoveFileOrDirectory(Path.Combine(base.StagingArea, from), Path.Combine(base.StagingArea, to));
        }

        public bool SupportsInstallInBuildFolder()
        {
            return true;
        }

        public bool SupportsScriptsOnlyBuild()
        {
            return false;
        }

        private string VariationNameFor(BuildTarget target)
        {
            return string.Format("{0}_{1}_{2}_{3}", new object[] { this.PlatformStringFor(target), !this.Headless ? "withgfx" : "headless", !base.Development ? "nondevelopment" : "development", this.ScriptingBackend });
        }

        protected override string DestinationFolderForInstallingIntoBuildsFolder
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        private string FullDataFolderPath
        {
            get
            {
                return Path.Combine(this.DestinationFolder, Path.GetFileNameWithoutExtension(base.InstallPath) + "_Data");
            }
        }

        private bool Headless
        {
            get
            {
                return ((this.m_PostProcessArgs.options & BuildOptions.EnableHeadlessMode) != BuildOptions.CompressTextures);
            }
        }

        private string ScriptingBackend
        {
            get
            {
                if (base.UseIl2Cpp)
                {
                    return "il2cpp";
                }
                return "mono";
            }
        }

        protected override string StagingAreaPluginsFolder
        {
            get
            {
                string[] paths = new string[] { base.StagingArea, "Data", "Plugins" };
                return FileUtil.CombinePaths(paths);
            }
        }
    }
}

