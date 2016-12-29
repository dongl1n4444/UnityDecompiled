namespace Unity.IL2CPP.Metadata
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Debugger;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class MetadataWriter
    {
        private readonly IDebuggerSupport _debuggerSupport;
        private readonly CppCodeWriter _writer;
        [Inject]
        public static IIl2CppTypeCollectorWriterService Il2CppTypeCollectorWriter;
        [Inject]
        public static INamingService Naming;

        protected MetadataWriter(CppCodeWriter writer)
        {
            this._writer = writer;
            this._debuggerSupport = DebuggerSupportFactory.GetDebuggerSupport();
        }

        protected void BeginBlock()
        {
            this._writer.BeginBlock();
        }

        protected void EndBlock(bool semicolon)
        {
            this._writer.EndBlock(semicolon);
        }

        protected string Quote(object val) => 
            $""{val}"";

        internal static string TypeRepositoryTypeFor(TypeReference type, int attrs = 0)
        {
            Il2CppTypeCollectorWriter.Add(type, attrs);
            return Naming.AddressOf(Naming.ForIl2CppType(type, attrs));
        }

        protected void Write(string format)
        {
            this._writer.Write(format);
        }

        protected void Write(string format, params object[] args)
        {
            this._writer.Write(format, args);
        }

        protected void WriteArrayInitializer(IEnumerable<string> initializers, ArrayTerminator terminator = 0)
        {
            this.BeginBlock();
            foreach (string str in initializers)
            {
                object[] args = new object[] { str };
                this.WriteLine("{0},", args);
            }
            if (terminator == ArrayTerminator.Null)
            {
                this.WriteLine(Naming.Null);
            }
            this.EndBlock(true);
        }

        protected void WriteFieldInitializer(FieldInitializers initializers)
        {
            this.BeginBlock();
            for (int i = 0; i < initializers.Count; i++)
            {
                FieldInitializer initializer = initializers[i];
                object obj2 = initializer.Value;
                if (obj2 is bool)
                {
                    obj2 = !((bool) obj2) ? "false" : "true";
                }
                if (i > 0)
                {
                    this.Write(", ");
                }
                this.Write(obj2.ToString());
                if (CodeGenOptions.EmitComments)
                {
                    this.Write(Formatter.Comment(initializer.Name));
                }
                this.Writer.WriteLine();
            }
            this.Writer.WriteLine();
            this.EndBlock(true);
        }

        protected void WriteLine(string line)
        {
            this._writer.WriteLine(line);
        }

        protected void WriteLine(string format, params object[] args)
        {
            this._writer.WriteLine(format, args);
        }

        internal static TableInfo WriteTable<T>(CppCodeWriter writer, string type, string name, ICollection<T> items, Func<T, string> map)
        {
            if (items.Count == 0)
            {
                return TableInfo.Empty;
            }
            writer.WriteArrayInitializer(type, name, items.Select<T, string>(map), false);
            return new TableInfo(items.Count, type, name);
        }

        internal static TableInfo WriteTable<T>(CppCodeWriter writer, string type, string name, ICollection<T> items, Func<T, int, string> map)
        {
            if (items.Count == 0)
            {
                return TableInfo.Empty;
            }
            writer.WriteArrayInitializer(type, name, items.Select<T, string>(map), false);
            return new TableInfo(items.Count, type, name);
        }

        protected IDebuggerSupport DebuggerSupport =>
            this._debuggerSupport;

        protected CppCodeWriter Writer =>
            this._writer;

        public enum ArrayTerminator
        {
            None,
            Null
        }
    }
}

