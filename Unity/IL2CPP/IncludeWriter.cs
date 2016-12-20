namespace Unity.IL2CPP
{
    using System;

    public class IncludeWriter
    {
        internal static void WriteRegistrationIncludes(CppCodeWriter writer)
        {
            writer.AddInclude("il2cpp-config.h");
            writer.AddInclude("class-internals.h");
            writer.AddInclude("codegen/il2cpp-codegen.h");
            writer.AddStdInclude("cstring");
            writer.AddStdInclude("string.h");
            writer.AddStdInclude("stdio.h");
            writer.AddStdInclude("cmath");
            writer.AddStdInclude("limits");
            writer.AddStdInclude("assert.h");
            writer.WriteLine();
        }
    }
}

