namespace UnityScriptUpdater
{
    using BooUpdater;
    using System;

    internal class UnityScriptDepricatedComponentPropertyGetterReplacer : DepricatedComponentPropertyGetterReplacer
    {
        public UnityScriptDepricatedComponentPropertyGetterReplacer(BooUpdateContext contex) : base(contex)
        {
        }

        protected override string ReplacementMethodCallFor(string type)
        {
            return ("GetComponent.<" + type + ">()");
        }
    }
}

