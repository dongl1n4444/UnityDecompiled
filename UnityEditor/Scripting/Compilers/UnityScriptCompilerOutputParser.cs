namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;

    internal class UnityScriptCompilerOutputParser : CompilerOutputParserBase
    {
        [CompilerGenerated]
        private static Func<string, Regex, NormalizedCompilerStatus> <>f__mg$cache0;
        private static Regex sCompilerOutput = new Regex(@"\s*(?<filename>.*)\((?<line>\d+),(?<column>\d+)\):\s*[BU]C(?<type>W|E)(?<id>[^:]*):\s*(?<message>.*)", RegexOptions.ExplicitCapture);
        private static Regex sUnknownTypeOrNamespace = new Regex("[^']*'(?<type_name>[^']+)'.*", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        protected override string GetErrorIdentifier()
        {
            return "E";
        }

        protected override Regex GetOutputRegex()
        {
            return sCompilerOutput;
        }

        protected override NormalizedCompilerStatus NormalizedStatusFor(Match match)
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new Func<string, Regex, NormalizedCompilerStatus>(null, (IntPtr) CompilerOutputParserBase.NormalizeSimpleUnknownTypeOfNamespaceError);
            }
            return CompilerOutputParserBase.TryNormalizeCompilerStatus(match, "0018", sUnknownTypeOrNamespace, <>f__mg$cache0);
        }
    }
}

