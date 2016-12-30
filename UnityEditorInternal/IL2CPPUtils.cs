namespace UnityEditorInternal
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Utils;

    internal class IL2CPPUtils
    {
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache1;
        public const string BinaryMetadataSuffix = "-metadata.dat";

        internal static string ApiCompatibilityLevelToDotNetProfileArgument(ApiCompatibilityLevel compatibilityLevel)
        {
            switch (compatibilityLevel)
            {
                case ApiCompatibilityLevel.NET_2_0:
                    return "Net20";

                case ApiCompatibilityLevel.NET_2_0_Subset:
                    return "Unity";

                case ApiCompatibilityLevel.NET_4_6:
                    return "Net45";
            }
            throw new NotSupportedException($"ApiCompatibilityLevel.{compatibilityLevel} is not supported by IL2CPP!");
        }

        internal static void CopyConfigFiles(string tempFolder, string destinationFolder)
        {
            string[] components = new string[] { IL2CPPBuilder.GetCppOutputPath(tempFolder), "Data", "etc" };
            FileUtil.CopyDirectoryRecursive(Paths.Combine(components), destinationFolder);
        }

        internal static void CopyEmbeddedResourceFiles(string tempFolder, string destinationFolder)
        {
            string[] components = new string[] { IL2CPPBuilder.GetCppOutputPath(tempFolder), "Data", "Resources" };
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = f => f.EndsWith("-resources.dat");
            }
            foreach (string str in Enumerable.Where<string>(Directory.GetFiles(Paths.Combine(components)), <>f__am$cache0))
            {
                string[] textArray2 = new string[] { destinationFolder, Path.GetFileName(str) };
                File.Copy(str, Paths.Combine(textArray2), true);
            }
        }

        internal static void CopyMetadataFiles(string tempFolder, string destinationFolder)
        {
            string[] components = new string[] { IL2CPPBuilder.GetCppOutputPath(tempFolder), "Data", "Metadata" };
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = f => f.EndsWith("-metadata.dat");
            }
            foreach (string str in Enumerable.Where<string>(Directory.GetFiles(Paths.Combine(components)), <>f__am$cache1))
            {
                string[] textArray2 = new string[] { destinationFolder, Path.GetFileName(str) };
                File.Copy(str, Paths.Combine(textArray2), true);
            }
        }

        internal static void CopySymmapFile(string tempFolder, string destinationFolder)
        {
            CopySymmapFile(tempFolder, destinationFolder, string.Empty);
        }

        internal static void CopySymmapFile(string tempFolder, string destinationFolder, string destinationFileNameSuffix)
        {
            string path = Path.Combine(tempFolder, "SymbolMap");
            if (File.Exists(path))
            {
                File.Copy(path, Path.Combine(destinationFolder, "SymbolMap" + destinationFileNameSuffix), true);
            }
        }

        internal static IIl2CppPlatformProvider PlatformProviderForNotModularPlatform(BuildTarget target, bool developmentBuild)
        {
            throw new Exception("Platform unsupported, or already modular.");
        }

        internal static IL2CPPBuilder RunCompileAndLink(string tempFolder, string stagingAreaData, IIl2CppPlatformProvider platformProvider, Action<string> modifyOutputBeforeCompile, RuntimeClassRegistry runtimeClassRegistry, bool debugBuild)
        {
            IL2CPPBuilder builder = new IL2CPPBuilder(tempFolder, stagingAreaData, platformProvider, modifyOutputBeforeCompile, runtimeClassRegistry, debugBuild);
            builder.RunCompileAndLink();
            return builder;
        }

        internal static IL2CPPBuilder RunIl2Cpp(string stagingAreaData, IIl2CppPlatformProvider platformProvider, Action<string> modifyOutputBeforeCompile, RuntimeClassRegistry runtimeClassRegistry, bool debugBuild)
        {
            IL2CPPBuilder builder = new IL2CPPBuilder(stagingAreaData, stagingAreaData, platformProvider, modifyOutputBeforeCompile, runtimeClassRegistry, debugBuild);
            builder.Run();
            return builder;
        }

        internal static IL2CPPBuilder RunIl2Cpp(string tempFolder, string stagingAreaData, IIl2CppPlatformProvider platformProvider, Action<string> modifyOutputBeforeCompile, RuntimeClassRegistry runtimeClassRegistry, bool debugBuild)
        {
            IL2CPPBuilder builder = new IL2CPPBuilder(tempFolder, stagingAreaData, platformProvider, modifyOutputBeforeCompile, runtimeClassRegistry, debugBuild);
            builder.Run();
            return builder;
        }

        internal static string editorIl2cppFolder
        {
            get
            {
                string str = "il2cpp";
                return Path.GetFullPath(Path.Combine(EditorApplication.applicationContentsPath, str));
            }
        }
    }
}

