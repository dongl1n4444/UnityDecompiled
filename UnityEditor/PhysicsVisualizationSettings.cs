namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>This class contains the settings controlling the Physics Debug Visualization.</para>
    /// </summary>
    public static class PhysicsVisualizationSettings
    {
        /// <summary>
        /// <para>Clears the highlighted Collider.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void ClearMouseHighlight();
        /// <summary>
        /// <para>Deinitializes the physics debug visualization system and tracking of changes Colliders.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void DeinitDebugDraw();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool GetShowBoxColliders(FilterWorkflow filterWorkflow);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool GetShowCapsuleColliders(FilterWorkflow filterWorkflow);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool GetShowCollisionLayer(FilterWorkflow filterWorkflow, int layer);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern int GetShowCollisionLayerMask(FilterWorkflow filterWorkflow);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool GetShowKinematicBodies(FilterWorkflow filterWorkflow);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool GetShowMeshColliders(FilterWorkflow filterWorkflow, MeshColliderType colliderType);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool GetShowRigidbodies(FilterWorkflow filterWorkflow);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool GetShowSleepingBodies(FilterWorkflow filterWorkflow);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool GetShowSphereColliders(FilterWorkflow filterWorkflow);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool GetShowStaticColliders(FilterWorkflow filterWorkflow);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool GetShowTerrainColliders(FilterWorkflow filterWorkflow);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool GetShowTriggers(FilterWorkflow filterWorkflow);
        /// <summary>
        /// <para>Returns true if there currently is any kind of physics object highlighted.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool HasMouseHighlight();
        /// <summary>
        /// <para>Initializes the physics debug visualization system. The system must be initialized for any physics objects to be visualized. It is normally initialized by the PhysicsDebugWindow.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void InitDebugDraw();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern GameObject INTERNAL_CALL_Internal_PickClosestGameObject(Camera cam, int layers, ref Vector2 position, GameObject[] ignore, GameObject[] filter, out int materialIndex);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_UpdateMouseHighlight(ref Vector2 pos);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void Internal_CollectCollidersForDebugDraw(object colliderList);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_get_kinematicColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_get_rigidbodyColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_get_sleepingBodyColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_get_staticColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_get_triggerColor(out Color value);
        internal static GameObject Internal_PickClosestGameObject(Camera cam, int layers, Vector2 position, GameObject[] ignore, GameObject[] filter, out int materialIndex) => 
            INTERNAL_CALL_Internal_PickClosestGameObject(cam, layers, ref position, ignore, filter, out materialIndex);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_set_kinematicColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_set_rigidbodyColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_set_sleepingBodyColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_set_staticColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_set_triggerColor(ref Color value);
        public static GameObject PickClosestGameObject(Camera cam, int layers, Vector2 position, GameObject[] ignore, GameObject[] filter, out int materialIndex)
        {
            if (cam == null)
            {
                throw new ArgumentNullException("cam");
            }
            return Internal_PickClosestGameObject(cam, layers, position, ignore, filter, out materialIndex);
        }

        /// <summary>
        /// <para>Resets the visualization options to their default state.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void Reset();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetShowBoxColliders(FilterWorkflow filterWorkflow, bool show);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetShowCapsuleColliders(FilterWorkflow filterWorkflow, bool show);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetShowCollisionLayer(FilterWorkflow filterWorkflow, int layer, bool show);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetShowCollisionLayerMask(FilterWorkflow filterWorkflow, int mask);
        public static void SetShowForAllFilters(FilterWorkflow filterWorkflow, bool selected)
        {
            for (int i = 0; i < 0x20; i++)
            {
                SetShowCollisionLayer(filterWorkflow, i, selected);
            }
            SetShowStaticColliders(filterWorkflow, selected);
            SetShowTriggers(filterWorkflow, selected);
            SetShowRigidbodies(filterWorkflow, selected);
            SetShowKinematicBodies(filterWorkflow, selected);
            SetShowSleepingBodies(filterWorkflow, selected);
            SetShowBoxColliders(filterWorkflow, selected);
            SetShowSphereColliders(filterWorkflow, selected);
            SetShowCapsuleColliders(filterWorkflow, selected);
            SetShowMeshColliders(filterWorkflow, MeshColliderType.Convex, selected);
            SetShowMeshColliders(filterWorkflow, MeshColliderType.NonConvex, selected);
            SetShowTerrainColliders(filterWorkflow, selected);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetShowKinematicBodies(FilterWorkflow filterWorkflow, bool show);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetShowMeshColliders(FilterWorkflow filterWorkflow, MeshColliderType colliderType, bool show);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetShowRigidbodies(FilterWorkflow filterWorkflow, bool show);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetShowSleepingBodies(FilterWorkflow filterWorkflow, bool show);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetShowSphereColliders(FilterWorkflow filterWorkflow, bool show);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetShowStaticColliders(FilterWorkflow filterWorkflow, bool show);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetShowTerrainColliders(FilterWorkflow filterWorkflow, bool show);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetShowTriggers(FilterWorkflow filterWorkflow, bool show);
        /// <summary>
        /// <para>Updates the mouse-over highlight at the given mouse position in screen space.</para>
        /// </summary>
        /// <param name="pos"></param>
        public static void UpdateMouseHighlight(Vector2 pos)
        {
            INTERNAL_CALL_UpdateMouseHighlight(ref pos);
        }

        /// <summary>
        /// <para>Alpha amount used for transparency blending.</para>
        /// </summary>
        public static float baseAlpha { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Used to disinguish neighboring Colliders.</para>
        /// </summary>
        public static float colorVariance { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Shows extra options used to develop and debug the physics visualization.</para>
        /// </summary>
        public static bool devOptions { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Dirty marker used for refreshing the GUI.</para>
        /// </summary>
        public static int dirtyCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public static float dotAlpha { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Enables the mouse-over highlighting and mouse selection modes.</para>
        /// </summary>
        public static bool enableMouseSelect { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>See PhysicsVisualizationSettings.FilterWorkflow.</para>
        /// </summary>
        public static FilterWorkflow filterWorkflow { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        public static bool forceDot { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Forcing the drawing of Colliders on top of any other geometry, regardless of depth.</para>
        /// </summary>
        public static bool forceOverdraw { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Color for kinematic Rigidbodies.</para>
        /// </summary>
        public static Color kinematicColor
        {
            get
            {
                Color color;
                INTERNAL_get_kinematicColor(out color);
                return color;
            }
            set
            {
                INTERNAL_set_kinematicColor(ref value);
            }
        }

        /// <summary>
        /// <para>Color for Rigidbodies, primarily active ones.</para>
        /// </summary>
        public static Color rigidbodyColor
        {
            get
            {
                Color color;
                INTERNAL_get_rigidbodyColor(out color);
                return color;
            }
            set
            {
                INTERNAL_set_rigidbodyColor(ref value);
            }
        }

        /// <summary>
        /// <para>Should the PhysicsDebugWindow display the collision geometry.</para>
        /// </summary>
        public static bool showCollisionGeometry { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Color for Rigidbodies that are controlled by the physics simulator, but are not currently being simulated.</para>
        /// </summary>
        public static Color sleepingBodyColor
        {
            get
            {
                Color color;
                INTERNAL_get_sleepingBodyColor(out color);
                return color;
            }
            set
            {
                INTERNAL_set_sleepingBodyColor(ref value);
            }
        }

        /// <summary>
        /// <para>Color for Colliders that do not have a Rigidbody component.</para>
        /// </summary>
        public static Color staticColor
        {
            get
            {
                Color color;
                INTERNAL_get_staticColor(out color);
                return color;
            }
            set
            {
                INTERNAL_set_staticColor(ref value);
            }
        }

        /// <summary>
        /// <para>Maximum number of mesh tiles available to draw all Terrain Colliders.</para>
        /// </summary>
        public static int terrainTilesMax { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Color for Colliders that are Triggers.</para>
        /// </summary>
        public static Color triggerColor
        {
            get
            {
                Color color;
                INTERNAL_get_triggerColor(out color);
                return color;
            }
            set
            {
                INTERNAL_set_triggerColor(ref value);
            }
        }

        /// <summary>
        /// <para>Controls whether the SceneView or the GameView camera is used. Not shown in the UI.</para>
        /// </summary>
        public static bool useSceneCam { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Colliders within this distance will be displayed.</para>
        /// </summary>
        public static float viewDistance { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Decides whether the Workflow in the Physics Debug window should be inclusive
        /// (&lt;a href="PhysicsVisualizationSettings.FilterWorkflow.ShowSelectedItems.html"&gt;ShowSelectedItems&lt;a&gt;) or exclusive (&lt;a href="PhysicsVisualizationSettings.FilterWorkflow.HideSelectedItems.html"&gt;HideSelectedItems&lt;a&gt;).</para>
        /// </summary>
        public enum FilterWorkflow
        {
            HideSelectedItems,
            ShowSelectedItems
        }

        /// <summary>
        /// <para>Is a MeshCollider convex.</para>
        /// </summary>
        public enum MeshColliderType
        {
            Convex,
            NonConvex
        }
    }
}

