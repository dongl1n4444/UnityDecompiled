namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering;

    [CustomEditor(typeof(ComputeShader))]
    internal class ComputeShaderInspector : Editor
    {
        private const float kSpace = 5f;
        private Vector2 m_ScrollPosition = Vector2.zero;

        private static List<KernelInfo> GetKernelDisplayInfo(ComputeShader cs)
        {
            List<KernelInfo> list = new List<KernelInfo>();
            int computeShaderPlatformCount = ShaderUtil.GetComputeShaderPlatformCount(cs);
            for (int i = 0; i < computeShaderPlatformCount; i++)
            {
                GraphicsDeviceType computeShaderPlatformType = ShaderUtil.GetComputeShaderPlatformType(cs, i);
                int computeShaderPlatformKernelCount = ShaderUtil.GetComputeShaderPlatformKernelCount(cs, i);
                for (int j = 0; j < computeShaderPlatformKernelCount; j++)
                {
                    string str = ShaderUtil.GetComputeShaderPlatformKernelName(cs, i, j);
                    bool flag = false;
                    foreach (KernelInfo info in list)
                    {
                        if (info.name == str)
                        {
                            info.platforms = info.platforms + ' ';
                            info.platforms = info.platforms + computeShaderPlatformType.ToString();
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        KernelInfo item = new KernelInfo {
                            name = str,
                            platforms = computeShaderPlatformType.ToString()
                        };
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        public override void OnInspectorGUI()
        {
            ComputeShader target = base.target as ComputeShader;
            if (target != null)
            {
                GUI.enabled = true;
                EditorGUI.indentLevel = 0;
                this.ShowKernelInfoSection(target);
                this.ShowCompiledCodeSection(target);
                this.ShowShaderErrors(target);
            }
        }

        private void ShowCompiledCodeSection(ComputeShader cs)
        {
            GUILayout.Space(5f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            if (GUILayout.Button(Styles.showCompiled, EditorStyles.miniButton, options))
            {
                ShaderUtil.OpenCompiledComputeShader(cs, true);
                GUIUtility.ExitGUI();
            }
        }

        private void ShowKernelInfoSection(ComputeShader cs)
        {
            GUILayout.Label(Styles.kernelsHeading, EditorStyles.boldLabel, new GUILayoutOption[0]);
            List<KernelInfo> kernelDisplayInfo = GetKernelDisplayInfo(cs);
            foreach (KernelInfo info in kernelDisplayInfo)
            {
                EditorGUILayout.LabelField(info.name, info.platforms, new GUILayoutOption[0]);
            }
        }

        private void ShowShaderErrors(ComputeShader s)
        {
            if (ShaderUtil.GetComputeShaderErrorCount(s) >= 1)
            {
                ShaderInspector.ShaderErrorListUI(s, ShaderUtil.GetComputeShaderErrors(s), ref this.m_ScrollPosition);
            }
        }

        private class KernelInfo
        {
            internal string name;
            internal string platforms;
        }

        internal class Styles
        {
            public static GUIContent kernelsHeading = EditorGUIUtility.TextContent("Kernels:");
            public static GUIContent showCompiled = EditorGUIUtility.TextContent("Show compiled code");
        }
    }
}

