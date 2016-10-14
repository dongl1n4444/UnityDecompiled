using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	public abstract class CustomMarshalInfoWriter : MarshalableMarshalInfoWriter
	{
		protected readonly TypeDefinition _type;

		protected readonly MarshalType _marshalType;

		private readonly MethodDefinition _defaultConstructor;

		protected readonly string _marshaledTypeName;

		protected readonly string _marshaledDecoratedTypeName;

		protected readonly string _marshalToNativeFunctionName;

		protected readonly string _marshalFromNativeFunctionName;

		protected readonly string _marshalCleanupFunctionName;

		protected readonly string _marshalToNativeFunctionDeclaration;

		protected readonly string _marshalFromNativeFunctionDeclaration;

		protected readonly string _marshalCleanupFunctionDeclaration;

		private FieldDefinition[] _fields;

		private DefaultMarshalInfoWriter[] _fieldMarshalInfoWriters;

		public sealed override string MarshaledTypeName
		{
			get
			{
				return this._marshaledTypeName;
			}
		}

		public sealed override string MarshaledDecoratedTypeName
		{
			get
			{
				return this._marshaledDecoratedTypeName;
			}
		}

		public sealed override bool HasMarshalFunctions
		{
			get
			{
				return true;
			}
		}

		public sealed override string MarshalToNativeFunctionName
		{
			get
			{
				return this._marshalToNativeFunctionName;
			}
		}

		public sealed override string MarshalFromNativeFunctionName
		{
			get
			{
				return this._marshalFromNativeFunctionName;
			}
		}

		public sealed override string MarshalCleanupFunctionName
		{
			get
			{
				return this._marshalCleanupFunctionName;
			}
		}

		public sealed override bool HasNativeStructDefinition
		{
			get
			{
				return true;
			}
		}

		protected FieldDefinition[] Fields
		{
			get
			{
				if (this._fields == null)
				{
					this.PopulateFields();
				}
				return this._fields;
			}
		}

		protected DefaultMarshalInfoWriter[] FieldMarshalInfoWriters
		{
			get
			{
				if (this._fieldMarshalInfoWriters == null)
				{
					this.PopulateFields();
				}
				return this._fieldMarshalInfoWriters;
			}
		}

		protected CustomMarshalInfoWriter(TypeDefinition type, MarshalType marshalType) : base(type)
		{
			this._type = type;
			this._marshalType = marshalType;
			string text = DefaultMarshalInfoWriter.Naming.ForTypeNameOnly(type);
			string arg = '_' + MarshalingUtils.MarshalTypeToString(marshalType);
			this._marshaledTypeName = CustomMarshalInfoWriter.GetMarshaledTypeName(type, marshalType);
			this._marshaledDecoratedTypeName = ((!this._type.IsValueType) ? (this._marshaledTypeName + "*") : this._marshaledTypeName);
			this._marshalToNativeFunctionName = string.Format("{0}_marshal{1}", text, arg);
			this._marshalFromNativeFunctionName = string.Format("{0}_marshal{1}_back", text, arg);
			this._marshalCleanupFunctionName = string.Format("{0}_marshal{1}_cleanup", text, arg);
			this._marshalToNativeFunctionDeclaration = string.Format("extern \"C\" void {0}(const {1}& unmarshaled, {2}& marshaled)", this.MarshalToNativeFunctionName, text, this.MarshaledTypeName);
			this._marshalFromNativeFunctionDeclaration = string.Format("extern \"C\" void {0}(const {1}& marshaled, {2}& unmarshaled)", this._marshalFromNativeFunctionName, this.MarshaledTypeName, text);
			this._marshalCleanupFunctionDeclaration = string.Format("extern \"C\" void {0}({1}& marshaled)", this.MarshalCleanupFunctionName, this.MarshaledTypeName);
			this._defaultConstructor = this._type.Methods.SingleOrDefault((MethodDefinition ctor) => ctor.Name == ".ctor" && ctor.Parameters.Count == 0);
		}

		public override void WriteNativeStructDefinition(CppCodeWriter writer)
		{
			writer.WriteLine("// Native definition for marshalling of: {0}", new object[]
			{
				this._type.FullName
			});
			foreach (FieldDefinition current in MarshalingUtils.NonStaticFieldsOf(this._type))
			{
				CustomMarshalInfoWriter.MarshalInfoWriterFor(current.FieldType, this._marshalType, current.MarshalInfo, this._type.IsUnicodeClass).WriteIncludesForFieldDeclaration(writer);
			}
			bool flag = TypeDefinitionWriter.NeedsPackingForNative(this._type) && !this._type.IsExplicitLayout;
			if (flag)
			{
				writer.WriteLine("#pragma pack(push, tp, {0})", new object[]
				{
					TypeDefinitionWriter.FieldLayoutPackingSizeFor(this._type)
				});
			}
			writer.WriteLine("struct {0}{1}", new object[]
			{
				this.MarshaledTypeName,
				(this._type.BaseType == null || this._type.BaseType.IsSpecialSystemBaseType() || !MarshalDataCollector.MarshalInfoWriterFor(this._type.BaseType, this._marshalType, null, false, false).HasNativeStructDefinition) ? string.Empty : string.Format(" : public {0}", CustomMarshalInfoWriter.GetMarshaledTypeName(this._type.BaseType, this._marshalType))
			});
			writer.BeginBlock();
			using (new TypeDefinitionPaddingWriter(writer, this._type))
			{
				if (!this._type.IsExplicitLayout)
				{
					foreach (FieldDefinition current2 in MarshalingUtils.NonStaticFieldsOf(this._type))
					{
						CustomMarshalInfoWriter.MarshalInfoWriterFor(current2.FieldType, this._marshalType, current2.MarshalInfo, this._type.IsUnicodeClass).WriteFieldDeclaration(writer, current2, null);
					}
				}
				else
				{
					writer.WriteLine("union");
					writer.BeginBlock();
					foreach (FieldDefinition current3 in MarshalingUtils.NonStaticFieldsOf(this._type))
					{
						this.WriteFieldWithExplicitLayout(writer, current3, false);
						this.WriteFieldWithExplicitLayout(writer, current3, true);
					}
					writer.EndBlock(true);
				}
			}
			writer.EndBlock(true);
			if (flag)
			{
				writer.WriteLine("#pragma pack(pop, tp)");
			}
		}

		private void WriteFieldWithExplicitLayout(CppCodeWriter writer, FieldDefinition field, bool forAlignmentOnly)
		{
			int num = TypeDefinitionWriter.AlignmentPackingSizeFor(this._type);
			bool flag = (!forAlignmentOnly && TypeDefinitionWriter.NeedsPackingForNative(this._type)) || (num != -1 && num != 0);
			string text = (!forAlignmentOnly) ? string.Empty : "_forAlignmentOnly";
			int offset = field.Offset;
			if (flag)
			{
				writer.WriteLine("#pragma pack(push, tp, {0})", new object[]
				{
					(!forAlignmentOnly) ? TypeDefinitionWriter.FieldLayoutPackingSizeFor(this._type) : num
				});
			}
			writer.WriteLine("struct");
			writer.BeginBlock();
			if (offset > 0)
			{
				writer.WriteLine("char {0}[{1}];", new object[]
				{
					DefaultMarshalInfoWriter.Naming.ForFieldPadding(field) + text,
					offset
				});
			}
			CustomMarshalInfoWriter.MarshalInfoWriterFor(field.FieldType, this._marshalType, field.MarshalInfo, this._type.IsUnicodeClass).WriteFieldDeclaration(writer, field, text);
			writer.EndBlock(true);
			if (flag)
			{
				writer.WriteLine("#pragma pack(pop, tp)");
			}
		}

		public override void WriteMarshalFunctionDeclarations(CppCodeWriter writer)
		{
			writer.AddForwardDeclaration(this._type);
			writer.AddForwardDeclaration(string.Format("struct {0}", this._marshaledTypeName));
			writer.WriteLine();
			writer.WriteCommentedLine("Methods for marshaling");
			writer.WriteLine("struct {0};", new object[]
			{
				DefaultMarshalInfoWriter.Naming.ForTypeNameOnly(this._type)
			});
			writer.WriteLine("struct {0};", new object[]
			{
				this.MarshaledTypeName
			});
			writer.WriteLine();
			writer.WriteLine("{0};", new object[]
			{
				this._marshalToNativeFunctionDeclaration
			});
			writer.WriteLine("{0};", new object[]
			{
				this._marshalFromNativeFunctionDeclaration
			});
			writer.WriteLine("{0};", new object[]
			{
				this._marshalCleanupFunctionDeclaration
			});
		}

		public override void WriteMarshalFunctionDefinitions(CppCodeWriter writer)
		{
			for (int i = 0; i < this.Fields.Length; i++)
			{
				this.FieldMarshalInfoWriters[i].WriteIncludesForMarshaling(writer);
			}
			writer.WriteLine("// Conversion methods for marshalling of: {0}", new object[]
			{
				this._type.FullName
			});
			this.WriteMarshalToNativeMethodDefinition(writer);
			this.WriteMarshalFromNativeMethodDefinition(writer);
			writer.WriteLine("// Conversion method for clean up from marshalling of: {0}", new object[]
			{
				this._type.FullName
			});
			this.WriteMarshalCleanupFunction(writer);
		}

		protected abstract void WriteMarshalCleanupFunction(CppCodeWriter writer);

		protected abstract void WriteMarshalFromNativeMethodDefinition(CppCodeWriter writer);

		protected abstract void WriteMarshalToNativeMethodDefinition(CppCodeWriter writer);

		protected static DefaultMarshalInfoWriter MarshalInfoWriterFor(TypeReference type, MarshalType marshalType, MarshalInfo marshalInfo, bool useUnicodeCharSet)
		{
			return MarshalDataCollector.MarshalInfoWriterFor(type, marshalType, marshalInfo, useUnicodeCharSet, false);
		}

		public override void WriteIncludesForMarshaling(CppCodeWriter writer)
		{
			base.WriteIncludesForMarshaling(writer);
			writer.AddIncludeForMethodDeclarations(this._typeRef);
		}

		public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			string block = (!this._type.IsValueType) ? "if ({1} != {3}) {0}(*{1}, {2});" : "{0}({1}, {2});";
			writer.WriteLine(block, new object[]
			{
				this.MarshalToNativeFunctionName,
				sourceVariable.Load(),
				destinationVariable,
				DefaultMarshalInfoWriter.Naming.Null
			});
		}

		public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			if (this._type.IsValueType)
			{
				writer.WriteLine("{0}({1}, {2});", new object[]
				{
					this.MarshalFromNativeFunctionName,
					variableName,
					destinationVariable.Load()
				});
			}
			else
			{
				Action writeMarshalFromNativeCode = delegate
				{
					writer.WriteLine("{0}({1}, *{2});", new object[]
					{
						this.MarshalFromNativeFunctionName,
						variableName,
						destinationVariable.Load()
					});
				};
				CustomMarshalInfoWriter.EmitCallToConstructor(writer, this._type, this._defaultConstructor, variableName, destinationVariable, writeMarshalFromNativeCode, true, metadataAccess);
			}
		}

		public override void WriteDeclareAndAllocateObject(CppCodeWriter writer, string unmarshaledVariableName, string marshaledVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			if (this._type.IsValueType)
			{
				base.WriteDeclareAndAllocateObject(writer, unmarshaledVariableName, marshaledVariableName, metadataAccess);
			}
			else
			{
				CustomMarshalInfoWriter.EmitNewObject(writer, this._type, unmarshaledVariableName, marshaledVariableName, true, metadataAccess);
			}
		}

		public override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
		{
			if (this._type.IsValueType)
			{
				writer.WriteLine("{0}({1});", new object[]
				{
					this.MarshalCleanupFunctionName,
					variableName
				});
			}
			else if (!string.IsNullOrEmpty(managedVariableName))
			{
				writer.WriteLine("if ({0} != {1}) {2}({3});", new object[]
				{
					managedVariableName,
					DefaultMarshalInfoWriter.Naming.Null,
					this.MarshalCleanupFunctionName,
					variableName
				});
			}
			else
			{
				writer.WriteLine("if (&({0}) != {1}) {2}({3});", new object[]
				{
					variableName,
					DefaultMarshalInfoWriter.Naming.Null,
					this.MarshalCleanupFunctionName,
					variableName
				});
			}
		}

		public override string DecorateVariable(string unmarshaledParameterName, string marshaledVariableName)
		{
			string result;
			if (!this._type.IsValueType)
			{
				result = string.Format("{0} != {2} ? &{1} : {2}", unmarshaledParameterName, marshaledVariableName, DefaultMarshalInfoWriter.Naming.Null);
			}
			else
			{
				result = marshaledVariableName;
			}
			return result;
		}

		public override string UndecorateVariable(string variableName)
		{
			string result;
			if (!this._type.IsValueType)
			{
				result = "*" + variableName;
			}
			else
			{
				result = variableName;
			}
			return result;
		}

		internal static void EmitCallToConstructor(CppCodeWriter writer, TypeDefinition typeDefinition, MethodDefinition defaultConstructor, string variableName, ManagedMarshalValue destinationVariable, Action writeMarshalFromNativeCode, bool emitNullCheck, IRuntimeMetadataAccess metadataAccess)
		{
			if (emitNullCheck)
			{
				writer.WriteLine("if ({0} != {1})", new object[]
				{
					destinationVariable.Load(),
					DefaultMarshalInfoWriter.Naming.Null
				});
				writer.BeginBlock();
			}
			if (defaultConstructor != null)
			{
				if (MethodSignatureWriter.NeedsHiddenMethodInfo(defaultConstructor, MethodCallType.Normal, true))
				{
					writer.WriteLine("{0}({1}, {2});", new object[]
					{
						DefaultMarshalInfoWriter.Naming.ForMethodNameOnly(defaultConstructor),
						destinationVariable.Load(),
						metadataAccess.HiddenMethodInfo(defaultConstructor)
					});
				}
				else
				{
					writer.WriteLine("{0}({1});", new object[]
					{
						DefaultMarshalInfoWriter.Naming.ForMethodNameOnly(defaultConstructor),
						destinationVariable.Load()
					});
				}
			}
			else
			{
				writer.WriteStatement(Emit.RaiseManagedException(string.Format("il2cpp_codegen_get_missing_method_exception(\"A parameterless constructor is required for type '{0}'.\")", typeDefinition.FullName)));
			}
			writeMarshalFromNativeCode();
			if (emitNullCheck)
			{
				writer.EndBlock(false);
			}
		}

		internal static void EmitNewObject(CppCodeWriter writer, TypeReference typeReference, string unmarshaledVariableName, string marshaledVariableName, bool emitNullCheck, IRuntimeMetadataAccess metadataAccess)
		{
			if (emitNullCheck)
			{
				writer.WriteLine("{0} {1} = ({2} != {4}) ? ({0})il2cpp_codegen_object_new({3}) : {4};", new object[]
				{
					DefaultMarshalInfoWriter.Naming.ForVariable(typeReference),
					unmarshaledVariableName,
					marshaledVariableName,
					metadataAccess.TypeInfoFor(typeReference),
					DefaultMarshalInfoWriter.Naming.Null
				});
			}
			else
			{
				writer.WriteLine("{0} {1} = ({0})il2cpp_codegen_object_new({2});", new object[]
				{
					DefaultMarshalInfoWriter.Naming.ForVariable(typeReference),
					unmarshaledVariableName,
					metadataAccess.TypeInfoFor(typeReference)
				});
			}
		}

		private static string GetMarshaledTypeName(TypeReference type, MarshalType marshalType)
		{
			return string.Format("{0}_marshaled_{1}", DefaultMarshalInfoWriter.Naming.ForTypeNameOnly(type), MarshalingUtils.MarshalTypeToString(marshalType));
		}

		private void PopulateFields()
		{
			this._fields = (from t in this._type.GetTypeHierarchy()
			where t == this._type || MarshalDataCollector.MarshalInfoWriterFor(t, this._marshalType, null, false, false).HasNativeStructDefinition
			select t).SelectMany((TypeDefinition t) => MarshalingUtils.NonStaticFieldsOf(t)).ToArray<FieldDefinition>();
			this._fieldMarshalInfoWriters = (from f in this._fields
			select (f.FieldType.Resolve() != this._type) ? CustomMarshalInfoWriter.MarshalInfoWriterFor(f.FieldType, this._marshalType, f.MarshalInfo, this._type.IsUnicodeClass) : this).ToArray<DefaultMarshalInfoWriter>();
		}
	}
}
