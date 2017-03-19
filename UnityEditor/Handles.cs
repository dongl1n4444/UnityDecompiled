namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Internal;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Custom 3D GUI controls and drawing in the scene view.</para>
    /// </summary>
    public sealed class Handles
    {
        [CompilerGenerated]
        private static CapFunction <>f__mg$cache0;
        [CompilerGenerated]
        private static CapFunction <>f__mg$cache1;
        [CompilerGenerated]
        private static CapFunction <>f__mg$cache2;
        [CompilerGenerated]
        private static CapFunction <>f__mg$cache3;
        [CompilerGenerated]
        private static CapFunction <>f__mg$cache4;
        [CompilerGenerated]
        private static CapFunction <>f__mg$cache5;
        [CompilerGenerated]
        private static CapFunction <>f__mg$cache6;
        [CompilerGenerated]
        private static CapFunction <>f__mg$cache7;
        [CompilerGenerated]
        private static CapFunction <>f__mg$cache8;
        [CompilerGenerated]
        private static CapFunction <>f__mg$cache9;
        internal static float backfaceAlphaMultiplier = 0.2f;
        private const float k_BoneThickness = 0.08f;
        private const int kMaxDottedLineVertices = 0x3e8;
        private static Color lineTransparency = new Color(1f, 1f, 1f, 0.75f);
        internal static Color s_BoundingBoxHandleColor = ((Color) (new Color(255f, 255f, 255f, 150f) / 255f));
        internal static int s_ButtonHash = "ButtonHash".GetHashCode();
        internal static PrefColor s_CenterColor = new PrefColor("Scene/Center Axis", 0.8f, 0.8f, 0.8f, 0.93f);
        internal static Color s_ColliderHandleColor = ((Color) (new Color(145f, 244f, 139f, 210f) / 255f));
        internal static Color s_ColliderHandleColorDisabled = ((Color) (new Color(84f, 200f, 77f, 140f) / 255f));
        internal static Mesh s_ConeMesh;
        internal static Mesh s_CubeMesh;
        internal static Mesh s_CylinderMesh;
        internal static int s_DiscHash = "DiscHash".GetHashCode();
        internal static int s_FreeMoveHandleHash = "FreeMoveHandleHash".GetHashCode();
        private static bool s_FreeMoveMode = false;
        internal static int s_FreeRotateHandleHash = "FreeRotateHandleHash".GetHashCode();
        private static Vector3 s_PlanarHandlesOctant = Vector3.one;
        internal static Mesh s_QuadMesh;
        internal static int s_RadiusHandleHash = "RadiusHandleHash".GetHashCode();
        private static Vector3[] s_RectangleCapPointsCache = new Vector3[5];
        private static Vector3[] s_RectangleHandlePointsCache = new Vector3[5];
        internal static int s_ScaleSliderHash = "ScaleSliderHash".GetHashCode();
        internal static int s_ScaleValueHandleHash = "ScaleValueHandleHash".GetHashCode();
        internal static PrefColor s_SecondaryColor = new PrefColor("Scene/Guide Line", 0.5f, 0.5f, 0.5f, 0.2f);
        internal static PrefColor s_SelectedColor = new PrefColor("Scene/Selected Axis", 0.9647059f, 0.9490196f, 0.1960784f, 0.89f);
        internal static int s_Slider2DHash = "Slider2DHash".GetHashCode();
        internal static int s_SliderHash = "SliderHash".GetHashCode();
        internal static Mesh s_SphereMesh;
        internal static PrefColor s_XAxisColor = new PrefColor("Scene/X Axis", 0.8588235f, 0.2431373f, 0.1137255f, 0.93f);
        internal static int s_xAxisMoveHandleHash = "xAxisFreeMoveHandleHash".GetHashCode();
        internal static int s_xyAxisMoveHandleHash = "xyAxisFreeMoveHandleHash".GetHashCode();
        internal static int s_xzAxisMoveHandleHash = "xzAxisFreeMoveHandleHash".GetHashCode();
        internal static PrefColor s_YAxisColor = new PrefColor("Scene/Y Axis", 0.6039216f, 0.9529412f, 0.282353f, 0.93f);
        internal static int s_yAxisMoveHandleHash = "yAxisFreeMoveHandleHash".GetHashCode();
        internal static int s_yzAxisMoveHandleHash = "yzAxisFreeMoveHandleHash".GetHashCode();
        internal static PrefColor s_ZAxisColor = new PrefColor("Scene/Z Axis", 0.227451f, 0.4784314f, 0.972549f, 0.93f);
        internal static int s_zAxisMoveHandleHash = "xAxisFreeMoveHandleHash".GetHashCode();
        internal static float staticBlend = 0.6f;
        internal static Color staticColor = new Color(0.5f, 0.5f, 0.5f, 0f);
        private static Vector3[] verts = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };

        [Obsolete("Use ArrowHandleCap instead")]
        public static void ArrowCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Vector3 forward = (Vector3) (rotation * Vector3.forward);
                ConeCap(controlID, position + ((Vector3) (forward * size)), Quaternion.LookRotation(forward), size * 0.2f);
                DrawLine(position, position + ((Vector3) ((forward * size) * 0.9f)));
            }
        }

        /// <summary>
        /// <para>Draw an arrow like those used by the move tool.</para>
        /// </summary>
        /// <param name="controlID">The control ID for the handle.</param>
        /// <param name="position">The position of the handle in the space of Handles.matrix.</param>
        /// <param name="rotation">The rotation of the handle in the space of Handles.matrix.</param>
        /// <param name="size">The size of the handle in the space of Handles.matrix. Use HandleUtility.GetHandleSize if you want a constant screen-space size.</param>
        /// <param name="eventType">Event type for the handle to act upon. By design it handles EventType.Layout and EventType.Repaint events.</param>
        public static void ArrowHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            if (eventType != EventType.Layout)
            {
                if (eventType == EventType.Repaint)
                {
                    Vector3 forward = (Vector3) (rotation * Vector3.forward);
                    ConeHandleCap(controlID, position + ((Vector3) (forward * size)), Quaternion.LookRotation(forward), size * 0.2f, eventType);
                    DrawLine(position, position + ((Vector3) ((forward * size) * 0.9f)));
                }
            }
            else
            {
                Vector3 vector = (Vector3) (rotation * Vector3.forward);
                HandleUtility.AddControl(controlID, HandleUtility.DistanceToLine(position, position + ((Vector3) ((vector * size) * 0.9f))));
                HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(position + ((Vector3) (vector * size)), size * 0.2f));
            }
        }

        /// <summary>
        /// <para>Begin a 2D GUI block inside the 3D handle GUI.</para>
        /// </summary>
        /// <param name="position">The position and size of the 2D GUI area.</param>
        public static void BeginGUI()
        {
            if ((Camera.current != null) && (Event.current.type == EventType.Repaint))
            {
                GUIClip.Reapply();
            }
        }

        /// <summary>
        /// <para>Begin a 2D GUI block inside the 3D handle GUI.</para>
        /// </summary>
        /// <param name="position">The position and size of the 2D GUI area.</param>
        [Obsolete("Please use BeginGUI() with GUILayout.BeginArea(position) / GUILayout.EndArea()")]
        public static void BeginGUI(Rect position)
        {
            GUILayout.BeginArea(position);
        }

        private static bool BeginLineDrawing(Matrix4x4 matrix, bool dottedLines, int mode)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return false;
            }
            Color c = color * lineTransparency;
            if (dottedLines)
            {
                HandleUtility.ApplyDottedWireMaterial(zTest);
            }
            else
            {
                HandleUtility.ApplyWireMaterial(zTest);
            }
            GL.PushMatrix();
            GL.MultMatrix(matrix);
            GL.Begin(mode);
            GL.Color(c);
            return true;
        }

        public static bool Button(Vector3 position, Quaternion direction, float size, float pickSize, CapFunction capFunction) => 
            UnityEditorInternal.Button.Do(GUIUtility.GetControlID(s_ButtonHash, FocusType.Passive), position, direction, size, pickSize, capFunction);

        [Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
        public static bool Button(Vector3 position, Quaternion direction, float size, float pickSize, DrawCapFunction capFunc) => 
            UnityEditorInternal.Button.Do(GUIUtility.GetControlID(s_ButtonHash, FocusType.Passive), position, direction, size, pickSize, capFunc);

        internal static bool Button(int controlID, Vector3 position, Quaternion direction, float size, float pickSize, CapFunction capFunction) => 
            UnityEditorInternal.Button.Do(controlID, position, direction, size, pickSize, capFunction);

        [Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
        internal static bool Button(int controlID, Vector3 position, Quaternion direction, float size, float pickSize, DrawCapFunction capFunc) => 
            UnityEditorInternal.Button.Do(controlID, position, direction, size, pickSize, capFunc);

        [Obsolete("Use CircleHandleCap instead")]
        public static void CircleCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if (Event.current.type == EventType.Repaint)
            {
                StartCapDraw(position, rotation, size);
                Vector3 normal = (Vector3) (rotation * new Vector3(0f, 0f, 1f));
                DrawWireDisc(position, normal, size);
            }
        }

        /// <summary>
        /// <para>Draw a circle handle. Pass this into handle functions.</para>
        /// </summary>
        /// <param name="controlID">The control ID for the handle.</param>
        /// <param name="position">The position of the handle in the space of Handles.matrix.</param>
        /// <param name="rotation">The rotation of the handle in the space of Handles.matrix.</param>
        /// <param name="size">The size of the handle in the space of Handles.matrix. Use HandleUtility.GetHandleSize if you want a constant screen-space size.</param>
        /// <param name="eventType">Event type for the handle to act upon. By design it handles EventType.Layout and EventType.Repaint events.</param>
        public static void CircleHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            if (eventType != EventType.Layout)
            {
                if (eventType == EventType.Repaint)
                {
                    StartCapDraw(position, rotation, size);
                    Vector3 normal = (Vector3) (rotation * new Vector3(0f, 0f, 1f));
                    DrawWireDisc(position, normal, size);
                }
            }
            else
            {
                HandleUtility.AddControl(controlID, HandleUtility.DistanceToRectangle(position, rotation, size));
            }
        }

        /// <summary>
        /// <para>Clears the camera.</para>
        /// </summary>
        /// <param name="position">Where in the Scene to clear.</param>
        /// <param name="camera">The camera to clear.</param>
        public static void ClearCamera(Rect position, Camera camera)
        {
            Event current = Event.current;
            if (camera.targetTexture == null)
            {
                Rect rect = EditorGUIUtility.PointsToPixels(GUIClip.Unclip(position));
                Rect rect2 = new Rect(rect.xMin, Screen.height - rect.yMax, rect.width, rect.height);
                camera.pixelRect = rect2;
            }
            else
            {
                camera.rect = new Rect(0f, 0f, 1f, 1f);
            }
            if (current.type == EventType.Repaint)
            {
                Internal_ClearCamera(camera);
            }
            else
            {
                Internal_SetCurrentCamera(camera);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void ClearHandles();
        [Obsolete("Use ConeHandleCap instead")]
        public static void ConeCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Graphics.DrawMeshNow(s_ConeMesh, StartCapDraw(position, rotation, size));
            }
        }

        internal static Vector3 ConeFrustrumHandle(Quaternion rotation, Vector3 position, Vector3 radiusAngleRange) => 
            DoConeFrustrumHandle(rotation, position, radiusAngleRange);

        internal static Vector2 ConeHandle(Quaternion rotation, Vector3 position, Vector2 angleAndRange, float angleScale, float rangeScale, bool handlesOnly) => 
            DoConeHandle(rotation, position, angleAndRange, angleScale, rangeScale, handlesOnly);

        /// <summary>
        /// <para>Draw a cone handle. Pass this into handle functions.</para>
        /// </summary>
        /// <param name="controlID">The control ID for the handle.</param>
        /// <param name="position">The position of the handle in the space of Handles.matrix.</param>
        /// <param name="rotation">The rotation of the handle in the space of Handles.matrix.</param>
        /// <param name="size">The size of the handle in the space of Handles.matrix. Use HandleUtility.GetHandleSize if you want a constant screen-space size.</param>
        /// <param name="eventType">Event type for the handle to act upon. By design it handles EventType.Layout and EventType.Repaint events.</param>
        public static void ConeHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            if (eventType != EventType.Layout)
            {
                if (eventType == EventType.Repaint)
                {
                    Graphics.DrawMeshNow(s_ConeMesh, StartCapDraw(position, rotation, size));
                }
            }
            else
            {
                HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(position, size));
            }
        }

        [Obsolete("Use CubeHandleCap instead")]
        public static void CubeCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Graphics.DrawMeshNow(s_CubeMesh, StartCapDraw(position, rotation, size));
            }
        }

        /// <summary>
        /// <para>Draw a cube handle. Pass this into handle functions.</para>
        /// </summary>
        /// <param name="controlID">The control ID for the handle.</param>
        /// <param name="position">The position of the handle in the space of Handles.matrix.</param>
        /// <param name="rotation">The rotation of the handle in the space of Handles.matrix.</param>
        /// <param name="size">The size of the handle in the space of Handles.matrix. Use HandleUtility.GetHandleSize if you want a constant screen-space size.</param>
        /// <param name="eventType">Event type for the handle to act upon. By design it handles EventType.Layout and EventType.Repaint events.</param>
        public static void CubeHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            if (eventType != EventType.Layout)
            {
                if (eventType == EventType.Repaint)
                {
                    Graphics.DrawMeshNow(s_CubeMesh, StartCapDraw(position, rotation, size));
                }
            }
            else
            {
                HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(position, size));
            }
        }

        [Obsolete("Use CylinderHandleCap instead")]
        public static void CylinderCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Graphics.DrawMeshNow(s_CylinderMesh, StartCapDraw(position, rotation, size));
            }
        }

        /// <summary>
        /// <para>Draw a cylinder handle. Pass this into handle functions.</para>
        /// </summary>
        /// <param name="controlID">The control ID for the handle.</param>
        /// <param name="position">The position of the handle in the space of Handles.matrix.</param>
        /// <param name="rotation">The rotation of the handle in the space of Handles.matrix.</param>
        /// <param name="size">The size of the handle in the space of Handles.matrix. Use HandleUtility.GetHandleSize if you want a constant screen-space size.</param>
        /// <param name="eventType">Event type for the handle to act upon. By design it handles EventType.Layout and EventType.Repaint events.</param>
        public static void CylinderHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            if (eventType != EventType.Layout)
            {
                if (eventType == EventType.Repaint)
                {
                    Graphics.DrawMeshNow(s_CylinderMesh, StartCapDraw(position, rotation, size));
                }
            }
            else
            {
                HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(position, size));
            }
        }

        /// <summary>
        /// <para>Make a 3D disc that can be dragged with the mouse.
        /// Note: Use HandleUtility.GetHandleSize where you might want to have constant screen-sized handles.</para>
        /// </summary>
        /// <param name="rotation">The rotation of the disc.</param>
        /// <param name="position">The center of the disc.</param>
        /// <param name="axis">The axis to rotate around.</param>
        /// <param name="size">The size of the disc in world space See Also:HandleUtility.GetHandleSize.</param>
        /// <param name="cutoffPlane">If true, only the front-facing half of the circle is draw / draggable. This is useful when you have many overlapping rotation axes (like in the default rotate tool) to avoid clutter.</param>
        /// <param name="snap">The grid size to snap to.</param>
        /// <returns>
        /// <para>The new rotation value modified by the user's interaction with the handle. If the user has not moved the handle, it will return the same value as you passed into the function.</para>
        /// </returns>
        public static Quaternion Disc(Quaternion rotation, Vector3 position, Vector3 axis, float size, bool cutoffPlane, float snap) => 
            UnityEditorInternal.Disc.Do(GUIUtility.GetControlID(s_DiscHash, FocusType.Keyboard), rotation, position, axis, size, cutoffPlane, snap);

        internal static float DistanceToPolygone(Vector3[] vertices) => 
            HandleUtility.DistanceToPolyLine(vertices);

        internal static void DoBoneHandle(Transform target)
        {
            DoBoneHandle(target, null);
        }

        internal static void DoBoneHandle(Transform target, Dictionary<Transform, bool> validBones)
        {
            Vector3 vector;
            int hashCode = target.name.GetHashCode();
            Event current = Event.current;
            bool flag = false;
            if (validBones != null)
            {
                IEnumerator enumerator = target.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Transform key = (Transform) enumerator.Current;
                        if (validBones.ContainsKey(key))
                        {
                            flag = true;
                            goto Label_0076;
                        }
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
            }
        Label_0076:
            vector = target.position;
            List<Vector3> list = new List<Vector3>();
            if (!flag && (target.parent != null))
            {
                list.Add(target.position + ((Vector3) ((target.position - target.parent.position) * 0.4f)));
            }
            else
            {
                IEnumerator enumerator2 = target.GetEnumerator();
                try
                {
                    while (enumerator2.MoveNext())
                    {
                        Transform transform2 = (Transform) enumerator2.Current;
                        if ((validBones == null) || validBones.ContainsKey(transform2))
                        {
                            list.Add(transform2.position);
                        }
                    }
                }
                finally
                {
                    IDisposable disposable2 = enumerator2 as IDisposable;
                    if (disposable2 != null)
                    {
                        disposable2.Dispose();
                    }
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                float num7;
                Vector3 endPoint = list[i];
                switch (current.GetTypeForControl(hashCode))
                {
                    case EventType.MouseDown:
                    {
                        if (!current.alt && (((HandleUtility.nearestControl == hashCode) && (current.button == 0)) || ((GUIUtility.keyboardControl == hashCode) && (current.button == 2))))
                        {
                            int num5 = hashCode;
                            GUIUtility.keyboardControl = num5;
                            GUIUtility.hotControl = num5;
                            if (current.shift)
                            {
                                UnityEngine.Object[] objects = Selection.objects;
                                if (!ArrayUtility.Contains<UnityEngine.Object>(objects, target))
                                {
                                    ArrayUtility.Add<UnityEngine.Object>(ref objects, target);
                                    Selection.objects = objects;
                                }
                            }
                            else
                            {
                                Selection.activeObject = target;
                            }
                            EditorGUIUtility.PingObject(target);
                            current.Use();
                        }
                        continue;
                    }
                    case EventType.MouseUp:
                    {
                        if ((GUIUtility.hotControl == hashCode) && ((current.button == 0) || (current.button == 2)))
                        {
                            GUIUtility.hotControl = 0;
                            current.Use();
                        }
                        continue;
                    }
                    case EventType.MouseMove:
                    case EventType.KeyDown:
                    case EventType.KeyUp:
                    case EventType.ScrollWheel:
                    {
                        continue;
                    }
                    case EventType.MouseDrag:
                    {
                        if (!current.alt && (GUIUtility.hotControl == hashCode))
                        {
                            DragAndDrop.PrepareStartDrag();
                            DragAndDrop.objectReferences = new UnityEngine.Object[] { target };
                            DragAndDrop.StartDrag(ObjectNames.GetDragAndDropTitle(target));
                            current.Use();
                        }
                        continue;
                    }
                    case EventType.Repaint:
                    {
                        float num6 = Vector3.Magnitude(endPoint - vector);
                        if (num6 > 0f)
                        {
                            num7 = num6 * 0.08f;
                            if (!flag)
                            {
                                break;
                            }
                            DrawBone(endPoint, vector, num7);
                        }
                        continue;
                    }
                    case EventType.Layout:
                    {
                        float radius = Vector3.Magnitude(endPoint - vector) * 0.08f;
                        Vector3[] vertices = GetBoneVertices(endPoint, vector, radius);
                        HandleUtility.AddControl(hashCode, DistanceToPolygone(vertices));
                        continue;
                    }
                    default:
                    {
                        continue;
                    }
                }
                SphereHandleCap(hashCode, vector, target.rotation, num7 * 0.2f, EventType.Repaint);
            }
        }

        internal static Vector3 DoConeFrustrumHandle(Quaternion rotation, Vector3 position, Vector3 radiusAngleRange)
        {
            Vector3 d = (Vector3) (rotation * Vector3.forward);
            Vector3 vector2 = (Vector3) (rotation * Vector3.up);
            Vector3 vector3 = (Vector3) (rotation * Vector3.right);
            float x = radiusAngleRange.x;
            float y = radiusAngleRange.y;
            float z = radiusAngleRange.z;
            y = Mathf.Max(0f, y);
            bool changed = GUI.changed;
            z = SizeSlider(position, d, z);
            GUI.changed |= changed;
            changed = GUI.changed;
            GUI.changed = false;
            x = SizeSlider(position, vector2, x);
            x = SizeSlider(position, -vector2, x);
            x = SizeSlider(position, vector3, x);
            x = SizeSlider(position, -vector3, x);
            if (GUI.changed)
            {
                x = Mathf.Max(0f, x);
            }
            GUI.changed |= changed;
            changed = GUI.changed;
            GUI.changed = false;
            float r = Mathf.Min((float) 1000f, (float) (Mathf.Abs((float) (z * Mathf.Tan(0.01745329f * y))) + x));
            r = SizeSlider(position + ((Vector3) (d * z)), vector2, r);
            r = SizeSlider(position + ((Vector3) (d * z)), -vector2, r);
            r = SizeSlider(position + ((Vector3) (d * z)), vector3, r);
            r = SizeSlider(position + ((Vector3) (d * z)), -vector3, r);
            if (GUI.changed)
            {
                y = Mathf.Clamp((float) (57.29578f * Mathf.Atan((r - x) / Mathf.Abs(z))), (float) 0f, (float) 90f);
            }
            GUI.changed |= changed;
            if (x > 0f)
            {
                DrawWireDisc(position, d, x);
            }
            if (r > 0f)
            {
                DrawWireDisc(position + ((Vector3) (z * d)), d, r);
            }
            DrawLine(position + ((Vector3) (vector2 * x)), (Vector3) ((position + (d * z)) + (vector2 * r)));
            DrawLine(position - ((Vector3) (vector2 * x)), (Vector3) ((position + (d * z)) - (vector2 * r)));
            DrawLine(position + ((Vector3) (vector3 * x)), (Vector3) ((position + (d * z)) + (vector3 * r)));
            DrawLine(position - ((Vector3) (vector3 * x)), (Vector3) ((position + (d * z)) - (vector3 * r)));
            return new Vector3(x, y, z);
        }

        internal static Vector2 DoConeHandle(Quaternion rotation, Vector3 position, Vector2 angleAndRange, float angleScale, float rangeScale, bool handlesOnly)
        {
            float x = angleAndRange.x;
            float y = angleAndRange.y;
            float r = y * rangeScale;
            Vector3 d = (Vector3) (rotation * Vector3.forward);
            Vector3 vector2 = (Vector3) (rotation * Vector3.up);
            Vector3 vector3 = (Vector3) (rotation * Vector3.right);
            bool changed = GUI.changed;
            GUI.changed = false;
            r = SizeSlider(position, d, r);
            if (GUI.changed)
            {
                y = Mathf.Max((float) 0f, (float) (r / rangeScale));
            }
            GUI.changed |= changed;
            changed = GUI.changed;
            GUI.changed = false;
            float num4 = (r * Mathf.Tan((0.01745329f * x) / 2f)) * angleScale;
            num4 = SizeSlider(position + ((Vector3) (d * r)), vector2, num4);
            num4 = SizeSlider(position + ((Vector3) (d * r)), -vector2, num4);
            num4 = SizeSlider(position + ((Vector3) (d * r)), vector3, num4);
            num4 = SizeSlider(position + ((Vector3) (d * r)), -vector3, num4);
            if (GUI.changed)
            {
                x = Mathf.Clamp((float) ((57.29578f * Mathf.Atan(num4 / (r * angleScale))) * 2f), (float) 0f, (float) 179f);
            }
            GUI.changed |= changed;
            if (!handlesOnly)
            {
                DrawLine(position, (Vector3) ((position + (d * r)) + (vector2 * num4)));
                DrawLine(position, (Vector3) ((position + (d * r)) - (vector2 * num4)));
                DrawLine(position, (Vector3) ((position + (d * r)) + (vector3 * num4)));
                DrawLine(position, (Vector3) ((position + (d * r)) - (vector3 * num4)));
                DrawWireDisc(position + ((Vector3) (r * d)), d, num4);
            }
            return new Vector2(x, y);
        }

        private static void DoDrawAAConvexPolygon(Vector3[] points, int actualNumberOfPoints, float alpha)
        {
            if (Event.current.type == EventType.Repaint)
            {
                HandleUtility.ApplyWireMaterial(zTest);
                Color defaultColor = new Color(1f, 1f, 1f, alpha) * color;
                Internal_DrawAAConvexPolygon(points, defaultColor, actualNumberOfPoints, matrix);
            }
        }

        private static void DoDrawAAPolyLine(Color[] colors, Vector3[] points, int actualNumberOfPoints, Texture2D lineTex, float width, float alpha)
        {
            if (Event.current.type == EventType.Repaint)
            {
                HandleUtility.ApplyWireMaterial(zTest);
                Color defaultColor = new Color(1f, 1f, 1f, alpha);
                if (colors != null)
                {
                    for (int i = 0; i < colors.Length; i++)
                    {
                        colors[i] *= defaultColor;
                    }
                }
                else
                {
                    defaultColor *= color;
                }
                Internal_DrawAAPolyLine(colors, points, defaultColor, actualNumberOfPoints, lineTex, width, matrix);
            }
        }

        private static Vector3 DoPlanarHandle(PlaneHandle planeID, Vector3 position, Quaternion rotation, float handleSize)
        {
            Vector3 normalized;
            int num = 0;
            int num2 = 0;
            int hint = 0;
            bool flag = (!Tools.s_Hidden && EditorApplication.isPlaying) && GameObjectUtility.ContainsStatic(Selection.gameObjects);
            if (planeID != PlaneHandle.xzPlane)
            {
                if (planeID == PlaneHandle.xyPlane)
                {
                    num = 0;
                    num2 = 1;
                    Handles.color = !flag ? zAxisColor : staticColor;
                    hint = s_xyAxisMoveHandleHash;
                }
                else if (planeID == PlaneHandle.yzPlane)
                {
                    num = 1;
                    num2 = 2;
                    Handles.color = !flag ? xAxisColor : staticColor;
                    hint = s_yzAxisMoveHandleHash;
                }
            }
            else
            {
                num = 0;
                num2 = 2;
                Handles.color = !flag ? yAxisColor : staticColor;
                hint = s_xzAxisMoveHandleHash;
            }
            int num4 = (3 - num2) - num;
            Color color = Handles.color;
            Matrix4x4 matrixx = Matrix4x4.TRS(position, rotation, Vector3.one);
            if (Camera.current.orthographic)
            {
                normalized = matrixx.inverse.MultiplyVector((Vector3) (SceneView.currentDrawingSceneView.cameraTargetRotation * -Vector3.forward)).normalized;
            }
            else
            {
                normalized = matrixx.inverse.MultiplyPoint(SceneView.currentDrawingSceneView.camera.transform.position).normalized;
            }
            int controlID = GUIUtility.GetControlID(hint, FocusType.Keyboard);
            if ((Mathf.Abs(normalized[num4]) < 0.05f) && (GUIUtility.hotControl != controlID))
            {
                Handles.color = color;
                return position;
            }
            if (!currentlyDragging)
            {
                s_PlanarHandlesOctant[num] = (normalized[num] >= -0.01f) ? ((float) 1) : ((float) (-1));
                s_PlanarHandlesOctant[num2] = (normalized[num2] >= -0.01f) ? ((float) 1) : ((float) (-1));
            }
            Vector3 offset = s_PlanarHandlesOctant;
            offset[num4] = 0f;
            offset = (Vector3) (rotation * ((offset * handleSize) * 0.5f));
            Vector3 zero = Vector3.zero;
            Vector3 vector7 = Vector3.zero;
            Vector3 handleDir = Vector3.zero;
            zero[num] = 1f;
            vector7[num2] = 1f;
            handleDir[num4] = 1f;
            zero = (Vector3) (rotation * zero);
            vector7 = (Vector3) (rotation * vector7);
            handleDir = (Vector3) (rotation * handleDir);
            verts[0] = (position + offset) + ((Vector3) (((zero + vector7) * handleSize) * 0.5f));
            verts[1] = (position + offset) + ((Vector3) (((-zero + vector7) * handleSize) * 0.5f));
            verts[2] = (position + offset) + ((Vector3) (((-zero - vector7) * handleSize) * 0.5f));
            verts[3] = (position + offset) + ((Vector3) (((zero - vector7) * handleSize) * 0.5f));
            DrawSolidRectangleWithOutline(verts, new Color(Handles.color.r, Handles.color.g, Handles.color.b, 0.1f), new Color(0f, 0f, 0f, 0f));
            if (<>f__mg$cache6 == null)
            {
                <>f__mg$cache6 = new CapFunction(Handles.RectangleHandleCap);
            }
            position = Slider2D(controlID, position, offset, handleDir, zero, vector7, handleSize * 0.5f, <>f__mg$cache6, new Vector2(SnapSettings.move[num], SnapSettings.move[num2]));
            Handles.color = color;
            return position;
        }

        public static Vector3 DoPositionHandle(Vector3 position, Quaternion rotation)
        {
            Event current = Event.current;
            EventType type = current.type;
            if (type != EventType.KeyDown)
            {
                if (type == EventType.KeyUp)
                {
                    position = DoPositionHandle_Internal(position, rotation);
                    if (((current.keyCode == KeyCode.V) && !current.shift) && !currentlyDragging)
                    {
                        s_FreeMoveMode = false;
                    }
                    return position;
                }
                if ((type == EventType.Layout) && (!currentlyDragging && !Tools.vertexDragging))
                {
                    s_FreeMoveMode = current.shift;
                }
            }
            else if ((current.keyCode == KeyCode.V) && !currentlyDragging)
            {
                s_FreeMoveMode = true;
            }
            return DoPositionHandle_Internal(position, rotation);
        }

        private static Vector3 DoPositionHandle_Internal(Vector3 position, Quaternion rotation)
        {
            float handleSize = HandleUtility.GetHandleSize(position);
            Color color = Handles.color;
            bool flag = (!Tools.s_Hidden && EditorApplication.isPlaying) && GameObjectUtility.ContainsStatic(Selection.gameObjects);
            Handles.color = !flag ? xAxisColor : Color.Lerp(xAxisColor, staticColor, staticBlend);
            GUI.SetNextControlName("xAxis");
            if (<>f__mg$cache2 == null)
            {
                <>f__mg$cache2 = new CapFunction(Handles.ArrowHandleCap);
            }
            position = Slider(position, (Vector3) (rotation * Vector3.right), handleSize, <>f__mg$cache2, SnapSettings.move.x);
            Handles.color = !flag ? yAxisColor : Color.Lerp(yAxisColor, staticColor, staticBlend);
            GUI.SetNextControlName("yAxis");
            if (<>f__mg$cache3 == null)
            {
                <>f__mg$cache3 = new CapFunction(Handles.ArrowHandleCap);
            }
            position = Slider(position, (Vector3) (rotation * Vector3.up), handleSize, <>f__mg$cache3, SnapSettings.move.y);
            Handles.color = !flag ? zAxisColor : Color.Lerp(zAxisColor, staticColor, staticBlend);
            GUI.SetNextControlName("zAxis");
            if (<>f__mg$cache4 == null)
            {
                <>f__mg$cache4 = new CapFunction(Handles.ArrowHandleCap);
            }
            position = Slider(position, (Vector3) (rotation * Vector3.forward), handleSize, <>f__mg$cache4, SnapSettings.move.z);
            if (s_FreeMoveMode)
            {
                Handles.color = centerColor;
                GUI.SetNextControlName("FreeMoveAxis");
                if (<>f__mg$cache5 == null)
                {
                    <>f__mg$cache5 = new CapFunction(Handles.RectangleHandleCap);
                }
                position = FreeMoveHandle(position, rotation, handleSize * 0.15f, SnapSettings.move, <>f__mg$cache5);
            }
            else
            {
                position = DoPlanarHandle(PlaneHandle.xzPlane, position, rotation, handleSize * 0.25f);
                position = DoPlanarHandle(PlaneHandle.xyPlane, position, rotation, handleSize * 0.25f);
                position = DoPlanarHandle(PlaneHandle.yzPlane, position, rotation, handleSize * 0.25f);
            }
            Handles.color = color;
            return position;
        }

        internal static float DoRadiusHandle(Quaternion rotation, Vector3 position, float radius, bool handlesOnly)
        {
            Vector3 forward;
            float num = 90f;
            Vector3[] vectorArray = new Vector3[] { rotation * Vector3.right, rotation * Vector3.up, rotation * Vector3.forward, rotation * -Vector3.right, rotation * -Vector3.up, rotation * -Vector3.forward };
            if (Camera.current.orthographic)
            {
                forward = Camera.current.transform.forward;
                if (!handlesOnly)
                {
                    DrawWireDisc(position, forward, radius);
                    for (int j = 0; j < 3; j++)
                    {
                        Vector3 normalized = Vector3.Cross(vectorArray[j], forward).normalized;
                        DrawTwoShadedWireDisc(position, vectorArray[j], normalized, 180f, radius);
                    }
                }
            }
            else
            {
                Matrix4x4 matrixx = Matrix4x4.Inverse(matrix);
                forward = position - matrixx.MultiplyPoint(Camera.current.transform.position);
                float sqrMagnitude = forward.sqrMagnitude;
                float num4 = radius * radius;
                float f = (num4 * num4) / sqrMagnitude;
                float num6 = f / num4;
                if (num6 < 1f)
                {
                    float y = Mathf.Sqrt(num4 - f);
                    num = Mathf.Atan2(y, Mathf.Sqrt(f)) * 57.29578f;
                    if (!handlesOnly)
                    {
                        DrawWireDisc(position - ((Vector3) ((num4 * forward) / sqrMagnitude)), forward, y);
                    }
                }
                else
                {
                    num = -1000f;
                }
                if (!handlesOnly)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        if (num6 < 1f)
                        {
                            float a = Vector3.Angle(forward, vectorArray[k]);
                            a = 90f - Mathf.Min(a, 180f - a);
                            float num10 = Mathf.Tan(a * 0.01745329f);
                            float num11 = Mathf.Sqrt(f + ((num10 * num10) * f)) / radius;
                            if (num11 < 1f)
                            {
                                float angle = Mathf.Asin(num11) * 57.29578f;
                                Vector3 from = Vector3.Cross(vectorArray[k], forward).normalized;
                                from = (Vector3) (Quaternion.AngleAxis(angle, vectorArray[k]) * from);
                                DrawTwoShadedWireDisc(position, vectorArray[k], from, (90f - angle) * 2f, radius);
                            }
                            else
                            {
                                DrawTwoShadedWireDisc(position, vectorArray[k], radius);
                            }
                        }
                        else
                        {
                            DrawTwoShadedWireDisc(position, vectorArray[k], radius);
                        }
                    }
                }
            }
            Color color = Handles.color;
            for (int i = 0; i < 6; i++)
            {
                int controlID = GUIUtility.GetControlID(s_RadiusHandleHash, FocusType.Keyboard);
                float num15 = Vector3.Angle(vectorArray[i], -forward);
                if (((num15 > 5f) && (num15 < 175f)) || (GUIUtility.hotControl == controlID))
                {
                    Color color2 = color;
                    if (num15 > (num + 5f))
                    {
                        color2.a = Mathf.Clamp01((backfaceAlphaMultiplier * color.a) * 2f);
                    }
                    else
                    {
                        color2.a = Mathf.Clamp01(color.a * 2f);
                    }
                    Handles.color = color2;
                    Vector3 vector6 = position + ((Vector3) (radius * vectorArray[i]));
                    bool changed = GUI.changed;
                    GUI.changed = false;
                    if (<>f__mg$cache7 == null)
                    {
                        <>f__mg$cache7 = new CapFunction(Handles.DotHandleCap);
                    }
                    vector6 = Slider1D.Do(controlID, vector6, vectorArray[i], HandleUtility.GetHandleSize(vector6) * 0.03f, <>f__mg$cache7, 0f);
                    if (GUI.changed)
                    {
                        radius = Vector3.Distance(vector6, position);
                    }
                    GUI.changed |= changed;
                }
            }
            Handles.color = color;
            return radius;
        }

        internal static Vector2 DoRectHandles(Quaternion rotation, Vector3 position, Vector2 size)
        {
            Vector3 vector = (Vector3) (rotation * Vector3.forward);
            Vector3 d = (Vector3) (rotation * Vector3.up);
            Vector3 vector3 = (Vector3) (rotation * Vector3.right);
            float r = 0.5f * size.x;
            float num2 = 0.5f * size.y;
            Vector3 vector4 = (Vector3) ((position + (d * num2)) + (vector3 * r));
            Vector3 vector5 = (Vector3) ((position - (d * num2)) + (vector3 * r));
            Vector3 vector6 = (Vector3) ((position - (d * num2)) - (vector3 * r));
            Vector3 vector7 = (Vector3) ((position + (d * num2)) - (vector3 * r));
            DrawLine(vector4, vector5);
            DrawLine(vector5, vector6);
            DrawLine(vector6, vector7);
            DrawLine(vector7, vector4);
            Color color = Handles.color;
            color.a = Mathf.Clamp01(color.a * 2f);
            Handles.color = color;
            num2 = SizeSlider(position, d, num2);
            num2 = SizeSlider(position, -d, num2);
            r = SizeSlider(position, vector3, r);
            r = SizeSlider(position, -vector3, r);
            if (((Tools.current != UnityEditor.Tool.Move) && (Tools.current != UnityEditor.Tool.Scale)) || (Tools.pivotRotation != PivotRotation.Local))
            {
                DrawLine(position, position + vector);
            }
            size.x = 2f * r;
            size.y = 2f * num2;
            return size;
        }

        public static Quaternion DoRotationHandle(Quaternion rotation, Vector3 position)
        {
            float handleSize = HandleUtility.GetHandleSize(position);
            Color color = Handles.color;
            bool flag = (!Tools.s_Hidden && EditorApplication.isPlaying) && GameObjectUtility.ContainsStatic(Selection.gameObjects);
            Handles.color = !flag ? xAxisColor : Color.Lerp(xAxisColor, staticColor, staticBlend);
            rotation = Disc(rotation, position, (Vector3) (rotation * Vector3.right), handleSize, true, SnapSettings.rotation);
            Handles.color = !flag ? yAxisColor : Color.Lerp(yAxisColor, staticColor, staticBlend);
            rotation = Disc(rotation, position, (Vector3) (rotation * Vector3.up), handleSize, true, SnapSettings.rotation);
            Handles.color = !flag ? zAxisColor : Color.Lerp(zAxisColor, staticColor, staticBlend);
            rotation = Disc(rotation, position, (Vector3) (rotation * Vector3.forward), handleSize, true, SnapSettings.rotation);
            if (!flag)
            {
                Handles.color = centerColor;
                rotation = Disc(rotation, position, Camera.current.transform.forward, handleSize * 1.1f, false, 0f);
                rotation = FreeRotateHandle(rotation, position, handleSize);
            }
            Handles.color = color;
            return rotation;
        }

        public static Vector3 DoScaleHandle(Vector3 scale, Vector3 position, Quaternion rotation, float size)
        {
            bool flag = (!Tools.s_Hidden && EditorApplication.isPlaying) && GameObjectUtility.ContainsStatic(Selection.gameObjects);
            color = !flag ? xAxisColor : Color.Lerp(xAxisColor, staticColor, staticBlend);
            scale.x = ScaleSlider(scale.x, position, (Vector3) (rotation * Vector3.right), rotation, size, SnapSettings.scale);
            color = !flag ? yAxisColor : Color.Lerp(yAxisColor, staticColor, staticBlend);
            scale.y = ScaleSlider(scale.y, position, (Vector3) (rotation * Vector3.up), rotation, size, SnapSettings.scale);
            color = !flag ? zAxisColor : Color.Lerp(zAxisColor, staticColor, staticBlend);
            scale.z = ScaleSlider(scale.z, position, (Vector3) (rotation * Vector3.forward), rotation, size, SnapSettings.scale);
            color = centerColor;
            EditorGUI.BeginChangeCheck();
            if (<>f__mg$cache8 == null)
            {
                <>f__mg$cache8 = new CapFunction(Handles.CubeHandleCap);
            }
            float num = ScaleValueHandle(scale.x, position, rotation, size, <>f__mg$cache8, SnapSettings.scale);
            if (EditorGUI.EndChangeCheck())
            {
                float num2 = num / scale.x;
                scale.x = num;
                scale.y *= num2;
                scale.z *= num2;
            }
            return scale;
        }

        internal static float DoSimpleEdgeHandle(Quaternion rotation, Vector3 position, float radius)
        {
            Vector3 d = (Vector3) (rotation * Vector3.right);
            EditorGUI.BeginChangeCheck();
            radius = SizeSlider(position, d, radius);
            radius = SizeSlider(position, -d, radius);
            if (EditorGUI.EndChangeCheck())
            {
                radius = Mathf.Max(0f, radius);
            }
            if (radius > 0f)
            {
                DrawLine(position - ((Vector3) (d * radius)), position + ((Vector3) (d * radius)));
            }
            return radius;
        }

        internal static void DoSimpleRadiusArcHandleXY(Quaternion rotation, Vector3 position, ref float radius, ref float arc)
        {
            Vector3 normal = (Vector3) (rotation * Vector3.forward);
            Vector3 d = (Vector3) (rotation * Vector3.up);
            Vector3 vector3 = (Vector3) (rotation * Vector3.right);
            Vector3 vector4 = (Vector3) (Quaternion.Euler(0f, 0f, arc) * vector3);
            EditorGUI.BeginChangeCheck();
            if (arc < 315f)
            {
                radius = SizeSlider(position, vector3, radius);
            }
            if (arc > 135f)
            {
                radius = SizeSlider(position, d, radius);
            }
            if (arc > 225f)
            {
                radius = SizeSlider(position, -vector3, radius);
            }
            if (arc > 315f)
            {
                radius = SizeSlider(position, -d, radius);
            }
            if (EditorGUI.EndChangeCheck())
            {
                radius = Mathf.Max(0f, radius);
            }
            if (radius > 0f)
            {
                DrawWireArc(position, normal, vector3, arc, radius);
                if (arc < 360f)
                {
                    DrawLine(position, vector3 * radius);
                    DrawLine(position, vector4 * radius);
                }
                else
                {
                    DrawDottedLine(position, vector3 * radius, 5f);
                }
                Vector3 vector5 = vector4 * radius;
                float handleSize = HandleUtility.GetHandleSize(vector5);
                EditorGUI.BeginChangeCheck();
                if (<>f__mg$cache9 == null)
                {
                    <>f__mg$cache9 = new CapFunction(Handles.CircleHandleCap);
                }
                Vector3 rhs = FreeMoveHandle(vector5, Quaternion.identity, handleSize * 0.03f, SnapSettings.move, <>f__mg$cache9);
                if (EditorGUI.EndChangeCheck())
                {
                    arc += Mathf.Atan2(Vector3.Dot(normal, Vector3.Cross(vector5, rhs)), Vector3.Dot(vector5, rhs)) * 57.29578f;
                }
            }
        }

        internal static float DoSimpleRadiusHandle(Quaternion rotation, Vector3 position, float radius, bool hemisphere)
        {
            Vector3 d = (Vector3) (rotation * Vector3.forward);
            Vector3 vector2 = (Vector3) (rotation * Vector3.up);
            Vector3 vector3 = (Vector3) (rotation * Vector3.right);
            bool changed = GUI.changed;
            GUI.changed = false;
            radius = SizeSlider(position, d, radius);
            if (!hemisphere)
            {
                radius = SizeSlider(position, -d, radius);
            }
            if (GUI.changed)
            {
                radius = Mathf.Max(0f, radius);
            }
            GUI.changed |= changed;
            changed = GUI.changed;
            GUI.changed = false;
            radius = SizeSlider(position, vector2, radius);
            radius = SizeSlider(position, -vector2, radius);
            radius = SizeSlider(position, vector3, radius);
            radius = SizeSlider(position, -vector3, radius);
            if (GUI.changed)
            {
                radius = Mathf.Max(0f, radius);
            }
            GUI.changed |= changed;
            if (radius > 0f)
            {
                DrawWireDisc(position, d, radius);
                DrawWireArc(position, vector2, -vector3, !hemisphere ? ((float) 360) : ((float) 180), radius);
                DrawWireArc(position, vector3, vector2, !hemisphere ? ((float) 360) : ((float) 180), radius);
            }
            return radius;
        }

        [Obsolete("Use DotHandleCap instead")]
        public static void DotCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if (Event.current.type == EventType.Repaint)
            {
                position = matrix.MultiplyPoint(position);
                Vector3 vector = (Vector3) (Camera.current.transform.right * size);
                Vector3 vector2 = (Vector3) (Camera.current.transform.up * size);
                Color c = color * new Color(1f, 1f, 1f, 0.99f);
                HandleUtility.ApplyWireMaterial(zTest);
                GL.Begin(7);
                GL.Color(c);
                GL.Vertex((position + vector) + vector2);
                GL.Vertex((position + vector) - vector2);
                GL.Vertex((position - vector) - vector2);
                GL.Vertex((position - vector) + vector2);
                GL.End();
            }
        }

        /// <summary>
        /// <para>Draw a dot handle. Pass this into handle functions.</para>
        /// </summary>
        /// <param name="controlID">The control ID for the handle.</param>
        /// <param name="position">The position of the handle in the space of Handles.matrix.</param>
        /// <param name="rotation">The rotation of the handle in the space of Handles.matrix.</param>
        /// <param name="size">The size of the handle in the space of Handles.matrix. Use HandleUtility.GetHandleSize if you want a constant screen-space size.</param>
        /// <param name="eventType">Event type for the handle to act upon. By design it handles EventType.Layout and EventType.Repaint events.</param>
        public static void DotHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            if (eventType != EventType.Layout)
            {
                if (eventType == EventType.Repaint)
                {
                    position = matrix.MultiplyPoint(position);
                    Vector3 vector = (Vector3) (Camera.current.transform.right * size);
                    Vector3 vector2 = (Vector3) (Camera.current.transform.up * size);
                    Color c = color * new Color(1f, 1f, 1f, 0.99f);
                    HandleUtility.ApplyWireMaterial();
                    GL.Begin(7);
                    GL.Color(c);
                    GL.Vertex((position + vector) + vector2);
                    GL.Vertex((position + vector) - vector2);
                    GL.Vertex((position - vector) - vector2);
                    GL.Vertex((position - vector) + vector2);
                    GL.End();
                }
            }
            else
            {
                HandleUtility.AddControl(controlID, HandleUtility.DistanceToRectangle(position, rotation, size));
            }
        }

        /// <summary>
        /// <para>Draw anti-aliased convex polygon specified with point array.</para>
        /// </summary>
        /// <param name="points">List of points describing the convex polygon.</param>
        public static void DrawAAConvexPolygon(params Vector3[] points)
        {
            DoDrawAAConvexPolygon(points, -1, 1f);
        }

        /// <summary>
        /// <para>Draw anti-aliased line specified with point array and width.</para>
        /// </summary>
        /// <param name="lineTex">The AA texture used for rendering. To get an anti-aliased effect use a texture that is 1x2 pixels with one transparent white pixel and one opaque white pixel.</param>
        /// <param name="width">The width of the line. Note: Use HandleUtility.GetHandleSize where you might want to have constant screen-sized handles.</param>
        /// <param name="points">List of points to build the line from.</param>
        /// <param name="actualNumberOfPoints"></param>
        public static void DrawAAPolyLine(params Vector3[] points)
        {
            DoDrawAAPolyLine(null, points, -1, null, 2f, 0.75f);
        }

        internal static void DrawAAPolyLine(Color[] colors, Vector3[] points)
        {
            DoDrawAAPolyLine(colors, points, -1, null, 2f, 0.75f);
        }

        /// <summary>
        /// <para>Draw anti-aliased line specified with point array and width.</para>
        /// </summary>
        /// <param name="lineTex">The AA texture used for rendering. To get an anti-aliased effect use a texture that is 1x2 pixels with one transparent white pixel and one opaque white pixel.</param>
        /// <param name="width">The width of the line. Note: Use HandleUtility.GetHandleSize where you might want to have constant screen-sized handles.</param>
        /// <param name="points">List of points to build the line from.</param>
        /// <param name="actualNumberOfPoints"></param>
        public static void DrawAAPolyLine(float width, params Vector3[] points)
        {
            DoDrawAAPolyLine(null, points, -1, null, width, 0.75f);
        }

        /// <summary>
        /// <para>Draw anti-aliased line specified with point array and width.</para>
        /// </summary>
        /// <param name="lineTex">The AA texture used for rendering. To get an anti-aliased effect use a texture that is 1x2 pixels with one transparent white pixel and one opaque white pixel.</param>
        /// <param name="width">The width of the line. Note: Use HandleUtility.GetHandleSize where you might want to have constant screen-sized handles.</param>
        /// <param name="points">List of points to build the line from.</param>
        /// <param name="actualNumberOfPoints"></param>
        public static void DrawAAPolyLine(Texture2D lineTex, params Vector3[] points)
        {
            DoDrawAAPolyLine(null, points, -1, lineTex, (float) (lineTex.height / 2), 0.99f);
        }

        internal static void DrawAAPolyLine(float width, Color[] colors, Vector3[] points)
        {
            DoDrawAAPolyLine(colors, points, -1, null, width, 0.75f);
        }

        /// <summary>
        /// <para>Draw anti-aliased line specified with point array and width.</para>
        /// </summary>
        /// <param name="lineTex">The AA texture used for rendering. To get an anti-aliased effect use a texture that is 1x2 pixels with one transparent white pixel and one opaque white pixel.</param>
        /// <param name="width">The width of the line. Note: Use HandleUtility.GetHandleSize where you might want to have constant screen-sized handles.</param>
        /// <param name="points">List of points to build the line from.</param>
        /// <param name="actualNumberOfPoints"></param>
        public static void DrawAAPolyLine(float width, int actualNumberOfPoints, params Vector3[] points)
        {
            DoDrawAAPolyLine(null, points, actualNumberOfPoints, null, width, 0.75f);
        }

        /// <summary>
        /// <para>Draw anti-aliased line specified with point array and width.</para>
        /// </summary>
        /// <param name="lineTex">The AA texture used for rendering. To get an anti-aliased effect use a texture that is 1x2 pixels with one transparent white pixel and one opaque white pixel.</param>
        /// <param name="width">The width of the line. Note: Use HandleUtility.GetHandleSize where you might want to have constant screen-sized handles.</param>
        /// <param name="points">List of points to build the line from.</param>
        /// <param name="actualNumberOfPoints"></param>
        public static void DrawAAPolyLine(Texture2D lineTex, float width, params Vector3[] points)
        {
            DoDrawAAPolyLine(null, points, -1, lineTex, width, 0.99f);
        }

        [Obsolete("DrawArrow has been renamed to ArrowCap.")]
        public static void DrawArrow(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            ArrowCap(controlID, position, rotation, size);
        }

        /// <summary>
        /// <para>Draw textured bezier line through start and end points with the given tangents.  To get an anti-aliased effect use a texture that is 1x2 pixels with one transparent white pixel and one opaque white pixel.  The bezier curve will be swept using this texture.</para>
        /// </summary>
        /// <param name="startPosition">The start point of the bezier line.</param>
        /// <param name="endPosition">The end point of the bezier line.</param>
        /// <param name="startTangent">The start tangent of the bezier line.</param>
        /// <param name="endTangent">The end tangent of the bezier line.</param>
        /// <param name="color">The color to use for the bezier line.</param>
        /// <param name="texture">The texture to use for drawing the bezier line.</param>
        /// <param name="width">The width of the bezier line.</param>
        public static void DrawBezier(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, Color color, Texture2D texture, float width)
        {
            if (Event.current.type == EventType.Repaint)
            {
                HandleUtility.ApplyWireMaterial(zTest);
                Internal_DrawBezier(startPosition, endPosition, startTangent, endTangent, color, texture, width, matrix);
            }
        }

        internal static void DrawBone(Vector3 endPoint, Vector3 basePoint, float size)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Vector3[] vectorArray = GetBoneVertices(endPoint, basePoint, size);
                HandleUtility.ApplyWireMaterial();
                GL.Begin(4);
                GL.Color(color);
                for (int i = 0; i < 3; i++)
                {
                    GL.Vertex(vectorArray[i * 6]);
                    GL.Vertex(vectorArray[(i * 6) + 1]);
                    GL.Vertex(vectorArray[(i * 6) + 2]);
                    GL.Vertex(vectorArray[(i * 6) + 3]);
                    GL.Vertex(vectorArray[(i * 6) + 4]);
                    GL.Vertex(vectorArray[(i * 6) + 5]);
                }
                GL.End();
                GL.Begin(1);
                GL.Color((color * new Color(1f, 1f, 1f, 0f)) + new Color(0f, 0f, 0f, 1f));
                for (int j = 0; j < 3; j++)
                {
                    GL.Vertex(vectorArray[j * 6]);
                    GL.Vertex(vectorArray[(j * 6) + 1]);
                    GL.Vertex(vectorArray[(j * 6) + 1]);
                    GL.Vertex(vectorArray[(j * 6) + 2]);
                }
                GL.End();
            }
        }

        /// <summary>
        /// <para>Draws a camera inside a rectangle.</para>
        /// </summary>
        /// <param name="position">The area to draw the camera within in GUI coordinates.</param>
        /// <param name="camera">The camera to draw.</param>
        /// <param name="drawMode">How the camera is drawn (textured, wireframe, etc.).</param>
        [ExcludeFromDocs]
        public static void DrawCamera(Rect position, Camera camera)
        {
            DrawCameraMode normal = DrawCameraMode.Normal;
            DrawCamera(position, camera, normal);
        }

        /// <summary>
        /// <para>Draws a camera inside a rectangle.</para>
        /// </summary>
        /// <param name="position">The area to draw the camera within in GUI coordinates.</param>
        /// <param name="camera">The camera to draw.</param>
        /// <param name="drawMode">How the camera is drawn (textured, wireframe, etc.).</param>
        public static void DrawCamera(Rect position, Camera camera, [DefaultValue("DrawCameraMode.Normal")] DrawCameraMode drawMode)
        {
            DrawGridParameters gridParam = new DrawGridParameters();
            DrawCameraImpl(position, camera, drawMode, false, gridParam, true);
        }

        internal static void DrawCamera(Rect position, Camera camera, DrawCameraMode drawMode, DrawGridParameters gridParam)
        {
            DrawCameraImpl(position, camera, drawMode, true, gridParam, true);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void DrawCameraFade(Camera camera, float fade);
        internal static void DrawCameraImpl(Rect position, Camera camera, DrawCameraMode drawMode, bool drawGrid, DrawGridParameters gridParam, bool finish)
        {
            if (Event.current.type == EventType.Repaint)
            {
                if (camera.targetTexture == null)
                {
                    Rect rect = EditorGUIUtility.PointsToPixels(GUIClip.Unclip(position));
                    camera.pixelRect = new Rect(rect.xMin, Screen.height - rect.yMax, rect.width, rect.height);
                }
                else
                {
                    camera.rect = new Rect(0f, 0f, 1f, 1f);
                }
                if (drawMode == DrawCameraMode.Normal)
                {
                    RenderTexture targetTexture = camera.targetTexture;
                    camera.targetTexture = RenderTexture.active;
                    camera.Render();
                    camera.targetTexture = targetTexture;
                }
                else
                {
                    if (drawGrid)
                    {
                        Internal_DrawCameraWithGrid(camera, (int) drawMode, ref gridParam);
                    }
                    else
                    {
                        Internal_DrawCamera(camera, (int) drawMode);
                    }
                    if (finish)
                    {
                        Internal_FinishDrawingCamera(camera);
                    }
                }
            }
            else
            {
                Internal_SetCurrentCamera(camera);
            }
        }

        internal static void DrawCameraStep1(Rect position, Camera camera, DrawCameraMode drawMode, DrawGridParameters gridParam)
        {
            DrawCameraImpl(position, camera, drawMode, true, gridParam, false);
        }

        internal static void DrawCameraStep2(Camera camera, DrawCameraMode drawMode)
        {
            if ((Event.current.type == EventType.Repaint) && (drawMode != DrawCameraMode.Normal))
            {
                Internal_FinishDrawingCamera(camera);
            }
        }

        [Obsolete("DrawCone has been renamed to ConeCap.")]
        public static void DrawCone(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            ConeCap(controlID, position, rotation, size);
        }

        [Obsolete("DrawCube has been renamed to CubeCap.")]
        public static void DrawCube(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            CubeCap(controlID, position, rotation, size);
        }

        [Obsolete("DrawCylinder has been renamed to CylinderCap.")]
        public static void DrawCylinder(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            CylinderCap(controlID, position, rotation, size);
        }

        /// <summary>
        /// <para>Draw a dotted line from p1 to p2.</para>
        /// </summary>
        /// <param name="p1">The start point.</param>
        /// <param name="p2">The end point.</param>
        /// <param name="screenSpaceSize">The size in pixels for the lengths of the line segments and the gaps between them.</param>
        public static void DrawDottedLine(Vector3 p1, Vector3 p2, float screenSpaceSize)
        {
            if (BeginLineDrawing(matrix, true, 1))
            {
                float x = screenSpaceSize * EditorGUIUtility.pixelsPerPoint;
                GL.MultiTexCoord(1, p1);
                GL.MultiTexCoord2(2, x, 0f);
                GL.Vertex(p1);
                GL.MultiTexCoord(1, p1);
                GL.MultiTexCoord2(2, x, 0f);
                GL.Vertex(p2);
                EndLineDrawing();
            }
        }

        /// <summary>
        /// <para>Draw a list of dotted line segments.</para>
        /// </summary>
        /// <param name="lineSegments">A list of pairs of points that represent the start and end of line segments.</param>
        /// <param name="screenSpaceSize">The size in pixels for the lengths of the line segments and the gaps between them.</param>
        public static void DrawDottedLines(Vector3[] lineSegments, float screenSpaceSize)
        {
            if (BeginLineDrawing(matrix, true, 1))
            {
                float x = screenSpaceSize * EditorGUIUtility.pixelsPerPoint;
                for (int i = 0; i < lineSegments.Length; i += 2)
                {
                    Vector3 v = lineSegments[i];
                    Vector3 vector2 = lineSegments[i + 1];
                    GL.MultiTexCoord(1, v);
                    GL.MultiTexCoord2(2, x, 0f);
                    GL.Vertex(v);
                    GL.MultiTexCoord(1, v);
                    GL.MultiTexCoord2(2, x, 0f);
                    GL.Vertex(vector2);
                }
                EndLineDrawing();
            }
        }

        /// <summary>
        /// <para>Draw a list of indexed dotted line segments.</para>
        /// </summary>
        /// <param name="points">A list of points.</param>
        /// <param name="segmentIndices">A list of pairs of indices to the start and end points of the line segments.</param>
        /// <param name="screenSpaceSize">The size in pixels for the lengths of the line segments and the gaps between them.</param>
        public static void DrawDottedLines(Vector3[] points, int[] segmentIndices, float screenSpaceSize)
        {
            if (BeginLineDrawing(matrix, true, 1))
            {
                float x = screenSpaceSize * EditorGUIUtility.pixelsPerPoint;
                for (int i = 0; i < segmentIndices.Length; i += 2)
                {
                    Vector3 v = points[segmentIndices[i]];
                    Vector3 vector2 = points[segmentIndices[i + 1]];
                    GL.MultiTexCoord(1, v);
                    GL.MultiTexCoord2(2, x, 0f);
                    GL.Vertex(v);
                    GL.MultiTexCoord(1, v);
                    GL.MultiTexCoord2(2, x, 0f);
                    GL.Vertex(vector2);
                }
                EndLineDrawing();
            }
        }

        /// <summary>
        /// <para>Draw a line from p1 to p2.</para>
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public static void DrawLine(Vector3 p1, Vector3 p2)
        {
            if (BeginLineDrawing(matrix, false, 1))
            {
                GL.Vertex(p1);
                GL.Vertex(p2);
                EndLineDrawing();
            }
        }

        /// <summary>
        /// <para>Draw a list of line segments.</para>
        /// </summary>
        /// <param name="lineSegments">A list of pairs of points that represent the start and end of line segments.</param>
        public static void DrawLines(Vector3[] lineSegments)
        {
            if (BeginLineDrawing(matrix, false, 1))
            {
                for (int i = 0; i < lineSegments.Length; i += 2)
                {
                    Vector3 v = lineSegments[i];
                    Vector3 vector2 = lineSegments[i + 1];
                    GL.Vertex(v);
                    GL.Vertex(vector2);
                }
                EndLineDrawing();
            }
        }

        /// <summary>
        /// <para>Draw a list of indexed line segments.</para>
        /// </summary>
        /// <param name="points">A list of points.</param>
        /// <param name="segmentIndices">A list of pairs of indices to the start and end points of the line segments.</param>
        public static void DrawLines(Vector3[] points, int[] segmentIndices)
        {
            if (BeginLineDrawing(matrix, false, 1))
            {
                for (int i = 0; i < segmentIndices.Length; i += 2)
                {
                    Vector3 v = points[segmentIndices[i]];
                    Vector3 vector2 = points[segmentIndices[i + 1]];
                    GL.Vertex(v);
                    GL.Vertex(vector2);
                }
                EndLineDrawing();
            }
        }

        /// <summary>
        /// <para>Draw a line going through the list of all points.</para>
        /// </summary>
        /// <param name="points"></param>
        public static void DrawPolyLine(params Vector3[] points)
        {
            if (BeginLineDrawing(matrix, false, 2))
            {
                for (int i = 0; i < points.Length; i++)
                {
                    GL.Vertex(points[i]);
                }
                EndLineDrawing();
            }
        }

        [Obsolete("DrawRectangle has been renamed to RectangleCap.")]
        public static void DrawRectangle(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            RectangleCap(controlID, position, rotation, size);
        }

        /// <summary>
        /// <para>Draw a camera facing selection frame.</para>
        /// </summary>
        /// <param name="controlID"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="size"></param>
        /// <param name="eventType"></param>
        public static void DrawSelectionFrame(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            if (eventType == EventType.Repaint)
            {
                StartCapDraw(position, rotation, size);
                Vector3 vector = (Vector3) (rotation * new Vector3(size, 0f, 0f));
                Vector3 vector2 = (Vector3) (rotation * new Vector3(0f, size, 0f));
                Vector3 vector3 = (position - vector) + vector2;
                Vector3 vector4 = (position + vector) + vector2;
                Vector3 vector5 = (position + vector) - vector2;
                Vector3 vector6 = (position - vector) - vector2;
                DrawLine(vector3, vector4);
                DrawLine(vector4, vector5);
                DrawLine(vector5, vector6);
                DrawLine(vector6, vector3);
            }
        }

        /// <summary>
        /// <para>Draw a circular sector (pie piece) in 3D space.</para>
        /// </summary>
        /// <param name="center">The center of the circle.</param>
        /// <param name="normal">The normal of the circle.</param>
        /// <param name="from">The direction of the point on the circumference, relative to the center, where the sector begins.</param>
        /// <param name="angle">The angle of the sector, in degrees.</param>
        /// <param name="radius">The radius of the circle
        /// 
        /// Note: Use HandleUtility.GetHandleSize where you might want to have constant screen-sized handles.</param>
        public static void DrawSolidArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Vector3[] dest = new Vector3[60];
                SetDiscSectionPoints(dest, center, normal, from, angle, radius);
                Shader.SetGlobalColor("_HandleColor", color * new Color(1f, 1f, 1f, 0.5f));
                Shader.SetGlobalFloat("_HandleSize", 1f);
                HandleUtility.ApplyWireMaterial(zTest);
                GL.PushMatrix();
                GL.MultMatrix(matrix);
                GL.Begin(4);
                for (int i = 1; i < dest.Length; i++)
                {
                    GL.Color(color);
                    GL.Vertex(center);
                    GL.Vertex(dest[i - 1]);
                    GL.Vertex(dest[i]);
                    GL.Vertex(center);
                    GL.Vertex(dest[i]);
                    GL.Vertex(dest[i - 1]);
                }
                GL.End();
                GL.PopMatrix();
            }
        }

        /// <summary>
        /// <para>Draw a solid flat disc in 3D space.</para>
        /// </summary>
        /// <param name="center">The center of the dics.</param>
        /// <param name="normal">The normal of the disc.</param>
        /// <param name="radius">The radius of the dics
        /// 
        /// Note: Use HandleUtility.GetHandleSize where you might want to have constant screen-sized handles.</param>
        public static void DrawSolidDisc(Vector3 center, Vector3 normal, float radius)
        {
            Vector3 from = Vector3.Cross(normal, Vector3.up);
            if (from.sqrMagnitude < 0.001f)
            {
                from = Vector3.Cross(normal, Vector3.right);
            }
            DrawSolidArc(center, normal, from, 360f, radius);
        }

        public static void DrawSolidRectangleWithOutline(Rect rectangle, Color faceColor, Color outlineColor)
        {
            Vector3[] verts = new Vector3[] { new Vector3(rectangle.xMin, rectangle.yMin, 0f), new Vector3(rectangle.xMax, rectangle.yMin, 0f), new Vector3(rectangle.xMax, rectangle.yMax, 0f), new Vector3(rectangle.xMin, rectangle.yMax, 0f) };
            DrawSolidRectangleWithOutline(verts, faceColor, outlineColor);
        }

        /// <summary>
        /// <para>Draw a solid outlined rectangle in 3D space.</para>
        /// </summary>
        /// <param name="verts">The 4 vertices of the rectangle in world coordinates.</param>
        /// <param name="faceColor">The color of the rectangle's face.</param>
        /// <param name="outlineColor">The outline color of the rectangle.</param>
        public static void DrawSolidRectangleWithOutline(Vector3[] verts, Color faceColor, Color outlineColor)
        {
            if (Event.current.type == EventType.Repaint)
            {
                HandleUtility.ApplyWireMaterial(zTest);
                GL.PushMatrix();
                GL.MultMatrix(matrix);
                if (faceColor.a > 0f)
                {
                    Color c = faceColor * color;
                    GL.Begin(4);
                    for (int i = 0; i < 2; i++)
                    {
                        GL.Color(c);
                        GL.Vertex(verts[i * 2]);
                        GL.Vertex(verts[(i * 2) + 1]);
                        GL.Vertex(verts[((i * 2) + 2) % 4]);
                        GL.Vertex(verts[i * 2]);
                        GL.Vertex(verts[((i * 2) + 2) % 4]);
                        GL.Vertex(verts[(i * 2) + 1]);
                    }
                    GL.End();
                }
                if (outlineColor.a > 0f)
                {
                    Color color2 = outlineColor * color;
                    GL.Begin(1);
                    GL.Color(color2);
                    for (int j = 0; j < 4; j++)
                    {
                        GL.Vertex(verts[j]);
                        GL.Vertex(verts[(j + 1) % 4]);
                    }
                    GL.End();
                }
                GL.PopMatrix();
            }
        }

        [Obsolete("DrawSphere has been renamed to SphereCap.")]
        public static void DrawSphere(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            SphereCap(controlID, position, rotation, size);
        }

        internal static void DrawTwoShadedWireDisc(Vector3 position, Vector3 axis, float radius)
        {
            Color color = Handles.color;
            Color color2 = color;
            color.a *= backfaceAlphaMultiplier;
            Handles.color = color;
            DrawWireDisc(position, axis, radius);
            Handles.color = color2;
        }

        internal static void DrawTwoShadedWireDisc(Vector3 position, Vector3 axis, Vector3 from, float degrees, float radius)
        {
            DrawWireArc(position, axis, from, degrees, radius);
            Color color = Handles.color;
            Color color2 = color;
            color.a *= backfaceAlphaMultiplier;
            Handles.color = color;
            DrawWireArc(position, axis, from, degrees - 360f, radius);
            Handles.color = color2;
        }

        /// <summary>
        /// <para>Draw a circular arc in 3D space.</para>
        /// </summary>
        /// <param name="center">The center of the circle.</param>
        /// <param name="normal">The normal of the circle.</param>
        /// <param name="from">The direction of the point on the circle circumference, relative to the center, where the arc begins.</param>
        /// <param name="angle">The angle of the arc, in degrees.</param>
        /// <param name="radius">The radius of the circle
        /// 
        /// Note: Use HandleUtility.GetHandleSize where you might want to have constant screen-sized handles.</param>
        public static void DrawWireArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
        {
            Vector3[] dest = new Vector3[60];
            SetDiscSectionPoints(dest, center, normal, from, angle, radius);
            DrawPolyLine(dest);
        }

        /// <summary>
        /// <para>Draw a wireframe box with center and size.</para>
        /// </summary>
        /// <param name="center"></param>
        /// <param name="size"></param>
        public static void DrawWireCube(Vector3 center, Vector3 size)
        {
            Vector3 vector = (Vector3) (size * 0.5f);
            Vector3[] points = new Vector3[] { center + new Vector3(-vector.x, -vector.y, -vector.z), center + new Vector3(-vector.x, vector.y, -vector.z), center + new Vector3(vector.x, vector.y, -vector.z), center + new Vector3(vector.x, -vector.y, -vector.z), center + new Vector3(-vector.x, -vector.y, -vector.z), center + new Vector3(-vector.x, -vector.y, vector.z), center + new Vector3(-vector.x, vector.y, vector.z), center + new Vector3(vector.x, vector.y, vector.z), center + new Vector3(vector.x, -vector.y, vector.z), center + new Vector3(-vector.x, -vector.y, vector.z) };
            DrawPolyLine(points);
            DrawLine(points[1], points[6]);
            DrawLine(points[2], points[7]);
            DrawLine(points[3], points[8]);
        }

        /// <summary>
        /// <para>Draw the outline of a flat disc in 3D space.</para>
        /// </summary>
        /// <param name="center">The center of the dics.</param>
        /// <param name="normal">The normal of the disc.</param>
        /// <param name="radius">The radius of the dics
        /// 
        /// Note: Use HandleUtility.GetHandleSize where you might want to have constant screen-sized handles.</param>
        public static void DrawWireDisc(Vector3 center, Vector3 normal, float radius)
        {
            Vector3 from = Vector3.Cross(normal, Vector3.up);
            if (from.sqrMagnitude < 0.001f)
            {
                from = Vector3.Cross(normal, Vector3.right);
            }
            DrawWireArc(center, normal, from, 360f, radius);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void EmitGUIGeometryForCamera(Camera source, Camera dest);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void EnableCameraFlares(Camera cam, bool flares);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void EnableCameraFx(Camera cam, bool fx);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void EnableCameraSkybox(Camera cam, bool skybox);
        /// <summary>
        /// <para>End a 2D GUI block and get back to the 3D handle GUI.</para>
        /// </summary>
        public static void EndGUI()
        {
            Camera current = Camera.current;
            if ((current != null) && (Event.current.type == EventType.Repaint))
            {
                Internal_SetupCamera(current);
            }
        }

        private static void EndLineDrawing()
        {
            GL.End();
            GL.PopMatrix();
        }

        public static Vector3 FreeMoveHandle(Vector3 position, Quaternion rotation, float size, Vector3 snap, CapFunction capFunction) => 
            FreeMove.Do(GUIUtility.GetControlID(s_FreeMoveHandleHash, FocusType.Keyboard), position, rotation, size, snap, capFunction);

        [Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
        public static Vector3 FreeMoveHandle(Vector3 position, Quaternion rotation, float size, Vector3 snap, DrawCapFunction capFunc) => 
            FreeMove.Do(GUIUtility.GetControlID(s_FreeMoveHandleHash, FocusType.Keyboard), position, rotation, size, snap, capFunc);

        /// <summary>
        /// <para>Make an unconstrained rotation handle.</para>
        /// </summary>
        /// <param name="rotation">Orientation of the handle.</param>
        /// <param name="position">Center of the handle in 3D space.</param>
        /// <param name="size">The size of the handle.
        /// 
        /// Note: Use HandleUtility.GetHandleSize where you might want to have constant screen-sized handles.</param>
        /// <returns>
        /// <para>The new rotation value modified by the user's interaction with the handle. If the user has not moved the handle, it will return the same value as you passed into the function.</para>
        /// </returns>
        public static Quaternion FreeRotateHandle(Quaternion rotation, Vector3 position, float size) => 
            FreeRotate.Do(GUIUtility.GetControlID(s_FreeRotateHandleHash, FocusType.Keyboard), rotation, position, size);

        internal static Vector3[] GetBoneVertices(Vector3 endPoint, Vector3 basePoint, float radius)
        {
            Vector3 lhs = Vector3.Normalize(endPoint - basePoint);
            Vector3 a = Vector3.Cross(lhs, Vector3.up);
            if (Vector3.SqrMagnitude(a) < 0.1f)
            {
                a = Vector3.Cross(lhs, Vector3.right);
            }
            a.Normalize();
            Vector3 vector3 = Vector3.Cross(lhs, a);
            Vector3[] vectorArray = new Vector3[0x12];
            float f = 0f;
            for (int i = 0; i < 3; i++)
            {
                float num3 = Mathf.Cos(f);
                float num4 = Mathf.Sin(f);
                float num5 = Mathf.Cos(f + 2.094395f);
                float num6 = Mathf.Sin(f + 2.094395f);
                Vector3 vector4 = (Vector3) ((basePoint + (a * (num3 * radius))) + (vector3 * (num4 * radius)));
                Vector3 vector5 = (Vector3) ((basePoint + (a * (num5 * radius))) + (vector3 * (num6 * radius)));
                vectorArray[i * 6] = endPoint;
                vectorArray[(i * 6) + 1] = vector4;
                vectorArray[(i * 6) + 2] = vector5;
                vectorArray[(i * 6) + 3] = basePoint;
                vectorArray[(i * 6) + 4] = vector5;
                vectorArray[(i * 6) + 5] = vector4;
                f += 2.094395f;
            }
            return vectorArray;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern FilterMode GetCameraFilterMode(Camera camera);
        internal static Rect GetCameraRect(Rect position)
        {
            Rect rect = GUIClip.Unclip(position);
            return new Rect(rect.xMin, Screen.height - rect.yMax, rect.width, rect.height);
        }

        /// <summary>
        /// <para>Get the width and height of the main game view.</para>
        /// </summary>
        public static Vector2 GetMainGameViewSize() => 
            GameView.GetMainGameViewTargetSize();

        internal static void Init()
        {
            if (s_CubeMesh == null)
            {
                GameObject obj2 = (GameObject) EditorGUIUtility.Load("SceneView/HandlesGO.fbx");
                if (obj2 == null)
                {
                    Debug.Log("ARGH - We couldn't find SceneView/HandlesGO.fbx");
                }
                obj2.SetActive(false);
                IEnumerator enumerator = obj2.transform.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Transform current = (Transform) enumerator.Current;
                        MeshFilter component = current.GetComponent<MeshFilter>();
                        string name = current.name;
                        if (name != null)
                        {
                            if (name != "Cube")
                            {
                                if (name == "Sphere")
                                {
                                    goto Label_00DE;
                                }
                                if (name == "Cone")
                                {
                                    goto Label_00EE;
                                }
                                if (name == "Cylinder")
                                {
                                    goto Label_00FE;
                                }
                                if (name == "Quad")
                                {
                                    goto Label_010E;
                                }
                            }
                            else
                            {
                                s_CubeMesh = component.sharedMesh;
                            }
                        }
                        continue;
                    Label_00DE:
                        s_SphereMesh = component.sharedMesh;
                        continue;
                    Label_00EE:
                        s_ConeMesh = component.sharedMesh;
                        continue;
                    Label_00FE:
                        s_CylinderMesh = component.sharedMesh;
                        continue;
                    Label_010E:
                        s_QuadMesh = component.sharedMesh;
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    ReplaceFontForWindows((Font) EditorGUIUtility.LoadRequired(EditorResourcesUtility.fontsPath + "Lucida Grande.ttf"));
                    ReplaceFontForWindows((Font) EditorGUIUtility.LoadRequired(EditorResourcesUtility.fontsPath + "Lucida Grande Bold.ttf"));
                    ReplaceFontForWindows((Font) EditorGUIUtility.LoadRequired(EditorResourcesUtility.fontsPath + "Lucida Grande Small.ttf"));
                    ReplaceFontForWindows((Font) EditorGUIUtility.LoadRequired(EditorResourcesUtility.fontsPath + "Lucida Grande Small Bold.ttf"));
                    ReplaceFontForWindows((Font) EditorGUIUtility.LoadRequired(EditorResourcesUtility.fontsPath + "Lucida Grande Big.ttf"));
                }
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_DrawAAConvexPolygon(Vector3[] points, ref Color defaultColor, int actualNumberOfPoints, ref Matrix4x4 toWorld);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_DrawAAPolyLine(Color[] colors, Vector3[] points, ref Color defaultColor, int actualNumberOfPoints, Texture2D texture, float width, ref Matrix4x4 toWorld);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_DrawBezier(ref Vector3 startPosition, ref Vector3 endPosition, ref Vector3 startTangent, ref Vector3 endTangent, ref Color color, Texture2D texture, float width, ref Matrix4x4 toWorld);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern Vector3[] INTERNAL_CALL_Internal_MakeBezierPoints(ref Vector3 startPosition, ref Vector3 endPosition, ref Vector3 startTangent, ref Vector3 endTangent, int division);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetDiscSectionPoints(Vector3[] dest, ref Vector3 center, ref Vector3 normal, ref Vector3 from, float angle, float radius);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetSceneViewColors(ref Color wire, ref Color wireOverlay, ref Color selectedOutline, ref Color selectedWire);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_ClearCamera(Camera cam);
        private static void Internal_DrawAAConvexPolygon(Vector3[] points, Color defaultColor, int actualNumberOfPoints, Matrix4x4 toWorld)
        {
            INTERNAL_CALL_Internal_DrawAAConvexPolygon(points, ref defaultColor, actualNumberOfPoints, ref toWorld);
        }

        private static void Internal_DrawAAPolyLine(Color[] colors, Vector3[] points, Color defaultColor, int actualNumberOfPoints, Texture2D texture, float width, Matrix4x4 toWorld)
        {
            INTERNAL_CALL_Internal_DrawAAPolyLine(colors, points, ref defaultColor, actualNumberOfPoints, texture, width, ref toWorld);
        }

        private static void Internal_DrawBezier(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, Color color, Texture2D texture, float width, Matrix4x4 toWorld)
        {
            INTERNAL_CALL_Internal_DrawBezier(ref startPosition, ref endPosition, ref startTangent, ref endTangent, ref color, texture, width, ref toWorld);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_DrawCamera(Camera cam, int renderMode);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_DrawCameraWithGrid(Camera cam, int renderMode, ref DrawGridParameters gridParam);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_FinishDrawingCamera(Camera cam);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_get_color(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_get_inverseMatrix(out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_get_matrix(out Matrix4x4 value);
        private static Vector3[] Internal_MakeBezierPoints(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, int division) => 
            INTERNAL_CALL_Internal_MakeBezierPoints(ref startPosition, ref endPosition, ref startTangent, ref endTangent, division);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_set_color(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_set_matrix(ref Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void Internal_SetCurrentCamera(Camera cam);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_SetupCamera(Camera cam);
        /// <summary>
        /// <para>Make a text label positioned in 3D space.</para>
        /// </summary>
        /// <param name="position">Position in 3D space as seen from the current handle camera.</param>
        /// <param name="text">Text to display on the label.</param>
        /// <param name="image">Texture to display on the label.</param>
        /// <param name="content">Text, image and tooltip for this label.</param>
        /// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.
        /// 
        /// Note: Use HandleUtility.GetHandleSize where you might want to have constant screen-sized handles.</param>
        public static void Label(Vector3 position, string text)
        {
            Label(position, EditorGUIUtility.TempContent(text), GUI.skin.label);
        }

        /// <summary>
        /// <para>Make a text label positioned in 3D space.</para>
        /// </summary>
        /// <param name="position">Position in 3D space as seen from the current handle camera.</param>
        /// <param name="text">Text to display on the label.</param>
        /// <param name="image">Texture to display on the label.</param>
        /// <param name="content">Text, image and tooltip for this label.</param>
        /// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.
        /// 
        /// Note: Use HandleUtility.GetHandleSize where you might want to have constant screen-sized handles.</param>
        public static void Label(Vector3 position, GUIContent content)
        {
            Label(position, content, GUI.skin.label);
        }

        /// <summary>
        /// <para>Make a text label positioned in 3D space.</para>
        /// </summary>
        /// <param name="position">Position in 3D space as seen from the current handle camera.</param>
        /// <param name="text">Text to display on the label.</param>
        /// <param name="image">Texture to display on the label.</param>
        /// <param name="content">Text, image and tooltip for this label.</param>
        /// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.
        /// 
        /// Note: Use HandleUtility.GetHandleSize where you might want to have constant screen-sized handles.</param>
        public static void Label(Vector3 position, Texture image)
        {
            Label(position, EditorGUIUtility.TempContent(image), GUI.skin.label);
        }

        /// <summary>
        /// <para>Make a text label positioned in 3D space.</para>
        /// </summary>
        /// <param name="position">Position in 3D space as seen from the current handle camera.</param>
        /// <param name="text">Text to display on the label.</param>
        /// <param name="image">Texture to display on the label.</param>
        /// <param name="content">Text, image and tooltip for this label.</param>
        /// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.
        /// 
        /// Note: Use HandleUtility.GetHandleSize where you might want to have constant screen-sized handles.</param>
        public static void Label(Vector3 position, string text, GUIStyle style)
        {
            Label(position, EditorGUIUtility.TempContent(text), style);
        }

        /// <summary>
        /// <para>Make a text label positioned in 3D space.</para>
        /// </summary>
        /// <param name="position">Position in 3D space as seen from the current handle camera.</param>
        /// <param name="text">Text to display on the label.</param>
        /// <param name="image">Texture to display on the label.</param>
        /// <param name="content">Text, image and tooltip for this label.</param>
        /// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.
        /// 
        /// Note: Use HandleUtility.GetHandleSize where you might want to have constant screen-sized handles.</param>
        public static void Label(Vector3 position, GUIContent content, GUIStyle style)
        {
            BeginGUI();
            GUI.Label(HandleUtility.WorldPointToSizedRect(position, content, style), content, style);
            EndGUI();
        }

        /// <summary>
        /// <para>Retuns an array of points to representing the bezier curve. See Handles.DrawBezier.</para>
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <param name="startTangent"></param>
        /// <param name="endTangent"></param>
        /// <param name="division"></param>
        public static Vector3[] MakeBezierPoints(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, int division)
        {
            if (division < 1)
            {
                throw new ArgumentOutOfRangeException("division", "Must be greater than zero");
            }
            return Internal_MakeBezierPoints(startPosition, endPosition, startTangent, endTangent, division);
        }

        /// <summary>
        /// <para>Make a position handle.</para>
        /// </summary>
        /// <param name="position">Center of the handle in 3D space.</param>
        /// <param name="rotation">Orientation of the handle in 3D space.</param>
        /// <returns>
        /// <para>The new value modified by the user's interaction with the handle. If the user has not moved the handle, it will return the same value as you passed into the function.</para>
        /// </returns>
        public static Vector3 PositionHandle(Vector3 position, Quaternion rotation) => 
            DoPositionHandle(position, rotation);

        /// <summary>
        /// <para>Make a Scene view radius handle.</para>
        /// </summary>
        /// <param name="rotation">Orientation of the handle.</param>
        /// <param name="position">Center of the handle in 3D space.</param>
        /// <param name="radius">Radius to modify.</param>
        /// <param name="handlesOnly">Whether to omit the circular outline of the radius and only draw the point handles.</param>
        /// <returns>
        /// <para>The new value modified by the user's interaction with the handle. If the user has not moved the handle, it will return the same value as you passed into the function.
        /// 
        /// Note: Use HandleUtility.GetHandleSize where you might want to have constant screen-sized handles.</para>
        /// </returns>
        public static float RadiusHandle(Quaternion rotation, Vector3 position, float radius) => 
            DoRadiusHandle(rotation, position, radius, false);

        /// <summary>
        /// <para>Make a Scene view radius handle.</para>
        /// </summary>
        /// <param name="rotation">Orientation of the handle.</param>
        /// <param name="position">Center of the handle in 3D space.</param>
        /// <param name="radius">Radius to modify.</param>
        /// <param name="handlesOnly">Whether to omit the circular outline of the radius and only draw the point handles.</param>
        /// <returns>
        /// <para>The new value modified by the user's interaction with the handle. If the user has not moved the handle, it will return the same value as you passed into the function.
        /// 
        /// Note: Use HandleUtility.GetHandleSize where you might want to have constant screen-sized handles.</para>
        /// </returns>
        public static float RadiusHandle(Quaternion rotation, Vector3 position, float radius, bool handlesOnly) => 
            DoRadiusHandle(rotation, position, radius, handlesOnly);

        [Obsolete("Use RectangleHandleCap instead")]
        public static void RectangleCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            RectangleCap(controlID, position, rotation, new Vector2(size, size));
        }

        internal static void RectangleCap(int controlID, Vector3 position, Quaternion rotation, Vector2 size)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Vector3 vector = (Vector3) (rotation * new Vector3(size.x, 0f, 0f));
                Vector3 vector2 = (Vector3) (rotation * new Vector3(0f, size.y, 0f));
                s_RectangleCapPointsCache[0] = (position + vector) + vector2;
                s_RectangleCapPointsCache[1] = (position + vector) - vector2;
                s_RectangleCapPointsCache[2] = (position - vector) - vector2;
                s_RectangleCapPointsCache[3] = (position - vector) + vector2;
                s_RectangleCapPointsCache[4] = (position + vector) + vector2;
                DrawPolyLine(s_RectangleCapPointsCache);
            }
        }

        /// <summary>
        /// <para>Draw a rectangle handle. Pass this into handle functions.</para>
        /// </summary>
        /// <param name="controlID">The control ID for the handle.</param>
        /// <param name="position">The position of the handle in the space of Handles.matrix.</param>
        /// <param name="rotation">The rotation of the handle in the space of Handles.matrix.</param>
        /// <param name="size">The size of the handle in the space of Handles.matrix. Use HandleUtility.GetHandleSize if you want a constant screen-space size.</param>
        /// <param name="eventType">Event type for the handle to act upon. By design it handles EventType.Layout and EventType.Repaint events.</param>
        public static void RectangleHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            RectangleHandleCap(controlID, position, rotation, new Vector2(size, size), eventType);
        }

        internal static void RectangleHandleCap(int controlID, Vector3 position, Quaternion rotation, Vector2 size, EventType eventType)
        {
            if (eventType != EventType.Layout)
            {
                if (eventType == EventType.Repaint)
                {
                    Vector3 vector = (Vector3) (rotation * new Vector3(size.x, 0f, 0f));
                    Vector3 vector2 = (Vector3) (rotation * new Vector3(0f, size.y, 0f));
                    s_RectangleHandlePointsCache[0] = (position + vector) + vector2;
                    s_RectangleHandlePointsCache[1] = (position + vector) - vector2;
                    s_RectangleHandlePointsCache[2] = (position - vector) - vector2;
                    s_RectangleHandlePointsCache[3] = (position - vector) + vector2;
                    s_RectangleHandlePointsCache[4] = (position + vector) + vector2;
                    DrawPolyLine(s_RectangleHandlePointsCache);
                }
            }
            else
            {
                HandleUtility.AddControl(controlID, HandleUtility.DistanceToRectangleInternal(position, rotation, size));
            }
        }

        private static void ReplaceFontForWindows(Font font)
        {
            if (font.name.Contains("Bold"))
            {
                font.fontNames = new string[] { "Verdana Bold", "Tahoma Bold" };
            }
            else
            {
                font.fontNames = new string[] { "Verdana", "Tahoma" };
            }
            font.hideFlags = HideFlags.HideAndDontSave;
        }

        /// <summary>
        /// <para>Make a Scene view rotation handle.</para>
        /// </summary>
        /// <param name="rotation">Orientation of the handle.</param>
        /// <param name="position">Center of the handle in 3D space.</param>
        /// <returns>
        /// <para>The new rotation value modified by the user's interaction with the handle. If the user has not moved the handle, it will return the same value as you passed into the function.</para>
        /// </returns>
        public static Quaternion RotationHandle(Quaternion rotation, Vector3 position) => 
            DoRotationHandle(rotation, position);

        /// <summary>
        /// <para>Make a Scene view scale handle.
        /// 
        /// Note: Use HandleUtility.GetHandleSize where you might want to have constant screen-sized handles.</para>
        /// </summary>
        /// <param name="scale">Scale to modify.</param>
        /// <param name="position">The position of the handle.</param>
        /// <param name="rotation">The rotation of the handle.</param>
        /// <param name="size">Allows you to scale the size of the handle on-scren.</param>
        /// <returns>
        /// <para>The new value modified by the user's interaction with the handle. If the user has not moved the handle, it will return the same value as you passed into the function.</para>
        /// </returns>
        public static Vector3 ScaleHandle(Vector3 scale, Vector3 position, Quaternion rotation, float size) => 
            DoScaleHandle(scale, position, rotation, size);

        /// <summary>
        /// <para>Make a directional scale slider.</para>
        /// </summary>
        /// <param name="scale">The value the user can modify.</param>
        /// <param name="position">The position of the handle in the space of Handles.matrix.</param>
        /// <param name="direction">The direction of the handle in the space of Handles.matrix.</param>
        /// <param name="rotation">The rotation of the handle in the space of Handles.matrix.</param>
        /// <param name="size">The size of the handle in the space of Handles.matrix. Use HandleUtility.GetHandleSize if you want a constant screen-space size.</param>
        /// <param name="snap">The snap increment. See Handles.SnapValue.</param>
        /// <returns>
        /// <para>The new value modified by the user's interaction with the handle. If the user has not moved the handle, it will return the same value as you passed into the function.</para>
        /// </returns>
        public static float ScaleSlider(float scale, Vector3 position, Vector3 direction, Quaternion rotation, float size, float snap) => 
            SliderScale.DoAxis(GUIUtility.GetControlID(s_ScaleSliderHash, FocusType.Keyboard), scale, position, direction, rotation, size, snap);

        public static float ScaleValueHandle(float value, Vector3 position, Quaternion rotation, float size, CapFunction capFunction, float snap) => 
            SliderScale.DoCenter(GUIUtility.GetControlID(s_ScaleValueHandleHash, FocusType.Keyboard), value, position, rotation, size, capFunction, snap);

        [Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
        public static float ScaleValueHandle(float value, Vector3 position, Quaternion rotation, float size, DrawCapFunction capFunc, float snap) => 
            SliderScale.DoCenter(GUIUtility.GetControlID(s_ScaleValueHandleHash, FocusType.Keyboard), value, position, rotation, size, capFunc, snap);

        public static void SelectionFrame(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if (Event.current.type == EventType.Repaint)
            {
                StartCapDraw(position, rotation, size);
                Vector3 vector = (Vector3) (rotation * new Vector3(size, 0f, 0f));
                Vector3 vector2 = (Vector3) (rotation * new Vector3(0f, size, 0f));
                Vector3 vector3 = (position - vector) + vector2;
                Vector3 vector4 = (position + vector) + vector2;
                Vector3 vector5 = (position + vector) - vector2;
                Vector3 vector6 = (position - vector) - vector2;
                DrawLine(vector3, vector4);
                DrawLine(vector4, vector5);
                DrawLine(vector5, vector6);
                DrawLine(vector6, vector3);
            }
        }

        /// <summary>
        /// <para>Set the current camera so all Handles and Gizmos are draw with its settings.</para>
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="position"></param>
        public static void SetCamera(Camera camera)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Internal_SetupCamera(camera);
            }
            else
            {
                Internal_SetCurrentCamera(camera);
            }
        }

        /// <summary>
        /// <para>Set the current camera so all Handles and Gizmos are draw with its settings.</para>
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="position"></param>
        public static void SetCamera(Rect position, Camera camera)
        {
            Rect rect = EditorGUIUtility.PointsToPixels(GUIClip.Unclip(position));
            Rect rect2 = new Rect(rect.xMin, Screen.height - rect.yMax, rect.width, rect.height);
            camera.pixelRect = rect2;
            if (Event.current.type == EventType.Repaint)
            {
                Internal_SetupCamera(camera);
            }
            else
            {
                Internal_SetCurrentCamera(camera);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void SetCameraFilterMode(Camera camera, FilterMode mode);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void SetCameraOnlyDrawMesh(Camera cam);
        internal static void SetDiscSectionPoints(Vector3[] dest, Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
        {
            INTERNAL_CALL_SetDiscSectionPoints(dest, ref center, ref normal, ref from, angle, radius);
        }

        internal static void SetSceneViewColors(Color wire, Color wireOverlay, Color selectedOutline, Color selectedWire)
        {
            INTERNAL_CALL_SetSceneViewColors(ref wire, ref wireOverlay, ref selectedOutline, ref selectedWire);
        }

        internal static void SetupIgnoreRaySnapObjects()
        {
            HandleUtility.ignoreRaySnapObjects = Selection.GetTransforms(UnityEditor.SelectionMode.Editable | UnityEditor.SelectionMode.Deep);
        }

        internal static void ShowStaticLabel(Vector3 pos)
        {
            color = Color.white;
            zTest = CompareFunction.Always;
            GUIStyle style = "SC ViewAxisLabel";
            style.alignment = TextAnchor.MiddleLeft;
            style.fixedWidth = 0f;
            BeginGUI();
            Rect position = HandleUtility.WorldPointToSizedRect(pos, EditorGUIUtility.TempContent("Static"), style);
            position.x += 10f;
            position.y += 10f;
            GUI.Label(position, EditorGUIUtility.TempContent("Static"), style);
            EndGUI();
        }

        internal static void ShowStaticLabelIfNeeded(Vector3 pos)
        {
            if ((!Tools.s_Hidden && EditorApplication.isPlaying) && GameObjectUtility.ContainsStatic(Selection.gameObjects))
            {
                ShowStaticLabel(pos);
            }
        }

        private static float SizeSlider(Vector3 p, Vector3 d, float r)
        {
            Vector3 position = p + ((Vector3) (d * r));
            float handleSize = HandleUtility.GetHandleSize(position);
            bool changed = GUI.changed;
            GUI.changed = false;
            if (<>f__mg$cache1 == null)
            {
                <>f__mg$cache1 = new CapFunction(Handles.DotHandleCap);
            }
            position = Slider(position, d, handleSize * 0.03f, <>f__mg$cache1, 0f);
            if (GUI.changed)
            {
                r = Vector3.Dot(position - p, d);
            }
            GUI.changed |= changed;
            return r;
        }

        /// <summary>
        /// <para>Make a 3D slider that moves along one axis.</para>
        /// </summary>
        /// <param name="position">The position of the current point in the space of Handles.matrix.</param>
        /// <param name="direction">The direction axis of the slider in the space of Handles.matrix.</param>
        /// <param name="size">The size of the handle in the space of Handles.matrix. Use HandleUtility.GetHandleSize if you want a constant screen-space size.</param>
        /// <param name="snap">The snap increment. See Handles.SnapValue.</param>
        /// <param name="capFunction">The function to call for doing the actual drawing. By default it is Handles.ArrowHandleCap, but any function that has the same signature can be used.</param>
        /// <returns>
        /// <para>The new value modified by the user's interaction with the handle. If the user has not moved the handle, it will return the position value passed into the function.</para>
        /// </returns>
        public static Vector3 Slider(Vector3 position, Vector3 direction)
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new CapFunction(Handles.ArrowHandleCap);
            }
            return Slider(position, direction, HandleUtility.GetHandleSize(position), <>f__mg$cache0, -1f);
        }

        public static Vector3 Slider(Vector3 position, Vector3 direction, float size, CapFunction capFunction, float snap) => 
            Slider1D.Do(GUIUtility.GetControlID(s_SliderHash, FocusType.Keyboard), position, direction, size, capFunction, snap);

        [Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
        public static Vector3 Slider(Vector3 position, Vector3 direction, float size, DrawCapFunction drawFunc, float snap) => 
            Slider1D.Do(GUIUtility.GetControlID(s_SliderHash, FocusType.Keyboard), position, direction, size, drawFunc, snap);

        [ExcludeFromDocs]
        public static Vector3 Slider2D(Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, CapFunction capFunction, float snap)
        {
            bool drawHelper = false;
            return Slider2D(handlePos, handleDir, slideDir1, slideDir2, handleSize, capFunction, snap, drawHelper);
        }

        [ExcludeFromDocs]
        public static Vector3 Slider2D(Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, CapFunction capFunction, Vector2 snap)
        {
            bool drawHelper = false;
            return Slider2D(handlePos, handleDir, slideDir1, slideDir2, handleSize, capFunction, snap, drawHelper);
        }

        [ExcludeFromDocs, Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
        public static Vector3 Slider2D(Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, DrawCapFunction drawFunc, float snap)
        {
            bool drawHelper = false;
            return Slider2D(handlePos, handleDir, slideDir1, slideDir2, handleSize, drawFunc, snap, drawHelper);
        }

        [ExcludeFromDocs, Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
        public static Vector3 Slider2D(Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, DrawCapFunction drawFunc, Vector2 snap)
        {
            bool drawHelper = false;
            return Slider2D(handlePos, handleDir, slideDir1, slideDir2, handleSize, drawFunc, snap, drawHelper);
        }

        [ExcludeFromDocs]
        public static Vector3 Slider2D(int id, Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, CapFunction capFunction, Vector2 snap)
        {
            bool drawHelper = false;
            return Slider2D(id, handlePos, handleDir, slideDir1, slideDir2, handleSize, capFunction, snap, drawHelper);
        }

        [Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap."), ExcludeFromDocs]
        public static Vector3 Slider2D(int id, Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, DrawCapFunction drawFunc, Vector2 snap)
        {
            bool drawHelper = false;
            return Slider2D(id, handlePos, handleDir, slideDir1, slideDir2, handleSize, drawFunc, snap, drawHelper);
        }

        public static Vector3 Slider2D(Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, CapFunction capFunction, float snap, [DefaultValue("false")] bool drawHelper) => 
            Slider2D(GUIUtility.GetControlID(s_Slider2DHash, FocusType.Keyboard), handlePos, new Vector3(0f, 0f, 0f), handleDir, slideDir1, slideDir2, handleSize, capFunction, new Vector2(snap, snap), drawHelper);

        public static Vector3 Slider2D(Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, CapFunction capFunction, Vector2 snap, [DefaultValue("false")] bool drawHelper) => 
            UnityEditorInternal.Slider2D.Do(GUIUtility.GetControlID(s_Slider2DHash, FocusType.Keyboard), handlePos, new Vector3(0f, 0f, 0f), handleDir, slideDir1, slideDir2, handleSize, capFunction, snap, drawHelper);

        [Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
        public static Vector3 Slider2D(Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, DrawCapFunction drawFunc, float snap, [DefaultValue("false")] bool drawHelper) => 
            Slider2D(GUIUtility.GetControlID(s_Slider2DHash, FocusType.Keyboard), handlePos, new Vector3(0f, 0f, 0f), handleDir, slideDir1, slideDir2, handleSize, drawFunc, new Vector2(snap, snap), drawHelper);

        [Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
        public static Vector3 Slider2D(Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, DrawCapFunction drawFunc, Vector2 snap, [DefaultValue("false")] bool drawHelper) => 
            UnityEditorInternal.Slider2D.Do(GUIUtility.GetControlID(s_Slider2DHash, FocusType.Keyboard), handlePos, new Vector3(0f, 0f, 0f), handleDir, slideDir1, slideDir2, handleSize, drawFunc, snap, drawHelper);

        public static Vector3 Slider2D(int id, Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, CapFunction capFunction, Vector2 snap, [DefaultValue("false")] bool drawHelper) => 
            UnityEditorInternal.Slider2D.Do(id, handlePos, new Vector3(0f, 0f, 0f), handleDir, slideDir1, slideDir2, handleSize, capFunction, snap, drawHelper);

        [Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
        public static Vector3 Slider2D(int id, Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, DrawCapFunction drawFunc, Vector2 snap, [DefaultValue("false")] bool drawHelper) => 
            UnityEditorInternal.Slider2D.Do(id, handlePos, new Vector3(0f, 0f, 0f), handleDir, slideDir1, slideDir2, handleSize, drawFunc, snap, drawHelper);

        [ExcludeFromDocs]
        public static Vector3 Slider2D(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, CapFunction capFunction, Vector2 snap)
        {
            bool drawHelper = false;
            return Slider2D(id, handlePos, offset, handleDir, slideDir1, slideDir2, handleSize, capFunction, snap, drawHelper);
        }

        [ExcludeFromDocs, Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
        public static Vector3 Slider2D(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, DrawCapFunction drawFunc, Vector2 snap)
        {
            bool drawHelper = false;
            return Slider2D(id, handlePos, offset, handleDir, slideDir1, slideDir2, handleSize, drawFunc, snap, drawHelper);
        }

        public static Vector3 Slider2D(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, CapFunction capFunction, Vector2 snap, [DefaultValue("false")] bool drawHelper) => 
            UnityEditorInternal.Slider2D.Do(id, handlePos, offset, handleDir, slideDir1, slideDir2, handleSize, capFunction, snap, drawHelper);

        [Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
        public static Vector3 Slider2D(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, DrawCapFunction drawFunc, Vector2 snap, [DefaultValue("false")] bool drawHelper) => 
            UnityEditorInternal.Slider2D.Do(id, handlePos, offset, handleDir, slideDir1, slideDir2, handleSize, drawFunc, snap, drawHelper);

        /// <summary>
        /// <para>Rounds the value val to the closest multiple of snap (snap can only be positive).</para>
        /// </summary>
        /// <param name="val"></param>
        /// <param name="snap"></param>
        /// <returns>
        /// <para>The rounded value, if snap is positive, and val otherwise.</para>
        /// </returns>
        public static float SnapValue(float val, float snap)
        {
            if (EditorGUI.actionKey && (snap > 0f))
            {
                return (Mathf.Round(val / snap) * snap);
            }
            return val;
        }

        [Obsolete("Use SphereHandleCap instead")]
        public static void SphereCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Graphics.DrawMeshNow(s_SphereMesh, StartCapDraw(position, rotation, size));
            }
        }

        /// <summary>
        /// <para>Draw a sphere handle. Pass this into handle functions.</para>
        /// </summary>
        /// <param name="controlID">The control ID for the handle.</param>
        /// <param name="position">The position of the handle in the space of Handles.matrix.</param>
        /// <param name="rotation">The rotation of the handle in the space of Handles.matrix.</param>
        /// <param name="eventType">Event type for the handle to act upon. By design it handles EventType.Layout and EventType.Repaint events.</param>
        /// <param name="size">The size of the handle in the space of Handles.matrix. Use HandleUtility.GetHandleSize if you want a constant screen-space size.</param>
        public static void SphereHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            if (eventType != EventType.Layout)
            {
                if (eventType == EventType.Repaint)
                {
                    Graphics.DrawMeshNow(s_SphereMesh, StartCapDraw(position, rotation, size));
                }
            }
            else
            {
                HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(position, size));
            }
        }

        internal static Matrix4x4 StartCapDraw(Vector3 position, Quaternion rotation, float size)
        {
            Shader.SetGlobalColor("_HandleColor", realHandleColor);
            Shader.SetGlobalFloat("_HandleSize", size);
            Matrix4x4 matrixx = matrix * Matrix4x4.TRS(position, rotation, Vector3.one);
            Shader.SetGlobalMatrix("_ObjectToWorld", matrixx);
            HandleUtility.handleMaterial.SetInt("_HandleZTest", (int) zTest);
            HandleUtility.handleMaterial.SetPass(0);
            return matrixx;
        }

        /// <summary>
        /// <para>Color to use for handles that represent the center of something.</para>
        /// </summary>
        public static Color centerColor =>
            ((Color) s_CenterColor);

        /// <summary>
        /// <para>Colors of the handles.</para>
        /// </summary>
        public static Color color
        {
            get
            {
                Color color;
                INTERNAL_get_color(out color);
                return color;
            }
            set
            {
                INTERNAL_set_color(ref value);
            }
        }

        /// <summary>
        /// <para>Setup viewport and stuff for a current camera.</para>
        /// </summary>
        public Camera currentCamera
        {
            get => 
                Camera.current;
            set
            {
                Internal_SetCurrentCamera(value);
            }
        }

        private static bool currentlyDragging =>
            (GUIUtility.hotControl != 0);

        /// <summary>
        /// <para>The inverse of the matrix for all handle operations.</para>
        /// </summary>
        public static Matrix4x4 inverseMatrix
        {
            get
            {
                Matrix4x4 matrixx;
                INTERNAL_get_inverseMatrix(out matrixx);
                return matrixx;
            }
        }

        /// <summary>
        /// <para>Are handles lit?</para>
        /// </summary>
        public static bool lighting { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Matrix for all handle operations.</para>
        /// </summary>
        public static Matrix4x4 matrix
        {
            get
            {
                Matrix4x4 matrixx;
                INTERNAL_get_matrix(out matrixx);
                return matrixx;
            }
            set
            {
                INTERNAL_set_matrix(ref value);
            }
        }

        internal static Color realHandleColor =>
            ((color * new Color(1f, 1f, 1f, 0.5f)) + (!lighting ? new Color(0f, 0f, 0f, 0f) : new Color(0f, 0f, 0f, 0.5f)));

        /// <summary>
        /// <para>Soft color to use for for general things.</para>
        /// </summary>
        public static Color secondaryColor =>
            ((Color) s_SecondaryColor);

        /// <summary>
        /// <para>Color to use for the currently active handle.</para>
        /// </summary>
        public static Color selectedColor =>
            ((Color) s_SelectedColor);

        /// <summary>
        /// <para>Color to use for handles that manipulates the X coordinate of something.</para>
        /// </summary>
        public static Color xAxisColor =>
            ((Color) s_XAxisColor);

        /// <summary>
        /// <para>Color to use for handles that manipulates the Y coordinate of something.</para>
        /// </summary>
        public static Color yAxisColor =>
            ((Color) s_YAxisColor);

        /// <summary>
        /// <para>Color to use for handles that manipulates the Z coordinate of something.</para>
        /// </summary>
        public static Color zAxisColor =>
            ((Color) s_ZAxisColor);

        /// <summary>
        /// <para>zTest of the handles.</para>
        /// </summary>
        public static CompareFunction zTest { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The function to use for drawing the handle e.g. Handles.RectangleCap.</para>
        /// </summary>
        /// <param name="controlID">The control ID for the handle.</param>
        /// <param name="position">The position of the handle in the space of Handles.matrix.</param>
        /// <param name="rotation">The rotation of the handle in the space of Handles.matrix.</param>
        /// <param name="size">The size of the handle in world-space units.</param>
        /// <param name="eventType">Event type for the handle to act upon. By design it handles EventType.Layout and EventType.Repaint events.</param>
        public delegate void CapFunction(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType);

        [Obsolete("This delegate is obsolete. Use CapFunction instead.")]
        public delegate void DrawCapFunction(int controlID, Vector3 position, Quaternion rotation, float size);

        /// <summary>
        /// <para>Disposable helper struct for automatically setting and reverting Handles.color and/or Handles.matrix.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct DrawingScope : IDisposable
        {
            private bool m_Disposed;
            private Color m_OriginalColor;
            private Matrix4x4 m_OriginalMatrix;
            /// <summary>
            /// <para>Create a new DrawingScope and set Handles.color and/or Handles.matrix to the specified values.</para>
            /// </summary>
            /// <param name="matrix">The matrix to use for displaying Handles inside the scope block.</param>
            /// <param name="color">The color to use for displaying Handles inside the scope block.</param>
            public DrawingScope(Color color) : this(color, Handles.matrix)
            {
            }

            /// <summary>
            /// <para>Create a new DrawingScope and set Handles.color and/or Handles.matrix to the specified values.</para>
            /// </summary>
            /// <param name="matrix">The matrix to use for displaying Handles inside the scope block.</param>
            /// <param name="color">The color to use for displaying Handles inside the scope block.</param>
            public DrawingScope(Matrix4x4 matrix) : this(Handles.color, matrix)
            {
            }

            /// <summary>
            /// <para>Create a new DrawingScope and set Handles.color and/or Handles.matrix to the specified values.</para>
            /// </summary>
            /// <param name="matrix">The matrix to use for displaying Handles inside the scope block.</param>
            /// <param name="color">The color to use for displaying Handles inside the scope block.</param>
            public DrawingScope(Color color, Matrix4x4 matrix)
            {
                this.m_Disposed = false;
                this.m_OriginalColor = Handles.color;
                this.m_OriginalMatrix = Handles.matrix;
                Handles.matrix = matrix;
                Handles.color = color;
            }

            /// <summary>
            /// <para>The value of Handles.color at the time this DrawingScope was created.</para>
            /// </summary>
            public Color originalColor =>
                this.m_OriginalColor;
            /// <summary>
            /// <para>The value of Handles.matrix at the time this DrawingScope was created.</para>
            /// </summary>
            public Matrix4x4 originalMatrix =>
                this.m_OriginalMatrix;
            /// <summary>
            /// <para>Automatically reverts Handles.color and Handles.matrix to their values prior to entering the scope, when the scope is exited. You do not need to call this method manually.</para>
            /// </summary>
            public void Dispose()
            {
                if (!this.m_Disposed)
                {
                    this.m_Disposed = true;
                    Handles.color = this.m_OriginalColor;
                    Handles.matrix = this.m_OriginalMatrix;
                }
            }
        }

        internal enum FilterMode
        {
            Off,
            ShowFiltered,
            ShowRest
        }

        private enum PlaneHandle
        {
            xzPlane,
            xyPlane,
            yzPlane
        }

        /// <summary>
        /// <para>A delegate type for getting a handle's size based on its current position.</para>
        /// </summary>
        /// <param name="position">The current position of the handle in the space of Handles.matrix.</param>
        public delegate float SizeFunction(Vector3 position);
    }
}

