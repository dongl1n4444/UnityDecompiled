namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using NiceIO;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Metadata;
    using Unity.IL2CPP.Portability;

    public class RuntimeInvokerCollectorComponent : IRuntimeInvokerCollectorAdderService, IRuntimeInvokerCollectorWriterService, IRuntimeInvokerCollectorReaderService, IDisposable
    {
        private readonly Dictionary<TypeReference[], int> _runtimeInvokerData = new Dictionary<TypeReference[], int>(new RuntimeInvokerComparer());
        [CompilerGenerated]
        private static Func<KeyValuePair<TypeReference[], int>, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<KeyValuePair<TypeReference[], int>, int> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<KeyValuePair<TypeReference[], int>, string> <>f__am$cache2;
        [Inject]
        public static IGenericSharingAnalysisService GenericSharingAnalysis;
        [Inject]
        public static INamingService Naming;

        public string Add(MethodReference method)
        {
            if (GenericSharingAnalysis.CanShareMethod(method))
            {
                method = GenericSharingAnalysis.GetSharedMethod(method);
            }
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = new Unity.IL2CPP.ILPreProcessor.TypeResolver(method.DeclaringType as GenericInstanceType, method as GenericInstanceMethod);
            TypeReference[] key = new TypeReference[method.Parameters.Count + 1];
            key[0] = InvokerReturnTypeFor(resolver.ResolveReturnType(method));
            for (int i = 0; i < method.Parameters.Count; i++)
            {
                key[i + 1] = InvokerParameterTypeFor(resolver.ResolveParameterType(method, method.Parameters[i]));
            }
            if (!this._runtimeInvokerData.ContainsKey(key))
            {
                this._runtimeInvokerData.Add(key, this._runtimeInvokerData.Count);
            }
            return NameForInvoker(key);
        }

        public void Dispose()
        {
            this._runtimeInvokerData.Clear();
        }

        public int GetIndex(MethodReference method)
        {
            int num3;
            if (method.HasGenericParameters || method.DeclaringType.HasGenericParameters)
            {
                return -1;
            }
            if (GenericSharingAnalysis.CanShareMethod(method))
            {
                method = GenericSharingAnalysis.GetSharedMethod(method);
            }
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = new Unity.IL2CPP.ILPreProcessor.TypeResolver(method.DeclaringType as GenericInstanceType, method as GenericInstanceMethod);
            TypeReference[] key = new TypeReference[method.Parameters.Count + 1];
            key[0] = InvokerReturnTypeFor(resolver.ResolveReturnType(method));
            for (int i = 0; i < method.Parameters.Count; i++)
            {
                key[i + 1] = InvokerParameterTypeFor(resolver.ResolveParameterType(method, method.Parameters[i]));
            }
            if (!this._runtimeInvokerData.TryGetValue(key, out num3))
            {
                return -1;
            }
            return num3;
        }

        public ReadOnlyCollection<string> GetInvokers()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<KeyValuePair<TypeReference[], int>, string>(null, (IntPtr) <GetInvokers>m__0);
            }
            return this._runtimeInvokerData.Select<KeyValuePair<TypeReference[], int>, string>(<>f__am$cache0).ToArray<string>().AsReadOnlyPortable<string>();
        }

        private static TypeReference InvokerParameterTypeFor(TypeReference type)
        {
            if (!type.IsByReference)
            {
                if (!type.IsValueType())
                {
                    return type.Module.TypeSystem.Object;
                }
                if (type.MetadataType == MetadataType.Byte)
                {
                    return type.Module.TypeSystem.SByte;
                }
                if (type.MetadataType == MetadataType.UInt16)
                {
                    return type.Module.TypeSystem.Int16;
                }
                if (type.MetadataType == MetadataType.UInt32)
                {
                    return type.Module.TypeSystem.Int32;
                }
                if (type.MetadataType == MetadataType.UInt64)
                {
                    return type.Module.TypeSystem.Int64;
                }
                if (type.MetadataType == MetadataType.Boolean)
                {
                    return type.Module.TypeSystem.SByte;
                }
                if (type.MetadataType == MetadataType.Char)
                {
                    return type.Module.TypeSystem.Int16;
                }
                if (type.MetadataType == MetadataType.UIntPtr)
                {
                    return type.Module.TypeSystem.IntPtr;
                }
                if (type.IsEnum())
                {
                    return type.GetUnderlyingEnumType();
                }
            }
            return type;
        }

        private static TypeReference InvokerReturnTypeFor(TypeReference type)
        {
            if (!type.IsByReference && !type.IsValueType())
            {
                return type.Module.TypeSystem.Object;
            }
            return type;
        }

        private static string LoadParameter(TypeReference type, string param)
        {
            if (type.IsByReference)
            {
                return Emit.Cast(type, param);
            }
            if (((((type.MetadataType == MetadataType.SByte) || (type.MetadataType == MetadataType.Byte)) || ((type.MetadataType == MetadataType.Boolean) || (type.MetadataType == MetadataType.Int16))) || (((type.MetadataType == MetadataType.UInt16) || (type.MetadataType == MetadataType.Char)) || ((type.MetadataType == MetadataType.Int32) || (type.MetadataType == MetadataType.UInt32)))) || ((((type.MetadataType == MetadataType.Int64) || (type.MetadataType == MetadataType.UInt64)) || ((type.MetadataType == MetadataType.IntPtr) || (type.MetadataType == MetadataType.UIntPtr))) || ((type.MetadataType == MetadataType.Single) || (type.MetadataType == MetadataType.Double))))
            {
                string[] textArray1 = new string[] { "*((", Naming.ForVariable(new PointerType(type)), ")", param, ")" };
                return string.Concat(textArray1);
            }
            if ((((type.MetadataType == MetadataType.String) || (type.MetadataType == MetadataType.Class)) || (((type.MetadataType == MetadataType.Array) || (type.MetadataType == MetadataType.Pointer)) || (type.MetadataType == MetadataType.Object))) && !type.IsValueType())
            {
                return Emit.Cast(type, param);
            }
            if ((type.MetadataType == MetadataType.GenericInstance) && !type.IsValueType())
            {
                return Emit.Cast(type, param);
            }
            if (!type.IsValueType())
            {
                throw new Exception();
            }
            if (type.IsEnum())
            {
                return LoadParameter(type.GetUnderlyingEnumType(), param);
            }
            string[] textArray2 = new string[] { "*((", Naming.ForVariable(new PointerType(type)), ")", param, ")" };
            return string.Concat(textArray2);
        }

        private static string NameForInvoker(TypeReference[] data)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("RuntimeInvoker_");
            builder.Append(Naming.ForType(data[0]));
            for (int i = 1; i < data.Length; i++)
            {
                builder.Append("_");
                builder.Append(Naming.ForType(data[i]));
            }
            return builder.ToString();
        }

        public TableInfo Write(NPath path)
        {
            using (SourceCodeWriter writer = new SourceCodeWriter(path))
            {
                writer.AddCodeGenIncludes();
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = new Func<KeyValuePair<TypeReference[], int>, int>(null, (IntPtr) <Write>m__1);
                }
                KeyValuePair<TypeReference[], int>[] items = this._runtimeInvokerData.OrderBy<KeyValuePair<TypeReference[], int>, int>(<>f__am$cache1).ToArray<KeyValuePair<TypeReference[], int>>();
                foreach (KeyValuePair<TypeReference[], int> pair in items)
                {
                    TypeReference[] key = pair.Key;
                    TypeReference type = key[0];
                    writer.AddIncludeOrExternForTypeDefinition(type.Module.TypeSystem.Object);
                    writer.AddIncludeOrExternForTypeDefinition(type);
                    for (int i = 1; i < key.Length; i++)
                    {
                        writer.AddIncludeOrExternForTypeDefinition(key[i]);
                    }
                    object[] args = new object[] { NameForInvoker(key) };
                    writer.WriteLine("void* {0} (const MethodInfo* method, void* obj, void** args)", args);
                    writer.BeginBlock();
                    WriteInvokerBody(writer, key, type);
                    writer.EndBlock(false);
                    writer.WriteLine();
                }
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = new Func<KeyValuePair<TypeReference[], int>, string>(null, (IntPtr) <Write>m__2);
                }
                return MetadataWriter.WriteTable<KeyValuePair<TypeReference[], int>>(writer, "extern const InvokerMethod", "g_Il2CppInvokerPointers", items, <>f__am$cache2);
            }
        }

        private static void WriteInvokerBody(CppCodeWriter writer, TypeReference[] data, TypeReference returnType)
        {
            writer.Write("typedef ");
            writer.Write(Naming.ForVariable(returnType));
            writer.Write(" (*Func)(void* obj");
            for (int i = 1; i < data.Length; i++)
            {
                writer.Write(", ");
                writer.Write(Naming.ForVariable(data[i]));
                writer.Write(" p");
                writer.Write(i.ToString());
            }
            writer.WriteLine(", const MethodInfo* method);");
            if (returnType.MetadataType != MetadataType.Void)
            {
                writer.Write(Naming.ForVariable(returnType));
                writer.Write(" ret = ");
            }
            writer.Write("((Func)method->methodPointer)(obj");
            for (int j = 1; j < data.Length; j++)
            {
                object[] args = new object[] { LoadParameter(data[j], "args[" + (j - 1) + "]") };
                writer.Write(", {0}", args);
            }
            writer.WriteLine(", method);");
            writer.Write("return ");
            if (returnType.MetadataType == MetadataType.Void)
            {
                writer.Write(Naming.Null);
            }
            else
            {
                writer.Write(!returnType.IsValueType() ? "ret" : "Box(il2cpp_codegen_class_from_type (method->return_type), &ret)");
            }
            writer.WriteLine(";");
        }
    }
}

