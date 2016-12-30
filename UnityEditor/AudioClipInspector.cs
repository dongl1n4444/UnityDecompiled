namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [CustomEditor(typeof(AudioClip)), CanEditMultipleObjects]
    internal class AudioClipInspector : Editor
    {
        private static bool m_bAutoPlay;
        private static bool m_bLoop;
        private static bool m_bPlayFirst;
        private AudioClip m_PlayingClip;
        private Vector2 m_Position = Vector2.zero;
        private PreviewRenderUtility m_PreviewUtility;
        private static Rect m_wantedRect;
        private static GUIContent[] s_AutoPlayIcons = new GUIContent[2];
        private static Texture2D s_DefaultIcon;
        private static GUIContent[] s_LoopIcons = new GUIContent[2];
        private static GUIContent[] s_PlayIcons = new GUIContent[2];
        private static GUIStyle s_PreButton;

        private void DoRenderPreview(AudioClip clip, AudioImporter audioImporter, Rect wantedRect, float scaleFactor)
        {
            <DoRenderPreview>c__AnonStorey1 storey = new <DoRenderPreview>c__AnonStorey1 {
                scaleFactor = scaleFactor
            };
            storey.scaleFactor *= 0.95f;
            storey.minMaxData = (audioImporter != null) ? AudioUtil.GetMinMaxData(audioImporter) : null;
            storey.numChannels = clip.channels;
            storey.numSamples = (storey.minMaxData != null) ? (storey.minMaxData.Length / (2 * storey.numChannels)) : 0;
            float height = wantedRect.height / ((float) storey.numChannels);
            <DoRenderPreview>c__AnonStorey2 storey2 = new <DoRenderPreview>c__AnonStorey2 {
                channel = 0
            };
            while (storey2.channel < storey.numChannels)
            {
                <DoRenderPreview>c__AnonStorey0 storey3 = new <DoRenderPreview>c__AnonStorey0 {
                    <>f__ref$1 = storey,
                    <>f__ref$2 = storey2
                };
                Rect r = new Rect(wantedRect.x, wantedRect.y + (height * storey2.channel), wantedRect.width, height);
                storey3.curveColor = new Color(1f, 0.5490196f, 0f, 1f);
                AudioCurveRendering.DrawMinMaxFilledCurve(r, new AudioCurveRendering.AudioMinMaxCurveAndColorEvaluator(storey3.<>m__0));
                storey2.channel++;
            }
        }

        public override string GetInfoString()
        {
            AudioClip target = base.target as AudioClip;
            int channelCount = AudioUtil.GetChannelCount(target);
            string str = (channelCount != 1) ? ((channelCount != 2) ? (((channelCount - 1)).ToString() + ".1") : "Stereo") : "Mono";
            AudioCompressionFormat targetPlatformSoundCompressionFormat = AudioUtil.GetTargetPlatformSoundCompressionFormat(target);
            AudioCompressionFormat soundCompressionFormat = AudioUtil.GetSoundCompressionFormat(target);
            string str2 = targetPlatformSoundCompressionFormat.ToString();
            if (targetPlatformSoundCompressionFormat != soundCompressionFormat)
            {
                str2 = str2 + " (" + soundCompressionFormat.ToString() + " in editor)";
            }
            string str3 = str2;
            object[] objArray1 = new object[] { str3, ", ", AudioUtil.GetFrequency(target), " Hz, ", str, ", " };
            str2 = string.Concat(objArray1);
            TimeSpan span = new TimeSpan(0, 0, 0, 0, (int) AudioUtil.GetDuration(target));
            if (((uint) AudioUtil.GetDuration(target)) == uint.MaxValue)
            {
                return (str2 + "Unlimited");
            }
            return (str2 + $"{span.Minutes:00}:{span.Seconds:00}.{span.Milliseconds:000}");
        }

        public override bool HasPreviewGUI() => 
            (base.targets != null);

        private static void Init()
        {
            if (s_PreButton == null)
            {
                s_PreButton = "preButton";
                m_bAutoPlay = EditorPrefs.GetBool("AutoPlayAudio", false);
                m_bLoop = false;
                s_AutoPlayIcons[0] = EditorGUIUtility.IconContent("preAudioAutoPlayOff", "|Turn Auto Play on");
                s_AutoPlayIcons[1] = EditorGUIUtility.IconContent("preAudioAutoPlayOn", "|Turn Auto Play off");
                s_PlayIcons[0] = EditorGUIUtility.IconContent("preAudioPlayOff", "|Play");
                s_PlayIcons[1] = EditorGUIUtility.IconContent("preAudioPlayOn", "|Stop");
                s_LoopIcons[0] = EditorGUIUtility.IconContent("preAudioLoopOff", "|Loop on");
                s_LoopIcons[1] = EditorGUIUtility.IconContent("preAudioLoopOn", "|Loop off");
                s_DefaultIcon = EditorGUIUtility.LoadIcon("Profiler.Audio");
            }
        }

        public void OnDestroy()
        {
            if (this.m_PreviewUtility != null)
            {
                this.m_PreviewUtility.Cleanup();
                this.m_PreviewUtility = null;
            }
        }

        public void OnDisable()
        {
            AudioUtil.StopAllClips();
            EditorPrefs.SetBool("AutoPlayAudio", m_bAutoPlay);
        }

        public void OnEnable()
        {
            m_bAutoPlay = EditorPrefs.GetBool("AutoPlayAudio", false);
            if (m_bAutoPlay)
            {
                m_bPlayFirst = true;
            }
        }

        public override void OnInspectorGUI()
        {
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (s_DefaultIcon == null)
            {
                Init();
            }
            AudioClip target = base.target as AudioClip;
            Event current = Event.current;
            if (((current.type != EventType.Repaint) && (current.type != EventType.Layout)) && (current.type != EventType.Used))
            {
                int num = AudioUtil.GetSampleCount(target) / ((int) r.width);
                EventType type = current.type;
                if (((type == EventType.MouseDrag) || (type == EventType.MouseDown)) && (r.Contains(current.mousePosition) && !AudioUtil.IsMovieAudio(target)))
                {
                    if (this.m_PlayingClip != target)
                    {
                        AudioUtil.StopAllClips();
                        AudioUtil.PlayClip(target, 0, m_bLoop);
                        this.m_PlayingClip = target;
                    }
                    AudioUtil.SetClipSamplePosition(target, num * ((int) current.mousePosition.x));
                    current.Use();
                }
            }
            else
            {
                if (Event.current.type == EventType.Repaint)
                {
                    background.Draw(r, false, false, false, false);
                }
                int channelCount = AudioUtil.GetChannelCount(target);
                m_wantedRect = new Rect(r.x, r.y, r.width, r.height);
                float num3 = m_wantedRect.width / target.length;
                if (!AudioUtil.HasPreview(target) && (AudioUtil.IsTrackerFile(target) || AudioUtil.IsMovieAudio(target)))
                {
                    float y = (r.height <= 150f) ? ((r.y + (r.height / 2f)) - 25f) : ((r.y + (r.height / 2f)) - 10f);
                    if (r.width > 64f)
                    {
                        if (AudioUtil.IsTrackerFile(target))
                        {
                            EditorGUI.DropShadowLabel(new Rect(r.x, y, r.width, 20f), string.Format("Module file with " + AudioUtil.GetMusicChannelCount(target) + " channels.", new object[0]));
                        }
                        else if (AudioUtil.IsMovieAudio(target))
                        {
                            if (r.width > 450f)
                            {
                                EditorGUI.DropShadowLabel(new Rect(r.x, y, r.width, 20f), "Audio is attached to a movie. To audition the sound, play the movie.");
                            }
                            else
                            {
                                EditorGUI.DropShadowLabel(new Rect(r.x, y, r.width, 20f), "Audio is attached to a movie.");
                                EditorGUI.DropShadowLabel(new Rect(r.x, y + 10f, r.width, 20f), "To audition the sound, play the movie.");
                            }
                        }
                        else
                        {
                            EditorGUI.DropShadowLabel(new Rect(r.x, y, r.width, 20f), "Can not show PCM data for this file");
                        }
                    }
                    if (this.m_PlayingClip == target)
                    {
                        float clipPosition = AudioUtil.GetClipPosition(target);
                        TimeSpan span = new TimeSpan(0, 0, 0, 0, (int) (clipPosition * 1000f));
                        EditorGUI.DropShadowLabel(new Rect(m_wantedRect.x, m_wantedRect.y, m_wantedRect.width, 20f), $"Playing - {span.Minutes:00}:{span.Seconds:00}.{span.Milliseconds:000}");
                    }
                }
                else
                {
                    PreviewGUI.BeginScrollView(m_wantedRect, this.m_Position, m_wantedRect, "PreHorizontalScrollbar", "PreHorizontalScrollbarThumb");
                    if (Event.current.type == EventType.Repaint)
                    {
                        this.DoRenderPreview(target, AudioUtil.GetImporterFromClip(target), m_wantedRect, 1f);
                    }
                    for (int i = 0; i < channelCount; i++)
                    {
                        if ((channelCount > 1) && (r.width > 64f))
                        {
                            Rect position = new Rect(m_wantedRect.x + 5f, m_wantedRect.y + ((m_wantedRect.height / ((float) channelCount)) * i), 30f, 20f);
                            EditorGUI.DropShadowLabel(position, "ch " + ((i + 1)).ToString());
                        }
                    }
                    if (this.m_PlayingClip == target)
                    {
                        float num8 = AudioUtil.GetClipPosition(target);
                        TimeSpan span2 = new TimeSpan(0, 0, 0, 0, (int) (num8 * 1000f));
                        GUI.DrawTexture(new Rect(m_wantedRect.x + ((int) (num3 * num8)), m_wantedRect.y, 2f, m_wantedRect.height), EditorGUIUtility.whiteTexture);
                        if (r.width > 64f)
                        {
                            EditorGUI.DropShadowLabel(new Rect(m_wantedRect.x, m_wantedRect.y, m_wantedRect.width, 20f), $"{span2.Minutes:00}:{span2.Seconds:00}.{span2.Milliseconds:000}");
                        }
                        else
                        {
                            EditorGUI.DropShadowLabel(new Rect(m_wantedRect.x, m_wantedRect.y, m_wantedRect.width, 20f), $"{span2.Minutes:00}:{span2.Seconds:00}");
                        }
                        if (!AudioUtil.IsClipPlaying(target))
                        {
                            this.m_PlayingClip = null;
                        }
                    }
                    PreviewGUI.EndScrollView();
                }
                if (m_bPlayFirst)
                {
                    AudioUtil.PlayClip(target, 0, m_bLoop);
                    this.m_PlayingClip = target;
                    m_bPlayFirst = false;
                }
                if (this.playing)
                {
                    GUIView.current.Repaint();
                }
            }
        }

        public override void OnPreviewSettings()
        {
            if (s_DefaultIcon == null)
            {
                Init();
            }
            AudioClip target = base.target as AudioClip;
            using (new EditorGUI.DisabledScope(AudioUtil.IsMovieAudio(target)))
            {
                bool disabled = base.targets.Length > 1;
                using (new EditorGUI.DisabledScope(disabled))
                {
                    m_bAutoPlay = !disabled ? m_bAutoPlay : false;
                    m_bAutoPlay = PreviewGUI.CycleButton(!m_bAutoPlay ? 0 : 1, s_AutoPlayIcons) != 0;
                }
                bool bLoop = m_bLoop;
                m_bLoop = PreviewGUI.CycleButton(!m_bLoop ? 0 : 1, s_LoopIcons) != 0;
                if ((bLoop != m_bLoop) && this.playing)
                {
                    AudioUtil.LoopClip(target, m_bLoop);
                }
                using (new EditorGUI.DisabledScope(disabled && !this.playing))
                {
                    bool flag3 = PreviewGUI.CycleButton(!this.playing ? 0 : 1, s_PlayIcons) != 0;
                    if (flag3 != this.playing)
                    {
                        if (flag3)
                        {
                            AudioUtil.PlayClip(target, 0, m_bLoop);
                            this.m_PlayingClip = target;
                        }
                        else
                        {
                            AudioUtil.StopAllClips();
                            this.m_PlayingClip = null;
                        }
                    }
                }
            }
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            AudioClip target = base.target as AudioClip;
            AudioImporter atPath = AssetImporter.GetAtPath(assetPath) as AudioImporter;
            if ((atPath == null) || !ShaderUtil.hardwareSupportsRectRenderTexture)
            {
                return null;
            }
            if (this.m_PreviewUtility == null)
            {
                this.m_PreviewUtility = new PreviewRenderUtility();
            }
            this.m_PreviewUtility.BeginStaticPreview(new Rect(0f, 0f, (float) width, (float) height));
            this.DoRenderPreview(target, atPath, new Rect((0.05f * width) * EditorGUIUtility.pixelsPerPoint, (0.05f * width) * EditorGUIUtility.pixelsPerPoint, (1.9f * width) * EditorGUIUtility.pixelsPerPoint, (1.9f * height) * EditorGUIUtility.pixelsPerPoint), 1f);
            return this.m_PreviewUtility.EndStaticPreview();
        }

        private bool playing =>
            (this.m_PlayingClip != null);

        [CompilerGenerated]
        private sealed class <DoRenderPreview>c__AnonStorey0
        {
            internal AudioClipInspector.<DoRenderPreview>c__AnonStorey1 <>f__ref$1;
            internal AudioClipInspector.<DoRenderPreview>c__AnonStorey2 <>f__ref$2;
            internal Color curveColor;

            internal void <>m__0(float x, out Color col, out float minValue, out float maxValue)
            {
                col = this.curveColor;
                if (this.<>f__ref$1.numSamples <= 0)
                {
                    minValue = 0f;
                    maxValue = 0f;
                }
                else
                {
                    int num2 = (int) Mathf.Floor(Mathf.Clamp(x * (this.<>f__ref$1.numSamples - 2), 0f, (float) (this.<>f__ref$1.numSamples - 2)));
                    int index = ((num2 * this.<>f__ref$1.numChannels) + this.<>f__ref$2.channel) * 2;
                    int num4 = index + (this.<>f__ref$1.numChannels * 2);
                    minValue = Mathf.Min(this.<>f__ref$1.minMaxData[index + 1], this.<>f__ref$1.minMaxData[num4 + 1]) * this.<>f__ref$1.scaleFactor;
                    maxValue = Mathf.Max(this.<>f__ref$1.minMaxData[index], this.<>f__ref$1.minMaxData[num4]) * this.<>f__ref$1.scaleFactor;
                    if (minValue > maxValue)
                    {
                        float num5 = minValue;
                        minValue = maxValue;
                        maxValue = num5;
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <DoRenderPreview>c__AnonStorey1
        {
            internal float[] minMaxData;
            internal int numChannels;
            internal int numSamples;
            internal float scaleFactor;
        }

        [CompilerGenerated]
        private sealed class <DoRenderPreview>c__AnonStorey2
        {
            internal int channel;
        }
    }
}

