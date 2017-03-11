namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class RuntimeClassRegistry
    {
        protected Dictionary<int, string> allNativeClasses = new Dictionary<int, string>();
        protected BuildTarget buildTarget;
        protected Dictionary<int, List<string>> classScenes = new Dictionary<int, List<string>>();
        internal List<MethodDescription> m_MethodsToPreserve = new List<MethodDescription>();
        protected Dictionary<string, string[]> m_UsedTypesPerUserAssembly = new Dictionary<string, string[]>();
        internal List<string> m_UserAssemblies = new List<string>();
        protected HashSet<string> monoBaseClasses = new HashSet<string>();
        protected UnityType objectUnityType = null;

        protected void AddManagedBaseClass(string className)
        {
            this.monoBaseClasses.Add(className);
        }

        internal void AddMethodToPreserve(string assembly, string @namespace, string klassName, string methodName)
        {
            MethodDescription item = new MethodDescription {
                assembly = assembly,
                fullTypeName = @namespace + ((@namespace.Length <= 0) ? "" : ".") + klassName,
                methodName = methodName
            };
            this.m_MethodsToPreserve.Add(item);
        }

        protected void AddNativeClassFromName(string className)
        {
            if (this.objectUnityType == null)
            {
                this.objectUnityType = UnityType.FindTypeByName("Object");
            }
            UnityType type = UnityType.FindTypeByName(className);
            if ((type != null) && (type.persistentTypeID != this.objectUnityType.persistentTypeID))
            {
                this.allNativeClasses[type.persistentTypeID] = className;
            }
        }

        public void AddNativeClassID(int ID)
        {
            string name = UnityType.FindTypeByPersistentTypeID(ID).name;
            if (name.Length > 0)
            {
                this.allNativeClasses[ID] = name;
            }
        }

        internal void AddUserAssembly(string assembly)
        {
            if (!this.m_UserAssemblies.Contains(assembly))
            {
                this.m_UserAssemblies.Add(assembly);
            }
        }

        public static RuntimeClassRegistry Create() => 
            new RuntimeClassRegistry();

        public List<string> GetAllManagedBaseClassesAsString() => 
            new List<string>(this.monoBaseClasses);

        public List<string> GetAllNativeClassesIncludingManagersAsString() => 
            new List<string>(this.allNativeClasses.Values);

        internal List<MethodDescription> GetMethodsToPreserve() => 
            this.m_MethodsToPreserve;

        public List<string> GetScenesForClass(int ID)
        {
            if (!this.classScenes.ContainsKey(ID))
            {
                return null;
            }
            return this.classScenes[ID];
        }

        internal string[] GetUserAssemblies() => 
            this.m_UserAssemblies.ToArray();

        public void Initialize(int[] nativeClassIDs, BuildTarget buildTarget)
        {
            this.buildTarget = buildTarget;
            this.InitRuntimeClassRegistry();
            foreach (int num in nativeClassIDs)
            {
                this.AddNativeClassID(num);
            }
        }

        protected void InitRuntimeClassRegistry()
        {
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(this.buildTarget);
            this.AddManagedBaseClass("UnityEngine.MonoBehaviour");
            this.AddManagedBaseClass("UnityEngine.ScriptableObject");
            if (buildTargetGroup == BuildTargetGroup.Android)
            {
                this.AddManagedBaseClass("UnityEngine.AndroidJavaProxy");
            }
            string[] dontStripClassNames = RuntimeInitializeOnLoadManager.dontStripClassNames;
            foreach (string str in dontStripClassNames)
            {
                this.AddManagedBaseClass(str);
            }
        }

        public bool IsDLLUsed(string dll) => 
            ((this.m_UsedTypesPerUserAssembly == null) || ((Array.IndexOf<string>(CodeStrippingUtils.UserAssemblies, dll) != -1) || this.m_UsedTypesPerUserAssembly.ContainsKey(dll)));

        public void SetSceneClasses(int[] nativeClassIDs, string scene)
        {
            foreach (int num in nativeClassIDs)
            {
                this.AddNativeClassID(num);
                if (!this.classScenes.ContainsKey(num))
                {
                    this.classScenes[num] = new List<string>();
                }
                this.classScenes[num].Add(scene);
            }
        }

        public void SetUsedTypesInUserAssembly(string[] typeNames, string assemblyName)
        {
            this.m_UsedTypesPerUserAssembly[assemblyName] = typeNames;
        }

        public Dictionary<string, string[]> UsedTypePerUserAssembly =>
            this.m_UsedTypesPerUserAssembly;

        internal class MethodDescription
        {
            public string assembly;
            public string fullTypeName;
            public string methodName;
        }
    }
}

