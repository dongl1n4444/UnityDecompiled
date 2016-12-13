using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cecil.Visitor;

namespace Unity.IL2CPP.ILPreProcessor
{
	public sealed class ApplyDefaultMarshalAsAttributeVisitor : Visitor
	{
		protected override void Visit(TypeDefinition typeDefinition, Context context)
		{
			if (!typeDefinition.IsWindowsRuntime)
			{
				ApplyDefaultMarshalAsAttributeVisitor.ProcessFields(typeDefinition);
				ApplyDefaultMarshalAsAttributeVisitor.ProcessMethods(typeDefinition);
			}
		}

		private static void ProcessFields(TypeDefinition type)
		{
			if (!type.IsPrimitive && !type.IsEnum)
			{
				foreach (FieldDefinition current in type.Fields)
				{
					ApplyDefaultMarshalAsAttributeVisitor.ProcessObject(current.FieldType, current, NativeType.IUnknown);
					ApplyDefaultMarshalAsAttributeVisitor.ProcessBoolean(current.FieldType, current, NativeType.Boolean);
				}
			}
		}

		private static void ProcessMethods(TypeDefinition type)
		{
			bool isComInterface = type.IsComInterface();
			foreach (MethodReturnType current in from m in type.Methods
			select m.MethodReturnType)
			{
				if (isComInterface)
				{
					ApplyDefaultMarshalAsAttributeVisitor.ProcessObject(current.ReturnType, current, NativeType.Struct);
				}
				ApplyDefaultMarshalAsAttributeVisitor.ProcessBoolean(current.ReturnType, current, (!isComInterface) ? NativeType.Boolean : NativeType.VariantBool);
			}
			IEnumerable<ParameterDefinition> enumerable = (from m in type.Methods
			where isComInterface || m.IsPInvokeImpl
			select m).SelectMany((MethodDefinition m) => m.Parameters);
			foreach (ParameterDefinition current2 in enumerable)
			{
				ApplyDefaultMarshalAsAttributeVisitor.ProcessObject(current2.ParameterType, current2, NativeType.Struct);
				ApplyDefaultMarshalAsAttributeVisitor.ProcessBoolean(current2.ParameterType, current2, (!isComInterface) ? NativeType.Boolean : NativeType.VariantBool);
			}
		}

		private static void ProcessObject(TypeReference type, IMarshalInfoProvider provider, NativeType nativeType)
		{
			MetadataType metadataType = type.MetadataType;
			if (metadataType != MetadataType.Object)
			{
				if (metadataType != MetadataType.ByReference)
				{
					if (metadataType == MetadataType.Array)
					{
						if (((ArrayType)type).ElementType.MetadataType == MetadataType.Object)
						{
							ArrayMarshalInfo arrayMarshalInfo = provider.MarshalInfo as ArrayMarshalInfo;
							if (arrayMarshalInfo != null && (arrayMarshalInfo.ElementType == NativeType.None || arrayMarshalInfo.ElementType == NativeType.Max))
							{
								arrayMarshalInfo.ElementType = nativeType;
							}
						}
					}
				}
				else
				{
					ApplyDefaultMarshalAsAttributeVisitor.ProcessObject(((ByReferenceType)type).ElementType, provider, nativeType);
				}
			}
			else if (!provider.HasMarshalInfo)
			{
				provider.MarshalInfo = new MarshalInfo(nativeType);
			}
		}

		private static void ProcessBoolean(TypeReference type, IMarshalInfoProvider provider, NativeType nativeType)
		{
			MetadataType metadataType = type.MetadataType;
			if (metadataType != MetadataType.Boolean)
			{
				if (metadataType != MetadataType.ByReference)
				{
					if (metadataType == MetadataType.Array)
					{
						if (((ArrayType)type).ElementType.MetadataType == MetadataType.Boolean)
						{
							ArrayMarshalInfo arrayMarshalInfo = provider.MarshalInfo as ArrayMarshalInfo;
							if (arrayMarshalInfo != null && (arrayMarshalInfo.ElementType == NativeType.None || arrayMarshalInfo.ElementType == NativeType.Max))
							{
								arrayMarshalInfo.ElementType = nativeType;
							}
						}
					}
				}
				else
				{
					ApplyDefaultMarshalAsAttributeVisitor.ProcessBoolean(((ByReferenceType)type).ElementType, provider, nativeType);
				}
			}
			else if (!provider.HasMarshalInfo)
			{
				provider.MarshalInfo = new MarshalInfo(nativeType);
			}
		}
	}
}
