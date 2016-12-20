namespace Unity.IL2CPP.Marshaling
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Marshaling.MarshalInfoWriters;

    public static class MarshalingUtils
    {
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<TypeDefinition, IEnumerable<FieldDefinition>> <>f__am$cache1;
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static IWindowsRuntimeProjections WindowsRuntimeProjections;

        private static bool AreFieldsBlittable(TypeDefinition typeDef, NativeType? nativeType, MarshalType marshalType)
        {
            <AreFieldsBlittable>c__AnonStorey0 storey = new <AreFieldsBlittable>c__AnonStorey0 {
                marshalType = marshalType
            };
            if (typeDef.IsPrimitive)
            {
                return IsPrimitiveBlittable(typeDef, nativeType, storey.marshalType);
            }
            return Enumerable.All<FieldDefinition>(typeDef.Fields, new Func<FieldDefinition, bool>(storey, (IntPtr) this.<>m__0));
        }

        public static IEnumerable<DefaultMarshalInfoWriter> GetFieldMarshalInfoWriters(TypeDefinition type, MarshalType marshalType)
        {
            <GetFieldMarshalInfoWriters>c__AnonStorey2 storey = new <GetFieldMarshalInfoWriters>c__AnonStorey2 {
                marshalType = marshalType,
                type = type
            };
            return Enumerable.Select<FieldDefinition, DefaultMarshalInfoWriter>(GetMarshaledFields(storey.type, storey.marshalType), new Func<FieldDefinition, DefaultMarshalInfoWriter>(storey, (IntPtr) this.<>m__0));
        }

        private static NativeType? GetFieldNativeType(FieldDefinition field)
        {
            if (field.MarshalInfo == null)
            {
                return null;
            }
            return new NativeType?(field.MarshalInfo.NativeType);
        }

        public static IEnumerable<FieldDefinition> GetMarshaledFields(TypeDefinition type, MarshalType marshalType)
        {
            <GetMarshaledFields>c__AnonStorey1 storey = new <GetMarshaledFields>c__AnonStorey1 {
                type = type,
                marshalType = marshalType
            };
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<TypeDefinition, IEnumerable<FieldDefinition>>(null, (IntPtr) <GetMarshaledFields>m__1);
            }
            return Enumerable.SelectMany<TypeDefinition, FieldDefinition>(Enumerable.Where<TypeDefinition>(Extensions.GetTypeHierarchy(storey.type), new Func<TypeDefinition, bool>(storey, (IntPtr) this.<>m__0)), <>f__am$cache1);
        }

        public static MarshalType[] GetMarshalTypesForMarshaledType(TypeDefinition type)
        {
            if ((type.IsWindowsRuntime || (WindowsRuntimeProjections.ProjectToWindowsRuntime(type) != type)) && ((type.MetadataType == MetadataType.ValueType) || Extensions.IsDelegate(type)))
            {
                return new MarshalType[] { MarshalType.PInvoke };
            }
            MarshalType[] typeArray1 = new MarshalType[2];
            typeArray1[1] = MarshalType.COM;
            return typeArray1;
        }

        internal static bool IsBlittable(TypeDefinition type, NativeType? nativeType, MarshalType marshalType)
        {
            if (type.HasGenericParameters)
            {
                return false;
            }
            if (type.IsEnum)
            {
                return IsPrimitiveBlittable(Extensions.GetUnderlyingEnumType(type).Resolve(), nativeType, marshalType);
            }
            return ((type.IsSequentialLayout || type.IsExplicitLayout) && AreFieldsBlittable(type, nativeType, marshalType));
        }

        internal static bool IsBlittable(TypeReference type, NativeType? nativeType, MarshalType marshalType)
        {
            if (type.IsGenericInstance)
            {
                return false;
            }
            if (type is TypeSpecification)
            {
                return false;
            }
            if (type is ArrayType)
            {
                return false;
            }
            return IsBlittable(type.Resolve(), nativeType, marshalType);
        }

        private static bool IsPrimitiveBlittable(TypeDefinition type, NativeType? nativeType, MarshalType marshalType)
        {
            if (!nativeType.HasValue)
            {
                return ((marshalType == MarshalType.WindowsRuntime) || ((type.MetadataType != MetadataType.Boolean) && (type.MetadataType != MetadataType.Char)));
            }
            switch (type.MetadataType)
            {
                case MetadataType.Boolean:
                case MetadataType.SByte:
                case MetadataType.Byte:
                    return (((((NativeType) nativeType.GetValueOrDefault()) == NativeType.U1) && nativeType.HasValue) || ((((NativeType) nativeType.GetValueOrDefault()) == NativeType.I1) && nativeType.HasValue));

                case MetadataType.Char:
                case MetadataType.Int16:
                case MetadataType.UInt16:
                    return (((((NativeType) nativeType.GetValueOrDefault()) == NativeType.U2) && nativeType.HasValue) || ((((NativeType) nativeType.GetValueOrDefault()) == NativeType.I2) && nativeType.HasValue));

                case MetadataType.Int32:
                case MetadataType.UInt32:
                    return (((((NativeType) nativeType.GetValueOrDefault()) == NativeType.U4) && nativeType.HasValue) || ((((NativeType) nativeType.GetValueOrDefault()) == NativeType.I4) && nativeType.HasValue));

                case MetadataType.Int64:
                case MetadataType.UInt64:
                    return (((((NativeType) nativeType.GetValueOrDefault()) == NativeType.U8) && nativeType.HasValue) || ((((NativeType) nativeType.GetValueOrDefault()) == NativeType.I8) && nativeType.HasValue));

                case MetadataType.Single:
                    return ((((NativeType) nativeType.GetValueOrDefault()) == NativeType.R4) && nativeType.HasValue);

                case MetadataType.Double:
                    return ((((NativeType) nativeType.GetValueOrDefault()) == NativeType.R8) && nativeType.HasValue);

                case MetadataType.IntPtr:
                case MetadataType.UIntPtr:
                    return (((((NativeType) nativeType.GetValueOrDefault()) == NativeType.UInt) && nativeType.HasValue) || ((((NativeType) nativeType.GetValueOrDefault()) == NativeType.Int) && nativeType.HasValue));
            }
            throw new ArgumentException(string.Format("{0} is not a primitive!", type.FullName));
        }

        internal static bool IsStringBuilder(TypeReference type)
        {
            return ((type.MetadataType == MetadataType.Class) && (type.FullName == "System.Text.StringBuilder"));
        }

        public static string MarshalTypeToNiceString(MarshalType marshalType)
        {
            switch (marshalType)
            {
                case MarshalType.PInvoke:
                    return "P/Invoke";

                case MarshalType.COM:
                    return "COM";

                case MarshalType.WindowsRuntime:
                    return "Windows Runtime";
            }
            throw new ArgumentException(string.Format("Unexpected MarshalType value '{0}'.", marshalType), "marshalType");
        }

        public static string MarshalTypeToString(MarshalType marshalType)
        {
            switch (marshalType)
            {
                case MarshalType.PInvoke:
                    return "pinvoke";

                case MarshalType.COM:
                    return "com";

                case MarshalType.WindowsRuntime:
                    return "windows_runtime";
            }
            throw new ArgumentException(string.Format("Unexpected MarshalType value '{0}'.", marshalType), "marshalType");
        }

        internal static IEnumerable<FieldDefinition> NonStaticFieldsOf(TypeDefinition typeDefinition)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<FieldDefinition, bool>(null, (IntPtr) <NonStaticFieldsOf>m__0);
            }
            return Enumerable.Where<FieldDefinition>(typeDefinition.Fields, <>f__am$cache0);
        }

        internal static bool UseUnicodeAsDefaultMarshalingForStringParameters(MethodReference method)
        {
            MethodDefinition definition = method.Resolve();
            return (definition.HasPInvokeInfo && definition.PInvokeInfo.IsCharSetUnicode);
        }

        [CompilerGenerated]
        private sealed class <AreFieldsBlittable>c__AnonStorey0
        {
            internal MarshalType marshalType;

            internal bool <>m__0(FieldDefinition field)
            {
                return (field.IsStatic || (Extensions.IsValueType(field.FieldType) && MarshalingUtils.IsBlittable(field.FieldType, MarshalingUtils.GetFieldNativeType(field), this.marshalType)));
            }
        }

        [CompilerGenerated]
        private sealed class <GetFieldMarshalInfoWriters>c__AnonStorey2
        {
            internal MarshalType marshalType;
            internal TypeDefinition type;

            internal DefaultMarshalInfoWriter <>m__0(FieldDefinition f)
            {
                return MarshalDataCollector.MarshalInfoWriterFor(f.FieldType, this.marshalType, f.MarshalInfo, this.type.IsUnicodeClass, false, true, null);
            }
        }

        [CompilerGenerated]
        private sealed class <GetMarshaledFields>c__AnonStorey1
        {
            internal MarshalType marshalType;
            internal TypeDefinition type;

            internal bool <>m__0(TypeDefinition t)
            {
                return ((t == this.type) || MarshalDataCollector.MarshalInfoWriterFor(t, this.marshalType, null, false, false, false, null).HasNativeStructDefinition);
            }
        }
    }
}

