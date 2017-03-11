using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Modules;
using UnityEditor.Scripting.Compilers;
using UnityEditor.Utils;
using UnityEditor.WSA;
using UnityEngine;

internal abstract class PostProcessWSA : PostProcessWinRT
{
    private static Dictionary<PlayerSettings.SplashScreen.UnityLogoStyle, string> _defaultSplashScreens;
    protected readonly bool _generateReferenceProjects;
    protected readonly ProjectImages _images;

    public PostProcessWSA(BuildPostProcessArgs args, WSASDK sdk, string stagingArea = null) : base(args, sdk, stagingArea)
    {
        this._generateReferenceProjects = EditorUserBuildSettings.wsaGenerateReferenceProjects;
        this._images = new ProjectImages();
        if (this._generateReferenceProjects)
        {
            MetroAssemblyCSharpCreator.ResetAssemblyCSharpProjectGUIDs();
        }
    }

    protected string CheckImageConsistencyAndGetName(params string[] strings)
    {
        string str = null;
        string a = null;
        foreach (string str3 in strings)
        {
            if (!string.IsNullOrEmpty(str3))
            {
                if (str == null)
                {
                    str = str3;
                    a = Path.GetExtension(str3);
                }
                else if (!string.Equals(a, Path.GetExtension(str3), StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new Exception($"Inconsistency between images in the same group ({str}). All images should be either PNG or JPEG!");
                }
            }
        }
        return str;
    }

    public override void CheckProfilerCapabilities()
    {
        if (base.AutoRunPlayer && base.Development)
        {
            bool capability = PlayerSettings.WSA.GetCapability(PlayerSettings.WSACapability.InternetClientServer);
            bool flag2 = PlayerSettings.WSA.GetCapability(PlayerSettings.WSACapability.PrivateNetworkClientServer);
            if (!capability || !flag2)
            {
                Debug.LogWarning("Warning: profiler will not be functional in this build due to missing capabilities. If you require profiler functionality, please enable InternetClientServer and PrivateNetworkClientServer capabilities in the player settings");
            }
        }
    }

    public override void CheckSafeProjectOverwrite()
    {
        string path = Utility.CombinePath(base.InstallPath, base.VisualStudioName);
        if (Directory.Exists(path))
        {
            string str2 = Utility.CombinePath(path, base.VisualStudioName + ".csproj");
            string str3 = Utility.CombinePath(path, base.VisualStudioName + ".vcxproj");
            string str4 = Utility.CombinePath(path, base.VisualStudioName + ".Shared");
            if (base._sdk != WSASDK.UniversalSDK81)
            {
                if (Directory.Exists(str4))
                {
                    throw new UnityException("Build path contains Universal project which is incompatible with current one.");
                }
            }
            else
            {
                if (!Directory.Exists(str4))
                {
                    if (File.Exists(str2))
                    {
                        throw new UnityException("Build path contains project which is incompatible with current one.");
                    }
                    return;
                }
                path = str4;
                str2 = Utility.CombinePath(path, base.VisualStudioName + ".shproj");
            }
            if (this.UseIL2CPP())
            {
                if (File.Exists(str2))
                {
                    throw new UnityException("Build path contains project built with .NET scripting backend, while current project is using IL2CPP scripting backend.");
                }
            }
            else if (File.Exists(str3))
            {
                throw new UnityException("Build path contains project built with IL2CPP scripting backend, while current project is using .NET scripting backend.");
            }
            if (File.Exists(str2))
            {
                Version projectFileToolsVersion = GetProjectFileToolsVersion(str2);
                Version toolsVersion = this.GetToolsVersion();
                if (projectFileToolsVersion != toolsVersion)
                {
                    throw new UnityException($"Build path contains project which is incompatible with current one. Expected tools version '{toolsVersion}', was '{projectFileToolsVersion}'");
                }
            }
        }
    }

    protected string CopyImage(PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale scale, string defaultSource, string destinationFileName)
    {
        string visualAssetsImage = PlayerSettings.WSA.GetVisualAssetsImage(type, scale);
        if (string.IsNullOrEmpty(visualAssetsImage))
        {
            if (string.IsNullOrEmpty(defaultSource))
            {
                return null;
            }
            visualAssetsImage = defaultSource;
        }
        destinationFileName = Path.ChangeExtension(destinationFileName, Path.GetExtension(visualAssetsImage));
        string str3 = Path.ChangeExtension(Utility.CombinePath("Assets", Path.GetFileName(!string.IsNullOrEmpty(defaultSource) ? defaultSource : visualAssetsImage)), Path.GetExtension(visualAssetsImage));
        if (string.IsNullOrEmpty(destinationFileName))
        {
            destinationFileName = str3;
        }
        else
        {
            destinationFileName = Utility.CombinePath("Assets", Path.GetFileName(destinationFileName));
        }
        string path = Utility.CombinePath(base.StagingArea, destinationFileName);
        Utility.CreateDirectory(Path.GetDirectoryName(path));
        FileUtil.CopyFileOrDirectory(visualAssetsImage, path);
        return (destinationFileName.Substring(0, destinationFileName.IndexOf('.')) + Path.GetExtension(visualAssetsImage));
    }

    public override void CopyTemplate()
    {
        string templateDirectorySource = this.GetTemplateDirectorySource();
        string templateDirectoryTarget = this.GetTemplateDirectoryTarget();
        FileUtil.CopyDirectoryRecursive(templateDirectorySource, templateDirectoryTarget);
    }

    public override void CopyTestCertificate()
    {
        string certificatePath = PlayerSettings.WSA.certificatePath;
        string destFileName = Utility.CombinePath(base.StagingArea, Path.GetFileName(certificatePath));
        File.Copy(certificatePath, destFileName);
    }

    private void CopyUnityTools()
    {
        string[] strArray = new string[] { "SerializationWeaver/Mono.Cecil.dll", "SerializationWeaver/Mono.Cecil.Mdb.dll", "SerializationWeaver/Mono.Cecil.Pdb.dll", "SerializationWeaver/Mono.Cecil.Rocks.dll", "SerializationWeaver/SerializationWeaver.exe", "SerializationWeaver/Unity.CecilTools.dll", "SerializationWeaver/Unity.SerializationLogic.dll", "SerializationWeaver/Unity.SerializationWeaver.Common.dll", "SerializationWeaver/Unity.SerializationWeaver.dll", "SerializationWeaver/Unity.UNetWeaver.dll", "AssemblyConverter.exe", "Mono.Cecil.dll", "Mono.Cecil.Mdb.dll", "Mono.Cecil.Pdb.dll" };
        string str = Path.Combine(base.InstallPath, "Unity");
        string[] components = new string[] { str, "Tools", "SerializationWeaver" };
        string path = Paths.Combine(components);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        foreach (string str3 in strArray)
        {
            string[] textArray3 = new string[] { base.PlayerPackage, "Tools", str3 };
            string sourceFileName = Paths.Combine(textArray3);
            string[] textArray4 = new string[] { base.InstallPath, "Unity", "Tools", str3 };
            string destFileName = Paths.Combine(textArray4);
            File.Copy(sourceFileName, destFileName, true);
            File.SetAttributes(destFileName, FileAttributes.Normal);
        }
    }

    public override void CreateCommandLineArgsPlaceholder()
    {
        string commandLineArgsFile = PlayerSettings.WSA.commandLineArgsFile;
        if (!string.IsNullOrEmpty(commandLineArgsFile))
        {
            File.WriteAllText(Utility.CombinePath(base.StagingAreaData, commandLineArgsFile), string.Empty, Encoding.UTF8);
        }
    }

    public override void CreateManifest()
    {
        ManifestWSA twsa = this.CreateManifestBuilder();
        string path = Utility.CombinePath(base.StagingArea, "Package.appxmanifest");
        twsa.Create(path, this._images);
    }

    protected abstract ManifestWSA CreateManifestBuilder();
    public override void CreateTestCertificate()
    {
        string certificatePath = PlayerSettings.WSA.certificatePath;
        if (string.IsNullOrEmpty(certificatePath) || !File.Exists(certificatePath))
        {
            if (string.IsNullOrEmpty(certificatePath))
            {
                certificatePath = Utility.CombinePath(Application.dataPath, "WSATestCertificate.pfx");
            }
            if (!File.Exists(certificatePath))
            {
                if (!EditorUtility.WSACreateTestCertificate(certificatePath, base.CompanyName, null, false))
                {
                    throw new UnityException("Failed to create test certificate.");
                }
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
            PlayerSettings.WSA.SetCertificate(certificatePath, null);
        }
    }

    public override void CreateVisualStudioSolution()
    {
        string path = Utility.CombinePath(base.InstallPath, base.VisualStudioName + "v11.suo");
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        Dictionary<WSASDK, LibraryCollection> dictionary = this.TEMP_GetLibraryCollections();
        MetroCSharpVisualStudioSolutionCreator creator = new MetroCSharpVisualStudioSolutionCreator(base.VisualStudioName, base.StagingArea, base.InstallPath, base.PlayerPackage) {
            SourceBuild = base.SourceBuild,
            DontOverwriteFiles = MetroVisualStudioSolutionHelper.GetDontOverwriteFilesCSharp(),
            LibraryCollections = dictionary
        };
        if (base.AutoRunPlayer)
        {
            IEnumerator enumerator = Enum.GetValues(typeof(WSABuildType)).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    creator.EnableDotNetNative[(WSABuildType) current] = false;
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }
        else
        {
            IEnumerator enumerator2 = Enum.GetValues(typeof(WSABuildType)).GetEnumerator();
            try
            {
                while (enumerator2.MoveNext())
                {
                    object obj3 = enumerator2.Current;
                    creator.EnableDotNetNative[(WSABuildType) obj3] = EditorUserBuildSettings.GetWSADotNetNative((WSABuildType) obj3);
                }
            }
            finally
            {
                IDisposable disposable2 = enumerator2 as IDisposable;
                if (disposable2 != null)
                {
                    disposable2.Dispose();
                }
            }
        }
        creator.CreateSolutionFileFrom();
        MetroVisualStudioSolutionHelper.WriteUnityCommonProps(Path.Combine(base.InstallPath, "UnityCommon.props"), base.PlayerPackage, base.InstallPath, base.SourceBuild, EditorUserBuildSettings.wsaGenerateReferenceProjects);
        if (!base.SourceBuild)
        {
            this.CopyUnityTools();
        }
    }

    protected override string GetAlternativeReferenceRewritterMappings() => 
        "System.Xml.Serialization";

    protected override IEnumerable<string> GetAssembliesIgnoredBySerializationWeaver()
    {
        IEnumerable<string> assembliesIgnoredBySerializationWeaver = base.GetAssembliesIgnoredBySerializationWeaver();
        if (this._generateReferenceProjects)
        {
            string[] second = new string[] { Utility.AssemblyCSharpName + ".dll", Utility.AssemblyCSharpFirstPassName + ".dll" };
            assembliesIgnoredBySerializationWeaver = assembliesIgnoredBySerializationWeaver.Union<string>(second);
        }
        return assembliesIgnoredBySerializationWeaver;
    }

    protected string GetDefaultSplashScreenImage()
    {
        string str;
        IDictionary<PlayerSettings.SplashScreen.UnityLogoStyle, string> defaultSplashScreens = this.GetDefaultSplashScreens();
        if ((defaultSplashScreens == null) || (defaultSplashScreens.Count == 0))
        {
            throw new Exception("No splash screens are available!");
        }
        if (!defaultSplashScreens.TryGetValue(PlayerSettings.SplashScreen.unityLogoStyle, out str))
        {
            Debug.LogWarning($"No splash screen for {PlayerSettings.SplashScreen.unityLogoStyle} available");
            str = defaultSplashScreens.First<KeyValuePair<PlayerSettings.SplashScreen.UnityLogoStyle, string>>().Value;
        }
        return str;
    }

    protected virtual IDictionary<PlayerSettings.SplashScreen.UnityLogoStyle, string> GetDefaultSplashScreens()
    {
        if (_defaultSplashScreens == null)
        {
            _defaultSplashScreens = new Dictionary<PlayerSettings.SplashScreen.UnityLogoStyle, string>();
            _defaultSplashScreens[PlayerSettings.SplashScreen.UnityLogoStyle.DarkOnLight] = "SplashScreen.png";
            _defaultSplashScreens[PlayerSettings.SplashScreen.UnityLogoStyle.LightOnDark] = "LightSplashScreen.png";
        }
        return _defaultSplashScreens;
    }

    protected override string GetIgnoredReferenceRewriterTypes() => 
        "System.IConvertible,mscorlib";

    protected override IEnumerable<string> GetIgnoreOutputAssembliesForReferenceRewriter()
    {
        IEnumerable<string> ignoreOutputAssembliesForReferenceRewriter = base.GetIgnoreOutputAssembliesForReferenceRewriter();
        if (base.ThisIsABuildMachine)
        {
            string[] second = new string[] { Utility.AssemblyCSharpName + ".dll", Utility.AssemblyCSharpFirstPassName + ".dll", Utility.AssemblyUnityScriptName + ".dll", Utility.AssemblyUnityScriptFirstPassName + ".dll" };
            ignoreOutputAssembliesForReferenceRewriter = ignoreOutputAssembliesForReferenceRewriter.Union<string>(second);
        }
        return ignoreOutputAssembliesForReferenceRewriter;
    }

    protected override string GetPlatformAssemblyPath() => 
        MetroCompilationExtension.GetWindowsWinmdPath(base._sdk);

    protected string GetPlayerFilesSourceDirectory(string path)
    {
        string[] paths = new string[] { base.PlayerPackage, "Players", path };
        return Utility.CombinePath(paths);
    }

    protected string GetPlayerFilesTargetDirectory(string path)
    {
        string[] paths = new string[] { base.InstallPath, "Players", path };
        return Utility.CombinePath(paths);
    }

    private static Version GetProjectFileToolsVersion(string path)
    {
        Version version;
        try
        {
            XElement element = XDocument.Load(path).Element("{http://schemas.microsoft.com/developer/msbuild/2003}Project");
            if (element == null)
            {
                return new Version(0, 0);
            }
            version = new Version(element.Attribute("ToolsVersion").Value);
        }
        catch (Exception exception)
        {
            throw new Exception($"Failed to extract ToolsVersion while reading '{path}'", exception);
        }
        return version;
    }

    protected static string GetReferenceAssembliesDirectory(WSASDK sdk) => 
        PostProcessWinRT.GetReferenceAssembliesDirectory(MicrosoftCSharpCompiler.GetNETCoreFrameworkReferencesDirectory(sdk));

    protected abstract string GetTemplateDirectorySource();
    protected string GetTemplateDirectorySource(string directory)
    {
        string[] paths = new string[] { base.PlayerPackage, "Templates", directory, "CSharp" };
        return Utility.CombinePath(paths);
    }

    protected virtual string GetTemplateDirectoryTarget() => 
        base.StagingArea;

    protected abstract Version GetToolsVersion();
    protected virtual Dictionary<WSASDK, LibraryCollection> TEMP_GetLibraryCollections() => 
        new Dictionary<WSASDK, LibraryCollection>(1) { { 
            base._sdk,
            base.Libraries
        } };
}

