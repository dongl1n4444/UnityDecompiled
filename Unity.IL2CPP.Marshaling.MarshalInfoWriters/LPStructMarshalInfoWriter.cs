using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	public class LPStructMarshalInfoWriter : DefaultMarshalInfoWriter
	{
		private readonly MarshaledType[] _marshaledTypes;

		public override MarshaledType[] MarshaledTypes
		{
			get
			{
				return this._marshaledTypes;
			}
		}

		public LPStructMarshalInfoWriter(TypeReference type, MarshalType marshalType) : base(type)
		{
			string text = DefaultMarshalInfoWriter.Naming.ForVariable(this._typeRef);
			this._marshaledTypes = new MarshaledType[]
			{
				new MarshaledType(text, text + "*")
			};
		}

		public override string WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			return sourceVariable.LoadAddress();
		}

		public override string WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			return DefaultMarshalInfoWriter.Naming.Dereference(variableName);
		}
	}
}
