namespace UnityEditor.Graphs.AnimationStateMachine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEditor.Graphs;
    using UnityEngine;

    internal class Node : UnityEditor.Graphs.Node
    {
        [CompilerGenerated]
        private static Predicate<ChildAnimatorState> <>f__am$cache0;
        [CompilerGenerated]
        private static Predicate<ChildAnimatorStateMachine> <>f__am$cache1;
        protected UnityEditor.Graphs.AnimationStateMachine.GraphGUI graphGUI;

        public override void BeginDrag()
        {
            base.BeginDrag();
            Undo.RegisterCompleteObjectUndo(this.undoableObject, "Moved " + this.title);
        }

        public virtual void Connect(UnityEditor.Graphs.AnimationStateMachine.Node toNode, UnityEditor.Graphs.Edge edge)
        {
        }

        public static void GenericMenuForStateMachineNode(StateMachineNode toStateMachineNode, bool showStateMachine, GenericMenu.MenuFunction2 func)
        {
            AnimatorStateMachine parentStateMachine = null;
            List<ChildAnimatorState> stateList = new List<ChildAnimatorState>();
            List<ChildAnimatorStateMachine> stateMachineList = new List<ChildAnimatorStateMachine>();
            PopulateSubNodeList(toStateMachineNode, ref stateList, ref stateMachineList, ref parentStateMachine);
            if ((stateList.Count == 0) && (stateMachineList.Count == 1))
            {
                ChildAnimatorStateMachine machine2 = stateMachineList[0];
                func(machine2.stateMachine);
            }
            else
            {
                GenericMenu menu = new GenericMenu();
                foreach (ChildAnimatorState state in stateList)
                {
                    <GenericMenuForStateMachineNode>c__AnonStorey0 storey = new <GenericMenuForStateMachineNode>c__AnonStorey0();
                    string name = state.state.name;
                    storey.currentParent = state.state.FindParent(AnimatorControllerTool.tool.stateMachineGraph.rootStateMachine);
                    while ((storey.currentParent != null) && (storey.currentParent != parentStateMachine))
                    {
                        name = name.Insert(0, storey.currentParent.name + "/");
                        storey.currentParent = stateMachineList.Find(new Predicate<ChildAnimatorStateMachine>(storey.<>m__0)).stateMachine;
                    }
                    if (((stateList.Count > 0) && (stateMachineList.Count > 0)) && showStateMachine)
                    {
                        name = name.Insert(0, "States/");
                    }
                    menu.AddItem(new GUIContent(name), false, func, state.state);
                }
                if (showStateMachine)
                {
                    foreach (ChildAnimatorStateMachine machine4 in stateMachineList)
                    {
                        string text = machine4.stateMachine.name;
                        if ((stateList.Count > 0) && (stateMachineList.Count > 0))
                        {
                            text = text.Insert(0, "StateMachine/");
                        }
                        menu.AddItem(new GUIContent(text), false, func, machine4.stateMachine);
                    }
                }
                menu.ShowAsContext();
            }
        }

        protected static bool IsDoubleClick()
        {
            Event current = Event.current;
            return (((current.type == EventType.MouseDown) && (current.button == 0)) && (current.clickCount == 2));
        }

        protected static bool IsLeftClick()
        {
            Event current = Event.current;
            return (((current.type == EventType.MouseDown) && (current.button == 0)) && (current.clickCount == 1));
        }

        protected static bool IsRightClick() => 
            (Event.current.type == EventType.ContextClick);

        private static void PopulateSubNodeList(StateMachineNode toStateMachineNode, ref List<ChildAnimatorState> stateList, ref List<ChildAnimatorStateMachine> stateMachineList, ref AnimatorStateMachine parentStateMachine)
        {
            if (toStateMachineNode.stateMachine == AnimatorControllerTool.tool.stateMachineGraph.parentStateMachine)
            {
                stateList = AnimatorControllerTool.tool.stateMachineGraph.rootStateMachine.statesRecursive;
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = s => AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine.HasState(s.state);
                }
                stateList.RemoveAll(<>f__am$cache0);
                stateMachineList = AnimatorControllerTool.tool.stateMachineGraph.rootStateMachine.stateMachinesRecursive;
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = s => AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine.HasStateMachine(s.stateMachine) || (AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine == s.stateMachine);
                }
                stateMachineList.RemoveAll(<>f__am$cache1);
                parentStateMachine = AnimatorControllerTool.tool.stateMachineGraph.rootStateMachine;
            }
            else
            {
                parentStateMachine = toStateMachineNode.stateMachine;
                stateList = toStateMachineNode.stateMachine.statesRecursive;
                stateMachineList = parentStateMachine.stateMachinesRecursive;
            }
            ChildAnimatorStateMachine item = new ChildAnimatorStateMachine {
                stateMachine = parentStateMachine
            };
            stateMachineList.Add(item);
        }

        public virtual UnityEngine.Object selectionObject =>
            null;

        public virtual UnityEngine.Object undoableObject =>
            null;

        [CompilerGenerated]
        private sealed class <GenericMenuForStateMachineNode>c__AnonStorey0
        {
            internal AnimatorStateMachine currentParent;

            internal bool <>m__0(ChildAnimatorStateMachine sm) => 
                sm.stateMachine.IsDirectParent(this.currentParent);
        }
    }
}

