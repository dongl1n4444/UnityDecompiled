namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Reflection;

    public static class AssemblyPortable
    {
        public static Assembly GetAssemblyPortable(Type type) => 
            type.GetAssemblyPortable();

        public static Assembly GetExecutingAssemblyPortable() => 
            Assembly.GetCallingAssembly();

        public static Assembly LoadFilePortable(string path) => 
            Assembly.LoadFile(path);

        public static Assembly LoadFromPortable(string assemblyFile) => 
            Assembly.LoadFrom(assemblyFile);

        public static Assembly LoadPortable(string partialName) => 
            Assembly.Load(partialName);
    }
}

