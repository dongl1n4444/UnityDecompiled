using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	internal sealed class PrimitiveMarshalInfoWriter : DefaultMarshalInfoWriter
	{
		private readonly int _nativeSizeWithoutPointers;

		private readonly string _nativeSize;

		private readonly string _marshaledTypeName;

		public override int NativeSizeWithoutPointers
		{
			get
			{
				return this._nativeSizeWithoutPointers;
			}
		}

		public override string NativeSize
		{
			get
			{
				return this._nativeSize;
			}
		}

		public override string MarshaledTypeName
		{
			get
			{
				return this._marshaledTypeName;
			}
		}

		public PrimitiveMarshalInfoWriter(TypeReference type, MarshalInfo marshalInfo) : base(type)
		{
			this._marshaledTypeName = DefaultMarshalInfoWriter.Naming.ForVariable(type);
			this._nativeSize = base.NativeSize;
			switch (type.MetadataType)
			{
			case MetadataType.Void:
				this._nativeSizeWithoutPointers = 1;
				this._nativeSize = "1";
				break;
			case MetadataType.Boolean:
				this._nativeSizeWithoutPointers = 4;
				this._nativeSize = "4";
				this._marshaledTypeName = "int32_t";
				break;
			case MetadataType.Char:
				this._nativeSizeWithoutPointers = 1;
				this._nativeSize = "1";
				this._marshaledTypeName = "char";
				break;
			case MetadataType.SByte:
			case MetadataType.Byte:
				this._nativeSizeWithoutPointers = 1;
				break;
			case MetadataType.Int16:
			case MetadataType.UInt16:
				this._nativeSizeWithoutPointers = 2;
				break;
			case MetadataType.Int32:
			case MetadataType.UInt32:
			case MetadataType.Single:
				this._nativeSizeWithoutPointers = 4;
				break;
			case MetadataType.Int64:
			case MetadataType.UInt64:
			case MetadataType.Double:
				this._nativeSizeWithoutPointers = 8;
				break;
			case MetadataType.Pointer:
				this._nativeSizeWithoutPointers = 0;
				break;
			case MetadataType.IntPtr:
				this._marshaledTypeName = DefaultMarshalInfoWriter.Naming.ForIntPtrT;
				this._nativeSizeWithoutPointers = 0;
				break;
			case MetadataType.UIntPtr:
				this._marshaledTypeName = DefaultMarshalInfoWriter.Naming.ForUIntPtrT;
				this._nativeSizeWithoutPointers = 0;
				break;
			}
			if (marshalInfo != null)
			{
				NativeType nativeType = marshalInfo.NativeType;
				switch (nativeType)
				{
				case NativeType.Boolean:
				case NativeType.I4:
					this._nativeSize = "4";
					this._nativeSizeWithoutPointers = 4;
					this._marshaledTypeName = "int32_t";
					break;
				case NativeType.I1:
					this._nativeSize = "1";
					this._nativeSizeWithoutPointers = 1;
					this._marshaledTypeName = "int8_t";
					break;
				case NativeType.U1:
					this._nativeSize = "1";
					this._nativeSizeWithoutPointers = 1;
					this._marshaledTypeName = "uint8_t";
					break;
				case NativeType.I2:
					this._nativeSize = "2";
					this._nativeSizeWithoutPointers = 2;
					this._marshaledTypeName = "int16_t";
					break;
				case NativeType.U2:
					this._nativeSize = "2";
					this._nativeSizeWithoutPointers = 2;
					this._marshaledTypeName = "uint16_t";
					break;
				case NativeType.U4:
					this._nativeSize = "4";
					this._nativeSizeWithoutPointers = 4;
					this._marshaledTypeName = "uint32_t";
					break;
				case NativeType.I8:
					this._nativeSize = "8";
					this._nativeSizeWithoutPointers = 8;
					this._marshaledTypeName = "int64_t";
					break;
				case NativeType.U8:
					this._nativeSize = "8";
					this._nativeSizeWithoutPointers = 8;
					this._marshaledTypeName = "uint64_t";
					break;
				case NativeType.R4:
					this._nativeSize = "4";
					this._nativeSizeWithoutPointers = 4;
					this._marshaledTypeName = "float";
					break;
				case NativeType.R8:
					this._nativeSize = "8";
					this._nativeSizeWithoutPointers = 8;
					this._marshaledTypeName = "double";
					break;
				default:
					if (nativeType != NativeType.Int)
					{
						if (nativeType != NativeType.UInt)
						{
							if (nativeType == NativeType.VariantBool)
							{
								this._nativeSize = "2";
								this._nativeSizeWithoutPointers = 2;
								this._marshaledTypeName = "IL2CPP_VARIANT_BOOL";
							}
						}
						else
						{
							this._nativeSize = "sizeof(void*)";
							this._nativeSizeWithoutPointers = 0;
							this._marshaledTypeName = DefaultMarshalInfoWriter.Naming.ForUIntPtrT;
						}
					}
					else
					{
						this._nativeSize = "sizeof(void*)";
						this._nativeSizeWithoutPointers = 0;
						this._marshaledTypeName = DefaultMarshalInfoWriter.Naming.ForIntPtrT;
					}
					break;
				}
			}
		}

		public override void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
		{
		}

		public override void WriteIncludesForFieldDeclaration(CppCodeWriter writer)
		{
		}

		public override void WriteIncludesForMarshaling(CppCodeWriter writer)
		{
		}

		public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			if (this._typeRef.MetadataType == MetadataType.IntPtr || this._typeRef.MetadataType == MetadataType.UIntPtr)
			{
				string fieldManagedName = (this._typeRef.MetadataType != MetadataType.IntPtr) ? DefaultMarshalInfoWriter.Naming.UIntPtrPointerField : DefaultMarshalInfoWriter.Naming.IntPtrValueField;
				string text = DefaultMarshalInfoWriter.Naming.ForFieldGetter(this._typeRef.Resolve().Fields.First((FieldDefinition f) => f.Name == fieldManagedName));
				if (this._marshaledTypeName == "intptr_t")
				{
					writer.WriteLine("{0} = reinterpret_cast<intptr_t>(({1}).{2}());", new object[]
					{
						destinationVariable,
						sourceVariable.Load(),
						text
					});
				}
				else
				{
					writer.WriteLine("{0} = static_cast<{3}>(reinterpret_cast<intptr_t>(({1}).{2}()));", new object[]
					{
						destinationVariable,
						sourceVariable.Load(),
						text,
						this._marshaledTypeName
					});
				}
			}
			else if (this._typeRef.MetadataType == MetadataType.Boolean && this._marshaledTypeName == "IL2CPP_VARIANT_BOOL")
			{
				writer.WriteLine("{0} = {1};", new object[]
				{
					destinationVariable,
					PrimitiveMarshalInfoWriter.MarshalVariantBoolToNative(sourceVariable.Load())
				});
			}
			else if (this._typeRef.MetadataType == MetadataType.Char)
			{
				writer.WriteLine("{1} = {0};", new object[]
				{
					PrimitiveMarshalInfoWriter.MarshalCharToNative(sourceVariable.Load()),
					destinationVariable
				});
			}
			else
			{
				base.WriteMarshalVariableToNative(writer, sourceVariable, destinationVariable, managedVariableName, metadataAccess);
			}
		}

		public override string WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			string result;
			if (this._typeRef.MetadataType == MetadataType.IntPtr || this._typeRef.MetadataType == MetadataType.UIntPtr)
			{
				string fieldManagedName = (this._typeRef.MetadataType != MetadataType.IntPtr) ? DefaultMarshalInfoWriter.Naming.UIntPtrPointerField : DefaultMarshalInfoWriter.Naming.IntPtrValueField;
				string arg = DefaultMarshalInfoWriter.Naming.ForFieldGetter(this._typeRef.Resolve().Fields.First((FieldDefinition f) => f.Name == fieldManagedName));
				if (this._marshaledTypeName == "intptr_t")
				{
					result = string.Format("reinterpret_cast<intptr_t>({0}.{1}())", sourceVariable.Load(), arg);
				}
				else
				{
					result = string.Format("static_cast<{2}>(reinterpret_cast<intptr_t>({0}.{1}()))", sourceVariable.Load(), arg, this._marshaledTypeName);
				}
			}
			else if (this._typeRef.MetadataType == MetadataType.Boolean && this._marshaledTypeName == "IL2CPP_VARIANT_BOOL")
			{
				result = PrimitiveMarshalInfoWriter.MarshalVariantBoolToNative(sourceVariable.Load());
			}
			else if (this._typeRef.MetadataType == MetadataType.Char)
			{
				result = PrimitiveMarshalInfoWriter.MarshalCharToNative(sourceVariable.Load());
			}
			else
			{
				result = base.WriteMarshalVariableToNative(writer, sourceVariable, managedVariableName, metadataAccess);
			}
			return result;
		}

		public override string WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			string text = string.Format("_{0}_unmarshaled", variableName);
			string result;
			if (this._typeRef.MetadataType == MetadataType.Char)
			{
				writer.WriteLine("uint16_t {0} = static_cast<Il2CppChar>(static_cast<uint8_t>({1}));", new object[]
				{
					text,
					variableName
				});
				result = text;
			}
			else if (this._typeRef.MetadataType == MetadataType.Boolean && this._marshaledTypeName == "IL2CPP_VARIANT_BOOL")
			{
				writer.WriteLine("bool {0} = {1};", new object[]
				{
					text,
					PrimitiveMarshalInfoWriter.MarshalVariantBoolFromNative(variableName)
				});
				result = text;
			}
			else if (this._typeRef.MetadataType == MetadataType.IntPtr || this._typeRef.MetadataType == MetadataType.UIntPtr)
			{
				writer.WriteLine("{0} {1};", new object[]
				{
					DefaultMarshalInfoWriter.Naming.ForVariable(this._typeRef),
					text
				});
				this.WriteIntPtrFieldAssignmentToLocalVariable(writer, variableName, text);
				result = text;
			}
			else
			{
				result = base.WriteMarshalVariableFromNative(writer, variableName, methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess);
			}
			return result;
		}

		public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			if (this._typeRef.MetadataType == MetadataType.Char)
			{
				writer.WriteLine(destinationVariable.Store("static_cast<Il2CppChar>(static_cast<uint8_t>({0}))", new object[]
				{
					variableName
				}));
			}
			else if (this._typeRef.MetadataType == MetadataType.Boolean && this._marshaledTypeName == "IL2CPP_VARIANT_BOOL")
			{
				writer.WriteLine(destinationVariable.Store(PrimitiveMarshalInfoWriter.MarshalVariantBoolFromNative(variableName)));
			}
			else if (this._typeRef.MetadataType == MetadataType.IntPtr || this._typeRef.MetadataType == MetadataType.UIntPtr)
			{
				this.WriteIntPtrFieldAssignmentToManagedValue(writer, variableName, destinationVariable);
			}
			else
			{
				base.WriteMarshalVariableFromNative(writer, variableName, destinationVariable, methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess);
			}
		}

		private static string MarshalVariantBoolToNative(string variableName)
		{
			return string.Format("(({0}) ? IL2CPP_VARIANT_TRUE : IL2CPP_VARIANT_FALSE)", variableName);
		}

		private static string MarshalVariantBoolFromNative(string variableName)
		{
			return string.Format("(({0}) != IL2CPP_VARIANT_FALSE)", variableName);
		}

		private static string MarshalCharToNative(string variableName)
		{
			return string.Format("static_cast<uint8_t>({0})", variableName);
		}

		private void WriteIntPtrFieldAssignmentToLocalVariable(CppCodeWriter writer, string variableName, string destinationVariable)
		{
			writer.WriteLine("{0}.{1}(reinterpret_cast<void*>(({2}){3}));", new object[]
			{
				destinationVariable,
				this.GetIntPtrValueSetterName(),
				this.GetIntPtrTypeName(),
				variableName
			});
		}

		private void WriteIntPtrFieldAssignmentToManagedValue(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable)
		{
			string text = destinationVariable.GetNiceName() + "_temp";
			writer.WriteLine("{0} {1};", new object[]
			{
				DefaultMarshalInfoWriter.Naming.ForType(this._typeRef),
				text
			});
			this.WriteIntPtrFieldAssignmentToLocalVariable(writer, variableName, text);
			writer.WriteLine(destinationVariable.Store(text));
		}

		private string GetIntPtrTypeName()
		{
			return (this._typeRef.MetadataType != MetadataType.IntPtr) ? DefaultMarshalInfoWriter.Naming.ForUIntPtrT : DefaultMarshalInfoWriter.Naming.ForIntPtrT;
		}

		private string GetIntPtrValueSetterName()
		{
			string fieldManagedName = (this._typeRef.MetadataType != MetadataType.IntPtr) ? DefaultMarshalInfoWriter.Naming.UIntPtrPointerField : DefaultMarshalInfoWriter.Naming.IntPtrValueField;
			return DefaultMarshalInfoWriter.Naming.ForFieldSetter(this._typeRef.Resolve().Fields.First((FieldDefinition f) => f.Name == fieldManagedName));
		}

		public override void WriteNativeVariableDeclarationOfType(CppCodeWriter writer, string variableName)
		{
			if (this._typeRef.IsPointer)
			{
				base.WriteNativeVariableDeclarationOfType(writer, variableName);
			}
			else
			{
				string text = "0";
				string marshaledTypeName = this._marshaledTypeName;
				if (marshaledTypeName != null)
				{
					if (!(marshaledTypeName == "float"))
					{
						if (!(marshaledTypeName == "double"))
						{
							if (marshaledTypeName == "IL2CPP_VARIANT_BOOL")
							{
								text = "IL2CPP_VARIANT_FALSE";
							}
						}
						else
						{
							text = "0.0";
						}
					}
					else
					{
						text = "0.0f";
					}
				}
				writer.WriteLine("{0} {1} = {2};", new object[]
				{
					this._marshaledTypeName,
					variableName,
					text
				});
			}
		}
	}
}
