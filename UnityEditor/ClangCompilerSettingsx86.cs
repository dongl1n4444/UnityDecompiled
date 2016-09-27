using System;

internal class ClangCompilerSettingsx86 : ICompilerSettings
{
    public string CompilerPath =>
        "clang++";

    public string[] LibPaths =>
        new string[0];

    public string LinkerPath =>
        "ld";

    public string MachineSpecification =>
        "i386";
}

