namespace UnityEditor.Facebook
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using UnityEditor;
    using UnityEditor.BuildReporting;
    using UnityEditor.Modules;
    using UnityEngine;

    internal sealed class BuildProcessor : DefaultBuildPostprocessor
    {
        private IBuildPostprocessor m_WebGLProcessor;
        private IBuildPostprocessor m_WindowsStandaloneProcessor;

        public BuildProcessor(IBuildPostprocessor winProcessor, IBuildPostprocessor webGLProcessor)
        {
            this.m_WindowsStandaloneProcessor = winProcessor;
            this.m_WebGLProcessor = webGLProcessor;
        }

        private string CreateSubmissionPackage(string installPath)
        {
            string str = (EditorUserBuildSettings.selectedFacebookTarget != BuildTarget.WebGL) ? Path.GetDirectoryName(installPath) : installPath;
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(installPath);
            string str3 = fileNameWithoutExtension + "_Data";
            string path = Path.Combine(str, "manifest.txt");
            File.WriteAllText(path, "");
            string destFile = fileNameWithoutExtension + ".7z";
            if (EditorUserBuildSettings.selectedFacebookTarget == BuildTarget.WebGL)
            {
                string[] paths = new string[] { "." };
                Utilities.Zip(str, destFile, paths);
            }
            else
            {
                string[] textArray2 = new string[] { Path.GetFileName(installPath), str3, path };
                Utilities.Zip(str, destFile, textArray2);
            }
            return Path.Combine(str, destFile);
        }

        private string EscapeURI(string path)
        {
            path = path.Replace('\\', '/');
            path = path.Replace(' ', '?');
            return WWW.EscapeURL(path).Replace("%3f", "%20");
        }

        private string GetExePath(string installPath)
        {
            string directoryName = Path.GetDirectoryName(installPath);
            if (!Path.IsPathRooted(directoryName))
            {
                directoryName = Path.Combine(Directory.GetCurrentDirectory(), directoryName);
            }
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(installPath);
            return Path.Combine(directoryName, fileNameWithoutExtension + ".exe");
        }

        public override string GetExtension(BuildTarget target, BuildOptions options) => 
            this.GetProcessor(target).GetExtension(target, options);

        private string GetPlayerExecutable(BuildPostProcessArgs args)
        {
            string str;
            BuildTarget target = args.target;
            if (target != BuildTarget.StandaloneWindows)
            {
                if (target != BuildTarget.StandaloneWindows64)
                {
                    return "";
                }
            }
            else
            {
                str = "win32";
                goto Label_003E;
            }
            str = "win64";
        Label_003E:
            str = str + (((args.options & BuildOptions.Development) == BuildOptions.CompressTextures) ? "_nondevelopment" : "_development") + (!this.UseIl2Cpp ? "_mono" : "_il2cpp");
            return Path.Combine(Path.Combine(Path.Combine(BuildPipeline.GetPlaybackEngineDirectory(BuildTargetGroup.Facebook, args.target, args.options), "Variations"), str), "player_win.exe");
        }

        private IBuildPostprocessor GetProcessor(BuildTarget target)
        {
            if (target != BuildTarget.StandaloneWindows64)
            {
                if (target == BuildTarget.WebGL)
                {
                    return this.m_WebGLProcessor;
                }
                if (target != BuildTarget.StandaloneWindows)
                {
                    throw new Exception("Unknown buildtarget: " + target);
                }
            }
            return this.m_WindowsStandaloneProcessor;
        }

        public override void LaunchPlayer(BuildLaunchPlayerArgs args)
        {
            if (EditorUserBuildSettings.selectedFacebookTarget == BuildTarget.WebGL)
            {
                this.m_WebGLProcessor.LaunchPlayer(args);
            }
            else
            {
                string exePath = this.GetExePath(args.installPath);
                string str2 = $"fbgames://launch_local/"{this.EscapeURI(exePath)}"";
                ProcessStartInfo startInfo = new ProcessStartInfo {
                    UseShellExecute = true,
                    FileName = str2
                };
                Process.Start(startInfo);
                UnityEngine.Debug.Log("Launching URI: " + str2);
            }
        }

        public override void PostProcess(BuildPostProcessArgs args)
        {
            this.GetProcessor(args.target).PostProcess(args);
            string playerExecutable = this.GetPlayerExecutable(args);
            if (!string.IsNullOrEmpty(playerExecutable))
            {
                string exePath = this.GetExePath(args.installPath);
                File.Copy(playerExecutable, exePath, true);
            }
            if (EditorUserBuildSettings.facebookCreatePackageForSubmission)
            {
                args.report.BeginBuildStep("Create Package for Submission");
                Artifacts artifacts = ScriptableObject.CreateInstance<Artifacts>();
                string path = this.CreateSubmissionPackage(args.installPath);
                if (!Path.IsPathRooted(path))
                {
                    string directoryName = Path.GetDirectoryName(args.installPath);
                    string str5 = !Path.IsPathRooted(directoryName) ? Path.Combine(Directory.GetCurrentDirectory(), directoryName) : directoryName;
                    path = Path.Combine(str5, path);
                }
                artifacts.paths = new string[] { path };
                args.report.AddAppendix(artifacts);
            }
        }

        public override string PrepareForBuild(BuildOptions options, BuildTarget target) => 
            this.GetProcessor(target).PrepareForBuild(options, target);

        public override bool SupportsInstallInBuildFolder() => 
            false;

        private bool UseIl2Cpp =>
            (PlayerSettings.GetScriptingBackend(BuildTargetGroup.Standalone) == ScriptingImplementation.IL2CPP);
    }
}

