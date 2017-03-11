namespace UnityEditorInternal
{
    using System;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    internal class AnimEditorOverlay
    {
        private Rect m_ContentRect;
        private TimeCursorManipulator m_PlayHeadCursor;
        private Rect m_Rect;
        public AnimationWindowState state;

        public void HandleEvents()
        {
            this.Initialize();
            this.m_PlayHeadCursor.HandleEvents();
        }

        public void Initialize()
        {
            if (this.m_PlayHeadCursor == null)
            {
                this.m_PlayHeadCursor = new TimeCursorManipulator(AnimationWindowStyles.playHead);
                this.m_PlayHeadCursor.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate) Delegate.Combine(this.m_PlayHeadCursor.onStartDrag, (manipulator, evt) => (evt.mousePosition.y <= (this.m_Rect.yMin + 20f)) && this.OnStartDragPlayHead(evt));
                this.m_PlayHeadCursor.onDrag = (AnimationWindowManipulator.OnDragDelegate) Delegate.Combine(this.m_PlayHeadCursor.onDrag, (manipulator, evt) => this.OnDragPlayHead(evt));
                this.m_PlayHeadCursor.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate) Delegate.Combine(this.m_PlayHeadCursor.onEndDrag, (manipulator, evt) => this.OnEndDragPlayHead(evt));
            }
        }

        public float MousePositionToTime(Event evt)
        {
            float width = this.m_ContentRect.width;
            float time = Mathf.Max((float) (((evt.mousePosition.x / width) * this.state.visibleTimeSpan) + this.state.minVisibleTime), (float) 0f);
            return this.state.SnapToFrame(time, AnimationWindowState.SnapMode.SnapToFrame);
        }

        public float MousePositionToValue(Event evt)
        {
            float num2 = this.m_ContentRect.height - evt.mousePosition.y;
            TimeArea timeArea = this.state.timeArea;
            float num3 = timeArea.m_Scale.y * -1f;
            float num4 = (timeArea.shownArea.yMin * num3) * -1f;
            return ((num2 - num4) / num3);
        }

        private bool OnDragPlayHead(Event evt)
        {
            this.state.controlInterface.ScrubTime(this.MousePositionToTime(evt));
            return true;
        }

        private bool OnEndDragPlayHead(Event evt)
        {
            this.state.controlInterface.EndScrubTime();
            return true;
        }

        public void OnGUI(Rect rect, Rect contentRect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                this.m_Rect = rect;
                this.m_ContentRect = contentRect;
                this.Initialize();
                this.m_PlayHeadCursor.OnGUI(this.m_Rect, this.m_Rect.xMin + this.TimeToPixel(this.state.currentTime));
            }
        }

        private bool OnStartDragPlayHead(Event evt)
        {
            this.state.controlInterface.StopPlayback();
            this.state.controlInterface.StartScrubTime();
            this.state.controlInterface.ScrubTime(this.MousePositionToTime(evt));
            return true;
        }

        public float TimeToPixel(float time) => 
            this.state.TimeToPixel(time);

        public float ValueToPixel(float value)
        {
            TimeArea timeArea = this.state.timeArea;
            float num = timeArea.m_Scale.y * -1f;
            float num2 = (timeArea.shownArea.yMin * num) * -1f;
            return ((value * num) + num2);
        }

        public Rect contentRect =>
            this.m_ContentRect;

        public Rect rect =>
            this.m_Rect;
    }
}

