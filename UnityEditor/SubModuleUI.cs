namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    internal class SubModuleUI : ModuleUI
    {
        private int m_CheckObjectIndex;
        private SerializedProperty m_SubEmitters;
        private static Texts s_Texts;

        public SubModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "SubModule", displayName)
        {
            this.m_CheckObjectIndex = -1;
            base.m_ToolTip = "Sub emission of particles. This allows each particle to emit particles in another system.";
            this.Init();
        }

        private bool CheckIfChild(UnityEngine.Object subEmitter)
        {
            ParticleSystem root = base.m_ParticleSystemUI.m_ParticleEffectUI.GetRoot();
            if (!IsChild(subEmitter as ParticleSystem, root))
            {
                string message = $"The assigned sub emitter is not a child of the current root particle system GameObject: '{root.gameObject.name}' and is therefore NOT considered a part of the current effect. Do you want to reparent it?";
                if (!EditorUtility.DisplayDialog("Reparent GameObjects", message, "Yes, Reparent", "No, Remove"))
                {
                    return false;
                }
                if (EditorUtility.IsPersistent(subEmitter))
                {
                    GameObject obj2 = UnityEngine.Object.Instantiate(subEmitter) as GameObject;
                    if (obj2 != null)
                    {
                        obj2.transform.parent = base.m_ParticleSystemUI.m_ParticleSystem.transform;
                        obj2.transform.localPosition = Vector3.zero;
                        obj2.transform.localRotation = Quaternion.identity;
                    }
                }
                else
                {
                    ParticleSystem system2 = subEmitter as ParticleSystem;
                    if (system2 != null)
                    {
                        Undo.SetTransformParent(system2.gameObject.transform.transform, base.m_ParticleSystemUI.m_ParticleSystem.transform, "Reparent sub emitter");
                    }
                }
            }
            return true;
        }

        private void CreateSubEmitter(SerializedProperty objectRefProp, int index, SubEmitterType type)
        {
            GameObject obj2 = base.m_ParticleSystemUI.m_ParticleEffectUI.CreateParticleSystem(base.m_ParticleSystemUI.m_ParticleSystem, type);
            obj2.name = "SubEmitter" + index;
            objectRefProp.objectReferenceValue = obj2.GetComponent<ParticleSystem>();
        }

        private List<UnityEngine.Object> GetSubEmitterProperties()
        {
            List<UnityEngine.Object> list = new List<UnityEngine.Object>();
            IEnumerator enumerator = this.m_SubEmitters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SerializedProperty current = (SerializedProperty) enumerator.Current;
                list.Add(current.FindPropertyRelative("emitter").objectReferenceValue);
            }
            return list;
        }

        protected override void Init()
        {
            if (this.m_SubEmitters == null)
            {
                this.m_SubEmitters = base.GetProperty("subEmitters");
            }
        }

        internal static bool IsChild(ParticleSystem subEmitter, ParticleSystem root)
        {
            if ((subEmitter == null) || (root == null))
            {
                return false;
            }
            return (ParticleSystemEditorUtils.GetRoot(subEmitter) == root);
        }

        public override void OnInspectorGUI(ParticleSystem s)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            List<UnityEngine.Object> subEmitterProperties = this.GetSubEmitterProperties();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
            GUILayout.Label("", ParticleSystemStyles.Get().label, options);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(120f) };
            GUILayout.Label(s_Texts.inherit, ParticleSystemStyles.Get().label, optionArray2);
            GUILayout.EndHorizontal();
            for (int i = 0; i < this.m_SubEmitters.arraySize; i++)
            {
                this.ShowSubEmitter(i);
            }
            List<UnityEngine.Object> list2 = this.GetSubEmitterProperties();
            for (int j = 0; j < Mathf.Min(subEmitterProperties.Count, list2.Count); j++)
            {
                if (subEmitterProperties[j] != list2[j])
                {
                    if (this.m_CheckObjectIndex == -1)
                    {
                        EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
                    }
                    this.m_CheckObjectIndex = j;
                }
            }
        }

        private void ShowSubEmitter(int index)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            SerializedProperty arrayElementAtIndex = this.m_SubEmitters.GetArrayElementAtIndex(index);
            SerializedProperty objectProp = arrayElementAtIndex.FindPropertyRelative("emitter");
            SerializedProperty intProp = arrayElementAtIndex.FindPropertyRelative("type");
            SerializedProperty property4 = arrayElementAtIndex.FindPropertyRelative("properties");
            GUILayoutOption[] layoutOptions = new GUILayoutOption[] { GUILayout.MaxWidth(80f) };
            ModuleUI.GUIPopup(GUIContent.none, intProp, s_Texts.subEmitterTypeTexts, layoutOptions);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(4f) };
            GUILayout.Label("", ParticleSystemStyles.Get().label, options);
            ModuleUI.GUIObject(GUIContent.none, objectProp, new GUILayoutOption[0]);
            if (objectProp.objectReferenceValue == null)
            {
                GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(8f) };
                GUILayout.Label("", ParticleSystemStyles.Get().label, optionArray3);
                GUILayoutOption[] optionArray4 = new GUILayoutOption[] { GUILayout.Width(16f) };
                if (GUILayout.Button(GUIContent.none, ParticleSystemStyles.Get().plus, optionArray4))
                {
                    this.CreateSubEmitter(objectProp, index, (SubEmitterType) intProp.intValue);
                }
            }
            else
            {
                GUILayoutOption[] optionArray5 = new GUILayoutOption[] { GUILayout.Width(24f) };
                GUILayout.Label("", ParticleSystemStyles.Get().label, optionArray5);
            }
            GUILayoutOption[] optionArray6 = new GUILayoutOption[] { GUILayout.Width(100f) };
            property4.intValue = ModuleUI.GUIMask(GUIContent.none, property4.intValue, s_Texts.propertyStrings, optionArray6);
            GUILayoutOption[] optionArray7 = new GUILayoutOption[] { GUILayout.Width(8f) };
            GUILayout.Label("", ParticleSystemStyles.Get().label, optionArray7);
            if (index == 0)
            {
                GUILayoutOption[] optionArray8 = new GUILayoutOption[] { GUILayout.Width(16f) };
                if (GUILayout.Button(GUIContent.none, new GUIStyle("OL Plus"), optionArray8))
                {
                    this.m_SubEmitters.InsertArrayElementAtIndex(this.m_SubEmitters.arraySize);
                    this.m_SubEmitters.GetArrayElementAtIndex(this.m_SubEmitters.arraySize - 1).FindPropertyRelative("emitter").objectReferenceValue = null;
                }
            }
            else
            {
                GUILayoutOption[] optionArray9 = new GUILayoutOption[] { GUILayout.Width(16f) };
                if (GUILayout.Button(GUIContent.none, new GUIStyle("OL Minus"), optionArray9))
                {
                    this.m_SubEmitters.DeleteArrayElementAtIndex(index);
                }
            }
            GUILayout.EndHorizontal();
        }

        private void Update()
        {
            if ((this.m_CheckObjectIndex >= 0) && !ObjectSelector.isVisible)
            {
                SerializedProperty property2 = this.m_SubEmitters.GetArrayElementAtIndex(this.m_CheckObjectIndex).FindPropertyRelative("emitter");
                UnityEngine.Object objectReferenceValue = property2.objectReferenceValue;
                ParticleSystem subEmitter = objectReferenceValue as ParticleSystem;
                if (subEmitter != null)
                {
                    bool flag = true;
                    if (this.ValidateSubemitter(subEmitter))
                    {
                        string str = ParticleSystemEditorUtils.CheckCircularReferences(subEmitter);
                        if (str.Length == 0)
                        {
                            if (!this.CheckIfChild(objectReferenceValue))
                            {
                                flag = false;
                            }
                        }
                        else
                        {
                            string message = $"'{subEmitter.gameObject.name}' could not be assigned as subemitter on '{base.m_ParticleSystemUI.m_ParticleSystem.gameObject.name}' due to circular referencing!
Backtrace: {str} 

Reference will be removed.";
                            EditorUtility.DisplayDialog("Circular References Detected", message, "Ok");
                            flag = false;
                        }
                    }
                    else
                    {
                        flag = false;
                    }
                    if (!flag)
                    {
                        property2.objectReferenceValue = null;
                        base.m_ParticleSystemUI.ApplyProperties();
                        base.m_ParticleSystemUI.m_ParticleEffectUI.m_Owner.Repaint();
                    }
                }
                this.m_CheckObjectIndex = -1;
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
            }
        }

        public override void UpdateCullingSupportedString(ref string text)
        {
            text = text + "\n\tSub Emitters are enabled.";
        }

        private bool ValidateSubemitter(ParticleSystem subEmitter)
        {
            if (subEmitter == null)
            {
                return false;
            }
            ParticleSystem root = base.m_ParticleSystemUI.m_ParticleEffectUI.GetRoot();
            if (root.gameObject.activeInHierarchy && !subEmitter.gameObject.activeInHierarchy)
            {
                string message = string.Format("The assigned sub emitter is part of a prefab and can therefore not be assigned.", new object[0]);
                EditorUtility.DisplayDialog("Invalid Sub Emitter", message, "Ok");
                return false;
            }
            if (!root.gameObject.activeInHierarchy && subEmitter.gameObject.activeInHierarchy)
            {
                string str2 = string.Format("The assigned sub emitter is part of a scene object and can therefore not be assigned to a prefab.", new object[0]);
                EditorUtility.DisplayDialog("Invalid Sub Emitter", str2, "Ok");
                return false;
            }
            return true;
        }

        public enum SubEmitterType
        {
            Birth = 0,
            Collision = 1,
            Death = 2,
            None = -1,
            TypesMax = 3
        }

        private class Texts
        {
            public GUIContent create = EditorGUIUtility.TextContent("|Create and assign a Particle System as sub emitter");
            public GUIContent inherit = EditorGUIUtility.TextContent("Inherit");
            public string[] propertyStrings = new string[] { "Color", "Size", "Rotation" };
            public string[] subEmitterTypeTexts = new string[] { "Birth", "Collision", "Death" };
        }
    }
}

