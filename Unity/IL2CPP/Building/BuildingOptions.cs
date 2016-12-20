namespace Unity.IL2CPP.Building
{
    using NiceIO;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class BuildingOptions
    {
        public IEnumerable<NPath> AdditionalCpp = new NPath[0];
        public IEnumerable<string> AdditionalDefines = new string[0];
        public IEnumerable<NPath> AdditionalIncludeDirectories = new NPath[0];
        public IEnumerable<string> AdditionalLibraries = new string[0];
        public IEnumerable<NPath> AdditionalLinkDirectories = new NPath[0];
        public Unity.IL2CPP.Building.Architecture Architecture = Unity.IL2CPP.Building.Architecture.OfCurrentProcess;
        public NPath CacheDirectory;
        public string CompilerFlags;
        public BuildConfiguration Configuration = BuildConfiguration.Debug;
        public bool ForceRebuild;
        public NPath LibIL2CPPCacheDirectory;
        public bool LibIL2CPPDynamic;
        public string LinkerFlags;
        public NPath OutputPath;
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
                        throw new ArgumentException(string.Format("Cannot specify directory \"{0}\" as an additional library file.", str), "--additional-libraries");
                    }
                }
                catch (FileNotFoundException exception)
                {
                    throw new ArgumentException(string.Format("Non-existent file \"{0}\" specified as an additional library file.", str), "--additional-libraries", exception);
                }
                catch (DirectoryNotFoundException exception2)
                {
                    throw new ArgumentException(string.Format("Non-existent directory \"{0}\" specified as an additional library file.  Cannot specify a directory as an additional library.", str), "--additional-libraries", exception2);
                }
                catch (ArgumentException)
                {
                    throw;
                }
                catch (Exception exception3)
                {
                    throw new ArgumentException(string.Format("Unknown error with additional library parameter \"{0}\".", str), "--additional-libraries", exception3);
                }
            }
        }
    }
}

