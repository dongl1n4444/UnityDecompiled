namespace UnityEditor.UI
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// <para>Base class for custom editors that are for components that implement the SelfControllerEditor interface.</para>
    /// </summary>
    public class SelfControllerEditor : Editor
    {
        private static string s_Warning = "Parent has a type of layout group component. A child of a layout group should not have a {0} component, since it should be driven by the layout group.";

        /// <summary>
        /// <para>See Editor.OnInspectorGUI.</para>
        /// </summary>
        public override void OnInspectorGUI()
        {
            bool flag = false;
            for (int i = 0; i < base.targets.Length; i++)
            {
                Component component = base.targets[i] as Component;
                ILayoutIgnorer ignorer = component.GetComponent(typeof(ILayoutIgnorer)) as ILayoutIgnorer;
                if ((ignorer == null) || !ignorer.ignoreLayout)
                {
                    RectTransform parent = component.transform.parent as RectTransform;
                    if (parent != null)
                    {
                        Behaviour behaviour = parent.GetComponent(typeof(ILayoutGroup)) as Behaviour;
                        if ((behaviour != null) && behaviour.enabled)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
            }
            if (flag)
            {
                EditorGUILayout.HelpBox(string.Format(s_Warning, ObjectNames.NicifyVariableName(base.target.GetType().Name)), MessageType.Warning);
            }
        }
    }
}

