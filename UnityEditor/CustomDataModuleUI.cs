namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class CustomDataModuleUI : ModuleUI
    {
        private const int k_NumChannelsPerStream = 4;
        private const int k_NumCustomDataStreams = 2;
        private SerializedMinMaxGradient[] m_Colors;
        private SerializedProperty[] m_Modes;
        private SerializedProperty[] m_VectorComponentCount;
        private SerializedMinMaxCurve[,] m_Vectors;
        private static Texts s_Texts;

        public CustomDataModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "CustomDataModule", displayName)
        {
            this.m_Modes = new SerializedProperty[2];
            this.m_VectorComponentCount = new SerializedProperty[2];
            this.m_Vectors = new SerializedMinMaxCurve[2, 4];
            this.m_Colors = new SerializedMinMaxGradient[2];
            base.m_ToolTip = "Configure custom data to be read in scripts or shaders. Use GetCustomParticleData from script, or send to shaders using the Custom Vertex Streams.";
        }

        protected override void Init()
        {
            if (this.m_Modes[0] == null)
            {
                if (s_Texts == null)
                {
                    s_Texts = new Texts();
                }
                for (int i = 0; i < 2; i++)
                {
                    this.m_Modes[i] = base.GetProperty("mode" + i);
                    this.m_VectorComponentCount[i] = base.GetProperty("vectorComponentCount" + i);
                    this.m_Colors[i] = new SerializedMinMaxGradient(this, "color" + i);
                    for (int j = 0; j < 4; j++)
                    {
                        object[] objArray1 = new object[] { "vector", i, "_", j };
                        this.m_Vectors[i, j] = new SerializedMinMaxCurve(this, s_Texts.components[j], string.Concat(objArray1), ModuleUI.kUseSignedRange);
                    }
                }
            }
        }

        public override void OnInspectorGUI(InitialModuleUI initial)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            for (int i = 0; i < 2; i++)
            {
                GUILayout.BeginVertical("Custom" + (i + 1), GUI.skin.window, new GUILayoutOption[0]);
                switch (((Mode) ModuleUI.GUIPopup(s_Texts.mode, this.m_Modes[i], s_Texts.modes, new GUILayoutOption[0])))
                {
                    case Mode.Vector:
                    {
                        int num2 = Mathf.Min(ModuleUI.GUIInt(s_Texts.vectorComponentCount, this.m_VectorComponentCount[i], new GUILayoutOption[0]), 4);
                        for (int j = 0; j < num2; j++)
                        {
                            ModuleUI.GUIMinMaxCurve(s_Texts.components[j], this.m_Vectors[i, j], new GUILayoutOption[0]);
                        }
                        break;
                    }
                    case Mode.Color:
                        base.GUIMinMaxGradient(s_Texts.color, this.m_Colors[i], true, new GUILayoutOption[0]);
                        break;
                }
                GUILayout.EndVertical();
            }
        }

        private enum Mode
        {
            Disabled,
            Vector,
            Color
        }

        private class Texts
        {
            public GUIContent color = EditorGUIUtility.TextContent("Color");
            public GUIContent[] components = new GUIContent[] { EditorGUIUtility.TextContent("X"), EditorGUIUtility.TextContent("Y"), EditorGUIUtility.TextContent("Z"), EditorGUIUtility.TextContent("W") };
            public GUIContent mode = EditorGUIUtility.TextContent("Mode|Select the type of data to populate this stream with.");
            public string[] modes = new string[] { "Disabled", "Vector", "Color" };
            public GUIContent vectorComponentCount = EditorGUIUtility.TextContent("Number of Components|How many of the components (XYZW) to fill.");
        }
    }
}

