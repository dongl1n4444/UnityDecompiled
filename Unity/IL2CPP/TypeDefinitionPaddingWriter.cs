namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class TypeDefinitionPaddingWriter : IDisposable
    {
        private readonly TypeDefinition _type;
        private readonly CppCodeWriter _writer;
        [Inject]
        public static INamingService Naming;

        public TypeDefinitionPaddingWriter(CppCodeWriter writer, TypeDefinition type)
        {
            this._type = type;
            this._writer = writer;
            this.WritePaddingStart();
        }

        public void Dispose()
        {
            this.WritePaddingEnd();
        }

        private bool NeedsPadding()
        {
            return (this._type.ClassSize > 0);
        }

        private void WritePaddingEnd()
        {
            if (this.NeedsPadding())
            {
                this._writer.EndBlock(true);
                object[] args = new object[] { Naming.ForPadding(this._type), this._type.ClassSize };
                this._writer.WriteLine("uint8_t {0}[{1}];", args);
                this._writer.EndBlock(true);
            }
        }

        private void WritePaddingStart()
        {
            if (this.NeedsPadding())
            {
                this._writer.WriteLine("union");
                this._writer.BeginBlock();
                this._writer.WriteLine("struct");
                this._writer.BeginBlock();
            }
        }
    }
}

