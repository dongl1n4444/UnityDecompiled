namespace Unity.IL2CPP.Building
{
    using NiceIO;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Building.Hashing;
    using Unity.IL2CPP.Common;

    public class CompilationInvocation
    {
        public IEnumerable<string> Arguments;
        public NPath CompilerExecutable;
        public Dictionary<string, string> EnvVars;
        public NPath SourceFile;

        public Shell.ExecuteResult Execute()
        {
            return Shell.Execute(this.ToExecuteArgs(), null);
        }

        public string Hash(string hashForAllHeaderFilesPossiblyInfluencingCompilation)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string str in this.Arguments)
            {
                builder.Append(str);
            }
            builder.Append(this.CompilerExecutable);
            builder.Append(this.SourceFile);
            builder.Append(hashForAllHeaderFilesPossiblyInfluencingCompilation);
            builder.Append(HashTools.HashOfFile(this.SourceFile));
            return HashTools.HashOf(builder.ToString());
        }

        public string Summary()
        {
            Shell.ExecuteArgs args = this.ToExecuteArgs();
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Executable: " + args.Executable);
            builder.AppendLine("Arguments: " + args.Arguments);
            if (args.EnvVars != null)
            {
                foreach (KeyValuePair<string, string> pair in args.EnvVars)
                {
                    builder.AppendLine("EnvArg key: " + pair.Key + " value: " + pair.Value);
                }
            }
            return builder.ToString();
        }

        public Shell.ExecuteArgs ToExecuteArgs()
        {
            string str = this.CompilerExecutable.ToString();
            if (PlatformUtils.IsWindows())
            {
                str = this.CompilerExecutable.InQuotes();
            }
            return new Shell.ExecuteArgs { 
                Arguments = ExtensionMethods.SeparateWithSpaces(this.Arguments),
                EnvVars = this.EnvVars,
                Executable = str
            };
        }
    }
}

