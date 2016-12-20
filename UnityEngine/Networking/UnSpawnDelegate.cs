namespace UnityEngine.Networking
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// <para>Delegate for a function which will handle destruction of objects created with NetworkServer.Spawn.</para>
    /// </summary>
    /// <param name="spawned"></param>
    public delegate void UnSpawnDelegate(GameObject spawned);
}

