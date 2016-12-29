namespace UnityEditor.WebGL
{
    using System;
    using UnityEditor;

    internal class ProgressHelper
    {
        internal float m_CurrentBuildStep = 0f;
        internal float m_NumBuildSteps = 0f;

        public float Advance() => 
            (++this.m_CurrentBuildStep / this.m_NumBuildSteps);

        public float Get() => 
            (this.m_CurrentBuildStep / this.m_NumBuildSteps);

        public float LastValue() => 
            ((this.m_CurrentBuildStep - 1f) / this.m_NumBuildSteps);

        public void Reset(float numSteps)
        {
            this.m_CurrentBuildStep = 0f;
            this.m_NumBuildSteps = numSteps;
        }

        public void Show(string title, string message)
        {
            if (EditorUtility.DisplayCancelableProgressBar(title, message, this.Get()))
            {
                throw new Exception(title);
            }
        }

        public void Step(string title, string message)
        {
            this.Advance();
            this.Show(title, message);
        }
    }
}

