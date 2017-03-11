namespace Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative
{
    using Mono.Cecil;
    using System;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Marshaling.BodyWriters;

    internal class ComMethodWithPreOwnedInterfacePointerMethodBodyWriter : ComMethodBodyWriter
    {
        private readonly bool _shouldReleaseInterfaceAfterCall;

        public ComMethodWithPreOwnedInterfacePointerMethodBodyWriter(MethodReference interfaceMethod, bool shouldReleaseInterfaceAfterCall) : base(interfaceMethod, interfaceMethod)
        {
            this._shouldReleaseInterfaceAfterCall = shouldReleaseInterfaceAfterCall;
        }

        protected override void OnBeforeHResultCheck(CppCodeWriter writer)
        {
            if (this._shouldReleaseInterfaceAfterCall)
            {
                writer.WriteStatement(InteropMethodInfo.Naming.ForInteropInterfaceVariable(base._interfaceType) + "->Release()");
            }
        }

        protected override void WriteMethodPrologue(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
        }

        protected override bool UseQueryInterfaceToObtainInterfacePointer =>
            false;
    }
}

