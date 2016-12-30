namespace Unity.IL2CPP.WindowsRuntime
{
    using Mono.Cecil;
    using System;
    using Unity.IL2CPP;

    internal sealed class GenericEnumerableCCWWriter : EnumerableCCWWriterBase
    {
        private GenericEnumerableCCWWriter(TypeReference type) : base(type)
        {
        }

        public static void WriteMethodDefinitions(TypeReference type, CppCodeWriter writer)
        {
            GenericEnumeratorToIteratorAdapterWriter.WriteDefinitions(writer, (GenericInstanceType) type);
            new GenericEnumerableCCWWriter(type).WriteMethodDefinitions(writer);
        }

        public static void WriteTypeDefinition(TypeReference type, CppCodeWriter writer)
        {
            new GenericEnumerableCCWWriter(type).WriteTypeDefinition(writer);
        }
    }
}

