namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class LightModeUtil
    {
        private static LightModeUtil gs_ptr = null;
        private const string kUpdateStatistics = "UpdateStatistics";
        private Object m_cachedObject = null;
        private SerializedProperty m_enabledBakedGI = null;
        private SerializedProperty m_enableRealtimeGI = null;
        private SerializedProperty m_environmentMode = null;
        private int[] m_modeVals = new int[3];
        private SerializedProperty m_shadowMaskMode = null;
        private bool m_shouldUpdateStatistics = SessionState.GetBool("UpdateStatistics", false);
        private SerializedObject m_so = null;
        private SerializedProperty m_stationaryBakeMode = null;
        private SerializedProperty m_workflowMode = null;
        public static readonly GUIContent s_dontUpdateStatistics = EditorGUIUtility.TextContent("Don't update statistics|Prevents statistics from being updated during play mode to improve performance.");
        public static readonly GUIContent s_enableBaked = EditorGUIUtility.TextContent("Use Baked Global Illumination|Controls whether Stationary and Static lights use baked global illumination. If enabled, Stationary lights will be baked based on the Stationary Lighting Mode allowing for more flexible lighting while Static lights will be completely baked and not adjustable at runtime.");
        private static readonly GUIContent[] s_modes = new GUIContent[] { new GUIContent(s_typenames[0]), new GUIContent(s_typenames[1]), new GUIContent(s_typenames[2]) };
        public static readonly string[] s_typenames = new string[] { "Dynamic", "Stationary", "Static" };

        private LightModeUtil()
        {
            this.Load();
        }

        public void AnalyzeScene(ref LightModeValidator.Stats stats)
        {
            LightModeValidator.AnalyzeScene(this.m_modeVals[0], this.m_modeVals[1], this.m_modeVals[2], this.GetAmbientMode(), ref stats);
        }

        public bool AreBakedLightmapsEnabled() => 
            ((this.m_enabledBakedGI != null) && this.m_enabledBakedGI.boolValue);

        private bool CheckCachedObject()
        {
            Object lightmapSettings = LightmapEditorSettings.GetLightmapSettings();
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
            this.m_stationaryBakeMode = this.m_so.FindProperty("m_LightmapEditorSettings.m_StationaryBakeMode");
            this.m_shadowMaskMode = this.m_so.FindProperty("m_ShadowMaskMode");
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

        public void DrawUIFlags()
        {
            bool flag = !EditorGUILayout.Toggle(s_dontUpdateStatistics, !this.m_shouldUpdateStatistics, new GUILayoutOption[0]);
            if (flag != this.m_shouldUpdateStatistics)
            {
                SessionState.SetBool("UpdateStatistics", flag);
            }
            this.m_shouldUpdateStatistics = flag;
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

        public int GetAmbientMode()
        {
            int num;
            this.GetAmbientMode(out num);
            return num;
        }

        public bool GetAmbientMode(out int mode)
        {
            if (this.AreBakedLightmapsEnabled() && this.IsRealtimeGIEnabled())
            {
                mode = this.m_environmentMode.intValue;
                return true;
            }
            mode = !this.AreBakedLightmapsEnabled() ? 0 : 1;
            return false;
        }

        public void GetModes(out int dynamicMode, out int stationaryMode)
        {
            dynamicMode = this.m_modeVals[0];
            stationaryMode = this.m_modeVals[1];
        }

        public void GetProps(out SerializedProperty o_enableRealtimeGI, out SerializedProperty o_enableBakedGI, out SerializedProperty o_stationaryBakeMode, out SerializedProperty o_shadowMaskMode)
        {
            o_enableRealtimeGI = this.m_enableRealtimeGI;
            o_enableBakedGI = this.m_enabledBakedGI;
            o_stationaryBakeMode = this.m_stationaryBakeMode;
            o_shadowMaskMode = this.m_shadowMaskMode;
        }

        public bool IsAnyGIEnabled() => 
            (this.IsRealtimeGIEnabled() || this.AreBakedLightmapsEnabled());

        public bool IsRealtimeGIEnabled() => 
            ((this.m_enableRealtimeGI != null) && this.m_enableRealtimeGI.boolValue);

        public bool IsSubtractiveModeEnabled() => 
            (this.m_modeVals[1] == 3);

        public bool IsWorkflowAuto() => 
            (this.m_workflowMode.intValue == 0);

        public bool Load()
        {
            if (!this.CheckCachedObject())
            {
                return false;
            }
            int dynamicMode = !this.m_enableRealtimeGI.boolValue ? 1 : 0;
            int stationaryMode = (int) this.MapStationarySettingsToRowIndex((LightmapStationaryBakeMode) this.m_stationaryBakeMode.intValue, (ShadowMaskMode) this.m_shadowMaskMode.intValue);
            this.Update(dynamicMode, stationaryMode);
            return true;
        }

        public StationaryLightModeRowIndex MapStationarySettingsToRowIndex(LightmapStationaryBakeMode bakeMode, ShadowMaskMode maskMode)
        {
            if (bakeMode != LightmapStationaryBakeMode.IndirectOnly)
            {
                if (bakeMode == LightmapStationaryBakeMode.ShadowMaskAndIndirect)
                {
                    return ((maskMode != ShadowMaskMode.PastShadowDistance) ? StationaryLightModeRowIndex.ShadowMaskAllTheWay : StationaryLightModeRowIndex.ShadowMaskPastShadowDistance);
                }
                if (bakeMode == LightmapStationaryBakeMode.LightmapsWithSubtractiveShadows)
                {
                    return StationaryLightModeRowIndex.Subtractive;
                }
            }
            else
            {
                return StationaryLightModeRowIndex.IndirectOnly;
            }
            Debug.LogError("Unkown stationary bake mode in LightModeUtil.MapSettings()");
            return StationaryLightModeRowIndex.IndirectOnly;
        }

        public void SetWorkflow(bool bAutoEnabled)
        {
            this.m_workflowMode.intValue = !bAutoEnabled ? 1 : 0;
        }

        public void Store(int dynamicMode, int stationaryMode)
        {
            this.Update(dynamicMode, stationaryMode);
            if (this.CheckCachedObject())
            {
                this.m_enableRealtimeGI.boolValue = this.m_modeVals[0] == 0;
                LightmapStationaryBakeMode[] modeArray = new LightmapStationaryBakeMode[] { LightmapStationaryBakeMode.IndirectOnly };
                this.m_stationaryBakeMode.intValue = (int) modeArray[this.m_modeVals[1]];
                ShadowMaskMode[] modeArray1 = new ShadowMaskMode[4];
                modeArray1[1] = ShadowMaskMode.PastShadowDistance;
                modeArray1[2] = ShadowMaskMode.Full;
                ShadowMaskMode[] modeArray2 = modeArray1;
                this.m_shadowMaskMode.intValue = (int) modeArray2[this.m_modeVals[1]];
            }
        }

        private void Update(int dynamicMode, int stationaryMode)
        {
            this.m_modeVals[0] = dynamicMode;
            this.m_modeVals[1] = stationaryMode;
            this.m_modeVals[2] = 0;
        }

        public bool UpdateStatistics() => 
            (this.m_shouldUpdateStatistics || !EditorApplication.isPlayingOrWillChangePlaymode);

        internal enum LightmapStationaryBakeMode
        {
            IndirectOnly,
            LightmapsWithSubtractiveShadows,
            DirectAndIndirect,
            ShadowMaskAndIndirect
        }

        internal enum ShadowMaskMode
        {
            None,
            Full,
            PastShadowDistance
        }

        internal enum StationaryLightModeRowIndex
        {
            IndirectOnly,
            ShadowMaskPastShadowDistance,
            ShadowMaskAllTheWay,
            Subtractive
        }
    }
}

