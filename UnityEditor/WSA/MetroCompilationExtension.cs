namespace UnityEditor.WSA
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEditor.Scripting.Compilers;

    internal class MetroCompilationExtension : DefaultCompilationExtension
    {
        [CompilerGenerated]
        private static Func<CustomAttribute, bool> <>f__am$cache0;
        private static List<string> userScriptAssemblies;

        static MetroCompilationExtension()
        {
            string[] collection = new string[] { "Assembly-CSharp", "Assembly-CSharp-firstpass", "Assembly-UnityScript", "Assembly-UnityScript-firstpass", "Assembly-Boo", "Assembly-Boo-firstpass" };
            userScriptAssemblies = new List<string>(collection);
        }

        public override IAssemblyResolver GetAssemblyResolver(bool buildingForEditor, string assemblyPath, string[] searchDirectories)
        {
            if (!buildingForEditor)
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = new Func<CustomAttribute, bool>(null, (IntPtr) <GetAssemblyResolver>m__0);
                }
                CustomAttribute attribute = Enumerable.FirstOrDefault<CustomAttribute>(AssemblyDefinition.ReadAssembly(assemblyPath).CustomAttributes, <>f__am$cache0);
                if (attribute != null)
                {
                    CustomAttributeArgument argument = attribute.ConstructorArguments[0];
                    string str = (string) argument.Value;
                    if (str == ".NETCore,Version=v5.0")
                    {
                        NuGetAssemblyResolver resolver = new NuGetAssemblyResolver(@"UWP\project.lock.json");
                        if (searchDirectories != null)
                        {
                            foreach (string str2 in searchDirectories)
                            {
                                resolver.AddSearchDirectory(str2);
                            }
                        }
                        return resolver;
                    }
                }
            }
            return base.GetAssemblyResolver(buildingForEditor, assemblyPath, searchDirectories);
        }

        public override string[] GetCompilerExtraAssemblyPaths(bool isEditor, string assemblyPathName)
        {
            List<string> list = new List<string>();
            list.AddRange(base.GetCompilerExtraAssemblyPaths(isEditor, assemblyPathName));
            bool flag = false;
            if (this.IsUserScriptAssembly(assemblyPathName))
            {
                if (this.GetCsCompiler(isEditor, assemblyPathName) == CSharpCompiler.Microsoft)
                {
                    flag = true;
                }
            }
            else
            {
                flag = !isEditor;
            }
            if (flag)
            {
                WSASDK wsaSDK = EditorUserBuildSettings.wsaSDK;
                bool flag2 = PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA) == ScriptingImplementation.IL2CPP;
                if (wsaSDK == WSASDK.UniversalSDK81)
                {
                    wsaSDK = WSASDK.SDK81;
                }
                if ((wsaSDK != WSASDK.UWP) || flag2)
                {
                    list.Add(MicrosoftCSharpCompiler.GetNETCoreFrameworkReferencesDirectory(wsaSDK));
                }
            }
            return list.ToArray();
        }

        public override CSharpCompiler GetCsCompiler(bool buildingForEditor, string assemblyName)
        {
            if (!buildingForEditor)
            {
                assemblyName = Path.GetFileNameWithoutExtension(assemblyName);
                PlayerSettings.WSACompilationOverrides compilationOverrides = PlayerSettings.WSA.compilationOverrides;
                if (compilationOverrides != PlayerSettings.WSACompilationOverrides.UseNetCore)
                {
                    if ((compilationOverrides == PlayerSettings.WSACompilationOverrides.UseNetCorePartially) && (string.Compare(assemblyName, Utility.AssemblyCSharpName, true) == 0))
                    {
                        return CSharpCompiler.Microsoft;
                    }
                }
                else if ((string.Compare(assemblyName, Utility.AssemblyCSharpName, true) == 0) || (string.Compare(assemblyName, Utility.AssemblyCSharpFirstPassName, true) == 0))
                {
                    return CSharpCompiler.Microsoft;
                }
            }
            return CSharpCompiler.Mono;
        }

        private bool IsUserScriptAssembly(string assemblyPathName)
        {
            <IsUserScriptAssembly>c__AnonStorey0 storey = new <IsUserScriptAssembly>c__AnonStorey0 {
                assemblyName = Path.GetFileNameWithoutExtension(assemblyPathName),
                suffix = EditorSettings.Internal_UserGeneratedProjectSuffix
            };
            return Enumerable.Any<string>(userScriptAssemblies, new Func<string, bool>(storey, (IntPtr) this.<>m__0));
        }

        [CompilerGenerated]
        private sealed class <IsUserScriptAssembly>c__AnonStorey0
        {
            internal string assemblyName;
            internal string suffix;

            internal bool <>m__0(string x) => 
                x.Equals(this.assemblyName + this.suffix, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}

