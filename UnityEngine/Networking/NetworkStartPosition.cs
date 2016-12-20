namespace UnityEngine.Networking
{
    using System;
    using UnityEngine;

    /// <summary>
    /// <para>This component is used to make a gameObject a starting position for spawning player objects in multiplayer games.</para>
    /// </summary>
    [DisallowMultipleComponent, AddComponentMenu("Network/NetworkStartPosition")]
    public class NetworkStartPosition : MonoBehaviour
    {
        public void Awake()
        {
            NetworkManager.RegisterStartPosition(base.transform);
        }

        public void OnDestroy()
        {
            NetworkManager.UnRegisterStartPosition(base.transform);
        }
    }
}

