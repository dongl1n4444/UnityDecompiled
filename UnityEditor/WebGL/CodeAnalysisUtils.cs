namespace UnityEditor.WebGL
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;

    internal class CodeAnalysisUtils
    {
        public static void ExtractFunctionsFromJS(string code, Action<string, string> processFunction, Action<string> processOther)
        {
            string str = null;
            int index = code.IndexOf("// EMSCRIPTEN_START_FUNCS");
            int num2 = code.IndexOf("// EMSCRIPTEN_END_FUNCS");
            int startIndex = 0;
            int num4 = 0;
        Label_0020:
            index = code.IndexOf("function ", index);
            if (index != -1)
            {
                if ((index > 0) && IsWordCharacter(code[index - 1]))
                {
                    goto Label_0020;
                }
                if (index <= num2)
                {
                    if (str != null)
                    {
                        string str2 = code.Substring(startIndex, index - startIndex);
                        processFunction(str, str2);
                    }
                    else
                    {
                        processOther(code.Substring(startIndex, index - startIndex));
                    }
                    startIndex = index;
                    index += 9;
                    num4 = code.IndexOf('(', index);
                    str = code.Substring(index, num4 - index);
                    goto Label_0020;
                }
            }
            processOther(code.Substring(startIndex));
        }

        private static bool IsWordCharacter(char ch) => 
            (((((ch >= 'a') && (ch <= 'z')) || ((ch >= 'A') && (ch <= 'Z'))) || (((ch >= '0') && (ch <= '9')) || (ch == '_'))) || (ch == '$'));

        public static Dictionary<string, string> ReadMinificationMap(string mapPath)
        {
            if (!File.Exists(mapPath))
            {
                return null;
            }
            string[] strArray = File.ReadAllLines(mapPath);
            Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
            foreach (string str in strArray)
            {
                char[] separator = new char[] { ':' };
                string[] strArray3 = str.Split(separator);
                dictionary2[strArray3[0]] = strArray3[1];
            }
            return dictionary2;
        }

        private static string ReplaceDuplicates(string codeIn, Dictionary<string, string> minificationMap)
        {
            <ReplaceDuplicates>c__AnonStorey0 storey = new <ReplaceDuplicates>c__AnonStorey0 {
                minificationMap = minificationMap,
                patchedCode = new StringBuilder(),
                hashFuncs = new Dictionary<string, List<string>>(),
                functionReplacement = new Dictionary<string, string>()
            };
            ExtractFunctionsFromJS(codeIn, new Action<string, string>(storey.<>m__0), new Action<string>(storey.<>m__1));
            string str = storey.patchedCode.ToString();
            storey.patchedCode = new StringBuilder();
            string key = "";
            bool flag = false;
            bool flag2 = false;
            foreach (char ch in str)
            {
                if (IsWordCharacter(ch))
                {
                    key = key + ch;
                }
                else
                {
                    if (key.Length > 0)
                    {
                        if (key == "EMSCRIPTEN_END_FUNCS")
                        {
                            flag = true;
                        }
                        if (flag && (key == "return"))
                        {
                            flag2 = true;
                        }
                        if ((((ch == '(') || (flag && !flag2)) || (flag2 && (ch != ':'))) && storey.functionReplacement.ContainsKey(key))
                        {
                            key = storey.functionReplacement[key];
                        }
                        storey.patchedCode.Append(key);
                        key = "";
                    }
                    storey.patchedCode.Append(ch);
                }
            }
            str = storey.patchedCode.ToString();
            Console.WriteLine("" + storey.functionReplacement.Count<KeyValuePair<string, string>>() + " functions replaced.");
            return str;
        }

        public static Dictionary<string, string> ReplaceDuplicates(string builtCodePath, int interations)
        {
            Dictionary<string, string> minificationMap = ReadMinificationMap(builtCodePath.Substring(0, builtCodePath.Length - 7) + ".js.symbols");
            string codeIn = File.ReadAllText(builtCodePath);
            for (int i = 0; i < interations; i++)
            {
                codeIn = ReplaceDuplicates(codeIn, minificationMap);
            }
            File.WriteAllText(builtCodePath, codeIn);
            return minificationMap;
        }

        [CompilerGenerated]
        private sealed class <ReplaceDuplicates>c__AnonStorey0
        {
            internal Dictionary<string, string> functionReplacement;
            internal Dictionary<string, List<string>> hashFuncs;
            internal Dictionary<string, string> minificationMap;
            internal StringBuilder patchedCode;

            internal void <>m__0(string name, string code)
            {
                int index = code.IndexOf('(');
                string key = code.Substring(index);
                if (!this.hashFuncs.ContainsKey(key))
                {
                    this.hashFuncs[key] = new List<string>();
                    this.patchedCode.Append(code);
                }
                else
                {
                    this.functionReplacement[name] = this.hashFuncs[key][0];
                    if (this.minificationMap != null)
                    {
                        this.minificationMap.Remove(name);
                        this.minificationMap.Remove(this.hashFuncs[key][0]);
                    }
                }
                this.hashFuncs[key].Add(name);
            }

            internal void <>m__1(string code)
            {
                this.patchedCode.Append(code);
            }
        }
    }
}

