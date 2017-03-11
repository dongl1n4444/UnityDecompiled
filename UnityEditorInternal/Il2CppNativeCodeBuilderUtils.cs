namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public static class Il2CppNativeCodeBuilderUtils
    {
        public static IEnumerable<string> AddBuilderArguments(Il2CppNativeCodeBuilder builder, string outputRelativePath, IEnumerable<string> includeRelativePaths, bool debugBuild)
        {
            List<string> list = new List<string> {
                "--compile-cpp",
                "--libil2cpp-static",
                FormatArgument("platform", builder.CompilerPlatform),
                FormatArgument("architecture", builder.CompilerArchitecture)
            };
            if (debugBuild)
            {
                list.Add(FormatArgument("configuration", "Debug"));
            }
            else
            {
                list.Add(FormatArgument("configuration", "Release"));
            }
            list.Add(FormatArgument("outputpath", builder.ConvertOutputFileToFullPath(outputRelativePath)));
            if (!string.IsNullOrEmpty(builder.CacheDirectory))
            {
                list.Add(FormatArgument("cachedirectory", CacheDirectoryPathFor(builder.CacheDirectory)));
            }
            if (!string.IsNullOrEmpty(builder.CompilerFlags))
            {
                list.Add(FormatArgument("compiler-flags", builder.CompilerFlags));
            }
            if (!string.IsNullOrEmpty(builder.LinkerFlags))
            {
                list.Add(FormatArgument("linker-flags", builder.LinkerFlags));
            }
            if (!string.IsNullOrEmpty(builder.PluginPath))
            {
                list.Add(FormatArgument("plugin", builder.PluginPath));
            }
            foreach (string str in builder.ConvertIncludesToFullPaths(includeRelativePaths))
            {
                list.Add(FormatArgument("additional-include-directories", str));
            }
            list.AddRange(builder.AdditionalIl2CPPArguments);
            return list;
        }

        private static string CacheDirectoryPathFor(string builderCacheDirectory) => 
            (builderCacheDirectory + "/il2cpp_cache");

        public static void ClearAndPrepareCacheDirectory(Il2CppNativeCodeBuilder builder)
        {
            string fullUnityVersion = InternalEditorUtility.GetFullUnityVersion();
            ClearCacheIfEditorVersionDiffers(builder, fullUnityVersion);
            PrepareCacheDirectory(builder, fullUnityVersion);
        }

        public static void ClearCacheIfEditorVersionDiffers(Il2CppNativeCodeBuilder builder, string currentEditorVersion)
        {
            string path = CacheDirectoryPathFor(builder.CacheDirectory);
            if (Directory.Exists(path) && !File.Exists(Path.Combine(builder.CacheDirectory, EditorVersionFilenameFor(currentEditorVersion))))
            {
                Directory.Delete(path, true);
            }
        }

        private static string EditorVersionFilenameFor(string editorVersion) => 
            $"il2cpp_cache {editorVersion}";

        private static string EscapeEmbeddedQuotes(string value) => 
            value.Replace("\"", "\\\"");

        private static string FormatArgument(string name, string value) => 
            $"--{name}="{EscapeEmbeddedQuotes(value)}"";

        public static string ObjectFilePathInCacheDirectoryFor(string builderCacheDirectory) => 
            CacheDirectoryPathFor(builderCacheDirectory);

        public static void PrepareCacheDirectory(Il2CppNativeCodeBuilder builder, string currentEditorVersion)
        {
            Directory.CreateDirectory(CacheDirectoryPathFor(builder.CacheDirectory));
            string path = Path.Combine(builder.CacheDirectory, EditorVersionFilenameFor(currentEditorVersion));
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
        }
    }
}

