namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public static class Il2CppNativeCodeBuilderUtils
    {
        public static IEnumerable<string> AddBuilderArguments(Il2CppNativeCodeBuilder builder, string outputRelativePath, IEnumerable<string> includeRelativePaths)
        {
            List<string> list = new List<string> {
                "--compile-cpp",
                "--libil2cpp-static",
                FormatArgument("platform", builder.CompilerPlatform),
                FormatArgument("architecture", builder.CompilerArchitecture),
                FormatArgument("configuration", "Release"),
                FormatArgument("outputpath", builder.ConvertOutputFileToFullPath(outputRelativePath))
            };
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

        private static string CacheDirectoryPathFor(string builderCacheDirectory)
        {
            return (builderCacheDirectory + "/il2cpp_cache");
        }

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

        private static string EditorVersionFilenameFor(string editorVersion)
        {
            return string.Format("il2cpp_cache {0}", editorVersion);
        }

        private static string FormatArgument(string name, string value)
        {
            return string.Format("--{0}=\"{1}\"", name, value);
        }

        public static string ObjectFilePathInCacheDirectoryFor(string builderCacheDirectory)
        {
            return CacheDirectoryPathFor(builderCacheDirectory);
        }

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

