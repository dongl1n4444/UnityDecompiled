namespace UnityEngine
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Allow an runtime class method to be initialized when Unity game loads runtime without action from the user.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    public class RuntimeInitializeOnLoadMethodAttribute : PreserveAttribute
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private RuntimeInitializeLoadType <loadType>k__BackingField;

        /// <summary>
        /// <para>Allow an runtime class method to be initialized when Unity game loads runtime without action from the user.</para>
        /// </summary>
        /// <param name="loadType">RuntimeInitializeLoadType: Before or After scene is loaded.</param>
        public RuntimeInitializeOnLoadMethodAttribute()
        {
            this.loadType = RuntimeInitializeLoadType.AfterSceneLoad;
        }

        /// <summary>
        /// <para>Allow an runtime class method to be initialized when Unity game loads runtime without action from the user.</para>
        /// </summary>
        /// <param name="loadType">RuntimeInitializeLoadType: Before or After scene is loaded.</param>
        public RuntimeInitializeOnLoadMethodAttribute(RuntimeInitializeLoadType loadType)
        {
            this.loadType = loadType;
        }

        /// <summary>
        /// <para>Set RuntimeInitializeOnLoadMethod type.</para>
        /// </summary>
        public RuntimeInitializeLoadType loadType { get; private set; }
    }
}

