namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    internal class GUIView : View
    {
        private int m_DepthBufferBits = 0;
        private bool m_WantsMouseMove = false;
        private bool m_WantsMouseEnterLeaveWindow = false;
        private bool m_AutoRepaintOnSceneChange = false;
        private bool m_BackgroundValid = false;
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void SetTitle(string title);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_Init(int depthBits);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_Recreate(int depthBits);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_Close();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern bool Internal_SendEvent(Event e);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void AddToAuxWindowList();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void RemoveFromAuxWindowList();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        protected extern void Internal_SetAsActiveWindow();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_SetWantsMouseMove(bool wantIt);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_SetWantsMouseEnterLeaveWindow(bool wantIt);
        public void SetInternalGameViewDimensions(Rect rect, Rect clippedRect, Vector2 targetSize)
        {
            INTERNAL_CALL_SetInternalGameViewDimensions(this, ref rect, ref clippedRect, ref targetSize);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetInternalGameViewDimensions(GUIView self, ref Rect rect, ref Rect clippedRect, ref Vector2 targetSize);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetAsStartView();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void ClearStartView();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_SetAutoRepaint(bool doit);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_SetWindow(ContainerWindow win);
        private void Internal_SetPosition(Rect windowPosition)
        {
            INTERNAL_CALL_Internal_SetPosition(this, ref windowPosition);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_SetPosition(GUIView self, ref Rect windowPosition);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void Focus();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void Repaint();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void RepaintImmediately();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void CaptureRenderDoc();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void MakeVistaDWMHappyDance();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void StealMouseCapture();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void ClearKeyboardControl();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void SetKeyboardControl(int id);
        public static GUIView current { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
        public static GUIView focusedView { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
        public static GUIView mouseOverView { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
        public bool hasFocus { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
        internal bool mouseRayInvisible { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
        internal void GrabPixels(RenderTexture rd, Rect rect)
        {
            INTERNAL_CALL_GrabPixels(this, rd, ref rect);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GrabPixels(GUIView self, RenderTexture rd, ref Rect rect);
        internal bool SendEvent(Event e)
        {
            int num = SavedGUIState.Internal_GetGUIDepth();
            bool flag = false;
            if (num > 0)
            {
                SavedGUIState state = SavedGUIState.Create();
                flag = this.Internal_SendEvent(e);
                state.ApplyAndForget();
                return flag;
            }
            return this.Internal_SendEvent(e);
        }

        protected override void SetWindow(ContainerWindow win)
        {
            base.SetWindow(win);
            this.Internal_Init(this.m_DepthBufferBits);
            if (win != null)
            {
                this.Internal_SetWindow(win);
            }
            this.Internal_SetAutoRepaint(this.m_AutoRepaintOnSceneChange);
            this.Internal_SetPosition(base.windowPosition);
            this.Internal_SetWantsMouseMove(this.m_WantsMouseMove);
            this.Internal_SetWantsMouseEnterLeaveWindow(this.m_WantsMouseEnterLeaveWindow);
            this.m_BackgroundValid = false;
        }

        internal void RecreateContext()
        {
            this.Internal_Recreate(this.m_DepthBufferBits);
            this.m_BackgroundValid = false;
        }

        public bool wantsMouseMove
        {
            get => 
                this.m_WantsMouseMove;
            set
            {
                this.m_WantsMouseMove = value;
                this.Internal_SetWantsMouseMove(this.m_WantsMouseMove);
            }
        }
        public bool wantsMouseEnterLeaveWindow
        {
            get => 
                this.m_WantsMouseEnterLeaveWindow;
            set
            {
                this.m_WantsMouseEnterLeaveWindow = value;
                this.Internal_SetWantsMouseEnterLeaveWindow(this.m_WantsMouseEnterLeaveWindow);
            }
        }
        internal bool backgroundValid
        {
            get => 
                this.m_BackgroundValid;
            set
            {
                this.m_BackgroundValid = value;
            }
        }
        public bool autoRepaintOnSceneChange
        {
            get => 
                this.m_AutoRepaintOnSceneChange;
            set
            {
                this.m_AutoRepaintOnSceneChange = value;
                this.Internal_SetAutoRepaint(this.m_AutoRepaintOnSceneChange);
            }
        }
        public int depthBufferBits
        {
            get => 
                this.m_DepthBufferBits;
            set
            {
                this.m_DepthBufferBits = value;
            }
        }
        [Obsolete("AA is not supported on GUIViews", false)]
        public int antiAlias
        {
            get => 
                1;
            set
            {
            }
        }
        protected override void SetPosition(Rect newPos)
        {
            Rect windowPosition = base.windowPosition;
            base.SetPosition(newPos);
            if (windowPosition == base.windowPosition)
            {
                this.Internal_SetPosition(base.windowPosition);
            }
            else
            {
                this.Internal_SetPosition(base.windowPosition);
                this.m_BackgroundValid = false;
                this.Repaint();
            }
        }

        public void OnDestroy()
        {
            this.Internal_Close();
            base.OnDestroy();
        }

        internal void DoWindowDecorationStart()
        {
            if (base.window != null)
            {
                base.window.HandleWindowDecorationStart(base.windowPosition);
            }
        }

        internal void DoWindowDecorationEnd()
        {
            if (base.window != null)
            {
                base.window.HandleWindowDecorationEnd(base.windowPosition);
            }
        }
    }
}

