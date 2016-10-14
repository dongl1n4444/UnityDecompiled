using Mono.Cecil;
using System;
using System.Collections.Generic;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.Marshaling.MarshalInfoWriters;

namespace Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative
{
	internal class ManagedToNativeMarshaler : InteropMarshaler
	{
		public ManagedToNativeMarshaler(TypeResolver typeResolver, MarshalType marshalType, bool useUnicodeCharset) : base(typeResolver, marshalType, useUnicodeCharset)
		{
		}

		public override bool CanMarshalAsInputParameter(MarshaledParameter parameter)
		{
			return base.MarshalInfoWriterFor(parameter).CanMarshalTypeToNative();
		}

		public override bool CanMarshalAsOutputParameter(MarshaledParameter parameter)
		{
			return base.MarshalInfoWriterFor(parameter).CanMarshalTypeFromNative();
		}

		public override bool CanMarshalAsOutputParameter(MethodReturnType methodReturnType)
		{
			return base.MarshalInfoWriterFor(methodReturnType).CanMarshalTypeFromNative();
		}

		public override string GetPrettyCalleeName()
		{
			return "Native function";
		}

		public override string WriteMarshalEmptyInputParameter(CppCodeWriter writer, MarshaledParameter parameter, IRuntimeMetadataAccess metadataAccess)
		{
			return InteropMarshaler.WriteContentAndCommentIfNeeded<string>(writer, string.Format("Marshaling of parameter '{0}' to native representation", parameter.NameInGeneratedCode), (CppCodeWriter bodyWriter) => this.MarshalInfoWriterFor(parameter).WriteMarshalEmptyVariableToNative(bodyWriter, parameter.NameInGeneratedCode));
		}

		public override string WriteMarshalInputParameter(CppCodeWriter writer, MarshaledParameter parameter, IList<MarshaledParameter> parameters, IRuntimeMetadataAccess metadataAccess)
		{
			return InteropMarshaler.WriteContentAndCommentIfNeeded<string>(writer, string.Format("Marshaling of parameter '{0}' to native representation", parameter.NameInGeneratedCode), (CppCodeWriter bodyWriter) => this.MarshalInfoWriterFor(parameter).WriteMarshalVariableToNative(bodyWriter, new ManagedMarshalValue(parameter.NameInGeneratedCode), parameter.Name, metadataAccess));
		}

		public override void WriteMarshalOutputParameter(CppCodeWriter writer, string valueName, MarshaledParameter parameter, IList<MarshaledParameter> parameters, IRuntimeMetadataAccess metadataAccess)
		{
			InteropMarshaler.WriteContentAndCommentIfNeeded(writer, string.Format("Marshaling of parameter '{0}' back from native representation", parameter.NameInGeneratedCode), delegate(CppCodeWriter bodyWriter)
			{
				DefaultMarshalInfoWriter defaultMarshalInfoWriter = this.MarshalInfoWriterFor(parameter);
				ManagedMarshalValue destinationVariable = new ManagedMarshalValue(parameter.NameInGeneratedCode);
				if (parameter.IsOut)
				{
					defaultMarshalInfoWriter.WriteMarshalOutParameterFromNative(bodyWriter, valueName, destinationVariable, parameters, false, false, metadataAccess);
				}
				else
				{
					defaultMarshalInfoWriter.WriteMarshalVariableFromNative(bodyWriter, valueName, destinationVariable, parameters, false, false, metadataAccess);
				}
			});
		}

		public override string WriteMarshalReturnValue(CppCodeWriter writer, MethodReturnType methodReturnType, IList<MarshaledParameter> parameters, IRuntimeMetadataAccess metadataAccess)
		{
			return InteropMarshaler.WriteContentAndCommentIfNeeded<string>(writer, "Marshaling of return value back from native representation", delegate(CppCodeWriter bodyWriter)
			{
				DefaultMarshalInfoWriter defaultMarshalInfoWriter = this.MarshalInfoWriterFor(methodReturnType);
				string variableName = defaultMarshalInfoWriter.UndecorateVariable(InteropMarshaler.Naming.ForInteropReturnValue());
				return defaultMarshalInfoWriter.WriteMarshalVariableFromNative(bodyWriter, variableName, parameters, true, false, metadataAccess);
			});
		}

		public override void WriteMarshalCleanupEmptyParameter(CppCodeWriter writer, string valueName, MarshaledParameter parameter, IRuntimeMetadataAccess metadataAccess)
		{
			InteropMarshaler.WriteContentAndCommentIfNeeded(writer, string.Format("Marshaling cleanup of parameter '{0}' native representation", parameter.NameInGeneratedCode), delegate(CppCodeWriter bodyWriter)
			{
				this.MarshalInfoWriterFor(parameter).WriteMarshalCleanupEmptyVariable(bodyWriter, valueName, metadataAccess, parameter.NameInGeneratedCode);
			});
		}

		public override void WriteMarshalCleanupParameter(CppCodeWriter writer, string valueName, MarshaledParameter parameter, IRuntimeMetadataAccess metadataAccess)
		{
			InteropMarshaler.WriteContentAndCommentIfNeeded(writer, string.Format("Marshaling cleanup of parameter '{0}' native representation", parameter.NameInGeneratedCode), delegate(CppCodeWriter bodyWriter)
			{
				this.MarshalInfoWriterFor(parameter).WriteMarshalCleanupVariable(bodyWriter, valueName, metadataAccess, parameter.NameInGeneratedCode);
			});
		}

		public override void WriteMarshalCleanupReturnValue(CppCodeWriter writer, MethodReturnType methodReturnType, IRuntimeMetadataAccess metadataAccess)
		{
			InteropMarshaler.WriteContentAndCommentIfNeeded(writer, "Marshaling cleanup of return value native representation", delegate(CppCodeWriter bodyWriter)
			{
				DefaultMarshalInfoWriter defaultMarshalInfoWriter = this.MarshalInfoWriterFor(methodReturnType);
				string variableName = defaultMarshalInfoWriter.UndecorateVariable(InteropMarshaler.Naming.ForInteropReturnValue());
				defaultMarshalInfoWriter.WriteMarshalCleanupVariable(bodyWriter, variableName, metadataAccess, null);
			});
		}
	}
}
