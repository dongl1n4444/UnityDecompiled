using Mono.Cecil;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.Metadata;

namespace Unity.IL2CPP.Marshaling.BodyWriters.NativeToManaged
{
	internal class ReversePInvokeMethodBodyWriter : NativeToManagedInteropMethodBodyWriter
	{
		private ReversePInvokeMethodBodyWriter(MethodReference managedMethod, MethodReference interopMethod, bool useUnicodeCharset) : base(managedMethod, interopMethod, MarshalType.PInvoke, useUnicodeCharset)
		{
		}

		public static ReversePInvokeMethodBodyWriter Create(MethodReference managedMethod)
		{
			MethodReference interopMethod = ReversePInvokeMethodBodyWriter.GetInteropMethod(managedMethod);
			bool useUnicodeCharset = MarshalingUtils.UseUnicodeAsDefaultMarshalingForStringParameters(interopMethod);
			return new ReversePInvokeMethodBodyWriter(managedMethod, interopMethod, useUnicodeCharset);
		}

		public static bool IsReversePInvokeWrapperNecessary(MethodReference method)
		{
			bool result;
			if (method.HasThis)
			{
				result = false;
			}
			else
			{
				MethodDefinition methodDefinition = method.Resolve();
				result = (!methodDefinition.HasGenericParameters && !methodDefinition.ReturnType.IsGenericParameter && !methodDefinition.DeclaringType.HasGenericParameters && (ReversePInvokeMethodBodyWriter.GetPInvokeCallbackAttribute(methodDefinition) != null || method.FullName == "System.Int32 System.IO.Compression.DeflateStream::UnmanagedWrite(System.IntPtr,System.Int32,System.IntPtr)" || method.FullName == "System.Int32 System.IO.Compression.DeflateStream::UnmanagedRead(System.IntPtr,System.Int32,System.IntPtr)"));
			}
			return result;
		}

		public void WriteMethodDeclaration(CppCodeWriter writer)
		{
			MarshaledParameter[] parameters = this._parameters;
			for (int i = 0; i < parameters.Length; i++)
			{
				MarshaledParameter parameter = parameters[i];
				base.MarshalInfoWriterFor(parameter).WriteIncludesForFieldDeclaration(writer);
			}
			base.MarshalInfoWriterFor(this.GetMethodReturnType()).WriteIncludesForFieldDeclaration(writer);
			writer.WriteStatement(this.GetMethodSignature());
		}

		public void WriteMethodDefinition(CppCodeWriter writer, IMethodCollector methodCollector)
		{
			MethodWriter.WriteMethodWithMetadataInitialization(writer, this.GetMethodSignature(), this._managedMethod.FullName, delegate(CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage)
			{
				base.WriteMethodBody(bodyWriter, MethodWriter.GetDefaultRuntimeMetadataAccess(this._managedMethod, metadataUsage, methodUsage));
			}, InteropMethodBodyWriter.Naming.ForReversePInvokeWrapperMethod(this._managedMethod));
			methodCollector.AddReversePInvokeWrapper(this._managedMethod);
		}

		private string GetMethodSignature()
		{
			string text = InteropMethodBodyWriter.Naming.ForReversePInvokeWrapperMethod(this._managedMethod);
			string decoratedName = this._marshaledReturnType.DecoratedName;
			string callingConvention = this.GetCallingConvention();
			string text2 = (from parameterType in this._marshaledParameterTypes
			select string.Format("{0} {1}", parameterType.DecoratedName, parameterType.VariableName)).AggregateWithComma();
			return string.Format("extern \"C\" {0} {1} {2}({3})", new object[]
			{
				decoratedName,
				callingConvention,
				text,
				text2
			});
		}

		private static MethodReference GetInteropMethod(MethodReference method)
		{
			MethodDefinition methodDef = method.Resolve();
			CustomAttribute pInvokeCallbackAttribute = ReversePInvokeMethodBodyWriter.GetPInvokeCallbackAttribute(methodDef);
			MethodReference result;
			if (pInvokeCallbackAttribute == null || !pInvokeCallbackAttribute.HasConstructorArguments)
			{
				result = method;
			}
			else
			{
				TypeReference typeReference = (from argument in pInvokeCallbackAttribute.ConstructorArguments
				where argument.Type.Name == "Type"
				select argument.Value).FirstOrDefault<object>() as TypeReference;
				if (typeReference == null)
				{
					result = method;
				}
				else
				{
					TypeDefinition typeDefinition = typeReference.Resolve();
					if (typeDefinition == null || !typeDefinition.IsDelegate())
					{
						result = method;
					}
					else
					{
						MethodDefinition methodDefinition = typeDefinition.Methods.SingleOrDefault((MethodDefinition m) => m.Name == "Invoke");
						if (methodDefinition == null)
						{
							result = method;
						}
						else
						{
							TypeResolver typeResolver = TypeResolver.For(TypeResolver.For(method.DeclaringType, method).Resolve(typeDefinition));
							MethodReference candidate = typeResolver.Resolve(methodDefinition);
							if (!VirtualMethodResolution.MethodSignaturesMatchIgnoreStaticness(candidate, method))
							{
								result = method;
							}
							else
							{
								result = methodDefinition;
							}
						}
					}
				}
			}
			return result;
		}

		private static CustomAttribute GetPInvokeCallbackAttribute(MethodDefinition methodDef)
		{
			return methodDef.CustomAttributes.FirstOrDefault((CustomAttribute attribute) => attribute.AttributeType.FullName.Contains("MonoPInvokeCallback"));
		}

		protected override void WriteInteropCallStatement(CppCodeWriter writer, string[] localVariableNames, IRuntimeMetadataAccess metadataAccess)
		{
			string methodCallExpression = base.GetMethodCallExpression(metadataAccess, InteropMethodBodyWriter.Naming.Null, localVariableNames);
			MethodReturnType methodReturnType = this.GetMethodReturnType();
			if (methodReturnType.ReturnType.MetadataType != MetadataType.Void)
			{
				string text = InteropMethodBodyWriter.Naming.ForVariable(this._typeResolver.Resolve(methodReturnType.ReturnType));
				writer.WriteLine("{0} {1} = {2};", new object[]
				{
					text,
					InteropMethodBodyWriter.Naming.ForInteropReturnValue(),
					methodCallExpression
				});
			}
			else
			{
				writer.WriteStatement(methodCallExpression);
			}
		}

		internal string GetCallingConvention()
		{
			CustomAttribute pInvokeCallbackAttribute = ReversePInvokeMethodBodyWriter.GetPInvokeCallbackAttribute(this._managedMethod.Resolve());
			string result;
			if (pInvokeCallbackAttribute == null || !pInvokeCallbackAttribute.HasConstructorArguments)
			{
				result = "DEFAULT_CALL";
			}
			else
			{
				TypeReference typeReference = (from argument in pInvokeCallbackAttribute.ConstructorArguments
				where argument.Type.Name == "Type"
				select argument.Value).FirstOrDefault<object>() as TypeReference;
				if (typeReference == null)
				{
					result = "DEFAULT_CALL";
				}
				else
				{
					TypeDefinition typeDefinition = typeReference.Resolve();
					if (typeDefinition == null)
					{
						result = "DEFAULT_CALL";
					}
					else
					{
						CustomAttribute customAttribute = typeDefinition.CustomAttributes.FirstOrDefault((CustomAttribute attribute) => attribute.AttributeType.FullName == "System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute");
						if (customAttribute == null || !customAttribute.HasConstructorArguments)
						{
							result = "DEFAULT_CALL";
						}
						else if (!(customAttribute.ConstructorArguments[0].Value is int))
						{
							result = "DEFAULT_CALL";
						}
						else
						{
							CallingConvention callingConvention = (CallingConvention)((int)customAttribute.ConstructorArguments[0].Value);
							if (callingConvention != CallingConvention.Cdecl)
							{
								if (callingConvention != CallingConvention.StdCall)
								{
									result = "DEFAULT_CALL";
								}
								else
								{
									result = "STDCALL";
								}
							}
							else
							{
								result = "CDECL";
							}
						}
					}
				}
			}
			return result;
		}

		protected override void WriteReturnStatementEpilogue(CppCodeWriter writer, string unmarshaledReturnValueVariableName)
		{
			if (this.GetMethodReturnType().ReturnType.MetadataType != MetadataType.Void)
			{
				writer.WriteLine("return {0};", new object[]
				{
					unmarshaledReturnValueVariableName
				});
			}
		}
	}
}
