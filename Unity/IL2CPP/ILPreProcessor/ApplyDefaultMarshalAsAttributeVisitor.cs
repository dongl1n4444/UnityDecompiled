namespace Unity.IL2CPP.ILPreProcessor
{
    using Mono.Cecil;
    using Mono.Cecil.Rocks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;

    public sealed class ApplyDefaultMarshalAsAttributeVisitor
    {
        [CompilerGenerated]
        private static Func<MethodDefinition, MethodReturnType> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<MethodDefinition, IEnumerable<ParameterDefinition>> <>f__am$cache1;

        public void Process(AssemblyDefinition assembly)
        {
            foreach (TypeDefinition definition in assembly.MainModule.GetAllTypes())
            {
                if (definition.IsWindowsRuntime)
                {
                    break;
                }
                ProcessFields(definition);
                ProcessMethods(definition);
            }
        }

        private static void ProcessBoolean(TypeReference type, IMarshalInfoProvider provider, NativeType nativeType)
        {
            MetadataType metadataType = type.MetadataType;
            if (metadataType == MetadataType.Boolean)
            {
                if (!provider.HasMarshalInfo)
                {
                    provider.MarshalInfo = new MarshalInfo(nativeType);
                }
            }
            else if (metadataType == MetadataType.ByReference)
            {
                ProcessBoolean(((ByReferenceType) type).ElementType, provider, nativeType);
            }
            else if ((metadataType == MetadataType.Array) && (((ArrayType) type).ElementType.MetadataType == MetadataType.Boolean))
            {
                ArrayMarshalInfo marshalInfo = provider.MarshalInfo as ArrayMarshalInfo;
                if ((marshalInfo != null) && ((marshalInfo.ElementType == NativeType.None) || (marshalInfo.ElementType == NativeType.Max)))
                {
                    marshalInfo.ElementType = nativeType;
                }
            }
        }

        private static void ProcessFields(TypeDefinition type)
        {
            if (!type.IsPrimitive && !type.IsEnum)
            {
                foreach (FieldDefinition definition in type.Fields)
                {
                    ProcessObject(definition.FieldType, definition, NativeType.IUnknown);
                    ProcessBoolean(definition.FieldType, definition, NativeType.Boolean);
                }
            }
        }

        private static void ProcessMethods(TypeDefinition type)
        {
            <ProcessMethods>c__AnonStorey0 storey = new <ProcessMethods>c__AnonStorey0 {
                isComInterface = type.IsComInterface()
            };
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = m => m.MethodReturnType;
            }
            foreach (MethodReturnType type2 in type.Methods.Select<MethodDefinition, MethodReturnType>(<>f__am$cache0))
            {
                if (storey.isComInterface)
                {
                    ProcessObject(type2.ReturnType, type2, NativeType.Struct);
                }
                ProcessBoolean(type2.ReturnType, type2, !storey.isComInterface ? NativeType.Boolean : NativeType.VariantBool);
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = m => m.Parameters;
            }
            IEnumerable<ParameterDefinition> enumerable = type.Methods.Where<MethodDefinition>(new Func<MethodDefinition, bool>(storey.<>m__0)).SelectMany<MethodDefinition, ParameterDefinition>(<>f__am$cache1);
            foreach (ParameterDefinition definition in enumerable)
            {
                ProcessObject(definition.ParameterType, definition, NativeType.Struct);
                ProcessBoolean(definition.ParameterType, definition, !storey.isComInterface ? NativeType.Boolean : NativeType.VariantBool);
            }
        }

        private static void ProcessObject(TypeReference type, IMarshalInfoProvider provider, NativeType nativeType)
        {
            MetadataType metadataType = type.MetadataType;
            if (metadataType == MetadataType.Object)
            {
                if (!provider.HasMarshalInfo)
                {
                    provider.MarshalInfo = new MarshalInfo(nativeType);
                }
            }
            else if (metadataType == MetadataType.ByReference)
            {
                ProcessObject(((ByReferenceType) type).ElementType, provider, nativeType);
            }
            else if ((metadataType == MetadataType.Array) && (((ArrayType) type).ElementType.MetadataType == MetadataType.Object))
            {
                ArrayMarshalInfo marshalInfo = provider.MarshalInfo as ArrayMarshalInfo;
                if ((marshalInfo != null) && ((marshalInfo.ElementType == NativeType.None) || (marshalInfo.ElementType == NativeType.Max)))
                {
                    marshalInfo.ElementType = nativeType;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <ProcessMethods>c__AnonStorey0
        {
            internal bool isComInterface;

            internal bool <>m__0(MethodDefinition m) => 
                (this.isComInterface || m.IsPInvokeImpl);
        }
    }
}

