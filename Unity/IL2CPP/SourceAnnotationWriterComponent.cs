namespace Unity.IL2CPP
{
    using Mono.Cecil.Cil;
    using NiceIO;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.IoCServices;

    internal class SourceAnnotationWriterComponent : ISourceAnnotationWriter
    {
        private Dictionary<NPath, string[]> _cachedFiles = new Dictionary<NPath, string[]>();

        public void EmitAnnotation(CppCodeWriter writer, SequencePoint sequencePoint)
        {
            string[] fileLines = this.GetFileLines(sequencePoint.Document.Url.ToNPath());
            if (fileLines != null)
            {
                int startLine = sequencePoint.StartLine;
                int endLine = sequencePoint.EndLine;
                if ((startLine >= 1) && (startLine <= fileLines.Length))
                {
                    if (endLine == -1)
                    {
                        endLine = startLine;
                    }
                    startLine--;
                    endLine--;
                    if (fileLines.Length > sequencePoint.EndLine)
                    {
                        ArrayView<string> view = new ArrayView<string>(fileLines, startLine, (endLine - startLine) + 1);
                        int startIndex = 0x7fffffff;
                        for (int i = 0; i < view.Length; i++)
                        {
                            string str = view[i];
                            if (!string.IsNullOrWhiteSpace(str))
                            {
                                int num5 = 0;
                                while ((num5 < str.Length) && (str[num5] == ' '))
                                {
                                    num5++;
                                }
                                if (startIndex > num5)
                                {
                                    startIndex = num5;
                                }
                            }
                        }
                        for (int j = 0; j < view.Length; j++)
                        {
                            string str2 = view[j];
                            if (str2.Length >= startIndex)
                            {
                                str2 = str2.Substring(startIndex);
                            }
                            str2 = str2.TrimEnd(new char[0]);
                            if (CodeGenOptions.EmitSourceMapping)
                            {
                                writer.WriteLine($"//<source_info:{sequencePoint.Document.Url.ToNPath()}:{(startLine + j) + 1}>");
                            }
                            writer.WriteLine($"// {str2}");
                        }
                    }
                }
            }
        }

        private string[] GetFileLines(NPath path)
        {
            string[] strArray = null;
            if (!this._cachedFiles.TryGetValue(path, out strArray))
            {
                try
                {
                    if (path.FileExists(""))
                    {
                        strArray = File.ReadAllLines(path.ToString());
                        int length = strArray.Length;
                        for (int i = 0; i < length; i++)
                        {
                            strArray[i] = strArray[i].Replace("\t", "    ").TrimEnd(new char[0]);
                            if (strArray[i].EndsWith(@"\"))
                            {
                                string[] strArray3;
                                int num3;
                                (strArray3 = strArray)[num3 = i] = strArray3[num3] + '.';
                            }
                        }
                    }
                }
                catch
                {
                }
                this._cachedFiles.Add(path, strArray);
            }
            return strArray;
        }
    }
}

