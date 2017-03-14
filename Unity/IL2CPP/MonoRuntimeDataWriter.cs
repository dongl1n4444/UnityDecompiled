namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using NiceIO;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Metadata;

    public class MonoRuntimeDataWriter
    {
        [CompilerGenerated]
        private static Func<TypeReference, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<KeyValuePair<MethodReference, uint>, MethodReference> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<MethodReference, bool> <>f__am$cache2;
        [Inject]
        public static IGenericSharingAnalysisService GenericSharingAnalysis;
        [Inject]
        public static IIl2CppGenericMethodCollectorReaderService Il2CppGenericMethodCollector;
        [Inject]
        public static IIl2CppTypeCollectorWriterService Il2CppTypeCollector;
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static IRuntimeInvokerCollectorReaderService RuntimeInvokerCollectorReader;

        private static void AddMethodHash(MethodReference sourceMethod, MethodReference actualMethod, MethodTables methodTables, Dictionary<ulong, MethodReference> allHashes, List<string> entries)
        {
            int num;
            methodTables.MethodPointers.TryGetValue(MethodTables.MethodPointerFor(actualMethod), out num);
            List<MethodHashComponent> typeHashes = GetTypeHashes(sourceMethod);
            ulong key = CombineAllHashes(typeHashes);
            if (allHashes.ContainsKey(key))
            {
                MethodReference x = allHashes[key];
                if (!Unity.IL2CPP.Common.MethodReferenceComparer.AreEqual(x, sourceMethod))
                {
                    throw new Exception($"Hash collision detected in GenericMethodInfoMapContainer - existing method: '{x.FullName}' new method: '{sourceMethod.FullName}'");
                }
            }
            else
            {
                allHashes.Add(key, sourceMethod);
            }
            entries.Add($"		m_map.add(0x{key:X8} /* {FormatForMonoReflectionMatching(sourceMethod)} - {FormatHashList(typeHashes)} */, MonoMethodInfo({RuntimeInvokerCollectorReader.GetIndex(actualMethod)}, {num}));");
        }

        private static ulong CombineAllHashes(MethodReference method) => 
            CombineAllHashes(GetTypeHashes(method));

        private static ulong CombineAllHashes(IEnumerable<MethodHashComponent> hashes)
        {
            ulong num = 0L;
            foreach (MethodHashComponent component in hashes)
            {
                if (num == 0L)
                {
                    num = component.Value;
                }
                else
                {
                    num = CombineHashes(num, (ulong) component.Value);
                }
            }
            return num;
        }

        private static ulong CombineHashes(ulong hash1, ulong hash2) => 
            ((hash1 * ((ulong) 0x1cfaa2dbL)) + hash2);

        private static string FormatForMonoReflectionMatching(MethodReference method) => 
            method.FullName.Substring(method.FullName.IndexOf(" ") + 1).Replace("(", " (").Replace("::", ":").Replace("System.Char", "char").Replace("System.Bool", "bool").Replace("System.Byte", "byte").Replace("System.SByte", "sbyte").Replace("System.UInt16", "uint16").Replace("System.Int16", "int16").Replace("System.UInt32", "uint").Replace("System.Int32", "int").Replace("System.UInt64", "ulong").Replace("System.Int64", "long").Replace("System.UIntPtr", "uintptr").Replace("System.IntPtr", "intptr").Replace("System.Single", "single").Replace("System.Double", "double").Replace("System.Object", "object");

        private static string FormatHashList(IEnumerable<MethodHashComponent> hashes)
        {
            StringBuilder builder = new StringBuilder();
            foreach (MethodHashComponent component in hashes)
            {
                builder.AppendFormat("0x{0:X8} ({1}), ", component.Value, component.Source);
            }
            return builder.ToString();
        }

        private static void GetArrayStructureHash(ArrayType type, List<MethodHashComponent> hashes)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append('[');
            int num = 0;
            foreach (ArrayDimension dimension in type.Dimensions)
            {
                if (dimension.IsSized)
                {
                    if (dimension.LowerBound.HasValue && (dimension.LowerBound != 0))
                    {
                        builder.AppendFormat("{0}:", dimension.LowerBound);
                    }
                    builder.Append(dimension.UpperBound);
                }
                if (num < (type.Dimensions.Count - 1))
                {
                    builder.Append(',');
                }
                num++;
            }
            builder.Append(']');
            hashes.Add(new MethodHashComponent(builder.ToString()));
        }

        private static List<MethodHashComponent> GetTypeHashes(MethodReference method)
        {
            List<MethodHashComponent> hashes = new List<MethodHashComponent>();
            MethodDefinition definition = method.Resolve();
            hashes.Add(new MethodHashComponent(definition));
            hashes.Add(new MethodHashComponent(definition.Module.Name));
            GenericInstanceType declaringType = method.DeclaringType as GenericInstanceType;
            if ((declaringType != null) && declaringType.HasGenericArguments)
            {
                hashes.Add(new MethodHashComponent(definition.DeclaringType));
                foreach (TypeReference reference in declaringType.GenericArguments)
                {
                    GetTypeHashes(reference, hashes);
                }
            }
            GenericInstanceMethod method2 = method as GenericInstanceMethod;
            if (method2 != null)
            {
                foreach (TypeReference reference2 in method2.GenericArguments)
                {
                    GetTypeHashes(reference2, hashes);
                }
            }
            return hashes;
        }

        private static void GetTypeHashes(TypeReference type, List<MethodHashComponent> hashes)
        {
            TypeDefinition definition = type.Resolve();
            if (!type.IsGenericInstance)
            {
                if (type.IsArray)
                {
                    hashes.Add(new MethodHashComponent(type));
                    GetArrayStructureHash(type as ArrayType, hashes);
                    GetTypeHashes((type as ArrayType).ElementType, hashes);
                }
                else
                {
                    hashes.Add(new MethodHashComponent(definition));
                    hashes.Add(new MethodHashComponent(definition.Module.Name));
                }
            }
            else
            {
                hashes.Add(new MethodHashComponent(definition));
                hashes.Add(new MethodHashComponent(definition.Module.Name));
                GenericInstanceType type2 = type as GenericInstanceType;
                foreach (TypeReference reference in type2.GenericArguments)
                {
                    GetTypeHashes(reference, hashes);
                }
            }
        }

        private static uint HashStringDjb2(string value)
        {
            uint num = 0x1505;
            foreach (char ch in value)
            {
                num = ((num << 5) + num) + ch;
            }
            return num;
        }

        private static Dictionary<string, List<MethodReference>> ImagesToMethodDictionaryFor(IEnumerable<MethodReference> methods)
        {
            Dictionary<string, List<MethodReference>> dictionary = new Dictionary<string, List<MethodReference>>();
            foreach (MethodReference reference in methods)
            {
                string name = reference.DeclaringType.Module.Assembly.Name.Name;
                if (!dictionary.ContainsKey(name))
                {
                    dictionary.Add(name, new List<MethodReference>());
                }
                dictionary[name].Add(reference);
            }
            return dictionary;
        }

        private static Dictionary<string, List<TypeReference>> ImagesToTypeDictionaryFor(IEnumerable<TypeReference> types)
        {
            Dictionary<string, List<TypeReference>> dictionary = new Dictionary<string, List<TypeReference>>();
            foreach (TypeReference reference in types)
            {
                string name = reference.Module.Assembly.Name.Name;
                if (!dictionary.ContainsKey(name))
                {
                    dictionary.Add(name, new List<TypeReference>());
                }
                dictionary[name].Add(reference);
            }
            return dictionary;
        }

        private static string IntToIl2CppRGCTXData(Il2CppRGCTXDataType value)
        {
            if (value == Il2CppRGCTXDataType.Type)
            {
                return "IL2CPP_RGCTX_DATA_TYPE";
            }
            if (value == Il2CppRGCTXDataType.Class)
            {
                return "IL2CPP_RGCTX_DATA_CLASS";
            }
            if (value == Il2CppRGCTXDataType.Method)
            {
                return "IL2CPP_RGCTX_DATA_METHOD";
            }
            if (value == Il2CppRGCTXDataType.Array)
            {
                return "IL2CPP_RGCTX_DATA_ARRAY";
            }
            return "IL2CPP_RGCTX_DATA_INVALID";
        }

        private static string NameForGenericInstArgsArray(GenericInstanceType inst) => 
            $"argTypes_Mono{Naming.ForGenericInst(inst.GenericArguments)}";

        private static int NumberOfMethodsInImage(Dictionary<string, List<MethodReference>> imagesWithMethods, string imageName)
        {
            List<MethodReference> list = null;
            if (imagesWithMethods.ContainsKey(imageName))
            {
                list = imagesWithMethods[imageName];
            }
            return ((list == null) ? 0 : list.Count);
        }

        private static string RGCTXArrayNameFor(string imageName) => 
            $"s_runtimeGenericContextMapping_{SanitizeForCppIndentifier(imageName)}";

        private static string SanitizeForCppIndentifier(string name) => 
            name.Replace('.', '_');

        private static void WriteGenericArgumentIndices(SourceCodeWriter writer, GenericInstanceType inst, HashSet<string> typeIndexArrays)
        {
            if (inst.HasGenericArguments)
            {
                string item = NameForGenericInstArgsArray(inst);
                if (!typeIndexArrays.Contains(item))
                {
                    typeIndexArrays.Add(item);
                    object[] args = new object[3];
                    args[0] = item;
                    args[1] = inst.GenericArguments.Count;
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = arg => Il2CppTypeCollector.GetIndex(arg, 0).ToString();
                    }
                    args[2] = inst.GenericArguments.Select<TypeReference, string>(<>f__am$cache0).AggregateWithComma();
                    writer.WriteLine("static TypeIndex {0}[{1}] = {{ {2} }};", args);
                }
            }
        }

        public static void WriteGenericMethodIndexTable(NPath outputDir, IMetadataCollection metadataCollector, MethodTables methodTables)
        {
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = item => item.Key;
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = m => (!m.HasGenericParameters && !m.DeclaringType.HasGenericParameters) && !m.ContainsGenericParameters();
            }
            IEnumerable<MethodReference> source = Il2CppGenericMethodCollector.Items.Select<KeyValuePair<MethodReference, uint>, MethodReference>(<>f__am$cache1).Where<MethodReference>(<>f__am$cache2);
            if (source.Any<MethodReference>())
            {
                string[] append = new string[] { "Il2CppGenericMethodIndexTable.cpp" };
                using (SourceCodeWriter writer = new SourceCodeWriter(outputDir.Combine(append)))
                {
                    writer.AddCodeGenIncludes();
                    writer.WriteLine("MethodIndex g_GenericMethodIndices[] = ");
                    writer.WriteLine("{");
                    writer.WriteLine("0,");
                    int num = 0;
                    foreach (MethodReference reference in source)
                    {
                        if (methodTables.IsMethodReferenceUsed(reference))
                        {
                            object[] args = new object[] { Il2CppGenericMethodCollector.GetIndex(reference) };
                            writer.WriteLine("{0},", args);
                        }
                        num++;
                    }
                    writer.WriteLine("};");
                    writer.WriteLine();
                }
            }
        }

        public static void WriteGenericMethodInfoMapping(NPath outputDir, MethodTables methodTables)
        {
            MethodReference[] referenceArray = MetadataCacheWriter.GenericMethodTableEntries(Il2CppGenericMethodCollector.Items.Keys);
            Dictionary<ulong, MethodReference> allHashes = new Dictionary<ulong, MethodReference>();
            string[] append = new string[] { "Il2CppGenericMethodInfoMapping.cpp" };
            using (SourceCodeWriter writer = new SourceCodeWriter(outputDir.Combine(append)))
            {
                writer.AddInclude("il2cpp-mapping.h");
                List<string> entries = new List<string>();
                foreach (MethodReference reference in referenceArray)
                {
                    IEnumerable<MethodReference> methodsSharedFrom = GenericSharingAnalysis.GetMethodsSharedFrom(reference);
                    foreach (MethodReference reference2 in methodsSharedFrom)
                    {
                        AddMethodHash(reference2, reference, methodTables, allHashes, entries);
                    }
                    if (!methodsSharedFrom.Any<MethodReference>())
                    {
                        AddMethodHash(reference, reference, methodTables, allHashes, entries);
                    }
                }
                writer.WriteLine("class GenericMethodInfoMapContainer : public MonoMethodInfoMapContainerBase");
                writer.WriteLine("{");
                writer.WriteLine("private:");
                int num2 = 0;
                foreach (List<string> list2 in entries.Chunk<string>(0x2710))
                {
                    object[] args = new object[] { num2 };
                    writer.WriteLine("\tvoid initializeMap{0}()", args);
                    writer.WriteLine("\t{");
                    foreach (string str in list2)
                    {
                        writer.WriteLine(str);
                    }
                    writer.WriteLine("\t}");
                    writer.WriteLine();
                    num2++;
                }
                writer.WriteLine("public:");
                writer.WriteLine("\tGenericMethodInfoMapContainer()");
                writer.WriteLine("\t{");
                for (int i = 0; i < num2; i++)
                {
                    object[] objArray2 = new object[] { i };
                    writer.WriteLine("\t\tinitializeMap{0}();", objArray2);
                }
                writer.WriteLine("\t}");
                writer.WriteLine("};");
                writer.WriteLine();
                writer.WriteLine("static const GenericMethodInfoMapContainer s_genericMethodsMapContainer;");
                writer.WriteLine("const MonoMethodInfoMapContainerBase* g_pGenericMethodsMapContainer = &s_genericMethodsMapContainer;");
                writer.WriteLine();
            }
        }

        public static void WriteMetadataUsages(NPath outputDir, List<KeyValuePair<uint, uint>> usagePairs, List<KeyValuePair<uint, uint>> usageLists)
        {
            string[] append = new string[] { "Il2CppMonoMetadataUsages.cpp" };
            using (SourceCodeWriter writer = new SourceCodeWriter(outputDir.Combine(append)))
            {
                writer.AddInclude("il2cpp-metadata.h");
                object[] args = new object[] { usageLists.Count };
                writer.WriteLine("extern const int g_MetadataUsageListsCount = {0};", args);
                writer.WriteLine("Il2CppMetadataUsageList g_MetadataUsageLists[g_MetadataUsageListsCount] = ");
                writer.WriteLine("{");
                foreach (KeyValuePair<uint, uint> pair in usageLists)
                {
                    object[] objArray2 = new object[] { pair.Key, pair.Value };
                    writer.WriteLine("{{ {0}, {1} }},", objArray2);
                }
                writer.WriteLine("};");
                writer.WriteLine();
                object[] objArray3 = new object[] { usagePairs.Count };
                writer.WriteLine("extern const int g_MetadataUsagePairsCount = {0};", objArray3);
                writer.WriteLine("Il2CppMetadataUsagePair g_MetadataUsagePairs[] = ");
                writer.WriteLine("{");
                foreach (KeyValuePair<uint, uint> pair2 in usagePairs)
                {
                    object[] objArray4 = new object[] { pair2.Key, pair2.Value };
                    writer.WriteLine("{{ {0}, {1} }},", objArray4);
                }
                writer.WriteLine("};");
                writer.WriteLine();
            }
        }

        public static void WriteMethodIndexTable(NPath outputDir, IMethodCollectorResults methodCollector, IMetadataCollection metadataCollector)
        {
            ReadOnlyCollection<MethodReference> methods = methodCollector.GetMethods();
            if (methods.Count > 0)
            {
                string[] append = new string[] { "Il2CppMethodIndexTable.cpp" };
                using (SourceCodeWriter writer = new SourceCodeWriter(outputDir.Combine(append)))
                {
                    writer.AddCodeGenIncludes();
                    writer.WriteLine("MethodIndex g_MethodIndices[] = ");
                    writer.WriteLine("{");
                    foreach (MethodReference reference in methods)
                    {
                        object[] args = new object[] { metadataCollector.GetMethodIndex(reference.Resolve()) };
                        writer.WriteLine("{0},", args);
                    }
                    writer.WriteLine("};");
                    writer.WriteLine();
                }
            }
        }

        public static void WriteMethodInfoMapping(NPath outputDir, IMetadataCollection metadataCollection, IMethodCollectorResults methodCollector)
        {
            ReadOnlyCollection<MethodDefinition> methods = metadataCollection.GetMethods();
            Dictionary<ulong, MethodReference> dictionary = new Dictionary<ulong, MethodReference>();
            string[] append = new string[] { "Il2CppMethodInfoMapping.cpp" };
            using (SourceCodeWriter writer = new SourceCodeWriter(outputDir.Combine(append)))
            {
                writer.AddInclude("il2cpp-mapping.h");
                writer.WriteLine("class MonoMethodInfoMapContainer : public MonoMethodInfoMapContainerBase");
                writer.WriteLine("{");
                writer.WriteLine("private:");
                writer.WriteLine();
                writer.Indent(1);
                int num = 0;
                foreach (List<MethodDefinition> list in methods.Chunk<MethodDefinition>(0x2710))
                {
                    object[] args = new object[] { num };
                    writer.WriteLine("void initializeMap{0}()", args);
                    writer.WriteLine("{");
                    writer.Indent(1);
                    foreach (MethodDefinition definition in list)
                    {
                        if (!definition.DeclaringType.IsIl2CppComDelegate() && !definition.DeclaringType.IsIl2CppComObject())
                        {
                            List<MethodHashComponent> typeHashes = GetTypeHashes(definition);
                            ulong key = CombineAllHashes(typeHashes);
                            if (dictionary.ContainsKey(key))
                            {
                                MethodReference reference = dictionary[key];
                                throw new Exception("Hash collision detected in MonoMethodInfoMapContainer - " + reference.FullName);
                            }
                            dictionary.Add(key, definition);
                            object[] objArray2 = new object[] { key, FormatForMonoReflectionMatching(definition), FormatHashList(typeHashes), RuntimeInvokerCollectorReader.GetIndex(definition), methodCollector.GetMethodIndex(definition) };
                            writer.WriteLine("m_map.add(0x{0:X8} /* {1} - {2} */, MonoMethodInfo({3}, {4}));", objArray2);
                        }
                    }
                    writer.Dedent(1);
                    writer.WriteLine("}");
                    num++;
                }
                writer.Dedent(1);
                writer.WriteLine();
                writer.WriteLine("public:");
                writer.Indent(1);
                writer.WriteLine("MonoMethodInfoMapContainer()");
                writer.WriteLine("{");
                writer.Indent(1);
                for (int i = 0; i < num; i++)
                {
                    object[] objArray3 = new object[] { i };
                    writer.WriteLine("initializeMap{0}();", objArray3);
                }
                writer.Dedent(1);
                writer.WriteLine("}");
                writer.Dedent(1);
                writer.WriteLine("};");
                writer.WriteLine();
                writer.WriteLine("static const MonoMethodInfoMapContainer s_methodsMapContainer;");
                writer.WriteLine("const MonoMethodInfoMapContainerBase* g_pMethodsMapContainer = &s_methodsMapContainer;");
                writer.WriteLine();
            }
        }

        public static void WriteMonoMetadataForFields(NPath outputDir, List<FieldReference> fields)
        {
            string[] append = new string[] { "Il2CppMonoFieldTable.cpp" };
            using (SourceCodeWriter writer = new SourceCodeWriter(outputDir.Combine(append)))
            {
                writer.AddInclude("il2cpp-mapping.h");
                object[] args = new object[] { fields.Count };
                writer.WriteLine("extern const int g_MonoFieldMetadataCount = {0};", args);
                writer.WriteLine("MonoFieldMetadata g_MonoFieldMetadataTable[g_MonoFieldMetadataCount] = ");
                writer.WriteLine("{");
                foreach (FieldReference reference in fields)
                {
                    object[] objArray2 = new object[] { Il2CppTypeCollector.GetIndex(reference.DeclaringType, 0), MetadataTokenUtils.FormattedMetadataTokenFor(reference) };
                    writer.WriteLine("{{ {0}, {1} }},", objArray2);
                }
                writer.WriteLine("};");
                writer.WriteLine();
            }
        }

        public static void WriteMonoMetadataForMethods(NPath outputDir, List<MethodDefinition> methods, IInteropDataCollectorResults interopDataCollector)
        {
            string[] append = new string[] { "Il2CppMonoMethodTable.cpp" };
            using (SourceCodeWriter writer = new SourceCodeWriter(outputDir.Combine(append)))
            {
                writer.AddInclude("il2cpp-mapping.h");
                object[] args = new object[] { methods.Count };
                writer.WriteLine("extern const int g_MonoMethodMetadataCount = {0};", args);
                writer.WriteLine("MonoMethodMetadata g_MonoMethodMetadataTable[g_MonoMethodMetadataCount] = ");
                writer.WriteLine("{");
                foreach (MethodDefinition definition in methods)
                {
                    object[] objArray2 = new object[] { MetadataTokenUtils.FormatMonoMetadataTokenFor(definition), interopDataCollector.GetReversePInvokeWrapperIndex(definition) };
                    writer.WriteLine("{{ {0}, {1} }},", objArray2);
                }
                writer.WriteLine("};");
                writer.WriteLine();
            }
        }

        public static void WriteMonoMetadataForStrings(NPath outputDir, ReadOnlyCollection<StringMetadataToken> stringMetadataTokens)
        {
            string[] append = new string[] { "Il2CppMonoStringTable.cpp" };
            using (SourceCodeWriter writer = new SourceCodeWriter(outputDir.Combine(append)))
            {
                writer.AddInclude("il2cpp-mapping.h");
                object[] args = new object[] { stringMetadataTokens.Count };
                writer.WriteLine("extern const int g_MonoStringMetadataCount = {0};", args);
                writer.WriteLine("MonoMetadataToken g_MonoStringMetadataTable[g_MonoStringMetadataCount] = ");
                writer.WriteLine("{");
                foreach (StringMetadataToken token in stringMetadataTokens)
                {
                    object[] objArray2 = new object[] { MetadataTokenUtils.FormatMonoMetadataTokenFor(token) };
                    writer.WriteLine("{0},", objArray2);
                }
                writer.WriteLine("};");
                writer.WriteLine();
            }
        }

        public static void WriteMonoMetadataForTypes(NPath outputDir, List<TypeReference> types)
        {
            string[] append = new string[] { "Il2CppMonoTypeTable.cpp" };
            using (SourceCodeWriter writer = new SourceCodeWriter(outputDir.Combine(append)))
            {
                writer.AddInclude("il2cpp-mapping.h");
                writer.WriteLine();
                HashSet<string> typeIndexArrays = new HashSet<string>();
                foreach (TypeReference reference in types)
                {
                    if (reference.IsGenericInstance)
                    {
                        WriteGenericArgumentIndices(writer, reference as GenericInstanceType, typeIndexArrays);
                    }
                }
                object[] args = new object[] { types.Count };
                writer.WriteLine("extern const int g_MonoTypeMetadataCount = {0};", args);
                writer.WriteLine("MonoClassMetadata g_MonoTypeMetadataTable[g_MonoTypeMetadataCount] = ");
                writer.WriteLine("{");
                foreach (TypeReference reference2 in types)
                {
                    GenericInstanceType inst = reference2 as GenericInstanceType;
                    ArrayType type2 = reference2 as ArrayType;
                    string str = !reference2.IsPointer ? "false" : "true";
                    if (inst != null)
                    {
                        TypeDefinition type = inst.Resolve();
                        int num = !inst.HasGenericArguments ? 0 : inst.GenericArguments.Count;
                        object[] objArray2 = new object[] { MetadataTokenUtils.FormatMonoMetadataTokenFor(type), num, NameForGenericInstArgsArray(inst), str, 0, -1 };
                        writer.WriteLine("{{ {0}, {1}, {2}, {3}, {4}, {5} }},", objArray2);
                    }
                    else if (type2 != null)
                    {
                        object[] objArray3 = new object[] { MetadataTokenUtils.FormatMonoMetadataTokenFor(type2.ElementType), str, type2.Rank, Il2CppTypeCollector.GetIndex(type2.ElementType, 0) };
                        writer.WriteLine("{{ {0}, 0, NULL, {1}, {2}, {3} }},", objArray3);
                    }
                    else
                    {
                        object[] objArray4 = new object[] { MetadataTokenUtils.FormatMonoMetadataTokenFor(reference2), str, 0, -1 };
                        writer.WriteLine("{{ {0}, 0, NULL, {1}, {2}, {3} }},", objArray4);
                    }
                }
                writer.WriteLine("};");
                writer.WriteLine();
            }
        }

        public static void WriteRuntimeGenericContextEntires(NPath outputDir, IMetadataCollection metadataCollection)
        {
            string[] append = new string[] { "Il2CppRuntimeGenericContexts.cpp" };
            using (SourceCodeWriter writer = new SourceCodeWriter(outputDir.Combine(append)))
            {
                writer.AddInclude("il2cpp-metadata.h");
                ReadOnlyCollection<RGCTXEntry> rGCTXEntries = metadataCollection.GetRGCTXEntries();
                object[] args = new object[] { rGCTXEntries.Count };
                writer.WriteLine("extern const MonoRGCTXDefinition g_RuntimeGenericContextIndices[{0}] =", args);
                writer.WriteLine("{");
                writer.Indent(1);
                foreach (RGCTXEntry entry in rGCTXEntries)
                {
                    object[] objArray2 = new object[] { IntToIl2CppRGCTXData(entry.Type), entry.ImageName, MetadataTokenUtils.FormattedMetadataTokenFor(entry.TypeOrMethodMetadataIndex, entry.FullName), entry.GenericParameterIndex };
                    writer.WriteLine("{{ {0}, \"{1}\", {2}, {3} }},", objArray2);
                }
                writer.Dedent(1);
                writer.WriteLine("};");
                writer.WriteLine();
                writer.AddInclude("il2cpp-mapping.h");
                Dictionary<string, List<MethodReference>> imagesWithMethods = ImagesToMethodDictionaryFor((IEnumerable<MethodReference>) metadataCollection.GetMethods());
                Dictionary<string, List<TypeReference>> dictionary2 = ImagesToTypeDictionaryFor((IEnumerable<TypeReference>) metadataCollection.GetTypeInfos());
                foreach (string str in dictionary2.Keys)
                {
                    List<MethodReference> list = null;
                    if (imagesWithMethods.ContainsKey(str))
                    {
                        list = imagesWithMethods[str];
                    }
                    int num = (list == null) ? 0 : list.Count;
                    List<TypeReference> list2 = dictionary2[str];
                    object[] objArray3 = new object[] { RGCTXArrayNameFor(str), list2.Count + num };
                    writer.WriteLine("static const RuntimeGenericContextInfo {0}[{1}] =", objArray3);
                    writer.WriteLine("{");
                    writer.Indent(1);
                    foreach (TypeReference reference in list2)
                    {
                        object[] objArray4 = new object[] { MetadataTokenUtils.FormattedMetadataTokenFor(reference), metadataCollection.GetRGCTXEntriesStartIndex(reference), metadataCollection.GetRGCTXEntriesCount(reference) };
                        writer.WriteLine("{{ {0}, {1}, {2} }},", objArray4);
                    }
                    if (list != null)
                    {
                        foreach (MethodReference reference2 in list)
                        {
                            object[] objArray5 = new object[] { MetadataTokenUtils.FormattedMetadataTokenFor(reference2), metadataCollection.GetRGCTXEntriesStartIndex(reference2), metadataCollection.GetRGCTXEntriesCount(reference2) };
                            writer.WriteLine("{{ {0}, {1}, {2} }},", objArray5);
                        }
                    }
                    writer.Dedent(1);
                    writer.WriteLine("};");
                    writer.WriteLine();
                }
                int count = dictionary2.Count;
                if (count > 0)
                {
                    object[] objArray6 = new object[] { count };
                    writer.WriteLine("int g_RuntimeGenericContextMappingSize = {0};", objArray6);
                    object[] objArray7 = new object[] { count };
                    writer.WriteLine("extern const ImageRuntimeGenericContextTokens g_RuntimeGenericContextMapping[{0}] =", objArray7);
                    writer.WriteLine("{");
                    writer.Indent(1);
                    foreach (string str2 in dictionary2.Keys)
                    {
                        object[] objArray8 = new object[] { str2, dictionary2[str2].Count + NumberOfMethodsInImage(imagesWithMethods, str2), RGCTXArrayNameFor(str2) };
                        writer.WriteLine("{{ \"{0}\", {1}, {2}, }},", objArray8);
                    }
                    writer.Dedent(1);
                    writer.WriteLine("};");
                }
            }
        }

        private class MethodHashComponent
        {
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private readonly string <Source>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private readonly uint <Value>k__BackingField;

            public MethodHashComponent(MethodReference type)
            {
                this.<Value>k__BackingField = type.MetadataToken.ToUInt32();
                this.<Source>k__BackingField = type.Name;
            }

            public MethodHashComponent(TypeReference type)
            {
                this.<Value>k__BackingField = type.MetadataToken.ToUInt32();
                this.<Source>k__BackingField = type.Name;
            }

            public MethodHashComponent(string name)
            {
                this.<Value>k__BackingField = MonoRuntimeDataWriter.HashStringDjb2(name);
                this.<Source>k__BackingField = name;
            }

            public MethodHashComponent(uint value, string source)
            {
                this.<Value>k__BackingField = value;
                this.<Source>k__BackingField = source;
            }

            public string Source =>
                this.<Source>k__BackingField;

            public uint Value =>
                this.<Value>k__BackingField;
        }
    }
}

