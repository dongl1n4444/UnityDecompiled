using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	internal sealed class ByReferenceMarshalInfoWriter : MarshalableMarshalInfoWriter
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

		public ByReferenceMarshalInfoWriter(ByReferenceType type, MarshalType marshalType, MarshalInfo marshalInfo) : base(type)
		{
			this._elementType = type.ElementType;
			this._elementTypeMarshalInfoWriter = MarshalDataCollector.MarshalInfoWriterFor(type.ElementType, marshalType, marshalInfo, false, true, false, null);
			this._marshaledTypes = (from t in this._elementTypeMarshalInfoWriter.MarshaledTypes
			select new MarshaledType(t.Name + "*", t.DecoratedName + "*", t.VariableName)).ToArray<MarshaledType>();
		}

		public override void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
		{
			this._elementTypeMarshalInfoWriter.WriteMarshaledTypeForwardDeclaration(writer);
		}

		public override void WriteIncludesForFieldDeclaration(CppCodeWriter writer)
		{
			this._elementTypeMarshalInfoWriter.WriteMarshaledTypeForwardDeclaration(writer);
		}

		public override void WriteIncludesForMarshaling(CppCodeWriter writer)
		{
			this._elementTypeMarshalInfoWriter.WriteIncludesForMarshaling(writer);
		}

		public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			string text = string.Format("{0}_dereferenced", DefaultMarshalInfoWriter.CleanVariableName(destinationVariable));
			this._elementTypeMarshalInfoWriter.WriteNativeVariableDeclarationOfType(writer, text);
			this._elementTypeMarshalInfoWriter.WriteMarshalVariableToNative(writer, sourceVariable.Dereferenced, text, managedVariableName, metadataAccess);
			writer.WriteLine("{0} = &{1};", new object[]
			{
				destinationVariable,
				text
			});
		}

		public override string WriteMarshalEmptyVariableToNative(CppCodeWriter writer, ManagedMarshalValue variableName, IList<MarshaledParameter> methodParameters)
		{
			string text = string.Format("_{0}_marshaled", variableName.GetNiceName());
			if (((ByReferenceType)this._typeRef).ElementType.MetadataType == MetadataType.Class && !(this._elementTypeMarshalInfoWriter is UnmarshalableMarshalInfoWriter) && !(this._elementTypeMarshalInfoWriter is SafeHandleMarshalInfoWriter) && !(this._elementTypeMarshalInfoWriter is ComObjectMarshalInfoWriter) && !(this._elementTypeMarshalInfoWriter is DelegateMarshalInfoWriter) && !(this._elementTypeMarshalInfoWriter is StringMarshalInfoWriter))
			{
				this.WriteNativeVariableDeclarationOfType(writer, text);
			}
			else
			{
				string text2 = string.Format("_{0}_empty", variableName.GetNiceName());
				this._elementTypeMarshalInfoWriter.WriteNativeVariableDeclarationOfType(writer, text2);
				MarshaledType[] marshaledTypes = this.MarshaledTypes;
				for (int i = 0; i < marshaledTypes.Length; i++)
				{
					MarshaledType marshaledType = marshaledTypes[i];
					writer.WriteLine("{0} {1} = &{2};", new object[]
					{
						marshaledType.Name,
						text + marshaledType.VariableName,
						text2 + marshaledType.VariableName
					});
				}
			}
			return text;
		}

		public override void WriteMarshalOutParameterToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IList<MarshaledParameter> methodParameters, IRuntimeMetadataAccess metadataAccess)
		{
			this._elementTypeMarshalInfoWriter.WriteMarshalVariableToNative(writer, sourceVariable.Dereferenced, DefaultMarshalInfoWriter.Naming.Dereference(this.UndecorateVariable(destinationVariable)), managedVariableName, metadataAccess);
		}

		public override string WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			string text = string.Format("_{0}_unmarshaled_dereferenced", DefaultMarshalInfoWriter.CleanVariableName(variableName));
			this._elementTypeMarshalInfoWriter.WriteDeclareAndAllocateObject(writer, text, variableName, metadataAccess);
			this._elementTypeMarshalInfoWriter.WriteMarshalVariableFromNative(writer, DefaultMarshalInfoWriter.Naming.Dereference(variableName), new ManagedMarshalValue(text), methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess);
			return DefaultMarshalInfoWriter.Naming.AddressOf(text);
		}

		public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			string text = string.Format("_{0}_unmarshaled_dereferenced", DefaultMarshalInfoWriter.CleanVariableName(variableName));
			this._elementTypeMarshalInfoWriter.WriteDeclareAndAllocateObject(writer, text, variableName, metadataAccess);
			this._elementTypeMarshalInfoWriter.WriteMarshalVariableFromNative(writer, DefaultMarshalInfoWriter.Naming.Dereference(variableName), new ManagedMarshalValue(text), methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess);
			writer.WriteLine(destinationVariable.Dereferenced.Store(text));
		}

		public override void WriteDeclareAndAllocateObject(CppCodeWriter writer, string unmarshaledVariableName, string marshaledVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			string text = unmarshaledVariableName + "_dereferenced";
			this._elementTypeMarshalInfoWriter.WriteDeclareAndAllocateObject(writer, text, DefaultMarshalInfoWriter.Naming.Dereference(marshaledVariableName), metadataAccess);
			writer.WriteLine("{0} {1} = &{2};", new object[]
			{
				DefaultMarshalInfoWriter.Naming.ForVariable(this._typeRef),
				unmarshaledVariableName,
				text
			});
		}

		public override string WriteMarshalEmptyVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, IRuntimeMetadataAccess metadataAccess)
		{
			string text = string.Format("_{0}_empty", DefaultMarshalInfoWriter.CleanVariableName(variableName));
			writer.WriteVariable(this._elementType, text);
			return DefaultMarshalInfoWriter.Naming.AddressOf(text);
		}

		public override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
		{
			this._elementTypeMarshalInfoWriter.WriteMarshalCleanupVariable(writer, DefaultMarshalInfoWriter.Naming.Dereference(variableName), metadataAccess, DefaultMarshalInfoWriter.Naming.Dereference(managedVariableName));
		}

		public override void WriteMarshalCleanupEmptyVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName)
		{
			this._elementTypeMarshalInfoWriter.WriteMarshalCleanupEmptyVariable(writer, DefaultMarshalInfoWriter.Naming.Dereference(variableName), metadataAccess, DefaultMarshalInfoWriter.Naming.Dereference(managedVariableName));
		}

		public override string DecorateVariable(string unmarshaledParameterName, string marshaledVariableName)
		{
			return this._elementTypeMarshalInfoWriter.DecorateVariable(unmarshaledParameterName, marshaledVariableName);
		}

		public override string UndecorateVariable(string variableName)
		{
			return this._elementTypeMarshalInfoWriter.UndecorateVariable(variableName);
		}

		public override bool CanMarshalTypeToNative()
		{
			return this._elementTypeMarshalInfoWriter.CanMarshalTypeToNative();
		}

		public override bool CanMarshalTypeFromNative()
		{
			return this._elementTypeMarshalInfoWriter.CanMarshalTypeFromNative();
		}

		public override string GetMarshalingException()
		{
			return this._elementTypeMarshalInfoWriter.GetMarshalingException();
		}
	}
}
