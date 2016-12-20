namespace UnityEngine
{
    using System;
    using UnityEngine.Scripting;

    internal class EnumInfo
    {
        public string[] annotations;
        public GUIContent[] guiNames;
        public bool isFlags;
        public string[] names;
        public int[] values;

        [UsedByNativeCode]
        internal static EnumInfo CreateEnumInfoFromNativeEnum(string[] names, int[] values, string[] annotations, bool isFlags)
        {
            EnumInfo info = new EnumInfo {
                names = names,
                values = values,
                annotations = annotations,
                isFlags = isFlags,
                guiNames = new GUIContent[names.Length]
            };
            for (int i = 0; i < names.Length; i++)
            {
                info.guiNames[i] = new GUIContent(names[i], annotations[i]);
            }
            return info;
        }
    }
}

