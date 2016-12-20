namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class ColorByVelocityModuleUI : ModuleUI
    {
        private SerializedMinMaxGradient m_Gradient;
        private SerializedProperty m_Range;
        private SerializedProperty m_Scale;
        private static Texts s_Texts;

        public ColorByVelocityModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "ColorBySpeedModule", displayName)
        {
            base.m_ToolTip = "Controls the color of each particle based on its speed.";
        }

        protected override void Init()
        {
            if (this.m_Gradient == null)
            {
                this.m_Gradient = new SerializedMinMaxGradient(this);
                this.m_Gradient.m_AllowColor = false;
                this.m_Gradient.m_AllowRandomBetweenTwoColors = false;
                this.m_Range = base.GetProperty("range");
            }
        }

        public override void OnInspectorGUI(ParticleSystem s)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            base.GUIMinMaxGradient(s_Texts.color, this.m_Gradient, new GUILayoutOption[0]);
            ModuleUI.GUIMinMaxRange(s_Texts.velocityRange, this.m_Range, new GUILayoutOption[0]);
        }

        private class Texts
        {
            public GUIContent color = EditorGUIUtility.TextContent("Color|Controls the color of each particle based on its speed.");
            public GUIContent velocityRange = EditorGUIUtility.TextContent("Speed Range|Remaps speed in the defined range to a color.");
        }
    }
}

