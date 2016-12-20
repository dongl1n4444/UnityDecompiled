namespace UnityEditor
{
    using System;
    using UnityEditor.Analytics;
    using UnityEditor.Connect;
    using UnityEngine;
    using UnityEngine.Analytics;

    [CustomEditor(typeof(AnalyticsTracker)), CanEditMultipleObjects]
    internal class AnalyticsTrackerEditor : Editor
    {
        private SerializedProperty nameProp;
        private SerializedProperty trackableProp;
        private SerializedProperty triggerProp;

        private void OnEnable()
        {
            this.trackableProp = base.serializedObject.FindProperty("m_TrackableProperty");
            this.nameProp = base.serializedObject.FindProperty("m_EventName");
            this.triggerProp = base.serializedObject.FindProperty("m_Trigger");
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            if (!AnalyticsSettings.enabled && Application.isHumanControllingUs)
            {
                EditorGUILayout.LabelField("Analytics is not enabled. Enable it here to use this component:", new GUILayoutOption[0]);
                if (GUILayout.Button("Services tab", new GUILayoutOption[0]))
                {
                    UnityConnectServiceCollection.instance.ShowService("Hub", true);
                }
            }
            EditorGUILayout.PropertyField(this.nameProp, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.triggerProp, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.trackableProp, new GUILayoutOption[0]);
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}

