namespace UnityEditor.HolographicEmulation
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.VR;

    internal class HolographicEmulationWindow : EditorWindow
    {
        private HolographicStreamerConnectionState m_ConnectionState = HolographicStreamerConnectionState.Disconnected;
        [SerializeField]
        private bool m_EnableAudio = true;
        [SerializeField]
        private bool m_EnableVideo = true;
        [SerializeField]
        private GestureHand m_Hand = GestureHand.Right;
        private bool m_InPlayMode = false;
        [SerializeField]
        private int m_MaxBitrateKbps = 0x1869f;
        [SerializeField]
        private EmulationMode m_Mode = EmulationMode.None;
        private bool m_OperatingSystemChecked = false;
        private bool m_OperatingSystemValid = false;
        [SerializeField]
        private string m_RemoteMachineAddress = "";
        private string[] m_RemoteMachineHistory;
        [SerializeField]
        private int m_RoomIndex = 0;
        private static Texture2D s_ConnectedTexture = null;
        private static Texture2D s_ConnectingTexture = null;
        private static GUIContent s_ConnectionButtonConnectText = new GUIContent("Connect");
        private static GUIContent s_ConnectionButtonDisconnectText = new GUIContent("Disconnect");
        private static GUIContent s_ConnectionStateConnectedText = new GUIContent("Connected");
        private static GUIContent s_ConnectionStateConnectingText = new GUIContent("Connecting");
        private static GUIContent s_ConnectionStateDisconnectedText = new GUIContent("Disconnected");
        private static GUIContent s_ConnectionStatusText = new GUIContent("Connection Status");
        private static Texture2D s_DisconnectedTexture = null;
        private static GUIContent s_EmulationModeText = new GUIContent("Emulation Mode");
        private static GUIContent s_EnableAudioText = new GUIContent("Enable Audio");
        private static GUIContent s_EnableVideoText = new GUIContent("Enable Video");
        private static GUIContent[] s_HandStrings = new GUIContent[] { new GUIContent("Left Hand"), new GUIContent("Right Hand") };
        private static GUIContent s_HandText = new GUIContent("Gesture Hand");
        private static GUIContent s_MaxBitrateText = new GUIContent("Max Bitrate (kbps)");
        private static int s_MaxHistoryLength = 4;
        private static GUIContent[] s_ModeStrings = new GUIContent[] { new GUIContent("None"), new GUIContent("Remote to Device"), new GUIContent("Simulate in Editor") };
        private static GUIContent s_RemoteMachineText = new GUIContent("Remote Machine");
        private static GUIContent[] s_RoomStrings = new GUIContent[] { new GUIContent("DefaultRoom"), new GUIContent("Bedroom1"), new GUIContent("Bedroom2"), new GUIContent("GreatRoom"), new GUIContent("LivingRoom") };
        private static GUIContent s_RoomText = new GUIContent("Room");

        private bool CheckOperatingSystem()
        {
            if (!this.m_OperatingSystemChecked)
            {
                this.m_OperatingSystemValid = Environment.OSVersion.Version.Build >= 0x37ee;
                this.m_OperatingSystemChecked = true;
            }
            return this.m_OperatingSystemValid;
        }

        private void Connect()
        {
            PerceptionRemotingPlugin.SetVideoEncodingParameters(this.m_MaxBitrateKbps);
            PerceptionRemotingPlugin.SetEnableVideo(this.m_EnableVideo);
            PerceptionRemotingPlugin.SetEnableAudio(this.m_EnableAudio);
            PerceptionRemotingPlugin.Connect(this.m_RemoteMachineAddress);
        }

        private void ConnectionStateGUI()
        {
            Texture2D textured;
            GUIContent content;
            GUIContent content2;
            if (s_ConnectedTexture == null)
            {
                s_ConnectedTexture = EditorGUIUtility.LoadIconRequired("sv_icon_dot3_sml");
                s_ConnectingTexture = EditorGUIUtility.LoadIconRequired("sv_icon_dot4_sml");
                s_DisconnectedTexture = EditorGUIUtility.LoadIconRequired("sv_icon_dot6_sml");
            }
            switch (this.m_ConnectionState)
            {
                case HolographicStreamerConnectionState.Connecting:
                    textured = s_ConnectingTexture;
                    content = s_ConnectionStateConnectingText;
                    content2 = s_ConnectionButtonDisconnectText;
                    break;

                case HolographicStreamerConnectionState.Connected:
                    textured = s_ConnectedTexture;
                    content = s_ConnectionStateConnectedText;
                    content2 = s_ConnectionButtonDisconnectText;
                    break;

                default:
                    textured = s_DisconnectedTexture;
                    content = s_ConnectionStateDisconnectedText;
                    content2 = s_ConnectionButtonConnectText;
                    break;
            }
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUILayout.PrefixLabel(s_ConnectionStatusText, "Button");
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            GUI.DrawTexture(GUILayoutUtility.GetRect(singleLineHeight, singleLineHeight, options), textured);
            EditorGUILayout.LabelField(content, new GUILayoutOption[0]);
            EditorGUILayout.EndHorizontal();
            EditorGUI.BeginDisabledGroup(this.m_InPlayMode);
            bool flag = EditorGUILayout.DropdownButton(content2, FocusType.Passive, EditorStyles.miniButton, new GUILayoutOption[0]);
            EditorGUI.EndDisabledGroup();
            if (flag)
            {
                this.HandleButtonPress();
            }
        }

        private void Disconnect()
        {
            PerceptionRemotingPlugin.Disconnect();
        }

        private void DrawRemotingMode()
        {
            this.m_Mode = (EmulationMode) EditorGUILayout.Popup(s_EmulationModeText, (int) this.m_Mode, s_ModeStrings, new GUILayoutOption[0]);
        }

        private void HandleButtonPress()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogError("Unable to connect / disconnect remoting while playing.");
            }
            else if ((this.m_ConnectionState == HolographicStreamerConnectionState.Connecting) || (this.m_ConnectionState == HolographicStreamerConnectionState.Connected))
            {
                this.Disconnect();
            }
            else if (this.RemoteMachineNameSpecified)
            {
                this.Connect();
            }
            else
            {
                Debug.LogError("Cannot connect without a remote machine address specified");
            }
        }

        public static void Init()
        {
            EditorWindow.GetWindow<HolographicEmulationWindow>(false).titleContent = new GUIContent("Holographic");
        }

        private void InitializeSimulation()
        {
            if (this.m_ConnectionState != HolographicStreamerConnectionState.Disconnected)
            {
                this.Disconnect();
            }
            UnityEditor.HolographicEmulation.HolographicEmulation.Initialize();
            this.LoadCurrentRoom();
        }

        private bool IsHoloLensCurrentTarget()
        {
            if (!PlayerSettings.virtualRealitySupported)
            {
                return false;
            }
            if (Array.IndexOf<string>(VRSettings.supportedDevices, "HoloLens") < 0)
            {
                return false;
            }
            return true;
        }

        private void LoadCurrentRoom()
        {
            UnityEditor.HolographicEmulation.HolographicEmulation.LoadRoom((EditorApplication.applicationContentsPath + "/UnityExtensions/Unity/VR/HolographicSimulation/Rooms/") + s_RoomStrings[this.m_RoomIndex].text + ".xef");
        }

        private void OnDisable()
        {
            EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.OnPlayModeChanged));
        }

        private void OnEnable()
        {
            EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.OnPlayModeChanged));
            this.m_InPlayMode = EditorApplication.isPlayingOrWillChangePlaymode;
            char[] separator = new char[] { ',' };
            this.m_RemoteMachineHistory = EditorPrefs.GetString("HolographicRemoting.RemoteMachineHistory").Split(separator);
        }

        private void OnGUI()
        {
            if (!this.CheckOperatingSystem())
            {
                EditorGUILayout.HelpBox("You must be running Windows build 14318 or later to use Holographic Simulation or Remoting.", MessageType.Warning);
            }
            else if (!this.IsHoloLensCurrentTarget())
            {
                EditorGUILayout.HelpBox("You must enable Virtual Reality support in settings and add Windows Holographic to the devices to use Holographic Emulation.", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.Space();
                EditorGUI.BeginDisabledGroup(this.m_InPlayMode);
                this.DrawRemotingMode();
                EditorGUI.EndDisabledGroup();
                switch (this.m_Mode)
                {
                    case EmulationMode.RemoteDevice:
                        EditorGUI.BeginDisabledGroup(this.m_ConnectionState != HolographicStreamerConnectionState.Disconnected);
                        this.RemotingPreferencesOnGUI();
                        EditorGUI.EndDisabledGroup();
                        this.ConnectionStateGUI();
                        break;

                    case EmulationMode.Simulated:
                        EditorGUI.BeginChangeCheck();
                        this.m_RoomIndex = EditorGUILayout.Popup(s_RoomText, this.m_RoomIndex, s_RoomStrings, new GUILayoutOption[0]);
                        if (EditorGUI.EndChangeCheck() && this.m_InPlayMode)
                        {
                            this.LoadCurrentRoom();
                        }
                        EditorGUI.BeginChangeCheck();
                        this.m_Hand = (GestureHand) EditorGUILayout.Popup(s_HandText, (int) this.m_Hand, s_HandStrings, new GUILayoutOption[0]);
                        if (EditorGUI.EndChangeCheck())
                        {
                            UnityEditor.HolographicEmulation.HolographicEmulation.SetGestureHand(this.m_Hand);
                        }
                        break;
                }
            }
        }

        private void OnPlayModeChanged()
        {
            bool inPlayMode = this.m_InPlayMode;
            this.m_InPlayMode = EditorApplication.isPlayingOrWillChangePlaymode;
            if (this.m_InPlayMode && !inPlayMode)
            {
                UnityEditor.HolographicEmulation.HolographicEmulation.SetEmulationMode(this.m_Mode);
                switch (this.m_Mode)
                {
                    case EmulationMode.Simulated:
                        this.InitializeSimulation();
                        break;
                }
            }
            else if (!this.m_InPlayMode && inPlayMode)
            {
                switch (this.m_Mode)
                {
                    case EmulationMode.Simulated:
                        UnityEditor.HolographicEmulation.HolographicEmulation.Shutdown();
                        break;
                }
            }
        }

        private void RemotingPreferencesOnGUI()
        {
            EditorGUI.BeginChangeCheck();
            this.m_RemoteMachineAddress = EditorGUILayout.DelayedTextFieldDropDown(s_RemoteMachineText, this.m_RemoteMachineAddress, this.m_RemoteMachineHistory);
            if (EditorGUI.EndChangeCheck())
            {
                this.UpdateRemoteMachineHistory();
            }
            this.m_EnableVideo = EditorGUILayout.Toggle(s_EnableVideoText, this.m_EnableVideo, new GUILayoutOption[0]);
            this.m_EnableAudio = EditorGUILayout.Toggle(s_EnableAudioText, this.m_EnableAudio, new GUILayoutOption[0]);
            this.m_MaxBitrateKbps = EditorGUILayout.IntSlider(s_MaxBitrateText, this.m_MaxBitrateKbps, 0x400, 0x1869f, new GUILayoutOption[0]);
        }

        private void Update()
        {
            switch (this.m_Mode)
            {
                case EmulationMode.Simulated:
                    UnityEditor.HolographicEmulation.HolographicEmulation.SetGestureHand(this.m_Hand);
                    break;

                case EmulationMode.RemoteDevice:
                {
                    HolographicStreamerConnectionState connectionState = this.m_ConnectionState;
                    this.m_ConnectionState = PerceptionRemotingPlugin.GetConnectionState();
                    if (connectionState != this.m_ConnectionState)
                    {
                        base.Repaint();
                    }
                    HolographicStreamerConnectionFailureReason reason = PerceptionRemotingPlugin.CheckForDisconnect();
                    if ((reason == HolographicStreamerConnectionFailureReason.Unreachable) || (reason == HolographicStreamerConnectionFailureReason.ConnectionLost))
                    {
                        Debug.LogWarning("Disconnected with failure reason " + reason + ", attempting to reconnect.");
                        this.Connect();
                    }
                    else if (reason != HolographicStreamerConnectionFailureReason.None)
                    {
                        Debug.LogError("Disconnected with error " + reason);
                    }
                    break;
                }
            }
        }

        private void UpdateRemoteMachineHistory()
        {
            List<string> list = new List<string>(this.m_RemoteMachineHistory);
            for (int i = 0; i < this.m_RemoteMachineHistory.Length; i++)
            {
                if (this.m_RemoteMachineHistory[i].Equals(this.m_RemoteMachineAddress, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (i == 0)
                    {
                        return;
                    }
                    list.RemoveAt(i);
                    break;
                }
            }
            list.Insert(0, this.m_RemoteMachineAddress);
            if (list.Count > s_MaxHistoryLength)
            {
                list.RemoveRange(s_MaxHistoryLength, list.Count - s_MaxHistoryLength);
            }
            this.m_RemoteMachineHistory = list.ToArray();
            EditorPrefs.SetString("HolographicRemoting.RemoteMachineHistory", string.Join(",", this.m_RemoteMachineHistory));
        }

        private bool RemoteMachineNameSpecified =>
            !string.IsNullOrEmpty(this.m_RemoteMachineAddress);
    }
}

