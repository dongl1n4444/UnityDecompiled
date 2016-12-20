namespace UnityEditor.SamsungTV
{
    using System;
    using System.Collections;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Networking;

    internal class DeploymentTargetBrowser : EditorWindow
    {
        private static readonly GUILayoutOption btnWidth = GUILayout.Width(140f);
        private int hostId = -1;
        private const int kMaxBroadcastMsgSize = 0x400;
        private ArrayList m_DeviceList = new ArrayList();
        private ListViewState m_ListView = new ListViewState();
        private const int m_RowHeight = 0x20;
        private static SamsungTVSettingsEditorExtension m_STVEditorExt;
        private byte[] msgInBuffer = null;
        private string target = "";

        private void CheckForDevices()
        {
            int num;
            int num2;
            int num3;
            byte num4;
            this.msgInBuffer = new byte[0x400];
            if (NetworkTransport.ReceiveFromHost(this.hostId, out num, out num2, this.msgInBuffer, 0x400, out num3, out num4) == NetworkEventType.BroadcastEvent)
            {
                string str;
                int num5;
                NetworkTransport.GetBroadcastConnectionMessage(this.hostId, this.msgInBuffer, 0x400, out num3, out num4);
                NetworkTransport.GetBroadcastConnectionInfo(this.hostId, out str, out num5, out num4);
                this.OnReceivedBroadcast(str, GetString(this.msgInBuffer));
            }
        }

        private static string GetString(byte[] bytes)
        {
            char[] dst = new char[bytes.Length / 2];
            Buffer.BlockCopy(bytes, 0, dst, 0, bytes.Length);
            return new string(dst);
        }

        private void InitUnetBroadcastDiscovery()
        {
            byte num;
            if (!NetworkTransport.IsStarted)
            {
                NetworkTransport.Init();
            }
            ConnectionConfig defaultConfig = new ConnectionConfig();
            defaultConfig.AddChannel(QosType.Unreliable);
            HostTopology topology = new HostTopology(defaultConfig, 1);
            this.hostId = NetworkTransport.AddHost(topology, 0xbaa1);
            if (this.hostId == -1)
            {
                Debug.LogError("Network Discovery addHost failed!");
            }
            NetworkTransport.SetBroadcastCredentials(this.hostId, 0x3e8, 1, 1, out num);
        }

        private void OnDisable()
        {
            if (this.hostId != -1)
            {
                NetworkTransport.RemoveHost(this.hostId);
                this.hostId = -1;
            }
        }

        private void OnEnable()
        {
            this.InitUnetBroadcastDiscovery();
            this.m_ListView = new ListViewState(0, 0x20);
            this.RefreshDeviceList();
            this.m_ListView.totalRows = this.m_DeviceList.Count;
        }

        private void OnGUI()
        {
            GUILayout.Label(EditorGUIUtility.TextContent("Choose one or more targets to deploy to."), new GUILayoutOption[0]);
            EditorGUILayout.Space();
            this.RefreshDeviceList();
            GUIStyle style = "box";
            int num = 0;
            IEnumerator enumerator = ListViewGUILayout.ListView(this.m_ListView, style, new GUILayoutOption[0]).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    ListViewElement current = (ListViewElement) enumerator.Current;
                    if ((current.row == this.m_ListView.row) && (Event.current.type == EventType.Repaint))
                    {
                        style.Draw(current.position, false, false, false, false);
                    }
                    if (ListViewGUILayout.HasMouseUp(current.position))
                    {
                        this.target = (string) this.m_DeviceList[num];
                    }
                    GUILayout.Label((string) this.m_DeviceList[num++], new GUILayoutOption[0]);
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
            GUILayoutOption[] options = new GUILayoutOption[] { btnWidth };
            if (GUILayout.Button(EditorGUIUtility.TextContent("Add"), options))
            {
                m_STVEditorExt.SetTVDeviceAddressString(this.target.Remove(this.target.IndexOf(" :: ")));
            }
        }

        private void OnReceivedBroadcast(string fromAddress, string data)
        {
            fromAddress = fromAddress.Substring(fromAddress.LastIndexOf(":") + 1);
            string item = fromAddress + " :: " + data;
            if (!this.m_DeviceList.Contains(item))
            {
                this.m_DeviceList.Add(item);
            }
        }

        private void RefreshDeviceList()
        {
            this.CheckForDevices();
            this.m_ListView.totalRows = this.m_DeviceList.Count;
        }

        internal static void ShowWindow(SamsungTVSettingsEditorExtension editorExt)
        {
            DeploymentTargetBrowser[] browserArray = (DeploymentTargetBrowser[]) UnityEngine.Resources.FindObjectsOfTypeAll(typeof(DeploymentTargetBrowser));
            DeploymentTargetBrowser browser = (browserArray.Length <= 0) ? ScriptableObject.CreateInstance<DeploymentTargetBrowser>() : browserArray[0];
            m_STVEditorExt = editorExt;
            if (browserArray.Length > 0)
            {
                browser.Focus();
            }
            else
            {
                browser.titleContent = EditorGUIUtility.TextContent("Discover Tizen Devices");
                browser.position = new Rect(100f, 100f, 500f, 250f);
                browser.minSize = new Vector2(browser.position.width, browser.position.height);
                browser.maxSize = browser.minSize;
                browser.ShowUtility();
            }
        }
    }
}

