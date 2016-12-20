namespace UnityEditor.Graphs
{
    using System;
    using UnityEditor.Animations;

    internal class AnimatorControllerCallback
    {
        internal static void OnInvalidateAnimatorController(AnimatorController controller)
        {
            if ((AnimatorControllerTool.tool != null) && (AnimatorControllerTool.tool.animatorController == controller))
            {
                AnimatorControllerTool.tool.OnInvalidateAnimatorController();
            }
            if ((ParameterControllerEditor.tool != null) && (ParameterControllerEditor.tool.animatorController == controller))
            {
                ParameterControllerEditor.tool.OnInvalidateAnimatorController();
            }
        }
    }
}

