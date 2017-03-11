namespace UnityEditor.Android
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEditor.Utils;

    internal class AndroidProjectExportGradle : AndroidProjectExport
    {
        public override void ExportWithCurrentSettings()
        {
            string str;
            bool flag = false;
            if (base.m_TargetPath == null)
            {
                str = Path.Combine("Temp", "gradleOut");
                flag = true;
                Directory.CreateDirectory(str);
                string[] strArray = AndroidFileLocator.Find(Path.Combine(str, "*"));
                foreach (string str2 in strArray)
                {
                    string fileName = Path.GetFileName(str2);
                    if ((fileName != "build") && (fileName != ".gradle"))
                    {
                        try
                        {
                            FileUtil.DeleteFileOrDirectory(str2);
                        }
                        catch (IOException)
                        {
                        }
                    }
                }
            }
            else
            {
                str = Path.Combine(base.m_TargetPath, base.m_ProductName);
            }
            int targetSDKVersion = base.m_TargetSDKVersion;
            string str4 = "proguard-user.txt";
            Dictionary<string, string> templateValues = new Dictionary<string, string> {
                { 
                    "BUILDTOOLS",
                    base.m_GoogleBuildTools.ToString()
                },
                { 
                    "APIVERSION",
                    targetSDKVersion.ToString()
                },
                { 
                    "TARGETSDKVERSION",
                    targetSDKVersion.ToString()
                },
                { 
                    "MINIFY_RELEASE",
                    !base.m_minifyRelease ? "false" : "true"
                },
                { 
                    "PROGUARD_RELEASE",
                    !base.m_useProguardRelease ? "false" : "true"
                },
                { 
                    "MINIFY_DEBUG",
                    !base.m_minifyDebug ? "false" : "true"
                },
                { 
                    "PROGUARD_DEBUG",
                    !base.m_useProguardDebug ? "false" : "true"
                },
                { 
                    "USER_PROGUARD",
                    ""
                }
            };
            string path = Path.Combine(base.m_AndroidPluginsPath, str4);
            if (File.Exists(path))
            {
                AndroidProjectExport.CopyFile(path, Path.Combine(str, str4));
                templateValues["USER_PROGUARD"] = ",'" + str4 + "'";
            }
            string[] components = new string[] { str, "src", "main" };
            string str6 = Paths.Combine(components);
            string target = Path.Combine(str, "libs");
            Directory.CreateDirectory(str);
            string sDKRootDir = AndroidSDKTools.GetInstance().SDKRootDir;
            string str9 = Path.Combine(str, "local.properties");
            if (!File.Exists(str9) || flag)
            {
                File.WriteAllText(str9, $"sdk.dir={sDKRootDir.Replace(@"\", @"\\")}
");
            }
            AndroidProjectExport.CopyFile(base.m_UnityJavaLibrary, Path.Combine(Path.Combine(str, "libs"), "unity-classes.jar"));
            AndroidProjectExport.CopyFile(Path.Combine(base.m_UnityAndroidBuildTools, "UnityProGuardTemplate.txt"), Path.Combine(str, "proguard-unity.txt"));
            try
            {
                string[] textArray2 = new string[] { str6, "assets", "bin", "Data" };
                Directory.Delete(Paths.Combine(textArray2), true);
            }
            catch (IOException)
            {
            }
            try
            {
                Directory.Delete(Path.Combine(str6, "res"), true);
            }
            catch (IOException)
            {
            }
            AndroidProjectExport.CopyDir(Path.Combine(base.m_StagingArea, "res"), Path.Combine(str6, "res"));
            AndroidProjectExport.CopyDir(Path.Combine(base.m_StagingArea, "plugins"), target);
            AndroidProjectExport.CopyDir(Path.Combine(base.m_StagingArea, "libs"), Path.Combine(str6, "jniLibs"));
            AndroidProjectExport.CopyDir(Path.Combine(base.m_StagingArea, "assets"), Path.Combine(str6, "assets"));
            string[] strArray3 = AndroidFileLocator.Find(Path.Combine(Path.Combine(base.m_StagingArea, "aar"), "*.aar"));
            string str11 = "";
            foreach (string str12 in strArray3)
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(str12);
                string str14 = Path.GetFileName(str12);
                str11 = str11 + $"	compile(name: '{fileNameWithoutExtension}', ext:'aar')
";
                AndroidProjectExport.CopyFile(str12, Path.Combine(target, str14));
            }
            templateValues["DEPS"] = str11;
            if (flag)
            {
                AndroidProjectExport.CopyFile(Path.Combine(base.m_StagingArea, "AndroidManifest.xml"), Path.Combine(str6, "AndroidManifest.xml"));
            }
            else
            {
                AndroidProjectExport.GenerateAndroidManifest(Path.Combine(str6, "AndroidManifest.xml"), base.m_StagingArea, base.m_PackageName, false);
                AndroidProjectExport.CopyAndPatchJavaSources(Path.Combine(str6, "java"), base.m_UnityJavaSources, base.m_PackageName);
            }
            string str15 = Path.Combine(str, "build.gradle");
            if (File.Exists(str15) && !File.ReadAllText(str15).StartsWith("// GENERATED BY UNITY"))
            {
                str15 = Path.Combine(str, "build.gradle.NEW");
            }
            this.WriteGradleBuildFiles(str, str15, templateValues);
            if (base.m_UseObb)
            {
                AndroidProjectExport.CopyFile(Path.Combine(base.m_StagingArea, "main.obb"), Path.Combine(str, $"{base.m_ProductName}.main.obb"));
            }
            else
            {
                AndroidProjectExport.CopyDir(Path.Combine(base.m_StagingArea, "raw"), Path.Combine(str6, "assets"));
            }
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

        private void WriteGradleBuildFiles(string projectPath, string gradleTarget, Dictionary<string, string> templateValues)
        {
            string str = Path.Combine(BuildPipeline.GetBuildToolsDirectory(BuildTarget.Android), "GradleTemplates");
            string path = Path.Combine(base.m_AndroidPluginsPath, "mainTemplate.gradle");
            if (!File.Exists(path))
            {
                path = Path.Combine(str, "mainTemplate.gradle");
            }
            string template = File.ReadAllText(path);
            string str4 = File.ReadAllText(Path.Combine(str, "libTemplate.gradle"));
            string str5 = "";
            string contents = "";
            foreach (string str7 in base.m_AndroidLibraries)
            {
                int result = 4;
                string str8 = Path.Combine(str7, "project.properties");
                if (File.Exists(str8))
                {
                    string input = File.ReadAllText(str8);
                    MatchCollection matchs = new Regex(@"^\s*target\s*=\s*android-(\d+)\s*$", RegexOptions.Multiline).Matches(input);
                    if (matchs.Count > 0)
                    {
                        int.TryParse(matchs[0].Groups[1].Value, out result);
                    }
                }
                templateValues["LIBSDKTARGET"] = result.ToString();
                string fileName = Path.GetFileName(str7);
                str5 = str5 + $"	compile project(':{fileName}')
";
                contents = contents + $"include '{fileName}'
";
                string target = Path.Combine(projectPath, fileName);
                AndroidProjectExport.CopyDir(str7, target);
                string str13 = TemplateReplace(str4, templateValues);
                File.WriteAllText(Path.Combine(target, "build.gradle"), str13);
            }
            string str14 = templateValues["DEPS"];
            templateValues["DEPS"] = str14 + str5;
            if (PlayerSettings.Android.keyaliasName.Length != 0)
            {
                string str15 = !Path.IsPathRooted(PlayerSettings.Android.keystoreName) ? Path.Combine(Directory.GetCurrentDirectory(), PlayerSettings.Android.keystoreName).Replace('\\', '/') : PlayerSettings.Android.keystoreName;
                string str16 = $"		storeFile file('{str15}')
		storePassword '{PlayerSettings.Android.keystorePass}'
		keyAlias '{PlayerSettings.Android.keyaliasName}'
		keyPassword '{PlayerSettings.Android.keyaliasPass}'";
                templateValues["SIGN"] = "\tsigningConfigs { release {\n" + str16 + "\n\t} }\n";
                templateValues["SIGNCONFIG"] = "signingConfig signingConfigs.release";
            }
            string str17 = TemplateReplace(template, templateValues);
            File.WriteAllText(gradleTarget, str17);
            if (contents != "")
            {
                File.WriteAllText(Path.Combine(projectPath, "settings.gradle"), contents);
            }
        }
    }
}

