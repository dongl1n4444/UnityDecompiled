using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;
using Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative;
using Unity.IL2CPP.Marshaling.BodyWriters.NativeToManaged;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	internal sealed class WindowsRuntimeDelegateMarshalInfoWriter : MarshalableMarshalInfoWriter
	{
		[Inject]
		public static IWindowsRuntimeProjections WindowsRuntimeProjections;

		private readonly TypeResolver _typeResolver;

		private readonly MethodReference _invokeMethod;

		private readonly string _comCallableWrapperInterfaceName;

		private readonly string _comCallableWrapperClassName;

		private readonly string _parameterList;

		private readonly string _nativeInvokerName;

		private readonly string _nativeInvokerSignature;

		private readonly MarshaledType[] _marshaledTypes;

		public override MarshaledType[] MarshaledTypes
		{
			get
			{
				return this._marshaledTypes;
			}
		}

		public WindowsRuntimeDelegateMarshalInfoWriter(TypeReference type) : base(type)
		{
			TypeDefinition typeDefinition = type.Resolve();
			if (!typeDefinition.IsDelegate())
			{
				throw new ArgumentException(string.Format("WindowsRuntimeDelegateMarshalInfoWriter cannot marshal non-delegate type {0}.", type.FullName));
			}
			this._typeResolver = TypeResolver.For(type);
			this._invokeMethod = this._typeResolver.Resolve(typeDefinition.Methods.Single((MethodDefinition m) => m.Name == "Invoke"));
			this._comCallableWrapperClassName = DefaultMarshalInfoWriter.Naming.ForWindowsRuntimeDelegateComCallableWrapperClass(type);
			this._comCallableWrapperInterfaceName = DefaultMarshalInfoWriter.Naming.ForWindowsRuntimeDelegateComCallableWrapperInterface(type);
			this._nativeInvokerName = DefaultMarshalInfoWriter.Naming.ForWindowsRuntimeDelegateNativeInvokerMethod(this._invokeMethod);
			this._parameterList = ComInterfaceWriter.BuildMethodParameterList(this._invokeMethod, this._invokeMethod, this._typeResolver, MarshalType.WindowsRuntime, true);
			this._marshaledTypes = new MarshaledType[]
			{
				new MarshaledType(this._comCallableWrapperInterfaceName, this._comCallableWrapperInterfaceName + '*')
			};
			string returnType = DefaultMarshalInfoWriter.Naming.ForVariable(this._typeResolver.Resolve(GenericParameterResolver.ResolveReturnTypeIfNeeded(this._invokeMethod)));
			string parameters = string.Format("{0} {1}, {2}", DefaultMarshalInfoWriter.Naming.ForVariable(DefaultMarshalInfoWriter.TypeProvider.Il2CppComObjectTypeReference), DefaultMarshalInfoWriter.Naming.ThisParameterName, MethodSignatureWriter.FormatParameters(this._invokeMethod, ParameterFormat.WithTypeAndNameNoThis, false, true));
			this._nativeInvokerSignature = MethodSignatureWriter.GetMethodSignature(this._nativeInvokerName, returnType, parameters, "extern \"C\"", string.Empty);
		}

		public override void WriteNativeStructDefinition(CppCodeWriter writer)
		{
			foreach (ParameterDefinition current in this._invokeMethod.Parameters)
			{
				MarshalDataCollector.MarshalInfoWriterFor(this._typeResolver.Resolve(current.ParameterType), MarshalType.WindowsRuntime, null, true, false, false, null).WriteMarshaledTypeForwardDeclaration(writer);
			}
			MarshalDataCollector.MarshalInfoWriterFor(this._typeResolver.Resolve(this._invokeMethod.ReturnType), MarshalType.WindowsRuntime, null, true, false, false, null).WriteMarshaledTypeForwardDeclaration(writer);
			writer.WriteCommentedLine(string.Format("COM Callable Wrapper interface definition for {0}", this._typeRef.FullName));
			writer.WriteLine("struct {0} : Il2CppIUnknown", new object[]
			{
				this._comCallableWrapperInterfaceName
			});
			using (new BlockWriter(writer, true))
			{
				writer.WriteLine("static const Il2CppGuid IID;");
				writer.WriteLine("virtual il2cpp_hresult_t STDCALL Invoke({0}) = 0;", new object[]
				{
					this._parameterList
				});
			}
			writer.WriteLine();
			string text = string.Format("il2cpp::vm::ComObjectBase<{0}, {1}, Il2CppIInspectable>", this._comCallableWrapperClassName, this._comCallableWrapperInterfaceName);
			writer.WriteCommentedLine(string.Format("COM Callable Wrapper class definition for {0}", this._typeRef.FullName));
			writer.WriteLine(string.Format("struct {0} : {1}", this._comCallableWrapperClassName, text));
			using (new BlockWriter(writer, true))
			{
				writer.WriteLine("inline {0}({1} obj) : ", new object[]
				{
					this._comCallableWrapperClassName,
					DefaultMarshalInfoWriter.Naming.ForVariable(DefaultMarshalInfoWriter.TypeProvider.ObjectTypeReference)
				});
				writer.Indent(1);
				writer.WriteLine(string.Format("{0}(obj)", text));
				writer.Dedent(1);
				using (new BlockWriter(writer, false))
				{
				}
				writer.WriteLine("virtual il2cpp_hresult_t STDCALL Invoke({0});", new object[]
				{
					this._parameterList
				});
			}
		}

		public override void WriteMarshalFunctionDeclarations(CppCodeWriter writer)
		{
			writer.WriteStatement(this._nativeInvokerSignature);
		}

		public override void WriteMarshalFunctionDefinitions(CppCodeWriter writer, IMethodCollector methodCollector)
		{
			writer.WriteLine("const Il2CppGuid {0}::IID = {1};", new object[]
			{
				this._comCallableWrapperInterfaceName,
				WindowsRuntimeDelegateMarshalInfoWriter.WindowsRuntimeProjections.ProjectToWindowsRuntime(this._typeRef).GetGuid().ToInitializer()
			});
			this.WriteCreateComCallableWrapperFunction(writer);
			this.WriteNativeInvoker(writer);
			this.WriteManagedInvoker(writer);
			methodCollector.AddCCWMarshallingFunction(this._typeRef.Resolve());
		}

		private void WriteCreateComCallableWrapperFunction(CppCodeWriter writer)
		{
			writer.WriteCommentedLine(string.Format("Create COM Callable Wrapper function for {0}", this._typeRef.FullName));
			writer.WriteLine("extern \"C\" Il2CppIManagedObjectHolder* {0}(Il2CppObject* obj)", new object[]
			{
				DefaultMarshalInfoWriter.Naming.ForCreateComCallableWrapperFunction(this._typeRef)
			});
			using (new BlockWriter(writer, false))
			{
				writer.WriteLine("return {0}::__CreateInstance(obj);", new object[]
				{
					this._comCallableWrapperClassName
				});
			}
		}

		private void WriteNativeInvoker(CppCodeWriter writer)
		{
			writer.WriteCommentedLine(string.Format("Native invoker for {0}", this._typeRef.FullName));
			MethodWriter.WriteMethodWithMetadataInitialization(writer, this._nativeInvokerSignature, this._invokeMethod.FullName, delegate(CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage)
			{
				IRuntimeMetadataAccess defaultRuntimeMetadataAccess = MethodWriter.GetDefaultRuntimeMetadataAccess(this._invokeMethod, metadataUsage, methodUsage);
				new WindowsRuntimeDelegateMethodBodyWriter(this._invokeMethod).WriteMethodBody(bodyWriter, defaultRuntimeMetadataAccess);
			}, this._nativeInvokerName);
		}

		private void WriteManagedInvoker(CppCodeWriter writer)
		{
			writer.WriteCommentedLine(string.Format("COM Callable invoker for {0}", this._typeRef.FullName));
			string methodSignature = string.Format("il2cpp_hresult_t STDCALL {0}::Invoke({1})", this._comCallableWrapperClassName, this._parameterList);
			MethodWriter.WriteMethodWithMetadataInitialization(writer, methodSignature, this._invokeMethod.FullName, delegate(CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage)
			{
				IRuntimeMetadataAccess defaultRuntimeMetadataAccess = MethodWriter.GetDefaultRuntimeMetadataAccess(this._invokeMethod, metadataUsage, methodUsage);
				new ComCallableWrapperMethodBodyWriter(this._invokeMethod, this._invokeMethod, MarshalType.WindowsRuntime).WriteMethodBody(bodyWriter, defaultRuntimeMetadataAccess);
			}, DefaultMarshalInfoWriter.Naming.ForMethod(this._invokeMethod) + "_WindowsRuntimeManagedInvoker");
		}

		public override void WriteNativeVariableDeclarationOfType(CppCodeWriter writer, string variableName)
		{
			writer.WriteLine("{0}* {1} = {2};", new object[]
			{
				this._comCallableWrapperInterfaceName,
				variableName,
				DefaultMarshalInfoWriter.Naming.Null
			});
		}

		public override string WriteMarshalEmptyVariableToNative(CppCodeWriter writer, ManagedMarshalValue variableName, IList<MarshaledParameter> methodParameters)
		{
			return DefaultMarshalInfoWriter.Naming.Null;
		}

		public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			writer.WriteLine("if ({0} != {1})", new object[]
			{
				sourceVariable.Load(),
				DefaultMarshalInfoWriter.Naming.Null
			});
			using (new BlockWriter(writer, false))
			{
				FieldDefinition field = DefaultMarshalInfoWriter.TypeProvider.SystemDelegate.Fields.Single((FieldDefinition f) => f.Name == "m_target");
				string text = DefaultMarshalInfoWriter.Naming.ForFieldGetter(field);
				FieldDefinition field2 = DefaultMarshalInfoWriter.TypeProvider.SystemMulticastDelegate.Fields.Single((FieldDefinition f) => f.Name == ((CodeGenOptions.Dotnetprofile != DotNetProfile.Net45) ? "prev" : "delegates"));
				string text2 = DefaultMarshalInfoWriter.Naming.ForFieldGetter(field2);
				writer.WriteLine("Il2CppObject* target = {0}->{1}();", new object[]
				{
					sourceVariable.Load(),
					text
				});
				writer.WriteLine();
				writer.WriteLine("if (target != {0} && {1}->{2}() == {0} && target->klass == {3})", new object[]
				{
					DefaultMarshalInfoWriter.Naming.Null,
					sourceVariable.Load(),
					text2,
					metadataAccess.TypeInfoFor(DefaultMarshalInfoWriter.TypeProvider.Il2CppComDelegateTypeReference)
				});
				using (new BlockWriter(writer, false))
				{
					writer.WriteLine("il2cpp_hresult_t {0} = static_cast<{1}>(target)->{2}->QueryInterface({3}::IID, reinterpret_cast<void**>(&{4}));", new object[]
					{
						DefaultMarshalInfoWriter.Naming.ForInteropHResultVariable(),
						DefaultMarshalInfoWriter.Naming.ForVariable(DefaultMarshalInfoWriter.TypeProvider.Il2CppComObjectTypeReference),
						DefaultMarshalInfoWriter.Naming.ForIl2CppComObjectIdentityField(),
						this._comCallableWrapperInterfaceName,
						destinationVariable
					});
					writer.WriteStatement(Emit.Call("il2cpp_codegen_com_raise_exception_if_failed", DefaultMarshalInfoWriter.Naming.ForInteropHResultVariable()));
				}
				writer.WriteLine("else");
				using (new BlockWriter(writer, false))
				{
					writer.WriteLine("{0} = il2cpp_codegen_com_get_or_create_ccw<{1}>({2});", new object[]
					{
						destinationVariable,
						this._comCallableWrapperInterfaceName,
						sourceVariable.Load()
					});
				}
			}
			writer.WriteLine("else");
			using (new BlockWriter(writer, false))
			{
				writer.WriteLine("{0} = {1};", new object[]
				{
					destinationVariable,
					DefaultMarshalInfoWriter.Naming.Null
				});
			}
		}

		public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			writer.WriteLine("if ({0} != {1})", new object[]
			{
				variableName,
				DefaultMarshalInfoWriter.Naming.Null
			});
			using (new BlockWriter(writer, false))
			{
				writer.WriteLine("Il2CppIManagedObjectHolder* imanagedObject = {0};", new object[]
				{
					DefaultMarshalInfoWriter.Naming.Null
				});
				writer.WriteLine("il2cpp_hresult_t {0} = {1}->QueryInterface(Il2CppIManagedObjectHolder::IID, reinterpret_cast<void**>(&imanagedObject));", new object[]
				{
					DefaultMarshalInfoWriter.Naming.ForInteropHResultVariable(),
					variableName
				});
				writer.WriteLine("if (IL2CPP_HR_SUCCEEDED({0}))", new object[]
				{
					DefaultMarshalInfoWriter.Naming.ForInteropHResultVariable()
				});
				using (new BlockWriter(writer, false))
				{
					writer.WriteLine(destinationVariable.Store("static_cast<{0}>(imanagedObject->GetManagedObject())", new object[]
					{
						DefaultMarshalInfoWriter.Naming.ForVariable(this._typeRef)
					}));
					writer.WriteLine("imanagedObject->Release();");
				}
				writer.WriteLine("else");
				using (new BlockWriter(writer, false))
				{
					FieldDefinition field = DefaultMarshalInfoWriter.TypeProvider.SystemDelegate.Fields.Single((FieldDefinition f) => f.Name == "method_ptr");
					string text = DefaultMarshalInfoWriter.Naming.ForFieldSetter(field);
					FieldDefinition field2 = DefaultMarshalInfoWriter.TypeProvider.SystemDelegate.Fields.Single((FieldDefinition f) => f.Name == "method");
					string text2 = DefaultMarshalInfoWriter.Naming.ForFieldSetter(field2);
					FieldDefinition field3 = DefaultMarshalInfoWriter.TypeProvider.SystemDelegate.Fields.Single((FieldDefinition f) => f.Name == "m_target");
					string text3 = DefaultMarshalInfoWriter.Naming.ForFieldSetter(field3);
					writer.WriteLine(destinationVariable.Store(Emit.NewObj(this._typeRef, metadataAccess)));
					writer.WriteLine("{0}->{1}((Il2CppMethodPointer){2});", new object[]
					{
						destinationVariable.Load(),
						text,
						this._nativeInvokerName
					});
					writer.WriteLine("{0} methodInfo;", new object[]
					{
						DefaultMarshalInfoWriter.Naming.ForVariable(DefaultMarshalInfoWriter.TypeProvider.SystemIntPtr)
					});
					string arg_276_1 = "methodInfo.{0}((void*){1});";
					object[] expr_228 = new object[2];
					expr_228[0] = DefaultMarshalInfoWriter.Naming.ForFieldSetter(DefaultMarshalInfoWriter.TypeProvider.SystemIntPtr.Fields.Single((FieldDefinition f) => f.Name == DefaultMarshalInfoWriter.Naming.IntPtrValueField));
					expr_228[1] = metadataAccess.MethodInfo(this._invokeMethod);
					writer.WriteLine(arg_276_1, expr_228);
					writer.WriteLine("{0}->{1}(methodInfo);", new object[]
					{
						destinationVariable.Load(),
						text2
					});
					writer.WriteLine("{0}->{1}(il2cpp_codegen_com_get_or_create_rcw_for_sealed_class<{2}>({3}, {4}));", new object[]
					{
						destinationVariable.Load(),
						text3,
						DefaultMarshalInfoWriter.Naming.ForTypeNameOnly(DefaultMarshalInfoWriter.TypeProvider.Il2CppComDelegateTypeReference),
						variableName,
						metadataAccess.TypeInfoFor(DefaultMarshalInfoWriter.TypeProvider.Il2CppComDelegateTypeReference)
					});
					writer.AddIncludeForTypeDefinition(DefaultMarshalInfoWriter.TypeProvider.Il2CppComDelegateTypeReference);
					writer.AddIncludeForMethodDeclarations(this._typeRef);
				}
			}
			writer.WriteLine("else");
			using (new BlockWriter(writer, false))
			{
				writer.WriteLine(destinationVariable.Store(DefaultMarshalInfoWriter.Naming.Null));
			}
		}

		public override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
		{
			writer.WriteLine("if ({0} != {1})", new object[]
			{
				variableName,
				DefaultMarshalInfoWriter.Naming.Null
			});
			using (new BlockWriter(writer, false))
			{
				writer.WriteLine("({0})->Release();", new object[]
				{
					variableName
				});
				writer.WriteLine("{0} = {1};", new object[]
				{
					variableName,
					DefaultMarshalInfoWriter.Naming.Null
				});
			}
		}
	}
}
