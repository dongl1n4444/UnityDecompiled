namespace Unity.IL2CPP.Building
{
    using NiceIO;
    using System;

    internal class IntermediateObjectFileCompilationData
    {
        public Unity.IL2CPP.Building.CompilationInvocation CompilationInvocation;
        public Unity.IL2CPP.Building.CppCompilationInstruction CppCompilationInstruction;
        public NPath ObjectFile;
    }
}

