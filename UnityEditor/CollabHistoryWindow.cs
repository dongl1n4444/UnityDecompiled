namespace UnityEditor
{
    using System;
    using UnityEditor.Collaboration;
    using UnityEditor.Web;
    using UnityEngine;

    internal class CollabHistoryWindow : WebViewEditorWindowTabs, IHasCustomMenu
    {
        private const string kServiceName = "Collab History";

        protected CollabHistoryWindow()
        {
            base.minSize = new Vector2(275f, 50f);
        }

        private static void CloseHistoryWindows()
        {
            CollabHistoryWindow[] windowArray = Resources.FindObjectsOfTypeAll(typeof(CollabHistoryWindow)) as CollabHistoryWindow[];
            if (windowArray != null)
            {
                foreach (CollabHistoryWindow window in windowArray)
                {
                    window.Close();
                }
            }
        }

        public void OnCollabStateChanged(CollabInfo info)
        {
            if (!CollabAccess.Instance.IsServiceEnabled())
            {
                CloseHistoryWindows();
            }
        }

        public void OnDestroy()
        {
            Collab.instance.StateChanged -= new StateChangedDelegate(this.OnCollabStateChanged);
            base.OnDestroy();
        }

        public override void OnEnable()
        {
            Collab.instance.StateChanged += new StateChangedDelegate(this.OnCollabStateChanged);
            base.initialOpenUrl = "file:///" + EditorApplication.userJavascriptPackagesPath + "unityeditor-collab-history/dist/index.html";
            base.OnEnable();
        }

        public void OnInitScripting()
        {
            base.OnInitScripting();
        }

        public void OnReceiveTitle(string title)
        {
            base.titleContent.text = title;
        }

        [MenuItem("Window/Collab History", false, 0x7db)]
        public static CollabHistoryWindow ShowHistoryWindow()
        {
            Type[] desiredDockNextTo = new Type[] { typeof(InspectorWindow) };
            return EditorWindow.GetWindow<CollabHistoryWindow>("Collab History", desiredDockNextTo);
        }

        public void ToggleMaximize()
        {
            base.ToggleMaximize();
        }

        [MenuItem("Window/Collab History", true)]
        public static bool ValidateShowHistoryWindow() => 
            CollabAccess.Instance.IsServiceEnabled();
    }
}

