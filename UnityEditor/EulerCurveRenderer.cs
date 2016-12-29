namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class EulerCurveRenderer : CurveRenderer
    {
        private int component;
        private EulerCurveCombinedRenderer renderer;

        public EulerCurveRenderer(int component, EulerCurveCombinedRenderer renderer)
        {
            this.component = component;
            this.renderer = renderer;
        }

        public void DrawCurve(float minTime, float maxTime, Color color, Matrix4x4 transform, Color wrapColor)
        {
            this.renderer.DrawCurve(minTime, maxTime, color, transform, this.component, wrapColor);
        }

        public float EvaluateCurveDeltaSlow(float time) => 
            this.renderer.EvaluateCurveDeltaSlow(time, this.component);

        public float EvaluateCurveSlow(float time) => 
            this.renderer.EvaluateCurveSlow(time, this.component);

        public void FlushCache()
        {
        }

        public Bounds GetBounds() => 
            this.GetBounds(this.renderer.RangeStart(), this.renderer.RangeEnd());

        public Bounds GetBounds(float minTime, float maxTime) => 
            this.renderer.GetBounds(minTime, maxTime, this.component);

        public AnimationCurve GetCurve() => 
            this.renderer.GetCurveOfComponent(this.component);

        public float RangeEnd() => 
            this.renderer.RangeEnd();

        public float RangeStart() => 
            this.renderer.RangeStart();

        public void SetCustomRange(float start, float end)
        {
            this.renderer.SetCustomRange(start, end);
        }

        public void SetWrap(WrapMode wrap)
        {
            this.renderer.SetWrap(wrap);
        }

        public void SetWrap(WrapMode preWrapMode, WrapMode postWrapMode)
        {
            this.renderer.SetWrap(preWrapMode, postWrapMode);
        }
    }
}

