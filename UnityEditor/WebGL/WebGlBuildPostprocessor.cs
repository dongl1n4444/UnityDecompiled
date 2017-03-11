namespace UnityEditor.WebGL
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEditor.Utils;
    using UnityEditor.WebGL.Emscripten;
    using UnityEditor.WebGL.Il2Cpp;
    using UnityEditorInternal;
    using UnityEngine;

    internal class WebGlBuildPostprocessor : DefaultBuildPostprocessor
    {
        [CompilerGenerated]
        private static Converter<byte, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Comparison<KeyValuePair<string, string>> <>f__am$cache2;
        [CompilerGenerated]
        private static Converter<byte, string> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<string, KeyValuePair<string, string>, string> <>f__am$cache4;
        private const string kAsmCodeFileType = "asm.js Code";
        private const string kAsmCodeNativeExtension = ".asm.js";
        private const string kAsmCodeSuffix = ".asm.code";
        private const string kAsmDynamicLibraryFileType = "asm.js Dynamic Library";
        private const string kAsmDynamicLibrarySuffix = ".asm.library";
        private const string kAsmFrameworkFileType = "asm.js Framework";
        private const string kAsmFrameworkNativeExtension = ".js";
        private const string kAsmFrameworkSuffix = ".asm.framework";
        private const string kAsmLinkResult = "linkresult_asm";
        private const string kAsmMemoryFileType = "asm.js Memory Initializer";
        private const string kAsmMemoryNativeExtension = ".js.mem";
        private const string kAsmMemorySuffix = ".asm.memory";
        private const string kAsmSymbolsFileType = "asm.js Debug Symbols";
        private const string kAsmSymbolsNativeExtension = ".js.symbols";
        private const string kAsmSymbolsStrippedExtension = ".js.symbols.stripped";
        private const string kAsmSymbolsSuffix = ".asm.symbols";
        private const string kBackgroundExtension = ".jpg";
        private const string kBackgroundFileType = "Build Background";
        private const string kBitcodeExtension = ".bc";
        private const string kBuildExtension = ".unityweb";
        private const string kBuildFolder = "Build";
        private const string kCompressedExtension = ".compressed";
        private const string kCompressionMarkerBrotli = "UnityWeb Compressed Content (brotli)";
        private const string kCompressionMarkerGzip = "UnityWeb Compressed Content (gzip)";
        private const string kDataFileType = "Application Data";
        private const string kDataSuffix = ".data";
        private const string kIl2CppDataFolder = "Il2CppData";
        private const string kJslibPluginExtension = ".jslib";
        private const string kJsonExtension = ".json";
        private const string kJsonFileType = "Build Index";
        private const string kJsPluginExtension = ".js";
        private const string kJsprePluginExtension = ".jspre";
        private const string kManagedFolder = "Managed";
        private const string kNativeFilename = "build";
        private const string kNativeFolder = "Native";
        private const string kOutputFolder = "Output";
        private const string kResourcesFolder = "Resources";
        private const string kTemplateFileType = "WebGL Template";
        private const string kUnityLoaderFileType = "Build Loader";
        private const string kUnityLoaderPath = "UnityLoader.js";
        private const string kWasmCodeFileType = "WebAssembly Code";
        private const string kWasmCodeNativeExtension = ".wasm";
        private const string kWasmCodeSuffix = ".wasm.code";
        private const string kWasmFrameworkFileType = "WebAssembly Framework";
        private const string kWasmFrameworkNativeExtension = ".js";
        private const string kWasmFrameworkSuffix = ".wasm.framework";
        private const string kWasmLinkResult = "linkresult_wasm";
        private const string kWasmSymbolsFileType = "WebAssembly Debug Symbols";
        private const string kWasmSymbolsNativeExtension = ".js.symbols";
        private const string kWasmSymbolsSuffix = ".wasm.symbols";
        private const string kWebGLCache = "webgl_cache";
        private Dictionary<string, string> m_BuildFiles;
        private string m_BuildName;
        private bool m_DevelopmentPlayer;
        private HashAlgorithm m_Hash = MD5.Create();
        private bool m_NameFilesAsHashes;
        private string[] m_NativePluginsExtensions = new string[] { ".cpp", ".cc", ".c", ".bc", ".a" };
        private WebGlIl2CppPlatformProvider m_PlatformProvider;
        private string m_PreBuiltUnityEngine;
        private readonly ProgressHelper m_Progress = new ProgressHelper();
        private RuntimeClassRegistry m_RCR;
        private string m_StagingAreaData;
        private string m_StagingAreaDataIl2CppData;
        private string m_StagingAreaDataManaged;
        private string m_StagingAreaDataNative;
        private string m_StagingAreaDataOutput;
        private string m_StagingAreaDataOutputBuild;
        private string m_StagingAreaDataResources;
        private string m_TemplatePath;
        private string m_TotalMemory;
        private bool m_UseAsm;
        private bool m_UseWasm;
        private string[] m_ValidTemplateIndexFiles = new string[] { 
            "default.asp", "default.html", "default.htm", "default.aspx", "default.php", "default.shtml", "default.shtm", "index.html", "index.htm", "index.asp", "index.php", "index.php5", "index.shtml", "index.shtm", "home.html", "home.htm",
            "home.shtml", "home.shtm", "welcome.html", "welcome.htm", "welcome.asp"
        };
        private string m_WebGLCache;
        private bool m_WebGLUsePreBuiltUnityEngine;

        private string ArgumentsForEmscripten(bool wasmBuild)
        {
            string str = " -O3";
            str = (((str + $" -g{(!this.m_DevelopmentPlayer ? 0 : 2)}") + " -DUNITY_WEBGL=1" + " -s PRECISE_F32=2") + " -s NO_EXIT_RUNTIME=1" + " -s USE_WEBGL2=1") + $" -s DISABLE_EXCEPTION_CATCHING={((PlayerSettings.WebGL.exceptionSupport != WebGLExceptionSupport.None) ? 0 : 1)}" + $" -s TOTAL_MEMORY={this.m_TotalMemory}";
            if (this.m_DevelopmentPlayer)
            {
                str = str + " -s ASSERTIONS=1" + " -s DEMANGLE_SUPPORT=1";
            }
            if (this.m_WebGLUsePreBuiltUnityEngine)
            {
                str = str + " -s SIDE_MODULE=1" + " -s ASM_JS=2";
            }
            if (wasmBuild)
            {
                str = str + " -s BINARYEN=1";
            }
            str = str + $" --memory-init-file {(!this.m_WebGLUsePreBuiltUnityEngine ? 1 : 0)}" + " --emit-symbol-map";
            if (!this.m_WebGLUsePreBuiltUnityEngine)
            {
                str = str + " --separate-asm";
            }
            return ((str + " --output_eol linux") + " " + PlayerSettings.WebGL.emscriptenArgs);
        }

        private void AssembleAsmCode(BuildPostProcessArgs args, string outputPath)
        {
            string path = (!this.m_WebGLUsePreBuiltUnityEngine ? Paths.Combine(new string[] { this.m_StagingAreaData, "linkresult_asm", "build" }) : this.m_PreBuiltUnityEngine) + ".asm.js";
            byte[] source = File.ReadAllBytes(path);
            using (BinaryWriter writer = new BinaryWriter(File.Open(outputPath, System.IO.FileMode.Create)))
            {
                byte[] bytes = Encoding.UTF8.GetBytes("Module[\"asm\"] =");
                int length = bytes.Length;
                if (!bytes.SequenceEqual<byte>(source.Take<byte>(length)))
                {
                    throw new Exception("Invalid module format: " + path);
                }
                while ((length < source.Length) && (source[length] == 0x20))
                {
                    length++;
                }
                writer.Write(source, length, source.Length - length);
            }
            if (!this.m_WebGLUsePreBuiltUnityEngine)
            {
                File.Copy(outputPath, path, true);
            }
        }

        private bool AssembleBackground(BuildPostProcessArgs args, string outputPath)
        {
            Rect windowRect = new Rect(0f, 0f, (float) PlayerSettings.defaultWebScreenWidth, (float) PlayerSettings.defaultWebScreenHeight);
            Texture2D splashScreenActualBackgroundImage = PlayerSettingsSplashScreenEditor.GetSplashScreenActualBackgroundImage(windowRect);
            if (splashScreenActualBackgroundImage == null)
            {
                return false;
            }
            RenderTexture dest = new RenderTexture(splashScreenActualBackgroundImage.width, splashScreenActualBackgroundImage.height, 0, RenderTextureFormat.ARGB32);
            Graphics.Blit(splashScreenActualBackgroundImage, dest);
            RenderTexture.active = dest;
            Texture2D textured2 = new Texture2D(splashScreenActualBackgroundImage.width, splashScreenActualBackgroundImage.height);
            Rect source = new Rect(0f, 0f, (float) splashScreenActualBackgroundImage.width, (float) splashScreenActualBackgroundImage.height);
            textured2.ReadPixels(source, 0, 0);
            textured2.Apply();
            File.WriteAllBytes(outputPath, textured2.EncodeToJPG());
            return true;
        }

        private void AssembleData(BuildPostProcessArgs args, string outputPath)
        {
            List<DataFile> list = new List<DataFile>();
            List<string> list3 = new List<string>();
            string[] components = new string[] { this.m_StagingAreaDataResources, "unity_default_resources" };
            list3.Add(Paths.Combine(components));
            string[] textArray2 = new string[] { this.m_StagingAreaDataManaged, "mono", "2.0", "machine.config" };
            list3.Add(Paths.Combine(textArray2));
            List<string> list2 = list3;
            list2.AddRange(Directory.GetFiles(this.m_StagingAreaDataIl2CppData, "*.*", SearchOption.AllDirectories));
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = f => !f.Contains("CAB-");
            }
            list2.AddRange(Enumerable.Where<string>(Directory.GetFiles(this.m_StagingAreaData), <>f__am$cache1));
            foreach (string str in list2)
            {
                list.Add(new DataFile(str, str.Substring(this.m_StagingAreaData.Length + 1)));
            }
            string[] textArray3 = new string[] { this.m_StagingAreaDataResources, "unity_builtin_extra" };
            string path = Paths.Combine(textArray3);
            if (File.Exists(path))
            {
                list.Add(new DataFile(path, path.Substring(this.m_StagingAreaData.Length + 1)));
            }
            byte[] bytes = Encoding.UTF8.GetBytes("UnityWebData1.0\0");
            long num = bytes.Length + 4;
            foreach (DataFile file in list)
            {
                num += 12 + file.internalPath.Length;
            }
            using (BinaryWriter writer = new BinaryWriter(File.Open(outputPath, System.IO.FileMode.Create)))
            {
                writer.Write(bytes);
                writer.Write((uint) num);
                foreach (DataFile file2 in list)
                {
                    writer.Write((uint) num);
                    writer.Write((uint) file2.length);
                    writer.Write((uint) file2.internalPath.Length);
                    writer.Write(file2.internalPath);
                    num += file2.length;
                }
                foreach (DataFile file3 in list)
                {
                    writer.Write(File.ReadAllBytes(file3.path));
                }
            }
        }

        private void AssembleDebugSymbols(BuildPostProcessArgs args, bool useWasm, string outputPath)
        {
            string mapPath = !useWasm ? Paths.Combine(new string[] { this.m_StagingAreaData, "linkresult_asm", "build.js.symbols.stripped" }) : Paths.Combine(new string[] { this.m_StagingAreaData, "linkresult_wasm", "build.js.symbols" });
            List<KeyValuePair<string, string>> list = CodeAnalysisUtils.ReadMinificationMap(mapPath).ToList<KeyValuePair<string, string>>();
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = (a, b) => string.Compare(a.Value, b.Value);
            }
            list.Sort(<>f__am$cache2);
            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                writer.Write("(function() {\n");
                writer.Write("var debugSymbols = {\n");
                foreach (KeyValuePair<string, string> pair in list)
                {
                    writer.Write(pair.Key + ":'" + pair.Value + "',\n");
                }
                writer.Write("};\n");
                string[] components = new string[] { EmscriptenPaths.buildToolsDir, "Demangle.js" };
                writer.Write(File.ReadAllText(Paths.Combine(components)));
                writer.Write("\nreturn (function (symbol) {\n");
                writer.Write("  if (debugSymbols[symbol])\n");
                writer.Write("    symbol = debugSymbols[symbol];\n");
                writer.Write("  return !symbol.lastIndexOf('__Z', 0) ? demangle(symbol) : symbol;\n");
                writer.Write("});\n");
                writer.Write("\n});\n");
            }
        }

        private void AssembleFramework(BuildPostProcessArgs args, bool useWasm, string outputPath)
        {
            string path = !useWasm ? ((!this.m_WebGLUsePreBuiltUnityEngine ? Paths.Combine(new string[] { this.m_StagingAreaData, "linkresult_asm", "build" }) : this.m_PreBuiltUnityEngine) + ".js") : (Paths.Combine(new string[] { this.m_StagingAreaData, "linkresult_wasm", "build" }) + ".js");
            char[] trimChars = new char[] { '\t', '\n', '\v', '\f', '\r', ' ', ';', '\0' };
            File.WriteAllText(outputPath, "(function(Module) {\n");
            foreach (PluginImporter importer in PluginImporter.GetImporters(args.target))
            {
                if (importer.isNativePlugin && (Path.GetExtension(importer.assetPath) == ".jspre"))
                {
                    File.AppendAllText(outputPath, File.ReadAllText(importer.assetPath).Trim(trimChars));
                    File.AppendAllText(outputPath, ";\n");
                }
            }
            if (this.m_WebGLUsePreBuiltUnityEngine)
            {
                bool flag = false;
                foreach (PluginImporter importer2 in PluginImporter.GetImporters(args.target))
                {
                    if (importer2.isNativePlugin && (Path.GetExtension(importer2.assetPath) == ".jslib"))
                    {
                        if (!flag)
                        {
                            string[] components = new string[] { EmscriptenPaths.buildToolsDir, "DynamicJslibLoader.js" };
                            File.AppendAllText(outputPath, File.ReadAllText(Paths.Combine(components)).Trim(trimChars));
                            File.AppendAllText(outputPath, ";\n");
                            flag = true;
                        }
                        File.AppendAllText(outputPath, File.ReadAllText(importer2.assetPath).Trim(trimChars));
                        File.AppendAllText(outputPath, ";\n");
                    }
                }
            }
            File.AppendAllText(outputPath, File.ReadAllText(path));
            File.AppendAllText(outputPath, "\n});");
        }

        private void AssembleOutput(BuildPostProcessArgs args)
        {
            FileUtil.CreateOrCleanDirectory(this.m_StagingAreaDataOutput);
            FileUtil.CopyDirectoryRecursiveForPostprocess(this.m_TemplatePath, this.m_StagingAreaDataOutput, true);
            FileUtil.UnityDirectoryRemoveReadonlyAttribute(this.m_StagingAreaDataOutput);
            string[] components = new string[] { this.m_StagingAreaDataOutput, "thumbnail.png" };
            File.Delete(Paths.Combine(components));
            Directory.CreateDirectory(this.m_StagingAreaDataOutputBuild);
            string[] textArray2 = new string[] { this.m_StagingAreaDataOutputBuild, this.m_BuildName + ".json" };
            string path = Paths.Combine(textArray2);
            string contents = "{\n\"TOTAL_MEMORY\": " + this.m_TotalMemory;
            string[] textArray3 = new string[] { this.m_StagingAreaDataOutputBuild, this.m_BuildName + ".data.unityweb" };
            string outputPath = Paths.Combine(textArray3);
            this.AssembleData(args, outputPath);
            string str4 = this.PostProcessBuildFile(outputPath, "Application Data");
            contents = contents + ",\n\"dataUrl\": \"" + str4 + "\"";
            if (this.m_UseWasm)
            {
                string[] textArray4 = new string[] { this.m_StagingAreaDataOutputBuild, this.m_BuildName + ".wasm.code.unityweb" };
                string str5 = Paths.Combine(textArray4);
                string[] textArray5 = new string[] { this.m_StagingAreaData, "linkresult_wasm", "build.wasm" };
                File.Copy(Paths.Combine(textArray5), str5);
                string str6 = this.PostProcessBuildFile(str5, "WebAssembly Code");
                contents = contents + ",\n\"wasmCodeUrl\": \"" + str6 + "\"";
                string[] textArray6 = new string[] { this.m_StagingAreaDataOutputBuild, this.m_BuildName + ".wasm.framework.unityweb" };
                string str7 = Paths.Combine(textArray6);
                this.AssembleFramework(args, true, str7);
                string str8 = this.PostProcessBuildFile(str7, "WebAssembly Framework");
                contents = contents + ",\n\"wasmFrameworkUrl\": \"" + str8 + "\"";
                if (!this.m_DevelopmentPlayer && PlayerSettings.WebGL.debugSymbols)
                {
                    string[] textArray7 = new string[] { this.m_StagingAreaDataOutputBuild, this.m_BuildName + ".wasm.symbols.unityweb" };
                    string str9 = Paths.Combine(textArray7);
                    this.AssembleDebugSymbols(args, true, str9);
                    string str10 = this.PostProcessBuildFile(str9, "WebAssembly Debug Symbols");
                    contents = contents + ",\n\"wasmSymbolsUrl\": \"" + str10 + "\"";
                }
            }
            if (this.m_UseAsm)
            {
                string[] textArray8 = new string[] { this.m_StagingAreaDataOutputBuild, this.m_BuildName + ".asm.code.unityweb" };
                string str11 = Paths.Combine(textArray8);
                this.AssembleAsmCode(args, str11);
                string str12 = this.PostProcessBuildFile(str11, "asm.js Code");
                contents = contents + ",\n\"asmCodeUrl\": \"" + str12 + "\"";
                string[] textArray9 = new string[] { this.m_StagingAreaDataOutputBuild, this.m_BuildName + ".asm.memory.unityweb" };
                string str13 = Paths.Combine(textArray9);
                File.Copy((!this.m_WebGLUsePreBuiltUnityEngine ? Paths.Combine(new string[] { this.m_StagingAreaData, "linkresult_asm", "build" }) : this.m_PreBuiltUnityEngine) + ".js.mem", str13);
                string str15 = this.PostProcessBuildFile(str13, "asm.js Memory Initializer");
                contents = contents + ",\n\"asmMemoryUrl\": \"" + str15 + "\"";
                string[] textArray11 = new string[] { this.m_StagingAreaDataOutputBuild, this.m_BuildName + ".asm.framework.unityweb" };
                string str16 = Paths.Combine(textArray11);
                this.AssembleFramework(args, false, str16);
                string str17 = this.PostProcessBuildFile(str16, "asm.js Framework");
                contents = contents + ",\n\"asmFrameworkUrl\": \"" + str17 + "\"";
                if (!this.m_DevelopmentPlayer && PlayerSettings.WebGL.debugSymbols)
                {
                    string[] textArray12 = new string[] { this.m_StagingAreaDataOutputBuild, this.m_BuildName + ".asm.symbols.unityweb" };
                    string str18 = Paths.Combine(textArray12);
                    this.AssembleDebugSymbols(args, false, str18);
                    string str19 = this.PostProcessBuildFile(str18, "asm.js Debug Symbols");
                    contents = contents + ",\n\"asmSymbolsUrl\": \"" + str19 + "\"";
                }
                if (this.m_WebGLUsePreBuiltUnityEngine)
                {
                    string[] textArray13 = new string[] { this.m_StagingAreaDataOutputBuild, this.m_BuildName + ".asm.library.unityweb" };
                    string str20 = Paths.Combine(textArray13);
                    string[] textArray14 = new string[] { this.m_StagingAreaData, "linkresult_asm", "build.js" };
                    File.Copy(Paths.Combine(textArray14), str20);
                    string str21 = this.PostProcessBuildFile(str20, "asm.js Dynamic Library");
                    contents = contents + ",\n\"asmLibraryUrl\": \"" + str21 + "\"";
                }
            }
            string[] textArray15 = new string[] { this.m_StagingAreaDataOutputBuild, this.m_BuildName + ".jpg" };
            string str22 = Paths.Combine(textArray15);
            if (this.AssembleBackground(args, str22))
            {
                string str23 = this.PostProcessBuildFile(str22, "Build Background");
                contents = contents + ",\n\"backgroundUrl\": \"" + str23 + "\"";
            }
            contents = ((contents + ",\n\"splashScreenStyle\": \"" + ((PlayerSettings.SplashScreen.unityLogoStyle != PlayerSettings.SplashScreen.UnityLogoStyle.DarkOnLight) ? "Dark" : "Light") + "\"") + ",\n\"backgroundColor\": \"#" + ColorUtility.ToHtmlStringRGB(PlayerSettingsSplashScreenEditor.GetSplashScreenActualBackgroundColor()) + "\"") + "\n}";
            File.WriteAllText(path, contents);
            string str24 = "Build/" + this.PostProcessBuildFile(path, "Build Index");
            string[] textArray16 = new string[] { this.m_StagingAreaDataOutputBuild, "UnityLoader.js" };
            string destFileName = Paths.Combine(textArray16);
            string[] textArray17 = new string[] { EmscriptenPaths.buildToolsDir, !this.m_DevelopmentPlayer ? "UnityLoader.min.js" : "UnityLoader.js" };
            File.Copy(Paths.Combine(textArray17), destFileName);
            string str26 = "Build/" + this.PostProcessBuildFile(destFileName, "Build Loader");
            Dictionary<string, string> dictionary = new Dictionary<string, string> {
                { 
                    "%UNITY_WIDTH%",
                    PlayerSettings.defaultWebScreenWidth.ToString()
                },
                { 
                    "%UNITY_HEIGHT%",
                    PlayerSettings.defaultWebScreenHeight.ToString()
                },
                { 
                    "%UNITY_WEB_NAME%",
                    PlayerSettings.productName
                },
                { 
                    "%UNITY_WEBGL_LOADER_URL%",
                    str26
                },
                { 
                    "%UNITY_WEBGL_BUILD_URL%",
                    str24
                }
            };
            foreach (string str27 in this.m_ValidTemplateIndexFiles)
            {
                string[] textArray18 = new string[] { this.m_StagingAreaDataOutput, str27 };
                string str28 = Paths.Combine(textArray18);
                if (File.Exists(str28))
                {
                    if (<>f__am$cache4 == null)
                    {
                        <>f__am$cache4 = (current, replace) => current.Replace(replace.Key, replace.Value);
                    }
                    File.WriteAllText(str28, Enumerable.Aggregate<KeyValuePair<string, string>, string>(dictionary, File.ReadAllText(str28), <>f__am$cache4));
                }
            }
        }

        private void BuildStep(BuildPostProcessArgs args, string title, string description)
        {
            this.m_Progress.Show(title, description);
            args.report.BeginBuildStep(description);
        }

        private void CheckBuildPrerequisites()
        {
            if (this.m_WebGLUsePreBuiltUnityEngine)
            {
                string str = "";
                string[] textArray1 = new string[] { ".js", ".js.mem", ".asm.js" };
                foreach (string str2 in textArray1)
                {
                    if (!File.Exists(this.m_PreBuiltUnityEngine + str2))
                    {
                        str = str + "\n" + this.m_PreBuiltUnityEngine + str2;
                    }
                }
                if (str != "")
                {
                    throw new Exception("'Fast Rebuild' option requires prebuilt JavaScript version of Unity engine. The following files are missing: " + str);
                }
            }
            if (!Directory.Exists(this.m_TemplatePath))
            {
                throw new Exception("Invalid WebGL template path: " + this.m_TemplatePath + "! Select a template in player settings.");
            }
            bool flag = false;
            for (int i = 0; !flag && (i < this.m_ValidTemplateIndexFiles.Length); i++)
            {
                string[] components = new string[] { this.m_TemplatePath, this.m_ValidTemplateIndexFiles[i] };
                flag = File.Exists(Paths.Combine(components));
            }
            if (!flag)
            {
                throw new Exception("Invalid WebGL template selection: " + this.m_TemplatePath + "! Select a template in player settings.");
            }
        }

        private void CompileBuild(BuildPostProcessArgs args)
        {
            WebGLStrippingInfo info = ScriptableObject.CreateInstance<WebGLStrippingInfo>();
            info.developmentBuild = this.m_DevelopmentPlayer;
            info.builtCodePath = !this.m_UseWasm ? Paths.Combine(new string[] { this.m_StagingAreaData, "linkresult_asm", "build.asm.js" }) : Paths.Combine(new string[] { this.m_StagingAreaData, "linkresult_wasm", "build.wasm" });
            args.report.AddAppendix(info);
            this.m_RCR = args.usedClassRegistry;
            this.m_PlatformProvider = new WebGlIl2CppPlatformProvider(BuildTarget.WebGL, this.m_DevelopmentPlayer, args.stagingAreaData, "build.bc", args.report);
            this.m_PlatformProvider.Libs = this.GetModules();
            this.m_PlatformProvider.JsPre = new List<string>();
            this.m_PlatformProvider.JsLib = new List<string>();
            IL2CPPUtils.RunIl2Cpp(args.stagingAreaData, this.m_PlatformProvider, new Action<string>(this.ModifyIl2CppOutputDirBeforeCompile), this.m_RCR, this.m_DevelopmentPlayer);
            if (PlayerSettings.WebGL.useEmbeddedResources)
            {
                string[] textArray3 = new string[] { this.m_StagingAreaDataIl2CppData, "Resources" };
                string str = Paths.Combine(textArray3);
                FileUtil.CreateOrCleanDirectory(str);
                IL2CPPUtils.CopyEmbeddedResourceFiles(args.stagingAreaData, str);
            }
            string[] components = new string[] { this.m_StagingAreaDataIl2CppData, "Metadata" };
            string dir = Paths.Combine(components);
            FileUtil.CreateOrCleanDirectory(dir);
            IL2CPPUtils.CopyMetadataFiles(args.stagingAreaData, dir);
            string[] textArray5 = new string[] { this.m_StagingAreaDataManaged, "mono", "2.0", "machine.config" };
            string path = Paths.Combine(textArray5);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            string[] textArray6 = new string[] { EditorApplication.applicationContentsPath, "Mono", "etc", "mono", "2.0", "machine.config" };
            File.Copy(Paths.Combine(textArray6), path);
            string[] textArray7 = new string[] { EmscriptenPaths.buildToolsDir, "data", "unity_default_resources" };
            string[] textArray8 = new string[] { this.m_StagingAreaDataResources, "unity_default_resources" };
            File.Copy(Paths.Combine(textArray7), Paths.Combine(textArray8));
        }

        public static void CompressAndMarkBrotli(string path)
        {
            string sourceFileName = path + ".compressed";
            string[] components = new string[] { EmscriptenPaths.buildToolsDir, "Brotli", "python", "bro.py" };
            string str2 = Paths.Combine(components);
            string str3 = (Application.platform != RuntimePlatform.WindowsEditor) ? "Brotli-0.4.0-py2.7-macosx-10.11-intel.egg" : "lib.win-amd64-2.7";
            ProcessStartInfo p = new ProcessStartInfo(EmscriptenPaths.pythonExecutable) {
                Arguments = $""{str2}" -o "{sourceFileName}" -i "{path}" --comment "{"UnityWeb Compressed Content (brotli)"}"",
                WorkingDirectory = Directory.GetCurrentDirectory(),
                UseShellExecute = false,
                CreateNoWindow = true
            };
            string[] textArray2 = new string[] { EmscriptenPaths.buildToolsDir, "Brotli", "dist", str3 };
            p.EnvironmentVariables["PYTHONPATH"] = Paths.Combine(textArray2);
            ProgramUtils.StartProgramChecked(p);
            File.Delete(path);
            File.Move(sourceFileName, path);
        }

        public static void CompressAndMarkGzip(string path)
        {
            string str = path + ".compressed";
            string fileName = (Application.platform != RuntimePlatform.LinuxEditor) ? Paths.Combine(new string[] { EditorApplication.applicationContentsPath, "Tools", (Application.platform != RuntimePlatform.WindowsEditor) ? "7za" : "7z.exe" }) : "gzip";
            string str3 = (Application.platform != RuntimePlatform.LinuxEditor) ? $"a -tgzip "{str}" "{path}"" : $"-9 --keep -S "{".compressed"}" "{path}"";
            ProcessStartInfo p = new ProcessStartInfo(fileName) {
                Arguments = str3,
                WorkingDirectory = Directory.GetCurrentDirectory(),
                UseShellExecute = false,
                CreateNoWindow = true
            };
            ProgramUtils.StartProgramChecked(p);
            if (!SetGzipComment(str, "UnityWeb Compressed Content (gzip)"))
            {
                throw new Exception("Invalid gzip archive format: " + str);
            }
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.Move(str, path);
        }

        private void CompressBuild(BuildPostProcessArgs args)
        {
            foreach (string str in Directory.GetFiles(this.m_StagingAreaDataOutputBuild, "*.unityweb"))
            {
                if (this.m_BuildFiles.ContainsKey(str))
                {
                    switch (PlayerSettings.WebGL.compressionFormat)
                    {
                        case WebGLCompressionFormat.Brotli:
                            CompressAndMarkBrotli(str);
                            break;

                        case WebGLCompressionFormat.Gzip:
                            goto Label_0052;
                    }
                }
                continue;
            Label_0052:
                CompressAndMarkGzip(str);
            }
        }

        private string ComputeHashString(byte[] data)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = b => b.ToString("X2");
            }
            return string.Join(string.Empty, Array.ConvertAll<byte, string>(this.m_Hash.ComputeHash(data), <>f__am$cache0));
        }

        private string ComputeHashString(bool b) => 
            this.ComputeHashString(b.ToString());

        private string ComputeHashString(string s) => 
            this.ComputeHashString(Encoding.ASCII.GetBytes(s));

        private void EmscriptenLink(BuildPostProcessArgs args, bool wasmBuild, string sourceFiles, string sourceFilesHash)
        {
            bool b = !wasmBuild && !this.m_DevelopmentPlayer;
            string str = !wasmBuild ? "linkresult_asm" : "linkresult_wasm";
            string[] components = new string[] { this.m_StagingAreaData, str };
            string dir = Paths.Combine(components);
            string str3 = "build" + (!wasmBuild ? ".js" : ".js");
            FileUtil.CreateOrCleanDirectory(dir);
            string[] textArray2 = new string[] { dir, str3 };
            string s = this.ArgumentsForEmscripten(wasmBuild) + $" -o "{Paths.Combine(textArray2)}"" + sourceFiles;
            string str5 = this.ComputeHashString(InternalEditorUtility.GetFullUnityVersion()) + this.ComputeHashString(s) + sourceFilesHash + this.ComputeHashString(b);
            string[] textArray3 = new string[] { this.m_WebGLCache, str + "_" + this.ComputeHashString(str5) };
            string path = Paths.Combine(textArray3);
            if (Directory.Exists(path))
            {
                FileUtil.CopyDirectoryRecursive(path, dir);
            }
            else
            {
                foreach (string str7 in Directory.GetDirectories(this.m_WebGLCache, str + "_*"))
                {
                    Directory.Delete(str7, true);
                }
                string[] textArray4 = new string[] { EmscriptenPaths.dataPath, "..", "Temp", "emcc_arguments.resp" };
                string str8 = Paths.Combine(textArray4);
                File.WriteAllText(str8, s);
                ProcessStartInfo startInfo = new ProcessStartInfo(EmscriptenPaths.pythonExecutable) {
                    Arguments = $""{EmscriptenPaths.emcc}" @"{str8}"",
                    WorkingDirectory = EmscriptenPaths.GetShortPathName(Path.GetFullPath(this.m_StagingAreaData)),
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                EmccArguments.SetupDefaultEmscriptenEnvironment(startInfo);
                if (!ProgramUtils.StartProgramChecked(startInfo))
                {
                    throw new Exception("Error running Emscripten:\n" + startInfo.Arguments + "\n" + s);
                }
                if (b)
                {
                    this.BuildStep(args, "Scripting", "Remove duplicate asm.js code");
                    string[] textArray5 = new string[] { dir, "build" };
                    string str9 = Paths.Combine(textArray5);
                    CodeAnalysisUtils.ReplaceDuplicates(str9 + ".asm.js", str9 + ".js.symbols", str9 + ".js.symbols.stripped", 2);
                }
                FileUtil.CopyDirectoryRecursive(dir, path);
            }
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

        private string[] GetJslibPlugins(BuildPostProcessArgs args)
        {
            List<string> list = new List<string>();
            if (!this.m_WebGLUsePreBuiltUnityEngine)
            {
                string[] components = new string[] { EmscriptenPaths.buildToolsDir, "lib" };
                string path = Paths.Combine(components);
                foreach (string str2 in Directory.GetFiles(path, "*.js"))
                {
                    string[] textArray2 = new string[] { path, Path.GetFileName(str2) };
                    list.Add(Paths.Combine(textArray2));
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

        private string[] GetJsprePlugins()
        {
            List<string> list = new List<string>();
            if (!this.m_WebGLUsePreBuiltUnityEngine)
            {
                string[] components = new string[] { EmscriptenPaths.buildToolsDir, "prejs" };
                string path = Paths.Combine(components);
                foreach (string str2 in Directory.GetFiles(path, "*.js"))
                {
                    string[] textArray2 = new string[] { path, Path.GetFileName(str2) };
                    list.Add(Paths.Combine(textArray2));
                }
                if (PlayerSettings.WebGL.exceptionSupport == WebGLExceptionSupport.None)
                {
                    string[] textArray3 = new string[] { EmscriptenPaths.buildToolsDir, "ExceptionLogger.js" };
                    list.Add(Paths.Combine(textArray3));
                }
            }
            return list.ToArray();
        }

        private string[] GetModules()
        {
            List<string> list = new List<string>();
            if (!this.m_WebGLUsePreBuiltUnityEngine)
            {
                string modulesDirectory = PlayerSettings.WebGL.modulesDirectory;
                if (string.IsNullOrEmpty(modulesDirectory))
                {
                    string[] components = new string[] { EmscriptenPaths.buildToolsDir, "lib", "modules" };
                    modulesDirectory = Paths.Combine(components);
                    if (this.m_DevelopmentPlayer)
                    {
                        modulesDirectory = modulesDirectory + "_development";
                    }
                }
                else if (!Path.IsPathRooted(modulesDirectory))
                {
                    string[] textArray2 = new string[] { EmscriptenPaths.buildToolsDir, modulesDirectory };
                    modulesDirectory = Paths.Combine(textArray2);
                }
                list.AddRange(Directory.GetFiles(modulesDirectory, "*.bc"));
            }
            return list.ToArray();
        }

        private string[] GetNativePlugins(BuildPostProcessArgs args)
        {
            List<string> list = new List<string>();
            foreach (PluginImporter importer in PluginImporter.GetImporters(args.target))
            {
                if (importer.isNativePlugin && this.m_NativePluginsExtensions.Contains<string>(Path.GetExtension(importer.assetPath)))
                {
                    list.Add(Path.GetFullPath(importer.assetPath));
                }
            }
            return list.ToArray();
        }

        public override void LaunchPlayer(BuildLaunchPlayerArgs args)
        {
            int num;
            HttpServerEditorWrapper.CreateIfNeeded(args.installPath, out num);
            Application.OpenURL("http://localhost:" + num + "/");
        }

        private void LinkBuild(BuildPostProcessArgs args)
        {
            string sourceFiles = "";
            string sourceFilesHash = "";
            foreach (string str3 in this.GetJsprePlugins())
            {
                sourceFiles = sourceFiles + $" --pre-js "{str3}"";
                sourceFilesHash = sourceFilesHash + this.ComputeHashString(File.ReadAllBytes(str3));
            }
            foreach (string str4 in this.GetJslibPlugins(args))
            {
                sourceFiles = sourceFiles + $" --js-library "{str4}"";
                sourceFilesHash = sourceFilesHash + this.ComputeHashString(File.ReadAllBytes(str4));
            }
            foreach (string str5 in this.GetNativePlugins(args))
            {
                sourceFiles = sourceFiles + $" "{str5}"";
                sourceFilesHash = sourceFilesHash + this.ComputeHashString(File.ReadAllBytes(str5));
            }
            string[] components = new string[] { this.m_StagingAreaDataNative, "build.bc" };
            string fullPath = Path.GetFullPath(Paths.Combine(components));
            sourceFiles = sourceFiles + $" "{fullPath}"";
            sourceFilesHash = sourceFilesHash + this.ComputeHashString(File.ReadAllBytes(fullPath));
            if (this.m_UseAsm)
            {
                this.BuildStep(args, "Scripting", "Compile asm.js module");
                this.EmscriptenLink(args, false, sourceFiles, sourceFilesHash);
            }
            if (this.m_UseWasm)
            {
                this.BuildStep(args, "Scripting", "Compile WebAssembly module");
                this.EmscriptenLink(args, true, sourceFiles, sourceFilesHash);
            }
        }

        private static void MinifyJS(string path)
        {
            ProcessStartInfo info2 = new ProcessStartInfo(EmscriptenPaths.nodeExecutable);
            string[] components = new string[] { EmscriptenPaths.buildToolsDir, "uglify-js", "bin", "uglifyjs" };
            info2.Arguments = $""{Paths.Combine(components)}" "{path}" -c -m -o "{path}"";
            info2.UseShellExecute = false;
            info2.CreateNoWindow = true;
            ProcessStartInfo p = info2;
            ProgramUtils.StartProgramChecked(p);
        }

        private void ModifyIl2CppOutputDirBeforeCompile(string outputDir)
        {
            string[] components = new string[] { this.m_StagingAreaDataManaged, "UnityICallRegistration.cpp" };
            string[] textArray2 = new string[] { outputDir, "UnityICallRegistration.cpp" };
            File.Copy(Paths.Combine(components), Paths.Combine(textArray2));
            this.m_RCR.SynchronizeClasses();
            UnityType[] classesToSkip = new UnityType[] { FindTypeByNameChecked("MasterServerInterface"), FindTypeByNameChecked("NetworkManager"), FindTypeByNameChecked("NetworkView"), FindTypeByNameChecked("ClusterInputManager"), FindTypeByNameChecked("WorldAnchor"), FindTypeByNameChecked("MovieTexture"), FindTypeByNameChecked("NScreenBridge") };
            string[] textArray3 = new string[] { this.m_StagingAreaDataManaged, "ICallSummary.txt" };
            CodeStrippingUtils.WriteModuleAndClassRegistrationFile(Path.GetFullPath(this.m_StagingAreaDataManaged), Path.GetFullPath(Paths.Combine(textArray3)), outputDir, this.m_RCR, classesToSkip, this.m_PlatformProvider);
            EmscriptenCompiler.CleanupAndCreateEmscriptenDirs();
        }

        private void MoveOutputToInstallPath(BuildPostProcessArgs args)
        {
            foreach (string str in Directory.GetDirectories(this.m_StagingAreaDataOutput, "*", SearchOption.AllDirectories))
            {
                string[] components = new string[] { args.installPath, str.Substring(this.m_StagingAreaDataOutput.Length + 1) };
                FileUtil.CreateOrCleanDirectory(Paths.Combine(components));
            }
            foreach (string str2 in Directory.GetFiles(this.m_StagingAreaDataOutput, "*", SearchOption.AllDirectories))
            {
                string[] textArray2 = new string[] { args.installPath, str2.Substring(this.m_StagingAreaDataOutput.Length + 1) };
                string destFileName = Paths.Combine(textArray2);
                File.Copy(str2, destFileName, true);
                args.report.AddFile(destFileName, !this.m_BuildFiles.ContainsKey(str2) ? "WebGL Template" : this.m_BuildFiles[str2]);
            }
        }

        public override void PostProcess(BuildPostProcessArgs args)
        {
            this.m_DevelopmentPlayer = (args.options & BuildOptions.Development) != BuildOptions.CompressTextures;
            this.m_WebGLUsePreBuiltUnityEngine = EditorUserBuildSettings.webGLUsePreBuiltUnityEngine && this.m_DevelopmentPlayer;
            this.m_UseAsm = true;
            this.m_UseWasm = PlayerSettings.WebGL.useWasm && !this.m_WebGLUsePreBuiltUnityEngine;
            this.m_NameFilesAsHashes = PlayerSettings.WebGL.nameFilesAsHashes && !this.m_DevelopmentPlayer;
            this.m_StagingAreaData = Path.GetFullPath(args.stagingAreaData);
            string[] components = new string[] { this.m_StagingAreaData, "Output" };
            this.m_StagingAreaDataOutput = Paths.Combine(components);
            string[] textArray2 = new string[] { this.m_StagingAreaDataOutput, "Build" };
            this.m_StagingAreaDataOutputBuild = Paths.Combine(textArray2);
            string[] textArray3 = new string[] { this.m_StagingAreaData, "Native" };
            this.m_StagingAreaDataNative = Paths.Combine(textArray3);
            string[] textArray4 = new string[] { this.m_StagingAreaData, "Managed" };
            this.m_StagingAreaDataManaged = Paths.Combine(textArray4);
            string[] textArray5 = new string[] { this.m_StagingAreaData, "Resources" };
            this.m_StagingAreaDataResources = Paths.Combine(textArray5);
            string[] textArray6 = new string[] { this.m_StagingAreaData, "Il2CppData" };
            this.m_StagingAreaDataIl2CppData = Paths.Combine(textArray6);
            string[] textArray7 = new string[] { EmscriptenPaths.buildToolsDir, "lib", "UnityNativeJs", "UnityNative" };
            this.m_PreBuiltUnityEngine = Paths.Combine(textArray7);
            this.m_BuildName = Path.GetFileName(args.installPath);
            this.m_TotalMemory = ((PlayerSettings.WebGL.memorySize * 0x400) * 0x400).ToString();
            char[] separator = new char[] { ':' };
            string[] strArray = PlayerSettings.WebGL.template.Split(separator);
            string[] textArray8 = new string[] { !strArray[0].Equals("PROJECT") ? EmscriptenPaths.buildToolsDir : EmscriptenPaths.dataPath, "WebGLTemplates", strArray[1] };
            this.m_TemplatePath = Paths.Combine(textArray8);
            this.m_BuildFiles = new Dictionary<string, string>();
            string[] textArray9 = new string[] { Path.GetFullPath("Library"), "webgl_cache" };
            this.m_WebGLCache = Paths.Combine(textArray9);
            if (!Directory.Exists(this.m_WebGLCache))
            {
                Directory.CreateDirectory(this.m_WebGLCache);
            }
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                HttpServerEditorWrapper.Kill();
            }
            this.m_Progress.Reset(1f);
            Directory.CreateDirectory(args.installPath);
            this.CheckBuildPrerequisites();
            this.BuildStep(args, "Scripting", "Convert and compile scripting files");
            this.CompileBuild(args);
            this.LinkBuild(args);
            this.BuildStep(args, "Files", "Packaging files");
            this.AssembleOutput(args);
            if (!this.m_DevelopmentPlayer && (PlayerSettings.WebGL.compressionFormat != WebGLCompressionFormat.Disabled))
            {
                this.BuildStep(args, "Compress", "Compressing build results");
                this.CompressBuild(args);
            }
            this.BuildStep(args, "Files", "Copying files to the final destination");
            this.MoveOutputToInstallPath(args);
            PostprocessBuildPlayer.InstallStreamingAssets(args.installPath, args.report);
            WebsockifyEditorWrapper.CreateIfNeeded();
        }

        private string PostProcessBuildFile(string path, string fileType)
        {
            if (Path.GetDirectoryName(path) != this.m_StagingAreaDataOutputBuild)
            {
                throw new Exception("Invalid build file path: " + path);
            }
            if (this.m_NameFilesAsHashes)
            {
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = b => b.ToString("x2");
                }
                string str = string.Join(string.Empty, Array.ConvertAll<byte, string>(this.m_Hash.ComputeHash(File.ReadAllBytes(path)), <>f__am$cache3));
                string[] components = new string[] { Path.GetDirectoryName(path), str + Path.GetExtension(path) };
                string destFileName = Paths.Combine(components);
                File.Move(path, destFileName);
                path = destFileName;
            }
            this.m_BuildFiles.Add(path, fileType);
            return Path.GetFileName(path);
        }

        private static bool SetGzipComment(string path, string comment)
        {
            byte[] buffer = File.ReadAllBytes(path);
            int index = 10;
            int num2 = 0;
            if (index > buffer.Length)
            {
                return false;
            }
            byte num3 = buffer[3];
            if ((num3 & 4) != 0)
            {
                if ((index + 2) > buffer.Length)
                {
                    return false;
                }
                index += (2 + buffer[index]) + (buffer[index + 1] << 8);
                if (index > buffer.Length)
                {
                    return false;
                }
            }
            if ((num3 & 8) != 0)
            {
                while ((index < buffer.Length) && (buffer[index] != 0))
                {
                    index++;
                }
                if ((index + 1) > buffer.Length)
                {
                    return false;
                }
                index++;
            }
            if ((num3 & 0x10) != 0)
            {
                while (((index + num2) < buffer.Length) && (buffer[index + num2] != 0))
                {
                    num2++;
                }
                if (((index + num2) + 1) > buffer.Length)
                {
                    return false;
                }
                num2++;
            }
            using (BinaryWriter writer = new BinaryWriter(File.Open(path, System.IO.FileMode.Create)))
            {
                buffer[3] = (byte) (buffer[3] | 0x10);
                writer.Write(buffer, 0, index);
                writer.Write(Encoding.UTF8.GetBytes(comment + '\0'));
                writer.Write(buffer, index + num2, (buffer.Length - index) - num2);
            }
            return true;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DataFile
        {
            public string path;
            public long length;
            public byte[] internalPath;
            public DataFile(string path, string internalPath)
            {
                this.path = path;
                this.length = new FileInfo(path).Length;
                this.internalPath = Encoding.UTF8.GetBytes(internalPath.Replace('\\', '/'));
            }
        }
    }
}

