namespace Unity.IL2CPP.WindowsRuntime
{
    using Mono.Cecil;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Marshaling;
    using Unity.IL2CPP.Marshaling.BodyWriters;
    using Unity.IL2CPP.Marshaling.BodyWriters.NativeToManaged;
    using Unity.IL2CPP.Marshaling.MarshalInfoWriters;
    using Unity.IL2CPP.Metadata;

    internal sealed class ReadOnlyListCCWWriter : IProjectedComCallableWrapperMethodWriter
    {
        private readonly MethodDefinition _getCountMethodDef;
        private readonly MethodDefinition _getItemMethodDef;
        private readonly TypeDefinition _iReadOnlyCollectionTypeDef;
        private readonly TypeReference _iReadOnlyCollectionTypeRef;
        private readonly TypeDefinition _iReadOnlyListTypeDef;
        [CompilerGenerated]
        private static Func<InterfaceImplementation, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache2;
        [Inject]
        public static IWindowsRuntimeProjections WindowsRuntimeProjections;

        public ReadOnlyListCCWWriter(TypeDefinition iReadOnlyList)
        {
            this._iReadOnlyListTypeDef = iReadOnlyList;
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<InterfaceImplementation, bool>(ReadOnlyListCCWWriter.<ReadOnlyListCCWWriter>m__0);
            }
            this._iReadOnlyCollectionTypeRef = iReadOnlyList.Interfaces.Single<InterfaceImplementation>(<>f__am$cache0).InterfaceType;
            this._iReadOnlyCollectionTypeDef = this._iReadOnlyCollectionTypeRef.Resolve();
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<MethodDefinition, bool>(ReadOnlyListCCWWriter.<ReadOnlyListCCWWriter>m__1);
            }
            this._getCountMethodDef = this._iReadOnlyCollectionTypeDef.Methods.Single<MethodDefinition>(<>f__am$cache1);
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<MethodDefinition, bool>(ReadOnlyListCCWWriter.<ReadOnlyListCCWWriter>m__2);
            }
            this._getItemMethodDef = this._iReadOnlyListTypeDef.Methods.Single<MethodDefinition>(<>f__am$cache2);
        }

        [CompilerGenerated]
        private static bool <ReadOnlyListCCWWriter>m__0(InterfaceImplementation t) => 
            (t.InterfaceType.Name == "IReadOnlyCollection`1");

        [CompilerGenerated]
        private static bool <ReadOnlyListCCWWriter>m__1(MethodDefinition m) => 
            (m.Name == "get_Count");

        [CompilerGenerated]
        private static bool <ReadOnlyListCCWWriter>m__2(MethodDefinition m) => 
            (m.Name == "get_Item");

        public ComCallableWrapperMethodBodyWriter GetBodyWriter(MethodReference method)
        {
            TypeReference declaringType = method.DeclaringType;
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(WindowsRuntimeProjections.ProjectToCLR(declaringType));
            MethodReference managedInterfaceMethod = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(resolver.Resolve(this._iReadOnlyCollectionTypeRef)).Resolve(this._getCountMethodDef);
            MethodReference getItemMethod = resolver.Resolve(this._getItemMethodDef);
            switch (method.Name)
            {
                case "get_Size":
                    return new ProjectedMethodBodyWriter(managedInterfaceMethod, method);

                case "GetAt":
                    return new GetAtMethodBodyWriter(getItemMethod, method);

                case "GetMany":
                    return new GetManyMethodBodyWriter(managedInterfaceMethod, getItemMethod, method);

                case "IndexOf":
                    return new IndexOfMethodBodyWriter(managedInterfaceMethod, getItemMethod, method);
            }
            throw new NotSupportedException($"ReadOnlyListCCWWriter does not support writing method body for {method.FullName}.");
        }

        public void WriteDependenciesFor(CppCodeWriter writer, TypeReference interfaceType)
        {
        }

        private class GetAtMethodBodyWriter : ProjectedMethodBodyWriter
        {
            public GetAtMethodBodyWriter(MethodReference getItemMethod, MethodReference method) : base(getItemMethod, method)
            {
            }

            protected override void WriteInteropCallStatementWithinTryBlock(CppCodeWriter writer, string[] localVariableNames, IRuntimeMetadataAccess metadataAccess)
            {
                writer.WriteLine("try");
                using (new BlockWriter(writer, false))
                {
                    string str = base.GetMethodCallExpression(metadataAccess, this.ManagedObjectExpression, localVariableNames);
                    writer.WriteLine($"{InteropMethodInfo.Naming.ForInteropReturnValue()} = {str};");
                }
                writer.WriteLine("catch (const Il2CppExceptionWrapper& ex)");
                using (new BlockWriter(writer, false))
                {
                    TypeReference type = new TypeReference("System", "ArgumentOutOfRangeException", InteropMethodBodyWriter.TypeProvider.Corlib.MainModule, InteropMethodBodyWriter.TypeProvider.Corlib.Name);
                    writer.WriteLine($"if (IsInst(ex.ex, {metadataAccess.TypeInfoFor(type)}))");
                    using (new BlockWriter(writer, false))
                    {
                        writer.WriteLine($"ex.ex->hresult = {-2147483637}; // E_BOUNDS");
                    }
                    writer.WriteLine();
                    writer.WriteLine("throw;");
                }
            }
        }

        private class GetManyMethodBodyWriter : ProjectedMethodBodyWriter
        {
            private readonly TypeDefinition _argumentOutOfRangeException;
            private readonly MethodDefinition _argumentOutOfRangeExceptionConstructor;
            private readonly MethodReference _getCountMethod;
            private readonly MethodReference _getItemMethod;
            private readonly TypeReference _itemType;
            [CompilerGenerated]
            private static Func<MethodDefinition, bool> <>f__am$cache0;
            [CompilerGenerated]
            private static Func<PropertyDefinition, bool> <>f__am$cache1;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private bool <AreParametersMarshaled>k__BackingField;

            public GetManyMethodBodyWriter(MethodReference getCountMethod, MethodReference getItemMethod, MethodReference method) : base(method, method)
            {
                this.<AreParametersMarshaled>k__BackingField = false;
                this._itemType = ((GenericInstanceType) getCountMethod.DeclaringType).GenericArguments[0];
                this._getCountMethod = getCountMethod;
                this._getItemMethod = getItemMethod;
                this._argumentOutOfRangeException = new TypeReference("System", "ArgumentOutOfRangeException", InteropMethodBodyWriter.TypeProvider.Corlib.MainModule, InteropMethodBodyWriter.TypeProvider.Corlib.Name).Resolve();
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = new Func<MethodDefinition, bool>(ReadOnlyListCCWWriter.GetManyMethodBodyWriter.<GetManyMethodBodyWriter>m__0);
                }
                this._argumentOutOfRangeExceptionConstructor = this._argumentOutOfRangeException.Methods.Single<MethodDefinition>(<>f__am$cache0);
            }

            [CompilerGenerated]
            private static bool <GetManyMethodBodyWriter>m__0(MethodDefinition m) => 
                (((m.IsConstructor && m.HasThis) && (m.Parameters.Count == 1)) && (m.Parameters[0].ParameterType.MetadataType == MetadataType.String));

            protected override void WriteInteropCallStatementWithinTryBlock(CppCodeWriter writer, string[] localVariableNames, IRuntimeMetadataAccess metadataAccess)
            {
                string variableName = base.MarshaledParameterTypes[0].VariableName;
                string str2 = base.MarshaledParameterTypes[1].VariableName;
                string str3 = base.MarshaledParameterTypes[2].VariableName;
                DefaultMarshalInfoWriter writer2 = MarshalDataCollector.MarshalInfoWriterFor(this._itemType, MarshalType.WindowsRuntime, null, false, false, true, null);
                string str4 = "elementsInCollection";
                string str5 = base.GetMethodCallExpression(metadataAccess, this.ManagedObjectExpression, this._getCountMethod, MethodCallType.Virtual, new string[0]);
                writer.WriteLine($"uint32_t {str4} = {str5};");
                writer.WriteLine($"if ({variableName} != {str4} && {str3} != {InteropMethodInfo.Naming.Null})");
                using (new BlockWriter(writer, false))
                {
                    writer.WriteLine($"if ({variableName} > {str4} || {variableName} > {0x7fffffff})");
                    using (new BlockWriter(writer, false))
                    {
                        string str6 = "exception";
                        if (<>f__am$cache1 == null)
                        {
                            <>f__am$cache1 = p => p.Name == "HResult";
                        }
                        PropertyDefinition definition = InteropMethodBodyWriter.TypeProvider.SystemException.Properties.Single<PropertyDefinition>(<>f__am$cache1);
                        writer.AddIncludeForTypeDefinition(this._argumentOutOfRangeException);
                        writer.AddIncludeForMethodDeclaration(this._argumentOutOfRangeExceptionConstructor);
                        writer.AddIncludeForMethodDeclaration(definition.SetMethod);
                        writer.WriteLine($"{InteropMethodInfo.Naming.ForVariable(this._argumentOutOfRangeException)} {str6} = {Emit.NewObj(this._argumentOutOfRangeException, metadataAccess)};");
                        string[] args = new string[] { metadataAccess.StringLiteral("index") };
                        writer.WriteStatement(base.GetMethodCallExpression(metadataAccess, str6, this._argumentOutOfRangeExceptionConstructor, MethodCallType.Normal, args));
                        string[] textArray2 = new string[] { -2147483637.ToString() };
                        writer.WriteLine($"{base.GetMethodCallExpression(metadataAccess, str6, definition.SetMethod, MethodCallType.Normal, textArray2)}; // E_BOUNDS");
                        writer.WriteStatement(Emit.RaiseManagedException(str6));
                    }
                    writer.WriteLine($"{InteropMethodInfo.Naming.ForInteropReturnValue()} = std::min({str2}, {str4} - {variableName});");
                    writer.WriteLine($"for (uint32_t i = 0; i < {InteropMethodInfo.Naming.ForInteropReturnValue()}; i++)");
                    using (new BlockWriter(writer, false))
                    {
                        string[] textArray3 = new string[] { $"i + {variableName}" };
                        string str7 = base.GetMethodCallExpression(metadataAccess, this.ManagedObjectExpression, this._getItemMethod, MethodCallType.Virtual, textArray3);
                        writer.WriteLine($"{InteropMethodInfo.Naming.ForVariable(this._itemType)} itemManaged = {str7};");
                        writer2.WriteMarshalVariableToNative(writer, new ManagedMarshalValue("itemManaged"), $"{str3}[i]", null, metadataAccess);
                    }
                }
                writer.WriteLine("else");
                using (new BlockWriter(writer, false))
                {
                    writer.WriteLine($"{InteropMethodInfo.Naming.ForInteropReturnValue()} = 0;");
                }
            }

            protected override bool AreParametersMarshaled =>
                this.<AreParametersMarshaled>k__BackingField;
        }

        private class IndexOfMethodBodyWriter : ProjectedMethodBodyWriter
        {
            private readonly MethodReference _getCountMethod;
            private readonly MethodReference _getItemMethod;
            private readonly TypeReference _itemType;
            [CompilerGenerated]
            private static Func<MethodDefinition, bool> <>f__am$cache0;
            [CompilerGenerated]
            private static Func<MethodDefinition, bool> <>f__am$cache1;

            public IndexOfMethodBodyWriter(MethodReference getCountMethod, MethodReference getItemMethod, MethodReference method) : base(method, method)
            {
                this._itemType = ((GenericInstanceType) getCountMethod.DeclaringType).GenericArguments[0];
                this._getCountMethod = getCountMethod;
                this._getItemMethod = getItemMethod;
            }

            private string GetComparisonExpression(CppCodeWriter writer, string value1, string value2, IRuntimeMetadataAccess metadataAccess)
            {
                switch (this._itemType.MetadataType)
                {
                    case MetadataType.SByte:
                    case MetadataType.Byte:
                    case MetadataType.Int16:
                    case MetadataType.UInt16:
                    case MetadataType.Int32:
                    case MetadataType.UInt32:
                    case MetadataType.Int64:
                    case MetadataType.UInt64:
                    case MetadataType.Single:
                    case MetadataType.Double:
                    case MetadataType.IntPtr:
                    case MetadataType.UIntPtr:
                        return $"{value1} == {value2}";

                    case MetadataType.String:
                    {
                        if (<>f__am$cache0 == null)
                        {
                            <>f__am$cache0 = m => ((!m.HasThis && (m.Name == "Equals")) && ((m.Parameters.Count == 2) && (m.Parameters[0].ParameterType.MetadataType == MetadataType.String))) && (m.Parameters[1].ParameterType.MetadataType == MetadataType.String);
                        }
                        MethodDefinition definition = InteropMethodBodyWriter.TypeProvider.SystemString.Methods.Single<MethodDefinition>(<>f__am$cache0);
                        writer.AddIncludeForMethodDeclaration(definition);
                        string[] textArray1 = new string[] { value1, value2 };
                        return base.GetMethodCallExpression(metadataAccess, InteropMethodInfo.Naming.Null, definition, MethodCallType.Normal, textArray1);
                    }
                }
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = m => ((m.HasThis && m.IsVirtual) && ((m.Name == "Equals") && (m.Parameters.Count == 1))) && (m.Parameters[0].ParameterType.MetadataType == MetadataType.Object);
                }
                MethodDefinition method = InteropMethodBodyWriter.TypeProvider.SystemObject.Methods.Single<MethodDefinition>(<>f__am$cache1);
                if (this._itemType.IsValueType)
                {
                    MethodReference virtualMethodTargetMethodForConstrainedCallOnValueType = new VTableBuilder().GetVirtualMethodTargetMethodForConstrainedCallOnValueType(this._itemType, method);
                    if ((virtualMethodTargetMethodForConstrainedCallOnValueType != null) && Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(virtualMethodTargetMethodForConstrainedCallOnValueType.DeclaringType, this._itemType, TypeComparisonMode.Exact))
                    {
                        writer.AddIncludeForMethodDeclaration(virtualMethodTargetMethodForConstrainedCallOnValueType);
                        string[] textArray2 = new string[] { Emit.Box(this._itemType, value2, metadataAccess) };
                        return base.GetMethodCallExpression(metadataAccess, InteropMethodInfo.Naming.AddressOf(value1), virtualMethodTargetMethodForConstrainedCallOnValueType, MethodCallType.Normal, textArray2);
                    }
                    value1 = Emit.Box(this._itemType, value1, metadataAccess);
                    value2 = Emit.Box(this._itemType, value2, metadataAccess);
                }
                string[] args = new string[] { value2 };
                return base.GetMethodCallExpression(metadataAccess, value1, method, MethodCallType.Virtual, args);
            }

            protected override void WriteInteropCallStatementWithinTryBlock(CppCodeWriter writer, string[] localVariableNames, IRuntimeMetadataAccess metadataAccess)
            {
                string str = localVariableNames[0];
                string str2 = localVariableNames[1];
                writer.WriteLine($"{InteropMethodInfo.Naming.Dereference(str2)} = 0;");
                writer.WriteLine($"{InteropMethodInfo.Naming.ForInteropReturnValue()} = false;");
                writer.WriteLine();
                string str3 = "elementsInCollection";
                string str4 = base.GetMethodCallExpression(metadataAccess, this.ManagedObjectExpression, this._getCountMethod, MethodCallType.Virtual, new string[0]);
                writer.WriteLine($"int {str3} = {str4};");
                writer.WriteLine($"for (int i = 0; i < {str3}; i++)");
                using (new BlockWriter(writer, false))
                {
                    string[] args = new string[] { "i" };
                    string str5 = base.GetMethodCallExpression(metadataAccess, this.ManagedObjectExpression, this._getItemMethod, MethodCallType.Virtual, args);
                    writer.WriteLine($"{InteropMethodInfo.Naming.ForVariable(this._itemType)} item = {str5};");
                    writer.WriteLine($"if ({this.GetComparisonExpression(writer, "item", str, metadataAccess)})");
                    using (new BlockWriter(writer, false))
                    {
                        writer.WriteLine($"{InteropMethodInfo.Naming.Dereference(str2)} = static_cast<uint32_t>(i);");
                        writer.WriteLine($"{InteropMethodInfo.Naming.ForInteropReturnValue()} = true;");
                        writer.WriteLine("break;");
                    }
                }
            }
        }
    }
}

