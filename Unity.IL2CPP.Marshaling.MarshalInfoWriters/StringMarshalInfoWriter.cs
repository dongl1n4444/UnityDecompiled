using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	internal sealed class StringMarshalInfoWriter : MarshalableMarshalInfoWriter
	{
		private const NativeType kNativeTypeHString = (NativeType)47;

		private readonly string _marshaledTypeName;

		private readonly NativeType _nativeType;

		private readonly bool _isStringBuilder;

		private readonly MarshalInfo _marshalInfo;

		private readonly bool _useUnicodeCharSet;

		private readonly MarshaledType[] _marshaledTypes;

		private readonly bool _canReferenceOriginalManagedString;

		public override MarshaledType[] MarshaledTypes
		{
			get
			{
				return this._marshaledTypes;
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
				return this._nativeType == NativeType.LPWStr || this._nativeType == NativeType.BStr || this._nativeType == (NativeType)47 || (this.IsFixedSizeString && this._useUnicodeCharSet);
			}
		}

		private int BytesPerCharacter
		{
			get
			{
				return (!this.IsWideString) ? 1 : 2;
			}
		}

		public StringMarshalInfoWriter(TypeReference type, MarshalType marshalType, MarshalInfo marshalInfo, bool useUnicodeCharSet, bool forByReferenceType, bool forFieldMarshaling) : base(type)
		{
			this._isStringBuilder = MarshalingUtils.IsStringBuilder(type);
			this._useUnicodeCharSet = useUnicodeCharSet;
			this._nativeType = StringMarshalInfoWriter.DetermineNativeTypeFor(marshalType, marshalInfo, this._useUnicodeCharSet, this._isStringBuilder);
			if (this._nativeType == (NativeType)47)
			{
				this._marshaledTypeName = "Il2CppHString";
			}
			else if (this.IsWideString)
			{
				this._marshaledTypeName = "Il2CppChar*";
			}
			else
			{
				this._marshaledTypeName = "char*";
			}
			this._marshaledTypes = new MarshaledType[]
			{
				new MarshaledType(this._marshaledTypeName, this._marshaledTypeName)
			};
			this._marshalInfo = marshalInfo;
			this._canReferenceOriginalManagedString = (!this._isStringBuilder && !forByReferenceType && !forFieldMarshaling && (this._nativeType == NativeType.LPWStr || this._nativeType == (NativeType)47));
		}

		private static NativeType DetermineNativeTypeFor(MarshalType marshalType, MarshalInfo marshalInfo, bool useUnicodeCharset, bool isStringBuilder)
		{
			NativeType nativeType;
			if (marshalInfo != null)
			{
				nativeType = marshalInfo.NativeType;
			}
			else if (marshalType == MarshalType.PInvoke)
			{
				nativeType = ((!useUnicodeCharset) ? NativeType.LPStr : NativeType.LPWStr);
			}
			else
			{
				nativeType = NativeType.None;
			}
			bool flag = false;
			switch (nativeType)
			{
			case NativeType.BStr:
			case NativeType.LPStr:
			case NativeType.LPWStr:
			case NativeType.FixedSysString:
				goto IL_5D;
			case NativeType.LPTStr:
				IL_50:
				if (nativeType != (NativeType)47)
				{
					goto IL_64;
				}
				goto IL_5D;
			}
			goto IL_50;
			IL_5D:
			flag = true;
			IL_64:
			if (!flag || (isStringBuilder && nativeType != NativeType.LPStr && nativeType != NativeType.LPWStr))
			{
				if (marshalType != MarshalType.PInvoke)
				{
					if (marshalType != MarshalType.COM)
					{
						if (marshalType == MarshalType.WindowsRuntime)
						{
							nativeType = (NativeType)47;
						}
					}
					else
					{
						nativeType = NativeType.BStr;
					}
				}
				else
				{
					nativeType = NativeType.LPStr;
				}
			}
			return nativeType;
		}

		public override void WriteFieldDeclaration(CppCodeWriter writer, FieldReference field, string fieldNameSuffix = null)
		{
			if (this.IsFixedSizeString)
			{
				string text = DefaultMarshalInfoWriter.Naming.ForField(field) + fieldNameSuffix;
				writer.WriteLine("{0} {1}[{2}];", new object[]
				{
					this._marshaledTypeName.Replace("*", ""),
					text,
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
			this.WriteMarshalVariableToNative(writer, sourceVariable, destinationVariable, managedVariableName, metadataAccess, false);
		}

		public override string WriteMarshalReturnValueToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, IRuntimeMetadataAccess metadataAccess)
		{
			string text = string.Format("_{0}_marshaled", sourceVariable.GetNiceName());
			this.WriteNativeVariableDeclarationOfType(writer, text);
			this.WriteMarshalVariableToNative(writer, sourceVariable, text, null, metadataAccess, true);
			return text;
		}

		private void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess, bool isMarshalingReturnValue)
		{
			if (this._nativeType == (NativeType)47)
			{
				writer.WriteLine("if ({0} == {1})", new object[]
				{
					sourceVariable.Load(),
					DefaultMarshalInfoWriter.Naming.Null
				});
				using (new BlockWriter(writer, false))
				{
					writer.WriteStatement(Emit.RaiseManagedException("il2cpp_codegen_get_argument_null_exception(\"{0}\")", new object[]
					{
						(!string.IsNullOrEmpty(managedVariableName)) ? managedVariableName : sourceVariable.GetNiceName()
					}));
				}
			}
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
			else if (this._canReferenceOriginalManagedString && !isMarshalingReturnValue)
			{
				if (this._nativeType == NativeType.LPWStr)
				{
					string text2 = sourceVariable.Load();
					writer.WriteLine("if ({0} != {1})", new object[]
					{
						text2,
						DefaultMarshalInfoWriter.Naming.Null
					});
					using (new BlockWriter(writer, false))
					{
						FieldDefinition field = DefaultMarshalInfoWriter.TypeProvider.SystemString.Fields.Single((FieldDefinition f) => !f.IsStatic && f.FieldType.MetadataType == MetadataType.Char);
						writer.WriteLine("{0} = {1}->{2}();", new object[]
						{
							destinationVariable,
							sourceVariable.Load(),
							DefaultMarshalInfoWriter.Naming.ForFieldAddressGetter(field)
						});
					}
				}
				else
				{
					if (this._nativeType != (NativeType)47)
					{
						throw new InvalidOperationException(string.Format("StringMarshalInfoWriter doesn't know how to marshal {0} while maintaining reference to original managed string.", this._nativeType));
					}
					string niceName = sourceVariable.GetNiceName();
					string text3 = niceName + "NativeView";
					string text4 = niceName + "HStringReference";
					writer.WriteLine();
					writer.WriteLine("DECLARE_IL2CPP_STRING_AS_STRING_VIEW_OF_NATIVE_CHARS({0}, {1});", new object[]
					{
						text3,
						sourceVariable.Load()
					});
					writer.WriteLine("il2cpp::utils::Il2CppHStringReference {0}({1});", new object[]
					{
						text4,
						text3
					});
					writer.WriteLine("{0} = {1};", new object[]
					{
						destinationVariable,
						text4
					});
				}
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
				else if (this._nativeType == (NativeType)47)
				{
					text = "il2cpp_codegen_create_hstring";
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

		public override string WriteMarshalEmptyVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, IRuntimeMetadataAccess metadataAccess)
		{
			return DefaultMarshalInfoWriter.Naming.Null;
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
				NativeType nativeType = this._nativeType;
				string text;
				if (nativeType != NativeType.BStr)
				{
					if (nativeType != (NativeType)47)
					{
						text = ((!this.IsWideString) ? "il2cpp_codegen_marshal_string_result" : "il2cpp_codegen_marshal_wstring_result");
					}
					else
					{
						text = "il2cpp_codegen_marshal_hstring_result";
					}
				}
				else
				{
					text = "il2cpp_codegen_marshal_bstring_result";
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
			if (!this._canReferenceOriginalManagedString)
			{
				this.FreeMarshaledString(writer, variableName);
			}
		}

		public override void WriteMarshalCleanupReturnValue(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess)
		{
			this.FreeMarshaledString(writer, variableName);
		}

		private void FreeMarshaledString(CppCodeWriter writer, string variableName)
		{
			if (!this.IsFixedSizeString)
			{
				NativeType nativeType = this._nativeType;
				if (nativeType != NativeType.BStr)
				{
					if (nativeType != (NativeType)47)
					{
						writer.WriteLine("il2cpp_codegen_marshal_free({0});", new object[]
						{
							variableName
						});
					}
					else
					{
						writer.WriteLine("il2cpp_codegen_marshal_free_hstring({0});", new object[]
						{
							variableName
						});
					}
				}
				else
				{
					writer.WriteLine("il2cpp_codegen_marshal_free_bstring({0});", new object[]
					{
						variableName
					});
				}
				writer.WriteLine("{0} = {1};", new object[]
				{
					variableName,
					DefaultMarshalInfoWriter.Naming.Null
				});
			}
		}
	}
}
