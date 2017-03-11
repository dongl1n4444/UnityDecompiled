namespace Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative.WindowsRuntimeProjection
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    internal sealed class IReadOnlyCollectionGetCountMethodBodyWriter
    {
        private readonly MethodDefinition _getSizeMethod;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache0;
        [Inject]
        public static ITypeProviderService TypeProvider;

        public IReadOnlyCollectionGetCountMethodBodyWriter(MethodDefinition getSizeMethod)
        {
            this._getSizeMethod = getSizeMethod;
        }

        public void WriteGetCount(MethodDefinition method)
        {
            GenericInstanceType typeReference = new GenericInstanceType(this._getSizeMethod.DeclaringType) {
                GenericArguments = { method.DeclaringType.GenericParameters[0] }
            };
            MethodReference reference = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(typeReference).Resolve(this._getSizeMethod);
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = m => ((m.HasThis && m.IsConstructor) && (m.Parameters.Count == 1)) && (m.Parameters[0].ParameterType.MetadataType == MetadataType.String);
            }
            MethodDefinition definition2 = TypeProvider.OptionalResolve("System", "InvalidOperationException", TypeProvider.Corlib.Name).Methods.Single<MethodDefinition>(<>f__am$cache0);
            method.Body.Variables.Add(new VariableDefinition(TypeProvider.Int32TypeReference));
            ILProcessor iLProcessor = method.Body.GetILProcessor();
            Instruction target = iLProcessor.Create(OpCodes.Ldstr, "Collection backing list is too large.");
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Callvirt, reference);
            iLProcessor.Emit(OpCodes.Stloc_0);
            iLProcessor.Emit(OpCodes.Ldloc_0);
            iLProcessor.Emit(OpCodes.Ldc_I4, 0x7fffffff);
            iLProcessor.Emit(OpCodes.Bge_Un, target);
            iLProcessor.Emit(OpCodes.Ldloc_0);
            iLProcessor.Emit(OpCodes.Ret);
            iLProcessor.Append(target);
            iLProcessor.Emit(OpCodes.Newobj, definition2);
            iLProcessor.Emit(OpCodes.Throw);
        }
    }
}

