namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    public sealed class ComponentUtility
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool CollectConnectedComponents(GameObject targetGameObject, Component[] components, bool copy, out Component[] outCollectedComponents, out string outErrorMessage);
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

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool CopyComponent(Component component);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool CopyComponentRelativeToComponent(Component component, Component targetComponent, bool aboveTarget, bool validateOnly, out Component outNewComponent);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool CopyComponentRelativeToComponents(Component component, Component[] targetComponents, bool aboveTarget, bool validateOnly, out Component[] outNewComponents);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool CopyComponentsRelativeToComponents(Component[] components, Component[] targetComponents, bool aboveTarget, bool validateOnly, out Component[] outNewComponents);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool CopyComponentToGameObject(Component component, GameObject targetGameObject, bool validateOnly, out Component outNewComponent);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool CopyComponentToGameObjects(Component component, GameObject[] targetGameObjects, bool validateOnly, out Component[] outNewComponents);
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

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool MoveComponentDown(Component component);
        [ExcludeFromDocs]
        internal static bool MoveComponentRelativeToComponent(Component component, Component targetComponent, bool aboveTarget)
        {
            bool validateOnly = false;
            return MoveComponentRelativeToComponent(component, targetComponent, aboveTarget, validateOnly);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool MoveComponentRelativeToComponent(Component component, Component targetComponent, bool aboveTarget, [DefaultValue("false")] bool validateOnly);
        [ExcludeFromDocs]
        internal static bool MoveComponentsRelativeToComponents(Component[] components, Component[] targetComponents, bool aboveTarget)
        {
            bool validateOnly = false;
            return MoveComponentsRelativeToComponents(components, targetComponents, aboveTarget, validateOnly);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool MoveComponentsRelativeToComponents(Component[] components, Component[] targetComponents, bool aboveTarget, [DefaultValue("false")] bool validateOnly);
        [ExcludeFromDocs]
        internal static bool MoveComponentToGameObject(Component component, GameObject targetGameObject)
        {
            bool validateOnly = false;
            return MoveComponentToGameObject(component, targetGameObject, validateOnly);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool MoveComponentToGameObject(Component component, GameObject targetGameObject, [DefaultValue("false")] bool validateOnly);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool MoveComponentUp(Component component);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool PasteComponentAsNew(GameObject go);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
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

