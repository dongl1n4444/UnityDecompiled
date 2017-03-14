namespace UnityEditor.iOS.Xcode
{
    using System;
    using System.IO;
    using System.Text;

    internal class JsonDocument
    {
        public string indentString = "  ";
        public JsonElementDict root = new JsonElementDict();

        private void AppendIndent(StringBuilder sb, int indent)
        {
            for (int i = 0; i < indent; i++)
            {
                sb.Append(this.indentString);
            }
        }

        private void WriteArray(StringBuilder sb, JsonElementArray el, int indent)
        {
            sb.Append("[");
            bool flag = false;
            foreach (JsonElement element in el.values)
            {
                if (flag)
                {
                    sb.Append(",");
                }
                sb.Append("\n");
                this.AppendIndent(sb, indent + 1);
                if (element is JsonElementString)
                {
                    this.WriteString(sb, element.AsString());
                }
                else if (element is JsonElementInteger)
                {
                    this.WriteInteger(sb, element.AsInteger());
                }
                else if (element is JsonElementBoolean)
                {
                    this.WriteBoolean(sb, element.AsBoolean());
                }
                else if (element is JsonElementDict)
                {
                    this.WriteDict(sb, element.AsDict(), indent + 1);
                }
                else if (element is JsonElementArray)
                {
                    this.WriteArray(sb, element.AsArray(), indent + 1);
                }
                flag = true;
            }
            sb.Append("\n");
            this.AppendIndent(sb, indent);
            sb.Append("]");
        }

        private void WriteBoolean(StringBuilder sb, bool value)
        {
            sb.Append(!value ? "false" : "true");
        }

        private void WriteDict(StringBuilder sb, JsonElementDict el, int indent)
        {
            sb.Append("{");
            bool flag = false;
            foreach (string str in el.values.Keys)
            {
                if (flag)
                {
                    sb.Append(",");
                }
                this.WriteDictKeyValue(sb, str, el[str], indent + 1);
                flag = true;
            }
            sb.Append("\n");
            this.AppendIndent(sb, indent);
            sb.Append("}");
        }

        private void WriteDictKeyValue(StringBuilder sb, string key, JsonElement value, int indent)
        {
            sb.Append("\n");
            this.AppendIndent(sb, indent);
            this.WriteString(sb, key);
            sb.Append(" : ");
            if (value is JsonElementString)
            {
                this.WriteString(sb, value.AsString());
            }
            else if (value is JsonElementInteger)
            {
                this.WriteInteger(sb, value.AsInteger());
            }
            else if (value is JsonElementBoolean)
            {
                this.WriteBoolean(sb, value.AsBoolean());
            }
            else if (value is JsonElementDict)
            {
                this.WriteDict(sb, value.AsDict(), indent);
            }
            else if (value is JsonElementArray)
            {
                this.WriteArray(sb, value.AsArray(), indent);
            }
        }

        private void WriteInteger(StringBuilder sb, int value)
        {
            sb.Append(value.ToString());
        }

        private void WriteString(StringBuilder sb, string str)
        {
            sb.Append('"');
            sb.Append(str);
            sb.Append('"');
        }

        public void WriteToFile(string path)
        {
            File.WriteAllText(path, this.WriteToString());
        }

        public void WriteToStream(TextWriter tw)
        {
            tw.Write(this.WriteToString());
        }

        public string WriteToString()
        {
            StringBuilder sb = new StringBuilder();
            this.WriteDict(sb, this.root, 0);
            return sb.ToString();
        }
    }
}

