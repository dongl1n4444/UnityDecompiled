namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// <para>Component that controls visibility of networked objects for players.</para>
    /// </summary>
    [AddComponentMenu("Network/NetworkProximityChecker"), RequireComponent(typeof(NetworkIdentity))]
    public class NetworkProximityChecker : NetworkBehaviour
    {
        /// <summary>
        /// <para>Which method to use for checking proximity of players.</para>
        /// </summary>
        public CheckMethod checkMethod = CheckMethod.Physics3D;
        /// <summary>
        /// <para>Flag to force this object to be hidden for players.</para>
        /// </summary>
        public bool forceHidden = false;
        private float m_VisUpdateTime;
        /// <summary>
        /// <para>The maximim range that objects will be visible at.</para>
        /// </summary>
        public int visRange = 10;
        /// <summary>
        /// <para>How often (in seconds) that this object should update the set of players that can see it.</para>
        /// </summary>
        public float visUpdateInterval = 1f;

        public override bool OnCheckObserver(NetworkConnection newObserver)
        {
            if (this.forceHidden)
            {
                return false;
            }
            GameObject gameObject = null;
            for (int i = 0; i < newObserver.playerControllers.Count; i++)
            {
                PlayerController controller = newObserver.playerControllers[i];
                if ((controller != null) && (controller.gameObject != null))
                {
                    gameObject = controller.gameObject;
                    break;
                }
            }
            if (gameObject == null)
            {
                return false;
            }
            Vector3 vector2 = gameObject.transform.position - base.transform.position;
            return (vector2.magnitude < this.visRange);
        }

        public override bool OnRebuildObservers(HashSet<NetworkConnection> observers, bool initial)
        {
            if (this.forceHidden)
            {
                NetworkIdentity component = base.GetComponent<NetworkIdentity>();
                if (component.connectionToClient != null)
                {
                    observers.Add(component.connectionToClient);
                }
                return true;
            }
            CheckMethod checkMethod = this.checkMethod;
            if (checkMethod != CheckMethod.Physics3D)
            {
                if (checkMethod != CheckMethod.Physics2D)
                {
                    return false;
                }
            }
            else
            {
                foreach (Collider collider in Physics.OverlapSphere(base.transform.position, (float) this.visRange))
                {
                    NetworkIdentity identity2 = collider.GetComponent<NetworkIdentity>();
                    if ((identity2 != null) && (identity2.connectionToClient != null))
                    {
                        observers.Add(identity2.connectionToClient);
                    }
                }
                return true;
            }
            foreach (Collider2D colliderd in Physics2D.OverlapCircleAll(base.transform.position, (float) this.visRange))
            {
                NetworkIdentity identity3 = colliderd.GetComponent<NetworkIdentity>();
                if ((identity3 != null) && (identity3.connectionToClient != null))
                {
                    observers.Add(identity3.connectionToClient);
                }
            }
            return true;
        }

        public override void OnSetLocalVisibility(bool vis)
        {
            SetVis(base.gameObject, vis);
        }

        private static void SetVis(GameObject go, bool vis)
        {
            foreach (Renderer renderer in go.GetComponents<Renderer>())
            {
                renderer.enabled = vis;
            }
            for (int i = 0; i < go.transform.childCount; i++)
            {
                SetVis(go.transform.GetChild(i).gameObject, vis);
            }
        }

        private void Update()
        {
            if (NetworkServer.active && ((Time.time - this.m_VisUpdateTime) > this.visUpdateInterval))
            {
                base.GetComponent<NetworkIdentity>().RebuildObservers(false);
                this.m_VisUpdateTime = Time.time;
            }
        }

        /// <summary>
        /// <para>Enumeration of methods to use to check proximity.</para>
        /// </summary>
        public enum CheckMethod
        {
            Physics3D,
            Physics2D
        }
    }
}

