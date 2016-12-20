namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(AudioImporter))]
    internal class AudioImporterInspector : AssetImporterInspector
    {
        [CompilerGenerated]
        private static Converter<AudioCompressionFormat, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Converter<AudioCompressionFormat, int> <>f__am$cache1;
        public SerializedProperty m_CompSize;
        private SampleSettingProperties m_DefaultSampleSettings;
        public SerializedProperty m_ForceToMono;
        public SerializedProperty m_LoadInBackground;
        public SerializedProperty m_Normalize;
        public SerializedProperty m_OrigSize;
        public SerializedProperty m_PreloadAudioData;
        private Dictionary<BuildTargetGroup, SampleSettingProperties> m_SampleSettingOverrides;

        internal override void Apply()
        {
            base.Apply();
            this.SyncSettingsToBackend();
            foreach (ProjectBrowser browser in ProjectBrowser.GetAllProjectBrowsers())
            {
                browser.Repaint();
            }
        }

        private bool CompressionFormatHasQuality(AudioCompressionFormat format)
        {
            switch (format)
            {
                case AudioCompressionFormat.Vorbis:
                case AudioCompressionFormat.MP3:
                case AudioCompressionFormat.XMA:
                case AudioCompressionFormat.AAC:
                    return true;
            }
            return false;
        }

        public bool CurrentPlatformHasAutoTranslatedCompression()
        {
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            foreach (AudioImporter importer in this.GetAllAudioImporterTargets())
            {
                AudioCompressionFormat compressionFormat = importer.defaultSampleSettings.compressionFormat;
                if (!importer.Internal_ContainsSampleSettingsOverride(buildTargetGroup))
                {
                    AudioCompressionFormat format2 = importer.Internal_GetOverrideSampleSettings(buildTargetGroup).compressionFormat;
                    if (compressionFormat != format2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CurrentSelectionContainsHardwareSounds()
        {
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            foreach (AudioImporter importer in this.GetAllAudioImporterTargets())
            {
                AudioImporterSampleSettings settings = importer.Internal_GetOverrideSampleSettings(buildTargetGroup);
                if (this.IsHardwareSound(settings.compressionFormat))
                {
                    return true;
                }
            }
            return false;
        }

        [DebuggerHidden]
        private IEnumerable<AudioImporter> GetAllAudioImporterTargets()
        {
            return new <GetAllAudioImporterTargets>c__Iterator0 { 
                $this = this,
                $PC = -2
            };
        }

        private AudioCompressionFormat[] GetFormatsForPlatform(BuildTargetGroup platform)
        {
            List<AudioCompressionFormat> list = new List<AudioCompressionFormat>();
            if (platform == BuildTargetGroup.WebGL)
            {
                list.Add(AudioCompressionFormat.AAC);
                return list.ToArray();
            }
            list.Add(AudioCompressionFormat.PCM);
            if ((platform != BuildTargetGroup.PSM) && (platform != BuildTargetGroup.PSP2))
            {
                list.Add(AudioCompressionFormat.Vorbis);
            }
            list.Add(AudioCompressionFormat.ADPCM);
            if ((((platform != BuildTargetGroup.Standalone) && (platform != BuildTargetGroup.WSA)) && ((platform != BuildTargetGroup.WiiU) && (platform != BuildTargetGroup.XboxOne))) && (platform != BuildTargetGroup.Unknown))
            {
                list.Add(AudioCompressionFormat.MP3);
            }
            if (platform == BuildTargetGroup.PSM)
            {
                list.Add(AudioCompressionFormat.VAG);
            }
            if (platform == BuildTargetGroup.PSP2)
            {
                list.Add(AudioCompressionFormat.HEVAG);
                list.Add(AudioCompressionFormat.ATRAC9);
            }
            if (platform == BuildTargetGroup.PS4)
            {
                list.Add(AudioCompressionFormat.ATRAC9);
            }
            if (platform == BuildTargetGroup.WiiU)
            {
                list.Add(AudioCompressionFormat.GCADPCM);
            }
            if (platform == BuildTargetGroup.XboxOne)
            {
                list.Add(AudioCompressionFormat.XMA);
            }
            return list.ToArray();
        }

        private MultiValueStatus GetMultiValueStatus(BuildTargetGroup platform)
        {
            MultiValueStatus status;
            status.multiLoadType = false;
            status.multiSampleRateSetting = false;
            status.multiSampleRateOverride = false;
            status.multiCompressionFormat = false;
            status.multiQuality = false;
            status.multiConversionMode = false;
            if (Enumerable.Any<AudioImporter>(this.GetAllAudioImporterTargets()))
            {
                AudioImporterSampleSettings defaultSampleSettings;
                AudioImporter importer = Enumerable.First<AudioImporter>(this.GetAllAudioImporterTargets());
                if (platform == BuildTargetGroup.Unknown)
                {
                    defaultSampleSettings = importer.defaultSampleSettings;
                }
                else
                {
                    defaultSampleSettings = importer.Internal_GetOverrideSampleSettings(platform);
                }
                AudioImporter[] second = new AudioImporter[] { importer };
                foreach (AudioImporter importer2 in Enumerable.Except<AudioImporter>(this.GetAllAudioImporterTargets(), second))
                {
                    AudioImporterSampleSettings settings2;
                    if (platform == BuildTargetGroup.Unknown)
                    {
                        settings2 = importer2.defaultSampleSettings;
                    }
                    else
                    {
                        settings2 = importer2.Internal_GetOverrideSampleSettings(platform);
                    }
                    status.multiLoadType |= defaultSampleSettings.loadType != settings2.loadType;
                    status.multiSampleRateSetting |= defaultSampleSettings.sampleRateSetting != settings2.sampleRateSetting;
                    status.multiSampleRateOverride |= defaultSampleSettings.sampleRateOverride != settings2.sampleRateOverride;
                    status.multiCompressionFormat |= defaultSampleSettings.compressionFormat != settings2.compressionFormat;
                    status.multiQuality |= defaultSampleSettings.quality != settings2.quality;
                    status.multiConversionMode |= defaultSampleSettings.conversionMode != settings2.conversionMode;
                }
            }
            return status;
        }

        private OverrideStatus GetOverrideStatus(BuildTargetGroup platform)
        {
            bool flag = false;
            bool flag2 = false;
            if (Enumerable.Any<AudioImporter>(this.GetAllAudioImporterTargets()))
            {
                AudioImporter importer = Enumerable.First<AudioImporter>(this.GetAllAudioImporterTargets());
                flag2 = importer.Internal_ContainsSampleSettingsOverride(platform);
                AudioImporter[] second = new AudioImporter[] { importer };
                foreach (AudioImporter importer2 in Enumerable.Except<AudioImporter>(this.GetAllAudioImporterTargets(), second))
                {
                    bool flag3 = importer2.Internal_ContainsSampleSettingsOverride(platform);
                    if (flag3 != flag2)
                    {
                        flag |= true;
                    }
                    flag2 |= flag3;
                }
            }
            if (!flag2)
            {
                return OverrideStatus.NoOverrides;
            }
            if (flag)
            {
                return OverrideStatus.MixedOverrides;
            }
            return OverrideStatus.AllOverrides;
        }

        internal override bool HasModified()
        {
            if (base.HasModified())
            {
                return true;
            }
            if (this.m_DefaultSampleSettings.HasModified())
            {
                return true;
            }
            Dictionary<BuildTargetGroup, SampleSettingProperties>.ValueCollection values = this.m_SampleSettingOverrides.Values;
            foreach (SampleSettingProperties properties in values)
            {
                if (properties.HasModified())
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsHardwareSound(AudioCompressionFormat format)
        {
            switch (format)
            {
                case AudioCompressionFormat.VAG:
                case AudioCompressionFormat.HEVAG:
                case AudioCompressionFormat.XMA:
                case AudioCompressionFormat.GCADPCM:
                    return true;
            }
            return false;
        }

        private void OnAudioImporterGUI(bool selectionContainsTrackerFile)
        {
            if (!selectionContainsTrackerFile)
            {
                EditorGUILayout.PropertyField(this.m_ForceToMono, new GUILayoutOption[0]);
                EditorGUI.indentLevel++;
                using (new EditorGUI.DisabledScope(!this.m_ForceToMono.boolValue))
                {
                    EditorGUILayout.PropertyField(this.m_Normalize, new GUILayoutOption[0]);
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.PropertyField(this.m_LoadInBackground, new GUILayoutOption[0]);
            }
            BuildPlayerWindow.BuildPlatform[] platforms = BuildPlayerWindow.GetValidPlatforms().ToArray();
            GUILayout.Space(10f);
            int index = EditorGUILayout.BeginPlatformGrouping(platforms, GUIContent.Temp("Default"));
            if (index == -1)
            {
                bool disablePreloadAudioDataOption = this.m_DefaultSampleSettings.settings.loadType == AudioClipLoadType.Streaming;
                MultiValueStatus multiValueStatus = this.GetMultiValueStatus(BuildTargetGroup.Unknown);
                this.OnSampleSettingGUI(BuildTargetGroup.Unknown, multiValueStatus, selectionContainsTrackerFile, ref this.m_DefaultSampleSettings, disablePreloadAudioDataOption);
            }
            else
            {
                BuildTargetGroup targetGroup = platforms[index].targetGroup;
                SampleSettingProperties properties = this.m_SampleSettingOverrides[targetGroup];
                OverrideStatus overrideStatus = this.GetOverrideStatus(targetGroup);
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = (overrideStatus == OverrideStatus.MixedOverrides) && !properties.overrideIsForced;
                bool flag2 = (properties.overrideIsForced && properties.forcedOverrideState) || (!properties.overrideIsForced && (overrideStatus != OverrideStatus.NoOverrides));
                flag2 = EditorGUILayout.ToggleLeft("Override for " + platforms[index].title.text, flag2, new GUILayoutOption[0]);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    properties.forcedOverrideState = flag2;
                    properties.overrideIsForced = true;
                }
                bool flag3 = ((properties.overrideIsForced && properties.forcedOverrideState) || (this.GetOverrideStatus(targetGroup) == OverrideStatus.AllOverrides)) && (properties.settings.loadType == AudioClipLoadType.Streaming);
                MultiValueStatus status = this.GetMultiValueStatus(targetGroup);
                bool disabled = (!properties.overrideIsForced || !properties.forcedOverrideState) && (overrideStatus != OverrideStatus.AllOverrides);
                using (new EditorGUI.DisabledScope(disabled))
                {
                    this.OnSampleSettingGUI(targetGroup, status, selectionContainsTrackerFile, ref properties, flag3);
                }
                this.m_SampleSettingOverrides[targetGroup] = properties;
            }
            EditorGUILayout.EndPlatformGrouping();
        }

        public void OnEnable()
        {
            this.m_ForceToMono = base.serializedObject.FindProperty("m_ForceToMono");
            this.m_Normalize = base.serializedObject.FindProperty("m_Normalize");
            this.m_PreloadAudioData = base.serializedObject.FindProperty("m_PreloadAudioData");
            this.m_LoadInBackground = base.serializedObject.FindProperty("m_LoadInBackground");
            this.m_OrigSize = base.serializedObject.FindProperty("m_PreviewData.m_OrigSize");
            this.m_CompSize = base.serializedObject.FindProperty("m_PreviewData.m_CompSize");
            this.ResetSettingsFromBackend();
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.UpdateIfDirtyOrScript();
            bool selectionContainsTrackerFile = false;
            foreach (AudioImporter importer in this.GetAllAudioImporterTargets())
            {
                switch (FileUtil.GetPathExtension(importer.assetPath).ToLowerInvariant())
                {
                    case "mod":
                    case "it":
                    case "s3m":
                    case "xm":
                        selectionContainsTrackerFile = true;
                        goto Label_00A6;
                }
            }
        Label_00A6:
            this.OnAudioImporterGUI(selectionContainsTrackerFile);
            int bytes = 0;
            int num2 = 0;
            foreach (AudioImporter importer2 in this.GetAllAudioImporterTargets())
            {
                bytes += importer2.origSize;
                num2 += importer2.compSize;
            }
            GUILayout.Space(10f);
            string[] textArray1 = new string[] { "Original Size: \t", EditorUtility.FormatBytes(bytes), "\nImported Size: \t", EditorUtility.FormatBytes(num2), "\nRatio: \t\t", ((100f * num2) / ((float) bytes)).ToString("0.00"), "%" };
            EditorGUILayout.HelpBox(string.Concat(textArray1), MessageType.Info);
            if (this.CurrentPlatformHasAutoTranslatedCompression())
            {
                GUILayout.Space(10f);
                EditorGUILayout.HelpBox("The selection contains different compression formats to the default settings for the current build platform.", MessageType.Info);
            }
            if (this.CurrentSelectionContainsHardwareSounds())
            {
                GUILayout.Space(10f);
                EditorGUILayout.HelpBox("The selection contains sounds that are decompressed in hardware. Advanced mixing is not available for these sounds.", MessageType.Info);
            }
            base.ApplyRevertGUI();
        }

        private void OnSampleSettingGUI(BuildTargetGroup platform, MultiValueStatus status, bool selectionContainsTrackerFile, ref SampleSettingProperties properties, bool disablePreloadAudioDataOption)
        {
            EditorGUI.showMixedValue = status.multiLoadType && !properties.loadTypeChanged;
            EditorGUI.BeginChangeCheck();
            AudioClipLoadType type = (AudioClipLoadType) EditorGUILayout.EnumPopup("Load Type", properties.settings.loadType, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                properties.settings.loadType = type;
                properties.loadTypeChanged = true;
            }
            using (new EditorGUI.DisabledScope(disablePreloadAudioDataOption))
            {
                if (disablePreloadAudioDataOption)
                {
                    EditorGUILayout.Toggle("Preload Audio Data", false, new GUILayoutOption[0]);
                }
                else
                {
                    EditorGUILayout.PropertyField(this.m_PreloadAudioData, new GUILayoutOption[0]);
                }
            }
            if (!selectionContainsTrackerFile)
            {
                AudioCompressionFormat[] formatsForPlatform = this.GetFormatsForPlatform(platform);
                EditorGUI.showMixedValue = status.multiCompressionFormat && !properties.compressionFormatChanged;
                EditorGUI.BeginChangeCheck();
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = value => value.ToString();
                }
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = value => (int) value;
                }
                AudioCompressionFormat format = (AudioCompressionFormat) EditorGUILayout.IntPopup("Compression Format", (int) properties.settings.compressionFormat, Array.ConvertAll<AudioCompressionFormat, string>(formatsForPlatform, <>f__am$cache0), Array.ConvertAll<AudioCompressionFormat, int>(formatsForPlatform, <>f__am$cache1), new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    properties.settings.compressionFormat = format;
                    properties.compressionFormatChanged = true;
                }
                if (this.CompressionFormatHasQuality(properties.settings.compressionFormat))
                {
                    EditorGUI.showMixedValue = status.multiQuality && !properties.qualityChanged;
                    EditorGUI.BeginChangeCheck();
                    int num = EditorGUILayout.IntSlider("Quality", (int) Mathf.Clamp((float) (properties.settings.quality * 100f), (float) 1f, (float) 100f), 1, 100, new GUILayoutOption[0]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        properties.settings.quality = 0.01f * num;
                        properties.qualityChanged = true;
                    }
                }
                if (platform != BuildTargetGroup.WebGL)
                {
                    EditorGUI.showMixedValue = status.multiSampleRateSetting && !properties.sampleRateSettingChanged;
                    EditorGUI.BeginChangeCheck();
                    AudioSampleRateSetting setting = (AudioSampleRateSetting) EditorGUILayout.EnumPopup("Sample Rate Setting", properties.settings.sampleRateSetting, new GUILayoutOption[0]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        properties.settings.sampleRateSetting = setting;
                        properties.sampleRateSettingChanged = true;
                    }
                    if (properties.settings.sampleRateSetting == AudioSampleRateSetting.OverrideSampleRate)
                    {
                        EditorGUI.showMixedValue = status.multiSampleRateOverride && !properties.sampleRateOverrideChanged;
                        EditorGUI.BeginChangeCheck();
                        int num2 = EditorGUILayout.IntPopup("Sample Rate", (int) properties.settings.sampleRateOverride, Styles.kSampleRateStrings, Styles.kSampleRateValues, new GUILayoutOption[0]);
                        if (EditorGUI.EndChangeCheck())
                        {
                            properties.settings.sampleRateOverride = (uint) num2;
                            properties.sampleRateOverrideChanged = true;
                        }
                    }
                }
                EditorGUI.showMixedValue = false;
            }
        }

        private bool ResetSettingsFromBackend()
        {
            if (Enumerable.Any<AudioImporter>(this.GetAllAudioImporterTargets()))
            {
                AudioImporter importer = Enumerable.First<AudioImporter>(this.GetAllAudioImporterTargets());
                this.m_DefaultSampleSettings.settings = importer.defaultSampleSettings;
                this.m_DefaultSampleSettings.ClearChangedFlags();
                this.m_SampleSettingOverrides = new Dictionary<BuildTargetGroup, SampleSettingProperties>();
                List<BuildPlayerWindow.BuildPlatform> validPlatforms = BuildPlayerWindow.GetValidPlatforms();
                foreach (BuildPlayerWindow.BuildPlatform platform in validPlatforms)
                {
                    BuildTargetGroup targetGroup = platform.targetGroup;
                    foreach (AudioImporter importer2 in this.GetAllAudioImporterTargets())
                    {
                        if (importer2.Internal_ContainsSampleSettingsOverride(targetGroup))
                        {
                            SampleSettingProperties properties = new SampleSettingProperties {
                                settings = importer2.Internal_GetOverrideSampleSettings(targetGroup)
                            };
                            this.m_SampleSettingOverrides[targetGroup] = properties;
                            break;
                        }
                    }
                    if (!this.m_SampleSettingOverrides.ContainsKey(targetGroup))
                    {
                        SampleSettingProperties properties2 = new SampleSettingProperties {
                            settings = importer.Internal_GetOverrideSampleSettings(targetGroup)
                        };
                        this.m_SampleSettingOverrides[targetGroup] = properties2;
                    }
                }
            }
            return true;
        }

        internal override void ResetValues()
        {
            base.ResetValues();
            this.ResetSettingsFromBackend();
        }

        private bool SyncSettingsToBackend()
        {
            BuildPlayerWindow.BuildPlatform[] platformArray = BuildPlayerWindow.GetValidPlatforms().ToArray();
            foreach (AudioImporter importer in this.GetAllAudioImporterTargets())
            {
                AudioImporterSampleSettings defaultSampleSettings = importer.defaultSampleSettings;
                if (this.m_DefaultSampleSettings.loadTypeChanged)
                {
                    defaultSampleSettings.loadType = this.m_DefaultSampleSettings.settings.loadType;
                }
                if (this.m_DefaultSampleSettings.sampleRateSettingChanged)
                {
                    defaultSampleSettings.sampleRateSetting = this.m_DefaultSampleSettings.settings.sampleRateSetting;
                }
                if (this.m_DefaultSampleSettings.sampleRateOverrideChanged)
                {
                    defaultSampleSettings.sampleRateOverride = this.m_DefaultSampleSettings.settings.sampleRateOverride;
                }
                if (this.m_DefaultSampleSettings.compressionFormatChanged)
                {
                    defaultSampleSettings.compressionFormat = this.m_DefaultSampleSettings.settings.compressionFormat;
                }
                if (this.m_DefaultSampleSettings.qualityChanged)
                {
                    defaultSampleSettings.quality = this.m_DefaultSampleSettings.settings.quality;
                }
                if (this.m_DefaultSampleSettings.conversionModeChanged)
                {
                    defaultSampleSettings.conversionMode = this.m_DefaultSampleSettings.settings.conversionMode;
                }
                importer.defaultSampleSettings = defaultSampleSettings;
                foreach (BuildPlayerWindow.BuildPlatform platform in platformArray)
                {
                    BuildTargetGroup targetGroup = platform.targetGroup;
                    if (this.m_SampleSettingOverrides.ContainsKey(targetGroup))
                    {
                        SampleSettingProperties properties = this.m_SampleSettingOverrides[targetGroup];
                        if (properties.overrideIsForced && !properties.forcedOverrideState)
                        {
                            importer.Internal_ClearSampleSettingOverride(targetGroup);
                        }
                        else if (importer.Internal_ContainsSampleSettingsOverride(targetGroup) || (properties.overrideIsForced && properties.forcedOverrideState))
                        {
                            AudioImporterSampleSettings settings = importer.Internal_GetOverrideSampleSettings(targetGroup);
                            if (properties.loadTypeChanged)
                            {
                                settings.loadType = properties.settings.loadType;
                            }
                            if (properties.sampleRateSettingChanged)
                            {
                                settings.sampleRateSetting = properties.settings.sampleRateSetting;
                            }
                            if (properties.sampleRateOverrideChanged)
                            {
                                settings.sampleRateOverride = properties.settings.sampleRateOverride;
                            }
                            if (properties.compressionFormatChanged)
                            {
                                settings.compressionFormat = properties.settings.compressionFormat;
                            }
                            if (properties.qualityChanged)
                            {
                                settings.quality = properties.settings.quality;
                            }
                            if (properties.conversionModeChanged)
                            {
                                settings.conversionMode = properties.settings.conversionMode;
                            }
                            importer.Internal_SetOverrideSampleSettings(targetGroup, settings);
                        }
                        this.m_SampleSettingOverrides[targetGroup] = properties;
                    }
                }
            }
            this.m_DefaultSampleSettings.ClearChangedFlags();
            foreach (BuildPlayerWindow.BuildPlatform platform2 in platformArray)
            {
                BuildTargetGroup key = platform2.targetGroup;
                if (this.m_SampleSettingOverrides.ContainsKey(key))
                {
                    SampleSettingProperties properties2 = this.m_SampleSettingOverrides[key];
                    properties2.ClearChangedFlags();
                    this.m_SampleSettingOverrides[key] = properties2;
                }
            }
            return true;
        }

        [CompilerGenerated]
        private sealed class <GetAllAudioImporterTargets>c__Iterator0 : IEnumerable, IEnumerable<AudioImporter>, IEnumerator, IDisposable, IEnumerator<AudioImporter>
        {
            internal AudioImporter $current;
            internal bool $disposing;
            internal Object[] $locvar0;
            internal int $locvar1;
            internal int $PC;
            internal AudioImporterInspector $this;
            internal AudioImporter <audioImporter>__1;
            internal Object <importer>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$disposing = true;
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$locvar0 = this.$this.targets;
                        this.$locvar1 = 0;
                        goto Label_00A5;

                    case 1:
                        break;

                    default:
                        goto Label_00BF;
                }
            Label_0096:
                this.$locvar1++;
            Label_00A5:
                if (this.$locvar1 < this.$locvar0.Length)
                {
                    this.<importer>__0 = this.$locvar0[this.$locvar1];
                    this.<audioImporter>__1 = this.<importer>__0 as AudioImporter;
                    if (this.<audioImporter>__1 != null)
                    {
                        this.$current = this.<audioImporter>__1;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        return true;
                    }
                    goto Label_0096;
                }
                this.$PC = -1;
            Label_00BF:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<AudioImporter> IEnumerable<AudioImporter>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new AudioImporterInspector.<GetAllAudioImporterTargets>c__Iterator0 { $this = this.$this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<UnityEditor.AudioImporter>.GetEnumerator();
            }

            AudioImporter IEnumerator<AudioImporter>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MultiValueStatus
        {
            public bool multiLoadType;
            public bool multiSampleRateSetting;
            public bool multiSampleRateOverride;
            public bool multiCompressionFormat;
            public bool multiQuality;
            public bool multiConversionMode;
        }

        private enum OverrideStatus
        {
            NoOverrides,
            MixedOverrides,
            AllOverrides
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SampleSettingProperties
        {
            public AudioImporterSampleSettings settings;
            public bool forcedOverrideState;
            public bool overrideIsForced;
            public bool loadTypeChanged;
            public bool sampleRateSettingChanged;
            public bool sampleRateOverrideChanged;
            public bool compressionFormatChanged;
            public bool qualityChanged;
            public bool conversionModeChanged;
            public bool HasModified()
            {
                return ((((this.overrideIsForced || this.loadTypeChanged) || (this.sampleRateSettingChanged || this.sampleRateOverrideChanged)) || (this.compressionFormatChanged || this.qualityChanged)) || this.conversionModeChanged);
            }

            public void ClearChangedFlags()
            {
                this.forcedOverrideState = false;
                this.overrideIsForced = false;
                this.loadTypeChanged = false;
                this.sampleRateSettingChanged = false;
                this.sampleRateOverrideChanged = false;
                this.compressionFormatChanged = false;
                this.qualityChanged = false;
                this.conversionModeChanged = false;
            }
        }

        private static class Styles
        {
            public static readonly string[] kSampleRateStrings = new string[] { "8,000 Hz", "11,025 Hz", "22,050 Hz", "44,100 Hz", "48,000 Hz", "96,000 Hz", "192,000 Hz" };
            public static readonly int[] kSampleRateValues = new int[] { 0x1f40, 0x2b11, 0x5622, 0xac44, 0xbb80, 0x17700, 0x2ee00 };
        }
    }
}

