namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;

    internal class BooCompilerOutputParser : CompilerOutputParserBase
    {
        [CompilerGenerated]
        private static Func<string, Regex, NormalizedCompilerStatus> <>f__mg$cache0;
        [CompilerGenerated]
        private static Func<string, Regex, NormalizedCompilerStatus> <>f__mg$cache1;
        private static Regex sCompilerOutput = new Regex(@"\s*(?<filename>.*)\((?<line>\d+),(?<column>\d+)\):\s*[BU]C(?<type>W|E)(?<id>[^:]*):\s*(?<message>.*)", RegexOptions.ExplicitCapture);
        private static Regex sMissingMember = new Regex("[^']*'(?<member_name>[^']+)'[^']+'(?<type_name>[^']+)'", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        private static Regex sUnknownTypeOrNamespace = new Regex("[^']*'(?<type_name>[^']+)'.*", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        protected override string GetErrorIdentifier() => 
            "E";

        protected override Regex GetOutputRegex() => 
            sCompilerOutput;

        protected override NormalizedCompilerStatus NormalizedStatusFor(Match match)
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new Func<string, Regex, NormalizedCompilerStatus>(CompilerOutputParserBase.NormalizeMemberNotFoundError);
            }
            NormalizedCompilerStatus status = CompilerOutputParserBase.TryNormalizeCompilerStatus(match, "0019", sMissingMember, <>f__mg$cache0);
            if (status.code != NormalizedCompilerStatusCode.NotNormalized)
            {
                return status;
            }
            if (<>f__mg$cache1 == null)
            {
                <>f__mg$cache1 = new Func<string, Regex, NormalizedCompilerStatus>(CompilerOutputParserBase.NormalizeSimpleUnknownTypeOfNamespaceError);
            }
            return CompilerOutputParserBase.TryNormalizeCompilerStatus(match, "0018", sUnknownTypeOrNamespace, <>f__mg$cache1);
        }
    }
}

