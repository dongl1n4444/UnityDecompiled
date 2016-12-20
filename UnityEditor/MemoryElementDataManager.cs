namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;

    internal class MemoryElementDataManager
    {
        [CompilerGenerated]
        private static Comparison<MemoryElement> <>f__mg$cache0;
        [CompilerGenerated]
        private static Comparison<MemoryElement> <>f__mg$cache1;
        [CompilerGenerated]
        private static Comparison<ObjectInfo> <>f__mg$cache2;
        [CompilerGenerated]
        private static Comparison<MemoryElement> <>f__mg$cache3;

        private static List<MemoryElement> GenerateObjectTypeGroups(ObjectInfo[] memory, ObjectTypeFilter filter)
        {
            List<MemoryElement> list = new List<MemoryElement>();
            MemoryElement item = null;
            foreach (ObjectInfo info in memory)
            {
                if (GetObjectTypeFilter(info) == filter)
                {
                    if ((item == null) || (info.className != item.name))
                    {
                        item = new MemoryElement(info.className);
                        list.Add(item);
                    }
                    item.AddChild(new MemoryElement(info, true));
                }
            }
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new Comparison<MemoryElement>(MemoryElementDataManager.SortByMemorySize);
            }
            list.Sort(<>f__mg$cache0);
            foreach (MemoryElement element2 in list)
            {
                if (<>f__mg$cache1 == null)
                {
                    <>f__mg$cache1 = new Comparison<MemoryElement>(MemoryElementDataManager.SortByMemorySize);
                }
                element2.children.Sort(<>f__mg$cache1);
                if ((filter == ObjectTypeFilter.Other) && !HasValidNames(element2.children))
                {
                    element2.children.Clear();
                }
            }
            return list;
        }

        private static ObjectTypeFilter GetObjectTypeFilter(ObjectInfo info)
        {
            switch (info.reason)
            {
                case 1:
                    return ObjectTypeFilter.BuiltinResource;

                case 2:
                    return ObjectTypeFilter.DontSave;

                case 3:
                case 8:
                case 9:
                    return ObjectTypeFilter.Asset;

                case 10:
                    return ObjectTypeFilter.Other;
            }
            return ObjectTypeFilter.Scene;
        }

        public static MemoryElement GetTreeRoot(ObjectMemoryInfo[] memoryObjectList, int[] referencesIndices)
        {
            ObjectInfo[] array = new ObjectInfo[memoryObjectList.Length];
            for (int i = 0; i < memoryObjectList.Length; i++)
            {
                array[i] = new ObjectInfo { 
                    instanceId = memoryObjectList[i].instanceId,
                    memorySize = memoryObjectList[i].memorySize,
                    reason = memoryObjectList[i].reason,
                    name = memoryObjectList[i].name,
                    className = memoryObjectList[i].className
                };
            }
            int num2 = 0;
            for (int j = 0; j < memoryObjectList.Length; j++)
            {
                for (int k = 0; k < memoryObjectList[j].count; k++)
                {
                    int index = referencesIndices[k + num2];
                    if (array[index].referencedBy == null)
                    {
                        array[index].referencedBy = new List<ObjectInfo>();
                    }
                    array[index].referencedBy.Add(array[j]);
                }
                num2 += memoryObjectList[j].count;
            }
            MemoryElement element = new MemoryElement();
            if (<>f__mg$cache2 == null)
            {
                <>f__mg$cache2 = new Comparison<ObjectInfo>(MemoryElementDataManager.SortByMemoryClassName);
            }
            Array.Sort<ObjectInfo>(array, <>f__mg$cache2);
            element.AddChild(new MemoryElement("Scene Memory", GenerateObjectTypeGroups(array, ObjectTypeFilter.Scene)));
            element.AddChild(new MemoryElement("Assets", GenerateObjectTypeGroups(array, ObjectTypeFilter.Asset)));
            element.AddChild(new MemoryElement("Builtin Resources", GenerateObjectTypeGroups(array, ObjectTypeFilter.BuiltinResource)));
            element.AddChild(new MemoryElement("Not Saved", GenerateObjectTypeGroups(array, ObjectTypeFilter.DontSave)));
            element.AddChild(new MemoryElement("Other", GenerateObjectTypeGroups(array, ObjectTypeFilter.Other)));
            if (<>f__mg$cache3 == null)
            {
                <>f__mg$cache3 = new Comparison<MemoryElement>(MemoryElementDataManager.SortByMemorySize);
            }
            element.children.Sort(<>f__mg$cache3);
            return element;
        }

        private static bool HasValidNames(List<MemoryElement> memory)
        {
            for (int i = 0; i < memory.Count; i++)
            {
                if (!string.IsNullOrEmpty(memory[i].name))
                {
                    return true;
                }
            }
            return false;
        }

        private static int SortByMemoryClassName(ObjectInfo x, ObjectInfo y)
        {
            return y.className.CompareTo(x.className);
        }

        private static int SortByMemorySize(MemoryElement x, MemoryElement y)
        {
            return y.totalMemory.CompareTo(x.totalMemory);
        }

        private enum ObjectTypeFilter
        {
            Scene,
            Asset,
            BuiltinResource,
            DontSave,
            Other
        }
    }
}

