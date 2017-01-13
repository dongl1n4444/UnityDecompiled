using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEditor.WSA;
using UnityEditorInternal;
using UnityEngine;

internal static class MetroVisualStudioSolutionHelper
{
    private static Dictionary<WSASDK, string> _assemblyConverterPlatform;
    [CompilerGenerated]
    private static Dictionary<string, int> <>f__switch$map0;
    private static readonly string[] DontOverwriteFilesCommon;
    private static readonly string[] DontOverwriteFilesCpp;
    private static readonly string[] DontOverwriteFilesCSharp;

    static MetroVisualStudioSolutionHelper()
    {
        Dictionary<WSASDK, string> dictionary = new Dictionary<WSASDK, string> {
            { 
                WSASDK.SDK80,
                "wsa80"
            },
            { 
                WSASDK.SDK81,
                "wsa81"
            },
            { 
                WSASDK.PhoneSDK81,
                "wp81"
            },
            { 
                WSASDK.UWP,
                "uap"
            }
        };
        _assemblyConverterPlatform = dictionary;
        DontOverwriteFilesCommon = new string[] { 
            "App.xaml", "MainPage.xaml", "Package.appxmanifest", "project.json", @"Assets\MediumTile.scale-80", @"Assets\MediumTile.scale-100", @"Assets\MediumTile.scale-140", @"Assets\MediumTile.scale-180", @"Assets\MediumTile.scale-240", @"Assets\SmallTile", @"Assets\StoreLogo", @"Assets\StoreLogo.scale-100", @"Assets\StoreLogo.scale-125", @"Assets\StoreLogo.scale-140", @"Assets\StoreLogo.scale-150", @"Assets\StoreLogo.scale-180",
            @"Assets\StoreLogo.scale-200", @"Assets\StoreLogo.scale-240", @"Assets\StoreLogo.scale-400", @"Assets\SmallLogo.scale-80", @"Assets\SmallLogo.scale-100", @"Assets\SmallLogo.scale-140", @"Assets\SmallLogo.scale-180", @"Assets\WideLogo.scale-80", @"Assets\WideLogo.scale-100", @"Assets\WideLogo.scale-140", @"Assets\WideLogo.scale-180", @"Assets\SmallLogo.scale-80", @"Assets\SmallLogo.scale-100", @"Assets\SmallLogo.scale-140", @"Assets\SmallLogo.scale-180", @"Assets\LargeLogo.scale-80",
            @"Assets\LargeLogo.scale-100", @"Assets\LargeLogo.scale-140", @"Assets\LargeLogo.scale-180", @"Assets\SplashScreen", @"Assets\SplashScreen.scale-100", @"Assets\SplashScreen.scale-125", @"Assets\SplashScreen.scale-140", @"Assets\SplashScreen.scale-150", @"Assets\SplashScreen.scale-180", @"Assets\SplashScreen.scale-200", @"Assets\SplashScreen.scale-240", @"Assets\SplashScreen.scale-400", @"Assets\AppIcon.scale-100", @"Assets\AppIcon.scale-140", @"Assets\AppIcon.scale-240", @"Assets\SmallTile.scale-100",
            @"Assets\SmallTile.scale-140", @"Assets\SmallTile.scale-240", @"Assets\LargeTile.scale-100", @"Assets\LargeTile.scale-140", @"Assets\LargeTile.scale-240", @"Assets\WideTile.scale-100", @"Assets\WideTile.scale-140", @"Assets\WideTile.scale-240", @"Assets\Square44x44Logo.png", @"Assets\Square44x44Logo.scale-100.png", @"Assets\Square44x44Logo.scale-125.png", @"Assets\Square44x44Logo.scale-150.png", @"Assets\Square44x44Logo.scale-200.png", @"Assets\Square44x44Logo.scale-400.png", @"Assets\Square44x44Logo.targetsize-16_altform-unplated.png", @"Assets\Square44x44Logo.targetsize-24_altform-unplated.png",
            @"Assets\Square44x44Logo.targetsize-48_altform-unplated.png", @"Assets\Square44x44Logo.targetsize-256_altform-unplated.png", @"Assets\Square71x71Logo.png", @"Assets\Square71x71Logo.scale-100.png", @"Assets\Square71x71Logo.scale-125.png", @"Assets\Square71x71Logo.scale-150.png", @"Assets\Square71x71Logo.scale-200.png", @"Assets\Square71x71Logo.scale-400.png", @"Assets\Square150x150Logo.png", @"Assets\Square150x150Logo.scale-100.png", @"Assets\Square150x150Logo.scale-125.png", @"Assets\Square150x150Logo.scale-150.png", @"Assets\Square150x150Logo.scale-200.png", @"Assets\Square150x150Logo.scale-400.png", @"Assets\Square310x310Logo.png", @"Assets\Square310x310Logo.scale-100.png",
            @"Assets\Square310x310Logo.scale-125.png", @"Assets\Square310x310Logo.scale-150.png", @"Assets\Square310x310Logo.scale-200.png", @"Assets\Square310x310Logo.scale-400.png", @"Assets\Wide310x150Logo.png", @"Assets\Wide310x150Logo.scale-100.png", @"Assets\Wide310x150Logo.scale-125.png", @"Assets\Wide310x150Logo.scale-150.png", @"Assets\Wide310x150Logo.scale-200.png", @"Assets\Wide310x150Logo.scale-400.png"
        };
        DontOverwriteFilesCSharp = new string[] { "App.cs", "App.xaml.cs", "MainPage.xaml.cs", @"Properties\AssemblyInfo.cs", @"Properties\Default.rd.xml" };
        DontOverwriteFilesCpp = new string[] { "App.h", "App.cpp", "Main.cpp", "App.xaml.h", "App.xaml.cpp", "MainPage.xaml.h", "MainPage.xaml.cpp", "pch.h", "pch.cpp" };
    }

    internal static void AddAssemblyConverterCommands(string playerPackage, bool sourceBuild, Dictionary<WSASDK, TemplateBuilder> templateBuilders, Dictionary<WSASDK, LibraryCollection> libraryCollections, bool useCSharpProjects = false, Dictionary<WSASDK, string> assemblyCSharpDllPaths = null, Dictionary<WSASDK, string> assemblyCSharpFirstpassDllPaths = null)
    {
        foreach (KeyValuePair<WSASDK, TemplateBuilder> pair in templateBuilders)
        {
            WSASDK key = pair.Key;
            foreach (KeyValuePair<WSASDK, LibraryCollection> pair2 in libraryCollections)
            {
                if (((WSASDK) pair2.Key) == key)
                {
                    string str = null;
                    string str2 = null;
                    if (assemblyCSharpDllPaths != null)
                    {
                        assemblyCSharpDllPaths.TryGetValue(key, out str);
                    }
                    if (assemblyCSharpFirstpassDllPaths != null)
                    {
                        assemblyCSharpFirstpassDllPaths.TryGetValue(key, out str2);
                    }
                    List<string> assemblies = new List<string>();
                    foreach (Library library in pair2.Value)
                    {
                        if (library.Process && (!useCSharpProjects || (!string.Equals(library.Name, Utility.AssemblyCSharpName, StringComparison.InvariantCultureIgnoreCase) && !string.Equals(library.Name, Utility.AssemblyCSharpFirstPassName, StringComparison.InvariantCultureIgnoreCase))))
                        {
                            assemblies.Add(library.Reference);
                        }
                    }
                    AssemblyConverterCommandAdder.AddCommands(assemblies, key, sourceBuild, pair.Value, useCSharpProjects, str, str2);
                }
            }
        }
    }

    internal static void AddPluginPreBuildEvents(Dictionary<WSASDK, TemplateBuilder> templateBuilders, Dictionary<WSASDK, LibraryCollection> libraryCollections)
    {
        foreach (KeyValuePair<WSASDK, TemplateBuilder> pair in templateBuilders)
        {
            foreach (KeyValuePair<WSASDK, LibraryCollection> pair2 in libraryCollections)
            {
                if (pair2.Key == pair.Key)
                {
                    AddPluginPreBuildEvents(pair.Value, pair2.Value);
                }
            }
        }
    }

    internal static void AddPluginPreBuildEvents(TemplateBuilder templateBuilder, LibraryCollection libraryCollection)
    {
        foreach (Library library in libraryCollection)
        {
            string referencePath = library.ReferencePath;
            if (referencePath.StartsWith(@"Plugins\", StringComparison.InvariantCultureIgnoreCase))
            {
                referencePath = "$(ProjectDir)" + referencePath;
                string destinationFile = $"$(ProjectDir){library.Reference}";
                AddPrebuildCopyEvent(templateBuilder, referencePath, destinationFile, library.WinMd && library.Native);
            }
        }
    }

    private static void AddPrebuildCopyEvent(TemplateBuilder templateBuilder, string sourceFile, string destinationFile, bool isWinmdCpp)
    {
        StringBuilder beforeResolveReferences = templateBuilder.BeforeResolveReferences;
        if (isWinmdCpp)
        {
            sourceFile = Path.ChangeExtension(sourceFile, ".dll");
            destinationFile = Path.ChangeExtension(destinationFile, ".dll");
        }
        string str = $"<Copy SourceFiles="{sourceFile}" DestinationFiles="{destinationFile}" />";
        if (!beforeResolveReferences.ToString().Contains(str))
        {
            beforeResolveReferences.AppendLineWithPrefix(str, new object[0]);
            string str2 = Path.ChangeExtension(sourceFile, ".pdb");
            string str3 = Path.ChangeExtension(destinationFile, ".pdb");
            string str4 = Path.ChangeExtension(destinationFile, ".dll.mdb");
            object[] args = new object[] { str3 };
            beforeResolveReferences.AppendLineWithPrefix("    <Delete Files=\"{0}\" Condition=\"Exists('{0}')\" />", args);
            object[] objArray2 = new object[] { str4 };
            beforeResolveReferences.AppendLineWithPrefix("    <Delete Files=\"{0}\" Condition=\"Exists('{0}')\" />", objArray2);
            object[] objArray3 = new object[] { str2, str3 };
            beforeResolveReferences.AppendLineWithPrefix("    <Copy SourceFiles=\"{0}\" DestinationFiles=\"{1}\" Condition=\"Exists('{0}')\" />", objArray3);
        }
    }

    public static string CalculateMD5ForFile(string file)
    {
        using (FileStream stream = File.Open(file, System.IO.FileMode.Open))
        {
            byte[] buffer = MD5.Create().ComputeHash(stream);
            StringBuilder builder = new StringBuilder();
            foreach (byte num in buffer)
            {
                builder.Append(num.ToString("X2"));
            }
            return builder.ToString();
        }
    }

    public static OverwriteFilesInfo CheckOverwriteFiles(string targetDirectory, string projectDir, bool targetDirEmpty)
    {
        OverwriteFilesInfo info = new OverwriteFilesInfo();
        if (!targetDirEmpty)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string path = Path.Combine(targetDirectory, "UnityOverwrite.txt");
            if (!File.Exists(path))
            {
                info.KeepAll = true;
                return info;
            }
            foreach (string str2 in File.ReadAllLines(path))
            {
                if ((str2.Trim() != "") && !str2.StartsWith("#"))
                {
                    if (str2.Equals("overwrite-all", StringComparison.InvariantCultureIgnoreCase))
                    {
                        info.OverwriteAll = true;
                        return info;
                    }
                    if (str2.Equals("keep-all", StringComparison.InvariantCultureIgnoreCase))
                    {
                        info.KeepAll = true;
                        return info;
                    }
                    int index = str2.IndexOf(':');
                    if ((index >= 0) && (str2.Length != (index + 1)))
                    {
                        string key = str2.Substring(0, index);
                        string str4 = str2.Substring(index + 1).Trim();
                        if (str4 != "")
                        {
                            dictionary.Add(key, str4);
                        }
                    }
                }
            }
            string prefix = FileUtil.RemovePathPrefix(projectDir, targetDirectory);
            foreach (KeyValuePair<string, string> pair in dictionary)
            {
                if (pair.Value.Equals("overwrite", StringComparison.InvariantCultureIgnoreCase))
                {
                    info.DoOverwrite.Add(FileUtil.RemovePathPrefix(pair.Key, prefix), true);
                }
                else
                {
                    string str6 = Path.Combine(targetDirectory, pair.Key);
                    if (File.Exists(str6))
                    {
                        string str7 = CalculateMD5ForFile(str6);
                        if (str7.Equals(pair.Value, StringComparison.InvariantCultureIgnoreCase))
                        {
                            info.DoOverwrite.Add(pair.Key, false);
                        }
                        else
                        {
                            info.UserModified.Add(FileUtil.RemovePathPrefix(pair.Key, targetDirectory));
                        }
                        info.Hashes.Add(pair.Key, str7);
                    }
                }
            }
        }
        return info;
    }

    internal static string ColorToXAMLAttribute(Color32 c)
    {
        if (c.a == 0)
        {
            return "transparent";
        }
        object[] args = new object[] { c.r, c.g, c.b };
        return string.Format(CultureInfo.InvariantCulture, "#{0:X2}{1:X2}{2:X2}", args);
    }

    private static string[] ConcatStringArrays(string[] left, string[] right)
    {
        string[] strArray = new string[left.Length + right.Length];
        for (int i = 0; i < left.Length; i++)
        {
            strArray[i] = left[i];
        }
        for (int j = 0; j < right.Length; j++)
        {
            strArray[j + left.Length] = right[j];
        }
        return strArray;
    }

    public static void CopyAssembliesToUnprocessed(IEnumerable<string> unprocessedDllFiles, string targetProjectDirectory)
    {
        foreach (string str in unprocessedDllFiles)
        {
            string path = Path.Combine(targetProjectDirectory, str);
            string str3 = Path.Combine(Path.GetDirectoryName(path), "Unprocessed");
            Directory.CreateDirectory(str3);
            string str4 = Path.Combine(str3, Path.GetFileName(str));
            if (File.Exists(str4))
            {
                File.Delete(str4);
            }
            FileUtil.MoveFileOrDirectory(path, str4);
            string str5 = Path.ChangeExtension(str, ".pdb");
            string str6 = Path.Combine(targetProjectDirectory, str5);
            if (File.Exists(str6))
            {
                string str7 = Path.Combine(str3, Path.GetFileName(str5));
                if (File.Exists(str7))
                {
                    File.Delete(str7);
                }
                FileUtil.MoveFileOrDirectory(str6, str7);
            }
        }
    }

    public static void DeleteFileAccountingForReadOnly(string path)
    {
        FileInfo info = new FileInfo(path);
        if (info.Exists)
        {
            info.Attributes &= ~FileAttributes.ReadOnly;
            info.Delete();
        }
    }

    internal static string FixLineEndings(this string str) => 
        str.Replace("\n", "\r\n").Replace("\r\r", "\r");

    internal static string FixPropsPath(string path)
    {
        path = Path.GetFullPath(path);
        if (path.EndsWith(Path.DirectorySeparatorChar.ToString()))
        {
            return path;
        }
        return (path + Path.DirectorySeparatorChar);
    }

    internal static string GetAssemblyCSharpFirstpassDllDir(WSASDK sdk)
    {
        string str = Path.Combine("$(UnityWSASolutionDir)GeneratedProjects", EditorUserBuildSettings.wsaSDK.ToString());
        switch (sdk)
        {
            case WSASDK.SDK81:
                return Path.Combine(str, @"Assembly-CSharp-firstpass\bin\Store 8.1\$(PlatformName)\$(ConfigurationName)");

            case WSASDK.PhoneSDK81:
                return Path.Combine(str, @"Assembly-CSharp-firstpass\bin\Phone 8.1\$(PlatformName)\$(ConfigurationName)");

            case WSASDK.UniversalSDK81:
                throw new Exception("Cannot get Assembly-CSharp-firstpass.dll path for UniversalSDK81");

            case WSASDK.UWP:
                return Path.Combine(str, @"Assembly-CSharp-firstpass\bin\$(PlatformName)\$(ConfigurationName)");
        }
        throw new Exception("Unknown Windows SDK: " + sdk.ToString());
    }

    private static string GetBackgroundReplacement()
    {
        Color32? splashScreenBackgroundColor = GetSplashScreenBackgroundColor();
        return (!splashScreenBackgroundColor.HasValue ? "" : string.Format(CultureInfo.InvariantCulture, "Background=\"{0}\"", new object[] { ColorToXAMLAttribute(splashScreenBackgroundColor.Value) }));
    }

    public static string[] GetDontOverwriteFilesCpp() => 
        ConcatStringArrays(DontOverwriteFilesCommon, DontOverwriteFilesCpp);

    public static string[] GetDontOverwriteFilesCSharp() => 
        ConcatStringArrays(DontOverwriteFilesCommon, DontOverwriteFilesCSharp);

    public static string GetPlayersDirectoryTag(WSASDK sdk)
    {
        switch (sdk)
        {
            case WSASDK.SDK80:
                return "Windows80";

            case WSASDK.SDK81:
                return "Windows81";

            case WSASDK.PhoneSDK81:
                return "WindowsPhone81";

            case WSASDK.UWP:
                return (@"UAP\" + (!Utility.UseIl2CppScriptingBackend() ? "dotnet" : "il2cpp"));
        }
        throw new Exception($"Unknown WSASDK value {sdk}.");
    }

    public static string GetPlayersRootPath(WSASDK sdk, bool sourceBuild) => 
        (@"$(UnityWSAPlayerDir)Players\" + GetPlayersDirectoryTag(sdk) + @"\$(PlatformTarget)\$(Configuration)");

    internal static string GetSDKPluginTag(WSASDK sdk)
    {
        switch (sdk)
        {
            case WSASDK.SDK80:
                return "SDK";

            case WSASDK.SDK81:
                return "SDK81";

            case WSASDK.PhoneSDK81:
                return "PhoneSDK81";

            case WSASDK.UWP:
                return "UWP";
        }
        throw new Exception("Unknown SDK");
    }

    internal static Color32? GetSplashScreenBackgroundColor()
    {
        Color? splashScreenBackgroundColor = PlayerSettings.WSA.splashScreenBackgroundColor;
        Color32? nullable = !splashScreenBackgroundColor.HasValue ? null : new Color32?(splashScreenBackgroundColor.GetValueOrDefault());
        if (!nullable.HasValue)
        {
            switch (PlayerSettings.SplashScreen.unityLogoStyle)
            {
                case PlayerSettings.SplashScreen.UnityLogoStyle.DarkOnLight:
                    return new Color32(0x21, 0x2c, 0x37, 0xff);

                case PlayerSettings.SplashScreen.UnityLogoStyle.LightOnDark:
                    return new Color32(0xff, 0xff, 0xff, 0xff);
            }
            Debug.LogWarning($"Splash screen style {PlayerSettings.SplashScreen.unityLogoStyle} not fully supported, please report a bug");
        }
        return nullable;
    }

    public static string GetUWPSDKVersion()
    {
        string str = RegistryUtil.GetRegistryStringValue32(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v10.0", "ProductVersion", null);
        str = !string.IsNullOrEmpty(str) ? str : "10.0.10240";
        Version version = new Version(str);
        if (version.Build == -1)
        {
            str = str + ".0";
        }
        if (version.Revision == -1)
        {
            str = str + ".0";
        }
        return str;
    }

    internal static bool IsManifestFileName(this string file) => 
        ((file.IndexOf(".appxmanifest") != -1) || (file.IndexOf("AppxManifest.xml") != -1));

    public static bool OverwriteFile(string installPath, string relFileName, string src, IEnumerable<string> dontOverwriteFiles, OverwriteFilesInfo overwriteControl)
    {
        <OverwriteFile>c__AnonStorey1 storey = new <OverwriteFile>c__AnonStorey1 {
            relFileName = relFileName
        };
        if (!overwriteControl.OverwriteAll)
        {
            if (!File.Exists(Path.Combine(installPath, storey.relFileName)))
            {
                return true;
            }
            if (overwriteControl.KeepAll)
            {
                return false;
            }
            if (ShouldNotOverwrite(storey.relFileName, dontOverwriteFiles))
            {
                if (Enumerable.Any<string>(overwriteControl.DoOverwrite.Keys, new Func<string, bool>(storey.<>m__0)))
                {
                    string str2;
                    if (overwriteControl.Hashes.TryGetValue(storey.relFileName, out str2) && CalculateMD5ForFile(src).Equals(str2))
                    {
                        return false;
                    }
                    return true;
                }
                return false;
            }
        }
        return true;
    }

    internal static void PatchVisualStudioFile(string fullPath)
    {
        string key = Path.GetFileName(fullPath).ToLowerInvariant();
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        if (key != null)
        {
            int num;
            if (<>f__switch$map0 == null)
            {
                Dictionary<string, int> dictionary2 = new Dictionary<string, int>(13) {
                    { 
                        "assemblyinfo.cs",
                        0
                    },
                    { 
                        "app.cs",
                        1
                    },
                    { 
                        "app.xaml",
                        1
                    },
                    { 
                        "app.xaml.cs",
                        1
                    },
                    { 
                        "app.xaml.h",
                        1
                    },
                    { 
                        "app.xaml.cpp",
                        1
                    },
                    { 
                        "mainpage.xaml.cs",
                        1
                    },
                    { 
                        "mainpage.xaml.h",
                        1
                    },
                    { 
                        "mainpage.xaml.cpp",
                        1
                    },
                    { 
                        "app.cpp",
                        1
                    },
                    { 
                        "app.h",
                        1
                    },
                    { 
                        "main.cpp",
                        1
                    },
                    { 
                        "mainpage.xaml",
                        2
                    }
                };
                <>f__switch$map0 = dictionary2;
            }
            if (<>f__switch$map0.TryGetValue(key, out num))
            {
                switch (num)
                {
                    case 0:
                        dictionary["$projectname$"] = Utility.GetVsName();
                        dictionary["$registeredorganization$"] = PlayerSettings.companyName;
                        break;

                    case 1:
                        dictionary["$safeprojectname$"] = Utility.GetVsNamespace();
                        break;

                    case 2:
                        dictionary["$safeprojectname$"] = Utility.GetVsNamespace();
                        dictionary["$background$"] = GetBackgroundReplacement();
                        break;
                }
            }
        }
        if (dictionary.Count != 0)
        {
            string contents = File.ReadAllText(fullPath);
            foreach (KeyValuePair<string, string> pair in dictionary)
            {
                contents = contents.Replace(pair.Key, pair.Value);
            }
            File.WriteAllText(fullPath, contents, Encoding.UTF8);
        }
    }

    private static void RemoveReadOnlyAttribute(DirectoryInfo directoryInfo)
    {
        try
        {
            directoryInfo.Attributes &= ~FileAttributes.ReadOnly;
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Failed to remove readonly attribute from {directoryInfo.FullName}: {exception.Message}.");
        }
    }

    private static void RemoveReadOnlyAttribute(FileInfo fileInfo)
    {
        try
        {
            fileInfo.Attributes &= ~FileAttributes.ReadOnly;
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Failed to remove readonly attribute from {fileInfo.FullName}: {exception.Message}.");
        }
    }

    public static void RemoveReadOnlyAttributes(string directory)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(directory);
        RemoveReadOnlyAttribute(directoryInfo);
        foreach (FileInfo info2 in directoryInfo.GetFiles("*", SearchOption.AllDirectories))
        {
            RemoveReadOnlyAttribute(info2);
        }
        foreach (DirectoryInfo info3 in directoryInfo.GetDirectories("*", SearchOption.AllDirectories))
        {
            RemoveReadOnlyAttribute(info3);
        }
    }

    private static bool ShouldAllowUnsafeCode()
    {
        string fullPath = Path.GetFullPath(Application.dataPath + @"\csc.rsp");
        if (File.Exists(fullPath))
        {
            string str2 = File.ReadAllText(fullPath);
            if (str2.Contains("-unsafe") || str2.Contains("/unsafe"))
            {
                return true;
            }
        }
        return false;
    }

    private static bool ShouldNotOverwrite(string fileName, IEnumerable<string> dontOverwriteFiles)
    {
        <ShouldNotOverwrite>c__AnonStorey0 storey = new <ShouldNotOverwrite>c__AnonStorey0 {
            fileName = fileName
        };
        if (Enumerable.Any<string>(dontOverwriteFiles, new Func<string, bool>(storey.<>m__0)))
        {
            return true;
        }
        storey.fileName = Path.ChangeExtension(storey.fileName, null);
        return Enumerable.Any<string>(dontOverwriteFiles, new Func<string, bool>(storey.<>m__1));
    }

    public static void WriteOverwriteProtectedFileControl(string targetDirectory, string projectDir, IEnumerable<string> dontOverwriteFile, OverwriteFilesInfo overwriteControl)
    {
        Dictionary<string, string> dictionary = null;
        if (!overwriteControl.OverwriteAll && !overwriteControl.KeepAll)
        {
            List<string> allFilesRecursive = FileUtil.GetAllFilesRecursive(targetDirectory);
            dictionary = new Dictionary<string, string>();
            foreach (string str in allFilesRecursive)
            {
                string fileName = FileUtil.RemovePathPrefix(str, projectDir);
                string item = FileUtil.RemovePathPrefix(str, targetDirectory);
                if (ShouldNotOverwrite(fileName, dontOverwriteFile))
                {
                    string str4;
                    if (overwriteControl.UserModified.Contains(item))
                    {
                        str4 = "modified";
                    }
                    else if (overwriteControl.DoOverwrite.ContainsKey(fileName) && overwriteControl.DoOverwrite[fileName])
                    {
                        str4 = "overwrite";
                    }
                    else
                    {
                        str4 = CalculateMD5ForFile(str);
                    }
                    dictionary.Add(item, str4);
                }
            }
        }
        using (StreamWriter writer = new StreamWriter(File.Open(Path.Combine(targetDirectory, "UnityOverwrite.txt"), System.IO.FileMode.Create)))
        {
            writer.WriteLine("# Generated by Unity");
            writer.WriteLine("# Contains a list of files, that will not be overwritten by Unity, if modifield");
            writer.WriteLine("# Change hash to word 'overwrite' (without qoutes) to force it's overwrite");
            writer.WriteLine("# Delete line for file to restore it's hash and control overwrite since then");
            writer.WriteLine("# Write a line equal to 'overwrite-all' (without quotes) to overwrite everything (preserved accross builds)");
            writer.WriteLine("# Write a line equal to 'keep-all' (without quotes) to keep files regardless their modification (preserved accross builds)");
            writer.WriteLine("# Default behavior is to never overwrite files");
            if (overwriteControl.OverwriteAll)
            {
                writer.WriteLine("overwrite-all");
            }
            else if (overwriteControl.KeepAll)
            {
                writer.WriteLine("keep-all");
            }
            else
            {
                foreach (KeyValuePair<string, string> pair in dictionary)
                {
                    writer.WriteLine($"{pair.Key}: {pair.Value}");
                }
            }
        }
    }

    internal static void WriteUnityCommonProps(string path, string playerPackage, string installPath, bool sourceBuild)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("<Project xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">");
        builder.AppendLine("    <PropertyGroup>");
        builder.AppendLine("        <UnityInstallationDir>" + FixPropsPath(Path.GetDirectoryName(EditorApplication.applicationPath)) + "</UnityInstallationDir>");
        builder.AppendLine("        <UnityWSASolutionName>" + Utility.GetVsName() + "</UnityWSASolutionName>");
        builder.AppendLine("        <UnityWSASolutionDir>" + FixPropsPath(installPath) + "</UnityWSASolutionDir>");
        if (UserBuildSettings.copyReferences && !sourceBuild)
        {
            builder.AppendLine("        <UnityWSAPlayerDir>$(UnityWSASolutionDir)</UnityWSAPlayerDir>");
        }
        else
        {
            builder.AppendLine("        <UnityWSAPlayerDir>" + FixPropsPath(playerPackage) + "</UnityWSAPlayerDir>");
        }
        if (!Utility.UseIl2CppScriptingBackend())
        {
            if (sourceBuild)
            {
                builder.AppendLine("        <UnityWSAToolsDir>" + Path.Combine(FixPropsPath(playerPackage), @"Tools\") + "</UnityWSAToolsDir>");
            }
            else
            {
                builder.AppendLine("        <UnityWSAToolsDir>$(UnityWSASolutionDir)" + Path.Combine("Unity", @"Tools\") + "</UnityWSAToolsDir>");
            }
            builder.AppendLine("        <UnityProjectDir>" + FixPropsPath(Application.dataPath + @"\..") + "</UnityProjectDir>");
            if (ShouldAllowUnsafeCode())
            {
                builder.AppendLine("        <AllowUnsafeBlocks>true</AllowUnsafeBlocks>");
            }
        }
        builder.AppendLine("    </PropertyGroup>");
        builder.AppendLine("</Project>");
        if (!File.Exists(path))
        {
            File.WriteAllText(path, builder.ToString(), Encoding.UTF8);
        }
        else if (File.ReadAllText(path) != builder.ToString())
        {
            File.WriteAllText(path, builder.ToString(), Encoding.UTF8);
        }
    }

    [CompilerGenerated]
    private sealed class <OverwriteFile>c__AnonStorey1
    {
        internal string relFileName;

        internal bool <>m__0(string f) => 
            this.relFileName.EndsWith(f);
    }

    [CompilerGenerated]
    private sealed class <ShouldNotOverwrite>c__AnonStorey0
    {
        internal string fileName;

        internal bool <>m__0(string f) => 
            this.fileName.EndsWith(f);

        internal bool <>m__1(string f) => 
            this.fileName.EndsWith(f);
    }

    private class AssemblyConverterCommandAdder
    {
        private IEnumerable<string> assemblies;
        private string assemblyCSharpDllDestPath;
        private string assemblyCSharpDllPath;
        private string assemblyCSharpDllSourcePath;
        private string assemblyCSharpFirstpassDllDestPath;
        private string assemblyCSharpFirstpassDllPath;
        private string assemblyCSharpFirstpassDllSourcePath;
        private bool sourceBuild;
        private TemplateBuilder templateBuilder;
        private bool useAssemblyCSharpProjects;
        private WSASDK wsaSDK;

        public static void AddCommands(IEnumerable<string> assemblies, WSASDK wsaSDK, bool sourceBuild, TemplateBuilder templateBuilder, bool useAssemblyCSharpProjects, string assemblyCSharpDllPath, string assemblyCSharpFirstpassDllPath)
        {
            new MetroVisualStudioSolutionHelper.AssemblyConverterCommandAdder { 
                assemblies = assemblies,
                wsaSDK = wsaSDK,
                sourceBuild = sourceBuild,
                templateBuilder = templateBuilder,
                useAssemblyCSharpProjects = useAssemblyCSharpProjects,
                assemblyCSharpDllPath = assemblyCSharpDllPath,
                assemblyCSharpFirstpassDllPath = assemblyCSharpFirstpassDllPath
            }.Execute();
        }

        private void CopyAssemblies()
        {
            StringBuilder modifyAppXPackage = this.templateBuilder.ModifyAppXPackage;
            modifyAppXPackage.AppendLineWithPrefix("<Message Importance=\"high\" Text=\"Copying unprocessed assemblies...\" />", new object[0]);
            modifyAppXPackage.AppendLineWithPrefix("<Copy SourceFiles=\"@(UnprocessedFile)\" DestinationFolder=\"$(ProjectDir)\" />", new object[0]);
        }

        private void CopyAssemblyCSharpAssembliesToTempFolder()
        {
            StringBuilder modifyAppXPackage = this.templateBuilder.ModifyAppXPackage;
            object[] args = new object[] { this.assemblyCSharpDllSourcePath, this.assemblyCSharpDllDestPath };
            modifyAppXPackage.AppendLineWithPrefix("<Copy SourceFiles=\"{0}\" DestinationFiles=\"{1}\" />", args);
            object[] objArray2 = new object[] { Path.ChangeExtension(this.assemblyCSharpDllSourcePath, ".pdb"), Path.ChangeExtension(this.assemblyCSharpDllDestPath, ".pdb") };
            modifyAppXPackage.AppendLineWithPrefix("<Copy SourceFiles=\"{0}\" DestinationFiles=\"{1}\" Condition=\"Exists('{0}')\" />", objArray2);
            object[] objArray3 = new object[] { this.assemblyCSharpFirstpassDllSourcePath, this.assemblyCSharpFirstpassDllDestPath };
            modifyAppXPackage.AppendLineWithPrefix("<Copy SourceFiles=\"{0}\" DestinationFiles=\"{1}\" />", objArray3);
            object[] objArray4 = new object[] { Path.ChangeExtension(this.assemblyCSharpFirstpassDllSourcePath, ".pdb"), Path.ChangeExtension(this.assemblyCSharpFirstpassDllDestPath, ".pdb") };
            modifyAppXPackage.AppendLineWithPrefix("<Copy SourceFiles=\"{0}\" DestinationFiles=\"{1}\" Condition=\"Exists('{0}')\" />", objArray4);
        }

        private void Execute()
        {
            StringBuilder modifyAppXPackage = this.templateBuilder.ModifyAppXPackage;
            this.assemblyCSharpDllSourcePath = this.assemblyCSharpDllPath;
            this.assemblyCSharpDllDestPath = "$(ProjectDir)" + Path.GetFileName(this.assemblyCSharpDllPath);
            this.assemblyCSharpFirstpassDllSourcePath = this.assemblyCSharpFirstpassDllPath;
            this.assemblyCSharpFirstpassDllDestPath = "$(ProjectDir)" + Path.GetFileName(this.assemblyCSharpFirstpassDllPath);
            modifyAppXPackage.AppendLineWithPrefix("<Message Importance=\"high\" Text=\"UnityInstallationDir &quot;$(UnityInstallationDir)&quot;.\" />", new object[0]);
            modifyAppXPackage.AppendLineWithPrefix("<Message Importance=\"high\" Text=\"UnityWSAPlayerDir &quot;$(UnityWSAPlayerDir)&quot;.\" />", new object[0]);
            modifyAppXPackage.AppendLineWithPrefix("<Message Importance=\"high\" Text=\"UnityProjectDir &quot;$(UnityProjectDir)&quot;.\" />", new object[0]);
            this.CopyAssemblies();
            if (this.useAssemblyCSharpProjects)
            {
                this.CopyAssemblyCSharpAssembliesToTempFolder();
            }
            this.RunAssemblyConverter();
            this.ModifyAppxPackagePayload();
        }

        private void ModifyAppxPackagePayload()
        {
            StringBuilder modifyAppXPackage = this.templateBuilder.ModifyAppXPackage;
            modifyAppXPackage.AppendLineWithPrefix("<Message Importance=\"high\" Text=\"Modifying AppxPackagePayload\" />", new object[0]);
            List<string> list = new List<string>(this.assemblies);
            if (this.useAssemblyCSharpProjects)
            {
                list.Add(Utility.AssemblyCSharpName + ".dll");
                list.Add(Utility.AssemblyCSharpFirstPassName + ".dll");
            }
            modifyAppXPackage.AppendLineWithPrefix("<ItemGroup>", new object[0]);
            foreach (string str in list)
            {
                string[] strArray = new string[] { str, Path.GetFileNameWithoutExtension(str) + ".pdb" };
                foreach (string str2 in strArray)
                {
                    object[] args = new object[] { str2 };
                    modifyAppXPackage.AppendLineWithPrefix("    <AppxPackagePayload Remove=\"@(AppxPackagePayload)\" Condition=\"'%(TargetPath)' == '{0}'\" />", args);
                    object[] objArray2 = new object[] { str2 };
                    modifyAppXPackage.AppendLineWithPrefix("    <AppxPackagePayload Include=\"$(ProjectDir){0}\">", objArray2);
                    object[] objArray3 = new object[] { str2 };
                    modifyAppXPackage.AppendLineWithPrefix("        <TargetPath>{0}</TargetPath>", objArray3);
                    modifyAppXPackage.AppendLineWithPrefix("    </AppxPackagePayload>", new object[0]);
                }
            }
            modifyAppXPackage.AppendLineWithPrefix("</ItemGroup>", new object[0]);
        }

        private void RunAssemblyConverter()
        {
            StringBuilder modifyAppXPackage = this.templateBuilder.ModifyAppXPackage;
            string[] textArray1 = new string[] { "        <PropertyGroup Condition=\" '$(Configuration)' == 'Master' \">", "           <RemoveDebuggableAttribute>True</RemoveDebuggableAttribute>", "        </PropertyGroup>", "        <PropertyGroup Condition=\" '$(Configuration)' != 'Master' \">", "           <RemoveDebuggableAttribute>False</RemoveDebuggableAttribute>", "        </PropertyGroup>" };
            modifyAppXPackage.AppendLine(string.Join("\r\n", textArray1));
            modifyAppXPackage.AppendLineWithPrefix("<Message Importance=\"high\" Text=\"Running AssemblyConverter...\" />", new object[0]);
            modifyAppXPackage.Append("        <Exec Command=\"&quot;$(UnityWSAToolsDir)AssemblyConverter.exe&quot; -platform=");
            modifyAppXPackage.Append(MetroVisualStudioSolutionHelper._assemblyConverterPlatform[this.wsaSDK]);
            if (this.wsaSDK == WSASDK.UWP)
            {
                modifyAppXPackage.Append(" -lock=&quot;$(ProjectDir)project.lock.json&quot; -bits=$(UnityBits) -configuration=$(Configuration) -removeDebuggableAttribute=$(RemoveDebuggableAttribute)");
            }
            string playersRootPath = MetroVisualStudioSolutionHelper.GetPlayersRootPath(this.wsaSDK, this.sourceBuild);
            modifyAppXPackage.Append(" -path=&quot;.&quot; -path=&quot;");
            modifyAppXPackage.Append(playersRootPath);
            modifyAppXPackage.Append("&quot;");
            if (this.useAssemblyCSharpProjects)
            {
                modifyAppXPackage.Append(" &quot;");
                modifyAppXPackage.Append(this.assemblyCSharpDllDestPath);
                modifyAppXPackage.Append("&quot; &quot;");
                modifyAppXPackage.Append(this.assemblyCSharpFirstpassDllDestPath);
                modifyAppXPackage.Append("&quot;");
            }
            foreach (string str2 in this.assemblies)
            {
                modifyAppXPackage.Append(" &quot;$(ProjectDir)");
                modifyAppXPackage.Append(Path.GetFileName(str2));
                modifyAppXPackage.Append("&quot;");
            }
            modifyAppXPackage.AppendLine("\" />");
            modifyAppXPackage.AppendLineWithPrefix("<Message Importance=\"high\" Text=\"AssemblyConverter done.\" />", new object[0]);
        }
    }
}

