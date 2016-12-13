using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	internal sealed class ComObjectMarshalInfoWriter : MarshalableMarshalInfoWriter
	{
		public const NativeType kNativeTypeIInspectable = (NativeType)46;

		private readonly bool _marshalAsInspectable;

		private readonly bool _isSealed;

		private readonly bool _isClass;

		private readonly string _managedTypeName;

		private readonly TypeReference _defaultInterface;

		private readonly string _interfaceTypeName;

		private readonly MarshaledType[] _marshaledTypes;

		public sealed override MarshaledType[] MarshaledTypes
		{
			get
			{
				return this._marshaledTypes;
			}
		}

		public ComObjectMarshalInfoWriter(TypeReference type, MarshalType marshalType, MarshalInfo marshalInfo) : base(type)
		{
			this._marshalAsInspectable = (marshalType == MarshalType.WindowsRuntime || (marshalInfo != null && marshalInfo.NativeType == (NativeType)46));
			TypeDefinition typeDefinition = type.Resolve();
			this._isSealed = typeDefinition.IsSealed;
			this._isClass = (marshalType == MarshalType.WindowsRuntime && !typeDefinition.IsInterface() && !type.IsSystemObject());
			this._defaultInterface = ((!this._isClass) ? type : typeDefinition.ExtractDefaultInterface());
			this._managedTypeName = ((!this._isClass) ? DefaultMarshalInfoWriter.Naming.ForTypeNameOnly(DefaultMarshalInfoWriter.TypeProvider.SystemObject) : DefaultMarshalInfoWriter.Naming.ForTypeNameOnly(type));
			if (type.IsSystemObject())
			{
				this._interfaceTypeName = ((!this._marshalAsInspectable) ? "Il2CppIUnknown" : "Il2CppIInspectable");
			}
			else
			{
				this._interfaceTypeName = DefaultMarshalInfoWriter.Naming.ForTypeNameOnly(this._defaultInterface);
			}
			this._marshaledTypes = new MarshaledType[]
			{
				new MarshaledType(this._interfaceTypeName + '*', this._interfaceTypeName + '*')
			};
		}

		public override void WriteIncludesForFieldDeclaration(CppCodeWriter writer)
		{
			this.WriteMarshaledTypeForwardDeclaration(writer);
		}

		public override void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
		{
			if (!this._typeRef.IsSystemObject())
			{
				writer.AddForwardDeclaration(string.Format("struct {0}", this._interfaceTypeName));
			}
		}

		public sealed override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			writer.WriteLine("if ({0} != {1})", new object[]
			{
				sourceVariable.Load(),
				DefaultMarshalInfoWriter.Naming.Null
			});
			using (new BlockWriter(writer, false))
			{
				if (this._isSealed)
				{
					writer.WriteLine("{0} = ({1})->{2}();", new object[]
					{
						destinationVariable,
						sourceVariable.Load(),
						DefaultMarshalInfoWriter.Naming.ForComTypeInterfaceFieldGetter(this._defaultInterface)
					});
				}
				else
				{
					this.WriteMarshalToNativeForNonSealedType(writer, sourceVariable, destinationVariable);
				}
			}
			writer.WriteLine("else");
			using (new BlockWriter(writer, false))
			{
				writer.WriteLine("{0} = {1};", new object[]
				{
					destinationVariable,
					DefaultMarshalInfoWriter.Naming.Null
				});
			}
		}

		private void WriteMarshalToNativeForNonSealedType(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable)
		{
			writer.WriteLine("if (({0})->klass->is_import_or_windows_runtime)", new object[]
			{
				sourceVariable.Load()
			});
			using (new BlockWriter(writer, false))
			{
				if (this._isClass)
				{
					writer.WriteLine("{0} = ({1})->{2}();", new object[]
					{
						destinationVariable,
						sourceVariable.Load(),
						DefaultMarshalInfoWriter.Naming.ForComTypeInterfaceFieldGetter(this._defaultInterface)
					});
					writer.WriteLine("{0}->AddRef();", new object[]
					{
						destinationVariable
					});
				}
				else
				{
					writer.WriteLine("il2cpp_hresult_t {0} = (({1}){2})->{3}->QueryInterface({4}::IID, reinterpret_cast<void**>(&{5}));", new object[]
					{
						DefaultMarshalInfoWriter.Naming.ForInteropHResultVariable(),
						DefaultMarshalInfoWriter.Naming.ForVariable(DefaultMarshalInfoWriter.TypeProvider.Il2CppComObjectTypeReference),
						sourceVariable.Load(),
						DefaultMarshalInfoWriter.Naming.ForIl2CppComObjectIdentityField(),
						this._interfaceTypeName,
						destinationVariable
					});
					writer.WriteStatement(Emit.Call("il2cpp_codegen_com_raise_exception_if_failed", DefaultMarshalInfoWriter.Naming.ForInteropHResultVariable()));
				}
			}
			writer.WriteLine("else");
			using (new BlockWriter(writer, false))
			{
				writer.WriteLine("{0} = il2cpp_codegen_com_get_or_create_ccw<{1}>({2});", new object[]
				{
					destinationVariable,
					this._interfaceTypeName,
					sourceVariable.Load()
				});
			}
		}

		public sealed override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			writer.WriteLine("if ({0} != {1})", new object[]
			{
				variableName,
				DefaultMarshalInfoWriter.Naming.Null
			});
			using (new BlockWriter(writer, false))
			{
				TypeReference type = (!this._typeRef.IsInterface() && this._typeRef.Resolve().IsComOrWindowsRuntimeType()) ? this._typeRef : DefaultMarshalInfoWriter.TypeProvider.Il2CppComObjectTypeReference;
				if (this._isSealed)
				{
					writer.WriteLine(destinationVariable.Store("il2cpp_codegen_com_get_or_create_rcw_for_sealed_class<{0}>({1}, {2})", new object[]
					{
						this._managedTypeName,
						variableName,
						metadataAccess.TypeInfoFor(this._typeRef)
					}));
				}
				else if (this._marshalAsInspectable)
				{
					writer.WriteLine(destinationVariable.Store("il2cpp_codegen_com_get_or_create_rcw_from_iinspectable<{0}>({1}, {2})", new object[]
					{
						this._managedTypeName,
						variableName,
						metadataAccess.TypeInfoFor(type)
					}));
				}
				else
				{
					writer.WriteLine(destinationVariable.Store("il2cpp_codegen_com_get_or_create_rcw_from_iunknown<{0}>({1}, {2})", new object[]
					{
						this._managedTypeName,
						variableName,
						metadataAccess.TypeInfoFor(type)
					}));
				}
			}
			writer.WriteLine("else");
			using (new BlockWriter(writer, false))
			{
				writer.WriteLine(destinationVariable.Store(DefaultMarshalInfoWriter.Naming.Null));
			}
		}

		public sealed override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName)
		{
			if (!this._isSealed)
			{
				writer.WriteLine("if ({0} != {1})", new object[]
				{
					variableName,
					DefaultMarshalInfoWriter.Naming.Null
				});
				using (new BlockWriter(writer, false))
				{
					writer.WriteLine("({0})->Release();", new object[]
					{
						variableName
					});
					writer.WriteLine("{0} = {1};", new object[]
					{
						variableName,
						DefaultMarshalInfoWriter.Naming.Null
					});
				}
			}
		}
	}
}
