using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	public class ComInterfaceMarshalInfoWriter : MarshalableMarshalInfoWriter
	{
		private readonly bool _iUnknown;

		private readonly string _interfaceTypeName;

		private readonly string _marshaledTypeName;

		public override string MarshaledTypeName
		{
			get
			{
				return this._marshaledTypeName;
			}
		}

		public ComInterfaceMarshalInfoWriter(TypeReference type) : base(type)
		{
			this._iUnknown = type.IsSystemObject();
			this._interfaceTypeName = ((!this._iUnknown) ? DefaultMarshalInfoWriter.Naming.ForTypeNameOnly(type) : "Il2CppIUnknown");
			this._marshaledTypeName = this._interfaceTypeName + '*';
		}

		public override void WriteIncludesForFieldDeclaration(CppCodeWriter writer)
		{
			this.WriteMarshaledTypeForwardDeclaration(writer);
		}

		public override void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
		{
			if (!this._iUnknown)
			{
				writer.AddForwardDeclaration(string.Format("struct {0}", DefaultMarshalInfoWriter.Naming.ForTypeNameOnly(this._typeRef)));
			}
		}

		public override void WriteIncludesForMarshaling(CppCodeWriter writer)
		{
			if (!this._iUnknown)
			{
				base.WriteIncludesForMarshaling(writer);
			}
		}

		public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			writer.WriteLine("if ({0} != {1})", new object[]
			{
				sourceVariable.Load(),
				DefaultMarshalInfoWriter.Naming.Null
			});
			using (new BlockWriter(writer, false))
			{
				writer.WriteLine("if (({0})->klass->is_import_or_windows_runtime)", new object[]
				{
					sourceVariable.Load()
				});
				using (new BlockWriter(writer, false))
				{
					writer.WriteLine("{0} = ({1})il2cpp_codegen_com_query_interface(({2}){3}, {4}::IID);", new object[]
					{
						destinationVariable,
						this._marshaledTypeName,
						DefaultMarshalInfoWriter.Naming.ForVariable(DefaultMarshalInfoWriter.TypeProvider.Il2CppComObjectTypeReference),
						sourceVariable.Load(),
						this._interfaceTypeName
					});
				}
				writer.WriteLine("else");
				using (new BlockWriter(writer, false))
				{
					writer.WriteLine("{0} = ({1})il2cpp_codegen_com_create_ccw({2}, {3}::IID);", new object[]
					{
						destinationVariable,
						this._marshaledTypeName,
						sourceVariable.Load(),
						this._interfaceTypeName
					});
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

		public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			writer.WriteLine(destinationVariable.Store(Emit.Call("il2cpp_codegen_com_create_rcw", variableName)));
		}

		public override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName)
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
