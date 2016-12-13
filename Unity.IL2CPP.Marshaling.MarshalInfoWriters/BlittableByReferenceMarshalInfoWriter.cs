using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	public class BlittableByReferenceMarshalInfoWriter : DefaultMarshalInfoWriter
	{
		private readonly TypeReference _elementType;

		private readonly DefaultMarshalInfoWriter _elementTypeMarshalInfoWriter;

		private readonly MarshaledType[] _marshaledTypes;

		public override MarshaledType[] MarshaledTypes
		{
			get
			{
				return this._marshaledTypes;
			}
		}

		public override int NativeSizeWithoutPointers
		{
			get
			{
				return 0;
			}
		}

		public BlittableByReferenceMarshalInfoWriter(ByReferenceType type, MarshalType marshalType, MarshalInfo marshalInfo) : base(type)
		{
			this._elementType = type.ElementType;
			this._elementTypeMarshalInfoWriter = MarshalDataCollector.MarshalInfoWriterFor(this._elementType, marshalType, marshalInfo, false, true, false, null);
			if (this._elementTypeMarshalInfoWriter.MarshaledTypes.Length > 1)
			{
				throw new InvalidOperationException(string.Format("BlittableByReferenceMarshalInfoWriter cannot marshal {0}&.", type.ElementType.FullName));
			}
			this._marshaledTypes = new MarshaledType[]
			{
				new MarshaledType(this._elementTypeMarshalInfoWriter.MarshaledTypes[0].Name + "*", this._elementTypeMarshalInfoWriter.MarshaledTypes[0].DecoratedName + "*")
			};
		}

		public override string WriteMarshalEmptyVariableToNative(CppCodeWriter writer, ManagedMarshalValue variableName, IList<MarshaledParameter> methodParameters)
		{
			string text = string.Format("_{0}_empty", variableName.GetNiceName());
			this._elementTypeMarshalInfoWriter.WriteNativeVariableDeclarationOfType(writer, text);
			return DefaultMarshalInfoWriter.Naming.AddressOf(text);
		}

		public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			writer.WriteLine("{0} = {1};", new object[]
			{
				destinationVariable,
				this.WriteMarshalVariableToNative(writer, sourceVariable, managedVariableName, metadataAccess)
			});
		}

		public override string WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			string result;
			if (DefaultMarshalInfoWriter.Naming.ForVariable(this._typeRef) != this._marshaledTypes[0].DecoratedName)
			{
				result = string.Format("reinterpret_cast<{0}>({1})", this._marshaledTypes[0].DecoratedName, sourceVariable.Load());
			}
			else
			{
				result = sourceVariable.Load();
			}
			return result;
		}

		public override void WriteMarshalOutParameterToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IList<MarshaledParameter> methodParameters, IRuntimeMetadataAccess metadataAccess)
		{
			this._elementTypeMarshalInfoWriter.WriteMarshalVariableToNative(writer, sourceVariable.Dereferenced, DefaultMarshalInfoWriter.Naming.Dereference(this.UndecorateVariable(destinationVariable)), managedVariableName, metadataAccess);
		}

		public override string WriteMarshalEmptyVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, IRuntimeMetadataAccess metadataAccess)
		{
			string text = string.Format("_{0}_empty", DefaultMarshalInfoWriter.CleanVariableName(variableName));
			writer.WriteVariable(this._elementType, text);
			return DefaultMarshalInfoWriter.Naming.AddressOf(text);
		}

		public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			writer.WriteLine(destinationVariable.Store(this.WriteMarshalVariableFromNative(writer, variableName, methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess)));
		}

		public override string WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			string text = DefaultMarshalInfoWriter.Naming.ForVariable(this._typeRef);
			string result;
			if (text != this._marshaledTypes[0].DecoratedName)
			{
				result = string.Format("reinterpret_cast<{0}>({1})", text, variableName);
			}
			else
			{
				result = variableName;
			}
			return result;
		}

		public override void WriteMarshalOutParameterFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			this._elementTypeMarshalInfoWriter.WriteMarshalVariableFromNative(writer, DefaultMarshalInfoWriter.Naming.Dereference(variableName), destinationVariable.Dereferenced, methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess);
		}

		public override void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
		{
			this._elementTypeMarshalInfoWriter.WriteMarshaledTypeForwardDeclaration(writer);
		}
	}
}
