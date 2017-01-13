using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEditor.Modules;
using UnityEditor.WSA;
using UnityEditorInternal;
using UnityEngine;

internal abstract class PostProcessWinRT
{
    private readonly BuildPostProcessArgs _args;
    protected readonly WSASDK _sdk;
    [CompilerGenerated]
    private static Func<Library, bool> <>f__am$cache0;
    [CompilerGenerated]
    private static Func<Library, string> <>f__am$cache1;
    protected const string cpuTag = "CPU";
    private static readonly string[] debugSymbolsExtensions = new string[] { ".pdb", ".mdb" };
    protected const string dontProcessPluginsTag = "DontProcess";
    protected readonly string InstallPath;
    public readonly LibraryCollection Libraries;
    private string m_ManagedAssemblyLocation;
    protected const string placeHolderPathTag = "PlaceholderPath";
    protected readonly string PlayerPackage;
    protected const string resources = @"Data\Resources\unity default resources";
    protected const string sdkTag = "SDK";
    protected readonly string StagingArea;
    protected readonly string StagingAreaData;
    public readonly string StagingAreaDataManaged;
    protected readonly string VisualStudioName;

    public PostProcessWinRT(BuildPostProcessArgs args, WSASDK sdk, string stagingArea = null)
    {
        this._args = args;
        this._sdk = sdk;
        this.PlayerPackage = this._args.playerPackage.ConvertToWindowsPath();
        if (stagingArea == null)
        {
        }
        this.StagingArea = args.stagingArea.ConvertToWindowsPath();
        this.StagingAreaData = Utility.CombinePath(this.StagingArea, "Data");
        this.StagingAreaDataManaged = Utility.CombinePath(this.StagingAreaData, "Managed");
        this.VisualStudioName = Utility.GetVsName();
        this.Libraries = new LibraryCollection();
        this.m_ManagedAssemblyLocation = this.StagingAreaDataManaged;
        if (this.SourceBuild)
        {
            string[] paths = new string[] { this.PlayerPackage, "SourceBuild", this.VisualStudioName };
            this.InstallPath = Utility.CombinePath(paths);
        }
        else
        {
            this.InstallPath = args.installPath.ConvertToWindowsPath();
        }
    }

    private static string ChangeExtensionDebugSymbols(string path, string extension)
    {
        if (string.Equals(extension, ".mdb", StringComparison.InvariantCultureIgnoreCase))
        {
            return (path + ".mdb");
        }
        return Path.ChangeExtension(path, ".pdb");
    }

    public virtual void CheckProfilerCapabilities()
    {
    }

    public virtual void CheckSafeProjectOverwrite()
    {
    }

    public virtual void CheckSDK()
    {
        if (this.GetPlatformAssemblyPath() == null)
        {
            string message = "Platform assembly not found.";
            string sDKNotFoundErrorMessage = this.GetSDKNotFoundErrorMessage();
            if (sDKNotFoundErrorMessage != null)
            {
                message = message + '\n' + sDKNotFoundErrorMessage;
            }
            throw new UnityException(message);
        }
    }

    public virtual void CheckVisualStudio()
    {
    }

    public virtual void CheckWindows()
    {
        bool flag = false;
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            Version version = Environment.OSVersion.Version;
            flag = (version.Major > 6) || ((version.Major == 6) && (version.Minor >= 2));
        }
        if (!flag)
        {
            throw new UnityException($"Windows 8 or newer is required to build {this.Target}.");
        }
    }

    private static void CopyFileWithDebugSymbols(string source, string target)
    {
        File.Copy(source, target, true);
        foreach (string str in debugSymbolsExtensions)
        {
            string path = ChangeExtensionDebugSymbols(source, str);
            string destFileName = ChangeExtensionDebugSymbols(target, str);
            if (File.Exists(path))
            {
                File.Copy(path, destFileName, true);
            }
            else if (File.Exists(destFileName))
            {
                File.Delete(destFileName);
            }
        }
    }

    public virtual void CopyImages()
    {
    }

    public virtual void CopyPlayerFiles()
    {
        string[] extensions = new string[] { ".dll", ".winmd", ".pdb", ".lib" };
        this.CopyPlayerFiles(extensions);
    }

    public virtual void CopyPlayerFiles(string[] extensions)
    {
        <CopyPlayerFiles>c__AnonStorey9 storey = new <CopyPlayerFiles>c__AnonStorey9 {
            extensions = extensions
        };
        if (!this.SourceBuild)
        {
            string playerFilesSourceDirectory = this.GetPlayerFilesSourceDirectory();
            string playerFilesTargetDirectory = this.GetPlayerFilesTargetDirectory();
            IEnumerable<string> enumerable = Enumerable.Where<string>(Directory.GetFiles(playerFilesSourceDirectory, "*", SearchOption.AllDirectories), new Func<string, bool>(storey.<>m__0));
            foreach (string str3 in enumerable)
            {
                string path = str3.Substring(playerFilesSourceDirectory.Length + 1);
                if (!string.Equals(Path.GetFileName(path), "d3dcompiler_46.dll", StringComparison.InvariantCultureIgnoreCase))
                {
                    string fileName = Utility.CombinePath(playerFilesTargetDirectory, path);
                    FileInfo info = new FileInfo(fileName);
                    if (info.Exists)
                    {
                        FileInfo info2 = new FileInfo(str3);
                        if ((info2.Length == info.Length) && (info2.LastWriteTime == info.LastWriteTime))
                        {
                            continue;
                        }
                    }
                    Utility.CreateDirectory(Path.GetDirectoryName(fileName));
                    File.Copy(str3, fileName, true);
                }
            }
        }
    }

    public virtual void CopyPlugins()
    {
        string buildTargetName = BuildPipeline.GetBuildTargetName(this.Target);
        HashSet<string> incompatiblePlugins = new HashSet<string>();
        foreach (PluginImporter importer in PluginImporter.GetAllImporters())
        {
            <CopyPlugins>c__AnonStorey3 storey = new <CopyPlugins>c__AnonStorey3();
            string fileName = Path.GetFileName(importer.assetPath);
            if (!string.IsNullOrEmpty(fileName))
            {
                storey.pluginName = Path.GetFileNameWithoutExtension(fileName);
                if (!Enumerable.Any<string>(this.GetCompatibleBuiltinPlugins(), new Func<string, bool>(storey.<>m__0)))
                {
                    DeletePlugin(Path.Combine(this.StagingAreaDataManaged, fileName));
                    incompatiblePlugins.Add(storey.pluginName);
                }
            }
        }
        List<PluginData> plugins = new List<PluginData>();
        foreach (PluginImporter importer2 in PluginImporter.GetImporters(this.Target))
        {
            <CopyPlugins>c__AnonStorey4 storey2 = new <CopyPlugins>c__AnonStorey4();
            if (!Directory.Exists(importer2.assetPath))
            {
                storey2.extension = Path.GetExtension(importer2.assetPath);
                string[] strArray = new string[] { ".dll", ".winmd" };
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(importer2.assetPath);
                string assetPath = importer2.assetPath;
                string str6 = MetroPluginImporterExtension.CalculateFinalPluginPath(buildTargetName, importer2, this._sdk);
                if (string.IsNullOrEmpty(str6) || !Enumerable.Any<string>(strArray, new Func<string, bool>(storey2.<>m__0)))
                {
                    incompatiblePlugins.Add(fileNameWithoutExtension);
                }
                else
                {
                    string path = Path.Combine(this.StagingAreaDataManaged, str6);
                    bool flag = true;
                    string platformData = importer2.GetPlatformData(this.Target, "SDK");
                    flag = string.IsNullOrEmpty(platformData) || string.Equals(platformData, "AnySDK", StringComparison.InvariantCultureIgnoreCase);
                    if (!flag && !string.Equals(platformData, this._sdk.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        incompatiblePlugins.Add(fileNameWithoutExtension);
                    }
                    else
                    {
                        bool flag3;
                        Utility.CreateDirectory(Path.GetDirectoryName(path));
                        CopyFileWithDebugSymbols(assetPath, path);
                        string str10 = importer2.GetPlatformData(buildTargetName, "PlaceholderPath");
                        string str11 = importer2.GetPlatformData(buildTargetName, "CPU");
                        DllType type = InternalEditorUtility.DetectDotNetDll(path);
                        bool flag2 = string.Equals(Path.GetExtension(path), ".winmd", StringComparison.InvariantCultureIgnoreCase);
                        if (!bool.TryParse(importer2.GetPlatformData(this.Target, "DontProcess"), out flag3))
                        {
                            flag3 = false;
                        }
                        PluginData item = new PluginData {
                            Name = fileNameWithoutExtension,
                            PlaceholderPath = str10,
                            Cpu = str11,
                            AnySdk = flag,
                            Native = type == DllType.Native,
                            WinMd = flag2,
                            Process = !flag3
                        };
                        plugins.Add(item);
                    }
                }
            }
        }
        this.OverwriteAssemblies(this.GetUnityPluginOverwrites(), true);
        this.Libraries.RegisterPlugins(plugins, incompatiblePlugins);
    }

    public virtual void CopyStreamingAssets()
    {
        if (Directory.Exists("Assets/StreamingAssets"))
        {
            FileUtil.CopyDirectoryRecursiveForPostprocess("Assets/StreamingAssets", Utility.CombinePath(this.StagingArea, "Data/StreamingAssets"), true);
        }
    }

    public virtual void CopyTemplate()
    {
    }

    public virtual void CopyTestCertificate()
    {
    }

    public virtual void CopyUnityResources()
    {
        string sourceFileName = Utility.CombinePath(this.PlayerPackage, @"Data\Resources\unity default resources");
        string destFileName = Utility.CombinePath(this.StagingArea, @"Data\Resources\unity default resources");
        File.Copy(sourceFileName, destFileName);
    }

    public virtual void CreateCommandLineArgsPlaceholder()
    {
    }

    protected virtual void CreateManagedRegistryTxtFile()
    {
        string[] strArray = new string[] { "UnityEngine", "WinRTLegacy" };
        StringBuilder builder = new StringBuilder();
        using (IEnumerator<Library> enumerator = this.Libraries.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                <CreateManagedRegistryTxtFile>c__AnonStorey8 storey = new <CreateManagedRegistryTxtFile>c__AnonStorey8 {
                    library = enumerator.Current
                };
                if ((!storey.library.Native && !storey.library.WinMd) && !Enumerable.Any<string>(strArray, new Func<string, bool>(storey.<>m__0)))
                {
                    if (builder.Length != 0)
                    {
                        builder.AppendLine();
                    }
                    builder.Append(storey.library.Reference);
                }
            }
        }
        if (builder.Length != 0)
        {
            File.WriteAllText(Path.Combine(this.StagingArea, @"Data\managedAssemblies.txt"), builder.ToString());
        }
    }

    public virtual void CreateManifest()
    {
    }

    protected virtual void CreateNativeRegistryTxtFile()
    {
        StringBuilder builder = new StringBuilder();
        foreach (Library library in this.Libraries)
        {
            if (library.Native)
            {
                if (builder.Length != 0)
                {
                    builder.AppendLine();
                }
                string referencePath = library.ReferencePath;
                if (library.WinMd)
                {
                    referencePath = Path.ChangeExtension(referencePath, ".dll");
                }
                builder.Append(Path.GetFileName(referencePath));
            }
        }
        if (builder.Length != 0)
        {
            File.WriteAllText(Path.Combine(this.StagingAreaData, "nativePlugins.txt"), builder.ToString());
        }
    }

    public virtual void CreateRegistryTxtFiles()
    {
        this.CreateManagedRegistryTxtFile();
        this.CreateNativeRegistryTxtFile();
    }

    public virtual void CreateTestCertificate()
    {
    }

    public abstract void CreateVisualStudioSolution();
    protected virtual void CreateWindowsResourceFile()
    {
        this.CreateWindowsResourceFile(this.StagingArea);
    }

    protected void CreateWindowsResourceFile(string targetDirectory)
    {
        string str7;
        string path = "Temp/WindowsResources";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        Texture2D image = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        Color32[] colors = new Color32[] { new Color32(0, 0, 0, 0) };
        image.SetPixels32(colors);
        InternalEditorUtility.SaveCursorToFile(Path.Combine(path, "hiddenCursor.cur"), image, Vector2.zero);
        bool flag = PlayerSettings.defaultCursor != null;
        if (flag)
        {
            InternalEditorUtility.SaveCursorToFile(Path.Combine(path, "defaultCursor.cur"), PlayerSettings.defaultCursor, PlayerSettings.cursorHotspot);
        }
        string str2 = Path.Combine(path, "Resource.rc");
        string str3 = Path.Combine(path, "Resource.h");
        string str4 = Path.Combine(targetDirectory, "Resource.res");
        string[] textArray1 = new string[] { "//{{NO_DEPENDENCIES}}", "#define IDC_HIDDEN_CURSOR  1000", !flag ? "" : "#define IDC_DEFAULT_CURSOR 1001", "" };
        File.WriteAllText(str3, string.Join("\r\n", textArray1));
        string[] textArray2 = new string[] { "#include \"Resource.h\"", "IDC_HIDDEN_CURSOR             CURSOR                  \"hiddenCursor.cur\"", !flag ? "" : "IDC_DEFAULT_CURSOR             CURSOR                  \"defaultCursor.cur\"" };
        File.WriteAllText(str2, string.Join("\r\n", textArray2));
        string resourceCompilerPath = this.GetResourceCompilerPath();
        string arguments = $"/fo "{str4}" "{str2}"";
        if (Utility.RunAndWait(resourceCompilerPath, arguments, out str7, null) != 0)
        {
            string[] textArray3 = new string[] { "Failed to compile windows resources:\n", resourceCompilerPath, " ", arguments, " \n", str7 };
            throw new Exception(string.Concat(textArray3));
        }
    }

    public virtual void DeleteMdbFiles()
    {
        foreach (string str in Directory.GetFiles(this.StagingArea, "*.mdb"))
        {
            File.Delete(str);
        }
    }

    public virtual void DeleteOldData()
    {
        string[] paths = new string[] { this.InstallPath, this.VisualStudioName, "Data" };
        Utility.DeleteDirectory(Utility.CombinePath(paths));
    }

    private static void DeletePlugin(string pluginPath)
    {
        MetroVisualStudioSolutionHelper.DeleteFileAccountingForReadOnly(pluginPath);
        MetroVisualStudioSolutionHelper.DeleteFileAccountingForReadOnly(Path.ChangeExtension(pluginPath, ".pdb"));
        MetroVisualStudioSolutionHelper.DeleteFileAccountingForReadOnly(pluginPath + ".mdb");
    }

    public virtual void EnumerateAllManagedAssemblies()
    {
        string[] files = Directory.GetFiles(this.StagingAreaDataManaged, "*.dll");
        foreach (string str in files)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(str);
            bool flag = (string.Equals(fileNameWithoutExtension, "Boo.Lang", StringComparison.OrdinalIgnoreCase) || string.Equals(fileNameWithoutExtension, "UnityScript.Lang", StringComparison.OrdinalIgnoreCase)) || string.Equals(fileNameWithoutExtension, "WinRTLegacy", StringComparison.OrdinalIgnoreCase);
            this.Libraries.RegisterManagedDll(fileNameWithoutExtension, !flag, null);
        }
    }

    [DebuggerHidden]
    protected virtual IEnumerable<string> GetAdditionalReferenceAssembliesDirectories() => 
        new <GetAdditionalReferenceAssembliesDirectories>c__Iterator1 { 
            $this = this,
            $PC = -2
        };

    protected virtual string GetAlternativeReferenceRewritterMappings() => 
        null;

    protected virtual IEnumerable<string> GetAssembliesIgnoredByReferenceRewriter() => 
        new string[] { "UnityEngine", "WinRTLegacy", "Boo.Lang", "UnityScript.Lang" };

    protected virtual IEnumerable<string> GetAssembliesIgnoredBySerializationWeaver() => 
        new string[] { "WinRTLegacy", "Boo.Lang", "UnityScript.Lang" };

    protected abstract string GetAssemblyConverterPlatform();
    protected virtual IEnumerable<string> GetCompatibleBuiltinPlugins() => 
        new string[] { "UnityEngine.Networking", "UnityEngine.UI" };

    protected virtual string GetIgnoredReferenceRewriterTypes() => 
        null;

    [DebuggerHidden]
    protected virtual IEnumerable<string> GetIgnoreOutputAssembliesForReferenceRewriter() => 
        new <GetIgnoreOutputAssembliesForReferenceRewriter>c__Iterator2 { $PC = -2 };

    protected virtual IEnumerable<string> GetLangAssemblies() => 
        new string[] { "Boo.Lang.dll", "UnityScript.Lang.dll" };

    protected abstract string GetPlatformAssemblyPath();
    protected virtual string GetPlayerFilesSourceDirectory() => 
        Utility.CombinePath(this.PlayerPackage, "Players");

    protected virtual string GetPlayerFilesTargetDirectory() => 
        Utility.CombinePath(this.InstallPath, "Players");

    [DebuggerHidden]
    protected virtual IEnumerable<string> GetReferenceAssembliesDirectories() => 
        new <GetReferenceAssembliesDirectories>c__Iterator0 { 
            $this = this,
            $PC = -2
        };

    protected abstract string GetReferenceAssembliesDirectory();
    protected static string GetReferenceAssembliesDirectory(string path)
    {
        if (path == null)
        {
            return null;
        }
        if (path.EndsWith(@"\", StringComparison.InvariantCulture))
        {
            path = path.Substring(0, path.Length - 1);
        }
        return (!File.Exists(Utility.CombinePath(path, "mscorlib.dll")) ? null : path);
    }

    protected virtual string GetResourceCompilerPath()
    {
        throw new NotImplementedException("GetResourceCompilerPath should be implemented in a child class");
    }

    protected virtual string GetSDKNotFoundErrorMessage() => 
        null;

    protected virtual IEnumerable<string> GetUnityAssemblies() => 
        new string[] { "UnityEngine.dll", "WinRTLegacy.dll" };

    protected virtual IEnumerable<string> GetUnityPluginOverwrites() => 
        new string[] { "UnityEngine.Networking.dll" };

    private static string JoinStrings(IEnumerable<string> strings)
    {
        StringBuilder builder = new StringBuilder();
        foreach (string str in strings)
        {
            if (builder.Length != 0)
            {
                builder.Append(',');
            }
            builder.Append('"');
            builder.Append(str);
            builder.Append('"');
        }
        return builder.ToString();
    }

    private string MakeWinmdAssembliesString()
    {
        if (<>f__am$cache0 == null)
        {
            <>f__am$cache0 = l => l.WinMd;
        }
        if (<>f__am$cache1 == null)
        {
            <>f__am$cache1 = l => l.Name;
        }
        return JoinStrings(Enumerable.Select<Library, string>(Enumerable.Where<Library>(this.Libraries, <>f__am$cache0), <>f__am$cache1));
    }

    public virtual void MoveDataManagedToRoot()
    {
        if (Directory.Exists(this.StagingAreaDataManaged))
        {
            MetroVisualStudioSolutionHelper.RemoveReadOnlyAttributes(this.StagingAreaDataManaged);
            FileUtil.CopyDirectoryRecursive(this.StagingAreaDataManaged, this.StagingArea);
            Utility.DeleteDirectory(this.StagingAreaDataManaged);
        }
        this.m_ManagedAssemblyLocation = this.StagingArea;
    }

    private void OverwriteAssemblies(IEnumerable<string> assemblies, bool optional)
    {
        string str = Utility.CombinePath(this.PlayerPackage, "Managed");
        foreach (string str2 in assemblies)
        {
            string path = Utility.CombinePath(this.StagingAreaDataManaged, Path.GetFileName(str2));
            if (!optional || File.Exists(path))
            {
                CopyFileWithDebugSymbols(Utility.CombinePath(str, str2), path);
            }
        }
    }

    public virtual void OverwriteUnityAssemblies()
    {
        this.OverwriteAssemblies(this.GetUnityAssemblies(), false);
        this.OverwriteAssemblies(this.GetLangAssemblies(), true);
    }

    public virtual void Process()
    {
        this.ShowStep("Checking Requirements", Step.CheckRequirements);
        this.CheckWindows();
        this.CheckVisualStudio();
        this.CheckSDK();
        this.CheckSafeProjectOverwrite();
        this.UpdatePlayerSettings();
        this.CheckProfilerCapabilities();
        this.ShowStep("Copying Unity resources", Step.CopyUnityResources);
        this.CopyUnityResources();
        this.CopyImages();
        this.OverwriteUnityAssemblies();
        this.EnumerateAllManagedAssemblies();
        this.ShowStep("Copying plugins", Step.CopyPlugins);
        this.CopyPlugins();
        if (!this.UseIL2CPP())
        {
            this.ShowStep("Patching assemblies, stage one", Step.RunSerializationWeaver);
            this.RunSerializationWeaver();
            this.MoveDataManagedToRoot();
            this.ShowStep("Patching assemblies, stage two", Step.RunReferenceRewriter);
            this.RunReferenceRewriter();
            this.ShowStep("Patching assemblies, stage three", Step.RunAssemblyConverter);
            this.RunAssemblyConverter();
            this.DeleteMdbFiles();
        }
        else
        {
            this.RunIL2CPP();
        }
        this.CreateTestCertificate();
        this.CreateManifest();
        this.CreateCommandLineArgsPlaceholder();
        this.ShowStep("Copying streaming assets", Step.CopyStreamingAssets);
        this.CopyStreamingAssets();
        this.RemoveReadOnlyFileAttributes();
        this.CreateRegistryTxtFiles();
        this.CopyTestCertificate();
        this.ShowStep("Copying windows resource files", Step.CopyWindowsResourceFiles);
        this.CreateWindowsResourceFile();
        this.ShowStep("Copying template files", Step.CopyTemplateFiles);
        this.CopyTemplate();
        this.DeleteOldData();
        this.ShowStep("Creating Visual Studio solution", Step.CreatingVisualStudioSolution);
        this.CreateVisualStudioSolution();
        if (UserBuildSettings.copyReferences)
        {
            this.ShowStep("Copying player files", Step.CopyPlayerFiles);
            this.CopyPlayerFiles();
        }
        this.ShowExplorer();
    }

    public virtual void RemoveReadOnlyFileAttributes()
    {
        MetroVisualStudioSolutionHelper.RemoveReadOnlyAttributes(this.StagingArea);
    }

    public virtual void RunAssemblyConverter()
    {
        string str4;
        string fileName = Utility.CombinePath(this.PlayerPackage, @"Tools\AssemblyConverter.exe");
        string arguments = "-platform=" + this.GetAssemblyConverterPlatform();
        foreach (Library library in this.Libraries)
        {
            if (library.AnyCpu && library.Process)
            {
                string str3 = arguments;
                object[] objArray1 = new object[] { str3, " \"", Utility.CombinePath(this.StagingArea, library.ReferencePath), '"' };
                arguments = string.Concat(objArray1);
            }
        }
        if (Utility.RunAndWait(fileName, arguments, out str4, null) != 0)
        {
            throw new UnityException($"Failed to run assembly converter with command {arguments}.
{str4}");
        }
        if (!string.IsNullOrEmpty(str4))
        {
            UnityEngine.Debug.LogError($"Assembly converter: {str4}");
        }
    }

    protected virtual void RunAssemblyConverterNoMetadata(string assembly)
    {
    }

    protected virtual void RunIL2CPP()
    {
        throw new NotSupportedException();
    }

    public virtual void RunReferenceRewriter()
    {
        string fileName = Utility.CombinePath(this.PlayerPackage, @"Tools\rrw\rrw.exe");
        string str2 = JoinStrings(this.GetReferenceAssembliesDirectories());
        string platformAssemblyPath = this.GetPlatformAssemblyPath();
        string str4 = JoinStrings(this.GetAdditionalReferenceAssembliesDirectories());
        string str5 = Utility.CombinePath(this.StagingArea, "WinRTLegacy.dll");
        string str6 = this.MakeWinmdAssembliesString();
        string alternativeReferenceRewritterMappings = this.GetAlternativeReferenceRewritterMappings();
        string ignoredReferenceRewriterTypes = this.GetIgnoredReferenceRewriterTypes();
        IEnumerable<string> assembliesIgnoredByReferenceRewriter = this.GetAssembliesIgnoredByReferenceRewriter();
        IEnumerable<string> ignoreOutputAssembliesForReferenceRewriter = this.GetIgnoreOutputAssembliesForReferenceRewriter();
        using (IEnumerator<Library> enumerator = this.Libraries.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                <RunReferenceRewriter>c__AnonStorey6 storey = new <RunReferenceRewriter>c__AnonStorey6 {
                    library = enumerator.Current
                };
                if (storey.library.Process && !Enumerable.Any<string>(assembliesIgnoredByReferenceRewriter, new Func<string, bool>(storey.<>m__0)))
                {
                    foreach (string str9 in storey.library.Archs)
                    {
                        <RunReferenceRewriter>c__AnonStorey7 storey2 = new <RunReferenceRewriter>c__AnonStorey7();
                        string archReferencePath = storey.library.GetArchReferencePath(str9);
                        archReferencePath = Path.Combine(this.StagingArea, archReferencePath);
                        string arguments = $"--target="{archReferencePath}" --additionalreferences={str4} --platform="{platformAssemblyPath}" --support="{str5}" --supportpartialns=Unity.Partial --system=System --dbg=pdb";
                        if (!string.IsNullOrEmpty(str2))
                        {
                            arguments = arguments + " --framework=" + str2;
                        }
                        else
                        {
                            arguments = arguments + @" --lock=UWP\project.lock.json";
                        }
                        if (!string.IsNullOrEmpty(str6))
                        {
                            arguments = arguments + " --winmdrefs=" + str6;
                        }
                        if (!string.IsNullOrEmpty(alternativeReferenceRewritterMappings))
                        {
                            arguments = arguments + " --alt=" + alternativeReferenceRewritterMappings;
                        }
                        if (!string.IsNullOrEmpty(ignoredReferenceRewriterTypes))
                        {
                            arguments = arguments + " --ignore=" + ignoredReferenceRewriterTypes;
                        }
                        if (Utility.RunAndWait(fileName, arguments, out storey2.output, null) != 0)
                        {
                            throw new UnityException($"Failed to run reference rewriter with command {arguments}.
{storey2.output}");
                        }
                        if (string.IsNullOrEmpty(storey2.output))
                        {
                            this.RunAssemblyConverterNoMetadata(archReferencePath);
                        }
                        else if (!Enumerable.Any<string>(ignoreOutputAssembliesForReferenceRewriter, new Func<string, bool>(storey2.<>m__0)))
                        {
                            char[] separator = new char[] { '\r', '\n' };
                            string[] strArray = storey2.output.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            if (strArray.Length > 0)
                            {
                                UnityEngine.Debug.LogError($"Reference Rewriter found some errors while running with command {arguments}.
{storey2.output}");
                            }
                            foreach (string str12 in strArray)
                            {
                                UnityEngine.Debug.LogError($"Reference rewriter: {str12}");
                            }
                        }
                    }
                }
            }
        }
    }

    public virtual void RunSerializationWeaver()
    {
        string fileName = Utility.CombinePath(this.PlayerPackage, @"Tools\SerializationWeaver\SerializationWeaver.exe");
        string str2 = Utility.CombinePath(this.StagingAreaDataManaged, "UnityEngine.dll");
        string path = Utility.CombinePath(this.StagingArea, "TempSerializationWeaver");
        Utility.CreateDirectory(path);
        IEnumerable<string> assembliesIgnoredBySerializationWeaver = this.GetAssembliesIgnoredBySerializationWeaver();
        string referenceAssembliesDirectory = this.GetReferenceAssembliesDirectory();
        string[] strArray = this.GetAdditionalReferenceAssembliesDirectories().ToArray<string>();
        using (IEnumerator<Library> enumerator = this.Libraries.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                <RunSerializationWeaver>c__AnonStorey5 storey = new <RunSerializationWeaver>c__AnonStorey5 {
                    library = enumerator.Current
                };
                if (storey.library.Process && !Enumerable.Any<string>(assembliesIgnoredBySerializationWeaver, new Func<string, bool>(storey.<>m__0)))
                {
                    foreach (string str5 in storey.library.Archs)
                    {
                        string str9;
                        string archReferencePath = storey.library.GetArchReferencePath(str5);
                        archReferencePath = Path.Combine(this.StagingAreaDataManaged, archReferencePath);
                        string arguments = $""{archReferencePath}" -pdb -verbose -unity-engine="{str2}" "{path}"";
                        if (!string.IsNullOrEmpty(referenceAssembliesDirectory))
                        {
                            arguments = arguments + $" -additionalAssemblyPath="{referenceAssembliesDirectory}"";
                        }
                        else
                        {
                            arguments = arguments + @" -lock=UWP\project.lock.json";
                        }
                        foreach (string str8 in strArray)
                        {
                            arguments = arguments + $" -additionalAssemblyPath="{str8}"";
                        }
                        if (Utility.RunAndWait(fileName, arguments, out str9, null) != 0)
                        {
                            throw new UnityException($"Failed to run serialization weaver with command {arguments}.
{str9}");
                        }
                        DeletePlugin(archReferencePath);
                        string str10 = Path.ChangeExtension(archReferencePath, ".pdb");
                        File.Move(Utility.CombinePath(path, Path.GetFileName(archReferencePath)), archReferencePath);
                        string str12 = Utility.CombinePath(path, Path.GetFileName(str10));
                        if (File.Exists(str12))
                        {
                            File.Move(str12, str10);
                        }
                    }
                }
            }
        }
        Utility.DeleteDirectory(path);
    }

    public virtual void ShowExplorer()
    {
        if ((this.SourceBuild && !this.ThisIsABuildMachine) && (this.VisualStudioName.IndexOf("Tests", StringComparison.InvariantCultureIgnoreCase) == -1))
        {
            System.Diagnostics.Process.Start("explorer.exe", Path.GetFullPath(this.InstallPath));
        }
    }

    private void ShowStep(string message, Step step)
    {
        if (EditorUtility.DisplayCancelableProgressBar("Building Player", message, ((float) step) / 11f))
        {
            throw new OperationCanceledException();
        }
    }

    public virtual void UpdatePlayerSettings()
    {
        PlayerSettings.GetSerializedObject().Update();
    }

    protected virtual bool UseIL2CPP() => 
        false;

    protected bool AutoRunPlayer =>
        ((this.Options & BuildOptions.AutoRunPlayer) != BuildOptions.CompressTextures);

    protected string CompanyName =>
        this._args.companyName;

    protected bool Development =>
        ((this.Options & BuildOptions.Development) != BuildOptions.CompressTextures);

    protected BuildOptions Options =>
        this._args.options;

    protected Guid ProductGUID =>
        this._args.productGUID;

    protected string ProductName =>
        this._args.productName;

    protected bool SourceBuild =>
        ((this.Options & BuildOptions.InstallInBuildFolder) != BuildOptions.CompressTextures);

    protected BuildTarget Target =>
        this._args.target;

    protected bool ThisIsABuildMachine =>
        (Environment.GetEnvironmentVariable("UNITY_THISISABUILDMACHINE") == "1");

    [CompilerGenerated]
    private sealed class <CopyPlayerFiles>c__AnonStorey9
    {
        internal string[] extensions;

        internal bool <>m__0(string f)
        {
            <CopyPlayerFiles>c__AnonStoreyA ya = new <CopyPlayerFiles>c__AnonStoreyA {
                <>f__ref$9 = this,
                f = f
            };
            return Enumerable.Any<string>(this.extensions, new Func<string, bool>(ya.<>m__0));
        }

        private sealed class <CopyPlayerFiles>c__AnonStoreyA
        {
            internal PostProcessWinRT.<CopyPlayerFiles>c__AnonStorey9 <>f__ref$9;
            internal string f;

            internal bool <>m__0(string e) => 
                this.f.EndsWith(e, StringComparison.InvariantCultureIgnoreCase);
        }
    }

    [CompilerGenerated]
    private sealed class <CopyPlugins>c__AnonStorey3
    {
        internal string pluginName;

        internal bool <>m__0(string p) => 
            string.Equals(p, this.pluginName, StringComparison.InvariantCultureIgnoreCase);
    }

    [CompilerGenerated]
    private sealed class <CopyPlugins>c__AnonStorey4
    {
        internal string extension;

        internal bool <>m__0(string e) => 
            string.Equals(this.extension, e, StringComparison.InvariantCultureIgnoreCase);
    }

    [CompilerGenerated]
    private sealed class <CreateManagedRegistryTxtFile>c__AnonStorey8
    {
        internal Library library;

        internal bool <>m__0(string e) => 
            string.Equals(this.library.Name, e, StringComparison.InvariantCultureIgnoreCase);
    }

    [CompilerGenerated]
    private sealed class <GetAdditionalReferenceAssembliesDirectories>c__Iterator1 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
    {
        internal string $current;
        internal bool $disposing;
        internal IEnumerator<Library> $locvar0;
        internal IEnumerator<string> $locvar1;
        internal int $PC;
        internal PostProcessWinRT $this;
        internal string <arch>__1;
        internal string <directory>__3;
        internal Library <library>__0;
        internal string <path>__2;

        [DebuggerHidden]
        public void Dispose()
        {
            uint num = (uint) this.$PC;
            this.$disposing = true;
            this.$PC = -1;
            switch (num)
            {
                case 2:
                    try
                    {
                        try
                        {
                        }
                        finally
                        {
                            if (this.$locvar1 != null)
                            {
                                this.$locvar1.Dispose();
                            }
                        }
                    }
                    finally
                    {
                        if (this.$locvar0 != null)
                        {
                            this.$locvar0.Dispose();
                        }
                    }
                    break;
            }
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            bool flag = false;
            switch (num)
            {
                case 0:
                    this.$current = this.$this.m_ManagedAssemblyLocation;
                    if (!this.$disposing)
                    {
                        this.$PC = 1;
                    }
                    goto Label_01A1;

                case 1:
                    this.$locvar0 = this.$this.Libraries.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 2:
                    break;

                default:
                    goto Label_019F;
            }
            try
            {
                switch (num)
                {
                    case 2:
                        goto Label_00A4;
                }
                while (this.$locvar0.MoveNext())
                {
                    this.<library>__0 = this.$locvar0.Current;
                    this.$locvar1 = this.<library>__0.Archs.GetEnumerator();
                    num = 0xfffffffd;
                Label_00A4:
                    try
                    {
                        while (this.$locvar1.MoveNext())
                        {
                            this.<arch>__1 = this.$locvar1.Current;
                            this.<path>__2 = this.<library>__0.GetArchReferencePath(this.<arch>__1);
                            this.<directory>__3 = Path.GetDirectoryName(this.<path>__2);
                            if (!string.IsNullOrEmpty(this.<directory>__3))
                            {
                                this.$current = Utility.CombinePath(this.$this.m_ManagedAssemblyLocation, this.<directory>__3);
                                if (!this.$disposing)
                                {
                                    this.$PC = 2;
                                }
                                flag = true;
                                goto Label_01A1;
                            }
                        }
                    }
                    finally
                    {
                        if (!flag)
                        {
                        }
                        if (this.$locvar1 != null)
                        {
                            this.$locvar1.Dispose();
                        }
                    }
                }
            }
            finally
            {
                if (!flag)
                {
                }
                if (this.$locvar0 != null)
                {
                    this.$locvar0.Dispose();
                }
            }
            this.$PC = -1;
        Label_019F:
            return false;
        Label_01A1:
            return true;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new PostProcessWinRT.<GetAdditionalReferenceAssembliesDirectories>c__Iterator1 { $this = this.$this };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator() => 
            this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();

        string IEnumerator<string>.Current =>
            this.$current;

        object IEnumerator.Current =>
            this.$current;
    }

    [CompilerGenerated]
    private sealed class <GetIgnoreOutputAssembliesForReferenceRewriter>c__Iterator2 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
    {
        internal string $current;
        internal bool $disposing;
        internal int $PC;

        [DebuggerHidden]
        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            this.$PC = -1;
            if (this.$PC == 0)
            {
            }
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new PostProcessWinRT.<GetIgnoreOutputAssembliesForReferenceRewriter>c__Iterator2();
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator() => 
            this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();

        string IEnumerator<string>.Current =>
            this.$current;

        object IEnumerator.Current =>
            this.$current;
    }

    [CompilerGenerated]
    private sealed class <GetReferenceAssembliesDirectories>c__Iterator0 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
    {
        internal string $current;
        internal bool $disposing;
        internal int $PC;
        internal PostProcessWinRT $this;
        internal string <path>__0;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$disposing = true;
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.<path>__0 = this.$this.GetReferenceAssembliesDirectory();
                    if (string.IsNullOrEmpty(this.<path>__0))
                    {
                        break;
                    }
                    this.$current = this.<path>__0;
                    if (!this.$disposing)
                    {
                        this.$PC = 1;
                    }
                    return true;

                case 1:
                    break;

                default:
                    goto Label_006A;
            }
            this.$PC = -1;
        Label_006A:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new PostProcessWinRT.<GetReferenceAssembliesDirectories>c__Iterator0 { $this = this.$this };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator() => 
            this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();

        string IEnumerator<string>.Current =>
            this.$current;

        object IEnumerator.Current =>
            this.$current;
    }

    [CompilerGenerated]
    private sealed class <RunReferenceRewriter>c__AnonStorey6
    {
        internal Library library;

        internal bool <>m__0(string a) => 
            string.Equals(a, this.library.Name, StringComparison.InvariantCultureIgnoreCase);
    }

    [CompilerGenerated]
    private sealed class <RunReferenceRewriter>c__AnonStorey7
    {
        internal string output;

        internal bool <>m__0(string a) => 
            (this.output.IndexOf(a, StringComparison.InvariantCultureIgnoreCase) != -1);
    }

    [CompilerGenerated]
    private sealed class <RunSerializationWeaver>c__AnonStorey5
    {
        internal Library library;

        internal bool <>m__0(string a) => 
            string.Equals(a, this.library.Name, StringComparison.InvariantCultureIgnoreCase);
    }

    private enum Step
    {
        CheckRequirements,
        CopyUnityResources,
        CopyPlugins,
        RunSerializationWeaver,
        RunReferenceRewriter,
        RunAssemblyConverter,
        CopyStreamingAssets,
        CopyWindowsResourceFiles,
        CopyTemplateFiles,
        CreatingVisualStudioSolution,
        CopyPlayerFiles,
        Count
    }
}

