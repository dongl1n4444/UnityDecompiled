namespace UnityEditorInternal
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    internal struct AnimationKeyTime
    {
        [SerializeField]
        private float m_FrameRate;
        [SerializeField]
        private int m_Frame;
        [SerializeField]
        private float m_Time;
        public float time =>
            this.m_Time;
        public int frame =>
            this.m_Frame;
        public float frameRate =>
            this.m_FrameRate;
        public float frameFloor =>
            ((this.frame - 0.5f) / this.frameRate);
        public float frameCeiling =>
            ((this.frame + 0.5f) / this.frameRate);
        public static AnimationKeyTime Time(float time, float frameRate)
        {
            AnimationKeyTime time2 = new AnimationKeyTime {
                m_Time = Mathf.Max(time, 0f),
                m_FrameRate = frameRate
            };
            time2.m_Frame = Mathf.RoundToInt(time2.m_Time * frameRate);
            return time2;
        }

        public static AnimationKeyTime Frame(int frame, float frameRate)
        {
            AnimationKeyTime time = new AnimationKeyTime {
                m_Frame = (frame >= 0) ? frame : 0
            };
            time.m_Time = ((float) time.m_Frame) / frameRate;
            time.m_FrameRate = frameRate;
            return time;
        }

        public bool ContainsTime(float time) => 
            ((time >= this.frameFloor) && (time < this.frameCeiling));

        public bool Equals(AnimationKeyTime key) => 
            (((this.m_Frame == key.m_Frame) && (this.m_FrameRate == key.m_FrameRate)) && Mathf.Approximately(this.m_Time, key.m_Time));
    }
}

