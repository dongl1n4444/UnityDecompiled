namespace UnityEditor.Web
{
    using System;
    using UnityEditor;

    internal abstract class WebViewEditorStaticWindow : WebViewEditorWindow, IHasCustomMenu
    {
        protected object m_GlobalObject = null;

        protected WebViewEditorStaticWindow()
        {
            this.m_GlobalObject = null;
        }

        public override void OnDestroy()
        {
            base.OnBecameInvisible();
            this.m_GlobalObject = null;
        }

        public override void OnInitScripting()
        {
            base.SetScriptObject();
        }
    }
}

