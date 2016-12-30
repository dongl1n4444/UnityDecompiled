namespace UnityEditor.Build
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Callbacks;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.Scripting;

    internal static class BuildPipelineInterfaces
    {
        [CompilerGenerated]
        private static Comparison<IPreprocessBuild> <>f__mg$cache0;
        [CompilerGenerated]
        private static Comparison<IPostprocessBuild> <>f__mg$cache1;
        [CompilerGenerated]
        private static Comparison<IProcessScene> <>f__mg$cache2;
        private static List<IPostprocessBuild> buildPostprocessors;
        private static List<IPreprocessBuild> buildPreprocessors;
        private static List<IProcessScene> sceneProcessors;

        private static void AddToList<T>(object o, ref List<T> list) where T: class
        {
            T item = o as T;
            if (item != null)
            {
                if (list == null)
                {
                    list = new List<T>();
                }
                list.Add(item);
            }
        }

        [RequiredByNativeCode]
        internal static void CleanupBuildCallbacks()
        {
            buildPreprocessors = null;
            buildPostprocessors = null;
            sceneProcessors = null;
        }

        private static int CompareICallbackOrder(IOrderedCallback a, IOrderedCallback b) => 
            (a.callbackOrder - b.callbackOrder);

        [RequiredByNativeCode]
        internal static void InitializeBuildCallbacks(bool findBuildProcessors, bool findSceneProcessors)
        {
            CleanupBuildCallbacks();
            List<Type> results = new List<Type>();
            if (findBuildProcessors && findSceneProcessors)
            {
                Type[] interfaces = new Type[] { typeof(IPreprocessBuild), typeof(IPostprocessBuild), typeof(IProcessScene) };
                EditorAssemblies.FindClassesThatImplementAnyInterface(results, interfaces);
            }
            else if (findBuildProcessors)
            {
                Type[] typeArray2 = new Type[] { typeof(IPreprocessBuild), typeof(IPostprocessBuild) };
                EditorAssemblies.FindClassesThatImplementAnyInterface(results, typeArray2);
            }
            else if (findSceneProcessors)
            {
                Type[] typeArray3 = new Type[] { typeof(IProcessScene) };
                EditorAssemblies.FindClassesThatImplementAnyInterface(results, typeArray3);
            }
            foreach (Type type in results)
            {
                try
                {
                    if (type != typeof(AttributeCallbackWrapper))
                    {
                        object o = Activator.CreateInstance(type);
                        if (findBuildProcessors)
                        {
                            AddToList<IPreprocessBuild>(o, ref buildPreprocessors);
                            AddToList<IPostprocessBuild>(o, ref buildPostprocessors);
                        }
                        if (findSceneProcessors)
                        {
                            AddToList<IProcessScene>(o, ref sceneProcessors);
                        }
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
            foreach (Type type2 in EditorAssemblies.loadedTypes)
            {
                foreach (MethodInfo info in type2.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    if (findBuildProcessors && info.IsDefined(typeof(PostProcessBuildAttribute), false))
                    {
                        if (!info.IsStatic)
                        {
                            object[] args = new object[] { info.Name };
                            Debug.LogErrorFormat("Method {0} with PostProcessBuild attribute must be static.", args);
                            continue;
                        }
                        if (info.IsGenericMethod || info.IsGenericMethodDefinition)
                        {
                            object[] objArray2 = new object[] { info.Name };
                            Debug.LogErrorFormat("Method {0} with PostProcessBuild attribute cannot be generic.", objArray2);
                            continue;
                        }
                        ParameterInfo[] parameters = info.GetParameters();
                        if (((parameters.Length != 2) || (parameters[0].ParameterType != typeof(BuildTarget))) || (parameters[1].ParameterType != typeof(string)))
                        {
                            object[] objArray3 = new object[] { info.Name };
                            Debug.LogErrorFormat("Method {0} with PostProcessBuild attribute does not have the correct signature, expected: static void {0}(BuildTarget target, string path).", objArray3);
                            continue;
                        }
                        AddToList<IPostprocessBuild>(new AttributeCallbackWrapper(info), ref buildPostprocessors);
                    }
                    if (findSceneProcessors && info.IsDefined(typeof(PostProcessSceneAttribute), false))
                    {
                        if (!info.IsStatic)
                        {
                            object[] objArray4 = new object[] { info.Name };
                            Debug.LogErrorFormat("Method {0} with PostProcessBuild attribute must be static.", objArray4);
                        }
                        else if (info.IsGenericMethod || info.IsGenericMethodDefinition)
                        {
                            object[] objArray5 = new object[] { info.Name };
                            Debug.LogErrorFormat("Method {0} with PostProcessScene attribute cannot be generic.", objArray5);
                        }
                        else if (info.GetParameters().Length != 0)
                        {
                            object[] objArray6 = new object[] { info.Name };
                            Debug.LogErrorFormat("Method {0} with PostProcessScene attribute does not have the correct signature, expected: static void {0}().", objArray6);
                        }
                        else
                        {
                            AddToList<IProcessScene>(new AttributeCallbackWrapper(info), ref sceneProcessors);
                        }
                    }
                }
            }
            if (buildPreprocessors != null)
            {
                if (<>f__mg$cache0 == null)
                {
                    <>f__mg$cache0 = new Comparison<IPreprocessBuild>(BuildPipelineInterfaces.CompareICallbackOrder);
                }
                buildPreprocessors.Sort(<>f__mg$cache0);
            }
            if (buildPostprocessors != null)
            {
                if (<>f__mg$cache1 == null)
                {
                    <>f__mg$cache1 = new Comparison<IPostprocessBuild>(BuildPipelineInterfaces.CompareICallbackOrder);
                }
                buildPostprocessors.Sort(<>f__mg$cache1);
            }
            if (sceneProcessors != null)
            {
                if (<>f__mg$cache2 == null)
                {
                    <>f__mg$cache2 = new Comparison<IProcessScene>(BuildPipelineInterfaces.CompareICallbackOrder);
                }
                sceneProcessors.Sort(<>f__mg$cache2);
            }
        }

        [RequiredByNativeCode]
        internal static void OnBuildPostProcess(BuildTarget platform, string path, bool strict)
        {
            if (buildPostprocessors != null)
            {
                foreach (IPostprocessBuild build in buildPostprocessors)
                {
                    try
                    {
                        build.OnPostprocessBuild(platform, path);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
                        if (strict)
                        {
                            break;
                        }
                    }
                }
            }
        }

        [RequiredByNativeCode]
        internal static void OnBuildPreProcess(BuildTarget platform, string path, bool strict)
        {
            if (buildPreprocessors != null)
            {
                foreach (IPreprocessBuild build in buildPreprocessors)
                {
                    try
                    {
                        build.OnPreprocessBuild(platform, path);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
                        if (strict)
                        {
                            break;
                        }
                    }
                }
            }
        }

        [RequiredByNativeCode]
        internal static void OnSceneProcess(Scene scene, bool strict)
        {
            if (sceneProcessors != null)
            {
                foreach (IProcessScene scene2 in sceneProcessors)
                {
                    try
                    {
                        scene2.OnProcessScene(scene);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
                        if (strict)
                        {
                            break;
                        }
                    }
                }
            }
        }

        private class AttributeCallbackWrapper : IPostprocessBuild, IProcessScene, IOrderedCallback
        {
            private int m_callbackOrder;
            private MethodInfo m_method;

            public AttributeCallbackWrapper(MethodInfo m)
            {
                this.m_callbackOrder = ((CallbackOrderAttribute) Attribute.GetCustomAttribute(m, typeof(CallbackOrderAttribute))).callbackOrder;
                this.m_method = m;
            }

            public void OnPostprocessBuild(BuildTarget target, string path)
            {
                object[] parameters = new object[] { target, path };
                this.m_method.Invoke(null, parameters);
            }

            public void OnProcessScene(Scene scene)
            {
                this.m_method.Invoke(null, null);
            }

            public int callbackOrder =>
                this.m_callbackOrder;
        }
    }
}

