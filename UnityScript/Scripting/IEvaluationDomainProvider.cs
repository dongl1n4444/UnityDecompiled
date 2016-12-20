namespace UnityScript.Scripting
{
    using System;
    using System.Reflection;

    public interface IEvaluationDomainProvider
    {
        Assembly[] GetAssemblyReferences();
        EvaluationDomain GetEvaluationDomain();
        string[] GetImports();
    }
}

