namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Marshaling;

    public class SafeHandleMarshalInfoWriter : MarshalableMarshalInfoWriter
    {
        private readonly MethodDefinition _addRefMethod;
        private readonly MethodDefinition _defaultConstructor;
        private readonly MarshaledType[] _marshaledTypes;
        private readonly MethodDefinition _releaseMethod;
        private readonly TypeDefinition _safeHandleTypeDefinition;
        private static int _unsusedBoolNameCounter = 1;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache4;
        private const string SafeHandleReferenceIncrementedLocalBoolNamePrefix = "___safeHandle_reference_incremented_for";

        public SafeHandleMarshalInfoWriter(TypeReference type, TypeDefinition safeHandleTypeTypeDefinition) : base(type)
        {
            this._safeHandleTypeDefinition = safeHandleTypeTypeDefinition;
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<MethodDefinition, bool>(null, (IntPtr) <SafeHandleMarshalInfoWriter>m__0);
            }
            this._addRefMethod = this._safeHandleTypeDefinition.Methods.Single<MethodDefinition>(<>f__am$cache0);
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<MethodDefinition, bool>(null, (IntPtr) <SafeHandleMarshalInfoWriter>m__1);
            }
            this._releaseMethod = this._safeHandleTypeDefinition.Methods.Single<MethodDefinition>(<>f__am$cache1);
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<MethodDefinition, bool>(null, (IntPtr) <SafeHandleMarshalInfoWriter>m__2);
            }
            this._defaultConstructor = base._typeRef.Resolve().Methods.SingleOrDefault<MethodDefinition>(<>f__am$cache2);
            this._marshaledTypes = new MarshaledType[] { new MarshaledType("void*", "void*") };
        }

        [CompilerGenerated]
        private static bool <SafeHandleMarshalInfoWriter>m__0(MethodDefinition method) => 
            (method.Name == "DangerousAddRef");

        [CompilerGenerated]
        private static bool <SafeHandleMarshalInfoWriter>m__1(MethodDefinition method) => 
            (method.Name == "DangerousRelease");

        [CompilerGenerated]
        private static bool <SafeHandleMarshalInfoWriter>m__2(MethodDefinition ctor) => 
            ((ctor.Name == ".ctor") && (ctor.Parameters.Count == 0));

        private void EmitCallToDangerousAddRef(CppCodeWriter writer, string variableName, bool generateBoolName, IRuntimeMetadataAccess metadataAccess)
        {
            string str = !generateBoolName ? this.SafeHandleReferenceIncrementedLocalBoolName(variableName) : this.SafeHandleReferenceIncrementedLocalBoolName($"unused{_unsusedBoolNameCounter++}");
            object[] args = new object[] { str };
            writer.WriteLine("bool {0} = false;", args);
            object[] objArray2 = new object[1];
            string[] arguments = new string[] { variableName, DefaultMarshalInfoWriter.Naming.AddressOf(str), metadataAccess.HiddenMethodInfo(this._addRefMethod) };
            objArray2[0] = Emit.Call(DefaultMarshalInfoWriter.Naming.ForMethod(this._addRefMethod), arguments);
            writer.WriteLine("{0};", objArray2);
        }

        private FieldReference GetIntPtrValueField()
        {
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = new Func<FieldDefinition, bool>(null, (IntPtr) <GetIntPtrValueField>m__3);
            }
            return DefaultMarshalInfoWriter.TypeProvider.SystemIntPtr.Fields.Single<FieldDefinition>(<>f__am$cache3);
        }

        private FieldReference GetSafeHandleHandleField()
        {
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = new Func<FieldDefinition, bool>(null, (IntPtr) <GetSafeHandleHandleField>m__4);
            }
            return this._safeHandleTypeDefinition.Fields.Single<FieldDefinition>(<>f__am$cache4);
        }

        private string LoadHandleFieldFor(string sourceVariable) => 
            $"({sourceVariable})->{DefaultMarshalInfoWriter.Naming.ForFieldGetter(this.GetSafeHandleHandleField())}().{DefaultMarshalInfoWriter.Naming.ForFieldGetter(this.GetIntPtrValueField())}()";

        private string SafeHandleReferenceIncrementedLocalBoolName(string variableName) => 
            $"{"___safeHandle_reference_incremented_for"}_{DefaultMarshalInfoWriter.CleanVariableName(variableName)}";

        public override void WriteDeclareAndAllocateObject(CppCodeWriter writer, string unmarshaledVariableName, string marshaledVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            TypeDefinition definition = base._typeRef.Resolve();
            if (definition.IsAbstract)
            {
                writer.WriteStatement(Emit.RaiseManagedException($"il2cpp_codegen_get_marshal_directive_exception("A returned SafeHandle cannot be abstract, but this type is: '{definition.FullName}'.")"));
            }
            CustomMarshalInfoWriter.EmitNewObject(writer, base._typeRef, unmarshaledVariableName, marshaledVariableName, false, metadataAccess);
        }

        public override void WriteIncludesForMarshaling(CppCodeWriter writer)
        {
            writer.AddIncludeForMethodDeclarations(base._typeRef);
            writer.AddIncludeForMethodDeclarations(this._safeHandleTypeDefinition);
        }

        public override void WriteMarshalCleanupEmptyVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName)
        {
        }

        public override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
        {
            if (!string.IsNullOrEmpty(managedVariableName))
            {
                object[] args = new object[] { this.SafeHandleReferenceIncrementedLocalBoolName(managedVariableName) };
                writer.WriteLine("if ({0})", args);
                writer.BeginBlock();
                object[] objArray2 = new object[1];
                string[] arguments = new string[] { managedVariableName, metadataAccess.HiddenMethodInfo(this._releaseMethod) };
                objArray2[0] = Emit.Call(DefaultMarshalInfoWriter.Naming.ForMethod(this._releaseMethod), arguments);
                writer.WriteLine("{0};", objArray2);
                writer.EndBlock(false);
            }
        }

        public override void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
        {
        }

        public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            <WriteMarshalVariableFromNative>c__AnonStorey0 storey = new <WriteMarshalVariableFromNative>c__AnonStorey0 {
                destinationVariable = destinationVariable,
                writer = writer,
                variableName = variableName,
                $this = this
            };
            if (forNativeWrapperOfManagedMethod)
            {
                storey.writer.WriteStatement(Emit.RaiseManagedException("il2cpp_codegen_get_marshal_directive_exception(\"Cannot marshal a SafeHandle from unmanaged to managed.\")"));
            }
            else
            {
                Action writeMarshalFromNativeCode = new Action(storey, (IntPtr) this.<>m__0);
                CustomMarshalInfoWriter.EmitCallToConstructor(storey.writer, base._typeRef.Resolve(), this._defaultConstructor, storey.variableName, storey.destinationVariable, writeMarshalFromNativeCode, false, metadataAccess);
                if (!returnValue)
                {
                    this.EmitCallToDangerousAddRef(storey.writer, storey.destinationVariable.Load(), true, metadataAccess);
                }
            }
        }

        public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            object[] args = new object[] { sourceVariable.Load(), DefaultMarshalInfoWriter.Naming.Null, Emit.RaiseManagedException($"il2cpp_codegen_get_argument_null_exception("{!string.IsNullOrEmpty(managedVariableName) ? managedVariableName : sourceVariable.GetNiceName()}")") };
            writer.WriteLine("if ({0} == {1}) {2};", args);
            this.EmitCallToDangerousAddRef(writer, sourceVariable.Load(), false, metadataAccess);
            object[] objArray2 = new object[] { destinationVariable, this.LoadHandleFieldFor(sourceVariable.Load()) };
            writer.WriteLine("{0} = {1};", objArray2);
        }

        public override MarshaledType[] MarshaledTypes =>
            this._marshaledTypes;

        [CompilerGenerated]
        private sealed class <WriteMarshalVariableFromNative>c__AnonStorey0
        {
            internal SafeHandleMarshalInfoWriter $this;
            internal ManagedMarshalValue destinationVariable;
            internal string variableName;
            internal CppCodeWriter writer;

            internal void <>m__0()
            {
                string str = this.destinationVariable.GetNiceName() + "_handle_temp";
                object[] args = new object[] { DefaultMarshalInfoWriter.Naming.ForVariable(DefaultMarshalInfoWriter.TypeProvider.SystemIntPtr), str };
                this.writer.WriteLine("{0} {1};", args);
                object[] objArray2 = new object[] { str, DefaultMarshalInfoWriter.Naming.ForFieldSetter(this.$this.GetIntPtrValueField()), this.variableName };
                this.writer.WriteLine("{0}.{1}({2});", objArray2);
                object[] objArray3 = new object[] { this.destinationVariable.Load(), DefaultMarshalInfoWriter.Naming.ForFieldSetter(this.$this.GetSafeHandleHandleField()), str };
                this.writer.WriteLine("({0})->{1}({2});", objArray3);
            }
        }
    }
}

