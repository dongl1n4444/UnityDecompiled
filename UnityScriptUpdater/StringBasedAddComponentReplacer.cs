namespace UnityScriptUpdater
{
    using BooUpdater;
    using System;

    public class StringBasedAddComponentReplacer : BooUpdater.StringBasedAddComponentReplacer
    {
        public StringBasedAddComponentReplacer(BooUpdateContext context) : base(context)
        {
        }

        protected override string GenericMethodSyntaxFor(string methodName, string typeName) => 
            (methodName + ".<" + typeName + ">");
    }
}

