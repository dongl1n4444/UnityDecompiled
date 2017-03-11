namespace UnityEditor.Collaboration
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    internal class Overlay
    {
        private static double OverlaySizeOnLargeIcon = 0.35;
        private static double OverlaySizeOnSmallIcon = 0.6;
        private static readonly Dictionary<Collab.CollabStates, GUIContent> s_Overlays = new Dictionary<Collab.CollabStates, GUIContent>();

        protected static bool AreOverlaysLoaded()
        {
            if (s_Overlays.Count == 0)
            {
                return false;
            }
            foreach (GUIContent content in s_Overlays.Values)
            {
                if (content == null)
                {
                    return false;
                }
            }
            return true;
        }

        protected static void DrawOverlayElement(Collab.CollabStates singleState, Rect itemRect)
        {
            GUIContent content;
            if (s_Overlays.TryGetValue(singleState, out content))
            {
                Texture image = content.image;
                if (image != null)
                {
                    Rect position = itemRect;
                    double overlaySizeOnLargeIcon = OverlaySizeOnLargeIcon;
                    if (position.width <= 24f)
                    {
                        overlaySizeOnLargeIcon = OverlaySizeOnSmallIcon;
                    }
                    position.width = Convert.ToInt32(Math.Ceiling((double) (position.width * overlaySizeOnLargeIcon)));
                    position.height = Convert.ToInt32(Math.Ceiling((double) (position.height * overlaySizeOnLargeIcon)));
                    position.x += itemRect.width - position.width;
                    GUI.DrawTexture(position, image, ScaleMode.ScaleToFit);
                }
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
            s_Overlays.Add(Collab.CollabStates.kCollabIgnored, EditorGUIUtility.IconContent("CollabExclude Icon"));
            s_Overlays.Add(Collab.CollabStates.kCollabConflicted, EditorGUIUtility.IconContent("CollabConflict Icon"));
            s_Overlays.Add(Collab.CollabStates.kCollabNone | Collab.CollabStates.kCollabPendingMerge, EditorGUIUtility.IconContent("CollabConflict Icon"));
            s_Overlays.Add(Collab.CollabStates.kCollabMovedLocal, EditorGUIUtility.IconContent("CollabMoved Icon"));
            s_Overlays.Add(Collab.CollabStates.kCollabCheckedOutLocal | Collab.CollabStates.kCollabMovedLocal, EditorGUIUtility.IconContent("CollabMoved Icon"));
            s_Overlays.Add(Collab.CollabStates.kCollabCheckedOutLocal, EditorGUIUtility.IconContent("CollabEdit Icon"));
            s_Overlays.Add(Collab.CollabStates.kCollabAddedLocal, EditorGUIUtility.IconContent("CollabCreate Icon"));
            s_Overlays.Add(Collab.CollabStates.kCollabDeletedLocal, EditorGUIUtility.IconContent("CollabDeleted Icon"));
            s_Overlays.Add(Collab.CollabStates.KCollabContentConflicted, EditorGUIUtility.IconContent("CollabChangesConflict Icon"));
            s_Overlays.Add(Collab.CollabStates.KCollabContentChanged, EditorGUIUtility.IconContent("CollabChanges Icon"));
            s_Overlays.Add(Collab.CollabStates.KCollabContentDeleted, EditorGUIUtility.IconContent("CollabChangesDeleted Icon"));
        }
    }
}

