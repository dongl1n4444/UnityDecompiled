using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Modules;
using UnityEditor.Scripting.Compilers;
using UnityEditorInternal;

internal static class MetroAssemblyCSharpCreator
{
    [CompilerGenerated]
    private static Func<MonoIsland?, bool> <>f__am$cache0;
    internal static string[] assemblyCSharpFirstpassGUIDs;
    internal static string[] assemblyCSharpGUIDs;
    internal static string[] availableConfigs = new string[] { "Debug", "Release", "Master" };
    internal const int kGUIDIndexPhone = 1;
    internal const int kGUIDIndexStore = 0;

    public static void CreateAssemblyCSharp(CSharpProject project, string playerPackage, string assemblyName, LibraryCollection libraryCollection, WSASDK wsaSDK, CSharpProject[] additionalProjectReferences)
    {
        string relativeFinalProjectDirectory = GetRelativeFinalProjectDirectory(wsaSDK);
        List<AssemblyCSharpPlugin> plugins = new List<AssemblyCSharpPlugin>();
        bool includeUnet = false;
        foreach (Library library in libraryCollection)
        {
            if ((!library.Native || library.WinMd) && (!string.Equals(library.Name, Utility.AssemblyCSharpName, StringComparison.InvariantCultureIgnoreCase) && !string.Equals(library.Name, Utility.AssemblyCSharpFirstPassName, StringComparison.InvariantCultureIgnoreCase)))
            {
                if (string.Equals(library.Name, "UnityEngine.Networking", StringComparison.InvariantCultureIgnoreCase))
                {
                    includeUnet = true;
                }
                string reference = library.Reference;
                string str3 = @"Unprocessed\";
                if (library.WinMd && library.Native)
                {
                    str3 = "";
                }
                if (!reference.StartsWith(@"Plugins\", StringComparison.InvariantCultureIgnoreCase))
                {
                    reference = str3 + library.Reference;
                }
                reference = Utility.CombinePath(relativeFinalProjectDirectory, reference);
                AssemblyCSharpPlugin item = new AssemblyCSharpPlugin {
                    Name = library.Name,
                    HintPath = reference
                };
                plugins.Add(item);
            }
        }
        StringBuilder builder = new StringBuilder();
        string str4 = "$(UnityProjectDir)Assets";
        string[] strArray = new string[] { "Plugins", "Standard Assets", "Standard Assets (Mobile)", "Pro Standard Assets" };
        string additionalReferencePath = "";
        if (additionalProjectReferences != null)
        {
            builder.AppendFormat("    <Compile Include=\"{0}\\**\\*.cs\" Exclude=\"{0}\\**\\Editor\\**\\*.cs", str4);
            foreach (string str6 in strArray)
            {
                builder.AppendFormat(@";{0}\{1}\**\*.cs", str4, str6);
            }
            builder.AppendLine("\">");
            if (str4.StartsWith(".."))
            {
                builder.AppendFormat("      <Link>%(RecursiveDir)%(Filename)%(Extension)</Link>", new object[0]);
                builder.AppendLine();
            }
            builder.AppendLine("    </Compile>");
            additionalReferencePath = string.Format("-additionalAssemblyPath=\"{0}\"", MetroVisualStudioSolutionHelper.GetAssemblyCSharpFirstpassDllDir(wsaSDK));
        }
        else
        {
            foreach (string str7 in strArray)
            {
                builder.AppendFormat("    <Compile Include=\"{0}\\{1}\\**\\*.cs\" Exclude=\"{0}\\{1}\\**\\Editor\\**\\*.cs\">", str4, str7);
                builder.AppendLine();
                if (str4.StartsWith(".."))
                {
                    builder.AppendFormat(@"      <Link>{0}\%(RecursiveDir)%(Filename)%(Extension)</Link>", str7);
                    builder.AppendLine();
                }
                builder.AppendLine("    </Compile>");
            }
        }
        if (<>f__am$cache0 == null)
        {
            <>f__am$cache0 = new Func<MonoIsland?, bool>(null, (IntPtr) <CreateAssemblyCSharp>m__0);
        }
        MonoIsland? nullable = Enumerable.FirstOrDefault<MonoIsland?>(Enumerable.Cast<MonoIsland?>(InternalEditorUtility.GetMonoIslands()), <>f__am$cache0);
        string preTargets = "  <Import Project=\"$(ProjectDir)..\\..\\..\\UnityCommon.props\" />";
        string postTargets = GetSerializationWeaverTargets(relativeFinalProjectDirectory, includeUnet, additionalReferencePath);
        CreateAssemblyCSharp(project, playerPackage, assemblyName, nullable.Value._defines, plugins, builder.ToString(), preTargets, postTargets, wsaSDK, additionalProjectReferences);
    }

    public static void CreateAssemblyCSharp(CSharpProject project, string playerPackage, string assemblyName, IEnumerable<string> defines, IEnumerable<AssemblyCSharpPlugin> plugins, string files, string preTargets, string postTargets, WSASDK wsaSDK, IEnumerable<CSharpProject> additionalProjectReferences)
    {
        string assemblyCSharpConfigs = GetAssemblyCSharpConfigs(wsaSDK, defines);
        StringBuilder builder = new StringBuilder();
        if (additionalProjectReferences != null)
        {
            string strA = Path.GetDirectoryName(project.Path);
            foreach (CSharpProject project2 in additionalProjectReferences)
            {
                string strB = Path.GetDirectoryName(project2.Path);
                if (string.Compare(strA, strB, true) == 0)
                {
                    throw new Exception(string.Format("Projects {0} and {1} should be in different folders.", strA, strB));
                }
                builder.AppendLine("    <ProjectReference Include=\"" + project2.Path.Replace('/', '\\') + "\">");
                builder.AppendLine("      <Project>{" + project2.Guid + "}</Project>");
                builder.AppendLine("      <Name>" + Path.GetFileNameWithoutExtension(project2.Path) + "</Name>");
                builder.Append("    </ProjectReference>");
            }
        }
        foreach (AssemblyCSharpPlugin plugin in plugins)
        {
            builder.AppendLine();
            builder.AppendFormat("    <Reference Include=\"{0}\">", plugin.Name);
            builder.AppendLine();
            builder.AppendFormat("      <HintPath>{0}</HintPath>", plugin.HintPath.Replace('/', '\\'));
            builder.AppendLine();
            builder.AppendLine("      <Private>False</Private>");
            builder.Append("    </Reference>");
        }
        if (wsaSDK == WSASDK.UWP)
        {
            foreach (UWPExtensionSDK nsdk in UWPReferences.GetExtensionSDKs())
            {
                builder.AppendLine();
                builder.AppendFormat("    <SDKReference Include=\"{0}, Version={1}\"/>", nsdk.Name, nsdk.Version);
            }
        }
        string contents = "";
        switch (wsaSDK)
        {
            case WSASDK.SDK81:
                contents = string.Format(GetAssemblyCSharpTemplate81(), new object[] { assemblyName, assemblyCSharpConfigs, builder, files, postTargets, project.Guid, preTargets });
                break;

            case WSASDK.PhoneSDK81:
                contents = string.Format(GetAssemblyCSharpTemplatePhone81(), new object[] { assemblyName, assemblyCSharpConfigs, builder, files, postTargets, project.Guid, preTargets });
                break;

            case WSASDK.UWP:
                contents = string.Format(GetAssemblyCSharpTemplateUWP(MetroVisualStudioSolutionHelper.GetUWPSDKVersion()), new object[] { assemblyName, assemblyCSharpConfigs, builder, files, postTargets, project.Guid, preTargets });
                break;

            default:
                throw new Exception(string.Format("Unknown Windows Store Apps SDK: {0}", wsaSDK));
        }
        if (File.Exists(project.Path))
        {
            File.SetAttributes(project.Path, FileAttributes.Normal);
        }
        string directoryName = Path.GetDirectoryName(project.Path);
        Directory.CreateDirectory(directoryName);
        File.WriteAllText(project.Path, contents, Encoding.UTF8);
        if (wsaSDK == WSASDK.UWP)
        {
            WriteProjectJSON(playerPackage, directoryName);
        }
    }

    private static string GenerateConfigWithPlatformTarget(string configuration, string architecture, string platformTarget, string sdkTarget, string defines)
    {
        bool flag = configuration == "Debug";
        string str = (sdkTarget != "UWP") ? FileUtil.CombinePaths(new string[] { "bin", sdkTarget, architecture, configuration }) : FileUtil.CombinePaths(new string[] { "bin", architecture, configuration });
        string str2 = (sdkTarget != "UWP") ? FileUtil.CombinePaths(new string[] { "obj", sdkTarget, architecture, configuration }) : FileUtil.CombinePaths(new string[] { "obj", architecture, configuration });
        return string.Format(GetConfigTemplate(), new object[] { configuration + "|" + architecture, !flag ? "pdbonly" : "full", !flag ? "true" : "false", str, str2, defines, platformTarget, (platformTarget != "AnyCPU") ? "true" : "false" });
    }

    internal static string GetAssemblyCSharpConfigs(WSASDK wsaSDK, IEnumerable<string> defines)
    {
        string str;
        string str2 = string.Empty;
        if (defines == null)
        {
            str2 = "NETFX_CORE;UNITY_METRO;ENABLE_WWW;ENABLE_UNITYWEBREQUEST;UNITY_WINRT;ENABLE_AUDIO_FMOD;ENABLE_PHYSICS;ENABLE_TERRAIN;ENABLE_CACHING;ENABLE_GENERICS;ENABLE_MOVIES;ENABLE_AUDIO";
            str2 = str2 + ";" + PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.WSA);
        }
        else
        {
            foreach (string str3 in defines)
            {
                if (((str3 != "UNITY_METRO_8_1") && (str3 != "UNITY_WSA_8_1")) && ((str3 != "UNITY_WP_8_1") && (str3 != "UNITY_WSA_10_0")))
                {
                    str2 = str2 + str3 + ";";
                }
            }
            str2 = str2 + "NETFX_CORE;";
        }
        switch (wsaSDK)
        {
            case WSASDK.SDK81:
                str2 = str2 + "UNITY_METRO_8_1;UNITY_WSA_8_1;";
                str = "Store 8.1";
                break;

            case WSASDK.PhoneSDK81:
                str2 = str2 + "UNITY_WP_8_1;";
                str = "Phone 8.1";
                break;

            case WSASDK.UWP:
                str2 = str2 + "WINDOWS_UWP;UNITY_UWP;UNITY_WSA_10_0;";
                str = "UWP";
                break;

            default:
                throw new Exception(string.Format("Unknown Windows Store Apps SDK: {0}", wsaSDK));
        }
        string str4 = "DEBUG;TRACE;ENABLE_PROFILER;" + str2;
        string str5 = "TRACE;ENABLE_PROFILER;" + str2;
        string str6 = "TRACE;" + str2;
        string str7 = "";
        str7 = (str7 + GenerateConfigWithPlatformTarget(availableConfigs[0], "x86", "x86", str, str4)) + GenerateConfigWithPlatformTarget(availableConfigs[1], "x86", "x86", str, str5) + GenerateConfigWithPlatformTarget(availableConfigs[2], "x86", "x86", str, str6);
        if (wsaSDK == WSASDK.UWP)
        {
            str7 = (str7 + GenerateConfigWithPlatformTarget(availableConfigs[0], "x64", "x64", str, str4)) + GenerateConfigWithPlatformTarget(availableConfigs[1], "x64", "x64", str, str5) + GenerateConfigWithPlatformTarget(availableConfigs[2], "x64", "x64", str, str6);
        }
        return ((str7 + GenerateConfigWithPlatformTarget(availableConfigs[0], "ARM", "ARM", str, str4)) + GenerateConfigWithPlatformTarget(availableConfigs[1], "ARM", "ARM", str, str5) + GenerateConfigWithPlatformTarget(availableConfigs[2], "ARM", "ARM", str, str6));
    }

    internal static string GetAssemblyCSharpFirstpassGuid(WSASDK wsaSDK)
    {
        if (wsaSDK == WSASDK.PhoneSDK81)
        {
            return assemblyCSharpFirstpassGUIDs[1];
        }
        return assemblyCSharpFirstpassGUIDs[0];
    }

    internal static string GetAssemblyCSharpGuid(WSASDK wsaSDK)
    {
        if (wsaSDK == WSASDK.PhoneSDK81)
        {
            return assemblyCSharpGUIDs[1];
        }
        return assemblyCSharpGUIDs[0];
    }

    internal static string GetAssemblyCSharpTemplate81()
    {
        string[] textArray1 = new string[] { 
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>", "<Project ToolsVersion=\"12.0\" DefaultTargets=\"Build\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">", "{6}", "  <Import Project=\"$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props\" Condition=\"Exists('$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props')\" />", "  <PropertyGroup>", "    <RootNamespace>AssemblyCSharpWSA</RootNamespace>", "    <Configuration Condition=\" '$(Configuration)' == '' \">Debug</Configuration>", "    <Platform Condition=\" '$(Platform)' == '' \">AnyCPU</Platform>", "    <ProductVersion>10.0.20506</ProductVersion>", "    <SchemaVersion>2.0</SchemaVersion>", "    <ProjectGuid>{{{5}}}</ProjectGuid>", "    <OutputType>Library</OutputType>", "    <AppDesignerFolder>Properties</AppDesignerFolder>", "    <AssemblyName>{0}</AssemblyName>", "    <FileAlignment>512</FileAlignment>", "    <ProjectTypeGuids>{{BC8A1FFA-BEE3-4634-8014-F334798102B3}};{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}</ProjectTypeGuids>",
            "    <RootNamespace>AssemblyCSharpmetro</RootNamespace>", "    <DefaultLanguage>en-US</DefaultLanguage>", "    <TargetPlatformVersion>8.1</TargetPlatformVersion>", "    <MinimumVisualStudioVersion>12</MinimumVisualStudioVersion>", "    <TargetFrameworkVersion />", "  </PropertyGroup>", "  {1}", "  <ItemGroup>", "    <Reference Include=\"System\" />", "    <Reference Include=\"System.XML\" />", "    <Reference Include=\"System.Core\" />{2}", "  </ItemGroup>", "  <ItemGroup>", "{3}  </ItemGroup>", "  <Import Project=\"$(MSBuildExtensionsPath)\\Microsoft\\WindowsXaml\\v$(VisualStudioVersion)\\Microsoft.Windows.UI.Xaml.CSharp.targets\" />", "{4}",
            "</Project>"
        };
        return string.Join("\r\n", textArray1);
    }

    internal static string GetAssemblyCSharpTemplatePhone81()
    {
        string[] textArray1 = new string[] { 
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>", "<Project ToolsVersion=\"12.0\" DefaultTargets=\"Build\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">", "{6}", "  <Import Project=\"$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props\" Condition=\"Exists('$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props')\" />", "  <PropertyGroup>", "    <Configuration Condition=\" '$(Configuration)' == '' \">Debug</Configuration>", "    <Platform Condition=\" '$(Platform)' == '' \">AnyCPU</Platform>", "    <ProductVersion>8.0.30703</ProductVersion>", "    <SchemaVersion>2.0</SchemaVersion>", "    <ProjectGuid>{{{5}}}</ProjectGuid>", "    <OutputType>Library</OutputType>", "    <AppDesignerFolder>Properties</AppDesignerFolder>", "    <AssemblyName>{0}</AssemblyName>", "    <FileAlignment>512</FileAlignment>", "    <ProjectTypeGuids>{{76F1466A-8B6D-4E39-A767-685A06062A39}};{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}</ProjectTypeGuids>", "    <RootNamespace>AssemblyCSharpWSA</RootNamespace>",
            "    <DefaultLanguage>en-US</DefaultLanguage>", "    <TargetPlatformVersion>8.1</TargetPlatformVersion>", "    <MinimumVisualStudioVersion>12</MinimumVisualStudioVersion>", "    <TargetFrameworkVersion />", "  </PropertyGroup>", "    {1}", "  <ItemGroup>", "    <!-- A reference to the entire .Net Framework and Windows SDK are automatically included -->{2}", "  </ItemGroup>", "  <ItemGroup>", "{3}  </ItemGroup>", "  <PropertyGroup Condition=\" '$(TargetPlatformIdentifier)' == '' \">", "    <TargetPlatformIdentifier>WindowsPhoneApp</TargetPlatformIdentifier>", "  </PropertyGroup>", "  <Import Project=\"$(MSBuildExtensionsPath)\\Microsoft\\WindowsXaml\\v$(VisualStudioVersion)\\Microsoft.Windows.UI.Xaml.CSharp.targets\" />", "{4}",
            "</Project>"
        };
        return string.Join("\r\n", textArray1);
    }

    internal static string GetAssemblyCSharpTemplateUWP(string version)
    {
        string[] textArray1 = new string[] { 
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>", "<Project ToolsVersion=\"14.0\" DefaultTargets=\"Build\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">", "{6}", "  <Import Project=\"$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props\" Condition=\"Exists('$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props')\" />", "  <PropertyGroup>", "    <Configuration Condition=\" '$(Configuration)' == '' \">Debug</Configuration>", "    <Platform Condition=\" '$(Platform)' == '' \">AnyCPU</Platform>", "    <ProjectGuid>{{{5}}}</ProjectGuid>", "    <OutputType>Library</OutputType>", "    <AppDesignerFolder>Properties</AppDesignerFolder>", "    <RootNamespace>AssemblyCSharpWSA</RootNamespace>", "    <AssemblyName>{0}</AssemblyName>", "    <DefaultLanguage>en-US</DefaultLanguage>", "    <TargetPlatformIdentifier>UAP</TargetPlatformIdentifier>", "    <TargetPlatformVersion>" + version + "</TargetPlatformVersion>", "    <TargetPlatformMinVersion>10.0.10240.0</TargetPlatformMinVersion>",
            "    <MinimumVisualStudioVersion>14</MinimumVisualStudioVersion>", "    <FileAlignment>512</FileAlignment>", "    <ProjectTypeGuids>{{A5A43C5B-DE2A-4C0C-9213-0A381AF9435A}};{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}</ProjectTypeGuids>", "  </PropertyGroup>", "{1}  <ItemGroup>", "    <None Include=\"project.json\" />", "  </ItemGroup>", "  <ItemGroup>", "{2}", "  </ItemGroup>", "  <ItemGroup>", "{3}  </ItemGroup>", "  <PropertyGroup Condition=\" '$(VisualStudioVersion)' == '' or '$(VisualStudioVersion)' &lt; '14.0' \">", "    <VisualStudioVersion>14.0</VisualStudioVersion>", "  </PropertyGroup>", "  <Import Project=\"$(MSBuildExtensionsPath)\\Microsoft\\WindowsXaml\\v$(VisualStudioVersion)\\Microsoft.Windows.UI.Xaml.CSharp.targets\" />",
            "  {4}", "  <!-- To modify your build process, add your task inside one of the targets below and uncomment it. ", "       Other similar extension points exist, see Microsoft.Common.targets.", "  <Target Name=\"BeforeBuild\">", "  </Target>", "  <Target Name=\"AfterBuild\">", "  </Target>", "  -->", "</Project>"
        };
        return string.Join("\r\n", textArray1);
    }

    internal static string GetConfigTemplate()
    {
        string[] textArray1 = new string[] { "  <PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == '{0}' \">", "    <DebugSymbols>true</DebugSymbols>", "    <DebugType>{1}</DebugType>", "    <Optimize>{2}</Optimize>", @"    <OutputPath>{3}\</OutputPath>", @"    <BaseIntermediateOutputPath>{4}\</BaseIntermediateOutputPath>", "    <DefineConstants>{5}</DefineConstants>", "    <PlatformTarget>{6}</PlatformTarget>", "    <UseVSHostingProcess>false</UseVSHostingProcess>", "    <ErrorReport>prompt</ErrorReport>", "    <Prefer32Bit>{7}</Prefer32Bit>", "  </PropertyGroup>", "" };
        return string.Join("\r\n", textArray1);
    }

    private static string GetRelativeFinalProjectDirectory(WSASDK wsaSDK)
    {
        if (EditorUserBuildSettings.wsaSDK == WSASDK.UniversalSDK81)
        {
            if (wsaSDK != WSASDK.SDK81)
            {
                if (wsaSDK != WSASDK.PhoneSDK81)
                {
                    throw new Exception(string.Format("Invalid Windows Store Apps SDK in universal app: {0}", wsaSDK));
                }
            }
            else
            {
                return @"$(UnityWSASolutionDir)$(UnityWSASolutionName)\$(UnityWSASolutionName).Windows\";
            }
            return @"$(UnityWSASolutionDir)$(UnityWSASolutionName)\$(UnityWSASolutionName).WindowsPhone\";
        }
        return @"$(UnityWSASolutionDir)$(UnityWSASolutionName)\";
    }

    private static string GetSerializationWeaverTargets(string finalProjectDir, bool includeUnet, string additionalReferencePath)
    {
        XElement element = new XElement("PropertyGroup", new XElement("TargetsTriggeredByCompilation", "$(TargetsTriggeredByCompilation);RunSerializationWeaver"));
        string str = !includeUnet ? string.Empty : string.Format("-unity-networking=\"{0}Unprocessed\\UnityEngine.Networking.dll\"", finalProjectDir);
        string[] textArray1 = new string[] { "\"$(UnityWSAToolsDir)SerializationWeaver\\SerializationWeaver.exe\" \"$(TargetDir)Unprocessed\\$(TargetFileName)\" -pdb -verbose -unity-engine=\"", finalProjectDir, "Unprocessed\\UnityEngine.dll\" ", str, " ", additionalReferencePath, " \"$([System.IO.Path]::GetFullPath('$(IntermediateOutputPath)').TrimEnd('\\'))\"" };
        string str2 = string.Concat(textArray1);
        object[] content = new object[7];
        content[0] = new XAttribute("Name", "RunSerializationWeaver");
        content[1] = new XAttribute("Condition", "'$(BuildingProject)' == 'true'");
        object[] objArray2 = new object[] { new XAttribute("Importance", "high"), new XAttribute("Text", "Running SerializationWeaver...") };
        content[2] = new XElement("Message", objArray2);
        object[] objArray3 = new object[] { new XAttribute("SourceFiles", "@(IntermediateAssembly)"), new XAttribute("DestinationFolder", "$(TargetDir)Unprocessed"), new XAttribute("OverwriteReadOnlyFiles", "true") };
        content[3] = new XElement("Move", objArray3);
        object[] objArray4 = new object[] { new XAttribute("SourceFiles", "@(IntermediateAssembly->'%(RelativeDir)%(Filename).pdb')"), new XAttribute("DestinationFolder", "$(TargetDir)Unprocessed"), new XAttribute("Condition", "Exists('@(IntermediateAssembly->'%(RelativeDir)%(Filename).pdb')')"), new XAttribute("OverwriteReadOnlyFiles", "true") };
        content[4] = new XElement("Move", objArray4);
        content[5] = new XElement("Exec", new XAttribute("Command", str2));
        object[] objArray5 = new object[] { new XAttribute("Importance", "high"), new XAttribute("Text", "SerializationWeaver done.") };
        content[6] = new XElement("Message", objArray5);
        XElement element2 = new XElement("Target", content);
        XElement element3 = new XElement("PropertyGroup", new XElement("CleanDependsOn", "$(CleanDependsOn);CleanSerializationWeaver"));
        object[] objArray6 = new object[] { new XAttribute("Name", "CleanSerializationWeaver"), new XElement("Delete", new XAttribute("Files", @"$(TargetDir)Unprocessed\$(TargetFileName)")), new XElement("Delete", new XAttribute("Files", @"$(TargetDir)Unprocessed\$(TargetName).pdb")) };
        XElement element4 = new XElement("Target", objArray6);
        return Indent(string.Format("{1}{0}{2}{0}{3}{0}{4}", new object[] { Environment.NewLine, element, element2, element3, element4 }), 2);
    }

    private static string Indent(string text, int value)
    {
        StringBuilder builder = new StringBuilder();
        string[] separator = new string[] { Environment.NewLine };
        string[] strArray = text.Split(separator, StringSplitOptions.None);
        for (int i = 0; i < strArray.Length; i++)
        {
            builder.Append(' ', value);
            builder.Append(strArray[i]);
            if (i != (strArray.Length - 1))
            {
                builder.AppendLine();
            }
        }
        return builder.ToString();
    }

    internal static void ResetAssemblyCSharpProjectGUIDs()
    {
        Guid guid = Guid.NewGuid();
        Guid guid2 = Guid.NewGuid();
        assemblyCSharpGUIDs = new string[] { guid.ToString("D"), guid2.ToString("D") };
        guid = Guid.NewGuid();
        guid2 = Guid.NewGuid();
        assemblyCSharpFirstpassGUIDs = new string[] { guid.ToString("D"), guid2.ToString("D") };
    }

    internal static void WriteProjectJSON(string playerPackage, string projectDirectory)
    {
        string path = Utility.CombinePath(projectDirectory, "project.json");
        if (!File.Exists(path))
        {
            File.Copy(Utility.CombinePath(playerPackage, @"Tools\project.json"), path);
        }
    }

    public class AssemblyCSharpPlugin
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <HintPath>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <Name>k__BackingField;

        public string HintPath { get; set; }

        public string Name { get; set; }
    }
}

