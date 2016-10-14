using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.IL2CPP.Metadata.Fields;

namespace Unity.IL2CPP.Metadata
{
	public interface IMetadataCollection
	{
		ReadOnlyCollection<EventDefinition> GetEvents();

		ReadOnlyCollection<FieldDefinition> GetFields();

		ReadOnlyCollection<FieldDefaultValue> GetFieldDefaultValues();

		ReadOnlyCollection<byte> GetDefaultValueData();

		ReadOnlyCollection<MethodDefinition> GetMethods();

		ReadOnlyCollection<ParameterDefinition> GetParameters();

		ReadOnlyCollection<PropertyDefinition> GetProperties();

		ReadOnlyCollection<byte> GetStringData();

		ReadOnlyCollection<TypeDefinition> GetTypeInfos();

		ReadOnlyCollection<IGenericParameterProvider> GetGenericContainers();

		ReadOnlyCollection<GenericParameter> GetGenericParameters();

		ReadOnlyCollection<int> GetGenericParameterConstraints();

		ReadOnlyCollection<ParameterDefaultValue> GetParameterDefaultValues();

		ReadOnlyCollection<FieldMarshaledSize> GetFieldMarshaledSizes();

		ReadOnlyCollection<int> GetNestedTypes();

		ReadOnlyCollection<int> GetInterfaces();

		ReadOnlyCollection<uint> GetVTableMethods();

		ReadOnlyCollection<KeyValuePair<int, int>> GetInterfaceOffsets();

		ReadOnlyCollection<KeyValuePair<int, uint>> GetRGCTXEntries();

		ReadOnlyCollection<ModuleDefinition> GetModules();

		ReadOnlyCollection<AssemblyDefinition> GetAssemblies();

		ReadOnlyCollection<int> GetReferencedAssemblyIndiciesIntoAssemblyTable();

		ReadOnlyCollection<TypeDefinition> GetTypesWithGuids();

		int GetTypeInfoIndex(TypeDefinition type);

		int GetEventIndex(EventDefinition @event);

		int GetFieldIndex(FieldDefinition field);

		int GetMethodIndex(MethodDefinition method);

		int GetParameterIndex(ParameterDefinition parameter);

		int GetPropertyIndex(PropertyDefinition property);

		int GetStringIndex(string str);

		int GetGenericContainerIndex(IGenericParameterProvider container);

		int GetGenericParameterIndex(GenericParameter genericParameter);

		int GetGenericParameterConstraintsStartIndex(GenericParameter genericParameter);

		int GetNestedTypesStartIndex(TypeDefinition type);

		int GetInterfacesStartIndex(TypeDefinition type);

		int GetVTableMethodsStartIndex(TypeDefinition type);

		int GetInterfaceOffsetsStartIndex(TypeDefinition type);

		int GetRGCTXEntriesStartIndex(IGenericParameterProvider provider);

		int GetRGCTXEntriesCount(IGenericParameterProvider provider);

		int GetModuleIndex(ModuleDefinition module);

		int GetAssemblyIndex(AssemblyDefinition assembly);

		int GetFirstIndexInReferencedAssemblyTableForAssembly(AssemblyDefinition assembly, out int length);

		int GetGuidIndex(TypeDefinition type);
	}
}
