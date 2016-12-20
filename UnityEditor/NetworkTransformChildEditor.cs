namespace UnityEditor
{
    using System;
    using UnityEngine;
    using UnityEngine.Networking;

    [CustomEditor(typeof(NetworkTransformChild), true), CanEditMultipleObjects]
    public class NetworkTransformChildEditor : Editor
    {
        private static string[] axisOptions = new string[] { "None", "X", "Y (Top-Down 2D)", "Z (Side-on 2D)", "XY (FPS)", "XZ", "YZ", "XYZ (full 3D)" };
        private bool m_Initialized = false;
        private SerializedProperty m_InterpolateMovement;
        protected GUIContent m_InterpolateMovementLabel;
        private SerializedProperty m_InterpolateRotation;
        protected GUIContent m_InterpolateRotationLabel;
        private SerializedProperty m_MovementThreshold;
        protected GUIContent m_MovementThresholdLabel;
        private GUIContent m_NetworkSendIntervalLabel;
        private SerializedProperty m_NetworkSendIntervalProperty;
        private SerializedProperty m_RotationSyncCompression;
        protected GUIContent m_RotationSyncCompressionLabel;
        private SerializedProperty m_Target;
        private NetworkTransformChild sync;

        public void Init()
        {
            if (!this.m_Initialized)
            {
                this.m_Initialized = true;
                this.sync = base.target as NetworkTransformChild;
                this.m_Target = base.serializedObject.FindProperty("m_Target");
                if (this.sync.GetComponent<NetworkTransform>() == null)
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("NetworkTransformChild must be on the root object with the NetworkTransform, not on the child node");
                    }
                    this.m_Target.objectReferenceValue = null;
                }
                this.m_MovementThreshold = base.serializedObject.FindProperty("m_MovementThreshold");
                this.m_InterpolateRotation = base.serializedObject.FindProperty("m_InterpolateRotation");
                this.m_InterpolateMovement = base.serializedObject.FindProperty("m_InterpolateMovement");
                this.m_RotationSyncCompression = base.serializedObject.FindProperty("m_RotationSyncCompression");
                this.m_NetworkSendIntervalProperty = base.serializedObject.FindProperty("m_SendInterval");
                this.m_NetworkSendIntervalLabel = new GUIContent("Network Send Rate (seconds)", "Number of network updates per second");
                EditorGUI.indentLevel++;
                this.m_MovementThresholdLabel = new GUIContent("Movement Threshold");
                this.m_InterpolateRotationLabel = new GUIContent("Interpolate Rotation Factor");
                this.m_InterpolateMovementLabel = new GUIContent("Interpolate Movement Factor");
                this.m_RotationSyncCompressionLabel = new GUIContent("Compress Rotation");
                EditorGUI.indentLevel--;
            }
        }

        public override void OnInspectorGUI()
        {
            this.ShowControls();
        }

        protected void ShowControls()
        {
            if (this.m_Target == null)
            {
                this.m_Initialized = false;
            }
            this.Init();
            base.serializedObject.Update();
            int num = 0;
            if (this.m_NetworkSendIntervalProperty.floatValue != 0f)
            {
                num = (int) (1f / this.m_NetworkSendIntervalProperty.floatValue);
            }
            int num2 = EditorGUILayout.IntSlider(this.m_NetworkSendIntervalLabel, num, 0, 30, new GUILayoutOption[0]);
            if (num2 != num)
            {
                if (num2 == 0)
                {
                    this.m_NetworkSendIntervalProperty.floatValue = 0f;
                }
                else
                {
                    this.m_NetworkSendIntervalProperty.floatValue = 1f / ((float) num2);
                }
            }
            if (EditorGUILayout.PropertyField(this.m_Target, new GUILayoutOption[0]) && (this.sync.GetComponent<NetworkTransform>() == null))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkTransformChild must be on the root object with the NetworkTransform, not on the child node");
                }
                this.m_Target.objectReferenceValue = null;
            }
            EditorGUILayout.PropertyField(this.m_MovementThreshold, this.m_MovementThresholdLabel, new GUILayoutOption[0]);
            if (this.m_MovementThreshold.floatValue < 0f)
            {
                this.m_MovementThreshold.floatValue = 0f;
                EditorUtility.SetDirty(this.sync);
            }
            EditorGUILayout.PropertyField(this.m_InterpolateMovement, this.m_InterpolateMovementLabel, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_InterpolateRotation, this.m_InterpolateRotationLabel, new GUILayoutOption[0]);
            int num3 = EditorGUILayout.Popup("Rotation Axis", (int) this.sync.syncRotationAxis, axisOptions, new GUILayoutOption[0]);
            if (num3 != this.sync.syncRotationAxis)
            {
                this.sync.syncRotationAxis = (NetworkTransform.AxisSyncMode) num3;
                EditorUtility.SetDirty(this.sync);
            }
            EditorGUILayout.PropertyField(this.m_RotationSyncCompression, this.m_RotationSyncCompressionLabel, new GUILayoutOption[0]);
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}

