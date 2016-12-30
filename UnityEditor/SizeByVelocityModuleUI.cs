namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class SizeByVelocityModuleUI : ModuleUI
    {
        private SerializedProperty m_Range;
        private SerializedProperty m_SeparateAxes;
        private SerializedMinMaxCurve m_X;
        private SerializedMinMaxCurve m_Y;
        private SerializedMinMaxCurve m_Z;
        private static Texts s_Texts;

        public SizeByVelocityModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "SizeBySpeedModule", displayName)
        {
            base.m_ToolTip = "Controls the size of each particle based on its speed.";
        }

        protected override void Init()
        {
            if (this.m_X == null)
            {
                if (s_Texts == null)
                {
                    s_Texts = new Texts();
                }
                this.m_SeparateAxes = base.GetProperty("separateAxes");
                this.m_Range = base.GetProperty("range");
                this.m_X = new SerializedMinMaxCurve(this, s_Texts.x, "curve");
                this.m_X.m_AllowConstant = false;
                this.m_Y = new SerializedMinMaxCurve(this, s_Texts.y, "y", false, false, this.m_SeparateAxes.boolValue);
                this.m_Y.m_AllowConstant = false;
                this.m_Z = new SerializedMinMaxCurve(this, s_Texts.z, "z", false, false, this.m_SeparateAxes.boolValue);
                this.m_Z.m_AllowConstant = false;
            }
        }

        public override void OnInspectorGUI(InitialModuleUI initial)
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
                    this.m_X.RemoveCurveFromEditor();
                }
                else
                {
                    this.m_X.RemoveCurveFromEditor();
                    this.m_Y.RemoveCurveFromEditor();
                    this.m_Z.RemoveCurveFromEditor();
                }
            }
            MinMaxCurveState state = this.m_X.state;
            this.m_Y.state = state;
            this.m_Z.state = state;
            MinMaxCurveState state2 = this.m_Z.state;
            if (flag)
            {
                this.m_X.m_DisplayName = s_Texts.x;
                base.GUITripleMinMaxCurve(GUIContent.none, s_Texts.x, this.m_X, s_Texts.y, this.m_Y, s_Texts.z, this.m_Z, null, new GUILayoutOption[0]);
            }
            else
            {
                this.m_X.m_DisplayName = s_Texts.size;
                ModuleUI.GUIMinMaxCurve(s_Texts.size, this.m_X, new GUILayoutOption[0]);
            }
            using (new EditorGUI.DisabledScope((state2 == MinMaxCurveState.k_Scalar) || (state2 == MinMaxCurveState.k_TwoScalars)))
            {
                ModuleUI.GUIMinMaxRange(s_Texts.velocityRange, this.m_Range, new GUILayoutOption[0]);
            }
        }

        private class Texts
        {
            public GUIContent separateAxes = new GUIContent("Separate Axes", "If enabled, you can control the angular velocity limit separately for each axis.");
            public GUIContent size = EditorGUIUtility.TextContent("Size|Controls the size of each particle based on its speed.");
            public GUIContent velocityRange = EditorGUIUtility.TextContent("Speed Range|Remaps speed in the defined range to a size.");
            public GUIContent x = new GUIContent("X");
            public GUIContent y = new GUIContent("Y");
            public GUIContent z = new GUIContent("Z");
        }
    }
}

