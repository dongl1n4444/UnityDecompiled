namespace UnityEditor.Android
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEditor.Android.PostProcessor;
    using UnityEditor.Android.PostProcessor.Tasks;
    using UnityEditor.Utils;

    internal abstract class AndroidProjectExport
    {
        protected static readonly string DefaultUnityPackage = "com.unity3d.player";
        protected AndroidLibraries m_AndroidLibraries;
        protected string m_AndroidPluginsPath;
        protected PostProcessorContext m_Context;
        protected Version m_GoogleBuildTools;
        protected string m_PackageName;
        protected string m_ProductName;
        protected ScriptingImplementation m_ScriptingBackend;
        protected bool m_SourceBuild;
        protected string m_StagingArea;
        protected string m_TargetPath;
        protected int m_TargetSDKVersion;
        protected string m_UnityAndroidBuildTools;
        protected string m_UnityJavaLibrary;
        protected string m_UnityJavaSources;
        protected bool m_UseObb;
        protected static readonly string[] UnityActivities = new string[] { "UnityPlayerNativeActivity", "UnityPlayerActivity", "UnityPlayerProxyActivity" };

        protected AndroidProjectExport()
        {
        }

        protected static void CopyAndPatchJavaSources(string targetPath, string srcPath, string packageName)
        {
            string str = Path.Combine(srcPath, DefaultUnityPackage.Replace('.', '/'));
            string path = Path.Combine(targetPath, packageName.Replace('.', '/'));
            Directory.CreateDirectory(path);
            foreach (string str3 in UnityActivities)
            {
                string str4 = Path.Combine(path, str3 + ".java");
                string str5 = Path.Combine(str, str3 + ".java");
                if (!File.Exists(str4) && File.Exists(str5))
                {
                    File.WriteAllText(str4, PatchJavaSource(File.ReadAllText(str5), packageName));
                }
            }
        }

        protected static void CopyDir(string source, string target)
        {
            if (Directory.Exists(source))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(target));
                FileUtil.CopyDirectoryRecursive(source, target, true);
            }
        }

        protected static void CopyFile(string source, string target)
        {
            if (File.Exists(source))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(target));
                FileUtil.UnityFileCopy(source, target, true);
            }
        }

        public static AndroidProjectExport Create(int system)
        {
            if (system == 2)
            {
                return new AndroidProjectExportADT();
            }
            if (system == 1)
            {
                return new AndroidProjectExportGradle();
            }
            if (system == 3)
            {
                return new AndroidProjectExportVisualStudioGradle();
            }
            return null;
        }

        public void Export(PostProcessorContext context, string targetPath)
        {
            this.Init(context, targetPath);
            this.ExportWithCurrentSettings();
        }

        public abstract void ExportWithCurrentSettings();
        protected static void GenerateAndroidManifest(string targetManifestFilePath, string stagingArea, string packageName, bool debugAttr = true)
        {
            AndroidManifest manifest = new AndroidManifest(Path.Combine(stagingArea, "AndroidManifest.xml"));
            foreach (string str in UnityActivities)
            {
                manifest.RenameActivity(DefaultUnityPackage + "." + str, packageName + "." + str);
            }
            if (!debugAttr)
            {
                manifest.RemoveApplicationFlag("debuggable");
            }
            manifest.SaveAs(targetManifestFilePath);
        }

        private void Init(PostProcessorContext context, string targetPath)
        {
            this.m_Context = context;
            this.m_UnityJavaSources = Path.Combine(context.Get<string>("PlayerPackage"), "Source");
            string[] components = new string[] { TasksCommon.GetClassDirectory(context), "classes.jar" };
            this.m_UnityJavaLibrary = Paths.Combine(components);
            this.m_UnityAndroidBuildTools = BuildPipeline.GetBuildToolsDirectory(BuildTarget.Android);
            this.m_StagingArea = context.Get<string>("StagingArea");
            this.m_AndroidLibraries = context.Get<AndroidLibraries>("AndroidLibraries");
            this.m_TargetPath = targetPath;
            this.m_PackageName = context.Get<string>("PackageName");
            this.m_ProductName = PlayerSettings.productName;
            this.m_TargetSDKVersion = context.Get<int>("TargetSDKVersion");
            this.m_GoogleBuildTools = context.Get<AndroidSDKTools>("SDKTools").BuildToolsVersion(null);
            this.m_UseObb = context.Get<bool>("UseObb");
            this.m_ScriptingBackend = this.m_Context.Get<ScriptingImplementation>("ScriptingBackend");
            this.m_SourceBuild = this.m_Context.Get<bool>("SourceBuild");
            this.m_AndroidPluginsPath = context.Get<string>("AndroidPluginsPath");
        }

        protected static string PatchJavaSource(string javaSource, string packageName)
        {
            javaSource = javaSource.Replace("package " + DefaultUnityPackage, "package " + packageName);
            foreach (string str in UnityActivities)
            {
                javaSource = javaSource.Replace(DefaultUnityPackage + "." + str, packageName + "." + str);
            }
            string str2 = "import com.unity3d.player.*;";
            int index = javaSource.IndexOf("import");
            if (index >= 0)
            {
                javaSource = javaSource.Insert(index, str2 + "\n");
                return javaSource;
            }
            Match match = new Regex(@"package\s.*;").Match(javaSource);
            if (match.Success)
            {
                javaSource = javaSource.Insert(match.Index + match.Length, "\n\n" + str2);
                return javaSource;
            }
            javaSource = javaSource.Insert(0, str2 + "\n\n");
            return javaSource;
        }
    }
}

