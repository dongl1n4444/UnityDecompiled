namespace UnityEditor
{
    using System;

    internal class EditorApplicationLayout
    {
        private static GameView m_GameView = null;
        private static bool m_MaximizePending = false;

        private static void Clear()
        {
            m_MaximizePending = false;
            m_GameView = null;
        }

        internal static void FinalizePlaymodeLayout()
        {
            if (m_GameView != null)
            {
                if (m_MaximizePending)
                {
                    WindowLayout.MaximizePresent(m_GameView);
                }
                m_GameView.m_Parent.ClearStartView();
            }
            Clear();
        }

        internal static void InitPlaymodeLayout()
        {
            m_GameView = WindowLayout.ShowAppropriateViewOnEnterExitPlaymode(true) as GameView;
            if (m_GameView != null)
            {
                if (m_GameView.maximizeOnPlay)
                {
                    DockArea parent = m_GameView.m_Parent as DockArea;
                    if (parent != null)
                    {
                        m_MaximizePending = WindowLayout.MaximizePrepare(parent.actualView);
                    }
                }
                m_GameView.m_Parent.SetAsStartView();
                Toolbar.RepaintToolbar();
            }
        }

        internal static bool IsInitializingPlaymodeLayout() => 
            (m_GameView != null);

        internal static void SetPausemodeLayout()
        {
            SetStopmodeLayout();
        }

        internal static void SetPlaymodeLayout()
        {
            InitPlaymodeLayout();
            FinalizePlaymodeLayout();
        }

        internal static void SetStopmodeLayout()
        {
            WindowLayout.ShowAppropriateViewOnEnterExitPlaymode(false);
            Toolbar.RepaintToolbar();
        }
    }
}

