using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	public class HandleRefMarshalInfoWriter : MarshalableMarshalInfoWriter
	{
		private readonly TypeDefinition _typeDefinition;

		private readonly bool _forByReferenceType;

		public override string MarshaledTypeName
		{
			get
			{
				return "void*";
			}
		}

		public HandleRefMarshalInfoWriter(TypeReference type, bool forByReferenceType) : base(type)
		{
			this._typeDefinition = type.Resolve();
			this._forByReferenceType = forByReferenceType;
		}

		public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			if (!this.CanMarshalTypeToNative())
			{
				throw new InvalidOperationException("Cannot marshal HandleRef by reference to native code.");
			}
			string arg_A9_1 = "{0} = {1}.{2}().{3}();";
			object[] expr_23 = new object[4];
			expr_23[0] = destinationVariable;
			expr_23[1] = sourceVariable.Load();
			expr_23[2] = DefaultMarshalInfoWriter.Naming.ForFieldGetter(this._typeDefinition.Fields.Single((FieldDefinition f) => f.Name == "handle"));
			expr_23[3] = DefaultMarshalInfoWriter.Naming.ForFieldGetter(DefaultMarshalInfoWriter.TypeProvider.SystemIntPtr.Fields.Single((FieldDefinition f) => f.Name == DefaultMarshalInfoWriter.Naming.IntPtrValueField));
			writer.WriteLine(arg_A9_1, expr_23);
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
	}
}
