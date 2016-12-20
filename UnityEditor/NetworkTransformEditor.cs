namespace UnityEditor
{
    using System;
    using UnityEngine;
    using UnityEngine.Networking;

    [CustomEditor(typeof(NetworkTransform), true), CanEditMultipleObjects]
    public class NetworkTransformEditor : Editor
    {
        private static string[] axisOptions = new string[] { "None", "X", "Y (Top-Down 2D)", "Z (Side-on 2D)", "XY (FPS)", "XZ", "YZ", "XYZ (full 3D)" };
        private bool m_Initialized;
        private SerializedProperty m_InterpolateMovement;
        protected GUIContent m_InterpolateMovementLabel;
        private SerializedProperty m_InterpolateRotation;
        protected GUIContent m_InterpolateRotationLabel;
        private SerializedProperty m_MovementTheshold;
        protected GUIContent m_MovementThesholdLabel;
        private GUIContent m_NetworkSendIntervalLabel;
        private SerializedProperty m_NetworkSendIntervalProperty;
        private SerializedProperty m_RotationSyncCompression;
        protected GUIContent m_RotationSyncCompressionLabel;
        private SerializedProperty m_SnapThreshold;
        protected GUIContent m_SnapThresholdLabel;
        private SerializedProperty m_SyncSpin;
        protected GUIContent m_SyncSpinLabel;
        private NetworkTransform m_SyncTransform;
        private SerializedProperty m_TransformSyncMode;

        public void Init()
        {
            if (!this.m_Initialized)
            {
                this.m_Initialized = true;
                this.m_SyncTransform = base.target as NetworkTransform;
                if (this.m_SyncTransform.transformSyncMode == NetworkTransform.TransformSyncMode.SyncNone)
                {
                    if (this.m_SyncTransform.GetComponent<Rigidbody>() != null)
                    {
                        this.m_SyncTransform.transformSyncMode = NetworkTransform.TransformSyncMode.SyncRigidbody3D;
                        this.m_SyncTransform.syncRotationAxis = NetworkTransform.AxisSyncMode.AxisXYZ;
                        EditorUtility.SetDirty(this.m_SyncTransform);
                    }
                    else if (this.m_SyncTransform.GetComponent<Rigidbody2D>() != null)
                    {
                        this.m_SyncTransform.transformSyncMode = NetworkTransform.TransformSyncMode.SyncRigidbody2D;
                        this.m_SyncTransform.syncRotationAxis = NetworkTransform.AxisSyncMode.AxisZ;
                        EditorUtility.SetDirty(this.m_SyncTransform);
                    }
                    else if (this.m_SyncTransform.GetComponent<CharacterController>() != null)
                    {
                        this.m_SyncTransform.transformSyncMode = NetworkTransform.TransformSyncMode.SyncCharacterController;
                        this.m_SyncTransform.syncRotationAxis = NetworkTransform.AxisSyncMode.AxisXYZ;
                        EditorUtility.SetDirty(this.m_SyncTransform);
                    }
                    else
                    {
                        this.m_SyncTransform.transformSyncMode = NetworkTransform.TransformSyncMode.SyncTransform;
                        this.m_SyncTransform.syncRotationAxis = NetworkTransform.AxisSyncMode.AxisXYZ;
                        EditorUtility.SetDirty(this.m_SyncTransform);
                    }
                }
                this.m_TransformSyncMode = base.serializedObject.FindProperty("m_TransformSyncMode");
                this.m_MovementTheshold = base.serializedObject.FindProperty("m_MovementTheshold");
                this.m_SnapThreshold = base.serializedObject.FindProperty("m_SnapThreshold");
                this.m_InterpolateRotation = base.serializedObject.FindProperty("m_InterpolateRotation");
                this.m_InterpolateMovement = base.serializedObject.FindProperty("m_InterpolateMovement");
                this.m_RotationSyncCompression = base.serializedObject.FindProperty("m_RotationSyncCompression");
                this.m_SyncSpin = base.serializedObject.FindProperty("m_SyncSpin");
                this.m_NetworkSendIntervalProperty = base.serializedObject.FindProperty("m_SendInterval");
                this.m_NetworkSendIntervalLabel = new GUIContent("Network Send Rate (seconds)", "Number of network updates per second");
                EditorGUI.indentLevel++;
                this.m_MovementThesholdLabel = new GUIContent("Movement Threshold");
                this.m_SnapThresholdLabel = new GUIContent("Snap Threshold");
                this.m_InterpolateRotationLabel = new GUIContent("Interpolate Rotation Factor");
                this.m_InterpolateMovementLabel = new GUIContent("Interpolate Movement Factor");
                this.m_RotationSyncCompressionLabel = new GUIContent("Compress Rotation");
                this.m_SyncSpinLabel = new GUIContent("Sync Angular Velocity");
                EditorGUI.indentLevel--;
            }
        }

        public override void OnInspectorGUI()
        {
            this.ShowControls();
        }

        protected void ShowControls()
        {
            if (this.m_TransformSyncMode == null)
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
            EditorGUILayout.PropertyField(this.m_TransformSyncMode, new GUILayoutOption[0]);
            if ((this.m_TransformSyncMode.enumValueIndex == 3) && (this.m_SyncTransform.GetComponent<Rigidbody>() == null))
            {
                Debug.LogError("Object has no Rigidbody component.");
                this.m_TransformSyncMode.enumValueIndex = 1;
                EditorUtility.SetDirty(this.m_SyncTransform);
            }
            if ((this.m_TransformSyncMode.enumValueIndex == 2) && (this.m_SyncTransform.GetComponent<Rigidbody2D>() == null))
            {
                Debug.LogError("Object has no Rigidbody2D component.");
                this.m_TransformSyncMode.enumValueIndex = 1;
                EditorUtility.SetDirty(this.m_SyncTransform);
            }
            if ((this.m_TransformSyncMode.enumValueIndex == 4) && (this.m_SyncTransform.GetComponent<CharacterController>() == null))
            {
                Debug.LogError("Object has no CharacterController component.");
                this.m_TransformSyncMode.enumValueIndex = 1;
                EditorUtility.SetDirty(this.m_SyncTransform);
            }
            EditorGUILayout.LabelField("Movement:", new GUILayoutOption[0]);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(this.m_MovementTheshold, this.m_MovementThesholdLabel, new GUILayoutOption[0]);
            if (this.m_MovementTheshold.floatValue < 0f)
            {
                this.m_MovementTheshold.floatValue = 0f;
                EditorUtility.SetDirty(this.m_SyncTransform);
            }
            EditorGUILayout.PropertyField(this.m_SnapThreshold, this.m_SnapThresholdLabel, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_InterpolateMovement, this.m_InterpolateMovementLabel, new GUILayoutOption[0]);
            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField("Rotation:", new GUILayoutOption[0]);
            EditorGUI.indentLevel++;
            int num3 = EditorGUILayout.Popup("Rotation Axis", (int) this.m_SyncTransform.syncRotationAxis, axisOptions, new GUILayoutOption[0]);
            if (num3 != this.m_SyncTransform.syncRotationAxis)
            {
                this.m_SyncTransform.syncRotationAxis = (NetworkTransform.AxisSyncMode) num3;
                EditorUtility.SetDirty(this.m_SyncTransform);
            }
            EditorGUILayout.PropertyField(this.m_InterpolateRotation, this.m_InterpolateRotationLabel, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_RotationSyncCompression, this.m_RotationSyncCompressionLabel, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_SyncSpin, this.m_SyncSpinLabel, new GUILayoutOption[0]);
            EditorGUI.indentLevel--;
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}

