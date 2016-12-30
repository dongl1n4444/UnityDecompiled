namespace UnityEditor.Android
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Xml.Linq;
    using UnityEditor;
    using UnityEditor.Android.PostProcessor.Tasks;
    using UnityEditor.Utils;
    using UnityEngine;

    internal class AndroidProjectExportVisualStudioGradle : AndroidProjectExport
    {
        [CompilerGenerated]
        private static Func<AndroidTargetDeviceType, string> <>f__am$cache0;
        private static readonly bool k_ExperimentalPlugin = true;
        private static readonly Version k_GradleVersion = new Version(2, 14, 1);
        private static readonly Version k_PluginVersion = new Version(0, 8, 2);
        private readonly XNamespace m_XmlNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

        private void CopySymbols(string projectSymbolsDir, string projectJniLibsDir)
        {
            if (base.m_ScriptingBackend == ScriptingImplementation.IL2CPP)
            {
                foreach (string str in Directory.GetDirectories(projectJniLibsDir))
                {
                    string path = Path.Combine(projectSymbolsDir, Path.GetFileName(str));
                    string str3 = Path.Combine(str, "libil2cpp.so.debug");
                    if (File.Exists(str3))
                    {
                        Directory.CreateDirectory(path);
                        string str4 = Path.Combine(path, "libil2cpp.so");
                        File.Delete(str4);
                        File.Move(str3, str4);
                    }
                }
            }
            if (!base.m_SourceBuild)
            {
                Directory.CreateDirectory(projectSymbolsDir);
                string symbolsDirectory = TasksCommon.GetSymbolsDirectory(base.m_Context);
                foreach (string str6 in Directory.GetDirectories(symbolsDirectory))
                {
                    string str7 = Path.Combine(projectSymbolsDir, Path.GetFileName(str6));
                    Directory.CreateDirectory(str7);
                    Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                    foreach (string str8 in Directory.GetFiles(str6, "*.sym.so"))
                    {
                        string fileName = Path.GetFileName(str8);
                        fileName = fileName.Substring(0, fileName.LastIndexOf(".sym.so"));
                        dictionary.Add(fileName, str8);
                    }
                    foreach (string str10 in Directory.GetFiles(str6, "*.dbg.so"))
                    {
                        string str11 = Path.GetFileName(str10);
                        str11 = str11.Substring(0, str11.LastIndexOf(".dbg.so"));
                        dictionary[str11] = str10;
                    }
                    foreach (KeyValuePair<string, string> pair in dictionary)
                    {
                        File.Copy(pair.Value, Path.Combine(str7, pair.Key + ".so"));
                    }
                }
            }
        }

        public override void ExportWithCurrentSettings()
        {
            <ExportWithCurrentSettings>c__AnonStorey1 storey = new <ExportWithCurrentSettings>c__AnonStorey1();
            if (base.m_TargetPath == null)
            {
                throw new ArgumentNullException("m_TargetPath");
            }
            string targetPath = base.m_TargetPath;
            string solutionFilePath = Path.Combine(targetPath, base.m_ProductName + ".sln");
            storey.projectPath = Path.Combine(targetPath, base.m_ProductName);
            string[] components = new string[] { storey.projectPath, base.m_ProductName + ".androidproj" };
            string androidPropjectFilePath = Paths.Combine(components);
            string[] textArray2 = new string[] { storey.projectPath, base.m_ProductName + ".androidproj.user" };
            string androidPropjectUserFilePath = Paths.Combine(textArray2);
            string[] textArray3 = new string[] { storey.projectPath, "app", "src", "main" };
            string str5 = Paths.Combine(textArray3);
            string[] textArray4 = new string[] { storey.projectPath, "app", "libs" };
            string target = Paths.Combine(textArray4);
            string str7 = Path.Combine(str5, "jniLibs");
            string[] textArray5 = new string[] { storey.projectPath, "app", "symbols" };
            string path = Paths.Combine(textArray5);
            Directory.CreateDirectory(targetPath);
            Directory.CreateDirectory(storey.projectPath);
            Directory.CreateDirectory(Path.Combine(storey.projectPath, "app"));
            if (!base.m_SourceBuild)
            {
                string[] textArray6 = new string[] { target, "unity-classes.jar" };
                AndroidProjectExport.CopyFile(base.m_UnityJavaLibrary, Paths.Combine(textArray6));
            }
            string[] textArray7 = new string[] { str5, "assets", "bin", "Data" };
            string str9 = Paths.Combine(textArray7);
            try
            {
                Directory.Delete(str9, true);
            }
            catch (IOException exception)
            {
                UnityEngine.Debug.LogWarning($"Failed to delete '{str9}' directory ({exception.Message}).");
            }
            string[] textArray8 = new string[] { str5, "res" };
            string str10 = Paths.Combine(textArray8);
            try
            {
                Directory.Delete(str10, true);
            }
            catch (IOException exception2)
            {
                UnityEngine.Debug.LogWarning($"Failed to delete '{str10}' directory ({exception2.Message}).");
            }
            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException exception3)
            {
                UnityEngine.Debug.LogWarning($"Failed to delete '{path}' directory ({exception3.Message}).");
            }
            AndroidProjectExport.CopyDir(Path.Combine(base.m_StagingArea, "res"), Path.Combine(str5, "res"));
            AndroidProjectExport.CopyDir(Path.Combine(base.m_StagingArea, "plugins"), target);
            AndroidProjectExport.CopyDir(Path.Combine(base.m_StagingArea, "libs"), str7);
            AndroidProjectExport.CopyDir(Path.Combine(base.m_StagingArea, "assets"), Path.Combine(str5, "assets"));
            this.CopySymbols(path, str7);
            this.WriteGradleBuildFiles(storey.projectPath);
            AndroidProjectExport.GenerateAndroidManifest(Path.Combine(str5, "AndroidManifest.xml.template"), base.m_StagingArea, base.m_PackageName, false);
            string str11 = Path.Combine(str5, "java");
            AndroidProjectExport.CopyAndPatchJavaSources(str11, base.m_UnityJavaSources, base.m_PackageName);
            string[] javaFilePaths = Enumerable.Select<string, string>(Directory.GetFiles(str11, "*.java", SearchOption.AllDirectories), new Func<string, string>(storey.<>m__0)).ToArray<string>();
            string projectGuid = Guid.NewGuid().ToString("B").ToUpperInvariant();
            this.WriteSolutionFile(solutionFilePath, base.m_ProductName, Guid.NewGuid().ToString("B").ToUpperInvariant(), projectGuid);
            this.WriteAndroidProjectFile(androidPropjectFilePath, base.m_ProductName, projectGuid, javaFilePaths);
            this.WriteAndroidProjectUserFile(androidPropjectUserFilePath);
        }

        private string GetBuildTypes()
        {
            if (!base.m_SourceBuild)
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            foreach (string str2 in this.GetConfigs())
            {
                builder.AppendLine();
                builder.AppendFormat("        create('{0}') {{", str2);
                builder.AppendLine();
                if (str2.StartsWith("Release_"))
                {
                    builder.AppendLine("            minifyEnabled = false");
                    builder.AppendLine("            proguardFiles.add(file('proguard-rules.txt'))");
                    builder.AppendLine("            ndk.debuggable = true");
                    builder.AppendLine("            debuggable = true");
                }
                else
                {
                    builder.AppendLine("            ndk.debuggable = true");
                    builder.AppendLine("            debuggable = true");
                    builder.AppendLine("            signingConfig = $(\"android.signingConfigs.debug\")");
                }
                builder.Append("        }");
            }
            return builder.ToString();
        }

        [DebuggerHidden]
        private IEnumerable<string> GetConfigs() => 
            new <GetConfigs>c__Iterator0 { 
                $this = this,
                $PC = -2
            };

        private string GetConfigurations()
        {
            if (!base.m_SourceBuild)
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine("configurations {");
            foreach (string str2 in ScriptingImplementations)
            {
                foreach (string str3 in this.SimpleConfigs)
                {
                    builder.AppendFormat("    {0}_{1}Compile", str3, str2);
                    builder.AppendLine();
                }
            }
            builder.Append("}");
            return builder.ToString();
        }

        private string GetDependencies()
        {
            if (!base.m_SourceBuild)
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            foreach (string str2 in ScriptingImplementations)
            {
                foreach (string str3 in this.SimpleConfigs)
                {
                    builder.AppendLine();
                    builder.AppendFormat("    '{0}_{1}Compile' fileTree(dir: '../../../../Variations/{2}/{0}/Classes', include: ['*.jar'])", str3, str2, str2.ToLowerInvariant());
                }
            }
            return builder.ToString();
        }

        private string GetSources()
        {
            if (!base.m_SourceBuild)
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine("    android.sources {");
            foreach (string str2 in ScriptingImplementations)
            {
                foreach (string str3 in this.SimpleConfigs)
                {
                    foreach (AndroidTargetDeviceType type in DeviceTypes)
                    {
                        builder.AppendFormat("        {0}{1}_{2} {{", type.GradleProductFlavor, str3, str2);
                        builder.AppendLine();
                        builder.AppendLine("            jniLibs {");
                        builder.AppendLine("                source {");
                        builder.AppendFormat("                    srcDirs = ['../../../../Variations/{0}/{1}/Libs']", str2.ToLowerInvariant(), str3);
                        builder.AppendLine();
                        builder.AppendLine("                }");
                        builder.AppendLine("            }");
                        builder.AppendLine("        }");
                    }
                }
            }
            builder.Append("    }");
            return builder.ToString();
        }

        private void WriteAndroidProjectFile(string androidPropjectFilePath, string productName, string projectGuid, string[] javaFilePaths)
        {
            XDeclaration declaration = new XDeclaration("1.0", "utf-8", null);
            XDocument document = new XDocument(declaration, new object[0]);
            object[] content = new object[] { new XAttribute("DefaultTargets", "Build"), new XAttribute("ToolsVersion", "14.0") };
            XElement element = new XElement((XName) (this.m_XmlNamespace + "Project"), content);
            document.Add(element);
            XElement element2 = new XElement((XName) (this.m_XmlNamespace + "ItemGroup"), new XAttribute("Label", "ProjectConfigurations"));
            foreach (AndroidTargetDeviceType type in DeviceTypes)
            {
                foreach (string str in this.GetConfigs())
                {
                    XElement element3 = new XElement((XName) (this.m_XmlNamespace + "ProjectConfiguration"), new XAttribute("Include", $"{str}|{type.VisualStudioPlatform}"));
                    element3.Add(new XElement((XName) (this.m_XmlNamespace + "Configuration"), str));
                    element3.Add(new XElement((XName) (this.m_XmlNamespace + "Platform"), type.VisualStudioPlatform));
                    element2.Add(element3);
                }
            }
            element.Add(element2);
            XElement element4 = new XElement((XName) (this.m_XmlNamespace + "PropertyGroup"), new XAttribute("Label", "Globals"));
            element4.Add(new XElement((XName) (this.m_XmlNamespace + "AndroidBuildType"), "Gradle"));
            element4.Add(new XElement((XName) (this.m_XmlNamespace + "RootNamespace"), productName));
            element4.Add(new XElement((XName) (this.m_XmlNamespace + "MinimumVisualStudioVersion"), "14.0"));
            element4.Add(new XElement((XName) (this.m_XmlNamespace + "ProjectVersion"), "1.0"));
            element4.Add(new XElement((XName) (this.m_XmlNamespace + "ProjectGuid"), projectGuid));
            element4.Add(new XElement((XName) (this.m_XmlNamespace + "_PackagingProjectWithoutNativeComponent"), "true"));
            XElement element5 = new XElement((XName) (this.m_XmlNamespace + "LaunchActivity"), new XAttribute("Condition", "'$(LaunchActivity)' == ''"));
            element5.SetValue(base.m_PackageName + ".UnityPlayerActivity");
            element4.Add(element5);
            element4.Add(new XElement((XName) (this.m_XmlNamespace + "JavaSourceRoots"), @"src\main\java"));
            element.Add(element4);
            element.Add(new XElement((XName) (this.m_XmlNamespace + "Import"), new XAttribute("Project", @"$(AndroidTargetsPath)\Android.Default.props")));
            foreach (AndroidTargetDeviceType type2 in DeviceTypes)
            {
                foreach (string str2 in this.GetConfigs())
                {
                    object[] objArray2 = new object[] { new XAttribute("Condition", $"'$(Configuration)|$(Platform)'=='{str2}|{type2.VisualStudioPlatform}'"), new XAttribute("Label", "Configuration") };
                    XElement element6 = new XElement((XName) (this.m_XmlNamespace + "PropertyGroup"), objArray2);
                    element6.Add(new XElement((XName) (this.m_XmlNamespace + "ConfigurationType"), "Application"));
                    element6.Add(new XElement((XName) (this.m_XmlNamespace + "AndroidAPILevel"), "android-" + base.m_TargetSDKVersion));
                    element.Add(element6);
                }
            }
            element.Add(new XElement((XName) (this.m_XmlNamespace + "Import"), new XAttribute("Project", @"$(AndroidTargetsPath)\Android.props")));
            object[] objArray3 = new object[3];
            objArray3[0] = new XElement((XName) (this.m_XmlNamespace + "ProjectDirectory"), "$(ProjectDir)app");
            object[] objArray4 = new object[] { "gradle", !k_ExperimentalPlugin ? string.Empty : "-experimental", ':', k_PluginVersion };
            objArray3[1] = new XElement((XName) (this.m_XmlNamespace + "GradlePlugin"), string.Concat(objArray4));
            objArray3[2] = new XElement((XName) (this.m_XmlNamespace + "GradleVersion"), k_GradleVersion);
            XElement element7 = new XElement((XName) (this.m_XmlNamespace + "GradlePackage"), objArray3);
            if (base.m_SourceBuild)
            {
                foreach (string str3 in this.GetConfigs())
                {
                    string str4 = !str3.StartsWith("Release_") ? string.Empty : "-unsigned";
                    foreach (AndroidTargetDeviceType type3 in DeviceTypes)
                    {
                        object[] objArray5 = new object[] { new XAttribute("Condition", $"'$(Configuration)'=='{str3}' and '$(Platform)'=='{type3.VisualStudioPlatform}'"), $"app-{type3.GradleProductFlavor}-$(Configuration){str4}.apk" };
                        element7.Add(new XElement((XName) (this.m_XmlNamespace + "ApkFileName"), objArray5));
                    }
                }
            }
            element.Add(new XElement((XName) (this.m_XmlNamespace + "ItemDefinitionGroup"), element7));
            element.Add(new XElement((XName) (this.m_XmlNamespace + "ImportGroup"), new XAttribute("Label", "ExtensionSettings")));
            element.Add(new XElement((XName) (this.m_XmlNamespace + "PropertyGroup"), new XAttribute("Label", "UserMacros")));
            object[] objArray6 = new object[] { new XElement((XName) (this.m_XmlNamespace + "GradleTemplate"), new XAttribute("Include", @"app\build.gradle.template")), new XElement((XName) (this.m_XmlNamespace + "GradleTemplate"), new XAttribute("Include", @"app\src\main\AndroidManifest.xml.template")), new XElement((XName) (this.m_XmlNamespace + "GradleTemplate"), new XAttribute("Include", "build.gradle.template")), new XElement((XName) (this.m_XmlNamespace + "GradleTemplate"), new XAttribute("Include", "settings.gradle.template")), new XElement((XName) (this.m_XmlNamespace + "GradleTemplate"), new XAttribute("Include", @"gradle\wrapper\gradle-wrapper.properties.template")) };
            element.Add(new XElement((XName) (this.m_XmlNamespace + "ItemGroup"), objArray6));
            List<object> list = new List<object>();
            foreach (string str5 in javaFilePaths)
            {
                list.Add(new XElement((XName) (this.m_XmlNamespace + "JavaCompile"), new XAttribute("Include", str5)));
            }
            element.Add(new XElement((XName) (this.m_XmlNamespace + "ItemGroup"), list.ToArray()));
            object[] objArray7 = new object[] { new XElement((XName) (this.m_XmlNamespace + "None"), new XAttribute("Include", @"app\src\main\assets\**")), new XElement((XName) (this.m_XmlNamespace + "None"), new XAttribute("Include", @"app\src\main\jniLibs\**")), new XElement((XName) (this.m_XmlNamespace + "None"), new XAttribute("Include", @"app\src\main\res\**")) };
            element.Add(new XElement((XName) (this.m_XmlNamespace + "ItemGroup"), objArray7));
            element.Add(new XElement((XName) (this.m_XmlNamespace + "Import"), new XAttribute("Project", @"$(AndroidTargetsPath)\Android.targets")));
            element.Add(new XElement((XName) (this.m_XmlNamespace + "ImportGroup"), new XAttribute("Label", "ExtensionTargets")));
            document.Save(androidPropjectFilePath);
        }

        private void WriteAndroidProjectUserFile(string androidPropjectUserFilePath)
        {
            XDeclaration declaration = new XDeclaration("1.0", "utf-8", null);
            XDocument document = new XDocument(declaration, new object[0]);
            object[] content = new object[] { new XAttribute("DefaultTargets", "Build"), new XAttribute("ToolsVersion", "14.0") };
            XElement element = new XElement((XName) (this.m_XmlNamespace + "Project"), content);
            document.Add(element);
            if (base.m_SourceBuild)
            {
                foreach (string str in ScriptingImplementations)
                {
                    foreach (string str2 in this.SimpleConfigs)
                    {
                        foreach (AndroidTargetDeviceType type in DeviceTypes)
                        {
                            XElement element2 = new XElement((XName) (this.m_XmlNamespace + "PropertyGroup"), new XAttribute("Condition", $"'$(Configuration)|$(Platform)'=='{str2}_{str}|{type.VisualStudioPlatform}'"));
                            string str3 = string.Format(@"$(ProjectDir)..\..\..\Variations\{0}\{1}\Symbols\{2};$(ProjectDir)app\symbols\{2};$(AdditionalSymbolSearchPaths)", str.ToLowerInvariant(), str2, type.ABI);
                            element2.Add(new XElement((XName) (this.m_XmlNamespace + "AdditionalSymbolSearchPaths"), str3));
                            element2.Add(new XElement((XName) (this.m_XmlNamespace + "DebuggerFlavor"), "AndroidDebugger"));
                            element.Add(element2);
                        }
                    }
                }
            }
            else
            {
                foreach (string str4 in this.GetConfigs())
                {
                    foreach (AndroidTargetDeviceType type2 in DeviceTypes)
                    {
                        XElement element3 = new XElement((XName) (this.m_XmlNamespace + "PropertyGroup"), new XAttribute("Condition", $"'$(Configuration)|$(Platform)'=='{str4}|{type2.VisualStudioPlatform}'"));
                        string str5 = $"$(ProjectDir)app\symbols\{type2.ABI};$(AdditionalSymbolSearchPaths)";
                        element3.Add(new XElement((XName) (this.m_XmlNamespace + "AdditionalSymbolSearchPaths"), str5));
                        element3.Add(new XElement((XName) (this.m_XmlNamespace + "DebuggerFlavor"), "AndroidDebugger"));
                        element.Add(element3);
                    }
                }
            }
            document.Save(androidPropjectUserFilePath);
        }

        private void WriteGradleBuildFiles(string projectPath)
        {
            string str = Path.Combine(BuildPipeline.GetBuildToolsDirectory(BuildTarget.Android), "VisualStudioGradleTemplates");
            File.Copy(Path.Combine(str, "root-build.gradle.template"), Path.Combine(projectPath, "build.gradle.template"), true);
            File.Copy(Path.Combine(str, "settings.gradle.template"), Path.Combine(projectPath, "settings.gradle.template"), true);
            File.Copy(Path.Combine(str, "gradlew.bat"), Path.Combine(projectPath, "gradlew.bat"), true);
            string path = Path.Combine(projectPath, @"gradle\wrapper");
            Directory.CreateDirectory(path);
            File.Copy(Path.Combine(str, "gradle-wrapper.jar"), Path.Combine(path, "gradle-wrapper.jar"), true);
            File.Copy(Path.Combine(str, "gradle-wrapper.properties.template"), Path.Combine(path, "gradle-wrapper.properties.template"), true);
            string contents = File.ReadAllText(Path.Combine(str, "build.gradle.template")).Replace("$(UNITY_MINSDKVERSION)", PlayerSettings.Android.minSdkVersion.ToString()).Replace("$(UNITY_TARGETSDKVERSION)", this.m_TargetSDKVersion.ToString()).Replace("$(UNITY_BUILDTYPES)", this.GetBuildTypes()).Replace("$(UNITY_SOURCES)", this.GetSources()).Replace("$(UNITY_CONFIGURATIONS)", this.GetConfigurations()).Replace("$(UNITY_DEPENDENCIES)", this.GetDependencies());
            File.WriteAllText(Path.Combine(projectPath, @"app\build.gradle.template"), contents);
        }

        private void WriteSolutionFile(string solutionFilePath, string productName, string solutionGuid, string projectGuid)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Microsoft Visual Studio Solution File, Format Version 12.00");
            builder.AppendLine("# Visual Studio 14");
            builder.AppendLine("VisualStudioVersion = 14.0.25420.1");
            builder.AppendLine("MinimumVisualStudioVersion = 10.0.40219.1");
            string str = Guid.NewGuid().ToString("B").ToUpperInvariant();
            if (base.m_SourceBuild)
            {
                builder.AppendFormat("Project(\"{0}\") = \"AndroidPlayer\", \"..\\..\\..\\..\\Projects\\Android\\AndroidPlayer.vcxproj\", \"{1}\"", str, "{38A75A80-2A29-4FB2-AE54-4BBF5BF6586D}");
                builder.AppendLine();
                builder.AppendLine("EndProject");
            }
            builder.AppendFormat("Project(\"{0}\") = \"{1}\", \"{1}\\{1}.androidproj\", \"{2}\"", solutionGuid, productName, projectGuid);
            builder.AppendLine();
            if (base.m_SourceBuild)
            {
                builder.AppendLine("\tProjectSection(ProjectDependencies) = postProject");
                builder.AppendFormat("\t\t{0} = {0}", "{38A75A80-2A29-4FB2-AE54-4BBF5BF6586D}");
                builder.AppendLine();
                builder.AppendLine("\tEndProjectSection");
            }
            builder.AppendLine("EndProject");
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = t => t.VisualStudioPlatform;
            }
            List<string> list = new List<string>(Enumerable.Select<AndroidTargetDeviceType, string>(DeviceTypes, <>f__am$cache0));
            if (base.m_SourceBuild)
            {
                list.Insert(1, "x64");
            }
            builder.AppendLine("Global");
            builder.AppendLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
            foreach (string str2 in this.GetConfigs())
            {
                foreach (string str3 in list)
                {
                    builder.AppendFormat("\t\t{0}|{1} = {0}|{1}", str2, str3);
                    builder.AppendLine();
                }
            }
            builder.AppendLine("\tEndGlobalSection");
            builder.AppendLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
            if (base.m_SourceBuild)
            {
                string[] strArray = new string[] { "ActiveCfg", "Build.0" };
                foreach (string str4 in this.GetConfigs())
                {
                    foreach (string str5 in list)
                    {
                        string str6 = (str5 != "x86") ? str5 : "Win32";
                        foreach (string str7 in strArray)
                        {
                            object[] args = new object[] { "{38A75A80-2A29-4FB2-AE54-4BBF5BF6586D}", str4, str5, str7, str6 };
                            builder.AppendFormat("\t\t{0}.{1}|{2}.{3} = {1}|{4}", args);
                            builder.AppendLine();
                        }
                    }
                }
            }
            foreach (string str8 in this.GetConfigs())
            {
                foreach (string str9 in list)
                {
                    string str10;
                    List<string> list2 = new List<string> { "ActiveCfg" };
                    if (str9 != "x64")
                    {
                        list2.Add("Build.0");
                        list2.Add("Deploy.0");
                        str10 = str9;
                    }
                    else
                    {
                        str10 = "x86";
                    }
                    foreach (string str11 in list2)
                    {
                        object[] objArray2 = new object[] { projectGuid, str11, str8, str9, str10 };
                        builder.AppendFormat("\t\t{0}.{2}|{3}.{1} = {2}|{4}", objArray2);
                        builder.AppendLine();
                    }
                }
            }
            builder.AppendLine("\tEndGlobalSection");
            builder.AppendLine("\tGlobalSection(SolutionProperties) = preSolution");
            builder.AppendLine("\t\tHideSolutionNode = FALSE");
            builder.AppendLine("\tEndGlobalSection");
            builder.AppendLine("EndGlobal");
            File.WriteAllText(solutionFilePath, builder.ToString());
        }

        private static AndroidTargetDeviceType[] DeviceTypes =>
            new AndroidTargetDeviceType[] { new AndroidTargetDeviceARMv7(), new AndroidTargetDevicex86() };

        private static string[] ScriptingImplementations =>
            new string[] { "IL2CPP", "Mono" };

        private string[] SimpleConfigs =>
            (!base.m_SourceBuild ? new string[] { "Debug", "Release" } : new string[] { "Debug", "Development", "Release" });

        [CompilerGenerated]
        private sealed class <ExportWithCurrentSettings>c__AnonStorey1
        {
            internal string projectPath;

            internal string <>m__0(string p) => 
                p.Substring(this.projectPath.Length + 1);
        }

        [CompilerGenerated]
        private sealed class <GetConfigs>c__Iterator0 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal string[] $locvar0;
            internal int $locvar1;
            internal string[] $locvar2;
            internal int $locvar3;
            internal string[] $locvar4;
            internal int $locvar5;
            internal int $PC;
            internal AndroidProjectExportVisualStudioGradle $this;
            internal string <config>__1;
            internal string <config>__2;
            internal string <scriptingImplementation>__3;

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
                        if (this.$this.m_SourceBuild)
                        {
                            this.$locvar2 = this.$this.SimpleConfigs;
                            this.$locvar3 = 0;
                            while (this.$locvar3 < this.$locvar2.Length)
                            {
                                this.<config>__2 = this.$locvar2[this.$locvar3];
                                this.$locvar4 = AndroidProjectExportVisualStudioGradle.ScriptingImplementations;
                                this.$locvar5 = 0;
                                while (this.$locvar5 < this.$locvar4.Length)
                                {
                                    this.<scriptingImplementation>__3 = this.$locvar4[this.$locvar5];
                                    this.$current = this.<config>__2 + '_' + this.<scriptingImplementation>__3;
                                    if (!this.$disposing)
                                    {
                                        this.$PC = 2;
                                    }
                                    goto Label_018A;
                                Label_013E:
                                    this.$locvar5++;
                                }
                                this.$locvar3++;
                            }
                            goto Label_0181;
                        }
                        this.$locvar0 = this.$this.SimpleConfigs;
                        this.$locvar1 = 0;
                        break;

                    case 1:
                        this.$locvar1++;
                        break;

                    case 2:
                        goto Label_013E;

                    default:
                        goto Label_0188;
                }
                if (this.$locvar1 < this.$locvar0.Length)
                {
                    this.<config>__1 = this.$locvar0[this.$locvar1];
                    this.$current = this.<config>__1;
                    if (!this.$disposing)
                    {
                        this.$PC = 1;
                    }
                    goto Label_018A;
                }
            Label_0181:
                this.$PC = -1;
            Label_0188:
                return false;
            Label_018A:
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
                return new AndroidProjectExportVisualStudioGradle.<GetConfigs>c__Iterator0 { $this = this.$this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();

            string IEnumerator<string>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }
    }
}

