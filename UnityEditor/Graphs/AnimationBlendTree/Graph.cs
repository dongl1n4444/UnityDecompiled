namespace UnityEditor.Graphs.AnimationBlendTree
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEditor.Graphs;
    using UnityEngine;

    internal class Graph : UnityEditor.Graphs.Graph
    {
        private const float kLeafHorizontalOffset = 20f;
        private const float kNodeHeight = 50f;
        private const float kNodeHorizontalPadding = 70f;
        private const float kNodeVerticalPadding = 5f;
        internal const float kNodeWidth = 200f;
        private const float kParameterVerticalOffset = 20f;
        private const float kSectionVerticalPadding = 15f;
        private Dictionary<string, float> m_ParameterValues = new Dictionary<string, float>();
        [SerializeField]
        private BlendTree m_RootBlendTree;
        private UnityEditor.Graphs.AnimationBlendTree.Node m_RootNode;
        private float m_VerticalLeafOffset;
        [SerializeField]
        public Animator previewAvatar;

        private void ArrangeNodeRecursive(UnityEditor.Graphs.AnimationBlendTree.Node node, int depth)
        {
            if (node.isLeaf && (node.parent != null))
            {
                Rect position = node.position;
                position.y = this.m_VerticalLeafOffset;
                position.x = (depth * 270f) + 70f;
                position.x += Mathf.PingPong((node.childIndex + 0.5f) / ((float) node.parent.children.Count), 0.5f) * 20f;
                node.position = position;
                this.m_VerticalLeafOffset += position.height + 5f;
            }
            else if (node.children.Count != 0)
            {
                float verticalLeafOffset = this.m_VerticalLeafOffset;
                float positiveInfinity = float.PositiveInfinity;
                float negativeInfinity = float.NegativeInfinity;
                foreach (UnityEditor.Graphs.AnimationBlendTree.Node node2 in node.children)
                {
                    this.ArrangeNodeRecursive(node2, depth + 1);
                    positiveInfinity = Mathf.Min(positiveInfinity, node2.position.y);
                    negativeInfinity = Mathf.Max(negativeInfinity, node2.position.y);
                }
                Rect rect2 = node.position;
                rect2.y = (positiveInfinity + negativeInfinity) * 0.5f;
                rect2.x = (depth * 270f) + 70f;
                node.position = rect2;
                this.m_VerticalLeafOffset = Mathf.Max((float) (15f + this.m_VerticalLeafOffset), (float) ((verticalLeafOffset + rect2.height) + 15f));
            }
        }

        public void AutoArrangeNodePositions()
        {
            this.m_VerticalLeafOffset = 5f;
            this.ArrangeNodeRecursive(this.m_RootNode, 0);
        }

        public void BuildFromBlendTree(BlendTree blendTree)
        {
            this.Clear(true);
            if (blendTree != null)
            {
                this.CreateNodeFromBlendTreeRecursive(blendTree, null);
                this.PopulateParameterValues();
                this.AutoArrangeNodePositions();
            }
        }

        public override void Clear(bool destroyNodes)
        {
            base.Clear(destroyNodes);
            this.m_ParameterValues.Clear();
            this.m_RootBlendTree = null;
            this.m_RootNode = null;
        }

        private void CreateEmptySlot(UnityEditor.Graphs.AnimationBlendTree.Node parentNode)
        {
            parentNode.AddOutputSlot("");
        }

        private UnityEditor.Graphs.AnimationBlendTree.Node CreateNode(Motion motion, string name)
        {
            UnityEditor.Graphs.AnimationBlendTree.Node node = UnityEditor.Graphs.Node.Instance<UnityEditor.Graphs.AnimationBlendTree.Node>();
            node.hideFlags = HideFlags.HideAndDontSave;
            node.name = name;
            node.motion = motion;
            BlendTree tree = motion as BlendTree;
            float num = 0f;
            if (tree != null)
            {
                num = tree.recursiveBlendParameterCount * 20f;
            }
            node.position = new Rect(0f, 0f, 200f, 50f + num);
            this.AddNode(node);
            return node;
        }

        private void CreateNodeFromAnimationClip(AnimationClip clip, UnityEditor.Graphs.AnimationBlendTree.Node parentNode)
        {
            UnityEditor.Graphs.AnimationBlendTree.Node node = this.CreateNode(clip, clip.name);
            node.parent = parentNode;
            Slot fromSlot = parentNode.AddOutputSlot(clip.name);
            Slot toSlot = node.AddInputSlot((parentNode.motion as BlendTree).name);
            this.Connect(fromSlot, toSlot);
        }

        private void CreateNodeFromBlendTreeRecursive(BlendTree blendTree, UnityEditor.Graphs.AnimationBlendTree.Node parentNode)
        {
            UnityEditor.Graphs.AnimationBlendTree.Node node = this.CreateNode(blendTree, blendTree.name);
            if (parentNode != null)
            {
                node.parent = parentNode;
                Slot fromSlot = parentNode.AddOutputSlot(blendTree.name);
                Slot toSlot = node.AddInputSlot((parentNode.motion as BlendTree).name);
                this.Connect(fromSlot, toSlot);
            }
            else
            {
                this.m_RootBlendTree = blendTree;
                this.m_RootNode = node;
            }
            int childCount = blendTree.GetChildCount();
            for (int i = 0; i < childCount; i++)
            {
                Motion childMotion = blendTree.GetChildMotion(i);
                if (childMotion == null)
                {
                    this.CreateEmptySlot(node);
                }
                else if (childMotion is BlendTree)
                {
                    this.CreateNodeFromBlendTreeRecursive(childMotion as BlendTree, node);
                }
                else
                {
                    if (!(childMotion is AnimationClip))
                    {
                        throw new NotImplementedException("Unknown Motion type:" + childMotion.GetType());
                    }
                    this.CreateNodeFromAnimationClip(childMotion as AnimationClip, node);
                }
            }
        }

        private static int FindMotionIndexOnBlendTree(BlendTree blendTree, Motion motion)
        {
            int childCount = blendTree.GetChildCount();
            for (int i = 0; i < childCount; i++)
            {
                if (blendTree.GetChildMotion(i) == motion)
                {
                    return i;
                }
            }
            return -1;
        }

        public UnityEditor.Graphs.AnimationBlendTree.Node FindNode(Motion motion)
        {
            <FindNode>c__AnonStorey0 storey = new <FindNode>c__AnonStorey0 {
                motion = motion
            };
            return Enumerable.FirstOrDefault<UnityEditor.Graphs.AnimationBlendTree.Node>(Enumerable.Cast<UnityEditor.Graphs.AnimationBlendTree.Node>(base.nodes), new Func<UnityEditor.Graphs.AnimationBlendTree.Node, bool>(storey, (IntPtr) this.<>m__0));
        }

        private T GetComponentFromSelection<T>() where T: Component
        {
            GameObject activeGameObject = Selection.activeGameObject;
            if (activeGameObject != null)
            {
                return activeGameObject.GetComponent<T>();
            }
            return null;
        }

        internal override UnityEditor.Graphs.GraphGUI GetEditor()
        {
            UnityEditor.Graphs.AnimationBlendTree.GraphGUI hgui = ScriptableObject.CreateInstance<UnityEditor.Graphs.AnimationBlendTree.GraphGUI>();
            hgui.graph = this;
            hgui.hideFlags = HideFlags.HideAndDontSave;
            return hgui;
        }

        private T GetObjectFromSelection<T>() where T: UnityEngine.Object
        {
            return (Selection.activeObject as T);
        }

        public float GetParameterValue(string parameterName)
        {
            if (this.m_ParameterValues.ContainsKey(parameterName))
            {
                return this.m_ParameterValues[parameterName];
            }
            Debug.LogError("parameter name does not exist.");
            return 0f;
        }

        public void PopulateParameterValues()
        {
            if (this.m_RootBlendTree != null)
            {
                for (int i = 0; i < this.m_RootBlendTree.recursiveBlendParameterCount; i++)
                {
                    string recursiveBlendParameter = this.m_RootBlendTree.GetRecursiveBlendParameter(i);
                    if (this.liveLink)
                    {
                        this.SetParameterValue(recursiveBlendParameter, this.previewAvatar.GetFloat(recursiveBlendParameter));
                    }
                    else
                    {
                        this.SetParameterValue(recursiveBlendParameter, this.m_RootBlendTree.GetInputBlendValue(recursiveBlendParameter));
                    }
                }
            }
        }

        public void RemoveNodeMotions(IEnumerable<UnityEditor.Graphs.Node> nodes)
        {
            foreach (UnityEditor.Graphs.Node node in nodes)
            {
                UnityEditor.Graphs.AnimationBlendTree.Node node2 = node as UnityEditor.Graphs.AnimationBlendTree.Node;
                if ((this.m_RootBlendTree != node2.motion) && (node2.motion != null))
                {
                    if (node2.parent != null)
                    {
                        BlendTree blendTree = node2.parent.motion as BlendTree;
                        int index = FindMotionIndexOnBlendTree(blendTree, node2.motion);
                        blendTree.RemoveChild(index);
                    }
                    BlendTree motion = node2.motion as BlendTree;
                    if ((motion != null) && MecanimUtilities.AreSameAsset(this.m_RootBlendTree, motion))
                    {
                        MecanimUtilities.DestroyBlendTreeRecursive(motion);
                    }
                }
            }
        }

        public void SetParameterValue(string parameterName, float parameterValue)
        {
            this.m_ParameterValues[parameterName] = parameterValue;
            this.SetParameterValueRecursive(this.m_RootBlendTree, parameterName, parameterValue);
            if (this.liveLink)
            {
                this.previewAvatar.SetFloat(parameterName, parameterValue);
            }
        }

        private void SetParameterValueRecursive(BlendTree blendTree, string parameterName, float parameterValue)
        {
            blendTree.SetInputBlendValue(parameterName, parameterValue);
            int childCount = blendTree.GetChildCount();
            for (int i = 0; i < childCount; i++)
            {
                BlendTree childMotion = blendTree.GetChildMotion(i) as BlendTree;
                if (childMotion != null)
                {
                    this.SetParameterValueRecursive(childMotion, parameterName, parameterValue);
                }
            }
        }

        public override void WakeUp(bool force)
        {
            base.WakeUp(force);
            this.PopulateParameterValues();
        }

        public bool liveLink
        {
            get
            {
                return (((EditorApplication.isPlaying && (this.previewAvatar != null)) && this.previewAvatar.enabled) && this.previewAvatar.gameObject.activeInHierarchy);
            }
        }

        public BlendTree rootBlendTree
        {
            get
            {
                return this.m_RootBlendTree;
            }
            set
            {
                if (this.m_RootBlendTree != value)
                {
                    this.m_RootBlendTree = value;
                    this.BuildFromBlendTree(this.m_RootBlendTree);
                }
            }
        }

        public UnityEditor.Graphs.AnimationBlendTree.Node rootNode
        {
            get
            {
                return this.m_RootNode;
            }
            private set
            {
                this.m_RootNode = value;
            }
        }

        [CompilerGenerated]
        private sealed class <FindNode>c__AnonStorey0
        {
            internal Motion motion;

            internal bool <>m__0(UnityEditor.Graphs.AnimationBlendTree.Node node)
            {
                return (node.motion == this.motion);
            }
        }
    }
}

