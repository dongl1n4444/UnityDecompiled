namespace Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative
{
    using Mono.Cecil;
    using System;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Marshaling.BodyWriters;

    internal class DelegatePInvokeMethodBodyWriter : PInvokeMethodBodyWriter
    {
        public DelegatePInvokeMethodBodyWriter(MethodReference interopMethod) : base(interopMethod)
        {
        }

        public bool IsDelegatePInvokeWrapperNecessary()
        {
            if (base._methodDefinition.Name != "Invoke")
            {
                return false;
            }
            TypeDefinition declaringType = base._methodDefinition.DeclaringType;
            if (!declaringType.IsDelegate())
            {
                return false;
            }
            if (declaringType.HasGenericParameters)
            {
                return false;
            }
            if (base._methodDefinition.HasGenericParameters)
            {
                return false;
            }
            if (base._methodDefinition.ReturnType.IsGenericParameter)
            {
                return false;
            }
            if (!base._methodDefinition.IsRuntime)
            {
                return false;
            }
            return (base.FirstOrDefaultUnmarshalableMarshalInfoWriter() == null);
        }

        protected override void WriteMethodPrologue(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            string decoratedName = base.MarshaledReturnType.DecoratedName;
            object[] args = new object[] { decoratedName, InteropMethodInfo.Naming.ForPInvokeFunctionPointerTypedef(), base.FormatParametersForTypedef() };
            writer.WriteLine("typedef {0} (STDCALL *{1})({2});", args);
            object[] objArray2 = new object[] { InteropMethodInfo.Naming.ForPInvokeFunctionPointerTypedef(), InteropMethodInfo.Naming.ForPInvokeFunctionPointerVariable(), InteropMethodInfo.Naming.ThisParameterName };
            writer.WriteLine("{0} {1} = reinterpret_cast<{0}>(((Il2CppDelegate*){2})->method->methodPointer);", objArray2);
            writer.WriteLine();
        }
    }
}

