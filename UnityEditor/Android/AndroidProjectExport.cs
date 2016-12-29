namespace UnityEditor.Android
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEditor.Utils;

    internal class AndroidProjectExport
    {
        private static readonly string DefaultUnityPackage = "com.unity3d.player";
        private static readonly string[] UnityActivities = new string[] { "UnityPlayerNativeActivity", "UnityPlayerActivity", "UnityPlayerProxyActivity" };

        private static void CopyAndPatchJavaSources(string targetPath, string srcPath, string packageName)
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

        private static void CopyDir(string source, string target)
        {
            if (Directory.Exists(source))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(target));
                FileUtil.CopyDirectoryRecursive(source, target, true);
            }
        }

        private static void CopyFile(string source, string target)
        {
            if (File.Exists(source))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(target));
                FileUtil.UnityFileCopy(source, target, true);
            }
        }

        public static void ExportADT(string unityJavaSources, string unityJavaLibrary, string unityAndroidBuildTools, string stagingArea, AndroidLibraries androidLibraries, string targetPath, string packageName, string productName, int platformApiLevel, bool useObb)
        {
            string str = Path.Combine(targetPath, productName);
            CopyFile(unityJavaLibrary, Path.Combine(Path.Combine(str, "libs"), "unity-classes.jar"));
            CopyFile(Path.Combine(unityAndroidBuildTools, "UnityProGuardTemplate.txt"), Path.Combine(str, "proguard-unity.txt"));
            CopyDir(Path.Combine(stagingArea, "res"), Path.Combine(str, "res"));
            CopyDir(Path.Combine(stagingArea, "plugins"), Path.Combine(str, "libs"));
            CopyDir(Path.Combine(stagingArea, "libs"), Path.Combine(str, "libs"));
            CopyDir(Path.Combine(stagingArea, "aar"), Path.Combine(str, "libs"));
            CopyDir(Path.Combine(stagingArea, "assets"), Path.Combine(str, "assets"));
            GenerateProjectProperties(str, platformApiLevel, androidLibraries);
            GenerateAndroidManifest(str, stagingArea, packageName, true);
            CopyAndPatchJavaSources(Path.Combine(str, "src"), unityJavaSources, packageName);
            foreach (string str2 in androidLibraries)
            {
                CopyDir(str2, Path.Combine(targetPath, Path.GetFileName(str2)));
            }
            if (useObb)
            {
                CopyFile(Path.Combine(stagingArea, "main.obb"), Path.Combine(targetPath, $"{productName}.main.obb"));
            }
            else
            {
                CopyDir(Path.Combine(stagingArea, "raw"), Path.Combine(str, "assets"));
            }
        }

        public static void ExportGradle(string unityJavaSources, string unityJavaLibrary, string unityAndroidBuildTools, string stagingArea, AndroidLibraries androidLibraries, string targetPath, string packageName, string productName, int platformApiLevel, string googleBuildTools, bool useObb)
        {
            string str;
            bool flag = false;
            if (targetPath == null)
            {
                str = Path.Combine(stagingArea, "gradleOut");
                flag = true;
            }
            else
            {
                str = Path.Combine(targetPath, productName);
            }
            Dictionary<string, string> templateValues = new Dictionary<string, string> {
                { 
                    "BUILDTOOLS",
                    googleBuildTools
                },
                { 
                    "APIVERSION",
                    platformApiLevel.ToString()
                },
                { 
                    "TARGETSDKVERSION",
                    platformApiLevel.ToString()
                }
            };
            string[] components = new string[] { str, "src", "main" };
            string str2 = Paths.Combine(components);
            string target = Path.Combine(str, "libs");
            Directory.CreateDirectory(str);
            string str4 = EditorPrefs.GetString("AndroidSdkRoot").NormalizePath();
            string path = Path.Combine(str, "local.properties");
            if (!File.Exists(path) || flag)
            {
                File.WriteAllText(path, $"sdk.dir={str4.Replace(@"\", @"\\")}
");
            }
            CopyFile(unityJavaLibrary, Path.Combine(Path.Combine(str, "libs"), "unity-classes.jar"));
            CopyFile(Path.Combine(unityAndroidBuildTools, "UnityProGuardTemplate.txt"), Path.Combine(str, "proguard-unity.txt"));
            try
            {
                string[] textArray2 = new string[] { str2, "assets", "bin", "Data" };
                Directory.Delete(Paths.Combine(textArray2), true);
            }
            catch (IOException)
            {
            }
            try
            {
                Directory.Delete(Path.Combine(str2, "res"), true);
            }
            catch (IOException)
            {
            }
            CopyDir(Path.Combine(stagingArea, "res"), Path.Combine(str2, "res"));
            CopyDir(Path.Combine(stagingArea, "plugins"), target);
            CopyDir(Path.Combine(stagingArea, "libs"), Path.Combine(str2, "jniLibs"));
            CopyDir(Path.Combine(stagingArea, "assets"), Path.Combine(str2, "assets"));
            string[] strArray = AndroidFileLocator.Find(Path.Combine(Path.Combine(stagingArea, "aar"), "*.aar"));
            string str7 = "";
            foreach (string str8 in strArray)
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(str8);
                string fileName = Path.GetFileName(str8);
                str7 = str7 + $"	compile(name: '{fileNameWithoutExtension}', ext:'aar')
";
                CopyFile(str8, Path.Combine(target, fileName));
            }
            templateValues["DEPS"] = str7;
            if (flag)
            {
                CopyFile(Path.Combine(stagingArea, "AndroidManifest.xml"), Path.Combine(str2, "AndroidManifest.xml"));
            }
            else
            {
                GenerateAndroidManifest(str2, stagingArea, packageName, false);
                CopyAndPatchJavaSources(Path.Combine(str2, "java"), unityJavaSources, packageName);
            }
            string str11 = Path.Combine(str, "build.gradle");
            if (File.Exists(str11) && !File.ReadAllText(str11).StartsWith("// GENERATED BY UNITY"))
            {
                str11 = Path.Combine(str, "build.gradle.NEW");
            }
            WriteGradleBuildFiles(str, str11, androidLibraries, templateValues);
            if (useObb)
            {
                CopyFile(Path.Combine(stagingArea, "main.obb"), Path.Combine(str, $"{productName}.main.obb"));
            }
            else
            {
                CopyDir(Path.Combine(stagingArea, "raw"), Path.Combine(str2, "assets"));
            }
        }

        private static void GenerateAndroidManifest(string targetPath, string stagingArea, string packageName, bool debugAttr = true)
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
            manifest.SaveAs(Path.Combine(targetPath, "AndroidManifest.xml"));
        }

        private static void GenerateProjectProperties(string targetPath, int platformApiLevel, AndroidLibraries androidLibraries)
        {
            int num = 1;
            string contents = "target=android-" + platformApiLevel + "\n";
            foreach (string str2 in androidLibraries)
            {
                string str3 = contents;
                object[] objArray1 = new object[] { str3, "android.library.reference.", num++, "=../", Path.GetFileName(str2), "\n" };
                contents = string.Concat(objArray1);
            }
            File.WriteAllText(Path.Combine(targetPath, "project.properties"), contents);
        }

        private static string PatchJavaSource(string javaSource, string packageName)
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

        private static string TemplateReplace(string template, Dictionary<string, string> values)
        {
            StringBuilder builder = new StringBuilder();
            string[] separator = new string[] { "**" };
            string[] strArray2 = template.Split(separator, StringSplitOptions.None);
            for (int i = 0; i < strArray2.Length; i++)
            {
                if ((i % 2) == 0)
                {
                    builder.Append(strArray2[i]);
                }
                else if (values.ContainsKey(strArray2[i]))
                {
                    builder.Append(values[strArray2[i]]);
                }
            }
            return builder.ToString();
        }

        private static void WriteGradleBuildFiles(string projectPath, string gradleTarget, AndroidLibraries androidLibraries, Dictionary<string, string> templateValues)
        {
            string str = Path.Combine(BuildPipeline.GetBuildToolsDirectory(BuildTarget.Android), "GradleTemplates");
            string template = File.ReadAllText(Path.Combine(str, "libTemplate.gradle"));
            string str3 = File.ReadAllText(Path.Combine(str, "mainTemplate.gradle"));
            string str4 = "";
            string contents = "";
            foreach (string str6 in androidLibraries)
            {
                int result = 4;
                string path = Path.Combine(str6, "project.properties");
                if (File.Exists(path))
                {
                    string input = File.ReadAllText(path);
                    MatchCollection matchs = new Regex(@"^\s*target\s*=\s*android-(\d+)\s*$", RegexOptions.Multiline).Matches(input);
                    if (matchs.Count > 0)
                    {
                        int.TryParse(matchs[0].Groups[1].Value, out result);
                    }
                }
                templateValues["LIBSDKTARGET"] = result.ToString();
                string fileName = Path.GetFileName(str6);
                str4 = str4 + $"	compile project(':{fileName}')
";
                contents = contents + $"include '{fileName}'
";
                string target = Path.Combine(projectPath, fileName);
                CopyDir(str6, target);
                string str12 = TemplateReplace(template, templateValues);
                File.WriteAllText(Path.Combine(target, "build.gradle"), str12);
            }
            string str13 = templateValues["DEPS"];
            templateValues["DEPS"] = str13 + str4;
            if (PlayerSettings.Android.keyaliasName.Length != 0)
            {
                string str14 = !Path.IsPathRooted(PlayerSettings.Android.keystoreName) ? Path.Combine(Directory.GetCurrentDirectory(), PlayerSettings.Android.keystoreName) : PlayerSettings.Android.keystoreName;
                string str15 = $"		storeFile file('{str14}')
		storePassword '{PlayerSettings.Android.keystorePass}'
		keyAlias '{PlayerSettings.Android.keyaliasName}'
		keyPassword '{PlayerSettings.Android.keyaliasPass}'";
                templateValues["SIGN"] = "\tsigningConfigs { release {\n" + str15 + "\n\t} }\n";
                templateValues["SIGNCONFIG"] = "signingConfig signingConfigs.release";
            }
            string str16 = TemplateReplace(str3, templateValues);
            File.WriteAllText(gradleTarget, str16);
            if (contents != "")
            {
                File.WriteAllText(Path.Combine(projectPath, "settings.gradle"), contents);
            }
        }
    }
}

