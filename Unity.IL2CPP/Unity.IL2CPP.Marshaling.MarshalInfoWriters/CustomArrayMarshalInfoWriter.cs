using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	public class CustomArrayMarshalInfoWriter : ArrayMarshalInfoWriter
	{
		private readonly bool _isByVal;

		public CustomArrayMarshalInfoWriter(ArrayType arrayType, MarshalType marshalType, MarshalInfo marshalInfo) : base(arrayType, marshalType, marshalInfo)
		{
			this._isByVal = (this._marshalInfo != null && this._marshalInfo.NativeType == NativeType.FixedArray);
		}

		public override void WriteFieldDeclaration(CppCodeWriter writer, FieldReference field, string fieldNameSuffix = null)
		{
			if (this._isByVal)
			{
				string text = DefaultMarshalInfoWriter.Naming.ForField(field) + fieldNameSuffix;
				writer.WriteLine("{0} {1}[{2}];", new object[]
				{
					this._elementTypeMarshalInfoWriter.MarshaledDecoratedTypeName,
					text,
					this._arraySize
				});
			}
			else
			{
				base.WriteFieldDeclaration(writer, field, fieldNameSuffix);
			}
		}

		public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			string niceName = sourceVariable.GetNiceName();
			string text = string.Format("_{0}_Length", niceName);
			writer.WriteLine("uint32_t {0} = 0;", new object[]
			{
				text
			});
			writer.WriteLine("if ({0} != NULL)", new object[]
			{
				sourceVariable.Load()
			});
			writer.BeginBlock();
			if (!this._isByVal)
			{
				writer.WriteLine("{0} = ((Il2CppCodeGenArray*){1})->max_length;", new object[]
				{
					text,
					sourceVariable.Load()
				});
				writer.WriteLine("{0} = il2cpp_codegen_marshal_allocate_array<{1}>({2});", new object[]
				{
					destinationVariable,
					this._elementTypeMarshalInfoWriter.MarshaledDecoratedTypeName,
					text
				});
			}
			else
			{
				writer.WriteLine("if ({0} > ((Il2CppCodeGenArray*){1})->max_length)", new object[]
				{
					this._arraySize,
					sourceVariable.Load()
				});
				writer.BeginBlock();
				writer.WriteStatement(Emit.RaiseManagedException("il2cpp_codegen_get_argument_exception(\"\", \"Type could not be marshaled because the length of an embedded array instance does not match the declared length in the layout.\")"));
				writer.EndBlock(false);
				writer.WriteLine();
				writer.WriteLine("{0} = {1};", new object[]
				{
					text,
					this._arraySize
				});
			}
			writer.EndBlock(false);
			writer.WriteLine("for (uint32_t i = 0; i < {0}; i++)", new object[]
			{
				text
			});
			writer.BeginBlock();
			this._elementTypeMarshalInfoWriter.WriteMarshalVariableToNative(writer, new ManagedMarshalValue(sourceVariable, "i"), this._elementTypeMarshalInfoWriter.UndecorateVariable(string.Format("({0})[i]", destinationVariable)), managedVariableName, metadataAccess);
			writer.EndBlock(false);
		}

		public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			if (!this._isByVal)
			{
				writer.WriteLine("if ({0} != NULL)", new object[]
				{
					variableName
				});
			}
			writer.BeginBlock();
			writer.WriteLine("uint32_t {0};", new object[]
			{
				"_loopCount"
			});
			if (!this._isByVal)
			{
				writer.WriteLine("if ({0} == NULL)", new object[]
				{
					destinationVariable.Load()
				});
				writer.BeginBlock();
			}
			writer.WriteLine("{0} = {1};", new object[]
			{
				"_loopCount",
				base.MarshaledArraySizeFor(methodParameters)
			});
			writer.WriteLine(destinationVariable.Store("reinterpret_cast<{0}>(SZArrayNew({1}, {2}))", new object[]
			{
				DefaultMarshalInfoWriter.Naming.ForVariable(this._arrayType),
				metadataAccess.TypeInfoFor(this._arrayType),
				"_loopCount"
			}));
			if (!this._isByVal)
			{
				writer.EndBlock(false);
				writer.WriteLine("else");
				writer.BeginBlock();
				writer.WriteLine("{0} = ((Il2CppCodeGenArray*){1})->max_length;", new object[]
				{
					"_loopCount",
					destinationVariable.Load()
				});
				writer.EndBlock(false);
			}
			writer.WriteLine("for (uint32_t i = 0; i < {0}; i++)", new object[]
			{
				"_loopCount"
			});
			writer.BeginBlock();
			writer.WriteVariable(this._elementType, "_item");
			this._elementTypeMarshalInfoWriter.WriteMarshalVariableFromNative(writer, this._elementTypeMarshalInfoWriter.UndecorateVariable(string.Format("({0})[i]", variableName)), new ManagedMarshalValue("_item"), methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess);
			writer.WriteLine("{0};", new object[]
			{
				Emit.StoreArrayElement(destinationVariable.Load(), "i", "_item")
			});
			writer.EndBlock(false);
			writer.EndBlock(false);
		}

		public override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
		{
			string arg = variableName.Replace("*", string.Empty).Replace('.', '_');
			string text = string.Format("{0}_CleanupLoopCount", arg);
			string text2 = (!this._isByVal && managedVariableName != null) ? string.Format("({0} != NULL) ? ((Il2CppCodeGenArray*){0})->max_length : 0", managedVariableName) : string.Format("{0}", this._arraySize);
			writer.WriteLine("const uint32_t {0} = {1};", new object[]
			{
				text,
				text2
			});
			writer.WriteLine("for (uint32_t i = 0; i < {0}; i++)", new object[]
			{
				text
			});
			writer.BeginBlock();
			this._elementTypeMarshalInfoWriter.WriteMarshalCleanupVariable(writer, this._elementTypeMarshalInfoWriter.UndecorateVariable(string.Format("({0})[i]", variableName)), metadataAccess, null);
			writer.EndBlock(false);
			if (!this._isByVal)
			{
				writer.WriteLine("il2cpp_codegen_marshal_free({0});", new object[]
				{
					variableName
				});
				writer.WriteLine("{0} = NULL;", new object[]
				{
					variableName
				});
			}
		}

		public override void WriteIncludesForFieldDeclaration(CppCodeWriter writer)
		{
			base.WriteIncludesForFieldDeclaration(writer);
			if (this._isByVal)
			{
				writer.AddIncludeForTypeDefinition(this._elementType);
			}
		}
	}
}
