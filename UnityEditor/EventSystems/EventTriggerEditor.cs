namespace UnityEditor.EventSystems
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// <para>Custom Editor for the EventTrigger Component.</para>
    /// </summary>
    [CustomEditor(typeof(EventTrigger), true)]
    public class EventTriggerEditor : Editor
    {
        private GUIContent m_AddButonContent;
        private SerializedProperty m_DelegatesProperty;
        private GUIContent m_EventIDName;
        private GUIContent[] m_EventTypes;
        private GUIContent m_IconToolbarMinus;

        private void OnAddNewSelected(object index)
        {
            int num = (int) index;
            this.m_DelegatesProperty.arraySize++;
            this.m_DelegatesProperty.GetArrayElementAtIndex(this.m_DelegatesProperty.arraySize - 1).FindPropertyRelative("eventID").enumValueIndex = num;
            base.serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnEnable()
        {
            this.m_DelegatesProperty = base.serializedObject.FindProperty("m_Delegates");
            this.m_AddButonContent = new GUIContent("Add New Event Type");
            this.m_EventIDName = new GUIContent("");
            this.m_IconToolbarMinus = new GUIContent(EditorGUIUtility.IconContent("Toolbar Minus"));
            this.m_IconToolbarMinus.tooltip = "Remove all events in this list.";
            string[] names = Enum.GetNames(typeof(EventTriggerType));
            this.m_EventTypes = new GUIContent[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                this.m_EventTypes[i] = new GUIContent(names[i]);
            }
        }

        /// <summary>
        /// <para>Implement specific EventTrigger inspector GUI code here. If you want to simply extend the existing editor call the base OnInspectorGUI () before doing any custom GUI code.</para>
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            int toBeRemovedEntry = -1;
            EditorGUILayout.Space();
            Vector2 vector = GUIStyle.none.CalcSize(this.m_IconToolbarMinus);
            for (int i = 0; i < this.m_DelegatesProperty.arraySize; i++)
            {
                SerializedProperty arrayElementAtIndex = this.m_DelegatesProperty.GetArrayElementAtIndex(i);
                SerializedProperty property2 = arrayElementAtIndex.FindPropertyRelative("eventID");
                SerializedProperty property = arrayElementAtIndex.FindPropertyRelative("callback");
                this.m_EventIDName.text = property2.enumDisplayNames[property2.enumValueIndex];
                EditorGUILayout.PropertyField(property, this.m_EventIDName, new GUILayoutOption[0]);
                Rect lastRect = GUILayoutUtility.GetLastRect();
                Rect rect2 = new Rect((lastRect.xMax - vector.x) - 8f, lastRect.y + 1f, vector.x, vector.y);
                if (GUI.Button(rect2, this.m_IconToolbarMinus, GUIStyle.none))
                {
                    toBeRemovedEntry = i;
                }
                EditorGUILayout.Space();
            }
            if (toBeRemovedEntry > -1)
            {
                this.RemoveEntry(toBeRemovedEntry);
            }
            Rect position = GUILayoutUtility.GetRect(this.m_AddButonContent, GUI.skin.button);
            position.x += (position.width - 200f) / 2f;
            position.width = 200f;
            if (GUI.Button(position, this.m_AddButonContent))
            {
                this.ShowAddTriggermenu();
            }
            base.serializedObject.ApplyModifiedProperties();
        }

        private void RemoveEntry(int toBeRemovedEntry)
        {
            this.m_DelegatesProperty.DeleteArrayElementAtIndex(toBeRemovedEntry);
        }

        private void ShowAddTriggermenu()
        {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < this.m_EventTypes.Length; i++)
            {
                bool flag = true;
                for (int j = 0; j < this.m_DelegatesProperty.arraySize; j++)
                {
                    if (this.m_DelegatesProperty.GetArrayElementAtIndex(j).FindPropertyRelative("eventID").enumValueIndex == i)
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    menu.AddItem(this.m_EventTypes[i], false, new GenericMenu.MenuFunction2(this.OnAddNewSelected), i);
                }
                else
                {
                    menu.AddDisabledItem(this.m_EventTypes[i]);
                }
            }
            menu.ShowAsContext();
            Event.current.Use();
        }
    }
}

