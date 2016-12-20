namespace UnityEditor
{
    using System;
    using UnityEditor.Callbacks;
    using UnityEngine;
    using UnityEngine.Networking;

    public class NetworkScenePostProcess : MonoBehaviour
    {
        [PostProcessScene]
        public static void OnPostProcessScene()
        {
            int num = 1;
            foreach (NetworkIdentity identity in UnityEngine.Object.FindObjectsOfType<NetworkIdentity>())
            {
                if (identity.GetComponent<NetworkManager>() != null)
                {
                    Debug.LogError("NetworkManager has a NetworkIdentity component. This will cause the NetworkManager object to be disabled, so it is not recommended.");
                }
                if (!identity.isClient && !identity.isServer)
                {
                    identity.gameObject.SetActive(false);
                    identity.ForceSceneId(num++);
                }
            }
        }
    }
}

