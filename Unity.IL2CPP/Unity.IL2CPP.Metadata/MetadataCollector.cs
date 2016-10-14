using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.IL2CPP.GenericSharing;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;
using Unity.IL2CPP.Marshaling;
using Unity.IL2CPP.Metadata.Fields;
using Unity.IL2CPP.Portability;

namespace Unity.IL2CPP.Metadata
{
	public class MetadataCollector : IMetadataCollection
	{
		private readonly Dictionary<string, int> _strings = new Dictionary<string, int>();

		private readonly List<byte> _stringData = new List<byte>();

		private readonly Dictionary<FieldDefinition, int> _fields = new Dictionary<FieldDefinition, int>();

		private readonly Dictionary<FieldDefaultValue, int> _fieldDefaultValues = new Dictionary<FieldDefaultValue, int>();

		private readonly Dictionary<ParameterDefaultValue, int> _parameterDefaultValues = new Dictionary<ParameterDefaultValue, int>();

		private readonly List<FieldMarshaledSize> _fieldMarshaledSizes = new List<FieldMarshaledSize>();

		private readonly Dictionary<MethodDefinition, int> _methods = new Dictionary<MethodDefinition, int>();

		private readonly Dictionary<ParameterDefinition, int> _parameters = new Dictionary<ParameterDefinition, int>();

		private readonly Dictionary<PropertyDefinition, int> _properties = new Dictionary<PropertyDefinition, int>();

		private readonly Dictionary<EventDefinition, int> _events = new Dictionary<EventDefinition, int>();

		private readonly Dictionary<TypeDefinition, int> _typeInfos = new Dictionary<TypeDefinition, int>();

		private readonly Dictionary<IGenericParameterProvider, int> _genericContainers = new Dictionary<IGenericParameterProvider, int>();

		private readonly Dictionary<GenericParameter, int> _genericParameters = new Dictionary<GenericParameter, int>();

		private readonly Dictionary<GenericParameter, int> _genericParameterConstraintsStart = new Dictionary<GenericParameter, int>();

		private readonly List<int> _genericParameterConstraints = new List<int>();

		private readonly Dictionary<TypeDefinition, int> _nestedTypesStart = new Dictionary<TypeDefinition, int>();

		private readonly List<int> _nestedTypes = new List<int>();

		private readonly Dictionary<TypeDefinition, int> _interfacesStart = new Dictionary<TypeDefinition, int>();

		private readonly List<int> _interfaces = new List<int>();

		private readonly Dictionary<TypeDefinition, int> _vtableMethodsStart = new Dictionary<TypeDefinition, int>();

		private readonly List<uint> _vtableMethods = new List<uint>();

		private readonly Dictionary<TypeDefinition, int> _interfaceOffsetsStart = new Dictionary<TypeDefinition, int>();

		private readonly List<KeyValuePair<int, int>> _interfaceOffsets = new List<KeyValuePair<int, int>>();

		private readonly Dictionary<IGenericParameterProvider, int> _rgctxEntriesStart = new Dictionary<IGenericParameterProvider, int>();

		private readonly Dictionary<IGenericParameterProvider, int> _rgctxEntriesCount = new Dictionary<IGenericParameterProvider, int>();

		private readonly List<KeyValuePair<int, uint>> _rgctxEntries = new List<KeyValuePair<int, uint>>();

		private readonly List<byte> _defaultValueData = new List<byte>();

		private readonly Dictionary<ModuleDefinition, int> _modules = new Dictionary<ModuleDefinition, int>();

		private readonly Dictionary<AssemblyDefinition, int> _assemblies = new Dictionary<AssemblyDefinition, int>();

		private readonly Dictionary<AssemblyDefinition, Tuple<int, int>> _firstReferencedAssemblyIndexCache = new Dictionary<AssemblyDefinition, Tuple<int, int>>();

		private readonly List<int> _referencedAssemblyTable = new List<int>();

		private readonly Dictionary<TypeDefinition, int> _guids = new Dictionary<TypeDefinition, int>();

		[Inject]
		public static IIl2CppTypeCollectorWriterService Il2CppTypeCollector;

		[Inject]
		public static IGenericSharingAnalysisService GenericSharingAnalysis;

		[Inject]
		public static IRuntimeInvokerCollectorAdderService RuntimeInvokerCollectorAdder;

		[Inject]
		public static IIl2CppGenericMethodCollectorReaderService Il2CppGenericMethodCollectorReader;

		[Inject]
		public static IIl2CppGenericMethodCollectorWriterService Il2CppGenericMethodCollectorWriter;

		[Inject]
		public static IIl2CppMethodReferenceCollectorWriterService MethodReferenceCollector;

		[CompilerGenerated]
		private static Func<RuntimeGenericData, KeyValuePair<int, uint>> <>f__mg$cache0;

		[CompilerGenerated]
		private static Func<RuntimeGenericData, KeyValuePair<int, uint>> <>f__mg$cache1;

		public void AddAssemblies(ICollection<AssemblyDefinition> assemblies)
		{
			foreach (AssemblyDefinition current in assemblies)
			{
				this.AddAssembly(current);
			}
			foreach (AssemblyDefinition current2 in assemblies)
			{
				this.AddVTables(current2.MainModule.Types);
			}
			this.AddReferencedAssemblyMetadata(assemblies);
		}

		private void AddAssembly(AssemblyDefinition assemblyDefinition)
		{
			MetadataCollector.AddUnique<AssemblyDefinition>(this._assemblies, assemblyDefinition, null);
			this.AddString(assemblyDefinition.Name.Name);
			this.AddString(assemblyDefinition.Name.Culture);
			this.AddString(Formatter.Stringify(assemblyDefinition.Name.Hash));
			this.AddString(Formatter.Stringify(assemblyDefinition.Name.PublicKey));
			MetadataCollector.AddUnique<ModuleDefinition>(this._modules, assemblyDefinition.MainModule, delegate(ModuleDefinition module)
			{
				if (!string.IsNullOrEmpty(module.FullyQualifiedName))
				{
					this.AddString(Path.GetFileName(module.FullyQualifiedName));
				}
				this.AddTypeInfos(module.Types);
			});
		}

		public int AddString(string str)
		{
			int count;
			int result;
			if (this._strings.TryGetValue(str, out count))
			{
				result = count;
			}
			else
			{
				count = this._stringData.Count;
				this._stringData.AddRange(Encoding.UTF8.GetBytes(str));
				this._stringData.Add(0);
				this._strings.Add(str, count);
				result = count;
			}
			return result;
		}

		public void AddFields(IEnumerable<FieldDefinition> fields, MarshalType marshalType)
		{
			FieldDefinition[] array = fields.ToArray<FieldDefinition>();
			MetadataCollector.AddUnique<FieldDefinition>(this._fields, array, delegate(FieldDefinition field)
			{
				this.AddString(field.Name);
				MetadataCollector.Il2CppTypeCollector.Add(field.FieldType, (int)field.Attributes);
			});
			MetadataCollector.AddUnique<FieldDefaultValue>(this._fieldDefaultValues, MetadataCollector.DefaultValueFromFields(this, MetadataCollector.Il2CppTypeCollector, array), null);
			MetadataCollector.AddUnique<FieldMarshaledSize>(this._fieldMarshaledSizes, MetadataCollector.MarshaledSizeFromFields(this, MetadataCollector.Il2CppTypeCollector, array, marshalType));
		}

		[DebuggerHidden]
		private static IEnumerable<FieldDefaultValue> DefaultValueFromFields(MetadataCollector metadataCollector, IIl2CppTypeCollectorWriterService typeCollector, IEnumerable<FieldDefinition> fields)
		{
			MetadataCollector.<DefaultValueFromFields>c__Iterator0 <DefaultValueFromFields>c__Iterator = new MetadataCollector.<DefaultValueFromFields>c__Iterator0();
			<DefaultValueFromFields>c__Iterator.fields = fields;
			<DefaultValueFromFields>c__Iterator.metadataCollector = metadataCollector;
			<DefaultValueFromFields>c__Iterator.typeCollector = typeCollector;
			MetadataCollector.<DefaultValueFromFields>c__Iterator0 expr_1C = <DefaultValueFromFields>c__Iterator;
			expr_1C.$PC = -2;
			return expr_1C;
		}

		[DebuggerHidden]
		private static IEnumerable<FieldMarshaledSize> MarshaledSizeFromFields(MetadataCollector metadataCollector, IIl2CppTypeCollectorWriterService typeCollector, IEnumerable<FieldDefinition> fields, MarshalType marshalType)
		{
			MetadataCollector.<MarshaledSizeFromFields>c__Iterator1 <MarshaledSizeFromFields>c__Iterator = new MetadataCollector.<MarshaledSizeFromFields>c__Iterator1();
			<MarshaledSizeFromFields>c__Iterator.fields = fields;
			<MarshaledSizeFromFields>c__Iterator.marshalType = marshalType;
			<MarshaledSizeFromFields>c__Iterator.metadataCollector = metadataCollector;
			<MarshaledSizeFromFields>c__Iterator.typeCollector = typeCollector;
			MetadataCollector.<MarshaledSizeFromFields>c__Iterator1 expr_23 = <MarshaledSizeFromFields>c__Iterator;
			expr_23.$PC = -2;
			return expr_23;
		}

		public void AddTypeInfos(IEnumerable<TypeDefinition> types)
		{
			MetadataCollector.AddUnique<TypeDefinition>(this._typeInfos, types, delegate(TypeDefinition type)
			{
				this.AddString(type.Name);
				this.AddString(type.Namespace);
				MetadataCollector.Il2CppTypeCollector.Add(type, 0);
				MetadataCollector.Il2CppTypeCollector.Add(new ByReferenceType(type), 0);
				if (type.BaseType != null)
				{
					MetadataCollector.Il2CppTypeCollector.Add(type.BaseType, 0);
				}
				this.AddMethods(type.Methods);
				this.AddFields(type.Fields, MarshalType.PInvoke);
				this.AddProperties(type.Properties);
				this.AddEvents(type.Events);
				if (type.IsComOrWindowsRuntimeInterface() && !type.HasGenericParameters)
				{
					this._guids.Add(type, this._guids.Count);
				}
				if (type.HasNestedTypes)
				{
					this.AddTypeInfos(type.NestedTypes);
					this._nestedTypesStart.Add(type, this._nestedTypes.Count);
					this._nestedTypes.AddRange(type.NestedTypes.Select(new Func<TypeDefinition, int>(this.GetTypeInfoIndex)));
				}
				if (type.HasInterfaces)
				{
					this._interfacesStart.Add(type, this._interfaces.Count);
					this._interfaces.AddRange(type.Interfaces.Select((TypeReference a, int b) => MetadataCollector.Il2CppTypeCollector.GetOrCreateIndex(a, b)));
				}
				if (type.HasGenericParameters)
				{
					this.AddGenericContainer(type);
					this.AddGenericParameters(type.GenericParameters);
					ReadOnlyCollection<RuntimeGenericData> runtimeGenericDatas = MetadataCollector.GenericSharingAnalysis.RuntimeGenericContextFor(type).RuntimeGenericDatas;
					if (runtimeGenericDatas.Count > 0)
					{
						this._rgctxEntriesCount.Add(type, runtimeGenericDatas.Count);
						this._rgctxEntriesStart.Add(type, this._rgctxEntries.Count);
						List<KeyValuePair<int, uint>> arg_1EA_0 = this._rgctxEntries;
						IEnumerable<RuntimeGenericData> arg_1E5_0 = runtimeGenericDatas;
						if (MetadataCollector.<>f__mg$cache0 == null)
						{
							MetadataCollector.<>f__mg$cache0 = new Func<RuntimeGenericData, KeyValuePair<int, uint>>(MetadataCollector.CreateRGCTXEntry);
						}
						arg_1EA_0.AddRange(arg_1E5_0.Select(MetadataCollector.<>f__mg$cache0));
					}
				}
			});
		}

		public void AddVTables(IEnumerable<TypeDefinition> types)
		{
			foreach (TypeDefinition current in types)
			{
				if (current.HasNestedTypes)
				{
					this.AddVTables(current.NestedTypes);
				}
				if (!current.IsInterface || current.IsComOrWindowsRuntimeType())
				{
					VTable vTable = new VTableBuilder().VTableFor(current, null);
					this._vtableMethodsStart.Add(current, this._vtableMethods.Count);
					this._vtableMethods.AddRange(from m in vTable.Slots
					select MetadataCollector.MethodReferenceCollector.GetOrCreateIndex(m, this));
					this._interfaceOffsetsStart.Add(current, this._interfaceOffsets.Count);
					this._interfaceOffsets.AddRange(from pair in vTable.InterfaceOffsets
					select new KeyValuePair<int, int>(MetadataCollector.Il2CppTypeCollector.GetOrCreateIndex(pair.Key, 0), pair.Value));
				}
			}
		}

		public void AddProperties(IEnumerable<PropertyDefinition> properties)
		{
			MetadataCollector.AddUnique<PropertyDefinition>(this._properties, properties, delegate(PropertyDefinition property)
			{
				this.AddString(property.Name);
			});
		}

		public void AddEvents(IEnumerable<EventDefinition> events)
		{
			MetadataCollector.AddUnique<EventDefinition>(this._events, events, delegate(EventDefinition evt)
			{
				this.AddString(evt.Name);
				MetadataCollector.Il2CppTypeCollector.Add(evt.EventType, 0);
			});
		}

		public void AddMethods(IEnumerable<MethodDefinition> methods)
		{
			MetadataCollector.AddUnique<MethodDefinition>(this._methods, methods, delegate(MethodDefinition method)
			{
				ErrorInformation.CurrentlyProcessing.Method = method;
				this.AddParameters(method.Parameters);
				MetadataCollector.Il2CppTypeCollector.Add(method.ReturnType, 0);
				this.AddString(method.Name);
				if (method.HasGenericParameters)
				{
					this.AddGenericContainer(method);
					this.AddGenericParameters(method.GenericParameters);
					ReadOnlyCollection<RuntimeGenericData> runtimeGenericDatas = MetadataCollector.GenericSharingAnalysis.RuntimeGenericContextFor(method).RuntimeGenericDatas;
					if (runtimeGenericDatas.Count > 0)
					{
						this._rgctxEntriesCount.Add(method, runtimeGenericDatas.Count);
						this._rgctxEntriesStart.Add(method, this._rgctxEntries.Count);
						List<KeyValuePair<int, uint>> arg_C5_0 = this._rgctxEntries;
						IEnumerable<RuntimeGenericData> arg_C0_0 = runtimeGenericDatas;
						if (MetadataCollector.<>f__mg$cache1 == null)
						{
							MetadataCollector.<>f__mg$cache1 = new Func<RuntimeGenericData, KeyValuePair<int, uint>>(MetadataCollector.CreateRGCTXEntry);
						}
						arg_C5_0.AddRange(arg_C0_0.Select(MetadataCollector.<>f__mg$cache1));
					}
				}
				if (!method.DeclaringType.HasGenericParameters && !method.HasGenericParameters)
				{
					MetadataCollector.RuntimeInvokerCollectorAdder.Add(method);
				}
			});
		}

		private static KeyValuePair<int, uint> CreateRGCTXEntry(RuntimeGenericData data)
		{
			RuntimeGenericInflatedTypeData runtimeGenericInflatedTypeData = data as RuntimeGenericInflatedTypeData;
			RuntimeGenericMethodData runtimeGenericMethodData = data as RuntimeGenericMethodData;
			KeyValuePair<int, uint> result;
			switch (data.InfoType)
			{
			case RuntimeGenericContextInfo.Class:
			case RuntimeGenericContextInfo.Static:
				result = new KeyValuePair<int, uint>(2, (uint)MetadataCollector.Il2CppTypeCollector.GetOrCreateIndex(runtimeGenericInflatedTypeData.Data, 0));
				break;
			case RuntimeGenericContextInfo.Type:
				result = new KeyValuePair<int, uint>(1, (uint)MetadataCollector.Il2CppTypeCollector.GetOrCreateIndex(runtimeGenericInflatedTypeData.Data, 0));
				break;
			case RuntimeGenericContextInfo.Array:
			{
				ArrayType type = new ArrayType(runtimeGenericInflatedTypeData.Data, 1);
				result = new KeyValuePair<int, uint>(2, (uint)MetadataCollector.Il2CppTypeCollector.GetOrCreateIndex(type, 0));
				break;
			}
			case RuntimeGenericContextInfo.Method:
				MetadataCollector.Il2CppGenericMethodCollectorWriter.Add(runtimeGenericMethodData.Data);
				result = new KeyValuePair<int, uint>(3, MetadataCollector.Il2CppGenericMethodCollectorReader.GetIndex(runtimeGenericMethodData.Data));
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return result;
		}

		private void AddParameters(IEnumerable<ParameterDefinition> parameters)
		{
			ParameterDefinition[] array = parameters.ToArray<ParameterDefinition>();
			MetadataCollector.AddUnique<ParameterDefinition>(this._parameters, array, delegate(ParameterDefinition parameter)
			{
				this.AddString(parameter.Name);
				MetadataCollector.Il2CppTypeCollector.Add(parameter.ParameterType, (int)parameter.Attributes);
			});
			MetadataCollector.AddUnique<ParameterDefaultValue>(this._parameterDefaultValues, MetadataCollector.FromParameters(this, MetadataCollector.Il2CppTypeCollector, array), null);
		}

		[DebuggerHidden]
		private static IEnumerable<ParameterDefaultValue> FromParameters(MetadataCollector metadataCollector, IIl2CppTypeCollectorWriterService typeCollector, IEnumerable<ParameterDefinition> parameters)
		{
			MetadataCollector.<FromParameters>c__Iterator2 <FromParameters>c__Iterator = new MetadataCollector.<FromParameters>c__Iterator2();
			<FromParameters>c__Iterator.parameters = parameters;
			<FromParameters>c__Iterator.metadataCollector = metadataCollector;
			<FromParameters>c__Iterator.typeCollector = typeCollector;
			MetadataCollector.<FromParameters>c__Iterator2 expr_1C = <FromParameters>c__Iterator;
			expr_1C.$PC = -2;
			return expr_1C;
		}

		public void AddGenericContainer(IGenericParameterProvider container)
		{
			MetadataCollector.AddUnique<IGenericParameterProvider>(this._genericContainers, container, null);
		}

		public void AddGenericParameters(IEnumerable<GenericParameter> genericParameters)
		{
			MetadataCollector.AddUnique<GenericParameter>(this._genericParameters, genericParameters, delegate(GenericParameter genericParameter)
			{
				this.AddString(genericParameter.Name);
				if (genericParameter.Constraints.Count > 0)
				{
					this._genericParameterConstraintsStart.Add(genericParameter, this._genericParameterConstraints.Count);
					this._genericParameterConstraints.AddRange(genericParameter.Constraints.Select((TypeReference a, int b) => MetadataCollector.Il2CppTypeCollector.GetOrCreateIndex(a, b)));
				}
			});
		}

		private static void AddUnique<T>(Dictionary<T, int> items, IEnumerable<T> itemsToAdd, Action<T> onAdd = null)
		{
			foreach (T current in itemsToAdd)
			{
				MetadataCollector.AddUnique<T>(items, current, onAdd);
			}
		}

		private static void AddUnique<T>(Dictionary<T, int> items, T item, Action<T> onAdd = null)
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

		private static void AddUnique<T>(List<T> items, IEnumerable<T> itemsToAdd)
		{
			foreach (T current in itemsToAdd)
			{
				MetadataCollector.AddUnique<T>(items, current);
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

		public ReadOnlyCollection<byte> GetStringData()
		{
			return this._stringData.AsReadOnly();
		}

		public int GetStringIndex(string str)
		{
			return this._strings[str];
		}

		public ReadOnlyCollection<FieldDefinition> GetFields()
		{
			return (from v in this._fields
			orderby v.Value
			select v.Key).ToArray<FieldDefinition>().AsReadOnlyPortable<FieldDefinition>();
		}

		public int GetFieldIndex(FieldDefinition field)
		{
			return this._fields[field];
		}

		public int AddDefaultValueData(byte[] data)
		{
			int count = this._defaultValueData.Count;
			this._defaultValueData.AddRange(data);
			return count;
		}

		public ReadOnlyCollection<FieldDefaultValue> GetFieldDefaultValues()
		{
			return (from v in this._fieldDefaultValues
			orderby v.Value
			select v.Key).ToArray<FieldDefaultValue>().AsReadOnlyPortable<FieldDefaultValue>();
		}

		public ReadOnlyCollection<ParameterDefaultValue> GetParameterDefaultValues()
		{
			return (from v in this._parameterDefaultValues
			orderby v.Value
			select v.Key).ToArray<ParameterDefaultValue>().AsReadOnlyPortable<ParameterDefaultValue>();
		}

		public ReadOnlyCollection<byte> GetDefaultValueData()
		{
			return this._defaultValueData.AsReadOnly();
		}

		public ReadOnlyCollection<FieldMarshaledSize> GetFieldMarshaledSizes()
		{
			return this._fieldMarshaledSizes.ToArray().AsReadOnlyPortable<FieldMarshaledSize>();
		}

		public ReadOnlyCollection<TypeDefinition> GetTypeInfos()
		{
			return (from v in this._typeInfos
			orderby v.Value
			select v.Key).ToArray<TypeDefinition>().AsReadOnlyPortable<TypeDefinition>();
		}

		public int GetTypeInfoIndex(TypeDefinition type)
		{
			return this._typeInfos[type];
		}

		public ReadOnlyCollection<MethodDefinition> GetMethods()
		{
			return (from v in this._methods
			orderby v.Value
			select v.Key).ToArray<MethodDefinition>().AsReadOnlyPortable<MethodDefinition>();
		}

		public int GetMethodIndex(MethodDefinition method)
		{
			return this._methods[method];
		}

		public ReadOnlyCollection<ParameterDefinition> GetParameters()
		{
			return (from v in this._parameters
			orderby v.Value
			select v.Key).ToArray<ParameterDefinition>().AsReadOnlyPortable<ParameterDefinition>();
		}

		public int GetParameterIndex(ParameterDefinition parameter)
		{
			return this._parameters[parameter];
		}

		public ReadOnlyCollection<PropertyDefinition> GetProperties()
		{
			return (from v in this._properties
			orderby v.Value
			select v.Key).ToArray<PropertyDefinition>().AsReadOnlyPortable<PropertyDefinition>();
		}

		public int GetPropertyIndex(PropertyDefinition property)
		{
			return this._properties[property];
		}

		public ReadOnlyCollection<EventDefinition> GetEvents()
		{
			return (from v in this._events
			orderby v.Value
			select v.Key).ToArray<EventDefinition>().AsReadOnlyPortable<EventDefinition>();
		}

		public int GetEventIndex(EventDefinition @event)
		{
			return this._events[@event];
		}

		public ReadOnlyCollection<IGenericParameterProvider> GetGenericContainers()
		{
			return (from v in this._genericContainers
			orderby v.Value
			select v.Key).ToArray<IGenericParameterProvider>().AsReadOnlyPortable<IGenericParameterProvider>();
		}

		public int GetGenericContainerIndex(IGenericParameterProvider container)
		{
			int num;
			int result;
			if (this._genericContainers.TryGetValue(container, out num))
			{
				result = num;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		public ReadOnlyCollection<GenericParameter> GetGenericParameters()
		{
			return (from v in this._genericParameters
			orderby v.Value
			select v.Key).ToArray<GenericParameter>().AsReadOnlyPortable<GenericParameter>();
		}

		public int GetGenericParameterIndex(GenericParameter genericParameter)
		{
			return this._genericParameters[genericParameter];
		}

		public ReadOnlyCollection<int> GetGenericParameterConstraints()
		{
			return this._genericParameterConstraints.ToArray().AsReadOnlyPortable<int>();
		}

		public int GetGenericParameterConstraintsStartIndex(GenericParameter genericParameter)
		{
			return this._genericParameterConstraintsStart[genericParameter];
		}

		public ReadOnlyCollection<int> GetNestedTypes()
		{
			return this._nestedTypes.ToArray().AsReadOnlyPortable<int>();
		}

		public int GetNestedTypesStartIndex(TypeDefinition type)
		{
			return this._nestedTypesStart[type];
		}

		public ReadOnlyCollection<int> GetInterfaces()
		{
			return this._interfaces.ToArray().AsReadOnlyPortable<int>();
		}

		public int GetInterfacesStartIndex(TypeDefinition type)
		{
			return this._interfacesStart[type];
		}

		public ReadOnlyCollection<uint> GetVTableMethods()
		{
			return this._vtableMethods.ToArray().AsReadOnlyPortable<uint>();
		}

		public int GetVTableMethodsStartIndex(TypeDefinition type)
		{
			int num;
			int result;
			if (this._vtableMethodsStart.TryGetValue(type, out num))
			{
				result = num;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		public ReadOnlyCollection<KeyValuePair<int, int>> GetInterfaceOffsets()
		{
			return this._interfaceOffsets.ToArray().AsReadOnlyPortable<KeyValuePair<int, int>>();
		}

		public int GetInterfaceOffsetsStartIndex(TypeDefinition type)
		{
			return this._interfaceOffsetsStart[type];
		}

		public ReadOnlyCollection<KeyValuePair<int, uint>> GetRGCTXEntries()
		{
			return this._rgctxEntries.ToArray().AsReadOnlyPortable<KeyValuePair<int, uint>>();
		}

		public int GetRGCTXEntriesStartIndex(IGenericParameterProvider provider)
		{
			int num;
			int result;
			if (this._rgctxEntriesStart.TryGetValue(provider, out num))
			{
				result = num;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		public int GetRGCTXEntriesCount(IGenericParameterProvider provider)
		{
			int num;
			int result;
			if (this._rgctxEntriesCount.TryGetValue(provider, out num))
			{
				result = num;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public ReadOnlyCollection<ModuleDefinition> GetModules()
		{
			return (from v in this._modules
			orderby v.Value
			select v.Key).ToArray<ModuleDefinition>().AsReadOnlyPortable<ModuleDefinition>();
		}

		public int GetModuleIndex(ModuleDefinition module)
		{
			return this._modules[module];
		}

		public ReadOnlyCollection<AssemblyDefinition> GetAssemblies()
		{
			return (from v in this._assemblies
			orderby v.Value
			select v.Key).ToArray<AssemblyDefinition>().AsReadOnlyPortable<AssemblyDefinition>();
		}

		public int GetAssemblyIndex(AssemblyDefinition assembly)
		{
			return this._assemblies[assembly];
		}

		public ReadOnlyCollection<int> GetReferencedAssemblyIndiciesIntoAssemblyTable()
		{
			return this._referencedAssemblyTable.AsReadOnly();
		}

		public ReadOnlyCollection<TypeDefinition> GetTypesWithGuids()
		{
			return (from kv in this._guids
			orderby kv.Value
			select kv.Key).ToArray<TypeDefinition>().AsReadOnlyPortable<TypeDefinition>();
		}

		private void AddReferencedAssemblyMetadata(ICollection<AssemblyDefinition> assemblies)
		{
			foreach (AssemblyDefinition current in assemblies)
			{
				List<AssemblyDefinition> list = MetadataCollector.GetReferencedAssembliesFor(current).ToList<AssemblyDefinition>();
				if (list.Count == 0)
				{
					this._firstReferencedAssemblyIndexCache.Add(current, new Tuple<int, int>(-1, 0));
				}
				else
				{
					this._firstReferencedAssemblyIndexCache.Add(current, new Tuple<int, int>(this._referencedAssemblyTable.Count, list.Count));
					this._referencedAssemblyTable.AddRange(list.Distinct<AssemblyDefinition>().Select(new Func<AssemblyDefinition, int>(this.GetAssemblyIndex)));
				}
			}
		}

		public int GetFirstIndexInReferencedAssemblyTableForAssembly(AssemblyDefinition assembly, out int length)
		{
			Tuple<int, int> tuple = this._firstReferencedAssemblyIndexCache[assembly];
			length = tuple.Item2;
			return tuple.Item1;
		}

		public int GetGuidIndex(TypeDefinition type)
		{
			int num;
			int result;
			if (this._guids.TryGetValue(type, out num))
			{
				result = num;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		private static IEnumerable<AssemblyDefinition> GetReferencedAssembliesFor(AssemblyDefinition assembly)
		{
			return from a in assembly.MainModule.AssemblyReferences
			select assembly.MainModule.AssemblyResolver.Resolve(a);
		}
	}
}
