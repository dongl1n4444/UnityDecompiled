namespace Unity.IL2CPP.Building.ToolChains
{
    using NiceIO;
    using System;
    using System.Collections.Generic;

    public class CppToolChainContext
    {
        private readonly List<CppCompilationInstruction> _extraCompileInstructions = new List<CppCompilationInstruction>();
        private readonly List<NPath> _extraIncludeDirectories = new List<NPath>();

        public void AddCompileInstructions(IEnumerable<CppCompilationInstruction> compileInstructions)
        {
            this._extraCompileInstructions.AddRange(compileInstructions);
        }

        public void AddIncludeDirectory(NPath includeDirectory)
        {
            this._extraIncludeDirectories.Add(includeDirectory);
        }

        public IEnumerable<CppCompilationInstruction> ExtraCompileInstructions
        {
            get
            {
                return this._extraCompileInstructions;
            }
        }

        public IEnumerable<NPath> ExtraIncludeDirectories
        {
            get
            {
                return this._extraIncludeDirectories;
            }
        }
    }
}

