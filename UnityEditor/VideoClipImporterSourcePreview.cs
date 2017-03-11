namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomPreview(typeof(VideoClipImporter))]
    internal class VideoClipImporterSourcePreview : ObjectPreview
    {
        private const float kIndentWidth = 30f;
        private const float kLabelWidth = 120f;
        private const float kValueWidth = 200f;
        private Styles m_Styles = new Styles();
        private GUIContent m_Title;

        private string GetAudioTrackDescription(VideoClipImporter importer, ushort audioTrackIdx)
        {
            ushort sourceAudioChannelCount = importer.GetSourceAudioChannelCount(audioTrackIdx);
            string str = (sourceAudioChannelCount != 0) ? ((sourceAudioChannelCount != 1) ? ((sourceAudioChannelCount != 2) ? ((sourceAudioChannelCount != 4) ? (((sourceAudioChannelCount - 1)).ToString() + ".1") : sourceAudioChannelCount.ToString()) : "Stereo") : "Mono") : "No channels";
            return (importer.GetSourceAudioSampleRate(audioTrackIdx) + " Hz, " + str);
        }

        public override GUIContent GetPreviewTitle()
        {
            if (this.m_Title == null)
            {
                this.m_Title = new GUIContent("Source Info");
            }
            return this.m_Title;
        }

        public override bool HasPreviewGUI()
        {
            VideoClipImporter target = this.target as VideoClipImporter;
            return ((target != null) && !target.useLegacyImporter);
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (Event.current.type == EventType.Repaint)
            {
                VideoClipImporter target = (VideoClipImporter) this.target;
                r = new RectOffset(-5, -5, -5, -5).Add(r);
                r.height = EditorGUIUtility.singleLineHeight;
                Rect labelRect = r;
                Rect valueRect = r;
                labelRect.width = 120f;
                valueRect.xMin += 120f;
                valueRect.width = 200f;
                this.ShowProperty(ref labelRect, ref valueRect, "Original Size", EditorUtility.FormatBytes((long) target.sourceFileSize));
                this.ShowProperty(ref labelRect, ref valueRect, "Imported Size", EditorUtility.FormatBytes((long) target.outputFileSize));
                int frameCount = target.frameCount;
                double frameRate = target.frameRate;
                TimeSpan span2 = new TimeSpan(0L);
                string str = (frameRate <= 0.0) ? span2.ToString() : TimeSpan.FromSeconds(((double) frameCount) / frameRate).ToString();
                if (str.IndexOf('.') != -1)
                {
                    str = str.Substring(0, str.Length - 4);
                }
                this.ShowProperty(ref labelRect, ref valueRect, "Duration", str);
                this.ShowProperty(ref labelRect, ref valueRect, "Frames", frameCount.ToString());
                this.ShowProperty(ref labelRect, ref valueRect, "FPS", frameRate.ToString("F2"));
                this.ShowProperty(ref labelRect, ref valueRect, "Pixels", target.GetResizeWidth(VideoResizeMode.OriginalSize) + "x" + target.GetResizeHeight(VideoResizeMode.OriginalSize));
                this.ShowProperty(ref labelRect, ref valueRect, "Alpha", !target.sourceHasAlpha ? "No" : "Yes");
                ushort sourceAudioTrackCount = target.sourceAudioTrackCount;
                this.ShowProperty(ref labelRect, ref valueRect, "Audio", (sourceAudioTrackCount != 0) ? ((sourceAudioTrackCount != 1) ? "" : this.GetAudioTrackDescription(target, 0)) : "none");
                if (sourceAudioTrackCount > 1)
                {
                    labelRect.xMin += 30f;
                    labelRect.width -= 30f;
                    for (ushort i = 0; i < sourceAudioTrackCount; i = (ushort) (i + 1))
                    {
                        this.ShowProperty(ref labelRect, ref valueRect, "Track #" + (i + 1), this.GetAudioTrackDescription(target, i));
                    }
                }
            }
        }

        private void ShowProperty(ref Rect labelRect, ref Rect valueRect, string label, string value)
        {
            GUI.Label(labelRect, label, this.m_Styles.labelStyle);
            GUI.Label(valueRect, value, this.m_Styles.labelStyle);
            labelRect.y += EditorGUIUtility.singleLineHeight;
            valueRect.y += EditorGUIUtility.singleLineHeight;
        }

        private class Styles
        {
            public GUIStyle labelStyle = new GUIStyle(EditorStyles.label);

            public Styles()
            {
                Color color = new Color(0.7f, 0.7f, 0.7f);
                RectOffset padding = this.labelStyle.padding;
                padding.right += 4;
                this.labelStyle.normal.textColor = color;
            }
        }
    }
}

