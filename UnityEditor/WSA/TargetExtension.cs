namespace UnityEditor.WSA
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEditorInternal;

    internal sealed class TargetExtension : DefaultPlatformSupportModule
    {
        private string[] assemblyReferencesForUserScripts;
        private MetroBuildWindowExtension buildWindow;
        internal readonly bool hasDotNetPlayers;
        internal readonly bool hasIl2CppPlayers;
        private MetroPluginImporterExtension pluginImporterExtension;
        private MetroProjectGeneratorExtension projectGeneratorExtension;
        private ScriptingImplementations scriptingImplementations;

        public TargetExtension()
        {
            <TargetExtension>c__AnonStorey0 storey = new <TargetExtension>c__AnonStorey0();
            this.assemblyReferencesForUserScripts = new string[] { Path.Combine(EditorApplication.applicationContentsPath, "Managed/Mono.Cecil.dll") };
            storey.playerPackage = BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.WSAPlayer, BuildOptions.CompressTextures);
            string[] strArray = new string[] { @"Managed\Store81\UnityEngine.dll", @"Managed\Phone\UnityEngine.dll", @"Managed\UAP\UnityEngine.dll" };
            this.hasDotNetPlayers = Enumerable.Any<string>(strArray, new Func<string, bool>(storey, (IntPtr) this.<>m__0));
            this.hasIl2CppPlayers = File.Exists(Path.Combine(storey.playerPackage, @"Managed\il2cpp\UnityEngine.dll"));
        }

        public override IBuildPostprocessor CreateBuildPostprocessor()
        {
            return new BuildPostprocessor();
        }

        public override IBuildWindowExtension CreateBuildWindowExtension()
        {
            return ((this.buildWindow != null) ? this.buildWindow : (this.buildWindow = new MetroBuildWindowExtension(this)));
        }

        public override ICompilationExtension CreateCompilationExtension()
        {
            return ((base.compilationExtension != null) ? base.compilationExtension : (base.compilationExtension = new MetroCompilationExtension()));
        }

        public override IPluginImporterExtension CreatePluginImporterExtension()
        {
            return ((this.pluginImporterExtension != null) ? this.pluginImporterExtension : (this.pluginImporterExtension = new MetroPluginImporterExtension()));
        }

        public override IProjectGeneratorExtension CreateProjectGeneratorExtension()
        {
            return ((this.projectGeneratorExtension != null) ? this.projectGeneratorExtension : (this.projectGeneratorExtension = new MetroProjectGeneratorExtension()));
        }

        public override IScriptingImplementations CreateScriptingImplementations()
        {
            return ((this.scriptingImplementations != null) ? this.scriptingImplementations : (this.scriptingImplementations = new ScriptingImplementations()));
        }

        public override ISettingEditorExtension CreateSettingsEditorExtension()
        {
            return new MetroSettingsEditorExtension();
        }

        public override void RegisterAdditionalUnityExtensions()
        {
            WSAExtension[] extensionArray1 = new WSAExtension[3];
            WSAExtension extension = new WSAExtension {
                path = @"Managed\Store81\UnityEngine.Networking.dll",
                guid = "67e9dad5654047ebbe623cce9dbf7b38"
            };
            extensionArray1[0] = extension;
            WSAExtension extension2 = new WSAExtension {
                path = @"Managed\Phone\UnityEngine.Networking.dll",
                guid = "f48df919adea4fa095a7407e773e5aa4"
            };
            extensionArray1[1] = extension2;
            WSAExtension extension3 = new WSAExtension {
                path = @"Managed\UAP\UnityEngine.Networking.dll",
                guid = "52a206b72a8f4f749c28d8b18b42dd19"
            };
            extensionArray1[2] = extension3;
            WSAExtension[] extensionArray = extensionArray1;
            string playbackEngineDirectory = BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.WSAPlayer, BuildOptions.CompressTextures);
            foreach (WSAExtension extension4 in extensionArray)
            {
                string path = Path.Combine(playbackEngineDirectory, extension4.path);
                if (File.Exists(path))
                {
                    InternalEditorUtility.RegisterExtensionDll(path, extension4.guid);
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                }
            }
        }

        public override string[] AssemblyReferencesForUserScripts
        {
            get
            {
                return this.assemblyReferencesForUserScripts;
            }
        }

        public override string JamTarget
        {
            get
            {
                return "MetroEditorExtensions";
            }
        }

        public override string TargetName
        {
            get
            {
                return "Metro";
            }
        }

        [CompilerGenerated]
        private sealed class <TargetExtension>c__AnonStorey0
        {
            internal string playerPackage;

            internal bool <>m__0(string dll)
            {
                return File.Exists(Path.Combine(this.playerPackage, dll));
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WSAExtension
        {
            internal string path;
            internal string guid;
        }
    }
}

