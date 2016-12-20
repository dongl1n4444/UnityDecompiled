namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Allow an editor class method to be initialized when Unity loads without action from the user.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class InitializeOnLoadMethodAttribute : Attribute
    {
    }
}

