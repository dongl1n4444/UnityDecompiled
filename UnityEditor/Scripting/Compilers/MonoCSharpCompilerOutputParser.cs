namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;

    internal class MonoCSharpCompilerOutputParser : CompilerOutputParserBase
    {
        [CompilerGenerated]
        private static Func<Match, Regex, NormalizedCompilerStatus> <>f__mg$cache0;
        [CompilerGenerated]
        private static Func<Match, Regex, NormalizedCompilerStatus> <>f__mg$cache1;
        [CompilerGenerated]
        private static Func<Match, Regex, NormalizedCompilerStatus> <>f__mg$cache2;
        [CompilerGenerated]
        private static Func<Match, Regex, NormalizedCompilerStatus> <>f__mg$cache3;
        private static Regex sCompilerOutput = new Regex(@"\s*(?<filename>.*)\((?<line>\d+),(?<column>\d+)\):\s*(?<type>warning|error)\s*(?<id>[^:]*):\s*(?<message>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        private static Regex sInternalErrorCompilerOutput = new Regex(@"\s*(?<message>Internal compiler (?<type>error)) at\s*(?<filename>.*)\((?<line>\d+),(?<column>\d+)\):\s*(?<id>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        private static Regex sMissingMember = new Regex("[^`]*`(?<type_name>[^']+)'[^`]+`(?<member_name>[^']+)'", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        private static Regex sMissingType = new Regex("[^`]*`(?<type_name>[^']+)'[^`]+`(?<namespace>[^']+)'", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        private static Regex sUnknownTypeOrNamespace = new Regex("[^`]*`(?<type_name>[^']+)'.*", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        protected override string GetErrorIdentifier() => 
            "error";

        protected override Regex GetInternalErrorOutputRegex() => 
            sInternalErrorCompilerOutput;

        protected override Regex GetOutputRegex() => 
            sCompilerOutput;

        protected override NormalizedCompilerStatus NormalizedStatusFor(Match match)
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new Func<Match, Regex, NormalizedCompilerStatus>(CompilerOutputParserBase.NormalizeMemberNotFoundError);
            }
            NormalizedCompilerStatus status = CompilerOutputParserBase.TryNormalizeCompilerStatus(match, "CS0117", sMissingMember, <>f__mg$cache0);
            if (status.code != NormalizedCompilerStatusCode.NotNormalized)
            {
                return status;
            }
            if (<>f__mg$cache1 == null)
            {
                <>f__mg$cache1 = new Func<Match, Regex, NormalizedCompilerStatus>(CompilerOutputParserBase.NormalizeSimpleUnknownTypeOfNamespaceError);
            }
            status = CompilerOutputParserBase.TryNormalizeCompilerStatus(match, "CS0246", sUnknownTypeOrNamespace, <>f__mg$cache1);
            if (status.code != NormalizedCompilerStatusCode.NotNormalized)
            {
                return status;
            }
            if (<>f__mg$cache2 == null)
            {
                <>f__mg$cache2 = new Func<Match, Regex, NormalizedCompilerStatus>(CompilerOutputParserBase.NormalizeUnknownTypeMemberOfNamespaceError);
            }
            status = CompilerOutputParserBase.TryNormalizeCompilerStatus(match, "CS0234", sMissingType, <>f__mg$cache2);
            if (status.code != NormalizedCompilerStatusCode.NotNormalized)
            {
                return status;
            }
            if (<>f__mg$cache3 == null)
            {
                <>f__mg$cache3 = new Func<Match, Regex, NormalizedCompilerStatus>(CompilerOutputParserBase.NormalizeSimpleUnknownTypeOfNamespaceError);
            }
            return CompilerOutputParserBase.TryNormalizeCompilerStatus(match, "CS0103", sUnknownTypeOrNamespace, <>f__mg$cache3);
        }
    }
}

