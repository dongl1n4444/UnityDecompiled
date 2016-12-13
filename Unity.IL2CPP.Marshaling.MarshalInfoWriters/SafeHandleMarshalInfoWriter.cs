using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	public class SafeHandleMarshalInfoWriter : MarshalableMarshalInfoWriter
	{
		private const string SafeHandleReferenceIncrementedLocalBoolNamePrefix = "___safeHandle_reference_incremented_for";

		private static int _unsusedBoolNameCounter = 1;

		private readonly TypeDefinition _safeHandleTypeDefinition;

		private readonly MethodDefinition _addRefMethod;

		private readonly MethodDefinition _releaseMethod;

		private readonly MethodDefinition _defaultConstructor;

		private readonly MarshaledType[] _marshaledTypes;

		public override MarshaledType[] MarshaledTypes
		{
			get
			{
				return this._marshaledTypes;
			}
		}

		public SafeHandleMarshalInfoWriter(TypeReference type, TypeDefinition safeHandleTypeTypeDefinition) : base(type)
		{
			this._safeHandleTypeDefinition = safeHandleTypeTypeDefinition;
			this._addRefMethod = this._safeHandleTypeDefinition.Methods.Single((MethodDefinition method) => method.Name == "DangerousAddRef");
			this._releaseMethod = this._safeHandleTypeDefinition.Methods.Single((MethodDefinition method) => method.Name == "DangerousRelease");
			this._defaultConstructor = this._typeRef.Resolve().Methods.SingleOrDefault((MethodDefinition ctor) => ctor.Name == ".ctor" && ctor.Parameters.Count == 0);
			this._marshaledTypes = new MarshaledType[]
			{
				new MarshaledType("void*", "void*")
			};
		}

		public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			writer.WriteLine("if ({0} == {1}) {2};", new object[]
			{
				sourceVariable.Load(),
				DefaultMarshalInfoWriter.Naming.Null,
				Emit.RaiseManagedException(string.Format("il2cpp_codegen_get_argument_null_exception(\"{0}\")", (!string.IsNullOrEmpty(managedVariableName)) ? managedVariableName : sourceVariable.GetNiceName()))
			});
			this.EmitCallToDangerousAddRef(writer, sourceVariable.Load(), false, metadataAccess);
			writer.WriteLine("{0} = {1};", new object[]
			{
				destinationVariable,
				this.LoadHandleFieldFor(sourceVariable.Load())
			});
		}

		public override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
		{
			if (!string.IsNullOrEmpty(managedVariableName))
			{
				writer.WriteLine("if ({0})", new object[]
				{
					this.SafeHandleReferenceIncrementedLocalBoolName(managedVariableName)
				});
				writer.BeginBlock();
				writer.WriteLine("{0};", new object[]
				{
					Emit.Call(DefaultMarshalInfoWriter.Naming.ForMethod(this._releaseMethod), new string[]
					{
						managedVariableName,
						metadataAccess.HiddenMethodInfo(this._releaseMethod)
					})
				});
				writer.EndBlock(false);
			}
		}

		public override void WriteMarshalCleanupEmptyVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName)
		{
		}

		public override void WriteDeclareAndAllocateObject(CppCodeWriter writer, string unmarshaledVariableName, string marshaledVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			TypeDefinition typeDefinition = this._typeRef.Resolve();
			if (typeDefinition.IsAbstract)
			{
				writer.WriteStatement(Emit.RaiseManagedException(string.Format("il2cpp_codegen_get_marshal_directive_exception(\"A returned SafeHandle cannot be abstract, but this type is: '{0}'.\")", typeDefinition.FullName)));
			}
			CustomMarshalInfoWriter.EmitNewObject(writer, this._typeRef, unmarshaledVariableName, marshaledVariableName, false, metadataAccess);
		}

		public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			if (forNativeWrapperOfManagedMethod)
			{
				writer.WriteStatement(Emit.RaiseManagedException("il2cpp_codegen_get_marshal_directive_exception(\"Cannot marshal a SafeHandle from unmanaged to managed.\")"));
			}
			else
			{
				Action writeMarshalFromNativeCode = delegate
				{
					string text = destinationVariable.GetNiceName() + "_handle_temp";
					writer.WriteLine("{0} {1};", new object[]
					{
						DefaultMarshalInfoWriter.Naming.ForVariable(DefaultMarshalInfoWriter.TypeProvider.SystemIntPtr),
						text
					});
					writer.WriteLine("{0}.{1}({2});", new object[]
					{
						text,
						DefaultMarshalInfoWriter.Naming.ForFieldSetter(this.GetIntPtrValueField()),
						variableName
					});
					writer.WriteLine("({0})->{1}({2});", new object[]
					{
						destinationVariable.Load(),
						DefaultMarshalInfoWriter.Naming.ForFieldSetter(this.GetSafeHandleHandleField()),
						text
					});
				};
				CustomMarshalInfoWriter.EmitCallToConstructor(writer, this._typeRef.Resolve(), this._defaultConstructor, variableName, destinationVariable, writeMarshalFromNativeCode, false, metadataAccess);
				if (!returnValue)
				{
					this.EmitCallToDangerousAddRef(writer, destinationVariable.Load(), true, metadataAccess);
				}
			}
		}

		public override void WriteIncludesForMarshaling(CppCodeWriter writer)
		{
			writer.AddIncludeForMethodDeclarations(this._typeRef);
			writer.AddIncludeForMethodDeclarations(this._safeHandleTypeDefinition);
		}

		private string LoadHandleFieldFor(string sourceVariable)
		{
			return string.Format("({0})->{1}().{2}()", sourceVariable, DefaultMarshalInfoWriter.Naming.ForFieldGetter(this.GetSafeHandleHandleField()), DefaultMarshalInfoWriter.Naming.ForFieldGetter(this.GetIntPtrValueField()));
		}

		private FieldReference GetIntPtrValueField()
		{
			return DefaultMarshalInfoWriter.TypeProvider.SystemIntPtr.Fields.Single((FieldDefinition f) => f.Name == DefaultMarshalInfoWriter.Naming.IntPtrValueField);
		}

		private FieldReference GetSafeHandleHandleField()
		{
			return this._safeHandleTypeDefinition.Fields.Single((FieldDefinition f) => f.Name == "handle");
		}

		private string SafeHandleReferenceIncrementedLocalBoolName(string variableName)
		{
			return string.Format("{0}_{1}", "___safeHandle_reference_incremented_for", DefaultMarshalInfoWriter.CleanVariableName(variableName));
		}

		private void EmitCallToDangerousAddRef(CppCodeWriter writer, string variableName, bool generateBoolName, IRuntimeMetadataAccess metadataAccess)
		{
			string text = (!generateBoolName) ? this.SafeHandleReferenceIncrementedLocalBoolName(variableName) : this.SafeHandleReferenceIncrementedLocalBoolName(string.Format("unused{0}", SafeHandleMarshalInfoWriter._unsusedBoolNameCounter++));
			writer.WriteLine("bool {0} = false;", new object[]
			{
				text
			});
			writer.WriteLine("{0};", new object[]
			{
				Emit.Call(DefaultMarshalInfoWriter.Naming.ForMethod(this._addRefMethod), new string[]
				{
					variableName,
					DefaultMarshalInfoWriter.Naming.AddressOf(text),
					metadataAccess.HiddenMethodInfo(this._addRefMethod)
				})
			});
		}

		public override void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
		{
		}
	}
}
