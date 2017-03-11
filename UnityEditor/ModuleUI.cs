namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal abstract class ModuleUI : SerializedModule
    {
        [CompilerGenerated]
        private static GenericMenu.MenuFunction2 <>f__mg$cache0;
        [CompilerGenerated]
        private static GenericMenu.MenuFunction2 <>f__mg$cache1;
        [CompilerGenerated]
        private static GenericMenu.MenuFunction2 <>f__mg$cache2;
        public static float k_CompactFixedModuleWidth = 295f;
        protected const float k_minMaxToggleWidth = 13f;
        public static float k_SpaceBetweenModules = 5f;
        protected const float k_toggleWidth = 9f;
        protected const float kDragSpace = 20f;
        protected const string kFormatString = "g7";
        protected const int kPlusAddRemoveButtonSpacing = 5;
        protected const int kPlusAddRemoveButtonWidth = 12;
        protected const float kReorderableListElementHeight = 16f;
        protected static readonly Rect kSignedRange = new Rect(0f, -1f, 1f, 2f);
        protected const int kSingleLineHeight = 13;
        protected const int kSpacingSubLabel = 4;
        protected const int kSubLabelWidth = 10;
        protected static readonly Rect kUnsignedRange = new Rect(0f, 0f, 1f, 1f);
        protected static readonly bool kUseSignedRange = true;
        private List<SerializedProperty> m_CurvesRemovedWhenFolded;
        private string m_DisplayName;
        private SerializedProperty m_Enabled;
        public List<SerializedProperty> m_ModuleCurves;
        public ParticleSystemUI m_ParticleSystemUI;
        protected string m_ToolTip;
        private VisibilityState m_VisibilityState;
        public static readonly GUIStyle s_ControlRectStyle;

        static ModuleUI()
        {
            GUIStyle style = new GUIStyle {
                margin = new RectOffset(0, 0, 2, 2)
            };
            s_ControlRectStyle = style;
        }

        public ModuleUI(ParticleSystemUI owner, SerializedObject o, string name, string displayName) : base(o, name)
        {
            this.m_ToolTip = "";
            this.m_ModuleCurves = new List<SerializedProperty>();
            this.m_CurvesRemovedWhenFolded = new List<SerializedProperty>();
            this.Setup(owner, o, name, displayName, VisibilityState.NotVisible);
        }

        public ModuleUI(ParticleSystemUI owner, SerializedObject o, string name, string displayName, VisibilityState initialVisibilityState) : base(o, name)
        {
            this.m_ToolTip = "";
            this.m_ModuleCurves = new List<SerializedProperty>();
            this.m_CurvesRemovedWhenFolded = new List<SerializedProperty>();
            this.Setup(owner, o, name, displayName, initialVisibilityState);
        }

        public void AddToModuleCurves(SerializedProperty curveProp)
        {
            this.m_ModuleCurves.Add(curveProp);
            if (!this.foldout)
            {
                this.m_CurvesRemovedWhenFolded.Add(curveProp);
            }
        }

        internal void CheckVisibilityState()
        {
            if ((!(this is RendererModuleUI) && !this.m_Enabled.boolValue) && !ParticleEffectUI.GetAllModulesVisible())
            {
                this.SetVisibilityState(VisibilityState.NotVisible);
            }
            if (this.m_Enabled.boolValue && !this.visibleUI)
            {
                this.SetVisibilityState(VisibilityState.VisibleAndFolded);
            }
        }

        private static float FloatDraggable(Rect rect, SerializedProperty floatProp, float remap, float dragWidth) => 
            FloatDraggable(rect, floatProp, remap, dragWidth, "g7");

        public static float FloatDraggable(Rect rect, float floatValue, float remap, float dragWidth, string formatString)
        {
            int id = GUIUtility.GetControlID(0x62dd15e9, FocusType.Keyboard, rect);
            Rect dragHotZone = rect;
            dragHotZone.width = dragWidth;
            Rect position = rect;
            position.xMin += dragWidth;
            return (EditorGUI.DoFloatField(EditorGUI.s_RecycledEditor, position, dragHotZone, id, floatValue * remap, formatString, ParticleSystemStyles.Get().numberField, true) / remap);
        }

        public static float FloatDraggable(Rect rect, SerializedProperty floatProp, float remap, float dragWidth, string formatString)
        {
            EditorGUI.showMixedValue = floatProp.hasMultipleDifferentValues;
            Color color = GUI.color;
            if (floatProp.isAnimated)
            {
                GUI.color = UnityEditor.AnimationMode.animatedPropertyColor;
            }
            EditorGUI.BeginChangeCheck();
            float num = FloatDraggable(rect, floatProp.floatValue, remap, dragWidth, formatString);
            if (EditorGUI.EndChangeCheck())
            {
                floatProp.floatValue = num;
            }
            GUI.color = color;
            EditorGUI.showMixedValue = false;
            return num;
        }

        private static Color GetColor(SerializedMinMaxCurve mmCurve) => 
            mmCurve.m_Module.m_ParticleSystemUI.m_ParticleEffectUI.GetParticleSystemCurveEditor().GetCurveColor(mmCurve.maxCurve);

        protected static Rect GetControlRect(int height, params GUILayoutOption[] layoutOptions) => 
            GUILayoutUtility.GetRect(0f, (float) height, s_ControlRectStyle, layoutOptions);

        protected ParticleSystem GetParticleSystem() => 
            (this.m_Enabled.serializedObject.targetObject as ParticleSystem);

        public ParticleSystemCurveEditor GetParticleSystemCurveEditor() => 
            this.m_ParticleSystemUI.m_ParticleEffectUI.GetParticleSystemCurveEditor();

        protected static Rect GetPopupRect(Rect position)
        {
            position.xMin = position.xMax - 13f;
            return position;
        }

        public virtual float GetXAxisScalar() => 
            1f;

        public static bool GUIBoolAsPopup(GUIContent label, SerializedProperty boolProp, string[] options, params GUILayoutOption[] layoutOptions)
        {
            EditorGUI.showMixedValue = boolProp.hasMultipleDifferentValues;
            Rect position = PrefixLabel(GetControlRect(13, layoutOptions), label);
            EditorGUI.BeginChangeCheck();
            int num = EditorGUI.Popup(position, !boolProp.boolValue ? 0 : 1, options, ParticleSystemStyles.Get().popup);
            if (EditorGUI.EndChangeCheck())
            {
                boolProp.boolValue = num > 0;
            }
            EditorGUI.showMixedValue = false;
            return (num > 0);
        }

        public static void GUIButtonGroup(UnityEditorInternal.EditMode.SceneViewEditMode[] modes, GUIContent[] guiContents, Bounds bounds, Editor caller)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(EditorGUIUtility.labelWidth);
            UnityEditorInternal.EditMode.DoInspectorToolbar(modes, guiContents, bounds, caller);
            GUILayout.EndHorizontal();
        }

        private static void GUIColor(Rect rect, SerializedProperty colorProp)
        {
            GUIColor(rect, colorProp, false);
        }

        private static void GUIColor(Rect rect, SerializedProperty colorProp, bool hdr)
        {
            EditorGUI.BeginChangeCheck();
            Color color = EditorGUI.ColorField(rect, GUIContent.none, colorProp.colorValue, false, true, hdr, ColorPicker.defaultHDRConfig);
            if (EditorGUI.EndChangeCheck())
            {
                colorProp.colorValue = color;
            }
        }

        private static void GUICurveField(Rect position, SerializedProperty maxCurve, SerializedProperty minCurve, Color color, Rect ranges, CurveFieldMouseDownCallback mouseDownCallback)
        {
            int controlID = GUIUtility.GetControlID(0x4ec1c30f, FocusType.Keyboard, position);
            Event current = Event.current;
            EventType typeForControl = current.GetTypeForControl(controlID);
            if (typeForControl == EventType.Repaint)
            {
                Rect rect = position;
                if (minCurve == null)
                {
                    EditorGUIUtility.DrawCurveSwatch(rect, null, maxCurve, color, EditorGUI.kCurveBGColor, ranges);
                }
                else
                {
                    EditorGUIUtility.DrawRegionSwatch(rect, maxCurve, minCurve, color, EditorGUI.kCurveBGColor, ranges);
                }
                EditorStyles.colorPickerBox.Draw(rect, GUIContent.none, controlID, false);
            }
            else if (typeForControl == EventType.ValidateCommand)
            {
                if (current.commandName == "UndoRedoPerformed")
                {
                    AnimationCurvePreviewCache.ClearCache();
                }
            }
            else if ((typeForControl == EventType.MouseDown) && (position.Contains(current.mousePosition) && ((mouseDownCallback != null) && mouseDownCallback(current.button, position, ranges))))
            {
                current.Use();
            }
        }

        public static Enum GUIEnumMask(GUIContent label, Enum enumValue, params GUILayoutOption[] layoutOptions) => 
            EditorGUI.EnumMaskField(PrefixLabel(GetControlRect(13, layoutOptions), label), enumValue, ParticleSystemStyles.Get().popup);

        public static float GUIFloat(string label, SerializedProperty floatProp, params GUILayoutOption[] layoutOptions) => 
            GUIFloat(GUIContent.Temp(label), floatProp, layoutOptions);

        public static float GUIFloat(GUIContent guiContent, SerializedProperty floatProp, params GUILayoutOption[] layoutOptions) => 
            GUIFloat(guiContent, floatProp, "g7", layoutOptions);

        public static float GUIFloat(GUIContent guiContent, float floatValue, string formatString, params GUILayoutOption[] layoutOptions)
        {
            Rect controlRect = GetControlRect(13, layoutOptions);
            PrefixLabel(controlRect, guiContent);
            return FloatDraggable(controlRect, floatValue, 1f, EditorGUIUtility.labelWidth, formatString);
        }

        public static float GUIFloat(GUIContent guiContent, SerializedProperty floatProp, string formatString, params GUILayoutOption[] layoutOptions)
        {
            Rect controlRect = GetControlRect(13, layoutOptions);
            PrefixLabel(controlRect, guiContent);
            return FloatDraggable(controlRect, floatProp, 1f, EditorGUIUtility.labelWidth, formatString);
        }

        public static int GUIInt(GUIContent guiContent, SerializedProperty intProp, params GUILayoutOption[] layoutOptions)
        {
            EditorGUI.showMixedValue = intProp.hasMultipleDifferentValues;
            Color color = GUI.color;
            if (intProp.isAnimated)
            {
                GUI.color = UnityEditor.AnimationMode.animatedPropertyColor;
            }
            Rect totalPosition = GUILayoutUtility.GetRect((float) 0f, (float) 13f, layoutOptions);
            PrefixLabel(totalPosition, guiContent);
            EditorGUI.BeginChangeCheck();
            int num = IntDraggable(totalPosition, null, intProp.intValue, EditorGUIUtility.labelWidth);
            if (EditorGUI.EndChangeCheck())
            {
                intProp.intValue = num;
            }
            GUI.color = color;
            EditorGUI.showMixedValue = false;
            return intProp.intValue;
        }

        public static void GUIIntDraggableX2(GUIContent mainLabel, GUIContent label1, SerializedProperty intProp1, GUIContent label2, SerializedProperty intProp2, params GUILayoutOption[] layoutOptions)
        {
            Rect rect = PrefixLabel(GetControlRect(13, layoutOptions), mainLabel);
            float width = (rect.width - 4f) * 0.5f;
            Rect rect2 = new Rect(rect.x, rect.y, width, rect.height);
            IntDraggable(rect2, label1, intProp1, 10f);
            rect2.x += width + 4f;
            IntDraggable(rect2, label2, intProp2, 10f);
        }

        public static void GUILayerMask(GUIContent guiContent, SerializedProperty boolProp, params GUILayoutOption[] layoutOptions)
        {
            EditorGUI.showMixedValue = boolProp.hasMultipleDifferentValues;
            EditorGUI.LayerMaskField(PrefixLabel(GetControlRect(13, layoutOptions), guiContent), boolProp, GUIContent.none, ParticleSystemStyles.Get().popup);
            EditorGUI.showMixedValue = false;
        }

        public int GUIListOfFloatObjectToggleFields(GUIContent label, SerializedProperty[] objectProps, EditorGUI.ObjectFieldValidator validator, GUIContent buttonTooltip, bool allowCreation, params GUILayoutOption[] layoutOptions)
        {
            int num = -1;
            int length = objectProps.Length;
            Rect totalPosition = GUILayoutUtility.GetRect((float) 0f, (float) (15 * length), layoutOptions);
            totalPosition.height = 13f;
            float num3 = 10f;
            float num4 = 35f;
            float num5 = 10f;
            float width = (((totalPosition.width - num3) - num4) - (num5 * 2f)) - 9f;
            PrefixLabel(totalPosition, label);
            for (int i = 0; i < length; i++)
            {
                SerializedProperty property = objectProps[i];
                EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
                Rect position = new Rect(((totalPosition.x + num3) + num4) + num5, totalPosition.y, width, totalPosition.height);
                int id = GUIUtility.GetControlID(0x12da2a, FocusType.Keyboard, position);
                EditorGUI.DoObjectField(position, position, id, null, null, property, validator, true, ParticleSystemStyles.Get().objectField);
                EditorGUI.showMixedValue = false;
                if (property.objectReferenceValue == null)
                {
                    position = new Rect(totalPosition.xMax - 9f, totalPosition.y + 3f, 9f, 9f);
                    if (allowCreation)
                    {
                        if (buttonTooltip == null)
                        {
                        }
                        if (!GUI.Button(position, GUIContent.none, ParticleSystemStyles.Get().plus))
                        {
                            goto Label_013D;
                        }
                    }
                    num = i;
                }
            Label_013D:
                totalPosition.y += 15f;
            }
            return num;
        }

        public static void GUIMask(GUIContent label, SerializedProperty intProp, string[] options, params GUILayoutOption[] layoutOptions)
        {
            EditorGUI.showMixedValue = intProp.hasMultipleDifferentValues;
            Rect position = PrefixLabel(GetControlRect(13, layoutOptions), label);
            EditorGUI.BeginChangeCheck();
            int num = EditorGUI.MaskField(position, label, intProp.intValue, options, ParticleSystemStyles.Get().popup);
            if (EditorGUI.EndChangeCheck())
            {
                intProp.intValue = num;
            }
            EditorGUI.showMixedValue = false;
        }

        public static void GUIMinMaxCurve(string label, SerializedMinMaxCurve mmCurve, params GUILayoutOption[] layoutOptions)
        {
            GUIMinMaxCurve(GUIContent.Temp(label), mmCurve, layoutOptions);
        }

        public static void GUIMinMaxCurve(GUIContent label, SerializedMinMaxCurve mmCurve, params GUILayoutOption[] layoutOptions)
        {
            bool stateHasMultipleDifferentValues = mmCurve.stateHasMultipleDifferentValues;
            Rect controlRect = GetControlRect(13, layoutOptions);
            Rect popupRect = GetPopupRect(controlRect);
            controlRect = SubtractPopupWidth(controlRect);
            Rect rect = PrefixLabel(controlRect, label);
            if (stateHasMultipleDifferentValues)
            {
                Label(rect, GUIContent.Temp("-"));
            }
            else
            {
                MinMaxCurveState state = mmCurve.state;
                switch (state)
                {
                    case MinMaxCurveState.k_Scalar:
                    {
                        EditorGUI.BeginChangeCheck();
                        float a = FloatDraggable(controlRect, mmCurve.scalar, mmCurve.m_RemapValue, EditorGUIUtility.labelWidth);
                        if (EditorGUI.EndChangeCheck() && !mmCurve.signedRange)
                        {
                            mmCurve.scalar.floatValue = Mathf.Max(a, 0f);
                        }
                        goto Label_01D2;
                    }
                    case MinMaxCurveState.k_TwoScalars:
                    {
                        Rect rect4 = rect;
                        rect4.width = (rect.width - 20f) * 0.5f;
                        float minConstant = mmCurve.minConstant;
                        float maxConstant = mmCurve.maxConstant;
                        Rect rect5 = rect4;
                        rect5.xMin -= 20f;
                        EditorGUI.BeginChangeCheck();
                        minConstant = FloatDraggable(rect5, minConstant, mmCurve.m_RemapValue, 20f, "g5");
                        if (EditorGUI.EndChangeCheck())
                        {
                            mmCurve.minConstant = minConstant;
                        }
                        rect5.x += rect4.width + 20f;
                        EditorGUI.BeginChangeCheck();
                        maxConstant = FloatDraggable(rect5, maxConstant, mmCurve.m_RemapValue, 20f, "g5");
                        if (EditorGUI.EndChangeCheck())
                        {
                            mmCurve.maxConstant = maxConstant;
                        }
                        goto Label_01D2;
                    }
                }
                Rect ranges = !mmCurve.signedRange ? kUnsignedRange : kSignedRange;
                SerializedProperty minCurve = (state != MinMaxCurveState.k_TwoCurves) ? null : mmCurve.minCurve;
                GUICurveField(rect, mmCurve.maxCurve, minCurve, GetColor(mmCurve), ranges, new CurveFieldMouseDownCallback(mmCurve.OnCurveAreaMouseDown));
            }
        Label_01D2:
            GUIMMCurveStateList(popupRect, mmCurve);
        }

        public void GUIMinMaxGradient(GUIContent label, SerializedMinMaxGradient minMaxGradient, bool hdr, params GUILayoutOption[] layoutOptions)
        {
            bool stateHasMultipleDifferentValues = minMaxGradient.stateHasMultipleDifferentValues;
            MinMaxGradientState state = minMaxGradient.state;
            bool flag2 = !stateHasMultipleDifferentValues && ((state == MinMaxGradientState.k_RandomBetweenTwoColors) || (state == MinMaxGradientState.k_RandomBetweenTwoGradients));
            Rect position = GUILayoutUtility.GetRect((float) 0f, !flag2 ? ((float) 13) : ((float) 0x1a), layoutOptions);
            Rect popupRect = GetPopupRect(position);
            Rect rect = PrefixLabel(SubtractPopupWidth(position), label);
            rect.height = 13f;
            if (stateHasMultipleDifferentValues)
            {
                Label(rect, GUIContent.Temp("-"));
            }
            else
            {
                switch (state)
                {
                    case MinMaxGradientState.k_Color:
                        EditorGUI.showMixedValue = minMaxGradient.m_MaxColor.hasMultipleDifferentValues;
                        GUIColor(rect, minMaxGradient.m_MaxColor, hdr);
                        EditorGUI.showMixedValue = false;
                        break;

                    case MinMaxGradientState.k_Gradient:
                    case MinMaxGradientState.k_RandomColor:
                        EditorGUI.showMixedValue = minMaxGradient.m_MaxGradient.hasMultipleDifferentValues;
                        EditorGUI.GradientField(rect, minMaxGradient.m_MaxGradient, hdr);
                        EditorGUI.showMixedValue = false;
                        break;

                    case MinMaxGradientState.k_RandomBetweenTwoColors:
                        EditorGUI.showMixedValue = minMaxGradient.m_MaxColor.hasMultipleDifferentValues;
                        GUIColor(rect, minMaxGradient.m_MaxColor, hdr);
                        EditorGUI.showMixedValue = false;
                        rect.y += rect.height;
                        EditorGUI.showMixedValue = minMaxGradient.m_MinColor.hasMultipleDifferentValues;
                        GUIColor(rect, minMaxGradient.m_MinColor, hdr);
                        EditorGUI.showMixedValue = false;
                        break;

                    case MinMaxGradientState.k_RandomBetweenTwoGradients:
                        EditorGUI.showMixedValue = minMaxGradient.m_MaxGradient.hasMultipleDifferentValues;
                        EditorGUI.GradientField(rect, minMaxGradient.m_MaxGradient, hdr);
                        EditorGUI.showMixedValue = false;
                        rect.y += rect.height;
                        EditorGUI.showMixedValue = minMaxGradient.m_MinGradient.hasMultipleDifferentValues;
                        EditorGUI.GradientField(rect, minMaxGradient.m_MinGradient, hdr);
                        EditorGUI.showMixedValue = false;
                        break;
                }
            }
            GUIMMGradientPopUp(popupRect, minMaxGradient);
        }

        public static void GUIMinMaxRange(GUIContent label, SerializedProperty vec2Prop, params GUILayoutOption[] layoutOptions)
        {
            EditorGUI.showMixedValue = vec2Prop.hasMultipleDifferentValues;
            Rect rect = PrefixLabel(SubtractPopupWidth(GetControlRect(13, layoutOptions)), label);
            float num = (rect.width - 20f) * 0.5f;
            Vector2 vector = vec2Prop.vector2Value;
            EditorGUI.BeginChangeCheck();
            rect.width = num;
            rect.xMin -= 20f;
            vector.x = FloatDraggable(rect, vector.x, 1f, 20f, "g7");
            vector.x = Mathf.Clamp(vector.x, 0f, vector.y - 0.01f);
            rect.x += num + 20f;
            vector.y = FloatDraggable(rect, vector.y, 1f, 20f, "g7");
            vector.y = Mathf.Max(vector.x + 0.01f, vector.y);
            if (EditorGUI.EndChangeCheck())
            {
                vec2Prop.vector2Value = vector;
            }
            EditorGUI.showMixedValue = false;
        }

        public static void GUIMinMaxSlider(GUIContent label, SerializedProperty vec2Prop, float a, float b, params GUILayoutOption[] layoutOptions)
        {
            EditorGUI.showMixedValue = vec2Prop.hasMultipleDifferentValues;
            Rect controlRect = GetControlRect(0x1a, layoutOptions);
            controlRect.height = 13f;
            controlRect.y += 3f;
            PrefixLabel(controlRect, label);
            Vector2 vector = vec2Prop.vector2Value;
            controlRect.y += 13f;
            EditorGUI.BeginChangeCheck();
            EditorGUI.MinMaxSlider(controlRect, ref vector.x, ref vector.y, a, b);
            if (EditorGUI.EndChangeCheck())
            {
                vec2Prop.vector2Value = vector;
            }
            EditorGUI.showMixedValue = false;
        }

        public static void GUIMMColorPopUp(Rect rect, SerializedProperty boolProp)
        {
            if (EditorGUI.DropdownButton(rect, GUIContent.none, FocusType.Passive, ParticleSystemStyles.Get().minMaxCurveStateDropDown))
            {
                GenericMenu menu = new GenericMenu();
                GUIContent[] contentArray = new GUIContent[] { new GUIContent("Constant Color"), new GUIContent("Random Between Two Colors") };
                bool[] flagArray1 = new bool[2];
                flagArray1[1] = true;
                bool[] flagArray = flagArray1;
                for (int i = 0; i < contentArray.Length; i++)
                {
                    if (<>f__mg$cache2 == null)
                    {
                        <>f__mg$cache2 = new GenericMenu.MenuFunction2(ModuleUI.SelectMinMaxColorStateCallback);
                    }
                    menu.AddItem(contentArray[i], boolProp.boolValue == flagArray[i], <>f__mg$cache2, new ColorCallbackData(flagArray[i], boolProp));
                }
                menu.ShowAsContext();
                Event.current.Use();
            }
        }

        public static void GUIMMCurveStateList(Rect rect, SerializedMinMaxCurve minMaxCurves)
        {
            SerializedMinMaxCurve[] curveArray = new SerializedMinMaxCurve[] { minMaxCurves };
            GUIMMCurveStateList(rect, curveArray);
        }

        public static void GUIMMCurveStateList(Rect rect, SerializedMinMaxCurve[] minMaxCurves)
        {
            if (EditorGUI.DropdownButton(rect, GUIContent.none, FocusType.Passive, ParticleSystemStyles.Get().minMaxCurveStateDropDown) && (minMaxCurves.Length != 0))
            {
                GUIContent[] contentArray = new GUIContent[] { new GUIContent("Constant"), new GUIContent("Curve"), new GUIContent("Random Between Two Constants"), new GUIContent("Random Between Two Curves") };
                MinMaxCurveState[] stateArray = new MinMaxCurveState[] { MinMaxCurveState.k_Scalar };
                bool[] flagArray = new bool[] { minMaxCurves[0].m_AllowConstant, minMaxCurves[0].m_AllowCurves, minMaxCurves[0].m_AllowRandom, minMaxCurves[0].m_AllowRandom && minMaxCurves[0].m_AllowCurves };
                bool flag = !minMaxCurves[0].stateHasMultipleDifferentValues;
                GenericMenu menu = new GenericMenu();
                for (int i = 0; i < contentArray.Length; i++)
                {
                    if (flagArray[i])
                    {
                        if (<>f__mg$cache0 == null)
                        {
                            <>f__mg$cache0 = new GenericMenu.MenuFunction2(ModuleUI.SelectMinMaxCurveStateCallback);
                        }
                        menu.AddItem(contentArray[i], flag && (minMaxCurves[0].state == stateArray[i]), <>f__mg$cache0, new CurveStateCallbackData(stateArray[i], minMaxCurves));
                    }
                }
                menu.DropDown(rect);
                Event.current.Use();
            }
        }

        public static void GUIMMGradientPopUp(Rect rect, SerializedMinMaxGradient gradientProp)
        {
            if (EditorGUI.DropdownButton(rect, GUIContent.none, FocusType.Passive, ParticleSystemStyles.Get().minMaxCurveStateDropDown))
            {
                GUIContent[] contentArray = new GUIContent[] { new GUIContent("Color"), new GUIContent("Gradient"), new GUIContent("Random Between Two Colors"), new GUIContent("Random Between Two Gradients"), new GUIContent("Random Color") };
                MinMaxGradientState[] stateArray = new MinMaxGradientState[] { MinMaxGradientState.k_Color };
                bool[] flagArray = new bool[] { gradientProp.m_AllowColor, gradientProp.m_AllowGradient, gradientProp.m_AllowRandomBetweenTwoColors, gradientProp.m_AllowRandomBetweenTwoGradients, gradientProp.m_AllowRandomColor };
                bool flag = !gradientProp.stateHasMultipleDifferentValues;
                GenericMenu menu = new GenericMenu();
                for (int i = 0; i < contentArray.Length; i++)
                {
                    if (flagArray[i])
                    {
                        if (<>f__mg$cache1 == null)
                        {
                            <>f__mg$cache1 = new GenericMenu.MenuFunction2(ModuleUI.SelectMinMaxGradientStateCallback);
                        }
                        menu.AddItem(contentArray[i], flag && (gradientProp.state == stateArray[i]), <>f__mg$cache1, new GradientCallbackData(stateArray[i], gradientProp));
                    }
                }
                menu.ShowAsContext();
                Event.current.Use();
            }
        }

        public static void GUIObject(GUIContent label, SerializedProperty objectProp, params GUILayoutOption[] layoutOptions)
        {
            EditorGUI.showMixedValue = objectProp.hasMultipleDifferentValues;
            EditorGUI.ObjectField(PrefixLabel(GetControlRect(13, layoutOptions), label), objectProp, null, GUIContent.none, ParticleSystemStyles.Get().objectField);
            EditorGUI.showMixedValue = false;
        }

        public static void GUIObjectFieldAndToggle(GUIContent label, SerializedProperty objectProp, SerializedProperty boolProp, params GUILayoutOption[] layoutOptions)
        {
            Rect position = PrefixLabel(GetControlRect(13, layoutOptions), label);
            EditorGUI.showMixedValue = objectProp.hasMultipleDifferentValues;
            position.xMax -= 19f;
            EditorGUI.ObjectField(position, objectProp, GUIContent.none);
            EditorGUI.showMixedValue = false;
            if (boolProp != null)
            {
                position.x += position.width + 10f;
                position.width = 9f;
                Toggle(position, boolProp);
            }
        }

        public static int GUIPopup(string name, SerializedProperty intProp, string[] options, params GUILayoutOption[] layoutOptions) => 
            GUIPopup(GUIContent.Temp(name), intProp, options, layoutOptions);

        public static int GUIPopup(GUIContent label, int intValue, string[] options, params GUILayoutOption[] layoutOptions) => 
            EditorGUI.Popup(PrefixLabel(GetControlRect(13, layoutOptions), label), intValue, options, ParticleSystemStyles.Get().popup);

        public static int GUIPopup(GUIContent label, SerializedProperty intProp, string[] options, params GUILayoutOption[] layoutOptions)
        {
            EditorGUI.showMixedValue = intProp.hasMultipleDifferentValues;
            Rect position = PrefixLabel(GetControlRect(13, layoutOptions), label);
            EditorGUI.BeginChangeCheck();
            int num = EditorGUI.Popup(position, intProp.intValue, options, ParticleSystemStyles.Get().popup);
            if (EditorGUI.EndChangeCheck())
            {
                intProp.intValue = num;
            }
            EditorGUI.showMixedValue = false;
            return intProp.intValue;
        }

        public static void GUISlider(SerializedProperty floatProp, float a, float b, float remap)
        {
            GUISlider("", floatProp, a, b, remap);
        }

        public static void GUISlider(string name, SerializedProperty floatProp, float a, float b, float remap)
        {
            EditorGUI.showMixedValue = floatProp.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(300f) };
            float num = EditorGUILayout.Slider(name, floatProp.floatValue * remap, a, b, options) / remap;
            if (EditorGUI.EndChangeCheck())
            {
                floatProp.floatValue = num;
            }
            EditorGUI.showMixedValue = false;
        }

        public static bool GUIToggle(string label, SerializedProperty boolProp, params GUILayoutOption[] layoutOptions) => 
            GUIToggle(GUIContent.Temp(label), boolProp, layoutOptions);

        public static bool GUIToggle(GUIContent guiContent, bool boolValue, params GUILayoutOption[] layoutOptions)
        {
            boolValue = EditorGUI.Toggle(PrefixLabel(GetControlRect(13, layoutOptions), guiContent), boolValue, ParticleSystemStyles.Get().toggle);
            return boolValue;
        }

        public static bool GUIToggle(GUIContent guiContent, SerializedProperty boolProp, params GUILayoutOption[] layoutOptions) => 
            Toggle(PrefixLabel(GetControlRect(13, layoutOptions), guiContent), boolProp);

        public static void GUIToggleWithFloatField(string name, SerializedProperty boolProp, SerializedProperty floatProp, bool invertToggle, params GUILayoutOption[] layoutOptions)
        {
            GUIToggleWithFloatField(EditorGUIUtility.TempContent(name), boolProp, floatProp, invertToggle, layoutOptions);
        }

        public static void GUIToggleWithFloatField(GUIContent guiContent, SerializedProperty boolProp, SerializedProperty floatProp, bool invertToggle, params GUILayoutOption[] layoutOptions)
        {
            Rect rect = PrefixLabel(GUILayoutUtility.GetRect((float) 0f, (float) 13f, layoutOptions), guiContent);
            Rect rect2 = rect;
            rect2.xMax = rect2.x + 9f;
            bool flag = Toggle(rect2, boolProp);
            if (!invertToggle ? flag : !flag)
            {
                float dragWidth = 25f;
                Rect rect3 = new Rect((rect.x + EditorGUIUtility.labelWidth) + 9f, rect.y, rect.width - 9f, rect.height);
                FloatDraggable(rect3, floatProp, 1f, dragWidth);
            }
        }

        public static void GUIToggleWithIntField(string name, SerializedProperty boolProp, SerializedProperty floatProp, bool invertToggle, params GUILayoutOption[] layoutOptions)
        {
            GUIToggleWithIntField(EditorGUIUtility.TempContent(name), boolProp, floatProp, invertToggle, layoutOptions);
        }

        public static void GUIToggleWithIntField(GUIContent guiContent, SerializedProperty boolProp, SerializedProperty intProp, bool invertToggle, params GUILayoutOption[] layoutOptions)
        {
            Rect controlRect = GetControlRect(13, layoutOptions);
            Rect rect = PrefixLabel(controlRect, guiContent);
            rect.xMax = rect.x + 9f;
            bool flag = Toggle(rect, boolProp);
            if (!invertToggle ? flag : !flag)
            {
                EditorGUI.showMixedValue = intProp.hasMultipleDifferentValues;
                float dragWidth = 25f;
                Rect rect4 = new Rect(rect.xMax, controlRect.y, (controlRect.width - rect.xMax) + 9f, controlRect.height);
                EditorGUI.BeginChangeCheck();
                int num2 = IntDraggable(rect4, null, intProp.intValue, dragWidth);
                if (EditorGUI.EndChangeCheck())
                {
                    intProp.intValue = num2;
                }
                EditorGUI.showMixedValue = false;
            }
        }

        public void GUITripleMinMaxCurve(GUIContent label, GUIContent x, SerializedMinMaxCurve xCurve, GUIContent y, SerializedMinMaxCurve yCurve, GUIContent z, SerializedMinMaxCurve zCurve, SerializedProperty randomizePerFrame, params GUILayoutOption[] layoutOptions)
        {
            bool stateHasMultipleDifferentValues = xCurve.stateHasMultipleDifferentValues;
            MinMaxCurveState state = xCurve.state;
            bool flag2 = label != GUIContent.none;
            int num = !flag2 ? 1 : 2;
            if (state == MinMaxCurveState.k_TwoScalars)
            {
                num++;
            }
            Rect controlRect = GetControlRect(13 * num, layoutOptions);
            Rect popupRect = GetPopupRect(controlRect);
            controlRect = SubtractPopupWidth(controlRect);
            Rect rect = controlRect;
            float num2 = (controlRect.width - 8f) / 3f;
            if (num > 1)
            {
                rect.height = 13f;
            }
            if (flag2)
            {
                PrefixLabel(controlRect, label);
                rect.y += rect.height;
            }
            rect.width = num2;
            GUIContent[] contentArray = new GUIContent[] { x, y, z };
            SerializedMinMaxCurve[] minMaxCurves = new SerializedMinMaxCurve[] { xCurve, yCurve, zCurve };
            if (stateHasMultipleDifferentValues)
            {
                Label(rect, GUIContent.Temp("-"));
            }
            else
            {
                switch (state)
                {
                    case MinMaxCurveState.k_Scalar:
                        for (int j = 0; j < minMaxCurves.Length; j++)
                        {
                            Label(rect, contentArray[j]);
                            EditorGUI.BeginChangeCheck();
                            float a = FloatDraggable(rect, minMaxCurves[j].scalar, minMaxCurves[j].m_RemapValue, 10f);
                            if (EditorGUI.EndChangeCheck() && !minMaxCurves[j].signedRange)
                            {
                                minMaxCurves[j].scalar.floatValue = Mathf.Max(a, 0f);
                            }
                            rect.x += num2 + 4f;
                        }
                        goto Label_0365;

                    case MinMaxCurveState.k_TwoScalars:
                        for (int k = 0; k < minMaxCurves.Length; k++)
                        {
                            Label(rect, contentArray[k]);
                            float minConstant = minMaxCurves[k].minConstant;
                            float maxConstant = minMaxCurves[k].maxConstant;
                            EditorGUI.BeginChangeCheck();
                            maxConstant = FloatDraggable(rect, maxConstant, minMaxCurves[k].m_RemapValue, 10f, "g5");
                            if (EditorGUI.EndChangeCheck())
                            {
                                minMaxCurves[k].maxConstant = maxConstant;
                            }
                            rect.y += 13f;
                            EditorGUI.BeginChangeCheck();
                            minConstant = FloatDraggable(rect, minConstant, minMaxCurves[k].m_RemapValue, 10f, "g5");
                            if (EditorGUI.EndChangeCheck())
                            {
                                minMaxCurves[k].minConstant = minConstant;
                            }
                            rect.x += num2 + 4f;
                            rect.y -= 13f;
                        }
                        goto Label_0365;
                }
                rect.width = num2;
                Rect ranges = !xCurve.signedRange ? kUnsignedRange : kSignedRange;
                for (int i = 0; i < minMaxCurves.Length; i++)
                {
                    Label(rect, contentArray[i]);
                    Rect position = rect;
                    position.xMin += 10f;
                    SerializedProperty minCurve = (state != MinMaxCurveState.k_TwoCurves) ? null : minMaxCurves[i].minCurve;
                    GUICurveField(position, minMaxCurves[i].maxCurve, minCurve, GetColor(minMaxCurves[i]), ranges, new CurveFieldMouseDownCallback(minMaxCurves[i].OnCurveAreaMouseDown));
                    rect.x += num2 + 4f;
                }
            }
        Label_0365:
            GUIMMCurveStateList(popupRect, minMaxCurves);
        }

        public static Vector3 GUIVector3Field(GUIContent guiContent, SerializedProperty vecProp, params GUILayoutOption[] layoutOptions)
        {
            EditorGUI.showMixedValue = vecProp.hasMultipleDifferentValues;
            Rect rect = PrefixLabel(GetControlRect(13, layoutOptions), guiContent);
            GUIContent[] contentArray = new GUIContent[] { new GUIContent("X"), new GUIContent("Y"), new GUIContent("Z") };
            float num = (rect.width - 8f) / 3f;
            rect.width = num;
            Vector3 vector = vecProp.vector3Value;
            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < 3; i++)
            {
                Label(rect, contentArray[i]);
                vector[i] = FloatDraggable(rect, vector[i], 1f, 25f, "g5");
                rect.x += num + 4f;
            }
            if (EditorGUI.EndChangeCheck())
            {
                vecProp.vector3Value = vector;
            }
            EditorGUI.showMixedValue = false;
            return vector;
        }

        protected abstract void Init();
        public static int IntDraggable(Rect rect, GUIContent label, int value, float dragWidth)
        {
            float width = rect.width;
            Rect position = rect;
            position.width = width;
            int id = GUIUtility.GetControlID(0xfd15f8, FocusType.Keyboard, position);
            Rect rect3 = position;
            rect3.width = dragWidth;
            if ((label != null) && !string.IsNullOrEmpty(label.text))
            {
                Label(rect3, label);
            }
            Rect rect4 = position;
            rect4.x += dragWidth;
            rect4.width = width - dragWidth;
            float dragSensitivity = Mathf.Max((float) 1f, (float) (Mathf.Pow(Mathf.Abs((float) value), 0.5f) * 0.03f));
            return (int) EditorGUI.DoFloatField(EditorGUI.s_RecycledEditor, rect4, rect3, id, (float) value, EditorGUI.kIntFieldFormatString, ParticleSystemStyles.Get().numberField, true, dragSensitivity);
        }

        public static int IntDraggable(Rect rect, GUIContent label, SerializedProperty intProp, float dragWidth)
        {
            EditorGUI.showMixedValue = intProp.hasMultipleDifferentValues;
            Color color = GUI.color;
            if (intProp.isAnimated)
            {
                GUI.color = UnityEditor.AnimationMode.animatedPropertyColor;
            }
            EditorGUI.BeginChangeCheck();
            int num = IntDraggable(rect, label, intProp.intValue, dragWidth);
            if (EditorGUI.EndChangeCheck())
            {
                intProp.intValue = num;
            }
            GUI.color = color;
            EditorGUI.showMixedValue = false;
            return intProp.intValue;
        }

        private static void Label(Rect rect, GUIContent guiContent)
        {
            GUI.Label(rect, guiContent, ParticleSystemStyles.Get().label);
        }

        protected static bool MinusButton(Rect position) => 
            GUI.Button(new Rect(position.x - 2f, position.y - 2f, 12f, 13f), GUIContent.none, "OL Minus");

        public abstract void OnInspectorGUI(InitialModuleUI initial);
        protected virtual void OnModuleDisable()
        {
            ParticleSystemCurveEditor particleSystemCurveEditor = this.m_ParticleSystemUI.m_ParticleEffectUI.GetParticleSystemCurveEditor();
            foreach (SerializedProperty property in this.m_ModuleCurves)
            {
                if (particleSystemCurveEditor.IsAdded(property))
                {
                    particleSystemCurveEditor.RemoveCurve(property);
                }
            }
        }

        protected virtual void OnModuleEnable()
        {
            this.Init();
        }

        public virtual void OnSceneViewGUI()
        {
        }

        internal UnityEngine.Object ParticleSystemValidator(UnityEngine.Object[] references, System.Type objType, SerializedProperty property)
        {
            foreach (UnityEngine.Object obj2 in references)
            {
                if (obj2 != null)
                {
                    GameObject obj3 = obj2 as GameObject;
                    if (obj3 != null)
                    {
                        ParticleSystem component = obj3.GetComponent<ParticleSystem>();
                        if (component != null)
                        {
                            return component;
                        }
                    }
                }
            }
            return null;
        }

        protected static bool PlusButton(Rect position) => 
            GUI.Button(new Rect(position.x - 2f, position.y - 2f, 12f, 13f), GUIContent.none, "OL Plus");

        protected static Rect PrefixLabel(Rect totalPosition, GUIContent label)
        {
            if (!EditorGUI.LabelHasContent(label))
            {
                return EditorGUI.IndentedRect(totalPosition);
            }
            Rect labelPosition = new Rect(totalPosition.x + EditorGUI.indent, totalPosition.y, EditorGUIUtility.labelWidth - EditorGUI.indent, 13f);
            Rect rect3 = new Rect(totalPosition.x + EditorGUIUtility.labelWidth, totalPosition.y, totalPosition.width - EditorGUIUtility.labelWidth, totalPosition.height);
            EditorGUI.HandlePrefixLabel(totalPosition, labelPosition, label, 0, ParticleSystemStyles.Get().label);
            return rect3;
        }

        private static void SelectMinMaxColorStateCallback(object obj)
        {
            ColorCallbackData data = (ColorCallbackData) obj;
            data.boolProp.boolValue = data.selectedState;
        }

        private static void SelectMinMaxCurveStateCallback(object obj)
        {
            CurveStateCallbackData data = (CurveStateCallbackData) obj;
            foreach (SerializedMinMaxCurve curve in data.minMaxCurves)
            {
                curve.state = data.selectedState;
            }
        }

        private static void SelectMinMaxGradientStateCallback(object obj)
        {
            GradientCallbackData data = (GradientCallbackData) obj;
            data.gradientProp.state = data.selectedState;
        }

        private void Setup(ParticleSystemUI owner, SerializedObject o, string name, string displayName, VisibilityState defaultVisibilityState)
        {
            this.m_ParticleSystemUI = owner;
            this.m_DisplayName = displayName;
            if (this is RendererModuleUI)
            {
                this.m_Enabled = base.GetProperty0("m_Enabled");
            }
            else
            {
                this.m_Enabled = base.GetProperty("enabled");
            }
            this.m_VisibilityState = VisibilityState.NotVisible;
            foreach (UnityEngine.Object obj2 in o.targetObjects)
            {
                VisibilityState @int = (VisibilityState) SessionState.GetInt(base.GetUniqueModuleName(obj2), (int) defaultVisibilityState);
                this.m_VisibilityState = (VisibilityState) Mathf.Max((int) @int, (int) this.m_VisibilityState);
            }
            this.CheckVisibilityState();
            if (this.foldout)
            {
                this.Init();
            }
        }

        protected virtual void SetVisibilityState(VisibilityState newState)
        {
            if (newState != this.m_VisibilityState)
            {
                if (newState == VisibilityState.VisibleAndFolded)
                {
                    ParticleSystemCurveEditor particleSystemCurveEditor = this.m_ParticleSystemUI.m_ParticleEffectUI.GetParticleSystemCurveEditor();
                    foreach (SerializedProperty property in this.m_ModuleCurves)
                    {
                        if (particleSystemCurveEditor.IsAdded(property))
                        {
                            this.m_CurvesRemovedWhenFolded.Add(property);
                            particleSystemCurveEditor.SetVisible(property, false);
                        }
                    }
                    particleSystemCurveEditor.Refresh();
                }
                else if (newState == VisibilityState.VisibleAndFoldedOut)
                {
                    ParticleSystemCurveEditor editor2 = this.m_ParticleSystemUI.m_ParticleEffectUI.GetParticleSystemCurveEditor();
                    foreach (SerializedProperty property2 in this.m_CurvesRemovedWhenFolded)
                    {
                        editor2.SetVisible(property2, true);
                    }
                    this.m_CurvesRemovedWhenFolded.Clear();
                    editor2.Refresh();
                }
                this.m_VisibilityState = newState;
                foreach (UnityEngine.Object obj2 in base.serializedObject.targetObjects)
                {
                    SessionState.SetInt(base.GetUniqueModuleName(obj2), (int) this.m_VisibilityState);
                }
                if (newState == VisibilityState.VisibleAndFoldedOut)
                {
                    this.Init();
                }
            }
        }

        protected static Rect SubtractPopupWidth(Rect position)
        {
            position.width -= 14f;
            return position;
        }

        private static bool Toggle(Rect rect, SerializedProperty boolProp)
        {
            EditorGUI.showMixedValue = boolProp.hasMultipleDifferentValues;
            EditorGUIInternal.mixedToggleStyle = ParticleSystemStyles.Get().toggleMixed;
            Color color = GUI.color;
            if (boolProp.isAnimated)
            {
                GUI.color = UnityEditor.AnimationMode.animatedPropertyColor;
            }
            EditorGUI.BeginChangeCheck();
            bool flag = EditorGUI.Toggle(rect, boolProp.boolValue, ParticleSystemStyles.Get().toggle);
            if (EditorGUI.EndChangeCheck())
            {
                boolProp.boolValue = flag;
            }
            GUI.color = color;
            EditorGUI.showMixedValue = false;
            EditorGUIInternal.mixedToggleStyle = EditorStyles.toggleMixed;
            return flag;
        }

        public virtual void UndoRedoPerformed()
        {
        }

        public virtual void UpdateCullingSupportedString(ref string text)
        {
        }

        public virtual void Validate()
        {
        }

        public string displayName =>
            this.m_DisplayName;

        public bool enabled
        {
            get => 
                this.m_Enabled.boolValue;
            set
            {
                if (this.m_Enabled.boolValue != value)
                {
                    this.m_Enabled.boolValue = value;
                    if (value)
                    {
                        this.OnModuleEnable();
                    }
                    else
                    {
                        this.OnModuleDisable();
                    }
                }
            }
        }

        public bool enabledHasMultipleDifferentValues =>
            this.m_Enabled.hasMultipleDifferentValues;

        public bool foldout
        {
            get => 
                (this.m_VisibilityState == VisibilityState.VisibleAndFoldedOut);
            set
            {
                this.SetVisibilityState(!value ? VisibilityState.VisibleAndFolded : VisibilityState.VisibleAndFoldedOut);
            }
        }

        public bool isWindowView =>
            (this.m_ParticleSystemUI.m_ParticleEffectUI.m_Owner is ParticleSystemWindow);

        public string toolTip =>
            this.m_ToolTip;

        public bool visibleUI
        {
            get => 
                (this.m_VisibilityState != VisibilityState.NotVisible);
            set
            {
                this.SetVisibilityState(!value ? VisibilityState.NotVisible : VisibilityState.VisibleAndFolded);
            }
        }

        private class ColorCallbackData
        {
            public SerializedProperty boolProp;
            public bool selectedState;

            public ColorCallbackData(bool state, SerializedProperty bp)
            {
                this.boolProp = bp;
                this.selectedState = state;
            }
        }

        public delegate bool CurveFieldMouseDownCallback(int button, Rect drawRect, Rect curveRanges);

        private class CurveStateCallbackData
        {
            public SerializedMinMaxCurve[] minMaxCurves;
            public MinMaxCurveState selectedState;

            public CurveStateCallbackData(MinMaxCurveState state, SerializedMinMaxCurve[] curves)
            {
                this.minMaxCurves = curves;
                this.selectedState = state;
            }
        }

        private class GradientCallbackData
        {
            public SerializedMinMaxGradient gradientProp;
            public MinMaxGradientState selectedState;

            public GradientCallbackData(MinMaxGradientState state, SerializedMinMaxGradient p)
            {
                this.gradientProp = p;
                this.selectedState = state;
            }
        }

        public enum VisibilityState
        {
            NotVisible,
            VisibleAndFolded,
            VisibleAndFoldedOut
        }
    }
}

