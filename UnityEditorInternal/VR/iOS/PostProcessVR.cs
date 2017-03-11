namespace UnityEditorInternal.VR.iOS
{
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEditor.Callbacks;
    using UnityEditor.iOS.Xcode;
    using UnityEditorInternal.VR;

    internal static class PostProcessVR
    {
        private static readonly string kVRFolder;

        static PostProcessVR()
        {
            string[] paths = new string[] { "UnityExtensions", "Unity", "VR" };
            kVRFolder = FileUtil.CombinePaths(paths);
        }

        private static void EditXcodeProject(string pluginFolder, string buildFolder)
        {
            string str6;
            if (!File.Exists(Path.Combine(buildFolder, "Podfile")))
            {
                FileUtil.CopyFileOrDirectory(Path.Combine(pluginFolder, "GVRSDK/Podfile"), Path.Combine(buildFolder, "Podfile"));
            }
            if (!File.Exists(Path.Combine(buildFolder, "Podfile.lock")))
            {
                FileUtil.CopyFileOrDirectory(Path.Combine(pluginFolder, "GVRSDK/Podfile.lock"), Path.Combine(buildFolder, "Podfile.lock"));
            }
            if (!Directory.Exists(Path.Combine(buildFolder, "Pods")))
            {
                FileUtil.CopyFileOrDirectory(Path.Combine(pluginFolder, "GVRSDK/Pods"), Path.Combine(buildFolder, "Pods"));
            }
            if (!File.Exists(Path.Combine(buildFolder, "Classes/CardboardAppController.mm")))
            {
                FileUtil.CopyFileOrDirectory(Path.Combine(pluginFolder, "Source/CardboardAppController.mm"), Path.Combine(buildFolder, "Classes/CardboardAppController.mm"));
            }
            PBXProject project = new PBXProject();
            string path = Path.Combine(buildFolder, FileUtil.NiceWinPath("Unity-iPhone.xcodeproj/project.pbxproj"));
            project.ReadFromFile(path);
            string targetGuid = project.TargetGuidByName(PBXProject.GetUnityTargetName());
            project.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "false");
            project.AddFrameworkToProject(targetGuid, "Security.framework", true);
            string fileGuid = project.AddFile("Classes/CardboardAppController.mm", "Classes/CardboardAppController.mm", PBXSourceTree.Source);
            project.AddFileToBuild(targetGuid, fileGuid);
            string configGuid = project.BuildConfigByName(targetGuid, "Debug");
            string[] strArray = new string[] { project.BuildConfigByName(targetGuid, "Release"), project.BuildConfigByName(targetGuid, "ReleaseForRunning"), project.BuildConfigByName(targetGuid, "ReleaseForProfiling") };
            fileGuid = project.AddFile("Pods/Target Support Files/Pods-Unity-iPhone/Pods-Unity-iPhone.release.xcconfig", "Pods/Pods-Unity-iPhone.release.xcconfig", PBXSourceTree.Source);
            foreach (string str5 in strArray)
            {
                project.SetBaseReferenceForConfig(str5, fileGuid);
            }
            fileGuid = project.AddFile("Pods/Target Support Files/Pods-Unity-iPhone/Pods-Unity-iPhone.debug.xcconfig", "Pods/Pods-Unity-iPhone.debug.xcconfig", PBXSourceTree.Source);
            project.SetBaseReferenceForConfig(configGuid, fileGuid);
            if (project.ShellScriptByName(targetGuid, "[CP] Copy Pods Resources") == null)
            {
                str6 = "\"${SRCROOT}/Pods/Target Support Files/Pods-Unity-iPhone/Pods-Unity-iPhone-resources.sh\"\n";
                project.AppendShellScriptBuildPhase(targetGuid, "[CP] Copy Pods Resources", "/bin/sh", str6);
            }
            if (project.ShellScriptByName(targetGuid, "[CP] Check Pods Manifest.lock") == null)
            {
                str6 = "diff \"${PODS_ROOT}/../Podfile.lock\" \"${PODS_ROOT}/Manifest.lock\" > /dev/null\nif [ $? != 0 ] ; then\n    # print error to STDERR\n    echo \"error: The sandbox is not in sync with the Podfile.lock. Run 'pod install' or update your CocoaPods installation.\" >&2\n    exit 1\nfi\n";
                project.AppendShellScriptBuildPhase(targetGuid, "[CP] Check Pods Manifest.lock", "/bin/sh", str6);
            }
            if (project.ShellScriptByName(targetGuid, "[CP] Embed Pods Frameworks") == null)
            {
                str6 = "\"${SRCROOT}/Pods/Target Support Files/Pods-Unity-iPhone/Pods-Unity-iPhone-frameworks.sh\"\n";
                project.AppendShellScriptBuildPhase(targetGuid, "[CP] Embed Pods Frameworks", "/bin/sh", str6);
            }
            string str7 = Path.Combine(buildFolder, "Pods/Target Support Files/Pods-Unity-iPhone/Pods-Unity-iPhone-resources.sh");
            string contents = File.ReadAllText(str7).Replace("if [[ \"$CONFIGURATION\" == \"Release\" ]]; then", "if [[ \"$CONFIGURATION\" == \"Release\" || \"$CONFIGURATION\" == \"ReleaseForRunning\" || \"$CONFIGURATION\" == \"ReleaseForProfiling\" ]]; then");
            File.WriteAllText(str7, contents);
            project.WriteToFile(path);
        }

        internal static string GetPluginFolder(BuildTarget target)
        {
            string buildTargetName = BuildPipeline.GetBuildTargetName(target);
            string[] paths = new string[] { EditorApplication.applicationContentsPath, kVRFolder, buildTargetName };
            string[] strArray2 = new string[] { EditorApplication.applicationContentsPath, kVRFolder, buildTargetName.ToLower() };
            string path = FileUtil.CombinePaths(paths);
            if (!Directory.Exists(path))
            {
                path = FileUtil.CombinePaths(strArray2);
                if (!Directory.Exists(path))
                {
                    path = null;
                }
            }
            return path;
        }

        private static bool IsVRDeviceEnabled(BuildTarget target, string deviceName)
        {
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(target);
            foreach (VRDeviceInfoEditor editor in VREditor.GetEnabledVRDeviceInfo(buildTargetGroup))
            {
                if (editor.deviceNameKey == deviceName)
                {
                    return true;
                }
            }
            return false;
        }

        [PostProcessBuild]
        private static void PostProcessBuild_iOS(BuildTarget target, string path)
        {
            if ((PlayerSettings.virtualRealitySupported && (target == BuildTarget.iOS)) && IsVRDeviceEnabled(target, "cardboard"))
            {
                string[] paths = new string[] { EditorApplication.applicationContentsPath, "UnityExtensions", "Unity", "VR", "iOS" };
                EditXcodeProject(FileUtil.CombinePaths(paths), path);
            }
        }
    }
}

