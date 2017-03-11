namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class InheritVelocityModuleUI : ModuleUI
    {
        private SerializedMinMaxCurve m_Curve;
        private SerializedProperty m_Mode;
        private static Texts s_Texts;

        public InheritVelocityModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "InheritVelocityModule", displayName)
        {
            base.m_ToolTip = "Controls the velocity inherited from the emitter, for each particle.";
        }

        protected override void Init()
        {
            if (this.m_Curve == null)
            {
                if (s_Texts == null)
                {
                    s_Texts = new Texts();
                }
                this.m_Mode = base.GetProperty("m_Mode");
                this.m_Curve = new SerializedMinMaxCurve(this, GUIContent.none, "m_Curve", ModuleUI.kUseSignedRange);
            }
        }

        public override void OnInspectorGUI(InitialModuleUI initial)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            ModuleUI.GUIPopup(s_Texts.mode, this.m_Mode, s_Texts.modes, new GUILayoutOption[0]);
            ModuleUI.GUIMinMaxCurve(s_Texts.velocity, this.m_Curve, new GUILayoutOption[0]);
        }

        public override void UpdateCullingSupportedString(ref string text)
        {
            this.Init();
            string failureReason = string.Empty;
            if (!this.m_Curve.SupportsProcedural(ref failureReason))
            {
                text = text + "\nInherit Velocity module curve: " + failureReason;
            }
        }

        private enum Modes
        {
            Initial,
            Current
        }

        private class Texts
        {
            public GUIContent mode = EditorGUIUtility.TextContent("Mode|Specifies whether the emitter velocity is inherited as a one-shot when a particle is born, always using the current emitter velocity, or using the emitter velocity when the particle was born.");
            public string[] modes = new string[] { "Initial", "Current" };
            public GUIContent velocity = EditorGUIUtility.TextContent("Multiplier|Controls the amount of emitter velocity inherited during each particle's lifetime.");
        }
    }
}

