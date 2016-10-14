using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	public class ComInterfaceArrayMarshalInfoWriter : ArrayMarshalInfoWriter
	{
		public ComInterfaceArrayMarshalInfoWriter(ArrayType arrayType, MarshalType marshalType, MarshalInfo marshalInfo) : base(arrayType, marshalType, marshalInfo)
		{
		}

		public override string WriteMarshalEmptyVariableToNative(CppCodeWriter writer, string variableName)
		{
			string text = string.Format("_{0}_marshaled", variableName);
			writer.WriteLine("{0} {1} = {2};", new object[]
			{
				this.MarshaledTypeName,
				text,
				DefaultMarshalInfoWriter.Naming.Null
			});
			return text;
		}

		public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			writer.WriteLine("{0} = ({1}){2};", new object[]
			{
				destinationVariable,
				this._marshaledTypeName,
				sourceVariable.Load()
			});
		}

		public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			writer.WriteLine(destinationVariable.Store("({0}*){1}", new object[]
			{
				DefaultMarshalInfoWriter.Naming.ForTypeNameOnly(this._arrayType),
				variableName
			}));
		}
	}
}
