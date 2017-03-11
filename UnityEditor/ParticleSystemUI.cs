namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class ParticleSystemUI
    {
        [CompilerGenerated]
        private static Func<ParticleSystem, bool> <>f__am$cache0;
        public ModuleUI[] m_Modules;
        public ParticleEffectUI m_ParticleEffectUI;
        public ParticleSystem[] m_ParticleSystems;
        public SerializedObject m_ParticleSystemSerializedObject;
        public SerializedObject m_RendererSerializedObject;
        private string m_SupportsCullingText;
        private static string[] s_ModuleNames;
        private static Texts s_Texts;

        private void AddModuleCallback(object obj)
        {
            int index = (int) obj;
            if ((index >= 0) && (index < this.m_Modules.Length))
            {
                if (index == (this.m_Modules.Length - 1))
                {
                    this.InitRendererUI();
                }
                else
                {
                    this.m_Modules[index].enabled = true;
                    this.m_Modules[index].foldout = true;
                }
            }
            else
            {
                this.m_ParticleEffectUI.SetAllModulesVisible(!ParticleEffectUI.GetAllModulesVisible());
            }
            this.ApplyProperties();
        }

        public void ApplyProperties()
        {
            bool hasModifiedProperties = this.m_ParticleSystemSerializedObject.hasModifiedProperties;
            if (this.m_ParticleSystemSerializedObject.targetObject != null)
            {
                this.m_ParticleSystemSerializedObject.ApplyModifiedProperties();
            }
            if (hasModifiedProperties)
            {
                foreach (ParticleSystem system in this.m_ParticleSystems)
                {
                    if (!ParticleEffectUI.IsStopped(ParticleSystemEditorUtils.GetRoot(system)) && ParticleSystemEditorUtils.editorResimulation)
                    {
                        ParticleSystemEditorUtils.PerformCompleteResimulation();
                    }
                }
                this.UpdateParticleSystemInfoString();
            }
            if ((this.m_RendererSerializedObject != null) && (this.m_RendererSerializedObject.targetObject != null))
            {
                this.m_RendererSerializedObject.ApplyModifiedProperties();
            }
        }

        private void ClearRenderers()
        {
            this.m_RendererSerializedObject = null;
            foreach (ParticleSystem system in this.m_ParticleSystems)
            {
                ParticleSystemRenderer component = system.GetComponent<ParticleSystemRenderer>();
                if (component != null)
                {
                    Undo.DestroyObjectImmediate(component);
                }
            }
            this.m_Modules[this.m_Modules.Length - 1] = null;
        }

        private static ModuleUI[] CreateUIModules(ParticleSystemUI e, SerializedObject so)
        {
            int num = 0;
            ModuleUI[] euiArray1 = new ModuleUI[0x17];
            euiArray1[0] = new InitialModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[1] = new EmissionModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[2] = new ShapeModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[3] = new VelocityModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[4] = new ClampVelocityModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[5] = new InheritVelocityModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[6] = new ForceModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[7] = new ColorModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[8] = new ColorByVelocityModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[9] = new SizeModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[10] = new SizeByVelocityModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[11] = new RotationModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[12] = new RotationByVelocityModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[13] = new ExternalForcesModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[14] = new NoiseModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[15] = new CollisionModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[0x10] = new TriggerModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[0x11] = new SubModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[0x12] = new UVModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[0x13] = new LightsModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[20] = new TrailModuleUI(e, so, s_ModuleNames[num++]);
            euiArray1[0x15] = new CustomDataModuleUI(e, so, s_ModuleNames[num++]);
            return euiArray1;
        }

        private void EmitterMenuCallback(object obj)
        {
            switch (((int) obj))
            {
                case 0:
                    this.m_ParticleEffectUI.CreateParticleSystem(this.m_ParticleSystems[0], SubModuleUI.SubEmitterType.None);
                    break;

                case 1:
                    this.ResetModules();
                    break;

                case 2:
                    EditorGUIUtility.PingObject(this.m_ParticleSystems[0]);
                    break;
            }
        }

        public float GetEmitterDuration()
        {
            InitialModuleUI eui = this.m_Modules[0] as InitialModuleUI;
            if (eui != null)
            {
                return eui.m_LengthInSec.floatValue;
            }
            return -1f;
        }

        internal ModuleUI GetParticleSystemRendererModuleUI() => 
            this.m_Modules[this.m_Modules.Length - 1];

        private ParticleSystem GetSelectedParticleSystem() => 
            Selection.activeGameObject.GetComponent<ParticleSystem>();

        public static string[] GetUIModuleNames() => 
            new string[] { 
                "", "Emission", "Shape", "Velocity over Lifetime", "Limit Velocity over Lifetime", "Inherit Velocity", "Force over Lifetime", "Color over Lifetime", "Color by Speed", "Size over Lifetime", "Size by Speed", "Rotation over Lifetime", "Rotation by Speed", "External Forces", "Noise", "Collision",
                "Triggers", "Sub Emitters", "Texture Sheet Animation", "Lights", "Trails", "Custom Data", "Renderer"
            };

        public void Init(ParticleEffectUI owner, ParticleSystem[] systems)
        {
            if (s_ModuleNames == null)
            {
                s_ModuleNames = GetUIModuleNames();
            }
            this.m_ParticleEffectUI = owner;
            this.m_ParticleSystems = systems;
            this.m_ParticleSystemSerializedObject = new SerializedObject(this.m_ParticleSystems);
            this.m_RendererSerializedObject = null;
            this.m_SupportsCullingText = null;
            this.m_Modules = CreateUIModules(this, this.m_ParticleSystemSerializedObject);
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = o => o.GetComponent<ParticleSystemRenderer>() == null;
            }
            if (Enumerable.FirstOrDefault<ParticleSystem>(this.m_ParticleSystems, <>f__am$cache0) == null)
            {
                this.InitRendererUI();
            }
            this.UpdateParticleSystemInfoString();
        }

        private void InitRendererUI()
        {
            List<ParticleSystemRenderer> list = new List<ParticleSystemRenderer>();
            foreach (ParticleSystem system in this.m_ParticleSystems)
            {
                if (system.GetComponent<ParticleSystemRenderer>() == null)
                {
                    system.gameObject.AddComponent<ParticleSystemRenderer>();
                }
                list.Add(system.GetComponent<ParticleSystemRenderer>());
            }
            if (list.Count > 0)
            {
                this.m_RendererSerializedObject = new SerializedObject(list.ToArray());
                this.m_Modules[this.m_Modules.Length - 1] = new RendererModuleUI(this, this.m_RendererSerializedObject, s_ModuleNames[s_ModuleNames.Length - 1]);
                foreach (ParticleSystemRenderer renderer2 in list)
                {
                    EditorUtility.SetSelectedRenderState(renderer2, !ParticleEffectUI.m_ShowWireframe ? EditorSelectedRenderState.Hidden : EditorSelectedRenderState.Wireframe);
                }
            }
        }

        private void ModuleMenuCallback(object obj)
        {
            int index = (int) obj;
            if (index == (this.m_Modules.Length - 1))
            {
                this.ClearRenderers();
            }
            else
            {
                if (!ParticleEffectUI.GetAllModulesVisible())
                {
                    this.m_Modules[index].visibleUI = false;
                }
                this.m_Modules[index].enabled = false;
            }
        }

        public void OnGUI(float width, bool fixedWidth)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            bool flag = Event.current.type == EventType.Repaint;
            string name = null;
            if (this.m_ParticleSystems.Length > 1)
            {
                name = "Multiple Particle Systems";
            }
            else if (this.m_ParticleSystems.Length > 0)
            {
                name = this.m_ParticleSystems[0].gameObject.name;
            }
            if (fixedWidth)
            {
                EditorGUIUtility.labelWidth = width * 0.4f;
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(width) };
                EditorGUILayout.BeginVertical(options);
            }
            else
            {
                EditorGUIUtility.labelWidth = 0f;
                EditorGUIUtility.labelWidth -= 4f;
                EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
            }
            InitialModuleUI initial = (InitialModuleUI) this.m_Modules[0];
            for (int i = 0; i < this.m_Modules.Length; i++)
            {
                GUIStyle emitterHeaderStyle;
                Rect rect;
                bool flag5;
                ModuleUI eui2 = this.m_Modules[i];
                if (eui2 == null)
                {
                    continue;
                }
                bool flag2 = eui2 == this.m_Modules[0];
                if (!eui2.visibleUI && !flag2)
                {
                    continue;
                }
                GUIContent content = new GUIContent();
                if (flag2)
                {
                    rect = GUILayoutUtility.GetRect(width, 25f);
                    emitterHeaderStyle = ParticleSystemStyles.Get().emitterHeaderStyle;
                }
                else
                {
                    rect = GUILayoutUtility.GetRect(width, 15f);
                    emitterHeaderStyle = ParticleSystemStyles.Get().moduleHeaderStyle;
                }
                if (eui2.foldout)
                {
                    using (new EditorGUI.DisabledScope(!eui2.enabled))
                    {
                        Rect rect2 = EditorGUILayout.BeginVertical(ParticleSystemStyles.Get().modulePadding, new GUILayoutOption[0]);
                        rect2.y -= 4f;
                        rect2.height += 4f;
                        GUI.Label(rect2, GUIContent.none, ParticleSystemStyles.Get().moduleBgStyle);
                        eui2.OnInspectorGUI(initial);
                        EditorGUILayout.EndVertical();
                    }
                }
                if (flag2)
                {
                    ParticleSystemRenderer component = this.m_ParticleSystems[0].GetComponent<ParticleSystemRenderer>();
                    float num2 = 21f;
                    Rect rect3 = new Rect(rect.x + 4f, rect.y + 2f, num2, num2);
                    if (flag && (component != null))
                    {
                        bool flag3 = false;
                        int instanceID = 0;
                        if (!this.multiEdit)
                        {
                            if (component.renderMode == ParticleSystemRenderMode.Mesh)
                            {
                                if (component.mesh != null)
                                {
                                    instanceID = component.mesh.GetInstanceID();
                                }
                            }
                            else if (component.sharedMaterial != null)
                            {
                                instanceID = component.sharedMaterial.GetInstanceID();
                            }
                            if (EditorUtility.IsDirty(instanceID))
                            {
                                AssetPreview.ClearTemporaryAssetPreviews();
                            }
                            if (instanceID != 0)
                            {
                                Texture2D assetPreview = AssetPreview.GetAssetPreview(instanceID);
                                if (assetPreview != null)
                                {
                                    GUI.DrawTexture(rect3, assetPreview, ScaleMode.StretchToFill, true);
                                    flag3 = true;
                                }
                            }
                        }
                        if (!flag3)
                        {
                            GUI.Label(rect3, GUIContent.none, ParticleSystemStyles.Get().moduleBgStyle);
                        }
                    }
                    if (!this.multiEdit && EditorGUI.DropdownButton(rect3, GUIContent.none, FocusType.Passive, GUIStyle.none))
                    {
                        if (EditorGUI.actionKey)
                        {
                            List<int> list = new List<int>();
                            int item = this.m_ParticleSystems[0].gameObject.GetInstanceID();
                            list.AddRange(Selection.instanceIDs);
                            if (!list.Contains(item) || (list.Count != 1))
                            {
                                if (list.Contains(item))
                                {
                                    list.Remove(item);
                                }
                                else
                                {
                                    list.Add(item);
                                }
                            }
                            Selection.instanceIDs = list.ToArray();
                        }
                        else
                        {
                            Selection.instanceIDs = new int[0];
                            Selection.activeInstanceID = this.m_ParticleSystems[0].gameObject.GetInstanceID();
                        }
                    }
                }
                Rect position = new Rect(rect.x + 2f, rect.y + 1f, 13f, 13f);
                if (!flag2 && GUI.Button(position, GUIContent.none, GUIStyle.none))
                {
                    eui2.enabled = !eui2.enabled;
                }
                Rect rect5 = new Rect((rect.x + rect.width) - 10f, (rect.y + rect.height) - 10f, 10f, 10f);
                Rect rect6 = new Rect(rect5.x - 4f, rect5.y - 4f, rect5.width + 4f, rect5.height + 4f);
                Rect rect7 = new Rect(rect5.x - 23f, rect5.y - 3f, 16f, 16f);
                if (flag2 && EditorGUI.DropdownButton(rect6, s_Texts.addModules, FocusType.Passive, GUIStyle.none))
                {
                    this.ShowAddModuleMenu();
                }
                if (!string.IsNullOrEmpty(name))
                {
                    content.text = !flag2 ? eui2.displayName : name;
                }
                else
                {
                    content.text = eui2.displayName;
                }
                content.tooltip = eui2.toolTip;
                if (GUI.Toggle(rect, eui2.foldout, content, emitterHeaderStyle) != eui2.foldout)
                {
                    switch (Event.current.button)
                    {
                        case 0:
                            flag5 = !eui2.foldout;
                            if (!Event.current.control)
                            {
                                goto Label_05EF;
                            }
                            foreach (ModuleUI eui3 in this.m_Modules)
                            {
                                if ((eui3 != null) && eui3.visibleUI)
                                {
                                    eui3.foldout = flag5;
                                }
                            }
                            break;

                        case 1:
                            if (flag2)
                            {
                                this.ShowEmitterMenu();
                            }
                            else
                            {
                                this.ShowModuleMenu(i);
                            }
                            break;
                    }
                }
                goto Label_061E;
            Label_05EF:
                eui2.foldout = flag5;
            Label_061E:
                if (!flag2)
                {
                    EditorGUI.showMixedValue = eui2.enabledHasMultipleDifferentValues;
                    GUIStyle style = !EditorGUI.showMixedValue ? ParticleSystemStyles.Get().checkmark : ParticleSystemStyles.Get().checkmarkMixed;
                    GUI.Toggle(position, eui2.enabled, GUIContent.none, style);
                    EditorGUI.showMixedValue = false;
                }
                if (flag && flag2)
                {
                    GUI.Label(rect5, GUIContent.none, ParticleSystemStyles.Get().plus);
                }
                s_Texts.supportsCullingText.tooltip = this.m_SupportsCullingText;
                if (flag2 && (s_Texts.supportsCullingText.tooltip != null))
                {
                    GUI.Label(rect7, s_Texts.supportsCullingText);
                }
                GUILayout.Space(1f);
            }
            GUILayout.Space(-1f);
            EditorGUILayout.EndVertical();
            this.ApplyProperties();
        }

        public void OnSceneViewGUI()
        {
            if (this.m_Modules != null)
            {
                foreach (ParticleSystem system in this.m_ParticleSystems)
                {
                    if (system.particleCount > 0)
                    {
                        ParticleSystemRenderer component = system.GetComponent<ParticleSystemRenderer>();
                        if (ParticleEffectUI.m_ShowBounds)
                        {
                            Color color = Handles.color;
                            Handles.color = Color.yellow;
                            Bounds bounds = component.bounds;
                            Handles.DrawWireCube(bounds.center, bounds.size);
                            Handles.color = color;
                        }
                        EditorUtility.SetSelectedRenderState(component, !ParticleEffectUI.m_ShowWireframe ? EditorSelectedRenderState.Hidden : EditorSelectedRenderState.Wireframe);
                    }
                }
                this.UpdateProperties();
                foreach (ModuleUI eui in this.m_Modules)
                {
                    if ((((eui != null) && eui.visibleUI) && eui.enabled) && eui.foldout)
                    {
                        eui.OnSceneViewGUI();
                    }
                }
                this.ApplyProperties();
            }
        }

        private void ResetModules()
        {
            foreach (ModuleUI eui in this.m_Modules)
            {
                if (eui != null)
                {
                    eui.enabled = false;
                    if (!ParticleEffectUI.GetAllModulesVisible())
                    {
                        eui.visibleUI = false;
                    }
                }
            }
            if (this.m_Modules[this.m_Modules.Length - 1] == null)
            {
                this.InitRendererUI();
            }
            int[] numArray1 = new int[] { 1, 2, this.m_Modules.Length - 1 };
            foreach (int num3 in numArray1)
            {
                if (this.m_Modules[num3] != null)
                {
                    this.m_Modules[num3].enabled = true;
                    this.m_Modules[num3].visibleUI = true;
                }
            }
        }

        private void ShowAddModuleMenu()
        {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < s_ModuleNames.Length; i++)
            {
                if ((this.m_Modules[i] == null) || !this.m_Modules[i].visibleUI)
                {
                    menu.AddItem(new GUIContent(s_ModuleNames[i]), false, new GenericMenu.MenuFunction2(this.AddModuleCallback), i);
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(s_ModuleNames[i]));
                }
            }
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Show All Modules"), ParticleEffectUI.GetAllModulesVisible(), new GenericMenu.MenuFunction2(this.AddModuleCallback), 0x2710);
            menu.ShowAsContext();
            Event.current.Use();
        }

        private void ShowEmitterMenu()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Show Location"), false, new GenericMenu.MenuFunction2(this.EmitterMenuCallback), 2);
            menu.AddSeparator("");
            if (this.m_ParticleSystems[0].gameObject.activeInHierarchy)
            {
                menu.AddItem(new GUIContent("Create Particle System"), false, new GenericMenu.MenuFunction2(this.EmitterMenuCallback), 0);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Create new Particle System"));
            }
            menu.AddItem(new GUIContent("Reset"), false, new GenericMenu.MenuFunction2(this.EmitterMenuCallback), 1);
            menu.ShowAsContext();
            Event.current.Use();
        }

        private void ShowModuleMenu(int moduleIndex)
        {
            GenericMenu menu = new GenericMenu();
            if (!ParticleEffectUI.GetAllModulesVisible())
            {
                menu.AddItem(new GUIContent("Remove"), false, new GenericMenu.MenuFunction2(this.ModuleMenuCallback), moduleIndex);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Remove"));
            }
            menu.ShowAsContext();
            Event.current.Use();
        }

        private void UpdateParticleSystemInfoString()
        {
            string text = "";
            foreach (ModuleUI eui in this.m_Modules)
            {
                if (((eui != null) && eui.visibleUI) && eui.enabled)
                {
                    eui.UpdateCullingSupportedString(ref text);
                }
            }
            if (text != "")
            {
                this.m_SupportsCullingText = "Automatic culling is disabled because: " + text;
            }
            else
            {
                this.m_SupportsCullingText = null;
            }
        }

        public void UpdateProperties()
        {
            if (this.m_ParticleSystemSerializedObject.targetObject != null)
            {
                this.m_ParticleSystemSerializedObject.UpdateIfRequiredOrScript();
            }
            if ((this.m_RendererSerializedObject != null) && (this.m_RendererSerializedObject.targetObject != null))
            {
                this.m_RendererSerializedObject.UpdateIfRequiredOrScript();
            }
        }

        public bool multiEdit =>
            ((this.m_ParticleSystems != null) && (this.m_ParticleSystems.Length > 1));

        public enum DefaultTypes
        {
            Root,
            SubBirth,
            SubCollision,
            SubDeath
        }

        protected class Texts
        {
            public GUIContent addModules = new GUIContent("", "Show/Hide Modules");
            public GUIContent supportsCullingText = new GUIContent("", ParticleSystemStyles.Get().warningIcon);
        }
    }
}

