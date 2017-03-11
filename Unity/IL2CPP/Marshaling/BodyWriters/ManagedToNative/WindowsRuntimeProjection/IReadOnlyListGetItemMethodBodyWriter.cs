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

    internal sealed class IReadOnlyListGetItemMethodBodyWriter
    {
        private readonly MethodDefinition _getAtMethod;
        [CompilerGenerated]
        private static Func<PropertyDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache1;
        [Inject]
        public static ITypeProviderService TypeProvider;

        public IReadOnlyListGetItemMethodBodyWriter(MethodDefinition getAtMethod)
        {
            this._getAtMethod = getAtMethod;
        }

        public void WriteGetItem(MethodDefinition method)
        {
            GenericInstanceType typeReference = new GenericInstanceType(this._getAtMethod.DeclaringType) {
                GenericArguments = { method.DeclaringType.GenericParameters[0] }
            };
            MethodReference reference = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(typeReference).Resolve(this._getAtMethod);
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = p => p.Name == "HResult";
            }
            PropertyDefinition definition = TypeProvider.SystemException.Properties.Single<PropertyDefinition>(<>f__am$cache0);
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = m => ((m.HasThis && m.IsConstructor) && (m.Parameters.Count == 1)) && (m.Parameters[0].ParameterType.MetadataType == MetadataType.String);
            }
            MethodDefinition definition3 = TypeProvider.OptionalResolve("System", "ArgumentOutOfRangeException", TypeProvider.Corlib.Name).Methods.Single<MethodDefinition>(<>f__am$cache1);
            ILProcessor iLProcessor = method.Body.GetILProcessor();
            Instruction target = iLProcessor.Create(OpCodes.Ldstr, "index");
            iLProcessor.Emit(OpCodes.Ldarg_1);
            iLProcessor.Emit(OpCodes.Ldc_I4_0);
            iLProcessor.Emit(OpCodes.Blt, target);
            Instruction instruction = iLProcessor.Create(OpCodes.Ldarg_0);
            iLProcessor.Append(instruction);
            iLProcessor.Emit(OpCodes.Ldarg_1);
            iLProcessor.Emit(OpCodes.Callvirt, reference);
            iLProcessor.Emit(OpCodes.Ret);
            Instruction instruction3 = iLProcessor.Create(OpCodes.Call, definition.GetMethod);
            iLProcessor.Append(instruction3);
            iLProcessor.Emit(OpCodes.Ldc_I4, -2147483637);
            iLProcessor.Emit(OpCodes.Beq, target);
            iLProcessor.Emit(OpCodes.Rethrow);
            iLProcessor.Append(target);
            iLProcessor.Emit(OpCodes.Newobj, definition3);
            iLProcessor.Emit(OpCodes.Dup);
            iLProcessor.Emit(OpCodes.Ldc_I4, -2147483637);
            iLProcessor.Emit(OpCodes.Call, definition.SetMethod);
            iLProcessor.Emit(OpCodes.Throw);
            ExceptionHandler item = new ExceptionHandler(ExceptionHandlerType.Catch) {
                TryStart = instruction,
                TryEnd = instruction3,
                HandlerStart = instruction3,
                HandlerEnd = target,
                CatchType = TypeProvider.SystemException
            };
            method.Body.ExceptionHandlers.Add(item);
        }
    }
}

