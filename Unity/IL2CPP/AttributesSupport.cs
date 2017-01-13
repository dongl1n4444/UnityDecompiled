namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using Mono.Cecil.Rocks;
    using Mono.Collections.Generic;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Metadata;

    public class AttributesSupport
    {
        private readonly AttributeCollection _collection;
        private readonly CppCodeWriter _writer;
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<CustomAttributeNamedArgument, CustomAttributeArgument> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<CustomAttributeNamedArgument, CustomAttributeArgument> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<string, string, string> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<CustomAttributeArgument, bool> <>f__mg$cache0;
        [CompilerGenerated]
        private static Func<CustomAttributeNamedArgument, bool> <>f__mg$cache1;
        [CompilerGenerated]
        private static Func<CustomAttributeNamedArgument, bool> <>f__mg$cache2;
        [CompilerGenerated]
        private static Func<CustomAttributeArgument, bool> <>f__mg$cache3;
        [CompilerGenerated]
        private static Func<CustomAttributeNamedArgument, bool> <>f__mg$cache4;
        [CompilerGenerated]
        private static Func<CustomAttributeNamedArgument, bool> <>f__mg$cache5;
        public static IGenericSharingAnalysisService GenericSharing;
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static ITypeProviderService TypeProvider;

        public AttributesSupport(CppCodeWriter writer, AttributeCollection collection)
        {
            this._writer = writer;
            this._collection = collection;
        }

        private static string CustomAttributeConstructorFormattedArgumentsFor(CustomAttribute attribute, DefaultRuntimeMetadataAccess metadataAccess)
        {
            <CustomAttributeConstructorFormattedArgumentsFor>c__AnonStorey5 storey = new <CustomAttributeConstructorFormattedArgumentsFor>c__AnonStorey5 {
                attribute = attribute,
                metadataAccess = metadataAccess
            };
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = (buff, s) => buff + ", " + s;
            }
            return storey.attribute.ConstructorArguments.Select<CustomAttributeArgument, string>(new Func<CustomAttributeArgument, string>(storey.<>m__0)).Aggregate<string, string>("tmp", <>f__am$cache3);
        }

        private static void DeclareTempLocals(CppCodeWriter writer, CustomAttribute attribute, IRuntimeMetadataAccess metadataAccess)
        {
            DeclareTempLocalsForBoxing(writer, attribute, metadataAccess);
            DeclareTempLocalsForArrays(writer, attribute, metadataAccess);
        }

        private static void DeclareTempLocalsForArrays(CppCodeWriter writer, CustomAttribute attribute, IRuntimeMetadataAccess metadataAccess)
        {
            if (<>f__mg$cache3 == null)
            {
                <>f__mg$cache3 = new Func<CustomAttributeArgument, bool>(AttributesSupport.ValueIsArray);
            }
            foreach (CustomAttributeArgument argument in attribute.ConstructorArguments.Where<CustomAttributeArgument>(<>f__mg$cache3))
            {
                WriteStoreArrayInTempLocal(writer, TempName(argument, attribute), argument, metadataAccess);
            }
            if (<>f__mg$cache4 == null)
            {
                <>f__mg$cache4 = new Func<CustomAttributeNamedArgument, bool>(AttributesSupport.ValueIsArray);
            }
            foreach (CustomAttributeNamedArgument argument2 in attribute.Fields.Where<CustomAttributeNamedArgument>(<>f__mg$cache4))
            {
                string varName = TempName(argument2);
                WriteStoreArrayInTempLocal(writer, varName, argument2.Argument, metadataAccess);
            }
            if (<>f__mg$cache5 == null)
            {
                <>f__mg$cache5 = new Func<CustomAttributeNamedArgument, bool>(AttributesSupport.ValueIsArray);
            }
            foreach (CustomAttributeNamedArgument argument3 in attribute.Properties.Where<CustomAttributeNamedArgument>(<>f__mg$cache5))
            {
                string introduced7 = TempName(argument3);
                WriteStoreArrayInTempLocal(writer, introduced7, argument3.Argument, metadataAccess);
            }
        }

        private static void DeclareTempLocalsForBoxing(CppCodeWriter writer, CustomAttribute attribute, IRuntimeMetadataAccess metadataAccess)
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new Func<CustomAttributeArgument, bool>(AttributesSupport.ValueNeedsBoxing);
            }
            foreach (CustomAttributeArgument argument in attribute.ConstructorArguments.Where<CustomAttributeArgument>(<>f__mg$cache0))
            {
                string variableName = TempName(argument, attribute);
                WriteStoreValueInTempLocal(writer, variableName, (CustomAttributeArgument) argument.Value, metadataAccess);
            }
            if (<>f__mg$cache1 == null)
            {
                <>f__mg$cache1 = new Func<CustomAttributeNamedArgument, bool>(AttributesSupport.ValueNeedsBoxing);
            }
            foreach (CustomAttributeNamedArgument argument2 in attribute.Fields.Where<CustomAttributeNamedArgument>(<>f__mg$cache1))
            {
                string introduced9 = TempName(argument2);
                WriteStoreValueInTempLocal(writer, introduced9, (CustomAttributeArgument) argument2.Argument.Value, metadataAccess);
            }
            if (<>f__mg$cache2 == null)
            {
                <>f__mg$cache2 = new Func<CustomAttributeNamedArgument, bool>(AttributesSupport.ValueNeedsBoxing);
            }
            foreach (CustomAttributeNamedArgument argument4 in attribute.Properties.Where<CustomAttributeNamedArgument>(<>f__mg$cache2))
            {
                string introduced10 = TempName(argument4);
                WriteStoreValueInTempLocal(writer, introduced10, (CustomAttributeArgument) argument4.Argument.Value, metadataAccess);
            }
        }

        [DebuggerHidden]
        private static IEnumerable<TypeReference> ExtractTypeReferencesFromCustomAttributeArguments(IEnumerable<CustomAttributeArgument> arguments) => 
            new <ExtractTypeReferencesFromCustomAttributeArguments>c__Iterator0 { 
                arguments = arguments,
                $PC = -2
            };

        private static string FormatAttributeValue(CustomAttributeArgument argument, string tempLocalName, DefaultRuntimeMetadataAccess metadataAccess)
        {
            MetadataType metadataType = argument.Type.MetadataType;
            if ((argument.Type.MetadataType == MetadataType.Class) && argument.Type.IsEnum())
            {
                metadataType = MetadataType.ValueType;
            }
            if (argument.Type.IsEnum())
            {
                TypeReference underlyingEnumType = argument.Type.GetUnderlyingEnumType();
                return FormatAttributeValue(underlyingEnumType.MetadataType, MetadataUtils.ChangePrimitiveType(argument.Value, underlyingEnumType), tempLocalName, metadataAccess);
            }
            return FormatAttributeValue(metadataType, argument.Value, tempLocalName, metadataAccess);
        }

        private static string FormatAttributeValue(MetadataType metadataType, object argumentValue, string tempLocalName, DefaultRuntimeMetadataAccess metadataAccess)
        {
            switch (metadataType)
            {
                case MetadataType.Boolean:
                case MetadataType.SByte:
                case MetadataType.Byte:
                case MetadataType.Int16:
                case MetadataType.UInt16:
                    return argumentValue.ToString().ToLower();

                case MetadataType.Char:
                    return Formatter.FormatChar((char) argumentValue);

                case MetadataType.Int32:
                case MetadataType.UInt32:
                case MetadataType.Int64:
                case MetadataType.UInt64:
                    return (argumentValue.ToString().ToLower() + "LL");

                case MetadataType.Single:
                    return Formatter.StringRepresentationFor((float) argumentValue);

                case MetadataType.Double:
                    return Formatter.StringRepresentationFor((double) argumentValue);

                case MetadataType.String:
                    return StringWrapperFor((string) argumentValue);

                case MetadataType.Class:
                    return TypeWrapperFor((TypeReference) argumentValue, metadataAccess);

                case MetadataType.Array:
                    return tempLocalName;

                case MetadataType.Object:
                {
                    CustomAttributeArgument argument = (CustomAttributeArgument) argumentValue;
                    if (argument.Type.MetadataType != MetadataType.String)
                    {
                        if ((argument.Type.MetadataType == MetadataType.Class) && (argument.Value is TypeReference))
                        {
                            return TypeWrapperFor((TypeReference) argument.Value, metadataAccess);
                        }
                        return Emit.Box(argument.Type, tempLocalName, metadataAccess);
                    }
                    return StringWrapperFor((string) argument.Value);
                }
            }
            throw new NotSupportedException("Unsupported constructor argument metadata type: " + metadataType);
        }

        private static string FormatForAssignment(object value, IRuntimeMetadataAccess metadataAccess)
        {
            if (value == null)
            {
                return Naming.Null;
            }
            if (value is bool)
            {
                return value.ToString().ToLower();
            }
            if (value is byte)
            {
                return (value.ToString().ToLower() + "U");
            }
            if (value is sbyte)
            {
                return value.ToString().ToLower();
            }
            if (value is char)
            {
                return ("'" + value + "'");
            }
            if (value is short)
            {
                return value.ToString().ToLower();
            }
            if (value is ushort)
            {
                return (value.ToString().ToLower() + "U");
            }
            if (value is int)
            {
                return value.ToString().ToLower();
            }
            if (value is uint)
            {
                return (value.ToString().ToLower() + "U");
            }
            if (value is long)
            {
                return value.ToString().ToLower();
            }
            if (value is ulong)
            {
                return (value.ToString().ToLower() + "U");
            }
            if (value is float)
            {
                return Formatter.StringRepresentationFor((float) value);
            }
            if (value is double)
            {
                return Formatter.StringRepresentationFor((double) value);
            }
            if (value is string)
            {
                return StringWrapperFor((string) value);
            }
            if (!(value is TypeReference))
            {
                throw new ArgumentException("Unsupported CustomAttribute value of type " + value.GetType().FullName);
            }
            return TypeWrapperFor((TypeReference) value, metadataAccess);
        }

        private static List<FieldDefinition> GatherFieldsFromTypeAndBaseTypes(TypeDefinition attributeType)
        {
            List<FieldDefinition> list = new List<FieldDefinition>();
            for (TypeDefinition definition = attributeType; definition != null; definition = definition.BaseType?.Resolve())
            {
                list.AddRange(definition.Fields);
            }
            return list;
        }

        private static List<PropertyDefinition> GatherPropertiesFromTypeAndBaseTypes(TypeDefinition attributeType)
        {
            List<PropertyDefinition> list = new List<PropertyDefinition>();
            for (TypeDefinition definition = attributeType; definition != null; definition = definition.BaseType?.Resolve())
            {
                list.AddRange(definition.Properties);
            }
            return list;
        }

        private static string StorageTypeFor(object value)
        {
            if (value is bool)
            {
                return "bool";
            }
            if (value is byte)
            {
                return "uint8_t";
            }
            if (value is sbyte)
            {
                return "int8_t";
            }
            if (value is char)
            {
                return "Il2CppChar";
            }
            if (value is short)
            {
                return "int16_t";
            }
            if (value is ushort)
            {
                return "uint16_t";
            }
            if (value is int)
            {
                return "int32_t";
            }
            if (value is uint)
            {
                return "uint32_t";
            }
            if (value is long)
            {
                return "int64_t";
            }
            if (value is ulong)
            {
                return "uint64_t";
            }
            if (value is float)
            {
                return "float";
            }
            if (value is double)
            {
                return "double";
            }
            if (value is TypeReference)
            {
                return "Il2CppCodeGenType*";
            }
            if (value is Array)
            {
                throw new NotSupportedException("IL2CPP does not support attributes with arguments that are arrays of arrays.");
            }
            throw new ArgumentException("Unsupported CustomAttribute value of type " + value.GetType().FullName);
        }

        private static string StringWrapperFor(string value)
        {
            if (value == null)
            {
                return Naming.Null;
            }
            if ((value.Contains("Microsoft") && value.Contains("Visual")) && value.Contains("\0ae"))
            {
                value = value.Replace("\0ae", "\x00ae");
            }
            return $"il2cpp_codegen_string_new_wrapper("{Formatter.EscapeString(value)}")";
        }

        private static string TempName(CustomAttributeNamedArgument argument) => 
            ("_tmp_" + argument.Name);

        private static string TempName(CustomAttributeArgument argument, CustomAttribute attribute) => 
            ("_tmp_" + attribute.ConstructorArguments.IndexOf(argument));

        private static string TypeWrapperFor(TypeReference typeReference, IRuntimeMetadataAccess metadataAccess)
        {
            if (typeReference == null)
            {
                return Naming.Null;
            }
            return $"il2cpp_codegen_type_get_object({metadataAccess.Il2CppTypeFor(typeReference)})";
        }

        private static bool ValueIsArray(CustomAttributeArgument argument) => 
            (argument.Type.MetadataType == MetadataType.Array);

        private static bool ValueIsArray(CustomAttributeNamedArgument namedArgument) => 
            ValueIsArray(namedArgument.Argument);

        private static bool ValueNeedsBoxing(CustomAttributeArgument argument)
        {
            if (argument.Type.MetadataType != MetadataType.Object)
            {
                return false;
            }
            CustomAttributeArgument argument2 = (CustomAttributeArgument) argument.Value;
            if (argument2.Type.MetadataType == MetadataType.String)
            {
                return false;
            }
            if ((argument2.Type.MetadataType == MetadataType.Class) && (argument2.Value is TypeReference))
            {
                return false;
            }
            return true;
        }

        private static bool ValueNeedsBoxing(CustomAttributeNamedArgument namedArgument) => 
            ValueNeedsBoxing(namedArgument.Argument);

        public TableInfo WriteAttributes(IEnumerable<AssemblyDefinition> assemblyDefinitions)
        {
            foreach (AssemblyDefinition definition in assemblyDefinitions)
            {
                this.WriteCustomAttributesCacheGeneratorFor(definition);
                foreach (TypeDefinition definition2 in definition.MainModule.GetAllTypes())
                {
                    this.WriteCustomAttributesCacheGenerators(definition2);
                }
            }
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = a => a;
            }
            return MetadataWriter.WriteTable<string>(this._writer, "extern const CustomAttributesCacheGenerator", "g_AttributeGenerators", this._collection.GetEntries(), <>f__am$cache0);
        }

        private void WriteCustomAttributesCacheGeneratorFor(AssemblyDefinition assemblyDefinition)
        {
            this.WriteCustomAttributesCacheGeneratorFor(Naming.ForCustomAttributesCacheGenerator(assemblyDefinition), assemblyDefinition);
        }

        private void WriteCustomAttributesCacheGeneratorFor(EventDefinition eventDefinition)
        {
            this.WriteCustomAttributesCacheGeneratorFor(Naming.ForCustomAttributesCacheGenerator(eventDefinition), eventDefinition);
        }

        private void WriteCustomAttributesCacheGeneratorFor(FieldDefinition fieldDefinition)
        {
            this.WriteCustomAttributesCacheGeneratorFor(Naming.ForCustomAttributesCacheGenerator(fieldDefinition), fieldDefinition);
        }

        private void WriteCustomAttributesCacheGeneratorFor(MethodDefinition methodDefinition)
        {
            this.WriteCustomAttributesCacheGeneratorFor(Naming.ForCustomAttributesCacheGenerator(methodDefinition), methodDefinition);
        }

        private void WriteCustomAttributesCacheGeneratorFor(PropertyDefinition propertyDefinition)
        {
            this.WriteCustomAttributesCacheGeneratorFor(Naming.ForCustomAttributesCacheGenerator(propertyDefinition), propertyDefinition);
        }

        private void WriteCustomAttributesCacheGeneratorFor(TypeDefinition typeDefinition)
        {
            this.WriteCustomAttributesCacheGeneratorFor(Naming.ForCustomAttributesCacheGenerator(typeDefinition), typeDefinition);
        }

        private void WriteCustomAttributesCacheGeneratorFor(ParameterDefinition parameterDefinition, MethodDefinition methodDefinition)
        {
            this.WriteCustomAttributesCacheGeneratorFor(Naming.ForCustomAttributesCacheGenerator(parameterDefinition, methodDefinition), parameterDefinition);
        }

        private void WriteCustomAttributesCacheGeneratorFor(string name, ICustomAttributeProvider customAttributeProvider)
        {
            <WriteCustomAttributesCacheGeneratorFor>c__AnonStorey2 storey = new <WriteCustomAttributesCacheGeneratorFor>c__AnonStorey2 {
                constructibleCustomAttributes = customAttributeProvider.GetConstructibleCustomAttributes().ToArray<CustomAttribute>()
            };
            if (storey.constructibleCustomAttributes.Length != 0)
            {
                this._collection.Add(name, storey.constructibleCustomAttributes);
                foreach (CustomAttribute attribute in storey.constructibleCustomAttributes)
                {
                    this._writer.AddIncludeForTypeDefinition(attribute.AttributeType);
                    this._writer.AddIncludeForMethodDeclarations(attribute.AttributeType);
                    foreach (TypeReference reference in ExtractTypeReferencesFromCustomAttributeArguments(attribute.ConstructorArguments))
                    {
                        if (reference != null)
                        {
                            this._writer.AddIncludeForTypeDefinition(reference);
                        }
                    }
                    if (<>f__am$cache1 == null)
                    {
                        <>f__am$cache1 = f => f.Argument;
                    }
                    foreach (TypeReference reference2 in ExtractTypeReferencesFromCustomAttributeArguments(attribute.Fields.Select<CustomAttributeNamedArgument, CustomAttributeArgument>(<>f__am$cache1)))
                    {
                        if (reference2 != null)
                        {
                            this._writer.AddIncludeForTypeDefinition(reference2);
                        }
                    }
                    if (<>f__am$cache2 == null)
                    {
                        <>f__am$cache2 = p => p.Argument;
                    }
                    foreach (TypeReference reference3 in ExtractTypeReferencesFromCustomAttributeArguments(attribute.Properties.Select<CustomAttributeNamedArgument, CustomAttributeArgument>(<>f__am$cache2)))
                    {
                        if (reference3 != null)
                        {
                            this._writer.AddIncludeForTypeDefinition(reference3);
                        }
                    }
                    List<PropertyDefinition> source = GatherPropertiesFromTypeAndBaseTypes(attribute.AttributeType.Resolve());
                    using (Collection<CustomAttributeNamedArgument>.Enumerator enumerator4 = attribute.Properties.GetEnumerator())
                    {
                        while (enumerator4.MoveNext())
                        {
                            <WriteCustomAttributesCacheGeneratorFor>c__AnonStorey1 storey2 = new <WriteCustomAttributesCacheGeneratorFor>c__AnonStorey1 {
                                property = enumerator4.Current
                            };
                            MethodDefinition setMethod = source.First<PropertyDefinition>(new Func<PropertyDefinition, bool>(storey2.<>m__0)).SetMethod;
                            this._writer.AddIncludeForMethodDeclarations(setMethod.DeclaringType);
                        }
                    }
                }
                MethodWriter.WriteMethodWithMetadataInitialization(this._writer, $"static void {name}(CustomAttributesCache* cache)", name, new Action<CppCodeWriter, MetadataUsage, MethodUsage>(storey.<>m__0), name);
            }
        }

        private void WriteCustomAttributesCacheGenerators(TypeDefinition type)
        {
            this.WriteCustomAttributesCacheGeneratorFor(type);
            foreach (FieldDefinition definition in type.Fields)
            {
                this.WriteCustomAttributesCacheGeneratorFor(definition);
            }
            foreach (MethodDefinition definition2 in type.Methods)
            {
                this.WriteCustomAttributesCacheGeneratorFor(definition2);
                foreach (ParameterDefinition definition3 in definition2.Parameters)
                {
                    this.WriteCustomAttributesCacheGeneratorFor(definition3, definition2);
                }
            }
            foreach (PropertyDefinition definition4 in type.Properties)
            {
                this.WriteCustomAttributesCacheGeneratorFor(definition4);
            }
            foreach (EventDefinition definition5 in type.Events)
            {
                this.WriteCustomAttributesCacheGeneratorFor(definition5);
            }
        }

        private static void WriteInitializeArray(CppCodeWriter writer, string varName, TypeReference elementType, IEnumerable<CustomAttributeArgument> arguments, IRuntimeMetadataAccess metadataAccess)
        {
            int num = 0;
            foreach (CustomAttributeArgument argument in arguments)
            {
                WriteStoreArrayElement(writer, varName, elementType, num++, FormatForAssignment(argument.Value, metadataAccess));
            }
        }

        private static void WriteInitializeObjectArray(CppCodeWriter writer, string varName, TypeReference elementType, IEnumerable<CustomAttributeArgument> arguments, IRuntimeMetadataAccess metadataAccess)
        {
            int index = 0;
            foreach (CustomAttributeArgument argument in arguments)
            {
                CustomAttributeArgument argument2 = argument;
                if (argument2.Value is CustomAttributeArgument)
                {
                    argument2 = (CustomAttributeArgument) argument2.Value;
                }
                if (argument2.Type.MetadataType == MetadataType.String)
                {
                    WriteStoreArrayElement(writer, varName, elementType, index, FormatForAssignment(argument2.Value, metadataAccess));
                }
                else
                {
                    string variableName = varName + "_" + index;
                    WriteStoreValueInTempLocal(writer, variableName, argument2, metadataAccess);
                    WriteStoreArrayElement(writer, varName, elementType, index, Emit.Box(argument2.Type, variableName, metadataAccess));
                }
                index++;
            }
        }

        private static void WriteMethodBody(CppCodeWriter writer, IEnumerable<CustomAttribute> customAttributes, DefaultRuntimeMetadataAccess metadataAccess)
        {
            int num = 0;
            foreach (CustomAttribute attribute in customAttributes)
            {
                writer.BeginBlock();
                DeclareTempLocals(writer, attribute, metadataAccess);
                TypeDefinition variableType = attribute.AttributeType.Resolve();
                object[] args = new object[] { Naming.ForVariable(variableType), num };
                writer.WriteLine("{0} tmp = ({0})cache->attributes[{1}];", args);
                object[] objArray2 = new object[] { Naming.ForMethodNameOnly(attribute.Constructor), CustomAttributeConstructorFormattedArgumentsFor(attribute, metadataAccess), metadataAccess.HiddenMethodInfo(attribute.Constructor) };
                writer.WriteLine("{0}({1}, {2});", objArray2);
                List<FieldDefinition> source = GatherFieldsFromTypeAndBaseTypes(variableType);
                using (Collection<CustomAttributeNamedArgument>.Enumerator enumerator2 = attribute.Fields.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        <WriteMethodBody>c__AnonStorey3 storey = new <WriteMethodBody>c__AnonStorey3 {
                            fieldArgument = enumerator2.Current
                        };
                        object[] objArray3 = new object[] { Naming.ForFieldSetter(source.First<FieldDefinition>(new Func<FieldDefinition, bool>(storey.<>m__0))), FormatAttributeValue(storey.fieldArgument.Argument, TempName(storey.fieldArgument), metadataAccess) };
                        writer.WriteLine("tmp->{0}({1});", objArray3);
                    }
                }
                List<PropertyDefinition> list2 = GatherPropertiesFromTypeAndBaseTypes(variableType);
                using (Collection<CustomAttributeNamedArgument>.Enumerator enumerator3 = attribute.Properties.GetEnumerator())
                {
                    while (enumerator3.MoveNext())
                    {
                        <WriteMethodBody>c__AnonStorey4 storey2 = new <WriteMethodBody>c__AnonStorey4 {
                            propertyArgument = enumerator3.Current
                        };
                        MethodDefinition setMethod = list2.First<PropertyDefinition>(new Func<PropertyDefinition, bool>(storey2.<>m__0)).SetMethod;
                        object[] objArray4 = new object[] { Naming.ForMethodNameOnly(setMethod), FormatAttributeValue(storey2.propertyArgument.Argument, TempName(storey2.propertyArgument), metadataAccess), metadataAccess.HiddenMethodInfo(setMethod) };
                        writer.WriteLine("{0}(tmp, {1}, {2});", objArray4);
                    }
                }
                writer.EndBlock(false);
                num++;
            }
        }

        private static void WriteStoreArrayElement(CppCodeWriter writer, string varName, TypeReference elementType, int index, string value)
        {
            object[] args = new object[] { Emit.StoreArrayElement(varName, index.ToString(), value, false) };
            writer.WriteLine("{0};", args);
        }

        private static void WriteStoreArrayInTempLocal(CppCodeWriter writer, string varName, CustomAttributeArgument attributeValue, IRuntimeMetadataAccess metadataAccess)
        {
            CustomAttributeArgument[] arguments = (CustomAttributeArgument[]) attributeValue.Value;
            ArrayType variableType = (ArrayType) attributeValue.Type;
            if (arguments == null)
            {
                writer.WriteLine(Statement.Expression(Emit.Assign($"{Naming.ForVariable(variableType)} {varName}", "NULL")));
            }
            else
            {
                writer.WriteLine(Statement.Expression(Emit.Assign($"{Naming.ForVariable(variableType)} {varName}", Emit.NewSZArray(variableType, arguments.Length, metadataAccess))));
                TypeReference elementType = variableType.ElementType;
                if (elementType.MetadataType == MetadataType.Object)
                {
                    WriteInitializeObjectArray(writer, varName, elementType, arguments, metadataAccess);
                }
                else
                {
                    WriteInitializeArray(writer, varName, elementType, arguments, metadataAccess);
                }
            }
        }

        private static void WriteStoreValueInTempLocal(CppCodeWriter writer, string variableName, CustomAttributeArgument argument, IRuntimeMetadataAccess metadataAccess)
        {
            object[] args = new object[] { StorageTypeFor(argument.Value), variableName, FormatForAssignment(argument.Value, metadataAccess) };
            writer.WriteLine("{0} {1} = {2};", args);
        }

        [CompilerGenerated]
        private sealed class <CustomAttributeConstructorFormattedArgumentsFor>c__AnonStorey5
        {
            internal CustomAttribute attribute;
            internal DefaultRuntimeMetadataAccess metadataAccess;

            internal string <>m__0(CustomAttributeArgument a) => 
                AttributesSupport.FormatAttributeValue(a, AttributesSupport.TempName(a, this.attribute), this.metadataAccess);
        }

        [CompilerGenerated]
        private sealed class <ExtractTypeReferencesFromCustomAttributeArguments>c__Iterator0 : IEnumerable, IEnumerable<TypeReference>, IEnumerator, IDisposable, IEnumerator<TypeReference>
        {
            internal TypeReference $current;
            internal bool $disposing;
            internal IEnumerator<CustomAttributeArgument> $locvar0;
            internal IEnumerator<TypeReference> $locvar1;
            internal int $PC;
            internal CustomAttributeArgument <argument>__0;
            internal MetadataType <metadataType>__1;
            internal TypeReference <val>__2;
            internal CustomAttributeArgument <value>__3;
            internal IEnumerable<CustomAttributeArgument> arguments;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        try
                        {
                            switch (num)
                            {
                                case 3:
                                    try
                                    {
                                    }
                                    finally
                                    {
                                        if (this.$locvar1 != null)
                                        {
                                            this.$locvar1.Dispose();
                                        }
                                    }
                                    return;
                            }
                        }
                        finally
                        {
                            if (this.$locvar0 != null)
                            {
                                this.$locvar0.Dispose();
                            }
                        }
                        break;
                }
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                        if (this.arguments != null)
                        {
                            this.$locvar0 = this.arguments.GetEnumerator();
                            num = 0xfffffffd;
                            break;
                        }
                        goto Label_02A7;

                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        break;

                    default:
                        goto Label_02A7;
                }
                try
                {
                    switch (num)
                    {
                        case 1:
                            goto Label_0121;

                        case 2:
                            this.$locvar1 = AttributesSupport.ExtractTypeReferencesFromCustomAttributeArguments((IEnumerable<CustomAttributeArgument>) this.<argument>__0.Value).GetEnumerator();
                            num = 0xfffffffd;
                            goto Label_0176;

                        case 3:
                            goto Label_0176;

                        case 4:
                            goto Label_0243;

                        case 5:
                            goto Label_026F;
                    }
                    while (this.$locvar0.MoveNext())
                    {
                        this.<argument>__0 = this.$locvar0.Current;
                        this.<metadataType>__1 = this.<argument>__0.Type.MetadataType;
                        if ((this.<argument>__0.Type.MetadataType == MetadataType.Class) && this.<argument>__0.Type.IsEnum())
                        {
                            this.<metadataType>__1 = MetadataType.ValueType;
                        }
                        if (this.<metadataType>__1 != MetadataType.Array)
                        {
                            if (this.<metadataType>__1 == MetadataType.Object)
                            {
                                goto Label_01EF;
                            }
                            continue;
                        }
                        this.$current = (ArrayType) this.<argument>__0.Type;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        flag = true;
                        goto Label_02A9;
                    Label_0121:
                        this.$current = ((ArrayType) this.<argument>__0.Type).ElementType;
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        flag = true;
                        goto Label_02A9;
                    Label_0176:
                        try
                        {
                            while (this.$locvar1.MoveNext())
                            {
                                this.<val>__2 = this.$locvar1.Current;
                                this.$current = this.<val>__2;
                                if (!this.$disposing)
                                {
                                    this.$PC = 3;
                                }
                                flag = true;
                                goto Label_02A9;
                            }
                        }
                        finally
                        {
                            if (!flag)
                            {
                            }
                            if (this.$locvar1 != null)
                            {
                                this.$locvar1.Dispose();
                            }
                        }
                        continue;
                    Label_01EF:
                        this.<value>__3 = (CustomAttributeArgument) this.<argument>__0.Value;
                        if (this.<value>__3.Type.MetadataType == MetadataType.String)
                        {
                            this.$current = this.<value>__3.Type;
                            if (!this.$disposing)
                            {
                                this.$PC = 4;
                            }
                            flag = true;
                            goto Label_02A9;
                        }
                    Label_0243:
                        this.$current = this.<value>__3.Type;
                        if (!this.$disposing)
                        {
                            this.$PC = 5;
                        }
                        flag = true;
                        goto Label_02A9;
                    Label_026F:;
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    if (this.$locvar0 != null)
                    {
                        this.$locvar0.Dispose();
                    }
                }
                this.$PC = -1;
            Label_02A7:
                return false;
            Label_02A9:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<TypeReference> IEnumerable<TypeReference>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new AttributesSupport.<ExtractTypeReferencesFromCustomAttributeArguments>c__Iterator0 { arguments = this.arguments };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<Mono.Cecil.TypeReference>.GetEnumerator();

            TypeReference IEnumerator<TypeReference>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        [CompilerGenerated]
        private sealed class <WriteCustomAttributesCacheGeneratorFor>c__AnonStorey1
        {
            internal CustomAttributeNamedArgument property;

            internal bool <>m__0(PropertyDefinition p) => 
                (p.Name == this.property.Name);
        }

        [CompilerGenerated]
        private sealed class <WriteCustomAttributesCacheGeneratorFor>c__AnonStorey2
        {
            internal CustomAttribute[] constructibleCustomAttributes;

            internal void <>m__0(CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage)
            {
                AttributesSupport.WriteMethodBody(bodyWriter, this.constructibleCustomAttributes, new DefaultRuntimeMetadataAccess(null, metadataUsage, methodUsage));
            }
        }

        [CompilerGenerated]
        private sealed class <WriteMethodBody>c__AnonStorey3
        {
            internal CustomAttributeNamedArgument fieldArgument;

            internal bool <>m__0(FieldDefinition p) => 
                (p.Name == this.fieldArgument.Name);
        }

        [CompilerGenerated]
        private sealed class <WriteMethodBody>c__AnonStorey4
        {
            internal CustomAttributeNamedArgument propertyArgument;

            internal bool <>m__0(PropertyDefinition p) => 
                (p.Name == this.propertyArgument.Name);
        }
    }
}

