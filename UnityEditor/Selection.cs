namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// <para>Access to the selection in the editor.</para>
    /// </summary>
    public sealed class Selection
    {
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
        public static bool Contains(int instanceID)
        {
            return (Array.IndexOf<int>(instanceIDs, instanceID) != -1);
        }

        /// <summary>
        /// <para>Returns whether an object is contained in the current selection.</para>
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="obj"></param>
        public static bool Contains(Object obj)
        {
            return Contains(obj.GetInstanceID());
        }

        /// <summary>
        /// <para>Returns the current selection filtered by type and mode.</para>
        /// </summary>
        /// <param name="type">Only objects of this type will be retrieved.</param>
        /// <param name="mode">Further options to refine the selection.</param>
        public static Object[] GetFiltered(Type type, SelectionMode mode)
        {
            ArrayList list = new ArrayList();
            if ((type == typeof(Component)) || type.IsSubclassOf(typeof(Component)))
            {
                Transform[] transforms = GetTransforms(mode);
                foreach (Transform transform in transforms)
                {
                    Component component = transform.GetComponent(type);
                    if (component != null)
                    {
                        list.Add(component);
                    }
                }
            }
            else if ((type == typeof(GameObject)) || type.IsSubclassOf(typeof(GameObject)))
            {
                Transform[] transformArray3 = GetTransforms(mode);
                foreach (Transform transform2 in transformArray3)
                {
                    list.Add(transform2.gameObject);
                }
            }
            else
            {
                Object[] objectsMode = GetObjectsMode(mode);
                foreach (Object obj2 in objectsMode)
                {
                    if ((obj2 != null) && ((obj2.GetType() == type) || obj2.GetType().IsSubclassOf(type)))
                    {
                        list.Add(obj2);
                    }
                }
            }
            return (Object[]) list.ToArray(typeof(Object));
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern Object[] GetObjectsMode(SelectionMode mode);
        /// <summary>
        /// <para>Allows for fine grained control of the selection type using the SelectionMode bitmask.</para>
        /// </summary>
        /// <param name="mode">Options for refining the selection.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern Transform[] GetTransforms(SelectionMode mode);
        private static void Internal_CallSelectionChanged()
        {
            if (selectionChanged != null)
            {
                selectionChanged.Invoke();
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
        /// <para>Returns the active game object. (The one shown in the inspector).</para>
        /// </summary>
        public static GameObject activeGameObject { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Returns the instanceID of the actual object selection. Includes prefabs, non-modifyable objects.</para>
        /// </summary>
        public static int activeInstanceID { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Returns the actual object selection. Includes prefabs, non-modifyable objects.</para>
        /// </summary>
        public static Object activeObject { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Returns the active transform. (The one shown in the inspector).</para>
        /// </summary>
        public static Transform activeTransform { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Returns the guids of the selected assets.</para>
        /// </summary>
        public static string[] assetGUIDs { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        internal static string[] assetGUIDsDeepSelection { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns the actual game object selection. Includes prefabs, non-modifyable objects.</para>
        /// </summary>
        public static GameObject[] gameObjects { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The actual unfiltered selection from the Scene returned as instance ids instead of objects.</para>
        /// </summary>
        public static int[] instanceIDs { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The actual unfiltered selection from the Scene.</para>
        /// </summary>
        public static Object[] objects { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Returns the top level selection, excluding prefabs.</para>
        /// </summary>
        public static Transform[] transforms { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

