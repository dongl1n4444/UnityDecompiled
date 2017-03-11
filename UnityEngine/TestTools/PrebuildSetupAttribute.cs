namespace UnityEngine.TestTools
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// <para>Allows to define a setup method for the test that will be invoked before the test run is started.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class PrebuildSetupAttribute : Attribute
    {
        [CompilerGenerated]
        private static Func<ConstructorInfo, bool> <>f__am$cache0;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private System.Type <SetupClass>k__BackingField;

        public PrebuildSetupAttribute(string setupClass)
        {
            this.SetupClass = System.Type.GetType(setupClass);
            if (this.SetupClass == null)
            {
                this.SetupClass = System.Type.GetType(setupClass + ",Assembly-CSharp");
            }
            if (this.SetupClass == null)
            {
                this.SetupClass = System.Type.GetType(setupClass + ",Assembly-CSharp-Editor");
            }
            if (this.SetupClass == null)
            {
                UnityEngine.Debug.LogWarningFormat("Class type not found: " + setupClass, new object[0]);
            }
            else
            {
                this.ValidateSetupClass();
            }
        }

        /// <summary>
        /// <para>Points to a class that imlpements TestTools.IPrebuildSetup. The method from TestTools.IPrebuildSetup will be executed before the test run is initiated.</para>
        /// </summary>
        /// <param name="setupClass">Type of the class implementing TestTools.IPrebuildSetup.</param>
        public PrebuildSetupAttribute(System.Type setupClass)
        {
            this.SetupClass = setupClass;
            this.ValidateSetupClass();
        }

        private void ValidateSetupClass()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = constructor => constructor.GetParameters().Length != 0;
            }
            if (Enumerable.All<ConstructorInfo>(this.SetupClass.GetConstructors(), <>f__am$cache0))
            {
                object[] args = new object[] { this.SetupClass.Name };
                UnityEngine.Debug.LogWarningFormat("{0} does not implement default constructor", args);
            }
            if (!typeof(IPrebuildSetup).IsAssignableFrom(this.SetupClass))
            {
                object[] objArray2 = new object[] { this.SetupClass.Name, typeof(IPrebuildSetup).Name };
                UnityEngine.Debug.LogWarningFormat("{0} does not implement {1}", objArray2);
            }
        }

        internal System.Type SetupClass { get; private set; }
    }
}

