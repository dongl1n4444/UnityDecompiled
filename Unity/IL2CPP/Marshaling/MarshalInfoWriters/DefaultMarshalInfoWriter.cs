namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Marshaling;

    public abstract class DefaultMarshalInfoWriter
    {
        protected readonly TypeReference _typeRef;
        [CompilerGenerated]
        private static Func<MarshaledType, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<string, string, string> <>f__am$cache1;
        protected static char[] CharactersToRemove = new char[] { '*', '(', ')', '\0' };
        protected static char[] CharactersToReplaceWithUnderscore = new char[] { '.', '[', ']', '\0' };
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static ITypeProviderService TypeProvider;

        public DefaultMarshalInfoWriter(TypeReference type)
        {
            this._typeRef = type;
        }

        public virtual bool CanMarshalTypeFromNative() => 
            this.CanMarshalTypeToNative();

        public virtual bool CanMarshalTypeToNative() => 
            true;

        protected static string CleanVariableName(string variableName)
        {
            foreach (char ch in CharactersToReplaceWithUnderscore)
            {
                variableName = variableName.Replace(ch, '_');
            }
            variableName = string.Concat(variableName.Split(CharactersToRemove, StringSplitOptions.None));
            return Naming.Clean(variableName);
        }

        public virtual string DecorateVariable(string unmarshaledParameterName, string marshaledVariableName) => 
            marshaledVariableName;

        public virtual string GetDefaultCppValue(MarshaledType type)
        {
            if (type.Name.EndsWith("*") || (type.Name == "Il2CppHString"))
            {
                return Naming.Null;
            }
            if ((this._typeRef.MetadataType == MetadataType.Class) && !this._typeRef.DerivesFromObject())
            {
                return (type.Name + "()");
            }
            if (this._typeRef.MetadataType.IsPrimitiveType())
            {
                return CppCodeWriter.InitializerStringForPrimitiveType(this._typeRef.MetadataType);
            }
            if (type.Name.IsPrimitiveCppType())
            {
                return CppCodeWriter.InitializerStringForPrimitiveCppType(type.Name);
            }
            return "{}";
        }

        public virtual string GetMarshalingException()
        {
            throw new NotSupportedException($"Cannot retrieve marshaling exception for type ({this._typeRef}) that can be marshaled.");
        }

        public virtual string GetMarshalingFromNativeException() => 
            this.GetMarshalingException();

        public virtual bool TreatAsValueType() => 
            this._typeRef.IsValueType();

        public virtual string UndecorateVariable(string variableName) => 
            variableName;

        public virtual void WriteDeclareAndAllocateObject(CppCodeWriter writer, string unmarshaledVariableName, string marshaledVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            writer.WriteVariable(this._typeRef, unmarshaledVariableName);
        }

        public virtual void WriteFieldDeclaration(CppCodeWriter writer, FieldReference field, string fieldNameSuffix = null)
        {
            foreach (MarshaledType type in this.MarshaledTypes)
            {
                string str = Naming.ForField(field) + type.VariableName + fieldNameSuffix;
                object[] args = new object[] { type.DecoratedName, str };
                writer.WriteLine("{0} {1};", args);
            }
        }

        public virtual void WriteIncludesForFieldDeclaration(CppCodeWriter writer)
        {
            if (this.TreatAsValueType())
            {
                writer.AddIncludeForTypeDefinition(this._typeRef);
            }
            else
            {
                this.WriteMarshaledTypeForwardDeclaration(writer);
            }
        }

        public virtual void WriteIncludesForMarshaling(CppCodeWriter writer)
        {
            writer.AddIncludeForTypeDefinition(this._typeRef);
        }

        public virtual void WriteMarshalCleanupEmptyVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName)
        {
            this.WriteMarshalCleanupVariable(writer, variableName, metadataAccess, managedVariableName);
        }

        public virtual void WriteMarshalCleanupReturnValue(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess)
        {
            this.WriteMarshalCleanupVariable(writer, variableName, metadataAccess, null);
        }

        public virtual void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
        {
        }

        public virtual void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
        {
            if (!this._typeRef.IsEnum() && !this._typeRef.IsSystemObject())
            {
                foreach (MarshaledType type in this.MarshaledTypes)
                {
                    writer.AddForwardDeclaration($"struct {type.Name}");
                }
            }
        }

        public virtual string WriteMarshalEmptyVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, IRuntimeMetadataAccess metadataAccess)
        {
            string name = $"_{CleanVariableName(variableName)}_empty";
            writer.WriteVariable(this._typeRef, name);
            return name;
        }

        public virtual string WriteMarshalEmptyVariableToNative(CppCodeWriter writer, ManagedMarshalValue variableName, IList<MarshaledParameter> methodParameters) => 
            variableName.Load();

        public virtual void WriteMarshalFunctionDeclarations(CppCodeWriter writer)
        {
        }

        public virtual void WriteMarshalFunctionDefinitions(CppCodeWriter writer, IInteropDataCollector interopDataCollector)
        {
        }

        public virtual void WriteMarshalOutParameterFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            this.WriteMarshalVariableFromNative(writer, variableName, destinationVariable, methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess);
        }

        public virtual void WriteMarshalOutParameterToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IList<MarshaledParameter> methodParameters, IRuntimeMetadataAccess metadataAccess)
        {
        }

        public virtual string WriteMarshalReturnValueToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, IRuntimeMetadataAccess metadataAccess) => 
            this.WriteMarshalVariableToNative(writer, sourceVariable, null, metadataAccess);

        public virtual string WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess) => 
            variableName;

        public virtual void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            writer.WriteLine(destinationVariable.Store(variableName));
        }

        public virtual string WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess) => 
            sourceVariable.Load();

        public virtual void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            object[] args = new object[] { sourceVariable.Load(), destinationVariable };
            writer.WriteLine("{1} = {0};", args);
        }

        public virtual void WriteNativeStructDefinition(CppCodeWriter writer)
        {
        }

        public virtual void WriteNativeVariableDeclarationOfType(CppCodeWriter writer, string variableName)
        {
            foreach (MarshaledType type in this.MarshaledTypes)
            {
                writer.WriteStatement(Emit.Assign($"{type.Name} {variableName + type.VariableName}", this.GetDefaultCppValue(type)));
            }
        }

        public virtual bool HasNativeStructDefinition =>
            false;

        public virtual string MarshalCleanupFunctionName =>
            Naming.Null;

        public abstract MarshaledType[] MarshaledTypes { get; }

        public virtual string MarshalFromNativeFunctionName =>
            Naming.Null;

        public virtual string MarshalToNativeFunctionName =>
            Naming.Null;

        public virtual string NativeSize
        {
            get
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = t => $"sizeof({t.Name})";
                }
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = (x, y) => x + " + " + y;
                }
                return this.MarshaledTypes.Select<MarshaledType, string>(<>f__am$cache0).Aggregate<string>(<>f__am$cache1);
            }
        }

        public virtual int NativeSizeWithoutPointers =>
            0;
    }
}

