using Mono.Cecil;
using System;
using System.Globalization;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.GenericSharing;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP
{
	public sealed class SharedRuntimeMetadataAccess : IRuntimeMetadataAccess
	{
		private readonly MethodReference _methodReference;

		private readonly TypeResolver _typeResolver;

		private readonly DefaultRuntimeMetadataAccess _default;

		[Inject]
		public static IGenericSharingAnalysisService GenericSharingAnalysis;

		[Inject]
		public static INamingService Naming;

		public SharedRuntimeMetadataAccess(MethodReference methodReference, DefaultRuntimeMetadataAccess defaultRuntimeMetadataAccess)
		{
			this._methodReference = methodReference;
			this._typeResolver = new TypeResolver(methodReference.DeclaringType as GenericInstanceType, methodReference as GenericInstanceMethod);
			this._default = defaultRuntimeMetadataAccess;
		}

		public string StaticData(TypeReference type)
		{
			return this.RetreiveType(type, () => this._default.StaticData(type), "IL2CPP_RGCTX_DATA", "IL2CPP_RGCTX_DATA", RuntimeGenericContextInfo.Static);
		}

		public string TypeInfoFor(TypeReference type)
		{
			return this.RetreiveType(type, () => this._default.TypeInfoFor(type), "IL2CPP_RGCTX_DATA", "IL2CPP_RGCTX_DATA", RuntimeGenericContextInfo.Class);
		}

		public string SizeOf(TypeReference type)
		{
			return this.RetreiveType(type, () => this._default.SizeOf(type), "IL2CPP_RGCTX_SIZEOF", "IL2CPP_RGCTX_SIZEOF", RuntimeGenericContextInfo.Class);
		}

		public string Il2CppTypeFor(TypeReference type)
		{
			return this.RetreiveType(type, () => this._default.Il2CppTypeFor(type), "IL2CPP_RGCTX_TYPE", "IL2CPP_RGCTX_TYPE", RuntimeGenericContextInfo.Type);
		}

		public string ArrayInfo(TypeReference elementType)
		{
			return this.RetreiveType(elementType, () => this._default.ArrayInfo(elementType), "IL2CPP_RGCTX_DATA", "IL2CPP_RGCTX_DATA", RuntimeGenericContextInfo.Array);
		}

		public string Newobj(MethodReference ctor)
		{
			return this.RetreiveType(ctor.DeclaringType, () => this._default.Newobj(ctor), "IL2CPP_RGCTX_DATA", "IL2CPP_RGCTX_DATA", RuntimeGenericContextInfo.Class);
		}

		private string GetTypeRgctxDataExpression()
		{
			string text = "method->declaring_type";
			if (!this._methodReference.HasThis || this._methodReference.DeclaringType.IsValueType())
			{
				text = SharedRuntimeMetadataAccess.Naming.ForInitializedTypeInfo(text);
			}
			return string.Format("{0}->rgctx_data", text);
		}

		public string Method(MethodReference method)
		{
			MethodReference methodReference = this._typeResolver.Resolve(method);
			return this.RetreiveMethod<string>(method, () => this._default.Method(method), (int index) => "(" + Emit.Cast(MethodSignatureWriter.GetMethodPointerForVTable(methodReference), Emit.Call("IL2CPP_RGCTX_METHOD_INFO", this.GetTypeRgctxDataExpression(), index.ToString(CultureInfo.InvariantCulture)) + "->methodPointer") + ")", (int index) => "(" + Emit.Cast(MethodSignatureWriter.GetMethodPointerForVTable(methodReference), Emit.Call("IL2CPP_RGCTX_METHOD_INFO", "method->rgctx_data", index.ToString(CultureInfo.InvariantCulture)) + "->methodPointer") + ")", RuntimeGenericContextInfo.Method);
		}

		public bool NeedsBoxingForValueTypeThis(MethodReference method)
		{
			return this.RetreiveMethod<bool>(method, () => false, (int index) => true, (int index) => true, RuntimeGenericContextInfo.Method);
		}

		public string FieldInfo(FieldReference field)
		{
			string result;
			if (SharedRuntimeMetadataAccess.GetRGCTXAccess(field.DeclaringType, this._methodReference) == RuntimeGenericAccess.None)
			{
				result = this._default.FieldInfo(field);
			}
			else
			{
				string arg = this.TypeInfoFor(field.DeclaringType);
				result = string.Format("IL2CPP_RGCTX_FIELD_INFO({0},{1})", arg, SharedRuntimeMetadataAccess.Naming.GetFieldIndex(field, false));
			}
			return result;
		}

		public string MethodInfo(MethodReference method)
		{
			return this.RetreiveMethod<string>(method, () => this._default.MethodInfo(method), (int index) => Emit.Call("IL2CPP_RGCTX_METHOD_INFO", this.GetTypeRgctxDataExpression(), index.ToString(CultureInfo.InvariantCulture)), (int index) => Emit.Call("IL2CPP_RGCTX_METHOD_INFO", "method->rgctx_data", index.ToString(CultureInfo.InvariantCulture)), RuntimeGenericContextInfo.Method);
		}

		public string HiddenMethodInfo(MethodReference method)
		{
			return this.RetreiveMethod<string>(method, () => this._default.HiddenMethodInfo(method), (int index) => Emit.Call("IL2CPP_RGCTX_METHOD_INFO", this.GetTypeRgctxDataExpression(), index.ToString(CultureInfo.InvariantCulture)), (int index) => Emit.Call("IL2CPP_RGCTX_METHOD_INFO", "method->rgctx_data", index.ToString(CultureInfo.InvariantCulture)), RuntimeGenericContextInfo.Method);
		}

		public string StringLiteral(string literal)
		{
			return this._default.StringLiteral(literal);
		}

		private T RetreiveMethod<T>(MethodReference method, Func<T> defaultFunc, Func<int, T> retrieveTypeSharedAccess, Func<int, T> retrieveMethodSharedAccess, RuntimeGenericContextInfo info)
		{
			RuntimeGenericAccess rGCTXAccess = SharedRuntimeMetadataAccess.GetRGCTXAccess(method, this._methodReference);
			T result;
			if (rGCTXAccess == RuntimeGenericAccess.None)
			{
				result = defaultFunc();
			}
			else if (rGCTXAccess == RuntimeGenericAccess.Method)
			{
				GenericSharingData rgctx = SharedRuntimeMetadataAccess.GenericSharingAnalysis.RuntimeGenericContextFor(this._methodReference.Resolve());
				int num = SharedRuntimeMetadataAccess.RetrieveMethodIndex(method, info, rgctx);
				if (num == -1)
				{
					throw new InvalidOperationException(SharedRuntimeMetadataAccess.FormatGenericContextErrorMessage(method.FullName));
				}
				result = retrieveMethodSharedAccess(num);
			}
			else
			{
				if (rGCTXAccess != RuntimeGenericAccess.This && rGCTXAccess != RuntimeGenericAccess.Type)
				{
					throw new ArgumentOutOfRangeException("method");
				}
				GenericSharingData rgctx2 = SharedRuntimeMetadataAccess.GenericSharingAnalysis.RuntimeGenericContextFor(this._methodReference.DeclaringType.Resolve());
				int num2 = SharedRuntimeMetadataAccess.RetrieveMethodIndex(method, info, rgctx2);
				if (num2 == -1)
				{
					throw new InvalidOperationException(SharedRuntimeMetadataAccess.FormatGenericContextErrorMessage(method.FullName));
				}
				result = retrieveTypeSharedAccess(num2);
			}
			return result;
		}

		private string RetreiveType(TypeReference type, Func<string> defaultFunc, string typeSharedAccessName, string methodSharedAccessName, RuntimeGenericContextInfo info)
		{
			RuntimeGenericAccess rGCTXAccess = SharedRuntimeMetadataAccess.GetRGCTXAccess(type, this._methodReference);
			string result;
			if (rGCTXAccess == RuntimeGenericAccess.None)
			{
				result = defaultFunc();
			}
			else if (rGCTXAccess == RuntimeGenericAccess.Method)
			{
				GenericSharingData rgctx = SharedRuntimeMetadataAccess.GenericSharingAnalysis.RuntimeGenericContextFor(this._methodReference.Resolve());
				int num = SharedRuntimeMetadataAccess.RetrieveTypeIndex(type, info, rgctx);
				if (num == -1)
				{
					throw new InvalidOperationException(SharedRuntimeMetadataAccess.FormatGenericContextErrorMessage(type.FullName));
				}
				result = Emit.Call(methodSharedAccessName, "method->rgctx_data", num.ToString(CultureInfo.InvariantCulture));
			}
			else
			{
				if (rGCTXAccess != RuntimeGenericAccess.This && rGCTXAccess != RuntimeGenericAccess.Type)
				{
					throw new ArgumentOutOfRangeException("type");
				}
				GenericSharingData rgctx2 = SharedRuntimeMetadataAccess.GenericSharingAnalysis.RuntimeGenericContextFor(this._methodReference.DeclaringType.Resolve());
				int num2 = SharedRuntimeMetadataAccess.RetrieveTypeIndex(type, info, rgctx2);
				if (num2 == -1)
				{
					throw new InvalidOperationException(SharedRuntimeMetadataAccess.FormatGenericContextErrorMessage(type.FullName));
				}
				result = Emit.Call(typeSharedAccessName, this.GetTypeRgctxDataExpression(), num2.ToString(CultureInfo.InvariantCulture));
			}
			return result;
		}

		public static RuntimeGenericAccess GetRGCTXAccess(TypeReference type, MethodReference enclosingMethod)
		{
			RuntimeGenericAccess result;
			switch (GenericSharingVisitor.GenericUsageFor(type))
			{
			case GenericContextUsage.None:
				result = RuntimeGenericAccess.None;
				break;
			case GenericContextUsage.Type:
				if (SharedRuntimeMetadataAccess.GenericSharingAnalysis.NeedsTypeContextAsArgument(enclosingMethod))
				{
					result = RuntimeGenericAccess.Type;
				}
				else
				{
					result = RuntimeGenericAccess.This;
				}
				break;
			case GenericContextUsage.Method:
			case GenericContextUsage.Both:
				result = RuntimeGenericAccess.Method;
				break;
			default:
				throw new ArgumentOutOfRangeException("type");
			}
			return result;
		}

		public static RuntimeGenericAccess GetRGCTXAccess(MethodReference method, MethodReference enclosingMethod)
		{
			RuntimeGenericAccess result;
			switch (GenericSharingVisitor.GenericUsageFor(method))
			{
			case GenericContextUsage.None:
				result = RuntimeGenericAccess.None;
				break;
			case GenericContextUsage.Type:
				if (SharedRuntimeMetadataAccess.GenericSharingAnalysis.NeedsTypeContextAsArgument(enclosingMethod))
				{
					result = RuntimeGenericAccess.Type;
				}
				else
				{
					result = RuntimeGenericAccess.This;
				}
				break;
			case GenericContextUsage.Method:
			case GenericContextUsage.Both:
				result = RuntimeGenericAccess.Method;
				break;
			default:
				throw new ArgumentOutOfRangeException("method");
			}
			return result;
		}

		public static int RetrieveTypeIndex(TypeReference type, RuntimeGenericContextInfo info, GenericSharingData rgctx)
		{
			int result = -1;
			for (int i = 0; i < rgctx.RuntimeGenericDatas.Count; i++)
			{
				RuntimeGenericData runtimeGenericData = rgctx.RuntimeGenericDatas[i];
				if (runtimeGenericData.InfoType == info)
				{
					RuntimeGenericTypeData runtimeGenericTypeData = (RuntimeGenericTypeData)runtimeGenericData;
					if (runtimeGenericTypeData.GenericType != null && TypeReferenceEqualityComparer.AreEqual(runtimeGenericTypeData.GenericType, type, TypeComparisonMode.Exact))
					{
						result = i;
						break;
					}
				}
			}
			return result;
		}

		public static int RetrieveMethodIndex(MethodReference method, RuntimeGenericContextInfo info, GenericSharingData rgctx)
		{
			int result = -1;
			for (int i = 0; i < rgctx.RuntimeGenericDatas.Count; i++)
			{
				RuntimeGenericData runtimeGenericData = rgctx.RuntimeGenericDatas[i];
				if (runtimeGenericData.InfoType == info)
				{
					RuntimeGenericMethodData runtimeGenericMethodData = (RuntimeGenericMethodData)runtimeGenericData;
					if (runtimeGenericMethodData.GenericMethod != null && new MethodReferenceComparer().Equals(runtimeGenericMethodData.GenericMethod, method))
					{
						result = i;
						break;
					}
				}
			}
			return result;
		}

		private static string FormatGenericContextErrorMessage(string name)
		{
			return string.Format("Unable to retrieve the runtime generic context for '{0}'.", name);
		}
	}
}
