﻿namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(HingeJoint))]
    internal class HingeJointEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();
            string str = "";
            JointLimits limits = ((HingeJoint) base.target).limits;
            if ((limits.min < -180f) || (limits.min > 180f))
            {
                str = str + "Min Limit needs to be within [-180,180].";
            }
            if ((limits.max < -180f) || (limits.max > 180f))
            {
                str = str + (!string.IsNullOrEmpty(str) ? "\n" : "") + "Max Limit needs to be within [-180,180].";
            }
            if (limits.max < limits.min)
            {
                str = str + (!string.IsNullOrEmpty(str) ? "\n" : "") + "Max Limit needs to be larger or equal to the Min Limit.";
            }
            if (!string.IsNullOrEmpty(str))
            {
                EditorGUILayout.HelpBox(str, MessageType.Warning);
            }
        }
    }
}

