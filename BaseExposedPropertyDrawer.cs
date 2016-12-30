using System;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

internal abstract class BaseExposedPropertyDrawer : PropertyDrawer
{
    protected readonly GUIContent ExposePropertyContent = EditorGUIUtility.TextContent("Expose Property");
    private static float kDriveWidgetWidth = 18f;
    private static GUIStyle kDropDownStyle = null;
    private static Color kMissingOverrideColor = new Color(1f, 0.11f, 0.11f, 1f);
    private GUIContent m_ModifiedLabel = new GUIContent();
    protected readonly GUIContent NotFoundOn = EditorGUIUtility.TextContent("not found on");
    protected readonly GUIContent OverridenByContent = EditorGUIUtility.TextContent("Overriden by ");
    protected readonly GUIContent UnexposePropertyContent = EditorGUIUtility.TextContent("Unexpose Property");

    public BaseExposedPropertyDrawer()
    {
        if (kDropDownStyle == null)
        {
            kDropDownStyle = new GUIStyle("ShurikenDropdown");
        }
    }

    private Rect DrawLabel(bool showContextMenu, OverrideState currentOverrideState, GUIContent label, Rect position, IExposedPropertyTable exposedPropertyTable, string exposedNameStr, SerializedProperty exposedName, SerializedProperty defaultValue)
    {
        if (showContextMenu)
        {
            position.xMax -= kDriveWidgetWidth;
        }
        EditorGUIUtility.SetBoldDefaultFont(currentOverrideState != OverrideState.DefaultValue);
        this.m_ModifiedLabel.text = label.text;
        this.m_ModifiedLabel.tooltip = label.tooltip;
        this.m_ModifiedLabel.image = label.image;
        if (!string.IsNullOrEmpty(this.m_ModifiedLabel.tooltip))
        {
            this.m_ModifiedLabel.tooltip = this.m_ModifiedLabel.tooltip + "\n";
        }
        if (currentOverrideState == OverrideState.MissingOverride)
        {
            GUI.color = kMissingOverrideColor;
            string tooltip = this.m_ModifiedLabel.tooltip;
            string[] textArray1 = new string[] { tooltip, label.text, " ", this.NotFoundOn.text, " ", exposedPropertyTable.ToString(), "." };
            this.m_ModifiedLabel.tooltip = string.Concat(textArray1);
        }
        else if ((currentOverrideState == OverrideState.Overridden) && (exposedPropertyTable != null))
        {
            this.m_ModifiedLabel.tooltip = this.m_ModifiedLabel.tooltip + this.OverridenByContent.text + exposedPropertyTable.ToString() + ".";
        }
        Rect rect = EditorGUI.PrefixLabel(position, this.m_ModifiedLabel);
        if (((exposedPropertyTable != null) && (Event.current.type == EventType.ContextClick)) && position.Contains(Event.current.mousePosition))
        {
            GenericMenu menu = new GenericMenu();
            this.PopulateContextMenu(menu, !string.IsNullOrEmpty(exposedNameStr) ? OverrideState.Overridden : OverrideState.DefaultValue, exposedPropertyTable, exposedName, defaultValue);
            menu.ShowAsContext();
        }
        return rect;
    }

    private static ExposedPropertyMode GetExposedPropertyMode(string propertyName)
    {
        GUID guid;
        if (string.IsNullOrEmpty(propertyName))
        {
            return ExposedPropertyMode.DefaultValue;
        }
        if (GUID.TryParse(propertyName, out guid))
        {
            return ExposedPropertyMode.NamedGUID;
        }
        return ExposedPropertyMode.Named;
    }

    protected IExposedPropertyTable GetExposedPropertyTable(SerializedProperty property) => 
        (property.serializedObject.context as IExposedPropertyTable);

    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        SerializedProperty defaultValue = prop.FindPropertyRelative("defaultValue");
        SerializedProperty exposedName = prop.FindPropertyRelative("exposedName");
        string stringValue = exposedName.stringValue;
        ExposedPropertyMode exposedPropertyMode = GetExposedPropertyMode(stringValue);
        Rect rect = position;
        rect.xMax -= kDriveWidgetWidth;
        Rect rect2 = position;
        rect2.x = rect.xMax;
        rect2.width = kDriveWidgetWidth;
        IExposedPropertyTable exposedPropertyTable = this.GetExposedPropertyTable(prop);
        bool showContextMenu = exposedPropertyTable != null;
        PropertyName exposedPropertyName = new PropertyName(stringValue);
        OverrideState currentOverrideState = OverrideState.DefaultValue;
        Object currentReferenceValue = this.Resolve(exposedPropertyName, exposedPropertyTable, defaultValue.objectReferenceValue, out currentOverrideState);
        Color color = GUI.color;
        bool boldDefaultFont = EditorGUIUtility.GetBoldDefaultFont();
        Rect rect3 = this.DrawLabel(showContextMenu, currentOverrideState, label, position, exposedPropertyTable, stringValue, exposedName, defaultValue);
        EditorGUI.BeginChangeCheck();
        switch (exposedPropertyMode)
        {
            case ExposedPropertyMode.DefaultValue:
            case ExposedPropertyMode.NamedGUID:
                this.OnRenderProperty(rect3, exposedPropertyName, currentReferenceValue, defaultValue, exposedName, exposedPropertyMode, exposedPropertyTable);
                break;

            default:
                rect3.width /= 2f;
                EditorGUI.BeginChangeCheck();
                stringValue = EditorGUI.TextField(rect3, stringValue);
                if (EditorGUI.EndChangeCheck())
                {
                    exposedName.stringValue = stringValue;
                }
                rect3.x += rect3.width;
                this.OnRenderProperty(rect3, new PropertyName(stringValue), currentReferenceValue, defaultValue, exposedName, exposedPropertyMode, exposedPropertyTable);
                break;
        }
        EditorGUI.EndDisabledGroup();
        GUI.color = color;
        EditorGUIUtility.SetBoldDefaultFont(boldDefaultFont);
        if (showContextMenu && GUI.Button(rect2, GUIContent.none, kDropDownStyle))
        {
            GenericMenu menu = new GenericMenu();
            this.PopulateContextMenu(menu, currentOverrideState, exposedPropertyTable, exposedName, defaultValue);
            menu.ShowAsContext();
            Event.current.Use();
        }
    }

    protected abstract void OnRenderProperty(Rect position, PropertyName exposedPropertyNameString, Object currentReferenceValue, SerializedProperty exposedPropertyDefault, SerializedProperty exposedPropertyName, ExposedPropertyMode mode, IExposedPropertyTable exposedProperties);
    protected abstract void PopulateContextMenu(GenericMenu menu, OverrideState overrideState, IExposedPropertyTable exposedPropertyTable, SerializedProperty exposedName, SerializedProperty defaultValue);
    protected Object Resolve(PropertyName exposedPropertyName, IExposedPropertyTable exposedPropertyTable, Object defaultValue, out OverrideState currentOverrideState)
    {
        Object referenceValue = null;
        bool idValid = false;
        bool flag2 = !PropertyName.IsNullOrEmpty(exposedPropertyName);
        currentOverrideState = OverrideState.DefaultValue;
        if (exposedPropertyTable != null)
        {
            referenceValue = exposedPropertyTable.GetReferenceValue(exposedPropertyName, out idValid);
            if (idValid)
            {
                currentOverrideState = OverrideState.Overridden;
            }
            else if (flag2)
            {
                currentOverrideState = OverrideState.MissingOverride;
            }
        }
        return ((currentOverrideState != OverrideState.Overridden) ? defaultValue : referenceValue);
    }

    protected enum ExposedPropertyMode
    {
        DefaultValue,
        Named,
        NamedGUID
    }

    protected enum OverrideState
    {
        DefaultValue,
        MissingOverride,
        Overridden
    }
}

