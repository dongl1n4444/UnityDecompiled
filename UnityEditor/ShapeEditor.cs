namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class ShapeEditor
    {
        [CompilerGenerated]
        private static Func<int, Vector3> <>f__am$cache0;
        [CompilerGenerated]
        private static Action<int, Vector3> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<DistanceToControl> <>f__am$cache10;
        [CompilerGenerated]
        private static Action <>f__am$cache11;
        [CompilerGenerated]
        private static Action <>f__am$cache12;
        [CompilerGenerated]
        private static Func<Vector3, Vector3> <>f__am$cache13;
        [CompilerGenerated]
        private static Action<Bounds> <>f__am$cache14;
        [CompilerGenerated]
        private static Action<int> <>f__am$cache15;
        [CompilerGenerated]
        private static Func<bool> <>f__am$cache16;
        [CompilerGenerated]
        private static Func<float> <>f__am$cache17;
        [CompilerGenerated]
        private static Func<int, Vector3> <>f__am$cache2;
        [CompilerGenerated]
        private static Action<int, Vector3> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<int, Vector3> <>f__am$cache4;
        [CompilerGenerated]
        private static Action<int, Vector3> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<int, TangentMode> <>f__am$cache6;
        [CompilerGenerated]
        private static Action<int, TangentMode> <>f__am$cache7;
        [CompilerGenerated]
        private static Action<int, Vector3> <>f__am$cache8;
        [CompilerGenerated]
        private static Action<int> <>f__am$cache9;
        [CompilerGenerated]
        private static Func<int> <>f__am$cacheA;
        [CompilerGenerated]
        private static Func<Vector2, Vector3> <>f__am$cacheB;
        [CompilerGenerated]
        private static Func<Vector3, Vector2> <>f__am$cacheC;
        [CompilerGenerated]
        private static Func<Matrix4x4> <>f__am$cacheD;
        [CompilerGenerated]
        private static Func<DistanceToControl> <>f__am$cacheE;
        [CompilerGenerated]
        private static Func<DistanceToControl> <>f__am$cacheF;
        [CompilerGenerated]
        private static DistanceToControl <>f__mg$cache0;
        [CompilerGenerated]
        private static DistanceToControl <>f__mg$cache1;
        [CompilerGenerated]
        private static DistanceToControl <>f__mg$cache2;
        [CompilerGenerated]
        private static Handles.CapFunction <>f__mg$cache3;
        [CompilerGenerated]
        private static Handles.CapFunction <>f__mg$cache4;
        [CompilerGenerated]
        private static Handles.CapFunction <>f__mg$cache5;
        [CompilerGenerated]
        private static DistanceToControl <>f__mg$cache6;
        [CompilerGenerated]
        private static Handles.CapFunction <>f__mg$cache7;
        [CompilerGenerated]
        private static Handles.CapFunction <>f__mg$cache8;
        [CompilerGenerated]
        private static Handles.CapFunction <>f__mg$cache9;
        [CompilerGenerated]
        private static Handles.CapFunction <>f__mg$cacheA;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <activePoint>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static Color <handleFillColor>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private static Color <handleOutlineColor>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <inEditMode>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private Texture2D <lineTexture>k__BackingField;
        public Func<DistanceToControl> DistanceToCircle;
        public Func<DistanceToControl> DistanceToDiamond;
        public Func<DistanceToControl> DistanceToRectangle;
        public Action<Bounds> Frame;
        public Func<float> GetHandleSize;
        public Func<int, Vector3> GetPointLTangent;
        public Func<int, Vector3> GetPointPosition;
        public Func<int, Vector3> GetPointRTangent;
        public Func<int> GetPointsCount;
        public Func<int, TangentMode> GetTangentMode;
        public Action<int, Vector3> InsertPointAt;
        private const float k_ActiveEdgeWidth = 6f;
        private readonly int k_CreatorID;
        private const float k_EdgeHoverDistance = 9f;
        private readonly int k_EdgeID;
        private const float k_EdgeWidth = 2f;
        private readonly int k_LeftTangentID;
        private const float k_MinExistingPointDistanceForInsert = 20f;
        private readonly int k_RightTangentID;
        private static readonly Color k_SelectedFill = new Color(0.1333333f, 0.6705883f, 1f);
        private static readonly Color k_SelectedHoveredFill = new Color(0.1333333f, 0.6705883f, 1f);
        private static readonly Color k_SelectedHoveredOutline = Color.white;
        private static readonly Color k_SelectedOutline = new Color(0.1333333f, 0.6705883f, 1f);
        private static readonly Color k_TangentColor = new Color(0.1333333f, 0.6705883f, 1f);
        private static readonly Color k_TangentColorAlternative = new Color(0.5137255f, 0.8627451f, 1f);
        private static readonly Color k_UnSelectedFill = Color.white;
        private static readonly Color k_UnSelectedHoveredFill = new Color(0.5137255f, 0.8627451f, 1f);
        private static readonly Color k_UnSelectedHoveredOutline = Color.white;
        private static readonly Color k_UnSelectedOutline = Color.gray;
        public Func<Vector3, Vector2> LocalToScreen;
        public Func<Matrix4x4> LocalToWorldMatrix;
        private int m_ActiveEdge;
        private int m_ActivePointOnLastMouseDown;
        private bool m_DelayedReset;
        private Vector3 m_EdgeDragStartMousePosition;
        private Vector3 m_EdgeDragStartP0;
        private Vector3 m_EdgeDragStartP1;
        private Vector2 m_MousePositionLastMouseDown;
        private bool m_NewPointDragFinished;
        private int m_NewPointIndex;
        private ShapeEditorSelection m_Selection;
        public Action<int> OnPointClick;
        public Func<bool> OpenEnded;
        public Action RecordUndo;
        public Action<int> RemovePointAt;
        public Action Repaint;
        public Func<Vector2, Vector3> ScreenToLocal;
        public Action<int, Vector3> SetPointLTangent;
        public Action<int, Vector3> SetPointPosition;
        public Action<int, Vector3> SetPointRTangent;
        public Action<int, TangentMode> SetTangentMode;
        public Func<Vector3, Vector3> Snap;

        public ShapeEditor()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<int, Vector3>(null, (IntPtr) <GetPointPosition>m__0);
            }
            this.GetPointPosition = <>f__am$cache0;
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Action<int, Vector3>(null, (IntPtr) <SetPointPosition>m__1);
            }
            this.SetPointPosition = <>f__am$cache1;
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<int, Vector3>(null, (IntPtr) <GetPointLTangent>m__2);
            }
            this.GetPointLTangent = <>f__am$cache2;
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = new Action<int, Vector3>(null, (IntPtr) <SetPointLTangent>m__3);
            }
            this.SetPointLTangent = <>f__am$cache3;
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = new Func<int, Vector3>(null, (IntPtr) <GetPointRTangent>m__4);
            }
            this.GetPointRTangent = <>f__am$cache4;
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = new Action<int, Vector3>(null, (IntPtr) <SetPointRTangent>m__5);
            }
            this.SetPointRTangent = <>f__am$cache5;
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = new Func<int, TangentMode>(null, (IntPtr) <GetTangentMode>m__6);
            }
            this.GetTangentMode = <>f__am$cache6;
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = new Action<int, TangentMode>(null, (IntPtr) <SetTangentMode>m__7);
            }
            this.SetTangentMode = <>f__am$cache7;
            if (<>f__am$cache8 == null)
            {
                <>f__am$cache8 = new Action<int, Vector3>(null, (IntPtr) <InsertPointAt>m__8);
            }
            this.InsertPointAt = <>f__am$cache8;
            if (<>f__am$cache9 == null)
            {
                <>f__am$cache9 = new Action<int>(ShapeEditor.<RemovePointAt>m__9);
            }
            this.RemovePointAt = <>f__am$cache9;
            if (<>f__am$cacheA == null)
            {
                <>f__am$cacheA = new Func<int>(null, (IntPtr) <GetPointsCount>m__A);
            }
            this.GetPointsCount = <>f__am$cacheA;
            if (<>f__am$cacheB == null)
            {
                <>f__am$cacheB = new Func<Vector2, Vector3>(null, (IntPtr) <ScreenToLocal>m__B);
            }
            this.ScreenToLocal = <>f__am$cacheB;
            if (<>f__am$cacheC == null)
            {
                <>f__am$cacheC = new Func<Vector3, Vector2>(null, (IntPtr) <LocalToScreen>m__C);
            }
            this.LocalToScreen = <>f__am$cacheC;
            if (<>f__am$cacheD == null)
            {
                <>f__am$cacheD = new Func<Matrix4x4>(null, (IntPtr) <LocalToWorldMatrix>m__D);
            }
            this.LocalToWorldMatrix = <>f__am$cacheD;
            if (<>f__am$cacheE == null)
            {
                <>f__am$cacheE = new Func<DistanceToControl>(null, (IntPtr) <DistanceToRectangle>m__E);
            }
            this.DistanceToRectangle = <>f__am$cacheE;
            if (<>f__am$cacheF == null)
            {
                <>f__am$cacheF = new Func<DistanceToControl>(null, (IntPtr) <DistanceToDiamond>m__F);
            }
            this.DistanceToDiamond = <>f__am$cacheF;
            if (<>f__am$cache10 == null)
            {
                <>f__am$cache10 = new Func<DistanceToControl>(null, (IntPtr) <DistanceToCircle>m__10);
            }
            this.DistanceToCircle = <>f__am$cache10;
            if (<>f__am$cache11 == null)
            {
                <>f__am$cache11 = new Action(null, (IntPtr) <Repaint>m__11);
            }
            this.Repaint = <>f__am$cache11;
            if (<>f__am$cache12 == null)
            {
                <>f__am$cache12 = new Action(null, (IntPtr) <RecordUndo>m__12);
            }
            this.RecordUndo = <>f__am$cache12;
            if (<>f__am$cache13 == null)
            {
                <>f__am$cache13 = new Func<Vector3, Vector3>(null, (IntPtr) <Snap>m__13);
            }
            this.Snap = <>f__am$cache13;
            if (<>f__am$cache14 == null)
            {
                <>f__am$cache14 = new Action<Bounds>(ShapeEditor.<Frame>m__14);
            }
            this.Frame = <>f__am$cache14;
            if (<>f__am$cache15 == null)
            {
                <>f__am$cache15 = new Action<int>(ShapeEditor.<OnPointClick>m__15);
            }
            this.OnPointClick = <>f__am$cache15;
            if (<>f__am$cache16 == null)
            {
                <>f__am$cache16 = new Func<bool>(null, (IntPtr) <OpenEnded>m__16);
            }
            this.OpenEnded = <>f__am$cache16;
            if (<>f__am$cache17 == null)
            {
                <>f__am$cache17 = new Func<float>(null, (IntPtr) <GetHandleSize>m__17);
            }
            this.GetHandleSize = <>f__am$cache17;
            this.m_ActivePointOnLastMouseDown = -1;
            this.m_NewPointIndex = -1;
            this.m_ActiveEdge = -1;
            this.m_DelayedReset = false;
            this.k_CreatorID = GUIUtility.GetPermanentControlID();
            this.k_EdgeID = GUIUtility.GetPermanentControlID();
            this.k_RightTangentID = GUIUtility.GetPermanentControlID();
            this.k_LeftTangentID = GUIUtility.GetPermanentControlID();
            this.m_Selection = new ShapeEditorSelection(this);
        }

        [CompilerGenerated]
        private static DistanceToControl <DistanceToCircle>m__10()
        {
            if (<>f__mg$cache2 == null)
            {
                <>f__mg$cache2 = new DistanceToControl(ShapeEditor.DistanceToCircleInternal);
            }
            return <>f__mg$cache2;
        }

        [CompilerGenerated]
        private static DistanceToControl <DistanceToDiamond>m__F()
        {
            if (<>f__mg$cache1 == null)
            {
                <>f__mg$cache1 = new DistanceToControl(HandleUtility.DistanceToDiamond);
            }
            return <>f__mg$cache1;
        }

        [CompilerGenerated]
        private static DistanceToControl <DistanceToRectangle>m__E()
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new DistanceToControl(HandleUtility.DistanceToRectangle);
            }
            return <>f__mg$cache0;
        }

        [CompilerGenerated]
        private static void <Frame>m__14(Bounds b)
        {
        }

        [CompilerGenerated]
        private static float <GetHandleSize>m__17()
        {
            return 5f;
        }

        [CompilerGenerated]
        private static Vector3 <GetPointLTangent>m__2(int i)
        {
            return Vector3.zero;
        }

        [CompilerGenerated]
        private static Vector3 <GetPointPosition>m__0(int i)
        {
            return Vector3.zero;
        }

        [CompilerGenerated]
        private static Vector3 <GetPointRTangent>m__4(int i)
        {
            return Vector3.zero;
        }

        [CompilerGenerated]
        private static int <GetPointsCount>m__A()
        {
            return 0;
        }

        [CompilerGenerated]
        private static TangentMode <GetTangentMode>m__6(int i)
        {
            return TangentMode.Linear;
        }

        [CompilerGenerated]
        private static void <InsertPointAt>m__8(int i, Vector3 p)
        {
        }

        [CompilerGenerated]
        private static Vector2 <LocalToScreen>m__C(Vector3 i)
        {
            return i;
        }

        [CompilerGenerated]
        private static Matrix4x4 <LocalToWorldMatrix>m__D()
        {
            return Matrix4x4.identity;
        }

        [CompilerGenerated]
        private static void <OnPointClick>m__15(int i)
        {
        }

        [CompilerGenerated]
        private static bool <OpenEnded>m__16()
        {
            return false;
        }

        [CompilerGenerated]
        private static void <RecordUndo>m__12()
        {
        }

        [CompilerGenerated]
        private static void <RemovePointAt>m__9(int i)
        {
        }

        [CompilerGenerated]
        private static void <Repaint>m__11()
        {
        }

        [CompilerGenerated]
        private static Vector3 <ScreenToLocal>m__B(Vector2 i)
        {
            return (Vector3) i;
        }

        [CompilerGenerated]
        private static void <SetPointLTangent>m__3(int i, Vector3 p)
        {
        }

        [CompilerGenerated]
        private static void <SetPointPosition>m__1(int i, Vector3 p)
        {
        }

        [CompilerGenerated]
        private static void <SetPointRTangent>m__5(int i, Vector3 p)
        {
        }

        [CompilerGenerated]
        private static void <SetTangentMode>m__7(int i, TangentMode m)
        {
        }

        [CompilerGenerated]
        private static Vector3 <Snap>m__13(Vector3 i)
        {
            return i;
        }

        public static void CircleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            if (eventType == EventType.Layout)
            {
                HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(position, size * 0.5f));
            }
            else if (eventType == EventType.Repaint)
            {
                Handles.StartCapDraw(position, rotation, size);
                Vector3 lhs = (Vector3) ((handleMatrixrotation * rotation) * Vector3.forward);
                Vector3 from = Vector3.Cross(lhs, Vector3.up);
                if (from.sqrMagnitude < 0.001f)
                {
                    from = Vector3.Cross(lhs, Vector3.right);
                }
                Vector3[] dest = new Vector3[60];
                Handles.SetDiscSectionPoints(dest, 60, position, lhs, from, 360f, size);
                HandleUtility.ApplyWireMaterial();
                GL.PushMatrix();
                GL.Begin(4);
                GL.MultMatrix(Handles.matrix);
                GL.Color(handleFillColor);
                for (int i = 1; i < dest.Length; i++)
                {
                    GL.Vertex(position);
                    GL.Vertex(dest[i - 1]);
                    GL.Vertex(dest[i]);
                }
                GL.End();
                GL.Begin(1);
                GL.Color(handleOutlineColor);
                for (int j = 0; j < (dest.Length - 1); j++)
                {
                    GL.Vertex(dest[j]);
                    GL.Vertex(dest[j + 1]);
                }
                GL.End();
                GL.PopMatrix();
            }
        }

        public void CycleTangentMode()
        {
            TangentMode current = this.GetTangentMode.Invoke(this.activePoint);
            TangentMode nextTangentMode = GetNextTangentMode(current);
            this.SetTangentMode.Invoke(this.activePoint, nextTangentMode);
            this.RefreshTangentsAfterModeChange(this.activePoint, current, nextTangentMode);
        }

        private void DelayedResetIfNecessary()
        {
            if (this.m_DelayedReset)
            {
                GUIUtility.hotControl = 0;
                GUIUtility.keyboardControl = 0;
                this.m_Selection.Clear();
                this.activePoint = -1;
                this.m_DelayedReset = false;
            }
        }

        public static void DiamondCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            if (eventType == EventType.Layout)
            {
                HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(position, size * 0.5f));
            }
            else if (eventType == EventType.Repaint)
            {
                Vector3 vector3 = ProjectPointOnPlane((Vector3) Handles.matrix.GetColumn(2), position, position + Vector3.up) - position;
                Vector3 normalized = vector3.normalized;
                Quaternion quaternion = Quaternion.LookRotation((Vector3) Handles.matrix.GetColumn(2), normalized);
                Vector3 vector4 = (Vector3) (((quaternion * Vector3.right) * size) * 1.25f);
                Vector3 vector5 = (Vector3) (((quaternion * Vector3.up) * size) * 1.25f);
                HandleUtility.ApplyWireMaterial();
                GL.PushMatrix();
                GL.Begin(7);
                GL.MultMatrix(Handles.matrix);
                GL.Color(handleFillColor);
                GL.Vertex(position + vector4);
                GL.Vertex(position - vector5);
                GL.Vertex(position - vector4);
                GL.Vertex(position + vector5);
                GL.End();
                GL.Begin(1);
                GL.Color(handleOutlineColor);
                GL.Vertex(position + vector4);
                GL.Vertex(position - vector5);
                GL.Vertex(position - vector5);
                GL.Vertex(position - vector4);
                GL.Vertex(position - vector4);
                GL.Vertex(position + vector5);
                GL.Vertex(position + vector5);
                GL.Vertex(position + vector4);
                GL.End();
                GL.PopMatrix();
            }
        }

        private static float DistanceToCircleInternal(Vector3 position, Quaternion rotation, float size)
        {
            return HandleUtility.DistanceToCircle(position, size);
        }

        private static Vector3 DoSlider(int id, Vector3 position, Vector3 slide1, Vector3 slide2, float s, Handles.CapFunction cap)
        {
            return Slider2D.Do(id, position, Vector3.zero, Vector3.Cross(slide1, slide2), slide1, slide2, s, cap, Vector2.zero, false);
        }

        private Vector3 DoTangent(Vector3 p0, Vector3 t0, int cid, int pointIndex, Color color)
        {
            float handleSizeForPoint = this.GetHandleSizeForPoint(pointIndex);
            float tangentSizeForPoint = this.GetTangentSizeForPoint(pointIndex);
            Handles.color = color;
            float num3 = HandleUtility.DistanceToCircle(t0, tangentSizeForPoint);
            if (this.lineTexture != null)
            {
                Vector3[] points = new Vector3[] { p0, t0 };
                Handles.DrawAAPolyLine(this.lineTexture, points);
            }
            else
            {
                Handles.DrawLine(p0, t0);
            }
            handleOutlineColor = (num3 <= 0f) ? k_SelectedHoveredOutline : color;
            handleFillColor = color;
            Vector3 vector = DoSlider(cid, t0, Vector3.up, Vector3.right, tangentSizeForPoint, this.GetCapForTangent(pointIndex)) - p0;
            return ((vector.magnitude >= handleSizeForPoint) ? vector : Vector3.zero);
        }

        private static bool EdgeDragModifiersActive()
        {
            return ((Event.current.modifiers == EventModifiers.Control) || (Event.current.modifiers == EventModifiers.Command));
        }

        public void Edges()
        {
            int total = this.GetPointsCount.Invoke();
            int closestEdge = -1;
            float maxValue = float.MaxValue;
            int index = 0;
            int num5 = NextIndex(index, total);
            int num6 = !this.OpenEnded.Invoke() ? total : (total - 1);
            for (int i = 0; i < num6; i++)
            {
                Vector3 startPosition = this.GetPointPosition.Invoke(index);
                Vector3 endPosition = this.GetPointPosition.Invoke(num5);
                Vector3 startTangent = startPosition + this.GetPointRTangent.Invoke(index);
                Vector3 endTangent = endPosition + this.GetPointLTangent.Invoke(num5);
                Vector2 vector5 = this.LocalToScreen.Invoke(startPosition);
                Vector2 vector6 = this.LocalToScreen.Invoke(endPosition);
                Vector2 vector7 = this.LocalToScreen.Invoke(startTangent);
                Vector2 vector8 = this.LocalToScreen.Invoke(endTangent);
                float num8 = HandleUtility.DistancePointBezier((Vector3) Event.current.mousePosition, (Vector3) vector5, (Vector3) vector6, (Vector3) vector7, (Vector3) vector8);
                Color color = (index != this.m_ActiveEdge) ? Color.white : Color.yellow;
                float width = ((index != this.m_ActiveEdge) && (!EdgeDragModifiersActive() || (num8 >= 9f))) ? 2f : 6f;
                Handles.DrawBezier(startPosition, endPosition, startTangent, endTangent, color, this.lineTexture, width);
                if (num8 < maxValue)
                {
                    closestEdge = index;
                    maxValue = num8;
                }
                index = NextIndex(index, total);
                num5 = NextIndex(num5, total);
            }
            if (this.inEditMode)
            {
                this.HandlePointInsertToEdge(closestEdge, maxValue);
                this.HandleEdgeDragging(closestEdge, maxValue);
            }
            if ((GUIUtility.hotControl != this.k_CreatorID) && (this.m_NewPointIndex != -1))
            {
                this.m_NewPointDragFinished = true;
                GUIUtility.keyboardControl = 0;
                this.m_NewPointIndex = -1;
            }
            if ((GUIUtility.hotControl != this.k_EdgeID) && (this.m_ActiveEdge != -1))
            {
                this.m_ActiveEdge = -1;
            }
        }

        private int FindClosestPointIndex(Vector3 position)
        {
            float maxValue = float.MaxValue;
            int num2 = -1;
            for (int i = 0; i < this.GetPointsCount.Invoke(); i++)
            {
                Vector3 vector2 = this.GetPointPosition.Invoke(i) - position;
                float magnitude = vector2.magnitude;
                if (magnitude < maxValue)
                {
                    num2 = i;
                    maxValue = magnitude;
                }
            }
            return num2;
        }

        public Vector3 FindClosestPointOnEdge(int edgeIndex, Vector3 position, int iterations)
        {
            float num = 1f / ((float) iterations);
            float maxValue = float.MaxValue;
            float index = edgeIndex;
            for (float i = 0f; i <= 1f; i += num)
            {
                Vector3 positionByIndex = this.GetPositionByIndex(edgeIndex + i);
                Vector3 vector2 = position - positionByIndex;
                float magnitude = vector2.magnitude;
                if (magnitude < maxValue)
                {
                    maxValue = magnitude;
                    index = edgeIndex + i;
                }
            }
            return this.GetPositionByIndex(index);
        }

        private int FindClosestPointToMouse()
        {
            Vector3 position = this.ScreenToLocal.Invoke(Event.current.mousePosition);
            return this.FindClosestPointIndex(position);
        }

        private void Framing()
        {
            if ((Event.current.commandName == "FrameSelected") && (this.m_Selection.Count > 0))
            {
                EventType type = Event.current.type;
                if (type != EventType.ExecuteCommand)
                {
                    if (type != EventType.ValidateCommand)
                    {
                        return;
                    }
                }
                else
                {
                    Bounds bounds = new Bounds(this.GetPointPosition.Invoke(Enumerable.First<int>(this.selectedPoints)), Vector3.zero);
                    foreach (int num in this.selectedPoints)
                    {
                        bounds.Encapsulate(this.GetPointPosition.Invoke(num));
                    }
                    this.Frame(bounds);
                }
                Event.current.Use();
            }
        }

        private void FromAllZeroToTangents(int pointIndex)
        {
            Vector3 vector = this.GetPointPosition.Invoke(pointIndex);
            int num = (pointIndex <= 0) ? (this.GetPointsCount.Invoke() - 1) : (pointIndex - 1);
            Vector3 vector2 = (Vector3) ((this.GetPointPosition.Invoke(num) - vector) * 0.33f);
            Vector3 vector3 = -vector2;
            Vector2 vector4 = this.LocalToScreen.Invoke(vector) - this.LocalToScreen.Invoke(vector + vector2);
            float magnitude = vector4.magnitude;
            Vector2 vector5 = this.LocalToScreen.Invoke(vector) - this.LocalToScreen.Invoke(vector + vector3);
            float num3 = vector5.magnitude;
            vector2 = (Vector3) (vector2 * Mathf.Min((float) (100f / magnitude), (float) 1f));
            vector3 = (Vector3) (vector3 * Mathf.Min((float) (100f / num3), (float) 1f));
            this.SetPointLTangent.Invoke(pointIndex, vector2);
            this.SetPointRTangent.Invoke(pointIndex, vector3);
        }

        private Handles.CapFunction GetCapForPoint(int index)
        {
            TangentMode mode = this.GetTangentMode.Invoke(index);
            if (mode != TangentMode.Broken)
            {
                if (mode == TangentMode.Continuous)
                {
                    if (<>f__mg$cache8 == null)
                    {
                        <>f__mg$cache8 = new Handles.CapFunction(ShapeEditor.CircleCap);
                    }
                    return <>f__mg$cache8;
                }
                if (mode == TangentMode.Linear)
                {
                    if (<>f__mg$cache9 == null)
                    {
                        <>f__mg$cache9 = new Handles.CapFunction(ShapeEditor.RectCap);
                    }
                    return <>f__mg$cache9;
                }
                if (<>f__mg$cacheA == null)
                {
                    <>f__mg$cacheA = new Handles.CapFunction(ShapeEditor.DiamondCap);
                }
                return <>f__mg$cacheA;
            }
            if (<>f__mg$cache7 == null)
            {
                <>f__mg$cache7 = new Handles.CapFunction(ShapeEditor.DiamondCap);
            }
            return <>f__mg$cache7;
        }

        private Handles.CapFunction GetCapForTangent(int index)
        {
            if (((TangentMode) this.GetTangentMode.Invoke(index)) == TangentMode.Continuous)
            {
                if (<>f__mg$cache4 == null)
                {
                    <>f__mg$cache4 = new Handles.CapFunction(ShapeEditor.CircleCap);
                }
                return <>f__mg$cache4;
            }
            if (<>f__mg$cache5 == null)
            {
                <>f__mg$cache5 = new Handles.CapFunction(ShapeEditor.DiamondCap);
            }
            return <>f__mg$cache5;
        }

        private DistanceToControl GetDistanceFuncForPoint(int index)
        {
            TangentMode mode = this.GetTangentMode.Invoke(index);
            if (mode != TangentMode.Broken)
            {
                if (mode == TangentMode.Continuous)
                {
                    return this.DistanceToCircle.Invoke();
                }
                if (mode == TangentMode.Linear)
                {
                    return this.DistanceToRectangle.Invoke();
                }
            }
            else
            {
                return this.DistanceToDiamond.Invoke();
            }
            return this.DistanceToRectangle.Invoke();
        }

        private DistanceToControl GetDistanceFuncForTangent(int index)
        {
            if (((TangentMode) this.GetTangentMode.Invoke(index)) == TangentMode.Continuous)
            {
                return this.DistanceToCircle.Invoke();
            }
            if (<>f__mg$cache6 == null)
            {
                <>f__mg$cache6 = new DistanceToControl(HandleUtility.DistanceToDiamond);
            }
            return <>f__mg$cache6;
        }

        private Color GetFillColorForPoint(int pointIndex, int handleID)
        {
            bool flag = this.MouseDistanceToPoint(pointIndex) <= 0f;
            bool flag2 = this.m_Selection.Contains(pointIndex);
            if ((flag && flag2) || (GUIUtility.hotControl == handleID))
            {
                return k_SelectedHoveredFill;
            }
            if (flag)
            {
                return k_UnSelectedHoveredFill;
            }
            if (flag2)
            {
                return k_SelectedFill;
            }
            return k_UnSelectedFill;
        }

        private float GetHandleSizeForPoint(int index)
        {
            return ((Camera.current == null) ? (this.GetHandleSize.Invoke() / Handles.matrix.m00) : (HandleUtility.GetHandleSize(this.GetPointPosition.Invoke(index)) * 0.075f));
        }

        public static TangentMode GetNextTangentMode(TangentMode current)
        {
            return ((current + 1) % Enum.GetValues(typeof(TangentMode)).Length);
        }

        private Color GetOutlineColorForPoint(int pointIndex, int handleID)
        {
            bool flag = this.MouseDistanceToPoint(pointIndex) <= 0f;
            bool flag2 = this.m_Selection.Contains(pointIndex);
            if ((flag && flag2) || (GUIUtility.hotControl == handleID))
            {
                return k_SelectedHoveredOutline;
            }
            if (flag)
            {
                return k_UnSelectedHoveredOutline;
            }
            if (flag2)
            {
                return k_SelectedOutline;
            }
            return k_UnSelectedOutline;
        }

        private static Vector3 GetPoint(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, float t)
        {
            t = Mathf.Clamp01(t);
            float num = 1f - t;
            return (Vector3) ((((((num * num) * num) * startPosition) + ((((3f * num) * num) * t) * (startPosition + startTangent))) + ((((3f * num) * t) * t) * (endPosition + endTangent))) + (((t * t) * t) * endPosition));
        }

        private Vector3 GetPositionByIndex(float index)
        {
            int num = Mathf.FloorToInt(index);
            int num2 = NextIndex(num, this.GetPointsCount.Invoke());
            float t = index - num;
            return GetPoint(this.GetPointPosition.Invoke(num), this.GetPointPosition.Invoke(num2), this.GetPointRTangent.Invoke(num), this.GetPointLTangent.Invoke(num2), t);
        }

        private float GetTangentSizeForPoint(int index)
        {
            return (this.GetHandleSizeForPoint(index) * 0.8f);
        }

        private void HandleEdgeDragging(int closestEdge, float closestEdgeDist)
        {
            bool flag = GUIUtility.hotControl == this.k_EdgeID;
            bool flag2 = this.MouseDistanceToPoint(this.FindClosestPointToMouse()) > 20f;
            bool flag3 = this.MouseDistanceToClosestTangent() > 20f;
            bool flag4 = flag2 && flag3;
            if (((((closestEdgeDist < 9f) && flag4) && !this.m_Selection.isSelecting) && EdgeDragModifiersActive()) || flag)
            {
                switch (Event.current.type)
                {
                    case EventType.MouseDown:
                        this.m_ActiveEdge = closestEdge;
                        this.m_EdgeDragStartP0 = this.GetPointPosition.Invoke(this.m_ActiveEdge);
                        this.m_EdgeDragStartP1 = this.GetPointPosition.Invoke(NextIndex(this.m_ActiveEdge, this.GetPointsCount.Invoke()));
                        if (Event.current.shift)
                        {
                            this.RecordUndo.Invoke();
                            this.InsertPointAt.Invoke(this.m_ActiveEdge + 1, this.m_EdgeDragStartP0);
                            this.InsertPointAt.Invoke(this.m_ActiveEdge + 2, this.m_EdgeDragStartP1);
                            this.m_ActiveEdge++;
                        }
                        this.m_EdgeDragStartMousePosition = this.ScreenToLocal.Invoke(Event.current.mousePosition);
                        GUIUtility.hotControl = this.k_EdgeID;
                        Event.current.Use();
                        break;

                    case EventType.MouseDrag:
                    {
                        this.RecordUndo.Invoke();
                        Vector3 vector2 = this.ScreenToLocal.Invoke(Event.current.mousePosition) - this.m_EdgeDragStartMousePosition;
                        Vector3 vector3 = this.GetPointPosition.Invoke(this.m_ActiveEdge);
                        Vector3 vector4 = this.m_EdgeDragStartP0 + vector2;
                        Vector3 vector5 = this.Snap.Invoke(vector4) - vector3;
                        int activeEdge = this.m_ActiveEdge;
                        int num2 = NextIndex(this.m_ActiveEdge, this.GetPointsCount.Invoke());
                        this.SetPointPosition.Invoke(this.m_ActiveEdge, this.GetPointPosition.Invoke(activeEdge) + vector5);
                        this.SetPointPosition.Invoke(num2, this.GetPointPosition.Invoke(num2) + vector5);
                        Event.current.Use();
                        break;
                    }
                    case EventType.MouseUp:
                        this.m_ActiveEdge = -1;
                        GUIUtility.hotControl = 0;
                        Event.current.Use();
                        break;
                }
            }
        }

        public void HandlePointClick(int pointIndex)
        {
            if (this.m_Selection.Count > 1)
            {
                this.m_Selection.SelectPoint(pointIndex, SelectionType.Normal);
            }
            else if ((!Event.current.control && !Event.current.shift) && (this.m_ActivePointOnLastMouseDown == this.activePoint))
            {
                this.OnPointClick(pointIndex);
            }
        }

        public void HandlePointInsertToEdge(int closestEdge, float closestEdgeDist)
        {
            bool flag = GUIUtility.hotControl == this.k_CreatorID;
            bool flag2 = this.MouseDistanceToPoint(this.FindClosestPointToMouse()) > 20f;
            bool flag3 = this.MouseDistanceToClosestTangent() > 20f;
            bool flag4 = flag2 && flag3;
            if (((((closestEdgeDist < 9f) && flag4) && !this.m_Selection.isSelecting) && (Event.current.modifiers == EventModifiers.None)) || flag)
            {
                Vector3 position = !flag ? this.FindClosestPointOnEdge(closestEdge, this.ScreenToLocal.Invoke(Event.current.mousePosition), 100) : this.GetPointPosition.Invoke(this.m_NewPointIndex);
                EditorGUI.BeginChangeCheck();
                handleFillColor = k_SelectedHoveredFill;
                handleOutlineColor = k_SelectedHoveredOutline;
                if (!flag)
                {
                    handleFillColor = handleFillColor.AlphaMultiplied(0.5f);
                    handleOutlineColor = handleOutlineColor.AlphaMultiplied(0.5f);
                }
                int hotControl = GUIUtility.hotControl;
                if (<>f__mg$cache3 == null)
                {
                    <>f__mg$cache3 = new Handles.CapFunction(ShapeEditor.RectCap);
                }
                Vector3 vector2 = DoSlider(this.k_CreatorID, position, Vector3.up, Vector3.right, this.GetHandleSizeForPoint(closestEdge), <>f__mg$cache3);
                if ((hotControl != this.k_CreatorID) && (GUIUtility.hotControl == this.k_CreatorID))
                {
                    this.RecordUndo.Invoke();
                    this.m_NewPointIndex = NextIndex(closestEdge, this.GetPointsCount.Invoke());
                    this.InsertPointAt.Invoke(this.m_NewPointIndex, vector2);
                    this.m_Selection.SelectPoint(this.m_NewPointIndex, SelectionType.Normal);
                }
                else if (EditorGUI.EndChangeCheck())
                {
                    this.RecordUndo.Invoke();
                    vector2 = this.Snap.Invoke(vector2);
                    this.m_Selection.MoveSelection(vector2 - position);
                }
            }
        }

        private static int mod(int x, int m)
        {
            int num = x % m;
            return ((num >= 0) ? num : (num + m));
        }

        private float MouseDistanceToClosestTangent()
        {
            if (this.activePoint < 0)
            {
                return float.MaxValue;
            }
            Vector3 vector = this.GetPointLTangent.Invoke(this.activePoint);
            Vector3 vector2 = this.GetPointRTangent.Invoke(this.activePoint);
            if ((vector.sqrMagnitude == 0f) && (vector2.sqrMagnitude == 0f))
            {
                return float.MaxValue;
            }
            Vector3 vector3 = this.GetPointPosition.Invoke(this.activePoint);
            float tangentSizeForPoint = this.GetTangentSizeForPoint(this.activePoint);
            return Mathf.Min(HandleUtility.DistanceToRectangle(vector3 + vector, Quaternion.identity, tangentSizeForPoint), HandleUtility.DistanceToRectangle(vector3 + vector2, Quaternion.identity, tangentSizeForPoint));
        }

        private float MouseDistanceToPoint(int index)
        {
            TangentMode mode = this.GetTangentMode.Invoke(index);
            if (mode != TangentMode.Broken)
            {
                if (mode == TangentMode.Linear)
                {
                    return HandleUtility.DistanceToRectangle(this.GetPointPosition.Invoke(index), Quaternion.identity, this.GetHandleSizeForPoint(index));
                }
                if (mode == TangentMode.Continuous)
                {
                    return HandleUtility.DistanceToCircle(this.GetPointPosition.Invoke(index), this.GetHandleSizeForPoint(index));
                }
            }
            else
            {
                return HandleUtility.DistanceToDiamond(this.GetPointPosition.Invoke(index), Quaternion.identity, this.GetHandleSizeForPoint(index));
            }
            return float.MaxValue;
        }

        private static int NextIndex(int index, int total)
        {
            return mod(index + 1, total);
        }

        public void OnGUI()
        {
            this.DelayedResetIfNecessary();
            if (Event.current.type == EventType.MouseDown)
            {
                this.StoreMouseDownState();
            }
            Color color = Handles.color;
            Matrix4x4 matrix = Handles.matrix;
            Handles.matrix = this.LocalToWorldMatrix.Invoke();
            this.Edges();
            if (this.inEditMode)
            {
                this.Framing();
                this.Tangents();
                this.Points();
                this.Selection();
            }
            Handles.color = color;
            Handles.matrix = matrix;
        }

        public void Points()
        {
            bool flag = ((Event.current.type == EventType.ExecuteCommand) || (Event.current.type == EventType.ValidateCommand)) && ((Event.current.commandName == "SoftDelete") || (Event.current.commandName == "Delete"));
            for (int i = 0; i < this.GetPointsCount.Invoke(); i++)
            {
                if (i != this.m_NewPointIndex)
                {
                    Vector3 position = this.GetPointPosition.Invoke(i);
                    int controlID = GUIUtility.GetControlID(0x14e9, FocusType.Keyboard);
                    bool flag2 = this.m_Selection.Contains(i);
                    bool flag3 = Event.current.GetTypeForControl(controlID) == EventType.MouseDown;
                    bool flag4 = Event.current.GetTypeForControl(controlID) == EventType.MouseUp;
                    EditorGUI.BeginChangeCheck();
                    handleOutlineColor = this.GetOutlineColorForPoint(i, controlID);
                    handleFillColor = this.GetFillColorForPoint(i, controlID);
                    Vector3 vector2 = position;
                    int hotControl = GUIUtility.hotControl;
                    if (!Event.current.alt || (GUIUtility.hotControl == controlID))
                    {
                        vector2 = DoSlider(controlID, position, Vector3.up, Vector3.right, this.GetHandleSizeForPoint(i), this.GetCapForPoint(i));
                    }
                    else if (Event.current.type == EventType.Repaint)
                    {
                        this.GetCapForPoint(i)(controlID, position, Quaternion.LookRotation(Vector3.forward, Vector3.up), this.GetHandleSizeForPoint(i), Event.current.type);
                    }
                    int num4 = GUIUtility.hotControl;
                    if (((flag4 && (hotControl == controlID)) && ((num4 == 0) && (Event.current.mousePosition == this.m_MousePositionLastMouseDown))) && !Event.current.shift)
                    {
                        this.HandlePointClick(i);
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        this.RecordUndo.Invoke();
                        vector2 = this.Snap.Invoke(vector2);
                        this.m_Selection.MoveSelection(vector2 - position);
                    }
                    if (((GUIUtility.hotControl == controlID) && !flag2) && flag3)
                    {
                        this.m_Selection.SelectPoint(i, !Event.current.shift ? SelectionType.Normal : SelectionType.Additive);
                        this.Repaint.Invoke();
                    }
                    if ((this.m_NewPointDragFinished && (this.activePoint == i)) && (controlID != -1))
                    {
                        GUIUtility.keyboardControl = controlID;
                        this.m_NewPointDragFinished = false;
                    }
                }
            }
            if (flag)
            {
                if (Event.current.type == EventType.ValidateCommand)
                {
                    Event.current.Use();
                }
                else if (Event.current.type == EventType.ExecuteCommand)
                {
                    this.RecordUndo.Invoke();
                    this.m_Selection.DeleteSelection();
                    Event.current.Use();
                }
            }
        }

        private static int PreviousIndex(int index, int total)
        {
            return mod(index - 1, total);
        }

        private static Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
        {
            planeNormal.Normalize();
            float num = -Vector3.Dot(planeNormal.normalized, point - planePoint);
            return (point + ((Vector3) (planeNormal * num)));
        }

        public static void RectCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            if (eventType == EventType.Layout)
            {
                HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(position, size * 0.5f));
            }
            else if (eventType == EventType.Repaint)
            {
                Vector3 vector3 = ProjectPointOnPlane((Vector3) Handles.matrix.GetColumn(2), position, position + Vector3.up) - position;
                Vector3 normalized = vector3.normalized;
                Quaternion quaternion = Quaternion.LookRotation((Vector3) Handles.matrix.GetColumn(2), normalized);
                Vector3 vector4 = (Vector3) ((quaternion * Vector3.right) * size);
                Vector3 vector5 = (Vector3) ((quaternion * Vector3.up) * size);
                HandleUtility.ApplyWireMaterial();
                GL.PushMatrix();
                GL.MultMatrix(Handles.matrix);
                GL.Begin(7);
                GL.Color(handleFillColor);
                GL.Vertex((position + vector4) + vector5);
                GL.Vertex((position + vector4) - vector5);
                GL.Vertex((position - vector4) - vector5);
                GL.Vertex((position - vector4) + vector5);
                GL.End();
                GL.Begin(1);
                GL.Color(handleOutlineColor);
                GL.Vertex((position + vector4) + vector5);
                GL.Vertex((position + vector4) - vector5);
                GL.Vertex((position + vector4) - vector5);
                GL.Vertex((position - vector4) - vector5);
                GL.Vertex((position - vector4) - vector5);
                GL.Vertex((position - vector4) + vector5);
                GL.Vertex((position - vector4) + vector5);
                GL.Vertex((position + vector4) + vector5);
                GL.End();
                GL.PopMatrix();
            }
        }

        private void RefreshTangents(int index, bool rightIsActive)
        {
            TangentMode mode = this.GetTangentMode.Invoke(index);
            Vector3 vector = this.GetPointLTangent.Invoke(index);
            Vector3 vector2 = this.GetPointRTangent.Invoke(index);
            if (mode == TangentMode.Continuous)
            {
                if (rightIsActive)
                {
                    vector = -vector2;
                    float magnitude = vector.magnitude;
                    vector = (Vector3) (vector.normalized * magnitude);
                }
                else
                {
                    vector2 = -vector;
                    float num2 = vector2.magnitude;
                    vector2 = (Vector3) (vector2.normalized * num2);
                }
            }
            this.SetPointLTangent.Invoke(this.activePoint, vector);
            this.SetPointRTangent.Invoke(this.activePoint, vector2);
        }

        public void RefreshTangentsAfterModeChange(int pointIndex, TangentMode oldMode, TangentMode newMode)
        {
            if ((oldMode != TangentMode.Linear) && (newMode == TangentMode.Linear))
            {
                this.SetPointLTangent.Invoke(pointIndex, Vector3.zero);
                this.SetPointRTangent.Invoke(pointIndex, Vector3.zero);
            }
            if (newMode == TangentMode.Continuous)
            {
                if (oldMode == TangentMode.Broken)
                {
                    this.SetPointRTangent.Invoke(pointIndex, (Vector3) (this.GetPointLTangent.Invoke(pointIndex) * -1f));
                }
                if (oldMode == TangentMode.Linear)
                {
                    this.FromAllZeroToTangents(pointIndex);
                }
            }
        }

        private void Selection()
        {
            this.m_Selection.OnGUI();
        }

        private void StoreMouseDownState()
        {
            this.m_MousePositionLastMouseDown = Event.current.mousePosition;
            this.m_ActivePointOnLastMouseDown = this.activePoint;
        }

        public void Tangents()
        {
            if (((this.activePoint >= 0) && (this.m_Selection.Count <= 1)) && (((TangentMode) this.GetTangentMode.Invoke(this.activePoint)) != TangentMode.Linear))
            {
                Event current = Event.current;
                Vector3 vector = this.GetPointPosition.Invoke(this.activePoint);
                Vector3 vector2 = this.GetPointLTangent.Invoke(this.activePoint);
                Vector3 vector3 = this.GetPointRTangent.Invoke(this.activePoint);
                bool flag = (GUIUtility.hotControl == this.k_RightTangentID) || (GUIUtility.hotControl == this.k_LeftTangentID);
                bool flag2 = (vector2.sqrMagnitude == 0f) && (vector3.sqrMagnitude == 0f);
                if (flag || !flag2)
                {
                    TangentMode mode = this.GetTangentMode.Invoke(this.activePoint);
                    bool flag3 = (current.GetTypeForControl(this.k_RightTangentID) == EventType.MouseDown) || (current.GetTypeForControl(this.k_LeftTangentID) == EventType.MouseDown);
                    bool flag4 = (current.GetTypeForControl(this.k_RightTangentID) == EventType.MouseUp) || (current.GetTypeForControl(this.k_LeftTangentID) == EventType.MouseUp);
                    Vector3 vector4 = this.DoTangent(vector, vector + vector2, this.k_LeftTangentID, this.activePoint, k_TangentColor);
                    Vector3 vector5 = this.DoTangent(vector, vector + vector3, this.k_RightTangentID, this.activePoint, (((TangentMode) this.GetTangentMode.Invoke(this.activePoint)) != TangentMode.Broken) ? k_TangentColor : k_TangentColorAlternative);
                    bool flag5 = (vector4 != vector2) || (vector5 != vector3);
                    flag2 = (vector4.sqrMagnitude == 0f) && (vector5.sqrMagnitude == 0f);
                    if (flag && flag3)
                    {
                        int num = (int) ((mode + 1) % (TangentMode.Broken | TangentMode.Continuous));
                        mode = (TangentMode) num;
                        this.SetTangentMode.Invoke(this.activePoint, mode);
                    }
                    if (flag4 && flag2)
                    {
                        this.SetTangentMode.Invoke(this.activePoint, TangentMode.Linear);
                        flag5 = true;
                    }
                    if (flag5)
                    {
                        this.RecordUndo.Invoke();
                        this.SetPointLTangent.Invoke(this.activePoint, vector4);
                        this.SetPointRTangent.Invoke(this.activePoint, vector5);
                        this.RefreshTangents(this.activePoint, GUIUtility.hotControl == this.k_RightTangentID);
                        this.Repaint.Invoke();
                    }
                }
            }
        }

        public int activeEdge
        {
            get
            {
                return this.m_ActiveEdge;
            }
            set
            {
                this.m_ActiveEdge = value;
            }
        }

        public int activePoint { get; set; }

        public bool delayedReset
        {
            set
            {
                this.m_DelayedReset = value;
            }
        }

        private static Color handleFillColor
        {
            [CompilerGenerated]
            get
            {
                return <handleFillColor>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                <handleFillColor>k__BackingField = value;
            }
        }

        private static Quaternion handleMatrixrotation
        {
            get
            {
                return Quaternion.LookRotation((Vector3) Handles.matrix.GetColumn(2), (Vector3) Handles.matrix.GetColumn(1));
            }
        }

        private static Color handleOutlineColor
        {
            [CompilerGenerated]
            get
            {
                return <handleOutlineColor>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                <handleOutlineColor>k__BackingField = value;
            }
        }

        public bool inEditMode { get; set; }

        public Texture2D lineTexture { get; set; }

        public HashSet<int> selectedPoints
        {
            get
            {
                return this.m_Selection.indices;
            }
        }

        public delegate float DistanceToControl(Vector3 pos, Quaternion rotation, float handleSize);

        internal enum SelectionType
        {
            Normal,
            Additive,
            Subtractive
        }

        internal enum TangentMode
        {
            Linear,
            Continuous,
            Broken
        }

        internal enum Tool
        {
            Edit,
            Create,
            Break
        }
    }
}

