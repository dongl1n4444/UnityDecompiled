namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    [EditorWindowTitle(title="Animation", useTypeNameAsIconName=true)]
    internal class AnimationWindow : EditorWindow
    {
        [SerializeField]
        private AnimEditor m_AnimEditor;
        private GUIStyle m_LockButtonStyle;
        [SerializeField]
        private AnimationWindowPolicy m_Policy;
        private static List<AnimationWindow> s_AnimationWindows = new List<AnimationWindow>();

        public void EditAnimationClip(AnimationClip animationClip, Object sourceObject)
        {
            AnimationClipSelectionItem selectedItem = AnimationClipSelectionItem.Create(animationClip, sourceObject);
            if (this.ShouldUpdateSelection(selectedItem))
            {
                this.m_AnimEditor.state.recording = false;
                this.m_AnimEditor.selectedItem = selectedItem;
            }
            else
            {
                Object.DestroyImmediate(selectedItem);
            }
        }

        public void EditGameObject(GameObject gameObject)
        {
            if (!EditorUtility.IsPersistent(gameObject) && ((gameObject.hideFlags & HideFlags.NotEditable) == HideFlags.None))
            {
                GameObjectSelectionItem selectedItem = GameObjectSelectionItem.Create(gameObject);
                if (this.ShouldUpdateSelection(selectedItem))
                {
                    this.m_AnimEditor.state.recording = false;
                    this.m_AnimEditor.selectedItem = selectedItem;
                }
                else
                {
                    Object.DestroyImmediate(selectedItem);
                }
            }
        }

        public void ForceRefresh()
        {
            if (this.m_AnimEditor != null)
            {
                this.m_AnimEditor.state.ForceRefresh();
            }
        }

        public static List<AnimationWindow> GetAllAnimationWindows()
        {
            return s_AnimationWindows;
        }

        public void OnControllerChange()
        {
            this.OnSelectionChange();
        }

        public void OnDestroy()
        {
            Object.DestroyImmediate(this.m_AnimEditor);
        }

        public void OnDisable()
        {
            s_AnimationWindows.Remove(this);
            this.m_AnimEditor.OnDisable();
        }

        public void OnEnable()
        {
            if (this.m_AnimEditor == null)
            {
                this.m_AnimEditor = ScriptableObject.CreateInstance(typeof(AnimEditor)) as AnimEditor;
                this.m_AnimEditor.hideFlags = HideFlags.HideAndDontSave;
            }
            s_AnimationWindows.Add(this);
            base.titleContent = base.GetLocalizedTitleContent();
            this.OnSelectionChange();
        }

        public void OnFocus()
        {
            this.OnSelectionChange();
        }

        public void OnGUI()
        {
            this.SynchronizePolicy();
            this.m_AnimEditor.OnAnimEditorGUI(this, base.position);
        }

        public void OnLostFocus()
        {
            if (this.m_AnimEditor != null)
            {
                this.m_AnimEditor.OnLostFocus();
            }
        }

        public void OnSelectionChange()
        {
            if (this.m_AnimEditor != null)
            {
                GameObject activeGameObject = Selection.activeGameObject;
                if (activeGameObject != null)
                {
                    this.EditGameObject(activeGameObject);
                }
                else
                {
                    AnimationClip activeObject = Selection.activeObject as AnimationClip;
                    if (activeObject != null)
                    {
                        this.EditAnimationClip(activeObject, null);
                    }
                }
            }
        }

        private bool ShouldUpdateSelection(AnimationClipSelectionItem selectedItem)
        {
            if (this.m_AnimEditor.locked)
            {
                return false;
            }
            AnimationWindowSelectionItem item = this.m_AnimEditor.selectedItem;
            if (item != null)
            {
                return (selectedItem.GetRefreshHash() != item.GetRefreshHash());
            }
            return true;
        }

        private bool ShouldUpdateSelection(GameObjectSelectionItem selectedItem)
        {
            <ShouldUpdateSelection>c__AnonStorey0 storey = new <ShouldUpdateSelection>c__AnonStorey0();
            if (this.m_AnimEditor.locked)
            {
                return false;
            }
            if (selectedItem.rootGameObject != null)
            {
                storey.currentlySelectedItem = this.m_AnimEditor.selectedItem;
                if (storey.currentlySelectedItem != null)
                {
                    return ((selectedItem.rootGameObject != storey.currentlySelectedItem.rootGameObject) || ((storey.currentlySelectedItem.animationClip == null) || ((storey.currentlySelectedItem.rootGameObject != null) && !Array.Exists<AnimationClip>(AnimationUtility.GetAnimationClips(storey.currentlySelectedItem.rootGameObject), new Predicate<AnimationClip>(storey.<>m__0)))));
                }
            }
            return true;
        }

        protected virtual void ShowButton(Rect r)
        {
            if (this.m_LockButtonStyle == null)
            {
                this.m_LockButtonStyle = "IN LockButton";
            }
            EditorGUI.BeginChangeCheck();
            using (new EditorGUI.DisabledScope(this.m_AnimEditor.stateDisabled))
            {
                this.m_AnimEditor.locked = GUI.Toggle(r, this.m_AnimEditor.locked, GUIContent.none, this.m_LockButtonStyle);
            }
            if (EditorGUI.EndChangeCheck())
            {
                this.OnSelectionChange();
            }
        }

        private void SynchronizePolicy()
        {
            if (this.m_AnimEditor != null)
            {
                if (this.m_Policy == null)
                {
                    this.m_Policy = new AnimationWindowPolicy();
                }
                if (this.m_Policy.unitialized)
                {
                    this.m_Policy.SynchronizeFrameRate = delegate (ref float frameRate) {
                        AnimationWindowSelectionItem selectedItem = this.m_AnimEditor.selectedItem;
                        if ((selectedItem != null) && (selectedItem.animationClip != null))
                        {
                            frameRate = selectedItem.animationClip.frameRate;
                        }
                        else
                        {
                            frameRate = 60f;
                        }
                        return true;
                    };
                    this.m_Policy.unitialized = false;
                }
                this.m_AnimEditor.policy = this.m_Policy;
            }
        }

        public void Update()
        {
            this.m_AnimEditor.Update();
        }

        internal AnimationWindowState state
        {
            get
            {
                if (this.m_AnimEditor != null)
                {
                    return this.m_AnimEditor.state;
                }
                return null;
            }
        }

        [CompilerGenerated]
        private sealed class <ShouldUpdateSelection>c__AnonStorey0
        {
            internal AnimationWindowSelectionItem currentlySelectedItem;

            internal bool <>m__0(AnimationClip x)
            {
                return (x == this.currentlySelectedItem.animationClip);
            }
        }
    }
}

