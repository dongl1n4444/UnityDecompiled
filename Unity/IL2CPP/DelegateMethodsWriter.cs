namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class DelegateMethodsWriter
    {
        private readonly string _methodGetterName;
        private readonly string _methodPtrGetterName;
        private readonly string _methodPtrSetterName;
        private readonly string _methodSetterName;
        private readonly string _prevGetterName;
        private readonly string _targetGetterName;
        private readonly string _targetSetterName;
        private readonly string _valueGetterName;
        private readonly CppCodeWriter _writer;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache4;
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static ITypeProviderService TypeProvider;

        public DelegateMethodsWriter(CppCodeWriter writer)
        {
            <DelegateMethodsWriter>c__AnonStorey0 storey = new <DelegateMethodsWriter>c__AnonStorey0();
            this._writer = writer;
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<FieldDefinition, bool>(DelegateMethodsWriter.<DelegateMethodsWriter>m__0);
            }
            FieldDefinition field = TypeProvider.SystemDelegate.Fields.Single<FieldDefinition>(<>f__am$cache0);
            this._methodPtrGetterName = Naming.ForFieldGetter(field);
            this._methodPtrSetterName = Naming.ForFieldSetter(field);
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<FieldDefinition, bool>(DelegateMethodsWriter.<DelegateMethodsWriter>m__1);
            }
            FieldDefinition definition2 = TypeProvider.SystemDelegate.Fields.Single<FieldDefinition>(<>f__am$cache1);
            this._methodGetterName = Naming.ForFieldGetter(definition2);
            this._methodSetterName = Naming.ForFieldSetter(definition2);
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<FieldDefinition, bool>(DelegateMethodsWriter.<DelegateMethodsWriter>m__2);
            }
            FieldDefinition definition3 = TypeProvider.SystemDelegate.Fields.Single<FieldDefinition>(<>f__am$cache2);
            this._targetGetterName = Naming.ForFieldGetter(definition3);
            this._targetSetterName = Naming.ForFieldSetter(definition3);
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = new Func<FieldDefinition, bool>(DelegateMethodsWriter.<DelegateMethodsWriter>m__3);
            }
            FieldDefinition definition4 = TypeProvider.SystemIntPtr.Fields.Single<FieldDefinition>(<>f__am$cache3);
            this._valueGetterName = Naming.ForFieldGetter(definition4);
            storey.expectedName = (CodeGenOptions.Dotnetprofile != DotNetProfile.Net45) ? "prev" : "delegates";
            FieldDefinition definition5 = TypeProvider.SystemMulticastDelegate.Fields.Single<FieldDefinition>(new Func<FieldDefinition, bool>(storey.<>m__0));
            this._prevGetterName = Naming.ForFieldGetter(definition5);
        }

        [CompilerGenerated]
        private static bool <DelegateMethodsWriter>m__0(FieldDefinition f) => 
            (f.Name == "method_ptr");

        [CompilerGenerated]
        private static bool <DelegateMethodsWriter>m__1(FieldDefinition f) => 
            (f.Name == "method");

        [CompilerGenerated]
        private static bool <DelegateMethodsWriter>m__2(FieldDefinition f) => 
            (f.Name == "m_target");

        [CompilerGenerated]
        private static bool <DelegateMethodsWriter>m__3(FieldDefinition f) => 
            (f.Name == "m_value");

        private static bool BeginInvokeHasAdditionalParameters(MethodReference method) => 
            (method.Parameters.Count > 2);

        private static List<string> CollectOutArgsIfAny(MethodReference method)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < (method.Parameters.Count - 1); i++)
            {
                if (method.Parameters[i].ParameterType.IsByReference)
                {
                    list.Add(Naming.ForParameterName(method.Parameters[i]));
                }
            }
            return list;
        }

        private static string CommaSeperate(IEnumerable<string> strings, bool alsoStartWithComma = false)
        {
            if (!strings.Any<string>())
            {
                return string.Empty;
            }
            string str2 = strings.AggregateWithComma();
            return (!alsoStartWithComma ? str2 : ("," + str2));
        }

        private void EmitInvocation(string delegateVariableName, MethodReference method, string methodPtrFieldName, string targetFieldName, List<string> parametersOnlyName, string methodInfoExpression, bool useFirstArgumentAsThis = false, bool forStatic = false, string resultVariableName = null)
        {
            List<string> strings = MethodSignatureWriter.ParametersFor(method, ParameterFormat.WithTypeAndNameThisObject, false, true, true, false).ToList<string>();
            if (useFirstArgumentAsThis)
            {
                strings.RemoveAt(1);
            }
            if (forStatic)
            {
                strings.Insert(0, Naming.ForVariable(TypeProvider.SystemObject));
            }
            object[] args = new object[] { Naming.ForVariable(TypeResolverFor(method).ResolveReturnType(method)), $"(*{"FunctionPointerType"})", CommaSeperate(strings, false) };
            this.WriteLine("typedef {0} {1} ({2});", args);
            string str = (resultVariableName != null) ? ((method.ReturnType.MetadataType == MetadataType.Void) ? string.Empty : $"{resultVariableName} = ") : ((method.ReturnType.MetadataType == MetadataType.Void) ? string.Empty : "return ");
            string str2 = $"(({"FunctionPointerType"}){ExpressionForFieldOf(delegateVariableName, methodPtrFieldName)})";
            string str3 = $"{delegateVariableName}->{this._targetGetterName}()";
            object[] objArray2 = new object[] { str, str2, !forStatic ? string.Empty : $"{Naming.Null},", !useFirstArgumentAsThis ? str3 : string.Empty, CommaSeperate(parametersOnlyName, !useFirstArgumentAsThis), methodInfoExpression };
            this.WriteLine("{0}{1}({2}{3}{4},{5});", objArray2);
        }

        private static string ExpressionForFieldOf(string variableName, string targetFieldName) => 
            $"{variableName}->{targetFieldName}";

        private static string ExpressionForFieldOfThis(string targetFieldName) => 
            ExpressionForFieldOf(Naming.ThisParameterName, targetFieldName);

        private static bool ShouldEmitNotBoundInstanceInvocation(MethodReference method)
        {
            if (!method.Parameters.Any<ParameterDefinition>())
            {
                return false;
            }
            TypeReference typeReference = TypeResolverFor(method).ResolveParameterType(method, method.Parameters[0]);
            if (typeReference.IsValueType())
            {
                return false;
            }
            if (typeReference.IsPointer)
            {
                return false;
            }
            if (typeReference.IsByReference)
            {
                return false;
            }
            return true;
        }

        private static Unity.IL2CPP.ILPreProcessor.TypeResolver TypeResolverFor(MethodReference method) => 
            new Unity.IL2CPP.ILPreProcessor.TypeResolver(method.DeclaringType as GenericInstanceType, method as GenericInstanceMethod);

        private void WriteInvocationsForDelegate(string delegateVariableName, MethodReference method, List<string> parametersOnlyName, string resultVariableName = null)
        {
            string methodInfoExpression = $"(MethodInfo*)({ExpressionForFieldOf(delegateVariableName, this._methodGetterName)}().{this._valueGetterName}())";
            bool flag = ShouldEmitNotBoundInstanceInvocation(method);
            object[] args = new object[] { methodInfoExpression };
            this.WriteLine("il2cpp_codegen_raise_execution_engine_exception_if_method_is_not_found({0});", args);
            object[] objArray2 = new object[] { "___methodIsStatic", methodInfoExpression };
            this.WriteLine("bool {0} = MethodIsStatic({1});", objArray2);
            if (parametersOnlyName.Count != 0)
            {
                object[] objArray3 = new object[] { ExpressionForFieldOf(delegateVariableName, this._targetGetterName), "___methodIsStatic" };
                this.WriteLine("if ({0}() != NULL && {1})", objArray3);
            }
            else
            {
                object[] objArray4 = new object[] { ExpressionForFieldOf(delegateVariableName, this._targetGetterName), methodInfoExpression, "___methodIsStatic" };
                this.WriteLine("if (({0}() != NULL || MethodHasParameters({1})) && {2})", objArray4);
            }
            this._writer.BeginBlock();
            this.EmitInvocation(delegateVariableName, method, this._methodPtrGetterName + "()", this._targetGetterName + "()", parametersOnlyName, methodInfoExpression, false, true, resultVariableName);
            this._writer.EndBlock(false);
            if (flag)
            {
                object[] objArray5 = new object[] { ExpressionForFieldOf(delegateVariableName, this._targetGetterName), "___methodIsStatic" };
                this.WriteLine("else if ({0}() != NULL || {1})", objArray5);
            }
            else
            {
                this.WriteLine("else", new object[0]);
            }
            this._writer.BeginBlock();
            string str2 = delegateVariableName;
            MethodReference reference = method;
            string methodPtrFieldName = this._methodPtrGetterName + "()";
            string targetFieldName = this._targetGetterName + "()";
            List<string> list = parametersOnlyName;
            string str5 = methodInfoExpression;
            string str6 = resultVariableName;
            this.EmitInvocation(str2, reference, methodPtrFieldName, targetFieldName, list, str5, false, false, str6);
            if (flag)
            {
                this._writer.EndBlock(false);
                this.WriteLine("else", new object[0]);
                this._writer.BeginBlock();
                str6 = delegateVariableName;
                reference = method;
                str5 = this._methodPtrGetterName + "()";
                targetFieldName = this._targetGetterName + "()";
                list = parametersOnlyName;
                methodPtrFieldName = methodInfoExpression;
                bool useFirstArgumentAsThis = true;
                str2 = resultVariableName;
                this.EmitInvocation(str6, reference, str5, targetFieldName, list, methodPtrFieldName, useFirstArgumentAsThis, false, str2);
            }
            this._writer.EndBlock(false);
        }

        private void WriteInvocationsForDelegate45(MethodReference method, List<string> parametersOnlyName)
        {
            string str = $"(MethodInfo*)({ExpressionForFieldOfThis(this._methodGetterName)}().{this._valueGetterName}())";
            object[] args = new object[] { str };
            this.WriteLine("il2cpp_codegen_raise_execution_engine_exception_if_method_is_not_found({0});", args);
            string str2 = "length";
            string name = "result";
            string array = "delegatesToInvoke";
            string delegateVariableName = "currentDelegate";
            if (method.ReturnType.MetadataType != MetadataType.Void)
            {
                TypeReference type = TypeResolverFor(method).ResolveReturnType(method);
                this._writer.WriteVariable(type, name);
            }
            string str6 = ExpressionForFieldOfThis(this._prevGetterName);
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = f => f.Name == "delegates";
            }
            FieldDefinition definition = TypeProvider.SystemMulticastDelegate.Fields.Single<FieldDefinition>(<>f__am$cache4);
            string str7 = Naming.ForVariable(definition.FieldType);
            this._writer.AddIncludeForTypeDefinition(definition.FieldType);
            object[] objArray2 = new object[] { str7, array, str6 };
            this._writer.WriteLine("{0} {1} = {2}();", objArray2);
            object[] objArray3 = new object[] { array };
            this._writer.WriteLine("if ({0} != NULL)", objArray3);
            this._writer.BeginBlock();
            object[] objArray4 = new object[] { str2, array };
            this._writer.WriteLine("uint32_t {0} = {1}->max_length;", objArray4);
            object[] objArray5 = new object[] { str2 };
            this._writer.WriteLine("for (uint32_t i = 0; i < {0}; i++)", objArray5);
            this._writer.BeginBlock();
            object[] objArray6 = new object[] { delegateVariableName, Emit.LoadArrayElement(array, "i", false), Naming.ForType(((ArrayType) definition.FieldType).ElementType) };
            this._writer.WriteLine("{2}* {0} = {1};", objArray6);
            this.WriteInvocationsForDelegate(delegateVariableName, method, parametersOnlyName, name);
            this._writer.EndBlock(false);
            if (method.ReturnType.MetadataType != MetadataType.Void)
            {
                object[] objArray7 = new object[] { name };
                this._writer.WriteLine("return {0};", objArray7);
            }
            this._writer.EndBlock(false);
            this._writer.WriteLine("else");
            this._writer.BeginBlock();
            this.WriteInvocationsForDelegate(Naming.ThisParameterName, method, parametersOnlyName, null);
            this._writer.EndBlock(false);
        }

        private void WriteInvokeChainedDelegates(MethodReference method, List<string> parametersOnlyName, IRuntimeMetadataAccess metadataAccess)
        {
            string str = CommaSeperate(parametersOnlyName, true);
            string str2 = ExpressionForFieldOfThis(this._prevGetterName) + "()";
            object[] args = new object[] { str2 };
            this.WriteLine("if({0} != NULL)", args);
            this._writer.BeginBlock();
            object[] objArray2 = new object[] { metadataAccess.Method(method), Naming.ForVariable(method.DeclaringType), str2, str };
            this.WriteLine("{0}(({1}){2}{3}, method);", objArray2);
            this._writer.EndBlock(false);
        }

        private void WriteLine(string format, params object[] args)
        {
            this._writer.WriteLine(format, args);
        }

        private void WriteMethodBodyForBeginInvoke(MethodReference method, IRuntimeMetadataAccess metadataAccess)
        {
            object[] args = new object[] { method.Parameters.Count - 1 };
            this.WriteLine("void *__d_args[{0}] = {{0}};", args);
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = TypeResolverFor(method);
            if (BeginInvokeHasAdditionalParameters(method))
            {
                for (int i = 0; i < (method.Parameters.Count - 2); i++)
                {
                    ParameterDefinition parameter = method.Parameters[i];
                    TypeReference typeReference = resolver.ResolveParameterType(method, parameter);
                    string str = Naming.ForParameterName(parameter);
                    if (CodeGenOptions.MonoRuntime)
                    {
                        str = Naming.AddressOf(str);
                    }
                    else if (typeReference.IsByReference)
                    {
                        TypeReference elementType = ((ByReferenceType) typeReference).ElementType;
                        str = !elementType.IsValueType() ? Emit.Dereference(str) : Emit.Box(elementType, Emit.Dereference(str), metadataAccess);
                    }
                    else if (typeReference.IsValueType())
                    {
                        str = Emit.Box(typeReference, str, metadataAccess);
                    }
                    object[] objArray2 = new object[] { i, str };
                    this.WriteLine("__d_args[{0}] = {1};", objArray2);
                }
            }
            object[] objArray3 = new object[] { Naming.ForVariable(resolver.ResolveReturnType(method)), Naming.ForParameterName(method.Parameters[method.Parameters.Count - 2]), Naming.ForParameterName(method.Parameters[method.Parameters.Count - 1]) };
            this.WriteLine("return ({0})il2cpp_codegen_delegate_begin_invoke((Il2CppDelegate*)__this, __d_args, (Il2CppDelegate*){1}, (Il2CppObject*){2});", objArray3);
        }

        private void WriteMethodBodyForDelegateConstructor(MethodReference method)
        {
            string str = Naming.ForParameterName(method.Parameters[0]);
            string str2 = Naming.ForParameterName(method.Parameters[1]);
            object[] args = new object[] { ExpressionForFieldOfThis(this._methodPtrSetterName), str2, this._valueGetterName };
            this.WriteLine("{0}(il2cpp_codegen_get_method_pointer((MethodInfo*){1}.{2}()));", args);
            object[] objArray2 = new object[] { ExpressionForFieldOfThis(this._methodSetterName), str2 };
            this.WriteLine("{0}({1});", objArray2);
            object[] objArray3 = new object[] { ExpressionForFieldOfThis(this._targetSetterName), str };
            this.WriteLine("{0}({1});", objArray3);
        }

        private void WriteMethodBodyForDelegateEndInvoke(MethodReference method)
        {
            ParameterDefinition parameterReference = method.Parameters[method.Parameters.Count - 1];
            string str = "0";
            List<string> list = CollectOutArgsIfAny(method);
            if (list.Count > 0)
            {
                this.WriteLine("void* ___out_args[] = {", new object[0]);
                foreach (string str2 in list)
                {
                    object[] args = new object[] { str2 };
                    this.WriteLine("{0},", args);
                }
                this.WriteLine("};", new object[0]);
                str = "___out_args";
            }
            if (method.ReturnType.MetadataType == MetadataType.Void)
            {
                object[] objArray2 = new object[] { Naming.ForParameterName(parameterReference), str };
                this.WriteLine("il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) {0}, {1});", objArray2);
            }
            else
            {
                object[] objArray3 = new object[] { Naming.ForParameterName(parameterReference), str };
                this.WriteLine("Il2CppObject *__result = il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) {0}, {1});", objArray3);
                TypeReference typeReference = TypeResolverFor(method).ResolveReturnType(method);
                if (!typeReference.IsValueType())
                {
                    object[] objArray4 = new object[] { Naming.ForVariable(typeReference) };
                    this.WriteLine("return ({0})__result;", objArray4);
                }
                else
                {
                    object[] objArray5 = new object[] { Emit.Cast(new PointerType(typeReference), "UnBox ((Il2CppCodeGenObject*)__result)") };
                    this.WriteLine("return *{0};", objArray5);
                }
            }
        }

        private void WriteMethodBodyForInvoke(MethodReference method, IRuntimeMetadataAccess metadataAccess)
        {
            List<string> parametersOnlyName = MethodSignatureWriter.ParametersFor(method, ParameterFormat.WithNameNoThis, false, false, false, false).ToList<string>();
            if (CodeGenOptions.Dotnetprofile == DotNetProfile.Net45)
            {
                this.WriteInvocationsForDelegate45(method, parametersOnlyName);
            }
            else
            {
                this.WriteInvokeChainedDelegates(method, parametersOnlyName, metadataAccess);
                this.WriteInvocationsForDelegate(Naming.ThisParameterName, method, parametersOnlyName, null);
            }
        }

        public void WriteMethodBodyForIsRuntimeMethod(MethodReference method, IRuntimeMetadataAccess metadataAccess)
        {
            TypeDefinition definition = method.DeclaringType.Resolve();
            if (definition.BaseType.FullName != "System.MulticastDelegate")
            {
                throw new NotSupportedException("Cannot WriteMethodBodyForIsRuntimeMethod for non multicase delegate type: " + definition.FullName);
            }
            switch (method.Name)
            {
                case "Invoke":
                    this.WriteMethodBodyForInvoke(method, metadataAccess);
                    return;

                case "BeginInvoke":
                    this.WriteMethodBodyForBeginInvoke(method, metadataAccess);
                    return;

                case "EndInvoke":
                    this.WriteMethodBodyForDelegateEndInvoke(method);
                    return;

                case ".ctor":
                    this.WriteMethodBodyForDelegateConstructor(method);
                    return;
            }
            this._writer.WriteDefaultReturn(TypeResolverFor(method).Resolve(Unity.IL2CPP.GenericParameterResolver.ResolveReturnTypeIfNeeded(method)));
        }

        [CompilerGenerated]
        private sealed class <DelegateMethodsWriter>c__AnonStorey0
        {
            internal string expectedName;

            internal bool <>m__0(FieldDefinition f) => 
                (f.Name == this.expectedName);
        }
    }
}

