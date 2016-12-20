namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.IO;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.Android;
    using UnityEditor.Android.Il2Cpp;
    using UnityEditor.Android.PostProcessor;
    using UnityEditor.Utils;
    using UnityEditorInternal;

    internal class RunIl2Cpp : IPostProcessorTask
    {
        public event ProgressHandler OnProgress;

        private void CopySymbolMap(string stagingArea, string assetsDataData, AndroidTargetDeviceType targetDeviceType)
        {
            string[] components = new string[] { assetsDataData, "Managed" };
            string destinationFolder = Paths.Combine(components);
            IL2CPPUtils.CopySymmapFile(Path.Combine(Path.Combine(Path.Combine(stagingArea, "libs"), targetDeviceType.ABI), "Data"), destinationFolder, "-" + targetDeviceType.Architecture);
        }

        public void Execute(PostProcessorContext context)
        {
            if (((ScriptingImplementation) context.Get<ScriptingImplementation>("ScriptingBackend")) == ScriptingImplementation.IL2CPP)
            {
                if (this.OnProgress != null)
                {
                    this.OnProgress(this, "Generating native assemblies");
                }
                BuildTarget target = context.Get<BuildTarget>("BuildTarget");
                string str = context.Get<string>("StagingArea");
                string[] components = new string[] { str, "assets", "bin", "Data" };
                string stagingAreaData = Paths.Combine(components);
                string[] textArray2 = new string[] { BuildPipeline.GetBuildToolsDirectory(target), "AndroidNativeLink.xml" };
                string[] textArray3 = new string[] { stagingAreaData, "platform_native_link.xml" };
                FileUtil.CopyFileOrDirectory(Paths.Combine(textArray2), Paths.Combine(textArray3));
                AndroidTargetDevice device = context.Get<AndroidTargetDevice>("TargetDevice");
                AndroidTargetDeviceType deviceType = new AndroidTargetDeviceARMv7();
                if (device == AndroidTargetDevice.x86)
                {
                    deviceType = new AndroidTargetDevicex86();
                }
                if (this.OnProgress != null)
                {
                    this.OnProgress(this, "Compiling native assemblies for " + deviceType.Architecture);
                }
                string tempFolder = Path.Combine(str, "Il2Cpp");
                bool isDevelopmentBuild = context.Get<bool>("DevelopmentPlayer");
                RuntimeClassRegistry runtimeClassRegistry = context.Get<RuntimeClassRegistry>("UsedClassRegistry");
                AndroidIl2CppPlatformProvider platformProvider = new AndroidIl2CppPlatformProvider(target, deviceType, isDevelopmentBuild);
                IL2CPPUtils.RunIl2Cpp(tempFolder, stagingAreaData, platformProvider, null, runtimeClassRegistry, isDevelopmentBuild);
                AndroidTargetDeviceType type2 = null;
                if (device == AndroidTargetDevice.FAT)
                {
                    type2 = new AndroidTargetDevicex86();
                    if (this.OnProgress != null)
                    {
                        this.OnProgress(this, "Compiling native assemblies for " + type2.Architecture);
                    }
                    platformProvider = new AndroidIl2CppPlatformProvider(target, type2, isDevelopmentBuild);
                    IL2CPPUtils.RunCompileAndLink(tempFolder, stagingAreaData, platformProvider, null, runtimeClassRegistry, isDevelopmentBuild);
                }
                this.FinalizeAndCleanup(str, stagingAreaData, tempFolder);
                this.CopySymbolMap(str, stagingAreaData, deviceType);
                if (type2 != null)
                {
                    this.CopySymbolMap(str, stagingAreaData, type2);
                }
            }
        }

        private void FinalizeAndCleanup(string stagingArea, string assetsDataData, string il2cppDir)
        {
            string[] components = new string[] { assetsDataData, "Managed" };
            string from = Paths.Combine(components);
            FileUtil.MoveFileOrDirectory(from, Path.Combine(il2cppDir, "Managed"));
            string[] textArray2 = new string[] { assetsDataData, "Native" };
            string[] textArray3 = new string[] { stagingArea, "libs" };
            FileUtil.CopyDirectoryRecursive(Paths.Combine(textArray2), Paths.Combine(textArray3));
            string[] textArray4 = new string[] { assetsDataData, "Native" };
            FileUtil.DeleteFileOrDirectory(Paths.Combine(textArray4));
            string dir = Path.Combine(from, "Resources");
            string str3 = Path.Combine(from, "Metadata");
            string str4 = Path.Combine(from, "etc");
            FileUtil.CreateOrCleanDirectory(str3);
            FileUtil.CreateOrCleanDirectory(dir);
            FileUtil.CreateOrCleanDirectory(str4);
            IL2CPPUtils.CopyEmbeddedResourceFiles(il2cppDir, dir);
            IL2CPPUtils.CopyMetadataFiles(il2cppDir, str3);
            IL2CPPUtils.CopyConfigFiles(il2cppDir, str4);
        }

        public string Name
        {
            get
            {
                return "IL2CPP";
            }
        }
    }
}

