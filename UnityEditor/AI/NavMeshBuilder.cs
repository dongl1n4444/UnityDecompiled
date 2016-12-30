namespace UnityEditor.AI
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.Scripting;
    using UnityEngine.Scripting.APIUpdating;

    [MovedFrom("UnityEditor")]
    public sealed class NavMeshBuilder
    {
        /// <summary>
        /// <para>Build the Navmesh.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void BuildNavMesh();
        /// <summary>
        /// <para>Build the Navmesh Asyncronously.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void BuildNavMeshAsync();
        /// <summary>
        /// <para>Builds the combined navmesh for the contents of multiple scenes.</para>
        /// </summary>
        /// <param name="paths">Array of paths to scenes that are used for building the navmesh.</param>
        public static void BuildNavMeshForMultipleScenes(string[] paths)
        {
            if (paths.Length != 0)
            {
                for (int i = 0; i < paths.Length; i++)
                {
                    for (int j = i + 1; j < paths.Length; j++)
                    {
                        if (paths[i] == paths[j])
                        {
                            throw new Exception("No duplicate scene names are allowed");
                        }
                    }
                }
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    if (!EditorSceneManager.OpenScene(paths[0]).IsValid())
                    {
                        throw new Exception("Could not open scene: " + paths[0]);
                    }
                    for (int k = 1; k < paths.Length; k++)
                    {
                        EditorSceneManager.OpenScene(paths[k], OpenSceneMode.Additive);
                    }
                    BuildNavMesh();
                    Object sceneNavMeshData = NavMeshBuilder.sceneNavMeshData;
                    for (int m = 0; m < paths.Length; m++)
                    {
                        if (EditorSceneManager.OpenScene(paths[m]).IsValid())
                        {
                            NavMeshBuilder.sceneNavMeshData = sceneNavMeshData;
                            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void Cancel();
        /// <summary>
        /// <para>Clear all Navmeshes.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void ClearAllNavMeshes();

        /// <summary>
        /// <para>Returns true if an asynchronous build is still running.</para>
        /// </summary>
        public static bool isRunning { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public static Object navMeshSettingsObject { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        internal static Object sceneNavMeshData { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

