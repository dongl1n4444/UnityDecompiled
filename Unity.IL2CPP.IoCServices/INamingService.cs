using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.IoCServices
{
	public interface INamingService
	{
		string ForIntPtrT
		{
			get;
		}

		string ForUIntPtrT
		{
			get;
		}

		string UIntPtrPointerField
		{
			get;
		}

		string IntPtrValueField
		{
			get;
		}

		string ThisParameterName
		{
			get;
		}

		string Null
		{
			get;
		}

		string ForType(TypeReference typeReference);

		string ForTypeNameOnly(TypeReference type);

		string ForCustomAttributesCacheGenerator(TypeDefinition typeDefinition);

		string ForCustomAttributesCacheGenerator(FieldDefinition fieldDefinition);

		string ForCustomAttributesCacheGenerator(MethodDefinition methodDefinition);

		string ForCustomAttributesCacheGenerator(PropertyDefinition propertyDefinition);

		string ForCustomAttributesCacheGenerator(EventDefinition eventDefinition);

		string ForCustomAttributesCacheGenerator(ParameterDefinition parameterDefinition, MethodDefinition method);

		string ForCustomAttributesCacheGenerator(AssemblyDefinition assemblyDefinition);

		TypeReference RemoveModifiers(TypeReference typeReference);

		string ForVariable(TypeReference variableType);

		string ForMethod(MethodReference method);

		string ForMethodAdjustorThunk(MethodReference method);

		string ForMethodNameOnly(MethodReference method);

		string ForVariableName(VariableReference variable);

		string ForFieldPadding(FieldReference field);

		int GetFieldIndex(FieldReference field, bool includeBase = false);

		string ForField(FieldReference field);

		string ForFieldOffsetGetter(FieldReference field);

		string ForFieldGetter(FieldReference field);

		string ForFieldAddressGetter(FieldReference field);

		string ForFieldSetter(FieldReference field);

		string ForArrayItems();

		string ForArrayItemGetter(bool useArrayBoundsCheck);

		string ForArrayItemAddressGetter(bool useArrayBoundsCheck);

		string ForArrayItemSetter(bool useArrayBoundsCheck);

		string ForArrayIndexType();

		string ForArrayIndexName();

		string Clean(string name);

		string ForFile(TypeDefinition type);

		string ForInitializedTypeInfo(string argument);

		string ForIl2CppType(TypeReference type, int attrs = 0);

		string ForGenericInst(IList<TypeReference> types);

		string ForGenericClass(TypeReference type);

		string ForStaticFieldsStruct(TypeReference type);

		string ForThreadFieldsStruct(TypeReference type);

		string ForDebugMethodInfo(MethodReference method);

		string ForDebugMethodInfoOffsetTable(MethodReference method);

		string ForDebugMethodLocalInfo(VariableDefinition variable, MethodReference method);

		string ForDebugLocalInfo(MethodReference method);

		string ForParameterName(TypeReference type, int index);

		string ForParameterName(ParameterReference parameterReference);

		string ForCreateStringMethod(MethodReference method);

		string ForDebugTypeInfos(TypeReference type);

		string ForArrayType(ArrayType type);

		string ForAssembly(AssemblyDefinition assembly);

		string ForAssemblyScope(AssemblyDefinition assembly, string symbol);

		string ModuleNameToPrependString(string name);

		bool IsSpecialArrayMethod(MethodReference methodReference);

		string ForImage(TypeDefinition type);

		string ForImage(ModuleDefinition module);

		string ForStringLiteralIdentifier(string literal);

		string AddressOf(string value);

		string Dereference(string value);

		string ForRuntimeIl2CppType(TypeReference type);

		string ForRuntimeTypeInfo(TypeReference type);

		string ForRuntimeMethodInfo(MethodReference method);

		string ForRuntimeFieldInfo(FieldReference field);

		string ForPadding(TypeDefinition typeDefinition);

		string ForComTypeInterfaceFieldName(TypeReference interfaceType);

		string ForComTypeInterfaceFieldGetter(TypeReference interfaceType);

		string ForInteropInterfaceVariable(TypeReference interfaceType);

		string ForInteropHResultVariable();

		string ForInteropReturnValue();

		string ForComInterfaceReturnParameterName();

		string ForPInvokeFunctionPointerTypedef();

		string ForPInvokeFunctionPointerVariable();

		string ForDelegatePInvokeWrapper(TypeReference type);

		string ForReversePInvokeWrapperMethod(MethodReference method);

		string ForIl2CppComObjectIdentityField();

		string ForWindowsRuntimeDelegateComCallableWrapperClass(TypeReference delegateType);

		string ForWindowsRuntimeDelegateComCallableWrapperInterface(TypeReference delegateType);

		string ForWindowsRuntimeDelegateNativeInvokerMethod(MethodReference invokeMethod);

		string ForCreateComCallableWrapperFunction(TypeReference type);
	}
}
