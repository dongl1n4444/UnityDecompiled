namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class CurveControlPointRenderer
    {
        private ControlPointRenderer m_SelectedPointOverlayRenderer;
        private ControlPointRenderer m_SelectedPointRenderer;
        private ControlPointRenderer m_SemiSelectedPointOverlayRenderer;
        private ControlPointRenderer m_UnselectedPointRenderer;

        public CurveControlPointRenderer(CurveEditor.Styles style)
        {
            this.m_UnselectedPointRenderer = new ControlPointRenderer(style.pointIcon);
            this.m_SelectedPointRenderer = new ControlPointRenderer(style.pointIconSelected);
            this.m_SelectedPointOverlayRenderer = new ControlPointRenderer(style.pointIconSelectedOverlay);
            this.m_SemiSelectedPointOverlayRenderer = new ControlPointRenderer(style.pointIconSemiSelectedOverlay);
        }

        public void AddPoint(Rect rect, Color color)
        {
            this.m_UnselectedPointRenderer.AddPoint(rect, color);
        }

        public void AddSelectedPoint(Rect rect, Color color)
        {
            this.m_SelectedPointRenderer.AddPoint(rect, color);
            this.m_SelectedPointOverlayRenderer.AddPoint(rect, Color.white);
        }

        public void AddSemiSelectedPoint(Rect rect, Color color)
        {
            this.m_SelectedPointRenderer.AddPoint(rect, color);
            this.m_SemiSelectedPointOverlayRenderer.AddPoint(rect, Color.white);
        }

        public void Clear()
        {
            this.m_UnselectedPointRenderer.Clear();
            this.m_SelectedPointRenderer.Clear();
            this.m_SelectedPointOverlayRenderer.Clear();
            this.m_SemiSelectedPointOverlayRenderer.Clear();
        }

        public void FlushCache()
        {
            this.m_UnselectedPointRenderer.FlushCache();
            this.m_SelectedPointRenderer.FlushCache();
            this.m_SelectedPointOverlayRenderer.FlushCache();
            this.m_SemiSelectedPointOverlayRenderer.FlushCache();
        }

        public void Render()
        {
            this.m_UnselectedPointRenderer.Render();
            this.m_SelectedPointRenderer.Render();
            this.m_SelectedPointOverlayRenderer.Render();
            this.m_SemiSelectedPointOverlayRenderer.Render();
        }
    }
}

