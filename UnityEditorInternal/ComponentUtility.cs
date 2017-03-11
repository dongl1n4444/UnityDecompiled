namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    public sealed class ComponentUtility
    {
        private static bool CompareComponentOrderAndTypes(List<Component> srcComponents, List<Component> dstComponents)
        {
            if (srcComponents.Count != dstComponents.Count)
            {
                return false;
            }
            for (int i = 0; i != srcComponents.Count; i++)
            {
                if (srcComponents[i].GetType() != dstComponents[i].GetType())
                {
                    return false;
                }
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool CopyComponent(Component component);
        private static void DestroyComponents(List<Component> components)
        {
            for (int i = components.Count - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(components[i]);
            }
        }

        public static void DestroyComponentsMatching(GameObject dst, IsDesiredComponent componentFilter)
        {
            <DestroyComponentsMatching>c__AnonStorey0 storey = new <DestroyComponentsMatching>c__AnonStorey0 {
                componentFilter = componentFilter
            };
            List<Component> results = new List<Component>();
            dst.GetComponents<Component>(results);
            results.RemoveAll(new Predicate<Component>(storey.<>m__0));
            DestroyComponents(results);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool MoveComponentDown(Component component);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool MoveComponentUp(Component component);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool PasteComponentAsNew(GameObject go);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool PasteComponentValues(Component component);
        public static void ReplaceComponentsIfDifferent(GameObject src, GameObject dst, IsDesiredComponent componentFilter)
        {
            <ReplaceComponentsIfDifferent>c__AnonStorey1 storey = new <ReplaceComponentsIfDifferent>c__AnonStorey1 {
                componentFilter = componentFilter
            };
            List<Component> results = new List<Component>();
            src.GetComponents<Component>(results);
            results.RemoveAll(new Predicate<Component>(storey.<>m__0));
            List<Component> list2 = new List<Component>();
            dst.GetComponents<Component>(list2);
            list2.RemoveAll(new Predicate<Component>(storey.<>m__1));
            if (!CompareComponentOrderAndTypes(results, list2))
            {
                DestroyComponents(list2);
                list2.Clear();
                for (int j = 0; j != results.Count; j++)
                {
                    Component item = dst.AddComponent(results[j].GetType());
                    list2.Add(item);
                }
            }
            for (int i = 0; i != results.Count; i++)
            {
                EditorUtility.CopySerializedIfDifferent(results[i], list2[i]);
            }
        }

        [CompilerGenerated]
        private sealed class <DestroyComponentsMatching>c__AnonStorey0
        {
            internal ComponentUtility.IsDesiredComponent componentFilter;

            internal bool <>m__0(Component x) => 
                !this.componentFilter(x);
        }

        [CompilerGenerated]
        private sealed class <ReplaceComponentsIfDifferent>c__AnonStorey1
        {
            internal ComponentUtility.IsDesiredComponent componentFilter;

            internal bool <>m__0(Component x) => 
                !this.componentFilter(x);

            internal bool <>m__1(Component x) => 
                !this.componentFilter(x);
        }

        public delegate bool IsDesiredComponent(Component c);
    }
}

