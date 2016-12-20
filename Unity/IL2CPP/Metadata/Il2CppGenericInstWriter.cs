namespace Unity.IL2CPP.Metadata
{
    using Mono.Cecil;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class Il2CppGenericInstWriter : MetadataWriter
    {
        [CompilerGenerated]
        private static Func<KeyValuePair<TypeReference[], uint>, TypeReference[]> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<TypeReference, string> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<KeyValuePair<TypeReference[], uint>, uint> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<KeyValuePair<TypeReference[], uint>, string> <>f__am$cache3;
        [Inject]
        public static IIl2CppGenericInstCollectorReaderService Il2CppGenericInstCollector;
        public const string TableVariableName = "g_Il2CppGenericInstTable";
        [Inject]
        public static IIl2CppTypeCollectorWriterService TypeCollector;

        public Il2CppGenericInstWriter(CppCodeWriter writer) : base(writer)
        {
        }

        public TableInfo WriteIl2CppGenericInstDefinitions()
        {
            base.Writer.AddCodeGenIncludes();
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<KeyValuePair<TypeReference[], uint>, TypeReference[]>(null, <WriteIl2CppGenericInstDefinitions>m__0);
            }
            foreach (TypeReference[] referenceArray in Enumerable.Select<KeyValuePair<TypeReference[], uint>, TypeReference[]>(Il2CppGenericInstCollector.Items, <>f__am$cache0))
            {
                for (int i = 0; i < referenceArray.Length; i++)
                {
                    base.Writer.WriteExternForIl2CppType(referenceArray[i]);
                }
                object[] args = new object[2];
                args[0] = MetadataWriter.Naming.ForGenericInst(referenceArray) + "_Types";
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = new Func<TypeReference, string>(null, (IntPtr) <WriteIl2CppGenericInstDefinitions>m__1);
                }
                args[1] = EnumerableExtensions.AggregateWithComma(Enumerable.Select<TypeReference, string>(referenceArray, <>f__am$cache1));
                base.WriteLine("static const Il2CppType* {0}[] = {{ {1} }};", args);
                object[] objArray2 = new object[] { MetadataWriter.Naming.ForGenericInst(referenceArray), referenceArray.Length, MetadataWriter.Naming.ForGenericInst(referenceArray) + "_Types" };
                base.WriteLine("extern const Il2CppGenericInst {0} = {{ {1}, {2} }};", objArray2);
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<KeyValuePair<TypeReference[], uint>, uint>(null, (IntPtr) <WriteIl2CppGenericInstDefinitions>m__2);
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = new Func<KeyValuePair<TypeReference[], uint>, string>(null, (IntPtr) <WriteIl2CppGenericInstDefinitions>m__3);
            }
            return MetadataWriter.WriteTable<KeyValuePair<TypeReference[], uint>>(base.Writer, "extern const Il2CppGenericInst* const", "g_Il2CppGenericInstTable", Enumerable.ToArray<KeyValuePair<TypeReference[], uint>>(Enumerable.OrderBy<KeyValuePair<TypeReference[], uint>, uint>(Il2CppGenericInstCollector.Items, <>f__am$cache2)), <>f__am$cache3);
        }
    }
}

