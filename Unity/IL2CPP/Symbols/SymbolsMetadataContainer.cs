namespace Unity.IL2CPP.Symbols
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;

    internal class SymbolsMetadataContainer
    {
        private Dictionary<string, Dictionary<string, List<LineNumberPair>>> m_CPPFilenameToDictionaryOfCSFilenameToLineNumberPairList = new Dictionary<string, Dictionary<string, List<LineNumberPair>>>();

        public void Add(string cppFileName, string csFileName, string cppLineNum, string csLineNum)
        {
            Dictionary<string, List<LineNumberPair>> dictionary;
            if (!this.m_CPPFilenameToDictionaryOfCSFilenameToLineNumberPairList.TryGetValue(cppFileName, out dictionary))
            {
                List<LineNumberPair> list = new List<LineNumberPair> {
                    new LineNumberPair(cppLineNum, csLineNum)
                };
                dictionary = new Dictionary<string, List<LineNumberPair>> {
                    { 
                        csFileName,
                        list
                    }
                };
                this.m_CPPFilenameToDictionaryOfCSFilenameToLineNumberPairList.Add(cppFileName, dictionary);
            }
            else
            {
                List<LineNumberPair> list2;
                if (!dictionary.TryGetValue(csFileName, out list2))
                {
                    list2 = new List<LineNumberPair> {
                        new LineNumberPair(cppLineNum, csLineNum)
                    };
                    dictionary.Add(csFileName, list2);
                }
                else
                {
                    dictionary.TryGetValue(csFileName, out list2);
                    list2.Add(new LineNumberPair(cppLineNum, csLineNum));
                }
            }
        }

        public void SerializeToJson(StreamWriter outputStream)
        {
            outputStream.WriteLine("{");
            bool flag = true;
            foreach (KeyValuePair<string, Dictionary<string, List<LineNumberPair>>> pair in this.m_CPPFilenameToDictionaryOfCSFilenameToLineNumberPairList)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    outputStream.WriteLine(",");
                }
                string str = pair.Key.Replace(@"\", @"\\");
                outputStream.WriteLine($""{str}" : [");
                bool flag2 = true;
                foreach (KeyValuePair<string, List<LineNumberPair>> pair2 in pair.Value)
                {
                    if (flag2)
                    {
                        flag2 = false;
                    }
                    else
                    {
                        outputStream.WriteLine(",");
                    }
                    outputStream.WriteLine("{");
                    string str2 = pair2.Key.Replace(@"\", @"\\");
                    outputStream.WriteLine($""{str2}" : [");
                    bool flag3 = true;
                    foreach (LineNumberPair pair3 in pair2.Value)
                    {
                        if (flag3)
                        {
                            flag3 = false;
                        }
                        else
                        {
                            outputStream.WriteLine(",");
                        }
                        string cppLineNumber = pair3.CppLineNumber;
                        string csLineNumber = pair3.CsLineNumber;
                        outputStream.Write("{ \"" + cppLineNumber + "\" : \"" + csLineNumber + "\" }");
                    }
                    outputStream.WriteLine();
                    outputStream.WriteLine("]");
                    outputStream.Write("}");
                }
                outputStream.WriteLine();
                outputStream.Write("]");
            }
            outputStream.WriteLine();
            outputStream.WriteLine("}");
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LineNumberPair
        {
            public readonly string CppLineNumber;
            public readonly string CsLineNumber;
            public LineNumberPair(string cppLineNumber, string csLineNumber)
            {
                this.CppLineNumber = cppLineNumber;
                this.CsLineNumber = csLineNumber;
            }
        }
    }
}

