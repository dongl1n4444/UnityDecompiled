using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.IL2CPP.Common;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	public class HandleRefMarshalInfoWriter : MarshalableMarshalInfoWriter
	{
		private readonly TypeDefinition _typeDefinition;

		private readonly bool _forByReferenceType;

		private readonly MarshaledType[] _marshaledTypes;

		public override MarshaledType[] MarshaledTypes
		{
			get
			{
				return this._marshaledTypes;
			}
		}

		public HandleRefMarshalInfoWriter(TypeReference type, bool forByReferenceType) : base(type)
		{
			this._typeDefinition = type.Resolve();
			this._forByReferenceType = forByReferenceType;
			this._marshaledTypes = new MarshaledType[]
			{
				new MarshaledType("void*", "void*")
			};
		}

		public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			if (!this.CanMarshalTypeToNative())
			{
				throw new InvalidOperationException("Cannot marshal HandleRef by reference to native code.");
			}
			FieldDefinition fieldDefinition = this._typeDefinition.Fields.SingleOrDefault((FieldDefinition f) => f.Name == "handle");
			if (fieldDefinition == null && CodeGenOptions.Dotnetprofile == DotNetProfile.Net45)
			{
				fieldDefinition = this._typeDefinition.Fields.SingleOrDefault((FieldDefinition f) => f.Name == "m_handle");
			}
			if (fieldDefinition == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate the handle field on {0}", this._typeDefinition));
			}
			string arg_108_1 = "{0} = {1}.{2}().{3}();";
			object[] expr_AE = new object[4];
			expr_AE[0] = destinationVariable;
			expr_AE[1] = sourceVariable.Load();
			expr_AE[2] = DefaultMarshalInfoWriter.Naming.ForFieldGetter(fieldDefinition);
			expr_AE[3] = DefaultMarshalInfoWriter.Naming.ForFieldGetter(DefaultMarshalInfoWriter.TypeProvider.SystemIntPtr.Fields.Single((FieldDefinition f) => f.Name == DefaultMarshalInfoWriter.Naming.IntPtrValueField));
			writer.WriteLine(arg_108_1, expr_AE);
		}

		public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			throw new InvalidOperationException("Cannot marshal HandleRef from native code");
		}

		public override string WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			throw new InvalidOperationException("Cannot marshal HandleRef from native code");
		}

		public override bool CanMarshalTypeToNative()
		{
			return !this._forByReferenceType;
		}

		public override bool CanMarshalTypeFromNative()
		{
			return false;
		}

		public override string GetMarshalingException()
		{
			return string.Format("il2cpp_codegen_get_marshal_directive_exception(\"HandleRefs cannot be marshaled ByRef or from unmanaged to managed.\")", this._typeRef);
		}

		public override void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
		{
		}
	}
}
