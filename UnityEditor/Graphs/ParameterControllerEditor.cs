namespace UnityEditor.Graphs
{
    using System;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEngine;

    [InitializeOnLoad, EditorWindowTitle(title="Parameters")]
    internal class ParameterControllerEditor : EditorWindow, IAnimatorControllerEditor
    {
        private const float kToolbarHeight = 17f;
        [SerializeField]
        private AnimatorController m_AnimatorController;
        private ParameterControllerView m_Editor = null;
        [SerializeField]
        private Animator m_PreviewAnimator;
        public static ParameterControllerEditor tool;

        private void DetectAnimatorControllerFromSelection()
        {
            AnimatorController activeObject = null;
            if ((Selection.activeObject == null) && (this.animatorController == null))
            {
                this.animatorController = null;
            }
            if ((Selection.activeObject is AnimatorController) && EditorUtility.IsPersistent(Selection.activeObject))
            {
                activeObject = Selection.activeObject as AnimatorController;
            }
            if (Selection.activeGameObject != null)
            {
                Animator component = Selection.activeGameObject.GetComponent<Animator>();
                if (component != null)
                {
                    AnimatorController effectiveAnimatorController = AnimatorController.GetEffectiveAnimatorController(component);
                    if (effectiveAnimatorController != null)
                    {
                        activeObject = effectiveAnimatorController;
                    }
                }
            }
            if ((activeObject != null) && (activeObject != this.animatorController))
            {
                this.animatorController = activeObject;
                this.editor.ResetUI();
            }
        }

        private void DetectAnimatorControllerFromTool()
        {
            if ((this.animatorController == null) && (AnimatorControllerTool.tool != null))
            {
                this.animatorController = AnimatorControllerTool.tool.animatorController;
            }
        }

        private void DetectPreviewObjectFromSelection()
        {
            if (Selection.activeGameObject != null)
            {
                Animator component = Selection.activeGameObject.GetComponent<Animator>();
                if ((component != null) && !AssetDatabase.Contains(Selection.activeGameObject))
                {
                    this.m_PreviewAnimator = component;
                    this.editor.ResetUI();
                }
            }
        }

        [UnityEditor.MenuItem("Window/Animator Parameter", false, 0x7dc)]
        public static void DoWindow()
        {
            EditorWindow.GetWindow<ParameterControllerEditor>();
        }

        protected void Init()
        {
            base.titleContent = base.GetLocalizedTitleContent();
            base.wantsMouseMove = true;
            this.editor.Init(this);
            tool = this;
        }

        private void OnControllerChange()
        {
            if ((this.m_PreviewAnimator != null) && (AnimatorController.GetEffectiveAnimatorController(this.m_PreviewAnimator) != this.animatorController))
            {
                this.animatorController = AnimatorController.GetEffectiveAnimatorController(this.m_PreviewAnimator);
                this.editor.ResetUI();
            }
        }

        private void OnDisable()
        {
            this.editor.OnDisable();
        }

        private void OnEnable()
        {
            this.Init();
            this.DetectAnimatorControllerFromSelection();
            this.DetectAnimatorControllerFromTool();
            this.editor.OnEnable();
        }

        public void OnFocus()
        {
            this.DetectAnimatorControllerFromSelection();
            this.DetectAnimatorControllerFromTool();
            this.DetectPreviewObjectFromSelection();
            this.editor.OnFocus();
        }

        public void OnGUI()
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(17f) };
            GUILayout.BeginHorizontal(EditorStyles.toolbar, options);
            GUILayout.Space(10f);
            GUILayout.FlexibleSpace();
            this.editor.OnToolbarGUI();
            GUILayout.EndHorizontal();
            Rect rect = new Rect(0f, 0f, base.position.width, base.position.height);
            this.editor.OnEvent();
            this.editor.OnGUI(rect);
        }

        public void OnInspectorUpdate()
        {
            if (this.liveLink)
            {
                base.Repaint();
            }
        }

        public void OnInvalidateAnimatorController()
        {
            this.editor.ResetUI();
            base.Repaint();
        }

        private void OnLostFocus()
        {
            this.editor.OnLostFocus();
        }

        public void OnProjectChange()
        {
            this.DetectAnimatorControllerFromSelection();
            this.editor.ResetUI();
            base.Repaint();
        }

        public void OnSelectionChange()
        {
            this.DetectAnimatorControllerFromSelection();
            this.DetectPreviewObjectFromSelection();
            this.editor.ResetUI();
            base.Repaint();
        }

        public void ResetUI()
        {
        }

        void IAnimatorControllerEditor.Repaint()
        {
            base.Repaint();
        }

        public AnimatorController animatorController
        {
            get => 
                this.m_AnimatorController;
            set
            {
                this.m_AnimatorController = value;
                if ((this.m_PreviewAnimator != null) && (AnimatorController.GetEffectiveAnimatorController(this.m_PreviewAnimator) != this.m_AnimatorController))
                {
                    this.m_PreviewAnimator = null;
                }
            }
        }

        protected IAnimatorControllerSubEditor editor
        {
            get
            {
                if (this.m_Editor == null)
                {
                    this.m_Editor = new ParameterControllerView();
                }
                return this.m_Editor;
            }
        }

        public bool liveLink =>
            (((EditorApplication.isPlaying && (this.previewAnimator != null)) && this.previewAnimator.enabled) && this.previewAnimator.gameObject.activeInHierarchy);

        public Animator previewAnimator =>
            this.m_PreviewAnimator;
    }
}

