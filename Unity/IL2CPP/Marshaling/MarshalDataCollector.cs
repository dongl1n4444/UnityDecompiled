namespace Unity.IL2CPP.Marshaling
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Marshaling.MarshalInfoWriters;

    public class MarshalDataCollector
    {
        [CompilerGenerated]
        private static Func<TypeDefinition, IEnumerable<FieldDefinition>> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<TypeDefinition, bool> <>f__am$cache1;
        [Inject]
        public static ITypeProviderService TypeProvider;
        [Inject]
        public static IWindowsRuntimeProjections WindowsRuntimeProjections;

        private static DefaultMarshalInfoWriter CreateMarshalInfoWriter(TypeReference type, MarshalType marshalType, MarshalInfo marshalInfo, bool useUnicodeCharSet, bool forByReferenceType, bool forFieldMarshaling, HashSet<TypeReference> typesForRecursiveFields)
        {
            <CreateMarshalInfoWriter>c__AnonStorey0 storey = new <CreateMarshalInfoWriter>c__AnonStorey0 {
                typesForRecursiveFields = typesForRecursiveFields,
                type = type,
                marshalType = marshalType,
                useUnicodeCharSet = useUnicodeCharSet
            };
            storey.useUnicodeCharSet |= (storey.type.Resolve().Attributes & (TypeAttributes.AnsiClass | TypeAttributes.UnicodeClass)) != TypeAttributes.AnsiClass;
            if ((storey.type.MetadataType == MetadataType.String) || MarshalingUtils.IsStringBuilder(storey.type))
            {
                return new StringMarshalInfoWriter(storey.type, storey.marshalType, marshalInfo, storey.useUnicodeCharSet, forByReferenceType, forFieldMarshaling);
            }
            if (storey.type.Resolve().IsDelegate() && (!(storey.type is TypeSpecification) || (storey.type is GenericInstanceType)))
            {
                if (storey.marshalType == MarshalType.WindowsRuntime)
                {
                    return new WindowsRuntimeDelegateMarshalInfoWriter(storey.type);
                }
                return new DelegateMarshalInfoWriter(storey.type);
            }
            storey.nativeType = (marshalInfo == null) ? null : new NativeType?(marshalInfo.NativeType);
            if ((storey.type.MetadataType == MetadataType.ValueType) && MarshalingUtils.IsBlittable(storey.type, storey.nativeType, storey.marshalType, storey.useUnicodeCharSet))
            {
                if ((!forByReferenceType && !forFieldMarshaling) && ((marshalInfo != null) && (marshalInfo.NativeType == NativeType.LPStruct)))
                {
                    return new LPStructMarshalInfoWriter(storey.type, storey.marshalType);
                }
                return new BlittableStructMarshalInfoWriter(storey.type.Resolve(), storey.marshalType);
            }
            if ((storey.type.IsPrimitive || storey.type.IsPointer) || (storey.type.IsEnum() || (storey.type.MetadataType == MetadataType.Void)))
            {
                return new PrimitiveMarshalInfoWriter(storey.type, marshalInfo, storey.marshalType);
            }
            if (Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(storey.type, TypeProvider.Corlib.MainModule.GetType("System.Runtime.InteropServices.HandleRef"), TypeComparisonMode.Exact))
            {
                return new HandleRefMarshalInfoWriter(storey.type, forByReferenceType);
            }
            ByReferenceType type2 = storey.type as ByReferenceType;
            if (type2 != null)
            {
                TypeReference elementType = type2.ElementType;
                if (MarshalingUtils.IsBlittable(elementType, storey.nativeType, storey.marshalType, storey.useUnicodeCharSet) && (elementType.IsValueType() || storey.type.IsPointer))
                {
                    return new BlittableByReferenceMarshalInfoWriter(type2, storey.marshalType, marshalInfo);
                }
                return new ByReferenceMarshalInfoWriter(type2, storey.marshalType, marshalInfo);
            }
            ArrayType type3 = storey.type as ArrayType;
            if (type3 != null)
            {
                TypeReference reference2 = type3.ElementType;
                ArrayMarshalInfo info = marshalInfo as ArrayMarshalInfo;
                NativeType? nativeType = (info == null) ? null : new NativeType?(info.ElementType);
                if (storey.marshalType != MarshalType.WindowsRuntime)
                {
                    if (!MarshalingUtils.IsStringBuilder(reference2) && (((reference2.MetadataType == MetadataType.Class) || (reference2.MetadataType == MetadataType.Object)) || (reference2.MetadataType == MetadataType.Array)))
                    {
                        return new UnmarshalableMarshalInfoWriter(storey.type);
                    }
                    if ((marshalInfo != null) && (marshalInfo.NativeType == NativeType.SafeArray))
                    {
                        return new ComSafeArrayMarshalInfoWriter(type3, marshalInfo);
                    }
                    if ((marshalInfo != null) && (marshalInfo.NativeType == NativeType.FixedArray))
                    {
                        return new FixedArrayMarshalInfoWriter(type3, storey.marshalType, marshalInfo);
                    }
                }
                if ((!forByReferenceType && !forFieldMarshaling) && MarshalingUtils.IsBlittable(reference2, nativeType, storey.marshalType, storey.useUnicodeCharSet))
                {
                    return new PinnedArrayMarshalInfoWriter(type3, storey.marshalType, marshalInfo);
                }
                return new LPArrayMarshalInfoWriter(type3, storey.marshalType, marshalInfo);
            }
            TypeDefinition potentialBaseType = TypeProvider.Corlib.MainModule.GetType("System.Runtime.InteropServices.SafeHandle");
            if (storey.type.DerivesFrom(potentialBaseType, false))
            {
                return new SafeHandleMarshalInfoWriter(storey.type, potentialBaseType);
            }
            if (storey.type.IsComOrWindowsRuntimeInterface())
            {
                return new ComObjectMarshalInfoWriter(storey.type, storey.marshalType, marshalInfo);
            }
            if (storey.type.IsSystemObject())
            {
                if (marshalInfo != null)
                {
                    switch (marshalInfo.NativeType)
                    {
                        case NativeType.IUnknown:
                        case NativeType.IntF:
                        case (NativeType.CustomMarshaler | NativeType.Boolean):
                            return new ComObjectMarshalInfoWriter(storey.type, storey.marshalType, marshalInfo);

                        case NativeType.Struct:
                            return new ComVariantMarshalInfoWriter(storey.type);
                    }
                }
                if (storey.marshalType == MarshalType.WindowsRuntime)
                {
                    return new ComObjectMarshalInfoWriter(storey.type, storey.marshalType, marshalInfo);
                }
            }
            storey.typeDefinition = storey.type.Resolve();
            if (storey.marshalType == MarshalType.WindowsRuntime)
            {
                if (Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(storey.typeDefinition, TypeProvider.SystemException, TypeComparisonMode.Exact))
                {
                    return new ExceptionMarshalInfoWriter(storey.typeDefinition);
                }
                if (storey.type.MetadataType == MetadataType.Class)
                {
                    if (storey.typeDefinition.IsWindowsRuntime && !(storey.type is TypeSpecification))
                    {
                        return new ComObjectMarshalInfoWriter(storey.typeDefinition, storey.marshalType, marshalInfo);
                    }
                    TypeDefinition definition2 = WindowsRuntimeProjections.ProjectToWindowsRuntime(storey.typeDefinition);
                    if ((definition2 != storey.typeDefinition) && WindowsRuntimeProjections.IsSupportedProjectedInterfaceWindowsRuntime(definition2))
                    {
                        return new ComObjectMarshalInfoWriter(definition2, storey.marshalType, marshalInfo);
                    }
                }
                else if (storey.type.MetadataType == MetadataType.GenericInstance)
                {
                    if ((TypeProvider.IReferenceType != null) && storey.type.IsNullable())
                    {
                        TypeReference reference3 = ((GenericInstanceType) storey.type).GenericArguments[0];
                        if (reference3.CanBoxToWindowsRuntime())
                        {
                            return new WindowsRuntimeNullableMarshalInfoWriter(storey.type);
                        }
                    }
                    TypeReference reference4 = WindowsRuntimeProjections.ProjectToWindowsRuntime(storey.type);
                    if (((reference4 != storey.type) && reference4.IsComOrWindowsRuntimeInterface()) && WindowsRuntimeProjections.IsSupportedProjectedInterfaceWindowsRuntime(reference4))
                    {
                        return new ComObjectMarshalInfoWriter(storey.type, storey.marshalType, marshalInfo);
                    }
                }
            }
            if (HasCustomMarshalingMethods(storey.type, storey.nativeType, storey.marshalType, storey.useUnicodeCharSet))
            {
                if (storey.typesForRecursiveFields == null)
                {
                    storey.typesForRecursiveFields = new HashSet<TypeReference>(new Unity.IL2CPP.Common.TypeReferenceEqualityComparer());
                }
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = t => MarshalingUtils.NonStaticFieldsOf(t);
                }
                FieldDefinition faultyField = storey.typeDefinition.GetTypeHierarchy().SelectMany<TypeDefinition, FieldDefinition>(<>f__am$cache0).FirstOrDefault<FieldDefinition>(new Func<FieldDefinition, bool>(storey.<>m__0));
                if (faultyField != null)
                {
                    return new TypeDefinitionWithUnsupportedFieldMarshalInfoWriter(storey.typeDefinition, storey.marshalType, faultyField);
                }
                return new TypeDefinitionMarshalInfoWriter(storey.typeDefinition, storey.marshalType, forFieldMarshaling);
            }
            return new UnmarshalableMarshalInfoWriter(storey.type);
        }

        private static bool FieldIsArrayOfType(FieldDefinition field, TypeReference typeRef)
        {
            ArrayType fieldType = field.FieldType as ArrayType;
            return ((fieldType != null) && new Unity.IL2CPP.Common.TypeReferenceEqualityComparer().Equals(fieldType.ElementType, typeRef));
        }

        private static bool HasCustomMarshalingMethods(TypeReference type, NativeType? nativeType, MarshalType marshalType, bool useUnicodeCharSet)
        {
            if ((type.MetadataType != MetadataType.ValueType) && (type.MetadataType != MetadataType.Class))
            {
                return false;
            }
            TypeDefinition definition = type.Resolve();
            if (definition.HasGenericParameters)
            {
                return false;
            }
            if (definition.IsInterface)
            {
                return false;
            }
            if ((definition.MetadataType == MetadataType.ValueType) && MarshalingUtils.IsBlittable(definition, nativeType, marshalType, useUnicodeCharSet))
            {
                return false;
            }
            if ((marshalType == MarshalType.WindowsRuntime) && (definition.MetadataType != MetadataType.ValueType))
            {
                return false;
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = t => (t.IsSpecialSystemBaseType() || t.IsSequentialLayout) || t.IsExplicitLayout;
            }
            return definition.GetTypeHierarchy().All<TypeDefinition>(<>f__am$cache1);
        }

        public static DefaultMarshalInfoWriter MarshalInfoWriterFor(TypeReference type, MarshalType marshalType, MarshalInfo marshalInfo = null, bool useUnicodeCharSet = false, bool forByReferenceType = false, bool forFieldMarshaling = false, HashSet<TypeReference> typesForRecursiveFields = null)
        {
            if ((((type is TypeSpecification) && !(type is ArrayType)) && (!(type is ByReferenceType) && !(type is PointerType))) && !(type is GenericInstanceType))
            {
                return new UnmarshalableMarshalInfoWriter(type);
            }
            if (((type is GenericParameter) || type.ContainsGenericParameters()) || type.HasGenericParameters)
            {
                return new UnmarshalableMarshalInfoWriter(type);
            }
            return CreateMarshalInfoWriter(type, marshalType, marshalInfo, useUnicodeCharSet, forByReferenceType, forFieldMarshaling, typesForRecursiveFields);
        }

        [CompilerGenerated]
        private sealed class <CreateMarshalInfoWriter>c__AnonStorey0
        {
            internal MarshalType marshalType;
            internal NativeType? nativeType;
            internal TypeReference type;
            internal TypeDefinition typeDefinition;
            internal HashSet<TypeReference> typesForRecursiveFields;
            internal bool useUnicodeCharSet;

            internal bool <>m__0(FieldDefinition field)
            {
                bool flag;
                this.typesForRecursiveFields.Add(this.type);
                try
                {
                    if (this.typesForRecursiveFields.Contains(field.FieldType))
                    {
                        return true;
                    }
                    if (Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(field.FieldType, MarshalDataCollector.TypeProvider.Corlib.MainModule.GetType("System.Runtime.InteropServices.HandleRef"), TypeComparisonMode.Exact))
                    {
                        return true;
                    }
                    if (field.FieldType.IsArray && ((field.MarshalInfo == null) || (field.MarshalInfo.NativeType == NativeType.Array)))
                    {
                        TypeReference elementType = ((ArrayType) field.FieldType).ElementType;
                        bool flag2 = (elementType.MetadataType == MetadataType.ValueType) && MarshalingUtils.IsBlittable(this.type, this.nativeType, this.marshalType, this.useUnicodeCharSet);
                        if ((!elementType.IsPrimitive && !elementType.IsPointer) && (!elementType.IsEnum() && !flag2))
                        {
                            return true;
                        }
                    }
                    if (MarshalDataCollector.FieldIsArrayOfType(field, this.type))
                    {
                        return true;
                    }
                    TypeReference fieldType = field.FieldType;
                    MarshalType marshalType = this.marshalType;
                    MarshalInfo marshalInfo = field.MarshalInfo;
                    bool isUnicodeClass = this.typeDefinition.IsUnicodeClass;
                    HashSet<TypeReference> typesForRecursiveFields = this.typesForRecursiveFields;
                    DefaultMarshalInfoWriter writer = MarshalDataCollector.MarshalInfoWriterFor(fieldType, marshalType, marshalInfo, isUnicodeClass, false, false, typesForRecursiveFields);
                    flag = !writer.CanMarshalTypeToNative() || !writer.CanMarshalTypeFromNative();
                }
                finally
                {
                    this.typesForRecursiveFields.Remove(this.type);
                }
                return flag;
            }
        }
    }
}

