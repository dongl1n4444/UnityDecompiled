namespace Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative.WindowsRuntimeProjection
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    internal class IDisposableDisposeMethodBodyWriter
    {
        private readonly MethodDefinition _closeMethod;
        [Inject]
        public static IWindowsRuntimeProjections WindowsRuntimeProjections;

        public IDisposableDisposeMethodBodyWriter(MethodDefinition closeMethod)
        {
            this._closeMethod = closeMethod;
        }

        public void WriteDispose(MethodDefinition method)
        {
            ILProcessor iLProcessor = method.Body.GetILProcessor();
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Callvirt, method.DeclaringType.Module.ImportReference(this._closeMethod));
            iLProcessor.Emit(OpCodes.Ret);
        }
    }
}

