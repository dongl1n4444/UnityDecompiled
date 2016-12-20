namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class RotationByVelocityModuleUI : ModuleUI
    {
        private SerializedProperty m_Range;
        private SerializedProperty m_SeparateAxes;
        private SerializedMinMaxCurve m_X;
        private SerializedMinMaxCurve m_Y;
        private SerializedMinMaxCurve m_Z;
        private static Texts s_Texts;

        public RotationByVelocityModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "RotationBySpeedModule", displayName)
        {
            base.m_ToolTip = "Controls the angular velocity of each particle based on its speed.";
        }

        protected override void Init()
        {
            if (this.m_Z == null)
            {
                if (s_Texts == null)
                {
                    s_Texts = new Texts();
                }
                this.m_X = new SerializedMinMaxCurve(this, s_Texts.x, "x", ModuleUI.kUseSignedRange);
                this.m_Y = new SerializedMinMaxCurve(this, s_Texts.y, "y", ModuleUI.kUseSignedRange);
                this.m_Z = new SerializedMinMaxCurve(this, s_Texts.z, "curve", ModuleUI.kUseSignedRange);
                this.m_X.m_RemapValue = 57.29578f;
                this.m_Y.m_RemapValue = 57.29578f;
                this.m_Z.m_RemapValue = 57.29578f;
                this.m_SeparateAxes = base.GetProperty("separateAxes");
                this.m_Range = base.GetProperty("range");
            }
        }

        public override void OnInspectorGUI(ParticleSystem s)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            EditorGUI.BeginChangeCheck();
            bool flag = ModuleUI.GUIToggle(s_Texts.separateAxes, this.m_SeparateAxes, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                if (flag)
                {
                    this.m_Z.RemoveCurveFromEditor();
                }
                else
                {
                    this.m_X.RemoveCurveFromEditor();
                    this.m_Y.RemoveCurveFromEditor();
                    this.m_Z.RemoveCurveFromEditor();
                }
            }
            MinMaxCurveState state = this.m_Z.state;
            this.m_Y.state = state;
            this.m_X.state = state;
            MinMaxCurveState state2 = this.m_Z.state;
            if (flag)
            {
                this.m_Z.m_DisplayName = s_Texts.z;
                base.GUITripleMinMaxCurve(GUIContent.none, s_Texts.x, this.m_X, s_Texts.y, this.m_Y, s_Texts.z, this.m_Z, null, new GUILayoutOption[0]);
            }
            else
            {
                this.m_Z.m_DisplayName = s_Texts.rotation;
                ModuleUI.GUIMinMaxCurve(s_Texts.rotation, this.m_Z, new GUILayoutOption[0]);
            }
            using (new EditorGUI.DisabledScope((state2 == MinMaxCurveState.k_Scalar) || (state2 == MinMaxCurveState.k_TwoScalars)))
            {
                ModuleUI.GUIMinMaxRange(s_Texts.velocityRange, this.m_Range, new GUILayoutOption[0]);
            }
        }

        public override void UpdateCullingSupportedString(ref string text)
        {
            text = text + "\n\tRotation by Speed is enabled.";
        }

        private class Texts
        {
            public GUIContent rotation = EditorGUIUtility.TextContent("Angular Velocity|Controls the angular velocity of each particle based on its speed.");
            public GUIContent separateAxes = EditorGUIUtility.TextContent("Separate Axes|If enabled, you can control the angular velocity limit separately for each axis.");
            public GUIContent velocityRange = EditorGUIUtility.TextContent("Speed Range|Maps the speed to a value along the curve, when using one of the curve modes.");
            public GUIContent x = EditorGUIUtility.TextContent("X");
            public GUIContent y = EditorGUIUtility.TextContent("Y");
            public GUIContent z = EditorGUIUtility.TextContent("Z");
        }
    }
}

