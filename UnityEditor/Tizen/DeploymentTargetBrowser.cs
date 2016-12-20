namespace UnityEditor.Tizen
{
    using System;
    using System.Collections;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEngine;

    public class DeploymentTargetBrowser : EditorWindow
    {
        private string[] m_DeploymentTargetTypeNames = new string[] { "Mobile", "Emulator", "All" };
        private string[] m_DeviceList = new string[0];
        private GUIStyle m_DeviceListBox;
        private GUIContent m_LabelTargetType = EditorGUIUtility.TextContent("Target Type");
        private ListViewState m_ListView;
        private const int m_RowHeight = 0x20;
        private int m_SelectedType = 0;

        private void OnEnable()
        {
            TizenUtilities.PrepareToolPaths();
            this.m_ListView = new ListViewState(0, 0x20);
            this.RefreshDeviceList();
            this.m_ListView.totalRows = this.m_DeviceList.Length;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUILayout.PrefixLabel(this.m_LabelTargetType, EditorStyles.popup);
            this.m_DeviceListBox = new GUIStyle("box");
            int num = EditorGUILayout.Popup(this.m_SelectedType, this.m_DeploymentTargetTypeNames, new GUILayoutOption[0]);
            if (num != this.m_SelectedType)
            {
                this.m_SelectedType = num;
                this.RefreshDeviceList();
            }
            EditorGUILayout.EndHorizontal();
            int index = 0;
            IEnumerator enumerator = ListViewGUILayout.ListView(this.m_ListView, this.m_DeviceListBox, new GUILayoutOption[0]).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    ListViewElement current = (ListViewElement) enumerator.Current;
                    if ((current.row == this.m_ListView.row) && (Event.current.type == EventType.Repaint))
                    {
                        this.m_DeviceListBox.Draw(current.position, false, false, false, false);
                    }
                    if (ListViewGUILayout.HasMouseUp(current.position) || (this.m_SelectedType == 2))
                    {
                        string str = "";
                        if (this.m_SelectedType == 0)
                        {
                            string pattern = @"Model\:\s\w+-\w+\sDUID\:\s(\w+)";
                            Match match = new Regex(pattern, RegexOptions.IgnoreCase).Match(this.m_DeviceList[index]);
                            if (match.Success)
                            {
                                str = match.Groups[1].Value;
                            }
                        }
                        else if (this.m_SelectedType == 1)
                        {
                            str = this.m_DeviceList[index];
                        }
                        PlayerSettings.Tizen.deploymentTarget = str;
                        PlayerSettings.Tizen.deploymentTargetType = this.m_SelectedType;
                    }
                    GUILayout.Label(this.m_DeviceList[index++], new GUILayoutOption[0]);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        private void RefreshDeviceList()
        {
            switch (this.m_SelectedType)
            {
                case 0:
                    this.m_DeviceList = TizenUtilities.ListDevices();
                    break;

                case 1:
                    this.m_DeviceList = TizenUtilities.ListEmulators();
                    break;

                case 2:
                    Array.Clear(this.m_DeviceList, 0, this.m_DeviceList.Length);
                    break;
            }
            this.m_ListView.totalRows = this.m_DeviceList.Length;
        }

        public static void ShowWindow()
        {
            DeploymentTargetBrowser[] browserArray = (DeploymentTargetBrowser[]) UnityEngine.Resources.FindObjectsOfTypeAll(typeof(DeploymentTargetBrowser));
            DeploymentTargetBrowser browser = (browserArray.Length <= 0) ? ScriptableObject.CreateInstance<DeploymentTargetBrowser>() : browserArray[0];
            if (browserArray.Length > 0)
            {
                browser.Focus();
            }
            else
            {
                browser.titleContent = EditorGUIUtility.TextContent("Discover Tizen Devices");
                browser.position = new Rect(100f, 100f, 250f, 150f);
                browser.minSize = new Vector2(browser.position.width, browser.position.height);
                browser.maxSize = browser.minSize;
                browser.ShowUtility();
            }
        }
    }
}

