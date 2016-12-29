using System;
using System.Runtime.CompilerServices;
using System.Text;

internal static class TemplateBuilderExtensions
{
    internal static void AppendLineWithPrefix(this StringBuilder stringBuilder, string format, params object[] args)
    {
        stringBuilder.AppendFormat("        {0}", string.Format(format, args));
        stringBuilder.AppendLine();
    }
}

