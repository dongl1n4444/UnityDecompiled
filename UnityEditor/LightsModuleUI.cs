namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class LightsModuleUI : ModuleUI
    {
        private SerializedProperty m_AlphaAffectsIntensity;
        private SerializedMinMaxCurve m_Intensity;
        private SerializedProperty m_Light;
        private SerializedProperty m_MaxLights;
        private SerializedProperty m_RandomDistribution;
        private SerializedMinMaxCurve m_Range;
        private SerializedProperty m_Ratio;
        private SerializedProperty m_SizeAffectsRange;
        private SerializedProperty m_UseParticleColor;
        private static Texts s_Texts;

        public LightsModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "LightsModule", displayName)
        {
            base.m_ToolTip = "Controls light sources attached to particles.";
        }

        protected override void Init()
        {
            if (this.m_Ratio == null)
            {
                if (s_Texts == null)
                {
                    s_Texts = new Texts();
                }
                this.m_Ratio = base.GetProperty("ratio");
                this.m_RandomDistribution = base.GetProperty("randomDistribution");
                this.m_Light = base.GetProperty("light");
                this.m_UseParticleColor = base.GetProperty("color");
                this.m_SizeAffectsRange = base.GetProperty("range");
                this.m_AlphaAffectsIntensity = base.GetProperty("intensity");
                this.m_MaxLights = base.GetProperty("maxLights");
                this.m_Range = new SerializedMinMaxCurve(this, s_Texts.rangeCurve, "rangeCurve");
                this.m_Intensity = new SerializedMinMaxCurve(this, s_Texts.intensityCurve, "intensityCurve");
            }
        }

        public override void OnInspectorGUI(InitialModuleUI initial)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            ModuleUI.GUIObject(s_Texts.light, this.m_Light, new GUILayoutOption[0]);
            ModuleUI.GUIFloat(s_Texts.ratio, this.m_Ratio, new GUILayoutOption[0]);
            ModuleUI.GUIToggle(s_Texts.randomDistribution, this.m_RandomDistribution, new GUILayoutOption[0]);
            ModuleUI.GUIToggle(s_Texts.color, this.m_UseParticleColor, new GUILayoutOption[0]);
            ModuleUI.GUIToggle(s_Texts.range, this.m_SizeAffectsRange, new GUILayoutOption[0]);
            ModuleUI.GUIToggle(s_Texts.intensity, this.m_AlphaAffectsIntensity, new GUILayoutOption[0]);
            ModuleUI.GUIMinMaxCurve(s_Texts.rangeCurve, this.m_Range, new GUILayoutOption[0]);
            ModuleUI.GUIMinMaxCurve(s_Texts.intensityCurve, this.m_Intensity, new GUILayoutOption[0]);
            ModuleUI.GUIInt(s_Texts.maxLights, this.m_MaxLights, new GUILayoutOption[0]);
            if (this.m_Light.objectReferenceValue != null)
            {
                Light objectReferenceValue = (Light) this.m_Light.objectReferenceValue;
                if ((objectReferenceValue.type != LightType.Point) && (objectReferenceValue.type != LightType.Spot))
                {
                    EditorGUILayout.HelpBox(EditorGUIUtility.TextContent("Only point and spot lights are supported on particles.").text, MessageType.Warning, true);
                }
            }
        }

        private class Texts
        {
            public GUIContent color = new GUIContent("Use Particle Color", "Check the option to multiply the particle color by the light color. Otherwise, only the color of the light is used.");
            public GUIContent intensity = new GUIContent("Alpha Affects Intensity", "Multiply the intensity of the light with the alpha of the particle.");
            public GUIContent intensityCurve = new GUIContent("Intensity Multiplier", "Apply a custom multiplier to the intensity of the lights.");
            public GUIContent light = new GUIContent("Light", "Light prefab to be used for spawning particle lights.");
            public GUIContent maxLights = new GUIContent("Maximum Lights", "Limit the amount of lights the system can create. This module makes it very easy to create lots of lights, which can hurt performance.");
            public GUIContent randomDistribution = new GUIContent("Random Distribution", "Emit lights randomly, or at regular intervals.");
            public GUIContent range = new GUIContent("Size Affects Range", "Multiply the range of the light with the size of the particle.");
            public GUIContent rangeCurve = new GUIContent("Range Multiplier", "Apply a custom multiplier to the range of the lights.");
            public GUIContent ratio = new GUIContent("Ratio", "Amount of particles that have a light source attached to them.");
        }
    }
}

