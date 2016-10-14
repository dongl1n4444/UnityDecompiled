using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	public sealed class UnmarshalableMarshalInfoWriter : MarshalableMarshalInfoWriter
	{
		public override string NativeSize
		{
			get
			{
				return "-1";
			}
		}

		public override string MarshaledTypeName
		{
			get
			{
				string result;
				if (this._typeRef is GenericParameter)
				{
					result = "void*";
				}
				else
				{
					result = DefaultMarshalInfoWriter.Naming.ForVariable(this._typeRef);
				}
				return result;
			}
		}

		public UnmarshalableMarshalInfoWriter(TypeReference type) : base(type)
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
			throw new InvalidOperationException(string.Format("Cannot marshal {0} to native!", this._typeRef.FullName));
		}

		public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			throw new InvalidOperationException(string.Format("Cannot marshal {0} from native!", this._typeRef.FullName));
		}

		public override string WriteMarshalEmptyVariableToNative(CppCodeWriter writer, string variableName)
		{
			throw new InvalidOperationException(string.Format("Cannot marshal {0} to native!", this._typeRef.FullName));
		}

		public override bool CanMarshalTypeToNative()
		{
			return false;
		}

		public override void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
		{
			writer.AddForwardDeclaration(this._typeRef);
		}

		public override string GetMarshalingException()
		{
			return string.Format("il2cpp_codegen_get_marshal_directive_exception(\"Cannot marshal type '{0}'.\")", this._typeRef.FullName);
		}
	}
}
