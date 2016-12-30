namespace Unity.IL2CPP.Marshaling.BodyWriters.NativeToManaged
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.Marshaling.BodyWriters;
    using Unity.IL2CPP.Metadata;
    using Unity.IL2CPP.StringLiterals;

    internal class IReferenceComCallableWrapperMethodBodyWriter : ComCallableWrapperMethodBodyWriter
    {
        private readonly TypeReference _boxedType;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<ParameterDefinition, bool> <>f__am$cache5;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map3;
        [Inject]
        public static IStringLiteralProvider StringLiteralProvider;

        public IReferenceComCallableWrapperMethodBodyWriter(MethodReference interfaceMethod, TypeReference boxedType) : base(interfaceMethod, interfaceMethod, MarshalType.WindowsRuntime)
        {
            this._boxedType = boxedType;
        }

        private static PropertyType GetBoxedPropertyType(TypeReference type)
        {
            switch (type.MetadataType)
            {
                case MetadataType.Boolean:
                    return PropertyType.Boolean;

                case MetadataType.Char:
                    return PropertyType.Char16;

                case MetadataType.Byte:
                    return PropertyType.UInt8;

                case MetadataType.Int16:
                    return PropertyType.Int16;

                case MetadataType.UInt16:
                    return PropertyType.UInt16;

                case MetadataType.Int32:
                    return PropertyType.Int32;

                case MetadataType.UInt32:
                    return PropertyType.UInt32;

                case MetadataType.Int64:
                    return PropertyType.Int64;

                case MetadataType.UInt64:
                    return PropertyType.UInt64;

                case MetadataType.Single:
                    return PropertyType.Single;

                case MetadataType.Double:
                    return PropertyType.Double;

                case MetadataType.String:
                    return PropertyType.String;

                case MetadataType.ValueType:
                    switch (type.FullName)
                    {
                        case "System.Guid":
                            return PropertyType.Guid;

                        case "System.DateTimeOffset":
                            return PropertyType.DateTime;

                        case "System.TimeSpan":
                            return PropertyType.TimeSpan;

                        case "Windows.Foundation.Point":
                            return PropertyType.Point;

                        case "Windows.Foundation.Size":
                            return PropertyType.Size;

                        case "Windows.Foundation.Rect":
                            return PropertyType.Rect;
                    }
                    return PropertyType.Other;
            }
            return PropertyType.Other;
        }

        private static string GetDesiredTypeNameInExceptionMessage(TypeReference desiredType)
        {
            if (desiredType.MetadataType == MetadataType.Byte)
            {
                return "Byte";
            }
            return GetBoxedPropertyType(desiredType).ToString();
        }

        private string GetMethodCallExpression(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess, MethodReference methodToCall, params string[] args)
        {
            List<string> argumentArray = new List<string>();
            argumentArray.AddRange(args);
            if (MethodSignatureWriter.NeedsHiddenMethodInfo(methodToCall, MethodCallType.Normal, false))
            {
                argumentArray.Add(metadataAccess.HiddenMethodInfo(methodToCall));
            }
            string str = MethodBodyWriter.GetMethodCallExpression(base._managedMethod, methodToCall, methodToCall, new Unity.IL2CPP.ILPreProcessor.TypeResolver(), MethodCallType.Normal, metadataAccess, new VTableBuilder(), argumentArray, false, null);
            writer.AddIncludeForMethodDeclarations(methodToCall.DeclaringType);
            return str;
        }

        private string GetPointerToValueExpression(IRuntimeMetadataAccess metadataAccess)
        {
            string str = InteropMethodInfo.Naming.ForVariable(this._boxedType);
            if (this._boxedType.IsValueType())
            {
                return $"static_cast<{str}*>(UnBox({this.ManagedObjectExpression}, {metadataAccess.TypeInfoFor(this._boxedType)}))";
            }
            return $"static_cast<{str}>({this.ManagedObjectExpression})";
        }

        private static string GetStringLiteral(string str, IRuntimeMetadataAccess metadataAccess)
        {
            StringLiteralProvider.Add(str);
            return metadataAccess.StringLiteral(str);
        }

        private string GetUnboxedValueExpression(IRuntimeMetadataAccess metadataAccess)
        {
            string pointerToValueExpression = this.GetPointerToValueExpression(metadataAccess);
            if (this._boxedType.IsValueType())
            {
                return ("*" + pointerToValueExpression);
            }
            return pointerToValueExpression;
        }

        private static bool IsNumericScalar(TypeReference type)
        {
            switch (type.MetadataType)
            {
                case MetadataType.Byte:
                case MetadataType.Int16:
                case MetadataType.UInt16:
                case MetadataType.Int32:
                case MetadataType.UInt32:
                case MetadataType.Int64:
                case MetadataType.UInt64:
                case MetadataType.Single:
                case MetadataType.Double:
                    return true;

                case MetadataType.ValueType:
                    return type.IsEnum();
            }
            return false;
        }

        private void WriteAssignMethodCallExpressionToReturnVariable(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess, MethodReference methodToCall, params string[] args)
        {
            string str = this.GetMethodCallExpression(writer, metadataAccess, methodToCall, args);
            writer.WriteLine($"{InteropMethodInfo.Naming.ForInteropReturnValue()} = {str};");
        }

        private void WriteConvertNumber(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess, TypeReference desiredType, TypeReference boxedUnderlyingType)
        {
            <WriteConvertNumber>c__AnonStorey0 storey = new <WriteConvertNumber>c__AnonStorey0 {
                desiredType = desiredType
            };
            writer.WriteLine("try");
            using (new BlockWriter(writer, false))
            {
                MethodDefinition methodToCall = boxedUnderlyingType.Resolve().Methods.Single<MethodDefinition>(new Func<MethodDefinition, bool>(storey.<>m__0));
                string[] args = new string[] { this.GetPointerToValueExpression(metadataAccess), InteropMethodInfo.Naming.Null };
                this.WriteAssignMethodCallExpressionToReturnVariable(writer, metadataAccess, methodToCall, args);
            }
            writer.WriteLine("catch (const Il2CppExceptionWrapper& ex)");
            using (new BlockWriter(writer, false))
            {
                TypeReference type = new TypeReference("System", "OverflowException", InteropMethodBodyWriter.TypeProvider.Corlib.MainModule, InteropMethodBodyWriter.TypeProvider.Corlib.Name);
                writer.WriteLine($"if (IsInst((Il2CppObject*)ex.ex, {metadataAccess.TypeInfoFor(type)}))");
                using (new BlockWriter(writer, false))
                {
                    this.WriteThrowInvalidCastExceptionWithValue(writer, metadataAccess, storey.desiredType);
                }
                writer.WriteLine();
                if (this._boxedType.MetadataType == MetadataType.String)
                {
                    TypeReference reference2 = new TypeReference("System", "FormatException", InteropMethodBodyWriter.TypeProvider.Corlib.MainModule, InteropMethodBodyWriter.TypeProvider.Corlib.Name);
                    writer.WriteLine($"if (IsInst((Il2CppObject*)ex.ex, {metadataAccess.TypeInfoFor(reference2)}))");
                    using (new BlockWriter(writer, false))
                    {
                        this.WriteThrowInvalidCastException(writer, storey.desiredType);
                    }
                    writer.WriteLine();
                }
                writer.WriteLine("throw;");
            }
        }

        private void WriteGetIsNumericScalar(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            string str = !IsNumericScalar(this._boxedType) ? "false" : "true";
            writer.WriteLine($"{InteropMethodInfo.Naming.ForInteropReturnValue()} = {str};");
        }

        private void WriteGetTypedValueMethod(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            TypeReference returnType = base._managedMethod.ReturnType;
            TypeReference type = this._boxedType;
            if (type.IsEnum())
            {
                type = type.GetUnderlyingEnumType();
            }
            if (Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(returnType, type, TypeComparisonMode.Exact))
            {
                writer.WriteLine($"{InteropMethodInfo.Naming.ForInteropReturnValue()} = {this.GetUnboxedValueExpression(metadataAccess)};");
            }
            else if ((returnType.MetadataType == MetadataType.String) && (this._boxedType.FullName == "System.Guid"))
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = m => (m.Name == "ToString") && (m.Parameters.Count == 0);
                }
                MethodDefinition methodToCall = this._boxedType.Resolve().Methods.Single<MethodDefinition>(<>f__am$cache0);
                string[] args = new string[] { this.GetPointerToValueExpression(metadataAccess) };
                this.WriteAssignMethodCallExpressionToReturnVariable(writer, metadataAccess, methodToCall, args);
            }
            else if ((returnType.FullName == "System.Guid") && (this._boxedType.MetadataType == MetadataType.String))
            {
                this.WriteGuidParse(writer, metadataAccess, returnType);
            }
            else
            {
                bool flag = IsNumericScalar(returnType);
                if (flag)
                {
                    flag = IsNumericScalar(type) || (type.MetadataType == MetadataType.String);
                }
                if (!flag)
                {
                    this.WriteThrowInvalidCastException(writer, returnType);
                }
                else
                {
                    this.WriteConvertNumber(writer, metadataAccess, returnType, type);
                }
            }
        }

        private void WriteGetTypeMethod(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            writer.WriteLine($"{InteropMethodInfo.Naming.ForInteropReturnValue()} = {(int) GetBoxedPropertyType(this._boxedType)};");
        }

        private void WriteGetValueMethod(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            writer.WriteLine($"{InteropMethodInfo.Naming.ForInteropReturnValue()} = {this.GetUnboxedValueExpression(metadataAccess)};");
        }

        private void WriteGuidParse(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess, TypeReference desiredType)
        {
            writer.WriteLine("try");
            using (new BlockWriter(writer, false))
            {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = m => m.Name == "Parse";
                }
                MethodDefinition methodToCall = desiredType.Resolve().Methods.Single<MethodDefinition>(<>f__am$cache1);
                string[] args = new string[] { InteropMethodInfo.Naming.Null, this.GetPointerToValueExpression(metadataAccess) };
                this.WriteAssignMethodCallExpressionToReturnVariable(writer, metadataAccess, methodToCall, args);
            }
            writer.WriteLine("catch (const Il2CppExceptionWrapper& ex)");
            using (new BlockWriter(writer, false))
            {
                TypeReference type = new TypeReference("System", "FormatException", InteropMethodBodyWriter.TypeProvider.Corlib.MainModule, InteropMethodBodyWriter.TypeProvider.Corlib.Name);
                writer.WriteLine($"if (IsInst((Il2CppObject*)ex.ex, {metadataAccess.TypeInfoFor(type)}))");
                using (new BlockWriter(writer, false))
                {
                    this.WriteThrowInvalidCastException(writer, desiredType);
                }
                writer.WriteLine();
                writer.WriteLine("throw;");
            }
        }

        protected override void WriteInteropCallStatementWithinTryBlock(CppCodeWriter writer, string[] localVariableNames, IRuntimeMetadataAccess metadataAccess)
        {
            string name = base._managedMethod.Name;
            if (name != null)
            {
                int num;
                if (<>f__switch$map3 == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(40) {
                        { 
                            "get_Value",
                            0
                        },
                        { 
                            "get_Type",
                            1
                        },
                        { 
                            "get_IsNumericScalar",
                            2
                        },
                        { 
                            "GetUInt8",
                            3
                        },
                        { 
                            "GetInt16",
                            3
                        },
                        { 
                            "GetUInt16",
                            3
                        },
                        { 
                            "GetInt32",
                            3
                        },
                        { 
                            "GetUInt32",
                            3
                        },
                        { 
                            "GetInt64",
                            3
                        },
                        { 
                            "GetUInt64",
                            3
                        },
                        { 
                            "GetSingle",
                            3
                        },
                        { 
                            "GetDouble",
                            3
                        },
                        { 
                            "GetChar16",
                            3
                        },
                        { 
                            "GetBoolean",
                            3
                        },
                        { 
                            "GetString",
                            3
                        },
                        { 
                            "GetGuid",
                            3
                        },
                        { 
                            "GetDateTime",
                            3
                        },
                        { 
                            "GetTimeSpan",
                            3
                        },
                        { 
                            "GetPoint",
                            3
                        },
                        { 
                            "GetSize",
                            3
                        },
                        { 
                            "GetRect",
                            3
                        },
                        { 
                            "GetUInt8Array",
                            4
                        },
                        { 
                            "GetInt16Array",
                            4
                        },
                        { 
                            "GetUInt16Array",
                            4
                        },
                        { 
                            "GetInt32Array",
                            4
                        },
                        { 
                            "GetUInt32Array",
                            4
                        },
                        { 
                            "GetInt64Array",
                            4
                        },
                        { 
                            "GetUInt64Array",
                            4
                        },
                        { 
                            "GetSingleArray",
                            4
                        },
                        { 
                            "GetDoubleArray",
                            4
                        },
                        { 
                            "GetChar16Array",
                            4
                        },
                        { 
                            "GetBooleanArray",
                            4
                        },
                        { 
                            "GetStringArray",
                            4
                        },
                        { 
                            "GetInspectableArray",
                            4
                        },
                        { 
                            "GetGuidArray",
                            4
                        },
                        { 
                            "GetDateTimeArray",
                            4
                        },
                        { 
                            "GetTimeSpanArray",
                            4
                        },
                        { 
                            "GetPointArray",
                            4
                        },
                        { 
                            "GetSizeArray",
                            4
                        },
                        { 
                            "GetRectArray",
                            4
                        }
                    };
                    <>f__switch$map3 = dictionary;
                }
                if (<>f__switch$map3.TryGetValue(name, out num))
                {
                    switch (num)
                    {
                        case 0:
                            this.WriteGetValueMethod(writer, metadataAccess);
                            return;

                        case 1:
                            this.WriteGetTypeMethod(writer, metadataAccess);
                            return;

                        case 2:
                            this.WriteGetIsNumericScalar(writer, metadataAccess);
                            return;

                        case 3:
                            this.WriteGetTypedValueMethod(writer, metadataAccess);
                            return;

                        case 4:
                            this.WriteThrowNotImplementedException(writer);
                            return;
                    }
                }
            }
            throw new NotSupportedException($"IReferenceComCallableWrapperMethodBodyWriter does not support writing body for {base._managedMethod.FullName}.");
        }

        private void WriteThrowInvalidCastException(CppCodeWriter writer, TypeReference desiredType)
        {
            string str = $"Object in an IPropertyValue is of type '{GetBoxedPropertyType(this._boxedType)}', which cannot be converted to a '{GetDesiredTypeNameInExceptionMessage(desiredType)}'.";
            writer.WriteStatement(Emit.RaiseManagedException($"il2cpp_codegen_get_invalid_cast_exception("{str}")"));
        }

        private void WriteThrowInvalidCastExceptionWithValue(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess, TypeReference desiredType)
        {
            TypeReference reference = !this._boxedType.IsEnum() ? this._boxedType : this._boxedType.GetBaseType();
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = m => (m.Name == "ToString") && (m.Parameters.Count == 0);
            }
            MethodDefinition methodToCall = reference.Resolve().Methods.Single<MethodDefinition>(<>f__am$cache2);
            TypeDefinition variableType = new TypeReference("System", "InvalidCastException", InteropMethodBodyWriter.TypeProvider.Corlib.MainModule, InteropMethodBodyWriter.TypeProvider.Corlib.Name).Resolve();
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = delegate (MethodDefinition m) {
                    if ((m.Name == "Concat") && (m.Parameters.Count == 3))
                    {
                    }
                    return (<>f__am$cache5 == null) && m.Parameters.All<ParameterDefinition>(<>f__am$cache5);
                };
            }
            MethodDefinition definition3 = InteropMethodBodyWriter.TypeProvider.SystemString.Methods.Single<MethodDefinition>(<>f__am$cache3);
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = m => ((m.Name == ".ctor") && (m.Parameters.Count == 1)) && (m.Parameters[0].ParameterType.MetadataType == MetadataType.String);
            }
            MethodDefinition definition4 = variableType.Methods.Single<MethodDefinition>(<>f__am$cache4);
            string str = InteropMethodInfo.Naming.ForVariable(InteropMethodBodyWriter.TypeProvider.SystemString);
            string stringLiteral = GetStringLiteral($"Object in an IPropertyValue is of type '{GetBoxedPropertyType(this._boxedType)}' with value '", metadataAccess);
            string[] args = new string[] { !this._boxedType.IsEnum() ? this.GetPointerToValueExpression(metadataAccess) : this.ManagedObjectExpression };
            string str3 = this.GetMethodCallExpression(writer, metadataAccess, methodToCall, args);
            string str4 = "valueString";
            string str5 = GetStringLiteral($"', which cannot be converted to a '{GetDesiredTypeNameInExceptionMessage(desiredType)}'.", metadataAccess);
            string str6 = InteropMethodInfo.Naming.ForVariable(variableType);
            writer.WriteLine($"{str} {str4} = {str3};");
            string[] textArray2 = new string[] { InteropMethodInfo.Naming.Null, stringLiteral, str4, str5 };
            writer.WriteLine($"{str} exceptionMessage = {this.GetMethodCallExpression(writer, metadataAccess, definition3, textArray2)};");
            writer.WriteLine($"{str6} translatedException = ({str6})il2cpp_codegen_object_new({metadataAccess.TypeInfoFor(variableType)});");
            string[] textArray3 = new string[] { "translatedException", "exceptionMessage" };
            writer.WriteStatement(this.GetMethodCallExpression(writer, metadataAccess, definition4, textArray3));
            writer.WriteStatement(Emit.RaiseManagedException("translatedException"));
        }

        private void WriteThrowNotImplementedException(CppCodeWriter writer)
        {
            writer.WriteStatement(Emit.RaiseManagedException("il2cpp_codegen_get_not_supported_exception(\"Boxing windows runtime arrays is not yet supported.\")"));
        }

        [CompilerGenerated]
        private sealed class <WriteConvertNumber>c__AnonStorey0
        {
            internal TypeReference desiredType;

            internal bool <>m__0(MethodDefinition m) => 
                (m.Name == $"System.IConvertible.To{this.desiredType.MetadataType}");
        }

        private enum PropertyType
        {
            Boolean = 11,
            BooleanArray = 0x40b,
            ByteArray = 0x401,
            Char16 = 10,
            Char16Array = 0x40a,
            DateTime = 14,
            DateTimeArray = 0x40e,
            Double = 9,
            DoubleArray = 0x409,
            Empty = 0,
            Guid = 0x10,
            GuidArray = 0x410,
            Inspectable = 13,
            InspectableArray = 0x40d,
            Int16 = 2,
            Int16Array = 0x402,
            Int32 = 4,
            Int32Array = 0x404,
            Int64 = 6,
            Int64Array = 0x406,
            Other = 20,
            OtherArray = 0x414,
            Point = 0x11,
            PointArray = 0x411,
            Rect = 0x13,
            RectArray = 0x413,
            Single = 8,
            SingleArray = 0x408,
            Size = 0x12,
            SizeArray = 0x412,
            String = 12,
            StringArray = 0x40c,
            TimeSpan = 15,
            TimeSpanArray = 0x40f,
            UInt16 = 3,
            UInt16Array = 0x403,
            UInt32 = 5,
            UInt32Array = 0x405,
            UInt64 = 7,
            UInt64Array = 0x407,
            UInt8 = 1
        }
    }
}

