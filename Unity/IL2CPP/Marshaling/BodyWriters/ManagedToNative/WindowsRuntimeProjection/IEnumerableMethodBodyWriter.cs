namespace Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative.WindowsRuntimeProjection
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    internal sealed class IEnumerableMethodBodyWriter
    {
        private readonly TypeDefinition _iteratorToEnumeratorAdapter;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache1;
        [Inject]
        public static ITypeProviderService TypeProvider;
        [Inject]
        public static IWindowsRuntimeProjections WindowsRuntimeProjections;

        public IEnumerableMethodBodyWriter(TypeDefinition iteratorToEnumeratorAdapter)
        {
            this._iteratorToEnumeratorAdapter = iteratorToEnumeratorAdapter;
        }

        public void WriteGetEnumerator(MethodDefinition method)
        {
            ILProcessor iLProcessor = method.Body.GetILProcessor();
            TypeReference declaringType = method.Overrides.First<MethodReference>().DeclaringType;
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(declaringType);
            TypeDefinition definition = WindowsRuntimeProjections.ProjectToWindowsRuntime(declaringType.Resolve());
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = m => m.Name == "First";
            }
            MethodReference reference2 = resolver.Resolve(definition.Methods.First<MethodDefinition>(<>f__am$cache0));
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = m => m.IsConstructor;
            }
            MethodReference reference3 = resolver.Resolve(this._iteratorToEnumeratorAdapter.Methods.First<MethodDefinition>(<>f__am$cache1));
            method.Body.Variables.Add(new VariableDefinition(resolver.Resolve(reference2.ReturnType)));
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Callvirt, reference2);
            iLProcessor.Emit(OpCodes.Dup);
            iLProcessor.Emit(OpCodes.Stloc_0);
            Instruction target = Instruction.Create(OpCodes.Ldloc_0);
            iLProcessor.Emit(OpCodes.Brtrue, target);
            iLProcessor.Emit(OpCodes.Ldnull);
            iLProcessor.Emit(OpCodes.Ret);
            iLProcessor.Append(target);
            iLProcessor.Emit(OpCodes.Newobj, reference3);
            iLProcessor.Emit(OpCodes.Ret);
        }
    }
}

