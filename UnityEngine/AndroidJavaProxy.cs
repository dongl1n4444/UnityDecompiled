namespace UnityEngine
{
    using System;
    using System.Reflection;

    /// <summary>
    /// <para>This class can be used to implement any java interface. Any java vm method invocation matching the interface on the proxy object will automatically be passed to the c# implementation.</para>
    /// </summary>
    public class AndroidJavaProxy
    {
        /// <summary>
        /// <para>Java interface implemented by the proxy.</para>
        /// </summary>
        public readonly AndroidJavaClass javaInterface;

        /// <summary>
        /// <para></para>
        /// </summary>
        /// <param name="javaInterface">Java interface to be implemented by the proxy.</param>
        public AndroidJavaProxy(string javaInterface) : this(new AndroidJavaClass(javaInterface))
        {
        }

        /// <summary>
        /// <para></para>
        /// </summary>
        /// <param name="javaInterface">Java interface to be implemented by the proxy.</param>
        public AndroidJavaProxy(AndroidJavaClass javaInterface)
        {
            this.javaInterface = javaInterface;
        }

        /// <summary>
        /// <para>Called by the java vm whenever a method is invoked on the java proxy interface. You can override this to run special code on method invokation, or you can leave the implementation as is, and leave the default behavior which is to look for c# methods matching the signature of the java method.</para>
        /// </summary>
        /// <param name="methodName">Name of the invoked java method.</param>
        /// <param name="args">Arguments passed from the java vm - converted into AndroidJavaObject, AndroidJavaClass or a primitive.</param>
        /// <param name="javaArgs">Arguments passed from the java vm - all objects are represented by AndroidJavaObject, int for instance is represented by a java.lang.Integer object.</param>
        public virtual AndroidJavaObject Invoke(string methodName, object[] args)
        {
            Exception inner = null;
            BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
            System.Type[] types = new System.Type[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                types[i] = (args[i] != null) ? args[i].GetType() : typeof(AndroidJavaObject);
            }
            try
            {
                MethodInfo info = base.GetType().GetMethod(methodName, bindingAttr, null, types, null);
                if (info != null)
                {
                    return _AndroidJNIHelper.Box(info.Invoke(this, args));
                }
            }
            catch (TargetInvocationException exception2)
            {
                inner = exception2.InnerException;
            }
            catch (Exception exception3)
            {
                inner = exception3;
            }
            string[] strArray = new string[args.Length];
            for (int j = 0; j < types.Length; j++)
            {
                strArray[j] = types[j].ToString();
            }
            if (inner != null)
            {
                object[] objArray1 = new object[] { base.GetType(), ".", methodName, "(", string.Join(",", strArray), ")" };
                throw new TargetInvocationException(string.Concat(objArray1), inner);
            }
            object[] objArray2 = new object[] { "No such proxy method: ", base.GetType(), ".", methodName, "(", string.Join(",", strArray), ")" };
            throw new Exception(string.Concat(objArray2));
        }

        /// <summary>
        /// <para>Called by the java vm whenever a method is invoked on the java proxy interface. You can override this to run special code on method invokation, or you can leave the implementation as is, and leave the default behavior which is to look for c# methods matching the signature of the java method.</para>
        /// </summary>
        /// <param name="methodName">Name of the invoked java method.</param>
        /// <param name="args">Arguments passed from the java vm - converted into AndroidJavaObject, AndroidJavaClass or a primitive.</param>
        /// <param name="javaArgs">Arguments passed from the java vm - all objects are represented by AndroidJavaObject, int for instance is represented by a java.lang.Integer object.</param>
        public virtual AndroidJavaObject Invoke(string methodName, AndroidJavaObject[] javaArgs)
        {
            object[] args = new object[javaArgs.Length];
            for (int i = 0; i < javaArgs.Length; i++)
            {
                args[i] = _AndroidJNIHelper.Unbox(javaArgs[i]);
            }
            return this.Invoke(methodName, args);
        }
    }
}

