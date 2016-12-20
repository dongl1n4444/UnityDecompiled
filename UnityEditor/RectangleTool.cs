namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class RectangleTool
    {
        private bool m_RippleTimeClutch;
        private Styles m_Styles;
        private TimeArea m_TimeArea;

        public bool CalculateScaleTimeMatrix(float fromTime, float toTime, float offsetTime, float pivotTime, float frameRate, out Matrix4x4 transform, out bool flipKeys)
        {
            transform = Matrix4x4.identity;
            flipKeys = false;
            float num = !Mathf.Approximately(frameRate, 0f) ? (1f / frameRate) : 0.001f;
            float f = toTime - pivotTime;
            float num3 = fromTime - pivotTime;
            if ((Mathf.Abs(f) - offsetTime) < 0f)
            {
                return false;
            }
            f = (Mathf.Sign(f) != Mathf.Sign(num3)) ? (f + offsetTime) : (f - offsetTime);
            if (Mathf.Approximately(num3, 0f))
            {
                transform.SetTRS(new Vector3(f, 0f, 0f), Quaternion.identity, Vector3.one);
                flipKeys = false;
                return true;
            }
            if (Mathf.Abs(f) < num)
            {
                f = (f >= 0f) ? num : -num;
            }
            float x = f / num3;
            transform.SetTRS(new Vector3(pivotTime, 0f, 0f), Quaternion.identity, Vector3.one);
            transform *= Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(x, 1f, 1f));
            transform *= Matrix4x4.TRS(new Vector3(-pivotTime, 0f), Quaternion.identity, Vector3.one);
            flipKeys = x < 0f;
            return true;
        }

        public bool CalculateScaleValueMatrix(float fromValue, float toValue, float offsetValue, float pivotValue, out Matrix4x4 transform, out bool flipKeys)
        {
            transform = Matrix4x4.identity;
            flipKeys = false;
            float num = 0.001f;
            float f = toValue - pivotValue;
            float num3 = fromValue - pivotValue;
            if ((Mathf.Abs(f) - offsetValue) < 0f)
            {
                return false;
            }
            f = (Mathf.Sign(f) != Mathf.Sign(num3)) ? (f + offsetValue) : (f - offsetValue);
            if (Mathf.Approximately(num3, 0f))
            {
                transform.SetTRS(new Vector3(0f, f, 0f), Quaternion.identity, Vector3.one);
                flipKeys = false;
                return true;
            }
            if (Mathf.Abs(f) < num)
            {
                f = (f >= 0f) ? num : -num;
            }
            float y = f / num3;
            transform.SetTRS(new Vector3(0f, pivotValue, 0f), Quaternion.identity, Vector3.one);
            transform *= Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f, y, 1f));
            transform *= Matrix4x4.TRS(new Vector3(0f, -pivotValue, 0f), Quaternion.identity, Vector3.one);
            flipKeys = y < 0f;
            return true;
        }

        public void HandleClutchKeys()
        {
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.KeyDown:
                    if (current.keyCode == KeyCode.R)
                    {
                        this.m_RippleTimeClutch = true;
                    }
                    break;

                case EventType.KeyUp:
                    if (current.keyCode == KeyCode.R)
                    {
                        this.m_RippleTimeClutch = false;
                    }
                    break;
            }
        }

        public virtual void Initialize(TimeArea timeArea)
        {
            this.m_TimeArea = timeArea;
            if (this.m_Styles == null)
            {
                this.m_Styles = new Styles();
            }
        }

        public float PixelToTime(float pixelTime, float frameRate)
        {
            float width = this.contentRect.width;
            float num2 = this.m_TimeArea.shownArea.xMax - this.m_TimeArea.shownArea.xMin;
            float xMin = this.m_TimeArea.shownArea.xMin;
            float num4 = ((pixelTime / width) * num2) + xMin;
            if (frameRate != 0f)
            {
                num4 = Mathf.Round(num4 * frameRate) / frameRate;
            }
            return num4;
        }

        public float PixelToValue(float pixelValue)
        {
            float height = this.contentRect.height;
            float num2 = this.m_TimeArea.m_Scale.y * -1f;
            float num3 = (this.m_TimeArea.shownArea.yMin * num2) * -1f;
            return (((height - pixelValue) - num3) / num2);
        }

        public float TimeToPixel(float time)
        {
            float width = this.contentRect.width;
            float num2 = this.m_TimeArea.shownArea.xMax - this.m_TimeArea.shownArea.xMin;
            float xMin = this.m_TimeArea.shownArea.xMin;
            return (((time - xMin) * width) / num2);
        }

        public Vector2 ToolCoordToPosition(ToolCoord coord, Bounds bounds)
        {
            switch (coord)
            {
                case ToolCoord.BottomLeft:
                    return bounds.min;

                case ToolCoord.Bottom:
                    return new Vector2(bounds.center.x, bounds.min.y);

                case ToolCoord.BottomRight:
                    return new Vector2(bounds.max.x, bounds.min.y);

                case ToolCoord.Left:
                    return new Vector2(bounds.min.x, bounds.center.y);

                case ToolCoord.Center:
                    return bounds.center;

                case ToolCoord.Right:
                    return new Vector2(bounds.max.x, bounds.center.y);

                case ToolCoord.TopLeft:
                    return new Vector2(bounds.min.x, bounds.max.y);

                case ToolCoord.Top:
                    return new Vector2(bounds.center.x, bounds.max.y);

                case ToolCoord.TopRight:
                    return bounds.max;
            }
            return Vector2.zero;
        }

        public float ValueToPixel(float value)
        {
            float height = this.contentRect.height;
            float num2 = this.m_TimeArea.m_Scale.y * -1f;
            float num3 = (this.m_TimeArea.shownArea.yMin * num2) * -1f;
            return (height - ((value * num2) + num3));
        }

        public Rect contentRect
        {
            get
            {
                return new Rect(0f, 0f, this.m_TimeArea.drawRect.width, this.m_TimeArea.drawRect.height);
            }
        }

        public bool rippleTimeClutch
        {
            get
            {
                return this.m_RippleTimeClutch;
            }
        }

        public Styles styles
        {
            get
            {
                return this.m_Styles;
            }
        }

        public TimeArea timeArea
        {
            get
            {
                return this.m_TimeArea;
            }
        }

        internal class Styles
        {
            public GUIStyle dopesheetScaleLeft = "DopesheetScaleLeft";
            public GUIStyle dopesheetScaleRight = "DopesheetScaleRight";
            public GUIStyle dragLabel = "ProfilerBadge";
            public GUIStyle rectangleToolHBar = "RectangleToolHBar";
            public GUIStyle rectangleToolHBarLeft = "RectangleToolHBarLeft";
            public GUIStyle rectangleToolHBarRight = "RectangleToolHBarRight";
            public GUIStyle rectangleToolHighlight = "RectangleToolHighlight";
            public GUIStyle rectangleToolScaleBottom = "RectangleToolScaleBottom";
            public GUIStyle rectangleToolScaleLeft = "RectangleToolScaleLeft";
            public GUIStyle rectangleToolScaleRight = "RectangleToolScaleRight";
            public GUIStyle rectangleToolScaleTop = "RectangleToolScaleTop";
            public GUIStyle rectangleToolSelection = "RectangleToolSelection";
            public GUIStyle rectangleToolVBar = "RectangleToolVBar";
            public GUIStyle rectangleToolVBarBottom = "RectangleToolVBarBottom";
            public GUIStyle rectangleToolVBarTop = "RectangleToolVBarTop";
        }

        internal enum ToolCoord
        {
            BottomLeft,
            Bottom,
            BottomRight,
            Left,
            Center,
            Right,
            TopLeft,
            Top,
            TopRight
        }
    }
}

