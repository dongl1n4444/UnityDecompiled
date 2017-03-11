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
    private static Func<string, string> <>f__am$cache0;
    [CompilerGenerated]
    private static Action<string> <>f__am$cache1;
    private List<string> CppPlugins;
    private static readonly string[] RemappedAssemblies_2_0_Profile = new string[] { 
        "Microsoft.Win32.SafeHandles,mscorlib", "System,mscorlib", "System,System", "System,System.Core", "System.CodeDom.Compiler,System", "System.Collections,mscorlib", "System.Collections.Generic,mscorlib", "System.Collections.Generic,System", "System.Collections.Generic,System.Core", "System.Collections.ObjectModel,mscorlib", "System.Collections.ObjectModel,WindowsBase", "System.Collections.Specialized,System", "System.Collections.Specialized,WindowsBase", "System.ComponentModel,System", "System.ComponentModel.DataAnnotations,System.ComponentModel.DataAnnotations", "System.Data,System.Data",
        "System.Data.Common,System.Data", "System.Diagnostics,mscorlib", "System.Diagnostics,System", "System.Diagnostics.CodeAnalysis,mscorlib", "System.Globalization,mscorlib", "System.IO,mscorlib", "System.IO,System", "System.IO,System.Core", "System.IO.Compression,System", "System.IO.IsolatedStorage,mscorlib", "System.Linq,System.Core", "System.Linq.Expressions,System.Core", "System.Net,System", "System.Net.NetworkInformation,System", "System.Net.Security,System", "System.Net.Sockets,System",
        "System.Reflection,mscorlib", "System.Reflection.Emit,mscorlib", "System.Resources,mscorlib", "System.Runtime,mscorlib", "System.Runtime.CompilerServices,mscorlib", "System.Runtime.CompilerServices,System.Core", "System.Runtime.InteropServices,mscorlib", "System.Runtime.InteropServices,System", "System.Runtime.InteropServices.ComTypes,mscorlib", "System.Runtime.InteropServices.ComTypes,System", "System.Runtime.Serialization,mscorlib", "System.Runtime.Serialization,System.Runtime.Serialization", "System.Runtime.Serialization.Json,System.ServiceModel.Web", "System.Security,mscorlib", "System.Security.Authentication,System", "System.Security.Principal,mscorlib",
        "System.ServiceModel,System.ServiceModel", "System.ServiceModel.Channels,System.ServiceModel", "System.ServiceModel.Description,System.ServiceModel", "System.ServiceModel.Dispatcher,System.ServiceModel", "System.ServiceModel.Security,System.ServiceModel", "System.ServiceModel.Security.Tokens,System.ServiceModel", "System.Text,mscorlib", "System.Text.RegularExpressions,System", "System.Threading,mscorlib", "System.Threading,System", "System.Threading,System.Core", "System.Xml,System.Runtime.Serialization", "System.Xml,System.Xml", "System.Xml.Linq,System.Xml.Linq", "System.Xml.Schema,System.Xml", "System.Xml.Serialization,System.Xml"
    };
    private static readonly string[] RemappedAssemblies_2_0_SubsetProfile = new string[] { 
        "Microsoft.Win32.SafeHandles,mscorlib", "System,mscorlib", "System,System", "System,System.Core", "System.CodeDom.Compiler,System", "System.Collections,mscorlib", "System.Collections.Generic,mscorlib", "System.Collections.Generic,System", "System.Collections.Generic,System.Core", "System.Collections.ObjectModel,mscorlib", "System.Collections.Specialized,System", "System.ComponentModel,System", "System.Data,System.Data", "System.Data.Common,System.Data", "System.Diagnostics,mscorlib", "System.Diagnostics,System",
        "System.Diagnostics.CodeAnalysis,mscorlib", "System.Globalization,mscorlib", "System.IO,mscorlib", "System.IO.Compression,System", "System.IO.IsolatedStorage,mscorlib", "System.Linq,System.Core", "System.Linq.Expressions,System.Core", "System.Net,System", "System.Net.NetworkInformation,System", "System.Net.Security,System", "System.Net.Sockets,System", "System.Reflection,mscorlib", "System.Reflection.Emit,mscorlib", "System.Resources,mscorlib", "System.Runtime,mscorlib", "System.Runtime.CompilerServices,mscorlib",
        "System.Runtime.CompilerServices,System.Core", "System.Runtime.InteropServices,mscorlib", "System.Runtime.InteropServices.ComTypes,mscorlib", "System.Runtime.Serialization,mscorlib", "System.Runtime.Serialization,System.Runtime.Serialization", "System.Runtime.Serialization.Json,System.ServiceModel.Web", "System.Security,mscorlib", "System.Security.Authentication,System", "System.Security.Principal,mscorlib", "System.ServiceModel,System.ServiceModel", "System.ServiceModel.Channels,System.ServiceModel", "System.ServiceModel.Description,System.ServiceModel", "System.ServiceModel.Dispatcher,System.ServiceModel", "System.ServiceModel.Security,System.ServiceModel", "System.Text,mscorlib", "System.Text.RegularExpressions,System",
        "System.Threading,mscorlib", "System.Threading,System", "System.Threading,System.Core", "System.Xml,System.Runtime.Serialization", "System.Xml,System.Xml", "System.Xml.Linq,System.Xml.Linq", "System.Xml.Schema,System.Xml", "System.Xml.Serialization,System.Xml"
    };
    private static readonly string[] RemappedAssemblies_4_6_Profile = new string[] { 
        "Microsoft.Win32.SafeHandles,mscorlib", "System,mscorlib", "System,System", "System,System.ComponentModel.Composition", "System,System.Core", "System.CodeDom.Compiler,System", "System.Collections,mscorlib", "System.Collections.Concurrent,mscorlib", "System.Collections.Concurrent,System", "System.Collections.Generic,mscorlib", "System.Collections.Generic,System", "System.Collections.Generic,System.Core", "System.Collections.ObjectModel,mscorlib", "System.Collections.ObjectModel,System", "System.Collections.Specialized,System", "System.ComponentModel,System",
        "System.ComponentModel.DataAnnotations,System.ComponentModel.DataAnnotations", "System.ComponentModel.DataAnnotations.Schema,System.ComponentModel.DataAnnotations", "System.Data,System.Data", "System.Data.Common,System.Data", "System.Diagnostics,mscorlib", "System.Diagnostics,System", "System.Diagnostics.CodeAnalysis,mscorlib", "System.Diagnostics.Contracts,mscorlib", "System.Diagnostics.Tracing,mscorlib", "System.Dynamic,System.Core", "System.Globalization,mscorlib", "System.IO,mscorlib", "System.IO,System", "System.IO,System.Core", "System.IO.Compression,System", "System.IO.Compression,System.IO.Compression.FileSystem",
        "System.IO.IsolatedStorage,mscorlib", "System.Linq,System.Core", "System.Linq.Expressions,System.Core", "System.Net,System", "System.Net.NetworkInformation,System", "System.Net.Security,System", "System.Net.Sockets,System", "System.Numerics,System.Numerics", "System.Reflection,mscorlib", "System.Reflection.Emit,mscorlib", "System.Resources,mscorlib", "System.Runtime,mscorlib", "System.Runtime.CompilerServices,mscorlib", "System.Runtime.CompilerServices,System.Core", "System.Runtime.ExceptionServices,mscorlib", "System.Runtime.InteropServices,mscorlib",
        "System.Runtime.InteropServices,System", "System.Runtime.InteropServices,System.Core", "System.Runtime.InteropServices.ComTypes,mscorlib", "System.Runtime.InteropServices.ComTypes,System", "System.Runtime.InteropServices.WindowsRuntime,mscorlib", "System.Runtime.Serialization,mscorlib", "System.Runtime.Serialization,System.Runtime.Serialization", "System.Runtime.Serialization.Json,System.Runtime.Serialization", "System.Runtime.Versioning,mscorlib", "System.Runtime.Versioning,System", "System.Security,mscorlib", "System.Security.Authentication,System", "System.Security.Authentication.ExtendedProtection,System", "System.Security.Claims,mscorlib", "System.Security.Principal,mscorlib", "System.ServiceModel,System.ServiceModel",
        "System.ServiceModel.Channels,System.ServiceModel", "System.ServiceModel.Description,System.ServiceModel", "System.ServiceModel.Dispatcher,System.ServiceModel", "System.ServiceModel.Security,System.ServiceModel", "System.ServiceModel.Security.Tokens,System.ServiceModel", "System.Text,mscorlib", "System.Text.RegularExpressions,System", "System.Threading,mscorlib", "System.Threading,System", "System.Threading,System.Core", "System.Threading.Tasks,mscorlib", "System.Threading.Tasks,System.Core", "System.Windows.Input,System", "System.Xml,System.Runtime.Serialization", "System.Xml,System.Xml", "System.Xml.Linq,System.Xml.Linq",
        "System.Xml.Schema,System.Xml", "System.Xml.Serialization,System.Xml"
    };
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

    protected override void CopyFrameworkAssemblies()
    {
        string monoLibDirectory = BuildPipeline.GetMonoLibDirectory(BuildTarget.WSAPlayer);
        if (PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.WSA) == ApiCompatibilityLevel.NET_4_6)
        {
            foreach (string str2 in Directory.GetFiles(Path.Combine(monoLibDirectory, "Facades"), "*.dll"))
            {
                string destFileName = Path.Combine(base.StagingAreaDataManaged, Path.GetFileName(str2));
                File.Copy(str2, destFileName, true);
            }
            string[] strArray2 = new string[] { "System.Runtime.WindowsRuntime.dll", "System.Runtime.WindowsRuntime.UI.Xaml.dll" };
            foreach (string str4 in strArray2)
            {
                string[] components = new string[] { IL2CPPUtils.editorIl2cppFolder, "BCLExtensions", str4 };
                string sourceFileName = Paths.Combine(components);
                string str6 = Path.Combine(base.StagingAreaDataManaged, str4);
                File.Copy(sourceFileName, str6, true);
            }
            foreach (string str7 in PostProcessUAP.UWPReferences)
            {
                string str8 = Path.Combine(base.StagingAreaDataManaged, Path.GetFileName(str7));
                File.Copy(str7, str8, true);
            }
        }
        foreach (string str9 in Directory.GetFiles(monoLibDirectory, "*.dll"))
        {
            string path = Path.Combine(base.StagingAreaDataManaged, Path.GetFileName(str9));
            if (!File.Exists(path))
            {
                File.Copy(str9, path, true);
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
        using (new ProfilerBlock("VisualStudioSolutionHelper.WriteUnityCommonProps"))
        {
            MetroVisualStudioSolutionHelper.WriteUnityCommonProps(Path.Combine(base.InstallPath, "UnityCommon.props"), base.PlayerPackage, base.InstallPath, base.SourceBuild, false);
        }
    }

    protected override string GetAlternativeReferenceRewritterMappings()
    {
        string[] strArray;
        ApiCompatibilityLevel apiCompatibilityLevel = PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.WSA);
        switch (apiCompatibilityLevel)
        {
            case ApiCompatibilityLevel.NET_2_0:
                strArray = RemappedAssemblies_2_0_Profile;
                break;

            case ApiCompatibilityLevel.NET_2_0_Subset:
                strArray = RemappedAssemblies_2_0_SubsetProfile;
                break;

            case ApiCompatibilityLevel.NET_4_6:
                strArray = RemappedAssemblies_4_6_Profile;
                break;

            default:
                throw new InvalidOperationException($"Cannot retrieve reference rewriter mappings for ApiCompatibilityLevel.{apiCompatibilityLevel}!");
        }
        string str = string.Join(";", strArray);
        if (apiCompatibilityLevel == ApiCompatibilityLevel.NET_4_6)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = r => Path.GetFileName(r);
            }
            foreach (string str2 in Enumerable.Select<string, string>(PostProcessUAP.UWPReferences, <>f__am$cache0))
            {
                str = str + ";<winmd>," + str2;
            }
        }
        return str;
    }

    protected override IEnumerable<string> GetAssembliesIgnoredByReferenceRewriter()
    {
        string[] second = new string[] { "UnityEngine.Networking", "UnityEngine.UI" };
        return base.GetAssembliesIgnoredByReferenceRewriter().Concat<string>(second).ToArray<string>();
    }

    protected override string GetAssemblyConverterPlatform()
    {
        throw new NotSupportedException();
    }

    protected override IEnumerable<string> GetLangAssemblies() => 
        new string[0];

    protected override string GetPlayerFilesSourceDirectory() => 
        base.GetPlayerFilesSourceDirectory(Path.Combine("UAP", "il2cpp"));

    protected override string GetReferenceAssembliesDirectory() => 
        base.StagingAreaDataManaged;

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
        string str = Utility.CombinePath(base.StagingArea, "Il2CppOutputProject");
        string path = Utility.CombinePath(str, "Source");
        if (EditorUtility.DisplayCancelableProgressBar("Building Player", "Cleaning up old IL2CPP generated files", 0.1f))
        {
            throw new OperationCanceledException();
        }
        using (new ProfilerBlock("PostProcessUAPIl2Cpp.RunIL2CPP.DeletePreviousGeneratedCppCode"))
        {
            Utility.DeleteDirectoryRecursive(str);
            Utility.CreateDirectory(path);
        }
        WinRTIl2CppPlatformProvider platformProvider = new WinRTIl2CppPlatformProvider();
        using (new ProfilerBlock("IL2CPPUtils.RunIL2CPP"))
        {
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = delegate (string s) {
                };
            }
            IL2CPPUtils.RunIl2Cpp(path, base.StagingAreaData, platformProvider, <>f__am$cache1, this.usedClassRegistry, false);
        }
        if (EditorUtility.DisplayCancelableProgressBar("Building Player", "Copying IL2CPP related files", 0.1f))
        {
            throw new OperationCanceledException();
        }
        string cppOutputPath = IL2CPPBuilder.GetCppOutputPath(path);
        string[] components = new string[] { cppOutputPath, "Data" };
        string source = Paths.Combine(components);
        string str5 = Utility.CombinePath(base.StagingAreaData, "il2cpp_data");
        using (new ProfilerBlock("IL2CPPUtils.MoveIL2CPPGeneratedDataFiles"))
        {
            if (Directory.Exists(str5))
            {
                Directory.Delete(str5, true);
            }
            Utility.MoveDirectory(source, str5, null);
        }
        using (new ProfilerBlock("IL2CPPUtils.CopyIL2CPP"))
        {
            string str6 = platformProvider.il2CppFolder;
            string[] textArray2 = new string[] { str, "IL2CPP" };
            string str7 = Paths.Combine(textArray2);
            using (new ProfilerBlock("Copy IL2CPP binaries"))
            {
                Utility.CopyDirectoryContents(Path.Combine(str6, "build"), Path.Combine(str7, "build"), true);
            }
            using (new ProfilerBlock("Copy libil2cpp external dependencies"))
            {
                Utility.CopyDirectoryContents(Path.Combine(str6, "external"), Path.Combine(str7, "external"), true);
            }
            using (new ProfilerBlock("Copy libil2cpp"))
            {
                Utility.CopyDirectoryContents(Path.Combine(str6, "libil2cpp"), Path.Combine(str7, "libil2cpp"), true);
            }
            using (new ProfilerBlock("Copy MapFileParser"))
            {
                Utility.CopyDirectoryContents(Path.GetDirectoryName(IL2CPPBuilder.GetMapFileParserPath()), Path.Combine(str7, "MapFileParser"), true);
            }
            using (new ProfilerBlock("Copy il2cpp_root"))
            {
                Utility.CopyFile(Path.Combine(str6, "il2cpp_root"), Path.Combine(str7, "il2cpp_root"));
            }
        }
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

