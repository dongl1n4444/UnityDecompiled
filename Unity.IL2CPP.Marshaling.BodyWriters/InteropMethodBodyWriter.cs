using Mono.Cecil;
using System;
using System.Collections.Generic;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;
using Unity.IL2CPP.Marshaling.MarshalInfoWriters;

namespace Unity.IL2CPP.Marshaling.BodyWriters
{
	internal abstract class InteropMethodBodyWriter
	{
		[Inject]
		public static INamingService Naming;

		[Inject]
		public static ITypeProviderService TypeProvider;

		protected readonly TypeResolver _typeResolver;

		private readonly MethodReference _interopMethod;

		protected readonly MarshaledParameter[] _parameters;

		protected readonly MarshaledType[] _marshaledParameterTypes;

		protected readonly MarshaledType _marshaledReturnType;

		private readonly InteropMarshaler _marshaler;

		protected InteropMethodBodyWriter(MethodReference interopMethod, MethodReference methodForParameterNames, InteropMarshaler marshaler)
		{
			this._typeResolver = TypeResolver.For(interopMethod.DeclaringType, interopMethod);
			this._interopMethod = interopMethod;
			this._marshaler = marshaler;
			MethodDefinition methodDefinition = this._interopMethod.Resolve();
			this._parameters = new MarshaledParameter[methodDefinition.Parameters.Count];
			for (int i = 0; i < methodDefinition.Parameters.Count; i++)
			{
				ParameterDefinition parameterDefinition = methodDefinition.Parameters[i];
				TypeReference parameterType = this._typeResolver.Resolve(parameterDefinition.ParameterType);
				this._parameters[i] = new MarshaledParameter(methodForParameterNames.Parameters[i].Name, InteropMethodBodyWriter.Naming.ForParameterName(methodForParameterNames.Parameters[i]), parameterType, parameterDefinition.MarshalInfo, parameterDefinition.IsIn, parameterDefinition.IsOut);
			}
			List<MarshaledType> list = new List<MarshaledType>();
			MarshaledParameter[] parameters = this._parameters;
			for (int j = 0; j < parameters.Length; j++)
			{
				MarshaledParameter marshaledParameter = parameters[j];
				MarshaledType[] marshaledTypes = this.MarshalInfoWriterFor(marshaledParameter).MarshaledTypes;
				for (int k = 0; k < marshaledTypes.Length; k++)
				{
					MarshaledType marshaledType = marshaledTypes[k];
					list.Add(new MarshaledType(marshaledType.Name, marshaledType.DecoratedName, marshaledParameter.NameInGeneratedCode + marshaledType.VariableName));
				}
			}
			MarshaledType[] marshaledTypes2 = this.MarshalInfoWriterFor(this.GetMethodReturnType()).MarshaledTypes;
			for (int l = 0; l < marshaledTypes2.Length - 1; l++)
			{
				MarshaledType marshaledType2 = marshaledTypes2[l];
				list.Add(new MarshaledType(marshaledType2.Name + "*", marshaledType2.DecoratedName + "*", InteropMethodBodyWriter.Naming.ForComInterfaceReturnParameterName() + marshaledType2.VariableName));
			}
			this._marshaledParameterTypes = list.ToArray();
			this._marshaledReturnType = marshaledTypes2[marshaledTypes2.Length - 1];
		}

		protected virtual void WriteMethodPrologue(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
		{
		}

		protected abstract void WriteInteropCallStatement(CppCodeWriter writer, string[] localVariableNames, IRuntimeMetadataAccess metadataAccess);

		protected virtual void WriteMethodEpilogue(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
		{
		}

		public void WriteMethodBody(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
		{
			DefaultMarshalInfoWriter defaultMarshalInfoWriter = this.FirstOrDefaultUnmarshalableMarshalInfoWriter();
			if (defaultMarshalInfoWriter != null)
			{
				writer.WriteStatement(Emit.RaiseManagedException(defaultMarshalInfoWriter.GetMarshalingException()));
			}
			else
			{
				MarshaledParameter[] parameters = this._parameters;
				for (int i = 0; i < parameters.Length; i++)
				{
					MarshaledParameter parameter = parameters[i];
					this.MarshalInfoWriterFor(parameter).WriteIncludesForMarshaling(writer);
				}
				this.MarshalInfoWriterFor(this.GetMethodReturnType()).WriteIncludesForMarshaling(writer);
				this.WriteMethodBodyImpl(writer, metadataAccess);
			}
		}

		private void WriteMethodBodyImpl(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
		{
			this.WriteMethodPrologue(writer, metadataAccess);
			string[] localVariableNames = this.WriteMarshalInputParameters(writer, metadataAccess);
			string unmarshaledReturnValueVariableName = null;
			writer.WriteLine("// {0} invocation", new object[]
			{
				this._marshaler.GetPrettyCalleeName()
			});
			this.WriteInteropCallStatement(writer, localVariableNames, metadataAccess);
			writer.WriteLine();
			MethodReturnType methodReturnType = this.GetMethodReturnType();
			if (methodReturnType.ReturnType.MetadataType != MetadataType.Void)
			{
				unmarshaledReturnValueVariableName = this._marshaler.WriteMarshalReturnValue(writer, methodReturnType, this._parameters, metadataAccess);
				this._marshaler.WriteMarshalCleanupReturnValue(writer, methodReturnType, metadataAccess);
			}
			this.WriteMarshalOutputParameters(writer, localVariableNames, metadataAccess);
			this.WriteMethodEpilogue(writer, metadataAccess);
			this.WriteReturnStatement(writer, unmarshaledReturnValueVariableName, metadataAccess);
		}

		protected DefaultMarshalInfoWriter FirstOrDefaultUnmarshalableMarshalInfoWriter()
		{
			MarshaledParameter[] parameters = this._parameters;
			int i = 0;
			DefaultMarshalInfoWriter result;
			while (i < parameters.Length)
			{
				MarshaledParameter parameter = parameters[i];
				if (!this._marshaler.CanMarshalAsInputParameter(parameter))
				{
					result = this._marshaler.MarshalInfoWriterFor(parameter);
				}
				else
				{
					if (!this.IsOutParameter(parameter) || this._marshaler.CanMarshalAsOutputParameter(parameter))
					{
						i++;
						continue;
					}
					result = this._marshaler.MarshalInfoWriterFor(parameter);
				}
				return result;
			}
			MethodReturnType methodReturnType = this.GetMethodReturnType();
			if (methodReturnType.ReturnType.MetadataType != MetadataType.Void)
			{
				if (!this._marshaler.CanMarshalAsOutputParameter(methodReturnType))
				{
					result = this._marshaler.MarshalInfoWriterFor(methodReturnType);
					return result;
				}
			}
			result = null;
			return result;
		}

		private string[] WriteMarshalInputParameters(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
		{
			string[] array = new string[this._parameters.Length];
			for (int i = 0; i < this._parameters.Length; i++)
			{
				array[i] = this.WriteMarshalInputParameter(writer, this._parameters[i], metadataAccess);
			}
			return array;
		}

		private string WriteMarshalInputParameter(CppCodeWriter writer, MarshaledParameter parameter, IRuntimeMetadataAccess metadataAccess)
		{
			string result;
			if (this.IsInParameter(parameter))
			{
				result = this._marshaler.WriteMarshalInputParameter(writer, parameter, this._parameters, metadataAccess);
			}
			else
			{
				result = this._marshaler.WriteMarshalEmptyInputParameter(writer, parameter, this._parameters, metadataAccess);
			}
			return result;
		}

		private void WriteMarshalOutputParameters(CppCodeWriter writer, string[] localVariableNames, IRuntimeMetadataAccess metadataAccess)
		{
			for (int i = 0; i < this._parameters.Length; i++)
			{
				this.WriteMarshalOutputParameter(writer, localVariableNames[i], this._parameters[i], metadataAccess);
				this.WriteCleanupParameter(writer, localVariableNames[i], this._parameters[i], metadataAccess);
			}
		}

		private void WriteMarshalOutputParameter(CppCodeWriter writer, string valueName, MarshaledParameter parameter, IRuntimeMetadataAccess metadataAccess)
		{
			if (this.IsOutParameter(parameter))
			{
				this._marshaler.WriteMarshalOutputParameter(writer, valueName, parameter, this._parameters, metadataAccess);
			}
		}

		private void WriteCleanupParameter(CppCodeWriter writer, string valueName, MarshaledParameter parameter, IRuntimeMetadataAccess metadataAccess)
		{
			if (this.ParameterRequiresCleanup(parameter))
			{
				if (this.IsInParameter(parameter))
				{
					this._marshaler.WriteMarshalCleanupParameter(writer, valueName, parameter, metadataAccess);
				}
				else
				{
					this._marshaler.WriteMarshalCleanupEmptyParameter(writer, valueName, parameter, metadataAccess);
				}
			}
		}

		protected virtual void WriteReturnStatement(CppCodeWriter writer, string unmarshaledReturnValueVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			if (this.GetMethodReturnType().ReturnType.MetadataType != MetadataType.Void)
			{
				writer.WriteLine("return {0};", new object[]
				{
					unmarshaledReturnValueVariableName
				});
			}
		}

		protected virtual MethodReturnType GetMethodReturnType()
		{
			return this._interopMethod.MethodReturnType;
		}

		protected string GetMethodName()
		{
			return this._interopMethod.Name;
		}

		protected virtual string GetMethodNameInGeneratedCode()
		{
			return InteropMethodBodyWriter.Naming.ForMethodNameOnly(this._interopMethod);
		}

		protected IList<CustomAttribute> GetCustomMethodAttributes()
		{
			return this._interopMethod.Resolve().CustomAttributes;
		}

		protected DefaultMarshalInfoWriter MarshalInfoWriterFor(MarshaledParameter parameter)
		{
			return this._marshaler.MarshalInfoWriterFor(parameter);
		}

		protected DefaultMarshalInfoWriter MarshalInfoWriterFor(MethodReturnType methodReturnType)
		{
			return this._marshaler.MarshalInfoWriterFor(methodReturnType);
		}

		protected bool IsInParameter(MarshaledParameter parameter)
		{
			TypeReference parameterType = parameter.ParameterType;
			return !parameter.IsOut || parameter.IsIn || (parameterType.IsValueType() && !parameterType.IsByReference);
		}

		protected bool IsOutParameter(MarshaledParameter parameter)
		{
			TypeReference parameterType = parameter.ParameterType;
			return (parameter.IsOut && !parameterType.IsValueType()) || ((!parameter.IsIn || parameter.IsOut) && (parameter.ParameterType.IsByReference || MarshalingUtils.IsStringBuilder(parameterType)));
		}

		private bool ParameterRequiresCleanup(MarshaledParameter parameter)
		{
			return this.IsInParameter(parameter) || parameter.IsOut;
		}
	}
}
