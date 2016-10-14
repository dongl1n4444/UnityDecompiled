using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	internal sealed class StringMarshalInfoWriter : MarshalableMarshalInfoWriter
	{
		private readonly string _marshaledTypeName;

		private readonly NativeType _nativeType;

		private readonly bool _isStringBuilder;

		private readonly MarshalInfo _marshalInfo;

		private readonly bool _useUnicodeCharSet;

		public override string MarshaledTypeName
		{
			get
			{
				return this._marshaledTypeName;
			}
		}

		public NativeType NativeType
		{
			get
			{
				return this._nativeType;
			}
		}

		public override int NativeSizeWithoutPointers
		{
			get
			{
				int result;
				if (this.IsFixedSizeString)
				{
					result = ((FixedSysStringMarshalInfo)this._marshalInfo).Size * this.BytesPerCharacter;
				}
				else
				{
					result = base.NativeSizeWithoutPointers;
				}
				return result;
			}
		}

		private bool IsFixedSizeString
		{
			get
			{
				return this._nativeType == NativeType.FixedSysString;
			}
		}

		private bool IsWideString
		{
			get
			{
				return this._nativeType == NativeType.LPWStr || this._nativeType == NativeType.BStr || (this.IsFixedSizeString && this._useUnicodeCharSet);
			}
		}

		private int BytesPerCharacter
		{
			get
			{
				return (!this.IsWideString) ? 1 : 2;
			}
		}

		public StringMarshalInfoWriter(TypeReference type, MarshalType marshalType, MarshalInfo marshalInfo, bool useUnicodeCharSet) : base(type)
		{
			this._isStringBuilder = (type.MetadataType == MetadataType.Class && type.MetadataType != MetadataType.String);
			this._useUnicodeCharSet = useUnicodeCharSet;
			if (marshalInfo != null)
			{
				this._nativeType = marshalInfo.NativeType;
			}
			else if (marshalType != MarshalType.COM)
			{
				this._nativeType = ((!this._useUnicodeCharSet) ? NativeType.LPStr : NativeType.LPWStr);
			}
			else
			{
				this._nativeType = NativeType.None;
			}
			if (this._nativeType != NativeType.LPStr && this._nativeType != NativeType.LPWStr && this._nativeType != NativeType.BStr && this._nativeType != NativeType.FixedSysString)
			{
				this._nativeType = ((marshalType != MarshalType.COM) ? NativeType.LPStr : NativeType.BStr);
			}
			this._marshaledTypeName = ((!this.IsWideString) ? "char*" : "Il2CppChar*");
			this._marshalInfo = marshalInfo;
		}

		public override void WriteFieldDeclaration(CppCodeWriter writer, FieldReference field, string fieldNameSuffix = null)
		{
			if (this.IsFixedSizeString)
			{
				writer.WriteLine("{0} {1}[{2}];", new object[]
				{
					this._marshaledTypeName.Replace("*", ""),
					DefaultMarshalInfoWriter.Naming.ForField(field) + fieldNameSuffix,
					((FixedSysStringMarshalInfo)this._marshalInfo).Size
				});
			}
			else
			{
				base.WriteFieldDeclaration(writer, field, fieldNameSuffix);
			}
		}

		public override void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
		{
		}

		public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			if (this.IsFixedSizeString)
			{
				string text = (!this.IsWideString) ? "il2cpp_codegen_marshal_string_fixed" : "il2cpp_codegen_marshal_wstring_fixed";
				writer.WriteLine("{0}({1}, ({2})&{3}, {4});", new object[]
				{
					text,
					sourceVariable.Load(),
					this._marshaledTypeName,
					destinationVariable,
					((FixedSysStringMarshalInfo)this._marshalInfo).Size
				});
			}
			else
			{
				string text;
				if (this._isStringBuilder)
				{
					text = ((!this.IsWideString) ? "il2cpp_codegen_marshal_string_builder" : "il2cpp_codegen_marshal_wstring_builder");
				}
				else if (this._nativeType == NativeType.BStr)
				{
					text = "il2cpp_codegen_marshal_bstring";
				}
				else
				{
					text = ((!this.IsWideString) ? "il2cpp_codegen_marshal_string" : "il2cpp_codegen_marshal_wstring");
				}
				writer.WriteLine("{0} = {1}({2});", new object[]
				{
					destinationVariable,
					text,
					sourceVariable.Load()
				});
			}
		}

		public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			if (this._isStringBuilder)
			{
				string text = (!this.IsWideString) ? "il2cpp_codegen_marshal_string_builder_result" : "il2cpp_codegen_marshal_wstring_builder_result";
				writer.WriteLine("{0}({1}, {2});", new object[]
				{
					text,
					destinationVariable.Load(),
					variableName
				});
			}
			else
			{
				string text;
				if (this._nativeType == NativeType.BStr)
				{
					text = "il2cpp_codegen_marshal_bstring_result";
				}
				else
				{
					text = ((!this.IsWideString) ? "il2cpp_codegen_marshal_string_result" : "il2cpp_codegen_marshal_wstring_result");
				}
				writer.WriteLine(destinationVariable.Store("{0}({1})", new object[]
				{
					text,
					variableName
				}));
			}
		}

		public override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
		{
			if (!this.IsFixedSizeString)
			{
				if (this._nativeType == NativeType.BStr)
				{
					writer.WriteLine("il2cpp_codegen_marshal_free_bstring({0});", new object[]
					{
						variableName
					});
				}
				else
				{
					writer.WriteLine("il2cpp_codegen_marshal_free({0});", new object[]
					{
						variableName
					});
				}
				writer.WriteLine("{0} = NULL;", new object[]
				{
					variableName
				});
			}
		}
	}
}
