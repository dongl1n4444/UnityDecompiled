namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class VelocityModuleUI : ModuleUI
    {
        private SerializedProperty m_InWorldSpace;
        private SerializedMinMaxCurve m_X;
        private SerializedMinMaxCurve m_Y;
        private SerializedMinMaxCurve m_Z;
        private static Texts s_Texts;

        public VelocityModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "VelocityModule", displayName)
        {
            base.m_ToolTip = "Controls the velocity of each particle during its lifetime.";
        }

        protected override void Init()
        {
            if (this.m_X == null)
            {
                if (s_Texts == null)
                {
                    s_Texts = new Texts();
                }
                this.m_X = new SerializedMinMaxCurve(this, s_Texts.x, "x", ModuleUI.kUseSignedRange);
                this.m_Y = new SerializedMinMaxCurve(this, s_Texts.y, "y", ModuleUI.kUseSignedRange);
                this.m_Z = new SerializedMinMaxCurve(this, s_Texts.z, "z", ModuleUI.kUseSignedRange);
                this.m_InWorldSpace = base.GetProperty("inWorldSpace");
            }
        }

        public override void OnInspectorGUI(InitialModuleUI initial)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            base.GUITripleMinMaxCurve(GUIContent.none, s_Texts.x, this.m_X, s_Texts.y, this.m_Y, s_Texts.z, this.m_Z, null, new GUILayoutOption[0]);
            ModuleUI.GUIBoolAsPopup(s_Texts.space, this.m_InWorldSpace, s_Texts.spaces, new GUILayoutOption[0]);
        }

        public override void UpdateCullingSupportedString(ref string text)
        {
            this.Init();
            string failureReason = string.Empty;
            if (!this.m_X.SupportsProcedural(ref failureReason))
            {
                text = text + "\nVelocity over Lifetime module curve X: " + failureReason;
            }
            failureReason = string.Empty;
            if (!this.m_Y.SupportsProcedural(ref failureReason))
            {
                text = text + "\nVelocity over Lifetime module curve Y: " + failureReason;
            }
            failureReason = string.Empty;
            if (!this.m_Z.SupportsProcedural(ref failureReason))
            {
                text = text + "\nVelocity over Lifetime module curve Z: " + failureReason;
            }
        }

        private class Texts
        {
            public GUIContent space = EditorGUIUtility.TextContent("Space|Specifies if the velocity values are in local space (rotated with the transform) or world space.");
            public string[] spaces = new string[] { "Local", "World" };
            public GUIContent x = EditorGUIUtility.TextContent("X");
            public GUIContent y = EditorGUIUtility.TextContent("Y");
            public GUIContent z = EditorGUIUtility.TextContent("Z");
        }
    }
}

