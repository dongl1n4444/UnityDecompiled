namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal class Slider2D
    {
        [CompilerGenerated]
        private static Handles.DrawCapFunction <>f__mg$cache0;
        [CompilerGenerated]
        private static Handles.DrawCapFunction <>f__mg$cache1;
        private static Vector2 s_CurrentMousePosition;
        private static Vector2 s_StartPlaneOffset;
        private static Vector3 s_StartPosition;

        private static Vector2 CalcDeltaAlongDirections(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.CapFunction capFunction, Vector2 snap, bool drawHelper)
        {
            Vector3 position = handlePos + offset;
            Quaternion rotation = Quaternion.LookRotation(handleDir, slideDir1);
            Vector2 vector2 = new Vector2(0f, 0f);
            Event current = Event.current;
            switch (current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (((HandleUtility.nearestControl == id) && (current.button == 0)) || ((GUIUtility.keyboardControl == id) && (current.button == 2)))
                    {
                        if (GUIUtility.hotControl != 0)
                        {
                            return vector2;
                        }
                        bool success = true;
                        Vector3 vector3 = Handles.inverseMatrix.MultiplyPoint(GetMousePosition(handleDir, handlePos, ref success));
                        if (success)
                        {
                            int num = id;
                            GUIUtility.keyboardControl = num;
                            GUIUtility.hotControl = num;
                            s_CurrentMousePosition = current.mousePosition;
                            s_StartPosition = handlePos;
                            Vector3 lhs = vector3 - handlePos;
                            s_StartPlaneOffset.x = Vector3.Dot(lhs, slideDir1);
                            s_StartPlaneOffset.y = Vector3.Dot(lhs, slideDir2);
                            current.Use();
                            EditorGUIUtility.SetWantsMouseJumping(1);
                        }
                    }
                    return vector2;

                case EventType.MouseUp:
                    if ((GUIUtility.hotControl == id) && ((current.button == 0) || (current.button == 2)))
                    {
                        GUIUtility.hotControl = 0;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(0);
                    }
                    return vector2;

                case EventType.MouseMove:
                case EventType.KeyDown:
                case EventType.KeyUp:
                case EventType.ScrollWheel:
                    return vector2;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == id)
                    {
                        s_CurrentMousePosition += current.delta;
                        bool flag2 = true;
                        Vector3 point = Handles.inverseMatrix.MultiplyPoint(GetMousePosition(handleDir, handlePos, ref flag2));
                        if (flag2)
                        {
                            vector2.x = HandleUtility.PointOnLineParameter(point, s_StartPosition, slideDir1);
                            vector2.y = HandleUtility.PointOnLineParameter(point, s_StartPosition, slideDir2);
                            vector2 -= s_StartPlaneOffset;
                            if ((snap.x > 0f) || (snap.y > 0f))
                            {
                                vector2.x = Handles.SnapValue(vector2.x, snap.x);
                                vector2.y = Handles.SnapValue(vector2.y, snap.y);
                            }
                            GUI.changed = true;
                        }
                        current.Use();
                    }
                    return vector2;

                case EventType.Repaint:
                    if (capFunction != null)
                    {
                        Color white = Color.white;
                        if (id == GUIUtility.keyboardControl)
                        {
                            white = Handles.color;
                            Handles.color = Handles.selectedColor;
                        }
                        capFunction(id, position, rotation, handleSize, EventType.Repaint);
                        if (id == GUIUtility.keyboardControl)
                        {
                            Handles.color = white;
                        }
                        if (drawHelper && (GUIUtility.hotControl == id))
                        {
                            Vector3[] verts = new Vector3[4];
                            float num2 = handleSize * 10f;
                            verts[0] = position + ((Vector3) ((slideDir1 * num2) + (slideDir2 * num2)));
                            verts[1] = verts[0] - ((Vector3) ((slideDir1 * num2) * 2f));
                            verts[2] = verts[1] - ((Vector3) ((slideDir2 * num2) * 2f));
                            verts[3] = verts[2] + ((Vector3) ((slideDir1 * num2) * 2f));
                            Color color = Handles.color;
                            Handles.color = Color.white;
                            float r = 0.6f;
                            Handles.DrawSolidRectangleWithOutline(verts, new Color(1f, 1f, 1f, 0.05f), new Color(r, r, r, 0.4f));
                            Handles.color = color;
                        }
                        return vector2;
                    }
                    return vector2;

                case EventType.Layout:
                    if (capFunction == null)
                    {
                        HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(handlePos + offset, handleSize * 0.5f));
                        return vector2;
                    }
                    capFunction(id, position, rotation, handleSize, EventType.Layout);
                    return vector2;
            }
            return vector2;
        }

        private static Vector2 CalcDeltaAlongDirections(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.DrawCapFunction drawFunc, Vector2 snap, bool drawHelper)
        {
            Vector2 vector = new Vector2(0f, 0f);
            Event current = Event.current;
            switch (current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if ((((HandleUtility.nearestControl == id) && (current.button == 0)) || ((GUIUtility.keyboardControl == id) && (current.button == 2))) && (GUIUtility.hotControl == 0))
                    {
                        Plane plane = new Plane(Handles.matrix.MultiplyVector(handleDir), Handles.matrix.MultiplyPoint(handlePos));
                        Ray ray = HandleUtility.GUIPointToWorldRay(current.mousePosition);
                        float enter = 0f;
                        plane.Raycast(ray, out enter);
                        int num2 = id;
                        GUIUtility.keyboardControl = num2;
                        GUIUtility.hotControl = num2;
                        s_CurrentMousePosition = current.mousePosition;
                        s_StartPosition = handlePos;
                        Vector3 lhs = Handles.inverseMatrix.MultiplyPoint(ray.GetPoint(enter)) - handlePos;
                        s_StartPlaneOffset.x = Vector3.Dot(lhs, slideDir1);
                        s_StartPlaneOffset.y = Vector3.Dot(lhs, slideDir2);
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }
                    return vector;

                case EventType.MouseUp:
                    if ((GUIUtility.hotControl == id) && ((current.button == 0) || (current.button == 2)))
                    {
                        GUIUtility.hotControl = 0;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(0);
                    }
                    return vector;

                case EventType.MouseMove:
                case EventType.KeyDown:
                case EventType.KeyUp:
                case EventType.ScrollWheel:
                    return vector;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == id)
                    {
                        s_CurrentMousePosition += current.delta;
                        Vector3 a = Handles.matrix.MultiplyPoint(handlePos);
                        Vector3 normalized = Handles.matrix.MultiplyVector(slideDir1).normalized;
                        Vector3 vector7 = Handles.matrix.MultiplyVector(slideDir2).normalized;
                        Ray ray2 = HandleUtility.GUIPointToWorldRay(s_CurrentMousePosition);
                        Plane plane2 = new Plane(a, a + normalized, a + vector7);
                        float num3 = 0f;
                        if (plane2.Raycast(ray2, out num3))
                        {
                            Vector3 point = Handles.inverseMatrix.MultiplyPoint(ray2.GetPoint(num3));
                            vector.x = HandleUtility.PointOnLineParameter(point, s_StartPosition, slideDir1);
                            vector.y = HandleUtility.PointOnLineParameter(point, s_StartPosition, slideDir2);
                            vector -= s_StartPlaneOffset;
                            if ((snap.x > 0f) || (snap.y > 0f))
                            {
                                vector.x = Handles.SnapValue(vector.x, snap.x);
                                vector.y = Handles.SnapValue(vector.y, snap.y);
                            }
                            GUI.changed = true;
                        }
                        current.Use();
                    }
                    return vector;

                case EventType.Repaint:
                    if (drawFunc != null)
                    {
                        Vector3 position = handlePos + offset;
                        Quaternion rotation = Quaternion.LookRotation(handleDir, slideDir1);
                        Color white = Color.white;
                        if (id == GUIUtility.keyboardControl)
                        {
                            white = Handles.color;
                            Handles.color = Handles.selectedColor;
                        }
                        drawFunc(id, position, rotation, handleSize);
                        if (id == GUIUtility.keyboardControl)
                        {
                            Handles.color = white;
                        }
                        if (drawHelper && (GUIUtility.hotControl == id))
                        {
                            Vector3[] verts = new Vector3[4];
                            float num4 = handleSize * 10f;
                            verts[0] = position + ((Vector3) ((slideDir1 * num4) + (slideDir2 * num4)));
                            verts[1] = verts[0] - ((Vector3) ((slideDir1 * num4) * 2f));
                            verts[2] = verts[1] - ((Vector3) ((slideDir2 * num4) * 2f));
                            verts[3] = verts[2] + ((Vector3) ((slideDir1 * num4) * 2f));
                            Color color = Handles.color;
                            Handles.color = Color.white;
                            float r = 0.6f;
                            Handles.DrawSolidRectangleWithOutline(verts, new Color(1f, 1f, 1f, 0.05f), new Color(r, r, r, 0.4f));
                            Handles.color = color;
                        }
                        return vector;
                    }
                    return vector;

                case EventType.Layout:
                    if (<>f__mg$cache0 == null)
                    {
                        <>f__mg$cache0 = new Handles.DrawCapFunction(Handles.ArrowCap);
                    }
                    if (drawFunc == <>f__mg$cache0)
                    {
                        HandleUtility.AddControl(id, HandleUtility.DistanceToLine(handlePos + offset, handlePos + ((Vector3) (handleDir * handleSize))));
                        HandleUtility.AddControl(id, HandleUtility.DistanceToCircle((handlePos + offset) + ((Vector3) (handleDir * handleSize)), handleSize * 0.2f));
                        return vector;
                    }
                    if (<>f__mg$cache1 == null)
                    {
                        <>f__mg$cache1 = new Handles.DrawCapFunction(Handles.RectangleCap);
                    }
                    if (drawFunc == <>f__mg$cache1)
                    {
                        HandleUtility.AddControl(id, HandleUtility.DistanceToRectangle(handlePos + offset, Quaternion.LookRotation(handleDir, slideDir1), handleSize));
                        return vector;
                    }
                    HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(handlePos + offset, handleSize * 0.5f));
                    return vector;
            }
            return vector;
        }

        public static Vector3 Do(int id, Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.CapFunction capFunction, float snap, bool drawHelper) => 
            Do(id, handlePos, new Vector3(0f, 0f, 0f), handleDir, slideDir1, slideDir2, handleSize, capFunction, new Vector2(snap, snap), drawHelper);

        public static Vector3 Do(int id, Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.DrawCapFunction drawFunc, float snap, bool drawHelper) => 
            Do(id, handlePos, new Vector3(0f, 0f, 0f), handleDir, slideDir1, slideDir2, handleSize, drawFunc, new Vector2(snap, snap), drawHelper);

        public static Vector3 Do(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.CapFunction capFunction, float snap, bool drawHelper) => 
            Do(id, handlePos, offset, handleDir, slideDir1, slideDir2, handleSize, capFunction, new Vector2(snap, snap), drawHelper);

        public static Vector3 Do(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.CapFunction capFunction, Vector2 snap, bool drawHelper)
        {
            bool changed = GUI.changed;
            GUI.changed = false;
            Vector2 vector = CalcDeltaAlongDirections(id, handlePos, offset, handleDir, slideDir1, slideDir2, handleSize, capFunction, snap, drawHelper);
            if (GUI.changed)
            {
                handlePos = (Vector3) ((s_StartPosition + (slideDir1 * vector.x)) + (slideDir2 * vector.y));
            }
            GUI.changed |= changed;
            return handlePos;
        }

        public static Vector3 Do(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.DrawCapFunction drawFunc, float snap, bool drawHelper) => 
            Do(id, handlePos, offset, handleDir, slideDir1, slideDir2, handleSize, drawFunc, new Vector2(snap, snap), drawHelper);

        public static Vector3 Do(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.DrawCapFunction drawFunc, Vector2 snap, bool drawHelper)
        {
            bool changed = GUI.changed;
            GUI.changed = false;
            Vector2 vector = CalcDeltaAlongDirections(id, handlePos, offset, handleDir, slideDir1, slideDir2, handleSize, drawFunc, snap, drawHelper);
            if (GUI.changed)
            {
                handlePos = (Vector3) ((s_StartPosition + (slideDir1 * vector.x)) + (slideDir2 * vector.y));
            }
            GUI.changed |= changed;
            return handlePos;
        }

        private static Vector3 GetMousePosition(Vector3 handleDirection, Vector3 handlePosition, ref bool success)
        {
            if (Camera.current != null)
            {
                Plane plane = new Plane(Handles.matrix.MultiplyVector(handleDirection), Handles.matrix.MultiplyPoint(handlePosition));
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                float enter = 0f;
                success = plane.Raycast(ray, out enter);
                return ray.GetPoint(enter);
            }
            success = true;
            return (Vector3) Event.current.mousePosition;
        }
    }
}

