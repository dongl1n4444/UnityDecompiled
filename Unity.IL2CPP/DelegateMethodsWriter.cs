using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP
{
	public class DelegateMethodsWriter
	{
		private readonly CppCodeWriter _writer;

		private readonly string _methodPtrGetterName;

		private readonly string _methodPtrSetterName;

		private readonly string _methodGetterName;

		private readonly string _methodSetterName;

		private readonly string _targetGetterName;

		private readonly string _targetSetterName;

		private readonly string _valueGetterName;

		private readonly string _prevGetterName;

		[Inject]
		public static INamingService Naming;

		[Inject]
		public static ITypeProviderService TypeProvider;

		public DelegateMethodsWriter(CppCodeWriter writer)
		{
			this._writer = writer;
			FieldDefinition field = DelegateMethodsWriter.TypeProvider.SystemDelegate.Fields.Single((FieldDefinition f) => f.Name == "method_ptr");
			this._methodPtrGetterName = DelegateMethodsWriter.Naming.ForFieldGetter(field);
			this._methodPtrSetterName = DelegateMethodsWriter.Naming.ForFieldSetter(field);
			FieldDefinition field2 = DelegateMethodsWriter.TypeProvider.SystemDelegate.Fields.Single((FieldDefinition f) => f.Name == "method");
			this._methodGetterName = DelegateMethodsWriter.Naming.ForFieldGetter(field2);
			this._methodSetterName = DelegateMethodsWriter.Naming.ForFieldSetter(field2);
			FieldDefinition field3 = DelegateMethodsWriter.TypeProvider.SystemDelegate.Fields.Single((FieldDefinition f) => f.Name == "m_target");
			this._targetGetterName = DelegateMethodsWriter.Naming.ForFieldGetter(field3);
			this._targetSetterName = DelegateMethodsWriter.Naming.ForFieldSetter(field3);
			FieldDefinition field4 = DelegateMethodsWriter.TypeProvider.SystemIntPtr.Fields.Single((FieldDefinition f) => f.Name == "m_value");
			this._valueGetterName = DelegateMethodsWriter.Naming.ForFieldGetter(field4);
			string expectedName = (CodeGenOptions.Dotnetprofile != DotNetProfile.Net45) ? "prev" : "delegates";
			FieldDefinition field5 = DelegateMethodsWriter.TypeProvider.SystemMulticastDelegate.Fields.Single((FieldDefinition f) => f.Name == expectedName);
			this._prevGetterName = DelegateMethodsWriter.Naming.ForFieldGetter(field5);
		}

		public void WriteMethodBodyForIsRuntimeMethod(MethodReference method, IRuntimeMetadataAccess metadataAccess)
		{
			TypeDefinition typeDefinition = method.DeclaringType.Resolve();
			if (typeDefinition.BaseType.FullName != "System.MulticastDelegate")
			{
				throw new NotSupportedException("Cannot WriteMethodBodyForIsRuntimeMethod for non multicase delegate type: " + typeDefinition.FullName);
			}
			string name = method.Name;
			if (name != null)
			{
				if (name == "Invoke")
				{
					this.WriteMethodBodyForInvoke(method);
					return;
				}
				if (name == "BeginInvoke")
				{
					this.WriteMethodBodyForBeginInvoke(method, metadataAccess);
					return;
				}
				if (name == "EndInvoke")
				{
					this.WriteMethodBodyForDelegateEndInvoke(method);
					return;
				}
				if (name == ".ctor")
				{
					this.WriteMethodBodyForDelegateConstructor(method);
					return;
				}
			}
			this._writer.WriteDefaultReturn(DelegateMethodsWriter.TypeResolverFor(method).Resolve(GenericParameterResolver.ResolveReturnTypeIfNeeded(method)));
		}

		private static TypeResolver TypeResolverFor(MethodReference method)
		{
			return new TypeResolver(method.DeclaringType as GenericInstanceType, method as GenericInstanceMethod);
		}

		private void WriteMethodBodyForDelegateConstructor(MethodReference method)
		{
			string text = DelegateMethodsWriter.Naming.ForParameterName(method.Parameters[0]);
			string text2 = DelegateMethodsWriter.Naming.ForParameterName(method.Parameters[1]);
			this.WriteLine("{0}((Il2CppMethodPointer)((MethodInfo*){1}.{2}())->methodPointer);", new object[]
			{
				DelegateMethodsWriter.ExpressionForFieldOfThis(this._methodPtrSetterName),
				text2,
				this._valueGetterName
			});
			this.WriteLine("{0}({1});", new object[]
			{
				DelegateMethodsWriter.ExpressionForFieldOfThis(this._methodSetterName),
				text2
			});
			this.WriteLine("{0}({1});", new object[]
			{
				DelegateMethodsWriter.ExpressionForFieldOfThis(this._targetSetterName),
				text
			});
		}

		private void WriteMethodBodyForInvoke(MethodReference method)
		{
			List<string> parametersOnlyName = MethodSignatureWriter.ParametersFor(method, ParameterFormat.WithNameNoThis, false, false, false).ToList<string>();
			if (CodeGenOptions.Dotnetprofile == DotNetProfile.Net45)
			{
				this.WriteInvocationsForDelegate45(method, parametersOnlyName);
			}
			else
			{
				this.WriteInvokeChainedDelegates(method, parametersOnlyName);
				this.WriteInvocationsForDelegate(DelegateMethodsWriter.Naming.ThisParameterName, method, parametersOnlyName, null);
			}
		}

		private void WriteInvocationsForDelegate45(MethodReference method, List<string> parametersOnlyName)
		{
			string text = string.Format("(MethodInfo*)({0}().{1}())", DelegateMethodsWriter.ExpressionForFieldOfThis(this._methodGetterName), this._valueGetterName);
			this.WriteLine("il2cpp_codegen_raise_execution_engine_exception_if_method_is_not_found({0});", new object[]
			{
				text
			});
			string text2 = "length";
			string text3 = "result";
			string text4 = "delegatesToInvoke";
			string text5 = "currentDelegate";
			if (method.ReturnType.MetadataType != MetadataType.Void)
			{
				TypeReference type = DelegateMethodsWriter.TypeResolverFor(method).ResolveReturnType(method);
				this._writer.WriteVariable(type, text3);
			}
			string text6 = DelegateMethodsWriter.ExpressionForFieldOfThis(this._prevGetterName);
			FieldDefinition fieldDefinition = DelegateMethodsWriter.TypeProvider.SystemMulticastDelegate.Fields.Single((FieldDefinition f) => f.Name == "delegates");
			string text7 = DelegateMethodsWriter.Naming.ForVariable(fieldDefinition.FieldType);
			this._writer.AddIncludeForTypeDefinition(fieldDefinition.FieldType);
			this._writer.WriteLine("{0} {1} = {2}();", new object[]
			{
				text7,
				text4,
				text6
			});
			this._writer.WriteLine("if ({0} != NULL)", new object[]
			{
				text4
			});
			this._writer.BeginBlock();
			this._writer.WriteLine("uint32_t {0} = {1}->max_length;", new object[]
			{
				text2,
				text4
			});
			this._writer.WriteLine("for (uint32_t i = 0; i < {0}; i++)", new object[]
			{
				text2
			});
			this._writer.BeginBlock();
			this._writer.WriteLine("{2}* {0} = {1};", new object[]
			{
				text5,
				Emit.LoadArrayElement(text4, "i", false),
				DelegateMethodsWriter.Naming.ForType(((ArrayType)fieldDefinition.FieldType).ElementType)
			});
			this.WriteInvocationsForDelegate(text5, method, parametersOnlyName, text3);
			this._writer.EndBlock(false);
			if (method.ReturnType.MetadataType != MetadataType.Void)
			{
				this._writer.WriteLine("return {0};", new object[]
				{
					text3
				});
			}
			this._writer.EndBlock(false);
			this._writer.WriteLine("else");
			this._writer.BeginBlock();
			this.WriteInvocationsForDelegate(DelegateMethodsWriter.Naming.ThisParameterName, method, parametersOnlyName, null);
			this._writer.EndBlock(false);
		}

		private void WriteInvocationsForDelegate(string delegateVariableName, MethodReference method, List<string> parametersOnlyName, string resultVariableName = null)
		{
			string text = string.Format("(MethodInfo*)({0}().{1}())", DelegateMethodsWriter.ExpressionForFieldOf(delegateVariableName, this._methodGetterName), this._valueGetterName);
			bool flag = DelegateMethodsWriter.ShouldEmitNotBoundInstanceInvocation(method);
			this.WriteLine("il2cpp_codegen_raise_execution_engine_exception_if_method_is_not_found({0});", new object[]
			{
				text
			});
			this.WriteLine("bool {0} = MethodIsStatic({1});", new object[]
			{
				"___methodIsStatic",
				text
			});
			if (parametersOnlyName.Count != 0)
			{
				this.WriteLine("if ({0}() != NULL && {1})", new object[]
				{
					DelegateMethodsWriter.ExpressionForFieldOf(delegateVariableName, this._targetGetterName),
					"___methodIsStatic"
				});
			}
			else
			{
				this.WriteLine("if (({0}() != NULL || MethodHasParameters({1})) && {2})", new object[]
				{
					DelegateMethodsWriter.ExpressionForFieldOf(delegateVariableName, this._targetGetterName),
					text,
					"___methodIsStatic"
				});
			}
			this._writer.BeginBlock();
			this.EmitInvocation(delegateVariableName, method, this._methodPtrGetterName + "()", this._targetGetterName + "()", parametersOnlyName, text, false, true, resultVariableName);
			this._writer.EndBlock(false);
			if (flag)
			{
				this.WriteLine("else if ({0}() != NULL || {1})", new object[]
				{
					DelegateMethodsWriter.ExpressionForFieldOf(delegateVariableName, this._targetGetterName),
					"___methodIsStatic"
				});
			}
			else
			{
				this.WriteLine("else", new object[0]);
			}
			this._writer.BeginBlock();
			string text2 = this._methodPtrGetterName + "()";
			string targetFieldName = this._targetGetterName + "()";
			string text3 = text;
			this.EmitInvocation(delegateVariableName, method, text2, targetFieldName, parametersOnlyName, text3, false, false, resultVariableName);
			if (flag)
			{
				this._writer.EndBlock(false);
				this.WriteLine("else", new object[0]);
				this._writer.BeginBlock();
				text3 = this._methodPtrGetterName + "()";
				targetFieldName = this._targetGetterName + "()";
				text2 = text;
				bool useFirstArgumentAsThis = true;
				this.EmitInvocation(delegateVariableName, method, text3, targetFieldName, parametersOnlyName, text2, useFirstArgumentAsThis, false, resultVariableName);
			}
			this._writer.EndBlock(false);
		}

		private void WriteInvokeChainedDelegates(MethodReference method, List<string> parametersOnlyName)
		{
			string text = DelegateMethodsWriter.CommaSeperate(parametersOnlyName, true);
			string text2 = DelegateMethodsWriter.ExpressionForFieldOfThis(this._prevGetterName) + "()";
			this.WriteLine("if({0} != NULL)", new object[]
			{
				text2
			});
			this._writer.BeginBlock();
			this.WriteLine("{0}(({1}){2}{3}, method);", new object[]
			{
				DelegateMethodsWriter.Naming.ForMethod(method),
				DelegateMethodsWriter.Naming.ForVariable(method.DeclaringType),
				text2,
				text
			});
			this._writer.EndBlock(false);
		}

		private static bool ShouldEmitNotBoundInstanceInvocation(MethodReference method)
		{
			bool result;
			if (!method.Parameters.Any<ParameterDefinition>())
			{
				result = false;
			}
			else
			{
				TypeReference typeReference = DelegateMethodsWriter.TypeResolverFor(method).ResolveParameterType(method, method.Parameters[0]);
				result = (!typeReference.IsValueType() && !typeReference.IsPointer && !typeReference.IsByReference);
			}
			return result;
		}

		private void EmitInvocation(string delegateVariableName, MethodReference method, string methodPtrFieldName, string targetFieldName, List<string> parametersOnlyName, string methodInfoExpression, bool useFirstArgumentAsThis = false, bool forStatic = false, string resultVariableName = null)
		{
			List<string> list = MethodSignatureWriter.ParametersFor(method, ParameterFormat.WithTypeAndNameThisObject, false, true, true).ToList<string>();
			if (useFirstArgumentAsThis)
			{
				list.RemoveAt(1);
			}
			if (forStatic)
			{
				list.Insert(0, DelegateMethodsWriter.Naming.ForVariable(DelegateMethodsWriter.TypeProvider.SystemObject));
			}
			this.WriteLine("typedef {0} {1} ({2});", new object[]
			{
				DelegateMethodsWriter.Naming.ForVariable(DelegateMethodsWriter.TypeResolverFor(method).ResolveReturnType(method)),
				string.Format("(*{0})", "FunctionPointerType"),
				DelegateMethodsWriter.CommaSeperate(list, false)
			});
			string text = (resultVariableName != null) ? ((method.ReturnType.MetadataType == MetadataType.Void) ? string.Empty : string.Format("{0} = ", resultVariableName)) : ((method.ReturnType.MetadataType == MetadataType.Void) ? string.Empty : "return ");
			string text2 = string.Format("(({0}){1})", "FunctionPointerType", DelegateMethodsWriter.ExpressionForFieldOf(delegateVariableName, methodPtrFieldName));
			string text3 = string.Format("{0}->{1}()", delegateVariableName, this._targetGetterName);
			this.WriteLine("{0}{1}({2}{3}{4},{5});", new object[]
			{
				text,
				text2,
				(!forStatic) ? string.Empty : string.Format("{0},", DelegateMethodsWriter.Naming.Null),
				(!useFirstArgumentAsThis) ? text3 : string.Empty,
				DelegateMethodsWriter.CommaSeperate(parametersOnlyName, !useFirstArgumentAsThis),
				methodInfoExpression
			});
		}

		private static string CommaSeperate(IEnumerable<string> strings, bool alsoStartWithComma = false)
		{
			string result;
			if (!strings.Any<string>())
			{
				result = string.Empty;
			}
			else
			{
				string text = strings.AggregateWithComma();
				result = ((!alsoStartWithComma) ? text : ("," + text));
			}
			return result;
		}

		private static string ExpressionForFieldOfThis(string targetFieldName)
		{
			return DelegateMethodsWriter.ExpressionForFieldOf(DelegateMethodsWriter.Naming.ThisParameterName, targetFieldName);
		}

		private static string ExpressionForFieldOf(string variableName, string targetFieldName)
		{
			return string.Format("{0}->{1}", variableName, targetFieldName);
		}

		private void WriteMethodBodyForBeginInvoke(MethodReference method, IRuntimeMetadataAccess metadataAccess)
		{
			this.WriteLine("void *__d_args[{0}] = {{0}};", new object[]
			{
				method.Parameters.Count - 1
			});
			TypeResolver typeResolver = DelegateMethodsWriter.TypeResolverFor(method);
			if (DelegateMethodsWriter.BeginInvokeHasAdditionalParameters(method))
			{
				for (int i = 0; i < method.Parameters.Count - 2; i++)
				{
					ParameterDefinition parameterDefinition = method.Parameters[i];
					TypeReference typeReference = typeResolver.ResolveParameterType(method, parameterDefinition);
					string text = DelegateMethodsWriter.Naming.ForParameterName(parameterDefinition);
					if (typeReference.IsByReference)
					{
						TypeReference elementType = ((ByReferenceType)typeReference).ElementType;
						text = ((!elementType.IsValueType()) ? Emit.Dereference(text) : Emit.Box(elementType, Emit.Dereference(text), metadataAccess));
					}
					else if (typeReference.IsValueType())
					{
						text = Emit.Box(typeReference, text, metadataAccess);
					}
					this.WriteLine("__d_args[{0}] = {1};", new object[]
					{
						i,
						text
					});
				}
			}
			this.WriteLine("return ({0})il2cpp_codegen_delegate_begin_invoke((Il2CppDelegate*)__this, __d_args, (Il2CppDelegate*){1}, (Il2CppObject*){2});", new object[]
			{
				DelegateMethodsWriter.Naming.ForVariable(typeResolver.ResolveReturnType(method)),
				DelegateMethodsWriter.Naming.ForParameterName(method.Parameters[method.Parameters.Count - 2]),
				DelegateMethodsWriter.Naming.ForParameterName(method.Parameters[method.Parameters.Count - 1])
			});
		}

		private static bool BeginInvokeHasAdditionalParameters(MethodReference method)
		{
			return method.Parameters.Count > 2;
		}

		private void WriteMethodBodyForDelegateEndInvoke(MethodReference method)
		{
			ParameterDefinition parameterReference = method.Parameters[method.Parameters.Count - 1];
			string text = "0";
			List<string> list = DelegateMethodsWriter.CollectOutArgsIfAny(method);
			if (list.Count > 0)
			{
				this.WriteLine("void* ___out_args[] = {", new object[0]);
				foreach (string current in list)
				{
					this.WriteLine("{0},", new object[]
					{
						current
					});
				}
				this.WriteLine("};", new object[0]);
				text = "___out_args";
			}
			if (method.ReturnType.MetadataType == MetadataType.Void)
			{
				this.WriteLine("il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) {0}, {1});", new object[]
				{
					DelegateMethodsWriter.Naming.ForParameterName(parameterReference),
					text
				});
			}
			else
			{
				this.WriteLine("Il2CppObject *__result = il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) {0}, {1});", new object[]
				{
					DelegateMethodsWriter.Naming.ForParameterName(parameterReference),
					text
				});
				TypeReference typeReference = DelegateMethodsWriter.TypeResolverFor(method).ResolveReturnType(method);
				if (!typeReference.IsValueType())
				{
					this.WriteLine("return ({0})__result;", new object[]
					{
						DelegateMethodsWriter.Naming.ForVariable(typeReference)
					});
				}
				else
				{
					this.WriteLine("return *{0};", new object[]
					{
						Emit.Cast(new PointerType(typeReference), "UnBox ((Il2CppCodeGenObject*)__result)")
					});
				}
			}
		}

		private static List<string> CollectOutArgsIfAny(MethodReference method)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < method.Parameters.Count - 1; i++)
			{
				if (method.Parameters[i].ParameterType.IsByReference)
				{
					list.Add(DelegateMethodsWriter.Naming.ForParameterName(method.Parameters[i]));
				}
			}
			return list;
		}

		private void WriteLine(string format, params object[] args)
		{
			this._writer.WriteLine(format, args);
		}
	}
}
