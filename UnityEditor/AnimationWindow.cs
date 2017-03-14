namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Profiling;

    [EditorWindowTitle(title="Animation", useTypeNameAsIconName=true)]
    internal class AnimationWindow : EditorWindow
    {
        [SerializeField]
        private AnimEditor m_AnimEditor;
        private GUIContent m_DefaultTitleContent;
        private GUIStyle m_LockButtonStyle;
        [SerializeField]
        private bool m_Locked = false;
        private GUIContent m_RecordTitleContent;
        private static List<AnimationWindow> s_AnimationWindows = new List<AnimationWindow>();

        public bool EditAnimationClip(AnimationClip animationClip)
        {
            if (this.state.linkedWithSequencer)
            {
                return false;
            }
            return this.EditAnimationClipInternal(animationClip, null, null);
        }

        private bool EditAnimationClipInternal(AnimationClip animationClip, UnityEngine.Object sourceObject, IAnimationWindowControl controlInterface)
        {
            AnimationClipSelectionItem selectedItem = AnimationClipSelectionItem.Create(animationClip, sourceObject);
            if (this.ShouldUpdateSelection(selectedItem))
            {
                this.m_AnimEditor.selectedItem = selectedItem;
                this.m_AnimEditor.overrideControlInterface = controlInterface;
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(selectedItem);
                return false;
            }
            return true;
        }

        public bool EditGameObject(GameObject gameObject)
        {
            if (this.state.linkedWithSequencer)
            {
                return false;
            }
            return this.EditGameObjectInternal(gameObject, null);
        }

        private bool EditGameObjectInternal(GameObject gameObject, IAnimationWindowControl controlInterface)
        {
            if (!EditorUtility.IsPersistent(gameObject))
            {
                if ((gameObject.hideFlags & HideFlags.NotEditable) != HideFlags.None)
                {
                    return false;
                }
                GameObjectSelectionItem selectedItem = GameObjectSelectionItem.Create(gameObject);
                if (this.ShouldUpdateGameObjectSelection(selectedItem))
                {
                    this.m_AnimEditor.selectedItem = selectedItem;
                    this.m_AnimEditor.overrideControlInterface = controlInterface;
                    return true;
                }
                UnityEngine.Object.DestroyImmediate(selectedItem);
            }
            return false;
        }

        public bool EditSequencerClip(AnimationClip animationClip, UnityEngine.Object sourceObject, IAnimationWindowControl controlInterface)
        {
            if (this.EditAnimationClipInternal(animationClip, sourceObject, controlInterface))
            {
                this.state.linkedWithSequencer = true;
                return true;
            }
            return false;
        }

        public void ForceRefresh()
        {
            if (this.m_AnimEditor != null)
            {
                this.m_AnimEditor.state.ForceRefresh();
            }
        }

        public static List<AnimationWindow> GetAllAnimationWindows() => 
            s_AnimationWindows;

        public void OnControllerChange()
        {
            this.OnSelectionChange();
        }

        public void OnDestroy()
        {
            UnityEngine.Object.DestroyImmediate(this.m_AnimEditor);
        }

        public void OnDisable()
        {
            s_AnimationWindows.Remove(this);
            this.m_AnimEditor.OnDisable();
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
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
            this.m_DefaultTitleContent = base.titleContent;
            this.m_RecordTitleContent = EditorGUIUtility.TextContentWithIcon(base.titleContent.text, "Animation.Record");
            this.OnSelectionChange();
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
        }

        public void OnFocus()
        {
            this.OnSelectionChange();
        }

        public void OnGUI()
        {
            Profiler.BeginSample("AnimationWindow.OnGUI");
            base.titleContent = !this.m_AnimEditor.state.recording ? this.m_DefaultTitleContent : this.m_RecordTitleContent;
            this.m_AnimEditor.OnAnimEditorGUI(this, base.position);
            Profiler.EndSample();
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
                        this.EditAnimationClip(activeObject);
                    }
                }
            }
        }

        private bool ShouldUpdateGameObjectSelection(GameObjectSelectionItem selectedItem)
        {
            <ShouldUpdateGameObjectSelection>c__AnonStorey0 storey = new <ShouldUpdateGameObjectSelection>c__AnonStorey0();
            if (this.m_Locked)
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

        private bool ShouldUpdateSelection(AnimationWindowSelectionItem selectedItem)
        {
            if (this.m_Locked)
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

        protected virtual void ShowButton(Rect r)
        {
            if (this.m_LockButtonStyle == null)
            {
                this.m_LockButtonStyle = "IN LockButton";
            }
            if (this.m_AnimEditor.stateDisabled)
            {
                this.m_Locked = false;
            }
            EditorGUI.BeginChangeCheck();
            using (new EditorGUI.DisabledScope(this.m_AnimEditor.stateDisabled))
            {
                this.m_Locked = GUI.Toggle(r, this.m_Locked, GUIContent.none, this.m_LockButtonStyle);
            }
            if (EditorGUI.EndChangeCheck())
            {
                this.OnSelectionChange();
            }
        }

        private void UndoRedoPerformed()
        {
            base.Repaint();
        }

        public void UnlinkSequencer()
        {
            if (this.state.linkedWithSequencer)
            {
                this.state.linkedWithSequencer = false;
                this.EditAnimationClip(null);
                this.OnSelectionChange();
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
        private sealed class <ShouldUpdateGameObjectSelection>c__AnonStorey0
        {
            internal AnimationWindowSelectionItem currentlySelectedItem;

            internal bool <>m__0(AnimationClip x) => 
                (x == this.currentlySelectedItem.animationClip);
        }
    }
}

