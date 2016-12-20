using System;
using System.Collections;
using UnityEditor;

internal class PatchImportSettingRecycleID
{
    public static void Patch(SerializedObject serializedObject, int classID, string oldName, string newName)
    {
        string[] oldNames = new string[] { oldName };
        string[] newNames = new string[] { newName };
        PatchMultiple(serializedObject, classID, oldNames, newNames);
    }

    public static void PatchMultiple(SerializedObject serializedObject, int classID, string[] oldNames, string[] newNames)
    {
        int length = oldNames.Length;
        IEnumerator enumerator = serializedObject.FindProperty("m_FileIDToRecycleName").GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                SerializedProperty current = (SerializedProperty) enumerator.Current;
                if (AssetImporter.LocalFileIDToClassID(current.FindPropertyRelative("first").longValue) == classID)
                {
                    SerializedProperty property4 = current.FindPropertyRelative("second");
                    int index = Array.IndexOf<string>(oldNames, property4.stringValue);
                    if (index >= 0)
                    {
                        property4.stringValue = newNames[index];
                        if (--length == 0)
                        {
                            return;
                        }
                    }
                }
            }
        }
        finally
        {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}

