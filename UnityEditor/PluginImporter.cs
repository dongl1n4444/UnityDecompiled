namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEditor.Callbacks;
    using UnityEditor.Modules;
    using UnityEditorInternal;

    /// <summary>
    /// <para>Represents plugin importer.</para>
    /// </summary>
    public sealed class PluginImporter : AssetImporter
    {
        /// <summary>
        /// <para>Clear all plugin settings and set the compatability with Any Platform to true.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void ClearSettings();
        /// <summary>
        /// <para>Returns all plugin importers for all platforms.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern PluginImporter[] GetAllImporters();
        /// <summary>
        /// <para>Is plugin comptabile with any platform.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool GetCompatibleWithAnyPlatform();
        /// <summary>
        /// <para>Is plugin compatible with editor.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool GetCompatibleWithEditor();
        /// <summary>
        /// <para>Is plugin compatible with specified platform.</para>
        /// </summary>
        /// <param name="platform">Target platform.</param>
        /// <param name="platformName"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool GetCompatibleWithPlatform(string platformName);
        /// <summary>
        /// <para>Is plugin compatible with specified platform.</para>
        /// </summary>
        /// <param name="platform">Target platform.</param>
        /// <param name="platformName"></param>
        public bool GetCompatibleWithPlatform(BuildTarget platform)
        {
            return this.GetCompatibleWithPlatform(BuildPipeline.GetBuildTargetName(platform));
        }

        /// <summary>
        /// <para>Returns editor specific data for specified key.</para>
        /// </summary>
        /// <param name="key">Key value for data.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern string GetEditorData(string key);
        /// <summary>
        /// <para>Is Editor excluded when Any Platform is set to true.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool GetExcludeEditorFromAnyPlatform();
        /// <summary>
        /// <para>Is platform excluded when Any Platform set to true.</para>
        /// </summary>
        /// <param name="platform">Target platform.</param>
        /// <param name="platformName"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool GetExcludeFromAnyPlatform(string platformName);
        /// <summary>
        /// <para>Is platform excluded when Any Platform set to true.</para>
        /// </summary>
        /// <param name="platform">Target platform.</param>
        /// <param name="platformName"></param>
        public bool GetExcludeFromAnyPlatform(BuildTarget platform)
        {
            return this.GetExcludeFromAnyPlatform(BuildPipeline.GetBuildTargetName(platform));
        }

        [DebuggerHidden]
        internal static IEnumerable<PluginDesc> GetExtensionPlugins(BuildTarget target)
        {
            return new <GetExtensionPlugins>c__Iterator0 { 
                target = target,
                $PC = -2
            };
        }

        /// <summary>
        /// <para>Returns all plugin importers for specfied platform.</para>
        /// </summary>
        /// <param name="platform">Target platform.</param>
        /// <param name="platformName">Name of the target platform.</param>
        public static PluginImporter[] GetImporters(string platformName)
        {
            <GetImporters>c__AnonStorey1 storey = new <GetImporters>c__AnonStorey1 {
                platformName = platformName
            };
            List<PluginImporter> list = new List<PluginImporter>();
            Dictionary<string, PluginImporter> dictionary = new Dictionary<string, PluginImporter>();
            PluginImporter[] importerArray = Enumerable.ToArray<PluginImporter>(Enumerable.Where<PluginImporter>(GetAllImporters(), new Func<PluginImporter, bool>(storey, (IntPtr) this.<>m__0)));
            IPluginImporterExtension pluginImporterExtension = ModuleManager.GetPluginImporterExtension(storey.platformName);
            if (pluginImporterExtension == null)
            {
                pluginImporterExtension = ModuleManager.GetPluginImporterExtension(BuildPipeline.GetBuildTargetByName(storey.platformName));
            }
            if (pluginImporterExtension == null)
            {
                return importerArray;
            }
            for (int i = 0; i < importerArray.Length; i++)
            {
                PluginImporter imp = importerArray[i];
                string str = pluginImporterExtension.CalculateFinalPluginPath(storey.platformName, imp);
                if (!string.IsNullOrEmpty(str))
                {
                    PluginImporter importer2;
                    if (!dictionary.TryGetValue(str, out importer2))
                    {
                        dictionary.Add(str, imp);
                    }
                    else if (importer2.GetIsOverridable() && !imp.GetIsOverridable())
                    {
                        dictionary[str] = imp;
                        list.Remove(importer2);
                    }
                    else if (imp.GetIsOverridable())
                    {
                        continue;
                    }
                }
                list.Add(imp);
            }
            return list.ToArray();
        }

        /// <summary>
        /// <para>Returns all plugin importers for specfied platform.</para>
        /// </summary>
        /// <param name="platform">Target platform.</param>
        /// <param name="platformName">Name of the target platform.</param>
        public static PluginImporter[] GetImporters(BuildTarget platform)
        {
            return GetImporters(BuildPipeline.GetBuildTargetName(platform));
        }

        /// <summary>
        /// <para>Identifies whether or not this plugin will be overridden if a plugin of the same name is placed in your project folder.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool GetIsOverridable();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool GetIsPreloaded();
        /// <summary>
        /// <para>Get platform specific data.</para>
        /// </summary>
        /// <param name="platform">Target platform.</param>
        /// <param name="key">Key value for data.</param>
        /// <param name="platformName"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern string GetPlatformData(string platformName, string key);
        /// <summary>
        /// <para>Get platform specific data.</para>
        /// </summary>
        /// <param name="platform">Target platform.</param>
        /// <param name="key">Key value for data.</param>
        /// <param name="platformName"></param>
        public string GetPlatformData(BuildTarget platform, string key)
        {
            return this.GetPlatformData(BuildPipeline.GetBuildTargetName(platform), key);
        }

        private static bool IsCompatible(PluginImporter imp, string platformName)
        {
            if (string.IsNullOrEmpty(imp.assetPath))
            {
                return false;
            }
            if (!imp.GetCompatibleWithPlatform(platformName) && (!imp.GetCompatibleWithAnyPlatform() || imp.GetExcludeFromAnyPlatform(platformName)))
            {
                return false;
            }
            return imp.ShouldIncludeInBuild();
        }

        /// <summary>
        /// <para>Set compatiblity with any platform.</para>
        /// </summary>
        /// <param name="enable">Is plugin compatible with any platform.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetCompatibleWithAnyPlatform(bool enable);
        /// <summary>
        /// <para>Set compatiblity with any editor.</para>
        /// </summary>
        /// <param name="enable">Is plugin compatible with editor.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetCompatibleWithEditor(bool enable);
        /// <summary>
        /// <para>Set compatiblity with specified platform.</para>
        /// </summary>
        /// <param name="platform">Target platform.</param>
        /// <param name="enable">Is plugin compatible with specified platform.</param>
        /// <param name="platformName">Target platform.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetCompatibleWithPlatform(string platformName, bool enable);
        /// <summary>
        /// <para>Set compatiblity with specified platform.</para>
        /// </summary>
        /// <param name="platform">Target platform.</param>
        /// <param name="enable">Is plugin compatible with specified platform.</param>
        /// <param name="platformName">Target platform.</param>
        public void SetCompatibleWithPlatform(BuildTarget platform, bool enable)
        {
            this.SetCompatibleWithPlatform(BuildPipeline.GetBuildTargetName(platform), enable);
        }

        /// <summary>
        /// <para>Set editor specific data.</para>
        /// </summary>
        /// <param name="key">Key value for data.</param>
        /// <param name="value">Data.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetEditorData(string key, string value);
        /// <summary>
        /// <para>Exclude Editor from compatible platforms when Any Platform is set to true.</para>
        /// </summary>
        /// <param name="excludedFromAny"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetExcludeEditorFromAnyPlatform(bool excludedFromAny);
        /// <summary>
        /// <para>Exclude platform from compatible platforms when Any Platform is set to true.</para>
        /// </summary>
        /// <param name="platformName">Target platform.</param>
        /// <param name="excludedFromAny"></param>
        /// <param name="platform"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetExcludeFromAnyPlatform(string platformName, bool excludedFromAny);
        /// <summary>
        /// <para>Exclude platform from compatible platforms when Any Platform is set to true.</para>
        /// </summary>
        /// <param name="platformName">Target platform.</param>
        /// <param name="excludedFromAny"></param>
        /// <param name="platform"></param>
        public void SetExcludeFromAnyPlatform(BuildTarget platform, bool excludedFromAny)
        {
            this.SetExcludeFromAnyPlatform(BuildPipeline.GetBuildTargetName(platform), excludedFromAny);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void SetIsPreloaded(bool isPreloaded);
        /// <summary>
        /// <para>Set platform specific data.</para>
        /// </summary>
        /// <param name="platform">Target platform.</param>
        /// <param name="key">Key value for data.</param>
        /// <param name="value">Data.</param>
        /// <param name="platformName"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetPlatformData(string platformName, string key, string value);
        /// <summary>
        /// <para>Set platform specific data.</para>
        /// </summary>
        /// <param name="platform">Target platform.</param>
        /// <param name="key">Key value for data.</param>
        /// <param name="value">Data.</param>
        /// <param name="platformName"></param>
        public void SetPlatformData(BuildTarget platform, string key, string value)
        {
            this.SetPlatformData(BuildPipeline.GetBuildTargetName(platform), key, value);
        }

        /// <summary>
        /// <para>Identifies whether or not this plugin should be included in the current build target.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool ShouldIncludeInBuild();

        internal DllType dllType { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Is plugin native or managed? Note: C++ libraries with CLR support are treated as native plugins, because Unity cannot load such libraries. You can still access them via P/Invoke.</para>
        /// </summary>
        public bool isNativePlugin { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        [CompilerGenerated]
        private sealed class <GetExtensionPlugins>c__Iterator0 : IEnumerable, IEnumerable<PluginDesc>, IEnumerator, IDisposable, IEnumerator<PluginDesc>
        {
            internal PluginDesc $current;
            internal bool $disposing;
            internal IEnumerator<IEnumerable<PluginDesc>> $locvar0;
            internal IEnumerator<PluginDesc> $locvar1;
            internal int $PC;
            internal IEnumerable<PluginDesc> <extensionPlugins>__1;
            internal PluginDesc <pluginDesc>__2;
            internal IEnumerable<IEnumerable<PluginDesc>> <pluginDescriptions>__0;
            internal BuildTarget target;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 1:
                        try
                        {
                            try
                            {
                            }
                            finally
                            {
                                if (this.$locvar1 != null)
                                {
                                    this.$locvar1.Dispose();
                                }
                            }
                        }
                        finally
                        {
                            if (this.$locvar0 != null)
                            {
                                this.$locvar0.Dispose();
                            }
                        }
                        break;
                }
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                    {
                        object[] arguments = new object[] { this.target };
                        this.<pluginDescriptions>__0 = AttributeHelper.CallMethodsWithAttribute<IEnumerable<PluginDesc>>(typeof(RegisterPluginsAttribute), arguments);
                        this.$locvar0 = this.<pluginDescriptions>__0.GetEnumerator();
                        num = 0xfffffffd;
                        break;
                    }
                    case 1:
                        break;

                    default:
                        goto Label_0148;
                }
                try
                {
                    switch (num)
                    {
                        case 1:
                            goto Label_009A;
                    }
                    while (this.$locvar0.MoveNext())
                    {
                        this.<extensionPlugins>__1 = this.$locvar0.Current;
                        this.$locvar1 = this.<extensionPlugins>__1.GetEnumerator();
                        num = 0xfffffffd;
                    Label_009A:
                        try
                        {
                            while (this.$locvar1.MoveNext())
                            {
                                this.<pluginDesc>__2 = this.$locvar1.Current;
                                this.$current = this.<pluginDesc>__2;
                                if (!this.$disposing)
                                {
                                    this.$PC = 1;
                                }
                                flag = true;
                                return true;
                            }
                        }
                        finally
                        {
                            if (!flag)
                            {
                            }
                            if (this.$locvar1 != null)
                            {
                                this.$locvar1.Dispose();
                            }
                        }
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    if (this.$locvar0 != null)
                    {
                        this.$locvar0.Dispose();
                    }
                }
                this.$PC = -1;
            Label_0148:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<PluginDesc> IEnumerable<PluginDesc>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new PluginImporter.<GetExtensionPlugins>c__Iterator0 { target = this.target };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<UnityEditorInternal.PluginDesc>.GetEnumerator();
            }

            PluginDesc IEnumerator<PluginDesc>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <GetImporters>c__AnonStorey1
        {
            internal string platformName;

            internal bool <>m__0(PluginImporter imp)
            {
                return PluginImporter.IsCompatible(imp, this.platformName);
            }
        }
    }
}

