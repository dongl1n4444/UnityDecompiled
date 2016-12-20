using System;
using System.Runtime.CompilerServices;
using System.Text;

[Extension]
internal static class TemplateBuilderExtensions
{
    [Extension]
    internal static void AppendLineWithPrefix(StringBuilder stringBuilder, string format, params object[] args)
    {
        stringBuilder.AppendFormat("        {0}", string.Format(format, args));
        stringBuilder.AppendLine();
    }
}

