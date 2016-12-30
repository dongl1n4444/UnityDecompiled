namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class EmissionModuleUI : ModuleUI
    {
        private const int k_MaxNumBursts = 4;
        private SerializedProperty m_BurstCount;
        private SerializedProperty[] m_BurstParticleMaxCount;
        private SerializedProperty[] m_BurstParticleMinCount;
        private SerializedProperty[] m_BurstTime;
        public SerializedMinMaxCurve m_Distance;
        public SerializedMinMaxCurve m_Time;
        private static Texts s_Texts;

        public EmissionModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "EmissionModule", displayName)
        {
            this.m_BurstTime = new SerializedProperty[4];
            this.m_BurstParticleMinCount = new SerializedProperty[4];
            this.m_BurstParticleMaxCount = new SerializedProperty[4];
            base.m_ToolTip = "Emission of the emitter. This controls the rate at which particles are emitted as well as burst emissions.";
        }

        private void DoBurstGUI(InitialModuleUI initial)
        {
            EditorGUILayout.Space();
            Rect controlRect = ModuleUI.GetControlRect(13, new GUILayoutOption[0]);
            GUI.Label(controlRect, s_Texts.burst, ParticleSystemStyles.Get().label);
            float dragWidth = 20f;
            float num2 = 40f;
            float width = (((num2 + dragWidth) * 3f) + dragWidth) - 1f;
            float a = controlRect.width - width;
            a = Mathf.Min(a, EditorGUIUtility.labelWidth);
            int intValue = this.m_BurstCount.intValue;
            Rect position = new Rect(controlRect.x + a, controlRect.y, width, 3f);
            GUI.Label(position, GUIContent.none, ParticleSystemStyles.Get().line);
            Rect rect3 = new Rect((controlRect.x + dragWidth) + a, controlRect.y, num2 + dragWidth, controlRect.height);
            GUI.Label(rect3, "Time", ParticleSystemStyles.Get().label);
            rect3.x += dragWidth + num2;
            GUI.Label(rect3, "Min", ParticleSystemStyles.Get().label);
            rect3.x += dragWidth + num2;
            GUI.Label(rect3, "Max", ParticleSystemStyles.Get().label);
            position.y += 12f;
            GUI.Label(position, GUIContent.none, ParticleSystemStyles.Get().line);
            float floatValue = initial.m_LengthInSec.floatValue;
            int num7 = intValue;
            for (int i = 0; i < intValue; i++)
            {
                SerializedProperty floatProp = this.m_BurstTime[i];
                SerializedProperty property2 = this.m_BurstParticleMinCount[i];
                SerializedProperty property3 = this.m_BurstParticleMaxCount[i];
                controlRect = ModuleUI.GetControlRect(13, new GUILayoutOption[0]);
                rect3 = new Rect(controlRect.x + a, controlRect.y, dragWidth + num2, controlRect.height);
                float num9 = ModuleUI.FloatDraggable(rect3, floatProp, 1f, dragWidth, "n2");
                if (num9 < 0f)
                {
                    floatProp.floatValue = 0f;
                }
                if (num9 > floatValue)
                {
                    floatProp.floatValue = floatValue;
                }
                int num10 = property2.intValue;
                int num11 = property3.intValue;
                rect3.x += rect3.width;
                property2.intValue = ModuleUI.IntDraggable(rect3, null, num10, dragWidth);
                rect3.x += rect3.width;
                property3.intValue = ModuleUI.IntDraggable(rect3, null, num11, dragWidth);
                if (i == (intValue - 1))
                {
                    rect3.x = position.xMax - 12f;
                    if (ModuleUI.MinusButton(rect3))
                    {
                        intValue--;
                    }
                }
            }
            if (intValue < 4)
            {
                rect3 = ModuleUI.GetControlRect(13, new GUILayoutOption[0]);
                rect3.xMin = rect3.xMax - 12f;
                if (ModuleUI.PlusButton(rect3))
                {
                    intValue++;
                }
            }
            if (intValue != num7)
            {
                this.m_BurstCount.intValue = intValue;
            }
        }

        protected override void Init()
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            if (this.m_BurstCount == null)
            {
                this.m_Time = new SerializedMinMaxCurve(this, s_Texts.rateOverTime, "rateOverTime");
                this.m_Time.m_AllowRandom = false;
                this.m_Distance = new SerializedMinMaxCurve(this, s_Texts.rateOverDistance, "rateOverDistance");
                this.m_Distance.m_AllowRandom = false;
                this.m_BurstTime[0] = base.GetProperty("time0");
                this.m_BurstTime[1] = base.GetProperty("time1");
                this.m_BurstTime[2] = base.GetProperty("time2");
                this.m_BurstTime[3] = base.GetProperty("time3");
                this.m_BurstParticleMinCount[0] = base.GetProperty("cnt0");
                this.m_BurstParticleMinCount[1] = base.GetProperty("cnt1");
                this.m_BurstParticleMinCount[2] = base.GetProperty("cnt2");
                this.m_BurstParticleMinCount[3] = base.GetProperty("cnt3");
                this.m_BurstParticleMaxCount[0] = base.GetProperty("cntmax0");
                this.m_BurstParticleMaxCount[1] = base.GetProperty("cntmax1");
                this.m_BurstParticleMaxCount[2] = base.GetProperty("cntmax2");
                this.m_BurstParticleMaxCount[3] = base.GetProperty("cntmax3");
                this.m_BurstCount = base.GetProperty("m_BurstCount");
            }
        }

        public override void OnInspectorGUI(InitialModuleUI initial)
        {
            ModuleUI.GUIMinMaxCurve(s_Texts.rateOverTime, this.m_Time, new GUILayoutOption[0]);
            ModuleUI.GUIMinMaxCurve(s_Texts.rateOverDistance, this.m_Distance, new GUILayoutOption[0]);
            this.DoBurstGUI(initial);
            if ((initial.m_SimulationSpace.intValue != 1) && (this.m_Distance.scalar.floatValue > 0f))
            {
                EditorGUILayout.HelpBox("Distance-based emission only works when using World Space Simulation Space", MessageType.Warning, true);
            }
        }

        public override void UpdateCullingSupportedString(ref string text)
        {
            this.Init();
            if (this.m_Distance.scalar.floatValue > 0f)
            {
                text = text + "\n\tEmission is distance based.";
            }
        }

        private class Texts
        {
            public GUIContent burst = EditorGUIUtility.TextContent("Bursts|Emission of extra particles at specific times during the duration of the system.");
            public GUIContent rateOverDistance = EditorGUIUtility.TextContent("Rate over Distance|The number of particles emitted per distance unit.");
            public GUIContent rateOverTime = EditorGUIUtility.TextContent("Rate over Time|The number of particles emitted per second.");
        }
    }
}

