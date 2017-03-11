namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>The type of a prefab object as returned by EditorUtility.GetPrefabType.</para>
    /// </summary>
    public enum PrefabType
    {
        None,
        Prefab,
        ModelPrefab,
        PrefabInstance,
        ModelPrefabInstance,
        MissingPrefabInstance,
        DisconnectedPrefabInstance,
        DisconnectedModelPrefabInstance
    }
}

