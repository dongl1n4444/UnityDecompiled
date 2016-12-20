namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>Describes status messages from the master server as returned in MonoBehaviour.OnMasterServerEvent|OnMasterServerEvent.</para>
    /// </summary>
    public enum MasterServerEvent
    {
        RegistrationFailedGameName,
        RegistrationFailedGameType,
        RegistrationFailedNoServer,
        RegistrationSucceeded,
        HostListReceived
    }
}

