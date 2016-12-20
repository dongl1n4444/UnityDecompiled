namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class TimeCursorManipulator : AnimationWindowManipulator
    {
        public Alignment alignment;
        public bool dottedLine;
        public bool drawHead;
        public bool drawLine;
        public Color headColor;
        public Color lineColor;
        private GUIStyle m_Style;
        public string tooltip;

        public TimeCursorManipulator(GUIStyle style)
        {
            this.m_Style = style;
            this.dottedLine = false;
            this.headColor = Color.white;
            this.lineColor = style.normal.textColor;
            this.drawLine = true;
            this.drawHead = true;
            this.tooltip = string.Empty;
            this.alignment = Alignment.Center;
        }

        public void OnGUI(Rect windowRect, float pixelTime)
        {
            float fixedWidth = this.m_Style.fixedWidth;
            float fixedHeight = this.m_Style.fixedHeight;
            Vector2 vector = new Vector2(pixelTime, windowRect.yMin);
            switch (this.alignment)
            {
                case Alignment.Center:
                    base.rect = new Rect(vector.x - (fixedWidth / 2f), vector.y, fixedWidth, fixedHeight);
                    break;

                case Alignment.Left:
                    base.rect = new Rect(vector.x - fixedWidth, vector.y, fixedWidth, fixedHeight);
                    break;

                case Alignment.Right:
                    base.rect = new Rect(vector.x, vector.y, fixedWidth, fixedHeight);
                    break;
            }
            Vector3 vector2 = new Vector3(vector.x, vector.y + fixedHeight, 0f);
            Vector3 vector3 = new Vector3(vector.x, windowRect.height, 0f);
            if (this.drawLine)
            {
                Handles.color = this.lineColor;
                if (this.dottedLine)
                {
                    Handles.DrawDottedLine(vector2, vector3, 5f);
                }
                else
                {
                    Handles.DrawLine(vector2, vector3);
                }
            }
            if (this.drawHead)
            {
                Color color = GUI.color;
                GUI.color = this.headColor;
                GUI.Box(base.rect, GUIContent.none, this.m_Style);
                GUI.color = color;
            }
        }

        public enum Alignment
        {
            Center,
            Left,
            Right
        }
    }
}

