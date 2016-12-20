namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;
    using UnityEngine.Networking.NetworkSystem;

    /// <summary>
    /// <para>This class holds information about which networked scripts use which QoS channels for updates.</para>
    /// </summary>
    public class NetworkCRC
    {
        private bool m_ScriptCRCCheck;
        private Dictionary<string, int> m_Scripts = new Dictionary<string, int>();
        internal static NetworkCRC s_Singleton;

        private void Dump(CRCMessageEntry[] remoteScripts)
        {
            foreach (string str in this.m_Scripts.Keys)
            {
                Debug.Log(string.Concat(new object[] { "CRC Local Dump ", str, " : ", this.m_Scripts[str] }));
            }
            foreach (CRCMessageEntry entry in remoteScripts)
            {
                Debug.Log(string.Concat(new object[] { "CRC Remote Dump ", entry.name, " : ", entry.channel }));
            }
        }

        /// <summary>
        /// <para>This is used to setup script network settings CRC data.</para>
        /// </summary>
        /// <param name="name">Script name.</param>
        /// <param name="channel">QoS Channel.</param>
        public static void RegisterBehaviour(string name, int channel)
        {
            singleton.m_Scripts[name] = channel;
        }

        public static void ReinitializeScriptCRCs(Assembly callingAssembly)
        {
            singleton.m_Scripts.Clear();
            foreach (System.Type type in callingAssembly.GetTypes())
            {
                if (DotNetCompatibility.GetBaseType(type) == typeof(NetworkBehaviour))
                {
                    MethodInfo method = type.GetMethod(".cctor", BindingFlags.Static);
                    if (method != null)
                    {
                        method.Invoke(null, new object[0]);
                    }
                }
            }
        }

        internal static bool Validate(CRCMessageEntry[] scripts, int numChannels)
        {
            return singleton.ValidateInternal(scripts, numChannels);
        }

        private bool ValidateInternal(CRCMessageEntry[] remoteScripts, int numChannels)
        {
            if (this.m_Scripts.Count != remoteScripts.Length)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("Network configuration mismatch detected. The number of networked scripts on the client does not match the number of networked scripts on the server. This could be caused by lazy loading of scripts on the client. This warning can be disabled by the checkbox in NetworkManager Script CRC Check.");
                }
                this.Dump(remoteScripts);
                return false;
            }
            for (int i = 0; i < remoteScripts.Length; i++)
            {
                CRCMessageEntry entry = remoteScripts[i];
                if (LogFilter.logDebug)
                {
                    Debug.Log(string.Concat(new object[] { "Script: ", entry.name, " Channel: ", entry.channel }));
                }
                if (this.m_Scripts.ContainsKey(entry.name))
                {
                    int num2 = this.m_Scripts[entry.name];
                    if (num2 != entry.channel)
                    {
                        if (LogFilter.logError)
                        {
                            Debug.LogError(string.Concat(new object[] { "HLAPI CRC Channel Mismatch. Script: ", entry.name, " LocalChannel: ", num2, " RemoteChannel: ", entry.channel }));
                        }
                        this.Dump(remoteScripts);
                        return false;
                    }
                }
                if (entry.channel >= numChannels)
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError(string.Concat(new object[] { "HLAPI CRC channel out of range! Script: ", entry.name, " Channel: ", entry.channel }));
                    }
                    this.Dump(remoteScripts);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// <para>Enables a CRC check between server and client that ensures the NetworkBehaviour scripts match.</para>
        /// </summary>
        public static bool scriptCRCCheck
        {
            get
            {
                return singleton.m_ScriptCRCCheck;
            }
            set
            {
                singleton.m_ScriptCRCCheck = value;
            }
        }

        /// <summary>
        /// <para>A dictionary of script QoS channels.</para>
        /// </summary>
        public Dictionary<string, int> scripts
        {
            get
            {
                return this.m_Scripts;
            }
        }

        internal static NetworkCRC singleton
        {
            get
            {
                if (s_Singleton == null)
                {
                    s_Singleton = new NetworkCRC();
                }
                return s_Singleton;
            }
        }
    }
}

