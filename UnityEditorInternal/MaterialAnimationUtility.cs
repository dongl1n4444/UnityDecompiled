namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;

    internal static class MaterialAnimationUtility
    {
        private const string kMaterialPrefix = "material.";

        private static bool ApplyMaterialModificationToAnimationRecording(string name, UnityEngine.Object target, Vector4 vec)
        {
            UndoPropertyModification[] modifications = CreateUndoPropertyModifications(4, target);
            SetupPropertyModification(name + ".x", vec.x, modifications[0]);
            SetupPropertyModification(name + ".y", vec.y, modifications[1]);
            SetupPropertyModification(name + ".z", vec.z, modifications[2]);
            SetupPropertyModification(name + ".w", vec.w, modifications[3]);
            return (Undo.postprocessModifications(modifications).Length != modifications.Length);
        }

        private static bool ApplyMaterialModificationToAnimationRecording(MaterialProperty materialProp, UnityEngine.Object target, float value)
        {
            UndoPropertyModification[] modifications = CreateUndoPropertyModifications(1, target);
            SetupPropertyModification(materialProp.name, value, modifications[0]);
            return (Undo.postprocessModifications(modifications).Length != modifications.Length);
        }

        private static bool ApplyMaterialModificationToAnimationRecording(MaterialProperty materialProp, UnityEngine.Object target, Color color)
        {
            UndoPropertyModification[] modifications = CreateUndoPropertyModifications(4, target);
            SetupPropertyModification(materialProp.name + ".r", color.r, modifications[0]);
            SetupPropertyModification(materialProp.name + ".g", color.g, modifications[1]);
            SetupPropertyModification(materialProp.name + ".b", color.b, modifications[2]);
            SetupPropertyModification(materialProp.name + ".a", color.a, modifications[3]);
            return (Undo.postprocessModifications(modifications).Length != modifications.Length);
        }

        public static bool ApplyMaterialModificationToAnimationRecording(MaterialProperty materialProp, int changedMask, Renderer target, object oldValue)
        {
            switch (materialProp.type)
            {
                case MaterialProperty.PropType.Color:
                    SetupMaterialPropertyBlock(materialProp, changedMask, target);
                    return ApplyMaterialModificationToAnimationRecording(materialProp, target, (Color) oldValue);

                case MaterialProperty.PropType.Vector:
                    SetupMaterialPropertyBlock(materialProp, changedMask, target);
                    return ApplyMaterialModificationToAnimationRecording(materialProp, target, (Color) ((Vector4) oldValue));

                case MaterialProperty.PropType.Float:
                case MaterialProperty.PropType.Range:
                    SetupMaterialPropertyBlock(materialProp, changedMask, target);
                    return ApplyMaterialModificationToAnimationRecording(materialProp, target, (float) oldValue);

                case MaterialProperty.PropType.Texture:
                {
                    if (!MaterialProperty.IsTextureOffsetAndScaleChangedMask(changedMask))
                    {
                        return false;
                    }
                    string name = materialProp.name + "_ST";
                    SetupMaterialPropertyBlock(materialProp, changedMask, target);
                    return ApplyMaterialModificationToAnimationRecording(name, target, (Vector4) oldValue);
                }
            }
            return false;
        }

        private static UndoPropertyModification[] CreateUndoPropertyModifications(int count, UnityEngine.Object target)
        {
            UndoPropertyModification[] modificationArray = new UndoPropertyModification[count];
            for (int i = 0; i < modificationArray.Length; i++)
            {
                modificationArray[i].previousValue = new PropertyModification();
                modificationArray[i].previousValue.target = target;
            }
            return modificationArray;
        }

        public static bool OverridePropertyColor(MaterialProperty materialProp, Renderer target, out Color color)
        {
            <OverridePropertyColor>c__AnonStorey0 storey = new <OverridePropertyColor>c__AnonStorey0 {
                target = target
            };
            List<string> list = new List<string>();
            string item = "material." + materialProp.name;
            if (materialProp.type == MaterialProperty.PropType.Texture)
            {
                list.Add(item + "_ST.x");
                list.Add(item + "_ST.y");
                list.Add(item + "_ST.z");
                list.Add(item + "_ST.w");
            }
            else if (materialProp.type == MaterialProperty.PropType.Color)
            {
                list.Add(item + ".r");
                list.Add(item + ".g");
                list.Add(item + ".b");
                list.Add(item + ".a");
            }
            else
            {
                list.Add(item);
            }
            if (list.Exists(new Predicate<string>(storey.<>m__0)))
            {
                color = UnityEditor.AnimationMode.animatedPropertyColor;
                if (UnityEditor.AnimationMode.InAnimationRecording())
                {
                    color = UnityEditor.AnimationMode.recordedPropertyColor;
                }
                else if (list.Exists(new Predicate<string>(storey.<>m__1)))
                {
                    color = UnityEditor.AnimationMode.candidatePropertyColor;
                }
                return true;
            }
            color = Color.white;
            return false;
        }

        public static void SetupMaterialPropertyBlock(MaterialProperty materialProp, int changedMask, Renderer target)
        {
            MaterialPropertyBlock dest = new MaterialPropertyBlock();
            target.GetPropertyBlock(dest);
            materialProp.WriteToMaterialPropertyBlock(dest, changedMask);
            target.SetPropertyBlock(dest);
        }

        private static void SetupPropertyModification(string name, float value, UndoPropertyModification prop)
        {
            prop.previousValue.propertyPath = "material." + name;
            prop.previousValue.value = value.ToString();
        }

        [CompilerGenerated]
        private sealed class <OverridePropertyColor>c__AnonStorey0
        {
            internal Renderer target;

            internal bool <>m__0(string path) => 
                UnityEditor.AnimationMode.IsPropertyAnimated(this.target, path);

            internal bool <>m__1(string path) => 
                UnityEditor.AnimationMode.IsPropertyCandidate(this.target, path);
        }
    }
}

