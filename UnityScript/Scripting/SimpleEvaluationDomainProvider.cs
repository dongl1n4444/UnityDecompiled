namespace UnityScript.Scripting
{
    using System;
    using System.Reflection;

    [Serializable]
    public class SimpleEvaluationDomainProvider : IEvaluationDomainProvider
    {
        private bool $initialized__UnityScript_Scripting_SimpleEvaluationDomainProvider$;
        private EvaluationDomain _domain;
        private string[] _imports;

        public SimpleEvaluationDomainProvider()
        {
            if (!this.$initialized__UnityScript_Scripting_SimpleEvaluationDomainProvider$)
            {
                this._domain = new EvaluationDomain();
                this.$initialized__UnityScript_Scripting_SimpleEvaluationDomainProvider$ = true;
            }
        }

        public SimpleEvaluationDomainProvider(params string[] imports)
        {
            if (!this.$initialized__UnityScript_Scripting_SimpleEvaluationDomainProvider$)
            {
                this._domain = new EvaluationDomain();
                this.$initialized__UnityScript_Scripting_SimpleEvaluationDomainProvider$ = true;
            }
            this._imports = imports;
        }

        public override Assembly[] GetAssemblyReferences() => 
            new Assembly[0];

        public override EvaluationDomain GetEvaluationDomain() => 
            this._domain;

        public override string[] GetImports() => 
            this._imports;
    }
}

