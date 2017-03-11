namespace UnityEditorInternal
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [Serializable, StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public struct UISystemProfilerInfo
    {
        public int objectInstanceId;
        public int objectNameOffset;
        public int parentId;
        public int batchCount;
        public int totalBatchCount;
        public int vertexCount;
        public int totalVertexCount;
        public bool isBatch;
        public BatchBreakingReason batchBreakingReason;
        public int instanceIDsIndex;
        public int instanceIDsCount;
        public int renderDataIndex;
        public int renderDataCount;
    }
}

