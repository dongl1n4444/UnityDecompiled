namespace UnityScriptUpdater
{
    using BooUpdater;
    using System;

    public class StringBasedAddComponentReplacer : StringBasedAddComponentReplacer
    {
        public StringBasedAddComponentReplacer(BooUpdateContext context) : base(context)
        {
        }

        protected override string GenericMethodSyntaxFor(string methodName, string typeName)
        {
            return (methodName + ".<" + typeName + ">");
        }
    }
}

