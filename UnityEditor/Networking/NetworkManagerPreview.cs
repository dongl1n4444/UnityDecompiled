namespace UnityEditor.Networking
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Networking;

    [CustomPreview(typeof(NetworkManager))]
    internal class NetworkManagerPreview : ObjectPreview
    {
        private const int k_ColumnWidth = 120;
        private const int k_Padding = 4;
        private const int k_RowHeight = 0x10;
        private NetworkManager m_Manager;
        protected GUIContent m_ShowClientMessagesLabel;
        protected GUIContent m_ShowServerMessagesLabel;
        private GUIContent m_Title;

        private static string FormatHandler(KeyValuePair<short, NetworkMessageDelegate> handler)
        {
            return string.Format("{0}:{1}()", handler.Value.Method.DeclaringType.Name, handler.Value.Method.Name);
        }

        private void GetNetworkInformation(NetworkManager man)
        {
            this.m_Manager = man;
        }

        public override GUIContent GetPreviewTitle()
        {
            if (this.m_Title == null)
            {
                this.m_Title = new GUIContent("NetworkManager Message Handlers");
            }
            return this.m_Title;
        }

        public override bool HasPreviewGUI()
        {
            return (this.m_Manager != null);
        }

        public override void Initialize(UnityEngine.Object[] targets)
        {
            base.Initialize(targets);
            this.GetNetworkInformation(this.target as NetworkManager);
            this.m_ShowServerMessagesLabel = new GUIContent("Server Message Handlers:", "Registered network message handler functions");
            this.m_ShowClientMessagesLabel = new GUIContent("Client Message Handlers:", "Registered network message handler functions");
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if ((Event.current.type == EventType.Repaint) && (this.m_Manager != null))
            {
                int posY = (int) (r.yMin + 4f);
                posY = this.ShowServerMessageHandlers(r, posY);
                posY = this.ShowClientMessageHandlers(r, posY);
            }
        }

        private int ShowClientMessageHandlers(Rect r, int posY)
        {
            if (NetworkClient.allClients.Count != 0)
            {
                NetworkClient client = NetworkClient.allClients[0];
                if (client == null)
                {
                    return posY;
                }
                GUI.Label(new Rect(r.xMin + 4f, (float) posY, 400f, 16f), this.m_ShowClientMessagesLabel);
                posY += 0x10;
                foreach (KeyValuePair<short, NetworkMessageDelegate> pair in client.handlers)
                {
                    GUI.Label(new Rect(r.xMin + 16f, (float) posY, 400f, 16f), MsgType.MsgTypeToString(pair.Key));
                    GUI.Label(new Rect((r.xMin + 16f) + 120f, (float) posY, 400f, 16f), FormatHandler(pair));
                    posY += 0x10;
                }
            }
            return posY;
        }

        private int ShowServerMessageHandlers(Rect r, int posY)
        {
            if (NetworkServer.handlers.Count != 0)
            {
                GUI.Label(new Rect(r.xMin + 4f, (float) posY, 400f, 16f), this.m_ShowServerMessagesLabel);
                posY += 0x10;
                foreach (KeyValuePair<short, NetworkMessageDelegate> pair in NetworkServer.handlers)
                {
                    GUI.Label(new Rect(r.xMin + 16f, (float) posY, 400f, 16f), MsgType.MsgTypeToString(pair.Key));
                    GUI.Label(new Rect((r.xMin + 16f) + 120f, (float) posY, 400f, 16f), FormatHandler(pair));
                    posY += 0x10;
                }
            }
            return posY;
        }
    }
}

