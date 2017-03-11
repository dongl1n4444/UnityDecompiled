namespace UnityEngine
{
    using System;

    internal class ScrollViewState
    {
        public bool apply;
        public Rect position;
        public Vector2 scrollPosition;
        public Rect viewRect;
        public Rect visibleRect;

        private Vector2 ScrollNeeded(Rect pos)
        {
            Rect visibleRect = this.visibleRect;
            visibleRect.x += this.scrollPosition.x;
            visibleRect.y += this.scrollPosition.y;
            float num = pos.width - this.visibleRect.width;
            if (num > 0f)
            {
                pos.width -= num;
                pos.x += num * 0.5f;
            }
            num = pos.height - this.visibleRect.height;
            if (num > 0f)
            {
                pos.height -= num;
                pos.y += num * 0.5f;
            }
            Vector2 zero = Vector2.zero;
            if (pos.xMax > visibleRect.xMax)
            {
                zero.x += pos.xMax - visibleRect.xMax;
            }
            else if (pos.xMin < visibleRect.xMin)
            {
                zero.x -= visibleRect.xMin - pos.xMin;
            }
            if (pos.yMax > visibleRect.yMax)
            {
                zero.y += pos.yMax - visibleRect.yMax;
            }
            else if (pos.yMin < visibleRect.yMin)
            {
                zero.y -= visibleRect.yMin - pos.yMin;
            }
            Rect viewRect = this.viewRect;
            viewRect.width = Mathf.Max(viewRect.width, this.visibleRect.width);
            viewRect.height = Mathf.Max(viewRect.height, this.visibleRect.height);
            zero.x = Mathf.Clamp(zero.x, viewRect.xMin - this.scrollPosition.x, (viewRect.xMax - this.visibleRect.width) - this.scrollPosition.x);
            zero.y = Mathf.Clamp(zero.y, viewRect.yMin - this.scrollPosition.y, (viewRect.yMax - this.visibleRect.height) - this.scrollPosition.y);
            return zero;
        }

        public void ScrollTo(Rect pos)
        {
            this.ScrollTowards(pos, float.PositiveInfinity);
        }

        public bool ScrollTowards(Rect pos, float maxDelta)
        {
            Vector2 vector = this.ScrollNeeded(pos);
            if (vector.sqrMagnitude < 0.0001f)
            {
                return false;
            }
            if (maxDelta != 0f)
            {
                if (vector.magnitude > maxDelta)
                {
                    vector = (Vector2) (vector.normalized * maxDelta);
                }
                this.scrollPosition += vector;
                this.apply = true;
            }
            return true;
        }
    }
}

