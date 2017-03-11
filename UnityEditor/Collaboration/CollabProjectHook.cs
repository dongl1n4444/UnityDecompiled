namespace UnityEditor.Collaboration
{
    using System;
    using UnityEditor.Web;
    using UnityEngine;

    internal class CollabProjectHook
    {
        public static void OnProjectWindowItemIconOverlay(string guid, Rect drawRect)
        {
            if (CollabAccess.Instance.IsServiceEnabled())
            {
                Overlay.DrawOverlays(Collab.instance.GetAssetState(guid), drawRect);
            }
        }
    }
}

