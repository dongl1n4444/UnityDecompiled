namespace Unity.IL2CPP.Metadata
{
    using Mono.Cecil;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using Unity.IL2CPP;
    using Unity.IL2CPP.GenericSharing;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Marshaling;
    using Unity.IL2CPP.Marshaling.MarshalInfoWriters;
    using Unity.IL2CPP.Metadata.Fields;
    using Unity.IL2CPP.Portability;

    public class MetadataCollector : IMetadataCollection
    {
        private readonly Dictionary<AssemblyDefinition, int> _assemblies = new Dictionary<AssemblyDefinition, int>();
        private readonly List<byte> _defaultValueData = new List<byte>();
        private readonly Dictionary<EventDefinition, int> _events = new Dictionary<EventDefinition, int>();
        private readonly Dictionary<FieldDefaultValue, int> _fieldDefaultValues = new Dictionary<FieldDefaultValue, int>();
        private readonly List<FieldMarshaledSize> _fieldMarshaledSizes = new List<FieldMarshaledSize>();
        private readonly Dictionary<FieldDefinition, int> _fields = new Dictionary<FieldDefinition, int>();
        private readonly Dictionary<AssemblyDefinition, Tuple<int, int>> _firstReferencedAssemblyIndexCache = new Dictionary<AssemblyDefinition, Tuple<int, int>>();
        private readonly Dictionary<IGenericParameterProvider, int> _genericContainers = new Dictionary<IGenericParameterProvider, int>();
        private readonly List<int> _genericParameterConstraints = new List<int>();
        private readonly Dictionary<GenericParameter, int> _genericParameterConstraintsStart = new Dictionary<GenericParameter, int>();
        private readonly Dictionary<GenericParameter, int> _genericParameters = new Dictionary<GenericParameter, int>();
        private readonly Dictionary<TypeDefinition, int> _guids = new Dictionary<TypeDefinition, int>();
        private readonly List<KeyValuePair<int, int>> _interfaceOffsets = new List<KeyValuePair<int, int>>();
        private readonly Dictionary<TypeDefinition, int> _interfaceOffsetsStart = new Dictionary<TypeDefinition, int>();
        private readonly List<int> _interfaces = new List<int>();
        private readonly Dictionary<TypeDefinition, int> _interfacesStart = new Dictionary<TypeDefinition, int>();
        private readonly Dictionary<MethodDefinition, int> _methods = new Dictionary<MethodDefinition, int>();
        private readonly Dictionary<ModuleDefinition, int> _modules = new Dictionary<ModuleDefinition, int>();
        private readonly List<int> _nestedTypes = new List<int>();
        private readonly Dictionary<TypeDefinition, int> _nestedTypesStart = new Dictionary<TypeDefinition, int>();
        private readonly Dictionary<ParameterDefaultValue, int> _parameterDefaultValues = new Dictionary<ParameterDefaultValue, int>();
        private readonly Dictionary<ParameterDefinition, int> _parameters = new Dictionary<ParameterDefinition, int>();
        private readonly Dictionary<PropertyDefinition, int> _properties = new Dictionary<PropertyDefinition, int>();
        private readonly List<int> _referencedAssemblyTable = new List<int>();
        private readonly List<KeyValuePair<int, uint>> _rgctxEntries = new List<KeyValuePair<int, uint>>();
        private readonly Dictionary<IGenericParameterProvider, int> _rgctxEntriesCount = new Dictionary<IGenericParameterProvider, int>();
        private readonly Dictionary<IGenericParameterProvider, int> _rgctxEntriesStart = new Dictionary<IGenericParameterProvider, int>();
        private readonly List<byte> _stringData = new List<byte>();
        private readonly Dictionary<string, int> _strings = new Dictionary<string, int>();
        private readonly Dictionary<TypeDefinition, int> _typeInfos = new Dictionary<TypeDefinition, int>();
        private readonly List<uint> _vtableMethods = new List<uint>();
        private readonly Dictionary<TypeDefinition, int> _vtableMethodsStart = new Dictionary<TypeDefinition, int>();
        [CompilerGenerated]
        private static Func<KeyValuePair<TypeReference, int>, KeyValuePair<int, int>> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<KeyValuePair<FieldDefinition, int>, int> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<KeyValuePair<EventDefinition, int>, EventDefinition> <>f__am$cache10;
        [CompilerGenerated]
        private static Func<KeyValuePair<IGenericParameterProvider, int>, int> <>f__am$cache11;
        [CompilerGenerated]
        private static Func<KeyValuePair<IGenericParameterProvider, int>, IGenericParameterProvider> <>f__am$cache12;
        [CompilerGenerated]
        private static Func<KeyValuePair<GenericParameter, int>, int> <>f__am$cache13;
        [CompilerGenerated]
        private static Func<KeyValuePair<GenericParameter, int>, GenericParameter> <>f__am$cache14;
        [CompilerGenerated]
        private static Func<KeyValuePair<ModuleDefinition, int>, int> <>f__am$cache15;
        [CompilerGenerated]
        private static Func<KeyValuePair<ModuleDefinition, int>, ModuleDefinition> <>f__am$cache16;
        [CompilerGenerated]
        private static Func<KeyValuePair<AssemblyDefinition, int>, int> <>f__am$cache17;
        [CompilerGenerated]
        private static Func<KeyValuePair<AssemblyDefinition, int>, AssemblyDefinition> <>f__am$cache18;
        [CompilerGenerated]
        private static Func<KeyValuePair<TypeDefinition, int>, int> <>f__am$cache19;
        [CompilerGenerated]
        private static Func<KeyValuePair<TypeDefinition, int>, TypeDefinition> <>f__am$cache1A;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache1B;
        [CompilerGenerated]
        private static Func<InterfaceImplementation, int, int> <>f__am$cache1C;
        [CompilerGenerated]
        private static Func<TypeReference, int, int> <>f__am$cache1D;
        [CompilerGenerated]
        private static Func<KeyValuePair<FieldDefinition, int>, FieldDefinition> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<KeyValuePair<FieldDefaultValue, int>, int> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<KeyValuePair<FieldDefaultValue, int>, FieldDefaultValue> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<KeyValuePair<ParameterDefaultValue, int>, int> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<KeyValuePair<ParameterDefaultValue, int>, ParameterDefaultValue> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<KeyValuePair<TypeDefinition, int>, int> <>f__am$cache7;
        [CompilerGenerated]
        private static Func<KeyValuePair<TypeDefinition, int>, TypeDefinition> <>f__am$cache8;
        [CompilerGenerated]
        private static Func<KeyValuePair<MethodDefinition, int>, int> <>f__am$cache9;
        [CompilerGenerated]
        private static Func<KeyValuePair<MethodDefinition, int>, MethodDefinition> <>f__am$cacheA;
        [CompilerGenerated]
        private static Func<KeyValuePair<ParameterDefinition, int>, int> <>f__am$cacheB;
        [CompilerGenerated]
        private static Func<KeyValuePair<ParameterDefinition, int>, ParameterDefinition> <>f__am$cacheC;
        [CompilerGenerated]
        private static Func<KeyValuePair<PropertyDefinition, int>, int> <>f__am$cacheD;
        [CompilerGenerated]
        private static Func<KeyValuePair<PropertyDefinition, int>, PropertyDefinition> <>f__am$cacheE;
        [CompilerGenerated]
        private static Func<KeyValuePair<EventDefinition, int>, int> <>f__am$cacheF;
        [CompilerGenerated]
        private static Func<RuntimeGenericData, KeyValuePair<int, uint>> <>f__mg$cache0;
        [CompilerGenerated]
        private static Func<RuntimeGenericData, KeyValuePair<int, uint>> <>f__mg$cache1;
        [Inject]
        public static IAssemblyDependencies AssemblyDependencies;
        [Inject]
        public static IGenericSharingAnalysisService GenericSharingAnalysis;
        [Inject]
        public static IIl2CppGenericMethodCollectorReaderService Il2CppGenericMethodCollectorReader;
        [Inject]
        public static IIl2CppGenericMethodCollectorWriterService Il2CppGenericMethodCollectorWriter;
        [Inject]
        public static IIl2CppTypeCollectorWriterService Il2CppTypeCollector;
        [Inject]
        public static IIl2CppMethodReferenceCollectorWriterService MethodReferenceCollector;
        [Inject]
        public static IRuntimeInvokerCollectorAdderService RuntimeInvokerCollectorAdder;

        public void AddAssemblies(ICollection<AssemblyDefinition> assemblies)
        {
            foreach (AssemblyDefinition definition in assemblies)
            {
                this.AddAssembly(definition);
            }
            foreach (AssemblyDefinition definition2 in assemblies)
            {
                this.AddVTables(definition2.MainModule.Types);
            }
            this.AddReferencedAssemblyMetadata(assemblies);
        }

        private void AddAssembly(AssemblyDefinition assemblyDefinition)
        {
            AddUnique<AssemblyDefinition>(this._assemblies, assemblyDefinition, null);
            this.AddString(assemblyDefinition.Name.Name);
            this.AddString(assemblyDefinition.Name.Culture);
            this.AddString(Formatter.Stringify(assemblyDefinition.Name.Hash));
            this.AddString(Formatter.Stringify(assemblyDefinition.Name.PublicKey));
            AddUnique<ModuleDefinition>(this._modules, assemblyDefinition.MainModule, delegate (ModuleDefinition module) {
                if (!string.IsNullOrEmpty(module.FullyQualifiedName))
                {
                    this.AddString(Path.GetFileName(module.FullyQualifiedName));
                }
                this.AddTypeInfos(module.Types);
            });
        }

        public int AddDefaultValueData(byte[] data)
        {
            int count = this._defaultValueData.Count;
            this._defaultValueData.AddRange(data);
            return count;
        }

        public void AddEvents(IEnumerable<EventDefinition> events)
        {
            AddUnique<EventDefinition>(this._events, events, delegate (EventDefinition evt) {
                this.AddString(evt.Name);
                Il2CppTypeCollector.Add(evt.EventType, 0);
            });
        }

        public void AddFields(IEnumerable<FieldDefinition> fields, MarshalType marshalType)
        {
            FieldDefinition[] itemsToAdd = Enumerable.ToArray<FieldDefinition>(fields);
            AddUnique<FieldDefinition>(this._fields, itemsToAdd, delegate (FieldDefinition field) {
                this.AddString(field.Name);
                Il2CppTypeCollector.Add(field.FieldType, (int) field.Attributes);
            });
            AddUnique<FieldDefaultValue>(this._fieldDefaultValues, DefaultValueFromFields(this, Il2CppTypeCollector, itemsToAdd), null);
            AddUnique<FieldMarshaledSize>(this._fieldMarshaledSizes, MarshaledSizeFromFields(this, Il2CppTypeCollector, itemsToAdd, marshalType));
        }

        public void AddGenericContainer(IGenericParameterProvider container)
        {
            AddUnique<IGenericParameterProvider>(this._genericContainers, container, null);
        }

        public void AddGenericParameters(IEnumerable<GenericParameter> genericParameters)
        {
            AddUnique<GenericParameter>(this._genericParameters, genericParameters, delegate (GenericParameter genericParameter) {
                this.AddString(genericParameter.Name);
                if (genericParameter.Constraints.Count > 0)
                {
                    this._genericParameterConstraintsStart.Add(genericParameter, this._genericParameterConstraints.Count);
                    if (<>f__am$cache1D == null)
                    {
                        <>f__am$cache1D = new Func<TypeReference, int, int>(null, (IntPtr) <AddGenericParameters>m__26);
                    }
                    this._genericParameterConstraints.AddRange(Enumerable.Select<TypeReference, int>(genericParameter.Constraints, <>f__am$cache1D));
                }
            });
        }

        public void AddMethods(IEnumerable<MethodDefinition> methods)
        {
            AddUnique<MethodDefinition>(this._methods, methods, delegate (MethodDefinition method) {
                ErrorInformation.CurrentlyProcessing.Method = method;
                this.AddParameters(method.Parameters);
                Il2CppTypeCollector.Add(method.ReturnType, 0);
                this.AddString(method.Name);
                if (method.HasGenericParameters)
                {
                    this.AddGenericContainer(method);
                    this.AddGenericParameters(method.GenericParameters);
                    ReadOnlyCollection<RuntimeGenericData> source = GenericSharingAnalysis.RuntimeGenericContextFor(method).RuntimeGenericDatas;
                    if (source.Count > 0)
                    {
                        this._rgctxEntriesCount.Add(method, source.Count);
                        this._rgctxEntriesStart.Add(method, this._rgctxEntries.Count);
                        if (<>f__mg$cache1 == null)
                        {
                            <>f__mg$cache1 = new Func<RuntimeGenericData, KeyValuePair<int, uint>>(null, (IntPtr) CreateRGCTXEntry);
                        }
                        this._rgctxEntries.AddRange(Enumerable.Select<RuntimeGenericData, KeyValuePair<int, uint>>(source, <>f__mg$cache1));
                    }
                }
                if (!method.DeclaringType.HasGenericParameters && !method.HasGenericParameters)
                {
                    RuntimeInvokerCollectorAdder.Add(method);
                }
            });
        }

        private void AddParameters(IEnumerable<ParameterDefinition> parameters)
        {
            ParameterDefinition[] itemsToAdd = Enumerable.ToArray<ParameterDefinition>(parameters);
            AddUnique<ParameterDefinition>(this._parameters, itemsToAdd, delegate (ParameterDefinition parameter) {
                this.AddString(parameter.Name);
                Il2CppTypeCollector.Add(parameter.ParameterType, (int) parameter.Attributes);
            });
            AddUnique<ParameterDefaultValue>(this._parameterDefaultValues, FromParameters(this, Il2CppTypeCollector, itemsToAdd), null);
        }

        public void AddProperties(IEnumerable<PropertyDefinition> properties)
        {
            AddUnique<PropertyDefinition>(this._properties, properties, property => this.AddString(property.Name));
        }

        private void AddReferencedAssemblyMetadata(ICollection<AssemblyDefinition> assemblies)
        {
            foreach (AssemblyDefinition definition in assemblies)
            {
                List<AssemblyDefinition> source = Enumerable.ToList<AssemblyDefinition>(AssemblyDependencies.GetReferencedAssembliesFor(definition));
                if (source.Count == 0)
                {
                    this._firstReferencedAssemblyIndexCache.Add(definition, new Tuple<int, int>(-1, 0));
                }
                else
                {
                    this._firstReferencedAssemblyIndexCache.Add(definition, new Tuple<int, int>(this._referencedAssemblyTable.Count, source.Count));
                    this._referencedAssemblyTable.AddRange(Enumerable.Select<AssemblyDefinition, int>(Enumerable.Distinct<AssemblyDefinition>(source), new Func<AssemblyDefinition, int>(this, (IntPtr) this.GetAssemblyIndex)));
                }
            }
        }

        public int AddString(string str)
        {
            int count;
            if (!this._strings.TryGetValue(str, out count))
            {
                count = this._stringData.Count;
                this._stringData.AddRange(Encoding.UTF8.GetBytes(str));
                this._stringData.Add(0);
                this._strings.Add(str, count);
            }
            return count;
        }

        public void AddTypeInfos(IEnumerable<TypeDefinition> types)
        {
            AddUnique<TypeDefinition>(this._typeInfos, types, delegate (TypeDefinition type) {
                this.AddString(type.Name);
                this.AddString(type.Namespace);
                Il2CppTypeCollector.Add(type, 0);
                Il2CppTypeCollector.Add(new ByReferenceType(type), 0);
                if (type.BaseType != null)
                {
                    Il2CppTypeCollector.Add(type.BaseType, 0);
                }
                if (<>f__am$cache1B == null)
                {
                    <>f__am$cache1B = new Func<MethodDefinition, bool>(null, (IntPtr) <AddTypeInfos>m__24);
                }
                this.AddMethods(Enumerable.Where<MethodDefinition>(type.Methods, <>f__am$cache1B));
                this.AddFields(type.Fields, MarshalType.PInvoke);
                this.AddProperties(type.Properties);
                this.AddEvents(type.Events);
                if (Extensions.IsComOrWindowsRuntimeInterface(type) && !type.HasGenericParameters)
                {
                    this._guids.Add(type, this._guids.Count);
                }
                if (type.HasNestedTypes)
                {
                    this.AddTypeInfos(type.NestedTypes);
                    this._nestedTypesStart.Add(type, this._nestedTypes.Count);
                    this._nestedTypes.AddRange(Enumerable.Select<TypeDefinition, int>(type.NestedTypes, new Func<TypeDefinition, int>(this, (IntPtr) this.GetTypeInfoIndex)));
                }
                if (type.HasInterfaces)
                {
                    this._interfacesStart.Add(type, this._interfaces.Count);
                    if (<>f__am$cache1C == null)
                    {
                        <>f__am$cache1C = new Func<InterfaceImplementation, int, int>(null, (IntPtr) <AddTypeInfos>m__25);
                    }
                    this._interfaces.AddRange(Enumerable.Select<InterfaceImplementation, int>(type.Interfaces, <>f__am$cache1C));
                }
                if (type.HasGenericParameters)
                {
                    this.AddGenericContainer(type);
                    this.AddGenericParameters(type.GenericParameters);
                    ReadOnlyCollection<RuntimeGenericData> source = GenericSharingAnalysis.RuntimeGenericContextFor(type).RuntimeGenericDatas;
                    if (source.Count > 0)
                    {
                        this._rgctxEntriesCount.Add(type, source.Count);
                        this._rgctxEntriesStart.Add(type, this._rgctxEntries.Count);
                        if (<>f__mg$cache0 == null)
                        {
                            <>f__mg$cache0 = new Func<RuntimeGenericData, KeyValuePair<int, uint>>(null, (IntPtr) CreateRGCTXEntry);
                        }
                        this._rgctxEntries.AddRange(Enumerable.Select<RuntimeGenericData, KeyValuePair<int, uint>>(source, <>f__mg$cache0));
                    }
                }
            });
        }

        private static void AddUnique<T>(List<T> items, IEnumerable<T> itemsToAdd)
        {
            foreach (T local in itemsToAdd)
            {
                AddUnique<T>(items, local);
            }
        }

        private static void AddUnique<T>(List<T> items, T item)
        {
            if (items.Contains(item))
            {
                throw new Exception(string.Format("Attempting to add unique metadata item {0} multiple times.", item));
            }
            items.Add(item);
        }

        private static void AddUnique<T>(Dictionary<T, int> items, IEnumerable<T> itemsToAdd, [Optional, DefaultParameterValue(null)] Action<T> onAdd)
        {
            foreach (T local in itemsToAdd)
            {
                AddUnique<T>(items, local, onAdd);
            }
        }

        private static void AddUnique<T>(Dictionary<T, int> items, T item, [Optional, DefaultParameterValue(null)] Action<T> onAdd)
        {
            int count;
            if (items.TryGetValue(item, out count))
            {
                throw new Exception(string.Format("Attempting to add unique metadata item {0} multiple times.", item));
            }
            count = items.Count;
            items.Add(item, count);
            if (onAdd != null)
            {
                onAdd(item);
            }
        }

        public void AddVTables(IEnumerable<TypeDefinition> types)
        {
            foreach (TypeDefinition definition in types)
            {
                if (definition.HasNestedTypes)
                {
                    this.AddVTables(definition.NestedTypes);
                }
                if (!definition.IsInterface || Extensions.IsComOrWindowsRuntimeType(definition))
                {
                    VTable table = new VTableBuilder().VTableFor(definition, null);
                    this._vtableMethodsStart.Add(definition, this._vtableMethods.Count);
                    this._vtableMethods.AddRange(Enumerable.Select<MethodReference, uint>(table.Slots, new Func<MethodReference, uint>(this, (IntPtr) this.<AddVTables>m__3)));
                    this._interfaceOffsetsStart.Add(definition, this._interfaceOffsets.Count);
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = new Func<KeyValuePair<TypeReference, int>, KeyValuePair<int, int>>(null, (IntPtr) <AddVTables>m__4);
                    }
                    this._interfaceOffsets.AddRange(Enumerable.Select<KeyValuePair<TypeReference, int>, KeyValuePair<int, int>>(table.InterfaceOffsets, <>f__am$cache0));
                }
            }
        }

        private static KeyValuePair<int, uint> CreateRGCTXEntry(RuntimeGenericData data)
        {
            RuntimeGenericInflatedTypeData data2 = data as RuntimeGenericInflatedTypeData;
            RuntimeGenericMethodData data3 = data as RuntimeGenericMethodData;
            switch (data.InfoType)
            {
                case RuntimeGenericContextInfo.Class:
                case RuntimeGenericContextInfo.Static:
                    return new KeyValuePair<int, uint>(2, (uint) Il2CppTypeCollector.GetOrCreateIndex(data2.Data, 0));

                case RuntimeGenericContextInfo.Type:
                    return new KeyValuePair<int, uint>(1, (uint) Il2CppTypeCollector.GetOrCreateIndex(data2.Data, 0));

                case RuntimeGenericContextInfo.Array:
                {
                    ArrayType type = new ArrayType(data2.Data, 1);
                    return new KeyValuePair<int, uint>(2, (uint) Il2CppTypeCollector.GetOrCreateIndex(type, 0));
                }
                case RuntimeGenericContextInfo.Method:
                    Il2CppGenericMethodCollectorWriter.Add(data3.Data);
                    return new KeyValuePair<int, uint>(3, Il2CppGenericMethodCollectorReader.GetIndex(data3.Data));
            }
            throw new ArgumentOutOfRangeException();
        }

        [DebuggerHidden]
        private static IEnumerable<FieldDefaultValue> DefaultValueFromFields(MetadataCollector metadataCollector, IIl2CppTypeCollectorWriterService typeCollector, IEnumerable<FieldDefinition> fields)
        {
            return new <DefaultValueFromFields>c__Iterator0 { 
                fields = fields,
                metadataCollector = metadataCollector,
                typeCollector = typeCollector,
                $PC = -2
            };
        }

        [DebuggerHidden]
        private static IEnumerable<ParameterDefaultValue> FromParameters(MetadataCollector metadataCollector, IIl2CppTypeCollectorWriterService typeCollector, IEnumerable<ParameterDefinition> parameters)
        {
            return new <FromParameters>c__Iterator2 { 
                parameters = parameters,
                metadataCollector = metadataCollector,
                typeCollector = typeCollector,
                $PC = -2
            };
        }

        public ReadOnlyCollection<AssemblyDefinition> GetAssemblies()
        {
            if (<>f__am$cache17 == null)
            {
                <>f__am$cache17 = new Func<KeyValuePair<AssemblyDefinition, int>, int>(null, (IntPtr) <GetAssemblies>m__20);
            }
            if (<>f__am$cache18 == null)
            {
                <>f__am$cache18 = new Func<KeyValuePair<AssemblyDefinition, int>, AssemblyDefinition>(null, (IntPtr) <GetAssemblies>m__21);
            }
            return CollectionExtensions.AsReadOnlyPortable<AssemblyDefinition>(Enumerable.ToArray<AssemblyDefinition>(Enumerable.Select<KeyValuePair<AssemblyDefinition, int>, AssemblyDefinition>(Enumerable.OrderBy<KeyValuePair<AssemblyDefinition, int>, int>(this._assemblies, <>f__am$cache17), <>f__am$cache18)));
        }

        public int GetAssemblyIndex(AssemblyDefinition assembly)
        {
            return this._assemblies[assembly];
        }

        public ReadOnlyCollection<byte> GetDefaultValueData()
        {
            return this._defaultValueData.AsReadOnly();
        }

        public int GetEventIndex(EventDefinition @event)
        {
            return this._events[@event];
        }

        public ReadOnlyCollection<EventDefinition> GetEvents()
        {
            if (<>f__am$cacheF == null)
            {
                <>f__am$cacheF = new Func<KeyValuePair<EventDefinition, int>, int>(null, (IntPtr) <GetEvents>m__18);
            }
            if (<>f__am$cache10 == null)
            {
                <>f__am$cache10 = new Func<KeyValuePair<EventDefinition, int>, EventDefinition>(null, (IntPtr) <GetEvents>m__19);
            }
            return CollectionExtensions.AsReadOnlyPortable<EventDefinition>(Enumerable.ToArray<EventDefinition>(Enumerable.Select<KeyValuePair<EventDefinition, int>, EventDefinition>(Enumerable.OrderBy<KeyValuePair<EventDefinition, int>, int>(this._events, <>f__am$cacheF), <>f__am$cache10)));
        }

        public ReadOnlyCollection<FieldDefaultValue> GetFieldDefaultValues()
        {
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = new Func<KeyValuePair<FieldDefaultValue, int>, int>(null, (IntPtr) <GetFieldDefaultValues>m__C);
            }
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = new Func<KeyValuePair<FieldDefaultValue, int>, FieldDefaultValue>(null, (IntPtr) <GetFieldDefaultValues>m__D);
            }
            return CollectionExtensions.AsReadOnlyPortable<FieldDefaultValue>(Enumerable.ToArray<FieldDefaultValue>(Enumerable.Select<KeyValuePair<FieldDefaultValue, int>, FieldDefaultValue>(Enumerable.OrderBy<KeyValuePair<FieldDefaultValue, int>, int>(this._fieldDefaultValues, <>f__am$cache3), <>f__am$cache4)));
        }

        public int GetFieldIndex(FieldDefinition field)
        {
            return this._fields[field];
        }

        public ReadOnlyCollection<FieldMarshaledSize> GetFieldMarshaledSizes()
        {
            return CollectionExtensions.AsReadOnlyPortable<FieldMarshaledSize>(this._fieldMarshaledSizes.ToArray());
        }

        public ReadOnlyCollection<FieldDefinition> GetFields()
        {
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<KeyValuePair<FieldDefinition, int>, int>(null, (IntPtr) <GetFields>m__A);
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<KeyValuePair<FieldDefinition, int>, FieldDefinition>(null, (IntPtr) <GetFields>m__B);
            }
            return CollectionExtensions.AsReadOnlyPortable<FieldDefinition>(Enumerable.ToArray<FieldDefinition>(Enumerable.Select<KeyValuePair<FieldDefinition, int>, FieldDefinition>(Enumerable.OrderBy<KeyValuePair<FieldDefinition, int>, int>(this._fields, <>f__am$cache1), <>f__am$cache2)));
        }

        public int GetFirstIndexInReferencedAssemblyTableForAssembly(AssemblyDefinition assembly, out int length)
        {
            Tuple<int, int> tuple = this._firstReferencedAssemblyIndexCache[assembly];
            length = tuple.get_Item2();
            return tuple.get_Item1();
        }

        public int GetGenericContainerIndex(IGenericParameterProvider container)
        {
            int num;
            if (this._genericContainers.TryGetValue(container, out num))
            {
                return num;
            }
            return -1;
        }

        public ReadOnlyCollection<IGenericParameterProvider> GetGenericContainers()
        {
            if (<>f__am$cache11 == null)
            {
                <>f__am$cache11 = new Func<KeyValuePair<IGenericParameterProvider, int>, int>(null, (IntPtr) <GetGenericContainers>m__1A);
            }
            if (<>f__am$cache12 == null)
            {
                <>f__am$cache12 = new Func<KeyValuePair<IGenericParameterProvider, int>, IGenericParameterProvider>(null, (IntPtr) <GetGenericContainers>m__1B);
            }
            return CollectionExtensions.AsReadOnlyPortable<IGenericParameterProvider>(Enumerable.ToArray<IGenericParameterProvider>(Enumerable.Select<KeyValuePair<IGenericParameterProvider, int>, IGenericParameterProvider>(Enumerable.OrderBy<KeyValuePair<IGenericParameterProvider, int>, int>(this._genericContainers, <>f__am$cache11), <>f__am$cache12)));
        }

        public ReadOnlyCollection<int> GetGenericParameterConstraints()
        {
            return CollectionExtensions.AsReadOnlyPortable<int>(this._genericParameterConstraints.ToArray());
        }

        public int GetGenericParameterConstraintsStartIndex(GenericParameter genericParameter)
        {
            return this._genericParameterConstraintsStart[genericParameter];
        }

        public int GetGenericParameterIndex(GenericParameter genericParameter)
        {
            return this._genericParameters[genericParameter];
        }

        public ReadOnlyCollection<GenericParameter> GetGenericParameters()
        {
            if (<>f__am$cache13 == null)
            {
                <>f__am$cache13 = new Func<KeyValuePair<GenericParameter, int>, int>(null, (IntPtr) <GetGenericParameters>m__1C);
            }
            if (<>f__am$cache14 == null)
            {
                <>f__am$cache14 = new Func<KeyValuePair<GenericParameter, int>, GenericParameter>(null, (IntPtr) <GetGenericParameters>m__1D);
            }
            return CollectionExtensions.AsReadOnlyPortable<GenericParameter>(Enumerable.ToArray<GenericParameter>(Enumerable.Select<KeyValuePair<GenericParameter, int>, GenericParameter>(Enumerable.OrderBy<KeyValuePair<GenericParameter, int>, int>(this._genericParameters, <>f__am$cache13), <>f__am$cache14)));
        }

        public int GetGuidIndex(TypeDefinition type)
        {
            int num;
            if (this._guids.TryGetValue(type, out num))
            {
                return num;
            }
            return -1;
        }

        public ReadOnlyCollection<KeyValuePair<int, int>> GetInterfaceOffsets()
        {
            return CollectionExtensions.AsReadOnlyPortable<KeyValuePair<int, int>>(this._interfaceOffsets.ToArray());
        }

        public int GetInterfaceOffsetsStartIndex(TypeDefinition type)
        {
            return this._interfaceOffsetsStart[type];
        }

        public ReadOnlyCollection<int> GetInterfaces()
        {
            return CollectionExtensions.AsReadOnlyPortable<int>(this._interfaces.ToArray());
        }

        public int GetInterfacesStartIndex(TypeDefinition type)
        {
            return this._interfacesStart[type];
        }

        public int GetMethodIndex(MethodDefinition method)
        {
            return this._methods[method];
        }

        public ReadOnlyCollection<MethodDefinition> GetMethods()
        {
            if (<>f__am$cache9 == null)
            {
                <>f__am$cache9 = new Func<KeyValuePair<MethodDefinition, int>, int>(null, (IntPtr) <GetMethods>m__12);
            }
            if (<>f__am$cacheA == null)
            {
                <>f__am$cacheA = new Func<KeyValuePair<MethodDefinition, int>, MethodDefinition>(null, (IntPtr) <GetMethods>m__13);
            }
            return CollectionExtensions.AsReadOnlyPortable<MethodDefinition>(Enumerable.ToArray<MethodDefinition>(Enumerable.Select<KeyValuePair<MethodDefinition, int>, MethodDefinition>(Enumerable.OrderBy<KeyValuePair<MethodDefinition, int>, int>(this._methods, <>f__am$cache9), <>f__am$cacheA)));
        }

        public int GetModuleIndex(ModuleDefinition module)
        {
            return this._modules[module];
        }

        public ReadOnlyCollection<ModuleDefinition> GetModules()
        {
            if (<>f__am$cache15 == null)
            {
                <>f__am$cache15 = new Func<KeyValuePair<ModuleDefinition, int>, int>(null, (IntPtr) <GetModules>m__1E);
            }
            if (<>f__am$cache16 == null)
            {
                <>f__am$cache16 = new Func<KeyValuePair<ModuleDefinition, int>, ModuleDefinition>(null, (IntPtr) <GetModules>m__1F);
            }
            return CollectionExtensions.AsReadOnlyPortable<ModuleDefinition>(Enumerable.ToArray<ModuleDefinition>(Enumerable.Select<KeyValuePair<ModuleDefinition, int>, ModuleDefinition>(Enumerable.OrderBy<KeyValuePair<ModuleDefinition, int>, int>(this._modules, <>f__am$cache15), <>f__am$cache16)));
        }

        public ReadOnlyCollection<int> GetNestedTypes()
        {
            return CollectionExtensions.AsReadOnlyPortable<int>(this._nestedTypes.ToArray());
        }

        public int GetNestedTypesStartIndex(TypeDefinition type)
        {
            return this._nestedTypesStart[type];
        }

        public ReadOnlyCollection<ParameterDefaultValue> GetParameterDefaultValues()
        {
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = new Func<KeyValuePair<ParameterDefaultValue, int>, int>(null, (IntPtr) <GetParameterDefaultValues>m__E);
            }
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = new Func<KeyValuePair<ParameterDefaultValue, int>, ParameterDefaultValue>(null, (IntPtr) <GetParameterDefaultValues>m__F);
            }
            return CollectionExtensions.AsReadOnlyPortable<ParameterDefaultValue>(Enumerable.ToArray<ParameterDefaultValue>(Enumerable.Select<KeyValuePair<ParameterDefaultValue, int>, ParameterDefaultValue>(Enumerable.OrderBy<KeyValuePair<ParameterDefaultValue, int>, int>(this._parameterDefaultValues, <>f__am$cache5), <>f__am$cache6)));
        }

        public int GetParameterIndex(ParameterDefinition parameter)
        {
            return this._parameters[parameter];
        }

        public ReadOnlyCollection<ParameterDefinition> GetParameters()
        {
            if (<>f__am$cacheB == null)
            {
                <>f__am$cacheB = new Func<KeyValuePair<ParameterDefinition, int>, int>(null, (IntPtr) <GetParameters>m__14);
            }
            if (<>f__am$cacheC == null)
            {
                <>f__am$cacheC = new Func<KeyValuePair<ParameterDefinition, int>, ParameterDefinition>(null, (IntPtr) <GetParameters>m__15);
            }
            return CollectionExtensions.AsReadOnlyPortable<ParameterDefinition>(Enumerable.ToArray<ParameterDefinition>(Enumerable.Select<KeyValuePair<ParameterDefinition, int>, ParameterDefinition>(Enumerable.OrderBy<KeyValuePair<ParameterDefinition, int>, int>(this._parameters, <>f__am$cacheB), <>f__am$cacheC)));
        }

        public ReadOnlyCollection<PropertyDefinition> GetProperties()
        {
            if (<>f__am$cacheD == null)
            {
                <>f__am$cacheD = new Func<KeyValuePair<PropertyDefinition, int>, int>(null, (IntPtr) <GetProperties>m__16);
            }
            if (<>f__am$cacheE == null)
            {
                <>f__am$cacheE = new Func<KeyValuePair<PropertyDefinition, int>, PropertyDefinition>(null, (IntPtr) <GetProperties>m__17);
            }
            return CollectionExtensions.AsReadOnlyPortable<PropertyDefinition>(Enumerable.ToArray<PropertyDefinition>(Enumerable.Select<KeyValuePair<PropertyDefinition, int>, PropertyDefinition>(Enumerable.OrderBy<KeyValuePair<PropertyDefinition, int>, int>(this._properties, <>f__am$cacheD), <>f__am$cacheE)));
        }

        public int GetPropertyIndex(PropertyDefinition property)
        {
            return this._properties[property];
        }

        public ReadOnlyCollection<int> GetReferencedAssemblyIndiciesIntoAssemblyTable()
        {
            return this._referencedAssemblyTable.AsReadOnly();
        }

        public ReadOnlyCollection<KeyValuePair<int, uint>> GetRGCTXEntries()
        {
            return CollectionExtensions.AsReadOnlyPortable<KeyValuePair<int, uint>>(this._rgctxEntries.ToArray());
        }

        public int GetRGCTXEntriesCount(IGenericParameterProvider provider)
        {
            int num;
            if (this._rgctxEntriesCount.TryGetValue(provider, out num))
            {
                return num;
            }
            return 0;
        }

        public int GetRGCTXEntriesStartIndex(IGenericParameterProvider provider)
        {
            int num;
            if (this._rgctxEntriesStart.TryGetValue(provider, out num))
            {
                return num;
            }
            return -1;
        }

        public ReadOnlyCollection<byte> GetStringData()
        {
            return this._stringData.AsReadOnly();
        }

        public int GetStringIndex(string str)
        {
            return this._strings[str];
        }

        public int GetTypeInfoIndex(TypeDefinition type)
        {
            return this._typeInfos[type];
        }

        public ReadOnlyCollection<TypeDefinition> GetTypeInfos()
        {
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = new Func<KeyValuePair<TypeDefinition, int>, int>(null, (IntPtr) <GetTypeInfos>m__10);
            }
            if (<>f__am$cache8 == null)
            {
                <>f__am$cache8 = new Func<KeyValuePair<TypeDefinition, int>, TypeDefinition>(null, (IntPtr) <GetTypeInfos>m__11);
            }
            return CollectionExtensions.AsReadOnlyPortable<TypeDefinition>(Enumerable.ToArray<TypeDefinition>(Enumerable.Select<KeyValuePair<TypeDefinition, int>, TypeDefinition>(Enumerable.OrderBy<KeyValuePair<TypeDefinition, int>, int>(this._typeInfos, <>f__am$cache7), <>f__am$cache8)));
        }

        public ReadOnlyCollection<TypeDefinition> GetTypesWithGuids()
        {
            if (<>f__am$cache19 == null)
            {
                <>f__am$cache19 = new Func<KeyValuePair<TypeDefinition, int>, int>(null, (IntPtr) <GetTypesWithGuids>m__22);
            }
            if (<>f__am$cache1A == null)
            {
                <>f__am$cache1A = new Func<KeyValuePair<TypeDefinition, int>, TypeDefinition>(null, (IntPtr) <GetTypesWithGuids>m__23);
            }
            return CollectionExtensions.AsReadOnlyPortable<TypeDefinition>(Enumerable.ToArray<TypeDefinition>(Enumerable.Select<KeyValuePair<TypeDefinition, int>, TypeDefinition>(Enumerable.OrderBy<KeyValuePair<TypeDefinition, int>, int>(this._guids, <>f__am$cache19), <>f__am$cache1A)));
        }

        public ReadOnlyCollection<uint> GetVTableMethods()
        {
            return CollectionExtensions.AsReadOnlyPortable<uint>(this._vtableMethods.ToArray());
        }

        public int GetVTableMethodsStartIndex(TypeDefinition type)
        {
            int num;
            if (this._vtableMethodsStart.TryGetValue(type, out num))
            {
                return num;
            }
            return -1;
        }

        [DebuggerHidden]
        private static IEnumerable<FieldMarshaledSize> MarshaledSizeFromFields(MetadataCollector metadataCollector, IIl2CppTypeCollectorWriterService typeCollector, IEnumerable<FieldDefinition> fields, MarshalType marshalType)
        {
            return new <MarshaledSizeFromFields>c__Iterator1 { 
                fields = fields,
                marshalType = marshalType,
                metadataCollector = metadataCollector,
                typeCollector = typeCollector,
                $PC = -2
            };
        }

        [CompilerGenerated]
        private sealed class <DefaultValueFromFields>c__Iterator0 : IEnumerable, IEnumerable<FieldDefaultValue>, IEnumerator, IDisposable, IEnumerator<FieldDefaultValue>
        {
            internal FieldDefaultValue $current;
            internal bool $disposing;
            internal IEnumerator<FieldDefinition> $locvar0;
            internal int $PC;
            internal FieldDefinition <field>__0;
            internal IEnumerable<FieldDefinition> fields;
            internal MetadataCollector metadataCollector;
            internal IIl2CppTypeCollectorWriterService typeCollector;

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
                        try
                        {
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
                        this.$locvar0 = this.fields.GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 1:
                    case 2:
                        break;

                    default:
                        goto Label_01B3;
                }
                try
                {
                    switch (num)
                    {
                        case 1:
                            goto Label_0104;

                        case 2:
                            goto Label_017B;
                    }
                    while (this.$locvar0.MoveNext())
                    {
                        this.<field>__0 = this.$locvar0.Current;
                        if (this.<field>__0.HasConstant)
                        {
                            this.$current = new FieldDefaultValue(this.metadataCollector.GetFieldIndex(this.<field>__0), this.typeCollector.GetOrCreateIndex(MetadataUtils.GetUnderlyingType(this.<field>__0.FieldType), 0), (this.<field>__0.Constant != null) ? this.metadataCollector.AddDefaultValueData(MetadataUtils.ConstantDataFor(this.<field>__0, this.<field>__0.FieldType, this.<field>__0.FullName)) : -1);
                            if (!this.$disposing)
                            {
                                this.$PC = 1;
                            }
                            flag = true;
                            goto Label_01B5;
                        }
                    Label_0104:
                        if (this.<field>__0.InitialValue.Length > 0)
                        {
                            this.$current = new FieldDefaultValue(this.metadataCollector.GetFieldIndex(this.<field>__0), this.typeCollector.GetOrCreateIndex(MetadataUtils.GetUnderlyingType(this.<field>__0.FieldType), 0), this.metadataCollector.AddDefaultValueData(this.<field>__0.InitialValue));
                            if (!this.$disposing)
                            {
                                this.$PC = 2;
                            }
                            flag = true;
                            goto Label_01B5;
                        }
                    Label_017B:;
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
            Label_01B3:
                return false;
            Label_01B5:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<FieldDefaultValue> IEnumerable<FieldDefaultValue>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new MetadataCollector.<DefaultValueFromFields>c__Iterator0 { 
                    fields = this.fields,
                    metadataCollector = this.metadataCollector,
                    typeCollector = this.typeCollector
                };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<Unity.IL2CPP.Metadata.Fields.FieldDefaultValue>.GetEnumerator();
            }

            FieldDefaultValue IEnumerator<FieldDefaultValue>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <FromParameters>c__Iterator2 : IEnumerable, IEnumerable<ParameterDefaultValue>, IEnumerator, IDisposable, IEnumerator<ParameterDefaultValue>
        {
            internal ParameterDefaultValue $current;
            internal bool $disposing;
            internal IEnumerator<ParameterDefinition> $locvar0;
            internal int $PC;
            internal ParameterDefinition <parameter>__0;
            internal MetadataCollector metadataCollector;
            internal IEnumerable<ParameterDefinition> parameters;
            internal IIl2CppTypeCollectorWriterService typeCollector;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 1:
                        try
                        {
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
                        this.$locvar0 = this.parameters.GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_0134;
                }
                try
                {
                    switch (num)
                    {
                        case 1:
                            goto Label_00FC;
                    }
                    while (this.$locvar0.MoveNext())
                    {
                        this.<parameter>__0 = this.$locvar0.Current;
                        if (this.<parameter>__0.HasConstant)
                        {
                            this.$current = new ParameterDefaultValue(this.metadataCollector.GetParameterIndex(this.<parameter>__0), this.typeCollector.GetOrCreateIndex(MetadataUtils.GetUnderlyingType(this.<parameter>__0.ParameterType), 0), (this.<parameter>__0.Constant != null) ? this.metadataCollector.AddDefaultValueData(MetadataUtils.ConstantDataFor(this.<parameter>__0, this.<parameter>__0.ParameterType, this.<parameter>__0.Name)) : -1);
                            if (!this.$disposing)
                            {
                                this.$PC = 1;
                            }
                            flag = true;
                            return true;
                        }
                    Label_00FC:;
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
            Label_0134:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<ParameterDefaultValue> IEnumerable<ParameterDefaultValue>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new MetadataCollector.<FromParameters>c__Iterator2 { 
                    parameters = this.parameters,
                    metadataCollector = this.metadataCollector,
                    typeCollector = this.typeCollector
                };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<Unity.IL2CPP.Metadata.ParameterDefaultValue>.GetEnumerator();
            }

            ParameterDefaultValue IEnumerator<ParameterDefaultValue>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <MarshaledSizeFromFields>c__Iterator1 : IEnumerable, IEnumerable<FieldMarshaledSize>, IEnumerator, IDisposable, IEnumerator<FieldMarshaledSize>
        {
            internal FieldMarshaledSize $current;
            internal bool $disposing;
            internal IEnumerator<FieldDefinition> $locvar0;
            internal int $PC;
            internal FieldDefinition <field>__0;
            internal DefaultMarshalInfoWriter <marshalInfoWriter>__1;
            internal IEnumerable<FieldDefinition> fields;
            internal MarshalType marshalType;
            internal MetadataCollector metadataCollector;
            internal IIl2CppTypeCollectorWriterService typeCollector;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 1:
                        try
                        {
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
                        this.$locvar0 = this.fields.GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_012A;
                }
                try
                {
                    switch (num)
                    {
                        case 1:
                            goto Label_00F2;
                    }
                    while (this.$locvar0.MoveNext())
                    {
                        this.<field>__0 = this.$locvar0.Current;
                        if (this.<field>__0.HasMarshalInfo)
                        {
                            this.<marshalInfoWriter>__1 = MarshalDataCollector.MarshalInfoWriterFor(this.<field>__0.FieldType, this.marshalType, this.<field>__0.MarshalInfo, false, false, false, null);
                            this.$current = new FieldMarshaledSize(this.metadataCollector.GetFieldIndex(this.<field>__0), this.typeCollector.GetOrCreateIndex(MetadataUtils.GetUnderlyingType(this.<field>__0.FieldType), 0), this.<marshalInfoWriter>__1.NativeSizeWithoutPointers);
                            if (!this.$disposing)
                            {
                                this.$PC = 1;
                            }
                            flag = true;
                            return true;
                        }
                    Label_00F2:;
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
            Label_012A:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<FieldMarshaledSize> IEnumerable<FieldMarshaledSize>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new MetadataCollector.<MarshaledSizeFromFields>c__Iterator1 { 
                    fields = this.fields,
                    marshalType = this.marshalType,
                    metadataCollector = this.metadataCollector,
                    typeCollector = this.typeCollector
                };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<Unity.IL2CPP.Metadata.Fields.FieldMarshaledSize>.GetEnumerator();
            }

            FieldMarshaledSize IEnumerator<FieldMarshaledSize>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

