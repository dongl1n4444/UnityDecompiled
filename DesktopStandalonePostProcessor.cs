using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.Modules;
using UnityEditorInternal;
using UnityEngine;

internal abstract class DesktopStandalonePostProcessor
{
    [CompilerGenerated]
    private static Action<string> <>f__am$cache0;
    [CompilerGenerated]
    private static Func<string, bool> <>f__am$cache1;
    [CompilerGenerated]
    private static Func<string, bool> <>f__am$cache2;
    protected BuildPostProcessArgs m_PostProcessArgs;

    protected DesktopStandalonePostProcessor()
    {
    }

    protected DesktopStandalonePostProcessor(BuildPostProcessArgs postProcessArgs)
    {
        this.m_PostProcessArgs = postProcessArgs;
    }

    protected abstract void CopyDataForBuildsFolder();
    protected virtual bool CopyFilter(string path)
    {
        bool flag = !path.Contains("UnityEngine.mdb");
        return (flag & !path.Contains("UnityEngine.xml"));
    }

    private void CopyNativePlugins()
    {
        string buildTargetName = BuildPipeline.GetBuildTargetName(this.m_PostProcessArgs.target);
        IPluginImporterExtension extension = new DesktopPluginImporterExtension();
        string stagingAreaPluginsFolder = this.StagingAreaPluginsFolder;
        string path = Path.Combine(stagingAreaPluginsFolder, "x86");
        string str4 = Path.Combine(stagingAreaPluginsFolder, "x86_64");
        bool flag = false;
        bool flag2 = false;
        bool flag3 = false;
        foreach (PluginImporter importer in PluginImporter.GetImporters(this.m_PostProcessArgs.target))
        {
            BuildTarget platform = this.m_PostProcessArgs.target;
            if (importer.isNativePlugin)
            {
                if (string.IsNullOrEmpty(importer.assetPath))
                {
                    Debug.LogWarning("Got empty plugin importer path for " + this.m_PostProcessArgs.target.ToString());
                    continue;
                }
                if (!flag)
                {
                    Directory.CreateDirectory(stagingAreaPluginsFolder);
                    flag = true;
                }
                bool flag4 = Directory.Exists(importer.assetPath);
                switch (importer.GetPlatformData(platform, "CPU"))
                {
                    case "x86":
                        switch (platform)
                        {
                            case BuildTarget.StandaloneOSXIntel64:
                            case BuildTarget.StandaloneWindows64:
                            case BuildTarget.StandaloneLinux64:
                            {
                                continue;
                            }
                        }
                        if (!flag2)
                        {
                            Directory.CreateDirectory(path);
                            flag2 = true;
                        }
                        break;

                    case "x86_64":
                        if ((((platform != BuildTarget.StandaloneOSXIntel64) && (platform != BuildTarget.StandaloneOSXUniversal)) && ((platform != BuildTarget.StandaloneWindows64) && (platform != BuildTarget.StandaloneLinux64))) && (platform != BuildTarget.StandaloneLinuxUniversal))
                        {
                            continue;
                        }
                        if (!flag3)
                        {
                            Directory.CreateDirectory(str4);
                            flag3 = true;
                        }
                        break;

                    case "None":
                    {
                        continue;
                    }
                }
                string str6 = extension.CalculateFinalPluginPath(buildTargetName, importer);
                if (!string.IsNullOrEmpty(str6))
                {
                    string target = Path.Combine(stagingAreaPluginsFolder, str6);
                    if (flag4)
                    {
                        FileUtil.CopyDirectoryRecursive(importer.assetPath, target);
                    }
                    else
                    {
                        FileUtil.UnityFileCopy(importer.assetPath, target);
                    }
                }
            }
        }
        foreach (PluginDesc desc in PluginImporter.GetExtensionPlugins(this.m_PostProcessArgs.target))
        {
            if (!flag)
            {
                Directory.CreateDirectory(stagingAreaPluginsFolder);
                flag = true;
            }
            string str8 = Path.Combine(stagingAreaPluginsFolder, Path.GetFileName(desc.pluginPath));
            if (!Directory.Exists(str8) && !File.Exists(str8))
            {
                if (Directory.Exists(desc.pluginPath))
                {
                    FileUtil.CopyDirectoryRecursive(desc.pluginPath, str8);
                }
                else
                {
                    FileUtil.CopyFileIfExists(desc.pluginPath, str8, false);
                }
            }
        }
    }

    protected void CopyStagingAreaIntoDestination()
    {
        if (this.InstallingIntoBuildsFolder)
        {
            string path = Unsupported.GetBaseUnityDeveloperFolder() + "/" + this.DestinationFolderForInstallingIntoBuildsFolder;
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                throw new Exception("Installing in builds folder failed because the player has not been built (You most likely want to enable 'Development build').");
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<string, bool>(null, (IntPtr) <CopyStagingAreaIntoDestination>m__2);
            }
            FileUtil.CopyDirectoryFiltered(this.DataFolder, path, true, <>f__am$cache1, true);
        }
        else
        {
            this.DeleteDestination();
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<string, bool>(null, (IntPtr) <CopyStagingAreaIntoDestination>m__3);
            }
            FileUtil.CopyDirectoryFiltered(this.StagingArea, this.DestinationFolder, true, <>f__am$cache2, true);
        }
    }

    protected virtual void CopyVariationFolderIntoStagingArea()
    {
        FileUtil.CopyDirectoryFiltered(this.m_PostProcessArgs.playerPackage + "/Variations/" + this.GetVariationName(), this.StagingArea, true, new Func<string, bool>(this, (IntPtr) this.<CopyVariationFolderIntoStagingArea>m__1), true);
    }

    protected void CreateApplicationData()
    {
        string[] textArray1 = new string[] { this.m_PostProcessArgs.companyName, this.m_PostProcessArgs.productName };
        File.WriteAllText(Path.Combine(this.DataFolder, "app.info"), string.Join("\n", textArray1));
    }

    protected abstract void DeleteDestination();
    protected abstract IIl2CppPlatformProvider GetPlatformProvider(BuildTarget target);
    protected virtual string GetVariationName() => 
        $"{this.PlatformStringFor(this.m_PostProcessArgs.target)}_{(!this.Development ? "nondevelopment" : "development")}";

    protected abstract string PlatformStringFor(BuildTarget target);
    public void PostProcess()
    {
        this.SetupStagingArea();
        this.CopyStagingAreaIntoDestination();
    }

    protected abstract void RenameFilesInStagingArea();
    protected virtual void SetupStagingArea()
    {
        Directory.CreateDirectory(this.DataFolder);
        this.CopyNativePlugins();
        if ((this.m_PostProcessArgs.target == BuildTarget.StandaloneWindows) || (this.m_PostProcessArgs.target == BuildTarget.StandaloneWindows64))
        {
            this.CreateApplicationData();
        }
        PostprocessBuildPlayer.InstallStreamingAssets(this.DataFolder);
        if (this.UseIl2Cpp)
        {
            this.CopyVariationFolderIntoStagingArea();
            string stagingAreaData = this.StagingArea + "/Data";
            string destinationFolder = this.DataFolder + "/Managed";
            string dir = destinationFolder + "/Resources";
            string str4 = destinationFolder + "/Metadata";
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = delegate (string s) {
                };
            }
            IL2CPPUtils.RunIl2Cpp(stagingAreaData, this.GetPlatformProvider(this.m_PostProcessArgs.target), <>f__am$cache0, this.m_PostProcessArgs.usedClassRegistry, this.Development);
            FileUtil.CreateOrCleanDirectory(dir);
            IL2CPPUtils.CopyEmbeddedResourceFiles(stagingAreaData, dir);
            FileUtil.CreateOrCleanDirectory(str4);
            IL2CPPUtils.CopyMetadataFiles(stagingAreaData, str4);
            IL2CPPUtils.CopySymmapFile(stagingAreaData + "/Native/Data", destinationFolder);
        }
        if (this.InstallingIntoBuildsFolder)
        {
            this.CopyDataForBuildsFolder();
        }
        else
        {
            if (!this.UseIl2Cpp)
            {
                this.CopyVariationFolderIntoStagingArea();
            }
            this.RenameFilesInStagingArea();
            this.m_PostProcessArgs.report.AddFilesRecursive(this.StagingArea, "");
            this.m_PostProcessArgs.report.RelocateFiles(this.StagingArea, "");
        }
    }

    protected string DataFolder =>
        (this.StagingArea + "/Data");

    protected virtual string DestinationFolder =>
        FileUtil.UnityGetDirectoryName(this.m_PostProcessArgs.installPath);

    protected abstract string DestinationFolderForInstallingIntoBuildsFolder { get; }

    protected bool Development =>
        ((this.m_PostProcessArgs.options & BuildOptions.Development) != BuildOptions.CompressTextures);

    protected bool InstallingIntoBuildsFolder =>
        ((this.m_PostProcessArgs.options & BuildOptions.InstallInBuildFolder) != BuildOptions.CompressTextures);

    protected string InstallPath =>
        this.m_PostProcessArgs.installPath;

    protected string StagingArea =>
        this.m_PostProcessArgs.stagingArea;

    protected abstract string StagingAreaPluginsFolder { get; }

    protected BuildTarget Target =>
        this.m_PostProcessArgs.target;

    protected bool UseIl2Cpp =>
        (PlayerSettings.GetScriptingBackend(BuildTargetGroup.Standalone) == ScriptingImplementation.IL2CPP);

    internal class ScriptingImplementations : IScriptingImplementations
    {
        public ScriptingImplementation[] Enabled()
        {
            if (Unsupported.IsDeveloperBuild())
            {
                ScriptingImplementation[] implementationArray1 = new ScriptingImplementation[2];
                implementationArray1[1] = ScriptingImplementation.IL2CPP;
                return implementationArray1;
            }
            return new ScriptingImplementation[1];
        }

        public ScriptingImplementation[] Supported()
        {
            ScriptingImplementation[] implementationArray1 = new ScriptingImplementation[2];
            implementationArray1[1] = ScriptingImplementation.IL2CPP;
            return implementationArray1;
        }
    }
}

