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
    using Unity.IL2CPP.WindowsRuntime;

    internal sealed class IReadOnlyDictionaryProjectedMethodBodyWriter
    {
        private readonly MethodDefinition _argumentNullExceptionConstructor;
        private readonly MethodDefinition _hresultGetter;
        private readonly Unity.IL2CPP.ILPreProcessor.TypeResolver _iMapViewInstanceTypeResolver;
        private readonly TypeDefinition _iMapViewTypeDef;
        private readonly Unity.IL2CPP.ILPreProcessor.TypeResolver _iReadOnlyDictionaryInstanceTypeResolver;
        private readonly TypeDefinition _iReadOnlyDictionaryTypeDef;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<PropertyDefinition, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache7;
        [Inject]
        public static ITypeProviderService TypeProvider;

        public IReadOnlyDictionaryProjectedMethodBodyWriter(TypeDefinition iReadOnlyDictionaryTypeDef, TypeDefinition iMapViewTypeDef)
        {
            this._iReadOnlyDictionaryTypeDef = iReadOnlyDictionaryTypeDef;
            this._iMapViewTypeDef = iMapViewTypeDef;
            GenericInstanceType typeReference = new GenericInstanceType(this._iReadOnlyDictionaryTypeDef) {
                GenericArguments = { 
                    this._iReadOnlyDictionaryTypeDef.GenericParameters[0],
                    this._iReadOnlyDictionaryTypeDef.GenericParameters[1]
                }
            };
            this._iReadOnlyDictionaryInstanceTypeResolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(typeReference);
            GenericInstanceType type2 = new GenericInstanceType(this._iMapViewTypeDef) {
                GenericArguments = { 
                    this._iReadOnlyDictionaryTypeDef.GenericParameters[0],
                    this._iReadOnlyDictionaryTypeDef.GenericParameters[1]
                }
            };
            this._iMapViewInstanceTypeResolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(type2);
            TypeDefinition definition = TypeProvider.OptionalResolve("System", "ArgumentNullException", TypeProvider.Corlib.Name);
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<MethodDefinition, bool>(IReadOnlyDictionaryProjectedMethodBodyWriter.<IReadOnlyDictionaryProjectedMethodBodyWriter>m__0);
            }
            this._argumentNullExceptionConstructor = definition.Methods.Single<MethodDefinition>(<>f__am$cache0);
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<PropertyDefinition, bool>(IReadOnlyDictionaryProjectedMethodBodyWriter.<IReadOnlyDictionaryProjectedMethodBodyWriter>m__1);
            }
            PropertyDefinition definition2 = TypeProvider.SystemException.Properties.Single<PropertyDefinition>(<>f__am$cache1);
            this._hresultGetter = definition2.GetMethod;
        }

        [CompilerGenerated]
        private static bool <IReadOnlyDictionaryProjectedMethodBodyWriter>m__0(MethodDefinition m) => 
            (((m.HasThis && m.IsConstructor) && (m.Parameters.Count == 1)) && (m.Parameters[0].ParameterType.MetadataType == MetadataType.String));

        [CompilerGenerated]
        private static bool <IReadOnlyDictionaryProjectedMethodBodyWriter>m__1(PropertyDefinition p) => 
            (p.Name == "HResult");

        public void WriteContainsKey(MethodDefinition method)
        {
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = m => m.Name == "HasKey";
            }
            MethodDefinition definition = this._iMapViewTypeDef.Methods.Single<MethodDefinition>(<>f__am$cache2);
            MethodReference reference = this._iMapViewInstanceTypeResolver.Resolve(definition);
            ILProcessor iLProcessor = method.Body.GetILProcessor();
            Instruction labelAfterThrow = iLProcessor.Create(OpCodes.Ldarg_0);
            this.WriteKeyNullCheck(iLProcessor, labelAfterThrow, 0);
            iLProcessor.Append(labelAfterThrow);
            iLProcessor.Emit(OpCodes.Ldarg_1);
            iLProcessor.Emit(OpCodes.Callvirt, reference);
            iLProcessor.Emit(OpCodes.Ret);
        }

        public void WriteGetItem(MethodDefinition method)
        {
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = m => m.Name == "Lookup";
            }
            MethodDefinition definition = this._iMapViewTypeDef.Methods.Single<MethodDefinition>(<>f__am$cache3);
            MethodReference reference = this._iMapViewInstanceTypeResolver.Resolve(definition);
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = m => ((m.HasThis && m.IsConstructor) && (m.Parameters.Count == 1)) && (m.Parameters[0].ParameterType.MetadataType == MetadataType.String);
            }
            MethodDefinition definition3 = TypeProvider.OptionalResolve("System.Collections.Generic", "KeyNotFoundException", TypeProvider.Corlib.Name).Methods.Single<MethodDefinition>(<>f__am$cache4);
            ILProcessor iLProcessor = method.Body.GetILProcessor();
            Instruction labelAfterThrow = iLProcessor.Create(OpCodes.Ldarg_0);
            this.WriteKeyNullCheck(iLProcessor, labelAfterThrow, 0);
            iLProcessor.Append(labelAfterThrow);
            iLProcessor.Emit(OpCodes.Ldarg_1);
            iLProcessor.Emit(OpCodes.Callvirt, reference);
            iLProcessor.Emit(OpCodes.Ret);
            Instruction instruction = iLProcessor.Create(OpCodes.Call, this._hresultGetter);
            Instruction target = iLProcessor.Create(OpCodes.Ldstr, "The given key was not present in the dictionary.");
            iLProcessor.Append(instruction);
            iLProcessor.Emit(OpCodes.Ldc_I4, -2147483637);
            iLProcessor.Emit(OpCodes.Beq, target);
            iLProcessor.Emit(OpCodes.Rethrow);
            iLProcessor.Append(target);
            iLProcessor.Emit(OpCodes.Newobj, definition3);
            iLProcessor.Emit(OpCodes.Throw);
            ExceptionHandler item = new ExceptionHandler(ExceptionHandlerType.Catch) {
                CatchType = TypeProvider.SystemException,
                TryStart = labelAfterThrow,
                TryEnd = instruction,
                HandlerStart = instruction,
                HandlerEnd = target
            };
            method.Body.ExceptionHandlers.Add(item);
        }

        public void WriteGetKeys(MethodDefinition method)
        {
            this.WriteGetReadOnlyCollection(method, ReadOnlyDictionaryCollectionTypesGenerator.CollectionKind.Key);
        }

        private void WriteGetReadOnlyCollection(MethodDefinition method, ReadOnlyDictionaryCollectionTypesGenerator.CollectionKind collectionKind)
        {
            TypeDefinition definition = new ReadOnlyDictionaryCollectionTypesGenerator(this._iReadOnlyDictionaryTypeDef, collectionKind).EmitReadOnlyDictionaryKeyCollection(method.DeclaringType.Module);
            GenericInstanceType typeReference = new GenericInstanceType(definition) {
                GenericArguments = { 
                    method.DeclaringType.GenericParameters[0],
                    method.DeclaringType.GenericParameters[1]
                }
            };
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = m => m.IsConstructor;
            }
            MethodDefinition definition2 = definition.Methods.Single<MethodDefinition>(<>f__am$cache7);
            MethodReference reference = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(typeReference).Resolve(definition2);
            ILProcessor iLProcessor = method.Body.GetILProcessor();
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Newobj, reference);
            iLProcessor.Emit(OpCodes.Ret);
        }

        public void WriteGetValues(MethodDefinition method)
        {
            this.WriteGetReadOnlyCollection(method, ReadOnlyDictionaryCollectionTypesGenerator.CollectionKind.Value);
        }

        private void WriteKeyNullCheck(ILProcessor ilProcessor, Instruction labelAfterThrow, int parameterIndex)
        {
            MethodDefinition method = ilProcessor.Body.Method;
            ilProcessor.Emit(OpCodes.Ldarg, (int) (parameterIndex + (!method.HasThis ? 0 : 1)));
            ilProcessor.Emit(OpCodes.Box, method.Parameters[parameterIndex].ParameterType);
            ilProcessor.Emit(OpCodes.Brtrue, labelAfterThrow);
            ilProcessor.Emit(OpCodes.Ldstr, "key");
            ilProcessor.Emit(OpCodes.Newobj, this._argumentNullExceptionConstructor);
            ilProcessor.Emit(OpCodes.Throw);
        }

        public void WriteTryGetValue(MethodDefinition method)
        {
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = m => m.Name == "System.Collections.Generic.IReadOnlyDictionary`2.ContainsKey";
            }
            MethodDefinition definition = method.DeclaringType.Methods.Single<MethodDefinition>(<>f__am$cache5);
            MethodReference reference = this._iReadOnlyDictionaryInstanceTypeResolver.Resolve(definition);
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = m => m.Name == "Lookup";
            }
            MethodDefinition definition2 = this._iMapViewTypeDef.Methods.Single<MethodDefinition>(<>f__am$cache6);
            MethodReference reference2 = this._iMapViewInstanceTypeResolver.Resolve(definition2);
            GenericParameter type = this._iReadOnlyDictionaryTypeDef.GenericParameters[1];
            ILProcessor iLProcessor = method.Body.GetILProcessor();
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Ldarg_1);
            iLProcessor.Emit(OpCodes.Call, reference);
            Instruction target = iLProcessor.Create(OpCodes.Ldarg_2);
            iLProcessor.Emit(OpCodes.Brtrue, target);
            Instruction instruction = iLProcessor.Create(OpCodes.Ldarg_2);
            iLProcessor.Append(instruction);
            iLProcessor.Emit(OpCodes.Initobj, type);
            iLProcessor.Emit(OpCodes.Ldc_I4_0);
            iLProcessor.Emit(OpCodes.Ret);
            iLProcessor.Append(target);
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Ldarg_1);
            iLProcessor.Emit(OpCodes.Callvirt, reference2);
            iLProcessor.Emit(OpCodes.Stobj, type);
            iLProcessor.Emit(OpCodes.Ldc_I4_1);
            iLProcessor.Emit(OpCodes.Ret);
            Instruction instruction3 = iLProcessor.Create(OpCodes.Call, this._hresultGetter);
            iLProcessor.Append(instruction3);
            iLProcessor.Emit(OpCodes.Ldc_I4, -2147483637);
            iLProcessor.Emit(OpCodes.Beq, instruction);
            iLProcessor.Emit(OpCodes.Rethrow);
            ExceptionHandler item = new ExceptionHandler(ExceptionHandlerType.Catch) {
                TryStart = target,
                TryEnd = instruction3,
                HandlerStart = instruction3,
                CatchType = TypeProvider.SystemException
            };
            method.Body.ExceptionHandlers.Add(item);
        }
    }
}

