namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// <para>GameObject utility functions.</para>
    /// </summary>
    public sealed class GameObjectUtility
    {
        [CompilerGenerated]
        private static Func<GameObject, bool> <>f__am$cache0;

        /// <summary>
        /// <para>Returns true if the passed in StaticEditorFlags are set on the GameObject specified.</para>
        /// </summary>
        /// <param name="go">The GameObject to check.</param>
        /// <param name="flags">The flags you want to check.</param>
        /// <returns>
        /// <para>Whether the GameObject's static flags match the flags specified.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool AreStaticEditorFlagsSet(GameObject go, StaticEditorFlags flags);
        internal static bool ContainsStatic(GameObject[] objects)
        {
            if ((objects != null) && (objects.Length != 0))
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    if ((objects[i] != null) && objects[i].isStatic)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        [Obsolete("use AnimatorUtility.DeoptimizeTransformHierarchy instead.")]
        private static void DeoptimizeTransformHierarchy(GameObject go)
        {
            AnimatorUtility.DeoptimizeTransformHierarchy(go);
        }

        internal static ShouldIncludeChildren DisplayUpdateChildrenDialogIfNeeded(IEnumerable<GameObject> gameObjects, string title, string message)
        {
            if (!HasChildren(gameObjects))
            {
                return ShouldIncludeChildren.HasNoChildren;
            }
            return (ShouldIncludeChildren) EditorUtility.DisplayDialogComplex(title, message, "Yes, change children", "No, this object only", "Cancel");
        }

        /// <summary>
        /// <para>Get the navmesh area index for the GameObject.</para>
        /// </summary>
        /// <param name="go">GameObject to query.</param>
        /// <returns>
        /// <para>NavMesh area index.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int GetNavMeshArea(GameObject go);
        /// <summary>
        /// <para>Get the navmesh area index from the area name.</para>
        /// </summary>
        /// <param name="name">NavMesh area name to query.</param>
        /// <returns>
        /// <para>The NavMesh area index. If there is no NavMesh area with the requested name, the return value is -1.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int GetNavMeshAreaFromName(string name);
        /// <summary>
        /// <para>Get all the navmesh area names.</para>
        /// </summary>
        /// <returns>
        /// <para>Names of all the NavMesh areas.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string[] GetNavMeshAreaNames();
        /// <summary>
        /// <para>Get the navmesh layer for the GameObject.</para>
        /// </summary>
        /// <param name="go">The GameObject to check.</param>
        /// <returns>
        /// <para>The navmesh layer for the GameObject specified.</para>
        /// </returns>
        [Obsolete("GetNavMeshArea instead.")]
        public static int GetNavMeshLayer(GameObject go) => 
            GetNavMeshArea(go);

        /// <summary>
        /// <para>Get the navmesh layer from the layer name.</para>
        /// </summary>
        /// <param name="name">The name of the navmesh layer.</param>
        /// <returns>
        /// <para>The layer number of the navmesh layer name specified.</para>
        /// </returns>
        [Obsolete("GetNavMeshAreaFromName instead.")]
        public static int GetNavMeshLayerFromName(string name) => 
            GetNavMeshAreaFromName(name);

        /// <summary>
        /// <para>Get all the navmesh layer names.</para>
        /// </summary>
        /// <returns>
        /// <para>An array of the names of all navmesh layers.</para>
        /// </returns>
        [Obsolete("GetNavMeshAreaNames instead.")]
        public static string[] GetNavMeshLayerNames() => 
            GetNavMeshAreaNames();

        /// <summary>
        /// <para>Gets the StaticEditorFlags of the GameObject specified.</para>
        /// </summary>
        /// <param name="go">The GameObject whose flags you are interested in.</param>
        /// <returns>
        /// <para>The static editor flags of the GameObject specified.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern StaticEditorFlags GetStaticEditorFlags(GameObject go);
        /// <summary>
        /// <para>Get unique name for a new GameObject compared to existing siblings. Useful when trying to avoid duplicate naming. When duplicate(s) are found, uses incremental a number after the base name.</para>
        /// </summary>
        /// <param name="parent">Target parent for a new GameObject. Null means root level.</param>
        /// <param name="name">Requested name for a new GameObject.</param>
        /// <returns>
        /// <para>Unique name for a new GameObject.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GetUniqueNameForSibling(Transform parent, string name);
        internal static bool HasChildren(IEnumerable<GameObject> gameObjects)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = go => go.transform.childCount > 0;
            }
            return Enumerable.Any<GameObject>(gameObjects, <>f__am$cache0);
        }

        [Obsolete("use AnimatorUtility.OptimizeTransformHierarchy instead.")]
        private static void OptimizeTransformHierarchy(GameObject go)
        {
            AnimatorUtility.OptimizeTransformHierarchy(go, null);
        }

        private static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform transform = go.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                SetLayerRecursively(transform.GetChild(i).gameObject, layer);
            }
        }

        /// <summary>
        /// <para>Set the navmesh area for the gameobject.</para>
        /// </summary>
        /// <param name="go">GameObject to modify.</param>
        /// <param name="areaIndex">NavMesh area index to set.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetNavMeshArea(GameObject go, int areaIndex);
        /// <summary>
        /// <para>Set the navmesh layer for the GameObject.</para>
        /// </summary>
        /// <param name="go">The GameObject on which to set the navmesh layer.</param>
        /// <param name="areaIndex">The layer number you want to set.</param>
        [Obsolete("SetNavMeshArea instead.")]
        public static void SetNavMeshLayer(GameObject go, int areaIndex)
        {
            SetNavMeshArea(go, areaIndex);
        }

        /// <summary>
        /// <para>Sets the parent and gives the child the same layer and position.</para>
        /// </summary>
        /// <param name="child">The GameObject that should have a new parent set.</param>
        /// <param name="parent">The GameObject that the child should get as a parent and have position and layer copied from. If null, this function does nothing.</param>
        public static void SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (parent != null)
            {
                child.transform.SetParent(parent.transform, false);
                RectTransform transform = child.transform as RectTransform;
                if (transform != null)
                {
                    transform.anchoredPosition = Vector2.zero;
                    Vector3 localPosition = transform.localPosition;
                    localPosition.z = 0f;
                    transform.localPosition = localPosition;
                }
                else
                {
                    child.transform.localPosition = Vector3.zero;
                }
                child.transform.localRotation = Quaternion.identity;
                child.transform.localScale = Vector3.one;
                SetLayerRecursively(child, parent.layer);
            }
        }

        /// <summary>
        /// <para>Sets the static editor flags on the specified GameObject.</para>
        /// </summary>
        /// <param name="go">The GameObject whose static editor flags you want to set.</param>
        /// <param name="flags">The flags to set on the GameObject.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetStaticEditorFlags(GameObject go, StaticEditorFlags flags);

        internal enum ShouldIncludeChildren
        {
            Cancel = 2,
            DontIncludeChildren = 1,
            HasNoChildren = -1,
            IncludeChildren = 0
        }
    }
}

