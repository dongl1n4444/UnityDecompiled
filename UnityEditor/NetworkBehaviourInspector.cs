namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;
    using UnityEngine.Networking;

    [CustomEditor(typeof(NetworkBehaviour), true), CanEditMultipleObjects]
    public class NetworkBehaviourInspector : Editor
    {
        private bool m_HasOnSerialize;
        private bool m_Initialized;
        protected GUIContent m_NetworkChannelLabel;
        protected GUIContent m_NetworkSendIntervalLabel;
        private System.Type m_ScriptClass;
        private bool[] m_ShowSyncLists;
        protected List<string> m_SyncVarNames = new List<string>();

        private void Init(MonoScript script)
        {
            this.m_Initialized = true;
            this.m_ScriptClass = script.GetClass();
            this.m_NetworkChannelLabel = new GUIContent("Network Channel", "QoS channel used for updates. Use the [NetworkSettings] class attribute to change this.");
            this.m_NetworkSendIntervalLabel = new GUIContent("Network Send Interval", "Maximum update rate in seconds. Use the [NetworkSettings] class attribute to change this, or implement GetNetworkSendInterval");
            foreach (System.Reflection.FieldInfo info in this.m_ScriptClass.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                Attribute[] customAttributes = (Attribute[]) info.GetCustomAttributes(typeof(SyncVarAttribute), true);
                if (customAttributes.Length > 0)
                {
                    this.m_SyncVarNames.Add(info.Name);
                }
            }
            MethodInfo method = script.GetClass().GetMethod("OnSerialize");
            if ((method != null) && (method.DeclaringType != typeof(NetworkBehaviour)))
            {
                this.m_HasOnSerialize = true;
            }
            int num2 = 0;
            foreach (System.Reflection.FieldInfo info3 in base.serializedObject.targetObject.GetType().GetFields())
            {
                if ((info3.FieldType.BaseType != null) && info3.FieldType.BaseType.Name.Contains("SyncList"))
                {
                    num2++;
                }
            }
            if (num2 > 0)
            {
                this.m_ShowSyncLists = new bool[num2];
            }
        }

        public override void OnInspectorGUI()
        {
            if (!this.m_Initialized)
            {
                base.serializedObject.Update();
                SerializedProperty property = base.serializedObject.FindProperty("m_Script");
                if (property == null)
                {
                    return;
                }
                MonoScript objectReferenceValue = property.objectReferenceValue as MonoScript;
                this.Init(objectReferenceValue);
            }
            EditorGUI.BeginChangeCheck();
            base.serializedObject.Update();
            SerializedProperty iterator = base.serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                bool flag2 = this.m_SyncVarNames.Contains(iterator.name);
                if (iterator.propertyType == SerializedPropertyType.ObjectReference)
                {
                    if (iterator.name == "m_Script")
                    {
                        if (this.hideScriptField)
                        {
                            continue;
                        }
                        EditorGUI.BeginDisabledGroup(true);
                    }
                    EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
                    if (iterator.name == "m_Script")
                    {
                        EditorGUI.EndDisabledGroup();
                    }
                }
                else
                {
                    EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
                    if (flag2)
                    {
                        GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(52f) };
                        GUILayout.Label("SyncVar", EditorStyles.miniLabel, options);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                enterChildren = false;
            }
            base.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
            int index = 0;
            foreach (System.Reflection.FieldInfo info in base.serializedObject.targetObject.GetType().GetFields())
            {
                if ((info.FieldType.BaseType != null) && info.FieldType.BaseType.Name.Contains("SyncList"))
                {
                    this.m_ShowSyncLists[index] = EditorGUILayout.Foldout(this.m_ShowSyncLists[index], "SyncList " + info.Name + "  [" + info.FieldType.Name + "]");
                    if (this.m_ShowSyncLists[index])
                    {
                        EditorGUI.indentLevel++;
                        IEnumerable enumerable = info.GetValue(base.serializedObject.targetObject) as IEnumerable;
                        if (enumerable != null)
                        {
                            int num3 = 0;
                            IEnumerator enumerator = enumerable.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                if (enumerator.Current != null)
                                {
                                    EditorGUILayout.LabelField("Item:" + num3, enumerator.Current.ToString(), new GUILayoutOption[0]);
                                }
                                num3++;
                            }
                        }
                        EditorGUI.indentLevel--;
                    }
                    index++;
                }
            }
            if (this.m_HasOnSerialize)
            {
                NetworkBehaviour target = base.target as NetworkBehaviour;
                if (target != null)
                {
                    EditorGUILayout.LabelField(this.m_NetworkChannelLabel, new GUIContent(target.GetNetworkChannel().ToString()), new GUILayoutOption[0]);
                    EditorGUILayout.LabelField(this.m_NetworkSendIntervalLabel, new GUIContent(target.GetNetworkSendInterval().ToString()), new GUILayoutOption[0]);
                }
            }
        }

        internal virtual bool hideScriptField =>
            false;
    }
}

