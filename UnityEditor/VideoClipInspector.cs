namespace UnityEditor
{
    using System;
    using UnityEngine;
    using UnityEngine.Video;

    [CustomEditor(typeof(VideoClip)), CanEditMultipleObjects]
    internal class VideoClipInspector : Editor
    {
        private static readonly GUID kEmptyGUID;
        private VideoClip m_PlayingClip;
        private Vector2 m_Position = Vector2.zero;
        private GUID m_PreviewID;

        private Texture GetAssetPreviewTexture()
        {
            Texture assetPreview = null;
            bool flag = AssetPreview.IsLoadingAssetPreview(base.target.GetInstanceID());
            assetPreview = AssetPreview.GetAssetPreview(base.target);
            if (assetPreview != null)
            {
                return assetPreview;
            }
            if (flag)
            {
                GUIView.current.Repaint();
            }
            return AssetPreview.GetMiniThumbnail(base.target);
        }

        public override string GetInfoString()
        {
            VideoClip target = base.target as VideoClip;
            ulong frameCount = target.frameCount;
            double frameRate = target.frameRate;
            TimeSpan span2 = new TimeSpan(0L);
            string str = (frameRate <= 0.0) ? span2.ToString() : TimeSpan.FromSeconds(((double) frameCount) / frameRate).ToString();
            if (str.IndexOf('.') != -1)
            {
                str = str.Substring(0, str.Length - 4);
            }
            string str3 = (str + ", " + frameCount.ToString() + " frames") + ", " + frameRate.ToString("F2") + " FPS";
            string[] textArray1 = new string[] { str3, ", ", target.width.ToString(), "x", target.height.ToString() };
            return string.Concat(textArray1);
        }

        public override bool HasPreviewGUI() => 
            (base.targets != null);

        private static void Init()
        {
        }

        public void OnDestroy()
        {
            this.StopPreview();
        }

        public void OnDisable()
        {
        }

        public void OnEnable()
        {
        }

        internal override void OnHeaderIconGUI(Rect iconRect)
        {
            GUI.DrawTexture(iconRect, this.GetAssetPreviewTexture(), ScaleMode.StretchToFill);
        }

        public override void OnInspectorGUI()
        {
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            VideoClip target = base.target as VideoClip;
            Event current = Event.current;
            if (((current.type != EventType.Repaint) && (current.type != EventType.Layout)) && (current.type != EventType.Used))
            {
                if ((current.type == EventType.MouseDown) && r.Contains(current.mousePosition))
                {
                    if (this.m_PlayingClip != null)
                    {
                        if (this.m_PreviewID.Empty() || !VideoUtil.IsPreviewPlaying(this.m_PreviewID))
                        {
                            this.PlayPreview();
                        }
                        else
                        {
                            this.StopPreview();
                        }
                    }
                    current.Use();
                }
            }
            else
            {
                bool flag = true;
                bool flag2 = (target != this.m_PlayingClip) || (!this.m_PreviewID.Empty() && VideoUtil.IsPreviewPlaying(this.m_PreviewID));
                if (target != this.m_PlayingClip)
                {
                    this.StopPreview();
                    this.m_PlayingClip = target;
                }
                Texture image = null;
                if (!this.m_PreviewID.Empty())
                {
                    image = VideoUtil.GetPreviewTexture(this.m_PreviewID);
                }
                if (((image == null) || (image.width == 0)) || (image.height == 0))
                {
                    image = this.GetAssetPreviewTexture();
                    flag = false;
                }
                if (((image != null) && (image.width != 0)) && (image.height != 0))
                {
                    if (Event.current.type == EventType.Repaint)
                    {
                        background.Draw(r, false, false, false, false);
                    }
                    float num = 1f;
                    float num2 = 1f;
                    float[] values = new float[] { (num * r.width) / ((float) image.width), (num2 * r.height) / ((float) image.height), num, num2 };
                    float num3 = Mathf.Min(values);
                    Rect viewRect = !flag ? r : new Rect(r.x, r.y, image.width * num3, image.height * num3);
                    PreviewGUI.BeginScrollView(r, this.m_Position, viewRect, "PreHorizontalScrollbar", "PreHorizontalScrollbarThumb");
                    if (flag)
                    {
                        EditorGUI.DrawTextureTransparent(viewRect, image, ScaleMode.StretchToFill);
                    }
                    else
                    {
                        GUI.DrawTexture(viewRect, image, ScaleMode.ScaleToFit);
                    }
                    this.m_Position = PreviewGUI.EndScrollView();
                    if (flag2)
                    {
                        GUIView.current.Repaint();
                    }
                }
            }
        }

        private void PlayPreview()
        {
            this.m_PreviewID = VideoUtil.StartPreview(this.m_PlayingClip);
            VideoUtil.PlayPreview(this.m_PreviewID, true);
        }

        private void StopPreview()
        {
            if (!this.m_PreviewID.Empty())
            {
                VideoUtil.StopPreview(this.m_PreviewID);
            }
            this.m_PlayingClip = null;
            this.m_PreviewID = kEmptyGUID;
        }
    }
}

