namespace Unity.IL2CPP
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class CodeWriter : IDisposable
    {
        private int _indent;
        private string _indentString = "";
        private readonly string[] _indentStrings;
        private bool _shouldIndent;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private StreamWriter <Writer>k__BackingField;

        public CodeWriter(StreamWriter writer)
        {
            this.Writer = writer;
            this._shouldIndent = true;
            this._indentStrings = new string[10];
            for (int i = 0; i < this._indentStrings.Length; i++)
            {
                this._indentStrings[i] = new string('\t', i);
            }
        }

        public void BeginBlock()
        {
            this.WriteLine("{");
            this.Indent(1);
        }

        public void BeginBlock(string comment)
        {
            this.Write("{ // ");
            this.WriteLine(comment);
            this.Indent(1);
        }

        public void Dedent(int count = 1)
        {
            if (count > this._indent)
            {
                throw new ArgumentException("Cannot dedent CppCodeWriter more than it was indented.", "count");
            }
            this._indent -= count;
            if (this._indent < this._indentStrings.Length)
            {
                this._indentString = this._indentStrings[this._indent];
            }
            else
            {
                this._indentString = new string('\t', this._indent);
            }
        }

        public virtual void Dispose()
        {
            this.Writer.Dispose();
        }

        public void EndBlock(bool semicolon = false)
        {
            this.Dedent(1);
            if (semicolon)
            {
                this.WriteLine("};");
            }
            else
            {
                this.WriteLine("}");
            }
        }

        public void EndBlock(string comment, bool semicolon = false)
        {
            this.Dedent(1);
            this.Write("}");
            if (semicolon)
            {
                this.Write(";");
            }
            this.Write(" // ");
            this.WriteLine(comment);
        }

        public void Indent(int count = 1)
        {
            this._indent += count;
            if (this._indent < this._indentStrings.Length)
            {
                this._indentString = this._indentStrings[this._indent];
            }
            else
            {
                this._indentString = new string('\t', this._indent);
            }
        }

        public void Write(string block)
        {
            this.WriteIndented(block);
        }

        public void Write(string block, params object[] args)
        {
            if (args.Length > 0)
            {
                block = string.Format(block, args);
            }
            this.Write(block);
        }

        public void WriteCommentedLine(string block)
        {
            if (CodeGenOptions.EmitComments)
            {
                char[] trimChars = new char[] { '\\' };
                block = block.TrimEnd(trimChars);
                object[] args = new object[] { block };
                this.WriteLine("// {0}", args);
            }
        }

        public void WriteCommentedLine(string format, params object[] parameters)
        {
            this.WriteCommentedLine(string.Format(format, parameters));
        }

        private void WriteIndented(string s)
        {
            if (this._shouldIndent)
            {
                this.Writer.Write(this._indentString);
                this._shouldIndent = false;
            }
            this.Writer.Write(s);
        }

        public void WriteLine()
        {
            this.Writer.Write("\n");
            this._shouldIndent = true;
        }

        public void WriteLine(string block)
        {
            this.WriteIndented(block);
            this.Writer.Write("\n");
            this._shouldIndent = true;
        }

        public void WriteLine(string block, params object[] args)
        {
            if (args.Length > 0)
            {
                block = string.Format(block, args);
            }
            this.WriteLine(block);
        }

        public void WriteStatement(string block)
        {
            this.WriteLine($"{block};");
        }

        public void WriteUnindented(string block, params object[] args)
        {
            if (args.Length > 0)
            {
                block = string.Format(block, args);
            }
            this.Writer.Write(block + "\n");
        }

        public int IndentationLevel =>
            this._indent;

        public StreamWriter Writer { get; private set; }
    }
}

