namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP.IoCServices;

    internal class RuntimeImplementedMethodWriterComponent : IRuntimeImplementedMethodWriter, IRuntimeImplementedMethodAdder
    {
        private Dictionary<MethodDefinition, RuntimeImplementedMethodData> _runtimeImplementedMethods = new Dictionary<MethodDefinition, RuntimeImplementedMethodData>();

        public IEnumerable<RuntimeGenericTypeData> GetGenericSharingDataFor(MethodDefinition method)
        {
            RuntimeImplementedMethodData data = this._runtimeImplementedMethods[method];
            return data.GetGenericSharingData();
        }

        public bool IsRuntimeImplementedMethod(MethodDefinition method) => 
            this._runtimeImplementedMethods.ContainsKey(method);

        public void RegisterMethod(MethodDefinition method, GetGenericSharingDataDelegate getGenericSharingData, WriteRuntimeImplementedMethodBodyDelegate writerMethodBody)
        {
            this._runtimeImplementedMethods.Add(method, new RuntimeImplementedMethodData(getGenericSharingData, writerMethodBody));
        }

        public void WriteMethodBody(CppCodeWriter writer, MethodReference method, IRuntimeMetadataAccess metadataAccess)
        {
            RuntimeImplementedMethodData data = this._runtimeImplementedMethods[method.Resolve()];
            data.WriteRuntimeImplementedMethodBody(writer, method, metadataAccess);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RuntimeImplementedMethodData
        {
            public readonly GetGenericSharingDataDelegate GetGenericSharingData;
            public readonly WriteRuntimeImplementedMethodBodyDelegate WriteRuntimeImplementedMethodBody;
            public RuntimeImplementedMethodData(GetGenericSharingDataDelegate getGenericSharingData, WriteRuntimeImplementedMethodBodyDelegate writeRuntimeImplementedMethodBody)
            {
                this.GetGenericSharingData = getGenericSharingData;
                this.WriteRuntimeImplementedMethodBody = writeRuntimeImplementedMethodBody;
            }
        }
    }
}

