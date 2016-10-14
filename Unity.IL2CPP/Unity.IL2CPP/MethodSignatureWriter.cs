using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP
{
	public class MethodSignatureWriter
	{
		[Inject]
		public static INamingService Naming;

		[Inject]
		public static ITypeProviderService TypeProvider;

		public static string GetICallMethodVariable(MethodDefinition method)
		{
			return string.Format("{0} (*{1}_ftn) ({2})", MethodSignatureWriter.Naming.ForVariable(method.ReturnType), MethodSignatureWriter.Naming.ForMethodNameOnly(method), MethodSignatureWriter.FormatParameters(method, ParameterFormat.WithType, method.IsStatic, false));
		}

		public static string GetMethodPointerForVTable(MethodReference method)
		{
			ParameterFormat parameterFormat = (!method.DeclaringType.IsValueType || !method.HasThis) ? ParameterFormat.WithType : ParameterFormat.WithTypeThisObject;
			return MethodSignatureWriter.GetMethodPointer(method, parameterFormat);
		}

		public static string GetMethodPointer(MethodReference method)
		{
			return MethodSignatureWriter.GetMethodPointer(method, ParameterFormat.WithType);
		}

		public static string GetMethodPointer(MethodReference method, ParameterFormat parameterFormat)
		{
			TypeResolver typeResolver = new TypeResolver(method.DeclaringType as GenericInstanceType, method as GenericInstanceMethod);
			return MethodSignatureWriter.GetMethodSignature("(*)", MethodSignatureWriter.Naming.ForVariable(typeResolver.ResolveReturnType(method)), MethodSignatureWriter.FormatParameters(method, parameterFormat, false, true), string.Empty, "");
		}

		internal static string GetMethodSignature(CppCodeWriter writer, MethodReference method)
		{
			TypeResolver typeResolver = new TypeResolver(method.DeclaringType as GenericInstanceType, method as GenericInstanceMethod);
			MethodSignatureWriter.RecordIncludes(writer, method, typeResolver);
			string attributes = MethodSignatureWriter.BuildMethodAttributes(method);
			return MethodSignatureWriter.GetMethodSignature(MethodSignatureWriter.Naming.ForMethodNameOnly(method), MethodSignatureWriter.Naming.ForVariable(typeResolver.Resolve(GenericParameterResolver.ResolveReturnTypeIfNeeded(method))), MethodSignatureWriter.FormatParameters(method, ParameterFormat.WithTypeAndName, false, true), "extern \"C\"", attributes);
		}

		public static string GetSharedMethodSignature(CppCodeWriter writer, MethodReference method)
		{
			TypeResolver typeResolver = new TypeResolver(method.DeclaringType as GenericInstanceType, method as GenericInstanceMethod);
			TypeReference variableType = typeResolver.Resolve(GenericParameterResolver.ResolveReturnTypeIfNeeded(method));
			MethodSignatureWriter.RecordIncludes(writer, method, typeResolver);
			string attributes = MethodSignatureWriter.BuildMethodAttributes(method);
			return MethodSignatureWriter.GetMethodSignature(MethodSignatureWriter.Naming.ForMethodNameOnly(method) + "_gshared", MethodSignatureWriter.Naming.ForVariable(variableType), MethodSignatureWriter.FormatParameters(method, ParameterFormat.WithTypeAndName, false, true), "extern \"C\"", attributes);
		}

		public static string GetSharedMethodSignatureRaw(MethodReference method)
		{
			TypeResolver typeResolver = new TypeResolver(method.DeclaringType as GenericInstanceType, method as GenericInstanceMethod);
			TypeReference variableType = typeResolver.Resolve(GenericParameterResolver.ResolveReturnTypeIfNeeded(method));
			string attributes = MethodSignatureWriter.BuildMethodAttributes(method);
			return MethodSignatureWriter.GetMethodSignature(MethodSignatureWriter.Naming.ForMethodNameOnly(method) + "_gshared", MethodSignatureWriter.Naming.ForVariable(variableType), MethodSignatureWriter.FormatParameters(method, ParameterFormat.WithTypeAndName, false, true), "extern \"C\"", attributes);
		}

		public static void WriteMethodSignature(CppCodeWriter writer, MethodReference method)
		{
			writer.Write(MethodSignatureWriter.GetMethodSignature(writer, method));
		}

		public static string FormatParameters(MethodReference method, ParameterFormat format = ParameterFormat.WithTypeAndName, bool forceNoStaticThis = false, bool includeHiddenMethodInfo = false)
		{
			List<string> list = MethodSignatureWriter.ParametersFor(method, format, forceNoStaticThis, includeHiddenMethodInfo, false).ToList<string>();
			return (list.Count != 0) ? list.AggregateWithComma() : string.Empty;
		}

		[DebuggerHidden]
		public static IEnumerable<string> ParametersFor(MethodReference methodDefinition, ParameterFormat format = ParameterFormat.WithTypeAndName, bool forceNoStaticThis = false, bool includeHiddenMethodInfo = false, bool useVoidPointerForThis = false)
		{
			MethodSignatureWriter.<ParametersFor>c__Iterator0 <ParametersFor>c__Iterator = new MethodSignatureWriter.<ParametersFor>c__Iterator0();
			<ParametersFor>c__Iterator.methodDefinition = methodDefinition;
			<ParametersFor>c__Iterator.forceNoStaticThis = forceNoStaticThis;
			<ParametersFor>c__Iterator.format = format;
			<ParametersFor>c__Iterator.useVoidPointerForThis = useVoidPointerForThis;
			<ParametersFor>c__Iterator.includeHiddenMethodInfo = includeHiddenMethodInfo;
			MethodSignatureWriter.<ParametersFor>c__Iterator0 expr_2B = <ParametersFor>c__Iterator;
			expr_2B.$PC = -2;
			return expr_2B;
		}

		private static string FormatHiddenMethodArgument(ParameterFormat format)
		{
			switch (format)
			{
			case ParameterFormat.WithTypeAndName:
			case ParameterFormat.WithTypeAndNameNoThis:
			case ParameterFormat.WithTypeAndNameThisObject:
			{
				string result = "const MethodInfo* method";
				return result;
			}
			case ParameterFormat.WithType:
			case ParameterFormat.WithTypeNoThis:
			case ParameterFormat.WithTypeThisObject:
			{
				string result = "const MethodInfo*";
				return result;
			}
			case ParameterFormat.WithName:
			case ParameterFormat.WithNameCastThis:
			case ParameterFormat.WithNameUnboxThis:
			{
				string result = "method";
				return result;
			}
			}
			throw new ArgumentOutOfRangeException("format");
		}

		private static string ParameterStringFor(MethodReference methodDefinition, ParameterFormat format, ParameterDefinition parameterDefinition)
		{
			TypeResolver typeResolver = TypeResolver.For(methodDefinition.DeclaringType);
			return MethodSignatureWriter.FormatParameterName(typeResolver.Resolve(GenericParameterResolver.ResolveParameterTypeIfNeeded(methodDefinition, parameterDefinition)), MethodSignatureWriter.Naming.ForParameterName(parameterDefinition), format);
		}

		private static string BuildMethodAttributes(MethodReference method)
		{
			string result = string.Empty;
			if (method.Resolve().NoInlining)
			{
				result = "IL2CPP_NO_INLINE";
			}
			return result;
		}

		internal static string GetMethodSignature(string name, string returnType, string parameters, string specifiers = "", string attributes = "")
		{
			return string.Format("{0} {1} {2} {3} ({4})", new object[]
			{
				specifiers,
				attributes,
				returnType,
				name,
				parameters
			});
		}

		private static void RecordIncludes(CppCodeWriter writer, MethodReference method, TypeResolver typeResolver)
		{
			if (method.HasThis)
			{
				writer.AddIncludesForTypeReference((!method.DeclaringType.IsComOrWindowsRuntimeInterface()) ? method.DeclaringType : MethodSignatureWriter.TypeProvider.SystemObject, false);
			}
			if (method.ReturnType.MetadataType != MetadataType.Void)
			{
				writer.AddIncludesForTypeReference(typeResolver.ResolveReturnType(method), false);
			}
			foreach (ParameterDefinition current in method.Parameters)
			{
				writer.AddIncludesForTypeReference(typeResolver.ResolveParameterType(method, current), true);
			}
		}

		private static bool NeedsUnusedThisParameterForStaticMethod(MethodReference methodDefinition)
		{
			return methodDefinition.Resolve().IsStatic;
		}

		private static string FormatThis(ParameterFormat format, TypeReference thisType)
		{
			string result;
			if (format == ParameterFormat.WithNameCastThis)
			{
				result = string.Format("({0}){1}", MethodSignatureWriter.Naming.ForVariable(thisType), MethodSignatureWriter.Naming.ThisParameterName);
			}
			else if (format == ParameterFormat.WithNameUnboxThis)
			{
				result = string.Format("({0})UnBox({1})", MethodSignatureWriter.Naming.ForVariable(thisType), MethodSignatureWriter.Naming.ThisParameterName);
			}
			else
			{
				result = MethodSignatureWriter.FormatParameterName(thisType, MethodSignatureWriter.Naming.ThisParameterName, format);
			}
			return result;
		}

		private static string FormatParameterAsVoidPointer(string parameterName)
		{
			return "void* " + parameterName;
		}

		private static string FormatParameterName(TypeReference parameterType, string parameterName, ParameterFormat format)
		{
			string text = string.Empty;
			if (format == ParameterFormat.WithTypeAndName || format == ParameterFormat.WithTypeAndNameNoThis || format == ParameterFormat.WithType || format == ParameterFormat.WithTypeNoThis || format == ParameterFormat.WithTypeAndNameThisObject || format == ParameterFormat.WithTypeThisObject)
			{
				text += MethodSignatureWriter.Naming.ForVariable(parameterType);
			}
			if (format == ParameterFormat.WithTypeAndName || format == ParameterFormat.WithTypeAndNameNoThis || format == ParameterFormat.WithTypeAndNameThisObject)
			{
				text += " ";
			}
			if (format == ParameterFormat.WithTypeAndName || format == ParameterFormat.WithTypeAndNameNoThis || format == ParameterFormat.WithName || format == ParameterFormat.WithTypeAndNameThisObject || format == ParameterFormat.WithNameNoThis || format == ParameterFormat.WithNameCastThis || format == ParameterFormat.WithNameUnboxThis)
			{
				text += parameterName;
			}
			return text;
		}

		public static bool CanDevirtualizeMethodCall(MethodDefinition method)
		{
			return !method.IsVirtual || method.DeclaringType.IsSealed || method.IsFinal;
		}

		public static bool NeedsHiddenMethodInfo(MethodReference method, MethodCallType callType, bool isConstructor)
		{
			return !IntrinsicRemap.ShouldRemap(method) && (!method.DeclaringType.IsArray || (!isConstructor && !(method.Name == "Set") && !(method.Name == "Get") && !(method.Name == "Address"))) && (!method.DeclaringType.IsSystemArray() || (!(method.Name == "GetGenericValueImpl") && !(method.Name == "SetGenericValueImpl"))) && !GenericsUtilities.IsGenericInstanceOfCompareExchange(method) && !GenericsUtilities.IsGenericInstanceOfExchange(method) && (callType != MethodCallType.Virtual || MethodSignatureWriter.CanDevirtualizeMethodCall(method.Resolve()));
		}
	}
}
