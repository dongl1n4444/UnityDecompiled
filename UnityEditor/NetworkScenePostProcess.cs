namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditor.Callbacks;
    using UnityEngine;
    using UnityEngine.Networking;

    public class NetworkScenePostProcess : MonoBehaviour
    {
        [PostProcessScene]
        public static void OnPostProcessScene()
        {
            HashSet<string> set = new HashSet<string>();
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
                    GameObject prefabParent = PrefabUtility.GetPrefabParent(identity.gameObject) as GameObject;
                    if (prefabParent != null)
                    {
                        GameObject obj3 = PrefabUtility.FindPrefabRoot(prefabParent);
                        if ((obj3 != null) && ((obj3.GetComponentsInChildren<NetworkIdentity>().Length > 1) && !set.Contains(obj3.name)))
                        {
                            set.Add(obj3.name);
                            object[] args = new object[] { obj3.name };
                            Debug.LogWarningFormat("Prefab '{0}' has several NetworkIdentity components attached to itself or its children, this is not supported.", args);
                        }
                    }
                }
            }
        }
    }
}

