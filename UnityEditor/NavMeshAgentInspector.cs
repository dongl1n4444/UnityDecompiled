namespace UnityEditor
{
    using System;
    using UnityEditor.AI;
    using UnityEngine;
    using UnityEngine.AI;

    [CanEditMultipleObjects, CustomEditor(typeof(NavMeshAgent))]
    internal class NavMeshAgentInspector : Editor
    {
        private SerializedProperty m_Acceleration;
        private SerializedProperty m_AgentTypeID;
        private SerializedProperty m_AngularSpeed;
        private SerializedProperty m_AutoBraking;
        private SerializedProperty m_AutoRepath;
        private SerializedProperty m_AutoTraverseOffMeshLink;
        private SerializedProperty m_AvoidancePriority;
        private SerializedProperty m_BaseOffset;
        private SerializedProperty m_ObstacleAvoidanceType;
        private SerializedProperty m_Speed;
        private SerializedProperty m_StoppingDistance;
        private SerializedProperty m_WalkableMask;
        private static Styles s_Styles;

        private static void AgentTypePopupInternal(string labelName, SerializedProperty agentTypeID)
        {
            int selectedIndex = -1;
            int settingsCount = NavMesh.GetSettingsCount();
            string[] displayedOptions = new string[settingsCount + 2];
            for (int i = 0; i < settingsCount; i++)
            {
                int num4 = NavMesh.GetSettingsByIndex(i).agentTypeID;
                displayedOptions[i] = NavMesh.GetSettingsNameFromID(num4);
                if (num4 == agentTypeID.intValue)
                {
                    selectedIndex = i;
                }
            }
            displayedOptions[settingsCount] = "";
            displayedOptions[settingsCount + 1] = "Open Agent Settings...";
            if (selectedIndex == -1)
            {
                EditorGUILayout.HelpBox("Agent Type invalid.", MessageType.Warning);
            }
            Rect totalPosition = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, new GUILayoutOption[0]);
            EditorGUI.BeginProperty(totalPosition, GUIContent.none, agentTypeID);
            EditorGUI.BeginChangeCheck();
            selectedIndex = EditorGUI.Popup(totalPosition, labelName, selectedIndex, displayedOptions);
            if (EditorGUI.EndChangeCheck())
            {
                if ((selectedIndex >= 0) && (selectedIndex < settingsCount))
                {
                    int num5 = NavMesh.GetSettingsByIndex(selectedIndex).agentTypeID;
                    agentTypeID.intValue = num5;
                }
                else if (selectedIndex == (settingsCount + 1))
                {
                    NavMeshEditorHelpers.OpenAgentSettings(-1);
                }
            }
            EditorGUI.EndProperty();
        }

        private void OnEnable()
        {
            this.m_AgentTypeID = base.serializedObject.FindProperty("m_AgentTypeID");
            this.m_WalkableMask = base.serializedObject.FindProperty("m_WalkableMask");
            this.m_Speed = base.serializedObject.FindProperty("m_Speed");
            this.m_Acceleration = base.serializedObject.FindProperty("m_Acceleration");
            this.m_AngularSpeed = base.serializedObject.FindProperty("m_AngularSpeed");
            this.m_StoppingDistance = base.serializedObject.FindProperty("m_StoppingDistance");
            this.m_AutoTraverseOffMeshLink = base.serializedObject.FindProperty("m_AutoTraverseOffMeshLink");
            this.m_AutoBraking = base.serializedObject.FindProperty("m_AutoBraking");
            this.m_AutoRepath = base.serializedObject.FindProperty("m_AutoRepath");
            this.m_BaseOffset = base.serializedObject.FindProperty("m_BaseOffset");
            this.m_ObstacleAvoidanceType = base.serializedObject.FindProperty("m_ObstacleAvoidanceType");
            this.m_AvoidancePriority = base.serializedObject.FindProperty("avoidancePriority");
        }

        public override void OnInspectorGUI()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            base.serializedObject.Update();
            AgentTypePopupInternal("Agent Type", this.m_AgentTypeID);
            EditorGUILayout.PropertyField(this.m_BaseOffset, new GUILayoutOption[0]);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(s_Styles.m_AgentSteeringHeader, EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Speed, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_AngularSpeed, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Acceleration, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_StoppingDistance, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_AutoBraking, new GUILayoutOption[0]);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(s_Styles.m_AgentAvoidanceHeader, EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_ObstacleAvoidanceType, GUIContent.Temp("Quality"), new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_AvoidancePriority, GUIContent.Temp("Priority"), new GUILayoutOption[0]);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(s_Styles.m_AgentPathFindingHeader, EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_AutoTraverseOffMeshLink, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_AutoRepath, new GUILayoutOption[0]);
            string[] navMeshAreaNames = GameObjectUtility.GetNavMeshAreaNames();
            long longValue = this.m_WalkableMask.longValue;
            int mask = 0;
            for (int i = 0; i < navMeshAreaNames.Length; i++)
            {
                int navMeshAreaFromName = GameObjectUtility.GetNavMeshAreaFromName(navMeshAreaNames[i]);
                if (((((int) 1) << navMeshAreaFromName) & longValue) != 0L)
                {
                    mask |= ((int) 1) << i;
                }
            }
            Rect position = GUILayoutUtility.GetRect(EditorGUILayout.kLabelFloatMinW, EditorGUILayout.kLabelFloatMaxW, 16f, 16f, EditorStyles.layerMaskField);
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = this.m_WalkableMask.hasMultipleDifferentValues;
            int num5 = EditorGUI.MaskField(position, "Area Mask", mask, navMeshAreaNames, EditorStyles.layerMaskField);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                if (num5 == -1)
                {
                    this.m_WalkableMask.longValue = 0xffffffffL;
                }
                else
                {
                    uint num6 = 0;
                    for (int j = 0; j < navMeshAreaNames.Length; j++)
                    {
                        if (((num5 >> j) & 1) != 0)
                        {
                            num6 |= ((uint) 1) << GameObjectUtility.GetNavMeshAreaFromName(navMeshAreaNames[j]);
                        }
                    }
                    this.m_WalkableMask.longValue = num6;
                }
            }
            base.serializedObject.ApplyModifiedProperties();
        }

        private class Styles
        {
            public readonly GUIContent m_AgentAvoidanceHeader = new GUIContent("Obstacle Avoidance");
            public readonly GUIContent m_AgentPathFindingHeader = new GUIContent("Path Finding");
            public readonly GUIContent m_AgentSteeringHeader = new GUIContent("Steering");
        }
    }
}

