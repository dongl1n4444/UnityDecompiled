namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    internal static class EditorAssemblies
    {
        [CompilerGenerated]
        private static Func<Assembly, IEnumerable<Type>> <>f__am$cache0;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static Assembly[] <loadedAssemblies>k__BackingField;
        internal static List<RuntimeInitializeClassInfo> m_RuntimeInitializeClassInfoList;
        internal static int m_TotalNumRuntimeInitializeMethods;

        [RequiredByNativeCode]
        private static RuntimeInitializeClassInfo[] GetRuntimeInitializeClassInfos() => 
            m_RuntimeInitializeClassInfoList?.ToArray();

        [RequiredByNativeCode]
        private static int GetTotalNumRuntimeInitializeMethods() => 
            m_TotalNumRuntimeInitializeMethods;

        private static void ProcessEditorInitializeOnLoad(Type type)
        {
            try
            {
                RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }
            catch (TypeInitializationException exception)
            {
                Debug.LogError(exception.InnerException);
            }
        }

        [RequiredByNativeCode]
        private static int[] ProcessInitializeOnLoadAttributes()
        {
            List<int> list = null;
            Assembly[] loadedAssemblies = EditorAssemblies.loadedAssemblies;
            m_TotalNumRuntimeInitializeMethods = 0;
            m_RuntimeInitializeClassInfoList = new List<RuntimeInitializeClassInfo>();
            for (int i = 0; i < loadedAssemblies.Length; i++)
            {
                int totalNumRuntimeInitializeMethods = m_TotalNumRuntimeInitializeMethods;
                int count = m_RuntimeInitializeClassInfoList.Count;
                try
                {
                    Type[] typesFromAssembly = AssemblyHelper.GetTypesFromAssembly(loadedAssemblies[i]);
                    foreach (Type type in typesFromAssembly)
                    {
                        if (type.IsDefined(typeof(InitializeOnLoadAttribute), false))
                        {
                            ProcessEditorInitializeOnLoad(type);
                        }
                        ProcessStaticMethodAttributes(type);
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                    if (list == null)
                    {
                        list = new List<int>();
                    }
                    if (totalNumRuntimeInitializeMethods != m_TotalNumRuntimeInitializeMethods)
                    {
                        m_TotalNumRuntimeInitializeMethods = totalNumRuntimeInitializeMethods;
                    }
                    if (count != m_RuntimeInitializeClassInfoList.Count)
                    {
                        m_RuntimeInitializeClassInfoList.RemoveRange(count, m_RuntimeInitializeClassInfoList.Count - count);
                    }
                    list.Add(i);
                }
            }
            return list?.ToArray();
        }

        private static void ProcessStaticMethodAttributes(Type type)
        {
            List<string> methodNames = null;
            List<RuntimeInitializeLoadType> loadTypes = null;
            MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            for (int i = 0; i < methods.GetLength(0); i++)
            {
                MethodInfo element = methods[i];
                if (Attribute.IsDefined(element, typeof(RuntimeInitializeOnLoadMethodAttribute)))
                {
                    RuntimeInitializeLoadType afterSceneLoad = RuntimeInitializeLoadType.AfterSceneLoad;
                    object[] customAttributes = element.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                    {
                        afterSceneLoad = ((RuntimeInitializeOnLoadMethodAttribute) customAttributes[0]).loadType;
                    }
                    if (methodNames == null)
                    {
                        methodNames = new List<string>();
                        loadTypes = new List<RuntimeInitializeLoadType>();
                    }
                    methodNames.Add(element.Name);
                    loadTypes.Add(afterSceneLoad);
                }
                if (Attribute.IsDefined(element, typeof(InitializeOnLoadMethodAttribute)))
                {
                    try
                    {
                        element.Invoke(null, null);
                    }
                    catch (TargetInvocationException exception)
                    {
                        Debug.LogError(exception.InnerException);
                    }
                }
            }
            if (methodNames != null)
            {
                StoreRuntimeInitializeClassInfo(type, methodNames, loadTypes);
            }
        }

        [RequiredByNativeCode]
        private static void SetLoadedEditorAssemblies(Assembly[] assemblies)
        {
            loadedAssemblies = assemblies;
        }

        private static void StoreRuntimeInitializeClassInfo(Type type, List<string> methodNames, List<RuntimeInitializeLoadType> loadTypes)
        {
            RuntimeInitializeClassInfo item = new RuntimeInitializeClassInfo {
                assemblyName = type.Assembly.GetName().Name.ToString(),
                className = type.ToString(),
                methodNames = methodNames.ToArray(),
                loadTypes = loadTypes.ToArray()
            };
            m_RuntimeInitializeClassInfoList.Add(item);
            m_TotalNumRuntimeInitializeMethods += methodNames.Count;
        }

        internal static IEnumerable<Type> SubclassesOf(Type parent)
        {
            <SubclassesOf>c__AnonStorey0 storey = new <SubclassesOf>c__AnonStorey0 {
                parent = parent
            };
            return Enumerable.Where<Type>(loadedTypes, new Func<Type, bool>(storey.<>m__0));
        }

        internal static Assembly[] loadedAssemblies
        {
            [CompilerGenerated]
            get => 
                <loadedAssemblies>k__BackingField;
            [CompilerGenerated]
            private set
            {
                <loadedAssemblies>k__BackingField = value;
            }
        }

        internal static IEnumerable<Type> loadedTypes
        {
            get
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = assembly => AssemblyHelper.GetTypesFromAssembly(assembly);
                }
                return Enumerable.SelectMany<Assembly, Type>(loadedAssemblies, <>f__am$cache0);
            }
        }

        [CompilerGenerated]
        private sealed class <SubclassesOf>c__AnonStorey0
        {
            internal Type parent;

            internal bool <>m__0(Type klass) => 
                klass.IsSubclassOf(this.parent);
        }
    }
}

