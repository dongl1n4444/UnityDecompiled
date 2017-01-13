namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class PointEditor
    {
        [CompilerGenerated]
        private static Func<KeyValuePair<int, float>, float> <>f__am$cache0;
        private static bool s_DidDrag;
        private static List<int> s_SelectionStart;
        private static List<int> s_StartDragSelection;
        private static Vector2 s_StartMouseDragPosition;

        public static void Draw(IEditablePoint points, Transform cloudTransform, List<int> selection, bool twoPassDrawing)
        {
            LightmapVisualization.DrawPointCloud(points.GetUnselectedPositions(), points.GetSelectedPositions(), points.GetDefaultColor(), points.GetSelectedColor(), points.GetPointScale(), cloudTransform);
        }

        public static int FindNearest(Vector2 point, Transform cloudTransform, IEditablePoint points)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(point);
            Dictionary<int, float> dictionary = new Dictionary<int, float>();
            for (int i = 0; i < points.Count; i++)
            {
                float t = 0f;
                Vector3 zero = Vector3.zero;
                if (MathUtils.IntersectRaySphere(ray, cloudTransform.TransformPoint(points.GetPosition(i)), points.GetPointScale() * 0.5f, ref t, ref zero) && (t > 0f))
                {
                    dictionary.Add(i, t);
                }
            }
            if (dictionary.Count <= 0)
            {
                return -1;
            }
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = x => x.Value;
            }
            return Enumerable.OrderBy<KeyValuePair<int, float>, float>(dictionary, <>f__am$cache0).First<KeyValuePair<int, float>>().Key;
        }

        private static Rect FromToRect(Vector2 from, Vector2 to)
        {
            Rect rect = new Rect(from.x, from.y, to.x - from.x, to.y - from.y);
            if (rect.width < 0f)
            {
                rect.x += rect.width;
                rect.width = -rect.width;
            }
            if (rect.height < 0f)
            {
                rect.y += rect.height;
                rect.height = -rect.height;
            }
            return rect;
        }

        public static bool MovePoints(IEditablePoint points, Transform cloudTransform, List<int> selection)
        {
            <MovePoints>c__AnonStorey0 storey = new <MovePoints>c__AnonStorey0 {
                points = points
            };
            if ((selection.Count != 0) && (Camera.current != null))
            {
                Vector3 zero = Vector3.zero;
                zero = (Tools.pivotMode != PivotMode.Pivot) ? ((Vector3) (Enumerable.Aggregate<int, Vector3>(selection, zero, new Func<Vector3, int, Vector3>(storey.<>m__0)) / ((float) selection.Count))) : storey.points.GetPosition(selection[0]);
                zero = cloudTransform.TransformPoint(zero);
                Vector3 position = Handles.PositionHandle(zero, (Tools.pivotRotation != PivotRotation.Local) ? Quaternion.identity : cloudTransform.rotation);
                if (GUI.changed)
                {
                    Vector3 vector3 = cloudTransform.InverseTransformPoint(position) - cloudTransform.InverseTransformPoint(zero);
                    foreach (int num in selection)
                    {
                        storey.points.SetPosition(num, storey.points.GetPosition(num) + vector3);
                    }
                    return true;
                }
            }
            return false;
        }

        public static bool SelectPoints(IEditablePoint points, Transform cloudTransform, ref List<int> selection, bool firstSelect)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            if (Event.current.alt && (Event.current.type != EventType.Repaint))
            {
                return false;
            }
            bool flag2 = false;
            Event current = Event.current;
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (((HandleUtility.nearestControl == controlID) || firstSelect) && (current.button == 0))
                    {
                        if (!current.shift && !EditorGUI.actionKey)
                        {
                            selection.Clear();
                            flag2 = true;
                        }
                        s_SelectionStart = new List<int>(selection);
                        GUIUtility.hotControl = controlID;
                        s_StartMouseDragPosition = current.mousePosition;
                        s_StartDragSelection = new List<int>(selection);
                        current.Use();
                    }
                    goto Label_02F5;

                case EventType.MouseUp:
                {
                    if ((GUIUtility.hotControl != controlID) || (current.button != 0))
                    {
                        goto Label_02F5;
                    }
                    if (s_DidDrag)
                    {
                        goto Label_0275;
                    }
                    int item = FindNearest(s_StartMouseDragPosition, cloudTransform, points);
                    if (item != -1)
                    {
                        if (current.shift || EditorGUI.actionKey)
                        {
                            int index = selection.IndexOf(item);
                            if (index != -1)
                            {
                                selection.RemoveAt(index);
                            }
                            else
                            {
                                selection.Add(item);
                            }
                            break;
                        }
                        selection.Add(item);
                    }
                    break;
                }
                case EventType.MouseDrag:
                    if ((GUIUtility.hotControl == controlID) && (current.button == 0))
                    {
                        s_DidDrag = true;
                        selection.Clear();
                        selection.AddRange(s_StartDragSelection);
                        Rect rect = FromToRect(s_StartMouseDragPosition, current.mousePosition);
                        Matrix4x4 matrix = Handles.matrix;
                        Handles.matrix = cloudTransform.localToWorldMatrix;
                        for (int i = 0; i < points.Count; i++)
                        {
                            Vector2 point = HandleUtility.WorldToGUIPoint(points.GetPosition(i));
                            if (rect.Contains(point))
                            {
                                if (EditorGUI.actionKey)
                                {
                                    if (s_SelectionStart.Contains(i))
                                    {
                                        selection.Remove(i);
                                    }
                                }
                                else if (!s_SelectionStart.Contains(i))
                                {
                                    selection.Add(i);
                                }
                            }
                        }
                        Handles.matrix = matrix;
                        GUI.changed = true;
                        current.Use();
                    }
                    goto Label_02F5;

                case EventType.Repaint:
                    if ((GUIUtility.hotControl == controlID) && (current.mousePosition != s_StartMouseDragPosition))
                    {
                        GUIStyle style = "SelectionRect";
                        Handles.BeginGUI();
                        style.Draw(FromToRect(s_StartMouseDragPosition, current.mousePosition), false, false, false, false);
                        Handles.EndGUI();
                    }
                    goto Label_02F5;

                case EventType.Layout:
                    HandleUtility.AddDefaultControl(controlID);
                    goto Label_02F5;

                default:
                    goto Label_02F5;
            }
            GUI.changed = true;
            flag2 = true;
        Label_0275:
            s_StartDragSelection = null;
            s_StartMouseDragPosition = Vector2.zero;
            s_DidDrag = false;
            GUIUtility.hotControl = 0;
            current.Use();
        Label_02F5:
            if (flag2)
            {
                selection = selection.Distinct<int>().ToList<int>();
            }
            return flag2;
        }

        [CompilerGenerated]
        private sealed class <MovePoints>c__AnonStorey0
        {
            internal IEditablePoint points;

            internal Vector3 <>m__0(Vector3 current, int index) => 
                (current + this.points.GetPosition(index));
        }
    }
}

