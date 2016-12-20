namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class GameViewSizeMenu : FlexibleMenu
    {
        private const float kMargin = 9f;
        private const float kTopMargin = 7f;
        private IGameViewSizeMenuUser m_GameView;

        public GameViewSizeMenu(IFlexibleMenuItemProvider itemProvider, int selectionIndex, FlexibleMenuModifyItemUI modifyItemUi, IGameViewSizeMenuUser gameView) : base(itemProvider, selectionIndex, modifyItemUi, new Action<int, object>(gameView, (IntPtr) gameView.SizeSelectionCallback))
        {
            this.m_GameView = gameView;
        }

        public override Vector2 GetWindowSize()
        {
            Vector2 vector = base.CalcSize();
            if (this.m_GameView.showLowResolutionToggle)
            {
                vector.y += this.frameHeight + 2f;
            }
            return vector;
        }

        public override void OnGUI(Rect rect)
        {
            if (!this.m_GameView.showLowResolutionToggle)
            {
                base.OnGUI(rect);
            }
            else
            {
                Rect position = new Rect(rect.x, rect.y, rect.width, this.frameHeight);
                GUI.Label(position, "", EditorStyles.inspectorBig);
                GUI.enabled = !this.m_GameView.forceLowResolutionAspectRatios;
                Rect rect3 = new Rect(9f, 7f, rect.width, 16f);
                this.m_GameView.lowResolutionForAspectRatios = GUI.Toggle(rect3, !this.m_GameView.forceLowResolutionAspectRatios ? this.m_GameView.lowResolutionForAspectRatios : true, GameView.Styles.lowResAspectRatiosContextMenuContent);
                GUI.enabled = true;
                rect.height -= this.contentOffset;
                rect.y += this.contentOffset;
                base.OnGUI(rect);
            }
        }

        private float contentOffset
        {
            get
            {
                return (this.frameHeight + 2f);
            }
        }

        private float frameHeight
        {
            get
            {
                return 30f;
            }
        }
    }
}

