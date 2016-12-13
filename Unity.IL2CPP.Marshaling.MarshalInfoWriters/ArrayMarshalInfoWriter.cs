using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Globalization;
using Unity.IL2CPP.Common;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	public abstract class ArrayMarshalInfoWriter : MarshalableMarshalInfoWriter
	{
		protected enum ArraySizeOptions
		{
			UseArraySize,
			UseSizeParameterIndex,
			UseFirstMarshaledType
		}

		protected readonly ArrayType _arrayType;

		protected readonly int _arraySize;

		protected readonly int _sizeParameterIndex;

		protected readonly ArrayMarshalInfoWriter.ArraySizeOptions _arraySizeSelection;

		protected readonly TypeReference _elementType;

		protected readonly MarshalInfo _marshalInfo;

		protected readonly MarshalType _marshalType;

		protected readonly DefaultMarshalInfoWriter _elementTypeMarshalInfoWriter;

		protected readonly NativeType _nativeElementType;

		protected readonly string _arrayMarshaledTypeName;

		private readonly MarshaledType[] _marshaledTypes;

		public override MarshaledType[] MarshaledTypes
		{
			get
			{
				return this._marshaledTypes;
			}
		}

		public override string NativeSize
		{
			get
			{
				return "-1";
			}
		}

		protected bool NeedsTrailingNullElement
		{
			get
			{
				return this._elementTypeMarshalInfoWriter is StringMarshalInfoWriter && this._marshalType != MarshalType.WindowsRuntime;
			}
		}

		protected ArrayMarshalInfoWriter(ArrayType type, MarshalType marshalType, MarshalInfo marshalInfo) : base(type)
		{
			this._marshalInfo = marshalInfo;
			this._marshalType = marshalType;
			this._arrayType = type;
			this._elementType = type.ElementType;
			MarshalInfo marshalInfo2 = null;
			ArrayMarshalInfo arrayMarshalInfo = marshalInfo as ArrayMarshalInfo;
			FixedArrayMarshalInfo fixedArrayMarshalInfo = marshalInfo as FixedArrayMarshalInfo;
			this._arraySize = 1;
			this._nativeElementType = NativeType.None;
			if (arrayMarshalInfo != null)
			{
				this._arraySize = arrayMarshalInfo.Size;
				this._sizeParameterIndex = arrayMarshalInfo.SizeParameterIndex;
				if (CodeGenOptions.Dotnetprofile == DotNetProfile.Net45)
				{
					if (this._arraySize == 0 || (this._arraySize == -1 && this._sizeParameterIndex >= 0))
					{
						this._arraySizeSelection = ArrayMarshalInfoWriter.ArraySizeOptions.UseSizeParameterIndex;
					}
					else
					{
						this._arraySizeSelection = ArrayMarshalInfoWriter.ArraySizeOptions.UseArraySize;
					}
				}
				else
				{
					this._arraySizeSelection = ((this._arraySize != 0) ? ArrayMarshalInfoWriter.ArraySizeOptions.UseArraySize : ArrayMarshalInfoWriter.ArraySizeOptions.UseSizeParameterIndex);
				}
				this._nativeElementType = arrayMarshalInfo.ElementType;
				marshalInfo2 = new MarshalInfo(this._nativeElementType);
			}
			else if (fixedArrayMarshalInfo != null)
			{
				this._arraySize = fixedArrayMarshalInfo.Size;
				this._nativeElementType = fixedArrayMarshalInfo.ElementType;
				marshalInfo2 = new MarshalInfo(this._nativeElementType);
			}
			if (this._arraySize == -1)
			{
				this._arraySize = 1;
			}
			this._elementTypeMarshalInfoWriter = MarshalDataCollector.MarshalInfoWriterFor(this._elementType, marshalType, marshalInfo2, false, false, true, null);
			if (this._elementTypeMarshalInfoWriter.MarshaledTypes.Length > 1)
			{
				throw new InvalidOperationException(string.Format("ArrayMarshalInfoWriter cannot marshal arrays of {0}.", this._elementType.FullName));
			}
			this._arrayMarshaledTypeName = this._elementTypeMarshalInfoWriter.MarshaledTypes[0].DecoratedName + "*";
			if (marshalType == MarshalType.WindowsRuntime)
			{
				string text = DefaultMarshalInfoWriter.Naming.ForVariable(DefaultMarshalInfoWriter.TypeProvider.UInt32TypeReference);
				this._arraySizeSelection = ArrayMarshalInfoWriter.ArraySizeOptions.UseFirstMarshaledType;
				this._marshaledTypes = new MarshaledType[]
				{
					new MarshaledType(text, text, "ArraySize"),
					new MarshaledType(this._arrayMarshaledTypeName, this._arrayMarshaledTypeName)
				};
			}
			else
			{
				this._marshaledTypes = new MarshaledType[]
				{
					new MarshaledType(this._arrayMarshaledTypeName, this._arrayMarshaledTypeName)
				};
			}
			StringMarshalInfoWriter stringMarshalInfoWriter = this._elementTypeMarshalInfoWriter as StringMarshalInfoWriter;
			if (stringMarshalInfoWriter != null)
			{
				this._nativeElementType = stringMarshalInfoWriter.NativeType;
			}
		}

		public override void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
		{
			this._elementTypeMarshalInfoWriter.WriteMarshaledTypeForwardDeclaration(writer);
		}

		public override void WriteIncludesForFieldDeclaration(CppCodeWriter writer)
		{
			this._elementTypeMarshalInfoWriter.WriteMarshaledTypeForwardDeclaration(writer);
		}

		public override void WriteIncludesForMarshaling(CppCodeWriter writer)
		{
			writer.AddIncludeForTypeDefinition(this._arrayType);
			this._elementTypeMarshalInfoWriter.WriteIncludesForMarshaling(writer);
			base.WriteIncludesForMarshaling(writer);
		}

		public override string WriteMarshalEmptyVariableToNative(CppCodeWriter writer, ManagedMarshalValue variableName, IList<MarshaledParameter> methodParameters)
		{
			string text = string.Format("_{0}_marshaled", variableName.GetNiceName());
			this.WriteNativeVariableDeclarationOfType(writer, text);
			writer.WriteLine("if ({0} != {1})", new object[]
			{
				variableName.Load(),
				DefaultMarshalInfoWriter.Naming.Null
			});
			using (new BlockWriter(writer, false))
			{
				string text2 = this.WriteArraySizeFromManagedArray(writer, variableName, text);
				string text3 = (!this.NeedsTrailingNullElement) ? text2 : string.Format("({0} + 1)", text2);
				writer.WriteLine("{0} = il2cpp_codegen_marshal_allocate_array<{1}>({2});", new object[]
				{
					text,
					this._elementTypeMarshalInfoWriter.MarshaledTypes[0].DecoratedName,
					text3
				});
				writer.WriteLine("memset({0}, 0, {1} * sizeof({2}));", new object[]
				{
					text,
					text3,
					this._elementTypeMarshalInfoWriter.MarshaledTypes[0].DecoratedName
				});
			}
			return text;
		}

		public override void WriteMarshalOutParameterToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IList<MarshaledParameter> methodParameters, IRuntimeMetadataAccess metadataAccess)
		{
			if (this._marshalType == MarshalType.WindowsRuntime)
			{
				writer.WriteLine("if ({0} != {1})", new object[]
				{
					sourceVariable.Load(),
					DefaultMarshalInfoWriter.Naming.Null
				});
				using (new BlockWriter(writer, false))
				{
					this.WriteMarshalToNativeLoop(writer, sourceVariable, destinationVariable, managedVariableName, metadataAccess, (CppCodeWriter bodyWriter) => this.MarshaledArraySizeFor(destinationVariable, methodParameters));
				}
				writer.WriteLine("else");
				using (new BlockWriter(writer, false))
				{
					this.WriteAssignNullArray(writer, destinationVariable);
				}
			}
		}

		public override string WriteMarshalEmptyVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, IRuntimeMetadataAccess metadataAccess)
		{
			string emptyVariableName = string.Format("_{0}_empty", DefaultMarshalInfoWriter.CleanVariableName(variableName));
			ManagedMarshalValue managedMarshalValue = new ManagedMarshalValue(emptyVariableName);
			writer.WriteVariable(this._typeRef, emptyVariableName);
			writer.WriteLine("if ({0} != {1})", new object[]
			{
				variableName,
				DefaultMarshalInfoWriter.Naming.Null
			});
			using (new BlockWriter(writer, false))
			{
				string arraySize = this.MarshaledArraySizeFor(variableName, methodParameters);
				writer.WriteLine(managedMarshalValue.Store("reinterpret_cast<{0}>(SZArrayNew({1}, {2}));", new object[]
				{
					DefaultMarshalInfoWriter.Naming.ForVariable(this._arrayType),
					metadataAccess.TypeInfoFor(this._arrayType),
					arraySize
				}));
				this.WriteLoop(writer, (CppCodeWriter bodyWriter) => arraySize, delegate(CppCodeWriter bodyWriter)
				{
					string value = this._elementTypeMarshalInfoWriter.WriteMarshalEmptyVariableFromNative(bodyWriter, "_item", methodParameters, metadataAccess);
					bodyWriter.WriteLine("{0};", new object[]
					{
						Emit.StoreArrayElement(emptyVariableName, "i", value, false)
					});
				});
			}
			return emptyVariableName;
		}

		private void WriteLoop(CppCodeWriter outerWriter, Func<CppCodeWriter, string> writeLoopCountVariable, Action<CppCodeWriter> writeLoopBody)
		{
			outerWriter.WriteIfNotEmpty(delegate(CppCodeWriter bodyWriter)
			{
				bodyWriter.WriteLine("for (int32_t i = 0; i < {0}; i++)", new object[]
				{
					writeLoopCountVariable(bodyWriter)
				});
				bodyWriter.BeginBlock();
			}, writeLoopBody, delegate(CppCodeWriter bodyWriter)
			{
				bodyWriter.EndBlock(false);
			});
		}

		protected void WriteMarshalToNativeLoop(CppCodeWriter outerWriter, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess, Func<CppCodeWriter, string> writeLoopCountVariable)
		{
			this.WriteLoop(outerWriter, writeLoopCountVariable, delegate(CppCodeWriter bodyWriter)
			{
				this._elementTypeMarshalInfoWriter.WriteMarshalVariableToNative(bodyWriter, new ManagedMarshalValue(sourceVariable, "i"), this._elementTypeMarshalInfoWriter.UndecorateVariable(string.Format("({0})[i]", destinationVariable)), managedVariableName, metadataAccess);
			});
		}

		protected void WriteMarshalFromNativeLoop(CppCodeWriter outerWriter, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess, Func<CppCodeWriter, string> writeLoopCountVariable)
		{
			this.WriteLoop(outerWriter, writeLoopCountVariable, delegate(CppCodeWriter bodyWriter)
			{
				string variableName2 = this._elementTypeMarshalInfoWriter.UndecorateVariable(string.Format("({0})[i]", variableName));
				string value = this._elementTypeMarshalInfoWriter.WriteMarshalVariableFromNative(bodyWriter, variableName2, methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess);
				bodyWriter.WriteLine("{0};", new object[]
				{
					Emit.StoreArrayElement(destinationVariable.Load(), "i", value, false)
				});
			});
		}

		protected void WriteCleanupLoop(CppCodeWriter outerWriter, string variableName, IRuntimeMetadataAccess metadataAccess, Func<CppCodeWriter, string> writeLoopCountVariable)
		{
			this.WriteLoop(outerWriter, writeLoopCountVariable, delegate(CppCodeWriter bodyWriter)
			{
				this._elementTypeMarshalInfoWriter.WriteMarshalCleanupVariable(bodyWriter, this._elementTypeMarshalInfoWriter.UndecorateVariable(string.Format("({0})[i]", variableName)), metadataAccess, null);
			});
		}

		protected void AllocateAndStoreManagedArray(CppCodeWriter writer, ManagedMarshalValue destinationVariable, IRuntimeMetadataAccess metadataAccess, string arraySizeVariable)
		{
			writer.WriteLine(destinationVariable.Store("reinterpret_cast<{0}>(SZArrayNew({1}, {2}))", new object[]
			{
				DefaultMarshalInfoWriter.Naming.ForVariable(this._arrayType),
				metadataAccess.TypeInfoFor(this._arrayType),
				arraySizeVariable
			}));
		}

		protected void AllocateAndStoreNativeArray(CppCodeWriter writer, string destinationVariable, string arraySize)
		{
			if (this.NeedsTrailingNullElement)
			{
				writer.WriteLine("{0} = il2cpp_codegen_marshal_allocate_array<{1}>({2} + 1);", new object[]
				{
					destinationVariable,
					this._elementTypeMarshalInfoWriter.MarshaledTypes[0].DecoratedName,
					arraySize
				});
				writer.WriteLine("({0})[{1}] = {2};", new object[]
				{
					destinationVariable,
					arraySize,
					DefaultMarshalInfoWriter.Naming.Null
				});
			}
			else
			{
				writer.WriteLine("{0} = il2cpp_codegen_marshal_allocate_array<{1}>({2});", new object[]
				{
					destinationVariable,
					this._elementTypeMarshalInfoWriter.MarshaledTypes[0].DecoratedName,
					arraySize
				});
			}
		}

		protected void WriteAssignNullArray(CppCodeWriter writer, string destinationVariable)
		{
			if (this._arraySizeSelection == ArrayMarshalInfoWriter.ArraySizeOptions.UseFirstMarshaledType)
			{
				writer.WriteLine("{0}{1} = {2};", new object[]
				{
					destinationVariable,
					this._marshaledTypes[0].VariableName,
					DefaultMarshalInfoWriter.Naming.Null
				});
			}
			writer.WriteLine("{0} = {1};", new object[]
			{
				destinationVariable,
				DefaultMarshalInfoWriter.Naming.Null
			});
		}

		protected string WriteArraySizeFromManagedArray(CppCodeWriter writer, ManagedMarshalValue managedArray, string nativeArray)
		{
			string result;
			if (this._arraySizeSelection != ArrayMarshalInfoWriter.ArraySizeOptions.UseFirstMarshaledType)
			{
				string text = string.Format("_{0}_Length", managedArray.GetNiceName());
				writer.WriteLine("int32_t {0} = ({1})->max_length;", new object[]
				{
					text,
					managedArray.Load()
				});
				result = text;
			}
			else
			{
				string text = nativeArray + this._marshaledTypes[0].VariableName;
				writer.WriteLine("{0} = static_cast<uint32_t>(({1})->max_length);", new object[]
				{
					text,
					managedArray.Load()
				});
				result = string.Format("static_cast<int32_t>({0})", text);
			}
			return result;
		}

		public abstract override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess);

		public abstract override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess);

		public abstract override void WriteMarshalOutParameterFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess);

		public abstract override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null);

		protected string MarshaledArraySizeFor(string nativeArray, IList<MarshaledParameter> methodParameters)
		{
			string result;
			switch (this._arraySizeSelection)
			{
			case ArrayMarshalInfoWriter.ArraySizeOptions.UseArraySize:
				result = this._arraySize.ToString(CultureInfo.InvariantCulture);
				break;
			case ArrayMarshalInfoWriter.ArraySizeOptions.UseSizeParameterIndex:
			{
				MarshaledParameter marshaledParameter = methodParameters[this._sizeParameterIndex];
				if (marshaledParameter.ParameterType.MetadataType != MetadataType.Int32)
				{
					result = string.Format("static_cast<int32_t>({0})", marshaledParameter.NameInGeneratedCode);
				}
				else
				{
					result = marshaledParameter.NameInGeneratedCode;
				}
				break;
			}
			case ArrayMarshalInfoWriter.ArraySizeOptions.UseFirstMarshaledType:
				result = string.Format("static_cast<int32_t>({0}{1})", nativeArray, this.MarshaledTypes[0].VariableName);
				break;
			default:
				throw new InvalidOperationException(string.Format("Unknown ArraySizeOptions: {0}", this._arraySizeSelection));
			}
			return result;
		}

		public override bool CanMarshalTypeToNative()
		{
			return this._elementTypeMarshalInfoWriter.CanMarshalTypeToNative();
		}

		public override bool CanMarshalTypeFromNative()
		{
			return this._elementTypeMarshalInfoWriter.CanMarshalTypeFromNative();
		}

		public override string GetMarshalingException()
		{
			return this._elementTypeMarshalInfoWriter.GetMarshalingException();
		}
	}
}
