using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	internal sealed class ByReferenceMarshalInfoWriter : MarshalableMarshalInfoWriter
	{
		private readonly DefaultMarshalInfoWriter _elementTypeMarshalInfoWriter;

		private readonly string _marshaledTypeName;

		private readonly string _marshaledDecoratedTypeName;

		public override string MarshaledTypeName
		{
			get
			{
				return this._marshaledTypeName;
			}
		}

		public override string MarshaledDecoratedTypeName
		{
			get
			{
				return this._marshaledDecoratedTypeName;
			}
		}

		public ByReferenceMarshalInfoWriter(ByReferenceType type, MarshalType marshalType, MarshalInfo marshalInfo) : base(type)
		{
			this._elementTypeMarshalInfoWriter = MarshalDataCollector.MarshalInfoWriterFor(type.ElementType, marshalType, marshalInfo, false, true);
			this._marshaledTypeName = string.Format("{0}*", this._elementTypeMarshalInfoWriter.MarshaledTypeName);
			this._marshaledDecoratedTypeName = string.Format("{0}*", this._elementTypeMarshalInfoWriter.MarshaledDecoratedTypeName);
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
			string text = string.Format("{0}_dereferenced", destinationVariable);
			this._elementTypeMarshalInfoWriter.WriteNativeVariableDeclarationOfType(writer, text);
			this._elementTypeMarshalInfoWriter.WriteMarshalVariableToNative(writer, sourceVariable.Dereferenced, text, managedVariableName, metadataAccess);
			writer.WriteLine("{0} = &{1};", new object[]
			{
				destinationVariable,
				text
			});
		}

		public override string WriteMarshalEmptyVariableToNative(CppCodeWriter writer, string variableName)
		{
			string text = string.Format("_{0}_marshaled", variableName);
			if (((ByReferenceType)this._typeRef).ElementType.MetadataType == MetadataType.Class && !(this._elementTypeMarshalInfoWriter is UnmarshalableMarshalInfoWriter) && !(this._elementTypeMarshalInfoWriter is SafeHandleMarshalInfoWriter) && !(this._elementTypeMarshalInfoWriter is ComInterfaceMarshalInfoWriter) && !(this._elementTypeMarshalInfoWriter is DelegateMarshalInfoWriter))
			{
				writer.WriteLine("{0} {1} = {2};", new object[]
				{
					this._elementTypeMarshalInfoWriter.MarshaledDecoratedTypeName,
					text,
					DefaultMarshalInfoWriter.Naming.Null
				});
			}
			else
			{
				string text2 = string.Format("_{0}_empty", variableName);
				this._elementTypeMarshalInfoWriter.WriteNativeVariableDeclarationOfType(writer, text2);
				writer.WriteLine("{0} {1} = &{2};", new object[]
				{
					this.MarshaledTypeName,
					text,
					text2
				});
			}
			return text;
		}

		public override void WriteMarshalOutVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, IRuntimeMetadataAccess metadataAccess)
		{
			this._elementTypeMarshalInfoWriter.WriteMarshalVariableToNative(writer, sourceVariable.Dereferenced, ByReferenceMarshalInfoWriter.DereferenceVariableName(this.UndecorateVariable(destinationVariable)), null, metadataAccess);
		}

		public override string WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			string text = string.Format("_{0}_unmarshaled_dereferenced", variableName.Replace("*", ""));
			this._elementTypeMarshalInfoWriter.WriteDeclareAndAllocateObject(writer, text, variableName, metadataAccess);
			this._elementTypeMarshalInfoWriter.WriteMarshalVariableFromNative(writer, ByReferenceMarshalInfoWriter.DereferenceVariableName(variableName), new ManagedMarshalValue(text), methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess);
			return DefaultMarshalInfoWriter.Naming.AddressOf(text);
		}

		public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			string text = string.Format("_{0}_unmarshaled_dereferenced", variableName.Replace("*", ""));
			this._elementTypeMarshalInfoWriter.WriteDeclareAndAllocateObject(writer, text, variableName, metadataAccess);
			this._elementTypeMarshalInfoWriter.WriteMarshalVariableFromNative(writer, ByReferenceMarshalInfoWriter.DereferenceVariableName(variableName), new ManagedMarshalValue(text), methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess);
			writer.WriteStatement(destinationVariable.Dereferenced.Store(text));
		}

		public override void WriteDeclareAndAllocateObject(CppCodeWriter writer, string unmarshaledVariableName, string marshaledVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			string text = unmarshaledVariableName + "_dereferenced";
			this._elementTypeMarshalInfoWriter.WriteDeclareAndAllocateObject(writer, text, ByReferenceMarshalInfoWriter.DereferenceVariableName(marshaledVariableName), metadataAccess);
			writer.WriteLine("{0} {1} = &{2};", new object[]
			{
				DefaultMarshalInfoWriter.Naming.ForVariable(this._typeRef),
				unmarshaledVariableName,
				text
			});
		}

		public override string WriteMarshalEmptyVariableFromNative(CppCodeWriter writer, string variableName)
		{
			string text = this._elementTypeMarshalInfoWriter.WriteMarshalEmptyVariableFromNative(writer, ByReferenceMarshalInfoWriter.DereferenceVariableName(variableName));
			string text2 = string.Format("_{0}_reference", text.Replace("*", ""));
			writer.WriteLine("{0} {1} = &({2});", new object[]
			{
				DefaultMarshalInfoWriter.Naming.ForVariable(this._typeRef),
				text2,
				text
			});
			return text2;
		}

		public override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
		{
			this._elementTypeMarshalInfoWriter.WriteMarshalCleanupVariable(writer, ByReferenceMarshalInfoWriter.DereferenceVariableName(variableName), metadataAccess, ByReferenceMarshalInfoWriter.DereferenceVariableName(managedVariableName));
		}

		public override void WriteMarshalCleanupEmptyVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName)
		{
			this._elementTypeMarshalInfoWriter.WriteMarshalCleanupEmptyVariable(writer, ByReferenceMarshalInfoWriter.DereferenceVariableName(variableName), metadataAccess, ByReferenceMarshalInfoWriter.DereferenceVariableName(managedVariableName));
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

		private static string DereferenceVariableName(string variableName)
		{
			string result;
			if (variableName.StartsWith("&"))
			{
				result = variableName.Substring(1);
			}
			else
			{
				result = string.Format("*{0}", variableName);
			}
			return result;
		}
	}
}
