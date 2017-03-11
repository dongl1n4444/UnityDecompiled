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
            HashSet<string> set = new HashSet<string> { 
                "UnityEditor",
                "UnityEngine.UI",
                "Unity.PackageManager",
                "UnityEngine.Networking",
                "nunit.framework",
                "UnityEditor.TreeEditor",
                "UnityEditor.Graphs",
                "UnityEditor.UI",
                "UnityEditor.TestRunner",
                "UnityEngine.TestRunner",
                "UnityEngine.HoloLens",
                "SyntaxTree.VisualStudio.Unity.Bridge",
                "UnityEditor.Android.Extensions"
            };
            BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            System.Type[] expectedArguments = new System.Type[] { typeof(BuildTarget), typeof(string) };
            for (int i = 0; i < EditorAssemblies.loadedAssemblies.Length; i++)
            {
                Assembly assembly = EditorAssemblies.loadedAssemblies[i];
                bool flag = !set.Contains(assembly.FullName.Substring(0, assembly.FullName.IndexOf(',')));
                System.Type[] types = null;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException exception)
                {
                    types = exception.Types;
                }
                for (int j = 0; j < types.Length; j++)
                {
                    System.Type c = types[j];
                    if (c != null)
                    {
                        object o = null;
                        bool flag2 = false;
                        if (findBuildProcessors)
                        {
                            flag2 = typeof(IOrderedCallback).IsAssignableFrom(c);
                            if (flag2)
                            {
                                if ((!c.IsInterface && typeof(IPreprocessBuild).IsAssignableFrom(c)) && (c != typeof(AttributeCallbackWrapper)))
                                {
                                    o = Activator.CreateInstance(c);
                                    AddToList<IPreprocessBuild>(o, ref buildPreprocessors);
                                }
                                if ((!c.IsInterface && typeof(IPostprocessBuild).IsAssignableFrom(c)) && (c != typeof(AttributeCallbackWrapper)))
                                {
                                    o = (o != null) ? o : Activator.CreateInstance(c);
                                    AddToList<IPostprocessBuild>(o, ref buildPostprocessors);
                                }
                            }
                        }
                        if ((findSceneProcessors && (!findBuildProcessors || flag2)) && ((!c.IsInterface && typeof(IProcessScene).IsAssignableFrom(c)) && (c != typeof(AttributeCallbackWrapper))))
                        {
                            o = (o != null) ? o : Activator.CreateInstance(c);
                            AddToList<IProcessScene>(o, ref sceneProcessors);
                        }
                        if (flag)
                        {
                            foreach (MethodInfo info in c.GetMethods(bindingAttr))
                            {
                                if (!info.IsSpecialName)
                                {
                                    if (findBuildProcessors && ValidateMethod(info, typeof(PostProcessBuildAttribute), expectedArguments))
                                    {
                                        AddToList<IPostprocessBuild>(new AttributeCallbackWrapper(info), ref buildPostprocessors);
                                    }
                                    if (findSceneProcessors && ValidateMethod(info, typeof(PostProcessSceneAttribute), System.Type.EmptyTypes))
                                    {
                                        AddToList<IProcessScene>(new AttributeCallbackWrapper(info), ref sceneProcessors);
                                    }
                                }
                            }
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

        private static bool ValidateMethod(MethodInfo method, System.Type attribute, System.Type[] expectedArguments)
        {
            if (!method.IsDefined(attribute, false))
            {
                return false;
            }
            if (!method.IsStatic)
            {
                string str = attribute.Name.Replace("Attribute", "");
                object[] args = new object[] { method.Name, str };
                Debug.LogErrorFormat("Method {0} with {1} attribute must be static.", args);
                return false;
            }
            if (method.IsGenericMethod || method.IsGenericMethodDefinition)
            {
                string str2 = attribute.Name.Replace("Attribute", "");
                object[] objArray2 = new object[] { method.Name, str2 };
                Debug.LogErrorFormat("Method {0} with {1} attribute cannot be generic.", objArray2);
                return false;
            }
            System.Reflection.ParameterInfo[] parameters = method.GetParameters();
            bool flag2 = parameters.Length == expectedArguments.Length;
            if (flag2)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].ParameterType != expectedArguments[i])
                    {
                        flag2 = false;
                        break;
                    }
                }
            }
            if (!flag2)
            {
                string str3 = attribute.Name.Replace("Attribute", "");
                string str4 = "static void " + method.Name + "(";
                for (int j = 0; j < expectedArguments.Length; j++)
                {
                    str4 = str4 + expectedArguments[j].Name;
                    if (j != (expectedArguments.Length - 1))
                    {
                        str4 = str4 + ", ";
                    }
                }
                str4 = str4 + ")";
                object[] objArray3 = new object[] { method.Name, str3, str4 };
                Debug.LogErrorFormat("Method {0} with {1} attribute does not have the correct signature, expected: {2}.", objArray3);
                return false;
            }
            return true;
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

