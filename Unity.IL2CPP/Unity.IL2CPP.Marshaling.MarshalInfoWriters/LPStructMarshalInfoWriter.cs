using Mono.Cecil;
using System;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	public class LPStructMarshalInfoWriter : DefaultMarshalInfoWriter
	{
		public override string MarshaledTypeName
		{
			get
			{
				return DefaultMarshalInfoWriter.Naming.ForVariable(this._typeRef);
			}
		}

		public override string MarshaledDecoratedTypeName
		{
			get
			{
				return string.Format("{0}*", base.MarshaledDecoratedTypeName);
			}
		}

		public LPStructMarshalInfoWriter(TypeReference type, MarshalType marshalType) : base(type)
		{
		}

		public override string WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			return sourceVariable.LoadAddress();
		}
	}
}
