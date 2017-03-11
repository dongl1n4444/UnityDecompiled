namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using UnityEditor.AnimatedValues;
    using UnityEditor.Build;
    using UnityEngine;
    using UnityEngine.Events;

    [CustomEditor(typeof(VideoClipImporter)), CanEditMultipleObjects]
    internal class VideoClipImporterInspector : AssetImporterInspector
    {
        private const int kNarrowLabelWidth = 0x2a;
        private const int kToggleButtonWidth = 0x10;
        private SerializedProperty m_Deinterlace;
        private SerializedProperty m_EncodeAlpha;
        private SerializedProperty m_FlipHorizontal;
        private SerializedProperty m_FlipVertical;
        private SerializedProperty m_ImportAudio;
        private SerializedProperty m_IsColorLinear;
        private bool m_IsPlaying = false;
        private bool m_ModifiedTargetSettings;
        private Vector2 m_Position = Vector2.zero;
        private SerializedProperty m_Quality;
        private AnimBool m_ShowResizeModeOptions = new AnimBool();
        private InspectorTargetSettings[,] m_TargetSettings;
        private SerializedProperty m_UseLegacyImporter;
        private static string[] s_LegacyFileTypes = new string[] { ".ogg", ".ogv", ".mov", ".asf", ".mpg", ".mpeg", ".mp4" };
        private static Styles s_Styles;

        private bool AnySettingsNotTranscoded()
        {
            if (this.m_TargetSettings != null)
            {
                for (int i = 0; i < this.m_TargetSettings.GetLength(0); i++)
                {
                    for (int j = 0; j < this.m_TargetSettings.GetLength(1); j++)
                    {
                        if ((this.m_TargetSettings[i, j].settings != null) && !this.m_TargetSettings[i, j].settings.enableTranscoding)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal override void Apply()
        {
            base.Apply();
            this.WriteSettingsToBackend();
            foreach (ProjectBrowser browser in ProjectBrowser.GetAllProjectBrowsers())
            {
                browser.Repaint();
            }
        }

        private MultiTargetSettingState CalculateMultiTargetSettingState(int platformIndex)
        {
            MultiTargetSettingState state = new MultiTargetSettingState();
            state.Init();
            if ((this.m_TargetSettings != null) && (this.m_TargetSettings.Length != 0))
            {
                int num = -1;
                for (int i = 0; i < this.m_TargetSettings.GetLength(0); i++)
                {
                    if (this.m_TargetSettings[i, platformIndex].overridePlatform)
                    {
                        if (num == -1)
                        {
                            num = i;
                            state.firstTranscoding = this.m_TargetSettings[i, platformIndex].settings.enableTranscoding;
                            state.firstCodec = this.m_TargetSettings[i, platformIndex].settings.codec;
                            state.firstResizeMode = this.m_TargetSettings[i, platformIndex].settings.resizeMode;
                            state.firstAspectRatio = this.m_TargetSettings[i, platformIndex].settings.aspectRatio;
                            state.firstCustomWidth = this.m_TargetSettings[i, platformIndex].settings.customWidth;
                            state.firstCustomHeight = this.m_TargetSettings[i, platformIndex].settings.customHeight;
                            state.firstBitrateMode = this.m_TargetSettings[i, platformIndex].settings.bitrateMode;
                            state.firstSpatialQuality = this.m_TargetSettings[i, platformIndex].settings.spatialQuality;
                        }
                        else
                        {
                            state.mixedTranscoding = state.firstTranscoding != this.m_TargetSettings[i, platformIndex].settings.enableTranscoding;
                            state.mixedCodec = state.firstCodec != this.m_TargetSettings[i, platformIndex].settings.codec;
                            state.mixedResizeMode = state.firstResizeMode != this.m_TargetSettings[i, platformIndex].settings.resizeMode;
                            state.mixedAspectRatio = state.firstAspectRatio != this.m_TargetSettings[i, platformIndex].settings.aspectRatio;
                            state.mixedCustomWidth = state.firstCustomWidth != this.m_TargetSettings[i, platformIndex].settings.customWidth;
                            state.mixedCustomHeight = state.firstCustomHeight != this.m_TargetSettings[i, platformIndex].settings.customHeight;
                            state.mixedBitrateMode = state.firstBitrateMode != this.m_TargetSettings[i, platformIndex].settings.bitrateMode;
                            state.mixedSpatialQuality = state.firstSpatialQuality != this.m_TargetSettings[i, platformIndex].settings.spatialQuality;
                        }
                    }
                }
                if (num == -1)
                {
                    state.firstTranscoding = this.m_TargetSettings[0, 0].settings.enableTranscoding;
                    state.firstCodec = this.m_TargetSettings[0, 0].settings.codec;
                    state.firstResizeMode = this.m_TargetSettings[0, 0].settings.resizeMode;
                    state.firstAspectRatio = this.m_TargetSettings[0, 0].settings.aspectRatio;
                    state.firstCustomWidth = this.m_TargetSettings[0, 0].settings.customWidth;
                    state.firstCustomHeight = this.m_TargetSettings[0, 0].settings.customHeight;
                    state.firstBitrateMode = this.m_TargetSettings[0, 0].settings.bitrateMode;
                    state.firstSpatialQuality = this.m_TargetSettings[0, 0].settings.spatialQuality;
                }
            }
            return state;
        }

        private VideoImporterTargetSettings CloneTargetSettings(VideoImporterTargetSettings settings) => 
            new VideoImporterTargetSettings { 
                enableTranscoding = settings.enableTranscoding,
                codec = settings.codec,
                resizeMode = settings.resizeMode,
                aspectRatio = settings.aspectRatio,
                customWidth = settings.customWidth,
                customHeight = settings.customHeight,
                bitrateMode = settings.bitrateMode,
                spatialQuality = settings.spatialQuality
            };

        private void EncodingSettingsGUI(int platformIndex, MultiTargetSettingState multiState)
        {
            EditorGUI.showMixedValue = multiState.mixedCodec;
            EditorGUI.BeginChangeCheck();
            VideoCodec codec = (VideoCodec) EditorGUILayout.EnumPopup(s_Styles.codecContent, multiState.firstCodec, new GUILayoutOption[0]);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < this.m_TargetSettings.GetLength(0); i++)
                {
                    if (this.m_TargetSettings[i, platformIndex].settings != null)
                    {
                        this.m_TargetSettings[i, platformIndex].settings.codec = codec;
                        this.m_ModifiedTargetSettings = true;
                    }
                }
            }
            EditorGUI.showMixedValue = multiState.mixedBitrateMode;
            EditorGUI.BeginChangeCheck();
            VideoBitrateMode mode = (VideoBitrateMode) EditorGUILayout.EnumPopup(s_Styles.bitrateContent, multiState.firstBitrateMode, new GUILayoutOption[0]);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                for (int j = 0; j < this.m_TargetSettings.GetLength(0); j++)
                {
                    if (this.m_TargetSettings[j, platformIndex].settings != null)
                    {
                        this.m_TargetSettings[j, platformIndex].settings.bitrateMode = mode;
                        this.m_ModifiedTargetSettings = true;
                    }
                }
            }
            EditorGUI.showMixedValue = multiState.mixedSpatialQuality;
            EditorGUI.BeginChangeCheck();
            VideoSpatialQuality quality = (VideoSpatialQuality) EditorGUILayout.EnumPopup(s_Styles.spatialQualityContent, multiState.firstSpatialQuality, new GUILayoutOption[0]);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                for (int k = 0; k < this.m_TargetSettings.GetLength(0); k++)
                {
                    if (this.m_TargetSettings[k, platformIndex].settings != null)
                    {
                        this.m_TargetSettings[k, platformIndex].settings.spatialQuality = quality;
                        this.m_ModifiedTargetSettings = true;
                    }
                }
            }
        }

        private void FrameSettingsGUI(int platformIndex, MultiTargetSettingState multiState)
        {
            EditorGUI.showMixedValue = multiState.mixedResizeMode;
            EditorGUI.BeginChangeCheck();
            VideoResizeMode mode = (VideoResizeMode) EditorGUILayout.Popup(s_Styles.dimensionsContent, (int) multiState.firstResizeMode, this.GetResizeModeList().ToArray(), new GUILayoutOption[0]);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < this.m_TargetSettings.GetLength(0); i++)
                {
                    if (this.m_TargetSettings[i, platformIndex].settings != null)
                    {
                        this.m_TargetSettings[i, platformIndex].settings.resizeMode = mode;
                        this.m_ModifiedTargetSettings = true;
                    }
                }
            }
            this.m_ShowResizeModeOptions.target = mode != VideoResizeMode.OriginalSize;
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowResizeModeOptions.faded))
            {
                EditorGUI.indentLevel++;
                if (mode == VideoResizeMode.CustomSize)
                {
                    EditorGUI.showMixedValue = multiState.mixedCustomWidth;
                    EditorGUI.BeginChangeCheck();
                    int num2 = EditorGUILayout.IntField(s_Styles.widthContent, multiState.firstCustomWidth, new GUILayoutOption[0]);
                    EditorGUI.showMixedValue = false;
                    if (EditorGUI.EndChangeCheck())
                    {
                        for (int j = 0; j < this.m_TargetSettings.GetLength(0); j++)
                        {
                            if (this.m_TargetSettings[j, platformIndex].settings != null)
                            {
                                this.m_TargetSettings[j, platformIndex].settings.customWidth = num2;
                                this.m_ModifiedTargetSettings = true;
                            }
                        }
                    }
                    EditorGUI.showMixedValue = multiState.mixedCustomHeight;
                    EditorGUI.BeginChangeCheck();
                    int num4 = EditorGUILayout.IntField(s_Styles.heightContent, multiState.firstCustomHeight, new GUILayoutOption[0]);
                    EditorGUI.showMixedValue = false;
                    if (EditorGUI.EndChangeCheck())
                    {
                        for (int k = 0; k < this.m_TargetSettings.GetLength(0); k++)
                        {
                            if (this.m_TargetSettings[k, platformIndex].settings != null)
                            {
                                this.m_TargetSettings[k, platformIndex].settings.customHeight = num4;
                                this.m_ModifiedTargetSettings = true;
                            }
                        }
                    }
                }
                EditorGUI.showMixedValue = multiState.mixedAspectRatio;
                EditorGUI.BeginChangeCheck();
                VideoEncodeAspectRatio ratio = (VideoEncodeAspectRatio) EditorGUILayout.EnumPopup(s_Styles.aspectRatioContent, multiState.firstAspectRatio, new GUILayoutOption[0]);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    for (int m = 0; m < this.m_TargetSettings.GetLength(0); m++)
                    {
                        if (this.m_TargetSettings[m, platformIndex].settings != null)
                        {
                            this.m_TargetSettings[m, platformIndex].settings.aspectRatio = ratio;
                            this.m_ModifiedTargetSettings = true;
                        }
                    }
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();
        }

        private List<GUIContent> GetResizeModeList()
        {
            List<GUIContent> list = new List<GUIContent>();
            VideoClipImporter target = (VideoClipImporter) base.target;
            IEnumerator enumerator = Enum.GetValues(typeof(VideoResizeMode)).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    VideoResizeMode current = (VideoResizeMode) enumerator.Current;
                    list.Add(EditorGUIUtility.TextContent(target.GetResizeModeName(current)));
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            return list;
        }

        private bool HasMixedOverrideStatus(int platformIndex, out bool overrideState)
        {
            overrideState = false;
            if ((this.m_TargetSettings != null) && (this.m_TargetSettings.Length != 0))
            {
                overrideState = this.m_TargetSettings[0, platformIndex].overridePlatform;
                for (int i = 1; i < this.m_TargetSettings.GetLength(0); i++)
                {
                    if (this.m_TargetSettings[i, platformIndex].overridePlatform != overrideState)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal override bool HasModified() => 
            (base.HasModified() || this.m_ModifiedTargetSettings);

        public override bool HasPreviewGUI() => 
            (base.target != null);

        private bool IsFileSupportedByLegacy(string assetPath) => 
            (Array.IndexOf<string>(s_LegacyFileTypes, Path.GetExtension(assetPath).ToLower()) != -1);

        private void OnCrossTargetInspectorGUI()
        {
            bool flag = true;
            bool flag2 = true;
            for (int i = 0; i < base.targets.Length; i++)
            {
                VideoClipImporter importer = (VideoClipImporter) base.targets[i];
                flag &= importer.sourceHasAlpha;
                flag2 &= importer.sourceAudioTrackCount > 0;
            }
            if (flag)
            {
                EditorGUILayout.PropertyField(this.m_EncodeAlpha, s_Styles.keepAlphaContent, new GUILayoutOption[0]);
            }
            EditorGUILayout.PropertyField(this.m_Deinterlace, s_Styles.deinterlaceContent, new GUILayoutOption[0]);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.m_FlipHorizontal, s_Styles.flipHorizontalContent, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_FlipVertical, s_Styles.flipVerticalContent, new GUILayoutOption[0]);
            EditorGUILayout.Space();
            using (new EditorGUI.DisabledScope(!flag2))
            {
                EditorGUILayout.PropertyField(this.m_ImportAudio, s_Styles.importAudioContent, new GUILayoutOption[0]);
            }
        }

        public override void OnDisable()
        {
            VideoClipImporter target = base.target as VideoClipImporter;
            if (target != null)
            {
                target.StopPreview();
            }
            base.OnDisable();
        }

        public void OnEnable()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            this.m_UseLegacyImporter = base.serializedObject.FindProperty("m_UseLegacyImporter");
            this.m_Quality = base.serializedObject.FindProperty("m_Quality");
            this.m_IsColorLinear = base.serializedObject.FindProperty("m_IsColorLinear");
            this.m_EncodeAlpha = base.serializedObject.FindProperty("m_EncodeAlpha");
            this.m_Deinterlace = base.serializedObject.FindProperty("m_Deinterlace");
            this.m_FlipVertical = base.serializedObject.FindProperty("m_FlipVertical");
            this.m_FlipHorizontal = base.serializedObject.FindProperty("m_FlipHorizontal");
            this.m_ImportAudio = base.serializedObject.FindProperty("m_ImportAudio");
            this.ResetSettingsFromBackend();
            MultiTargetSettingState state = this.CalculateMultiTargetSettingState(0);
            this.m_ShowResizeModeOptions.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowResizeModeOptions.value = state.mixedResizeMode || (state.firstResizeMode != VideoResizeMode.OriginalSize);
        }

        internal override void OnHeaderControlsGUI()
        {
            base.serializedObject.UpdateIfRequiredOrScript();
            bool flag = true;
            for (int i = 0; flag && (i < base.targets.Length); i++)
            {
                VideoClipImporter importer = (VideoClipImporter) base.targets[i];
                flag &= this.IsFileSupportedByLegacy(importer.assetPath);
            }
            if (!flag)
            {
                base.OnHeaderControlsGUI();
            }
            else
            {
                EditorGUI.showMixedValue = this.m_UseLegacyImporter.hasMultipleDifferentValues;
                EditorGUI.BeginChangeCheck();
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 100f;
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MaxWidth(230f) };
                int num3 = EditorGUILayout.Popup(s_Styles.importerVersionContent, !this.m_UseLegacyImporter.boolValue ? 1 : 0, s_Styles.importerVersionOptions, EditorStyles.popup, options);
                EditorGUIUtility.labelWidth = labelWidth;
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    this.m_UseLegacyImporter.boolValue = num3 == 0;
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Open", EditorStyles.miniButton, new GUILayoutOption[0]))
                {
                    AssetDatabase.OpenAsset(this.assetEditor.targets);
                    GUIUtility.ExitGUI();
                }
            }
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.UpdateIfRequiredOrScript();
            if (this.m_UseLegacyImporter.boolValue)
            {
                EditorGUILayout.PropertyField(this.m_IsColorLinear, MovieImporterInspector.linearTextureContent, new GUILayoutOption[0]);
                EditorGUILayout.Slider(this.m_Quality, 0f, 1f, new GUILayoutOption[0]);
            }
            else
            {
                this.OnCrossTargetInspectorGUI();
                EditorGUILayout.Space();
                this.OnTargetsInspectorGUI();
                if (this.AnySettingsNotTranscoded())
                {
                    EditorGUILayout.HelpBox(s_Styles.transcodeWarning.text, MessageType.Info);
                }
            }
            base.ApplyRevertGUI();
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (Event.current.type == EventType.Repaint)
            {
                background.Draw(r, false, false, false, false);
                VideoClipImporter target = (VideoClipImporter) base.target;
                if (this.m_IsPlaying && !target.isPlayingPreview)
                {
                    target.PlayPreview();
                }
                else if (!this.m_IsPlaying && target.isPlayingPreview)
                {
                    target.StopPreview();
                }
                Texture previewTexture = target.GetPreviewTexture();
                if (((previewTexture != null) && (previewTexture.width != 0)) && (previewTexture.height != 0))
                {
                    float num = 1f;
                    float num2 = 1f;
                    if (target.defaultTargetSettings.enableTranscoding)
                    {
                        VideoResizeMode resizeMode = target.defaultTargetSettings.resizeMode;
                        num = target.GetResizeWidth(resizeMode) / previewTexture.width;
                        num2 = target.GetResizeHeight(resizeMode) / previewTexture.height;
                    }
                    float[] values = new float[] { (num * r.width) / ((float) previewTexture.width), (num2 * r.height) / ((float) previewTexture.height), num, num2 };
                    float num3 = Mathf.Min(values);
                    Rect viewRect = new Rect(r.x, r.y, previewTexture.width * num3, previewTexture.height * num3);
                    PreviewGUI.BeginScrollView(r, this.m_Position, viewRect, "PreHorizontalScrollbar", "PreHorizontalScrollbarThumb");
                    EditorGUI.DrawTextureTransparent(viewRect, previewTexture, ScaleMode.StretchToFill);
                    this.m_Position = PreviewGUI.EndScrollView();
                    if (this.m_IsPlaying)
                    {
                        GUIView.current.Repaint();
                    }
                }
            }
        }

        public override void OnPreviewSettings()
        {
            EditorGUI.BeginDisabledGroup(Application.isPlaying || this.HasModified());
            this.m_IsPlaying = PreviewGUI.CycleButton(!this.m_IsPlaying ? 0 : 1, s_Styles.playIcons) != 0;
            EditorGUI.EndDisabledGroup();
        }

        private void OnTargetInspectorGUI(int platformIndex, string platformName)
        {
            bool flag = true;
            if (platformIndex != 0)
            {
                bool flag2;
                EditorGUI.showMixedValue = this.HasMixedOverrideStatus(platformIndex, out flag2);
                EditorGUI.BeginChangeCheck();
                flag2 = EditorGUILayout.Toggle("Override for " + platformName, flag2, new GUILayoutOption[0]);
                flag = flag2 || EditorGUI.showMixedValue;
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    for (int i = 0; i < this.m_TargetSettings.GetLength(0); i++)
                    {
                        this.m_TargetSettings[i, platformIndex].overridePlatform = flag2;
                        this.m_ModifiedTargetSettings = true;
                        if (this.m_TargetSettings[i, platformIndex].settings == null)
                        {
                            this.m_TargetSettings[i, platformIndex].settings = this.CloneTargetSettings(this.m_TargetSettings[i, 0].settings);
                        }
                    }
                }
            }
            EditorGUILayout.Space();
            MultiTargetSettingState multiState = this.CalculateMultiTargetSettingState(platformIndex);
            using (new EditorGUI.DisabledScope(!flag))
            {
                this.OnTargetSettingsInspectorGUI(platformIndex, multiState);
            }
        }

        private void OnTargetSettingsInspectorGUI(int platformIndex, MultiTargetSettingState multiState)
        {
            EditorGUI.showMixedValue = multiState.mixedTranscoding;
            EditorGUI.BeginChangeCheck();
            bool flag = EditorGUILayout.Toggle(s_Styles.transcodeContent, multiState.firstTranscoding, new GUILayoutOption[0]);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < this.m_TargetSettings.GetLength(0); i++)
                {
                    if (this.m_TargetSettings[i, platformIndex].settings != null)
                    {
                        this.m_TargetSettings[i, platformIndex].settings.enableTranscoding = flag;
                        this.m_ModifiedTargetSettings = true;
                    }
                }
            }
            EditorGUI.indentLevel++;
            using (new EditorGUI.DisabledScope(!(flag || multiState.mixedTranscoding)))
            {
                this.FrameSettingsGUI(platformIndex, multiState);
                this.EncodingSettingsGUI(platformIndex, multiState);
            }
            EditorGUI.indentLevel--;
        }

        private void OnTargetsInspectorGUI()
        {
            BuildPlatform[] platforms = BuildPlatforms.instance.GetValidPlatforms().ToArray();
            int index = EditorGUILayout.BeginPlatformGrouping(platforms, GUIContent.Temp("Default"));
            string platformName = (index != -1) ? platforms[index].name : "Default";
            this.OnTargetInspectorGUI(index + 1, platformName);
            EditorGUILayout.EndPlatformGrouping();
        }

        private void ResetSettingsFromBackend()
        {
            this.m_TargetSettings = null;
            if (base.targets.Length > 0)
            {
                List<BuildPlatform> validPlatforms = BuildPlatforms.instance.GetValidPlatforms();
                this.m_TargetSettings = new InspectorTargetSettings[base.targets.Length, validPlatforms.Count + 1];
                for (int i = 0; i < base.targets.Length; i++)
                {
                    VideoClipImporter importer = (VideoClipImporter) base.targets[i];
                    this.m_TargetSettings[i, 0] = new InspectorTargetSettings();
                    this.m_TargetSettings[i, 0].overridePlatform = true;
                    this.m_TargetSettings[i, 0].settings = importer.defaultTargetSettings;
                    for (int j = 1; j < (validPlatforms.Count + 1); j++)
                    {
                        BuildTargetGroup targetGroup = validPlatforms[j - 1].targetGroup;
                        this.m_TargetSettings[i, j] = new InspectorTargetSettings();
                        this.m_TargetSettings[i, j].settings = importer.Internal_GetTargetSettings(targetGroup);
                        this.m_TargetSettings[i, j].overridePlatform = this.m_TargetSettings[i, j].settings != null;
                    }
                }
            }
            this.m_ModifiedTargetSettings = false;
        }

        internal override void ResetValues()
        {
            base.ResetValues();
            this.OnEnable();
        }

        private void WriteSettingsToBackend()
        {
            if (this.m_TargetSettings != null)
            {
                List<BuildPlatform> validPlatforms = BuildPlatforms.instance.GetValidPlatforms();
                for (int i = 0; i < base.targets.Length; i++)
                {
                    VideoClipImporter importer = (VideoClipImporter) base.targets[i];
                    importer.defaultTargetSettings = this.m_TargetSettings[i, 0].settings;
                    for (int j = 1; j < (validPlatforms.Count + 1); j++)
                    {
                        BuildTargetGroup targetGroup = validPlatforms[j - 1].targetGroup;
                        if ((this.m_TargetSettings[i, j].settings != null) && this.m_TargetSettings[i, j].overridePlatform)
                        {
                            importer.Internal_SetTargetSettings(targetGroup, this.m_TargetSettings[i, j].settings);
                        }
                        else
                        {
                            importer.Internal_ClearTargetSettings(targetGroup);
                        }
                    }
                }
            }
            this.m_ModifiedTargetSettings = false;
        }

        internal override bool showImportedObject =>
            false;

        protected override bool useAssetDrawPreview =>
            true;

        internal class InspectorTargetSettings
        {
            public bool overridePlatform;
            public VideoImporterTargetSettings settings;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MultiTargetSettingState
        {
            public bool mixedTranscoding;
            public bool mixedCodec;
            public bool mixedResizeMode;
            public bool mixedAspectRatio;
            public bool mixedCustomWidth;
            public bool mixedCustomHeight;
            public bool mixedBitrateMode;
            public bool mixedSpatialQuality;
            public bool firstTranscoding;
            public VideoCodec firstCodec;
            public VideoResizeMode firstResizeMode;
            public VideoEncodeAspectRatio firstAspectRatio;
            public int firstCustomWidth;
            public int firstCustomHeight;
            public VideoBitrateMode firstBitrateMode;
            public VideoSpatialQuality firstSpatialQuality;
            public void Init()
            {
                this.mixedTranscoding = false;
                this.mixedCodec = false;
                this.mixedResizeMode = false;
                this.mixedAspectRatio = false;
                this.mixedCustomWidth = false;
                this.mixedCustomHeight = false;
                this.mixedBitrateMode = false;
                this.mixedSpatialQuality = false;
                this.firstTranscoding = false;
                this.firstCodec = VideoCodec.Auto;
                this.firstResizeMode = VideoResizeMode.OriginalSize;
                this.firstAspectRatio = VideoEncodeAspectRatio.NoScaling;
                this.firstCustomWidth = -1;
                this.firstCustomHeight = -1;
                this.firstBitrateMode = VideoBitrateMode.High;
                this.firstSpatialQuality = VideoSpatialQuality.HighSpatialQuality;
            }
        }

        private class Styles
        {
            public GUIContent aspectRatioContent = EditorGUIUtility.TextContent("Aspect Ratio|How the original video is mapped into the target dimensions.");
            public GUIContent bitrateContent = EditorGUIUtility.TextContent("Bitrate Mode|Higher bit rates give a better quality, but impose higher load on network connections or storage.");
            public GUIContent codecContent = EditorGUIUtility.TextContent("Codec|Codec for the resulting clip. Automatic will make the best choice for the target platform.");
            public GUIContent deinterlaceContent = EditorGUIUtility.TextContent("Deinterlace|Remove interlacing on this video.");
            public GUIContent dimensionsContent = EditorGUIUtility.TextContent("Dimensions|Pixel size of the resulting video.");
            public GUIContent flipHorizontalContent = EditorGUIUtility.TextContent("Flip Horizontally|Flip the video horizontally during transcoding.");
            public GUIContent flipVerticalContent = EditorGUIUtility.TextContent("Flip Vertically|Flip the video vertically during transcoding.");
            public GUIContent heightContent = EditorGUIUtility.TextContent("Height|Height in pixels of the resulting video.");
            public GUIContent importAudioContent = EditorGUIUtility.TextContent("Import Audio|Defines if the audio tracks will be imported during transcoding.");
            public GUIContent importerVersionContent = EditorGUIUtility.TextContent("Importer Version|Selects the type of asset produced (legacy MovieTexture or new VideoClip).");
            public GUIContent[] importerVersionOptions = new GUIContent[] { EditorGUIUtility.TextContent("MovieTexture (Legacy)|Produce MovieTexture asset (old version)"), EditorGUIUtility.TextContent("VideoClip|Produce VideoClip asset (for use with VideoPlayer)") };
            public GUIContent keepAlphaContent = EditorGUIUtility.TextContent("Keep Alpha|If the source clip has alpha, this will encode it in the resulting clip so that transparency is usable during render.");
            public GUIContent[] playIcons = new GUIContent[] { EditorGUIUtility.IconContent("preAudioPlayOff"), EditorGUIUtility.IconContent("preAudioPlayOn") };
            public GUIContent spatialQualityContent = EditorGUIUtility.TextContent("Spatial Quality|Adds a downsize during import to reduce bitrate using resolution.");
            public GUIContent transcodeContent = EditorGUIUtility.TextContent("Transcode|Transcoding a clip gives more flexibility through the options below, but takes more time.");
            public GUIContent transcodeWarning = EditorGUIUtility.TextContent("Not all platforms transcoded. Clip is not guaranteed to be compatible on platforms without transcoding.");
            public GUIContent widthContent = EditorGUIUtility.TextContent("Width|Width in pixels of the resulting video.");
        }
    }
}

