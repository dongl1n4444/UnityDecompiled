using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEditor;
using UnityEditor.Modules;

internal class MetroCSharpVisualStudioSolutionCreator
{
    [CompilerGenerated]
    private static Func<string, bool> <>f__am$cache0;
    [CompilerGenerated]
    private static Func<string, bool> <>f__am$cache1;
    [CompilerGenerated]
    private static Func<string, string> <>f__am$cache2;
    [CompilerGenerated]
    private static Func<string, string> <>f__am$cache3;
    [CompilerGenerated]
    private static Func<string, string> <>f__am$cache4;
    [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string[] <DontOverwriteFiles>k__BackingField;
    [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private bool <GenerateReferenceProjects>k__BackingField;
    [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string <InstallPath>k__BackingField;
    [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private Dictionary<WSASDK, LibraryCollection> <LibraryCollections>k__BackingField;
    [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string <Name>k__BackingField;
    [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string <PlayerPackage>k__BackingField;
    [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private bool <SourceBuild>k__BackingField;
    [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string <StagingArea>k__BackingField;
    private static Dictionary<WSASDK, string> assemblyCSharpFirstpassProjectPaths = new Dictionary<WSASDK, string>();
    private static Dictionary<WSASDK, string> assemblyCSharpProjectPaths = new Dictionary<WSASDK, string>();
    private static string currentSrcDirectory;
    private Dictionary<WSABuildType, bool> enableDotNetNative = new Dictionary<WSABuildType, bool>();
    private static string sharedProjectItemsPath = "";
    private static Dictionary<WSASDK, TemplateBuilder> templateBuilders;

    internal MetroCSharpVisualStudioSolutionCreator(string name, string stagingArea, string installPath, string playerPackage)
    {
        this.Name = name;
        this.StagingArea = stagingArea;
        this.InstallPath = installPath;
        this.PlayerPackage = playerPackage;
        this.GenerateReferenceProjects = EditorUserBuildSettings.wsaGenerateReferenceProjects;
        this.EnableDotNetNative[WSABuildType.Debug] = false;
        this.EnableDotNetNative[WSABuildType.Release] = false;
        this.EnableDotNetNative[WSABuildType.Master] = true;
    }

    private static void AppendProjectReference(StringBuilder sb, string guid, string path)
    {
        sb.AppendLine();
        sb.Append("    <ProjectReference Include=\"");
        sb.Append(path);
        sb.AppendLine("\">");
        sb.Append("      <Project>{");
        sb.Append(guid);
        sb.AppendLine("}</Project>");
        sb.Append("      <Name>");
        sb.Append(Path.GetFileNameWithoutExtension(path));
        sb.AppendLine("</Name>");
        sb.AppendLine("      <Private>False</Private>");
        sb.Append("    </ProjectReference>");
    }

    private static void CreateDirectoryIfMissing(string dir)
    {
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    public void CreateSolutionFileFrom()
    {
        CSharpProject project;
        <CreateSolutionFileFrom>c__AnonStorey0 storey = new <CreateSolutionFileFrom>c__AnonStorey0();
        Dictionary<WSASDK, string> assemblyCSharpFirstpassDllPaths = new Dictionary<WSASDK, string>();
        Dictionary<WSASDK, string> assemblyCSharpDllPaths = new Dictionary<WSASDK, string>();
        assemblyCSharpProjectPaths.Clear();
        assemblyCSharpFirstpassProjectPaths.Clear();
        if (this.GenerateReferenceProjects)
        {
            string[] paths = new string[] { this.InstallPath, "GeneratedProjects", EditorUserBuildSettings.wsaSDK.ToString() };
            string str = FileUtil.CombinePaths(paths);
            string[] textArray2 = new string[] { "$(UnityWSASolutionDir)", "GeneratedProjects", EditorUserBuildSettings.wsaSDK.ToString() };
            string str2 = FileUtil.CombinePaths(textArray2);
            switch (EditorUserBuildSettings.wsaSDK)
            {
                case WSASDK.SDK81:
                {
                    assemblyCSharpProjectPaths[WSASDK.SDK81] = Path.Combine(str, @"Assembly-CSharp\Assembly-CSharp-metro-vs2013.csproj");
                    assemblyCSharpFirstpassProjectPaths[WSASDK.SDK81] = Path.Combine(str, @"Assembly-CSharp-firstpass\Assembly-CSharp-firstpass-metro-vs2013.csproj");
                    assemblyCSharpFirstpassDllPaths[WSASDK.SDK81] = Path.Combine(MetroVisualStudioSolutionHelper.GetAssemblyCSharpFirstpassDllDir(WSASDK.SDK81), Utility.AssemblyCSharpFirstPassName + ".dll");
                    string[] textArray3 = new string[] { str2, @"Assembly-CSharp\bin\Store 8.1\$(PlatformName)\$(ConfigurationName)", Utility.AssemblyCSharpName + ".dll" };
                    assemblyCSharpDllPaths[WSASDK.SDK81] = FileUtil.CombinePaths(textArray3);
                    goto Label_036F;
                }
                case WSASDK.PhoneSDK81:
                {
                    assemblyCSharpProjectPaths[WSASDK.PhoneSDK81] = Path.Combine(str, @"Assembly-CSharp\Assembly-CSharp-windows-phone-8.1.csproj");
                    assemblyCSharpFirstpassProjectPaths[WSASDK.PhoneSDK81] = Path.Combine(str, @"Assembly-CSharp-firstpass\Assembly-CSharp-firstpass-windows-phone-8.1.csproj");
                    assemblyCSharpFirstpassDllPaths[WSASDK.PhoneSDK81] = Path.Combine(MetroVisualStudioSolutionHelper.GetAssemblyCSharpFirstpassDllDir(WSASDK.PhoneSDK81), Utility.AssemblyCSharpFirstPassName + ".dll");
                    string[] textArray4 = new string[] { str2, @"Assembly-CSharp\bin\Phone 8.1\$(PlatformName)\$(ConfigurationName)", Utility.AssemblyCSharpName + ".dll" };
                    assemblyCSharpDllPaths[WSASDK.PhoneSDK81] = FileUtil.CombinePaths(textArray4);
                    goto Label_036F;
                }
                case WSASDK.UniversalSDK81:
                {
                    assemblyCSharpProjectPaths[WSASDK.SDK81] = Path.Combine(str, @"Assembly-CSharp\Assembly-CSharp-metro-vs2013.csproj");
                    assemblyCSharpFirstpassProjectPaths[WSASDK.SDK81] = Path.Combine(str, @"Assembly-CSharp-firstpass\Assembly-CSharp-firstpass-metro-vs2013.csproj");
                    assemblyCSharpProjectPaths[WSASDK.PhoneSDK81] = Path.Combine(str, @"Assembly-CSharp\Assembly-CSharp-windows-phone-8.1.csproj");
                    assemblyCSharpFirstpassProjectPaths[WSASDK.PhoneSDK81] = Path.Combine(str, @"Assembly-CSharp-firstpass\Assembly-CSharp-firstpass-windows-phone-8.1.csproj");
                    assemblyCSharpFirstpassDllPaths[WSASDK.SDK81] = Path.Combine(MetroVisualStudioSolutionHelper.GetAssemblyCSharpFirstpassDllDir(WSASDK.SDK81), Utility.AssemblyCSharpFirstPassName + ".dll");
                    string[] textArray5 = new string[] { str2, @"Assembly-CSharp\bin\Store 8.1\$(PlatformName)\$(ConfigurationName)", Utility.AssemblyCSharpName + ".dll" };
                    assemblyCSharpDllPaths[WSASDK.SDK81] = FileUtil.CombinePaths(textArray5);
                    assemblyCSharpFirstpassDllPaths[WSASDK.PhoneSDK81] = Path.Combine(MetroVisualStudioSolutionHelper.GetAssemblyCSharpFirstpassDllDir(WSASDK.PhoneSDK81), Utility.AssemblyCSharpFirstPassName + ".dll");
                    string[] textArray6 = new string[] { str2, @"Assembly-CSharp\bin\Phone 8.1\$(PlatformName)\$(ConfigurationName)", Utility.AssemblyCSharpName + ".dll" };
                    assemblyCSharpDllPaths[WSASDK.PhoneSDK81] = FileUtil.CombinePaths(textArray6);
                    goto Label_036F;
                }
                case WSASDK.UWP:
                {
                    assemblyCSharpProjectPaths[WSASDK.UWP] = Path.Combine(str, @"Assembly-CSharp\Assembly-CSharp.csproj");
                    assemblyCSharpFirstpassProjectPaths[WSASDK.UWP] = Path.Combine(str, @"Assembly-CSharp-firstpass\Assembly-CSharp-firstpass.csproj");
                    assemblyCSharpFirstpassDllPaths[WSASDK.UWP] = Path.Combine(MetroVisualStudioSolutionHelper.GetAssemblyCSharpFirstpassDllDir(WSASDK.UWP), Utility.AssemblyCSharpFirstPassName + ".dll");
                    string[] textArray7 = new string[] { str2, @"Assembly-CSharp\bin\$(PlatformName)\$(ConfigurationName)", Utility.AssemblyCSharpName + ".dll" };
                    assemblyCSharpDllPaths[WSASDK.UWP] = FileUtil.CombinePaths(textArray7);
                    goto Label_036F;
                }
            }
            throw new Exception("Unknown Windows SDK: " + EditorUserBuildSettings.wsaSDK.ToString());
        }
    Label_036F:
        templateBuilders = new Dictionary<WSASDK, TemplateBuilder>();
        switch (EditorUserBuildSettings.wsaSDK)
        {
            case WSASDK.SDK80:
                templateBuilders[WSASDK.SDK80] = new TemplateBuilder();
                break;

            case WSASDK.SDK81:
                templateBuilders[WSASDK.SDK81] = new TemplateBuilder();
                break;

            case WSASDK.PhoneSDK81:
                templateBuilders[WSASDK.PhoneSDK81] = new TemplateBuilder();
                break;

            case WSASDK.UniversalSDK81:
                templateBuilders[WSASDK.SDK81] = new TemplateBuilder();
                templateBuilders[WSASDK.PhoneSDK81] = new TemplateBuilder();
                break;

            case WSASDK.UWP:
                templateBuilders[WSASDK.UWP] = new TemplateBuilder();
                break;

            default:
                throw new Exception("Unknown Windows SDK: " + EditorUserBuildSettings.wsaSDK.ToString());
        }
        currentSrcDirectory = this.StagingArea;
        List<string> dontOverwriteFiles = new List<string>(this.DontOverwriteFiles);
        string str3 = null;
        string str4 = null;
        Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
        List<string> first = new List<string>();
        List<string> unprocessedDllFiles = new List<string>();
        storey.unprocessedPlugins = new List<string>();
        List<string> list4 = new List<string>();
        try
        {
            string[] strArray = Directory.GetFiles(currentSrcDirectory, "*.*", SearchOption.AllDirectories);
            foreach (string str5 in strArray)
            {
                string path = str5.Substring(currentSrcDirectory.Length + 1);
                if ((path.IndexOf(@"Players\") != 0) && (Path.GetExtension(path) != ".mdb"))
                {
                    string str7 = Path.GetExtension(path).ToLower();
                    if (path.IndexOf(".pfx") != -1)
                    {
                        str3 = path;
                    }
                    else if (path.IsManifestFileName())
                    {
                        string str8 = Path.Combine(Path.GetDirectoryName(path), "Package.appxmanifest");
                        dictionary3[str8] = path;
                        path = str8;
                        str4 = path;
                    }
                    else
                    {
                        switch (str7)
                        {
                            case ".dll":
                            case ".winmd":
                            {
                                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(Path.GetFileName(path));
                                bool flag2 = true;
                                if ((fileNameWithoutExtension == Utility.AssemblyCSharpFirstPassName) || (fileNameWithoutExtension == Utility.AssemblyCSharpName))
                                {
                                    flag2 = !this.GenerateReferenceProjects;
                                }
                                foreach (LibraryCollection librarys in this.LibraryCollections.Values)
                                {
                                    if (librarys.Contains(fileNameWithoutExtension))
                                    {
                                        bool flag = !librarys[fileNameWithoutExtension].Native;
                                        flag2 = flag2 && flag;
                                    }
                                }
                                bool flag3 = path.IndexOf(@"Plugins\", StringComparison.OrdinalIgnoreCase) != -1;
                                if (flag2)
                                {
                                    if (!flag3)
                                    {
                                        if (!path.Contains(@"Plugins\"))
                                        {
                                            unprocessedDllFiles.Add(path);
                                        }
                                    }
                                    else
                                    {
                                        string fileName = Path.GetFileName(path);
                                        if (!storey.unprocessedPlugins.Contains<string>(fileName, StringComparer.InvariantCultureIgnoreCase))
                                        {
                                            storey.unprocessedPlugins.Add(fileName);
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                    if (!list4.Contains(path) && (path.IndexOf(".metadata") == -1))
                    {
                        if (this.GenerateReferenceProjects)
                        {
                            string str12 = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(path));
                            if ((str12 == Utility.AssemblyCSharpName) || (str12 == Utility.AssemblyCSharpFirstPassName))
                            {
                                continue;
                            }
                        }
                        first.Add(path);
                    }
                }
            }
        }
        catch (Exception exception)
        {
            throw new Exception($"Failed to get files from {currentSrcDirectory}, {exception.Message}");
        }
        unprocessedDllFiles = Enumerable.Where<string>(unprocessedDllFiles, new Func<string, bool>(storey.<>m__0)).ToList<string>();
        if (str3 == null)
        {
            throw new Exception($"Failed to find *.pfx file in {currentSrcDirectory}");
        }
        if (str4 == null)
        {
            throw new Exception($"Failed to find *.appxmanifest file in {currentSrcDirectory}");
        }
        MetroVisualStudioSolutionHelper.AddPluginPreBuildEvents(templateBuilders, this.LibraryCollections);
        MetroVisualStudioSolutionHelper.AddAssemblyConverterCommands(this.PlayerPackage, this.SourceBuild, templateBuilders, this.LibraryCollections, this.GenerateReferenceProjects, assemblyCSharpDllPaths, assemblyCSharpFirstpassDllPaths);
        List<string> list5 = new List<string>();
        foreach (string str13 in first)
        {
            for (int i = 0; (i = str13.IndexOf(@"\", i)) != -1; i++)
            {
                string item = str13.Substring(0, i);
                if (!list5.Contains(item))
                {
                    list5.Add(item);
                }
            }
        }
        string[] strArray3 = list5.ToArray();
        CreateDirectoryIfMissing(this.InstallPath);
        string str15 = this.Name + ".sln";
        this.WriteSolutionFile(Path.Combine(currentSrcDirectory, str15));
        if (EditorUserBuildSettings.wsaSDK == WSASDK.UniversalSDK81)
        {
            string str16 = this.Name + ".Windows";
            string str17 = this.Name + ".WindowsPhone";
            string str18 = this.Name + ".Shared";
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = x => x.Contains(".Shared");
            }
            List<string> list6 = Enumerable.Where<string>(first, <>f__am$cache0).ToList<string>();
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = x => x.Contains(".WindowsPhone");
            }
            IEnumerable<string> second = Enumerable.Where<string>(first, <>f__am$cache1);
            IEnumerable<string> enumerable2 = first.Except<string>(list6.Union<string>(second));
            string[] strArray4 = UnityGeneratedCreator.CSharp.Create(Path.Combine(currentSrcDirectory, str18));
            foreach (string str19 in strArray4)
            {
                string str20 = Path.Combine(str18, str19);
                list6.Add(str20);
                first.Add(str20);
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = x => "$(MSBuildThisFileDirectory)" + x.Substring((x.IndexOf(".Shared") + ".Shared".Length) + 1);
            }
            list6 = Enumerable.Select<string, string>(list6, <>f__am$cache2).ToList<string>();
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = x => x.Substring((x.IndexOf(".Windows") + ".Windows".Length) + 1);
            }
            enumerable2 = Enumerable.Select<string, string>(enumerable2, <>f__am$cache3);
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = x => x.Substring((x.IndexOf(".WindowsPhone") + ".WindowsPhone".Length) + 1);
            }
            second = Enumerable.Select<string, string>(second, <>f__am$cache4);
            if (this.GenerateReferenceProjects)
            {
                string[] textArray8 = new string[] { assemblyCSharpFirstpassDllPaths[WSASDK.PhoneSDK81], assemblyCSharpDllPaths[WSASDK.PhoneSDK81] };
                second = second.Union<string>(textArray8);
                string[] textArray9 = new string[] { assemblyCSharpFirstpassDllPaths[WSASDK.SDK81], assemblyCSharpDllPaths[WSASDK.SDK81] };
                enumerable2 = enumerable2.Union<string>(textArray9);
            }
            string str21 = Path.Combine(str16, this.Name + ".Windows.csproj");
            string str22 = Path.Combine(str17, this.Name + ".WindowsPhone.csproj");
            string str23 = Path.Combine(str18, this.Name + ".Shared.shproj");
            string str24 = str21 + ".user";
            string str25 = str22 + ".user";
            string unityPropsPath = @"..\..\UnityCommon.props";
            sharedProjectItemsPath = Path.ChangeExtension(Path.Combine("..", str23), ".projitems");
            this.WriteCSProjUserProps(Path.Combine(currentSrcDirectory, str24), WSASDK.SDK81);
            this.WriteCSProjUserProps(Path.Combine(currentSrcDirectory, str25), WSASDK.PhoneSDK81);
            this.WriteCSProj(Path.Combine(currentSrcDirectory, str21), Path.Combine("..", str3), enumerable2.ToArray<string>(), WSASDK.SDK81, unityPropsPath, this.LibraryCollections[WSASDK.SDK81]);
            this.WriteCSProj(Path.Combine(currentSrcDirectory, str22), Path.Combine("..", str3), second.ToArray<string>(), WSASDK.PhoneSDK81, unityPropsPath, this.LibraryCollections[WSASDK.PhoneSDK81]);
            WriteSHProj(Path.Combine(currentSrcDirectory, str23), list6.ToArray());
            list4.Add(str21);
            dontOverwriteFiles.Add(this.Name + ".Windows.csproj");
            list4.Add(str24);
            dontOverwriteFiles.Add(this.Name + ".Windows.csproj.user");
            list4.Add(str22);
            dontOverwriteFiles.Add(this.Name + ".WindowsPhone.csproj");
            list4.Add(str25);
            dontOverwriteFiles.Add(this.Name + ".WindowsPhone.csproj.user");
            list4.Add(str23);
            dontOverwriteFiles.Add(this.Name + ".Shared.shproj");
            list4.Add(Path.ChangeExtension(str23, ".projItems"));
            dontOverwriteFiles.Add(this.Name + ".Shared.projItems");
        }
        else
        {
            string[] collection = UnityGeneratedCreator.CSharp.Create(currentSrcDirectory);
            first.AddRange(collection);
            string str27 = this.Name + ".csproj";
            string str28 = this.Name + ".csproj.user";
            string str29 = @"..\UnityCommon.props";
            this.WriteCSProjUserProps(Path.Combine(currentSrcDirectory, str28), EditorUserBuildSettings.wsaSDK);
            WSASDK wsaSDK = EditorUserBuildSettings.wsaSDK;
            this.WriteCSProj(Path.Combine(currentSrcDirectory, str27), str3, first.ToArray(), wsaSDK, str29, this.LibraryCollections[wsaSDK]);
            list4.Add(str27);
            dontOverwriteFiles.Add(str27);
            list4.Add(str28);
            dontOverwriteFiles.Add(str28);
        }
        dontOverwriteFiles.Add(str15);
        bool targetDirEmpty = !this.TargetDirHasProject();
        string projectDir = Path.Combine(this.InstallPath, this.Name);
        OverwriteFilesInfo overwriteControl = MetroVisualStudioSolutionHelper.CheckOverwriteFiles(this.InstallPath, projectDir, targetDirEmpty);
        string src = Path.Combine(currentSrcDirectory, str15);
        if (MetroVisualStudioSolutionHelper.OverwriteFile(this.InstallPath, str15, src, dontOverwriteFiles, overwriteControl))
        {
            FileUtil.UnityFileCopy(src, Path.Combine(this.InstallPath, str15), true);
        }
        CreateDirectoryIfMissing(projectDir);
        foreach (string str32 in strArray3)
        {
            CreateDirectoryIfMissing(Path.Combine(projectDir, str32));
        }
        foreach (string str33 in list4)
        {
            string str34 = Path.Combine(currentSrcDirectory, str33);
            string str35 = Path.Combine(projectDir, str33);
            if (File.Exists(str34))
            {
                if (MetroVisualStudioSolutionHelper.OverwriteFile(this.InstallPath, Path.Combine(this.Name, str33), str34, dontOverwriteFiles, overwriteControl))
                {
                    CreateDirectoryIfMissing(Path.GetDirectoryName(str35));
                    FileUtil.UnityFileCopy(str34, str35, true);
                }
            }
            else if ((((str33.IndexOf(".pdb") == -1) && (str33.IndexOf(".scale-80.") == -1)) && ((str33.IndexOf(".scale-140.") == -1) && (str33.IndexOf(".scale-180.") == -1))) && (str33.IndexOf(".scale-240.") == -1))
            {
                throw new Exception(string.Format("Failed to find " + str34, new object[0]));
            }
        }
        string commandLineArgsFile = PlayerSettings.WSA.commandLineArgsFile;
        bool flag5 = !string.IsNullOrEmpty(commandLineArgsFile);
        foreach (string str37 in first)
        {
            string file = str37;
            if (file.IsManifestFileName())
            {
                file = dictionary3[file];
            }
            string fullPath = Path.Combine(currentSrcDirectory, file);
            string str40 = Path.Combine(projectDir, str37);
            if ((Path.GetFileName(str40) == "MainPage.xaml.cs") && flag5)
            {
                PatchFileToLoadCommandLineArgs(fullPath, commandLineArgsFile);
            }
            MetroVisualStudioSolutionHelper.PatchVisualStudioFile(fullPath);
            if (MetroVisualStudioSolutionHelper.OverwriteFile(this.InstallPath, Path.Combine(this.Name, str37), fullPath, dontOverwriteFiles, overwriteControl))
            {
                FileUtil.UnityFileCopy(fullPath, str40, true);
            }
        }
        MetroVisualStudioSolutionHelper.CopyAssembliesToUnprocessed(unprocessedDllFiles, projectDir);
        foreach (KeyValuePair<WSASDK, string> pair in assemblyCSharpProjectPaths)
        {
            CSharpProject[] projectArray1 = new CSharpProject[1];
            project = new CSharpProject {
                Path = assemblyCSharpFirstpassProjectPaths[pair.Key],
                Guid = new Guid(MetroAssemblyCSharpCreator.GetAssemblyCSharpFirstpassGuid(pair.Key))
            };
            projectArray1[0] = project;
            CSharpProject[] additionalProjectReferences = projectArray1;
            project = new CSharpProject {
                Path = pair.Value,
                Guid = new Guid(MetroAssemblyCSharpCreator.GetAssemblyCSharpGuid(pair.Key))
            };
            MetroAssemblyCSharpCreator.CreateAssemblyCSharp(project, this.PlayerPackage, Utility.AssemblyCSharpName, this.LibraryCollections[pair.Key], pair.Key, additionalProjectReferences);
        }
        foreach (KeyValuePair<WSASDK, string> pair2 in assemblyCSharpFirstpassProjectPaths)
        {
            project = new CSharpProject {
                Path = pair2.Value,
                Guid = new Guid(MetroAssemblyCSharpCreator.GetAssemblyCSharpFirstpassGuid(pair2.Key))
            };
            MetroAssemblyCSharpCreator.CreateAssemblyCSharp(project, this.PlayerPackage, Utility.AssemblyCSharpFirstPassName, this.LibraryCollections[pair2.Key], pair2.Key, null);
        }
        MetroVisualStudioSolutionHelper.WriteOverwriteProtectedFileControl(this.InstallPath, projectDir, dontOverwriteFiles, overwriteControl);
    }

    private static string GenerateConfigMappings(string guid, string srcSolutionPlatform, string dstProjectPlatform, string[] availableProjectConfigs)
    {
        string[] strArray = new string[] { "Debug", "Release", "Master" };
        StringBuilder builder = new StringBuilder();
        foreach (string str in strArray)
        {
            string str2 = !availableProjectConfigs.Contains<string>(str) ? availableProjectConfigs.Last<string>() : str;
            builder.AppendLine($"		{{{guid}}}.{str}|{srcSolutionPlatform}.ActiveCfg = {str2}|{dstProjectPlatform}");
            builder.AppendLine($"		{{{guid}}}.{str}|{srcSolutionPlatform}.Build.0 = {str2}|{dstProjectPlatform}");
        }
        return builder.ToString();
    }

    private string GetAssemblyCSharpFirstpassProjectPath(WSASDK wsaSDK)
    {
        if (!this.GenerateReferenceProjects)
        {
            return null;
        }
        return assemblyCSharpFirstpassProjectPaths[wsaSDK];
    }

    private string GetAssemblyCSharpProjectPath(WSASDK wsaSDK)
    {
        if (!this.GenerateReferenceProjects)
        {
            return null;
        }
        return assemblyCSharpProjectPaths[wsaSDK];
    }

    public static string GetTagForFile(string basePath, string file)
    {
        string str = "App.xaml";
        int index = file.IndexOf(str);
        if ((index != -1) && (index == (file.Length - str.Length)))
        {
            return "ApplicationDefinition";
        }
        if (file.IsManifestFileName())
        {
            return "AppxManifest";
        }
        if (file.IndexOf(".png") != (file.Length - 4))
        {
            if (file.IndexOf(".cs") == (file.Length - 3))
            {
                return "Compile";
            }
            if (file.IndexOf(".xaml") == (file.Length - 5))
            {
                return "Page";
            }
        }
        return "Content";
    }

    private static string GetXamlFileFromCSFile(string path)
    {
        int index = path.IndexOf(')');
        if (index != -1)
        {
            path = path.Substring(index + 1);
        }
        return Path.GetFileNameWithoutExtension(path);
    }

    private static bool IgnoreFile(string file)
    {
        string fileName = Path.GetFileName(file);
        string extension = Path.GetExtension(fileName);
        bool flag = (((string.Equals(extension, ".pfx", StringComparison.InvariantCultureIgnoreCase) || string.Equals(extension, ".dll", StringComparison.InvariantCultureIgnoreCase)) || (string.Equals(extension, ".winmd", StringComparison.InvariantCultureIgnoreCase) || string.Equals(extension, ".pdb", StringComparison.InvariantCultureIgnoreCase))) || (string.Equals(extension, ".mdb", StringComparison.InvariantCultureIgnoreCase) || string.Equals(fileName, "project.json", StringComparison.InvariantCultureIgnoreCase))) || file.IsManifestFileName();
        if (!flag)
        {
            bool flag2 = EditorUserBuildSettings.wsaGenerateReferenceProjects && ((fileName == (Utility.AssemblyCSharpName + ".dll")) || (fileName == (Utility.AssemblyCSharpFirstPassName + ".dll")));
            flag |= (file.IndexOf(@"Data\") != -1) && !flag2;
        }
        return flag;
    }

    private static void PatchFileToLoadCommandLineArgs(string fullPath, string cmdLineArgsFile)
    {
        string contents = File.ReadAllText(fullPath);
        string oldValue = "appCallbacks.SetBridge(_bridge);";
        contents = contents.Replace(oldValue, oldValue + $"
				appCallbacks.ParseCommandLineArgsFromFiles("{cmdLineArgsFile}");");
        File.WriteAllText(fullPath, contents, Encoding.UTF8);
    }

    private static string ProcessFilesToProjectFormat(string basePath, string[] files, LibraryCollection libraryCollection, bool generateReferenceProjects)
    {
        StringBuilder builder = new StringBuilder();
        foreach (string str in files)
        {
            if (IgnoreFile(str))
            {
                continue;
            }
            string tagForFile = GetTagForFile(basePath, str);
            builder.AppendLine($"    <{tagForFile} Include="{str}">");
            if (tagForFile != null)
            {
                if (tagForFile == "Page")
                {
                    builder.AppendLine("      <Generator>MSBuild:Compile</Generator>");
                    builder.AppendLine("      <SubType>Designer</SubType>");
                }
                else if (tagForFile == "Compile")
                {
                    goto Label_008E;
                }
            }
            goto Label_00C2;
        Label_008E:
            if (str.IndexOf(".xaml.") != -1)
            {
                builder.AppendLine($"      <DependentUpon>{GetXamlFileFromCSFile(str)}</DependentUpon>");
            }
        Label_00C2:
            builder.AppendLine($"    </{tagForFile}>");
        }
        foreach (Library library in libraryCollection)
        {
            if (library.Native && !library.WinMd)
            {
                builder.AppendLine($"    <Content Include="{library.Name}.dll" />");
                builder.AppendLine(string.Format("    <Content Include=\"{0}.pdb\" Condition=\"Exists('{0}.pdb')\" />", library.Name));
            }
            else if (!generateReferenceProjects || (!string.Equals(library.Name, Utility.AssemblyCSharpName, StringComparison.InvariantCultureIgnoreCase) && !string.Equals(library.Name, Utility.AssemblyCSharpFirstPassName, StringComparison.InvariantCultureIgnoreCase)))
            {
                builder.AppendLine($"    <Reference Include="{library.Name}">");
                if (library.Native && library.WinMd)
                {
                    builder.AppendLine($"      <HintPath>.\{library.Name}.{!library.WinMd ? "dll" : "winmd"}</HintPath>");
                }
                else
                {
                    builder.AppendLine($"      <HintPath>.\Unprocessed\{library.Name}.{!library.WinMd ? "dll" : "winmd"}</HintPath>");
                }
                builder.AppendLine("    </Reference>");
            }
        }
        return builder.ToString();
    }

    private bool TargetDirHasProject()
    {
        string[] fileSystemEntries = Directory.GetFileSystemEntries(this.InstallPath);
        if (fileSystemEntries.Length == 0)
        {
            return false;
        }
        return (fileSystemEntries.Contains<string>(Path.Combine(this.InstallPath, "Players")) || (fileSystemEntries.Contains<string>(Path.Combine(this.InstallPath, "Unity")) || (fileSystemEntries.Contains<string>(Path.Combine(this.InstallPath, this.Name)) || fileSystemEntries.Contains<string>(Path.Combine(this.InstallPath, this.Name + ".sln")))));
    }

    private void WriteCSProj(string fileName, string pfxFile, string[] files, WSASDK wsaSDK, string unityPropsPath, LibraryCollection libraryCollection)
    {
        string vSProjTemplate;
        StringBuilder builder = new StringBuilder();
        StringBuilder builder2 = new StringBuilder();
        StringBuilder builder3 = new StringBuilder();
        builder3.Append(ProcessFilesToProjectFormat(Path.GetDirectoryName(fileName), files, libraryCollection, this.GenerateReferenceProjects));
        switch (wsaSDK)
        {
            case WSASDK.SDK81:
                vSProjTemplate = MetroCSharpVS2013Templates.GetVSProjTemplate();
                break;

            case WSASDK.PhoneSDK81:
                vSProjTemplate = WP_8_1_CSharpTemplates.GetVSProjTemplate();
                break;

            case WSASDK.UWP:
                vSProjTemplate = UAPCSharpTemplates.GetVSProjTemplate(MetroVisualStudioSolutionHelper.GetUWPSDKVersion(), this.EnableDotNetNative);
                break;

            default:
                throw new Exception("Unknown Windows SDK: " + wsaSDK.ToString());
        }
        string[] strArray = new string[] { "UnityPlayer.winmd", "WinRTBridge.winmd", "BridgeInterface.winmd", "UnityEngineDelegates.winmd", "UnityEngineProxy.dll" };
        string playersRootPath = MetroVisualStudioSolutionHelper.GetPlayersRootPath(wsaSDK, this.SourceBuild);
        foreach (string str4 in strArray)
        {
            string oldValue = @"..\Players\$(PlatformTarget)\$(Configuration)\" + str4;
            string newValue = Path.Combine(playersRootPath, str4);
            vSProjTemplate = vSProjTemplate.Replace(oldValue, newValue);
        }
        if (!string.IsNullOrEmpty(sharedProjectItemsPath))
        {
            builder2.AppendLine($"  <Import Project="{sharedProjectItemsPath}" Label="Shared" />");
            builder.AppendLine("<SynthesizeLinkMetadata>true</SynthesizeLinkMetadata>");
        }
        string str7 = string.Empty;
        if (this.GenerateReferenceProjects)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.Append("  <ItemGroup>");
            AppendProjectReference(sb, MetroAssemblyCSharpCreator.GetAssemblyCSharpGuid(wsaSDK), this.GetAssemblyCSharpProjectPath(wsaSDK));
            AppendProjectReference(sb, MetroAssemblyCSharpCreator.GetAssemblyCSharpFirstpassGuid(wsaSDK), this.GetAssemblyCSharpFirstpassProjectPath(wsaSDK));
            sb.AppendLine();
            sb.Append("  </ItemGroup>");
            str7 = sb.ToString();
        }
        TemplateBuilder builder5 = templateBuilders[wsaSDK];
        string contents = string.Format(vSProjTemplate, new object[] { pfxFile, builder3, builder.ToString(), builder2.ToString(), unityPropsPath, str7, Utility.GetVsNamespace(), Utility.GetVsName(), builder5.GetAdditionalTargets() });
        File.WriteAllText(fileName, contents, Encoding.UTF8);
    }

    private void WriteCSProjUserProps(string fileName, WSASDK wsaSDK)
    {
        switch (wsaSDK)
        {
            case WSASDK.SDK81:
                File.WriteAllText(fileName, MetroCSharpVS2013Templates.GetVSProjUserPropsTemplate(), Encoding.UTF8);
                return;

            case WSASDK.PhoneSDK81:
                File.WriteAllText(fileName, WP_8_1_CSharpTemplates.GetVSProjUserPropsTemplate(), Encoding.UTF8);
                return;

            case WSASDK.UWP:
                File.WriteAllText(fileName, UAPCSharpTemplates.GetVSProjUserPropsTemplate(this.SourceBuild));
                return;
        }
        throw new Exception("Unknown Windows SDK: " + wsaSDK.ToString());
    }

    private static void WriteSHProj(string fileName, string[] files)
    {
        string path = Path.ChangeExtension(fileName, "projitems");
        string str2 = ProcessFilesToProjectFormat(Path.GetDirectoryName(fileName), files, new LibraryCollection(), false);
        string sHProjTemplate = Universal_8_1_CSharpTemplates.GetSHProjTemplate();
        string projItemsTemplate = Universal_8_1_CSharpTemplates.GetProjItemsTemplate();
        string contents = string.Format(sHProjTemplate, Path.GetFileName(path), Utility.GetVsNamespace());
        File.WriteAllText(fileName, contents, Encoding.UTF8);
        contents = string.Format(projItemsTemplate, str2, Utility.GetPackageName(true));
        File.WriteAllText(path, contents, Encoding.UTF8);
    }

    private void WriteSolutionFile(string solutionFileName)
    {
        string str;
        string str2;
        string str3;
        string str6;
        Dictionary<WSASDK, StringBuilder> source = new Dictionary<WSASDK, StringBuilder>();
        StringBuilder builder = new StringBuilder();
        StringBuilder builder2 = new StringBuilder();
        bool sourceBuild = this.SourceBuild;
        if (sourceBuild && (Environment.GetEnvironmentVariable("UNITY_THISISABUILDMACHINE") == "1"))
        {
            sourceBuild = false;
        }
        switch (EditorUserBuildSettings.wsaSDK)
        {
            case WSASDK.SDK81:
                str = !sourceBuild ? MetroCSharpVS2013Templates.GetSolutionTemplate() : MetroCSharpVS2013Templates.GetSourceBuildSolutionTemplate();
                str2 = "Metro81";
                str3 = "MetroSupport.vcxproj";
                source[WSASDK.SDK81] = new StringBuilder();
                break;

            case WSASDK.PhoneSDK81:
                str = !sourceBuild ? WP_8_1_CSharpTemplates.GetSolutionTemplate() : WP_8_1_CSharpTemplates.GetSourceBuildSolutionTemplate();
                str2 = "WP8.1";
                str3 = "WP8.1Support.vcxproj";
                source[WSASDK.PhoneSDK81] = new StringBuilder();
                break;

            case WSASDK.UniversalSDK81:
                str = !sourceBuild ? Universal_8_1_CSharpTemplates.GetSolutionTemplate() : Universal_8_1_CSharpTemplates.GetSourceBuildSolutionTemplate();
                str2 = "";
                str3 = "";
                source[WSASDK.SDK81] = new StringBuilder();
                source[WSASDK.PhoneSDK81] = new StringBuilder();
                break;

            case WSASDK.UWP:
                str = !sourceBuild ? UAPCSharpTemplates.GetSolutionTemplate() : UAPCSharpTemplates.GetSourceBuildSolutionTemplate();
                str2 = "UAP";
                str3 = "UAPSupport.vcxproj";
                source[WSASDK.UWP] = new StringBuilder();
                break;

            default:
                throw new Exception("Unknown Windows SDK: " + EditorUserBuildSettings.wsaSDK.ToString());
        }
        if (sourceBuild)
        {
            if (EditorUserBuildSettings.wsaSDK == WSASDK.UniversalSDK81)
            {
                str2 = "WP8.1";
                str3 = "WP8.1Support.vcxproj";
                builder2.AppendLine("Project(\"{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}\") = \"WP8.1Support\", \"..\\..\\..\\..\\Projects\\" + str2 + @"\JamGen\_workspace.vs2010_\" + str3 + "\", \"{59D28A5A-ABA9-4BDF-A2B6-AB532C6DADFE}\"");
                builder2.AppendLine("EndProject");
                str2 = "Metro81";
                str3 = "MetroSupport.vcxproj";
            }
            builder2.AppendLine("Project(\"{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}\") = \"MetroSupport\", \"..\\..\\..\\..\\Projects\\" + str2 + @"\JamGen\_workspace.vs2010_\" + str3 + "\", \"{B6936B32-2A3B-4BD6-8799-A7EE3C02E72E}\"");
            builder2.AppendLine("EndProject");
        }
        foreach (KeyValuePair<WSASDK, string> pair in assemblyCSharpProjectPaths)
        {
            string assemblyCSharpGuid = MetroAssemblyCSharpCreator.GetAssemblyCSharpGuid(pair.Key);
            source[pair.Key].AppendLine(string.Format("\t\t{{{0}}} = {{{0}}}", assemblyCSharpGuid));
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pair.Value);
            builder2.AppendLine($"Project("{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}") = "{fileNameWithoutExtension}", "{Path.GetFullPath(pair.Value)}", "{{{assemblyCSharpGuid}}}"");
            builder2.AppendLine("EndProject");
            builder.Append(GenerateConfigMappings(assemblyCSharpGuid, "x86", "x86", MetroAssemblyCSharpCreator.availableConfigs));
            if (EditorUserBuildSettings.wsaSDK == WSASDK.UWP)
            {
                builder.Append(GenerateConfigMappings(assemblyCSharpGuid, "x64", "x64", MetroAssemblyCSharpCreator.availableConfigs));
            }
            builder.Append(GenerateConfigMappings(assemblyCSharpGuid, "ARM", "ARM", MetroAssemblyCSharpCreator.availableConfigs));
        }
        foreach (KeyValuePair<WSASDK, string> pair2 in assemblyCSharpFirstpassProjectPaths)
        {
            string assemblyCSharpFirstpassGuid = MetroAssemblyCSharpCreator.GetAssemblyCSharpFirstpassGuid(pair2.Key);
            source[pair2.Key].AppendLine(string.Format("\t\t{{{0}}} = {{{0}}}", assemblyCSharpFirstpassGuid));
            string introduced20 = Path.GetFileNameWithoutExtension(pair2.Value);
            builder2.AppendLine($"Project("{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}") = "{introduced20}", "{Path.GetFullPath(pair2.Value)}", "{{{assemblyCSharpFirstpassGuid}}}"");
            builder2.AppendLine("EndProject");
            builder.Append(GenerateConfigMappings(assemblyCSharpFirstpassGuid, "x86", "x86", MetroAssemblyCSharpCreator.availableConfigs));
            builder.Append(GenerateConfigMappings(assemblyCSharpFirstpassGuid, "ARM", "ARM", MetroAssemblyCSharpCreator.availableConfigs));
        }
        if (EditorUserBuildSettings.wsaSDK == WSASDK.UniversalSDK81)
        {
            str6 = string.Format(str, new object[] { this.Name, source[WSASDK.SDK81], source[WSASDK.PhoneSDK81], builder2, builder });
        }
        else
        {
            str6 = string.Format(str, new object[] { this.Name, source.First<KeyValuePair<WSASDK, StringBuilder>>().Value, builder2, builder });
        }
        File.WriteAllText(solutionFileName, str6, Encoding.UTF8);
    }

    public string[] DontOverwriteFiles { get; set; }

    public Dictionary<WSABuildType, bool> EnableDotNetNative =>
        this.enableDotNetNative;

    private bool GenerateReferenceProjects { get; set; }

    private string InstallPath { get; set; }

    public Dictionary<WSASDK, LibraryCollection> LibraryCollections { get; set; }

    private string Name { get; set; }

    private string PlayerPackage { get; set; }

    public bool SourceBuild { get; set; }

    private string StagingArea { get; set; }

    [CompilerGenerated]
    private sealed class <CreateSolutionFileFrom>c__AnonStorey0
    {
        internal List<string> unprocessedPlugins;

        internal bool <>m__0(string x) => 
            !this.unprocessedPlugins.Contains(Path.GetFileName(x));
    }
}

