namespace UnityEditor.WSA
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEditor.Utils;
    using UnityEditorInternal;
    using UnityEngine;

    internal class MetroProjectGeneratorExtension : DefaultProjectGeneratorExtension
    {
        public override void GenerateCSharpProject(CSharpProject project, string assemblyName, IEnumerable<string> sourceFiles, IEnumerable<string> defines, IEnumerable<CSharpProject> additionalProjectReferences)
        {
            MetroAssemblyCSharpCreator.AssemblyCSharpPlugin plugin;
            string[] strArray2;
            string[] strArray3;
            WSASDK wsaSDK = EditorUserBuildSettings.wsaSDK;
            if (wsaSDK == WSASDK.UniversalSDK81)
            {
                wsaSDK = WSASDK.SDK81;
            }
            string playbackEngineDirectory = BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.WSAPlayer, BuildOptions.CompressTextures);
            if (additionalProjectReferences != null)
            {
                string directoryName = Path.GetDirectoryName(project.Path);
                foreach (CSharpProject project2 in additionalProjectReferences)
                {
                    string strB = Path.GetDirectoryName(project2.Path);
                    if (string.Compare(directoryName, strB, true) == 0)
                    {
                        throw new Exception(string.Format("Projects {0} and {1} should be in different folders.", directoryName, strB));
                    }
                }
            }
            List<MetroAssemblyCSharpCreator.AssemblyCSharpPlugin> plugins = new List<MetroAssemblyCSharpCreator.AssemblyCSharpPlugin>();
            string[] strArray = new string[] { ".dll", ".winmd" };
            string fullPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            foreach (PluginImporter importer in PluginImporter.GetImporters(BuildTarget.WSAPlayer))
            {
                <GenerateCSharpProject>c__AnonStorey0 storey = new <GenerateCSharpProject>c__AnonStorey0();
                if (!Directory.Exists(importer.assetPath))
                {
                    storey.extension = Path.GetExtension(importer.assetPath);
                    if (!string.IsNullOrEmpty(importer.assetPath) && Enumerable.Any<string>(strArray, new Func<string, bool>(storey, (IntPtr) this.<>m__0)))
                    {
                        string platformData = importer.GetPlatformData(BuildTarget.WSAPlayer, Plugin.sdkTag);
                        if ((string.IsNullOrEmpty(platformData) || string.Equals(platformData, 0.ToString(), StringComparison.InvariantCultureIgnoreCase)) || string.Equals(platformData, wsaSDK.ToString(), StringComparison.InvariantCultureIgnoreCase))
                        {
                            string str6 = importer.GetPlatformData(BuildTarget.WSAPlayer, Plugin.scriptingBackendTag);
                            bool flag2 = string.IsNullOrEmpty(str6) || string.Equals(str6, 0.ToString(), StringComparison.InvariantCultureIgnoreCase);
                            MetroPluginImporterExtension.MetroPluginScriptingBackend backend2 = MetroPluginImporterExtension.ToMetroPluginScriptingBackend(PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA));
                            if (((flag2 || string.Equals(str6, backend2.ToString(), StringComparison.InvariantCultureIgnoreCase)) && (importer.dllType != DllType.Unknown)) && (importer.dllType != DllType.Native))
                            {
                                plugin = new MetroAssemblyCSharpCreator.AssemblyCSharpPlugin {
                                    Name = Path.GetFileNameWithoutExtension(importer.assetPath),
                                    HintPath = Path.Combine(fullPath, importer.assetPath)
                                };
                                plugins.Add(plugin);
                            }
                        }
                    }
                }
            }
            if (Utility.UseIl2CppScriptingBackend())
            {
                strArray2 = new string[] { @"il2cpp\UnityEngine.dll" };
            }
            else
            {
                switch (wsaSDK)
                {
                    case WSASDK.SDK81:
                        strArray2 = new string[] { @"Store81\UnityEngine.dll", "WinRTLegacy.dll" };
                        goto Label_0324;

                    case WSASDK.PhoneSDK81:
                        strArray2 = new string[] { @"Phone\UnityEngine.dll", @"Phone\WinRTLegacy.dll" };
                        goto Label_0324;

                    case WSASDK.UWP:
                        strArray2 = new string[] { @"UAP\UnityEngine.dll", @"UAP\WinRTLegacy.dll" };
                        goto Label_0324;
                }
                throw new Exception(string.Format("Unknown Windows Store Apps SDK: {0}", wsaSDK));
            }
        Label_0324:
            strArray3 = strArray2;
            for (int i = 0; i < strArray3.Length; i++)
            {
                string path = strArray3[i];
                plugin = new MetroAssemblyCSharpCreator.AssemblyCSharpPlugin {
                    Name = Path.GetFileName(path)
                };
                string[] components = new string[] { playbackEngineDirectory, "Managed", path };
                plugin.HintPath = Paths.Combine(components);
                plugins.Add(plugin);
            }
            StringBuilder builder = new StringBuilder();
            foreach (string str8 in sourceFiles)
            {
                builder.AppendFormat("    <Compile Include=\"{0}\">", str8.Replace('/', '\\'));
                builder.AppendLine();
                builder.AppendFormat("      <Link>{0}</Link>", Path.GetFileName(str8));
                builder.AppendLine();
                builder.AppendLine("    </Compile>");
            }
            MetroAssemblyCSharpCreator.CreateAssemblyCSharp(project, playbackEngineDirectory, assemblyName, defines, plugins, builder.ToString(), "", "", wsaSDK, additionalProjectReferences);
        }

        [CompilerGenerated]
        private sealed class <GenerateCSharpProject>c__AnonStorey0
        {
            internal string extension;

            internal bool <>m__0(string e)
            {
                return string.Equals(this.extension, e, StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}

