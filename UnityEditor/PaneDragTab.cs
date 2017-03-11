namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class PaneDragTab : GUIView
    {
        private const float kMaxArea = 50000f;
        private const float kTopThumbnailOffset = 10f;
        private GUIContent m_Content;
        private bool m_DidResizeOnLastLayout;
        [SerializeField]
        private Vector2 m_FullWindowSize = new Vector2(80f, 60f);
        [SerializeField]
        private ContainerWindow m_InFrontOfWindow = null;
        [SerializeField]
        private bool m_Shadow;
        private Rect m_StartRect;
        private bool m_TabVisible;
        private float m_TargetAlpha = 1f;
        [SerializeField]
        private Rect m_TargetRect;
        private DropInfo.Type m_Type = ~DropInfo.Type.Tab;
        [SerializeField]
        internal ContainerWindow m_Window;
        private static PaneDragTab s_Get;
        [SerializeField]
        private static GUIStyle s_PaneStyle;
        [SerializeField]
        private static GUIStyle s_TabStyle;

        public void Close()
        {
            if (this.m_Window != null)
            {
                this.m_Window.Close();
            }
            UnityEngine.Object.DestroyImmediate(this, true);
            s_Get = null;
        }

        private void OnGUI()
        {
            if (s_PaneStyle == null)
            {
                s_PaneStyle = "dragtabdropwindow";
                s_TabStyle = "dragtab";
            }
            if (Event.current.type == EventType.Repaint)
            {
                Color color = GUI.color;
                GUI.color = Color.white;
                s_PaneStyle.Draw(new Rect(0f, 0f, base.position.width, base.position.height), this.m_Content, false, false, true, true);
                if (this.m_TabVisible)
                {
                    s_TabStyle.Draw(new Rect(0f, 0f, base.position.width, base.position.height), this.m_Content, false, false, true, true);
                }
                GUI.color = color;
                this.m_Window.SetAlpha(this.m_TargetAlpha);
            }
        }

        public void SetDropInfo(DropInfo di, Vector2 mouseScreenPos, ContainerWindow inFrontOf)
        {
            if ((this.m_Type != di.type) || ((di.type == DropInfo.Type.Pane) && (di.rect != this.m_TargetRect)))
            {
                this.m_Type = di.type;
                switch (di.type)
                {
                    case DropInfo.Type.Window:
                        this.m_TargetAlpha = 0.6f;
                        break;

                    case DropInfo.Type.Pane:
                    case DropInfo.Type.Tab:
                        this.m_TargetAlpha = 1f;
                        break;
                }
            }
            switch (di.type)
            {
                case DropInfo.Type.Window:
                    this.m_TargetRect = new Rect(mouseScreenPos.x - (this.m_FullWindowSize.x / 2f), mouseScreenPos.y - (this.m_FullWindowSize.y / 2f), this.m_FullWindowSize.x, this.m_FullWindowSize.y);
                    break;

                case DropInfo.Type.Pane:
                case DropInfo.Type.Tab:
                    this.m_TargetRect = di.rect;
                    break;
            }
            this.m_TabVisible = di.type == DropInfo.Type.Tab;
            this.m_TargetRect.x = Mathf.Round(this.m_TargetRect.x);
            this.m_TargetRect.y = Mathf.Round(this.m_TargetRect.y);
            this.m_TargetRect.width = Mathf.Round(this.m_TargetRect.width);
            this.m_TargetRect.height = Mathf.Round(this.m_TargetRect.height);
            this.m_InFrontOfWindow = inFrontOf;
            this.m_Window.MoveInFrontOf(this.m_InFrontOfWindow);
            this.SetWindowPos(this.m_TargetRect);
            base.Repaint();
        }

        private void SetWindowPos(Rect screenPosition)
        {
            this.m_Window.position = screenPosition;
        }

        public void Show(Rect pixelPos, GUIContent content, Vector2 viewSize, Vector2 mouseScreenPosition)
        {
            this.m_Content = content;
            float num = viewSize.x * viewSize.y;
            this.m_FullWindowSize = (Vector2) (viewSize * Mathf.Sqrt(Mathf.Clamp01(50000f / num)));
            if (this.m_Window == null)
            {
                this.m_Window = ScriptableObject.CreateInstance<ContainerWindow>();
                this.m_Window.m_DontSaveToLayout = true;
                base.SetMinMaxSizes(Vector2.zero, new Vector2(10000f, 10000f));
                this.SetWindowPos(pixelPos);
                this.m_Window.rootView = this;
            }
            else
            {
                this.SetWindowPos(pixelPos);
            }
            this.m_Window.Show(ShowMode.NoShadow, true, false);
            this.m_TargetRect = pixelPos;
        }

        public static PaneDragTab get
        {
            get
            {
                if (s_Get == null)
                {
                    UnityEngine.Object[] objArray = UnityEngine.Resources.FindObjectsOfTypeAll(typeof(PaneDragTab));
                    if (objArray.Length != 0)
                    {
                        s_Get = (PaneDragTab) objArray[0];
                    }
                    if (s_Get != null)
                    {
                        return s_Get;
                    }
                    s_Get = ScriptableObject.CreateInstance<PaneDragTab>();
                }
                return s_Get;
            }
        }
    }
}

