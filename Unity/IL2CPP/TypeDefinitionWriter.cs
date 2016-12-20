namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using Unity.IL2CPP.Com;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Marshaling;

    public class TypeDefinitionWriter
    {
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache5;
        private const char kArrayFirstIndexName = 'i';
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static ITypeProviderService TypeProvider;

        internal static int AlignmentPackingSizeFor(TypeDefinition typeDefinition)
        {
            return typeDefinition.PackingSize;
        }

        internal static int AlignmentSizeFor(TypeDefinition typeDefinition)
        {
            foreach (CustomAttribute attribute in typeDefinition.CustomAttributes)
            {
                if (attribute.AttributeType.FullName.Contains("IL2CPPStructAlignment"))
                {
                    foreach (CustomAttributeNamedArgument argument in attribute.Fields)
                    {
                        if (argument.Name == "Align")
                        {
                            return (int) argument.Argument.Value;
                        }
                    }
                }
            }
            return 1;
        }

        private static string BoundVariableNameFor(int i)
        {
            return string.Format("{0}Bound", (char) (0x69 + i));
        }

        private static string BuildArrayBoundsVariables(StringBuilder stringBuilder, int rank, bool emitArrayBoundsCheck, int indentationLevel)
        {
            stringBuilder.Clear();
            string str = new string('\t', indentationLevel);
            for (int i = 0; i < rank; i++)
            {
                string length = BoundVariableNameFor(i);
                if (i != 0)
                {
                    stringBuilder.Append(str);
                }
                object[] args = new object[] { Naming.ForArrayIndexType(), length, i, Environment.NewLine };
                stringBuilder.AppendFormat("{0} {1} = bounds[{2}].length;{3}", args);
                if (emitArrayBoundsCheck)
                {
                    char ch = (char) (0x69 + i);
                    stringBuilder.AppendFormat("{0}{1}{2}", str, Emit.MultiDimensionalArrayBoundsCheck(length, ch.ToString()), Environment.NewLine);
                }
            }
            return stringBuilder.ToString();
        }

        private static string BuildArrayIndexCalculation(StringBuilder stringBuilder, int rank)
        {
            stringBuilder.Clear();
            stringBuilder.AppendFormat("{0} {1} = ", Naming.ForArrayIndexType(), Naming.ForArrayIndexName());
            for (int i = 0; i < (rank - 2); i++)
            {
                stringBuilder.Append('(');
            }
            for (int j = 0; j < rank; j++)
            {
                stringBuilder.Append((char) (0x69 + j));
                if ((j != 0) && (j != (rank - 1)))
                {
                    stringBuilder.Append(')');
                }
                if (j != (rank - 1))
                {
                    stringBuilder.AppendFormat(" * {0} + ", BoundVariableNameFor(j + 1));
                }
            }
            stringBuilder.Append(';');
            return stringBuilder.ToString();
        }

        private static string BuildArrayIndexParameters(StringBuilder stringBuilder, int rank)
        {
            stringBuilder.Clear();
            char ch = (char) (0x69 + rank);
            for (char ch2 = 'i'; ch2 < ch; ch2 = (char) (ch2 + '\x0001'))
            {
                stringBuilder.AppendFormat("{0} {1}", Naming.ForArrayIndexType(), ch2);
                if (ch2 != (ch - '\x0001'))
                {
                    stringBuilder.Append(", ");
                }
            }
            return stringBuilder.ToString();
        }

        private void CollectIncludes(CppCodeWriter writer, TypeReference type, TypeDefinition typeDefinition)
        {
            if (!type.HasGenericParameters)
            {
                if (type is ArrayType)
                {
                    writer.AddIncludeForTypeDefinition(TypeProvider.Corlib.MainModule.GetType("System.Array"));
                }
                else
                {
                    Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = new Unity.IL2CPP.ILPreProcessor.TypeResolver(type as GenericInstanceType);
                    TypeReference typeReference = resolver.Resolve(typeDefinition.BaseType);
                    if (typeReference != null)
                    {
                        writer.AddIncludeForTypeDefinition(typeReference);
                    }
                    foreach (FieldDefinition definition in typeDefinition.Fields)
                    {
                        writer.AddIncludesForTypeReference(resolver.Resolve(definition.FieldType), false);
                    }
                    foreach (FieldDefinition definition2 in typeDefinition.Fields)
                    {
                        PointerType fieldType = definition2.FieldType as PointerType;
                        if (fieldType != null)
                        {
                            writer.AddForwardDeclaration(resolver.Resolve(fieldType.ElementType));
                        }
                    }
                    foreach (TypeReference reference2 in Extensions.GetAllFactoryTypes(type))
                    {
                        writer.AddForwardDeclaration(reference2);
                    }
                    if (Extensions.IsDelegate(typeDefinition))
                    {
                        MethodDefinition[] definitionArray;
                        if (!typeDefinition.IsWindowsRuntime)
                        {
                            MethodDefinition[] definitionArray1 = new MethodDefinition[3];
                            if (<>f__am$cache2 == null)
                            {
                                <>f__am$cache2 = new Func<MethodDefinition, bool>(null, (IntPtr) <CollectIncludes>m__2);
                            }
                            definitionArray1[0] = Enumerable.Single<MethodDefinition>(typeDefinition.Methods, <>f__am$cache2);
                            if (<>f__am$cache3 == null)
                            {
                                <>f__am$cache3 = new Func<MethodDefinition, bool>(null, (IntPtr) <CollectIncludes>m__3);
                            }
                            definitionArray1[1] = Enumerable.Single<MethodDefinition>(typeDefinition.Methods, <>f__am$cache3);
                            if (<>f__am$cache4 == null)
                            {
                                <>f__am$cache4 = new Func<MethodDefinition, bool>(null, (IntPtr) <CollectIncludes>m__4);
                            }
                            definitionArray1[2] = Enumerable.Single<MethodDefinition>(typeDefinition.Methods, <>f__am$cache4);
                            definitionArray = definitionArray1;
                        }
                        else
                        {
                            MethodDefinition[] definitionArray3 = new MethodDefinition[1];
                            if (<>f__am$cache5 == null)
                            {
                                <>f__am$cache5 = new Func<MethodDefinition, bool>(null, (IntPtr) <CollectIncludes>m__5);
                            }
                            definitionArray3[0] = Enumerable.Single<MethodDefinition>(typeDefinition.Methods, <>f__am$cache5);
                            definitionArray = definitionArray3;
                        }
                        foreach (MethodDefinition definition3 in definitionArray)
                        {
                            writer.AddIncludesForTypeReference(resolver.Resolve(Unity.IL2CPP.GenericParameterResolver.ResolveReturnTypeIfNeeded(definition3)), false);
                            foreach (ParameterDefinition definition4 in definition3.Parameters)
                            {
                                TypeReference elementType = resolver.Resolve(Unity.IL2CPP.GenericParameterResolver.ResolveParameterTypeIfNeeded(definition3, definition4));
                                writer.AddIncludesForTypeReference(elementType, false);
                                if (elementType.IsByReference)
                                {
                                    ByReferenceType type3 = (ByReferenceType) elementType;
                                    if (Extensions.IsValueType(type3.ElementType))
                                    {
                                        elementType = type3.ElementType;
                                    }
                                }
                                if (Extensions.IsValueType(elementType))
                                {
                                    writer.AddIncludeForTypeDefinition(elementType);
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static int FieldLayoutPackingSizeFor(TypeDefinition typeDefinition)
        {
            if (typeDefinition.IsExplicitLayout)
            {
                return 1;
            }
            return typeDefinition.PackingSize;
        }

        private static bool FieldMatches(FieldDefinition field, FieldType fieldType)
        {
            if (fieldType == FieldType.Static)
            {
                return Extensions.IsNormalStatic(field);
            }
            if (fieldType == FieldType.ThreadStatic)
            {
                return Extensions.IsThreadStatic(field);
            }
            return !field.IsStatic;
        }

        private string GetBaseTypeDeclaration(TypeReference type)
        {
            if (type.IsArray)
            {
                return string.Format(" : public " + Naming.ForType(TypeProvider.Corlib.MainModule.GetType("System.Array")), new object[0]);
            }
            TypeDefinition definition = type.Resolve();
            if (((definition.BaseType != null) && (definition.BaseType.FullName != "System.Enum")) && ((definition.BaseType.FullName != "System.ValueType") || (definition.FullName == "System.Enum")))
            {
                Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(type);
                return string.Format(" : public " + Naming.ForType(resolver.Resolve(definition.BaseType)), new object[0]);
            }
            return string.Empty;
        }

        private static string GetDeclaringTypeStructName(TypeReference declaringType, FieldReference field)
        {
            if (Extensions.IsThreadStatic(field))
            {
                return Naming.ForThreadFieldsStruct(declaringType);
            }
            if (Extensions.IsNormalStatic(field))
            {
                return Naming.ForStaticFieldsStruct(declaringType);
            }
            return Naming.ForTypeNameOnly(declaringType);
        }

        private static List<ComFieldWriteInstruction> MakeComFieldWriteInstructionsForType(CppCodeWriter writer, TypeReference type, TypeDefinition typeDefinition, FieldType fieldType)
        {
            if (!Extensions.IsComOrWindowsRuntimeType(typeDefinition))
            {
                return new List<ComFieldWriteInstruction>();
            }
            TypeReference[] referenceArray = (fieldType != FieldType.Static) ? Enumerable.ToArray<TypeReference>(Extensions.ImplementedComOrWindowsRuntimeInterfaces(typeDefinition)) : Enumerable.ToArray<TypeReference>(Extensions.GetAllFactoryTypes(typeDefinition));
            List<ComFieldWriteInstruction> list2 = new List<ComFieldWriteInstruction>(referenceArray.Length);
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(type);
            bool flag = false;
            foreach (TypeReference reference in referenceArray)
            {
                if (Extensions.IsIActivationFactory(reference))
                {
                    flag = true;
                }
                list2.Add(new ComFieldWriteInstruction(resolver.Resolve(reference), fieldType == FieldType.Static));
            }
            if ((!flag && (fieldType == FieldType.Static)) && (list2.Count > 0))
            {
                list2.Insert(0, new ComFieldWriteInstruction(TypeProvider.IActivationFactoryTypeReference, true));
            }
            return list2;
        }

        private static List<FieldWriteInstruction> MakeFieldWriteInstructionsForType(CppCodeWriter writer, TypeReference type, TypeDefinition typeDefinition, FieldType fieldType)
        {
            <MakeFieldWriteInstructionsForType>c__AnonStorey0 storey = new <MakeFieldWriteInstructionsForType>c__AnonStorey0 {
                fieldType = fieldType
            };
            List<FieldWriteInstruction> list = new List<FieldWriteInstruction>();
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(type);
            foreach (FieldDefinition definition in Enumerable.Where<FieldDefinition>(typeDefinition.Fields, new Func<FieldDefinition, bool>(storey, (IntPtr) this.<>m__0)))
            {
                string str;
                FieldReference reference;
                TypeReference reference2;
                if ((definition.DeclaringType.FullName == "System.Delegate") && (definition.Name == "method_ptr"))
                {
                    reference = definition;
                    str = "Il2CppMethodPointer";
                    reference2 = definition.FieldType;
                }
                else
                {
                    reference = new FieldReference(definition.Name, definition.FieldType, type);
                    reference2 = resolver.ResolveFieldType(reference);
                    str = Naming.ForVariable(reference2);
                }
                list.Add(new FieldWriteInstruction(definition, str, reference2));
            }
            return list;
        }

        private static bool NeedsPacking(TypeDefinition typeDefinition, LayoutMode layoutMode)
        {
            if (typeDefinition.IsExplicitLayout)
            {
                return true;
            }
            if ((layoutMode == LayoutMode.Managed) && !MarshalingUtils.IsBlittable(typeDefinition, null, MarshalType.PInvoke))
            {
                return false;
            }
            return ((typeDefinition.IsSequentialLayout && (typeDefinition.PackingSize != 0)) && (typeDefinition.PackingSize != -1));
        }

        internal static bool NeedsPackingForManaged(TypeDefinition typeDefinition)
        {
            return NeedsPacking(typeDefinition, LayoutMode.Managed);
        }

        internal static bool NeedsPackingForNative(TypeDefinition typeDefinition)
        {
            return NeedsPacking(typeDefinition, LayoutMode.Native);
        }

        internal static bool NeedsPadding(TypeDefinition typeDefinition)
        {
            return (typeDefinition.ClassSize > 0);
        }

        private static void WriteAccessSpecifier(CppCodeWriter writer, string accessSpecifier)
        {
            writer.Dedent(1);
            object[] args = new object[] { accessSpecifier };
            writer.WriteLine("{0}:", args);
            writer.Indent(1);
        }

        private static void WriteArrayAccessors(CppCodeWriter writer, TypeReference elementType, string elementTypeName, bool emitArrayBoundsCheck)
        {
            object[] args = new object[] { elementTypeName, Naming.ForArrayItemGetter(emitArrayBoundsCheck), Naming.ForArrayIndexType(), Naming.ForArrayIndexName(), Naming.ForArrayItems() };
            writer.WriteLine("inline {0} {1}({2} {3}) const", args);
            using (new BlockWriter(writer, false))
            {
                if (emitArrayBoundsCheck)
                {
                    writer.WriteLine(Emit.ArrayBoundsCheck("this", "index"));
                }
                object[] objArray2 = new object[] { Naming.ForArrayItems(), Naming.ForArrayIndexName() };
                writer.WriteLine("return {0}[{1}];", objArray2);
            }
            object[] objArray3 = new object[] { elementTypeName, Naming.ForArrayItemAddressGetter(emitArrayBoundsCheck), Naming.ForArrayIndexType(), Naming.ForArrayIndexName(), Naming.ForArrayItems() };
            writer.WriteLine("inline {0}* {1}({2} {3})", objArray3);
            using (new BlockWriter(writer, false))
            {
                if (emitArrayBoundsCheck)
                {
                    writer.WriteLine(Emit.ArrayBoundsCheck("this", "index"));
                }
                object[] objArray4 = new object[] { Naming.ForArrayItems(), Naming.ForArrayIndexName() };
                writer.WriteLine("return {0} + {1};", objArray4);
            }
            object[] objArray5 = new object[] { Naming.ForArrayItemSetter(emitArrayBoundsCheck), Naming.ForArrayIndexType(), Naming.ForArrayIndexName(), elementTypeName };
            writer.WriteLine("inline void {0}({1} {2}, {3} value)", objArray5);
            using (new BlockWriter(writer, false))
            {
                if (emitArrayBoundsCheck)
                {
                    writer.WriteLine(Emit.ArrayBoundsCheck("this", "index"));
                }
                object[] objArray6 = new object[] { Naming.ForArrayItems(), Naming.ForArrayIndexName() };
                writer.WriteLine("{0}[{1}] = value;", objArray6);
                writer.WriteWriteBarrierIfNeeded(elementType, string.Format("{0} + {1}", Naming.ForArrayItems(), Naming.ForArrayIndexName()), "value");
            }
        }

        private static void WriteArrayAccessorsForMultiDimensionalArray(CppCodeWriter writer, int rank, TypeReference elementType, string elementTypeName, bool emitArrayBoundsCheck)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string str = BuildArrayIndexParameters(stringBuilder, rank);
            string block = BuildArrayIndexCalculation(stringBuilder, rank);
            string str3 = BuildArrayBoundsVariables(stringBuilder, rank, emitArrayBoundsCheck, writer.IndentationLevel + 1);
            object[] args = new object[] { elementTypeName, Naming.ForArrayItemGetter(emitArrayBoundsCheck), str };
            writer.WriteLine("inline {0} {1}({2}) const", args);
            using (new BlockWriter(writer, false))
            {
                writer.WriteLine(str3);
                writer.WriteLine(block);
                object[] objArray2 = new object[] { Naming.ForArrayItems(), Naming.ForArrayIndexName() };
                writer.WriteLine("return {0}[{1}];", objArray2);
            }
            object[] objArray3 = new object[] { elementTypeName, Naming.ForArrayItemAddressGetter(emitArrayBoundsCheck), str };
            writer.WriteLine("inline {0}* {1}({2})", objArray3);
            using (new BlockWriter(writer, false))
            {
                writer.WriteLine(str3);
                writer.WriteLine(block);
                object[] objArray4 = new object[] { Naming.ForArrayItems(), Naming.ForArrayIndexName() };
                writer.WriteLine("return {0} + {1};", objArray4);
            }
            object[] objArray5 = new object[] { Naming.ForArrayItemSetter(emitArrayBoundsCheck), str, elementTypeName };
            writer.WriteLine("inline void {0}({1}, {2} value)", objArray5);
            using (new BlockWriter(writer, false))
            {
                writer.WriteLine(str3);
                writer.WriteLine(block);
                object[] objArray6 = new object[] { Naming.ForArrayItems(), Naming.ForArrayIndexName() };
                writer.WriteLine("{0}[{1}] = value;", objArray6);
                writer.WriteWriteBarrierIfNeeded(elementType, string.Format("{0} + {1}", Naming.ForArrayItems(), Naming.ForArrayIndexName()), "value");
            }
        }

        private static void WriteArrayFieldsWithAccessors(CppCodeWriter writer, ArrayType arrayType)
        {
            TypeReference elementType = arrayType.ElementType;
            string elementTypeName = Naming.ForVariable(elementType);
            WriteAccessSpecifier(writer, "public");
            object[] args = new object[] { Naming.ForVariable(arrayType.ElementType), Naming.ForArrayItems() };
            writer.WriteLine("ALIGN_FIELD (8) {0} {1}[1];", args);
            writer.WriteLine();
            WriteAccessSpecifier(writer, "public");
            WriteArrayAccessors(writer, elementType, elementTypeName, true);
            WriteArrayAccessors(writer, elementType, elementTypeName, false);
            if (arrayType.Rank > 1)
            {
                WriteArrayAccessorsForMultiDimensionalArray(writer, arrayType.Rank, elementType, elementTypeName, true);
                WriteArrayAccessorsForMultiDimensionalArray(writer, arrayType.Rank, elementType, elementTypeName, false);
            }
        }

        public void WriteArrayTypeDefinition(TypeReference type, CppCodeWriter writer)
        {
            if (!(type is ArrayType))
            {
                throw new ArgumentException("ArrayType expected", "type");
            }
            ErrorInformation.CurrentlyProcessing.Type = type.Resolve();
            if (CodeGenOptions.EnableErrorMessageTest)
            {
                ErrorTypeAndMethod.ThrowIfIsErrorType(type.Resolve());
            }
            writer.WriteCommentedLine(type.FullName);
            object[] args = new object[] { Naming.ForTypeNameOnly(type), this.GetBaseTypeDeclaration(type) };
            writer.WriteLine("struct {0} {1}", args);
            writer.BeginBlock();
            WriteArrayFieldsWithAccessors(writer, (ArrayType) type);
            writer.EndBlock(true);
        }

        private static void WriteCLSID(CppCodeWriter writer, TypeDefinition type)
        {
            if (type.IsImport && !Extensions.IsWindowsRuntimeProjection(type))
            {
                WriteAccessSpecifier(writer, "public");
                writer.WriteLine("static const Il2CppGuid CLSID;");
                writer.WriteLine();
            }
        }

        private static void WriteComFieldGetters(CppCodeWriter writer, TypeReference declaringType, List<ComFieldWriteInstruction> fieldWriteInstructions)
        {
            bool flag = Enumerable.Count<TypeReference>(Extensions.GetComposableFactoryTypes(declaringType)) > 0;
            for (int i = 0; i < fieldWriteInstructions.Count; i++)
            {
                ComFieldWriteInstruction instruction = fieldWriteInstructions[i];
                TypeReference interfaceType = instruction.InterfaceType;
                string str = Naming.ForTypeNameOnly(interfaceType);
                string str2 = Naming.ForComTypeInterfaceFieldName(interfaceType);
                string str3 = Naming.ForInteropReturnValue();
                if (i != 0)
                {
                    writer.WriteLine();
                }
                writer.AddIncludeForTypeDefinition(interfaceType);
                writer.WriteLine(string.Format("inline {0}* {1}()", str, Naming.ForComTypeInterfaceFieldGetter(interfaceType)));
                using (new BlockWriter(writer, false))
                {
                    writer.WriteLine(string.Format("{0}* {1} = {2};", str, str3, str2));
                    writer.WriteLine(string.Format("if ({0} == {1})", str3, Naming.Null));
                    using (new BlockWriter(writer, false))
                    {
                        if (instruction.IsStatic && Extensions.IsIActivationFactory(interfaceType))
                        {
                            writer.WriteLine(string.Format("il2cpp::utils::StringView<Il2CppNativeChar> className(IL2CPP_NATIVE_STRING(\"{0}\"));", declaringType.FullName));
                            writer.WriteStatement(Emit.Assign(str3, "il2cpp_codegen_windows_runtime_get_activation_factory(className)"));
                        }
                        else
                        {
                            string str4 = !instruction.IsStatic ? Naming.ForIl2CppComObjectIdentityField() : Emit.Call(Naming.ForComTypeInterfaceFieldGetter(TypeProvider.IActivationFactoryTypeReference));
                            string left = string.Format(string.Format("const il2cpp_hresult_t {0}", Naming.ForInteropHResultVariable()), new object[0]);
                            string right = string.Format(string.Format("{0}->QueryInterface({1}::IID, reinterpret_cast<void**>(&{2}))", str4, str, str3), new object[0]);
                            writer.WriteStatement(Emit.Assign(left, right));
                            writer.WriteStatement(Emit.Call("il2cpp_codegen_com_raise_exception_if_failed", Naming.ForInteropHResultVariable()));
                        }
                        writer.WriteLine();
                        writer.WriteLine(string.Format("if (il2cpp_codegen_atomic_compare_exchange_pointer<{0}>({1}, {2}, {3}) != {4})", new object[] { str, Naming.AddressOf(str2), str3, Naming.Null, Naming.Null }));
                        using (new BlockWriter(writer, false))
                        {
                            writer.WriteLine(string.Format("{0}->Release();", str3));
                            writer.WriteStatement(Emit.Assign(str3, str2));
                        }
                        if (flag && !instruction.IsStatic)
                        {
                            writer.WriteLine("else if (!klass->is_import_or_windows_runtime)");
                            using (new BlockWriter(writer, false))
                            {
                                writer.WriteLine(string.Format("{0}->Release();", str3));
                            }
                        }
                    }
                    writer.WriteLine(string.Format("return {0};", str3));
                }
            }
        }

        private static void WriteFieldGettersAndSetters(CppCodeWriter writer, TypeReference declaringType, List<FieldWriteInstruction> fieldWriteInstructions)
        {
            for (int i = 0; i < fieldWriteInstructions.Count; i++)
            {
                FieldWriteInstruction instruction = fieldWriteInstructions[i];
                object[] args = new object[] { Naming.ForFieldOffsetGetter(instruction.Field), GetDeclaringTypeStructName(declaringType, instruction.Field), Naming.ForField(instruction.Field) };
                writer.WriteLine("inline static int32_t {0}() {{ return static_cast<int32_t>(offsetof({1}, {2})); }}", args);
                object[] objArray2 = new object[] { instruction.FieldTypeName, Naming.ForFieldGetter(instruction.Field), instruction.FieldName };
                writer.WriteLine("inline {0} {1}() const {{ return {2}; }}", objArray2);
                object[] objArray3 = new object[] { instruction.FieldTypeName, Naming.ForFieldAddressGetter(instruction.Field), instruction.FieldName };
                writer.WriteLine("inline {0}* {1}() {{ return &{2}; }}", objArray3);
                object[] objArray4 = new object[] { Naming.ForFieldSetter(instruction.Field), instruction.FieldTypeName };
                writer.WriteLine("inline void {0}({1} value)", objArray4);
                using (new BlockWriter(writer, false))
                {
                    object[] objArray5 = new object[] { instruction.FieldName };
                    writer.WriteLine("{0} = value;", objArray5);
                    writer.WriteWriteBarrierIfNeeded(instruction.FieldType, Naming.AddressOf(instruction.FieldName), "value");
                }
                if (i != (fieldWriteInstructions.Count - 1))
                {
                    writer.WriteLine();
                }
            }
        }

        private static void WriteFieldInstruction(CppCodeWriter writer, TypeDefinition typeDefinition, bool needsPacking, bool explicitLayout, FieldWriteInstruction instruction, [Optional, DefaultParameterValue(false)] bool forAlignmentOnly)
        {
            int num = AlignmentPackingSizeFor(typeDefinition);
            bool flag = needsPacking || ((forAlignmentOnly && (num != -1)) && (num != 0));
            string str = !forAlignmentOnly ? string.Empty : "_forAlignmentOnly";
            if (explicitLayout)
            {
                if (flag)
                {
                    object[] args = new object[] { !forAlignmentOnly ? FieldLayoutPackingSizeFor(typeDefinition) : num };
                    writer.WriteLine("#pragma pack(push, tp, {0})", args);
                }
                writer.WriteLine("struct");
                writer.BeginBlock();
                int offset = instruction.Field.Offset;
                if (offset > 0)
                {
                    object[] objArray2 = new object[] { Naming.ForFieldPadding(instruction.Field) + str, offset };
                    writer.WriteLine("char {0}[{1}];", objArray2);
                }
            }
            if (!forAlignmentOnly)
            {
                writer.WriteCommentedLine(instruction.Field.FullName);
            }
            writer.Write(string.Format("{0} {1}", instruction.FieldTypeName, instruction.FieldName + str));
            writer.WriteLine(";");
            if (explicitLayout)
            {
                writer.EndBlock(true);
                if (flag)
                {
                    writer.WriteLine("#pragma pack(pop, tp)");
                }
            }
        }

        private static void WriteFields(CppCodeWriter writer, TypeDefinition typeDefinition, bool needsPacking, FieldType fieldType, List<FieldWriteInstruction> fieldWriteInstructions, List<ComFieldWriteInstruction> comFieldWriteInstructions)
        {
            bool explicitLayout = typeDefinition.IsExplicitLayout && (fieldType == FieldType.Instance);
            if (explicitLayout)
            {
                writer.WriteLine("union");
                writer.BeginBlock();
            }
            foreach (FieldWriteInstruction instruction in fieldWriteInstructions)
            {
                WriteFieldInstruction(writer, typeDefinition, needsPacking, explicitLayout, instruction, false);
                if (explicitLayout)
                {
                    WriteFieldInstruction(writer, typeDefinition, false, true, instruction, true);
                }
            }
            if (explicitLayout)
            {
                writer.EndBlock(true);
            }
            foreach (ComFieldWriteInstruction instruction2 in comFieldWriteInstructions)
            {
                writer.WriteCommentedLine(string.Format("Cached pointer to {0}", instruction2.InterfaceType.FullName));
                object[] args = new object[] { Naming.ForTypeNameOnly(instruction2.InterfaceType), Naming.ForComTypeInterfaceFieldName(instruction2.InterfaceType) };
                writer.WriteLine("{0}* {1};", args);
            }
        }

        private static void WriteFieldsWithAccessors(CppCodeWriter writer, TypeReference type, bool needsPacking, [Optional, DefaultParameterValue(0)] FieldType fieldType)
        {
            TypeDefinition typeDefinition = type.Resolve();
            List<FieldWriteInstruction> fieldWriteInstructions = MakeFieldWriteInstructionsForType(writer, type, typeDefinition, fieldType);
            List<ComFieldWriteInstruction> comFieldWriteInstructions = MakeComFieldWriteInstructionsForType(writer, type, typeDefinition, fieldType);
            WriteAccessSpecifier(writer, "public");
            if (fieldType == FieldType.Instance)
            {
                using (new TypeDefinitionPaddingWriter(writer, typeDefinition))
                {
                    WriteFields(writer, typeDefinition, needsPacking, fieldType, fieldWriteInstructions, comFieldWriteInstructions);
                }
            }
            else
            {
                WriteFields(writer, typeDefinition, needsPacking, fieldType, fieldWriteInstructions, comFieldWriteInstructions);
            }
            writer.WriteLine();
            WriteAccessSpecifier(writer, "public");
            WriteFieldGettersAndSetters(writer, type, fieldWriteInstructions);
            WriteComFieldGetters(writer, type, comFieldWriteInstructions);
        }

        private static void WriteNativeStructDefinitions(TypeReference type, CppCodeWriter writer)
        {
            foreach (MarshalType type2 in MarshalingUtils.GetMarshalTypesForMarshaledType(type.Resolve()))
            {
                MarshalDataCollector.MarshalInfoWriterFor(type, type2, null, false, false, false, null).WriteNativeStructDefinition(writer);
            }
        }

        public void WriteTypeDefinitionFor(TypeReference type, CppCodeWriter writer)
        {
            TypeDefinition typeDefinition = type.Resolve();
            this.CollectIncludes(writer, type, typeDefinition);
            ErrorInformation.CurrentlyProcessing.Type = typeDefinition;
            if (CodeGenOptions.EnableErrorMessageTest)
            {
                ErrorTypeAndMethod.ThrowIfIsErrorType(type.Resolve());
            }
            TypeDefinition definition2 = type as TypeDefinition;
            if ((definition2 == null) || !definition2.HasGenericParameters)
            {
                writer.WriteClangWarningDisables();
                writer.WriteLine();
                if (Extensions.IsSystemObject(type))
                {
                    writer.WriteLine("struct Il2CppClass;");
                }
                writer.WriteCommentedLine(type.FullName);
                bool flag = NeedsPackingForManaged(typeDefinition) && !typeDefinition.IsExplicitLayout;
                if (flag)
                {
                    object[] args = new object[] { FieldLayoutPackingSizeFor(typeDefinition) };
                    writer.WriteLine("#pragma pack(push, tp, {0})", args);
                }
                int num = AlignmentSizeFor(typeDefinition);
                string str = "";
                if (num != 1)
                {
                    str = "ALIGN_TYPE(" + num + ")";
                }
                if ((Extensions.IsSystemObject(type) || Extensions.IsSystemArray(type)) || Extensions.IsIl2CppComObject(type))
                {
                    writer.AddInclude("object-internals.h");
                }
                else
                {
                    object[] objArray2 = new object[] { str, Naming.ForTypeNameOnly(type), this.GetBaseTypeDeclaration(type) };
                    writer.WriteLine("struct {0} {1} {2}", objArray2);
                    writer.BeginBlock();
                    WriteCLSID(writer, typeDefinition);
                    WriteFieldsWithAccessors(writer, type, NeedsPackingForManaged(typeDefinition), FieldType.Instance);
                    writer.EndBlock(true);
                }
                if (flag)
                {
                    writer.WriteLine("#pragma pack(pop, tp)");
                }
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = new Func<FieldDefinition, bool>(null, (IntPtr) <WriteTypeDefinitionFor>m__0);
                }
                if (Enumerable.Any<FieldDefinition>(typeDefinition.Fields, <>f__am$cache0) || Extensions.StoresNonFieldsInStaticFields(typeDefinition))
                {
                    writer.WriteLine();
                    object[] objArray3 = new object[] { Naming.ForStaticFieldsStruct(type) };
                    writer.WriteLine("struct {0}", objArray3);
                    writer.BeginBlock();
                    WriteFieldsWithAccessors(writer, type, false, FieldType.Static);
                    writer.EndBlock(true);
                }
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = new Func<FieldDefinition, bool>(null, (IntPtr) <WriteTypeDefinitionFor>m__1);
                }
                if (Enumerable.Any<FieldDefinition>(typeDefinition.Fields, <>f__am$cache1))
                {
                    writer.WriteLine();
                    object[] objArray4 = new object[] { Naming.ForThreadFieldsStruct(type) };
                    writer.WriteLine("struct {0}", objArray4);
                    writer.BeginBlock();
                    WriteFieldsWithAccessors(writer, type, false, FieldType.ThreadStatic);
                    writer.EndBlock(true);
                }
                writer.WriteLine();
                writer.WriteClangWarningEnables();
                WriteNativeStructDefinitions(type, writer);
                if (Extensions.NeedsComCallableWrapper(typeDefinition))
                {
                    new CCWWriter(typeDefinition).WriteTypeDefinition(writer);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <MakeFieldWriteInstructionsForType>c__AnonStorey0
        {
            internal TypeDefinitionWriter.FieldType fieldType;

            internal bool <>m__0(FieldDefinition f)
            {
                return TypeDefinitionWriter.FieldMatches(f, this.fieldType);
            }
        }

        [StructLayout(LayoutKind.Sequential, Size=1)]
        private struct ComFieldWriteInstruction
        {
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private TypeReference <InterfaceType>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private bool <IsStatic>k__BackingField;
            public ComFieldWriteInstruction(TypeReference interfaceType, bool isStatic)
            {
                this = new TypeDefinitionWriter.ComFieldWriteInstruction();
                this.InterfaceType = interfaceType;
                this.IsStatic = isStatic;
            }

            public TypeReference InterfaceType { get; private set; }
            public bool IsStatic { get; private set; }
        }

        private enum FieldType
        {
            Instance,
            Static,
            ThreadStatic
        }

        [StructLayout(LayoutKind.Sequential, Size=1)]
        private struct FieldWriteInstruction
        {
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private FieldDefinition <Field>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private string <FieldName>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private string <FieldTypeName>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private TypeReference <FieldType>k__BackingField;
            public FieldWriteInstruction(FieldDefinition field, string fieldTypeName, TypeReference fieldType)
            {
                this = new TypeDefinitionWriter.FieldWriteInstruction();
                this.Field = field;
                this.FieldName = TypeDefinitionWriter.Naming.ForField(field);
                this.FieldTypeName = fieldTypeName;
                this.FieldType = fieldType;
            }

            public FieldDefinition Field { get; private set; }
            public string FieldName { get; private set; }
            public string FieldTypeName { get; private set; }
            public TypeReference FieldType { get; private set; }
        }

        private enum LayoutMode
        {
            Managed,
            Native
        }
    }
}

