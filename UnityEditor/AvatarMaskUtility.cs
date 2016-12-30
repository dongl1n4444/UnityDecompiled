namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class AvatarMaskUtility
    {
        private static string sBoneName = "m_BoneName";
        private static string sHuman = "m_HumanDescription.m_Human";

        public static string[] GetAvatarHumanTransform(SerializedObject so, string[] refTransformsPath)
        {
            SerializedProperty property = so.FindProperty(sHuman);
            if ((property == null) || !property.isArray)
            {
                return null;
            }
            string[] array = new string[0];
            for (int i = 0; i < property.arraySize; i++)
            {
                SerializedProperty property2 = property.GetArrayElementAtIndex(i).FindPropertyRelative(sBoneName);
                ArrayUtility.Add<string>(ref array, property2.stringValue);
            }
            return TokeniseHumanTransformsPath(refTransformsPath, array);
        }

        public static void SetActiveHumanTransforms(AvatarMask mask, string[] humanTransforms)
        {
            for (int i = 0; i < mask.transformCount; i++)
            {
                <SetActiveHumanTransforms>c__AnonStorey4 storey = new <SetActiveHumanTransforms>c__AnonStorey4 {
                    path = mask.GetTransformPath(i)
                };
                if (ArrayUtility.FindIndex<string>(humanTransforms, new Predicate<string>(storey.<>m__0)) != -1)
                {
                    mask.SetTransformActive(i, true);
                }
            }
        }

        private static string[] TokeniseHumanTransformsPath(string[] refTransformsPath, string[] humanTransforms)
        {
            <TokeniseHumanTransformsPath>c__AnonStorey5 storey = new <TokeniseHumanTransformsPath>c__AnonStorey5 {
                humanTransforms = humanTransforms
            };
            if (storey.humanTransforms == null)
            {
                return null;
            }
            string[] array = new string[] { "" };
            <TokeniseHumanTransformsPath>c__AnonStorey6 storey2 = new <TokeniseHumanTransformsPath>c__AnonStorey6 {
                <>f__ref$5 = storey,
                i = 0
            };
            while (storey2.i < storey.humanTransforms.Length)
            {
                int index = ArrayUtility.FindIndex<string>(refTransformsPath, new Predicate<string>(storey2.<>m__0));
                if (index != -1)
                {
                    <TokeniseHumanTransformsPath>c__AnonStorey7 storey3 = new <TokeniseHumanTransformsPath>c__AnonStorey7();
                    int length = array.Length;
                    storey3.path = refTransformsPath[index];
                    while (storey3.path.Length > 0)
                    {
                        if (ArrayUtility.FindIndex<string>(array, new Predicate<string>(storey3.<>m__0)) == -1)
                        {
                            ArrayUtility.Insert<string>(ref array, length, storey3.path);
                        }
                        int num4 = storey3.path.LastIndexOf('/');
                        storey3.path = storey3.path.Substring(0, (num4 == -1) ? 0 : num4);
                    }
                }
                storey2.i++;
            }
            return array;
        }

        public static void UpdateTransformMask(SerializedProperty transformMask, string[] refTransformsPath, string[] humanTransforms)
        {
            <UpdateTransformMask>c__AnonStorey2 storey = new <UpdateTransformMask>c__AnonStorey2 {
                refTransformsPath = refTransformsPath
            };
            AvatarMask mask = new AvatarMask {
                transformCount = storey.refTransformsPath.Length
            };
            <UpdateTransformMask>c__AnonStorey3 storey2 = new <UpdateTransformMask>c__AnonStorey3 {
                <>f__ref$2 = storey,
                i = 0
            };
            while (storey2.i < storey.refTransformsPath.Length)
            {
                bool flag = (humanTransforms != null) ? (ArrayUtility.FindIndex<string>(humanTransforms, new Predicate<string>(storey2.<>m__0)) != -1) : true;
                mask.SetTransformPath(storey2.i, storey.refTransformsPath[storey2.i]);
                mask.SetTransformActive(storey2.i, flag);
                storey2.i++;
            }
            ModelImporter.UpdateTransformMask(mask, transformMask);
        }

        public static void UpdateTransformMask(AvatarMask mask, string[] refTransformsPath, string[] humanTransforms)
        {
            <UpdateTransformMask>c__AnonStorey0 storey = new <UpdateTransformMask>c__AnonStorey0 {
                refTransformsPath = refTransformsPath
            };
            mask.transformCount = storey.refTransformsPath.Length;
            <UpdateTransformMask>c__AnonStorey1 storey2 = new <UpdateTransformMask>c__AnonStorey1 {
                <>f__ref$0 = storey,
                i = 0
            };
            while (storey2.i < storey.refTransformsPath.Length)
            {
                mask.SetTransformPath(storey2.i, storey.refTransformsPath[storey2.i]);
                bool flag = (humanTransforms != null) ? (ArrayUtility.FindIndex<string>(humanTransforms, new Predicate<string>(storey2.<>m__0)) != -1) : true;
                mask.SetTransformActive(storey2.i, flag);
                storey2.i++;
            }
        }

        [CompilerGenerated]
        private sealed class <SetActiveHumanTransforms>c__AnonStorey4
        {
            internal string path;

            internal bool <>m__0(string s) => 
                (this.path == s);
        }

        [CompilerGenerated]
        private sealed class <TokeniseHumanTransformsPath>c__AnonStorey5
        {
            internal string[] humanTransforms;
        }

        [CompilerGenerated]
        private sealed class <TokeniseHumanTransformsPath>c__AnonStorey6
        {
            internal AvatarMaskUtility.<TokeniseHumanTransformsPath>c__AnonStorey5 <>f__ref$5;
            internal int i;

            internal bool <>m__0(string s) => 
                (this.<>f__ref$5.humanTransforms[this.i] == FileUtil.GetLastPathNameComponent(s));
        }

        [CompilerGenerated]
        private sealed class <TokeniseHumanTransformsPath>c__AnonStorey7
        {
            internal string path;

            internal bool <>m__0(string s) => 
                (this.path == s);
        }

        [CompilerGenerated]
        private sealed class <UpdateTransformMask>c__AnonStorey0
        {
            internal string[] refTransformsPath;
        }

        [CompilerGenerated]
        private sealed class <UpdateTransformMask>c__AnonStorey1
        {
            internal AvatarMaskUtility.<UpdateTransformMask>c__AnonStorey0 <>f__ref$0;
            internal int i;

            internal bool <>m__0(string s) => 
                (this.<>f__ref$0.refTransformsPath[this.i] == s);
        }

        [CompilerGenerated]
        private sealed class <UpdateTransformMask>c__AnonStorey2
        {
            internal string[] refTransformsPath;
        }

        [CompilerGenerated]
        private sealed class <UpdateTransformMask>c__AnonStorey3
        {
            internal AvatarMaskUtility.<UpdateTransformMask>c__AnonStorey2 <>f__ref$2;
            internal int i;

            internal bool <>m__0(string s) => 
                (this.<>f__ref$2.refTransformsPath[this.i] == s);
        }
    }
}

