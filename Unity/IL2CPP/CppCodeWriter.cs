﻿namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.FileNaming;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Metadata;

    public abstract class CppCodeWriter : CodeWriter
    {
        private readonly HashSet<ArrayType> _arrayTypes;
        private readonly HashSet<TypeReference> _forwardDeclarations;
        private readonly HashSet<string> _includes;
        private readonly HashSet<MethodReference> _methods;
        private readonly HashSet<string> _rawMethodForwardDeclarations;
        private readonly HashSet<string> _rawTypeForwardDeclarations;
        private readonly HashSet<MethodReference> _sharedMethods;
        private readonly HashSet<string> _writtenExterns;
        private readonly HashSet<string> _writtenInternalPInvokeMethodDeclarations;
        [CompilerGenerated]
        private static Func<object, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<object, string> <>f__am$cache1;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map0;
        public bool ErrorOccurred;
        [Inject]
        public static IGenericSharingAnalysisService GenericSharingAnalysis;
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static IStatsService StatsService;

        protected CppCodeWriter(StreamWriter stream) : base(stream)
        {
            this._includes = new HashSet<string>();
            this._writtenExterns = new HashSet<string>();
            this._writtenInternalPInvokeMethodDeclarations = new HashSet<string>();
            this._forwardDeclarations = new HashSet<TypeReference>(new Unity.IL2CPP.Common.TypeReferenceEqualityComparer());
            this._arrayTypes = new HashSet<ArrayType>(new Unity.IL2CPP.Common.TypeReferenceEqualityComparer());
            this._rawTypeForwardDeclarations = new HashSet<string>();
            this._rawMethodForwardDeclarations = new HashSet<string>();
            this._methods = new HashSet<MethodReference>(new Unity.IL2CPP.Common.MethodReferenceComparer());
            this._sharedMethods = new HashSet<MethodReference>(new Unity.IL2CPP.Common.MethodReferenceComparer());
        }

        public void AddCodeGenIncludes()
        {
            this.AddInclude("class-internals.h");
            this.AddInclude("codegen/il2cpp-codegen.h");
        }

        public void AddForwardDeclaration(TypeReference typeReference)
        {
            if (typeReference == null)
            {
                throw new ArgumentNullException("typeReference");
            }
            this._forwardDeclarations.Add(GetForwardDeclarationType(typeReference));
        }

        public void AddForwardDeclaration(string declaration)
        {
            if (string.IsNullOrEmpty(declaration))
            {
                throw new ArgumentException("Type forward declaration must not be empty.", "declaration");
            }
            this._rawTypeForwardDeclarations.Add(declaration);
        }

        public void AddInclude(string path)
        {
            this._includes.Add($""{path}"");
        }

        public void AddIncludeForMethodDeclaration(MethodReference method)
        {
            TypeReference declaringType = method.DeclaringType;
            if (((!declaringType.IsInterface() || declaringType.IsComOrWindowsRuntimeInterface()) && !declaringType.IsArray) && (!declaringType.HasGenericParameters && this._methods.Add(method)))
            {
                MethodSignatureWriter.RecordIncludes(this, method);
                if (MethodWriter.GenericSharingAnalysis.CanShareMethod(method))
                {
                    MethodReference sharedMethod = MethodWriter.GenericSharingAnalysis.GetSharedMethod(method);
                    MethodSignatureWriter.RecordIncludes(this, sharedMethod);
                    this._sharedMethods.Add(sharedMethod);
                }
            }
        }

        private void AddIncludeForType(TypeReference type)
        {
            type = Naming.RemoveModifiers(type);
            if (!type.HasGenericParameters && (!type.IsInterface() || type.IsComOrWindowsRuntimeInterface()))
            {
                if (type.IsArray)
                {
                    ArrayType item = (ArrayType) type;
                    this.AddIncludeOrExternForTypeDefinition(item.ElementType);
                    this._arrayTypes.Add(item);
                }
                else
                {
                    this.AddInclude(FileNameProvider.Instance.ForTypeDefinition(type));
                }
            }
        }

        public void AddIncludeForTypeDefinition(TypeReference typeReference)
        {
            TypeReference reference = typeReference;
            if (reference.ContainsGenericParameters())
            {
                if (reference.IsGenericParameter)
                {
                    return;
                }
                TypeDefinition definition = reference.Resolve();
                if ((definition == null) || definition.IsEnum())
                {
                    return;
                }
            }
            reference = Naming.RemoveModifiers(reference);
            ByReferenceType type = reference as ByReferenceType;
            if (type != null)
            {
                this.AddIncludeForTypeDefinition(type.ElementType);
            }
            else
            {
                PointerType type2 = reference as PointerType;
                if (type2 != null)
                {
                    this.AddIncludeForTypeDefinition(type2.ElementType);
                }
                else
                {
                    ArrayType type3 = reference as ArrayType;
                    if (type3 != null)
                    {
                        this.AddIncludeForType(type3);
                        this.AddIncludeForType(type3.ElementType);
                    }
                    else
                    {
                        GenericInstanceType type4 = reference as GenericInstanceType;
                        if (type4 != null)
                        {
                            this.AddIncludeForType(type4);
                        }
                        else
                        {
                            this.AddIncludeForType(reference);
                        }
                    }
                }
            }
        }

        public void AddIncludeOrExternForTypeDefinition(TypeReference type)
        {
            type = Naming.RemoveModifiers(type);
            ByReferenceType type2 = type as ByReferenceType;
            if (type2 != null)
            {
                type = type2.ElementType;
            }
            PointerType type3 = type as PointerType;
            if (type3 != null)
            {
                type = type3.ElementType;
            }
            if (!type.IsValueType())
            {
                this.AddForwardDeclaration(type);
            }
            this.AddIncludeForType(type);
        }

        public void AddIncludesForTypeReference(TypeReference typeReference, bool requiresCompleteType = false)
        {
            TypeReference elementType = typeReference;
            if (!elementType.ContainsGenericParameters())
            {
                ArrayType type = elementType as ArrayType;
                if (type != null)
                {
                    this.AddForwardDeclaration(type);
                }
                GenericInstanceType type2 = elementType as GenericInstanceType;
                if (type2 != null)
                {
                    if (type2.ElementType.IsValueType())
                    {
                        this.AddIncludeForType(type2);
                    }
                    else
                    {
                        this.AddForwardDeclaration(type2);
                    }
                }
                ByReferenceType type3 = elementType as ByReferenceType;
                if (type3 != null)
                {
                    elementType = type3.ElementType;
                }
                PointerType type4 = elementType as PointerType;
                if (type4 != null)
                {
                    elementType = type4.ElementType;
                }
                if (elementType.IsPrimitive)
                {
                    if ((elementType.MetadataType == MetadataType.IntPtr) || (elementType.MetadataType == MetadataType.UIntPtr))
                    {
                        this.AddIncludeForType(elementType);
                    }
                }
                else
                {
                    bool flag = elementType.IsValueType();
                    if (flag || (requiresCompleteType && !(elementType is TypeSpecification)))
                    {
                        this.AddIncludeForType(elementType);
                    }
                    if (!flag)
                    {
                        this.AddForwardDeclaration(elementType);
                    }
                }
            }
        }

        public void AddMethodForwardDeclaration(string declaration)
        {
            if (string.IsNullOrEmpty(declaration))
            {
                throw new ArgumentException("Method forward declaration must not be empty.", "declaration");
            }
            this._rawMethodForwardDeclarations.Add(declaration);
        }

        public void AddRawInclude(string path)
        {
            this._includes.Add(path);
        }

        public void AddStdInclude(string path)
        {
            this._includes.Add($"<{path}>");
        }

        private static TypeReference GetForwardDeclarationType(TypeReference typeReference)
        {
            typeReference = Naming.RemoveModifiers(typeReference);
            PointerType type = typeReference as PointerType;
            if (type != null)
            {
                return GetForwardDeclarationType(type.ElementType);
            }
            ByReferenceType type2 = typeReference as ByReferenceType;
            if (type2 != null)
            {
                return GetForwardDeclarationType(type2.ElementType);
            }
            return typeReference;
        }

        public static string InitializerStringFor(TypeReference type)
        {
            if (((type.FullName == "intptr_t") || (type.FullName == "uintptr_t")) || type.IsEnum())
            {
                return " = 0";
            }
            if (type.IsPrimitive)
            {
                string str2 = InitializerStringForPrimitiveType(type);
                if (str2 != null)
                {
                    return $" = {str2}";
                }
                return string.Empty;
            }
            if (!type.IsValueType())
            {
                return $" = {Naming.Null}";
            }
            return string.Empty;
        }

        public static string InitializerStringForPrimitiveCppType(string typeName)
        {
            if (typeName != null)
            {
                int num;
                if (<>f__switch$map0 == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(14) {
                        { 
                            "bool",
                            0
                        },
                        { 
                            "char",
                            1
                        },
                        { 
                            "wchar_t",
                            1
                        },
                        { 
                            "size_t",
                            2
                        },
                        { 
                            "int8_t",
                            2
                        },
                        { 
                            "int16_t",
                            2
                        },
                        { 
                            "int32_t",
                            2
                        },
                        { 
                            "int64_t",
                            2
                        },
                        { 
                            "uint8_t",
                            2
                        },
                        { 
                            "uint16_t",
                            2
                        },
                        { 
                            "uint32_t",
                            2
                        },
                        { 
                            "uint64_t",
                            2
                        },
                        { 
                            "double",
                            3
                        },
                        { 
                            "float",
                            4
                        }
                    };
                    <>f__switch$map0 = dictionary;
                }
                if (<>f__switch$map0.TryGetValue(typeName, out num))
                {
                    switch (num)
                    {
                        case 0:
                            return InitializerStringForPrimitiveType(MetadataType.Boolean);

                        case 1:
                            return InitializerStringForPrimitiveType(MetadataType.Char);

                        case 2:
                            return InitializerStringForPrimitiveType(MetadataType.Int32);

                        case 3:
                            return InitializerStringForPrimitiveType(MetadataType.Double);

                        case 4:
                            return InitializerStringForPrimitiveType(MetadataType.Single);
                    }
                }
            }
            return null;
        }

        public static string InitializerStringForPrimitiveType(MetadataType type)
        {
            switch (type)
            {
                case MetadataType.Boolean:
                    return "false";

                case MetadataType.Char:
                case MetadataType.SByte:
                case MetadataType.Byte:
                    return "0x0";

                case MetadataType.Int16:
                case MetadataType.UInt16:
                case MetadataType.Int32:
                case MetadataType.UInt32:
                case MetadataType.Int64:
                case MetadataType.UInt64:
                    return "0";

                case MetadataType.Single:
                    return "0.0f";

                case MetadataType.Double:
                    return "0.0";
            }
            return null;
        }

        public static string InitializerStringForPrimitiveType(TypeReference type) => 
            InitializerStringForPrimitiveType(type.MetadataType);

        public void Write(CppCodeWriter other)
        {
            foreach (TypeReference reference in other._forwardDeclarations)
            {
                this._forwardDeclarations.Add(reference);
            }
            foreach (string str in other._rawTypeForwardDeclarations)
            {
                this._rawTypeForwardDeclarations.Add(str);
            }
            foreach (string str2 in other._rawMethodForwardDeclarations)
            {
                this._rawMethodForwardDeclarations.Add(str2);
            }
            foreach (string str3 in other._includes)
            {
                this._includes.Add(str3);
            }
            foreach (MethodReference reference2 in other._methods)
            {
                this._methods.Add(reference2);
            }
            foreach (MethodReference reference3 in other._sharedMethods)
            {
                this._sharedMethods.Add(reference3);
            }
            foreach (ArrayType type in other._arrayTypes)
            {
                this._arrayTypes.Add(type);
            }
            base.Writer.Flush();
            other.Writer.Flush();
            Stream baseStream = other.Writer.BaseStream;
            long position = baseStream.Position;
            baseStream.Seek(0L, SeekOrigin.Begin);
            baseStream.CopyTo(base.Writer.BaseStream);
            baseStream.Seek(position, SeekOrigin.Begin);
            base.Writer.Flush();
        }

        public void WriteArrayInitializer(params object[] values)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = v => v.ToString();
            }
            this.WriteFieldInitializer(values.Select<object, string>(<>f__am$cache0));
        }

        public TableInfo WriteArrayInitializer(string type, string variableName, IEnumerable<string> values, bool nullTerminate = true)
        {
            values = !nullTerminate ? values : values.Concat<string>(new string[] { "NULL" });
            string[] initializers = values.ToArray<string>();
            object[] args = new object[] { type, variableName, initializers.Length };
            base.WriteLine("{0} {1}[{2}] = ", args);
            this.WriteFieldInitializer(initializers);
            return new TableInfo(initializers.Length, type, variableName);
        }

        public void WriteClangWarningDisables()
        {
            base.WriteLine("#ifdef __clang__");
            base.WriteLine("#pragma clang diagnostic push");
            base.WriteLine("#pragma clang diagnostic ignored \"-Winvalid-offsetof\"");
            base.WriteLine("#pragma clang diagnostic ignored \"-Wunused-variable\"");
            base.WriteLine("#endif");
        }

        public void WriteClangWarningEnables()
        {
            base.WriteLine("#ifdef __clang__");
            base.WriteLine("#pragma clang diagnostic pop");
            base.WriteLine("#endif");
        }

        public void WriteDefaultReturn(TypeReference type)
        {
            if (type.MetadataType == MetadataType.Void)
            {
                base.WriteLine("return;");
            }
            else
            {
                this.WriteVariable(type, "ret");
                base.WriteLine("return ret;");
            }
        }

        internal void WriteExternForGenericClass(TypeReference type)
        {
            string item = Naming.ForGenericClass(type);
            if (this._writtenExterns.Add(item))
            {
                base.WriteLine($"extern Il2CppGenericClass {item};");
            }
        }

        internal void WriteExternForIl2CppGenericInst(IList<TypeReference> types)
        {
            string item = Naming.ForGenericInst(types);
            if (this._writtenExterns.Add(item))
            {
                base.WriteLine($"extern const Il2CppGenericInst {item};");
            }
        }

        internal void WriteExternForIl2CppType(TypeReference type)
        {
            string item = Naming.ForIl2CppType(type, 0);
            if (this._writtenExterns.Add(item))
            {
                base.WriteLine($"extern const Il2CppType {item};");
            }
        }

        protected void WriteFieldInitializer(IEnumerable<string> initializers)
        {
            base.BeginBlock();
            foreach (string str in initializers)
            {
                object[] args = new object[] { str };
                base.WriteLine("{0},", args);
            }
            base.EndBlock(true);
        }

        public void WriteIfNotEmpty(Action<CppCodeWriter> writePrefixIfNotEmpty, Action<CppCodeWriter> writeContent, Action<CppCodeWriter> writePostfixIfNotEmpty)
        {
            <WriteIfNotEmpty>c__AnonStorey0 storey = new <WriteIfNotEmpty>c__AnonStorey0 {
                writeContent = writeContent
            };
            this.WriteIfNotEmpty<object>(writePrefixIfNotEmpty, new Func<CppCodeWriter, object>(storey.<>m__0), writePostfixIfNotEmpty);
        }

        public T WriteIfNotEmpty<T>(Action<CppCodeWriter> writePrefixIfNotEmpty, Func<CppCodeWriter, T> writeContent, Action<CppCodeWriter> writePostfixIfNotEmpty)
        {
            T local2;
            using (InMemoryCodeWriter writer = new InMemoryCodeWriter())
            {
                using (InMemoryCodeWriter writer2 = new InMemoryCodeWriter())
                {
                    writer.Indent(base.IndentationLevel);
                    writePrefixIfNotEmpty(writer);
                    writer2.Indent(writer.IndentationLevel);
                    T local = writeContent(writer2);
                    writer2.Dedent(writer.IndentationLevel);
                    writer.Dedent(base.IndentationLevel);
                    writer2.Writer.Flush();
                    if (writer2.Writer.BaseStream.Length > 0L)
                    {
                        writer.Writer.Flush();
                        this.Write(writer);
                        this.Write(writer2);
                        int count = writer.IndentationLevel + writer2.IndentationLevel;
                        if (count > 0)
                        {
                            base.Indent(count);
                        }
                        else if (count < 0)
                        {
                            base.Dedent(-count);
                        }
                        if (writePostfixIfNotEmpty != null)
                        {
                            writePostfixIfNotEmpty(this);
                        }
                    }
                    local2 = local;
                }
            }
            return local2;
        }

        public void WriteInternalCallResolutionStatement(MethodDefinition method)
        {
            string str = method.FullName.Substring(method.FullName.IndexOf(" ") + 1);
            object[] args = new object[] { MethodSignatureWriter.GetICallMethodVariable(method) };
            base.WriteLine("typedef {0};", args);
            object[] objArray2 = new object[] { Naming.ForMethodNameOnly(method) };
            base.WriteLine("static {0}_ftn _il2cpp_icall_func;", objArray2);
            object[] objArray3 = new object[] { Naming.ForMethodNameOnly(method) };
            base.WriteLine("if (!_il2cpp_icall_func)", objArray3);
            object[] objArray4 = new object[] { Naming.ForMethodNameOnly(method), str };
            base.WriteLine("_il2cpp_icall_func = ({0}_ftn)il2cpp_codegen_resolve_icall (\"{1}\");", objArray4);
        }

        public void WriteInternalPInvokeDeclaration(string methodName, string internalPInvokeDeclaration)
        {
            if (!this._writtenInternalPInvokeMethodDeclarations.Contains(methodName))
            {
                base.WriteLine(internalPInvokeDeclaration);
                this._writtenInternalPInvokeMethodDeclarations.Add(methodName);
            }
        }

        public void WriteNullTerminatedArrayInitializer(params object[] values)
        {
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = v => v.ToString();
            }
            string[] second = new string[] { "NULL" };
            this.WriteFieldInitializer(values.Select<object, string>(<>f__am$cache1).Concat<string>(second));
        }

        public void WriteStructInitializer(string type, string variableName, IEnumerable<string> values)
        {
            object[] args = new object[] { type, variableName };
            base.WriteLine("{0} {1} = ", args);
            this.WriteFieldInitializer(values);
        }

        public void WriteVariable(TypeReference type, string name)
        {
            if (type.IsGenericParameter())
            {
                throw new ArgumentException("Generic parameter encountered as variable type", "type");
            }
            string str = InitializerStringFor(type);
            string str2 = Naming.ForVariable(type);
            if (!string.IsNullOrEmpty(str))
            {
                object[] args = new object[] { str2, name, str };
                base.WriteLine("{0} {1}{2};", args);
            }
            else
            {
                object[] objArray2 = new object[] { str2, name };
                base.WriteLine("{0} {1};", objArray2);
                object[] objArray3 = new object[] { name };
                base.WriteLine("memset(&{0}, 0, sizeof({0}));", objArray3);
            }
        }

        public IEnumerable<ArrayType> ArrayTypes =>
            this._arrayTypes;

        public IEnumerable<TypeReference> ForwardDeclarations =>
            this._forwardDeclarations;

        public IEnumerable<string> Includes =>
            this._includes;

        public IEnumerable<MethodReference> Methods =>
            this._methods;

        public IEnumerable<string> RawMethodForwardDeclarations =>
            this._rawMethodForwardDeclarations;

        public IEnumerable<string> RawTypeForwardDeclarations =>
            this._rawTypeForwardDeclarations;

        public IEnumerable<MethodReference> SharedMethods =>
            this._sharedMethods;

        public IEnumerable<string> WrittenExterns =>
            this._writtenExterns;

        [CompilerGenerated]
        private sealed class <WriteIfNotEmpty>c__AnonStorey0
        {
            internal Action<CppCodeWriter> writeContent;

            internal object <>m__0(CppCodeWriter bodyWriter)
            {
                this.writeContent(bodyWriter);
                return null;
            }
        }
    }
}

