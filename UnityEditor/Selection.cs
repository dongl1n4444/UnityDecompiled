namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Access to the selection in the editor.</para>
    /// </summary>
    public sealed class Selection
    {
        [CompilerGenerated]
        private static Func<Component, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<Transform, GameObject> <>f__am$cache1;
        /// <summary>
        /// <para>Delegate callback triggered when currently active/selected item has changed.</para>
        /// </summary>
        public static Action selectionChanged;

        internal static void Add(int instanceID)
        {
            List<int> list = new List<int>(instanceIDs);
            if (list.IndexOf(instanceID) < 0)
            {
                list.Add(instanceID);
                instanceIDs = list.ToArray();
            }
        }

        internal static void Add(Object obj)
        {
            if (obj != null)
            {
                Add(obj.GetInstanceID());
            }
        }

        /// <summary>
        /// <para>Returns whether an object is contained in the current selection.</para>
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="obj"></param>
        public static bool Contains(int instanceID) => 
            (Array.IndexOf<int>(instanceIDs, instanceID) != -1);

        /// <summary>
        /// <para>Returns whether an object is contained in the current selection.</para>
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="obj"></param>
        public static bool Contains(Object obj) => 
            Contains(obj.GetInstanceID());

        public static T[] GetFiltered<T>(SelectionMode mode) => 
            GetFilteredInternal(typeof(T), mode).Cast<T>().ToArray<T>();

        /// <summary>
        /// <para>Returns the current selection filtered by type and mode.</para>
        /// </summary>
        /// <param name="type">Only objects of this type will be retrieved.</param>
        /// <param name="mode">Further options to refine the selection.</param>
        public static Object[] GetFiltered(Type type, SelectionMode mode) => 
            GetFilteredInternal(type, mode).Cast<Object>().ToArray<Object>();

        private static IEnumerable GetFilteredInternal(Type type, SelectionMode mode)
        {
            <GetFilteredInternal>c__AnonStorey0 storey = new <GetFilteredInternal>c__AnonStorey0 {
                type = type
            };
            if (typeof(Component).IsAssignableFrom(storey.type) || storey.type.IsInterface)
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = c => c != null;
                }
                return Enumerable.Where<Component>(Enumerable.Select<Transform, Component>(GetTransforms(mode), new Func<Transform, Component>(storey.<>m__0)), <>f__am$cache0);
            }
            if (typeof(GameObject).IsAssignableFrom(storey.type))
            {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = t => t.gameObject;
                }
                return Enumerable.Select<Transform, GameObject>(GetTransforms(mode), <>f__am$cache1);
            }
            return Enumerable.Where<Object>(GetObjectsMode(mode), new Func<Object, bool>(storey.<>m__1));
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern Object[] GetObjectsMode(SelectionMode mode);
        /// <summary>
        /// <para>Allows for fine grained control of the selection type using the SelectionMode bitmask.</para>
        /// </summary>
        /// <param name="mode">Options for refining the selection.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern Transform[] GetTransforms(SelectionMode mode);
        private static void Internal_CallSelectionChanged()
        {
            if (selectionChanged != null)
            {
                selectionChanged();
            }
        }

        internal static void Remove(int instanceID)
        {
            List<int> list = new List<int>(instanceIDs);
            list.Remove(instanceID);
            instanceIDs = list.ToArray();
        }

        internal static void Remove(Object obj)
        {
            if (obj != null)
            {
                Remove(obj.GetInstanceID());
            }
        }

        /// <summary>
        /// <para>Selects an object with a context.</para>
        /// </summary>
        /// <param name="obj">Object being selected (will be equal activeObject).</param>
        /// <param name="context">Context object.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetActiveObjectWithContext(Object obj, Object context);

        /// <summary>
        /// <para>Returns the current context object, as was set via SetActiveObjectWithContext.</para>
        /// </summary>
        public static Object activeContext { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Returns the active game object. (The one shown in the inspector).</para>
        /// </summary>
        public static GameObject activeGameObject { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Returns the instanceID of the actual object selection. Includes prefabs, non-modifyable objects.</para>
        /// </summary>
        public static int activeInstanceID { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Returns the actual object selection. Includes prefabs, non-modifyable objects.</para>
        /// </summary>
        public static Object activeObject { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Returns the active transform. (The one shown in the inspector).</para>
        /// </summary>
        public static Transform activeTransform { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Returns the guids of the selected assets.</para>
        /// </summary>
        public static string[] assetGUIDs { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        internal static string[] assetGUIDsDeepSelection { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Returns the actual game object selection. Includes prefabs, non-modifyable objects.</para>
        /// </summary>
        public static GameObject[] gameObjects { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The actual unfiltered selection from the Scene returned as instance ids instead of objects.</para>
        /// </summary>
        public static int[] instanceIDs { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The actual unfiltered selection from the Scene.</para>
        /// </summary>
        public static Object[] objects { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Returns the top level selection, excluding prefabs.</para>
        /// </summary>
        public static Transform[] transforms { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        [CompilerGenerated]
        private sealed class <GetFilteredInternal>c__AnonStorey0
        {
            internal Type type;

            internal Component <>m__0(Transform t) => 
                t.GetComponent(this.type);

            internal bool <>m__1(Object o) => 
                ((o != null) && this.type.IsAssignableFrom(o.GetType()));
        }
    }
}

