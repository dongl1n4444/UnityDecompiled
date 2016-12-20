namespace UnityEditor.Graphs.AnimationStateMachine
{
    using System;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEngine;

    [CustomEditor(typeof(InvalidStateMachineBehaviour), false)]
    internal class InvalidStateMachineBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            InvalidStateMachineBehaviour target = base.target as InvalidStateMachineBehaviour;
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.ObjectField("Script", target.monoScript, typeof(MonoScript), false, new GUILayoutOption[0]);
            }
            if ((target.monoScript != null) && !target.monoScript.GetScriptTypeWasJustCreatedFromComponentMenu())
            {
                EditorGUILayout.HelpBox(EditorGUIUtility.TextContent("The associated script can not be loaded.\nPlease fix any compile errors\nand assign a valid script.").text, MessageType.Warning, true);
            }
        }

        [UnityEditor.MenuItem("CONTEXT/InvalidStateMachineBehaviour/Remove")]
        private static void Remove(MenuCommand command)
        {
            InvalidStateMachineBehaviour context = command.context as InvalidStateMachineBehaviour;
            AnimatorController controller = context.controller;
            AnimatorState state = context.state;
            AnimatorStateMachine stateMachine = context.stateMachine;
            if (state != null)
            {
                if (controller != null)
                {
                    controller.RemoveStateEffectiveBehaviour(state, context.layerIndex, context.behaviourIndex);
                }
                else
                {
                    state.RemoveBehaviour(context.behaviourIndex);
                }
            }
            else if (stateMachine != null)
            {
                stateMachine.RemoveBehaviour(context.behaviourIndex);
            }
            UnityEngine.Object.DestroyImmediate(context, true);
        }
    }
}

