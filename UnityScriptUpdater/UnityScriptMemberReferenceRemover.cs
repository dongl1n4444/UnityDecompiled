namespace UnityScriptUpdater
{
    using BooUpdater;
    using System;

    public class UnityScriptMemberReferenceRemover : MemberReferenceRemover
    {
        public UnityScriptMemberReferenceRemover(BooUpdateContext context) : base(context)
        {
            base.LanguageTraits = new UnityScriptLanguageTraits();
        }
    }
}

