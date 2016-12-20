namespace UnityEditor.Graphs.AnimationStateMachine
{
    using System;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEngine;

    [CustomEditor(typeof(StateMachineBehaviour), true)]
    internal class StateMachineBehaviourEditor : Editor
    {
        [UnityEditor.MenuItem("CONTEXT/StateMachineBehaviour/Move Down")]
        internal static void MoveDown(MenuCommand command)
        {
            StateMachineBehaviour context = command.context as StateMachineBehaviour;
            StateMachineBehaviourContext[] contextArray = AnimatorController.FindStateMachineBehaviourContext(context);
            for (int i = 0; i < contextArray.Length; i++)
            {
                AnimatorController animatorController = contextArray[i].animatorController;
                AnimatorState animatorObject = contextArray[i].animatorObject as AnimatorState;
                AnimatorStateMachine machine = contextArray[i].animatorObject as AnimatorStateMachine;
                if (animatorObject != null)
                {
                    StateMachineBehaviour[] behaviours = (animatorController == null) ? animatorObject.behaviours : animatorController.GetStateEffectiveBehaviours(animatorObject, contextArray[i].layerIndex);
                    for (int j = 0; j < behaviours.Length; j++)
                    {
                        if ((behaviours[j] == context) && (j < (behaviours.Length - 1)))
                        {
                            behaviours[j] = behaviours[j + 1];
                            behaviours[j + 1] = context;
                            break;
                        }
                    }
                    if (animatorController != null)
                    {
                        animatorController.SetStateEffectiveBehaviours(animatorObject, contextArray[i].layerIndex, behaviours);
                    }
                    else
                    {
                        animatorObject.behaviours = behaviours;
                    }
                    continue;
                }
                if (machine != null)
                {
                    StateMachineBehaviour[] behaviourArray2 = machine.behaviours;
                    for (int k = 0; k < behaviourArray2.Length; k++)
                    {
                        if ((behaviourArray2[k] == context) && (k < (behaviourArray2.Length - 1)))
                        {
                            behaviourArray2[k] = behaviourArray2[k + 1];
                            behaviourArray2[k + 1] = context;
                            break;
                        }
                    }
                    machine.behaviours = behaviourArray2;
                }
            }
        }

        [UnityEditor.MenuItem("CONTEXT/StateMachineBehaviour/Move Up")]
        internal static void MoveUp(MenuCommand command)
        {
            StateMachineBehaviour context = command.context as StateMachineBehaviour;
            StateMachineBehaviourContext[] contextArray = AnimatorController.FindStateMachineBehaviourContext(context);
            for (int i = 0; i < contextArray.Length; i++)
            {
                AnimatorController animatorController = contextArray[i].animatorController;
                AnimatorState animatorObject = contextArray[i].animatorObject as AnimatorState;
                AnimatorStateMachine machine = contextArray[i].animatorObject as AnimatorStateMachine;
                if (animatorObject != null)
                {
                    StateMachineBehaviour[] behaviours = (animatorController == null) ? animatorObject.behaviours : animatorController.GetStateEffectiveBehaviours(animatorObject, contextArray[i].layerIndex);
                    for (int j = 0; j < behaviours.Length; j++)
                    {
                        if ((behaviours[j] == context) && (j > 0))
                        {
                            behaviours[j] = behaviours[j - 1];
                            behaviours[j - 1] = context;
                            break;
                        }
                    }
                    if (animatorController != null)
                    {
                        animatorController.SetStateEffectiveBehaviours(animatorObject, contextArray[i].layerIndex, behaviours);
                    }
                    else
                    {
                        animatorObject.behaviours = behaviours;
                    }
                    continue;
                }
                if (machine != null)
                {
                    StateMachineBehaviour[] behaviourArray2 = machine.behaviours;
                    for (int k = 0; k < behaviourArray2.Length; k++)
                    {
                        if ((behaviourArray2[k] == context) && (k > 0))
                        {
                            behaviourArray2[k] = behaviourArray2[k - 1];
                            behaviourArray2[k - 1] = context;
                            break;
                        }
                    }
                    machine.behaviours = behaviourArray2;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            SerializedProperty iterator = base.serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                using (new EditorGUI.DisabledScope(iterator.name == "m_Script"))
                {
                    EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
                    enterChildren = false;
                }
            }
            base.serializedObject.ApplyModifiedProperties();
        }

        [UnityEditor.MenuItem("CONTEXT/StateMachineBehaviour/Remove")]
        internal static void Remove(MenuCommand command)
        {
            StateMachineBehaviour context = command.context as StateMachineBehaviour;
            StateMachineBehaviourContext[] contextArray = AnimatorController.FindStateMachineBehaviourContext(context);
            for (int i = 0; i < contextArray.Length; i++)
            {
                AnimatorController animatorController = contextArray[i].animatorController;
                AnimatorState animatorObject = contextArray[i].animatorObject as AnimatorState;
                AnimatorStateMachine machine = contextArray[i].animatorObject as AnimatorStateMachine;
                if (animatorObject != null)
                {
                    StateMachineBehaviour[] array = (animatorController == null) ? animatorObject.behaviours : animatorController.GetStateEffectiveBehaviours(animatorObject, contextArray[i].layerIndex);
                    ArrayUtility.Remove<StateMachineBehaviour>(ref array, context);
                    if (animatorController != null)
                    {
                        animatorController.SetStateEffectiveBehaviours(animatorObject, contextArray[i].layerIndex, array);
                    }
                    else
                    {
                        animatorObject.behaviours = array;
                    }
                }
                else if (machine != null)
                {
                    StateMachineBehaviour[] behaviours = machine.behaviours;
                    ArrayUtility.Remove<StateMachineBehaviour>(ref behaviours, context);
                    machine.behaviours = behaviours;
                }
            }
            UnityEngine.Object.DestroyImmediate(context, true);
        }

        internal static bool ValidateMenucommand(MenuCommand command)
        {
            if (!AnimatorController.CanAddStateMachineBehaviours())
            {
                return false;
            }
            StateMachineBehaviour context = command.context as StateMachineBehaviour;
            StateMachineBehaviourContext[] contextArray = AnimatorController.FindStateMachineBehaviourContext(context);
            for (int i = 0; i < contextArray.Length; i++)
            {
                AnimatorController animatorController = contextArray[i].animatorController;
                AnimatorState animatorObject = contextArray[i].animatorObject as AnimatorState;
                AnimatorStateMachine machine = contextArray[i].animatorObject as AnimatorStateMachine;
                StateMachineBehaviour[] behaviours = null;
                if (animatorObject != null)
                {
                    behaviours = (animatorController == null) ? animatorObject.behaviours : animatorController.GetStateEffectiveBehaviours(animatorObject, contextArray[i].layerIndex);
                }
                else if (machine != null)
                {
                    behaviours = machine.behaviours;
                }
                if (behaviours != null)
                {
                    for (int j = 0; j < behaviours.Length; j++)
                    {
                        if (behaviours[j] == null)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        [UnityEditor.MenuItem("CONTEXT/StateMachineBehaviour/Move Down", true)]
        internal static bool ValidateMoveDown(MenuCommand command)
        {
            return ValidateMenucommand(command);
        }

        [UnityEditor.MenuItem("CONTEXT/StateMachineBehaviour/Move Up", true)]
        internal static bool ValidateMoveUp(MenuCommand command)
        {
            return ValidateMenucommand(command);
        }

        [UnityEditor.MenuItem("CONTEXT/StateMachineBehaviour/Remove", true)]
        internal static bool ValidateRemove(MenuCommand command)
        {
            return ValidateMenucommand(command);
        }
    }
}

