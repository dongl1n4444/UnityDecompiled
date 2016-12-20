namespace Unity.IL2CPP.Metadata
{
    using Mono.Cecil;
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.InteropServices;

    public interface IMetadataCollection
    {
        ReadOnlyCollection<AssemblyDefinition> GetAssemblies();
        int GetAssemblyIndex(AssemblyDefinition assembly);
        ReadOnlyCollection<byte> GetDefaultValueData();
        int GetEventIndex(EventDefinition @event);
        ReadOnlyCollection<EventDefinition> GetEvents();
        ReadOnlyCollection<FieldDefaultValue> GetFieldDefaultValues();
        int GetFieldIndex(FieldDefinition field);
        ReadOnlyCollection<FieldMarshaledSize> GetFieldMarshaledSizes();
        ReadOnlyCollection<FieldDefinition> GetFields();
        int GetFirstIndexInReferencedAssemblyTableForAssembly(AssemblyDefinition assembly, out int length);
        int GetGenericContainerIndex(IGenericParameterProvider container);
        ReadOnlyCollection<IGenericParameterProvider> GetGenericContainers();
        ReadOnlyCollection<int> GetGenericParameterConstraints();
        int GetGenericParameterConstraintsStartIndex(GenericParameter genericParameter);
        int GetGenericParameterIndex(GenericParameter genericParameter);
        ReadOnlyCollection<GenericParameter> GetGenericParameters();
        int GetGuidIndex(TypeDefinition type);
        ReadOnlyCollection<KeyValuePair<int, int>> GetInterfaceOffsets();
        int GetInterfaceOffsetsStartIndex(TypeDefinition type);
        ReadOnlyCollection<int> GetInterfaces();
        int GetInterfacesStartIndex(TypeDefinition type);
        int GetMethodIndex(MethodDefinition method);
        ReadOnlyCollection<MethodDefinition> GetMethods();
        int GetModuleIndex(ModuleDefinition module);
        ReadOnlyCollection<ModuleDefinition> GetModules();
        ReadOnlyCollection<int> GetNestedTypes();
        int GetNestedTypesStartIndex(TypeDefinition type);
        ReadOnlyCollection<ParameterDefaultValue> GetParameterDefaultValues();
        int GetParameterIndex(ParameterDefinition parameter);
        ReadOnlyCollection<ParameterDefinition> GetParameters();
        ReadOnlyCollection<PropertyDefinition> GetProperties();
        int GetPropertyIndex(PropertyDefinition property);
        ReadOnlyCollection<int> GetReferencedAssemblyIndiciesIntoAssemblyTable();
        ReadOnlyCollection<KeyValuePair<int, uint>> GetRGCTXEntries();
        int GetRGCTXEntriesCount(IGenericParameterProvider provider);
        int GetRGCTXEntriesStartIndex(IGenericParameterProvider provider);
        ReadOnlyCollection<byte> GetStringData();
        int GetStringIndex(string str);
        int GetTypeInfoIndex(TypeDefinition type);
        ReadOnlyCollection<TypeDefinition> GetTypeInfos();
        ReadOnlyCollection<TypeDefinition> GetTypesWithGuids();
        ReadOnlyCollection<uint> GetVTableMethods();
        int GetVTableMethodsStartIndex(TypeDefinition type);
    }
}

