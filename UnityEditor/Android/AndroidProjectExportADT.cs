namespace UnityEditor.Android
{
    using System;
    using System.IO;

    internal class AndroidProjectExportADT : AndroidProjectExport
    {
        public override void ExportWithCurrentSettings()
        {
            string str = Path.Combine(base.m_TargetPath, base.m_ProductName);
            AndroidProjectExport.CopyFile(base.m_UnityJavaLibrary, Path.Combine(Path.Combine(str, "libs"), "unity-classes.jar"));
            AndroidProjectExport.CopyFile(Path.Combine(base.m_UnityAndroidBuildTools, "UnityProGuardTemplate.txt"), Path.Combine(str, "proguard-unity.txt"));
            AndroidProjectExport.CopyDir(Path.Combine(base.m_StagingArea, "res"), Path.Combine(str, "res"));
            AndroidProjectExport.CopyDir(Path.Combine(base.m_StagingArea, "plugins"), Path.Combine(str, "libs"));
            AndroidProjectExport.CopyDir(Path.Combine(base.m_StagingArea, "libs"), Path.Combine(str, "libs"));
            AndroidProjectExport.CopyDir(Path.Combine(base.m_StagingArea, "aar"), Path.Combine(str, "libs"));
            AndroidProjectExport.CopyDir(Path.Combine(base.m_StagingArea, "assets"), Path.Combine(str, "assets"));
            GenerateProjectProperties(str, base.m_TargetSDKVersion, base.m_AndroidLibraries);
            AndroidProjectExport.GenerateAndroidManifest(Path.Combine(str, "AndroidManifest.xml"), base.m_StagingArea, base.m_PackageName, true);
            AndroidProjectExport.CopyAndPatchJavaSources(Path.Combine(str, "src"), base.m_UnityJavaSources, base.m_PackageName);
            foreach (string str2 in base.m_AndroidLibraries)
            {
                AndroidProjectExport.CopyDir(str2, Path.Combine(base.m_TargetPath, Path.GetFileName(str2)));
            }
            if (base.m_UseObb)
            {
                AndroidProjectExport.CopyFile(Path.Combine(base.m_StagingArea, "main.obb"), Path.Combine(base.m_TargetPath, $"{base.m_ProductName}.main.obb"));
            }
            else
            {
                AndroidProjectExport.CopyDir(Path.Combine(base.m_StagingArea, "raw"), Path.Combine(str, "assets"));
            }
        }

        private static void GenerateProjectProperties(string m_TargetPath, int m_PlatformApiLevel, AndroidLibraries m_AndroidLibraries)
        {
            int num = 1;
            string contents = "target=android-" + m_PlatformApiLevel + "\n";
            foreach (string str2 in m_AndroidLibraries)
            {
                string str3 = contents;
                object[] objArray1 = new object[] { str3, "android.library.reference.", num++, "=../", Path.GetFileName(str2), "\n" };
                contents = string.Concat(objArray1);
            }
            File.WriteAllText(Path.Combine(m_TargetPath, "project.properties"), contents);
        }
    }
}

