namespace UnityEditor
{
    using System;
    using System.Collections.Generic;

    internal class ObjectInfo
    {
        public string className;
        public int instanceId;
        public long memorySize;
        public string name;
        public int reason;
        public List<ObjectInfo> referencedBy;
    }
}

