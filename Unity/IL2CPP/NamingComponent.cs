namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Collections.Generic;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class NamingComponent : INamingService, IDisposable
    {
        private const string _intPtrValueField = "m_value";
        private readonly HashCodeCache<MethodReference> _methodHashCache;
        private const string _null = "NULL";
        private readonly HashCodeCache<string> _stringLiteralHashCache;
        private const string _thisParameterName = "__this";
        private readonly HashCodeCache<TypeReference> _typeHashCache;
        private const string _uIntPtrPointerField = "_pointer";
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<string, string, string> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Action<uint> <>f__am$cache3;
        [CompilerGenerated]
        private static Action<uint> <>f__am$cache4;
        [CompilerGenerated]
        private static Action<uint> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<TypeReference, uint> <>f__mg$cache0;
        [CompilerGenerated]
        private static Func<MethodReference, uint> <>f__mg$cache1;
        [CompilerGenerated]
        private static Func<string, uint> <>f__mg$cache2;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map2;
        private readonly Dictionary<string, string> CleanNamesCache = new Dictionary<string, string>();
        private readonly StringBuilder CleanStringBuilder;
        private readonly Dictionary<MethodReference, string> ForMethodNameOnlyCache = new Dictionary<MethodReference, string>(new Unity.IL2CPP.Common.MethodReferenceComparer());
        private readonly Dictionary<string, string> ForStringLiteralCache = new Dictionary<string, string>();
        private readonly Dictionary<TypeReference, string> ForTypeNameOnlyCache = new Dictionary<TypeReference, string>(new Unity.IL2CPP.Common.TypeReferenceEqualityComparer());
        private readonly MetadataNames MemberNames = new MetadataNames();
        [Inject]
        public static IStatsService StatsService;
        [Inject]
        public static ITypeProviderService TypeProvider;

        public NamingComponent()
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new Func<TypeReference, uint>(null, (IntPtr) SemiUniqueStableTokenGenerator.GenerateFor);
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = new Action<uint>(NamingComponent.<_typeHashCache>m__3);
            }
            this._typeHashCache = new HashCodeCache<TypeReference>(<>f__mg$cache0, <>f__am$cache3, new Unity.IL2CPP.Common.TypeReferenceEqualityComparer());
            if (<>f__mg$cache1 == null)
            {
                <>f__mg$cache1 = new Func<MethodReference, uint>(null, (IntPtr) SemiUniqueStableTokenGenerator.GenerateFor);
            }
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = new Action<uint>(NamingComponent.<_methodHashCache>m__4);
            }
            this._methodHashCache = new HashCodeCache<MethodReference>(<>f__mg$cache1, <>f__am$cache4, new Unity.IL2CPP.Common.MethodReferenceComparer());
            if (<>f__mg$cache2 == null)
            {
                <>f__mg$cache2 = new Func<string, uint>(null, (IntPtr) SemiUniqueStableTokenGenerator.GenerateFor);
            }
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = new Action<uint>(NamingComponent.<_stringLiteralHashCache>m__5);
            }
            this._stringLiteralHashCache = new HashCodeCache<string>(<>f__mg$cache2, <>f__am$cache5);
            this.CleanStringBuilder = new StringBuilder();
        }

        [CompilerGenerated]
        private static void <_methodHashCache>m__4(uint notUsed)
        {
            StatsService.MethodHashCollisions++;
        }

        [CompilerGenerated]
        private static void <_stringLiteralHashCache>m__5(uint notUsed)
        {
            StatsService.MethodHashCollisions++;
        }

        [CompilerGenerated]
        private static void <_typeHashCache>m__3(uint notUsed)
        {
            StatsService.TypeHashCollisions++;
        }

        public string AddressOf(string value)
        {
            if (value.StartsWith("*"))
            {
                return value.Substring(1);
            }
            return $"&{value}";
        }

        public string Clean(string name)
        {
            if (this.CleanNamesCache.ContainsKey(name))
            {
                return this.CleanNamesCache[name];
            }
            StringBuilder builder = this.CleanStringBuilder.Clear();
            char[] chArray = name.ToCharArray();
            for (int i = 0; i < chArray.Length; i++)
            {
                char c = chArray[i];
                if (this.IsSafeCharacter(c) || (this.IsAsciiDigit(c) && (i != 0)))
                {
                    builder.Append(c);
                }
                else
                {
                    ushort num2 = Convert.ToUInt16(c);
                    if (num2 < 0xff)
                    {
                        if (((num2 == 0x2e) || (num2 == 0x2f)) || ((num2 == 0x60) || (num2 == 0x5f)))
                        {
                            builder.Append("_");
                        }
                        else
                        {
                            builder.AppendFormat("U{0:X2}", num2);
                        }
                    }
                    else if (num2 < 0xfff)
                    {
                        builder.AppendFormat("U{0:X3}", num2);
                    }
                    else
                    {
                        builder.AppendFormat("U{0:X4}", num2);
                    }
                }
            }
            string str2 = builder.ToString();
            this.CleanNamesCache[name] = str2;
            return str2;
        }

        public string Dereference(string value)
        {
            if (value.StartsWith("&"))
            {
                return value.Substring(1);
            }
            return $"*{value}";
        }

        public void Dispose()
        {
            this.CleanNamesCache.Clear();
            this.ForMethodNameOnlyCache.Clear();
            this.ForTypeNameOnlyCache.Clear();
            this.ForStringLiteralCache.Clear();
            this._typeHashCache.Clear();
            this._methodHashCache.Clear();
            this._stringLiteralHashCache.Clear();
        }

        private string EscapeKeywords(string fieldName) => 
            ("___" + fieldName);

        public string ForArrayIndexName() => 
            "index";

        public string ForArrayIndexType() => 
            "il2cpp_array_size_t";

        public string ForArrayItemAddressGetter(bool useArrayBoundsCheck) => 
            (!useArrayBoundsCheck ? "GetAddressAtUnchecked" : "GetAddressAt");

        public string ForArrayItemGetter(bool useArrayBoundsCheck) => 
            (!useArrayBoundsCheck ? "GetAtUnchecked" : "GetAt");

        public string ForArrayItems() => 
            "m_Items";

        public string ForArrayItemSetter(bool useArrayBoundsCheck) => 
            (!useArrayBoundsCheck ? "SetAtUnchecked" : "SetAt");

        public string ForArrayType(ArrayType type) => 
            (this.ForTypeNameOnly(type) + "_ArrayType");

        public string ForAssembly(AssemblyDefinition assembly) => 
            $"g_{this.Clean(assembly.Name.Name)}_Assembly";

        public string ForAssemblyScope(AssemblyDefinition assembly, string symbol) => 
            $"{this.ForAssembly(assembly)}_{symbol}";

        public string ForComInterfaceReturnParameterName() => 
            "comReturnValue";

        public string ForComTypeInterfaceFieldGetter(TypeReference interfaceType) => 
            ("get_" + this.ForInteropInterfaceVariable(interfaceType));

        public string ForComTypeInterfaceFieldName(TypeReference interfaceType) => 
            this.ForInteropInterfaceVariable(interfaceType);

        public string ForCreateComCallableWrapperFunction(TypeReference type) => 
            (this.ForTypeNameOnly(type) + "_create_ccw");

        public string ForCreateStringMethod(MethodReference method)
        {
            if (method.DeclaringType.Name != "String")
            {
                throw new Exception("method.DeclaringType.Name != \"String\"");
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<MethodDefinition, bool>(null, (IntPtr) <ForCreateStringMethod>m__2);
            }
            foreach (MethodDefinition definition in method.DeclaringType.Resolve().Methods.Where<MethodDefinition>(<>f__am$cache2))
            {
                if (definition.Parameters.Count == method.Parameters.Count)
                {
                    bool flag = false;
                    for (int i = 0; i < definition.Parameters.Count; i++)
                    {
                        if (definition.Parameters[i].ParameterType.FullName != method.Parameters[i].ParameterType.FullName)
                        {
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        return this.ForMethodNameOnly(definition);
                    }
                }
            }
            throw new Exception($"Can't find proper CreateString : {method.FullName}");
        }

        public string ForCustomAttributesCacheGenerator(AssemblyDefinition assemblyDefinition) => 
            $"{this.ForAssembly(assemblyDefinition)}_CustomAttributesCacheGenerator";

        public string ForCustomAttributesCacheGenerator(EventDefinition eventDefinition) => 
            $"{this.ForCustomAttributesCacheGenerator(eventDefinition.DeclaringType)}_{this.ForEventInfo(eventDefinition)}";

        public string ForCustomAttributesCacheGenerator(FieldDefinition fieldDefinition) => 
            $"{this.ForCustomAttributesCacheGenerator(fieldDefinition.DeclaringType)}_{this.Clean(fieldDefinition.Name)}";

        public string ForCustomAttributesCacheGenerator(MethodDefinition methodDefinition) => 
            $"{this.ForCustomAttributesCacheGenerator(methodDefinition.DeclaringType)}_{this.ForMethodNameOnly(methodDefinition)}";

        public string ForCustomAttributesCacheGenerator(PropertyDefinition propertyDefinition) => 
            $"{this.ForCustomAttributesCacheGenerator(propertyDefinition.DeclaringType)}_{this.ForPropertyInfo(propertyDefinition)}";

        public string ForCustomAttributesCacheGenerator(TypeDefinition typeDefinition) => 
            $"{this.ForTypeNameOnly(typeDefinition)}_CustomAttributesCacheGenerator";

        public string ForCustomAttributesCacheGenerator(ParameterDefinition parameterDefinition, MethodDefinition method) => 
            $"{this.ForCustomAttributesCacheGenerator(method)}_{this.ForParameterName(parameterDefinition)}";

        public string ForDebugLocalInfo(MethodReference method) => 
            this.ForMethodInfoInternal(method, "DebugLocalInfos");

        public string ForDebugMethodInfo(MethodReference method) => 
            this.ForMethodInfoInternal(method, this.MemberNames.DebugMethodInfo);

        public string ForDebugMethodInfoOffsetTable(MethodReference method) => 
            this.ForMethodInfoInternal(method, this.MemberNames.DebugMethodInfoOffsetTable);

        public string ForDebugMethodLocalInfo(VariableDefinition variable, MethodReference method)
        {
            object[] objArray1 = new object[] { variable.Name, "_", variable.Index, "_DebugLocalInfo" };
            return this.ForMethodInfoInternal(method, string.Concat(objArray1));
        }

        public string ForDebugTypeInfos(TypeReference type) => 
            this.TypeMember(type, this.MemberNames.DebugTypeInfo);

        public string ForDelegatePInvokeWrapper(TypeReference type) => 
            ("DelegatePInvokeWrapper_" + this.ForType(type));

        private string ForEventInfo(EventDefinition ev) => 
            this.TypeMember(ev.DeclaringType, this.Clean(this.EscapeKeywords(ev.Name)) + "_EventInfo");

        public string ForField(FieldReference field) => 
            (this.Clean(this.EscapeKeywords(field.Name)) + "_" + this.GetFieldIndex(field, true));

        public string ForFieldAddressGetter(FieldReference field) => 
            ($"get_address_of_{this.Clean(field.Name)}_" + this.GetFieldIndex(field, true));

        public string ForFieldGetter(FieldReference field) => 
            ($"get_{this.Clean(field.Name)}_" + this.GetFieldIndex(field, true));

        private string ForFieldInfo(FieldReference field) => 
            this.TypeMember(field.DeclaringType, this.ForField(field) + "_FieldInfo");

        public string ForFieldOffsetGetter(FieldReference field) => 
            ($"get_offset_of_{this.Clean(field.Name)}_" + this.GetFieldIndex(field, true));

        public string ForFieldPadding(FieldReference field) => 
            (this.ForField(field) + "_OffsetPadding");

        public string ForFieldSetter(FieldReference field) => 
            ($"set_{this.Clean(field.Name)}_" + this.GetFieldIndex(field, true));

        public string ForFile(TypeDefinition type) => 
            (this.ModuleNameToPrependString(type.Module.Name) + "_" + this.Clean(type.FullName));

        public string ForGenericClass(TypeReference type) => 
            this.TypeMember(type, this.MemberNames.GenericClass);

        public string ForGenericInst(IList<TypeReference> types)
        {
            string str = "GenInst";
            for (int i = 0; i < types.Count; i++)
            {
                str = str + "_" + this.ForIl2CppType(types[i], 0);
            }
            return str;
        }

        public string ForIl2CppComObjectIdentityField() => 
            "identity";

        public string ForIl2CppType(TypeReference type, int attrs = 0)
        {
            TypeReference nonPinnedAndNonByReferenceType = type.GetNonPinnedAndNonByReferenceType();
            string str = this.ForType(nonPinnedAndNonByReferenceType);
            GenericParameter parameter = nonPinnedAndNonByReferenceType as GenericParameter;
            if (parameter != null)
            {
                if (parameter.Owner is TypeDefinition)
                {
                    str = this.ForTypeNameOnly((TypeReference) parameter.Owner) + "_gp_" + parameter.Position;
                }
                else
                {
                    str = this.ForMethodNameOnly((MethodReference) parameter.Owner) + "_gp_" + parameter.Position;
                }
            }
            object[] objArray1 = new object[] { str, "_", !type.IsByReference ? 0 : 1, "_", !type.IsPinned ? 0 : 1, "_", attrs };
            return string.Concat(objArray1);
        }

        public string ForImage(ModuleDefinition module) => 
            $"g_{this.Clean(module.Name)}_Image";

        public string ForImage(TypeDefinition type) => 
            $"g_{this.Clean(type.Module.Name)}_Image";

        public string ForInitializedTypeInfo(string argument) => 
            $"InitializedTypeInfo({argument})";

        public string ForInteropHResultVariable() => 
            "hr";

        public string ForInteropInterfaceVariable(TypeReference interfaceType)
        {
            if (interfaceType.IsIActivationFactory())
            {
                return "activationFactory";
            }
            char[] trimChars = new char[] { '_' };
            string str2 = this.ForTypeNameOnly(interfaceType).TrimStart(trimChars);
            return ("____" + str2.Substring(0, 2).ToLower() + str2.Substring(2));
        }

        public string ForInteropReturnValue() => 
            "returnValue";

        public string ForMethod(MethodReference method) => 
            this.ForMethodNameOnly(method);

        public string ForMethodAdjustorThunk(MethodReference method) => 
            (this.ForMethod(method) + "_AdjustorThunk");

        private string ForMethodInfoInternal(MethodReference method, string suffix) => 
            (this.ForMethodNameOnly(method) + "_" + suffix);

        private string ForMethodInternal(MethodReference method)
        {
            GenericInstanceMethod method2 = method as GenericInstanceMethod;
            StringBuilder builder = new StringBuilder();
            builder.Append(this.Clean(method.DeclaringType.Name));
            builder.Append("_");
            builder.Append(this.Clean(method.Name));
            if (method2 != null)
            {
                foreach (TypeReference reference in method2.GenericArguments)
                {
                    builder.Append("_Tis" + this.ForTypeNameOnly(reference));
                }
            }
            builder.Append("_m");
            builder.Append(this.GenerateUniqueMethodPostFix(method));
            return builder.ToString();
        }

        public string ForMethodNameOnly(MethodReference method)
        {
            string str;
            if (this.ForMethodNameOnlyCache.TryGetValue(method, out str))
            {
                return str;
            }
            MethodReference key = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(method.DeclaringType, method).Resolve(method.Resolve());
            if (this.ForMethodNameOnlyCache.TryGetValue(key, out str))
            {
                return str;
            }
            if (!Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(key.DeclaringType, method.DeclaringType, TypeComparisonMode.Exact))
            {
                return this.ForMethodNameOnly(key);
            }
            string str3 = this.ForMethodInternal(method);
            this.ForMethodNameOnlyCache[method] = str3;
            return str3;
        }

        public string ForPadding(TypeDefinition typeDefinition) => 
            (this.ForType(typeDefinition) + "__padding");

        public string ForParameterName(ParameterReference parameterReference)
        {
            string name = !string.IsNullOrEmpty(parameterReference.Name) ? this.EscapeKeywords(parameterReference.Name) : "p";
            return (this.Clean(name) + parameterReference.Index.ToString(CultureInfo.InvariantCulture));
        }

        public string ForParameterName(TypeReference type, int index)
        {
            string name = !string.IsNullOrEmpty(type.Name) ? this.EscapeKeywords(type.Name) : "p";
            return (this.Clean(name) + index.ToString(CultureInfo.InvariantCulture));
        }

        public string ForPInvokeFunctionPointerTypedef() => 
            "PInvokeFunc";

        public string ForPInvokeFunctionPointerVariable() => 
            "il2cppPInvokeFunc";

        private string ForPropertyInfo(PropertyDefinition property) => 
            this.ForPropertyInfo(property, property.DeclaringType);

        private string ForPropertyInfo(PropertyDefinition property, TypeReference declaringType)
        {
            <ForPropertyInfo>c__AnonStorey0 storey = new <ForPropertyInfo>c__AnonStorey0 {
                property = property,
                $this = this
            };
            string str = this.Clean(this.EscapeKeywords(storey.property.Name));
            if (declaringType.Resolve().Properties.Count<PropertyDefinition>(new Func<PropertyDefinition, bool>(storey, (IntPtr) this.<>m__0)) > 1)
            {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = new Func<string, string, string>(null, (IntPtr) <ForPropertyInfo>m__1);
                }
                str = str + "_" + storey.property.Parameters.Select<ParameterDefinition, string>(new Func<ParameterDefinition, string>(storey, (IntPtr) this.<>m__1)).Aggregate<string>(<>f__am$cache1);
            }
            return this.TypeMember(declaringType, str + "_PropertyInfo");
        }

        public string ForReversePInvokeWrapperMethod(MethodReference method) => 
            ("ReversePInvokeWrapper_" + this.ForMethod(method));

        public string ForRuntimeFieldInfo(FieldReference field) => 
            (this.ForFieldInfo(field) + "_var");

        public string ForRuntimeIl2CppType(TypeReference type) => 
            (this.ForIl2CppType(type, 0) + "_var");

        public string ForRuntimeMethodInfo(MethodReference method) => 
            (this.ForMethodInfoInternal(method, this.MemberNames.MethodInfo) + "_var");

        public string ForRuntimeTypeInfo(TypeReference type) => 
            (this.ForTypeInfo(type) + "_var");

        public string ForStaticFieldsStruct(TypeReference type) => 
            this.TypeMember(type, this.MemberNames.StaticFields);

        public string ForStringLiteralIdentifier(string literal)
        {
            string str;
            if (this.ForStringLiteralCache.TryGetValue(literal, out str))
            {
                return str;
            }
            string str3 = "_stringLiteral" + this.GenerateUniqueStringLiteralPostFix(literal);
            this.ForStringLiteralCache[literal] = str3;
            return str3;
        }

        public string ForThreadFieldsStruct(TypeReference type) => 
            this.TypeMember(type, this.MemberNames.ThreadStaticFields);

        public string ForType(TypeReference typeReference)
        {
            typeReference = this.RemoveModifiers(typeReference);
            return this.ForTypeNameOnly(typeReference);
        }

        private string ForTypeInfo(TypeReference typeReference) => 
            this.TypeMember(typeReference, this.MemberNames.Il2CppClass);

        private string ForTypeMangling(TypeReference typeReference)
        {
            if (typeReference.IsGenericParameter)
            {
                GenericParameter parameter = (GenericParameter) typeReference;
                return (((parameter.MetadataType != MetadataType.Var) ? "mgp" : "tgp") + parameter.Position);
            }
            if (typeReference.IsArray)
            {
                ArrayType type = (ArrayType) typeReference;
                return (this.ForTypeMangling(type.ElementType) + "_arr" + type.Rank);
            }
            if (typeReference.IsGenericInstance)
            {
                GenericInstanceType type2 = (GenericInstanceType) typeReference;
                string str2 = this.ForTypeMangling(type2.ElementType) + "_git_";
                foreach (TypeReference reference in type2.GenericArguments)
                {
                    str2 = str2 + "_" + this.ForTypeMangling(reference);
                }
                return str2;
            }
            string name = this.ForTypeNameOnly(typeReference);
            if (typeReference is ArrayType)
            {
                return (name + "_arr");
            }
            if (typeReference is PointerType)
            {
                return (name + "_ptr");
            }
            if (typeReference is ByReferenceType)
            {
                return (name + "_ref");
            }
            return this.Clean(name);
        }

        private string ForTypeNameInternal(TypeReference typeReference)
        {
            string wellKnownNameFor = this.GetWellKnownNameFor(typeReference);
            if (wellKnownNameFor != null)
            {
                return wellKnownNameFor;
            }
            return (this.Clean(typeReference.Name) + "_t" + this.GenerateUniqueTypePostFix(typeReference));
        }

        public string ForTypeNameOnly(TypeReference type)
        {
            string str;
            if (!this.ForTypeNameOnlyCache.TryGetValue(type, out str))
            {
                str = this.ForTypeNameInternal(type);
                this.ForTypeNameOnlyCache[type] = str;
            }
            return str;
        }

        public string ForVariable(TypeReference variableType)
        {
            variableType = this.RemoveModifiers(variableType);
            ArrayType typeReference = variableType as ArrayType;
            PointerType type2 = variableType as PointerType;
            ByReferenceType type3 = variableType as ByReferenceType;
            if (typeReference != null)
            {
                if ((typeReference.Rank != 1) && (typeReference.Rank <= 1))
                {
                    throw new NotImplementedException("Invalid array rank");
                }
                return $"{this.ForType(typeReference)}*";
            }
            if (type2 != null)
            {
                return (this.ForVariable(type2.ElementType) + "*");
            }
            if (type3 != null)
            {
                return (this.ForVariable(type3.ElementType) + "*");
            }
            switch (variableType.MetadataType)
            {
                case MetadataType.Void:
                    return "void";

                case MetadataType.Boolean:
                    return "bool";

                case MetadataType.Single:
                    return "float";

                case MetadataType.Double:
                    return "double";

                case MetadataType.String:
                    return (this.ForType(variableType) + "*");

                case MetadataType.SByte:
                    return "int8_t";

                case MetadataType.Byte:
                    return "uint8_t";

                case MetadataType.Char:
                    return "Il2CppChar";

                case MetadataType.Int16:
                    return "int16_t";

                case MetadataType.UInt16:
                    return "uint16_t";

                case MetadataType.Int32:
                    return "int32_t";

                case MetadataType.UInt32:
                    return "uint32_t";

                case MetadataType.Int64:
                    return "int64_t";

                case MetadataType.UInt64:
                    return "uint64_t";

                case MetadataType.IntPtr:
                    return this.ForTypeNameOnly(variableType);
            }
            if (variableType.Name == this.ForIntPtrT)
            {
                return this.ForIntPtrT;
            }
            if (variableType.Name == this.ForUIntPtrT)
            {
                return this.ForUIntPtrT;
            }
            if (variableType is GenericParameter)
            {
                throw new ArgumentException("Generic parameter encountered as variable type", "variableType");
            }
            TypeDefinition definition = variableType.Resolve();
            if (definition.IsEnum)
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = new Func<FieldDefinition, bool>(null, (IntPtr) <ForVariable>m__0);
                }
                FieldDefinition definition2 = definition.Fields.Single<FieldDefinition>(<>f__am$cache0);
                return this.ForVariable(definition2.FieldType);
            }
            if (variableType is GenericInstanceType)
            {
                if (variableType.Resolve().IsInterface)
                {
                    return (this.ForType(variableType.Module.TypeSystem.Object) + "*");
                }
                return $"{this.ForTypeNameOnly(variableType)} {(!NeedsAsterisk(variableType) ? string.Empty : "*")}";
            }
            return this.ForVariableInternal(variableType);
        }

        private string ForVariableInternal(TypeReference variableType)
        {
            RequiredModifierType type = variableType as RequiredModifierType;
            if (type != null)
            {
                variableType = type.ElementType;
            }
            if (variableType.Resolve().IsInterface)
            {
                return this.ForVariable(TypeProvider.SystemObject);
            }
            return $"{this.ForType(variableType)} {(!NeedsAsterisk(variableType) ? string.Empty : "*")}";
        }

        public string ForVariableName(VariableReference variable) => 
            ("V_" + variable.Index);

        public string ForWindowsRuntimeDelegateComCallableWrapperClass(TypeReference delegateType) => 
            (this.ForTypeNameOnly(delegateType) + "_ComCallableWrapper");

        public string ForWindowsRuntimeDelegateComCallableWrapperInterface(TypeReference delegateType) => 
            ("I" + this.ForWindowsRuntimeDelegateComCallableWrapperClass(delegateType));

        public string ForWindowsRuntimeDelegateNativeInvokerMethod(MethodReference invokeMethod) => 
            (this.ForMethod(invokeMethod) + "_NativeInvoker");

        private string GenerateUniqueMethodPostFix(MethodReference methodReference) => 
            this._methodHashCache.GetUniqueHash(methodReference).ToString();

        private string GenerateUniqueStringLiteralPostFix(string literal) => 
            this._stringLiteralHashCache.GetUniqueHash(literal).ToString();

        private string GenerateUniqueTypePostFix(TypeReference typeReference) => 
            this._typeHashCache.GetUniqueHash(typeReference).ToString();

        public int GetFieldIndex(FieldReference field, bool includeBase = false)
        {
            FieldDefinition definition = field.Resolve();
            TypeDefinition definition2 = (definition.DeclaringType.BaseType == null) ? definition.DeclaringType : definition.DeclaringType.BaseType.Resolve();
            int num = 0;
            while (includeBase && (definition2 != null))
            {
                num += definition2.Fields.Count;
                definition2 = definition2.BaseType?.Resolve();
            }
            Collection<FieldDefinition> fields = definition.DeclaringType.Fields;
            for (int i = 0; i < fields.Count; i++)
            {
                if (definition == fields[i])
                {
                    return (num + i);
                }
            }
            throw new InvalidOperationException($"Field {field.Name} was not found on its definition {definition.DeclaringType.FullName}!");
        }

        private string GetWellKnownNameFor(TypeReference typeReference)
        {
            switch (typeReference.MetadataType)
            {
                case MetadataType.IntPtr:
                    return "IntPtr_t";

                case MetadataType.UIntPtr:
                    return "UIntPtr_t";

                case MetadataType.Object:
                    return "Il2CppObject";

                case MetadataType.String:
                    return "String_t";
            }
            TypeDefinition definition = typeReference.Resolve();
            if (((definition != null) && (definition.Module != null)) && (definition.Module.Name == "mscorlib.dll"))
            {
                string fullName = typeReference.FullName;
                if (fullName != null)
                {
                    int num;
                    if (<>f__switch$map2 == null)
                    {
                        Dictionary<string, int> dictionary = new Dictionary<string, int>(13) {
                            { 
                                "System.Array",
                                0
                            },
                            { 
                                "System.Type",
                                1
                            },
                            { 
                                "System.Reflection.MemberInfo",
                                2
                            },
                            { 
                                "System.Reflection.MethodInfo",
                                3
                            },
                            { 
                                "System.Reflection.FieldInfo",
                                4
                            },
                            { 
                                "System.Reflection.PropertyInfo",
                                5
                            },
                            { 
                                "System.Reflection.EventInfo",
                                6
                            },
                            { 
                                "System.MonoType",
                                7
                            },
                            { 
                                "System.Reflection.MonoMethod",
                                8
                            },
                            { 
                                "System.Reflection.MonoGenericMethod",
                                9
                            },
                            { 
                                "System.Reflection.MonoField",
                                10
                            },
                            { 
                                "System.Reflection.MonoProperty",
                                11
                            },
                            { 
                                "System.Reflection.MonoEvent",
                                12
                            }
                        };
                        <>f__switch$map2 = dictionary;
                    }
                    if (<>f__switch$map2.TryGetValue(fullName, out num))
                    {
                        switch (num)
                        {
                            case 0:
                                return "Il2CppArray";

                            case 1:
                                return "Type_t";

                            case 2:
                                return "MemberInfo_t";

                            case 3:
                                return "MethodInfo_t";

                            case 4:
                                return "FieldInfo_t";

                            case 5:
                                return "PropertyInfo_t";

                            case 6:
                                return "EventInfo_t";

                            case 7:
                                return "MonoType_t";

                            case 8:
                                return "MonoMethod_t";

                            case 9:
                                return "MonoGenericMethod_t";

                            case 10:
                                return "MonoField_t";

                            case 11:
                                return "MonoProperty_t";

                            case 12:
                                return "MonoEvent_t";
                        }
                    }
                }
            }
            if (typeReference.IsIActivationFactory())
            {
                return "Il2CppIActivationFactory";
            }
            if (typeReference.IsIl2CppComObject())
            {
                return "Il2CppComObject";
            }
            return null;
        }

        private bool IsAsciiDigit(char c) => 
            ((c >= '0') && (c <= '9'));

        private bool IsSafeCharacter(char c) => 
            ((((c >= 'a') && (c <= 'z')) || ((c >= 'A') && (c <= 'Z'))) || (c == '_'));

        public bool IsSpecialArrayMethod(MethodReference methodReference) => 
            ((((methodReference.Name == "Set") || (methodReference.Name == "Get")) || ((methodReference.Name == "Address") || (methodReference.Name == ".ctor"))) && methodReference.DeclaringType.IsArray);

        public string ModuleNameToPrependString(string name) => 
            this.Clean(name.Replace(".dll", "").Replace(".DLL", ""));

        private static bool NeedsAsterisk(TypeReference type) => 
            (!UnderlyingType(type).IsValueType() || type.IsByReference);

        public TypeReference RemoveModifiers(TypeReference typeReference)
        {
            TypeReference elementType = typeReference;
            while (elementType != null)
            {
                PinnedType type = elementType as PinnedType;
                if (type != null)
                {
                    elementType = type.ElementType;
                }
                else
                {
                    RequiredModifierType type2 = elementType as RequiredModifierType;
                    if (type2 != null)
                    {
                        elementType = type2.ElementType;
                        continue;
                    }
                    return elementType;
                }
            }
            throw new Exception();
        }

        private string TypeMember(TypeReference type, string memberName)
        {
            if (type.IsGenericParameter)
            {
                GenericParameter parameter = (GenericParameter) type;
                object[] objArray1 = new object[] { (parameter.Owner.GenericParameterType != GenericParameterType.Type) ? this.ForMethodNameOnly((MethodReference) parameter.Owner) : this.ForTypeNameOnly((TypeReference) parameter.Owner), "_gp_", this.Clean(parameter.Name), "_", parameter.Position };
                return $"{string.Concat(objArray1)}_{memberName}";
            }
            return $"{this.ForType(type)}_{memberName}";
        }

        private static TypeReference UnderlyingType(TypeReference type)
        {
            TypeSpecification specification = type as TypeSpecification;
            if (specification != null)
            {
                return UnderlyingType(specification.ElementType);
            }
            return type;
        }

        public string ForIntPtrT =>
            "intptr_t";

        public string ForUIntPtrT =>
            "uintptr_t";

        public string IntPtrValueField =>
            "m_value";

        public string Null =>
            "NULL";

        public string ThisParameterName =>
            "__this";

        public string UIntPtrPointerField =>
            "_pointer";

        [CompilerGenerated]
        private sealed class <ForPropertyInfo>c__AnonStorey0
        {
            internal NamingComponent $this;
            internal PropertyDefinition property;

            internal bool <>m__0(PropertyDefinition p) => 
                (p.Name == this.property.Name);

            internal string <>m__1(ParameterDefinition param) => 
                this.$this.ForTypeMangling(param.ParameterType);
        }
    }
}

