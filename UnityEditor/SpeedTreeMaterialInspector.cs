namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Rendering;

    [CanEditMultipleObjects]
    internal class SpeedTreeMaterialInspector : MaterialEditor
    {
        [CompilerGenerated]
        private static Predicate<MaterialProperty> <>f__am$cache0;
        [CompilerGenerated]
        private static Predicate<MaterialProperty> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<UnityEngine.Object, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Predicate<MaterialProperty> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<UnityEngine.Object, bool> <>f__am$cache4;
        [CompilerGenerated]
        private static Predicate<MaterialProperty> <>f__am$cache5;
        [CompilerGenerated]
        private static Predicate<MaterialProperty> <>f__am$cache6;
        private string[] speedTreeGeometryTypeString = new string[] { "GEOM_TYPE_BRANCH", "GEOM_TYPE_BRANCH_DETAIL", "GEOM_TYPE_FROND", "GEOM_TYPE_LEAF", "GEOM_TYPE_MESH" };

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            SerializedProperty property = base.serializedObject.FindProperty("m_Shader");
            if ((base.isVisible && !property.hasMultipleDifferentValues) && (property.objectReferenceValue != null))
            {
                List<MaterialProperty> list = new List<MaterialProperty>(MaterialEditor.GetMaterialProperties(base.targets));
                base.SetDefaultGUIWidths();
                SpeedTreeGeometryType[] source = new SpeedTreeGeometryType[base.targets.Length];
                for (int i = 0; i < base.targets.Length; i++)
                {
                    source[i] = SpeedTreeGeometryType.Branch;
                    for (int j = 0; j < this.speedTreeGeometryTypeString.Length; j++)
                    {
                        if (((Material) base.targets[i]).shaderKeywords.Contains<string>(this.speedTreeGeometryTypeString[j]))
                        {
                            source[i] = (SpeedTreeGeometryType) j;
                            break;
                        }
                    }
                }
                EditorGUI.showMixedValue = source.Distinct<SpeedTreeGeometryType>().Count<SpeedTreeGeometryType>() > 1;
                EditorGUI.BeginChangeCheck();
                SpeedTreeGeometryType geomType = (SpeedTreeGeometryType) EditorGUILayout.EnumPopup("Geometry Type", source[0], new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    bool flag = this.ShouldEnableAlphaTest(geomType);
                    CullMode mode = !flag ? CullMode.Back : CullMode.Off;
                    foreach (Material material in base.targets.Cast<Material>())
                    {
                        if (flag)
                        {
                            material.SetOverrideTag("RenderType", "treeTransparentCutout");
                        }
                        for (int k = 0; k < this.speedTreeGeometryTypeString.Length; k++)
                        {
                            material.DisableKeyword(this.speedTreeGeometryTypeString[k]);
                        }
                        material.EnableKeyword(this.speedTreeGeometryTypeString[(int) geomType]);
                        material.renderQueue = !flag ? 0x7d0 : 0x992;
                        material.SetInt("_Cull", (int) mode);
                    }
                }
                EditorGUI.showMixedValue = false;
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = prop => prop.name == "_MainTex";
                }
                MaterialProperty item = list.Find(<>f__am$cache0);
                if (item != null)
                {
                    list.Remove(item);
                    base.ShaderProperty(item, item.displayName);
                }
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = prop => prop.name == "_BumpMap";
                }
                MaterialProperty property3 = list.Find(<>f__am$cache1);
                if (property3 != null)
                {
                    list.Remove(property3);
                    if (<>f__am$cache2 == null)
                    {
                        <>f__am$cache2 = t => ((Material) t).shaderKeywords.Contains<string>("EFFECT_BUMP");
                    }
                    IEnumerable<bool> enumerable = Enumerable.Select<UnityEngine.Object, bool>(base.targets, <>f__am$cache2);
                    bool? nullable = this.ToggleShaderProperty(property3, enumerable.First<bool>(), enumerable.Distinct<bool>().Count<bool>() > 1);
                    if (nullable.HasValue)
                    {
                        foreach (Material material2 in base.targets.Cast<Material>())
                        {
                            if (nullable.Value)
                            {
                                material2.EnableKeyword("EFFECT_BUMP");
                            }
                            else
                            {
                                material2.DisableKeyword("EFFECT_BUMP");
                            }
                        }
                    }
                }
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = prop => prop.name == "_DetailTex";
                }
                MaterialProperty property4 = list.Find(<>f__am$cache3);
                if (property4 != null)
                {
                    list.Remove(property4);
                    if (source.Contains<SpeedTreeGeometryType>(SpeedTreeGeometryType.BranchDetail))
                    {
                        base.ShaderProperty(property4, property4.displayName);
                    }
                }
                if (<>f__am$cache4 == null)
                {
                    <>f__am$cache4 = t => ((Material) t).shaderKeywords.Contains<string>("EFFECT_HUE_VARIATION");
                }
                IEnumerable<bool> enumerable2 = Enumerable.Select<UnityEngine.Object, bool>(base.targets, <>f__am$cache4);
                if (<>f__am$cache5 == null)
                {
                    <>f__am$cache5 = prop => prop.name == "_HueVariation";
                }
                MaterialProperty property5 = list.Find(<>f__am$cache5);
                if ((enumerable2 != null) && (property5 != null))
                {
                    list.Remove(property5);
                    bool? nullable2 = this.ToggleShaderProperty(property5, enumerable2.First<bool>(), enumerable2.Distinct<bool>().Count<bool>() > 1);
                    if (nullable2.HasValue)
                    {
                        foreach (Material material3 in base.targets.Cast<Material>())
                        {
                            if (nullable2.Value)
                            {
                                material3.EnableKeyword("EFFECT_HUE_VARIATION");
                            }
                            else
                            {
                                material3.DisableKeyword("EFFECT_HUE_VARIATION");
                            }
                        }
                    }
                }
                if (<>f__am$cache6 == null)
                {
                    <>f__am$cache6 = prop => prop.name == "_Cutoff";
                }
                MaterialProperty property6 = list.Find(<>f__am$cache6);
                if (property6 != null)
                {
                    list.Remove(property6);
                    if (Enumerable.Any<SpeedTreeGeometryType>(source, (Func<SpeedTreeGeometryType, bool>) (t => this.ShouldEnableAlphaTest(t))))
                    {
                        base.ShaderProperty(property6, property6.displayName);
                    }
                }
                foreach (MaterialProperty property7 in list)
                {
                    if ((property7.flags & (MaterialProperty.PropFlags.PerRendererData | MaterialProperty.PropFlags.HideInInspector)) == MaterialProperty.PropFlags.None)
                    {
                        base.ShaderProperty(property7, property7.displayName);
                    }
                }
            }
        }

        private bool ShouldEnableAlphaTest(SpeedTreeGeometryType geomType) => 
            ((geomType == SpeedTreeGeometryType.Frond) || (geomType == SpeedTreeGeometryType.Leaf));

        private bool? ToggleShaderProperty(MaterialProperty prop, bool enable, bool hasMixedEnable)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = hasMixedEnable;
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            enable = EditorGUI.ToggleLeft(EditorGUILayout.GetControlRect(false, options), prop.displayName, enable);
            EditorGUI.showMixedValue = false;
            bool? nullable = !EditorGUI.EndChangeCheck() ? null : new bool?(enable);
            GUILayout.Space(-EditorGUIUtility.singleLineHeight);
            using (new EditorGUI.DisabledScope(!enable && !hasMixedEnable))
            {
                EditorGUI.showMixedValue = prop.hasMixedValue;
                base.ShaderProperty(prop, " ");
                EditorGUI.showMixedValue = false;
            }
            return nullable;
        }

        private enum SpeedTreeGeometryType
        {
            Branch,
            BranchDetail,
            Frond,
            Leaf,
            Mesh
        }
    }
}

