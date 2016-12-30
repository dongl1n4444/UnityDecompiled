namespace UnityEditor.Collaboration
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class Overlay
    {
        private static readonly Dictionary<Collab.CollabStates, Texture2D> s_Overlays = new Dictionary<Collab.CollabStates, Texture2D>();

        protected static bool AreOverlaysLoaded()
        {
            if (s_Overlays.Count == 0)
            {
                return false;
            }
            foreach (Texture2D textured in s_Overlays.Values)
            {
                if (textured == null)
                {
                    return false;
                }
            }
            return true;
        }

        protected static void DrawOverlayElement(Collab.CollabStates singleState, Rect itemRect)
        {
            Texture2D textured;
            if (s_Overlays.TryGetValue(singleState, out textured) && (textured != null))
            {
                GUI.DrawTexture(itemRect, textured);
            }
        }

        public static void DrawOverlays(Collab.CollabStates assetState, Rect itemRect)
        {
            if (((assetState != Collab.CollabStates.kCollabInvalidState) && (assetState != Collab.CollabStates.kCollabNone)) && (Event.current.type == EventType.Repaint))
            {
                if (!AreOverlaysLoaded())
                {
                    LoadOverlays();
                }
                DrawOverlayElement(GetOverlayStateForAsset(assetState), itemRect);
            }
        }

        protected static Collab.CollabStates GetOverlayStateForAsset(Collab.CollabStates assetStates)
        {
            foreach (Collab.CollabStates states in s_Overlays.Keys)
            {
                if (HasState(assetStates, states))
                {
                    return states;
                }
            }
            return Collab.CollabStates.kCollabNone;
        }

        protected static bool HasState(Collab.CollabStates assetStates, Collab.CollabStates includesState) => 
            ((assetStates & includesState) == includesState);

        protected static void LoadOverlays()
        {
            s_Overlays.Clear();
            s_Overlays.Add(Collab.CollabStates.kCollabIgnored, TextureUtility.LoadTextureFromApplicationContents("ignored.png"));
            s_Overlays.Add(Collab.CollabStates.kCollabConflicted, TextureUtility.LoadTextureFromApplicationContents("conflict.png"));
            s_Overlays.Add(Collab.CollabStates.kCollabNone | Collab.CollabStates.kCollabPendingMerge, TextureUtility.LoadTextureFromApplicationContents("conflict.png"));
            s_Overlays.Add(Collab.CollabStates.kCollabChanges, TextureUtility.LoadTextureFromApplicationContents("changes.png"));
            s_Overlays.Add(Collab.CollabStates.kCollabCheckedOutLocal | Collab.CollabStates.kCollabMovedLocal, TextureUtility.LoadTextureFromApplicationContents("modif-local.png"));
            s_Overlays.Add(Collab.CollabStates.kCollabAddedLocal, TextureUtility.LoadTextureFromApplicationContents("added-local.png"));
            s_Overlays.Add(Collab.CollabStates.kCollabCheckedOutLocal, TextureUtility.LoadTextureFromApplicationContents("modif-local.png"));
            s_Overlays.Add(Collab.CollabStates.kCollabDeletedLocal, TextureUtility.LoadTextureFromApplicationContents("deleted-local.png"));
            s_Overlays.Add(Collab.CollabStates.kCollabMovedLocal, TextureUtility.LoadTextureFromApplicationContents("modif-local.png"));
        }
    }
}

