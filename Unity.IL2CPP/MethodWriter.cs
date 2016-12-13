using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.IL2CPP.Com;
using Unity.IL2CPP.Debugger;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;
using Unity.IL2CPP.Marshaling;
using Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative;
using Unity.IL2CPP.Marshaling.BodyWriters.NativeToManaged;
using Unity.IL2CPP.Marshaling.MarshalInfoWriters;
using Unity.IL2CPP.Metadata;

namespace Unity.IL2CPP
{
	public class MethodWriter
	{
		private readonly TypeReference _type;

		private readonly CppCodeWriter _writer;

		private readonly VTableBuilder _vTableBuilder;

		[Inject]
		public static IStatsService StatsService;

		[Inject]
		public static IMetadataUsageCollectorWriterService MetadataUsageCollector;

		[Inject]
		public static IIl2CppGenericMethodCollectorWriterService Il2CppGenericMethodCollector;

		[Inject]
		public static IIl2CppTypeCollectorWriterService Il2CppTypeCollector;

		[Inject]
		public static IGenericSharingAnalysisService GenericSharingAnalysis;

		[Inject]
		public static INamingService Naming;

		[Inject]
		public static ITypeProviderService TypeProvider;

		[Inject]
		public static IIcallMappingService IcallMap;

		[Inject]
		public static IWindowsRuntimeProjections WindowsRuntimeProjections;

		public MethodWriter(TypeReference type, CppCodeWriter writer, VTableBuilder vTableBuilder)
		{
			this._type = type;
			this._writer = writer;
			this._vTableBuilder = vTableBuilder;
		}

		internal void WriteMethodDeclarationsFor(Func<MethodDefinition, bool> filter)
		{
			TypeDefinition typeDefinition = this._type.Resolve();
			foreach (MethodDefinition current in typeDefinition.Methods.Where(filter))
			{
				MethodReference method = current;
				GenericInstanceType genericInstanceType = this._type as GenericInstanceType;
				if (genericInstanceType != null)
				{
					method = VTableBuilder.CloneMethodReference(genericInstanceType, current);
				}
				this.WriteMethodDeclaration(method);
			}
			MarshalType[] marshalTypesForMarshaledType = MarshalingUtils.GetMarshalTypesForMarshaledType(typeDefinition);
			for (int i = 0; i < marshalTypesForMarshaledType.Length; i++)
			{
				MarshalType marshalType = marshalTypesForMarshaledType[i];
				DefaultMarshalInfoWriter defaultMarshalInfoWriter = MarshalDataCollector.MarshalInfoWriterFor(this._type, marshalType, null, false, false, false, null);
				defaultMarshalInfoWriter.WriteMarshalFunctionDeclarations(this._writer);
			}
			if (typeDefinition.NeedsComCallableWrapper())
			{
				new CCWWriter(typeDefinition).WriteCreateCCWDeclaration(this._writer);
			}
		}

		internal void WriteMethodDefinitions(IMethodCollector methodCollector)
		{
			TypeDefinition typeDefinition = this._type.Resolve();
			foreach (MethodDefinition current in from m in typeDefinition.Methods
			where !m.HasGenericParameters
			select m)
			{
				ErrorInformation.CurrentlyProcessing.Method = current;
				if (CodeGenOptions.EnableErrorMessageTest)
				{
					ErrorTypeAndMethod.ThrowIfIsErrorMethod(current);
				}
				MethodReference method = current;
				GenericInstanceType genericInstanceType = this._type as GenericInstanceType;
				if (genericInstanceType != null)
				{
					method = VTableBuilder.CloneMethodReference(genericInstanceType, current);
				}
				this.WriteMethodDefinition(method, methodCollector);
			}
			MarshalType[] marshalTypesForMarshaledType = MarshalingUtils.GetMarshalTypesForMarshaledType(typeDefinition);
			for (int i = 0; i < marshalTypesForMarshaledType.Length; i++)
			{
				MarshalType marshalType = marshalTypesForMarshaledType[i];
				DefaultMarshalInfoWriter defaultMarshalInfoWriter = MarshalDataCollector.MarshalInfoWriterFor(this._type, marshalType, null, false, false, false, null);
				defaultMarshalInfoWriter.WriteMarshalFunctionDefinitions(this._writer, methodCollector);
			}
			if (typeDefinition.NeedsComCallableWrapper())
			{
				methodCollector.AddCCWMarshallingFunction(typeDefinition);
				new CCWWriter(typeDefinition).WriteCreateCCWDefinition(this._writer);
			}
		}

		private static bool MethodNeedsWritten(MethodReference method)
		{
			bool result;
			if (MethodWriter.IsGetOrSetGenericValueImplOnArray(method))
			{
				result = false;
			}
			else if (GenericsUtilities.IsGenericInstanceOfCompareExchange(method))
			{
				result = false;
			}
			else if (GenericsUtilities.IsGenericInstanceOfExchange(method))
			{
				result = false;
			}
			else if (method.IsStripped())
			{
				result = false;
			}
			else
			{
				MethodDefinition method2 = method.Resolve();
				result = MethodWriter.MethodCanBeDirectlyCalled(method2);
			}
			return result;
		}

		internal void WriteMethodDeclaration(MethodReference method)
		{
			if (MethodWriter.MethodNeedsWritten(method))
			{
				this._writer.WriteCommentedLine(method.FullName);
				if (MethodWriter.GenericSharingAnalysis.CanShareMethod(method))
				{
					bool flag = MethodWriter.GenericSharingAnalysis.IsSharedMethod(method);
					MethodWriter.StatsService.ShareableMethods++;
					MethodReference sharedMethod = MethodWriter.GenericSharingAnalysis.GetSharedMethod(method);
					if (flag)
					{
						this._writer.WriteLine("{0};", new object[]
						{
							MethodSignatureWriter.GetSharedMethodSignature(this._writer, method)
						});
					}
					else if (!method.IsGenericInstance)
					{
						this._writer.AddIncludeForMethodDeclarations(sharedMethod.DeclaringType);
					}
					string text = MethodSignatureWriter.FormatParameters(method, ParameterFormat.WithName, false, true);
					this.WriteLine("#define {0}({1}) (({2}){3})({4})", new object[]
					{
						MethodWriter.Naming.ForMethodNameOnly(method),
						text,
						MethodSignatureWriter.GetMethodPointer(method),
						MethodWriter.Naming.ForMethod(sharedMethod) + "_gshared",
						text
					});
				}
				else
				{
					MethodSignatureWriter.WriteMethodSignature(this._writer, method);
					this.WriteLine(" IL2CPP_METHOD_ATTR;");
				}
				if (ReversePInvokeMethodBodyWriter.IsReversePInvokeWrapperNecessary(method))
				{
					MethodWriter.WriteReversePInvokeMethodDeclaration(this._writer, method);
				}
			}
		}

		internal void WriteMethodDefinition(MethodReference method, IMethodCollector methodCollector)
		{
			if (MethodWriter.MethodNeedsWritten(method))
			{
				MethodDefinition methodDefinition = method.Resolve();
				if (methodDefinition.IsPInvokeImpl)
				{
					MethodWriter.WriteExternMethodeDeclarationForInternalPInvokeImpl(this._writer, methodDefinition);
				}
				this._writer.WriteCommentedLine(method.FullName);
				if (method.IsGenericInstance || method.DeclaringType.IsGenericInstance)
				{
					MethodWriter.Il2CppGenericMethodCollector.Add(method);
				}
				string methodSignature;
				if (MethodWriter.GenericSharingAnalysis.CanShareMethod(method))
				{
					if (!MethodWriter.GenericSharingAnalysis.IsSharedMethod(method))
					{
						return;
					}
					methodSignature = MethodSignatureWriter.GetSharedMethodSignature(this._writer, method);
				}
				else
				{
					methodSignature = MethodSignatureWriter.GetMethodSignature(this._writer, method);
				}
				methodCollector.AddMethod(method);
				MethodWriter.StatsService.RecordMethod(method);
				TypeResolver typeResolver = new TypeResolver(method.DeclaringType as GenericInstanceType, method as GenericInstanceMethod);
				TypeReference typeReference = typeResolver.Resolve(GenericParameterResolver.ResolveReturnTypeIfNeeded(method));
				this._writer.AddIncludeForTypeDefinition(typeReference);
				foreach (ParameterDefinition current in method.Parameters)
				{
					TypeReference typeReference2 = GenericParameterResolver.ResolveParameterTypeIfNeeded(method, current);
					if (MethodWriter.ShouldWriteIncludeForParameter(typeReference2))
					{
						this._writer.AddIncludeForTypeDefinition(typeResolver.Resolve(typeReference2));
					}
				}
				MethodWriter.WriteMethodWithMetadataInitialization(this._writer, methodSignature, method.FullName, delegate(CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage)
				{
					MethodWriter.WritePrologue(method, bodyWriter, MethodWriter.GetDefaultRuntimeMetadataAccess(method, metadataUsage, methodUsage));
					this.WriteMethodBody(method, bodyWriter, MethodWriter.GetDefaultRuntimeMetadataAccess(method, metadataUsage, methodUsage));
				}, MethodWriter.Naming.ForMethod(method));
				this.WriteMethodForDelegatePInvokeIfNeeded(this._writer, method, methodCollector);
				if (method.HasThis && method.DeclaringType.IsValueType())
				{
					this.WriteAdjustorThunk(method, methodCollector);
				}
				if (ReversePInvokeMethodBodyWriter.IsReversePInvokeWrapperNecessary(method))
				{
					MethodWriter.WriteReversePInvokeMethodDefinition(this._writer, method, methodCollector);
				}
			}
		}

		private void WriteAdjustorThunk(MethodReference method, IMethodCollector methodCollector)
		{
			TypeResolver typeResolver = new TypeResolver(method.DeclaringType as GenericInstanceType, method as GenericInstanceMethod);
			string parameters = MethodSignatureWriter.FormatParameters(method, ParameterFormat.WithTypeAndNameThisObject, false, true);
			string methodSignature = MethodSignatureWriter.GetMethodSignature(MethodWriter.Naming.ForMethodAdjustorThunk(method), MethodWriter.Naming.ForVariable(typeResolver.Resolve(GenericParameterResolver.ResolveReturnTypeIfNeeded(method))), parameters, "extern \"C\"", "");
			this._writer.WriteLine(methodSignature);
			using (new BlockWriter(this._writer, false))
			{
				string item;
				if (method.DeclaringType.IsNullable())
				{
					this._writer.WriteLine("{0} _thisAdjusted;", new object[]
					{
						MethodWriter.Naming.ForVariable(method.DeclaringType)
					});
					CodeWriter arg_128_0 = this._writer;
					string arg_128_1 = "_thisAdjusted.{0}(*reinterpret_cast<{1}*>({2} + 1));";
					object[] expr_B9 = new object[3];
					expr_B9[0] = MethodWriter.Naming.ForFieldSetter(method.DeclaringType.Resolve().Fields.Single((FieldDefinition f) => f.Name == "value"));
					expr_B9[1] = MethodWriter.Naming.ForVariable(((GenericInstanceType)method.DeclaringType).GenericArguments[0]);
					expr_B9[2] = MethodWriter.Naming.ThisParameterName;
					arg_128_0.WriteLine(arg_128_1, expr_B9);
					CodeWriter arg_17D_0 = this._writer;
					string arg_17D_1 = "_thisAdjusted.{0}(true);";
					object[] expr_13E = new object[1];
					expr_13E[0] = MethodWriter.Naming.ForFieldSetter(method.DeclaringType.Resolve().Fields.Single((FieldDefinition f) => f.Name == "has_value"));
					arg_17D_0.WriteLine(arg_17D_1, expr_13E);
					item = "&_thisAdjusted";
				}
				else
				{
					this._writer.WriteLine("{0}* _thisAdjusted = reinterpret_cast<{0}*>({1} + 1);", new object[]
					{
						MethodWriter.Naming.ForVariable(method.DeclaringType),
						MethodWriter.Naming.ThisParameterName
					});
					item = "_thisAdjusted";
				}
				List<string> list = new List<string>();
				list.Add(item);
				for (int i = 0; i < method.Parameters.Count; i++)
				{
					list.Add(MethodWriter.Naming.ForParameterName(method.Parameters[i]));
				}
				list.Add("method");
				if (method.DeclaringType.IsNullable())
				{
					if (method.ReturnType.MetadataType != MetadataType.Void)
					{
						this._writer.WriteLine("{0} _returnValue = {1};", new object[]
						{
							MethodWriter.Naming.ForVariable(typeResolver.Resolve(method.ReturnType)),
							Emit.Call(MethodWriter.Naming.ForMethodNameOnly(method), list)
						});
					}
					else
					{
						this._writer.WriteStatement(Emit.Call(MethodWriter.Naming.ForMethodNameOnly(method), list));
					}
					CodeWriter arg_334_0 = this._writer;
					string arg_334_1 = "*reinterpret_cast<{1}*>({2} + 1) = _thisAdjusted.{0}();";
					object[] expr_2C5 = new object[3];
					expr_2C5[0] = MethodWriter.Naming.ForFieldGetter(method.DeclaringType.Resolve().Fields.Single((FieldDefinition f) => f.Name == "value"));
					expr_2C5[1] = MethodWriter.Naming.ForVariable(((GenericInstanceType)method.DeclaringType).GenericArguments[0]);
					expr_2C5[2] = MethodWriter.Naming.ThisParameterName;
					arg_334_0.WriteLine(arg_334_1, expr_2C5);
					if (method.ReturnType.MetadataType != MetadataType.Void)
					{
						this._writer.WriteLine("return _returnValue;");
					}
				}
				else
				{
					string text = (method.ReturnType.MetadataType == MetadataType.Void) ? string.Empty : "return ";
					this._writer.WriteLine("{0}{1};", new object[]
					{
						text,
						Emit.Call(MethodWriter.Naming.ForMethodNameOnly(method), list)
					});
				}
			}
		}

		internal static void WriteMethodWithMetadataInitialization(CppCodeWriter writer, string methodSignature, string methodFullName, Action<CppCodeWriter, MetadataUsage, MethodUsage> writeMethodBody, string uniqueIdentifier)
		{
			string identifier = uniqueIdentifier + "_MetadataUsageId";
			MetadataUsage metadataUsage = new MetadataUsage();
			MethodUsage methodUsage = new MethodUsage();
			using (CppCodeWriter cppCodeWriter = new InMemoryCodeWriter())
			{
				using (CppCodeWriter cppCodeWriter2 = new InMemoryCodeWriter())
				{
					cppCodeWriter2.Indent(1);
					cppCodeWriter.Indent(1);
					writeMethodBody(cppCodeWriter2, metadataUsage, methodUsage);
					if (metadataUsage.UsesMetadata)
					{
						MethodWriter.WriteMethodMetadataInitialization(cppCodeWriter, identifier);
					}
					cppCodeWriter2.Dedent(1);
					cppCodeWriter.Dedent(1);
					foreach (MethodReference current in methodUsage.GetMethods())
					{
						writer.AddIncludeForMethodDeclarations(current.DeclaringType);
					}
					if (metadataUsage.UsesMetadata)
					{
						MethodWriter.WriteMethodMetadataInitializationDeclarations(writer, identifier, metadataUsage.GetIl2CppTypes(), metadataUsage.GetTypeInfos(), metadataUsage.GetInflatedMethods(), metadataUsage.GetFieldInfos(), metadataUsage.GetStringLiterals());
					}
					using (new OptimizationWriter(writer, methodFullName))
					{
						writer.WriteLine(methodSignature);
						using (new BlockWriter(writer, false))
						{
							writer.Write(cppCodeWriter);
							writer.Write(cppCodeWriter2);
						}
					}
				}
			}
			if (metadataUsage.UsesMetadata)
			{
				MethodWriter.MetadataUsageCollector.Add(identifier, metadataUsage);
			}
		}

		private static bool ShouldWriteIncludeForParameter(TypeReference resolvedParameterType)
		{
			ByReferenceType byReferenceType = resolvedParameterType as ByReferenceType;
			bool result;
			if (byReferenceType != null)
			{
				result = MethodWriter.ShouldWriteIncludeForParameter(byReferenceType.ElementType);
			}
			else
			{
				PointerType pointerType = resolvedParameterType as PointerType;
				if (pointerType != null)
				{
					result = MethodWriter.ShouldWriteIncludeForParameter(pointerType.ElementType);
				}
				else
				{
					result = ((!(resolvedParameterType is TypeSpecification) || resolvedParameterType is GenericInstanceType || resolvedParameterType is ArrayType) && !resolvedParameterType.IsGenericParameter);
				}
			}
			return result;
		}

		private static void WriteMethodBodyForComOrWindowsRuntimeMethod(MethodReference method, CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
		{
			MethodDefinition methodDefinition = method.Resolve();
			if (methodDefinition.IsConstructor)
			{
				if (methodDefinition.DeclaringType.IsImport && !methodDefinition.DeclaringType.IsWindowsRuntimeProjection())
				{
					MethodWriter.WriteMethodBodyForComObjectConstructor(method, writer, methodDefinition);
				}
				else
				{
					MethodWriter.WriteMethodBodyForWindowsRuntimeObjectConstructor(method, writer, methodDefinition, metadataAccess);
				}
			}
			else if (methodDefinition.IsFinalizerMethod())
			{
				MethodWriter.WriteMethodBodyForComOrWindowsRuntimeFinalizer(methodDefinition, writer, metadataAccess);
			}
			else if (method.HasThis)
			{
				MethodWriter.WriteMethodBodyForDirectComOrWindowsRuntimeCall(method, writer, metadataAccess);
			}
			else
			{
				new ComStaticMethodBodyWriter(method).WriteMethodBody(writer, metadataAccess);
			}
		}

		private static void WriteMethodBodyForComObjectConstructor(MethodReference method, CppCodeWriter writer, MethodDefinition methodDefinition)
		{
			writer.WriteLine("il2cpp_codegen_com_create_instance({0}::CLSID, &{1}->{2});", new object[]
			{
				MethodWriter.Naming.ForTypeNameOnly(method.DeclaringType),
				MethodWriter.Naming.ThisParameterName,
				MethodWriter.Naming.ForIl2CppComObjectIdentityField()
			});
			writer.WriteLine("il2cpp_codegen_com_register_rcw({0});", new object[]
			{
				MethodWriter.Naming.ThisParameterName
			});
		}

		private static void WriteMethodBodyForWindowsRuntimeObjectConstructor(MethodReference method, CppCodeWriter writer, MethodDefinition methodDefinition, IRuntimeMetadataAccess metadataAccess)
		{
			MethodDefinition methodDefinition2 = method.Resolve();
			if (methodDefinition2.HasGenericParameters)
			{
				throw new InvalidOperationException("Cannot construct generic Windows Runtime objects.");
			}
			string arg;
			if (MethodWriter.IsUnconstructibleWindowsRuntimeClass(method.DeclaringType, out arg))
			{
				writer.WriteStatement(Emit.RaiseManagedException(string.Format("il2cpp_codegen_get_invalid_operation_exception(\"{0}\")", arg)));
			}
			else
			{
				new WindowsRuntimeConstructorMethodBodyWriter(method).WriteMethodBody(writer, metadataAccess);
			}
		}

		private static bool IsUnconstructibleWindowsRuntimeClass(TypeReference type, out string errorMessage)
		{
			bool result;
			if (type.IsAttribute())
			{
				errorMessage = string.Format("Cannot construct type '{0}'. Windows Runtime attribute types are not constructable.", type.FullName);
				result = true;
			}
			else
			{
				TypeReference typeReference = MethodWriter.WindowsRuntimeProjections.ProjectToCLR(type);
				if (typeReference != type)
				{
					errorMessage = string.Format("Cannot construct type '{0}'. It has no managed representation. Instead, use '{1}'.", type.FullName, typeReference.FullName);
					result = true;
				}
				else
				{
					errorMessage = null;
					result = false;
				}
			}
			return result;
		}

		private static void WriteMethodBodyForComOrWindowsRuntimeFinalizer(MethodDefinition finalizer, CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
		{
			TypeDefinition declaringType = finalizer.DeclaringType;
			if (!declaringType.IsIl2CppComObject())
			{
				MethodWriter.ReleaseCachedInterfaces(writer, declaringType);
			}
			else
			{
				MethodWriter.ReleaseIl2CppObjectIdentity(writer);
			}
			MethodWriter.CallBaseTypeFinalizer(finalizer, writer, metadataAccess);
		}

		private static void ReleaseCachedInterfaces(CppCodeWriter writer, TypeDefinition declaringType)
		{
			TypeReference[] array = declaringType.ImplementedComOrWindowsRuntimeInterfaces().ToArray<TypeReference>();
			if (array.Length != 0)
			{
				bool flag = declaringType.GetComposableFactoryTypes().Count<TypeReference>() > 0;
				if (flag)
				{
					writer.WriteLine(string.Format("if ({0}->klass->is_import_or_windows_runtime)", MethodWriter.Naming.ThisParameterName));
					writer.BeginBlock();
				}
				TypeReference[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					TypeReference interfaceType = array2[i];
					string arg = MethodWriter.Naming.ForComTypeInterfaceFieldName(interfaceType);
					writer.WriteLine(string.Format("if ({0}->{1} != {2})", MethodWriter.Naming.ThisParameterName, arg, MethodWriter.Naming.Null));
					using (new BlockWriter(writer, false))
					{
						writer.WriteLine(string.Format("{0}->{1}->Release();", MethodWriter.Naming.ThisParameterName, arg));
					}
				}
				if (flag)
				{
					writer.EndBlock(false);
				}
				writer.WriteLine();
				TypeReference[] array3 = array;
				for (int j = 0; j < array3.Length; j++)
				{
					TypeReference interfaceType2 = array3[j];
					string arg2 = MethodWriter.Naming.ForComTypeInterfaceFieldName(interfaceType2);
					writer.WriteLine(string.Format("{0}->{1} = {2};", MethodWriter.Naming.ThisParameterName, arg2, MethodWriter.Naming.Null));
				}
				writer.WriteLine();
			}
		}

		private static void ReleaseIl2CppObjectIdentity(CppCodeWriter writer)
		{
			string arg = MethodWriter.Naming.ForIl2CppComObjectIdentityField();
			writer.WriteLine(string.Format("if ({0}->{1} != {2})", MethodWriter.Naming.ThisParameterName, arg, MethodWriter.Naming.Null));
			using (new BlockWriter(writer, false))
			{
				writer.WriteLine(string.Format("if ({0}->klass->is_import_or_windows_runtime)", MethodWriter.Naming.ThisParameterName));
				using (new BlockWriter(writer, false))
				{
					writer.WriteLine(string.Format("il2cpp_codegen_il2cpp_com_object_cleanup({0});", MethodWriter.Naming.ThisParameterName));
				}
				writer.WriteLine(string.Format("{0}->{1}->Release();", MethodWriter.Naming.ThisParameterName, arg));
				writer.WriteLine(string.Format("{0}->{1} = {2};", MethodWriter.Naming.ThisParameterName, arg, MethodWriter.Naming.Null));
			}
			writer.WriteLine();
		}

		private static void CallBaseTypeFinalizer(MethodDefinition finalizer, CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
		{
			MethodReference methodReference = null;
			TypeDefinition typeDefinition;
			TypeResolver typeResolver;
			for (TypeReference typeReference = finalizer.DeclaringType.BaseType; typeReference != null; typeReference = typeResolver.Resolve(typeDefinition.BaseType))
			{
				typeDefinition = typeReference.Resolve();
				typeResolver = TypeResolver.For(typeReference);
				foreach (MethodDefinition current in typeDefinition.Methods)
				{
					if (current.IsFinalizerMethod())
					{
						methodReference = typeResolver.Resolve(current);
						goto IL_8F;
					}
				}
			}
			IL_8F:
			if (methodReference != null)
			{
				List<string> list = new List<string>(2);
				list.Add(MethodWriter.Naming.ThisParameterName);
				if (MethodSignatureWriter.NeedsHiddenMethodInfo(methodReference, MethodCallType.Normal, false))
				{
					list.Add(metadataAccess.HiddenMethodInfo(methodReference));
				}
				TypeResolver typeResolver2 = TypeResolver.For(finalizer.DeclaringType);
				string methodCallExpression = MethodBodyWriter.GetMethodCallExpression(finalizer, typeResolver2.Resolve(methodReference), methodReference, typeResolver2, MethodCallType.Normal, metadataAccess, new VTableBuilder(), list, false, null);
				writer.WriteStatement(methodCallExpression);
			}
		}

		private static void WriteMethodBodyForDirectComOrWindowsRuntimeCall(MethodReference method, CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
		{
			MethodDefinition methodDefinition = method.Resolve();
			if (!methodDefinition.IsComOrWindowsRuntimeMethod())
			{
				throw new InvalidOperationException("WriteMethodBodyForDirectComOrWindowsRuntimeCall called for non-COM and non-Windows Runtime method");
			}
			MethodReference methodReference = (!methodDefinition.DeclaringType.IsInterface) ? method.GetOverridenInterfaceMethod(method.DeclaringType.GetInterfaces()) : method;
			if (methodReference == null)
			{
				writer.WriteStatement(Emit.RaiseManagedException(string.Format("il2cpp_codegen_get_missing_method_exception(\"The method '{0}' has no implementation.\")", method.FullName)));
			}
			else if (!methodReference.DeclaringType.IsComOrWindowsRuntimeInterface())
			{
				writer.WriteStatement(Emit.RaiseManagedException(string.Format("il2cpp_codegen_get_not_supported_exception(\"Cannot call method '{0}' (overriding '{1}'). IL2CPP does not yet support calling projected methods.\")", method.FullName, methodReference.FullName)));
			}
			else
			{
				new ComInstanceMethodBodyWriter(method).WriteMethodBody(writer, metadataAccess);
			}
		}

		private static void WriteMethodBodyForInternalCall(MethodReference method, CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
		{
			MethodDefinition methodDefinition = method.Resolve();
			if (!methodDefinition.IsInternalCall)
			{
				throw new Exception();
			}
			if (IntrinsicRemap.ShouldRemap(methodDefinition))
			{
				string text = IntrinsicRemap.MappedNameFor(methodDefinition);
				IEnumerable<string> enumerable = MethodSignatureWriter.ParametersFor(methodDefinition, ParameterFormat.WithName, methodDefinition.IsStatic, false, false);
				enumerable = ((!IntrinsicRemap.HasCustomArguments(methodDefinition)) ? enumerable : IntrinsicRemap.GetCustomArguments(methodDefinition, methodDefinition, metadataAccess, enumerable));
				if (methodDefinition.ReturnType.MetadataType != MetadataType.Void)
				{
					writer.WriteLine("return {0}({1});", new object[]
					{
						text,
						enumerable.AggregateWithComma()
					});
				}
				else
				{
					writer.WriteLine("{0}({1});", new object[]
					{
						text,
						enumerable.AggregateWithComma()
					});
				}
			}
			else
			{
				if (methodDefinition.HasGenericParameters)
				{
					throw new NotSupportedException(string.Format("Internal calls cannot have generic parameters: {0}", methodDefinition.FullName));
				}
				string text2 = (!MethodWriter.ReturnsVoid(methodDefinition)) ? "return " : string.Empty;
				string icall = method.FullName.Substring(method.FullName.IndexOf(" ") + 1);
				string text3 = MethodWriter.IcallMap.ResolveICallFunction(icall);
				if (text3 != null)
				{
					writer.WriteLine("using namespace il2cpp::icalls;");
					writer.WriteLine("typedef {0};", new object[]
					{
						MethodSignatureWriter.GetICallMethodVariable(methodDefinition)
					});
					writer.WriteLine("{0} (({1}_ftn){2}) ({3});", new object[]
					{
						text2,
						MethodWriter.Naming.ForMethodNameOnly(method),
						text3,
						MethodSignatureWriter.FormatParameters(method, ParameterFormat.WithName, methodDefinition.IsStatic, false)
					});
				}
				else
				{
					writer.WriteInternalCallResolutionStatement(methodDefinition);
					writer.WriteLine("{0}{1}({2});", new object[]
					{
						text2,
						"_il2cpp_icall_func",
						MethodSignatureWriter.FormatParameters(method, ParameterFormat.WithName, methodDefinition.IsStatic, false)
					});
				}
			}
		}

		private static void WriteMethodBodyForPInvokeImpl(CppCodeWriter writer, MethodReference method, IRuntimeMetadataAccess metadataAccess)
		{
			new PInvokeMethodBodyWriter(method).WriteMethodBody(writer, metadataAccess);
		}

		private static void WriteExternMethodeDeclarationForInternalPInvokeImpl(CppCodeWriter writer, MethodReference method)
		{
			new PInvokeMethodBodyWriter(method).WriteExternMethodeDeclarationForInternalPInvoke(writer);
		}

		private void WriteMethodForDelegatePInvokeIfNeeded(CppCodeWriter _writer, MethodReference method, IMethodCollector methodCollector)
		{
			DelegatePInvokeMethodBodyWriter delegatePInvokeMethodBodyWriter = new DelegatePInvokeMethodBodyWriter(method);
			if (delegatePInvokeMethodBodyWriter.IsDelegatePInvokeWrapperNecessary())
			{
				TypeResolver typeResolver = TypeResolver.For(method.DeclaringType, method);
				string text = MethodWriter.Naming.ForDelegatePInvokeWrapper(method.DeclaringType);
				string methodSignature = MethodSignatureWriter.GetMethodSignature(text, MethodWriter.Naming.ForVariable(typeResolver.Resolve(method.ReturnType)), MethodSignatureWriter.FormatParameters(method, ParameterFormat.WithTypeAndName, false, true), "extern \"C\"", "");
				MethodWriter.WriteMethodWithMetadataInitialization(_writer, methodSignature, method.FullName, delegate(CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage)
				{
					IRuntimeMetadataAccess defaultRuntimeMetadataAccess = MethodWriter.GetDefaultRuntimeMetadataAccess(method, metadataUsage, methodUsage);
					delegatePInvokeMethodBodyWriter.WriteMethodBody(bodyWriter, defaultRuntimeMetadataAccess);
				}, text);
				methodCollector.AddWrapperForDelegateFromManagedToNative(method);
			}
		}

		private static void WriteReversePInvokeMethodDeclaration(CppCodeWriter writer, MethodReference method)
		{
			ReversePInvokeMethodBodyWriter.Create(method).WriteMethodDeclaration(writer);
		}

		private static void WriteReversePInvokeMethodDefinition(CppCodeWriter writer, MethodReference method, IMethodCollector methodCollector)
		{
			ReversePInvokeMethodBodyWriter.Create(method).WriteMethodDefinition(writer, methodCollector);
		}

		private static bool ReturnsVoid(MethodReference method)
		{
			return method.ReturnType.MetadataType == MetadataType.Void;
		}

		internal static bool IsGenericVirtualMethod(MethodDefinition method)
		{
			return method.IsVirtual && method.HasGenericParameters;
		}

		internal static bool MethodNeedsInvoker(MethodDefinition method)
		{
			return (!method.IsConstructor || !method.DeclaringType.IsDelegate()) && MethodWriter.MethodCanBeDirectlyCalled(method);
		}

		internal static bool MethodCanBeDirectlyCalled(MethodDefinition method)
		{
			return (!method.DeclaringType.IsInterface && !method.IsAbstract) || method.DeclaringType.IsComOrWindowsRuntimeInterface();
		}

		internal static bool IsGetOrSetGenericValueImplOnArray(MethodReference method)
		{
			return method.DeclaringType.IsSystemArray() && (method.Name == "GetGenericValueImpl" || method.Name == "SetGenericValueImpl");
		}

		private void WriteMethodBody(MethodReference method, CppCodeWriter methodBodyWriter, IRuntimeMetadataAccess metadataAccess)
		{
			MethodDefinition methodDefinition = method.Resolve();
			if (!MethodWriter.ReplaceWithHardcodedAlternativeIfPresent(method, methodBodyWriter, metadataAccess))
			{
				if (!methodDefinition.HasBody)
				{
					MethodWriter.WriteMethodBodyForMethodWithoutBody(method, methodBodyWriter, metadataAccess);
				}
				else
				{
					this.AddRetInstructionAtTheEndIfNeeded(methodDefinition);
					new MethodBodyWriter(methodBodyWriter, method, new TypeResolver(this._type as GenericInstanceType, method as GenericInstanceMethod), metadataAccess, this._vTableBuilder).Generate();
				}
			}
		}

		private static void WriteMethodMetadataInitializationDeclarations(CppCodeWriter writer, string identifier, IEnumerable<TypeReference> types, IEnumerable<TypeReference> typeInfos, IEnumerable<MethodReference> methods, IEnumerable<FieldReference> fields, IEnumerable<string> stringLiterals)
		{
			foreach (TypeReference current in types)
			{
				writer.WriteStatement("extern const Il2CppType* " + MethodWriter.Naming.ForRuntimeIl2CppType(current));
			}
			foreach (TypeReference current2 in typeInfos)
			{
				writer.WriteStatement("extern Il2CppClass* " + MethodWriter.Naming.ForRuntimeTypeInfo(current2));
			}
			foreach (MethodReference current3 in methods)
			{
				writer.WriteStatement("extern const MethodInfo* " + MethodWriter.Naming.ForRuntimeMethodInfo(current3));
			}
			foreach (FieldReference current4 in fields)
			{
				writer.WriteStatement("extern FieldInfo* " + MethodWriter.Naming.ForRuntimeFieldInfo(current4));
			}
			foreach (string current5 in stringLiterals)
			{
				writer.WriteStatement("extern Il2CppCodeGenString* " + MethodWriter.Naming.ForStringLiteralIdentifier(current5));
			}
			writer.WriteStatement("extern const uint32_t " + identifier);
		}

		private static void WritePrologue(MethodReference method, CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
		{
			if (CodeGenOptions.EnableStacktrace && !DebuggerOptions.Enabled)
			{
				writer.WriteLine("StackTraceSentry _stackTraceSentry({0});", new object[]
				{
					metadataAccess.MethodInfo(method)
				});
			}
			if (CodeGenOptions.EnableDeepProfiler)
			{
				writer.WriteLine("ProfilerMethodSentry _profilerMethodSentry({0});", new object[]
				{
					metadataAccess.MethodInfo(method)
				});
			}
		}

		private static void WriteMethodMetadataInitialization(CppCodeWriter writer, string identifier)
		{
			writer.WriteStatement("static bool s_Il2CppMethodInitialized");
			writer.WriteLine("if (!{0})", new object[]
			{
				"s_Il2CppMethodInitialized"
			});
			writer.BeginBlock();
			writer.WriteStatement(string.Format("il2cpp_codegen_initialize_method ({0})", identifier));
			writer.WriteStatement(Emit.Assign("s_Il2CppMethodInitialized", "true"));
			writer.EndBlock(false);
		}

		private static void WriteMethodBodyForMethodWithoutBody(MethodReference method, CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
		{
			MethodDefinition methodDefinition = method.Resolve();
			if (!MethodWriter.MethodCanBeDirectlyCalled(methodDefinition))
			{
				throw new InvalidOperationException(string.Format("Trying to generate a body for method '{0}'", method.FullName));
			}
			DebuggerSupportFactory.GetDebuggerSupport().WriteCallStackInformation(writer, method, new KeyValuePair<string, TypeReference>[0], metadataAccess);
			if (methodDefinition.IsRuntime && !methodDefinition.IsInternalCall && !methodDefinition.DeclaringType.IsInterface)
			{
				TypeDefinition typeDefinition = method.DeclaringType.Resolve();
				if (typeDefinition.BaseType.FullName == "System.MulticastDelegate")
				{
					new DelegateMethodsWriter(writer).WriteMethodBodyForIsRuntimeMethod(method, metadataAccess);
					return;
				}
			}
			if (methodDefinition.IsComOrWindowsRuntimeMethod())
			{
				MethodWriter.WriteMethodBodyForComOrWindowsRuntimeMethod(method, writer, metadataAccess);
			}
			else if (methodDefinition.IsInternalCall)
			{
				MethodWriter.WriteMethodBodyForInternalCall(method, writer, metadataAccess);
			}
			else
			{
				if (!methodDefinition.IsPInvokeImpl)
				{
					throw new NotSupportedException();
				}
				MethodWriter.WriteMethodBodyForPInvokeImpl(writer, methodDefinition, metadataAccess);
			}
		}

		public static IRuntimeMetadataAccess GetDefaultRuntimeMetadataAccess(MethodReference method, MetadataUsage metadataUsage, MethodUsage methodUsage)
		{
			DefaultRuntimeMetadataAccess defaultRuntimeMetadataAccess = new DefaultRuntimeMetadataAccess(method, metadataUsage, methodUsage);
			return (method == null || !MethodWriter.GenericSharingAnalysis.IsSharedMethod(method)) ? defaultRuntimeMetadataAccess : new SharedRuntimeMetadataAccess(method, defaultRuntimeMetadataAccess);
		}

		private static bool ReplaceWithHardcodedAlternativeIfPresent(MethodReference method, CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
		{
			MethodDefinition methodDefinition = method.Resolve();
			string fullName = methodDefinition.FullName;
			bool result;
			if (fullName == "System.Int32 System.Double::GetHashCode()")
			{
				TypeDefinition type = methodDefinition.Module.GetType("System.Int64");
				MethodDefinition method2 = type.Methods.Single((MethodDefinition m) => m.Name == "GetHashCode");
				writer.AddIncludeForMethodDeclarations(type);
				writer.WriteLine("return {0};", new object[]
				{
					Emit.Call(MethodWriter.Naming.ForMethod(method2), Emit.Cast(new ByReferenceType(type), MethodWriter.Naming.ThisParameterName), metadataAccess.HiddenMethodInfo(method2))
				});
				result = true;
			}
			else if (fullName == "R System.Array::UnsafeMov(S)")
			{
				TypeReference variableType = TypeResolver.For(method.DeclaringType, method).Resolve(method.ReturnType);
				writer.WriteLine("return static_cast<{0}>({1});", new object[]
				{
					MethodWriter.Naming.ForVariable(variableType),
					MethodWriter.Naming.ForParameterName(method.Parameters.First<ParameterDefinition>())
				});
				result = true;
			}
			else
			{
				result = (fullName == "System.Void Microsoft.FSharp.Core.CompilerMessageAttribute::.ctor(System.Object,System.Object)");
			}
			return result;
		}

		private void AddRetInstructionAtTheEndIfNeeded(MethodDefinition method)
		{
			if (method.HasBody)
			{
				if (method.Body.HasExceptionHandlers)
				{
					if (!(method.Body.Instructions[method.Body.Instructions.Count - 1].OpCode == OpCodes.Ret))
					{
						ExceptionHandler exceptionHandler = method.Body.ExceptionHandlers[method.Body.ExceptionHandlers.Count - 1];
						if (exceptionHandler.HandlerEnd == null)
						{
							if (method.ReturnType.MetadataType != MetadataType.Void)
							{
								this.InjectEmptyVariableToTheStack(method.ReturnType, method.Body);
							}
							Instruction instruction = method.Body.Instructions[method.Body.Instructions.Count - 1];
							Instruction instruction2 = Instruction.Create(OpCodes.Ret);
							instruction2.Offset = instruction.Offset + instruction.GetSize();
							exceptionHandler.HandlerEnd = instruction2;
							method.Body.Instructions.Add(instruction2);
						}
					}
				}
			}
		}

		private void InjectEmptyVariableToTheStack(TypeReference type, MethodBody body)
		{
			Instruction instruction;
			if (!type.IsValueType())
			{
				instruction = Instruction.Create(OpCodes.Ldnull);
			}
			else if (type.IsPrimitive && type.MetadataType != MetadataType.UIntPtr && type.MetadataType != MetadataType.IntPtr)
			{
				switch (type.MetadataType)
				{
				case MetadataType.Boolean:
				case MetadataType.Char:
				case MetadataType.SByte:
				case MetadataType.Byte:
				case MetadataType.Int16:
				case MetadataType.UInt16:
				case MetadataType.Int32:
				case MetadataType.UInt32:
				case MetadataType.Int64:
				case MetadataType.UInt64:
					instruction = Instruction.Create(OpCodes.Ldc_I4_0);
					break;
				case MetadataType.Single:
					instruction = Instruction.Create(OpCodes.Ldc_R4, 0f);
					break;
				case MetadataType.Double:
					instruction = Instruction.Create(OpCodes.Ldc_R8, 0.0);
					break;
				default:
					throw new Exception();
				}
			}
			else
			{
				VariableDefinition variableDefinition = new VariableDefinition(type);
				body.Variables.Add(variableDefinition);
				instruction = Instruction.Create(OpCodes.Ldloc, variableDefinition);
			}
			body.Instructions.Add(instruction);
			instruction.Offset = instruction.Previous.Offset + instruction.Previous.GetSize();
		}

		private void WriteLine(string line)
		{
			this._writer.WriteLine(line);
		}

		private void WriteLine(string format, params object[] args)
		{
			this._writer.WriteLine(format, args);
		}
	}
}
