namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.U2D.Interface;

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
        private static Func<ShapeEditor, float> <>f__am$cache18;
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
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private IEvent <currentEvent>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IEventSystem <eventSystem>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IGUIUtility <guiUtility>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static Color <handleFillColor>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
        private int m_MouseClosestEdge;
        private float m_MouseClosestEdgeDist;
        private Vector2 m_MousePositionLastMouseDown;
        private bool m_NewPointDragFinished;
        private int m_NewPointIndex;
        private ShapeEditorRectSelectionTool m_RectSelectionTool;
        private ShapeEditorSelection m_Selection;
        private HashSet<ShapeEditor> m_ShapeEditorListeners;
        private int m_ShapeEditorRegisteredTo;
        private int m_ShapeEditorUpdateDone;
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

        public ShapeEditor(IGUIUtility gu, IEventSystem es)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<int, Vector3>(ShapeEditor.<GetPointPosition>m__0);
            }
            this.GetPointPosition = <>f__am$cache0;
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Action<int, Vector3>(ShapeEditor.<SetPointPosition>m__1);
            }
            this.SetPointPosition = <>f__am$cache1;
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<int, Vector3>(ShapeEditor.<GetPointLTangent>m__2);
            }
            this.GetPointLTangent = <>f__am$cache2;
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = new Action<int, Vector3>(ShapeEditor.<SetPointLTangent>m__3);
            }
            this.SetPointLTangent = <>f__am$cache3;
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = new Func<int, Vector3>(ShapeEditor.<GetPointRTangent>m__4);
            }
            this.GetPointRTangent = <>f__am$cache4;
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = new Action<int, Vector3>(ShapeEditor.<SetPointRTangent>m__5);
            }
            this.SetPointRTangent = <>f__am$cache5;
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = new Func<int, TangentMode>(ShapeEditor.<GetTangentMode>m__6);
            }
            this.GetTangentMode = <>f__am$cache6;
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = new Action<int, TangentMode>(ShapeEditor.<SetTangentMode>m__7);
            }
            this.SetTangentMode = <>f__am$cache7;
            if (<>f__am$cache8 == null)
            {
                <>f__am$cache8 = new Action<int, Vector3>(ShapeEditor.<InsertPointAt>m__8);
            }
            this.InsertPointAt = <>f__am$cache8;
            if (<>f__am$cache9 == null)
            {
                <>f__am$cache9 = new Action<int>(ShapeEditor.<RemovePointAt>m__9);
            }
            this.RemovePointAt = <>f__am$cache9;
            if (<>f__am$cacheA == null)
            {
                <>f__am$cacheA = new Func<int>(ShapeEditor.<GetPointsCount>m__A);
            }
            this.GetPointsCount = <>f__am$cacheA;
            if (<>f__am$cacheB == null)
            {
                <>f__am$cacheB = new Func<Vector2, Vector3>(ShapeEditor.<ScreenToLocal>m__B);
            }
            this.ScreenToLocal = <>f__am$cacheB;
            if (<>f__am$cacheC == null)
            {
                <>f__am$cacheC = new Func<Vector3, Vector2>(ShapeEditor.<LocalToScreen>m__C);
            }
            this.LocalToScreen = <>f__am$cacheC;
            if (<>f__am$cacheD == null)
            {
                <>f__am$cacheD = new Func<Matrix4x4>(ShapeEditor.<LocalToWorldMatrix>m__D);
            }
            this.LocalToWorldMatrix = <>f__am$cacheD;
            if (<>f__am$cacheE == null)
            {
                <>f__am$cacheE = new Func<DistanceToControl>(ShapeEditor.<DistanceToRectangle>m__E);
            }
            this.DistanceToRectangle = <>f__am$cacheE;
            if (<>f__am$cacheF == null)
            {
                <>f__am$cacheF = new Func<DistanceToControl>(ShapeEditor.<DistanceToDiamond>m__F);
            }
            this.DistanceToDiamond = <>f__am$cacheF;
            if (<>f__am$cache10 == null)
            {
                <>f__am$cache10 = new Func<DistanceToControl>(ShapeEditor.<DistanceToCircle>m__10);
            }
            this.DistanceToCircle = <>f__am$cache10;
            if (<>f__am$cache11 == null)
            {
                <>f__am$cache11 = new Action(ShapeEditor.<Repaint>m__11);
            }
            this.Repaint = <>f__am$cache11;
            if (<>f__am$cache12 == null)
            {
                <>f__am$cache12 = new Action(ShapeEditor.<RecordUndo>m__12);
            }
            this.RecordUndo = <>f__am$cache12;
            if (<>f__am$cache13 == null)
            {
                <>f__am$cache13 = new Func<Vector3, Vector3>(ShapeEditor.<Snap>m__13);
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
                <>f__am$cache16 = new Func<bool>(ShapeEditor.<OpenEnded>m__16);
            }
            this.OpenEnded = <>f__am$cache16;
            if (<>f__am$cache17 == null)
            {
                <>f__am$cache17 = new Func<float>(ShapeEditor.<GetHandleSize>m__17);
            }
            this.GetHandleSize = <>f__am$cache17;
            this.m_ActivePointOnLastMouseDown = -1;
            this.m_NewPointIndex = -1;
            this.m_ActiveEdge = -1;
            this.m_DelayedReset = false;
            this.m_ShapeEditorListeners = new HashSet<ShapeEditor>();
            this.m_MouseClosestEdge = -1;
            this.m_MouseClosestEdgeDist = float.MaxValue;
            this.m_ShapeEditorRegisteredTo = 0;
            this.m_ShapeEditorUpdateDone = 0;
            this.m_Selection = new ShapeEditorSelection(this);
            this.guiUtility = gu;
            this.eventSystem = es;
            this.k_CreatorID = this.guiUtility.GetPermanentControlID();
            this.k_EdgeID = this.guiUtility.GetPermanentControlID();
            this.k_RightTangentID = this.guiUtility.GetPermanentControlID();
            this.k_LeftTangentID = this.guiUtility.GetPermanentControlID();
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
        private static float <GetHandleSize>m__17() => 
            5f;

        [CompilerGenerated]
        private static Vector3 <GetPointLTangent>m__2(int i) => 
            Vector3.zero;

        [CompilerGenerated]
        private static Vector3 <GetPointPosition>m__0(int i) => 
            Vector3.zero;

        [CompilerGenerated]
        private static Vector3 <GetPointRTangent>m__4(int i) => 
            Vector3.zero;

        [CompilerGenerated]
        private static int <GetPointsCount>m__A() => 
            0;

        [CompilerGenerated]
        private static TangentMode <GetTangentMode>m__6(int i) => 
            TangentMode.Linear;

        [CompilerGenerated]
        private static void <InsertPointAt>m__8(int i, Vector3 p)
        {
        }

        [CompilerGenerated]
        private static Vector2 <LocalToScreen>m__C(Vector3 i) => 
            i;

        [CompilerGenerated]
        private static Matrix4x4 <LocalToWorldMatrix>m__D() => 
            Matrix4x4.identity;

        [CompilerGenerated]
        private static void <OnPointClick>m__15(int i)
        {
        }

        [CompilerGenerated]
        private static bool <OpenEnded>m__16() => 
            false;

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
        private static Vector3 <ScreenToLocal>m__B(Vector2 i) => 
            ((Vector3) i);

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
        private static Vector3 <Snap>m__13(Vector3 i) => 
            i;

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
                Handles.SetDiscSectionPoints(dest, position, lhs, from, 360f, size);
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

        private void ClearSelectedPoints()
        {
            this.selectedPoints.Clear();
            this.activePoint = -1;
        }

        public void CycleTangentMode()
        {
            TangentMode current = this.GetTangentMode(this.activePoint);
            TangentMode nextTangentMode = GetNextTangentMode(current);
            this.SetTangentMode(this.activePoint, nextTangentMode);
            this.RefreshTangentsAfterModeChange(this.activePoint, current, nextTangentMode);
        }

        private void DelayedResetIfNecessary()
        {
            if (this.m_DelayedReset)
            {
                this.guiUtility.hotControl = 0;
                this.guiUtility.keyboardControl = 0;
                this.m_Selection.Clear();
                this.activePoint = -1;
                this.m_DelayedReset = false;
            }
        }

        private void DeleteSelections()
        {
            foreach (ShapeEditor editor in this.m_ShapeEditorListeners)
            {
                editor.m_Selection.DeleteSelection();
            }
            this.m_Selection.DeleteSelection();
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

        private static float DistanceToCircleInternal(Vector3 position, Quaternion rotation, float size) => 
            HandleUtility.DistanceToCircle(position, size);

        private static Vector3 DoSlider(int id, Vector3 position, Vector3 slide1, Vector3 slide2, float s, Handles.CapFunction cap) => 
            Slider2D.Do(id, position, Vector3.zero, Vector3.Cross(slide1, slide2), slide1, slide2, s, cap, Vector2.zero, false);

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

        private bool EdgeDragModifiersActive() => 
            ((this.currentEvent.modifiers == EventModifiers.Control) || (this.currentEvent.modifiers == EventModifiers.Command));

        public void Edges()
        {
            float maxValue = float.MaxValue;
            if (this.m_ShapeEditorListeners.Count > 0)
            {
                if (<>f__am$cache18 == null)
                {
                    <>f__am$cache18 = se => se.GetMouseClosestEdgeDistance();
                }
                maxValue = Enumerable.Select<ShapeEditor, float>(this.m_ShapeEditorListeners, <>f__am$cache18).Max();
            }
            int total = this.GetPointsCount();
            int index = 0;
            int num4 = NextIndex(index, total);
            int num5 = !this.OpenEnded() ? total : (total - 1);
            for (int i = 0; i < num5; i++)
            {
                Vector3 startPosition = this.GetPointPosition(index);
                Vector3 endPosition = this.GetPointPosition(num4);
                Vector3 startTangent = startPosition + this.GetPointRTangent(index);
                Vector3 endTangent = endPosition + this.GetPointLTangent(num4);
                Vector2 vector5 = this.LocalToScreen(startPosition);
                Vector2 vector6 = this.LocalToScreen(endPosition);
                Vector2 vector7 = this.LocalToScreen(startTangent);
                Vector2 vector8 = this.LocalToScreen(endTangent);
                float num7 = HandleUtility.DistancePointBezier((Vector3) this.currentEvent.mousePosition, (Vector3) vector5, (Vector3) vector6, (Vector3) vector7, (Vector3) vector8);
                Color color = (index != this.m_ActiveEdge) ? Color.white : Color.yellow;
                float width = ((index != this.m_ActiveEdge) && ((!this.EdgeDragModifiersActive() || (num7 >= 9f)) || (num7 >= maxValue))) ? 2f : 6f;
                Handles.DrawBezier(startPosition, endPosition, startTangent, endTangent, color, this.lineTexture, width);
                index = NextIndex(index, total);
                num4 = NextIndex(num4, total);
            }
            if (this.inEditMode && (maxValue > this.GetMouseClosestEdgeDistance()))
            {
                this.HandlePointInsertToEdge(this.m_MouseClosestEdge, this.m_MouseClosestEdgeDist);
                this.HandleEdgeDragging(this.m_MouseClosestEdge, this.m_MouseClosestEdgeDist);
            }
            if ((this.guiUtility.hotControl != this.k_CreatorID) && (this.m_NewPointIndex != -1))
            {
                this.m_NewPointDragFinished = true;
                this.guiUtility.keyboardControl = 0;
                this.m_NewPointIndex = -1;
            }
            if ((this.guiUtility.hotControl != this.k_EdgeID) && (this.m_ActiveEdge != -1))
            {
                this.m_ActiveEdge = -1;
            }
        }

        private int FindClosestPointIndex(Vector3 position)
        {
            float maxValue = float.MaxValue;
            int num2 = -1;
            for (int i = 0; i < this.GetPointsCount(); i++)
            {
                Vector3 vector2 = this.GetPointPosition(i) - position;
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
            Vector3 position = this.ScreenToLocal(this.currentEvent.mousePosition);
            return this.FindClosestPointIndex(position);
        }

        private void Framing()
        {
            if ((this.currentEvent.commandName == "FrameSelected") && (this.m_Selection.Count > 0))
            {
                EventType type = this.currentEvent.type;
                if (type != EventType.ExecuteCommand)
                {
                    if (type != EventType.ValidateCommand)
                    {
                        return;
                    }
                }
                else
                {
                    Bounds bounds = new Bounds(this.GetPointPosition(this.selectedPoints.First<int>()), Vector3.zero);
                    foreach (int num in this.selectedPoints)
                    {
                        bounds.Encapsulate(this.GetPointPosition(num));
                    }
                    this.Frame(bounds);
                }
                this.currentEvent.Use();
            }
        }

        private void FromAllZeroToTangents(int pointIndex)
        {
            Vector3 vector = this.GetPointPosition(pointIndex);
            int num = (pointIndex <= 0) ? (this.GetPointsCount() - 1) : (pointIndex - 1);
            Vector3 vector2 = (Vector3) ((this.GetPointPosition(num) - vector) * 0.33f);
            Vector3 vector3 = -vector2;
            Vector2 vector4 = this.LocalToScreen(vector) - this.LocalToScreen(vector + vector2);
            float magnitude = vector4.magnitude;
            Vector2 vector5 = this.LocalToScreen(vector) - this.LocalToScreen(vector + vector3);
            float num3 = vector5.magnitude;
            vector2 = (Vector3) (vector2 * Mathf.Min((float) (100f / magnitude), (float) 1f));
            vector3 = (Vector3) (vector3 * Mathf.Min((float) (100f / num3), (float) 1f));
            this.SetPointLTangent(pointIndex, vector2);
            this.SetPointRTangent(pointIndex, vector3);
        }

        private Handles.CapFunction GetCapForPoint(int index)
        {
            TangentMode mode = this.GetTangentMode(index);
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
            if (((TangentMode) this.GetTangentMode(index)) == TangentMode.Continuous)
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
            TangentMode mode = this.GetTangentMode(index);
            if (mode != TangentMode.Broken)
            {
                if (mode == TangentMode.Continuous)
                {
                    return this.DistanceToCircle();
                }
                if (mode == TangentMode.Linear)
                {
                    return this.DistanceToRectangle();
                }
            }
            else
            {
                return this.DistanceToDiamond();
            }
            return this.DistanceToRectangle();
        }

        private DistanceToControl GetDistanceFuncForTangent(int index)
        {
            if (((TangentMode) this.GetTangentMode(index)) == TangentMode.Continuous)
            {
                return this.DistanceToCircle();
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

        private float GetHandleSizeForPoint(int index) => 
            ((Camera.current == null) ? this.GetHandleSize() : (HandleUtility.GetHandleSize(this.GetPointPosition(index)) * 0.075f));

        private float GetMouseClosestEdgeDistance()
        {
            int total = this.GetPointsCount();
            if ((this.m_MouseClosestEdge == -1) && (total > 0))
            {
                this.m_MouseClosestEdgeDist = float.MaxValue;
                int index = 0;
                int num3 = NextIndex(index, total);
                int num4 = !this.OpenEnded() ? total : (total - 1);
                for (int i = 0; i < num4; i++)
                {
                    Vector3 vector = this.GetPointPosition(index);
                    Vector3 vector2 = this.GetPointPosition(num3);
                    Vector3 vector3 = vector + this.GetPointRTangent(index);
                    Vector3 vector4 = vector2 + this.GetPointLTangent(num3);
                    Vector2 vector5 = this.LocalToScreen(vector);
                    Vector2 vector6 = this.LocalToScreen(vector2);
                    Vector2 vector7 = this.LocalToScreen(vector3);
                    Vector2 vector8 = this.LocalToScreen(vector4);
                    float num6 = HandleUtility.DistancePointBezier((Vector3) this.eventSystem.current.mousePosition, (Vector3) vector5, (Vector3) vector6, (Vector3) vector7, (Vector3) vector8);
                    if (num6 < this.m_MouseClosestEdgeDist)
                    {
                        this.m_MouseClosestEdge = index;
                        this.m_MouseClosestEdgeDist = num6;
                    }
                    index = NextIndex(index, total);
                    num3 = NextIndex(num3, total);
                }
            }
            if ((this.guiUtility.hotControl == this.k_CreatorID) || (this.guiUtility.hotControl == this.k_EdgeID))
            {
                return float.MinValue;
            }
            return this.m_MouseClosestEdgeDist;
        }

        public static TangentMode GetNextTangentMode(TangentMode current) => 
            ((current + 1) % Enum.GetValues(typeof(TangentMode)).Length);

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
            int num2 = NextIndex(num, this.GetPointsCount());
            float t = index - num;
            return GetPoint(this.GetPointPosition(num), this.GetPointPosition(num2), this.GetPointRTangent(num), this.GetPointLTangent(num2), t);
        }

        private float GetTangentSizeForPoint(int index) => 
            (this.GetHandleSizeForPoint(index) * 0.8f);

        private void HandleEdgeDragging(int closestEdge, float closestEdgeDist)
        {
            bool flag = GUIUtility.hotControl == this.k_EdgeID;
            bool flag2 = this.MouseDistanceToPoint(this.FindClosestPointToMouse()) > 20f;
            bool flag3 = this.MouseDistanceToClosestTangent() > 20f;
            bool flag4 = flag2 && flag3;
            if (((((closestEdgeDist < 9f) && flag4) && !this.m_RectSelectionTool.isSelecting) && this.EdgeDragModifiersActive()) || flag)
            {
                switch (this.currentEvent.type)
                {
                    case EventType.MouseDown:
                        this.m_ActiveEdge = closestEdge;
                        this.m_EdgeDragStartP0 = this.GetPointPosition(this.m_ActiveEdge);
                        this.m_EdgeDragStartP1 = this.GetPointPosition(NextIndex(this.m_ActiveEdge, this.GetPointsCount()));
                        if (this.currentEvent.shift)
                        {
                            this.RecordUndo();
                            this.InsertPointAt(this.m_ActiveEdge + 1, this.m_EdgeDragStartP0);
                            this.InsertPointAt(this.m_ActiveEdge + 2, this.m_EdgeDragStartP1);
                            this.m_ActiveEdge++;
                        }
                        this.m_EdgeDragStartMousePosition = this.ScreenToLocal(this.currentEvent.mousePosition);
                        GUIUtility.hotControl = this.k_EdgeID;
                        this.currentEvent.Use();
                        break;

                    case EventType.MouseDrag:
                    {
                        this.RecordUndo();
                        Vector3 vector2 = this.ScreenToLocal(this.currentEvent.mousePosition) - this.m_EdgeDragStartMousePosition;
                        Vector3 vector3 = this.GetPointPosition(this.m_ActiveEdge);
                        Vector3 vector4 = this.m_EdgeDragStartP0 + vector2;
                        Vector3 vector5 = this.Snap(vector4) - vector3;
                        int activeEdge = this.m_ActiveEdge;
                        int num2 = NextIndex(this.m_ActiveEdge, this.GetPointsCount());
                        this.SetPointPosition(this.m_ActiveEdge, this.GetPointPosition(activeEdge) + vector5);
                        this.SetPointPosition(num2, this.GetPointPosition(num2) + vector5);
                        this.currentEvent.Use();
                        break;
                    }
                    case EventType.MouseUp:
                        this.m_ActiveEdge = -1;
                        GUIUtility.hotControl = 0;
                        this.currentEvent.Use();
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
            else if ((!this.currentEvent.control && !this.currentEvent.shift) && (this.m_ActivePointOnLastMouseDown == this.activePoint))
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
            if (((((closestEdgeDist < 9f) && flag4) && !this.m_RectSelectionTool.isSelecting) && (this.currentEvent.modifiers == EventModifiers.None)) || flag)
            {
                Vector3 position = !flag ? this.FindClosestPointOnEdge(closestEdge, this.ScreenToLocal(this.currentEvent.mousePosition), 100) : this.GetPointPosition(this.m_NewPointIndex);
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
                    this.RecordUndo();
                    this.m_NewPointIndex = NextIndex(closestEdge, this.GetPointsCount());
                    this.InsertPointAt(this.m_NewPointIndex, vector2);
                    this.SelectPoint(this.m_NewPointIndex, SelectionType.Normal);
                }
                else if (EditorGUI.EndChangeCheck())
                {
                    this.RecordUndo();
                    vector2 = this.Snap(vector2);
                    this.MoveSelections(vector2 - position);
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
            Vector3 vector = this.GetPointLTangent(this.activePoint);
            Vector3 vector2 = this.GetPointRTangent(this.activePoint);
            if ((vector.sqrMagnitude == 0f) && (vector2.sqrMagnitude == 0f))
            {
                return float.MaxValue;
            }
            Vector3 vector3 = this.GetPointPosition(this.activePoint);
            float tangentSizeForPoint = this.GetTangentSizeForPoint(this.activePoint);
            return Mathf.Min(HandleUtility.DistanceToRectangle(vector3 + vector, Quaternion.identity, tangentSizeForPoint), HandleUtility.DistanceToRectangle(vector3 + vector2, Quaternion.identity, tangentSizeForPoint));
        }

        private float MouseDistanceToPoint(int index)
        {
            TangentMode mode = this.GetTangentMode(index);
            if (mode != TangentMode.Broken)
            {
                if (mode == TangentMode.Linear)
                {
                    return HandleUtility.DistanceToRectangle(this.GetPointPosition(index), Quaternion.identity, this.GetHandleSizeForPoint(index));
                }
                if (mode == TangentMode.Continuous)
                {
                    return HandleUtility.DistanceToCircle(this.GetPointPosition(index), this.GetHandleSizeForPoint(index));
                }
            }
            else
            {
                return HandleUtility.DistanceToDiamond(this.GetPointPosition(index), Quaternion.identity, this.GetHandleSizeForPoint(index));
            }
            return float.MaxValue;
        }

        private void MoveSelections(Vector2 distance)
        {
            foreach (ShapeEditor editor in this.m_ShapeEditorListeners)
            {
                editor.m_Selection.MoveSelection((Vector3) distance);
            }
            this.m_Selection.MoveSelection((Vector3) distance);
        }

        private static int NextIndex(int index, int total) => 
            mod(index + 1, total);

        public void OnDisable()
        {
            this.m_RectSelectionTool.RectSelect -= new Action<Rect, SelectionType>(this.SelectPointsInRect);
            this.m_RectSelectionTool.ClearSelection -= new Action(this.ClearSelectedPoints);
            this.m_RectSelectionTool = null;
        }

        public void OnGUI()
        {
            this.DelayedResetIfNecessary();
            this.currentEvent = this.eventSystem.current;
            if (this.currentEvent.type == EventType.MouseDown)
            {
                this.StoreMouseDownState();
            }
            Color color = Handles.color;
            Matrix4x4 matrix = Handles.matrix;
            Handles.matrix = this.LocalToWorldMatrix();
            this.Edges();
            if (this.inEditMode)
            {
                this.Framing();
                this.Tangents();
                this.Points();
            }
            Handles.color = color;
            Handles.matrix = matrix;
            this.OnShapeEditorUpdateDone();
            foreach (ShapeEditor editor in this.m_ShapeEditorListeners)
            {
                editor.OnShapeEditorUpdateDone();
            }
        }

        private void OnShapeEditorUpdateDone()
        {
            this.m_ShapeEditorUpdateDone++;
            if (this.m_ShapeEditorUpdateDone >= this.m_ShapeEditorRegisteredTo)
            {
                this.m_ShapeEditorUpdateDone = 0;
                this.m_MouseClosestEdge = -1;
                this.m_MouseClosestEdgeDist = float.MaxValue;
            }
        }

        public void Points()
        {
            bool flag = ((Event.current.type == EventType.ExecuteCommand) || (Event.current.type == EventType.ValidateCommand)) && ((Event.current.commandName == "SoftDelete") || (Event.current.commandName == "Delete"));
            for (int i = 0; i < this.GetPointsCount(); i++)
            {
                if (i != this.m_NewPointIndex)
                {
                    Vector3 position = this.GetPointPosition(i);
                    int controlID = this.guiUtility.GetControlID(0x14e9, FocusType.Keyboard);
                    bool flag2 = this.m_Selection.Contains(i);
                    bool flag3 = this.currentEvent.GetTypeForControl(controlID) == EventType.MouseDown;
                    bool flag4 = this.currentEvent.GetTypeForControl(controlID) == EventType.MouseUp;
                    EditorGUI.BeginChangeCheck();
                    handleOutlineColor = this.GetOutlineColorForPoint(i, controlID);
                    handleFillColor = this.GetFillColorForPoint(i, controlID);
                    Vector3 vector2 = position;
                    int hotControl = this.guiUtility.hotControl;
                    if (!this.currentEvent.alt || (this.guiUtility.hotControl == controlID))
                    {
                        vector2 = DoSlider(controlID, position, Vector3.up, Vector3.right, this.GetHandleSizeForPoint(i), this.GetCapForPoint(i));
                    }
                    else if (this.currentEvent.type == EventType.Repaint)
                    {
                        this.GetCapForPoint(i)(controlID, position, Quaternion.LookRotation(Vector3.forward, Vector3.up), this.GetHandleSizeForPoint(i), this.currentEvent.type);
                    }
                    int num4 = this.guiUtility.hotControl;
                    if (((flag4 && (hotControl == controlID)) && ((num4 == 0) && (this.currentEvent.mousePosition == this.m_MousePositionLastMouseDown))) && !this.currentEvent.shift)
                    {
                        this.HandlePointClick(i);
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        this.RecordUndo();
                        vector2 = this.Snap(vector2);
                        this.MoveSelections(vector2 - position);
                    }
                    if (((this.guiUtility.hotControl == controlID) && !flag2) && flag3)
                    {
                        this.SelectPoint(i, !this.currentEvent.shift ? SelectionType.Normal : SelectionType.Additive);
                        this.Repaint();
                    }
                    if ((this.m_NewPointDragFinished && (this.activePoint == i)) && (controlID != -1))
                    {
                        this.guiUtility.keyboardControl = controlID;
                        this.m_NewPointDragFinished = false;
                    }
                }
            }
            if (flag)
            {
                if (this.currentEvent.type == EventType.ValidateCommand)
                {
                    this.currentEvent.Use();
                }
                else if (this.currentEvent.type == EventType.ExecuteCommand)
                {
                    this.RecordUndo();
                    this.DeleteSelections();
                    this.currentEvent.Use();
                }
            }
        }

        private static int PreviousIndex(int index, int total) => 
            mod(index - 1, total);

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
            TangentMode mode = this.GetTangentMode(index);
            Vector3 vector = this.GetPointLTangent(index);
            Vector3 vector2 = this.GetPointRTangent(index);
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
            this.SetPointLTangent(this.activePoint, vector);
            this.SetPointRTangent(this.activePoint, vector2);
        }

        public void RefreshTangentsAfterModeChange(int pointIndex, TangentMode oldMode, TangentMode newMode)
        {
            if ((oldMode != TangentMode.Linear) && (newMode == TangentMode.Linear))
            {
                this.SetPointLTangent(pointIndex, Vector3.zero);
                this.SetPointRTangent(pointIndex, Vector3.zero);
            }
            if (newMode == TangentMode.Continuous)
            {
                if (oldMode == TangentMode.Broken)
                {
                    this.SetPointRTangent(pointIndex, (Vector3) (this.GetPointLTangent(pointIndex) * -1f));
                }
                if (oldMode == TangentMode.Linear)
                {
                    this.FromAllZeroToTangents(pointIndex);
                }
            }
        }

        public void RegisterToShapeEditor(ShapeEditor se)
        {
            this.m_ShapeEditorRegisteredTo++;
            se.m_ShapeEditorListeners.Add(this);
        }

        private void SelectPoint(int index, SelectionType st)
        {
            if (st == SelectionType.Normal)
            {
                foreach (ShapeEditor editor in this.m_ShapeEditorListeners)
                {
                    editor.ClearSelectedPoints();
                }
            }
            this.m_Selection.SelectPoint(index, st);
        }

        private void SelectPointsInRect(Rect r, SelectionType st)
        {
            Rect rect = EditorGUIExt.FromToRect(this.ScreenToLocal(r.min), this.ScreenToLocal(r.max));
            this.m_Selection.RectSelect(rect, st);
        }

        public void SetRectSelectionTool(ShapeEditorRectSelectionTool sers)
        {
            if (this.m_RectSelectionTool != null)
            {
                this.m_RectSelectionTool.RectSelect -= new Action<Rect, SelectionType>(this.SelectPointsInRect);
                this.m_RectSelectionTool.ClearSelection -= new Action(this.ClearSelectedPoints);
            }
            this.m_RectSelectionTool = sers;
            this.m_RectSelectionTool.RectSelect += new Action<Rect, SelectionType>(this.SelectPointsInRect);
            this.m_RectSelectionTool.ClearSelection += new Action(this.ClearSelectedPoints);
        }

        private void StoreMouseDownState()
        {
            this.m_MousePositionLastMouseDown = this.currentEvent.mousePosition;
            this.m_ActivePointOnLastMouseDown = this.activePoint;
        }

        public void Tangents()
        {
            if (((this.activePoint >= 0) && (this.m_Selection.Count <= 1)) && (((TangentMode) this.GetTangentMode(this.activePoint)) != TangentMode.Linear))
            {
                IEvent current = this.eventSystem.current;
                Vector3 vector = this.GetPointPosition(this.activePoint);
                Vector3 vector2 = this.GetPointLTangent(this.activePoint);
                Vector3 vector3 = this.GetPointRTangent(this.activePoint);
                bool flag = (this.guiUtility.hotControl == this.k_RightTangentID) || (this.guiUtility.hotControl == this.k_LeftTangentID);
                bool flag2 = (vector2.sqrMagnitude == 0f) && (vector3.sqrMagnitude == 0f);
                if (flag || !flag2)
                {
                    TangentMode mode = this.GetTangentMode(this.activePoint);
                    bool flag3 = (current.GetTypeForControl(this.k_RightTangentID) == EventType.MouseDown) || (current.GetTypeForControl(this.k_LeftTangentID) == EventType.MouseDown);
                    bool flag4 = (current.GetTypeForControl(this.k_RightTangentID) == EventType.MouseUp) || (current.GetTypeForControl(this.k_LeftTangentID) == EventType.MouseUp);
                    Vector3 vector4 = this.DoTangent(vector, vector + vector2, this.k_LeftTangentID, this.activePoint, k_TangentColor);
                    Vector3 vector5 = this.DoTangent(vector, vector + vector3, this.k_RightTangentID, this.activePoint, (((TangentMode) this.GetTangentMode(this.activePoint)) != TangentMode.Broken) ? k_TangentColor : k_TangentColorAlternative);
                    bool flag5 = (vector4 != vector2) || (vector5 != vector3);
                    flag2 = (vector4.sqrMagnitude == 0f) && (vector5.sqrMagnitude == 0f);
                    if (flag && flag3)
                    {
                        int num = (int) ((mode + 1) % (TangentMode.Broken | TangentMode.Continuous));
                        mode = (TangentMode) num;
                        this.SetTangentMode(this.activePoint, mode);
                    }
                    if (flag4 && flag2)
                    {
                        this.SetTangentMode(this.activePoint, TangentMode.Linear);
                        flag5 = true;
                    }
                    if (flag5)
                    {
                        this.RecordUndo();
                        this.SetPointLTangent(this.activePoint, vector4);
                        this.SetPointRTangent(this.activePoint, vector5);
                        this.RefreshTangents(this.activePoint, this.guiUtility.hotControl == this.k_RightTangentID);
                        this.Repaint();
                    }
                }
            }
        }

        public void UnregisterFromShapeEditor(ShapeEditor se)
        {
            this.m_ShapeEditorRegisteredTo--;
            se.m_ShapeEditorListeners.Remove(this);
        }

        public int activeEdge
        {
            get => 
                this.m_ActiveEdge;
            set
            {
                this.m_ActiveEdge = value;
            }
        }

        public int activePoint { get; set; }

        private IEvent currentEvent { get; set; }

        public bool delayedReset
        {
            set
            {
                this.m_DelayedReset = value;
            }
        }

        private IEventSystem eventSystem { get; set; }

        private IGUIUtility guiUtility { get; set; }

        private static Color handleFillColor
        {
            [CompilerGenerated]
            get => 
                <handleFillColor>k__BackingField;
            [CompilerGenerated]
            set
            {
                <handleFillColor>k__BackingField = value;
            }
        }

        private static Quaternion handleMatrixrotation =>
            Quaternion.LookRotation((Vector3) Handles.matrix.GetColumn(2), (Vector3) Handles.matrix.GetColumn(1));

        private static Color handleOutlineColor
        {
            [CompilerGenerated]
            get => 
                <handleOutlineColor>k__BackingField;
            [CompilerGenerated]
            set
            {
                <handleOutlineColor>k__BackingField = value;
            }
        }

        public bool inEditMode { get; set; }

        public Texture2D lineTexture { get; set; }

        public HashSet<int> selectedPoints =>
            this.m_Selection.indices;

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

