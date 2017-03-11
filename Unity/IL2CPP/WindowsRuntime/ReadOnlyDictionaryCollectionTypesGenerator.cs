namespace Unity.IL2CPP.WindowsRuntime
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Rocks;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    internal sealed class ReadOnlyDictionaryCollectionTypesGenerator
    {
        private readonly CollectionKind _collectionKind;
        private readonly TypeDefinition _iDisposableTypeDef;
        private readonly TypeDefinition _iEnumerableTypeDef;
        private readonly TypeDefinition _iEnumeratorTypeDef;
        private readonly TypeDefinition _iReadOnlyDictionaryTypeDef;
        private readonly TypeDefinition _keyValuePairTypeDef;
        private readonly TypeDefinition _nonGenericIEnumeratorTypeDef;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache1;
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
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache8;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache9;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cacheA;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cacheB;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cacheC;
        [Inject]
        public static ITypeProviderService TypeProvider;

        public ReadOnlyDictionaryCollectionTypesGenerator(TypeDefinition iReadOnlyDictionaryTypeDef, CollectionKind collectionKind)
        {
            this._iReadOnlyDictionaryTypeDef = iReadOnlyDictionaryTypeDef;
            this._collectionKind = collectionKind;
            this._iDisposableTypeDef = TypeProvider.OptionalResolve("System", "IDisposable", TypeProvider.Corlib.Name);
            this._iEnumerableTypeDef = TypeProvider.OptionalResolve("System.Collections.Generic", "IEnumerable`1", TypeProvider.Corlib.Name);
            this._iEnumeratorTypeDef = TypeProvider.OptionalResolve("System.Collections.Generic", "IEnumerator`1", TypeProvider.Corlib.Name);
            this._nonGenericIEnumeratorTypeDef = TypeProvider.OptionalResolve("System.Collections", "IEnumerator", TypeProvider.Corlib.Name);
            this._keyValuePairTypeDef = TypeProvider.OptionalResolve("System.Collections.Generic", "KeyValuePair`2", TypeProvider.Corlib.Name);
        }

        private FieldDefinition AddDictionaryField(TypeDefinition typeDefinition)
        {
            GenericInstanceType fieldType = new GenericInstanceType(this._iReadOnlyDictionaryTypeDef) {
                GenericArguments = { 
                    typeDefinition.GenericParameters[0],
                    typeDefinition.GenericParameters[1]
                }
            };
            FieldDefinition item = new FieldDefinition("dictionary", FieldAttributes.CompilerControlled | FieldAttributes.InitOnly | FieldAttributes.Private, fieldType);
            typeDefinition.Fields.Add(item);
            return item;
        }

        private FieldDefinition AddEnumeratorField(TypeDefinition typeDefinition, TypeDefinition iEnumeratorTypeDef)
        {
            GenericInstanceType type = new GenericInstanceType(this._keyValuePairTypeDef) {
                GenericArguments = { 
                    typeDefinition.GenericParameters[0],
                    typeDefinition.GenericParameters[1]
                }
            };
            GenericInstanceType fieldType = new GenericInstanceType(iEnumeratorTypeDef) {
                GenericArguments = { type }
            };
            FieldDefinition item = new FieldDefinition("enumerator", FieldAttributes.CompilerControlled | FieldAttributes.Private, fieldType);
            typeDefinition.Fields.Add(item);
            return item;
        }

        private void EmitCollectionConstructor(TypeDefinition typeDefinition, FieldReference dictionaryField)
        {
            MethodAttributes attributes = MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName;
            MethodDefinition item = new MethodDefinition(".ctor", attributes, typeDefinition.Module.ImportReference(TypeProvider.SystemVoid)) {
                Parameters = { new ParameterDefinition("dictionary", ParameterAttributes.None, dictionaryField.FieldType) }
            };
            typeDefinition.Methods.Add(item);
            ILProcessor iLProcessor = item.Body.GetILProcessor();
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Call, GetObjectConstructor());
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Ldarg_1);
            iLProcessor.Emit(OpCodes.Stfld, dictionaryField);
            iLProcessor.Emit(OpCodes.Ret);
            item.Body.OptimizeMacros();
        }

        private void EmitEnumeratorConstructor(TypeDefinition typeDefinition, FieldReference dictionaryField, FieldReference enumeratorField, MethodReference getEnumeratorMethod)
        {
            MethodAttributes attributes = MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName;
            MethodDefinition item = new MethodDefinition(".ctor", attributes, typeDefinition.Module.ImportReference(TypeProvider.SystemVoid)) {
                Parameters = { new ParameterDefinition("dictionary", ParameterAttributes.None, dictionaryField.FieldType) }
            };
            typeDefinition.Methods.Add(item);
            ILProcessor iLProcessor = item.Body.GetILProcessor();
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Call, GetObjectConstructor());
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Ldarg_1);
            iLProcessor.Emit(OpCodes.Stfld, dictionaryField);
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Ldarg_1);
            iLProcessor.Emit(OpCodes.Callvirt, getEnumeratorMethod);
            iLProcessor.Emit(OpCodes.Stfld, enumeratorField);
            iLProcessor.Emit(OpCodes.Ret);
            item.Body.OptimizeMacros();
        }

        private void EmitEnumeratorDisposeBody(MethodDefinition method, FieldReference enumeratorFieldInstance)
        {
            if (<>f__am$cache9 == null)
            {
                <>f__am$cache9 = m => (m.Name == "Dispose") && (m.Parameters.Count == 0);
            }
            MethodDefinition methodToForwardTo = this._iDisposableTypeDef.Methods.Single<MethodDefinition>(<>f__am$cache9);
            this.EmitMethodForwardingToFieldMethodBody(method, enumeratorFieldInstance, methodToForwardTo);
        }

        private void EmitEnumeratorGetCurrentMethod(MethodDefinition getCurrentMethod, MethodReference iEnumeratorOfTCurrentMethod)
        {
            getCurrentMethod.Attributes = (MethodAttributes) ((ushort) (getCurrentMethod.Attributes & (MethodAttributes.Assembly | MethodAttributes.CheckAccessOnOverride | MethodAttributes.Family | MethodAttributes.Final | MethodAttributes.HasSecurity | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.PInvokeImpl | MethodAttributes.RequireSecObject | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName | MethodAttributes.Static | MethodAttributes.UnmanagedExport | MethodAttributes.Virtual)));
            ILProcessor iLProcessor = getCurrentMethod.Body.GetILProcessor();
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Call, iEnumeratorOfTCurrentMethod);
            iLProcessor.Emit(OpCodes.Box, getCurrentMethod.DeclaringType.GenericParameters[(int) this._collectionKind]);
            iLProcessor.Emit(OpCodes.Ret);
            getCurrentMethod.Body.OptimizeMacros();
        }

        private void EmitEnumeratorMoveNextMethodBody(MethodDefinition method, FieldReference dictionaryFieldInstance, FieldReference enumeratorFieldInstance)
        {
            if (<>f__am$cacheA == null)
            {
                <>f__am$cacheA = m => (m.Name == "MoveNext") && (m.Parameters.Count == 0);
            }
            MethodDefinition methodToForwardTo = this._nonGenericIEnumeratorTypeDef.Methods.Single<MethodDefinition>(<>f__am$cacheA);
            this.EmitMethodForwardingToFieldMethodBody(method, enumeratorFieldInstance, methodToForwardTo);
        }

        private void EmitEnumeratorOfTGetCurrentMethod(MethodDefinition getCurrentMethod, FieldReference enumeratorFieldInstance)
        {
            <EmitEnumeratorOfTGetCurrentMethod>c__AnonStorey0 storey = new <EmitEnumeratorOfTGetCurrentMethod>c__AnonStorey0();
            if (<>f__am$cacheB == null)
            {
                <>f__am$cacheB = m => m.Name == "get_Current";
            }
            MethodDefinition method = this._iEnumeratorTypeDef.Methods.Single<MethodDefinition>(<>f__am$cacheB);
            MethodReference reference = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(enumeratorFieldInstance.FieldType).Resolve(method);
            GenericInstanceType typeReference = (GenericInstanceType) ((GenericInstanceType) enumeratorFieldInstance.FieldType).GenericArguments[0];
            storey.keyValuePairGetItemMethodName = (this._collectionKind != CollectionKind.Key) ? "get_Value" : "get_Key";
            MethodDefinition definition2 = typeReference.Resolve().Methods.Single<MethodDefinition>(new Func<MethodDefinition, bool>(storey.<>m__0));
            MethodReference reference2 = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(typeReference).Resolve(definition2);
            getCurrentMethod.Attributes = (MethodAttributes) ((ushort) (getCurrentMethod.Attributes & (MethodAttributes.Assembly | MethodAttributes.CheckAccessOnOverride | MethodAttributes.Family | MethodAttributes.Final | MethodAttributes.HasSecurity | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.PInvokeImpl | MethodAttributes.RequireSecObject | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName | MethodAttributes.Static | MethodAttributes.UnmanagedExport | MethodAttributes.Virtual)));
            MethodBody body = getCurrentMethod.Body;
            ILProcessor iLProcessor = body.GetILProcessor();
            body.Variables.Add(new VariableDefinition(typeReference));
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Ldfld, enumeratorFieldInstance);
            iLProcessor.Emit(OpCodes.Callvirt, reference);
            iLProcessor.Emit(OpCodes.Stloc_0);
            iLProcessor.Emit(OpCodes.Ldloca_S, body.Variables[0]);
            iLProcessor.Emit(OpCodes.Call, reference2);
            iLProcessor.Emit(OpCodes.Ret);
            getCurrentMethod.Body.OptimizeMacros();
        }

        private void EmitEnumeratorResetMethodBody(MethodDefinition resetMethod, FieldReference dictionaryFieldInstance, FieldReference enumeratorFieldInstance, MethodReference getEnumeratorMethod)
        {
            resetMethod.Attributes = (MethodAttributes) ((ushort) (resetMethod.Attributes & (MethodAttributes.Assembly | MethodAttributes.CheckAccessOnOverride | MethodAttributes.Family | MethodAttributes.Final | MethodAttributes.HasSecurity | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.PInvokeImpl | MethodAttributes.RequireSecObject | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName | MethodAttributes.Static | MethodAttributes.UnmanagedExport | MethodAttributes.Virtual)));
            ILProcessor iLProcessor = resetMethod.Body.GetILProcessor();
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Ldfld, dictionaryFieldInstance);
            iLProcessor.Emit(OpCodes.Callvirt, getEnumeratorMethod);
            iLProcessor.Emit(OpCodes.Stfld, enumeratorFieldInstance);
            iLProcessor.Emit(OpCodes.Ret);
            resetMethod.Body.OptimizeMacros();
        }

        private TypeDefinition EmitEnumeratorType(ModuleDefinition module)
        {
            TypeAttributes attributes = TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.Sealed;
            TypeDefinition item = new TypeDefinition("System.Runtime.InteropServices.WindowsRuntime", $"ReadOnlyDictionary{this._collectionKind}Enumerator`2", attributes, TypeProvider.SystemObject);
            module.Types.Add(item);
            item.GenericParameters.Add(new GenericParameter("K", item));
            item.GenericParameters.Add(new GenericParameter("V", item));
            GenericInstanceType interfaceType = new GenericInstanceType(this._iEnumeratorTypeDef) {
                GenericArguments = { item.GenericParameters[(int) this._collectionKind] }
            };
            InterfaceUtilities.MakeImplementInterface(item, interfaceType);
            FieldDefinition field = this.AddDictionaryField(item);
            FieldDefinition definition3 = this.AddEnumeratorField(item, this._iEnumeratorTypeDef);
            GenericInstanceType typeReference = new GenericInstanceType(item) {
                GenericArguments = { 
                    item.GenericParameters[0],
                    item.GenericParameters[1]
                }
            };
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(typeReference);
            FieldReference dictionaryField = resolver.Resolve(field);
            FieldReference enumeratorField = resolver.Resolve(definition3);
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = m => m.Name == "GetEnumerator";
            }
            MethodDefinition method = this._iEnumerableTypeDef.Methods.Single<MethodDefinition>(<>f__am$cache3);
            MethodReference getEnumeratorMethod = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(enumeratorField.FieldType).Resolve(method);
            this.EmitEnumeratorConstructor(item, dictionaryField, enumeratorField, getEnumeratorMethod);
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = m => m.Name == "System.IDisposable.Dispose";
            }
            this.EmitEnumeratorDisposeBody(item.Methods.Single<MethodDefinition>(<>f__am$cache4), enumeratorField);
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = m => m.Name == "System.Collections.IEnumerator.MoveNext";
            }
            this.EmitEnumeratorMoveNextMethodBody(item.Methods.Single<MethodDefinition>(<>f__am$cache5), dictionaryField, enumeratorField);
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = m => m.Name == "System.Collections.IEnumerator.Reset";
            }
            this.EmitEnumeratorResetMethodBody(item.Methods.Single<MethodDefinition>(<>f__am$cache6), dictionaryField, enumeratorField, getEnumeratorMethod);
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = m => m.Name == "System.Collections.Generic.IEnumerator`1.get_Current";
            }
            MethodDefinition getCurrentMethod = item.Methods.Single<MethodDefinition>(<>f__am$cache7);
            this.EmitEnumeratorOfTGetCurrentMethod(getCurrentMethod, enumeratorField);
            if (<>f__am$cache8 == null)
            {
                <>f__am$cache8 = m => m.Name == "System.Collections.IEnumerator.get_Current";
            }
            this.EmitEnumeratorGetCurrentMethod(item.Methods.Single<MethodDefinition>(<>f__am$cache8), resolver.Resolve(getCurrentMethod));
            return item;
        }

        private void EmitIEnumerableGetEnumeratorMethodBody(MethodDefinition getEnumeratorMethod, MethodReference iEnumeratorOfTGetEnumeratorMethod)
        {
            getEnumeratorMethod.Attributes = (MethodAttributes) ((ushort) (getEnumeratorMethod.Attributes & (MethodAttributes.Assembly | MethodAttributes.CheckAccessOnOverride | MethodAttributes.Family | MethodAttributes.Final | MethodAttributes.HasSecurity | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.PInvokeImpl | MethodAttributes.RequireSecObject | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName | MethodAttributes.Static | MethodAttributes.UnmanagedExport | MethodAttributes.Virtual)));
            ILProcessor iLProcessor = getEnumeratorMethod.Body.GetILProcessor();
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Call, iEnumeratorOfTGetEnumeratorMethod);
            iLProcessor.Emit(OpCodes.Ret);
            getEnumeratorMethod.Body.OptimizeMacros();
        }

        private void EmitIEnumerableOfTGetEnumeratorMethodBody(MethodDefinition getEnumeratorMethod, TypeDefinition enumeratorType, FieldReference dictionaryField)
        {
            GenericInstanceType typeReference = new GenericInstanceType(enumeratorType) {
                GenericArguments = { 
                    getEnumeratorMethod.DeclaringType.GenericParameters[0],
                    getEnumeratorMethod.DeclaringType.GenericParameters[1]
                }
            };
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = m => m.IsConstructor;
            }
            MethodReference method = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(typeReference).Resolve(enumeratorType.Methods.Single<MethodDefinition>(<>f__am$cache2));
            getEnumeratorMethod.Attributes = (MethodAttributes) ((ushort) (getEnumeratorMethod.Attributes & (MethodAttributes.Assembly | MethodAttributes.CheckAccessOnOverride | MethodAttributes.Family | MethodAttributes.Final | MethodAttributes.HasSecurity | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.PInvokeImpl | MethodAttributes.RequireSecObject | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName | MethodAttributes.Static | MethodAttributes.UnmanagedExport | MethodAttributes.Virtual)));
            ILProcessor iLProcessor = getEnumeratorMethod.Body.GetILProcessor();
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Ldfld, dictionaryField);
            iLProcessor.Emit(OpCodes.Newobj, method);
            iLProcessor.Emit(OpCodes.Ret);
            getEnumeratorMethod.Body.OptimizeMacros();
        }

        private void EmitMethodForwardingToFieldMethodBody(MethodDefinition method, FieldReference fieldInstance, MethodReference methodToForwardTo)
        {
            method.Attributes = (MethodAttributes) ((ushort) (method.Attributes & (MethodAttributes.Assembly | MethodAttributes.CheckAccessOnOverride | MethodAttributes.Family | MethodAttributes.Final | MethodAttributes.HasSecurity | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.PInvokeImpl | MethodAttributes.RequireSecObject | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName | MethodAttributes.Static | MethodAttributes.UnmanagedExport | MethodAttributes.Virtual)));
            ILProcessor iLProcessor = method.Body.GetILProcessor();
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Ldfld, fieldInstance);
            for (int i = 0; i < method.Parameters.Count; i++)
            {
                iLProcessor.Emit(OpCodes.Ldarg, (int) (i + 1));
            }
            iLProcessor.Emit(OpCodes.Callvirt, methodToForwardTo);
            iLProcessor.Emit(OpCodes.Ret);
            method.Body.OptimizeMacros();
        }

        public TypeDefinition EmitReadOnlyDictionaryKeyCollection(ModuleDefinition module)
        {
            TypeAttributes attributes = TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.Sealed;
            TypeDefinition item = new TypeDefinition("System.Runtime.InteropServices.WindowsRuntime", $"ReadOnlyDictionary{this._collectionKind}Collection`2", attributes, TypeProvider.SystemObject);
            module.Types.Add(item);
            item.GenericParameters.Add(new GenericParameter("K", item));
            item.GenericParameters.Add(new GenericParameter("V", item));
            GenericInstanceType interfaceType = new GenericInstanceType(this._iEnumerableTypeDef) {
                GenericArguments = { item.GenericParameters[(int) this._collectionKind] }
            };
            InterfaceUtilities.MakeImplementInterface(item, interfaceType);
            FieldDefinition field = this.AddDictionaryField(item);
            GenericInstanceType typeReference = new GenericInstanceType(item) {
                GenericArguments = { 
                    item.GenericParameters[0],
                    item.GenericParameters[1]
                }
            };
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(typeReference);
            FieldReference dictionaryField = resolver.Resolve(field);
            this.EmitCollectionConstructor(item, dictionaryField);
            TypeDefinition enumeratorType = this.EmitEnumeratorType(module);
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = m => m.Name == "System.Collections.Generic.IEnumerable`1.GetEnumerator";
            }
            MethodDefinition getEnumeratorMethod = item.Methods.Single<MethodDefinition>(<>f__am$cache0);
            this.EmitIEnumerableOfTGetEnumeratorMethodBody(getEnumeratorMethod, enumeratorType, dictionaryField);
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = m => m.Name == "System.Collections.IEnumerable.GetEnumerator";
            }
            MethodDefinition definition6 = item.Methods.Single<MethodDefinition>(<>f__am$cache1);
            this.EmitIEnumerableGetEnumeratorMethodBody(definition6, resolver.Resolve(getEnumeratorMethod));
            return item;
        }

        private static MethodDefinition GetObjectConstructor()
        {
            if (<>f__am$cacheC == null)
            {
                <>f__am$cacheC = m => (m.IsConstructor && m.HasThis) && (m.Parameters.Count == 0);
            }
            return TypeProvider.SystemObject.Methods.Single<MethodDefinition>(<>f__am$cacheC);
        }

        [CompilerGenerated]
        private sealed class <EmitEnumeratorOfTGetCurrentMethod>c__AnonStorey0
        {
            internal string keyValuePairGetItemMethodName;

            internal bool <>m__0(MethodDefinition m) => 
                ((m.HasThis && (m.Name == this.keyValuePairGetItemMethodName)) && (m.Parameters.Count == 0));
        }

        public enum CollectionKind
        {
            Key,
            Value
        }
    }
}

