namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Allow an editor class to be initialized when Unity loads without action from the user.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class InitializeOnLoadAttribute : Attribute
    {
    }
}

