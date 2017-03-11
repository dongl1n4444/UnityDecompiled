namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class TrailModuleUI : ModuleUI
    {
        private SerializedMinMaxGradient m_ColorOverLifetime;
        private SerializedMinMaxGradient m_ColorOverTrail;
        private SerializedProperty m_DieWithParticles;
        private SerializedProperty m_InheritParticleColor;
        private SerializedMinMaxCurve m_Lifetime;
        private SerializedProperty m_MinVertexDistance;
        private SerializedProperty m_Ratio;
        private SerializedProperty m_SizeAffectsLifetime;
        private SerializedProperty m_SizeAffectsWidth;
        private SerializedProperty m_TextureMode;
        private SerializedMinMaxCurve m_WidthOverTrail;
        private SerializedProperty m_WorldSpace;
        private static Texts s_Texts;

        public TrailModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "TrailModule", displayName)
        {
            base.m_ToolTip = "Attach trails to the particles.";
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
                this.m_Lifetime = new SerializedMinMaxCurve(this, s_Texts.lifetime, "lifetime");
                this.m_MinVertexDistance = base.GetProperty("minVertexDistance");
                this.m_TextureMode = base.GetProperty("textureMode");
                this.m_WorldSpace = base.GetProperty("worldSpace");
                this.m_DieWithParticles = base.GetProperty("dieWithParticles");
                this.m_SizeAffectsWidth = base.GetProperty("sizeAffectsWidth");
                this.m_SizeAffectsLifetime = base.GetProperty("sizeAffectsLifetime");
                this.m_InheritParticleColor = base.GetProperty("inheritParticleColor");
                this.m_ColorOverLifetime = new SerializedMinMaxGradient(this, "colorOverLifetime");
                this.m_WidthOverTrail = new SerializedMinMaxCurve(this, s_Texts.widthOverTrail, "widthOverTrail");
                this.m_ColorOverTrail = new SerializedMinMaxGradient(this, "colorOverTrail");
            }
        }

        public override void OnInspectorGUI(InitialModuleUI initial)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            ModuleUI.GUIFloat(s_Texts.ratio, this.m_Ratio, new GUILayoutOption[0]);
            ModuleUI.GUIMinMaxCurve(s_Texts.lifetime, this.m_Lifetime, new GUILayoutOption[0]);
            ModuleUI.GUIFloat(s_Texts.minVertexDistance, this.m_MinVertexDistance, new GUILayoutOption[0]);
            ModuleUI.GUIPopup(s_Texts.textureMode, this.m_TextureMode, s_Texts.textureModeOptions, new GUILayoutOption[0]);
            ModuleUI.GUIToggle(s_Texts.worldSpace, this.m_WorldSpace, new GUILayoutOption[0]);
            ModuleUI.GUIToggle(s_Texts.dieWithParticles, this.m_DieWithParticles, new GUILayoutOption[0]);
            ModuleUI.GUIToggle(s_Texts.sizeAffectsWidth, this.m_SizeAffectsWidth, new GUILayoutOption[0]);
            ModuleUI.GUIToggle(s_Texts.sizeAffectsLifetime, this.m_SizeAffectsLifetime, new GUILayoutOption[0]);
            ModuleUI.GUIToggle(s_Texts.inheritParticleColor, this.m_InheritParticleColor, new GUILayoutOption[0]);
            base.GUIMinMaxGradient(s_Texts.colorOverLifetime, this.m_ColorOverLifetime, false, new GUILayoutOption[0]);
            ModuleUI.GUIMinMaxCurve(s_Texts.widthOverTrail, this.m_WidthOverTrail, new GUILayoutOption[0]);
            base.GUIMinMaxGradient(s_Texts.colorOverTrail, this.m_ColorOverTrail, false, new GUILayoutOption[0]);
            foreach (ParticleSystem system in base.m_ParticleSystemUI.m_ParticleSystems)
            {
                if (system.trails.enabled)
                {
                    ParticleSystemRenderer component = system.GetComponent<ParticleSystemRenderer>();
                    if ((component != null) && (component.trailMaterial == null))
                    {
                        EditorGUILayout.HelpBox("Assign a Trail Material in the Renderer Module", MessageType.Warning, true);
                        break;
                    }
                }
            }
        }

        public override void UpdateCullingSupportedString(ref string text)
        {
            text = text + "\nTrails module is enabled.";
        }

        private class Texts
        {
            public GUIContent colorOverLifetime = EditorGUIUtility.TextContent("Color over Lifetime|The color of the trails during the lifetime of the particle they are attached to.");
            public GUIContent colorOverTrail = EditorGUIUtility.TextContent("Color over Trail|Select a color for the trail from its start to end vertex.");
            public GUIContent dieWithParticles = EditorGUIUtility.TextContent("Die with Particles|The trails will disappear when their owning particles die.");
            public GUIContent inheritParticleColor = EditorGUIUtility.TextContent("Inherit Particle Color|The trails will use the particle color as their base color.");
            public GUIContent lifetime = EditorGUIUtility.TextContent("Lifetime|How long each trail will last, relative to the life of the particle.");
            public GUIContent minVertexDistance = EditorGUIUtility.TextContent("Minimum Vertex Distance|The minimum distance each trail can travel before adding a new vertex.");
            public GUIContent ratio = new GUIContent("Ratio", "Choose what proportion of particles will receive a trail.");
            public GUIContent sizeAffectsLifetime = EditorGUIUtility.TextContent("Size affects Lifetime|The trails will use the particle size to control their lifetime.");
            public GUIContent sizeAffectsWidth = EditorGUIUtility.TextContent("Size affects Width|The trails will use the particle size to control their width.");
            public GUIContent textureMode = EditorGUIUtility.TextContent("Texture Mode|Should the U coordinate be stretched or tiled?");
            public string[] textureModeOptions = new string[] { "Stretch", "Tile" };
            public GUIContent widthOverTrail = EditorGUIUtility.TextContent("Width over Trail|Select a width for the trail from its start to end vertex.");
            public GUIContent worldSpace = EditorGUIUtility.TextContent("World Space|Trail points will be dropped in world space, even if the particle system is simulating in local space.");
        }
    }
}

