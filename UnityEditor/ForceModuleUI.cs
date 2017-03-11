namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class ForceModuleUI : ModuleUI
    {
        private SerializedProperty m_InWorldSpace;
        private SerializedProperty m_RandomizePerFrame;
        private SerializedMinMaxCurve m_X;
        private SerializedMinMaxCurve m_Y;
        private SerializedMinMaxCurve m_Z;
        private static Texts s_Texts;

        public ForceModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "ForceModule", displayName)
        {
            base.m_ToolTip = "Controls the force of each particle during its lifetime.";
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
                this.m_RandomizePerFrame = base.GetProperty("randomizePerFrame");
                this.m_InWorldSpace = base.GetProperty("inWorldSpace");
            }
        }

        public override void OnInspectorGUI(InitialModuleUI initial)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            MinMaxCurveState state = this.m_X.state;
            base.GUITripleMinMaxCurve(GUIContent.none, s_Texts.x, this.m_X, s_Texts.y, this.m_Y, s_Texts.z, this.m_Z, this.m_RandomizePerFrame, new GUILayoutOption[0]);
            ModuleUI.GUIBoolAsPopup(s_Texts.space, this.m_InWorldSpace, s_Texts.spaces, new GUILayoutOption[0]);
            using (new EditorGUI.DisabledScope((state != MinMaxCurveState.k_TwoScalars) && (state != MinMaxCurveState.k_TwoCurves)))
            {
                ModuleUI.GUIToggle(s_Texts.randomizePerFrame, this.m_RandomizePerFrame, new GUILayoutOption[0]);
            }
        }

        public override void UpdateCullingSupportedString(ref string text)
        {
            this.Init();
            string failureReason = string.Empty;
            if (!this.m_X.SupportsProcedural(ref failureReason))
            {
                text = text + "\nForce over Lifetime module curve X: " + failureReason;
            }
            failureReason = string.Empty;
            if (!this.m_Y.SupportsProcedural(ref failureReason))
            {
                text = text + "\nForce over Lifetime module curve Y: " + failureReason;
            }
            failureReason = string.Empty;
            if (!this.m_Z.SupportsProcedural(ref failureReason))
            {
                text = text + "\nForce over Lifetime module curve Z: " + failureReason;
            }
            if (this.m_RandomizePerFrame.boolValue)
            {
                text = text + "\nRandomize is enabled in the Force over Lifetime module.";
            }
        }

        private class Texts
        {
            public GUIContent randomizePerFrame = EditorGUIUtility.TextContent("Randomize|Randomize force every frame. Only available when using random between two constants or random between two curves.");
            public GUIContent space = EditorGUIUtility.TextContent("Space|Specifies if the force values are in local space (rotated with the transform) or world space.");
            public string[] spaces = new string[] { "Local", "World" };
            public GUIContent x = EditorGUIUtility.TextContent("X");
            public GUIContent y = EditorGUIUtility.TextContent("Y");
            public GUIContent z = EditorGUIUtility.TextContent("Z");
        }
    }
}

