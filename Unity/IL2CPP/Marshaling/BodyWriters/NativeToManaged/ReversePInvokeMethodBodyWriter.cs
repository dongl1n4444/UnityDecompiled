namespace Unity.IL2CPP.Marshaling.BodyWriters.NativeToManaged
{
    using Mono.Cecil;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.Marshaling;
    using Unity.IL2CPP.Marshaling.BodyWriters;
    using Unity.IL2CPP.Metadata;

    internal class ReversePInvokeMethodBodyWriter : NativeToManagedInteropMethodBodyWriter
    {
        [CompilerGenerated]
        private static Func<MarshaledType, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<CustomAttributeArgument, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<CustomAttributeArgument, object> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<CustomAttribute, bool> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<CustomAttributeArgument, bool> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<CustomAttributeArgument, object> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<CustomAttribute, bool> <>f__am$cache7;

        private ReversePInvokeMethodBodyWriter(MethodReference managedMethod, MethodReference interopMethod, bool useUnicodeCharset) : base(managedMethod, interopMethod, MarshalType.PInvoke, useUnicodeCharset)
        {
        }

        public static ReversePInvokeMethodBodyWriter Create(MethodReference managedMethod)
        {
            MethodReference interopMethod = GetInteropMethod(managedMethod);
            return new ReversePInvokeMethodBodyWriter(managedMethod, interopMethod, MarshalingUtils.UseUnicodeAsDefaultMarshalingForStringParameters(interopMethod));
        }

        internal string GetCallingConvention()
        {
            CustomAttribute pInvokeCallbackAttribute = GetPInvokeCallbackAttribute(base._managedMethod.Resolve());
            if ((pInvokeCallbackAttribute != null) && pInvokeCallbackAttribute.HasConstructorArguments)
            {
                if (<>f__am$cache5 == null)
                {
                    <>f__am$cache5 = new Func<CustomAttributeArgument, bool>(null, (IntPtr) <GetCallingConvention>m__6);
                }
                if (<>f__am$cache6 == null)
                {
                    <>f__am$cache6 = new Func<CustomAttributeArgument, object>(null, (IntPtr) <GetCallingConvention>m__7);
                }
                TypeReference reference = Enumerable.FirstOrDefault<object>(Enumerable.Select<CustomAttributeArgument, object>(Enumerable.Where<CustomAttributeArgument>(pInvokeCallbackAttribute.ConstructorArguments, <>f__am$cache5), <>f__am$cache6)) as TypeReference;
                if (reference == null)
                {
                    return "DEFAULT_CALL";
                }
                TypeDefinition definition = reference.Resolve();
                if (definition == null)
                {
                    return "DEFAULT_CALL";
                }
                if (<>f__am$cache7 == null)
                {
                    <>f__am$cache7 = new Func<CustomAttribute, bool>(null, (IntPtr) <GetCallingConvention>m__8);
                }
                CustomAttribute attribute2 = Enumerable.FirstOrDefault<CustomAttribute>(definition.CustomAttributes, <>f__am$cache7);
                if ((attribute2 == null) || !attribute2.HasConstructorArguments)
                {
                    return "DEFAULT_CALL";
                }
                CustomAttributeArgument argument = attribute2.ConstructorArguments[0];
                if (!(argument.Value is int))
                {
                    return "DEFAULT_CALL";
                }
                CustomAttributeArgument argument2 = attribute2.ConstructorArguments[0];
                switch (((CallingConvention) ((int) argument2.Value)))
                {
                    case CallingConvention.Cdecl:
                        return "CDECL";

                    case CallingConvention.StdCall:
                        return "STDCALL";
                }
            }
            return "DEFAULT_CALL";
        }

        private static MethodReference GetInteropMethod(MethodReference method)
        {
            CustomAttribute pInvokeCallbackAttribute = GetPInvokeCallbackAttribute(method.Resolve());
            if ((pInvokeCallbackAttribute == null) || !pInvokeCallbackAttribute.HasConstructorArguments)
            {
                return method;
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<CustomAttributeArgument, bool>(null, (IntPtr) <GetInteropMethod>m__2);
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<CustomAttributeArgument, object>(null, (IntPtr) <GetInteropMethod>m__3);
            }
            TypeReference reference2 = Enumerable.FirstOrDefault<object>(Enumerable.Select<CustomAttributeArgument, object>(Enumerable.Where<CustomAttributeArgument>(pInvokeCallbackAttribute.ConstructorArguments, <>f__am$cache1), <>f__am$cache2)) as TypeReference;
            if (reference2 == null)
            {
                return method;
            }
            TypeDefinition type = reference2.Resolve();
            if ((type == null) || !Extensions.IsDelegate(type))
            {
                return method;
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = new Func<MethodDefinition, bool>(null, (IntPtr) <GetInteropMethod>m__4);
            }
            MethodDefinition definition3 = Enumerable.SingleOrDefault<MethodDefinition>(type.Methods, <>f__am$cache3);
            if (definition3 == null)
            {
                return method;
            }
            if (!VirtualMethodResolution.MethodSignaturesMatchIgnoreStaticness(Unity.IL2CPP.ILPreProcessor.TypeResolver.For(Unity.IL2CPP.ILPreProcessor.TypeResolver.For(method.DeclaringType, method).Resolve(type)).Resolve(definition3), method))
            {
                return method;
            }
            return definition3;
        }

        private string GetMethodSignature()
        {
            string str = InteropMethodBodyWriter.Naming.ForReversePInvokeWrapperMethod(base._managedMethod);
            string decoratedName = base._marshaledReturnType.DecoratedName;
            string callingConvention = this.GetCallingConvention();
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<MarshaledType, string>(null, (IntPtr) <GetMethodSignature>m__1);
            }
            string str4 = EnumerableExtensions.AggregateWithComma(Enumerable.Select<MarshaledType, string>(base._marshaledParameterTypes, <>f__am$cache0));
            return string.Format("extern \"C\" {0} {1} {2}({3})", new object[] { decoratedName, callingConvention, str, str4 });
        }

        private static CustomAttribute GetPInvokeCallbackAttribute(MethodDefinition methodDef)
        {
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = new Func<CustomAttribute, bool>(null, (IntPtr) <GetPInvokeCallbackAttribute>m__5);
            }
            return Enumerable.FirstOrDefault<CustomAttribute>(methodDef.CustomAttributes, <>f__am$cache4);
        }

        public static bool IsReversePInvokeWrapperNecessary(MethodReference method)
        {
            if (method.HasThis)
            {
                return false;
            }
            MethodDefinition methodDef = method.Resolve();
            if ((methodDef.HasGenericParameters || methodDef.ReturnType.IsGenericParameter) || methodDef.DeclaringType.HasGenericParameters)
            {
                return false;
            }
            return ((GetPInvokeCallbackAttribute(methodDef) != null) || ((method.FullName == "System.Int32 System.IO.Compression.DeflateStream::UnmanagedWrite(System.IntPtr,System.Int32,System.IntPtr)") || (method.FullName == "System.Int32 System.IO.Compression.DeflateStream::UnmanagedRead(System.IntPtr,System.Int32,System.IntPtr)")));
        }

        protected override void WriteInteropCallStatement(CppCodeWriter writer, string[] localVariableNames, IRuntimeMetadataAccess metadataAccess)
        {
            string block = base.GetMethodCallExpression(metadataAccess, InteropMethodBodyWriter.Naming.Null, localVariableNames);
            MethodReturnType methodReturnType = this.GetMethodReturnType();
            if (methodReturnType.ReturnType.MetadataType != MetadataType.Void)
            {
                string str2 = InteropMethodBodyWriter.Naming.ForVariable(base._typeResolver.Resolve(methodReturnType.ReturnType));
                object[] args = new object[] { str2, InteropMethodBodyWriter.Naming.ForInteropReturnValue(), block };
                writer.WriteLine("{0} {1} = {2};", args);
            }
            else
            {
                writer.WriteStatement(block);
            }
        }

        public void WriteMethodDeclaration(CppCodeWriter writer)
        {
            foreach (MarshaledParameter parameter in base._parameters)
            {
                base.MarshalInfoWriterFor(parameter).WriteIncludesForFieldDeclaration(writer);
            }
            base.MarshalInfoWriterFor(this.GetMethodReturnType()).WriteIncludesForFieldDeclaration(writer);
            writer.WriteStatement(this.GetMethodSignature());
        }

        public void WriteMethodDefinition(CppCodeWriter writer, IMethodCollector methodCollector)
        {
            MethodWriter.WriteMethodWithMetadataInitialization(writer, this.GetMethodSignature(), base._managedMethod.FullName, new Action<CppCodeWriter, MetadataUsage, MethodUsage>(this, (IntPtr) this.<WriteMethodDefinition>m__0), InteropMethodBodyWriter.Naming.ForReversePInvokeWrapperMethod(base._managedMethod));
            methodCollector.AddReversePInvokeWrapper(base._managedMethod);
        }

        protected override void WriteReturnStatementEpilogue(CppCodeWriter writer, string unmarshaledReturnValueVariableName)
        {
            if (this.GetMethodReturnType().ReturnType.MetadataType != MetadataType.Void)
            {
                object[] args = new object[] { unmarshaledReturnValueVariableName };
                writer.WriteLine("return {0};", args);
            }
        }
    }
}

