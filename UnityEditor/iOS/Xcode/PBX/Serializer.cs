namespace UnityEditor.iOS.Xcode.PBX
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal class Serializer
    {
        private static string k_Indent = "\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t";

        private static string GetIndent(int indent) => 
            k_Indent.Substring(0, indent);

        public static PBXElementArray ParseArrayAST(ArrayAST ast, TokenList tokens, string text)
        {
            PBXElementArray array = new PBXElementArray();
            foreach (ValueAST east in ast.values)
            {
                array.values.Add(ParseValueAST(east, tokens, text));
            }
            return array;
        }

        public static PBXElementString ParseIdentifierAST(IdentifierAST ast, TokenList tokens, string text)
        {
            Token token = tokens[ast.value];
            UnityEditor.iOS.Xcode.PBX.TokenType type = token.type;
            if (type != UnityEditor.iOS.Xcode.PBX.TokenType.String)
            {
                if (type != UnityEditor.iOS.Xcode.PBX.TokenType.QuotedString)
                {
                    throw new Exception("Internal parser error");
                }
            }
            else
            {
                return new PBXElementString(text.Substring(token.begin, token.end - token.begin));
            }
            return new PBXElementString(PBXStream.UnquoteString(text.Substring(token.begin, token.end - token.begin)));
        }

        public static PBXElementDict ParseTreeAST(TreeAST ast, TokenList tokens, string text)
        {
            PBXElementDict dict = new PBXElementDict();
            foreach (KeyValueAST east in ast.values)
            {
                PBXElementString str = ParseIdentifierAST(east.key, tokens, text);
                PBXElement element = ParseValueAST(east.value, tokens, text);
                dict[str.value] = element;
            }
            return dict;
        }

        public static PBXElement ParseValueAST(ValueAST ast, TokenList tokens, string text)
        {
            if (ast is TreeAST)
            {
                return ParseTreeAST((TreeAST) ast, tokens, text);
            }
            if (ast is ArrayAST)
            {
                return ParseArrayAST((ArrayAST) ast, tokens, text);
            }
            if (ast is IdentifierAST)
            {
                return ParseIdentifierAST((IdentifierAST) ast, tokens, text);
            }
            return null;
        }

        public static void WriteArray(StringBuilder sb, PBXElementArray el, int indent, bool compact, PropertyCommentChecker checker, GUIDToCommentMap comments)
        {
            sb.Append("(");
            foreach (PBXElement element in el.values)
            {
                if (!compact)
                {
                    sb.Append("\n");
                    sb.Append(GetIndent(indent + 1));
                }
                if (element is PBXElementString)
                {
                    WriteStringImpl(sb, element.AsString(), checker.CheckStringValueInArray(element.AsString()), comments);
                }
                else if (element is PBXElementDict)
                {
                    WriteDict(sb, element.AsDict(), indent + 1, compact, checker.NextLevel("*"), comments);
                }
                else if (element is PBXElementArray)
                {
                    WriteArray(sb, element.AsArray(), indent + 1, compact, checker.NextLevel("*"), comments);
                }
                sb.Append(",");
                if (compact)
                {
                    sb.Append(" ");
                }
            }
            if (!compact)
            {
                sb.Append("\n");
                sb.Append(GetIndent(indent));
            }
            sb.Append(")");
        }

        public static void WriteDict(StringBuilder sb, PBXElementDict el, int indent, bool compact, PropertyCommentChecker checker, GUIDToCommentMap comments)
        {
            sb.Append("{");
            if (el.Contains("isa"))
            {
                WriteDictKeyValue(sb, "isa", el["isa"], indent + 1, compact, checker, comments);
            }
            List<string> list = new List<string>(el.values.Keys);
            list.Sort(StringComparer.Ordinal);
            foreach (string str in list)
            {
                if (str != "isa")
                {
                    WriteDictKeyValue(sb, str, el[str], indent + 1, compact, checker, comments);
                }
            }
            if (!compact)
            {
                sb.Append("\n");
                sb.Append(GetIndent(indent));
            }
            sb.Append("}");
        }

        public static void WriteDictKeyValue(StringBuilder sb, string key, PBXElement value, int indent, bool compact, PropertyCommentChecker checker, GUIDToCommentMap comments)
        {
            if (!compact)
            {
                sb.Append("\n");
                sb.Append(GetIndent(indent));
            }
            WriteStringImpl(sb, key, checker.CheckKeyInDict(key), comments);
            sb.Append(" = ");
            if (value is PBXElementString)
            {
                WriteStringImpl(sb, value.AsString(), checker.CheckStringValueInDict(key, value.AsString()), comments);
            }
            else if (value is PBXElementDict)
            {
                WriteDict(sb, value.AsDict(), indent, compact, checker.NextLevel(key), comments);
            }
            else if (value is PBXElementArray)
            {
                WriteArray(sb, value.AsArray(), indent, compact, checker.NextLevel(key), comments);
            }
            sb.Append(";");
            if (compact)
            {
                sb.Append(" ");
            }
        }

        private static void WriteStringImpl(StringBuilder sb, string s, bool comment, GUIDToCommentMap comments)
        {
            if (comment)
            {
                comments.WriteStringBuilder(sb, s);
            }
            else
            {
                sb.Append(PBXStream.QuoteStringIfNeeded(s));
            }
        }
    }
}

