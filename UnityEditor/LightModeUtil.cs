namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class LightModeUtil
    {
        private static LightModeUtil gs_ptr = null;
        private UnityEngine.Object m_cachedObject = null;
        private SerializedProperty m_enabledBakedGI = null;
        private SerializedProperty m_enableRealtimeGI = null;
        private SerializedProperty m_environmentMode = null;
        private SerializedProperty m_mixedBakeMode = null;
        private int[] m_modeVals = new int[3];
        private SerializedObject m_so = null;
        private SerializedProperty m_useShadowmask = null;
        private SerializedProperty m_workflowMode = null;
        public static readonly GUIContent s_enableBaked = EditorGUIUtility.TextContent("Baked Global Illumination|Controls whether Mixed and Baked lights will use baked Global Illumination. If enabled, Mixed lights are baked using the specified Lighting Mode and Baked lights will be completely baked and not adjustable at runtime.");
        private static readonly GUIContent[] s_modes = new GUIContent[] { new GUIContent(s_typenames[0]), new GUIContent(s_typenames[1]), new GUIContent(s_typenames[2]) };
        public static readonly string[] s_typenames = new string[] { "Realtime", "Mixed", "Baked" };

        private LightModeUtil()
        {
            this.Load();
        }

        public void AnalyzeScene(ref LightModeValidator.Stats stats)
        {
            LightModeValidator.AnalyzeScene(this.m_modeVals[0], this.m_modeVals[1], this.m_modeVals[2], this.GetAmbientLightingMode(), ref stats);
        }

        public bool AreBakedLightmapsEnabled() => 
            ((this.m_enabledBakedGI != null) && this.m_enabledBakedGI.boolValue);

        private bool CheckCachedObject()
        {
            UnityEngine.Object lightmapSettings = LightmapEditorSettings.GetLightmapSettings();
            if (lightmapSettings == null)
            {
                return false;
            }
            if (lightmapSettings == this.m_cachedObject)
            {
                this.m_so.UpdateIfRequiredOrScript();
                return true;
            }
            this.m_cachedObject = lightmapSettings;
            this.m_so = new SerializedObject(lightmapSettings);
            this.m_enableRealtimeGI = this.m_so.FindProperty("m_GISettings.m_EnableRealtimeLightmaps");
            this.m_mixedBakeMode = this.m_so.FindProperty("m_LightmapEditorSettings.m_MixedBakeMode");
            this.m_useShadowmask = this.m_so.FindProperty("m_UseShadowmask");
            this.m_enabledBakedGI = this.m_so.FindProperty("m_GISettings.m_EnableBakedLightmaps");
            this.m_workflowMode = this.m_so.FindProperty("m_GIWorkflowMode");
            this.m_environmentMode = this.m_so.FindProperty("m_GISettings.m_EnvironmentLightingMode");
            return true;
        }

        public void DrawBakedGIElement()
        {
            EditorGUILayout.PropertyField(this.m_enabledBakedGI, s_enableBaked, new GUILayoutOption[0]);
        }

        public void DrawElement(SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            int num = EditorGUILayout.IntPopup(label, property.intValue, s_modes, new int[] { 4, 1, 2 }, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = num;
            }
        }

        public void DrawElement(Rect r, SerializedProperty property, SerializedProperty dependency)
        {
            bool disabled = dependency.enumValueIndex == 3;
            using (new EditorGUI.DisabledScope(disabled))
            {
                EditorGUI.BeginChangeCheck();
                int num = EditorGUI.IntPopup(r, property.intValue, s_modes, new int[] { 4, 1, 2 });
                if (EditorGUI.EndChangeCheck())
                {
                    property.intValue = num;
                }
            }
        }

        public bool Flush() => 
            this.m_so.ApplyModifiedProperties();

        public static LightModeUtil Get()
        {
            if (gs_ptr == null)
            {
                gs_ptr = new LightModeUtil();
            }
            return gs_ptr;
        }

        public int GetAmbientLightingMode()
        {
            int num;
            this.GetAmbientLightingMode(out num);
            return num;
        }

        public bool GetAmbientLightingMode(out int mode)
        {
            if (this.AreBakedLightmapsEnabled() && this.IsRealtimeGIEnabled())
            {
                mode = this.m_environmentMode.intValue;
                return true;
            }
            mode = !this.AreBakedLightmapsEnabled() ? 0 : 1;
            return false;
        }

        public void GetModes(out int realtimeMode, out int mixedMode)
        {
            realtimeMode = this.m_modeVals[0];
            mixedMode = this.m_modeVals[1];
        }

        public void GetProps(out SerializedProperty o_enableRealtimeGI, out SerializedProperty o_enableBakedGI, out SerializedProperty o_mixedBakeMode, out SerializedProperty o_useShadowMask)
        {
            o_enableRealtimeGI = this.m_enableRealtimeGI;
            o_enableBakedGI = this.m_enabledBakedGI;
            o_mixedBakeMode = this.m_mixedBakeMode;
            o_useShadowMask = this.m_useShadowmask;
        }

        public bool IsAnyGIEnabled() => 
            (this.IsRealtimeGIEnabled() || this.AreBakedLightmapsEnabled());

        public bool IsRealtimeGIEnabled() => 
            ((this.m_enableRealtimeGI != null) && this.m_enableRealtimeGI.boolValue);

        public bool IsSubtractiveModeEnabled() => 
            (this.m_modeVals[1] == 1);

        public bool IsWorkflowAuto() => 
            (this.m_workflowMode.intValue == 0);

        public bool Load()
        {
            if (!this.CheckCachedObject())
            {
                return false;
            }
            int realtimeMode = !this.m_enableRealtimeGI.boolValue ? 1 : 0;
            int intValue = this.m_mixedBakeMode.intValue;
            this.Update(realtimeMode, intValue);
            return true;
        }

        public void SetWorkflow(bool bAutoEnabled)
        {
            this.m_workflowMode.intValue = !bAutoEnabled ? 1 : 0;
        }

        public void Store(int realtimeMode, int mixedMode)
        {
            this.Update(realtimeMode, mixedMode);
            if (this.CheckCachedObject())
            {
                this.m_enableRealtimeGI.boolValue = this.m_modeVals[0] == 0;
                this.m_mixedBakeMode.intValue = this.m_modeVals[1];
                this.m_useShadowmask.boolValue = this.m_modeVals[1] == 2;
            }
        }

        private void Update(int realtimeMode, int mixedMode)
        {
            this.m_modeVals[0] = realtimeMode;
            this.m_modeVals[1] = mixedMode;
            this.m_modeVals[2] = 0;
        }

        internal enum LightmapMixedBakeMode
        {
            IndirectOnly,
            LightmapsWithSubtractiveShadows,
            ShadowmaskAndIndirect
        }
    }
}

