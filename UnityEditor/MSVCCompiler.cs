using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.Utils;

internal class MSVCCompiler : NativeCompiler
{
    private readonly string m_CompilerOptions = "/bigobj /Od /Zi /MTd /MP /EHsc /D_SECURE_SCL=0 /D_HAS_ITERATOR_DEBUGGING=0";
    private readonly string m_DefFile;
    private readonly string[] m_IncludePaths = new string[] { (VisualStudioDir() + @"\include"), (ProgramFilesx86() + @"\Microsoft SDKs\Windows\v7.0A\Include") };
    private readonly string[] m_Libraries = new string[] { "user32.lib", "advapi32.lib", "ole32.lib", "oleaut32.lib", "ws2_32.lib", "Shell32.lib", "Psapi.lib" };
    private readonly ICompilerSettings m_Settings;

    public MSVCCompiler(ICompilerSettings settings, string defFile)
    {
        this.m_Settings = settings;
        this.m_DefFile = defFile;
    }

    private void Compile(string file, string includePaths)
    {
        string str = $" /c {this.m_CompilerOptions} "{file}" {includePaths} /Fo{Path.GetDirectoryName(file)}\ ";
        base.Execute($"{str}", this.m_Settings.CompilerPath);
    }

    public override void CompileDynamicLibrary(string outputFile, IEnumerable<string> sources, IEnumerable<string> includePaths, IEnumerable<string> libraries, IEnumerable<string> libraryPaths)
    {
        <CompileDynamicLibrary>c__AnonStorey81 storey = new <CompileDynamicLibrary>c__AnonStorey81 {
            <>f__this = this
        };
        string[] source = sources.ToArray<string>();
        string str = NativeCompiler.Aggregate(source.Select<string, string>(new Func<string, string>(this.ObjectFileFor)), " \"", "\" " + Environment.NewLine);
        storey.includePathsString = NativeCompiler.Aggregate(includePaths.Union<string>(this.m_IncludePaths), "/I \"", "\" ");
        string str2 = NativeCompiler.Aggregate(libraries.Union<string>(this.m_Libraries), " ", " ");
        string str3 = NativeCompiler.Aggregate(libraryPaths.Union<string>(this.m_Settings.LibPaths), "/LIBPATH:\"", "\" ");
        this.GenerateEmptyPdbFile(outputFile);
        NativeCompiler.ParallelFor<string>(source, new Action<string>(storey.<>m__12C));
        string contents = string.Format(" {0} {1} {2} /DEBUG /INCREMENTAL:NO /MACHINE:{4} /DLL /out:\"{3}\" /MAP /DEF:\"{5}\" ", new object[] { str, str2, str3, outputFile, this.m_Settings.MachineSpecification, this.m_DefFile });
        string tempFileName = Path.GetTempFileName();
        File.WriteAllText(tempFileName, contents);
        base.Execute($"@{tempFileName}", VisualStudioDir() + @"\bin\link.exe");
        string command = Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), "Tools/MapFileParser/MapFileParser.exe");
        string str7 = "\"" + Path.GetFullPath(Path.Combine(Path.GetDirectoryName(outputFile), Path.GetFileNameWithoutExtension(outputFile) + ".map")) + "\"";
        string str8 = "\"" + Path.GetFullPath(Path.Combine(Path.GetDirectoryName(outputFile), "SymbolMap")) + "\"";
        string[] arguments = new string[] { "-format=MSVC", str7, str8 };
        base.ExecuteCommand(command, arguments);
    }

    private void GenerateEmptyPdbFile(string outputFile)
    {
        string tempFileName = Path.GetTempFileName();
        File.WriteAllText(tempFileName, " /* **** */");
        string fullPath = Path.GetFullPath(Path.GetDirectoryName(outputFile));
        string str3 = Path.Combine(fullPath, Path.GetFileNameWithoutExtension(outputFile) + ".pdb");
        Directory.CreateDirectory(fullPath);
        string str4 = $" -c /Tp {tempFileName} /Zi /Fd"{str3}"";
        base.Execute($"{str4}", this.m_Settings.CompilerPath);
    }

    private static string ProgramFilesx86()
    {
        if ((IntPtr.Size != 8) && string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432")))
        {
            return Environment.GetEnvironmentVariable("ProgramFiles");
        }
        return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
    }

    protected override void SetupProcessStartInfo(ProcessStartInfo startInfo)
    {
        string str = Environment.ExpandEnvironmentVariables(@"%VS100COMNTOOLS%..\IDE");
        startInfo.CreateNoWindow = true;
        if (!startInfo.EnvironmentVariables.ContainsKey("PATH"))
        {
            startInfo.EnvironmentVariables.Add("PATH", str);
        }
        else
        {
            string str2 = startInfo.EnvironmentVariables["PATH"];
            str2 = str + Path.PathSeparator + str2;
            startInfo.EnvironmentVariables["PATH"] = str2;
        }
    }

    private static string VisualStudioDir() => 
        Environment.ExpandEnvironmentVariables(@"%VS100COMNTOOLS%..\..\VC");

    protected override string objectFileExtension =>
        "obj";

    [CompilerGenerated]
    private sealed class <CompileDynamicLibrary>c__AnonStorey81
    {
        internal MSVCCompiler <>f__this;
        internal string includePathsString;

        internal void <>m__12C(string file)
        {
            this.<>f__this.Compile(file, this.includePathsString);
        }
    }
}

