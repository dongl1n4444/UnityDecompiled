namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class CurveEditorRectangleTool : RectangleTool
    {
        private static Rect g_EmptyRect = new Rect(0f, 0f, 0f, 0f);
        private const int kHBarHeight = 13;
        private const int kHBarLeftHeight = 13;
        private const int kHBarLeftWidth = 15;
        private const int kHBarMinWidth = 14;
        private const int kHBarRightHeight = 13;
        private const int kHBarRightWidth = 15;
        private const int kHLabelMarginHorizontal = 3;
        private const int kHLabelMarginVertical = 1;
        private const int kScaleBottomHeight = 0x11;
        private const int kScaleBottomMarginVertical = 0;
        private const float kScaleBottomRatio = 0.8f;
        private const int kScaleLeftMarginHorizontal = 0;
        private const float kScaleLeftRatio = 0.8f;
        private const int kScaleLeftWidth = 0x11;
        private const int kScaleRightMarginHorizontal = 0;
        private const float kScaleRightRatio = 0.8f;
        private const int kScaleRightWidth = 0x11;
        private const int kScaleTopHeight = 0x11;
        private const int kScaleTopMarginVertical = 0;
        private const float kScaleTopRatio = 0.8f;
        private const int kVBarBottomHeight = 15;
        private const int kVBarBottomWidth = 13;
        private const int kVBarMinHeight = 15;
        private const int kVBarTopHeight = 15;
        private const int kVBarTopWidth = 13;
        private const int kVBarWidth = 13;
        private const int kVLabelMarginHorizontal = 1;
        private const int kVLabelMarginVertical = 2;
        private CurveEditor m_CurveEditor;
        private DragMode m_DragMode;
        private AreaManipulator m_HBar;
        private AreaManipulator m_HBarLeft;
        private AreaManipulator m_HBarRight;
        private ToolLayout m_Layout;
        private Vector2 m_MouseOffset;
        private Vector2 m_Pivot;
        private Vector2 m_Previous;
        private bool m_RippleTime;
        private float m_RippleTimeEnd;
        private float m_RippleTimeStart;
        private AreaManipulator m_SelectionBox;
        private AreaManipulator m_SelectionScaleBottom;
        private AreaManipulator m_SelectionScaleLeft;
        private AreaManipulator m_SelectionScaleRight;
        private AreaManipulator m_SelectionScaleTop;
        private AreaManipulator m_VBar;
        private AreaManipulator m_VBarBottom;
        private AreaManipulator m_VBarTop;

        private ToolLayout CalculateLayout()
        {
            ToolLayout layout = new ToolLayout();
            bool flag = !Mathf.Approximately(this.selectionBounds.size.x, 0f);
            bool flag2 = !Mathf.Approximately(this.selectionBounds.size.y, 0f);
            float x = base.TimeToPixel(this.selectionBounds.min.x);
            float num2 = base.TimeToPixel(this.selectionBounds.max.x);
            float y = base.ValueToPixel(this.selectionBounds.max.y);
            float num4 = base.ValueToPixel(this.selectionBounds.min.y);
            layout.selectionRect = new Rect(x, y, num2 - x, num4 - y);
            layout.displayHScale = true;
            float width = (layout.selectionRect.width - 15f) - 15f;
            if (width < 14f)
            {
                layout.displayHScale = false;
                width = layout.selectionRect.width;
                if (width < 14f)
                {
                    layout.selectionRect.x = layout.selectionRect.center.x - 7f;
                    layout.selectionRect.width = 14f;
                    width = 14f;
                }
            }
            if (layout.displayHScale)
            {
                layout.hBarLeftRect = new Rect(layout.selectionRect.xMin, base.contentRect.yMin, 15f, 13f);
                layout.hBarRect = new Rect(layout.hBarLeftRect.xMax, base.contentRect.yMin, width, 13f);
                layout.hBarRightRect = new Rect(layout.hBarRect.xMax, base.contentRect.yMin, 15f, 13f);
            }
            else
            {
                layout.hBarRect = new Rect(layout.selectionRect.xMin, base.contentRect.yMin, width, 13f);
                layout.hBarLeftRect = new Rect(0f, 0f, 0f, 0f);
                layout.hBarRightRect = new Rect(0f, 0f, 0f, 0f);
            }
            layout.displayVScale = true;
            float height = (layout.selectionRect.height - 15f) - 15f;
            if (height < 15f)
            {
                layout.displayVScale = false;
                height = layout.selectionRect.height;
                if (height < 15f)
                {
                    layout.selectionRect.y = layout.selectionRect.center.y - 7.5f;
                    layout.selectionRect.height = 15f;
                    height = 15f;
                }
            }
            if (layout.displayVScale)
            {
                layout.vBarTopRect = new Rect(base.contentRect.xMin, layout.selectionRect.yMin, 13f, 15f);
                layout.vBarRect = new Rect(base.contentRect.xMin, layout.vBarTopRect.yMax, 13f, height);
                layout.vBarBottomRect = new Rect(base.contentRect.xMin, layout.vBarRect.yMax, 13f, 15f);
            }
            else
            {
                layout.vBarRect = new Rect(base.contentRect.xMin, layout.selectionRect.yMin, 13f, height);
                layout.vBarTopRect = g_EmptyRect;
                layout.vBarBottomRect = g_EmptyRect;
            }
            if (flag)
            {
                float num7 = 0.09999999f;
                float num8 = 0.09999999f;
                layout.scaleLeftRect = new Rect(layout.selectionRect.xMin - 17f, layout.selectionRect.yMin + (layout.selectionRect.height * num7), 17f, layout.selectionRect.height * 0.8f);
                layout.scaleRightRect = new Rect(layout.selectionRect.xMax, layout.selectionRect.yMin + (layout.selectionRect.height * num8), 17f, layout.selectionRect.height * 0.8f);
            }
            else
            {
                layout.scaleLeftRect = g_EmptyRect;
                layout.scaleRightRect = g_EmptyRect;
            }
            if (flag2)
            {
                float num9 = 0.09999999f;
                float num10 = 0.09999999f;
                layout.scaleTopRect = new Rect(layout.selectionRect.xMin + (layout.selectionRect.width * num10), layout.selectionRect.yMin - 17f, layout.selectionRect.width * 0.8f, 17f);
                layout.scaleBottomRect = new Rect(layout.selectionRect.xMin + (layout.selectionRect.width * num9), layout.selectionRect.yMax, layout.selectionRect.width * 0.8f, 17f);
            }
            else
            {
                layout.scaleTopRect = g_EmptyRect;
                layout.scaleBottomRect = g_EmptyRect;
            }
            if (flag)
            {
                layout.leftLabelAnchor = new Vector2(layout.selectionRect.xMin - 3f, base.contentRect.yMin + 1f);
                layout.rightLabelAnchor = new Vector2(layout.selectionRect.xMax + 3f, base.contentRect.yMin + 1f);
            }
            else
            {
                layout.leftLabelAnchor = layout.rightLabelAnchor = new Vector2(layout.selectionRect.xMax + 3f, base.contentRect.yMin + 1f);
            }
            if (flag2)
            {
                layout.bottomLabelAnchor = new Vector2(base.contentRect.xMin + 1f, layout.selectionRect.yMax + 2f);
                layout.topLabelAnchor = new Vector2(base.contentRect.xMin + 1f, layout.selectionRect.yMin - 2f);
            }
            else
            {
                layout.bottomLabelAnchor = layout.topLabelAnchor = new Vector2(base.contentRect.xMin + 1f, layout.selectionRect.yMin - 2f);
            }
            layout.selectionLeftRect = new Rect(base.contentRect.xMin + 13f, layout.selectionRect.yMin, layout.selectionRect.xMin - 13f, layout.selectionRect.height);
            layout.selectionTopRect = new Rect(layout.selectionRect.xMin, base.contentRect.yMin + 13f, layout.selectionRect.width, layout.selectionRect.yMin - 13f);
            layout.underlayTopRect = new Rect(base.contentRect.xMin, base.contentRect.yMin, base.contentRect.width, 13f);
            layout.underlayLeftRect = new Rect(base.contentRect.xMin, base.contentRect.yMin + 13f, 13f, base.contentRect.height - 13f);
            return layout;
        }

        private void DrawLabels()
        {
            if (this.dragMode != DragMode.None)
            {
                CurveEditorSettings.RectangleToolFlags rectangleToolFlags = this.m_CurveEditor.settings.rectangleToolFlags;
                bool flag = !Mathf.Approximately(this.selectionBounds.size.x, 0f);
                bool flag2 = !Mathf.Approximately(this.selectionBounds.size.y, 0f);
                if (rectangleToolFlags == CurveEditorSettings.RectangleToolFlags.FullRectangleTool)
                {
                    if ((this.dragMode & DragMode.MoveScaleHorizontal) != DragMode.None)
                    {
                        if (flag)
                        {
                            GUIContent content = new GUIContent($"{this.m_CurveEditor.FormatTime(this.selectionBounds.min.x, this.m_CurveEditor.invSnap, this.m_CurveEditor.timeFormat)}");
                            GUIContent content2 = new GUIContent($"{this.m_CurveEditor.FormatTime(this.selectionBounds.max.x, this.m_CurveEditor.invSnap, this.m_CurveEditor.timeFormat)}");
                            Vector2 vector5 = base.styles.dragLabel.CalcSize(content);
                            Vector2 vector6 = base.styles.dragLabel.CalcSize(content2);
                            EditorGUI.DoDropShadowLabel(new Rect(this.m_Layout.leftLabelAnchor.x - vector5.x, this.m_Layout.leftLabelAnchor.y, vector5.x, vector5.y), content, base.styles.dragLabel, 0.3f);
                            EditorGUI.DoDropShadowLabel(new Rect(this.m_Layout.rightLabelAnchor.x, this.m_Layout.rightLabelAnchor.y, vector6.x, vector6.y), content2, base.styles.dragLabel, 0.3f);
                        }
                        else
                        {
                            GUIContent content3 = new GUIContent($"{this.m_CurveEditor.FormatTime(this.selectionBounds.center.x, this.m_CurveEditor.invSnap, this.m_CurveEditor.timeFormat)}");
                            Vector2 vector8 = base.styles.dragLabel.CalcSize(content3);
                            EditorGUI.DoDropShadowLabel(new Rect(this.m_Layout.leftLabelAnchor.x, this.m_Layout.leftLabelAnchor.y, vector8.x, vector8.y), content3, base.styles.dragLabel, 0.3f);
                        }
                    }
                    if ((this.dragMode & DragMode.MoveScaleVertical) != DragMode.None)
                    {
                        if (flag2)
                        {
                            GUIContent content4 = new GUIContent($"{this.m_CurveEditor.FormatValue(this.selectionBounds.min.y)}");
                            GUIContent content5 = new GUIContent($"{this.m_CurveEditor.FormatValue(this.selectionBounds.max.y)}");
                            Vector2 vector11 = base.styles.dragLabel.CalcSize(content4);
                            Vector2 vector12 = base.styles.dragLabel.CalcSize(content5);
                            EditorGUI.DoDropShadowLabel(new Rect(this.m_Layout.bottomLabelAnchor.x, this.m_Layout.bottomLabelAnchor.y, vector11.x, vector11.y), content4, base.styles.dragLabel, 0.3f);
                            EditorGUI.DoDropShadowLabel(new Rect(this.m_Layout.topLabelAnchor.x, this.m_Layout.topLabelAnchor.y - vector12.y, vector12.x, vector12.y), content5, base.styles.dragLabel, 0.3f);
                        }
                        else
                        {
                            GUIContent content6 = new GUIContent($"{this.m_CurveEditor.FormatValue(this.selectionBounds.center.y)}");
                            Vector2 vector14 = base.styles.dragLabel.CalcSize(content6);
                            EditorGUI.DoDropShadowLabel(new Rect(this.m_Layout.topLabelAnchor.x, this.m_Layout.topLabelAnchor.y - vector14.y, vector14.x, vector14.y), content6, base.styles.dragLabel, 0.3f);
                        }
                    }
                }
                else if ((rectangleToolFlags == CurveEditorSettings.RectangleToolFlags.MiniRectangleTool) && ((this.dragMode & DragMode.MoveBothAxis) != DragMode.None))
                {
                    Vector2 vector15 = (!flag && !flag2) ? this.selectionBounds.center : new Vector2(base.PixelToTime(Event.current.mousePosition.x, this.frameRate), base.PixelToValue(Event.current.mousePosition.y));
                    Vector2 vector18 = new Vector2(base.TimeToPixel(vector15.x), base.ValueToPixel(vector15.y));
                    GUIContent content7 = new GUIContent($"{this.m_CurveEditor.FormatTime(vector15.x, this.m_CurveEditor.invSnap, this.m_CurveEditor.timeFormat)}, {this.m_CurveEditor.FormatValue(vector15.y)}");
                    Vector2 vector19 = base.styles.dragLabel.CalcSize(content7);
                    EditorGUI.DoDropShadowLabel(new Rect(vector18.x, vector18.y - vector19.y, vector19.x, vector19.y), content7, base.styles.dragLabel, 0.3f);
                }
            }
        }

        public void HandleEvents()
        {
            if (this.m_CurveEditor.settings.rectangleToolFlags != CurveEditorSettings.RectangleToolFlags.NoRectangleTool)
            {
                this.m_SelectionScaleTop.HandleEvents();
                this.m_SelectionScaleBottom.HandleEvents();
                this.m_SelectionScaleLeft.HandleEvents();
                this.m_SelectionScaleRight.HandleEvents();
                this.m_SelectionBox.HandleEvents();
            }
        }

        public void HandleOverlayEvents()
        {
            base.HandleClutchKeys();
            switch (this.m_CurveEditor.settings.rectangleToolFlags)
            {
                case CurveEditorSettings.RectangleToolFlags.FullRectangleTool:
                    this.m_VBarBottom.HandleEvents();
                    this.m_VBarTop.HandleEvents();
                    this.m_VBar.HandleEvents();
                    this.m_HBarLeft.HandleEvents();
                    this.m_HBarRight.HandleEvents();
                    this.m_HBar.HandleEvents();
                    break;
            }
        }

        public override void Initialize(TimeArea timeArea)
        {
            base.Initialize(timeArea);
            this.m_CurveEditor = timeArea as CurveEditor;
            if (this.m_HBarLeft == null)
            {
                this.m_HBarLeft = new AreaManipulator(base.styles.rectangleToolHBarLeft, MouseCursor.ResizeHorizontal);
                this.m_HBarLeft.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate) Delegate.Combine(this.m_HBarLeft.onStartDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    if (this.hasSelection && manipulator.rect.Contains(evt.mousePosition))
                    {
                        this.OnStartScale(RectangleTool.ToolCoord.Right, RectangleTool.ToolCoord.Left, new Vector2(base.PixelToTime(evt.mousePosition.x, this.frameRate), 0f), DragMode.ScaleHorizontal, base.rippleTimeClutch);
                        return true;
                    }
                    return false;
                });
                this.m_HBarLeft.onDrag = (AnimationWindowManipulator.OnDragDelegate) Delegate.Combine(this.m_HBarLeft.onDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    this.OnScaleTime(base.PixelToTime(evt.mousePosition.x, this.frameRate));
                    return true;
                });
                this.m_HBarLeft.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate) Delegate.Combine(this.m_HBarLeft.onEndDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    this.OnEndScale();
                    return true;
                });
            }
            if (this.m_HBarRight == null)
            {
                this.m_HBarRight = new AreaManipulator(base.styles.rectangleToolHBarRight, MouseCursor.ResizeHorizontal);
                this.m_HBarRight.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate) Delegate.Combine(this.m_HBarRight.onStartDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    if (this.hasSelection && manipulator.rect.Contains(evt.mousePosition))
                    {
                        this.OnStartScale(RectangleTool.ToolCoord.Left, RectangleTool.ToolCoord.Right, new Vector2(base.PixelToTime(evt.mousePosition.x, this.frameRate), 0f), DragMode.ScaleHorizontal, base.rippleTimeClutch);
                        return true;
                    }
                    return false;
                });
                this.m_HBarRight.onDrag = (AnimationWindowManipulator.OnDragDelegate) Delegate.Combine(this.m_HBarRight.onDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    this.OnScaleTime(base.PixelToTime(evt.mousePosition.x, this.frameRate));
                    return true;
                });
                this.m_HBarRight.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate) Delegate.Combine(this.m_HBarRight.onEndDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    this.OnEndScale();
                    return true;
                });
            }
            if (this.m_HBar == null)
            {
                this.m_HBar = new AreaManipulator(base.styles.rectangleToolHBar, MouseCursor.MoveArrow);
                this.m_HBar.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate) Delegate.Combine(this.m_HBar.onStartDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    if (this.hasSelection && manipulator.rect.Contains(evt.mousePosition))
                    {
                        this.OnStartMove(new Vector2(base.PixelToTime(evt.mousePosition.x, this.frameRate), 0f), DragMode.MoveHorizontal, base.rippleTimeClutch);
                        return true;
                    }
                    return false;
                });
                this.m_HBar.onDrag = (AnimationWindowManipulator.OnDragDelegate) Delegate.Combine(this.m_HBar.onDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    this.OnMove(new Vector2(base.PixelToTime(evt.mousePosition.x, this.frameRate), 0f));
                    return true;
                });
                this.m_HBar.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate) Delegate.Combine(this.m_HBar.onEndDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    this.OnEndMove();
                    return true;
                });
            }
            if (this.m_VBarBottom == null)
            {
                this.m_VBarBottom = new AreaManipulator(base.styles.rectangleToolVBarBottom, MouseCursor.ResizeVertical);
                this.m_VBarBottom.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate) Delegate.Combine(this.m_VBarBottom.onStartDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    if (this.hasSelection && manipulator.rect.Contains(evt.mousePosition))
                    {
                        this.OnStartScale(RectangleTool.ToolCoord.Top, RectangleTool.ToolCoord.Bottom, new Vector2(0f, base.PixelToValue(evt.mousePosition.y)), DragMode.ScaleVertical, false);
                        return true;
                    }
                    return false;
                });
                this.m_VBarBottom.onDrag = (AnimationWindowManipulator.OnDragDelegate) Delegate.Combine(this.m_VBarBottom.onDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    this.OnScaleValue(base.PixelToValue(evt.mousePosition.y));
                    return true;
                });
                this.m_VBarBottom.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate) Delegate.Combine(this.m_VBarBottom.onEndDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    this.OnEndScale();
                    return true;
                });
            }
            if (this.m_VBarTop == null)
            {
                this.m_VBarTop = new AreaManipulator(base.styles.rectangleToolVBarTop, MouseCursor.ResizeVertical);
                this.m_VBarTop.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate) Delegate.Combine(this.m_VBarTop.onStartDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    if (this.hasSelection && manipulator.rect.Contains(evt.mousePosition))
                    {
                        this.OnStartScale(RectangleTool.ToolCoord.Bottom, RectangleTool.ToolCoord.Top, new Vector2(0f, base.PixelToValue(evt.mousePosition.y)), DragMode.ScaleVertical, false);
                        return true;
                    }
                    return false;
                });
                this.m_VBarTop.onDrag = (AnimationWindowManipulator.OnDragDelegate) Delegate.Combine(this.m_VBarTop.onDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    this.OnScaleValue(base.PixelToValue(evt.mousePosition.y));
                    return true;
                });
                this.m_VBarTop.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate) Delegate.Combine(this.m_VBarTop.onEndDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    this.OnEndScale();
                    return true;
                });
            }
            if (this.m_VBar == null)
            {
                this.m_VBar = new AreaManipulator(base.styles.rectangleToolVBar, MouseCursor.MoveArrow);
                this.m_VBar.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate) Delegate.Combine(this.m_VBar.onStartDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    if (this.hasSelection && manipulator.rect.Contains(evt.mousePosition))
                    {
                        this.OnStartMove(new Vector2(0f, base.PixelToValue(evt.mousePosition.y)), DragMode.MoveVertical, false);
                        return true;
                    }
                    return false;
                });
                this.m_VBar.onDrag = (AnimationWindowManipulator.OnDragDelegate) Delegate.Combine(this.m_VBar.onDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    this.OnMove(new Vector2(0f, base.PixelToValue(evt.mousePosition.y)));
                    return true;
                });
                this.m_VBar.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate) Delegate.Combine(this.m_VBar.onEndDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    this.OnEndMove();
                    return true;
                });
            }
            if (this.m_SelectionBox == null)
            {
                this.m_SelectionBox = new AreaManipulator(base.styles.rectangleToolSelection, MouseCursor.MoveArrow);
                this.m_SelectionBox.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate) Delegate.Combine(this.m_SelectionBox.onStartDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    if ((!(evt.shift || EditorGUI.actionKey) && this.hasSelection) && manipulator.rect.Contains(evt.mousePosition))
                    {
                        this.OnStartMove(new Vector2(base.PixelToTime(evt.mousePosition.x, this.frameRate), base.PixelToValue(evt.mousePosition.y)), !base.rippleTimeClutch ? DragMode.MoveBothAxis : DragMode.MoveHorizontal, base.rippleTimeClutch);
                        return true;
                    }
                    return false;
                });
                this.m_SelectionBox.onDrag = (AnimationWindowManipulator.OnDragDelegate) Delegate.Combine(this.m_SelectionBox.onDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    if (evt.shift && (this.m_DragMode == DragMode.MoveBothAxis))
                    {
                        float f = evt.mousePosition.x - base.TimeToPixel(this.m_Previous.x);
                        float num2 = evt.mousePosition.y - base.ValueToPixel(this.m_Previous.y);
                        this.m_DragMode = (Mathf.Abs(f) <= Mathf.Abs(num2)) ? DragMode.MoveVertical : DragMode.MoveHorizontal;
                    }
                    float x = ((this.m_DragMode & DragMode.MoveHorizontal) == DragMode.None) ? this.m_Previous.x : base.PixelToTime(evt.mousePosition.x, this.frameRate);
                    float y = ((this.m_DragMode & DragMode.MoveVertical) == DragMode.None) ? this.m_Previous.y : base.PixelToValue(evt.mousePosition.y);
                    this.OnMove(new Vector2(x, y));
                    return true;
                });
                this.m_SelectionBox.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate) Delegate.Combine(this.m_SelectionBox.onEndDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    this.OnEndMove();
                    return true;
                });
            }
            if (this.m_SelectionScaleLeft == null)
            {
                this.m_SelectionScaleLeft = new AreaManipulator(base.styles.rectangleToolScaleLeft, MouseCursor.ResizeHorizontal);
                this.m_SelectionScaleLeft.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate) Delegate.Combine(this.m_SelectionScaleLeft.onStartDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    if (this.hasSelection && manipulator.rect.Contains(evt.mousePosition))
                    {
                        this.OnStartScale(RectangleTool.ToolCoord.Right, RectangleTool.ToolCoord.Left, new Vector2(base.PixelToTime(evt.mousePosition.x, this.frameRate), 0f), DragMode.ScaleHorizontal, base.rippleTimeClutch);
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
                this.m_SelectionScaleRight = new AreaManipulator(base.styles.rectangleToolScaleRight, MouseCursor.ResizeHorizontal);
                this.m_SelectionScaleRight.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate) Delegate.Combine(this.m_SelectionScaleRight.onStartDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    if (this.hasSelection && manipulator.rect.Contains(evt.mousePosition))
                    {
                        this.OnStartScale(RectangleTool.ToolCoord.Left, RectangleTool.ToolCoord.Right, new Vector2(base.PixelToTime(evt.mousePosition.x, this.frameRate), 0f), DragMode.ScaleHorizontal, base.rippleTimeClutch);
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
            if (this.m_SelectionScaleBottom == null)
            {
                this.m_SelectionScaleBottom = new AreaManipulator(base.styles.rectangleToolScaleBottom, MouseCursor.ResizeVertical);
                this.m_SelectionScaleBottom.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate) Delegate.Combine(this.m_SelectionScaleBottom.onStartDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    if (this.hasSelection && manipulator.rect.Contains(evt.mousePosition))
                    {
                        this.OnStartScale(RectangleTool.ToolCoord.Top, RectangleTool.ToolCoord.Bottom, new Vector2(0f, base.PixelToValue(evt.mousePosition.y)), DragMode.ScaleVertical, false);
                        return true;
                    }
                    return false;
                });
                this.m_SelectionScaleBottom.onDrag = (AnimationWindowManipulator.OnDragDelegate) Delegate.Combine(this.m_SelectionScaleBottom.onDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    this.OnScaleValue(base.PixelToValue(evt.mousePosition.y));
                    return true;
                });
                this.m_SelectionScaleBottom.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate) Delegate.Combine(this.m_SelectionScaleBottom.onEndDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    this.OnEndScale();
                    return true;
                });
            }
            if (this.m_SelectionScaleTop == null)
            {
                this.m_SelectionScaleTop = new AreaManipulator(base.styles.rectangleToolScaleTop, MouseCursor.ResizeVertical);
                this.m_SelectionScaleTop.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate) Delegate.Combine(this.m_SelectionScaleTop.onStartDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    if (this.hasSelection && manipulator.rect.Contains(evt.mousePosition))
                    {
                        this.OnStartScale(RectangleTool.ToolCoord.Bottom, RectangleTool.ToolCoord.Top, new Vector2(0f, base.PixelToValue(evt.mousePosition.y)), DragMode.ScaleVertical, false);
                        return true;
                    }
                    return false;
                });
                this.m_SelectionScaleTop.onDrag = (AnimationWindowManipulator.OnDragDelegate) Delegate.Combine(this.m_SelectionScaleTop.onDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    this.OnScaleValue(base.PixelToValue(evt.mousePosition.y));
                    return true;
                });
                this.m_SelectionScaleTop.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate) Delegate.Combine(this.m_SelectionScaleTop.onEndDrag, delegate (AnimationWindowManipulator manipulator, Event evt) {
                    this.OnEndScale();
                    return true;
                });
            }
        }

        internal void OnEndMove()
        {
            this.m_CurveEditor.EndLiveEdit();
            this.m_DragMode = DragMode.None;
            GUI.changed = true;
        }

        private void OnEndScale()
        {
            this.m_CurveEditor.EndLiveEdit();
            this.m_DragMode = DragMode.None;
            GUI.changed = true;
        }

        public void OnGUI()
        {
            if (this.hasSelection && (Event.current.type == EventType.Repaint))
            {
                CurveEditorSettings.RectangleToolFlags rectangleToolFlags = this.m_CurveEditor.settings.rectangleToolFlags;
                if (rectangleToolFlags != CurveEditorSettings.RectangleToolFlags.NoRectangleTool)
                {
                    Color color = GUI.color;
                    GUI.color = Color.white;
                    this.m_Layout = this.CalculateLayout();
                    if (rectangleToolFlags == CurveEditorSettings.RectangleToolFlags.FullRectangleTool)
                    {
                        GUI.Label(this.m_Layout.selectionLeftRect, GUIContent.none, base.styles.rectangleToolHighlight);
                        GUI.Label(this.m_Layout.selectionTopRect, GUIContent.none, base.styles.rectangleToolHighlight);
                        GUI.Label(this.m_Layout.underlayLeftRect, GUIContent.none, base.styles.rectangleToolHighlight);
                        GUI.Label(this.m_Layout.underlayTopRect, GUIContent.none, base.styles.rectangleToolHighlight);
                    }
                    this.m_SelectionBox.OnGUI(this.m_Layout.selectionRect);
                    this.m_SelectionScaleTop.OnGUI(this.m_Layout.scaleTopRect);
                    this.m_SelectionScaleBottom.OnGUI(this.m_Layout.scaleBottomRect);
                    this.m_SelectionScaleLeft.OnGUI(this.m_Layout.scaleLeftRect);
                    this.m_SelectionScaleRight.OnGUI(this.m_Layout.scaleRightRect);
                    GUI.color = color;
                }
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
            this.OnStartMove(position, DragMode.None, rippleTime);
        }

        private void OnStartMove(Vector2 position, DragMode dragMode, bool rippleTime)
        {
            Bounds selectionBounds = this.selectionBounds;
            this.m_DragMode = dragMode;
            this.m_Previous = position;
            this.m_RippleTime = rippleTime;
            this.m_RippleTimeStart = selectionBounds.min.x;
            this.m_RippleTimeEnd = selectionBounds.max.x;
            this.m_CurveEditor.StartLiveEdit();
        }

        private void OnStartScale(RectangleTool.ToolCoord pivotCoord, RectangleTool.ToolCoord pickedCoord, Vector2 mousePos, DragMode dragMode, bool rippleTime)
        {
            Bounds selectionBounds = this.selectionBounds;
            this.m_DragMode = dragMode;
            this.m_Pivot = base.ToolCoordToPosition(pivotCoord, selectionBounds);
            this.m_Previous = base.ToolCoordToPosition(pickedCoord, selectionBounds);
            this.m_MouseOffset = mousePos - this.m_Previous;
            this.m_RippleTime = rippleTime;
            this.m_RippleTimeStart = selectionBounds.min.x;
            this.m_RippleTimeEnd = selectionBounds.max.x;
            this.m_CurveEditor.StartLiveEdit();
        }

        public void OverlayOnGUI()
        {
            if (this.hasSelection && (Event.current.type == EventType.Repaint))
            {
                Color color = GUI.color;
                if (this.m_CurveEditor.settings.rectangleToolFlags == CurveEditorSettings.RectangleToolFlags.FullRectangleTool)
                {
                    GUI.color = Color.white;
                    this.m_HBar.OnGUI(this.m_Layout.hBarRect);
                    this.m_HBarLeft.OnGUI(this.m_Layout.hBarLeftRect);
                    this.m_HBarRight.OnGUI(this.m_Layout.hBarRightRect);
                    this.m_VBar.OnGUI(this.m_Layout.vBarRect);
                    this.m_VBarBottom.OnGUI(this.m_Layout.vBarBottomRect);
                    this.m_VBarTop.OnGUI(this.m_Layout.vBarTopRect);
                }
                this.DrawLabels();
                GUI.color = color;
            }
        }

        private void TransformKeys(Matrix4x4 matrix, bool flipX, bool flipY)
        {
            if (this.m_RippleTime)
            {
                this.m_CurveEditor.TransformRippleKeys(matrix, this.m_RippleTimeStart, this.m_RippleTimeEnd, flipX);
                GUI.changed = true;
            }
            else
            {
                this.m_CurveEditor.TransformSelectedKeys(matrix, flipX, flipY);
                GUI.changed = true;
            }
        }

        private DragMode dragMode
        {
            get
            {
                if (this.m_DragMode != DragMode.None)
                {
                    return this.m_DragMode;
                }
                if (this.m_CurveEditor.IsDraggingKey())
                {
                    return DragMode.MoveBothAxis;
                }
                return DragMode.None;
            }
        }

        private float frameRate =>
            this.m_CurveEditor.invSnap;

        private bool hasSelection =>
            (this.m_CurveEditor.hasSelection && !this.m_CurveEditor.IsDraggingCurveOrRegion());

        private Bounds selectionBounds =>
            this.m_CurveEditor.selectionBounds;

        private enum DragMode
        {
            MoveBothAxis = 3,
            MoveHorizontal = 1,
            MoveScaleHorizontal = 5,
            MoveScaleVertical = 10,
            MoveVertical = 2,
            None = 0,
            ScaleBothAxis = 12,
            ScaleHorizontal = 4,
            ScaleVertical = 8
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ToolLayout
        {
            public Rect selectionRect;
            public Rect hBarRect;
            public Rect hBarLeftRect;
            public Rect hBarRightRect;
            public bool displayHScale;
            public Rect vBarRect;
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
            public Vector2 bottomLabelAnchor;
            public Vector2 topLabelAnchor;
        }
    }
}

