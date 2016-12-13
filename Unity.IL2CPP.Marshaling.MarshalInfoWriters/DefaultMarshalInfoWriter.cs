using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
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

		protected static char[] CharactersToReplaceWithUnderscore = new char[]
		{
			'.',
			'[',
			']'
		};

		protected static char[] CharactersToRemove = new char[]
		{
			'*',
			'(',
			')'
		};

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
				return (from t in this.MarshaledTypes
				select string.Format("sizeof({0})", t.Name)).Aggregate((string x, string y) => x + " + " + y);
			}
		}

		public abstract MarshaledType[] MarshaledTypes
		{
			get;
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

		public virtual void WriteMarshalFunctionDefinitions(CppCodeWriter writer, IMethodCollector methodCollector)
		{
		}

		public virtual void WriteFieldDeclaration(CppCodeWriter writer, FieldReference field, string fieldNameSuffix = null)
		{
			MarshaledType[] marshaledTypes = this.MarshaledTypes;
			for (int i = 0; i < marshaledTypes.Length; i++)
			{
				MarshaledType marshaledType = marshaledTypes[i];
				string text = DefaultMarshalInfoWriter.Naming.ForField(field) + marshaledType.VariableName + fieldNameSuffix;
				writer.WriteLine("{0} {1};", new object[]
				{
					marshaledType.DecoratedName,
					text
				});
			}
		}

		public virtual void WriteIncludesForFieldDeclaration(CppCodeWriter writer)
		{
			if (this.TreatAsValueType())
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
				MarshaledType[] marshaledTypes = this.MarshaledTypes;
				for (int i = 0; i < marshaledTypes.Length; i++)
				{
					MarshaledType marshaledType = marshaledTypes[i];
					writer.AddForwardDeclaration(string.Format("struct {0}", marshaledType.Name));
				}
			}
		}

		public virtual void WriteIncludesForMarshaling(CppCodeWriter writer)
		{
			writer.AddIncludeForTypeDefinition(this._typeRef);
		}

		public virtual void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			writer.WriteLine("{1} = {0};", new object[]
			{
				sourceVariable.Load(),
				destinationVariable
			});
		}

		public virtual string WriteMarshalEmptyVariableToNative(CppCodeWriter writer, ManagedMarshalValue variableName, IList<MarshaledParameter> methodParameters)
		{
			return variableName.Load();
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

		public virtual string WriteMarshalEmptyVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, IRuntimeMetadataAccess metadataAccess)
		{
			string text = string.Format("_{0}_empty", DefaultMarshalInfoWriter.CleanVariableName(variableName));
			writer.WriteVariable(this._typeRef, text);
			return text;
		}

		public virtual void WriteMarshalOutParameterToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IList<MarshaledParameter> methodParameters, IRuntimeMetadataAccess metadataAccess)
		{
		}

		public virtual string WriteMarshalReturnValueToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, IRuntimeMetadataAccess metadataAccess)
		{
			return this.WriteMarshalVariableToNative(writer, sourceVariable, null, metadataAccess);
		}

		public virtual string WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			return variableName;
		}

		public virtual void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
		{
		}

		public virtual void WriteMarshalCleanupReturnValue(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess)
		{
			this.WriteMarshalCleanupVariable(writer, variableName, metadataAccess, null);
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
			MarshaledType[] marshaledTypes = this.MarshaledTypes;
			for (int i = 0; i < marshaledTypes.Length; i++)
			{
				MarshaledType marshaledType = marshaledTypes[i];
				if (marshaledType.Name.EndsWith("*") || marshaledType.Name == "Il2CppHString")
				{
					writer.WriteLine("{0} {1} = NULL;", new object[]
					{
						marshaledType.Name,
						variableName + marshaledType.VariableName
					});
				}
				else if (this._typeRef.MetadataType == MetadataType.Class && !this._typeRef.DerivesFromObject())
				{
					writer.WriteLine("{0} {1} = {0}();", new object[]
					{
						marshaledType.Name,
						variableName + marshaledType.VariableName
					});
				}
				else if (this._typeRef.MetadataType.IsPrimitiveType())
				{
					writer.WriteLine("{0} {1} = {2};", new object[]
					{
						marshaledType.Name,
						variableName + marshaledType.VariableName,
						CppCodeWriter.InitializerStringForPrimitiveType(this._typeRef.MetadataType)
					});
				}
				else if (marshaledType.Name.IsPrimitiveCppType())
				{
					writer.WriteLine("{0} {1} = {2};", new object[]
					{
						marshaledType.Name,
						variableName + marshaledType.VariableName,
						CppCodeWriter.InitializerStringForPrimitiveCppType(marshaledType.Name)
					});
				}
				else
				{
					writer.WriteLine("{0} {1} = {{ }};", new object[]
					{
						marshaledType.Name,
						variableName + marshaledType.VariableName
					});
				}
			}
		}

		public virtual bool TreatAsValueType()
		{
			return this._typeRef.IsValueType();
		}

		protected static string CleanVariableName(string variableName)
		{
			char[] charactersToReplaceWithUnderscore = DefaultMarshalInfoWriter.CharactersToReplaceWithUnderscore;
			for (int i = 0; i < charactersToReplaceWithUnderscore.Length; i++)
			{
				char oldChar = charactersToReplaceWithUnderscore[i];
				variableName = variableName.Replace(oldChar, '_');
			}
			variableName = string.Concat(variableName.Split(DefaultMarshalInfoWriter.CharactersToRemove, StringSplitOptions.None));
			return DefaultMarshalInfoWriter.Naming.Clean(variableName);
		}
	}
}
