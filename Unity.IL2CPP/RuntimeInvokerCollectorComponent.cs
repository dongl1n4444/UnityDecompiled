using Mono.Cecil;
using NiceIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;
using Unity.IL2CPP.Metadata;
using Unity.IL2CPP.Portability;

namespace Unity.IL2CPP
{
	public class RuntimeInvokerCollectorComponent : IRuntimeInvokerCollectorAdderService, IRuntimeInvokerCollectorWriterService, IRuntimeInvokerCollectorReaderService, IDisposable
	{
		private readonly Dictionary<TypeReference[], int> _runtimeInvokerData = new Dictionary<TypeReference[], int>(new RuntimeInvokerComparer());

		[Inject]
		public static IGenericSharingAnalysisService GenericSharingAnalysis;

		[Inject]
		public static INamingService Naming;

		public string Add(MethodReference method)
		{
			if (RuntimeInvokerCollectorComponent.GenericSharingAnalysis.CanShareMethod(method))
			{
				method = RuntimeInvokerCollectorComponent.GenericSharingAnalysis.GetSharedMethod(method);
			}
			TypeResolver typeResolver = new TypeResolver(method.DeclaringType as GenericInstanceType, method as GenericInstanceMethod);
			TypeReference[] array = new TypeReference[method.Parameters.Count + 1];
			array[0] = RuntimeInvokerCollectorComponent.InvokerReturnTypeFor(typeResolver.ResolveReturnType(method));
			for (int i = 0; i < method.Parameters.Count; i++)
			{
				array[i + 1] = RuntimeInvokerCollectorComponent.InvokerParameterTypeFor(typeResolver.ResolveParameterType(method, method.Parameters[i]));
			}
			if (!this._runtimeInvokerData.ContainsKey(array))
			{
				this._runtimeInvokerData.Add(array, this._runtimeInvokerData.Count);
			}
			return RuntimeInvokerCollectorComponent.NameForInvoker(array);
		}

		public int GetIndex(MethodReference method)
		{
			int result;
			if (method.HasGenericParameters || method.DeclaringType.HasGenericParameters)
			{
				result = -1;
			}
			else
			{
				if (RuntimeInvokerCollectorComponent.GenericSharingAnalysis.CanShareMethod(method))
				{
					method = RuntimeInvokerCollectorComponent.GenericSharingAnalysis.GetSharedMethod(method);
				}
				TypeResolver typeResolver = new TypeResolver(method.DeclaringType as GenericInstanceType, method as GenericInstanceMethod);
				TypeReference[] array = new TypeReference[method.Parameters.Count + 1];
				array[0] = RuntimeInvokerCollectorComponent.InvokerReturnTypeFor(typeResolver.ResolveReturnType(method));
				for (int i = 0; i < method.Parameters.Count; i++)
				{
					array[i + 1] = RuntimeInvokerCollectorComponent.InvokerParameterTypeFor(typeResolver.ResolveParameterType(method, method.Parameters[i]));
				}
				int num;
				if (!this._runtimeInvokerData.TryGetValue(array, out num))
				{
					result = -1;
				}
				else
				{
					result = num;
				}
			}
			return result;
		}

		public ReadOnlyCollection<string> GetInvokers()
		{
			return (from invoker in this._runtimeInvokerData
			select RuntimeInvokerCollectorComponent.NameForInvoker(invoker.Key)).ToArray<string>().AsReadOnlyPortable<string>();
		}

		private static string NameForInvoker(TypeReference[] data)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("RuntimeInvoker_");
			stringBuilder.Append(RuntimeInvokerCollectorComponent.Naming.ForType(data[0]));
			for (int i = 1; i < data.Length; i++)
			{
				stringBuilder.Append("_");
				stringBuilder.Append(RuntimeInvokerCollectorComponent.Naming.ForType(data[i]));
			}
			return stringBuilder.ToString();
		}

		private static TypeReference InvokerReturnTypeFor(TypeReference type)
		{
			TypeReference result;
			if (type.IsByReference)
			{
				result = type;
			}
			else if (!type.IsValueType())
			{
				result = type.Module.TypeSystem.Object;
			}
			else
			{
				result = type;
			}
			return result;
		}

		private static TypeReference InvokerParameterTypeFor(TypeReference type)
		{
			TypeReference result;
			if (type.IsByReference)
			{
				result = type;
			}
			else if (!type.IsValueType())
			{
				result = type.Module.TypeSystem.Object;
			}
			else if (type.MetadataType == MetadataType.Byte)
			{
				result = type.Module.TypeSystem.SByte;
			}
			else if (type.MetadataType == MetadataType.UInt16)
			{
				result = type.Module.TypeSystem.Int16;
			}
			else if (type.MetadataType == MetadataType.UInt32)
			{
				result = type.Module.TypeSystem.Int32;
			}
			else if (type.MetadataType == MetadataType.UInt64)
			{
				result = type.Module.TypeSystem.Int64;
			}
			else if (type.MetadataType == MetadataType.Boolean)
			{
				result = type.Module.TypeSystem.SByte;
			}
			else if (type.MetadataType == MetadataType.Char)
			{
				result = type.Module.TypeSystem.Int16;
			}
			else if (type.MetadataType == MetadataType.UIntPtr)
			{
				result = type.Module.TypeSystem.IntPtr;
			}
			else if (type.IsEnum())
			{
				result = type.GetUnderlyingEnumType();
			}
			else
			{
				result = type;
			}
			return result;
		}

		private static string LoadParameter(TypeReference type, string param)
		{
			string result;
			if (type.IsByReference)
			{
				result = Emit.Cast(type, param);
			}
			else if (type.MetadataType == MetadataType.SByte || type.MetadataType == MetadataType.Byte || type.MetadataType == MetadataType.Boolean || type.MetadataType == MetadataType.Int16 || type.MetadataType == MetadataType.UInt16 || type.MetadataType == MetadataType.Char || type.MetadataType == MetadataType.Int32 || type.MetadataType == MetadataType.UInt32 || type.MetadataType == MetadataType.Int64 || type.MetadataType == MetadataType.UInt64 || type.MetadataType == MetadataType.IntPtr || type.MetadataType == MetadataType.UIntPtr || type.MetadataType == MetadataType.Single || type.MetadataType == MetadataType.Double)
			{
				result = string.Concat(new string[]
				{
					"*((",
					RuntimeInvokerCollectorComponent.Naming.ForVariable(new PointerType(type)),
					")",
					param,
					")"
				});
			}
			else if ((type.MetadataType == MetadataType.String || type.MetadataType == MetadataType.Class || type.MetadataType == MetadataType.Array || type.MetadataType == MetadataType.Pointer || type.MetadataType == MetadataType.Object) && !type.IsValueType())
			{
				result = Emit.Cast(type, param);
			}
			else if (type.MetadataType == MetadataType.GenericInstance && !type.IsValueType())
			{
				result = Emit.Cast(type, param);
			}
			else
			{
				if (!type.IsValueType())
				{
					throw new Exception();
				}
				if (type.IsEnum())
				{
					result = RuntimeInvokerCollectorComponent.LoadParameter(type.GetUnderlyingEnumType(), param);
				}
				else
				{
					result = string.Concat(new string[]
					{
						"*((",
						RuntimeInvokerCollectorComponent.Naming.ForVariable(new PointerType(type)),
						")",
						param,
						")"
					});
				}
			}
			return result;
		}

		public TableInfo Write(NPath path)
		{
			TableInfo result;
			using (SourceCodeWriter sourceCodeWriter = new SourceCodeWriter(path))
			{
				sourceCodeWriter.AddCodeGenIncludes();
				KeyValuePair<TypeReference[], int>[] array = (from item in this._runtimeInvokerData
				orderby item.Value
				select item).ToArray<KeyValuePair<TypeReference[], int>>();
				KeyValuePair<TypeReference[], int>[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					KeyValuePair<TypeReference[], int> keyValuePair = array2[i];
					TypeReference[] key = keyValuePair.Key;
					TypeReference typeReference = key[0];
					sourceCodeWriter.AddIncludeOrExternForTypeDefinition(typeReference.Module.TypeSystem.Object);
					sourceCodeWriter.AddIncludeOrExternForTypeDefinition(typeReference);
					for (int j = 1; j < key.Length; j++)
					{
						sourceCodeWriter.AddIncludeOrExternForTypeDefinition(key[j]);
					}
					sourceCodeWriter.WriteLine("void* {0} (const MethodInfo* method, void* obj, void** args)", new object[]
					{
						RuntimeInvokerCollectorComponent.NameForInvoker(key)
					});
					sourceCodeWriter.BeginBlock();
					RuntimeInvokerCollectorComponent.WriteInvokerBody(sourceCodeWriter, key, typeReference);
					sourceCodeWriter.EndBlock(false);
					sourceCodeWriter.WriteLine();
				}
				result = MetadataWriter.WriteTable<KeyValuePair<TypeReference[], int>>(sourceCodeWriter, "extern const InvokerMethod", "g_Il2CppInvokerPointers", array, (KeyValuePair<TypeReference[], int> kvp) => RuntimeInvokerCollectorComponent.NameForInvoker(kvp.Key));
			}
			return result;
		}

		private static void WriteInvokerBody(CppCodeWriter writer, TypeReference[] data, TypeReference returnType)
		{
			writer.Write("typedef ");
			writer.Write(RuntimeInvokerCollectorComponent.Naming.ForVariable(returnType));
			writer.Write(" (*Func)(void* obj");
			for (int i = 1; i < data.Length; i++)
			{
				writer.Write(", ");
				writer.Write(RuntimeInvokerCollectorComponent.Naming.ForVariable(data[i]));
				writer.Write(" p");
				writer.Write(i.ToString());
			}
			writer.WriteLine(", const MethodInfo* method);");
			if (returnType.MetadataType != MetadataType.Void)
			{
				writer.Write(RuntimeInvokerCollectorComponent.Naming.ForVariable(returnType));
				writer.Write(" ret = ");
			}
			writer.Write("((Func)method->methodPointer)(obj");
			for (int j = 1; j < data.Length; j++)
			{
				writer.Write(", {0}", new object[]
				{
					RuntimeInvokerCollectorComponent.LoadParameter(data[j], "args[" + (j - 1) + "]")
				});
			}
			writer.WriteLine(", method);");
			writer.Write("return ");
			if (returnType.MetadataType == MetadataType.Void)
			{
				writer.Write(RuntimeInvokerCollectorComponent.Naming.Null);
			}
			else
			{
				writer.Write((!returnType.IsValueType()) ? "ret" : "Box(il2cpp_codegen_class_from_type (method->return_type), &ret)");
			}
			writer.WriteLine(";");
		}

		public void Dispose()
		{
			this._runtimeInvokerData.Clear();
		}
	}
}
