namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public abstract class Il2CppNativeCodeBuilder
    {
        protected Il2CppNativeCodeBuilder()
        {
        }

        public virtual IEnumerable<string> ConvertIncludesToFullPaths(IEnumerable<string> relativeIncludePaths)
        {
            <ConvertIncludesToFullPaths>c__AnonStorey0 storey = new <ConvertIncludesToFullPaths>c__AnonStorey0 {
                workingDirectory = Directory.GetCurrentDirectory()
            };
            return Enumerable.Select<string, string>(relativeIncludePaths, new Func<string, string>(storey, (IntPtr) this.<>m__0));
        }

        public virtual string ConvertOutputFileToFullPath(string outputFileRelativePath)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), outputFileRelativePath);
        }

        protected virtual void SetupEnvironment(ProcessStartInfo startInfo)
        {
        }

        public void SetupStartInfo(ProcessStartInfo startInfo)
        {
            if (this.SetsUpEnvironment)
            {
                this.SetupEnvironment(startInfo);
            }
        }

        public virtual IEnumerable<string> AdditionalIl2CPPArguments
        {
            get
            {
                return new string[0];
            }
        }

        public virtual string CacheDirectory
        {
            get
            {
                return string.Empty;
            }
        }

        public abstract string CompilerArchitecture { get; }

        public virtual string CompilerFlags
        {
            get
            {
                return string.Empty;
            }
        }

        public abstract string CompilerPlatform { get; }

        public virtual string LinkerFlags
        {
            get
            {
                return string.Empty;
            }
        }

        public virtual string PluginPath
        {
            get
            {
                return string.Empty;
            }
        }

        public virtual bool SetsUpEnvironment
        {
            get
            {
                return false;
            }
        }

        [CompilerGenerated]
        private sealed class <ConvertIncludesToFullPaths>c__AnonStorey0
        {
            internal string workingDirectory;

            internal string <>m__0(string path)
            {
                return Path.Combine(this.workingDirectory, path);
            }
        }
    }
}

