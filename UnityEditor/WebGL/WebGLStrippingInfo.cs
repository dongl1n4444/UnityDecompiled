namespace UnityEditor.WebGL
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEditor.BuildReporting;
    using UnityEditor.Utils;
    using UnityEditor.WebGL.Emscripten;
    using UnityEditorInternal;

    internal class WebGLStrippingInfo : StrippingInfo
    {
        [CompilerGenerated]
        private static Func<string, string, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<string, string, string> <>f__am$cache1;
        [CompilerGenerated]
        private static Comparison<KeyValuePair<string, int>> <>f__am$cache2;
        [CompilerGenerated]
        private static Comparison<KeyValuePair<string, int>> <>f__am$cache3;
        [CompilerGenerated]
        private static Action<string> <>f__am$cache4;
        public string builtCodePath;
        public bool developmentBuild;
        private static char[] objectTrimChar = new char[] { ':' };
        private static string[] objectTrimPrefixes = new string[] { "Temp/", "lib/modules/WebGLSupport_", "lib/modules_development/WebGLSupport_" };

        public void Analyze()
        {
            <Analyze>c__AnonStorey1 storey = new <Analyze>c__AnonStorey1();
            bool flag = Unsupported.IsDeveloperBuild();
            string directory = Path.Combine(EmscriptenPaths.emscriptenCache, "asmjs");
            string str2 = Il2CppNativeCodeBuilderUtils.ObjectFilePathInCacheDirectoryFor(EmscriptenPaths.cacheDirForIl2CppIncrementalBuildArtifacts);
            IEnumerable<string> first = GetDirectorySymbolArtifactsFromGeneratedCode(str2).Concat<string>(GetDirectorySymbolArtifacts(directory));
            if (flag)
            {
                first = first.Concat<string>(GetSourceCodeSymbolArtifacts());
            }
            Dictionary<string, string> symbolArtifacts = GetSymbolArtifacts(first);
            string str3 = !this.developmentBuild ? "lib/modules" : "lib/modules_development";
            Dictionary<string, string> dictionary2 = GetSymbolArtifacts(GetDirectorySymbolArtifacts(Path.Combine(EmscriptenPaths.buildToolsDir, str3)).ToArray<string>());
            Dictionary<string, int> sizes = new Dictionary<string, int>();
            Dictionary<string, int> dictionary4 = new Dictionary<string, int>();
            Dictionary<string, int> dictionary5 = new Dictionary<string, int>();
            storey.functionSizes = new Dictionary<string, int>();
            int num = 0;
            base.totalSize = 0;
            bool flag2 = Path.GetExtension(this.builtCodePath) == ".wasm";
            storey.minificationMap = CodeAnalysisUtils.ReadMinificationMap(this.builtCodePath.Substring(0, this.builtCodePath.Length - (!flag2 ? 7 : 5)) + ".js.symbols");
            if (flag2)
            {
                int num2;
                string wastPath = "Temp/StagingArea/Data/linkresult_wasm/build.wast";
                ProcessStartInfo si = new ProcessStartInfo(EmscriptenPaths.binaryenDisExecutable) {
                    Arguments = "\"" + this.builtCodePath + "\" -o " + wastPath,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                Program program = new Program(si);
                program.Start();
                program.WaitForExit();
                if (program.ExitCode != 0)
                {
                    throw new Exception("wasm-dis failed with arguments: " + si.Arguments);
                }
                storey.functionSizes = GetFunctionSizesFromWast(wastPath, out num2, storey.minificationMap);
                base.totalSize = (int) new FileInfo(this.builtCodePath).Length;
                float num3 = ((float) base.totalSize) / ((float) num2);
                foreach (string str7 in storey.functionSizes.Keys.ToList<string>())
                {
                    storey.functionSizes[str7] = (int) (((float) storey.functionSizes[str7]) * num3);
                }
            }
            else
            {
                <Analyze>c__AnonStorey2 storey2 = new <Analyze>c__AnonStorey2 {
                    <>f__ref$1 = storey
                };
                string code = File.ReadAllText(this.builtCodePath);
                base.totalSize = code.Length;
                int index = code.IndexOf("// EMSCRIPTEN_START_FUNCS");
                int num5 = code.IndexOf("// EMSCRIPTEN_END_FUNCS");
                int num6 = (index + base.totalSize) - num5;
                dictionary4["JS Pre- and Postfix code"] = num6;
                storey2.trimChar = new char[] { '_' };
                if (<>f__am$cache4 == null)
                {
                    <>f__am$cache4 = delegate (string functionCode) {
                    };
                }
                CodeAnalysisUtils.ExtractFunctionsFromJS(code, new Action<string, string>(storey2.<>m__0), <>f__am$cache4);
            }
            foreach (KeyValuePair<string, int> pair in storey.functionSizes)
            {
                Dictionary<string, int> dictionary6;
                if (symbolArtifacts.ContainsKey(pair.Key))
                {
                    string key = symbolArtifacts[pair.Key].Replace('\\', '/');
                    if (flag)
                    {
                        string str10;
                        if (!dictionary5.ContainsKey(key))
                        {
                            dictionary5[key] = 0;
                        }
                        (dictionary6 = dictionary5)[str10 = key] = dictionary6[str10] + pair.Value;
                    }
                    if (key.LastIndexOf('/') != -1)
                    {
                        string str12;
                        string str11 = key.Substring(0, key.LastIndexOf('/'));
                        if (!sizes.ContainsKey(str11))
                        {
                            sizes[str11] = 0;
                        }
                        (dictionary6 = sizes)[str12 = str11] = dictionary6[str12] + pair.Value;
                    }
                }
                if (dictionary2.ContainsKey(pair.Key))
                {
                    string str14;
                    string str13 = dictionary2[pair.Key];
                    str13 = StrippingInfo.ModuleName(str13.Substring(0, str13.Length - "Module_Dynamic.bc".Length));
                    if (!dictionary4.ContainsKey(str13))
                    {
                        dictionary4[str13] = 0;
                    }
                    (dictionary6 = dictionary4)[str14 = str13] = dictionary6[str14] + pair.Value;
                    num += pair.Value;
                }
            }
            dictionary4["IL2CPP Generated"] = sizes[LeafDirectoryFor(str2)];
            dictionary4["emscripten runtime"] = sizes[directory.Replace('\\', '/')];
            base.AddModule("IL2CPP Generated");
            if (!flag2)
            {
                base.AddModule("JS Pre- and Postfix code");
            }
            base.AddModule("emscripten runtime");
            int totalSize = base.totalSize;
            foreach (KeyValuePair<string, int> pair2 in dictionary4)
            {
                if (base.modules.Contains(pair2.Key))
                {
                    totalSize -= pair2.Value;
                }
            }
            dictionary4["Unaccounted"] = totalSize;
            base.AddModule("Unaccounted");
            foreach (KeyValuePair<string, int> pair3 in dictionary4)
            {
                base.AddModuleSize(pair3.Key, pair3.Value);
            }
            if (flag)
            {
                Console.WriteLine("Code size per module: ");
                PrintSizesDictionary(dictionary4);
                Console.WriteLine("\n\n");
                Console.WriteLine("Code size per source folder: ");
                PrintSizesDictionary(sizes);
                Console.WriteLine("\n\n");
                Console.WriteLine("Code size per object file: ");
                PrintSizesDictionary(dictionary5);
                Console.WriteLine("\n\n");
                Console.WriteLine("Code size per function: ");
                PrintSizesDictionary(storey.functionSizes);
                Console.WriteLine("\n\n");
            }
        }

        private static string[] GetDirectorySymbolArtifacts(string directory)
        {
            string[] strArray = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
            ProcessStartInfo info2 = new ProcessStartInfo(EmscriptenPaths.nmExecutable);
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = (current, additionalFile) => current + " \"" + additionalFile + "\"";
            }
            info2.Arguments = "-n " + Enumerable.Aggregate<string, string>(strArray, "", <>f__am$cache0);
            info2.UseShellExecute = false;
            info2.CreateNoWindow = true;
            ProcessStartInfo si = info2;
            Program program = new Program(si);
            program.Start();
            program.WaitForExit();
            return program.GetStandardOutput();
        }

        private static string[] GetDirectorySymbolArtifactsFromGeneratedCode(string directory)
        {
            <GetDirectorySymbolArtifactsFromGeneratedCode>c__AnonStorey0 storey = new <GetDirectorySymbolArtifactsFromGeneratedCode>c__AnonStorey0 {
                firstSubdirectory = LeafDirectoryFor(directory)
            };
            IEnumerable<string> enumerable = Enumerable.Select<string, string>(Directory.GetFiles(directory), new Func<string, string>(storey.<>m__0));
            ProcessStartInfo info2 = new ProcessStartInfo(EmscriptenPaths.nmExecutable);
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = (current, additionalFile) => current + " \"" + additionalFile + "\"";
            }
            info2.Arguments = "-n " + Enumerable.Aggregate<string, string>(enumerable, "", <>f__am$cache1);
            info2.UseShellExecute = false;
            info2.CreateNoWindow = true;
            info2.WorkingDirectory = Path.GetDirectoryName(UseForwardSlashesAndTrimTrailingSlashFromDirectoryName(directory));
            ProcessStartInfo si = info2;
            Program program = new Program(si);
            program.Start();
            program.WaitForExit();
            return program.GetStandardOutput();
        }

        private static Dictionary<string, int> GetFunctionSizesFromWast(string wastPath, out int totalWastSize, Dictionary<string, string> minificationMap)
        {
            ProcessStartInfo si = new ProcessStartInfo(EmscriptenPaths.binaryenShellExecutable) {
                Arguments = "\"" + wastPath + "\" --nm",
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Program program = new Program(si);
            program.Start();
            program.WaitForExit();
            if (program.ExitCode != 0)
            {
                throw new Exception("wasm-opt failed with arguments: " + si.Arguments);
            }
            string[] standardOutput = program.GetStandardOutput();
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            totalWastSize = 0;
            foreach (string str in standardOutput)
            {
                char[] separator = new char[] { ':' };
                string[] strArray3 = str.Split(separator);
                string key = strArray3[0].Trim(new char[] { ' ', '$', '_', '\0' });
                if (minificationMap.ContainsKey(key))
                {
                    key = minificationMap[key].Trim(new char[] { ' ', '$', '_', '\0' });
                }
                int num2 = int.Parse(strArray3[1]);
                dictionary[key] = num2;
                totalWastSize += num2;
            }
            return dictionary;
        }

        private static IEnumerable<string> GetSourceCodeSymbolArtifacts()
        {
            string str2 = FileUtil.DeleteLastPathNameComponent(FileUtil.DeleteLastPathNameComponent(BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.WebGL, BuildOptions.CompressTextures)));
            string[] components = new string[] { str2, "artifacts" };
            string path = Paths.Combine(components);
            IEnumerable<string> first = new string[0];
            string[] directories = Directory.GetDirectories(path, "webgl_release*1");
            foreach (string str4 in directories)
            {
                string[] textArray2 = new string[] { path, str4 };
                first = first.Concat<string>(GetDirectorySymbolArtifacts(Paths.Combine(textArray2)));
            }
            return first;
        }

        private static Dictionary<string, string> GetSymbolArtifacts(IEnumerable<string> lines)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string str = null;
            char[] trimChars = new char[] { '_', '-', ' ', '\0' };
            foreach (string str2 in lines)
            {
                if (str2.EndsWith(":"))
                {
                    str = ParseObjectName(str2);
                }
                else
                {
                    string str3 = str2.Trim(trimChars);
                    if ((str3.Length > 0) && (((str3[0] == 'T') || (str3[0] == 't')) || ((str3[0] == 'W') || (str3[0] == 'w'))))
                    {
                        string key = str3.Substring(1).Trim(trimChars);
                        if (!dictionary.ContainsKey(key))
                        {
                            dictionary[key] = str;
                        }
                    }
                }
            }
            return dictionary;
        }

        private static string LeafDirectoryFor(string directory)
        {
            directory = UseForwardSlashesAndTrimTrailingSlashFromDirectoryName(directory);
            return Path.GetFileName(directory);
        }

        private static void OutputSizes(Dictionary<string, int> sizes, int totalLines)
        {
            List<KeyValuePair<string, int>> list = sizes.ToList<KeyValuePair<string, int>>();
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = (firstPair, nextPair) => nextPair.Value.CompareTo(firstPair.Value);
            }
            list.Sort(<>f__am$cache2);
            foreach (KeyValuePair<string, int> pair in list)
            {
                if (pair.Value < 0x2710)
                {
                    break;
                }
                string[] textArray1 = new string[] { pair.Value.ToString("D6"), " ", ((((double) pair.Value) * 100.0) / ((double) totalLines)).ToString("F2"), "% ", pair.Key };
                Console.WriteLine(string.Concat(textArray1));
            }
        }

        private static string ParseObjectName(string obj)
        {
            string str = obj;
            str = str.Trim(objectTrimChar);
            foreach (string str2 in objectTrimPrefixes)
            {
                int index = str.Replace('\\', '/').IndexOf(str2);
                if (index != -1)
                {
                    str = str.Substring(index + str2.Length);
                }
            }
            return str;
        }

        private static void PrintSizesDictionary(Dictionary<string, int> sizes)
        {
            PrintSizesDictionary(sizes, 500);
        }

        private static void PrintSizesDictionary(Dictionary<string, int> sizes, int maxSize)
        {
            List<KeyValuePair<string, int>> list = sizes.ToList<KeyValuePair<string, int>>();
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = (pair1, pair2) => pair2.Value.CompareTo(pair1.Value);
            }
            list.Sort(<>f__am$cache3);
            for (int i = 0; (i < maxSize) && (i < list.Count); i++)
            {
                KeyValuePair<string, int> pair = list[i];
                KeyValuePair<string, int> pair2 = list[i];
                Console.WriteLine(pair.Value.ToString("D8") + " " + pair2.Key);
            }
        }

        private static string UseForwardSlashesAndTrimTrailingSlashFromDirectoryName(string directory)
        {
            directory = directory.Replace('\\', '/');
            char[] trimChars = new char[] { '/' };
            directory = directory.TrimEnd(trimChars);
            return directory;
        }

        [CompilerGenerated]
        private sealed class <Analyze>c__AnonStorey1
        {
            internal Dictionary<string, int> functionSizes;
            internal Dictionary<string, string> minificationMap;
        }

        [CompilerGenerated]
        private sealed class <Analyze>c__AnonStorey2
        {
            internal WebGLStrippingInfo.<Analyze>c__AnonStorey1 <>f__ref$1;
            internal char[] trimChar;

            internal void <>m__0(string name, string functionCode)
            {
                Dictionary<string, int> dictionary;
                string str;
                if (this.<>f__ref$1.minificationMap.ContainsKey(name))
                {
                    name = this.<>f__ref$1.minificationMap[name];
                }
                name = name.Trim(this.trimChar);
                if (!this.<>f__ref$1.functionSizes.ContainsKey(name))
                {
                    this.<>f__ref$1.functionSizes[name] = 0;
                }
                (dictionary = this.<>f__ref$1.functionSizes)[str = name] = dictionary[str] + functionCode.Length;
            }
        }

        [CompilerGenerated]
        private sealed class <GetDirectorySymbolArtifactsFromGeneratedCode>c__AnonStorey0
        {
            internal string firstSubdirectory;

            internal string <>m__0(string path) => 
                Path.Combine(this.firstSubdirectory, Path.GetFileName(path));
        }
    }
}

