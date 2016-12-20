namespace UnityEngine.Networking
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// <para>Signature of spawn functions that are passed to NetworkClient.RegisterSpawnFunction(). This is optional, as in most cases RegisterPrefab will be used instead.</para>
    /// </summary>
    /// <param name="position"></param>
    /// <param name="assetId"></param>
    public delegate GameObject SpawnDelegate(Vector3 position, NetworkHash128 assetId);
}

