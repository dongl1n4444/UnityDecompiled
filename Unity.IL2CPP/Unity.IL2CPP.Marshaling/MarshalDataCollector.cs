using Mono.Cecil;
using System;
using System.Linq;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;
using Unity.IL2CPP.Marshaling.MarshalInfoWriters;

namespace Unity.IL2CPP.Marshaling
{
	public class MarshalDataCollector
	{
		[Inject]
		public static ITypeProviderService TypeProvider;

		public static DefaultMarshalInfoWriter MarshalInfoWriterFor(TypeReference type, MarshalType marshalType, MarshalInfo marshalInfo = null, bool useUnicodeCharSet = false, bool forByReferenceType = false)
		{
			DefaultMarshalInfoWriter result;
			if (MarshalDataCollector.IsInflatedGenericDelegate(type))
			{
				result = new DelegateMarshalInfoWriter(type);
			}
			else if (type is TypeSpecification && !(type is ArrayType) && !(type is ByReferenceType) && !(type is PointerType))
			{
				result = new UnmarshalableMarshalInfoWriter(type);
			}
			else if (type is GenericParameter || type.ContainsGenericParameters())
			{
				result = new UnmarshalableMarshalInfoWriter(type);
			}
			else
			{
				result = MarshalDataCollector.CreateMarshalInfoWriter(type, marshalType, marshalInfo, useUnicodeCharSet, forByReferenceType);
			}
			return result;
		}

		private static DefaultMarshalInfoWriter CreateMarshalInfoWriter(TypeReference type, MarshalType marshalType, MarshalInfo marshalInfo, bool useUnicodeCharSet, bool forByReferenceType)
		{
			DefaultMarshalInfoWriter result;
			if (marshalType != MarshalType.WindowsRuntime)
			{
				if (type.MetadataType == MetadataType.String || (type.MetadataType == MetadataType.Class && type.FullName == "System.Text.StringBuilder"))
				{
					result = new StringMarshalInfoWriter(type, marshalType, marshalInfo, useUnicodeCharSet);
					return result;
				}
			}
			if (!(type is TypeSpecification) && type.Resolve().IsDelegate() && marshalType != MarshalType.WindowsRuntime)
			{
				result = new DelegateMarshalInfoWriter(type);
			}
			else if (type.IsPrimitive || type.IsPointer || type.IsEnum() || type.MetadataType == MetadataType.Void)
			{
				result = new PrimitiveMarshalInfoWriter(type, marshalInfo);
			}
			else if (marshalType == MarshalType.WindowsRuntime)
			{
				result = new UnmarshalableMarshalInfoWriter(type);
			}
			else
			{
				if (type.MetadataType == MetadataType.ValueType && MarshalingUtils.IsBlittable(type, marshalInfo))
				{
					if (marshalInfo != null && marshalInfo.NativeType == NativeType.LPStruct)
					{
						result = new LPStructMarshalInfoWriter(type, marshalType);
						return result;
					}
				}
				if (TypeReferenceEqualityComparer.AreEqual(type, MarshalDataCollector.TypeProvider.Corlib.MainModule.GetType("System.Runtime.InteropServices.HandleRef"), TypeComparisonMode.Exact))
				{
					result = new HandleRefMarshalInfoWriter(type, forByReferenceType);
				}
				else
				{
					ByReferenceType byReferenceType = type as ByReferenceType;
					if (byReferenceType != null)
					{
						result = new ByReferenceMarshalInfoWriter(byReferenceType, marshalType, marshalInfo);
					}
					else
					{
						ArrayType arrayType = type as ArrayType;
						if (arrayType != null)
						{
							TypeReference elementType = arrayType.ElementType;
							if (elementType.MetadataType == MetadataType.String && (marshalInfo == null || marshalInfo.NativeType != NativeType.SafeArray))
							{
								result = new StringArrayMarshalInfoWriter(arrayType, marshalType, marshalInfo);
							}
							else if (elementType.MetadataType == MetadataType.Class)
							{
								if (marshalType == MarshalType.COM && (elementType.IsSystemObject() || elementType.IsComOrWindowsRuntimeInterface()))
								{
									result = new ComInterfaceArrayMarshalInfoWriter(arrayType, marshalType, marshalInfo);
								}
								else
								{
									result = new UnmarshalableMarshalInfoWriter(type);
								}
							}
							else if (marshalInfo != null && marshalInfo.NativeType == NativeType.SafeArray)
							{
								result = new ComSafeArrayMarshalInfoWriter(arrayType, marshalInfo);
							}
							else if (forByReferenceType || MarshalDataCollector.IsValueTypeThatRequiresCustomMarshaling(elementType, marshalInfo) || elementType.MetadataType == MetadataType.Char || (marshalInfo != null && marshalInfo.NativeType == NativeType.FixedArray) || MarshalDataCollector.IsFourByteBoolean(elementType, marshalInfo))
							{
								result = new CustomArrayMarshalInfoWriter(arrayType, marshalType, marshalInfo);
							}
							else
							{
								result = new PinnedArrayMarshalInfoWriter(arrayType, marshalType, marshalInfo);
							}
						}
						else
						{
							TypeDefinition type2 = MarshalDataCollector.TypeProvider.Corlib.MainModule.GetType("System.Runtime.InteropServices.SafeHandle");
							if (type.DerivesFrom(type2, false))
							{
								result = new SafeHandleMarshalInfoWriter(type, type2);
							}
							else if (marshalType == MarshalType.COM && type.IsComOrWindowsRuntimeInterface())
							{
								result = new ComInterfaceMarshalInfoWriter(type);
							}
							else
							{
								if (type.IsSystemObject() && marshalInfo != null)
								{
									NativeType nativeType = marshalInfo.NativeType;
									if (nativeType == NativeType.IUnknown || nativeType == NativeType.IntF)
									{
										result = new ComInterfaceMarshalInfoWriter(type);
										return result;
									}
									if (nativeType == NativeType.Struct)
									{
										result = new ComVariantMarshalInfoWriter(type);
										return result;
									}
								}
								if (MarshalDataCollector.HasCustomMarshalingMethods(type, marshalType))
								{
									TypeDefinition typeDefinition = type.Resolve();
									FieldDefinition fieldDefinition = typeDefinition.GetTypeHierarchy().SelectMany((TypeDefinition t) => MarshalingUtils.NonStaticFieldsOf(t)).FirstOrDefault(delegate(FieldDefinition field)
									{
										bool result2;
										if (field.FieldType.MetadataType == MetadataType.Class && !field.FieldType.Resolve().IsDelegate())
										{
											result2 = true;
										}
										else if (TypeReferenceEqualityComparer.AreEqual(field.FieldType, MarshalDataCollector.TypeProvider.Corlib.MainModule.GetType("System.Runtime.InteropServices.HandleRef"), TypeComparisonMode.Exact))
										{
											result2 = true;
										}
										else
										{
											if (field.FieldType.IsArray && (field.MarshalInfo == null || field.MarshalInfo.NativeType == NativeType.Array))
											{
												TypeReference elementType2 = ((ArrayType)field.FieldType).ElementType;
												bool flag = elementType2.MetadataType == MetadataType.ValueType && MarshalingUtils.IsBlittable(type, marshalInfo);
												if (!elementType2.IsPrimitive && !elementType2.IsPointer && !elementType2.IsEnum() && !flag)
												{
													result2 = true;
													return result2;
												}
											}
											if (MarshalDataCollector.FieldIsArrayOfType(field, type))
											{
												result2 = true;
											}
											else
											{
												DefaultMarshalInfoWriter defaultMarshalInfoWriter = MarshalDataCollector.MarshalInfoWriterFor(field.FieldType, marshalType, field.MarshalInfo, typeDefinition.IsUnicodeClass, false);
												result2 = (!defaultMarshalInfoWriter.CanMarshalTypeToNative() || !defaultMarshalInfoWriter.CanMarshalTypeFromNative());
											}
										}
										return result2;
									});
									if (fieldDefinition != null)
									{
										result = new TypeDefinitionWithUnsupportedFieldMarshalInfoWriter(typeDefinition, marshalType, fieldDefinition);
									}
									else
									{
										result = new TypeDefinitionMarshalInfoWriter(typeDefinition, marshalType);
									}
								}
								else
								{
									result = new UnmarshalableMarshalInfoWriter(type);
								}
							}
						}
					}
				}
			}
			return result;
		}

		private static bool IsValueTypeThatRequiresCustomMarshaling(TypeReference elementType, MarshalInfo marshalInfo)
		{
			return elementType.MetadataType == MetadataType.ValueType && !MarshalingUtils.IsBlittable(elementType, marshalInfo);
		}

		private static bool IsFourByteBoolean(TypeReference elementType, MarshalInfo marshalInfo)
		{
			bool result;
			if (elementType.MetadataType != MetadataType.Boolean)
			{
				result = false;
			}
			else
			{
				if (marshalInfo != null)
				{
					if (marshalInfo.NativeType == NativeType.I1 || marshalInfo.NativeType == NativeType.U1)
					{
						result = false;
						return result;
					}
					ArrayMarshalInfo arrayMarshalInfo = marshalInfo as ArrayMarshalInfo;
					if (arrayMarshalInfo != null && (arrayMarshalInfo.ElementType == NativeType.I1 || arrayMarshalInfo.ElementType == NativeType.U1))
					{
						result = false;
						return result;
					}
				}
				result = true;
			}
			return result;
		}

		private static bool HasCustomMarshalingMethods(TypeReference type, MarshalType marshalType)
		{
			bool result;
			if (type.MetadataType != MetadataType.ValueType && type.MetadataType != MetadataType.Class)
			{
				result = false;
			}
			else
			{
				TypeDefinition typeDefinition = type.Resolve();
				if (typeDefinition.HasGenericParameters)
				{
					result = false;
				}
				else if (typeDefinition.IsInterface)
				{
					result = false;
				}
				else
				{
					result = typeDefinition.GetTypeHierarchy().All((TypeDefinition t) => t.IsSpecialSystemBaseType() || t.IsSequentialLayout || t.IsExplicitLayout);
				}
			}
			return result;
		}

		private static bool FieldIsArrayOfType(FieldDefinition field, TypeReference typeRef)
		{
			ArrayType arrayType = field.FieldType as ArrayType;
			return arrayType != null && new TypeReferenceEqualityComparer().Equals(arrayType.ElementType, typeRef);
		}

		private static bool IsInflatedGenericDelegate(TypeReference type)
		{
			return type is GenericInstanceType && type.Resolve().IsDelegate();
		}
	}
}
