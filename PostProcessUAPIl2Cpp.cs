using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.Modules;
using UnityEditor.Utils;
using UnityEditorInternal;
using UnityEngine;

internal class PostProcessUAPIl2Cpp : PostProcessUAP
{
    [CompilerGenerated]
    private static Action<string> <>f__am$cache0;
    private List<string> CppPlugins;
    private RuntimeClassRegistry usedClassRegistry;

    public PostProcessUAPIl2Cpp(BuildPostProcessArgs args, string stagingArea = null) : base(args, stagingArea)
    {
        this.CppPlugins = new List<string>();
        this.usedClassRegistry = args.usedClassRegistry;
    }

    public override void CheckSDK()
    {
        if (this.GetPlatformAssemblyPath() == null)
        {
            throw new UnityException($"{"Platform assembly not found."}{Environment.NewLine}{this.GetSDKNotFoundErrorMessage()}");
        }
    }

    private void CopyClassLibraries()
    {
        foreach (string str in Directory.GetFiles(BuildPipeline.GetMonoLibDirectory(BuildTarget.WSAPlayer), "*.dll"))
        {
            string path = Path.Combine(base.StagingAreaDataManaged, Path.GetFileName(str));
            if (!File.Exists(path))
            {
                File.Copy(str, path);
            }
        }
    }

    public override void CopyPlugins()
    {
        string[] strArray = new string[] { ".cpp", ".c", ".h" };
        foreach (PluginImporter importer in PluginImporter.GetImporters(base.Target))
        {
            <CopyPlugins>c__AnonStorey0 storey = new <CopyPlugins>c__AnonStorey0();
            string assetPath = importer.assetPath;
            storey.pluginExtension = Path.GetExtension(assetPath);
            if (Enumerable.Any<string>(strArray, new Func<string, bool>(storey.<>m__0)))
            {
                this.CppPlugins.Add(assetPath);
            }
        }
        base.CopyPlugins();
        foreach (Library library in base.Libraries)
        {
            if (library.Native && library.WinMd)
            {
                File.Copy(Path.Combine(base.StagingAreaDataManaged, library.Reference), Path.Combine(base.StagingArea, library.Reference), true);
            }
        }
    }

    public override void CopyTemplate()
    {
        base.CopyTemplate();
        string templateDirectoryTarget = this.GetTemplateDirectoryTarget();
        foreach (string str2 in Directory.GetFiles(templateDirectoryTarget))
        {
            MetroVisualStudioSolutionHelper.PatchVisualStudioFile(str2);
        }
    }

    protected override void CreateManagedRegistryTxtFile()
    {
    }

    public override void CreateVisualStudioSolution()
    {
        MetroIl2CppVisualStudioSolutionCreator.CreateSolution(base.InstallPath, base.VisualStudioName, base.StagingArea, base.SourceBuild, this.CppPlugins, base.Libraries);
        MetroVisualStudioSolutionHelper.WriteUnityCommonProps(Path.Combine(base.InstallPath, "UnityCommon.props"), base.PlayerPackage, base.InstallPath, base.SourceBuild);
    }

    protected override string GetAssemblyConverterPlatform()
    {
        throw new NotSupportedException();
    }

    protected override IEnumerable<string> GetLangAssemblies() => 
        new string[0];

    protected override string GetPlayerFilesSourceDirectory() => 
        base.GetPlayerFilesSourceDirectory(Path.Combine("UAP", "il2cpp"));

    protected override string GetReferenceAssembliesDirectory()
    {
        throw new NotSupportedException();
    }

    protected override string GetTemplateDirectorySource()
    {
        string str = (EditorUserBuildSettings.wsaUWPBuildType != WSAUWPBuildType.XAML) ? "UWP_IL2CPP_D3D" : "UWP_IL2CPP_XAML";
        string[] paths = new string[] { base.PlayerPackage, "Templates", str };
        return Utility.CombinePath(paths);
    }

    protected override IEnumerable<string> GetUnityAssemblies() => 
        new string[] { @"il2cpp\UnityEngine.dll" };

    protected override IEnumerable<string> GetUnityPluginOverwrites() => 
        new string[0];

    protected override void RunIL2CPP()
    {
        string path = Utility.CombinePath(base.StagingArea, "Il2CppOutputProject");
        if (EditorUtility.DisplayCancelableProgressBar("Building Player", "Cleaning up old IL2CPP generated files", 0.1f))
        {
            throw new OperationCanceledException();
        }
        if (Directory.Exists(path))
        {
            foreach (string str2 in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
            {
                File.Delete(str2);
            }
        }
        this.CopyClassLibraries();
        WinRTIl2CppPlatformProvider platformProvider = new WinRTIl2CppPlatformProvider();
        if (<>f__am$cache0 == null)
        {
            <>f__am$cache0 = delegate (string s) {
            };
        }
        IL2CPPUtils.RunIl2Cpp(path, base.StagingAreaData, platformProvider, <>f__am$cache0, this.usedClassRegistry, false);
        if (EditorUtility.DisplayCancelableProgressBar("Building Player", "Copying IL2CPP related files", 0.1f))
        {
            throw new OperationCanceledException();
        }
        string cppOutputPath = IL2CPPBuilder.GetCppOutputPath(path);
        string[] components = new string[] { cppOutputPath, "Data" };
        string source = Paths.Combine(components);
        string str5 = Utility.CombinePath(base.StagingAreaData, "il2cpp_data");
        if (Directory.Exists(str5))
        {
            Directory.Delete(str5, true);
        }
        Directory.CreateDirectory(str5);
        Utility.MoveDirectory(source, str5, null);
        Utility.CopyDirectoryContents(platformProvider.il2CppFolder, Path.Combine(path, "IL2CPP"), true);
    }

    protected override bool UseIL2CPP() => 
        true;

    [CompilerGenerated]
    private sealed class <CopyPlugins>c__AnonStorey0
    {
        internal string pluginExtension;

        internal bool <>m__0(string e) => 
            string.Equals(e, this.pluginExtension, StringComparison.InvariantCultureIgnoreCase);
    }
}

