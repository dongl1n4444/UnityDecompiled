namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.Marshaling;

    public class HandleRefMarshalInfoWriter : MarshalableMarshalInfoWriter
    {
        private readonly bool _forByReferenceType;
        private readonly MarshaledType[] _marshaledTypes;
        private readonly TypeDefinition _typeDefinition;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache2;

        public HandleRefMarshalInfoWriter(TypeReference type, bool forByReferenceType) : base(type)
        {
            this._typeDefinition = type.Resolve();
            this._forByReferenceType = forByReferenceType;
            this._marshaledTypes = new MarshaledType[] { new MarshaledType("void*", "void*") };
        }

        public override bool CanMarshalTypeFromNative() => 
            false;

        public override bool CanMarshalTypeToNative() => 
            !this._forByReferenceType;

        public override string GetMarshalingException() => 
            string.Format("il2cpp_codegen_get_marshal_directive_exception(\"HandleRefs cannot be marshaled ByRef or from unmanaged to managed.\")", base._typeRef);

        public override void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
        {
        }

        public override string WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            throw new InvalidOperationException("Cannot marshal HandleRef from native code");
        }

        public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            throw new InvalidOperationException("Cannot marshal HandleRef from native code");
        }

        public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            if (!this.CanMarshalTypeToNative())
            {
                throw new InvalidOperationException("Cannot marshal HandleRef by reference to native code.");
            }
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<FieldDefinition, bool>(null, (IntPtr) <WriteMarshalVariableToNative>m__0);
            }
            FieldDefinition field = this._typeDefinition.Fields.SingleOrDefault<FieldDefinition>(<>f__am$cache0);
            if ((field == null) && (CodeGenOptions.Dotnetprofile == DotNetProfile.Net45))
            {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = new Func<FieldDefinition, bool>(null, (IntPtr) <WriteMarshalVariableToNative>m__1);
                }
                field = this._typeDefinition.Fields.SingleOrDefault<FieldDefinition>(<>f__am$cache1);
            }
            if (field == null)
            {
                throw new InvalidOperationException($"Unable to locate the handle field on {this._typeDefinition}");
            }
            object[] args = new object[4];
            args[0] = destinationVariable;
            args[1] = sourceVariable.Load();
            args[2] = DefaultMarshalInfoWriter.Naming.ForFieldGetter(field);
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<FieldDefinition, bool>(null, (IntPtr) <WriteMarshalVariableToNative>m__2);
            }
            args[3] = DefaultMarshalInfoWriter.Naming.ForFieldGetter(DefaultMarshalInfoWriter.TypeProvider.SystemIntPtr.Fields.Single<FieldDefinition>(<>f__am$cache2));
            writer.WriteLine("{0} = {1}.{2}().{3}();", args);
        }

        public override MarshaledType[] MarshaledTypes =>
            this._marshaledTypes;
    }
}

