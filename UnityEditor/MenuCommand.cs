namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Used to extract the context for a MenuItem. MenuCommand objects are passed to custom menu item functions defined using the MenuItem attribute.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public sealed class MenuCommand
    {
        /// <summary>
        /// <para>Context is the object that is the target of a menu command.</para>
        /// </summary>
        public Object context;
        /// <summary>
        /// <para>An integer for passing custom information to a menu item.</para>
        /// </summary>
        public int userData;
        /// <summary>
        /// <para>Creates a new MenuCommand object.</para>
        /// </summary>
        /// <param name="inContext"></param>
        /// <param name="inUserData"></param>
        public MenuCommand(Object inContext, int inUserData)
        {
            this.context = inContext;
            this.userData = inUserData;
        }

        /// <summary>
        /// <para>Creates a new MenuCommand object.</para>
        /// </summary>
        /// <param name="inContext"></param>
        public MenuCommand(Object inContext)
        {
            this.context = inContext;
            this.userData = 0;
        }
    }
}

