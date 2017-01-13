namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.GenericsCollection;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Metadata;

    public sealed class MethodTableWriter
    {
        private readonly CppCodeWriter _writer;
        [CompilerGenerated]
        private static Func<KeyValuePair<string, int>, int> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<KeyValuePair<string, int>, string> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<MethodReference, bool> <>f__am$cache2;
        [Inject]
        public static IIl2CppGenericMethodCollectorReaderService GenericMethodsCollection;
        [Inject]
        public static INamingService Naming;

        public MethodTableWriter(CppCodeWriter writer)
        {
            this._writer = writer;
        }

        public TableInfo Write(InflatedCollectionCollector generics, MethodTables methodTables, IMethodCollectorResults methodCollector)
        {
            IncludeWriter.WriteRegistrationIncludes(this._writer);
            WriteIncludesFor(this._writer, generics);
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = value => value.Value;
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = m => string.Concat(new object[] { m.Key, "/* ", m.Value, "*/" });
            }
            string[] values = methodTables.MethodPointers.OrderBy<KeyValuePair<string, int>, int>(<>f__am$cache0).Select<KeyValuePair<string, int>, string>(<>f__am$cache1).ToArray<string>();
            this._writer.WriteArrayInitializer("extern const Il2CppMethodPointer", "g_Il2CppGenericMethodPointers", values, false);
            return new TableInfo(values.Length, "extern const Il2CppMethodPointer", "g_Il2CppGenericMethodPointers");
        }

        private static void WriteIncludesFor(CppCodeWriter writer, InflatedCollectionCollector generics)
        {
            HashSet<string> set = new HashSet<string>();
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = m => !m.HasGenericParameters && !m.ContainsGenericParameters();
            }
            foreach (MethodReference reference in GenericMethodsCollection.Items.Keys.Where<MethodReference>(<>f__am$cache2))
            {
                string item = MethodTables.MethodPointerNameFor(reference);
                if ((item != Naming.Null) && set.Add(item))
                {
                    object[] args = new object[] { item };
                    writer.WriteLine("extern \"C\" void {0} ();", args);
                }
            }
        }
    }
}

