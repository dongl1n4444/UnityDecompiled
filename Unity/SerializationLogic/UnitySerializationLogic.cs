namespace Unity.SerializationLogic
{
    using Mono.Cecil;
    using Mono.Collections.Generic;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Unity.CecilTools;
    using Unity.CecilTools.Extensions;

    public static class UnitySerializationLogic
    {
        [CompilerGenerated]
        private static Func<CustomAttribute, TypeReference> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<CustomAttribute, bool> <>f__am$cache1;

        [DebuggerHidden]
        private static IEnumerable<KeyValuePair<FieldDefinition, Unity.SerializationLogic.TypeResolver>> AllFieldsFor(TypeDefinition definition, Unity.SerializationLogic.TypeResolver typeResolver) => 
            new <AllFieldsFor>c__Iterator0 { 
                definition = definition,
                typeResolver = typeResolver,
                $PC = -2
            };

        private static bool CanFieldContainUnityEngineObjectReference(TypeReference typeReference, FieldDefinition t, Unity.SerializationLogic.TypeResolver typeResolver)
        {
            if (typeResolver.Resolve(t.FieldType) == typeReference)
            {
                return false;
            }
            if (!WillUnitySerialize(t, typeResolver))
            {
                return false;
            }
            if (UnityEngineTypePredicates.IsUnityEngineValueType(typeReference))
            {
                return false;
            }
            return true;
        }

        private static bool CanTypeContainUnityEngineObjectReference(TypeReference typeReference)
        {
            if (IsUnityEngineObject(typeReference))
            {
                return true;
            }
            if (typeReference.IsEnum())
            {
                return false;
            }
            if (IsSerializablePrimitive(typeReference))
            {
                return false;
            }
            if (IsSupportedCollection(typeReference))
            {
                return CanTypeContainUnityEngineObjectReference(CecilUtils.ElementTypeOfCollection(typeReference));
            }
            TypeDefinition definition = typeReference.Resolve();
            if (definition == null)
            {
                return false;
            }
            return HasFieldsThatCanContainUnityEngineObjectReferences(definition, new Unity.SerializationLogic.TypeResolver(typeReference as GenericInstanceType));
        }

        private static IEnumerable<TypeReference> FieldAttributes(FieldDefinition field)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<CustomAttribute, TypeReference>(null, (IntPtr) <FieldAttributes>m__0);
            }
            return Enumerable.Select<CustomAttribute, TypeReference>(field.CustomAttributes, <>f__am$cache0);
        }

        private static bool HasFieldsThatCanContainUnityEngineObjectReferences(TypeDefinition definition, Unity.SerializationLogic.TypeResolver typeResolver)
        {
            <HasFieldsThatCanContainUnityEngineObjectReferences>c__AnonStorey1 storey = new <HasFieldsThatCanContainUnityEngineObjectReferences>c__AnonStorey1 {
                definition = definition
            };
            return Enumerable.Any<KeyValuePair<FieldDefinition, Unity.SerializationLogic.TypeResolver>>(Enumerable.Where<KeyValuePair<FieldDefinition, Unity.SerializationLogic.TypeResolver>>(AllFieldsFor(storey.definition, typeResolver), new Func<KeyValuePair<FieldDefinition, Unity.SerializationLogic.TypeResolver>, bool>(storey, (IntPtr) this.<>m__0)), new Func<KeyValuePair<FieldDefinition, Unity.SerializationLogic.TypeResolver>, bool>(storey, (IntPtr) this.<>m__1));
        }

        public static bool HasSerializeFieldAttribute(FieldDefinition field)
        {
            foreach (TypeReference reference in FieldAttributes(field))
            {
                if (UnityEngineTypePredicates.IsSerializeFieldAttribute(reference))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsConst(FieldDefinition fieldDefinition) => 
            (fieldDefinition.IsLiteral && !fieldDefinition.IsInitOnly);

        private static bool IsDelegate(TypeReference typeReference) => 
            typeReference.IsAssignableTo("System.Delegate");

        private static bool IsFieldTypeSerializable(TypeReference typeReference) => 
            (IsTypeSerializable(typeReference) || IsSupportedCollection(typeReference));

        public static bool IsNonSerialized(TypeReference typeDeclaration) => 
            ((typeDeclaration == null) || (typeDeclaration.IsEnum() || (typeDeclaration.HasGenericParameters || ((typeDeclaration.MetadataType == MetadataType.Object) || (typeDeclaration.FullName.StartsWith("System.") || (typeDeclaration.IsArray || ((typeDeclaration.FullName == "UnityEngine.MonoBehaviour") || (typeDeclaration.FullName == "UnityEngine.ScriptableObject"))))))));

        private static bool IsOrExtendsGenericDictionary(TypeReference typeReference)
        {
            TypeDefinition definition;
            for (TypeReference reference = typeReference; reference != null; reference = definition.BaseType)
            {
                if (CecilUtils.IsGenericDictionary(reference))
                {
                    return true;
                }
                definition = reference.CheckedResolve();
                if (definition == null)
                {
                    break;
                }
            }
            return false;
        }

        private static bool IsSerializablePrimitive(TypeReference typeReference)
        {
            switch (typeReference.MetadataType)
            {
                case MetadataType.Boolean:
                case MetadataType.Char:
                case MetadataType.SByte:
                case MetadataType.Byte:
                case MetadataType.Int16:
                case MetadataType.UInt16:
                case MetadataType.Int32:
                case MetadataType.UInt32:
                case MetadataType.Int64:
                case MetadataType.UInt64:
                case MetadataType.Single:
                case MetadataType.Double:
                case MetadataType.String:
                    return true;
            }
            return false;
        }

        public static bool IsSupportedCollection(TypeReference typeReference)
        {
            if (!(typeReference is ArrayType) && !CecilUtils.IsGenericList(typeReference))
            {
                return false;
            }
            if (typeReference.IsArray && (((ArrayType) typeReference).Rank > 1))
            {
                return false;
            }
            return IsTypeSerializable(CecilUtils.ElementTypeOfCollection(typeReference));
        }

        private static bool IsTypeSerializable(TypeReference typeReference)
        {
            if (typeReference.IsAssignableTo("UnityScript.Lang.Array"))
            {
                return false;
            }
            if (IsOrExtendsGenericDictionary(typeReference))
            {
                return false;
            }
            return (((IsSerializablePrimitive(typeReference) || typeReference.IsEnum()) || (IsUnityEngineObject(typeReference) || UnityEngineTypePredicates.IsSerializableUnityStruct(typeReference))) || ShouldImplementIDeserializable(typeReference));
        }

        private static bool IsUnityEngineObject(TypeReference typeReference) => 
            UnityEngineTypePredicates.IsUnityEngineObject(typeReference);

        public static bool ShouldFieldBePPtrRemapped(FieldDefinition fieldDefinition) => 
            ShouldFieldBePPtrRemapped(fieldDefinition, new Unity.SerializationLogic.TypeResolver(null));

        public static bool ShouldFieldBePPtrRemapped(FieldDefinition fieldDefinition, Unity.SerializationLogic.TypeResolver typeResolver)
        {
            if (!WillUnitySerialize(fieldDefinition, typeResolver))
            {
                return false;
            }
            return CanTypeContainUnityEngineObjectReference(typeResolver.Resolve(fieldDefinition.FieldType));
        }

        private static bool ShouldHaveHadAllFieldsPublic(FieldDefinition field) => 
            UnityEngineTypePredicates.IsUnityEngineValueType(field.DeclaringType);

        public static bool ShouldImplementIDeserializable(TypeReference typeDeclaration)
        {
            if (IsNonSerialized(typeDeclaration))
            {
                return false;
            }
            if (typeDeclaration is GenericInstanceType)
            {
                return false;
            }
            try
            {
                if (((!UnityEngineTypePredicates.IsMonoBehaviour(typeDeclaration) && !UnityEngineTypePredicates.IsScriptableObject(typeDeclaration)) && (typeDeclaration.CheckedResolve().IsSerializable && !typeDeclaration.CheckedResolve().IsAbstract)) && (<>f__am$cache1 == null))
                {
                    <>f__am$cache1 = new Func<CustomAttribute, bool>(null, (IntPtr) <ShouldImplementIDeserializable>m__1);
                }
                return (!Enumerable.Any<CustomAttribute>(typeDeclaration.CheckedResolve().CustomAttributes, <>f__am$cache1) || UnityEngineTypePredicates.ShouldHaveHadSerializableAttribute(typeDeclaration));
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool ShouldNotTryToResolve(TypeReference typeReference)
        {
            if (typeReference.Scope.Name == "Windows")
            {
                return true;
            }
            if (typeReference.Scope.Name == "mscorlib")
            {
                return (typeReference.Resolve() == null);
            }
            try
            {
                typeReference.Resolve();
            }
            catch
            {
                return true;
            }
            return false;
        }

        public static bool WillUnitySerialize(FieldDefinition fieldDefinition) => 
            WillUnitySerialize(fieldDefinition, new Unity.SerializationLogic.TypeResolver(null));

        public static bool WillUnitySerialize(FieldDefinition fieldDefinition, Unity.SerializationLogic.TypeResolver typeResolver)
        {
            if (fieldDefinition == null)
            {
                return false;
            }
            if ((fieldDefinition.IsStatic || IsConst(fieldDefinition)) || (fieldDefinition.IsNotSerialized || fieldDefinition.IsInitOnly))
            {
                return false;
            }
            if (ShouldNotTryToResolve(fieldDefinition.FieldType))
            {
                return false;
            }
            bool flag2 = HasSerializeFieldAttribute(fieldDefinition);
            if ((!fieldDefinition.IsPublic && !flag2) && !ShouldHaveHadAllFieldsPublic(fieldDefinition))
            {
                return false;
            }
            if (fieldDefinition.FullName == "UnityScript.Lang.Array")
            {
                return false;
            }
            if (!IsFieldTypeSerializable(typeResolver.Resolve(fieldDefinition.FieldType)))
            {
                return false;
            }
            if (IsDelegate(typeResolver.Resolve(fieldDefinition.FieldType)))
            {
                return false;
            }
            return true;
        }

        [CompilerGenerated]
        private sealed class <AllFieldsFor>c__Iterator0 : IEnumerable, IEnumerable<KeyValuePair<FieldDefinition, Unity.SerializationLogic.TypeResolver>>, IEnumerator, IDisposable, IEnumerator<KeyValuePair<FieldDefinition, Unity.SerializationLogic.TypeResolver>>
        {
            internal KeyValuePair<FieldDefinition, Unity.SerializationLogic.TypeResolver> $current;
            internal bool $disposing;
            internal IEnumerator<KeyValuePair<FieldDefinition, Unity.SerializationLogic.TypeResolver>> $locvar0;
            internal Collection<FieldDefinition>.Enumerator $locvar1;
            internal int $PC;
            internal TypeReference <baseType>__0;
            internal FieldDefinition <fieldDefinition>__3;
            internal GenericInstanceType <genericBaseInstanceType>__1;
            internal KeyValuePair<FieldDefinition, Unity.SerializationLogic.TypeResolver> <kv>__2;
            internal TypeDefinition definition;
            internal Unity.SerializationLogic.TypeResolver typeResolver;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 1:
                        try
                        {
                        }
                        finally
                        {
                            if (this.$locvar0 != null)
                            {
                                this.$locvar0.Dispose();
                            }
                        }
                        break;

                    case 2:
                        try
                        {
                        }
                        finally
                        {
                            this.$locvar1.Dispose();
                        }
                        break;
                }
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                        this.<baseType>__0 = this.definition.BaseType;
                        if (this.<baseType>__0 == null)
                        {
                            goto Label_0128;
                        }
                        this.<genericBaseInstanceType>__1 = this.<baseType>__0 as GenericInstanceType;
                        if (this.<genericBaseInstanceType>__1 != null)
                        {
                            this.typeResolver.Add(this.<genericBaseInstanceType>__1);
                        }
                        this.$locvar0 = UnitySerializationLogic.AllFieldsFor(this.<baseType>__0.Resolve(), this.typeResolver).GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 1:
                        break;

                    case 2:
                        goto Label_0142;

                    default:
                        goto Label_01C3;
                }
                try
                {
                    while (this.$locvar0.MoveNext())
                    {
                        this.<kv>__2 = this.$locvar0.Current;
                        this.$current = this.<kv>__2;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        flag = true;
                        goto Label_01C5;
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    if (this.$locvar0 != null)
                    {
                        this.$locvar0.Dispose();
                    }
                }
                if (this.<genericBaseInstanceType>__1 != null)
                {
                    this.typeResolver.Remove(this.<genericBaseInstanceType>__1);
                }
            Label_0128:
                this.$locvar1 = this.definition.Fields.GetEnumerator();
                num = 0xfffffffd;
            Label_0142:
                try
                {
                    while (this.$locvar1.MoveNext())
                    {
                        this.<fieldDefinition>__3 = this.$locvar1.Current;
                        this.$current = new KeyValuePair<FieldDefinition, Unity.SerializationLogic.TypeResolver>(this.<fieldDefinition>__3, this.typeResolver);
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        flag = true;
                        goto Label_01C5;
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    this.$locvar1.Dispose();
                }
                this.$PC = -1;
            Label_01C3:
                return false;
            Label_01C5:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<KeyValuePair<FieldDefinition, Unity.SerializationLogic.TypeResolver>> IEnumerable<KeyValuePair<FieldDefinition, Unity.SerializationLogic.TypeResolver>>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new UnitySerializationLogic.<AllFieldsFor>c__Iterator0 { 
                    definition = this.definition,
                    typeResolver = this.typeResolver
                };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<Mono.Cecil.FieldDefinition,Unity.SerializationLogic.TypeResolver>>.GetEnumerator();

            KeyValuePair<FieldDefinition, Unity.SerializationLogic.TypeResolver> IEnumerator<KeyValuePair<FieldDefinition, Unity.SerializationLogic.TypeResolver>>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        [CompilerGenerated]
        private sealed class <HasFieldsThatCanContainUnityEngineObjectReferences>c__AnonStorey1
        {
            internal TypeDefinition definition;

            internal bool <>m__0(KeyValuePair<FieldDefinition, Unity.SerializationLogic.TypeResolver> kv) => 
                (kv.Value.Resolve(kv.Key.FieldType).Resolve() != this.definition);

            internal bool <>m__1(KeyValuePair<FieldDefinition, Unity.SerializationLogic.TypeResolver> kv) => 
                UnitySerializationLogic.CanFieldContainUnityEngineObjectReference(this.definition, kv.Key, kv.Value);
        }
    }
}

