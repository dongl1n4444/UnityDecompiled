namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditorInternal;
    using UnityEngine;

    [Serializable]
    internal class AnimationWindowClipPopup
    {
        [SerializeField]
        private int selectedIndex;
        [SerializeField]
        public AnimationWindowState state;

        private int ClipToIndex(AnimationClip clip)
        {
            if (this.state.activeRootGameObject != null)
            {
                int num = 0;
                foreach (AnimationClip clip2 in AnimationUtility.GetAnimationClips(this.state.activeRootGameObject))
                {
                    if (clip == clip2)
                    {
                        return num;
                    }
                    num++;
                }
            }
            return 0;
        }

        private string[] GetClipMenuContent()
        {
            List<string> list = new List<string>();
            list.AddRange(this.GetClipNames());
            AnimationWindowSelectionItem selectedItem = this.state.selectedItem;
            if ((selectedItem.rootGameObject != null) && selectedItem.animationIsEditable)
            {
                list.Add("");
                list.Add(AnimationWindowStyles.createNewClip.text);
            }
            return list.ToArray();
        }

        private string[] GetClipNames()
        {
            AnimationClip[] animationClips = new AnimationClip[0];
            if ((this.state.activeRootGameObject != null) && (this.state.activeAnimationClip != null))
            {
                animationClips = AnimationUtility.GetAnimationClips(this.state.activeRootGameObject);
            }
            string[] strArray = new string[animationClips.Length];
            for (int i = 0; i < animationClips.Length; i++)
            {
                strArray[i] = CurveUtility.GetClipName(animationClips[i]);
            }
            return strArray;
        }

        private AnimationClip IndexToClip(int index)
        {
            if (this.state.activeRootGameObject != null)
            {
                AnimationClip[] animationClips = AnimationUtility.GetAnimationClips(this.state.activeRootGameObject);
                if ((index >= 0) && (index < animationClips.Length))
                {
                    return AnimationUtility.GetAnimationClips(this.state.activeRootGameObject)[index];
                }
            }
            return null;
        }

        public void OnGUI()
        {
            AnimationWindowSelectionItem selectedItem = this.state.selectedItem;
            if ((selectedItem != null) && selectedItem.canChangeAnimationClip)
            {
                string[] clipMenuContent = this.GetClipMenuContent();
                EditorGUI.BeginChangeCheck();
                this.selectedIndex = EditorGUILayout.Popup(this.ClipToIndex(this.state.activeAnimationClip), clipMenuContent, EditorStyles.toolbarPopup, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    if (clipMenuContent[this.selectedIndex] == AnimationWindowStyles.createNewClip.text)
                    {
                        AnimationClip newClip = AnimationWindowUtility.CreateNewClip(selectedItem.rootGameObject.name);
                        if (newClip != null)
                        {
                            AnimationWindowUtility.AddClipToAnimationPlayerComponent(this.state.activeAnimationPlayer, newClip);
                            this.state.selection.UpdateClip(this.state.selectedItem, newClip);
                            this.state.currentTime = 0f;
                            this.state.ResampleAnimation();
                            GUIUtility.ExitGUI();
                        }
                    }
                    else
                    {
                        this.state.selection.UpdateClip(this.state.selectedItem, this.IndexToClip(this.selectedIndex));
                        this.state.currentTime = 0f;
                        this.state.ResampleAnimation();
                    }
                }
            }
        }
    }
}

