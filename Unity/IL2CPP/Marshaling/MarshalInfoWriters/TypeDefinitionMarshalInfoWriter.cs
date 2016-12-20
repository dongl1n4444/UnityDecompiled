namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
    using Mono.Cecil;
    using System;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Marshaling;

    internal sealed class TypeDefinitionMarshalInfoWriter : CustomMarshalInfoWriter
    {
        private readonly int _nativeSizeWithoutPointers;

        public TypeDefinitionMarshalInfoWriter(TypeDefinition type, MarshalType marshalType, bool forFieldMarshaling) : base(type, marshalType, forFieldMarshaling)
        {
            this._nativeSizeWithoutPointers = this.CalculateNativeSizeWithoutPointers();
        }

        internal int CalculateNativeSizeWithoutPointers()
        {
            int num = 0;
            foreach (DefaultMarshalInfoWriter writer in base.FieldMarshalInfoWriters)
            {
                num += writer.NativeSizeWithoutPointers;
            }
            return num;
        }

        protected override void WriteMarshalCleanupFunction(CppCodeWriter writer)
        {
            string uniqueIdentifier = string.Format("{0}_{1}_MarshalCleanupMethodDefinition", DefaultMarshalInfoWriter.Naming.ForType(base._type), MarshalingUtils.MarshalTypeToString(base._marshalType));
            MethodWriter.WriteMethodWithMetadataInitialization(writer, base._marshalCleanupFunctionDeclaration, base._marshalToNativeFunctionName, new Action<CppCodeWriter, MetadataUsage, MethodUsage>(this, (IntPtr) this.<WriteMarshalCleanupFunction>m__2), uniqueIdentifier);
        }

        protected override void WriteMarshalFromNativeMethodDefinition(CppCodeWriter writer)
        {
            string uniqueIdentifier = string.Format("{0}_{1}_FromNativeMethodDefinition", DefaultMarshalInfoWriter.Naming.ForType(base._type), MarshalingUtils.MarshalTypeToString(base._marshalType));
            MethodWriter.WriteMethodWithMetadataInitialization(writer, base._marshalFromNativeFunctionDeclaration, base._marshalFromNativeFunctionName, new Action<CppCodeWriter, MetadataUsage, MethodUsage>(this, (IntPtr) this.<WriteMarshalFromNativeMethodDefinition>m__1), uniqueIdentifier);
        }

        protected override void WriteMarshalToNativeMethodDefinition(CppCodeWriter writer)
        {
            string uniqueIdentifier = string.Format("{0}_{1}_ToNativeMethodDefinition", DefaultMarshalInfoWriter.Naming.ForType(base._type), MarshalingUtils.MarshalTypeToString(base._marshalType));
            MethodWriter.WriteMethodWithMetadataInitialization(writer, base._marshalToNativeFunctionDeclaration, base._marshalToNativeFunctionName, new Action<CppCodeWriter, MetadataUsage, MethodUsage>(this, (IntPtr) this.<WriteMarshalToNativeMethodDefinition>m__0), uniqueIdentifier);
        }

        public override int NativeSizeWithoutPointers
        {
            get
            {
                return this._nativeSizeWithoutPointers;
            }
        }
    }
}

