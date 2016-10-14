using System;
using System.IO;

namespace Unity.IL2CPP
{
	public abstract class CodeWriter : IDisposable
	{
		private int _indent;

		private bool _shouldIndent;

		private string _indentString = "";

		private readonly string[] _indentStrings;

		public StreamWriter Writer
		{
			get;
			private set;
		}

		public int IndentationLevel
		{
			get
			{
				return this._indent;
			}
		}

		protected CodeWriter(StreamWriter writer)
		{
			this.Writer = writer;
			this._shouldIndent = true;
			this._indentStrings = new string[10];
			for (int i = 0; i < this._indentStrings.Length; i++)
			{
				this._indentStrings[i] = new string('\t', i);
			}
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

		public void WriteCommentedLine(string block)
		{
			if (CodeGenOptions.EmitComments)
			{
				block = block.TrimEnd(new char[]
				{
					'\\'
				});
				this.WriteLine("// {0}", new object[]
				{
					block
				});
			}
		}

		public void WriteStatement(string block)
		{
			this.WriteLine(string.Format("{0};", block));
		}

		public void WriteLine(string block, params object[] args)
		{
			if (args.Length > 0)
			{
				block = string.Format(block, args);
			}
			this.WriteLine(block);
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

		public void WriteUnindented(string block, params object[] args)
		{
			if (args.Length > 0)
			{
				block = string.Format(block, args);
			}
			this.Writer.Write(block + "\n");
		}

		public void Indent()
		{
			this._indent++;
			if (this._indent < this._indentStrings.Length)
			{
				this._indentString = this._indentStrings[this._indent];
			}
			else
			{
				this._indentString = new string('\t', this._indent);
			}
		}

		public void Dedent()
		{
			this._indent--;
			if (this._indent < this._indentStrings.Length)
			{
				this._indentString = this._indentStrings[this._indent];
			}
			else
			{
				this._indentString = new string('\t', this._indent);
			}
		}

		public void BeginBlock()
		{
			this.WriteLine("{");
			this.Indent();
		}

		public void BeginBlock(string comment)
		{
			this.Write("{ // ");
			this.WriteLine(comment);
			this.Indent();
		}

		public void EndBlock(bool semicolon = false)
		{
			this.Dedent();
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
			this.Dedent();
			this.Write("}");
			if (semicolon)
			{
				this.Write(";");
			}
			this.Write(" // ");
			this.WriteLine(comment);
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

		public virtual void Dispose()
		{
			this.Writer.Dispose();
		}
	}
}
