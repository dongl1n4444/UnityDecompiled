namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class AvatarBipedMapper
    {
        private static BipedBone[] s_BipedBones = new BipedBone[] { 
            new BipedBone("Pelvis", 0), new BipedBone("L Thigh", 1), new BipedBone("R Thigh", 2), new BipedBone("L Calf", 3), new BipedBone("R Calf", 4), new BipedBone("L Foot", 5), new BipedBone("R Foot", 6), new BipedBone("Spine", 7), new BipedBone("Spine1", 8), new BipedBone("Spine2", 0x36), new BipedBone("Neck", 9), new BipedBone("Head", 10), new BipedBone("L Clavicle", 11), new BipedBone("R Clavicle", 12), new BipedBone("L UpperArm", 13), new BipedBone("R UpperArm", 14),
            new BipedBone("L Forearm", 15), new BipedBone("R Forearm", 0x10), new BipedBone("L Hand", 0x11), new BipedBone("R Hand", 0x12), new BipedBone("L Toe0", 0x13), new BipedBone("R Toe0", 20), new BipedBone("L Finger0", 0x18), new BipedBone("L Finger01", 0x19), new BipedBone("L Finger02", 0x1a), new BipedBone("L Finger1", 0x1b), new BipedBone("L Finger11", 0x1c), new BipedBone("L Finger12", 0x1d), new BipedBone("L Finger2", 30), new BipedBone("L Finger21", 0x1f), new BipedBone("L Finger22", 0x20), new BipedBone("L Finger3", 0x21),
            new BipedBone("L Finger31", 0x22), new BipedBone("L Finger32", 0x23), new BipedBone("L Finger4", 0x24), new BipedBone("L Finger41", 0x25), new BipedBone("L Finger42", 0x26), new BipedBone("R Finger0", 0x27), new BipedBone("R Finger01", 40), new BipedBone("R Finger02", 0x29), new BipedBone("R Finger1", 0x2a), new BipedBone("R Finger11", 0x2b), new BipedBone("R Finger12", 0x2c), new BipedBone("R Finger2", 0x2d), new BipedBone("R Finger21", 0x2e), new BipedBone("R Finger22", 0x2f), new BipedBone("R Finger3", 0x30), new BipedBone("R Finger31", 0x31),
            new BipedBone("R Finger32", 50), new BipedBone("R Finger4", 0x33), new BipedBone("R Finger41", 0x34), new BipedBone("R Finger42", 0x35)
        };

        internal static void BipedPose(GameObject go, AvatarSetupTool.BoneWrapper[] bones)
        {
            BipedPose(go.transform, true);
            Quaternion rotation = AvatarSetupTool.AvatarComputeOrientation(bones);
            go.transform.rotation = Quaternion.Inverse(rotation) * go.transform.rotation;
            AvatarSetupTool.MakeCharacterPositionValid(bones);
        }

        private static void BipedPose(Transform t, bool ignore)
        {
            if (t.name.EndsWith("Pelvis"))
            {
                t.localRotation = Quaternion.Euler(270f, 90f, 0f);
                ignore = false;
            }
            else if (t.name.EndsWith("Thigh"))
            {
                t.localRotation = Quaternion.Euler(0f, 180f, 0f);
            }
            else if (t.name.EndsWith("Toe0"))
            {
                t.localRotation = Quaternion.Euler(0f, 0f, 270f);
            }
            else if (t.name.EndsWith("L Clavicle"))
            {
                t.localRotation = Quaternion.Euler(0f, 270f, 180f);
            }
            else if (t.name.EndsWith("R Clavicle"))
            {
                t.localRotation = Quaternion.Euler(0f, 90f, 180f);
            }
            else if (t.name.EndsWith("L Hand"))
            {
                t.localRotation = Quaternion.Euler(270f, 0f, 0f);
            }
            else if (t.name.EndsWith("R Hand"))
            {
                t.localRotation = Quaternion.Euler(90f, 0f, 0f);
            }
            else if (t.name.EndsWith("L Finger0"))
            {
                t.localRotation = Quaternion.Euler(0f, 315f, 0f);
            }
            else if (t.name.EndsWith("R Finger0"))
            {
                t.localRotation = Quaternion.Euler(0f, 45f, 0f);
            }
            else if (!ignore)
            {
                t.localRotation = Quaternion.identity;
            }
            IEnumerator enumerator = t.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    BipedPose(current, ignore);
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

        public static bool IsBiped(Transform root, List<string> report)
        {
            if (report != null)
            {
                report.Clear();
            }
            Transform[] humanToTransform = new Transform[HumanTrait.BoneCount];
            return MapBipedBones(root, ref humanToTransform, report);
        }

        private static Transform MapBipedBone(int bipedBoneIndex, Transform transform, Transform parentTransform, List<string> report)
        {
            Transform child = null;
            if (transform != null)
            {
                int childCount = transform.childCount;
                for (int i = 0; (child == null) && (i < childCount); i++)
                {
                    string name = s_BipedBones[bipedBoneIndex].name;
                    int index = s_BipedBones[bipedBoneIndex].index;
                    if (transform.GetChild(i).name.EndsWith(name))
                    {
                        child = transform.GetChild(i);
                        if (((child != null) && (report != null)) && ((index != 0) && (transform != parentTransform)))
                        {
                            string[] textArray1 = new string[] { "- Invalid parent for ", child.name, ". Expected ", parentTransform.name, ", but found ", transform.name, "." };
                            string item = string.Concat(textArray1);
                            switch (index)
                            {
                                case 1:
                                case 2:
                                    item = item + " Disable Triangle Pelvis";
                                    break;

                                case 11:
                                case 12:
                                    item = item + " Enable Triangle Neck";
                                    break;

                                case 9:
                                    item = item + " Preferred is three Spine Links";
                                    break;

                                case 10:
                                    item = item + " Preferred is one Neck Links";
                                    break;
                            }
                            item = item + "\n";
                            report.Add(item);
                        }
                    }
                }
                for (int j = 0; (child == null) && (j < childCount); j++)
                {
                    child = MapBipedBone(bipedBoneIndex, transform.GetChild(j), parentTransform, report);
                }
            }
            return child;
        }

        private static bool MapBipedBones(Transform root, ref Transform[] humanToTransform, List<string> report)
        {
            for (int i = 0; i < s_BipedBones.Length; i++)
            {
                int index = s_BipedBones[i].index;
                int parentBone = HumanTrait.GetParentBone(index);
                bool flag = HumanTrait.RequiredBone(index);
                bool flag2 = (parentBone == -1) || HumanTrait.RequiredBone(parentBone);
                Transform transform = (parentBone == -1) ? root : humanToTransform[parentBone];
                if ((transform == null) && !flag2)
                {
                    parentBone = HumanTrait.GetParentBone(parentBone);
                    flag2 = (parentBone == -1) || HumanTrait.RequiredBone(parentBone);
                    transform = (parentBone == -1) ? null : humanToTransform[parentBone];
                    if ((transform == null) && !flag2)
                    {
                        parentBone = HumanTrait.GetParentBone(parentBone);
                        transform = (parentBone == -1) ? null : humanToTransform[parentBone];
                    }
                }
                humanToTransform[index] = MapBipedBone(i, transform, transform, report);
                if ((humanToTransform[index] == null) && flag)
                {
                    return false;
                }
            }
            return true;
        }

        public static Dictionary<int, Transform> MapBones(Transform root)
        {
            Dictionary<int, Transform> dictionary = new Dictionary<int, Transform>();
            Transform[] humanToTransform = new Transform[HumanTrait.BoneCount];
            if (MapBipedBones(root, ref humanToTransform, null))
            {
                for (int i = 0; i < HumanTrait.BoneCount; i++)
                {
                    if (humanToTransform[i] != null)
                    {
                        dictionary.Add(i, humanToTransform[i]);
                    }
                }
            }
            if (!dictionary.ContainsKey(8) && dictionary.ContainsKey(0x36))
            {
                dictionary.Add(8, dictionary[0x36]);
                dictionary.Remove(0x36);
            }
            return dictionary;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BipedBone
        {
            public string name;
            public int index;
            public BipedBone(string name, int index)
            {
                this.name = name;
                this.index = index;
            }
        }
    }
}

