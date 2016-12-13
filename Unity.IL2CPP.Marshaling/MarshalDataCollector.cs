using Mono.Cecil;
using System;
using System.Collections.Generic;
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

		public static DefaultMarshalInfoWriter MarshalInfoWriterFor(TypeReference type, MarshalType marshalType, MarshalInfo marshalInfo = null, bool useUnicodeCharSet = false, bool forByReferenceType = false, bool forFieldMarshaling = false, HashSet<TypeReference> typesForRecursiveFields = null)
		{
			DefaultMarshalInfoWriter result;
			if (type is TypeSpecification && !(type is ArrayType) && !(type is ByReferenceType) && !(type is PointerType) && !(type is GenericInstanceType))
			{
				result = new UnmarshalableMarshalInfoWriter(type);
			}
			else if (type is GenericParameter || type.ContainsGenericParameters() || type.HasGenericParameters)
			{
				result = new UnmarshalableMarshalInfoWriter(type);
			}
			else
			{
				result = MarshalDataCollector.CreateMarshalInfoWriter(type, marshalType, marshalInfo, useUnicodeCharSet, forByReferenceType, forFieldMarshaling, typesForRecursiveFields);
			}
			return result;
		}

		private static DefaultMarshalInfoWriter CreateMarshalInfoWriter(TypeReference type, MarshalType marshalType, MarshalInfo marshalInfo, bool useUnicodeCharSet, bool forByReferenceType, bool forFieldMarshaling, HashSet<TypeReference> typesForRecursiveFields)
		{
			DefaultMarshalInfoWriter result;
			if (type.MetadataType == MetadataType.String || MarshalingUtils.IsStringBuilder(type))
			{
				result = new StringMarshalInfoWriter(type, marshalType, marshalInfo, useUnicodeCharSet, forByReferenceType, forFieldMarshaling);
			}
			else if (type.Resolve().IsDelegate() && (!(type is TypeSpecification) || type is GenericInstanceType))
			{
				if (marshalType == MarshalType.WindowsRuntime)
				{
					if (type is GenericInstanceType)
					{
						result = new UnmarshalableMarshalInfoWriter(type);
					}
					else
					{
						result = new WindowsRuntimeDelegateMarshalInfoWriter(type);
					}
				}
				else
				{
					result = new DelegateMarshalInfoWriter(type);
				}
			}
			else
			{
				NativeType? nativeType = (marshalInfo == null) ? null : new NativeType?(marshalInfo.NativeType);
				if (type.MetadataType == MetadataType.ValueType && MarshalingUtils.IsBlittable(type, nativeType, marshalType))
				{
					if (!forByReferenceType && !forFieldMarshaling && marshalInfo != null && marshalInfo.NativeType == NativeType.LPStruct)
					{
						result = new LPStructMarshalInfoWriter(type, marshalType);
					}
					else
					{
						result = new BlittableStructMarshalInfoWriter(type.Resolve(), marshalType);
					}
				}
				else if (type.IsPrimitive || type.IsPointer || type.IsEnum() || type.MetadataType == MetadataType.Void)
				{
					result = new PrimitiveMarshalInfoWriter(type, marshalInfo, marshalType);
				}
				else if (TypeReferenceEqualityComparer.AreEqual(type, MarshalDataCollector.TypeProvider.Corlib.MainModule.GetType("System.Runtime.InteropServices.HandleRef"), TypeComparisonMode.Exact))
				{
					result = new HandleRefMarshalInfoWriter(type, forByReferenceType);
				}
				else
				{
					ByReferenceType byReferenceType = type as ByReferenceType;
					if (byReferenceType != null)
					{
						TypeReference elementType = byReferenceType.ElementType;
						bool flag = MarshalingUtils.IsBlittable(elementType, nativeType, marshalType);
						if (flag && (elementType.IsValueType() || type.IsPointer))
						{
							result = new BlittableByReferenceMarshalInfoWriter(byReferenceType, marshalType, marshalInfo);
						}
						else
						{
							result = new ByReferenceMarshalInfoWriter(byReferenceType, marshalType, marshalInfo);
						}
					}
					else
					{
						ArrayType arrayType = type as ArrayType;
						if (arrayType != null)
						{
							TypeReference elementType2 = arrayType.ElementType;
							ArrayMarshalInfo arrayMarshalInfo = marshalInfo as ArrayMarshalInfo;
							NativeType? nativeType3 = (arrayMarshalInfo == null) ? null : new NativeType?(arrayMarshalInfo.ElementType);
							if (marshalType != MarshalType.WindowsRuntime)
							{
								if (!MarshalingUtils.IsStringBuilder(elementType2))
								{
									if (elementType2.MetadataType == MetadataType.Class || elementType2.MetadataType == MetadataType.Object || elementType2.MetadataType == MetadataType.Array)
									{
										result = new UnmarshalableMarshalInfoWriter(type);
										return result;
									}
								}
								if (marshalInfo != null && marshalInfo.NativeType == NativeType.SafeArray)
								{
									result = new ComSafeArrayMarshalInfoWriter(arrayType, marshalInfo);
									return result;
								}
								if (marshalInfo != null && marshalInfo.NativeType == NativeType.FixedArray)
								{
									result = new FixedArrayMarshalInfoWriter(arrayType, marshalType, marshalInfo);
									return result;
								}
							}
							if (!forByReferenceType && !forFieldMarshaling && MarshalingUtils.IsBlittable(elementType2, nativeType3, marshalType))
							{
								result = new PinnedArrayMarshalInfoWriter(arrayType, marshalType, marshalInfo);
							}
							else
							{
								result = new LPArrayMarshalInfoWriter(arrayType, marshalType, marshalInfo);
							}
						}
						else
						{
							TypeDefinition type2 = MarshalDataCollector.TypeProvider.Corlib.MainModule.GetType("System.Runtime.InteropServices.SafeHandle");
							if (type.DerivesFrom(type2, false))
							{
								result = new SafeHandleMarshalInfoWriter(type, type2);
							}
							else if (type.IsComOrWindowsRuntimeInterface())
							{
								result = new ComObjectMarshalInfoWriter(type, marshalType, marshalInfo);
							}
							else
							{
								if (type.IsSystemObject())
								{
									if (marshalInfo != null)
									{
										NativeType nativeType2 = marshalInfo.NativeType;
										switch (nativeType2)
										{
										case NativeType.IUnknown:
										case NativeType.IntF:
											goto IL_47C;
										case NativeType.IDispatch:
											IL_46E:
											if (nativeType2 != (NativeType)46)
											{
												goto IL_4A6;
											}
											goto IL_47C;
										case NativeType.Struct:
											result = new ComVariantMarshalInfoWriter(type);
											return result;
										}
										goto IL_46E;
										IL_47C:
										result = new ComObjectMarshalInfoWriter(type, marshalType, marshalInfo);
										return result;
									}
									IL_4A6:
									if (marshalType == MarshalType.WindowsRuntime)
									{
										result = new ComObjectMarshalInfoWriter(type, marshalType, marshalInfo);
										return result;
									}
								}
								TypeDefinition typeDefinition = type.Resolve();
								if (marshalType == MarshalType.WindowsRuntime && typeDefinition.IsWindowsRuntime && !(type is TypeSpecification) && type.MetadataType == MetadataType.Class)
								{
									result = new ComObjectMarshalInfoWriter(type.Resolve(), marshalType, marshalInfo);
								}
								else if (MarshalDataCollector.HasCustomMarshalingMethods(type, nativeType, marshalType))
								{
									if (typesForRecursiveFields == null)
									{
										typesForRecursiveFields = new HashSet<TypeReference>(new TypeReferenceEqualityComparer());
									}
									FieldDefinition fieldDefinition = typeDefinition.GetTypeHierarchy().SelectMany((TypeDefinition t) => MarshalingUtils.NonStaticFieldsOf(t)).FirstOrDefault(delegate(FieldDefinition field)
									{
										typesForRecursiveFields.Add(type);
										bool result2;
										try
										{
											if (typesForRecursiveFields.Contains(field.FieldType))
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
													TypeReference elementType3 = ((ArrayType)field.FieldType).ElementType;
													bool flag2 = elementType3.MetadataType == MetadataType.ValueType && MarshalingUtils.IsBlittable(type, nativeType, marshalType);
													if (!elementType3.IsPrimitive && !elementType3.IsPointer && !elementType3.IsEnum() && !flag2)
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
													TypeReference fieldType = field.FieldType;
													MarshalType marshalType2 = marshalType;
													MarshalInfo marshalInfo2 = field.MarshalInfo;
													bool isUnicodeClass = typeDefinition.IsUnicodeClass;
													HashSet<TypeReference> typesForRecursiveFields2 = typesForRecursiveFields;
													DefaultMarshalInfoWriter defaultMarshalInfoWriter = MarshalDataCollector.MarshalInfoWriterFor(fieldType, marshalType2, marshalInfo2, isUnicodeClass, false, false, typesForRecursiveFields2);
													result2 = (!defaultMarshalInfoWriter.CanMarshalTypeToNative() || !defaultMarshalInfoWriter.CanMarshalTypeFromNative());
												}
											}
										}
										finally
										{
											typesForRecursiveFields.Remove(type);
										}
										return result2;
									});
									if (fieldDefinition != null)
									{
										result = new TypeDefinitionWithUnsupportedFieldMarshalInfoWriter(typeDefinition, marshalType, fieldDefinition);
									}
									else
									{
										result = new TypeDefinitionMarshalInfoWriter(typeDefinition, marshalType, forFieldMarshaling);
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

		private static bool HasCustomMarshalingMethods(TypeReference type, NativeType? nativeType, MarshalType marshalType)
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
				else if (typeDefinition.MetadataType == MetadataType.ValueType && MarshalingUtils.IsBlittable(typeDefinition, nativeType, marshalType))
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
	}
}
