namespace Unity.IL2CPP.WindowsRuntime
{
    using Mono.Cecil;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Marshaling.BodyWriters.NativeToManaged;

    internal class DisposableCCWWriter : IProjectedComCallableWrapperMethodWriter
    {
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache0;
        [Inject]
        public static IWindowsRuntimeProjections WindowsRuntimeProjections;

        public ComCallableWrapperMethodBodyWriter GetBodyWriter(MethodReference closeMethod)
        {
            TypeDefinition type = closeMethod.DeclaringType.Resolve();
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = m => m.Name == "Dispose";
            }
            return new ProjectedMethodBodyWriter(WindowsRuntimeProjections.ProjectToCLR(type).Methods.Single<MethodDefinition>(<>f__am$cache0), closeMethod);
        }

        public void WriteDependenciesFor(CppCodeWriter writer, TypeReference interfaceType)
        {
        }
    }
}

