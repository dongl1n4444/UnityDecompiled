namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>AndroidJavaClass is the Unity representation of a generic instance of java.lang.Class.</para>
    /// </summary>
    public class AndroidJavaClass : AndroidJavaObject
    {
        internal AndroidJavaClass(IntPtr jclass)
        {
            if (jclass == IntPtr.Zero)
            {
                throw new Exception("JNI: Init'd AndroidJavaClass with null ptr!");
            }
            base.m_jclass = AndroidJNI.NewGlobalRef(jclass);
            base.m_jobject = IntPtr.Zero;
        }

        /// <summary>
        /// <para>Construct an AndroidJavaClass from the class name.</para>
        /// </summary>
        /// <param name="className">Specifies the Java class name (e.g. &lt;tt&gt;java.lang.String&lt;/tt&gt;).</param>
        public AndroidJavaClass(string className)
        {
            this._AndroidJavaClass(className);
        }

        private void _AndroidJavaClass(string className)
        {
            base.DebugPrint("Creating AndroidJavaClass from " + className);
            using (AndroidJavaObject obj2 = AndroidJavaObject.FindClass(className))
            {
                base.m_jclass = AndroidJNI.NewGlobalRef(obj2.GetRawObject());
                base.m_jobject = IntPtr.Zero;
            }
        }
    }
}

