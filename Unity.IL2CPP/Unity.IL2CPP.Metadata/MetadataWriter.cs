using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.IL2CPP.Debugger;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP.Metadata
{
	public class MetadataWriter
	{
		public enum ArrayTerminator
		{
			None,
			Null
		}

		private readonly CppCodeWriter _writer;

		private readonly IDebuggerSupport _debuggerSupport;

		[Inject]
		public static IIl2CppTypeCollectorWriterService Il2CppTypeCollectorWriter;

		[Inject]
		public static INamingService Naming;

		protected IDebuggerSupport DebuggerSupport
		{
			get
			{
				return this._debuggerSupport;
			}
		}

		protected CppCodeWriter Writer
		{
			get
			{
				return this._writer;
			}
		}

		protected MetadataWriter(CppCodeWriter writer)
		{
			this._writer = writer;
			this._debuggerSupport = DebuggerSupportFactory.GetDebuggerSupport();
		}

		protected void WriteLine(string line)
		{
			this._writer.WriteLine(line);
		}

		protected void WriteLine(string format, params object[] args)
		{
			this._writer.WriteLine(format, args);
		}

		protected void Write(string format)
		{
			this._writer.Write(format);
		}

		protected void Write(string format, params object[] args)
		{
			this._writer.Write(format, args);
		}

		protected void BeginBlock()
		{
			this._writer.BeginBlock();
		}

		protected void EndBlock(bool semicolon)
		{
			this._writer.EndBlock(semicolon);
		}

		internal static TableInfo WriteTable<T>(CppCodeWriter writer, string type, string name, ICollection<T> items, Func<T, string> map)
		{
			TableInfo result;
			if (items.Count == 0)
			{
				result = TableInfo.Empty;
			}
			else
			{
				writer.WriteArrayInitializer(type, name, items.Select(map), false);
				result = new TableInfo(items.Count, type, name);
			}
			return result;
		}

		internal static TableInfo WriteTable<T>(CppCodeWriter writer, string type, string name, ICollection<T> items, Func<T, int, string> map)
		{
			TableInfo result;
			if (items.Count == 0)
			{
				result = TableInfo.Empty;
			}
			else
			{
				writer.WriteArrayInitializer(type, name, items.Select(map), false);
				result = new TableInfo(items.Count, type, name);
			}
			return result;
		}

		protected void WriteArrayInitializer(IEnumerable<string> initializers, MetadataWriter.ArrayTerminator terminator = MetadataWriter.ArrayTerminator.None)
		{
			this.BeginBlock();
			foreach (string current in initializers)
			{
				this.WriteLine("{0},", new object[]
				{
					current
				});
			}
			if (terminator == MetadataWriter.ArrayTerminator.Null)
			{
				this.WriteLine(MetadataWriter.Naming.Null);
			}
			this.EndBlock(true);
		}

		protected void WriteFieldInitializer(FieldInitializers initializers)
		{
			this.BeginBlock();
			for (int i = 0; i < initializers.Count; i++)
			{
				FieldInitializer fieldInitializer = initializers[i];
				object obj = fieldInitializer.Value;
				if (obj is bool)
				{
					obj = ((!(bool)obj) ? "false" : "true");
				}
				if (i > 0)
				{
					this.Write(", ");
				}
				this.Write(obj.ToString());
				if (CodeGenOptions.EmitComments)
				{
					this.Write(Formatter.Comment(fieldInitializer.Name));
				}
				this.Writer.WriteLine();
			}
			this.Writer.WriteLine();
			this.EndBlock(true);
		}

		internal static string TypeRepositoryTypeFor(TypeReference type, int attrs = 0)
		{
			MetadataWriter.Il2CppTypeCollectorWriter.Add(type, attrs);
			return MetadataWriter.Naming.AddressOf(MetadataWriter.Naming.ForIl2CppType(type, attrs));
		}

		protected string Quote(object val)
		{
			return string.Format("\"{0}\"", val);
		}
	}
}
