namespace Unity.IL2CPP.Building
{
    using NiceIO;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Unity.IL2CPP.Building.BuildDescriptions;
    using Unity.IL2CPP.Common;

    public class BuildingOptions
    {
        public IEnumerable<NPath> AdditionalCpp = new NPath[0];
        public IEnumerable<string> AdditionalDefines = new string[0];
        public IEnumerable<NPath> AdditionalIncludeDirectories = new NPath[0];
        public IEnumerable<string> AdditionalLibraries = new string[0];
        public IEnumerable<NPath> AdditionalLinkDirectories = new NPath[0];
        public Unity.IL2CPP.Common.Architecture Architecture = Unity.IL2CPP.Common.Architecture.OfCurrentProcess;
        public NPath CacheDirectory;
        public string CompilerFlags;
        public BuildConfiguration Configuration = BuildConfiguration.Debug;
        public bool ForceRebuild;
        public NPath LibIL2CPPCacheDirectory;
        public string LinkerFlags;
        public NPath OutputPath;
        public RuntimeBuildType Runtime;
        public NPath SourceDirectory;
        public NPath ToolChainPath;
        public bool TreatWarningsAsErrors;
        public bool Verbose;

        public void Validate()
        {
            foreach (string str in this.AdditionalLibraries)
            {
                try
                {
                    if ((File.GetAttributes(str) & FileAttributes.Directory) != 0)
                    {
                        throw new ArgumentException($"Cannot specify directory "{str}" as an additional library file.", "--additional-libraries");
                    }
                }
                catch (FileNotFoundException exception)
                {
                    throw new ArgumentException($"Non-existent file "{str}" specified as an additional library file.", "--additional-libraries", exception);
                }
                catch (DirectoryNotFoundException exception2)
                {
                    throw new ArgumentException($"Non-existent directory "{str}" specified as an additional library file.  Cannot specify a directory as an additional library.", "--additional-libraries", exception2);
                }
                catch (ArgumentException)
                {
                    throw;
                }
                catch (Exception exception3)
                {
                    throw new ArgumentException($"Unknown error with additional library parameter "{str}".", "--additional-libraries", exception3);
                }
            }
        }
    }
}

