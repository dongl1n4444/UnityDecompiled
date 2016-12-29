namespace UnityScript.Scripting
{
    using Boo.Lang;
    using System;

    [Serializable]
    public class EvaluationContext
    {
        private bool $initialized__UnityScript_Scripting_EvaluationContext$;
        private List _activeScripts;
        private IEvaluationDomainProvider _container;

        public EvaluationContext()
        {
            if (!this.$initialized__UnityScript_Scripting_EvaluationContext$)
            {
                this._activeScripts = new List();
                this.$initialized__UnityScript_Scripting_EvaluationContext$ = true;
            }
            this._container = new SimpleEvaluationDomainProvider();
        }

        public EvaluationContext(IEvaluationDomainProvider container)
        {
            if (!this.$initialized__UnityScript_Scripting_EvaluationContext$)
            {
                this._activeScripts = new List();
                this.$initialized__UnityScript_Scripting_EvaluationContext$ = true;
            }
            if (container <= null)
            {
            }
            this._container = new SimpleEvaluationDomainProvider();
        }

        public void AddScript(object script)
        {
            EvaluationContext context = this;
            lock (context)
            {
                this._activeScripts.Add(script);
            }
        }

        public object GetActiveScript(int scriptId)
        {
            EvaluationContext context = this;
            lock (context)
            {
                return this._activeScripts[scriptId];
            }
        }

        public int GetActiveScriptId(object script)
        {
            EvaluationContext context = this;
            lock (context)
            {
                return this._activeScripts.IndexOf(script);
            }
        }

        public object[] GetActiveScripts()
        {
            EvaluationContext context = this;
            lock (context)
            {
                return this._activeScripts.ToArray();
            }
        }

        public bool IsStaticContext =>
            (this._container is SimpleEvaluationDomainProvider);

        public IEvaluationDomainProvider ScriptContainer =>
            this._container;
    }
}

