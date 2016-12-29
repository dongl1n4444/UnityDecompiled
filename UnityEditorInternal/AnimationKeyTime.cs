namespace UnityEditorInternal
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    internal struct AnimationKeyTime
    {
        private float m_FrameRate;
        private int m_Frame;
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
        public static AnimationKeyTime Time(float time, float frameRate) => 
            new AnimationKeyTime { 
                m_Time = time,
                m_FrameRate = frameRate,
                m_Frame = Mathf.RoundToInt(time * frameRate)
            };

        public static AnimationKeyTime Frame(int frame, float frameRate) => 
            new AnimationKeyTime { 
                m_Time = ((float) frame) / frameRate,
                m_FrameRate = frameRate,
                m_Frame = frame
            };

        public bool ContainsTime(float time) => 
            ((time >= this.frameFloor) && (time < this.frameCeiling));
    }
}

