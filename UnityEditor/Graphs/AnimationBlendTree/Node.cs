namespace UnityEditor.Graphs.AnimationBlendTree
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEditor.Graphs;
    using UnityEngine;

    public class Node : UnityEditor.Graphs.Node
    {
        public List<UnityEditor.Graphs.AnimationBlendTree.Node> children = new List<UnityEditor.Graphs.AnimationBlendTree.Node>();
        [NonSerialized]
        private Animator m_Animator = null;
        [NonSerialized]
        private AnimatorController m_Controller = null;
        private bool m_ControllerIsDirty = false;
        private UnityEditor.Graphs.AnimationBlendTree.Node m_Parent;
        private GameObject m_PreviewInstance = null;
        [NonSerialized]
        private AnimatorState m_State = null;
        [NonSerialized]
        private AnimatorStateMachine m_StateMachine = null;
        public Motion motion;
        public float weight = 1f;

        private void ClearStateMachine()
        {
            if (this.m_Animator != null)
            {
                AnimatorController.SetAnimatorController(this.m_Animator, null);
            }
            UnityEngine.Object.DestroyImmediate(this.m_Controller);
            UnityEngine.Object.DestroyImmediate(this.m_State);
            this.m_StateMachine = null;
            this.m_Controller = null;
            this.m_State = null;
        }

        protected virtual void ControllerDirty()
        {
            this.m_ControllerIsDirty = true;
        }

        public void CreateParameters()
        {
            for (int i = 0; i < this.blendTree.recursiveBlendParameterCount; i++)
            {
                this.m_Controller.AddParameter(this.blendTree.GetRecursiveBlendParameter(i), AnimatorControllerParameterType.Float);
            }
        }

        private void CreateStateMachine()
        {
            if ((this.m_Animator != null) && (this.m_Controller == null))
            {
                this.m_Controller = new AnimatorController();
                this.m_Controller.pushUndo = false;
                this.m_Controller.AddLayer("node");
                this.m_StateMachine = this.m_Controller.layers[0].stateMachine;
                this.m_StateMachine.pushUndo = false;
                this.CreateParameters();
                this.m_State = this.m_StateMachine.AddState("node", new Vector3());
                this.m_State.pushUndo = false;
                this.m_State.motion = this.motion;
                this.m_State.hideFlags = HideFlags.DontSave;
                this.m_Controller.hideFlags = HideFlags.DontSave;
                this.m_StateMachine.hideFlags = HideFlags.DontSave;
                AnimatorController.SetAnimatorController(this.m_Animator, this.m_Controller);
                this.m_Animator.Update(0f);
                this.m_Controller.OnAnimatorControllerDirty = (Action) Delegate.Combine(this.m_Controller.OnAnimatorControllerDirty, new Action(this.ControllerDirty));
                this.m_ControllerIsDirty = false;
            }
        }

        private void OnDestroy()
        {
            if (this.m_Controller != null)
            {
                this.m_Controller.OnAnimatorControllerDirty = (Action) Delegate.Remove(this.m_Controller.OnAnimatorControllerDirty, new Action(this.ControllerDirty));
            }
            UnityEngine.Object.DestroyImmediate(this.m_PreviewInstance);
        }

        public void UpdateAnimator()
        {
            if (this.animator != null)
            {
                if (this.m_ControllerIsDirty)
                {
                    this.ClearStateMachine();
                    this.CreateStateMachine();
                }
                int recursiveBlendParameterCount = this.blendTree.recursiveBlendParameterCount;
                if (this.m_Controller.parameters.Length >= recursiveBlendParameterCount)
                {
                    for (int i = 0; i < recursiveBlendParameterCount; i++)
                    {
                        string recursiveBlendParameter = this.blendTree.GetRecursiveBlendParameter(i);
                        float inputBlendValue = this.blendTree.GetInputBlendValue(recursiveBlendParameter);
                        this.animator.SetFloat(recursiveBlendParameter, inputBlendValue);
                    }
                    this.animator.EvaluateController();
                }
            }
        }

        public Animator animator
        {
            get
            {
                if ((this.m_Animator == null) && (this.blendTree != null))
                {
                    GameObject original = (GameObject) EditorGUIUtility.Load("Avatar/DefaultAvatar.fbx");
                    if (this.m_PreviewInstance == null)
                    {
                        this.m_PreviewInstance = EditorUtility.InstantiateForAnimatorPreview(original);
                    }
                    this.m_Animator = this.m_PreviewInstance.GetComponent<Animator>();
                    foreach (Renderer renderer in this.m_PreviewInstance.GetComponentsInChildren<Renderer>())
                    {
                        renderer.enabled = false;
                    }
                    this.CreateStateMachine();
                }
                return this.m_Animator;
            }
        }

        public BlendTree blendTree =>
            (this.motion as BlendTree);

        public int childIndex
        {
            get
            {
                if (this.m_Parent == null)
                {
                    return -1;
                }
                return this.m_Parent.children.IndexOf(this);
            }
        }

        public bool controllerDirty =>
            this.m_ControllerIsDirty;

        public bool isLeaf =>
            (this.children.Count == 0);

        public UnityEditor.Graphs.AnimationBlendTree.Node parent
        {
            get => 
                this.m_Parent;
            set
            {
                if (this.m_Parent != null)
                {
                    this.m_Parent.children.Remove(this);
                }
                this.m_Parent = value;
                this.m_Parent.children.Add(this);
            }
        }

        public UnityEngine.Color weightColor =>
            UnityEngine.Color.Lerp(new UnityEngine.Color(0.8f, 0.8f, 0.8f, 1f), UnityEngine.Color.white, Mathf.Pow(this.weight, 0.5f));

        public UnityEngine.Color weightEdgeColor =>
            UnityEngine.Color.Lerp(UnityEngine.Color.white, new UnityEngine.Color(0.42f, 0.7f, 1f, 1f), Mathf.Pow(this.weight, 0.5f));
    }
}

