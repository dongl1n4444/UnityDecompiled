namespace UnityEditor
{
    using System;
    using UnityEngine;
    using UnityEngine.Rendering;

    internal class UVModuleUI : ModuleUI
    {
        private SerializedProperty m_AnimationType;
        private SerializedProperty m_Cycles;
        private SerializedProperty m_FlipU;
        private SerializedProperty m_FlipV;
        private SerializedMinMaxCurve m_FrameOverTime;
        private SerializedProperty m_RandomRow;
        private SerializedProperty m_RowIndex;
        private SerializedMinMaxCurve m_StartFrame;
        private SerializedProperty m_TilesX;
        private SerializedProperty m_TilesY;
        private SerializedProperty m_UVChannelMask;
        private static Texts s_Texts;

        public UVModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "UVModule", displayName)
        {
            base.m_ToolTip = "Particle UV animation. This allows you to specify a texture sheet (a texture with multiple tiles/sub frames) and animate or randomize over it per particle.";
        }

        protected override void Init()
        {
            if (this.m_TilesX == null)
            {
                if (s_Texts == null)
                {
                    s_Texts = new Texts();
                }
                this.m_FrameOverTime = new SerializedMinMaxCurve(this, s_Texts.frameOverTime, "frameOverTime");
                this.m_StartFrame = new SerializedMinMaxCurve(this, s_Texts.startFrame, "startFrame");
                this.m_StartFrame.m_AllowCurves = false;
                this.m_TilesX = base.GetProperty("tilesX");
                this.m_TilesY = base.GetProperty("tilesY");
                this.m_AnimationType = base.GetProperty("animationType");
                this.m_Cycles = base.GetProperty("cycles");
                this.m_RandomRow = base.GetProperty("randomRow");
                this.m_RowIndex = base.GetProperty("rowIndex");
                this.m_UVChannelMask = base.GetProperty("uvChannelMask");
                this.m_FlipU = base.GetProperty("flipU");
                this.m_FlipV = base.GetProperty("flipV");
            }
        }

        public override void OnInspectorGUI(ParticleSystem s)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            ModuleUI.GUIIntDraggableX2(s_Texts.tiles, s_Texts.tilesX, this.m_TilesX, s_Texts.tilesY, this.m_TilesY, new GUILayoutOption[0]);
            int num = ModuleUI.GUIPopup(s_Texts.animation, this.m_AnimationType, s_Texts.types, new GUILayoutOption[0]);
            if (num == 1)
            {
                ModuleUI.GUIToggle(s_Texts.randomRow, this.m_RandomRow, new GUILayoutOption[0]);
                if (!this.m_RandomRow.boolValue)
                {
                    ModuleUI.GUIInt(s_Texts.row, this.m_RowIndex, new GUILayoutOption[0]);
                }
            }
            switch (num)
            {
                case 1:
                    this.m_FrameOverTime.m_RemapValue = this.m_TilesX.intValue;
                    this.m_StartFrame.m_RemapValue = this.m_TilesX.intValue;
                    break;

                case 0:
                    this.m_FrameOverTime.m_RemapValue = this.m_TilesX.intValue * this.m_TilesY.intValue;
                    this.m_StartFrame.m_RemapValue = this.m_TilesX.intValue * this.m_TilesY.intValue;
                    break;
            }
            ModuleUI.GUIMinMaxCurve(s_Texts.frameOverTime, this.m_FrameOverTime, new GUILayoutOption[0]);
            ModuleUI.GUIMinMaxCurve(s_Texts.startFrame, this.m_StartFrame, new GUILayoutOption[0]);
            ModuleUI.GUIFloat(s_Texts.cycles, this.m_Cycles, new GUILayoutOption[0]);
            ParticleSystemRenderer component = base.m_ParticleSystemUI.m_ParticleSystem.GetComponent<ParticleSystemRenderer>();
            using (new EditorGUI.DisabledScope((component != null) && (component.renderMode == ParticleSystemRenderMode.Mesh)))
            {
                ModuleUI.GUIFloat(s_Texts.flipU, this.m_FlipU, new GUILayoutOption[0]);
                ModuleUI.GUIFloat(s_Texts.flipV, this.m_FlipV, new GUILayoutOption[0]);
            }
            this.m_UVChannelMask.intValue = (int) ((UVChannelFlags) ModuleUI.GUIEnumMask(s_Texts.uvChannelMask, (UVChannelFlags) this.m_UVChannelMask.intValue, new GUILayoutOption[0]));
        }

        private enum AnimationType
        {
            WholeSheet,
            SingleRow
        }

        private class Texts
        {
            public GUIContent animation = EditorGUIUtility.TextContent("Animation|Specifies the animation type: Whole Sheet or Single Row. Whole Sheet will animate over the whole texture sheet from left to right, top to bottom. Single Row will animate a single row in the sheet from left to right.");
            public GUIContent cycles = EditorGUIUtility.TextContent("Cycles|Specifies how many times the animation will loop during the lifetime of the particle.");
            public GUIContent flipU = EditorGUIUtility.TextContent("Flip U|Cause some particle texture mapping to be flipped horizontally. (Set between 0 and 1, where a higher value causes more to flip)");
            public GUIContent flipV = EditorGUIUtility.TextContent("Flip V|Cause some particle texture mapping to be flipped vertically. (Set between 0 and 1, where a higher value causes more to flip)");
            public GUIContent frame = EditorGUIUtility.TextContent("Frame|The frame in the sheet which will be used.");
            public GUIContent frameOverTime = EditorGUIUtility.TextContent("Frame over Time|Controls the uv animation frame of each particle over its lifetime. On the horisontal axis you will find the lifetime. On the vertical axis you will find the sheet index.");
            public GUIContent randomRow = EditorGUIUtility.TextContent("Random Row|If enabled, the animated row will be chosen randomly.");
            public GUIContent row = EditorGUIUtility.TextContent("Row|The row in the sheet which will be played.");
            public GUIContent startFrame = EditorGUIUtility.TextContent("Start Frame|Phase the animation, so it starts on a frame other than 0.");
            public GUIContent tiles = EditorGUIUtility.TextContent("Tiles|Defines the tiling of the texture.");
            public GUIContent tilesX = EditorGUIUtility.TextContent("X");
            public GUIContent tilesY = EditorGUIUtility.TextContent("Y");
            public string[] types = new string[] { "Whole Sheet", "Single Row" };
            public GUIContent uvChannelMask = EditorGUIUtility.TextContent("Enabled UV Channels|Specifies which UV channels will be animated.");
        }
    }
}

