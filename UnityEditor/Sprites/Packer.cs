namespace UnityEditor.Sprites
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Sprite Packer helpers.</para>
    /// </summary>
    public sealed class Packer
    {
        [CompilerGenerated]
        private static Func<Type, string> <>f__am$cache0;
        /// <summary>
        /// <para>Name of the default Sprite Packer policy.</para>
        /// </summary>
        public static string kDefaultPolicy = typeof(DefaultPackerPolicy).Name;
        private static string[] m_policies = null;
        private static Dictionary<string, Type> m_policyTypeCache = null;
        private static string m_selectedPolicy = null;

        internal static void ExecuteSelectedPolicy(BuildTarget target, int[] textureImporterInstanceIDs)
        {
            RegenerateList();
            Type type = m_policyTypeCache[m_selectedPolicy];
            (Activator.CreateInstance(type) as IPackerPolicy).OnGroupAtlases(target, new PackerJob(), textureImporterInstanceIDs);
        }

        /// <summary>
        /// <para>Returns all alpha atlas textures generated for the specified atlas.</para>
        /// </summary>
        /// <param name="atlasName">Name of the atlas.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern Texture2D[] GetAlphaTexturesForAtlas(string atlasName);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void GetAtlasDataForSprite(Sprite sprite, out string atlasName, out Texture2D atlasTexture);
        internal static string GetSelectedPolicyId()
        {
            RegenerateList();
            Type type = m_policyTypeCache[m_selectedPolicy];
            IPackerPolicy policy = Activator.CreateInstance(type) as IPackerPolicy;
            return $"{type.AssemblyQualifiedName}::{policy.GetVersion()}";
        }

        /// <summary>
        /// <para>Returns all atlas textures generated for the specified atlas.</para>
        /// </summary>
        /// <param name="atlasName">Atlas name.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern Texture2D[] GetTexturesForAtlas(string atlasName);
        [ExcludeFromDocs]
        public static void RebuildAtlasCacheIfNeeded(BuildTarget target)
        {
            Execution normal = Execution.Normal;
            bool displayProgressBar = false;
            RebuildAtlasCacheIfNeeded(target, displayProgressBar, normal);
        }

        [ExcludeFromDocs]
        public static void RebuildAtlasCacheIfNeeded(BuildTarget target, bool displayProgressBar)
        {
            Execution normal = Execution.Normal;
            RebuildAtlasCacheIfNeeded(target, displayProgressBar, normal);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void RebuildAtlasCacheIfNeeded(BuildTarget target, [DefaultValue("false")] bool displayProgressBar, [DefaultValue("Execution.Normal")] Execution execution);
        private static void RegenerateList()
        {
            if (m_policies == null)
            {
                List<Type> list = new List<Type>();
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly assembly in assemblies)
                {
                    try
                    {
                        Type[] types = assembly.GetTypes();
                        foreach (Type type in types)
                        {
                            if (typeof(IPackerPolicy).IsAssignableFrom(type) && (type != typeof(IPackerPolicy)))
                            {
                                list.Add(type);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.Log($"SpritePacker failed to get types from {assembly.FullName}. Error: {exception.Message}");
                    }
                }
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = t => t.Name;
                }
                m_policies = Enumerable.Select<Type, string>(list, <>f__am$cache0).ToArray<string>();
                m_policyTypeCache = new Dictionary<string, Type>();
                foreach (Type type2 in list)
                {
                    if (m_policyTypeCache.ContainsKey(type2.Name))
                    {
                        Type type3 = m_policyTypeCache[type2.Name];
                        Debug.LogError($"Duplicate Sprite Packer policies found: {type2.FullName} and {type3.FullName}. Please rename one.");
                    }
                    else
                    {
                        m_policyTypeCache[type2.Name] = type2;
                    }
                }
                m_selectedPolicy = !string.IsNullOrEmpty(PlayerSettings.spritePackerPolicy) ? PlayerSettings.spritePackerPolicy : kDefaultPolicy;
                if (!m_policies.Contains<string>(m_selectedPolicy))
                {
                    SetSelectedPolicy(kDefaultPolicy);
                }
            }
        }

        internal static void SaveUnappliedTextureImporterSettings()
        {
            foreach (InspectorWindow window in InspectorWindow.GetAllInspectorWindows())
            {
                ActiveEditorTracker tracker = window.tracker;
                foreach (Editor editor in tracker.activeEditors)
                {
                    TextureImporterInspector inspector = editor as TextureImporterInspector;
                    if ((inspector != null) && inspector.HasModified())
                    {
                        TextureImporter target = inspector.target as TextureImporter;
                        if (EditorUtility.DisplayDialog("Unapplied import settings", "Unapplied import settings for '" + target.assetPath + "'", "Apply", "Revert"))
                        {
                            inspector.ApplyAndImport();
                        }
                    }
                }
            }
        }

        private static void SetSelectedPolicy(string value)
        {
            m_selectedPolicy = value;
            PlayerSettings.spritePackerPolicy = m_selectedPolicy;
        }

        /// <summary>
        /// <para>Array of Sprite atlas names found in the current atlas cache.</para>
        /// </summary>
        public static string[] atlasNames { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Available Sprite Packer policies for this project.</para>
        /// </summary>
        public static string[] Policies
        {
            get
            {
                RegenerateList();
                return m_policies;
            }
        }

        /// <summary>
        /// <para>The active Sprite Packer policy for this project.</para>
        /// </summary>
        public static string SelectedPolicy
        {
            get
            {
                RegenerateList();
                return m_selectedPolicy;
            }
            set
            {
                RegenerateList();
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                if (!m_policies.Contains<string>(value))
                {
                    throw new ArgumentException("Specified policy {0} is not in the policy list.", value);
                }
                SetSelectedPolicy(value);
            }
        }

        /// <summary>
        /// <para>Sprite Packer execution mode.</para>
        /// </summary>
        public enum Execution
        {
            Normal,
            ForceRegroup
        }
    }
}

