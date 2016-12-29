namespace Unity.IL2CPP.IoCServices
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public interface INamingService
    {
        string AddressOf(string value);
        string Clean(string name);
        string Dereference(string value);
        string ForArrayIndexName();
        string ForArrayIndexType();
        string ForArrayItemAddressGetter(bool useArrayBoundsCheck);
        string ForArrayItemGetter(bool useArrayBoundsCheck);
        string ForArrayItems();
        string ForArrayItemSetter(bool useArrayBoundsCheck);
        string ForArrayType(ArrayType type);
        string ForAssembly(AssemblyDefinition assembly);
        string ForAssemblyScope(AssemblyDefinition assembly, string symbol);
        string ForComInterfaceReturnParameterName();
        string ForComTypeInterfaceFieldGetter(TypeReference interfaceType);
        string ForComTypeInterfaceFieldName(TypeReference interfaceType);
        string ForCreateComCallableWrapperFunction(TypeReference type);
        string ForCreateStringMethod(MethodReference method);
        string ForCustomAttributesCacheGenerator(AssemblyDefinition assemblyDefinition);
        string ForCustomAttributesCacheGenerator(EventDefinition eventDefinition);
        string ForCustomAttributesCacheGenerator(FieldDefinition fieldDefinition);
        string ForCustomAttributesCacheGenerator(MethodDefinition methodDefinition);
        string ForCustomAttributesCacheGenerator(PropertyDefinition propertyDefinition);
        string ForCustomAttributesCacheGenerator(TypeDefinition typeDefinition);
        string ForCustomAttributesCacheGenerator(ParameterDefinition parameterDefinition, MethodDefinition method);
        string ForDebugLocalInfo(MethodReference method);
        string ForDebugMethodInfo(MethodReference method);
        string ForDebugMethodInfoOffsetTable(MethodReference method);
        string ForDebugMethodLocalInfo(VariableDefinition variable, MethodReference method);
        string ForDebugTypeInfos(TypeReference type);
        string ForDelegatePInvokeWrapper(TypeReference type);
        string ForField(FieldReference field);
        string ForFieldAddressGetter(FieldReference field);
        string ForFieldGetter(FieldReference field);
        string ForFieldOffsetGetter(FieldReference field);
        string ForFieldPadding(FieldReference field);
        string ForFieldSetter(FieldReference field);
        string ForFile(TypeDefinition type);
        string ForGenericClass(TypeReference type);
        string ForGenericInst(IList<TypeReference> types);
        string ForIl2CppComObjectIdentityField();
        string ForIl2CppType(TypeReference type, int attrs = 0);
        string ForImage(ModuleDefinition module);
        string ForImage(TypeDefinition type);
        string ForInitializedTypeInfo(string argument);
        string ForInteropHResultVariable();
        string ForInteropInterfaceVariable(TypeReference interfaceType);
        string ForInteropReturnValue();
        string ForMethod(MethodReference method);
        string ForMethodAdjustorThunk(MethodReference method);
        string ForMethodNameOnly(MethodReference method);
        string ForPadding(TypeDefinition typeDefinition);
        string ForParameterName(ParameterReference parameterReference);
        string ForParameterName(TypeReference type, int index);
        string ForPInvokeFunctionPointerTypedef();
        string ForPInvokeFunctionPointerVariable();
        string ForReversePInvokeWrapperMethod(MethodReference method);
        string ForRuntimeFieldInfo(FieldReference field);
        string ForRuntimeIl2CppType(TypeReference type);
        string ForRuntimeMethodInfo(MethodReference method);
        string ForRuntimeTypeInfo(TypeReference type);
        string ForStaticFieldsStruct(TypeReference type);
        string ForStringLiteralIdentifier(string literal);
        string ForThreadFieldsStruct(TypeReference type);
        string ForType(TypeReference typeReference);
        string ForTypeNameOnly(TypeReference type);
        string ForVariable(TypeReference variableType);
        string ForVariableName(VariableReference variable);
        string ForWindowsRuntimeDelegateComCallableWrapperClass(TypeReference delegateType);
        string ForWindowsRuntimeDelegateComCallableWrapperInterface(TypeReference delegateType);
        string ForWindowsRuntimeDelegateNativeInvokerMethod(MethodReference invokeMethod);
        int GetFieldIndex(FieldReference field, bool includeBase = false);
        bool IsSpecialArrayMethod(MethodReference methodReference);
        string ModuleNameToPrependString(string name);
        TypeReference RemoveModifiers(TypeReference typeReference);

        string ForIntPtrT { get; }

        string ForUIntPtrT { get; }

        string IntPtrValueField { get; }

        string Null { get; }

        string ThisParameterName { get; }

        string UIntPtrPointerField { get; }
    }
}

