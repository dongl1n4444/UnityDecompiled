using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ExposedReference<>))]
internal class ExposedReferencePropertyDrawer : BaseExposedPropertyDrawer
{
    protected override void OnRenderProperty(Rect position, PropertyName exposedPropertyNameString, UnityEngine.Object currentReferenceValue, SerializedProperty exposedPropertyDefault, SerializedProperty exposedPropertyName, BaseExposedPropertyDrawer.ExposedPropertyMode mode, IExposedPropertyTable exposedPropertyTable)
    {
        System.Type objType = base.fieldInfo.FieldType.GetGenericArguments()[0];
        EditorGUI.BeginChangeCheck();
        UnityEngine.Object target = EditorGUI.ObjectField(position, currentReferenceValue, objType, exposedPropertyTable != null);
        if (EditorGUI.EndChangeCheck())
        {
            if (mode == BaseExposedPropertyDrawer.ExposedPropertyMode.DefaultValue)
            {
                if ((!EditorUtility.IsPersistent(exposedPropertyDefault.serializedObject.targetObject) || (target == null)) || EditorUtility.IsPersistent(target))
                {
                    if (!EditorGUI.CheckForCrossSceneReferencing(exposedPropertyDefault.serializedObject.targetObject, target))
                    {
                        exposedPropertyDefault.objectReferenceValue = target;
                    }
                }
                else
                {
                    string name = GUID.Generate().ToString();
                    exposedPropertyNameString = new PropertyName(name);
                    exposedPropertyName.stringValue = name;
                    Undo.RecordObject(exposedPropertyTable as UnityEngine.Object, "Set Exposed Property");
                    exposedPropertyTable.SetReferenceValue(exposedPropertyNameString, target);
                }
            }
            else
            {
                Undo.RecordObject(exposedPropertyTable as UnityEngine.Object, "Set Exposed Property");
                exposedPropertyTable.SetReferenceValue(exposedPropertyNameString, target);
            }
        }
    }

    protected override void PopulateContextMenu(GenericMenu menu, BaseExposedPropertyDrawer.OverrideState overrideState, IExposedPropertyTable exposedPropertyTable, SerializedProperty exposedName, SerializedProperty defaultValue)
    {
        BaseExposedPropertyDrawer.OverrideState state;
        <PopulateContextMenu>c__AnonStorey0 storey = new <PopulateContextMenu>c__AnonStorey0 {
            exposedName = exposedName,
            exposedPropertyTable = exposedPropertyTable
        };
        storey.propertyName = new PropertyName(storey.exposedName.stringValue);
        storey.currentValue = base.Resolve(new PropertyName(storey.exposedName.stringValue), storey.exposedPropertyTable, defaultValue.objectReferenceValue, out state);
        if (overrideState == BaseExposedPropertyDrawer.OverrideState.DefaultValue)
        {
            menu.AddItem(new GUIContent(base.ExposePropertyContent.text), false, new GenericMenu.MenuFunction2(storey.<>m__0), null);
        }
        else
        {
            menu.AddItem(base.UnexposePropertyContent, false, new GenericMenu.MenuFunction2(storey.<>m__1), null);
        }
    }

    [CompilerGenerated]
    private sealed class <PopulateContextMenu>c__AnonStorey0
    {
        internal UnityEngine.Object currentValue;
        internal SerializedProperty exposedName;
        internal IExposedPropertyTable exposedPropertyTable;
        internal PropertyName propertyName;

        internal void <>m__0(object userData)
        {
            this.exposedName.stringValue = GUID.Generate().ToString();
            this.exposedName.serializedObject.ApplyModifiedProperties();
            PropertyName id = new PropertyName(this.exposedName.stringValue);
            Undo.RecordObject(this.exposedPropertyTable as UnityEngine.Object, "Set Exposed Property");
            this.exposedPropertyTable.SetReferenceValue(id, this.currentValue);
        }

        internal void <>m__1(object userData)
        {
            this.exposedName.stringValue = "";
            this.exposedName.serializedObject.ApplyModifiedProperties();
            Undo.RecordObject(this.exposedPropertyTable as UnityEngine.Object, "Clear Exposed Property");
            this.exposedPropertyTable.ClearReferenceValue(this.propertyName);
        }
    }
}

