using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEditorInternal.VR;

internal class MetroIl2CppVisualStudioSolutionCreator
{
    [CompilerGenerated]
    private static Func<string, bool> <>f__am$cache0;
    [CompilerGenerated]
    private static Func<string, string> <>f__am$cache1;
    [CompilerGenerated]
    private static Func<string, string, string> <>f__am$cache2;
    [CompilerGenerated]
    private static Func<string, string, string> <>f__am$cache3;
    [CompilerGenerated]
    private static Func<string, bool> <>f__am$cache4;
    private const string AdditionalDefines = "WINDOWS_UWP;UNITY_UWP;UNITY_WSA_10_0;UNITY_WSA;UNITY_WINRT";
    private static readonly string[] BaseDefines = new string[] { 
        "WINAPI_FAMILY=WINAPI_FAMILY_APP", "_CRT_SECURE_NO_WARNINGS", "_WINSOCK_DEPRECATED_NO_WARNINGS", "WIN32", "WINDOWS", "_UNICODE", "UNICODE", "ALL_INTERIOR_POINTERS=1", "GC_GCJ_SUPPORT=1", "JAVA_FINALIZATION=1", "NO_EXECUTE_PERMISSION=1", "GC_NO_THREADS_DISCOVERY=1", "IGNORE_DYNAMIC_LOADING=1", "GC_DONT_REGISTER_MAIN_STATIC_DATA=1", "GC_VERSION_MAJOR=7", "GC_VERSION_MINOR=4",
        "GC_VERSION_MICRO=0", "GC_THREADS=1", "USE_MMAP=1", "USE_MUNMAP=1"
    };
    private readonly IEnumerable<string> CppPlugins;
    private readonly HashSet<string> DontOverwriteFiles = new HashSet<string>(MetroVisualStudioSolutionHelper.GetDontOverwriteFilesCpp(), StringComparer.InvariantCultureIgnoreCase);
    private readonly string Il2CppOutputProjectDirectory;
    private readonly bool InstallInBuildsFolder;
    private readonly string InstallPath;
    private readonly LibraryCollection LibraryCollection;
    private readonly string ProjectName;
    private readonly string StagingArea;
    private readonly string UserProjectDirectory;

    private MetroIl2CppVisualStudioSolutionCreator(string installPath, string projectName, string stagingArea, bool installInBuildsFolder, IEnumerable<string> cppPlugins, LibraryCollection libraryCollection)
    {
        this.InstallPath = Path.GetFullPath(installPath);
        this.ProjectName = projectName;
        this.StagingArea = Path.GetFullPath(stagingArea);
        this.InstallInBuildsFolder = installInBuildsFolder;
        this.CppPlugins = cppPlugins;
        this.LibraryCollection = libraryCollection;
        this.Il2CppOutputProjectDirectory = Path.Combine(this.InstallPath, "Il2CppOutputProject");
        this.UserProjectDirectory = Path.Combine(this.InstallPath, this.ProjectName);
    }

    private void CopyPlugins()
    {
        string str = Path.Combine(this.Il2CppOutputProjectDirectory, "il2cppOutput");
        foreach (string str2 in this.CppPlugins)
        {
            File.Copy(str2, Path.Combine(str, Path.GetFileName(str2)));
        }
    }

    private void CreateSolution()
    {
        this.ReshapeStagingArea();
        string[] source = UnityGeneratedCreator.Cpp.Create(this.StagingArea);
        IEnumerable<string> enumerable = this.GatherUserProjectFiles();
        this.MoveFilesFromStagingArea();
        this.CopyPlugins();
        this.WriteSolutionFile();
        this.WriteIl2CppOutputProject();
        List<string> projectFiles = new List<string>();
        List<string> list2 = new List<string>();
        foreach (string str in enumerable)
        {
            if (!this.ShouldOverwriteFile(str) || source.Contains<string>(Path.GetFileName(str)))
            {
                projectFiles.Add(str);
            }
            else
            {
                list2.Add(str);
            }
        }
        this.WriteUserProject(projectFiles);
        this.WriteUnityDataProject(list2);
    }

    public static void CreateSolution(string installPath, string projectName, string stagingArea, bool installInBuildsFolder, IEnumerable<string> cppPlugins, LibraryCollection libraryCollection)
    {
        new MetroIl2CppVisualStudioSolutionCreator(installPath, projectName, stagingArea, installInBuildsFolder, cppPlugins, libraryCollection).CreateSolution();
    }

    private static string DetermineFileTag(string file)
    {
        if (Path.GetFileName(file).Equals("App.xaml", StringComparison.InvariantCultureIgnoreCase))
        {
            return "ApplicationDefinition";
        }
        switch (Path.GetExtension(file))
        {
            case ".cpp":
                return "ClCompile";

            case ".h":
                return "ClInclude";

            case ".appxmanifest":
                return "AppxManifest";

            case ".xaml":
                return "Page";

            case ".winmd":
                return "Reference";

            case ".res":
                return "Resource";
        }
        return "None";
    }

    private IEnumerable<string> GatherUserProjectFiles()
    {
        <GatherUserProjectFiles>c__AnonStorey0 storey = new <GatherUserProjectFiles>c__AnonStorey0 {
            $this = this
        };
        List<string> list = new List<string>();
        list.AddRange(Directory.GetFiles(this.StagingArea, "*.*", SearchOption.TopDirectoryOnly));
        list.AddRange(Directory.GetFiles(Path.Combine(this.StagingArea, "Assets"), "*.*", SearchOption.AllDirectories));
        list.AddRange(Directory.GetFiles(Path.Combine(this.StagingArea, "Data"), "*.*", SearchOption.AllDirectories));
        foreach (Library library in this.LibraryCollection)
        {
            if (library.Native && !library.WinMd)
            {
                list.Add(Path.Combine(this.StagingArea, library.Reference));
            }
        }
        storey.ignoredExtensions = new string[] { ".pdb" };
        return Enumerable.Select<string, string>(Enumerable.Where<string>(list, new Func<string, bool>(storey.<>m__0)), new Func<string, string>(storey.<>m__1));
    }

    private static string MakeFilterItems(IEnumerable<string> files, string UserProjectDirectory, string pathPrefix = "")
    {
        StringBuilder builder = new StringBuilder();
        Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        foreach (string str in files)
        {
            string path = MakeRelativePath(UserProjectDirectory, str);
            string str3 = DetermineFileTag(str);
            string directoryName = Path.GetDirectoryName(path);
            path = pathPrefix + path;
            if (string.IsNullOrEmpty(directoryName))
            {
                builder.AppendFormat("    <{0} Include=\"{1}\" />{2}", str3, path, Environment.NewLine);
            }
            else
            {
                if (!dictionary.ContainsKey(directoryName))
                {
                    dictionary.Add(directoryName, Guid.NewGuid().ToString());
                }
                builder.AppendFormat("    <{0} Include=\"{1}\">{2}", str3, path, Environment.NewLine);
                builder.AppendFormat("      <Filter>{0}</Filter>{1}", directoryName, Environment.NewLine);
                builder.AppendFormat("    </{0}>{1}", str3, Environment.NewLine);
            }
        }
        HashSet<string> set = new HashSet<string>();
        foreach (KeyValuePair<string, string> pair in dictionary)
        {
            for (string str5 = Path.GetDirectoryName(pair.Key); !string.IsNullOrEmpty(str5); str5 = Path.GetDirectoryName(str5))
            {
                if (!dictionary.ContainsKey(str5))
                {
                    set.Add(str5);
                }
            }
        }
        foreach (string str6 in set)
        {
            dictionary.Add(str6, Guid.NewGuid().ToString());
        }
        StringBuilder builder2 = new StringBuilder();
        foreach (KeyValuePair<string, string> pair2 in dictionary)
        {
            builder2.AppendFormat("    <Filter Include=\"{0}\">{1}", pair2.Key, Environment.NewLine);
            builder2.AppendFormat("      <UniqueIdentifier>{{{0}}}</UniqueIdentifier>{1}", pair2.Value, Environment.NewLine);
            builder2.AppendFormat("    </Filter>{0}", Environment.NewLine);
        }
        return (builder2.ToString() + builder.ToString());
    }

    private static string MakeProjectItems(IEnumerable<string> files, string UserProjectDirectory, string pathPrefix = "")
    {
        StringBuilder builder = new StringBuilder();
        foreach (string str in files)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(str);
            string str3 = pathPrefix + MakeRelativePath(UserProjectDirectory, str);
            string str4 = DetermineFileTag(str);
            builder.AppendFormat("    <{0} Include=\"{1}\">{2}", str4, str3, Environment.NewLine);
            if (str4 == null)
            {
                goto Label_0172;
            }
            if (str4 != "ClCompile")
            {
                if (str4 == "ClInclude")
                {
                    goto Label_00E8;
                }
                if (str4 == "None")
                {
                    goto Label_0115;
                }
                if (str4 == "Reference")
                {
                    goto Label_013B;
                }
                if ((str4 == "AppxManifest") || (str4 == "Page"))
                {
                    goto Label_0161;
                }
                goto Label_0172;
            }
            if (Path.GetFileName(str).Equals("pch.cpp", StringComparison.InvariantCultureIgnoreCase))
            {
                builder.AppendLine("      <PrecompiledHeader>Create</PrecompiledHeader>");
            }
        Label_00E8:
            if (Path.GetFileNameWithoutExtension(str).EndsWith(".xaml", StringComparison.InvariantCultureIgnoreCase))
            {
                builder.AppendFormat("      <DependentUpon>{0}</DependentUpon>{1}", fileNameWithoutExtension, Environment.NewLine);
            }
            goto Label_0172;
        Label_0115:
            if (Path.GetExtension(str) != ".pfx")
            {
                builder.AppendLine("      <DeploymentContent>true</DeploymentContent>");
            }
            goto Label_0172;
        Label_013B:
            if (Path.GetExtension(str) == ".winmd")
            {
                builder.AppendLine("      <IsWinMDFile>true</IsWinMDFile>");
            }
            goto Label_0172;
        Label_0161:
            builder.AppendLine("      <SubType>Designer</SubType>");
        Label_0172:
            builder.AppendFormat("    </{0}>{1}", str4, Environment.NewLine);
        }
        return builder.ToString();
    }

    private static string MakeRelativePath(string basePath, string filePath) => 
        Uri.UnescapeDataString(new Uri(basePath.Replace('\\', '/') + "/").MakeRelativeUri(new Uri(filePath)).ToString()).Replace('/', '\\');

    private TemplateBuilder MakeTemplateBuilder()
    {
        TemplateBuilder templateBuilder = new TemplateBuilder();
        MetroVisualStudioSolutionHelper.AddPluginPreBuildEvents(templateBuilder, this.LibraryCollection);
        return templateBuilder;
    }

    private void MoveFilesFromStagingArea()
    {
        Utility.MoveDirectory(Path.Combine(this.StagingArea, "Il2CppOutputProject"), this.Il2CppOutputProjectDirectory, null);
        Utility.MoveDirectory(this.StagingArea, this.UserProjectDirectory, new Func<string, bool>(this.ShouldOverwriteFile));
    }

    private void ReshapeStagingArea()
    {
        string[] paths = new string[] { this.StagingArea, "Data", "Managed" };
        string str = Utility.CombinePath(paths);
        string path = Path.Combine(str, "Plugins");
        if (Directory.Exists(path))
        {
            Utility.MoveDirectory(path, Path.Combine(this.StagingArea, "Plugins"), null);
        }
        Utility.MoveDirectory(str, Path.Combine(this.StagingArea, "Managed"), null);
        if (Directory.Exists(this.Il2CppOutputProjectDirectory))
        {
            MetroVisualStudioSolutionHelper.RemoveReadOnlyAttributes(this.Il2CppOutputProjectDirectory);
            Directory.Delete(this.Il2CppOutputProjectDirectory, true);
        }
    }

    private bool ShouldOverwriteFile(string path)
    {
        string item = MakeRelativePath(this.UserProjectDirectory, path);
        return ((!this.DontOverwriteFiles.Contains(item) && !this.DontOverwriteFiles.Contains(Path.ChangeExtension(item, null))) && (Path.GetExtension(path) != ".pfx"));
    }

    private void WriteIl2CppOutputProject()
    {
        string str = Path.Combine(this.Il2CppOutputProjectDirectory, "Il2CppOutputProject.vcxproj");
        string[] strArray = new string[] { ".c", ".cpp", ".h" };
        if (<>f__am$cache0 == null)
        {
            <>f__am$cache0 = path => !path.Contains(@"IL2CPP\MapFileParser");
        }
        IEnumerable<string> files = Enumerable.Where<string>(from extension in strArray select Directory.GetFiles(this.Il2CppOutputProjectDirectory, "*" + extension, SearchOption.AllDirectories), <>f__am$cache0);
        string str2 = MakeProjectItems(files, this.Il2CppOutputProjectDirectory, "");
        string str3 = MakeFilterItems(files, this.Il2CppOutputProjectDirectory, "");
        char[] separator = new char[] { ';' };
        if (<>f__am$cache1 == null)
        {
            <>f__am$cache1 = d => "--additional-defines=" + d;
        }
        if (<>f__am$cache2 == null)
        {
            <>f__am$cache2 = (x, y) => x + " " + y;
        }
        string str4 = Enumerable.Aggregate<string>(Enumerable.Select<string, string>("WINDOWS_UWP;UNITY_UWP;UNITY_WSA_10_0;UNITY_WSA;UNITY_WINRT".Split(separator), <>f__am$cache1), <>f__am$cache2);
        if (<>f__am$cache3 == null)
        {
            <>f__am$cache3 = (x, y) => x + ";" + y;
        }
        string contents = string.Format(MetroIl2CppTemplates.GetIl2CppOutputProjectTemplate(MetroVisualStudioSolutionHelper.GetUWPSDKVersion()), str2, Enumerable.Aggregate<string>(BaseDefines, <>f__am$cache3) + ";WINDOWS_UWP;UNITY_UWP;UNITY_WSA_10_0;UNITY_WSA;UNITY_WINRT", str4);
        File.WriteAllText(str, contents, Encoding.UTF8);
        string str7 = string.Format(MetroIl2CppTemplates.GetFiltersTemplate(), str3);
        File.WriteAllText(str + ".filters", str7, Encoding.UTF8);
    }

    private void WriteSolutionFile()
    {
        string path = Path.Combine(this.InstallPath, this.ProjectName + ".sln");
        if (!File.Exists(path))
        {
            string str2;
            if (this.InstallInBuildsFolder && (Environment.GetEnvironmentVariable("UNITY_THISISABUILDMACHINE") != "1"))
            {
                StringBuilder builder = new StringBuilder();
                string str3 = "UAP";
                string str4 = "UAPSupport.vcxproj";
                builder.AppendLine("Project(\"{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}\") = \"MetroSupport\", \"..\\..\\..\\..\\Projects\\" + str3 + @"\JamGen\_workspace.vs2010_\" + str4 + "\", \"{C0821136-A691-4E12-91D0-67724AF00396}\"");
                builder.AppendLine("EndProject");
                str2 = string.Format(MetroIl2CppTemplates.GetSourceBuildSolutionTemplate(), this.ProjectName, builder.ToString());
            }
            else
            {
                str2 = string.Format(MetroIl2CppTemplates.GetSolutionTemplate(), this.ProjectName);
            }
            File.WriteAllText(path, str2, Encoding.UTF8);
        }
    }

    private void WriteUnityDataProject(IEnumerable<string> projectFiles)
    {
        string path = Path.Combine(this.UserProjectDirectory, "Unity Data.vcxitems");
        string str2 = path + ".filters";
        string str3 = MakeProjectItems(projectFiles, this.UserProjectDirectory, "$(MSBuildThisFileDirectory)");
        TemplateBuilder builder = this.MakeTemplateBuilder();
        string playersRootPath = MetroVisualStudioSolutionHelper.GetPlayersRootPath(WSASDK.UWP, this.InstallInBuildsFolder);
        string contents = string.Format(MetroIl2CppTemplates.GetUnityDataProjectTemplate(), str3, builder.BeforeResolveReferences.ToString(), playersRootPath);
        File.WriteAllText(path, contents, Encoding.UTF8);
        string str6 = MakeFilterItems(projectFiles, this.UserProjectDirectory, "$(MSBuildThisFileDirectory)");
        string str7 = string.Format(MetroIl2CppTemplates.GetFiltersTemplate(), str6);
        File.WriteAllText(str2, str7, Encoding.UTF8);
    }

    private void WriteUserProject(IEnumerable<string> projectFiles)
    {
        string path = Path.Combine(this.UserProjectDirectory, this.ProjectName + ".vcxproj");
        string str2 = path + ".filters";
        if (!File.Exists(path))
        {
            string str3 = MakeProjectItems(projectFiles, this.UserProjectDirectory, "");
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = f => string.Equals(Path.GetExtension(f), ".pfx", StringComparison.InvariantCultureIgnoreCase);
            }
            string filePath = Enumerable.FirstOrDefault<string>(projectFiles, <>f__am$cache4);
            if (filePath == null)
            {
                throw new Exception("Failed to find package certificate (.pfx) file in the project!");
            }
            string str5 = MakeRelativePath(this.UserProjectDirectory, filePath);
            string uWPSDKVersion = MetroVisualStudioSolutionHelper.GetUWPSDKVersion();
            string str7 = "_UNICODE;UNICODE;%(PreprocessorDefinitions)";
            if (VREditor.GetVREnabledOnTargetGroup(BuildTargetGroup.WSA) && (Array.IndexOf<string>(VREditor.GetVREnabledDevicesOnTargetGroup(BuildTargetGroup.WSA), "HoloLens") > -1))
            {
                str7 = str7 + ";UNITY_HOLOGRAPHIC=1";
            }
            string str8 = !VisualStudioUtil.CanVS2017BuildCppCode() ? "v140" : "v141";
            string contents = string.Format(MetroIl2CppTemplates.GetUserProjectTemplate(uWPSDKVersion), new object[] { Utility.GetVsNamespace(), str3, str5, str7, str8 });
            File.WriteAllText(path, contents, Encoding.UTF8);
        }
        if (!File.Exists(str2))
        {
            string str10 = MakeFilterItems(projectFiles, this.UserProjectDirectory, "");
            string str11 = string.Format(MetroIl2CppTemplates.GetFiltersTemplate(), str10);
            File.WriteAllText(str2, str11, Encoding.UTF8);
        }
    }

    [CompilerGenerated]
    private sealed class <GatherUserProjectFiles>c__AnonStorey0
    {
        internal MetroIl2CppVisualStudioSolutionCreator $this;
        internal string[] ignoredExtensions;

        internal bool <>m__0(string path)
        {
            <GatherUserProjectFiles>c__AnonStorey1 storey = new <GatherUserProjectFiles>c__AnonStorey1 {
                <>f__ref$0 = this,
                path = path
            };
            return !Enumerable.Any<string>(this.ignoredExtensions, new Func<string, bool>(storey.<>m__0));
        }

        internal string <>m__1(string path) => 
            Path.Combine(this.$this.UserProjectDirectory, MetroIl2CppVisualStudioSolutionCreator.MakeRelativePath(this.$this.StagingArea, path));

        private sealed class <GatherUserProjectFiles>c__AnonStorey1
        {
            internal MetroIl2CppVisualStudioSolutionCreator.<GatherUserProjectFiles>c__AnonStorey0 <>f__ref$0;
            internal string path;

            internal bool <>m__0(string ext) => 
                string.Equals(Path.GetExtension(this.path), ext, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}

