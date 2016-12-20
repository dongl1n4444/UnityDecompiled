﻿namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Helper functions for Scene View style 3D GUI.</para>
    /// </summary>
    public sealed class HandleUtility
    {
        internal static Transform[] ignoreRaySnapObjects = null;
        private const float kHandleSize = 80f;
        internal const float kPickDistance = 5f;
        private static Vector3[] points = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
        internal static float s_CustomPickDistance = 5f;
        private static Material s_HandleDottedWireMaterial;
        private static Material s_HandleDottedWireMaterial2D;
        private static int s_HandleDottedWireTextureIndex;
        private static int s_HandleDottedWireTextureIndex2D;
        private static Material s_HandleMaterial;
        private static Material s_HandleWireMaterial;
        private static Material s_HandleWireMaterial2D;
        private static int s_HandleWireTextureIndex;
        private static int s_HandleWireTextureIndex2D;
        private static int s_NearestControl;
        private static float s_NearestDistance;
        private static Stack s_SavedCameras = new Stack();
        private static bool s_UseYSign = false;
        private static bool s_UseYSignZoom = false;

        /// <summary>
        /// <para>Record a distance measurement from a handle.</para>
        /// </summary>
        /// <param name="controlId"></param>
        /// <param name="distance"></param>
        public static void AddControl(int controlId, float distance)
        {
            if ((distance < s_CustomPickDistance) && (distance > 5f))
            {
                distance = 5f;
            }
            if (distance <= s_NearestDistance)
            {
                s_NearestDistance = distance;
                s_NearestControl = controlId;
            }
        }

        /// <summary>
        /// <para>Add the ID for a default control. This will be picked if nothing else is.</para>
        /// </summary>
        /// <param name="controlId"></param>
        public static void AddDefaultControl(int controlId)
        {
            AddControl(controlId, 5f);
        }

        internal static void ApplyDottedWireMaterial()
        {
            handleDottedWireMaterial.SetPass(0);
            int textureIndex = (Camera.current == null) ? s_HandleDottedWireTextureIndex2D : s_HandleDottedWireTextureIndex;
            Internal_SetHandleWireTextureIndex(textureIndex);
        }

        internal static void ApplyWireMaterial()
        {
            handleWireMaterial.SetPass(0);
            int textureIndex = (Camera.current == null) ? s_HandleWireTextureIndex2D : s_HandleWireTextureIndex;
            Internal_SetHandleWireTextureIndex(textureIndex);
        }

        [RequiredByNativeCode]
        private static void BeginHandles()
        {
            Handles.Init();
            if (Event.current.type == EventType.Layout)
            {
                s_NearestControl = 0;
                s_NearestDistance = 5f;
            }
            Handles.lighting = true;
            Handles.color = Color.white;
            s_CustomPickDistance = 5f;
            Handles.Internal_SetCurrentCamera(null);
            EditorGUI.s_DelayedTextEditor.BeginGUI();
        }

        /// <summary>
        /// <para>Map a mouse drag onto a movement along a line in 3D space.</para>
        /// </summary>
        /// <param name="src">The source point of the drag.</param>
        /// <param name="dest">The destination point of the drag.</param>
        /// <param name="srcPosition">The 3D position the dragged object had at src ray.</param>
        /// <param name="constraintDir">3D direction of constrained movement.</param>
        /// <returns>
        /// <para>The distance travelled along constraintDir.</para>
        /// </returns>
        public static float CalcLineTranslation(Vector2 src, Vector2 dest, Vector3 srcPosition, Vector3 constraintDir)
        {
            srcPosition = Handles.matrix.MultiplyPoint(srcPosition);
            constraintDir = Handles.matrix.MultiplyVector(constraintDir);
            float num = 1f;
            Vector3 forward = Camera.current.transform.forward;
            if (Vector3.Dot(constraintDir, forward) < 0f)
            {
                num = -1f;
            }
            Vector3 vector2 = constraintDir;
            vector2.y = -vector2.y;
            Camera current = Camera.current;
            Vector2 vector3 = EditorGUIUtility.PixelsToPoints(current.WorldToScreenPoint(srcPosition));
            Vector2 vector4 = EditorGUIUtility.PixelsToPoints(current.WorldToScreenPoint(srcPosition + ((Vector3) (constraintDir * num))));
            Vector2 vector5 = dest;
            Vector2 vector6 = src;
            if (vector3 == vector4)
            {
                return 0f;
            }
            vector5.y = -vector5.y;
            vector6.y = -vector6.y;
            float num3 = GetParametrization(vector6, vector3, vector4);
            return ((GetParametrization(vector5, vector3, vector4) - num3) * num);
        }

        internal static float CalcRayPlaceOffset(Transform[] objects, Vector3 normal)
        {
            return INTERNAL_CALL_CalcRayPlaceOffset(objects, ref normal);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool CameraNeedsToRenderIntoRT(Camera camera);
        /// <summary>
        /// <para>Get the point on an arc (in 3D space) which is closest to the current mouse position.</para>
        /// </summary>
        /// <param name="center"></param>
        /// <param name="normal"></param>
        /// <param name="from"></param>
        /// <param name="angle"></param>
        /// <param name="radius"></param>
        public static Vector3 ClosestPointToArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
        {
            Vector3[] dest = new Vector3[60];
            Handles.SetDiscSectionPoints(dest, 60, center, normal, from, angle, radius);
            return ClosestPointToPolyLine(dest);
        }

        /// <summary>
        /// <para>Get the point on an disc (in 3D space) which is closest to the current mouse position.</para>
        /// </summary>
        /// <param name="center"></param>
        /// <param name="normal"></param>
        /// <param name="radius"></param>
        public static Vector3 ClosestPointToDisc(Vector3 center, Vector3 normal, float radius)
        {
            Vector3 from = Vector3.Cross(normal, Vector3.up);
            if (from.sqrMagnitude < 0.001f)
            {
                from = Vector3.Cross(normal, Vector3.right);
            }
            return ClosestPointToArc(center, normal, from, 360f, radius);
        }

        /// <summary>
        /// <para>Get the point on a polyline (in 3D space) which is closest to the current mouse position.</para>
        /// </summary>
        /// <param name="vertices"></param>
        public static Vector3 ClosestPointToPolyLine(params Vector3[] vertices)
        {
            float num = DistanceToLine(vertices[0], vertices[1]);
            int index = 0;
            for (int i = 2; i < vertices.Length; i++)
            {
                float num4 = DistanceToLine(vertices[i - 1], vertices[i]);
                if (num4 < num)
                {
                    num = num4;
                    index = i - 1;
                }
            }
            Vector3 world = vertices[index];
            Vector3 vector2 = vertices[index + 1];
            Vector2 vector3 = Event.current.mousePosition - WorldToGUIPoint(world);
            Vector2 vector4 = WorldToGUIPoint(vector2) - WorldToGUIPoint(world);
            float magnitude = vector4.magnitude;
            float num6 = Vector3.Dot((Vector3) vector4, (Vector3) vector3);
            if (magnitude > 1E-06f)
            {
                num6 /= magnitude * magnitude;
            }
            num6 = Mathf.Clamp01(num6);
            return Vector3.Lerp(world, vector2, num6);
        }

        /// <summary>
        /// <para>Calculate distance between a point and a Bezier curve.</para>
        /// </summary>
        /// <param name="point"></param>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <param name="startTangent"></param>
        /// <param name="endTangent"></param>
        public static float DistancePointBezier(Vector3 point, Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent)
        {
            return INTERNAL_CALL_DistancePointBezier(ref point, ref startPosition, ref endPosition, ref startTangent, ref endTangent);
        }

        /// <summary>
        /// <para>Calculate distance between a point and a line.</para>
        /// </summary>
        /// <param name="point"></param>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        public static float DistancePointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            return Vector3.Magnitude(ProjectPointLine(point, lineStart, lineEnd) - point);
        }

        /// <summary>
        /// <para>Distance from a point p in 2d to a line defined by two points a and b.</para>
        /// </summary>
        /// <param name="p"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static float DistancePointToLine(Vector2 p, Vector2 a, Vector2 b)
        {
            Vector2 vector = b - a;
            return (Mathf.Abs((float) (((b.x - a.x) * (a.y - p.y)) - ((a.x - p.x) * (b.y - a.y)))) / vector.magnitude);
        }

        /// <summary>
        /// <para>Distance from a point p in 2d to a line segment defined by two points a and b.</para>
        /// </summary>
        /// <param name="p"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static float DistancePointToLineSegment(Vector2 p, Vector2 a, Vector2 b)
        {
            Vector2 vector = b - a;
            float sqrMagnitude = vector.sqrMagnitude;
            if (sqrMagnitude == 0.0)
            {
                Vector2 vector2 = p - a;
                return vector2.magnitude;
            }
            float num3 = Vector2.Dot(p - a, b - a) / sqrMagnitude;
            if (num3 < 0.0)
            {
                Vector2 vector3 = p - a;
                return vector3.magnitude;
            }
            if (num3 > 1.0)
            {
                Vector2 vector4 = p - b;
                return vector4.magnitude;
            }
            Vector2 vector5 = a + ((Vector2) (num3 * (b - a)));
            Vector2 vector6 = p - vector5;
            return vector6.magnitude;
        }

        /// <summary>
        /// <para>Pixel distance from mouse pointer to a 3D section of a disc.</para>
        /// </summary>
        /// <param name="center"></param>
        /// <param name="normal"></param>
        /// <param name="from"></param>
        /// <param name="angle"></param>
        /// <param name="radius"></param>
        public static float DistanceToArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
        {
            Vector3[] dest = new Vector3[60];
            Handles.SetDiscSectionPoints(dest, 60, center, normal, from, angle, radius);
            return DistanceToPolyLine(dest);
        }

        /// <summary>
        /// <para>Pixel distance from mouse pointer to camera facing circle.</para>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        public static float DistanceToCircle(Vector3 position, float radius)
        {
            Vector2 vector = WorldToGUIPoint(position);
            Camera current = Camera.current;
            Vector2 zero = Vector2.zero;
            if (current != null)
            {
                zero = WorldToGUIPoint(position + ((Vector3) (current.transform.right * radius)));
                Vector2 vector3 = vector - zero;
                radius = vector3.magnitude;
            }
            Vector2 vector4 = vector - Event.current.mousePosition;
            float magnitude = vector4.magnitude;
            if (magnitude < radius)
            {
                return 0f;
            }
            return (magnitude - radius);
        }

        internal static float DistanceToDiamond(Vector3 position, Quaternion rotation, float size)
        {
            return DistanceToDiamondInternal(position, rotation, size, Event.current.mousePosition);
        }

        internal static float DistanceToDiamondInternal(Vector3 position, Quaternion rotation, float size, Vector2 mousePosition)
        {
            Vector3 vector = (Vector3) (rotation * new Vector3(size, 0f, 0f));
            Vector3 vector2 = (Vector3) (rotation * new Vector3(0f, size, 0f));
            points[0] = (Vector3) WorldToGUIPoint(position + vector);
            points[1] = (Vector3) WorldToGUIPoint(position - vector2);
            points[2] = (Vector3) WorldToGUIPoint(position - vector);
            points[3] = (Vector3) WorldToGUIPoint(position + vector2);
            points[4] = points[0];
            Vector2 p = mousePosition;
            bool flag = false;
            int index = 4;
            for (int i = 0; i < 5; i++)
            {
                if (((points[i].y > p.y) != (points[index].y > p.y)) && (p.x < ((((points[index].x - points[i].x) * (p.y - points[i].y)) / (points[index].y - points[i].y)) + points[i].x)))
                {
                    flag = !flag;
                }
                index = i;
            }
            if (!flag)
            {
                float num4 = -1f;
                index = 1;
                for (int j = 0; j < 4; j++)
                {
                    float num3 = DistancePointToLineSegment(p, points[j], points[index++]);
                    if ((num3 < num4) || (num4 < 0f))
                    {
                        num4 = num3;
                    }
                }
                return num4;
            }
            return 0f;
        }

        /// <summary>
        /// <para>Pixel distance from mouse pointer to a 3D disc.</para>
        /// </summary>
        /// <param name="center"></param>
        /// <param name="normal"></param>
        /// <param name="radius"></param>
        public static float DistanceToDisc(Vector3 center, Vector3 normal, float radius)
        {
            Vector3 from = Vector3.Cross(normal, Vector3.up);
            if (from.sqrMagnitude < 0.001f)
            {
                from = Vector3.Cross(normal, Vector3.right);
            }
            return DistanceToArc(center, normal, from, 360f, radius);
        }

        /// <summary>
        /// <para>Pixel distance from mouse pointer to line.</para>
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public static float DistanceToLine(Vector3 p1, Vector3 p2)
        {
            p1 = (Vector3) WorldToGUIPoint(p1);
            p2 = (Vector3) WorldToGUIPoint(p2);
            float num = DistancePointLine((Vector3) Event.current.mousePosition, p1, p2);
            if (num < 0f)
            {
                num = 0f;
            }
            return num;
        }

        /// <summary>
        /// <para>Pixel distance from mouse pointer to a polyline.</para>
        /// </summary>
        /// <param name="points"></param>
        public static float DistanceToPolyLine(params Vector3[] points)
        {
            float num = DistanceToLine(points[0], points[1]);
            for (int i = 2; i < points.Length; i++)
            {
                float num3 = DistanceToLine(points[i - 1], points[i]);
                if (num3 < num)
                {
                    num = num3;
                }
            }
            return num;
        }

        /// <summary>
        /// <para>Pixel distance from mouse pointer to a rectangle on screen.</para>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="size"></param>
        public static float DistanceToRectangle(Vector3 position, Quaternion rotation, float size)
        {
            return DistanceToRectangleInternal(position, rotation, new Vector2(size, size));
        }

        internal static float DistanceToRectangleInternal(Vector3 position, Quaternion rotation, Vector2 size)
        {
            Vector3 vector = (Vector3) (rotation * new Vector3(size.x, 0f, 0f));
            Vector3 vector2 = (Vector3) (rotation * new Vector3(0f, size.y, 0f));
            points[0] = (Vector3) WorldToGUIPoint((position + vector) + vector2);
            points[1] = (Vector3) WorldToGUIPoint((position + vector) - vector2);
            points[2] = (Vector3) WorldToGUIPoint((position - vector) - vector2);
            points[3] = (Vector3) WorldToGUIPoint((position - vector) + vector2);
            points[4] = points[0];
            Vector2 mousePosition = Event.current.mousePosition;
            bool flag = false;
            int index = 4;
            for (int i = 0; i < 5; i++)
            {
                if (((points[i].y > mousePosition.y) != (points[index].y > mousePosition.y)) && (mousePosition.x < ((((points[index].x - points[i].x) * (mousePosition.y - points[i].y)) / (points[index].y - points[i].y)) + points[i].x)))
                {
                    flag = !flag;
                }
                index = i;
            }
            if (!flag)
            {
                float num4 = -1f;
                index = 1;
                for (int j = 0; j < 4; j++)
                {
                    float num3 = DistancePointToLineSegment(mousePosition, points[j], points[index++]);
                    if ((num3 < num4) || (num4 < 0f))
                    {
                        num4 = num3;
                    }
                }
                return num4;
            }
            return 0f;
        }

        [RequiredByNativeCode]
        private static void EndHandles()
        {
            EventType type = Event.current.type;
            if (type != EventType.Layout)
            {
                GUIUtility.s_HasKeyboardFocus = false;
                GUIUtility.s_EditorScreenPointOffset = Vector2.zero;
            }
            EditorGUI.s_DelayedTextEditor.EndGUI(type);
        }

        internal static bool FindNearestVertex(Vector2 guiPoint, Transform[] objectsToSearch, out Vector3 vertex)
        {
            Camera current = Camera.current;
            Vector2 screenPoint = EditorGUIUtility.PointsToPixels(guiPoint);
            screenPoint.y = current.pixelRect.yMax - screenPoint.y;
            return Internal_FindNearestVertex(current, screenPoint, objectsToSearch, ignoreRaySnapObjects, out vertex);
        }

        internal static GameObject FindSelectionBase(GameObject go)
        {
            if (go != null)
            {
                Transform transform = null;
                switch (PrefabUtility.GetPrefabType(go))
                {
                    case PrefabType.PrefabInstance:
                    case PrefabType.ModelPrefabInstance:
                        transform = PrefabUtility.FindPrefabRoot(go).transform;
                        break;
                }
                for (Transform transform2 = go.transform; transform2 != null; transform2 = transform2.parent)
                {
                    if (transform2 == transform)
                    {
                        return transform2.gameObject;
                    }
                    if (AttributeHelper.GameObjectContainsAttribute(transform2.gameObject, typeof(SelectionBaseAttribute)))
                    {
                        return transform2.gameObject;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// <para>Get world space size of a manipulator handle at given position.</para>
        /// </summary>
        /// <param name="position">The position of the handle in 3d space.</param>
        /// <returns>
        /// <para>A constant screen-size for the handle, based on the distance between from the supplied handle's position to the camera.</para>
        /// </returns>
        public static float GetHandleSize(Vector3 position)
        {
            Camera current = Camera.current;
            position = Handles.matrix.MultiplyPoint(position);
            if (current != null)
            {
                Transform transform = current.transform;
                Vector3 vector = transform.position;
                float z = Vector3.Dot(position - vector, transform.TransformDirection(new Vector3(0f, 0f, 1f)));
                Vector3 vector2 = current.WorldToScreenPoint(vector + transform.TransformDirection(new Vector3(0f, 0f, z)));
                Vector3 vector3 = current.WorldToScreenPoint(vector + transform.TransformDirection(new Vector3(1f, 0f, z)));
                Vector3 vector4 = vector2 - vector3;
                float magnitude = vector4.magnitude;
                return ((80f / Mathf.Max(magnitude, 0.0001f)) * EditorGUIUtility.pixelsPerPoint);
            }
            return 20f;
        }

        internal static float GetParametrization(Vector2 x0, Vector2 x1, Vector2 x2)
        {
            Vector2 vector = x2 - x1;
            return -(Vector2.Dot(x1 - x0, x2 - x1) / vector.sqrMagnitude);
        }

        /// <summary>
        /// <para>Convert 2D GUI position to a world space ray.</para>
        /// </summary>
        /// <param name="position"></param>
        public static Ray GUIPointToWorldRay(Vector2 position)
        {
            if (Camera.current == null)
            {
                Debug.LogError("Unable to convert GUI point to world ray if a camera has not been set up!");
                return new Ray(Vector3.zero, Vector3.forward);
            }
            Vector2 vector2 = EditorGUIUtility.PointsToPixels(GUIClip.Unclip(position));
            vector2.y = Screen.height - vector2.y;
            return Camera.current.ScreenPointToRay((Vector3) new Vector2(vector2.x, vector2.y));
        }

        private static void InitHandleMaterials()
        {
            if (s_HandleWireMaterial == null)
            {
                s_HandleWireMaterial = (Material) EditorGUIUtility.LoadRequired("SceneView/HandleLines.mat");
                s_HandleWireMaterial2D = (Material) EditorGUIUtility.LoadRequired("SceneView/2DHandleLines.mat");
                s_HandleWireTextureIndex = ShaderUtil.GetTextureBindingIndex(s_HandleWireMaterial.shader, Shader.PropertyToID("_MainTex"));
                s_HandleWireTextureIndex2D = ShaderUtil.GetTextureBindingIndex(s_HandleWireMaterial2D.shader, Shader.PropertyToID("_MainTex"));
                s_HandleDottedWireMaterial = (Material) EditorGUIUtility.LoadRequired("SceneView/HandleDottedLines.mat");
                s_HandleDottedWireMaterial2D = (Material) EditorGUIUtility.LoadRequired("SceneView/2DHandleDottedLines.mat");
                s_HandleDottedWireTextureIndex = ShaderUtil.GetTextureBindingIndex(s_HandleDottedWireMaterial.shader, Shader.PropertyToID("_MainTex"));
                s_HandleDottedWireTextureIndex2D = ShaderUtil.GetTextureBindingIndex(s_HandleDottedWireMaterial2D.shader, Shader.PropertyToID("_MainTex"));
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern float INTERNAL_CALL_CalcRayPlaceOffset(Transform[] objects, ref Vector3 normal);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern float INTERNAL_CALL_DistancePointBezier(ref Vector3 point, ref Vector3 startPosition, ref Vector3 endPosition, ref Vector3 startTangent, ref Vector3 endTangent);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_Internal_FindNearestVertex(Camera cam, ref Vector2 screenPoint, Transform[] objectsToSearch, Transform[] ignoreObjects, out Vector3 vertex);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern GameObject INTERNAL_CALL_Internal_PickClosestGO(Camera cam, int layers, ref Vector2 position, GameObject[] ignore, GameObject[] filter, out int materialIndex);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern GameObject[] INTERNAL_CALL_Internal_PickRectObjects(Camera cam, ref Rect rect, bool selectPrefabRoots);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_IntersectRayMesh(ref Ray ray, Mesh mesh, ref Matrix4x4 matrix, out RaycastHit hit);
        private static bool Internal_FindNearestVertex(Camera cam, Vector2 screenPoint, Transform[] objectsToSearch, Transform[] ignoreObjects, out Vector3 vertex)
        {
            return INTERNAL_CALL_Internal_FindNearestVertex(cam, ref screenPoint, objectsToSearch, ignoreObjects, out vertex);
        }

        internal static GameObject Internal_PickClosestGO(Camera cam, int layers, Vector2 position, GameObject[] ignore, GameObject[] filter, out int materialIndex)
        {
            return INTERNAL_CALL_Internal_PickClosestGO(cam, layers, ref position, ignore, filter, out materialIndex);
        }

        internal static GameObject[] Internal_PickRectObjects(Camera cam, Rect rect, bool selectPrefabRoots)
        {
            return INTERNAL_CALL_Internal_PickRectObjects(cam, ref rect, selectPrefabRoots);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_Repaint();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_SetHandleWireTextureIndex(int textureIndex);
        internal static bool IntersectRayMesh(Ray ray, Mesh mesh, Matrix4x4 matrix, out RaycastHit hit)
        {
            return INTERNAL_CALL_IntersectRayMesh(ref ray, mesh, ref matrix, out hit);
        }

        public static GameObject PickGameObject(Vector2 position, out int materialIndex)
        {
            return PickGameObject(position, null, out materialIndex);
        }

        /// <summary>
        /// <para>Pick game object closest to specified position.</para>
        /// </summary>
        /// <param name="selectPrefabRoot">Select prefab.</param>
        /// <param name="materialIndex">Returns index into material array of the Renderer component that is closest to specified position.</param>
        /// <param name="position"></param>
        public static GameObject PickGameObject(Vector2 position, bool selectPrefabRoot)
        {
            return PickGameObject(position, selectPrefabRoot, null);
        }

        public static GameObject PickGameObject(Vector2 position, GameObject[] ignore, out int materialIndex)
        {
            return PickGameObject(position, ignore, null, out materialIndex);
        }

        public static GameObject PickGameObject(Vector2 position, bool selectPrefabRoot, GameObject[] ignore)
        {
            return PickGameObject(position, selectPrefabRoot, ignore, null);
        }

        internal static GameObject PickGameObject(Vector2 position, GameObject[] ignore, GameObject[] filter, out int materialIndex)
        {
            Camera current = Camera.current;
            int cullingMask = current.cullingMask;
            position = GUIClip.Unclip(position);
            position = EditorGUIUtility.PointsToPixels(position);
            position.y = (Screen.height - position.y) - current.pixelRect.yMin;
            return Internal_PickClosestGO(current, cullingMask, position, ignore, filter, out materialIndex);
        }

        internal static GameObject PickGameObject(Vector2 position, bool selectPrefabRoot, GameObject[] ignore, GameObject[] filter)
        {
            GameObject obj6;
            int num;
            GameObject obj3;
            GameObject go = PickGameObject(position, ignore, filter, out num);
            if ((go == null) || !selectPrefabRoot)
            {
                return go;
            }
            GameObject obj1 = FindSelectionBase(go);
            if (obj1 != null)
            {
                obj3 = obj1;
            }
            else
            {
                obj3 = go;
            }
            Transform activeTransform = Selection.activeTransform;
            if (activeTransform != null)
            {
                obj6 = FindSelectionBase(activeTransform.gameObject);
            }
            GameObject obj4 = (obj6 != null) ? null : activeTransform.gameObject;
            if (obj3 == obj4)
            {
                return go;
            }
            return obj3;
        }

        /// <summary>
        /// <para>Pick GameObjects that lie within a specified screen rectangle.</para>
        /// </summary>
        /// <param name="rect">An screen rectangle specified with pixel coordinates.</param>
        public static GameObject[] PickRectObjects(Rect rect)
        {
            return PickRectObjects(rect, true);
        }

        /// <summary>
        /// <para></para>
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="selectPrefabRootsOnly"></param>
        public static GameObject[] PickRectObjects(Rect rect, bool selectPrefabRootsOnly)
        {
            Camera current = Camera.current;
            rect = EditorGUIUtility.PointsToPixels(rect);
            rect.x /= (float) current.pixelWidth;
            rect.width /= (float) current.pixelWidth;
            rect.y /= (float) current.pixelHeight;
            rect.height /= (float) current.pixelHeight;
            return Internal_PickRectObjects(current, rect, selectPrefabRootsOnly);
        }

        /// <summary>
        /// <para>Returns the parameter for the projection of the point on the given line.</para>
        /// </summary>
        /// <param name="point"></param>
        /// <param name="linePoint"></param>
        /// <param name="lineDirection"></param>
        public static float PointOnLineParameter(Vector3 point, Vector3 linePoint, Vector3 lineDirection)
        {
            return (Vector3.Dot(lineDirection, point - linePoint) / lineDirection.sqrMagnitude);
        }

        /// <summary>
        /// <para>Retrieve all camera settings.</para>
        /// </summary>
        /// <param name="camera"></param>
        public static void PopCamera(Camera camera)
        {
            ((SavedCamera) s_SavedCameras.Pop()).Restore(camera);
        }

        /// <summary>
        /// <para>Project point onto a line.</para>
        /// </summary>
        /// <param name="point"></param>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        public static Vector3 ProjectPointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 rhs = point - lineStart;
            Vector3 vector2 = lineEnd - lineStart;
            float magnitude = vector2.magnitude;
            Vector3 lhs = vector2;
            if (magnitude > 1E-06f)
            {
                lhs = (Vector3) (lhs / magnitude);
            }
            float num2 = Mathf.Clamp(Vector3.Dot(lhs, rhs), 0f, magnitude);
            return (lineStart + ((Vector3) (lhs * num2)));
        }

        /// <summary>
        /// <para>Store all camera settings.</para>
        /// </summary>
        /// <param name="camera"></param>
        public static void PushCamera(Camera camera)
        {
            s_SavedCameras.Push(new SavedCamera(camera));
        }

        /// <summary>
        /// <para>Casts ray against the scene and report if an object lies in its path.</para>
        /// </summary>
        /// <param name="ray"></param>
        /// <returns>
        /// <para>A boxed RaycastHit, null if nothing hit it.</para>
        /// </returns>
        public static object RaySnap(Ray ray)
        {
            RaycastHit[] hitArray = Physics.RaycastAll(ray, float.PositiveInfinity, Camera.current.cullingMask);
            float positiveInfinity = float.PositiveInfinity;
            int index = -1;
            if (ignoreRaySnapObjects != null)
            {
                for (int i = 0; i < hitArray.Length; i++)
                {
                    if (hitArray[i].collider.isTrigger || (hitArray[i].distance >= positiveInfinity))
                    {
                        continue;
                    }
                    bool flag = false;
                    for (int j = 0; j < ignoreRaySnapObjects.Length; j++)
                    {
                        if (hitArray[i].transform == ignoreRaySnapObjects[j])
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        positiveInfinity = hitArray[i].distance;
                        index = i;
                    }
                }
            }
            else
            {
                for (int k = 0; k < hitArray.Length; k++)
                {
                    if (hitArray[k].distance < positiveInfinity)
                    {
                        positiveInfinity = hitArray[k].distance;
                        index = k;
                    }
                }
            }
            if (index >= 0)
            {
                return hitArray[index];
            }
            return null;
        }

        /// <summary>
        /// <para>Repaint the current view.</para>
        /// </summary>
        public static void Repaint()
        {
            Internal_Repaint();
        }

        [RequiredByNativeCode]
        private static void SetViewInfo(Vector2 screenPosition)
        {
            GUIUtility.s_EditorScreenPointOffset = screenPosition;
        }

        /// <summary>
        /// <para>Calculate a rectangle to display a 2D GUI element near a projected point in 3D space.</para>
        /// </summary>
        /// <param name="position">The world-space position to use.</param>
        /// <param name="content">The content to make room for.</param>
        /// <param name="style">The style to use. The style's alignment.</param>
        public static Rect WorldPointToSizedRect(Vector3 position, GUIContent content, GUIStyle style)
        {
            Vector2 vector = WorldToGUIPoint(position);
            Vector2 vector2 = style.CalcSize(content);
            Rect rect = new Rect(vector.x, vector.y, vector2.x, vector2.y);
            switch (style.alignment)
            {
                case TextAnchor.UpperCenter:
                    rect.xMin -= rect.width * 0.5f;
                    break;

                case TextAnchor.UpperRight:
                    rect.xMin -= rect.width;
                    break;

                case TextAnchor.MiddleLeft:
                    rect.yMin -= rect.height * 0.5f;
                    break;

                case TextAnchor.MiddleCenter:
                    rect.xMin -= rect.width * 0.5f;
                    rect.yMin -= rect.height * 0.5f;
                    break;

                case TextAnchor.MiddleRight:
                    rect.xMin -= rect.width;
                    rect.yMin -= rect.height * 0.5f;
                    break;

                case TextAnchor.LowerLeft:
                    rect.yMin -= rect.height * 0.5f;
                    break;

                case TextAnchor.LowerCenter:
                    rect.xMin -= rect.width * 0.5f;
                    rect.yMin -= rect.height;
                    break;

                case TextAnchor.LowerRight:
                    rect.xMin -= rect.width;
                    rect.yMin -= rect.height;
                    break;
            }
            return style.padding.Add(rect);
        }

        /// <summary>
        /// <para>Convert world space point to a 2D GUI position.</para>
        /// </summary>
        /// <param name="world">Point in world space.</param>
        public static Vector2 WorldToGUIPoint(Vector3 world)
        {
            world = Handles.matrix.MultiplyPoint(world);
            Camera current = Camera.current;
            if (current != null)
            {
                Vector2 position = current.WorldToScreenPoint(world);
                position.y = Screen.height - position.y;
                return GUIClip.Clip(EditorGUIUtility.PixelsToPoints(position));
            }
            return new Vector2(world.x, world.y);
        }

        /// <summary>
        /// <para>Get standard acceleration for dragging values (Read Only).</para>
        /// </summary>
        public static float acceleration
        {
            get
            {
                return ((!Event.current.shift ? ((float) 1) : ((float) 4)) * (!Event.current.alt ? 1f : 0.25f));
            }
        }

        private static Material handleDottedWireMaterial
        {
            get
            {
                InitHandleMaterials();
                return ((Camera.current == null) ? s_HandleDottedWireMaterial2D : s_HandleDottedWireMaterial);
            }
        }

        public static Material handleMaterial
        {
            get
            {
                if (s_HandleMaterial == null)
                {
                    s_HandleMaterial = (Material) EditorGUIUtility.Load("SceneView/Handles.mat");
                }
                return s_HandleMaterial;
            }
        }

        private static Material handleWireMaterial
        {
            get
            {
                InitHandleMaterials();
                return ((Camera.current == null) ? s_HandleWireMaterial2D : s_HandleWireMaterial);
            }
        }

        public static int nearestControl
        {
            get
            {
                return ((s_NearestDistance > 5f) ? 0 : s_NearestControl);
            }
            set
            {
                s_NearestControl = value;
            }
        }

        /// <summary>
        /// <para>Get nice mouse delta to use for dragging a float value (Read Only).</para>
        /// </summary>
        public static float niceMouseDelta
        {
            get
            {
                Vector2 delta = Event.current.delta;
                delta.y = -delta.y;
                float introduced2 = Mathf.Abs(delta.x);
                float introduced3 = Mathf.Abs((float) (introduced2 - Mathf.Abs(delta.y)));
                float a = Mathf.Abs(delta.x);
                if ((introduced3 / Mathf.Max(a, Mathf.Abs(delta.y))) > 0.1f)
                {
                    float introduced5 = Mathf.Abs(delta.x);
                    if (introduced5 > Mathf.Abs(delta.y))
                    {
                        s_UseYSign = false;
                    }
                    else
                    {
                        s_UseYSign = true;
                    }
                }
                if (s_UseYSign)
                {
                    float introduced6 = Mathf.Sign(delta.y);
                    return ((introduced6 * delta.magnitude) * acceleration);
                }
                float introduced7 = Mathf.Sign(delta.x);
                return ((introduced7 * delta.magnitude) * acceleration);
            }
        }

        /// <summary>
        /// <para>Get nice mouse delta to use for zooming (Read Only).</para>
        /// </summary>
        public static float niceMouseDeltaZoom
        {
            get
            {
                Vector2 vector = -Event.current.delta;
                float introduced2 = Mathf.Abs(vector.x);
                float introduced3 = Mathf.Abs((float) (introduced2 - Mathf.Abs(vector.y)));
                float a = Mathf.Abs(vector.x);
                if ((introduced3 / Mathf.Max(a, Mathf.Abs(vector.y))) > 0.1f)
                {
                    float introduced5 = Mathf.Abs(vector.x);
                    if (introduced5 > Mathf.Abs(vector.y))
                    {
                        s_UseYSignZoom = false;
                    }
                    else
                    {
                        s_UseYSignZoom = true;
                    }
                }
                if (s_UseYSignZoom)
                {
                    float introduced6 = Mathf.Sign(vector.y);
                    return ((introduced6 * vector.magnitude) * acceleration);
                }
                float introduced7 = Mathf.Sign(vector.x);
                return ((introduced7 * vector.magnitude) * acceleration);
            }
        }

        private sealed class SavedCamera
        {
            private CameraClearFlags clearFlags;
            private int cullingMask;
            private float far;
            private float fov;
            private bool isOrtho;
            private float near;
            private float orthographicSize;
            private Rect pixelRect;
            private Vector3 pos;
            private Quaternion rot;

            internal SavedCamera(Camera source)
            {
                this.near = source.nearClipPlane;
                this.far = source.farClipPlane;
                this.pixelRect = source.pixelRect;
                this.pos = source.transform.position;
                this.rot = source.transform.rotation;
                this.clearFlags = source.clearFlags;
                this.cullingMask = source.cullingMask;
                this.fov = source.fieldOfView;
                this.orthographicSize = source.orthographicSize;
                this.isOrtho = source.orthographic;
            }

            internal void Restore(Camera dest)
            {
                dest.nearClipPlane = this.near;
                dest.farClipPlane = this.far;
                dest.pixelRect = this.pixelRect;
                dest.transform.position = this.pos;
                dest.transform.rotation = this.rot;
                dest.clearFlags = this.clearFlags;
                dest.fieldOfView = this.fov;
                dest.orthographicSize = this.orthographicSize;
                dest.orthographic = this.isOrtho;
                dest.cullingMask = this.cullingMask;
            }
        }
    }
}

