using Mono.Cecil;
using System;
using System.Collections.Generic;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	public abstract class DefaultMarshalInfoWriter
	{
		protected readonly TypeReference _typeRef;

		[Inject]
		public static INamingService Naming;

		[Inject]
		public static ITypeProviderService TypeProvider;

		public virtual int NativeSizeWithoutPointers
		{
			get
			{
				return 0;
			}
		}

		public virtual string NativeSize
		{
			get
			{
				return string.Format("sizeof({0})", this.MarshaledTypeName);
			}
		}

		public abstract string MarshaledTypeName
		{
			get;
		}

		public virtual string MarshaledDecoratedTypeName
		{
			get
			{
				return this.MarshaledTypeName;
			}
		}

		public virtual bool HasMarshalFunctions
		{
			get
			{
				return false;
			}
		}

		public virtual string MarshalToNativeFunctionName
		{
			get
			{
				return DefaultMarshalInfoWriter.Naming.Null;
			}
		}

		public virtual string MarshalFromNativeFunctionName
		{
			get
			{
				return DefaultMarshalInfoWriter.Naming.Null;
			}
		}

		public virtual string MarshalCleanupFunctionName
		{
			get
			{
				return DefaultMarshalInfoWriter.Naming.Null;
			}
		}

		public virtual bool HasNativeStructDefinition
		{
			get
			{
				return false;
			}
		}

		public DefaultMarshalInfoWriter(TypeReference type)
		{
			this._typeRef = type;
		}

		public virtual void WriteNativeStructDefinition(CppCodeWriter writer)
		{
		}

		public virtual void WriteMarshalFunctionDeclarations(CppCodeWriter writer)
		{
		}

		public virtual void WriteMarshalFunctionDefinitions(CppCodeWriter writer)
		{
		}

		public virtual void WriteCCWMarshalFunctionDeclaration(CppCodeWriter writer)
		{
		}

		public virtual void WriteCCWMarshalFunctionDefinition(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
		{
		}

		public virtual void WriteFieldDeclaration(CppCodeWriter writer, FieldReference field, string fieldNameSuffix = null)
		{
			string text = DefaultMarshalInfoWriter.Naming.ForField(field) + fieldNameSuffix;
			writer.WriteLine("{0} {1};", new object[]
			{
				this.MarshaledDecoratedTypeName,
				text
			});
		}

		public virtual void WriteIncludesForFieldDeclaration(CppCodeWriter writer)
		{
			if (this._typeRef.IsValueType())
			{
				writer.AddIncludeForTypeDefinition(this._typeRef);
			}
			else
			{
				this.WriteMarshaledTypeForwardDeclaration(writer);
			}
		}

		public virtual void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
		{
			if (!this._typeRef.IsEnum() && !this._typeRef.IsSystemObject())
			{
				writer.AddForwardDeclaration(string.Format("struct {0}", this.MarshaledTypeName));
			}
		}

		public virtual void WriteIncludesForMarshaling(CppCodeWriter writer)
		{
			writer.AddIncludeForTypeDefinition(this._typeRef.Resolve());
		}

		public virtual void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			writer.WriteLine("{1} = {0};", new object[]
			{
				sourceVariable.Load(),
				destinationVariable
			});
		}

		public virtual string WriteMarshalEmptyVariableToNative(CppCodeWriter writer, string variableName)
		{
			return variableName;
		}

		public virtual void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			writer.WriteLine(destinationVariable.Store(variableName));
		}

		public virtual void WriteMarshalOutParameterFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			this.WriteMarshalVariableFromNative(writer, variableName, destinationVariable, methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess);
		}

		public virtual string WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			return sourceVariable.Load();
		}

		public virtual string WriteMarshalEmptyVariableFromNative(CppCodeWriter writer, string variableName)
		{
			string text = string.Format("_{0}_empty", variableName.Replace("*", ""));
			writer.WriteVariable(this._typeRef, text);
			return text;
		}

		public virtual void WriteMarshalOutVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, IRuntimeMetadataAccess metadataAccess)
		{
		}

		public virtual string WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			return variableName;
		}

		public virtual void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
		{
		}

		public virtual void WriteMarshalCleanupEmptyVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName)
		{
			this.WriteMarshalCleanupVariable(writer, variableName, metadataAccess, managedVariableName);
		}

		public virtual void WriteDeclareAndAllocateObject(CppCodeWriter writer, string unmarshaledVariableName, string marshaledVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			writer.WriteVariable(this._typeRef, unmarshaledVariableName);
		}

		public virtual string DecorateVariable(string unmarshaledParameterName, string marshaledVariableName)
		{
			return marshaledVariableName;
		}

		public virtual string UndecorateVariable(string variableName)
		{
			return variableName;
		}

		public virtual bool CanMarshalTypeToNative()
		{
			return true;
		}

		public virtual bool CanMarshalTypeFromNative()
		{
			return this.CanMarshalTypeToNative();
		}

		public virtual string GetMarshalingException()
		{
			throw new NotSupportedException(string.Format("Cannot retrieve marshaling exception for type ({0}) that can be marshaled.", new object[0]));
		}

		public virtual string GetMarshalingFromNativeException()
		{
			return this.GetMarshalingException();
		}

		public virtual void WriteNativeVariableDeclarationOfType(CppCodeWriter writer, string variableName)
		{
			if (this.MarshaledTypeName.EndsWith("*"))
			{
				writer.WriteLine("{0} {1} = NULL;", new object[]
				{
					this.MarshaledTypeName,
					variableName
				});
			}
			else if (this._typeRef.MetadataType == MetadataType.Class && !this._typeRef.DerivesFromObject())
			{
				writer.WriteLine("{0} {1} = {0}();", new object[]
				{
					this.MarshaledTypeName,
					variableName
				});
			}
			else
			{
				writer.WriteLine("{0} {1} = {{ }};", new object[]
				{
					this.MarshaledTypeName,
					variableName
				});
			}
		}
	}
}
