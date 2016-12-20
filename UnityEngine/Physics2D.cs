namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Global settings and helpers for 2D physics.</para>
    /// </summary>
    public class Physics2D
    {
        /// <summary>
        /// <para>Layer mask constant that includes all layers.</para>
        /// </summary>
        public const int AllLayers = -1;
        /// <summary>
        /// <para>Layer mask constant that includes all layers participating in raycasts by default.</para>
        /// </summary>
        public const int DefaultRaycastLayers = -5;
        /// <summary>
        /// <para>Layer mask constant for the default layer that ignores raycasts.</para>
        /// </summary>
        public const int IgnoreRaycastLayer = 4;
        private static List<Rigidbody2D> m_LastDisabledRigidbody2D = new List<Rigidbody2D>();

        [ExcludeFromDocs]
        public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            float distance = float.PositiveInfinity;
            return BoxCast(origin, size, angle, direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return BoxCast(origin, size, angle, direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return BoxCast(origin, size, angle, direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return BoxCast(origin, size, angle, direction, distance, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Casts a box against colliders in the scene, returning the first collider to contact with it.</para>
        /// </summary>
        /// <param name="origin">The point in 2D space where the shape originates.</param>
        /// <param name="size">The size of the shape.</param>
        /// <param name="angle">The angle of the shape (in degrees).</param>
        /// <param name="direction">Vector representing the direction of the shape.</param>
        /// <param name="distance">Maximum distance over which to cast the shape.</param>
        /// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
        /// <returns>
        /// <para>The cast results returned.</para>
        /// </returns>
        public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            RaycastHit2D hitd;
            Internal_BoxCast(origin, size, angle, direction, distance, layerMask, minDepth, maxDepth, out hitd);
            return hitd;
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            float distance = float.PositiveInfinity;
            return INTERNAL_CALL_BoxCastAll(ref origin, ref size, angle, ref direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_BoxCastAll(ref origin, ref size, angle, ref direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_BoxCastAll(ref origin, ref size, angle, ref direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_BoxCastAll(ref origin, ref size, angle, ref direction, distance, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Casts a box against colliders in the scene, returning all colliders that contact with it.</para>
        /// </summary>
        /// <param name="origin">The point in 2D space where the shape originates.</param>
        /// <param name="size">The size of the shape.</param>
        /// <param name="angle">The angle of the shape (in degrees).</param>
        /// <param name="direction">Vector representing the direction of the shape.</param>
        /// <param name="distance">Maximum distance over which to cast the shape.</param>
        /// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
        /// <returns>
        /// <para>The cast results returned.</para>
        /// </returns>
        public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_BoxCastAll(ref origin, ref size, angle, ref direction, distance, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            float distance = float.PositiveInfinity;
            return INTERNAL_CALL_BoxCastNonAlloc(ref origin, ref size, angle, ref direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, float distance)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_BoxCastNonAlloc(ref origin, ref size, angle, ref direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_BoxCastNonAlloc(ref origin, ref size, angle, ref direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_BoxCastNonAlloc(ref origin, ref size, angle, ref direction, results, distance, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Casts a box into the scene, returning colliders that contact with it into the provided results array.</para>
        /// </summary>
        /// <param name="origin">The point in 2D space where the shape originates.</param>
        /// <param name="size">The size of the shape.</param>
        /// <param name="angle">The angle of the shape (in degrees).</param>
        /// <param name="direction">Vector representing the direction of the shape.</param>
        /// <param name="results">Array to receive results.</param>
        /// <param name="distance">Maximum distance over which to cast the shape.</param>
        /// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
        /// <returns>
        /// <para>The number of results returned.</para>
        /// </returns>
        public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_BoxCastNonAlloc(ref origin, ref size, angle, ref direction, results, distance, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            float distance = float.PositiveInfinity;
            return CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Casts a capsule against colliders in the scene, returning the first collider to contact with it.</para>
        /// </summary>
        /// <param name="origin">The point in 2D space where the shape originates.</param>
        /// <param name="size">The size of the shape.</param>
        /// <param name="capsuleDirection">The direction of the capsule.</param>
        /// <param name="angle">The angle of the shape (in degrees).</param>
        /// <param name="direction">Vector representing the direction to cast the shape.</param>
        /// <param name="distance">Maximum distance over which to cast the shape.</param>
        /// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than this value.</param>
        /// <returns>
        /// <para>The cast results returned.</para>
        /// </returns>
        public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            RaycastHit2D hitd;
            Internal_CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, layerMask, minDepth, maxDepth, out hitd);
            return hitd;
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            float distance = float.PositiveInfinity;
            return INTERNAL_CALL_CapsuleCastAll(ref origin, ref size, capsuleDirection, angle, ref direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_CapsuleCastAll(ref origin, ref size, capsuleDirection, angle, ref direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_CapsuleCastAll(ref origin, ref size, capsuleDirection, angle, ref direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_CapsuleCastAll(ref origin, ref size, capsuleDirection, angle, ref direction, distance, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Casts a capsule against colliders in the scene, returning all colliders that contact with it.</para>
        /// </summary>
        /// <param name="origin">The point in 2D space where the shape originates.</param>
        /// <param name="size">The size of the shape.</param>
        /// <param name="capsuleDirection">The direction of the capsule.</param>
        /// <param name="angle">The angle of the shape (in degrees).</param>
        /// <param name="direction">Vector representing the direction to cast the shape.</param>
        /// <param name="distance">Maximum distance over which to cast the shape.</param>
        /// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than this value.</param>
        /// <returns>
        /// <para>The cast results returned.</para>
        /// </returns>
        public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_CapsuleCastAll(ref origin, ref size, capsuleDirection, angle, ref direction, distance, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            float distance = float.PositiveInfinity;
            return INTERNAL_CALL_CapsuleCastNonAlloc(ref origin, ref size, capsuleDirection, angle, ref direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, float distance)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_CapsuleCastNonAlloc(ref origin, ref size, capsuleDirection, angle, ref direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_CapsuleCastNonAlloc(ref origin, ref size, capsuleDirection, angle, ref direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_CapsuleCastNonAlloc(ref origin, ref size, capsuleDirection, angle, ref direction, results, distance, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Casts a capsule into the scene, returning colliders that contact with it into the provided results array.</para>
        /// </summary>
        /// <param name="origin">The point in 2D space where the shape originates.</param>
        /// <param name="size">The size of the shape.</param>
        /// <param name="capsuleDirection">The direction of the capsule.</param>
        /// <param name="angle">The angle of the shape (in degrees).</param>
        /// <param name="direction">Vector representing the direction to cast the shape.</param>
        /// <param name="results">Array to receive results.</param>
        /// <param name="distance">Maximum distance over which to cast the shape.</param>
        /// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than this value.</param>
        /// <returns>
        /// <para>The number of results returned.</para>
        /// </returns>
        public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_CapsuleCastNonAlloc(ref origin, ref size, capsuleDirection, angle, ref direction, results, distance, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            float distance = float.PositiveInfinity;
            return CircleCast(origin, radius, direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, float distance)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return CircleCast(origin, radius, direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return CircleCast(origin, radius, direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return CircleCast(origin, radius, direction, distance, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Casts a circle against colliders in the scene, returning the first collider to contact with it.</para>
        /// </summary>
        /// <param name="origin">The point in 2D space where the shape originates.</param>
        /// <param name="radius">The radius of the shape.</param>
        /// <param name="direction">Vector representing the direction of the shape.</param>
        /// <param name="distance">Maximum distance over which to cast the shape.</param>
        /// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
        /// <returns>
        /// <para>The cast results returned.</para>
        /// </returns>
        public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            RaycastHit2D hitd;
            Internal_CircleCast(origin, radius, direction, distance, layerMask, minDepth, maxDepth, out hitd);
            return hitd;
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            float distance = float.PositiveInfinity;
            return INTERNAL_CALL_CircleCastAll(ref origin, radius, ref direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, float distance)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_CircleCastAll(ref origin, radius, ref direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_CircleCastAll(ref origin, radius, ref direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_CircleCastAll(ref origin, radius, ref direction, distance, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Casts a circle against colliders in the scene, returning all colliders that contact with it.</para>
        /// </summary>
        /// <param name="origin">The point in 2D space where the shape originates.</param>
        /// <param name="radius">The radius of the shape.</param>
        /// <param name="direction">Vector representing the direction of the shape.</param>
        /// <param name="distance">Maximum distance over which to cast the shape.</param>
        /// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
        /// <returns>
        /// <para>The cast results returned.</para>
        /// </returns>
        public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_CircleCastAll(ref origin, radius, ref direction, distance, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            float distance = float.PositiveInfinity;
            return INTERNAL_CALL_CircleCastNonAlloc(ref origin, radius, ref direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, float distance)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_CircleCastNonAlloc(ref origin, radius, ref direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_CircleCastNonAlloc(ref origin, radius, ref direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_CircleCastNonAlloc(ref origin, radius, ref direction, results, distance, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Casts a circle into the scene, returning colliders that contact with it into the provided results array.</para>
        /// </summary>
        /// <param name="origin">The point in 2D space where the shape originates.</param>
        /// <param name="radius">The radius of the shape.</param>
        /// <param name="direction">Vector representing the direction of the shape.</param>
        /// <param name="results">Array to receive results.</param>
        /// <param name="distance">Maximum distance over which to cast the shape.</param>
        /// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
        /// <returns>
        /// <para>The number of results returned.</para>
        /// </returns>
        public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_CircleCastNonAlloc(ref origin, radius, ref direction, results, distance, layerMask, minDepth, maxDepth);
        }

        /// <summary>
        /// <para>Checks whether the collision detection system will ignore all collisionstriggers between collider1 and collider2/ or not.</para>
        /// </summary>
        /// <param name="collider1">The first collider to compare to collider2.</param>
        /// <param name="collider2">The second collider to compare to collider1.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool GetIgnoreCollision(Collider2D collider1, Collider2D collider2);
        /// <summary>
        /// <para>Should collisions between the specified layers be ignored?</para>
        /// </summary>
        /// <param name="layer1">ID of first layer.</param>
        /// <param name="layer2">ID of second layer.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool GetIgnoreLayerCollision(int layer1, int layer2);
        /// <summary>
        /// <para>Get the collision layer mask that indicates which layer(s) the specified layer can collide with.</para>
        /// </summary>
        /// <param name="layer">The layer to retrieve the collision layer mask for.</param>
        /// <returns>
        /// <para>A mask where each bit indicates a layer and whether it can collide with layer or not.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int GetLayerCollisionMask(int layer);
        [ExcludeFromDocs]
        public static RaycastHit2D GetRayIntersection(Ray ray)
        {
            int layerMask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return GetRayIntersection(ray, positiveInfinity, layerMask);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D GetRayIntersection(Ray ray, float distance)
        {
            int layerMask = -5;
            return GetRayIntersection(ray, distance, layerMask);
        }

        /// <summary>
        /// <para>Cast a 3D ray against the colliders in the scene returning the first collider along the ray.</para>
        /// </summary>
        /// <param name="ray">The 3D ray defining origin and direction to test.</param>
        /// <param name="distance">Maximum distance over which to cast the ray.</param>
        /// <param name="layerMask">Filter to detect colliders only on certain layers.</param>
        /// <returns>
        /// <para>The cast results returned.</para>
        /// </returns>
        public static RaycastHit2D GetRayIntersection(Ray ray, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
        {
            RaycastHit2D hitd;
            Internal_GetRayIntersection(ray, distance, layerMask, out hitd);
            return hitd;
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] GetRayIntersectionAll(Ray ray)
        {
            int layerMask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_GetRayIntersectionAll(ref ray, positiveInfinity, layerMask);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] GetRayIntersectionAll(Ray ray, float distance)
        {
            int layerMask = -5;
            return INTERNAL_CALL_GetRayIntersectionAll(ref ray, distance, layerMask);
        }

        /// <summary>
        /// <para>Cast a 3D ray against the colliders in the scene returning all the colliders along the ray.</para>
        /// </summary>
        /// <param name="ray">The 3D ray defining origin and direction to test.</param>
        /// <param name="distance">Maximum distance over which to cast the ray.</param>
        /// <param name="layerMask">Filter to detect colliders only on certain layers.</param>
        /// <returns>
        /// <para>The cast results returned.</para>
        /// </returns>
        [RequiredByNativeCode]
        public static RaycastHit2D[] GetRayIntersectionAll(Ray ray, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
        {
            return INTERNAL_CALL_GetRayIntersectionAll(ref ray, distance, layerMask);
        }

        [ExcludeFromDocs]
        public static int GetRayIntersectionNonAlloc(Ray ray, RaycastHit2D[] results)
        {
            int layerMask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_GetRayIntersectionNonAlloc(ref ray, results, positiveInfinity, layerMask);
        }

        [ExcludeFromDocs]
        public static int GetRayIntersectionNonAlloc(Ray ray, RaycastHit2D[] results, float distance)
        {
            int layerMask = -5;
            return INTERNAL_CALL_GetRayIntersectionNonAlloc(ref ray, results, distance, layerMask);
        }

        /// <summary>
        /// <para>Cast a 3D ray against the colliders in the scene returning the colliders along the ray.</para>
        /// </summary>
        /// <param name="ray">The 3D ray defining origin and direction to test.</param>
        /// <param name="distance">Maximum distance over which to cast the ray.</param>
        /// <param name="layerMask">Filter to detect colliders only on certain layers.</param>
        /// <param name="results">Array to receive results.</param>
        /// <returns>
        /// <para>The number of results returned.</para>
        /// </returns>
        public static int GetRayIntersectionNonAlloc(Ray ray, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
        {
            return INTERNAL_CALL_GetRayIntersectionNonAlloc(ref ray, results, distance, layerMask);
        }

        [ExcludeFromDocs]
        public static void IgnoreCollision(Collider2D collider1, Collider2D collider2)
        {
            bool ignore = true;
            IgnoreCollision(collider1, collider2, ignore);
        }

        /// <summary>
        /// <para>Makes the collision detection system ignore all collisionstriggers between collider1 and collider2/.</para>
        /// </summary>
        /// <param name="collider1">The first collider to compare to collider2.</param>
        /// <param name="collider2">The second collider to compare to collider1.</param>
        /// <param name="ignore">Whether collisionstriggers between collider1 and collider2/ should be ignored or not.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void IgnoreCollision(Collider2D collider1, Collider2D collider2, [DefaultValue("true")] bool ignore);
        [ExcludeFromDocs]
        public static void IgnoreLayerCollision(int layer1, int layer2)
        {
            bool ignore = true;
            IgnoreLayerCollision(layer1, layer2, ignore);
        }

        /// <summary>
        /// <para>Choose whether to detect or ignore collisions between a specified pair of layers.</para>
        /// </summary>
        /// <param name="layer1">ID of the first layer.</param>
        /// <param name="layer2">ID of the second layer.</param>
        /// <param name="ignore">Should collisions between these layers be ignored?</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void IgnoreLayerCollision(int layer1, int layer2, [DefaultValue("true")] bool ignore);
        private static void Internal_BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit)
        {
            INTERNAL_CALL_Internal_BoxCast(ref origin, ref size, angle, ref direction, distance, layerMask, minDepth, maxDepth, out raycastHit);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern RaycastHit2D[] INTERNAL_CALL_BoxCastAll(ref Vector2 origin, ref Vector2 size, float angle, ref Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int INTERNAL_CALL_BoxCastNonAlloc(ref Vector2 origin, ref Vector2 size, float angle, ref Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern RaycastHit2D[] INTERNAL_CALL_CapsuleCastAll(ref Vector2 origin, ref Vector2 size, CapsuleDirection2D capsuleDirection, float angle, ref Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int INTERNAL_CALL_CapsuleCastNonAlloc(ref Vector2 origin, ref Vector2 size, CapsuleDirection2D capsuleDirection, float angle, ref Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern RaycastHit2D[] INTERNAL_CALL_CircleCastAll(ref Vector2 origin, float radius, ref Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int INTERNAL_CALL_CircleCastNonAlloc(ref Vector2 origin, float radius, ref Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern RaycastHit2D[] INTERNAL_CALL_GetRayIntersectionAll(ref Ray ray, float distance, int layerMask);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int INTERNAL_CALL_GetRayIntersectionNonAlloc(ref Ray ray, RaycastHit2D[] results, float distance, int layerMask);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Internal_BoxCast(ref Vector2 origin, ref Vector2 size, float angle, ref Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Internal_CapsuleCast(ref Vector2 origin, ref Vector2 size, CapsuleDirection2D capsuleDirection, float angle, ref Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Internal_CircleCast(ref Vector2 origin, float radius, ref Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Internal_GetRayIntersection(ref Ray ray, float distance, int layerMask, out RaycastHit2D raycastHit);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Internal_Linecast(ref Vector2 start, ref Vector2 end, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Internal_Raycast(ref Vector2 origin, ref Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern RaycastHit2D[] INTERNAL_CALL_LinecastAll(ref Vector2 start, ref Vector2 end, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int INTERNAL_CALL_LinecastNonAlloc(ref Vector2 start, ref Vector2 end, RaycastHit2D[] results, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Collider2D INTERNAL_CALL_OverlapArea(ref Vector2 pointA, ref Vector2 pointB, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Collider2D[] INTERNAL_CALL_OverlapAreaAll(ref Vector2 pointA, ref Vector2 pointB, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int INTERNAL_CALL_OverlapAreaNonAlloc(ref Vector2 pointA, ref Vector2 pointB, Collider2D[] results, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Collider2D INTERNAL_CALL_OverlapBox(ref Vector2 point, ref Vector2 size, float angle, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Collider2D[] INTERNAL_CALL_OverlapBoxAll(ref Vector2 point, ref Vector2 size, float angle, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int INTERNAL_CALL_OverlapBoxNonAlloc(ref Vector2 point, ref Vector2 size, float angle, Collider2D[] results, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Collider2D INTERNAL_CALL_OverlapCapsule(ref Vector2 point, ref Vector2 size, CapsuleDirection2D direction, float angle, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Collider2D[] INTERNAL_CALL_OverlapCapsuleAll(ref Vector2 point, ref Vector2 size, CapsuleDirection2D direction, float angle, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int INTERNAL_CALL_OverlapCapsuleNonAlloc(ref Vector2 point, ref Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Collider2D INTERNAL_CALL_OverlapCircle(ref Vector2 point, float radius, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Collider2D[] INTERNAL_CALL_OverlapCircleAll(ref Vector2 point, float radius, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int INTERNAL_CALL_OverlapCircleNonAlloc(ref Vector2 point, float radius, Collider2D[] results, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Collider2D INTERNAL_CALL_OverlapPoint(ref Vector2 point, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Collider2D[] INTERNAL_CALL_OverlapPointAll(ref Vector2 point, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int INTERNAL_CALL_OverlapPointNonAlloc(ref Vector2 point, Collider2D[] results, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern RaycastHit2D[] INTERNAL_CALL_RaycastAll(ref Vector2 origin, ref Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int INTERNAL_CALL_RaycastNonAlloc(ref Vector2 origin, ref Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth, float maxDepth);
        private static void Internal_CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit)
        {
            INTERNAL_CALL_Internal_CapsuleCast(ref origin, ref size, capsuleDirection, angle, ref direction, distance, layerMask, minDepth, maxDepth, out raycastHit);
        }

        private static void Internal_CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit)
        {
            INTERNAL_CALL_Internal_CircleCast(ref origin, radius, ref direction, distance, layerMask, minDepth, maxDepth, out raycastHit);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_get_colliderAABBColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_get_colliderAsleepColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_get_colliderAwakeColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_get_colliderContactColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_get_gravity(out Vector2 value);
        private static void Internal_GetRayIntersection(Ray ray, float distance, int layerMask, out RaycastHit2D raycastHit)
        {
            INTERNAL_CALL_Internal_GetRayIntersection(ref ray, distance, layerMask, out raycastHit);
        }

        private static void Internal_Linecast(Vector2 start, Vector2 end, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit)
        {
            INTERNAL_CALL_Internal_Linecast(ref start, ref end, layerMask, minDepth, maxDepth, out raycastHit);
        }

        private static void Internal_Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit)
        {
            INTERNAL_CALL_Internal_Raycast(ref origin, ref direction, distance, layerMask, minDepth, maxDepth, out raycastHit);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_set_colliderAABBColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_set_colliderAsleepColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_set_colliderAwakeColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_set_colliderContactColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_set_gravity(ref Vector2 value);
        /// <summary>
        /// <para>Check whether collider1 is touching collider2 or not.</para>
        /// </summary>
        /// <param name="collider1">The collider to check if it is touching collider2.</param>
        /// <param name="collider2">The collider to check if it is touching collider1.</param>
        /// <returns>
        /// <para>Whether collider1 is touching collider2 or not.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool IsTouching(Collider2D collider1, Collider2D collider2);
        [ExcludeFromDocs]
        public static bool IsTouchingLayers(Collider2D collider)
        {
            int layerMask = -1;
            return IsTouchingLayers(collider, layerMask);
        }

        /// <summary>
        /// <para>Checks whether the collider is touching any colliders on the specified layerMask or not.</para>
        /// </summary>
        /// <param name="collider">The collider to check if it is touching colliders on the layerMask.</param>
        /// <param name="layerMask">Any colliders on any of these layers count as touching.</param>
        /// <returns>
        /// <para>Whether the collider is touching any colliders on the specified layerMask or not.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool IsTouchingLayers(Collider2D collider, [DefaultValue("AllLayers")] int layerMask);
        [ExcludeFromDocs]
        public static RaycastHit2D Linecast(Vector2 start, Vector2 end)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return Linecast(start, end, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D Linecast(Vector2 start, Vector2 end, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return Linecast(start, end, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D Linecast(Vector2 start, Vector2 end, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return Linecast(start, end, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Casts a line against colliders in the scene.</para>
        /// </summary>
        /// <param name="start">The start point of the line in world space.</param>
        /// <param name="end">The end point of the line in world space.</param>
        /// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
        /// <returns>
        /// <para>The cast results returned.</para>
        /// </returns>
        public static RaycastHit2D Linecast(Vector2 start, Vector2 end, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            RaycastHit2D hitd;
            Internal_Linecast(start, end, layerMask, minDepth, maxDepth, out hitd);
            return hitd;
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_LinecastAll(ref start, ref end, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_LinecastAll(ref start, ref end, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_LinecastAll(ref start, ref end, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Casts a line against colliders in the scene.</para>
        /// </summary>
        /// <param name="start">The start point of the line in world space.</param>
        /// <param name="end">The end point of the line in world space.</param>
        /// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
        /// <returns>
        /// <para>The cast results returned.</para>
        /// </returns>
        public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_LinecastAll(ref start, ref end, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_LinecastNonAlloc(ref start, ref end, results, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_LinecastNonAlloc(ref start, ref end, results, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_LinecastNonAlloc(ref start, ref end, results, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Casts a line against colliders in the scene.</para>
        /// </summary>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
        /// <param name="start">The start point of the line in world space.</param>
        /// <param name="end">The end point of the line in world space.</param>
        /// <param name="results">Returned array of objects that intersect the line.</param>
        /// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
        /// <returns>
        /// <para>The number of results returned.</para>
        /// </returns>
        public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_LinecastNonAlloc(ref start, ref end, results, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_OverlapArea(ref pointA, ref pointB, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_OverlapArea(ref pointA, ref pointB, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_OverlapArea(ref pointA, ref pointB, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Check if a collider falls within a rectangular area.</para>
        /// </summary>
        /// <param name="pointA">One corner of the rectangle.</param>
        /// <param name="pointB">Diagonally opposite corner of the rectangle.</param>
        /// <param name="layerMask">Filter to check objects only on specific layers.</param>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
        /// <returns>
        /// <para>The cast results returned.</para>
        /// </returns>
        public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_OverlapArea(ref pointA, ref pointB, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_OverlapAreaAll(ref pointA, ref pointB, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_OverlapAreaAll(ref pointA, ref pointB, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_OverlapAreaAll(ref pointA, ref pointB, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Get a list of all colliders that fall within a rectangular area.</para>
        /// </summary>
        /// <param name="pointA">One corner of the rectangle.</param>
        /// <param name="pointB">Diagonally opposite corner of the rectangle.</param>
        /// <param name="layerMask">Filter to check objects only on specific layers.</param>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
        /// <returns>
        /// <para>The cast results returned.</para>
        /// </returns>
        public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_OverlapAreaAll(ref pointA, ref pointB, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_OverlapAreaNonAlloc(ref pointA, ref pointB, results, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_OverlapAreaNonAlloc(ref pointA, ref pointB, results, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_OverlapAreaNonAlloc(ref pointA, ref pointB, results, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Get a list of all colliders that fall within a specified area.</para>
        /// </summary>
        /// <param name="pointA">One corner of the rectangle.</param>
        /// <param name="pointB">Diagonally opposite corner of the rectangle.</param>
        /// <param name="results">Array to receive results.</param>
        /// <param name="layerMask">Filter to check objects only on specified layers.</param>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
        /// <returns>
        /// <para>The number of results returned.</para>
        /// </returns>
        public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_OverlapAreaNonAlloc(ref pointA, ref pointB, results, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static Collider2D OverlapBox(Vector2 point, Vector2 size, float angle)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_OverlapBox(ref point, ref size, angle, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D OverlapBox(Vector2 point, Vector2 size, float angle, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_OverlapBox(ref point, ref size, angle, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D OverlapBox(Vector2 point, Vector2 size, float angle, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_OverlapBox(ref point, ref size, angle, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Check if a collider falls within a box area.</para>
        /// </summary>
        /// <param name="point">Center of the box.</param>
        /// <param name="size">Size of the box.</param>
        /// <param name="angle">Angle of the box.</param>
        /// <param name="layerMask">Filter to check objects only on specific layers.</param>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than this value.</param>
        /// <returns>
        /// <para>The cast results returned.</para>
        /// </returns>
        public static Collider2D OverlapBox(Vector2 point, Vector2 size, float angle, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_OverlapBox(ref point, ref size, angle, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size, float angle)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_OverlapBoxAll(ref point, ref size, angle, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size, float angle, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_OverlapBoxAll(ref point, ref size, angle, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size, float angle, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_OverlapBoxAll(ref point, ref size, angle, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Get a list of all colliders that fall within a box area.</para>
        /// </summary>
        /// <param name="point">Center of the box.</param>
        /// <param name="size">Size of the box.</param>
        /// <param name="angle">Angle of the box.</param>
        /// <param name="layerMask">Filter to check objects only on specific layers.</param>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than this value.</param>
        /// <returns>
        /// <para>The cast results returned.</para>
        /// </returns>
        public static Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size, float angle, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_OverlapBoxAll(ref point, ref size, angle, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_OverlapBoxNonAlloc(ref point, ref size, angle, results, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_OverlapBoxNonAlloc(ref point, ref size, angle, results, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_OverlapBoxNonAlloc(ref point, ref size, angle, results, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Get a list of all colliders that fall within a box area.</para>
        /// </summary>
        /// <param name="point">Center of the box.</param>
        /// <param name="size">Size of the box.</param>
        /// <param name="angle">Angle of the box.</param>
        /// <param name="results">Array to receive results.</param>
        /// <param name="layerMask">Filter to check objects only on specific layers.</param>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than this value.</param>
        /// <returns>
        /// <para>The number of results returned.</para>
        /// </returns>
        public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_OverlapBoxNonAlloc(ref point, ref size, angle, results, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_OverlapCapsule(ref point, ref size, direction, angle, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_OverlapCapsule(ref point, ref size, direction, angle, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_OverlapCapsule(ref point, ref size, direction, angle, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Check if a collider falls within a capsule area.</para>
        /// </summary>
        /// <param name="point">Center of the capsule.</param>
        /// <param name="size">Size of the capsule.</param>
        /// <param name="direction">The direction of the capsule.</param>
        /// <param name="angle">Angle of the capsule.</param>
        /// <param name="layerMask">Filter to check objects only on specific layers.</param>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than this value.</param>
        /// <returns>
        /// <para>The cast results returned.</para>
        /// </returns>
        public static Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_OverlapCapsule(ref point, ref size, direction, angle, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static Collider2D[] OverlapCapsuleAll(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_OverlapCapsuleAll(ref point, ref size, direction, angle, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D[] OverlapCapsuleAll(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_OverlapCapsuleAll(ref point, ref size, direction, angle, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D[] OverlapCapsuleAll(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_OverlapCapsuleAll(ref point, ref size, direction, angle, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Get a list of all colliders that fall within a capsule area.</para>
        /// </summary>
        /// <param name="point">Center of the capsule.</param>
        /// <param name="size">Size of the capsule.</param>
        /// <param name="direction">The direction of the capsule.</param>
        /// <param name="angle">Angle of the capsule.</param>
        /// <param name="layerMask">Filter to check objects only on specific layers.</param>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than this value.</param>
        /// <returns>
        /// <para>The cast results returned.</para>
        /// </returns>
        public static Collider2D[] OverlapCapsuleAll(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_OverlapCapsuleAll(ref point, ref size, direction, angle, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_OverlapCapsuleNonAlloc(ref point, ref size, direction, angle, results, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_OverlapCapsuleNonAlloc(ref point, ref size, direction, angle, results, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_OverlapCapsuleNonAlloc(ref point, ref size, direction, angle, results, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Get a list of all colliders that fall within a capsule area.</para>
        /// </summary>
        /// <param name="point">Center of the capsule.</param>
        /// <param name="size">Size of the capsule.</param>
        /// <param name="direction">The direction of the capsule.</param>
        /// <param name="angle">Angle of the capsule.</param>
        /// <param name="results">Array to receive results.</param>
        /// <param name="layerMask">Filter to check objects only on specific layers.</param>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than this value.</param>
        /// <returns>
        /// <para>The number of results returned.</para>
        /// </returns>
        public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_OverlapCapsuleNonAlloc(ref point, ref size, direction, angle, results, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static Collider2D OverlapCircle(Vector2 point, float radius)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_OverlapCircle(ref point, radius, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D OverlapCircle(Vector2 point, float radius, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_OverlapCircle(ref point, radius, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D OverlapCircle(Vector2 point, float radius, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_OverlapCircle(ref point, radius, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Check if a collider falls within a circular area.</para>
        /// </summary>
        /// <param name="point">Centre of the circle.</param>
        /// <param name="radius">Radius of the circle.</param>
        /// <param name="layerMask">Filter to check objects only on specific layers.</param>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
        public static Collider2D OverlapCircle(Vector2 point, float radius, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_OverlapCircle(ref point, radius, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static Collider2D[] OverlapCircleAll(Vector2 point, float radius)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_OverlapCircleAll(ref point, radius, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D[] OverlapCircleAll(Vector2 point, float radius, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_OverlapCircleAll(ref point, radius, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D[] OverlapCircleAll(Vector2 point, float radius, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_OverlapCircleAll(ref point, radius, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Get a list of all colliders that fall within a circular area.</para>
        /// </summary>
        /// <param name="point">Center of the circle.</param>
        /// <param name="radius">Radius of the circle.</param>
        /// <param name="layerMask">Filter to check objects only on specified layers.</param>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
        public static Collider2D[] OverlapCircleAll(Vector2 point, float radius, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_OverlapCircleAll(ref point, radius, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_OverlapCircleNonAlloc(ref point, radius, results, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_OverlapCircleNonAlloc(ref point, radius, results, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_OverlapCircleNonAlloc(ref point, radius, results, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Get a list of all colliders that fall within a circular area.</para>
        /// </summary>
        /// <param name="point">Center of the circle.</param>
        /// <param name="radius">Radius of the circle.</param>
        /// <param name="results">Array to receive results.</param>
        /// <param name="layerMask">Filter to check objects only on specific layers.</param>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
        /// <returns>
        /// <para>The number of results returned.</para>
        /// </returns>
        public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_OverlapCircleNonAlloc(ref point, radius, results, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static Collider2D OverlapPoint(Vector2 point)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_OverlapPoint(ref point, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D OverlapPoint(Vector2 point, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_OverlapPoint(ref point, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D OverlapPoint(Vector2 point, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_OverlapPoint(ref point, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Check if a collider overlaps a point in space.</para>
        /// </summary>
        /// <param name="point">A point in world space.</param>
        /// <param name="layerMask">Filter to check objects only on specific layers.</param>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
        /// <returns>
        /// <para>The cast results returned.</para>
        /// </returns>
        public static Collider2D OverlapPoint(Vector2 point, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_OverlapPoint(ref point, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static Collider2D[] OverlapPointAll(Vector2 point)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_OverlapPointAll(ref point, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D[] OverlapPointAll(Vector2 point, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_OverlapPointAll(ref point, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D[] OverlapPointAll(Vector2 point, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_OverlapPointAll(ref point, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Get a list of all colliders that overlap a point in space.</para>
        /// </summary>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
        /// <param name="point">A point in space.</param>
        /// <param name="layerMask">Filter to check objects only on specific layers.</param>
        /// <returns>
        /// <para>The cast results returned.</para>
        /// </returns>
        public static Collider2D[] OverlapPointAll(Vector2 point, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_OverlapPointAll(ref point, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_OverlapPointNonAlloc(ref point, results, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_OverlapPointNonAlloc(ref point, results, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_OverlapPointNonAlloc(ref point, results, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Get a list of all colliders that overlap a point in space.</para>
        /// </summary>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
        /// <param name="point">A point in space.</param>
        /// <param name="results">Array to receive results.</param>
        /// <param name="layerMask">Filter to check objects only on specific layers.</param>
        /// <returns>
        /// <para>The number of results returned.</para>
        /// </returns>
        public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_OverlapPointNonAlloc(ref point, results, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            float distance = float.PositiveInfinity;
            return Raycast(origin, direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return Raycast(origin, direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return Raycast(origin, direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs, RequiredByNativeCode]
        public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return Raycast(origin, direction, distance, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Casts a ray against colliders in the scene.</para>
        /// </summary>
        /// <param name="origin">The point in 2D space where the ray originates.</param>
        /// <param name="direction">Vector representing the direction of the ray.</param>
        /// <param name="distance">Maximum distance over which to cast the ray.</param>
        /// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
        /// <returns>
        /// <para>The cast results returned.</para>
        /// </returns>
        public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            RaycastHit2D hitd;
            Internal_Raycast(origin, direction, distance, layerMask, minDepth, maxDepth, out hitd);
            return hitd;
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            float distance = float.PositiveInfinity;
            return INTERNAL_CALL_RaycastAll(ref origin, ref direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, float distance)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_RaycastAll(ref origin, ref direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, float distance, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_RaycastAll(ref origin, ref direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, float distance, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_RaycastAll(ref origin, ref direction, distance, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Casts a ray against colliders in the scene, returning all colliders that contact with it.</para>
        /// </summary>
        /// <param name="origin">The point in 2D space where the ray originates.</param>
        /// <param name="direction">Vector representing the direction of the ray.</param>
        /// <param name="distance">Maximum distance over which to cast the ray.</param>
        /// <param name="layerMask">Filter to detect Colliders only on certain layers.</param>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
        /// <returns>
        /// <para>The cast results returned.</para>
        /// </returns>
        public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_RaycastAll(ref origin, ref direction, distance, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            float distance = float.PositiveInfinity;
            return INTERNAL_CALL_RaycastNonAlloc(ref origin, ref direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, float distance)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_RaycastNonAlloc(ref origin, ref direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_RaycastNonAlloc(ref origin, ref direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_RaycastNonAlloc(ref origin, ref direction, results, distance, layerMask, minDepth, positiveInfinity);
        }

        /// <summary>
        /// <para>Casts a ray into the scene.</para>
        /// </summary>
        /// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than or equal to this value.</param>
        /// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than or equal to this value.</param>
        /// <param name="origin">The point in 2D space where the ray originates.</param>
        /// <param name="direction">Vector representing the direction of the ray.</param>
        /// <param name="results">Array to receive results.</param>
        /// <param name="distance">Maximum distance over which to cast the ray.</param>
        /// <param name="layerMask">Filter to check objects only on specific layers.</param>
        /// <returns>
        /// <para>The number of results returned.</para>
        /// </returns>
        public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_RaycastNonAlloc(ref origin, ref direction, results, distance, layerMask, minDepth, maxDepth);
        }

        internal static void SetEditorDragMovement(bool dragging, GameObject[] objs)
        {
            foreach (Rigidbody2D rigidbodyd in m_LastDisabledRigidbody2D)
            {
                if (rigidbodyd != null)
                {
                    rigidbodyd.SetDragBehaviour(false);
                }
            }
            m_LastDisabledRigidbody2D.Clear();
            if (dragging)
            {
                foreach (GameObject obj2 in objs)
                {
                    Rigidbody2D[] componentsInChildren = obj2.GetComponentsInChildren<Rigidbody2D>(false);
                    foreach (Rigidbody2D rigidbodyd2 in componentsInChildren)
                    {
                        m_LastDisabledRigidbody2D.Add(rigidbodyd2);
                        rigidbodyd2.SetDragBehaviour(true);
                    }
                }
            }
        }

        /// <summary>
        /// <para>Set the collision layer mask that indicates which layer(s) the specified layer can collide with.</para>
        /// </summary>
        /// <param name="layer">The layer to set the collision layer mask for.</param>
        /// <param name="layerMask">A mask where each bit indicates a layer and whether it can collide with layer or not.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetLayerCollisionMask(int layer, int layerMask);

        /// <summary>
        /// <para>Should the collider gizmos always be shown even when they are not selected?</para>
        /// </summary>
        public static bool alwaysShowColliders { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>A rigid-body cannot sleep if its angular velocity is above this tolerance.</para>
        /// </summary>
        public static float angularSleepTolerance { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The scale factor that controls how fast overlaps are resolved.</para>
        /// </summary>
        public static float baumgarteScale { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The scale factor that controls how fast TOI overlaps are resolved.</para>
        /// </summary>
        public static float baumgarteTOIScale { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Whether or not to stop reporting collision callbacks immediately if any of the objects involved in the collision are deleted/moved. </para>
        /// </summary>
        public static bool changeStopsCallbacks { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The color used by the gizmos to show all collider AABBs.</para>
        /// </summary>
        public static Color colliderAABBColor
        {
            get
            {
                Color color;
                INTERNAL_get_colliderAABBColor(out color);
                return color;
            }
            set
            {
                INTERNAL_set_colliderAABBColor(ref value);
            }
        }

        /// <summary>
        /// <para>The color used by the gizmos to show all asleep colliders (collider is asleep when the body is asleep).</para>
        /// </summary>
        public static Color colliderAsleepColor
        {
            get
            {
                Color color;
                INTERNAL_get_colliderAsleepColor(out color);
                return color;
            }
            set
            {
                INTERNAL_set_colliderAsleepColor(ref value);
            }
        }

        /// <summary>
        /// <para>The color used by the gizmos to show all awake colliders (collider is awake when the body is awake).</para>
        /// </summary>
        public static Color colliderAwakeColor
        {
            get
            {
                Color color;
                INTERNAL_get_colliderAwakeColor(out color);
                return color;
            }
            set
            {
                INTERNAL_set_colliderAwakeColor(ref value);
            }
        }

        /// <summary>
        /// <para>The color used by the gizmos to show all collider contacts.</para>
        /// </summary>
        public static Color colliderContactColor
        {
            get
            {
                Color color;
                INTERNAL_get_colliderContactColor(out color);
                return color;
            }
            set
            {
                INTERNAL_set_colliderContactColor(ref value);
            }
        }

        /// <summary>
        /// <para>The scale of the contact arrow used by the collider gizmos.</para>
        /// </summary>
        public static float contactArrowScale { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Whether to stop processing collision callbacks if any of the objects involved in the collision are deleted or not.</para>
        /// </summary>
        [Obsolete("Physics2D.deleteStopsCallbacks is deprecated. Use Physics2D.changeStopsCallbacks instead. (UnityUpgradable) -> changeStopsCallbacks", true)]
        public static bool deleteStopsCallbacks
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        /// <summary>
        /// <para>Acceleration due to gravity.</para>
        /// </summary>
        public static Vector2 gravity
        {
            get
            {
                Vector2 vector;
                INTERNAL_get_gravity(out vector);
                return vector;
            }
            set
            {
                INTERNAL_set_gravity(ref value);
            }
        }

        /// <summary>
        /// <para>A rigid-body cannot sleep if its linear velocity is above this tolerance.</para>
        /// </summary>
        public static float linearSleepTolerance { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The maximum angular position correction used when solving constraints.  This helps to prevent overshoot.</para>
        /// </summary>
        public static float maxAngularCorrection { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The maximum linear position correction used when solving constraints.  This helps to prevent overshoot.</para>
        /// </summary>
        public static float maxLinearCorrection { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The maximum angular speed of a rigid-body per physics update.  Increasing this can cause numerical problems.</para>
        /// </summary>
        public static float maxRotationSpeed { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The maximum linear speed of a rigid-body per physics update.  Increasing this can cause numerical problems.</para>
        /// </summary>
        public static float maxTranslationSpeed { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The minimum contact penetration radius allowed before any separation impulse force is applied.  Extreme caution should be used when modifying this value as making this smaller means that polygons will have an insufficient buffer for continuous collision and making it larger may create artefacts for vertex collision.</para>
        /// </summary>
        public static float minPenetrationForPenalty { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The number of iterations of the physics solver when considering objects' positions.</para>
        /// </summary>
        public static int positionIterations { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Do raycasts detect Colliders configured as triggers?</para>
        /// </summary>
        public static bool queriesHitTriggers { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Do ray/line casts that start inside a collider(s) detect those collider(s)?</para>
        /// </summary>
        public static bool queriesStartInColliders { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Do raycasts detect Colliders configured as triggers?</para>
        /// </summary>
        [Obsolete("Physics2D.raycastsHitTriggers is deprecated. Use Physics2D.queriesHitTriggers instead. (UnityUpgradable) -> queriesHitTriggers", true)]
        public static bool raycastsHitTriggers
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        /// <summary>
        /// <para>Do ray/line casts that start inside a collider(s) detect those collider(s)?</para>
        /// </summary>
        [Obsolete("Physics2D.raycastsStartInColliders is deprecated. Use Physics2D.queriesStartInColliders instead. (UnityUpgradable) -> queriesStartInColliders", true)]
        public static bool raycastsStartInColliders
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        /// <summary>
        /// <para>Should the collider gizmos show the AABBs for each collider?</para>
        /// </summary>
        public static bool showColliderAABB { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should the collider gizmos show current contacts for each collider?</para>
        /// </summary>
        public static bool showColliderContacts { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should the collider gizmos show the sleep-state for each collider?</para>
        /// </summary>
        public static bool showColliderSleep { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The time in seconds that a rigid-body must be still before it will go to sleep.</para>
        /// </summary>
        public static float timeToSleep { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The number of iterations of the physics solver when considering objects' velocities.</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public static int velocityIterations { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Any collisions with a relative linear velocity below this threshold will be treated as inelastic.</para>
        /// </summary>
        public static float velocityThreshold { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

