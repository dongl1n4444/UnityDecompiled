namespace Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative.WindowsRuntimeProjection
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.GenericSharing;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Metadata;

    internal sealed class IReadOnlyCollectionGetCountMethodBodyWriter
    {
        private readonly MethodDefinition _iMapViewGetSizeMethod;
        private readonly string[] _iMapViewGetSizeMethodArguments;
        private readonly TypeDefinition _iReadOnlyCollectionType;
        private readonly MethodDefinition _iVectorViewGetSizeMethod;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache0;
        private const string kIReadOnlyDictionaryClassLocalVariableName = "iReadOnlyDictionaryClass";
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static IRuntimeImplementedMethodAdder RuntimeImplementedMethods;
        [Inject]
        public static ITypeProviderService TypeProvider;

        public IReadOnlyCollectionGetCountMethodBodyWriter(TypeDefinition iReadOnlyCollectionType, MethodDefinition iMapViewGetSizeMethod, MethodDefinition iVectorViewGetSizeMethod)
        {
            this._iReadOnlyCollectionType = iReadOnlyCollectionType;
            this._iVectorViewGetSizeMethod = iVectorViewGetSizeMethod;
            this._iMapViewGetSizeMethod = iMapViewGetSizeMethod;
            int num = new VTableBuilder().IndexFor(this._iMapViewGetSizeMethod);
            string fullName = this._iMapViewGetSizeMethod.FullName;
            string[] textArray1 = new string[3];
            object[] objArray1 = new object[] { num, " /* ", fullName, " */" };
            textArray1[0] = string.Concat(objArray1);
            textArray1[1] = "iReadOnlyDictionaryClass";
            textArray1[2] = Naming.ThisParameterName;
            this._iMapViewGetSizeMethodArguments = textArray1;
        }

        private IEnumerable<RuntimeGenericTypeData> GetIReadOnlyDictionarySizeMethodGenericSharingData() => 
            new RuntimeGenericTypeData[] { new RuntimeGenericTypeData(RuntimeGenericContextInfo.Class, this._iReadOnlyCollectionType.GenericParameters[0]) };

        public void WriteGetCount(MethodDefinition method)
        {
            GenericInstanceType typeReference = null;
            MethodReference reference = null;
            MethodReference reference2 = null;
            if (this._iVectorViewGetSizeMethod != null)
            {
                typeReference = new GenericInstanceType(this._iVectorViewGetSizeMethod.DeclaringType) {
                    GenericArguments = { method.DeclaringType.GenericParameters[0] }
                };
                reference = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(typeReference).Resolve(this._iVectorViewGetSizeMethod);
            }
            if (this._iMapViewGetSizeMethod != null)
            {
                MethodDefinition item = new MethodDefinition("GetIReadOnlyDictionarySize", MethodAttributes.CompilerControlled | MethodAttributes.Private, TypeProvider.Int32TypeReference) {
                    ImplAttributes = MethodImplAttributes.CodeTypeMask
                };
                method.DeclaringType.Methods.Add(item);
                RuntimeImplementedMethods.RegisterMethod(item, new GetGenericSharingDataDelegate(this.GetIReadOnlyDictionarySizeMethodGenericSharingData), new WriteRuntimeImplementedMethodBodyDelegate(this.WriteGetIReadOnlyDictionarySizeMethodBody));
                GenericInstanceType type2 = new GenericInstanceType(method.DeclaringType);
                foreach (GenericParameter parameter in method.DeclaringType.GenericParameters)
                {
                    type2.GenericArguments.Add(parameter);
                }
                reference2 = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(type2).Resolve(item);
            }
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = m => ((m.HasThis && m.IsConstructor) && (m.Parameters.Count == 1)) && (m.Parameters[0].ParameterType.MetadataType == MetadataType.String);
            }
            MethodDefinition definition3 = TypeProvider.OptionalResolve("System", "InvalidOperationException", TypeProvider.Corlib.Name).Methods.Single<MethodDefinition>(<>f__am$cache0);
            method.Body.Variables.Add(new VariableDefinition(TypeProvider.Int32TypeReference));
            ILProcessor iLProcessor = method.Body.GetILProcessor();
            Instruction target = iLProcessor.Create(OpCodes.Ldstr, "Collection backing list is too large.");
            Instruction instruction2 = iLProcessor.Create(OpCodes.Ldloc_0);
            if ((this._iVectorViewGetSizeMethod != null) && (this._iMapViewGetSizeMethod != null))
            {
                Instruction instruction3 = iLProcessor.Create(OpCodes.Ldarg_0);
                iLProcessor.Emit(OpCodes.Ldarg_0);
                iLProcessor.Emit(OpCodes.Isinst, typeReference);
                iLProcessor.Emit(OpCodes.Brfalse, instruction3);
                iLProcessor.Emit(OpCodes.Ldarg_0);
                iLProcessor.Emit(OpCodes.Callvirt, reference);
                iLProcessor.Emit(OpCodes.Stloc_0);
                iLProcessor.Emit(OpCodes.Br, instruction2);
                iLProcessor.Append(instruction3);
                iLProcessor.Emit(OpCodes.Call, reference2);
                iLProcessor.Emit(OpCodes.Stloc_0);
            }
            else if (this._iVectorViewGetSizeMethod != null)
            {
                iLProcessor.Emit(OpCodes.Ldarg_0);
                iLProcessor.Emit(OpCodes.Callvirt, reference);
                iLProcessor.Emit(OpCodes.Stloc_0);
            }
            else
            {
                iLProcessor.Emit(OpCodes.Ldarg_0);
                iLProcessor.Emit(OpCodes.Call, reference2);
                iLProcessor.Emit(OpCodes.Stloc_0);
            }
            iLProcessor.Append(instruction2);
            iLProcessor.Emit(OpCodes.Ldc_I4, 0x7fffffff);
            iLProcessor.Emit(OpCodes.Bge_Un, target);
            iLProcessor.Emit(OpCodes.Ldloc_0);
            iLProcessor.Emit(OpCodes.Ret);
            iLProcessor.Append(target);
            iLProcessor.Emit(OpCodes.Newobj, definition3);
            iLProcessor.Emit(OpCodes.Throw);
        }

        private void WriteGetIReadOnlyDictionarySizeMethodBody(CppCodeWriter writer, MethodReference method, IRuntimeMetadataAccess metadataAccess)
        {
            TypeDefinition definition = TypeProvider.OptionalResolve("System.Collections.Generic", "KeyValuePair`2", TypeProvider.Corlib.Name);
            GenericInstanceType declaringType = (GenericInstanceType) method.DeclaringType;
            TypeReference reference = declaringType.GenericArguments[0];
            if ((definition == null) || (definition != reference.Resolve()))
            {
                writer.WriteStatement(Emit.RaiseManagedException("il2cpp_codegen_get_invalid_cast_exception(\"\")"));
            }
            else
            {
                string str = metadataAccess.UnresolvedTypeInfoFor(this._iMapViewGetSizeMethod.DeclaringType);
                writer.WriteLine($"Il2CppClass* keyValuePairClass = {metadataAccess.TypeInfoFor(this._iReadOnlyCollectionType.GenericParameters[0])};");
                writer.WriteLine($"Il2CppClass* {"iReadOnlyDictionaryClass"} = InitializedTypeInfo(il2cpp_codegen_inflate_generic_class({str}, il2cpp_codegen_get_generic_class_inst(keyValuePairClass)));");
                writer.WriteLine($"return {Emit.Call(Emit.VirtualCallInvokeMethod(this._iMapViewGetSizeMethod, Unity.IL2CPP.ILPreProcessor.TypeResolver.Empty), this._iMapViewGetSizeMethodArguments)};");
            }
        }
    }
}

