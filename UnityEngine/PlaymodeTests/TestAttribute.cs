namespace UnityEngine.PlaymodeTests
{
    using System;
    using System.IO;
    using System.Reflection;
    using UnityEngine.PlaymodeTestsRunner;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class TestAttribute : Attribute
    {
        internal const float kDefaultTimeout = 5f;
        private string m_SceneName;
        private TestPlatform m_TestPlatform;
        private float m_Timeout;

        public TestAttribute()
        {
            this.m_Timeout = 5f;
            this.m_TestPlatform = TestPlatform.All;
        }

        public TestAttribute(float timeout) : this(TestPlatform.All, null, timeout)
        {
        }

        public TestAttribute(string scene) : this(TestPlatform.All, scene, 5f)
        {
        }

        public TestAttribute(TestPlatform testPlatform) : this(testPlatform, null, 5f)
        {
        }

        public TestAttribute(TestPlatform testPlatform, string scene, float timeout)
        {
            this.m_Timeout = 5f;
            this.m_TestPlatform = TestPlatform.All;
            this.m_TestPlatform = testPlatform;
            this.m_Timeout = timeout;
            if (!string.IsNullOrEmpty(scene))
            {
                if (scene.EndsWith(".unity"))
                {
                    scene = scene.Substring(0, scene.Length - ".unity".Length);
                }
                this.m_SceneName = scene;
            }
        }

        internal static PlayModeTestAttribute GetAttribute(MethodInfo method)
        {
            if (!Attribute.IsDefined(method, typeof(PlayModeTestAttribute)))
            {
                return null;
            }
            return (Attribute.GetCustomAttribute(method, typeof(PlayModeTestAttribute)) as PlayModeTestAttribute);
        }

        internal static PlayModeTestAttribute GetAttribute(Type type)
        {
            if (!Attribute.IsDefined(type, typeof(PlayModeTestAttribute)))
            {
                return null;
            }
            return (Attribute.GetCustomAttribute(type, typeof(PlayModeTestAttribute)) as PlayModeTestAttribute);
        }

        internal float GetTimeout()
        {
            return this.m_Timeout;
        }

        internal bool IncludeOnPlatform(TestPlatform testPlatform)
        {
            return ((this.m_TestPlatform & testPlatform) == testPlatform);
        }

        internal bool IncludeOnScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                return false;
            }
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(sceneName);
            return (this.m_SceneName == fileNameWithoutExtension);
        }

        internal bool IsSceneBased()
        {
            return !string.IsNullOrEmpty(this.m_SceneName);
        }
    }
}

