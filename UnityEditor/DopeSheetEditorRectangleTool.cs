namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class DopeSheetEditorRectangleTool : RectangleTool
    {
        private static Rect g_EmptyRect = new Rect(0f, 0f, 0f, 0f);
        private const float kDefaultFrameRate = 60f;
        private const int kHLabelMarginHorizontal = 8;
        private const int kHLabelMarginVertical = 1;
        private const int kScaleLeftMarginHorizontal = 0;
        private const float kScaleLeftMarginVertical = 4f;
        private const int kScaleLeftWidth = 0x11;
        private const int kScaleRightMarginHorizontal = 0;
        private const float kScaleRightMarginVertical = 4f;
        private const int kScaleRightWidth = 0x11;
        private DopeSheetEditor m_DopeSheetEditor;
        private bool m_IncrementalUpdate;
        private bool m_IsDragging;
        private ToolLayout m_Layout;
        private Vector2 m_MouseOffset;
        private Vector2 m_Pivot;
        private Vector2 m_Previous;
        private bool m_RippleTime;
        private float m_RippleTimeEnd;
        private float m_RippleTimeStart;
        private AreaManipulator[] m_SelectionBoxes;
        private AreaManipulator m_SelectionScaleLeft;
        private AreaManipulator m_SelectionScaleRight;
        private AnimationWindowState m_State;

        private ToolLayout CalculateLayout()
        {
            ToolLayout layout = new ToolLayout();
            Bounds selectionBounds = this.selectionBounds;
            bool flag = !Mathf.Approximately(selectionBounds.size.x, 0f);
            float x = base.TimeToPixel(selectionBounds.min.x);
            float num2 = base.TimeToPixel(selectionBounds.max.x);
            float y = 0f;
            float num4 = 0f;
            bool flag2 = true;
            float num5 = 0f;
            List<DopeLine> dopelines = this.m_State.dopelines;
            for (int i = 0; i < dopelines.Count; i++)
            {
                DopeLine line = dopelines[i];
                float num7 = !line.tallMode ? 16f : 32f;
                if (!line.isMasterDopeline)
                {
                    int count = line.keys.Count;
                    for (int j = 0; j < count; j++)
                    {
                        AnimationWindowKeyframe keyframe = line.keys[j];
                        if (this.m_State.KeyIsSelected(keyframe))
                        {
                            if (flag2)
                            {
                                y = num5;
                                flag2 = false;
                            }
                            num4 = num5 + num7;
                            break;
                        }
                    }
                }
                num5 += num7;
            }
            layout.summaryRect = new Rect(x, 0f, num2 - x, 16f);
            layout.selectionRect = new Rect(x, y, num2 - x, num4 - y);
            if (flag)
            {
                layout.scaleLeftRect = new Rect(layout.selectionRect.xMin - 17f, layout.selectionRect.yMin + 4f, 17f, layout.selectionRect.height - 8f);
                layout.scaleRightRect = new Rect(layout.selectionRect.xMax, layout.selectionRect.yMin + 4f, 17f, layout.selectionRect.height - 8f);
            }
            else
            {
                layout.scaleLeftRect = g_EmptyRect;
                layout.scaleRightRect = g_EmptyRect;
            }
            if (flag)
            {
                layout.leftLabelAnchor = new Vector2(layout.summaryRect.xMin - 8f, base.contentRect.yMin + 1f);
                layout.rightLabelAnchor = new Vector2(layout.summaryRect.xMax + 8f, base.contentRect.yMin + 1f);
                return layout;
            }
            layout.leftLabelAnchor = layout.rightLabelAnchor = new Vector2(layout.summaryRect.center.x + 8f, base.contentRect.yMin + 1f);
            return layout;
        }

        private void DrawLabels()
        {
            if (this.isDragging)
            {
                if (!Mathf.Approximately(this.selectionBounds.size.x, 0f))
                {
                    GUIContent content = new GUIContent($"{this.m_DopeSheetEditor.FormatTime(this.selectionBounds.min.x, this.m_State.frameRate, this.m_State.timeFormat)}");
                    GUIContent content2 = new GUIContent($"{this.m_DopeSheetEditor.FormatTime(this.selectionBounds.max.x, this.m_State.frameRate, this.m_State.timeFormat)}");
                    Vector2 vector4 = base.styles.dragLabel.CalcSize(content);
                    Vector2 vector5 = base.styles.dragLabel.CalcSize(content2);
                    EditorGUI.DoDropShadowLabel(new Rect(this.m_Layout.leftLabelAnchor.x - vector4.x, this.m_Layout.leftLabelAnchor.y, vector4.x, vector4.y), content, base.styles.dragLabel, 0.3f);
                    EditorGUI.DoDropShadowLabel(new Rect(this.m_Layout.rightLabelAnchor.x, this.m_Layout.rightLabelAnchor.y, vector5.x, vector5.y), content2, base.styles.dragLabel, 0.3f);
                }
                else
                {
                    GUIContent content3 = new GUIContent($"{this.m_DopeSheetEditor.FormatTime(this.selectionBounds.center.x, this.m_State.frameRate, this.m_State.timeFormat)}");
                    Vector2 vector7 = base.styles.dragLabel.CalcSize(content3);
                    EditorGUI.DoDropShadowLabel(new Rect(this.m_Layout.leftLabelAnchor.x, this.m_Layout.leftLabelAnchor.y, vector7.x, vector7.y), content3, base.styles.dragLabel, 0.3f);
                }
            }
        }

        public void HandleEvents()
        {
            base.HandleClutchKeys();
            this.m_SelectionScaleLeft.HandleEvents();
            this.m_SelectionScaleRight.HandleEvents();
            this.m_SelectionBoxes[0].HandleEvents();
            this.m_SelectionBoxes[1].HandleEvents();
        }

        public override void Initialize(TimeArea timeArea)
        {
            base.Initialize(timeArea);
            this.m_DopeSheetEditor = timeArea as DopeSheetEditor;
            this.m_State = this.m_DopeSheetEditor.state;
            if (this.m_SelectionBoxes == null)
            {
                this.m_SelectionBoxes = new AreaManipulator[2];
                for (int i = 0; i < 2; i++)
                {
                    this.m_SelectionBoxes[i] = new AreaManipulator(base.styles.rectangleToolSelection, MouseCursor.MoveArrow);
                    AreaManipulator manipulator1 = this.m_SelectionBoxes[i];
                    manipulator1.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate) Delegate.Combine(manipulator1.onStartDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                        if ((!(evt.shift || EditorGUI.actionKey) && this.hasSelection) && manipulator.rect.Contains(evt.mousePosition))
                        {
                            this.OnStartMove(new Vector2(base.PixelToTime(evt.mousePosition.x, this.frameRate), 0f), base.rippleTimeClutch);
                            return true;
                        }
                        return false;
                    });
                    AreaManipulator manipulator2 = this.m_SelectionBoxes[i];
                    manipulator2.onDrag = (AnimationWindowManipulator.OnDragDelegate) Delegate.Combine(manipulator2.onDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                        this.OnMove(new Vector2(base.PixelToTime(evt.mousePosition.x, this.frameRate), 0f));
                        return true;
                    });
                    AreaManipulator manipulator3 = this.m_SelectionBoxes[i];
                    manipulator3.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate) Delegate.Combine(manipulator3.onEndDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                        this.OnEndMove();
                        return true;
                    });
                }
            }
            if (this.m_SelectionScaleLeft == null)
            {
                this.m_SelectionScaleLeft = new AreaManipulator(base.styles.dopesheetScaleLeft, MouseCursor.ResizeHorizontal);
                this.m_SelectionScaleLeft.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate) Delegate.Combine(this.m_SelectionScaleLeft.onStartDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    if (this.hasSelection && manipulator.rect.Contains(evt.mousePosition))
                    {
                        this.OnStartScale(RectangleTool.ToolCoord.Right, RectangleTool.ToolCoord.Left, new Vector2(base.PixelToTime(evt.mousePosition.x, this.frameRate), 0f), base.rippleTimeClutch);
                        return true;
                    }
                    return false;
                });
                this.m_SelectionScaleLeft.onDrag = (AnimationWindowManipulator.OnDragDelegate) Delegate.Combine(this.m_SelectionScaleLeft.onDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    this.OnScaleTime(base.PixelToTime(evt.mousePosition.x, this.frameRate));
                    return true;
                });
                this.m_SelectionScaleLeft.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate) Delegate.Combine(this.m_SelectionScaleLeft.onEndDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    this.OnEndScale();
                    return true;
                });
            }
            if (this.m_SelectionScaleRight == null)
            {
                this.m_SelectionScaleRight = new AreaManipulator(base.styles.dopesheetScaleRight, MouseCursor.ResizeHorizontal);
                this.m_SelectionScaleRight.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate) Delegate.Combine(this.m_SelectionScaleRight.onStartDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    if (this.hasSelection && manipulator.rect.Contains(evt.mousePosition))
                    {
                        this.OnStartScale(RectangleTool.ToolCoord.Left, RectangleTool.ToolCoord.Right, new Vector2(base.PixelToTime(evt.mousePosition.x, this.frameRate), 0f), base.rippleTimeClutch);
                        return true;
                    }
                    return false;
                });
                this.m_SelectionScaleRight.onDrag = (AnimationWindowManipulator.OnDragDelegate) Delegate.Combine(this.m_SelectionScaleRight.onDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    this.OnScaleTime(base.PixelToTime(evt.mousePosition.x, this.frameRate));
                    return true;
                });
                this.m_SelectionScaleRight.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate) Delegate.Combine(this.m_SelectionScaleRight.onEndDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    this.OnEndScale();
                    return true;
                });
            }
        }

        internal void OnEndMove()
        {
            this.m_State.EndLiveEdit();
            this.m_IsDragging = false;
        }

        private void OnEndScale()
        {
            this.m_State.EndLiveEdit();
            this.m_IsDragging = false;
        }

        public void OnGUI()
        {
            if (this.hasSelection && (Event.current.type == EventType.Repaint))
            {
                this.m_Layout = this.CalculateLayout();
                this.m_SelectionBoxes[0].OnGUI(this.m_Layout.summaryRect);
                this.m_SelectionBoxes[1].OnGUI(this.m_Layout.selectionRect);
                this.m_SelectionScaleLeft.OnGUI(this.m_Layout.scaleLeftRect);
                this.m_SelectionScaleRight.OnGUI(this.m_Layout.scaleRightRect);
                this.DrawLabels();
            }
        }

        internal void OnMove(Vector2 position)
        {
            Vector2 vector = position - this.m_Previous;
            Matrix4x4 identity = Matrix4x4.identity;
            identity.SetTRS(new Vector3(vector.x, vector.y, 0f), Quaternion.identity, Vector3.one);
            this.TransformKeys(identity, false, false);
        }

        private void OnScaleTime(float time)
        {
            Matrix4x4 matrixx;
            bool flag;
            if (base.CalculateScaleTimeMatrix(this.m_Previous.x, time, this.m_MouseOffset.x, this.m_Pivot.x, this.frameRate, out matrixx, out flag))
            {
                this.TransformKeys(matrixx, flag, false);
            }
        }

        private void OnScaleValue(float val)
        {
            Matrix4x4 matrixx;
            bool flag;
            if (base.CalculateScaleValueMatrix(this.m_Previous.y, val, this.m_MouseOffset.y, this.m_Pivot.y, out matrixx, out flag))
            {
                this.TransformKeys(matrixx, false, flag);
            }
        }

        internal void OnStartMove(Vector2 position, bool rippleTime)
        {
            Bounds selectionBounds = this.selectionBounds;
            this.m_IsDragging = true;
            this.m_Previous = position;
            this.m_RippleTime = rippleTime;
            this.m_RippleTimeStart = selectionBounds.min.x;
            this.m_RippleTimeEnd = selectionBounds.max.x;
            this.m_State.StartLiveEdit();
        }

        private void OnStartScale(RectangleTool.ToolCoord pivotCoord, RectangleTool.ToolCoord pickedCoord, Vector2 mousePos, bool rippleTime)
        {
            Bounds selectionBounds = this.selectionBounds;
            this.m_IsDragging = true;
            this.m_Pivot = base.ToolCoordToPosition(pivotCoord, selectionBounds);
            this.m_Previous = base.ToolCoordToPosition(pickedCoord, selectionBounds);
            this.m_MouseOffset = mousePos - this.m_Previous;
            this.m_RippleTime = rippleTime;
            this.m_RippleTimeStart = selectionBounds.min.x;
            this.m_RippleTimeEnd = selectionBounds.max.x;
            this.m_State.StartLiveEdit();
        }

        private void TransformKeys(Matrix4x4 matrix, bool flipX, bool flipY)
        {
            if (this.m_RippleTime)
            {
                this.m_State.TransformRippleKeys(matrix, this.m_RippleTimeStart, this.m_RippleTimeEnd, flipX, true);
            }
            else
            {
                this.m_State.TransformSelectedKeys(matrix, flipX, flipY, true);
            }
        }

        private float frameRate =>
            this.m_State.frameRate;

        private bool hasSelection =>
            (this.m_State.selectedKeys.Count > 0);

        private bool isDragging =>
            (this.m_IsDragging || this.m_DopeSheetEditor.isDragging);

        private Bounds selectionBounds =>
            this.m_State.selectionBounds;

        [StructLayout(LayoutKind.Sequential)]
        private struct ToolLayout
        {
            public Rect summaryRect;
            public Rect selectionRect;
            public Rect hBarRect;
            public Rect hBarOverlayRect;
            public Rect hBarLeftRect;
            public Rect hBarRightRect;
            public bool displayHScale;
            public Rect vBarRect;
            public Rect vBarOverlayRect;
            public Rect vBarBottomRect;
            public Rect vBarTopRect;
            public bool displayVScale;
            public Rect selectionLeftRect;
            public Rect selectionTopRect;
            public Rect underlayTopRect;
            public Rect underlayLeftRect;
            public Rect scaleLeftRect;
            public Rect scaleRightRect;
            public Rect scaleTopRect;
            public Rect scaleBottomRect;
            public Vector2 leftLabelAnchor;
            public Vector2 rightLabelAnchor;
        }
    }
}

