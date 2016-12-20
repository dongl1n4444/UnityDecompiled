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

        public virtual bool CanMarshalTypeFromNative()
        {
            return this.CanMarshalTypeToNative();
        }

        public virtual bool CanMarshalTypeToNative()
        {
            return true;
        }

        protected static string CleanVariableName(string variableName)
        {
            foreach (char ch in CharactersToReplaceWithUnderscore)
            {
                variableName = variableName.Replace(ch, '_');
            }
            variableName = string.Concat(variableName.Split(CharactersToRemove, StringSplitOptions.None));
            return Naming.Clean(variableName);
        }

        public virtual string DecorateVariable(string unmarshaledParameterName, string marshaledVariableName)
        {
            return marshaledVariableName;
        }

        public virtual string GetMarshalingException()
        {
            throw new NotSupportedException(string.Format("Cannot retrieve marshaling exception for type ({0}) that can be marshaled.", new object[0]));
        }

        public virtual string GetMarshalingFromNativeException()
        {
            return this.GetMarshalingException();
        }

        public virtual bool TreatAsValueType()
        {
            return Extensions.IsValueType(this._typeRef);
        }

        public virtual string UndecorateVariable(string variableName)
        {
            return variableName;
        }

        public virtual void WriteDeclareAndAllocateObject(CppCodeWriter writer, string unmarshaledVariableName, string marshaledVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            writer.WriteVariable(this._typeRef, unmarshaledVariableName);
        }

        public virtual void WriteFieldDeclaration(CppCodeWriter writer, FieldReference field, [Optional, DefaultParameterValue(null)] string fieldNameSuffix)
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

        public virtual void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, [Optional, DefaultParameterValue(null)] string managedVariableName)
        {
        }

        public virtual void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
        {
            if (!Extensions.IsEnum(this._typeRef) && !Extensions.IsSystemObject(this._typeRef))
            {
                foreach (MarshaledType type in this.MarshaledTypes)
                {
                    writer.AddForwardDeclaration(string.Format("struct {0}", type.Name));
                }
            }
        }

        public virtual string WriteMarshalEmptyVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, IRuntimeMetadataAccess metadataAccess)
        {
            string name = string.Format("_{0}_empty", CleanVariableName(variableName));
            writer.WriteVariable(this._typeRef, name);
            return name;
        }

        public virtual string WriteMarshalEmptyVariableToNative(CppCodeWriter writer, ManagedMarshalValue variableName, IList<MarshaledParameter> methodParameters)
        {
            return variableName.Load();
        }

        public virtual void WriteMarshalFunctionDeclarations(CppCodeWriter writer)
        {
        }

        public virtual void WriteMarshalFunctionDefinitions(CppCodeWriter writer, IMethodCollector methodCollector)
        {
        }

        public virtual void WriteMarshalOutParameterFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            this.WriteMarshalVariableFromNative(writer, variableName, destinationVariable, methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess);
        }

        public virtual void WriteMarshalOutParameterToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IList<MarshaledParameter> methodParameters, IRuntimeMetadataAccess metadataAccess)
        {
        }

        public virtual string WriteMarshalReturnValueToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, IRuntimeMetadataAccess metadataAccess)
        {
            return this.WriteMarshalVariableToNative(writer, sourceVariable, null, metadataAccess);
        }

        public virtual string WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            return variableName;
        }

        public virtual void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            writer.WriteLine(destinationVariable.Store(variableName));
        }

        public virtual string WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            return sourceVariable.Load();
        }

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
                if (type.Name.EndsWith("*") || (type.Name == "Il2CppHString"))
                {
                    object[] args = new object[] { type.Name, variableName + type.VariableName };
                    writer.WriteLine("{0} {1} = NULL;", args);
                }
                else if ((this._typeRef.MetadataType == MetadataType.Class) && !Extensions.DerivesFromObject(this._typeRef))
                {
                    object[] objArray2 = new object[] { type.Name, variableName + type.VariableName };
                    writer.WriteLine("{0} {1} = {0}();", objArray2);
                }
                else if (Extensions.IsPrimitiveType(this._typeRef.MetadataType))
                {
                    object[] objArray3 = new object[] { type.Name, variableName + type.VariableName, CppCodeWriter.InitializerStringForPrimitiveType(this._typeRef.MetadataType) };
                    writer.WriteLine("{0} {1} = {2};", objArray3);
                }
                else if (Extensions.IsPrimitiveCppType(type.Name))
                {
                    object[] objArray4 = new object[] { type.Name, variableName + type.VariableName, CppCodeWriter.InitializerStringForPrimitiveCppType(type.Name) };
                    writer.WriteLine("{0} {1} = {2};", objArray4);
                }
                else
                {
                    object[] objArray5 = new object[] { type.Name, variableName + type.VariableName };
                    writer.WriteLine("{0} {1} = {{ }};", objArray5);
                }
            }
        }

        public virtual bool HasNativeStructDefinition
        {
            get
            {
                return false;
            }
        }

        public virtual string MarshalCleanupFunctionName
        {
            get
            {
                return Naming.Null;
            }
        }

        public abstract MarshaledType[] MarshaledTypes { get; }

        public virtual string MarshalFromNativeFunctionName
        {
            get
            {
                return Naming.Null;
            }
        }

        public virtual string MarshalToNativeFunctionName
        {
            get
            {
                return Naming.Null;
            }
        }

        public virtual string NativeSize
        {
            get
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = new Func<MarshaledType, string>(null, (IntPtr) <get_NativeSize>m__0);
                }
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = new Func<string, string, string>(null, (IntPtr) <get_NativeSize>m__1);
                }
                return Enumerable.Aggregate<string>(Enumerable.Select<MarshaledType, string>(this.MarshaledTypes, <>f__am$cache0), <>f__am$cache1);
            }
        }

        public virtual int NativeSizeWithoutPointers
        {
            get
            {
                return 0;
            }
        }
    }
}

