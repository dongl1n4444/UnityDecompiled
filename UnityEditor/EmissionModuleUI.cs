namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class EmissionModuleUI : ModuleUI
    {
        [CompilerGenerated]
        private static GenericMenu.MenuFunction2 <>f__mg$cache0;
        private const float k_BurstDragWidth = 15f;
        private const int k_MaxNumBursts = 8;
        private SerializedProperty m_BurstCount;
        private ReorderableList m_BurstList;
        private SerializedProperty m_Bursts;
        public SerializedMinMaxCurve m_Distance;
        public SerializedMinMaxCurve m_Time;
        private static Texts s_Texts;

        public EmissionModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "EmissionModule", displayName)
        {
            base.m_ToolTip = "Emission of the emitter. This controls the rate at which particles are emitted as well as burst emissions.";
        }

        private void DoBurstGUI(InitialModuleUI initial)
        {
            EditorGUILayout.Space();
            GUI.Label(ModuleUI.GetControlRect(13, new GUILayoutOption[0]), s_Texts.burst, ParticleSystemStyles.Get().label);
            this.m_BurstList.displayAdd = this.m_Bursts.arraySize < 8;
            this.m_BurstList.DoLayoutList();
        }

        private void DrawBurstListElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty arrayElementAtIndex = this.m_Bursts.GetArrayElementAtIndex(index);
            SerializedProperty floatProp = arrayElementAtIndex.FindPropertyRelative("time");
            SerializedProperty intProp = arrayElementAtIndex.FindPropertyRelative("minCount");
            SerializedProperty property4 = arrayElementAtIndex.FindPropertyRelative("maxCount");
            SerializedProperty property5 = arrayElementAtIndex.FindPropertyRelative("cycleCount");
            SerializedProperty property6 = arrayElementAtIndex.FindPropertyRelative("repeatInterval");
            rect.width /= 5f;
            ModuleUI.FloatDraggable(rect, floatProp, 1f, 15f, "n2");
            rect.x += rect.width;
            ModuleUI.IntDraggable(rect, null, intProp, 15f);
            rect.x += rect.width;
            ModuleUI.IntDraggable(rect, null, property4, 15f);
            rect.x += rect.width;
            rect.width -= 13f;
            if (property5.intValue == 0)
            {
                rect.x += 15f;
                rect.width -= 15f;
                EditorGUI.LabelField(rect, s_Texts.burstCycleCountInfinite, ParticleSystemStyles.Get().label);
            }
            else
            {
                ModuleUI.IntDraggable(rect, null, property5, 15f);
            }
            rect.width += 13f;
            GUIMMModePopUp(ModuleUI.GetPopupRect(rect), property5);
            rect.x += rect.width;
            ModuleUI.FloatDraggable(rect, property6, 1f, 15f, "n2");
            rect.x += rect.width;
        }

        private void DrawBurstListHeaderCallback(Rect rect)
        {
            rect.x += 35f;
            rect.width -= 20f;
            rect.width /= 5f;
            EditorGUI.LabelField(rect, s_Texts.burstTime, ParticleSystemStyles.Get().label);
            rect.x += rect.width;
            EditorGUI.LabelField(rect, s_Texts.burstMin, ParticleSystemStyles.Get().label);
            rect.x += rect.width;
            EditorGUI.LabelField(rect, s_Texts.burstMax, ParticleSystemStyles.Get().label);
            rect.x += rect.width;
            EditorGUI.LabelField(rect, s_Texts.burstCycleCount, ParticleSystemStyles.Get().label);
            rect.x += rect.width;
            EditorGUI.LabelField(rect, s_Texts.burstRepeatInterval, ParticleSystemStyles.Get().label);
            rect.x += rect.width;
        }

        private static void GUIMMModePopUp(Rect rect, SerializedProperty modeProp)
        {
            if (EditorGUI.DropdownButton(rect, GUIContent.none, FocusType.Passive, ParticleSystemStyles.Get().minMaxCurveStateDropDown))
            {
                GUIContent[] contentArray = new GUIContent[] { new GUIContent("Infinite"), new GUIContent("Count") };
                GenericMenu menu = new GenericMenu();
                for (int i = 0; i < contentArray.Length; i++)
                {
                    if (<>f__mg$cache0 == null)
                    {
                        <>f__mg$cache0 = new GenericMenu.MenuFunction2(EmissionModuleUI.SelectModeCallback);
                    }
                    menu.AddItem(contentArray[i], modeProp.intValue == i, <>f__mg$cache0, new ModeCallbackData(i, modeProp));
                }
                menu.ShowAsContext();
                Event.current.Use();
            }
        }

        protected override void Init()
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            if (this.m_BurstCount == null)
            {
                this.m_Time = new SerializedMinMaxCurve(this, s_Texts.rateOverTime, "rateOverTime");
                this.m_Time.m_AllowRandom = false;
                this.m_Distance = new SerializedMinMaxCurve(this, s_Texts.rateOverDistance, "rateOverDistance");
                this.m_Distance.m_AllowRandom = false;
                this.m_BurstCount = base.GetProperty("m_BurstCount");
                this.m_Bursts = base.GetProperty("m_Bursts");
                this.m_BurstList = new ReorderableList(base.serializedObject, this.m_Bursts, true, true, true, true);
                this.m_BurstList.elementHeight = 16f;
                this.m_BurstList.onAddCallback = new ReorderableList.AddCallbackDelegate(this.OnBurstListAddCallback);
                this.m_BurstList.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(this.OnBurstListRemoveCallback);
                this.m_BurstList.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(this.DrawBurstListHeaderCallback);
                this.m_BurstList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawBurstListElementCallback);
            }
        }

        private void OnBurstListAddCallback(ReorderableList list)
        {
            ReorderableList.defaultBehaviours.DoAddButton(list);
            this.m_BurstCount.intValue++;
            SerializedProperty arrayElementAtIndex = this.m_Bursts.GetArrayElementAtIndex(list.index);
            SerializedProperty property2 = arrayElementAtIndex.FindPropertyRelative("minCount");
            SerializedProperty property3 = arrayElementAtIndex.FindPropertyRelative("maxCount");
            SerializedProperty property4 = arrayElementAtIndex.FindPropertyRelative("cycleCount");
            property2.intValue = 30;
            property3.intValue = 30;
            property4.intValue = 1;
        }

        private void OnBurstListRemoveCallback(ReorderableList list)
        {
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
            this.m_BurstCount.intValue--;
        }

        public override void OnInspectorGUI(InitialModuleUI initial)
        {
            ModuleUI.GUIMinMaxCurve(s_Texts.rateOverTime, this.m_Time, new GUILayoutOption[0]);
            ModuleUI.GUIMinMaxCurve(s_Texts.rateOverDistance, this.m_Distance, new GUILayoutOption[0]);
            this.DoBurstGUI(initial);
            if ((initial.m_SimulationSpace.intValue != 1) && (this.m_Distance.scalar.floatValue > 0f))
            {
                EditorGUILayout.HelpBox("Distance-based emission only works when using World Space Simulation Space", MessageType.Warning, true);
            }
        }

        private static void SelectModeCallback(object obj)
        {
            ModeCallbackData data = (ModeCallbackData) obj;
            data.modeProp.intValue = data.selectedState;
        }

        public override void UpdateCullingSupportedString(ref string text)
        {
            this.Init();
            if (this.m_Distance.scalar.floatValue > 0f)
            {
                text = text + "\n\tEmission is distance based.";
            }
        }

        private class ModeCallbackData
        {
            public SerializedProperty modeProp;
            public int selectedState;

            public ModeCallbackData(int i, SerializedProperty p)
            {
                this.modeProp = p;
                this.selectedState = i;
            }
        }

        private class Texts
        {
            public GUIContent burst = EditorGUIUtility.TextContent("Bursts|Emission of extra particles at specific times during the duration of the system.");
            public GUIContent burstCycleCount = EditorGUIUtility.TextContent("Cycles|How many times to emit the burst. Use the dropdown to repeat infinitely.");
            public GUIContent burstCycleCountInfinite = EditorGUIUtility.TextContent("Infinite");
            public GUIContent burstMax = EditorGUIUtility.TextContent("Max|The maximum number of particles to emit.");
            public GUIContent burstMin = EditorGUIUtility.TextContent("Min|The minimum number of particles to emit.");
            public GUIContent burstRepeatInterval = EditorGUIUtility.TextContent("Interval|Repeat the burst every N seconds.");
            public GUIContent burstTime = EditorGUIUtility.TextContent("Time|When the burst will trigger.");
            public GUIContent rateOverDistance = EditorGUIUtility.TextContent("Rate over Distance|The number of particles emitted per distance unit.");
            public GUIContent rateOverTime = EditorGUIUtility.TextContent("Rate over Time|The number of particles emitted per second.");
        }
    }
}

