namespace UnityEditor.Networking
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Networking;

    [CustomPreview(typeof(GameObject))]
    internal class NetworkInformationPreview : ObjectPreview
    {
        private List<NetworkBehaviourInfo> m_Behaviours;
        private NetworkIdentity m_Identity;
        private List<NetworkIdentityInfo> m_Info;
        private Styles m_Styles = new Styles();
        private GUIContent m_Title;

        private NetworkIdentityInfo GetAssetId()
        {
            string str = this.m_Identity.assetId.ToString();
            if (string.IsNullOrEmpty(str))
            {
                str = "<object has no prefab>";
            }
            return GetString("Asset ID", str);
        }

        private static NetworkIdentityInfo GetBoolean(string name, bool value) => 
            new NetworkIdentityInfo { 
                name = new GUIContent(name),
                value = new GUIContent(!value ? "No" : "Yes")
            };

        private Vector2 GetMaxBehaviourLabelSize()
        {
            Vector2 zero = Vector2.zero;
            foreach (NetworkBehaviourInfo info in this.m_Behaviours)
            {
                Vector2 vector2 = this.m_Styles.labelStyle.CalcSize(info.name);
                if (zero.x < vector2.x)
                {
                    zero.x = vector2.x;
                }
                if (zero.y < vector2.y)
                {
                    zero.y = vector2.y;
                }
            }
            return zero;
        }

        private Vector2 GetMaxNameLabelSize()
        {
            Vector2 zero = Vector2.zero;
            foreach (NetworkIdentityInfo info in this.m_Info)
            {
                Vector2 vector2 = this.m_Styles.labelStyle.CalcSize(info.value);
                if (zero.x < vector2.x)
                {
                    zero.x = vector2.x;
                }
                if (zero.y < vector2.y)
                {
                    zero.y = vector2.y;
                }
            }
            return zero;
        }

        private void GetNetworkInformation(GameObject gameObject)
        {
            this.m_Identity = gameObject.GetComponent<NetworkIdentity>();
            if (this.m_Identity != null)
            {
                this.m_Info = new List<NetworkIdentityInfo>();
                this.m_Info.Add(this.GetAssetId());
                this.m_Info.Add(GetString("Scene ID", this.m_Identity.sceneId.ToString()));
                if (Application.isPlaying)
                {
                    this.m_Info.Add(GetString("Network ID", this.m_Identity.netId.ToString()));
                    this.m_Info.Add(GetString("Player Controller ID", this.m_Identity.playerControllerId.ToString()));
                    this.m_Info.Add(GetBoolean("Is Client", this.m_Identity.isClient));
                    this.m_Info.Add(GetBoolean("Is Server", this.m_Identity.isServer));
                    this.m_Info.Add(GetBoolean("Has Authority", this.m_Identity.hasAuthority));
                    this.m_Info.Add(GetBoolean("Is Local Player", this.m_Identity.isLocalPlayer));
                    NetworkBehaviour[] components = gameObject.GetComponents<NetworkBehaviour>();
                    if (components.Length > 0)
                    {
                        this.m_Behaviours = new List<NetworkBehaviourInfo>();
                        foreach (NetworkBehaviour behaviour in components)
                        {
                            NetworkBehaviourInfo item = new NetworkBehaviourInfo {
                                name = new GUIContent(behaviour.GetType().FullName),
                                behaviour = behaviour
                            };
                            this.m_Behaviours.Add(item);
                        }
                    }
                }
            }
        }

        public override GUIContent GetPreviewTitle()
        {
            if (this.m_Title == null)
            {
                this.m_Title = new GUIContent("Network Information");
            }
            return this.m_Title;
        }

        private static NetworkIdentityInfo GetString(string name, string value) => 
            new NetworkIdentityInfo { 
                name = new GUIContent(name),
                value = new GUIContent(value)
            };

        public override bool HasPreviewGUI() => 
            ((this.m_Info != null) && (this.m_Info.Count > 0));

        public override void Initialize(UnityEngine.Object[] targets)
        {
            base.Initialize(targets);
            this.GetNetworkInformation(this.target as GameObject);
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (((Event.current.type == EventType.Repaint) && (this.m_Info != null)) && (this.m_Info.Count != 0))
            {
                if (this.m_Styles == null)
                {
                    this.m_Styles = new Styles();
                }
                Vector2 vector = new Vector2(140f, 16f);
                Vector2 maxNameLabelSize = this.GetMaxNameLabelSize();
                r = new RectOffset(-5, -5, -5, -5).Add(r);
                float x = r.x + 10f;
                float y = r.y + 10f;
                Rect position = new Rect(x, y, vector.x, vector.y);
                Rect rect2 = new Rect(vector.x, y, maxNameLabelSize.x, maxNameLabelSize.y);
                foreach (NetworkIdentityInfo info in this.m_Info)
                {
                    GUI.Label(position, info.name, this.m_Styles.labelStyle);
                    GUI.Label(rect2, info.value, this.m_Styles.componentName);
                    position.y += position.height;
                    position.x = x;
                    rect2.y += rect2.height;
                }
                float num3 = position.y;
                if ((this.m_Behaviours != null) && (this.m_Behaviours.Count > 0))
                {
                    Vector2 maxBehaviourLabelSize = this.GetMaxBehaviourLabelSize();
                    Rect rect3 = new Rect(x, position.y + 10f, maxBehaviourLabelSize.x, maxBehaviourLabelSize.y);
                    GUI.Label(rect3, new GUIContent("Network Behaviours"), this.m_Styles.labelStyle);
                    rect3.x += 20f;
                    rect3.y += rect3.height;
                    foreach (NetworkBehaviourInfo info2 in this.m_Behaviours)
                    {
                        if (info2.behaviour != null)
                        {
                            if (info2.behaviour.enabled)
                            {
                                GUI.Label(rect3, info2.name, this.m_Styles.componentName);
                            }
                            else
                            {
                                GUI.Label(rect3, info2.name, this.m_Styles.disabledName);
                            }
                            rect3.y += rect3.height;
                            num3 = rect3.y;
                        }
                    }
                    if ((this.m_Identity.observers != null) && (this.m_Identity.observers.Count > 0))
                    {
                        Rect rect4 = new Rect(x, num3 + 10f, 200f, 20f);
                        GUI.Label(rect4, new GUIContent("Network observers"), this.m_Styles.labelStyle);
                        rect4.x += 20f;
                        rect4.y += rect4.height;
                        foreach (NetworkConnection connection in this.m_Identity.observers)
                        {
                            GUI.Label(rect4, connection.address + ":" + connection.connectionId, this.m_Styles.componentName);
                            rect4.y += rect4.height;
                            num3 = rect4.y;
                        }
                    }
                    if (this.m_Identity.clientAuthorityOwner != null)
                    {
                        Rect rect5 = new Rect(x, num3 + 10f, 400f, 20f);
                        GUI.Label(rect5, new GUIContent("Client Authority: " + this.m_Identity.clientAuthorityOwner), this.m_Styles.labelStyle);
                    }
                }
            }
        }

        private class NetworkBehaviourInfo
        {
            public NetworkBehaviour behaviour;
            public GUIContent name;
        }

        private class NetworkIdentityInfo
        {
            public GUIContent name;
            public GUIContent value;
        }

        private class Styles
        {
            public GUIStyle componentName = new GUIStyle(EditorStyles.boldLabel);
            public GUIStyle disabledName = new GUIStyle(EditorStyles.miniLabel);
            public GUIStyle labelStyle = new GUIStyle(EditorStyles.label);

            public Styles()
            {
                Color color = new Color(0.7f, 0.7f, 0.7f);
                RectOffset padding = this.labelStyle.padding;
                padding.right += 20;
                this.labelStyle.normal.textColor = color;
                this.labelStyle.active.textColor = color;
                this.labelStyle.focused.textColor = color;
                this.labelStyle.hover.textColor = color;
                this.labelStyle.onNormal.textColor = color;
                this.labelStyle.onActive.textColor = color;
                this.labelStyle.onFocused.textColor = color;
                this.labelStyle.onHover.textColor = color;
                this.componentName.normal.textColor = color;
                this.componentName.active.textColor = color;
                this.componentName.focused.textColor = color;
                this.componentName.hover.textColor = color;
                this.componentName.onNormal.textColor = color;
                this.componentName.onActive.textColor = color;
                this.componentName.onFocused.textColor = color;
                this.componentName.onHover.textColor = color;
                this.disabledName.normal.textColor = color;
                this.disabledName.active.textColor = color;
                this.disabledName.focused.textColor = color;
                this.disabledName.hover.textColor = color;
                this.disabledName.onNormal.textColor = color;
                this.disabledName.onActive.textColor = color;
                this.disabledName.onFocused.textColor = color;
                this.disabledName.onHover.textColor = color;
            }
        }
    }
}

