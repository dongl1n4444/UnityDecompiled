namespace Unity.IL2CPP.WindowsRuntime
{
    using Mono.Cecil;
    using System;
    using Unity.IL2CPP;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    internal sealed class EnumerableCCWWriter : EnumerableCCWWriterBase
    {
        [Inject]
        public static IWindowsRuntimeProjectionsInitializer WindowsRuntimeProjectionsInitializer;

        private EnumerableCCWWriter(TypeReference type) : base(type)
        {
        }

        public static void WriteMethodDefinitions(TypeReference type, CppCodeWriter writer)
        {
            bool flag = EnumeratorToBindableIteratorAdapterWriter.WriteDefinitions(writer);
            if (flag)
            {
                EnumerableCCWWriter writer2 = new EnumerableCCWWriter(type);
                flag = writer2.HasIEnumerable();
                if (flag)
                {
                    writer2.WriteMethodDefinitions(writer);
                }
            }
            WindowsRuntimeProjectionsInitializer.HasIEnumerableCCW = flag;
        }

        public static void WriteTypeDefinition(TypeReference type, CppCodeWriter writer)
        {
            EnumerableCCWWriter writer2 = new EnumerableCCWWriter(type);
            bool flag = writer2.HasIEnumerable();
            if (flag)
            {
                writer2.WriteTypeDefinition(writer);
            }
            WindowsRuntimeProjectionsInitializer.HasIEnumerableCCW = flag;
        }
    }
}

