namespace UnityEditor.Graphs.AnimationStateMachine
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor.Animations;
    using UnityEditor.Graphs;
    using UnityEngine;

    internal class Graph : UnityEditor.Graphs.Graph
    {
        [NonSerialized]
        private AnimatorStateMachine m_ActiveStateMachine;
        [NonSerialized]
        private AnyStateNode m_AnyStateNode;
        private readonly Dictionary<string, EdgeInfo> m_ConnectedSlotsCache = new Dictionary<string, EdgeInfo>();
        private AnimatorState m_DefaultState;
        [NonSerialized]
        private EntryNode m_EntryNode;
        [NonSerialized]
        private ExitNode m_ExitNode;
        private int m_StateCount;
        private int m_StateMachineCount;
        private readonly Dictionary<AnimatorStateMachine, AnimatorStateMachine> m_StateMachineLookup = new Dictionary<AnimatorStateMachine, AnimatorStateMachine>();
        private readonly Dictionary<AnimatorStateMachine, StateMachineNode> m_StateMachineNodeLookup = new Dictionary<AnimatorStateMachine, StateMachineNode>();
        private readonly Dictionary<AnimatorState, AnimatorStateMachine> m_StateMachineProxyLookup = new Dictionary<AnimatorState, AnimatorStateMachine>();
        private readonly Dictionary<AnimatorState, StateNode> m_StateNodeLookup = new Dictionary<AnimatorState, StateNode>();
        private int m_TransitionCount;
        [NonSerialized]
        public AnimatorStateMachine parentStateMachine;
        [NonSerialized]
        public AnimatorStateMachine rootStateMachine;

        public void BuildGraphFromStateMachine(AnimatorStateMachine stateMachine)
        {
            Assert.IsNotNull(stateMachine);
            Assert.IsNotNull(this.rootStateMachine);
            this.Clear(false);
            this.m_ActiveStateMachine = stateMachine;
            this.CreateStateMachineLookup();
            this.CreateNodes();
            this.CreateEdges();
            this.m_StateCount = this.m_ActiveStateMachine.states.Length;
            this.m_StateMachineCount = this.m_ActiveStateMachine.stateMachines.Length;
            this.m_TransitionCount = this.rootStateMachine.transitionCount;
            this.m_DefaultState = this.rootStateMachine.defaultState;
        }

        private T CreateAndAddNode<T>(string name, Vector3 position) where T: UnityEditor.Graphs.AnimationStateMachine.Node
        {
            T node = ScriptableObject.CreateInstance<T>();
            node.hideFlags = HideFlags.HideAndDontSave;
            node.name = name;
            node.position = new Rect(position.x, position.y, 0f, 0f);
            node.AddInputSlot("In");
            node.AddOutputSlot("Out");
            this.AddNode(node);
            return node;
        }

        private void CreateAnyStateEdges(AnimatorStateTransition transition)
        {
            bool flag = transition == null;
            bool flag2 = (transition.destinationState != null) && (transition.destinationState.FindParent(this.rootStateMachine) != this.activeStateMachine);
            bool flag3 = (transition.destinationStateMachine != null) && (this.rootStateMachine.FindParent(transition.destinationStateMachine) != this.activeStateMachine);
            bool flag4 = (transition.destinationStateMachine != null) && (transition.destinationStateMachine != this.activeStateMachine);
            if ((!flag && !flag2) && (!flag3 || !flag4))
            {
                UnityEditor.Graphs.AnimationStateMachine.Node dstNode = (transition.destinationState == null) ? this.FindNodeForEdges(transition.destinationStateMachine) : this.FindNodeForEdges(transition.destinationState);
                this.CreateEdges(this.m_AnyStateNode, dstNode, new TransitionEditionContext(transition, null, null, this.rootStateMachine));
            }
        }

        private void CreateAnyStateNode()
        {
            this.m_AnyStateNode = this.CreateAndAddNode<AnyStateNode>("Any State", this.activeStateMachine.anyStatePosition);
            this.m_AnyStateNode.color = UnityEditor.Graphs.Styles.Color.Aqua;
        }

        private void CreateDefaultStateEdge(AnimatorState defaultState, AnimatorStateMachine owner)
        {
            if (defaultState != null)
            {
                UnityEditor.Graphs.AnimationStateMachine.Node dstNode = this.FindNodeForEdges(defaultState);
                if (dstNode != null)
                {
                    this.CreateEdges(this.m_EntryNode, dstNode, new TransitionEditionContext(null, null, null, owner));
                }
            }
        }

        private void CreateEdges()
        {
            this.m_ConnectedSlotsCache.Clear();
            List<ChildAnimatorState> statesRecursive = this.rootStateMachine.statesRecursive;
            foreach (ChildAnimatorState state in statesRecursive)
            {
                AnimatorState sourceState = state.state;
                AnimatorStateTransition[] transitions = sourceState.transitions;
                foreach (AnimatorStateTransition transition in transitions)
                {
                    if (transition != null)
                    {
                        this.CreateStateEdges(sourceState, transition);
                    }
                }
            }
            AnimatorStateTransition[] anyStateTransitions = this.rootStateMachine.anyStateTransitions;
            foreach (AnimatorStateTransition transition2 in anyStateTransitions)
            {
                if (transition2 != null)
                {
                    this.CreateAnyStateEdges(transition2);
                }
            }
            List<ChildAnimatorStateMachine> stateMachinesRecursive = this.rootStateMachine.stateMachinesRecursive;
            ChildAnimatorStateMachine item = new ChildAnimatorStateMachine {
                stateMachine = this.rootStateMachine
            };
            stateMachinesRecursive.Add(item);
            foreach (ChildAnimatorStateMachine machine2 in stateMachinesRecursive)
            {
                ChildAnimatorStateMachine[] stateMachines = machine2.stateMachine.stateMachines;
                foreach (ChildAnimatorStateMachine machine3 in stateMachines)
                {
                    AnimatorTransition[] stateMachineTransitions = machine2.stateMachine.GetStateMachineTransitions(machine3.stateMachine);
                    foreach (AnimatorTransition transition3 in stateMachineTransitions)
                    {
                        this.CreateSelectorEdges(transition3, machine2.stateMachine, machine3.stateMachine);
                    }
                }
            }
            AnimatorTransition[] entryTransitions = this.activeStateMachine.entryTransitions;
            foreach (AnimatorTransition transition4 in entryTransitions)
            {
                this.CreateEntryEdges(transition4, this.activeStateMachine);
            }
            this.CreateDefaultStateEdge(this.activeStateMachine.defaultState, this.activeStateMachine);
        }

        private void CreateEdges(UnityEditor.Graphs.AnimationStateMachine.Node srcNode, UnityEditor.Graphs.AnimationStateMachine.Node dstNode, TransitionEditionContext context)
        {
            if ((srcNode != null) && (dstNode != null))
            {
                string key = GenerateConnectionKey(srcNode, dstNode);
                if (this.m_ConnectedSlotsCache.ContainsKey(key))
                {
                    this.m_ConnectedSlotsCache[key].Add(context);
                }
                else if ((srcNode != dstNode) || (((context.sourceState == null) || this.HasState(this.activeStateMachine, context.sourceState, false)) && ((context.sourceStateMachine == null) || this.HasStateMachine(this.activeStateMachine, context.sourceStateMachine, false))))
                {
                    Slot fromSlot = Enumerable.First<Slot>(srcNode.outputSlots);
                    Slot toSlot = Enumerable.First<Slot>(dstNode.inputSlots);
                    this.Connect(fromSlot, toSlot);
                    this.m_ConnectedSlotsCache.Add(key, new EdgeInfo(context));
                }
            }
        }

        private void CreateEntryEdges(AnimatorTransition transition, AnimatorStateMachine owner)
        {
            UnityEditor.Graphs.AnimationStateMachine.Node dstNode = null;
            if (transition.destinationStateMachine != null)
            {
                dstNode = this.FindNodeForEdges(transition.destinationStateMachine);
            }
            else
            {
                dstNode = this.FindNodeForEdges(transition.destinationState);
            }
            this.CreateEdges(this.m_EntryNode, dstNode, new TransitionEditionContext(transition, null, null, owner));
        }

        private void CreateEntryExitNodes()
        {
            this.m_EntryNode = this.CreateAndAddNode<EntryNode>("Entry", this.activeStateMachine.entryPosition);
            this.m_EntryNode.color = UnityEditor.Graphs.Styles.Color.Green;
            this.m_EntryNode.stateMachine = this.activeStateMachine;
            this.m_ExitNode = this.CreateAndAddNode<ExitNode>("Exit", this.activeStateMachine.exitPosition);
            this.m_ExitNode.color = UnityEditor.Graphs.Styles.Color.Red;
        }

        private void CreateNodeFromState(ChildAnimatorState state)
        {
            StateNode node = this.CreateAndAddNode<StateNode>("", state.position);
            node.state = state.state;
            if (this.rootStateMachine.defaultState == state.state)
            {
                node.color = UnityEditor.Graphs.Styles.Color.Orange;
            }
            this.m_StateNodeLookup.Add(state.state, node);
        }

        private void CreateNodeFromStateMachine(ChildAnimatorStateMachine subStateMachine)
        {
            StateMachineNode node = this.CreateAndAddNode<StateMachineNode>("", subStateMachine.position);
            node.stateMachine = subStateMachine.stateMachine;
            node.style = "node hex";
            if ((this.rootStateMachine.defaultState != null) && this.HasState(subStateMachine.stateMachine, this.rootStateMachine.defaultState, true))
            {
                node.color = UnityEditor.Graphs.Styles.Color.Orange;
            }
            this.m_StateMachineNodeLookup.Add(subStateMachine.stateMachine, node);
        }

        private void CreateNodes()
        {
            this.m_StateNodeLookup.Clear();
            this.m_StateMachineNodeLookup.Clear();
            foreach (ChildAnimatorState state in this.activeStateMachine.states)
            {
                this.CreateNodeFromState(state);
            }
            foreach (ChildAnimatorStateMachine machine in this.activeStateMachine.stateMachines)
            {
                this.CreateNodeFromStateMachine(machine);
            }
            this.CreateAnyStateNode();
            this.CreateEntryExitNodes();
            if (this.parentStateMachine != null)
            {
                this.CreateParentStateMachineNode();
            }
        }

        private void CreateParentStateMachineNode()
        {
            StateMachineNode node = this.CreateAndAddNode<StateMachineNode>("(Up) ", this.activeStateMachine.parentStateMachinePosition);
            node.stateMachine = this.parentStateMachine;
            node.style = "node hex";
            if (((this.rootStateMachine.defaultState != null) && this.HasState(this.parentStateMachine, this.rootStateMachine.defaultState, true)) && !this.HasState(this.activeStateMachine, this.rootStateMachine.defaultState, true))
            {
                node.color = UnityEditor.Graphs.Styles.Color.Orange;
            }
            this.m_StateMachineNodeLookup.Add(this.parentStateMachine, node);
        }

        private void CreateSelectorEdges(AnimatorTransition transition, AnimatorStateMachine owner, AnimatorStateMachine sourceStateMachine)
        {
            UnityEditor.Graphs.AnimationStateMachine.Node srcNode = this.FindNodeForEdges(sourceStateMachine);
            if (!(srcNode is EntryNode))
            {
                UnityEditor.Graphs.AnimationStateMachine.Node dstNode = null;
                if (transition.destinationStateMachine != null)
                {
                    dstNode = this.FindNodeForEdges(transition.destinationStateMachine);
                }
                else if (transition.destinationState != null)
                {
                    dstNode = this.FindNodeForEdges(transition.destinationState);
                }
                else if (transition.isExit)
                {
                    dstNode = this.m_ExitNode;
                }
                StateMachineNode node3 = srcNode as StateMachineNode;
                if ((!transition.isExit || (srcNode == null)) || ((node3 == null) || (node3.stateMachine == sourceStateMachine)))
                {
                    this.CreateEdges(srcNode, dstNode, new TransitionEditionContext(transition, null, sourceStateMachine, owner));
                }
            }
        }

        private void CreateStateEdges(AnimatorState sourceState, AnimatorStateTransition transition)
        {
            UnityEditor.Graphs.AnimationStateMachine.Node srcNode = this.FindNodeForEdges(sourceState);
            UnityEditor.Graphs.AnimationStateMachine.Node exitNode = this.m_ExitNode;
            if (transition.destinationStateMachine != null)
            {
                exitNode = this.FindNodeForEdges(transition.destinationStateMachine);
            }
            else if (transition.destinationState != null)
            {
                exitNode = this.FindNodeForEdges(transition.destinationState);
            }
            if (!(exitNode is EntryNode) && (!transition.isExit || (transition.isExit && (srcNode is StateNode))))
            {
                this.CreateEdges(srcNode, exitNode, new TransitionEditionContext(transition, sourceState, null, null));
            }
        }

        private void CreateStateMachineLookup()
        {
            this.m_StateMachineProxyLookup.Clear();
            this.m_StateMachineLookup.Clear();
            this.FillStateMachineLookupFromStateMachine(this.rootStateMachine);
        }

        public bool DisplayDirty()
        {
            return ((this.activeStateMachine.states.Length != this.m_StateCount) || ((this.activeStateMachine.stateMachines.Length != this.m_StateMachineCount) || ((this.rootStateMachine.transitionCount != this.m_TransitionCount) || (this.rootStateMachine.defaultState != this.m_DefaultState))));
        }

        private void FillStateMachineLookupFromStateMachine(AnimatorStateMachine stateMachine)
        {
            foreach (ChildAnimatorStateMachine machine in stateMachine.stateMachines)
            {
                this.FillStateMachineLookupFromStateMachine(machine.stateMachine);
                this.m_StateMachineLookup.Add(machine.stateMachine, stateMachine);
            }
            foreach (ChildAnimatorState state in stateMachine.states)
            {
                this.m_StateMachineProxyLookup.Add(state.state, stateMachine);
            }
        }

        public UnityEditor.Graphs.AnimationStateMachine.Node FindNode(AnimatorState state)
        {
            if (this.m_StateNodeLookup.ContainsKey(state))
            {
                return this.m_StateNodeLookup[state];
            }
            UnityEditor.Graphs.AnimationStateMachine.Node node2 = this.FindStateMachineNodeFromState(state, this.activeStateMachine);
            if (node2 != null)
            {
                return node2;
            }
            if ((this.parentStateMachine != null) && this.HasState(this.rootStateMachine, state, true))
            {
                return this.m_StateMachineNodeLookup[this.parentStateMachine];
            }
            return null;
        }

        public UnityEditor.Graphs.AnimationStateMachine.Node FindNode(AnimatorStateMachine stateMachine)
        {
            if (stateMachine != null)
            {
                if (stateMachine == this.activeStateMachine)
                {
                    return null;
                }
                if (this.m_StateMachineNodeLookup.ContainsKey(stateMachine))
                {
                    return this.m_StateMachineNodeLookup[stateMachine];
                }
                foreach (ChildAnimatorStateMachine machine in this.activeStateMachine.stateMachines)
                {
                    if (this.HasStateMachine(machine.stateMachine, stateMachine, true))
                    {
                        return this.m_StateMachineNodeLookup[machine.stateMachine];
                    }
                }
                if (this.parentStateMachine != null)
                {
                    return this.m_StateMachineNodeLookup[this.parentStateMachine];
                }
            }
            return null;
        }

        public UnityEditor.Graphs.AnimationStateMachine.Node FindNodeForEdges(AnimatorState state)
        {
            return this.FindNode(state);
        }

        public UnityEditor.Graphs.AnimationStateMachine.Node FindNodeForEdges(AnimatorStateMachine stateMachine)
        {
            if (stateMachine == this.activeStateMachine)
            {
                return this.m_EntryNode;
            }
            return this.FindNode(stateMachine);
        }

        private UnityEditor.Graphs.AnimationStateMachine.Node FindStateMachineNodeFromState(AnimatorState state, AnimatorStateMachine stateMachine)
        {
            AnimatorStateMachine machine = null;
            if (this.m_StateMachineProxyLookup.TryGetValue(state, out machine))
            {
                AnimatorStateMachine machine2 = null;
                while (this.m_StateMachineLookup.TryGetValue(machine, out machine2))
                {
                    if (machine2 == stateMachine)
                    {
                        return this.m_StateMachineNodeLookup[machine];
                    }
                    if (machine2 == this.rootStateMachine)
                    {
                        return this.m_StateMachineNodeLookup[this.parentStateMachine];
                    }
                    machine = machine2;
                }
            }
            return null;
        }

        private static string GenerateConnectionKey(UnityEditor.Graphs.Node srcNode, UnityEditor.Graphs.Node dstNode)
        {
            return (srcNode.GetInstanceID() + "->" + dstNode.GetInstanceID());
        }

        public EdgeInfo GetEdgeInfo(UnityEditor.Graphs.Edge edge)
        {
            if (edge.toSlot == null)
            {
                return null;
            }
            return this.m_ConnectedSlotsCache[GenerateConnectionKey(edge.fromSlot.node, edge.toSlot.node)];
        }

        internal override UnityEditor.Graphs.GraphGUI GetEditor()
        {
            UnityEditor.Graphs.AnimationStateMachine.GraphGUI hgui = ScriptableObject.CreateInstance<UnityEditor.Graphs.AnimationStateMachine.GraphGUI>();
            hgui.graph = this;
            hgui.hideFlags = HideFlags.HideAndDontSave;
            return hgui;
        }

        public string GetStateMachinePath(AnimatorStateMachine stateMachine)
        {
            AnimatorStateMachine machine;
            string name = stateMachine.name;
            while (this.m_StateMachineLookup.TryGetValue(stateMachine, out machine))
            {
                name = machine.name + "." + name;
                stateMachine = machine;
            }
            return name;
        }

        public string GetStatePath(AnimatorState state)
        {
            AnimatorStateMachine machine;
            string name = state.name;
            if (this.m_StateMachineProxyLookup.TryGetValue(state, out machine))
            {
                name = this.GetStateMachinePath(machine) + "." + name;
            }
            return name;
        }

        private bool HasState(AnimatorStateMachine stateMachine, AnimatorState state, bool recursive)
        {
            AnimatorStateMachine machine = null;
            bool flag = false;
            if (this.m_StateMachineProxyLookup.TryGetValue(state, out machine))
            {
                if (stateMachine == machine)
                {
                    return true;
                }
                if (recursive)
                {
                    flag = this.HasStateMachine(stateMachine, machine, recursive);
                }
            }
            return flag;
        }

        private bool HasStateMachine(AnimatorStateMachine stateMachineParent, AnimatorStateMachine stateMachineChild, bool recursive)
        {
            AnimatorStateMachine machine = null;
            while (this.m_StateMachineLookup.TryGetValue(stateMachineChild, out machine))
            {
                if (machine == stateMachineParent)
                {
                    return true;
                }
                if (machine == this.rootStateMachine)
                {
                    return false;
                }
                if (!recursive)
                {
                    return false;
                }
                stateMachineChild = machine;
            }
            return false;
        }

        private void ReadAnyStatePosition(AnyStateNode anyStateNode)
        {
            anyStateNode.position.x = this.activeStateMachine.anyStatePosition.x;
            anyStateNode.position.y = this.activeStateMachine.anyStatePosition.y;
        }

        public void ReadNodePositions()
        {
            foreach (UnityEditor.Graphs.AnimationStateMachine.Node node in base.nodes)
            {
                if (node is StateNode)
                {
                    this.ReadStatePosition(node as StateNode);
                }
                if (node is StateMachineNode)
                {
                    this.ReadStateMachinePosition(node as StateMachineNode);
                }
                if (node is AnyStateNode)
                {
                    this.ReadAnyStatePosition(node as AnyStateNode);
                }
            }
        }

        private void ReadStateMachinePosition(StateMachineNode stateMachineNode)
        {
            Vector2 parentStateMachinePosition;
            if (stateMachineNode.stateMachine == this.parentStateMachine)
            {
                parentStateMachinePosition = this.activeStateMachine.parentStateMachinePosition;
            }
            else
            {
                parentStateMachinePosition = this.activeStateMachine.GetStateMachinePosition(stateMachineNode.stateMachine);
            }
            stateMachineNode.position.x = parentStateMachinePosition.x;
            stateMachineNode.position.y = parentStateMachinePosition.y;
        }

        private void ReadStatePosition(StateNode stateNode)
        {
            Vector2 statePosition = this.activeStateMachine.GetStatePosition(stateNode.state);
            stateNode.position.x = statePosition.x;
            stateNode.position.y = statePosition.y;
        }

        public void RebuildGraph()
        {
            if (this.activeStateMachine != null)
            {
                this.BuildGraphFromStateMachine(this.activeStateMachine);
            }
            else
            {
                this.Clear(false);
            }
        }

        public void SetStateMachines(AnimatorStateMachine stateMachine, AnimatorStateMachine parent, AnimatorStateMachine root)
        {
            this.rootStateMachine = root;
            this.parentStateMachine = parent;
            if (stateMachine != this.m_ActiveStateMachine)
            {
                this.BuildGraphFromStateMachine(stateMachine);
            }
        }

        public AnimatorStateMachine activeStateMachine
        {
            get
            {
                return this.m_ActiveStateMachine;
            }
        }
    }
}

