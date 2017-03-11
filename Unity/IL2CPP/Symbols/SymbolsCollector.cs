namespace Unity.IL2CPP.Symbols
{
    using NiceIO;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Portability;
    using Unity.TinyProfiling;

    public class SymbolsCollector
    {
        private Regex m_regexPattern = new Regex("\t*//<source_info:(.+):(\\d+)>", RegexOptions.Compiled);
        private SymbolsMetadataContainer m_SymbolsMetadataContainer = new SymbolsMetadataContainer();
        private List<string> m_visitedCppFiles = new List<string>();
        private const string REGEX_PATTERN_STRING = "\t*//<source_info:(.+):(\\d+)>";

        public void CollectLineNumberInformation(NPath CppSourcePath)
        {
            using (TinyProfiler.Section("SymbolsCollection", ""))
            {
                if (!this.m_visitedCppFiles.Contains(CppSourcePath.ToString()))
                {
                    using (System.IO.StreamReader reader = new Unity.IL2CPP.Portability.StreamReader(CppSourcePath.ToString()))
                    {
                        uint num = 0;
                        while (!reader.EndOfStream)
                        {
                            string input = reader.ReadLine();
                            num++;
                            Match match = this.m_regexPattern.Match(input);
                            if (match.Success)
                            {
                                string csFileName = match.Groups[1].ToString();
                                string csLineNum = match.Groups[2].ToString();
                                this.m_SymbolsMetadataContainer.Add(CppSourcePath.ToString(), csFileName, num.ToString(), csLineNum);
                            }
                        }
                    }
                    this.m_visitedCppFiles.Add(CppSourcePath.ToString());
                }
            }
        }

        public void EmitLineMappingFile(NPath outputPath)
        {
            if (CodeGenOptions.EmitSourceMapping)
            {
                using (TinyProfiler.Section("SymbolsCollection", ""))
                {
                    string[] append = new string[] { "LineNumberMappings.json" };
                    NPath path = outputPath.Combine(append);
                    Directory.CreateDirectory(outputPath.ToString());
                    using (System.IO.StreamWriter writer = new Unity.IL2CPP.Portability.StreamWriter(path.ToString()))
                    {
                        this.m_SymbolsMetadataContainer.SerializeToJson(writer);
                    }
                }
            }
        }
    }
}

