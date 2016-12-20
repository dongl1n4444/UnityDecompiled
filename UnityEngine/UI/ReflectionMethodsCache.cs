namespace UnityEngine.UI
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngineInternal;

    internal class ReflectionMethodsCache
    {
        public GetRayIntersectionAllCallback getRayIntersectionAll = null;
        public Raycast2DCallback raycast2D = null;
        public Raycast3DCallback raycast3D = null;
        public RaycastAllCallback raycast3DAll = null;
        private static ReflectionMethodsCache s_ReflectionMethodsCache = null;

        public ReflectionMethodsCache()
        {
            System.Type[] types = new System.Type[] { typeof(Ray), typeof(RaycastHit).MakeByRefType(), typeof(float), typeof(int) };
            MethodInfo method = typeof(Physics).GetMethod("Raycast", types);
            if (method != null)
            {
                this.raycast3D = (Raycast3DCallback) ScriptingUtils.CreateDelegate(typeof(Raycast3DCallback), method);
            }
            System.Type[] typeArray2 = new System.Type[] { typeof(Vector2), typeof(Vector2), typeof(float), typeof(int) };
            MethodInfo methodInfo = typeof(Physics2D).GetMethod("Raycast", typeArray2);
            if (methodInfo != null)
            {
                this.raycast2D = (Raycast2DCallback) ScriptingUtils.CreateDelegate(typeof(Raycast2DCallback), methodInfo);
            }
            System.Type[] typeArray3 = new System.Type[] { typeof(Ray), typeof(float), typeof(int) };
            MethodInfo info3 = typeof(Physics).GetMethod("RaycastAll", typeArray3);
            if (info3 != null)
            {
                this.raycast3DAll = (RaycastAllCallback) ScriptingUtils.CreateDelegate(typeof(RaycastAllCallback), info3);
            }
            System.Type[] typeArray4 = new System.Type[] { typeof(Ray), typeof(float), typeof(int) };
            MethodInfo info4 = typeof(Physics2D).GetMethod("GetRayIntersectionAll", typeArray4);
            if (info4 != null)
            {
                this.getRayIntersectionAll = (GetRayIntersectionAllCallback) ScriptingUtils.CreateDelegate(typeof(GetRayIntersectionAllCallback), info4);
            }
        }

        public static ReflectionMethodsCache Singleton
        {
            get
            {
                if (s_ReflectionMethodsCache == null)
                {
                    s_ReflectionMethodsCache = new ReflectionMethodsCache();
                }
                return s_ReflectionMethodsCache;
            }
        }

        public delegate RaycastHit2D[] GetRayIntersectionAllCallback(Ray r, float f, int i);

        public delegate RaycastHit2D Raycast2DCallback(Vector2 p1, Vector2 p2, float f, int i);

        public delegate bool Raycast3DCallback(Ray r, out RaycastHit hit, float f, int i);

        public delegate RaycastHit[] RaycastAllCallback(Ray r, float f, int i);
    }
}

