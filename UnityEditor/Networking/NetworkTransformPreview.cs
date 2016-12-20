namespace UnityEditor.Networking
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Networking;

    [CustomPreview(typeof(GameObject))]
    internal class NetworkTransformPreview : ObjectPreview
    {
        private Rigidbody2D m_Rigidbody2D;
        private Rigidbody m_Rigidbody3D;
        private GUIContent m_Title;
        private NetworkTransform m_Transform;

        private void GetNetworkInformation(GameObject gameObject)
        {
            this.m_Transform = gameObject.GetComponent<NetworkTransform>();
            this.m_Rigidbody3D = gameObject.GetComponent<Rigidbody>();
            this.m_Rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        }

        public override GUIContent GetPreviewTitle()
        {
            if (this.m_Title == null)
            {
                this.m_Title = new GUIContent("Network Transform");
            }
            return this.m_Title;
        }

        public override bool HasPreviewGUI()
        {
            return (this.m_Transform != null);
        }

        public override void Initialize(UnityEngine.Object[] targets)
        {
            base.Initialize(targets);
            this.GetNetworkInformation(this.target as GameObject);
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if ((Event.current.type == EventType.Repaint) && (this.m_Transform != null))
            {
                int num = 4;
                Vector3 vector = this.m_Transform.transform.position - this.m_Transform.targetSyncPosition;
                float magnitude = vector.magnitude;
                GUI.Label(new Rect(r.xMin + 4f, r.y + num, 600f, 20f), string.Concat(new object[] { "Position: ", this.m_Transform.transform.position, " Target: ", this.m_Transform.targetSyncPosition, " Diff: ", magnitude }));
                num += 20;
                if (this.m_Rigidbody3D != null)
                {
                    float num3 = Quaternion.Angle(this.m_Transform.rigidbody3D.rotation, this.m_Transform.targetSyncRotation3D);
                    GUI.Label(new Rect(r.xMin + 4f, r.y + num, 600f, 20f), string.Concat(new object[] { "Angle: ", this.m_Transform.rigidbody3D.rotation, " Target: ", this.m_Transform.targetSyncRotation3D, " Diff: ", num3 }));
                    num += 20;
                    GUI.Label(new Rect(r.xMin + 4f, r.y + num, 600f, 20f), string.Concat(new object[] { "Velocity: ", this.m_Transform.rigidbody3D.velocity, " Target: ", this.m_Transform.targetSyncVelocity }));
                    num += 20;
                }
                if (this.m_Rigidbody2D != null)
                {
                    float num4 = this.m_Transform.rigidbody2D.rotation - this.m_Transform.targetSyncRotation2D;
                    GUI.Label(new Rect(r.xMin + 4f, r.y + num, 600f, 20f), string.Concat(new object[] { "Angle: ", this.m_Transform.rigidbody2D.rotation, " Target: ", this.m_Transform.targetSyncRotation2D, " Diff: ", num4 }));
                    num += 20;
                    GUI.Label(new Rect(r.xMin + 4f, r.y + num, 600f, 20f), string.Concat(new object[] { "Velocity: ", this.m_Transform.rigidbody2D.velocity, " Target: ", this.m_Transform.targetSyncVelocity }));
                    num += 20;
                }
                GUI.Label(new Rect(r.xMin + 4f, r.y + num, 200f, 20f), "Last SyncTime: " + (Time.time - this.m_Transform.lastSyncTime));
                num += 20;
            }
        }
    }
}

