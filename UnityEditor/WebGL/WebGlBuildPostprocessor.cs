namespace UnityEditor.WebGL
{
    using Mono.Unix.Native;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.BuildReporting;
    using UnityEditor.Modules;
    using UnityEditor.Utils;
    using UnityEditor.WebGL.Emscripten;
    using UnityEditor.WebGL.Il2Cpp;
    using UnityEditorInternal;
    using UnityEngine;

    internal class WebGlBuildPostprocessor : DefaultBuildPostprocessor
    {
        [CompilerGenerated]
        private static Func<string, string, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Comparison<KeyValuePair<string, string>> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache3;
        private const string kDataFolderDevelopment = "Development";
        private const string kDataFolderRelease = "Release";
        private const string kFileTypeData = "Asset Data";
        private const string kFileTypeJS = "Executable";
        private const string kFileTypeMem = "Memory Initializer";
        private const string kFileTypeTemplate = "WebGL Template";
        private const string kIl2CppDataDirName = "Il2CppData";
        private const string kInternalFilename = "build";
        private const string kInternalTemplateFolder = "Template";
        private const string kOutputAsmJsExtension = ".asm.js";
        private const string kOutputBackgroundFileName = "background.jpg";
        private const string kOutputCompressedExtension = ".compressed";
        private const string kOutputDataExtension = ".data";
        private const string kOutputDebugSymbolsExtension = ".debugSymbols.js";
        private const string kOutputFileLoaderFileName = "fileloader.js";
        private const string kOutputHtmlExtension = ".html";
        private const string kOutputHtmlFileName = "index.html";
        private const string kOutputJsExtension = ".js";
        private const string kOutputMappedGlobalsExtension = ".wasm.mappedGlobals";
        private const string kOutputMemExtension = ".mem";
        private const string kOutputSymbolsExtension = ".js.symbols";
        private const string kOutputWasmExtension = ".wasm";
        private const string kResourcesDirName = "Resources";
        private const string kResourcesExtraFileName = "unity_builtin_extra";
        private const string kResourcesFileName = "unity_default_resources";
        private const string kUnityLoaderGlue = "WebGLLoaderGlue.html";
        private const string kUnityLoaderJs = "UnityLoader.js";
        private WebGlIl2CppPlatformProvider m_PlatformProvider;
        private readonly ProgressHelper m_Progress = new ProgressHelper();
        private RuntimeClassRegistry m_RCR;
        private string m_UnityEngineDll;

        internal static string ArgumentsForEmscripten(BuildPostProcessArgs args)
        {
            bool flag = (args.options & BuildOptions.Development) != BuildOptions.CompressTextures;
            bool flag2 = flag && EditorUserBuildSettings.webGLUsePreBuiltUnityEngine;
            string str = "-s PRECISE_F32=2 ";
            bool flag3 = PlayerSettings.WebGL.exceptionSupport != WebGLExceptionSupport.None;
            string str2 = str;
            object[] objArray1 = new object[] { str2, "-s DISABLE_EXCEPTION_CATCHING=", !flag3 ? 1 : 0, " " };
            str2 = string.Concat(objArray1);
            str = string.Concat(new object[] { str2, "--memory-init-file ", !flag2 ? 1 : 0, " " }) + "-O3 " + "-s NO_EXIT_RUNTIME=1 ";
            if (flag)
            {
                str = (str + "-g2 ") + "-s ASSERTIONS=1 " + "-s DEMANGLE_SUPPORT=1 ";
            }
            else
            {
                str = str + "-g0 ";
            }
            str = str + "-s USE_WEBGL2=1 ";
            if (PlayerSettings.WebGL.useWasm)
            {
                str = str + "-s BINARYEN=1 ";
            }
            string str3 = ((uint) ((PlayerSettings.WebGL.memorySize * 0x400) * 0x400)).ToString();
            str = str + "-s TOTAL_MEMORY=" + str3 + " ";
            if (!flag2)
            {
                str = str + "--separate-asm ";
            }
            str = str + "--emit-symbol-map " + "--output_eol linux ";
            if (flag2)
            {
                str = str + "-s SIDE_MODULE=1 -s ASM_JS=2 ";
            }
            return (str + "-s MEMFS_APPEND_TO_TYPED_ARRAYS=1 " + PlayerSettings.WebGL.emscriptenArgs);
        }

        private void BuildStep(BuildPostProcessArgs args, string title, string description)
        {
            this.m_Progress.Show(title, description);
            args.report.BeginBuildStep(description);
        }

        private static void CompressFilesInOutputDirectory(BuildPostProcessArgs args)
        {
            string compressedFileExtension = GetCompressedFileExtension();
            if (compressedFileExtension != null)
            {
                string[] components = new string[] { args.installPath, DataFolderName(args) };
                string path = Paths.Combine(components);
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = f => ((f.EndsWith(".js") || f.EndsWith(".mem")) || ((f.EndsWith(".data") || f.EndsWith(".wasm.mappedGlobals")) || f.EndsWith(".wasm"))) && !f.EndsWith("UnityLoader.js");
                }
                IEnumerable<string> enumerable = Enumerable.Where<string>(Directory.GetFiles(path), <>f__am$cache1);
                foreach (string str3 in enumerable)
                {
                    string[] textArray2 = new string[] { path, Path.GetFileName(str3) };
                    string str4 = Paths.Combine(textArray2) + compressedFileExtension;
                    File.Delete(str4);
                    ProgramUtils.StartProgramChecked(new Program(GetCompressionStartInfo(str3, str4)));
                    if (File.Exists(str3))
                    {
                        File.Delete(str3);
                    }
                    string role = "Executable";
                    if (str3.EndsWith(".mem"))
                    {
                        role = "Memory Initializer";
                    }
                    else if (str3.EndsWith(".data"))
                    {
                        role = "Asset Data";
                    }
                    args.report.AddFile(str4, role);
                }
            }
        }

        private void CopyFileFromBuildToolsToInstallPath(BuildPostProcessArgs args, string file)
        {
            string[] components = new string[] { EmscriptenPaths.buildToolsDir, file };
            string[] textArray2 = new string[] { args.installPath, DataFolderName(args), file };
            File.Copy(Paths.Combine(components), Paths.Combine(textArray2), true);
        }

        private void CopyFileFromStagingAreaToInstallPath(BuildPostProcessArgs args, string file)
        {
            string[] components = new string[] { args.stagingAreaData, file };
            string[] textArray2 = new string[] { args.installPath, DataFolderName(args), file };
            File.Copy(Paths.Combine(components), Paths.Combine(textArray2), true);
        }

        private void CopyFilesFromStagingAreaToInstallPath(BuildPostProcessArgs args, IEnumerable<string> files)
        {
            foreach (string str in files)
            {
                this.CopyFileFromStagingAreaToInstallPath(args, str);
            }
        }

        private void CopyFinalFilesToInstallPath(BuildPostProcessArgs args)
        {
            args.report.DeleteFilesRecursive(args.stagingAreaData);
            string[] components = new string[] { args.installPath, "Development" };
            string path = Paths.Combine(components);
            if (Directory.Exists(path))
            {
                FileUtil.UnityDirectoryDelete(path, true);
            }
            string[] textArray2 = new string[] { args.installPath, "Release" };
            string str2 = Paths.Combine(textArray2);
            if (Directory.Exists(str2))
            {
                FileUtil.UnityDirectoryDelete(str2, true);
            }
            string fileName = Path.GetFileName(args.installPath);
            string[] textArray3 = new string[] { args.installPath, DataFolderName(args) };
            string str4 = Paths.Combine(textArray3);
            Directory.CreateDirectory(str4);
            string templateFolder = GetTemplateFolder();
            string[] textArray4 = new string[] { args.stagingAreaData, "Template" };
            string dir = Paths.Combine(textArray4);
            FileUtil.CreateOrCleanDirectory(dir);
            FileUtil.CopyDirectoryRecursiveForPostprocess(templateFolder, dir, true);
            FileUtil.UnityDirectoryRemoveReadonlyAttribute(dir);
            string[] textArray5 = new string[] { dir, "thumbnail.png" };
            File.Delete(Paths.Combine(textArray5));
            string[] textArray6 = new string[] { dir, "index.html" };
            ProcessTemplateFile(args, Paths.Combine(textArray6));
            FileUtil.CopyDirectoryRecursive(dir, args.installPath, true);
            args.report.AddFilesRecursive(dir, "WebGL Template");
            args.report.RelocateFiles(dir, args.installPath);
            bool flag = (args.options & BuildOptions.Development) != BuildOptions.CompressTextures;
            bool flag2 = flag && EditorUserBuildSettings.webGLUsePreBuiltUnityEngine;
            if (flag2)
            {
                string[] textArray7 = new string[] { EmscriptenPaths.buildToolsDir, "lib", "UnityNativeJs", "UnityNative" };
                string str7 = Paths.Combine(textArray7);
                string[] textArray8 = new string[] { str4, "UnityEngine.js" };
                string destFileName = Paths.Combine(textArray8);
                string[] textArray9 = new string[] { str4, "UnityEngine.asm.js" };
                string str9 = Paths.Combine(textArray9);
                string[] textArray10 = new string[] { str4, "UnityEngine.mem" };
                string str10 = Paths.Combine(textArray10);
                File.Copy(str7 + ".js", destFileName, true);
                File.Copy(str7 + ".js.mem", str10, true);
                File.Copy(str7 + ".asm.js", str9, true);
                args.report.AddFile(destFileName, "Executable");
                args.report.AddFile(str9, "Executable");
                args.report.AddFile(str10, "Memory Initializer");
            }
            List<string> list2 = new List<string> {
                fileName + ".data",
                fileName + ".js",
                "UnityLoader.js"
            };
            List<string> files = list2;
            this.CopyFilesFromStagingAreaToInstallPath(args, files);
            bool flag3 = !flag && (GetCompressedFileExtension() != null);
            if (!flag3)
            {
                string[] textArray11 = new string[] { str4, fileName + ".data" };
                args.report.AddFile(Paths.Combine(textArray11), "Asset Data");
                string[] textArray12 = new string[] { str4, fileName + ".js" };
                args.report.AddFile(Paths.Combine(textArray12), "Executable");
            }
            string[] textArray13 = new string[] { str4, "UnityLoader.js" };
            args.report.AddFile(Paths.Combine(textArray13), "Executable");
            if (PlayerSettings.WebGL.useWasm)
            {
                list2 = new List<string> {
                    fileName + ".wasm",
                    fileName + ".wasm.mappedGlobals",
                    fileName + ".asm.js"
                };
                files = list2;
                this.CopyFilesFromStagingAreaToInstallPath(args, files);
                string[] textArray14 = new string[] { str4, fileName + ".wasm" };
                args.report.AddFile(Paths.Combine(textArray14), "Executable");
                string[] textArray15 = new string[] { str4, fileName + ".wasm.mappedGlobals" };
                args.report.AddFile(Paths.Combine(textArray15), "Executable");
            }
            if (!flag2)
            {
                this.CopyFileFromStagingAreaToInstallPath(args, fileName + ".mem");
                this.CopyFileFromStagingAreaToInstallPath(args, fileName + ".asm.js");
                if (!flag3)
                {
                    string[] textArray16 = new string[] { str4, fileName + ".mem" };
                    args.report.AddFile(Paths.Combine(textArray16), "Memory Initializer");
                    string[] textArray17 = new string[] { str4, fileName + ".asm.js" };
                    args.report.AddFile(Paths.Combine(textArray17), "Executable");
                }
            }
            if (!flag && PlayerSettings.WebGL.debugSymbols)
            {
                this.CopyFileFromStagingAreaToInstallPath(args, fileName + ".debugSymbols.js");
                string[] textArray18 = new string[] { str4, fileName + ".debugSymbols.js" };
                args.report.AddFile(Paths.Combine(textArray18), "Executable");
            }
            PostprocessBuildPlayer.InstallStreamingAssets(args.installPath, args.report);
        }

        private static string DataFolderName(BuildPostProcessArgs args)
        {
            if ((args.options & BuildOptions.Development) != BuildOptions.CompressTextures)
            {
                return "Development";
            }
            return "Release";
        }

        private static UnityType FindTypeByNameChecked(string name)
        {
            UnityType type = UnityType.FindTypeByName(name);
            if (type == null)
            {
                throw new ArgumentException($"Could not map typename '{name}' to type info (WebGL class registration skipped classes)");
            }
            return type;
        }

        private void GenerateDebugSymbolsJs(BuildPostProcessArgs args, string filename, Dictionary<string, string> minificationMap)
        {
            if (minificationMap == null)
            {
                minificationMap = CodeAnalysisUtils.ReadMinificationMap(filename + ".js.symbols");
            }
            List<KeyValuePair<string, string>> list = minificationMap.ToList<KeyValuePair<string, string>>();
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = (a, b) => string.Compare(a.Value, b.Value);
            }
            list.Sort(<>f__am$cache2);
            string[] components = new string[] { args.stagingAreaData, filename + ".debugSymbols.js" };
            using (StreamWriter writer = new StreamWriter(Paths.Combine(components)))
            {
                writer.WriteLine("Module.debugSymbols = {");
                foreach (KeyValuePair<string, string> pair in list)
                {
                    writer.WriteLine(pair.Key + ":'" + pair.Value + "',");
                }
                writer.WriteLine("};");
                writer.WriteLine(File.ReadAllText(Path.Combine(EmscriptenPaths.buildToolsDir, "Demangle.js")));
            }
        }

        private static string GetCompressedFileExtension()
        {
            switch (PlayerSettings.WebGL.compressionFormat)
            {
                case WebGLCompressionFormat.Brotli:
                    return "br";

                case WebGLCompressionFormat.Gzip:
                    return "gz";
            }
            return null;
        }

        private static ProcessStartInfo GetCompressionStartInfo(string file, string destFile)
        {
            ProcessStartInfo info2;
            WebGLCompressionFormat compressionFormat = PlayerSettings.WebGL.compressionFormat;
            if (compressionFormat != WebGLCompressionFormat.Brotli)
            {
                if (compressionFormat != WebGLCompressionFormat.Gzip)
                {
                    return null;
                }
            }
            else
            {
                string[] components = new string[] { EmscriptenPaths.buildToolsDir, "Brotli" };
                string str = Paths.Combine(components);
                info2 = new ProcessStartInfo(EmscriptenPaths.pythonExecutable);
                string[] textArray2 = new string[7];
                textArray2[0] = "\"";
                string[] textArray3 = new string[] { str, "python", "bro.py" };
                textArray2[1] = Paths.Combine(textArray3);
                textArray2[2] = "\" -o \"";
                textArray2[3] = destFile;
                textArray2[4] = "\" -i \"";
                textArray2[5] = file;
                textArray2[6] = "\"";
                info2.Arguments = string.Concat(textArray2);
                info2.WorkingDirectory = Directory.GetCurrentDirectory();
                info2.UseShellExecute = false;
                info2.CreateNoWindow = true;
                ProcessStartInfo info = info2;
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    string[] textArray4 = new string[] { str, "dist", "lib.win-amd64-2.7" };
                    info.EnvironmentVariables["PYTHONPATH"] = Paths.Combine(textArray4);
                    return info;
                }
                string[] textArray5 = new string[] { str, "dist", "Brotli-0.4.0-py2.7-macosx-10.11-intel.egg" };
                info.EnvironmentVariables["PYTHONPATH"] = Paths.Combine(textArray5);
                return info;
            }
            if (Application.platform == RuntimePlatform.LinuxEditor)
            {
                return new ProcessStartInfo("gzip") { 
                    Arguments = $"-9 --keep -S gz "{file}"",
                    WorkingDirectory = Directory.GetCurrentDirectory(),
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
            }
            string str2 = "7za";
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                str2 = "7z.exe";
            }
            info2 = new ProcessStartInfo(EditorApplication.applicationContentsPath + "/Tools/" + str2);
            string[] textArray6 = new string[] { "a -tgzip \"", destFile, "\" \"", file, "\"" };
            info2.Arguments = string.Concat(textArray6);
            info2.WorkingDirectory = Directory.GetCurrentDirectory();
            info2.UseShellExecute = false;
            info2.CreateNoWindow = true;
            return info2;
        }

        private static string GetDecompressor()
        {
            string[] components = new string[] { EmscriptenPaths.buildToolsDir, "Decompressors" };
            string str = Paths.Combine(components);
            WebGLCompressionFormat compressionFormat = PlayerSettings.WebGL.compressionFormat;
            if (compressionFormat != WebGLCompressionFormat.Brotli)
            {
                if (compressionFormat != WebGLCompressionFormat.Gzip)
                {
                    return null;
                }
            }
            else
            {
                string[] textArray2 = new string[] { str, "brotli.js" };
                return Paths.Combine(textArray2);
            }
            string[] textArray3 = new string[] { str, "pako_inflate.js" };
            return Paths.Combine(textArray3);
        }

        internal static string[] GetJSLib(BuildPostProcessArgs args)
        {
            bool flag2 = ((args.options & BuildOptions.Development) != BuildOptions.CompressTextures) && EditorUserBuildSettings.webGLUsePreBuiltUnityEngine;
            string buildToolsDir = EmscriptenPaths.buildToolsDir;
            List<string> list = new List<string>();
            if (!flag2)
            {
                string[] components = new string[] { buildToolsDir, "lib" };
                string path = Paths.Combine(components);
                string[] files = Directory.GetFiles(path);
                foreach (string str3 in files)
                {
                    if (Path.GetExtension(str3) == ".js")
                    {
                        string[] textArray2 = new string[] { path, Path.GetFileName(str3) };
                        list.Add(Paths.Combine(textArray2));
                    }
                }
                foreach (PluginImporter importer in PluginImporter.GetImporters(args.target))
                {
                    if (importer.isNativePlugin && (Path.GetExtension(importer.assetPath) == ".jslib"))
                    {
                        list.Add(Path.GetFullPath(importer.assetPath));
                    }
                }
            }
            return list.ToArray();
        }

        internal static string[] GetJSPre(BuildPostProcessArgs args)
        {
            bool flag2 = ((args.options & BuildOptions.Development) != BuildOptions.CompressTextures) && EditorUserBuildSettings.webGLUsePreBuiltUnityEngine;
            string buildToolsDir = EmscriptenPaths.buildToolsDir;
            List<string> list = new List<string>();
            if (flag2)
            {
                bool flag3 = false;
                foreach (PluginImporter importer in PluginImporter.GetImporters(args.target))
                {
                    if (importer.isNativePlugin && (Path.GetExtension(importer.assetPath) == ".jslib"))
                    {
                        if (!flag3)
                        {
                            string[] textArray1 = new string[] { buildToolsDir, "DynamicJslibLoader.js" };
                            list.Add(Paths.Combine(textArray1));
                            flag3 = true;
                        }
                        string[] components = new string[] { args.stagingAreaData, "jspre", importer.assetPath };
                        string path = Paths.Combine(components);
                        Directory.CreateDirectory(Path.GetDirectoryName(path));
                        File.WriteAllText(path, File.ReadAllText(Path.GetFullPath(importer.assetPath)));
                        list.Add(Path.GetFullPath(path));
                    }
                }
            }
            else
            {
                string[] textArray3 = new string[] { buildToolsDir, "prejs" };
                string str3 = Paths.Combine(textArray3);
                string[] files = Directory.GetFiles(str3);
                foreach (string str4 in files)
                {
                    if (Path.GetExtension(str4) == ".js")
                    {
                        string[] textArray4 = new string[] { str3, Path.GetFileName(str4) };
                        list.Add(Paths.Combine(textArray4));
                    }
                }
                if (PlayerSettings.WebGL.exceptionSupport == WebGLExceptionSupport.None)
                {
                    string[] textArray5 = new string[] { buildToolsDir, "ExceptionLogger.js" };
                    list.Add(Paths.Combine(textArray5));
                }
            }
            return list.ToArray();
        }

        internal static string[] GetModules(BuildPostProcessArgs args)
        {
            bool flag = (args.options & BuildOptions.Development) != BuildOptions.CompressTextures;
            bool flag2 = flag && EditorUserBuildSettings.webGLUsePreBuiltUnityEngine;
            string buildToolsDir = EmscriptenPaths.buildToolsDir;
            List<string> list = new List<string>();
            if (!flag2)
            {
                string modulesDirectory = PlayerSettings.WebGL.modulesDirectory;
                if (string.IsNullOrEmpty(modulesDirectory))
                {
                    string[] components = new string[] { buildToolsDir, "lib", "modules" };
                    modulesDirectory = Paths.Combine(components);
                    if (flag)
                    {
                        modulesDirectory = modulesDirectory + "_development";
                    }
                }
                else if (!Path.IsPathRooted(modulesDirectory))
                {
                    string[] textArray2 = new string[] { buildToolsDir, modulesDirectory };
                    modulesDirectory = Paths.Combine(textArray2);
                }
                list.AddRange(Directory.GetFiles(modulesDirectory, "*.bc"));
            }
            foreach (PluginImporter importer in PluginImporter.GetImporters(args.target))
            {
                if (importer.isNativePlugin)
                {
                    string extension = Path.GetExtension(importer.assetPath);
                    if (((extension != ".jslib") && (extension != ".jspre")) && (extension != ".h"))
                    {
                        list.Add(Path.GetFullPath(importer.assetPath));
                    }
                }
            }
            return list.ToArray();
        }

        private WebGlIl2CppPlatformProvider GetPlatformProvider(BuildTarget target, string dataDirectory, bool developmentBuild, BuildReport buildReport) => 
            new WebGlIl2CppPlatformProvider(target, developmentBuild, dataDirectory, "build.js", buildReport);

        private static string GetTemplateFolder()
        {
            string dataPath;
            char[] separator = new char[] { ':' };
            string[] strArray = PlayerSettings.WebGL.template.Split(separator);
            if (strArray[0].Equals("PROJECT"))
            {
                dataPath = EmscriptenPaths.dataPath;
            }
            else
            {
                dataPath = EmscriptenPaths.buildToolsDir;
            }
            string[] components = new string[] { dataPath, "WebGLTemplates", strArray[1] };
            dataPath = Paths.Combine(components);
            if (!Directory.Exists(dataPath))
            {
                throw new Exception("Invalid WebGL template path: " + dataPath + "! Select a template in player settings.");
            }
            string[] textArray2 = new string[] { dataPath, "index.html" };
            if (!File.Exists(Paths.Combine(textArray2)))
            {
                throw new Exception("Invalid WebGL template selection: " + dataPath + "! Select a template in player settings.");
            }
            return dataPath;
        }

        public override void LaunchPlayer(BuildLaunchPlayerArgs args)
        {
            int num;
            HttpServerEditorWrapper.CreateIfNeeded(args.installPath, out num);
            Application.OpenURL("http://localhost:" + num + "/");
        }

        private static void MergeJSFiles(IEnumerable<string> inputs, string output)
        {
            string contents = "";
            foreach (string str2 in inputs)
            {
                if (str2 != null)
                {
                    IEnumerable<string> files;
                    if (Directory.Exists(str2))
                    {
                        files = Directory.GetFiles(str2, "*.js");
                    }
                    else
                    {
                        string[] textArray1 = new string[] { str2 };
                        files = textArray1;
                    }
                    foreach (string str3 in files)
                    {
                        if (contents.Length != 0)
                        {
                            contents = contents + '\n';
                        }
                        contents = contents + File.ReadAllText(str3);
                    }
                }
            }
            File.WriteAllText(output, contents);
        }

        private static void MinifyJS(string path)
        {
            ProcessStartInfo info2 = new ProcessStartInfo(EmscriptenPaths.nodeExecutable);
            string[] textArray1 = new string[7];
            textArray1[0] = " \"";
            string[] components = new string[] { EmscriptenPaths.buildToolsDir, "uglify-js", "bin", "uglifyjs" };
            textArray1[1] = Paths.Combine(components);
            textArray1[2] = "\" \"";
            textArray1[3] = path;
            textArray1[4] = "\" -c -m -o \"";
            textArray1[5] = path;
            textArray1[6] = "\"";
            info2.Arguments = string.Concat(textArray1);
            info2.UseShellExecute = false;
            info2.CreateNoWindow = true;
            ProcessStartInfo si = info2;
            ProgramUtils.StartProgramChecked(new Program(si));
        }

        private void ModifyIl2CppOutputDirBeforeCompile(string outputDir)
        {
            string directoryName = Path.GetDirectoryName(Path.GetFullPath(this.m_UnityEngineDll));
            string[] components = new string[] { directoryName, "UnityICallRegistration.cpp" };
            string[] textArray2 = new string[] { outputDir, "UnityICallRegistration.cpp" };
            File.Copy(Paths.Combine(components), Paths.Combine(textArray2));
            this.m_RCR.SynchronizeClasses();
            UnityType[] classesToSkip = new UnityType[] { FindTypeByNameChecked("MasterServerInterface"), FindTypeByNameChecked("NetworkManager"), FindTypeByNameChecked("NetworkView"), FindTypeByNameChecked("ClusterInputManager"), FindTypeByNameChecked("WorldAnchor"), FindTypeByNameChecked("MovieTexture"), FindTypeByNameChecked("NScreenBridge") };
            string[] textArray3 = new string[] { directoryName, "ICallSummary.txt" };
            CodeStrippingUtils.WriteModuleAndClassRegistrationFile(directoryName, Paths.Combine(textArray3), outputDir, this.m_RCR, classesToSkip, this.m_PlatformProvider);
            EmscriptenCompiler.CleanupAndCreateEmscriptenDirs();
        }

        private static bool PackageData(string filename, BuildPostProcessArgs args, IEnumerable<string> filesToShip)
        {
            string str = (("\"" + EmscriptenPaths.packager + "\"") + " \"" + filename + ".data\"") + " --no-heap-copy" + " --js-output=\"fileloader.js\"";
            if (PlayerSettings.WebGL.dataCaching)
            {
                str = str + " --use-preload-cache";
            }
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = (current, file) => current + " \"" + Path.GetFileName(file) + "\"";
            }
            string[] components = new string[] { "Resources", "unity_default_resources" };
            string[] textArray2 = new string[] { "Resources", "unity_builtin_extra" };
            string[] textArray3 = new string[] { "Managed", "mono", "2.0", "machine.config" };
            str = ((((str + " --preload") + Enumerable.Aggregate<string, string>(filesToShip, "", <>f__am$cache0) + " \"Il2CppData\"") + " \"" + Paths.Combine(components) + "\"") + " \"" + Paths.Combine(textArray2) + "\"") + " \"" + Paths.Combine(textArray3) + "\"";
            ProcessStartInfo startInfo = new ProcessStartInfo(EmscriptenPaths.pythonExecutable) {
                Arguments = str,
                WorkingDirectory = args.stagingAreaData,
                UseShellExecute = false
            };
            EmccArguments.SetupDefaultEmscriptenEnvironment(startInfo);
            return ProgramUtils.StartProgramChecked(new Program(startInfo));
        }

        public override void PostProcess(BuildPostProcessArgs args)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                HttpServerEditorWrapper.Kill();
            }
            this.m_Progress.Reset(1f);
            string buildToolsDir = EmscriptenPaths.buildToolsDir;
            string fileName = Path.GetFileName(args.installPath);
            bool developmentBuild = (args.options & BuildOptions.Development) != BuildOptions.CompressTextures;
            WebGLStrippingInfo info = ScriptableObject.CreateInstance<WebGLStrippingInfo>();
            if (PlayerSettings.WebGL.useWasm)
            {
                string[] textArray1 = new string[] { args.stagingAreaData, fileName + ".wasm" };
                info.builtCodePath = Paths.Combine(textArray1);
            }
            else
            {
                string[] textArray2 = new string[] { args.stagingAreaData, fileName + ".asm.js" };
                info.builtCodePath = Paths.Combine(textArray2);
            }
            info.developmentBuild = developmentBuild;
            args.report.AddAppendix(info);
            Directory.CreateDirectory(args.installPath);
            string[] components = new string[] { buildToolsDir, "data", "unity_default_resources" };
            string[] textArray4 = new string[] { args.stagingAreaData, "Resources", "unity_default_resources" };
            File.Copy(Paths.Combine(components), Paths.Combine(textArray4));
            this.BuildStep(args, "Scripting", "Convert and compile scripting files");
            bool flag2 = developmentBuild && EditorUserBuildSettings.webGLUsePreBuiltUnityEngine;
            string[] textArray5 = new string[] { EmscriptenPaths.buildToolsDir, "lib", "UnityNativeJs", "UnityNative.js" };
            string path = Paths.Combine(textArray5);
            if (flag2 && (!File.Exists(path) || !File.Exists(path + ".mem")))
            {
                throw new Exception("'Fast Rebuild' option requires prebuilt JavaScript version of Unity engine. The following files are missing: " + (File.Exists(path) ? "" : ("\n" + path)) + (File.Exists(path + ".mem") ? "" : ("\n" + path + ".mem")));
            }
            this.m_RCR = args.usedClassRegistry;
            string[] textArray6 = new string[] { args.stagingAreaData, "Managed", "UnityEngine.dll" };
            this.m_UnityEngineDll = Path.GetFullPath(Paths.Combine(textArray6));
            this.m_PlatformProvider = this.GetPlatformProvider(BuildTarget.WebGL, args.stagingAreaData, developmentBuild, args.report);
            this.m_PlatformProvider.LinkerFlags = ArgumentsForEmscripten(args);
            this.m_PlatformProvider.Libs = GetModules(args);
            this.m_PlatformProvider.JsPre = GetJSPre(args);
            this.m_PlatformProvider.JsLib = GetJSLib(args);
            IL2CPPUtils.RunIl2Cpp(args.stagingAreaData, this.m_PlatformProvider, new Action<string>(this.ModifyIl2CppOutputDirBeforeCompile), this.m_RCR, developmentBuild);
            string[] textArray7 = new string[] { args.stagingAreaData, "Native" };
            string str4 = Paths.Combine(textArray7);
            if (Application.platform != RuntimePlatform.WindowsEditor)
            {
                string[] textArray8 = new string[] { str4, "build.js" };
                Syscall.chmod(Paths.Combine(textArray8), FilePermissions.S_IRGRP | FilePermissions.S_IROTH | FilePermissions.S_IRUSR | FilePermissions.S_IWUSR);
            }
            if (PlayerSettings.WebGL.useEmbeddedResources)
            {
                string[] textArray9 = new string[] { args.stagingAreaData, "Il2CppData", "Resources" };
                string str5 = Paths.Combine(textArray9);
                FileUtil.CreateOrCleanDirectory(str5);
                IL2CPPUtils.CopyEmbeddedResourceFiles(args.stagingAreaData, str5);
            }
            string[] textArray10 = new string[] { args.stagingAreaData, "Il2CppData", "Metadata" };
            string dir = Paths.Combine(textArray10);
            FileUtil.CreateOrCleanDirectory(dir);
            IL2CPPUtils.CopyMetadataFiles(args.stagingAreaData, dir);
            string[] textArray11 = new string[] { args.stagingAreaData, "Managed", "mono", "2.0", "machine.config" };
            string str7 = Paths.Combine(textArray11);
            Directory.CreateDirectory(Path.GetDirectoryName(str7));
            string[] textArray12 = new string[] { EditorApplication.applicationContentsPath, "Mono", "etc", "mono", "2.0", "machine.config" };
            File.Copy(Paths.Combine(textArray12), str7);
            Dictionary<string, string> minificationMap = null;
            if (!developmentBuild)
            {
                this.BuildStep(args, "Optimizing", "Remove duplicate code");
                string[] textArray13 = new string[] { str4, "build.asm.js" };
                minificationMap = CodeAnalysisUtils.ReplaceDuplicates(Paths.Combine(textArray13), 2);
            }
            this.BuildStep(args, "Files", "Packaging files");
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = f => !f.Contains("CAB-");
            }
            IEnumerable<string> filesToShip = Enumerable.Where<string>(Directory.GetFiles(args.stagingAreaData), <>f__am$cache3);
            if (!PackageData("build", args, filesToShip))
            {
                UnityEngine.Debug.LogError("Error packaging files for WebGL build.");
            }
            else
            {
                string[] textArray14 = new string[] { args.stagingAreaData, "build.data" };
                string[] textArray15 = new string[] { args.stagingAreaData, fileName + ".data" };
                File.Move(Paths.Combine(textArray14), Paths.Combine(textArray15));
                string[] textArray16 = new string[] { args.stagingAreaData, "UnityLoader.js" };
                string output = Paths.Combine(textArray16);
                string[] inputs = new string[3];
                inputs[0] = GetDecompressor();
                string[] textArray18 = new string[] { buildToolsDir, "UnityConfig" };
                inputs[1] = Paths.Combine(textArray18);
                string[] textArray19 = new string[] { args.stagingAreaData, "fileloader.js" };
                inputs[2] = Paths.Combine(textArray19);
                MergeJSFiles(inputs, output);
                ProcessTemplateFile(args, output);
                if (!developmentBuild)
                {
                    MinifyJS(output);
                }
                List<string> list = new List<string>();
                foreach (PluginImporter importer in PluginImporter.GetImporters(args.target))
                {
                    if (importer.isNativePlugin && (Path.GetExtension(importer.assetPath) == ".jspre"))
                    {
                        list.Add(Path.GetFullPath(importer.assetPath));
                    }
                }
                if (list.Count != 0)
                {
                    string[] textArray20 = new string[] { args.stagingAreaData, "UnityLoader.js" };
                    string item = Paths.Combine(textArray20);
                    list.Add(item);
                    MergeJSFiles(list, item);
                    list.Clear();
                }
                string[] textArray21 = new string[] { str4, "build.js" };
                string[] textArray22 = new string[] { args.stagingAreaData, fileName + ".js" };
                File.Move(Paths.Combine(textArray21), Paths.Combine(textArray22));
                if (PlayerSettings.WebGL.useWasm)
                {
                    string[] textArray23 = new string[] { str4, "build.wasm" };
                    string[] textArray24 = new string[] { args.stagingAreaData, fileName + ".wasm" };
                    File.Move(Paths.Combine(textArray23), Paths.Combine(textArray24));
                    string[] textArray25 = new string[] { str4, "build.wasm.mappedGlobals" };
                    string[] textArray26 = new string[] { args.stagingAreaData, fileName + ".wasm.mappedGlobals" };
                    File.Move(Paths.Combine(textArray25), Paths.Combine(textArray26));
                }
                if (!flag2)
                {
                    string[] textArray27 = new string[] { str4, "build.asm.js" };
                    string[] textArray28 = new string[] { args.stagingAreaData, fileName + ".asm.js" };
                    File.Move(Paths.Combine(textArray27), Paths.Combine(textArray28));
                    string[] textArray29 = new string[] { str4, "build.js.mem" };
                    string[] textArray30 = new string[] { args.stagingAreaData, fileName + ".mem" };
                    File.Move(Paths.Combine(textArray29), Paths.Combine(textArray30));
                }
                if (!developmentBuild)
                {
                    string[] textArray31 = new string[] { str4, "build.js.symbols" };
                    string[] textArray32 = new string[] { args.stagingAreaData, fileName + ".js.symbols" };
                    File.Move(Paths.Combine(textArray31), Paths.Combine(textArray32));
                    if (PlayerSettings.WebGL.debugSymbols)
                    {
                        this.GenerateDebugSymbolsJs(args, fileName, minificationMap);
                    }
                }
                this.CopyFinalFilesToInstallPath(args);
                if (!developmentBuild)
                {
                    this.BuildStep(args, "Compress", "Compressing build results.");
                    CompressFilesInOutputDirectory(args);
                }
                args.report.RelocateFiles(args.installPath, "/" + fileName);
                WebsockifyEditorWrapper.CreateIfNeeded();
            }
        }

        private static void ProcessTemplateFile(BuildPostProcessArgs args, string templatePath)
        {
            bool flag = (args.options & BuildOptions.Development) != BuildOptions.CompressTextures;
            bool flag2 = flag && EditorUserBuildSettings.webGLUsePreBuiltUnityEngine;
            List<string> list = new List<string> {
                "%UNITY_WIDTH%",
                PlayerSettings.defaultWebScreenWidth.ToString(),
                "%UNITY_HEIGHT%",
                PlayerSettings.defaultWebScreenHeight.ToString(),
                "%UNITY_WEB_NAME%",
                PlayerSettings.productName,
                "%UNITY_DEVELOPMENT_PLAYER%",
                ((args.options & BuildOptions.Development) == BuildOptions.CompressTextures) ? "0" : "1",
                "%UNITY_WEBGL_LOADER_GLUE%"
            };
            string[] components = new string[] { EmscriptenPaths.buildToolsDir, "WebGLLoaderGlue.html" };
            list.Add(File.ReadAllText(Paths.Combine(components)));
            list.Add("%UNITY_WEBGL_BACKGROUND_COLOR%");
            list.Add("#" + ColorUtility.ToHtmlStringRGB(PlayerSettingsSplashScreenEditor.GetSplashScreenActualBackgroundColor()));
            list.Add("%UNITY_WEBGL_SPLASH_STYLE%");
            list.Add((PlayerSettings.SplashScreen.unityLogoStyle != PlayerSettings.SplashScreen.UnityLogoStyle.DarkOnLight) ? "Light" : "Dark");
            list.Add("%UNITY_WEBGL_DATA_FOLDER%");
            list.Add(DataFolderName(args));
            list.Add("%UNITY_WEBGL_FILE_NAME%");
            list.Add(Path.GetFileName(args.installPath));
            list.Add("%UNITY_WEBGL_MAIN_MODULE_FILE_NAME%");
            list.Add(!flag2 ? Path.GetFileName(args.installPath) : "UnityEngine");
            list.Add("%UNITY_WEBGL_TOTAL_MEMORY%");
            list.Add(((uint) ((PlayerSettings.WebGL.memorySize * 0x400) * 0x400)).ToString());
            list.Add("%UNITY_WEBGL_WASM_BINARY_FILE%");
            if (PlayerSettings.WebGL.useWasm)
            {
                list.Add("\n    wasmBinaryFile: \"" + DataFolderName(args) + "/" + Path.GetFileName(args.installPath) + ".wasm\",\n    wasmJSMethod: \"native-wasm,asmjs\",");
            }
            else
            {
                list.Add("");
            }
            list.Add("%UNITY_WEBGL_DEBUG_SYMBOLS_URL%");
            if (!flag && PlayerSettings.WebGL.debugSymbols)
            {
                list.Add("\n    debugSymbolsUrl: \"" + DataFolderName(args) + "/" + Path.GetFileName(args.installPath) + ".debugSymbols.js\",");
            }
            else
            {
                list.Add("");
            }
            list.Add("%UNITY_WEBGL_DINAMIC_LIBRARIES%");
            list.Add(!flag2 ? "" : ("\n    dynamicLibraries: [\"" + DataFolderName(args) + "/" + Path.GetFileName(args.installPath) + ".js\"],"));
            list.Add("%UNITY_WEBGL_BACKGROUND_IMAGE%");
            string str = WriteBackgroundImage(args);
            if (str != null)
            {
                list.Add("backgroundImage: \"" + str + "\",");
            }
            else
            {
                list.Add("");
            }
            list.Add("fetchRemotePackage(REMOTE_PACKAGE_NAME");
            list.Add("fetchRemotePackageWrapper(REMOTE_PACKAGE_NAME");
            foreach (string str2 in PlayerSettings.templateCustomKeys)
            {
                list.Add("%UNITY_CUSTOM_" + str2.ToUpper() + "%");
                list.Add(PlayerSettings.GetTemplateCustomValue(str2));
            }
            FileUtil.ReplaceText(templatePath, list.ToArray());
        }

        private static string WriteBackgroundImage(BuildPostProcessArgs args)
        {
            Rect windowRect = new Rect(0f, 0f, (float) PlayerSettings.defaultWebScreenWidth, (float) PlayerSettings.defaultWebScreenHeight);
            Texture2D splashScreenActualBackgroundImage = PlayerSettingsSplashScreenEditor.GetSplashScreenActualBackgroundImage(windowRect);
            if (splashScreenActualBackgroundImage == null)
            {
                return null;
            }
            RenderTexture dest = new RenderTexture(splashScreenActualBackgroundImage.width, splashScreenActualBackgroundImage.height, 0, RenderTextureFormat.ARGB32);
            Graphics.Blit(splashScreenActualBackgroundImage, dest);
            RenderTexture.active = dest;
            Texture2D textured2 = new Texture2D(splashScreenActualBackgroundImage.width, splashScreenActualBackgroundImage.height);
            Rect source = new Rect(0f, 0f, (float) splashScreenActualBackgroundImage.width, (float) splashScreenActualBackgroundImage.height);
            textured2.ReadPixels(source, 0, 0);
            textured2.Apply();
            File.WriteAllBytes(Path.Combine(args.installPath, "background.jpg"), textured2.EncodeToJPG());
            return "background.jpg";
        }
    }
}

