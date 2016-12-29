namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class AssetPostprocessingInternal
    {
        [CompilerGenerated]
        private static Func<MethodInfo, bool> <>f__am$cache0;
        private static ArrayList m_ImportProcessors = null;
        private static ArrayList m_PostprocessorClasses = null;
        private static ArrayList m_PostprocessStack = null;

        private static IEnumerable<MethodInfo> AllPostProcessorMethodsNamed(string callbackName)
        {
            <AllPostProcessorMethodsNamed>c__AnonStorey0 storey = new <AllPostProcessorMethodsNamed>c__AnonStorey0 {
                callbackName = callbackName
            };
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<MethodInfo, bool>(null, (IntPtr) <AllPostProcessorMethodsNamed>m__0);
            }
            return Enumerable.Where<MethodInfo>(Enumerable.Select<Type, MethodInfo>(EditorAssemblies.SubclassesOf(typeof(AssetPostprocessor)), new Func<Type, MethodInfo>(storey, (IntPtr) this.<>m__0)), <>f__am$cache0);
        }

        internal static void CallOnGeneratedCSProjectFiles()
        {
            object[] parameters = new object[0];
            foreach (MethodInfo info in AllPostProcessorMethodsNamed("OnGeneratedCSProjectFiles"))
            {
                info.Invoke(null, parameters);
            }
        }

        private static void CleanupPostprocessors()
        {
            if (m_PostprocessStack != null)
            {
                m_PostprocessStack.RemoveAt(m_PostprocessStack.Count - 1);
                if (m_PostprocessStack.Count != 0)
                {
                    PostprocessStack stack = (PostprocessStack) m_PostprocessStack[m_PostprocessStack.Count - 1];
                    m_ImportProcessors = stack.m_ImportProcessors;
                }
            }
        }

        private static uint[] GetAudioProcessorVersions()
        {
            List<uint> list = new List<uint>();
            foreach (Type type in EditorAssemblies.SubclassesOf(typeof(AssetPostprocessor)))
            {
                try
                {
                    AssetPostprocessor postprocessor = Activator.CreateInstance(type) as AssetPostprocessor;
                    Type type2 = postprocessor.GetType();
                    bool flag = type2.GetMethod("OnPreprocessAudio", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) != null;
                    bool flag2 = type2.GetMethod("OnPostprocessAudio", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) != null;
                    uint version = postprocessor.GetVersion();
                    if ((version != 0) && (flag || flag2))
                    {
                        list.Add(version);
                    }
                }
                catch (MissingMethodException)
                {
                    LogPostProcessorMissingDefaultConstructor(type);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
            return list.ToArray();
        }

        private static ArrayList GetCachedAssetPostprocessorClasses()
        {
            if (m_PostprocessorClasses == null)
            {
                m_PostprocessorClasses = new ArrayList();
                foreach (Type type in EditorAssemblies.SubclassesOf(typeof(AssetPostprocessor)))
                {
                    m_PostprocessorClasses.Add(type);
                }
            }
            return m_PostprocessorClasses;
        }

        private static uint[] GetMeshProcessorVersions()
        {
            List<uint> list = new List<uint>();
            foreach (Type type in EditorAssemblies.SubclassesOf(typeof(AssetPostprocessor)))
            {
                try
                {
                    AssetPostprocessor postprocessor = Activator.CreateInstance(type) as AssetPostprocessor;
                    Type type2 = postprocessor.GetType();
                    bool flag = type2.GetMethod("OnPreprocessModel", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) != null;
                    bool flag2 = type2.GetMethod("OnProcessMeshAssingModel", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) != null;
                    bool flag3 = type2.GetMethod("OnPostprocessModel", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) != null;
                    uint version = postprocessor.GetVersion();
                    if ((version != 0) && ((flag || flag2) || flag3))
                    {
                        list.Add(version);
                    }
                }
                catch (MissingMethodException)
                {
                    LogPostProcessorMissingDefaultConstructor(type);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
            return list.ToArray();
        }

        private static uint[] GetTextureProcessorVersions()
        {
            List<uint> list = new List<uint>();
            foreach (Type type in EditorAssemblies.SubclassesOf(typeof(AssetPostprocessor)))
            {
                try
                {
                    AssetPostprocessor postprocessor = Activator.CreateInstance(type) as AssetPostprocessor;
                    Type type2 = postprocessor.GetType();
                    bool flag = type2.GetMethod("OnPreprocessTexture", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) != null;
                    bool flag2 = type2.GetMethod("OnPostprocessTexture", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) != null;
                    uint version = postprocessor.GetVersion();
                    if ((version != 0) && (flag || flag2))
                    {
                        list.Add(version);
                    }
                }
                catch (MissingMethodException)
                {
                    LogPostProcessorMissingDefaultConstructor(type);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
            return list.ToArray();
        }

        private static void InitPostprocessors(string pathName)
        {
            m_ImportProcessors = new ArrayList();
            IEnumerator enumerator = GetCachedAssetPostprocessorClasses().GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Type current = (Type) enumerator.Current;
                    try
                    {
                        AssetPostprocessor postprocessor = Activator.CreateInstance(current) as AssetPostprocessor;
                        postprocessor.assetPath = pathName;
                        m_ImportProcessors.Add(postprocessor);
                    }
                    catch (MissingMethodException)
                    {
                        LogPostProcessorMissingDefaultConstructor(current);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
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
            m_ImportProcessors.Sort(new CompareAssetImportPriority());
            PostprocessStack stack = new PostprocessStack {
                m_ImportProcessors = m_ImportProcessors
            };
            if (m_PostprocessStack == null)
            {
                m_PostprocessStack = new ArrayList();
            }
            m_PostprocessStack.Add(stack);
        }

        private static void LogPostProcessorMissingDefaultConstructor(Type type)
        {
            object[] args = new object[] { type };
            Debug.LogErrorFormat("{0} requires a default constructor to be used as an asset post processor", args);
        }

        internal static bool OnPreGeneratingCSProjectFiles()
        {
            object[] parameters = new object[0];
            bool flag = false;
            foreach (MethodInfo info in AllPostProcessorMethodsNamed("OnPreGeneratingCSProjectFiles"))
            {
                object obj2 = info.Invoke(null, parameters);
                if (info.ReturnType == typeof(bool))
                {
                    flag |= (bool) obj2;
                }
            }
            return flag;
        }

        private static void PostprocessAllAssets(string[] importedAssets, string[] addedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPathAssets)
        {
            object[] parameters = new object[] { importedAssets, deletedAssets, movedAssets, movedFromPathAssets };
            foreach (Type type in EditorAssemblies.SubclassesOf(typeof(AssetPostprocessor)))
            {
                MethodInfo method = type.GetMethod("OnPostprocessAllAssets", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                if (method != null)
                {
                    method.Invoke(null, parameters);
                }
            }
            SyncVS.PostprocessSyncProject(importedAssets, addedAssets, deletedAssets, movedAssets, movedFromPathAssets);
        }

        private static void PostprocessAssetbundleNameChanged(string assetPAth, string prevoiusAssetBundleName, string newAssetBundleName)
        {
            object[] args = new object[] { assetPAth, prevoiusAssetBundleName, newAssetBundleName };
            foreach (Type type in EditorAssemblies.SubclassesOf(typeof(AssetPostprocessor)))
            {
                AssetPostprocessor target = Activator.CreateInstance(type) as AssetPostprocessor;
                AttributeHelper.InvokeMemberIfAvailable(target, "OnPostprocessAssetbundleNameChanged", args);
            }
        }

        private static void PostprocessAudio(AudioClip tex, string pathName)
        {
            IEnumerator enumerator = m_ImportProcessors.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    AssetPostprocessor current = (AssetPostprocessor) enumerator.Current;
                    object[] args = new object[] { tex };
                    AttributeHelper.InvokeMemberIfAvailable(current, "OnPostprocessAudio", args);
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

        private static void PostprocessGameObjectWithUserProperties(GameObject go, string[] prop_names, object[] prop_values)
        {
            IEnumerator enumerator = m_ImportProcessors.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    AssetPostprocessor current = (AssetPostprocessor) enumerator.Current;
                    object[] args = new object[] { go, prop_names, prop_values };
                    AttributeHelper.InvokeMemberIfAvailable(current, "OnPostprocessGameObjectWithUserProperties", args);
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

        private static void PostprocessMesh(GameObject gameObject)
        {
            IEnumerator enumerator = m_ImportProcessors.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    AssetPostprocessor current = (AssetPostprocessor) enumerator.Current;
                    object[] args = new object[] { gameObject };
                    AttributeHelper.InvokeMemberIfAvailable(current, "OnPostprocessModel", args);
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

        private static void PostprocessSpeedTree(GameObject gameObject)
        {
            IEnumerator enumerator = m_ImportProcessors.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    AssetPostprocessor current = (AssetPostprocessor) enumerator.Current;
                    object[] args = new object[] { gameObject };
                    AttributeHelper.InvokeMemberIfAvailable(current, "OnPostprocessSpeedTree", args);
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

        private static void PostprocessSprites(Texture2D tex, string pathName, Sprite[] sprites)
        {
            IEnumerator enumerator = m_ImportProcessors.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    AssetPostprocessor current = (AssetPostprocessor) enumerator.Current;
                    object[] args = new object[] { tex, sprites };
                    AttributeHelper.InvokeMemberIfAvailable(current, "OnPostprocessSprites", args);
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

        private static void PostprocessTexture(Texture2D tex, string pathName)
        {
            IEnumerator enumerator = m_ImportProcessors.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    AssetPostprocessor current = (AssetPostprocessor) enumerator.Current;
                    object[] args = new object[] { tex };
                    AttributeHelper.InvokeMemberIfAvailable(current, "OnPostprocessTexture", args);
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

        private static void PreprocessAnimation(string pathName)
        {
            IEnumerator enumerator = m_ImportProcessors.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    AssetPostprocessor current = (AssetPostprocessor) enumerator.Current;
                    AttributeHelper.InvokeMemberIfAvailable(current, "OnPreprocessAnimation", null);
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

        private static void PreprocessAssembly(string pathName)
        {
            IEnumerator enumerator = m_ImportProcessors.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    AssetPostprocessor current = (AssetPostprocessor) enumerator.Current;
                    string[] args = new string[] { pathName };
                    AttributeHelper.InvokeMemberIfAvailable(current, "OnPreprocessAssembly", args);
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

        private static void PreprocessAudio(string pathName)
        {
            IEnumerator enumerator = m_ImportProcessors.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    AssetPostprocessor current = (AssetPostprocessor) enumerator.Current;
                    AttributeHelper.InvokeMemberIfAvailable(current, "OnPreprocessAudio", null);
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

        private static void PreprocessMesh(string pathName)
        {
            IEnumerator enumerator = m_ImportProcessors.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    AssetPostprocessor current = (AssetPostprocessor) enumerator.Current;
                    AttributeHelper.InvokeMemberIfAvailable(current, "OnPreprocessModel", null);
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

        private static void PreprocessSpeedTree(string pathName)
        {
            IEnumerator enumerator = m_ImportProcessors.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    AssetPostprocessor current = (AssetPostprocessor) enumerator.Current;
                    AttributeHelper.InvokeMemberIfAvailable(current, "OnPreprocessSpeedTree", null);
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

        private static void PreprocessTexture(string pathName)
        {
            IEnumerator enumerator = m_ImportProcessors.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    AssetPostprocessor current = (AssetPostprocessor) enumerator.Current;
                    AttributeHelper.InvokeMemberIfAvailable(current, "OnPreprocessTexture", null);
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

        private static Material ProcessMeshAssignMaterial(Renderer renderer, Material material)
        {
            IEnumerator enumerator = m_ImportProcessors.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    AssetPostprocessor current = (AssetPostprocessor) enumerator.Current;
                    object[] args = new object[] { material, renderer };
                    object obj2 = AttributeHelper.InvokeMemberIfAvailable(current, "OnAssignMaterialModel", args);
                    if (obj2 is Material)
                    {
                        return (obj2 as Material);
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
            return null;
        }

        private static bool ProcessMeshHasAssignMaterial()
        {
            IEnumerator enumerator = m_ImportProcessors.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    AssetPostprocessor current = (AssetPostprocessor) enumerator.Current;
                    if (current.GetType().GetMethod("OnAssignMaterialModel", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) != null)
                    {
                        return true;
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
            return false;
        }

        [CompilerGenerated]
        private sealed class <AllPostProcessorMethodsNamed>c__AnonStorey0
        {
            internal string callbackName;

            internal MethodInfo <>m__0(Type assetPostprocessorClass) => 
                assetPostprocessorClass.GetMethod(this.callbackName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        }

        internal class CompareAssetImportPriority : IComparer
        {
            int IComparer.Compare(object xo, object yo)
            {
                int postprocessOrder = ((AssetPostprocessor) xo).GetPostprocessOrder();
                int num2 = ((AssetPostprocessor) yo).GetPostprocessOrder();
                return postprocessOrder.CompareTo(num2);
            }
        }

        internal class PostprocessStack
        {
            internal ArrayList m_ImportProcessors = null;
        }
    }
}

