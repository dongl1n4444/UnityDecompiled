namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>'Raw' JNI interface to Android Dalvik (Java) VM from Mono (CS/JS).
    /// 
    /// Note: Using raw JNI functions requires advanced knowledge of the Android Java Native Interface (JNI). Please take note.</para>
    /// </summary>
    public sealed class AndroidJNI
    {
        private AndroidJNI()
        {
        }

        /// <summary>
        /// <para>Allocates a new Java object without invoking any of the constructors for the object.</para>
        /// </summary>
        /// <param name="clazz"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr AllocObject(IntPtr clazz)
        {
            IntPtr ptr;
            INTERNAL_CALL_AllocObject(clazz, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Attaches the current thread to a Java (Dalvik) VM.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern int AttachCurrentThread();
        /// <summary>
        /// <para>Calls an instance (nonstatic) Java method defined by &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodID"></param>
        /// <param name="args"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern bool CallBooleanMethod(IntPtr obj, IntPtr methodID, jvalue[] args);
        /// <summary>
        /// <para>Calls an instance (nonstatic) Java method defined by &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodID"></param>
        /// <param name="args"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern byte CallByteMethod(IntPtr obj, IntPtr methodID, jvalue[] args);
        /// <summary>
        /// <para>Calls an instance (nonstatic) Java method defined by &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodID"></param>
        /// <param name="args"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern char CallCharMethod(IntPtr obj, IntPtr methodID, jvalue[] args);
        /// <summary>
        /// <para>Calls an instance (nonstatic) Java method defined by &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodID"></param>
        /// <param name="args"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern double CallDoubleMethod(IntPtr obj, IntPtr methodID, jvalue[] args);
        /// <summary>
        /// <para>Calls an instance (nonstatic) Java method defined by &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodID"></param>
        /// <param name="args"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern float CallFloatMethod(IntPtr obj, IntPtr methodID, jvalue[] args);
        /// <summary>
        /// <para>Calls an instance (nonstatic) Java method defined by &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodID"></param>
        /// <param name="args"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern int CallIntMethod(IntPtr obj, IntPtr methodID, jvalue[] args);
        /// <summary>
        /// <para>Calls an instance (nonstatic) Java method defined by &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodID"></param>
        /// <param name="args"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern long CallLongMethod(IntPtr obj, IntPtr methodID, jvalue[] args);
        /// <summary>
        /// <para>Calls an instance (nonstatic) Java method defined by &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodID"></param>
        /// <param name="args"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr CallObjectMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
        {
            IntPtr ptr;
            INTERNAL_CALL_CallObjectMethod(obj, methodID, args, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Calls an instance (nonstatic) Java method defined by &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodID"></param>
        /// <param name="args"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern short CallShortMethod(IntPtr obj, IntPtr methodID, jvalue[] args);
        /// <summary>
        /// <para>Invokes a static method on a Java object, according to the specified &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="methodID"></param>
        /// <param name="args"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern bool CallStaticBooleanMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);
        /// <summary>
        /// <para>Invokes a static method on a Java object, according to the specified &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="methodID"></param>
        /// <param name="args"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern byte CallStaticByteMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);
        /// <summary>
        /// <para>Invokes a static method on a Java object, according to the specified &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="methodID"></param>
        /// <param name="args"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern char CallStaticCharMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);
        /// <summary>
        /// <para>Invokes a static method on a Java object, according to the specified &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="methodID"></param>
        /// <param name="args"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern double CallStaticDoubleMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);
        /// <summary>
        /// <para>Invokes a static method on a Java object, according to the specified &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="methodID"></param>
        /// <param name="args"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern float CallStaticFloatMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);
        /// <summary>
        /// <para>Invokes a static method on a Java object, according to the specified &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="methodID"></param>
        /// <param name="args"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern int CallStaticIntMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);
        /// <summary>
        /// <para>Invokes a static method on a Java object, according to the specified &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="methodID"></param>
        /// <param name="args"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern long CallStaticLongMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);
        /// <summary>
        /// <para>Invokes a static method on a Java object, according to the specified &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="methodID"></param>
        /// <param name="args"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr CallStaticObjectMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
        {
            IntPtr ptr;
            INTERNAL_CALL_CallStaticObjectMethod(clazz, methodID, args, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Invokes a static method on a Java object, according to the specified &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="methodID"></param>
        /// <param name="args"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern short CallStaticShortMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);
        /// <summary>
        /// <para>Invokes a static method on a Java object, according to the specified &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="methodID"></param>
        /// <param name="args"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern string CallStaticStringMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);
        /// <summary>
        /// <para>Invokes a static method on a Java object, according to the specified &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="methodID"></param>
        /// <param name="args"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void CallStaticVoidMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);
        /// <summary>
        /// <para>Calls an instance (nonstatic) Java method defined by &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodID"></param>
        /// <param name="args"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern string CallStringMethod(IntPtr obj, IntPtr methodID, jvalue[] args);
        /// <summary>
        /// <para>Calls an instance (nonstatic) Java method defined by &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodID"></param>
        /// <param name="args"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void CallVoidMethod(IntPtr obj, IntPtr methodID, jvalue[] args);
        /// <summary>
        /// <para>Deletes the global reference pointed to by &lt;tt&gt;obj&lt;/tt&gt;.</para>
        /// </summary>
        /// <param name="obj"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void DeleteGlobalRef(IntPtr obj);
        /// <summary>
        /// <para>Deletes the local reference pointed to by &lt;tt&gt;obj&lt;/tt&gt;.</para>
        /// </summary>
        /// <param name="obj"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void DeleteLocalRef(IntPtr obj);
        /// <summary>
        /// <para>Detaches the current thread from a Java (Dalvik) VM.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern int DetachCurrentThread();
        /// <summary>
        /// <para>Ensures that at least a given number of local references can be created in the current thread.</para>
        /// </summary>
        /// <param name="capacity"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern int EnsureLocalCapacity(int capacity);
        /// <summary>
        /// <para>Clears any exception that is currently being thrown.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void ExceptionClear();
        /// <summary>
        /// <para>Prints an exception and a backtrace of the stack to the &lt;tt&gt;logcat&lt;/tt&gt;</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void ExceptionDescribe();
        /// <summary>
        /// <para>Determines if an exception is being thrown.</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public static IntPtr ExceptionOccurred()
        {
            IntPtr ptr;
            INTERNAL_CALL_ExceptionOccurred(out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Raises a fatal error and does not expect the VM to recover. This function does not return.</para>
        /// </summary>
        /// <param name="message"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void FatalError(string message);
        /// <summary>
        /// <para>This function loads a locally-defined class.</para>
        /// </summary>
        /// <param name="name"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr FindClass(string name)
        {
            IntPtr ptr;
            INTERNAL_CALL_FindClass(name, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Convert a Java array of &lt;tt&gt;boolean&lt;/tt&gt; to a managed array of System.Boolean.</para>
        /// </summary>
        /// <param name="array"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern bool[] FromBooleanArray(IntPtr array);
        /// <summary>
        /// <para>Convert a Java array of &lt;tt&gt;byte&lt;/tt&gt; to a managed array of System.Byte.</para>
        /// </summary>
        /// <param name="array"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern byte[] FromByteArray(IntPtr array);
        /// <summary>
        /// <para>Convert a Java array of &lt;tt&gt;char&lt;/tt&gt; to a managed array of System.Char.</para>
        /// </summary>
        /// <param name="array"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern char[] FromCharArray(IntPtr array);
        /// <summary>
        /// <para>Convert a Java array of &lt;tt&gt;double&lt;/tt&gt; to a managed array of System.Double.</para>
        /// </summary>
        /// <param name="array"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern double[] FromDoubleArray(IntPtr array);
        /// <summary>
        /// <para>Convert a Java array of &lt;tt&gt;float&lt;/tt&gt; to a managed array of System.Single.</para>
        /// </summary>
        /// <param name="array"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern float[] FromFloatArray(IntPtr array);
        /// <summary>
        /// <para>Convert a Java array of &lt;tt&gt;int&lt;/tt&gt; to a managed array of System.Int32.</para>
        /// </summary>
        /// <param name="array"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern int[] FromIntArray(IntPtr array);
        /// <summary>
        /// <para>Convert a Java array of &lt;tt&gt;long&lt;/tt&gt; to a managed array of System.Int64.</para>
        /// </summary>
        /// <param name="array"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern long[] FromLongArray(IntPtr array);
        /// <summary>
        /// <para>Convert a Java array of &lt;tt&gt;java.lang.Object&lt;/tt&gt; to a managed array of System.IntPtr, representing Java objects.</para>
        /// </summary>
        /// <param name="array"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern IntPtr[] FromObjectArray(IntPtr array);
        /// <summary>
        /// <para>Converts a &lt;tt&gt;java.lang.reflect.Field&lt;/tt&gt; to a field ID.</para>
        /// </summary>
        /// <param name="refField"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr FromReflectedField(IntPtr refField)
        {
            IntPtr ptr;
            INTERNAL_CALL_FromReflectedField(refField, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Converts a &lt;tt&gt;java.lang.reflect.Method&lt;tt&gt; or &lt;tt&gt;java.lang.reflect.Constructor&lt;tt&gt; object to a method ID.</para>
        /// </summary>
        /// <param name="refMethod"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr FromReflectedMethod(IntPtr refMethod)
        {
            IntPtr ptr;
            INTERNAL_CALL_FromReflectedMethod(refMethod, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Convert a Java array of &lt;tt&gt;short&lt;/tt&gt; to a managed array of System.Int16.</para>
        /// </summary>
        /// <param name="array"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern short[] FromShortArray(IntPtr array);
        /// <summary>
        /// <para>Returns the number of elements in the array.</para>
        /// </summary>
        /// <param name="array"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern int GetArrayLength(IntPtr array);
        /// <summary>
        /// <para>Returns the value of one element of a primitive array.</para>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern bool GetBooleanArrayElement(IntPtr array, int index);
        /// <summary>
        /// <para>This function returns the value of an instance (nonstatic) field of an object.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldID"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern bool GetBooleanField(IntPtr obj, IntPtr fieldID);
        /// <summary>
        /// <para>Returns the value of one element of a primitive array.</para>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern byte GetByteArrayElement(IntPtr array, int index);
        /// <summary>
        /// <para>This function returns the value of an instance (nonstatic) field of an object.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldID"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern byte GetByteField(IntPtr obj, IntPtr fieldID);
        /// <summary>
        /// <para>Returns the value of one element of a primitive array.</para>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern char GetCharArrayElement(IntPtr array, int index);
        /// <summary>
        /// <para>This function returns the value of an instance (nonstatic) field of an object.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldID"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern char GetCharField(IntPtr obj, IntPtr fieldID);
        /// <summary>
        /// <para>Returns the value of one element of a primitive array.</para>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern double GetDoubleArrayElement(IntPtr array, int index);
        /// <summary>
        /// <para>This function returns the value of an instance (nonstatic) field of an object.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldID"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern double GetDoubleField(IntPtr obj, IntPtr fieldID);
        /// <summary>
        /// <para>Returns the field ID for an instance (nonstatic) field of a class.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="name"></param>
        /// <param name="sig"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr GetFieldID(IntPtr clazz, string name, string sig)
        {
            IntPtr ptr;
            INTERNAL_CALL_GetFieldID(clazz, name, sig, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Returns the value of one element of a primitive array.</para>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern float GetFloatArrayElement(IntPtr array, int index);
        /// <summary>
        /// <para>This function returns the value of an instance (nonstatic) field of an object.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldID"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern float GetFloatField(IntPtr obj, IntPtr fieldID);
        /// <summary>
        /// <para>Returns the value of one element of a primitive array.</para>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern int GetIntArrayElement(IntPtr array, int index);
        /// <summary>
        /// <para>This function returns the value of an instance (nonstatic) field of an object.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldID"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern int GetIntField(IntPtr obj, IntPtr fieldID);
        /// <summary>
        /// <para>Returns the value of one element of a primitive array.</para>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern long GetLongArrayElement(IntPtr array, int index);
        /// <summary>
        /// <para>This function returns the value of an instance (nonstatic) field of an object.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldID"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern long GetLongField(IntPtr obj, IntPtr fieldID);
        /// <summary>
        /// <para>Returns the method ID for an instance (nonstatic) method of a class or interface.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="name"></param>
        /// <param name="sig"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr GetMethodID(IntPtr clazz, string name, string sig)
        {
            IntPtr ptr;
            INTERNAL_CALL_GetMethodID(clazz, name, sig, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Returns an element of an &lt;tt&gt;Object&lt;/tt&gt; array.</para>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr GetObjectArrayElement(IntPtr array, int index)
        {
            IntPtr ptr;
            INTERNAL_CALL_GetObjectArrayElement(array, index, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Returns the class of an object.</para>
        /// </summary>
        /// <param name="obj"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr GetObjectClass(IntPtr obj)
        {
            IntPtr ptr;
            INTERNAL_CALL_GetObjectClass(obj, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>This function returns the value of an instance (nonstatic) field of an object.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldID"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr GetObjectField(IntPtr obj, IntPtr fieldID)
        {
            IntPtr ptr;
            INTERNAL_CALL_GetObjectField(obj, fieldID, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Returns the value of one element of a primitive array.</para>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern short GetShortArrayElement(IntPtr array, int index);
        /// <summary>
        /// <para>This function returns the value of an instance (nonstatic) field of an object.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldID"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern short GetShortField(IntPtr obj, IntPtr fieldID);
        /// <summary>
        /// <para>This function returns the value of a static field of an object.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="fieldID"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern bool GetStaticBooleanField(IntPtr clazz, IntPtr fieldID);
        /// <summary>
        /// <para>This function returns the value of a static field of an object.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="fieldID"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern byte GetStaticByteField(IntPtr clazz, IntPtr fieldID);
        /// <summary>
        /// <para>This function returns the value of a static field of an object.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="fieldID"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern char GetStaticCharField(IntPtr clazz, IntPtr fieldID);
        /// <summary>
        /// <para>This function returns the value of a static field of an object.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="fieldID"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern double GetStaticDoubleField(IntPtr clazz, IntPtr fieldID);
        /// <summary>
        /// <para>Returns the field ID for a static field of a class.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="name"></param>
        /// <param name="sig"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr GetStaticFieldID(IntPtr clazz, string name, string sig)
        {
            IntPtr ptr;
            INTERNAL_CALL_GetStaticFieldID(clazz, name, sig, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>This function returns the value of a static field of an object.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="fieldID"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern float GetStaticFloatField(IntPtr clazz, IntPtr fieldID);
        /// <summary>
        /// <para>This function returns the value of a static field of an object.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="fieldID"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern int GetStaticIntField(IntPtr clazz, IntPtr fieldID);
        /// <summary>
        /// <para>This function returns the value of a static field of an object.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="fieldID"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern long GetStaticLongField(IntPtr clazz, IntPtr fieldID);
        /// <summary>
        /// <para>Returns the method ID for a static method of a class.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="name"></param>
        /// <param name="sig"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr GetStaticMethodID(IntPtr clazz, string name, string sig)
        {
            IntPtr ptr;
            INTERNAL_CALL_GetStaticMethodID(clazz, name, sig, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>This function returns the value of a static field of an object.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="fieldID"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr GetStaticObjectField(IntPtr clazz, IntPtr fieldID)
        {
            IntPtr ptr;
            INTERNAL_CALL_GetStaticObjectField(clazz, fieldID, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>This function returns the value of a static field of an object.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="fieldID"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern short GetStaticShortField(IntPtr clazz, IntPtr fieldID);
        /// <summary>
        /// <para>This function returns the value of a static field of an object.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="fieldID"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern string GetStaticStringField(IntPtr clazz, IntPtr fieldID);
        /// <summary>
        /// <para>This function returns the value of an instance (nonstatic) field of an object.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldID"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern string GetStringField(IntPtr obj, IntPtr fieldID);
        /// <summary>
        /// <para>Returns a managed string object representing the string in modified UTF-8 encoding.</para>
        /// </summary>
        /// <param name="str"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern string GetStringUTFChars(IntPtr str);
        /// <summary>
        /// <para>Returns the length in bytes of the modified UTF-8 representation of a string.</para>
        /// </summary>
        /// <param name="str"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern int GetStringUTFLength(IntPtr str);
        /// <summary>
        /// <para>If &lt;tt&gt;clazz&lt;tt&gt; represents any class other than the class &lt;tt&gt;Object&lt;tt&gt;, then this function returns the object that represents the superclass of the class specified by &lt;tt&gt;clazz&lt;/tt&gt;.</para>
        /// </summary>
        /// <param name="clazz"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr GetSuperclass(IntPtr clazz)
        {
            IntPtr ptr;
            INTERNAL_CALL_GetSuperclass(clazz, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Returns the version of the native method interface.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern int GetVersion();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_AllocObject(IntPtr clazz, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_CallObjectMethod(IntPtr obj, IntPtr methodID, jvalue[] args, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_CallStaticObjectMethod(IntPtr clazz, IntPtr methodID, jvalue[] args, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_ExceptionOccurred(out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_FindClass(string name, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_FromReflectedField(IntPtr refField, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_FromReflectedMethod(IntPtr refMethod, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetFieldID(IntPtr clazz, string name, string sig, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetMethodID(IntPtr clazz, string name, string sig, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetObjectArrayElement(IntPtr array, int index, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetObjectClass(IntPtr obj, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetObjectField(IntPtr obj, IntPtr fieldID, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetStaticFieldID(IntPtr clazz, string name, string sig, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetStaticMethodID(IntPtr clazz, string name, string sig, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetStaticObjectField(IntPtr clazz, IntPtr fieldID, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetSuperclass(IntPtr clazz, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_NewBooleanArray(int size, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_NewByteArray(int size, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_NewCharArray(int size, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_NewDoubleArray(int size, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_NewFloatArray(int size, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_NewGlobalRef(IntPtr obj, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_NewIntArray(int size, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_NewLocalRef(IntPtr obj, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_NewLongArray(int size, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_NewObject(IntPtr clazz, IntPtr methodID, jvalue[] args, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_NewObjectArray(int size, IntPtr clazz, IntPtr obj, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_NewShortArray(int size, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_NewStringUTF(string bytes, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_PopLocalFrame(IntPtr ptr, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_ToBooleanArray(bool[] array, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_ToByteArray(byte[] array, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_ToCharArray(char[] array, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_ToDoubleArray(double[] array, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_ToFloatArray(float[] array, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_ToIntArray(int[] array, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_ToLongArray(long[] array, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_ToObjectArray(IntPtr[] array, IntPtr arrayClass, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_ToReflectedField(IntPtr clazz, IntPtr fieldID, bool isStatic, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_ToReflectedMethod(IntPtr clazz, IntPtr methodID, bool isStatic, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_ToShortArray(short[] array, out IntPtr value);
        /// <summary>
        /// <para>Determines whether an object of &lt;tt&gt;clazz1&lt;tt&gt; can be safely cast to &lt;tt&gt;clazz2&lt;tt&gt;.</para>
        /// </summary>
        /// <param name="clazz1"></param>
        /// <param name="clazz2"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern bool IsAssignableFrom(IntPtr clazz1, IntPtr clazz2);
        /// <summary>
        /// <para>Tests whether an object is an instance of a class.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="clazz"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern bool IsInstanceOf(IntPtr obj, IntPtr clazz);
        /// <summary>
        /// <para>Tests whether two references refer to the same Java object.</para>
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern bool IsSameObject(IntPtr obj1, IntPtr obj2);
        /// <summary>
        /// <para>Construct a new primitive array object.</para>
        /// </summary>
        /// <param name="size"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr NewBooleanArray(int size)
        {
            IntPtr ptr;
            INTERNAL_CALL_NewBooleanArray(size, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Construct a new primitive array object.</para>
        /// </summary>
        /// <param name="size"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr NewByteArray(int size)
        {
            IntPtr ptr;
            INTERNAL_CALL_NewByteArray(size, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Construct a new primitive array object.</para>
        /// </summary>
        /// <param name="size"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr NewCharArray(int size)
        {
            IntPtr ptr;
            INTERNAL_CALL_NewCharArray(size, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Construct a new primitive array object.</para>
        /// </summary>
        /// <param name="size"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr NewDoubleArray(int size)
        {
            IntPtr ptr;
            INTERNAL_CALL_NewDoubleArray(size, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Construct a new primitive array object.</para>
        /// </summary>
        /// <param name="size"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr NewFloatArray(int size)
        {
            IntPtr ptr;
            INTERNAL_CALL_NewFloatArray(size, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Creates a new global reference to the object referred to by the &lt;tt&gt;obj&lt;/tt&gt; argument.</para>
        /// </summary>
        /// <param name="obj"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr NewGlobalRef(IntPtr obj)
        {
            IntPtr ptr;
            INTERNAL_CALL_NewGlobalRef(obj, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Construct a new primitive array object.</para>
        /// </summary>
        /// <param name="size"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr NewIntArray(int size)
        {
            IntPtr ptr;
            INTERNAL_CALL_NewIntArray(size, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Creates a new local reference that refers to the same object as &lt;tt&gt;obj&lt;/tt&gt;.</para>
        /// </summary>
        /// <param name="obj"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr NewLocalRef(IntPtr obj)
        {
            IntPtr ptr;
            INTERNAL_CALL_NewLocalRef(obj, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Construct a new primitive array object.</para>
        /// </summary>
        /// <param name="size"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr NewLongArray(int size)
        {
            IntPtr ptr;
            INTERNAL_CALL_NewLongArray(size, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Constructs a new Java object. The method ID indicates which constructor method to invoke. This ID must be obtained by calling GetMethodID() with &lt;init&gt; as the method name and void (V) as the return type.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="methodID"></param>
        /// <param name="args"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr NewObject(IntPtr clazz, IntPtr methodID, jvalue[] args)
        {
            IntPtr ptr;
            INTERNAL_CALL_NewObject(clazz, methodID, args, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Constructs a new array holding objects in class &lt;tt&gt;clazz&lt;tt&gt;. All elements are initially set to &lt;tt&gt;obj&lt;tt&gt;.</para>
        /// </summary>
        /// <param name="size"></param>
        /// <param name="clazz"></param>
        /// <param name="obj"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr NewObjectArray(int size, IntPtr clazz, IntPtr obj)
        {
            IntPtr ptr;
            INTERNAL_CALL_NewObjectArray(size, clazz, obj, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Construct a new primitive array object.</para>
        /// </summary>
        /// <param name="size"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr NewShortArray(int size)
        {
            IntPtr ptr;
            INTERNAL_CALL_NewShortArray(size, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Constructs a new &lt;tt&gt;java.lang.String&lt;/tt&gt; object from an array of characters in modified UTF-8 encoding.</para>
        /// </summary>
        /// <param name="bytes"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr NewStringUTF(string bytes)
        {
            IntPtr ptr;
            INTERNAL_CALL_NewStringUTF(bytes, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Pops off the current local reference frame, frees all the local references, and returns a local reference in the previous local reference frame for the given &lt;tt&gt;result&lt;/tt&gt; object.</para>
        /// </summary>
        /// <param name="ptr"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr PopLocalFrame(IntPtr ptr)
        {
            IntPtr ptr2;
            INTERNAL_CALL_PopLocalFrame(ptr, out ptr2);
            return ptr2;
        }

        /// <summary>
        /// <para>Creates a new local reference frame, in which at least a given number of local references can be created.</para>
        /// </summary>
        /// <param name="capacity"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern int PushLocalFrame(int capacity);
        /// <summary>
        /// <para>Sets the value of one element in a primitive array.</para>
        /// </summary>
        /// <param name="array">The array of native booleans.</param>
        /// <param name="index">Index of the array element to set.</param>
        /// <param name="val">The value to set - for 'true' use 1, for 'false' use 0.</param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetBooleanArrayElement(IntPtr array, int index, byte val);
        /// <summary>
        /// <para>This function sets the value of an instance (nonstatic) field of an object.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldID"></param>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetBooleanField(IntPtr obj, IntPtr fieldID, bool val);
        /// <summary>
        /// <para>Sets the value of one element in a primitive array.</para>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetByteArrayElement(IntPtr array, int index, sbyte val);
        /// <summary>
        /// <para>This function sets the value of an instance (nonstatic) field of an object.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldID"></param>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetByteField(IntPtr obj, IntPtr fieldID, byte val);
        /// <summary>
        /// <para>Sets the value of one element in a primitive array.</para>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetCharArrayElement(IntPtr array, int index, char val);
        /// <summary>
        /// <para>This function sets the value of an instance (nonstatic) field of an object.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldID"></param>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetCharField(IntPtr obj, IntPtr fieldID, char val);
        /// <summary>
        /// <para>Sets the value of one element in a primitive array.</para>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetDoubleArrayElement(IntPtr array, int index, double val);
        /// <summary>
        /// <para>This function sets the value of an instance (nonstatic) field of an object.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldID"></param>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetDoubleField(IntPtr obj, IntPtr fieldID, double val);
        /// <summary>
        /// <para>Sets the value of one element in a primitive array.</para>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetFloatArrayElement(IntPtr array, int index, float val);
        /// <summary>
        /// <para>This function sets the value of an instance (nonstatic) field of an object.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldID"></param>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetFloatField(IntPtr obj, IntPtr fieldID, float val);
        /// <summary>
        /// <para>Sets the value of one element in a primitive array.</para>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetIntArrayElement(IntPtr array, int index, int val);
        /// <summary>
        /// <para>This function sets the value of an instance (nonstatic) field of an object.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldID"></param>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetIntField(IntPtr obj, IntPtr fieldID, int val);
        /// <summary>
        /// <para>Sets the value of one element in a primitive array.</para>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetLongArrayElement(IntPtr array, int index, long val);
        /// <summary>
        /// <para>This function sets the value of an instance (nonstatic) field of an object.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldID"></param>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetLongField(IntPtr obj, IntPtr fieldID, long val);
        /// <summary>
        /// <para>Sets an element of an &lt;tt&gt;Object&lt;/tt&gt; array.</para>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="obj"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetObjectArrayElement(IntPtr array, int index, IntPtr obj);
        /// <summary>
        /// <para>This function sets the value of an instance (nonstatic) field of an object.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldID"></param>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetObjectField(IntPtr obj, IntPtr fieldID, IntPtr val);
        /// <summary>
        /// <para>Sets the value of one element in a primitive array.</para>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetShortArrayElement(IntPtr array, int index, short val);
        /// <summary>
        /// <para>This function sets the value of an instance (nonstatic) field of an object.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldID"></param>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetShortField(IntPtr obj, IntPtr fieldID, short val);
        /// <summary>
        /// <para>This function ets the value of a static field of an object.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="fieldID"></param>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetStaticBooleanField(IntPtr clazz, IntPtr fieldID, bool val);
        /// <summary>
        /// <para>This function ets the value of a static field of an object.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="fieldID"></param>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetStaticByteField(IntPtr clazz, IntPtr fieldID, byte val);
        /// <summary>
        /// <para>This function ets the value of a static field of an object.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="fieldID"></param>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetStaticCharField(IntPtr clazz, IntPtr fieldID, char val);
        /// <summary>
        /// <para>This function ets the value of a static field of an object.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="fieldID"></param>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetStaticDoubleField(IntPtr clazz, IntPtr fieldID, double val);
        /// <summary>
        /// <para>This function ets the value of a static field of an object.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="fieldID"></param>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetStaticFloatField(IntPtr clazz, IntPtr fieldID, float val);
        /// <summary>
        /// <para>This function ets the value of a static field of an object.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="fieldID"></param>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetStaticIntField(IntPtr clazz, IntPtr fieldID, int val);
        /// <summary>
        /// <para>This function ets the value of a static field of an object.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="fieldID"></param>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetStaticLongField(IntPtr clazz, IntPtr fieldID, long val);
        /// <summary>
        /// <para>This function ets the value of a static field of an object.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="fieldID"></param>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetStaticObjectField(IntPtr clazz, IntPtr fieldID, IntPtr val);
        /// <summary>
        /// <para>This function ets the value of a static field of an object.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="fieldID"></param>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetStaticShortField(IntPtr clazz, IntPtr fieldID, short val);
        /// <summary>
        /// <para>This function ets the value of a static field of an object.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="fieldID"></param>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetStaticStringField(IntPtr clazz, IntPtr fieldID, string val);
        /// <summary>
        /// <para>This function sets the value of an instance (nonstatic) field of an object.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldID"></param>
        /// <param name="val"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetStringField(IntPtr obj, IntPtr fieldID, string val);
        /// <summary>
        /// <para>Causes a &lt;tt&gt;java.lang.Throwable&lt;/tt&gt; object to be thrown.</para>
        /// </summary>
        /// <param name="obj"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern int Throw(IntPtr obj);
        /// <summary>
        /// <para>Constructs an exception object from the specified class with the &lt;tt&gt;message&lt;/tt&gt; specified by message and causes that exception to be thrown.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="message"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern int ThrowNew(IntPtr clazz, string message);
        /// <summary>
        /// <para>Convert a managed array of System.Boolean to a Java array of &lt;tt&gt;boolean&lt;/tt&gt;.</para>
        /// </summary>
        /// <param name="array"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr ToBooleanArray(bool[] array)
        {
            IntPtr ptr;
            INTERNAL_CALL_ToBooleanArray(array, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Convert a managed array of System.Byte to a Java array of &lt;tt&gt;byte&lt;/tt&gt;.</para>
        /// </summary>
        /// <param name="array"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr ToByteArray(byte[] array)
        {
            IntPtr ptr;
            INTERNAL_CALL_ToByteArray(array, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Convert a managed array of System.Char to a Java array of &lt;tt&gt;char&lt;/tt&gt;.</para>
        /// </summary>
        /// <param name="array"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr ToCharArray(char[] array)
        {
            IntPtr ptr;
            INTERNAL_CALL_ToCharArray(array, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Convert a managed array of System.Double to a Java array of &lt;tt&gt;double&lt;/tt&gt;.</para>
        /// </summary>
        /// <param name="array"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr ToDoubleArray(double[] array)
        {
            IntPtr ptr;
            INTERNAL_CALL_ToDoubleArray(array, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Convert a managed array of System.Single to a Java array of &lt;tt&gt;float&lt;/tt&gt;.</para>
        /// </summary>
        /// <param name="array"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr ToFloatArray(float[] array)
        {
            IntPtr ptr;
            INTERNAL_CALL_ToFloatArray(array, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Convert a managed array of System.Int32 to a Java array of &lt;tt&gt;int&lt;/tt&gt;.</para>
        /// </summary>
        /// <param name="array"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr ToIntArray(int[] array)
        {
            IntPtr ptr;
            INTERNAL_CALL_ToIntArray(array, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Convert a managed array of System.Int64 to a Java array of &lt;tt&gt;long&lt;/tt&gt;.</para>
        /// </summary>
        /// <param name="array"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr ToLongArray(long[] array)
        {
            IntPtr ptr;
            INTERNAL_CALL_ToLongArray(array, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Convert a managed array of System.IntPtr, representing Java objects, to a Java array of &lt;tt&gt;java.lang.Object&lt;/tt&gt;.</para>
        /// </summary>
        /// <param name="array"></param>
        public static IntPtr ToObjectArray(IntPtr[] array)
        {
            return ToObjectArray(array, IntPtr.Zero);
        }

        [ThreadAndSerializationSafe]
        public static IntPtr ToObjectArray(IntPtr[] array, IntPtr arrayClass)
        {
            IntPtr ptr;
            INTERNAL_CALL_ToObjectArray(array, arrayClass, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Converts a field ID derived from cls to a &lt;tt&gt;java.lang.reflect.Field&lt;/tt&gt; object.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="fieldID"></param>
        /// <param name="isStatic"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr ToReflectedField(IntPtr clazz, IntPtr fieldID, bool isStatic)
        {
            IntPtr ptr;
            INTERNAL_CALL_ToReflectedField(clazz, fieldID, isStatic, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Converts a method ID derived from clazz to a &lt;tt&gt;java.lang.reflect.Method&lt;tt&gt; or &lt;tt&gt;java.lang.reflect.Constructor&lt;tt&gt; object.</para>
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="methodID"></param>
        /// <param name="isStatic"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr ToReflectedMethod(IntPtr clazz, IntPtr methodID, bool isStatic)
        {
            IntPtr ptr;
            INTERNAL_CALL_ToReflectedMethod(clazz, methodID, isStatic, out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Convert a managed array of System.Int16 to a Java array of &lt;tt&gt;short&lt;/tt&gt;.</para>
        /// </summary>
        /// <param name="array"></param>
        [ThreadAndSerializationSafe]
        public static IntPtr ToShortArray(short[] array)
        {
            IntPtr ptr;
            INTERNAL_CALL_ToShortArray(array, out ptr);
            return ptr;
        }
    }
}

