namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class RotateTool : ManipulationTool
    {
        private static RotateTool s_Instance;

        public static void OnGUI(SceneView view)
        {
            if (s_Instance == null)
            {
                s_Instance = new RotateTool();
            }
            s_Instance.OnToolGUI(view);
        }

        public override void ToolGUI(SceneView view, Vector3 handlePosition, bool isStatic)
        {
            Quaternion handleRotation = Tools.handleRotation;
            EditorGUI.BeginChangeCheck();
            Quaternion quaternion2 = Handles.RotationHandle(handleRotation, handlePosition);
            if (EditorGUI.EndChangeCheck() && !isStatic)
            {
                float num;
                Vector3 vector;
                (Quaternion.Inverse(handleRotation) * quaternion2).ToAngleAxis(out num, out vector);
                Undo.RecordObjects(Selection.transforms, "Rotate");
                foreach (Transform transform in Selection.transforms)
                {
                    if (Tools.pivotMode == PivotMode.Center)
                    {
                        transform.RotateAround(handlePosition, (Vector3) (handleRotation * vector), num);
                    }
                    else if (TransformManipulator.individualSpace)
                    {
                        transform.Rotate((Vector3) (transform.rotation * vector), num, Space.World);
                    }
                    else
                    {
                        transform.Rotate((Vector3) (handleRotation * vector), num, Space.World);
                    }
                    transform.SetLocalEulerHint(transform.GetLocalEulerAngles(transform.rotationOrder));
                    if (transform.parent != null)
                    {
                        transform.SendTransformChangedScale();
                    }
                }
                Tools.handleRotation = quaternion2;
            }
        }
    }
}

