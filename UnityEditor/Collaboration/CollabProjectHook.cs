namespace UnityEditor.Collaboration
{
    using System;
    using UnityEditor.Connect;
    using UnityEditor.Web;
    using UnityEngine;

    internal class CollabProjectHook
    {
        public static void OnProjectWindowItemIconOverlay(string guid, Rect drawRect)
        {
            if (CollabAccess.Instance.IsServiceEnabled() && UnityConnect.instance.userInfo.whitelisted)
            {
                Collab instance = Collab.instance;
                if (instance.collabInfo.whitelisted)
                {
                    Overlay.DrawOverlays(instance.GetAssetState(guid), drawRect);
                }
            }
        }
    }
}

